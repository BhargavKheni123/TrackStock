var _NarrowSearchLayout = (function ($) {
    var self = {};
    self.pageName = null;
    self.listName = null;
    self.companyID = null;
    self.roomID = null;
    self.enterpriseID = null;

    self.init = function (pageName, listName, companyID, roomID, enterpriseID) {
        self.pageName = pageName;
        self.listName = listName;
        self.companyID = companyID;
        self.roomID = roomID;
        self.enterpriseID = enterpriseID;
    };

    self.saveNarrowSearch = function () {
    }

    return self;
})(jQuery);

$(document).ready(function () {

    $(".handle").each(function () {
        $(this).find("img").attr("src", "../../Content/images/Drage.png");
    });
    GetNarroHTMLForUDFIM(_NarrowSearchLayout.pageName, _NarrowSearchLayout.companyID, _NarrowSearchLayout.roomID, _IsArchived, _IsDeleted); 
    CallNarrowfunctions();

    if (_NarrowSearchLayout.pageName == 'ItemMaster' || _NarrowSearchLayout.pageName == 'ItemBinMaster' || _NarrowSearchLayout.pageName == 'RequisitionMaster' ||
        _NarrowSearchLayout.pageName == 'WorkOrder' || _NarrowSearchLayout.pageName == 'ToolMaster' || _NarrowSearchLayout.pageName == 'ToolMasterNew' ||
        _NarrowSearchLayout.pageName == 'KitToolMaster' || _NarrowSearchLayout.pageName == "AssetMaster" || _NarrowSearchLayout.pageName == 'PullMaster' ||
        _NarrowSearchLayout.pageName == "KitToolMasterNew" || _NarrowSearchLayout.pageName == "CartItemList" || _NarrowSearchLayout.pageName == "QuoteMaster" ) {
        $("#nssortable").sortable({
            handle: "> .handle",
            stop: function (event, ui) {
                var linkOrderData = $("#nssortable").sortable("toArray", { attribute: 'attrsortOrder' });

                $.ajax({
                    "type": "POST",
                    "url": '/Master/SaveGridState',
                    "data": { Data: JSON.stringify(linkOrderData), ListName: _NarrowSearchLayout.pageName + "_NarrowSearch" },
                    "dataType": "json",
                    "cache": false,
                    "async": false,
                    "success": function (json1) {
                    }
                });
            }
        });

        $.ajax({
            "type": "POST",
            "url": '/Master/LoadGridState',
            "data": { ListName: _NarrowSearchLayout.pageName + "_NarrowSearch" },
            "dataType": "json",
            "cache": false,
            "async": false,
            "success": function (json1) {

                if (json1.jsonData != null && json1.jsonData != '') {

                    var sorted = JSON.parse(json1.jsonData);
                    sorted = sorted.reverse();

                    sorted.forEach(function (id) {
                        $("#nssortable [attrsortOrder=" + parseInt(id) + "]").prependTo("#nssortable");
                    });
                }
            }
        });
    }
    if (_NarrowSearchLayout.pageName == "AssetMaster") {
        var _IsArchived = false;
        var _IsDeleted = false;

        if (typeof ($('#IsArchivedRecords')) != undefined)
            _IsArchived = $('#IsArchivedRecords').is(':checked');

        if (typeof ($('#IsDeletedRecords')) != undefined)
            _IsDeleted = $('#IsDeletedRecords').is(':checked');

        GetAssetNarrowSearchData('AssetMaster', _IsArchived, _IsDeleted);
    }

    if (_NarrowSearchLayout.pageName == 'ToolMaster' || _NarrowSearchLayout.pageName == 'KitToolMaster') {
        var _IsArchived = false;
        var _IsDeleted = false;
        var _TabName = 'ToolList';

        if (typeof ($('#IsArchivedRecords')) != undefined)
            _IsArchived = $('#IsArchivedRecords').is(':checked');

        if (typeof ($('#IsDeletedRecords')) != undefined)
            _IsDeleted = $('#IsDeletedRecords').is(':checked');

        if (_NarrowSearchLayout.pageName == 'KitToolMaster')
            _TabName = 'KitList';

        GetToolsNarrowSearchData(_IsArchived, _IsDeleted, _NarrowSearchLayout.pageName, _TabName);
    }
    if (_NarrowSearchLayout.pageName == 'ToolMasterNew' || _NarrowSearchLayout.pageName == 'KitToolMasterNew') {
        var _IsArchived = false;
        var _IsDeleted = false;
        var _TabName = 'ToolList';

        if (typeof ($('#IsArchivedRecords')) != undefined)
            _IsArchived = $('#IsArchivedRecords').is(':checked');

        if (typeof ($('#IsDeletedRecords')) != undefined)
            _IsDeleted = $('#IsDeletedRecords').is(':checked');

        if (_NarrowSearchLayout.pageName == 'KitToolMasterNew')
            _TabName = 'KitList';

        GetToolsNarrowSearchDataNew(_IsArchived, _IsDeleted, _NarrowSearchLayout.pageName, _TabName);
    }
    if (_NarrowSearchLayout.pageName == 'PullMaster' || _NarrowSearchLayout.pageName == 'ItemMaster' || _NarrowSearchLayout.pageName == 'BOMItemMaster' || _NarrowSearchLayout.pageName == 'ItemBinMaster') {
        var _IsArchived = false;
        var _IsDeleted = false;

        if (typeof ($('#IsArchivedRecords')) != undefined)
            _IsArchived = $('#IsArchivedRecords').is(':checked');

        if (typeof ($('#IsDeletedRecords')) != undefined)
            _IsDeleted = $('#IsDeletedRecords').is(':checked');
        
        GetPullNarrowSearchData(_NarrowSearchLayout.pageName, _IsArchived, _IsDeleted);
    }

    if (_NarrowSearchLayout.pageName == 'CompanyMaster') {
        var _IsArchived = false;
        var _IsDeleted = false;

        if (typeof ($('#IsArchivedRecords')) != undefined)
            _IsArchived = $('#IsArchivedRecords').is(':checked');

        if (typeof ($('#IsDeletedRecords')) != undefined)
            _IsDeleted = $('#IsDeletedRecords').is(':checked');

        GetCompanyMasterNarrowSearchData(_NarrowSearchLayout.pageName, _IsArchived, _IsDeleted);
    }

    if (_NarrowSearchLayout.pageName == 'Room') {
        var _IsArchived = false;
        var _IsDeleted = false;

        if (typeof ($('#IsArchivedRecords')) != undefined)
            _IsArchived = $('#IsArchivedRecords').is(':checked');

        if (typeof ($('#IsDeletedRecords')) != undefined)
            _IsDeleted = $('#IsDeletedRecords').is(':checked');

        GetRoomMasterNarrowSearchData(_NarrowSearchLayout.pageName, _IsArchived, _IsDeleted);
    }

    if (_NarrowSearchLayout.pageName == 'ItemMaster' || _NarrowSearchLayout.pageName == 'ItemBinMaster') {
        RefreshInventoryClassification();
    }

    if (_NarrowSearchLayout.pageName == 'RequisitionMaster') {        
        if (window.location.hash.toLowerCase() == "#list") {
            _NarrowSearchSave.currentListName = "RequisitionMaster";
        }
        else if (window.location.hash.toLowerCase() == "#unsubmitted") {
            _NarrowSearchSave.currentListName = "RequisitionMasterUnsubmitted";
        }
        else if (window.location.hash.toLowerCase() == "#submitted") {
            _NarrowSearchSave.currentListName = "RequisitionMasterSubmitted";
        }
        else if (window.location.hash.toLowerCase() == "#approved") {
            _NarrowSearchSave.currentListName = "RequisitionMasterApproved";
        }
        else if (window.location.hash.toLowerCase() == "#closed") {
            _NarrowSearchSave.currentListName = "RequisitionMasterClosed"
        }
        _NarrowSearchSave.loadNarrowSearch();
        setTimeout(function () {
            CallReqNarrowFunctions();
        }, 200);
    }

    if (_NarrowSearchLayout.pageName == 'WorkOrder') {
        CallWONarrowFunctions();
    }

    if (_NarrowSearchLayout.pageName == 'OrderMaster') {
        var _IsArchived = false;
        var _IsDeleted = false;

        if (typeof ($('#IsArchivedRecords')) != undefined)
            _IsArchived = $('#IsArchivedRecords').is(':checked');

        if (typeof ($('#IsDeletedRecords')) != undefined)
            _IsDeleted = $('#IsDeletedRecords').is(':checked');

        GetOrderNarrowSearchData(_NarrowSearchLayout.pageName, _IsArchived, _IsDeleted);
    }
    if (_NarrowSearchLayout.pageName == 'ReceiveMaster') {
        var _IsArchived = false;
        var _IsDeleted = false;

        if (typeof ($('#IsArchivedRecords')) != undefined)
            _IsArchived = $('#IsArchivedRecords').is(':checked');

        if (typeof ($('#IsDeletedRecords')) != undefined)
            _IsDeleted = $('#IsDeletedRecords').is(':checked');

        GetReceiveNarrowSearchData(_NarrowSearchLayout.pageName, _IsArchived, _IsDeleted);
    }
    if (_NarrowSearchLayout.pageName == 'MaterialStaging') {

        RefressFilterMS("MaterialStaging", resStagingLocation);
    }
    if (_NarrowSearchLayout.pageName == 'InventoryCountList') {
        RefressFilterICount(_NarrowSearchLayout.pageName, counttyperesourcename, countstatusresourcename);
    }
    if (_NarrowSearchLayout.pageName == 'NotificationMasterList') {

        RefressFilterNotification(_NarrowSearchLayout.pageName);
    }
    if (_NarrowSearchLayout.pageName == 'CartItem' || _NarrowSearchLayout.pageName == 'CartItemList') {

        RefressFilterCart(_NarrowSearchLayout.pageName);
    }
    if (_NarrowSearchLayout.pageName == 'KitMaster') {
        var _IsArchived = false;
        var _IsDeleted = false;

        if (typeof ($('#IsArchivedRecords')) != undefined)
            _IsArchived = $('#IsArchivedRecords').is(':checked');

        if (typeof ($('#IsDeletedRecords')) != undefined)
            _IsDeleted = $('#IsDeletedRecords').is(':checked');

        GetKitMasterNarrowSearchData(_NarrowSearchLayout.pageName, _IsArchived, _IsDeleted);
    }

    if (_NarrowSearchLayout.pageName == 'KitToolMasterNew') {
        var _IsArchived = false;
        var _IsDeleted = false;

        if (typeof ($('#IsArchivedRecords')) != undefined)
            _IsArchived = $('#IsArchivedRecords').is(':checked');

        if (typeof ($('#IsDeletedRecords')) != undefined)
            _IsDeleted = $('#IsDeletedRecords').is(':checked');

        GetKitMasterNarrowSearchDataNew(_NarrowSearchLayout.pageName, _IsArchived, _IsDeleted);
    }
    if (_NarrowSearchLayout.pageName == 'BarcodeMaster') {
    }

    if (_NarrowSearchLayout.pageName == 'QuoteMaster') {
        CallQuoteMasterNarrowFunctions();
    }

    $('a.downarrow').click(function (e) {
        e.preventDefault();
        $(this).closest('.accordion').find('.dropcontent').slideToggle();
    });

});
function GetModuleWiseCreatedData(tableName, companyID, roomID, currentModule) {

    var Moduleguid = $("body").find("input#hdnModuleGuid").val();
    var _IsArchived = false;
    var _IsDeleted = false;

    if (typeof ($('#IsArchivedRecords')) != undefined)
        _IsArchived = $('#IsArchivedRecords').is(':checked');

    if (typeof ($('#IsDeletedRecords')) != undefined)
        _IsDeleted = $('#IsDeletedRecords').is(':checked');
    $.ajax({
        'url': '/Master/GetNarrowDDDataForBarcode',
        data: { TableName: tableName, TextFieldName: 'CreatedBy', IsArchived: _IsArchived, IsDeleted: _IsDeleted, Moduleguid: Moduleguid },
        success: function (response) {
            var s = '';

            if (response.DDData != null) {
                $.each(response.DDData, function (i, val) {
                    var ArrData = i.toString().split('[###]');
                    s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + val + ')' + '</option>';
                });
            }

            $("#UserCreated").empty();
            $("#UserCreated").multiselect('destroy');
            $("#UserCreated").multiselectfilter('destroy');
            $("#UserCreated").append(s);
            $("#UserCreated").multiselect
                (
                    {
                        noneSelectedText: UserCreatedBy, selectedList: 5,
                        selectedText: function (numChecked, numTotal, checkedItems) {
                            return CreatedBy + ' ' + numChecked + ' ' + selected;
                        }
                    },
                    {
                        checkAll: function (ui) {
                            $("#UserCreatedCollapse").html('');
                            for (var i = 0; i <= ui.target.length - 1; i++) {
                                if ($("#UserCreatedCollapse").text().indexOf(ui.target[i].text) == -1) {
                                    $("#UserCreatedCollapse").append("<span>" + ui.target[i].text + "</span>");
                                }
                            }
                            $("#UserCreatedCollapse").show();
                        }
                    }
                )
                .unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                    if (ui.checked) {
                        if ($("#UserCreatedCollapse").text().indexOf(ui.text) == -1) {
                            $("#UserCreatedCollapse").append("<span>" + ui.text + "</span>");
                        }
                    }
                    else {
                        if (ui.checked == undefined) {
                            $("#UserCreatedCollapse").html('');
                        }
                        else if (!ui.checked) {
                            var text = $("#UserCreatedCollapse").html();
                            text = text.replace("<span>" + ui.text + "</span>", '');
                            $("#UserCreatedCollapse").html(text);
                        }
                        else
                            $("#UserCreatedCollapse").html('');
                    }
                    UserCreatedNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                        return input.value;
                    })

                    _NarrowSearchSave.objSearch.UserCreated = UserCreatedNarroValues;

                    if ($("#UserCreatedCollapse").text().trim() != '')
                        $("#UserCreatedCollapse").show();
                    else
                        $("#UserCreatedCollapse").hide();

                    if ($("#UserCreatedCollapse").find('span').length <= 2) {
                        $("#UserCreatedCollapse").scrollTop(0).height(50);
                    }
                    else {
                        $("#UserCreatedCollapse").scrollTop(0).height(100);
                    }
                    clearGlobalIfNotInFocus();
                    DoNarrowSearch();
                }).multiselectfilter();

            _NarrowSearchSave.setControlValue("UserCreated");

        },
        error: function (response) {
        }
    });

    $.ajax({
        'url': '/Master/GetNarrowDDDataForBarcode',
        data: { TableName: tableName, TextFieldName: 'LastUpdatedBy', IsArchived: _IsArchived, IsDeleted: _IsDeleted, Moduleguid: Moduleguid },
        success: function (response) {
            var s = '';
            if (response.DDData != null) {
                $.each(response.DDData, function (i, val) {
                    var ArrData = i.toString().split('[###]');
                    s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + val + ')' + '</option>';
                });
            }

            $("#UserUpdated").empty();
            $("#UserUpdated").multiselect('destroy');
            $("#UserUpdated").multiselectfilter('destroy');
            $("#UserUpdated").append(s);
            $("#UserUpdated").multiselect
                (
                    {
                        noneSelectedText: UserUpdatedby, selectedList: 5,
                        selectedText: function (numChecked, numTotal, checkedItems) {
                            return UpdatedBy + ' ' + numChecked + ' ' + selected;
                        }
                    },
                    {
                        checkAll: function (ui) {
                            $("#UserUpdatedCollapse").html('');
                            for (var i = 0; i <= ui.target.length - 1; i++) {
                                if ($("#UserUpdatedCollapse").text().indexOf(ui.target[i].text) == -1) {
                                    $("#UserUpdatedCollapse").append("<span>" + ui.target[i].text + "</span>");
                                }
                            }
                            $("#UserUpdatedCollapse").show();
                        }
                    }
                )
                .unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                    if (ui.checked) {
                        if ($("#UserUpdatedCollapse").text().indexOf(ui.text) == -1) {
                            $("#UserUpdatedCollapse").append("<span>" + ui.text + "</span>");
                        }
                    }
                    else {
                        if (ui.checked == undefined) {
                            $("#UserUpdatedCollapse").html('');
                        }
                        else if (!ui.checked) {
                            var text = $("#UserUpdatedCollapse").html();
                            text = text.replace("<span>" + ui.text + "</span>", '');
                            $("#UserUpdatedCollapse").html(text);
                        }
                        else
                            $("#UserUpdatedCollapse").html('');
                    }
                    UserUpdatedNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                        return input.value;
                    })

                    _NarrowSearchSave.objSearch.UserUpdated = UserUpdatedNarroValues;

                    if ($("#UserUpdatedCollapse").text().trim() != '')
                        $("#UserUpdatedCollapse").show();
                    else
                        $("#UserUpdatedCollapse").hide();

                    if ($("#UserUpdatedCollapse").find('span').length <= 2) {
                        $("#UserUpdatedCollapse").scrollTop(0).height(50);
                    }
                    else {
                        $("#UserUpdatedCollapse").scrollTop(0).height(100);
                    }
                    clearGlobalIfNotInFocus();
                    DoNarrowSearch();
                }).multiselectfilter();

            _NarrowSearchSave.setControlValue("UserUpdated");
        },
        error: function (response) {
        }
    });
    $.ajax({
        'url': '/Master/GetNarrowDDDataForBarcode',
        data: { TableName: tableName, TextFieldName: 'Supplier', IsArchived: _IsArchived, IsDeleted: _IsDeleted, Moduleguid: Moduleguid },
        success: function (response) {
            var s = '';
            if (response.DDData != null) {
                $.each(response.DDData, function (i, val) {
                    var ArrData = i.toString().split('[###]');
                    s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + val + ')' + '</option>';
                });
            }

            $("#ddlSupplier").empty();
            $("#ddlSupplier").multiselect('destroy');
            $("#ddlSupplier").multiselectfilter('destroy');
            $("#ddlSupplier").append(s);
            $("#ddlSupplier").multiselect
                (
                    {
                        noneSelectedText: Supplier, selectedList: 5,
                        selectedText: function (numChecked, numTotal, checkedItems) {
                            return Supplier + ' ' + numChecked + ' ' + selected;
                        }
                    },
                    {
                        checkAll: function (ui) {
                            $("#ddlSupplierSearchCollapse").html('');
                            for (var i = 0; i <= ui.target.length - 1; i++) {
                                if ($("#ddlSupplierSearchCollapse").text().indexOf(ui.target[i].text) == -1) {
                                    $("#ddlSupplierSearchCollapse").append("<span>" + ui.target[i].text + "</span>");
                                }
                            }
                            $("#ddlSupplierSearchCollapse").show();
                        }
                    }
                )
                .unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                    if (ui.checked) {
                        if ($("#ddlSupplierSearchCollapse").text().indexOf(ui.text) == -1) {
                            $("#ddlSupplierSearchCollapse").append("<span>" + ui.text + "</span>");
                        }
                    }
                    else {
                        if (ui.checked == undefined) {
                            $("#ddlSupplierSearchCollapse").html('');
                        }
                        else if (!ui.checked) {
                            var text = $("#ddlSupplierSearchCollapse").html();
                            text = text.replace("<span>" + ui.text + "</span>", '');
                            $("#ddlSupplierSearchCollapse").html(text);
                        }
                        else
                            $("#ddlSupplierSearchCollapse").html('');
                    }
                    UserSupplierNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                        return input.value;
                    })

                    if ($("#ddlSupplierSearchCollapse").text().trim() != '')
                        $("#ddlSupplierSearchCollapse").show();
                    else
                        $("#ddlSupplierSearchCollapse").hide();

                    if ($("#ddlSupplierSearchCollapse").find('span').length <= 2) {
                        $("#ddlSupplierSearchCollapse").scrollTop(0).height(50);
                    }
                    else {
                        $("#ddlSupplierSearchCollapse").scrollTop(0).height(100);
                    }
                    clearGlobalIfNotInFocus();
                    DoNarrowSearch();
                }).multiselectfilter();
        },
        error: function (response) {
        }
    });

    $.ajax({
        'url': '/Master/GetNarrowDDDataForBarcode',
        data: { TableName: tableName, TextFieldName: 'Items', IsArchived: _IsArchived, IsDeleted: _IsDeleted, Moduleguid: Moduleguid },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('[###]');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });

            $("#ddlItems").empty();
            $("#ddlItems").multiselect('destroy');
            $("#ddlItems").multiselectfilter('destroy');

            $("#ddlItems").append(s);
            $("#ddlItems").multiselect
                (
                    {
                        noneSelectedText: Items, selectedList: 5,
                        selectedText: function (numChecked, numTotal, checkedItems) {
                            return Items + numChecked + ' ' + selected;
                        }
                    },
                    {
                        checkAll: function (ui) {
                            $("#ddlItemsSearchCollapse").html('');
                            for (var i = 0; i <= ui.target.length - 1; i++) {
                                if ($("#ddlItemsSearchCollapse").text().indexOf(ui.target[i].text) == -1) {
                                    $("#ddlItemsSearchCollapse").append("<span>" + ui.target[i].text + "</span>");
                                }
                            }
                            $("#ddlItemsSearchCollapse").show();
                        }
                    }
                )
                .unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                    if (ui.checked) {
                        if ($("#ddlItemsSearchCollapse").text().indexOf(ui.text) == -1) {
                            $("#ddlItemsSearchCollapse").append("<span>" + ui.text + "</span>");
                        }
                    }
                    else {
                        if (ui.checked == undefined) {
                            $("#ddlItemsSearchCollapse").html('');
                        }
                        else if (!ui.checked) {
                            var text = $("#ddlItemsSearchCollapse").html();
                            text = text.replace("<span>" + ui.text + "</span>", '');
                            $("#ddlItemsSearchCollapse").html(text);
                        }
                        else
                            $("#ddlItemsSearchCollapse").html('');
                    }
                    ItemsNarroValues = $.map($(this).multiselect("getChecked"), function (input) {

                        return input.value;
                    })

                    if ($("#ddlItemsSearchCollapse").text().trim() != '')
                        $("#ddlItemsSearchCollapse").show();
                    else
                        $("#ddlItemsSearchCollapse").hide();


                    if ($("#ddlItemsSearchCollapse").find('span').length <= 2) {
                        $("#ddlItemsSearchCollapse").scrollTop(0).height(50);
                    }
                    else {
                        $("#ddlItemsSearchCollapse").scrollTop(0).height(100);
                    }
                    clearGlobalIfNotInFocus();
                    DoNarrowSearch();
                }).multiselectfilter();
        },
        error: function (response) {
        }
    });

    $.ajax({
        'url': '/Master/GetNarrowDDDataForBarcode',
        data: { TableName: tableName, TextFieldName: 'Category', IsArchived: _IsArchived, IsDeleted: _IsDeleted, Moduleguid: Moduleguid },
        success: function (response) {
            var s = '';
            if (response.DDData != null) {
                $.each(response.DDData, function (i, val) {
                    var ArrData = i.toString().split('[###]');
                    s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + val + ')' + '</option>';
                });
            }
            $("#ddlBinItemCategory").empty();
            $("#ddlBinItemCategory").multiselect('destroy');
            $("#ddlBinItemCategory").multiselectfilter('destroy');
            $("#ddlBinItemCategory").append(s);
            $("#ddlBinItemCategory").multiselect
                (
                    {
                        noneSelectedText: Category, selectedList: 5,
                        selectedText: function (numChecked, numTotal, checkedItems) {
                            return Category + ' ' + numChecked + ' ' + selected;
                        }
                    },
                    {
                        checkAll: function (ui) {
                            $("#ddlBinItemCategorySearchCollapse").html('');
                            for (var i = 0; i <= ui.target.length - 1; i++) {
                                if ($("#ddlBinItemCategorySearchCollapse").text().indexOf(ui.target[i].text) == -1) {
                                    $("#ddlBinItemCategorySearchCollapse").append("<span>" + ui.target[i].text + "</span>");
                                }
                            }
                            $("#ddlBinItemCategorySearchCollapse").show();
                        }
                    }
                )
                .unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                    if (ui.checked) {
                        if ($("#ddlBinItemCategorySearchCollapse").text().indexOf(ui.text) == -1) {
                            $("#ddlBinItemCategorySearchCollapse").append("<span>" + ui.text + "</span>");
                        }
                    }
                    else {
                        if (ui.checked == undefined) {
                            $("#ddlBinItemCategorySearchCollapse").html('');
                        }
                        else if (!ui.checked) {
                            var text = $("#ddlBinItemCategorySearchCollapse").html();
                            text = text.replace("<span>" + ui.text + "</span>", '');
                            $("#ddlBinItemCategorySearchCollapse").html(text);
                        }
                        else
                            $("#ddlBinItemCategorySearchCollapse").html('');
                    }
                    CateogryNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                        return input.value;
                    })

                    if ($("#ddlBinItemCategorySearchCollapse").text().trim() != '')
                        $("#ddlBinItemCategorySearchCollapse").show();
                    else
                        $("#ddlBinItemCategorySearchCollapse").hide();

                    if ($("#ddlBinItemCategorySearchCollapse").find('span').length <= 2) {
                        $("#ddlBinItemCategorySearchCollapse").scrollTop(0).height(50);
                    }
                    else {
                        $("#ddlBinItemCategorySearchCollapse").scrollTop(0).height(100);
                    }
                    clearGlobalIfNotInFocus();
                    DoNarrowSearch();
                }).multiselectfilter();
        },
        error: function (response) {
        }
    });
}

