//----------------------------------Moved from page Start-----------------------------

var oTable;
var IsRefreshGrid = false;
var objColumns = {}
var ImageIDToOpen = "";

var oLanguage = {
    "sLengthMenu": sLengthMenu,
    "sEmptyTable": sEmptyTable,
    "sInfo": sInfo,
    "sInfoEmpty": sInfoEmpty
};

var _ReceiveList = (function ($) {
    var self = {};

    self.urls = {
        saveGridStateURL: null, receiveItemURL: null, receiveListAjaxURL: null, loadGridStateURL: null
        , deleteRecieveAndUpdateReceivedQtyURL: null, openDialogForEditReceiptURL: null,
        getItemLocationsURL: null, getReceivedQuantityURL: null, saveReceiveWithOrderURL: null
        , closeOrderDetailLineItemsURL: null, unCloseOrderLineItemsURL: null, receivedItemDetailURL: null
        , duplicateCheckSrNumberURL: null
    };

    self.roomDateJSFormat = null;
    self.isUnclose = null;
    self.recvDate = null;
    self.model = { companyId: 0, roomId: 0, };
    self.isSaveGridState = false;
    self.isGetReplinshRedCount = true;
    self.isGetUDFEditableOptionsByUDF = true;

    self.init = function (companyId, roomId, roomDateJSFormat, isUnclose, recvDate) {

        self.model.companyId = companyId;
        self.model.roomId = roomId ;

        //self.urls.saveGridStateURL = saveGridStateURL;
        //self.urls.receiveItemURL = receiveItemURL;
        //self.urls.receiveListAjaxURL = receiveListAjaxURL;
        //self.urls.loadGridStateURL = loadGridStateURL;
        //self.urls.deleteRecieveAndUpdateReceivedQtyURL = deleteRecieveAndUpdateReceivedQtyURL;
        //self.urls.openDialogForEditReceiptURL = openDialogForEditReceiptURL;
        //self.urls.getItemLocationsURL = getItemLocationsURL;
        //self.urls.getReceivedQuantityURL = getReceivedQuantityURL;
        //self.urls.saveReceiveWithOrderURL = saveReceiveWithOrderURL;
        //self.urls.closeOrderDetailLineItemsURL = closeOrderDetailLineItemsURL;
        //self.urls.receivedItemDetailURL = receivedItemDetailURL;
        //self.urls.unCloseOrderLineItemsURL = unCloseOrderLineItemsURL;
        //self.urls.duplicateCheckSrNumberURL = duplicateCheckSrNumberURL;

        self.isUnclose = isUnclose;
        self.roomDateJSFormat = roomDateJSFormat;
        self.recvDate = recvDate;
        
        self.initEvents();

    }

    self.initUrls = function (saveGridStateURL, receiveItemURL, receiveListAjaxURL, loadGridStateURL
        , deleteRecieveAndUpdateReceivedQtyURL, openDialogForEditReceiptURL,
        getItemLocationsURL, getReceivedQuantityURL,
        saveReceiveWithOrderURL, closeOrderDetailLineItemsURL,
        receivedItemDetailURL, unCloseOrderLineItemsURL, duplicateCheckSrNumberURL, backOrderedDetails
    ) {
        self.urls.saveGridStateURL = saveGridStateURL;
        self.urls.receiveItemURL = receiveItemURL;
        self.urls.receiveListAjaxURL = receiveListAjaxURL;
        self.urls.loadGridStateURL = loadGridStateURL;
        self.urls.deleteRecieveAndUpdateReceivedQtyURL = deleteRecieveAndUpdateReceivedQtyURL;
        self.urls.openDialogForEditReceiptURL = openDialogForEditReceiptURL;
        self.urls.getItemLocationsURL = getItemLocationsURL;
        self.urls.getReceivedQuantityURL = getReceivedQuantityURL;
        self.urls.saveReceiveWithOrderURL = saveReceiveWithOrderURL;
        self.urls.closeOrderDetailLineItemsURL = closeOrderDetailLineItemsURL;
        self.urls.receivedItemDetailURL = receivedItemDetailURL;
        self.urls.unCloseOrderLineItemsURL = unCloseOrderLineItemsURL;
        self.urls.duplicateCheckSrNumberURL = duplicateCheckSrNumberURL;
        self.urls.backOrderedDetails = backOrderedDetails;
    }

    self.initEvents = function () {
        $(document).ready(function () {
            $('#DivLoading').show();
            // init dialogues
            var $divEditReceipt = $('#divEditReceipt');

            $divEditReceipt.dialog({
                autoOpen: false,
                modal: true,
                draggable: true,
                resizable: true,
                width: '90%',
                height: 400,
                title: EditReceipts,
                open: function () {
                    //$('#DivLoading').show();
                    _ReceiveList.showHideLoader(true);
                    var itemGuid = $(this).data("ItemGuid");
                    var orderDetailGuid = $(this).data("OrderDetailGuid");
                    //$.ajax({
                    //    url: _ReceiveList.urls.openDialogForEditReceiptUrl,
                    //    data: { 'ItemGuid': itemGuid, 'OrderDetailGuid': orderDetailGuid },
                    //    type: 'Post',
                    //    "async": false,
                    //    "cache": false,
                    //    "dataType": "text",
                    //    success: function (result) {
                    //        $('#DivLoading').hide();
                    //        $('#divEditReceipt').html(result)
                    //    },
                    //    error: function (xhr) {
                    //        $('#DivLoading').hide();
                    //    }

                    //});
                    _AjaxUtil.postText(_ReceiveList.urls.openDialogForEditReceiptURL, { 'ItemGuid': itemGuid, 'OrderDetailGuid': orderDetailGuid }
                        , function (result) {
                            //$('#DivLoading').hide();
                            _ReceiveList.showHideLoader(false);
                            $divEditReceipt.html(result)
                        }
                        , function (xhr) {
                            //$('#DivLoading').hide();
                            _ReceiveList.showHideLoader(false);
                        }, false, false, false);
                },
                close: function () {
                    $(this).empty();
                    $divEditReceipt.empty();
                    //$('#DivLoading').hide();
                    _ReceiveList.showHideLoader(false);
                    if ($(this).data("Success")) {
                        $(this).data({ "Success": false })
                        $('#myDataTable').dataTable().fnStandingRedraw();
                    }

                }
            });

            // init click
            $("#btnClearAll").on("click", function () {
                $('#myDataTable').find("tbody tr").each(function (e) {
                    var colr = hexc($(this).css('background-color'));
                    if (colr !== '#d3d3d3') {
                        $(this).removeAttr('style');
                    };
                    $(this).removeClass("row_selected");
                });
                //$("#myDataTable").find("tbody tr").removeAttr("style");
                // $("#myDataTable").find("tbody tr").removeClass("row_selected");
                $("#btnReceiveALL").removeAttr('disabled');
                $('#btnEditReciept').css('display', 'none');
                $('#DivLoading').hide();
            });

            $("#btnReceiveALL").on("click", function (e) {
                //setTimeout(function () { $('#DivLoading').show(); }, 300);
                $(this).attr('disabled', 'disabled');
                $('#DivLoading').show();
                $('#OrdReceiveProcessing').modal();
                $('#OrdReceiveProcessing').parent().parent().find(".modalCloseImg").css('display', 'none');
                setTimeout(function () {
                    var ItemSelected = fnGetSelected(oTable);
                    var IsRecivedExceed = false;
                    var ItemsList = '';

                    if (!isNaN(ItemSelected.length) && ItemSelected.length > 0) {
                        for (var i = 0; i < ItemSelected.length; i++) {
                            var rowPosition = oTable.fnGetPosition(ItemSelected[i]);
                            var aData = oTable.fnGetData(rowPosition);
                            var approvedQty = aData.ApprovedQuantity;
                            var receivedQty = aData.ReceivedQuantity;
                            var qty = $(ItemSelected[i]).find("input[type='text'][id^='txtQtyToRecv']").val();
                            var itemNumber = aData.ItemNumber;
                            if (!isNaN(parseFloat(approvedQty)) && parseFloat(approvedQty) <= 0) {
                                approvedQty = 0;
                            }
                            if (!isNaN(parseFloat(receivedQty)) && parseFloat(receivedQty) <= 0) {
                                receivedQty = 0;
                            }
                            if (!isNaN(parseFloat(qty)) && parseFloat(qty) <= 0) {
                                qty = 0;
                            }
                            var totalReceive = parseFloat(receivedQty) + parseFloat(qty);

                            if (parseFloat(totalReceive) > parseFloat(approvedQty)) {
                                IsRecivedExceed = true;
                                ItemsList += parseInt(i + 1) + ') ' + itemNumber + '<br />';
                            }
                        }
                    }
                    else {
                        $('#DivLoading').hide();
                        ErrorMessage = "<b>" + MsgSelectRowToReceive+" </b>";
                        $('#OrdReceivedInfoDialog').find("#OrdReceivedMSG").html(ErrorMessage);
                        closeOrdReceiveInfoModel();
                        $("#btnReceiveALL").removeAttr('disabled');
                        $('#OrdReceivedInfoDialog').modal();
                    }

                    if (IsRecivedExceed) {
                        closeOrdReceiveInfoModel();
                        $('#ExceedRecieveInfoDialog').modal();
                        $('#pItemList').html(ItemsList);
                        $('#DivLoading').hide();
                        $("#btnReceiveALL").removeAttr('disabled');
                        $('#DivLoading').hide();
                        return false;
                    }
                    else {
                        ReceiveAll()
                    }
                }, 1000);
            });

            $('#btnCloseOrderLineItem').click(function () {
                if ($('#myDataTable').find("tbody tr.row_selected").length > 0) {
                    $('#CloseOderLineItemDialog').modal();
                }
                else {
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    $("#spanGlobalMessage").html(MsgSelectLineItem);
                    showNotificationDialog();
                    //alert(MsgSelectLineItem);
                }
            });

            $('#btnUnCloseLineItem').click(function () {
                if ($('#myDataTable').find("tbody tr.row_selected").length > 0) {
                    $('#UnCloseOderLineItemDialog').modal();
                }
                else {
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    $("#spanGlobalMessage").html(MsgSelectClosedLineItem);
                    showNotificationDialog();
                    //alert(MsgSelectClosedLineItem);
                }
            });

            $('#bntCloseLineItemConfirmYes').click(function () {
                $('#DivLoading').show();
                var orderDetailID = '';

                $('#myDataTable').find("tbody tr.row_selected").each(function (e) {
                    if (orderDetailID.length > 0) {
                        orderDetailID = orderDetailID + ",";
                    }
                    orderDetailID = orderDetailID + $.trim($(this).find('#spnOrderDetailID').text());
                });
                if (orderDetailID.length > 0) {
                    //alert(orderDetailID);

                    _AjaxUtil.getJson(_ReceiveList.urls.closeOrderDetailLineItemsURL, { 'ids': orderDetailID, 'CallFrom': 'Receive' }
                        , function (result) {
                            closeOrdReceiveInfoModel();
                            $('#myDataTable').dataTable().fnStandingRedraw();
                            $('#DivLoading').hide();
                        }, function (result) {
                            closeOrdReceiveInfoModel();
                            $('#DivLoading').hide();
                        });

                    //$.ajax({
                    //    'url': _ReceiveList.urls.closeOrderDetailLineItemsURL,
                    //    'data': { 'ids': orderDetailID, 'CallFrom': 'Receive' },
                    //    'success': function (result) {
                    //        closeOrdReceiveInfoModel();
                    //        $('#myDataTable').dataTable().fnStandingRedraw();
                    //        $('#DivLoading').hide();
                    //    },
                    //    'error': function (result) {
                    //        closeOrdReceiveInfoModel();
                    //        $('#DivLoading').hide();
                    //    }

                    //});
                }
                else {
                    closeOrdReceiveInfoModel();
                    $('#DivLoading').hide();
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    $("#spanGlobalMessage").html(MsgSelectLineItem);
                    showNotificationDialog();
                    //alert(MsgSelectLineItem);
                }

            });

            $('#bntUnCloseItemConfirmYes').click(function () {
                $('#DivLoading').show();
                var orderDetailID = '';

                $('#myDataTable').find("tbody tr.row_selected").each(function (e) {
                    if (orderDetailID.length > 0) {
                        orderDetailID = orderDetailID + ",";
                    }
                    orderDetailID = orderDetailID + $(this).find('#spnOrderDetailID').text();
                });
                if (orderDetailID.length > 0) {

                    _AjaxUtil.getJson(_ReceiveList.urls.unCloseOrderLineItemsURL, { 'ids': orderDetailID, 'CallFrom': 'Receive' }
                        , function (result) {
                            closeOrdReceiveInfoModel();
                            $('#myDataTable').dataTable().fnStandingRedraw();
                            $('#DivLoading').hide();
                        }, function (result) {
                            closeOrdReceiveInfoModel();
                            $('#DivLoading').hide();
                        }
                    );

                    //$.ajax({
                    //    'url': _ReceiveList.urls.unCloseOrderLineItemsURL,
                    //    'data': { 'ids': orderDetailID, 'CallFrom': 'Receive' },
                    //    'success': function (result) {
                    //        closeOrdReceiveInfoModel();
                    //        $('#myDataTable').dataTable().fnStandingRedraw();
                    //        $('#DivLoading').hide();
                    //    },
                    //    'error': function (result) {
                    //        closeOrdReceiveInfoModel();
                    //        $('#DivLoading').hide();
                    //    }

                    //});
                }
                else {
                    closeOrdReceiveInfoModel();
                    $('#DivLoading').hide();
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    $("#spanGlobalMessage").html(MsgSelectLineItem);
                    showNotificationDialog();
                    //alert(MsgSelectLineItem);
                }
            });

            $('#btnEditReciept').click(function (e) {
                ClickFrom = '';
                var anSelectedRows = fnGetSelected(oTable);
                if (anSelectedRows.length == 1) {
                    var recQty = anSelectedRows

                    var rowPosition = oTable.fnGetPosition(anSelectedRows[0]);
                    var aData = oTable.fnGetData(rowPosition);

                    var receivedQty = aData.ReceivedQuantity;
                    var orderStatus = aData.OrderStatus
                    var colr = hexc($('#myDataTable tbody tr.row_selected').css('background-color'));
                    if (isNaN(parseFloat(receivedQty)) || parseFloat(receivedQty) <= 0 || parseInt(orderStatus) == 4) {
                        //alert(MsgNoPreviousReceiptToEdit);
                        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                        $("#spanGlobalMessage").html(MsgNoPreviousReceiptToEdit);
                        showNotificationDialog();
                        return false;
                    }
                    if (colr === '#d3d3d3') {
                        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                        $("#spanGlobalMessage").html(MsgSelectUnclosedItemValidation);
                        showNotificationDialog();
                        //alert(MsgSelectUnclosedItemValidation);
                        return false;
                    }

                    if (colr !== '#d3d3d3') {
                        var itemGuid = $('#myDataTable tbody tr.row_selected').find('#spnItemID').text();
                        var orderDetailGuid = $('#myDataTable tbody tr.row_selected').find('#spnOrderDetailGUID').text();
                        $('#divEditReceipt').data({ "ItemGuid": itemGuid, 'OrderDetailGuid': orderDetailGuid }).dialog('open');
                        return false;
                    }

                }
                else if (anSelectedRows.length > 1) {
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    $("#spanGlobalMessage").html(MsgSelectOnlyOneRecord);
                    showNotificationDialog();
                    //alert(MsgSelectOnlyOneRecord);
                    return false;
                }
                else {
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    $("#spanGlobalMessage").html(MsgSelectRow);
                    showNotificationDialog();
                    //alert(MsgSelectRow);
                    return false;
                }

            });

            $('#amnuReceiveNewLink').click(function () {
                $("#tab1").click();
            });

            $('#amnuReceiveIncomplete').click(function () {
                $("#tab4").click();
            });

            $('#amnuReceiveHistoryLink').click(function () {
                $("#tab5").click();
            });

        });
    }
       

    self.initDataTable = function (HasOnTheFlyEntryRight, txtReceiveDateVal,
        arrCol_ItemMaster, arrCol_ReceivedOrderTransferDetail
        , oLanguage1, isCost) {
        $(document).ready(function () {
            $('#DivLoading').show();
            var $myDataTable = $('#myDataTable');

            var sImageUrl = "/Content/images/";


            var anOpen = [];
            //var SelectedItemID = 0;
            var costFixedLength;
            var qtyFixedLength;
            
                        
            objColumns = null;
            objColumns = GetGridHeaderColumnsObject('myDataTable');

            costFixedLength = $("#hdCostcentsLimit").val();
            qtyFixedLength = $("#hdQuantitycentsLimit").val();

            if (isNaN(parseInt(qtyFixedLength)) || parseInt(qtyFixedLength) < 0) {
                qtyFixedLength = 2;
            }

            if (isNaN(parseInt(costFixedLength)) || costFixedLength < 0) {
                costFixedLength = 3;
            }

            var arrColumns = new Array();
            arrColumns.push({ mDataProp: null, sClass: "read_only center NotHide RowNo", "bSortable": false, sDefaultContent: '' });
            arrColumns.push({
                "sClass": "read_only control center NotHide", "bSortable": false,
                "sDefaultContent": '',
                "fnRender": function (obj, val) {
                    return '<img id="imgPlusMinus_' + obj.aData.OrderDetailID + '" src="' + sImageUrl + 'drildown_open.jpg' + '">' + ' <span id="spnItemType" style="display:none">' + obj.aData.ItemType + ' </span> '
                        + ' <span id="spnItemID" style="display:none">' + obj.aData.ItemGUID.toString() + ' </span> '
                        + ' <span id="spnOrderGUIID" style="display:none">' + obj.aData.OrderGUID.toString() + ' </span> '
                        + ' <span id="spnOrderDetailID" style="display:none">' + obj.aData.OrderDetailID.toString() + ' </span> '
                        + ' <span id="spnOrderStatus" style="display:none">' + obj.aData.OrderStatus + '</span>'
                        + ' <span id="spnOrderDetailGUID" style="display:none">' + obj.aData.OrderDetailGUID.toString() + ' </span> '
                        + ' <span id="spnIsCloseLineItem" style="display:none">' + obj.aData.IsCloseItem.toString() + ' </span> ';
                }
            });
            arrColumns.push({
                "sClass": "read_only control center NotHide", "bSortable": false,
                "sDefaultContent": '',
                "fnRender": function (obj, val) {
                    if (parseInt(obj.aData.OrderStatus) < 8) {
                        return '<input id="btnRecieveInline" type="button" value="' + btnReceive + '" class="GridBtnStyle inLineBtn" />'
                    }
                    else
                        return "";
                }
            });
            arrColumns.push({ "mDataProp": "ItemNumber", "sClass": "read_only NotHide" });
            arrColumns.push({ "mDataProp": "OrderNumber", "sClass": "read_only" });
            arrColumns.push({ "mDataProp": "OrderReleaseNumber", "sClass": "read_only" });
            arrColumns.push({
                "mDataProp": "RequestedQuantity", "sClass": "read_only numericalign", "fnRender": function (obj, val) {
                    if (!isNaN(parseFloat(val))) {
                        return FormatedCostQtyValues(val, 2);
                    }
                    else {
                        return FormatedCostQtyValues(0, 2);
                    }
                }
            });
            arrColumns.push({
                "mDataProp": "ApprovedQuantity", "sClass": "read_only numericalign", "fnRender": function (obj, val) {
                    if (!isNaN(parseFloat(val))) {
                        return FormatedCostQtyValues(val, 2);
                    }
                    else {
                        return FormatedCostQtyValues(0, 2);
                    }
                }
            });
            arrColumns.push({
                "mDataProp": "ReceivedQuantity", "sClass": "read_only numericalign NotHide", "fnRender": function (obj, val) {
                    if (!isNaN(parseFloat(val))) {
                        return FormatedCostQtyValues(val, 2);
                        //return "<span id='spnReceivedQty' >" + FormatedCostQtyValues(val, 2) + "</span>";
                    }
                    else {
                        //return "<span id='spnReceivedQty' >" + FormatedCostQtyValues(0, 2) + "</span>";
                        return FormatedCostQtyValues(0, 2);
                    }
                }
            });
            arrColumns.push({
                "mDataProp": "ReceiveBinName", "sClass": "read_only NotHide", "fnRender": function (obj, val) {
                    if (obj.aData.OrderStatus == '8' || obj.aData.SerialNumberTracking || obj.aData.LotNumberTracking || obj.aData.DateCodeTracking) {
                        if (val == "[|EmptyStagingBin|]")
                            return '';
                        else
                            return val;
                    }
                    else {
                        if (HasOnTheFlyEntryRight == 'True') {
                            if (val == "[|EmptyStagingBin|]")
                                return '<span style="position:relative"><input type="text" id="txtBinNumber" class="text-boxinner ReciveAuto" style = "width:90px;" value="" /><a id="lnkShowAllOptions" href="javascript:void(0);" style="position:absolute; right:5px; top:0px;" class="ShowAllOptions" ><img src="/Content/images/arrow_down_black.png" alt="select" /></a><input type="hidden" value="false" id="hdnIsLoadMoreLocations" /></span>';
                            else
                                return '<span style="position:relative"><input type="text" id="txtBinNumber" class="text-boxinner ReciveAuto" style = "width:90px;" value="' + val + '" /><a id="lnkShowAllOptions" href="javascript:void(0);" style="position:absolute; right:5px; top:0px;" class="ShowAllOptions" ><img src="/Content/images/arrow_down_black.png" alt="select" /></a><input type="hidden" value="false" id="hdnIsLoadMoreLocations" /></span>';
                        }
                        else
                            return "<select id='slctBinName' name='slctBinName' class='selectBox'><option value='" + val + "'>" + val + "</option</select><input type='text' id='txtBinNumber' name='txtBinName' value='" + val + "' style='display:none;' />";
                    }
                }
            });
            arrColumns.push({
                "mDataProp": "QuantityToReceive", "sDefaultContent": '', "sClass": "read_only NotHide", "fnRender": function (obj, val) {
                    if (obj.aData.OrderStatus == '8' || obj.aData.SerialNumberTracking || obj.aData.LotNumberTracking || obj.aData.DateCodeTracking) {

                        var apprQty = 0;
                        var recvedQty = 0;
                        apprQty = parseFloat(obj.aData.ApprovedQuantity.toString()).toFixed(qtyFixedLength);
                        recvedQty = parseFloat(obj.aData.ReceivedQuantity.toString()).toFixed(qtyFixedLength);

                        if (isNaN(apprQty)) {
                            apprQty = 0;
                        }
                        if (isNaN(recvedQty)) {
                            recvedQty = 0;
                        }

                        var QtyToRecv = (apprQty - recvedQty);

                        if (obj.aData.isOktaEnable) {
                            var inTransitQuantity = obj.aData.InTransitQuantity;
                            if (obj.aData.InTransitQuantity == null) {
                                inTransitQuantity = 0;
                            }
                            QtyToRecv = parseFloat(inTransitQuantity);
                        }
                        if (isNaN(parseFloat(QtyToRecv)) && parseFloat(QtyToRecv).toFixed(qtyFixedLength) <= 0) {
                            QtyToRecv = 0;
                        }


                        if (obj.aData.SerialNumberTracking) {
                            QtyToRecv = QtyToRecv.toString();
                        }
                        else {
                            QtyToRecv = QtyToRecv.toString();
                        }
                        if (QtyToRecv <= 0 || obj.aData.IsCloseItem)
                            QtyToRecv = 0;

                        return "<input type='hidden' style='width:60px;' id='txtQuantityToReceive" + obj.aData.OrderDetailID + "' value='" + QtyToRecv + "' class='text-boxinner numericinput'/>"

                        //return "";
                    }
                    else {
                        var apprQty = 0;
                        var recvedQty = 0;
                        apprQty = parseFloat(obj.aData.ApprovedQuantity.toString()).toFixed(qtyFixedLength);
                        recvedQty = parseFloat(obj.aData.ReceivedQuantity.toString()).toFixed(qtyFixedLength);

                        if (isNaN(apprQty)) {
                            apprQty = 0;
                        }
                        if (isNaN(recvedQty)) {
                            recvedQty = 0;
                        }
                        
                        var QtyToRecv = (apprQty - recvedQty);
     
                        if (obj.aData.isOktaEnable) {
                            var inTransitQuantity = obj.aData.InTransitQuantity;
                            if (obj.aData.InTransitQuantity == null) {
                                inTransitQuantity = 0;
                            }
                            QtyToRecv = parseFloat(inTransitQuantity);
                        }
                        if (isNaN(parseFloat(QtyToRecv)) && parseFloat(QtyToRecv).toFixed(qtyFixedLength) <= 0) {
                            QtyToRecv = 0;
                        }


                        if (obj.aData.SerialNumberTracking) {
                            QtyToRecv = QtyToRecv.toString();
                        }
                        else {
                            QtyToRecv = QtyToRecv.toString();
                        }
                        if (QtyToRecv <= 0 || obj.aData.IsCloseItem)
                            QtyToRecv = 0;

                        return "<input type='text' style='width:60px;' id='txtQtyToRecv" + obj.aData.OrderDetailID + "' value='" + QtyToRecv + "' class='text-boxinner numericinput'/>"
                    }
                }
            });
            arrColumns.push({
                "mDataProp": null, "sDefaultContent": '', "sClass": "read_only NotHide", "fnRender": function (obj, val) {
                    if (obj.aData.OrderStatus == '8' || obj.aData.SerialNumberTracking || obj.aData.LotNumberTracking || obj.aData.DateCodeTracking) {
                        return "";
                    }
                    else {
                        return "<input type='text' style='width:80px;' id='txtReceiveDate" + obj.aData.OrderDetailID + "' value='" + txtReceiveDateVal + "' class='text-boxinner hasDatePicker'/>"
                    }
                }
            });
            arrColumns.push({
                "mDataProp": "OrderDetailRequiredDate", "sClass": "read_only",
                "fnRender": function (obj, val) {
                    return obj.aData.strReqDtlDate;
                    //return GetDateInShortFormat(val);
                }
            });
            arrColumns.push({
                "mDataProp": "OrderStatusChar", "sClass": "read_only", "fnRender": function (obj, val) {
                    return "<span id='spnOrdStatusText_" + obj.aData.OrderDetailID + "' >" + val + "</span>";
                }
            });
            arrColumns.push({ "mDataProp": "ItemDescription", "sClass": "read_only" });
            arrColumns.push({
                "mDataProp": "ItemCost", "sClass": "read_only numericalign", "fnRender": function (obj, val) {
                    //if (!isNaN(parseFloat(val))) {
                    //    return FormatedCostQtyValues(val, 1);
                    //}
                    //else {
                    //    return FormatedCostQtyValues(0, 1);
                    //}
                    
                    val = 0;
                    if (obj.aData.OrderItemCost != null && obj.aData.OrderItemCost != NaN) {
                        val = obj.aData.OrderItemCost;
                    }
                    if (obj.aData.OrderStatus == '8' || obj.aData.ItemConsignment || obj.aData.SerialNumberTracking == "Yes" || obj.aData.LotNumberTracking == "Yes" || obj.aData.DateCodeTracking == "Yes") {
                        return "<input type='text' style='width:60px;' id='txtItemCost' value='" + FormatedCostQtyValues(val, 1) + "' class='text-boxinner numericinput' readonly='readonly'/><input type = 'hidden' id='hdItemCost' value='" + FormatedCostQtyValues(val, 1) + "' />"
                    }
                    else {
                        return "<input type='text' style='width:60px;' id='txtItemCost' value='" + FormatedCostQtyValues(val, 1) + "' class='text-boxinner numericinput'/><input type = 'hidden' id='hdItemCost' value='" + FormatedCostQtyValues(val, 1) + "' />"
                    }
                }
            });
            arrColumns.push({ "mDataProp": "OrderSupplierName", "sClass": "read_only" });
            arrColumns.push({ "mDataProp": "SupplierPartNumber", "sClass": "read_only" });
            arrColumns.push({ "mDataProp": "Manufacturer", "sClass": "read_only" });
            arrColumns.push({ "mDataProp": "ManufacturerNumber", "sClass": "read_only" });
            arrColumns.push({
                "mDataProp": "SerialNumberTracking", "sClass": "read_only", "fnRender": function (obj, val) {
                    return GetBoolInFormat(obj, val);
                }
            });
            arrColumns.push({
                "mDataProp": "LotNumberTracking", "sClass": "read_only", "fnRender": function (obj, val) {
                    return GetBoolInFormat(obj, val);
                }
            });
            arrColumns.push({
                "mDataProp": "DateCodeTracking", "sClass": "read_only", "fnRender": function (obj, val) {
                    return GetBoolInFormat(obj, val);
                }
            });
            arrColumns.push({
                "mDataProp": "IsTransfer", "sClass": "read_only", "fnRender": function (obj, val) {
                    return GetBoolInFormat(obj, val);
                }
            });
            arrColumns.push({
                "mDataProp": "IsPurchase", "sClass": "read_only", "fnRender": function (obj, val) {
                    return GetBoolInFormat(obj, val);
                }
            });
            arrColumns.push({ "mDataProp": "UnitName", "sClass": "read_only" });
            arrColumns.push({ "mDataProp": "InTransitQuantity", "sClass": "read_only numericalign" });
            arrColumns.push({
                "mDataProp": "PackSlipNumber", "sClass": "read_only",
                "fnRender": function (obj, val) {
                    //  return "<span id='spnPackSlipNumber_" + obj.aData.OrderDetailID + "' >" + val + "</span>";
                    if (obj.aData.OrderStatus == '8' || obj.aData.SerialNumberTracking == "Yes" || obj.aData.LotNumberTracking == "Yes" || obj.aData.DateCodeTracking == "Yes") {
                        return "<span id='spnPackSlipNumber_" + obj.aData.OrderDetailID + "'>" + val + "</span>";
                    }
                    else {
                        return "<input type='text' style='width:97%;' id='txtPackslipNo' value='" + val + "' class='text-boxinner'/>"
                    }
                }
            });
            arrColumns.push({ "mDataProp": "ASNNumber", "sClass": "read_only" });
            arrColumns.push({
                "mDataProp": "ShippingTrackNumber", "sClass": "read_only"
                , "fnRender": function (obj, val) {
                    return "<span id='spnShipmentTrack_" + obj.aData.OrderDetailID + "' >" + val + "</span>";
                }
            });
            arrColumns.push({ "mDataProp": "StagingName", "sClass": "read_only" });
            arrColumns.push({ "mDataProp": "OrderCreatedByName", "sClass": "read_only" });
            arrColumns.push({ "mDataProp": "OrderUpdatedByName", "sClass": "read_only" });
            arrColumns.push({
                "mDataProp": "OrderCreated", "sClass": "read_only",
                "fnRender": function (obj, val) {
                    return obj.aData.CreatedDate;
                }
            });
            arrColumns.push({
                "mDataProp": "OrderLastUpdated", "sClass": "read_only",
                "fnRender": function (obj, val) {
                    return obj.aData.UpdatedDate;
                }
            });
            arrColumns.push({ "mDataProp": "AddedFrom", "sClass": "read_only" });
            arrColumns.push({ "mDataProp": "EditedFrom", "sClass": "read_only" });
            arrColumns.push({
                "mDataProp": "ReceivedOn", "sClass": "read_only",
                "fnRender": function (obj, val) {
                    return obj.aData.ReceivedOnDate;
                }
            });
            arrColumns.push({
                "mDataProp": "ReceivedOnWeb", "sClass": "read_only",
                "fnRender": function (obj, val) {
                    return obj.aData.ReceivedOnDateWeb;
                }
            });
            arrColumns.push({ "mDataProp": "CostUOM", "sClass": "read_only" });
            arrColumns.push({
                "mDataProp": "OnHandQuantity", "sClass": "read_only", "fnRender": function (obj, val) {
                    if (!isNaN(parseFloat(val))) {
                        return FormatedCostQtyValues(val, 2);
                    }
                    else {
                        return FormatedCostQtyValues(0, 2);
                    }
                }
            });
            arrColumns.push({
                "mDataProp": "ItemMinimumQuantity", "sClass": "read_only numericalign", "fnRender": function (obj, val) {
                    if (val == "-1")
                        return "<span>" + 'N/A' + "</span>";
                    else {
                        if (!isNaN(parseFloat(val)))
                            return FormatedCostQtyValues(val, 2);
                        else
                            return FormatedCostQtyValues(0, 2);
                    }
                }
            });
            arrColumns.push({
                "mDataProp": "ItemMaximumQuantity", "sClass": "read_only numericalign", "fnRender": function (obj, val) {
                    if (val == "-1") {
                        return "<span>" + 'N/A' + "</span>";
                    }
                    else {
                        if (!isNaN(parseFloat(val))) {
                            return FormatedCostQtyValues(val, 2);
                        }
                        else {
                            return FormatedCostQtyValues(0, 2);
                        }
                    }
                }
            });
            arrColumns.push({
                "mDataProp": "BinOnHandQTY", "sClass": "read_only", "fnRender": function (obj, val) {
                    if (!isNaN(parseFloat(val))) {
                        return FormatedCostQtyValues(val, 2);
                    }
                    else {
                        return FormatedCostQtyValues(0, 2);
                    }
                }
            });
            arrColumns.push({
                "mDataProp": "OnOrderQuantity", "sClass": "read_only", "fnRender": function (obj, val) {
                    if (!isNaN(parseFloat(val))) {
                        return FormatedCostQtyValues(val, 2);
                    }
                    else {
                        return FormatedCostQtyValues(0, 2);
                    }
                }
            });
            $.each(arrCol_ItemMaster, function (index, val) {
                arrColumns.push(val);
            });

            $.each(arrCol_ReceivedOrderTransferDetail, function (index, val) {
                arrColumns.push(val);
            });
            arrColumns.push({
                "sClass": "read_only control center NotHide", "bSortable": false,
                "sDefaultContent": '',
                "fnRender": function (obj, val) {
                    var fileid = "file_" + obj.aData.OrderDetailGUID.toString();
                    var validFileList = "validFileList_file_" + obj.aData.OrderDetailGUID.toString();
                    if (parseInt(obj.aData.OrderStatus) < 8) {
                        return "<input type='file' class='receivefileattachment' id=" + fileid + " multiple/><span id=" + validFileList + "></span>";
                    }
                    else {
                        return "<input type='file' class='receivefileattachment' id=" + fileid + " multiple disabled='disabled'/><span id=" + validFileList + "></span>";
                    }
                }
            });

            arrColumns.push({
                "sClass": "read_only NotHide", "bSortable": false,
                "sDefaultContent": '',
                "fnRender": function (obj, val) {
                    var filename = "";
                    if (obj.aData.AttachmentFileNames.length > 0) {
                        for (var i = 0; i < obj.aData.AttachmentFileNames.length; i++) {
                            if (filename == "") {
                                if (parseInt(obj.aData.OrderStatus) < 8) {
                                    filename = "<a class='preview' href='/Consume/Get?path=" + ReceiveFile_Path + "/" + obj.aData.OrderDetailID + "/" + obj.aData.AttachmentFileNames[i].FileName + "' target = '_blank' > " + obj.aData.AttachmentFileNames[i].FileName + "</a ><image class='receivedattachmentdelete' data-id=" + obj.aData.AttachmentFileNames[i].FileGUID + " id='received_" + obj.aData.AttachmentFileNames[i].FileGUID + "' src='/content/images/delete.png' alt='Delete'/>";
                                } else {
                                    filename = "<a class='preview' href='/Consume/Get?path=" + ReceiveFile_Path + "/" + obj.aData.OrderDetailID + "/" + obj.aData.AttachmentFileNames[i].FileName + "' target = '_blank' > " + obj.aData.AttachmentFileNames[i].FileName + "</a >";
                                }
                            }
                            else {
                                if (parseInt(obj.aData.OrderStatus) < 8) {
                                    filename = filename + "<a class='preview' href='/Consume/Get?path=" + ReceiveFile_Path + "/" + obj.aData.OrderDetailID + "/" + obj.aData.AttachmentFileNames[i].FileName + "' target = '_blank' >" + obj.aData.AttachmentFileNames[i].FileName + "</a ><image class='receivedattachmentdelete' data-id=" + obj.aData.AttachmentFileNames[i].FileGUID + " id='received_" + obj.aData.AttachmentFileNames[i].FileGUID + "' src='/content/images/delete.png' alt='Delete'/>";
                                } else {
                                    filename = filename + "<a class='preview' href='/Consume/Get?path=" + ReceiveFile_Path + "/" + obj.aData.OrderDetailID + "/" + obj.aData.AttachmentFileNames[i].FileName + "' target = '_blank' > || " + obj.aData.AttachmentFileNames[i].FileName + "</a >";
                                }
                            }
                        }
                    }
                    return filename;
                }
            });

            arrColumns.push({
                "mDataProp": "IsBackOrdered", "sClass": "read_only control center", "fnRender": function (obj, val) {
                    if (val) {
                        return "<input checked='checked' id='IsBackOrdered' name='IsBackOrdered' type='checkbox' value='true' class='check-box' disabled='disabled'>";
                    }
                    else {
                        return "<input id='IsBackOrdered' name='IsBackOrdered' type='checkbox' value='true' class='check-box' disabled='disabled'>";
                    }
                }
            });
            arrColumns.push({
                "sClass": "read_only control center NotHide", "bSortable": false,
                "sDefaultContent": '', "fnRender": function (obj, val) {
                    return '<img id="BackOrderedExpand_' + obj.aData.OrderDetailID + '" src="' + sImageUrl + 'drildown_open.jpg' + '">';
                }
            });
            arrColumns.push({
                "mDataProp": "OrderLineException", "sClass": "read_only control center", "fnRender": function (obj, val) {
                    if (val) {
                        return "<input checked='checked' id='OrderLineException' name='OrderLineException' type='checkbox' value='true' class='check-box' disabled='disabled'>";
                    }
                    else {
                        return "<input id='OrderLineException' name='OrderLineException' type='checkbox' value='true' class='check-box' disabled='disabled'>";
                    }
                }
            });
            arrColumns.push({ "mDataProp": "OrderLineExceptionDesc", "sClass": "read_only" });


            var gaiSelected = [];
            LoadTabs();
            oTable = $myDataTable.dataTable({
                "bJQueryUI": true,
                "bScrollCollapse": true,
                "sScrollX": "99%",
                "sDom": 'RC<"top"lp<"clear">>rt<"bottom"i<"clear">>T',
                "oColVis": {},
                "aaSorting": [[2, "asc"]],
                "oColReorder": {},
                "sPaginationType": "full_numbers",
                "bProcessing": true,
                "bStateSave": true,
                "oLanguage": oLanguage,

                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                    if (aData.IsDeleted == true && aData.IsArchived == true)
                        nRow.className = "GridDeleatedArchivedRow";
                    else if (aData.IsDeleted == true)
                        nRow.className = "GridDeletedRow";
                    else if (aData.IsArchived == true)
                        nRow.className = "GridArchivedRow";
                    else if (aData.IsCloseItem == true)
                        $(nRow).css('background-color', '#d3d3d3');

                    $("td.RowNo:first", nRow).html(this.fnSettings()._iDisplayStart + iDisplayIndex + 1);
                    return nRow;
                },
                "fnStateSaveParams": self.saveReceiveMasterListState,
                "fnStateLoad": function (oSettings) {
                    var o;

                    _AjaxUtil.postJson(_ReceiveList.urls.loadGridStateURL
                        , { ListName: 'ReceiveMasterList' }
                        , function (json) {
                            if (json.jsonData != '') {
                                o = JSON.parse(json.jsonData);
                            }
                        }
                        , null, false, false
                    );

                    return o;
                }, 
                "bServerSide": true,
                "sAjaxSource": _ReceiveList.urls.receiveListAjaxURL,
                "fnServerData": self.grid_fnServerData,
                "fnInitComplete": function () {
                    $('.ColVis').detach().appendTo(".setting-arrow");
                    $('#divQTYLegends').show(1000);
                },
                "aoColumns": arrColumns

            }).makeEditable();
            oTable.on('draw', function () {
                imagePreview();
                $(".receivefileattachment").off('change');
                $(".receivefileattachment").change(function () {
                    var validExtension = ReceivedFileExtension.split(',');
                    var strValidationMessage = "";
                    var totalfiles = document.getElementById($(this).attr("id")).files.length;
                    if ($("#validFileList_" + $(this).attr("id")).length > 0) {
                        $("#validFileList_" + $(this).attr("id")).empty();
                        
                    }
                    if ($(this).attr("id").length > 0 && $(this).attr("id").indexOf("_") > 3 && $(this).attr("id").split("_")[1].length > 1) {
                        var orderDetailsGuid = $(this).attr("id").split("_")[1];
                        deleteFromRemovedList(orderDetailsGuid);
                    }
                    for (var i = 0; i < totalfiles; i++) {
                        var IsValidFile = true;
                        var fileExt = document.getElementById($(this).attr("id")).files[i].name;
                        fileExt = fileExt.substring(fileExt.lastIndexOf('.'));
                        if (validExtension.indexOf(fileExt.toLowerCase()) <= -1) {
                            IsValidFile = false;
                            strValidationMessage = strValidationMessage + document.getElementById($(this).attr("id")).files[i].name + MsgInvalidFileSelected;
                        } else {
                            if ($("#validFileList_" + $(this).attr("id")).length > 0) {
                                var deletedID = "delete_" + $(this).attr("id");
                                $("#validFileList_" + $(this).attr("id")).append("<label>" + document.getElementById($(this).attr("id")).files[i].name + "</label><image class='receiveattachmentdelete' data-id=" + $(this).attr("id") + " data-filename=" + document.getElementById($(this).attr("id")).files[i].name + " id=" + deletedID + " src='/content/images/delete.png' alt='Delete'/>");
                            }
                        }
                    }

                    if (strValidationMessage != "") {
                        alert(strValidationMessage + MsgvalidFileList.replace("{0}", validExtension.toString()));
                    }
                });
            });
            $('.DTTT_container').css('z-index', '-1');

            if (_isCost == 'False') {

                ColumnsToHideinPopUp.push(12);

                oTable.fnSetColumnVis(12, false);
            }

            //This is Most important
            jQuery("#btnDiv").click(function (e) {
                var offset = $(this).offset();
                var leftpx = (parseInt(e.clientX) + parseInt($(this).css("width").toString().replace("px", "")) + parseInt(15)) + 'px';
                var toppx = (parseInt(e.clientY) + parseInt(5)) + 'px';
                jQuery('#myDataTable_wrapper div.ColVis .ColVis_Button').click();
                jQuery('.ColVis_collection').css("left", leftpx);
                jQuery('.ColVis_collection').css("top", toppx);
                e.preventDefault();
            });
            //HIDE PRINT CONTAINER
            /*Functions used for nasted data binding START*/
            var $myDataTableTbody = $myDataTable.find('tbody');
            $myDataTableTbody.on('click', 'img[id^="imgPlusMinus_"]', function (event) {
                //$('#myDataTable tbody').on('click', 'img[id^="imgPlusMinus_"]', function (event) {
                if ($(this).parent().parent().find(".BackOrderedIsOpen") != undefined && $(this).parent().parent().find(".BackOrderedIsOpen").length > 0) {
                    $(this).parent().parent().find(".BackOrderedIsOpen").trigger("click");
                }
                self.detailGrid.isSaveGridState = false;
                var nTr = this.parentNode.parentNode;
                var i = $.inArray(nTr, anOpen);
                if (i === -1) {
                    $(this).attr('src', sImageUrl + "drildown_close.jpg");
                    oTable.fnOpen(nTr, fnFormatDetails(oTable, nTr), '');
                    anOpen.push(nTr);
                    $(this).addClass("IsOpen");
                }
                else {
                    $(this).attr('src', sImageUrl + "drildown_open.jpg");
                    oTable.fnClose(nTr);
                    anOpen.splice(i, 1);
                    $(this).removeClass("IsOpen");
                }
            });


            function fnFormatDetails(oTable, nTr) {
                var oData = oTable.fnGetData(nTr);
                var sOut = '';
                //$('#DivLoading').show();
                _ReceiveList.showHideLoader(true);

                _AjaxUtil.getText(_ReceiveList.urls.receiveItemURL, { 'OrderDetailGUID': oData.OrderDetailGUID }
                    , function (json) {
                        sOut = json;
                        //$('#DivLoading').hide();
                        _ReceiveList.showHideLoader(false);
                    }, function (response) {
                        //$('#DivLoading').hide();
                        _ReceiveList.showHideLoader(false);
                    }
                    , false
                    , false
                );

                return sOut;
            }

            $myDataTableTbody.on('click', 'img[id^="BackOrderedExpand_"]', function (event) {
                //$('#myDataTable tbody').on('click', 'img[id^="imgPlusMinus_"]', function (event) {
                _ReceiveList.showHideLoader(true);
                if ($(this).parent().parent().find(".IsOpen") != undefined && $(this).parent().parent().find(".IsOpen").length > 0) {
                    $(this).parent().parent().find(".IsOpen").trigger("click");
                }
                self.detailGrid.isSaveGridState = false;
                var nTr = this.parentNode.parentNode;
                var i = $.inArray(nTr, anOpen);
                if (i === -1) {
                    $(this).attr('src', sImageUrl + "drildown_close.jpg");
                    oTable.fnOpen(nTr, fnBackOrderDetails(oTable, nTr), '');
                    anOpen.push(nTr);
                    $(this).addClass("BackOrderedIsOpen");
                }
                else {
                    $(this).attr('src', sImageUrl + "drildown_open.jpg");
                    oTable.fnClose(nTr);
                    anOpen.splice(i, 1);
                    $(this).removeClass("BackOrderedIsOpen");
                }
                _ReceiveList.showHideLoader(false);
            });

            function fnBackOrderDetails(oTable, nTr) {
                var oData = oTable.fnGetData(nTr);
                var sOut = '';
                //$('#DivLoading').show();
               
                _AjaxUtil.getText(_ReceiveList.urls.backOrderedDetails, { 'OrderDetailsGUID': oData.OrderDetailGUID }
                    , function (json) {
                        sOut = json;
                        //$('#DivLoading').hide();
                        _ReceiveList.showHideLoader(false);
                    }, function (response) {
                        //$('#DivLoading').hide();
                        _ReceiveList.showHideLoader(false);
                    }
                    , false
                    , false
                );

                return sOut;
            }


            //$('table#myDataTable tbody').on('click', '#btnRecieveInline', function (event) {
            $myDataTableTbody.on('click', '#btnRecieveInline', function (event) {
                self.detailGrid.isSaveGridState = false;
                var nTr = this.parentNode.parentNode;
                $(nTr).removeClass('row_selected');
                $(nTr).addClass('row_selected');
                $('#btnReceiveAllNew').trigger('click');
            });

            //$('#myDataTable tbody').delegate(".hasDatePicker", "focusin", function () {
            $myDataTableTbody.delegate(".hasDatePicker", "focusin", function () {
                $(this).datepicker({
                    dateFormat: RoomDateJSFormat,
                    changeMonth: true,
                    changeYear: true,
                    clearText: 'Clear', onClose: function () { this.focus(); }
                });
            });

            //----- script 2
            var QueryStringParam1 = self.getParameterByName('fromdashboard');
            var QueryStringParam2 = self.getParameterByName('OrderQuantity');
            var QueryStringParam3 = self.getParameterByName('ItemName');
            if (QueryStringParam1 == 'yes' && QueryStringParam2 == 'yes' && QueryStringParam3 != '') {
                $('#myDataTable').dataTable().fnFilter(QueryStringParam3, null, null, null);
            }
            if (window.location.hash.toLowerCase() == "#incomplete") {
                $("#tab4").click();
            }

            var QueryStringParam4 = self.getParameterByName('incomplete');
            var QueryStringParam5 = self.getParameterByName('itemnumber');

            if (QueryStringParam1 == 'yes' && QueryStringParam4 == 'yes' && QueryStringParam5 != '') {
                $("#tab4").click();
                $myDataTable.dataTable().fnFilter(QueryStringParam5, null, null, null);
            }

            $myDataTable.on({
                mouseover: function () {
                    var tr = $(this).parent().parent();
                    var itmGuid = $(tr).find('#spnItemID').text();
                    var orderGuid = $(tr).find('#spnOrderGUIID').text();
                    var td = $(this).parent();
                    var tdWidth = "140px";
                    if (typeof (td) != "undefined" && td != null && td.css('width') != null && typeof (td.css('width')) != "undefined") {
                        tdWidth = td.css('width');
                    }
                    getItemInventoryStagingLocation($(this), "", itmGuid, orderGuid, tdWidth);
                },
                focus: function () {
                    var tr = $(this).parent().parent();
                    var itmGuid = $(tr).find('#spnItemID').text();
                    var orderGuid = $(tr).find('#spnOrderGUIID').text();
                    var td = $(this).parent();
                    var tdWidth = "140px";
                    if (typeof (td) != "undefined" && td != null && td.css('width') != null && typeof (td.css('width')) != "undefined") {
                        tdWidth = td.css('width');
                    }
                    getItemInventoryStagingLocation($(this), "", itmGuid, orderGuid, tdWidth);
                },
                change: function () {
                    $(this).parent().find("input[id='txtBinNumber']").val($(this).val());
                }
            }, "#slctBinName");

            $myDataTable.on('focus', "input.ReciveAuto", function (e) {
                var ajaxURL = _ReceiveList.urls.getItemLocationsURL; //'/Receive/GetItemLocations';
                var tr = $(this).parent().parent().parent();
                var itmGuid = $(tr).find('#spnItemID').text();
                var orderGuid = $(tr).find('#spnOrderGUIID').text();
                //var hdnIsLoadMoreLocations = $(tr).find("#hdnIsLoadMoreLocations").val();
                var $this = $(this);

                _AutoCompleteWrapper.init($this
                    , ajaxURL
                    , function (request) {
                        var obj = JSON.stringify({ 'OrderGuid': orderGuid, 'ItemGuid': itmGuid, 'NameStartWith': request.term, 'IsLoadMoreLocations': $(tr).find("#hdnIsLoadMoreLocations").val() });
                        return obj;
                    },
                    function (data) {
                        return $.map(data, function (Items) {
                            return {
                                label: Items.Value,
                                value: Items.Key,
                            }
                        })
                    }
                    , function (curVal, selectedItem) {
                        //        $(this).val(ui.item.value);
                    }
                    , function (selectedItem) {
                    }
                    , true, true);               

            });
            
            // ------- stript 2 end

            //$myDataTable.on('order.dt', function () {
            //});
            //$myDataTable.on('search.dt', function () {
            //});
            //$myDataTable.on('page.dt', function () {
            //    // on grid page change
            //    self.isSaveGridState = false;
            //    self.isGetReplinshRedCount = false;
            //    self.isGetUDFEditableOptionsByUDF = false;
            //});
            

        }); // doc ready


    }

    self.grid_fnServerData = function (sSource, aoData, fnCallback, oSettings) {
        $('#DivLoading').show();
        var $myDataTable = $('#myDataTable');
        var arrCols = new Array();
        var objCols = this.fnSettings().aoColumns;
        for (var i = 0; i <= objCols.length - 1; i++) {
            arrCols.push(objCols[i].mDataProp);
        }
        for (var j = 0; j <= aoData.length - 1; j++) {
            if (aoData[j].name == "sColumns") {
                aoData[j].value = arrCols.join("|");
                break;
            }
        }

        var ordStatus = "4,5,6,7,8";
        var tabid = $(".tabs li div.selected").attr('id');
        if (tabid === "tab4") {
            ordStatus = "4,5,6,7";
        }
        aoData.push({ "name": "OrderStatusIn", "value": ordStatus });
        if (oSettings.aaSorting.length != 0) {
            //aoData.push({ "name": "SortingField", "value": oSettings.aaSorting[0][3] });
            var sortValue = "";
            for (var k = 0; k <= oSettings.aaSorting.length - 1; k++) {
                if (sortValue.length > 0)
                    sortValue += ", "
                sortValue += arrCols[oSettings.aaSorting[k][0]] + ' ' + oSettings.aaSorting[k][1];

            }
            aoData.push({ "name": "SortingField", "value": sortValue });
        }
        else
            aoData.push({ "name": "SortingField", "value": "0" });

        var tmpisDeleted = $('#IsDeletedRecords');
        var tmpIsArchived = $('#IsArchivedRecords');
        var tmpIsDeletedRecords = typeof (tmpisDeleted) !== "undefined" && tmpisDeleted !== null
            ? tmpisDeleted.is(':checked') : "false";
        var tmpIsArchivedRecords = typeof (tmpIsArchived) !== "undefined" && tmpIsArchived !== null
            ? tmpIsArchived.is(':checked') : "false";

        aoData.push({ "name": "IsArchived", "value": tmpIsArchivedRecords });
        aoData.push({ "name": "IsDeleted", "value": tmpIsDeletedRecords });
        
        if (_Common.selectedGridOperation == _Common.gridOperations.Search
            || _Common.selectedGridOperation == _Common.gridOperations.IncludeDeleted
            || _Common.selectedGridOperation == _Common.gridOperations.IncludeArchived
            || _Common.selectedGridOperation == _Common.gridOperations.AutoRefresh
            || _Common.selectedGridOperation == _Common.gridOperations.PageChange
        ) {
            // prevent api calls
            self.isGetReplinshRedCount = false;
            self.isGetUDFEditableOptionsByUDF = false;
            self.isSaveGridState = false;
        }
        else if (_Common.selectedGridOperation == _Common.gridOperations.PageSizeChange) {
            self.isGetReplinshRedCount = false;
            self.isGetUDFEditableOptionsByUDF = false;
            self.isSaveGridState = true;
        }
        else if (_Common.selectedGridOperation == _Common.gridOperations.Sorting
            || _Common.selectedGridOperation == _Common.gridOperations.ColumnResize
        ) {
            self.isGetReplinshRedCount = false;
            self.isGetUDFEditableOptionsByUDF = false;
            self.isSaveGridState = true;
        }
        else if (_Common.selectedGridOperation == _Common.gridOperations.Refresh) {
            self.isGetReplinshRedCount = true;
            self.isGetUDFEditableOptionsByUDF = false;
            self.isSaveGridState = false;
        }

        //_Common.selectedGridOperation = _Common.gridOperations.None;

        oSettings.jqXHR = _AjaxUtil.postJson(sSource, aoData, fnCallback,
            null, null, false,
            function () {
                $myDataTable.removeHighlight();
                $('.dataTables_scroll').css({ "opacity": 0.2 });
            }, self.grid_fnServerData_complete
            , { "__RequestVerificationToken": $("input[name='__RequestVerificationToken'][type='hidden']").val() }
        );

    }

    self.grid_fnServerData_complete = function () {

        //var end = new Date();
        var $myDataTable = $('#myDataTable');

        $('#btnUnCloseLineItem').css('display', 'none');
        $('#btnEditReciept').css('display', 'none');
        $('.dataTables_scroll').css({ "opacity": 1 });
        if ($("#global_filter").val() != '') {
            $myDataTable.highlight($("#global_filter").val());
        }

        $("input[type='text'][id^='txtQtyToRecv']").change(function () {
            var recvQty = $(this).val();
            if (isNaN(parseFloat(recvQty)) === false && parseFloat(recvQty) > 0) {
                $(this).parent().parent().parent().removeClass("row_selected");
                $(this).parent().parent().addClass("row_selected");
            }
            else {
                $(this).parent().parent().removeClass("row_selected");
            }
        });
        if (ImageIDToOpen.length > 0) {
            $myDataTable.find(ImageIDToOpen).click();
            ImageIDToOpen = "";
        }

        if (self.isGetReplinshRedCount) {
            SetReplenishRedCount();
        }

        self.isGetReplinshRedCount = true;

        if (self.isGetUDFEditableOptionsByUDF) {
            UDFfillEditableOptionsForGrid();
        }

        self.isGetUDFEditableOptionsByUDF = true;

        $('.ShowAllOptions').click(function () {
            //$(this).siblings('.ReciveAuto').trigger("focus");
            //$(this).siblings(".ReciveAuto").autocomplete("search", " ");

            var ddl = $(this).siblings('.ReciveAuto');
            _AutoCompleteWrapper.searchHide(ddl);

        });
        $('#DivLoading').hide();
    }

    self.saveReceiveMasterListState = function(oSettings, oData) {

        if (self.isSaveGridState) {
            oData.oSearch.sSearch = "";
            _AjaxUtil.postJson(_ReceiveList.urls.saveGridStateURL, { Data: JSON.stringify(oData), ListName: 'ReceiveMasterList' }
                , function (json) {
                    o = json;
                }, null, false, false);
        }
        else {
            self.isSaveGridState = true;
        }

    }

    self.showHideLoader = function (isShow) {
        if (isShow) {
            $('#DivLoading').show();
        }
        else {
            $('#DivLoading').hide();
        }
    }

    self.getParameterByName = function (name) {
        name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
        var regexS = "[\\?&]" + name + "=([^&#]*)";
        var regex = new RegExp(regexS);
        var results = regex.exec(window.location.search);
        if (results == null)
            return "";
        else
            return decodeURIComponent(results[1].replace(/\+/g, " "));
    }

    self.detailGrid = {};

    self.detailGrid.isSaveGridState = false;

    self.fillReceivedItemDetailGrid = function (orderDetailID) {

        var oTableRecved = $('#ReceivedItemDetail_' + orderDetailID).dataTable({
            "bJQueryUI": true,
            "bRetrieve": true,
            "bDestroy": true,
            "bScrollCollapse": true,
            "sScrollX": "100%",
            "sDom": 'R<"top"lp<"clear">>rt<"bottom"i<"clear">>',
            "oColVis": {},
            "aaSorting": [[1, "desc"]],
            "oColReorder": {},
            "sPaginationType": "full_numbers",
            "bProcessing": true,
            "bStateSave": true,
            "oLanguage": oLanguage,
            //{
            //    "sLengthMenu": '@eTurns.DTO.Resources.ResGridHeader.Show' + ' _MENU_ ' + '@eTurns.DTO.Resources.ResGridHeader.Records',
            //    "sEmptyTable": '@eTurns.DTO.Resources.ResMessage.NoDataAvailableInTable',
            //    "sInfo": '@eTurns.DTO.Resources.ResMessage.ShowingNoOfEntries',
            //    "sInfoEmpty": '@eTurns.DTO.Resources.ResMessage.ShowingZeroEntries'
            //},
            "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                if ($('#spnOrdStatusText_' + orderDetailID).text().toLowerCase() == 'closed') {
                    $(nRow).find('#aEditLink').css('display', 'none');
                }
            },
            "fnStateSaveParams": function (oSettings, oData) {
                if (self.detailGrid.isSaveGridState) {
                    oData.oSearch.sSearch = "";
                    _AjaxUtil.postJson(_ReceiveList.urls.saveGridStateURL, { Data: JSON.stringify(oData), ListName: 'ReceivedItemDetailGrid' }
                        , function (json) {
                            if (json.jsonData != '')
                                o = json;
                        }, null, false, false
                    );
                }

                self.detailGrid.isSaveGridState = true;
            },
            "fnStateLoad": function (oSettings) {
                var o;

                _AjaxUtil.postJson(_ReceiveList.urls.loadGridStateURL, { ListName: 'ReceivedItemDetailGrid' }
                    , function (json) {
                        if (json.jsonData != '')
                            o = JSON.parse(json.jsonData);
                    }
                    , null, false, false);


                return o;
            }
            // , "aoColumns": ColumnObject
        });

    }

    return self;
})(jQuery);

