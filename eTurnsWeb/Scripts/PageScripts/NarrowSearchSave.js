/*
 * Steps to setup narrow search save ,
 * 1) Set global narrow search variable value to _NarrowSearchSave.objSearch property 
 *      e.g. _NarrowSearchSave.objSearch.PullSupplier = PullSupplierNarroValues
 * 2) At time of save record - set _NarrowSearchSave.objSearch value to json object per list.
 *      Refer - getJsonToSave() . Set field , control id , value and control type in json
 * 3) When page is loaded, Set control value from saved search. 
 *      There are different ajax calls per control. 
 *      Once control values are set via ajax call , Call _NarrowSearchSave.setControlValue(ctlID)
 * 4) Set global narrow search variable value from saved search. It wil be done from _NarrowSearchSave.setControlValue.
 *      Make sure that the variable is set in setGlobalVariableVal() function
 * */
var _NarrowSearchSave = (function ($, _notify, _datePicker, _utils) {

    var self = {};

    self.listNameEnum = {
        ItemMaster: 'ItemMaster',
        ItemBinMaster: 'ItemBinMaster',
        ItemMasterPictureView: 'ItemMasterPictureView',
        NewPULL: 'NewPULL',
        PullMaster: 'PullMaster',
        ReceiveMaster: 'ReceiveMaster',
        ReceiveMasterIncomplete: 'ReceiveMasterIncomplete',
        ReceiveItemWithoutOrder: 'ReceiveItemWithoutOrder',
        OrderMaster: 'OrderMaster',
        OrderMasterUnSubmitted: 'OrderMasterUnSubmitted',
        OrderMasterApprove: 'OrderMasterApprove',
        OrderMasterChangeOrder: 'OrderMasterChangeOrder',
        OrderMasterReceive: 'OrderMasterReceive',
        RequisitionMaster: 'RequisitionMaster',
        RequisitionMasterApproved: 'RequisitionMasterApproved',
        RequisitionMasterSubmitted: 'RequisitionMasterSubmitted',
        RequisitionMasterUnsubmitted: 'RequisitionMasterUnsubmitted',
        RequisitionMasterClosed: 'RequisitionMasterClosed',
        InventoryCount: "InventoryCount",
        ToolMaster: "ToolMaster",
        ToolHistory: "ToolHistory",
        WrittenOffToolList: "WrittenOffToolList",
        AssetMaster: "AssetMaster",
        ToolMasterNew: "ToolMasterNew",
        ToolHistoryNew: "ToolHistoryNew",
        QuoteMaster:"QuoteMasterList"
    };

    self.operationEnum = {
        PageLoad: 'PageLoad',
        ReloadSearch: 'ReloadSearch',
        SearchOnTabChange: 'SearchOnTabChange'
    }

    var ctlTypeEnum = {
        GlobalSearch: 'Gbl',
        Select: 'Sel',
        MultiSelect: 'MSel',
        TextBox: 'Txt',
        FromDate: 'FDT',
        ToDate: 'TDT',
        Radio: 'Rad',
        UDF: 'UDF'
    }


    self.companyID = null;
    self.roomID = null;
    self.enterpriseID = null;
    self.currentBtnSaveID = null;
    self.currentListName = null;
    self.currentOperation = self.operationEnum.PageLoad;
    self.isPageLoading = true;
    var isControlValuesLoading = false;
    var isValidFilter = true;
    self.isApplySearchOnTabChange = false;
    ReceiveIncomplateTab = window.location.hash.toLowerCase() == "#incomplete";
    var ReqApproved = window.location.hash.toLowerCase() == "#approved";
    var ReqSubmitted = window.location.hash.toLowerCase() == "#submitted";
    var ReqUnsubmitted = window.location.href.indexOf("Consume") != -1 && window.location.hash.toLowerCase() == "#unsubmitted";
    var ReqClosed = window.location.hash.toLowerCase() == "#closed";
    /**
     * on page load , get data from server and store data in loadedData object 
     * and set in related variables
     * */
    self.loadedData = (function () {
        var loadedDataObj = {};
        loadedDataObj.listName = null;
        loadedDataObj.objJson = null;

        loadedDataObj.processedDataLen = 0;

        /**
         *  mark object data as processed , check if all loaded data are processed
         * @param {any} obj
         */
        loadedDataObj.setProcessed = function (obj) {
            obj.isProcessed = true;
            loadedDataObj.processedDataLen = loadedDataObj.processedDataLen + 1;
            setGlobalVariableVal(obj);

            processDeletedUDF();
            if (loadedDataObj.processedDataLen >= loadedDataObj.objJson.length) {
                // all data are processed 
                // narrow search 
                if (self.isPageLoading) {
                    if (self.isNarrowSearchVisible() == true) {
                        self.isApplySearchOnTabChange = false;
                        loadedDataObj.call_narrowSearch();
                    }
                    else {
                        self.isApplySearchOnTabChange = true;
                    }
                }

                loadedDataObj.processedDataLen = 0;

                resetProcessed();
                self.isPageLoading = false;
                isControlValuesLoading = false;
                // reset
                //loadedDataObj.reset();
            }
        }

        resetProcessed = function () {
            var arrObj = loadedDataObj.objJson;
            if (arrObj) {
                for (var i = 0; i < arrObj.length; i++) {
                    var obj = arrObj[i];
                    obj.isProcessed = false;
                }
            }
        }

        processDeletedUDF = function () {
            // handle if saved search has udf and the udf do not exist on page
            if (loadedDataObj.processedDataLen >= loadedDataObj.objJson.length - 10) {
                for (var i = 0; i < loadedDataObj.objJson.length; i++) {
                    var obj = loadedDataObj.objJson[i];
                    if (obj.ctlType == ctlTypeEnum.UDF &&
                        (typeof obj.isProcessed == 'undefined' || obj.isProcessed == false)) {
                        var udfDDL = getUDFCtl(obj);
                        if (udfDDL.length == 0) {

                            if (_utils.isNullUndefined(obj.arrVal) == false) {
                                isValidFilter = false;
                            }

                            obj.val = null;
                            obj.arrVal = null;
                            obj.isNotFound = true;
                            loadedDataObj.setProcessed(obj);
                        }
                    }
                }
            }
        }

        loadedDataObj.reset = function () {
            loadedDataObj.listName = null;
            loadedDataObj.objJson = null;
            loadedDataObj.processedDataLen = 0;
            loadedDataObj.dataLen = 0;
        }


        setGlobalVariableVal = function (obj) {
            // set global variable value defined in common.js
            var val = getFieldValueFromObj(obj);
            var ctlID = _utils.isNullUndefinedBlank(obj.ctlId) ? obj.ctlName : obj.ctlId;
            self.objSearch[ctlID] = val;
            var ctlIDName = _utils.isNullUndefinedBlank(obj.ctlId) ? obj.ctlName : obj.ctlId;
            switch (ctlIDName) {
                case "GlobalSearch":
                    if (self.currentListName == self.listNameEnum.NewPULL) {
                        $("#ItemModel_filter").val(val);
                    }
                    else {
                        $("#global_filter").val(val);
                    }
                    break;
                case "UserCreated":
                case "OrderCreatedBy":
                    UserCreatedNarroValues = val;
                    CreatedBySelectedValues = val;
                    break;
                case "UserUpdated":
                case "OrderUpdatedBy":
                    UserUpdatedNarroValues = val;
                    UpdatedBySelectedValues = val;
                    break;
                case "PullSupplier":
                    PullSupplierNarroValues = val;
                    break;
                case "OrderSupplier":
                    SupplierBySelectedValues = val;
                    break;
                case "OrderStatus":
                    StatusBySelectedValues = val;
                    break;
                case "OrderRequiredDate":
                    RequiredDateSelectedValues = val;
                    OrderRequiredDateNarroValues = val;
                    break;
                case "OrderShippingVendor":
                    WorkOrderSelectedValues = val;
                    break;
                case "OrderPackslipNumber":
                    PackslipNumberSelectedValues = val;
                    break;
                case "PullCategory":
                    PullCategoryNarroValues = val;
                    break;
                case "Manufacturer":
                    ManufacturerNarroValues = val;
                    break;
                case "ItemLocation":
                    ItemLocationNarroValues = val;
                    break;
                case "ItemTrackingType":
                    ItemTrackingTypeNarroValues = val;
                    break;
                case "StockStatus":
                    if (self.currentListName == self.listNameEnum.NewPULL) {
                        SSNarroSearchValue = val;
                    }
                    else {
                        StockStatusTypeNarroValues = val;
                    }
                    break;
                case "ItemTypeNarroDDL":
                    ItemTypeNarroSearchValue = val;
                    break;
                case "InventoryClassificationDDL":
                    InventoryClassificationNarroSearchValue = val;
                    break;
                case "IsActive":
                    ItemActive = val;
                    break;
                case "PullCost":
                    CostNarroSearchValue = val;
                    break;
                case "AverageUsage":
                    AverageCostNarroSearchValue = val;
                    break;
                case "Turns":
                    TurnsNarroSearchValue = val;
                    break;
                case "UDF1":
                    UserUDF1NarrowValues = val;
                    break;
                case "UDF2":
                    UserUDF2NarrowValues = val;
                    break;
                case "UDF3":
                    UserUDF3NarrowValues = val;
                    break;
                case "UDF4":
                    UserUDF4NarrowValues = val;
                    break;
                case "UDF5":
                    UserUDF5NarrowValues = val;
                    break;
                case "UDF6":
                    UserUDF6NarrowValues = val;
                    break;
                case "UDF7":
                    UserUDF7NarrowValues = val;
                    break;
                case "UDF8":
                    UserUDF8NarrowValues = val;
                    break;
                case "UDF9":
                    UserUDF9NarrowValues = val;
                    break;
                case "UDF10":
                    UserUDF10NarrowValues = val;
                    break;
                case "StageLocationHeaderIM":
                    StageLocationHeaderNarroValues = val;
                    break;
                case "StageLocationIM":
                    StageLocationNarroValues = val;
                    break;
                case "ManufacturerIM":
                    ManufacturerNarroValues = val;
                    break;
                case "PullCategoryIM":
                    PullCategoryNarroValues = val;
                    break;
                case "PullSupplierIM":
                    PullSupplierNarroValues = val;
                    break;
                case "ItemTypeIM":
                    ItemTypeNarroSearchValue = val;
                    break;
                case "UserCreatedIM":
                    UserCreatedNarroValues = val;
                    break;
                case "UserUpdatedIM":
                    UserUpdatedNarroValues = val;
                    break;
                case "PullCostIM":
                    CostNarroSearchValue = val;
                    break;
                case "NarroSearchAvgUsage":
                    AvgUsageNarroSearchValue = val;
                    break;
                case "NarroSearchTurns":
                    TurnsNarroSearchValue = val;
                    break;
                case "PullActionType":
                    PullActionTypeNarroSearchValue = val;
                    break;
                case "PullProjectSpend":
                    PullProjectSpendNarroValues = val;
                    break;
                case "PullWorkOrder":
                    PullWorkOrderValues = val;
                    break;
                case "PullRequistion":
                    PullRequistionarroValues = val;
                    break;
                case "PullOrderNumber":
                    PullOrderNumbernarroValues = val;
                    break;
                case "PullConsignment":
                    PullConsignmentNarroSearchValue = val;
                    break;
                case "PullSupplierAccountNumber":
                    PullSupplierAccountNumberValues = val;
                    break;
                case "isBilling":
                    IsBillingNarroSearchValue = val;
                    break;
                case "isEDISent":
                    IsEDISentNarroSearchValue = val;
                    break;
                case "ReceiveSupplier":
                    ReceiveSupplierNarroValues = val;
                    break;
                case "ReceivePO":
                    ReceivePONarroValues = val;
                    break;
                case "POReceiveaDate":
                    POReceiveaDateValues = val;
                    break;
                case "ReceiveCreatedBy":
                    ReceiveCreatedByNarroValues = val;
                    break;
                case "ReceiveUpdatedBy":
                    ReceiveUpdatedByNarroValues = val;
                    break;
                case "ReceiveDateCFrom":
                    ReceiveDateCFrom = val;
                    break;
                case "ReceiveDateCTo":
                    ReceiveDateCTo = val;
                    break;
                case "ReceiveDateUFrom":
                    ReceiveDateUFrom = val;
                    break;
                case "ReceiveDateUTo":
                    ReceiveDateUTo = val;
                    break;
                case "OrderUDF1":
                    OrderUDF1NarrowValues = val;
                    break;
                case "RequisitionSupplier":
                    RequistionSupplierValues = val;
                    break;
                case "ReqCustomer":
                    OrderSupplierNarroValues = val;
                    ReqCustomerNarroValues = val;
                    break;
                case "ReqWorkOrder":
                    WorkOrderNarroValues = val;
                    ReqWorkOrderNarroValues = val;
                    break;
                case "ddlRequisitionStatus":
                    ddlRequisitionStatusNarroValues = val;
                    break;
                case "ICountType":
                    ICountTypeNarroValues = val;
                    break;
                case "ICountStatus":
                    ICountStatusNarroValues = val;
                    break;
                case "ToolsCategory":
                case "ToolsCategoryNew":
                    ToolCategoryValue = val;
                    break;
                case "ToolsCost":
                case "ToolsCostNew":
                case "ToolHistoryCost":
                    ToolCostValue = val;
                    break;
                case "ToolCheckout":
                case "ToolCheckoutNew":
                    MaintenanceValue = val;
                    break;
                case "ToolsTechnician":
                case "ToolsTechnicianNew":
                    ToolTechnicianValue = val;
                    break;
                case "ToolsLocation":
                case "ToolsLocationNew":
                    LocationValue = val;
                    break;
                case "ToolHistoryCategory":
                case "ToolHistoryCategoryNew":
                    ToolHistoryCategoryValue = val;
                    break;
                case "ToolHistoryCheckout":
                case "ToolHistoryCheckoutNew":
                    THMaintenanceValue = val;
                    break;
                case "ToolHistoryTechnician":
                case "ToolHistoryTechnicianNew":
                    ToolHistoryTechnicianValue = val;
                    break;
                case "ToolHistoryCreatedBy":
                case "ToolHistoryCreatedByNew":
                    ToolHistoryCreatedByNarroValues = val;
                    break;
                case "ToolHistoryUpdatedBy":
                case "ToolHistoryUpdatedByNew":
                    ToolHistoryUpdatedByNarroValues = val;
                    break;
                case "ToolWrittenOffCategory":
                    ToolWrittenOffCategoryValue = val;
                    break;
                case "ToolWrittenOffCreatedBy":
                    ToolWrittenOffCreatedByNarroValues = val;
                    break;
                case "ToolWrittenOffUpdatedBy":
                    ToolWrittenOffUpdatedByNarroValues = val;
                    break;
                case "AssetsCategory":
                    AssetCategoryValue = val;
                    break;
                case "ToolHistoryLocationNew":
                    THLocationValue = val;
                    break;
                case "ToolWrittenOffDateCFrom":
                    DateCFrom = val;
                    break;
                case "ToolWrittenOffDateCTo":
                    DateCTo = val;
                    break;
                case "ToolWrittenOffDateUFrom":
                    DateUFrom = val;
                    break;
                case "ToolWrittenOffDateUTo":
                    DateUTo = val;
                    break;
                case "QuoteStatus":
                    QuoteStatusNarroValues = val;
                    break;
                case "QuoteSupplier":
                    QuoteSupplierNarrowValues = val;
                    break;
            }
        }

        loadedDataObj.isSearchHasValue = function () {
            // is search has any value in criteria
            var isAnyValue = false;
            var arrObj = loadedDataObj.objJson;
            if (arrObj) {
                for (var i = 0; i < arrObj.length; i++) {
                    var obj = arrObj[i];
                    var val = getFieldValueFromObj(obj);
                    if (!_utils.isNullUndefined(val) && val != "") {
                        isAnyValue = true;
                        break;
                    }
                }
            }

            return isAnyValue;
        }

        loadedDataObj.call_narrowSearch = function () {
            var msg = "";
            var isSearchHasValue = loadedDataObj.isSearchHasValue();
            if (isValidFilter == false) {
                msg = MsgInvalidFilterCrtNotification;

                if (self.currentOperation == self.operationEnum.ReloadSearch) {
                    DoNarrowSearchForList(msg, true);
                } else if (isSearchHasValue) {
                    DoNarrowSearchForList(msg, true);
                }
                else {
                    if (self.isNarrowSearchVisible() == true) {
                        _notify.showWarning(msg);
                    }
                }

            }
            else if (isSearchHasValue) {
                msg = MsgFilterCriteriaApplied;

                //if (self.currentOperation == self.operationEnum.ReloadSearch) {
                //    msg = '';
                //}

                DoNarrowSearchForList(msg, false);

            }
            else if (self.isApplySearchOnTabChange) {
                DoNarrowSearchForList(msg, false);
                self.currentOperation = null;
            }
            self.currentOperation = null;
            self.isApplySearchOnTabChange = false;
        }

        var DoNarrowSearchForList = function (msg, isWarn) {
            _utils.showHideLoader(true);
            switch (self.currentListName) {
                case self.listNameEnum.ItemMaster:
                case self.listNameEnum.ItemMasterPictureView:
                case self.listNameEnum.ItemBinMaster:
                case self.listNameEnum.PullMaster:
                case self.listNameEnum.InventoryCount:
                case self.listNameEnum.ToolMaster:
                case self.listNameEnum.ToolHistory:
                case self.listNameEnum.AssetMaster:
                case self.listNameEnum.ToolMasterNew:
                case self.listNameEnum.QuoteMaster:
                    DoNarrowSearch();
                    break;
                case self.listNameEnum.RequisitionMaster:
                case self.listNameEnum.RequisitionMasterApproved:
                case self.listNameEnum.RequisitionMasterSubmitted:
                case self.listNameEnum.RequisitionMasterUnsubmitted:
                case self.listNameEnum.RequisitionMasterClosed:
                    DoReqNarrowSearch();
                    break;
                case self.listNameEnum.NewPULL:
                case self.listNameEnum.ReceiveItemWithoutOrder:
                    DoNarrowSearchIM();
                    break;
                case self.listNameEnum.ReceiveMaster:
                case self.listNameEnum.ReceiveMasterIncomplete:
                case self.listNameEnum.ToolHistoryNew:
                    DoNarrowSearchSC();
                    break;
                case self.listNameEnum.OrderMaster:
                case self.listNameEnum.OrderMasterUnSubmitted:
                case self.listNameEnum.OrderMasterApprove:
                case self.listNameEnum.OrderMasterReceive:
                case self.listNameEnum.OrderMasterChangeOrder:
                    DoOrderNarrowSearch();
                    break;
                case self.listNameEnum.WrittenOffToolList:
                    DoNarrowSearchWOT();
                    break;
            }

            setTimeout(function () {
                _utils.showHideLoader(false);
                if (self.isNarrowSearchVisible() == true) {
                    if (msg != '') {
                        if (isWarn) {
                            _notify.showWarning(msg);
                        }
                        else {
                            _notify.showSuccess(msg);
                        }
                    }
                }
            }, 800);
        }

        return loadedDataObj;
    })();

    self.init = function (companyID, roomID, enterpriseID) {
        self.companyID = companyID;
        self.roomID = roomID;
        self.enterpriseID = enterpriseID;
    };

    /**
     * Call DoNarrowSearch() in case "add new tab" is loaded first
     * */
    self.applySearchOnTabChange = function () {
        if (self.isApplySearchOnTabChange) {
            self.currentOperation = self.operationEnum.SearchOnTabChange
            self.loadedData.call_narrowSearch();
        }
    }

    self.saveNarrowSearch = function (btnSaveID, listName) {
        if (CurrentPageName.toLowerCase() == "requisitionmaster") {
            if (window.location.hash.toLowerCase() == "#list") {
                listName = "RequisitionMaster";
            }
            else if (window.location.hash.toLowerCase() == "#unsubmitted") {
                listName = "RequisitionMasterUnsubmitted";
            }
            else if (window.location.hash.toLowerCase() == "#submitted") {
                listName ="RequisitionMasterSubmitted";
            }
            else if (window.location.hash.toLowerCase() == "#approved") {
                listName = "RequisitionMasterApproved";
            }
            else if (window.location.hash.toLowerCase() == "#closed") {
                listName = "RequisitionMasterClosed"
            }
        }
        else {
            if (ReceiveIncomplateTab) {
                listName = "OrderMasterIncomplete";
            }
            if (ReqApproved) {
                listName = "OrderMasterApproved";
            }
            if (ReqSubmitted) {
                listName = "OrderMasterSubmitted";
            }
            if (ReqUnsubmitted) {
                listName = "OrderMasterUnsubmitted";
            }
            if (ReqClosed) {
                listName = "OrderMasterClosed";
            }
        }
        var btnSave = $("#" + btnSaveID);
        var id = btnSave.attr("data-id");
        var json = JSON.stringify(getJsonToSave(listName));
        _utils.showHideLoader(true);
        _AjaxUtil.postJson("/UserNarrowSearchSettings/SaveUserNarrowSearchSettings"
            , { ID: id, ListName: listName, SettingsJson: json }
            , function (resp) {
                _utils.showHideLoader(false);
                var status = resp.status;
                btnSave.attr("data-id", resp.ID);
                _notify.showSuccess(status);
            },
            function (error) {
                _utils.showHideLoader(false);
                _notify.showError(MsgErrorInProcess);
                console.log(error);
            }
        );
    }


    self.loadNarrowSearch = function () {
        //if (self.isNarrowSearchVisible() == false) {
        //    return;
        //}
        var btnSaveID = self.currentBtnSaveID;
        //var listName = ;        
        var btnSave = $("#" + btnSaveID);

        if (btnSave.length == 0) {
            // save button not found
            return;
        }

        _utils.showHideLoader(true);
        isLoadingSettings = true;

        _AjaxUtil.getJson("/UserNarrowSearchSettings/GetUserNarrowSearchSettings"
            , { listName: self.currentListName }
            , function (resp) {
                var obj = resp.obj;
                _utils.showHideLoader(false);
                self.objSearch.reset();
                if (_utils.isNullUndefined(obj) == false) {
                    var objJson = JSON.parse(obj.SettingsJson);
                    btnSave.attr("data-id", obj.ID);
                    
                    self.loadedData.reset();
                    self.loadedData.listName = self.currentListName;
                    self.loadedData.objJson = objJson;


                    // check if any search value is found

                    if (self.loadedData.isSearchHasValue()) {
                        isValidFilter = true;

                        $(function () {
                            if (window.location.hash != "#new") {
                                _notify.showSuccess(MsgFilterCriteriaLoading);
                            }
                            else if (window.location.hash == "#new" && self.listNameEnum.ReceiveItemWithoutOrder == 'ReceiveItemWithoutOrder') {
                                _notify.showSuccess(MsgFilterCriteriaLoading);
                            }
                        });
                    }

                    //setNarrowSearchData(objJson, listName);
                }
                else {
                    btnSave.attr("data-id", 0);
                }
                isLoadingSettings = false;
            },
            function (error) {
                _utils.showHideLoader(false);
                _notify.showError(MsgErrorInProcess);
                console.log(error);
                isLoadingSettings = false;
            }, false, false
        );
    }

    /**
     * Set control value from narrow search saved data
     * @param {any} ctlId
     */
    self.setControlValue = function (ctlId) {

        //if (self.loadedData.listName == null && self.loadedData.objJson == null) {
        //    // get narrow search saved data from server if not loaded
        //    self.loadNarrowSearch();
        //    setStaticControlValues();
        //}
        if (_utils.isNullUndefined(self.loadedData.listName) || _utils.isNullUndefined(self.loadedData.objJson)
            || self.loadedData.objJson.length == 0
        ) {
            return;
        }

        if (
            (self.isPageLoading == false && isControlValuesLoading == false)
            || (self.currentOperation == self.operationEnum.ReloadSearch && isControlValuesLoading == false)
        ) {
            isControlValuesLoading = true;
            self.loadedData.listName = self.currentListName;
            self.loadedData.objJson = getJsonToSave(self.currentListName);
            self.setStaticControlValues();
        }
        processControlValue(ctlId);
    }

    /**
     * Narrow search controls are reloading via ajax
     * */
    self.processSearchReload = function () {
        //_notify.showSuccess("Filter criteria re-loading ...");

        self.isPageLoading = true;
        isControlValuesLoading = false;
        self.loadedData.processedDataLen = 0;
        self.currentOperation = self.operationEnum.ReloadSearch;
    }

    var processControlValue = function (ctlId) {
        var arrObj = self.loadedData.objJson;
        if (_utils.isNullUndefined(arrObj)) {
            return;
        }
        for (var i = 0; i < arrObj.length; i++) {
            var obj = arrObj[i];

            if (_utils.isNullUndefined(obj.isProcessed) == false && obj.isProcessed == true) {
                continue;
            }

            if (obj.ctlId == ctlId || obj.ctlName == ctlId) {
                setControlValueFromObj(obj);
                self.loadedData.setProcessed(obj);
                break;
            }
        }
    }

    // set control values which are loaded without ajax call
    self.setStaticControlValues = function () {
        //if (self.isNarrowSearchVisible() == false) {
        //    return;
        //}

        //var listName = ;
        switch (self.currentListName) {
            case this.listNameEnum.ItemMaster:
            case this.listNameEnum.ItemMasterPictureView:
            case this.listNameEnum.ItemBinMaster:
                processControlValue("GlobalSearch");
                processControlValue("IsActive");
                processControlValue("PullCost");
                processControlValue("AverageUsage");
                processControlValue("Turns");
                processControlValue("DateCFrom");
                processControlValue("DateCTo");
                processControlValue("DateUFrom");
                processControlValue("DateUTo");
                break;
            case this.listNameEnum.NewPULL:
            case this.listNameEnum.ReceiveItemWithoutOrder:
                processControlValue("GlobalSearch");
                processControlValue("DateCFromIM");
                processControlValue("DateCToIM");
                processControlValue("DateUFromIM");
                processControlValue("DateUToIM");
                processControlValue("PullCostIM");
                processControlValue("StockStatus");
                processControlValue("NarroSearchAvgUsage");
                processControlValue("NarroSearchTurns");
                break;
            case this.listNameEnum.PullMaster:
                processControlValue("GlobalSearch");
                processControlValue("DateCFrom");
                processControlValue("DateCTo");
                processControlValue("DateUFrom");
                processControlValue("DateUTo");
                processControlValue("PullCost");
                processControlValue("isBilling");
                processControlValue("isEDISent");
                break;
            case this.listNameEnum.ReceiveMaster:
            case this.listNameEnum.ReceiveMasterIncomplete:
                processControlValue("ReceiveDateCFrom");
                processControlValue("ReceiveDateCTo");
                processControlValue("ReceiveDateUFrom");
                processControlValue("ReceiveDateUTo");
                break;
            case self.listNameEnum.OrderMaster:
            case self.listNameEnum.OrderMasterUnSubmitted:
            case self.listNameEnum.OrderMasterApprove:
            case self.listNameEnum.OrderMasterReceive:
            case self.listNameEnum.OrderMasterChangeOrder:
                //processControlValue("GlobalSearch");
                processControlValue("OrdDateCFrom");
                processControlValue("OrdDateCTo");
                processControlValue("OrdDateUFrom");
                processControlValue("OrdDateUTo");
                break;
            case self.listNameEnum.RequisitionMaster:
            case self.listNameEnum.RequisitionMasterApproved:
            case self.listNameEnum.RequisitionMasterSubmitted:
            case self.listNameEnum.RequisitionMasterUnsubmitted:
            case self.listNameEnum.RequisitionMasterClosed:
            case self.listNameEnum.InventoryCount:
            case self.listNameEnum.AssetMaster:
            case self.listNameEnum.QuoteMaster:
                processControlValue("DateCFrom");
                processControlValue("DateCTo");
                processControlValue("DateUFrom");
                processControlValue("DateUTo");
                break;
            case self.listNameEnum.ToolMaster:
                processControlValue("DateCFrom");
                processControlValue("DateCTo");
                processControlValue("DateUFrom");
                processControlValue("DateUTo");
                processControlValue("ToolsCost");
                processControlValue("UserCreated");
                processControlValue("UserUpdated");
                break;
            case self.listNameEnum.ToolHistory:
                processControlValue("ToolHistoryDateCFrom");
                processControlValue("ToolHistoryDateCTo");
                processControlValue("ToolHistoryDateUFrom");
                processControlValue("ToolHistoryDateUTo");
                processControlValue("ToolHistoryToolsCost");
                processControlValue("ToolHistoryUserCreated");
                processControlValue("ToolHistoryUserUpdated");
                break;
            case self.listNameEnum.WrittenOffToolList:
                processControlValue("ToolWrittenOffDateCFrom");
                processControlValue("ToolWrittenOffDateCTo");
                processControlValue("ToolWrittenOffDateUFrom");
                processControlValue("ToolWrittenOffDateUTo");
                processControlValue("ToolWrittenOffUserCreated");
                processControlValue("ToolWrittenOffUserUpdated");
                break;
            case self.listNameEnum.ToolMasterNew:
                processControlValue("DateCFrom");
                processControlValue("DateCTo");
                processControlValue("DateUFrom");
                processControlValue("DateUTo");
                processControlValue("ToolsCostNew");
                break;
            case self.listNameEnum.ToolHistoryNew:
                processControlValue("ToolHistoryDateCFrom");
                processControlValue("ToolHistoryDateCTo");
                processControlValue("ToolHistoryDateUFrom");
                processControlValue("ToolHistoryDateUTo");
                processControlValue("ToolHistoryCost");
                processControlValue("ToolHistoryCreatedByNew");
                processControlValue("ToolHistoryUpdatedByNew");
                break;
        }
    }

    self.isNarrowSearchVisible = function () {
        var b = $(".IteamBlock").is(":visible");
        return b;
    }

    /**
     * set All Control Values if all controls have loaded data
     * */
    //self.setAllControlValues = function () {
    //    self.loadedData.reset();
    //    self.loadNarrowSearch();
    //    var arrObj = self.loadedData.objJson;

    //    for (var i = 0; i < arrObj.length; i++) {
    //        var obj = arrObj[i];
    //        setControlValueFromObj(obj);
    //    }
    //}


    setControlValueFromObj = function (obj) {

        var val = getFieldValueFromObj(obj);
        obj.isValNotFound = false;
        switch (obj.ctlType) {
            case ctlTypeEnum.MultiSelect:
            case ctlTypeEnum.UDF:

                var ctlId = obj.ctlId;

                if (obj.ctlType == ctlTypeEnum.UDF) {
                    ctlId = getUDFCtl(obj).prop("id");
                }

                var arrVal = _multiSelectWrapper.tickMultipleCheckbox(ctlId, val);
                var msg = "";
                if (typeof obj.arrVal !== 'undefined'
                    && obj.arrVal != null
                    && obj.arrVal.length > 0
                    && obj.arrVal.length != arrVal.length) {

                    obj.isValNotFound = true;
                    isValidFilter = false;
                    //msg = "Filter criteria has invalid values. Please update criteria.";
                    //if (self.isNarrowSearchVisible() == true) {
                    //    _notify.showWarning(msg);
                    //}
                }

                obj.arrVal = arrVal;
                break;
            case ctlTypeEnum.Select:
                var ctl = null;
                if (_utils.isNullUndefinedBlank(obj.ctlId) == false) {
                    ctl = $("#" + obj.ctlId, getNarrowSearchDiv());
                }
                else {
                    ctl = $("[name = " + obj.ctlName + "]");
                }

                if (ctl.length) {
                    ctl.val(val);
                }

                break;
            case ctlTypeEnum.FromDate:
                //$("#" + obj.ctlId).val(val);
                _datePicker.setDate(obj.ctlId, val);
                break;
            case ctlTypeEnum.ToDate:
                //$("#" + obj.ctlId).val(val);
                _datePicker.setDate(obj.ctlId, val);
                break;
            case ctlTypeEnum.TextBox:
                $("#" + obj.ctlId).val(val);
                break;
            case ctlTypeEnum.Radio:
                $("input[name=" + obj.ctlId + "][value=" + value + "]").prop('checked', true);
                break;
        }
    }

    var getUDFCtl = function (obj) {
        return getUDFCtlById(obj.ctlId); //$("[uid='" + obj.ctlId + "']");
    }

    var getUDFCtlById = function (ctlId) {
        return $("[uid='" + ctlId + "']", getNarrowSearchDiv());
    }

    var getNarrowSearchDiv = function () {
        var div = $(".IteamBlock");
        return div;
    }

    var getFieldValueFromObj = function (obj) {
        var val = '';
        switch (obj.ctlType) {
            case ctlTypeEnum.GlobalSearch:
                val = obj.val;
                break;
            case ctlTypeEnum.Select:
                val = obj.val;
                break;
            case ctlTypeEnum.MultiSelect:
                val = obj.arrVal;
                break;
            case ctlTypeEnum.TextBox:
                val = obj.val;
                break;
            case ctlTypeEnum.FromDate:
            case ctlTypeEnum.ToDate:
                val = obj.val;
                break;
            case ctlTypeEnum.Radio:
                val = obj.val;
                break;
            case ctlTypeEnum.UDF:
                val = obj.arrVal;
                break;
        }
        return val;
    }

    var getJsonToSave = function (listName) {

        var obj = {};
        switch (listName) {
            case self.listNameEnum.ItemMaster:
                obj = getItemMasterSaveObj();
                break;
            case self.listNameEnum.ItemMasterPictureView:
                obj = getItemMasterSaveObj();
                break;
            case self.listNameEnum.ItemBinMaster:
                obj = getItemBinMasterSaveObj();
                break;
            case self.listNameEnum.NewPULL:
                obj = getNewPULLSaveObj();
                break;
            case self.listNameEnum.PullMaster:
                obj = getPullMasterSaveObj();
                break;
            case self.listNameEnum.ReceiveMaster:
            case self.listNameEnum.ReceiveMasterIncomplete:
                obj = getReceiveMasterSaveObj();
                break;
            case self.listNameEnum.OrderMaster:
            case self.listNameEnum.OrderMasterUnSubmitted:
            case self.listNameEnum.OrderMasterApprove:
            case self.listNameEnum.OrderMasterReceive:
            case self.listNameEnum.OrderMasterChangeOrder:
                obj = getOrderMasterSaveObj();
                break;
            case self.listNameEnum.ReceiveItemWithoutOrder:
                obj = getReceiveItemWithoutOrderSaveObj();
                break;
            case self.listNameEnum.RequisitionMaster:
            case self.listNameEnum.RequisitionMasterApproved:
            case self.listNameEnum.RequisitionMasterSubmitted:
            case self.listNameEnum.RequisitionMasterUnsubmitted:
            case self.listNameEnum.RequisitionMasterClosed:
                obj = getRequisitionMasterSaveObj();
                break;
            case self.listNameEnum.InventoryCount:
                obj = getInventoryCountSaveObj();
                break;
            case self.listNameEnum.ToolMaster:
                obj = getToolMasterSaveObj();
                break;
            case self.listNameEnum.ToolHistory:
                obj = getToolHistorySaveObj();
                break;
            case self.listNameEnum.WrittenOffToolList:
                obj = getWrittenOffToolListSaveObj();
                break;
            case self.listNameEnum.AssetMaster:
                obj = getAssetMasterSaveObj();
                break;
            case self.listNameEnum.ToolMasterNew:
                obj = getToolMasterNewSaveObj();
                break;
            case self.listNameEnum.ToolHistoryNew:
                obj = getToolHistoryNewSaveObj();
                break;
            case self.listNameEnum.QuoteMaster:
                obj = getQuoteMasterSaveObj();
        }
        return obj;
    }

    // global narrow search object
    self.objSearch = {
        // controlid : value
        GlobalSearch: null,
        PullSupplier: null,
        OrderSupplier: null,
        OrderStatus: null,
        OrderRequiredDate: null,
        OrderShippingVendor: null,
        PullCategory: null,
        Manufacturer: null,
        ItemLocation: null,
        ItemTrackingType: null,
        StockStatus: null,
        IsActive: null,
        PullCost: null,
        AverageUsage: null,
        Turns: null,
        ItemTypeNarroDDL: null,
        InventoryClassificationDDL: null,
        UserCreated: null,
        UserUpdated: null,
        DateCFrom: null,
        DateCTo: null,
        DateUFrom: null,
        DateUTo: null,
        UDF1: null,
        UDF2: null,
        UDF3: null,
        UDF4: null,
        UDF5: null,
        UDF6: null,
        UDF7: null,
        UDF8: null,
        UDF9: null,
        UDF10: null,

        StageLocationHeaderIM: null,
        StageLocationIM: null,
        ManufacturerIM: null,
        PullCategoryIM: null,
        PullSupplierIM: null,
        DateCFromIM: null,
        DateCToIM: null,

        DateUFromIM: null,
        DateUToIM: null,
        ItemTypeIM: null,
        UserCreatedIM: null,
        UserUpdatedIM: null,
        PullCostIM: null,
        NarroSearchAvgUsage: null,
        NarroSearchTurns: null,

        PullActionType: null,
        PullProjectSpend: null,
        PullWorkOrder: null,
        PullRequistion: null,
        PullOrderNumber: null,
        PullConsignment: null,
        PullSupplierAccountNumber: null,
        isBilling: null,
        isEDISent: null,

        ReceiveSupplier: null,
        ReceivePO: null,
        POReceiveaDate: null,
        ReceiveCreatedBy: null,
        ReceiveUpdatedBy: null,
        ReceiveDateCFrom: null,
        ReceiveDateCTo: null,
        ReceiveDateUFrom: null,
        ReceiveDateUTo: null,

        RequisitionSupplier: null,
        ReqCustomer: null,
        ReqWorkOrder: null,
        ddlRequisitionStatus: null,
        ICountType: null,
        ICountStatus: null,

        ToolsCategory: null,
        ToolsCategoryNew: null,
        ToolsCost: null,
        ToolsCostNew: null,
        ToolCheckout: null,
        ToolCheckoutNew: null,
        ToolsTechnicianNew: null,
        ToolsTechnician: null,
        ToolsLocationNew: null,
        ToolsLocation: null,
        ToolHistoryCategory: null,
        ToolHistoryCategoryNew: null,
        ToolHistoryCost: null,
        ToolHistoryCheckout: null,
        ToolHistoryCheckoutNew: null,
        ToolHistoryTechnician: null,
        ToolHistoryTechnicianNew: null,
        ToolHistoryCreatedBy: null,
        ToolHistoryCreatedByNew: null,
        ToolHistoryUpdatedBy: null,
        ToolHistoryUpdatedByNew: null,
        ToolHistoryLocation: null,
        ToolHistoryLocationNew: null,

        ToolWrittenOffCategory: null,
        ToolWrittenOffCreatedBy: null,
        ToolWrittenOffUpdatedBy: null,
        AssetsCategory: null,
        QuoteStatus: null,
        QuoteSupplier: null,
        OrderPackslipNumber: null,

        reset: function () {
            this.GlobalSearch = null;
            this.PullSupplier = null;
            this.OrderSupplier = null;
            this.OrderStatus = null;
            this.OrderRequiredDate = null;
            this.OrderShippingVendor = null;
            this.PullCategory = null;
            this.Manufacturer = null;
            this.ItemLocation = null;
            this.ItemTrackingType = null;
            this.StockStatus = null;
            this.IsActive = null;
            this.PullCost = null;
            this.AverageUsage = null;
            this.Turns = null;
            this.ItemTypeNarroDDL = null;
            this.InventoryClassificationDDL = null;
            this.UserCreated = null;
            this.UserUpdated = null;
            this.DateCFrom = null;
            this.DateCTo = null;
            this.DateUFrom = null;
            this.DateUTo = null;
            this.UDF1 = null;
            this.UDF2 = null;
            this.UDF3 = null;
            this.UDF4 = null;
            this.UDF5 = null;
            this.UDF6 = null;
            this.UDF7 = null;
            this.UDF8 = null;
            this.UDF9 = null;
            this.UDF10 = null;

            this.StageLocationHeaderIM = null;
            this.StageLocationIM = null;
            this.ManufacturerIM = null;
            this.PullCategoryIM = null;
            this.PullSupplierIM = null;

            this.DateCFromIM = null;
            this.DateCToIM = null;
            this.DateUFromIM = null;
            this.DateUToIM = null;
            this.ItemTypeIM = null;
            this.UserCreatedIM = null;
            this.UserUpdatedIM = null;

            this.PullCostIM = null;
            this.NarroSearchAvgUsage = null;
            this.NarroSearchTurns = null;

            this.PullActionType = null;
            this.PullProjectSpend = null;
            this.PullWorkOrder = null;
            this.PullRequistion = null;
            this.PullOrderNumber = null;
            this.PullConsignment = null;
            this.PullSupplierAccountNumber = null;
            this.isBilling = null;
            this.isEDISent = null;

            this.ReceiveSupplier = null;
            this.ReceivePO = null;
            this.POReceiveaDate = null;
            this.ReceiveCreatedBy = null;
            this.ReceiveUpdatedBy = null;
            this.ReceiveDateCFrom = null;
            this.ReceiveDateCTo = null;
            this.ReceiveDateUFrom = null;
            this.ReceiveDateUTo = null;

            this.RequisitionSupplier = null;
            this.ReqCustomer = null;
            this.ReqWorkOrder = null;
            this.ddlRequisitionStatus = null;

            this.ICountType = null;
            this.ICountStatus = null;

            this.ToolsCategory = null;
            this.ToolsCategoryNew = null;
            this.ToolsCost = null;
            this.ToolsCostNew = null;
            this.ToolCheckout = null;
            this.ToolCheckoutNew = null;
            this.ToolsTechnicianNew = null;
            this.ToolsTechnician = null;
            this.ToolsLocationNew = null;
            this.ToolsLocation = null;
            this.ToolHistoryCategory = null;
            this.ToolHistoryCategoryNew = null;
            this.ToolHistoryCost = null;
            this.ToolHistoryCheckout = null;
            this.ToolHistoryCheckoutNew = null;
            this.ToolHistoryTechnician = null;
            this.ToolHistoryTechnicianNew = null;
            this.ToolHistoryCreatedBy = null;
            this.ToolHistoryUpdatedBy = null;
            this.ToolHistoryLocation = null;
            this.ToolHistoryLocationNew = null;
            this.ToolWrittenOffCategory = null;
            this.AssetsCategory = null;
            this.QuoteStatus = null;
            this.QuoteSupplier = null;
            this.OrderPackslipNumber= null;
        }
    };


    var getItemMasterSaveObj = function () {
        var GlobalSearchVal = "";
        GlobalSearchVal = $("#global_filter").val();

        _NarrowSearchSave.objSearch.DateCFrom = getTxtDateVal('DateCFrom');
        _NarrowSearchSave.objSearch.DateCTo = getTxtDateVal('DateCTo');

        _NarrowSearchSave.objSearch.DateUFrom = getTxtDateVal('DateUFrom');
        _NarrowSearchSave.objSearch.DateUTo = getTxtDateVal('DateUTo');

        var obj = [
            new clsSearch('GlobalSearch', 'GlobalSearch', ctlTypeEnum.GlobalSearch, null, GlobalSearchVal),
            new clsSearch('SupplierID', 'PullSupplier', ctlTypeEnum.MultiSelect, self.objSearch.PullSupplier),
            new clsSearch('CategoryID', 'PullCategory', ctlTypeEnum.MultiSelect, self.objSearch.PullCategory),
            new clsSearch('ManufacturerID', 'Manufacturer', ctlTypeEnum.MultiSelect, self.objSearch.Manufacturer),
            new clsSearch('ItemLocations', 'ItemLocation', ctlTypeEnum.MultiSelect, self.objSearch.ItemLocation),
            new clsSearch('ItemTrackingType', 'ItemTrackingType', ctlTypeEnum.MultiSelect, self.objSearch.ItemTrackingType),
            new clsSearch('StockStatus', 'StockStatus', ctlTypeEnum.MultiSelect, self.objSearch.StockStatus),
            new clsSearch('IsActive', 'IsActive', ctlTypeEnum.Select, null, self.objSearch.IsActive),
            new clsSearch('Cost', 'PullCost', ctlTypeEnum.Select, null, self.objSearch.PullCost),
            new clsSearch('AverageCost', 'AverageUsage', ctlTypeEnum.Select, null, self.objSearch.AverageUsage),
            new clsSearch('Turns', 'Turns', ctlTypeEnum.Select, null, self.objSearch.Turns),
            new clsSearch('ItemType', 'ItemTypeNarroDDL', ctlTypeEnum.MultiSelect, self.objSearch.ItemTypeNarroDDL),
            new clsSearch('InventoryClassification', 'InventoryClassificationDDL', ctlTypeEnum.MultiSelect, self.objSearch.InventoryClassificationDDL),
            new clsSearch('CreatedBy', 'UserCreated', ctlTypeEnum.MultiSelect, self.objSearch.UserCreated),
            new clsSearch('UpdatedBy', 'UserUpdated', ctlTypeEnum.MultiSelect, self.objSearch.UserUpdated),
            new clsSearch('DateCreatedFrom', 'DateCFrom', ctlTypeEnum.FromDate, null, self.objSearch.DateCFrom),
            new clsSearch('DateCreatedFrom', 'DateCTo', ctlTypeEnum.ToDate, null, self.objSearch.DateCTo),
            new clsSearch('DateUpdatedFrom', 'DateUFrom', ctlTypeEnum.FromDate, null, self.objSearch.DateUFrom),
            new clsSearch('DateUpdatedFrom', 'DateUTo', ctlTypeEnum.ToDate, null, self.objSearch.DateUTo)

        ];

        if (getUDFCtlById('UDF1').length > 0) {
            obj.push(new clsSearch('UDF1', 'UDF1', ctlTypeEnum.UDF, self.objSearch.UDF1));
        }
        if (getUDFCtlById('UDF2').length > 0) {
            obj.push(new clsSearch('UDF2', 'UDF2', ctlTypeEnum.UDF, self.objSearch.UDF2));
        }
        if (getUDFCtlById('UDF3').length > 0) {
            obj.push(new clsSearch('UDF3', 'UDF3', ctlTypeEnum.UDF, self.objSearch.UDF3));
        }
        if (getUDFCtlById('UDF4').length > 0) {
            obj.push(new clsSearch('UDF4', 'UDF4', ctlTypeEnum.UDF, self.objSearch.UDF4));
        }
        if (getUDFCtlById('UDF5').length > 0) {
            obj.push(new clsSearch('UDF5', 'UDF5', ctlTypeEnum.UDF, self.objSearch.UDF5));
        }
        if (getUDFCtlById('UDF6').length > 0) {
            obj.push(new clsSearch('UDF6', 'UDF6', ctlTypeEnum.UDF, self.objSearch.UDF6));
        }
        if (getUDFCtlById('UDF7').length > 0) {
            obj.push(new clsSearch('UDF7', 'UDF7', ctlTypeEnum.UDF, self.objSearch.UDF7));
        }
        if (getUDFCtlById('UDF8').length > 0) {
            obj.push(new clsSearch('UDF8', 'UDF8', ctlTypeEnum.UDF, self.objSearch.UDF8));
        }
        if (getUDFCtlById('UDF9').length > 0) {
            obj.push(new clsSearch('UDF9', 'UDF9', ctlTypeEnum.UDF, self.objSearch.UDF9));
        }
        if (getUDFCtlById('UDF10').length > 0) {
            obj.push(new clsSearch('UDF10', 'UDF10', ctlTypeEnum.UDF, self.objSearch.UDF10));
        }

        return obj;
    }

    var getItemBinMasterSaveObj = function () {
        var GlobalSearchVal = "";
        GlobalSearchVal = $("#global_filter").val();

        _NarrowSearchSave.objSearch.DateCFrom = getTxtDateVal('DateCFrom');
        _NarrowSearchSave.objSearch.DateCTo = getTxtDateVal('DateCTo');

        _NarrowSearchSave.objSearch.DateUFrom = getTxtDateVal('DateUFrom');
        _NarrowSearchSave.objSearch.DateUTo = getTxtDateVal('DateUTo');

        var obj = [
            new clsSearch('GlobalSearch', 'GlobalSearch', ctlTypeEnum.GlobalSearch, null, GlobalSearchVal),
            new clsSearch('SupplierID', 'PullSupplier', ctlTypeEnum.MultiSelect, self.objSearch.PullSupplier),
            new clsSearch('CategoryID', 'PullCategory', ctlTypeEnum.MultiSelect, self.objSearch.PullCategory),
            new clsSearch('ManufacturerID', 'Manufacturer', ctlTypeEnum.MultiSelect, self.objSearch.Manufacturer),
            new clsSearch('ItemLocations', 'ItemLocation', ctlTypeEnum.MultiSelect, self.objSearch.ItemLocation),
            new clsSearch('ItemTrackingType', 'ItemTrackingType', ctlTypeEnum.MultiSelect, self.objSearch.ItemTrackingType),
            new clsSearch('StockStatus', 'StockStatus', ctlTypeEnum.MultiSelect, self.objSearch.StockStatus),
            new clsSearch('IsActive', 'IsActive', ctlTypeEnum.Select, null, self.objSearch.IsActive),
            new clsSearch('Cost', 'PullCost', ctlTypeEnum.Select, null, self.objSearch.PullCost),
            new clsSearch('AverageCost', 'AverageUsage', ctlTypeEnum.Select, null, self.objSearch.AverageUsage),
            new clsSearch('Turns', 'Turns', ctlTypeEnum.Select, null, self.objSearch.Turns),
            new clsSearch('ItemType', 'ItemTypeNarroDDL', ctlTypeEnum.MultiSelect, self.objSearch.ItemTypeNarroDDL),
            new clsSearch('InventoryClassification', 'InventoryClassificationDDL', ctlTypeEnum.MultiSelect, self.objSearch.InventoryClassificationDDL),
            new clsSearch('CreatedBy', 'UserCreated', ctlTypeEnum.MultiSelect, self.objSearch.UserCreated),
            new clsSearch('UpdatedBy', 'UserUpdated', ctlTypeEnum.MultiSelect, self.objSearch.UserUpdated),
            new clsSearch('DateCreatedFrom', 'DateCFrom', ctlTypeEnum.FromDate, null, self.objSearch.DateCFrom),
            new clsSearch('DateCreatedFrom', 'DateCTo', ctlTypeEnum.ToDate, null, self.objSearch.DateCTo),
            new clsSearch('DateUpdatedFrom', 'DateUFrom', ctlTypeEnum.FromDate, null, self.objSearch.DateUFrom),
            new clsSearch('DateUpdatedFrom', 'DateUTo', ctlTypeEnum.ToDate, null, self.objSearch.DateUTo)
        ];

        if (getUDFCtlById('UDF1').length > 0) {
            obj.push(new clsSearch('UDF1', 'UDF1', ctlTypeEnum.UDF, self.objSearch.UDF1));
        }
        if (getUDFCtlById('UDF2').length > 0) {
            obj.push(new clsSearch('UDF2', 'UDF2', ctlTypeEnum.UDF, self.objSearch.UDF2));
        }
        if (getUDFCtlById('UDF3').length > 0) {
            obj.push(new clsSearch('UDF3', 'UDF3', ctlTypeEnum.UDF, self.objSearch.UDF3));
        }
        if (getUDFCtlById('UDF4').length > 0) {
            obj.push(new clsSearch('UDF4', 'UDF4', ctlTypeEnum.UDF, self.objSearch.UDF4));
        }
        if (getUDFCtlById('UDF5').length > 0) {
            obj.push(new clsSearch('UDF5', 'UDF5', ctlTypeEnum.UDF, self.objSearch.UDF5));
        }
        if (getUDFCtlById('UDF6').length > 0) {
            obj.push(new clsSearch('UDF6', 'UDF6', ctlTypeEnum.UDF, self.objSearch.UDF6));
        }
        if (getUDFCtlById('UDF7').length > 0) {
            obj.push(new clsSearch('UDF7', 'UDF7', ctlTypeEnum.UDF, self.objSearch.UDF7));
        }
        if (getUDFCtlById('UDF8').length > 0) {
            obj.push(new clsSearch('UDF8', 'UDF8', ctlTypeEnum.UDF, self.objSearch.UDF8));
        }
        if (getUDFCtlById('UDF9').length > 0) {
            obj.push(new clsSearch('UDF9', 'UDF9', ctlTypeEnum.UDF, self.objSearch.UDF9));
        }
        if (getUDFCtlById('UDF10').length > 0) {
            obj.push(new clsSearch('UDF10', 'UDF10', ctlTypeEnum.UDF, self.objSearch.UDF10));
        }

        return obj;
    }

    var getNewPULLSaveObj = function () {
        var GlobalSearchVal = "";
        GlobalSearchVal = $("#ItemModel_filter").val();

        _NarrowSearchSave.objSearch.DateCFromIM = getTxtDateVal('DateCFromIM');
        _NarrowSearchSave.objSearch.DateCToIM = getTxtDateVal('DateCToIM');

        _NarrowSearchSave.objSearch.DateUFromIM = getTxtDateVal('DateUFromIM');
        _NarrowSearchSave.objSearch.DateUToIM = getTxtDateVal('DateUToIM');

        var obj = [
            new clsSearch('GlobalSearch', 'GlobalSearch', ctlTypeEnum.GlobalSearch, null, GlobalSearchVal),

            new clsSearch('MSID', 'StageLocationHeaderIM', ctlTypeEnum.MultiSelect, self.objSearch.StageLocationHeaderIM),
            new clsSearch('MSID', 'StageLocationIM', ctlTypeEnum.MultiSelect, self.objSearch.StageLocationIM),

            new clsSearch('ManufacturerID', 'ManufacturerIM', ctlTypeEnum.MultiSelect, self.objSearch.ManufacturerIM),
            new clsSearch('CategoryID', 'PullCategoryIM', ctlTypeEnum.MultiSelect, self.objSearch.PullCategoryIM),

            new clsSearch('SupplierID', 'PullSupplierIM', ctlTypeEnum.MultiSelect, self.objSearch.PullSupplierIM),
            new clsSearch('ItemType', 'ItemTypeIM', ctlTypeEnum.MultiSelect, self.objSearch.ItemTypeIM),
            new clsSearch('CreatedBy', 'UserCreatedIM', ctlTypeEnum.MultiSelect, self.objSearch.UserCreatedIM),
            new clsSearch('UpdatedBy', 'UserUpdatedIM', ctlTypeEnum.MultiSelect, self.objSearch.UserUpdatedIM),


            new clsSearch('DateCreatedFrom', 'DateCFromIM', ctlTypeEnum.FromDate, null, self.objSearch.DateCFromIM),
            new clsSearch('DateCreatedFrom', 'DateCToIM', ctlTypeEnum.ToDate, null, self.objSearch.DateCToIM),
            new clsSearch('DateUpdatedFrom', 'DateUFromIM', ctlTypeEnum.FromDate, null, self.objSearch.DateUFromIM),
            new clsSearch('DateUpdatedFrom', 'DateUToIM', ctlTypeEnum.ToDate, null, self.objSearch.DateUToIM),

            new clsSearch('Cost', 'PullCostIM', ctlTypeEnum.Select, null, self.objSearch.PullCostIM),
            new clsSearch('SS', 'StockStatus', ctlTypeEnum.Select, null, self.objSearch.StockStatus),
            new clsSearch('AverageUsage', null, ctlTypeEnum.Select, null, self.objSearch.NarroSearchAvgUsage, 'NarroSearchAvgUsage'),
            new clsSearch('Turns', null, ctlTypeEnum.Select, null, self.objSearch.NarroSearchTurns, 'NarroSearchTurns'),
        ];

        if (getUDFCtlById('UDF1').length > 0) {
            obj.push(new clsSearch('UDF1', 'UDF1', ctlTypeEnum.UDF, self.objSearch.UDF1));
        }
        if (getUDFCtlById('UDF2').length > 0) {
            obj.push(new clsSearch('UDF2', 'UDF2', ctlTypeEnum.UDF, self.objSearch.UDF2));
        }
        if (getUDFCtlById('UDF3').length > 0) {
            obj.push(new clsSearch('UDF3', 'UDF3', ctlTypeEnum.UDF, self.objSearch.UDF3));
        }
        if (getUDFCtlById('UDF4').length > 0) {
            obj.push(new clsSearch('UDF4', 'UDF4', ctlTypeEnum.UDF, self.objSearch.UDF4));
        }
        if (getUDFCtlById('UDF5').length > 0) {
            obj.push(new clsSearch('UDF5', 'UDF5', ctlTypeEnum.UDF, self.objSearch.UDF5));
        }
        if (getUDFCtlById('UDF6').length > 0) {
            obj.push(new clsSearch('UDF6', 'UDF6', ctlTypeEnum.UDF, self.objSearch.UDF6));
        }
        if (getUDFCtlById('UDF7').length > 0) {
            obj.push(new clsSearch('UDF7', 'UDF7', ctlTypeEnum.UDF, self.objSearch.UDF7));
        }
        if (getUDFCtlById('UDF8').length > 0) {
            obj.push(new clsSearch('UDF8', 'UDF8', ctlTypeEnum.UDF, self.objSearch.UDF8));
        }
        if (getUDFCtlById('UDF9').length > 0) {
            obj.push(new clsSearch('UDF9', 'UDF9', ctlTypeEnum.UDF, self.objSearch.UDF9));
        }
        if (getUDFCtlById('UDF10').length > 0) {
            obj.push(new clsSearch('UDF10', 'UDF10', ctlTypeEnum.UDF, self.objSearch.UDF10));
        }

        return obj;
    }

    var getPullMasterSaveObj = function () {
        var GlobalSearchVal = "";
        GlobalSearchVal = $("#global_filter").val();

        _NarrowSearchSave.objSearch.DateCFrom = getTxtDateVal('DateCFrom');
        _NarrowSearchSave.objSearch.DateCTo = getTxtDateVal('DateCTo');

        _NarrowSearchSave.objSearch.DateUFrom = getTxtDateVal('DateUFrom');
        _NarrowSearchSave.objSearch.DateUTo = getTxtDateVal('DateUTo');

        var obj = [
            new clsSearch('GlobalSearch', 'GlobalSearch', ctlTypeEnum.GlobalSearch, null, GlobalSearchVal),
            new clsSearch('SupplierID', 'PullSupplier', ctlTypeEnum.MultiSelect, self.objSearch.PullSupplier),
            new clsSearch('ActionType', 'PullActionType', ctlTypeEnum.MultiSelect, self.objSearch.PullActionType),

            new clsSearch('ManufacturerID', 'Manufacturer', ctlTypeEnum.MultiSelect, self.objSearch.Manufacturer),
            new clsSearch('CategoryID', 'PullCategory', ctlTypeEnum.MultiSelect, self.objSearch.PullCategory),

            new clsSearch('ProjectSpendID', 'PullProjectSpend', ctlTypeEnum.MultiSelect, self.objSearch.PullProjectSpend),
            new clsSearch('WorkOrderID', 'PullWorkOrder', ctlTypeEnum.MultiSelect, self.objSearch.PullWorkOrder),
            new clsSearch('RequistionID', 'PullRequistion', ctlTypeEnum.MultiSelect, self.objSearch.PullRequistion),

            new clsSearch('OrderNumber', 'PullOrderNumber', ctlTypeEnum.MultiSelect, self.objSearch.PullOrderNumber),
            new clsSearch('Consignment', 'PullConsignment', ctlTypeEnum.MultiSelect, self.objSearch.PullConsignment),
            new clsSearch('SupplierAccountNumber', 'PullSupplierAccountNumber', ctlTypeEnum.MultiSelect, self.objSearch.PullSupplierAccountNumber),

            new clsSearch('CreatedBy', 'UserCreated', ctlTypeEnum.MultiSelect, self.objSearch.UserCreated),
            new clsSearch('UpdatedBy', 'UserUpdated', ctlTypeEnum.MultiSelect, self.objSearch.UserUpdated),

            new clsSearch('DateCreatedFrom', 'DateCFrom', ctlTypeEnum.FromDate, null, self.objSearch.DateCFrom),
            new clsSearch('DateCreatedFrom', 'DateCTo', ctlTypeEnum.ToDate, null, self.objSearch.DateCTo),
            new clsSearch('DateUpdatedFrom', 'DateUFrom', ctlTypeEnum.FromDate, null, self.objSearch.DateUFrom),
            new clsSearch('DateUpdatedFrom', 'DateUTo', ctlTypeEnum.ToDate, null, self.objSearch.DateUTo),

            new clsSearch('Billing', 'isBilling', ctlTypeEnum.Select, null, self.objSearch.isBilling),
            new clsSearch('IsEDISent', 'isEDISent', ctlTypeEnum.Select, null, self.objSearch.isEDISent),
            new clsSearch('Cost', 'PullCost', ctlTypeEnum.Select, null, self.objSearch.PullCost),
        ];

        if (getUDFCtlById('UDF1').length > 0) {
            obj.push(new clsSearch('UDF1', 'UDF1', ctlTypeEnum.UDF, self.objSearch.UDF1));
        }
        if (getUDFCtlById('UDF2').length > 0) {
            obj.push(new clsSearch('UDF2', 'UDF2', ctlTypeEnum.UDF, self.objSearch.UDF2));
        }
        if (getUDFCtlById('UDF3').length > 0) {
            obj.push(new clsSearch('UDF3', 'UDF3', ctlTypeEnum.UDF, self.objSearch.UDF3));
        }
        if (getUDFCtlById('UDF4').length > 0) {
            obj.push(new clsSearch('UDF4', 'UDF4', ctlTypeEnum.UDF, self.objSearch.UDF4));
        }
        if (getUDFCtlById('UDF5').length > 0) {
            obj.push(new clsSearch('UDF5', 'UDF5', ctlTypeEnum.UDF, self.objSearch.UDF5));
        }


        return obj;
    }

    var getTxtDateVal = function (txtDateID) {
        var val = _datePicker.getFormatedDate(txtDateID, 'yy-mm-dd');
        if (val == '') {
            val = null;
        }
        return val;
    }

    var getReceiveMasterSaveObj = function () {
        if (ReceiveIncomplateTab) {
            _NarrowSearchSave.objSearch.ReceiveDateCFromIncomplete = getTxtDateVal('ReceiveDateCFrom');
            _NarrowSearchSave.objSearch.ReceiveDateCToIncomplete = getTxtDateVal('ReceiveDateCTo');

            _NarrowSearchSave.objSearch.ReceiveDateUFromIncomplete = getTxtDateVal('ReceiveDateUFrom');
            _NarrowSearchSave.objSearch.ReceiveDateUToIncomplete = getTxtDateVal('ReceiveDateUTo');
        }
        else {
            _NarrowSearchSave.objSearch.ReceiveDateCFrom = getTxtDateVal('ReceiveDateCFrom');
            _NarrowSearchSave.objSearch.ReceiveDateCTo = getTxtDateVal('ReceiveDateCTo');

            _NarrowSearchSave.objSearch.ReceiveDateUFrom = getTxtDateVal('ReceiveDateUFrom');
            _NarrowSearchSave.objSearch.ReceiveDateUTo = getTxtDateVal('ReceiveDateUTo');
        }

        var obj = [
            new clsSearch('OrderSupplierID', 'ReceiveSupplier', ctlTypeEnum.MultiSelect, self.objSearch.ReceiveSupplier),
            new clsSearch('OrderNumber', 'ReceivePO', ctlTypeEnum.MultiSelect, self.objSearch.ReceivePO),

            new clsSearch('POReceiveDate', 'POReceiveaDate', ctlTypeEnum.MultiSelect, self.objSearch.POReceiveaDate),
            new clsSearch('CreatedBy', 'ReceiveCreatedBy', ctlTypeEnum.MultiSelect, self.objSearch.ReceiveCreatedBy),
            new clsSearch('UpdatedBy', 'ReceiveUpdatedBy', ctlTypeEnum.MultiSelect, self.objSearch.ReceiveUpdatedBy),

            new clsSearch('DateCreatedFrom', 'ReceiveDateCFrom', ctlTypeEnum.FromDate, null, self.objSearch.ReceiveDateCFrom),
            new clsSearch('DateCreatedTo', 'ReceiveDateCTo', ctlTypeEnum.ToDate, null, self.objSearch.ReceiveDateCTo),
            new clsSearch('DateUpdatedFrom', 'ReceiveDateUFrom', ctlTypeEnum.FromDate, null, self.objSearch.ReceiveDateUFrom),
            new clsSearch('DateUpdatedTo', 'ReceiveDateUTo', ctlTypeEnum.ToDate, null, self.objSearch.ReceiveDateUTo),
        ];

        if (getUDFCtlById('UDF1').length > 0) {
            obj.push(new clsSearch('UDF1', 'UDF1', ctlTypeEnum.UDF, self.objSearch.UDF1));
        }
        if (getUDFCtlById('UDF2').length > 0) {
            obj.push(new clsSearch('UDF2', 'UDF2', ctlTypeEnum.UDF, self.objSearch.UDF2));
        }
        if (getUDFCtlById('UDF3').length > 0) {
            obj.push(new clsSearch('UDF3', 'UDF3', ctlTypeEnum.UDF, self.objSearch.UDF3));
        }
        if (getUDFCtlById('UDF4').length > 0) {
            obj.push(new clsSearch('UDF4', 'UDF4', ctlTypeEnum.UDF, self.objSearch.UDF4));
        }
        if (getUDFCtlById('UDF5').length > 0) {
            obj.push(new clsSearch('UDF5', 'UDF5', ctlTypeEnum.UDF, self.objSearch.UDF5));
        }
        if (getUDFCtlById('UDF6').length > 0) {
            obj.push(new clsSearch('UDF6', 'UDF6', ctlTypeEnum.UDF, self.objSearch.UDF6));
        }
        if (getUDFCtlById('UDF7').length > 0) {
            obj.push(new clsSearch('UDF7', 'UDF7', ctlTypeEnum.UDF, self.objSearch.UDF7));
        }
        if (getUDFCtlById('UDF8').length > 0) {
            obj.push(new clsSearch('UDF8', 'UDF8', ctlTypeEnum.UDF, self.objSearch.UDF8));
        }
        if (getUDFCtlById('UDF9').length > 0) {
            obj.push(new clsSearch('UDF9', 'UDF9', ctlTypeEnum.UDF, self.objSearch.UDF9));
        }
        if (getUDFCtlById('UDF10').length > 0) {
            obj.push(new clsSearch('UDF10', 'UDF10', ctlTypeEnum.UDF, self.objSearch.UDF10));
        }

        return obj;
    }

    var getOrderMasterSaveObj = function () {
        //var GlobalSearchVal = "";
        //GlobalSearchVal = $("#global_filter").val();

        _NarrowSearchSave.objSearch.DateCFrom = getTxtDateVal('OrdDateCFrom');
        _NarrowSearchSave.objSearch.DateCTo = getTxtDateVal('OrdDateCTo');

        _NarrowSearchSave.objSearch.DateUFrom = getTxtDateVal('OrdDateUFrom');
        _NarrowSearchSave.objSearch.DateUTo = getTxtDateVal('OrdDateUTo');

        var obj = [
            //new clsSearch('GlobalSearch', 'GlobalSearch', ctlTypeEnum.GlobalSearch, null, GlobalSearchVal),
            new clsSearch('SupplierID', 'OrderSupplier', ctlTypeEnum.MultiSelect, self.objSearch.OrderSupplier), // self.objSearch.StageLocationHeaderIM  SupplierBySelectedValues
            new clsSearch('OrderStatus', 'OrderStatus', ctlTypeEnum.MultiSelect, self.objSearch.OrderStatus),
            new clsSearch('OrderRequiredDate', 'OrderRequiredDate', ctlTypeEnum.MultiSelect, self.objSearch.OrderRequiredDate),
            new clsSearch('OrderShippingVendor', 'OrderShippingVendor', ctlTypeEnum.MultiSelect, self.objSearch.OrderShippingVendor),
            new clsSearch('OrderPackslipNumber', 'OrderPackslipNumber', ctlTypeEnum.MultiSelect, self.objSearch.OrderPackslipNumber),
            new clsSearch('OrderCreatedBy', 'OrderCreatedBy', ctlTypeEnum.MultiSelect, self.objSearch.UserCreated),
            new clsSearch('OrderUpdatedBy', 'OrderUpdatedBy', ctlTypeEnum.MultiSelect, self.objSearch.UserUpdated),
            new clsSearch('DateCreatedFrom', 'OrdDateCFrom', ctlTypeEnum.FromDate, null, self.objSearch.DateCFrom),
            new clsSearch('DateCreatedFrom', 'OrdDateCTo', ctlTypeEnum.ToDate, null, self.objSearch.DateCTo),
            new clsSearch('DateUpdatedFrom', 'OrdDateUFrom', ctlTypeEnum.FromDate, null, self.objSearch.DateUFrom),
            new clsSearch('DateUpdatedFrom', 'OrdDateUTo', ctlTypeEnum.ToDate, null, self.objSearch.DateUTo),
        ];
        if (typeof (OrderUDF1) != "undefined" && OrderUDF1 != null && OrderUDF1.length > 0) {
            obj.push(new clsSearch('OrderUDF1', 'OrderUDF1', ctlTypeEnum.UDF, self.objSearch.UDF1));
        }
        if (typeof (OrderUDF2) != "undefined" && OrderUDF2 != null && OrderUDF2.length > 0) {
            obj.push(new clsSearch('OrderUDF2', 'OrderUDF2', ctlTypeEnum.UDF, self.objSearch.UDF2));
        }
        if (typeof (OrderUDF3) != "undefined" && OrderUDF3 != null && OrderUDF3.length > 0) {
            obj.push(new clsSearch('OrderUDF3', 'OrderUDF3', ctlTypeEnum.UDF, self.objSearch.UDF3));
        }
        if (typeof (OrderUDF4) != "undefined" && OrderUDF4 != null && OrderUDF4.length > 0) {
            obj.push(new clsSearch('OrderUDF4', 'OrderUDF4', ctlTypeEnum.UDF, self.objSearch.UDF4));
        }
        if (typeof (OrderUDF5) != "undefined" && OrderUDF5 != null && OrderUDF5.length > 0) {
            obj.push(new clsSearch('OrderUDF5', 'OrderUDF5', ctlTypeEnum.UDF, self.objSearch.UDF5));
        }

        return obj;
    }

    var getReceiveItemWithoutOrderSaveObj = function () {
        _NarrowSearchSave.objSearch.DateCFromIM = getTxtDateVal('DateCFromIM');
        _NarrowSearchSave.objSearch.DateCToIM = getTxtDateVal('DateCToIM');

        _NarrowSearchSave.objSearch.DateUFromIM = getTxtDateVal('DateUFromIM');
        _NarrowSearchSave.objSearch.DateUToIM = getTxtDateVal('DateUToIM');

        var obj = [
            new clsSearch('SupplierID', 'PullSupplierIM', ctlTypeEnum.MultiSelect, self.objSearch.PullSupplierIM),
            new clsSearch('ManufacturerID', 'ManufacturerIM', ctlTypeEnum.MultiSelect, self.objSearch.ManufacturerIM),
            new clsSearch('CategoryID', 'PullCategoryIM', ctlTypeEnum.MultiSelect, self.objSearch.PullCategoryIM),
            new clsSearch('Cost', 'PullCostIM', ctlTypeEnum.Select, null, self.objSearch.PullCostIM),
            new clsSearch('SS', 'StockStatus', ctlTypeEnum.Select, null, self.objSearch.StockStatus),
            new clsSearch('AverageUsage', null, ctlTypeEnum.Select, null, self.objSearch.NarroSearchAvgUsage, 'NarroSearchAvgUsage'),
            new clsSearch('Turns', null, ctlTypeEnum.Select, null, self.objSearch.NarroSearchTurns, 'NarroSearchTurns'),
            new clsSearch('ItemType', 'ItemTypeIM', ctlTypeEnum.MultiSelect, self.objSearch.ItemTypeIM),

            new clsSearch('CreatedBy', 'UserCreatedIM', ctlTypeEnum.MultiSelect, self.objSearch.UserCreatedIM),
            new clsSearch('UpdatedBy', 'UserUpdatedIM', ctlTypeEnum.MultiSelect, self.objSearch.UserUpdatedIM),
            new clsSearch('DateCreatedFrom', 'DateCFromIM', ctlTypeEnum.FromDate, null, self.objSearch.DateCFromIM),
            new clsSearch('DateCreatedFrom', 'DateCToIM', ctlTypeEnum.ToDate, null, self.objSearch.DateCToIM),
            new clsSearch('DateUpdatedFrom', 'DateUFromIM', ctlTypeEnum.FromDate, null, self.objSearch.DateUFromIM),
            new clsSearch('DateUpdatedFrom', 'DateUToIM', ctlTypeEnum.ToDate, null, self.objSearch.DateUToIM)
        ];

        if (getUDFCtlById('UDF1').length > 0) {
            obj.push(new clsSearch('UDF1', 'UDF1', ctlTypeEnum.UDF, self.objSearch.UDF1));
        }
        if (getUDFCtlById('UDF2').length > 0) {
            obj.push(new clsSearch('UDF2', 'UDF2', ctlTypeEnum.UDF, self.objSearch.UDF2));
        }
        if (getUDFCtlById('UDF3').length > 0) {
            obj.push(new clsSearch('UDF3', 'UDF3', ctlTypeEnum.UDF, self.objSearch.UDF3));
        }
        if (getUDFCtlById('UDF4').length > 0) {
            obj.push(new clsSearch('UDF4', 'UDF4', ctlTypeEnum.UDF, self.objSearch.UDF4));
        }
        if (getUDFCtlById('UDF5').length > 0) {
            obj.push(new clsSearch('UDF5', 'UDF5', ctlTypeEnum.UDF, self.objSearch.UDF5));
        }
        if (getUDFCtlById('UDF6').length > 0) {
            obj.push(new clsSearch('UDF6', 'UDF6', ctlTypeEnum.UDF, self.objSearch.UDF6));
        }
        if (getUDFCtlById('UDF7').length > 0) {
            obj.push(new clsSearch('UDF7', 'UDF7', ctlTypeEnum.UDF, self.objSearch.UDF7));
        }
        if (getUDFCtlById('UDF8').length > 0) {
            obj.push(new clsSearch('UDF8', 'UDF8', ctlTypeEnum.UDF, self.objSearch.UDF8));
        }
        if (getUDFCtlById('UDF9').length > 0) {
            obj.push(new clsSearch('UDF9', 'UDF9', ctlTypeEnum.UDF, self.objSearch.UDF9));
        }
        if (getUDFCtlById('UDF10').length > 0) {
            obj.push(new clsSearch('UDF10', 'UDF10', ctlTypeEnum.UDF, self.objSearch.UDF10));
        }

        return obj;
    }

    var getRequisitionMasterSaveObj = function () {
        //var GlobalSearchVal = "";
        //GlobalSearchVal = $("#global_filter").val();

        _NarrowSearchSave.objSearch.DateCFrom = getTxtDateVal('DateCFrom');
        _NarrowSearchSave.objSearch.DateCTo = getTxtDateVal('DateCTo');

        _NarrowSearchSave.objSearch.DateUFrom = getTxtDateVal('DateUFrom');
        _NarrowSearchSave.objSearch.DateUTo = getTxtDateVal('DateUTo');

        var obj = [
            //new clsSearch('GlobalSearch', 'GlobalSearch', ctlTypeEnum.GlobalSearch, null, GlobalSearchVal),
            new clsSearch('OrderRequiredDate', 'OrderRequiredDate', ctlTypeEnum.MultiSelect, self.objSearch.OrderRequiredDate),
            new clsSearch('RequisitionSupplier', 'RequisitionSupplier', ctlTypeEnum.MultiSelect, self.objSearch.RequisitionSupplier),
            new clsSearch('ReqCustomer', 'ReqCustomer', ctlTypeEnum.MultiSelect, self.objSearch.ReqCustomer),
            new clsSearch('ReqWorkOrder', 'ReqWorkOrder', ctlTypeEnum.MultiSelect, self.objSearch.ReqWorkOrder),
            new clsSearch('ddlRequisitionStatus', 'ddlRequisitionStatus', ctlTypeEnum.MultiSelect, self.objSearch.ddlRequisitionStatus),

            new clsSearch('UserCreated', 'UserCreated', ctlTypeEnum.MultiSelect, self.objSearch.UserCreated),
            new clsSearch('UserUpdated', 'UserUpdated', ctlTypeEnum.MultiSelect, self.objSearch.UserUpdated),
            new clsSearch('DateCreatedFrom', 'DateCFrom', ctlTypeEnum.FromDate, null, self.objSearch.DateCFrom),
            new clsSearch('DateCreatedFrom', 'DateCTo', ctlTypeEnum.ToDate, null, self.objSearch.DateCTo),
            new clsSearch('DateUpdatedFrom', 'DateUFrom', ctlTypeEnum.FromDate, null, self.objSearch.DateUFrom),
            new clsSearch('DateUpdatedFrom', 'DateUTo', ctlTypeEnum.ToDate, null, self.objSearch.DateUTo)
        ];

        if (getUDFCtlById('UDF1').length > 0) {
            obj.push(new clsSearch('UDF1', 'UDF1', ctlTypeEnum.UDF, self.objSearch.UDF1));
        }
        if (getUDFCtlById('UDF2').length > 0) {
            obj.push(new clsSearch('UDF2', 'UDF2', ctlTypeEnum.UDF, self.objSearch.UDF2));
        }
        if (getUDFCtlById('UDF3').length > 0) {
            obj.push(new clsSearch('UDF3', 'UDF3', ctlTypeEnum.UDF, self.objSearch.UDF3));
        }
        if (getUDFCtlById('UDF4').length > 0) {
            obj.push(new clsSearch('UDF4', 'UDF4', ctlTypeEnum.UDF, self.objSearch.UDF4));
        }
        if (getUDFCtlById('UDF5').length > 0) {
            obj.push(new clsSearch('UDF5', 'UDF5', ctlTypeEnum.UDF, self.objSearch.UDF5));
        }
        return obj;
    }

    var getQuoteMasterSaveObj = function () {
        //var GlobalSearchVal = "";
        //GlobalSearchVal = $("#global_filter").val();

        _NarrowSearchSave.objSearch.DateCFrom = getTxtDateVal('DateCFrom');
        _NarrowSearchSave.objSearch.DateCTo = getTxtDateVal('DateCTo');

        _NarrowSearchSave.objSearch.DateUFrom = getTxtDateVal('DateUFrom');
        _NarrowSearchSave.objSearch.DateUTo = getTxtDateVal('DateUTo');

        var obj = [
            new clsSearch('QuoteStatus', 'QuoteStatus', ctlTypeEnum.MultiSelect, self.objSearch.QuoteStatus),
            new clsSearch('QuoteSupplier', 'QuoteSupplier', ctlTypeEnum.MultiSelect, self.objSearch.QuoteSupplier),
            new clsSearch('UserCreated', 'UserCreated', ctlTypeEnum.MultiSelect, self.objSearch.UserCreated),
            new clsSearch('UserUpdated', 'UserUpdated', ctlTypeEnum.MultiSelect, self.objSearch.UserUpdated),
            new clsSearch('DateCreatedFrom', 'DateCFrom', ctlTypeEnum.FromDate, null, self.objSearch.DateCFrom),
            new clsSearch('DateCreatedFrom', 'DateCTo', ctlTypeEnum.ToDate, null, self.objSearch.DateCTo),
            new clsSearch('DateUpdatedFrom', 'DateUFrom', ctlTypeEnum.FromDate, null, self.objSearch.DateUFrom),
            new clsSearch('DateUpdatedFrom', 'DateUTo', ctlTypeEnum.ToDate, null, self.objSearch.DateUTo)
        ];

        if (getUDFCtlById('UDF1').length > 0) {
            obj.push(new clsSearch('UDF1', 'UDF1', ctlTypeEnum.UDF, self.objSearch.UDF1));
        }
        if (getUDFCtlById('UDF2').length > 0) {
            obj.push(new clsSearch('UDF2', 'UDF2', ctlTypeEnum.UDF, self.objSearch.UDF2));
        }
        if (getUDFCtlById('UDF3').length > 0) {
            obj.push(new clsSearch('UDF3', 'UDF3', ctlTypeEnum.UDF, self.objSearch.UDF3));
        }
        if (getUDFCtlById('UDF4').length > 0) {
            obj.push(new clsSearch('UDF4', 'UDF4', ctlTypeEnum.UDF, self.objSearch.UDF4));
        }
        if (getUDFCtlById('UDF5').length > 0) {
            obj.push(new clsSearch('UDF5', 'UDF5', ctlTypeEnum.UDF, self.objSearch.UDF5));
        }
        return obj;
    }

    var getInventoryCountSaveObj = function () {
        //var GlobalSearchVal = "";
        //GlobalSearchVal = $("#global_filter").val();

        _NarrowSearchSave.objSearch.DateCFrom = getTxtDateVal('DateCFrom');
        _NarrowSearchSave.objSearch.DateCTo = getTxtDateVal('DateCTo');

        _NarrowSearchSave.objSearch.DateUFrom = getTxtDateVal('DateUFrom');
        _NarrowSearchSave.objSearch.DateUTo = getTxtDateVal('DateUTo');

        var obj = [
            //new clsSearch('GlobalSearch', 'GlobalSearch', ctlTypeEnum.GlobalSearch, null, GlobalSearchVal),
            new clsSearch('ICountType', 'ICountType', ctlTypeEnum.MultiSelect, self.objSearch.ICountType),
            new clsSearch('ICountStatus', 'ICountStatus', ctlTypeEnum.MultiSelect, self.objSearch.ICountStatus),

            new clsSearch('CreatedBy', 'UserCreated', ctlTypeEnum.MultiSelect, self.objSearch.UserCreated),
            new clsSearch('UpdatedBy', 'UserUpdated', ctlTypeEnum.MultiSelect, self.objSearch.UserUpdated),
            new clsSearch('DateCreatedFrom', 'DateCFrom', ctlTypeEnum.FromDate, null, self.objSearch.DateCFrom),
            new clsSearch('DateCreatedFrom', 'DateCTo', ctlTypeEnum.ToDate, null, self.objSearch.DateCTo),
            new clsSearch('DateUpdatedFrom', 'DateUFrom', ctlTypeEnum.FromDate, null, self.objSearch.DateUFrom),
            new clsSearch('DateUpdatedFrom', 'DateUTo', ctlTypeEnum.ToDate, null, self.objSearch.DateUTo)
        ];

        if (getUDFCtlById('UDF1').length > 0) {
            obj.push(new clsSearch('UDF1', 'UDF1', ctlTypeEnum.UDF, self.objSearch.UDF1));
        }
        if (getUDFCtlById('UDF2').length > 0) {
            obj.push(new clsSearch('UDF2', 'UDF2', ctlTypeEnum.UDF, self.objSearch.UDF2));
        }
        if (getUDFCtlById('UDF3').length > 0) {
            obj.push(new clsSearch('UDF3', 'UDF3', ctlTypeEnum.UDF, self.objSearch.UDF3));
        }
        if (getUDFCtlById('UDF4').length > 0) {
            obj.push(new clsSearch('UDF4', 'UDF4', ctlTypeEnum.UDF, self.objSearch.UDF4));
        }
        if (getUDFCtlById('UDF5').length > 0) {
            obj.push(new clsSearch('UDF5', 'UDF5', ctlTypeEnum.UDF, self.objSearch.UDF5));
        }
        return obj;
    }
    var getToolMasterSaveObj = function () {
        //var GlobalSearchVal = "";
        //GlobalSearchVal = $("#global_filter").val();

        _NarrowSearchSave.objSearch.DateCFrom = getTxtDateVal('DateCFrom');
        _NarrowSearchSave.objSearch.DateCTo = getTxtDateVal('DateCTo');

        _NarrowSearchSave.objSearch.DateUFrom = getTxtDateVal('DateUFrom');
        _NarrowSearchSave.objSearch.DateUTo = getTxtDateVal('DateUTo');

        var obj = [
            //new clsSearch('GlobalSearch', 'GlobalSearch', ctlTypeEnum.GlobalSearch, null, GlobalSearchVal),
            new clsSearch('ToolsCategory', 'ToolsCategory', ctlTypeEnum.MultiSelect, self.objSearch.ToolsCategory),
            new clsSearch('ToolsCost', 'ToolsCost', ctlTypeEnum.Select, null, self.objSearch.ToolsCost),
            new clsSearch('ToolCheckout', 'ToolCheckout', ctlTypeEnum.MultiSelect, self.objSearch.ToolCheckout),
            new clsSearch('ToolsTechnician', 'ToolsTechnician', ctlTypeEnum.MultiSelect, self.objSearch.ToolsTechnician),
            new clsSearch('ToolsLocation', 'ToolsLocation', ctlTypeEnum.MultiSelect, self.objSearch.ToolsLocation),

            new clsSearch('CreatedBy', 'UserCreated', ctlTypeEnum.MultiSelect, self.objSearch.UserCreated),
            new clsSearch('UpdatedBy', 'UserUpdated', ctlTypeEnum.MultiSelect, self.objSearch.UserUpdated),
            new clsSearch('DateCreatedFrom', 'DateCFrom', ctlTypeEnum.FromDate, null, self.objSearch.DateCFrom),
            new clsSearch('DateCreatedFrom', 'DateCTo', ctlTypeEnum.ToDate, null, self.objSearch.DateCTo),
            new clsSearch('DateUpdatedFrom', 'DateUFrom', ctlTypeEnum.FromDate, null, self.objSearch.DateUFrom),
            new clsSearch('DateUpdatedFrom', 'DateUTo', ctlTypeEnum.ToDate, null, self.objSearch.DateUTo)
        ];

        if (getUDFCtlById('UDF1').length > 0) {
            obj.push(new clsSearch('UDF1', 'UDF1', ctlTypeEnum.UDF, self.objSearch.UDF1));
        }
        if (getUDFCtlById('UDF2').length > 0) {
            obj.push(new clsSearch('UDF2', 'UDF2', ctlTypeEnum.UDF, self.objSearch.UDF2));
        }
        if (getUDFCtlById('UDF3').length > 0) {
            obj.push(new clsSearch('UDF3', 'UDF3', ctlTypeEnum.UDF, self.objSearch.UDF3));
        }
        if (getUDFCtlById('UDF4').length > 0) {
            obj.push(new clsSearch('UDF4', 'UDF4', ctlTypeEnum.UDF, self.objSearch.UDF4));
        }
        if (getUDFCtlById('UDF5').length > 0) {
            obj.push(new clsSearch('UDF5', 'UDF5', ctlTypeEnum.UDF, self.objSearch.UDF5));
        }
        if (getUDFCtlById('UDF6').length > 0) {
            obj.push(new clsSearch('UDF6', 'UDF6', ctlTypeEnum.UDF, self.objSearch.UDF6));
        }
        if (getUDFCtlById('UDF7').length > 0) {
            obj.push(new clsSearch('UDF7', 'UDF7', ctlTypeEnum.UDF, self.objSearch.UDF7));
        }
        if (getUDFCtlById('UDF8').length > 0) {
            obj.push(new clsSearch('UDF8', 'UDF8', ctlTypeEnum.UDF, self.objSearch.UDF8));
        }
        if (getUDFCtlById('UDF9').length > 0) {
            obj.push(new clsSearch('UDF9', 'UDF9', ctlTypeEnum.UDF, self.objSearch.UDF9));
        }
        if (getUDFCtlById('UDF10').length > 0) {
            obj.push(new clsSearch('UDF10', 'UDF10', ctlTypeEnum.UDF, self.objSearch.UDF10));
        }
        return obj;
    }
    var getToolHistorySaveObj = function () {
        //var GlobalSearchVal = "";
        //GlobalSearchVal = $("#global_filter").val();

        _NarrowSearchSave.objSearch.DateCFrom = getTxtDateVal('ToolHistoryDateCFrom');
        _NarrowSearchSave.objSearch.DateCTo = getTxtDateVal('ToolHistoryDateCTo');

        _NarrowSearchSave.objSearch.DateUFrom = getTxtDateVal('ToolHistoryDateUFrom');
        _NarrowSearchSave.objSearch.DateUTo = getTxtDateVal('ToolHistoryDateUTo');

        var obj = [
            //new clsSearch('GlobalSearch', 'GlobalSearch', ctlTypeEnum.GlobalSearch, null, GlobalSearchVal),
            new clsSearch('ToolHistoryCategory', 'ToolHistoryCategory', ctlTypeEnum.MultiSelect, self.objSearch.ToolHistoryCategory),
            new clsSearch('ToolHistoryCost', 'ToolHistoryCost', ctlTypeEnum.Select, null, self.objSearch.ToolHistoryCost),
            new clsSearch('ToolHistoryCheckout', 'ToolHistoryCheckout', ctlTypeEnum.MultiSelect, self.objSearch.ToolHistoryCheckout),
            new clsSearch('ToolHistoryTechnician', 'ToolHistoryTechnician', ctlTypeEnum.MultiSelect, self.objSearch.ToolHistoryTechnician),
            new clsSearch('ToolHistoryLocation', 'ToolHistoryLocation', ctlTypeEnum.MultiSelect, self.objSearch.ToolHistoryLocation),

            new clsSearch('ToolHistoryCreatedBy', 'ToolHistoryCreatedBy', ctlTypeEnum.MultiSelect, self.objSearch.ToolHistoryCreatedBy),
            new clsSearch('ToolHistoryUpdatedBy', 'ToolHistoryUpdatedBy', ctlTypeEnum.MultiSelect, self.objSearch.ToolHistoryUpdatedBy),
            new clsSearch('DateCreatedFrom', 'ToolHistoryDateCFrom', ctlTypeEnum.FromDate, null, self.objSearch.DateCFrom),
            new clsSearch('DateCreatedFrom', 'ToolHistoryDateCTo', ctlTypeEnum.ToDate, null, self.objSearch.DateCTo),
            new clsSearch('DateUpdatedFrom', 'ToolHistoryDateUFrom', ctlTypeEnum.FromDate, null, self.objSearch.DateUFrom),
            new clsSearch('DateUpdatedFrom', 'ToolHistoryDateUTo', ctlTypeEnum.ToDate, null, self.objSearch.DateUTo)
        ];

        if (getUDFCtlById('UDF1').length > 0) {
            obj.push(new clsSearch('UDF1', 'UDF1', ctlTypeEnum.UDF, self.objSearch.UDF1));
        }
        if (getUDFCtlById('UDF2').length > 0) {
            obj.push(new clsSearch('UDF2', 'UDF2', ctlTypeEnum.UDF, self.objSearch.UDF2));
        }
        if (getUDFCtlById('UDF3').length > 0) {
            obj.push(new clsSearch('UDF3', 'UDF3', ctlTypeEnum.UDF, self.objSearch.UDF3));
        }
        if (getUDFCtlById('UDF4').length > 0) {
            obj.push(new clsSearch('UDF4', 'UDF4', ctlTypeEnum.UDF, self.objSearch.UDF4));
        }
        if (getUDFCtlById('UDF5').length > 0) {
            obj.push(new clsSearch('UDF5', 'UDF5', ctlTypeEnum.UDF, self.objSearch.UDF5));
        }
        if (getUDFCtlById('UDF6').length > 0) {
            obj.push(new clsSearch('UDF6', 'UDF6', ctlTypeEnum.UDF, self.objSearch.UDF6));
        }
        if (getUDFCtlById('UDF7').length > 0) {
            obj.push(new clsSearch('UDF7', 'UDF7', ctlTypeEnum.UDF, self.objSearch.UDF7));
        }
        if (getUDFCtlById('UDF8').length > 0) {
            obj.push(new clsSearch('UDF8', 'UDF8', ctlTypeEnum.UDF, self.objSearch.UDF8));
        }
        if (getUDFCtlById('UDF9').length > 0) {
            obj.push(new clsSearch('UDF9', 'UDF9', ctlTypeEnum.UDF, self.objSearch.UDF9));
        }
        if (getUDFCtlById('UDF10').length > 0) {
            obj.push(new clsSearch('UDF10', 'UDF10', ctlTypeEnum.UDF, self.objSearch.UDF10));
        }
        return obj;
    }
    var getWrittenOffToolListSaveObj = function () {
        //var GlobalSearchVal = "";
        //GlobalSearchVal = $("#global_filter").val();

        _NarrowSearchSave.objSearch.DateCFrom = getTxtDateVal('ToolWrittenOffDateCFrom');
        _NarrowSearchSave.objSearch.DateCTo = getTxtDateVal('ToolWrittenOffDateCTo');

        _NarrowSearchSave.objSearch.DateUFrom = getTxtDateVal('ToolWrittenOffDateUFrom');
        _NarrowSearchSave.objSearch.DateUTo = getTxtDateVal('ToolWrittenOffDateUTo');

        var obj = [
            //new clsSearch('GlobalSearch', 'GlobalSearch', ctlTypeEnum.GlobalSearch, null, GlobalSearchVal),
            new clsSearch('ToolWrittenOffCategory', 'ToolWrittenOffCategory', ctlTypeEnum.MultiSelect, self.objSearch.ToolWrittenOffCategory),

            new clsSearch('ToolWrittenOffCreatedBy', 'ToolWrittenOffCreatedBy', ctlTypeEnum.MultiSelect, self.objSearch.ToolWrittenOffCreatedBy),
            new clsSearch('ToolWrittenOffUpdatedBy', 'ToolWrittenOffUpdatedBy', ctlTypeEnum.MultiSelect, self.objSearch.ToolWrittenOffUpdatedBy),
            new clsSearch('DateCreatedFrom', 'ToolWrittenOffDateCFrom', ctlTypeEnum.FromDate, null, self.objSearch.DateCFrom),
            new clsSearch('DateCreatedFrom', 'ToolWrittenOffDateCTo', ctlTypeEnum.ToDate, null, self.objSearch.DateCTo),
            new clsSearch('DateUpdatedFrom', 'ToolWrittenOffDateUFrom', ctlTypeEnum.FromDate, null, self.objSearch.DateUFrom),
            new clsSearch('DateUpdatedFrom', 'ToolWrittenOffDateUTo', ctlTypeEnum.ToDate, null, self.objSearch.DateUTo)
        ];

        return obj;
    }
    var getAssetMasterSaveObj = function () {
        //var GlobalSearchVal = "";
        //GlobalSearchVal = $("#global_filter").val();

        _NarrowSearchSave.objSearch.DateCFrom = getTxtDateVal('DateCFrom');
        _NarrowSearchSave.objSearch.DateCTo = getTxtDateVal('DateCTo');

        _NarrowSearchSave.objSearch.DateUFrom = getTxtDateVal('DateUFrom');
        _NarrowSearchSave.objSearch.DateUTo = getTxtDateVal('DateUTo');

        var obj = [
            //new clsSearch('GlobalSearch', 'GlobalSearch', ctlTypeEnum.GlobalSearch, null, GlobalSearchVal),
            new clsSearch('AssetsCategory', 'AssetsCategory', ctlTypeEnum.MultiSelect, self.objSearch.AssetsCategory),

            new clsSearch('UserCreated', 'UserCreated', ctlTypeEnum.MultiSelect, self.objSearch.UserCreated),
            new clsSearch('UserUpdated', 'UserUpdated', ctlTypeEnum.MultiSelect, self.objSearch.UserUpdated),
            new clsSearch('DateCreatedFrom', 'DateCFrom', ctlTypeEnum.FromDate, null, self.objSearch.DateCFrom),
            new clsSearch('DateCreatedFrom', 'DateCTo', ctlTypeEnum.ToDate, null, self.objSearch.DateCTo),
            new clsSearch('DateUpdatedFrom', 'DateUFrom', ctlTypeEnum.FromDate, null, self.objSearch.DateUFrom),
            new clsSearch('DateUpdatedFrom', 'DateUTo', ctlTypeEnum.ToDate, null, self.objSearch.DateUTo)
        ];
        if (getUDFCtlById('UDF1').length > 0) {
            obj.push(new clsSearch('UDF1', 'UDF1', ctlTypeEnum.UDF, self.objSearch.UDF1));
        }
        if (getUDFCtlById('UDF2').length > 0) {
            obj.push(new clsSearch('UDF2', 'UDF2', ctlTypeEnum.UDF, self.objSearch.UDF2));
        }
        if (getUDFCtlById('UDF3').length > 0) {
            obj.push(new clsSearch('UDF3', 'UDF3', ctlTypeEnum.UDF, self.objSearch.UDF3));
        }
        if (getUDFCtlById('UDF4').length > 0) {
            obj.push(new clsSearch('UDF4', 'UDF4', ctlTypeEnum.UDF, self.objSearch.UDF4));
        }
        if (getUDFCtlById('UDF5').length > 0) {
            obj.push(new clsSearch('UDF5', 'UDF5', ctlTypeEnum.UDF, self.objSearch.UDF5));
        }
        if (getUDFCtlById('UDF6').length > 0) {
            obj.push(new clsSearch('UDF6', 'UDF6', ctlTypeEnum.UDF, self.objSearch.UDF6));
        }
        if (getUDFCtlById('UDF7').length > 0) {
            obj.push(new clsSearch('UDF7', 'UDF7', ctlTypeEnum.UDF, self.objSearch.UDF7));
        }
        if (getUDFCtlById('UDF8').length > 0) {
            obj.push(new clsSearch('UDF8', 'UDF8', ctlTypeEnum.UDF, self.objSearch.UDF8));
        }
        if (getUDFCtlById('UDF9').length > 0) {
            obj.push(new clsSearch('UDF9', 'UDF9', ctlTypeEnum.UDF, self.objSearch.UDF9));
        }
        if (getUDFCtlById('UDF10').length > 0) {
            obj.push(new clsSearch('UDF10', 'UDF10', ctlTypeEnum.UDF, self.objSearch.UDF10));
        }
        return obj;
    }

    var getToolMasterNewSaveObj = function () {
        //var GlobalSearchVal = "";
        //GlobalSearchVal = $("#global_filter").val();

        _NarrowSearchSave.objSearch.DateCFrom = getTxtDateVal('DateCFrom');
        _NarrowSearchSave.objSearch.DateCTo = getTxtDateVal('DateCTo');

        _NarrowSearchSave.objSearch.DateUFrom = getTxtDateVal('DateUFrom');
        _NarrowSearchSave.objSearch.DateUTo = getTxtDateVal('DateUTo');

        var obj = [
            //new clsSearch('GlobalSearch', 'GlobalSearch', ctlTypeEnum.GlobalSearch, null, GlobalSearchVal),
            new clsSearch('ToolsCategoryNew', 'ToolsCategoryNew', ctlTypeEnum.MultiSelect, self.objSearch.ToolsCategoryNew),
            new clsSearch('ToolsCostNew', 'ToolsCostNew', ctlTypeEnum.Select, null, self.objSearch.ToolsCostNew),
            new clsSearch('ToolCheckoutNew', 'ToolCheckoutNew', ctlTypeEnum.MultiSelect, self.objSearch.ToolCheckoutNew),
            new clsSearch('ToolsTechnicianNew', 'ToolsTechnicianNew', ctlTypeEnum.MultiSelect, self.objSearch.ToolsTechnicianNew),
            new clsSearch('ToolsLocationNew', 'ToolsLocationNew', ctlTypeEnum.MultiSelect, self.objSearch.ToolsLocationNew),

            new clsSearch('UserCreated', 'UserCreated', ctlTypeEnum.MultiSelect, self.objSearch.UserCreated),
            new clsSearch('UserUpdated', 'UserUpdated', ctlTypeEnum.MultiSelect, self.objSearch.UserUpdated),
            new clsSearch('DateCreatedFrom', 'DateCFrom', ctlTypeEnum.FromDate, null, self.objSearch.DateCFrom),
            new clsSearch('DateCreatedFrom', 'DateCTo', ctlTypeEnum.ToDate, null, self.objSearch.DateCTo),
            new clsSearch('DateUpdatedFrom', 'DateUFrom', ctlTypeEnum.FromDate, null, self.objSearch.DateUFrom),
            new clsSearch('DateUpdatedFrom', 'DateUTo', ctlTypeEnum.ToDate, null, self.objSearch.DateUTo)
        ];
        if (getUDFCtlById('UDF1').length > 0) {
            obj.push(new clsSearch('UDF1', 'UDF1', ctlTypeEnum.UDF, self.objSearch.UDF1));
        }
        if (getUDFCtlById('UDF2').length > 0) {
            obj.push(new clsSearch('UDF2', 'UDF2', ctlTypeEnum.UDF, self.objSearch.UDF2));
        }
        if (getUDFCtlById('UDF3').length > 0) {
            obj.push(new clsSearch('UDF3', 'UDF3', ctlTypeEnum.UDF, self.objSearch.UDF3));
        }
        if (getUDFCtlById('UDF4').length > 0) {
            obj.push(new clsSearch('UDF4', 'UDF4', ctlTypeEnum.UDF, self.objSearch.UDF4));
        }
        if (getUDFCtlById('UDF5').length > 0) {
            obj.push(new clsSearch('UDF5', 'UDF5', ctlTypeEnum.UDF, self.objSearch.UDF5));
        }
        if (getUDFCtlById('UDF6').length > 0) {
            obj.push(new clsSearch('UDF6', 'UDF6', ctlTypeEnum.UDF, self.objSearch.UDF6));
        }
        if (getUDFCtlById('UDF7').length > 0) {
            obj.push(new clsSearch('UDF7', 'UDF7', ctlTypeEnum.UDF, self.objSearch.UDF7));
        }
        if (getUDFCtlById('UDF8').length > 0) {
            obj.push(new clsSearch('UDF8', 'UDF8', ctlTypeEnum.UDF, self.objSearch.UDF8));
        }
        if (getUDFCtlById('UDF9').length > 0) {
            obj.push(new clsSearch('UDF9', 'UDF9', ctlTypeEnum.UDF, self.objSearch.UDF9));
        }
        if (getUDFCtlById('UDF10').length > 0) {
            obj.push(new clsSearch('UDF10', 'UDF10', ctlTypeEnum.UDF, self.objSearch.UDF10));
        }
        return obj;
    }
    var getToolHistoryNewSaveObj = function () {
        //var GlobalSearchVal = "";
        //GlobalSearchVal = $("#global_filter").val();

        _NarrowSearchSave.objSearch.DateCFrom = getTxtDateVal('ToolHistoryDateCFrom');
        _NarrowSearchSave.objSearch.DateCTo = getTxtDateVal('ToolHistoryDateCTo');

        _NarrowSearchSave.objSearch.DateUFrom = getTxtDateVal('ToolHistoryDateUFrom');
        _NarrowSearchSave.objSearch.DateUTo = getTxtDateVal('ToolHistoryDateUTo');

        var obj = [
            //new clsSearch('GlobalSearch', 'GlobalSearch', ctlTypeEnum.GlobalSearch, null, GlobalSearchVal),
            new clsSearch('ToolHistoryCategoryNew', 'ToolHistoryCategoryNew', ctlTypeEnum.MultiSelect, self.objSearch.ToolHistoryCategoryNew),
            new clsSearch('ToolHistoryCost', 'ToolHistoryCost', ctlTypeEnum.Select, null, self.objSearch.ToolHistoryCost),
            new clsSearch('ToolHistoryCheckoutNew', 'ToolHistoryCheckoutNew', ctlTypeEnum.MultiSelect, self.objSearch.ToolHistoryCheckoutNew),
            new clsSearch('ToolHistoryTechnicianNew', 'ToolHistoryTechnicianNew', ctlTypeEnum.MultiSelect, self.objSearch.ToolHistoryTechnicianNew),
            new clsSearch('ToolHistoryLocationNew', 'ToolHistoryLocationNew', ctlTypeEnum.MultiSelect, self.objSearch.ToolHistoryLocationNew),

            new clsSearch('ToolHistoryCreatedByNew', 'ToolHistoryCreatedByNew', ctlTypeEnum.MultiSelect, self.objSearch.ToolHistoryCreatedByNew),
            new clsSearch('ToolHistoryUpdatedByNew', 'ToolHistoryUpdatedByNew', ctlTypeEnum.MultiSelect, self.objSearch.ToolHistoryUpdatedByNew),
            new clsSearch('DateCreatedFrom', 'ToolHistoryDateCFrom', ctlTypeEnum.FromDate, null, self.objSearch.DateCFrom),
            new clsSearch('DateCreatedFrom', 'ToolHistoryDateCTo', ctlTypeEnum.ToDate, null, self.objSearch.DateCTo),
            new clsSearch('DateUpdatedFrom', 'ToolHistoryDateUFrom', ctlTypeEnum.FromDate, null, self.objSearch.DateUFrom),
            new clsSearch('DateUpdatedFrom', 'ToolHistoryDateUTo', ctlTypeEnum.ToDate, null, self.objSearch.DateUTo)
        ];

        if (getUDFCtlById('UDF1').length > 0) {
            obj.push(new clsSearch('UDF1', 'UDF1', ctlTypeEnum.UDF, self.objSearch.UDF1));
        }
        if (getUDFCtlById('UDF2').length > 0) {
            obj.push(new clsSearch('UDF2', 'UDF2', ctlTypeEnum.UDF, self.objSearch.UDF2));
        }
        if (getUDFCtlById('UDF3').length > 0) {
            obj.push(new clsSearch('UDF3', 'UDF3', ctlTypeEnum.UDF, self.objSearch.UDF3));
        }
        if (getUDFCtlById('UDF4').length > 0) {
            obj.push(new clsSearch('UDF4', 'UDF4', ctlTypeEnum.UDF, self.objSearch.UDF4));
        }
        if (getUDFCtlById('UDF5').length > 0) {
            obj.push(new clsSearch('UDF5', 'UDF5', ctlTypeEnum.UDF, self.objSearch.UDF5));
        }
        if (getUDFCtlById('UDF6').length > 0) {
            obj.push(new clsSearch('UDF6', 'UDF6', ctlTypeEnum.UDF, self.objSearch.UDF6));
        }
        if (getUDFCtlById('UDF7').length > 0) {
            obj.push(new clsSearch('UDF7', 'UDF7', ctlTypeEnum.UDF, self.objSearch.UDF7));
        }
        if (getUDFCtlById('UDF8').length > 0) {
            obj.push(new clsSearch('UDF8', 'UDF8', ctlTypeEnum.UDF, self.objSearch.UDF8));
        }
        if (getUDFCtlById('UDF9').length > 0) {
            obj.push(new clsSearch('UDF9', 'UDF9', ctlTypeEnum.UDF, self.objSearch.UDF9));
        }
        if (getUDFCtlById('UDF10').length > 0) {
            obj.push(new clsSearch('UDF10', 'UDF10', ctlTypeEnum.UDF, self.objSearch.UDF10));
        }
        return obj;
    }

    /**
     * 
     * */
    function clsSearch(field, ctlId, ctlType, arrVal, val, ctlName) {
        var self = this;
        self.field = field;
        self.ctlId = ctlId;
        self.ctlType = ctlType;
        self.val = val;
        self.arrVal = arrVal;
        self.ctlName = ctlName;
        return self;
    }

    return self;

})(jQuery, _notification, _datePickerWrapper, _utils);