function RefreshItemType() {
    GetNarroHTMLForItemType();
}

function RefreshInventoryClassification() {
    GetNarroHTMLForInventoryClassification();
}

function RefreshBOMItemType() {
    GetNarroHTMLForBOMItemType();
}

function RefressFilterUserMaster(pagename) {
    var _IsArchived = false;
    var _IsDeleted = false;

    if (typeof ($('#IsArchivedRecords')) != undefined)
        _IsArchived = $('#IsArchivedRecords').is(':checked');

    if (typeof ($('#IsDeletedRecords')) != undefined)
        _IsDeleted = $('#IsDeletedRecords').is(':checked');

    GetUserMasterNarrowSearchData(pagename, _IsArchived, _IsDeleted);
}

function RefressFilterItemMasterBin(pagename) {
    var _IsArchived = false;
    var _IsDeleted = false;

    if (typeof ($('#IsArchivedRecords')) != undefined)
        _IsArchived = $('#IsArchivedRecords').is(':checked');

    if (typeof ($('#IsDeletedRecords')) != undefined)
        _IsDeleted = $('#IsDeletedRecords').is(':checked');

    GetItemMasterBinNarrowSearchData(pagename, _IsArchived, _IsDeleted);

}

function RefressFilterRoleMaster(pagename) {
    var _IsArchived = false;
    var _IsDeleted = false;

    if (typeof ($('#IsArchivedRecords')) != undefined)
        _IsArchived = $('#IsArchivedRecords').is(':checked');

    if (typeof ($('#IsDeletedRecords')) != undefined)
        _IsDeleted = $('#IsDeletedRecords').is(':checked');

    GetRoleMasterNarrowSearchData(pagename, _IsArchived, _IsDeleted);

}
function RefressFilterICount(pagename,resstrcountname) {
    var _IsArchived = false;
    var _IsDeleted = false;

    if (typeof ($('#IsArchivedRecords')) != undefined)
        _IsArchived = $('#IsArchivedRecords').is(':checked');

    if (typeof ($('#IsDeletedRecords')) != undefined)
        _IsDeleted = $('#IsDeletedRecords').is(':checked');

    GetICountNarrowSearchData(pagename, _IsArchived, _IsDeleted, resstrcountname, countstatusresourcename);

}
function RefressFilterNotification(pagename) {
    var _IsArchived = false;
    var _IsDeleted = false;

    if (typeof ($('#IsArchivedRecords')) != undefined)
        _IsArchived = $('#IsArchivedRecords').is(':checked');

    if (typeof ($('#IsDeletedRecords')) != undefined)
        _IsDeleted = $('#IsDeletedRecords').is(':checked');

    GetNotificationNarrowSearchData(pagename, _IsArchived, _IsDeleted);

}
function RefressFilterMS(pagename, resStagingLocation) {
    var _IsArchived = false;
    var _IsDeleted = false;

    if (typeof ($('#IsArchivedRecords')) != undefined)
        _IsArchived = $('#IsArchivedRecords').is(':checked');

    if (typeof ($('#IsDeletedRecords')) != undefined)
        _IsDeleted = $('#IsDeletedRecords').is(':checked');

    GetMatStagNarrowSearchData(pagename, _IsArchived, _IsDeleted, resStagingLocation);

}
function RefressFilterCart(pagename) {
    var _IsArchived = false;
    var _IsDeleted = false;

    if (typeof ($('#IsArchivedRecords')) != undefined)
        _IsArchived = $('#IsArchivedRecords').is(':checked');

    if (typeof ($('#IsDeletedRecords')) != undefined)
        _IsDeleted = $('#IsDeletedRecords').is(':checked');

    var ReplanishType = '';
    var ArrFregments = location.hash.split('#');
    var i = 0;
    for (i = 0; i < ArrFregments.length; i++) {
        if (ArrFregments[i].toUpperCase() == 'LISTORDERS') {
            ReplanishType = "Purchase";
            break;
        }
        else if (ArrFregments[i].toUpperCase() == 'LISTTRANSFERS') {
            ReplanishType = "Transfer";
            break;
        }
        else if (ArrFregments[i].toUpperCase() == 'LISTSUGGESTEDRETURN') {
            ReplanishType = "SuggestedReturn";
            break;
        }
    }

    GetCartNarrowSearchData(pagename, _IsArchived, _IsDeleted, ReplanishType);

}
function clearReqNarrowSearchSelection() {
    OrderRequiredDateNarroValues = "";
    RequistionSupplierValues = "";
    WorkOrderNarroValues = "";
    OrderSupplierNarroValues = "";
    ddlRequisitionStatusNarroValues = "";
    UserCreatedNarroValues = "";
    UserUpdatedNarroValues = "";
    UserUDF1NarrowValues = "";
    UserUDF2NarrowValues = "";
    UserUDF3NarrowValues = "";
    UserUDF4NarrowValues = "";
    UserUDF5NarrowValues = "";
    $('#NarroSearchClear').click();   
    $("#UserCreated").multiselect("uncheckAll");
    $("#UserCreatedCollapse").html('');
    $("#UserUpdated").multiselect("uncheckAll");
    $("#UserUpdatedCollapse").html('');
}
function CallReqNarrowFunctions() {
    var _IsArchived = false;
    var _IsDeleted = false;

    if (typeof ($('#IsArchivedRecords')) != undefined)
        _IsArchived = $('#IsArchivedRecords').is(':checked');

    if (typeof ($('#IsDeletedRecords')) != undefined)
        _IsDeleted = $('#IsDeletedRecords').is(':checked');

    GetRequisitionNarrowSearchData(_NarrowSearchLayout.pageName, _IsArchived, _IsDeleted, RequisitionCurrentTab);    
    _NarrowSearchSave.setStaticControlValues();
    if (!isFromNarrowSearchClear) {
        setTimeout(function () {
            DoNarrowSearch();
        }, 1000);
    }
}

