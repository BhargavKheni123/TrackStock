var oTableProjectItems;
//var deleteURL1 = "/WorkOrder/WOItemsDelete";
var deleteURL1 = "/Pull/DeletePullMasterRecords";
var NotAllowedCharCodes = [9, 16, 17, 18, 19, 20, 27, 33, 34, 35, 36, 37, 38, 39, 40];
//var DTName = _CreateWOItem.DTName; //'WOItemsTable' + _CreateWOItem.WorkOrderGUID;
var bIsFilter = false;
var anOpen = [];
var sImageUrl1 = "/Content/images/";
var objWODetailGridColumns = {};

var _CreateWOItem = (function ($) {
    var self = {};

    self.isSaveGridState = false;
    self.WorkOrderGUID = null;

    self.DTName = null;

    self.gridObj = new _GridWrapper();
    var gridSelRows = [];

    self.urls = {
        UpdatePullDataUrl: null,
        SaveGridStateUrl: null,
        LoadGridStateUrl: null,
        CreateRequisitionForMaintenanceUrl: null,
        LoadWOItemsUrl: null,
        RequisitionListUrl: null,
        WOToolCheckoutsUrl: null,
        PullDetailsUrl: null,
        CreateWOItemsAjaxUrl: null
    };

    self.initUrls = function (UpdatePullDataUrl, SaveGridStateUrl, LoadGridStateUrl
        , CreateRequisitionForMaintenanceUrl, LoadWOItemsUrl, RequisitionListUrl
        , WOToolCheckoutsUrl, PullDetailsUrl, CreateWOItemsAjaxUrl
    ) {
        self.urls.UpdatePullDataUrl = UpdatePullDataUrl;
        self.urls.SaveGridStateUrl = SaveGridStateUrl;
        self.urls.LoadGridStateUrl = LoadGridStateUrl;
        self.urls.CreateRequisitionForMaintenanceUrl = CreateRequisitionForMaintenanceUrl;
        self.urls.LoadWOItemsUrl = LoadWOItemsUrl;
        self.urls.RequisitionListUrl = RequisitionListUrl;
        self.urls.WOToolCheckoutsUrl = WOToolCheckoutsUrl;
        self.urls.PullDetailsUrl = PullDetailsUrl;
        self.urls.CreateWOItemsAjaxUrl = CreateWOItemsAjaxUrl;
    };

    

    self.init = function (WorkOrderGUID) {
        self.WorkOrderGUID = WorkOrderGUID;
        self.DTName = 'WOItemsTable' + self.WorkOrderGUID;
        self.gridObj.tableId = self.DTName;
        self.initEvents();
    };

    self.initEvents = function () {
        $(document).ready(function () {
            self.initDataTable();

            $('.DTTT_container').css('z-index', '-1');


            if (isCostWOI == 'False') {
                HideColumnUsingClassName(_CreateWOItem.DTName);

                ColumnsToHideinPopUp.push(5);
                ColumnsToHideinPopUp.push(20);
                ColumnsToHideinPopUp.push(21);
                ColumnsToHideinPopUp.push(22);
                ColumnsToHideinPopUp.push(23);
                ColumnsToHideinPopUp.push(47);
                ColumnsToHideinPopUp.push(48);
                ColumnsToHideinPopUp.push(49);
                ColumnsToHideinPopUp.push(50);

                oTableProjectItems.fnSetColumnVis(5, false);
                oTableProjectItems.fnSetColumnVis(20, false);
                oTableProjectItems.fnSetColumnVis(21, false);
                oTableProjectItems.fnSetColumnVis(22, false);
                oTableProjectItems.fnSetColumnVis(23, false);
                oTableProjectItems.fnSetColumnVis(47, false);
                oTableProjectItems.fnSetColumnVis(48, false);
                oTableProjectItems.fnSetColumnVis(49, false);
                oTableProjectItems.fnSetColumnVis(50, false);
            }

            $("#project-spend-limit-basic-modal-content").on("click", "#btnModelYesPSLimit", function () {

                if (mntsGUID == GuidEmpty) {
                    var url = _CreateWOItem.urls.RequisitionListUrl;
                    url = url + '?fromPull=' + 'yes'
                    window.location.href = url;
                }
                else if (mntsGUID != GuidEmpty) {
                    $.ajax({
                        "url": _CreateWOItem.urls.CreateRequisitionForMaintenanceUrl,
                        "data": "{ mntsGUID :mntsGUID }",
                        "type": "POST",
                        "dataType": "json",
                        "contentType": 'application/json; charset=utf-8',
                        "success": function (response) {
                            if (response.Status == "ok") {
                                $.modal.impl.close();
                                $("#ItemModelTemp").dialog('close');
                                mntsGUID = response.mntsDTO.GUID;
                                woGUID = response.mntsDTO.WorkorderGUID;
                                ReqGUID = response.mntsDTO.RequisitionGUID;
                                $("#tab31").show();
                                $("#tab31").attr("IsEnable", true);
                                $("#tab31").click();
                            }
                            //else {

                            //}
                            $('#DivLoading').hide();
                        },
                        error: function (response) {

                            $('#DivLoading').hide();
                            $("#spanGlobalMessage").html(response.message);
                            $('div#target').fadeToggle();
                            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                        }
                    });
                }
                else {
                    var url = _CreateWOItem.urls.RequisitionListUrl;
                    url = url + '?fromPull=' + 'yes'
                    window.location.href = url;
                }
            });

            if (ViewBagViewOnly == 'True') {

                $('select', _CreateWOItem.DTName)
                    .attr('disabled', 'disabled');

                $(':input[type=text], textarea', _CreateWOItem.DTName)
                    .attr('readonly', 'readonly');

                $('#saveRows').attr('style', 'display:none');
                $('#deleteRows1').attr('style', 'display:none');
            }

            
            $('#deleteRows1').unbind("click");
            $('#Gobtn1').unbind("click");
            $('#clear_QLItem_filter').unbind("click");
            $('#saveRows').die('click');
            $('#PageNumber1').unbind("keydown");
            $('#btnModelYesInnerGrid').die('click');
            $('[name = "pullDrillDown"]').die("click");


            // To adjust the print block next to colvis button
            $('#btnblock').css({ 'margin-right': '45px' });
            $('#' + _CreateWOItem.DTName + ' tbody tr').die('tap click');
            var lastChecked1;
            var starttrvalue1 = "";
            $('#' + _CreateWOItem.DTName + ' tbody tr').live('tap click', function (e) {
                //$(this).toggleClass('row_selected');
                //return false;
                 
                if ($(e.target).hasClass("control") == true || e.target.nodeName.toLowerCase() == "img" || e.target.type == "checkbox" || e.target.type == "radio" || e.target.type == "text" || e.target.type == "button" || $(e.target).is('a') == true || $(e.target).is('span') == true
                    || $(e.target).hasClass("selectBox")
                ) {
                    e.stopPropagation();
                }
                else {
                    if (lastChecked1 !== undefined && lastChecked1 != null && !lastChecked1) {
                        lastChecked1 = this;
                    }

                    if (e.shiftKey) {
                        var start = $('#' + _CreateWOItem.DTName + ' tbody tr').index(this);
                        var end = $('#' + _CreateWOItem.DTName + ' tbody tr').index(lastChecked1);

                        var stringval1 = readCookieforshift("selectstartindex");
                        if (stringval1 != null) {
                            var endindex = $(this).closest('tr').attr('id');
                            createCookieforshift("selectendindex", endindex, 1);
                            if ($("#hdnPageName").val() !== undefined) {
                                var pagename = '';
                                pagename = $("#hdnPageName").val();
                                GetOnlyIdsForPassPagesForshift(pagename, true);
                            }
                        }

                        for (i = Math.min(start, end); i <= Math.max(start, end); i++) {
                            if (!$('#' + _CreateWOItem.DTName + ' tbody tr').eq(i).hasClass('row_selected')) {
                                $('#' + _CreateWOItem.DTName + ' tbody tr').eq(i).addClass("row_selected");
                            }
                        }
                        if (window.getSelection) {
                            if (window.getSelection().empty) {  // Chrome
                                window.getSelection().empty();
                            } else if (window.getSelection().removeAllRanges) {  // Firefox
                                window.getSelection().removeAllRanges();
                            }
                        } else if (document.selection) {  // IE?
                            document.selection.empty();
                        }
                    } else if ((e.metaKey || e.ctrlKey)) {
                        $(this).toggleClass('row_selected');
                    } else {
                        $(this).toggleClass('row_selected');


                        if ($(this).hasClass('row_selected')) {
                            (starttrvalue1 == "") ? starttrvalue1 = $(this).closest('tr').attr('id') : starttrvalue1 = starttrvalue1 + "," + $(this).closest('tr').attr('id');
                            createCookieforshift("selectstartindex", starttrvalue1, 1);
                        } else {

                            var stringval = readCookieforshift("selectstartindex");
                            if (stringval != "undefined") {
                                if (stringval != null) {
                                    var tmp = stringval.split(',');
                                    var index = tmp.indexOf($(this).closest('tr').attr('id'));
                                    if (index !== -1) {
                                        tmp.splice(index, 1);
                                        stringval = tmp.join(',');
                                        createCookieforshift("selectstartindex", stringval, 1);
                                    }
                                }
                            }

                        }
                    }

                    lastChecked1 = this;
                }
            });


            $('#' + _CreateWOItem.DTName).on("click", "td", function (event) {
                event.preventDefault();
            });


            //  var countK = 0;

            /* Add a click handler for the delete rows multiple rows */
            /*Functions used for nasted data binding START*/
            //$(document).on("click", 'img[name = "pullDrillDown"]', function (event) {
            //$('[name = "pullDrillDown"]').on("click", function (event) {
            $('#' + _CreateWOItem.DTName).on("click", '[name = "pullDrillDown"]', function (event) {
                //  countK++;
                // alert(DTName + " - " + countK.toString());
                //  if (countK <= 1) {
                var nTr = this.parentNode.parentNode;
                var i = $.inArray(nTr, anOpen);
                if (i === -1) {
                    $(this).attr('src', sImageUrl1 + "drildown_close.jpg");
                    $('#' + _CreateWOItem.DTName).dataTable().fnOpen(nTr, fnWOIFormatDetails($('#' + _CreateWOItem.DTName).dataTable(), nTr), '');
                    anOpen.push(nTr);
                    //$('#' + _CreateWOItem.DTName).dataTable().fnDraw(); // commented by amit t to fix issue after ajax data load
                }
                else {
                    $(this).attr('src', sImageUrl1 + "drildown_open.jpg");
                    $('#' + _CreateWOItem.DTName).dataTable().fnClose(nTr);
                    anOpen.splice(i, 1);
                }
                // }
            });


            $('#deleteRows1').click(function () {
                /* IF PRINT PREVIEW DONT SHOW CONTEXT MENU */
                if ($("body").hasClass('DTTT_Print')) {
                    return false;
                }
                /* IF PRINT PREVIEW DONT SHOW CONTEXT MENU */


                var anSelected = fnGetSelected(oTableProjectItems);
                var stringIDs = "";
                for (var i = 0; i <= anSelected.length - 1; i++) {
                    //anSelected[0].cells[0].innerHTML
                    stringIDs = stringIDs + $(anSelected[i]).find("#hdnPullID").val() + ",";
                }
                if (anSelected.length !== 0) {
                    $('#Inner-Grid-basic-modal-content').modal();
                    IsDeletePopupOpen = false;
                }
            });
            var TempIdsForDelete = "";
            $("#btnModelYesInnerGrid").live("click", function () {
                var anSelected = fnGetSelected(oTableProjectItems);
                var stringIDs = "";
                var toolCheckInOutGuids = "";
                for (var i = 0; i <= anSelected.length - 1; i++) {
                    var txtQuantityPulled = $(anSelected[i]).find('#txtQuantityPulled').val() == '' ? 0 : $(anSelected[i]).find('#txtQuantityPulled').val();
                    var txtAvalQuantity = $(anSelected[i]).find('#hdnQuantity').val() == '' ? 0 : $(anSelected[i]).find('#hdnQuantity').val();
                    var txxt = $(anSelected[i]).find('#txtQty').val() == '' ? 0 : $(anSelected[i]).find('#txtQty').val();
                    var vBinID = $(anSelected[i]).find("#item_BinID").val();
                    var txtQty = parseFloat(txxt, 10);

                    txtQuantityPulled = parseFloat(txtQuantityPulled, 10);
                    txtAvalQuantity = parseFloat(txtAvalQuantity, 10);

                    //            if (txtQuantityPulled == txtAvalQuantity || txtQuantityPulled == 0) {
                    //                stringIDs = stringIDs + $(anSelected[i]).find("#hdnID").val() + ",";
                    //            }
                    //            else {
                    //                TempIdsForDelete = TempIdsForDelete + $(anSelected[i]).find("#hdnID").val() + ",";
                    //            }
                    var toolGuid = $(anSelected[i]).find("#hdnToolGuid").val();
                    if (typeof (toolGuid) != "undefined" && toolGuid != null && toolGuid.length > 0) {
                        toolCheckInOutGuids = toolCheckInOutGuids + $(anSelected[i]).find("#hdnID").val() + ",";
                    }
                    else
                    {
                        stringIDs = stringIDs + $(anSelected[i]).find("#hdnPullID").val() + ",";
                    }
                    
                }

                if (toolCheckInOutGuids.length !== 0)
                {
                    $.ajax({
                        'url': "/WorkOrder/DeleteWorkorderTool",
                        "type": "POST",
                        data: { ToolCheckInOutHistoryIds: toolCheckInOutGuids, WOGUID: _CreateWOItem.WorkOrderGUID },
                        success: function (response) {
                            if (response.Status == "ok") {
                                for (var i = 0; i <= anSelected.length - 1; i++) {
                                    oTableProjectItems.fnDeleteRow(anSelected[i]);
                                }
                                if (anSelected.length > 0 && stringIDs.length < 1) {
                                    $("#spanGlobalMessage").html(MsgRecordDeletedSuccessfully.replace("{0}", anSelected.length));
                                    $('div#target').fadeToggle();
                                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                                }                                
                                if (stringIDs.length < 1)
                                {
                                    ReDirectData();
                                }                                
                            }
                        },
                        error: function (response) {
                            // through errror message
                        }
                    });

                    if (stringIDs.length < 1)
                    {
                        $.modal.impl.close();
                    }                    
                }
                if (stringIDs.length !== 0) {
                    $.ajax({
                        'url': deleteURL1,
                        data: { ids: stringIDs, fromwhere: "wo", WOGUID: _CreateWOItem.WorkOrderGUID },
                        success: function (response) {
                           
                            if (response.Status == "ok") {
                                for (var i = 0; i <= anSelected.length - 1; i++) {
                                    oTableProjectItems.fnDeleteRow(anSelected[i]);
                                }
                                if (anSelected.length > 0) {
                                    if (TempIdsForDelete.length > 0) {
                                        $("#spanGlobalMessage").html(MsgRecordPartialPullNotDelete.replace("{0}", anSelected.length).replace("{1}", TempIdsForDelete));
                                    }
                                    else {
                                        $("#spanGlobalMessage").html(MsgRecordDeletedSuccessfully.replace("{0}", anSelected.length));
                                    }

                                    // here needs to write code for delete the data from PULL Master
                                    // based on WO Detail ID , needs to remove the record from Pull Master Table ...
                                }
                                $('div#target').fadeToggle();
                                $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                                ReDirectData();
                            }
                        },
                        error: function (response) {

                            // through errror message
                        }
                    });
                    $.modal.impl.close();
                }
                else {
                    if (TempIdsForDelete.length > 0) {
                        $("#spanGlobalMessage").html(MsgRecordNotDeletedPartialPull.replace("0", MsgRecordNotDeletedPartialPull));
                        $('div#target').fadeToggle();
                        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                        $.modal.impl.close();
                        TempIdsForDelete = "";
                    }
                }
            });

            $('#PageNumber1').keydown(function (e) {
                var code = (e.keyCode ? e.keyCode : e.which);
                if (code == 13) {
                    $("#Gobtn1").click();
                    return false;
                }
            });

            $("#Gobtn1").click(function () {
                var pval = $('#PageNumber1').val();
                if (pval == "" || pval.match(/[^0-9]/)) {
                    return;
                }
                if (pval == 0)
                    return;
                $('#' + _CreateWOItem.DTName).dataTable().fnPageChange(Number(pval - 1));
                $('#PageNumber1').val('');
            });


            //Apply filter
            $("#InnerItem_filter").keyup(function (e) {
                var code = (e.keyCode ? e.keyCode : e.which);
                var index = $.inArray(code, NotAllowedCharCodes);
                if (code == 13) {
                }
                else {
                    fnFilterGlobalPS();
                }
            });

            //Keydown event is required to handle ENTER KEY to work in IE
            $("#InnerItem_filter").keydown(function (e) {
                var code = (e.keyCode ? e.keyCode : e.which);
                if (code == 13) {
                    fnFilterGlobalPS();
                }
            });

            //Clear Filter
            $("#clear_QLItem_filter").click(function () {
                $("#InnerItem_filter").val('');
                fnFilterGlobalPS();
                $("#InnerItem_filter").focus();
                return false;
            });



            /* DATA TABLE GRID COMMON FUNCTIONS END */
            $("#ColumnOrderSetup1").off('click')
            $("#ColumnOrderSetup1").on('click', function () {
                WOItemFlag = true;
                $("#divDetailGridReorderPopup").data({ 'DataTableName': _CreateWOItem.DTName, 'ListName': 'WorkOrderDetails', 'ColumnObjectName': 'objWODetailGridColumns' }).dialog("open");
                return false;
            });



        }); // ready

        $(document).keyup(function (e) {
            if (ViewBagViewOnly == 'True') {
                return false;
            }
            if (e.target.type == 'text' && e.target.localName == 'input')
                return false;

            var code = (e.keyCode ? e.keyCode : e.which);
            if (code == 46) {
                $('#deleteRows1').click();
            }
        });

    };// init events

    self.initDataTable = function () {
        var tableName = $('table[id^=WOItemsTable]').attr("id");
        //objColumns = GetGridHeaderColumnsObject(tableName);
        //var rowno = 0;
        $('form').areYouSure({ 'message': MsgLostChangesConfirmation });
        //$('#' + tableName + ' tr').each(function () {

        //    var qtyTempPoolQuantity = $('#qtyTempPoolQuantity_' + rowno).text();
        //    if (qtyTempPoolQuantity != "")
        //        $('#qtyTempPoolQuantity_' + rowno).text(FormatedCostQtyValues(parseFloat(qtyTempPoolQuantity), 2));

        //    var tempCostValue = $('#CostTempCost_' + rowno).text();
        //    if (tempCostValue != "")
        //        $('#CostTempCost_' + rowno).text(FormatedCostQtyValues(parseFloat(tempCostValue), 1));

        //    var tempPullCostVal = $('#CostTempPullCost_' + rowno).text();

        //    if (tempPullCostVal != "")
        //        $('#CostTempPullCost_' + rowno).text(FormatedCostQtyValues(parseFloat(tempPullCostVal), 1));
        //    var tempPullSellPrice = $('#CostTempSellPrice_' + rowno).text();

        //    if (tempPullSellPrice != "")
        //        $('#CostTempSellPrice_' + rowno).text(FormatedCostQtyValues(parseFloat(tempPullSellPrice), 1));
        //    rowno += 1;
        //});
        //var DTName = 'WOItemsTable' + _CreateWOItem.WorkOrderGUID;
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
        $(".myDatePicker").datepicker({ dateFormat: 'm/d/yy' });
        $("#saveRows").click(function () {
            SaveAllClick();
        });
        //var gaiSelected = [];
        objWODetailGridColumns = GetGridChildGridColumnsObject(self.DTName);

        var aoColumnsWOI = [
            {
                "mDataProp": null,
                "bSortable": false,
                sClass: "read_only center NotHide RowNo",
                "sDefaultContent": "",
            },
            {
                "mDataProp": null,
                "bSortable": false, "sClass": "read_only",
                "sDefaultContent": ""
                , "fnRender": function (obj, val) {
                    var item = obj.aData;
                    var ret = "<img src='/Content/images/drildown_open.jpg' name='pullDrillDown' alt='Show/Hide Pull' />";
                    ret = ret + "<input type ='hidden' id ='hdnID' value = '" + (item.GUID === null ? "" : item.GUID) + "' />";
                    ret = ret + "<input type='hidden' id='hdnPullID' value='" + (item.ID === null ? "" : item.ID) + "' />";
                    ret = ret + "<input type='hidden' id='hdnReqDetailGUID' value='" + (item.RequisitionDetailGUID === null ? "" : item.RequisitionDetailGUID) + "' />";
                    ret = ret + "<input type='hidden' id='hdnWorkOrderGUID' value='" + (item.WorkOrderDetailGUID === null ? "" : item.WorkOrderDetailGUID) + "' />";
                    ret = ret + "<input type='hidden' id='hdnToolGuid' value='" + (item.ToolGUID === null ? "" : item.ToolGUID) + "' />";
                    return ret;
                }
            },
            {
                "mDataProp": "ActionType",
                "bSortable": true, "sClass": "read_only"
            },
            {
                "mDataProp": "PoolQuantity",
                "bSortable": true, "sClass": "numericalign read_only"
                , "fnRender": function (obj, val) {
                    var item = obj.aData;
                    var rowNo = obj.iDataRow; // starts with 0
                    return '<label id="qtyTempPoolQuantity_' + rowNo + '">' + FormatedCostQtyValues(parseFloat(val), 2) + '</label>';
                }

            },
            {
                "mDataProp": "PullOrderNumber",
                "bSortable": true, "sClass": "read_only"
            },
            {
                "mDataProp": "PullCost",
                "bSortable": true, "sClass": "numericalign  read_only isCost"
                , "fnRender": function (obj, val) {
                    var item = obj.aData;
                    var rowNo = obj.iDataRow; // starts with 0
                    return '<label id="CostTempCost_' + rowNo + '">' + FormatedCostQtyValues(parseFloat(val), 1) + '</label>';

                }
            },
            {
                "mDataProp": "BinNumber",
                "bSortable": true, "sClass": "read_only"
            },
            {
                "mDataProp": "ProjectSpendName",
                "bSortable": true, "sClass": "read_only"
            },
            {
                "mDataProp": "ID",
                "bSortable": true, "sClass": "read_only"
            },
            {
                "mDataProp": "ItemNumber",
                "bSortable": true, "sClass": "read_only"
            },
            {
                "mDataProp": "ToolName",
                "bSortable": true, "sClass": "read_only"
            },
            {
                "mDataProp": "Created",
                "bSortable": true, "sClass": "read_only"
                , "fnRender": function (obj, val) {
                    var item = obj.aData;
                    return item.CreatedDate;

                }

            },
            {
                "mDataProp": "Updated",
                "bSortable": true, "sClass": "read_only"
                , "fnRender": function (obj, val) {
                    var item = obj.aData;
                    return item.UpdatedDate;

                }
            },
            {
                "mDataProp": "RoomName",
                "bSortable": true, "sClass": "read_only"
            },
            {
                "mDataProp": "UpdatedByName",
                "bSortable": true, "sClass": "read_only"
            },
            {
                "mDataProp": "CreatedByName",
                "bSortable": true, "sClass": "read_only"
            },
            {
                "mDataProp": null,
                "bSortable": true, "sClass": "read_only",
                "sDefaultContent": "",
                "fnRender": function (obj, val) {
                    var item = obj.aData;
                    if (item.ItemType === 1) {
                        return "<text>Item</text>";
                    }
                    else if (item.ItemType === 2) {
                        return "<text>Kit</text>";
                    }
                    else if (item.ItemType === 3) {
                        return "<text>Quick List</text>";
                    }
                    else if (item.ItemType === 4) {
                        return "<text>Labor</text>";
                    }
                    else {
                        return "";
                    }
                }
            },
            {
                "mDataProp": "Description",
                "bSortable": true, "sClass": "read_only"
            },
            {
                "mDataProp": "CategoryName",
                "bSortable": true, "sClass": "read_only"
            },
            {
                "mDataProp": "Unit",
                "bSortable": true, "sClass": "read_only"
            },
            {
                "mDataProp": "ItemCost",
                "bSortable": true, "sClass": "numericalign read_only isCost "
                , "fnRender": function (obj, val) {
                    var item = obj.aData;
                    var rowNo = obj.iDataRow; // starts with 0
                    return "<label id='CostTempPullCost_" + rowNo + "'>" + FormatedCostQtyValues(parseFloat(val), 1) + "</label>";
                }
            },
            {
                "mDataProp": "Markup",
                "bSortable": true, "sClass": "numericalign read_only "//"read_only isCost"
            },

            {
                "mDataProp": "SellPrice",
                "bSortable": true, "sClass": "numericalign read_only isCost"
                , "fnRender": function (obj, val) {
                    var item = obj.aData;
                    var rowNo = obj.iDataRow; // starts with 0
                    return "<label id='CostTempSellPrice_" + rowNo + "'>" + FormatedCostQtyValues(parseFloat(val), 1) + "</label>";
                }
            },
            {
                "mDataProp": "ExtendedCost",
                "bSortable": true, "sClass": "numericalign read_only isCost"
                , "fnRender": function (obj, val) {
                    var item = obj.aData;
                    var rowNo = obj.iDataRow; // starts with 0
                    return "<label id='CostExtended_" + rowNo + "'>" + val + "</label>";
                }
            },
            {
                "mDataProp": "DefaultPullQuantity",
                "bSortable": true, "sClass": "numericalign read_only isCost "
            },
            {
                "mDataProp": "ManufacturerNumber",
                "bSortable": true, "sClass": "read_only"
            },
            {
                "mDataProp": "Manufacturer",
                "bSortable": true, "sClass": "read_only"
            },
            {
                "mDataProp": "SupplierPartNo",
                "bSortable": true, "sClass": "read_only"
            },
            {
                "mDataProp": "SupplierName",
                "bSortable": true, "sClass": "read_only"
            },
            {
                "mDataProp": "LongDescription",
                "bSortable": true, "sClass": "read_only"
            },
            {
                "mDataProp": "GLAccount",
                "bSortable": true, "sClass": "read_only"
            },
            {
                "mDataProp": "Taxable",
                "bSortable": true, "sClass": "read_only",
                "fnRender": function (obj, val) {
                    return GetBoolInFormat(obj, val);
                }
            },
            {
                "mDataProp": "InTransitquantity",
                "bSortable": true, "sClass": "numericalign read_only ",
                "fnRender": function (obj, val) {
                    return FormatedCostQtyValues(parseFloat(val), 2);
                }
            },
            {
                "mDataProp": "OnOrderQuantity",
                "bSortable": true, "sClass": "numericalign read_only",
                "fnRender": function (obj, val) {
                    return FormatedCostQtyValues(parseFloat(val), 2);
                }
            },
            {
                "mDataProp": "OnTransferQuantity",
                "bSortable": true, "sClass": "numericalign read_only",
                "fnRender": function (obj, val) {
                 return FormatedCostQtyValues(parseFloat(val), 2);
                }
            },
            {
                "mDataProp": "AverageUsage",
                "bSortable": true, "sClass": "numericalign read_only"
                , "fnRender": function (obj, val) {
                    return FormatedCostQtyValues(parseFloat(val), 4);
                }
            },
            {
                "mDataProp": "Turns",
                "bSortable": true, "sClass": "numericalign read_only",
                 "fnRender": function (obj, val) {
                  return FormatedCostQtyValues(parseFloat(val), 4);
                 }
            },
            {
                "mDataProp": "OnHandQuantity",
                "bSortable": true, "sClass": "numericalign read_only",
                "fnRender": function (obj, val) {
                    return FormatedCostQtyValues(parseFloat(val), 2);
                }
            },
            {
                "mDataProp": "CriticalQuantity",
                "bSortable": true, "sClass": "numericalign read_only",
                "fnRender": function (obj, val) {
                    return FormatedCostQtyValues(parseFloat(val), 2);
                }
            },
            {
                "mDataProp": "MinimumQuantity",
                "bSortable": true, "sClass": "numericalign read_only",
                "fnRender": function (obj, val) {
                    return FormatedCostQtyValues(parseFloat(val), 2);
                }
            },
            {
                "mDataProp": "MaximumQuantity",
                "bSortable": true, "sClass": "numericalign read_only",
                "fnRender": function (obj, val) {
                    return FormatedCostQtyValues(parseFloat(val), 2);
                }
            },
            {
                "mDataProp": "CostUOMName",
                "bSortable": true, "sClass": "numericalign read_only"
            },
            {
                "mDataProp": "AddedFrom",
                "bSortable": true, "sClass": "read_only"
            },
            {
                "mDataProp": "EditedFrom",
                "bSortable": true, "sClass": "read_only"
            },
            {
                "mDataProp": "ReceivedOnWeb",//"ReceivedOnWebDate",
                "bSortable": true, "sClass": "read_only"
                , "fnRender": function (obj, val) {
                    var item = obj.aData;
                    return item.ReceivedOnWebDate;
                }
            },
            {
                "mDataProp": "ReceivedOn",//"ReceivedOnDate",
                "bSortable": true, "sClass": "read_only"
                , "fnRender": function (obj, val) {
                    var item = obj.aData;
                    return item.ReceivedOnDate;
                }
            }, {
                "mDataProp": "IsEDISent",
                "bSortable": true, "sClass": "read_only"
                , "fnRender": function (obj, val) {
                    var item = obj.aData;
                    var ret = "no";
                    if (typeof item.IsEDISent !== "undefined" && item.IsEDISent !== null) {
                        ret = item.IsEDISent ? "yes" : "no";
                    }
                    return ret;
                }
            }, {
                "mDataProp": "PullItemCost",
                "bSortable": true, "sClass": "numericalign read_only isCost"
            }
            , {
                "mDataProp": "PullItemSellPrice",
                "bSortable": true, "sClass": "numericalign read_only isCost"
            }
            , {
                "mDataProp": "PullMarkup",
                "bSortable": true, "sClass": "read_only isCost"
            }
            , {
                "mDataProp": "ItemCostOnPullDate",
                "bSortable": true, "sClass": "read_only isCost"
            } 
            , {
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
            }
        ];

        $.each(arrItemMasterWOI, function (index, val) {
            aoColumnsWOI.push(val);
        });

        $.each(arrPullMasterWOI, function (index, val) {
            aoColumnsWOI.push(val);
        });

        oTableProjectItems = $('#' + self.DTName).dataTable({
            "bJQueryUI": true,
            "bRetrieve": true,
            "bDestroy": true,
            "bScrollCollapse": true,
            "sScrollX": "900px",
            "sDom": 'RC<"top"lp<"clear">>rt<"bottom"i<"clear">>',
            //"aaSorting": [],
            "aaSorting": [[1, "desc"]],
            "sPaginationType": "full_numbers",
            "bProcessing": true,
            "bStateSave": true,
            "oLanguage": oLanguageWOItem,
            "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                $("td.RowNo:first", nRow).html(this.fnSettings()._iDisplayStart + iDisplayIndex + 1);
                return nRow;
            },
            "fnStateSaveParams": self.grid_SaveState,
            "fnStateLoad": function (oSettings) {
                var o;
                $.ajax({
                    "url": self.urls.LoadGridStateUrl,
                    "type": "POST",
                    data: { ListName: 'WorkOrderDetails' },
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
            "sAjaxSource": self.urls.CreateWOItemsAjaxUrl, // added by amit t
            "fnServerData": function (sSource, aoData, fnCallback, oSettings) { // added by amit t
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
                if (oSettings.aaSorting.length != 0) {
                    //aoData.push({ "name": "SortingField", "value": oSettings.aaSorting[0][3] });
                    var sortValue = ""
                    for (var i = 0; i <= oSettings.aaSorting.length - 1; i++) {
                        if (sortValue.length > 0)
                            sortValue += ", "
                        sortValue += arrCols[oSettings.aaSorting[i][0]] + ' ' + oSettings.aaSorting[i][1];

                    }
                    aoData.push({ "name": "SortingField", "value": sortValue });
                }
                else
                    aoData.push({ "name": "SortingField", "value": "0" });

                aoData.push({ "name": "IsArchived", "value": $('#IsArchivedRecords').is(':checked') });
                aoData.push({ "name": "IsDeleted", "value": $('#IsDeletedRecords').is(':checked') });
                aoData.push({ "name": "WOIGUID", "value": self.WorkOrderGUID });

                var getHeader = function () {
                    var $RequestVerificationToken = $("input[name='__RequestVerificationToken'][type='hidden']");
                    var token = $($RequestVerificationToken[0]).val();
                    return { "__RequestVerificationToken": token };
                };

                gridSelRows = self.gridObj.getSelectedRowNos("hdnID");

                oSettings.jqXHR = $.ajax({
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource,
                    "cache": false,
                    "async":false,
                    "data": aoData,
                    "headers": getHeader(),//{ "__RequestVerificationToken": $($("input[name='__RequestVerificationToken'][type='hidden']")[$("input[name='__RequestVerificationToken'][type='hidden']").length - 1]).val() },
                    "success": function (json) {

                        //var bindJson = [];

                        //$.each(json.aaData, function (index, obj) {
                        //    bindJson.push(obj);
                        //    //bindJson.push(new createWODTO(obj));
                        //});

                        //json.aaData = bindJson;
                        

                        fnCallback(json);
                    },
                    beforeSend: function () {
                        //$('#myDataTable').removeHighlight();
                        $('.dataTables_scroll').css({ "opacity": 0.2 });
                        $("#" + self.DTName).css({ "opacity": 0 });
                    },
                    complete: function () {
                        //$('.dataTables_scroll').css({ "opacity": 1 });
                        //if ($("#global_filter").val() != '') {
                        //    $('#myDataTable').highlight($("#global_filter").val());
                        //}
                        CallShowHideData();
                        //var rowno = 0;
                        //// moved from page load hare after ajax call
                        //$('#' + self.DTName + ' tr').each(function () {

                            //var qtyTempPoolQuantity = $('#qtyTempPoolQuantity_' + rowno).text();
                            //if (qtyTempPoolQuantity != "")
                            //    $('#qtyTempPoolQuantity_' + rowno).text(FormatedCostQtyValues(parseFloat(qtyTempPoolQuantity), 2));

                            //var tempCostValue = $('#CostTempCost_' + rowno).text();
                            //if (tempCostValue != "")
                            //    $('#CostTempCost_' + rowno).text(FormatedCostQtyValues(parseFloat(tempCostValue), 1));

                            //var tempPullCostVal = $('#CostTempPullCost_' + rowno).text();

                            //if (tempPullCostVal != "")
                            //    $('#CostTempPullCost_' + rowno).text(FormatedCostQtyValues(parseFloat(tempPullCostVal), 1));
                            //var tempPullSellPrice = $('#CostTempSellPrice_' + rowno).text();

                            //if (tempPullSellPrice != "")
                            //    $('#CostTempSellPrice_' + rowno).text(FormatedCostQtyValues(parseFloat(tempPullSellPrice), 1));
                            //rowno += 1;
                        //});
                         // reselect rows after ajax call
                        self.gridObj.toggleSelectRowNo(gridSelRows, "hdnID");
                        setTimeout(function () {
                            $('.dataTables_scroll').css({ "opacity": 1 });
                            $("#" + self.DTName).css({ "opacity": 1 });
                         
                        }, 500);
                    }
                })
            },
            "fnInitComplete": function () {
                $('.ColVis').detach().appendTo(".setting-arrow");

            },
            "aoColumns": aoColumnsWOI


        });
    }; // initdatatable
        
    self.grid_SaveState = function (oSettings, oData) {
        if (self.isSaveGridState            
        ) {
            oData.oSearch.sSearch = "";
            // if (!bIsFilter) {
            $.ajax({
                "url": _CreateWOItem.urls.SaveGridStateUrl,
                "type": "POST",
                data: { Data: JSON.stringify(oData), ListName: 'WorkOrderDetails' },
                "async": false,
                cache: false,
                "dataType": "json",
                "success": function (json) {
                    o = json;
                }
            });
            //  }
        }
        else {
            self.isSaveGridState = true;
        }
        bIsFilter = false;

    }

    // private functions

    function fnWOIFormatDetails(oTableProjectItems, nTr) {
        var sOut = '';
        var hdnID = $(nTr).find('#hdnID').val();
        var hdnReqDetailGUID = $(nTr).find('#hdnReqDetailGUID').val();
        var hdnToolGUID = $(nTr).find('#hdnToolGuid').val();
        var hdnWorkOrderGUID = $(nTr).find('#hdnWorkOrderGUID').val();
        WorkOrderItemGUID = hdnWorkOrderGUID;
        ItemUniqueID = hdnID;
        $('#DivLoading').show();
        if (hdnToolGUID != undefined && hdnToolGUID.length > 0 && hdnToolGUID != '00000000-0000-0000-0000-000000000000') {
            $.ajax({
                "url": _CreateWOItem.urls.WOToolCheckoutsUrl,
                data: { "WOGUID": hdnWorkOrderGUID, 'ToolGUID': hdnToolGUID, 'ToolCheckoutGUID': hdnID },
                "async": false,
                cache: false,
                "dataType": "text",
                "contentType": 'application/json',
                "success": function (json) {
                    sOut = json;
                    $('#DivLoading').hide();
                },
                error: function (response) {
                    //;
                }
            });
            return sOut;
        }

        else {//if (hdnID != undefined && hdnID.length > 0 && hdnID != '00000000-0000-0000-0000-000000000000') {
            $.ajax({
                "url": _CreateWOItem.urls.PullDetailsUrl,
                data: { ItemID: hdnID },
                "async": false,
                cache: false,
                "dataType": "text",
                "success": function (json) {
                    sOut = json;
                    $('#DivLoading').hide();
                },
                error: function (response) {
                    //;
                }
            });
            return sOut;
        }

    }

    return self;

})(jQuery); // _CreateWOItem end


function AddSingleItemToPullList(obj) {
    var txtQuantityPulled = $(obj).parent().parent().find('#txtQuantityPulled').val() == '' ? 0 : $(obj).parent().parent().find('#txtQuantityPulled').val();
    var txtAvalQuantity = $(obj).parent().parent().find('#hdnQuantity').val() == '' ? 0 : $(obj).parent().parent().find('#hdnQuantity').val();
    var txxt = $(obj).parent().parent().find('#txtQty').val() == '' ? 0 : $(obj).parent().parent().find('#txtQty').val();
    var vBinID; // = $(obj).parent().parent().find("#item_BinID").val();
    var vProjectGUID;
    var ProjectSpendName;
    var txtQty = parseFloat(txxt, 10);
    txtQuantityPulled = parseFloat(txtQuantityPulled, 10);
    txtAvalQuantity = parseFloat(txtAvalQuantity, 10);
    var vItemType = $(obj).parent().parent().find('#hdnItemType').val();

    if (vItemType != '4') {
        if ((txtQty + txtQuantityPulled) > txtAvalQuantity) {
            //alert('You can Pull Max to Available Quantity: ' + txtAvalQuantity);
            $("#spanGlobalMessage").html(MsgMaxAvailableQuantity.replace("{0}", txtAvalQuantity)); 
            $('div#target').fadeToggle();
            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
            return false;
        }
        if (txtQty <= 0) {
            //alert('Invalid Pull value.');
            $("#spanGlobalMessage").html(MsgInvalidPullValue);
            $('div#target').fadeToggle();
            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
            return false;
        }
        vBinID = $(obj).parent().parent().find('#item_BinID')[0].value == '' ? 0 : $(obj).parent().parent().find('#item_BinID')[0].value;
        vProjectGUID = $(obj).parent().parent().find("#ProjectSpendGUID").val() == "" ? "" : $(obj).parent().parent().find("#ProjectSpendGUID").val();
        ProjectSpendName = $(obj).parent().parent().find("#ProjectSpendGUID").val() == "" ? "" : $(obj).parent().parent().find("#ProjectSpendGUID option:selected").text();
        if (txtQty == 'undefined' && txtQty.length == 0) {
            //alert('Qty to Pull is Mandatory.');
            $("#spanGlobalMessage").html(MsgreqQtyToPull);
            $('div#target').fadeToggle();
            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
            return false;
        }
        if (vBinID == 0) {
            //alert('Inventory Location are Mandatory.');
            $("#spanGlobalMessage").html(MsgInventoryLocationMandatory);
            $('div#target').fadeToggle();
            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
            return false;
        }
    }
    else {
        if (txtQty == 'undefined' || txtQty == '') {
            //alert('Labour Item Required Hours to Pull.');
            $("#spanGlobalMessage").html(MsgLabourItemRequiredHours);
            $('div#target').fadeToggle();
            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
            return false;
        }
        if (parseFloat(txtQty) <= 0) {
            //alert('Labour Item Required Hours to Pull.');
            $("#spanGlobalMessage").html(MsgLabourItemRequiredHours);
            $('div#target').fadeToggle();
            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
            return false;
        }
        if (parseFloat(txtQty) == NaN) {
            $("#spanGlobalMessage").html(MsgLabourItemRequiredHours);
            $('div#target').fadeToggle();
            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
            return false;
        }

        vBinID = 0;
        vProjectID = '';
    }

    //if (txtQty != 'undefined' && txtQty > 0 && vBinID > 0) {
    var trimtxtVal = txtQty.toString().replace(/ /g, '');
    trimtxtVal = parseFloat(trimtxtVal, 10);
    if (trimtxtVal > 0) {

        //var vItemID = $(obj).parent().parent().find('#hdnItemID').val();
        var vItemGUID = $(obj).parent().parent().find('#hdnItemGUID').val();
        //var vProjectGUID = $(obj).parent().parent().find("#ProjectSpendGUID").val() == "" ? "" : $(obj).parent().parent().find("#ProjectSpendGUID").val();
        var hdnID = $(obj).parent().parent().find('#hdnID').val();
        var hdnWorkOrderID = $(obj).parent().parent().find('#hdnWorkOrderID').val();
        var vspnOn_HandQuantity = 0;
        var vPullCreditText = 'pull';

        $('#DivLoading').show();

        $.ajax({
            "url": _CreateWOItem.urls.UpdatePullDataUrl,
            data: { ID: 0, ItemGUID: vItemGUID, ProjectGUID: vProjectGUID, PullCreditQuantity: txtQty, BinID: vBinID, PullCredit: vPullCreditText, TempPullQTY: txtQty, UDF1: '', UDF2: '', UDF3: '', UDF4: '', UDF5: '', RequisitionDetailGUID: "", WorkOrderDetailGUID: hdnID, ProjectSpendName: ProjectSpendName, PullType: PullType },
            "async": false,
            cache: false,
            "dataType": "json",
            success: function (response) {
                $('#DivLoading').hide();
                if (response.Status == "ok") {
                    if (response.LocationMSG != "") {
                        if (response.PSLimitExceed) {
                            // write code to redirect to requisition with confirm box
                            $("#PSPlimit").text(response.LocationMSG + " " + msgProjectSpendLimitConfirmation);
                            $('#project-spend-limit-basic-modal-content').modal();
                        }
                        else {
                            //alert(response.LocationMSG);
                            $("#spanGlobalMessage").html(response.LocationMSG);
                            $('div#target').fadeToggle();
                            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                        }
                    }
                    else {
                        $('div#target').fadeToggle();
                        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                        $("#spanGlobalMessage").html(response.Message);
                        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                        ShowEditTabGUID("WOEdit?WorkOrderGUID=" + hdnWorkOrderID, "frmWOMaster");
                    }
                }
                else {
                    $("#spanGlobalMessage").html(response.Message);
                    $('div#target').fadeToggle();
                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                }
                //txxt.val('');
            },
            error: function (response) {
                $('#DivLoading').hide();
                $("#spanGlobalMessage").html(response.message);
                $('div#target').fadeToggle();
                $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
            }
        });
    }
    //}
    //        else {
    //            //alert('Qty to Pull and Inventory Location are Mandatory.');
    //            $("#spanGlobalMessage").text('Qty to Pull and Inventory Location are Mandatory.');
    //            $('div#target').fadeToggle();
    //            $("div#target").delay(2000).fadeOut(200);
    //        }

}
function CallThisFunctionFromModel(msg) {
    if (msg == 'success') {
        $('#DivLoading').show();
        $('#WOLineItems').empty();

        $('#WOLineItems').load(_CreateWOItem.urls.LoadWOItemsUrl, function () {
            $('#DivLoading').hide();
        });
    }
}

function ReDirectData() {
    if (mntsGUID == GuidEmpty) {
        ShowEditTabGUID("WOEdit?WorkOrderGUID=" + _CreateWOItem.WorkOrderGUID, "frmWOMaster");
    }
    else {
        EditMaintenance(mntsGUID, WOHEaderGUID);
    }

}


function closeModalPSLimit() {
    $.modal.impl.close();
}






/* global search function */
function fnFilterGlobalPS() {
    //set filter only if more than 2 characters are pressed
    //if (typeof $("#InnerItem_filter") != 'undefined' && ($("#InnerItem_filter").val().length > 2 || $("#InnerItem_filter").val().length == 0)) {
    if (typeof $("#InnerItem_filter") != 'undefined') {

        bIsFilter = true;
        var searchtext = $("#InnerItem_filter").val().replace(/'/g, "''");
        $('#' + _CreateWOItem.DTName).dataTable().fnFilter(searchtext, null, null, false);
        $('#' + _CreateWOItem.DTName + ' tr.odd td').removeHighlight();
        $('#' + _CreateWOItem.DTName + ' tr.even td').removeHighlight();

        if (searchtext.length > 0) {
            $('#' + _CreateWOItem.DTName + ' tr.odd td').highlight($("#InnerItem_filter").val());
            $('#' + _CreateWOItem.DTName + ' tr.even td').highlight($("#InnerItem_filter").val());
        }
    }
}



function callPrint1(DataTableName) {
    var oConfig = {
        "sInfo": "<h6>Print view</h6><p>" + MsgUseBrowserToPrint +"</p>",
        "sMessage": null,
        "bShowAll": false,
        "sToolTip": "View print view",
        "sButtonClass": "DTTT_button_print",
        "sButtonText": "Print"
    };
    if (typeof (TableTools) != undefined && typeof (TableTools) != 'undefined')
        TableTools.fnGetInstance(DataTableName).fnPrint(true, oConfig);
}


function FillDetailGridDiv() {
    $('#WOLineItems').empty();
    $('#WOLineItems').load(_CreateWOItem.urls.LoadWOItemsUrl, function () {
        $('#DivLoading').hide();
        $("#divDetailGridReorderPopup").dialog("close");
    });
}

function FillWODetailGridDiv() {
    $('#WOLineItems').empty();
    $('#WOLineItems').load(_CreateWOItem.urls.LoadWOItemsUrl, function () {
        $('#DivLoading').hide();
        $("#divDetailGridReorderPopup").dialog("close");
    });
}

function createWODTO(serverObj) {
    var self = this;

    return self;
}