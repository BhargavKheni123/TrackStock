var isDeleteSrLotRowEdit = false
var gblPreCreditObjToSaveForCredit = new Array();
var DeletedRowObject = "";

var _PullMasterList = (function ($) {
    var self = {};

    var myDataTableId = 'myDataTable';

    self.isSaveGridState = false;
    //self.isGetReplinshRedCount = true;
    //self.isGetUDFEditableOptionsByUDF = true;

    self.urls = {
        saveGridStateURL: null, loadGridStateUrl: null
        , pullMasterListAjaxUrl: null, getUDFOptionByIDUrl: null
        , getQTYFromLocationAndItemUrl: null, pullDetailsUrl: null
        , newPullUrl: null, pullMasterListUrl: null
        , setBillingForPullMasterUrl: null, updateUDFInPullHistoryUrl: null
        , updatePullOrderNumberInPullHistoryUrl: null
        , UpdatePullQtyInPullHistoryUrl: null
        , PullItemEditQuantityUrl: null
        , PullLotSrSelectionDetailsEditUrl: null
        , ValidateSerialLotNumberForEditUrl: null
        , ValidateLotDateCodeForCreditUrl: null
        , ValidateSerialNumberForCreditUrl: null
        , GetLotOrSerailNumberListForEditUrl: null
        , PullSerialsAndLotsNewEditUrl: null
        , PullLotSrSelectionForCreditEditPopupUrl: null
        , SavePullCreditEditUrl: null
        , GetPullOrderNumberUrl : null
    };

    self.init = function () {

        self.initDataTable();
        self.initEvents();
    };

    self.gridRowStartIndex = null;

    self.initDataTable = function () {


        $(document).ready(function () {
            
            $("#DivPullSelectionEdit").dialog({
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
                    $("#DivPullSelectionEdit").empty();
                    $('#myDataTable').dataTable().fnStandingRedraw();
                }
            });

            var PullColumnsArr = new Array();
            objColumns = GetGridHeaderColumnsObject(myDataTableId);


            PullColumnsArr.push({ "mDataProp": null, "sClass": "read_only center NotHide RowNo", "bSortable": false, "sDefaultContent": '' });
            PullColumnsArr.push({ "mDataProp": null, "bSortable": false, "sClass": "read_only control center", "sDefaultContent": '<img src="' + sImageUrl + 'drildown_open.jpg' + '">' });
            PullColumnsArr.push({
                "mDataProp": null,
                "sClass": "read_only center NotHide",
                "bSortable": false,
                "sDefaultContent": '',
                "fnRender": function (obj, val) {
                    if (obj.aData.ScheduleMode == 6) {


                        if ((obj.aData.Billing == null || obj.aData.Billing == false) && (obj.aData.Consignment == true)) {


                            return "<input type='button' value='" + BtnSendPull + "' id='btnAddSingleBilling' onclick='return AddSingleBillingOrPullOrderNoToPullList(this);' class='CreateBtn' style='float: none;padding: 2px 6px;margin-left: 5px;' /><span id='spnPullGUID' style='display:none'>" + obj.aData.GUID + "</span><span id='spnSupplierID' style='display:none'>" + obj.aData.SupplierID + "</span>";
                        }
                        else {

                            return "<span id='spnPullGUID' style='visibility:hidden;'>" + obj.aData.GUID + "</span><span id='spnSupplierID' style='display:none'>" + obj.aData.SupplierID + "</span>";
                        }
                    }
                    else {

                        return "<span id='spnPullGUID' style='visibility:hidden;'>" + obj.aData.GUID + "</span><span id='spnSupplierID' style='display:none'>" + obj.aData.SupplierID + "</span>";
                    }
                }
            });
            PullColumnsArr.push({
                "mDataProp": null,
                "sClass": "read_only center NotHide",
                "bSortable": false,
                "sDefaultContent": '',
                "fnRender": function (obj, val) {
                    if (obj.aData.ScheduleMode == 6) {
                        if ((obj.aData.Billing == null || obj.aData.Billing == false) && (obj.aData.Consignment == true)) {
                            return "<input type='checkbox' id='chkIsBilling' />";
                        }
                        else {
                            return GetBoolInFormat(obj, obj.aData.Billing);
                        }
                    }
                    else {
                        return GetBoolInFormat(obj, obj.aData.Billing);
                    }
                }
            });
            PullColumnsArr.push(PullPoMasterArray);
            PullColumnsArr.push({ "mDataProp": "PullCredit", "sClass": "read_only center", "sDefaultContent": '', "fnRender": function (obj, val) { return "<span id='spnPullOrCredit'>" + obj.aData.PullCredit + "</span><span id='spnPullMasterID' style='display:none'>" + obj.aData.ID + "</span>" + " <input type='hidden' id='hdnID' value='" + obj.aData.GUID.toString() + "' />" + " <input type='hidden' id='hdnItemGuid' value='" + obj.aData.ItemGUID.toString() + "' />" + " <input type='hidden' id='hdnSerialNumberTracking' value='" + obj.aData.SerialNumberTracking.toString() + "' />" + " <input type='hidden' id='hdnLotNumberTracking' value='" + obj.aData.LotNumberTracking.toString() + "' />" + " <input type='hidden' id='hdnDateCodeTracking' value='" + obj.aData.DateCodeTracking.toString() + "' />" + " <input type='hidden' id='hdnPullQuantity' value='" + obj.aData.PoolQuantity.toString() + "' />"; } });
            PullColumnsArr.push({
                "mDataProp": "PoolQuantity_LabelView",
                "sClass": "read_only numericalign",
                "fnRender": function (obj, val) {                    
                    var hdnAllowedPullQtyEdit = $("#hdnAllowedPullQtyEdit").val();
                    if (hdnAllowedPullQtyEdit != "" && hdnAllowedPullQtyEdit.toLowerCase() == "yes") {
                        if ((obj.aData.requisitiondetailguid == null || obj.aData.requisitiondetailguid == "")
                            && (obj.aData.countlineitemguid == null || obj.aData.countlineitemguid == "")
                            && obj.aData.ItemType == "1"
                            && !$('#IsArchivedRecords').is(':checked')
                            && !$('#IsDeletedRecords').is(':checked')
                            && obj.aData.creditcustomerownedquantity == 0
                            && obj.aData.creditconsignedquantity == 0
                            && (obj.aData.ActionType.toLowerCase() == 'pull' 
                            || obj.aData.ActionType.toLowerCase() == 'ms pull' 
                            || obj.aData.ActionType.toLowerCase() == 'credit' 
                            || obj.aData.ActionType.toLowerCase() == 'ms credit')
                        ) {

                            if ((obj.aData.SerialNumberTracking
                                || obj.aData.LotNumberTracking
                                || obj.aData.DateCodeTracking)
                                &&
                                (
                                    obj.aData.ActionType.toLowerCase() == 'ms pull'
                                    || obj.aData.ActionType.toLowerCase() == 'ms credit'
                                )
                            ) {
                                return '<span id="spnPullQuantity" class="spnimprove" style="position:relative">' + obj.aData.PoolQuantity_LabelView + '</span>';
                            }
                            else
                            {
                                return '<div id="dveditPullQty" class="editPullQty"><img src="/Content/images/editico.png" title="Edit Pull Quantity" /></div><span id="spnPullQuantity" class="spnimprove" style="position:relative">' + obj.aData.PoolQuantity_LabelView + '</span>';
                            }
                        }
                        else {
                            return '<span id="spnPullQuantity" class="spnimprove" style="position:relative">' + obj.aData.PoolQuantity_LabelView + '</span>';
                        }
                    }
                    else {
                        return '<span id="spnPullQuantity" class="spnimprove" style="position:relative">' + obj.aData.PoolQuantity_LabelView + '</span>';
                    }
                }
            });

            PullColumnsArr.push({ "mDataProp": "PullCost_LabelView", "sClass": "read_only numericalign isCost" });
            PullColumnsArr.push({ "mDataProp": "BinNumber", "sClass": "read_only" });
            PullColumnsArr.push({
                "mDataProp": "ProjectSpendName", "sClass": "read_only",
                "fnRender": function (obj, val) {
                    if (obj.aData.ProjectSpendName != null && obj.aData.ProjectSpendName != '') {
                        return '<span class="spnimprove" style="position:relative">' + obj.aData.ProjectSpendName + '</span>';
                    }
                    else {
                        return "";
                    }
                }
            });




            PullColumnsArr.push({ "mDataProp": "ID", "bSortable": true, "sClass": "read_only" });
            PullColumnsArr.push({ "mDataProp": "ItemNumber", "sClass": "read_only" });
            PullColumnsArr.push({ "mDataProp": "Created", "sClass": "read_only", "fnRender": function (obj, val) { return obj.aData.CreatedDate; } });
            PullColumnsArr.push({ "mDataProp": "Updated", "sClass": "read_only", "fnRender": function (obj, val) { return obj.aData.UpdatedDate; } });
            PullColumnsArr.push({ "mDataProp": "CreatedByName", "sClass": "read_only" });
            PullColumnsArr.push({ "mDataProp": "UpdatedByName", "sClass": "read_only" });
            PullColumnsArr.push({ "mDataProp": "ItemType_LabelView", "sClass": "read_only" });
            PullColumnsArr.push({ "mDataProp": "AddedFrom", "sClass": "read_only" });
            PullColumnsArr.push({ "mDataProp": "EditedFrom", "sClass": "read_only" });
            PullColumnsArr.push({ "mDataProp": "ReceivedOn", "sClass": "read_only", "fnRender": function (obj, val) { return obj.aData.ReceivedOnDate; } });
            PullColumnsArr.push({ "mDataProp": "ReceivedOnWeb", "sClass": "read_only", "fnRender": function (obj, val) { return obj.aData.ReceivedOnWebDate; } });
            PullColumnsArr.push({ "mDataProp": "Markup", "sClass": "read_only numericalign isCost" });
            PullColumnsArr.push({ "mDataProp": "SellPrice_LabelView", "sClass": "read_only numericalign isCost" });
            PullColumnsArr.push({ "mDataProp": "ItemCost_LabelView", "sClass": "read_only numericalign isCost" });
            PullColumnsArr.push({ "mDataProp": "Description", "sClass": "read_only" });
            PullColumnsArr.push({ "mDataProp": "Unit", "sClass": "read_only" });
            PullColumnsArr.push({ "mDataProp": "CategoryName", "sClass": "read_only" });
            PullColumnsArr.push({ "mDataProp": "DefaultPullQuantity_LabelView", "sClass": "read_only numericalign" });
            PullColumnsArr.push({ "mDataProp": "Manufacturer", "sClass": "read_only" });
            PullColumnsArr.push({ "mDataProp": "ManufacturerNumber", "sClass": "read_only" });
            PullColumnsArr.push({ "mDataProp": "SupplierName", "sClass": "read_only" });
            PullColumnsArr.push({ "mDataProp": "SupplierPartNo", "sClass": "read_only" });
            PullColumnsArr.push({ "mDataProp": "GLAccount", "sClass": "read_only" });
            PullColumnsArr.push({ "mDataProp": "OnHandQuantity_LabelView", "sClass": "read_only numericalign" });
            PullColumnsArr.push({ "mDataProp": "ItemOnhandQty_LabelView", "sClass": "read_only numericalign" });
            PullColumnsArr.push({ "mDataProp": "ItemLocationOnHandQty_LabelView", "sClass": "read_only numericalign" });
            PullColumnsArr.push({ "mDataProp": "CriticalQuantity_LabelView", "sClass": "read_only numericalign" });
            PullColumnsArr.push({ "mDataProp": "MinimumQuantity_LabelView", "sClass": "read_only numericalign" });
            PullColumnsArr.push({ "mDataProp": "MaximumQuantity_LabelView", "sClass": "read_only numericalign" });
            PullColumnsArr.push({ "mDataProp": "Taxable_LabelView", "sClass": "read_only" });
            PullColumnsArr.push({ "mDataProp": "AverageUsage_LabelView", "sClass": "read_only numericalign" });
            PullColumnsArr.push({ "mDataProp": "Turns_LabelView", "sClass": "read_only numericalign" });
            PullColumnsArr.push({ "mDataProp": "OnOrderQuantity_LabelView", "sClass": "read_only numericalign" });
            PullColumnsArr.push({ "mDataProp": "OnTransferQuantity_LabelView", "sClass": "read_only numericalign" });
            PullColumnsArr.push({ "mDataProp": "InTransitquantity_LabelView", "sClass": "read_only numericalign" });
            PullColumnsArr.push({ "mDataProp": "LongDescription", "sClass": "read_only" });
            PullColumnsArr.push({ "mDataProp": "RequisitionNumber", "sClass": "read_only" });
            PullColumnsArr.push({ "mDataProp": "WOName", "sClass": "read_only" });
            PullColumnsArr.push({ "mDataProp": "CostUOMName", "sClass": "read_only" });
            PullColumnsArr.push({ "mDataProp": "PullPrice_LabelView", "sClass": "read_only numericalign isCost" });
            PullColumnsArr.push({ "mDataProp": "ItemBlanketPO", "sClass": "read_only" });
            //PullColumnsArr.push({ "mDataProp": "ItemNumber", "sClass": "read_only" });
            PullColumnsArr.push({ "mDataProp": "SupplierAccountNo", "sClass": "read_only" });
            PullColumnsArr.push({ "mDataProp": "PullItemCost", "sClass": "read_only isCost" });
            PullColumnsArr.push({ "mDataProp": "PullMasterItemCost", "sClass": "read_only isCost" });
            PullColumnsArr.push({ "mDataProp": "PullMasterItemSellPrice", "sClass": "read_only isCost" });
            PullColumnsArr.push({ "mDataProp": "PullMasterItemMarkup", "sClass": "read_only isCost" });
            PullColumnsArr.push({ "mDataProp": "PullMasterItemCostUOMValue", "sClass": "read_only isCost" });
            PullColumnsArr.push({ "mDataProp": "PullMasterItemAverageCost", "sClass": "read_only isCost" });
            PullColumnsArr.push({
                "mDataProp": "ImagePath",
                "bSortable": true, "sClass": "read_only"
                , "fnRender": function (obj, val) {
                    var item = obj.aData;
                    if ((item.ImagePath != '' && item.ImagePath != null) || (item.ItemImageExternalURL != '' && item.ItemImageExternalURL != null)) {
                        if (item.ImageType != '' && item.ImageType != null && item.ImageType == 'ImagePath' && item.ImagePath != '' && item.ImagePath != null) {

                            var path = logoPathItemImage;
                            return '<img style="cursor:pointer;"  alt="' + (item.ItemNumber) + '" id="ItemImageBox" width="120px" height="120px" src="' + path + "/" + item.ItemID + "/" + item.ImagePath + '">';
                        }
                        else if (item.ImageType != '' && item.ImageType != null && item.ImageType == "ExternalImage" && item.ItemImageExternalURL != '' && item.ItemImageExternalURL != null) {
                            return '<img style="cursor:pointer;"  alt="' + (item.ItemNumber) + '" id="ItemImageBox" width="120px" height="120px" src="' + item.ItemImageExternalURL + '">';
                        }
                        else {
                            return "<img src='../Content/images/no-image.jpg' />";
                        }
                    }
                    else {
                        return "<img src='../Content/images/no-image.jpg' />";
                    }
                }                    
            });
            PullColumnsArr.push({ "mDataProp": "IsCustomerEDISent_LabelView", "sClass": "read_only" });
            $.each(PullMasterArray, function (index, val) {
                PullColumnsArr.push(val);
            });

            $.each(RequisitionMasterArray, function (index, val) {
                PullColumnsArr.push(val);
            });

            $.each(WorkOrderArray, function (index, val) {
                PullColumnsArr.push(val);
            });

            var $myDataTable = $('#' + myDataTableId);

            LoadTabs();
            //$('#tab1').click();
            var gaiSelected = [];
            var rowCallbackCount = 0;
            oTable = $myDataTable.dataTable({
                "bJQueryUI": true,
                "bScrollCollapse": true,
                //"bAutoWidth": false,
                "sScrollX": "150%",
                //"sScrollX": "8000px",
                "sDom": 'RC<"top"lp<"clear">>rt<"bottom"i<"clear">>T',
                "oColVis": {},
                "aaSorting": [[2, "asc"]],
                "oColReorder": {},
                "sPaginationType": "full_numbers",
                "bProcessing": true,
                "bStateSave": true,
                "oLanguage": oLanguage,
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                   // rowCallbackCount++;
                    //if (rowCallbackCount > 24) {
                    //}
                    var $nRow = $(nRow);
                    if (aData.IsDeleted == true && aData.IsArchived == true) {
                        $nRow.css('background-color', '#B9BCBF');
                        //nRow.className = "GridDeleatedArchivedRow";
                    }
                    else if (aData.IsDeleted == true) {
                        $nRow.css('background-color', '#FFCCCC');
                        //nRow.className = "GridDeletedRow";
                    }
                    else if (aData.IsArchived == true) {
                        $nRow.css('background-color', '#CCFFCC');
                        //nRow.className = "GridArchivedRow";
                    }

                    var isPull = $nRow.find('#spnPullOrCredit').text();
                    //var isrequisition = aData.requisitiondetailguid;
                    //var iscount = aData.countlineitemguid;
                    //var ItemType = aData.ItemType;

                    //var isSerialNumberTracking = aData.SerialNumberTracking;
                    //var isLotNumberTracking = aData.LotNumberTracking;
                    //var isDateCodeTracking = aData.DateCodeTracking;                        
                    //if (
                    //    ($.trim(isPull).toLowerCase() == 'pull' 
                    //        || $.trim(isPull).toLowerCase() == 'ms pull'           
                    //        || $.trim(isPull).toLowerCase() == 'credit'
                    //        || $.trim(isPull).toLowerCase() == 'ms credit')
                    //    && ($.trim(isrequisition).toLowerCase() == 'null' || $.trim(isrequisition).toLowerCase() == '')
                    //    && ($.trim(iscount).toLowerCase() == 'null' || $.trim(iscount).toLowerCase() == '')                        
                    //    && ItemType == 1
                    //    && !$('#IsDeletedRecords').is(':checked')
                    //    && !$('#IsArchivedRecords').is(':checked')) {
                    //    $nRow.find('#dveditPullQty').show();
                    //}
                    //else {
                    //    if ($.trim(isSerialNumberTracking).toLowerCase() == 'true'
                    //        && $.trim(isLotNumberTracking).toLowerCase() == 'false'
                    //        && $.trim(isDateCodeTracking).toLowerCase() == 'false') {
                    //        $nRow.find('#dveditPullQty').show();
                    //    }
                    //    else {
                    //        $nRow.find('#dveditPullQty').hide();
                    //    }
                    //}
                    var creditcustomerownedquantity = aData.creditcustomerownedquantity;
                    var creditconsignedquantity = aData.creditconsignedquantity;

                    //var vUdf1_new = $(nRow).find("input#hdnUDF1").val() == "null" ? "" : $(nRow).find("input#hdnUDF1").val();
                    //var vUdf2_new = $(nRow).find("input#hdnUDF2").val() == "null" ? "" : $(nRow).find("input#hdnUDF2").val();
                    //var vUdf3_new = $(nRow).find("input#hdnUDF3").val() == "null" ? "" : $(nRow).find("input#hdnUDF3").val();
                    //var vUdf4_new = $(nRow).find("input#hdnUDF4").val() == "null" ? "" : $(nRow).find("input#hdnUDF4").val();
                    //var vUdf5_new = $(nRow).find("input#hdnUDF5").val() == "null" ? "" : $(nRow).find("input#hdnUDF5").val();

                    //if ($(nRow).find('#UDF1') != null && vUdf1_new != "") {
                    //    if ($(nRow).find('#UDF1').attr("class") == 'selectBox') {
                    //        $(nRow).find('#UDF1').find("option:contains(" + vUdf1_new + ")").attr('selected', 'selected');

                    //    }
                    //    else {
                    //        $(nRow).find('#UDF1').val(vUdf1_new);
                    //    }

                    //}
                    //if ($(nRow).find('#UDF2') != null && vUdf2_new != "") {
                    //    if ($(nRow).find('#UDF2').attr("class") == 'selectBox') {
                    //        $(nRow).find('#UDF2').find("option:contains(" + vUdf2_new + ")").attr('selected', 'selected');

                    //    }
                    //    else {
                    //        $(nRow).find('#UDF2').val(vUdf2_new);
                    //    }
                    //}
                    //if ($(nRow).find('#UDF3') != null && vUdf3_new != "") {
                    //    if ($(nRow).find('#UDF3').attr("class") == 'selectBox') {
                    //        $(nRow).find('#UDF3').find("option:contains(" + vUdf3_new + ")").attr('selected', 'selected');

                    //    }
                    //    else {
                    //        $(nRow).find('#UDF3').val(vUdf3_new);
                    //    }
                    //}
                    //if ($(nRow).find('#UDF4') != null && vUdf4_new != "") {
                    //    if ($(nRow).find('#UDF4').attr("class") == 'selectBox') {
                    //        $(nRow).find('#UDF4').find("option:contains(" + vUdf4_new + ")").attr('selected', 'selected');

                    //    }
                    //    else {
                    //        $(nRow).find('#UDF4').val(vUdf4_new);
                    //    }
                    //}
                    //if ($(nRow).find('#UDF5') != null && vUdf5_new != "") {
                    //    if ($(nRow).find('#UDF5').attr("class") == 'selectBox') {
                    //        $(nRow).find('#UDF5').find("option:contains(" + vUdf5_new + ")").attr('selected', 'selected');

                    //    }
                    //    else {
                    //        $(nRow).find('#UDF5').val(vUdf5_new);
                    //    }
                    //}

                    if ($.trim(isPull).toLowerCase() == 'pull' && !isNaN(parseFloat(aData.ConsignedQuantity)) && parseFloat(aData.ConsignedQuantity) > 0) // billing flag needs to check only for Consigned pull
                    {

                        //                        if (aData.Billing == 'Yes') {
                        //                            $(nRow).attr({ 'style': 'background-color:#F6B3B3' });
                        //                        }
                        //                        else {
                        //                            $(nRow).attr({ 'style': 'background-color:#C8F6C8' });
                        //                        }

                        if (aData.Billing == true && aData.IsEDISent == true) {
                            $nRow.attr({ 'style': 'background-color:#C8F6C8' });
                        }
                        else if (aData.Billing == true && aData.IsEDISent == false) {
                            $nRow.attr({ 'style': 'background-color:#FFFF00' });
                        }
                        else if (aData.Billing == false || aData.Billing == null) {
                            $nRow.attr({ 'style': 'background-color:#F6B3B3' });
                        }
                    }

                    if (self.gridRowStartIndex === null) {
                        self.gridRowStartIndex = this.fnSettings()._iDisplayStart;
                    }

                    $("td.RowNo:first", $nRow).html(self.gridRowStartIndex + iDisplayIndex + 1);
                    return nRow;
                },
                "fnStateSaveParams": self.grid_fnStateSaveParams,
                "fnStateLoad": function (oSettings) {
                    var o;
                    $.ajax({
                        "url": self.urls.loadGridStateUrl,
                        "type": "POST",
                        data: { ListName: 'PullMaster' },
                        "async": false,
                        cache: false,
                        "dataType": "json",
                        "success": function (json) {
                            if (json.jsonData != '')
                                o = JSON.parse(json.jsonData);
                        }
                    });
                    return o;
                },
                "bServerSide": true,
                "sAjaxSource": self.urls.pullMasterListAjaxUrl,
                "fnServerData": self.grid_fnServerData,
                "fnInitComplete": function (oSettings) {
                    $('.ColVis').detach().appendTo(".setting-arrow");
                },
                "aoColumns": PullColumnsArr
            });

            //HIDE PRINT CONTAINER
            $('.DTTT_container').css('z-index', '-1');
                  

            if (isCost == 'False') {
                HideColumnUsingClassName(myDataTableId);
            }

            //IsRefresh
            var QueryStringParam = _Common.getParameterByName('IsRefresh');
            if (QueryStringParam != '' && QueryStringParam == 'yes') {
                $("#tab1").click();
            }

            $myDataTable.on("tap click", "tbody tr", function (e) {
                if ($(this)[0].style.backgroundColor == "red") {
                    var $divTarget = $('div#target'),
                        $spanGlobalMessage = $("#spanGlobalMessage");

                    $(this).removeClass('row_selected');
                    $divTarget.fadeToggle();
                    $divTarget.delay(DelayTime).fadeOut(FadeOutTime);
                    $spanGlobalMessage.text(MsgBillingDoneDeleteValidation);
                    $spanGlobalMessage.removeClass('succesIcon errorIcon').addClass('WarningIcon');
                    return false;
                }
                return true;
            });


            /*Functions used for nasted data binding START*/
            $myDataTable.on("click", "td.control", function (event) {

                if ($(this).find('img').length <= 0)
                    return;

                var nTr = this.parentNode;
                var i = $.inArray(nTr, anOpen);
                if (i === -1) {
                    $('img', this).attr('src', sImageUrl + "drildown_close.jpg");
                    //oTable.fnOpen(nTr, fnFormatDetails(oTable, nTr), '');

                    fnFormatDetails(oTable, nTr);

                    anOpen.push(nTr);
                }
                else {
                    $('img', this).attr('src', sImageUrl + "drildown_open.jpg");
                    oTable.fnClose(nTr);
                    anOpen.splice(i, 1);
                }
            });

            $myDataTable.on("click", ".editPullQty img", function (event) {                   
                var parenttd = $(this).parents('td');
                var currentVal = $('span.spnimprove', parenttd).text();           
                if (currentVal == "") {
                    currentVal = $(this).parents('td').find("#txtPullQty").val();
                }
                var _strtxt = '';
                _strtxt = "<input type='text' id='txtPullQty' value='" + currentVal + "' return onlyNumeric(event); class='text-boxinner numericinput numericalign'  style='width:93%;'>";
                $('span.spnimprove', parenttd).html(_strtxt);
            });
            $myDataTable.on("blur", "#txtPullQty", function (event) {
                var SerialNumberTracking = $(this).parents().find("#hdnSerialNumberTracking").val();
                var LotNumberTracking = $(this).parents().find("#hdnLotNumberTracking").val();
                var DateCodeTracking = $(this).parents().find("#hdnDateCodeTracking").val();  
                var hdnPullQuantity = $(this).parents().find("#hdnPullQuantity").val(); 

                hdnPullQuantity = hdnPullQuantity.replace(/[^\d].+/, "");
                $(this).val($(this).val().replace(/[^\d].+/, ""));
                if (event.which == 0) {
                    UpdatePullQtyInPullHistory($(this).parents(), hdnPullQuantity, $(this).val(), SerialNumberTracking, LotNumberTracking, DateCodeTracking);
                }
                else if ((event.which < 48 || event.which > 57)) {
                    event.preventDefault();
                }
                else {
                    UpdatePullQtyInPullHistory($(this).parents(), hdnPullQuantity, $(this).val(), SerialNumberTracking, LotNumberTracking, DateCodeTracking);
                }
            });

            $myDataTable.on("click", ".editudf img", function (event) {
                //$("#myDataTable td span.spnimprove").click(function (event) {
                //if ($(this).find('span').length <= 0)
                //    return;

                var parenttd = $(this).parents('td');

                // if (event.toElement.tagName == "IMG" && event.toElement.alt != "select") {



                var _strtxt = ''
                var currentVal = $('span.spnimprove', parenttd).text();
                var _vctltype = $('span.spnimprove', parenttd).attr("data-ctype");
                var _dataudfname = $('span.spnimprove', parenttd).attr("data-udfname");
                var _datamaxl = $('span.spnimprove', parenttd).attr("data-maxl");
                var _dataid = $('span.spnimprove', parenttd).attr("data-id");
                if (_vctltype == "Textbox") {
                    if (currentVal == "") {
                        var CurrentSetValue = $(parenttd).find("#" + _dataudfname).val();
                        if (typeof CurrentSetValue != "undefined"
                            && CurrentSetValue != undefined
                            && CurrentSetValue != "") {
                            currentVal = CurrentSetValue;
                        }
                    }
                    _strtxt = '<input type="text" id="' + _dataudfname + '" value="' + currentVal + '" class="text-boxinner" onchange="UpdateUDFInPullHistory(this);" style="width:93%;" maxlength="' + _datamaxl + '">';
                    $('span.spnimprove', parenttd).html(_strtxt);
                }
                else if (_vctltype == "Dropdown") {
                    var _strselect = '';
                    var currElement = $(parenttd);
                    var CurrentSetValue = $(parenttd).find("#" + _dataudfname + " option:selected").text();;                    
                    if (typeof CurrentSetValue != "undefined"
                        && CurrentSetValue != undefined
                        && CurrentSetValue != "") {
                        currentVal = CurrentSetValue;
                    }
                    _AjaxUtil.postJson(_PullMasterList.urls.getUDFOptionByIDUrl
                        , { UDFID: _dataid }
                        , function (response) {
                            _strselect = '<select id="' + _dataudfname + '" class="selectBox" style="width:93%;" onchange="UpdateUDFInPullHistory(this);"><option></option>';
                            $.each(response.UDFData, function (key, value) {
                                //$("select#EmailTemplateToken").append($("<option></option>").val(value.key).html(value.value));
                                if (value.UDFOption == currentVal)
                                    _strselect += "<option selected='selected'  value='" + value.ID + "'>" + value.UDFOption + "</option>";
                                else
                                    _strselect += "<option value='" + value.ID + "'>" + value.UDFOption + "</option>";
                            });
                            _strselect += "</select>";

                            $('span.spnimprove', currElement).html(_strselect);
                        }
                        , function (response) {

                        },true,false
                    );

                    //$.ajax({
                    //    url: _PullMasterList.urls.getUDFOptionByIDUrl,
                    //    data: { UDFID: _dataid },
                    //    dataType: 'json',
                    //    type: 'POST',
                    //    //contentType: 'application/json',
                    //    //async: false,
                    //    cache: false,
                    //    success: function (response) {
                    //        _strselect = '<select id="' + _dataudfname + '" class="selectBox" style="width:93%;" onchange="UpdateUDFInPullHistory(this);"><option></option>';
                    //        $.each(response.UDFData, function (key, value) {
                    //            //$("select#EmailTemplateToken").append($("<option></option>").val(value.key).html(value.value));
                    //            if (value.UDFOption == currentVal)
                    //                _strselect += "<option selected='selected'  value='" + value.ID + "'>" + value.UDFOption + "</option>";
                    //            else
                    //                _strselect += "<option value='" + value.ID + "'>" + value.UDFOption + "</option>";
                    //        });
                    //        _strselect += "</select>";

                    //        $('span.spnimprove', currElement).html(_strselect);
                    //    },
                    //    error: function (response) {

                    //    }
                    //});
                }
                else if (_vctltype == "Dropdown Editable") {
                    if (currentVal == "") {
                        var CurrentSetValue = $(parenttd).find("#" + _dataudfname).val();
                        if (typeof CurrentSetValue != "undefined"
                            && CurrentSetValue != undefined
                            && CurrentSetValue != "") {
                            currentVal = CurrentSetValue;
                        }
                    }
                    var _streditdrop = '';
                    _streditdrop += "<input id='hdn" + _dataid + "' type='hidden' value='" + _dataid + "' /><input type='text' id='" + _dataudfname + "' name='" + _dataudfname + "' value='" + currentVal + "' class='text-boxinner udf-editable-dropdownbox udf-editable-autocomplete-dropdownbox' onchange='UpdateUDFInPullHistory(this);'  maxlength:22px; style='min-width:80px;width:93%' >";
                    _streditdrop += "<a id='lnkShowAllOptions' href='javascript:void(0);' style='position:absolute; right:5px; top:0px;' class='show-all-options' ><img src='" + arrow_down_black +"' alt='select' /></a>";

                    $('span.spnimprove', parenttd).html(_streditdrop);
                }

                // }
            });

            $myDataTable.on("click", ".editpullOrder img", function (event) {
                var parenttd = $(this).parents('td');
                var _strtxt = ''
                var currentVal = $('span.spnimprove', parenttd).text();
                var _vctltype = $('span.spnimprove', parenttd).attr("data-ctype");                
                var _datamaxl = $('span.spnimprove', parenttd).attr("data-maxl");                

                if (_vctltype == "Textbox") {  
                    if (currentVal == "") {
                        var CurrentSetValue = $(parenttd).find("#txtPullOrderNumber").val();
                        if (typeof CurrentSetValue != "undefined"
                            && CurrentSetValue != undefined
                            && CurrentSetValue != "") {
                            currentVal = CurrentSetValue;
                        }
                    }
                    _strtxt = '<input type="text" id="txtPullOrderNumber"  value="' + currentVal + '" class="text-boxinner" onchange="UpdatePullOrderNumberInPullHistory(this,true);" style="width:93%;" maxlength="' + _datamaxl + '">';
                    $('span.spnimprove', parenttd).html(_strtxt);
                }
                else if (_vctltype == "Dropdown") {
                    var _strselect = '';
                    var currElement = $(parenttd);
                    var CurrentSetValue = $(parenttd).find("#txtPullOrderNumber option:selected").text();
                    if (typeof CurrentSetValue != "undefined"
                        && CurrentSetValue != undefined
                        && CurrentSetValue != "") {
                        currentVal = CurrentSetValue;
                    }
                    _AjaxUtil.postJson(_PullMasterList.urls.GetPullOrderNumberUrl
                        , { NameStartWith: "" }
                        , function (response) {                            
                            _strselect = '<select id="txtPullOrderNumber" class="selectBox" style="width:93%;" onchange="UpdatePullOrderNumberInPullHistory(this,true);"><option></option>';
                            $.each(response, function (key, value) {
                                if (value.Key == currentVal)
                                    _strselect += "<option selected='selected'  value='" + value.ID + "'>" + value.Key + "</option>";
                                else
                                    _strselect += "<option value='" + value.ID + "'>" + value.Key + "</option>";
                            });
                            _strselect += "</select>";

                            $('span.spnimprove', currElement).html(_strselect);
                        }
                        , function (response) {

                        }, true, false
                    );
                }
                else if (_vctltype == "Dropdown Editable") {
                    if (currentVal == "") {
                        var CurrentSetValue = $(parenttd).find("#txtPullOrderNumber").val();
                        if (typeof CurrentSetValue != "undefined"
                            && CurrentSetValue != undefined
                            && CurrentSetValue != "") {
                            currentVal = CurrentSetValue;
                        }
                    }
                    var _streditdrop = '';
                    _streditdrop += "<input id='hdntxtPullOrderNumber' type='hidden' value='" + currentVal + "' /><input type='text' id='txtPullOrderNumber' name='txtPullOrderNumber' value='" + currentVal + "' class='text-boxinner AutoPullOrderNumber' onchange='UpdatePullOrderNumberInPullHistory(this,true);'  maxlength:22'; style='min-width:80px;width:93%' >";
                    _streditdrop += "<a id='lnkShowAllOptions' href='javascript:void(0);' style='position:absolute; right:5px; top:0px;' class='ShowAllOptionsOrderNumber' ><img src='" + arrow_down_black + "' alt='select' /></a>";

                    $('span.spnimprove', parenttd).html(_streditdrop);
                }
            });

            // For Edit pull Quantity for serail lot datecode popup button events start //

            // raw selection start ///

            $("#DivPullSelectionEdit").off("tap click", ".tbl-item-pull tbody tr");
            $("#DivPullSelectionEdit").on("tap click", ".tbl-item-pull tbody tr", function (e) {
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

            // txtbox focus and change event start //

            $("#DivPullSelectionEdit").off('change', "input[type='text'][name^='txtLotOrSerailNumber']");
            $("#DivPullSelectionEdit").on('change', "input[type='text'][name^='txtLotOrSerailNumber']", function (e) {                
                var objCurtxt = $(this);
                var oldValue = $(objCurtxt).val();
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
                var PullGUID = strpullobj.GUID;

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

                    if ($("#hdnTrackingType_" + ids[1].toString() + "_" + ids[2].toString()).val() == "LotNumberTracking")
                        alert(MsgDuplicateLotNumber);
                    else if ($("#hdnTrackingType_" + ids[1].toString() + "_" + ids[2].toString()).val() == "SerialNumberTracking")
                        alert(MsgDuplicateSerialNumberValidation);
                    else
                        alert(DuplicateNumber);

                    $(objCurtxt).val("");
                    $(objCurtxt).focus();
                }
                else {
                    $.ajax({
                        type: "POST",
                        url: _PullMasterList.urls.ValidateSerialLotNumberForEditUrl,
                        contentType: 'application/json',
                        dataType: 'json',
                        data: "{ ItemGuid: '" + ids[1].toString() + "',PullGUID: '" + PullGUID + "' , SerialOrLotNumber: '" + $.trim($(objCurtxt).val()) + "',BinID: '" + aData.BinID + "',MaterialStagingGUID:'" + materialStagingGuid + "' }",
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
                            console.log(err);
                        }
                    });
                }
            });

            $("#DivPullSelectionEdit").off('focus', "input[type='text'][name^='txtLotOrSerailNumber']");
            $("#DivPullSelectionEdit").on('focus', "input[type='text'][name^='txtLotOrSerailNumber']", function (e) {                
                var objCurtxt = $(this);
                //var ids = $(this).parent().parent().parent().find("input[type='hidden'][name='hdnRowUniqueId']").val().split('_');
                var ids = $(this).parent().parent().parent().parent().parent().parent().parent().parent().parent().find("[id^='hdnPullIds_']").val().split('_');
                var aPos = $("#tblItemPull_" + ids[1].toString() + "_" + ids[2].toString()).dataTable().fnGetPosition($(this).parent().parent().parent()[0]);
                var aData = $("#tblItemPull_" + ids[1].toString() + "_" + ids[2].toString()).dataTable().fnGetData(aPos);

                var dtItemPull = "#tblItemPull_" + ids[1].toString() + "_" + ids[2].toString();
                var strSerialLotNos = "";
                var strpullobj = JSON.parse($("#hdnPullMasterDTO_" + ids[1].toString() + "_" + ids[2].toString()).val());
                var materialStagingGuid = strpullobj.MaterialStagingGUID;
                var PullGUID = strpullobj.GUID;

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
                                url: _PullMasterList.urls.GetLotOrSerailNumberListForEditUrl,
                                contentType: 'application/json',
                                dataType: 'json',
                                data: {
                                    maxRows: 1000,
                                    name_startsWith: request.term,
                                    ItemGuid: ids[1].toString(),
                                    PullGUID: PullGUID,
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

            // txtbox focus and change event end //


            // raw selection end//

            function IsEditLotSerialExistsInCurrentLoaded(strIds, SerialLot) {
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

            /// for add new raw button click start ///

            $("#DivPullSelectionEdit").off("click", "input[type='button'][name='btnLoadMoreLots']");
            $("#DivPullSelectionEdit").on("click", "input[type='button'][name='btnLoadMoreLots']", function () {                
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
                                if (txtLotOrSerailNumber != undefined && !IsEditLotSerialExistsInCurrentLoaded(strIds, txtLotOrSerailNumber)) {
                                    strIds = strIds + txtLotOrSerailNumber + "_" + txtPullQty + ",";
                                }
                            }
                            else if ((hdnLotNumberTracking == "true" && hdnDateCodeTracking == "true")
                                || (hdnSerialNumberTracking == "true" && hdnDateCodeTracking == "true")) {
                                var hdnExpiration = $tr.find("input[name='hdnExpirationDate']").val();
                                var txtLotOrSerailNumber = $.trim($tr.find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                                if (txtLotOrSerailNumber != undefined && hdnExpiration != undefined && !IsEditLotSerialExistsInCurrentLoaded(strIds, hdnExpiration)) {
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
                                if (hdnBinNumber != undefined && !IsEditLotSerialExistsInCurrentLoaded(strIds, hdnBinNumber))
                                    strIds = strIds + hdnBinNumber + "_" + txtPullQty + ",";
                            }
                        }
                    });//loop

                    $("#hdnCurrentLoadedIds_" + vItemGUID + "_" + vRequisitionDetailGUID).val(strIds);

                    var dt = $(dtID).dataTable();
                    dt.fnStandingRedraw();
                }
                else {
                    alert(MsgPullCreditQuantity);
                }
            });

            /// for add new raw button click end ///

            /// for delete raw start///

            $("#DivPullSelectionEdit").off('click', "input[type='button'][name='btnDeleteLots']");
            $("#DivPullSelectionEdit").on('click', "input[type='button'][name='btnDeleteLots']", function (e) {                
                var vItemGUID = $(this).prop("id").split('_')[1];
                var vRequisitionDetailGUID = $(this).prop("id").split('_')[2];

                var dtID = "#tblItemPull_" + vItemGUID + "_" + vRequisitionDetailGUID;

                var TotalRows = $(dtID + ' tbody tr').length;
                var SelectedRows = $(dtID + ' tbody tr.row_selected').length;
                var RemainingRows = TotalRows - SelectedRows;

                if (SelectedRows <= 0) {
                    alert(MsgSelectRowToDelete);
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
                            if ((hdnLotNumberTracking == "true" && hdnDateCodeTracking == "false")
                                || (hdnSerialNumberTracking == "true" && hdnDateCodeTracking == "false")) {
                                var txtLotOrSerailNumber = $.trim($tr.find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                                if (txtLotOrSerailNumber != undefined && !IsEditLotSerialExistsInCurrentLoaded(strIds, txtLotOrSerailNumber)) {
                                    strIds = strIds + txtLotOrSerailNumber + "_" + txtPullQty + ",";
                                }
                            }
                            else if ((hdnLotNumberTracking == "true" && hdnDateCodeTracking == "true")
                                || (hdnSerialNumberTracking == "true" && hdnDateCodeTracking == "true")) {
                                var hdnExpiration = $tr.find("input[name='hdnExpirationDate']").val();
                                var txtLotOrSerailNumber = $.trim($tr.find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                                if (txtLotOrSerailNumber != undefined && hdnExpiration != undefined && !IsEditLotSerialExistsInCurrentLoaded(strIds, hdnExpiration)) {
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
                                if (hdnBinNumber != undefined && !IsEditLotSerialExistsInCurrentLoaded(strIds, hdnBinNumber)) {
                                    strIds = strIds + hdnBinNumber + "_" + txtPullQty + ",";
                                }
                            }

                        });// loop

                        $("#hdnCurrentLoadedIds_" + vItemGUID + "_" + vRequisitionDetailGUID).val(strIds);
                        isDeleteSrLotRowEdit = true;
                        var dtThisItem = $(dtID).dataTable();
                        dtThisItem.fnStandingRedraw();
                    }
                    else {
                        alert(MsgRowShouldExists);
                    }
                }
            });

            // for delete raw end ///

            // for change Pull Qty start //

            $("#DivPullSelectionEdit").off("change", "input[type='text'][name='txtPullQty']");
            $("#DivPullSelectionEdit").on("change", "input[type='text'][name='txtPullQty']", function () {
                var ids = $(this).parent().parent().parent().parent().parent().parent().parent().parent().find("[id^='hdnPullIds_']").val().split('_');
                var aPos = $("#tblItemPull_" + ids[1].toString() + "_" + ids[2].toString()).dataTable().fnGetPosition($(this).parent().parent()[0]);
                $("#tblItemPull_" + ids[1].toString() + "_" + ids[2].toString()).dataTable().fnGetData(aPos).PullQuantity = $(this).val();
            });

            // for chagne Pull Qty end //

            // for cancel button start //

            $("#DivPullSelectionEdit").off("click", "input[type='button'][name='btnCancelPullPopup']");
            $("#DivPullSelectionEdit").on("click", "input[type='button'][name='btnCancelPullPopup']", function () {
                $("#DivPullSelectionEdit").empty();
                $('#DivPullSelectionEdit').dialog('close');
            });

            // for cancel button end //

            // for pull and pull all button start //

            $("#DivPullSelectionEdit").off("click", "input[type='button'][name='btnPullPopup']");
            $("#DivPullSelectionEdit").on("click", "input[type='button'][name='btnPullPopup']", function () {
               
                var vItemGUID = $(this).prop("id").split('_')[1];
                var vRequisitionDetailGUID = $(this).prop("id").split('_')[2];
                var dtID = "#tblItemPull_" + vItemGUID + "_" + vRequisitionDetailGUID;

                var ArrItem = new Array();
                var arrItemDetails;
                var ErrorMessage = ValidateSingleEditPull(vItemGUID, vRequisitionDetailGUID);

                if (ErrorMessage == "") {

                    arrItemDetails = new Array();
                    var ID = vItemGUID;
                    var SpanQty = $("#DivPullSelectionEdit").find("#txtPoolQuantity_" + vItemGUID + "_" + vRequisitionDetailGUID);

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
                            hdnExpirationDate = $tr.find("input[name='hdnExpirationDate']").val();

                        if (txtPullQty != "") {
                            var txtLotOrSerailNumber = "";
                            if (hdnLotNumberTracking == "true" || hdnSerialNumberTracking == "true") {
                                txtLotOrSerailNumber = $tr.find("input[type='text'][name^='txtLotOrSerailNumber']").val();
                            }

                            var vSerialNumber = "",
                                vLotNumber = "",
                                vExpiration = "";
                                vExpirationDate = "";

                            if (hdnSerialNumberTracking == "true") {
                                vSerialNumber = txtLotOrSerailNumber;
                            }
                            if (hdnLotNumberTracking == "true") {
                                vLotNumber = txtLotOrSerailNumber;
                            }
                            if (hdnDateCodeTracking == "true") {
                                vExpiration = hdnExpiration;
                                vExpirationDate = hdnExpirationDate;
                            }                            
                            var obj = {
                                "LotOrSerailNumber": txtLotOrSerailNumber, "BinNumber": hdnBinNumber, "PullQuantity": parseFloat(txtPullQty.toString())
                                , "LotNumberTracking": hdnLotNumberTracking, "SerialNumberTracking": hdnSerialNumberTracking, "DateCodeTracking": hdnDateCodeTracking
                                , "Expiration": hdnExpiration, "ExpirationDate": hdnExpirationDate, "SerialNumber": $.trim(vSerialNumber), "LotNumber": $.trim(vLotNumber)
                                , "ItemGUID": strpullobj.ItemGUID, "BinID": strpullobj.BinID, "ID": strpullobj.BinID
                                , "PullGUID": strpullobj.GUID
                                , "TotalTobePulled": parseFloat(txtPullQty.toString())
                            };

                            arrItemDetails.push(obj);
                        }
                    }); // loop

                    var pullQty = parseFloat($(SpanQty).val().toString());

                    var PullItem = {
                        ID: 1,
                        ItemGUID: strpullobj.ItemGUID,
                        PullGUID: strpullobj.GUID,
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
                        TotalConsignedTobePulled: pullQty,
                        TotalCustomerOwnedTobePulled: pullQty
                    };

                    ArrItem.push(PullItem);

                    if (ArrItem.length > 0) {
                        EditPullMultipleItemNew(ArrItem);
                    }
                }
                else {
                    alert(ErrorMessage);
                }
            });

            $("#DivPullSelectionEdit").off("click", "input[type='button'][name='btnPullAllPopUp']");
            $("#DivPullSelectionEdit").on("click", "input[type='button'][name='btnPullAllPopUp']", function () {
                EditPullAllNewFlow();
            });

            // for pull and pull all button end //

            // For Edit pull Quantity for serail lot datecode popup button events end //

            /* For Edit Credit for Lot,serial and datecode start */

            function IsLotSerialExistsInCurrentLoadedForCredit(strIds, SerialLot) {
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

            $("#divPreCreditInforSerialLotEdit").off("click", "input[type='button'][name='btnPullPopup']");
            $("#divPreCreditInforSerialLotEdit").on("click", "input[type='button'][name='btnPullPopup']", function () {                
                var vItemGUID = $(this).prop("id").split('_')[1];

                var dtID = "#tblItemPull_" + vItemGUID;

                var ArrItem = new Array();
                var arrItemDetails;
                var ErrorMessage = ValidateSinglePullForCreditEdit(vItemGUID);
                gblPreCreditObjToSaveForCredit = new Array();

                if (ErrorMessage == "") {

                    var itemGuid = vItemGUID;
                    var SpanQty = $("#divPreCreditInforSerialLotEdit").find("#txtPoolQuantity_" + vItemGUID);
                    var BinID = $("#divPreCreditInforSerialLotEdit").find("#hdnBinID_" + vItemGUID).val();
                    var MainBinNumber = $("#divPreCreditInforSerialLotEdit").find("#hdnMainBinNumber_" + vItemGUID).val();

                    var strpullobj;
                    if ($("input[type='hidden'][name^='hdnPullMasterDTO']").length > 0)
                        strpullobj = JSON.parse($("input[type='hidden'][name^='hdnPullMasterDTO']").val());

                    var creditarrObj = new Array();
                    $("#tblItemPull_" + vItemGUID).find("tbody").find("tr").each(function (index, tr) {

                        var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();

                        var hdnLotNumberTracking = $(tr).find("input[name='hdnLotNumberTracking']").val();
                        var hdnSerialNumberTracking = $(tr).find("input[name='hdnSerialNumberTracking']").val();
                        var hdnDateCodeTracking = $(tr).find("input[name='hdnDateCodeTracking']").val();

                        var hdnBinNumber = $(tr).find("input[name='hdnBinNumber']").val();
                        var hdnExpiration = $(tr).find("input[type='text'][name='txtExpirationDate']").val();
                        var hdnExpirationDate = $(tr).find("input[name='hdnExpirationDate']").val();

                        if (txtPullQty != "") {

                            var txtLotOrSerailNumber = "";
                            if (hdnLotNumberTracking == "true" || hdnSerialNumberTracking == "true")
                                var txtLotOrSerailNumber = $(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val();

                            var vSerialNumber = "";
                            var vLotNumber = "";
                            var vExpiration = "";
                            var vExpirationDate = "";

                            if (hdnSerialNumberTracking == "true")
                                vSerialNumber = txtLotOrSerailNumber;
                            if (hdnLotNumberTracking == "true")
                                vLotNumber = txtLotOrSerailNumber;
                            if (hdnDateCodeTracking == "true") {
                                vExpiration = hdnExpiration;
                                vExpirationDate = hdnExpirationDate;
                            }
                        }                        
                        creditarrObj.push({ 'Serial': $.trim(vSerialNumber), 'Lot': $.trim(vLotNumber), 'ExpireDate': hdnExpiration, 'Qty': parseFloat(txtPullQty.toString()) });                                                                    
                    });
                    var Creditobj = {};
                    var ItemTracking = "";
                    if (strpullobj.SerialNumberTracking)
                        ItemTracking = "SERIALTRACK";
                    else if (strpullobj.LotNumberTracking)
                        ItemTracking = "LOTTRACK";
                    else if (strpullobj.DateCodeTracking)
                        ItemTracking = "DATECODETRACK";

                    Creditobj = {
                        'ItemGuid': strpullobj.ItemGUID, 'PullGuid': strpullobj.GUID, 'WOGuid': strpullobj.WorkOrderDetailGUID,
                        'ItemType': strpullobj.ItemType, 'Bin': MainBinNumber,
                        'ProjectName': strpullobj.ProjectName, 'Quantity': parseFloat(SpanQty.val().toString()),
                        'UDF1': strpullobj.UDF1, 'UDF2': strpullobj.UDF2, 'UDF3': strpullobj.UDF3,
                        'UDF4': strpullobj.UDF4, 'UDF5': strpullobj.UDF5, 'ItemTracking': ItemTracking,
                        'PrevPullQty': 0, 'ItemNumber': strpullobj.ItemNumber, 'PullOrderNumber': strpullobj.PullOrderNumber,
                        'SupplierAccountGuid': strpullobj.SupplierAccountGuid,
                        'PrevPullsToCredit': creditarrObj
                    };
                    gblPreCreditObjToSaveForCredit.push(Creditobj);

                    if (gblPreCreditObjToSaveForCredit != null && gblPreCreditObjToSaveForCredit.length > 0) {                      
                        setTimeout(function () {
                            SavePullCreditEdit(gblPreCreditObjToSaveForCredit)
                        }, 100);
                    }
                }
                else {
                    alert(ErrorMessage);
                }
            });

            $("#divPreCreditInforSerialLotEdit").off("click", "input[type='button'][name='btnPullAllPopUp']");
            $("#divPreCreditInforSerialLotEdit").on("click", "input[type='button'][name='btnPullAllPopUp']", function () {                
                var ArrItem = new Array();
                gblPreCreditObjToSaveForCredit = new Array();
                var arrItemDetails;
                var ErrorMessage = ValidateAllPullForCreditEdit();

                if (ErrorMessage == "") {
                    $("#divPreCreditInforSerialLotEdit").find("table[id^='tblItemPullheader']").each(function (indx, tblHeader) {

                        var strpullobj = JSON.parse($(tblHeader).find("input[name='hdnPullMasterDTO']").val());
                        var vItemGUID = $(tblHeader).prop("id").split('_')[1];

                        var ArrItem = new Array();
                        var arrItemDetails;
                        var itemGuid = vItemGUID;
                        var SpanQty = $(tblHeader).find("#txtPoolQuantity_" + vItemGUID);

                        var BinID = $(tblHeader).find("#hdnBinID_" + vItemGUID).val();
                        var MainBinNumber = $(tblHeader).find("#hdnMainBinNumber_" + vItemGUID).val();

                        var creditarrObj = new Array();
                        var ItemTracking = "";

                        $("#tblItemPull_" + vItemGUID).find("tbody").find("tr").each(function (index, tr) {

                            var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();

                            var hdnLotNumberTracking = $(tr).find("input[name='hdnLotNumberTracking']").val();
                            var hdnSerialNumberTracking = $(tr).find("input[name='hdnSerialNumberTracking']").val();
                            var hdnDateCodeTracking = $(tr).find("input[name='hdnDateCodeTracking']").val();

                            var hdnBinNumber = $(tr).find("input[name='hdnBinNumber']").val();
                            var hdnExpiration = $(tr).find("input[type='text'][name='txtExpirationDate']").val();
                            var hdnExpirationDate = $(tr).find("input[name='hdnExpirationDate']").val();

                            if (txtPullQty != "") {

                                var txtLotOrSerailNumber = "";
                                if (hdnLotNumberTracking == "true" || hdnSerialNumberTracking == "true")
                                    var txtLotOrSerailNumber = $(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val();

                                var vSerialNumber = "";
                                var vLotNumber = "";
                                var vExpiration = "";
                                var vExpirationDate = "";

                                if (hdnSerialNumberTracking == "true")
                                    vSerialNumber = txtLotOrSerailNumber;
                                if (hdnLotNumberTracking == "true")
                                    vLotNumber = txtLotOrSerailNumber;
                                if (hdnDateCodeTracking == "true") {
                                    vExpiration = hdnExpiration;
                                    vExpirationDate = hdnExpirationDate;
                                }
                            }

                            creditarrObj.push({ 'Serial': $.trim(vSerialNumber), 'Lot': $.trim(vLotNumber), 'ExpireDate': hdnExpiration, 'Qty': parseFloat(txtPullQty.toString()) });                            
                        });

                        if (strpullobj.SerialNumberTracking)
                            ItemTracking = "SERIALTRACK";
                        else if (strpullobj.LotNumberTracking)
                            ItemTracking = "LOTTRACK";
                        else if (strpullobj.DateCodeTracking)
                            ItemTracking = "DATECODETRACK";

                        var Creditobj = {};
                        Creditobj = {
                            'ItemGuid': strpullobj.ItemGUID, 'PullGuid': strpullobj.GUID, 'WOGuid': strpullobj.WorkOrderDetailGUID,
                            'ItemType': strpullobj.ItemType, 'Bin': MainBinNumber,
                            'ProjectName': strpullobj.ProjectName, 'Quantity': parseFloat(SpanQty.val().toString()),
                            'UDF1': strpullobj.UDF1, 'UDF2': strpullobj.UDF2, 'UDF3': strpullobj.UDF3,
                            'UDF4': strpullobj.UDF4, 'UDF5': strpullobj.UDF5, 'ItemTracking': ItemTracking,
                            'PrevPullQty': 0, 'ItemNumber': strpullobj.ItemNumber, 'PullOrderNumber': strpullobj.PullOrderNumber,
                            'SupplierAccountGuid': strpullobj.SupplierAccountGuid,
                            'PrevPullsToCredit': creditarrObj
                        };

                        gblPreCreditObjToSaveForCredit.push(Creditobj);
                    });

                    if (gblPreCreditObjToSaveForCredit != null && gblPreCreditObjToSaveForCredit.length > 0) {                        
                        setTimeout(function () {
                            SavePullCreditEdit(gblPreCreditObjToSaveForCredit)
                        }, 100);
                    }
                }
                else {
                    alert(ErrorMessage);
                }
            });

            $("#divPreCreditInforSerialLotEdit").off("click", "input[type='button'][name='btnPullAllPopUpNew']");
            $("#divPreCreditInforSerialLotEdit").on("click", "input[type='button'][name='btnPullAllPopUpNew']", function () {                
                var ArrItem = new Array();
                var arrItemDetails;
                var ErrorMessage = ValidateAllPullForCreditEdit();
                gblPreCreditObjToSaveForCredit = new Array();
                if (ErrorMessage == "") {
                    $("#divPreCreditInforSerialLotEdit").find("table[id^='tblItemPullheader']").each(function (indx, tblHeader) {
                        var strpullobj = JSON.parse($(tblHeader).find("input[name='hdnPullMasterDTO']").val());
                        var vItemGUID = $(tblHeader).prop("id").split('_')[1];

                        var ArrItem = new Array();
                        var arrItemDetails;

                        var itemGuid = vItemGUID;
                        var SpanQty = $(tblHeader).find("#txtPoolQuantity_" + vItemGUID);
                        var BinID = $(tblHeader).find("#hdnBinID_" + vItemGUID).val();
                        var MainBinNumber = $(tblHeader).find("#hdnMainBinNumber_" + vItemGUID).val();

                        var creditarrObj = new Array();
                        var ItemTracking = "";

                        $("#tblItemPull_" + vItemGUID).find("tbody").find("tr").each(function (index, tr) {

                            var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();

                            var hdnLotNumberTracking = $(tr).find("input[name='hdnLotNumberTracking']").val();
                            var hdnSerialNumberTracking = $(tr).find("input[name='hdnSerialNumberTracking']").val();
                            var hdnDateCodeTracking = $(tr).find("input[name='hdnDateCodeTracking']").val();

                            var hdnBinNumber = $(tr).find("input[name='hdnBinNumber']").val();
                            var hdnExpiration = $(tr).find("input[type='text'][name='txtExpirationDate']").val();
                            var hdnExpirationDate = $(tr).find("input[name='hdnExpirationDate']").val();

                            if (txtPullQty != "") {

                                var txtLotOrSerailNumber = "";
                                if (hdnLotNumberTracking == "true" || hdnSerialNumberTracking == "true")
                                    var txtLotOrSerailNumber = $(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val();

                                var vSerialNumber = "";
                                var vLotNumber = "";
                                var vExpiration = "";
                                var vExpirationDate = "";

                                if (hdnSerialNumberTracking == "true")
                                    vSerialNumber = txtLotOrSerailNumber;
                                if (hdnLotNumberTracking == "true")
                                    vLotNumber = txtLotOrSerailNumber;
                                if (hdnDateCodeTracking == "true") {
                                    vExpiration = hdnExpiration;
                                    vExpirationDate = hdnExpirationDate;
                                }
                            }


                            creditarrObj.push({ 'Serial': $.trim(vSerialNumber), 'Lot': $.trim(vLotNumber), 'ExpireDate': hdnExpiration, 'Qty': parseFloat(txtPullQty.toString()) });

                            if (strpullobj.SerialNumberTracking)
                                ItemTracking = "SERIALTRACK";
                            else if (strpullobj.LotNumberTracking)
                                ItemTracking = "LOTTRACK";
                            else if (strpullobj.DateCodeTracking)
                                ItemTracking = "DATECODETRACK";
                        });

                        var Creditobj = {};
                        Creditobj = {
                            'ItemGuid': strpullobj.ItemGUID, 'PullGuid': strpullobj.GUID, 'WOGuid': strpullobj.WorkOrderDetailGUID,
                            'ItemType': strpullobj.ItemType, 'Bin': MainBinNumber,
                            'ProjectName': strpullobj.ProjectName, 'Quantity': parseFloat(SpanQty.val().toString()),
                            'UDF1': strpullobj.UDF1, 'UDF2': strpullobj.UDF2, 'UDF3': strpullobj.UDF3,
                            'UDF4': strpullobj.UDF4, 'UDF5': strpullobj.UDF5, 'ItemTracking': ItemTracking,
                            'PrevPullQty': 0, 'ItemNumber': strpullobj.ItemNumber, 'PullOrderNumber': strpullobj.PullOrderNumber,
                            'SupplierAccountGuid': strpullobj.SupplierAccountGuid,
                            'PrevPullsToCredit': creditarrObj
                        };

                        gblPreCreditObjToSaveForCredit.push(Creditobj);
                    });

                    if (gblPreCreditObjToSaveForCredit != null && gblPreCreditObjToSaveForCredit.length > 0) {                        
                        setTimeout(function () {
                            SavePullCreditEdit(gblPreCreditObjToSaveForCredit)
                        }, 100);
                    }
                }
                else {
                    alert(ErrorMessage);
                }
            });

            $("#divPreCreditInforSerialLotEdit").off("click", "input[type='button'][name='btnCancelPullPopup']");
            $("#divPreCreditInforSerialLotEdit").on("click", "input[type='button'][name='btnCancelPullPopup']", function () {
                
                gblPreCreditObjToSaveForCredit = null;
                DeletedRowObject = "";
                isDeleteSrLotRowEdit = false;
                $("#divPreCreditInforSerialLotEdit").empty();
                $('#divPreCreditInforSerialLotEdit').dialog('close');
            });

            $("#divPreCreditInforSerialLotEdit").off("change", "input[type='text'][name='txtPullQty']");
            $("#divPreCreditInforSerialLotEdit").on("change", "input[type='text'][name='txtPullQty']", function () {                
                var ids = $(this).parent().parent().parent().parent().parent().parent().parent().parent().find("[id^='hdnPullIds_']").val().split('_');
                var aPos = $("#tblItemPull_" + ids[1].toString()).dataTable().fnGetPosition($(this).parent().parent()[0]);
                $("#tblItemPull_" + ids[1].toString()).dataTable().fnGetData(aPos).PullQuantity = $(this).val();
            });

            $("#divPreCreditInforSerialLotEdit").off("tap click", ".tbl-item-pull tbody tr");
            $("#divPreCreditInforSerialLotEdit").on("tap click", ".tbl-item-pull tbody tr", function (e) {
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

            $("#divPreCreditInforSerialLotEdit").off('click', "input[type='button'][name='btnDeleteLots']");
            $("#divPreCreditInforSerialLotEdit").on('click', "input[type='button'][name='btnDeleteLots']", function (e) {                
                var vItemGUID = $(this).prop("id").split('_')[1];

                var dtID = "#tblItemPull_" + vItemGUID;

                var TotalRows = $(dtID + ' tbody tr').length;
                var SelectedRows = $(dtID + ' tbody tr.row_selected').length;
                var RemainingRows = TotalRows - SelectedRows;

                if (SelectedRows <= 0) {
                    alert(MsgSelectRowToDelete);
                }
                else {
                    if (RemainingRows >= 1) {

                        $(dtID).find("tbody").find("tr.row_selected").each(function (index, tr) {

                            var hdnLotNumberTracking = $(tr).find("input[name='hdnLotNumberTracking']").val();
                            var hdnSerialNumberTracking = $(tr).find("input[name='hdnSerialNumberTracking']").val();
                            var hdnDateCodeTracking = $(tr).find("input[name='hdnDateCodeTracking']").val();
                            var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();

                            if (txtPullQty == "")
                                txtPullQty = "0";

                            if ((hdnLotNumberTracking == "true" && hdnDateCodeTracking == "false")
                                || (hdnSerialNumberTracking == "true" && hdnDateCodeTracking == "false")) {
                                var txtLotOrSerailNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                                if (txtLotOrSerailNumber != undefined && !IsLotSerialExistsInCurrentLoadedForCredit(DeletedRowObject, txtLotOrSerailNumber))
                                    DeletedRowObject = DeletedRowObject + txtLotOrSerailNumber + "_" + txtPullQty + ",";
                            }
                            else if ((hdnLotNumberTracking == "true" && hdnDateCodeTracking == "true")
                                || (hdnSerialNumberTracking == "true" && hdnDateCodeTracking == "true")) {
                                var hdnExpiration = $(tr).find("input[type='text'][name='txtExpirationDate']").val();
                                var txtLotOrSerailNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                                if (txtLotOrSerailNumber != undefined && hdnExpiration != undefined && !IsLotSerialExistsInCurrentLoadedForCredit(DeletedRowObject, hdnExpiration))
                                    DeletedRowObject = DeletedRowObject + txtLotOrSerailNumber + "_" + hdnExpiration + "_" + txtPullQty + ",";
                            }
                            else if (hdnLotNumberTracking == "false" && hdnSerialNumberTracking == "false" && hdnDateCodeTracking == "true") {
                                var hdnExpiration = $(tr).find("input[type='text'][name='txtExpirationDate']").val();
                                if (hdnExpiration != undefined)
                                    DeletedRowObject = DeletedRowObject + hdnExpiration + "_" + txtPullQty + ",";
                            }
                            else {
                                var hdnBinNumber = $(tr).find("input[name='hdnBinNumber']").val();
                                if (hdnBinNumber != undefined && !IsLotSerialExistsInCurrentLoadedForCredit(DeletedRowObject, hdnBinNumber))
                                    DeletedRowObject = DeletedRowObject + hdnBinNumber + "_" + txtPullQty + ",";
                            }
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

                            if ((hdnLotNumberTracking == "true" && hdnDateCodeTracking == "false")
                                || (hdnSerialNumberTracking == "true" && hdnDateCodeTracking == "false")) {
                                var txtLotOrSerailNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                                if (txtLotOrSerailNumber != undefined && !IsLotSerialExistsInCurrentLoadedForCredit(strIds, txtLotOrSerailNumber))
                                    strIds = strIds + txtLotOrSerailNumber + "_" + txtPullQty + ",";
                            }
                            else if ((hdnLotNumberTracking == "true" && hdnDateCodeTracking == "true")
                                || (hdnSerialNumberTracking == "true" && hdnDateCodeTracking == "true")) {
                                var hdnExpiration = $(tr).find("input[type='text'][name='txtExpirationDate']").val();
                                var txtLotOrSerailNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                                if (txtLotOrSerailNumber != undefined && hdnExpiration != undefined && !IsLotSerialExistsInCurrentLoadedForCredit(strIds, hdnExpiration))
                                    strIds = strIds + txtLotOrSerailNumber + "_" + hdnExpiration + "_" + txtPullQty + ",";
                            }
                            else if (hdnLotNumberTracking == "false" && hdnSerialNumberTracking == "false" && hdnDateCodeTracking == "true") {
                                var hdnExpiration = $(tr).find("input[type='text'][name='txtExpirationDate']").val();
                                if (hdnExpiration != undefined)
                                    strIds = strIds + hdnExpiration + "_" + txtPullQty + ",";
                            }
                            else {
                                var hdnBinNumber = $(tr).find("input[name='hdnBinNumber']").val();
                                if (hdnBinNumber != undefined && !IsLotSerialExistsInCurrentLoadedForCredit(strIds, hdnBinNumber))
                                    strIds = strIds + hdnBinNumber + "_" + txtPullQty + ",";
                            }

                        });

                        $("#hdnCurrentDeletedLoadedIds_" + vItemGUID).val(DeletedRowObject);
                        $("#hdnCurrentLoadedIds_" + vItemGUID).val(strIds);
                        isDeleteSrLotRowEdit = true;
                        var dtThisItem = $(dtID).dataTable();
                        dtThisItem.fnStandingRedraw();
                    }
                    else {
                        alert(MsgRowShouldExists);
                    }
                }
            });

            $("#divPreCreditInforSerialLotEdit").off('change', "input.dateTextbox");
            $("#divPreCreditInforSerialLotEdit").on('change', "input.dateTextbox", function (e) {    
                
                var objCurtxt = $(this);
                var oldValue = $(objCurtxt).val();

                //var ids = $(this).parent().parent().parent().find("input[type='hidden'][name='hdnRowUniqueId']").val().split('_');
                var ids = $(this).parent().parent().parent().parent().parent().parent().parent().parent().parent().find("[id^='hdnPullIds_']").val().split('_');
                var aPos = $("#tblItemPull_" + ids[1].toString()).dataTable().fnGetPosition($(this).parent().parent()[0]);
                var aData = $("#tblItemPull_" + ids[1].toString()).dataTable().fnGetData(aPos);

                var dtThisItem = $("#tblItemPull_" + ids[1].toString()).dataTable();
                var currentTR = $(objCurtxt).parent().parent()[0];
                var row_id = dtThisItem.fnGetPosition(currentTR);

                var CurrentSelectedLotNumberValue = "";
                var CurrentSelectedLotNumber = $(currentTR).find('#txtLotOrSerailNumber');

                if ($.trim(oldValue) == '')
                    return;

                if ($("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "LOTTDATECODERACK") {
                    if (CurrentSelectedLotNumber != undefined) {
                        CurrentSelectedLotNumberValue = $(CurrentSelectedLotNumber).val();
                    }
                }

                $(currentTR).find("input[type='text'][name='txtExpirationDate']").val($(objCurtxt).val());

                var CurrentSelectedExpirationDate = $(currentTR).find("input[type='text'][name='txtExpirationDate']").val();


                var BinID = 0;
                if ($("#hdnBinIDValue").length > 0) {
                    BinID = $("#hdnBinIDValue").val();
                }

                var isDuplicateEntry = false;
                $("#tblItemPull_" + ids[1].toString() + " tbody tr").each(function (i) {

                    if ($("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "LOTTDATECODERACK") {
                        if (i != row_id && CurrentSelectedLotNumberValue != "") {
                            var tr = $(this);
                            var SelectedExpirationDate = $(tr).find('#' + objCurtxt.prop("id")).val();
                            var SelectedLotNumber = $(tr).find('#txtLotOrSerailNumber').val();
                            if (SelectedExpirationDate == $(objCurtxt).val() && SelectedLotNumber == CurrentSelectedLotNumberValue) {
                                isDuplicateEntry = true;
                            }
                        }
                    }
                    else if ($("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "DateCodeTracking") {
                        if (i != row_id) {
                            var tr = $(this);
                            var SelectedExpirationDate = $(tr).find('#' + objCurtxt.prop("id")).val();
                            if (SelectedExpirationDate == $(objCurtxt).val()) {
                                isDuplicateEntry = true;
                            }
                        }
                    }
                });

                if (isDuplicateEntry == true) {

                    if ($("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "LOTTDATECODERACK")
                        alert(MsgDuplicateLotNumberExpirationDate);
                    else if ($("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "DateCodeTracking") {
                        alert(MsgDuplicateExpirationDate);
                    }

                    $(objCurtxt).val("");
                    $(objCurtxt).focus();
                }
                else {
                    if (CurrentSelectedLotNumberValue != "" && $("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "LOTTDATECODERACK")

                        $.ajax({
                            type: "POST",
                            url: _PullMasterList.urls.ValidateLotDateCodeForCreditUrl,
                            contentType: 'application/json',
                            dataType: 'json',
                            data: "{ ItemGuid: '" + ids[1].toString() + "', LotNumber: '" + CurrentSelectedLotNumberValue + "', ExpirationDate: '" + $.trim($(objCurtxt).val()) + "',BinID: '" + BinID + "' }",
                            success: function (result) {
                                if (result.IsSerailAvailableForCredit == "false" || result.IsSerailAvailableForCredit == " False" || result.IsSerailAvailableForCredit == false) {
                                    alert(MsgLotNumberExpDateValidation);
                                    $(objCurtxt).val("");
                                    $(objCurtxt).focus();
                                }
                            },
                            error: function (err) {
                                console.log(err);
                            }
                        });
                }
            });

            $("#divPreCreditInforSerialLotEdit").off('focus', "input.dateTextbox");
            $("#divPreCreditInforSerialLotEdit").on('focus', "input.dateTextbox", function (e) {                
                var objCurtxt = $(this);
                var ids = $(this).parent().parent().parent().parent().parent().parent().parent().parent().parent().find("[id^='hdnPullIds_']").val().split('_');
                var aPos = $("#tblItemPull_" + ids[1].toString()).dataTable().fnGetPosition($(this).parent().parent()[0]);
                var aData = $("#tblItemPull_" + ids[1].toString()).dataTable().fnGetData(aPos);

                var dtItemPull = "#tblItemPull_" + ids[1].toString();
                var strSerialLotNos = "";

                $(dtItemPull).find("tbody").find("tr").each(function (index, tr) {

                    if (index != aPos) {
                        var hdnLotNumberTracking = $(tr).find("input[name='hdnLotNumberTracking']").val();
                        var hdnSerialNumberTracking = $(tr).find("input[name='hdnSerialNumberTracking']").val();
                        var hdnDateCodeTracking = $(tr).find("input[name='hdnDateCodeTracking']").val();

                        if (hdnLotNumberTracking == "true" || hdnSerialNumberTracking == "true") {
                            var txtLotOrSerailNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                            if (txtLotOrSerailNumber != undefined)
                                strSerialLotNos = strSerialLotNos + txtLotOrSerailNumber + "|#|";
                        }
                        if (hdnDateCodeTracking == "true") {
                            var hdnExpiration = $(tr).find("input[type='text'][name='txtExpirationDate']").val();
                            if (hdnExpiration != undefined)
                                strSerialLotNos = strSerialLotNos + hdnExpiration + "|#|";
                        }
                        else if (hdnLotNumberTracking == "false" && hdnSerialNumberTracking == "false") {
                            var hdnBinNumber = $(tr).find("input[name='hdnBinNumber']").val();
                            if (hdnBinNumber != undefined)
                                strSerialLotNos = strSerialLotNos + hdnBinNumber + "|#|";
                        }
                    }

                });
            });

            $("#divPreCreditInforSerialLotEdit").off('change', "input[type='text'][name^='txtLotOrSerailNumber']");
            $("#divPreCreditInforSerialLotEdit").on('change', "input[type='text'][name^='txtLotOrSerailNumber']", function (e) {                
                
                var objCurtxt = $(this);
                var oldValue = $(objCurtxt).val();
                //var ids = $(this).parent().parent().parent().find("input[type='hidden'][name='hdnRowUniqueId']").val().split('_');
                var ids = $(this).parent().parent().parent().parent().parent().parent().parent().parent().parent().find("[id^='hdnPullIds_']").val().split('_');
                var aPos = $("#tblItemPull_" + ids[1].toString()).dataTable().fnGetPosition($(this).parent().parent().parent()[0]);
                var aData = $("#tblItemPull_" + ids[1].toString()).dataTable().fnGetData(aPos);

                var dtThisItem = $("#tblItemPull_" + ids[1].toString()).dataTable();
                var currentTR = $(objCurtxt).parent().parent().parent()[0];
                var row_id = dtThisItem.fnGetPosition(currentTR);

                var CurrentSelectedExpirationDate = $(currentTR).find("input[type='text'][name='txtExpirationDate']").val();

                if ($.trim(oldValue) == '')
                    return;


                var BinID = 0;
                if ($("#hdnBinIDValue").length > 0) {
                    BinID = $("#hdnBinIDValue").val();
                }

                var isDuplicateEntry = false;
                var OtherPullQuantity = 0;

                $("#tblItemPull_" + ids[1].toString() + " tbody tr").each(function (i) {

                    if ($("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "LOTTDATECODERACK") {
                        if (i != row_id && $(objCurtxt).val() != "") {
                            var tr = $(this);
                            var SelectedExpirationDate = $(tr).find("input[type='text'][name='txtExpirationDate']").val();
                            var SelectedLotNumber = $(tr).find('#' + objCurtxt.prop("id")).val();
                            if (SelectedExpirationDate != "") {
                                if (SelectedExpirationDate == CurrentSelectedExpirationDate && SelectedLotNumber == $(objCurtxt).val()) {
                                    isDuplicateEntry = true;
                                }
                            }
                        }
                    }
                    else {
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
                    }
                });

                if (isDuplicateEntry == true) {

                    if ($("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "LOTTDATECODERACK")
                        alert(MsgDuplicateLotNumberExpirationDate);
                    else if ($("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "SERIALDATECODETRACK")
                        alert(MsgDuplicateSerialNumberValidation);
                    else if ($("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "LotNumberTracking")
                        alert(MsgDuplicateLotNumber);
                    else if ($("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "SerialNumberTracking")
                        alert(MsgDuplicateSerialNumberValidation);
                    else if ($("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "DateCodeTracking") {
                        alert(MsgDuplicateExpirationDate);
                    }

                    $(objCurtxt).val("");
                    $(objCurtxt).focus();
                }
                else {
                    if ($("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "SerialNumberTracking" ||
                        $("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "SERIALDATECODETRACK") {

                        $.ajax({
                            type: "POST",
                            url: _PullMasterList.urls.ValidateSerialNumberForCreditUrl,
                            contentType: 'application/json',
                            dataType: 'json',
                            data: "{ ItemGuid: '" + ids[1].toString() + "', SerialNumber: '" + $.trim($(objCurtxt).val()) + "',BinID: '" + BinID + "' }",
                            success: function (result) {
                                if (result.IsSerailAvailableForCredit == "false" || result.IsSerailAvailableForCredit == " False" || result.IsSerailAvailableForCredit == false) {
                                    alert(MsgCreditTransactionForSerialNumber);
                                    $(objCurtxt).val("");
                                    $(objCurtxt).focus();
                                }
                            },
                            error: function (err) {
                                console.log(err);
                            }
                        });
                    }
                    else if (CurrentSelectedExpirationDate != "" && $("#hdnTrackingTypeForCreditRule_" + ids[1].toString()).val() == "LOTTDATECODERACK") {

                        $.ajax({
                            type: "POST",
                            url: _PullMasterList.urls.ValidateLotDateCodeForCreditUrl,
                            contentType: 'application/json',
                            dataType: 'json',
                            data: "{ ItemGuid: '" + ids[1].toString() + "', LotNumber: '" + $.trim($(objCurtxt).val()) + "', ExpirationDate: '" + CurrentSelectedExpirationDate + "',BinID: '" + BinID + "' }",
                            success: function (result) {
                                if (result.IsSerailAvailableForCredit == "false" || result.IsSerailAvailableForCredit == " False" || result.IsSerailAvailableForCredit == false) {
                                    alert(MsgLotNumberExpDateValidation);
                                    $(objCurtxt).val("");
                                    $(objCurtxt).focus();
                                }
                            },
                            error: function (err) {
                                console.log(err);
                            }
                        });
                    }
                }
            });

            $("#divPreCreditInforSerialLotEdit").off('focus', "input[type='text'][name^='txtLotOrSerailNumber']");
            $("#divPreCreditInforSerialLotEdit").on('focus', "input[type='text'][name^='txtLotOrSerailNumber']", function (e) {               
                var objCurtxt = $(this);
                //var ids = $(this).parent().parent().parent().find("input[type='hidden'][name='hdnRowUniqueId']").val().split('_');
                var ids = $(this).parent().parent().parent().parent().parent().parent().parent().parent().parent().find("[id^='hdnPullIds_']").val().split('_');
                var aPos = $("#tblItemPull_" + ids[1].toString()).dataTable().fnGetPosition($(this).parent().parent().parent()[0]);
                var aData = $("#tblItemPull_" + ids[1].toString()).dataTable().fnGetData(aPos);

                var dtItemPull = "#tblItemPull_" + ids[1].toString();
                var strSerialLotNos = "";

                var BinID = $("#divPreCreditInforSerialLotEdit").find("#hdnBinID_" + ids[1].toString()).val();
                var strpullobj = JSON.parse($("#hdnPullMasterDTO_" + ids[1].toString()).val());
                var PullGUID = strpullobj.GUID;
                $(dtItemPull).find("tbody").find("tr").each(function (index, tr) {

                    if (index != aPos) {
                        var hdnLotNumberTracking = $(tr).find("input[name='hdnLotNumberTracking']").val();
                        var hdnSerialNumberTracking = $(tr).find("input[name='hdnSerialNumberTracking']").val();
                        var hdnDateCodeTracking = $(tr).find("input[name='hdnDateCodeTracking']").val();

                        if (hdnLotNumberTracking == "true" || hdnSerialNumberTracking == "true") {
                            var txtLotOrSerailNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                            if (txtLotOrSerailNumber != undefined)
                                strSerialLotNos = strSerialLotNos + txtLotOrSerailNumber + "|#|";
                        }
                        if (hdnDateCodeTracking == "true") {
                            var hdnExpiration = $(tr).find("input[type='text'][name='txtExpirationDate']").val();
                            if (hdnExpiration != undefined)
                                strSerialLotNos = strSerialLotNos + hdnExpiration + "|#|";
                        }
                        else if (hdnLotNumberTracking == "false" && hdnSerialNumberTracking == "false") {
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
                                url: '/Pull/GetItemLocationsWithLotSerialsForCreditEdit',
                                contentType: 'application/json',
                                dataType: 'json',
                                data: {
                                    maxRows: 1000,
                                    name_startsWith: request.term,
                                    ItemGuid: ids[1].toString(),
                                    PullGUID: PullGUID,
                                    BinID: BinID,
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

            $("#divPreCreditInforSerialLotEdit").off("click", "input[type='button'][name='btnLoadMoreLots']");
            $("#divPreCreditInforSerialLotEdit").on("click", "input[type='button'][name='btnLoadMoreLots']", function () {
                var vItemGUID = $(this).prop("id").split('_')[1];
                var dtID = "#tblItemPull_" + vItemGUID;
                var strIds = "";

                var BinNumber = "";
                var IsLotNumberTracking = false;
                var IsSerialNumberTracking = false;
                var IsDateCodeTracking = false;

                var MaxQuantity = $("#txtPoolQuantity_" + vItemGUID)[0].value;
                var BinID = $("#hdnBinID_" + vItemGUID)[0].value;
                var CurrentDate = $("#hdnCurrentDate_" + vItemGUID)[0].value;

                var TotalQuantity = 0;
                $("#tblItemPull_" + vItemGUID).find("[id*='txtPullQty_']").each(function () {
                    TotalQuantity = TotalQuantity + parseInt($(this)[0].value);
                });

                var isError = false;

                if (MaxQuantity > TotalQuantity) {
                    IsLoadMoreLotsClicked = true;
                    $(dtID).find("tbody").find("tr").each(function (index, tr) {

                        BinNumber = $(tr).find("input[name='hdnBinNumber']").val();
                        var hdnLotNumberTracking = $(tr).find("input[name='hdnLotNumberTracking']").val();
                        var hdnSerialNumberTracking = $(tr).find("input[name='hdnSerialNumberTracking']").val();
                        var hdnDateCodeTracking = $(tr).find("input[name='hdnDateCodeTracking']").val();
                        var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();


                        if (hdnLotNumberTracking == "true")
                            IsLotNumberTracking = true;
                        if (hdnSerialNumberTracking == "true")
                            IsSerialNumberTracking = true;
                        if (hdnDateCodeTracking == "true")
                            IsDateCodeTracking = true;

                        if (IsLotNumberTracking == true) {
                            var txtLotNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                            if (txtLotNumber == "" || txtLotNumber == null) {
                                alert(MsgEnterLotNumber);
                                isError = true;
                                return false;
                            }
                        }
                        if (IsSerialNumberTracking == true) {
                            var txtSerailNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                            if (txtSerailNumber == "" || txtSerailNumber == null) {
                                alert(MsgSerialNumberValidation);
                                isError = true;
                                return false;
                            }
                        }
                        if (IsDateCodeTracking == true) {
                            var txtExpiration = $.trim($(tr).find("input[type='text'][name^='txtExpirationDate']").val());
                            if (txtExpiration == "" || txtExpiration == null) {
                                alert(MsgEnterExpirationDate);
                                isError = true;
                                return false;
                            }
                        }

                        if (txtPullQty != undefined) {
                            if (txtPullQty == "") {
                                txtPullQty = "0";
                            }
                            if ((hdnLotNumberTracking == "true" && hdnDateCodeTracking == "false")
                                || (hdnSerialNumberTracking == "true" && hdnDateCodeTracking == "false")) {
                                var txtLotOrSerailNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                                if (txtLotOrSerailNumber != undefined && !IsLotSerialExistsInCurrentLoadedForCredit(strIds, txtLotOrSerailNumber))
                                    strIds = strIds + txtLotOrSerailNumber + "_" + txtPullQty + ",";
                            }
                            else if ((hdnLotNumberTracking == "true" && hdnDateCodeTracking == "true")
                                || (hdnSerialNumberTracking == "true" && hdnDateCodeTracking == "true")) {
                                var hdnExpiration = $(tr).find("input[type='text'][name='txtExpirationDate']").val();
                                var txtLotOrSerailNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                                if (txtLotOrSerailNumber != undefined && hdnExpiration != undefined && !IsLotSerialExistsInCurrentLoadedForCredit(strIds, hdnExpiration))
                                    strIds = strIds + txtLotOrSerailNumber + "_" + hdnExpiration + "_" + txtPullQty + ",";
                            }
                            else if (hdnLotNumberTracking == "false" && hdnSerialNumberTracking == "false" && hdnDateCodeTracking == "true") {
                                var hdnExpiration = $(tr).find("input[type='text'][name='txtExpirationDate']").val();
                                if (hdnExpiration != undefined)
                                    strIds = strIds + hdnExpiration + "_" + txtPullQty + ",";
                            }
                            else {
                                var hdnBinNumber = $(tr).find("input[name='hdnBinNumber']").val();
                                if (hdnBinNumber != undefined && !IsLotSerialExistsInCurrentLoadedForCredit(strIds, hdnBinNumber))
                                    strIds = strIds + hdnBinNumber + "_" + txtPullQty + ",";
                            }
                        }
                    });

                    if (isError == false) {
                        $("#hdnCurrentLoadedIds_" + vItemGUID).val(strIds);
                        var dt = $(dtID).dataTable();
                        dt.fnStandingRedraw();
                    }
                }
                else {
                    alert(MsgPullCreditQuantity);
                }
            });

    /* For Edit Credit for Lot,serial and datecode end */

        }); // ready
    };

    self.grid_fnStateSaveParams = function (oSettings, oData) {
        oData.oSearch.sSearch = "";
        //if (PostCount > 1) {
        if (self.isSaveGridState) {

            _AjaxUtil.postJson(self.urls.saveGridStateURL
                , { Data: JSON.stringify(oData), ListName: 'PullMaster' }
                , function (json) {
                    o = json;
                }, null
                , true, false);

            //$.ajax({
            //    "url": self.urls.saveGridStateURL,
            //    "type": "POST",
            //    data: { Data: JSON.stringify(oData), ListName: 'PullMaster' },
            //    "async": false,
            //    cache: false,
            //    "dataType": "json",
            //    "success": function (json) {
            //        o = json;
            //    }
            //});
        }
        else {
            self.isSaveGridState = true;
        }
        //}
    };

    self.grid_fnServerData = function (sSource, aoData, fnCallback, oSettings) {

        var $myDataTable = $("#myDataTable");

        //PostCount = PostCount + 1;
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

        if (oSettings.aaSorting != null && oSettings.aaSorting.length != 0) {
            var sortValue = "";
            for (var i = 0; i <= oSettings.aaSorting.length - 1; i++) {
                if (sortValue.length > 0)
                    sortValue += ", "
                sortValue += arrCols[oSettings.aaSorting[i][0]] + ' ' + oSettings.aaSorting[i][1];
            }
            aoData.push({ "name": "SortingField", "value": sortValue });
        }
            //                    aoData.push({ "name": "SortingField", "value": oSettings.aaSorting[0][3] });
        else
            aoData.push({ "name": "SortingField", "value": "0" });

        aoData.push({ "name": "IsArchived", "value": $('#IsArchivedRecords').is(':checked') });
        aoData.push({ "name": "IsDeleted", "value": $('#IsDeletedRecords').is(':checked') });

        
        if (_Common.selectedGridOperation === _Common.gridOperations.Search
            || _Common.selectedGridOperation === _Common.gridOperations.IncludeDeleted
            || _Common.selectedGridOperation === _Common.gridOperations.IncludeArchived
            || _Common.selectedGridOperation === _Common.gridOperations.AutoRefresh
            || _Common.selectedGridOperation === _Common.gridOperations.PageChange
        ) {
            // prevent api calls
            //self.isGetReplinshRedCount = false;
            //self.isGetUDFEditableOptionsByUDF = false;
            self.isSaveGridState = false;
        }
        else if (_Common.selectedGridOperation === _Common.gridOperations.PageSizeChange) {
            //self.isGetReplinshRedCount = false;
            //self.isGetUDFEditableOptionsByUDF = false;
            self.isSaveGridState = true;
        }
        else if (_Common.selectedGridOperation === _Common.gridOperations.Sorting
            || _Common.selectedGridOperation === _Common.gridOperations.ColumnResize
        ) {
            //self.isGetReplinshRedCount = false;
            //self.isGetUDFEditableOptionsByUDF = false;
            self.isSaveGridState = true;
        }
        else if (_Common.selectedGridOperation === _Common.gridOperations.Refresh) {
            //self.isGetReplinshRedCount = true;
            //self.isGetUDFEditableOptionsByUDF = false;
            self.isSaveGridState = false;
        }

        self.gridRowStartIndex = null;
        rowCallbackCount = 0;
        oSettings.jqXHR = $.ajax({
            "dataType": 'json',
            "type": "POST",
            "url": sSource,
            cache: false,
            "data": aoData,
            "headers": { "__RequestVerificationToken": $("input[name='__RequestVerificationToken'][type='hidden']").val() },
            "success": fnCallback,
            beforeSend: function () {
                $myDataTable.removeHighlight();
                $('.dataTables_scroll').css({ "opacity": 0.2 });
            },
            complete: function () {
                
                //$('.dataTables_scroll').css({ "opacity": 1 });
                setTimeout(function () { $('.dataTables_scroll').css({ "opacity": 1 }); }, 100);
                if ($("#global_filter").val() != '') {
                    $myDataTable.highlight($("#global_filter").val());
                }

                $(".text-boxPriceFormat").priceFormat({
                    prefix: '',
                    thousandsSeparator: '',
                    centsLimit: parseInt($('#hdCostcentsLimit').val(), 10)
                });


                $myDataTable.off("mouseenter");
                UDFfillEditableOptionsForGrid_PullMaster();
                PONfillEditableOptionsForGrid_PullMaster();

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
                SetUDFAndPOSelected(oSettings);
             
            }
        });
    };

    self.initEvents = function () {
        $(document).ready(function () {
            $("#actionSelectAll").click(function () {
                var IsDeleteBilling = false;
                $('#' + myDataTableId +' tbody tr').each(function () {
                    if ($(this)[0].style.backgroundColor == "red") {
                        $(this).removeClass('row_selected');
                        IsDeleteBilling = true;
                    }
                })
                if (IsDeleteBilling) {
                    $('div#target').fadeToggle();
                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                    $("#spanGlobalMessage").html(MsgBillingDoneDeleteValidation);
                    $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
                }
            });
            $("#LocationDetails").dialog({
                autoOpen: false,
                show: "blind",
                hide: "explode",
                height: 700,
                title: "Item Locations",
                width: 900,
                modal: true,
                beforeClose: function () {
                    if (isDirtyForm) {
                        if (confirm(SaveConfirmationMSG)) {
                            //isDirtyForm = false;
                            return false;
                        }
                        isDirtyForm = false;
                    }
                },
                close: function () {

                }
            });

            /* HISTORY related data deleated and archived START */
            $('#IsDeletedRecords').live('click', function () {

                NarrowSearchInGrid('');
            });
            $('#IsArchivedRecords').live('click', function () {
                NarrowSearchInGrid('');
            });

            $('#ViewHistory').live('click', function () {
                HistorySelected = fnGetSelected(oTable);
                if (HistorySelected != undefined && HistorySelected.length == 1) {
                    $("#atab5").click();
                }
                else {
                    $('#tab5').html('');
                    $("#spanGlobalMessage").html(msgSelectForViewHistory);
                    $('div#target').fadeToggle();
                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                    return false;
                }
            });

            if (window.location.href.toString().toLowerCase().indexOf("newpull") > 0) {
                TabEnableDisable("#tab1", false);
                TabEnableDisable("#tab5", true);
                $("#tab1").removeClass("unselected").addClass("selected");
                $("#tab5").removeClass("selected").addClass("unselected");
            }
            else if (window.location.href.toString().toLowerCase().indexOf("pullmasterlist") > 0) {
                TabEnableDisable("#tab1", true);
                TabEnableDisable("#tab5", false);
                $("#tab5").removeClass("unselected").addClass("selected");
                $("#tab1").removeClass("selected").addClass("unselected");
            }

            $("#PopupSetBilling").dialog({
                autoOpen: false,
                show: "blind",
                hide: "explode",
                height: 700,
                title: MsgPullMaster,
                width: 900,
                modal: true,
                beforeClose: function () {
                    if (isDirtyForm) {
                        if (confirm(SaveConfirmationMSG)) {
                            return false;
                        }
                        isDirtyForm = false;
                    }
                },
                close: function () {
                    IsRefreshGrid = true;
                    $('#DivLoading').hide();
                    $("#PopupSetBilling").empty();
                    $('#myDataTable').dataTable().fnStandingRedraw();
                }
            });


        });// ready
    };

    self.initUrls = function (saveGridStateURL, loadGridStateUrl
        , pullMasterListAjaxUrl, getUDFOptionByIDUrl
        , getQTYFromLocationAndItemUrl, pullDetailsUrl
        , newPullUrl, pullMasterListUrl
        , setBillingForPullMasterUrl, updateUDFInPullHistoryUrl
        , updatePullOrderNumberInPullHistoryUrl
        , UpdatePullQtyInPullHistoryUrl
        , PullItemEditQuantityUrl
        , PullLotSrSelectionDetailsEditUrl
        , ValidateSerialLotNumberForEditUrl
        , ValidateLotDateCodeForCreditUrl
        , ValidateSerialNumberForCreditUrl
        , GetLotOrSerailNumberListForEditUrl
        , PullSerialsAndLotsNewEditUrl
        , PullLotSrSelectionForCreditEditPopupUrl
        , SavePullCreditEditUrl
        , GetPullOrderNumberUrl
    ) {
        self.urls.saveGridStateURL = saveGridStateURL;
        self.urls.loadGridStateUrl = loadGridStateUrl;
        self.urls.pullMasterListAjaxUrl = pullMasterListAjaxUrl;
        self.urls.getUDFOptionByIDUrl = getUDFOptionByIDUrl;
        self.urls.getQTYFromLocationAndItemUrl = getQTYFromLocationAndItemUrl;
        self.urls.pullDetailsUrl = pullDetailsUrl;
        self.urls.newPullUrl = newPullUrl;
        self.urls.pullMasterListUrl = pullMasterListUrl;
        self.urls.setBillingForPullMasterUrl = setBillingForPullMasterUrl;
        self.urls.updateUDFInPullHistoryUrl = updateUDFInPullHistoryUrl;
        self.urls.updatePullOrderNumberInPullHistoryUrl = updatePullOrderNumberInPullHistoryUrl;
        self.urls.UpdatePullQtyInPullHistoryUrl = UpdatePullQtyInPullHistoryUrl;
        self.urls.PullItemEditQuantityUrl = PullItemEditQuantityUrl;
        self.urls.PullLotSrSelectionDetailsEditUrl = PullLotSrSelectionDetailsEditUrl;
        self.urls.ValidateSerialLotNumberForEditUrl = ValidateSerialLotNumberForEditUrl;
        self.urls.ValidateLotDateCodeForCreditUrl = ValidateLotDateCodeForCreditUrl;
        self.urls.ValidateSerialNumberForCreditUrl = ValidateSerialNumberForCreditUrl;
        self.urls.GetLotOrSerailNumberListForEditUrl = GetLotOrSerailNumberListForEditUrl;
        self.urls.PullSerialsAndLotsNewEditUrl = PullSerialsAndLotsNewEditUrl;
        self.urls.PullLotSrSelectionForCreditEditPopupUrl = PullLotSrSelectionForCreditEditPopupUrl;
        self.urls.SavePullCreditEditUrl = SavePullCreditEditUrl;
        self.urls.GetPullOrderNumberUrl = GetPullOrderNumberUrl;
    };


    // private function
    function fnFormatDetails(oTable, nTr) {

        var oData = oTable.fnGetData(nTr);
        ItemName = oData.ItemNumber;
        ItemUniqueID = oData.GUID;
        var sOut = '';
        $('#DivLoading').show();

        _AjaxUtil.getText(_PullMasterList.urls.pullDetailsUrl
            ,{ ItemID: oData.GUID }
            , function (json) {
                sOut = json;
                oTable.fnOpen(nTr, json, '');
                $('#DivLoading').hide();
            }
            , function (response) {

            },true,false);

        //$.ajax({
        //    "url": _PullMasterList.urls.pullDetailsUrl,
        //    data: { ItemID: oData.GUID },
        //    "async": false,
        //    cache: false,
        //    "dataType": "text",
        //    "success": function (json) {
        //        sOut = json;
        //        $('#DivLoading').hide();
        //    },
        //    error: function (response) {

        //    }
        //});

        return sOut;
    }

    return self;
})(jQuery);

/////////

function CreditItems(RowObject) {
    var itemId = $(RowObject).parent().parent().find('#spnItemID').text();
    var tempItemType = $(RowObject).parent().parent().find('#spnOrderItemType').text();

    var idtype = itemId + '%23' + tempItemType + '%23' + 'frompullcredit';

    $('#DivLoading').show();
    $('#LocationDetails').load('../Inventory/LocationDetails?ItemID_ItemType=' + idtype, function () {
        $('#LocationDetails').dialog('open');
        $('#DivLoading').hide();
    });
    return false;


}

function OpenDialog(RowObject, event) {
    var spnitemnumber = $(RowObject).parent().find('#spnItemNumber').text();
    $("#lblItemData").text(spnitemnumber);
    var spndefaultqtn = $(RowObject).parent().find('#spnDefaultQnt').text();
    $("#lblDefaultQtn").text(spndefaultqtn);

    $("#lblPulledQtn").text(RowObject.innerHTML);

    GlobalPullQuantity = RowObject;

    GlobalIsCreditPull = $(RowObject).parent().find('#spnActionType');

    GlobalTempPulledQTY = $(RowObject).parent().find('#spnTempPullQTY');

    //            if ($(RowObject).parent().find('#spnDefaultPullQnt').text() == null || $(RowObject).parent().find('#spnDefaultPullQnt').text() == 'null')
    //                $("#txtPullQuantity").val(0);
    //            else
    //                $("#txtPullQuantity").val($(RowObject).parent().find('#spnDefaultPullQnt').text());

    var tempBinID = $(RowObject).parent().parent().find('#BinID option:selected')[0].value == "" ? 0 : $(RowObject).parent().parent().find('#BinID option:selected')[0].value;
    var tempItemID = $(RowObject).parent().parent().find('#spnItemID').text();
    var tempBinName = $(RowObject).parent().parent().find('#BinID option:selected')[0].text == "" ? "" : $(RowObject).parent().parent().find('#BinID option:selected')[0].text;
    if (tempBinID > 0 && tempItemID > 0) {
        // find out available quantity for this bin location
        $.ajax({
            url: _PullMasterList.urls.getQTYFromLocationAndItemUrl,
            data: { BindID: tempBinID, ItemID: tempItemID },
            dataType: 'json',
            type: 'POST',
            async: false,
            cache: false,
            success: function (response) {
                $("#lbldialogAvalLocation").text(MsgAvailableQtyText.replace("{0}", tempBinName));
                $("#lbldialogAvalLocationQTY").text(response.AvalQTY);
                $("#dialog-form").dialog("open");
            },
            error: function (response) {

            }
        });
    }
    else {
        $("#spanGlobalMessage").html(MsgSelectLocationPullCreditValidation);
        $('div#target').fadeToggle();
        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
        $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
    }
}
function SetBindings() {


}
function CreditQuantity() {
    var MainQnt = parseInt($("#lblPulledQtn").text(), 10);
    var PullQnt = parseInt($("#txtPullQuantity").val(), 10);
    var DefQnt = parseInt($("#lblDefaultQtn").text(), 10);

    var NewQnt = MainQnt - PullQnt;

    if (NewQnt < 0) {
        $("#spanGlobalMessage").html('Not enough quantity to CREDIT');
        $('div#target').fadeToggle();
        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
        $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
    }
    else {
        GlobalTempPulledQTY.text(PullQnt);
        $("#lblPulledQtn").text(NewQnt)
        GlobalIsCreditPull.text("credit");
        IsSaveRequired = true;
        GlobalPullQuantity.innerHTML = $("#lblPulledQtn").text();
        $("#dialog-form").dialog("close");
    }
}
function PullQuantity() {
    var MainQnt = parseInt($("#lblPulledQtn").text(), 10);
    var PullQnt = parseInt($("#txtPullQuantity").val(), 10);
    var DefQnt = parseInt($("#lbldialogAvalLocationQTY").text(), 10); //parseInt($("#lblDefaultQtn").text(), 10);
    var NewQnt = MainQnt + PullQnt;

    if (PullQnt <= DefQnt) {
        GlobalTempPulledQTY.text(PullQnt);
        $("#lblPulledQtn").text(NewQnt)
        GlobalIsCreditPull.text("pull");
        IsSaveRequired = true;
        GlobalPullQuantity.innerHTML = $("#lblPulledQtn").text();
        $("#dialog-form").dialog("close");
    }
    else {
        $("#spanGlobalMessage").html(MsgPullQuantitySelectedLocation);
        $('div#target').fadeToggle();
        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
        $("#spanGlobalMessage").removeClass('succesIcon errorIcon').addClass('WarningIcon');
    }
}
function onlyNumeric(event) {
    var charCode = (event.which) ? event.which : event.keyCode
    if (charCode > 31 && (charCode < 48 || charCode > 57 || code == 86))
        return false;

    return true;
}

// needs to correct below code ...




/* HISTORY related data deleated and archived START */
function fnGetSelected(oTableLocal) {
    return oTableLocal.$('tr.row_selected');
}
function HistoryTabClick() {
    GetHistoryData();
}
function GetHistoryData() {
    HistorySelected = fnGetSelected(oTable);
    if (HistorySelected != undefined && HistorySelected.length == 1) {
        //                if(SelectedHistoryRecordID != HistorySelected[0].id)
        //                {
        var hdnPullID = $(HistorySelected).find('#hdnID')[0].value;
        SelectedHistoryRecordID = hdnPullID;
        $('#DivLoading').show();
        $("#CTab").hide();
        $("#CtabCL").show();
        $('#CtabCL').load('/Master/PullHistory', function () { $('#DivLoading').hide(); });
        //}
    }
    else {
        $('#CtabCL').html('');
        $("#spanGlobalMessage").html(msgSelectForViewHistory);
        $('div#target').fadeToggle();
        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
        return false;
    }
}

/* HISTORY related data deleated and archived END */

/* Cost Narrow Search Related Code  START */
function CostNarroSearch(CostDDLObject) {
    //oTable.fnFilter($(CostDDLObject).val(),null,null,null);
    if ($(CostDDLObject).val() != "0_-1") {
        CostNarroSearchValue = $(CostDDLObject).val();
        _NarrowSearchSave.objSearch.PullCost = CostNarroSearchValue;
        DoNarrowSearch();
    }
    else {
        CostNarroSearchValue = '';
        _NarrowSearchSave.objSearch.PullCost = CostNarroSearchValue;
        DoNarrowSearch();
    }
}

function BillingNarroSearch(BillingDDLObject) {
    //oTable.fnFilter($(CostDDLObject).val(),null,null,null);
    if ($(BillingDDLObject).val() != "") {
        IsBillingNarroSearchValue = $(BillingDDLObject).val();
        _NarrowSearchSave.objSearch.isBilling = IsBillingNarroSearchValue;
        DoNarrowSearch();
    }
    else {
        IsBillingNarroSearchValue = '';
        _NarrowSearchSave.objSearch.isBilling = IsBillingNarroSearchValue;
        DoNarrowSearch();
    }
}

function EDISentNarroSearch(EDISentDDLObject) {
    //oTable.fnFilter($(CostDDLObject).val(),null,null,null);
    if ($(EDISentDDLObject).val() != "") {
        IsEDISentNarroSearchValue = $(EDISentDDLObject).val();
        _NarrowSearchSave.objSearch.isEDISent = IsEDISentNarroSearchValue;
        DoNarrowSearch();
    }
    else {
        IsEDISentNarroSearchValue = '';
        _NarrowSearchSave.objSearch.isEDISent = IsEDISentNarroSearchValue;
        DoNarrowSearch();
    }
}

/* Cost Narrow Search Related Code  END */
function callbacknew() {
    window.location = _PullMasterList.urls.newPullUrl;
    return false;
    //ShowNewTab('NewPull', 'frmPullMaster');
    //$('#CtabNew').html('');
    //if (IsEditMode) {
    //    IsEditMode = false;
    //    return;
    //}
    //AllowDeletePopup = false;

    //if ($('#CtabNew :input').length == 0) {
    //    $('#DivLoading').show();
    //    $('frmPullMaster').append($('#CtabNew').load('NewPull', function () { $('#DivLoading').hide(); $("#" + 'frmPullMaster' + " :input:text:visible:first").focus(); }));
    //    $.validator.unobtrusive.parseDynamicContent('#' + 'frmPullMaster' + ' input:last');
    //    if (window.location.href.lastIndexOf('#new') < 1) {
    //        var url = window.location.href;
    //        if (url.lastIndexOf('#list') > 0) {
    //            url = url.replace("#list", '');
    //        }
    //        window.location.href = url + '#new';
    //    }
    //}
    //else {
    //    oTableItemModel.fnDraw();
    //    if (window.location.href.lastIndexOf('#new') < 1) {
    //        var url = window.location.href;
    //        if (url.lastIndexOf('#list') > 0) {
    //            url = url.replace("#list", '');
    //        }
    //        window.location.href = url + '#new';
    //    }

    //}
}
function callbackhistory() {
    //hide undelete button when only deleted is unchecked
    if ($('#IsDeletedRecords').is(':checked')) {
        $('#undeleteRows').css('display', '');
        $('#deleteRows').css('display', 'none');
    }
    else {
        $('#undeleteRows').css('display', 'none');
        $('#deleteRows').css('display', '');
    }
    var QueryStringParam = _Common.getParameterByName('IsRefresh');

    if (QueryStringParam != '' && QueryStringParam == 'yes') {
        var urlRedFor = _PullMasterList.urls.pullMasterListUrl;
        window.location.href = urlRedFor;
    }
    else {
        //oTable.fnDraw();

        var urlRedFor = window.location.href.replace('#new', '');
        window.location.href = urlRedFor;
    }
} //HistoryTabClick(); }
//  function callbackCL() { HistoryTabClick(); }

function SetUDFAndPOSelected(objParams) {

    $("#" + objParams.sInstance).find("tbody").find("tr").each(function () {

        var binId = $(this).find("input[name='hdnBinID']").val();
        //            $(this).find("#slctBinName").val(binId);

        var $objUdf1 = $(this).find("#UDF1");
        var $spnUDF1 = $(this).find("#hdnUDF1");
        var $objUdf2 = $(this).find("#UDF2");
        var $spnUDF2 = $(this).find("#hdnUDF2");
        var $objUdf3 = $(this).find("#UDF3");
        var $spnUDF3 = $(this).find("#hdnUDF3");
        var $objUdf4 = $(this).find("#UDF4");
        var $spnUDF4 = $(this).find("#hdnUDF4");
        var $objUdf5 = $(this).find("#UDF5");
        var $spnUDF5 = $(this).find("#hdnUDF5");

        var $vPullOrderNumber = $(this).find("#txtPullOrderNumber");
        var $spnPullOrderNumber = $(this).find("#hdnPullOrderNumber");

        if ($objUdf1 != undefined && $spnUDF1 != undefined) {
            if ($objUdf1.is("select")) {
                $objUdf1.find("option").filter(function () {
                    return this.text == $spnUDF1.val();
                }).attr('selected', true);
            }
            else if ($objUdf1.is("input[type='text']")) {
                $objUdf1.val($spnUDF1.val());
            }
        }
        if ($objUdf2 != undefined && $spnUDF2 != undefined) {
            if ($objUdf2.is("select")) {
                $objUdf2.find("option").filter(function () {
                    return this.text == $spnUDF2.val();
                }).attr('selected', true);
            }
            else if ($objUdf2.is("input[type='text']")) {
                $objUdf2.val($spnUDF2.val());
            }
        }
        if ($objUdf3 != undefined && $spnUDF3 != undefined) {
            if ($objUdf3.is("select")) {
                $objUdf3.find("option").filter(function () {
                    return this.text == $spnUDF3.val();
                }).attr('selected', true);
            }
            else if ($objUdf3.is("input[type='text']")) {
                $objUdf3.val($spnUDF3.val());
            }
        }
        if ($objUdf4 != undefined && $spnUDF4 != undefined) {
            if ($objUdf4.is("select")) {
                $objUdf4.find("option").filter(function () {
                    return this.text == $spnUDF4.val();
                }).attr('selected', true);
            }
            else if ($objUdf4.is("input[type='text']")) {
                $objUdf4.val($spnUDF4.val());
            }
        }

        if ($objUdf5 != undefined && $spnUDF5 != undefined) {
            if ($objUdf5.is("select")) {
                $objUdf5.find("option").filter(function () {
                    return this.text == $spnUDF5.val();
                }).attr('selected', true);
            }
            else if ($objUdf5.is("input[type='text']")) {
                $objUdf5.val($spnUDF5.val());
            }
        }

        if ($vPullOrderNumber != undefined && $spnPullOrderNumber != undefined) {
            if ($vPullOrderNumber.is("select")) {
                $vPullOrderNumber.find("option").filter(function () {
                    return this.text == $spnPullOrderNumber.val();
                }).attr('selected', true);
            }
            else if ($vPullOrderNumber.is("input[type='text']")) {
                $vPullOrderNumber.val($spnPullOrderNumber.val());
            }
        }

    });
}



function UDFfillEditableOptionsForGrid_PullMaster() {



    $("table#myDataTable").on("mouseenter", ".udf-editable-autocomplete-dropdownbox", function () {
        var _EnterPriseId = $("#hdnEnterpriseId").val();
        

        if ($(this).data('autocomplete') == undefined) {
            var _UDFID = $(this).prev().val();
            $('.show-all-options').unbind("click");
            $('.show-all-options').click(function () {
                var currentObj = $(this);
                //setTimeout(function () {
                var ddl = $(currentObj).siblings(".udf-editable-dropdownbox");

                if (_AutoCompleteWrapper.isOpen(ddl)) {
                    _AutoCompleteWrapper.close(ddl);
                }
                else {
                    _AutoCompleteWrapper.search(ddl, " ").focus(ddl);
                }

                //if (ddl.data("is_open") != true) {
                //    ddl.autocomplete("search", " ");
                //    ddl.trigger("focus");
                //}
                //else {
                //    ddl.trigger("close");
                //}

                //setTimeout(function () {
                //$('ul.ui-autocomplete').css('overflow-y', 'auto');
                //$('ul.ui-autocomplete').css('max-height', '300px');
                //$('ul.ui-autocomplete').css('z-index', '99999');
                //}, 100);



                //setTimeout(function(){

                //},1000);

            });

            var $this = $(this);

            _AutoCompleteWrapper.init($this
                , '/UDF/GetUDFEditableOptionsByUDF'
                , function (request) {

                    var obj = {
                        "maxRows": 1000,
                        "name_startsWith": request.term,
                        "UDFID": _UDFID,
                        "EnterpriseID": _EnterPriseId
                    };

                    return obj;                    
                },
                function (data) {
                    return $.map(data, function (item) {
                        return {
                            label: item.UDFOption,
                            value: item.UDFOption,
                            selval: item.UDFOption
                        };
                    });
                }
                , function (curVal, selectedItem) {
                    if (curVal != selectedItem.value) {
                        UpdateUDFInPullHistory($this);
                    }
                    //$("#" + _UDFColumnName).val(ui.item.selval);
                });

            //$(this).autocomplete({
            //    source: function (request, response) {
            //        $.ajax({
            //            url: '/UDF/GetUDFEditableOptionsByUDF',
            //            contentType: 'application/json',
            //            dataType: 'json',
            //            data: {
            //                maxRows: 1000,
            //                name_startsWith: request.term,
            //                UDFID: _UDFID,
            //                EnterpriseID: _EnterPriseId
            //            },
            //            cache: false,
            //            success: function (data) {
            //                response($.map(data, function (item) {
            //                    return {
            //                        label: item.UDFOption,
            //                        value: item.UDFOption,
            //                        selval: item.UDFOption
            //                    }
            //                }));
            //            }
            //        });
            //    },
            //    autoFocus: false,
            //    minLength: 0,
            //    select: function (event, ui) {
            //        if ($(this).val() != ui.item.value) {
            //            $(this).val(ui.item.value);
            //            UpdateUDFInPullHistory($(this));
            //        }
            //        //$("#" + _UDFColumnName).val(ui.item.selval);
            //    },
            //    change: function (event, ui) {
                
            //    },
            //    open: function () {
            //        $(this).removeClass("ui-corner-all").addClass("ui-corner-top");

            //        $(this).autocomplete('widget').css('z-index', '99999 !important');
            //        $(this).data('is_open', true);
            //    },
            //    close: function () {
            //        $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
            //        $(this).data('is_open', false);
            //    }
            //});
        }
    });

        



    //$('.udf-editable-autocomplete-dropdownbox').each(function () {

    //    var _UDFID = $(this).prev().val();
    //    $('.show-all-options').unbind("click");
    //    $('.show-all-options').click(function () {
    //        var currentObj = $(this);
    //        //setTimeout(function () {

    //        $(currentObj).siblings(".udf-editable-dropdownbox").autocomplete("search", " ");
    //        $(currentObj).siblings('.udf-editable-dropdownbox').trigger("focus");
    //        //setTimeout(function () {
    //        //$('ul.ui-autocomplete').css('overflow-y', 'auto');
    //        //$('ul.ui-autocomplete').css('max-height', '300px');
    //        //$('ul.ui-autocomplete').css('z-index', '99999');
    //        //}, 100);



    //        //setTimeout(function(){

    //        //},1000);

    //    });
    //    $(this).autocomplete({
    //        source: function (request, response) {
    //            $.ajax({
    //                url: '/UDF/GetUDFEditableOptionsByUDF',
    //                contentType: 'application/json',
    //                dataType: 'json',
    //                data: {
    //                    maxRows: 1000,
    //                    name_startsWith: request.term,
    //                    UDFID: _UDFID,
    //                    EnterpriseID: _EnterPriseId
    //                },
    //                cache: false,
    //                success: function (data) {
    //                    response($.map(data, function (item) {
    //                        return {
    //                            label: item.UDFOption,
    //                            value: item.UDFOption,
    //                            selval: item.UDFOption
    //                        }
    //                    }));
    //                }
    //            });
    //        },
    //        autoFocus: false,
    //        minLength: 0,
    //        select: function (event, ui) {
    //            $(this).val(ui.item.value);
    //            UpdateUDFInPullHistory($(this));
    //            //$("#" + _UDFColumnName).val(ui.item.selval);
    //        },
    //        open: function () {
    //            $(this).removeClass("ui-corner-all").addClass("ui-corner-top");

    //            $(this).autocomplete('widget').css('z-index', '99999 !important');
    //        },
    //        close: function () {
    //            $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
    //        }
    //    });

    //});

}



function PONfillEditableOptionsForGrid_PullMaster() {



    $("table#myDataTable").on("mouseenter", ".AutoPullOrderNumber", function () {

        if ($(this).data('autocomplete') == undefined) {
            var _UDFID = $(this).prev().val();
            $('.ShowAllOptionsOrderNumber').unbind("click");
            $('.ShowAllOptionsOrderNumber').click(function () {
                var currentObj = $(this);
                //setTimeout(function () {

                var ddl = $(currentObj).siblings(".AutoPullOrderNumber");

                if (ddl.data("is_open") != true) {
                    ddl.autocomplete("search", " ");
                    ddl.trigger("focus");
                }
                else {
                    ddl.trigger("close");
                }
                

                //setTimeout(function () {
                //$('ul.ui-autocomplete').css('overflow-y', 'auto');
                //$('ul.ui-autocomplete').css('max-height', '300px');
                //$('ul.ui-autocomplete').css('z-index', '99999');
                //}, 100);



                //setTimeout(function(){

                //},1000);

            });
            $(this).autocomplete({
                source: function (request, response) {
                    $.ajax({
                        url: '/Pull/GetPullOrderNumberForNewPullGrid',
                        contentType: 'application/json',
                        dataType: 'json',
                        data: {

                            NameStartWith: request.term


                        },
                        cache: false,
                        success: function (data) {
                            response($.map(data, function (item) {
                                return {
                                    label: item.Value,
                                    value: item.Value,
                                    selval: item.Value
                                }
                            }));
                        }
                    });
                },
                autoFocus: false,
                minLength: 0,
                select: function (event, ui) {
                    if ($(this).val() != ui.item.value) {
                        $(this).val(ui.item.value);
                        UpdatePullOrderNumberInPullHistory($(this));
                    }
                    //$("#" + _UDFColumnName).val(ui.item.selval);
                },
                open: function () {
                    $(this).removeClass("ui-corner-all").addClass("ui-corner-top");

                    $(this).autocomplete('widget').css('z-index', '99999 !important');
                    $(this).data('is_open', true);
                },
                close: function () {
                    $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
                    $(this).data('is_open', false);
                }
            });
        }
    });

}


//////////////// script 2

function AddBillingOrPullOrderNoToPullList() {

    var vPullGUIDs = '';

    $('#myDataTable tbody tr.row_selected').each(function (i) {
        var tr = $(this);
        var hdnID = $(tr).find('#hdnID').val();
        vPullGUIDs = vPullGUIDs + hdnID + ',';

    });

    $('#DivLoading').show();
    //$('#PopupSetBilling').load('../Pull/SetBillingForPull?PullGUIDs=' + vPullGUIDs + '', function () {
    $('#PopupSetBilling').load('../Pull/SetBillingForPull', { 'PullGUIDs': vPullGUIDs} , function () {
        $('#PopupSetBilling').dialog('open');
        $('#DivLoading').hide();
        IsRefreshGrid = true;
    });
    return false;
}

function AddSingleBillingOrPullOrderNoToPullList(obj) {
    var row = $(obj).closest('tr');
    var vIsBilling = row.find("#chkIsBilling").is(":checked");
    var vPullOrderNumber = row.find("#txtPullOrderNumber").val();
    var vPullGUID = row.find("#spnPullGUID").text();

    if (vPullOrderNumber == null || vPullOrderNumber == "") {
        row.find("#txtPullOrderNumber").focus();
        alert(MsgEnterPullOrderNumber);
        return false;
    }
    $('#DivLoading').show();
    var vUDF1 = '';
    var vUDF2 = '';
    var vUDF3 = '';
    var vUDF4 = '';
    var vUDF5 = '';

    var udf1Object = row.find('[data-udfname="UDF1"]');
    if (udf1Object !== undefined && udf1Object != null && udf1Object.length > 0)
    {
        vUDF1 = udf1Object.text();
    }
    var udf2Object = row.find('[data-udfname="UDF2"]');
    if (udf2Object !== undefined && udf2Object != null && udf2Object.length > 0) {
        vUDF2 = udf2Object.text();
    }
    var udf3Object = row.find('[data-udfname="UDF3"]');
    if (udf3Object !== undefined && udf3Object != null && udf3Object.length > 0) {
        vUDF3 = udf3Object.text();
    }
    var udf4Object = row.find('[data-udfname="UDF4"]');
    if (udf4Object !== undefined && udf4Object != null && udf4Object.length > 0) {
        vUDF4 = udf4Object.text();
    }
    var udf5Object = row.find('[data-udfname="UDF5"]');
    if (udf5Object !== undefined && udf5Object != null && udf5Object.length > 0) {
        vUDF5 = udf5Object.text();
    }

    ///////////////////////////////////// UPDATE PULL DATA CALL//////////////////////////////////////////START
    $.ajax({
        "url": _PullMasterList.urls.setBillingForPullMasterUrl,
        data: { PullGUID: vPullGUID, PullOrderNumber: vPullOrderNumber, IsBilling: vIsBilling, 'UDF1': vUDF1, 'UDF2': vUDF2, 'UDF3': vUDF3, 'UDF4': vUDF4, 'UDF5': vUDF5 },
        "async": false,
        cache: false,
        "dataType": "json",
        success: function (response) {
            $('#DivLoading').hide();
            if (response.Status == "OK") {
                $("#spanGlobalMessage").html(response.Message);
                $('div#target').fadeToggle();
                $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                $('#myDataTable').dataTable().fnStandingRedraw();
                UDFInsertNewForGrid(row);
            }
            else {
                alert(response.Message);
            }
        },
        error: function (response) {
            $('#DivLoading').hide();
            $("#spanGlobalMessage").html(response.message);
            $('div#target').fadeToggle();
            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
        }
    });
    ///////////////////////////////////// UPDATE PULL DATA CALL//////////////////////////////////////////END

    return false;
}


function UpdateUDFInPullHistory(rowobj) {
    var urow = $(rowobj).closest('tr');
    var uPullGUID = urow.find("#spnPullGUID").text();
    var _udfID = $(rowobj).attr("id");
    var _udfvalue = '';
    if ($(rowobj).is('input')) {
        _udfvalue = $(rowobj).val();
    }
    else {
        _udfvalue = $(rowobj).find("option:selected").text();
    }

    $('#DivLoading').show();
    ///////////////////////////////////// UPDATE UDF DATA CALL//////////////////////////////////////////START
    $.ajax({
        "url": _PullMasterList.urls.updateUDFInPullHistoryUrl,
        data: { PullGUID: uPullGUID, UDFID: _udfID, UDFValue: _udfvalue },
        "async": false,
        cache: false,
        "dataType": "json",
        success: function (response) {
            $('#DivLoading').hide();
            if (response.Status == "OK") {
                $("#spanGlobalMessage").html(response.Message);
                $('div#target').fadeToggle();
                $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                $('#myDataTable').dataTable().fnStandingRedraw();

            }
            else {
                alert(response.Message);
            }
        },
        error: function (response) {
            $('#DivLoading').hide();
            $("#spanGlobalMessage").html(response.message);
            $('div#target').fadeToggle();
            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
        }
    });
    ///////////////////////////////////// UPDATE UDF DATA CALL//////////////////////////////////////////END

}

function UpdatePullQtyInPullHistory(rowobj, OldPullQuantity, NewPullQuantity, SerialNumberTracking, LotNumberTracking, DateCodeTracking) {       
    var urow = $(rowobj).closest('tr');
    var PullGUID = urow.find("#spnPullGUID").text();
    var ItemGuid = urow.find("#hdnItemGuid").val();      
   // var OldPullQuantity = OldVal;
   // var NewPullQuantity = $(rowobj).val();
    var PullOrCredit = urow.find("#spnPullOrCredit").text();

    if (NewPullQuantity == "" || NewPullQuantity == "0") {
        $("#spanGlobalMessage").removeClass('succesIcon').addClass('errorIcon');
        $("#spanGlobalMessage").html(MsgQuantityBlankZeroValidation);
        $('div#target').fadeToggle();
        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
        return false;
    }

    if (SerialNumberTracking.toLowerCase() == "false"
        && LotNumberTracking.toLowerCase() == "false"
        && DateCodeTracking.toLowerCase() == "false" ) {
        if (NewPullQuantity == OldPullQuantity) {
            $("#spanGlobalMessage").removeClass('succesIcon').addClass('errorIcon');
            $("#spanGlobalMessage").html(MsgNewOldQuantitySameValidation);
            $('div#target').fadeToggle();
            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
            return false;
        }
    }

    if (PullOrCredit.toLowerCase() == "pull" || PullOrCredit.toLowerCase() == "ms pull") {
        if (!confirm(MsgUpdatePullQuantityValidation)) {
            $('#myDataTable').dataTable().fnStandingRedraw();
            return false;
        }
    }
    else if (PullOrCredit.toLowerCase() == "credit" || PullOrCredit.toLowerCase() == "ms credit") {
        if (!confirm(MsgUpdateCreditQuantityValidation)) {
            $('#myDataTable').dataTable().fnStandingRedraw();
            return false;
        }
    }   

    $('#DivLoading').show();
    if (SerialNumberTracking.toLowerCase() == "false"
        && LotNumberTracking.toLowerCase() == "false"
        && DateCodeTracking.toLowerCase() == "false") {       
        ///////////////////////////////////// UPDATE Pull Quantity DATA CALL//////////////////////////////////////////START
        $.ajax({
            "url": _PullMasterList.urls.UpdatePullQtyInPullHistoryUrl,
            data: { PullGUID: PullGUID, ItemGuid: ItemGuid, OldPullQuantity: parseInt(OldPullQuantity), NewPullQuantity: parseInt(NewPullQuantity), PullCreditType: PullOrCredit },
            "async": false,
            cache: false,
            "dataType": "json",
            success: function (response) {
                $('#DivLoading').hide();
                if (response.Status == "OK") {
                    $("#spanGlobalMessage").html(response.Message);
                    $('div#target').fadeToggle();
                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                    $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                    $('#myDataTable').dataTable().fnStandingRedraw();
                }
                else {
                    if (response.LocationMSG != "" && response.LocationMSG != undefined) {
                        if (response.PSLimitExceed) {
                            $("#spanGlobalMessage").html(MsgUpdatePullQuantityConfirmation.replace("{0}", response.LocationMSG));
                            $('div#target').fadeToggle();
                            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                            $("#spanGlobalMessage").removeClass('succesIcon').addClass('errorIcon');
                            //$('#myDataTable').dataTable().fnStandingRedraw();
                        }
                        else {
                            $("#spanGlobalMessage").html(response.LocationMSG);
                            $('div#target').fadeToggle();
                            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                            $("#spanGlobalMessage").removeClass('succesIcon').addClass('errorIcon');
                            //$('#myDataTable').dataTable().fnStandingRedraw();
                        }
                    }
                    else {
                        $("#spanGlobalMessage").html(response.Message);
                        $('div#target').fadeToggle();
                        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                        $("#spanGlobalMessage").removeClass('succesIcon').addClass('errorIcon');
                        //$('#myDataTable').dataTable().fnStandingRedraw();
                    }
                    //alert(response.Message);
                }
            },
            error: function (response) {
                $('#DivLoading').hide();
                $("#spanGlobalMessage").html(response.message);
                $('div#target').fadeToggle();
                $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                $("#spanGlobalMessage").removeClass('succesIcon').addClass('errorIcon');
                $('#myDataTable').dataTable().fnStandingRedraw();
            }
        });
    ///////////////////////////////////// UPDATE Pull Quantity DATA CALL//////////////////////////////////////////END
    }
    else {
        ///////////////////////////////////// UPDATE Pull Quantity DATA CALL For tracking Item//////////////////////////////////////////START
        if (PullOrCredit.toLowerCase() == "pull" || PullOrCredit.toLowerCase() == "ms pull") {
            $.ajax({
                type: "POST",
                url: _PullMasterList.urls.PullItemEditQuantityUrl,
                contentType: 'application/json',
                dataType: 'html',
                data: JSON.stringify({ PullGUID: PullGUID, ItemGUID: ItemGuid, OldPullQuantity: parseInt(OldPullQuantity), NewPullQuantity: parseInt(NewPullQuantity), PullCreditType: PullOrCredit }),

                success: function (RetData) {
                    $("#DivPullSelectionEdit").html("");
                    $("#DivPullSelectionEdit").html(RetData);
                    $("#DivPullSelectionEdit").dialog('open');
                    $('#DivLoading').hide();
                },
                error: function (err) {
                    $('#DivLoading').hide();
                    $("#spanGlobalMessage").html(err);
                    $('div#target').fadeToggle();
                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                    $("#spanGlobalMessage").removeClass('succesIcon').addClass('errorIcon');
                    // $('#myDataTable').dataTable().fnStandingRedraw();
                }
            });
        }
        else if (PullOrCredit.toLowerCase() == "credit" || PullOrCredit.toLowerCase() == "ms credit") {

            var itemTrack = ''
            if (SerialNumberTracking) {
                itemTrack = "SERIALTRACK"
            }
            else if (LotNumberTracking) {
                itemTrack = "LOTTRACK"
            }
            else if (DateCodeTracking) {
                itemTrack = "DATECODETRACK"
            }

            $("#divPreCreditInforSerialLotEdit").dialog({
                autoOpen: true,
                modal: true,
                draggable: true,
                resizable: true,
                width: '70%',
                height: 500,
                title: "Credit",
                open: function () {
                    setTimeout(function () {
                        $.ajax({
                            type: "POST",
                            url: "/Pull/PullLotSrSelectionForCreditEdit",
                            contentType: 'application/json',
                            dataType: 'html',
                            data: JSON.stringify({ PullGUID: PullGUID, ItemGUID: ItemGuid, OldCreditQuantity: parseInt(OldPullQuantity), NewCreditQuantity: parseInt(NewPullQuantity), PullCreditType: PullOrCredit }),
                            success: function (RetData) {
                                $("#divPreCreditInforSerialLotEdit").html(RetData);
                                $('#DivLoading').hide();
                            },
                            error: function (response) {

                                $('#DivLoading').hide();
                                $("#spanGlobalMessage").html(response);
                                $('div#target').fadeToggle();
                                $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                                $("#spanGlobalMessage").removeClass('succesIcon').addClass('errorIcon');
                            }
                        });
                    }, 100);
                },
                close: function () {
                    DeletedRowObject = "";
                    $('#divPreCreditInforSerialLotEdit').empty();
                }
            });
        }
        ///////////////////////////////////// UPDATE Pull Quantity DATA CALL For tracking Item//////////////////////////////////////////START
    }
}
function UpdatePullOrderNumberInPullHistory(rowobj,allowBlank) {
    if (allowBlank == undefined) {
        allowBlank = false;
    }
    var urow = $(rowobj).closest('tr');
    var uPullGUID = urow.find("#spnPullGUID").text();
    var vSupID = urow.find("#spnSupplierID").text();
    var _PONvalue = '';
    if ($(rowobj).is('input')) {
        _PONvalue = $(rowobj).val();
    }
    else {
        _PONvalue = $(rowobj).find("option:selected").text();
    }

    //if (_PONvalue != null && _PONvalue.length > 0)
    {
        $('#DivLoading').show();
        ///////////////////////////////////// UPDATE UDF DATA CALL//////////////////////////////////////////START
        $.ajax({
            "url": _PullMasterList.urls.updatePullOrderNumberInPullHistoryUrl,
            data: { SupplierID: vSupID, PullGUID: uPullGUID, PullOrderNumber: _PONvalue, allowBlank: allowBlank },
            "async": false,
            cache: false,
            "dataType": "json",
            success: function (response) {
                $('#DivLoading').hide();
                if (response.Status == "OK") {
                    $("#spanGlobalMessage").html(response.Message);
                    $('div#target').fadeToggle();
                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                    $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                    $('#myDataTable').dataTable().fnStandingRedraw();

                }
                else {
                    alert(response.Message);
                }
            },
            error: function (response) {
                $('#DivLoading').hide();
                $("#spanGlobalMessage").html(response.message);
                $('div#target').fadeToggle();
                $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
            }
        });
        ///////////////////////////////////// UPDATE UDF DATA CALL//////////////////////////////////////////END
    }
}
var LenBeforeRebind = 0;
var LenAfterRebind = 0;
function PreparePullDataTableForEdit(objPullItemDTO) {
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
        "sAjaxSource": _PullMasterList.urls.PullLotSrSelectionDetailsEditUrl,
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
            aoData.push({ "name": "PullGUID", "value": objPullItemDTO.PullGUID });
            aoData.push({ "name": "BinID", "value": objPullItemDTO.BinID });
            if (objPullItemDTO.ItemGUID != '00000000-0000-0000-0000-000000000000' && objPullItemDTO.ItemGUID != '')
                aoData.push({ "name": "PullQuantity", "value": FormatedCostQtyValues($("#txtPoolQuantity_" + objPullItemDTO.ItemGUID + "_" + objPullItemDTO.RequisitionDetailGUID).val(), 2) });
            //else
            //    aoData.push({ "name": "PullQuantity", "value": FormatedCostQtyValues($("#txtPoolQuantity_" + objPullItemDTO.ToolGUID + "_" + objPullItemDTO.RequisitionDetailGUID).val(), 2) });
            aoData.push({ "name": "InventoryConsuptionMethod", "value": objPullItemDTO.InventoryConsuptionMethod });
            aoData.push({ "name": "CurrentLoaded", "value": $("#hdnCurrentLoadedIds_" + objPullItemDTO.ItemGUID + "_" + objPullItemDTO.RequisitionDetailGUID).val() });
            aoData.push({ "name": "ViewRight", "value": objPullItemDTO.ViewRight });
            aoData.push({ "name": "IsDeleteRowMode", "value": isDeleteSrLotRowEdit });
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
                    isDeleteSrLotRowEdit = false;
                    $('.ShowAllOptionsSL').click(function () {
                        $(this).siblings('.AutoSerialLot').trigger("focus");
                        $(this).siblings(".AutoSerialLot").autocomplete("search", "");
                    });

                    if (objPullItemDTO.ViewRight == "ViewOnly") {
                        $("input[type='text'][name='txtLotOrSerailNumberViewOnly']").keypress(function () {
                            return false;
                        });

                        $("#DivPullSelectionEdit input[type='text'][name='txtPullQty']").keypress(function () {
                            return false;
                        });
                    }

                    LenAfterRebind = $('#' + objPullItemDTO.tableID).find("tbody").find("tr").length;
                    if (LenBeforeRebind == LenAfterRebind && IsLoadMoreLotsClicked == true) {
                        alert(MsgNoLocationToAdd);
                    }
                    IsLoadMoreLotsClicked = false;
                }
            });
        }
    });
}

function ValidateSingleEditPull(vItemGUID, RequisitionDetailGUID) {    
    var returnVal = true;
    var errormsg = "";
    var isMoreQty = false;
    var dtID = "#tblItemPull_" + vItemGUID + "_" + RequisitionDetailGUID;

    var SpanQty = $("#DivPullSelectionEdit").find("#txtPoolQuantity_" + vItemGUID + "_" + RequisitionDetailGUID);

    var TotalEntered = 0;
    $("#tblItemPull_" + vItemGUID + "_" + RequisitionDetailGUID).find("tbody").find("tr").each(function (index, tr) {
        var $tr = $(tr);
        var txtPullQty = $tr.find("input[type='text'][name='txtPullQty']").val();
        var spnLotSerialQuantity = $tr.find("span[name='spnLotSerialQuantity']").text();

        if (parseFloat(txtPullQty) > parseFloat(spnLotSerialQuantity)) {
            errormsg = "\n" + MsgPullMoreQuantityValidation;
            isMoreQty = true;
            return errormsg;
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

function ValidateAllEditPull() {
    var returnVal = true;
    var errormsg = "";
    var isMoreQty = false;
    $("#DivPullSelectionEdit").find("table[id^='tblItemPullheader']").each(function (indx, tblHeader) {
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
                        errormsg = "\n" + MsgPullMoreQuantityValidation;
                        isMoreQty = true;
                        return errormsg;
                    }

                    TotalEntered = TotalEntered + parseFloat(txtPullQty);
                }
            }); // loop

            if (isMoreQty == false) {
                var pullQty = parseFloat($(SpanQty).val().toString());
                if (TotalEntered != pullQty) {
                    ////errormsg = errormsg + "\nentered :" + TotalEntered + "\tPull Qty :" + pullQty;
                    errormsg = errormsg + "\n"+ MsgEnteredPullQuantityValidation.replace("{ 0 } ", TotalEntered).replace("{ 1 } ", pullQty)
                }
            }
            else {
                errormsg = MsgPullMoreQuantityValidation;
            }
        }
    });

    return errormsg;
}

function EditPullAllNewFlow() {    
    var ArrItem = new Array();
    var arrItemDetails;
    var ErrorMessage = ValidateAllEditPull();

    if (ErrorMessage == "") {
        $("#DivPullSelectionEdit").find("table[id^='tblItemPullheader']").each(function (indx, tblHeader) {
            var $tblHeader = $(tblHeader);
            var strpullobj = JSON.parse($tblHeader.find("input[name='hdnPullMasterDTO']").val());
            arrItemDetails = new Array();
            var ID = $tblHeader.prop("id").split('_')[1];
            var vPullGUID = strpullobj.GUID;
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
                    var hdnExpirationDate = $tr.find("input[name='hdnExpirationDate']").val();
                    if (txtPullQty != "") {
                        var txtLotOrSerailNumber = "";
                        if (hdnLotNumberTracking == "true" || hdnSerialNumberTracking == "true") {
                            txtLotOrSerailNumber = $tr.find("input[type='text'][name^='txtLotOrSerailNumber']").val();
                        }

                        var vSerialNumber = "",
                            vLotNumber = "",
                            vExpiration = "";
                            vExpirationDate = "";

                        if (hdnSerialNumberTracking == "true") {
                            vSerialNumber = $.trim(txtLotOrSerailNumber);
                        }
                        if (hdnLotNumberTracking == "true") {
                            vLotNumber = $.trim(txtLotOrSerailNumber);
                        }
                        if (hdnDateCodeTracking == "true") {
                            vExpiration = hdnExpiration;
                            vExpirationDate = hdnExpirationDate;
                        }                        
                        var obj = {
                            "LotOrSerailNumber": $.trim(txtLotOrSerailNumber), "BinNumber": hdnBinNumber, "PullQuantity": parseFloat(txtPullQty.toString())
                            , "LotNumberTracking": hdnLotNumberTracking, "SerialNumberTracking": hdnSerialNumberTracking, "DateCodeTracking": hdnDateCodeTracking
                            , "Expiration": hdnExpiration, "ExpirationDate": hdnExpirationDate,  "SerialNumber": $.trim(vSerialNumber), "LotNumber": vLotNumber
                            , "ItemGUID": strpullobj.ItemGUID, "BinID": strpullobj.BinID, "ID": strpullobj.BinID
                            , "CustomerOwnedTobePulled": parseFloat(txtPullQty.toString())
                            , "ConsignedTobePulled": parseFloat(txtPullQty.toString())
                            , "TotalTobePulled": parseFloat(txtPullQty.toString())
                        };

                        arrItemDetails.push(obj);
                    }
                }); // loop
            }

            var pullQty = parseFloat($(SpanQty).val());

            var PullItem = {
                ID: indx, ItemGUID: strpullobj.ItemGUID, PullGUID: strpullobj.GUID, ProjectSpendGUID: strpullobj.ProjectSpendGUID, ProjectSpendName: strpullobj.ProjectSpendName,
                ItemID: strpullobj.ItemID, ItemNumber: strpullobj.ItemNumber, BinID: strpullobj.BinID, BinNumber: strpullobj.BinNumber,
                PullQuantity: pullQty, UDF1: strpullobj.UDF1, UDF2: strpullobj.UDF2, UDF3: strpullobj.UDF3, UDF4: strpullobj.UDF4,
                UDF5: strpullobj.UDF5, PullOrderNumber: strpullobj.PullOrderNumber, lstItemPullDetails: arrItemDetails,
                WorkOrderDetailGUID: strpullobj.WorkOrderDetailGUID, RequisitionDetailsGUID: strpullobj.RequisitionDetailGUID,               
                SupplierAccountGuid: strpullobj.SupplierAccountGuid
                , TotalConsignedTobePulled: pullQty
                , TotalCustomerOwnedTobePulled: pullQty
            };
            ArrItem.push(PullItem);
        });

        if (ArrItem.length > 0) {
            EditPullMultipleItemNew(ArrItem);
        }
    }
    else {
        alert(ErrorMessage);
    }
}

function EditPullMultipleItemNew(ArrItem) {    
    $('#DivLoading').show();
    $.ajax({
        type: "POST",
        url: _PullMasterList.urls.PullSerialsAndLotsNewEditUrl,
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
                    var $divItemRet2 = $('#divItem_' + RetDataItem.ItemGUID + '_');
                    $divItemRet2.attr('style', '');
                    $divItemRet2.remove();
                }
            });

            $('#DivLoading').hide();
            if (errorMessage != "") {
                $.modal.impl.close();
                errorMessage = "<b>" + SomeItemNotPulled+"</b><br /><br />" + errorMessage;
                $('#dlgCommonErrorMsgPopupEdit').find("#pOkbtn").css('display', '');
                $('#dlgCommonErrorMsgPopupEdit').find("#pErrMessage").html(errorMessage);
                $('#dlgCommonErrorMsgPopupEdit').modal();
                $('#dlgCommonErrorMsgPopupEdit').css("z-index", "1104");
                $('#simplemodal-overlay').css("z-index", "1103");
                $('#simplemodal-container').css("z-index", "1104");
            }
            else {
                var IsFromPullMaster = false;

                if ($("input[type='hidden'][name^='hdnPullMasterDTO']").length > 0) {
                    IsFromPullMaster = true;
                }
                else {
                    IsFromPullMaster = false;
                }

                if (IsFromPullMaster == true) {
                    $.modal.impl.close();
                    $('#dlgCommonErrorMsgPopupEdit').find("#pOkbtn").css('display', '');
                    $('#dlgCommonErrorMsgPopupEdit').find("#pErrMessage").html("<b>" + MsgPullDoneSuccess+"</b><br /><br />");
                    $('#dlgCommonErrorMsgPopupEdit').modal();
                    $('#dlgCommonErrorMsgPopupEdit').css("z-index", "1104");
                    $('#simplemodal-overlay').css("z-index", "1103");
                    $('#simplemodal-container').css("z-index", "1104");
                    if ($('div[id^="divItem_"]').length <= 0) {
                        $('#DivPullSelectionEdit').dialog('close');
                        $('#myDataTable').dataTable().fnStandingRedraw();
                    }
                }
                else {
                    $("#spanGlobalMessage").html(AllPulldon);
                    $('div#target').fadeToggle();
                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                    $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                    if ($('div[id^="divItem_"]').length <= 0) {
                        $('#DivPullSelectionEdit').dialog('close');
                        $('#myDataTable').dataTable().fnStandingRedraw();
                    }
                }
            }
        },
        error: function (err) {
            $('#DivLoading').hide();
            $.modal.impl.close();
            $('#dlgCommonErrorMsgPopupEdit').find("#pOkbtn").css('display', '');
            $('#dlgCommonErrorMsgPopupEdit').find("#pErrMessage").html(MsgErrorInProcess);
            $('#dlgCommonErrorMsgPopupEdit').modal();
            $('#dlgCommonErrorMsgPopupEdit').css("z-index", "1004");
            $('#simplemodal-overlay').css("z-index", "1003");
            $('#simplemodal-container').css("z-index", "1004");
        },
        complete: function () {
        }
    });
}

/* For Edit Credit for Lot,serial and datecode start */

/////////////// New added for allow More Credit for Serial,lot and date code ////////////////

function ValidateSinglePullForCreditEdit(vItemGUID) {    
    var IsIgnoreCreditRule = false;
    var returnVal = true;
    var errormsg = "";
    var isMoreQty = false;
    var dtID = "#tblItemPull_" + vItemGUID;

    var SpanQty = $("#divPreCreditInforSerialLotEdit").find("#txtPoolQuantity_" + vItemGUID);

    var TotalEntered = 0;
    $("#tblItemPull_" + vItemGUID).find("tbody").find("tr").each(function (index, tr) {
        var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();
        var spnLotSerialQuantity = $(tr).find("span[name='spnLotSerialQuantity']").text();

        IsIgnoreCreditRule = $(tr).find("#hdnIsIgnoreCreditRule").val();

        var hdnLotNumberTracking = $(tr).find("input[name='hdnLotNumberTracking']").val();
        var hdnSerialNumberTracking = $(tr).find("input[name='hdnSerialNumberTracking']").val();
        var hdnDateCodeTracking = $(tr).find("input[name='hdnDateCodeTracking']").val();

        if (hdnLotNumberTracking == "true") {
            var txtLotNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
            if (txtLotNumber == "" || txtLotNumber == null) {
                errormsg = "\n" + MsgEnterLotNumber;
                return errormsg;
            }
        }

        if (hdnSerialNumberTracking == "true") {
            var txtSerailNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
            if (txtSerailNumber == "" || txtSerailNumber == null) {
                errormsg = "\n" + MsgSerialNumberValidation;
                return errormsg;
            }
        }

        if (hdnDateCodeTracking == "true") {
            var txtExpiration = $.trim($(tr).find("input[type='text'][name^='txtExpirationDate']").val());
            if (txtExpiration == "" || txtExpiration == null) {
                errormsg = "\n" + MsgEnterExpirationDate;
                return errormsg;
            }
        }

        if (IsIgnoreCreditRule == "False" || IsIgnoreCreditRule == "false" || IsIgnoreCreditRule == false) {
            if (parseFloat(txtPullQty) > parseFloat(spnLotSerialQuantity)) {
                errormsg = "\n"+MsgCreditMoreAvailableQTY;
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
        if (IsIgnoreCreditRule == "False" || IsIgnoreCreditRule == "false" || IsIgnoreCreditRule == false) {
            errormsg = MsgCreditMoreAvailablePullQTY;
        }
    }

    return errormsg;
}

function ValidateAllPullForCreditEdit() {    
    //
    var returnVal = true;
    var errormsg = "";
    var isMoreQty = false;

    var IsIgnoreCreditRule = false;

    $("#divPreCreditInforSerialLotEdit").find("table[id^='tblItemPullheader']").each(function (indx, tblHeader) {
        var ID = $(tblHeader).prop("id").split('_')[1];
        var SpanQty = $(tblHeader).find("#txtPoolQuantity_" + ID);

        var TotalEntered = 0;
        if ($("#tblItemPull_" + ID).length > 0) {
            $("#tblItemPull_" + ID).find("tbody").find("tr").each(function (index, tr) {
                if ($(tr).find("input[type='text'][name='txtPullQty']").length > 0) {


                    IsIgnoreCreditRule = $(tr).find("#hdnIsIgnoreCreditRule").val();

                    var txtPullQty = $(tr).find("input[type='text'][name='txtPullQty']").val();
                    var spnLotSerialQuantity = $(tr).find("span[name='spnLotSerialQuantity']").text();

                    var hdnLotNumberTracking = $(tr).find("input[name='hdnLotNumberTracking']").val();
                    var hdnSerialNumberTracking = $(tr).find("input[name='hdnSerialNumberTracking']").val();
                    var hdnDateCodeTracking = $(tr).find("input[name='hdnDateCodeTracking']").val();

                    if (hdnLotNumberTracking == "true") {
                        var txtLotNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                        if (txtLotNumber == "" || txtLotNumber == null) {
                            errormsg = "\n" + MsgEnterLotNumber;
                            return errormsg;
                        }
                    }

                    if (hdnSerialNumberTracking == "true") {
                        var txtSerailNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                        if (txtSerailNumber == "" || txtSerailNumber == null) {
                            errormsg = "\n" + MsgSerialNumberValidation;
                            return errormsg;
                        }
                    }

                    if (hdnDateCodeTracking == "true") {
                        var txtExpiration = $.trim($(tr).find("input[type='text'][name^='txtExpirationDate']").val());
                        if (txtExpiration == "" || txtExpiration == null) {
                            errormsg = "\n" + MsgEnterExpirationDate;
                            return errormsg;
                        }
                    }

                    if (IsIgnoreCreditRule == "False" || IsIgnoreCreditRule == "false" || IsIgnoreCreditRule == false) {
                        if (parseFloat(txtPullQty) > parseFloat(spnLotSerialQuantity)) {
                            errormsg = "\n" + MsgCreditMoreAvailablePullQTY;
                            isMoreQty = true;
                            return errormsg;
                        }
                    }

                    TotalEntered = TotalEntered + parseFloat(txtPullQty);
                }
            });

            //
            if (isMoreQty == false) {
                var pullQty = parseFloat($(SpanQty).val().toString());
                if (TotalEntered != pullQty) {
                    ////errormsg = errormsg + "\nentered :" + TotalEntered + "\tPull Qty :" + pullQty;
                    errormsg = errormsg + "\n" + MsgEnteredPullQuantityValidation.replace("{0}", TotalEntered).replace("{1}", pullQty);
                }
            }
            else {
                if (IsIgnoreCreditRule == "False" || IsIgnoreCreditRule == "false" || IsIgnoreCreditRule == false) {
                    errormsg = MsgCreditMoreAvailablePullQTY;
                }
            }
        }
    });

    return errormsg;
}

function PreparePullDataTableForSerialLotEdit(objPullItemDTO) {    
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
    columnarrIL.push({
        mDataProp: null, sClass: "read_only", sDefaultContent: '', fnRender: function (obj, val) {
            var strReturn = "<span name='spnLotSerialQuantity' id='spnLotSerialQuantity_" + obj.aData.ID + "'>" + obj.aData.LotSerialQuantity + "</span>";
            return strReturn;
        }
    });
    columnarrIL.push({
        mDataProp: null, sClass: "read_only", sDefaultContent: '', fnRender: function (obj, val) {

            var strReturn = "<input type='hidden' name='hdnRowUniqueId' value='" + obj.aData.ID + "_" + obj.aData.ItemGUID + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnLotNumberTracking' value='" + obj.aData.LotNumberTracking + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnSerialNumberTracking' value='" + obj.aData.SerialNumberTracking + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnDateCodeTracking' value='" + obj.aData.DateCodeTracking + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnExpiration' value='" + obj.aData.Expiration + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnExpirationDate' value='" + obj.aData.strExpirationDate + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnBinNumber' id='hdnBinNumber' value='" + obj.aData.BinNumber + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnBinIDValue' id='hdnBinIDValue' value='" + obj.aData.BinID + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnIsIgnoreCreditRule' id='hdnIsIgnoreCreditRule' value='" + objPullItemDTO.IsIgnoreCreditRule + "' />";            

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
    columnarrIL.push({
        mDataProp: null, sClass: "read_only", sDefaultContent: '', fnRender: function (obj, val) {
            return strReturn = "<input type='text'  value='" + obj.aData.Expiration + "' id='txtExpirationDate_" + obj.aData.KitDetailGUID + "' name='txtExpirationDate' class='text-boxinner dateTextbox txtExpiration' />";
        }
    });
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
        "sAjaxSource": _PullMasterList.urls.PullLotSrSelectionForCreditEditPopupUrl,
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            if (aData.IsConsignedLotSerial == true) {
                nRow.className = "even trconsigned";
            }
        },
        "fnInitComplete": function (oSettings) {
            var strAllSelected = "";

            $("#hdnSelectedId_" + objPullItemDTO.ItemGUID).val();
            if (objPullItemDTO.LotNumberTracking != BoolTrueString && objPullItemDTO.SerialNumberTracking != BoolTrueString) {
                $('#' + objPullItemDTO.tableID).dataTable().fnSetColumnVis(0, false);
            }
            if (objPullItemDTO.DateCodeTracking != BoolTrueString) {
                $('#' + objPullItemDTO.tableID).dataTable().fnSetColumnVis(4, false);
            }
        },
        "fnServerData": function (sSource, aoData, fnCallback, oSettings) {

            aoData.push({ "name": "ItemGUID", "value": objPullItemDTO.ItemGUID });
            aoData.push({ "name": "PullGUID", "value": objPullItemDTO.PullGUID });
            aoData.push({ "name": "BinID", "value": objPullItemDTO.BinID });
            aoData.push({ "name": "BinNumber", "value": objPullItemDTO.BinNumber });
            if (objPullItemDTO.ItemGUID != '00000000-0000-0000-0000-000000000000' && objPullItemDTO.ItemGUID != '')
                aoData.push({ "name": "PullQuantity", "value": FormatedCostQtyValues($("#txtPoolQuantity_" + objPullItemDTO.ItemGUID).val(), 2) });
            else
                aoData.push({ "name": "PullQuantity", "value": FormatedCostQtyValues($("#txtPoolQuantity_" + objPullItemDTO.ToolGUID).val(), 2) });
            aoData.push({ "name": "InventoryConsuptionMethod", "value": objPullItemDTO.InventoryConsuptionMethod });
            aoData.push({ "name": "CurrentLoaded", "value": $("#hdnCurrentLoadedIds_" + objPullItemDTO.ItemGUID).val() });
            aoData.push({ "name": "CurrentDeletedLoaded", "value": $("#hdnCurrentDeletedLoadedIds_" + objPullItemDTO.ItemGUID).val() });
            aoData.push({ "name": "ViewRight", "value": objPullItemDTO.ViewRight });
            aoData.push({ "name": "IsDeleteRowMode", "value": isDeleteSrLotRowEdit });
            aoData.push({ "name": "IsIgnoreCreditRule", "value": objPullItemDTO.IsIgnoreCreditRule });
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
                    isDeleteSrLotRowEdit = false;
                    $('.ShowAllOptionsSL').click(function () {
                        $(this).siblings('.AutoSerialLot').trigger("focus");
                        $(this).siblings(".AutoSerialLot").autocomplete("search", "");
                    });

                    $('#' + objPullItemDTO.tableID).each(function (i) {                                             
                        $(this).find('input.dateTextbox').removeClass('hasDatepicker');
                        var currentDatePicker = $(this);
                        $(this).find('input.dateTextbox').datepicker({
                            dateFormat: RoomDateJSFormat, showButtonPanel: true,
                            clearText: 'Clear', onClose: function () {                               
                                this.focus();
                            }
                        });
                    });

                    if (objPullItemDTO.ViewRight == "ViewOnly") {
                        $("input[type='text'][name='txtLotOrSerailNumberViewOnly']").keypress(function () {
                            return false;
                        });

                        $("#divPreCreditInforSerialLotEdit input[type='text'][name='txtPullQty']").keypress(function () {
                            return false;
                        });
                    }

                    LenAfterRebind = $('#' + objPullItemDTO.tableID).find("tbody").find("tr").length;
                    if (LenBeforeRebind == LenAfterRebind && IsLoadMoreLotsClicked == true) {
                        alert(MsgNoLocationToAdd);
                    }
                    IsLoadMoreLotsClicked = false;
                }
            });
        }
    });
}

function SavePullCreditEdit(arr) {    
    $.ajax({
        "url": _PullMasterList.urls.SavePullCreditEditUrl, //"/Pull/SavePullCreditEdit",
        "data": JSON.stringify(arr),
        "type": 'POST',
        "async": false,
        "cache": false,
        "dataType": "json",
        "contentType": "application/json",
        success: function (result) {            
            alert(result.Message);            
            gblPreCreditObjToSaveForCredit = null;
            if (result.Status == true) {               
                DeletedRowObject = "";
                isDeleteSrLotRowEdit = false;
                $('#myDataTable').dataTable().fnStandingRedraw();
                $('#divPreCreditInforSerialLotEdit').dialog('close');
            }
        },
        error: function (xhr) {            
            alert(MsgSaveCreditAjaxError);
        }
    });
}

/////////////// New added for allow More Credit for Serial,lot and date code ////////////////


/* For Edit Credit for Lot,serial and datecode end */

