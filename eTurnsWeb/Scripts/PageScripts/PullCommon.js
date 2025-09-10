var _pullCommon = (function ($) {

    var self = {};

    self.isNewConsumePullObj = function () {
        return typeof _NewConsumePull !== 'undefined' && _NewConsumePull !== null;
    };

    self.getBinId = function ($tr) {
        var vBinID = 0;

        if (self.isNewConsumePullObj()) {
            vBinID = _NewConsumePull.getDataFromRow($tr, 'BinID');
        }
        else {
            vBinID = $tr.find('#BinID')[0].value == '' ? 0 : $tr.find('#BinID')[0].value;
        }

        return vBinID;
    }

    self.GetUdfVal = function ($udf) {
        var val = "";
        if ($udf != null) {
            if ($udf.attr("class") == 'selectBox' || $udf.attr("class") == 'selectBox loadondemandudf') {
                val = $udf.find("option:selected").text(); //$("#UDF1PullCommon option:selected").text();
            }
            else {
                val = $udf.val();
            }
        }
        return val;
    }

    self.GetProjectId = function ($tre) {
        var vProjectID = "";
        if (self.isNewConsumePullObj()) {
            vProjectID = _NewConsumePull.GetProjectId($tre);
        }
        else {
            if ($tre.find('#ProjectID')[0] == undefined) {
                vProjectID = "";
            }
            else {
                vProjectID = $tre.find('#ProjectID')[0].value == "" ? "" : $tre.find('#ProjectID')[0].value;
            }
        }
        return vProjectID;
    };


    self.getspnDefaultPullQuantity = function ($tr) {
        if (self.isNewConsumePullObj()) {
            return _NewConsumePull.getDataFromRow($tr, 'spnDefaultPullQuantity');
        }
        else {
            return $tr.find('#spnDefaultPullQuantity').text() == "" ? 0 : $tr.find('#spnDefaultPullQuantity').text();
        }
    };

    self.getItemDataFromRow = function ($tr) {
        var itemData = null;
        if (self.isNewConsumePullObj()) {
            itemData = _NewConsumePull.getItemDataFromRow($tr);
        }
        return itemData;
    }

    self.getspnOrderItemType = function ($tr) {
        var val;
        if (self.isNewConsumePullObj()) {
            var itemData = _NewConsumePull.getItemDataFromRow($tr);
            val = itemData.spnOrderItemType;
        }
        else {
            val = $tr.find('#spnOrderItemType').text();
        }

        return val;
    }

    return self;

})(jQuery); //_pullCommon end

