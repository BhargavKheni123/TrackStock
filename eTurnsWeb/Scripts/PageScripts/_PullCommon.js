var PullPageName = "";
function OpenPullPopup(btnObj) {
    
    var errorMsg = '';
    $('#DivLoading').show();
    var PullInfo = new Array();
    if ($(btnObj).prop("id") == "btnPullAllNewFlow") {
        $('#ItemModeDataTable').find("tbody").find("tr.row_selected").each(function (index, tr) {
            var aPos = $('#ItemModeDataTable').dataTable().fnGetPosition($(tr)[0]);
            var aData = $('#ItemModeDataTable').dataTable().fnGetData(aPos);
            errorMsg = '';
            var txxt = $(tr).find('#txtQty');
            var vBinID;
            var vProjectID;
            var vProjectSpendName = '';
            var itemType = $(tr).find('#spnOrderItemType').text();
            var txtQty = txxt.val();
            if (itemType != '4') {
                vBinID = $(tr).find('#BinID')[0].value == '' ? 0 : $(tr).find('#BinID')[0].value;

                if ($("#chkUsePullCommonUDF").is(":checked")) {
                    if ($('#ProjectIDCommon') != undefined)
                        vProjectID = $('#ProjectIDCommon').val() == "" ? "" : $('#ProjectIDCommon').val();
                    else
                        vProjectID = "";
                }
                else {
                    if ($(tr).find('#ProjectID')[0] == undefined) {
                        vProjectID = "";
                    }
                    else {
                        vProjectID = $(tr).find('#ProjectID')[0].value == "" ? "" : $(tr).find('#ProjectID')[0].value;
                    }
                }
                
                if ($("#chkUsePullCommonUDF").is(":checked")) {
                    if ($('#txtProjectSpentCommon') != undefined)
                        vProjectSpendName = $('#txtProjectSpentCommon').val() == "" ? "" : $('#txtProjectSpentCommon').val();
                    else
                        vProjectSpendName = "";
                }
                else {
                    if ($(tr).find('#txtProjectSpent')[0] == undefined) {
                        vProjectSpendName = "";
                    }
                    else {
                        vProjectSpendName = $(tr).find('#txtProjectSpent')[0].value == "" ? "" : $(tr).find('#txtProjectSpent')[0].value;
                    }
                }

                if (!(!isNaN(parseFloat(txtQty)) && parseFloat(txtQty) > 0)) {
                    $(tr).css('background-color', 'Olive');
                    IsGlobalErrorMsgShow = true;
                    errorMsg += "<b style='color:Olive;'>" + aData.ItemNumber + ": Qty to Pull is Mandatory.</b><br/>"
                }

                if (!(!isNaN(parseInt(vBinID)) && parseInt(vBinID) > 0)) {
                    $(tr).css('background-color', 'Olive');
                    IsGlobalErrorMsgShow = true;
                    errorMsg += "<b style='color:Olive;'>" + aData.ItemNumber + ": Inventory Location are Mandatory.</b><br/>"
                }


            }
            else {

                if (!(!isNaN(parseFloat(txtQty)) && parseFloat(txtQty) > 0)) {
                    $(tr).css('background-color', 'Olive');
                    IsGlobalErrorMsgShow = true;
                    errorMsg += "<b style='color:Olive;'>" + aData.ItemNumber + ": Labour Item Required Hours to Pull.</b><br/>"
                }
                vBinID = 0;
                vProjectID = '';
                vProjectSpendName = '';
            }

            if (errorMsg.length <= 0) {
                var vItemID = $(tr).find('#spnItemID').text();
                var vItemGUID = $(tr).find('#spnItemGUID').text();
                var vspnOn_HandQuantity = $(tr).find('#spnOn_HandQuantity').text() == "" ? 0 : $(tr).find('#spnOn_HandQuantity').text();
                var vPullCreditText = "pull"; //$(obj)[0].value;//$(obj).parent().parent().find('input[name=colors'+vItemID+']:checked')[0].value;
                var VspnDefaultPullQuantity = $(this).find('#spnDefaultPullQuantity').text() == "" ? 0 : $(tr).find('#spnDefaultPullQuantity').text();
                var vUDF1 = ''; var vUDF2 = ''; var vUDF3 = ''; var vUDF4 = ''; var vUDF5 = '';
                var vUDF1PullCommon = ''; var vUDF2PullCommon = ''; var vUDF3PullCommon = ''; var vUDF4PullCommon = ''; var vUDF5PullCommon = '';
                var vPullOrderNumber = "";

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
                    if ($(tr).find('#txtPullOrderNumber') != null) {
                        if ($(tr).find('#txtPullOrderNumber').attr("class") == 'selectBox') {
                            vPullOrderNumber = $(tr).find('#txtPullOrderNumber option:selected').text();
                        }
                        else {
                            vPullOrderNumber = $(tr).find('#txtPullOrderNumber').val();
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

                if ($(tr).find('#UDF1') != null) {
                    if ($(tr).find('#UDF1').attr("class") == 'selectBox') {
                        vUDF1 = $(tr).find('#UDF1 option:selected').text();
                    }
                    else {
                        vUDF1 = $(tr).find('#UDF1').val();
                    }
                }

                if ($(tr).find('#UDF2') != null) {
                    if ($(tr).find('#UDF2').attr("class") == 'selectBox') {
                        vUDF2 = $(tr).find('#UDF2 option:selected').text();
                    }
                    else {
                        vUDF2 = $(tr).find('#UDF2').val();
                    }
                }

                if ($(tr).find('#UDF3') != null) {
                    if ($(tr).find('#UDF3').attr("class") == 'selectBox') {
                        vUDF3 = $(tr).find('#UDF3 option:selected').text();
                    }
                    else {
                        vUDF3 = $(tr).find('#UDF3').val();
                    }
                }

                if ($(tr).find('#UDF4') != null) {
                    if ($(tr).find('#UDF4').attr("class") == 'selectBox') {
                        vUDF4 = $(tr).find('#UDF4 option:selected').text();
                    }
                    else {
                        vUDF4 = $(tr).find('#UDF4').val();
                    }
                }

                if ($(tr).find('#UDF5') != null) {
                    if ($(tr).find('#UDF5').attr("class") == 'selectBox') {
                        vUDF5 = $(tr).find('#UDF5 option:selected').text();
                    }
                    else {
                        vUDF5 = $(tr).find('#UDF5').val();
                    }
                }

                if ($("#chkUsePullCommonUDF").is(":checked")) {
                    vUDF1 = vUDF1PullCommon;
                    vUDF2 = vUDF2PullCommon;
                    vUDF3 = vUDF3PullCommon;
                    vUDF4 = vUDF4PullCommon;
                    vUDF5 = vUDF5PullCommon;
                }

                PullInfo.push({ ID: index, ItemID: vItemID, PoolQuantity: txtQty, ItemGUID: vItemGUID, BinID: vBinID, DefaultPullQuantity: VspnDefaultPullQuantity, ProjectSpendGUID: vProjectID, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5, WorkOrderDetailGUID: vWorkOrderDetailGUID, PullOrderNumber: vPullOrderNumber });
            }
        });
    }
    else if ($(btnObj).prop("id") == "btnAdd") {
        
        var tr = $(btnObj).parent().parent()[0];
        var aPos = $('#ItemModeDataTable').dataTable().fnGetPosition($(tr)[0]);
        var aData = $('#ItemModeDataTable').dataTable().fnGetData(aPos);

        var txxt = $(tr).find('#txtQty');
        var vBinID;
        var vProjectID;
        var vProjectSpendName = '';
        var itemType = $(tr).find('#spnOrderItemType').text();
        var txtQty = txxt.val();
        if (itemType != '4') {

            vBinID = $(tr).find('#BinID')[0].value == '' ? 0 : $(tr).find('#BinID')[0].value;

            if ($("#chkUsePullCommonUDF").is(":checked")) {
                if ($('#ProjectIDCommon') != undefined)
                    vProjectID = $('#ProjectIDCommon').val() == "" ? "" : $('#ProjectIDCommon').val();
                else
                    vProjectID = "";
            }
            else {
                if ($(tr).find('#ProjectID')[0] != undefined) {
                    vProjectID = $(tr).find('#ProjectID')[0].value == "" ? "" : $(tr).find('#ProjectID')[0].value;
                }
                else
                    vProjectID = "";
            }

            if ($("#chkUsePullCommonUDF").is(":checked")) {
                if ($('#txtProjectSpentCommon') != undefined)
                    vProjectSpendName = $('#txtProjectSpentCommon').val() == "" ? "" : $('#txtProjectSpentCommon').val();
                else
                    vProjectSpendName = "";
            }
            else {
                if ($(tr).find('#txtProjectSpent')[0] == undefined) {
                    vProjectSpendName = "";
                }
                else {
                    vProjectSpendName = $(tr).find('#txtProjectSpent')[0].value == "" ? "" : $(tr).find('#txtProjectSpent')[0].value;
                }
            }

            if (!(!isNaN(parseFloat(txtQty)) && parseFloat(txtQty) > 0)) {
                $(tr).css('background-color', 'Olive');
                IsGlobalErrorMsgShow = true;
                errorMsg += "<b style='color:Olive;'>" + aData.ItemNumber + ": Qty to Pull is Mandatory.</b><br/>"
            }

            if (!(!isNaN(parseInt(vBinID)) && parseInt(vBinID) > 0)) {
                $(tr).css('background-color', 'Olive');
                IsGlobalErrorMsgShow = true;
                errorMsg += "<b style='color:Olive;'>" + aData.ItemNumber + ": Inventory Location are Mandatory.</b><br/>"
            }
        }
        else {

            if (!(!isNaN(parseFloat(txtQty)) && parseFloat(txtQty) > 0)) {
                $(tr).css('background-color', 'Olive');
                IsGlobalErrorMsgShow = true;
                errorMsg += "<b style='color:Olive;'>" + aData.ItemNumber + ": Labour Item Required Hours to Pull.</b><br/>"
            }
            vBinID = 0;
            vProjectID = '';
            vProjectSpendName = '';
        }

        if (errorMsg.length <= 0) {
            var vItemID = $(tr).find('#spnItemID').text();
            var vItemGUID = $(tr).find('#spnItemGUID').text();
            var vspnOn_HandQuantity = $(tr).find('#spnOn_HandQuantity').text() == "" ? 0 : $(tr).find('#spnOn_HandQuantity').text();
            var vPullCreditText = "pull"; //$(obj)[0].value;//$(obj).parent().parent().find('input[name=colors'+vItemID+']:checked')[0].value;
            var VspnDefaultPullQuantity = $(this).find('#spnDefaultPullQuantity').text() == "" ? 0 : $(tr).find('#spnDefaultPullQuantity').text();
            var vUDF1 = ''; var vUDF2 = ''; var vUDF3 = ''; var vUDF4 = ''; var vUDF5 = '';
            var vUDF1PullCommon = ''; var vUDF2PullCommon = ''; var vUDF3PullCommon = ''; var vUDF4PullCommon = ''; var vUDF5PullCommon = '';
            var vPullOrderNumber = "";

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
                if ($(tr).find('#txtPullOrderNumber') != null) {
                    if ($(tr).find('#txtPullOrderNumber').attr("class") == 'selectBox') {
                        vPullOrderNumber = $(tr).find('#txtPullOrderNumber option:selected').text();
                    }
                    else {
                        vPullOrderNumber = $(tr).find('#txtPullOrderNumber').val();
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

            if ($(tr).find('#UDF1') != null) {
                if ($(tr).find('#UDF1').attr("class") == 'selectBox') {
                    vUDF1 = $(tr).find('#UDF1 option:selected').text();
                }
                else {
                    vUDF1 = $(tr).find('#UDF1').val();
                }
            }

            if ($(tr).find('#UDF2') != null) {
                if ($(tr).find('#UDF2').attr("class") == 'selectBox') {
                    vUDF2 = $(tr).find('#UDF2 option:selected').text();
                }
                else {
                    vUDF2 = $(tr).find('#UDF2').val();
                }
            }

            if ($(tr).find('#UDF3') != null) {
                if ($(tr).find('#UDF3').attr("class") == 'selectBox') {
                    vUDF3 = $(tr).find('#UDF3 option:selected').text();
                }
                else {
                    vUDF3 = $(tr).find('#UDF3').val();
                }
            }

            if ($(tr).find('#UDF4') != null) {
                if ($(tr).find('#UDF4').attr("class") == 'selectBox') {
                    vUDF4 = $(tr).find('#UDF4 option:selected').text();
                }
                else {
                    vUDF4 = $(tr).find('#UDF4').val();
                }
            }

            if ($(tr).find('#UDF5') != null) {
                if ($(tr).find('#UDF5').attr("class") == 'selectBox') {
                    vUDF5 = $(tr).find('#UDF5 option:selected').text();
                }
                else {
                    vUDF5 = $(tr).find('#UDF5').val();
                }
            }

            if ($("#chkUsePullCommonUDF").is(":checked")) {
                vUDF1 = vUDF1PullCommon;
                vUDF2 = vUDF2PullCommon;
                vUDF3 = vUDF3PullCommon;
                vUDF4 = vUDF4PullCommon;
                vUDF5 = vUDF5PullCommon;
            }

            PullInfo.push({ ID: 0, ItemID: vItemID, PoolQuantity: txtQty, ItemGUID: vItemGUID, BinID: vBinID, DefaultPullQuantity: VspnDefaultPullQuantity, ProjectSpendGUID: vProjectID, ProjectSpendName: vProjectSpendName, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5, WorkOrderDetailGUID: vWorkOrderDetailGUID, PullOrderNumber: vPullOrderNumber });
        }
    }
    
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
            },
            error: function (err) {
                alert(err);
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
    }
    return false;
}

function OpenPullPopupFromRequisition(btnObj, WorkOrderId) {
    
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
            $('#dlgCommonErrorMsg').find("#pErrMessage").html('Please select record');
            $('#dlgCommonErrorMsg').modal({
                escClose: false,
                close: false
            });
            return false;
        }

        var HasSerialLotItem = false;
        $('[id^="RequisitionItemsTable"]').find("tbody").find("tr.row_selected").each(function (index, tr) {
            var SerialNumberTracking = $(tr).find('#hdnSerialNumberTracking').val();
            var LotNumberTracking = $(tr).find('#hdnLotNumberTracking').val();
            if (SerialNumberTracking == 'True' || SerialNumberTracking == 'true' || LotNumberTracking == 'True' || LotNumberTracking == 'true') {
                HasSerialLotItem = true;
            }
        });

        if (HasSerialLotItem == false) {
            $('#GlobalModalProcessing').modal();
            setTimeout('AddSingleItemToPullList($("#btnPullAll"))', 1000);
        }
        else {
            $('[id^="RequisitionItemsTable"]').find("tbody").find("tr.row_selected").each(function (index, tr) {                
                var _hdnIsCloseItem = $(tr).find('#hdnIsCloseItem').val();
                if (_hdnIsCloseItem != "True") {
                    var ItemNumber = $(tr).find('#hdnItemNumber').val();

                    errorMsg = '';
                    var _RequisitionDetailGUID = $(tr).find('#hdnRequisitionDetailGUID').val();
                    var txxt = $(tr).find('#txtQty');
                    var vBinID;
                    var vProjectID;
                    var itemType = $(tr).find('#spnOrderItemType').text();
                    var txtQty = txxt.val();
                    if (itemType != '4') {
                        vBinID = $(tr).find('#item_BinID')[0].value == '' ? 0 : $(tr).find('#item_BinID')[0].value;

                        if ($("#chkUsePullCommonUDF").is(":checked")) {
                            if ($('#ProjectSpendGUIDCommon') != undefined)
                                vProjectID = $('#ProjectSpendGUIDCommon').val() == "" ? "" : $('#ProjectSpendGUIDCommon').val();
                            else
                                vProjectID = "";
                        }
                        else {
                            if ($(tr).find('#ProjectSpendGUID')[0] == undefined) {
                                vProjectID = "";
                            }
                            else {
                                vProjectID = $(tr).find('#ProjectSpendGUID')[0].value == "" ? "" : $(tr).find('#ProjectSpendGUID')[0].value;
                            }
                        }

                        if (!(!isNaN(parseFloat(txtQty)) && parseFloat(txtQty) > 0)) {
                            $(tr).css('background-color', 'Olive');
                            IsGlobalErrorMsgShow = true;
                            errorMsg += "<b style='color:Olive;'>" + ItemNumber + ": Qty to Pull is Mandatory.</b><br/>"
                        }

                        if (!(!isNaN(parseInt(vBinID)) && parseInt(vBinID) > 0)) {
                            $(tr).css('background-color', 'Olive');
                            IsGlobalErrorMsgShow = true;
                            errorMsg += "<b style='color:Olive;'>" + ItemNumber + ": Inventory Location are Mandatory.</b><br/>"
                        }


                    }
                    else {

                        if (!(!isNaN(parseFloat(txtQty)) && parseFloat(txtQty) > 0)) {
                            $(tr).css('background-color', 'Olive');
                            IsGlobalErrorMsgShow = true;
                            errorMsg += "<b style='color:Olive;'>" + ItemNumber + ": Labour Item Required Hours to Pull.</b><br/>"
                        }
                        vBinID = 0;
                        vProjectID = '';
                    }

                    if (errorMsg.length <= 0) {
                        var vItemID = $(tr).find('#hdnItemID').val();
                        var vItemGUID = $(tr).find('#hdnItemGUID').val();
                        var vspnOn_HandQuantity = $(tr).find('#hdnHandQuantity').val() == "" ? 0 : $(tr).find('#hdnHandQuantity').val();
                        var vPullCreditText = "pull"; //$(obj)[0].value;//$(obj).parent().parent().find('input[name=colors'+vItemID+']:checked')[0].value;
                        var VspnDefaultPullQuantity = $(this).find('#hdnDefaultPullQuantity').val() == "" ? 0 : $(tr).find('#hdnDefaultPullQuantity').val();
                        var vUDF1 = ''; var vUDF2 = ''; var vUDF3 = ''; var vUDF4 = ''; var vUDF5 = '';
                        var vUDF1PullCommon = ''; var vUDF2PullCommon = ''; var vUDF3PullCommon = ''; var vUDF4PullCommon = ''; var vUDF5PullCommon = '';
                        var vPullOrderNumber = "";

                        //if ($("#chkUsePullCommonUDF").is(":checked")) {
                        //    if ($('#txtPullOrderNumberCommon') != null) {
                        //        if ($('#txtPullOrderNumberCommon').attr("class") == 'selectBox') {
                        //            vPullOrderNumber = $('#txtPullOrderNumberCommon option:selected').text();
                        //        }
                        //        else {
                        //            vPullOrderNumber = $('#txtPullOrderNumberCommon').val();
                        //        }
                        //    }
                        //}
                        //else {
                        //    if ($(tr).find('#txtPullOrderNumber') != null) {
                        //        if ($(tr).find('#txtPullOrderNumber').attr("class") == 'selectBox') {
                        //            vPullOrderNumber = $(tr).find('#txtPullOrderNumber option:selected').text();
                        //        }
                        //        else {
                        //            vPullOrderNumber = $(tr).find('#txtPullOrderNumber').val();
                        //        }
                        //    }
                        //}

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

                        if ($(tr).find('#UDF1') != null) {
                            if ($(tr).find('#UDF1').attr("class") == 'selectBox') {
                                vUDF1 = $(tr).find('#UDF1 option:selected').text();
                            }
                            else {
                                vUDF1 = $(tr).find('#UDF1').val();
                            }
                        }

                        if ($(tr).find('#UDF2') != null) {
                            if ($(tr).find('#UDF2').attr("class") == 'selectBox') {
                                vUDF2 = $(tr).find('#UDF2 option:selected').text();
                            }
                            else {
                                vUDF2 = $(tr).find('#UDF2').val();
                            }
                        }

                        if ($(tr).find('#UDF3') != null) {
                            if ($(tr).find('#UDF3').attr("class") == 'selectBox') {
                                vUDF3 = $(tr).find('#UDF3 option:selected').text();
                            }
                            else {
                                vUDF3 = $(tr).find('#UDF3').val();
                            }
                        }

                        if ($(tr).find('#UDF4') != null) {
                            if ($(tr).find('#UDF4').attr("class") == 'selectBox') {
                                vUDF4 = $(tr).find('#UDF4 option:selected').text();
                            }
                            else {
                                vUDF4 = $(tr).find('#UDF4').val();
                            }
                        }

                        if ($(tr).find('#UDF5') != null) {
                            if ($(tr).find('#UDF5').attr("class") == 'selectBox') {
                                vUDF5 = $(tr).find('#UDF5 option:selected').text();
                            }
                            else {
                                vUDF5 = $(tr).find('#UDF5').val();
                            }
                        }

                        if ($("#chkUsePullCommonUDF").is(":checked")) {
                            vUDF1 = vUDF1PullCommon;
                            vUDF2 = vUDF2PullCommon;
                            vUDF3 = vUDF3PullCommon;
                            vUDF4 = vUDF4PullCommon;
                            vUDF5 = vUDF5PullCommon;
                        }
                        PullInfo.push({ ID: index, ItemID: vItemID, PoolQuantity: txtQty, ItemGUID: vItemGUID, BinID: vBinID, DefaultPullQuantity: VspnDefaultPullQuantity, ProjectSpendGUID: vProjectID, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5, WorkOrderDetailGUID: WorkOrderId, PullOrderNumber: vPullOrderNumber, RequisitionDetailGUID: _RequisitionDetailGUID });
                    }
                }
            });
        }
    }
    else if ($(btnObj).prop("id") == "btnAdd") {

        var tr = $(btnObj).parent().parent()[0];
        var ItemNumber = $(tr).find('#hdnItemNumber').val();

        var _RequisitionDetailGUID = $(tr).find('#hdnRequisitionDetailGUID').val();
        var txxt = $(tr).find('#txtQty');
        var vBinID;
        var vProjectID;
        var itemType = $(tr).find('#spnOrderItemType').text();
        var txtQty = txxt.val();
        if (itemType != '4') {

            vBinID = $(tr).find('#item_BinID')[0].value == '' ? 0 : $(tr).find('#item_BinID')[0].value;

            if ($("#chkUsePullCommonUDF").is(":checked")) {
                if ($('#ProjectSpendGUIDCommon') != undefined)
                    vProjectID = $('#ProjectSpendGUIDCommon').val() == "" ? "" : $('#ProjectSpendGUIDCommon').val();
                else
                    vProjectID = "";
            }
            else {
                if ($(tr).find('#ProjectSpendGUID')[0] != undefined) {
                    vProjectID = $(tr).find('#ProjectSpendGUID')[0].value == "" ? "" : $(tr).find('#ProjectSpendGUID')[0].value;
                }
                else
                    vProjectID = "";
            }

            if (!(!isNaN(parseFloat(txtQty)) && parseFloat(txtQty) > 0)) {
                $(tr).css('background-color', 'Olive');
                IsGlobalErrorMsgShow = true;
                errorMsg += "<b style='color:Olive;'>" + ItemNumber + ": Qty to Pull is Mandatory.</b><br/>"
            }

            if (!(!isNaN(parseInt(vBinID)) && parseInt(vBinID) > 0)) {
                $(tr).css('background-color', 'Olive');
                IsGlobalErrorMsgShow = true;
                errorMsg += "<b style='color:Olive;'>" + ItemNumber + ": Inventory Location are Mandatory.</b><br/>"
            }
        }
        else {

            if (!(!isNaN(parseFloat(txtQty)) && parseFloat(txtQty) > 0)) {
                $(tr).css('background-color', 'Olive');
                IsGlobalErrorMsgShow = true;
                errorMsg += "<b style='color:Olive;'>" + ItemNumber + ": Labour Item Required Hours to Pull.</b><br/>"
            }
            vBinID = 0;
            vProjectID = '';
        }

        if (errorMsg.length <= 0) {
            var vItemID = $(tr).find('#hdnItemID').val();
            var vItemGUID = $(tr).find('#hdnItemGUID').val();
            var vspnOn_HandQuantity = $(tr).find('#hdnHandQuantity').val() == "" ? 0 : $(tr).find('#hdnHandQuantity').val();
            var vPullCreditText = "pull"; //$(obj)[0].value;//$(obj).parent().parent().find('input[name=colors'+vItemID+']:checked')[0].value;
            var VspnDefaultPullQuantity = $(this).find('#hdnDefaultPullQuantity').val() == "" ? 0 : $(tr).find('#hdnDefaultPullQuantity').val();
            var vUDF1 = ''; var vUDF2 = ''; var vUDF3 = ''; var vUDF4 = ''; var vUDF5 = '';
            var vUDF1PullCommon = ''; var vUDF2PullCommon = ''; var vUDF3PullCommon = ''; var vUDF4PullCommon = ''; var vUDF5PullCommon = '';
            var vPullOrderNumber = "";

            //if ($("#chkUsePullCommonUDF").is(":checked")) {
            //    if ($('#txtPullOrderNumberCommon') != null) {
            //        if ($('#txtPullOrderNumberCommon').attr("class") == 'selectBox') {
            //            vPullOrderNumber = $('#txtPullOrderNumberCommon option:selected').text();
            //        }
            //        else {
            //            vPullOrderNumber = $('#txtPullOrderNumberCommon').val();
            //        }
            //    }
            //}
            //else {
            //    if ($(tr).find('#txtPullOrderNumber') != null) {
            //        if ($(tr).find('#txtPullOrderNumber').attr("class") == 'selectBox') {
            //            vPullOrderNumber = $(tr).find('#txtPullOrderNumber option:selected').text();
            //        }
            //        else {
            //            vPullOrderNumber = $(tr).find('#txtPullOrderNumber').val();
            //        }
            //    }
            //}

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

            if ($(tr).find('#UDF1') != null) {
                if ($(tr).find('#UDF1').attr("class") == 'selectBox') {
                    vUDF1 = $(tr).find('#UDF1 option:selected').text();
                }
                else {
                    vUDF1 = $(tr).find('#UDF1').val();
                }
            }

            if ($(tr).find('#UDF2') != null) {
                if ($(tr).find('#UDF2').attr("class") == 'selectBox') {
                    vUDF2 = $(tr).find('#UDF2 option:selected').text();
                }
                else {
                    vUDF2 = $(tr).find('#UDF2').val();
                }
            }

            if ($(tr).find('#UDF3') != null) {
                if ($(tr).find('#UDF3').attr("class") == 'selectBox') {
                    vUDF3 = $(tr).find('#UDF3 option:selected').text();
                }
                else {
                    vUDF3 = $(tr).find('#UDF3').val();
                }
            }

            if ($(tr).find('#UDF4') != null) {
                if ($(tr).find('#UDF4').attr("class") == 'selectBox') {
                    vUDF4 = $(tr).find('#UDF4 option:selected').text();
                }
                else {
                    vUDF4 = $(tr).find('#UDF4').val();
                }
            }

            if ($(tr).find('#UDF5') != null) {
                if ($(tr).find('#UDF5').attr("class") == 'selectBox') {
                    vUDF5 = $(tr).find('#UDF5 option:selected').text();
                }
                else {
                    vUDF5 = $(tr).find('#UDF5').val();
                }
            }

            if ($("#chkUsePullCommonUDF").is(":checked")) {
                vUDF1 = vUDF1PullCommon;
                vUDF2 = vUDF2PullCommon;
                vUDF3 = vUDF3PullCommon;
                vUDF4 = vUDF4PullCommon;
                vUDF5 = vUDF5PullCommon;
            }
            PullInfo.push({ ID: 0, ItemID: vItemID, PoolQuantity: txtQty, ItemGUID: vItemGUID, BinID: vBinID, DefaultPullQuantity: VspnDefaultPullQuantity, ProjectSpendGUID: vProjectID, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5, WorkOrderDetailGUID: WorkOrderId, PullOrderNumber: vPullOrderNumber, RequisitionDetailGUID: _RequisitionDetailGUID });
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
                    alert(err);
                }
            });
        }
        else
        {
            $('#DivLoading').hide();
            $('#dlgCommonErrorMsg').find("#pOkbtn").css('display', '');
            $('#dlgCommonErrorMsg').find("#pYesNobtn").css('display', 'none');
            $('#dlgCommonErrorMsg').find("#pErrMessage").html('Please select record');
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
                strReturn = strReturn + "<input type='text' value='" + obj.aData.LotOrSerailNumber + "' id='txtLotOrSerailNumber' name='txtLotOrSerailNumber' class='text-boxinner AutoSerialLot' />";
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
    columnarrIL.push({ mDataProp: "BinNumber", sClass: "read_only" });
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
            strReturn = strReturn + "<input type='hidden' name='hdnBinNumber' value='" + obj.aData.BinNumber + "' />";

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
            aoData.push({ "name": "BinID", "value": objPullItemDTO.BinID });
            aoData.push({ "name": "PullQuantity", "value": FormatedCostQtyValues($("#txtPoolQuantity_" + objPullItemDTO.ItemGUID + "_" + objPullItemDTO.RequisitionDetailGUID).val(), 2) });
            aoData.push({ "name": "InventoryConsuptionMethod", "value": objPullItemDTO.InventoryConsuptionMethod });
            aoData.push({ "name": "CurrentLoaded", "value": $("#hdnCurrentLoadedIds_" + objPullItemDTO.ItemGUID + "_" + objPullItemDTO.RequisitionDetailGUID).val() });
            aoData.push({ "name": "ViewRight", "value": objPullItemDTO.ViewRight });
            aoData.push({ "name": "IsDeleteRowMode", "value": isDeleteSrLotRow });
            oSettings.jqXHR = $.ajax({
                dataType: 'json',
                type: "POST",
                url: sSource,
                cache: false,
                data: aoData,
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
                        alert("No location found to add.");
                    }
                    IsLoadMoreLotsClicked = false;
                }
            });
        }
    });
}

