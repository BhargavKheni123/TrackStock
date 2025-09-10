_narrowSearchForIM = (function ($) {

    var self = {};

    var divNarrowULForIMID = 'divNarrowULForIM'; // NarrowUL

    self.pageName = null;
    self.companyId = null;
    self.roomId = null;
    self.controlId = null;
    self.IMCallFromPageName = null;
    self.actionName = null;
    self.controllerName = null;

    self.init = function (pageName, companyId, roomId, controlId, IMCallFromPageName
        , actionName, controllerName
    ) {
        
        self.pageName = pageName;
        self.companyId = companyId;
        self.roomId = roomId;
        self.controlId = controlId;
        self.IMCallFromPageName = IMCallFromPageName
        self.actionName = actionName;
        self.controllerName = controllerName;
        self.initEvents();
    }

    self.initEvents = function () {

        //$.ready(function () {
            $(".handle").each(function () {
                $(this).find("img").attr("src", "../../Content/images/Drage.png");
            });
        NSForItemModel_ExecuteOnDocReady();
        if (_narrowSearchForIM.actionName == 'NewPull') {
            fnMakeNarrowSearchSortable(_narrowSearchForIM.actionName + "_NarrowSearch");
            
        }
        else {
            var tmpIMNSListName = gblActionName + "_ItemModelNarrowSearch";
            fnMakeNarrowSearchSortable(tmpIMNSListName);
            
        }
        //});

        //$('#NarroSearchGo').click(function () {
        $('#NarroSearchGo').off('click').on('click',function (e) {
            DoNarrowSearchIM();
        });

        //$('#DateCFromIM,#DateCToIM').change(function () {
        $('#DateCFromIM,#DateCToIM').off('change').on('change',function (e) {

            var DateCFromValidIM = true;//Date.isValid($('#DateCFromIM').val(),format);
            var DateCToValidIM = true;//Date.isValid($('#DateCToIM').val(),format);
            try {
                $.datepicker.parseDate(RoomDateJSFormat, $('#DateCFromIM').val());
                DateCFromValidIM = true;
            } catch (e) {
                DateCFromValidIM = false;
            }

            try {
                $.datepicker.parseDate(RoomDateJSFormat, $('#DateCToIM').val());
                DateCToValidIM = true;
            } catch (e) {
                DateCToValidIM = false;
            }
            if (DateCFromValidIM && DateCToValidIM) {
                // $("#ItemModel_filter").val('');
                if (!isFromNarrowSearchClear)
                {
                    DoNarrowSearchIM();
                }                
            }
            else {
                if (!DateCFromValidIM)
                    $('#DateCFromIM').val('');
                if (!DateCToValidIM)
                    $('#DateCToIM').val('');
            }
        });

        $('#DateUFromIM,#DateUToIM').change(function () {

            var DateUFromValidIM = true;//Date.isValid($('#DateUFromIM').val(),format);
            var DateUToValidIM = true;//Date.isValid($('#DateUToIM').val(),format);
            try {
                $.datepicker.parseDate(RoomDateJSFormat, $('#DateUFromIM').val());
                DateUFromValidIM = true;
            } catch (e) {
                DateUFromValidIM = false;
            }

            try {
                $.datepicker.parseDate(RoomDateJSFormat, $('#DateUToIM').val());
                DateUToValidIM = true;
            } catch (e) {
                DateUToValidIM = false;
            }
            if (DateUFromValidIM && DateUToValidIM) {
                //  $("#ItemModel_filter").val('');
                if (!isFromNarrowSearchClear) {
                    DoNarrowSearchIM();
                }                
            }
            else {
                if (!DateUFromValidIM)
                    $('#DateUFromIM').val('');
                if (!DateUToValidIM)
                    $('#DateUToIM').val('');
            }
        });



        //CLEAR NARROW SEARCH
        //$('#NarroSearchClearIM').click(function () {
        $('#NarroSearchClearIM').off('click').on('click', function (e) {
            //        $('#DateCFromIM').val('');
            //        $('#DateCToIM').val('');
            //        $('#DateUFromIM').val('');
            //        $('#DateUToIM').val('');
            //        $("#UserCreatedIM").multiselect("uncheckAll");
            //        $("#UserCreatedCollapseIM").html('');
            //        $("#UserUpdated").multiselect("uncheckAll");
            //        $("select[name='udflist']").each(function (index) {
            //            $(this).multiselect("uncheckAll");
            //        });
            //        // clear pull narrow search extra items
            //        if ($("#PullSupplierIM") != undefined) {
            //            $("#PullSupplierIM").multiselect("uncheckAll");
            //            $("#PullSupplierCollapseIM").html('');
            //            $("#ManufacturerIM").multiselect("uncheckAll");
            //            $("#ManufacturerCollapseIM").html('');
            //            $("#PullCategoryIM").multiselect("uncheckAll");
            //            $("#PullCategoryCollapseIM").html('');
            //        }

            //        if ($('#PullCost') != undefined) {
            //            $('#PullCost').val('0_-1');
            //        }

            //        if ($('#StockStatus') != undefined) {
            //            $('#StockStatus').val('0');
            //        }

            //        $("#ItemType").multiselect("uncheckAll");
            //        $("#ItemTypeCollapseIM").html('');

            isFromNarrowSearchClear = true;
            _NarrowSearchSave.objSearch.reset();

            if ($('#DateCFromIM').val() != '') $('#DateCFromIM').val('');
            if ($('#DateCToIM').val() != '') $('#DateCToIM').val('');
            if ($('#DateUFromIM').val() != '') $('#DateUFromIM').val('');
            if ($('#DateUToIM').val() != '') $('#DateUToIM').val('');

            if (typeof ($("#UserCreatedIM").multiselect("getChecked").length) != undefined && $("#UserCreatedIM").multiselect("getChecked").length > 0) {
                $("#UserCreatedIM").multiselect("uncheckAll");
                $("#UserCreatedCollapseIM").html('');
            }
            if (typeof ($("#UserCreatedOrd").multiselect("getChecked").length) != undefined && $("#UserCreatedOrd").multiselect("getChecked").length > 0) {
                $("#UserCreatedOrd").multiselect("uncheckAll");
                $("#UserCreatedCollapseOrd").html('');
            }
            if (typeof ($("#UserUpdatedIM").multiselect("getChecked").length) != undefined && $("#UserUpdatedIM").multiselect("getChecked").length > 0) {
                $("#UserUpdatedIM").multiselect("uncheckAll");
                $("#UserUpdatedCollapseIM").html('');
            }
            if (typeof ($("#UserUpdatedOrd").multiselect("getChecked").length) != undefined && $("#UserUpdatedOrd").multiselect("getChecked").length > 0) {
                $("#UserUpdatedOrd").multiselect("uncheckAll");
                $("#UserUpdatedCollapseOrd").html('');
            }

            $("select[name='udflist_Item']").each(function (index) {
                if (typeof ($(this).multiselect("getChecked").length) != undefined && $(this).multiselect("getChecked").length > 0) {
                    var UDFUniqueID = this.getAttribute('UID');
                    $(this).multiselect("uncheckAll");
                    $('#' + UDFUniqueID + 'Collapse_Item').html('');
                }
            });

            if (typeof ($("#PullSupplierIM").multiselect("getChecked").length) != undefined && $("#PullSupplierIM").multiselect("getChecked").length > 0) {
                $("#PullSupplierIM").multiselect("uncheckAll");
                $("#PullSupplierCollapseIM").html('');
            }
            if (typeof ($("#BinListIM").multiselect("getChecked").length) != undefined && $("#BinListIM").multiselect("getChecked").length > 0) {
                $("#BinListIM").multiselect("uncheckAll");
                $("#BinListCollapseIM").html('');
            }
            if (typeof ($("#PullSupplierOrd").multiselect("getChecked").length) != undefined && $("#PullSupplierOrd").multiselect("getChecked").length > 0) {
                $("#PullSupplierOrd").multiselect("uncheckAll");
                $("#PullSupplierCollapseOrd").html('');
            }
            if (typeof ($("#StageLocationHeaderIM").multiselect("getChecked").length) != undefined && $("#StageLocationHeaderIM").multiselect("getChecked").length > 0) {
                $("#StageLocationHeaderIM").multiselect("uncheckAll");
                $("#StageLocationHeaderCollapseIM").html('');
                IsStagingLocationOnly = false;
            }

            if (typeof ($("#StageLocationIM").multiselect("getChecked").length) != undefined && $("#StageLocationIM").multiselect("getChecked").length > 0) {
                $("#StageLocationIM").multiselect("uncheckAll");
                $("#StageLocationCollapseIM").html('');
                IsStagingLocationOnly = false;
            }

            if (typeof ($("#ManufacturerIM").multiselect("getChecked").length) != undefined && $("#ManufacturerIM").multiselect("getChecked").length > 0) {
                $("#ManufacturerIM").multiselect("uncheckAll");
                $("#ManufacturerCollapseIM").html('');
            }
            if (typeof ($("#ManufacturerOrd").multiselect("getChecked").length) != undefined && $("#ManufacturerOrd").multiselect("getChecked").length > 0) {
                $("#ManufacturerOrd").multiselect("uncheckAll");
                $("#ManufacturerCollapseOrd").html('');
            }
            if (typeof ($("#PullCategoryIM").multiselect("getChecked").length) != undefined && $("#PullCategoryIM").multiselect("getChecked").length > 0) {
                $("#PullCategoryIM").multiselect("uncheckAll");
                $("#PullCategoryCollapseIM").html('');
            }
            if (typeof ($("#PullCategoryOrd").multiselect("getChecked").length) != undefined && $("#PullCategoryOrd").multiselect("getChecked").length > 0) {
                $("#PullCategoryOrd").multiselect("uncheckAll");
                $("#PullCategoryCollapseOrd").html('');
            }
            if (typeof ($("#ItemLocationsIC").multiselect("getChecked").length) != undefined && $("#ItemLocationsIC").multiselect("getChecked").length > 0) {
                $("#ItemLocationsIC").multiselect("uncheckAll");
                $("#ItemLocationsICCollapseIM").html('');
            }

            if ($('#PullCost') != undefined) {
                $('#PullCost').val('0_-1');
            }

            if ($('#StockStatus') != undefined) {
                $('#StockStatus').val('0');
                SSNarroSearchValue = "";
            }

            //cost
            if ($('#PullCostIM') != undefined) {
                $('#PullCostIM').val('0');
                CostNarroSearchValue = "";
            }

            //avg usage
            if ($('select[name ="NarroSearchAvgUsage"]') != undefined) {
                $('select[name ="NarroSearchAvgUsage"]').val('0');
                AvgUsageNarroSearchValue = "";
            }

            //Turns
            if ($('select[name ="NarroSearchTurns"]') != undefined) {
                $('select[name ="NarroSearchTurns"]').val('0');
                TurnsNarroSearchValue = "";
            }

            if (typeof ($("#ItemTypeIM").multiselect("getChecked").length) != undefined && $("#ItemTypeIM").multiselect("getChecked").length > 0) {
                $("#ItemTypeIM").multiselect("uncheckAll");
                $("#ItemTypeCollapseIM").html('');
            }
            if (typeof ($("#ItemTypeOrd").multiselect("getChecked").length) != undefined && $("#ItemTypeOrd").multiselect("getChecked").length > 0) {
                $("#ItemTypeOrd").multiselect("uncheckAll");
                $("#ItemTypeCollapseOrd").html('');
            }
            ////////////////////////////////////////////
            isFromNarrowSearchClear = false;
            $('input[type="search"]').val('').trigger('keyup');

            if ($('#ItemModel_filter').val() != '') $('#ItemModel_filter').val('');
            //
            if ($('#Cart_ItemModel_filter').val() != '') $('#Cart_ItemModel_filter').val('');
            if ($('#searchInAllItems').val() != '') $('#searchInAllItems').val('');

            var searchText = '';
            if ($('input#ItemModel_filter').length > 0) {
                searchText = $('input#ItemModel_filter').val();
            }
            if ($("#ddlMoveType").length > 0) {
                $("#ddlMoveType").val(1);
                $("#ddlMoveType").trigger('change');
            }
            $("#btnAddAll").show();
            //NarrowSearchInGridIM(searchText);
            $('#ItemModeDataTable').dataTable().fnFilter(searchText, null, null, null)
        });



        //CREATE DATE PICKER

        $('#DateCFromIM').blur(function () {
        }).datepicker({
            changeMonth: true,
            changeYear: true, dateFormat: RoomDateJSFormat
        });
        $('#DateCToIM').blur(function () {
        }).datepicker({
            changeMonth: true,
            changeYear: true, dateFormat: RoomDateJSFormat
        });
        $('#DateUFromIM').blur(function () {
        }).datepicker({
            changeMonth: true,
            changeYear: true, dateFormat: RoomDateJSFormat
        });
        $('#DateUToIM').blur(function () {
        }).datepicker({
            changeMonth: true,
            changeYear: true, dateFormat: RoomDateJSFormat
        });


        //$('#ancDateCFromIM').click(function () {
        $('#ancDateCFromIM').off('click').on('click', function (e) {
            $('#DateCFromIM').focus();
        });
        //$('#ancDateCToIM').click(function () {
        $('#ancDateCToIM').off('click').on('click', function (e) {
            $('#DateCToIM').focus();
        });
        //$('#ancDateUFromIM').click(function () {
        $('#ancDateUFromIM').off('click').on('click', function (e) {
            $('#DateUFromIM').focus();
        });
        $('#ancDateUToIM').click(function () {
            $('#DateUToIM').focus();
        });


        //$('#DateCreatedClearIM').click(function () {
        $('#DateCreatedClearIM').off('click').on('click', function (e) {
            if ($('#DateCFromIM').val() != '' || $('#DateCToIM').val() != '') {
                $('#DateCFromIM').val('');
                $('#DateCToIM').val('');
                //NarrowSearchInGrid('');
                if (!isFromNarrowSearchClear) {
                    DoNarrowSearchIM();
                }                
            }
        });
        //$('#DateUpdatedClearIM').click(function () {
        $('#DateUpdatedClearIM').off('click').on('click', function (e) {
            if ($('#DateUFromIM').val() != '' || $('#DateUToIM').val() != '') {
                $('#DateUFromIM').val('');
                $('#DateUToIM').val('');
                //NarrowSearchInGrid('');
                if (!isFromNarrowSearchClear) {
                    DoNarrowSearchIM();
                }                
            }
        });

    }

    var fnMakeNarrowSearchSortable = function (listName) {
        //var tmpItemModelNSListName = 
        $("#" + divNarrowULForIMID).sortable({
            handle: "> .handle",
            stop: function (event, ui) {
                var linkOrderData = $("#" + divNarrowULForIMID).sortable("toArray", { attribute: 'attrsortOrder' });

                $.ajax({
                    "type": "POST",
                    "url": '/Master/SaveGridState',
                    "data": { Data: JSON.stringify(linkOrderData), ListName: listName },
                    "dataType": "json",
                    "cache": false,
                    "async": false,
                    "success": function (json1) {
                    }
                });
            }
        });


        //var orderArray = '3,2,4,5,6,7,8,9,10,1,12,11,103,101,102,104,';
        //var listArray = $('#nssortable li');
        //for (var i = 0; i < orderArray.length; i++) {
        //    $('#nssortable').append(listArray[orderArray[i]-1]);
        //}
        var headerVal = $("input[name='__RequestVerificationToken'][type='hidden']").val();
        $.ajax({
            "type": "POST",
            "url": '/Master/LoadGridState',
            headers: { "__RequestVerificationToken": headerVal },
            "data": { ListName: listName },
            "dataType": "json",
            "cache": false,
            "async": false,
            "success": function (json1) {

                if (json1.jsonData != null && json1.jsonData != '') {

                    var sorted = JSON.parse(json1.jsonData);
                    sorted = sorted.reverse();

                    sorted.forEach(function (id) {
                        $("#" + divNarrowULForIMID + " [attrsortOrder=" + parseInt(id) + "]").prependTo("#" + divNarrowULForIMID);
                    });
                }
            }
        });



    }

    return self;

})(jQuery); //_pullCommon end