var PullPageName = "";
function OpenPullPopup(btnObj) {
    $(btnObj).attr("disabled", "disabled");
    var errorMsg = '';
    $('#DivLoading').show();
    var PullInfo = new Array();
    if ($(btnObj).prop("id") == "btnPullAllNewFlow") {
        $('#ItemModeDataTable').find("tbody").find("tr.row_selected").each(function (index, tr) {
            var $tr = $(tr);
            var aPos = $('#ItemModeDataTable').dataTable().fnGetPosition($tr[0]),
                aData = $('#ItemModeDataTable').dataTable().fnGetData(aPos);
            errorMsg = '';
            var txxt = $tr.find('#txtQty'),
                vBinID,
                vProjectID,
                vProjectSpendName = '';
            var itemType = _pullCommon.getspnOrderItemType($tr); //$(tr).find('#spnOrderItemType').text();
            var txtQty = txxt.val();
            if (itemType != '4') {
                vBinID = _pullCommon.getBinId($tr);  //$(tr).find('#BinID')[0].value == '' ? 0 : $(tr).find('#BinID')[0].value;

                if ($("#chkUsePullCommonUDF").is(":checked")) {
                    if ($('#ProjectIDCommon') != undefined) {
                        vProjectID = $('#ProjectIDCommon').val() == "" ? "" : $('#ProjectIDCommon').val();
                    }
                    else {
                        vProjectID = "";
                    }
                }
                else {
                    //if ($(tr).find('#ProjectID')[0] == undefined) {
                    //    vProjectID = "";
                    //}
                    //else {
                    //    vProjectID = $(tr).find('#ProjectID')[0].value == "" ? "" : $(tr).find('#ProjectID')[0].value;
                    //}
                    vProjectID = _pullCommon.GetProjectId($tr);
                }

                if ($("#chkUsePullCommonUDF").is(":checked")) {
                    if ($('#txtProjectSpentCommon') != undefined)
                        vProjectSpendName = $('#txtProjectSpentCommon').val() == "" ? "" : $('#txtProjectSpentCommon').val();
                    else
                        vProjectSpendName = "";
                }
                else {
                    if ($tr.find('#txtProjectSpent')[0] == undefined) {
                        vProjectSpendName = "";
                    }
                    else {
                        vProjectSpendName = $tr.find('#txtProjectSpent')[0].value == "" ? "" : $tr.find('#txtProjectSpent')[0].value;
                    }
                }

                if (!(!isNaN(parseFloat(txtQty)) && parseFloat(txtQty) > 0)) {
                    $tr.css('background-color', 'Olive');
                    IsGlobalErrorMsgShow = true;
                    errorMsg += "<b style='color:Olive;'>" + aData.ItemNumber + ": " + MsgQtyToPullMandatory +"</b><br/>"
                }

                if (!(!isNaN(parseInt(vBinID)) && parseInt(vBinID) > 0)) {
                    $tr.css('background-color', 'Olive');
                    IsGlobalErrorMsgShow = true;
                    errorMsg += "<b style='color:Olive;'>" + aData.ItemNumber + ": " + MsgInventoryLocationMandatory +"</b><br/>"
                }


            }
            else {

                if (!(!isNaN(parseFloat(txtQty)) && parseFloat(txtQty) > 0)) {
                    $tr.css('background-color', 'Olive');
                    IsGlobalErrorMsgShow = true;
                    errorMsg += "<b style='color:Olive;'>" + aData.ItemNumber + ": " + MsgLabourItemRequiredHours +"</b><br/>"
                }
                vBinID = 0;
                vProjectID = '';
                vProjectSpendName = '';
            }

            if (errorMsg.length <= 0) {
                var WOSellPrice = 0;
                if (typeof isCalledFromWorkOrder != "undefined" &&
                    typeof AllowEditItemSellPriceonWorkOrderPull != "undefined" &&
                    isCalledFromWorkOrder.toLowerCase() == "true"
                    && AllowEditItemSellPriceonWorkOrderPull.toLowerCase() == "true") {
                    var txtWOPullSellPrice = $tr.find('#WOPullSellPrice');
                    if (txtWOPullSellPrice != typeof (undefined) && txtWOPullSellPrice.length > 0) {
                        WOSellPrice = txtWOPullSellPrice.val();
                    }
                }
                var vItemID, vItemGUID, vspnOn_HandQuantity;

                if (!_pullCommon.isNewConsumePullObj()) {
                    vItemID = $tr.find('#spnItemID').text();
                    vItemGUID = $tr.find('#spnItemGUID').text();
                    vspnOn_HandQuantity = $tr.find('#spnOn_HandQuantity').text() == "" ? 0 : $tr.find('#spnOn_HandQuantity').text();
                }
                else {
                    var itemData = _pullCommon.getItemDataFromRow($tr);
                    vItemID = itemData.spnItemID;  // $(tr).find('#spnItemID').text();
                    vItemGUID = itemData.spnItemGUID; //$(tr).find('#spnItemGUID').text();
                    vspnOn_HandQuantity = itemData.spnOn_HandQuantity; //$(tr).find('#spnOn_HandQuantity').text() == "" ? 0 : $(tr).find('#spnOn_HandQuantity').text();
                }

                var vPullCreditText = "pull"; //$(obj)[0].value;//$(obj).parent().parent().find('input[name=colors'+vItemID+']:checked')[0].value;
                var VspnDefaultPullQuantity = _pullCommon.getspnDefaultPullQuantity($(this));  //$(this).find('#spnDefaultPullQuantity').text() == "" ? 0 : $(tr).find('#spnDefaultPullQuantity').text();
                var vUDF1 = ''; var vUDF2 = ''; var vUDF3 = ''; var vUDF4 = ''; var vUDF5 = '';
                var vUDF1PullCommon = ''; var vUDF2PullCommon = ''; var vUDF3PullCommon = ''; var vUDF4PullCommon = ''; var vUDF5PullCommon = '';
                var vPullOrderNumber = ""; var vProjectID; var vProjectSpendName = ''; var vPullSupplierAccountNumber = '';

                if ($("#chkUsePullCommonUDF").is(":checked")) {
                    if ($('#txtPullOrderNumberCommon') != null) {
                        if ($('#txtPullOrderNumberCommon').attr("class") == 'selectBox') {
                            vPullOrderNumber = $('#txtPullOrderNumberCommon option:selected').text();
                        }
                        else {
                            vPullOrderNumber = $('#txtPullOrderNumberCommon').val();
                        }
                    }
                }
                else {
                    if ($tr.find('#txtPullOrderNumber') != null) {
                        if ($tr.find('#txtPullOrderNumber').attr("class") == 'selectBox') {
                            vPullOrderNumber = $tr.find('#txtPullOrderNumber option:selected').text();
                        }
                        else {
                            vPullOrderNumber = $tr.find('#txtPullOrderNumber').val();
                        }
                    }
                }                
                if ($(tr).find('#hdnSupplierAccountNumber') != null) {
                    vPullSupplierAccountNumber = $tr.find('#hdnSupplierAccountNumber').val();
                }
                if ($("#chkUsePullCommonUDF").is(":checked")) {
                    if ($('#ProjectIDCommon') != undefined) {
                        vProjectID = $('#ProjectIDCommon').val() == "" ? "" : $('#ProjectIDCommon').val();
                    }
                    else {
                        vProjectID = "";
                    }
                }
                else {
                    //if ($(tr).find('#ProjectID')[0] == undefined) {
                    //    vProjectID = "";
                    //}
                    //else {
                    //    vProjectID = $(tr).find('#ProjectID')[0].value == "" ? "" : $(tr).find('#ProjectID')[0].value;
                    //}

                    vProjectID = _pullCommon.GetProjectId($tr);

                }
                if ($("#chkUsePullCommonUDF").is(":checked")) {
                    if ($('#txtProjectSpentCommon') != undefined)
                        vProjectSpendName = $('#txtProjectSpentCommon').val() == "" ? "" : $('#txtProjectSpentCommon').val();
                    else
                        vProjectSpendName = "";
                }
                else {
                    if ($tr.find('#txtProjectSpent')[0] == undefined) {
                        vProjectSpendName = "";
                    }
                    else {
                        vProjectSpendName = $tr.find('#txtProjectSpent')[0].value == "" ? "" : $tr.find('#txtProjectSpent')[0].value;
                    }
                }
                
                if ($("#chkUsePullCommonUDF").is(":checked")) {

                    vUDF1PullCommon = _pullCommon.GetUdfVal($("#UDF1PullCommon"));
                    vUDF2PullCommon = _pullCommon.GetUdfVal($("#UDF2PullCommon"));
                    vUDF3PullCommon = _pullCommon.GetUdfVal($("#UDF3PullCommon"));
                    vUDF4PullCommon = _pullCommon.GetUdfVal($("#UDF4PullCommon"));
                    vUDF5PullCommon = _pullCommon.GetUdfVal($("#UDF5PullCommon"));
                                        


                //    if ($("#UDF1PullCommon") != null) {
                //        if ($("#UDF1PullCommon").attr("class") == 'selectBox') {
                //            vUDF1PullCommon = $("#UDF1PullCommon option:selected").text();
                //        }
                //        else {
                //            vUDF1PullCommon = $("#UDF1PullCommon").val();
                //        }
                //    }

                //    if ($("#UDF2PullCommon") != null) {
                //        if ($("#UDF2PullCommon").attr("class") == 'selectBox') {
                //            vUDF2PullCommon = $("#UDF2PullCommon option:selected").text();
                //        }
                //        else {
                //            vUDF2PullCommon = $("#UDF2PullCommon").val();
                //        }
                //    }

                //    if ($("#UDF3PullCommon") != null) {
                //        if ($("#UDF3PullCommon").attr("class") == 'selectBox') {
                //            vUDF3PullCommon = $("#UDF3PullCommon option:selected").text();
                //        }
                //        else {
                //            vUDF3PullCommon = $("#UDF3PullCommon").val();
                //        }
                //    }

                //    if ($("#UDF4PullCommon") != null) {
                //        if ($("#UDF4PullCommon").attr("class") == 'selectBox') {
                //            vUDF4PullCommon = $("#UDF4PullCommon option:selected").text();
                //        }
                //        else {
                //            vUDF4PullCommon = $("#UDF4PullCommon").val();
                //        }
                //    }

                //    if ($("#UDF5PullCommon") != null) {
                //        if ($("#UDF5PullCommon").attr("class") == 'selectBox') {
                //            vUDF5PullCommon = $("#UDF5PullCommon option:selected").text();
                //        }
                //        else {
                //            vUDF5PullCommon = $("#UDF5PullCommon").val();
                //        }
                //    }
                }

                vUDF1 = _pullCommon.GetUdfVal($tr.find('#UDF1'));
                vUDF2 = _pullCommon.GetUdfVal($tr.find('#UDF2'));
                vUDF3 = _pullCommon.GetUdfVal($tr.find('#UDF3'));
                vUDF4 = _pullCommon.GetUdfVal($tr.find('#UDF4'));
                vUDF5 = _pullCommon.GetUdfVal($tr.find('#UDF5'));

                //if ($(tr).find('#UDF1') != null) {
                //    if ($(tr).find('#UDF1').attr("class") == 'selectBox') {
                //        vUDF1 = $(tr).find('#UDF1 option:selected').text();
                //    }
                //    else {
                //        vUDF1 = $(tr).find('#UDF1').val();
                //    }
                //}

                //if ($(tr).find('#UDF2') != null) {
                //    if ($(tr).find('#UDF2').attr("class") == 'selectBox') {
                //        vUDF2 = $(tr).find('#UDF2 option:selected').text();
                //    }
                //    else {
                //        vUDF2 = $(tr).find('#UDF2').val();
                //    }
                //}

                //if ($(tr).find('#UDF3') != null) {
                //    if ($(tr).find('#UDF3').attr("class") == 'selectBox') {
                //        vUDF3 = $(tr).find('#UDF3 option:selected').text();
                //    }
                //    else {
                //        vUDF3 = $(tr).find('#UDF3').val();
                //    }
                //}

                //if ($(tr).find('#UDF4') != null) {
                //    if ($(tr).find('#UDF4').attr("class") == 'selectBox') {
                //        vUDF4 = $(tr).find('#UDF4 option:selected').text();
                //    }
                //    else {
                //        vUDF4 = $(tr).find('#UDF4').val();
                //    }
                //}

                //if ($(tr).find('#UDF5') != null) {
                //    if ($(tr).find('#UDF5').attr("class") == 'selectBox') {
                //        vUDF5 = $(tr).find('#UDF5 option:selected').text();
                //    }
                //    else {
                //        vUDF5 = $(tr).find('#UDF5').val();
                //    }
                //}

                if ($("#chkUsePullCommonUDF").is(":checked")) {
                    vUDF1 = vUDF1PullCommon;
                    vUDF2 = vUDF2PullCommon;
                    vUDF3 = vUDF3PullCommon;
                    vUDF4 = vUDF4PullCommon;
                    vUDF5 = vUDF5PullCommon;
                }                
                PullInfo.push({ ID: index, ItemID: vItemID, PoolQuantity: txtQty, ItemGUID: vItemGUID, BinID: vBinID, DefaultPullQuantity: VspnDefaultPullQuantity, ProjectSpendGUID: vProjectID, ProjectSpendName: vProjectSpendName, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5, WorkOrderDetailGUID: vWorkOrderDetailGUID, PullOrderNumber: vPullOrderNumber, SupplierAccountGuid: vPullSupplierAccountNumber, ItemSellPrice: WOSellPrice });
            }
        }); // loop
    }
    else if ($(btnObj).prop("id") == "btnAdd") {

        var tr = $(btnObj).parent().parent()[0]
            , $trAdd = $(tr);
        var aPos = $('#ItemModeDataTable').dataTable().fnGetPosition($trAdd[0]),
            aData = $('#ItemModeDataTable').dataTable().fnGetData(aPos);

        var WOSellPrice = 0;
        if (typeof isCalledFromWorkOrder != "undefined" &&
            typeof AllowEditItemSellPriceonWorkOrderPull != "undefined" &&
            isCalledFromWorkOrder.toLowerCase() == "true"
            && AllowEditItemSellPriceonWorkOrderPull.toLowerCase() == "true") {
            var txtWOPullSellPrice = $trAdd.find('#WOPullSellPrice');
            if (txtWOPullSellPrice != typeof (undefined) && txtWOPullSellPrice.length > 0) {
                WOSellPrice = txtWOPullSellPrice.val();
            }
        }

        var txxt = $trAdd.find('#txtQty');
        var vBinID;
        var vProjectID;
        var vProjectSpendName = '';
        var itemType = _pullCommon.getspnOrderItemType($trAdd);//$(tr).find('#spnOrderItemType').text();

        var txtQty = txxt.val();
        if (itemType != '4') {

            vBinID = _pullCommon.getBinId($trAdd);//$(tr).find('#BinID')[0].value == '' ? 0 : $(tr).find('#BinID')[0].value;

            if ($("#chkUsePullCommonUDF").is(":checked")) {
                if ($('#ProjectIDCommon') != undefined)
                    vProjectID = $('#ProjectIDCommon').val() == "" ? "" : $('#ProjectIDCommon').val();
                else
                    vProjectID = "";
            }
            else {
                //if ($(tr).find('#ProjectID')[0] != undefined) {
                //    vProjectID = $(tr).find('#ProjectID')[0].value == "" ? "" : $(tr).find('#ProjectID')[0].value;
                //}
                //else {
                //    vProjectID = "";
                //}
                vProjectID = _pullCommon.GetProjectId($trAdd);
            }

            if ($("#chkUsePullCommonUDF").is(":checked")) {
                if ($('#txtProjectSpentCommon') != undefined)
                    vProjectSpendName = $('#txtProjectSpentCommon').val() == "" ? "" : $('#txtProjectSpentCommon').val();
                else
                    vProjectSpendName = "";
            }
            else {
                if ($trAdd.find('#txtProjectSpent')[0] == undefined) {
                    vProjectSpendName = "";
                }
                else {
                    vProjectSpendName = $trAdd.find('#txtProjectSpent')[0].value == "" ? "" : $trAdd.find('#txtProjectSpent')[0].value;
                }
            }

            if (!(!isNaN(parseFloat(txtQty)) && parseFloat(txtQty) > 0)) {
                $trAdd.css('background-color', 'Olive');
                IsGlobalErrorMsgShow = true;
                errorMsg += "<b style='color:Olive;'>" + aData.ItemNumber + ": " + MsgQtyToPullMandatory +"</b><br/>"
            }

            if (!(!isNaN(parseInt(vBinID)) && parseInt(vBinID) > 0)) {
                $trAdd.css('background-color', 'Olive');
                IsGlobalErrorMsgShow = true;
                errorMsg += "<b style='color:Olive;'>" + aData.ItemNumber + ": " + MsgInventoryLocationMandatory +"</b><br/>"
            }
        }
        else {

            if (!(!isNaN(parseFloat(txtQty)) && parseFloat(txtQty) > 0)) {
                $trAdd.css('background-color', 'Olive');
                IsGlobalErrorMsgShow = true;
                errorMsg += "<b style='color:Olive;'>" + aData.ItemNumber + ": " + MsgLabourItemRequiredHours +"</b><br/>"
            }
            vBinID = 0;
            vProjectID = '';
            vProjectSpendName = '';
        }

        if (errorMsg.length <= 0) {
            var vItemID, vItemGUID, vspnOn_HandQuantity;
            if (!_pullCommon.isNewConsumePullObj()) {
                vItemID = $trAdd.find('#spnItemID').text();
                vItemGUID = $trAdd.find('#spnItemGUID').text();
                vspnOn_HandQuantity = $trAdd.find('#spnOn_HandQuantity').text() == "" ? 0 : $trAdd.find('#spnOn_HandQuantity').text();
            }
            else {
                var itemData = _pullCommon.getItemDataFromRow($trAdd);
                vItemID = itemData.spnItemID; //$(tr).find('#spnItemID').text();
                vItemGUID = itemData.spnItemGUID; //$(tr).find('#spnItemGUID').text();
                vspnOn_HandQuantity = itemData.spnOn_HandQuantity; //$(tr).find('#spnOn_HandQuantity').text() == "" ? 0 : $(tr).find('#spnOn_HandQuantity').text();
            }

            var vPullCreditText = "pull"; //$(obj)[0].value;//$(obj).parent().parent().find('input[name=colors'+vItemID+']:checked')[0].value;
            var VspnDefaultPullQuantity = $trAdd.find('#spnDefaultPullQuantity').text() == "" ? 0 : $trAdd.find('#spnDefaultPullQuantity').text();
            var vUDF1 = ''; var vUDF2 = ''; var vUDF3 = ''; var vUDF4 = ''; var vUDF5 = '';
            var vUDF1PullCommon = ''; var vUDF2PullCommon = ''; var vUDF3PullCommon = ''; var vUDF4PullCommon = ''; var vUDF5PullCommon = '';
            var vPullOrderNumber = ""; var vProjectID; var vProjectSpendName = ''; var vPullSupplierAccountNumber = '';            

            if ($("#chkUsePullCommonUDF").is(":checked")) {
                if ($('#txtPullOrderNumberCommon') != null) {
                    if ($('#txtPullOrderNumberCommon').attr("class") == 'selectBox') {
                        vPullOrderNumber = $('#txtPullOrderNumberCommon option:selected').text();
                    }
                    else {
                        vPullOrderNumber = $('#txtPullOrderNumberCommon').val();
                    }
                }
            }
            else {
                if ($trAdd.find('#txtPullOrderNumber') != null) {
                    if ($(tr).find('#txtPullOrderNumber').attr("class") == 'selectBox') {
                        vPullOrderNumber = $trAdd.find('#txtPullOrderNumber option:selected').text();
                    }
                    else {
                        vPullOrderNumber = $trAdd.find('#txtPullOrderNumber').val();
                    }
                }
            }            
            if ($(tr).find('#hdnSupplierAccountNumber') != null) {
                vPullSupplierAccountNumber = $trAdd.find('#hdnSupplierAccountNumber').val();
            }
            if ($("#chkUsePullCommonUDF").is(":checked")) {
                if ($('#ProjectIDCommon') != null) {
                    if ($('#ProjectIDCommon').attr("class") == 'selectBox') {
                        vProjectID = $('#ProjectIDCommon option:selected').text();
                    }
                    else {
                        vProjectID = $('#ProjectIDCommon').val();
                    }
                }
            }
            else {
                if ($trAdd.find('#ProjectIDCommon') != null) {
                    if ($(tr).find('#ProjectIDCommon').attr("class") == 'selectBox') {
                        vProjectID = $trAdd.find('#ProjectIDCommon option:selected').text();
                    }
                    else {
                        vProjectID = $trAdd.find('#ProjectIDCommon').val();
                    }
                }
            }
            
            if ($("#chkUsePullCommonUDF").is(":checked")) {
                if ($('#txtProjectSpentCommon') != null) {
                    if ($('#txtProjectSpentCommon').attr("class") == 'selectBox') {
                        vProjectSpendName = $('#txtProjectSpentCommon option:selected').text();
                    }
                    else {
                        vProjectSpendName = $('#txtProjectSpentCommon').val();
                    }
                }
            }
            else {
                if ($trAdd.find('#txtProjectSpent') != null) {
                    if ($trAdd.find('#txtProjectSpent').attr("class") == 'selectBox') {
                        vProjectSpendName = $trAdd.find('#txtProjectSpent option:selected').text();
                    }
                    else {
                        vProjectSpendName = $trAdd.find('#txtProjectSpent').val();
                    }
                }
            }
            
            if ($("#chkUsePullCommonUDF").is(":checked")) {
                if ($("#UDF1PullCommon") != null) {
                    if ($("#UDF1PullCommon").attr("class") == 'selectBox') {
                        vUDF1PullCommon = $("#UDF1PullCommon option:selected").text();
                    }
                    else {
                        vUDF1PullCommon = $("#UDF1PullCommon").val();
                    }
                }

                if ($("#UDF2PullCommon") != null) {
                    if ($("#UDF2PullCommon").attr("class") == 'selectBox') {
                        vUDF2PullCommon = $("#UDF2PullCommon option:selected").text();
                    }
                    else {
                        vUDF2PullCommon = $("#UDF2PullCommon").val();
                    }
                }

                if ($("#UDF3PullCommon") != null) {
                    if ($("#UDF3PullCommon").attr("class") == 'selectBox') {
                        vUDF3PullCommon = $("#UDF3PullCommon option:selected").text();
                    }
                    else {
                        vUDF3PullCommon = $("#UDF3PullCommon").val();
                    }
                }

                if ($("#UDF4PullCommon") != null) {
                    if ($("#UDF4PullCommon").attr("class") == 'selectBox') {
                        vUDF4PullCommon = $("#UDF4PullCommon option:selected").text();
                    }
                    else {
                        vUDF4PullCommon = $("#UDF4PullCommon").val();
                    }
                }

                if ($("#UDF5PullCommon") != null) {
                    if ($("#UDF5PullCommon").attr("class") == 'selectBox') {
                        vUDF5PullCommon = $("#UDF5PullCommon option:selected").text();
                    }
                    else {
                        vUDF5PullCommon = $("#UDF5PullCommon").val();
                    }
                }
            }

            if ($trAdd.find('#UDF1') != null) {
                if ($trAdd.find('#UDF1').attr("class") == 'selectBox' || $trAdd.find('#UDF1').attr("class") == 'selectBox loadondemandudf') {
                    vUDF1 = $trAdd.find('#UDF1 option:selected').text();
                }
                else {
                    vUDF1 = $trAdd.find('#UDF1').val();
                }
            }

            if ($trAdd.find('#UDF2') != null) {
                if ($trAdd.find('#UDF2').attr("class") == 'selectBox' || $trAdd.find('#UDF2').attr("class") == 'selectBox loadondemandudf') {
                    vUDF2 = $trAdd.find('#UDF2 option:selected').text();
                }
                else {
                    vUDF2 = $trAdd.find('#UDF2').val();
                }
            }

            if ($trAdd.find('#UDF3') != null) {
                if ($trAdd.find('#UDF3').attr("class") == 'selectBox' || $trAdd.find('#UDF3').attr("class") == 'selectBox loadondemandudf') {
                    vUDF3 = $trAdd.find('#UDF3 option:selected').text();
                }
                else {
                    vUDF3 = $trAdd.find('#UDF3').val();
                }
            }

            if ($trAdd.find('#UDF4') != null) {
                if ($trAdd.find('#UDF4').attr("class") == 'selectBox' || $trAdd.find('#UDF4').attr("class") == 'selectBox loadondemandudf') {
                    vUDF4 = $trAdd.find('#UDF4 option:selected').text();
                }
                else {
                    vUDF4 = $trAdd.find('#UDF4').val();
                }
            }

            if ($trAdd.find('#UDF5') != null) {
                if ($trAdd.find('#UDF5').attr("class") == 'selectBox' || $trAdd.find('#UDF5').attr("class") == 'selectBox loadondemandudf') {
                    vUDF5 = $trAdd.find('#UDF5 option:selected').text();
                }
                else {
                    vUDF5 = $trAdd.find('#UDF5').val();
                }
            }

            if ($("#chkUsePullCommonUDF").is(":checked")) {
                vUDF1 = vUDF1PullCommon;
                vUDF2 = vUDF2PullCommon;
                vUDF3 = vUDF3PullCommon;
                vUDF4 = vUDF4PullCommon;
                vUDF5 = vUDF5PullCommon;
            }                       
            PullInfo.push({ ID: 0, ItemID: vItemID, PoolQuantity: txtQty, ItemGUID: vItemGUID, BinID: vBinID, DefaultPullQuantity: VspnDefaultPullQuantity, ProjectSpendGUID: vProjectID, ProjectSpendName: vProjectSpendName, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5, WorkOrderDetailGUID: vWorkOrderDetailGUID, PullOrderNumber: vPullOrderNumber, SupplierAccountGuid: vPullSupplierAccountNumber, ItemSellPrice: WOSellPrice });
        }
    }// btnAdd
    if (errorMsg.length <= 0) {
        $.ajax({
            type: "POST",
            url: PullItemQuantityUrl,
            contentType: 'application/json',
            dataType: 'html',
            data: JSON.stringify(PullInfo),
            success: function (RetData) {
                $("#DivPullSelection").html("");
                $("#DivPullSelection").html(RetData);
                $("#DivPullSelection").dialog('open');
                $('#DivLoading').hide();
                $(btnObj).removeAttr("disabled");
            },
            error: function (err) {
                //alert(err);
                console.log(err);
                $(btnObj).removeAttr("disabled");
            }
        });
    }
    else {
        $('#DivLoading').hide();
        $('#dlgCommonErrorMsg').find("#pOkbtn").css('display', '');
        $('#dlgCommonErrorMsg').find("#pYesNobtn").css('display', 'none');
        $('#dlgCommonErrorMsg').find("#pErrMessage").html(errorMsg);
        $('#dlgCommonErrorMsg').modal({
            escClose: false,
            close: false
        });
        $(btnObj).removeAttr("disabled");
    }
    return false;
}