function PullAllNewFlow() {
    var ArrItem = new Array();
    var arrItemDetails;
    var ErrorMessage = ValidateAllPull();

    if (ErrorMessage == "") {
        $("#DivPullSelection").find("table[id^='tblItemPullheader']").each(function (indx, tblHeader) {
            arrItemDetails = new Array();
            var ID = $(tblHeader).prop("id").split('_')[1];
            var RequisitionDetailGUID = $(tblHeader).prop("id").split('_')[2];
            var SpanQty = $(tblHeader).find("#txtPoolQuantity_" + ID + "_" + RequisitionDetailGUID);
            
            var dt = $("#tblItemPull_" + ID + "_" + RequisitionDetailGUID).dataTable();
            var currentData = dt.fnGetData();

            var strpullobj = JSON.parse($(tblHeader).find("input[name='hdnPullMasterDTO']").val());
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

            $("#tblItemPull_" + ID + "_" + RequisitionDetailGUID).find("tbody").find("tr").each(function (index, tr) {
                var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();
                var hdnLotNumberTracking = $(tr).find("input[name='hdnLotNumberTracking']").val();
                var hdnSerialNumberTracking = $(tr).find("input[name='hdnSerialNumberTracking']").val();
                var hdnDateCodeTracking = $(tr).find("input[name='hdnDateCodeTracking']").val();
                var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();
                var hdnBinNumber = $(tr).find("input[name='hdnBinNumber']").val();
                var hdnExpiration = $(tr).find("input[name='hdnExpiration']").val();

                if (txtPullQty != "") {
                    var txtLotOrSerailNumber = "";
                    if (hdnLotNumberTracking == "true" || hdnSerialNumberTracking == "true")
                        var txtLotOrSerailNumber = $(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val();

                    var vSerialNumber = "";
                    var vLotNumber = "";
                    var vExpiration = "";

                    if (hdnSerialNumberTracking == "true")
                        vSerialNumber = txtLotOrSerailNumber;
                    if (hdnLotNumberTracking == "true")
                        vLotNumber = txtLotOrSerailNumber;
                    if (hdnDateCodeTracking == "true")
                        vExpiration = hdnExpiration;

                    var obj = { "LotOrSerailNumber": txtLotOrSerailNumber, "BinNumber": hdnBinNumber, "PullQuantity": parseFloat(txtPullQty.toString())
                                    , "LotNumberTracking": hdnLotNumberTracking, "SerialNumberTracking": hdnSerialNumberTracking, "DateCodeTracking": hdnDateCodeTracking
                                    , "Expiration": hdnExpiration, "SerialNumber": vSerialNumber, "LotNumber": vLotNumber
                                    , "ItemGUID": strpullobj.ItemGUID, "BinID": strpullobj.BinID, "ID": strpullobj.BinID
                    };

                    arrItemDetails.push(obj);
                }
            });

            var pullQty = parseFloat($(SpanQty).val().toString());
           
            var PullItem = {
                ID: indx,
                ItemGUID: strpullobj.ItemGUID,
                ProjectSpendGUID: strpullobj.ProjectSpendGUID,
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
                PullOrderNumber: strpullobj.PullOrderNumber,
                lstItemPullDetails: arrItemDetails,
                WorkOrderDetailGUID: strpullobj.WorkOrderDetailGUID,
                RequisitionDetailsGUID: strpullobj.RequisitionDetailGUID
            };
            ArrItem.push(PullItem);
        });

        if (ArrItem.length > 0) {
            PullMultipleItemNew(ArrItem);
        }
    }
    else {
        alert(ErrorMessage);
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
            alert(err);
        },
        complete: function () {
            if ((indx + 1) < ArrItem.length) {
                PullSingleItem((indx + 1), ArrItem);
            }
        }
    });
}