//var _AjaxUtil = (function () {
//    var self = {};

//    self.getText = function (url, data, fnSuccess, fnError, async, cache) {

//        if (typeof cache === 'undefined' || cache == null) {
//            cache = true;
//        }

//        if (typeof async === 'undefined' || async == null) {
//            async = true;
//        }

//        var jqXHR = $.ajax({
//            "url": url,
//            "type": 'GET',
//            "data": data,
//            "async": async,
//            "cache": cache,
//            "dataType": "text",
//            "success": function (json) {
//                if (fnSuccess != null && typeof fnSuccess === 'function') {
//                    fnSuccess(json);
//                }
//            },
//            error: function (response) {
//                if (fnError != null && typeof fnError === 'function') {
//                    fnError(response);
//                }
//            }
//        });

//        return jqXHR;
//    }

//    self.getJson = function (url, data, fnSuccess, fnError, async, cache) {

//        if (typeof cache === 'undefined' || cache == null) {
//            cache = true;
//        }

//        if (typeof async === 'undefined' || async == null) {
//            async = true;
//        }

//        var jqXHR = $.ajax({
//            "url": url,
//            "type": 'GET',
//            "data": data,
//            "async": async,
//            "cache": cache,
//            "dataType": "json",
//            "success": function (json) {
//                if (fnSuccess != null && typeof fnSuccess === 'function') {
//                    fnSuccess(json);
//                }
//            },
//            error: function (response) {
//                if (fnError != null && typeof fnError === 'function') {
//                    fnError(response);
//                }
//            }
//        });

