var SelectedCompanyIDs = "";
var IsFirstTime = true;
var SelectedRoomIDs = "";
var IsRoomFirstTime = true;
function GetMultiSelectCompanyDD(_CntrlName, _CntrlNameCollapse, _TableName, _ComapnyIds, _DisplayName) {
    
    $.ajax({ 'url': "/Master/GetMultiSelectDD",
        data: { TableName: _TableName, CompanyIds: _ComapnyIds },
        success: function (response) {
            
            var s = '';
            $.each(response.DDData, function (i, val) {
                s += '<option value="' + val + '">' + i + '</option>';
            });
            //Destroy widgets before reapplying the filter
            $("#" + _CntrlName).empty();
            $("#" + _CntrlName).multiselect('destroy');
            $("#" + _CntrlName).multiselectfilter('destroy');

            $("#" + _CntrlName).append(s);
            $("#" + _CntrlName).multiselect(
                        { noneSelectedText: _DisplayName, selectedList: 5,
                            selectedText: function (numChecked, numTotal, checkedItems) {
                                return numChecked + ' ' + _DisplayName + ' ' + selected;
                            }
                        },
                        {
                            checkAll: function (ui) {
                                //                                $("#" + _CntrlNameCollapse).html('');
                                //                                for (var i = 0; i <= ui.target.length - 1; i++) {
                                //                                    if ($("#" + _CntrlNameCollapse).text().indexOf(ui.target[i].text) == -1) {
                                //                                        $("#" + _CntrlNameCollapse).append("<span>" + ui.target[i].text + "</span>");
                                //                                    }
                                //                                }
                                //                                $("#" + _CntrlNameCollapse).show();
                            }
                        }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                //                if (ui.checked) {
                //                    if ($("#" + _CntrlNameCollapse).text().indexOf(ui.text) == -1) {
                //                        $("#" + _CntrlNameCollapse).append("<span>" + ui.text + "</span>");
                //                    }
                //                }
                //                else {
                //                    if (ui.checked == undefined) {
                //                        $("#" + _CntrlNameCollapse).html('');
                //                    }
                //                    else if (!ui.checked) {
                //                        var text = $("#" + _CntrlNameCollapse).html();
                //                        text = text.replace("<span>" + ui.text + "</span>", '');
                //                        $("#" + _CntrlNameCollapse).html(text);
                //                    }
                //                    else
                //                        $("#" + _CntrlNameCollapse).html('');
                //                }

                SelectedCompanyIDs = $.map($(this).multiselect("getChecked"), function (input) {
                    //return input.title;
                    return input.value;
                })

                //                if ($("#" + _CntrlNameCollapse).text().trim() != '')
                //                    $("#" + _CntrlNameCollapse).show();
                //                else
                //                    $("#" + _CntrlNameCollapse).hide();

                //                if ($("#" + _CntrlNameCollapse).find('span').length <= 2) {
                //                    $("#" + _CntrlNameCollapse).scrollTop(0).height(50);
                //                }
                //                else {
                //                    $("#" + _CntrlNameCollapse).scrollTop(0).height(100);
                //                }
                GetMultiSelectRoomDD("RoomGlobalReprotBuilder", "RoomGlobalReprotBuilderCollapse", "Room", SelectedCompanyIDs.toString(), "Room");
            }).multiselectfilter();
            if (IsFirstTime) {
                setTimeout(SetRoomData, 1000);
                IsFirstTime = false;
            }
        },
        error: function (response) {
            // through errror message
        }
    });
    function SetRoomData() {
        $("#ComapnyGlobalReprotBuilder").multiselect("widget").find(":checkbox[value='" + $("#ddlCompany").val() + "']").each(function () {
            this.click();
        });
    }
}
function GetMultiSelectRoomDD(_CntrlName, _CntrlNameCollapse, _TableName, _ComapnyIds, _DisplayName) {

    $.ajax({ 'url': "/Master/GetMultiSelectDD",
        data: { TableName: _TableName, CompanyIds: _ComapnyIds },
        success: function (response) {

            var s = '';
            $.each(response.DDData, function (i, val) {
                s += '<option value="' + val + '">' + i + '</option>';
            });
            //Destroy widgets before reapplying the filter
            $("#" + _CntrlName).empty();
            $("#" + _CntrlName).multiselect('destroy');
            $("#" + _CntrlName).multiselectfilter('destroy');

            $("#" + _CntrlName).append(s);
            $("#" + _CntrlName).multiselect(
                        { noneSelectedText: _DisplayName, selectedList: 5,
                            selectedText: function (numChecked, numTotal, checkedItems) {
                                return numChecked + ' ' + _DisplayName + ' ' + selected;
                            }
                        },
                        {
                            checkAll: function (ui) {
                                //                                $("#" + _CntrlNameCollapse).html('');
                                //                                for (var i = 0; i <= ui.target.length - 1; i++) {
                                //                                    if ($("#" + _CntrlNameCollapse).text().indexOf(ui.target[i].text) == -1) {
                                //                                        $("#" + _CntrlNameCollapse).append("<span>" + ui.target[i].text + "</span>");
                                //                                    }
                                //                                }
                                //                                $("#" + _CntrlNameCollapse).show();
                            }
                        }
            )
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                //                if (ui.checked) {
                //                    if ($("#" + _CntrlNameCollapse).text().indexOf(ui.text) == -1) {
                //                        $("#" + _CntrlNameCollapse).append("<span>" + ui.text + "</span>");
                //                    }
                //                }
                //                else {
                //                    if (ui.checked == undefined) {
                //                        $("#" + _CntrlNameCollapse).html('');
                //                    }
                //                    else if (!ui.checked) {
                //                        var text = $("#" + _CntrlNameCollapse).html();
                //                        text = text.replace("<span>" + ui.text + "</span>", '');
                //                        $("#" + _CntrlNameCollapse).html(text);
                //                    }
                //                    else
                //                        $("#" + _CntrlNameCollapse).html('');
                //                }

                SelectedRoomIDs = $.map($(this).multiselect("getChecked"), function (input) {
                    return input.value;
                })

                //                if ($("#" + _CntrlNameCollapse).text().trim() != '')
                //                    $("#" + _CntrlNameCollapse).show();
                //                else
                //                    $("#" + _CntrlNameCollapse).hide();

                //                if ($("#" + _CntrlNameCollapse).find('span').length <= 2) {
                //                    $("#" + _CntrlNameCollapse).scrollTop(0).height(50);
                //                }
                //                else {
                //                    $("#" + _CntrlNameCollapse).scrollTop(0).height(100);
                //}
            }).multiselectfilter();
            if (IsRoomFirstTime) {
                setTimeout(SetRoomDefault, 1000);
                IsRoomFirstTime = false;
            }
            $('#DivLoading').hide();
        },
        error: function (response) {
            // through errror message
        }
    });
}
function SetRoomDefault() {
    $("#RoomGlobalReprotBuilder").multiselect("widget").find(":checkbox[value='" + $("#ddlStockroom").val() + "']").each(function () {
        this.click();
    });
}