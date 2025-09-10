var MMSupplierNarroValues;
var MMCategoryNarroValues;
var MMManufacturerNarroValues;
var MMItemLocationNarroValues;
var MMItemTrackingTypeNarroValues;
var MMItemStockStatusNarroValues;
var MMItemTypeNarroSearchValue;
var MMInventoryClassificationNarroSearchValue;
var MMItemActiveNarroSearchValue;
var MMCostNarroSearchValue;
var MMAverageCostNarroSearchValue;
var MMTurnsNarroSearchValue;

var MMUserCreatedNarroValues;
var MMUserUpdatedNarroValues;
var MMUDF1NarrowValues;
var MMUDF2NarrowValues;
var MMUDF3NarrowValues;
var MMUDF4NarrowValues;
var MMUDF5NarrowValues;
var MMUDF6NarrowValues;
var MMUDF7NarrowValues;
var MMUDF8NarrowValues;
var MMUDF9NarrowValues;
var MMUDF10NarrowValues;

function ExpandNarrowSearchMM() {   
    if ($('#dvMinMaxAnalysis .IteamBlock').length > 0) {
        var w = $('#dvMinMaxAnalysis .IteamBlock').css("width");
        $('#dvMinMaxAnalysis .IteamBlock').show();
        $('#dvMinMaxAnalysis .IteamBlock').stop().animate({
            //width: "18%"
        }, 0, function () {
            $('#dvMinMaxAnalysis .userContent').css({ "width": "80.5%", "margin": "0" });
            $('#MinMaxTable_length').css({ "left": "0px" });
            $('#MinMaxTable_paginate').css({ "left": "145px" });
            $('#dvMinMaxAnalysis .leftopenContent').css({ "display": "none" });
           // $('#dvMinMaxAnalysis .searchWrapper').css({ "width": "80%" });
            setCookie('NarrowSearchStateMM', 'Collapsed');
        });
    }
}

function CollapseNarrowSearchMM() {
    if ($('#dvMinMaxAnalysis .IteamBlock').length > 0) {
        $('#dvMinMaxAnalysis .IteamBlock').stop().animate({
            //width: "0%"
        }, 0, function () {
            $('#dvMinMaxAnalysis .IteamBlock').hide();
            $('#dvMinMaxAnalysis .userContent').css({ "width": "98.5%", margin: "0 0.4% 1%" });
            var Left = $('.viewBlock').css("width");
           // $('#MinMaxTable_length').css({ "left": Left });
            var LeftW = 145 + parseInt(Left);
            //$('#MinMaxTable_paginate').css({ "left": LeftW + 'px' });
            if (typeof (oTableItemModel) != "undefined") {
                oTableItemModel.fnAdjustColumnSizing();
            }
            $('#dvMinMaxAnalysis .leftopenContent').css({ "display": "" });
           // $('#dvMinMaxAnalysis .searchWrapper').css({ "width": "98.5%" });
            setCookie('NarrowSearchStateMM', 'Expanded');
        });
    }
}

function MMSActive(SSDDLObject) {
    if ($(SSDDLObject).val() != "") {
        MMItemActiveNarroSearchValue = $(SSDDLObject).val();
        DoMMNarrowSearch();
    }
    else {
        MMItemActiveNarroSearchValue = '';
        DoMMNarrowSearch();
    }
}
function MMCostNarroSearch(CostDDLObject) {
    if ($(CostDDLObject).val() != "0_-1") {
        MMCostNarroSearchValue = $(CostDDLObject).val();
        DoMMNarrowSearch();
    }
    else {
        MMCostNarroSearchValue = '';
        DoMMNarrowSearch();
    }
}
function MMAverageCostNarroSearch(AverageCostDDLObject) {
    if ($(AverageCostDDLObject).val() != "0_-1") {
        MMAverageCostNarroSearchValue = $(AverageCostDDLObject).val();
        DoMMNarrowSearch();
    }
    else {
        MMAverageCostNarroSearchValue = '';
        DoMMNarrowSearch();
    }
}

function MMTurnsNarroSearch(TurnsDDLObject) {
    if ($(TurnsDDLObject).val() != "0_-1") {
        MMTurnsNarroSearchValue = $(TurnsDDLObject).val();
        DoMMNarrowSearch();
    }
    else {
        MMTurnsNarroSearchValue = '';
        DoMMNarrowSearch();
    }
}

