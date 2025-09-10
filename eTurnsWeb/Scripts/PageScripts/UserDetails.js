
var _CreateUser = (function ($) {

    var self = {};
    self.selectedBillRoomID = '';

    self.init = function () {

    }

    self.EnableDisableRoomsControls = function () {
        var SelectedRoomscnt = $("#ddlSelectedRooms > option").length;

        if (SelectedRoomscnt == 0) {
            $('#CreateRolePermissionDIV :input').attr('disabled', true);
        }
        else {

            $('#CreateRolePermissionDIV :input').removeAttr('disabled');
            self.getRoomEnabledModules();
        }
    }

    

    self.getEntCompRoom = function (selId) {
        var sp = selId.split('_');
        return { EntId: sp[0], CompId: sp[1], RoomId: sp[2] };
    }

    self.getRoomEnabledModules = function () {

        var selId = $("#ddlSelectedRooms").val();
               

        if (_utils.isNullUndefined(selId) ) {
            self.selectedBillRoomID = '';
            return;
        }

        if (selId == "") {
            return;
        }

        if (self.selectedBillRoomID == selId) {
            return;
        }

        self.selectedBillRoomID = selId;
        

        var selECR = self.getEntCompRoom(selId);

        _Common.showHideLoader(true);

        _AjaxUtil.getJson('/BillingTypeModules/GetBillingRoomModules'
            , { roomId: selECR.RoomId, compId: selECR.CompId, entId: selECR.EntId }
            , function (res) {
                _Common.showHideLoader(false);
                var list = res.list;
                //self.selectedBillRoomID = '';
                if (typeof list !== 'undefined' && list != null && list.length) {

                    var len = list.length;
                    for (var i = 0; i < len; i++) {
                        var obj = list[i];
                        if (obj.ModuleID == AllUDFSetUpModuleID) {
                            if (obj.IsModuleEnabled == false) {
                                $("input:checkbox._ShowUDFAll").removeAttr("checked");
                                $("input:checkbox._ShowUDFAll").attr({ 'disabled': 'disabled', 'IsModuleDisable': true });
                                $("input:checkbox._ShowUDFAll").parent("td").css('background-color', '#FF6347');
                                var RoomTab2SelectAllUDF = $("#Roomtab2 th input[type='checkbox'][id='AllShowUDF$Tab2']");
                                if (RoomTab2SelectAllUDF.length > 0) {
                                    RoomTab2SelectAllUDF.removeAttr("checked");
                                    RoomTab2SelectAllUDF.attr({ 'disabled': 'disabled', 'IsModuleDisable': true });
                                    RoomTab2SelectAllUDF.parent("th").css('background-color', '#FF6347');
                                }
                                var RoomTab3SelectAllUDF = $("#Roomtab3 th input[type='checkbox'][id='AllShowUDF$Tab3']");
                                if (RoomTab3SelectAllUDF.length > 0) {
                                    RoomTab3SelectAllUDF.removeAttr("checked");
                                    RoomTab3SelectAllUDF.attr({ 'disabled': 'disabled', 'IsModuleDisable': true });
                                    RoomTab3SelectAllUDF.parent("th").css('background-color', '#FF6347');
                                }
                            }

                        } else {
                            var chkList = $("input:checkbox[id^='" + obj.ModuleID + "_']");

                            if (chkList.length == 0) {
                                continue;
                            }

                            if (obj.IsModuleEnabled == false) {
                                var moduleId = obj.ModuleID;
                                chkList.attr({ 'disabled': 'disabled', 'IsModuleDisable': true });
                                chkList.parent("td").css('background-color', '#FF6347');
                            }
                        }
                        //else {
                        //    chkList.removeAttr('disabled');
                        //    chkList.parent("td").css('background-color', '');
                        //}
                    }


                }

            }, function (error) {
                _Common.showHideLoader(false);
            }, true, false);
    }

    self.SaveToUserPermissionsToSession = function (RoomID, RoleID,  SelectedRoomSupplier
        ,fnSuccess, fnError) {

        var SelectedModuleList = hdnSelectedModuleList.val();
        var SelectedNonModuleList = hdnSelectedNonModuleList.val();
        var SelectedDefaultSettings = hdnSelectedDefaultSettings.val();

        $.ajax({
            type: "POST",
            url: url_SaveToUserPermissionsToSession,
            //data: "{'RoomID': '" + hdnCurrentSelectedRoom + "' ,'RoleID':'" + hdnRoleID.val() + "','SelectedModuleList':'" + hdnSelectedModuleList.val() + "','SelectedNonModuleList':'" + hdnSelectedNonModuleList.val() + "','SelectedDefaultSettings':'" + hdnSelectedDefaultSettings.val() + "','SelectedRoomSupplier':'" + SelectedRoomSupplier + "'}",
            data: JSON.stringify({
                RoomID: RoomID,
                RoleID: RoleID,
                SelectedModuleList: SelectedModuleList,
                SelectedNonModuleList: SelectedNonModuleList,
                SelectedDefaultSettings: SelectedDefaultSettings,
                SelectedRoomSupplier: SelectedRoomSupplier
            }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (data) {
                if ($.isFunction(fnSuccess)) {
                    fnSuccess(data);
                }
            },
            error: function (response) {
                // through errror message
                if ($.isFunction(fnError)) {
                    fnError(response);
                }
            }
        });
    }

    self.GetUserID = function () {
        var UserID = 0;
        var $hiddenID = $("#hiddenID");
        if ($hiddenID.val() != '') {
            UserID = $hiddenID.val();
        }
        return UserID;
    }

    self.GetSelectedRoleID = function() {
        var SelectedRoleID = 0; // $("#ddlSelectedRooms").val();
        $("#ddlRole > option").each(function () {
            if (this.selected == true) {
                SelectedRoleID = this.value;
            }
        });
        $("#hdnRoleID").val(SelectedRoleID);
        return SelectedRoleID;
    }


    self.SetSelectedModule_NonModulePermissions = function() {

        hdnSelectedModuleList.val('');
        hdnSelectedNonModuleList.val('');
        hdnSelectedDefaultSettings.val('');

        SelectedNonModuleList = [];
        SelectedModuleList = [];
        SelectedDefaultSettings = [];

        var orderDollerLimit = '';
        $("#CreateRolePermissionDIV input:checkbox").each(function () {
            var ControlID = this.id.toLowerCase();
            if (ValidateCheckBox(ControlID) == true) {
                if (this.checked == true) {
                    if (ControlID.toLowerCase().indexOf("_ischecked") != -1) {
                        SelectedNonModuleList.push(ControlID);
                    }
                    else {
                        SelectedModuleList.push(ControlID);
                    }
                }
            }
        });

        $("#CreateRolePermissionDIV input:text").each(function () {
            if (this.value != '') {
                var txtcnt = "";
                if (this.id == "99") {
                    txtcnt = this.id + '#' + this.value;
                }
                //else {
                //    txtcnt = this.id + '#' + this.value;
                //}
                SelectedDefaultSettings.push(txtcnt);
            }
        });
        $("#CreateRolePermissionDIV select").each(function (indx, slctobj) {
            if ($(this).val() != '') {
                var txtcnt = "";
                if ($(this).attr("id") == "99") {
                    //txtcnt = $(this).attr("id") + '#' + $(this).val();
                    $("ul.ui-multiselect-checkboxes").find("input[name=multiselect_99]").each(function () {

                        if ($(this).is(":checked")) {
                            if ($(this).val() != '')
                                txtcnt += ($(this).val() + "@");
                        }
                    });
                    SelectedDefaultSettings.push($(this).attr("id") + "#" + txtcnt);
                }
            }
        });
        //$("ul.ui-multiselect-checkboxes").find("input[name=multiselect_99]").click(function () {
        //    var currentVal = $(this).val();
        //    if ($(this).is(":checked")) {
        //        $("ul.ui-multiselect-checkboxes").find("input[name=multiselect_99]").each(function () {
        //            if ($(this).val() != currentVal)
        //                $(this).removeAttr("checked");
        //            else
        //                $(this).attr("checked","checked");
        //        });
        //    }
        //});


        var vtmFreq = "0";
        var vtmUnit = "0";
        var vdrLimit = "0";
        var vusedLimit = "0";
        var vlimitType = "1";
        var vItemQtyLimit = "0";


        var rdoOption = $('#CreateRolePermissionDIV').find('input[name=OrderLimitOption]:checked').val();

        if (rdoOption == "NoLimit") {
            orderDollerLimit = "125#NoLimit";
        }
        else {

            if ($('#CreateRolePermissionDIV').find('#txtTimeFrequency').val().length > 0) {
                vtmFreq = $('#txtTimeFrequency').val();
            }

            if ($('#CreateRolePermissionDIV').find('#ddlTimeBasedUnit').val().length > 0) {
                vtmUnit = $('#ddlTimeBasedUnit').val();
            }

            if ($('#CreateRolePermissionDIV').find('#txtorderDollerLimit').val().length > 0) {
                vdrLimit = $('#txtorderDollerLimit').val();
            }

            if ($('#CreateRolePermissionDIV').find('#txtorderUsedLimit').val().length > 0) {
                vusedLimit = $('#txtorderUsedLimit').val();
            }

            if ($('#CreateRolePermissionDIV').find('#ddlOrderLimitType').val().length > 0) {
                vlimitType = $('#ddlOrderLimitType').val();
            }

            if ($('#CreateRolePermissionDIV').find('#txtItemOrderQuantityLimit').val().length > 0) {
                vItemQtyLimit = $('#txtItemOrderQuantityLimit').val();
            }

            if (parseFloat(vtmFreq) <= 0 || parseFloat(vtmUnit) <= 0 || (parseFloat(vdrLimit) <= 0 && parseFloat(vItemQtyLimit) <= 0))
                orderDollerLimit = "125#NoLimit";
            else
                orderDollerLimit = "125#" + vtmFreq + "|" + vtmUnit + "|" + vdrLimit + "|" + vusedLimit + "|" + vlimitType + "|" + vItemQtyLimit;
        }
        //alert(orderDollerLimit);
        SelectedDefaultSettings.push(orderDollerLimit);

        if (SelectedModuleList.toString() != '') {
            hdnSelectedModuleList.val(SelectedModuleList.toString());
        }
        if (SelectedNonModuleList.toString() != '') {
            $("#hdnSelectedNonModuleList").val(SelectedNonModuleList.toString());
        }

        if (SelectedDefaultSettings.toString() != '') {

            $("#hdnSelectedDefaultSettings").val(SelectedDefaultSettings.toString());
        }
    }

    self.ClearAllRolesDropdown = function () {

        $("#ddlDefaultPermissionRooms > option").remove();
        $("#ddlDefaultPermissionRooms").multiselect("refresh");


        $("#EnterpriseData > option").remove();
        $("#EnterpriseData").multiselect("refresh");

        $("#CompanyData > option").remove();
        $("#CompanyData").multiselect("refresh");

        $("#RoomData > option").remove();
        $("#RoomData").multiselect("refresh");

        $("#RoomReplanishment > option").remove();
        $("#RoomReplanishment").multiselect("refresh");

        $("#ddlSelectedRooms > option").remove();
        $("#ddlSelectedRooms").multiselect("refresh");

        $("#RoomDataSupplier > option").remove();
        $("#RoomDataSupplier").multiselect("refresh");

        $("#CreateRolePermissionDIV input:checkbox").each(function () {
            this.checked = false;
        });

        $("#CreateRolePermissionDIV input:text").each(function () {
            this.value = '';
        });

    }

    self.selectedRole = {
        data: null,
        getCompanyByEnt: function (arrEnt) {
            var list = this.data.CompanyList;
            var filterList = $.grep(list, function (obj, index) {
                for (var i = 0; i < arrEnt.length; i++) {
                    if (parseFloat(arrEnt[i]) == obj.EnterPriseId) {
                        return true;
                    }
                }
            });
            return filterList;
        },
        getRoomByEntComp: function (arrEntComp) {
            var list = this.data.RoomList;
            var filterList = $.grep(list, function (obj, index) {
                for (var i = 0; i < arrEntComp.length; i++) {
                    if (arrEntComp[i] == obj.EnterPriseId + "_" + obj.CompanyId) {
                        return true;
                    }
                }
            });
            return filterList;
        }
    }

    self.setCompanyDdlMultiSelect = function () {
        $("#CompanyData").multiselect(
            {
                noneSelectedText: 'Company Access', selectedList: 5,
                selectedText: function (numChecked, numTotal, checkedItems) {
                    return 'Company Access';
                }
            }

        ).unbind("multiselectclick multiselectcheckall multiselectuncheckall")
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
           self.CheckCompanyClick(ui, event);
        }).multiselectfilter();
    }

    self.setRoomDdlMultiSelect = function () {
        $("#RoomData").multiselect(
            {
                noneSelectedText: 'Room Access', selectedList: 5,
                selectedText: function (numChecked, numTotal, checkedItems) {
                    return 'Room Access';
                }
            }

        ).unbind("multiselectclick multiselectcheckall multiselectuncheckall")
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
            _CreateUser.SetSelectBox();//Checkclick(ui, event);
        }).multiselectfilter();
    }

    self.setCopyToRoomsMultiSelect = function () {
        $("#ddlDefaultPermissionRooms").multiselect(
            {
                noneSelectedText: 'Room  ', selectedList: 5,
                selectedText: function (numChecked, numTotal, checkedItems) { return 'Room : ' + numChecked + ' ' + selected; }
            }).unbind("multiselectclick multiselectcheckall multiselectuncheckall")
            .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                $.map($(this).multiselect("getChecked"), function (input) { return input.value; })
            });

    }

    

    /**
     * Rebind company muti select based on selected enterprise
     * */
    self.rebindCompanyDdl = function (selId, isChecked) {

        var arrEnt = _multiSelectWrapper.getCheckedValueArray('EnterpriseData');

        var companyList = self.selectedRole.getCompanyByEnt(arrEnt);

        self.bindCompanyDdl(companyList, true, true, selId, isChecked);

        self.rebindRoomDdl(selId, isChecked,false);
    }
        

    /**
     * Rebind room muti select based on selected company
     * */
    self.rebindRoomDdl = function (selId,isChecked,isCompanyDDLClick) {

        var arrEntComp = _multiSelectWrapper.getCheckedValueArray('CompanyData');
        
        var roomList = self.selectedRole.getRoomByEntComp(arrEntComp);

        self.bindRoomDdl(roomList, true, true, selId, isChecked, isCompanyDDLClick);
    }

    self.bindCompanyDdl = function (companyList, isDestroy, isFilter, selId, isChecked) {

        var checkedVals = [];
        if (isDestroy) {
            checkedVals = _multiSelectWrapper.getCheckedValueArray('CompanyData');
            _multiSelectWrapper.destroy('CompanyData');
            self.setCompanyDdlMultiSelect();
        }
        var UserType = $("#drpUserType").val();
        var s = '';
        var selectedEps = "";
        $.each(companyList, function (i, val) {
            var isSelectedEps = false;
            if (UserType == 1) {

                var selOption = '<option selected="selected" value="' + val.EnterPriseId_CompanyId + '">' + val.CompanyName + '(' + val.EnterPriseName + ')</option>';
                var nonSelOption = '<option value="' + val.EnterPriseId_CompanyId + '">' + val.CompanyName + '(' + val.EnterPriseName + ')</option>';

                if (isFilter) {
                    var id = val.EnterPriseId_CompanyId;
                    var entId = parseFloat(val.EnterPriseId_CompanyId.split('_')[0]);
                    // Enterprise Checkbox is selected then select all company with selected enterprise id
                    if (typeof selId !== 'undefined' && parseFloat(selId) == entId && isChecked) {
                        s += selOption;
                        isSelectedEps = true;
                    }
                    else {
                        // Enterprise Checkbox is un-selected , Maintain previous selected checkbox

                        if (selId == 0 && isChecked) {
                            // check all operation
                            s += selOption;
                            isSelectedEps = true;
                        }
                        else if (checkedVals.length) {
                           
                            var OldSelected = $.grep(checkedVals, function (obj, index) {
                                
                                return obj == id;
                            });

                            if (OldSelected.length) {
                                s += selOption;
                                isSelectedEps = true;
                            }
                            else {
                                s += nonSelOption;
                            }
                        }                        
                        else {
                            s += nonSelOption;
                        }
                    }
                }
                else if (val.IsSelected == true) {
                    s += selOption;
                    isSelectedEps = true;
                }
                else {
                    s += nonSelOption;
                }
            }
            else {
                var selOptionE = '<option selected="selected" value="' + val.EnterPriseId_CompanyId + '">' + val.CompanyName + '</option>';;
                var nonSelOptionE = '<option value="' + val.EnterPriseId_CompanyId + '">' + val.CompanyName + '</option>';

                if (isFilter) {
                    var entId = parseFloat(val.EnterPriseId_CompanyId.split('_')[0]);
                    if (typeof selId !== 'undefined' && parseFloat(selId) == entId && isChecked) {
                        s += selOptionE;
                        isSelectedEps = true;
                    }
                    else {
                        if (selId == 0 && isChecked) {
                            // check all operation
                            s += selOptionE;
                            isSelectedEps = true;
                        }
                        else if (checkedVals.length) {

                            var OldSelected = $.grep(checkedVals, function (obj, index) {

                                return obj == id;
                            });

                            if (OldSelected.length) {
                                s += selOptionE;
                                isSelectedEps = true;
                            }
                            else {
                                s += nonSelOptionE;
                            }
                        }
                        
                        else {
                            s += nonSelOptionE;
                        }
                    }
                }
                else if (val.IsSelected == true) {
                    s += selOptionE;
                    isSelectedEps = true;
                }
                else {
                    s += nonSelOptionE;
                }
            }

            if (isSelectedEps) {
                if (selectedEps == "") {
                    selectedEps = val.EnterPriseId_CompanyId + "_" + val.CompanyName;
                }
                else {
                    selectedEps += sep1 + val.EnterPriseId_CompanyId + "_" + val.CompanyName;
                }
            }
        });
        $("#hdnSelectedCompanyAccessValue").val(selectedEps);
        $("#CompanyData").html(s);
        $("#CompanyData").multiselect("refresh");

        
    }

    self.bindRoomDdl = function (roomList, isDestroy, isFilter, selId, isChecked, isCompanyDDLClick) {
        var selectedEps = '';
        var s = '';

        var checkedVals = [];
        if (isDestroy) {
            checkedVals = _multiSelectWrapper.getCheckedValueArray('RoomData');
            _multiSelectWrapper.destroy('RoomData');
            self.setRoomDdlMultiSelect();
        }
        var UserType = $("#drpUserType").val();
        $.each(roomList, function (i, val) {
            var isSelectedEps = false;


            if (UserType == 1 || UserType == 2) {
                var selOption = '<option selected="selected" value="' + val.EnterPriseId_CompanyId_RoomId + '">' + val.RoomName + '(' + val.CompanyName + ')</option>';
                var nonSelOption = '<option value="' + val.EnterPriseId_CompanyId_RoomId + '">' + val.RoomName + '(' + val.CompanyName + ')</option>';

                if (isFilter) {
                    
                    var id = val.EnterPriseId_CompanyId_RoomId;
                    
                    var entId = (val.EnterPriseId_CompanyId_RoomId.split('_')[0]);
                    var compId = (val.EnterPriseId_CompanyId_RoomId.split('_')[1]);


                    // Enterprise Checkbox is selected or Company Checkbox is selected 
                    // then select all room with selected id
                    if (typeof selId !== 'undefined'
                        && (
                        (selId == entId && isCompanyDDLClick == false) || (selId == entId + "_" + compId)
                    )
                        && isChecked) {
                        s += selOption;
                        isSelectedEps = true;
                    }
                    else {
                        // Enterprise Checkbox or Company Checkbox is un-selected , Maintain previous selected checkbox

                        if (selId == 0 && isChecked) {
                            // check all operation
                            s += selOption;
                            isSelectedEps = true;
                        }
                        else if (checkedVals.length) {

                            var OldSelected = $.grep(checkedVals, function (obj, index) {

                                return obj == id;
                            });

                            if (OldSelected.length) {
                                s += selOption;
                                isSelectedEps = true;
                            }
                            else {
                                s += nonSelOption;
                            }
                        }
                        
                        else {
                            s += nonSelOption;
                        }
                    }
                }
                else if (val.IsSelected == true) {
                    s += selOption;
                    isSelectedEps = true;
                }
                else {
                    s += nonSelOption;
                }
            }
            else {
                var selOptionE = '<option selected="selected" value="' + val.EnterPriseId_CompanyId_RoomId + '">' + val.RoomName + '</option>';
                var nonSelOptionE = '<option value="' + val.EnterPriseId_CompanyId_RoomId + '">' + val.RoomName + '</option>';


                if (isFilter) {
                    var id = val.EnterPriseId_CompanyId_RoomId;
                    var entId = (val.EnterPriseId_CompanyId_RoomId.split('_')[0]);
                    var compId = (val.EnterPriseId_CompanyId_RoomId.split('_')[1]);

                    // Company Checkbox is selected then select all company with selected enterprise id
                    if (typeof selId !== 'undefined' && selId == entId + "_" + compId && isChecked) {
                        s += selOptionE;
                        isSelectedEps = true;
                    }
                    else {
                        // Company Checkbox is un-selected , Maintain previous selected checkbox

                        if (selId == 0 && isChecked) {
                            // check all operation
                            s += selOption;
                            isSelectedEps = true;
                        }
                        else if (checkedVals.length) {

                            var OldSelected = $.grep(checkedVals, function (obj, index) {

                                return obj == id;
                            });

                            if (OldSelected.length) {
                                s += selOptionE;
                                isSelectedEps = true;
                            }
                            else {
                                s += nonSelOptionE;
                            }
                        }                        
                        else {
                            s += nonSelOptionE;
                        }
                    }
                }
                else if (val.IsSelected == true) {
                    s += selOptionE;
                    isSelectedEps = true;
                }
                else {
                    s += nonSelOptionE;
                }
            }

            if (isSelectedEps) {
                if (selectedEps == '') {
                    selectedEps = val.EnterPriseId_CompanyId_RoomId + "_" + val.RoomName;
                }
                else {
                    selectedEps += sep1 + val.EnterPriseId_CompanyId_RoomId + "_" + val.RoomName;
                }
            }
        });
        hdnSelectedRoomAccessValue.val(selectedEps);
        $("#RoomData").html(s);
        $("#RoomData").multiselect("refresh");

        

    }

    self.RoleSelection = function () {
        self.ClearAllRolesDropdown();
        var SelectedRoleID = _CreateUser.GetSelectedRoleID();
        var UserType = $("#drpUserType").val();
        var SelecteUserID = _CreateUser.GetUserID();
        if (SelectedRoleID > 0) {
            _Common.showHideLoader(true); //$('#DivLoading').show();
            $.ajax({
                url: url_GetRoleDetailsInfo,
                type: 'POST',
                data: { RoleID: SelectedRoleID, UserType: UserType, UserId: SelecteUserID },
                success: function (response) {

                    self.selectedRole.data = response;

                    var s = '';
                    var selectedEps = "";
                    $.each(response.EnterPriseList, function (i, val) {
                        if (val.IsSelected == true) {
                            s += '<option selected="selected" value="' + val.EnterPriseId + '">' + val.EnterPriseName + '</option>';
                        }
                        else {
                            s += '<option value="' + val.EnterPriseId + '">' + val.EnterPriseName + '</option>';
                        }

                        if (selectedEps == "") {
                            selectedEps = val.EnterPriseId + "_" + val.EnterPriseName;
                        }
                        else {
                            selectedEps += sep1 + val.EnterPriseId + "_" + val.EnterPriseName;
                        }
                    });
                    $("#hdnSelectedEnterpriseAccessValue").val(selectedEps);
                    $("#EnterpriseData").html(s);
                    $("#EnterpriseData").multiselect("refresh");

                    var arrEnt = _multiSelectWrapper.getCheckedValueArray('EnterpriseData');
                    var CompanyList = self.selectedRole.getCompanyByEnt(arrEnt);

                    self.bindCompanyDdl(CompanyList, false, false);

                    var arrEntComp = _multiSelectWrapper.getCheckedValueArray('CompanyData');
                    var RoomList = self.selectedRole.getRoomByEntComp(arrEntComp);

                    self.bindRoomDdl(RoomList,false,false);

                    //resetCompanyonRoomSelection();

                    selectedEps = '';
                    s = '';

                    if (response.ReplenishList != null) {
                        $.each(response.ReplenishList, function (i, val) {
                            if (UserType == 1 || UserType == 2) {
                                s += '<option value="' + val.EnterPriseId_CompanyId_RoomId + '">' + val.RoomName + '(' + val.CompanyName + ')</option>';
                            } else {
                                s += '<option value="' + val.EnterPriseId_CompanyId_RoomId + '">' + val.RoomName + '</option>';
                            }
                            if (selectedEps == '') {
                                selectedEps = val.EnterPriseId_CompanyId_RoomId + "_" + val.RoomName;
                            }
                            else {
                                selectedEps += sep1 + val.EnterPriseId_CompanyId_RoomId + "_" + val.RoomName;
                            }
                        });
                    }

                    $("#RoomReplanishment").html(s);
                    $("#RoomReplanishment").multiselect("refresh");
                    $("#hdnSelectedRoomReplanishmentValue").val(selectedEps);
                    //                    EditModeSetRoomReplanishmentData();
                    //New change

                    //New change

                    ResetRoomAccessSelection();
                    _Common.showHideLoader(false); //$('#DivLoading').hide();
                },
                error: function (response) {
                    // through errror message
                    _Common.showHideLoader(false); //$('#DivLoading').hide();
                }
            });
            //            EditModeSetRoomReplanishmentData();
        }
    }

    /**
     * Enterprise checkbox check
     * @param {any} chkDropdown
     * @param {any} event
     */
    self.CheckEnterpriseClick = function (chkDropdown, event) {
        var arrItems = new Array();
        //if (event.type == 'multiselectcheckall') {
        $("#EnterpriseData > option").each(function () {
            if (this.selected == true)
                arrItems.push(this.value);
        });

        if (chkDropdown.checked == true) {
            arrItems.push(chkDropdown.value);
        }
        else {
            var selected = chkDropdown.value;
            arrItems = jQuery.grep(arrItems, function (value) {
                return value != selected;
            });
        }
        if (chkDropdown.selected == true) {
            arrItems.push(chkDropdown.value);
        }
        SelectedEnterprises = arrItems;
        $("#hdnSelectedEnterpriseAccessValue").val(SelectedEnterprises.join(sep1));

        
        if (event.type == 'multiselectcheckall' ) {

            self.rebindCompanyDdl(0,true);
            //self.rebindRoomDdl(0, true);

        }
        else if (event.type == 'multiselectuncheckall') {
            self.rebindCompanyDdl(0, false);
            //self.rebindRoomDdl(0, false);
        }
        else {
            // single checkbox click in enterprise
            var valuesToselect = chkDropdown.value;
            var isChecked = $(event.srcElement).is(":checked");
            self.rebindCompanyDdl(valuesToselect, isChecked);
            //self.rebindRoomDdl(valuesToselect, isChecked);
        }
        self.SetSelectBox();
    }

    self.CheckCompanyClick = function (chkDropdown, event) {
        var arrItems = new Array();
        //if (event.type == 'multiselectcheckall') {
        $("#CompanyData > option").each(function () {
            if (this.selected == true)
                arrItems.push(this.value);
        });

        if (chkDropdown.checked == true) {
            arrItems.push(chkDropdown.value);
        }
        else {
            var selected = chkDropdown.value;
            arrItems = jQuery.grep(arrItems, function (value) {
                return value != selected;
            });
        }
        if (chkDropdown.selected == true) {
            arrItems.push(chkDropdown.value);
        }
        SelectedCompanies = arrItems;
        $("#hdnSelectedCompanyAccessValue").val(SelectedCompanies.join(sep1));

        
        if (event.type == 'multiselectcheckall') {

            self.rebindRoomDdl(0, true, true);

        }
        else if (event.type == 'multiselectuncheckall') {

            self.rebindRoomDdl(0, false, true);
        }
        else {
            // single checkbox click in enterprise
            var valuesToselect = chkDropdown.value;
            var isChecked = $(event.srcElement).is(":checked");

            self.rebindRoomDdl(valuesToselect, isChecked, true);
        }

        
        self.SetSelectBox();

    }

    self.SetSelectBox = function () {
        $("#ddlSelectedRooms").html("");
        $("#RoomDataSupplier").html("");
        SelectedRooms = [];
        var checkedList = $("#RoomData").multiselect("getChecked");
        $(checkedList).each(function (indx, obj) {
            var ss = '<option value="' + obj.value + '">' + obj.title + '</option>';
            if (indx == 0) {
                ss = '<option value="' + obj.value + '" selected="selected">' + obj.title + '</option>'
            }
            $("#ddlSelectedRooms").append(ss);
            SelectedRooms.push(obj.value);
        });
        var SelectedRoomID = 0
        $.ajax({
            type: "POST",
            url: url_AddRemoveUserRoomsToSession,
            data: JSON.stringify({
                RoomIDs: SelectedRooms.join(sep1)
            }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (message) {
                if (SelectedRooms.toString() != '') {
                    SelectedRoomID = SelectedRooms[0];
                }
                var SelectedRoleID = _CreateUser.GetSelectedRoleID();
                var SelecteUserID = _CreateUser.GetUserID();
                //    $("#CreateRolePermissionDIV").load(url_UserRolePermissionCreate,
                //    { RoomID: SelectedRoomID, RoleID: SelectedRoleID, UserID: SelecteUserID, UserType: $("#drpUserType").val() }, function () {
                //        hdnCurrentSelectedRoom = SelectedRoomID;
                //        SetSelectedModule_NonModulePermissions();
                //    });
                LoadPermissionDiv(SelectedRoomID, SelectedRoleID, SelecteUserID, $("#drpUserType").val());
                SetDefaultPermissionRooms();
                //_CreateUser.EnableDisableRoomsControls();
                self.SetMultySupplierByRoomID(SelectedRoomID, SelectedRoleID, SelecteUserID, $("#drpUserType").val());
            },
            error: function (response) {
                // through errror message
            }
        });

    };


    self.SetMultySupplierByRoomID = function (FirstRoomID, SelectedRoleID, SelecteUserID, UserType) {
        $("#RoomDataSupplier").html("");
        $.ajax({
            url: url_GetSupplierByRoomRole,
            type: 'POST',
            data: { Ent_Com_RoomID: FirstRoomID, RoleID: SelectedRoleID, UserID: SelecteUserID },
            success: function (response) {
                selectedEps = '';
                s = '';

                if (response != null) {
                    $.each(response.RoomSupplierList, function (i, val) {
                        if (val.IsSelected == true) {
                            s += '<option selected="selected" value="' + val.Ent_Com_Room_ID + '">' + val.SupplierName + '</option>';
                        }
                        else {
                            s += '<option value="' + val.Ent_Com_Room_ID + '">' + val.SupplierName + '</option>';
                        }
                        if (selectedEps == '') {
                            selectedEps = val.EnterPriseId_CompanyId_RoomId + "_" + val.RoomName;
                        }
                        else {
                            selectedEps += sep1 + val.EnterPriseId_CompanyId_RoomId + "_" + val.RoomName;
                        }
                    });
                }

                //hdnSelectedRoomAccessValue.val(selectedEps);  // start
                $("#RoomDataSupplier").html(s);
                $("#RoomDataSupplier").multiselect("refresh");

                selectedEps = '';
                s = '';

                $('#DivLoading').hide();
            },
            error: function (response) {
                alert(MsgResCommonError);
                $('#DivLoading').hide();
            }
        });
    }


    return self;
})(jQuery);