function OpenPullPopupFromRequisition(btnObj, WorkOrderId,materialStagingGUID) {
    PullPageName = "REQUISITION";
    var vWorkOrderDetailGUID = '00000000-0000-0000-0000-000000000000';
    if (WorkOrderId != null && WorkOrderId != undefined && WorkOrderId.trim() != '') {
        vWorkOrderDetailGUID = WorkOrderId;
    }

    var errorMsg = '';
    $('#DivLoading').show();
    var PullInfo = new Array();
    if ($(btnObj).prop("id") == "btnPullAll") {

        if ($('[id^="RequisitionItemsTable"]').find("tbody").find("tr.row_selected").length <= 0) {
            $('#DivLoading').hide();
            $('#dlgCommonErrorMsg').find("#pOkbtn").css('display', '');
            $('#dlgCommonErrorMsg').find("#pYesNobtn").css('display', 'none');
            $('#dlgCommonErrorMsg').find("#pErrMessage").html(PleaseSelectRecord);
            $('#dlgCommonErrorMsg').modal({
                escClose: false,
                close: false
            });
            return false;
        }


        //var iCount = 0;
        //$('[id^="RequisitionItemsTable"]').find("tbody").find("tr.row_selected").each(function (index, tr1) {
        //    if ($(tr1).find('[type="button"]').length <= 0) {
        //        iCount++;
        //    }
        //});

        ////if (iCount > 0) {
        ////    $('#DivLoading').hide();
        ////    $('#dlgCommonErrorMsg').find("#pOkbtn").css('display', '');
        ////    $('#dlgCommonErrorMsg').find("#pYesNobtn").css('display', 'none');
        ////    $('#dlgCommonErrorMsg').find("#pErrMessage").html('You can not pull for closed items.');
        ////    $('#dlgCommonErrorMsg').modal({
        ////        escClose: false,
        ////        close: false
        ////    });
        ////    return false;
        ////}

        var HasSerialLotItem = false;
        $('[id^="RequisitionItemsTable"]').find("tbody").find("tr.row_selected").each(function (index, tr) {
            var SerialNumberTracking = $(tr).find('#hdnSerialNumberTracking').val();
            var LotNumberTracking = $(tr).find('#hdnLotNumberTracking').val();
            var DateCodeTracking = $(tr).find('#hdnDateCodeTracking').val();
            if (SerialNumberTracking == 'True' || SerialNumberTracking == 'true' || LotNumberTracking == 'True' || LotNumberTracking == 'true'
                || DateCodeTracking == 'True' || DateCodeTracking == 'true'             ) {
                HasSerialLotItem = true;
            }
        });

        if (HasSerialLotItem == false) {
            $('#GlobalModalProcessing').modal();
            setTimeout('AddSingleItemToPullList($("#btnPullAll"))', 1000);
        }
        else {
            $('[id^="RequisitionItemsTable"]').find("tbody").find("tr.row_selected").each(function (index, tr) {
                var $tr = $(tr);
                var _hdnIsCloseItem = $tr.find('#hdnIsCloseItem').val();
                if (_hdnIsCloseItem != "True") {
                    var ItemNumber = $tr.find('#hdnItemNumber').val();
                    var vToolName = $tr.find('#hdnToolName').val();
                    errorMsg = '';
                    var _RequisitionDetailGUID = $tr.find('#hdnRequisitionDetailGUID').val(),
                        txxt = $tr.find('#txtQty'),
                        vBinID,
                        vProjectID;
                        vProjectName = '';
                    var itemType = _pullCommon.getspnOrderItemType($tr);//$(tr).find('#spnOrderItemType').text();
                    var txtQty = txxt.val();
                    if (itemType != '4') {
                        if ($tr.find('#item_BinID').length > 0 && $tr.find('#item_BinID')[0].length > 0) {
                            vBinID = $tr.find('#item_BinID')[0].value == '' ? 0 : $tr.find('#item_BinID')[0].value;
                        }
                        if (typeof (vBinID) == "undefined"
                            || vBinID == "") {
                            if ($tr.find('#item_BinID').length > 0) {
                                vBinID = $tr.find('#item_BinID').val() == '' ? 0 : $tr.find('#item_BinID').val();
                            }
                        }
                        if ($("#chkUsePullCommonUDF").is(":checked")) {
                            if ($('#txtProjectSpentCommon') != undefined && $('#txtProjectSpentCommon').length > 0) {
                                vProjectName = $('#txtProjectSpentCommon').val() == "" ? "" : $('#txtProjectSpentCommon').val();
                            }
                            else if ($('#ProjectSpendGUIDCommon') != undefined) {
                                vProjectID = $('#ProjectSpendGUIDCommon').val() == "" ? "" : $('#ProjectSpendGUIDCommon').val();
                            }
                            else {
                                vProjectID = "";
                            }
                        }
                        else {
                            if ($tr.find('#txtProjectSpent') != undefined && $tr.find('#txtProjectSpent').length > 0) {
                                vProjectName = $tr.find('#txtProjectSpent').val();
                            }
                            else if ($tr.find('#ProjectSpendGUID')[0] == undefined) {
                                vProjectID = "";
                            }
                            else {
                                vProjectID = $tr.find('#ProjectSpendGUID')[0].value == "" ? "" : $tr.find('#ProjectSpendGUID')[0].value;
                            }
                        }

                        if (!(!isNaN(parseFloat(txtQty)) && parseFloat(txtQty) > 0)) {
                            $tr.css('background-color', 'Olive');
                            IsGlobalErrorMsgShow = true;
                            errorMsg += "<b style='color:Olive;'>" + ItemNumber + ": " + MsgQtyToPullMandatory +"</b><br/>"
                        }

                        if ($tr.find('#item_BinID').length > 0 && $.trim(ItemNumber).length > 0) {
                            if (!(!isNaN(parseInt(vBinID)) && parseInt(vBinID) > 0)) {
                                $tr.css('background-color', 'Olive');
                                IsGlobalErrorMsgShow = true;
                                errorMsg += "<b style='color:Olive;'>" + ItemNumber + ": " + MsgInventoryLocationMandatory+"</b><br/>"
                            }
                        }
                        else {
                            var vTech; var vTechGUID = '';
                            if ($("#chkUseToolCommonUDF").is(":checked") && $("#txtUseThisTechnician") != null) {
                                vTech = $("#txtUseThisTechnician").val();
                                vTechGUID = $("#UseThisTechnicianGUID").val();
                            }
                            else {
                                vTech = $tr.find("#txtTechnician").val();
                                vTechGUID = $tr.find("#TechnicianGUID").val();
                            }

                            if ($.trim(vTechGUID).length <= 0 || vTechGUID == '00000000-0000-0000-0000-000000000000') {
                                $tr.css('background-color', 'Olive');
                                IsGlobalErrorMsgShow = true;
                                errorMsg += "<b style='color:Olive;'>" + ReqTechnician+"</b><br/>"
                            }

                        }
                    }
                    else {

                        if (!(!isNaN(parseFloat(txtQty)) && parseFloat(txtQty) > 0)) {
                            $(tr).css('background-color', 'Olive');
                            IsGlobalErrorMsgShow = true;
                            errorMsg += "<b style='color:Olive;'>" + ItemNumber + ": " + MsgLabourItemRequiredHours +"</b><br/>"
                        }
                        vBinID = 0;
                        vProjectID = '';
                    }

                    if (errorMsg.length <= 0) {
                        var vItemID = $tr.find('#hdnItemID').val();

                        var vItemGUID = $tr.find('#hdnItemGUID').val();
                        var vToolGUID = $tr.find('#hdnToolGUID').val();

                        var vspnOn_HandQuantity = $tr.find('#hdnHandQuantity').val() == "" ? 0 : $tr.find('#hdnHandQuantity').val();
                        var vPullCreditText = "pull"; //$(obj)[0].value;//$(obj).parent().parent().find('input[name=colors'+vItemID+']:checked')[0].value;

                        var VspnDefaultPullQuantity = $tr.find('#hdnDefaultPullQuantity').val() == "" ? 0 : $tr.find('#hdnDefaultPullQuantity').val();
                        var vUDF1 = ''; var vUDF2 = ''; var vUDF3 = ''; var vUDF4 = ''; var vUDF5 = '';
                        //var vUDF1PullCommon = ''; var vUDF2PullCommon = ''; var vUDF3PullCommon = ''; var vUDF4PullCommon = ''; var vUDF5PullCommon = '';

                        var vTCUDF1 = ''; var vTCUDF2 = ''; var vTCUDF3 = ''; var vTCUDF4 = ''; var vTCUDF5 = '';
                        var vTechnicianName = ''; var vTechnicianGUID = '';                        
                        var SupplierAccountGuid = '';
                        if ($tr.find('#hdnSupplierAccountNumber') != undefined) {
                            SupplierAccountGuid = $(tr).find("#hdnSupplierAccountNumber").val();
                        }

                        var vPullOrderNumber = "";

                        if ($("#chkUsePullCommonUDF").is(":checked")) {
                            vUDF1 = JSGetCommonUDFValue("UDF1PullCommon");
                            vUDF2 = JSGetCommonUDFValue("UDF2PullCommon");
                            vUDF3 = JSGetCommonUDFValue("UDF3PullCommon");
                            vUDF4 = JSGetCommonUDFValue("UDF4PullCommon");
                            vUDF5 = JSGetCommonUDFValue("UDF5PullCommon");

                            if ($('#txtPullOrderNumberCommon') != null) {
                                if ($('#txtPullOrderNumberCommon').attr("class") == 'selectBox') {
                                    vPullOrderNumber = $('#txtPullOrderNumberCommon option:selected').text();
                                }
                                else {
                                    vPullOrderNumber = $('#txtPullOrderNumberCommon').val();
                                }
                            }
                        }
                        else {
                            vUDF1 = JSGetCommonUDFValueFromTR("UDF1Pull", tr);
                            vUDF2 = JSGetCommonUDFValueFromTR("UDF2Pull", tr);
                            vUDF3 = JSGetCommonUDFValueFromTR("UDF3Pull", tr);
                            vUDF4 = JSGetCommonUDFValueFromTR("UDF4Pull", tr);
                            vUDF5 = JSGetCommonUDFValueFromTR("UDF5Pull", tr);

                            if ($tr.find('#txtPullOrderNumber') != null) {
                                if ($tr.find('#txtPullOrderNumber').attr("class") == 'selectBox') {
                                    vPullOrderNumber = $tr.find('#txtPullOrderNumber option:selected').text();
                                }
                                else {
                                    vPullOrderNumber = $tr.find('#txtPullOrderNumber').val();
                                }
                            }

                        }

                        if ($("#chkUseToolCommonUDF").is(":checked")) {
                            vTCUDF1 = JSGetCommonUDFValue("UDF1ToolCommon");
                            vTCUDF2 = JSGetCommonUDFValue("UDF2ToolCommon");
                            vTCUDF3 = JSGetCommonUDFValue("UDF3ToolCommon");
                            vTCUDF4 = JSGetCommonUDFValue("UDF4ToolCommon");
                            vTCUDF5 = JSGetCommonUDFValue("UDF5ToolCommon");

                            if ($("#txtUseThisTechnician") != null) {
                                vTechnicianName = $("#txtUseThisTechnician").val();
                                vTechnicianGUID = $("#UseThisTechnicianGUID").val();
                            }
                        }
                        else {

                            vTCUDF1 = JSGetCommonUDFValueFromTR("UDF1TC", tr);
                            vTCUDF2 = JSGetCommonUDFValueFromTR("UDF2TC", tr);
                            vTCUDF3 = JSGetCommonUDFValueFromTR("UDF3TC", tr);
                            vTCUDF4 = JSGetCommonUDFValueFromTR("UDF4TC", tr);
                            vTCUDF5 = JSGetCommonUDFValueFromTR("UDF5TC", tr);

                            vTechnicianName = $tr.find("#txtTechnician").val();
                            vTechnicianGUID = $tr.find("#TechnicianGUID").val();
                        }


                        PullInfo.push({
                            ID: index, ItemID: vItemID, PoolQuantity: txtQty, ItemGUID: vItemGUID, BinID: vBinID, DefaultPullQuantity: VspnDefaultPullQuantity,
                            ProjectSpendGUID: vProjectID, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5, WorkOrderDetailGUID: WorkOrderId,
                            PullOrderNumber: vPullOrderNumber, RequisitionDetailGUID: _RequisitionDetailGUID, ToolGUID: vToolGUID, ToolCheckoutUDF1: vTCUDF1,
                            ToolCheckoutUDF2: vTCUDF2, ToolCheckoutUDF3: vTCUDF3, ToolCheckoutUDF4: vTCUDF4, ToolCheckoutUDF5: vTCUDF5,
                            TechnicianGUID: vTechnicianGUID, Technician: vTechnicianName, ToolName: vToolName, MaterialStagingGUID: materialStagingGUID, SupplierAccountGuid: SupplierAccountGuid,
                            ProjectSpendName: vProjectName
                        });
                    }
                }
                else{                    
                    var ItemNumber = $tr.find('#hdnItemNumber').val();
                    $tr.css('background-color', 'Olive');
                    IsGlobalErrorMsgShow = true;
                    errorMsg += "<b style='color:Olive;'>" + ItemNumber + ": " + MsgPullClosedValidation +"</b><br/>"
                }
            }); // loop
        }
    }
    else if ($(btnObj).prop("id") == "btnAdd") {

        var tr = $(btnObj).parent().parent()[0];
        var $tr = $(tr);
        var ItemNumber = $tr.find('#hdnItemNumber').val();

        var _RequisitionDetailGUID = $tr.find('#hdnRequisitionDetailGUID').val(),
            txxt = $tr.find('#txtQty'),
            vBinID,
            vProjectID,
            itemType = _pullCommon.getspnOrderItemType($tr),//$(tr).find('#spnOrderItemType').text();
            txtQty = txxt.val();
            vProjectName = '';
        if (itemType != '4') {

            vBinID = $tr.find('#item_BinID')[0].value == '' ? 0 : $tr.find('#item_BinID')[0].value;
            if ($("#chkUsePullCommonUDF").is(":checked")) {

                if ($('#txtProjectSpentCommon') != undefined && $('#txtProjectSpentCommon').length > 0) {
                    vProjectName = $('#txtProjectSpentCommon').val() == "" ? "" : $('#txtProjectSpentCommon').val();
                }else if ($('#ProjectSpendGUIDCommon') != undefined) {
                    vProjectID = $('#ProjectSpendGUIDCommon').val() == "" ? "" : $('#ProjectSpendGUIDCommon').val();
                }
                else {
                    vProjectID = "";
                }
            }
            else {
                if ($tr.find('#txtProjectSpent') != undefined && $tr.find('#txtProjectSpent').length > 0) {
                    vProjectName = $tr.find('#txtProjectSpent').val();
                }else if ($tr.find('#ProjectSpendGUID')[0] != undefined) {
                    vProjectID = $tr.find('#ProjectSpendGUID')[0].value == "" ? "" : $tr.find('#ProjectSpendGUID')[0].value;
                }
                else {
                    vProjectID = "";
                }
            }

            if (!(!isNaN(parseFloat(txtQty)) && parseFloat(txtQty) > 0)) {
                $tr.css('background-color', 'Olive');
                IsGlobalErrorMsgShow = true;
                errorMsg += "<b style='color:Olive;'>" + ItemNumber + ": " + MsgQtyToPullMandatory +"</b><br/>"
            }

            if (!(!isNaN(parseInt(vBinID)) && parseInt(vBinID) > 0)) {
                $tr.css('background-color', 'Olive');
                IsGlobalErrorMsgShow = true;
                errorMsg += "<b style='color:Olive;'>" + ItemNumber + ": " + MsgInventoryLocationMandatory +"</b><br/>"
            }
        }
        else {

            if (!(!isNaN(parseFloat(txtQty)) && parseFloat(txtQty) > 0)) {
                $tr.css('background-color', 'Olive');
                IsGlobalErrorMsgShow = true;
                errorMsg += "<b style='color:Olive;'>" + ItemNumber + ": " + MsgInventoryLocationMandatory +"</b><br/>"
            }
            vBinID = 0;
            vProjectID = '';
        }

        if (errorMsg.length <= 0) {
            var vItemID = $tr.find('#hdnItemID').val(),
                vItemGUID = $tr.find('#hdnItemGUID').val(),
                vspnOn_HandQuantity = $tr.find('#hdnHandQuantity').val() == "" ? 0 : $tr.find('#hdnHandQuantity').val(),
                vPullCreditText = "pull"; //$(obj)[0].value;//$(obj).parent().parent().find('input[name=colors'+vItemID+']:checked')[0].value;
            var VspnDefaultPullQuantity = $(this).find('#hdnDefaultPullQuantity').val() == "" ? 0 : $tr.find('#hdnDefaultPullQuantity').val();
            var vUDF1 = ''; var vUDF2 = ''; var vUDF3 = ''; var vUDF4 = ''; var vUDF5 = '';
            //var vUDF1PullCommon = ''; var vUDF2PullCommon = ''; var vUDF3PullCommon = ''; var vUDF4PullCommon = ''; var vUDF5PullCommon = '';
            var vPullOrderNumber = "";
            
            var SupplierAccountGuid = '';
            if ($tr.find('#hdnSupplierAccountNumber') != undefined) {
                SupplierAccountGuid = $tr.find("#hdnSupplierAccountNumber").val();
            }


            if ($("#chkUsePullCommonUDF").is(":checked")) {
                vUDF1 = JSGetCommonUDFValue("UDF1PullCommon");
                vUDF2 = JSGetCommonUDFValue("UDF2PullCommon");
                vUDF3 = JSGetCommonUDFValue("UDF3PullCommon");
                vUDF4 = JSGetCommonUDFValue("UDF4PullCommon");
                vUDF5 = JSGetCommonUDFValue("UDF5PullCommon");

                if ($('#txtPullOrderNumberCommon') != null) {
                    if ($('#txtPullOrderNumberCommon').attr("class") == 'selectBox') {
                        vPullOrderNumber = $('#txtPullOrderNumberCommon option:selected').text();
                    }
                    else {
                        vPullOrderNumber = $('#txtPullOrderNumberCommon').val();
                    }
                }
            }
            else {
                vUDF1 = JSGetCommonUDFValueFromTR('UDF1Pull', tr);
                vUDF2 = JSGetCommonUDFValueFromTR('UDF2Pull', tr);
                vUDF3 = JSGetCommonUDFValueFromTR('UDF3Pull', tr);
                vUDF4 = JSGetCommonUDFValueFromTR('UDF4Pull', tr);
                vUDF5 = JSGetCommonUDFValueFromTR('UDF5Pull', tr);

                if ($tr.find('#txtPullOrderNumber') != null) {
                    if ($tr.find('#txtPullOrderNumber').attr("class") == 'selectBox') {
                        vPullOrderNumber = $tr.find('#txtPullOrderNumber option:selected').text();
                    }
                    else {
                        vPullOrderNumber = $tr.find('#txtPullOrderNumber').val();
                    }
                }
            }

            PullInfo.push({
                ID: 0, ItemID: vItemID, PoolQuantity: txtQty, ItemGUID: vItemGUID, BinID: vBinID, DefaultPullQuantity: VspnDefaultPullQuantity,
                ProjectSpendGUID: vProjectID, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5, WorkOrderDetailGUID: WorkOrderId,
                PullOrderNumber: vPullOrderNumber, RequisitionDetailGUID: _RequisitionDetailGUID, MaterialStagingGUID: materialStagingGUID, SupplierAccountGuid: SupplierAccountGuid,
                ProjectSpendName: vProjectName
            });
        }
    }
  
    if (errorMsg.length <= 0) {

        if (PullInfo.length > 0) {
            $.ajax({
                type: "POST",
                url: PullItemQuantityUrl,
                contentType: 'application/json',
                dataType: 'html',
                data: JSON.stringify(PullInfo),
                success: function (RetData) {
                    $("#DivPullSelection").html("");
                    $("#DivPullSelection").html(RetData);
                    $("#DivPullSelection").dialog('open');
                    $('#DivLoading').hide();
                },
                error: function (err) {
                    //alert(err);
                    console.log(err);
                }
            });
        }
        else {
            $('#DivLoading').hide();
            $('#dlgCommonErrorMsg').find("#pOkbtn").css('display', '');
            $('#dlgCommonErrorMsg').find("#pYesNobtn").css('display', 'none');
            $('#dlgCommonErrorMsg').find("#pErrMessage").html(PleaseSelectRecord);
            $('#dlgCommonErrorMsg').modal({
                escClose: false,
                close: false
            });
            return false;
        }
    }
    else {
        $('#DivLoading').hide();
        $('#dlgCommonErrorMsg').find("#pOkbtn").css('display', '');
        $('#dlgCommonErrorMsg').find("#pYesNobtn").css('display', 'none');
        $('#dlgCommonErrorMsg').find("#pErrMessage").html(errorMsg);
        $('#dlgCommonErrorMsg').modal({
            escClose: false,
            close: false
        });
    }
    return false;
}