function RefreshPullNarrowSearch() {
    SSNarroSearchValue = "";
    CostNarroSearchValue = "";
    AvgUsageNarroSearchValue = "";
    TurnsNarroSearchValue = "";
    ItemTypeNarroSearchValue = "";
    StageLocationHeaderNarroValues = "";
    StageLocationNarroValues = "";
    PullSupplierNarroValues = "";
    BinNarroValuesIM = "";
    ManufacturerNarroValues = "";
    PullCategoryNarroValues = "";
    UserCreatedNarroValues = "";
    UserUpdatedNarroValues = "";
    UserUDF1NarrowValues = "";
    UserUDF2NarrowValues = "";
    UserUDF3NarrowValues = "";
    UserUDF4NarrowValues = "";
    UserUDF5NarrowValues = "";
    UserUDF6NarrowValues = "";
    UserUDF7NarrowValues = "";
    UserUDF8NarrowValues = "";
    UserUDF9NarrowValues = "";
    UserUDF10NarrowValues = "";
    isFromNarrowSearchClear = true;
    _NarrowSearchSave.objSearch.reset();
    if (typeof ($("#ItemTypeIM").multiselect("getChecked").length) != undefined && $("#ItemTypeIM").multiselect("getChecked").length > 0) {
        $('#ItemTypeIM').multiselect('refresh')
        $("#ItemTypeIM").multiselect("widget").find(":checkbox").removeAttr("checked");
        $("#ItemTypeCollapseIM").html('');
        $("#ItemTypeCollapseIM").hide();
        $("#ItemTypeIM").multiselect({ selectedText: "" });
    }
    if (typeof ($("#PullSupplierIM").multiselect("getChecked").length) != undefined && $("#PullSupplierIM").multiselect("getChecked").length > 0) {
        $('#PullSupplierIM').multiselect('refresh')
        $("#PullSupplierIM").multiselect("widget").find(":checkbox").removeAttr("checked");
        $("#PullSupplierCollapseIM").html('');
        $("#PullSupplierCollapseIM").hide();
        $("#PullSupplierIM").multiselect({ selectedText: "" });
    }
    if (typeof ($("#BinListIM").multiselect("getChecked").length) != undefined && $("#BinListIM").multiselect("getChecked").length > 0) {
        $('#BinListIM').multiselect('refresh')
        $("#BinListIM").multiselect("widget").find(":checkbox").removeAttr("checked");
        $("#BinListCollapseIM").html('');
        $("#BinListCollapseIM").hide();
        $("#BinListIM").multiselect({ selectedText: "" });
    }
    if (typeof ($("#StageLocationHeaderIM").multiselect("getChecked").length) != undefined && $("#StageLocationHeaderIM").multiselect("getChecked").length > 0) {
        $('#StageLocationHeaderIM').multiselect('refresh')
        $("#StageLocationHeaderIM").multiselect("widget").find(":checkbox").removeAttr("checked");
        $("#StageLocationHeaderCollapseIM").html('');
        $("#StageLocationHeaderCollapseIM").hide();
        $("#StageLocationHeaderIM").multiselect({ selectedText: "" });
    }
    if (typeof ($("#StageLocationIM").multiselect("getChecked").length) != undefined && $("#StageLocationIM").multiselect("getChecked").length > 0) {
        $('#StageLocationIM').multiselect('refresh')
        $("#StageLocationIM").multiselect("widget").find(":checkbox").removeAttr("checked");
        $("#StageLocationCollapseIM").html('');
        $("#StageLocationCollapseIM").hide();
        $("#StageLocationIM").multiselect({ selectedText: "" });
    }
    if (typeof ($("#ManufacturerIM").multiselect("getChecked").length) != undefined && $("#ManufacturerIM").multiselect("getChecked").length > 0) {
        $('#ManufacturerIM').multiselect('refresh')
        $("#ManufacturerIM").multiselect("widget").find(":checkbox").removeAttr("checked");
        $("#ManufacturerCollapseIM").html('');
        $("#ManufacturerCollapseIM").hide();
        $("#ManufacturerIM").multiselect({ selectedText: "" });
    }
    if (typeof ($("#PullCategoryIM").multiselect("getChecked").length) != undefined && $("#PullCategoryIM").multiselect("getChecked").length > 0) {
        $('#PullCategoryIM').multiselect('refresh')
        $("#PullCategoryIM").multiselect("widget").find(":checkbox").removeAttr("checked");
        $("#PullCategoryCollapseIM").html('');
        $("#PullCategoryCollapseIM").hide();
        $("#PullCategoryIM").multiselect({ selectedText: "" });
    }
    if (typeof ($("#UserCreatedIM").multiselect("getChecked").length) != undefined && $("#UserCreatedIM").multiselect("getChecked").length > 0) {
        $('#UserCreatedIM').multiselect('refresh')
        $("#UserCreatedIM").multiselect("widget").find(":checkbox").removeAttr("checked");
        $("#UserCreatedCollapseIM").html('');
        $("#UserCreatedCollapseIM").hide();
        $("#UserCreatedIM").multiselect({ selectedText: "" });
    }

    if (typeof ($("#UserUpdatedIM").multiselect("getChecked").length) != undefined && $("#UserUpdatedIM").multiselect("getChecked").length > 0) {
        $('#UserUpdatedIM').multiselect('refresh')
        $("#UserUpdatedIM").multiselect("widget").find(":checkbox").removeAttr("checked");
        $("#UserUpdatedCollapseIM").html('');
        $("#UserUpdatedCollapseIM").hide();
        $("#UserUpdatedIM").multiselect({ selectedText: "" });
    }
    //UDFs
    $("select[name='udflist_Item']").each(function (index) {
        if (typeof ($(this).multiselect("getChecked").length) != undefined && $(this).multiselect("getChecked").length > 0) {
            var UDFUniqueID = this.getAttribute('UID');

            $('#' + UDFUniqueID).multiselect('refresh')
            $('#' + UDFUniqueID).multiselect("widget").find(":checkbox").removeAttr("checked");
            $('#' + UDFUniqueID + 'Collapse_Item').html('');
            $('#' + UDFUniqueID + 'Collapse_Item').hide();
            $('#' + UDFUniqueID).multiselect({ selectedText: "" });
        }
    });

    if ($('#DateCFromIM').val() != '') $('#DateCFromIM').val('');
    if ($('#DateCToIM').val() != '') $('#DateCToIM').val('');
    if ($('#DateUFromIM').val() != '') $('#DateUFromIM').val('');
    if ($('#DateUToIM').val() != '') $('#DateUToIM').val('');

    $("#PullCostIM").val("");
    //avg usage
    if ($('select[name ="NarroSearchAvgUsage"]') != undefined) {
        $('select[name ="NarroSearchAvgUsage"]').val('0');
        AvgUsageNarroSearchValue = "";
    }

    //Turns
    if ($('select[name ="NarroSearchTurns"]') != undefined) {
        $('select[name ="NarroSearchTurns"]').val('0');
        TurnsNarroSearchValue = "";
    }
    isFromNarrowSearchClear = false;
    DoNarrowSearchIM();
}

function CallNarrowfunctionsIM() {
    var _IsArchived = false;
    var _IsDeleted = false;

    if (typeof ($('#IsArchivedRecords')) != undefined)
        _IsArchived = $('#IsArchivedRecords').is(':checked');

    if (typeof ($('#IsDeletedRecords')) != undefined)
        _IsDeleted = $('#IsDeletedRecords').is(':checked');
    if (_narrowSearchForIM.actionName.toLowerCase() == "loaditemmastermodel"
        && _narrowSearchForIM.pageName.toLowerCase() == "itemmaster"
        && _narrowSearchForIM.controllerName.toLowerCase() == "order") {
        GetNarrowDDDataOrd(_narrowSearchForIM.pageName, _narrowSearchForIM.companyId, _narrowSearchForIM.roomId, _IsArchived, _IsDeleted, _narrowSearchForIM.controlId);
    }
    else {
        GetNarrowDDDataIM(_narrowSearchForIM.pageName, _narrowSearchForIM.companyId, _narrowSearchForIM.roomId, _IsArchived, _IsDeleted, _narrowSearchForIM.controlId);
    }
    
    GetNarroHTMLForUDFIM(_narrowSearchForIM.pageName, _narrowSearchForIM.companyId, _narrowSearchForIM.roomId, _IsArchived, _IsDeleted);

    if (_IsDeleted || _IsArchived) {
        $('#deleteRows').attr("style", "display:none");
        AllowDeletePopup = false;
    }
    else {
        $('#deleteRows').attr("style", "display:''");
        AllowDeletePopup = true;
    }

    //$(".tab5").hide();

}




function SSNarroSearch(SSDDLObject) {

    //oTable.fnFilter($(CostDDLObject).val(),null,null,null);
    if ($(SSDDLObject).val() != "0") {
        SSNarroSearchValue = $(SSDDLObject).val();
        _NarrowSearchSave.objSearch.StockStatus = SSNarroSearchValue;
        if (!isFromNarrowSearchClear) {
            DoNarrowSearchIM();
        }        
    }
    else {
        SSNarroSearchValue = '';
        _NarrowSearchSave.objSearch.StockStatus = SSNarroSearchValue;
        if (!isFromNarrowSearchClear) {
            DoNarrowSearchIM();
        }        
    }
}
function SSNarroSearchAvgUsage(SSDDLObject) {

    //oTable.fnFilter($(CostDDLObject).val(),null,null,null);
    if ($(SSDDLObject).val() != "0_-1") {
        AvgUsageNarroSearchValue = $(SSDDLObject).val();
        _NarrowSearchSave.objSearch.NarroSearchAvgUsage = AvgUsageNarroSearchValue;
        if (!isFromNarrowSearchClear) {
            DoNarrowSearchIM();
        }        
    }
    else {
        AvgUsageNarroSearchValue = '';
        _NarrowSearchSave.objSearch.NarroSearchAvgUsage = AvgUsageNarroSearchValue;
        if (!isFromNarrowSearchClear) {
            DoNarrowSearchIM();
        }        
    }
}
function SSNarroSearchTurns(SSDDLObject) {

    //oTable.fnFilter($(CostDDLObject).val(),null,null,null);
    if ($(SSDDLObject).val() != "0_-1") {
        TurnsNarroSearchValue = $(SSDDLObject).val();
        _NarrowSearchSave.objSearch.NarroSearchTurns = TurnsNarroSearchValue;
        if (!isFromNarrowSearchClear) {
            DoNarrowSearchIM();
        }        
    }
    else {
        TurnsNarroSearchValue = '';
        _NarrowSearchSave.objSearch.NarroSearchTurns = TurnsNarroSearchValue;
        if (!isFromNarrowSearchClear) {
            DoNarrowSearchIM();
        }        
    }
}

function CostNarroSearchIM(CostDDLObject) {
    //oTable.fnFilter($(CostDDLObject).val(),null,null,null);
    if ($(CostDDLObject).val() != "0_-1") {
        CostNarroSearchValue = $(CostDDLObject).val();
        _NarrowSearchSave.objSearch.PullCostIM = CostNarroSearchValue;
        if (!isFromNarrowSearchClear) {
            DoNarrowSearchIM();
        }        
    }
    else {
        CostNarroSearchValue = '';
        _NarrowSearchSave.objSearch.PullCostIM = CostNarroSearchValue;
        if (!isFromNarrowSearchClear) {
            DoNarrowSearchIM();
        }        
    }
}


function GetNarrowHTMLForUserType() {

    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _narrowSearchForIM.pageName, TextFieldName: 'UserType', IsArchived: false, IsDeleted: false },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });

            //Destroy widgets before reapplying the filter
            $("#UserType").empty();
            $("#UserType").multiselect('destroy');
            $("#UserType").multiselectfilter('destroy');

            $("#UserType").append(s);
            $("#UserType").multiselect
                (
                    {
                        noneSelectedText: 'User type', selectedList: 5,
                        selectedText: function (numChecked, numTotal, checkedItems) {
                            return 'User type ' + numChecked + ' ' + selected;
                        }
                    },
                    {
                        checkAll: function (ui) {
                            $("#UserTypeCollapse").html('');
                            for (var i = 0; i <= ui.target.length - 1; i++) {
                                if ($("#UserTypeCollapse").text().indexOf(ui.target[i].text) == -1) {
                                    $("#UserTypeCollapse").append("<span>" + ui.target[i].text + "</span>");
                                }
                            }
                            $("#UserTypeCollapse").show();
                        }
                    }
            )
                .unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                    if (ui.checked) {
                        if ($("#UserTypeCollapse").text().indexOf(ui.text) == -1) {
                            $("#UserTypeCollapse").append("<span>" + ui.text + "</span>");
                        }
                    }
                    else {
                        if (ui.checked == undefined) {
                            $("#UserTypeCollapse").html('');
                        }
                        else if (!ui.checked) {
                            var text = $("#UserTypeCollapse").html();
                            text = text.replace("<span>" + ui.text + "</span>", '');
                            $("#UserTypeCollapse").html(text);
                        }
                        else
                            $("#UserTypeCollapse").html('');
                    }
                    UserTypeNarroValues = $.map($(this).multiselect("getChecked"), function (input) {

                        return input.value;
                    })

                    if ($("#UserTypeCollapse").text().trim() != '')
                        $("#UserTypeCollapse").show();
                    else
                        $("#UserTypeCollapse").hide();


                    if ($("#UserTypeCollapse").find('span').length <= 2) {
                        $("#UserTypeCollapse").scrollTop(0).height(50);
                    }
                    else {
                        $("#UserTypeCollapse").scrollTop(0).height(100);
                    }
                    clearGlobalIfNotInFocus();
                    DoNarrowSearch();
                }).multiselectfilter();
        },
        error: function (response) {
            // through errror message
        }
    });
}


function GetItemLocationsForCount(ShowStagLocs, ShowLabourItems) {

    if (ShowLabourItems == null || ShowLabourItems == undefined)
        ShowLabourItems = true;

    var ItemCallFrom = '';
    if (_narrowSearchForIM.IMCallFromPageName == 'NewPULL') {
        ItemCallFrom = 'newpull';
    }
    else {
        ItemCallFrom = _narrowSearchForIM.controlId;
    }

    $.ajax({
        'url': '/Master/GetLocationsinItemMaster',
        data: { ShowStagLocs: ShowStagLocs, ShowLabourItems: ShowLabourItems, "ItemModelCallFrom": ItemCallFrom, ParentID: modelID },
        success: function (response) {

            var s = '';
            $.each(response, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });


            //Destroy widgets before reapplying the filter
            $("#ItemLocationsIC").empty();
            $("#ItemLocationsIC").multiselect('destroy');
            $("#ItemLocationsIC").multiselectfilter('destroy');

            $("#ItemLocationsIC").append(s);
            $("#ItemLocationsIC").multiselect
                (
                    {
                        noneSelectedText: 'Location', selectedList: 5,
                        selectedText: function (numChecked, numTotal, checkedItems) {
                            return 'Location ' + numChecked + ' ' + selected;
                        }
                    },
                    {
                        checkAll: function (ui) {
                            $("#ItemLocationsICCollapseIM").html('');
                            for (var i = 0; i <= ui.target.length - 1; i++) {
                                if ($("#ItemLocationsICCollapseIM").text().indexOf(ui.target[i].text) == -1) {
                                    $("#ItemLocationsICCollapseIM").append("<span>" + ui.target[i].text + "</span>");
                                }
                            }
                            $("#ItemLocationsICCollapseIM").show();
                        }
                    }
            )
                .unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                    if (ui.checked) {
                        if ($("#ItemLocationsICCollapseIM").text().indexOf(ui.text) == -1) {
                            $("#ItemLocationsICCollapseIM").append("<span>" + ui.text + "</span>");
                        }
                    }
                    else {
                        if (ui.checked == undefined) {
                            $("#ItemLocationsICCollapseIM").html('');
                        }
                        else if (!ui.checked) {
                            var text = $("#ItemLocationsICCollapseIM").html();
                            text = text.replace("<span>" + ui.text + "</span>", '');
                            $("#ItemLocationsICCollapseIM").html(text);
                        }
                        else
                            $("#ItemLocationsICCollapseIM").html('');
                    }
                    ICLNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                        return input.value;
                    })

                    if ($("#ItemLocationsICCollapseIM").text().trim() != '')
                        $("#ItemLocationsICCollapseIM").show();
                    else
                        $("#ItemLocationsICCollapseIM").hide();


                    if ($("#ItemLocationsICCollapseIM").find('span').length <= 2) {
                        $("#ItemLocationsICCollapseIM").scrollTop(0).height(50);
                    }
                    else {
                        $("#PullSupplierCollapseIM").scrollTop(0).height(100);
                    }
                    //clearGlobalIMIfNotInFocus();
                    if (!isFromNarrowSearchClear) {
                        DoNarrowSearchIM();
                    }                    
                }).multiselectfilter();
        },
        error: function (response) {

            // through errror message
        }
    });
}



/* Narrow Search For Item Type Filter END */