var ProperUsername = true;
var show = true;
var pswd = '';
var Cpswd = '';

//$("form").submit(function (e) {
//    $.validator.unobtrusive.parse("#frmUser");
//    if ($(this).valid()) {
//        rememberUDFValues($("#hdnPageName").val(), ModelUserID);
//    }
//    e.preventDefault();
//});

$(document).ready(function () {
    $('form').areYouSure();
    checkRememberUDFValues($("#hdnPageName").val(), ModelUserID);
    show = false;
    $('input#txtPassword').keyup(function () {

        pswd = $(this).val();
        if (pswd.length < 8) {
            $('#length').removeClass('valid').addClass('invalid');
        } else {
            $('#length').removeClass('invalid').addClass('valid');
        }
        if (pswd.match(/[a-z]/)) {
            $('#letter').removeClass('invalid').addClass('valid');
        } else {
            $('#letter').removeClass('valid').addClass('invalid');
        }

        //validate capital letter
        if (pswd.match(/[A-Z]/)) {
            $('#capital').removeClass('invalid').addClass('valid');
        } else {
            $('#capital').removeClass('valid').addClass('invalid');
        }
        if (pswd.match(/\W/g)) {
            $('#special').removeClass('invalid').addClass('valid');
        } else {
            $('#special').removeClass('valid').addClass('invalid');
        }

        //validate number
        if (pswd.match(/\d/)) {
            $('#number').removeClass('invalid').addClass('valid');
        } else {
            $('#number').removeClass('valid').addClass('invalid');
        }
        $('#pswd_info').show();
    }).focus(function () {
        $('#pswd_info').show();
    }).blur(function () {
        $('#pswd_info').hide();
    });
    $('input#txtConfirmPassword').keyup(function () {

        Cpswd = $(this).val();
        if (Cpswd.length < 8) {
            $('#Clength').removeClass('valid').addClass('invalid');
        } else {
            $('#Clength').removeClass('invalid').addClass('valid');
        }
        if (Cpswd.match(/[a-z]/)) {
            $('#Cletter').removeClass('invalid').addClass('valid');
        } else {
            $('#Cletter').removeClass('valid').addClass('invalid');
        }

        //validate capital letter
        if (Cpswd.match(/[A-Z]/)) {
            $('#Ccapital').removeClass('invalid').addClass('valid');
        } else {
            $('#Ccapital').removeClass('valid').addClass('invalid');
        }
        if (Cpswd.match(/\W/g)) {
            $('#Cspecial').removeClass('invalid').addClass('valid');
        } else {
            $('#Cspecial').removeClass('valid').addClass('invalid');
        }

        //validate number
        if (Cpswd.match(/\d/)) {
            $('#Cnumber').removeClass('invalid').addClass('valid');
        } else {
            $('#Cnumber').removeClass('valid').addClass('invalid');
        }
        $('#Cpswd_info').show();
    }).focus(function () {
        $('#Cpswd_info').show();
    }).blur(function () {
        $('#Cpswd_info').hide();
    });

});