var LenBeforeRebind = 0;
var LenAfterRebind = 0;
function PreparePullDataTable(objPullItemDTO) {

    var columnarrIL = new Array();
    columnarrIL.push({
        mDataProp: null, sClass: "read_only", sDefaultContent: '', fnRender: function (obj, val) {
            if (objPullItemDTO.ViewRight == "ViewOverwrite") {
                var strReturn = "<span style='position:relative'>";
                strReturn = strReturn + "<input type='text' value='" + obj.aData.LotOrSerailNumber + "' id='txtLotOrSerailNumber' name='txtLotOrSerailNumber' class='text-boxinner95 AutoSerialLot' />";
                strReturn = strReturn + '<a id="lnkShowAllOptions" href="javascript:void(0);" style="position:absolute; right:5px; top:0px;" class="ShowAllOptionsSL" ><img src="/Content/images/arrow_down_black.png" alt="select" /></a></span>';
                return strReturn;
            }
            else if (objPullItemDTO.ViewRight == "NoRight") // && IsCheckViewRight == false)
            {
                var strReturn = "<span style='position:relative'>";
                strReturn = strReturn + "<input type='text' value='" + obj.aData.LotOrSerailNumber + "' id='txtLotOrSerailNumberNoRight' name='txtLotOrSerailNumberNoRight' class='text-boxinner' />";
                //strReturn = strReturn + '<a id="lnkShowAllOptions" href="javascript:void(0);" style="position:absolute; right:5px; top:0px;" class="ShowAllOptionsSL" ><img src="/Content/images/arrow_down_black.png" alt="select" /></a></span>';
                return strReturn;
            }
            else if (objPullItemDTO.ViewRight == "ViewOnly") {
                var strReturn = "<input type='text' value='" + obj.aData.LotOrSerailNumber + "' id='txtLotOrSerailNumberViewOnly' name='txtLotOrSerailNumberViewOnly' class='text-boxinner' />";
                return strReturn;
            }
            else {
                var strReturn = "<input type='text' value='" + obj.aData.LotOrSerailNumber + "' id='txtLotOrSerailNumberViewOnly' name='txtLotOrSerailNumberViewOnly' class='text-boxinner' />";
                return strReturn;
            }
        }
    });
    // changed by hetal dave to remove [|EmptyStagingBin|]
    //columnarrIL.push({ mDataProp: "BinNumber", sClass: "read_only" });    
    columnarrIL.push({
        mDataProp: null, sClass: "read_only", sDefaultContent: '', fnRender: function (obj, val) {
            var strReturn = "<span name='spnBinNumber' id='spnBinNumber_" + obj.aData.ID + "'>" + (obj.aData.BinNumber == "[|EmptyStagingBin|]" ? "" : obj.aData.BinNumber) + "</span>";
            return strReturn;
        }
    });
    columnarrIL.push({
        mDataProp: null, sClass: "read_only", sDefaultContent: '', fnRender: function (obj, val) {
            var strReturn = "<span name='spnLotSerialQuantity' id='spnLotSerialQuantity_" + obj.aData.ID + "'>" + obj.aData.LotSerialQuantity + "</span>";
            return strReturn;
        }
    });
    columnarrIL.push({
        mDataProp: null, sClass: "read_only", sDefaultContent: '', fnRender: function (obj, val) {

            var RequisitionDetailGUID = '';
            if (obj.aData.RequisitionDetailGUID != null && obj.aData.RequisitionDetailGUID != undefined)
                RequisitionDetailGUID = obj.aData.RequisitionDetailGUID;

            var strReturn = "<input type='hidden' name='hdnRowUniqueId' value='" + obj.aData.ID + "_" + obj.aData.ItemGUID + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnLotNumberTracking' value='" + obj.aData.LotNumberTracking + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnSerialNumberTracking' value='" + obj.aData.SerialNumberTracking + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnDateCodeTracking' value='" + obj.aData.DateCodeTracking + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnExpiration' value='" + obj.aData.Expiration + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnExpirationDate' value='" + obj.aData.strExpirationDate + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnBinNumber' value='" + obj.aData.BinNumber + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnAllowPullBeyondAvailableQty' value='" + objPullItemDTO.AllowPullBeyondAvailableQty + "' />";

            if (objPullItemDTO.SerialNumberTracking == BoolTrueString) {
                strReturn = strReturn + "<input type='text' value='" + FormatedCostQtyValues(obj.aData.PullQuantity, 2) + "' id='txtPullQty_" + obj.aData.ID + "' name='txtPullQty' class='text-boxinner pull-quantity' readonly='readonly' />";
            }
            else {
                strReturn = strReturn + "<input type='text' value='" + FormatedCostQtyValues(obj.aData.PullQuantity, 2) + "' id='txtPullQty_" + obj.aData.ID + "' name='txtPullQty' class='text-boxinner pull-quantity numericinput' />";
            }
            return strReturn;
        }
    });
    columnarrIL.push({ mDataProp: "Received", sClass: "read_only" });
    columnarrIL.push({ mDataProp: "Expiration", sClass: "read_only" });

    //alert(JSON.stringify(columnarrIL));
    var Curtable = $('#' + objPullItemDTO.tableID).dataTable({
        "bPaginate": false,
        "bLengthChange": false,
        "bFilter": false,
        "bSort": false,
        "bInfo": false,
        "bAutoWidth": false,
        "sScrollX": "100%",
        "bRetrieve": true,
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "aoColumns": columnarrIL,
        "sAjaxSource": PullLotSrSelectionUrl,
        "oLanguage": {
        "sLengthMenu": MsgShowRecordsGridBtn,
        "sEmptyTable": MsgNoDataAvailableInTable,
        "sInfo": MsgShowingNoOfEntries,
        "sInfoEmpty": MsgShowingZeroEntries,
        "sZeroRecords": MsgNoDataAvailableInTable
        },
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            if (aData.IsConsignedLotSerial == true) {
                nRow.className = "even trconsigned";
            }
        },
        "fnInitComplete": function (oSettings) {
            var strAllSelected = "";

            $("#hdnSelectedId_" + objPullItemDTO.ItemGUID + "_" + objPullItemDTO.RequisitionDetailGUID).val();
            if (objPullItemDTO.LotNumberTracking != BoolTrueString && objPullItemDTO.SerialNumberTracking != BoolTrueString) {
                $('#' + objPullItemDTO.tableID).dataTable().fnSetColumnVis(0, false);
            }
            if (objPullItemDTO.DateCodeTracking != BoolTrueString) {
                $('#' + objPullItemDTO.tableID).dataTable().fnSetColumnVis(5, false);
            }
        },
        "fnServerData": function (sSource, aoData, fnCallback, oSettings) {

            aoData.push({ "name": "ItemGUID", "value": objPullItemDTO.ItemGUID });
            aoData.push({ "name": "ToolGUID", "value": objPullItemDTO.ToolGUID });
            aoData.push({ "name": "BinID", "value": objPullItemDTO.BinID });
            if (objPullItemDTO.ItemGUID != '00000000-0000-0000-0000-000000000000' && objPullItemDTO.ItemGUID != '')
                aoData.push({ "name": "PullQuantity", "value": FormatedCostQtyValues($("#txtPoolQuantity_" + objPullItemDTO.ItemGUID + "_" + objPullItemDTO.RequisitionDetailGUID).val(), 2) });
            else
                aoData.push({ "name": "PullQuantity", "value": FormatedCostQtyValues($("#txtPoolQuantity_" + objPullItemDTO.ToolGUID + "_" + objPullItemDTO.RequisitionDetailGUID).val(), 2) });
            aoData.push({ "name": "InventoryConsuptionMethod", "value": objPullItemDTO.InventoryConsuptionMethod });
            aoData.push({ "name": "CurrentLoaded", "value": $("#hdnCurrentLoadedIds_" + objPullItemDTO.ItemGUID + "_" + objPullItemDTO.RequisitionDetailGUID).val() });
            aoData.push({ "name": "ViewRight", "value": objPullItemDTO.ViewRight });
            aoData.push({ "name": "IsDeleteRowMode", "value": isDeleteSrLotRow });
            aoData.push({ "name": "MaterialStagingGUID", "value": objPullItemDTO.MaterialStagingGUID });
            aoData.push({ "name": "SupplierAccountGuid", "value": objPullItemDTO.SupplierAccountGuid });

            oSettings.jqXHR = $.ajax({
                dataType: 'json',
                type: "POST",
                url: sSource,
                cache: false,
                data: aoData,
                headers: { "__RequestVerificationToken": $("input[name='__RequestVerificationToken'][type='hidden']").val() },
                success: fnCallback,
                beforeSend: function () {
                    LenBeforeRebind = $('#' + objPullItemDTO.tableID).find("tbody").find("tr").length;
                    $('.dataTables_scroll').css({ "opacity": 0.2 });
                },
                complete: function () {
                    $('.dataTables_scroll').css({ "opacity": 1 });
                    isDeleteSrLotRow = false;
                    $('.ShowAllOptionsSL').click(function () {
                        $(this).siblings('.AutoSerialLot').trigger("focus");
                        $(this).siblings(".AutoSerialLot").autocomplete("search", "");
                    });

                    if (objPullItemDTO.ViewRight == "ViewOnly") {
                        $("input[type='text'][name='txtLotOrSerailNumberViewOnly']").keypress(function () {
                            return false;
                        });

                        $("#DivPullSelection input[type='text'][name='txtPullQty']").keypress(function () {
                            return false;
                        });
                    }

                    LenAfterRebind = $('#' + objPullItemDTO.tableID).find("tbody").find("tr").length;
                    if (LenBeforeRebind == LenAfterRebind && IsLoadMoreLotsClicked == true) {
                        showNotificationDialog();
                        $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
                        $("#spanGlobalMessage").html(MsgNoLocationToAdd);
                    }
                    IsLoadMoreLotsClicked = false;
                }
            });
        }
    });
}