function GetMMNarroHTMLForUDF(tableName, companyID, roomID, _IsArchived, _IsDeleted) {    
    var UDFObject;
    $("select[name='udflist']").each(function (index) {

        var UDFUniqueID = this.getAttribute('UID');        
        $.ajax({
            'url': '/Master/GetUDFDDData',
            data: { TableName: tableName, UDFName: this.id, UDFUniqueID: UDFUniqueID, IsArchived: _IsArchived, IsDeleted: _IsDeleted},
            success: function (response) {
                var s = '';
                if (response.DDData != null) {
                    $.each(response.DDData, function (UDFVal, ValCount) {
                        s += '<option value="' + UDFVal + '">' + UDFVal + ' (' + ValCount + ')' + '</option>';
                    });
                }

                var UDFColumnNameTemp = response.UDFColName.toString().replace("_dd", "");

                //Destroy widgets before reapplying the filter
                //$('[id="' + response.UDFColName + '"]').empty();
                //$('[id="' + response.UDFColName + '"]').multiselect('destroy');
                //$('[id="' + response.UDFColName + '"]').multiselectfilter('destroy');

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
                            var CollapseObject = $('#' + UDFUniqueID + 'Collapse')
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
                    var CollapseObject = $('#' + UDFUniqueID + 'Collapse')
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
                        MMUDF1NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                            return escape(input.value);
                        })
                    }
                    else if (UDFUniqueID == "UDF2") {
                        MMUDF2NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                            return escape(input.value);
                        })
                    }
                    else if (UDFUniqueID == "UDF3") {
                        MMUDF3NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                            return escape(input.value);

                        })
                    }
                    else if (UDFUniqueID == "UDF4") {
                        MMUDF4NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                            return escape(input.value);
                        })
                    }
                    else if (UDFUniqueID == "UDF5") {
                        MMUDF5NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                            return escape(input.value);
                        })
                    }
                    else if (UDFUniqueID == "UDF6") {
                        MMUDF6NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                            return escape(input.value);
                        })
                    }
                    else if (UDFUniqueID == "UDF7") {
                        MMUDF7NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                            return escape(input.value);
                        })
                    }
                    else if (UDFUniqueID == "UDF8") {
                        MMUDF8NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                            return escape(input.value);
                        })
                    }
                    else if (UDFUniqueID == "UDF9") {
                        MMUDF9NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                            return escape(input.value);
                        })
                    }
                    else if (UDFUniqueID == "UDF10") {
                        MMUDF10NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
                            return escape(input.value);
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
                    DoMMNarrowSearch();
                }).multiselectfilter();
            },
            error: function (response) {

                // through errror message
            }
        });
    });
}