function GetPullNarrowSearchDataIM(_TableName, _IsArchived, _IsDeleted) {
    var ItemCallFrom = '';
    if (_narrowSearchForIM.IMCallFromPageName == 'NewPULL') {
        ItemCallFrom = 'newpull';
        if ($('#NewPullAction') != undefined && $('#NewPullAction').val() != "") {
            if ($('#NewPullAction').val() == "Credit") {
                ItemCallFrom = $('#NewPullAction').val();
            }
            else if ($('#NewPullAction').val() == "CreditMS") {
                ItemCallFrom = $('#NewPullAction').val();
            }
            else if ($('#NewPullAction').val() == "Pull") {
                ItemCallFrom = "newpull";
            }
        }
    }
    else {
        ItemCallFrom = _narrowSearchForIM.controlId;
    }

    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _TableName, TextFieldName: 'StageLocationHeader', IsArchived: _IsArchived, IsDeleted: _IsDeleted, "ItemModelCallFrom": ItemCallFrom, ParentID: modelID },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });


            //Destroy widgets before reapplying the filter
            $("#StageLocationHeaderIM").empty();
            $("#StageLocationHeaderIM").multiselect('destroy');
            $("#StageLocationHeaderIM").multiselectfilter('destroy');

            $("#StageLocationHeaderIM").append(s);
            $("#StageLocationHeaderIM").multiselect
                (
                    {
                        checkAllText: Check,
                        uncheckAllText: UnCheck,
                        noneSelectedText: StageHeaderLocation, selectedList: 5,
                        selectedText: function (numChecked, numTotal, checkedItems) {
                            return StageHeaderLocation + ' ' + numChecked + ' ' + selected;
                        }
                    },
                    {
                        checkAll: function (ui) {
                            $("#StageLocationHeaderCollapseIM").html('');
                            for (var i = 0; i <= ui.target.length - 1; i++) {
                                if ($("#StageLocationHeaderCollapseIM").text().indexOf(ui.target[i].text) == -1) {
                                    $("#StageLocationHeaderCollapseIM").append("<span>" + ui.target[i].text + "</span>");
                                }
                            }
                            $("#StageLocationHeaderCollapseIM").show();
                        }
                    }
            )
                .unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                    if (ui.checked) {
                        if ($("#StageLocationHeaderCollapseIM").text().indexOf(ui.text) == -1) {
                            $("#StageLocationHeaderCollapseIM").append("<span>" + ui.text + "</span>");
                        }
                    }
                    else {
                        if (ui.checked == undefined) {
                            $("#StageLocationHeaderCollapseIM").html('');
                        }
                        else if (!ui.checked) {
                            var text = $("#StageLocationHeaderCollapseIM").html();
                            text = text.replace("<span>" + ui.text + "</span>", '');
                            $("#StageLocationHeaderCollapseIM").html(text);
                        }
                        else
                            $("#StageLocationHeaderCollapseIM").html('');

                        IsStagingLocationOnly = false;
                    }
                    StageLocationHeaderNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                        return input.value;
                    })
                    _NarrowSearchSave.objSearch.StageLocationHeaderIM = StageLocationHeaderNarroValues;

                    if ($("#StageLocationHeaderCollapseIM").text().trim() != '')
                        $("#StageLocationHeaderCollapseIM").show();
                    else
                        $("#StageLocationHeaderCollapseIM").hide();


                    if ($("#StageLocationHeaderCollapseIM").find('span').length <= 2) {
                        $("#StageLocationHeaderCollapseIM").scrollTop(0).height(50);
                    }
                    else {

                        $("#StageLocationHeaderCollapseIM").scrollTop(0).height(100);
                    }
                    //clearGlobalIMIfNotInFocus();
                    if (!isFromNarrowSearchClear) {
                        DoNarrowSearchIM();
                    }
                    
                }).multiselectfilter({ label: Filter, placeholder: Enterkeywords });

            _NarrowSearchSave.setControlValue('StageLocationHeaderIM');
        },
        error: function (response) {
            // through errror message
        }
    });

    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _TableName, TextFieldName: 'StageLocation', IsArchived: _IsArchived, IsDeleted: _IsDeleted, "ItemModelCallFrom": ItemCallFrom, ParentID: modelID },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });


            //Destroy widgets before reapplying the filter
            $("#StageLocationIM").empty();
            $("#StageLocationIM").multiselect('destroy');
            $("#StageLocationIM").multiselectfilter('destroy');

            $("#StageLocationIM").append(s);
            $("#StageLocationIM").multiselect
                (
                    {
                        checkAllText: Check,
                        uncheckAllText: UnCheck,
                        noneSelectedText: StageLocation, selectedList: 5,
                        selectedText: function (numChecked, numTotal, checkedItems) {
                            return StageLocation + ' ' + numChecked + ' ' + selected;
                        }
                    },
                    {
                        checkAll: function (ui) {
                            $("#StageLocationCollapseIM").html('');
                            for (var i = 0; i <= ui.target.length - 1; i++) {
                                if ($("#StageLocationCollapseIM").text().indexOf(ui.target[i].text) == -1) {
                                    $("#StageLocationCollapseIM").append("<span>" + ui.target[i].text + "</span>");
                                }
                            }
                            $("#StageLocationCollapseIM").show();
                        }
                    }
            )
                .unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                    if (ui.checked) {
                        if ($("#StageLocationCollapseIM").text().indexOf(ui.text) == -1) {
                            $("#StageLocationCollapseIM").append("<span>" + ui.text + "</span>");
                        }
                    }
                    else {
                        if (ui.checked == undefined) {
                            $("#StageLocationCollapseIM").html('');
                        }
                        else if (!ui.checked) {
                            var text = $("#StageLocationCollapseIM").html();
                            text = text.replace("<span>" + ui.text + "</span>", '');
                            $("#StageLocationCollapseIM").html(text);
                        }
                        else
                            $("#StageLocationCollapseIM").html('');

                        IsStagingLocationOnly = false;
                    }
                    StageLocationNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                        return input.value;
                    })
                    _NarrowSearchSave.objSearch.StageLocationIM = StageLocationNarroValues;
                    if ($("#StageLocationCollapseIM").text().trim() != '')
                        $("#StageLocationCollapseIM").show();
                    else
                        $("#StageLocationCollapseIM").hide();


                    if ($("#StageLocationCollapseIM").find('span').length <= 2) {
                        $("#StageLocationCollapseIM").scrollTop(0).height(50);
                    }
                    else {
                        $("#StageLocationCollapseIM").scrollTop(0).height(100);
                    }
                    //clearGlobalIMIfNotInFocus();
                    if (!isFromNarrowSearchClear) {
                        DoNarrowSearchIM();
                    }                    
                }).multiselectfilter({ label: Filter, placeholder: Enterkeywords });

            _NarrowSearchSave.setControlValue("StageLocationIM");
        },
        error: function (response) {
            // through errror message
        }
    });


}




function GetNarrowDDDataIM_Common(_TableName, _IsArchived, _IsDeleted) {

    var ItemCallFrom = '';
    if (_narrowSearchForIM.IMCallFromPageName == 'NewPULL') {
        ItemCallFrom = 'newpull';
        if ($('#NewPullAction') != undefined && $('#NewPullAction').val() != "") {
            if ($('#NewPullAction').val() == "Credit") {
                ItemCallFrom = $('#NewPullAction').val();
            }
            else if ($('#NewPullAction').val() == "CreditMS") {
                ItemCallFrom = $('#NewPullAction').val();
            }
            else if ($('#NewPullAction').val() == "Pull") {
                ItemCallFrom = "newpull";
            }
        }
    }
    else if (_narrowSearchForIM.IMCallFromPageName.toLowerCase() == 'NEWCART'.toLowerCase()) {
        ItemCallFrom = 'newcart';
    }
    else if (_narrowSearchForIM.IMCallFromPageName.toLowerCase() == 'receive'.toLowerCase()) {
        ItemCallFrom = 'receive';
    }
    else if (_narrowSearchForIM.IMCallFromPageName.toLowerCase() == 'Materialstaging'.toLowerCase()) {
        ItemCallFrom = 'materialstaging';
    }
    else if (_narrowSearchForIM.IMCallFromPageName.toLowerCase() == 'ps'.toLowerCase()) {
        ItemCallFrom = 'ps';
    }
    else {
        ItemCallFrom = _narrowSearchForIM.controlId;
    }


    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _TableName, TextFieldName: 'SupplierName', IsArchived: _IsArchived, IsDeleted: _IsDeleted, "ItemModelCallFrom": ItemCallFrom, ParentID: modelID },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });


            //Destroy widgets before reapplying the filter
            $("#PullSupplierIM").empty();
            $("#PullSupplierIM").multiselect('destroy');
            $("#PullSupplierIM").multiselectfilter('destroy');

            $("#PullSupplierIM").append(s);

            if (typeof (ItemCallFrom) != "undefined" && ItemCallFrom != null && ItemCallFrom.toLowerCase() == "movemtr") {
                $("#PullSupplierIM").multiselect
                    (
                        {
                            checkAllText: Check,
                            uncheckAllText: UnCheck,
                            noneSelectedText: Supplier, selectedList: 5,
                            selectedText: function (numChecked, numTotal, checkedItems) {
                                return Supplier + ' ' + numChecked + ' ' + selected;
                            }
                        },
                        {
                            checkAll: function (ui) {
                                $("#PullSupplierCollapseIM").html('');
                                for (var i = 0; i <= ui.target.length - 1; i++) {
                                    if ($("#PullSupplierCollapseIM").text().indexOf(ui.target[i].text) == -1) {
                                        $("#PullSupplierCollapseIM").append("<span>" + ui.target[i].text + "</span>");
                                    }
                                }
                                $("#PullSupplierCollapseIM").show();
                            }
                        }
                    )
                    .unbind("multiselectclick multiselectcheckall multiselectuncheckall multiselectloadmore multiselectloadall")
                    .bind("multiselectclick multiselectcheckall multiselectuncheckall multiselectloadmore multiselectloadall", function (event, ui) {
                        
                        if (event.type == 'multiselectloadmore' || event.type == 'multiselectloadall') {

                            var currentDataCount = $('select#PullSupplierIM option').length;
                            var _selectedPMS = $.map($(this).multiselect("getChecked"), function (input) {
                                return input.value;
                            });

                            var _loaddataCount = parseInt(currentDataCount) + parseInt(loadNarrowSearchDataCount);
                            if (event.type == 'multiselectloadall') {
                                _loaddataCount = -1;
                                $(".allpullsupplierim").hide();
                                $(".morepullsupplierim").hide();
                            }
                            $.ajax({
                                'url': '/Master/GetNarrowDDData',
                                data: { TableName: _TableName, TextFieldName: 'SupplierName', IsArchived: _IsArchived, IsDeleted: _IsDeleted, "ItemModelCallFrom": ItemCallFrom, ParentID: modelID, LoadDataCount: _loaddataCount },
                                success: function (response) {
                                    var s = '';
                                    $.each(response.DDData, function (ValData, ValCount) {

                                        var ArrData = ValData.toString().split('[###]');
                                        if ($.inArray(ArrData[1], _selectedPMS) > -1) {
                                            s += '<option selected="selected" value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
                                        }
                                        else {
                                            s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
                                        }
                                    });

                                    //Destroy widgets before reapplying the filter
                                    $("#PullSupplierIM").empty();
                                    $("#PullSupplierIM").append(s);
                                    $("#PullSupplierIM").multiselect("refresh");
                                    $("#PullSupplierIM").multiselectfilter("refresh");

                                    var currentNewDataCount = $('select#PullSupplierIM option').length;
                                    if (parseInt(currentDataCount) >= parseInt(currentNewDataCount)) {
                                        $(".allpullsupplierim").hide();
                                        $(".morepullsupplierim").hide();
                                    }

                                },
                                error: function (response) {
                                    // through errror message
                                }
                            });

                        }
                        else {
                            if (ui.checked) {
                                if ($("#PullSupplierCollapseIM").text().indexOf(ui.text) == -1) {
                                    $("#PullSupplierCollapseIM").append("<span>" + ui.text + "</span>");
                                }
                            }
                            else {
                                if (ui.checked == undefined) {
                                    $("#PullSupplierCollapseIM").html('');
                                }
                                else if (!ui.checked) {
                                    var text = $("#PullSupplierCollapseIM").html();
                                    text = text.replace("<span>" + ui.text + "</span>", '');
                                    $("#PullSupplierCollapseIM").html(text);
                                }
                                else
                                    $("#PullSupplierCollapseIM").html('');
                            }

                            PullSupplierNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                                return input.value;
                            })
                            _NarrowSearchSave.objSearch.PullSupplierIM = PullSupplierNarroValues;
                            if ($("#PullSupplierCollapseIM").text().trim() != '')
                                $("#PullSupplierCollapseIM").show();
                            else
                                $("#PullSupplierCollapseIM").hide();


                            if ($("#PullSupplierCollapseIM").find('span').length <= 2) {
                                $("#PullSupplierCollapseIM").scrollTop(0).height(50);
                            }
                            else {
                                $("#PullSupplierCollapseIM").scrollTop(0).height(100);
                            }
                            //  clearGlobalIMIfNotInFocus();
                            if (!isFromNarrowSearchClear) {
                                DoNarrowSearchIM();
                            }
                        }

                    }).multiselectfilter({ label: Filter, placeholder: Enterkeywords });

                _NarrowSearchSave.setControlValue("PullSupplierIM");
                var currentDataCount = $('select#PullSupplierIM option').length;
                if (parseInt(currentDataCount) < parseInt(loadNarrowSearchDataCount)) {
                    $(".allpullsupplierim").hide();
                    $(".morepullsupplierim").hide();
                }
            }
            else
            {
                $("#PullSupplierIM").multiselect
                    (
                        {
                            checkAllText: Check,
                            uncheckAllText: UnCheck,
                            noneSelectedText: Supplier, selectedList: 5,
                            selectedText: function (numChecked, numTotal, checkedItems) {
                                return Supplier + ' ' + numChecked + ' ' + selected;
                            }
                        },
                        {
                            checkAll: function (ui) {
                                $("#PullSupplierCollapseIM").html('');
                                for (var i = 0; i <= ui.target.length - 1; i++) {
                                    if ($("#PullSupplierCollapseIM").text().indexOf(ui.target[i].text) == -1) {
                                        $("#PullSupplierCollapseIM").append("<span>" + ui.target[i].text + "</span>");
                                    }
                                }
                                $("#PullSupplierCollapseIM").show();
                            }
                        }
                    )
                    .unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                    .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                        if (ui.checked) {
                            if ($("#PullSupplierCollapseIM").text().indexOf(ui.text) == -1) {
                                $("#PullSupplierCollapseIM").append("<span>" + ui.text + "</span>");
                            }
                        }
                        else {
                            if (ui.checked == undefined) {
                                $("#PullSupplierCollapseIM").html('');
                            }
                            else if (!ui.checked) {
                                var text = $("#PullSupplierCollapseIM").html();
                                text = text.replace("<span>" + ui.text + "</span>", '');
                                $("#PullSupplierCollapseIM").html(text);
                            }
                            else
                                $("#PullSupplierCollapseIM").html('');
                        }
                        PullSupplierNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                            return input.value;
                        })
                        _NarrowSearchSave.objSearch.PullSupplierIM = PullSupplierNarroValues;
                        if ($("#PullSupplierCollapseIM").text().trim() != '')
                            $("#PullSupplierCollapseIM").show();
                        else
                            $("#PullSupplierCollapseIM").hide();


                        if ($("#PullSupplierCollapseIM").find('span').length <= 2) {
                            $("#PullSupplierCollapseIM").scrollTop(0).height(50);
                        }
                        else {
                            $("#PullSupplierCollapseIM").scrollTop(0).height(100);
                        }
                        //  clearGlobalIMIfNotInFocus();
                        if (!isFromNarrowSearchClear) {
                            DoNarrowSearchIM();
                        }
                    }).multiselectfilter({ label: Filter, placeholder: Enterkeywords });

                _NarrowSearchSave.setControlValue("PullSupplierIM");
            }
        },
        error: function (response) {
            // through errror message
        }
    });
    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _TableName, TextFieldName: 'BinNumber', IsArchived: _IsArchived, IsDeleted: _IsDeleted, "ItemModelCallFrom": ItemCallFrom, ParentID: modelID },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });


            //Destroy widgets before reapplying the filter
            $("#BinListIM").empty();
            $("#BinListIM").multiselect('destroy');
            $("#BinListIM").multiselectfilter('destroy');

            $("#BinListIM").append(s);

            $("#BinListIM").multiselect
                (
                    {
                        checkAllText: Check,
                        uncheckAllText: UnCheck,
                        noneSelectedText: BinNumber, selectedList: 5,
                        selectedText: function (numChecked, numTotal, checkedItems) {
                            return BinNumber + ' ' + numChecked + ' ' + selected;
                        }
                    },
                    {
                        checkAll: function (ui) {
                            $("#BinListCollapseIM").html('');
                            for (var i = 0; i <= ui.target.length - 1; i++) {
                                if ($("#BinListCollapseIM").text().indexOf(ui.target[i].text) == -1) {
                                    $("#BinListCollapseIM").append("<span>" + ui.target[i].text + "</span>");
                                }
                            }
                            $("#BinListCollapseIM").show();
                        }
                    }
                )
                .unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                    if (ui.checked) {
                        if ($("#BinListCollapseIM").text().indexOf(ui.text) == -1) {
                            $("#BinListCollapseIM").append("<span>" + ui.text + "</span>");
                        }
                    }
                    else {
                        if (ui.checked == undefined) {
                            $("#BinListCollapseIM").html('');
                        }
                        else if (!ui.checked) {
                            var text = $("#BinListCollapseIM").html();
                            text = text.replace("<span>" + ui.text + "</span>", '');
                            $("#BinListCollapseIM").html(text);
                        }
                        else
                            $("#BinListCollapseIM").html('');
                    }
                    BinNarroValuesIM = $.map($(this).multiselect("getChecked"), function (input) {
                        return input.value;
                    })
                    _NarrowSearchSave.objSearch.BinListIM = BinNarroValuesIM;
                    if ($("#BinListCollapseIM").text().trim() != '')
                        $("#BinListCollapseIM").show();
                    else
                        $("#BinListCollapseIM").hide();


                    if ($("#BinListCollapseIM").find('span').length <= 2) {
                        $("#BinListCollapseIM").scrollTop(0).height(50);
                    }
                    else {
                        $("#BinListCollapseIM").scrollTop(0).height(100);
                    }
                    // clearGlobalIMIfNotInFocus();
                    if (!isFromNarrowSearchClear) {
                        DoNarrowSearchIM();
                    }

                }).multiselectfilter({ label: Filter, placeholder: Enterkeywords });

            _NarrowSearchSave.setControlValue("BinListIM");



        },
        error: function (response) {
            // through errror message
        }
    });
    //alert('6');

    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _TableName, TextFieldName: 'Manufacturer', IsArchived: _IsArchived, IsDeleted: _IsDeleted, "ItemModelCallFrom": ItemCallFrom, ParentID: modelID },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });


            //Destroy widgets before reapplying the filter
            $("#ManufacturerIM").empty();
            $("#ManufacturerIM").multiselect('destroy');
            $("#ManufacturerIM").multiselectfilter('destroy');

            $("#ManufacturerIM").append(s);

            if (typeof (ItemCallFrom) != "undefined" && ItemCallFrom != null && ItemCallFrom.toLowerCase() == "movemtr") {
                $("#ManufacturerIM").multiselect
                    (
                        {
                            checkAllText: Check,
                            uncheckAllText: UnCheck,
                            noneSelectedText: Manufacturer, selectedList: 5,
                            selectedText: function (numChecked, numTotal, checkedItems) {
                                return Manufacturer + ' ' + numChecked + ' ' + selected;
                            }
                        },
                        {
                            checkAll: function (ui) {
                                $("#ManufacturerCollapseIM").html('');
                                for (var i = 0; i <= ui.target.length - 1; i++) {
                                    if ($("#ManufacturerCollapseIM").text().indexOf(ui.target[i].text) == -1) {
                                        $("#ManufacturerCollapseIM").append("<span>" + ui.target[i].text + "</span>");
                                    }
                                }
                                $("#ManufacturerCollapseIM").show();
                            }
                        }
                    )
                    .unbind("multiselectclick multiselectcheckall multiselectuncheckall multiselectloadmore multiselectloadall")
                    .bind("multiselectclick multiselectcheckall multiselectuncheckall multiselectloadmore multiselectloadall", function (event, ui) {
                        if (event.type == 'multiselectloadmore' || event.type == 'multiselectloadall') {

                            var currentDataCount = $('select#ManufacturerIM option').length;
                            var _selectedPMS = $.map($(this).multiselect("getChecked"), function (input) {
                                return input.value;
                            });

                            var _loaddataCount = parseInt(currentDataCount) + parseInt(loadNarrowSearchDataCount);
                            if (event.type == 'multiselectloadall') {
                                _loaddataCount = -1;
                                $(".allmanufacturerim").hide();
                                $(".moremanufacturerim").hide();
                            }
                            $.ajax({
                                'url': '/Master/GetNarrowDDData',
                                data: { TableName: _TableName, TextFieldName: 'Manufacturer', IsArchived: _IsArchived, IsDeleted: _IsDeleted, "ItemModelCallFrom": ItemCallFrom, ParentID: modelID, LoadDataCount: _loaddataCount },
                                success: function (response) {
                                    var s = '';
                                    $.each(response.DDData, function (ValData, ValCount) {

                                        var ArrData = ValData.toString().split('[###]');
                                        if ($.inArray(ArrData[1], _selectedPMS) > -1) {
                                            s += '<option selected="selected" value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
                                        }
                                        else {
                                            s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
                                        }
                                    });

                                    //Destroy widgets before reapplying the filter
                                    $("#ManufacturerIM").empty();
                                    $("#ManufacturerIM").append(s);
                                    $("#ManufacturerIM").multiselect("refresh");
                                    $("#ManufacturerIM").multiselectfilter("refresh");

                                    var currentNewDataCount = $('select#ManufacturerIM option').length;
                                    if (parseInt(currentDataCount) >= parseInt(currentNewDataCount)) {
                                        $(".allmanufacturerim").hide();
                                        $(".moremanufacturerim").hide();
                                    }

                                },
                                error: function (response) {
                                    // through errror message
                                }
                            });

                        }
                        else {
                            if (ui.checked) {
                                if ($("#ManufacturerCollapseIM").text().indexOf(ui.text) == -1) {
                                    $("#ManufacturerCollapseIM").append("<span>" + ui.text + "</span>");
                                }
                            }
                            else {
                                if (ui.checked == undefined) {
                                    $("#ManufacturerCollapseIM").html('');
                                }
                                else if (!ui.checked) {
                                    var text = $("#ManufacturerCollapseIM").html();
                                    text = text.replace("<span>" + ui.text + "</span>", '');
                                    $("#ManufacturerCollapseIM").html(text);
                                }
                                else
                                    $("#ManufacturerCollapseIM").html('');
                            }

                            ManufacturerNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                                return input.value;
                            })
                            _NarrowSearchSave.objSearch.ManufacturerIM = ManufacturerNarroValues;
                            if ($("#ManufacturerCollapseIM").text().trim() != '')
                                $("#ManufacturerCollapseIM").show();
                            else
                                $("#ManufacturerCollapseIM").hide();


                            if ($("#ManufacturerCollapseIM").find('span').length <= 2) {
                                $("#ManufacturerCollapseIM").scrollTop(0).height(50);
                            }
                            else {
                                $("#ManufacturerCollapseIM").scrollTop(0).height(100);
                            }
                            // clearGlobalIMIfNotInFocus();
                            //if (!isFromNarrowSearchClear && event.type != 'multiselectloadmore' && event.type != 'multiselectloadall') {
                            if (!isFromNarrowSearchClear) {
                                DoNarrowSearchIM();
                            }
                        }                        

                    }).multiselectfilter({ label: Filter, placeholder: Enterkeywords });

                _NarrowSearchSave.setControlValue("ManufacturerIM");
                var currentDataCount = $('select#ManufacturerIM option').length;
                if (parseInt(currentDataCount) < parseInt(loadNarrowSearchDataCount)) {
                    $(".allmanufacturerim").hide();
                    $(".moremanufacturerim").hide();
                }
            }
            else
            {
                $("#ManufacturerIM").multiselect
                    (
                        {
                            checkAllText: Check,
                            uncheckAllText: UnCheck,
                            noneSelectedText: Manufacturer, selectedList: 5,
                            selectedText: function (numChecked, numTotal, checkedItems) {
                                return Manufacturer + ' ' + numChecked + ' ' + selected;
                            }
                        },
                        {
                            checkAll: function (ui) {
                                $("#ManufacturerCollapseIM").html('');
                                for (var i = 0; i <= ui.target.length - 1; i++) {
                                    if ($("#ManufacturerCollapseIM").text().indexOf(ui.target[i].text) == -1) {
                                        $("#ManufacturerCollapseIM").append("<span>" + ui.target[i].text + "</span>");
                                    }
                                }
                                $("#ManufacturerCollapseIM").show();
                            }
                        }
                    )
                    .unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                    .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                        if (ui.checked) {
                            if ($("#ManufacturerCollapseIM").text().indexOf(ui.text) == -1) {
                                $("#ManufacturerCollapseIM").append("<span>" + ui.text + "</span>");
                            }
                        }
                        else {
                            if (ui.checked == undefined) {
                                $("#ManufacturerCollapseIM").html('');
                            }
                            else if (!ui.checked) {
                                var text = $("#ManufacturerCollapseIM").html();
                                text = text.replace("<span>" + ui.text + "</span>", '');
                                $("#ManufacturerCollapseIM").html(text);
                            }
                            else
                                $("#ManufacturerCollapseIM").html('');
                        }
                        ManufacturerNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                            return input.value;
                        })
                        _NarrowSearchSave.objSearch.ManufacturerIM = ManufacturerNarroValues;
                        if ($("#ManufacturerCollapseIM").text().trim() != '')
                            $("#ManufacturerCollapseIM").show();
                        else
                            $("#ManufacturerCollapseIM").hide();


                        if ($("#ManufacturerCollapseIM").find('span').length <= 2) {
                            $("#ManufacturerCollapseIM").scrollTop(0).height(50);
                        }
                        else {
                            $("#ManufacturerCollapseIM").scrollTop(0).height(100);
                        }
                        // clearGlobalIMIfNotInFocus();
                        if (!isFromNarrowSearchClear) {
                            DoNarrowSearchIM();
                        }

                    }).multiselectfilter({ label: Filter, placeholder: Enterkeywords });

                _NarrowSearchSave.setControlValue("ManufacturerIM");
                
            }
            
        },
        error: function (response) {
            // through errror message
        }
    });

    //alert('7');

    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _TableName, TextFieldName: 'Category', IsArchived: _IsArchived, IsDeleted: _IsDeleted, "ItemModelCallFrom": ItemCallFrom, ParentID: modelID },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });


            //Destroy widgets before reapplying the filter
            $("#PullCategoryIM").empty();
            $("#PullCategoryIM").multiselect('destroy');
            $("#PullCategoryIM").multiselectfilter('destroy');
            $("#PullCategoryIM").append(s);

            if (typeof (ItemCallFrom) != "undefined" && ItemCallFrom != null && ItemCallFrom.toLowerCase() == "movemtr")
            {
                $("#PullCategoryIM").multiselect
                    (
                        {
                            checkAllText: Check,
                            uncheckAllText: UnCheck,
                            noneSelectedText: Category, selectedList: 5,
                            selectedText: function (numChecked, numTotal, checkedItems) {
                                return Category + ' ' + numChecked + ' ' + selected;
                            }
                        },
                        {
                            checkAll: function (ui) {
                                $("#PullCategoryCollapseIM").html('');
                                for (var i = 0; i <= ui.target.length - 1; i++) {
                                    if ($("#PullCategoryCollapseIM").text().indexOf(ui.target[i].text) == -1) {
                                        $("#PullCategoryCollapseIM").append("<span>" + ui.target[i].text + "</span>");
                                    }
                                }
                                $("#PullCategoryCollapseIM").show();
                            }
                        }
                    )
                    .unbind("multiselectclick multiselectcheckall multiselectuncheckall multiselectloadmore multiselectloadall")
                    .bind("multiselectclick multiselectcheckall multiselectuncheckall multiselectloadmore multiselectloadall", function (event, ui) {
                        if (event.type == 'multiselectloadmore' || event.type == 'multiselectloadall')
                        {
                            var currentDataCount = $('select#PullCategoryIM option').length;
                            var _selectedPMS = $.map($(this).multiselect("getChecked"), function (input) {
                                return input.value;
                            });

                            var _loaddataCount = parseInt(currentDataCount) + parseInt(loadNarrowSearchDataCount);
                            if (event.type == 'multiselectloadall') {
                                _loaddataCount = -1;
                                $(".allpullcategoryim").hide();
                                $(".morepullcategoryim").hide();
                            }
                            $.ajax({
                                'url': '/Master/GetNarrowDDData',
                                data: { TableName: _TableName, TextFieldName: 'Category', IsArchived: _IsArchived, IsDeleted: _IsDeleted, "ItemModelCallFrom": ItemCallFrom, ParentID: modelID, LoadDataCount: _loaddataCount },
                                success: function (response) {
                                    var s = '';
                                    $.each(response.DDData, function (ValData, ValCount) {

                                        var ArrData = ValData.toString().split('[###]');
                                        if ($.inArray(ArrData[1], _selectedPMS) > -1) {
                                            s += '<option selected="selected" value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
                                        }
                                        else {
                                            s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
                                        }
                                    });

                                    //Destroy widgets before reapplying the filter
                                    $("#PullCategoryIM").empty();
                                    $("#PullCategoryIM").append(s);
                                    $("#PullCategoryIM").multiselect("refresh");
                                    $("#PullCategoryIM").multiselectfilter("refresh");

                                    var currentNewDataCount = $('select#PullCategoryIM option').length;
                                    if (parseInt(currentDataCount) >= parseInt(currentNewDataCount)) {
                                        $(".allpullcategoryim").hide();
                                        $(".morepullcategoryim").hide();
                                    }

                                },
                                error: function (response) {
                                    // through errror message
                                }
                            });

                        }
                        else
                        {
                            if (ui.checked) {
                                if ($("#PullCategoryCollapseIM").text().indexOf(ui.text) == -1) {
                                    $("#PullCategoryCollapseIM").append("<span>" + ui.text + "</span>");
                                }
                            }
                            else {
                                if (ui.checked == undefined) {
                                    $("#PullCategoryCollapseIM").html('');
                                }
                                else if (!ui.checked) {
                                    var text = $("#PullCategoryCollapseIM").html();
                                    text = text.replace("<span>" + ui.text + "</span>", '');
                                    $("#PullCategoryCollapseIM").html(text);
                                }
                                else
                                    $("#PullCategoryCollapseIM").html('');
                            }
                            PullCategoryNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                                return input.value;
                            })
                            _NarrowSearchSave.objSearch.PullCategoryIM = PullCategoryNarroValues;

                            if ($("#PullCategoryCollapseIM").text().trim() != '')
                                $("#PullCategoryCollapseIM").show();
                            else
                                $("#PullCategoryCollapseIM").hide();


                            if ($("#PullCategoryCollapseIM").find('span').length <= 2) {
                                $("#PullCategoryCollapseIM").scrollTop(0).height(50);
                            }
                            else {
                                $("#PullCategoryCollapseIM").scrollTop(0).height(100);
                            }
                            //clearGlobalIMIfNotInFocus();
                            if (!isFromNarrowSearchClear) {
                                DoNarrowSearchIM();
                            }
                        }

                        

                    }).multiselectfilter({ label: Filter, placeholder: Enterkeywords });

                _NarrowSearchSave.setControlValue("PullCategoryIM");
            }
            else
            {
                $("#PullCategoryIM").multiselect
                    (
                        {
                            checkAllText: Check,
                            uncheckAllText: UnCheck,
                            noneSelectedText: Category, selectedList: 5,
                            selectedText: function (numChecked, numTotal, checkedItems) {
                                return Category + ' ' + numChecked + ' ' + selected;
                            }
                        },
                        {
                            checkAll: function (ui) {
                                $("#PullCategoryCollapseIM").html('');
                                for (var i = 0; i <= ui.target.length - 1; i++) {
                                    if ($("#PullCategoryCollapseIM").text().indexOf(ui.target[i].text) == -1) {
                                        $("#PullCategoryCollapseIM").append("<span>" + ui.target[i].text + "</span>");
                                    }
                                }
                                $("#PullCategoryCollapseIM").show();
                            }
                        }
                    )
                    .unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                    .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                        if (ui.checked) {
                            if ($("#PullCategoryCollapseIM").text().indexOf(ui.text) == -1) {
                                $("#PullCategoryCollapseIM").append("<span>" + ui.text + "</span>");
                            }
                        }
                        else {
                            if (ui.checked == undefined) {
                                $("#PullCategoryCollapseIM").html('');
                            }
                            else if (!ui.checked) {
                                var text = $("#PullCategoryCollapseIM").html();
                                text = text.replace("<span>" + ui.text + "</span>", '');
                                $("#PullCategoryCollapseIM").html(text);
                            }
                            else
                                $("#PullCategoryCollapseIM").html('');
                        }
                        PullCategoryNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                            return input.value;
                        })
                        _NarrowSearchSave.objSearch.PullCategoryIM = PullCategoryNarroValues;

                        if ($("#PullCategoryCollapseIM").text().trim() != '')
                            $("#PullCategoryCollapseIM").show();
                        else
                            $("#PullCategoryCollapseIM").hide();


                        if ($("#PullCategoryCollapseIM").find('span').length <= 2) {
                            $("#PullCategoryCollapseIM").scrollTop(0).height(50);
                        }
                        else {
                            $("#PullCategoryCollapseIM").scrollTop(0).height(100);
                        }
                        //clearGlobalIMIfNotInFocus();
                        if (!isFromNarrowSearchClear) {
                            DoNarrowSearchIM();
                        }

                    }).multiselectfilter({ label: Filter, placeholder: Enterkeywords });

                _NarrowSearchSave.setControlValue("PullCategoryIM");
            }
            
        },
        error: function (response) {
            // through errror message
        }
    });
}