function PullAllNewFlow(isValidateExpiredItem) {
    var ArrItem = new Array();
    var arrItemDetails;
    //alert("In new flow");
    var ErrorMessage = ValidateAllPull();
    if (ErrorMessage == "") {
        $("#DivPullSelection").find("table[id^='tblItemPullheader']").each(function (indx, tblHeader) {
            var $tblHeader = $(tblHeader);
            var strpullobj = JSON.parse($tblHeader.find("input[name='hdnPullMasterDTO']").val());
            arrItemDetails = new Array();
            var ID = $tblHeader.prop("id").split('_')[1];
            var vToolGUID = strpullobj.ToolGUID;
            var RequisitionDetailGUID = $tblHeader.prop("id").split('_')[2];
            var SpanQty = 0;

            if (ID != '00000000-0000-0000-0000-000000000000') {
                SpanQty = $tblHeader.find("#txtPoolQuantity_" + ID + "_" + RequisitionDetailGUID);
            }
            else {
                SpanQty = $tblHeader.find("#txtPoolQuantity_" + vToolGUID + "_" + RequisitionDetailGUID);
            }

            var dt = null;
            if ($("#tblItemPull_" + ID + "_" + RequisitionDetailGUID).length > 0) {
                dt = $("#tblItemPull_" + ID + "_" + RequisitionDetailGUID).dataTable();
            }

            //if (strpullobj.RequisitionDetailGUID != null && strpullobj.RequisitionDetailGUID != undefined && strpullobj.RequisitionDetailGUID != '' && strpullobj.RequisitionDetailGUID != '00000000-0000-0000-0000-000000000000') {
            //    strpullobj.WorkOrderDetailGUID = null;
            //}
            //else if (strpullobj.WorkOrderDetailGUID != null && strpullobj.WorkOrderDetailGUID != undefined && strpullobj.WorkOrderDetailGUID != '' && strpullobj.WorkOrderDetailGUID != '00000000-0000-0000-0000-000000000000') {
            //    strpullobj.RequisitionDetailGUID = null;
            //}
            //else {
            //    strpullobj.WorkOrderDetailGUID = null;
            //    strpullobj.RequisitionDetailGUID = null;
            //}

            if ($("#tblItemPull_" + ID + "_" + RequisitionDetailGUID).length > 0) {
                var currentData = dt.fnGetData();
                $("#tblItemPull_" + ID + "_" + RequisitionDetailGUID).find("tbody").find("tr").each(function (index, tr) {
                    var $tr = $(tr);

                    var txtPullQty = $tr.find("input[type='text'][name='txtPullQty']").val();
                    var hdnLotNumberTracking = $tr.find("input[name='hdnLotNumberTracking']").val();
                    var hdnSerialNumberTracking = $tr.find("input[name='hdnSerialNumberTracking']").val();
                    var hdnDateCodeTracking = $tr.find("input[name='hdnDateCodeTracking']").val();
                    var txtPullQty = $tr.find("input[type='text'][name='txtPullQty']").val();
                    var hdnBinNumber = $tr.find("input[name='hdnBinNumber']").val();
                    var hdnExpiration = $tr.find("input[name='hdnExpiration']").val();

                    if (txtPullQty != "") {
                        var txtLotOrSerailNumber = "";
                        if (hdnLotNumberTracking == "true" || hdnSerialNumberTracking == "true") {
                            txtLotOrSerailNumber = $tr.find("input[type='text'][name^='txtLotOrSerailNumber']").val();
                        }

                        var vSerialNumber = "",
                            vLotNumber = "",
                            vExpiration = "";

                        if (hdnSerialNumberTracking == "true") {
                            vSerialNumber = $.trim(txtLotOrSerailNumber);
                        }
                        if (hdnLotNumberTracking == "true") {
                            vLotNumber = $.trim(txtLotOrSerailNumber);
                        }
                        if (hdnDateCodeTracking == "true") {
                            vExpiration = hdnExpiration;
                        }
                        if (typeof (txtPullQty) != "undefined") {
                            var obj = {
                                "LotOrSerailNumber": $.trim(txtLotOrSerailNumber), "BinNumber": hdnBinNumber, "PullQuantity": parseFloat(txtPullQty.toString())
                                , "LotNumberTracking": hdnLotNumberTracking, "SerialNumberTracking": hdnSerialNumberTracking, "DateCodeTracking": hdnDateCodeTracking
                                , "Expiration": hdnExpiration, "SerialNumber": $.trim(vSerialNumber), "LotNumber": vLotNumber
                                , "ItemGUID": strpullobj.ItemGUID, "BinID": strpullobj.BinID, "ID": strpullobj.BinID
                            };

                            arrItemDetails.push(obj);
                        }
                    }
                }); // loop
            }

            var pullQty = parseFloat($(SpanQty).val());

            var PullItem = {
                ID: indx, ItemGUID: strpullobj.ItemGUID, ToolGUID: strpullobj.ToolGUID, ProjectSpendGUID: strpullobj.ProjectSpendGUID, ProjectSpendName: strpullobj.ProjectSpendName,
                ItemID: strpullobj.ItemID, ItemNumber: strpullobj.ItemNumber, BinID: strpullobj.BinID, BinNumber: strpullobj.BinNumber,
                PullQuantity: pullQty, UDF1: strpullobj.UDF1, UDF2: strpullobj.UDF2, UDF3: strpullobj.UDF3, UDF4: strpullobj.UDF4,
                UDF5: strpullobj.UDF5, PullOrderNumber: strpullobj.PullOrderNumber, lstItemPullDetails: arrItemDetails,
                WorkOrderDetailGUID: strpullobj.WorkOrderDetailGUID, RequisitionDetailsGUID: strpullobj.RequisitionDetailGUID,
                Technician: strpullobj.Technician, TechnicianGUID: strpullobj.TechnicianGUID, ToolCheckoutUDF1: strpullobj.ToolCheckoutUDF1,
                ToolCheckoutUDF2: strpullobj.ToolCheckoutUDF2, ToolCheckoutUDF3: strpullobj.ToolCheckoutUDF3,
                ToolCheckoutUDF4: strpullobj.ToolCheckoutUDF4, ToolCheckoutUDF5: strpullobj.ToolCheckoutUDF5,
                SupplierAccountGuid: strpullobj.SupplierAccountGuid,
                isValidateExpiredItem: isValidateExpiredItem,
                PullCost: strpullobj.ItemSellPrice
            };
            ArrItem.push(PullItem);
        });
        if (ArrItem.length > 0) {
            PullMultipleItemNew(ArrItem, '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000');
        }
    }
    else {
        showNotificationDialog();
        $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
        $("#spanGlobalMessage").html(ErrorMessage);
    }
}

function ConfirmPullExpiredItems(vItemGUID, vRequisitionDetailGUID) {       
    if (typeof vRequisitionDetailGUID == "undefined"
        || vRequisitionDetailGUID == undefined) {
        vRequisitionDetailGUID = "";
    }
    //var vItemGUID = $(this).prop("id").split('_')[1];
    //var vRequisitionDetailGUID = $(this).prop("id").split('_')[2];
    var dtID = "#tblItemPull_" + vItemGUID + "_" + vRequisitionDetailGUID;

    var ArrItem = new Array();
    var arrItemDetails;
    var ErrorMessage = ValidateSinglePull(vItemGUID, vRequisitionDetailGUID);

    if (ErrorMessage == "") {

        arrItemDetails = new Array();
        var ID = vItemGUID;
        var SpanQty = $("#DivPullSelection").find("#txtPoolQuantity_" + vItemGUID + "_" + vRequisitionDetailGUID);

        var dt = $("#tblItemPull_" + vItemGUID + "_" + vRequisitionDetailGUID).dataTable();
        var currentData = dt.fnGetData();

        var strpullobj = JSON.parse($("#hdnPullMasterDTO_" + vItemGUID + "_" + vRequisitionDetailGUID).val());

        $("#tblItemPull_" + vItemGUID + "_" + vRequisitionDetailGUID).find("tbody").find("tr").each(function (index, tr) {
            var $tr = $(tr);
            var txtPullQty = $tr.find("input[type='text'][name='txtPullQty']").val(),
                hdnLotNumberTracking = $tr.find("input[name='hdnLotNumberTracking']").val(),
                hdnSerialNumberTracking = $tr.find("input[name='hdnSerialNumberTracking']").val(),
                hdnDateCodeTracking = $tr.find("input[name='hdnDateCodeTracking']").val(),
                txtPullQty = $tr.find("input[type='text'][name='txtPullQty']").val(),
                hdnBinNumber = $tr.find("input[name='hdnBinNumber']").val(),
                hdnExpiration = $tr.find("input[name='hdnExpiration']").val();

            if (txtPullQty != "") {
                var txtLotOrSerailNumber = "";
                if (hdnLotNumberTracking == "true" || hdnSerialNumberTracking == "true") {
                    txtLotOrSerailNumber = $tr.find("input[type='text'][name^='txtLotOrSerailNumber']").val();
                }

                var vSerialNumber = "",
                    vLotNumber = "",
                    vExpiration = "";

                if (hdnSerialNumberTracking == "true") {
                    vSerialNumber = txtLotOrSerailNumber;
                }
                if (hdnLotNumberTracking == "true") {
                    vLotNumber = txtLotOrSerailNumber;
                }
                if (hdnDateCodeTracking == "true") {
                    vExpiration = hdnExpiration;
                }

                var obj = {
                    "LotOrSerailNumber": txtLotOrSerailNumber, "BinNumber": hdnBinNumber, "PullQuantity": parseFloat(txtPullQty.toString())
                    , "LotNumberTracking": hdnLotNumberTracking, "SerialNumberTracking": hdnSerialNumberTracking, "DateCodeTracking": hdnDateCodeTracking
                    , "Expiration": hdnExpiration, "SerialNumber": $.trim(vSerialNumber), "LotNumber": $.trim(vLotNumber)
                    , "ItemGUID": strpullobj.ItemGUID, "BinID": strpullobj.BinID, "ID": strpullobj.BinID
                };

                arrItemDetails.push(obj);
            }
        }); // loop

        var pullQty = parseFloat($(SpanQty).val().toString());

        var PullItem = {
            ID: 1,
            ItemGUID: strpullobj.ItemGUID,
            ProjectSpendGUID: strpullobj.ProjectSpendGUID,
            ProjectSpendName: strpullobj.ProjectSpendName,
            ItemID: strpullobj.ItemID,
            ItemNumber: strpullobj.ItemNumber,
            BinID: strpullobj.BinID,
            BinNumber: strpullobj.BinNumber,
            PullQuantity: pullQty,
            UDF1: strpullobj.UDF1,
            UDF2: strpullobj.UDF2,
            UDF3: strpullobj.UDF3,
            UDF4: strpullobj.UDF4,
            UDF5: strpullobj.UDF5,
            lstItemPullDetails: arrItemDetails,
            PullOrderNumber: strpullobj.PullOrderNumber,
            WorkOrderDetailGUID: strpullobj.WorkOrderDetailGUID,
            RequisitionDetailsGUID: strpullobj.RequisitionDetailGUID,
            SupplierAccountGuid: strpullobj.SupplierAccountGuid,
            isValidateExpiredItem: false,
            PullCost: strpullobj.ItemSellPrice
        };

        ArrItem.push(PullItem);
        if (ArrItem.length > 0) {
            PullMultipleItemNew(ArrItem, vItemGUID, vRequisitionDetailGUID);
        }
    }
    else {
        showNotificationDialog();
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
        $("#spanGlobalMessage").html(ErrorMessage);
    }
}

function PullSingleItem(indx, ArrItem) {
    var SingleItemArrItem = ArrItem[indx];
    $.ajax({
        type: "POST",
        url: PullSerialsAndLotsUrl,
        contentType: 'application/json',
        dataType: 'json',
        data: JSON.stringify(SingleItemArrItem),
        success: function (RetData) {
            if (RetData.ErrorList.length > 0) {

            }
            else {
                alert(RetData.ErrorList.length);
            }

        },
        error: function (err) {
            //alert(err);
            console.log(err);
        },
        complete: function () {
            if ((indx + 1) < ArrItem.length) {
                PullSingleItem((indx + 1), ArrItem);
            }
        }
    });
}