function clearQuoteMasterNarrowSearchSelection() {
    QuoteStatusNarroValues = "";
    QuoteSupplierNarrowValues = "";
    UserCreatedNarroValues = "";
    UserUpdatedNarroValues = "";
    UserUDF1NarrowValues = "";
    UserUDF2NarrowValues = "";
    UserUDF3NarrowValues = "";
    UserUDF4NarrowValues = "";
    UserUDF5NarrowValues = "";
    $('#NarroSearchClear').click();
    $("#UserCreated").multiselect("uncheckAll");
    $("#UserCreatedCollapse").html('');
    $("#UserUpdated").multiselect("uncheckAll");
    $("#UserUpdatedCollapse").html('');
}

function clearQuoteMasterNarrowSearchSelectionForTabClick() {
    QuoteStatusNarroValues = "";
    QuoteSupplierNarrowValues = "";
    UserCreatedNarroValues = "";
    UserUpdatedNarroValues = "";
    UserUDF1NarrowValues = "";
    UserUDF2NarrowValues = "";
    UserUDF3NarrowValues = "";
    UserUDF4NarrowValues = "";
    UserUDF5NarrowValues = "";
    //$('#NarroSearchClear').click();

    if (typeof ($("#QuoteStatus").multiselect("getChecked").length) != "undefined" && $("#QuoteStatus").multiselect("getChecked").length > 0) {
        //$("#QuoteStatus").multiselect("uncheckAll");
        $("#QuoteStatusCollapse").html('').hide();
        QuoteStatusNarroValues = '';
    }
    else if (typeof ($("#QuoteStatusCollapse")) != "undefined") {
        $("#QuoteStatusCollapse").html('');
        QuoteStatusNarroValues = '';
        $("#QuoteStatusCollapse").hide();
    }

    if (typeof ($("#QuoteSupplier").multiselect("getChecked").length) != "undefined" && $("#QuoteSupplier").multiselect("getChecked").length > 0) {
        //$("#QuoteStatus").multiselect("uncheckAll");
        $("#QuoteSupplierCollapse").html('').hide();
        QuoteSupplierNarrowValues = '';
    }
    else if (typeof ($("#QuoteSupplierCollapse")) != "undefined") {
        $("#QuoteSupplierCollapse").html('');
        QuoteSupplierNarrowValues = '';
        $("#QuoteSupplierCollapse").hide();
    }

    //$("#QuoteStatus").empty();
    //$("#QuoteStatus").multiselect('destroy');
    //$("#QuoteStatus").multiselectfilter('destroy');

    //$("#UserCreated").multiselect("uncheckAll");
    $("#UserCreatedCollapse").html('').hide();
    //$("#UserUpdated").multiselect("uncheckAll");
    $("#UserUpdatedCollapse").html('').hide();
}

