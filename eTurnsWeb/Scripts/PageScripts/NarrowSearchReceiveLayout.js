var selected = null;
var Supplier = null;
var dateReceived = null;

var OrderNumver = "Order Number";

//var CreatedBy = "CreatedBy";
//var UpdatedBy = "UpdatedBy";
var RoomResourceName = "Room";

//var ReceiveSupplierNarroValues = "";

//var ReceivePONarroValues = "";
//var POReceiveaDateValues = "";

//var ReceiveCreatedByNarroValues = "";
//var ReceiveUpdatedByNarroValues = "";

var _NarrowSearchReceiveLayout = (function ($) {
    var self = {};

    self.model = { CompanyID: null, RoomID: null, selected: null, Supplier: null, dateReceived: null }

    self.init = function (CompanyID, RoomID, pselected, psupplier, pdateReceived) {
        self.model.CompanyID = CompanyID;
        self.model.RoomID = RoomID;
        self.model.selected = pselected;
        self.model.Supplier = psupplier;
        self.model.dateReceived = pdateReceived;

        selected = self.model.selected;
        Supplier = self.model.Supplier;
        dateReceived = self.model.dateReceived;


        self.initEvents();
    }

    self.initEvents = function () {
        $(document).ready(function () {
            var _IsArchived = false;
            var _IsDeleted = false;

            $('#ReceiveDateCFrom').blur(function () {
            }).datepicker({
                changeMonth: true,
                changeYear: true, dateFormat: RoomDateJSFormat
            });
            $('#ReceiveDateCTo').blur(function () {
            }).datepicker({
                changeMonth: true,
                changeYear: true, dateFormat: RoomDateJSFormat
            });
            $('#ReceiveDateUFrom').blur(function () {
            }).datepicker({
                changeMonth: true,
                changeYear: true, dateFormat: RoomDateJSFormat
            });
            $('#ReceiveDateUTo').blur(function () {
            }).datepicker({
                changeMonth: true,
                changeYear: true, dateFormat: RoomDateJSFormat
            });

            $('#ancReceiveDateCFrom').click(function () {
                $('#ReceiveDateCFrom').focus();
            });
            $('#ancReceiveDateCTo').click(function () {
                $('#ReceiveDateCTo').focus();
            });
            $('#ancReceiveDateUFrom').click(function () {
                $('#ReceiveDateUFrom').focus();
            });
            $('#ancReceiveDateUTo').click(function () {
                $('#ReceiveDateUTo').focus();
            });
            if (window.location.hash.toLowerCase() == "#incomplete") {
                $("#tab4").click();
            }
            self.GetReceiveNarrowSearches('ReceiveMaster', _IsArchived, _IsDeleted);

            $('a.downarrow').click(function (evt) {
                evt.stopPropagation();
                evt.preventDefault();
                $(this).closest('.accordion').find('.dropcontent').slideToggle();
                return false;
            });


            $('#NarroSearchGo').click(function () {
                DoNarrowSearchSC();
            });
            $('#ReceiveDateCFrom,#ReceiveDateCTo').change(function () {

                var DateCFromValid = true;// Date.isValid($('#ReceiveDateCFrom').val(),format);
                var DateCToValid = true;//Date.isValid($('#ReceiveDateCTo').val(),format);

                try {
                    $.datepicker.parseDate(RoomDateJSFormat, $('#ReceiveDateCFrom').val());
                    DateCFromValid = true;
                } catch (e) {
                    DateCFromValid = false;
                }

                try {
                    $.datepicker.parseDate(RoomDateJSFormat, $('#ReceiveDateCTo').val());
                    DateCToValid = true;
                } catch (e) {
                    DateCToValid = false;
                }
                if (DateCFromValid && DateCToValid) {
                    if (!isFromNarrowSearchClear) {
                        DoNarrowSearchSC();
                    }
                }
                else {
                    if (!DateCFromValid)
                        $('#ReceiveDateCFrom').val('');
                    if (!DateCToValid)
                        $('#ReceiveDateCTo').val('');
                }
            });

            $('#ReceiveDateUFrom,#ReceiveDateUTo').change(function () {

                var DateUFromValid = true;// Date.isValid($('#ReceiveDateUFrom').val(),format);
                var DateUToValid = true;//Date.isValid($('#ReceiveDateUTo').val(),format);

                try {
                    $.datepicker.parseDate(RoomDateJSFormat, $('#ReceiveDateUFrom').val());
                    DateUFromValid = true;
                } catch (e) {
                    DateUFromValid = false;
                }

                try {
                    $.datepicker.parseDate(RoomDateJSFormat, $('#ReceiveDateUTo').val());
                    DateUToValid = true;
                } catch (e) {
                    DateUToValid = false;
                }
                if (DateUFromValid && DateUToValid) {
                    if (!isFromNarrowSearchClear) {
                        DoNarrowSearchSC();
                    }
                }
                else {
                    if (!DateUFromValid)
                        $('#ReceiveDateUFrom').val('');
                    if (!DateUToValid)
                        $('#ReceiveDateUTo').val('');
                }
            });
            $('#ReceiveDateCreatedClear').click(function () {
                if ($('#ReceiveDateCFrom').val() != '' || $('#ReceiveDateCTo').val() != '') {
                    $('#ReceiveDateCFrom').val('');
                    $('#ReceiveDateCTo').val('');
                    //NarrowSearchInGrid('');
                    if (!isFromNarrowSearchClear) {
                        DoNarrowSearchSC();
                    }
                }
            });
            $('#ReceiveDateUpdatedClear').click(function () {
                if ($('#ReceiveDateUFrom').val() != '' || $('#ReceiveDateUTo').val() != '') {
                    $('#ReceiveDateUFrom').val('');
                    $('#ReceiveDateUTo').val('');
                    //NarrowSearchInGrid('');
                    if (!isFromNarrowSearchClear) {
                        DoNarrowSearchSC();
                    }
                }
            });
            //CLEAR NARROW SEARCH
            $('#NarroSearchClearSC').click(function () {
                isFromNarrowSearchClear = true;
                _NarrowSearchSave.objSearch.reset();

                if (typeof ($("#ReceivePO").multiselect("getChecked").length) != undefined && $("#ReceivePO").multiselect("getChecked").length > 0) {
                    $("#ReceivePO").multiselect("uncheckAll");
                    $("#ReceivePOCollapse").html('');
                }
                else if (typeof ($("#ReceivePOCollapse")) != undefined) {
                    $("#ReceivePOCollapse").html('');
                    $("#ReceivePOCollapse").hide();
                }
                if (typeof ($("#ReceiveSupplier").multiselect("getChecked").length) != undefined && $("#ReceiveSupplier").multiselect("getChecked").length > 0) {
                    $("#ReceiveSupplier").multiselect("uncheckAll");
                    $("#ReceiveSupplierCollapse").html('');
                }
                else if (typeof ($("#ReceiveSupplierCollapse")) != undefined) {
                    $("#ReceiveSupplierCollapse").html('');
                    $("#ReceiveSupplierCollapse").hide();
                }
                if (typeof ($("#ReceiveCreatedBy").multiselect("getChecked").length) != undefined && $("#ReceiveCreatedBy").multiselect("getChecked").length > 0) {
                    $("#ReceiveCreatedBy").multiselect("uncheckAll");
                    $("#ReceiveCreatedByCollapse").html('');
                }
                else if (typeof ($("#ReceiveCreatedByCollapse")) != undefined) {
                    $("#ReceiveCreatedByCollapse").html('');
                    $("#ReceiveCreatedByCollapse").hide();
                }
                if (typeof ($("#ReceiveUpdatedBy").multiselect("getChecked").length) != undefined && $("#ReceiveUpdatedBy").multiselect("getChecked").length > 0) {
                    $("#ReceiveUpdatedBy").multiselect("uncheckAll");
                    $("#ReceiveUpdatedByCollapse").html('');
                }
                else if (typeof ($("#ReceiveUpdatedByCollapse")) != undefined) {
                    $("#ReceiveUpdatedByCollapse").html('');
                    $("#ReceiveUpdatedByCollapse").hide();
                }
                if (typeof ($("#POReceiveaDate").multiselect("getChecked").length) != undefined && $("#POReceiveaDate").multiselect("getChecked").length > 0) {
                    $("#POReceiveaDate").multiselect("uncheckAll");
                    $("#POReceiveaDateCollapse").html('');
                }
                else if (typeof ($("#POReceiveaDateCollapse")) != undefined) {
                    $("#POReceiveaDateCollapse").html('');
                    $("#POReceiveaDateCollapse").hide();
                }
                if (typeof ($("#POReceiveaDate").multiselect("getChecked").length) != undefined && $("#POReceiveaDate").multiselect("getChecked").length > 0) {
                    $("#POReceiveaDate").multiselect("uncheckAll");
                    $("#POReceiveaDateCollapse").html('');
                }
                else if (typeof ($("#POReceiveaDateCollapse")) != undefined) {
                    $("#POReceiveaDateCollapse").html('');
                    $("#POReceiveaDateCollapse").hide();
                }

                $("select[name='udflist']").each(function (index) {
                    if (typeof ($(this).multiselect("getChecked").length) != undefined && $(this).multiselect("getChecked").length > 0) {
                        var UDFUniqueID = this.getAttribute('UID');
                        $(this).multiselect("uncheckAll");
                        $('#' + UDFUniqueID + 'Collapse').html('');
                        $('#' + UDFUniqueID + 'Collapse').hide();
                    }
                    else if ((typeof (this.getAttribute('UID'))) != undefined) {
                        var UDFUniqueID = this.getAttribute('UID');
                        $('#' + UDFUniqueID + 'Collapse').html('');
                        $('#' + UDFUniqueID + 'Collapse').hide();
                    }
                });
                if ($('#ReceiveDateCFrom').val() != '') $('#ReceiveDateCFrom').val('');
                if ($('#ReceiveDateCTo').val() != '') $('#ReceiveDateCTo').val('');
                if ($('#ReceiveDateUFrom').val() != '') $('#ReceiveDateUFrom').val('');
                if ($('#ReceiveDateUTo').val() != '') $('#ReceiveDateUTo').val('');
                isFromNarrowSearchClear = false;
                if ($('#global_filter').val() != '') $('#global_filter').val('');

                $('#myDataTable tbody tr').removeClass('row_selected');
                ShowHidHistoryTab();
                ShowHideOrderTab();
                ShowHideChangeLog();
                $('input[type="search"]').val('').trigger('keyup');

                NarrowSearchInGridSC('');
            });

            ///// 
            $('#ExpandNarrowSearchSC').click(function (e) {
                ExpandNarrowSearchSC();
            });
            $('#CollapseNarrowSearchSC').click(function (e) {
                CollapseNarrowSearchSC();
            });
            var NarrowSearchStateSC = getCookieIM('NarrowSearchStateSC');

            if (NarrowSearchStateSC == 'Expanded') {
                CollapseNarrowSearchSC();
            }
            else {
                ExpandNarrowSearchSC();
            }

            var _IsArchived = false;
            var _IsDeleted = false;

            if (typeof ($('#IsArchivedRecords')) != undefined)
                _IsArchived = $('#IsArchivedRecords').is(':checked');

            if (typeof ($('#IsDeletedRecords')) != undefined)
                _IsDeleted = $('#IsDeletedRecords').is(':checked');

            var iscloseorder = true;


            var OrderStatusin = '4,5,6,7,8'
            if ($('#tab4').hasClass('selected'))
                OrderStatusin = '4,5,6,7'



            self.GetNarroFromItemHTMLForUDF('ReceiveList', _NarrowSearchReceiveLayout.model.CompanyID, _NarrowSearchReceiveLayout.model.RoomID, _IsArchived, _IsDeleted, OrderStatusin);

            //        GetNarroHTMLForItemTypeIM();

        });// ready

    }
    self.GetReceiveNarrowSearches = function (_TableName, _IsArchived, _IsDeleted) {
        var iscloseorder = true;
        if ($('#tab4').hasClass('selected'))
            iscloseorder = false;

        var tabname = window.location.hash.toLowerCase();
        switch (tabname) {
            case "#incomplete":
                iscloseorder = false;
                break
        }

        $.ajax({
            'url': '/Master/GetNarrowDDData',
            data: { TableName: _TableName, TextFieldName: 'SupplierName', IsArchived: _IsArchived, IsDeleted: _IsDeleted, IsIncludeClosedOrder: iscloseorder },
            success: function (response) {
                var s = '';

                $.each(response.DDData, function (ValData, ValCount) {
                    var ArrData = ValData.toString().split('[###]');
                    s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
                });


                //Destroy widgets before reapplying the filter
                $("#ReceiveSupplier").empty();
                $("#ReceiveSupplier").multiselect('destroy');
                $("#ReceiveSupplier").multiselectfilter('destroy');

                $("#ReceiveSupplier").append(s);
                $("#ReceiveSupplier").multiselect
                    (
                        {
                            noneSelectedText: Supplier, selectedList: 5,
                            selectedText: function (numChecked, numTotal, checkedItems) {
                                return Supplier + ' ' + numChecked + ' ' + selected;
                            }
                        },
                        {
                            checkAll: function (ui) {
                                $("#ReceiveSupplierCollapse").html('');
                                for (var i = 0; i <= ui.target.length - 1; i++) {
                                    if ($("#ReceiveSupplierCollapse").text().indexOf(ui.target[i].text) == -1) {
                                        $("#ReceiveSupplierCollapse").append("<span>" + ui.target[i].text + "</span>");
                                    }
                                }
                                $("#ReceiveSupplierCollapse").show();
                            }
                        }
                    )
                    .unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                    .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                        if (ui.checked) {
                            if ($("#ReceiveSupplierCollapse").text().indexOf(ui.text) == -1) {
                                $("#ReceiveSupplierCollapse").append("<span>" + ui.text + "</span>");
                            }
                        }
                        else {
                            if (ui.checked == undefined) {
                                $("#ReceiveSupplierCollapse").html('');
                            }
                            else if (!ui.checked) {
                                var text = $("#ReceiveSupplierCollapse").html();
                                text = text.replace("<span>" + ui.text + "</span>", '');
                                $("#ReceiveSupplierCollapse").html(text);
                            }
                            else
                                $("#ReceiveSupplierCollapse").html('');
                        }
                        ReceiveSupplierNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                            return input.value;
                        })

                        //if (ReceiveIncomplateTab)
                        //    _NarrowSearchSave.objSearch.ReceiveSupplierIncomplete = ReceiveSupplierNarroValues;
                        //else
                        _NarrowSearchSave.objSearch.ReceiveSupplier = ReceiveSupplierNarroValues;

                        if ($("#ReceiveSupplierCollapse").text().trim() != '')
                            $("#ReceiveSupplierCollapse").show();
                        else
                            $("#ReceiveSupplierCollapse").hide();

                        if ($("#ReceiveSupplierCollapse").find('span').length <= 2) {
                            $("#ReceiveSupplierCollapse").scrollTop(0).height(50);
                        }
                        else {
                            $("#ReceiveSupplierCollapse").scrollTop(0).height(100);
                        }
                        clearGlobalIMIfNotInFocus();
                        if (!isFromNarrowSearchClear) {
                            DoNarrowSearchSC();
                        }
                    }).multiselectfilter();

                _NarrowSearchSave.setControlValue("ReceiveSupplier");
            },
            error: function (response) {
                // through errror message
            }
        });

        $.ajax({
            'url': '/Master/GetNarrowDDData',
            data: { TableName: _TableName, TextFieldName: 'OrderNumber', IsArchived: _IsArchived, IsDeleted: _IsDeleted, IsIncludeClosedOrder: iscloseorder },
            success: function (response) {
                var s = '';
                $.each(response.DDData, function (ValData, ValCount) {
                    var ArrData = ValData.toString().split('[###]');
                    s += '<option value="' + ArrData[0] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
                });


                //Destroy widgets before reapplying the filter
                $("#ReceivePO").empty();
                $("#ReceivePO").multiselect('destroy');
                $("#ReceivePO").multiselectfilter('destroy');

                $("#ReceivePO").append(s);
                $("#ReceivePO").multiselect
                    (
                        {
                            noneSelectedText: OrderNumver, selectedList: 5,
                            selectedText: function (numChecked, numTotal, checkedItems) {
                                return OrderNumver + ' ' + numChecked + ' ' + selected;
                            }
                        },
                        {
                            checkAll: function (ui) {
                                $("#ReceivePOCollapse").html('');
                                for (var i = 0; i <= ui.target.length - 1; i++) {
                                    if ($("#ReceivePOCollapse").text().indexOf(ui.target[i].text) == -1) {
                                        $("#ReceivePOCollapse").append("<span>" + ui.target[i].text + "</span>");
                                    }
                                }
                                $("#ReceivePOCollapse").show();
                            }
                        }
                    )
                    .unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                    .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                        if (ui.checked) {
                            if ($("#ReceivePOCollapse").text().indexOf(ui.text) == -1) {
                                $("#ReceivePOCollapse").append("<span>" + ui.text + "</span>");
                            }
                        }
                        else {
                            if (ui.checked == undefined) {
                                $("#ReceivePOCollapse").html('');
                            }
                            else if (!ui.checked) {
                                var text = $("#ReceivePOCollapse").html();
                                text = text.replace("<span>" + ui.text + "</span>", '');
                                $("#ReceivePOCollapse").html(text);
                            }
                            else {
                                $("#ReceivePOCollapse").html('');
                            }
                        }
                        ReceivePONarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                            return input.value;
                        })

                        _NarrowSearchSave.objSearch.ReceivePO = ReceivePONarroValues;

                        if ($("#ReceivePOCollapse").text().trim() != '')
                            $("#ReceivePOCollapse").show();
                        else
                            $("#ReceivePOCollapse").hide();


                        if ($("#ReceivePOCollapse").find('span').length <= 2) {
                            $("#ReceivePOCollapse").scrollTop(0).height(50);
                        }
                        else {
                            $("#ReceivePOCollapse").scrollTop(0).height(100);
                        }
                        clearGlobalIMIfNotInFocus();
                        if (!isFromNarrowSearchClear) {
                            DoNarrowSearchSC();
                        }
                    }).multiselectfilter();

                _NarrowSearchSave.setControlValue("ReceivePO");
            },
            error: function (response) {
                // through errror message
            }
        });

        $.ajax({
            'url': '/Master/GetNarrowDDData',
            data: { TableName: _TableName, TextFieldName: 'CreatedBy', IsArchived: _IsArchived, IsDeleted: _IsDeleted, IsIncludeClosedOrder: iscloseorder },
            success: function (response) {
                var s = '';
                $.each(response.DDData, function (ValData, ValCount) {
                    var ArrData = ValData.toString().split('[###]');
                    s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
                });


                //Destroy widgets before reapplying the filter
                $("#ReceiveCreatedBy").empty();
                $("#ReceiveCreatedBy").multiselect('destroy');
                $("#ReceiveCreatedBy").multiselectfilter('destroy');

                $("#ReceiveCreatedBy").append(s);
                $("#ReceiveCreatedBy").multiselect
                    (
                        {
                            noneSelectedText: CreatedBy, selectedList: 5,
                            selectedText: function (numChecked, numTotal, checkedItems) {
                                return CreatedBy + ' ' + numChecked + ' ' + selected;
                            }
                        },
                        {
                            checkAll: function (ui) {
                                $("#ReceiveCreatedByCollapse").html('');
                                for (var i = 0; i <= ui.target.length - 1; i++) {
                                    if ($("#ReceiveCreatedByCollapse").text().indexOf(ui.target[i].text) == -1) {
                                        $("#ReceiveCreatedByCollapse").append("<span>" + ui.target[i].text + "</span>");
                                    }
                                }
                                $("#ReceiveCreatedByCollapse").show();
                            }
                        }
                    )
                    .unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                    .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                        if (ui.checked) {
                            if ($("#ReceiveCreatedByCollapse").text().indexOf(ui.text) == -1) {
                                $("#ReceiveCreatedByCollapse").append("<span>" + ui.text + "</span>");
                            }
                        }
                        else {
                            if (ui.checked == undefined) {
                                $("#ReceiveCreatedByCollapse").html('');
                            }
                            else if (!ui.checked) {
                                var text = $("#ReceiveCreatedByCollapse").html();
                                text = text.replace("<span>" + ui.text + "</span>", '');
                                $("#ReceiveCreatedByCollapse").html(text);
                            }
                            else {
                                $("#ReceiveCreatedByCollapse").html('');
                            }
                        }
                        ReceiveCreatedByNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                            return input.value;
                        })
                        //if (ReceiveIncomplateTab)
                        //    _NarrowSearchSave.objSearch.ReceiveCreatedByIncomplete = ReceiveCreatedByNarroValues;
                        //else
                        _NarrowSearchSave.objSearch.ReceiveCreatedBy = ReceiveCreatedByNarroValues;

                        if ($("#ReceiveCreatedByCollapse").text().trim() != '')
                            $("#ReceiveCreatedByCollapse").show();
                        else
                            $("#ReceiveCreatedByCollapse").hide();


                        if ($("#ReceiveCreatedByCollapse").find('span').length <= 2) {
                            $("#ReceiveCreatedByCollapse").scrollTop(0).height(50);
                        }
                        else {
                            $("#ReceiveCreatedByCollapse").scrollTop(0).height(100);
                        }
                        clearGlobalIMIfNotInFocus();
                        if (!isFromNarrowSearchClear) {
                            DoNarrowSearchSC();
                        }
                    }).multiselectfilter();

                _NarrowSearchSave.setControlValue("ReceiveCreatedBy");
            },
            error: function (response) {
                // through errror message
            }
        });
        $.ajax({
            'url': '/Master/GetNarrowDDData',
            data: { TableName: _TableName, TextFieldName: 'UpdatedBy', IsArchived: _IsArchived, IsDeleted: _IsDeleted, IsIncludeClosedOrder: iscloseorder },
            success: function (response) {
                var s = '';
                $.each(response.DDData, function (ValData, ValCount) {
                    var ArrData = ValData.toString().split('[###]');
                    s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
                });


                //Destroy widgets before reapplying the filter
                $("#ReceiveUpdatedBy").empty();
                $("#ReceiveUpdatedBy").multiselect('destroy');
                $("#ReceiveUpdatedBy").multiselectfilter('destroy');

                $("#ReceiveUpdatedBy").append(s);
                $("#ReceiveUpdatedBy").multiselect
                    (
                        {
                            noneSelectedText: UpdatedBy, selectedList: 5,
                            selectedText: function (numChecked, numTotal, checkedItems) {
                                return UpdatedBy + ' ' + numChecked + ' ' + selected;
                            }
                        },
                        {
                            checkAll: function (ui) {
                                $("#ReceiveUpdatedByCollapse").html('');
                                for (var i = 0; i <= ui.target.length - 1; i++) {
                                    if ($("#ReceiveUpdatedByCollapse").text().indexOf(ui.target[i].text) == -1) {
                                        $("#ReceiveUpdatedByCollapse").append("<span>" + ui.target[i].text + "</span>");
                                    }
                                }
                                $("#ReceiveUpdatedByCollapse").show();
                            }
                        }
                    )
                    .unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                    .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                        if (ui.checked) {
                            if ($("#ReceiveUpdatedByCollapse").text().indexOf(ui.text) == -1) {
                                $("#ReceiveUpdatedByCollapse").append("<span>" + ui.text + "</span>");
                            }
                        }
                        else {
                            if (ui.checked == undefined) {
                                $("#ReceiveUpdatedByCollapse").html('');
                            }
                            else if (!ui.checked) {
                                var text = $("#ReceiveUpdatedByCollapse").html();
                                text = text.replace("<span>" + ui.text + "</span>", '');
                                $("#ReceiveUpdatedByCollapse").html(text);
                            }
                            else {
                                $("#ReceiveUpdatedByCollapse").html('');
                            }
                        }
                        ReceiveUpdatedByNarroValues = $.map($(this).multiselect("getChecked"), function (input) {
                            return input.value;
                        })
                        //if (ReceiveIncomplateTab)
                        //    _NarrowSearchSave.objSearch.ReceiveUpdatedByIncomplete = ReceiveUpdatedByNarroValues;
                        //else
                        _NarrowSearchSave.objSearch.ReceiveUpdatedBy = ReceiveUpdatedByNarroValues;

                        if ($("#ReceiveUpdatedByCollapse").text().trim() != '')
                            $("#ReceiveUpdatedByCollapse").show();
                        else
                            $("#ReceiveUpdatedByCollapse").hide();


                        if ($("#ReceiveUpdatedByCollapse").find('span').length <= 2) {
                            $("#ReceiveUpdatedByCollapse").scrollTop(0).height(50);
                        }
                        else {
                            $("#ReceiveUpdatedByCollapse").scrollTop(0).height(100);
                        }
                        clearGlobalIMIfNotInFocus();
                        if (!isFromNarrowSearchClear) {
                            DoNarrowSearchSC();
                        }
                    }).multiselectfilter();

                _NarrowSearchSave.setControlValue("ReceiveUpdatedBy");
            },
            error: function (response) {
                // through errror message
            }
        });
        $.ajax({
            'url': '/Master/GetNarrowDDData',
            data: { TableName: _TableName, TextFieldName: 'POReceiveaDate', IsArchived: _IsArchived, IsDeleted: _IsDeleted, IsIncludeClosedOrder: iscloseorder },
            success: function (response) {
                var s = '';
                $.each(response.DDData, function (ValData, ValCount) {
                    var ArrData = ValData.toString().split('[###]');
                    //s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
                    s += '<option value="' + ArrData[1] + '">' + ArrData[0] + '</option>';
                });


                //Destroy widgets before reapplying the filter
                $("#POReceiveaDate").empty();
                $("#POReceiveaDate").multiselect('destroy');
                $("#POReceiveaDate").multiselectfilter('destroy');

                $("#POReceiveaDate").append(s);
                $("#POReceiveaDate").multiselect
                    (
                        {
                            noneSelectedText: dateReceived, selectedList: 5,
                            selectedText: function (numChecked, numTotal, checkedItems) {
                                return dateReceived + ' ' + numChecked + ' ' + selected;
                            }
                        },
                        {
                            checkAll: function (ui) {
                                $("#POReceiveaDateCollapse").html('');
                                for (var i = 0; i <= ui.target.length - 1; i++) {
                                    if ($("#POReceiveaDateCollapse").text().indexOf(ui.target[i].text) == -1) {
                                        $("#POReceiveaDateCollapse").append("<span>" + ui.target[i].text + "</span>");
                                    }
                                }
                                $("#POReceiveaDateCollapse").show();
                            }
                        }
                    )
                    .unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                    .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                        if (ui.checked) {
                            if ($("#POReceiveaDateCollapse").text().indexOf(ui.text) == -1) {
                                $("#POReceiveaDateCollapse").append("<span>" + ui.text + "</span>");
                            }
                        }
                        else {
                            if (ui.checked == undefined) {
                                $("#POReceiveaDateCollapse").html('');
                            }
                            else if (!ui.checked) {
                                var text = $("#POReceiveaDateCollapse").html();
                                text = text.replace("<span>" + ui.text + "</span>", '');
                                $("#POReceiveaDateCollapse").html(text);
                            }
                            else
                                $("#POReceiveaDateCollapse").html('');
                        }
                        POReceiveaDateValues = $.map($(this).multiselect("getChecked"), function (input) {
                            return input.value;
                        })
                        //if (ReceiveIncomplateTab)
                        //    _NarrowSearchSave.objSearch.POReceiveaDateIncomplete = POReceiveaDateValues;
                        //else
                        _NarrowSearchSave.objSearch.POReceiveaDate = POReceiveaDateValues;

                        if ($("#POReceiveaDateCollapse").text().trim() != '')
                            $("#POReceiveaDateCollapse").show();
                        else
                            $("#POReceiveaDateCollapse").hide();


                        if ($("#POReceiveaDateCollapse").find('span').length <= 2) {
                            $("#POReceiveaDateCollapse").scrollTop(0).height(50);
                        }
                        else {
                            $("#POReceiveaDateCollapse").scrollTop(0).height(100);
                        }
                        clearGlobalIMIfNotInFocus();
                        if (!isFromNarrowSearchClear) {
                            DoNarrowSearchSC();
                        }
                    }).multiselectfilter();

                _NarrowSearchSave.setControlValue("POReceiveaDate");
            },
            error: function (response) {
                // through errror message
            }
        });
        //if (_NarrowSearchSave.isPageLoading == false) {
        //    setTimeout(function () {
        //        _NarrowSearchSave.loadNarrowSearch();
        //        setTimeout(function () {
        //            DoNarrowSearchSC();
        //        }, 800);
        //    }, 300);
        //}
    }

    self.GetNarroFromItemHTMLForUDF = function (tableName, companyID, roomID, _IsArchived, _IsDeleted, _RequisitionCurrentTab) {
        var UDFObject;

        $("select[name='udflist']").each(function (index) {
            var UDFUniqueID = this.getAttribute('UID');
            $.ajax({
                'url': '/Master/GetUDFDDData',
                data: { TableName: tableName, UDFName: this.id, UDFUniqueID: UDFUniqueID, IsArchived: _IsArchived, IsDeleted: _IsDeleted, RequisitionCurrentTab: _RequisitionCurrentTab },
                success: function (response) {
                    var s = '';
                    if (response.DDData != null) {
                        $.each(response.DDData, function (UDFVal, ValCount) {
                            var ArrData = UDFVal.toString().split('[###]');
                            s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
                        });
                    }
                    var UDFColumnNameTemp = response.UDFColName.toString().replace("_dd", "");

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
                                UserUD64NarrowValues = $.map($(this).multiselect("getChecked"), function (input) {
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
                            clearGlobalIfNotInFocus();

                            if (!isFromNarrowSearchClear) {
                                DoNarrowSearchSC();
                            }
                        }).multiselectfilter();

                    var UDFUniqueIDVal = _Common.getQueryStringVal(this.url, "UDFUniqueID");
                    _NarrowSearchSave.setControlValue(UDFUniqueIDVal);
                },
                error: function (response) {

                    // through errror message
                }
            });
        });
    }

    // private functions start


    // private functions end

    return self;
})(jQuery);