function GetNarrowDDDataOrd_Common(_TableName, _IsArchived, _IsDeleted) {

    var ItemCallFrom = '';
    if (_narrowSearchForIM.IMCallFromPageName == 'NewPULL') {
        ItemCallFrom = 'newpull';
        if ($('#NewPullAction') != undefined && $('#NewPullAction').val() != "") {
            if ($('#NewPullAction').val() == "Credit") {
                ItemCallFrom = $('#NewPullAction').val();
            }
            else if ($('#NewPullAction').val() == "CreditMS") {
                ItemCallFrom = $('#NewPullAction').val();
            }
            else if ($('#NewPullAction').val() == "Pull") {
                ItemCallFrom = "newpull";
            }
        }
    }
    else if (_narrowSearchForIM.IMCallFromPageName.toLowerCase() == 'NEWCART'.toLowerCase()) {
        ItemCallFrom = 'newcart';
    }
    else if (_narrowSearchForIM.IMCallFromPageName.toLowerCase() == 'receive'.toLowerCase()) {
        ItemCallFrom = 'receive';
    }
    else if (_narrowSearchForIM.IMCallFromPageName.toLowerCase() == 'Materialstaging'.toLowerCase()) {
        ItemCallFrom = 'materialstaging';
    }
    else if (_narrowSearchForIM.IMCallFromPageName.toLowerCase() == 'ps'.toLowerCase()) {
        ItemCallFrom = 'ps';
    }
    else {
        ItemCallFrom = _narrowSearchForIM.controlId;
    }


    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _TableName, TextFieldName: 'SupplierName', IsArchived: _IsArchived, IsDeleted: _IsDeleted, "ItemModelCallFrom": ItemCallFrom, ParentID: modelID },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });


            //Destroy widgets before reapplying the filter
            $("#PullSupplierOrd").empty();
            $("#PullSupplierOrd").multiselect('destroy');
            $("#PullSupplierOrd").multiselectfilter('destroy');            
            $("#PullSupplierOrd").append(s);
            $("#PullSupplierOrd").multiselect
                (
                    {
                        checkAllText: Check,
                        uncheckAllText: UnCheck,
                        noneSelectedText: Supplier, selectedList: 5,
                        selectedText: function (numChecked, numTotal, checkedItems) {
                            return Supplier + ' ' + numChecked + ' ' + selected;
                        }
                    },
                    {
                        checkAll: function (ui) {
                            $("#PullSupplierCollapseOrd").html('');
                            for (var i = 0; i <= ui.target.length - 1; i++) {
                                if ($("#PullSupplierCollapseOrd").text().indexOf(ui.target[i].text) == -1) {
                                    $("#PullSupplierCollapseOrd").append("<span>" + ui.target[i].text + "</span>");
                                }
                            }
                            $("#PullSupplierCollapseOrd").show();
                        }
                    }
                )
                .unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                    if (ui.checked) {
                        if ($("#PullSupplierCollapseOrd").text().indexOf(ui.text) == -1) {
                            $("#PullSupplierCollapseOrd").append("<span>" + ui.text + "</span>");
                        }
                    }
                    else {
                        if (ui.checked == undefined) {
                            $("#PullSupplierCollapseOrd").html('');
                        }
                        else if (!ui.checked) {
                            var text = $("#PullSupplierCollapseOrd").html();
                            text = text.replace("<span>" + ui.text + "</span>", '');
                            $("#PullSupplierCollapseOrd").html(text);
                        }
                        else
                            $("#PullSupplierCollapseOrd").html('');
                    }
                    PullSupplierNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                        return input.value;
                    })
                    _NarrowSearchSave.objSearch.PullSupplierIM = PullSupplierNarroValues;
                    if ($("#PullSupplierCollapseOrd").text().trim() != '')
                        $("#PullSupplierCollapseOrd").show();
                    else
                        $("#PullSupplierCollapseOrd").hide();


                    if ($("#PullSupplierCollapseOrd").find('span').length <= 2) {
                        $("#PullSupplierCollapseOrd").scrollTop(0).height(50);
                    }
                    else {
                        $("#PullSupplierCollapseOrd").scrollTop(0).height(100);
                    }
                    //  clearGlobalIMIfNotInFocus();
                    if (!isFromNarrowSearchClear) {
                        DoNarrowSearchIM();
                    }
                }).multiselectfilter({ label: Filter, placeholder: Enterkeywords });

            _NarrowSearchSave.setControlValue("PullSupplierOrd")

        },
        error: function (response) {
            // through errror message
        }
    });

    //alert('6');

    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _TableName, TextFieldName: 'Manufacturer', IsArchived: _IsArchived, IsDeleted: _IsDeleted, "ItemModelCallFrom": ItemCallFrom, ParentID: modelID },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });


            //Destroy widgets before reapplying the filter
            $("#ManufacturerOrd").empty();
            $("#ManufacturerOrd").multiselect('destroy');
            $("#ManufacturerOrd").multiselectfilter('destroy');

            $("#ManufacturerOrd").append(s);
            $("#ManufacturerOrd").multiselect
                (
                    {
                        checkAllText: Check,
                        uncheckAllText: UnCheck,
                        noneSelectedText: Manufacturer, selectedList: 5,
                        selectedText: function (numChecked, numTotal, checkedItems) {
                            return Manufacturer + ' ' + numChecked + ' ' + selected;
                        }
                    },
                    {
                        checkAll: function (ui) {
                            $("#ManufacturerCollapseOrd").html('');
                            for (var i = 0; i <= ui.target.length - 1; i++) {
                                if ($("#ManufacturerCollapseOrd").text().indexOf(ui.target[i].text) == -1) {
                                    $("#ManufacturerCollapseOrd").append("<span>" + ui.target[i].text + "</span>");
                                }
                            }
                            $("#ManufacturerCollapseOrd").show();
                        }
                    }
                )
                .unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                    if (ui.checked) {
                        if ($("#ManufacturerCollapseOrd").text().indexOf(ui.text) == -1) {
                            $("#ManufacturerCollapseOrd").append("<span>" + ui.text + "</span>");
                        }
                    }
                    else {
                        if (ui.checked == undefined) {
                            $("#ManufacturerCollapseOrd").html('');
                        }
                        else if (!ui.checked) {
                            var text = $("#ManufacturerCollapseOrd").html();
                            text = text.replace("<span>" + ui.text + "</span>", '');
                            $("#ManufacturerCollapseOrd").html(text);
                        }
                        else
                            $("#ManufacturerCollapseOrd").html('');
                    }
                    ManufacturerNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                        return input.value;
                    })
                    _NarrowSearchSave.objSearch.ManufacturerIM = ManufacturerNarroValues;
                    if ($("#ManufacturerCollapseOrd").text().trim() != '')
                        $("#ManufacturerCollapseOrd").show();
                    else
                        $("#ManufacturerCollapseOrd").hide();


                    if ($("#ManufacturerCollapseOrd").find('span').length <= 2) {
                        $("#ManufacturerCollapseOrd").scrollTop(0).height(50);
                    }
                    else {
                        $("#ManufacturerCollapseOrd").scrollTop(0).height(100);
                    }
                    // clearGlobalIMIfNotInFocus();
                    if (!isFromNarrowSearchClear) {
                        DoNarrowSearchIM();
                    }

                }).multiselectfilter({ label: Filter, placeholder: Enterkeywords });

            _NarrowSearchSave.setControlValue("ManufacturerOrd");
        },
        error: function (response) {
            // through errror message
        }
    });

    //alert('7');

    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: _TableName, TextFieldName: 'Category', IsArchived: _IsArchived, IsDeleted: _IsDeleted, "ItemModelCallFrom": ItemCallFrom, ParentID: modelID },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });


            //Destroy widgets before reapplying the filter
            $("#PullCategoryOrd").empty();
            $("#PullCategoryOrd").multiselect('destroy');
            $("#PullCategoryOrd").multiselectfilter('destroy');

            $("#PullCategoryOrd").append(s);
            $("#PullCategoryOrd").multiselect
                (
                    {
                        checkAllText: Check,
                        uncheckAllText: UnCheck,
                        noneSelectedText: Category, selectedList: 5,
                        selectedText: function (numChecked, numTotal, checkedItems) {
                            return Category + ' ' + numChecked + ' ' + selected;
                        }
                    },
                    {
                        checkAll: function (ui) {
                            $("#PullCategoryCollapseOrd").html('');
                            for (var i = 0; i <= ui.target.length - 1; i++) {
                                if ($("#PullCategoryCollapseOrd").text().indexOf(ui.target[i].text) == -1) {
                                    $("#PullCategoryCollapseOrd").append("<span>" + ui.target[i].text + "</span>");
                                }
                            }
                            $("#PullCategoryCollapseOrd").show();
                        }
                    }
                )
                .unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                    if (ui.checked) {
                        if ($("#PullCategoryCollapseOrd").text().indexOf(ui.text) == -1) {
                            $("#PullCategoryCollapseOrd").append("<span>" + ui.text + "</span>");
                        }
                    }
                    else {
                        if (ui.checked == undefined) {
                            $("#PullCategoryCollapseOrd").html('');
                        }
                        else if (!ui.checked) {
                            var text = $("#PullCategoryCollapseOrd").html();
                            text = text.replace("<span>" + ui.text + "</span>", '');
                            $("#PullCategoryCollapseOrd").html(text);
                        }
                        else
                            $("#PullCategoryCollapseOrd").html('');
                    }
                    PullCategoryNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                        return input.value;
                    })
                    _NarrowSearchSave.objSearch.PullCategoryIM = PullCategoryNarroValues;

                    if ($("#PullCategoryCollapseOrd").text().trim() != '')
                        $("#PullCategoryCollapseOrd").show();
                    else
                        $("#PullCategoryCollapseOrd").hide();


                    if ($("#PullCategoryCollapseOrd").find('span').length <= 2) {
                        $("#PullCategoryCollapseOrd").scrollTop(0).height(50);
                    }
                    else {
                        $("#PullCategoryCollapseOrd").scrollTop(0).height(100);
                    }
                    //clearGlobalIMIfNotInFocus();
                    if (!isFromNarrowSearchClear) {
                        DoNarrowSearchIM();
                    }

                }).multiselectfilter({ label: Filter, placeholder: Enterkeywords });

            _NarrowSearchSave.setControlValue("PullCategoryCollapseOrd");
        },
        error: function (response) {
            // through errror message
        }
    });
}