function PullMultipleItemNew(ArrItem) {
    $('#DivLoading').show();
    $.ajax({
        type: "POST",
        url: PullSerialsAndLotsNewUrl,
        contentType: 'application/json',
        dataType: 'json',
        data: JSON.stringify(ArrItem),
        success: function (RetData) {
            
            var errorMessage = "";
            var _RequisitionDetailGUID = "";
            $.each(RetData, function (indx, RetDataItem) {
                
                if (RetDataItem.ErrorMessage != null && RetDataItem.ErrorMessage != undefined && RetDataItem.ErrorMessage.trim() > '') {
                    errorMessage += RetDataItem.ErrorMessage + "<br />";
                }
                else if (RetDataItem.ErrorList.length > 0) {
                    $.each(RetDataItem.ErrorList, function (indx, ErrorListItem) {
                        errorMessage += ErrorListItem.ErrorMessage + "<br />";
                    });
                }
                else {
                    if (RetDataItem.RequisitionDetailsGUID != null && RetDataItem.RequisitionDetailsGUID != undefined && RetDataItem.RequisitionDetailsGUID.trim() != '') {
                        $('#divItem_' + RetDataItem.ItemGUID + '_' + RetDataItem.RequisitionDetailsGUID).attr('style', '');
                        $('#divItem_' + RetDataItem.ItemGUID + '_' + RetDataItem.RequisitionDetailsGUID).remove();
                    }
                    else {
                        $('#divItem_' + RetDataItem.ItemGUID + '_').attr('style', '');
                        $('#divItem_' + RetDataItem.ItemGUID + '_').remove();
                    }
                }

                _RequisitionDetailGUID = RetDataItem.RequisitionDetailsGUID;
            });

            $('#DivLoading').hide();
            if (errorMessage != "") {
                $.modal.impl.close();
                errorMessage = "<b>Some of the Item(s) is(are) not able to Pull(s) Due to following reasons.</b><br /><br />" + errorMessage;
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
                                $('#dlgCommonErrorMsgPopup').find("#pErrMessage").html("<b>Pull done successfully.</b><br /><br />");
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
                                $("#spanGlobalMessage").html("All pull done successfully");
                                $('div#target').fadeToggle();
                                $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                                $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                                if ($('div[id^="divItem_"]').length <= 0) {
                                    $('#DivPullSelection').dialog('close');
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
                        $('#dlgCommonErrorMsgPopup').find("#pErrMessage").html("<b>Pull done successfully.</b><br /><br />");
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
                        $("#spanGlobalMessage").html("All pull done successfully");
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
            $('#dlgCommonErrorMsgPopup').find("#pErrMessage").html("Not saved, error occured");
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
        var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();
        var spnLotSerialQuantity = $(tr).find("span[name='spnLotSerialQuantity']").text();

        if (parseFloat(txtPullQty) > parseFloat(spnLotSerialQuantity)) {
            errormsg = "\nYou can not pull more QTY than available QTY.";
            isMoreQty = true;
            return errormsg;
        }

        TotalEntered = TotalEntered + parseFloat(txtPullQty);
    });

    if (isMoreQty == false) {
        var pullQty = parseFloat($(SpanQty).val().toString());
        if (TotalEntered != pullQty) {
            errormsg = errormsg + "\n You have entered :" + TotalEntered + " QTY. You had entered Pulled Qty :" + pullQty;
        }
    }
    else {
        errormsg = "You can not pull more QTY than available QTY.";
    }

    return errormsg;
}

function ValidateAllPull() {
    var returnVal = true;
    var errormsg = "";
    var isMoreQty = false;
    $("#DivPullSelection").find("table[id^='tblItemPullheader']").each(function (indx, tblHeader) {
        var ID = $(tblHeader).prop("id").split('_')[1];
        var RequisitionDetailGUID = $(tblHeader).prop("id").split('_')[2];
        var SpanQty = $(tblHeader).find("#txtPoolQuantity_" + ID + "_" + RequisitionDetailGUID);
        
        var TotalEntered = 0;
        $("#tblItemPull_" + ID + "_" + RequisitionDetailGUID).find("tbody").find("tr").each(function (index, tr) {
            var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();
            var spnLotSerialQuantity = $(tr).find("span[name='spnLotSerialQuantity']").text();

            if (parseFloat(txtPullQty) > parseFloat(spnLotSerialQuantity)) {
                errormsg = "\nYou can not pull more QTY than available QTY.";
                isMoreQty = true;
                return errormsg;
            }

            TotalEntered = TotalEntered + parseFloat(txtPullQty);
        });

        if (isMoreQty == false) {
            var pullQty = parseFloat($(SpanQty).val().toString());
            if (TotalEntered != pullQty) {
                ////errormsg = errormsg + "\nentered :" + TotalEntered + "\tPull Qty :" + pullQty;
                errormsg = errormsg + "\n You have entered :" + TotalEntered + " QTY. You had entered Pulled Qty :" + pullQty;
            }
        }
        else {
            errormsg = "You can not pull more QTY than available QTY.";
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
        title: "Pull Details",
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
        $("#tblItemPull_" + ids[1].toString() + "_" + ids[2].toString() + " tbody tr").each(function (i) {
            if (i != row_id) {
                var tr = $(this);
                var SerialOrLotNumber = $(tr).find('#' + objCurtxt.prop("id")).val();
                if (SerialOrLotNumber == $(objCurtxt).val()) {
                    isDuplicateEntry = true;
                }
                else {
                    var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();
                    OtherPullQuantity = OtherPullQuantity + parseFloat(txtPullQty);
                }
            }
        });

        if (isDuplicateEntry == true) {

            if ($("#hdnTrackingType_" + ids[1].toString() + "_" + ids[2].toString()).val() == "LotNumberTracking")
                alert("Duplicate lot number.");
            else if ($("#hdnTrackingType_" + ids[1].toString() + "_" + ids[2].toString()).val() == "SerialNumberTracking")
                alert("Duplicate serial number.");
            else
                alert("Duplicate number.");

            $(objCurtxt).val("");
            $(objCurtxt).focus();
        }
        else {
            $.ajax({
                type: "POST",
                url: ValidateSerialLotNumberUrl,
                contentType: 'application/json',
                dataType: 'json',
                data: "{ ItemGuid: '" + ids[1].toString() + "', SerialOrLotNumber: '" + $(objCurtxt).val() + "',BinID: '" + aData.BinID + "' }",
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
                    alert(err);
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

        $(dtItemPull).find("tbody").find("tr").each(function (index, tr) {

            if (index != aPos) {
                var hdnLotNumberTracking = $(tr).find("input[name='hdnLotNumberTracking']").val();
                var hdnSerialNumberTracking = $(tr).find("input[name='hdnSerialNumberTracking']").val();
                var hdnDateCodeTracking = $(tr).find("input[name='hdnDateCodeTracking']").val();

                if (hdnLotNumberTracking == "true" || hdnSerialNumberTracking == "true") {
                    var txtLotOrSerailNumber = $(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val();
                    if (txtLotOrSerailNumber != undefined)
                        strSerialLotNos = strSerialLotNos + txtLotOrSerailNumber + "|#|";
                }
                else if (hdnDateCodeTracking == "true") {
                    var hdnExpiration = $(tr).find("input[name='hdnExpiration']").val();
                    if (hdnExpiration != undefined)
                        strSerialLotNos = strSerialLotNos + hdnExpiration + "|#|";
                }
                else {
                    var hdnBinNumber = $(tr).find("input[name='hdnBinNumber']").val();
                    if (hdnBinNumber != undefined)
                        strSerialLotNos = strSerialLotNos + hdnBinNumber + "|#|";
                }
            }
            
        });

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
                            prmSerialLotNos: strSerialLotNos
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

                var hdnLotNumberTracking = $(tr).find("input[name='hdnLotNumberTracking']").val();
                var hdnSerialNumberTracking = $(tr).find("input[name='hdnSerialNumberTracking']").val();
                var hdnDateCodeTracking = $(tr).find("input[name='hdnDateCodeTracking']").val();
                var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();

                if (txtPullQty != undefined) {
                    if (txtPullQty == "") {
                        txtPullQty = "0";
                    }
                    if (hdnLotNumberTracking == "true" || hdnSerialNumberTracking == "true") {
                        var txtLotOrSerailNumber = $(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val();
                        if (txtLotOrSerailNumber != undefined)
                            strIds = strIds + txtLotOrSerailNumber + "_" + txtPullQty + ",";
                    }
                    else if (hdnDateCodeTracking == "true") {
                        var hdnExpiration = $(tr).find("input[name='hdnExpiration']").val();
                        if (hdnExpiration != undefined)
                            strIds = strIds + hdnExpiration + "_" + txtPullQty + ",";
                    }
                    else {
                        var hdnBinNumber = $(tr).find("input[name='hdnBinNumber']").val();
                        if (hdnBinNumber != undefined)
                            strIds = strIds + hdnBinNumber + "_" + txtPullQty + ",";
                    }
                }
            });

            $("#hdnCurrentLoadedIds_" + vItemGUID + "_" + vRequisitionDetailGUID).val(strIds);
            
            var dt = $(dtID).dataTable();
            dt.fnStandingRedraw();            
        }
        else
        {
            alert("Can not add new row because total Pull/Credit Quantity is greater then or equal to Pull/Credit Quantity.");
            //$("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            //$("#spanGlobalMessage").html("Can not add new row because total Pull/Credit Quantity is greater then or equal to Pull/Credit Quantity.");
            //$('div#target').fadeToggle();
            //$("div#target").delay(2000).fadeOut(200);
        }
    });

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
                var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();
                var hdnLotNumberTracking = $(tr).find("input[name='hdnLotNumberTracking']").val();
                var hdnSerialNumberTracking = $(tr).find("input[name='hdnSerialNumberTracking']").val();
                var hdnDateCodeTracking = $(tr).find("input[name='hdnDateCodeTracking']").val();
                var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();
                var hdnBinNumber = $(tr).find("input[name='hdnBinNumber']").val();
                var hdnExpiration = $(tr).find("input[name='hdnExpiration']").val();

                if (txtPullQty != "") {
                    var txtLotOrSerailNumber = "";
                    if (hdnLotNumberTracking == "true" || hdnSerialNumberTracking == "true")
                        var txtLotOrSerailNumber = $(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val();

                    var vSerialNumber = "";
                    var vLotNumber = "";
                    var vExpiration = "";

                    if (hdnSerialNumberTracking == "true")
                        vSerialNumber = txtLotOrSerailNumber;
                    if (hdnLotNumberTracking == "true")
                        vLotNumber = txtLotOrSerailNumber;
                    if (hdnDateCodeTracking == "true")
                        vExpiration = hdnExpiration;

                    var obj = { "LotOrSerailNumber": txtLotOrSerailNumber, "BinNumber": hdnBinNumber, "PullQuantity": parseFloat(txtPullQty.toString())
                                    , "LotNumberTracking": hdnLotNumberTracking, "SerialNumberTracking": hdnSerialNumberTracking, "DateCodeTracking": hdnDateCodeTracking
                                    , "Expiration": hdnExpiration, "SerialNumber": vSerialNumber, "LotNumber": vLotNumber
                                    , "ItemGUID": strpullobj.ItemGUID, "BinID": strpullobj.BinID, "ID": strpullobj.BinID
                    };

                    arrItemDetails.push(obj);
                }
            });

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
                RequisitionDetailsGUID: strpullobj.RequisitionDetailGUID
            };

            ArrItem.push(PullItem);

            if (ArrItem.length > 0) {
                PullMultipleItemNew(ArrItem);
            }
        }
        else {
            alert(ErrorMessage);
        }
    });

    $("#DivPullSelection").off("click", "input[type='button'][name='btnPullAllPopUp']");
    $("#DivPullSelection").on("click", "input[type='button'][name='btnPullAllPopUp']", function () {
        PullAllNewFlow();
    });

    $("#DivPullSelection").off("click", "input[type='button'][name='btnCancelPullPopup']");
    $("#DivPullSelection").on("click", "input[type='button'][name='btnCancelPullPopup']", function () {
        $("#DivPullSelection").empty();
        $('#DivPullSelection').dialog('close');
        $('#ItemModeDataTable').dataTable().fnStandingRedraw();
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
            alert("Please enter correct pull quantity");
        }
        else {
            alert("Validated");
        }

    });

    $("#DivPullSelection").off('click', "input[type='button'][name='btnvalidateAll']");
    $("#DivPullSelection").on('click', "input[type='button'][name='btnvalidateAll']", function (e) {
        var returnmsg = ValidateAllPull();
        if (returnmsg == "")
            returnmsg = "Validated All";
        alert(returnmsg);
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
            alert("Select at least one row to delete.");
        }
        else {
            if (RemainingRows >= 1) {
                $(dtID).find("tbody").find("tr.row_selected").each(function (index, tr) {
                    $(tr).remove();
                });

                var strIds = "";
                $(dtID).find("tbody").find("tr").each(function (index, tr) {

                    var hdnLotNumberTracking = $(tr).find("input[name='hdnLotNumberTracking']").val();
                    var hdnSerialNumberTracking = $(tr).find("input[name='hdnSerialNumberTracking']").val();
                    var hdnDateCodeTracking = $(tr).find("input[name='hdnDateCodeTracking']").val();
                    var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();

                    if (txtPullQty == "")
                        txtPullQty = "0";

                    if (hdnLotNumberTracking == "true" || hdnSerialNumberTracking == "true") {
                        var txtLotOrSerailNumber = $(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val();
                        strIds = strIds + txtLotOrSerailNumber + "_" + txtPullQty + ",";
                    }
                    else if (hdnDateCodeTracking == "true") {
                        var hdnExpiration = $(tr).find("input[name='hdnExpiration']").val();
                        strIds = strIds + hdnExpiration + "_" + txtPullQty + ",";
                    }
                    else {
                        var hdnBinNumber = $(tr).find("input[name='hdnBinNumber']").val();
                        strIds = strIds + hdnBinNumber + "_" + txtPullQty + ",";
                    }
                });

                $("#hdnCurrentLoadedIds_" + vItemGUID + "_" + vRequisitionDetailGUID).val(strIds);
                isDeleteSrLotRow = true;
                var dtThisItem = $(dtID).dataTable();
                dtThisItem.fnStandingRedraw();
            }
            else {
                alert("Can not delete row because at least one row should exists.");
                //$("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                //$("#spanGlobalMessage").html("Can not add new row because total Pull/Credit Quantity is greater then or equal to Pull/Credit Quantity.");
                //$('div#target').fadeToggle();
                //$("div#target").delay(2000).fadeOut(200);
            }

        }
    });

});