function GetItemMMNarrowSearchData() {

        $.ajax({
            'url': '/Master/GetMinMaxNarrowDDData',
            data: { TextFieldName: 'Supplier'},
            success: function (response) {
                var s = '';
                if (response.DDData != null) {
                    $.each(response.DDData, function (ValData, ValCount) {
                        s += '<option value="' + ValData + '">' + ValData + ' (' + ValCount + ')' + '</option>';
                    });
                }

                //Destroy widgets before reapplying the filter
                //$("#MMItemSupplier").empty();
                //$("#MMItemSupplier").multiselect('destroy');
                //$("#MMItemSupplier").multiselectfilter('destroy');

                $("#MMItemSupplier").append(s);
                $("#MMItemSupplier").multiselect
                (
                    {
                        noneSelectedText: Supplier, selectedList: 5,
                        selectedText: function (numChecked, numTotal, checkedItems) {
                            return Supplier + ' ' + numChecked + selected;
                        }
                    },
                    {
                        checkAll: function (ui) {
                            $("#MMItemSupplierCollapse").html('');
                            for (var i = 0; i <= ui.target.length - 1; i++) {
                                if ($("#MMItemSupplierCollapse").text().indexOf(ui.target[i].text) == -1) {
                                    $("#MMItemSupplierCollapse").append("<span>" + ui.target[i].text + "</span>");
                                }
                            }
                            $("#MMItemSupplierCollapse").show();
                        }
                    }
                ).unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                 .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                    if (ui.checked) {
                        if ($("#MMItemSupplierCollapse").text().indexOf(ui.text) == -1) {
                            $("#MMItemSupplierCollapse").append("<span>" + ui.text + "</span>");
                        }
                    }
                    else {
                        if (ui.checked == undefined) {
                            $("#MMItemSupplierCollapse").html('');
                        }
                        else if (!ui.checked) {
                            var text = $("#MMItemSupplierCollapse").html();
                            text = text.replace("<span>" + ui.text + "</span>", '');
                            $("#MMItemSupplierCollapse").html(text);
                        }
                        else
                            $("#MMItemSupplierCollapse").html('');
                    }                    
                    MMSupplierNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                        return input.value;
                    });

                    if ($("#MMItemSupplierCollapse").text().trim() != '')
                        $("#MMItemSupplierCollapse").show();
                    else
                        $("#MMItemSupplierCollapse").hide();


                    if ($("#MMItemSupplierCollapse").find('span').length <= 2) {
                        $("#MMItemSupplierCollapse").scrollTop(0).height(50);
                    }
                    else {
                        $("#MMItemSupplierCollapse").scrollTop(0).height(100);
                    }
                    DoMMNarrowSearch();
                }).multiselectfilter();
            },
            error: function (response) {
                // through errror message
            }
        });

        $.ajax({
            'url': '/Master/GetMinMaxNarrowDDData',
            data: { TextFieldName: 'Category' },
            success: function (response) {
                var s = '';
                if (response.DDData != null) {
                    $.each(response.DDData, function (ValData, ValCount) {
                        s += '<option value="' + ValData + '">' + ValData + ' (' + ValCount + ')' + '</option>';
                    });
                }

                //Destroy widgets before reapplying the filter
                //$("#MMItemCategory").empty();
                //$("#MMItemCategory").multiselect('destroy');
                //$("#MMItemCategory").multiselectfilter('destroy');

                $("#MMItemCategory").append(s);
                $("#MMItemCategory").multiselect
                (
                    {
                        noneSelectedText: Category, selectedList: 5,
                        selectedText: function (numChecked, numTotal, checkedItems) {
                            return Category + ' ' + numChecked + selected;
                        }
                    },
                    {
                        checkAll: function (ui) {
                            $("#MMItemCategoryCollapse").html('');
                            for (var i = 0; i <= ui.target.length - 1; i++) {
                                if ($("#MMItemCategoryCollapse").text().indexOf(ui.target[i].text) == -1) {
                                    $("#MMItemCategoryCollapse").append("<span>" + ui.target[i].text + "</span>");
                                }
                            }
                            $("#MMItemCategoryCollapse").show();
                        }
                    }
                ).unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                    if (ui.checked) {
                        if ($("#MMItemCategoryCollapse").text().indexOf(ui.text) == -1) {
                            $("#MMItemCategoryCollapse").append("<span>" + ui.text + "</span>");
                        }
                    }
                    else {
                        if (ui.checked == undefined) {
                            $("#MMItemCategoryCollapse").html('');
                        }
                        else if (!ui.checked) {
                            var text = $("#MMItemCategoryCollapse").html();
                            text = text.replace("<span>" + ui.text + "</span>", '');
                            $("#MMItemCategoryCollapse").html(text);
                        }
                        else
                            $("#MMItemCategoryCollapse").html('');
                    }
                    MMCategoryNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                        return input.value;
                    })

                    if ($("#MMItemCategoryCollapse").text().trim() != '')
                        $("#MMItemCategoryCollapse").show();
                    else
                        $("#MMItemCategoryCollapse").hide();

                    if ($("#MMItemCategoryCollapse").find('span').length <= 2) {
                        $("#MMItemCategoryCollapse").scrollTop(0).height(50);
                    }
                    else {
                        $("#MMItemCategoryCollapse").scrollTop(0).height(100);
                    }
                    DoMMNarrowSearch();
                }).multiselectfilter();
            },
            error: function (response) {
                // through errror message
            }
        });

        $.ajax({
            'url': '/Master/GetMinMaxNarrowDDData',
            data: { TextFieldName: 'Manufacturer' },
            success: function (response) {                
                var s = '';
                if (response.DDData != null) {
                    $.each(response.DDData, function (ValData, ValCount) {
                        s += '<option value="' + ValData + '">' + ValData + ' (' + ValCount + ')' + '</option>';
                    });
                }

                //Destroy widgets before reapplying the filter
                //$("#MMItemManufacturer").empty();
                //$("#MMItemManufacturer").multiselect('destroy');
                //$("#MMItemManufacturer").multiselectfilter('destroy');

                $("#MMItemManufacturer").append(s);
                $("#MMItemManufacturer").multiselect
                (
                    {
                        noneSelectedText: Manufacturer, selectedList: 5,
                        selectedText: function (numChecked, numTotal, checkedItems) {
                            return Manufacturer + ' ' + numChecked + ' ' + selected;
                        }
                    },
                    {
                        checkAll: function (ui) {
                            $("#MMItemManufacturerCollapse").html('');
                            for (var i = 0; i <= ui.target.length - 1; i++) {
                                if ($("#MMItemManufacturerCollapse").text().indexOf(ui.target[i].text) == -1) {
                                    $("#MMItemManufacturerCollapse").append("<span>" + ui.target[i].text + "</span>");
                                }
                            }
                            $("#MMItemManufacturerCollapse").show();
                        }
                    }
                ).unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                    .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                    if (ui.checked) {
                        if ($("#MMItemManufacturerCollapse").text().indexOf(ui.text) == -1) {
                            $("#MMItemManufacturerCollapse").append("<span>" + ui.text + "</span>");
                        }
                    }
                    else {
                        if (ui.checked == undefined) {
                            $("#MMItemManufacturerCollapse").html('');
                        }
                        else if (!ui.checked) {
                            var text = $("#MMItemManufacturerCollapse").html();
                            text = text.replace("<span>" + ui.text + "</span>", '');
                            $("#MMItemManufacturerCollapse").html(text);
                        }
                        else
                            $("#MMItemManufacturerCollapse").html('');
                    }
                    MMManufacturerNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                        return input.value;
                    })

                    if ($("#MMItemManufacturerCollapse").text().trim() != '')
                        $("#MMItemManufacturerCollapse").show();
                    else
                        $("#MMItemManufacturerCollapse").hide();


                    if ($("#MMItemManufacturerCollapse").find('span').length <= 2) {
                        $("#MMItemManufacturerCollapse").scrollTop(0).height(50);
                    }
                    else {
                        $("#MMItemManufacturerCollapse").scrollTop(0).height(100);
                    }                    
                    DoMMNarrowSearch();
                }).multiselectfilter();
            },
            error: function (response) {
                // through errror message
            }
        });

        $.ajax({
            'url': '/Master/GetMinMaxNarrowDDData',
            data: { TextFieldName: 'ItemLocation' },
            success: function (response) {
                var s = '';
                if (response.DDData != null) {
                    $.each(response.DDData, function (ValData, ValCount) {
                        s += '<option value="' + ValData + '">' + ValData + ' (' + ValCount + ')' + '</option>';
                    });
                }

                //Destroy widgets before reapplying the filter
                //$("#MMItemLocation").empty();
                //$("#MMItemLocation").multiselect('destroy');
                //$("#MMItemLocation").multiselectfilter('destroy');

                $("#MMItemLocation").append(s);
                $("#MMItemLocation").multiselect
                (
                    {
                        noneSelectedText: ItemLocation, selectedList: 5,
                        selectedText: function (numChecked, numTotal, checkedItems) {
                            return ItemLocation + ' ' + numChecked + selected;
                        }
                    },
                    {
                        checkAll: function (ui) {
                            $("#MMItemLocationCollapse").html('');
                            for (var i = 0; i <= ui.target.length - 1; i++) {
                                if ($("#MMItemLocationCollapse").text().indexOf(ui.target[i].text) == -1) {
                                    $("#MMItemLocationCollapse").append("<span>" + ui.target[i].text + "</span>");
                                }
                            }
                            $("#MMItemLocationCollapse").show();
                        }
                    }
                ).unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                    if (ui.checked) {
                        if ($("#MMItemLocationCollapse").text().indexOf(ui.text) == -1) {
                            $("#MMItemLocationCollapse").append("<span>" + ui.text + "</span>");
                        }
                    }
                    else {
                        if (ui.checked == undefined) {
                            $("#MMItemLocationCollapse").html('');
                        }
                        else if (!ui.checked) {
                            var text = $("#MMItemLocationCollapse").html();
                            text = text.replace("<span>" + ui.text + "</span>", '');
                            $("#MMItemLocationCollapse").html(text);
                        }
                        else
                            $("#MMItemLocationCollapse").html('');
                    }
                    MMItemLocationNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                        return input.value;
                    })

                    if ($("#MMItemLocationCollapse").text().trim() != '')
                        $("#MMItemLocationCollapse").show();
                    else
                        $("#MMItemLocationCollapse").hide();


                    if ($("#MMItemLocationCollapse").find('span').length <= 2) {
                        $("#MMItemLocationCollapse").scrollTop(0).height(50);
                    }
                    else {
                        $("#MMItemLocationCollapse").scrollTop(0).height(100);
                    }
                    DoMMNarrowSearch();
                }).multiselectfilter();
            },
            error: function (response) {
                // through errror message
            }
        });

        $.ajax({
            'url': '/Master/GetMinMaxNarrowDDData',
            data: { TextFieldName: 'ItemTrackingType' },
            success: function (response) {                
                var s = '';
                if (response.DDData != null) {
                    $.each(response.DDData, function (i, val) {
                        if (i == "1")
                            s += '<option value="' + i + '"> No Tracking (' + val + ')' + '</option>';
                        if (i == "2")
                            s += '<option value="' + i + '"> Serial # Tracking (' + val + ')' + '</option>';
                        if (i == "3")
                            s += '<option value="' + i + '"> Lot # Tracking (' + val + ')' + '</option>';
                        if (i == "4")
                            s += '<option value="' + i + '"> Expiration Date Tracking (' + val + ')' + '</option>';
                    });
                }

                //Destroy widgets before reapplying the filter
                //$("#MMItemTrackingType").empty();
                //$("#MMItemTrackingType").multiselect('destroy');
                //$("#MMItemTrackingType").multiselectfilter('destroy');

                $("#MMItemTrackingType").append(s);
                $("#MMItemTrackingType").multiselect
                (
                    {
                        noneSelectedText: ItemTrackingType, selectedList: 5,
                        selectedText: function (numChecked, numTotal, checkedItems) {
                            return ItemTrackingType + ' ' + numChecked + selected;
                        }
                    },
                    {
                        checkAll: function (ui) {
                            $("#MMItemTrackingTypeCollapse").html('');
                            for (var i = 0; i <= ui.target.length - 1; i++) {
                                if ($("#MMItemTrackingTypeCollapse").text().indexOf(ui.target[i].text) == -1) {
                                    $("#MMItemTrackingTypeCollapse").append("<span>" + ui.target[i].text + "</span>");
                                }
                            }
                            $("#MMItemTrackingTypeCollapse").show();
                        }
                    }
                ).unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                    if (ui.checked) {
                        if ($("#MMItemTrackingTypeCollapse").text().indexOf(ui.text) == -1) {
                            $("#MMItemTrackingTypeCollapse").append("<span>" + ui.text + "</span>");
                        }
                    }
                    else {
                        if (ui.checked == undefined) {
                            $("#MMItemTrackingTypeCollapse").html('');
                        }
                        else if (!ui.checked) {
                            var text = $("#MMItemTrackingTypeCollapse").html();
                            text = text.replace("<span>" + ui.text + "</span>", '');
                            $("#MMItemTrackingTypeCollapse").html(text);
                        }
                        else
                            $("#MMItemTrackingTypeCollapse").html('');
                    }
                    MMItemTrackingTypeNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                        return input.value;
                    })

                    if ($("#MMItemTrackingTypeCollapse").text().trim() != '')
                        $("#MMItemTrackingTypeCollapse").show();
                    else
                        $("#MMItemTrackingTypeCollapse").hide();


                    if ($("#MMItemTrackingTypeCollapse").find('span').length <= 2) {
                        $("#MMItemTrackingTypeCollapse").scrollTop(0).height(50);
                    }
                    else {
                        $("#MMItemTrackingTypeCollapse").scrollTop(0).height(100);
                    }
                    DoMMNarrowSearch();
                }).multiselectfilter();
            },
            error: function (response) {
                // through errror message
            }
        });

        $.ajax({
            'url': '/Master/GetMinMaxNarrowDDData',
            data: { TextFieldName: 'ItemStockStatus' },
            success: function (response) {                
                var s = '';
                if (response.DDData != null) {
                    $.each(response.DDData, function (i, val) {
                        if (i == "1")
                            s += '<option value="' + i + '"> Out of Stock (' + val + ')' + '</option>';
                        if (i == "2")
                            s += '<option value="' + i + '"> Below Critical (' + val + ')' + '</option>';
                        if (i == "3")
                            s += '<option value="' + i + '"> Below Min (' + val + ')' + '</option>';
                        if (i == "4")
                            s += '<option value="' + i + '"> Above Max (' + val + ')' + '</option>';
                    });
                }

                //Destroy widgets before reapplying the filter
                $("#MMItemStockStatus").empty();
                $("#MMItemStockStatus").multiselect('destroy');
                $("#MMItemStockStatus").multiselectfilter('destroy');

                $("#MMItemStockStatus").append(s);
                $("#MMItemStockStatus")[0].selectedIndex = -1;
                $("#MMItemStockStatus").multiselect("refresh");
                $("#MMItemStockStatus").multiselectfilter("refresh");
                $("#MMItemStockStatus").multiselect
                (
                    {
                        noneSelectedText: ItemStockStatus, selectedList: 5,
                        selectedText: function (numChecked, numTotal, checkedItems) {
                            return ItemStockStatus + ' ' + numChecked + selected;
                        },
                        header: false
                    },
                    {
                        checkAll: function (ui) {
                            $("#MMItemStockStatusCollapse").html('');
                            for (var i = 0; i <= ui.target.length - 1; i++) {
                                if ($("#MMItemStockStatusCollapse").text().indexOf(ui.target[i].text) == -1) {
                                    $("#MMItemStockStatusCollapse").append("<span>" + ui.target[i].text + "</span>");
                                }
                            }
                            $("#MMItemStockStatusCollapse").show();
                        }
                    }
                ).unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                    if (ui.checked) {
                        var uiLastest = ui;
                        $("#MMItemStockStatus").multiselect("uncheckAll");
                        $("#MMItemStockStatusCollapse").html('');

                        ui = uiLastest;
                        var $el = $("#MMItemStockStatus");
                        $el.find('option').each(function () {                           
                            if ($.trim($(this).val()) == $.trim(ui.value)) {
                                $("#MMItemStockStatus").multiselect("widget").find(":checkbox[value='" + $(this).val() + "']").attr("checked", "checked");
                            }
                        });

                        //$("ul.ui-multiselect-checkboxes li").each(function () {
                        //    $("#MMItemStockStatus").find('span').each(function (e, obj) {
                        //        if ($.trim($(obj).text()) == $.trim(ui.text)) {
                        //            $(this).find("input[type=checkbox]").attr("checked", "checked");
                        //        }
                        //    });
                        //});

                        MMItemStockStatusNarroValues = ui.value;
                        if ($("#MMItemStockStatusCollapse").text().indexOf(ui.text) == -1) {
                            $("#MMItemStockStatusCollapse").append("<span>" + ui.text + "</span>");
                        }
                    }
                    else {
                        MMItemStockStatusNarroValues = '';
                        if (ui.checked == undefined) {
                            $("#MMItemStockStatusCollapse").html('');
                        }
                        else if (!ui.checked) {
                            var text = $("#MMItemStockStatusCollapse").html();
                            text = text.replace("<span>" + ui.text + "</span>", '');
                            $("#MMItemStockStatusCollapse").html(text);
                        }
                        else
                            $("#MMItemStockStatusCollapse").html('');
                    }
                    if ($("#MMItemStockStatusCollapse").text().trim() != '')
                        $("#MMItemStockStatusCollapse").show();
                    else
                        $("#MMItemStockStatusCollapse").hide();

                    if ($("#MMItemStockStatusCollapse").find('span').length <= 2) {
                        $("#MMItemStockStatusCollapse").scrollTop(0).height(50);
                    }
                    else {
                        $("#MMItemStockStatusCollapse").scrollTop(0).height(100);
                    }
                    DoMMNarrowSearch();
                }).multiselectfilter();
            },
            error: function (response) {
                // through errror message
            }
        });

        $.ajax({
            'url': '/Master/GetMinMaxNarrowDDData',
            data: { TextFieldName: 'ItemType' },
            success: function (response) {                
                var s = '';
                $("#MMItemTypeCollapse").html('');
                if (response.DDData != null) {
                    $.each(response.DDData, function (i, val) {
                        if (i == "1")
                            s += '<option value="' + i + '"> Item (' + val + ')' + '</option>';
                        if (i == "2" && gblActionName.toLowerCase() != "itemmasterlist" && gblActionName.toLowerCase() != "itemmasterpictureview")
                            s += '<option value="' + i + '"> Quick List (' + val + ')' + '</option>';
                        if (i == "3")
                            s += '<option value="' + i + '"> Kit (' + val + ')' + '</option>';
                        if (i == "4")
                            s += '<option value="' + i + '"> Labor (' + val + ')' + '</option>';
                    });
                }
                //Destroy widgets before reapplying the filter
                //$("#MMItemType").empty();
                //$("#MMItemType").multiselect('destroy');
                //$("#MMItemType").multiselectfilter('destroy');

                $("#MMItemType").append(s);
                $("#MMItemType").multiselect
                (
                    {
                        noneSelectedText: ItemType, selectedList: 5,
                        selectedText: function (numChecked, numTotal, checkedItems) {
                            return ItemType + ' ' + numChecked + ' ' + selected;
                        }
                    },
                    {
                        checkAll: function (ui) {
                            $("#MMItemTypeCollapse").html('');
                            for (var i = 0; i <= ui.target.length - 1; i++) {
                                if ($("#MMItemTypeCollapse").text().indexOf(ui.target[i].text) == -1) {
                                    $("#MMItemTypeCollapse").append("<span>" + ui.target[i].text + "</span>");
                                }
                            }
                            $("#MMItemTypeCollapse").show();
                        }
                    }
                ).unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                    if (ui.checked) {
                        if ($("#MMItemTypeCollapse").text().indexOf(ui.text) == -1) {
                            $("#MMItemTypeCollapse").append("<span>" + ui.text + "</span>");
                        }
                    }
                    else {
                        if (ui.checked == undefined) {
                            $("#MMItemTypeCollapse").html('');
                        }
                        else if (!ui.checked) {
                            var text = $("#MMItemTypeCollapse").html();
                            text = text.replace("<span>" + ui.text + "</span>", '');
                            $("#MMItemTypeCollapse").html(text);
                        }
                        else
                            $("#MMItemTypeCollapse").html('');
                    }
                    MMItemTypeNarroSearchValue = $.map($(this).multiselect("getChecked"), function (input) {
                        return input.value;
                    })

                    if ($("#MMItemTypeCollapse").text().trim() != '')
                        $("#MMItemTypeCollapse").show();
                    else
                        $("#MMItemTypeCollapse").hide();


                    if ($("#MMItemTypeCollapse").find('span').length <= 2) {
                        $("#MMItemTypeCollapse").scrollTop(0).height(50);
                    }
                    else {
                        $("#MMItemTypeCollapse").scrollTop(0).height(100);
                    }                    
                    DoMMNarrowSearch();
                }).multiselectfilter();
            },
            error: function (response) {
                // through errror message
            }
        });

        $.ajax({
            'url': '/Master/GetMinMaxNarrowDDData',
            data: { TextFieldName: 'InventoryClassification' },
            success: function (response) {
                var s = '';
                if (response.DDData != null) {
                    $.each(response.DDData, function (ValData, ValCount) {
                        s += '<option value="' + ValData + '">' + ValData + ' (' + ValCount + ')' + '</option>';
                    });
                }

                //Destroy widgets before reapplying the filter
                //$("#MMInventoryClassification").empty();
                //$("#MMInventoryClassification").multiselect('destroy');
                //$("#MMInventoryClassification").multiselectfilter('destroy');

                $("#MMInventoryClassification").append(s);
                $("#MMInventoryClassification").multiselect
                (
                    {
                        noneSelectedText: ItemTypeInventoryClassification, selectedList: 5,
                        selectedText: function (numChecked, numTotal, checkedItems) {
                            return ItemTypeInventoryClassification + ' ' + numChecked + selected;
                        }
                    },
                    {
                        checkAll: function (ui) {
                            $("#MMInventoryClassificationCollapse").html('');
                            for (var i = 0; i <= ui.target.length - 1; i++) {
                                if ($("#MMInventoryClassificationCollapse").text().indexOf(ui.target[i].text) == -1) {
                                    $("#MMInventoryClassificationCollapse").append("<span>" + ui.target[i].text + "</span>");
                                }
                            }
                            $("#MMInventoryClassificationCollapse").show();
                        }
                    }
                ).unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                    if (ui.checked) {
                        if ($("#MMInventoryClassificationCollapse").text().indexOf(ui.text) == -1) {
                            $("#MMInventoryClassificationCollapse").append("<span>" + ui.text + "</span>");
                        }
                    }
                    else {
                        if (ui.checked == undefined) {
                            $("#MMInventoryClassificationCollapse").html('');
                        }
                        else if (!ui.checked) {
                            var text = $("#MMInventoryClassificationCollapse").html();
                            text = text.replace("<span>" + ui.text + "</span>", '');
                            $("#MMInventoryClassificationCollapse").html(text);
                        }
                        else
                            $("#MMInventoryClassificationCollapse").html('');
                    }
                    MMInventoryClassificationNarroSearchValue = $.map($(this).multiselect("getChecked"), function (input) {
                        return input.value;
                    })

                    if ($("#MMInventoryClassificationCollapse").text().trim() != '')
                        $("#MMInventoryClassificationCollapse").show();
                    else
                        $("#MMInventoryClassificationCollapse").hide();


                    if ($("#MMInventoryClassificationCollapse").find('span').length <= 2) {
                        $("#MMInventoryClassificationCollapse").scrollTop(0).height(50);
                    }
                    else {
                        $("#MMInventoryClassificationCollapse").scrollTop(0).height(100);
                    }
                    DoMMNarrowSearch();
                }).multiselectfilter();
            },
            error: function (response) {
                // through errror message
            }
        });

        $.ajax({
            'url': '/Master/GetMinMaxNarrowDDData',
            data: { TextFieldName: 'CreatedByUser' },
            success: function (response) {
                var s = '';
                if (response.DDData != null) {
                    $.each(response.DDData, function (ValData, ValCount) {
                        s += '<option value="' + ValData + '">' + ValData + ' (' + ValCount + ')' + '</option>';
                    });
                }

                $("#UserCreatedIM").append(s);
                $("#UserCreatedIM").multiselect
                (
                {
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
            ).unbind("multiselectclick multiselectcheckall multiselectuncheckall")
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
                MMUserCreatedNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                    return input.value;
                })

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
                DoMMNarrowSearch();
            }).multiselectfilter();
            },
            error: function (response) {
                // through errror message
            }
        });

        $.ajax({
            'url': '/Master/GetMinMaxNarrowDDData',
            data: { TextFieldName: 'LastUpdatedByUser' },
            success: function (response) {
                var s = '';
                if (response.DDData != null) {
                    $.each(response.DDData, function (ValData, ValCount) {
                        s += '<option value="' + ValData + '">' + ValData + ' (' + ValCount + ')' + '</option>';
                    });
                }

                $("#UserUpdatedIM").append(s);
                $("#UserUpdatedIM").multiselect
                (
                {
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
            ).unbind("multiselectclick multiselectcheckall multiselectuncheckall")
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
                MMUserUpdatedNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                    return input.value;
                })

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
                DoMMNarrowSearch();
            }).multiselectfilter();
            },
            error: function (response) {
                // through errror message
            }
        });
}