function GetNarrowDDDataIM(tableName, companyID, roomID) {
    var ItemCallFrom = '';
    if (_narrowSearchForIM.IMCallFromPageName == 'NewPULL') {
        ItemCallFrom = 'newpull';
        if ($('#NewPullAction') != undefined && $('#NewPullAction').val() != "") {
            if ($('#NewPullAction').val() == "Credit") {
                ItemCallFrom = $('#NewPullAction').val();
            }
            else if ($('#NewPullAction').val() == "CreditMS") {
                ItemCallFrom = $('#NewPullAction').val();
            }
            else if ($('#NewPullAction').val() == "Pull") {
                ItemCallFrom = "newpull";
            }
        }
    }
    else if (_narrowSearchForIM.IMCallFromPageName.toLowerCase() == 'receive'.toLowerCase()) {
        ItemCallFrom = 'receive';
    }
    else if (_narrowSearchForIM.IMCallFromPageName.toLowerCase() == 'NEWCART'.toLowerCase()) {
        ItemCallFrom = 'newcart';
    }
    else if (_narrowSearchForIM.IMCallFromPageName.toLowerCase() == 'Materialstaging'.toLowerCase()) {
        ItemCallFrom = 'materialstaging';
    }
    else if (_narrowSearchForIM.IMCallFromPageName.toLowerCase() == 'ps'.toLowerCase()) {
        ItemCallFrom = 'ps';
    }
    else {
        ItemCallFrom = _narrowSearchForIM.controlId;
    }

    //alert('2');

    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: tableName, TextFieldName: 'CreatedBy', IsArchived: false, IsDeleted: false, "ItemModelCallFrom": ItemCallFrom, ParentID: modelID },
        success: function (response) {
            var s = '';

            if (response.DDData != null) {
                $.each(response.DDData, function (i, val) {
                    var ArrData1 = i.toString().split('[###]');
                    s += '<option value="' + ArrData1[1] + '">' + ArrData1[0] + '(' + val + ')' + '</option>';
                });
            }

            $("#UserCreatedIM").empty();
            $("#UserCreatedIM").multiselect('destroy');
            $("#UserCreatedIM").multiselectfilter('destroy');

            $("#UserCreatedIM").append(s);
            $("#UserCreatedIM").multiselect
                (
                    {
                        checkAllText: Check,
                        uncheckAllText: UnCheck,
                        noneSelectedText: UserCreatedBy, selectedList: 5,
                        selectedText: function (numChecked, numTotal, checkedItems) {
                            return CreatedBy + ' ' + numChecked + ' ' + selected;
                        }
                    },
                    {
                        checkAll: function (ui) {
                            $("#UserCreatedCollapseIM").html('');
                            for (var i = 0; i <= ui.target.length - 1; i++) {
                                if ($("#UserCreatedCollapseIM").text().indexOf(ui.target[i].text) == -1) {
                                    $("#UserCreatedCollapseIM").append("<span>" + ui.target[i].text + "</span>");
                                }
                            }
                            $("#UserCreatedCollapseIM").show();
                        }
                    }
            )
                .unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                    if (ui.checked) {
                        if ($("#UserCreatedCollapseIM").text().indexOf(ui.text) == -1) {
                            $("#UserCreatedCollapseIM").append("<span>" + ui.text + "</span>");
                        }
                    }
                    else {
                        if (ui.checked == undefined) {
                            $("#UserCreatedCollapseIM").html('');
                        }
                        else if (!ui.checked) {
                            var text = $("#UserCreatedCollapseIM").html();
                            text = text.replace("<span>" + ui.text + "</span>", '');
                            $("#UserCreatedCollapseIM").html(text);
                        }
                        else
                            $("#UserCreatedCollapseIM").html('');
                    }
                    UserCreatedNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                        return input.value;
                    })
                    _NarrowSearchSave.objSearch.UserCreatedIM = UserCreatedNarroValues;

                    if ($("#UserCreatedCollapseIM").text().trim() != '')
                        $("#UserCreatedCollapseIM").show();
                    else
                        $("#UserCreatedCollapseIM").hide();


                    if ($("#UserCreatedCollapseIM").find('span').length <= 2) {
                        $("#UserCreatedCollapseIM").scrollTop(0).height(50);
                    }
                    else {
                        $("#UserCreatedCollapseIM").scrollTop(0).height(100);
                    }
                    //clearGlobalIMIfNotInFocus();
                    if (!isFromNarrowSearchClear) {
                        DoNarrowSearchIM();
                    }
                    
                }).multiselectfilter({ label: Filter, placeholder: Enterkeywords });

            _NarrowSearchSave.setControlValue("UserCreatedIM");
        },
        error: function (response) {
            // through errror message
        }
    });

    //alert('3');

    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: tableName, TextFieldName: 'LastUpdatedBy', IsArchived: false, IsDeleted: false, "ItemModelCallFrom": ItemCallFrom, ParentID: modelID },
        success: function (response) {
            var s = '';
            if (response.DDData != null) {
                $.each(response.DDData, function (i, val) {
                    var ArrData1 = i.toString().split('[###]');
                    s += '<option value="' + ArrData1[1] + '">' + ArrData1[0] + '(' + val + ')' + '</option>';
                });
            }
            $("#UserUpdatedIM").empty();
            $("#UserUpdatedIM").multiselect('destroy');
            $("#UserUpdatedIM").multiselectfilter('destroy');

            $("#UserUpdatedIM").append(s);
            $("#UserUpdatedIM").multiselect
                (
                    {
                        checkAllText: Check,
                        uncheckAllText: UnCheck,
                        noneSelectedText: UserUpdatedby, selectedList: 5,
                        selectedText: function (numChecked, numTotal, checkedItems) {
                            return UpdatedBy + ' ' + numChecked + ' ' + selected;
                        }
                    },
                    {
                        checkAll: function (ui) {
                            $("#UserUpdatedCollapseIM").html('');
                            for (var i = 0; i <= ui.target.length - 1; i++) {
                                if ($("#UserUpdatedCollapseIM").text().indexOf(ui.target[i].text) == -1) {
                                    $("#UserUpdatedCollapseIM").append("<span>" + ui.target[i].text + "</span>");
                                }
                            }
                            $("#UserUpdatedCollapseIM").show();
                        }
                    }
            )
                .unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                    if (ui.checked) {
                        if ($("#UserUpdatedCollapseIM").text().indexOf(ui.text) == -1) {
                            $("#UserUpdatedCollapseIM").append("<span>" + ui.text + "</span>");
                        }
                    }
                    else {
                        if (ui.checked == undefined) {
                            $("#UserUpdatedCollapseIM").html('');
                        }
                        else if (!ui.checked) {
                            var text = $("#UserUpdatedCollapseIM").html();
                            text = text.replace("<span>" + ui.text + "</span>", '');
                            $("#UserUpdatedCollapseIM").html(text);
                        }
                        else
                            $("#UserUpdatedCollapseIM").html('');
                    }
                    UserUpdatedNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                        return input.value;
                    })

                    _NarrowSearchSave.objSearch.UserUpdatedIM = UserUpdatedNarroValues;

                    if ($("#UserUpdatedCollapseIM").text().trim() != '')
                        $("#UserUpdatedCollapseIM").show();
                    else
                        $("#UserUpdatedCollapseIM").hide();

                    if ($("#UserUpdatedCollapseIM").find('span').length <= 2) {
                        $("#UserUpdatedCollapseIM").scrollTop(0).height(50);
                    }
                    else {
                        $("#UserUpdatedCollapseIM").scrollTop(0).height(100);
                    }
                    //  clearGlobalIMIfNotInFocus();
                    if (!isFromNarrowSearchClear) {
                        DoNarrowSearchIM();
                    }                    
                }).multiselectfilter({ label: Filter, placeholder: Enterkeywords });

            _NarrowSearchSave.setControlValue("UserUpdatedIM");
        },
        error: function (response) {
            // through errror message
        }
    });



}

function GetNarrowDDDataOrd(tableName, companyID, roomID) {
    var ItemCallFrom = '';
    if (_narrowSearchForIM.IMCallFromPageName == 'NewPULL') {
        ItemCallFrom = 'newpull';
        if ($('#NewPullAction') != undefined && $('#NewPullAction').val() != "") {
            if ($('#NewPullAction').val() == "Credit") {
                ItemCallFrom = $('#NewPullAction').val();
            }
            else if ($('#NewPullAction').val() == "CreditMS") {
                ItemCallFrom = $('#NewPullAction').val();
            }
            else if ($('#NewPullAction').val() == "Pull") {
                ItemCallFrom = "newpull";
            }
        }
    }
    else if (_narrowSearchForIM.IMCallFromPageName.toLowerCase() == 'receive'.toLowerCase()) {
        ItemCallFrom = 'receive';
    }
    else if (_narrowSearchForIM.IMCallFromPageName.toLowerCase() == 'NEWCART'.toLowerCase()) {
        ItemCallFrom = 'newcart';
    }
    else if (_narrowSearchForIM.IMCallFromPageName.toLowerCase() == 'Materialstaging'.toLowerCase()) {
        ItemCallFrom = 'materialstaging';
    }
    else if (_narrowSearchForIM.IMCallFromPageName.toLowerCase() == 'ps'.toLowerCase()) {
        ItemCallFrom = 'ps';
    }
    else {
        ItemCallFrom = _narrowSearchForIM.controlId;
    }

    //alert('2');

    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: tableName, TextFieldName: 'CreatedBy', IsArchived: false, IsDeleted: false, "ItemModelCallFrom": ItemCallFrom, ParentID: modelID },
        success: function (response) {
            var s = '';

            if (response.DDData != null) {
                $.each(response.DDData, function (i, val) {
                    var ArrData1 = i.toString().split('[###]');
                    s += '<option value="' + ArrData1[1] + '">' + ArrData1[0] + '(' + val + ')' + '</option>';
                });
            }

            $("#UserCreatedOrd").empty();
            $("#UserCreatedOrd").multiselect('destroy');
            $("#UserCreatedOrd").multiselectfilter('destroy');

            $("#UserCreatedOrd").append(s);
            $("#UserCreatedOrd").multiselect
                (
                    {
                        checkAllText: Check,
                        uncheckAllText: UnCheck,
                        noneSelectedText: UserCreatedBy, selectedList: 5,
                        selectedText: function (numChecked, numTotal, checkedItems) {
                            return CreatedBy + ' ' + numChecked + ' ' + selected;
                        }
                    },
                    {
                        checkAll: function (ui) {
                            $("#UserCreatedCollapseOrd").html('');
                            for (var i = 0; i <= ui.target.length - 1; i++) {
                                if ($("#UserCreatedCollapseOrd").text().indexOf(ui.target[i].text) == -1) {
                                    $("#UserCreatedCollapseOrd").append("<span>" + ui.target[i].text + "</span>");
                                }
                            }
                            $("#UserCreatedCollapseOrd").show();
                        }
                    }
                )
                .unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                    if (ui.checked) {
                        if ($("#UserCreatedCollapseOrd").text().indexOf(ui.text) == -1) {
                            $("#UserCreatedCollapseOrd").append("<span>" + ui.text + "</span>");
                        }
                    }
                    else {
                        if (ui.checked == undefined) {
                            $("#UserCreatedCollapseOrd").html('');
                        }
                        else if (!ui.checked) {
                            var text = $("#UserCreatedCollapseOrd").html();
                            text = text.replace("<span>" + ui.text + "</span>", '');
                            $("#UserCreatedCollapseOrd").html(text);
                        }
                        else
                            $("#UserCreatedCollapseOrd").html('');
                    }
                    UserCreatedNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                        return input.value;
                    })
                    _NarrowSearchSave.objSearch.UserCreatedIM = UserCreatedNarroValues;

                    if ($("#UserCreatedCollapseOrd").text().trim() != '')
                        $("#UserCreatedCollapseOrd").show();
                    else
                        $("#UserCreatedCollapseOrd").hide();


                    if ($("#UserCreatedCollapseOrd").find('span').length <= 2) {
                        $("#UserCreatedCollapseOrd").scrollTop(0).height(50);
                    }
                    else {
                        $("#UserCreatedCollapseOrd").scrollTop(0).height(100);
                    }
                    //clearGlobalIMIfNotInFocus();
                    if (!isFromNarrowSearchClear) {
                        DoNarrowSearchIM();
                    }

                }).multiselectfilter({ label: Filter, placeholder: Enterkeywords });

            _NarrowSearchSave.setControlValue("UserCreatedOrd");
        },
        error: function (response) {
            // through errror message
        }
    });

    //alert('3');

    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: tableName, TextFieldName: 'LastUpdatedBy', IsArchived: false, IsDeleted: false, "ItemModelCallFrom": ItemCallFrom, ParentID: modelID },
        success: function (response) {
            var s = '';
            if (response.DDData != null) {
                $.each(response.DDData, function (i, val) {
                    var ArrData1 = i.toString().split('[###]');
                    s += '<option value="' + ArrData1[1] + '">' + ArrData1[0] + '(' + val + ')' + '</option>';
                });
            }
            $("#UserUpdatedOrd").empty();
            $("#UserUpdatedOrd").multiselect('destroy');
            $("#UserUpdatedOrd").multiselectfilter('destroy');

            $("#UserUpdatedOrd").append(s);
            $("#UserUpdatedOrd").multiselect
                (
                    {
                        checkAllText: Check,
                        uncheckAllText: UnCheck,
                        noneSelectedText: UserUpdatedby, selectedList: 5,
                        selectedText: function (numChecked, numTotal, checkedItems) {
                            return UpdatedBy + ' ' + numChecked + ' ' + selected;
                        }
                    },
                    {
                        checkAll: function (ui) {
                            $("#UserUpdatedCollapseOrd").html('');
                            for (var i = 0; i <= ui.target.length - 1; i++) {
                                if ($("#UserUpdatedCollapseOrd").text().indexOf(ui.target[i].text) == -1) {
                                    $("#UserUpdatedCollapseOrd").append("<span>" + ui.target[i].text + "</span>");
                                }
                            }
                            $("#UserUpdatedCollapseOrd").show();
                        }
                    }
                )
                .unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                    if (ui.checked) {
                        if ($("#UserUpdatedCollapseOrd").text().indexOf(ui.text) == -1) {
                            $("#UserUpdatedCollapseOrd").append("<span>" + ui.text + "</span>");
                        }
                    }
                    else {
                        if (ui.checked == undefined) {
                            $("#UserUpdatedCollapseOrd").html('');
                        }
                        else if (!ui.checked) {
                            var text = $("#UserUpdatedCollapseOrd").html();
                            text = text.replace("<span>" + ui.text + "</span>", '');
                            $("#UserUpdatedCollapseOrd").html(text);
                        }
                        else
                            $("#UserUpdatedCollapseOrd").html('');
                    }
                    UserUpdatedNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                        return input.value;
                    })

                    _NarrowSearchSave.objSearch.UserUpdatedIM = UserUpdatedNarroValues;

                    if ($("#UserUpdatedCollapseOrd").text().trim() != '')
                        $("#UserUpdatedCollapseOrd").show();
                    else
                        $("#UserUpdatedCollapseOrd").hide();

                    if ($("#UserUpdatedCollapseOrd").find('span').length <= 2) {
                        $("#UserUpdatedCollapseOrd").scrollTop(0).height(50);
                    }
                    else {
                        $("#UserUpdatedCollapseOrd").scrollTop(0).height(100);
                    }
                    //  clearGlobalIMIfNotInFocus();
                    if (!isFromNarrowSearchClear) {
                        DoNarrowSearchIM();
                    }
                }).multiselectfilter({ label: Filter, placeholder: Enterkeywords });

            _NarrowSearchSave.setControlValue("UserUpdatedOrd");
        },
        error: function (response) {
            // through errror message
        }
    });



}