//        return jqXHR;
//    }

//    self.postJson = function (url, data, fnSuccess, fnError, async, cache, fnBeforeSend, fnComplete, headers) {

//        if (typeof cache === 'undefined' || cache == null) {
//            cache = true;
//        }

//        if (typeof async === 'undefined' || async == null) {
//            async = true;
//        }

//        var jqXHR = $.ajax({
//            "url": url,
//            "type": "POST",
//            data: data,
//            "async": async,
//            cache: cache,
//            "dataType": "json",
//            headers: headers,
//            beforeSend: fnBeforeSend,
//            //beforeSend: function () {
//            //    if (fnBeforeSend != null && typeof fnBeforeSend === 'function') {
//            //        fnBeforeSend(json);
//            //    }
//            //},
//            "success": function (json) {
//                if (fnSuccess != null && typeof fnSuccess === 'function') {
//                    fnSuccess(json);
//                }
//            },
//            "error": function (response) {
//                if (fnError != null && typeof fnError === 'function') {
//                    fnError(response);
//                }
//            },
//            complete: function (xhr, status) {
//                // A function to be called when the request finishes (after success and error callbacks are executed)
//                if (fnComplete != null && typeof fnComplete === 'function') {
//                    fnComplete(xhr, status);
//                }
//            }
//        });