function CallQuoteMasterNarrowFunctions() {
    var _IsArchived = false;
    var _IsDeleted = false;

    if (typeof ($('#IsArchivedRecords')) != undefined)
        _IsArchived = $('#IsArchivedRecords').is(':checked');

    if (typeof ($('#IsDeletedRecords')) != undefined)
        _IsDeleted = $('#IsDeletedRecords').is(':checked');

    GetQuoteMasterNarrowSearchData(_NarrowSearchLayout.pageName, _IsArchived, _IsDeleted, RequisitionCurrentTab);
}


function CallWONarrowFunctions() {
    var _IsArchived = false;
    var _IsDeleted = false;

    if (typeof ($('#IsArchivedRecords')) != undefined)
        _IsArchived = $('#IsArchivedRecords').is(':checked');

    if (typeof ($('#IsDeletedRecords')) != undefined)
        _IsDeleted = $('#IsDeletedRecords').is(':checked');

    GetWONarrowSearchData(_NarrowSearchLayout.pageName, _IsArchived, _IsDeleted);
}
function CallNarrowfunctions(operation) {

    var _IsArchived = false;
    var _IsDeleted = false;

    if (typeof ($('#IsArchivedRecords')) != undefined)
        _IsArchived = $('#IsArchivedRecords').is(':checked');

    if (typeof ($('#IsDeletedRecords')) != undefined)
        _IsDeleted = $('#IsDeletedRecords').is(':checked');
    if (_NarrowSearchLayout.pageName != "RequisitionMaster" && _NarrowSearchLayout.pageName != "QuoteMaster") {
        RequisitionCurrentTab = ToolListTab;
    }
    GetNarrowDDData(_NarrowSearchLayout.pageName, _NarrowSearchLayout.companyID, _NarrowSearchLayout.roomID, _IsArchived, _IsDeleted, RequisitionCurrentTab);    
    GetNarroHTMLForUDF(_NarrowSearchLayout.pageName, _NarrowSearchLayout.companyID, _NarrowSearchLayout.roomID, _IsArchived, _IsDeleted, RequisitionCurrentTab, IDsufix, IDsufix2);

    $("#ToolCheckout").multiselect(
        {
            noneSelectedText: 'Tool Status', selectedList: 5,
            selectedText: function (numChecked, numTotal, checkedItems) {
                return 'Tool Status: ' + numChecked + ' ' + selected;
            }
        },
        {
            checkAll: function (ui) {
                $("#ToolCheckoutCollapse").html('');
                for (var i = 0; i <= ui.target.length - 1; i++) {
                    if ($("#ToolCheckoutCollapse").text().indexOf(ui.target[i].text) == -1) {
                        $("#ToolCheckoutCollapse").append("<span>" + ui.target[i].text + "</span>");
                    }
                }
                $("#ToolCheckoutCollapse").show();
            }
        }
    )
        .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
            if (ui.checked) {
                if ($("#ToolCheckoutCollapse").text().indexOf(ui.text) == -1) {
                    $("#ToolCheckoutCollapse").append("<span>" + ui.text + "</span>");
                }
            }
            else {
                if (ui.checked == undefined) {
                    $("#ToolCheckoutCollapse").html('');
                }
                else if (!ui.checked) {
                    var text = $("#ToolCheckoutCollapse").html();
                    text = text.replace("<span>" + ui.text + "</span>", '');
                    $("#ToolCheckoutCollapse").html(text);
                }
                else
                    $("#ToolCheckoutCollapse").html('');
            }
            ToolCheckoutValue = $.map($(this).multiselect("getChecked"), function (input) {
                return input.value;
            })
            _NarrowSearchSave.objSearch.ToolCheckout = ToolCheckoutValue;

            if ($("#ToolCheckoutCollapse").text().trim() != '')
                $("#ToolCheckoutCollapse").show();
            else
                $("#ToolCheckoutCollapse").hide();


            if ($("#ToolCheckoutCollapse").find('span').length <= 2) {
                $("#ToolCheckoutCollapse").scrollTop(0).height(50);
            }
            else {
                $("#ToolCheckoutCollapse").scrollTop(0).height(100);
            }
            clearGlobalIfNotInFocus();
            DoNarrowSearch();
        }).multiselectfilter();
    _NarrowSearchSave.setControlValue("ToolCheckout");
    $("#ToolCheckoutNew").multiselect(
        {
            noneSelectedText: 'Tool Status', selectedList: 5,
            selectedText: function (numChecked, numTotal, checkedItems) {
                return 'Tool Status: ' + numChecked + ' ' + selected;
            }
        },
        {
            checkAll: function (ui) {
                $("#ToolCheckoutNewCollapse").html('');
                for (var i = 0; i <= ui.target.length - 1; i++) {
                    if ($("#ToolCheckoutNewCollapse").text().indexOf(ui.target[i].text) == -1) {
                        $("#ToolCheckoutNewCollapse").append("<span>" + ui.target[i].text + "</span>");
                    }
                }
                $("#ToolCheckoutNewCollapse").show();
            }
        }
    )
        .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {

            if (ui.checked) {
                if ($("#ToolCheckoutNewCollapse").text().indexOf(ui.text) == -1) {
                    $("#ToolCheckoutNewCollapse").append("<span>" + ui.text + "</span>");
                }
            }
            else {
                if (ui.checked == undefined) {
                    $("#ToolCheckoutNewCollapse").html('');
                }
                else if (!ui.checked) {
                    var text = $("#ToolCheckoutNewCollapse").html();
                    text = text.replace("<span>" + ui.text + "</span>", '');
                    $("#ToolCheckoutNewCollapse").html(text);
                }
                else
                    $("#ToolCheckoutNewCollapse").html('');
            }
            ToolCheckoutValue = $.map($(this).multiselect("getChecked"), function (input) {
                return input.value;
            })

            if ($("#ToolCheckoutNewCollapse").text().trim() != '')
                $("#ToolCheckoutNewCollapse").show();
            else
                $("#ToolCheckoutNewCollapse").hide();


            if ($("#ToolCheckoutNewCollapse").find('span').length <= 2) {
                $("#ToolCheckoutNewCollapse").scrollTop(0).height(50);
            }
            else {
                $("#ToolCheckoutNewCollapse").scrollTop(0).height(100);
            }
            clearGlobalIfNotInFocus();
            DoNarrowSearch();
        }).multiselectfilter();

    /*
    $("#ToolCheckout").multiselect(
          {
              noneSelectedText: 'ToolMaintence', selectedList: 5,
              selectedText: function (numChecked, numTotal, checkedItems) {
                  return 'ToolMaintence: ' + numChecked + ' selected';
              }
          },
                    {
                        checkAll: function (ui) {
                            $("#ToolCheckoutCollapse").html('');
                            for (var i = 0; i <= ui.target.length - 1; i++) {
                                if ($("#ToolCheckoutCollapse").text().indexOf(ui.target[i].text) == -1) {
                                    $("#ToolCheckoutCollapse").append("<span>" + ui.target[i].text + "</span>");
                                }
                            }
                            $("#ToolCheckoutCollapse").show();
                        }
                    }
        )
        .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
            if (ui.checked) {
                if ($("#ToolCheckoutCollapse").text().indexOf(ui.text) == -1) {
                    $("#ToolCheckoutCollapse").append("<span>" + ui.text + "</span>");
                }
            }
            else {
                if (ui.checked == undefined) {
                    $("#ToolCheckoutCollapse").html('');
                }
                else if (!ui.checked) {
                    var text = $("#ToolCheckoutCollapse").html();
                    text = text.replace("<span>" + ui.text + "</span>", '');
                    $("#ToolCheckoutCollapse").html(text);
                }
                else
                    $("#ToolCheckoutCollapse").html('');
            }
            MaintenseValue = $.map($(this).multiselect("getChecked"), function (input) {
                return input.value;
            })

            if ($("#ToolCheckoutCollapse").text().trim() != '')
                $("#ToolCheckoutCollapse").show();
            else
                $("#ToolCheckoutCollapse").hide();


            if ($("#ToolCheckoutCollapse").find('span').length <= 2) {
                $("#ToolCheckoutCollapse").scrollTop(0).height(50);
            }
            else {
                $("#ToolCheckoutCollapse").scrollTop(0).height(100);
            }
            DoNarrowSearch();
        });
        */

    if (_NarrowSearchLayout.pageName == 'ItemMaster' || _NarrowSearchLayout.pageName == 'ItemBinMaster') {
        if (_utils.isNullUndefined(operation) ||
            (operation != "IsDeletedRecords_Click" && operation != "IsArchivedRecords_Click")
        ) {
            RefreshItemType();
            RefreshInventoryClassification();
        }
    }
    if (_NarrowSearchLayout.pageName == 'BOMItemMaster') {
        RefreshBOMItemType();
    }
    if (_NarrowSearchLayout.pageName == 'UserMaster') {
        RefressFilterUserMaster('UserMaster');
    }
    if (_NarrowSearchLayout.pageName == 'ItemMasterBinList') {
        RefressFilterItemMasterBin('ItemMasterBinList');
    }

    if (_NarrowSearchLayout.pageName == 'RoleMaster') {
        RefressFilterRoleMaster(_NarrowSearchLayout.pageName);
    }
    if (_NarrowSearchLayout.pageName == 'AssetMaster') {
        var _IsArchived = false;
        var _IsDeleted = false;

        if (typeof ($('#IsArchivedRecords')) != undefined)
            _IsArchived = $('#IsArchivedRecords').is(':checked');

        if (typeof ($('#IsDeletedRecords')) != undefined)
            _IsDeleted = $('#IsDeletedRecords').is(':checked');

        GetAssetNarrowSearchData('AssetMaster', _IsArchived, _IsDeleted);        
    }
    //if (_IsDeleted || _IsArchived) {
    //    $('#deleteRows').attr("style", "display:none");
    //    $('#undeleteRows').attr("style", "display:''");
    //    AllowDeletePopup = false;
    //}
    //else {
    //    $('#deleteRows').attr("style", "display:''");
    //    $('#undeleteRows').attr("style", "display:none");
    //    AllowDeletePopup = true;
    //}
    if (_IsDeleted) {
        $('#deleteRows,#aArchiveRows,#aUnArchiveRows').attr("style", "display:none"); //,
        $('#undeleteRows').attr("style", "display:''");
        AllowDeletePopup = false;
    }
    else {
        $('#deleteRows,#aArchiveRows').attr("style", "display:''");
        $('#undeleteRows,#aUnArchiveRows').attr("style", "display:none");
        AllowDeletePopup = true;
    }
    
    if (_IsArchived) {
        $('#aArchiveRows,#deleteRows,#undeleteRows').attr("style", "display:none");
        $('#aUnArchiveRows').attr("style", "display:''");
        //AllowDeletePopup = false;
    }
    else if (_NarrowSearchLayout.pageName != 'FTPMasterList' && _NarrowSearchLayout.pageName != 'InventoryClassificationMaster'
        && _NarrowSearchLayout.pageName != 'BomInventoryClassificationMaster'
        && _NarrowSearchLayout.pageName != 'RoleMaster'
        && _NarrowSearchLayout.pageName != 'ToolMaster'
        && _NarrowSearchLayout.pageName != 'ToolMasterNew'
        && _NarrowSearchLayout.pageName != 'WorkOrder'
        && _NarrowSearchLayout.pageName != 'CategoryMaster'
        && _NarrowSearchLayout.pageName != 'GLAccountMaster'
        && _NarrowSearchLayout.pageName != 'BOMItemMaster'
        && _NarrowSearchLayout.pageName != 'BomSupplierMaster'
        && _NarrowSearchLayout.pageName != 'AssetMaster'
        && _NarrowSearchLayout.pageName != 'AssetCategoryMaster'
        && _NarrowSearchLayout.pageName != 'EnterpriseQLMaster'
        && _NarrowSearchLayout.pageName != 'NotificationMasterList'
        && _NarrowSearchLayout.pageName != 'Room'
        && _NarrowSearchLayout.pageName != 'WrittenOffCategory'
        && _NarrowSearchLayout.pageName != 'PullMaster'
        && _NarrowSearchLayout.pageName != 'ProjectList'
        && _NarrowSearchLayout.pageName != 'UserMaster'    ) {
        $('#aArchiveRows,#deleteRows').attr("style", "display:''");
        $('#aUnArchiveRows,#undeleteRows').attr("style", "display:none");
        //AllowDeletePopup = true;
        if ((_NarrowSearchLayout.pageName == 'ItemMaster' && _IsDeleted) || (_NarrowSearchLayout.pageName == 'SupplierMaster' && _IsDeleted)) {
            $('#deleteRows').attr("style", "display:none");
            $('#undeleteRows').attr("style", "display:''");
        }
    }
    $(".tab5").hide();

}