function GetNarroHTMLForUDFIM(tableName, companyID, roomID) {
    var ItemCallFrom = '';
    if (_narrowSearchForIM.IMCallFromPageName == 'NewPULL') {
        ItemCallFrom = 'newpull';
        if ($('#NewPullAction') != undefined && $('#NewPullAction').val() != "") {
            if ($('#NewPullAction').val() == "Credit") {
                ItemCallFrom = $('#NewPullAction').val();
            }
            else if ($('#NewPullAction').val() == "CreditMS") {
                ItemCallFrom = $('#NewPullAction').val();
            }
            else if ($('#NewPullAction').val() == "Pull") {
                ItemCallFrom = "newpull";
            }
        }
    }
    else if (_narrowSearchForIM.IMCallFromPageName.toLowerCase() == 'NEWCART'.toLowerCase()) {
        ItemCallFrom = 'newcart';
    }
    else if (_narrowSearchForIM.IMCallFromPageName.toLowerCase() == 'receive'.toLowerCase()) {
        ItemCallFrom = 'receive';
    }
    else if (_narrowSearchForIM.IMCallFromPageName.toLowerCase() == 'Materialstaging'.toLowerCase()) {
        ItemCallFrom = 'materialstaging';
    }
    else if (_narrowSearchForIM.IMCallFromPageName.toLowerCase() == 'ps'.toLowerCase()) {
        ItemCallFrom = 'ps';
    }
    else {
        ItemCallFrom = _narrowSearchForIM.controlId;
    }
    var UDFObject;
    $("select[name='udflist_Item']").each(function (index) {
        var UDFUniqueID = this.getAttribute('UID');
        var DataValue = { TableName: tableName, UDFName: this.id, UDFUniqueID: UDFUniqueID, IsArchived: false, IsDeleted: false, "ItemModelCallFrom": ItemCallFrom, ParentID: modelID }
        if (ItemCallFrom.toLowerCase() == "movemtr") {
            DataValue = { TableName: tableName, UDFName: this.id, UDFUniqueID: UDFUniqueID, IsArchived: false, IsDeleted: false, "ItemModelCallFrom": ItemCallFrom, ParentID: modelID, moveType: $('#ddlMoveType').val() };
        }

        $.ajax({
            'url': '/Master/GetUDFDDData',
            data: DataValue,
            success: function (response) {
                
                var s = '';
                if (response.DDData != null) {
                    if (ItemCallFrom.toLowerCase() == "newpull"
                        || ItemCallFrom.toLowerCase() == "credit"
                        || ItemCallFrom.toLowerCase() == "creditms"
                        || ItemCallFrom.toLowerCase() == "credit"
                        || ItemCallFrom.toLowerCase() == "creditms"
                        || ItemCallFrom.toLowerCase() == "rq"
                        || ItemCallFrom.toLowerCase() == "ord"
                        || ItemCallFrom.toLowerCase() == "retord"
                        || ItemCallFrom.toLowerCase() == "ql"
                        || ItemCallFrom.toLowerCase() == "trf"
                        || ItemCallFrom.toLowerCase() == "newcart"
                        || ItemCallFrom.toLowerCase() == "movemtr"
                        || ItemCallFrom.toLowerCase() == "mntnance"
                        || ItemCallFrom.toLowerCase() == "as"
                        || ItemCallFrom.toLowerCase() == "receive"
                        || ItemCallFrom.toLowerCase() == "materialstaging"
                        || ItemCallFrom.toLowerCase() == "ps"
                        || tableName.toLowerCase() == "itemmaster") {
                        $.each(response.DDData, function (ValData, ValCount) {
                            var ArrData = ValData.toString().split('[###]');
                            s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
                        });
                    }
                    else {
                        $.each(response.DDData, function (UDFVal, ValCount) {
                            s += '<option value="' + UDFVal + '">' + UDFVal + '(' + ValCount + ')' + '</option>';
                        });
                    }
                }
                var UDFColumnNameTemp = response.UDFColName.toString().replace("_dd_Item", "");
                //$('#' + response.UDFColName).append(s);
                //$('#' + response.UDFColName).multiselect

                //Destroy widgets before reapplying the filter
                $('[id="' + response.UDFColName + '"]').empty();
                $('[id="' + response.UDFColName + '"]').multiselect('destroy');
                $('[id="' + response.UDFColName + '"]').multiselectfilter('destroy');

                $('[id="' + response.UDFColName + '"]').append(s);

                if (ItemCallFrom.toLowerCase() == "movemtr")
                {
                    $('[id="' + response.UDFColName + '"]').multiselect
                        (
                            {
                                checkAllText: Check,
                                uncheckAllText: UnCheck,
                                noneSelectedText: UDFColumnNameTemp, selectedList: 5,
                                selectedText: function (numChecked, numTotal, checkedItems) {
                                    return UDFColumnNameTemp + ' ' + numChecked + ' ' + selected;
                                }
                            },
                            {
                                checkAll: function (ui) {
                                    var CollapseObject = $('#' + UDFUniqueID + 'Collapse_Item')
                                    $(CollapseObject).html('');
                                    for (var i = 0; i <= ui.target.length - 1; i++) {
                                        if ($(CollapseObject).text().indexOf(ui.target[i].text) == -1) {
                                            $(CollapseObject).append("<span>" + ui.target[i].text + "</span>");
                                        }
                                    }
                                    $(CollapseObject).show();
                                }
                            }
                        )
                        .unbind("multiselectclick multiselectcheckall multiselectuncheckall multiselectloadmore multiselectloadall")
                        .bind("multiselectclick multiselectcheckall multiselectuncheckall multiselectloadmore multiselectloadall", function (event, ui) {

                            if (event.type == 'multiselectloadmore' || event.type == 'multiselectloadall')
                            {
                                var currentDataCount = $("select[id='" + this.id + "'] option").length; //$('select#' + this.id + ' option').length;
                                var _selectedPUDF = $.map($(this).multiselect("getChecked"), function (input) {
                                    return input.value;
                                });
                                var _loaddataCount = parseInt(currentDataCount) + parseInt(loadNarrowSearchDataCount);
                                if (event.type == 'multiselectloadall') {
                                    _loaddataCount = -1;
                                    $(".allmovemtr" + UDFUniqueID + "").hide();
                                    $(".moremovemtr" + UDFUniqueID + "").hide();
                                }

                                $.ajax({
                                    'url': '/Master/GetUDFDDData',
                                    data: { TableName: tableName, UDFName: this.id, UDFUniqueID: UDFUniqueID, IsArchived: false, IsDeleted: false, "ItemModelCallFrom": ItemCallFrom, ParentID: modelID, moveType: $('#ddlMoveType').val(), LoadDataCount: _loaddataCount },
                                    success: function (responseNew) {
                                        var s = '';
                                        if (responseNew.DDData != null)
                                        {
                                            $.each(responseNew.DDData, function (i, val) {
                                                var ArrData = i.toString().split('[###]');
                                                if ($.inArray(ArrData[1], _selectedPUDF) > -1) {
                                                    s += '<option selected="selected" value="' + ArrData[1] + '">' + ArrData[0] + ' (' + val + ')' + '</option>';
                                                }
                                                else {
                                                    s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + val + ')' + '</option>';
                                                }
                                            });
                                        }

                                        //Destroy widgets before reapplying the filter
                                        $('[id="' + responseNew.UDFColName + '"]').empty();
                                        $('[id="' + responseNew.UDFColName + '"]').append(s);
                                        $('[id="' + responseNew.UDFColName + '"]').multiselect("refresh");
                                        $('[id="' + responseNew.UDFColName + '"]').multiselectfilter("refresh");

                                        var currentNewDataCount = $("select[id='" + responseNew.UDFColName + "'] option").length; //$('select#' + responseNew.UDFColName + ' option').length;
                                        if (parseInt(currentDataCount) >= parseInt(currentNewDataCount)) {
                                            $(".allmovemtr" + UDFUniqueID + "").hide();
                                            $(".moremovemtr" + UDFUniqueID + "").hide();
                                        }
                                    },
                                    error: function (responseNew) {
                                        // through errror message
                                    }
                                });
                            }
                            else
                            {
                                var CollapseObject = $('#' + UDFUniqueID + 'Collapse_Item')
                                if (ui.checked) {
                                    if ($(CollapseObject).text().indexOf(ui.text) == -1) {
                                        $(CollapseObject).append("<span>" + ui.text + "</span>");
                                    }
                                }
                                else {
                                    if (ui.checked == undefined) {
                                        $(CollapseObject).html('');
                                    }
                                    else if (!ui.checked) {
                                        var text = $(CollapseObject).html();
                                        text = text.replace("<span>" + ui.text + "</span>", '');
                                        $(CollapseObject).html(text);
                                    }
                                    else
                                        $(CollapseObject).html('');
                                }
                                if (UDFUniqueID == "UDF1") {
                                    UserUDF1NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                                        return input.value;
                                    })
                                    _NarrowSearchSave.objSearch.UDF1 = UserUDF1NarrowValues;
                                }
                                else if (UDFUniqueID == "UDF2") {
                                    UserUDF2NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                                        return input.value;
                                    })
                                    _NarrowSearchSave.objSearch.UDF2 = UserUDF2NarrowValues;
                                }
                                else if (UDFUniqueID == "UDF3") {
                                    UserUDF3NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                                        return input.value;
                                    })
                                    _NarrowSearchSave.objSearch.UDF3 = UserUDF3NarrowValues;
                                }
                                else if (UDFUniqueID == "UDF4") {
                                    UserUDF4NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                                        return input.value;
                                    })
                                    _NarrowSearchSave.objSearch.UDF4 = UserUDF4NarrowValues;
                                }
                                else if (UDFUniqueID == "UDF5") {
                                    UserUDF5NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                                        return input.value;
                                    })
                                    _NarrowSearchSave.objSearch.UDF5 = UserUDF5NarrowValues;
                                }
                                else if (UDFUniqueID == "UDF6") {
                                    UserUDF6NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                                        return input.value;
                                    })
                                    _NarrowSearchSave.objSearch.UDF6 = UserUDF6NarrowValues;
                                }
                                else if (UDFUniqueID == "UDF7") {
                                    UserUDF7NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                                        return input.value;
                                    })
                                    _NarrowSearchSave.objSearch.UDF7 = UserUDF7NarrowValues;
                                }
                                else if (UDFUniqueID == "UDF8") {
                                    UserUDF8NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                                        return input.value;
                                    })
                                    _NarrowSearchSave.objSearch.UDF8 = UserUDF8NarrowValues;
                                }
                                else if (UDFUniqueID == "UDF9") {
                                    UserUDF9NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                                        return input.value;
                                    })
                                    _NarrowSearchSave.objSearch.UDF9 = UserUDF9NarrowValues;
                                }
                                else if (UDFUniqueID == "UDF10") {
                                    UserUDF10NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                                        return input.value;
                                    })
                                    _NarrowSearchSave.objSearch.UDF10 = UserUDF10NarrowValues;
                                }

                                if ($(CollapseObject).text().trim() != '')
                                    $(CollapseObject).show();
                                else
                                    $(CollapseObject).hide();


                                if ($(CollapseObject).find('span').length <= 2) {
                                    $(CollapseObject).scrollTop(0).height(50);
                                }
                                else {
                                    $(CollapseObject).scrollTop(0).height(100);
                                }
                                // clearGlobalIMIfNotInFocus();
                                if (!isFromNarrowSearchClear) {
                                    DoNarrowSearchIM();
                                }
                            }
                            
                        }).multiselectfilter({ label: Filter, placeholder: Enterkeywords });

                    var UDFUniqueIDVal = _Common.getQueryStringVal(this.url, "UDFUniqueID");
                    _NarrowSearchSave.setControlValue(UDFUniqueIDVal);
                }
                else
                {
                    $('[id="' + response.UDFColName + '"]').multiselect
                        (
                            {
                                checkAllText: Check,
                                uncheckAllText: UnCheck,
                                noneSelectedText: UDFColumnNameTemp, selectedList: 5,
                                selectedText: function (numChecked, numTotal, checkedItems) {
                                    return UDFColumnNameTemp + ' ' + numChecked + ' ' + selected;
                                }
                            },
                            {
                                checkAll: function (ui) {
                                    var CollapseObject = $('#' + UDFUniqueID + 'Collapse_Item')
                                    $(CollapseObject).html('');
                                    for (var i = 0; i <= ui.target.length - 1; i++) {
                                        if ($(CollapseObject).text().indexOf(ui.target[i].text) == -1) {
                                            $(CollapseObject).append("<span>" + ui.target[i].text + "</span>");
                                        }
                                    }
                                    $(CollapseObject).show();
                                }
                            }
                        )
                        .unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                        .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                            var CollapseObject = $('#' + UDFUniqueID + 'Collapse_Item')
                            if (ui.checked) {
                                if ($(CollapseObject).text().indexOf(ui.text) == -1) {
                                    $(CollapseObject).append("<span>" + ui.text + "</span>");
                                }
                            }
                            else {
                                if (ui.checked == undefined) {
                                    $(CollapseObject).html('');
                                }
                                else if (!ui.checked) {
                                    var text = $(CollapseObject).html();
                                    text = text.replace("<span>" + ui.text + "</span>", '');
                                    $(CollapseObject).html(text);
                                }
                                else
                                    $(CollapseObject).html('');
                            }
                            if (UDFUniqueID == "UDF1") {
                                UserUDF1NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                                    return input.value;
                                })
                                _NarrowSearchSave.objSearch.UDF1 = UserUDF1NarrowValues;
                            }
                            else if (UDFUniqueID == "UDF2") {
                                UserUDF2NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                                    return input.value;
                                })
                                _NarrowSearchSave.objSearch.UDF2 = UserUDF2NarrowValues;
                            }
                            else if (UDFUniqueID == "UDF3") {
                                UserUDF3NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                                    return input.value;
                                })
                                _NarrowSearchSave.objSearch.UDF3 = UserUDF3NarrowValues;
                            }
                            else if (UDFUniqueID == "UDF4") {
                                UserUDF4NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                                    return input.value;
                                })
                                _NarrowSearchSave.objSearch.UDF4 = UserUDF4NarrowValues;
                            }
                            else if (UDFUniqueID == "UDF5") {
                                UserUDF5NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                                    return input.value;
                                })
                                _NarrowSearchSave.objSearch.UDF5 = UserUDF5NarrowValues;
                            }
                            else if (UDFUniqueID == "UDF6") {
                                UserUDF6NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                                    return input.value;
                                })
                                _NarrowSearchSave.objSearch.UDF6 = UserUDF6NarrowValues;
                            }
                            else if (UDFUniqueID == "UDF7") {
                                UserUDF7NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                                    return input.value;
                                })
                                _NarrowSearchSave.objSearch.UDF7 = UserUDF7NarrowValues;
                            }
                            else if (UDFUniqueID == "UDF8") {
                                UserUDF8NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                                    return input.value;
                                })
                                _NarrowSearchSave.objSearch.UDF8 = UserUDF8NarrowValues;
                            }
                            else if (UDFUniqueID == "UDF9") {
                                UserUDF9NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                                    return input.value;
                                })
                                _NarrowSearchSave.objSearch.UDF9 = UserUDF9NarrowValues;
                            }
                            else if (UDFUniqueID == "UDF10") {
                                UserUDF10NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                                    return input.value;
                                })
                                _NarrowSearchSave.objSearch.UDF10 = UserUDF10NarrowValues;
                            }

                            if ($(CollapseObject).text().trim() != '')
                                $(CollapseObject).show();
                            else
                                $(CollapseObject).hide();


                            if ($(CollapseObject).find('span').length <= 2) {
                                $(CollapseObject).scrollTop(0).height(50);
                            }
                            else {
                                $(CollapseObject).scrollTop(0).height(100);
                            }
                            // clearGlobalIMIfNotInFocus();
                            if (!isFromNarrowSearchClear) {
                                DoNarrowSearchIM();
                            }
                        }).multiselectfilter({ label: Filter, placeholder: Enterkeywords });

                    var UDFUniqueIDVal = _Common.getQueryStringVal(this.url, "UDFUniqueID");
                    _NarrowSearchSave.setControlValue(UDFUniqueIDVal);
                }               

            },
            error: function (response) {

                // through errror message
            }
        });
    });
}