$("input#txtUserName").focusout(function () {
    return ValidateUserName();
});

function ValidateUserName() {
    var currentUser = $.trim($("input#txtUserName").val());
    var UserId = $("input#hiddenID").val();
    //_Common.showHideLoader(true);
    $.ajax({
        url: "/master/ValidateUserName",
        type: "POST",
        data: { "UserName": currentUser, "UserID": UserId },
        success: function (res) {
            if (res != "ok") {
                if (res == "duplicate") {
                    $("span.DuplicateUserName").html(MsgUserNameAlreadyExist.replace("{0}", currentUser));
                    $("span.DuplicateUserName").show();
                    $("input#txtUserName").val('');
                    ProperUsername = false;
                    return false;
                }
                else {
                    $("span.DuplicateUserName").html(res);
                    $("span.DuplicateUserName").show();
                    $("input#txtUserName").val('');
                    ProperUsername = false;
                    return false;
                }
            }
            else {
                $("span.DuplicateUserName").hide();
                $("span.DuplicateUserName").html('');
                ProperUsername = true;
                return true;
            }
            //_Common.showHideLoader(false);
        },
        error: function (xhr) {
            console.log(xhr.status);
            //_Common.showHideLoader(false);
        },
        complete: function () {
            
        }
    });
}
$("input#txtPassword").focus(function (e) {
    $('#pswd_info').show();
});
$("input#txtPassword").focusout(function (e) {
    $('#pswd_info').hide();
});

