/**
/**
* jQuery eComboBox - An editable Combo Box Plugin - http://www.reckdesign.de/eComboBox
* 
* @author  Recep Karadas
* @version 1.0
*
* jQuery eComboBox generates editable Combo Boxes.
*
* Licensed under The MIT License
*
* @version			1.0
* @since			22.05.2011
* @author			Recep Karadas
* @documentation	http://www.reckdesign.de/eComboBox
* @license			http://opensource.org/licenses/mit-license.php
*
* Usage with default values:
* ---------------------------------------------------------------------------------
* $('#eComboBox').eComboBox();
*
* <select id="eComboBox">
* 	<option>Value 1</option>
* 	<option>Value 2</option>
* 	<option>Value 3</option>
* 	<option>Value 4</option>
* </select>
*
*/
(function ($) {

    var methods = {
        init: function (options) {

            var settings = {
                'allowNewElements': true,
                'editableElements': false,
                'textBoxFocusColor': '#F1F1F1',
                'NewElementText': "{Add New Bin}",
                'AjaxURL': '',
                'AddToOther': false,
                'DataTableName': "",
                'IsStagingLocation': false
            };


            return this.each(function () {

                var wrapperElement = null;
                var selectElement = null;
                selectElement = $(this);
                var selected = "";
                // If options exist, lets merge them
                // with our default settings
                if (options) {
                    $.extend(settings, options);
                }

                $(this).data("settings", settings);

                if (settings.allowNewElements) {
                    selectElement.prepend('<option>' + settings.NewElementText + '</option>');
                }

                // Create Wrapper Element 
                var wrapperEl = document.createElement('span');
                wrapperElement = jQuery(wrapperEl);

                // Create Input Element
                var inputEl = document.createElement('input');
                var inputElement = jQuery(inputEl);

                // put input and select element in wrapper element
                selectElement.before(wrapperElement);
                wrapperElement.append(inputElement).append(selectElement);


                inputElement.css({
                    "background": settings.textBoxFocusColor,
                    "position": "absolute",
                    "display": "none",
                    "width": "124px",
                    "margin": "3px"
                });


                resizeElements();

                selectElement.keydown(function (e) {
                    //alert(e.keyCode);
                    if (e.keyCode >= 37 && e.keyCode <= 40 || e.keyCode == 13) // arrow buttons or enter button
                        return;

                    selected = $(this).val();

                    if (e.keyCode == "46") { // del-button
                        if (selected != settings.NewElementText) {
                            $(this).children("option:selected").remove();
                        }
                        return;
                    }

                    if (selected == settings.NewElementText || settings['editableElements']) {
                        inputElement.css({ "display": "inline" });
                        if (selected == settings.NewElementText) {
                            inputElement.val("").focus();
                        } else if (settings['editableElements']) {
                            inputElement.val($(this).val()).focus();
                        }
                    }
                });

                selectElement.change(function (e) {
                    if ($(this).val() == settings.NewElementText) {
                        selected = $(this).val();
                        inputElement.css({ "display": "inline" });
                        inputElement.val("").focus();
                    }
                });



                inputElement.keyup(function (e) {
                    if (e.keyCode == 13) { //enter

                        if (selected == settings.NewElementText) {
                            if ($(this).val() != "" && $(this).val() != settings.NewElementText) {
                                if ($.trim(settings.AjaxURL).length <= 0) {
                                    selectElement.append('<option>' + $.trim($(this).val()) + '</option>');
                                    selectElement.val($(this).val());
                                }
                                else {
                                    var obj = this;
                                    var isStaging = false;
                                    if ($.trim(settings.DataTableName) != "") {
                                        var tr = $(selectElement).parent().parent().parent();
                                        var rowPosition = $("#" + settings.DataTableName).dataTable().fnGetPosition(tr[0]);
                                        var aData = oTable.fnGetData(rowPosition);

                                        if (isNaN(parseInt(aData.StagingID)) == false && parseInt(aData.StagingID) > 0) {
                                            isStaging = true;
                                        }
                                    }
                                    else {
                                        isStaging = settings.IsStagingLocation
                                    }
                                    $.ajax({
                                        "url": $.trim(settings.AjaxURL),
                                        "data": { "NewBinNumber": $.trim($(obj).val()), "IsStaging": isStaging },
                                        "type": "POST",
                                        "dataType": "json",
                                        "success": function (response) {
                                            if (response.Status === "OK") {
                                                selectElement.append('<option value="' + response.BinDTO.ID + '" selected="true">' + $.trim($(obj).val()) + '</option>');
                                                if (settings.AddToOther && $.trim(settings.DataTableName) != "") {
                                                    var ddlID = $(selectElement).attr('id').split("_")[0];
                                                    $("#" + settings.DataTableName + ' tbody tr').each(function (e) {
                                                        var rowPosition = oTable.fnGetPosition(this);
                                                        var aData = oTable.fnGetData(rowPosition);
                                                        if (aData.SerialNumberTracking === "No" && aData.LotNumberTracking === "No" && aData.DateCodeTracking === "No") {
                                                            if (isStaging) {
                                                                if (isNaN(parseInt(aData.StagingID)) == false && parseInt(aData.StagingID) > 0) {
                                                                    var ddl = $(this).find("select[id^='" + ddlID + "']");
                                                                    if ($(ddl).attr('id') !== $(selectElement).attr('id')) {
                                                                        ddl.append('<option value="' + response.BinDTO.ID + '" >' + $.trim($(obj).val()) + '</option>');
                                                                    }
                                                                }
                                                            }
                                                            else {
                                                                if (isNaN(parseInt(aData.StagingID)) == true || parseInt(aData.StagingID) <= 0) {
                                                                    var ddl = $(this).find("select[id^='" + ddlID + "']");
                                                                    if ($(ddl).attr('id') !== $(selectElement).attr('id')) {
                                                                        ddl.append('<option value="' + response.BinDTO.ID + '" >' + $.trim($(obj).val()) + '</option>');
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    });
                                                }

                                            }
                                            else {
                                                alert(response.Message);
                                            }
                                        }
                                    });
                                }

                            }
                        } else {
                            if ($(this).val() == "") {
                                selectElement.children('option:selected').remove();
                            } else {
                                selectElement.children('option:selected').text($(this).val());
                            }
                        }
                        $(this).hide();
                        selectElement.focus();
                    }
                    resizeElements();
                });


                function resizeElements() {
                    wrapperElement.css({
                        "width": selectElement.outerWidth()
                    });
                    inputElement.css({
                        "width": selectElement.outerWidth()
                    });
                }


            }); // END RETURN 
        },
        destroy: function () {
            $(this).parent().remove();
        },
        disableAddingNewElements: function () {
            var fChild = $(this).children().first();
            if (fChild.text() == settings.NewElementText) {
                fChild.remove();
            }
        },
        enableAddingNewElements: function () {
            var fChild = $(this).children().first();
            if (fChild.text() != settings.NewElementText) {
                $(this).prepend('<option>' + settings.NewElementText + '</option>');
            }
        },
        disableEditingElements: function () {
            $(this).data('settings').editableElements = false;
        },
        enableEditingElements: function () {
            $(this).data('settings').editableElements = true;
        }
    };


    $.fn.eComboBox = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.eComboBox');
        }

    };

})(jQuery);