//        return jqXHR;
//    }

//    self.postText = function (url, data, fnSuccess, fnError, async, cache, isJsonPara) {

//        if (typeof cache === 'undefined' || cache == null) {
//            cache = true;
//        }

//        if (typeof async === 'undefined' || async == null) {
//            async = true;
//        }

//        var contentType = "application/json";

//        if (isJsonPara == false) {
//            contentType = "application/x-www-form-urlencoded; charset=UTF-8";
//        }

//        var jqXHR = $.ajax({
//            "url": url,
//            "type": "POST",
//            data: data,
//            "async": async,
//            cache: cache,
//            "dataType": "text", // dataType is what you're expecting back from the server
//            "contentType": contentType, // contentType is the type of data you're sending
//            "success": function (json) {
//                if (fnSuccess != null && typeof fnSuccess === 'function') {
//                    fnSuccess(json);
//                }
//            },
//            "error": function (response) {
//                if (fnError != null && typeof fnError === 'function') {
//                    fnError(response);
//                }
//            }
//        });
//        return jqXHR;
//    }

//    return self;
//})();

function ShowHideUnCloseButton() {
    $('#btnUnCloseLineItem').css('display', 'none');

    if (_ReceiveList.isUnclose === 'True') {
        $('#myDataTable tbody tr.row_selected').each(function (i) {
            var unCloseVal = $(this).find('#spnIsCloseLineItem').text();
            if ($.trim(unCloseVal).toLowerCase() === 'true') {
                $('#btnUnCloseLineItem').css('display', 'inline');
                return;
            }
        });
    }
}