$("input#txtPassword").click(function (e) {
    $('#pswd_info').show();
});
//confirmpassword
$("input#txtConfirmPassword").focus(function (e) {
    $('#Cpswd_info').show();
});
$("input#txtConfirmPassword").focusout(function (e) {
    $('#Cpswd_info').hide();
});

$("input#txtConfirmPassword").click(function (e) {
    $('#Cpswd_info').show();
});


function ShowHideEturnsAdmin() {
    if ($('#drpUserType').val() == '1') {
        $('#liIseTurnsAdmin').show();
    }
    else {
        $('#liIseTurnsAdmin').hide();
    }
}

var hdnSelectedRoomReplanishmentValue = $("#hdnSelectedRoomReplanishmentValue");
var hdnSelectedRoomAccessValue = $("#hdnSelectedRoomAccessValue");
var hdnRoleID = $("#hdnRoleID");

var SelectedModuleList = new Array();
var SelectedNonModuleList = new Array();
var SelectedDefaultSettings = new Array();
var SelectedRooms = new Array();

var hdnSelectedModuleList = $("#hdnSelectedModuleList");
var hdnSelectedNonModuleList = $("#hdnSelectedNonModuleList");
var hdnSelectedDefaultSettings = $("#hdnSelectedDefaultSettings");

var ddlDefaultPermissionRooms = $("#ddlDefaultPermissionRooms");
var hdnCurrentSelectedRoom = 0;


