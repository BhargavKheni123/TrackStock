

// Common wrapper objects


_utils = (function ($) {

    var self = {};

    self.showHideLoader = function (isShow) {
        if (isShow) {
            $('#DivLoading').show();
        }
        else {
            $('#DivLoading').hide();
        }
    }

    self.isFunction = function (fnFunction) {
        return $.isFunction(fnFunction);
    };

    self.isNullUndefined = function (obj) {
        return typeof obj === 'undefined' || obj == null;
    }

    self.isNullUndefinedBlank = function (obj) {
        return typeof obj === 'undefined' || obj == null || obj == '';
    }

    self.parseNumber = function (val) {

        if (self.isNullUndefined(val)) {
            val = 0;
        }

        var num = parseFloat(val);
        if (isNaN(num)) {
            num = 0;
        }
        return num;
    }

    return self;
})(jQuery)

_AjaxUtil = (function ($) {
    var self = {};

    self.getText = function (url, data, fnSuccess, fnError, async, cache) {

        if (typeof cache === 'undefined' || cache == null) {
            cache = true;
        }

        if (typeof async === 'undefined' || async == null) {
            async = true;
        }

        var jqXHR = $.ajax({
            "url": url,
            "type": 'GET',
            "data": data,
            "async": async,
            "cache": cache,
            "dataType": "text",
            "success": function (json) {
                if (fnSuccess != null && typeof fnSuccess === 'function') {
                    fnSuccess(json);
                }
            },
            error: function (response) {
                if (fnError != null && typeof fnError === 'function') {
                    fnError(response);
                }
            }
        });

        return jqXHR;
    }

    self.getJson = function (url, data, fnSuccess, fnError, async, cache) {

        if (typeof cache === 'undefined' || cache == null) {
            cache = true;
        }

        if (typeof async === 'undefined' || async == null) {
            async = true;
        }

        var jqXHR = $.ajax({
            "url": url,
            "type": 'GET',
            "data": data,
            "async": async,
            "cache": cache,
            "dataType": "json",
            "success": function (json) {
                if (fnSuccess != null && typeof fnSuccess === 'function') {
                    fnSuccess(json);
                }
            },
            error: function (response) {
                if (fnError != null && typeof fnError === 'function') {
                    fnError(response);
                }
            }
        });

        return jqXHR;
    }

    self.postJson = function (url, data, fnSuccess, fnError, async, cache, fnBeforeSend, fnComplete, headers) {

        if (typeof cache === 'undefined' || cache == null) {
            cache = true;
        }

        if (typeof async === 'undefined' || async == null) {
            async = true;
        }

        var jqXHR = $.ajax({
            "url": url,
            "type": "POST",
            data: data,
            "async": async,
            cache: cache,
            "dataType": "json",
            headers: headers,
            beforeSend: fnBeforeSend,
            //beforeSend: function () {
            //    if (fnBeforeSend != null && typeof fnBeforeSend === 'function') {
            //        fnBeforeSend(json);
            //    }
            //},
            "success": function (json) {
                if (fnSuccess != null && typeof fnSuccess === 'function') {
                    fnSuccess(json);
                }
            },
            "error": function (response) {
                if (fnError != null && typeof fnError === 'function') {
                    fnError(response);
                }
            },
            complete: function (xhr, status) {
                // A function to be called when the request finishes (after success and error callbacks are executed)
                if (fnComplete != null && typeof fnComplete === 'function') {
                    fnComplete(xhr, status);
                }
            }
        });

        return jqXHR;
    }

    self.postText = function (url, data, fnSuccess, fnError, async, cache, isJsonPara) {

        if (typeof cache === 'undefined' || cache == null) {
            cache = true;
        }

        if (typeof async === 'undefined' || async == null) {
            async = true;
        }

        var contentType = "application/json";

        if (isJsonPara == false) {
            contentType = "application/x-www-form-urlencoded; charset=UTF-8";
        }

        var jqXHR = $.ajax({
            "url": url,
            "type": "POST",
            data: data,
            "async": async,
            cache: cache,
            "dataType": "text", // dataType is what you're expecting back from the server
            "contentType": contentType, // contentType is the type of data you're sending
            "success": function (json) {
                if (fnSuccess != null && typeof fnSuccess === 'function') {
                    fnSuccess(json);
                }
            },
            "error": function (response) {
                if (fnError != null && typeof fnError === 'function') {
                    fnError(response);
                }
            }
        });
        return jqXHR;
    }

    return self;
})(jQuery);