function DoMMNarrowSearch() {
    globalSearchMinMaxTuning = true;
    var MMnarrowSearchFields = '';
    var MMnarrowSearchValues = '';

    //eraseCookieforshift("selectstartindex");
    //eraseCookieforshift("selectendindex");
    //eraseCookieforshift("finalselectedarray");
////////////0///////////
    if (MMSupplierNarroValues != undefined && MMSupplierNarroValues.length > 0) {
        MMnarrowSearchFields += "SupplierName" + ",";
        MMnarrowSearchValues += MMSupplierNarroValues + "@";
    }
    else {
        MMnarrowSearchFields += "SupplierName" + ",";
        MMnarrowSearchValues += "@";
    }
    ////////////1///////////
    if (MMCategoryNarroValues != undefined && MMCategoryNarroValues.length > 0) {
        MMnarrowSearchFields += "Category" + ",";
        MMnarrowSearchValues += MMCategoryNarroValues + "@";
    }
    else {
        MMnarrowSearchFields += "Category" + ",";
        MMnarrowSearchValues += "@";
    }
    ////////////2///////////
    if (MMManufacturerNarroValues != undefined && MMManufacturerNarroValues.length > 0) {
        MMnarrowSearchFields += "Manufacturer" + ",";
        MMnarrowSearchValues += MMManufacturerNarroValues + "@";
    }
    else {
        MMnarrowSearchFields += "Manufacturer" + ",";
        MMnarrowSearchValues += "@";
    }
    ////////////3///////////
    if (MMItemLocationNarroValues != undefined && MMItemLocationNarroValues.length > 0) {
        MMnarrowSearchFields += "BinNumber" + ",";
        MMnarrowSearchValues += MMItemLocationNarroValues + "@";
    }
    else {
        MMnarrowSearchFields += "BinNumber" + ",";
        MMnarrowSearchValues += "@";
    }
    ////////////4///////////
    if (MMItemTrackingTypeNarroValues != undefined && MMItemTrackingTypeNarroValues.length > 0) {
        MMnarrowSearchFields += "ItemTrackingType" + ",";
        MMnarrowSearchValues += MMItemTrackingTypeNarroValues + "@";
    }
    else {
        MMnarrowSearchFields += "ItemTrackingType" + ",";
        MMnarrowSearchValues += "@";
    }
    ////////////5///////////
    if (MMItemStockStatusNarroValues != undefined && MMItemStockStatusNarroValues.length > 0) {
        MMnarrowSearchFields += "ItemStockStatus" + ",";
        MMnarrowSearchValues += MMItemStockStatusNarroValues + "@";
    }
    else {
        MMnarrowSearchFields += "ItemStockStatus" + ",";
        MMnarrowSearchValues += "@";
    }
    ////////////6///////////
    if (MMItemTypeNarroSearchValue != undefined && MMItemTypeNarroSearchValue.length > 0) {
        MMnarrowSearchFields += "ItemTypeValue" + ",";
        MMnarrowSearchValues += MMItemTypeNarroSearchValue + "@";
    }
    else {
        MMnarrowSearchFields += "ItemTypeValue" + ",";
        MMnarrowSearchValues += "@";
    }
    ////////////7///////////
    if (MMInventoryClassificationNarroSearchValue != undefined && MMInventoryClassificationNarroSearchValue.length > 0) {
        MMnarrowSearchFields += "InventoryClassification" + ",";
        MMnarrowSearchValues += MMInventoryClassificationNarroSearchValue + "@";
    }
    else {
        MMnarrowSearchFields += "InventoryClassification" + ",";
        MMnarrowSearchValues += "@";
    }
    ////////////8///////////
    if (MMItemActiveNarroSearchValue != undefined && MMItemActiveNarroSearchValue.length > 0) {
        MMnarrowSearchFields += "ItemActive" + ",";
        MMnarrowSearchValues += MMItemActiveNarroSearchValue + "@";
    }
    else {
        MMnarrowSearchFields += "ItemActive" + ",";
        MMnarrowSearchValues += "@";
    }
    ////////////9///////////
    if (MMCostNarroSearchValue != undefined && MMCostNarroSearchValue.length > 0) {
        MMnarrowSearchFields += "Cost" + ",";
        MMnarrowSearchValues += MMCostNarroSearchValue + "@";
    }
    else {
        MMnarrowSearchFields += "Cost" + ",";
        MMnarrowSearchValues += "@";
    }
    ////////////10///////////
    if (MMAverageCostNarroSearchValue != undefined && MMAverageCostNarroSearchValue.length > 0) {
        MMnarrowSearchFields += "AverageCost" + ",";
        MMnarrowSearchValues += MMAverageCostNarroSearchValue + "@";
    }
    else {
        MMnarrowSearchFields += "AverageCost" + ",";
        MMnarrowSearchValues += "@";
    }
    ////////////11///////////
    if (MMTurnsNarroSearchValue != undefined && MMTurnsNarroSearchValue.length > 0) {
        MMnarrowSearchFields += "Turns" + ",";
        MMnarrowSearchValues += MMTurnsNarroSearchValue + "@";
    }
    else {
        MMnarrowSearchFields += "Turns" + ",";
        MMnarrowSearchValues += "@";
    }
    ////////////12///////////
    if (MMUDF1NarrowValues != undefined && MMUDF1NarrowValues.length > 0) {
        MMnarrowSearchFields += "UDF1" + ",";
        MMnarrowSearchValues += MMUDF1NarrowValues + "@";
    }
    else {
        MMnarrowSearchFields += "UDF1" + ",";
        MMnarrowSearchValues += "@";
    }
    ////////////13///////////
    if (MMUDF2NarrowValues != undefined && MMUDF2NarrowValues.length > 0) {
        MMnarrowSearchFields += "UDF2" + ",";
        MMnarrowSearchValues += MMUDF2NarrowValues + "@";
    }
    else {
        MMnarrowSearchFields += "UDF2" + ",";
        MMnarrowSearchValues += "@";
    }
    ////////////14///////////
    if (MMUDF3NarrowValues != undefined && MMUDF3NarrowValues.length > 0) {
        MMnarrowSearchFields += "UDF3" + ",";
        MMnarrowSearchValues += MMUDF3NarrowValues + "@";
    }
    else {
        MMnarrowSearchFields += "UDF3" + ",";
        MMnarrowSearchValues += "@";
    }
    ////////////15///////////
    if (MMUDF4NarrowValues != undefined && MMUDF4NarrowValues.length > 0) {
        MMnarrowSearchFields += "UDF4" + ",";
        MMnarrowSearchValues += MMUDF4NarrowValues + "@";
    }
    else {
        MMnarrowSearchFields += "UDF4" + ",";
        MMnarrowSearchValues += "@";
    }
    ////////////16///////////
    if (MMUDF5NarrowValues != undefined && MMUDF5NarrowValues.length > 0) {
        MMnarrowSearchFields += "UDF5" + ",";
        MMnarrowSearchValues += MMUDF5NarrowValues + "@";
    }
    else {
        MMnarrowSearchFields += "UDF5" + ",";
        MMnarrowSearchValues += "@";
    }
    ////////////17///////////
    if (MMUDF6NarrowValues != undefined && MMUDF6NarrowValues.length > 0) {
        MMnarrowSearchFields += "UDF6" + ",";
        MMnarrowSearchValues += MMUDF6NarrowValues + "@";
    }
    else {
        MMnarrowSearchFields += "UDF6" + ",";
        MMnarrowSearchValues += "@";
    }
    ////////////18///////////
    if (MMUDF7NarrowValues != undefined && MMUDF7NarrowValues.length > 0) {
        MMnarrowSearchFields += "UDF7" + ",";
        MMnarrowSearchValues += MMUDF7NarrowValues + "@";
    }
    else {
        MMnarrowSearchFields += "UDF7" + ",";
        MMnarrowSearchValues += "@";
    }
    ////////////19///////////
    if (MMUDF8NarrowValues != undefined && MMUDF8NarrowValues.length > 0) {
        MMnarrowSearchFields += "UDF8" + ",";
        MMnarrowSearchValues += MMUDF8NarrowValues + "@";
    }
    else {
        MMnarrowSearchFields += "UDF8" + ",";
        MMnarrowSearchValues += "@";
    }
    ////////////20///////////
    if (MMUDF9NarrowValues != undefined && MMUDF9NarrowValues.length > 0) {
        MMnarrowSearchFields += "UDF9" + ",";
        MMnarrowSearchValues += MMUDF9NarrowValues + "@";
    }
    else {
        MMnarrowSearchFields += "UDF9" + ",";
        MMnarrowSearchValues += "@";
    }
    ////////////21///////////
    if (MMUDF10NarrowValues != undefined && MMUDF10NarrowValues.length > 0) {
        MMnarrowSearchFields += "UDF10" + ",";
        MMnarrowSearchValues += MMUDF10NarrowValues + "@";
    }
    else {
        MMnarrowSearchFields += "UDF10" + ",";
        MMnarrowSearchValues += "@";
    }
    ////////////22///////////
    if (MMUserCreatedNarroValues != undefined && MMUserCreatedNarroValues.length > 0) {        
        MMnarrowSearchFields += "CreatedBy" + ",";
        MMnarrowSearchValues += MMUserCreatedNarroValues + "@";
    }
    else {
        MMnarrowSearchFields += "CreatedBy" + ",";
        MMnarrowSearchValues += "@";
    }
    /////////////23///////////////////////////////////////////////////////////////////////////////////
    if (MMUserUpdatedNarroValues != undefined && MMUserUpdatedNarroValues.length > 0) {
        MMnarrowSearchFields += "UpdatedBy" + ",";
        MMnarrowSearchValues += MMUserUpdatedNarroValues + "@";
    }
    else {
        MMnarrowSearchFields += "UpdatedBy" + ",";
        MMnarrowSearchValues += "@";
    }
    ///////////////24////////////////////////////////////////////////////////////////////////////////
    
    if ($('#DateCFromIM').val() != '' && $('#DateCToIM').val() != '') {
        MMnarrowSearchFields += "DateCreatedFrom" + ",";
        MMnarrowSearchValues += ($('#DateCFromIM').val()) + "," + ($('#DateCToIM').val()) + "@";
    }
    else {
        MMnarrowSearchFields += "DateCreatedFrom" + ",";
        MMnarrowSearchValues += "@";
    }
    ////////////////25////////////////////////////////////////////////////////////////////////////////
    if ($('#DateUFromIM').val() != '' && $('#DateUToIM').val() != '') {
        MMnarrowSearchFields += "DateUpdatedFrom" + ",";
        MMnarrowSearchValues += ($('#DateUFromIM').val()) + "," + ($('#DateUToIM').val()) + "@";
    }
    else {
        MMnarrowSearchFields += "DateUpdatedFrom" + ",";
        MMnarrowSearchValues += "@";
    }

    var narrowSearch = MMnarrowSearchFields + "[###]" + MMnarrowSearchValues;
    MMNarrowSearchInGrid(narrowSearch);
}

function MMNarrowSearchInGrid(searchstr) {    
    //global_filterMinMaxTuning = searchstr;
    if ($("#global_filterMinMaxTuning").val() != "") {
        searchstr = searchstr + "[^^^]" + $("#global_filterMinMaxTuning").val().replace(/'/g, "''");
    }
    $('#MinMaxTable').dataTable().fnFilter(searchstr, null, null, null);
}