//--------- ----------
$("form").submit(function (e) {
    if (RoomValidation() == false) {
        //$('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        showNotificationDialog();
        $("#spanGlobalMessage").html(MsgReqRoomAccess);
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
        return false;
    }

    $.validator.unobtrusive.parse("#frmUser");
    if ($(this).valid()) {
        rememberUDFValues($("#hdnPageName").val(), ModelUserID);
    }

    //-----Set control when direct click without room selection ------\
    var SelectedRoomReplanishment = '';
    $("#RoomReplanishment > option").each(function () {
        if (this.selected == true) {
            if (SelectedRoomReplanishment == '')
                SelectedRoomReplanishment = this.value;
            else
                SelectedRoomReplanishment += sep1 + this.value;
        }
    });
    $("#hdnSelectedRoomReplanishmentValue").val('');
    $("#hdnSelectedRoomReplanishmentValue").val(SelectedRoomReplanishment);

    // un-tick disabled checkboxes for modules
    $("input:checkbox:disabled[IsModuleDisable=true]").prop('checked', false);

    _CreateUser.SetSelectedModule_NonModulePermissions();

    var SelectedRoomSupplier = GetSelectedRoomSupplier();

    if ($.trim(hdnSelectedModuleList.val()) != "" || $.trim(hdnSelectedNonModuleList.val()) != "" || $.trim(hdnSelectedDefaultSettings.val()) != "" || $.trim(SelectedRoomSupplier) != "") {
        
        //data: "{'RoomID': '" + hdnCurrentSelectedRoom + "' ,'RoleID':'" + hdnRoleID.val() + "','SelectedModuleList':'" + hdnSelectedModuleList.val() + "','SelectedNonModuleList':'" + hdnSelectedNonModuleList.val() + "','SelectedDefaultSettings':'" + hdnSelectedDefaultSettings.val() + "','SelectedRoomSupplier':'" + SelectedRoomSupplier + "'}",
        _CreateUser.SaveToUserPermissionsToSession(hdnCurrentSelectedRoom, hdnRoleID.val(), SelectedRoomSupplier, null, null);
    }
    else {
        alert(MsgRoomWithoutPermission);
        return false;
    }
    //------------

    //$.validator.unobtrusive.parse("#frmUser");
    //if ($(this).valid()) {
    //}

    
    //$("#txtPassword").val(hex_sha1($.trim($("#txtPassword").val())));
    //$("#txtConfirmPassword").val(hex_sha1($.trim($("#txtPassword").val())));
    e.preventDefault();
    
    $('#NarroSearchClear').click();
    
});


function onSuccess(response) {
    IsRefreshGrid = true;
    //$('div#target').fadeToggle();
    //$("div#target").delay(2000).fadeOut(200);
    showNotificationDialog();
    $("#spanGlobalMessage").html(response.Message);
    $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
    var idValue = $("#hiddenID").val();

    if (response.Status == "fail") {
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
        clearControls('frmUser');
        _CreateUser.ClearAllRolesDropdown();

        $('input:checkbox').removeAttr('checked');
        // $("#txtDescription").val("");

        $("#txtUserName").val("");
        $("#txtUserName").focus();
    }
    else if(response.Status == 'strongpassword')
    {
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
        clearControls('frmUser');
        _CreateUser.ClearAllRolesDropdown();

        $('input:checkbox').removeAttr('checked');
        // $("#txtDescription").val("");

        $("#txtUserName").val("");
        $("#txtUserName").focus();
    }
    else if (idValue == 0) {
        //clearControls('frmRole');
        $("#txtUserName").val("");
        $("#txtUserName").focus();
        if (response.Status == "duplicate")
            $("#spanGlobalMessage").removeClass('errorIcon succesIcon').addClass('WarningIcon');
        else {
            
            clearControls('frmUser');
            _CreateUser.ClearAllRolesDropdown();            
            CallNarrowfunctions();

            if (oTable !== undefined && oTable != null) {
                oTable.fnDraw();
            }
            $('#DivLoading').hide();
            $("#tab5").click();
        }
    }
    else if (idValue > 0) {

        if (response.Status == "duplicate") {
            $("#spanGlobalMessage").removeClass('errorIcon succesIcon').addClass('WarningIcon');
            $("#txtUserName").val("");
            $("#txtUserName").focus();
        }
        else {
            
            clearControls('frmUser');
            _CreateUser.ClearAllRolesDropdown();           
            CallNarrowfunctions();
            SwitchTextTab(0, 'UserCreate', 'frmUser');
        }
    }
}

function Login_OnBegin() {
    //var str = $("#txtPassword").val();
    //var re = /(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%]).{8,20}/;
    //var found = str.match(re);


    //if (found == null) {
    //    $('div#target').fadeToggle();
    //    $("div#target").delay(2000).fadeOut(200);
    //    $("#spanGlobalMessage").html('@ResUserMaster.errPasswordRuleBreak');
    //    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('succesIcon');
    //    return false;
    //}



}
function onFailure(message) {
    $("#spanGlobalMessage").html(message.statusText);
    //$('div#target').fadeToggle();
    //$("div#target").delay(2000).fadeOut(200);
    showNotificationDialog();
    $("#txtUserName").focus();
}

$(document).ready(function () {
    $("#ddlPermissionTemplateName").change(function () {

        var epid, compid, roomid;
        epid = $("#ddlSelectedRooms").val().split("_")[0];
        compid = $("#ddlSelectedRooms").val().split("_")[1];
        roomid = $("#ddlSelectedRooms").val().split("_")[2];

        TemplateSelection(epid, compid, roomid, $("#ddlRole").val(), $("#hiddenID").val(), $(this).val());
    });
    //    jQuery.validator.addMethod('passwordrequired', function (value, element, params) {
    //        if (ModelUserID < 1) {
    //            return false;
    //        }
    //        else {
    //            return true;
    //        }
    //    }, '');
    //    jQuery.validator.unobtrusive.adapters.add('passwordrequired', {}, function (options) {

    //        options.rules['passwordrequired'] = true;
    //        options.messages['passwordrequired'] = options.message;

    //    });
    SetAccessLevels($("#drpUserType").val());
    ShowHideEturnsAdmin();
    if ($("#hdnDisableControl").val() != '') {
        if ($("#hdnDisableControl").val().toLowerCase() == "true") {
            $(':input', '#frmUser')
                            .not('#btnCancel') 
                            .not('#ddlSelectedRooms') //Wi- 1182 room dropdown can not be change if see details for same login user so enable it as per discuss with rock
                            .attr('disabled', 'disabled');
        }


    }
    if ($("#hdnSelectedModuleList").val() != '') {
        SelectedModuleList = $("#hdnSelectedModuleList").val().split(',');
    }

    if ($("#hdnSelectedNonModuleList").val() != '') {
        SelectedNonModuleList = $("#hdnSelectedNonModuleList").val().split(',');
    }
    if ($("#hdnSelectedDefaultSettings").val() != '') {
        SelectedDefaultSettings = $("#hdnSelectedDefaultSettings").val().split(',');
    }

    $("#drpUserType").change(function () {
        ShowHideEturnsAdmin();
        SetAccessLevels($(this).val());
    });
    $('#btnCancel').click(function (e) {
        //            if (IsRefreshGrid)
        $('#NarroSearchClear').click();
        SwitchTextTab(0, 'UserCreate', 'frmUser');
        if (oTable !== undefined && oTable != null) {
            oTable.fnDraw();
        }
    });


    $('#ddlRole').change(function (e) {
        _CreateUser.RoleSelection();
    });


    $('#ddlSelectedRooms').change(function (e) {
        RoomChanged();
    });

    //$("#ddlDefaultPermissionRooms").multiselect(
    //                            {
    //                                noneSelectedText: 'Room  ', selectedList: 5,
    //                                selectedText: function (numChecked, numTotal, checkedItems) { return 'Room : ' + numChecked + ' selected'; }
    //                            }).bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
    //                                $.map($(this).multiselect("getChecked"), function (input) { return input.value; })
    //                            });

    _CreateUser.setCopyToRoomsMultiSelect();

    _CreateUser.EnableDisableRoomsControls();

});

$(function () {
    $("#tabs").tabs();
});
//-----------------------------------------------

function GetSelectedRoomAccessID() {
    var SelectedRoomID = 0; // $("#ddlSelectedRooms").val();

    $("#ddlSelectedRooms > option").each(function () {
        if (this.selected == true) {
            SelectedRoomID = this.value;
        }
    });
    return SelectedRoomID;
}

function GetSelectedRoomSupplier() {  
    var SelectedRoomSupplierID = '';
    var arrItems1 = new Array();
    $("#RoomDataSupplier > option").each(function () {
        if (this.selected == true)
            arrItems1.push(this.value);
    });
     
    if (arrItems1.length > 0) {
        SelectedRoomSupplierID = arrItems1;
        SelectedRoomSupplierID = SelectedRoomSupplierID.join(sep1)
    }
     
    return SelectedRoomSupplierID;
}








function RoomChanged() {
    var SelectedRoomID = GetSelectedRoomAccessID();
    var SelectedRoleID = _CreateUser.GetSelectedRoleID();
    var SelectedRoomSupplier = GetSelectedRoomSupplier();
    SetDefaultPermissionRooms();

    _CreateUser.SetSelectedModule_NonModulePermissions();
    _CreateUser.SaveToUserPermissionsToSession(hdnCurrentSelectedRoom, SelectedRoleID, SelectedRoomSupplier);
    
    ClearRoomAccessHidden();
    var SelecteUserID = _CreateUser.GetUserID();
    //    $("#CreateRolePermissionDIV").load(url_UserRolePermissionCreate,
    //    { RoomID: SelectedRoomID, RoleID: SelectedRoleID, UserID: SelecteUserID, UserType: $("#drpUserType").val() }, function () {
    //        hdnCurrentSelectedRoom = SelectedRoomID;
    //        SetSelectedModule_NonModulePermissions();
    //    });
    LoadPermissionDiv(SelectedRoomID, SelectedRoleID, SelecteUserID, $("#drpUserType").val());
    //_CreateUser.EnableDisableRoomsControls();
    _CreateUser.SetMultySupplierByRoomID(SelectedRoomID, SelectedRoleID, SelecteUserID, $("#drpUserType").val());
}



function ClearRoomAccessHidden() {
    hdnSelectedModuleList.val('');
    hdnSelectedNonModuleList.val('');
    SelectedModuleList = [];
    SelectedNonModuleList = [];
}