function fnGetSelected(oTableLocal) {
    return oTableLocal.$('tr.row_selected');
}


// ---------------- script 2 start

function isDuplicateSerial(ordDetaiID) {
    var txtsr = $("#txtsrnumber_" + ordDetaiID);
    var itemGuid = $('#hdnItemGuid_' + ordDetaiID).val();

    var returnResult = false;
    if ($(txtsr).val() != '') {
        var consign = $("#hdnIsConsign_" + ordDetaiID).val();
        fillReceivedDate("txtsrnumber_", ordDetaiID);

        _AjaxUtil.getText(_ReceiveList.urls.duplicateCheckSrNumberURL, { SrNumber: $(txtsr).val(), ID: 0, ItemGUID: itemGuid }
            , function (response) {
                if (response == "duplicate") {
                    $(txtsr).css("background-color", "#F7BBC4");
                    $(txtsr).select();
                    $(txtsr).focus();
                    returnResult = false;
                }
                else {
                    $(txtsr).css("background-color", "");
                    returnResult = true;
                }
            }
            , function (response) {
                $("#spanGlobalMessage").html(response);
                returnResult = false;
            });

        //$.ajax({
        //    "url": _ReceiveList.urls.duplicateCheckSrNumberURL,
        //    data: { SrNumber: $(txtsr).val(), ID: 0, ItemGUID: itemGuid },
        //    "async": false,
        //    cache: false,
        //    "dataType": "text",
        //    "success": function (response) {
        //        if (response == "duplicate") {
        //            $(txtsr).css("background-color", "#F7BBC4");
        //            $(txtsr).select();
        //            $(txtsr).focus();
        //            returnResult = false;
        //        }
        //        else {
        //            $(txtsr).css("background-color", "");
        //            returnResult = true;
        //        }
        //    },
        //    error: function (response) {
        //        $("#spanGlobalMessage").text(response);
        //        returnResult = false;
        //    }
        //});
    }
    else {
        returnResult = true;
    }
    if (returnResult == false) {
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
        $("#spanGlobalMessage").html(MsgDuplicateSerialFound);
        showNotificationDialog();
        //alert(MsgDuplicateSerialFound);
        $(txtsr).focus();
    }

    return returnResult;
}