/* Narrow Search For Item Type Filter START */
function GetNarroHTMLForItemTypeIM() {

    var ModelTextTemp = modelText;
    if (ModelTextTemp == null)
        ModelTextTemp = "eTurns";

    var qlType = '1';
    if (gblActionName.toLowerCase() === "assetlist" || gblActionName.toLowerCase() === "toollist") {
        qlType = '2';
    }
    else if (gblActionName.toLowerCase() == "inventorycountlist") {
        qlType = '3';
    }

    var ItemCallFrom = '';
    if (_narrowSearchForIM.IMCallFromPageName == 'NewPULL') {
        ItemCallFrom = 'newpull';
        if ($('#NewPullAction') != undefined && $('#NewPullAction').val() != "") {
            if ($('#NewPullAction').val() == "Credit") {
                ItemCallFrom = $('#NewPullAction').val();
            }
            else if ($('#NewPullAction').val() == "CreditMS") {
                ItemCallFrom = $('#NewPullAction').val();
            }
            else if ($('#NewPullAction').val() == "Pull") {
                ItemCallFrom = "newpull";
            }
        }
    }
    else if (_narrowSearchForIM.IMCallFromPageName.toLowerCase() == 'receive'.toLowerCase()) {
        ItemCallFrom = 'receive';
    }
    else if (_narrowSearchForIM.IMCallFromPageName.toLowerCase() == 'NEWCART'.toLowerCase()) {
        ItemCallFrom = 'newcart';
    }
    else if (_narrowSearchForIM.IMCallFromPageName.toLowerCase() == 'Materialstaging'.toLowerCase()) {
        ItemCallFrom = 'materialstaging';
    }
    else if (_narrowSearchForIM.IMCallFromPageName.toLowerCase() == 'ps'.toLowerCase()) {
        ItemCallFrom = 'ps';
    }
    else {
        ItemCallFrom = _narrowSearchForIM.controlId; //'@Model.ControlID';
        }

    var CalledPageName = _narrowSearchForIM.pageName;
    var DataValue = { TableName: _narrowSearchForIM.pageName, TextFieldName: 'ItemType', IsArchived: false, IsDeleted: false, QLType: qlType, ParentID: modelID, ItemModelCallFrom: ItemCallFrom };
    if (ItemCallFrom.toLowerCase() == "movemtr") {
        DataValue = { TableName: _narrowSearchForIM.pageName, TextFieldName: 'ItemType', IsArchived: false, IsDeleted: false, QLType: qlType, ParentID: modelID, ItemModelCallFrom: ItemCallFrom, moveType: $('#ddlMoveType').val() };
    }
    //alert('9');
    $.ajax({
        'url': '/Master/GetNarrowDDDataIMItemType',
        data: DataValue,
        success: function (response) {
            var s = '';
            if (response.DDData != null) {
                if (ItemCallFrom.toLowerCase() == "newpull"
                    || ItemCallFrom.toLowerCase() == "credit"
                    || ItemCallFrom.toLowerCase() == "creditms"
                    || ItemCallFrom.toLowerCase() == "rq"
                    || ItemCallFrom.toLowerCase() == "ord"
                    || ItemCallFrom.toLowerCase() == "retord"
                    || ItemCallFrom.toLowerCase() == "ql"
                    || ItemCallFrom.toLowerCase() == "trf"
                    || ItemCallFrom.toLowerCase() == "newcart"
                    || ItemCallFrom.toLowerCase() == "movemtr"
                    || ItemCallFrom.toLowerCase() == "mntnance"
                    || ItemCallFrom.toLowerCase() == "as"
                    || ItemCallFrom.toLowerCase() == "receive"
                    || ItemCallFrom.toLowerCase() == "materialstaging"
                    || ItemCallFrom.toLowerCase() == "ps"
                    || CalledPageName.toLowerCase() == "itemmaster") {
                    $.each(response.DDData, function (ValData, ValCount) {
                        var ArrData = ValData.toString().split('[###]');
                        s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
                    });
                }
                else {
                    $.each(response.DDData, function (i, val) {

                        if (i == "1")
                            s += '<option value="' + i + '"> Item (' + val + ')' + '</option>';

                        //if (i == "2" && gblActionName.toLowerCase() != "cartitemlist")
                        //if (i == "2" && '@Model.ControlID' !== 'MOVEMTR' && '@Model.ControlID' == 'transferlist')
                        //if (i == "2" && '@Model.ControlID' !== 'MOVEMTR' && '@Model.ControlID' != 'TRF' && val > 0)
                        if (i == "2" && _narrowSearchForIM.controlId !== 'MOVEMTR' && val > 0)
                            s += '<option value="' + i + '"> Quick List (' + val + ')' + '</option>';

                        if (i == "3") {
                            if (ModelTextTemp != "Kit")
                                s += '<option value="' + i + '"> Kit (' + val + ')' + '</option>';
                        }
                        if (i == "4" && _narrowSearchForIM.controlId !== 'MOVEMTR' && gblActionName.toLowerCase() != "orderlist" && gblActionName.toLowerCase() != "cartitemlist")
                            s += '<option value="' + i + '"> Labor (' + val + ')' + '</option>';
                    });
                }
            }
            var $ItemTypeIM = $("#ItemTypeIM");
            $ItemTypeIM.empty();
            $ItemTypeIM.multiselect('destroy');
            $ItemTypeIM.multiselectfilter('destroy');

            $ItemTypeIM.append(s);
            $ItemTypeIM.multiselect
                (
                    {
                        checkAllText: Check,
                        uncheckAllText: UnCheck,
                        noneSelectedText: ItemType, selectedList: 5,
                        selectedText: function (numChecked, numTotal, checkedItems) {
                            return ItemType + ' ' + numChecked + ' ' + selected;
                        }
                    },
                    {
                        checkAll: function (ui) {
                            $("#ItemTypeCollapseIM").html('');
                            for (var i = 0; i <= ui.target.length - 1; i++) {
                                if ($("#ItemTypeCollapseIM").text().indexOf(ui.target[i].text) == -1) {
                                    $("#ItemTypeCollapseIM").append("<span>" + ui.target[i].text + "</span>");
                                }
                            }
                            $("#ItemTypeCollapseIM").show();
                        }
                    }
            )
                .unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                    if (ui.checked) {
                        if ($("#ItemTypeCollapseIM").text().indexOf(ui.text) == -1) {
                            $("#ItemTypeCollapseIM").append("<span>" + ui.text + "</span>");
                        }
                    }
                    else {
                        if (ui.checked == undefined) {
                            $("#ItemTypeCollapseIM").html('');
                        }
                        else if (!ui.checked) {
                            var text = $("#ItemTypeCollapseIM").html();
                            text = text.replace("<span>" + ui.text + "</span>", '');
                            $("#ItemTypeCollapseIM").html(text);
                        }
                        else
                            $("#ItemTypeCollapseIM").html('');
                    }
                    ItemTypeNarroSearchValue = $.map($(this).multiselect("getChecked"), function (input) {
                        return input.value;
                    })

                    _NarrowSearchSave.objSearch.ItemTypeIM = ItemTypeNarroSearchValue;

                    if ($("#ItemTypeCollapseIM").text().trim() != '')
                        $("#ItemTypeCollapseIM").show();
                    else
                        $("#ItemTypeCollapseIM").hide();


                    if ($("#ItemTypeCollapseIM").find('span').length <= 2) {
                        $("#ItemTypeCollapseIM").scrollTop(0).height(50);
                    }
                    else {
                        $("#ItemTypeCollapseIM").scrollTop(0).height(100);
                    }
                    // clearGlobalIfNotInFocus();
                    if (!isFromNarrowSearchClear) {
                        DoNarrowSearchIM();
                    }                    
                }).multiselectfilter({ label: Filter, placeholder: Enterkeywords });

            _NarrowSearchSave.setControlValue("ItemTypeIM");
        },
        error: function (response) {
            // through errror message
        }
    });


    $("#ItemTypeIM").multiselect


}
function GetNarroHTMLForItemTypeOrd() {

    var ModelTextTemp = modelText;
    if (ModelTextTemp == null)
        ModelTextTemp = "eTurns";

    var qlType = '1';
    if (gblActionName.toLowerCase() === "assetlist" || gblActionName.toLowerCase() === "toollist") {
        qlType = '2';
    }
    else if (gblActionName.toLowerCase() == "inventorycountlist") {
        qlType = '3';
    }

    var ItemCallFrom = '';
    if (_narrowSearchForIM.IMCallFromPageName == 'NewPULL') {
        ItemCallFrom = 'newpull';
        if ($('#NewPullAction') != undefined && $('#NewPullAction').val() != "") {
            if ($('#NewPullAction').val() == "Credit") {
                ItemCallFrom = $('#NewPullAction').val();
            }
            else if ($('#NewPullAction').val() == "CreditMS") {
                ItemCallFrom = $('#NewPullAction').val();
            }
            else if ($('#NewPullAction').val() == "Pull") {
                ItemCallFrom = "newpull";
            }
        }
    }
    else if (_narrowSearchForIM.IMCallFromPageName.toLowerCase() == 'receive'.toLowerCase()) {
        ItemCallFrom = 'receive';
    }
    else if (_narrowSearchForIM.IMCallFromPageName.toLowerCase() == 'NEWCART'.toLowerCase()) {
        ItemCallFrom = 'newcart';
    }
    else if (_narrowSearchForIM.IMCallFromPageName.toLowerCase() == 'Materialstaging'.toLowerCase()) {
        ItemCallFrom = 'materialstaging';
    }
    else if (_narrowSearchForIM.IMCallFromPageName.toLowerCase() == 'ps'.toLowerCase()) {
        ItemCallFrom = 'ps';
    }
    else {
        ItemCallFrom = _narrowSearchForIM.controlId; //'@Model.ControlID';
    }

    var CalledPageName = _narrowSearchForIM.pageName;
    var DataValue = { TableName: _narrowSearchForIM.pageName, TextFieldName: 'ItemType', IsArchived: false, IsDeleted: false, QLType: qlType, ParentID: modelID, ItemModelCallFrom: ItemCallFrom };
    if (ItemCallFrom.toLowerCase() == "movemtr") {
        DataValue = { TableName: _narrowSearchForIM.pageName, TextFieldName: 'ItemType', IsArchived: false, IsDeleted: false, QLType: qlType, ParentID: modelID, ItemModelCallFrom: ItemCallFrom, moveType: $('#ddlMoveType').val() };
    }
    //alert('9');
    $.ajax({
        'url': '/Master/GetNarrowDDDataIMItemType',
        data: DataValue,
        success: function (response) {
            var s = '';
            if (response.DDData != null) {
                if (ItemCallFrom.toLowerCase() == "newpull"
                    || ItemCallFrom.toLowerCase() == "credit"
                    || ItemCallFrom.toLowerCase() == "creditms"
                    || ItemCallFrom.toLowerCase() == "rq"
                    || ItemCallFrom.toLowerCase() == "ord"
                    || ItemCallFrom.toLowerCase() == "retord"
                    || ItemCallFrom.toLowerCase() == "ql"
                    || ItemCallFrom.toLowerCase() == "trf"
                    || ItemCallFrom.toLowerCase() == "newcart"
                    || ItemCallFrom.toLowerCase() == "movemtr"
                    || ItemCallFrom.toLowerCase() == "mntnance"
                    || ItemCallFrom.toLowerCase() == "as"
                    || ItemCallFrom.toLowerCase() == "receive"
                    || ItemCallFrom.toLowerCase() == "materialstaging"
                    || ItemCallFrom.toLowerCase() == "ps"
                    || CalledPageName.toLowerCase() == "itemmaster") {
                    $.each(response.DDData, function (ValData, ValCount) {
                        var ArrData = ValData.toString().split('[###]');
                        s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
                    });
                }
                else {
                    $.each(response.DDData, function (i, val) {

                        if (i == "1")
                            s += '<option value="' + i + '"> Item (' + val + ')' + '</option>';

                        //if (i == "2" && gblActionName.toLowerCase() != "cartitemlist")
                        //if (i == "2" && '@Model.ControlID' !== 'MOVEMTR' && '@Model.ControlID' == 'transferlist')
                        //if (i == "2" && '@Model.ControlID' !== 'MOVEMTR' && '@Model.ControlID' != 'TRF' && val > 0)
                        if (i == "2" && _narrowSearchForIM.controlId !== 'MOVEMTR' && val > 0)
                            s += '<option value="' + i + '"> Quick List (' + val + ')' + '</option>';

                        if (i == "3") {
                            if (ModelTextTemp != "Kit")
                                s += '<option value="' + i + '"> Kit (' + val + ')' + '</option>';
                        }
                        if (i == "4" && _narrowSearchForIM.controlId !== 'MOVEMTR' && gblActionName.toLowerCase() != "orderlist" && gblActionName.toLowerCase() != "cartitemlist")
                            s += '<option value="' + i + '"> Labor (' + val + ')' + '</option>';
                    });
                }
            }
            var $ItemTypeIM = $("#ItemTypeOrd");
            $ItemTypeIM.empty();
            $ItemTypeIM.multiselect('destroy');
            $ItemTypeIM.multiselectfilter('destroy');

            $ItemTypeIM.append(s);
            $ItemTypeIM.multiselect
                (
                    {
                        checkAllText: Check,
                        uncheckAllText: UnCheck,
                        noneSelectedText: ItemType, selectedList: 5,
                        selectedText: function (numChecked, numTotal, checkedItems) {
                            return ItemType + ' ' + numChecked + ' ' + selected;
                        }
                    },
                    {
                        checkAll: function (ui) {
                            $("#ItemTypeCollapseOrd").html('');
                            for (var i = 0; i <= ui.target.length - 1; i++) {
                                if ($("#ItemTypeCollapseOrd").text().indexOf(ui.target[i].text) == -1) {
                                    $("#ItemTypeCollapseOrd").append("<span>" + ui.target[i].text + "</span>");
                                }
                            }
                            $("#ItemTypeCollapseOrd").show();
                        }
                    }
                )
                .unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                    if (ui.checked) {
                        if ($("#ItemTypeCollapseOrd").text().indexOf(ui.text) == -1) {
                            $("#ItemTypeCollapseOrd").append("<span>" + ui.text + "</span>");
                        }
                    }
                    else {
                        if (ui.checked == undefined) {
                            $("#ItemTypeCollapseOrd").html('');
                        }
                        else if (!ui.checked) {
                            var text = $("#ItemTypeCollapseOrd").html();
                            text = text.replace("<span>" + ui.text + "</span>", '');
                            $("#ItemTypeCollapseOrd").html(text);
                        }
                        else
                            $("#ItemTypeCollapseOrd").html('');
                    }
                    ItemTypeNarroSearchValue = $.map($(this).multiselect("getChecked"), function (input) {
                        return input.value;
                    })

                    _NarrowSearchSave.objSearch.ItemTypeIM = ItemTypeNarroSearchValue;

                    if ($("#ItemTypeCollapseOrd").text().trim() != '')
                        $("#ItemTypeCollapseOrd").show();
                    else
                        $("#ItemTypeCollapseOrd").hide();


                    if ($("#ItemTypeCollapseOrd").find('span').length <= 2) {
                        $("#ItemTypeCollapseOrd").scrollTop(0).height(50);
                    }
                    else {
                        $("#ItemTypeCollapseOrd").scrollTop(0).height(100);
                    }
                    // clearGlobalIfNotInFocus();
                    if (!isFromNarrowSearchClear) {
                        DoNarrowSearchIM();
                    }
                }).multiselectfilter({ label: Filter, placeholder: Enterkeywords });

            _NarrowSearchSave.setControlValue("ItemTypeOrd");
        },
        error: function (response) {
            // through errror message
        }
    });
}

$(document).ready(function () {
    //$(".handle").each(function () {
    //    $(this).find("img").attr("src", "../../Content/images/Drage.png");
    //});
    //NSForItemModel_ExecuteOnDocReady();
    //if (_narrowSearchForIM.actionName == 'NewPull') {
    //    $("#NarrowUL").sortable({
    //        handle: "> .handle",
    //        stop: function (event, ui) {
    //            var linkOrderData = $("#NarrowUL").sortable("toArray", { attribute: 'attrsortOrder' });

    //            $.ajax({
    //                "type": "POST",
    //                "url": '/Master/SaveGridState',
    //                "data": { Data: JSON.stringify(linkOrderData), ListName: _narrowSearchForIM.actionName + "_NarrowSearch" },
    //                "dataType": "json",
    //                "cache": false,
    //                "async": false,
    //                "success": function (json1) {
    //                }
    //            });
    //        }
    //    });

    //    //var orderArray = '3,2,4,5,6,7,8,9,10,1,12,11,103,101,102,104,';
    //    //var listArray = $('#nssortable li');
    //    //for (var i = 0; i < orderArray.length; i++) {
    //    //    $('#nssortable').append(listArray[orderArray[i]-1]);
    //    //}
    //    $.ajax({
    //        "type": "POST",
    //        "url": '/Master/LoadGridState',
    //        "data": { ListName: _narrowSearchForIM.actionName + "_NarrowSearch" },
    //        "dataType": "json",
    //        "cache": false,
    //        "async": false,
    //        "success": function (json1) {

    //            if (json1.jsonData != null && json1.jsonData != '') {

    //                var sorted = JSON.parse(json1.jsonData);
    //                sorted = sorted.reverse();

    //                sorted.forEach(function (id) {
    //                    $("[attrsortOrder=" + parseInt(id) + "]").prependTo("#NarrowUL");
    //                });
    //            }
    //        }
    //    });
    //}
});