function ExpandNarrowSearchSC() {
    var w = $('#divSupplierCatalogItems,#Ctab .IteamBlock').css("width");
    $('#divSupplierCatalogItems,#Ctab .IteamBlock').show();
    $('#divSupplierCatalogItems,#Ctab .IteamBlock').stop().animate({
        width: "18%"
    }, 0, function () {
        $('#divSupplierCatalogItems,#Ctab .userContent').css({ "width": "80.5%", "margin": "0" });
        $('#CatalogItemDataTable_length').css({ "left": "0px" });
        $('#CatalogItemDataTable_paginate').css({ "left": "145px" });
        $('#divSupplierCatalogItems,#Ctab .leftopenContent').css({ "display": "none" });
        setCookieIM('NarrowSearchStateSC', 'Collapsed');
    });
}

function CollapseNarrowSearchSC() {
    $('#divSupplierCatalogItems,#Ctab .IteamBlock').stop().animate({
        width: "0%"
    }, 0, function () {
        $('#divSupplierCatalogItems,#Ctab .IteamBlock').hide();
        $('#divSupplierCatalogItems,#Ctab .userContent').css({ "width": "98.5%", margin: "0 0.4% 1%" });
        var Left = $('.viewBlock').css("width");
        $('#CatalogItemDataTable_length').css({ "left": Left });
        var LeftW = 145 + parseInt(Left);
        $('#CatalogItemDataTable_paginate').css({ "left": LeftW + 'px' });
        oTable.fnAdjustColumnSizing();
        $('#divSupplierCatalogItems,#Ctab .leftopenContent').css({ "display": "" });
        setCookie('NarrowSearchStateSC', 'Expanded');
    });
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

/* COMMON FUNCTION TO NARROW SEARCH HIDE/SHOW ALONG WITH STATE SAVED : Dec 24, 2012 : IJ */

/* CLEAR NARROW SEARCH - START */
function clearNarrowSearchFilterIM() {

    if (typeof ($("#ReceiveSupplier").multiselect("getChecked").length) != undefined && $("#ReceiveSupplier").multiselect("getChecked").length > 0) {
        $("#ReceiveSupplier").multiselect("uncheckAll");
        $("#ReceiveSupplierCollapse").html('');
    }

    if (typeof ($("#ReceivePO").multiselect("getChecked").length) != undefined && $("#ReceivePO").multiselect("getChecked").length > 0) {
        $("#ReceivePO").multiselect("uncheckAll");
        $("#ReceivePOCollapse").html('');
    }

    if (typeof ($("#ReceiveCreatedBy").multiselect("getChecked").length) != undefined && $("#ReceiveCreatedBy").multiselect("getChecked").length > 0) {
        $("#ReceiveCreatedBy").multiselect("uncheckAll");
        $("#ReceiveCreatedByCollapse").html('');
    }
    if (typeof ($("#ReceiveUpdatedBy").multiselect("getChecked").length) != undefined && $("#ReceiveUpdatedBy").multiselect("getChecked").length > 0) {
        $("#ReceiveUpdatedBy").multiselect("uncheckAll");
        $("#ReceiveUpdatedByCollapse").html('');
    }

    if (typeof ($("#POReceiveaDate").multiselect("getChecked").length) != undefined && $("#POReceiveaDate").multiselect("getChecked").length > 0) {
        $("#POReceiveaDate").multiselect("uncheckAll");
        $("#POReceiveaDateCollapse").html('');
    }

    if ($('#ReceiveDateCFrom').val() != '') $('#ReceiveDateCFrom').val('');
    if ($('#ReceiveDateCTo').val() != '') $('#ReceiveDateCTo').val('');
    if ($('#ReceiveDateUFrom').val() != '') $('#ReceiveDateUFrom').val('');
    if ($('#ReceiveDateUTo').val() != '') $('#ReceiveDateUTo').val('');
    isFromNarrowSearchClear = true;
    _NarrowSearchSave.objSearch.reset();
}
/* CLEAR NARROW SEARCH - END */

/* CLEAR GLOBAL FILTER IF NOT IN FOCUS - START */
function clearGlobalIMIfNotInFocus() {
    if ($(document.activeElement).attr('id') != 'FilterSupplierCatalogItems')
        $("#FilterSupplierCatalogItems").val('');
}
/* CLEAR GLOBAL FILTER IF NOT IN FOCUS - END */
function DoNarrowSearchSC() {

    var narrowSearchFields = '';
    var narrowSearchValues = '';
    var narrowSearchItem = '';

    if (ReceiveSupplierNarroValues != undefined && ReceiveSupplierNarroValues.length > 0) {
        narrowSearchFields += "Supplier" + ",";
        narrowSearchValues += ReceiveSupplierNarroValues + '@';
    }
    else {
        narrowSearchFields += "Supplier" + ",";
        narrowSearchValues += '@';
    }
    if (ReceivePONarroValues != undefined && ReceivePONarroValues.length > 0) {
        narrowSearchFields += "OrderNumber" + ",";
        narrowSearchValues += ReceivePONarroValues + '@';
    }
    else {
        narrowSearchFields += "OrderNumber" + ",";
        narrowSearchValues += '@';
    }

    if (ReceiveCreatedByNarroValues != undefined && ReceiveCreatedByNarroValues.length > 0) {
        narrowSearchFields += "CreatedBy" + ",";
        narrowSearchValues += ReceiveCreatedByNarroValues + '@';
    }
    else {
        narrowSearchFields += "CreatedBy" + ",";
        narrowSearchValues += '@';
    }

    if (ReceiveUpdatedByNarroValues != undefined && ReceiveUpdatedByNarroValues.length > 0) {
        narrowSearchFields += "UpdatedBy" + ",";
        narrowSearchValues += ReceiveUpdatedByNarroValues + '@';
    }
    else {
        narrowSearchFields += "UpdatedBy" + ",";
        narrowSearchValues += '@';
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    if ($('#ReceiveDateCFrom').val() != '' && $('#ReceiveDateCTo').val() != '') {

        narrowSearchFields += "DateCreatedFrom" + ",";
        narrowSearchValues += ($('#ReceiveDateCFrom').val()) + "," + ($('#ReceiveDateCTo').val()) + '@';
    }
    else {
        narrowSearchFields += "DateCreatedFrom" + ",";
        narrowSearchValues += '@';
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////
    if ($('#ReceiveDateUFrom').val() != '' && $('#ReceiveDateUTo').val() != '') {

        narrowSearchFields += "DateUpdatedFrom" + ",";
        narrowSearchValues += ($('#ReceiveDateUFrom').val()) + "," + ($('#ReceiveDateUTo').val()) + '@';
    }
    else {
        narrowSearchFields += "DateUpdatedFrom" + ",";
        narrowSearchValues += '@';
    }
    // POReceive Date
    if (POReceiveaDateValues != undefined && POReceiveaDateValues.length > 0) {
        narrowSearchFields += "POReceiveDate" + ",";
        narrowSearchValues += POReceiveaDateValues + '@';
    }
    else {
        narrowSearchFields += "POReceiveDate" + ",";
        narrowSearchValues += '@';
    }
    // POReceive Date
    if (UserUDF1NarrowValues != undefined && UserUDF1NarrowValues.length > 0) {
        //narrowSearchItem += "[###]UDF1#" + UserUDF1NarrowValues;
        narrowSearchFields += "UDF1" + ",";
        narrowSearchValues += UserUDF1NarrowValues + '@';
    }
    else {
        narrowSearchFields += "UDF1" + ",";
        narrowSearchValues += '@';
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////////
    if (UserUDF2NarrowValues != undefined && UserUDF2NarrowValues.length > 0) {
        //narrowSearchItem += "[###]UDF2#" + UserUDF2NarrowValues;
        narrowSearchFields += "UDF2" + ",";
        narrowSearchValues += UserUDF2NarrowValues + '@';
    }
    else {
        narrowSearchFields += "UDF2" + ",";
        narrowSearchValues += '@';
    }
    ///////////////////////////////////////////////////////////////////////////////////////////////
    if (UserUDF3NarrowValues != undefined && UserUDF3NarrowValues.length > 0) {
        //narrowSearchItem += "[###]UDF3#" + UserUDF3NarrowValues;
        narrowSearchFields += "UDF3" + ",";
        narrowSearchValues += UserUDF3NarrowValues + '@';
    }
    else {
        narrowSearchFields += "UDF3" + ",";
        narrowSearchValues += '@';
    }
    /////////////////////////////////////////////////////////////////////////////////////////////
    if (UserUDF4NarrowValues != undefined && UserUDF4NarrowValues.length > 0) {
        //narrowSearchItem += "[###]UDF4#" + UserUDF4NarrowValues;
        narrowSearchFields += "UDF4" + ",";
        narrowSearchValues += UserUDF4NarrowValues + '@';
    }
    else {
        narrowSearchFields += "UDF4" + ",";
        narrowSearchValues += '@';
    }
    /////////////////////////////////////////////////////////////////////////////////////////////
    if (UserUDF5NarrowValues != undefined && UserUDF5NarrowValues.length > 0) {
        //narrowSearchItem += "[###]UDF5#" + UserUDF5NarrowValues;
        narrowSearchFields += "UDF5" + ",";
        narrowSearchValues += UserUDF5NarrowValues + '@';
    }
    else {
        narrowSearchFields += "UDF5" + ",";
        narrowSearchValues += '@';
    }

    /////////////////////////////////////////////////////////////////////////////////////////////
    if (UserUDF6NarrowValues != undefined && UserUDF6NarrowValues.length > 0) {
        narrowSearchFields += "UDF6" + ",";
        narrowSearchValues += UserUDF6NarrowValues + '@';
    }
    else {
        narrowSearchFields += "UDF6" + ",";
        narrowSearchValues += '@';
    }

    /////////////////////////////////////////////////////////////////////////////////////////////
    if (UserUDF7NarrowValues != undefined && UserUDF7NarrowValues.length > 0) {
        narrowSearchFields += "UDF7" + ",";
        narrowSearchValues += UserUDF7NarrowValues + '@';
    }
    else {
        narrowSearchFields += "UDF7" + ",";
        narrowSearchValues += '@';
    }

    /////////////////////////////////////////////////////////////////////////////////////////////
    if (UserUDF8NarrowValues != undefined && UserUDF8NarrowValues.length > 0) {
        narrowSearchFields += "UDF8" + ",";
        narrowSearchValues += UserUDF8NarrowValues + '@';
    }
    else {
        narrowSearchFields += "UDF8" + ",";
        narrowSearchValues += '@';
    }

    /////////////////////////////////////////////////////////////////////////////////////////////
    if (UserUDF9NarrowValues != undefined && UserUDF9NarrowValues.length > 0) {
        narrowSearchFields += "UDF9" + ",";
        narrowSearchValues += UserUDF9NarrowValues + '@';
    }
    else {
        narrowSearchFields += "UDF9" + ",";
        narrowSearchValues += '@';
    }

    /////////////////////////////////////////////////////////////////////////////////////////////
    if (UserUDF10NarrowValues != undefined && UserUDF10NarrowValues.length > 0) {
        narrowSearchFields += "UDF10" + ",";
        narrowSearchValues += UserUDF10NarrowValues + '@';
    }
    else {
        narrowSearchFields += "UDF10" + ",";
        narrowSearchValues += '@';
    }


    //narrowSearch = 'STARTWITH#' + narrowSearchItem;
    //  if (narrowSearchValues.replace(/@("@")/g, '') == '')
    //    NarrowSearchInGridSC('');
    //  else {
    var searchtext = $("#global_filter").val().replace(/'/g, "''");
    narrowSearch = narrowSearchFields + "[###]" + narrowSearchValues + "[###]" + searchtext;

    if (narrowSearch.length > 10)
        NarrowSearchInGridSC(narrowSearch);
    else if (UserCreatedNarroValues == undefined || UserCreatedNarroValues.length == 0 ||
        UserUpdatedNarroValues == undefined || UserUpdatedNarroValues.length == 0)
        NarrowSearchInGridSC(narrowSearch);
}

function NarrowSearchInGridSC(searchstr) {
    //if (typeof _ReceiveList !== 'undefined') {
    //    _ReceiveList.isSaveGridState = false;
    //    _ReceiveList.isGetReplinshRedCount = false;
    //    _ReceiveList.isGetUDFEditableOptionsByUDF = false;
    //}


    $('#myDataTable').dataTable().fnFilter(searchstr, null, null, null)
}

function ClearNarrowSearch() {
    isFromNarrowSearchClear = true;
    _NarrowSearchSave.objSearch.reset();

    if (typeof ($("#ReceivePO").multiselect("getChecked").length) != undefined && $("#ReceivePO").multiselect("getChecked").length > 0) {
        $("#ReceivePO").multiselect("uncheckAll");
        $("#ReceivePOCollapse").html('');
    }
    else if (typeof ($("#ReceivePOCollapse")) != undefined) {
        $("#ReceivePOCollapse").html('');
        $("#ReceivePOCollapse").hide();
    }
    if (typeof ($("#ReceiveSupplier").multiselect("getChecked").length) != undefined && $("#ReceiveSupplier").multiselect("getChecked").length > 0) {
        $("#ReceiveSupplier").multiselect("uncheckAll");
        $("#ReceiveSupplierCollapse").html('');
    }
    else if (typeof ($("#ReceiveSupplierCollapse")) != undefined) {
        $("#ReceiveSupplierCollapse").html('');
        $("#ReceiveSupplierCollapse").hide();
    }
    if (typeof ($("#ReceiveCreatedBy").multiselect("getChecked").length) != undefined && $("#ReceiveCreatedBy").multiselect("getChecked").length > 0) {
        $("#ReceiveCreatedBy").multiselect("uncheckAll");
        $("#ReceiveCreatedByCollapse").html('');
    }
    else if (typeof ($("#ReceiveCreatedByCollapse")) != undefined) {
        $("#ReceiveCreatedByCollapse").html('');
        $("#ReceiveCreatedByCollapse").hide();
    }
    if (typeof ($("#ReceiveUpdatedBy").multiselect("getChecked").length) != undefined && $("#ReceiveUpdatedBy").multiselect("getChecked").length > 0) {
        $("#ReceiveUpdatedBy").multiselect("uncheckAll");
        $("#ReceiveUpdatedByCollapse").html('');
    }
    else if (typeof ($("#ReceiveUpdatedByCollapse")) != undefined) {
        $("#ReceiveUpdatedByCollapse").html('');
        $("#ReceiveUpdatedByCollapse").hide();
    }
    if (typeof ($("#POReceiveaDate").multiselect("getChecked").length) != undefined && $("#POReceiveaDate").multiselect("getChecked").length > 0) {
        $("#POReceiveaDate").multiselect("uncheckAll");
        $("#POReceiveaDateCollapse").html('');
    }
    else if (typeof ($("#POReceiveaDateCollapse")) != undefined) {
        $("#POReceiveaDateCollapse").html('');
        $("#POReceiveaDateCollapse").hide();
    }
    if (typeof ($("#POReceiveaDate").multiselect("getChecked").length) != undefined && $("#POReceiveaDate").multiselect("getChecked").length > 0) {
        $("#POReceiveaDate").multiselect("uncheckAll");
        $("#POReceiveaDateCollapse").html('');
    }
    else if (typeof ($("#POReceiveaDateCollapse")) != undefined) {
        $("#POReceiveaDateCollapse").html('');
        $("#POReceiveaDateCollapse").hide();
    }

    $("select[name='udflist']").each(function (index) {
        if (typeof ($(this).multiselect("getChecked").length) != undefined && $(this).multiselect("getChecked").length > 0) {
            var UDFUniqueID = this.getAttribute('UID');
            $(this).multiselect("uncheckAll");
            $('#' + UDFUniqueID + 'Collapse').html('');
            $('#' + UDFUniqueID + 'Collapse').hide();
        }
        else if ((typeof (this.getAttribute('UID'))) != undefined) {
            var UDFUniqueID = this.getAttribute('UID');
            $('#' + UDFUniqueID + 'Collapse').html('');
            $('#' + UDFUniqueID + 'Collapse').hide();
        }
    });
    if ($('#ReceiveDateCFrom').val() != '') $('#ReceiveDateCFrom').val('');
    if ($('#ReceiveDateCTo').val() != '') $('#ReceiveDateCTo').val('');
    if ($('#ReceiveDateUFrom').val() != '') $('#ReceiveDateUFrom').val('');
    if ($('#ReceiveDateUTo').val() != '') $('#ReceiveDateUTo').val('');
    isFromNarrowSearchClear = false;
    if ($('#global_filter').val() != '') $('#global_filter').val('');

    $('input[type="search"]').val('').trigger('keyup');
}