_dialogWrapper = (function ($) {

    var self = {}; // single object

    self.init = function (dialogId, title, width, fnCreate, fnOpen, fnClose) {

        if (typeof width === 'undefined' || width === null) {
            width = 500;
        }

        $("#" + dialogId).dialog({
            autoOpen: false,
            modal: true,
            width: width,
            //title: "ReOrder Columns",
            title: title,
            draggable: true,
            resizable: true,
            create: function (event, ui) {
                //$("#ColumnSortableModal").dialog("open");
                if ($.isFunction(fnCreate)) {
                    fnCreate(event, ui);
                }
            },
            open: function () {
                if ($.isFunction(fnOpen)) {
                    fnOpen();
                }
            },
            close: function () {
                if ($.isFunction(fnClose)) {
                    fnClose();
                }
            }
        });
    }

    self.open = function (dialogId) {
        $("#" + dialogId).dialog("open");
    }

    self.destroy = function (dialogId) {
        $("#" + dialogId).dialog("destroy");
    }

    return self;

})(jQuery);

_datePickerWrapper = (function ($) {
    // https://api.jqueryui.com/datepicker/
    var self = {};


    self.initDatePicker = function (txtId, dateFormat, changeMonth, changeYear, date, onSelect) {

        if (typeof changeMonth == 'undefined') {
            changeMonth = false;
        }

        if (typeof changeYear == 'undefined') {
            changeYear = false;
        }
        
        if (typeof date == 'undefined') {
            date = false;
        }

        $("#" + txtId).datepicker({
            dateFormat: dateFormat,
            changeMonth: changeMonth, // Whether the month should be rendered as a dropdown instead of text
            changeYear: changeYear, // Whether the year should be rendered as a dropdown instead of text.
            date: date,
            onSelect: function (selected) {
                if ($.isFunction(onSelect)) {
                    onSelect(selected)
                }
            }
        });

        return self;
    }

    self.getDate = function (txtId) {
        var dt = $("#" + txtId).datepicker("getDate");
        return dt;
    }

    self.setDate = function (txtId, dt) {
        if (typeof dt == "undefined" || dt == null) {
            return;
        }
        try {
            var d = new Date(dt);
            if (self.isValidDate(d)) {
                $("#" + txtId).datepicker("setDate", d);
            }
        }
        catch (e) {
            console.error(e)
        }
        return self;
    }

    self.destroy = function (txtId) {
        $("#" + txtId).datepicker("destroy");
    }

    self.disable = function (txtId,isDisable) {
        $("#" + txtId).datepicker("option", "disabled", isDisable);
        return self;
    }

    self.isDisabled = function (txtId) {
        return $("#" + txtId).datepicker("isDisabled");
    }

    self.showDatePicker = function (txtId) {
        $("#" + txtId).datepicker("show");
        return self;
    }
      

    self.hideDatePicker = function (txtId) {
        $("#" + txtId).datepicker("hide");
        return self;
    }

    self.refresh = function () {
        $("#" + txtId).datepicker("refresh");
        return self;
    }

    self.getFormatedDate = function (txtId, format) {

        var hdDateFormat = $("#hdDateFormat"),
            hdDateFormatVal = "";

        if (hdDateFormat.length) {
            hdDateFormatVal = hdDateFormat.val();
        }
        hdDateFormat.val(format);
        var dt = self.getDate(txtId);

        var formattedDate = $.datepicker.formatDate(format, dt);

        if (hdDateFormat.length) {
            hdDateFormat.val(hdDateFormatVal);
        }
                

        return formattedDate;
    }

    self.parseDate = function (txtId, format) {

        var dt = $.datepicker.parseDate(format, $("#" + txtId).val());
        return dt;
    }

    self.isValidDate = function (d) {
        if (Object.prototype.toString.call(d) === "[object Date]") {
            // it is a date
            if (isNaN(d.getTime())) {  // d.valueOf() could also work
                // date is not valid
                return false;
            } else {
                // date is valid
                return true;
            }
        } else {
            // not a date
            return false;
        }
    }

    return self;

})(jQuery);