function PullMultipleItemNew(ArrItem, ItemGuid, ReqGuid) {
    if (typeof ReqGuid == "undefined"
        || ReqGuid == undefined) {
        ReqGuid = "";
    }
    $('#DivLoading').show();
    $.ajax({
        type: "POST",
        url: PullSerialsAndLotsNewUrl,
        contentType: 'application/json',
        dataType: 'json',
        data: JSON.stringify(ArrItem),
        success: function (RetData) {

            var errorMessage = "";
            var expiredErrorMessage = "";
            var _RequisitionDetailGUID = "";
            $.each(RetData, function (indx, RetDataItem) {

                if (RetDataItem.ErrorMessage != null && RetDataItem.ErrorMessage != undefined && RetDataItem.ErrorMessage.trim() > '') {
                    if (RetDataItem.ErrorCode == "16") {
                        expiredErrorMessage += RetDataItem.ErrorMessage + "<br />";
                    }
                    else {
                        errorMessage += RetDataItem.ErrorMessage + "<br />";
                    }
                }
                else if (RetDataItem.ErrorList.length > 0) {
                    $.each(RetDataItem.ErrorList, function (indx, ErrorListItem) {
                        if (ErrorListItem.ErrorCode == "16") {
                            expiredErrorMessage += ErrorListItem.ErrorMessage + "<br />";
                        }
                        else {
                            errorMessage += ErrorListItem.ErrorMessage + "<br />";
                        }
                    });
                }
                else {
                    
                    if (RetDataItem.RequisitionDetailsGUID != null && RetDataItem.RequisitionDetailsGUID != undefined && RetDataItem.RequisitionDetailsGUID.trim() != '') {
                        var $divItemRet = $('#divItem_' + RetDataItem.ItemGUID + '_' + RetDataItem.RequisitionDetailsGUID);
                        $divItemRet.attr('style', '');
                        $divItemRet.remove();
                    }
                    else {
                        var $divItemRet2 = $('#divItem_' + RetDataItem.ItemGUID + '_');
                        $divItemRet2.attr('style', '');
                        $divItemRet2.remove();
                    }
                }

                _RequisitionDetailGUID = RetDataItem.RequisitionDetailsGUID;
            });

            $('#DivLoading').hide();
            if (expiredErrorMessage != "")
            {
                $.modal.impl.close();
                errorMessage = "<b>" + MsgPullExpiredItemList +"</b><br /><br />" + expiredErrorMessage;
                errorMessage = errorMessage + "<br />" + "<b>" + MsgCommonConfirmation +"</b>";
                $('#dlgExpiredItemErrorMsg').find("#pYesForExpiredItem").css('display', '');
                $('#dlgExpiredItemErrorMsg').find("#pErrMessage").html(errorMessage);
                $('#dlgExpiredItemErrorMsg').find("#btnPullExItemGuid").val(ItemGuid);
                $('#dlgExpiredItemErrorMsg').find("#btnPullExReqGuid").val(ReqGuid);
                $('#dlgExpiredItemErrorMsg').modal();
                $('#dlgExpiredItemErrorMsg').css("z-index", "1104");
                $('#simplemodal-overlay').css("z-index", "1103");
                $('#simplemodal-container').css("z-index", "1104");
            }
            else if (errorMessage != "") {
                $.modal.impl.close();
                errorMessage = "<b>" + SomeItemNotPulled +"</b><br /><br />" + errorMessage;
                $('#dlgCommonErrorMsgPopup').find("#pOkbtn").css('display', '');
                //$('#dlgCommonErrorMsg').find("#pYesNobtn").css('display', 'none');
                $('#dlgCommonErrorMsgPopup').find("#pErrMessage").html(errorMessage);
                $('#dlgCommonErrorMsgPopup').modal();
                $('#dlgCommonErrorMsgPopup').css("z-index", "1104");
                $('#simplemodal-overlay').css("z-index", "1103");
                $('#simplemodal-container').css("z-index", "1104");
            }
            else {
                var IsFromPullMaster = false;

                if ($("table[id^='RequisitionItemsTable']").length > 0) {
                    IsFromPullMaster = false;
                }
                else if ($("input[type='hidden'][name^='hdnPullMasterDTO']").length > 0) {
                    IsFromPullMaster = true;
                }
                else {
                    IsFromPullMaster = false;
                }

                if (_RequisitionDetailGUID != null && _RequisitionDetailGUID != undefined && _RequisitionDetailGUID.trim() != '') {
                    $.ajax({
                        "url": CloseRequisitionIfPullCompletedUrl,
                        data: { RequisitionDetailGUID: _RequisitionDetailGUID },
                        "async": false,
                        cache: false,
                        "dataType": "json",
                        success: function (response) {
                            if (IsFromPullMaster == true) {
                                $.modal.impl.close();
                                $('#dlgCommonErrorMsgPopup').find("#pOkbtn").css('display', '');
                                $('#dlgCommonErrorMsgPopup').find("#pErrMessage").html("<b>" + MsgPullDoneSuccess +"</b><br /><br />");
                                $('#dlgCommonErrorMsgPopup').modal();
                                $('#dlgCommonErrorMsgPopup').css("z-index", "1104");
                                $('#simplemodal-overlay').css("z-index", "1103");
                                $('#simplemodal-container').css("z-index", "1104");
                                if ($('div[id^="divItem_"]').length <= 0) {
                                    $('#DivPullSelection').dialog('close');
                                    if ($('#ItemModeDataTable').length > 0)
                                        $('#ItemModeDataTable').dataTable().fnStandingRedraw();
                                }
                            }
                            else {
                                $("#spanGlobalMessage").html(MsgAllPulldone);
                                $('div#target').fadeToggle();
                                $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                                $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                                if ($('div[id^="divItem_"]').length <= 0) {
                                    $('#DivPullSelection').dialog('close');
                                    if ($('#ItemModeDataTable').length > 0)
                                        $('#ItemModeDataTable').dataTable().fnStandingRedraw();
                                }
                            }
                        }
                    });
                }
                else {
                    if (IsFromPullMaster == true) {
                        $.modal.impl.close();
                        $('#dlgCommonErrorMsgPopup').find("#pOkbtn").css('display', '');
                        $('#dlgCommonErrorMsgPopup').find("#pErrMessage").html("<b>" + MsgPullDoneSuccess +"</b><br /><br />");
                        $('#dlgCommonErrorMsgPopup').modal();
                        $('#dlgCommonErrorMsgPopup').css("z-index", "1104");
                        $('#simplemodal-overlay').css("z-index", "1103");
                        $('#simplemodal-container').css("z-index", "1104");
                        if ($('div[id^="divItem_"]').length <= 0) {
                            $('#DivPullSelection').dialog('close');
                            $('#ItemModeDataTable').dataTable().fnStandingRedraw();
                        }
                    }
                    else {
                        $("#spanGlobalMessage").html(MsgAllPulldone);
                        $('div#target').fadeToggle();
                        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                        if ($('div[id^="divItem_"]').length <= 0) {
                            $('#DivPullSelection').dialog('close');
                            $('#ItemModeDataTable').dataTable().fnStandingRedraw();
                        }
                    }
                }
            }
        },
        error: function (err) {
            $('#DivLoading').hide();
            $.modal.impl.close();
            $('#dlgCommonErrorMsgPopup').find("#pOkbtn").css('display', '');
            //$('#dlgCommonErrorMsg').find("#pYesNobtn").css('display', 'none');
            $('#dlgCommonErrorMsgPopup').find("#pErrMessage").html(MsgErrorInProcess);
            $('#dlgCommonErrorMsgPopup').modal();
            $('#dlgCommonErrorMsgPopup').css("z-index", "1004");
            $('#simplemodal-overlay').css("z-index", "1003");
            $('#simplemodal-container').css("z-index", "1004");
        },
        complete: function () {
        }
    });
}

function ValidateSinglePull(vItemGUID, RequisitionDetailGUID) {

    var returnVal = true;
    var errormsg = "";
    var isMoreQty = false;
    var dtID = "#tblItemPull_" + vItemGUID + "_" + RequisitionDetailGUID;

    var SpanQty = $("#DivPullSelection").find("#txtPoolQuantity_" + vItemGUID + "_" + RequisitionDetailGUID);

    var TotalEntered = 0;
    $("#tblItemPull_" + vItemGUID + "_" + RequisitionDetailGUID).find("tbody").find("tr").each(function (index, tr) {
        var $tr = $(tr);
        var txtPullQty = $tr.find("input[type='text'][name='txtPullQty']").val();
        var spnLotSerialQuantity = $tr.find("span[name='spnLotSerialQuantity']").text();

        if (parseFloat(txtPullQty) > parseFloat(spnLotSerialQuantity)) {
            var hdnLotNumberTracking = $tr.find("input[name='hdnLotNumberTracking']").val(),
                hdnSerialNumberTracking = $tr.find("input[name='hdnSerialNumberTracking']").val(),
                hdnDateCodeTracking = $tr.find("input[name='hdnDateCodeTracking']").val(),
                hdnAllowPullBeyondAvailableQty = $tr.find("input[name='hdnAllowPullBeyondAvailableQty']").val();
            var isAllowNegative = false;
            if (hdnAllowPullBeyondAvailableQty == "True"
                && hdnSerialNumberTracking == "false"
                && hdnLotNumberTracking == "false"
                && hdnDateCodeTracking == "false") {
                isAllowNegative = true;
            }
            if (isAllowNegative == false) {
                errormsg = "\n" + MsgPullMoreQuantityValidation;
                isMoreQty = true;
                return errormsg;
            }
        }

        TotalEntered = TotalEntered + parseFloat(txtPullQty);
    });

    if (isMoreQty == false) {
        var pullQty = parseFloat($(SpanQty).val().toString());
        if (TotalEntered != pullQty) {
            errormsg = errormsg + "\n" + MsgEnteredPullQuantityValidation.replace("{0}", TotalEntered).replace("{1}", pullQty);
        }
    }
    else {
        errormsg = MsgPullMoreQuantityValidation;
    }

    return errormsg;
}

function ValidateAllPull() {
    var returnVal = true;
    var errormsg = "";
    var isMoreQty = false;
    $("#DivPullSelection").find("table[id^='tblItemPullheader']").each(function (indx, tblHeader) {
        var $tblHeader = $(tblHeader);
        var ID = $tblHeader.prop("id").split('_')[1];
        var RequisitionDetailGUID = $tblHeader.prop("id").split('_')[2];
        var SpanQty = $tblHeader.find("#txtPoolQuantity_" + ID + "_" + RequisitionDetailGUID);

        var TotalEntered = 0;
        if ($("#tblItemPull_" + ID + "_" + RequisitionDetailGUID).length > 0) {
            $("#tblItemPull_" + ID + "_" + RequisitionDetailGUID).find("tbody").find("tr").each(function (index, tr) {
                var $tr = $(tr);
                if ($tr.find("input[type='text'][name='txtPullQty']").length > 0) {
                    var txtPullQty = $tr.find("input[type='text'][name='txtPullQty']").val();
                    var spnLotSerialQuantity = $tr.find("span[name='spnLotSerialQuantity']").text();

                    if (parseFloat(txtPullQty) > parseFloat(spnLotSerialQuantity)) {
                        var hdnLotNumberTracking = $tr.find("input[name='hdnLotNumberTracking']").val(),
                            hdnSerialNumberTracking = $tr.find("input[name='hdnSerialNumberTracking']").val(),
                            hdnDateCodeTracking = $tr.find("input[name='hdnDateCodeTracking']").val(),
                            hdnAllowPullBeyondAvailableQty = $tr.find("input[name='hdnAllowPullBeyondAvailableQty']").val();
                        var isAllowNegative = false;
                        if (hdnAllowPullBeyondAvailableQty == "True"
                            && hdnSerialNumberTracking == "false"
                            && hdnLotNumberTracking == "false"
                            && hdnDateCodeTracking == "false") {
                            isAllowNegative = true;
                        }
                        if (isAllowNegative == false) {
                            errormsg = "\n" + MsgPullMoreQuantityValidation;
                            isMoreQty = true;
                            return errormsg;
                        }
                    }

                    TotalEntered = TotalEntered + parseFloat(txtPullQty);
                }
            }); // loop

            if (isMoreQty == false) {
                var pullQty = parseFloat($(SpanQty).val().toString());
                if (TotalEntered != pullQty) {
                    ////errormsg = errormsg + "\nentered :" + TotalEntered + "\tPull Qty :" + pullQty;
                    errormsg = errormsg + MsgEnteredPullQuantityValidation.replace("{0}", TotalEntered).replace("{1}", pullQty);
                }
            }
            else {
                errormsg = MsgPullMoreQuantityValidation;
            }
        }
    });

    return errormsg;
}