function fillReceivedDate(txtName, ordDetailID) {
    var txt = $('#' + txtName + ordDetailID);
    if ($(txt).val() != '' && $(txt).val() != '0') {
        var myDate = new Date();
        var prettyDate = (myDate.getMonth() + 1) + '/' + myDate.getDate() + '/' + myDate.getFullYear();
        var seq = $(txt).parent().parent()[0].rowIndex - 1;
        if ($(txt).parent().parent().find('[name*="[' + seq.toString() + '].Received"]').val() == '') {
            $(txt).parent().parent().find('[name*="[' + seq.toString() + '].Received"]').val(prettyDate);
        }
    }
}

function fillReceivedDateQuantity(txtName, ordDetailID) {
    var txt = $('#' + txtName + ordDetailID);
    if ($(txt).val() != '' && parseFloat($(txt).val()) > 0) {
        var myDate = new Date();
        var prettyDate = (myDate.getMonth() + 1) + '/' + myDate.getDate() + '/' + myDate.getFullYear();
        var seq = $(txt).parent().parent()[0].rowIndex - 1;
        if ($(txt).parent().parent().find('[name*="[' + seq.toString() + '].Received"]').val() == '') {
            $(txt).parent().parent().find('[name*="[' + seq.toString() + '].Received"]').val(prettyDate);
        }
    }
}


function fillReceivedItemGrid(ordDetailID) {
    var itemGUID = $("#hdnItemGuid_" + ordDetailID).val();
    var orderDetailGUID = $("#hdnOrderDetailGUID_" + ordDetailID).val();
    var orderGUID = $("#hdnOrderGuid_" + ordDetailID).val();
    var isLot = $("#hdnIsLot_" + ordDetailID).val();
    var isDateCode = $("#hdnIsDateCode_" + ordDetailID).val();
    var isSerial = $("#hdnIsSerial_" + ordDetailID).val();
    var binID = $("#ddlBin_" + ordDetailID).val();
    var obj = {
        "OrderDetailID": ordDetailID,
        "ItemGUID": itemGUID,
        "OrderDetailGUID": orderDetailGUID,
        "DateCodeTracking": isDateCode,
        "SerialNumberTracking": isSerial,
        "LotNumberTracking": isLot,
        "ReceiveBinID": binID,
        "OrderGUID": orderGUID
    }
    var data = JSON.stringify(obj);
    _AjaxUtil.postText(_ReceiveList.urls.receivedItemDetailURL, data,
        function (response) {
            $("#ItemReceivedGrid_" + ordDetailID).html(response);
            $('#DivLoading').hide();
        }, function (err) {
            $('#DivLoading').hide();
        }, false, false
    );

    //var url = _ReceiveList.urls.receivedItemDetailURL;

    //$.ajax({
    //    "url": url,
    //    "data": JSON.stringify(obj),
    //    "type": 'POST',
    //    "async": false,
    //    "cache": false,
    //    "dataType": "text",
    //    "contentType": "application/json",
    //    "success": function (response) {
    //        $("#ItemReceivedGrid_" + ordDetailID).html(response);
    //        $('#DivLoading').hide();
    //    },
    //    "error": function (err) {
    //        $('#DivLoading').hide();
    //    }

    //});
}



function deleteRowsReceived(ordDetailID) {
    var anSelected = fnGetSelected($('#ReceivedItemDetail_' + ordDetailID).dataTable());
    if (anSelected != null) {
        if (anSelected.length > 0) {
            $('#ReceivedOrderInnerGrid_' + ordDetailID).modal();
        }
    }
    return false;
}

function DeleteYesSelectedRows(ordDetailID) {
    $('#DivLoading').show();
    $.modal.impl.close();
    var anSelected = fnGetSelected($('#ReceivedItemDetail_' + ordDetailID).dataTable());
    var stringGUIDs = "";
    if (anSelected.length > 0) {
        for (var i = 0; i <= anSelected.length - 1; i++) {
            stringGUIDs = stringGUIDs + $(anSelected[i]).find("#hdnGUID").val() + ",";
        }
    }

    if (stringGUIDs.length > 0) {
        var ordDetailGUID = $("#hdnOrderDetailGUID_" + ordDetailID).val();
        var itemGUID = $("#hdnItemGuid_" + ordDetailID).val();

        var fnSuccess = function (response) {
            if (response == "ok") {
                NewUpdateReceivedQty(ordDetailID, ordDetailGUID);
                for (var i = 0; i <= anSelected.length - 1; i++) {
                    $('#ReceivedItemDetail_' + ordDetailID).dataTable().fnDeleteRow(anSelected[i]);
                }
                if (anSelected.length > 0)
                    $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon').html(MsgRecordDeletedSuccessfully.replace("{0}", anSelected.length));

                // $('#ReceivedItemDetail_' + ordDetailID).dataTable().fnDraw();
                $('#DivLoading').hide();
                showNotificationDialog();
                ImageIDToOpen = "#imgPlusMinus_" + ordDetailID;
                $('#myDataTable').dataTable().fnStandingRedraw();
                $('#DivLoading').hide();
                return false;
            }
            else {
                $("#spanGlobalMessage").removeClass('WarningIcon succesIcon').addClass('errorIcon').html(MsgRecordsNotDeleted);
                $('#DivLoading').hide();
                showNotificationDialog();
                return false;
            }

        }
        var ajaxData = JSON.stringify({ 'ItemGUID': itemGUID, 'ordDetailGUID': ordDetailGUID, 'deleteIDs': stringGUIDs });
        _AjaxUtil.postText(_ReceiveList.urls.deleteRecieveAndUpdateReceivedQtyURL, ajaxData
            , fnSuccess, function (err) {
                $('#DivLoading').hide();
            }, false, false);

    }

}

function NewUpdateReceivedQty(ordDetailID, ordDetailGUID) {
    _AjaxUtil.postJson(_ReceiveList.urls.getReceivedQuantityURL, { 'OrderDetailGUID': ordDetailGUID }
        , function (response) {
            if (response.Status = "ok") {
                var isSerial = $("#hdnIsSerial_" + ordDetailID).val();
                //$('#spnReceivedQty_' + ordDetailID).text(response.ReturnDTO.ReceivedQuantity);
                var RecvedQty = parseFloat(response.ReturnDTO.ReceivedQuantity);
                var ReqstedQty = parseFloat($("#hdnRequestedQty_" + ordDetailID).val());
                if (isSerial != "True") {
                    $("#txtReceiveQuantity_" + ordDetailID).val((ReqstedQty - RecvedQty).toString());
                }
            }
        }, null, false, false);
    //$.ajax({
    //    'url': _ReceiveList.urls.getReceivedQuantityURL,
    //    'type': 'POST',
    //    'data': { 'OrderDetailGUID': ordDetailGUID },
    //    'async': false,
    //    'cache': false,
    //    'success': function (response) {
    //        if (response.Status = "ok") {
    //            var isSerial = $("#hdnIsSerial_" + ordDetailID).val();
    //            //$('#spnReceivedQty_' + ordDetailID).text(response.ReturnDTO.ReceivedQuantity);
    //            var RecvedQty = parseFloat(response.ReturnDTO.ReceivedQuantity);
    //            var ReqstedQty = parseFloat($("#hdnRequestedQty_" + ordDetailID).val());
    //            if (isSerial != "True") {
    //                $("#txtReceiveQuantity_" + ordDetailID).val((ReqstedQty - RecvedQty).toString());
    //            }
    //        }
    //    }
    //});
}


function closeOrdReceiveInfoModel() {
    $.modal.impl.close();
}
function closeErrorDialog() {
    $.modal.impl.close();
    return false;
}

function closeFromOk() {
    $.modal.impl.close();
    $('#DivLoading').show();
    setTimeout(function () { $("#btnClearAll").click(); $('#myDataTable').dataTable().fnStandingRedraw(); }, 3000);
}

function ExceedRecieveModelYesClick() {
    closeOrdReceiveInfoModel();
    $('#OrdReceiveProcessing').modal();
    $('#OrdReceiveProcessing').parent().parent().find(".modalCloseImg").css('display', 'none');
    setTimeout(function () { ReceiveAll(); }, 1000);
}