/* created for set company dropdown on room selection */
function resetCompanyonRoomSelection() {
    var SelectedCompanyId = [];
    var checkedList = $("#RoomData").multiselect("getChecked");
    $(checkedList).each(function (indx, obj) {
        SelectedCompanyId.push($(this).attr("value").split("_")[0] + "_" + $(this).attr("value").split("_")[1]);
    });

    var arrItems = new Array();
    $("#CompanyData").multiselect("widget").find(":checkbox").each(function () {
        var ccid = $(this).attr("value").split("_")[0] + "_" + $(this).attr("value").split("_")[1];
        if ($.inArray(ccid, SelectedCompanyId) > -1) {
            $(this).attr("checked", "checked");
            arrItems.push(this.value);
        }
        else {
            $(this).removeAttr("checked");
        }
    });

    $("#hdnSelectedCompanyAccessValue").val(arrItems.join(sep1));
}




//function Checkclick(chkDropdown, event) {
//    _CreateUser.SetSelectBox();
//    //resetCompanyonRoomSelection();
//}

function CheckRoomsSupplierClick(chkDropdown, event) {
    var arrItems = new Array();
    //if (event.type == 'multiselectcheckall') {
    $("#RoomDataSupplier > option").each(function () {
        if (this.selected == true)
            arrItems.push(this.value);
    });

    if (chkDropdown.checked == true) {
        arrItems.push(chkDropdown.value);
    }
    else {
        var selected = chkDropdown.value;
        arrItems = jQuery.grep(arrItems, function (value) {
            return value != selected;
        });
    }
    if (chkDropdown.selected == true) {
        arrItems.push(chkDropdown.value);
    }
    SelectedRoomSupplier = arrItems;
    ////// SelectedCompanies = is now SelectedRoomSupplier
    $("#hdnSelectedRoomDataSupplier").val(SelectedRoomSupplier.join(sep1));
     

            if (event.type == 'multiselectcheckall' || event.type == 'multiselectuncheckall') {
                if (event.type == 'multiselectcheckall') {
                    $("#RoomDataSupplier").multiselect("widget").find(":checkbox").each(function () {
                        $(this).attr("checked", "checked");
                    });
                    $("#RoomDataSupplier option").each(function () {
                        $(this).attr("selected", 1);
                    });
                }
                else {
                    $("#RoomDataSupplier").multiselect("widget").find(":checkbox").each(function () {
                        $(this).removeAttr("checked");
                    });
                    $("#RoomDataSupplier option").each(function () {
                        $(this).removeAttr("selected");
                    });
                }
            }
//    //    else {
//    //        var valuesToselect = chkDropdown.value;
//    //        if (chkDropdown.checked) {
//    //            $("#RoomData").multiselect("widget").find(":checkbox").each(function () {

//    //                var cid = $(this).attr("value").split("_")[0] + "_" + $(this).attr("value").split("_")[1];
//    //                if (cid == valuesToselect) {
//    //                    $(this).attr("checked", "checked");
//    //                }

//    //            });
//    //            $("#RoomData option").each(function () {

//    //                var cid = $(this).attr("value").split("_")[0] + "_" + $(this).attr("value").split("_")[1];
//    //                if (cid == valuesToselect) {
//    //                    $(this).attr("selected", 1);
//    //                }
//    //            });
//    //        }
//    //        else {

//    //            $("#RoomData").multiselect("widget").find(":checkbox").each(function () {

//    //                var cid = $(this).attr("value").split("_")[0] + "_" + $(this).attr("value").split("_")[1];
//    //                if (cid == valuesToselect) {
//    //                    $(this).removeAttr("checked");
//    //                }

//    //            });
//    //            $("#RoomData option").each(function () {

//    //                var cid = $(this).attr("value").split("_")[0] + "_" + $(this).attr("value").split("_")[1];
//    //                if (cid == valuesToselect) {
//    //                    $(this).removeAttr("selected");
//    //                }
//    //            });
//    //        }
//    //    }
//    //    SetSelectBox();

}
 
function EditModeSetRoomReplanishmentData() {
    $("#RoomReplanishment > option").each(function () {
        this.selected = false;
    });
    $("#RoomReplanishment").multiselect("refresh");

    if ($("#hdnSelectedRoomReplanishmentValue").val() != '') {
        var Rnreplenish = $("#hdnSelectedRoomReplanishmentValue").val().split(sep1);
        for (var i = 0; i < Rnreplenish.length; i++) {
            $("#RoomReplanishment").multiselect("widget").find(":checkbox[value='" + Rnreplenish[i] + "']").attr("checked", "checked");
            $("#RoomReplanishment option[value='" + Rnreplenish[i] + "']").attr("selected", 1);
            $("#RoomReplanishment").multiselect("refresh");
        }
    }

}
function nth_occurrence(string, char, nth) {
    var first_index = string.indexOf(char);
    var length_up_to_first_index = first_index + 1;

    if (nth == 1) {
        return first_index;
    } else {
        var string_after_first_occurrence = string.slice(length_up_to_first_index);
        var next_occurrence = nth_occurrence(string_after_first_occurrence, char, nth - 1);

        if (next_occurrence === -1) {
            return -1;
        } else {
            return length_up_to_first_index + next_occurrence;
        }
    }
}
function ResetRoomAccessSelection() {
    var checkedList1 = $("#RoomData").multiselect("getChecked");
    
    $("#ddlSelectedRooms > option").remove();
    if (checkedList1.length > 0) {

        var SelectedRoleID = _CreateUser.GetSelectedRoleID();
        var SelecteUserID = _CreateUser.GetUserID();
        var FirstRoomID = 0;
        var Rn = hdnSelectedRoomAccessValue.val().split(sep1);
        for (var i = 0; i < checkedList1.length; i++) {
            //            checkedList1[0].title
            //            checkedList1[0].value
            //            var Rnames = Rn[i].split("_");
            if (i == 0) {
                FirstRoomID = checkedList1[i].value;
            }
            //            var optionname = "";
            //            var thirducindx = nth_occurrence(Rn[i], '_', 3);
            //            if (thirducindx != -1) {
            //                optionname = Rn[i].substring((thirducindx + 1), Rn[i].length);
            //            }
            var ss = '<option value="' + checkedList1[i].value + '">' + checkedList1[i].title + '</option>';
            $("#ddlSelectedRooms").append(ss);
            //            $("#RoomData").multiselect("widget").find(":checkbox[value='" + Rnames[0] + "_" + Rnames[1] + "_" + Rnames[2] + "']").attr("checked", "checked");
            //            $("#RoomData option[value='" + Rnames[0] + "_" + Rnames[1] + "_" + Rnames[2] + "']").attr("selected", 1);
            //            $("#RoomData").multiselect("refresh");
        }

        Rn = $("#hdnSelectedEnterpriseAccessValue").val().split(sep1);
        //        for (var i = 0; i < Rn.length; i++) {
        //            var Rnames = Rn[i].split("_");
        //            $("#EnterpriseData").multiselect("widget").find(":checkbox[value='" + Rnames[0] + "']").attr("checked", "checked");
        //            $("#EnterpriseData option[value='" + Rnames[0] + "']").attr("selected", 1);
        //            $("#EnterpriseData").multiselect("refresh");
        //        }
        Rn = $("#hdnSelectedCompanyAccessValue").val().split(sep1);
        //        for (var i = 0; i < Rn.length; i++) {
        //            var Rnames = Rn[i].split("_");
        //            $("#CompanyData").multiselect("widget").find(":checkbox[value='" + Rnames[0] + "_" + Rnames[1] + "']").attr("checked", "checked");
        //            $("#CompanyData option[value='" + Rnames[0] + "_" + Rnames[1] + "']").attr("selected", 1);
        //            $("#CompanyData").multiselect("refresh");
        //        }

        Rn = $("#hdnSelectedRoomReplanishmentValue").val().split(sep1);
        //        for (var i = 0; i < Rn.length; i++) {
        //            var Rnames = Rn[i].split("_");
        //            $("#RoomReplanishment").multiselect("widget").find(":checkbox[value='" + Rnames[0] + "_" + Rnames[1] + "_" + Rnames[2] + "']").attr("checked", "checked");
        //            $("#RoomReplanishment option[value='" + Rnames[0] + "_" + Rnames[1] + "_" + Rnames[2] + "']").attr("selected", 1);
        //            $("#RoomReplanishment").multiselect("refresh");
        //        }
        if (FirstRoomID != "") {
            $("#ddlSelectedRooms").multiselect("widget").find(":checkbox[value='" + FirstRoomID + "']").attr("checked", "checked");
            $("#ddlSelectedRooms option[value='" + FirstRoomID + "']").attr("selected", 1);
            $("#ddlSelectedRooms").multiselect("refresh");

            SetDefaultPermissionRooms();
            LoadPermissionDiv(FirstRoomID, SelectedRoleID, SelecteUserID, $("#drpUserType").val());
            _CreateUser.SetMultySupplierByRoomID(FirstRoomID, SelectedRoleID, SelecteUserID, $("#drpUserType").val());

        }
    }
}