var IsLoadMoreLotsClicked = false;
$(document).ready(function () {
    $("#DivPullSelection").dialog({
        autoOpen: false,
        show: "blind",
        hide: "explode",
        height: 700,
        title: MsgPullDetails,
        width: 900,
        modal: true,
        open: function () {
        },
        beforeClose: function () {
        },
        close: function () {
            //$('.ui-widget-overlay').css('position', 'absolute');
            IsRefreshGrid = true;
            $('#DivLoading').hide();
            $("#DivPullSelection").empty();
            if (PullPageName == "REQUISITION") {
                ShowEditTabGUIDTRUEOnly("RequisitionEdit?RequisitionGUID=" + $('#hdnRequisitionGUID').val(), "frmRequisitionMaster");
            }
            else {
                $('#ItemModeDataTable').dataTable().fnStandingRedraw();
            }
        }
    });

    $("#DivPullSelection").off('change', "input[type='text'][name^='txtLotOrSerailNumber']");
    $("#DivPullSelection").on('change', "input[type='text'][name^='txtLotOrSerailNumber']", function (e) {

        var objCurtxt = $(this);
        var oldValue = $(objCurtxt).val();
        //var ids = $(this).parent().parent().parent().find("input[type='hidden'][name='hdnRowUniqueId']").val().split('_');
        var ids = $(this).parent().parent().parent().parent().parent().parent().parent().parent().parent().find("[id^='hdnPullIds_']").val().split('_');
        var aPos = $("#tblItemPull_" + ids[1].toString() + "_" + ids[2].toString()).dataTable().fnGetPosition($(this).parent().parent().parent()[0]);
        var aData = $("#tblItemPull_" + ids[1].toString() + "_" + ids[2].toString()).dataTable().fnGetData(aPos);

        var dtThisItem = $("#tblItemPull_" + ids[1].toString() + "_" + ids[2].toString()).dataTable();
        var currentTR = $(objCurtxt).parent().parent().parent()[0];
        var row_id = dtThisItem.fnGetPosition(currentTR);

        if ($.trim(oldValue) == '')
            return;

        var isDuplicateEntry = false;
        var OtherPullQuantity = 0;
        var strpullobj = JSON.parse($("#hdnPullMasterDTO_" + ids[1].toString() + "_" + ids[2].toString()).val());
        var materialStagingGuid = strpullobj.MaterialStagingGUID;

        $("#tblItemPull_" + ids[1].toString() + "_" + ids[2].toString() + " tbody tr").each(function (i) {
            if (i != row_id) {
                var tr = $(this);
                var SerialOrLotNumber = tr.find('#' + objCurtxt.prop("id")).val();
                if (SerialOrLotNumber == $(objCurtxt).val()) {
                    isDuplicateEntry = true;
                }
                else {
                    var txtPullQty = tr.find("input[type='text'][name='txtPullQty']").val();
                    OtherPullQuantity = OtherPullQuantity + parseFloat(txtPullQty);
                }
            }
        });

        if (isDuplicateEntry == true) {

            if ($("#hdnTrackingType_" + ids[1].toString() + "_" + ids[2].toString()).val() == "LotNumberTracking") {
                showNotificationDialog();
                $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                $("#spanGlobalMessage").html(MsgDuplicateLotNumber);
            }
            else if ($("#hdnTrackingType_" + ids[1].toString() + "_" + ids[2].toString()).val() == "SerialNumberTracking") {
                showNotificationDialog();
                $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                $("#spanGlobalMessage").html(DuplicateSerialFound);
            }
            else {
                showNotificationDialog();
                $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                $("#spanGlobalMessage").html(DuplicateNumber);
            }
            $(objCurtxt).val("");
            $(objCurtxt).focus();
        }
        else {
            $.ajax({
                type: "POST",
                url: ValidateSerialLotNumberUrl,
                contentType: 'application/json',
                dataType: 'json',
                data: "{ ItemGuid: '" + ids[1].toString() + "', SerialOrLotNumber: '" + $.trim($(objCurtxt).val()) + "',BinID: '" + aData.BinID + "',MaterialStagingGUID:'" + materialStagingGuid + "' }",
                success: function (RetData) {
                    if (RetData.ID > 0) {
                        IsCheckViewRight = false;

                        var spnPoolQuantity = parseFloat($("#txtPoolQuantity_" + ids[1].toString() + "_" + ids[2].toString()).val());
                        if ((spnPoolQuantity - OtherPullQuantity) > 0) {
                            if ((spnPoolQuantity - OtherPullQuantity) < RetData.PullQuantity)
                                RetData.PullQuantity = spnPoolQuantity - OtherPullQuantity;
                        }
                        else {
                            RetData.PullQuantity = 0;
                        }

                        dtThisItem.fnUpdate(RetData, row_id, undefined, false, false);
                        IsCheckViewRight = true;

                        $('.ShowAllOptionsSL').click(function () {
                            $(this).siblings('.AutoSerialLot').trigger("focus");
                            $(this).siblings(".AutoSerialLot").autocomplete("search", "");
                        });

                        if (RetData.IsConsignedLotSerial) {
                            $(currentTR).addClass("trconsigned");
                        }
                        else {
                            $(currentTR).removeClass("trconsigned");
                        }
                    }
                    else {
                        $(objCurtxt).val("");
                        $(objCurtxt).focus();
                    }
                },
                error: function (err) {
                    //alert(err);
                    console.log(err);
                }
            });
        }
    });

    $("#DivPullSelection").off('focus', "input[type='text'][name^='txtLotOrSerailNumber']");
    $("#DivPullSelection").on('focus', "input[type='text'][name^='txtLotOrSerailNumber']", function (e) {

        var objCurtxt = $(this);
        //var ids = $(this).parent().parent().parent().find("input[type='hidden'][name='hdnRowUniqueId']").val().split('_');
        var ids = $(this).parent().parent().parent().parent().parent().parent().parent().parent().parent().find("[id^='hdnPullIds_']").val().split('_');
        var aPos = $("#tblItemPull_" + ids[1].toString() + "_" + ids[2].toString()).dataTable().fnGetPosition($(this).parent().parent().parent()[0]);
        var aData = $("#tblItemPull_" + ids[1].toString() + "_" + ids[2].toString()).dataTable().fnGetData(aPos);

        var dtItemPull = "#tblItemPull_" + ids[1].toString() + "_" + ids[2].toString();
        var strSerialLotNos = "";
        var strpullobj = JSON.parse($("#hdnPullMasterDTO_" + ids[1].toString() + "_" + ids[2].toString()).val());
        var materialStagingGuid = strpullobj.MaterialStagingGUID;

        $(dtItemPull).find("tbody").find("tr").each(function (index, tr) {

            if (index != aPos) {
                var $tr = $(tr);
                var hdnLotNumberTracking = $tr.find("input[name='hdnLotNumberTracking']").val(),
                    hdnSerialNumberTracking = $tr.find("input[name='hdnSerialNumberTracking']").val(),
                    hdnDateCodeTracking = $tr.find("input[name='hdnDateCodeTracking']").val();

                if (hdnLotNumberTracking == "true" || hdnSerialNumberTracking == "true") {
                    var txtLotOrSerailNumber = $.trim($tr.find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                    if (txtLotOrSerailNumber != undefined) {
                        strSerialLotNos = strSerialLotNos + txtLotOrSerailNumber + "|#|";
                    }
                }
                else if (hdnDateCodeTracking == "true") {
                    var hdnExpiration = $tr.find("input[name='hdnExpiration']").val();
                    if (hdnExpiration != undefined) {
                        strSerialLotNos = strSerialLotNos + hdnExpiration + "|#|";
                    }
                }
                else {
                    var hdnBinNumber = $tr.find("input[name='hdnBinNumber']").val();
                    if (hdnBinNumber != undefined) {
                        strSerialLotNos = strSerialLotNos + hdnBinNumber + "|#|";
                    }
                }
            }

        }); // loop

        if ($(this).hasClass("AutoSerialLot")) {
            $(this).autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: '/Pull/GetLotOrSerailNumberList',
                        contentType: 'application/json',
                        dataType: 'json',
                        data: {
                            maxRows: 1000,
                            name_startsWith: request.term,
                            ItemGuid: ids[1].toString(),
                            BinID: aData.BinID,
                            prmSerialLotNos: strSerialLotNos,
                            materialStagingGUID: materialStagingGuid
                        },
                        success: function (data) {
                            response($.map(data, function (item) {
                                return {
                                    label: item.LotOrSerailNumber,
                                    value: item.LotOrSerailNumber,
                                    selval: item.LotOrSerailNumber
                                }
                            }));
                        },
                        error: function (err) {

                        }
                    });
                },
                autoFocus: false,
                minLength: 0,
                select: function (event, ui) {
                    //                    $(objCurtxt).parent().parent().find("input[type='hidden'][name='hdnStagingBin']").val(ui.item.selval);
                    //                    $(objCurtxt).parent().parent().find("input[type='hidden'][name='hdnStagingBinName']").val(ui.item.label);
                },
                open: function () {
                    $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
                    $(this).autocomplete('widget').css('z-index', 9000);
                    $('ul.ui-autocomplete').css('overflow-y', 'auto');
                    $('ul.ui-autocomplete').css('max-height', '300px');
                },
                close: function () {
                    $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
                    $(objCurtxt).trigger("change");
                }
            });
        }
    });

    $("#DivPullSelection").off("click", "input[type='button'][name='btnLoadMoreLots']");
    $("#DivPullSelection").on("click", "input[type='button'][name='btnLoadMoreLots']", function () {
        var vItemGUID = $(this).prop("id").split('_')[1];
        var vRequisitionDetailGUID = $(this).prop("id").split('_')[2];

        var dtID = "#tblItemPull_" + vItemGUID + "_" + vRequisitionDetailGUID;
        var strIds = "";

        var MaxQuantity = $("#txtPoolQuantity_" + vItemGUID + "_" + vRequisitionDetailGUID)[0].value;
        var TotalQuantity = 0;
        $("#tblItemPull_" + vItemGUID + "_" + vRequisitionDetailGUID).find("[id*='txtPullQty_']").each(function () {
            TotalQuantity = TotalQuantity + parseInt($(this)[0].value);
        });

        if (MaxQuantity > TotalQuantity) {
            IsLoadMoreLotsClicked = true;
            $(dtID).find("tbody").find("tr").each(function (index, tr) {

                var $tr = $(tr);

                var hdnLotNumberTracking = $tr.find("input[name='hdnLotNumberTracking']").val(),
                    hdnSerialNumberTracking = $tr.find("input[name='hdnSerialNumberTracking']").val(),
                    hdnDateCodeTracking = $tr.find("input[name='hdnDateCodeTracking']").val(),
                    txtPullQty = $tr.find("input[type='text'][name='txtPullQty']").val();
               
                if (txtPullQty != undefined) {
                    if (txtPullQty == "") {
                        txtPullQty = "0";
                    }
                    if ((hdnLotNumberTracking == "true" && hdnDateCodeTracking == "false")
                        || (hdnSerialNumberTracking == "true" && hdnDateCodeTracking == "false")) {
                        var txtLotOrSerailNumber = $.trim($tr.find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                        if (txtLotOrSerailNumber != undefined && !IsLotSerialExistsInCurrentLoaded(strIds, txtLotOrSerailNumber)) {
                            strIds = strIds + txtLotOrSerailNumber + "_" + txtPullQty + ",";
                        }
                    }
                        //else if (hdnDateCodeTracking == "true") {
                    else if ((hdnLotNumberTracking == "true" && hdnDateCodeTracking == "true")
                        || (hdnSerialNumberTracking == "true" && hdnDateCodeTracking == "true")) {
                        var hdnExpiration = $tr.find("input[name='hdnExpirationDate']").val();
                        var txtLotOrSerailNumber = $.trim($tr.find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                        if (txtLotOrSerailNumber != undefined && hdnExpiration != undefined && !IsLotSerialExistsInCurrentLoaded(strIds, hdnExpiration)) {
                            strIds = strIds + txtLotOrSerailNumber + "_" + hdnExpiration + "_" + txtPullQty + ",";
                        }
                    }
                    else if (hdnLotNumberTracking == "false" && hdnSerialNumberTracking == "false" && hdnDateCodeTracking == "true") {
                        var hdnExpiration = $tr.find("input[name='hdnExpirationDate']").val();
                        if (hdnExpiration != undefined) {
                            strIds = strIds + hdnExpiration + "_" + txtPullQty + ",";
                        }
                    }
                    else {
                        var hdnBinNumber = $tr.find("input[name='hdnBinNumber']").val();
                        if (hdnBinNumber != undefined && !IsLotSerialExistsInCurrentLoaded(strIds, hdnBinNumber))
                            strIds = strIds + hdnBinNumber + "_" + txtPullQty + ",";
                    }
                }
            });//loop

            $("#hdnCurrentLoadedIds_" + vItemGUID + "_" + vRequisitionDetailGUID).val(strIds);

            var dt = $(dtID).dataTable();
            dt.fnStandingRedraw();
        }
        else {
            showNotificationDialog();
            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            $("#spanGlobalMessage").html(MsgPullCreditQuantity);
            //$("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            //$("#spanGlobalMessage").text("Can not add new row because total Pull/Credit Quantity is greater then or equal to Pull/Credit Quantity.");
            //$('div#target').fadeToggle();
            //$("div#target").delay(2000).fadeOut(200);
        }
    });

    function IsLotSerialExistsInCurrentLoaded(strIds, SerialLot) {
        if (SerialLot.trim() == '')
            return true;

        if (strIds.trim() == '')
            return false

        var ArrIds = strIds.split(',');
        var i = 0;
        for (i = 0; i < ArrIds.length; i++) {
            if (ArrIds[i].split('_')[0].toLowerCase() == SerialLot.toLowerCase()) {
                return true;
            }
        }

        return false;
    }

    $("#DivPullSelection").off("click", "input[type='button'][name='btnPullPopup']");
    $("#DivPullSelection").on("click", "input[type='button'][name='btnPullPopup']", function () {
        //RequisitionDetailsGUID
        var vItemGUID = $(this).prop("id").split('_')[1];
        var vRequisitionDetailGUID = $(this).prop("id").split('_')[2];
        var dtID = "#tblItemPull_" + vItemGUID + "_" + vRequisitionDetailGUID;
        var ArrItem = new Array();
        var arrItemDetails;
        var ErrorMessage = ValidateSinglePull(vItemGUID, vRequisitionDetailGUID);

        if (ErrorMessage == "") {

            arrItemDetails = new Array();
            var ID = vItemGUID;
            var SpanQty = $("#DivPullSelection").find("#txtPoolQuantity_" + vItemGUID + "_" + vRequisitionDetailGUID);

            var dt = $("#tblItemPull_" + vItemGUID + "_" + vRequisitionDetailGUID).dataTable();
            var currentData = dt.fnGetData();

            var strpullobj = JSON.parse($("#hdnPullMasterDTO_" + vItemGUID + "_" + vRequisitionDetailGUID).val());

            //if (strpullobj.RequisitionDetailGUID != null && strpullobj.RequisitionDetailGUID != undefined && strpullobj.RequisitionDetailGUID != '' && strpullobj.RequisitionDetailGUID != '00000000-0000-0000-0000-000000000000') {
            //    strpullobj.WorkOrderDetailGUID = null;
            //}
            //else if (strpullobj.WorkOrderDetailGUID != null && strpullobj.WorkOrderDetailGUID != undefined && strpullobj.WorkOrderDetailGUID != '' && strpullobj.WorkOrderDetailGUID != '00000000-0000-0000-0000-000000000000') {
            //    strpullobj.RequisitionDetailGUID = null;
            //}
            //else {
            //    strpullobj.WorkOrderDetailGUID = null;
            //    strpullobj.RequisitionDetailGUID = null;
            //}

            $("#tblItemPull_" + vItemGUID + "_" + vRequisitionDetailGUID).find("tbody").find("tr").each(function (index, tr) {
                var $tr = $(tr);
                var txtPullQty = $tr.find("input[type='text'][name='txtPullQty']").val(),
                    hdnLotNumberTracking = $tr.find("input[name='hdnLotNumberTracking']").val(),
                    hdnSerialNumberTracking = $tr.find("input[name='hdnSerialNumberTracking']").val(),
                    hdnDateCodeTracking = $tr.find("input[name='hdnDateCodeTracking']").val(),
                    txtPullQty = $tr.find("input[type='text'][name='txtPullQty']").val(),
                    hdnBinNumber = $tr.find("input[name='hdnBinNumber']").val(),
                    hdnExpiration = $tr.find("input[name='hdnExpiration']").val();

                if (txtPullQty != "") {
                    var txtLotOrSerailNumber = "";
                    if (hdnLotNumberTracking == "true" || hdnSerialNumberTracking == "true") {
                        txtLotOrSerailNumber = $tr.find("input[type='text'][name^='txtLotOrSerailNumber']").val();
                    }

                    var vSerialNumber = "",
                        vLotNumber = "",
                        vExpiration = "";

                    if (hdnSerialNumberTracking == "true") {
                        vSerialNumber = txtLotOrSerailNumber;
                    }
                    if (hdnLotNumberTracking == "true") {
                        vLotNumber = txtLotOrSerailNumber;
                    }
                    if (hdnDateCodeTracking == "true") {
                        vExpiration = hdnExpiration;
                    }

                    var obj = {
                        "LotOrSerailNumber": txtLotOrSerailNumber, "BinNumber": hdnBinNumber, "PullQuantity": parseFloat(txtPullQty.toString())
                                    , "LotNumberTracking": hdnLotNumberTracking, "SerialNumberTracking": hdnSerialNumberTracking, "DateCodeTracking": hdnDateCodeTracking
                                    , "Expiration": hdnExpiration, "SerialNumber": $.trim(vSerialNumber), "LotNumber": $.trim(vLotNumber)
                        , "ItemGUID": strpullobj.ItemGUID, "BinID": strpullobj.BinID, "ID": strpullobj.BinID
                    };

                    arrItemDetails.push(obj);
                }
            }); // loop

            var pullQty = parseFloat($(SpanQty).val().toString());

            var PullItem = {
                ID: 1,
                ItemGUID: strpullobj.ItemGUID,
                ProjectSpendGUID: strpullobj.ProjectSpendGUID,
                ProjectSpendName: strpullobj.ProjectSpendName,
                ItemID: strpullobj.ItemID,
                ItemNumber: strpullobj.ItemNumber,
                BinID: strpullobj.BinID,
                BinNumber: strpullobj.BinNumber,
                PullQuantity: pullQty,
                UDF1: strpullobj.UDF1,
                UDF2: strpullobj.UDF2,
                UDF3: strpullobj.UDF3,
                UDF4: strpullobj.UDF4,
                UDF5: strpullobj.UDF5,
                lstItemPullDetails: arrItemDetails,
                PullOrderNumber: strpullobj.PullOrderNumber,
                WorkOrderDetailGUID: strpullobj.WorkOrderDetailGUID,
                RequisitionDetailsGUID: strpullobj.RequisitionDetailGUID,
                SupplierAccountGuid: strpullobj.SupplierAccountGuid,
                isValidateExpiredItem: true,
                PullCost: strpullobj.ItemSellPrice
            };

            ArrItem.push(PullItem);            
            if (ArrItem.length > 0) {
                PullMultipleItemNew(ArrItem, vItemGUID, vRequisitionDetailGUID);
            }
        }
        else {
            showNotificationDialog();
            $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
            $("#spanGlobalMessage").html(ErrorMessage);
        }
    });

    $("#DivPullSelection").off("click", "input[type='button'][name='btnPullAllPopUp']");
    $("#DivPullSelection").on("click", "input[type='button'][name='btnPullAllPopUp']", function () {
        PullAllNewFlow(true);
    });

    $("#DivPullSelection").off("click", "input[type='button'][name='btnCancelPullPopup']");
    $("#DivPullSelection").on("click", "input[type='button'][name='btnCancelPullPopup']", function () {
        $("#DivPullSelection").empty();
        $('#DivPullSelection').dialog('close');
        //$('#ItemModeDataTable').dataTable().fnStandingRedraw();
    });

    $("#DivPullSelection").off('click', "input[type='button'][name='btnValidateThis']");
    $("#DivPullSelection").on('click', "input[type='button'][name='btnValidateThis']", function (e) {

        var dt = $("#tblItemPull_" + $(this).prop("id").split('_')[1] + "_" + $(this).prop("id").split('_')[2]).dataTable();
        var currentData = dt.fnGetData();
        var TotalEntered = 0;
        $(currentData).each(function (indx, obj) {
            TotalEntered = TotalEntered + parseFloat(obj.PullQuantity);
        });
        var pullQty = parseFloat($("#DivPullSelection").find("#txtPoolQuantity_" + $(this).prop("id").split('_')[1] + "_" + $(this).prop("id").split('_')[2]).val().toString());
        //alert("Entered :" + TotalEntered + ", Pull Qty :" + pullQty);
        if (TotalEntered != pullQty) {
            showNotificationDialog();
            $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
            $("#spanGlobalMessage").html(MsgEnterPullQuantity);
        }
        else {
            showNotificationDialog();
            $("#spanGlobalMessage").removeClass('WarningIcon errorIcon').addClass('succesIcon');
            $("#spanGlobalMessage").html(MsgAlertValidated);

        }

    });

    $("#DivPullSelection").off('click', "input[type='button'][name='btnvalidateAll']");
    $("#DivPullSelection").on('click', "input[type='button'][name='btnvalidateAll']", function (e) {
        var returnmsg = ValidateAllPull();
        if (returnmsg == "") {
            returnmsg = MsgAlertValidated;
            showNotificationDialog();
            $("#spanGlobalMessage").removeClass('WarningIcon errorIcon').addClass('succesIcon');
            $("#spanGlobalMessage").html(returnmsg);
        } else {
            showNotificationDialog();
            $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
            $("#spanGlobalMessage").html(returnmsg);
        }
    });

    $("#DivPullSelection").off("change", "input[type='text'][name='txtPullQty']");
    $("#DivPullSelection").on("change", "input[type='text'][name='txtPullQty']", function () {
        //var ids = $(this).parent().parent().find("input[type='hidden'][name='hdnRowUniqueId']").val().split('_');
        var ids = $(this).parent().parent().parent().parent().parent().parent().parent().parent().find("[id^='hdnPullIds_']").val().split('_');
        var aPos = $("#tblItemPull_" + ids[1].toString() + "_" + ids[2].toString()).dataTable().fnGetPosition($(this).parent().parent()[0]);
        $("#tblItemPull_" + ids[1].toString() + "_" + ids[2].toString()).dataTable().fnGetData(aPos).PullQuantity = $(this).val();
    });

    $("#DivPullSelection").off("tap click", ".tbl-item-pull tbody tr");
    $("#DivPullSelection").on("tap click", ".tbl-item-pull tbody tr", function (e) {
        if (e.target.type == "checkbox" || e.target.type == "radio" || e.target.type == "text") {
            e.stopPropagation();
        }
        else if (e.currentTarget.getElementsByTagName("input").btnLoad != undefined) {
            e.stopPropagation();
        }
        else {
            if ((e.metaKey || e.ctrlKey)) {
                $(this).toggleClass('row_selected');
            } else {
                $(this).toggleClass('row_selected');
            }
        }
        return false;
    });

    $("#DivPullSelection").off('click', "input[type='button'][name='btnDeleteLots']");
    $("#DivPullSelection").on('click', "input[type='button'][name='btnDeleteLots']", function (e) {

        var vItemGUID = $(this).prop("id").split('_')[1];
        var vRequisitionDetailGUID = $(this).prop("id").split('_')[2];

        var dtID = "#tblItemPull_" + vItemGUID + "_" + vRequisitionDetailGUID;

        var TotalRows = $(dtID + ' tbody tr').length;
        var SelectedRows = $(dtID + ' tbody tr.row_selected').length;
        var RemainingRows = TotalRows - SelectedRows;

        if (SelectedRows <= 0) {
            $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
            $("#spanGlobalMessage").html(MsgSelectRowToDelete);
            showNotificationDialog();
        }
        else {
            if (RemainingRows >= 1) {
                $(dtID).find("tbody").find("tr.row_selected").each(function (index, tr) {
                    $(tr).remove();
                });

                var strIds = "";
                $(dtID).find("tbody").find("tr").each(function (index, tr) {
                    var $tr = $(tr);
                    var hdnLotNumberTracking = $tr.find("input[name='hdnLotNumberTracking']").val(),
                        hdnSerialNumberTracking = $tr.find("input[name='hdnSerialNumberTracking']").val(),
                        hdnDateCodeTracking = $tr.find("input[name='hdnDateCodeTracking']").val(),
                        txtPullQty = $tr.find("input[type='text'][name='txtPullQty']").val();

                    if (txtPullQty == "") {
                        txtPullQty = "0";
                    }

                    //if (hdnLotNumberTracking == "true" || hdnSerialNumberTracking == "true") {
                    //    var txtLotOrSerailNumber = $(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val();
                    //    if (!IsLotSerialExistsInCurrentLoaded(strIds, txtLotOrSerailNumber))
                    //        strIds = strIds + txtLotOrSerailNumber + "_" + txtPullQty + ",";
                    //}
                    //else if (hdnDateCodeTracking == "true") {
                    //    var hdnExpiration = $(tr).find("input[name='hdnExpiration']").val();
                    //    if (!IsLotSerialExistsInCurrentLoaded(strIds, hdnExpiration))
                    //        strIds = strIds + hdnExpiration + "_" + txtPullQty + ",";
                    //}
                    //else {
                    //    var hdnBinNumber = $(tr).find("input[name='hdnBinNumber']").val();
                    //    if (!IsLotSerialExistsInCurrentLoaded(strIds, hdnBinNumber))
                    //        strIds = strIds + hdnBinNumber + "_" + txtPullQty + ",";
                    //}

                    if ((hdnLotNumberTracking == "true" && hdnDateCodeTracking == "false")
                        || (hdnSerialNumberTracking == "true" && hdnDateCodeTracking == "false")) {
                        var txtLotOrSerailNumber = $.trim($tr.find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                        if (txtLotOrSerailNumber != undefined && !IsLotSerialExistsInCurrentLoaded(strIds, txtLotOrSerailNumber)) {
                            strIds = strIds + txtLotOrSerailNumber + "_" + txtPullQty + ",";
                        }
                    }
                        //else if (hdnDateCodeTracking == "true") {
                    else if ((hdnLotNumberTracking == "true" && hdnDateCodeTracking == "true")
                        || (hdnSerialNumberTracking == "true" && hdnDateCodeTracking == "true")) {
                        var hdnExpiration = $tr.find("input[name='hdnExpirationDate']").val();
                        var txtLotOrSerailNumber = $.trim($tr.find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                        if (txtLotOrSerailNumber != undefined && hdnExpiration != undefined && !IsLotSerialExistsInCurrentLoaded(strIds, hdnExpiration)) {
                            strIds = strIds + txtLotOrSerailNumber + "_" + hdnExpiration + "_" + txtPullQty + ",";
                        }
                    }
                    else if (hdnLotNumberTracking == "false" && hdnSerialNumberTracking == "false" && hdnDateCodeTracking == "true") {
                        var hdnExpiration = $tr.find("input[name='hdnExpirationDate']").val();
                        if (hdnExpiration != undefined) {
                            strIds = strIds + hdnExpiration + "_" + txtPullQty + ",";
                        }
                    }
                    else {
                        var hdnBinNumber = $tr.find("input[name='hdnBinNumber']").val();
                        if (hdnBinNumber != undefined && !IsLotSerialExistsInCurrentLoaded(strIds, hdnBinNumber)) {
                            strIds = strIds + hdnBinNumber + "_" + txtPullQty + ",";
                        }
                    }

                });// loop

                $("#hdnCurrentLoadedIds_" + vItemGUID + "_" + vRequisitionDetailGUID).val(strIds);
                isDeleteSrLotRow = true;
                var dtThisItem = $(dtID).dataTable();
                dtThisItem.fnStandingRedraw();
            }
            else {
                showNotificationDialog();
                $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
                $("#spanGlobalMessage").html(MsgRowShouldExists);
                //$("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                //$("#spanGlobalMessage").text("Can not add new row because total Pull/Credit Quantity is greater then or equal to Pull/Credit Quantity.");
                //$('div#target').fadeToggle();
                //$("div#target").delay(2000).fadeOut(200);
            }

        }
    });


    $("#DivPullSelection").off("click", "input[type='button'][name='btnToolPopup']");
    $("#DivPullSelection").on("click", "input[type='button'][name='btnToolPopup']", function () {
        //RequisitionDetailsGUID
        var vToolGUID = $(this).prop("id").split('_')[1];
        var vRequisitionDetailGUID = $(this).prop("id").split('_')[2];
        var dtID = "#tblItemPull_" + vToolGUID + "_" + vRequisitionDetailGUID;

        var ArrItem = new Array();
        var arrItemDetails;
        var ErrorMessage = ''// ValidateSinglePull(vItemGUID, vRequisitionDetailGUID);



        arrItemDetails = new Array();
        var ID = vToolGUID;
        var SpanQty = $("#DivPullSelection").find("#txtPoolQuantity_" + vToolGUID + "_" + vRequisitionDetailGUID);

        var dt = $("#tblItemPull_" + vToolGUID + "_" + vRequisitionDetailGUID).dataTable();
        //var currentData = dt.fnGetData();

        var strpullobj = JSON.parse($("#hdnPullMasterDTO_" + vToolGUID + "_" + vRequisitionDetailGUID).val());

        var txtQty = parseFloat($(SpanQty).val().toString());
        var hdnRequisitionGUID = '';
        var reqToolCheckout = {
            "ID": 0, "PullCreditQuantity": txtQty, "BinID": 0, "PullCredit": 'checkout', "TempPullQTY": txtQty,
            "RequisitionDetailGUID": vRequisitionDetailGUID, "WorkOrderDetailGUID": strpullobj.WorkOrderDetailGUID, "RequisitionMasterGUID": hdnRequisitionGUID, "ToolGUID": vToolGUID,
            "TechnicianGUID": strpullobj.TechnicianGUID, "ToolCheckoutUDF1": strpullobj.ToolCheckoutUDF1, "ToolCheckoutUDF2": strpullobj.ToolCheckoutUDF2,
            "ToolCheckoutUDF3": strpullobj.ToolCheckoutUDF3, "ToolCheckoutUDF4": strpullobj.ToolCheckoutUDF4, "ToolCheckoutUDF5": strpullobj.ToolCheckoutUDF5
        };

        $('#DivLoading').show();
        var ajaxURL = '/Pull/RequisitionToolCheckout';
        $.ajax({
            "url": ajaxURL,
            "data": JSON.stringify(reqToolCheckout),
            "type": 'POST',
            "dataType": "json",
            "contentType": "application/json",
            "success": function (response) {
                $('#DivLoading').hide();
                if (response.Status == "ok") {
                    $('#divItem_00000000-0000-0000-0000-000000000000_' + vRequisitionDetailGUID).attr('style', '');
                    $('#divItem_00000000-0000-0000-0000-000000000000_' + vRequisitionDetailGUID).remove();
                    $("#spanGlobalMessage").html("Sucess");
                    $('div#target').fadeToggle();
                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                    $("#spanGlobalMessage").removeClass('WarningIcon errorIcon').addClass('succesIcon');

                }
                else {
                    $("#spanGlobalMessage").html(response.Message);
                    $('div#target').fadeToggle();
                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                }
            },
            "error": function (xhr) {
                $('#DivLoading').hide();
                $("#spanGlobalMessage").html(xhr.status + " " + xhr.statusText);
                $('div#target').fadeToggle();
                $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
            }
        });

    });
    _Common.setGloblaSearch('ItemModel_filter', 'ItemModeDataTable');
});


function JSGetCommonUDFValue(udfName) {
    var udfVal = '';
    if ($("#" + udfName) != null) {
        if ($("#" + udfName).attr("class") == 'selectBox') {
            udfVal = $("#" + udfName + " option:selected").text();
        }
        else {
            udfVal = $("#" + udfName).val();
        }
    }
    return udfVal;
}


function JSGetCommonUDFValueFromTR(udfName, tr) {
    var vUDFValue = '';
    if ($(tr).find('#' + udfName).length > 0) {
        if ($(tr).find('#' + udfName).attr("class") == 'selectBox') {
            vUDFValue = $(tr).find('#' + udfName + ' option:selected').text();
        }
        else {
            vUDFValue = $(tr).find('#' + udfName).val();
        }
    }
    return vUDFValue;
}