function CommonUDFNarrowSearch() {
    //alert('test');
    //alert(_NarrowSearchLayout.pageName);
    var _IsArchived = false;
    var _IsDeleted = false;

    if (typeof ($('#IsArchivedRecords')) != undefined)
        _IsArchived = $('#IsArchivedRecords').is(':checked');

    if (typeof ($('#IsDeletedRecords')) != undefined)
        _IsDeleted = $('#IsDeletedRecords').is(':checked');

    GetNarroHTMLForUDF(_NarrowSearchLayout.pageName, _NarrowSearchLayout.companyID, _NarrowSearchLayout.roomID, _IsArchived, _IsDeleted, RequisitionCurrentTab, IDsufix, IDsufix2);


}

function CompanyStatusNarroSearch(SSDDLObject) {
    //oTable.fnFilter($(CostDDLObject).val(),null,null,null);
    if ($(SSDDLObject).val() != "") {
        CompanyStatusValue = $(SSDDLObject).val();
        DoNarrowSearch();
    }
    else {
        CompanyStatusValue = '';
        DoNarrowSearch();
    }
}

/* COMMON FUNCTION TO NARROW SEARCH HIDE/SHOW ALONG WITH STATE SAVED : Dec 24, 2012 : IJ */

$(document).ready(function () {
    $('#ExpandNarrowSearch').click(function (e) {
        ExpandNarrowSearch();
    });
    $('#CollapseNarrowSearch').click(function (e) {
        CollapseNarrowSearch();
    });

    var NarrowSearchState = getCookie('NarrowSearchState');
    if (NarrowSearchState == 'Expanded') {
        CollapseNarrowSearch();
    }
    else {
        ExpandNarrowSearch();
    }
});