function LoadPermissionDiv(FirstRoomID, SelectedRoleID, SelecteUserID, UserType) {
    $("#CreateRolePermissionDIV").load(url_UserRolePermissionCreate, { RoomID: FirstRoomID, RoleID: SelectedRoleID, UserID: SelecteUserID, UserType: UserType }, function () {
        if (UserType == 1) {
            $("tr[id='41']").show();
            $("tr[id='39']").show();
        }
        else if (UserType == 2) {
            $("tr[id='41']").hide();
            $("tr[id='39']").show();
        }
        else {
            $("tr[id='41']").hide();
            $("tr[id='39']").hide();
        }
        hdnCurrentSelectedRoom = FirstRoomID;
        _CreateUser.SetSelectedModule_NonModulePermissions();
        _CreateUser.EnableDisableRoomsControls();
    });
}
function EditModeSetData() {

    if ($("#hdnRoleID").val() != '') {
        $("#ddlRole > option").each(function () {
            if (this.value == $("#hdnRoleID").val()) {
                this.selected = true;
            }
        });
    }
    //var SelectedRoleID = GetSelectedRoleID();
    _CreateUser.RoleSelection();

    //        ResetRoomAccessSelection();
    //        EnableDisableRoomsControls();
}

function SetDefaultPermissionRooms() {
    $("#ddlDefaultPermissionRooms > option").remove();

    $("#ddlSelectedRooms > option").each(function () {
        if (this.selected == false) {
            var ss = '<option value="' + this.value + '">' + this.text + '</option>';
            ddlDefaultPermissionRooms.append(ss);
        }
    });
    ddlDefaultPermissionRooms.multiselect("refresh");
}

function CopyPermission() {
    var SelectedRoomID = GetSelectedRoomAccessID();
    var SelectedRoomSupplier = ''; // GetSelectedRoomSupplier();

    var CopyToRoomIDs = '';
    $("#ddlDefaultPermissionRooms > option").each(function () {
        if (this.selected == true) {
            if (CopyToRoomIDs == '')
                CopyToRoomIDs = this.value;
            else
                CopyToRoomIDs += sep1 + this.value;
        }
    });

    _CreateUser.SetSelectedModule_NonModulePermissions();

    if (CopyToRoomIDs != '') {
        $.ajax({
            type: "POST",
            url: url_SaveAndCopyPermissionsToSession,
            data: JSON.stringify({
                RoomID: hdnCurrentSelectedRoom,
                RoleID: hdnRoleID.val(),
                SelectedModuleList: hdnSelectedModuleList.val(),
                SelectedNonModuleList: hdnSelectedNonModuleList.val(),
                SelectedDefaultSettings: hdnSelectedDefaultSettings.val(),
                SelectedRoomSupplier: SelectedRoomSupplier,
                CopyToRoomIDs: CopyToRoomIDs,
                ParentRoomID: SelectedRoomID
            }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (message) {
                showNotificationDialog();
                $("#spanGlobalMessage").html(MsgPermissionCopied);
                $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('succesIcon');
            },
            error: function (response) {
                // error handling
            }
        });
    }
    else {
        //$('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        showNotificationDialog();
        $("#spanGlobalMessage").html(MsgRoomToCopyPermission);
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('WarningIcon');
        return false;
    }
}

function RoomValidation() {
    var RoomAvailable = false;
    var SelectedRooms = '';
    $("#ddlSelectedRooms > option").each(function () {
        if (SelectedRooms == '') {
            SelectedRooms = this.value + "_" + this.text;
        }
        else {
            SelectedRooms += sep1 + this.value + "_" + this.text;
        }
        RoomAvailable = true;
    });

    hdnSelectedRoomAccessValue.val(SelectedRooms);
    
    var arrItems = new Array();
    var checkedList = $("#CompanyData").multiselect("getChecked");
    $(checkedList).each(function (indx, obj) {
        arrItems.push(obj.value);
    });
    $("#hdnSelectedCompanyAccessValue").val(arrItems.join(sep1));


    return RoomAvailable;
}


function ValidateCheckBox(Chked) {
    var result = true;
    if (Chked.toLowerCase().indexOf("rowall") != -1) {
        result = false;
    }
    else if (Chked.toLowerCase().indexOf("allview") != -1) {
        result = false;
    }
    else if (Chked.toLowerCase().indexOf("allinsert") != -1) {
        result = false;
    }
    else if (Chked.toLowerCase().indexOf("alldelete") != -1) {
        result = false;
    }
    else if (Chked.toLowerCase().indexOf("allupdate") != -1) {
        result = false;
    }
    else if (Chked.toLowerCase().indexOf("allshowdeleted") != -1) {
        result = false;
    }
    else if (Chked.toLowerCase().indexOf("allshowarchived") != -1) {
        result = false;
    }
    else if (Chked.toLowerCase().indexOf("allshowudf") != -1) {
        result = false;
    }
    else if (Chked.toLowerCase().indexOf("allshowchangelog") != -1) {
        result = false;
    }
    return result;

}
function SetAccessLevels(userType) {

    if (userType == "1") {
        $("#liEnterprises").css("display", "");
        $("#liCompanies").css("display", "");
        $("tr[id='41']").show();
        $("tr[id='39']").show();
        LoadEnterprises();
    }
    if (userType == "2") {
        $("#liEnterprises").css("display", "none");
        $("#liCompanies").css("display", "");
        $("tr[id='41']").hide();
        $("tr[id='39']").show();
        var arrnewEnterprise = new Array();
        arrnewEnterprise.push(UserEnterpriseID);
        LoadCompanies(arrnewEnterprise);
    }
    if (userType == "3") {
        $("#liEnterprises").css("display", "none");
        $("#liCompanies").css("display", "none");
        $("tr[id='41']").hide();
        $("tr[id='39']").hide();

        var arrnewCompany = new Array();
        arrnewCompany.push(EnterPriceID_CompanyId);
        LoadRooms(arrnewCompany);
        LoadRoomsSupplier(arrnewCompany);
        LoadReplenishRoomSelectBox();
    }
    LoadRolesByUserType(userType);
}
function LoadEnterprises() {
    $("#EnterpriseData").multiselect({
        noneSelectedText: 'Enterprise Access', selectedList: 5,
        selectedText: function (numChecked, numTotal, checkedItems) {
            return 'Enterprise Access';
        }
    }).bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
        _CreateUser.CheckEnterpriseClick(ui, event);
    }).multiselectfilter();
    LoadCompanies("")
}

function LoadRolesByUserType(userType) {
    $("#ddlRole > option").remove();

    $.ajax({
        url: url_getRoleListByUserType,
        type: "POST",
        data: { UserType: userType, CompanyId: UserCompanyId, EnterpriseId: UserEnterpriseId }, 
        success: function (response) {
            var s = '';
            $.each(response, function (i, val) {
                s += '<option value="' + val.ID + '"  >' + val.RoleName + '</option>';
            });
            $("#ddlRole").append(s);
            EditModeSetData();
        },
        error: function (response) {
            // through errror message
        }
    });
}

function LoadCompanies(enterpriseids) {

    //var styrdata = JSON.stringify(enterpriseids);
    //$("#CompanyData").multiselect(
    //                            {
    //                                noneSelectedText: 'Company Access', selectedList: 5,
    //                                selectedText: function (numChecked, numTotal, checkedItems) {
    //                                    return 'Company Access';
    //                                }
    //                            }

    //                ).bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
    //                    CheckCompanyClick(ui, event);
    //                }).multiselectfilter();
    _CreateUser.setCompanyDdlMultiSelect();
    LoadRooms("");
    LoadRoomsSupplier("");
    LoadReplenishRoomSelectBox();
}

function LoadRooms(companyids) {
    //alert('in load rooms ');
    //$("#RoomData").multiselect(
    //                            {
    //                                noneSelectedText: 'Room Access', selectedList: 5,
    //                                selectedText: function (numChecked, numTotal, checkedItems) {
    //                                    return 'Room Access';
    //                                }
    //                            }

    //                ).bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
    //                    Checkclick(ui, event);
    //                }).multiselectfilter();
    _CreateUser.setRoomDdlMultiSelect();

}

function LoadRoomsSupplier(companyids) {
    //alert('in room Supplier ');
    $("#RoomDataSupplier").multiselect(
                                {
                                    noneSelectedText: 'Supplier Access', selectedList: 5,
                                    selectedText: function (numChecked, numTotal, checkedItems) {
                                        return 'Supplier Access';
                                    }
                                }

                    ).bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                        CheckRoomsSupplierClick(ui, event); 
                    }).multiselectfilter();

                }

function LoadReplenishRoomSelectBox() {
    $("#RoomReplanishment").multiselect(
                        {
                            noneSelectedText: 'Room Replenishment', selectedList: 5,
                            selectedText: function (numChecked, numTotal, checkedItems) {
                                return 'Room Access';
                            }
                        }

            ).bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {

            }).multiselectfilter();
}
function FillSuppliers() {
    $.ajax({
        type: "POST",
        url: '@Url.Action("GetSuppliersByRoom", "Master")',
        contentType: 'application/json',
        dataType: 'json',
        data: "{RoomId:'" + parseInt('@Model.RoomID') + "' , CompanyId: '" + parseInt('@Model.CompanyId') + "'}",
        success: function (retdt) {
            $("select[id='99']").html("");
            $(retdt).each(function (index, obj) {
                $("select[id='99']").append("<option value='" + obj.ID + "'>" + obj.SupplierName + "</option>");
            });
        },
        error: function (err) {
            alert(MsgErrorInProcess);
        }
    });
}

function numberOnly() {
    if ((event.keyCode < 48) || (event.keyCode > 57))
        return false;
}