function NSForItemModel_ExecuteOnDocReady() {
    //--------------------------------------------------------------------------------
    //
    //alert("This is test");


    //alert("a: " + actionName + " c: " + controllerName);
    
    var _IsArchived = false;
    var _IsDeleted = false;
    var ShowStagingLocations = false;


    if (_narrowSearchForIM.actionName == "NewPull"
        || _narrowSearchForIM.actionName == "Credit"
        || _narrowSearchForIM.actionName == "CreditMS"
        || (_narrowSearchForIM.controllerName == "WorkOrder" && _narrowSearchForIM.actionName == "LoadItemMasterModel")) {
        GetPullNarrowSearchDataIM(_narrowSearchForIM.pageName, _IsArchived, _IsDeleted);
    }

    //alert('4');
    if (_narrowSearchForIM.actionName.toLowerCase() == "loaditemmastermodel"
        && _narrowSearchForIM.pageName.toLowerCase() == "itemmaster"
        && _narrowSearchForIM.controllerName.toLowerCase() == "order" ) {
        GetNarrowDDDataOrd_Common(_narrowSearchForIM.pageName, _IsArchived, _IsDeleted);
    }
    else {
        GetNarrowDDDataIM_Common(_narrowSearchForIM.pageName, _IsArchived, _IsDeleted);
    }

    //if ('@string.IsNullOrEmpty(Model.Text)' == '@Boolean.FalseString') {
    if (modelText != null && modelText.trim() != '') {
        if (modelText.toLowerCase() == "inventory count") {
            GetItemLocationsForCount(ShowStagingLocations, false);
        }
    }

    CallNarrowfunctionsIM();

    //$('a.downarrow').click(function (e) {
    $('a.downarrow').off('click').on('click', function (e) {
        e.preventDefault();
        $(this).closest('.accordion').find('.dropcontent').slideToggle();
    });

    //--------------------------------------------------------------------------------
    //
    //$('#ExpandNarrowSearchIM').click(function (e) {
    $('#ExpandNarrowSearchIM').off('click').on('click', function (e) {
        ExpandNarrowSearchIM();
    });
    //$('#CollapseNarrowSearchIM').click(function (e) {
    $('#CollapseNarrowSearchIM').off('click').on('click', function (e) {

        CollapseNarrowSearchIM();
    });
    var NarrowSearchStateIM = getCookieIM('NarrowSearchStateIM');

    if (NarrowSearchStateIM == 'Expanded') {
        CollapseNarrowSearchIM();
    }
    else {
        ExpandNarrowSearchIM();
    }

    //alert('type 8');
    if (_narrowSearchForIM.actionName.toLowerCase() == "loaditemmastermodel"
        && _narrowSearchForIM.pageName.toLowerCase() == "itemmaster"
        && _narrowSearchForIM.controllerName.toLowerCase() == "order") {
        GetNarroHTMLForItemTypeOrd();
    }
    else {
        GetNarroHTMLForItemTypeIM();
    }
    $("div#ItemModelTemp").find("div.userHead:first").find("div.BtnBlock a.clsactionSelectAll").click(function () {

        $("#myDataTable").find("tbody tr").removeClass("row_selected").addClass("row_selected");

        $("#ItemModeDataTable").find("tbody tr").removeClass("row_selected").addClass("row_selected");

        $("#ItemModeDataTable tbody tr").each(function () {
            if ($(this).find("input#btnLoad").length > 0) {
                $(this).removeClass("row_selected");
            }
        });
        $(this).next("a.clsactionDeSelectAll").css('display', '');
        $(this).css('display', 'none');
        ShowHideCartCreateButton();
        ShowHideCartDeleteButton();
    });
    $("div#ItemModelTemp").find("div.userHead:first").find("div.BtnBlock a.clsactionDeSelectAll").click(function () {

        $("#myDataTable").find("tbody tr").removeClass("row_selected");
        $("#ItemModeDataTable").find("tbody tr").removeClass("row_selected");
        $(this).prev("a.clsactionSelectAll").css('display', '');
        $(this).css('display', 'none');
        ShowHideCartCreateButton();
        ShowHideCartDeleteButton();
    });
}


function ExpandNarrowSearchIM() {

    if ($('#ItemModelTemp .IteamBlock').length > 0) {

        var w = $('#ItemModelTemp .IteamBlock').css("width");
        $('#ItemModelTemp .IteamBlock').show();
        $('#ItemModelTemp .IteamBlock').stop().animate({
            width: "18%"
        }, 0, function () {
            $('#ItemModelTemp .userContent').css({ "width": "80.5%", "margin": "0" });
            $('#ItemModeDataTable_length').css({ "left": "0px" });
            $('#ItemModeDataTable_paginate').css({ "left": "145px" });
            $('#ItemModelTemp .leftopenContent').css({ "display": "none" });
            setCookieIM('NarrowSearchStateIM', 'Collapsed');
        });
    }
    else if ($('.userListBlock .IteamBlock').length > 0) {

        var w = $('.userListBlock .IteamBlock').css("width");
        $('.userListBlock .IteamBlock').show();
        $('.userListBlock .IteamBlock').stop().animate({
            width: "18%"
        }, 0, function () {
            $('.userListBlock .userContent').css({ "width": "80.5%", "margin": "0" });
            $('#ItemModeDataTable_length').css({ "left": "0px" });
            $('#ItemModeDataTable_paginate').css({ "left": "145px" });
            $('.userListBlock .leftopenContent').css({ "display": "none" });
            setCookieIM('NarrowSearchStateIM', 'Collapsed');
        });
    }
    else if ($('#CtabNew .IteamBlock').length > 0) {
        var w = $('#CtabNew .IteamBlock').css("width");
        $('#CtabNew .IteamBlock').show();
        $('#CtabNew .IteamBlock').stop().animate({
            width: "18%"
        }, 0, function () {
            $('#CtabNew .userContent').css({ "width": "80.5%", "margin": "0" });
            $('#ItemModeDataTable_length').css({ "left": "0px" });
            $('#ItemModeDataTable_paginate').css({ "left": "145px" });
            $('#CtabNew .leftopenContent').css({ "display": "none" });
            setCookieIM('NarrowSearchStateIM', 'Collapsed');
        });
    }
    else if ($('#Ctab .IteamBlock').length > 0) {

        var w = $('#Ctab .IteamBlock').css("width");
        $('#Ctab .IteamBlock').show();
        $('#Ctab .IteamBlock').stop().animate({
            width: "18%"
        }, 0, function () {
            $('#Ctab .userContent').css({ "width": "80.5%", "margin": "0" });
            $('#ItemModeDataTable_length').css({ "left": "0px" });
            $('#ItemModeDataTable_paginate').css({ "left": "145px" });
            $('#Ctab .leftopenContent').css({ "display": "none" });
            setCookieIM('NarrowSearchStateIM', 'Collapsed');
        });
    }


    else if ($('#ItemModel .IteamBlock').length > 0) {
        var w = $('#ItemModel .IteamBlock').css("width");
        $('#ItemModel .IteamBlock').show();
        $('#ItemModel .IteamBlock').stop().animate({
            width: "18%"
        }, 0, function () {
            $('#ItemModel .userContent').css({ "width": "80.5%", "margin": "0" });
            $('#ItemModeDataTable_length').css({ "left": "0px" });
            $('#ItemModeDataTable_paginate').css({ "left": "145px" });
            $('#ItemModel .leftopenContent').css({ "display": "none" });
            setCookieIM('NarrowSearchStateIM', 'Collapsed');
        });
    }

    if (typeof (gblActionName) !== "undefined" && gblActionName !== undefined && gblActionName != null && gblActionName.toLowerCase() == 'newpull') {
        $('.dataTables_length').attr('style', 'left:0;top:-90px !important');
        $('.dataTables_paginate').attr('style', 'left: 145px;top:-80px !important');
    }
}



function CollapseNarrowSearchIM() {

    if ($('#ItemModelTemp .IteamBlock').length > 0) {

        $('#ItemModelTemp .IteamBlock').stop().animate({
            width: "0%"
        }, 0, function () {
            $('#ItemModelTemp .IteamBlock').hide();

            $('#ItemModelTemp .userContent').css({ "width": "98.5%", margin: "0 0.4% 1%" });
            var Left = $('.viewBlock').css("width");
            $('#ItemModeDataTable_length').css({ "left": Left });
            var LeftW = 145 + parseInt(Left);
            $('#ItemModeDataTable_paginate').css({ "left": LeftW + 'px' });
            if (typeof (oTableItemModel) != "undefined") {
                oTableItemModel.fnAdjustColumnSizing();
            }


            $('#ItemModelTemp .leftopenContent').css({ "display": "" });

            setCookie('NarrowSearchStateIM', 'Expanded');
        });
    }
    else if ($('#CtabNew .IteamBlock').length > 0) {

        $('#CtabNew .IteamBlock').stop().animate({
            width: "0%"
        }, 0, function () {
            $('#CtabNew .IteamBlock').hide();
            $('#CtabNew .userContent').css({ "width": "98.5%", margin: "0 0.4% 1%" });
            var Left = $('.viewBlock').css("width");
            $('#ItemModeDataTable_length').css({ "left": Left });
            var LeftW = 145 + parseInt(Left);
            $('#ItemModeDataTable_paginate').css({ "left": LeftW + 'px' });
            if (typeof (oTableItemModel) != "undefined") {
                oTableItemModel.fnAdjustColumnSizing();
            }
            $('#CtabNew .leftopenContent').css({ "display": "" });
            setCookie('NarrowSearchStateIM', 'Expanded');
        });
    }
    else if ($('.userListBlock .IteamBlock').length > 0) {

        $('.userListBlock .IteamBlock').stop().animate({
            width: "0%"
        }, 0, function () {
            $('.userListBlock .IteamBlock').hide();
            $('.userListBlock .userContent').css({ "width": "98.5%", margin: "0 0.4% 1%" });
            var Left = $('.viewBlock').css("width");
            $('#ItemModeDataTable_length').css({ "left": Left });
            var LeftW = 145 + parseInt(Left);
            $('#ItemModeDataTable_paginate').css({ "left": LeftW + 'px' });
            if (typeof (oTableItemModel) != "undefined") {
                oTableItemModel.fnAdjustColumnSizing();
            }
            $('.userListBlock .leftopenContent').css({ "display": "" });
            setCookie('NarrowSearchStateIM', 'Expanded');
        });
    }
    else if ($('#Ctab .IteamBlock').length > 0) {

        $('#Ctab .IteamBlock').stop().animate({
            width: "0%"
        }, 0, function () {
            $('#Ctab .IteamBlock').hide();

            $('#Ctab .userContent').css({ "width": "98.5%", margin: "0 0.4% 1%" });
            var Left = $('.viewBlock').css("width");
            $('#ItemModeDataTable_length').css({ "left": Left });
            var LeftW = 145 + parseInt(Left);
            $('#ItemModeDataTable_paginate').css({ "left": LeftW + 'px' });
            if (typeof (oTableItemModel) != "undefined") {
                oTableItemModel.fnAdjustColumnSizing();
            }


            $('#Ctab .leftopenContent').css({ "display": "" });

            setCookie('NarrowSearchStateIM', 'Expanded');
        });
    }
    else if ($('#ItemModel .IteamBlock').length > 0) {
        $('#ItemModel .IteamBlock').stop().animate({
            width: "0%"
        }, 0, function () {
            $('#ItemModel .IteamBlock').hide();
            $('#ItemModel .userContent').css({ "width": "98.5%", margin: "0 0.4% 1%" });
            var Left = $('.viewBlock').css("width");
            $('#ItemModeDataTable_length').css({ "left": Left });
            var LeftW = 145 + parseInt(Left);
            $('#ItemModeDataTable_paginate').css({ "left": LeftW + 'px' });
            if (typeof (oTableItemModel) != "undefined") {
                oTableItemModel.fnAdjustColumnSizing();
            }
            $('#ItemModel .leftopenContent').css({ "display": "" });
            setCookie('NarrowSearchStateIM', 'Expanded');
        });
    }

    if (typeof (gblActionName) !== "undefined" && gblActionName !== undefined && gblActionName != null && gblActionName.toLowerCase() == 'newpull') {
        $('.dataTables_length').attr('style', 'left:0;top:-82px !important');
        $('.dataTables_paginate').attr('style', 'left: 145px;top:-72px !important');
    }
}



function getCookieIM(name) {
    var arg = name + "=";
    var alen = arg.length;
    var clen = document.cookie.length;
    var i = 0;
    while (i < clen) {
        var j = i + alen;
        if (document.cookie.substring(i, j) == arg) {
            return getCookieValIM(j);
        }
        i = document.cookie.indexOf(" ", i) + 1;
        if (i == 0) break;
    }
    return null;
}

function getCookieValIM(offset) {
    var endstr = document.cookie.indexOf(";", offset);
    if (endstr == -1) { endstr = document.cookie.length; }
    return unescape(document.cookie.substring(offset, endstr));
}

function setCookieIM(name, value, days) {
    if (typeof days != "undefined") { //if set persistent cookie
        var expireDate = new Date();
        expireDate.setDate(expireDate.getDate() + days);
        document.cookie = name + "=" + value + "; path=/; expires=" + expireDate.toGMTString() + " ;SameSite=Strict;";
    }
    else //else if this is a session only cookie
        document.cookie = name + "=" + value + "; path=/ ;SameSite=Strict;";
}

/* CLEAR NARROW SEARCH - START */
function clearNarrowSearchFilterIM() {

    if (typeof ($("#ItemLocationsIC").multiselect("getChecked").length) != undefined && $("#ItemLocationsIC").multiselect("getChecked").length > 0) {
        $("#ItemLocationsIC").multiselect("uncheckAll");
        $("#ItemLocationsICCollapseIM").html('');
    }
    if (typeof ($("#UserCreatedIM").multiselect("getChecked").length) != undefined && $("#UserCreatedIM").multiselect("getChecked").length > 0) {
        $("#UserCreatedIM").multiselect("uncheckAll");
        $("#UserCreatedCollapseIM").html('');
    }
    if (typeof ($("#UserCreatedIM").multiselect("getChecked").length) != undefined && $("#UserCreatedIM").multiselect("getChecked").length > 0) {
        $("#UserCreatedIM").multiselect("uncheckAll");
        $("#UserCreatedCollapseIM").html('');
    }
    if (typeof ($("#UserUpdatedIM").multiselect("getChecked").length) != undefined && $("#UserUpdatedIM").multiselect("getChecked").length > 0) {
        $("#UserUpdatedIM").multiselect("uncheckAll");
        $("#UserUpdatedCollapseIM").html('');
    }

    $("select[name='udflist_Item']").each(function (index) {
        if (typeof ($(this).multiselect("getChecked").length) != undefined && $(this).multiselect("getChecked").length > 0) {
            var UDFUniqueID = this.getAttribute('UID');
            $(this).multiselect("uncheckAll");
            $('#' + UDFUniqueID + 'Collapse_Item').html('');
        }
    });

    if (typeof ($("#PullSupplierIM").multiselect("getChecked").length) != undefined && $("#PullSupplierIM").multiselect("getChecked").length > 0) {
        $("#PullSupplierIM").multiselect("uncheckAll");
        $("#PullSupplierCollapseIM").html('');
    }

    if (typeof ($("#StageLocationHeaderIM").multiselect("getChecked").length) != undefined && $("#StageLocationHeaderIM").multiselect("getChecked").length > 0) {
        $("#StageLocationHeaderIM").multiselect("uncheckAll");
        $("#StageLocationHeaderCollapseIM").html('');
        IsStagingLocationOnly = false;
    }

    if (typeof ($("#StageLocationIM").multiselect("getChecked").length) != undefined && $("#StageLocationIM").multiselect("getChecked").length > 0) {
        $("#StageLocationIM").multiselect("uncheckAll");
        $("#StageLocationCollapseIM").html('');
        IsStagingLocationOnly = false;
    }

    if (typeof ($("#ManufacturerIM").multiselect("getChecked").length) != undefined && $("#ManufacturerIM").multiselect("getChecked").length > 0) {
        $("#ManufacturerIM").multiselect("uncheckAll");
        $("#ManufacturerCollapseIM").html('');
    }

    if (typeof ($("#PullCategoryIM").multiselect("getChecked").length) != undefined && $("#PullCategoryIM").multiselect("getChecked").length > 0) {
        $("#PullCategoryIM").multiselect("uncheckAll");
        $("#PullCategoryCollapseIM").html('');
    }

    if ($('#DateCFromIM').val() != '') $('#DateCFromIM').val('');
    if ($('#DateCToIM').val() != '') $('#DateCToIM').val('');
    if ($('#DateUFromIM').val() != '') $('#DateUFromIM').val('');
    if ($('#DateUToIM').val() != '') $('#DateUToIM').val('');

    if ($('#PullCost') != undefined) {
        $('#PullCost').val('0_-1');
    }

    if ($('#StockStatus') != undefined) {
        $('#StockStatus').val('0');
    }

    if (typeof ($("#ItemTypeIM").multiselect("getChecked").length) != undefined && $("#ItemTypeIM").multiselect("getChecked").length > 0) {
        $("#ItemTypeIM").multiselect("uncheckAll");
        $("#ItemTypeCollapseIM").html('');
    }

}
/* CLEAR NARROW SEARCH - END */

/* CLEAR GLOBAL FILTER IF NOT IN FOCUS - START */
function clearGlobalIMIfNotInFocus() {
    if ($(document.activeElement).attr('id') != 'ItemModel_filter')
        $("#ItemModel_filter").val('');
}
    /* CLEAR GLOBAL FILTER IF NOT IN FOCUS - END */