function ExpandNarrowSearch() {
    var w = $('.IteamBlock').css("width");
    $('.IteamBlock').show();
    $('.IteamBlock').stop().animate({
        width: "18%"
    }, 0, function () {
        $('.userContent').css({ "width": "80.5%", "margin": "0" });
        $('#myDataTable_length').css({ "left": "0px" });
        $('#myDataTable_paginate').css({ "left": "145px" });
        $('div#divModule').css({ "margin-left": "253px" });
        $('.leftopenContent').css({ "display": "none" });
        setCookie('NarrowSearchState', 'Collapsed');
    });
}

function CollapseNarrowSearch() {

    $('.IteamBlock').stop().animate({
        width: "0%"
    }, 0, function () {
        $('.IteamBlock').hide();
        $('.userContent').css({ "width": "98.5%", margin: "0 0.4% 1%" });
        //var Left = $('.viewBlock').css("width");
        var Left = '270px';
        $('#myDataTable_length').css({ "left": Left });
        var LeftW = 145 + parseInt(Left);
        $('#myDataTable_paginate').css({ "left": LeftW + 'px' });
        $('div#divModule').css({ "margin-left": '270px' });
        if (typeof (oTable) != "undefined") {
            oTable.fnAdjustColumnSizing();
        }
        $('.leftopenContent').css({ "display": "" });
        setCookie('NarrowSearchState', 'Expanded');
    });
}

