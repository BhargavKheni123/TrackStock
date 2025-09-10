var _roomRules = (function ($) {
    var self = {};

    var ddlModulesId = 'ddlModules';
    var tbodyRuleId = 'tbodyRule';
    var chkReqAllId = 'chkReqAll';

    self.init = function () {
        self.initEvents();
    }

    self.initEvents = function () {

        $('#' + ddlModulesId).change(function () {
            $("#" + chkReqAllId).prop('checked', false);
            self.bindColumns();
        });

        $("input#btnSave").click(function () {
            self.save();
        });

        $("input#" + chkReqAllId).click(function () {
            self.checkAllRequired();
        });

    }

    self.checkAllRequired = function () {
        var $chkReqAll = $("#" + chkReqAllId);

        var isChecked = $chkReqAll.is(":checked");
        $("input[type='checkbox'][data-type='req']:enabled").prop('checked', isChecked);
    }

    self.save = function () {
        var arrRulesDTOs = [];

        var rows = $("#" + tbodyRuleId).children("tr");

        $.each(rows, function (index, obj) {

            var $obj = $(obj);
            var reqChkBox = $obj.find("[data-type='req']");

            var rule = {};
            rule.ID = parseFloat(reqChkBox.attr("data-ID"));
            rule.DTOProperty = reqChkBox.attr("data-DTOProperty") ;
            rule.IsRequired = reqChkBox.prop("checked");
            rule.ValidationModuleID = reqChkBox.attr("data-ValidationModuleID");
            rule.DisplayOrder = parseInt(reqChkBox.attr("data-DisplayOrder"));
            rule.IsMasterDTO = reqChkBox.attr("data-IsMasterDTO") == 'true';
            //rule.DTOName = reqChkBox.attr("data-ID");
            arrRulesDTOs.push(rule);
        });

        if (arrRulesDTOs.length == 0) {
            _notification.showError("Invalid data to save");
            //_Common.showNotification("Invalid data to save");
            return;
        }

        showHideLoader(true);
        _AjaxUtil.postText('/ValidationRules/SaveRules',
            JSON.stringify({ rulesDTOs: arrRulesDTOs }),
            function (res) {
                showHideLoader(false);
                var r = JSON.parse(res);
                //alert(r.status);
                _notification.showSuccess(r.status);
                self.bindColumns(); // refresh grid
            },
            function (err) {
                showHideLoader(false);
                console.log(err);
            },true,false
        );
    }

    self.bindColumns = function () {

       
        var moduleId = $('#' + ddlModulesId).val();
        
        $("#" + tbodyRuleId).html("");
        $("#breadcrumb").text("");

        //if (DTOName.trim() == "") {
        //    return false;
        //}

        if (moduleId == "" || moduleId == null) {
            return false;
        }

        showHideLoader(true);

        _AjaxUtil.getJson('/ValidationRules/GetColumns',
            { moduleId: moduleId},
            function (res) {
                showHideLoader(false);
                var moduleDetail = res.ModuleDetail;
                var cols = res.ColumnList;
                var rows = "";
                $("#breadcrumb").text(moduleDetail.BreadCrumb);
                $.each(cols, function (index, obj) {
                    var cls = "odd";
                    if ((index + 1) % 2 == 0) {
                        cls = "even";
                    }

                    var reqChecked = "";
                    var lnkDel = "";

                    if (obj.IsRequiredDefault) {
                        // default checked and can not be changed
                        reqChecked = " checked=checked disabled=disabled "
                    }
                    else if (obj.IsRequired) {
                        reqChecked = " checked=checked "
                    }
                    
                    var rdata = " data-type='req' data-IsMasterDTO='" + obj.IsMasterDTO
                        + "' data-id='" + obj.ID
                        + "' data-DTOProperty='" + obj.DTOProperty
                        + "' data-IsRequired='" + obj.IsRequired
                        + "' data-IsRequiredDefault='" + obj.IsRequiredDefault
                        + "' data-ValidationModuleID='" + obj.ValidationModuleID 
                        + "' data-DisplayOrder='" + obj.DisplayOrder + "'";

                    var reqChkBox = "<input type='checkbox' " + rdata + " class='check-box' " + reqChecked + "/>";


                    if (obj.IsMasterDTO == false && obj.IsRequiredDefault == false) {
                        lnkDel = "<a id='delRule' onclick='return _roomRules.delete(this," + obj.ID + "," + obj.IsMasterDTO + ")' title='delete' href='#' id='deleteRows'><img src='/content/images/Delete.png' alt='Delete'></a>";
                    }

                    var peopertyName = obj.ColumnResName;
                    
                    rows += "<tr class='" + cls + "'>";
                    rows += "   <td class='read_only '>" + (index + 1) + "</td>";
                    //rows += "   <td class='read_only center'>" + lnkDel + "</td>";
                    rows += "   <td class='read_only '>" + peopertyName + "</td>";
                    rows += "   <td class='read_only '>" + reqChkBox + "</td>";
                    rows += "</tr>";
                })

                $("#" + tbodyRuleId).html(rows);
            },
            function (err) {
                showHideLoader(false);
            }

        )
    }

    self.delete = function (delCtl,id, isMaster) {

        if (isMaster) {
            _notification.showWarning(MsgRoomRulesNotificationPartial + "\n" + MsgSaveRecordsRulesNotification);
            return false;
        }

        var reqChkBox = $(delCtl).closest("tr").find("[data-type='req']");

        if (reqChkBox[0].dataset.isrequireddefault == true || reqChkBox[0].dataset.isrequireddefault == 'true') {
            _notification.showWarning(MsgRequiredColumnDeleteValidation);
            return false;
        }

        if (confirm(MsgDeleteRecordConfirmation)) {

            showHideLoader(true);

            _AjaxUtil.postJson('/ValidationRules/DeleteRule',
                { id: id },
                function (res) {
                    showHideLoader(false);
                    _Common.showNotification(res.status);
                    self.bindColumns(); // refresh grid
                },
                function (error) {
                    showHideLoader(false);
                }
            )
        }
        return false;
    }


    // -------- Private functions
    showHideLoader = function (isShow) {
        if (isShow) {
            $('#DivLoading').show();
        }
        else {
            $('#DivLoading').hide();
        }
    }

    return self;
})(jQuery);


    //$(document).ready(function () {
    //    var currentPageListId = $("select#PageNameList").val();
    //    if ($("select#PageNameList").val() == '')
    //    {
    //    $("select#PageNameList").val("4");
    //        currentPageListId = 4;
    //    }
    //    $.ajax({
    //    url: '@Url.Content("~/Master/BindColumns")',
    //        data: {currentPageListId: currentPageListId, isForeTurns: true },
    //        dataType: 'json',
    //        type: 'POST',
    //        async: false,
    //        cache: false,
    //        success: function (response) {
    //    bindColums(response.objSiteListColumnDetailDTO);

    //            $("select#ColumNameList").val(response.ColumnName.trim());
    //            $("select#OrderBy").val(response.SortType);
    //            $("select#PageSize").val(response.Pagesize);
    //            var result = response;
    //            $("#breadcrumb").text(result.ListBreadCrumb);
    //            $('#ColumnSortable').sortable({

    //});
    //        }
    //    });
    //});

    //$("select#PageNameList").change(function () {
    //    var currentPageListId = $(this).val();
    //    if (currentPageListId > 0) {
    //    $("div.listing").show();
    //    }
    //    else {
    //    $("div.listing").hide();
    //    }
    //    $('#ColumnSortable li').each(function (index) {
    //    $(this).remove();
    //    });
    //    $.ajax({
    //    url: '@Url.Content("~/Master/BindColumns")',
    //        data: {currentPageListId: currentPageListId, isForeTurns: true },
    //        dataType: 'json',
    //        type: 'POST',
    //        async: false,
    //        cache: false,
    //        success: function (response) {
    //    bindColums(response.objSiteListColumnDetailDTO);

    //            $("select#ColumNameList").val(response.ColumnName.trim());
    //            $("select#OrderBy").val(response.SortType);
    //            $("select#PageSize").val(response.Pagesize);
    //            var result = response;
    //            $("#breadcrumb").text(result.ListBreadCrumb);
    //            $('#ColumnSortable').sortable({

    //});
    //        }
    //    });
    //});
    //function sort(result) {
    //    var sortableList = $('#ColumnSortable');
    //    var listitems = $('li', sortableList);

    //    var i = -1;
    //    listitems.sort(function (a, b) {
    //        return ($(a).text().toUpperCase() > $(b).text().toUpperCase()) ? 1 : -1;
    //    });
    //    sortableList.append(listitems);

    //}

    //$("input#btnSave").click(function () {

    //    var sortelist = '';
    //    var visible = '';
    //    var colsizelist = '';
    //    var orderNumberCounter = 0;
    //    $('#ColumnSortable li').each(function () {
    //        var tmpVisibility = "";
    //        if ($('input[data-OrdNo="' + orderNumberCounter + '"]').is(":checked")) {
    //    tmpVisibility = "true";
    //        }
    //        else {
    //    tmpVisibility = "false";
    //        }

    //        if (visible != '') {
    //    visible = visible + ',' + tmpVisibility;
    //        }
    //        else {
    //    visible = tmpVisibility;
    //        }
    //        orderNumberCounter++;

    //        if (sortelist != '') {
    //    sortelist = sortelist + ',' + $(this).children("input#OrderNumber").val();
    //        }
    //        else {
    //    sortelist = $(this).children("input#OrderNumber").val();
    //        }
    //        if (colsizelist != '') {
    //    colsizelist = colsizelist + ',' + $(this).children("input#txtSize").val();
    //        }
    //        else {
    //    colsizelist = $(this).children("input#txtSize").val();
    //        }
    //    });

    //    var pageSize = $("select#PageSize").val();
    //    var currentPageListId = $("select#PageNameList").val();
    //    var SortColumn = $("select#ColumNameList").val();
    //    var SortColumnIndex = $("ul#ColumnSortable").find("label[text=" + SortColumn + "]").next("input[type=hidden]").val();
    //    var sortColumnIndexValue = $('option[value=' + SortColumn + ']').data('number');
    //    var Sorttype = $("select#OrderBy").val();

    //    $.ajax({
    //    url: '@Url.Content("~/Master/SaveJsonData")',
    //        data: {sortelist: sortelist, visible: visible, pageSize: pageSize, currentPageListId: currentPageListId, SortColumn: SortColumn, Sorttype: Sorttype, colsizelist: colsizelist, SortColumnIndex: sortColumnIndexValue, isForeTurns: true },
    //        dataType: 'json',
    //        type: 'POST',
    //        async: false,
    //        cache: false,
    //        success: function (response) {
    //    alert("Record Saved Successfully...!!!!");
    //        },
    //        complete: function () {
    //    alert("Record Saved Successfully...!!!!");
    //        }
    //    });
    //});
    //function bindColums(response) {
    //    $('#ColumnSortable').html("");
    //    $('#ColumNameList').html("");

    //    for (var i = 0; i < response.length; {

    //        if (response[i].ActualColumnName != null && response[i].ActualColumnName != "" && response[i].ActualColumnName.length > 0) {
    //    $("#ColumNameList").append("<option  value='" + response[i].ActualColumnName.trim() + "' data-number=" + response[i].OrderNumber + " >" + response[i].ColumnName.trim() + "</option>");
    //        }

    //        if (response[i].Visibility) {
    //            if (response[i].IsVisibilityEditable == 1) {
    //    $('#ColumnSortable').append('<li class="ui-state-default" ><input type="checkbox"  class="checkBox" checked="checked" id="' + response[i].ID + '" data-OrdNo="' + response[i].OrderNumber + '" /><label for="' + response[i].ID + '">' + response[i].ColumnName.trim() + '</label><input type="hidden" id="OrderNumber"  value="' + response[i].OrderNumber + '" /><input type="hidden" id="LastOrder" value="' + response[i].LastOrder + '" /><label style="float:right">px</label><input type="textbox" id="txtSize" style="width:45px;float:right;" maxlength="3" value="' + response[i].ColumnSize + '" /></li>');
    //            }
    //            else {
    //    $('#ColumnSortable').append('<li class="ui-state-default" ><input type="checkbox"  class="checkBox" checked="checked" disabled="disabled" id="' + response[i].ID + '"  data-OrdNo="' + response[i].OrderNumber + '" /><label for="' + response[i].ID + '">' + response[i].ColumnName.trim() + '</label><input type="hidden" id="OrderNumber"  value="' + response[i].OrderNumber + '" /><input type="hidden" id="LastOrder" value="' + response[i].LastOrder + '" /><label style="float:right">px</label><input type="textbox" id="txtSize" style="width:45px;float:right;" maxlength="3" value="' + response[i].ColumnSize + '" /></li>');
    //            }

    //            $('#ColumNameList option').each(function () {
    //                if ($(this).val() == response[i].ColumnName.trim()) {
    //    $(this).show();
    //                }
    //            });
    //        }
    //        else {
    //            if (response[i].IsVisibilityEditable == 1)
    //            {
    //    $('#ColumnSortable').append('<li class="ui-state-default" ><input type="checkbox" class="checkBox" id="' + response[i].ID + '" data-OrdNo="' + response[i].OrderNumber + '" /><label for="' + response[i].ID + '">' + response[i].ColumnName.trim() + '</label><input type="hidden" id="OrderNumber"  value="' + response[i].OrderNumber + '" /><input type="hidden" id="LastOrder"  value="' + response[i].LastOrder + '" /><label style="float:right">px</label><input type="textbox" id="txtSize" style="width:45px;float:right;" maxlength="3" value="' + response[i].ColumnSize + '" /></li>');
    //            }
    //            else
    //            {
    //    $('#ColumnSortable').append('<li class="ui-state-default" ><input type="checkbox" disabled="disabled" class="checkBox" id="' + response[i].ID + '" data-OrdNo="' + response[i].OrderNumber + '" /><label for="' + response[i].ID + '">' + response[i].ColumnName.trim() + '</label><input type="hidden" id="OrderNumber"  value="' + response[i].OrderNumber + '" /><input type="hidden" id="LastOrder"  value="' + response[i].LastOrder + '" /><label style="float:right">px</label><input type="textbox" id="txtSize" style="width:45px;float:right;" maxlength="3" value="' + response[i].ColumnSize + '" /></li>');
    //            }

    //            $('#ColumNameList option').each(function () {
    //                if ($(this).val() == response[i].ColumnName.trim()) {
    //    $(this).hide();
    //                }
    //            });
    //        }
    //    }
    //    $('#ColumnSortable li input[type=checkbox]').click(function () {

    //        var column = $(this).parents("li").find("label:first").text();
    //        if ($(this).is(":checked")) {
    //    $('#ColumNameList option').each(function () {
    //        if ($(this).val() == column) {
    //            $(this).show();
    //        }
    //    });
    //        }
    //        else {
    //    $('#ColumNameList option').each(function () {
    //        if ($(this).val() == column) {
    //            $(this).hide();
    //        }
    //    });
    //        }
    //        if ($('#ColumNameList').val() == column) {
    //            var val = $('#ColumNameList option:first').attr("value");
    //            $('#ColumNameList').prop('selectedIndex', 0);
    //            $('#ColumNameList').val(val);
    //        }
    //    });
    //}