function ReceiveAll() {
    $('#DivLoading').show();
    $('#myDataTable tbody tr').each(function (i) {
        var colr = hexc($(this).css('background-color'));
        if (colr !== '#d3d3d3') {
            $(this).removeAttr('style');
        };
        //$(this).removeAttr('style');
    });

    var ItemSelected = fnGetSelected(oTable);
    var isError = false;
    var ErrorMessage = "";
    var errorIndex = 0;
    var arrObj = new Array();
    var objRODDTO = {};

    var ItemSelected = fnGetSelected(oTable);
    var trcolor = '';

    if (!isNaN(ItemSelected.length) && ItemSelected.length > 0) {
        arrObj = new Array();
        objRODDTO = {};
        isError = false;
        ErrorMessage = "";
        errorIndex = 0;
        trcolor = '';
        var duration = 500;
        for (var i = 0; i < ItemSelected.length; i++) {
            var rowPosition = oTable.fnGetPosition(ItemSelected[i]);
            var aData = oTable.fnGetData(rowPosition);
            var approvQty = aData.ApprovedQuantity;
            var receiveQty = aData.ReceivedQuantity;

            //setTimeout(function () {
            duration = 500;
            if (i === ItemSelected.length) {
                duration = 1;
            }

            var binID = 0;
            var binNumber = $(ItemSelected[i]).find("#txtBinNumber").val();
            var OrderStatus = $(ItemSelected[i]).find("span[id*='spnOrdStatusText_']").text();

            var recvQty = $(ItemSelected[i]).find("input[type='text'][id^='txtQtyToRecv']").val();
            var recvDate = _ReceiveList.recvDate;
            var itemGUID = aData.ItemGUID;
            var orderGUID = aData.OrderGUID;
            var orderDetailGUID = aData.OrderDetailGUID;
            var recvcost = aData.ItemCost;
            var itemNumber = aData.ItemNumber;
            var stagingID = aData.StagingID;
            var ReceiveUDF1 = '';
            var ReceiveUDF2 = '';
            var ReceiveUDF3 = '';
            var ReceiveUDF4 = '';
            var ReceiveUDF5 = '';
            var packslip = $(ItemSelected[i]).find("span[id^='spnPackSlipNumber_']").text();

            var IsCloseLineItem = '';
            IsCloseLineItem = $(ItemSelected[i]).find("span[id^='spnIsCloseLineItem']").text().trim();

            var isPackslipMandatory = aData.IsPackSlipNumberMandatory;

            if ($(ItemSelected[i]).find("#UDF1").length > 0) {
                if ($(ItemSelected[i]).find("#UDF1")[0].nodeName === "SELECT")
                    ReceiveUDF1 = $(ItemSelected[i]).find("#UDF1 option:selected").text();
                else
                    ReceiveUDF1 = $(ItemSelected[i]).find("#UDF1").val();
            }
            if ($(ItemSelected[i]).find("#UDF2").length > 0) {
                if ($(ItemSelected[i]).find("#UDF2")[0].nodeName === "SELECT")
                    ReceiveUDF2 = $(ItemSelected[i]).find("#UDF2 option:selected").text();
                else
                    ReceiveUDF2 = $(ItemSelected[i]).find("#UDF2").val();
            }
            if ($(ItemSelected[i]).find("#UDF3").length > 0) {
                if ($(ItemSelected[i]).find("#UDF3")[0].nodeName === "SELECT")
                    ReceiveUDF3 = $(ItemSelected[i]).find("#UDF3 option:selected").text();
                else
                    ReceiveUDF3 = $(ItemSelected[i]).find("#UDF3").val();
            }
            if ($(ItemSelected[i]).find("#UDF4").length > 0) {
                if ($(ItemSelected[i]).find("#UDF4")[0].nodeName === "SELECT")
                    ReceiveUDF4 = $(ItemSelected[i]).find("#UDF4 option:selected").text();
                else
                    ReceiveUDF4 = $(ItemSelected[i]).find("#UDF4").val();
            }
            if ($(ItemSelected[i]).find("#UDF5").length > 0) {
                if ($(ItemSelected[i]).find("#UDF5")[0].nodeName === "SELECT")
                    ReceiveUDF5 = $(ItemSelected[i]).find("#UDF5 option:selected").text();
                else
                    ReceiveUDF5 = $(ItemSelected[i]).find("#UDF5").val();
            }

            if (IsCloseLineItem != 'true') {
                if (OrderStatus.toLowerCase() != 'c' && OrderStatus.toLowerCase() != 'closed') {
                    if (binNumber !== undefined && $.trim(binNumber).length > 0 || parseInt(stagingID) > 0) {
                        if (isNaN(parseFloat(recvQty)) === false && parseFloat(recvQty) > 0) {

                            objROTDDTO = {
                                "BinID": binID, "Received": recvDate, "Cost": recvcost
                                , "ItemGUID": itemGUID, "ReceivedDate": recvDate
                                , "UDF1": ReceiveUDF1, "UDF2": ReceiveUDF2
                                , "UDF3": ReceiveUDF3, "UDF5": ReceiveUDF4
                                , "UDF5": ReceiveUDF5
                            };

                            arrObj.push(objROTDDTO);
                            objRODDTO = {
                                "OrderGUID": orderGUID, "ItemGUID": itemGUID,
                                "ReceiveBinID": binID, "ReceivedQuantity": recvQty,
                                "OrderRequiredDate": recvDate, "OrderDetailGUID": orderDetailGUID,
                                "StagingID": stagingID, 'ReceiveBinName': binNumber,
                                "IsPackSlipNumberMandatory": isPackslipMandatory,
                                "PackSlipNumber": packslip,
                                "ReceivedItemDetail": arrObj
                            };

                            var fnSuccess = function (response) {
                                if (response.Success === "ok") {
                                    UDFInsertNewForGrid($(ItemSelected[i]));
                                    var qtyToRecv = parseFloat(aData.ApprovedQuantity) - parseFloat(response.ReceivedQty);
                                    if (isNaN(parseFloat(qtyToRecv)) === true || parseFloat(qtyToRecv) <= 0) {
                                        qtyToRecv = 0;
                                    }
                                    $(ItemSelected[i]).find("input[type='text'][id^='txtQtyToRecv']").val(qtyToRecv.toFixed(qtyFixedLength));
                                    trcolor = "Green";
                                }
                                else if (response.Status === "fail" && response.Message !== undefined && response.Message.length > 0) {
                                    isError = true;
                                    errorIndex = errorIndex + 1;
                                    ErrorMessage += "<br/><b style='color:Olive'>" + errorIndex + ") " + itemNumber + ": " + response.Message + "</b>";
                                    trcolor = "Olive";
                                }
                                else {
                                    isError = true;
                                    errorIndex = errorIndex + 1;
                                    ErrorMessage += "<br/><b style='color:Red'>" + errorIndex + ") " + itemNumber + ": " + MsgErrorInAjaxrequest+"</b>";
                                    trcolor = "Red";
                                }
                            }

                            _AjaxUtil.postJson(_ReceiveList.urls.saveReceiveWithOrderURL, JSON.stringify(objRODDTO)
                                , fnSuccess, function (xhr) {
                                    isError = true;
                                    errorIndex = errorIndex + 1;
                                    ErrorMessage += "<br/><b style='color:Red'>" + errorIndex + ") " + itemNumber + ": " + MsgErrorInAjaxrequest+" </b>";
                                    trcolor = "Red";
                                }, false, false);

                            //$.ajax({
                            //    "url": _ReceiveList.urls.saveReceiveWithOrderURL,
                            //    "type": "POST",
                            //    "dataType": "json",
                            //    "contentType": "application/json",
                            //    "data": JSON.stringify(objRODDTO),
                            //    "async": false,
                            //    "cache": false,
                            //    "success": function (response) {
                            //        if (response.Success === "ok") {
                            //            UDFInsertNewForGrid($(ItemSelected[i]));
                            //            var qtyToRecv = parseFloat(aData.ApprovedQuantity) - parseFloat(response.ReceivedQty);
                            //            if (isNaN(parseFloat(qtyToRecv)) === true || parseFloat(qtyToRecv) <= 0) {
                            //                qtyToRecv = 0;
                            //            }
                            //            $(ItemSelected[i]).find("input[type='text'][id^='txtQtyToRecv']").val(qtyToRecv.toFixed(qtyFixedLength));
                            //            trcolor = "Green";
                            //        }
                            //        else if (response.Status === "fail" && response.Message !== undefined && response.Message.length > 0) {
                            //            isError = true;
                            //            errorIndex = errorIndex + 1;
                            //            ErrorMessage += "<br/><b style='color:Olive'>" + errorIndex + ") " + itemNumber + ": " + response.Message + "</b>";
                            //            trcolor = "Olive";
                            //        }
                            //        else {
                            //            isError = true;
                            //            errorIndex = errorIndex + 1;
                            //            ErrorMessage += "<br/><b style='color:Red'>" + errorIndex + ") " + itemNumber + ": Server error</b>";
                            //            trcolor = "Red";
                            //        }
                            //    },
                            //    "error": function (xhr) {
                            //        isError = true;
                            //        errorIndex = errorIndex + 1;
                            //        ErrorMessage += "<br/><b style='color:Red'>" + errorIndex + ") " + itemNumber + ": Server error </b>";
                            //        trcolor = "Red";
                            //    }
                            //});



                            //End Ajax Call


                        }
                        else {
                            trcolor = "Olive";
                            isError = true;
                            $('#DivLoading').hide();
                            errorIndex = errorIndex + 1;
                            ErrorMessage += "<br/><b style='color:Olive'>" + errorIndex + ") " + itemNumber + ": " + MsgEnterQuantityReceive+" </b>";
                        }
                    }
                    else {
                        trcolor = "Olive";
                        isError = true;
                        $('#DivLoading').hide();
                        errorIndex = errorIndex + 1;
                        ErrorMessage += "<br/><b style='color:Olive'>" + errorIndex + ") " + itemNumber + ": " + MsgSelectLocationToReceive+"</b>";
                    }
                }
                else {

                    trcolor = "Olive";
                    isError = true;
                    $('#DivLoading').hide();
                    errorIndex = errorIndex + 1;
                    ErrorMessage += "<br/><b style='color:Olive'>" + errorIndex + ") " + itemNumber + ": " + MsgClosedOrderNotReceive+" </b>";
                }
            }
            $(ItemSelected[i]).removeClass('row_selected');
            $(ItemSelected[i]).css('background-color', trcolor);
        }
        $('#DivLoading').hide();
    }
    else {
        ErrorMessage = "<b>" + MsgSelectRowToReceive+" </b>";
        $('#OrdReceivedInfoDialog').find("#OrdReceivedMSG").html(ErrorMessage);
        closeOrdReceiveInfoModel();
        $('#OrdReceivedInfoDialog').modal();
        $('#DivLoading').hide();
        $("#btnReceiveALL").removeAttr('disabled');
    }

    if (isError) {

        setTimeout(function () {
            ErrorMessage = '<b>'+ SomeItemsNotReceivedDueToReason +'</b><br />' + ErrorMessage;
            $('#OrdReceivedInfoDialog').find("#OrdReceivedMSG").html(ErrorMessage);
            closeOrdReceiveInfoModel();
            $('#OrdReceivedInfoDialog').modal();
            $('#DivLoading').hide();
        }, 1000);
        $("#btnReceiveALL").removeAttr('disabled');
    }
    else {
        isDirtyForm = false;
        setTimeout(function () {
            $('#OrdReceivedInfoDialog').find("#OrdReceivedMSG").html("<b style='color:green'>" + MsgReceivedSuccessfully+"</b>");
            closeOrdReceiveInfoModel();
            SetReplenishRedCount();
            $('#OrdReceivedInfoDialog').modal();
            $('#DivLoading').hide();
        }, 500);
        $("#btnReceiveALL").removeAttr('disabled');
    }

}

function OrdersTabClick() {
    var ItemSelected = fnGetSelected(oTable);
    if (ItemSelected != undefined && ItemSelected.length == 1) {
        var ItemGUID = $(ItemSelected).find('#spnItemID').text();
        if (ItemGUID != '') {
            $.get('LoadReceiveOrders?ItemGUID=' + ItemGUID.toString(), {}, function (data) { $('#CtabCL').html(data); }, "html");
        }
    }
    else {
        $('#CtabCL').html('');
        $("#spanGlobalMessage").html(MsgSelectRecordToOrders);
        showNotificationDialog();
        return false;
    }
}


function UpdateReceiveQty(OrdDetailID, UniqueID) {
    $('#DivLoading').show();

    _AjaxUtil.getJson(_ReceiveList.urls.getReceivedQuantityURL, { 'OrderDetailGUID': OrdDetailID }
        , function (response) {
            if (response.Status = "ok") {
                $('#lblReceiveQuantityDisp' + UniqueID).html(response.ReturnDTO.ReceivedQuantity);
            }
            $('#DivLoading').hide();
        }, function (xhr) {
            $('#DivLoading').hide();
        }
    );

}