/* COMMON FUNCTION TO NARROW SEARCH HIDE/SHOW ALONG WITH STATE SAVED : Dec 24, 2012 : IJ */


function RefreshNarrowSearchCommonly() {
    CallNarrowfunctions();


    if (_NarrowSearchLayout.pageName == "AssetMaster") {
        var _IsArchived = false;
        var _IsDeleted = false;

        if (typeof ($('#IsArchivedRecords')) != undefined)
            _IsArchived = $('#IsArchivedRecords').is(':checked');

        if (typeof ($('#IsDeletedRecords')) != undefined)
            _IsDeleted = $('#IsDeletedRecords').is(':checked');

        GetAssetNarrowSearchData('AssetMaster', _IsArchived, _IsDeleted);
    }


    if (_NarrowSearchLayout.pageName == 'ToolMaster' || _NarrowSearchLayout.pageName == 'KitToolMaster') {
        var _IsArchived = false;
        var _IsDeleted = false;
        var _TabName = 'ToolList';

        if (typeof ($('#IsArchivedRecords')) != undefined)
            _IsArchived = $('#IsArchivedRecords').is(':checked');

        if (typeof ($('#IsDeletedRecords')) != undefined)
            _IsDeleted = $('#IsDeletedRecords').is(':checked');

        if (_NarrowSearchLayout.pageName == 'KitToolMaster')
            _TabName = 'KitList';

        GetToolsNarrowSearchData(_IsArchived, _IsDeleted, _NarrowSearchLayout.pageName, _TabName);
    }
    if (_NarrowSearchLayout.pageName == 'ToolMasterNew' || _NarrowSearchLayout.pageName == 'KitToolMasterNew') {
        var _IsArchived = false;
        var _IsDeleted = false;
        var _TabName = 'ToolList';
        if (typeof ($('#IsArchivedRecords')) != undefined)
            _IsArchived = $('#IsArchivedRecords').is(':checked');

        if (typeof ($('#IsDeletedRecords')) != undefined)
            _IsDeleted = $('#IsDeletedRecords').is(':checked');

        if (_NarrowSearchLayout.pageName == 'KitToolMasterNew')
            _TabName = 'KitList';
        GetToolsNarrowSearchDataNew(_IsArchived, _IsDeleted, _NarrowSearchLayout.pageName, _TabName);
    }

    if (_NarrowSearchLayout.pageName == 'PullMaster' || _NarrowSearchLayout.pageName == 'ItemMaster' || _NarrowSearchLayout.pageName == 'BOMItemMaster' || _NarrowSearchLayout.pageName == 'ItemBinMaster') {
        var _IsArchived = false;
        var _IsDeleted = false;

        if (typeof ($('#IsArchivedRecords')) != undefined)
            _IsArchived = $('#IsArchivedRecords').is(':checked');

        if (typeof ($('#IsDeletedRecords')) != undefined)
            _IsDeleted = $('#IsDeletedRecords').is(':checked');
        
        GetPullNarrowSearchData(_NarrowSearchLayout.pageName, _IsArchived, _IsDeleted);
        //GetPullNarrowSearchData();
    }

    if (_NarrowSearchLayout.pageName == 'CompanyMaster') {
        var _IsArchived = false;
        var _IsDeleted = false;

        if (typeof ($('#IsArchivedRecords')) != undefined)
            _IsArchived = $('#IsArchivedRecords').is(':checked');

        if (typeof ($('#IsDeletedRecords')) != undefined)
            _IsDeleted = $('#IsDeletedRecords').is(':checked');

        GetCompanyMasterNarrowSearchData(_NarrowSearchLayout.pageName, _IsArchived, _IsDeleted);
    }

    if (_NarrowSearchLayout.pageName == 'Room') {
        var _IsArchived = false;
        var _IsDeleted = false;

        if (typeof ($('#IsArchivedRecords')) != undefined)
            _IsArchived = $('#IsArchivedRecords').is(':checked');

        if (typeof ($('#IsDeletedRecords')) != undefined)
            _IsDeleted = $('#IsDeletedRecords').is(':checked');

        GetRoomMasterNarrowSearchData(_NarrowSearchLayout.pageName, _IsArchived, _IsDeleted);
    }

    if (_NarrowSearchLayout.pageName == 'ItemMaster') {
        RefreshItemType();
        RefreshInventoryClassification();
    }

    if (_NarrowSearchLayout.pageName == 'BOMItemMaster') {
        RefreshBOMItemType();
    }

    if (_NarrowSearchLayout.pageName == 'RequisitionMaster') {
        if (window.location.hash.toLowerCase() == "#list") {
            _NarrowSearchSave.currentListName = "RequisitionMaster";
        }
        else if (window.location.hash.toLowerCase() == "#unsubmitted") {
            _NarrowSearchSave.currentListName = "RequisitionMasterUnsubmitted";
        }
        else if (window.location.hash.toLowerCase() == "#submitted") {
            _NarrowSearchSave.currentListName = "RequisitionMasterSubmitted";
        }
        else if (window.location.hash.toLowerCase() == "#approved") {
            _NarrowSearchSave.currentListName = "RequisitionMasterApproved";
        }
        else if (window.location.hash.toLowerCase() == "#closed") {
            _NarrowSearchSave.currentListName = "RequisitionMasterClosed"
        }
        _NarrowSearchSave.loadNarrowSearch();
        setTimeout(function () {
            CallReqNarrowFunctions();
            //_NarrowSearchSave.setStaticControlValues();
        }, 500);                
    }

    if (_NarrowSearchLayout.pageName == 'WorkOrder') {
        CallWONarrowFunctions();
    }

    if (_NarrowSearchLayout.pageName == 'OrderMaster') {
        var _IsArchived = false;
        var _IsDeleted = false;

        if (typeof ($('#IsArchivedRecords')) != undefined)
            _IsArchived = $('#IsArchivedRecords').is(':checked');

        if (typeof ($('#IsDeletedRecords')) != undefined)
            _IsDeleted = $('#IsDeletedRecords').is(':checked');

        GetOrderNarrowSearchData(_NarrowSearchLayout.pageName, _IsArchived, _IsDeleted);
    }
    if (_NarrowSearchLayout.pageName == 'ReceiveMaster') {
        var _IsArchived = false;
        var _IsDeleted = false;

        if (typeof ($('#IsArchivedRecords')) != undefined)
            _IsArchived = $('#IsArchivedRecords').is(':checked');

        if (typeof ($('#IsDeletedRecords')) != undefined)
            _IsDeleted = $('#IsDeletedRecords').is(':checked');

        GetReceiveNarrowSearchData(_NarrowSearchLayout.pageName, _IsArchived, _IsDeleted);
    }
    if (_NarrowSearchLayout.pageName == 'MaterialStaging') {

        RefressFilterMS("MaterialStaging", resStagingLocation);
    }
    if (_NarrowSearchLayout.pageName == 'InventoryCountList') {
        RefressFilterICount(_NarrowSearchLayout.pageName, counttyperesourcename, countstatusresourcename);
    }
    if (_NarrowSearchLayout.pageName == 'NotificationMasterList') {

        RefressFilterNotification(_NarrowSearchLayout.pageName);
    }

    if (_NarrowSearchLayout.pageName == 'CartItem' || _NarrowSearchLayout.pageName == 'CartItemList') {

        RefressFilterCart(_NarrowSearchLayout.pageName);
    }
    if (_NarrowSearchLayout.pageName == 'RoleMaster') {
        RefressFilterRoleMaster(_NarrowSearchLayout.pageName);
    }
    if (_NarrowSearchLayout.pageName == 'QuoteMaster') {
        CallQuoteMasterNarrowFunctions();
    }

    $('a.downarrow').click(function (e) {
        e.preventDefault();
        $(this).closest('.accordion').find('.dropcontent').slideToggle();
    });


};
function GetNarroHTMLForUDFIM(tableName, companyID, roomID) {
    if (tableName != "CartItemList") {
        
        var UDFObject;
        $("select[name='udflist_CartListItem']").each(function (index) {
            var UDFUniqueID = this.getAttribute('UID');
            
            var DataValue = { TableName: tableName, UDFName: this.id, UDFUniqueID: UDFUniqueID, IsArchived: false, IsDeleted: false, "ItemModelCallFrom": ItemModelCallFrom, ParentID: ItemModelParentID };

            if (tableName.toUpperCase() == 'MoveMaterial'.toUpperCase()) {
                DataValue = { TableName: tableName, UDFName: this.id, UDFUniqueID: UDFUniqueID, IsArchived: false, IsDeleted: false, "ItemModelCallFrom": ItemModelCallFrom, ParentID: ItemModelParentID, moveType: $('#ddlMoveType').val() };
            }

            $.ajax({
                'url': '/Master/GetUDFDDData',
                data: DataValue,
                success: function (response) {
                    var s = '';
                    if (response.DDData != null) {
                        $.each(response.DDData, function (UDFVal, ValCount) {
                            s += '<option value="' + UDFVal + '">' + UDFVal + '(' + ValCount + ')' + '</option>';
                        });
                    }
                    var UDFColumnNameTemp = response.UDFColName.toString().replace("_dd_CartListItem", "");
                    //$('#' + response.UDFColName).append(s);
                    //$('#' + response.UDFColName).multiselect

                    //Destroy widgets before reapplying the filter
                    $('[id="' + response.UDFColName + '"]').empty();
                    $('[id="' + response.UDFColName + '"]').multiselect('destroy');
                    $('[id="' + response.UDFColName + '"]').multiselectfilter('destroy');

                    $('[id="' + response.UDFColName + '"]').append(s);
                    $('[id="' + response.UDFColName + '"]').multiselect
                        (
                            {
                                noneSelectedText: UDFColumnNameTemp, selectedList: 5,
                                selectedText: function (numChecked, numTotal, checkedItems) {
                                    return UDFColumnNameTemp + ' ' + numChecked + ' ' + selected;
                                }
                            },
                            {
                                checkAll: function (ui) {
                                    var CollapseObject = $('#' + UDFUniqueID + 'Collapse_CartListItem')
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
                        .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                            var CollapseObject = $('#' + UDFUniqueID + 'Collapse_CartListItem')

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

                            if (UDFUniqueID == "ITEMUDF1") {
                                ItemUDF1 = $.map($(this).multiselect("getChecked"), function (input) {
                                    return input.value;
                                })
                            }
                            else if (UDFUniqueID == "ITEMUDF2") {
                                ItemUDF2 = $.map($(this).multiselect("getChecked"), function (input) {
                                    return input.value;
                                })
                            }
                            else if (UDFUniqueID == "ITEMUDF3") {
                                ItemUDF3 = $.map($(this).multiselect("getChecked"), function (input) {
                                    return input.value;
                                })
                            }
                            else if (UDFUniqueID == "ITEMUDF4") {
                                ItemUDF4 = $.map($(this).multiselect("getChecked"), function (input) {
                                    return input.value;
                                })
                            }
                            else {
                                ItemUDF5 = $.map($(this).multiselect("getChecked"), function (input) {
                                    return input.value;
                                })
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
                            clearGlobalIMIfNotInFocus();
                            DoNarrowSearch();
                        }).multiselectfilter();
                },
                error: function (response) {

                    // through errror message
                }
            });
        });
    }
};
function clearGlobalIMIfNotInFocus() {
    if ($(document.activeElement).attr('id') != 'ItemModel_filter')
        $("#ItemModel_filter").val('');
};