_notification = (function ($) {

    var self = {};

    self.msgQueue = [];
    var isProgress = false;

    self.showSuccess = function (msg) {
        self.msgQueue.push({ msg: msg, isSuccess: true, isWarn: false, isError: false });
        //var $spanGlobalMessage = $("#spanGlobalMessage");
        //$spanGlobalMessage.text(msg);
        //$spanGlobalMessage.removeClass('WarningIcon errorIcon').addClass('succesIcon');
        showNotification(false);
    }

    self.showError = function (msg) {
        self.msgQueue.push({ msg: msg, isSuccess: false, isWarn: false, isError: true });
        //var $spanGlobalMessage = $("#spanGlobalMessage");
        //$spanGlobalMessage.text(msg);
        //$spanGlobalMessage.removeClass('WarningIcon succesIcon').addClass('errorIcon');
        showNotification(false);
    }

    self.showWarning = function (msg) {
        //var $spanGlobalMessage = $("#spanGlobalMessage");
        //$spanGlobalMessage.text(msg);
        //$spanGlobalMessage.removeClass('errorIcon succesIcon').addClass('WarningIcon');
        self.msgQueue.push({ msg: msg, isSuccess: false, isWarn: true, isError: false });
        showNotification(false);
    }

    function showNotification(isChild) {


        if ((self.msgQueue.length > 0 && isProgress == false) ||
            (self.msgQueue.length > 0 && isChild)) {

            isProgress = true;
            var objMsg = self.msgQueue.shift();
            var $spanGlobalMessage = $("#spanGlobalMessage");

            if (objMsg.isSuccess) {
                $spanGlobalMessage.removeClass('WarningIcon errorIcon').addClass('succesIcon');
            }
            else if (objMsg.isWarn) {
                $spanGlobalMessage.removeClass('errorIcon succesIcon').addClass('WarningIcon');
            }
            else if (objMsg.isError) {
                $spanGlobalMessage.removeClass('WarningIcon succesIcon').addClass('errorIcon');
            }

            $spanGlobalMessage.html(objMsg.msg);
                      

            $('div#target').css("z-index", 100000);
            $('div#target').fadeToggle().delay(DelayTime).fadeOut(FadeOutTime, function () {
                if (self.msgQueue.length == 0) {
                    isProgress = false;
                }
                showNotification(true);
            });
        }
        
    }

    return self;
})(jQuery);

_multiSelectWrapper = (function () {
    // http://comsim.esac.esa.int/rossim/3dtool/common/utils/jquery/ehynds-jquery-ui-multiselect-widget-f51f209/demos/index.htm
    var self = {}; // single object

    self.init = function (ctlId, options) {
        return $("#" + ctlId).multiselect(options);
    }

    self.open = function (ctlId) {
        $("#" + ctlId).multiselect("open");
    }

    self.isOpen = function (ctlId) {
        return $("#" + ctlId).multiselect("isOpen");
    }

    self.close = function (ctlId) {
        $("#" + ctlId).multiselect("close");
    }

    self.destroy = function (ctlId) {
        $("#" + ctlId).empty();
        $("#" + ctlId).multiselect('destroy');
        $("#" + ctlId).multiselectfilter('destroy');
    }

    //self.removeOption = function (ctlId, value) {
    //    $("#" + ctlId).multiselect("removeOption", value);
    //}

    self.uncheckAll = function (ctlId) {
        $("#" + ctlId).multiselect("uncheckAll");
    }

    self.getChecked = function (ctlId) {
        return $("#" + ctlId).multiselect("getChecked");
    }

    self.getCheckedValueArray = function (ctlId) {
        var selectedOpts = self.getChecked(ctlId);
        var arr = [];
        $.each(selectedOpts, function (index, obj) {
            arr.push($(obj).attr("value"));
        });

        return arr;
    }

    //self.getUnchecked = function (ctlId) {
    //    return $("#" + ctlId).multiselect("getUnchecked");
    //}

    self.enable = function (ctlId) {
        $("#" + ctlId).multiselect("enable");
    }

    self.disable = function (ctlId) {
        $("#" + ctlId).multiselect("disable");
    }


    self.getWidget = function (ctlId) {
        var ddl = $("#" + ctlId);
        if (ddl.length == 0) {
            ddl = $("[id='" + ctlId + "']");
        }
        return ddl.multiselect("widget");
    }

    self.getAllCheckbox = function (ctlId) {
        return self.getWidget(ctlId).find('input');
    }

    //self.tickCheckbox = function (ctlId, value) {
    //    var arrChk = self.getAllCheckbox(ctlId);

    //    $.each(arrChk, function (idx , chk) {
    //        if (chk.value == value) {
    //            $(chk).prop('checked', true);
    //        }
    //    })

    //}

    self.refresh = function (ctlId) {
        var ddl = $("#" + ctlId);
        if (ddl.length == 0) {
            ddl = $("[id='" + ctlId + "']");
        }
        ddl.multiselect("refresh");
    }

    self.tickMultipleCheckbox = function (ctlId, arrValue) {

        if (arrValue == null || arrValue.length == 0) {
            return [];
        }

        //var arrChk = self.getAllCheckbox(ctlId);
        var widget = self.getWidget(ctlId);

        var accordionText = "";
        var tickValues = [];

        var ddl = $("#" + ctlId);
        if (ddl.length == 0) {
            ddl = $("[id='" + ctlId + "']");
        }

        $.each(arrValue, function (idx, value) {
            var chk = widget.find(":checkbox[value='" + value + "']");

            if (chk.length == 0) {
                value = unescape(value); // html decode
                chk = widget.find(":checkbox[value='" + value + "']");
            }

            if (chk.length) {
                tickValues.push(value);
                chk.attr("checked", "checked");

                var opt = ddl.find("option[value='" + value + "']")  //$("#" + ctlId + " option[value='" + value + "']");
                if (opt.length) {
                    opt.attr("selected", 1);
                    accordionText = accordionText + "<span>" + opt.text() + "</span>";
                }
            }
        });

        self.refresh(ctlId);

        var accordion = ddl.siblings(".accordion").find(".dropcontent");

        if (accordion.length) {
            accordion.html("");
            accordion.append(accordionText);
        }

        if (accordion.text().trim() != '') {

            if (accordion.find('span').length <= 2) {
                accordion.scrollTop(0).height(50);
            }
            else {
                accordion.scrollTop(0).height(100);
            }

            if (accordion.is(":visible") == false) {
                accordion.slideDown();
            }
            accordion.css("overflow", '');

        }
        else {
            accordion.slideUp();
        }

        return tickValues;
    }

    return self;

})(jQuery);