function SelectAllTabChk(Chked, TabNO) {
    if (TabNO == '1') {
        $("#Roomtab2 input:checkbox:enabled").each(function () {
            this.checked = Chked.checked;
        });
    }
    else if (TabNO == '2') {
        $("#Roomtab3 input:checkbox:enabled").each(function () {
            this.checked = Chked.checked;
        });
    }
    else if (TabNO == '3') {
        $("#Roomtab4 input:checkbox:enabled").each(function () {
            var vModuleID = $(this).prop("id").split('_')[0]
            if ((vModuleID != "116" && vModuleID != "118" && vModuleID != "121") || Chked.checked == false)
                this.checked = Chked.checked;
            else
                this.checked = false;
        });
    }
}
function SelectRowColumnChk(Chked, TabNO) {

    if (Chked.id.toLowerCase().indexOf("rowall") != -1) {
        var chkModuleID = Chked.id.split("_");
        var Tabs = '';
        if (TabNO == '1') {
            var Tabs = $("#Roomtab2 input:checkbox");
        }
        else if (TabNO == '2') {
            var Tabs = $("#Roomtab3 input:checkbox");
        }
        Tabs.each(function () {
            var currentModuleID = this.id.split("_");
            if (currentModuleID[0] == chkModuleID[0]) {
                this.checked = Chked.checked;
            }
        });
    }

    var fnTickColumnCheckbox = function (permission) {
        var Tabs = '';
        if (TabNO == '1') {
            var Tabs = $("#Roomtab2 input:checkbox:enabled");
        }
        else if (TabNO == '2') {
            var Tabs = $("#Roomtab3 input:checkbox:enabled");
        }

        Tabs.each(function () {
            var currentModuleID = this.id.split("_");
            if (currentModuleID.length > 1) {
                if (currentModuleID[1].toLowerCase() == permission) {
                    this.checked = Chked.checked;
                }
            }
        });
    }

    // ----- Select All View permission --------
    if (Chked.id.toLowerCase().indexOf("allview") != -1) {
        //CheckViewChkBox(Chked, TabNO);
        fnTickColumnCheckbox('view');
    }

    // ----- Select All Insert permission --------
    else if (Chked.id.toLowerCase().indexOf("allinsert") != -1) {
        fnTickColumnCheckbox('insert');
        
    }

    // ----- Select All Delete permission --------
    else if (Chked.id.toLowerCase().indexOf("alldelete") != -1) {
        fnTickColumnCheckbox('delete');
    }
    // ----- Select All Update permission --------

    else if (Chked.id.toLowerCase().indexOf("allupdate") != -1) {
        fnTickColumnCheckbox('update');
    }
    else if (Chked.id.toLowerCase().indexOf("allshowdeleted") != -1) {
        fnTickColumnCheckbox('showdeleted');
    }
    else if (Chked.id.toLowerCase().indexOf("allshowarchived") != -1) {
        fnTickColumnCheckbox('showarchived');
    }
    else if (Chked.id.toLowerCase().indexOf("allshowudf") != -1) {
        fnTickColumnCheckbox('showudf');
    }
    else if (Chked.id.toLowerCase().indexOf("allshowchangelog") != -1) {
        fnTickColumnCheckbox('showchangelog');
    }

    // ----- Select All Show Deleted,Archived and UDF permission --------
    //SelectRowColumnChkAll(Chked, TabNO, "allshowdeleted", "showdeleted");
    //SelectRowColumnChkAll(Chked, TabNO, "allshowarchived", "showarchived");
    //SelectRowColumnChkAll(Chked, TabNO, "allshowudf", "showudf");
    //SelectRowColumnChkAll(Chked, TabNO, "allshowchangelog", "showchangelog");
    // ----- Select All Show Deleted,Archived and UDF permission --------

}


//function SelectRowColumnChkAll(Chked, TabNO, Allchkname, chkname) {
//    if (Chked.id.toLowerCase().indexOf(Allchkname) != -1) {
//        var Tabs = '';
//        if (TabNO == '1') {
//            var Tabs = $("#Roomtab2 input:checkbox");
//        }
//        else if (TabNO == '2') {
//            var Tabs = $("#Roomtab3 input:checkbox");
//        }

//        Tabs.each(function () {
//            var currentModuleID = this.id.split("_");
//            if (currentModuleID.length > 1) {
//                if (currentModuleID[1].toLowerCase() == chkname) {
//                    this.checked = Chked.checked;
//                }
//            }
//        });
//    }
//}

//function CheckViewChkBox(Chked, TabNO) {
//    //  alert(TabNO);
//    var Tabs = '';
//    if (TabNO == '1') {
//        var Tabs = $("#Roomtab2 input:checkbox");
//    }
//    else if (TabNO == '2') {
//        var Tabs = $("#Roomtab3 input:checkbox");
//    }

//    Tabs.each(function () {
//        var currentModuleID = this.id.split("_");
//        if (currentModuleID.length > 1) {
//            if (currentModuleID[1].toLowerCase() == 'view') {
//                this.checked = Chked.checked;
//            }
//        }
//    });
//}

function SelectViewPermission(Chked, TabNO) {
    var SelectedModuleID = Chked.id.split("_");

    var hasView = false;
    var hasDelete = false;
    var hasUpdate = false;
    
    var hasInsert = false;
    var hasShowUDF = false;
    var hasShowChangeLog = false;
    var Viewbtn = null;
    var hasShowDeleted = false;

    var hasShowArchived = false;


    var Tabs = '';
    if (TabNO == '1') {
        var Tabs = $("#Roomtab2 input:checkbox");
    }
    else if (TabNO == '2') {
        var Tabs = $("#Roomtab3 input:checkbox");
    }

    Tabs.each(function () {
        var currentModuleID = this.id.split("_");
        if (currentModuleID.length > 1) {
            if (currentModuleID[0] == SelectedModuleID[0]) {

                if (Chked.checked == true && currentModuleID[1].toLowerCase() == 'view') {
                    Viewbtn = this;
                    this.checked = true;
                    this.disabled = true;
                }
                else {

                    if (currentModuleID[1].toLowerCase() == 'view') {
                        Viewbtn = this;
                    }

                    if (currentModuleID[1].toLowerCase() == 'delete') {
                        if (this.checked == true) {
                            hasDelete = true;
                        }
                    }

                    if (currentModuleID[1].toLowerCase() == 'update') {
                        if (this.checked == true) {
                            hasUpdate = true;
                        }
                    }

                    if (currentModuleID[1].toLowerCase() == 'insert') {
                        if (this.checked == true) {
                            hasInsert = true;
                        }
                    }
                    if (currentModuleID[1].toLowerCase() == 'showdeleted') {
                        if (this.checked == true) {
                            hasShowDeleted = true;
                        }
                    }
                    if (currentModuleID[1].toLowerCase() == 'showarchived') {
                        if (this.checked == true) {
                            hasShowArchived = true;
                        }
                    }
                    if (currentModuleID[1].toLowerCase() == 'showudf') {
                        if (this.checked == true) {
                            hasShowUDF = true;
                        }
                    }
                    if (currentModuleID[1].toLowerCase() == 'showchangelog') {
                        if (this.checked == true) {
                            hasShowChangeLog = true;
                        }
                    }
                }
            }
        }
    });

    if (hasDelete == true || hasUpdate == true || hasInsert == true || hasShowDeleted == true || hasShowArchived == true || hasShowUDF == true || hasShowChangeLog == true) {
        Viewbtn.disabled = true;
    }
    else {
        Viewbtn.disabled = false;
        Viewbtn.checked = false;
    }
}


function TemplateSelection(EnterpriseID, CompanyID, RoomID, RoleID, UserID, templateID) {
    //alert(EnterpriseID + "______" + CompanyID + "______" + RoomID + "______" + RoleID + "______" + UserID + "______" + templateID)
    var SelectedRoomID;
    $.ajax({
        type: "POST",
        url: url_SetTemplatePermissionToUserSession,
        data: "{'EnterpriseID': '" + EnterpriseID + "' ,'CompanyID': '" + CompanyID + "' ,'RoomID': '" + RoomID + "' ,'RoleID':'" + RoleID + "','UserID':'" + UserID + "','templateID':'" + templateID + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (message) {

            SelectedRoomID = EnterpriseID + "_" + CompanyID + "_" + RoomID;
            var SelectedRoleID = _CreateUser.GetSelectedRoleID();
            var SelecteUserID = _CreateUser.GetUserID();
            LoadPermissionDiv(SelectedRoomID, SelectedRoleID, SelecteUserID, $("#drpUserType").val());
            SetDefaultPermissionRooms();
            //_CreateUser.EnableDisableRoomsControls();
        },
        error: function (response) {
            // through errror message
        }
    });

}
; (function ($) {
    $.fn.extend({
        donetyping: function (callback, timeout) {
            timeout = timeout || 1e3; // 1 second default timeout
            var timeoutReference,
                doneTyping = function (el) {
                    if (!timeoutReference) return;
                    timeoutReference = null;
                    callback.call(el);
                };
            return this.each(function (i, el) {
                var $el = $(el);
                // Chrome Fix (Use keyup over keypress to detect backspace)
                // thank you @palerdot
                $el.is(':input') && $el.on('keyup keypress paste', function (e) {
                    // This catches the backspace button in chrome, but also prevents
                    // the event from triggering too preemptively. Without this line,
                    // using tab/shift+tab will make the focused element fire the callback.
                    if (e.type == 'keyup' && e.keyCode != 8) return;

                    // Check if timeout has been set. If it has, "reset" the clock and
                    // start over again.
                    if (timeoutReference) clearTimeout(timeoutReference);
                    timeoutReference = setTimeout(function () {
                        // if we made it here, our timeout has elapsed. Fire the
                        // callback
                        doneTyping(el);
                    }, timeout);
                }).on('blur', function () {
                    // If we can, fire the event since we're leaving the field
                    doneTyping(el);
                });
            });
        }
    });
})(jQuery);
$("input#txtConfirmPassword").donetyping(function () {
    show = false;
    //setTimeout(function () { $('#Cpswd_info').hide(); }, 3000);
});
$("input#txtPassword").donetyping(function () {
    show = false;
    //setTimeout(function () { $('#pswd_info').hide(); }, 3000);
});