function getItemInventoryStagingLocation(dropdownobj, binNumber, itmGuid, orderGuid,tdwidth) {

    var selval = $(dropdownobj).val();
    if (!$(dropdownobj).hasClass("populated")) {
        $(dropdownobj).html("");
        var stroptions = "";
        var stroptions = "<option value=''></option>";

        _AjaxUtil.postJson(_ReceiveList.urls.getItemLocationsURL, { 'OrderGuid': orderGuid, 'ItemGuid': itmGuid, 'NameStartWith': '' }
            , function (response) {
                $(response).each(function (indx, obj) {
                    if (selval == obj.Key) {
                        stroptions = stroptions + "<option selected='selected' value='" + obj.Key + "'>" + obj.Key + "</option>";
                    }
                    else {
                        stroptions = stroptions + "<option value='" + obj.Key + "'>" + obj.Key + "</option>";
                    }

                });
                $(dropdownobj).html(stroptions);
                $(dropdownobj).addClass("populated");
                if (typeof (tdwidth) != "undefined" && tdwidth != null && tdwidth.length > 0) {
                    $(dropdownobj).css('max-width', tdwidth);
                    $(dropdownobj).css($(dropdownobj).width() - 21 + "px");
                    $(dropdownobj).css('overflow-x', 'auto');
                }
            }, function (response) {
            }, false, false);

        //$.ajax({
        //    "url": _ReceiveList.urls.getItemLocationsURL, //'/Receive/GetItemLocations',
        //    "type": "POST",
        //    "data": { 'OrderGuid': orderGuid, 'ItemGuid': itmGuid, 'NameStartWith': '' },
        //    "async": false,
        //    "cache": false,
        //    "dataType": "json",
        //    "success": function (response) {
        //        $(response).each(function (indx, obj) {
        //            if (selval == obj.Key) {
        //                stroptions = stroptions + "<option selected='selected' value='" + obj.Key + "'>" + obj.Key + "</option>";
        //            }
        //            else {
        //                stroptions = stroptions + "<option value='" + obj.Key + "'>" + obj.Key + "</option>";
        //            }

        //        });
        //        $(dropdownobj).html(stroptions);
        //        $(dropdownobj).addClass("populated");
        //    },
        //    "error": function (response) {
        //    }
        //});
    }
}

function ShowHideButtons() {
    if ($('#myDataTable tbody tr.row_selected').length == 1) {
        var ItemSelected = fnGetSelected(oTable);

        if (ItemSelected.length > 0) {
            var rowPosition = oTable.fnGetPosition(ItemSelected[0]);
            var aData = oTable.fnGetData(rowPosition);
            var OrderStatus = $(ItemSelected[0]).find("span[id*='spnOrdStatusText_']").text();
            if (OrderStatus !== "T") {
                $('#btnEditReciept').css('display', '');
            }
            else {
                $('#btnEditReciept').css('display', 'none');
            }
        }
    }
    else {
        $('#btnEditReciept').css('display', 'none');
    }
}

function closeModalCloseOderDialog() {
    $.modal.impl.close();
}


// tabs 
function callbacknew() {
    //$('#NarroSearchClearSC').click();
    ReceiveIncomplateTab = false;
    window.location.hash = '#new';
    $('#DivLoading').show();
    $("#CtabNew").html('');
    LoadNewReceive("LoadAllItems");
}

function callbackhistory() {
    $('#NarroSearchClearSC').click();
    window.location.hash = '#list';
    ReceiveIncomplateTab = false;
    if (ReceiveIncomplateTab)
        _NarrowSearchSave.currentListName = "ReceiveMasterIncomplete";
    else
        _NarrowSearchSave.currentListName = "ReceiveMaster";

    _NarrowSearchReceiveLayout.GetReceiveNarrowSearches('ReceiveMaster', false, false);
    var OrderStatuses = '4,5,6,7,8'
    if ($('#tab4').hasClass('selected')) {
        OrderStatuses = '4,5,6,7'
        //$('#btnEditReciept').css('display', 'none');
    }
    else {
        //$('#btnEditReciept').css('display', 'none');
    }

    //GetNarroFromItemHTMLForUDF('ReceiveList', '@eTurnsWeb.Helper.SessionHelper.CompanyID', '@eTurnsWeb.Helper.SessionHelper.RoomID', false, false, OrderStatuses);
    _NarrowSearchReceiveLayout.GetNarroFromItemHTMLForUDF('ReceiveList', _ReceiveList.model.companyId, _ReceiveList.model.roomId, false, false, OrderStatuses);

    $('#myDataTable').dataTable().fnDraw();
} //HistoryTabClick(); }

function callbackIncomplete() {
    $('#NarroSearchClearSC').click();
    window.location.hash = '#Incomplete';
    ReceiveIncomplateTab = true;
    if (ReceiveIncomplateTab) 
        _NarrowSearchSave.currentListName = "ReceiveMasterIncomplete";    
    else
        _NarrowSearchSave.currentListName = "ReceiveMaster";

    _NarrowSearchReceiveLayout.GetReceiveNarrowSearches('ReceiveMaster', false, false);
    var OrderStatuses = '4,5,6,7,8'
    if ($('#tab4').hasClass('selected')) {
        OrderStatuses = '4,5,6,7'
        //$('#btnEditReciept').css('display', 'none');
    }
    else {
        //$('#btnEditReciept').css('display', 'none');
    }

    //GetNarroFromItemHTMLForUDF('ReceiveList', '@eTurnsWeb.Helper.SessionHelper.CompanyID', '@eTurnsWeb.Helper.SessionHelper.RoomID', false, false, OrderStatuses);
    _NarrowSearchReceiveLayout.GetNarroFromItemHTMLForUDF('ReceiveList', _ReceiveList.model.companyId, _ReceiveList.model.roomId, false, false, OrderStatuses);

    $('#myDataTable').dataTable().fnDraw();
}

function callbackOrder() { window.location.hash = ''; OrdersTabClick(); }

function LoadNewReceive(action) {
    $("#CtabNew").html('<div id="DivLoading" class="DivLoadingProcessing" style=""></div >');
    $.get(action, function (data) {
        $("#CtabNew").html(data);
        //if (_NarrowSearchSave.isPageLoading == false) {
        //    setTimeout(function () {
        //        _NarrowSearchSave.loadNarrowSearch();
        //        setTimeout(function () {
        //            DoNarrowSearchIM();
        //        }, 800);
        //    }, 300);
        //}
    });
}
$('#txtOrderFilter').keyup(function (evt) {
    if (evt.keyCode === 27 || evt.key === "Escape" || evt.key === "Esc") {
        $('#txtOrderFilter').val('');
        DoOrderNarrowSearch();
    }
});



function saveReceiveFiles(objtr,result) {
    //selected Receieve items
    for (var i = 0; i < objtr.length; i++) {
        var receivedDetailGuid;
        var isValidFileAvailable = false;
        if ($(objtr[i]).find("#spnOrderDetailGUID").length > 0 && $(objtr[i]).find("#spnOrderDetailID").length > 0) {
            var orderDetailsGUID = $(objtr[i]).find("#spnOrderDetailGUID").text().trim();
            var ID = $(objtr[i]).find("#spnOrderDetailID").text().trim();
            var TotalFiles = document.getElementById("file_" + orderDetailsGUID) != null ? document.getElementById("file_" + orderDetailsGUID).files.length : 0;
            if (TotalFiles > 0) {
                for (var j = 0; j < TotalFiles; j++) {
                    var validExtension = ReceivedFileExtension.split(',');
                    var fileExt = document.getElementById("file_" + orderDetailsGUID).files[j].name;
                    fileExt = fileExt.substring(fileExt.lastIndexOf('.'));
                    if (validExtension.indexOf(fileExt.toLowerCase()) <= -1) {
                        // do nothing for invalid files
                    } else {
                        var isavailable = removedReceiveListName.filter(function (item) {
                            return item.id == orderDetailsGUID && item.fileName === document.getElementById("file_" + orderDetailsGUID).files[j].name
                        });
                        if (isavailable.length == 0) {
                            formData.append("file" + orderDetailsGUID + "" + j.toString(), document.getElementById("file_" + orderDetailsGUID).files[j]);
                            isValidFileAvailable = true;
                        }
                    }
                }
                if (isValidFileAvailable && result != null && result.OrderDetailReceived != null) {
                    if (result.OrderDetailReceived.length > 0) {
                        for (var i = 0; i < result.OrderDetailReceived.length; i++) {
                            if (result.OrderDetailReceived[i].OrderDetailsGuid == orderDetailsGUID) {
                                receivedDetailGuid = result.OrderDetailReceived[i].ReceivedDetailGuid;
                            }
                        }
                    }
                    if (receivedDetailGuid != "00000000-0000-0000-0000-000000000000" && orderDetailsGUID != "00000000-0000-0000-0000-000000000000") {
                        //Ajax call
                        $.ajax({
                            url: "/api/fileupload/ReceiveFileUpload/" + ID + "?OrderDetailsGUID=" + orderDetailsGUID + "&receivedDetailGuid=" + receivedDetailGuid,
                            type: 'post',
                            data: formData,
                            dataType: 'html', // we return html from our file
                            async: false,
                            processData: false,  // tell jQuery not to process the data
                            contentType: false,   // tell jQuery not to set contentType
                            success: function (data) {
                                formData = new FormData();
                                document.getElementById("file_" + orderDetailsGUID).value = null;
                                deleteFromRemovedList(orderDetailsGUID);
                                //$('#upload-result').append('<div class="alert alert-success"><p>File(s) uploaded successfully!</p><br />');
                                //$('#upload-result .alert').append(data);
                            },
                            error: function (request) {
                                formData = new FormData();
                                console.log(request.responseText);

                            }
                        });
                    } else {
                        formData = new FormData();
                    }
                }


            }
        }
    }
}

$("#myDataTable").on("click", ".receiveattachmentdelete", function () {
    var ID = $(this).attr("data-id");
    if ($(this).attr("data-id").indexOf("_") > 0) {
        var dataID = $(this).attr("data-id").split("_")[1].length;
        if (dataID > 0) {
            ID = $(this).attr("data-id").split("_")[1];
        }
    }
    var FILENAME = $(this).attr("data-filename");
    var isavailable = removedReceiveListName.filter(function (item) {
        return item.id == ID && item.fileName === FILENAME
    });
    if (isavailable.length == 0) {
        removedReceiveListName.push({
            id: ID,
            fileName: FILENAME
        });
    }
    $(this).prev().remove();
    $(this).remove();
});



$("#myDataTable").on("click", ".receivedattachmentdelete", function () {
    var ID = $(this).attr("data-id");
    $('#DivLoading').show();
    $.ajax({
        'url': "/Receive/DeleteFileAttachment",
        'type': 'POST',
        'data': JSON.stringify({ 'FileGuid': ID }),
        'async': false,
        'cache': false,
        'dataType': 'json',
        'contentType': 'application/json',
        'success': function (response) {
            $('#DivLoading').hide();

        },
        'error': function (err) {
            $('#DivLoading').hide();
        }
    });
    $(this).prev().remove();
    $(this).remove();
});



function deleteFromRemovedList(ID) {
    var removedCount = removedReceiveListName.length;
    for (var i = 0; i <= removedCount; i++) {
        var isavailable = removedReceiveListName.filter(function (item) {
            return item.id == ID;
        });
        if (isavailable.length > 0) {
            var Index = removedReceiveListName.indexOf(isavailable[0]);
            if (Index >= 0) {
                removedReceiveListName.splice(Index, 1);
            }
        }
    }

}

function PrintReceivedAttachedDocs(lnk) {
    var rowSelected = fnGetSelected(oTable);
    var arrIds = new Array();
    if (rowSelected.length > 0) {
        for (var i = 0; i < rowSelected.length; i++) {
            var ReqGUID = $(rowSelected[i]).find("#spnOrderDetailGUID").text();
            arrIds.push(ReqGUID);
        }
    }

    if (arrIds.length) {
        $.ajax({
            url: 'DownloadReceiveDocument',
            type: 'Post',
            data: JSON.stringify(arrIds),
            dataType: 'json',
            async: false,
            contentType: 'application/json',
            success: function (result) {
                if (result.Status) {
                    var isURLHttps = false;
                    var CurrentURL = window.location.href;
                    if (CurrentURL.indexOf("localhost:") < 0
                        && CurrentURL.toLowerCase().indexOf("demo") < 0) {
                        if (CurrentURL.indexOf("https:") >= 0) {
                            isURLHttps = true;
                        }
                    }

                    if (result.ReturnFiles.length > 0) {
                        for (var i = 0; i < result.ReturnFiles.length; i++) {
                            if (isURLHttps == false) {
                                if (result.ReturnFiles[i].indexOf("https:") >= 0) {
                                    result.ReturnFiles[i] = result.ReturnFiles[i].replace("https:", "http:");
                                }
                            }
                            if (isURLHttps == true) {
                                if (result.ReturnFiles[i].indexOf("http:") >= 0) {
                                    result.ReturnFiles[i] = result.ReturnFiles[i].replace("http:", "https:");
                                }
                            }
                            window.open(result.ReturnFiles[i]);
                        }
                    } else {
                        $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
                        $("#spanGlobalMessage").html('@ResCommon.NoDocsToDownload');
                        showNotificationDialog();
                    }
                }
            },
            error: function (xhr) {
                alert(MsgResErrorInProcess);
            }
        });
    }
    else {
        alert(MsgSelectRow);

    }
}


// ---------------- script 2 end

//----------------------------------Moved from page End-----------------------------