_AutoCompleteWrapper = (function ($) {

    var self = {};

    self.selectedItem = null;

    self.isInit = function (ddl) {
        return ddl.data('is_init') == true;
    }

    self.init = function (ddl, dataUrl, fnGetAjaxReqData, fnGetAjaxResData, fnSelect, fnChange, isPost, isCheckInit) {

        if (isCheckInit && ddl.data('is_init') == true) {
            return false;
        }

        //self.getAjaxReqData = function (request) {
        //    return fnGetAjaxReqData(request);
        //};
        ddl.data('is_init', true);

        ddl.autocomplete({
            source: function (request, response) {

                if (isPost) {
                    //_AjaxUtil.postJson(dataUrl
                    //    , reqData
                    //    , function (data) {
                    //        response(fnGetAjaxResData(data));
                    //    }, null, true, false
                    //);
                    $.ajax({
                        url: dataUrl,
                        type: 'POST',
                        data: fnGetAjaxReqData(request), //ON.stringify({ 'NameStartWith': request.term }),
                        contentType: 'application/json',
                        dataType: 'json',
                        success: function (data) {
                            response(fnGetAjaxResData(data));
                        }
                    });
                }
                else {
                    //_AjaxUtil.getJson(dataUrl
                    //    , fnGetAjaxReqData(request)
                    //    , function (data) {
                    //        response(fnGetAjaxResData(data));
                    //    }, null, true, false
                    //);

                    $.ajax({
                        url: dataUrl,
                        type: 'GET',
                        data: fnGetAjaxReqData(request),
                        contentType: 'application/json',
                        dataType: 'json',
                        success: function (data) {
                            response(fnGetAjaxResData(data));
                        }
                    });
                }


            },
            autoFocus: false,
            minLength: 0,
            select: function (event, ui) {
                var curVal = $(this).val();
                self.selectedItem = ui.item;

                if (typeof ($("#hdnPageName").val()) != "undefined" && $("#hdnPageName").val() == "ReceiveMaster")
                {
                    if (ui.item.value == MoreLocation) {
                        $(this).parent().parent().parent().find("#hdnIsLoadMoreLocations").val("true");
                        $(this).trigger("focus");
                        $(this).autocomplete("search", " ");
                        return false;
                    }
                    else {
                        $(this).val(ui.item.value);
                    }
                }

                if ($(this).val() !== ui.item.value 
                    && ui.item.selval !== MoreLocation
                ) {
                    $(this).val(ui.item.value);
                }

                if (_utils.isFunction(fnSelect)) {
                    fnSelect(curVal, self.selectedItem);
                }

            },
            change: function (event, ui) {
                self.selectedItem = ui.item;
                if (_utils.isFunction(fnChange)) {
                    fnChange(self.selectedItem);
                }
            },
            open: function () {
                $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
                $('ul.ui-autocomplete').css('overflow-y', 'auto');
                $('ul.ui-autocomplete').css('overflow-x', 'hidden');
                $('ul.ui-autocomplete').css('max-height', '200px');
                $(this).autocomplete('widget').css('z-index', '99999 !important');
                $(this).data('is_open', true);
            },
            close: function () {
                $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
                $(this).data('is_open', false);
            }
        });

    };

    self.focus = function (ddl) {
        ddl.trigger("focus");
        return this;
    };

    self.close = function (ddl) {
        ddl.trigger("close");
        return this;
    };

    self.search = function (ddl, searchVal) {
        ddl.autocomplete("search", searchVal);
        return this;
    };

    self.isOpen = function (ddl) {
        return ddl.data("is_open") === true;
    };

    self.searchHide = function (ddl) {
        if (self.isOpen(ddl)) {
            self.close(ddl);
        }
        else {
            ddl.trigger("focus").autocomplete("search", " ");
        }
    };

    return self;
})(jQuery);

