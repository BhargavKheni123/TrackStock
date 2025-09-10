
 var  oTableItemModel = null,
    objColumns = {};
var oTable = null,
    ProjectSpendClass = "read_only",
    ProjectSpendVisible = false,
    isDeleteSrLotRow = false,
    vWorkOrderDetailGUID = '',
    UDF1Index = '',
    UDF2Index = '',
    UDF3Index = '',
    UDF4Index = '',
    UDF5Index = '';
var ProjectSpendNameSCR = '',
    ProjectSpendGuidSCR = '';



var lastCheckedNewPull;
var IsQLLoaded = false;

var _NewConsumePull = (function ($) {
    var self = {};
    var tmpZeroCostQtyFormatted2 = FormatedCostQtyValues(0, 2);
    var tmpZeroCostQtyFormatted1 = FormatedCostQtyValues(0, 1);
    var hdnSuppPOPairVal = {
        val: null,
        json: null,
        clear: function () {
            this.val = null;
            this.json = null;
        }
    };

    self.isSaveGridState = false;

    self.gridId = "ItemModeDataTable";
    self.gridUseThisId = "tblPullCommonUDF";

    self.urls = {
        PullLotSrSelectionUrl: null,
        PullItemQuantityUrl: null,
        ValidateSerialLotNumberUrl: null,
        PullSerialsAndLotsUrl: null,
        PullSerialsAndLotsNewUrl: null,
        ValidateSerialNumberForCreditUrl: null,
        ValidateLotDateCodeForCreditUrl: null,
        PullLotSrSelectionForCreditPopupUrl: null,
        SaveMaterialStagingFromCreditUrl: null,
        SaveGridStateUrl: null,
        LoadGridStateUrl: null,
        AjaxURLToFillItemGridUrl: null,
        PullMasterListUrl: null,
        NewPullUrl: null,
        RequisitionListUrl: null,
        CheckValidPullDataUrl: null,
        UpdatePullDataUrl: null,
        SavePullGuidsInScheduleUrl: null,
        ItemLocationDetailsSaveForCreditPullUrl: null,
        GetItemByQLGuidUrl: null
    };

    self.initUrls = function (PullLotSrSelectionUrl, PullItemQuantityUrl, ValidateSerialLotNumberUrl
        , PullSerialsAndLotsUrl, PullSerialsAndLotsNewUrl, ValidateSerialNumberForCreditUrl,
        ValidateLotDateCodeForCreditUrl, PullLotSrSelectionForCreditPopupUrl,
        SaveMaterialStagingFromCreditUrl, SaveGridStateUrl, LoadGridStateUrl
        , AjaxURLToFillItemGridUrl, PullMasterListUrl, NewPullUrl, RequisitionListUrl
        , CheckValidPullDataUrl, UpdatePullDataUrl, SavePullGuidsInScheduleUrl
        , ItemLocationDetailsSaveForCreditPullUrl, GetItemByQLGuidUrl
    ) {
        self.urls.PullLotSrSelectionUrl = PullLotSrSelectionUrl;
        self.urls.PullItemQuantityUrl = PullItemQuantityUrl;
        self.urls.ValidateSerialLotNumberUrl = ValidateSerialLotNumberUrl;
        self.urls.PullSerialsAndLotsUrl = PullSerialsAndLotsUrl;
        self.urls.PullSerialsAndLotsNewUrl = PullSerialsAndLotsNewUrl;
        self.urls.ValidateSerialNumberForCreditUrl = ValidateSerialNumberForCreditUrl;
        self.urls.ValidateLotDateCodeForCreditUrl = ValidateLotDateCodeForCreditUrl;
        self.urls.PullLotSrSelectionForCreditPopupUrl = PullLotSrSelectionForCreditPopupUrl;
        self.urls.SaveMaterialStagingFromCreditUrl = SaveMaterialStagingFromCreditUrl;
        self.urls.SaveGridStateUrl = SaveGridStateUrl;
        self.urls.LoadGridStateUrl = LoadGridStateUrl;
        self.urls.AjaxURLToFillItemGridUrl = AjaxURLToFillItemGridUrl;
        self.urls.PullMasterListUrl = PullMasterListUrl;
        self.urls.NewPullUrl = NewPullUrl;
        self.urls.RequisitionListUrl = RequisitionListUrl;
        self.urls.CheckValidPullDataUrl = CheckValidPullDataUrl;
        self.urls.UpdatePullDataUrl = UpdatePullDataUrl;
        self.urls.SavePullGuidsInScheduleUrl = SavePullGuidsInScheduleUrl;
        self.urls.ItemLocationDetailsSaveForCreditPullUrl = ItemLocationDetailsSaveForCreditPullUrl;
        self.urls.GetItemByQLGuidUrl = GetItemByQLGuidUrl;

    };

    self.init = function () {
        self.initEvents();
        self.initDataTable();
    };
    self.gridRowStartIndex = null;
    self.initDataTable = function () {
        var PullItemListColumnsArr = new Array();

        PullItemListColumnsArr.push({ mDataProp: null, sClass: "read_only center NotHide RowNo", "bSortable": false, sDefaultContent: '' });
        PullItemListColumnsArr.push({
            "mDataProp": "sDispImg",//null,
            "sClass": "read_only control center NotHide",
            "bSortable": false,
            "sDefaultContent": '<img id="imgQty" src="' + sImageUrl + 'drildown_open.jpg' + '">'
        //    , "fnRender": function (obj, val) {
        //        var $NewPullAction = $('#NewPullAction');
                
        //        var dataAttr = "";

        //        if (obj.aData.QuickListGUID == null || obj.aData.QuickListGUID == '') {                    

        //            dataAttr += " data-spnItemID='" + obj.aData.ID + "'";
        //            dataAttr += " data-spnItemGUID='" + obj.aData.GUID + "'";
        //            dataAttr += " data-spnOn_HandQuantity='" + obj.aData.OnHandQuantity + "'";
        //            dataAttr += " data-spnOrderItemType='" + obj.aData.ItemType + "'";
        //            dataAttr += " data-itemWisePONumber='" + obj.aData.ItemBlanketPO + "'";

        //            if ($NewPullAction.val() == "Pull") {
        //                if (viewNewPullbuttons == 'no') {

        //                    //return "<input type='button' value='Pull' onclick='return AddSingleItemToPullList(this);' id='btnAdd' class='CreateBtn pullbutton' style='float: none;padding: 2px 6px;margin-left: 5px;' />" + "<span id='spnItemID' style='display:none'>" + obj.aData.ID + "</span>" + "<span id='spnItemGUID' style='display:none'>" + obj.aData.GUID + "</span><span id='spnOn_HandQuantity' style='display:none'>" + obj.aData.OnHandQuantity + "</span>" + "<span id='spnOrderItemType'  style='display:none'>" + obj.aData.ItemType + "</span>" + "<input type='hidden' id='itemWisePONumber' value='" + obj.aData.ItemBlanketPO + "' />";
        //                    return "<input type='button' value='Pull' onclick='return AddSingleItemToPullList(this);' id='btnAdd' class='CreateBtn pullbutton' style='float: none;padding: 2px 6px;margin-left: 5px;' " + dataAttr + " />";
        //                }
        //                else {
        //                    if (obj.aData.SerialNumberTracking == true || obj.aData.LotNumberTracking == true || obj.aData.DateCodeTracking == true) {
        //                        //return "<input type='button' value='Pull' onclick='return OpenPullPopup(this);' id='btnAdd' class='CreateBtn pullbutton' style='float: none;padding: 2px 6px;margin-left: 5px;' />" + "<span id='spnItemID' style='display:none'>" + obj.aData.ID + "</span>" + "<span id='spnItemGUID' style='display:none'>" + obj.aData.GUID + "</span><span id='spnOn_HandQuantity' style='display:none'>" + obj.aData.OnHandQuantity + "</span>" + "<span id='spnOrderItemType'  style='display:none'>" + obj.aData.ItemType + "</span>" + "<input type='hidden' id='itemWisePONumber' value='" + obj.aData.ItemBlanketPO + "' />";
        //                        return "<input type='button' value='Pull' onclick='return OpenPullPopup(this);' id='btnAdd' class='CreateBtn pullbutton' style='float: none;padding: 2px 6px;margin-left: 5px;' " + dataAttr + " />";
        //                    }
        //                    else {
        //                        //return "<input type='button' value='Pull' onclick='return AddSingleItemToPullList(this);' id='btnAdd' class='CreateBtn pullbutton' style='float: none;padding: 2px 6px;margin-left: 5px;' />" + "<span id='spnItemID' style='display:none'>" + obj.aData.ID + "</span>" + "<span id='spnItemGUID' style='display:none'>" + obj.aData.GUID + "</span><span id='spnOn_HandQuantity' style='display:none'>" + obj.aData.OnHandQuantity + "</span>" + "<span id='spnOrderItemType'  style='display:none'>" + obj.aData.ItemType + "</span>" + "<input type='hidden' id='itemWisePONumber' value='" + obj.aData.ItemBlanketPO + "' />";
        //                        return "<input type='button' value='Pull' onclick='return AddSingleItemToPullList(this);' id='btnAdd' class='CreateBtn pullbutton' style='float: none;padding: 2px 6px;margin-left: 5px;' " + dataAttr + " />" ;
        //                    }
        //                }
        //            }
        //            else if ($NewPullAction.val() == "Credit") {
        //                //return "<input type='button' value='Credit' onclick='return CreditItems(this);' id='btnAdd' class='CreateBtn pullbutton' style='float: none;padding: 2px 6px;margin-left: 5px;' />" + "<span id='spnItemID' style='display:none'>" + obj.aData.ID + "</span>" + "<span id='spnItemGUID' style='display:none'>" + obj.aData.GUID + "</span><span id='spnOn_HandQuantity' style='display:none'>" + obj.aData.OnHandQuantity + "</span>" + "<span id='spnOrderItemType'  style='display:none'>" + obj.aData.ItemType + "</span>" + "<input type='hidden' id='itemWisePONumber' value='" + obj.aData.ItemBlanketPO + "' />" + "<span id='spnIsIgnoreCreditRule'  style='display:none'>" + serIsIgnoreCreditRule + "</span>" + "<span id='spnItemNumber' style='display:none'>" + obj.aData.ItemNumber + "</span>";                        
        //                dataAttr += " data-spnIsIgnoreCreditRule='" + serIsIgnoreCreditRule + "'";
        //                dataAttr += " data-spnItemNumber='" + obj.aData.ItemNumber + "'";
        //                return "<input type='button' value='Credit' onclick='return CreditItems(this);' id='btnAdd' class='CreateBtn pullbutton' style='float: none;padding: 2px 6px;margin-left: 5px;' " + dataAttr + " />";
        //            }
        //            else if ($NewPullAction.val() == "CreditMS") {
        //                //return "<input type='button' value='Credit MS'  onclick='return MsCreditItems(this);' id='btnAdd' class='CreateBtn pullbutton' style='float: none;padding: 2px 6px;margin-left: 5px;' />" + "<span id='spnItemID' style='display:none'>" + obj.aData.ID + "</span>" + "<span id='spnItemGUID' style='display:none'>" + obj.aData.GUID + "</span><span id='spnOn_HandQuantity' style='display:none'>" + obj.aData.OnHandQuantity + "</span>" + "<span id='spnOrderItemType'  style='display:none'>" + obj.aData.ItemType + "</span>" + "<input type='hidden' id='itemWisePONumber' value='" + obj.aData.ItemBlanketPO + "' />" + "<span id='spnIsIgnoreCreditRule'  style='display:none'>" + serIsIgnoreCreditRule + "</span>" + "<span id='spnItemNumber' style='display:none'>" + obj.aData.ItemNumber + "</span>";
        //                dataAttr += " data-spnIsIgnoreCreditRule='" + serIsIgnoreCreditRule + "'";
        //                dataAttr += " data-spnItemNumber='" + obj.aData.ItemNumber + "'";
        //                return "<input type='button' value='Credit MS'  onclick='return MsCreditItems(this);' id='btnAdd' class='CreateBtn pullbutton' style='float: none;padding: 2px 6px;margin-left: 5px;' " + dataAttr + " />";
        //            }
        //            else {
        //                //return "" + "<input type='hidden' id='itemWisePONumber' value='" + obj.aData.ItemBlanketPO + "' />";
        //                return "<input type='button' style='display:none' data-itemWisePONumber='" + obj.aData.ItemBlanketPO + "' />";
        //            }
        //        }
        //        else {
        //            dataAttr = "";
        //            dataAttr += " data-spnQuickListGUID='" + obj.aData.QuickListGUID + "'";
        //            dataAttr += " data-spnOrderItemType='" + obj.aData.ItemType + "'";
        //            dataAttr += " data-itemWisePONumber='" + obj.aData.ItemBlanketPO + "'";

        //            if ($NewPullAction.val() == "Pull") {
        //                //return "<input type='button' value='Pull' onclick='return LoadQuickListData(this);' id='btnLoad' class='CreateBtn pullbutton' style='float: none;padding: 2px 6px;margin-left: 5px;' />" + "<span id='spnQuickListGUID'  style='display:none'>" + obj.aData.QuickListGUID + "</span>" + "<span id='spnOrderItemType'  style='display:none'>" + obj.aData.ItemType + "</span>" + "<input type='hidden' id='itemWisePONumber' value='" + obj.aData.ItemBlanketPO + "' />";
        //                return "<input type='button' value='Pull' onclick='return LoadQuickListData(this);' id='btnLoad' class='CreateBtn pullbutton' style='float: none;padding: 2px 6px;margin-left: 5px;' " + dataAttr + "/>";
        //            }
        //            else if ($NewPullAction.val() == "Credit") {
        //                //return "<input type='button' value='Credit' onclick='return CreditItems(this);' id='btnAdd' class='CreateBtn pullbutton' style='float: none;padding: 2px 6px;margin-left: 5px;' />" + "<span id='spnQuickListGUID'  style='display:none'>" + obj.aData.QuickListGUID + "</span>" + "<span id='spnOrderItemType'  style='display:none'>" + obj.aData.ItemType + "</span>" + "<input type='hidden' id='itemWisePONumber' value='" + obj.aData.ItemBlanketPO + "' />" + "<span id='spnIsIgnoreCreditRule'  style='display:none'>" + serIsIgnoreCreditRule + "</span>" + "<span id='spnItemNumber' style='display:none'>" + obj.aData.ItemNumber + "</span>";
        //                dataAttr += " data-spnIsIgnoreCreditRule='" + serIsIgnoreCreditRule + "'";
        //                dataAttr += " data-spnItemNumber='" + obj.aData.ItemNumber + "'";

        //                return "<input type='button' value='Credit' onclick='return CreditItems(this);' id='btnAdd' class='CreateBtn pullbutton' style='float: none;padding: 2px 6px;margin-left: 5px;' " + dataAttr + " />";
        //            }
        //            else if ($NewPullAction.val() == "CreditMS") {
        //                dataAttr += " data-spnIsIgnoreCreditRule='" + serIsIgnoreCreditRule + "'";
        //                dataAttr += " data-spnItemNumber='" + obj.aData.ItemNumber + "'";

        //                //return "<input type='button' value='Credit MS' onclick='return MsCreditItems(this);' id='btnLoad' class='CreateBtn pullbutton' style='float: none;padding: 2px 6px;margin-left: 5px;' />" + "<span id='spnQuickListGUID'  style='display:none'>" + obj.aData.QuickListGUID + "</span>" + "<span id='spnOrderItemType'  style='display:none'>" + obj.aData.ItemType + "</span>" + "<input type='hidden' id='itemWisePONumber' value='" + obj.aData.ItemBlanketPO + "' />" + "<span id='spnIsIgnoreCreditRule'  style='display:none'>" + serIsIgnoreCreditRule + "</span>" + "<span id='spnItemNumber' style='display:none'>" + obj.aData.ItemNumber + "</span>";
        //                return "<input type='button' value='Credit MS' onclick='return MsCreditItems(this);' id='btnLoad' class='CreateBtn pullbutton' style='float: none;padding: 2px 6px;margin-left: 5px;' " + dataAttr + " />";
        //            }
        //            else {
        //                //return "" + "<input type='hidden' id='itemWisePONumber' value='" + obj.aData.ItemBlanketPO + "' />";
        //                return "<input type='button' style='display:none' data-itemWisePONumber='" + obj.aData.ItemBlanketPO + "' />";
        //            }
        //        }
        //    }
        });
        PullItemListColumnsArr.push({ "mDataProp": "ID", "sClass": "read_only" },
            {
                "mDataProp": "txtQty",//null,
                "bSortable": false,
                "sClass": "read_only control center NotHide",
                "sDefaultContent": '<img id="imgQty" src="' + sImageUrl + 'drildown_open.jpg' + '">',
                //"fnRender": function (obj, val) {

                //    if ($('#NewPullAction').val() == "Credit" || $('#NewPullAction').val() == "CreditMS") {

                //        if (obj.aData.QLCreditQuantity != null) {
                //            var QuantityToCredit = 0;
                //            if (QuickListPULLQty > 0)
                //                QuantityToCredit = obj.aData.QLCreditQuantity * QuickListPULLQty;
                //            else
                //                QuantityToCredit = obj.aData.QLCreditQuantity;
                //            return "<input maxlength='10' type='text' value='" + QuantityToCredit + "' class='text-boxinner text-boxQuantityFormatSR numericalign' id='txtQty' style='width:60px;' />";
                //        }
                //        else {
                //            return "<input maxlength='10' type='text' value='0' class='text-boxinner text-boxQuantityFormatSR numericalign' id='txtQty' style='width:60px;' />";
                //        }
                //    }
                //    else {
                //        if (obj.aData.QuickListGUID == null || obj.aData.QuickListGUID == '') {
                //            var vStrReturn = "";
                //            var dataAttr = " data-hdnPullQtyScanOverride='" + obj.aData.PullQtyScanOverride + "'";

                //            if (obj.aData.SerialNumberTracking == true) {
                //                var DefaultPullQTy = obj.aData.DefaultPullQuantity;
                //                if (DefaultPullQTy != null) {
                //                    if (isNaN(DefaultPullQTy)) {
                //                        DefaultPullQTy = DefaultPullQTy.replace("<span>", "");
                //                        DefaultPullQTy = DefaultPullQTy.replace("</span>", "");
                //                    }
                //                    dataAttr += " data-spnDefaultPullQuantity='" + parseFloat(DefaultPullQTy).toFixed(0) + "'";
                //                    vStrReturn = "<input maxlength='10' type='text' value='" + DefaultPullQTy + "' class='text-boxinner text-boxQuantityFormatSR numericalign' id='txtQty' style='width:60px;' " + dataAttr + " />";
                //                    //vStrReturn = "<input maxlength='10' type='text' value='" + DefaultPullQTy + "' class='text-boxinner text-boxQuantityFormatSR numericalign' id='txtQty' style='width:60px;' />" + "<span id='spnDefaultPullQuantity'  style='display:none'>" + parseFloat(DefaultPullQTy).toFixed(0) + "</span>";
                //                }
                //                else {
                //                    vStrReturn = "<input maxlength='10' type='text' value='' class='text-boxinner text-boxQuantityFormatSR numericalign' id='txtQty' style='width:60px;' " + dataAttr + "/>";
                //                }
                //            }
                //            else {
                //                var DefaultPullQTy = obj.aData.DefaultPullQuantity;
                //                if (DefaultPullQTy != null) {
                //                    if (isNaN(DefaultPullQTy)) {
                //                        DefaultPullQTy = DefaultPullQTy.replace("<span>", "");
                //                        DefaultPullQTy = DefaultPullQTy.replace("</span>", "");
                //                    }
                //                    ////vStrReturn = "<input maxlength='10' type='text' value='" + obj.aData.DefaultPullQuantity + "' class='text-boxinner numericinput numericalign' id='txtQty' style='width:60px;' />" + "<span id='spnDefaultPullQuantity'  style='display:none'>" + parseFloat(obj.aData.DefaultPullQuantity).toFixed(parseInt($('#hdQuantitycentsLimit').val(), 10)) + "</span>";
                //                    //vStrReturn = "<input maxlength='10' type='text' value='" + DefaultPullQTy + "' class='text-boxinner numericinput numericalign' id='txtQty' style='width:60px;' />" + "<span id='spnDefaultPullQuantity'  style='display:none'>" + parseFloat(DefaultPullQTy) + "</span>";
                //                    dataAttr += " data-spnDefaultPullQuantity='" + parseFloat(DefaultPullQTy) + "'";
                //                    vStrReturn = "<input maxlength='10' type='text' value='" + DefaultPullQTy + "' class='text-boxinner numericinput numericalign' id='txtQty' style='width:60px;' " + dataAttr + "/>";
                //                }
                //                else {
                //                    vStrReturn = "<input maxlength='10' type='text' value='' class='text-boxinner numericinput numericalign' id='txtQty' style='width:60px;' " + dataAttr + "/>";
                //                }
                //            }
                //            //vStrReturn = vStrReturn + "<input id='hdnPullQtyScanOverride'  type='hidden' value='" + obj.aData.PullQtyScanOverride + "' />"; // use "data-" attribute
                //            return vStrReturn;
                //        }
                //        else {
                //            return "<input maxlength='10' type='text' value='' class='text-boxinner numericalign' id='txtQty' onkeypress = 'return onlyNumeric(event);' style='width:60px;' />";
                //        }
                //    }
                //}
            });
        PullItemListColumnsArr.push({
            "mData": "ImagePathDisp",
            "mDataProp": "ImagePath",
            "sClass": "read_only",
            "sDefaultContent": '',
            "bSortable": true,
            "bSearchable": false,
            "bVisible": false,
            //"fnRender": function (obj, val) {
            //"mRender": function (val, type, obj) {
            //    if ((obj.ImagePath != '' && obj.ImagePath != null) || (obj.ItemImageExternalURL != '' && obj.ItemImageExternalURL != null)) {

            //        if (obj.ImageType != '' && obj.ImageType != null && obj.ImageType == 'ImagePath' && obj.ImagePath != '' && obj.ImagePath != null) {

            //            var path = logoPathItemImage;
            //            return '<img style="cursor:pointer;"  alt="' + (obj.ItemNumber) + '" id="ItemImageBox" width="120px" height="120px" src="' + path + "/" + obj.ID + "/" + obj.ImagePath + '">';
            //        }
            //        else if (obj.ImageType != '' && obj.ImageType != null && obj.ImageType == "ExternalImage" && obj.ItemImageExternalURL != '' && obj.ItemImageExternalURL != null) {
            //            return '<img style="cursor:pointer;"  alt="' + (obj.ItemNumber) + '" id="ItemImageBox" width="120px" height="120px" src="' + obj.ItemImageExternalURL + '">';
            //        }
            //        else {
            //            return "<img src='../Content/images/no-image.jpg' />";
            //        }
            //    }
            //    else {
            //        return "<img src='../Content/images/no-image.jpg' />";
            //    }

            //}
        });
        PullItemListColumnsArr.push({
            "mData": "ItemNumber",
            "mDataProp": "ItemNumber",
            "sClass": "read_only NotHide",
            "sDefaultContent": '',
            "bSortable": true,
            "bSearchable": false
        });
        PullItemListColumnsArr.push({
            "mData": "DefaultLocationNameDisp",
            "mDataProp": 'DefaultLocationName',
            "sClass": "read_only NotHide",
            "bSortable": true,
            "bSearchable": false,
            "sDefaultContent": '',
            //"fnRender": function (obj, val) {
            //"mRender": function (val, type, obj) {
            //    if (obj.QuickListGUID == null || obj.QuickListGUID == '') {

            //        if ($('#NewPullAction').val() == "Credit") {
            //            var vDefaultLocationName = '';
            //            if (obj.ItemType != 4) {
            //                vDefaultLocationName = obj.DefaultLocationName;
            //            }

            //            vDefaultLocationName = ((obj.BinNumber != undefined && obj.BinNumber != null && obj.BinNumber != "") ? obj.BinNumber : obj.DefaultLocationName);
            //            var dataAttr = " data-vDefaultLocationName='" + vDefaultLocationName + "'";
            //            var strReturn = '<span style="display:none">"' + vDefaultLocationName + '"</span><span style="position:relative">';
            //            if (hasOnTheFlyEntryRight == "False") {
            //                //strReturn = strReturn + '<input type="hidden" value="" id="hdnIsLoadMoreLocations" /><input type="text" id="txtBinNumber" class="text-boxinner AutoCreditBins" style = "width:93%;" value="' + vDefaultLocationName + '" />';
            //                dataAttr += " data-hdnIsLoadMoreLocations=''";
            //                strReturn = strReturn + '<input type="text" id="txtBinNumber" class="text-boxinner AutoCreditBins" style = "width:93%;" value="' + vDefaultLocationName + '" />';
            //            }
            //            else {
            //                //strReturn = strReturn + '<input type="hidden" value="false" id="hdnIsLoadMoreLocations" /><input type="text" id="txtBinNumber" class="text-boxinner AutoCreditBins" style = "width:93%;" value="' + vDefaultLocationName + '" />';
            //                dataAttr += " data-hdnIsLoadMoreLocations='false'";
            //                strReturn = strReturn + '<input type="text" id="txtBinNumber" class="text-boxinner AutoCreditBins" style = "width:93%;" value="' + vDefaultLocationName + '" />';
            //            }

            //            var binID = obj.DefaultLocation;

            //            binID = ((obj.BinID != undefined && obj.BinID != null && obj.BinID != "" && obj.BinID > 0) ? obj.BinID : obj.DefaultLocation);

            //            if (isNaN(parseInt(binID)) || parseInt(binID) <= 0) {
            //                binID = '';
            //            }

            //            dataAttr += " data-BinID='" + binID +"'";
            //            //strReturn = strReturn + ' <input type="hidden" id="BinID" value="' + binID + '" />';

            //            var DefaultPullQTy1 = obj.ItemDefaultPullQuantity;
            //            if (DefaultPullQTy1 != null) {
            //                if (isNaN(DefaultPullQTy1)) {
            //                    DefaultPullQTy1 = DefaultPullQTy1.replace("<span>", "");
            //                    DefaultPullQTy1 = DefaultPullQTy1.replace("</span>", "");
            //                }
            //            }
            //            //strReturn = strReturn + ' <input type="hidden" id="hdnDPQ" value="' + DefaultPullQTy1 + '" />';
            //            dataAttr += " data-hdnDPQ='" + DefaultPullQTy1 + "'";
            //            strReturn = strReturn + ' <a id="lnkShowAllOptionsCR" href="javascript:void(0);" style="position:absolute; right:5px; top:0px;" class="ShowAllOptionsBinCR" ><img src="/Content/images/arrow_down_black.png" alt="select" ' + dataAttr +' /></a></span>';
            //            return strReturn
            //        }
            //        else if ($('#NewPullAction').val() == "CreditMS") {

            //            var vDefaultLocationName = '';
            //            if (obj.ItemType != 4) {
            //                vDefaultLocationName = obj.DefaultLocationName;
            //            }

            //            vDefaultLocationName = ((obj.BinNumber != undefined && obj.BinNumber != null && obj.BinNumber != "") ? obj.BinNumber : obj.DefaultLocationName);
            //            var dataAttrMS = " data-vDefaultLocationName='" + vDefaultLocationName + "'";
            //            var strReturn = '<span style="display:none">"' + vDefaultLocationName + '"</span><span style="position:relative">';
            //            if (hasOnTheFlyEntryRight == "False") {
            //                //strReturn = strReturn + '<input type="hidden" value="" id="hdnIsLoadMoreLocations" /><input type="text" id="txtBinNumber" class="text-boxinner AutoMSCreditBins" style = "width:93%;" value="' + vDefaultLocationName + '" />';
            //                dataAttrMS += " data-hdnIsLoadMoreLocations=''";
            //                strReturn = strReturn + '<input type="text" id="txtBinNumber" class="text-boxinner AutoMSCreditBins" style = "width:93%;" value="' + vDefaultLocationName + '" />';
            //            }
            //            else {
            //                //strReturn = strReturn + '<input type="hidden" value="false" id="hdnIsLoadMoreLocations" /><input type="text" id="txtBinNumber" class="text-boxinner AutoMSCreditBins" style = "width:93%;" value="' + vDefaultLocationName + '" />';
            //                dataAttrMS += " data-hdnIsLoadMoreLocations='false'";
            //                strReturn = strReturn + '<input type="text" id="txtBinNumber" class="text-boxinner AutoMSCreditBins" style = "width:93%;" value="' + vDefaultLocationName + '" />';
            //            }

            //            var binID = obj.DefaultLocation;

            //            binID = ((obj.BinID != undefined && obj.BinID != null && obj.BinID != "" && obj.BinID > 0) ? obj.BinID : obj.DefaultLocation);

            //            if (isNaN(parseInt(binID)) || parseInt(binID) <= 0) {
            //                binID = '';
            //            }

            //            //strReturn = strReturn + ' <input type="hidden" id="BinID" value="' + binID + '" />';
            //            dataAttrMS += " data-BinID='" + binID + "'";
            //            var DefaultPullQTy1 = obj.ItemDefaultPullQuantity;
            //            if (DefaultPullQTy1 != null) {
            //                if (isNaN(DefaultPullQTy1)) {
            //                    DefaultPullQTy1 = DefaultPullQTy1.replace("<span>", "");
            //                    DefaultPullQTy1 = DefaultPullQTy1.replace("</span>", "");
            //                }
            //            }
            //            //strReturn = strReturn + ' <input type="hidden" id="hdnDPQ" value="' + DefaultPullQTy1 + '" />';
            //            dataAttrMS += " data-hdnDPQ='" + DefaultPullQTy1 + "'";
            //            strReturn = strReturn + ' <a id="lnkShowAllOptionsCR" href="javascript:void(0);" style="position:absolute; right:5px; top:0px;" class="ShowAllOptionsBinMCR" ><img src="/Content/images/arrow_down_black.png" alt="select" ' + dataAttrMS +' /></a></span>';
            //            return strReturn
            //        }
            //        else {
            //            var vDefaultLocationName = '';
            //            if (obj.ItemType != 4) {
            //                vDefaultLocationName = obj.DefaultLocationName;
            //            }

            //            vDefaultLocationName = ((obj.BinNumber != undefined && obj.BinNumber != null && obj.BinNumber != "") ? obj.BinNumber : obj.DefaultLocationName);
            //            var dataAttrP = " data-vDefaultLocationName='" + vDefaultLocationName + "'";
            //            var strReturn = '<span style="display:none">"' + vDefaultLocationName + '"</span><span style="position:relative"><input type="text" id="txtBinNumber" class="text-boxinner AutoPullBins bin-input-readonly" style = "width:93%;" value="' + vDefaultLocationName + '" />';
            //            //var strReturn = '<span style="position:relative"><input type="text" id="txtBinNumber" class="text-boxinner AutoPullBins bin-input-readonly" style = "width:93%;" value="' + vDefaultLocationName + '" />';
            //            var binID = obj.DefaultLocation;

            //            binID = ((obj.BinID != undefined && obj.BinID != null && obj.BinID != "" && obj.BinID > 0) ? obj.BinID : obj.DefaultLocation);

            //            if (isNaN(parseInt(binID)) || parseInt(binID) <= 0) {
            //                binID = '';
            //            }

            //            //strReturn = strReturn + ' <input type="hidden" id="BinID" value="' + binID + '" />';
            //            dataAttrP += " data-BinID='" + binID + "'";

            //            var DefaultPullQTy1 = obj.ItemDefaultPullQuantity;
            //            if (DefaultPullQTy1 != null) {
            //                if (isNaN(DefaultPullQTy1)) {
            //                    DefaultPullQTy1 = DefaultPullQTy1.replace("<span>", "");
            //                    DefaultPullQTy1 = DefaultPullQTy1.replace("</span>", "");
            //                }
            //            }
            //            //strReturn = strReturn + ' <input type="hidden" id="hdnDPQ" value="' + DefaultPullQTy1 + '" />';
            //            dataAttrP += " data-hdnDPQ='" + DefaultPullQTy1 + "'";
            //            strReturn = strReturn + ' <a id="lnkShowAllOptions" href="javascript:void(0);" style="position:absolute; right:5px; top:0px;" class="ShowAllOptionsBin" ><img src="/Content/images/arrow_down_black.png" alt="select" ' + dataAttrP +'/></a></span>';
            //            return strReturn
            //        }
            //    }
            //    else {
            //        return "";
            //    }
            //}
        });

        PullItemListColumnsArr.push(arrPullPoMaster);
            
            PullItemListColumnsArr.push({
            "mDataProp": "txtProjectSpentCol",//null,
            "sClass": ProjectSpendClass,
            "bSortable": false,
            "bSearchable": false,
            "bVisible": true,
            "sDefaultContent": ''
            //,"fnRender": function (obj, val) {
            //    if (obj.aData.QuickListGUID == null || obj.aData.QuickListGUID == '') {
            //        var dataAttr = '';
            //        var strReturn = '';

            //        //if (obj.aData.ItemType != 4) {
            //        if (obj.aData.IsDefaultProjectSpend == false || (!ProjectSpendVisible)) {
            //            if (ProjectSpendNameSCR == '') {
            //                dataAttr += " data-ProjectID=''";
            //                strReturn = '<span style="position:relative"><input type="text" id="txtProjectSpent" class="text-boxinner AutoPullProjectSpents" style = "width:93%;" value="" ' + dataAttr +' />'; 
            //                //strReturn = strReturn + ' <input type="hidden" id="ProjectID" value="" />';
            //                strReturn = strReturn + ' <a id="lnkShowAllOptions" href="javascript:void(0);" style="position:absolute; right:5px; top:0px;" class="ShowAllOptions" ><img src="/Content/images/arrow_down_black.png" alt="select" /></a></span>';
            //            }
            //            else {
            //                dataAttr += " data-ProjectID='" + ProjectSpendGuidSC + "'";
            //                strReturn = '<span style="position:relative"><input type="text" id="txtProjectSpent" class="text-boxinner" disabled="disabled" readonly="readonly" style = "width:93%;" value="' + ProjectSpendNameSCR + '" ' + dataAttr +' />';
            //                //strReturn = strReturn + ' <input type="hidden" id="ProjectID" value="' + ProjectSpendGuidSCR + '" />';

            //            }
            //        }
            //        else {
            //            dataAttr += " data-ProjectID='" + obj.aData.DefaultProjectSpendGuid + "'";
            //            strReturn = '<span style="position:relative"><input type="text" disabled="disabled" id="txtProjectSpent" class="text-boxinner AutoPullProjectSpents" style = "width:93%;" value="' + obj.aData.DefaultProjectSpend + '" ' + dataAttr +' />';
            //            //strReturn = strReturn + ' <input type="hidden" id="ProjectID" value="' + obj.aData.DefaultProjectSpendGuid + '" />';
            //            strReturn = strReturn + ' <a id="lnkShowAllOptions" href="javascript:void(0);" disabled="disabled" style="position:absolute; right:5px; top:0px;" class="ShowAllOptions" ><img src="/Content/images/arrow_down_black.png" alt="select" /></a></span>';
            //        }
                                    
            //        return strReturn
            //        //}
            //    }
            //    return "";
            //}
        });
        
         
        
        PullItemListColumnsArr.push({ "mDataProp": "ManufacturerNumber", "sClass": "read_only" });
        PullItemListColumnsArr.push({ "mDataProp": "ManufacturerName", "sClass": "read_only" });
        PullItemListColumnsArr.push({ "mDataProp": "SupplierPartNo", "sClass": "read_only" });
        PullItemListColumnsArr.push({
            "mData":"SupplierNameDisp",
            "mDataProp": "SupplierName",
            "sClass": "read_only supppo",
            //"fnRender": function (obj, val) {
            //"mRender": function (val, type, obj) {
            //    //return "<span>" + obj.aData.SupplierName + "</span><input type='hidden' id='supID' value=" + obj.aData.SupplierID + " />";
            //    var dataAttr = " data-supID='" + obj.SupplierID + "'";
            //    return "<span " + dataAttr +">" + obj.SupplierName + "</span>";
            //}
        });
        PullItemListColumnsArr.push({ "mDataProp": "UPC", "sClass": "read_only" });
        PullItemListColumnsArr.push({ "mDataProp": "UNSPSC", "sClass": "read_only" });
        PullItemListColumnsArr.push({ "mDataProp": "Description", "sClass": "read_only" });
        PullItemListColumnsArr.push({
            "mData":"LongDescriptionDisp",
            "mDataProp": "LongDescription", "sClass": "read_only", "sDefaultContent": '',
            //"fnRender": function (obj, val) {
            //"mRender": function (val, type, obj) {
            //    return "<div class='comment more'>" + val + "</div>";
            //}
        });
        PullItemListColumnsArr.push({ "mDataProp": "CostUOMName", "sClass": "read_only" });
        PullItemListColumnsArr.push({
            "mData": "DefaultReorderQuantityDisp",
            "mDataProp": "DefaultReorderQuantity", "sClass": "read_only numericalign",
            //"fnRender": function (obj, val) {
            //"mRender": function (val, type, obj) {
            //    if (obj.DefaultReorderQuantity != null && obj.DefaultReorderQuantity != NaN) {
            //        return  FormatedCostQtyValues(obj.DefaultReorderQuantity, 2) ;
            //    }
            //    else {
            //        //return "<span>" + FormatedCostQtyValues(0, 2) + "</span>";
            //        return  tmpZeroCostQtyFormatted2 ;
            //    }
            //}
        });
        PullItemListColumnsArr.push({
            "mData": "DefaultPullQuantityDisp",
            "mDataProp": "DefaultPullQuantity", "sClass": "read_only numericalign",
            //"fnRender": function (obj, val) {
            // "mRender": function (val, type, obj) {
            //    if (obj.DefaultPullQuantity != null && obj.DefaultPullQuantity != NaN)
            //        return FormatedCostQtyValues(obj.DefaultPullQuantity, 2) ;
            //    else
            //        return  tmpZeroCostQtyFormatted2;
            //}
        });
        if (isCost == 'True') {
            PullItemListColumnsArr.push({
                "mData": "CostDisp",
                "mDataProp": "Cost", "sClass": "read_only numericalign isCost",
                //"fnRender": function (obj, val) {
                //"mRender": function (val, type, obj) {

                //    if (obj.Cost != null && obj.Cost != NaN)
                //        return FormatedCostQtyValues(obj.Cost, 1) ;
                //    else
                //        return  tmpZeroCostQtyFormatted1 ;

                //}
            });
            PullItemListColumnsArr.push({
                "mDataProp": "Markup", "sClass": "read_only numericalign isCost"
                //,"fnRender": function (obj, val) {
                //    return obj.aData.Markup;
                //}
            });
            PullItemListColumnsArr.push({
                "mDataProp": "SellPrice", "sClass": "read_only numericalign isCost",
                //"fnRender": function (obj, val) {
                "mRender": function (val, type, obj) {
                    if (obj.SellPrice != null && obj.SellPrice != NaN)
                        return FormatedCostQtyValues(obj.SellPrice, 1) ;
                    else
                        return tmpZeroCostQtyFormatted1 ;

                }
            });
            PullItemListColumnsArr.push({
                "mDataProp": "ExtendedCost", "sClass": "read_only numericalign isCost",
                //"fnRender": function (obj, val) {
                "mRender": function (val, type, obj) {

                    if (obj.ExtendedCost != null && obj.ExtendedCost != NaN)
                        return  FormatedCostQtyValues(obj.ExtendedCost, 1) ;
                    else
                        return tmpZeroCostQtyFormatted1 ;

                }
            });
        }

        PullItemListColumnsArr.push({ "mDataProp": "LeadTimeInDays", "sClass": "read_only" });
        PullItemListColumnsArr.push({
            //"mData": "TrendDisp",
            "mDataProp": "Trend", "sClass": "read_only"
            , "fnRender": function (obj, val) {
                return GetBoolInFormat(obj, val);
            }
        });
        PullItemListColumnsArr.push({
            //"mData": "TaxableDisp",
            "mDataProp": "Taxable", "sClass": "read_only"
            , "fnRender": function (obj, val) {
                return GetBoolInFormat(obj, val);
            }
        });
        PullItemListColumnsArr.push({
            //"mData": "ConsignmentDisp",
            "mDataProp": "Consignment", "sClass": "read_only"
            , "fnRender": function (obj, val) {            
                return GetBoolInFormat(obj, val);
            }
        });
        PullItemListColumnsArr.push({
            "mData":"StagedQuantityDisp",
            "mDataProp": "StagedQuantity", "sClass": "read_only numericalign",
            //"fnRender": function (obj, val) {
            //"mRender": function (val, type, obj) {
            //    if (obj.StagedQuantity != null && obj.StagedQuantity != NaN)
            //        return  FormatedCostQtyValues(obj.StagedQuantity, 2) ;
            //    else
            //        return tmpZeroCostQtyFormatted2 ;
            //}
        });
        PullItemListColumnsArr.push({
            "mData": "InTransitquantityDisp",
            "mDataProp": "InTransitquantity", "sClass": "read_only numericalign",
            //"fnRender": function (obj, val) {
            //"mRender": function (val, type, obj) {
            //    if (obj.InTransitquantity != null && obj.InTransitquantity != NaN)
            //        return FormatedCostQtyValues(obj.InTransitquantity, 2) ;
            //    else
            //        return  tmpZeroCostQtyFormatted2 ;
            //}
        });
        PullItemListColumnsArr.push({
            "mData": "OnOrderQuantityDisp",
            "mDataProp": "OnOrderQuantity", "sClass": "read_only numericalign",
            //"fnRender": function (obj, val) {
            //"mRender": function (val, type, obj) {
            //    if (obj.OnOrderQuantity != null && obj.OnOrderQuantity != NaN)
            //        return  FormatedCostQtyValues(obj.OnOrderQuantity, 2) ;
            //    else
            //        return  tmpZeroCostQtyFormatted2 ;
            //}
        });
        PullItemListColumnsArr.push({
            "mData": "OnTransferQuantityDisp",
            "mDataProp": "OnTransferQuantity", "sClass": "read_only numericalign",
            //"fnRender": function (obj, val) {
            //"mRender": function (val, type, obj) {
            //    if (obj.OnTransferQuantity != null && obj.OnTransferQuantity != NaN)
            //        return  FormatedCostQtyValues(obj.OnTransferQuantity, 2) ;
            //    else
            //        return  tmpZeroCostQtyFormatted2 ;
            //}
        });
        PullItemListColumnsArr.push({
            "mData": "SuggestedOrderQuantityDisp",
            "mDataProp": "SuggestedOrderQuantity", "sClass": "read_only numericalign",
            //"fnRender": function (obj, val) {
            //"mRender": function (val, type, obj) {
            //    if (obj.SuggestedOrderQuantity != null && obj.SuggestedOrderQuantity != NaN) {
            //        return  FormatedCostQtyValues(obj.SuggestedOrderQuantity, 2) ;
            //    }
            //    else {
            //        return tmpZeroCostQtyFormatted2 ;
            //    }
            //}
        });
        PullItemListColumnsArr.push({
            "mData": "RequisitionedQuantityDisp",
            "mDataProp": "RequisitionedQuantity", "sClass": "read_only numericalign",
            //"fnRender": function (obj, val) {
            //"mRender": function (val, type, obj) {
            //    if (obj.RequisitionedQuantity != null && obj.RequisitionedQuantity != NaN) {
            //        return  FormatedCostQtyValues(obj.RequisitionedQuantity, 2) ;
            //    }
            //    else {
            //        return  tmpZeroCostQtyFormatted2 ;
            //    }
            //}
        });
        PullItemListColumnsArr.push({
            "mData": "AverageUsageDisp",
            "mDataProp": "AverageUsage", "sClass": "read_only numericalign",
            //"fnRender": function (obj, val) {
            //"mRender": function (val, type, obj) {
            //    if (obj.AverageUsage != null && obj.AverageUsage != NaN) {
            //        return  FormatedCostQtyValues(obj.AverageUsage, 4) ;
            //    }
            //    else {
            //        return  FormatedCostQtyValues(0, 4) ;
            //    }
            //}

        });
        PullItemListColumnsArr.push({
            "mData": "TurnsDisp",
            "mDataProp": "Turns", "sClass": "read_only numericalign",
            //"fnRender": function (obj, val) {
            //"mRender": function (val, type, obj) {
            //    if (obj.Turns != null && obj.Turns != NaN)
            //        return FormatedCostQtyValues(obj.Turns, 4) ;
            //    else
            //        return  FormatedCostQtyValues(0, 4) ;
            //}
        });
        PullItemListColumnsArr.push({
            "mData": "OnHandQuantityDisp",
            "mDataProp": "OnHandQuantity", "sClass": "read_only numericalign",
            //"fnRender": function (obj, val) {
            //"mRender": function (val, type, obj) {
            //    if (obj.OnHandQuantity != null && obj.OnHandQuantity != NaN) {
            //        return FormatedCostQtyValues(obj.OnHandQuantity, 2) ;
            //    }
            //    else {
            //        return  tmpZeroCostQtyFormatted2 ;
            //    }
            //}
        });
        PullItemListColumnsArr.push({
            "mData": "CriticalQuantityDisp",
            "mDataProp": "CriticalQuantity", "sClass": "read_only numericalign",
            //"fnRender": function (obj, val) {
            //"mRender": function (val, type, obj) {
            //    if (obj.CriticalQuantity != null && obj.CriticalQuantity != NaN) {
            //        return  FormatedCostQtyValues(obj.CriticalQuantity, 2) ;
            //    }
            //    else {
            //        return  tmpZeroCostQtyFormatted2 ;
            //    }
            //}
        });
        PullItemListColumnsArr.push({
            "mData": "MinimumQuantityDisp",
            "mDataProp": "MinimumQuantity", "sClass": "read_only numericalign",
            //"fnRender": function (obj, val) {
            //"mRender": function (val, type, obj) {
            //    if (obj.MinimumQuantity != null && obj.MinimumQuantity != NaN) {
            //        return  FormatedCostQtyValues(obj.MinimumQuantity, 2) ;
            //    }
            //    else {
            //        return  tmpZeroCostQtyFormatted2 ;
            //    }
            //}
        });
        PullItemListColumnsArr.push({
            "mData": "MaximumQuantityDisp",
            "mDataProp": "MaximumQuantity", "sClass": "read_only numericalign",
            //"fnRender": function (obj, val) {
            //"mRender": function (val, type, obj) {
            //    if (obj.MaximumQuantity != null && obj.MaximumQuantity != NaN) {
            //        return  FormatedCostQtyValues(obj.MaximumQuantity, 2) ;
            //    }
            //    else {
            //        return  tmpZeroCostQtyFormatted2 ;
            //    }
            //}
        });
        PullItemListColumnsArr.push({ "mDataProp": "WeightPerPiece", "sClass": "read_only numericalign" });
        PullItemListColumnsArr.push({ "mDataProp": "ItemUniqueNumber", "sClass": "read_only" });
        PullItemListColumnsArr.push({
            //"mData": "IsTransferDisp",
            "mDataProp": "IsTransfer", "sClass": "read_only"
            , "fnRender": function (obj, val) {
                return GetBoolInFormat(obj, val);
            }
        });
        PullItemListColumnsArr.push({
            //"mData": "IsPurchaseDisp",
            "mDataProp": "IsPurchase", "sClass": "read_only"
            , "fnRender": function (obj, val) {
                return GetBoolInFormat(obj, val);
            }
        });
        PullItemListColumnsArr.push({ "mDataProp": "InventoryClassificationName", "sClass": "read_only" });
        PullItemListColumnsArr.push({
            "mDataProp": "SerialNumberTracking", "sClass": "read_only"
            , "fnRender": function (obj, val) {
                return GetBoolInFormat(obj, val);
            }
        });
        PullItemListColumnsArr.push({
            //"mData": "LotNumberTrackingDisp",
            "mDataProp": "LotNumberTracking", "sClass": "read_only"
            , "fnRender": function (obj, val) {
                return GetBoolInFormat(obj, val);
            }
        });
        PullItemListColumnsArr.push({
            //"mData": "DateCodeTrackingDisp",
            "mDataProp": "DateCodeTracking", "sClass": "read_only"
            , "fnRender": function (obj, val) {
                return GetBoolInFormat(obj, val);
            }
        });
        PullItemListColumnsArr.push({
            "mData": "ItemTypeDisp",
            "mDataProp": "ItemType", "sClass": "read_only"
            //, "fnRender": function (obj, val) {
            //,"mRender": function (val, type, obj) {
            //    if (val == 1)
            //        return "Item";
            //    else if (val == 2)
            //        return "Quick List";
            //    else if (val == 3)
            //        return "Kit";
            //    else if (val == 4)
            //        return "Labor";
            //    else
            //        return "";
            //}
        });
        PullItemListColumnsArr.push({ "mDataProp": "IsLotSerialExpiryCost", "sClass": "read_only" });
        PullItemListColumnsArr.push({ "mDataProp": "RoomName", "sClass": "read_only" });
        PullItemListColumnsArr.push({
            "mDataProp": "Created", "sClass": "read_only",
            //"fnRender": function (obj, val) {
            "mRender": function (val, type, obj) {
                //return GetDateInFullFormat(val);
                return obj.CreatedDate;
            }
        });
        PullItemListColumnsArr.push({
            "mDataProp": "Updated", "sClass": "read_only",
            //"fnRender": function (obj, val) {
            "mRender": function (val, type, obj) {
                //return GetDateInFullFormat(val);
                return obj.UpdatedDate;
            }
        });
        PullItemListColumnsArr.push({ "mDataProp": "UpdatedByName", "sClass": "read_only" });
        PullItemListColumnsArr.push({ "mDataProp": "CreatedByName", "sClass": "read_only" });
        PullItemListColumnsArr.push({ "mDataProp": "CategoryName", "sClass": "read_only" });
        PullItemListColumnsArr.push({ "mDataProp": "Unit", "sClass": "read_only" });
        PullItemListColumnsArr.push({ "mDataProp": "GLAccount", "sClass": "read_only" });
        if (serIsIgnoreCreditRule == "true" || serIsIgnoreCreditRule == "True" || serIsIgnoreCreditRule == true) {
            PullItemListColumnsArr.push({
                "mDataProp": 'StagingHeaderName',
                "sClass": "read_only",
                "bSortable": false,
                "bSearchable": false,
                "sDefaultContent": '',
                //"fnRender": function (obj, val) {
                //    if ($('#NewPullAction').val() == "CreditMS") {

                //        var vStagingHeaderName = '';
                //        var dataAttr = "";
                //        var strReturn = '<span style="display:none">"' + vStagingHeaderName + '"</span><span style="position:relative">';
                //        if (hasOnTheFlyEntryRight == "False") {
                //            dataAttr += " data-hdnIsLoadMoreStaging = ''";
                //            //strReturn = strReturn + '<input type="hidden" value="" id="hdnIsLoadMoreStaging" /><input type="text" id="txtMSStagingHeader" name="txtMSStagingHeader" class="text-boxinner AutoMSCreditStagingHeader" style = "width:93%;"/>';
                //            strReturn = strReturn + '<input type="text" id="txtMSStagingHeader" name="txtMSStagingHeader" class="text-boxinner AutoMSCreditStagingHeader" style = "width:93%;"/>';
                //        }
                //        else {
                //            dataAttr += " data-hdnIsLoadMoreStaging = ''";
                //            //strReturn = strReturn + '<input type="hidden" id="hdnIsLoadMoreStaging" /><input type="text" id="txtMSStagingHeader" name="txtMSStagingHeader" class="text-boxinner AutoMSCreditStagingHeader" style = "width:93%;" />';
                //            strReturn = strReturn + '<input type="text" id="txtMSStagingHeader" name="txtMSStagingHeader" class="text-boxinner AutoMSCreditStagingHeader" style = "width:93%;" />';
                //        }
                //        strReturn = strReturn + ' <a id="lnkShowAllOptionsCR" href="javascript:void(0);" style="position:absolute; right:5px; top:0px;" class="ShowAllOptionsStagingHeader" ><img src="/Content/images/arrow_down_black.png" alt="select" '+ dataAttr +' /></a></span>';
                //        return strReturn;
                //    }
                //}
            }
            );
        }
        PullItemListColumnsArr.push({
            "mDataProp": null,
            "sClass": "read_only NotHide",
            "bSortable": false,
            "bSearchable": false,
            "sDefaultContent": '',
            "fnRender": function (obj, val) {
            //"mRender": function (val, type, obj) {
                if (obj.QuickListGUID == null || obj.QuickListGUID == '') {
                    var strReturn = '';
                    //if ($('#NewPullAction').val() == "Pull") {
                    strReturn = strReturn + ' <input type="hidden" id="hdnSupplierAccountNumber"/>';
                    strReturn = strReturn + '<span style="position:relative"><input type="text" id="txtSupplierAccountNumber" class="text-boxinner AutoSupplierAccountNumber" style="width:120px;" />';
                    strReturn = strReturn + ' <a id="lnkShowAllOptions" href="javascript:void(0);" style="position:absolute; right:5px; top:0px;" class="ShowAllOptionsSupplierAccountNumber" ><img src="/Content/images/arrow_down_black.png" alt="select" /></a></span>';
                    //}
                    return strReturn;
                }
                else {
                    return "1";
                }
            }
        });

        //PullItemListColumnsArr.push(arrPullMasterEO);
        $.each(arrPullMasterEO, function (index, val) {
            PullItemListColumnsArr.push(val);
        });

        //PullItemListColumnsArr.push(arrItemMaster);
        $.each(arrItemMaster, function (index, val) {
            PullItemListColumnsArr.push(val);
        });

        if (sProjectSpendVisible == "True") {
            ProjectSpendClass = "read_only NotHide";
        }


        if (sProjectSpendName == '') {
            ProjectSpendNameSCR = sProjectSpendName;
            ProjectSpendGuidSCR = sProjectSpendGuid;
        }
        if (sProjectSpendVisible == "True") {
            ProjectSpendVisible = true;
        }

        $(document).ready(function () {

            objColumns = GetGridHeaderColumnsObject(self.gridId, 'New Pull Columns', 'NewPullItemMaster', 'callbacknewFromReorder()');

            LoadTabs();
            $('#tab1').addClass('selected').removeClass('unselected');
            $('#tab5').removeClass('selected');

            var gaiSelected = [];

            $("#THStagingHeader").hide();
            $("#TDStagingHeader").hide();

            var $ItemModeDataTable = $('#' + self.gridId);
            

            oTableItemModel = $ItemModeDataTable.dataTable({
                "bJQueryUI": true,
                "bScrollCollapse": true,
                "bDeferRender": false, // true value causes issue on some autocomplete
                //"bFilter":false,
                //"bAutoWidth": false,
                "sScrollX": "350%",
                //"sScrollX": "8000px",
                "sDom": 'RC<"top"lp<"clear">>rt<"bottom"i<"clear">>T',
                "oColVis": {},
                "aaSorting": [[2, "asc"]],
                "oColReorder": {},
                "sPaginationType": "full_numbers",
                "bProcessing": true,
                "bStateSave": true,
                "oLanguage": oLanguage,
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                    var $nRow = $(nRow);

                    if (self.gridRowStartIndex === null) {
                        self.gridRowStartIndex = this.fnSettings()._iDisplayStart;
                    }

                    $("td.RowNo:first", nRow).html(self.gridRowStartIndex + iDisplayIndex + 1);
                    var vSuppID = aData.SupplierID; //_NewConsumePull.getDataFromRow($nRow, 'supID'); //$(nRow).find("#supID").val();
                    
                    var itemWisePONumber = aData.ItemBlanketPO; //_NewConsumePull.getItemDataFromRow($nRow).itemWisePONumber; //$(nRow).find("input#itemWisePONumber").val();
                    var $txtPullOrderNumber = $nRow.find('#txtPullOrderNumber');

                    if ($txtPullOrderNumber != null) {
                        $txtPullOrderNumber.attr("maxlength", 22);
                        if (itemWisePONumber != '' && itemWisePONumber != "null") {
                            //if ($txtPullOrderNumber != null) {
                                $txtPullOrderNumber.val(itemWisePONumber);
                                return false;
                            //}
                        }

                        if (hdnSuppPOPairVal.val == null) {
                            hdnSuppPOPairVal.val = $("#hdnSuppPOPair").val();
                        }
                        //var strjson = $("#hdnSuppPOPair").val();
                        //if (strjson.length > 0) {
                        if (hdnSuppPOPairVal.val.length > 0) {
                            if (hdnSuppPOPairVal.json == null) {
                                hdnSuppPOPairVal.json = $.parseJSON(hdnSuppPOPairVal.val);
                            }
                            //var jsonobj = $.parseJSON(strjson);

                            var poNos = $.grep(hdnSuppPOPairVal.json, function (obj, idx) {
                                return obj.SupplierID == vSuppID; 
                            });

                            if (poNos.length) {
                                $txtPullOrderNumber.val(poNos[0].PONumber);
                            }

                            //$.each(hdnSuppPOPairVal.json, function (idx, obj) {
                            //    if (obj.SupplierID == vSuppID) {
                            //        //if ($txtPullOrderNumber != null) {
                            //        $txtPullOrderNumber.val(obj.PONumber);
                            //        return false;
                            //        //}
                            //    }
                            //});

                        }
                    }

                    return nRow;
                },
                "fnStateSaveParams": self.grid_fnStateSaveParams,
                "fnStateLoad": function (oSettings) {
                    var o;
                    $.ajax({
                        "url": _NewConsumePull.urls.LoadGridStateUrl,
                        "type": "POST",
                        data: { ListName: 'NewPullItemMaster' },
                        "async": false,
                        cache: false,
                        "dataType": "json",
                        "success": function (json) {

                            if (json.jsonData != '')
                                o = JSON.parse(json.jsonData);

                            //var ColreOrder = o.ColReorder.toString().split(',');
                            if (json.jsonData != '') {
                                var InvisibleUDF = 0;

                                var PullOrderNumberIndex = objColumns['Pull Order Number'];
                                if (o.abVisCols[PullOrderNumberIndex] == false) {
                                    //$('#THPullOrderNumber').hide();
                                    //$('#TDPullOrderNumber').hide();
                                    $('#THPullOrderNumber').html('');
                                    $('#TDPullOrderNumber').html('');
                                    InvisibleUDF++;
                                }

                                var ProjectSpendNameIndex = objColumns[resProjectSpendName];
                                if (sProjectSpendVisible == "True") {
                                    o.abVisCols[ProjectSpendNameIndex] = true;
                                }
                                else if (o.abVisCols[ProjectSpendNameIndex] == false) {
                                    $('#THProjectSpendName').html('');
                                    $('#TDProjectSpendName').html('');
                                    InvisibleUDF++;
                                }

                                UDF1Index = objColumns[uDF1Text];
                                if (UDF1Index != null && UDF1Index != undefined) {
                                    if (uDF1IsRequired == 'True') {
                                        o.abVisCols[UDF1Index] = true;
                                    }
                                    else if (o.abVisCols[UDF1Index] == false) {
                                        $("#tblPullCommonUDF th:contains('" + uDF1Text + "')").html('');
                                        $('#UDF1PullCommon').closest('td').html('');
                                        InvisibleUDF++;
                                    }
                                }
                                else
                                    InvisibleUDF++;

                                UDF2Index = objColumns[uDF2Text];
                                if (UDF2Index != null && UDF2Index != undefined) {
                                    if (uDF2IsRequired == 'True') {
                                        o.abVisCols[UDF2Index] = true;
                                    }
                                    else if (o.abVisCols[UDF2Index] == false) {

                                        $("#tblPullCommonUDF th:contains('" + uDF2Text + "')").html('');
                                        $('#UDF2PullCommon').closest('td').html('');
                                        InvisibleUDF++;
                                    }
                                }
                                else
                                    InvisibleUDF++;

                                UDF3Index = objColumns[uDF3Text];
                                if (UDF3Index != null && UDF3Index != undefined) {
                                    if (uDF3IsRequired == 'True') {
                                        o.abVisCols[UDF3Index] = true;
                                    }
                                    else if (o.abVisCols[UDF3Index] == false) {

                                        $("#tblPullCommonUDF th:contains('" + uDF3Text + "')").html('');
                                        $('#UDF3PullCommon').closest('td').html('');
                                        InvisibleUDF++;
                                    }
                                }
                                else
                                    InvisibleUDF++;

                                UDF4Index = objColumns[uDF4Text];
                                if (UDF4Index != null && UDF4Index != undefined) {
                                    if (uDF4IsRequired == 'True') {
                                        o.abVisCols[UDF4Index] = true;
                                    }
                                    else if (o.abVisCols[UDF4Index] == false) {

                                        $("#tblPullCommonUDF th:contains('" + uDF4Text + "')").html('');
                                        $('#UDF4PullCommon').closest('td').html('');
                                        InvisibleUDF++;
                                    }
                                }
                                else
                                    InvisibleUDF++;

                                UDF5Index = objColumns[uDF5Text];
                                if (UDF5Index != null && UDF5Index != undefined) {
                                    if (uDF5IsRequired == 'True') {
                                        o.abVisCols[UDF5Index] = true;
                                    }
                                    else if (o.abVisCols[UDF5Index] == false) {

                                        $("#tblPullCommonUDF th:contains('" + uDF5Text + "')").html('');
                                        $('#UDF5PullCommon').closest('td').html('');
                                        InvisibleUDF++;
                                    }
                                }
                                else
                                    InvisibleUDF++;

                                //----------------------------
                                //
                                setTimeout(function () {
                                    if (InvisibleUDF == 7) {
                                        $('#tblPullCommonUDF').hide();
                                        $('.dataTables_length').attr('style', 'left:0;top:-35px !important');
                                        $('.dataTables_paginate').attr('style', 'left: 145px;top:-24px !important');
                                    }
                                    else {
                                        $('.dataTables_length').attr('style', 'left:0;top:-100px !important');
                                        $('.dataTables_paginate').attr('style', 'left: 145px;top:-90px !important');
                                        //$(".pull-credit-popup .dataTables_paginate").css("top", "-90px");
                                        //$(".pull-credit-popup .dataTables_paginate").css("top", "-100px");
                                    }
                                }, 500);
                            }
                        }
                    });

                    return o;
                },
                "bServerSide": true,
                "sAjaxSource": _NewConsumePull.urls.AjaxURLToFillItemGridUrl, //'@Url.Content(Model.AjaxURLToFillItemGrid)', //'@Url.Content("~/Inventory/ItemMasterListAjax")',
                "fnServerData": self.grid_fnServerData,
                "fnInitComplete": function () {

                    $('.ColVis').detach().appendTo(".setting-arrow");
                    $('#divQTYLegends').show(1000);

                },
                "aoColumns": PullItemListColumnsArr,
                "fnDrawCallback": function () {
                    SetPullValuesAfterBarcodeSearch();
                }
            });

            if (isCost == 'False') {

                HideColumnUsingClassName(self.gridId);
            }

            $ItemModeDataTable.on("tap click", "tbody tr", function (e) {
                if (e.target.type == "checkbox" || e.target.type == "radio" || e.target.type == "text") {
                    e.stopPropagation();
                }
                else if (e.currentTarget.getElementsByTagName("input").btnLoad != undefined) {
                    e.stopPropagation();
                }
                else {
                    //$(this).toggleClass('row_selected');
                    if (!lastCheckedNewPull) {
                        lastCheckedNewPull = this;
                    }

                    if (e.shiftKey) {
                        var start = $('#ItemModeDataTable tbody tr').index(this);
                        var end = $('#ItemModeDataTable tbody tr').index(lastCheckedNewPull);

                        for (i = Math.min(start, end); i <= Math.max(start, end); i++) {
                            if (!$('#ItemModeDataTable tbody tr').eq(i).hasClass('row_selected')) {
                                $('#ItemModeDataTable tbody tr').eq(i).addClass("row_selected");
                            }
                        }
                        if (window.getSelection) {
                            if (window.getSelection().empty) {  // Chrome
                                window.getSelection().empty();
                            } else if (window.getSelection().removeAllRanges) {  // Firefox
                                window.getSelection().removeAllRanges();
                            }
                        } else if (document.selection) {  // IE?
                            document.selection.empty();
                        }
                    } else if ((e.metaKey || e.ctrlKey)) {
                        $(this).toggleClass('row_selected');
                    } else {
                        $(this).toggleClass('row_selected');
                    }

                    lastCheckedNewPull = this;
                }
                return false;
            });


            $ItemModeDataTable.live('contextmenu', function (e) {
                /* IF PRINT PREVIEW DONT SHOW CONTEXT MENU */
                if ($("body").hasClass('DTTT_Print')) {
                    e.preventDefault();
                    return false;
                }
                /* IF PRINT PREVIEW DONT SHOW CONTEXT MENU */

                if ($('#tab1')[0].className == "verticalText selected") {
                    var x = e.pageX - this.offsetLeft;
                    var y = e.pageY - this.offsetTop;
                    $('#divContextMenu1').css({ 'top': e.pageY, 'left': e.pageX }).fadeIn('slow');
                    e.preventDefault();
                }
            });
            $('.DTTT_container').css('z-index', '-1');
            if (tmpdWorkOrderGUID != "") {
                $('.DTTT_container').hide();
            }

            $('#ItemModeDataTable, #tblPullCommonUDF').on('focus', "input.AutoPullProjectSpents", function (e) {
                var ajaxURL = '/Pull/GetProjectSpentListForNewPULL';
                var tr = $(this).parent().parent().parent();
                //var itemData = _NewConsumePull.getItemDataFromRow($(tr));
                //var itmGuid = itemData.spnItemGUID; //$(tr).find('#spnItemGUID').text();
                var stagName = '';

                var $this = $(this);
                var varProjectName = '';
                _AutoCompleteWrapper.init($this
                    , ajaxURL
                    , function (request) {
                        var obj = JSON.stringify({ 'NameStartWith': request.term })
                        varProjectName = request.term ;
                        return obj;
                    },
                    function (data) {
                        _NewConsumePull.setDataFromRow($this.parent(), 'ProjectID', '');
                        if ($this.parent().find('#ProjectIDCommon') != undefined) {
                            $this.parent().find('#ProjectIDCommon').val(''); 
                        }
                        return $.map(data, function (Items) {
                        $.each(data, function (index, Items) {
                           if(Items.Value == varProjectName && Items.Key == varProjectName) 
                           {
                                _NewConsumePull.setDataFromRow($this.parent(), 'ProjectID', Items.GUID);
                                if ($this.parent().find('#ProjectIDCommon') != undefined) {
                                    $this.parent().find('#ProjectIDCommon').val(Items.GUID);
                                }
                           }
                        });

                            return {
                                label: Items.Value,
                                value: Items.Key,
                                id: Items.GUID
                            }
                        })
                    }
                    , function (curVal, selectedItem) {
                        //$this.val(ui.item.value);
                        if ($.trim(selectedItem.value).length > 0) {

                            //if ($this.parent().find('#ProjectID') != undefined) {
                            //    $this.parent().find('#ProjectID').val(selectedItem.id);
                            //}

                            _NewConsumePull.setDataFromRow($this.parent(), 'ProjectID', selectedItem.id);

                            if ($this.parent().find('#ProjectIDCommon') != undefined) {
                                $this.parent().find('#ProjectIDCommon').val(selectedItem.id);
                            }
                        }
                        else {
                            //$this.parent().find('#ProjectID').val('');
                            _NewConsumePull.setDataFromRow($this.parent(), 'ProjectID', '');
                            $this.parent().find('#ProjectIDCommon').val('');
                        }
                    }
                    , function (selectedItem) {
                        //if (selectedItem != null && selectedItem != undefined && $.trim(selectedItem.id).length > 0) {
                        //    if ($this.parent().find('#ProjectID') != undefined) {
                        //        $this.parent().find('#ProjectID').val(selectedItem.id);
                        //    }
                        //    if ($this.parent().find('#ProjectIDCommon') != undefined) {
                        //        $this.parent().find('#ProjectIDCommon').val(selectedItem.id);
                        //    }
                        //}
                        //else {
                        //    $this.parent().find('#ProjectID').val('');
                        //    $this.parent().find('#ProjectIDCommon').val('');
                        //}
                    }
                    , true,true);

                //    $(this).autocomplete({
                //        source: function (request, response) {
                //            $('#DivLoading').show()
                //            $.ajax({
                //                url: ajaxURL,
                //                type: 'POST',
                //                data: JSON.stringify({ 'NameStartWith': request.term }),
                //                contentType: 'application/json',
                //                dataType: 'json',
                //                success: function (data) {
                //                    $('#DivLoading').hide()
                //                    response($.map(data, function (Items) {

                //                        return {
                //                            label: Items.Value,
                //                            value: Items.Key,
                //                            id: Items.GUID
                //                        }
                //                    }));
                //                },
                //                error: function (err) {
                //                    $('#DivLoading').hide();
                //                }
                //            });
                //        },
                //        autoFocus: false,
                //        minLength: 1,
                //        select: function (event, ui) {
                //            $(this).val(ui.item.value);
                //            if ($.trim(ui.item.value).length > 0) {
                //                if ($(this).parent().find('#ProjectID') != undefined)
                //                    $(this).parent().find('#ProjectID').val(ui.item.id);
                //                if ($(this).parent().find('#ProjectIDCommon') != undefined)
                //                    $(this).parent().find('#ProjectIDCommon').val(ui.item.id);
                //            }
                //            else {
                //                $(this).parent().find('#ProjectID').val('');
                //                $(this).parent().find('#ProjectIDCommon').val('');
                //            }
                //        },
                //        open: function () {
                //            $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
                //        },
                //        close: function () {
                //            $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
                //        },
                //        change: function (event, ui) {
                //            if (ui.item != null && ui.item != undefined && $.trim(ui.item.id).length > 0) {
                //                if ($(this).parent().find('#ProjectID') != undefined)
                //                    $(this).parent().find('#ProjectID').val(ui.item.id);
                //                if ($(this).parent().find('#ProjectIDCommon') != undefined)
                //                    $(this).parent().find('#ProjectIDCommon').val(ui.item.id);
                //            }
                //            else {
                //                $(this).parent().find('#ProjectID').val('');
                //                $(this).parent().find('#ProjectIDCommon').val('');
                //            }
                //        }
                //    });
            });// focus


            $ItemModeDataTable.on('focus', "input.AutoPullBins", function (e) {
                
                var ajaxURL = '/Pull/GetItemLocationsForNewPullGrid';
                var tr = $(this).parent().parent().parent();
               
                //var stagName = '';
                var $this = $(this);

                _AutoCompleteWrapper.init($this
                    , ajaxURL
                    , function (request) {
                        var IsStagingHeaderSelected = false;
                        if (typeof IsStagingLocationOnly != 'undefined') {
                            IsStagingHeaderSelected = IsStagingLocationOnly;
                        }
                        var itemData = _NewConsumePull.getItemDataFromRow($(tr), 'spnItemGUID');
                        var itmGuid = itemData.spnItemGUID; //$(tr).find('#spnItemGUID').text();
                        var obj = JSON.stringify({
                            'ItemGuid': itmGuid,
                            'NameStartWith': request.term,
                            'IsStagingHeaderSelected': IsStagingHeaderSelected
                        });
                        return obj;
                    },
                    function (data) {
                        if (data.isNewBinCreated == true && data.NewBinID > 0) {
                            self.setDataFromRow($this.parent(), 'BinID', data.NewBinID);
                        }
                        return $.map(data.returnKeyValList, function (Items) {
                            //self.setDataFromRow($this.parent(), 'BinID', Items.ID);
                            //if (parseFloat(Items.OtherInfo1) > 0) {
                            //    $this.parents('tr').find('#txtQty').val(Items.OtherInfo1);
                            //}
                            //else {
                            //    var dfQty = self.getDataFromRow($this.parent(), 'hdnDPQ');  //$this.parent().find('#hdnDPQ').val();
                            //    $this.parents('tr').find('#txtQty').val(dfQty);
                            //}
                            return {
                                label: Items.Value,
                                value: Items.Value,
                                id: Items.ID,
                                oinfo: Items.OtherInfo1
                            }
                        });
                    }
                    , function (curVal, selectedItem) {
                        //$this.val(ui.item.value);
                        if (selectedItem != null && selectedItem != undefined
                            && $.trim(selectedItem.id).length > 0 && $.trim(selectedItem.value).length > 0) {
                            //$this.parent().find('#BinID').val(selectedItem.id);
                            self.setDataFromRow($this.parent(), 'BinID', selectedItem.id);
                            if (parseFloat(selectedItem.oinfo) > 0) {
                                $this.parents('tr').find('#txtQty').val(selectedItem.oinfo);
                            }
                            else {
                                var dfQty = self.getDataFromRow($this.parent(), 'hdnDPQ');  //$this.parent().find('#hdnDPQ').val();
                                $this.parents('tr').find('#txtQty').val(dfQty);
                            }
                        }
                        else {
                            //$this.parent().find('#BinID').val('');
                            self.setDataFromRow($this.parent(), 'BinID', '');
                        }
                    }
                    , function (selectedItem) {

                    }
                    , true,true);


                //$(this).autocomplete({
                //    source: function (request, response) {
                //        $('#DivLoading').show()
                //        $.ajax({
                //            url: ajaxURL,
                //            type: 'POST',
                //            data: JSON.stringify({ 'ItemGuid': itmGuid, 'NameStartWith': request.term, 'IsStagingHeaderSelected': IsStagingHeaderSelected }),
                //            contentType: 'application/json',
                //            dataType: 'json',
                //            success: function (data) {
                //                $('#DivLoading').hide()

                //                response($.map(data, function (Items) {
                //                    return {
                //                        label: Items.Value,
                //                        value: Items.Value,
                //                        id: Items.ID,
                //                        oinfo: Items.OtherInfo1
                //                    }
                //                }));
                //            },
                //            error: function (err) {
                //                $('#DivLoading').hide();
                //            }
                //        });
                //    },
                //    autoFocus: false,
                //    minLength: 1,
                //    select: function (event, ui) {
                //        $(this).val(ui.item.value);
                //        if (ui.item != null && ui.item != undefined && $.trim(ui.item.id).length > 0 && $.trim(ui.item.value).length > 0) {
                //            $(this).parent().find('#BinID').val(ui.item.id);
                //            if (parseFloat(ui.item.oinfo) > 0) {
                //                $(this).parents('tr').find('#txtQty').val(ui.item.oinfo);
                //            }
                //            else {
                //                var dfQty = $(this).parent().find('#hdnDPQ').val();
                //                $(this).parents('tr').find('#txtQty').val(dfQty);
                //            }
                //        }
                //        else {
                //            $(this).parent().find('#BinID').val('');
                //        }
                //    },
                //    open: function () {
                //        $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
                //    },
                //    close: function () {
                //        $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
                //    },
                //    change: function (event, ui) {
                //        //                    if (ui.item != null && ui.item != undefined && $.trim(ui.item.id).length > 0) {
                //        //                        $(this).parent().find('#BinID').val(ui.item.id);
                //        //                    }
                //        //                    else {
                //        //                        $(this).parent().find('#BinID').val('');
                //        //                    }
                //    }
                //});
            });

            $ItemModeDataTable.on('focus', "input.AutoCreditBins", function (e) {
                var tr = $(this).parent().parent().parent();
                
                //var stagName = '';
                //var hdnIsLoadMoreLocations = self.getDataFromRow($(tr), 'hdnIsLoadMoreLocations'); //$(tr).find("#hdnIsLoadMoreLocations").val();
                                
                var $this = $(this);
                
                _AutoCompleteWrapper.init($this
                    , '/Master/GetBinsOfItem'
                    , function (request) {
                        var qtyRequired = false;
                        var itemData = _NewConsumePull.getItemDataFromRow($(tr));
                        var itmGuid = itemData.spnItemGUID; //$(tr).find('#spnItemGUID').text();

                        var obj = {
                            'StagingName': '', 'NameStartWith': request.term,
                            'ItemGUID': itmGuid,
                            'QtyRequired': qtyRequired,
                            'IsLoadMoreLocations': self.getDataFromRow($(tr), 'hdnIsLoadMoreLocations')
                        };
                        return obj;
                    },
                    function (data) {
                        return $.map(data, function (Items) {
                            return {
                                label: Items.Key,
                                value: Items.Value,
                                selval: Items.Value
                            }
                        })
                    }
                    , function (curVal, selectedItem) {
                        //$this.val(ui.item.value);
                        if (selectedItem.selval == MoreLocation) {
                            //$(tr).find("#hdnIsLoadMoreLocations").val("true");
                            self.setDataFromRow($(tr), 'hdnIsLoadMoreLocations', 'true');
                            $this.trigger("focus");
                            $this.autocomplete("search", " ");
                            return false;
                        }
                        //else {
                        //    $this.val(selectedItem.value);
                        //}
                    }
                    , function (selectedItem) {

                    }
                    , false,true);

                //$(this).autocomplete({
                //    source: function (request, response) {
                //        $('#DivLoading').show();
                //        var qtyRequired = false;

                //        $.ajax({
                //            url: '/Master/GetBinsOfItem',
                //            //type: 'POST',
                //            data: { 'StagingName': '', 'NameStartWith': request.term, 'ItemGUID': itmGuid, 'QtyRequired': qtyRequired, 'IsLoadMoreLocations': hdnIsLoadMoreLocations },
                //            contentType: 'application/json',
                //            dataType: 'json',
                //            success: function (data) {
                //                $('#DivLoading').hide();
                //                response($.map(data, function (Items) {
                //                    return {
                //                        label: Items.Key,
                //                        value: Items.Value,
                //                        selval: Items.Value
                //                    }
                //                }));
                //            },
                //            error: function (err) {
                //                $('#DivLoading').hide();
                //            }
                //        });
                //    },
                //    autoFocus: false,
                //    minLength: 1,
                //    select: function (event, ui) {

                //        if (ui.item.selval == "More Locations") {
                //            $(tr).find("#hdnIsLoadMoreLocations").val("true");
                //            $(this).trigger("focus");
                //            $(this).autocomplete("search", " ");
                //            return false;
                //        }
                //        else {
                //            $(this).val(ui.item.value);
                //        }

                //    },
                //    open: function () {
                //        $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
                //        $(this).autocomplete('widget').css('z-index', 9000);
                //        $('ul.ui-autocomplete').css('overflow-y', 'auto');
                //        $('ul.ui-autocomplete').css('max-height', '300px');
                //    },
                //    close: function () {
                //        $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
                //    },
                //    change: function (event, ui) {
                //    }
                //});
            });

            $('#ItemModeDataTable, #tblPullCommonUDF').on('focus', "input.AutoMSCreditStagingHeader", function (e) {

                var tr = $(this).parent().parent().parent();
                //var itmGuid = _NewConsumePull.getItemDataFromRow($(tr)).spnItemGUID; //$(tr).find('#spnItemGUID').text();
                var stagName = '';


                var $this = $(this);

                _AutoCompleteWrapper.init($this
                    , '/Inventory/GetAllStagingHeaders'
                    , function (request) {
                        var obj = { NameStartWith: request.term };
                        return obj;
                    },
                    function (data) {
                        return $.map(data, function (Items) {
                            return {
                                label: Items.StagingName,
                                value: Items.StagingName,
                                selval: Items.GUID,
                                selID: Items.ID
                            }
                        });
                    }
                    , function (curVal, selectedItem) {
                        //$this.val(ui.item.value);
                        if ($("#tblPullCommonUDF").find("#MSPullStagingHeaderValue").length > 0) {
                            $("#tblPullCommonUDF").find("#MSPullStagingHeaderValue").val(selectedItem.selval);
                        }
                        $(tr).find("#hdnIsLoadMoreStaging").val(selectedItem.selval);
                        _NewConsumePull.setDataFromRow($(tr), 'hdnIsLoadMoreStaging', selectedItem.selval)
                        $this.val(selectedItem.value);

                    }
                    , function (selectedItem) {

                    }
                    , false,true);


                //$(this).autocomplete({
                //    source: function (request, response) {
                //        $('#DivLoading').show();
                //        var qtyRequired = false;

                //        $.ajax({
                //            url: '/Inventory/GetAllStagingHeaders',
                //            contentType: 'application/json',
                //            dataType: 'json',
                //            data: { NameStartWith: request.term },
                //            success: function (data) {
                //                $('#DivLoading').hide();
                //                response($.map(data, function (Items) {
                //                    return {
                //                        label: Items.StagingName,
                //                        value: Items.StagingName,
                //                        selval: Items.GUID,
                //                        selID: Items.ID
                //                    }
                //                }));
                //            },
                //            error: function (err) {
                //                $('#DivLoading').hide();
                //            }
                //        });
                //    },
                //    autoFocus: false,
                //    minLength: 1,
                //    select: function (event, ui) {

                //        if ($("#tblPullCommonUDF").find("#MSPullStagingHeaderValue").length > 0) {
                //            $("#tblPullCommonUDF").find("#MSPullStagingHeaderValue").val(ui.item.selval);
                //        }
                //        $(tr).find("#hdnIsLoadMoreStaging").val(ui.item.selval);
                //        $(this).val(ui.item.value);
                //    },
                //    open: function () {
                //        $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
                //        $(this).autocomplete('widget').css('z-index', 9000);
                //        $('ul.ui-autocomplete').css('overflow-y', 'auto');
                //        $('ul.ui-autocomplete').css('max-height', '300px');
                //    },
                //    close: function () {
                //        $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
                //    },
                //    change: function (event, ui) {
                //    }
                //});
            });

            $ItemModeDataTable.off('change', "input[type='text'][name^='txtMSStagingHeader']");
            $ItemModeDataTable.on('change', "input[type='text'][name^='txtMSStagingHeader']", function (e) {
                var objCurtxt = $(this);
                var CurrentValue = $(objCurtxt).val();
                var vItemGUID = $(this).prop("id").split('_')[1];
                var BinID = $("#hdnBinIDValue").val();

                if ($.trim(CurrentValue) != null && $.trim(CurrentValue) != "") {
                    $.ajax({
                        type: "POST",
                        url: SaveMaterialStagingUrl,
                        contentType: 'application/json',
                        dataType: 'json',
                        data: "{ BinID: '" + 0 + "' , StagingName: '" + $.trim(CurrentValue) + "'}",
                        success: function (result) {
                            if (result.Status == "Success") {
                                if (result.MaterialStagingGuid != "") {
                                    //$ItemModeDataTable.find("#hdnIsLoadMoreStaging").val(result.MaterialStagingGuid);
                                    //self.setDataFromTable($ItemModeDataTable, 'hdnIsLoadMoreStaging');

                                    var $tr = objCurtxt.closest("tr"); // parent tr
                                    self.setDataFromTable($ItemModeDataTable, 'hdnIsLoadMoreStaging', result.MaterialStagingGuid);
                                    //self.setDataFromTable($tr, 'hdnIsLoadMoreStaging', result.MaterialStagingGuid); // set value
                                }
                            }
                        },
                        error: function (err) {
                            console.log(err);
                        }
                    });
                }
            });


            $ItemModeDataTable.on('focus', "input.AutoMSCreditBins", function (e) {
                var tr = $(this).parent().parent().parent();
                
                var $this = $(this);

                _AutoCompleteWrapper.init($this
                    , '/Master/GetBinsOfItem'
                    , function (request) {
                        var itmGuid = _NewConsumePull.getItemDataFromRow($(tr)).spnItemGUID;  //$(tr).find('#spnItemGUID').text();

                        var hdnIsLoadMoreLocations = self.getDataFromRow($(tr), 'hdnIsLoadMoreLocations'); //$(tr).find("#hdnIsLoadMoreLocations").val();
                        var qtyRequired = false;

                        var obj = {
                            'StagingName': 'GetAllStagLocationsOfItems',
                            'NameStartWith': request.term,
                            'ItemGUID': itmGuid,
                            'QtyRequired': qtyRequired,
                            'IsLoadMoreLocations': hdnIsLoadMoreLocations
                        };
                        return obj;
                    },
                    function (data) {
                        return $.map(data, function (Items) {
                            return {
                                label: Items.Key,
                                value: Items.Value,
                                selval: Items.Value
                            }
                        });
                    }
                    , function (curVal, selectedItem) {
                        //$this.val(ui.item.value);
                        if (selectedItem.selval == MoreLocation) {
                            //$(tr).find("#hdnIsLoadMoreLocations").val("true");
                            self.setDataFromRow($(tr), 'hdnIsLoadMoreLocations', 'true');
                            $this.trigger("focus");
                            $this.autocomplete("search", " ");
                            return false;
                        }
                        //else {
                        //    $this.val(selectedItem.value);
                        //}

                    }
                    , function (selectedItem) {

                    }
                    , false,true);


                //$(this).autocomplete({
                //    source: function (request, response) {
                //        $('#DivLoading').show();
                //        var qtyRequired = false;

                //        $.ajax({
                //            url: '/Master/GetBinsOfItem',
                //            //type: 'POST',
                //            data: { 'StagingName': 'GetAllStagLocationsOfItems', 'NameStartWith': request.term, 'ItemGUID': itmGuid, 'QtyRequired': qtyRequired, 'IsLoadMoreLocations': hdnIsLoadMoreLocations },
                //            contentType: 'application/json',
                //            dataType: 'json',
                //            success: function (data) {
                //                $('#DivLoading').hide();
                //                response($.map(data, function (Items) {
                //                    return {
                //                        label: Items.Key,
                //                        value: Items.Value,
                //                        selval: Items.Value
                //                    }
                //                }));
                //            },
                //            error: function (err) {
                //                $('#DivLoading').hide();
                //            }
                //        });
                //    },
                //    autoFocus: false,
                //    minLength: 1,
                //    select: function (event, ui) {

                //        if (ui.item.selval == "More Locations") {
                //            $(tr).find("#hdnIsLoadMoreLocations").val("true");
                //            $(this).trigger("focus");
                //            $(this).autocomplete("search", " ");
                //            return false;
                //        }
                //        else {
                //            $(this).val(ui.item.value);
                //        }

                //    },
                //    open: function () {
                //        $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
                //        $(this).autocomplete('widget').css('z-index', 9000);
                //        $('ul.ui-autocomplete').css('overflow-y', 'auto');
                //        $('ul.ui-autocomplete').css('max-height', '300px');
                //    },
                //    close: function () {
                //        $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
                //    },
                //    change: function (event, ui) {
                //    }
                //});
            });

            $('#ItemModeDataTable, #tblPullCommonUDF').on('focus', "input.AutoPullOrderNumber", function (e) {
                var ajaxURL = '/Pull/GetPullOrderNumberForNewPullGrid';
                var tr = $(this).parent().parent().parent();
                //var itmGuid = _NewConsumePull.getItemDataFromRow($(tr)).spnItemGUID; //$(tr).find('#spnItemGUID').text();

                var $this = $(this);

                _AutoCompleteWrapper.init($this
                    , ajaxURL
                    , function (request) {
                        var obj = JSON.stringify({ 'NameStartWith': request.term });
                        return obj;
                    },
                    function (data) {
                        return $.map(data, function (Items) {
                            return {
                                label: Items.Value,
                                value: Items.Value
                            }
                        })
                    }
                    , function (curVal, selectedItem) {
                        //        $(this).val(ui.item.value);
                    }
                    , function (selectedItem) {

                    }
                    , true,true);

                //$(this).autocomplete({
                //    source: function (request, response) {
                //        $('#DivLoading').show()
                //        $.ajax({
                //            url: ajaxURL,
                //            type: 'POST',
                //            data: JSON.stringify({ 'NameStartWith': request.term }),
                //            contentType: 'application/json',
                //            dataType: 'json',
                //            success: function (data) {
                //                $('#DivLoading').hide()

                //                response($.map(data, function (Items) {
                //                    return {
                //                        label: Items.Value,
                //                        value: Items.Value
                //                    }
                //                }));
                //            },
                //            error: function (err) {
                //                $('#DivLoading').hide();
                //            }
                //        });
                //    },
                //    autoFocus: false,
                //    minLength: 1,
                //    select: function (event, ui) {
                //        $(this).val(ui.item.value);
                //    },
                //    open: function () {
                //        $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
                //        $(this).autocomplete('widget').css('z-index', 9000);
                //        $('ul.ui-autocomplete').css('overflow-y', 'auto');
                //        $('ul.ui-autocomplete').css('max-height', '300px');
                //    },
                //    close: function () {
                //        $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
                //    },
                //    change: function (event, ui) {

                //    }
                //});

            });

            $('#ItemModeDataTable').on('focus', "input.AutoSupplierAccountNumber", function (e) {
                var ajaxURL = '/Pull/GetSupplierAccountNumbersforPull';
                var $this = $(this);
                var tr = $this.parent().parent().parent();
                
                

                _AutoCompleteWrapper.init($this
                    , ajaxURL
                    , function (request) {
                        var itemData = _NewConsumePull.getItemDataFromRow($(tr));
                        var itmGuid = itemData.spnItemGUID; //$(tr).find('#spnItemGUID').text();
                        var obj = JSON.stringify({ 'ItemGuid': itmGuid, 'NameStartWith': request.term });
                        return obj;
                    },
                    function (data) {
                         return $.map(data, function (Items) {
                                    return {
                                        label: Items.Value,
                                        value: Items.Value,
                                        selval: Items.GUID
                                    }
                                });
                    }
                    , function (curVal, selectedItem) {
                        $(tr).find("#hdnSupplierAccountNumber").val(selectedItem.selval);
                        //$this.val(selectedItem.value);
                    }
                    , function (selectedItem) {

                    }
                    , true, true);

                //$(this).autocomplete({
                //    source: function (request, response) {
                //        $('#DivLoading').show()
                //        $.ajax({
                //            url: ajaxURL,
                //            type: 'POST',
                //            data: JSON.stringify({ 'ItemGuid': itmGuid, 'NameStartWith': request.term }),
                //            contentType: 'application/json',
                //            dataType: 'json',
                //            success: function (data) {
                //                $('#DivLoading').hide()

                //                response($.map(data, function (Items) {
                //                    return {
                //                        label: Items.Value,
                //                        value: Items.Value,
                //                        selval: Items.GUID
                //                    }
                //                }));
                //            },
                //            error: function (err) {
                //                $('#DivLoading').hide();
                //            }
                //        });
                //    },
                //    autoFocus: false,
                //    minLength: 1,
                //    select: function (event, ui) {
                //        $(tr).find("#hdnSupplierAccountNumber").val(ui.item.selval);
                //        $(this).val(ui.item.value);
                //    },
                //    open: function () {
                //        $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
                //        $(this).autocomplete('widget').css('z-index', 9000);
                //        $('ul.ui-autocomplete').css('overflow-y', 'auto');
                //        $('ul.ui-autocomplete').css('max-height', '300px');
                //    },
                //    close: function () {
                //        $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
                //    },
                //    change: function (event, ui) {

                //    }
                //});

            });

            UDFfillEditableOptionsForGridNewPull();

            //$('.ShowAllOptions').unbind("click");
            $('#' + self.gridId).on("click", ".ShowAllOptions", function () {
                var ddl = $(this).siblings('.AutoPullProjectSpents');
                _AutoCompleteWrapper.searchHide(ddl);

                //$(this).siblings('.AutoPullProjectSpents').trigger("focus");
                //$(this).siblings(".AutoPullProjectSpents").autocomplete("search", " ");
            });

            $('#' + self.gridUseThisId).on("click", ".ShowAllOptions", function () {
                var ddl = $(this).siblings('.AutoPullProjectSpents');
                _AutoCompleteWrapper.searchHide(ddl);

                //$(this).siblings('.AutoPullProjectSpents').trigger("focus");
                //$(this).siblings(".AutoPullProjectSpents").autocomplete("search", " ");
            });


            //$('.ShowAllOptionsBin').unbind("click");
            $('#' + self.gridId).on("click", ".ShowAllOptionsBin", function () {
                var ddl = $(this).siblings('.AutoPullBins');
                _AutoCompleteWrapper.searchHide(ddl);
                //$(this).siblings('.AutoPullBins').trigger("focus");
                //$(this).siblings(".AutoPullBins").autocomplete("search", " ");
            });

            $('#' + self.gridUseThisId).on("click", ".ShowAllOptionsBin", function () {
                var ddl = $(this).siblings('.AutoPullBins');
                _AutoCompleteWrapper.searchHide(ddl);
                //$(this).siblings('.AutoPullBins').trigger("focus");
                //$(this).siblings(".AutoPullBins").autocomplete("search", " ");
            });

            //$('.ShowAllOptionsBinCR').unbind("click");
            $('#' + self.gridId).on("click", ".ShowAllOptionsBinCR", function () {
                var ddl = $(this).siblings('.AutoCreditBins');
                _AutoCompleteWrapper.searchHide(ddl);
                //$(this).siblings('.AutoCreditBins').trigger("focus");
                //$(this).siblings(".AutoCreditBins").autocomplete("search", " ");
            });

            $('#' + self.gridUseThisId).on("click", ".ShowAllOptionsBinCR", function () {
                var ddl = $(this).siblings('.AutoCreditBins');
                _AutoCompleteWrapper.searchHide(ddl);
                //$(this).siblings('.AutoCreditBins').trigger("focus");
                //$(this).siblings(".AutoCreditBins").autocomplete("search", " ");
            });

            //$('.ShowAllOptionsBinMCR').unbind("click");
            $('#' + self.gridId).on("click", ".ShowAllOptionsBinMCR", function () {
                var ddl = $(this).siblings('.AutoMSCreditBins');
                _AutoCompleteWrapper.searchHide(ddl);
                //$(this).siblings('.AutoMSCreditBins').trigger("focus");
                //$(this).siblings(".AutoMSCreditBins").autocomplete("search", " ");
            });

            //    $('.ShowAllOptionsStagingHeader').unbind("click");
            //$('#' + self.gridId).on("click", ".ShowAllOptionsStagingHeader", function () {
            //    var ddl = $(this).siblings('.AutoMSCreditStagingHeader');
            //    _AutoCompleteWrapper.searchHide(ddl);
            //    //$(this).siblings('.AutoMSCreditStagingHeader').trigger("focus");
            //    //$(this).siblings(".AutoMSCreditStagingHeader").autocomplete("search", " ");
            //});



            $('#' + self.gridId).on("click", ".ShowAllOptionsOrderNumber", function () {
                var ddl = $(this).siblings('.AutoPullOrderNumber');
                _AutoCompleteWrapper.searchHide(ddl);
                //$(this).siblings('.AutoPullOrderNumber').trigger("focus");
                //$(this).siblings(".AutoPullOrderNumber").autocomplete("search", " ");
            });

            //$('.ShowAllOptionsOrderNumber').unbind("click");
            $('#' + self.gridUseThisId).on("click", ".ShowAllOptionsOrderNumber", function () {
                var ddl = $(this).siblings('.AutoPullOrderNumber');
                _AutoCompleteWrapper.searchHide(ddl);
                //$(this).siblings('.AutoPullOrderNumber').trigger("focus");
                //$(this).siblings(".AutoPullOrderNumber").autocomplete("search", " ");
            });

            //$('.ShowAllOptionsSupplierAccountNumber').click(function () {
            $('#' + self.gridId).on("click", ".ShowAllOptionsSupplierAccountNumber", function () {
                var ddl = $(this).siblings('.AutoSupplierAccountNumber');
                _AutoCompleteWrapper.searchHide(ddl);
                //$(this).siblings('.AutoSupplierAccountNumber').trigger("focus");
                //$(this).siblings(".AutoSupplierAccountNumber").autocomplete("search", " ");
            });
                      

            //$('.bin-input-readonly').unbind("keypress");
            $('#' + self.gridId).on("keypress", ".bin-input-readonly", function () {
                //$('.bin-input-readonly').keypress(function () {
                return false;
            });

            $('#' + self.gridUseThisId).on("keypress", ".bin-input-readonly", function () {
                //$('.bin-input-readonly').keypress(function () {
                return false;
            });


            if (hasOnTheFlyEntryRight == "False") {
                //$('.AutoCreditBins').unbind("keypress");
                $('#' + self.gridId).on("keypress", ".AutoCreditBins", function () {
                    //$('.AutoCreditBins').keypress(function () {
                    return false;
                });

                $('#' + self.gridUseThisId).on("keypress", ".AutoCreditBins", function () {
                    //$('.AutoCreditBins').keypress(function () {
                    return false;
                });


                //$('.AutoMSCreditBins').unbind("keypress");
                $('#' + self.gridId).on("keypress", ".AutoMSCreditBins", function () {
                    //$('.AutoMSCreditBins').keypress(function () {
                    return false;
                });

                $('#' + self.gridUseThisId).on("keypress", ".AutoMSCreditBins", function () {
                    //$('.AutoMSCreditBins').keypress(function () {
                    return false;
                });

                //$('.AutoMSCreditStagingHeader').unbind("keypress");
                $('#' + self.gridId).on("keypress", ".AutoMSCreditStagingHeader", function () {
                    //$('.AutoMSCreditStagingHeader').keypress(function () {
                    return false;
                });

                $('#' + self.gridUseThisId).on("keypress", ".AutoMSCreditStagingHeader", function () {
                    //$('.AutoMSCreditStagingHeader').keypress(function () {
                    return false;
                });
            }

            $('#' + self.gridId).on("focus", ".loadondemandudf", function (e) {
                var currentselectUDF = $(this);
                if ($(currentselectUDF).find('option').length < 2) {
                    var udfId = $(this).data('value');
                    var UDFdefaultvalue = $(this).data('udfdefaultvalue');
                    $.ajax({
                        'url': '/UDF/GetUDFOptionsByUDF',
                        data: { 'UDFID': udfId, 'UDFTableName': 'PullMaster' },
                        success: function (response) {
                            // start code
                            var s = '';
                            s += '<option></option>';
                            $.each(response.DDData, function (i, val) {
                                if (val.UDFOption == UDFdefaultvalue) {
                                    s += '<option value="' + val.ID + '" selected="selected">' + val.UDFOption + '</option>';
                                } else {
                                    s += '<option value="' + val.ID + '">' + val.UDFOption + '</option>';
                                }
                                //s += '<option value="' + val.ID + '">' + val.UDFOption + '</option>';
                            });
                            $(currentselectUDF).empty();
                            $(currentselectUDF).append(s);
                            
                            $(currentselectUDF).trigger('change');
                            //End code
                        }, error: function () {

                        }
                    });
                }
            });

        });//ready

    };

    self.getDataEleFromRow = function ($tr, dataAttr) {
        // get element from tr with data attribute
        var attr = "data-" + dataAttr;
        var elms = $tr.find("[" + attr + "]");
        if (elms.length) {
            return $(elms[0]);
        }
        return null;
    }

    self.getDataFromRow = function ($tr, dataAttr) {
        var attr = "data-" + dataAttr ;
        //var elms = $tr.find("[" + attr + "]");
        var elm = self.getDataEleFromRow($tr, dataAttr);
        //if (elms.length) {
        //var val = $(elms[0]).attr(attr);
        var val = $(elm).attr(attr);
        return val;
        //}
        //return '';
    };

    self.setDataFromRow = function ($tr, dataAttr, val) {
        // set data attribute on element on a row
        var attr = "data-" + dataAttr;
        //var elms = $tr.find("[" + attr + "]");

        var elm = self.getDataEleFromRow($tr, dataAttr);
        if (typeof elm !== 'undefined' && elm != null ) {
            $(elm).attr(attr, val);
        }
    };

    self.setDataFromTable = function ($table, dataAttr, val) {
        // set data attribute on all elements on table
        var attr = "data-" + dataAttr;
        $table.find("[" + attr + "]").attr(attr, val);        
        //if (elms.length) {
        //    $(elms).attr(attr, val);
        //}
    };


    self.grid_fnStateSaveParams = function (oSettings, oData) {
        if (self.isSaveGridState) {
            oData.oSearch.sSearch = "";

            _AjaxUtil.postJson(_NewConsumePull.urls.SaveGridStateUrl
                , { Data: JSON.stringify(oData), ListName: 'NewPullItemMaster' }
                , function (json) {
                    o = json;
                }, null, false, false);

            //$.ajax({
            //    "url": _NewConsumePull.urls.SaveGridStateUrl,
            //    "type": "POST",
            //    data: { Data: JSON.stringify(oData), ListName: 'NewPullItemMaster' },
            //    "async": false,
            //    cache: false,
            //    "dataType": "json",
            //    "success": function (json) {
            //        o = json;

            //    }
            //});
        }
        else {
            self.isSaveGridState = true;
        }
    };

    self.grid_fnServerData = function (sSource, aoData, fnCallback, oSettings) {
        //PostCount = PostCount + 1;
        var arrCols = new Array();
        var objCols = this.fnSettings().aoColumns;

        for (var i = 0; i <= objCols.length - 1; i++) {
            arrCols.push(objCols[i].mDataProp);
        }
        for (var j = 0; j <= aoData.length - 1; j++) {
            if (aoData[j].name == "sColumns") {
                aoData[j].value = arrCols.join("|");
                break;
            }
        }
        if (oSettings.aaSorting != null && oSettings.aaSorting.length != 0) {
            var sortValue = "";
            for (var i = 0; i <= oSettings.aaSorting.length - 1; i++) {
                if (sortValue.length > 0)
                    sortValue += ", "
                sortValue += arrCols[oSettings.aaSorting[i][0]] + ' ' + oSettings.aaSorting[i][1];
            }
            aoData.push({ "name": "SortingField", "value": sortValue });
        }
        //                    aoData.push({ "name": "SortingField", "value": oSettings.aaSorting[0][3] });
        else
            aoData.push({ "name": "SortingField", "value": "0" });

        aoData.push({ "name": "IsArchived", "value": $('#IsArchivedRecords').is(':checked') });
        aoData.push({ "name": "IsDeleted", "value": $('#IsDeletedRecords').is(':checked') });

        aoData.push({ "name": "ActionFilter", "value": $('#NewPullAction').val() });

        if (_Common.selectedGridOperation == _Common.gridOperations.Search
            || _Common.selectedGridOperation == _Common.gridOperations.IncludeDeleted
            || _Common.selectedGridOperation == _Common.gridOperations.IncludeArchived
            || _Common.selectedGridOperation == _Common.gridOperations.AutoRefresh
            || _Common.selectedGridOperation == _Common.gridOperations.PageChange
        ) {
            // prevent api calls
            self.isSaveGridState = false;
        }
        else if (_Common.selectedGridOperation == _Common.gridOperations.PageSizeChange) {

            self.isSaveGridState = true;
        }
        else if (_Common.selectedGridOperation == _Common.gridOperations.Sorting
            || _Common.selectedGridOperation == _Common.gridOperations.ColumnResize
        ) {

            self.isSaveGridState = true;
        }
        else if (_Common.selectedGridOperation == _Common.gridOperations.Refresh) {

            self.isSaveGridState = false;
        }

        hdnSuppPOPairVal.clear();
        self.gridRowStartIndex = null;

        oSettings.jqXHR = _AjaxUtil.postJson(sSource, aoData
            , function (json) {
                var bindJson = [];

                $.each(json.aaData, function (index, obj) {
                    //bindJson.push(obj);
                    bindJson.push(new newConsumePullDTO(obj));
                });
                delete json.aaData;
                json.aaData = bindJson;
                fnCallback(json);
            }
            ,null
            , true
            , false
            , function () {
                //$('#ItemModeDataTable td').removeHighlight();
                removeHighlight2(self.gridId);
                $('.dataTables_scroll').css({ "opacity": 0.2 });
            }
            , function () {

                if ($("#ItemModel_filter").val() != '') {
                    $('#ItemModeDataTable td').highlight($("#ItemModel_filter").val());
                }

                //$("input[type='radio']").filter('[value=pull]').attr('checked', 'checked');

                CallShowHideData();

                $(".text-boxQuantityFormat").priceFormat({
                    prefix: '',
                    thousandsSeparator: '',
                    centsLimit: parseInt($('#hdQuantitycentsLimit').val(), 10)
                });

                $(".text-boxQuantityFormatSR").priceFormat({
                    prefix: '',
                    thousandsSeparator: '',
                    centsLimit: 0
                });
                $('.dataTables_scroll').css({ "opacity": 1 });
              
            }
            , { "__RequestVerificationToken": $("input[name='__RequestVerificationToken'][type='hidden']").val() }
        );

        //oSettings.jqXHR = $.ajax({
        //    "dataType": 'json',
        //    "type": "POST",
        //    "url": sSource,
        //    cache: false,
        //    "data": aoData,
        //    "headers": { "__RequestVerificationToken": $("input[name='__RequestVerificationToken'][type='hidden']").val() },
        //    "success": fnCallback,
        //    beforeSend: function () {
        //        //$('#ItemModeDataTable td').removeHighlight();
        //        removeHighlight2(self.gridId);
        //        $('.dataTables_scroll').css({ "opacity": 0.2 });
        //    },
        //    complete: function () {
                

        //        if ($("#ItemModel_filter").val() != '') {
        //            $('#ItemModeDataTable td').highlight($("#ItemModel_filter").val());
        //        }

        //        //$("input[type='radio']").filter('[value=pull]').attr('checked', 'checked');

        //        CallShowHideData();

        //        $(".text-boxQuantityFormat").priceFormat({
        //            prefix: '',
        //            thousandsSeparator: '',
        //            centsLimit: parseInt($('#hdQuantitycentsLimit').val(), 10)
        //        });

        //        $(".text-boxQuantityFormatSR").priceFormat({
        //            prefix: '',
        //            thousandsSeparator: '',
        //            centsLimit: 0
        //        });
        //        $('.dataTables_scroll').css({ "opacity": 1 });

        //        //UDFfillEditableOptionsForGridNewPull();

        //        //$('.ShowAllOptions').unbind("click");
        //        //$('.ShowAllOptions').click(function () {
        //        //    var ddl = $(this).siblings('.AutoPullProjectSpents');
        //        //    _AutoCompleteWrapper.searchHide(ddl);

        //        //    //$(this).siblings('.AutoPullProjectSpents').trigger("focus");
        //        //    //$(this).siblings(".AutoPullProjectSpents").autocomplete("search", " ");
        //        //});

        //        //$('.ShowAllOptionsBin').unbind("click");
        //        //$('.ShowAllOptionsBin').click(function () {
        //        //    var ddl = $(this).siblings('.AutoPullBins');
        //        //    _AutoCompleteWrapper.searchHide(ddl);

        //        //    //$(this).siblings('.AutoPullBins').trigger("focus");
        //        //    //$(this).siblings(".AutoPullBins").autocomplete("search", " ");
        //        //});

        //        //$('.ShowAllOptionsBinCR').unbind("click");
        //        //$('.ShowAllOptionsBinCR').click(function () {
        //        //    var ddl = $(this).siblings('.AutoCreditBins');
        //        //    _AutoCompleteWrapper.searchHide(ddl);
        //        //    //$(this).siblings('.AutoCreditBins').trigger("focus");
        //        //    //$(this).siblings(".AutoCreditBins").autocomplete("search", " ");
        //        //});

        //        //$('.ShowAllOptionsBinMCR').unbind("click");
        //        //$('.ShowAllOptionsBinMCR').click(function () {
        //        //    var ddl = $(this).siblings('.AutoMSCreditBins');
        //        //    _AutoCompleteWrapper.searchHide(ddl);
        //        //    //$(this).siblings('.AutoMSCreditBins').trigger("focus");
        //        //    //$(this).siblings(".AutoMSCreditBins").autocomplete("search", " ");
        //        //});

        //        //$('.ShowAllOptionsStagingHeader').unbind("click");
        //        //$('.ShowAllOptionsStagingHeader').click(function () {
        //        //    var ddl = $(this).siblings('.AutoMSCreditStagingHeader');
        //        //    _AutoCompleteWrapper.searchHide(ddl);
        //        //    //$(this).siblings('.AutoMSCreditStagingHeader').trigger("focus");
        //        //    //$(this).siblings(".AutoMSCreditStagingHeader").autocomplete("search", " ");
        //        //});

        //        //$('.ShowAllOptionsOrderNumber').unbind("click");
        //        //$('.ShowAllOptionsOrderNumber').click(function () {
        //        //    var ddl = $(this).siblings('.AutoPullOrderNumber');
        //        //    _AutoCompleteWrapper.searchHide(ddl);
        //        //    //$(this).siblings('.AutoPullOrderNumber').trigger("focus");
        //        //    //$(this).siblings(".AutoPullOrderNumber").autocomplete("search", " ");
        //        //});

        //        //$('.bin-input-readonly').unbind("keypress");
        //        //$('.bin-input-readonly').keypress(function () {
        //        //    return false;
        //        //});

        //        //if (hasOnTheFlyEntryRight == "False") {
        //        //    $('.AutoCreditBins').unbind("keypress");
        //        //    $('.AutoCreditBins').keypress(function () {
        //        //        return false;
        //        //    });
        //        //    $('.AutoMSCreditBins').unbind("keypress");
        //        //    $('.AutoMSCreditBins').keypress(function () {
        //        //        return false;
        //        //    });

        //        //    $('.AutoMSCreditStagingHeader').unbind("keypress");
        //        //    $('.AutoMSCreditStagingHeader').keypress(function () {
        //        //        return false;
        //        //    });
        //        //}
        //    }
        //})
    };

    

    self.initEvents = function () {
        $(document).ready(function () {
            $("#ColumnOrderSetupIM").live('click', function () {
                $("#divReorderPopup").dialog('open');
                return false;
            });

            

            //if (isCost == 'False') {

            //    HideColumnUsingClassName(self.gridId);
            //}
            

            

            if (sProjectSpendVisible == "True") { //if ('@(ProjectSpendVisible)' == "True") {
                ProjectSpendClass = "read_only NotHide";
            }

            if (sProjectSpendVisible == "True") {
                ProjectSpendVisible = true;
            }


            $('#btnPullAllNewFlow').die('click');
            $("#btnPullAllNewFlow").click(function () {
                if ($('#ItemModeDataTable tbody tr.row_selected').length <= 0) {
                    $('#dlgNoSelectErrorMsg').modal();
                }
                else {
                    var isSerialLot = false;
                    $('#ItemModeDataTable').find("tbody").find("tr.row_selected").each(function (index, tr) {
                        var aPos = $('#ItemModeDataTable').dataTable().fnGetPosition($(tr)[0]);
                        var aData = $('#ItemModeDataTable').dataTable().fnGetData(aPos);
                        if (aData.SerialNumberTracking == "Yes" || aData.LotNumberTracking == "Yes" || aData.DateCodeTracking == "Yes") {
                            isSerialLot = true;
                        }
                    });

                    if (isSerialLot == true) {
                        OpenPullPopup($(this));
                    }
                    else {
                        $('#GlobalModalProcessing').modal({
                            escClose: false,
                            close: false
                        });
                        setTimeout('AddSingleItemToPullList($("#btnPullAllNewFlow"))', 1000);
                    }
                }
            });

            $('#btnPullAll').die('click');            
            $('#btnPullAllQL').die('click');
            $('#NewPullAction').die('change');


            //        if (window.location.href.indexOf("Pull/PullMasterList") > 0) {
            //            objColumns = GetGridHeaderColumnsObject('ItemModeDataTable', 'New Pull Columns', 'NewPullItemMaster', 'callbacknew()');
            //        }
            //        else {
            //            objColumns = GetGridHeaderColumnsObject('ItemModeDataTable', 'New Pull Columns', 'NewPullItemMaster', 'callbacknewFromReorder()');
            //        }

            $('#NewPullAction').live('change', function () {
                RefreshPullNarrowSearch();//DoNarrowSearchIM(); // Maintain narrow search value - WI-329  // fires ajax call for grid data 1
                NSForItemModel_ExecuteOnDocReady();
                //$('#ItemModeDataTable').dataTable().fnDraw(); // fires ajax call for grid data 2 so commented

                if ($(this).val() == "Pull") {
                    $("#liStageLocationHeaderIM").show();
                    $("#liStageLocationIM").show();
                    $("#THStagingHeader").hide();
                    $("#TDStagingHeader").hide();
                    $("#btnPullAll").show();
                    $("#btnCreditAll").hide();
                    $("#btnMSCreditAll").hide();
                    $("#btnPullAllNewFlow").show();
                    $('#CtabNew').find('#actionSelectAll3').show();
                    if ($("#ItemTypeIM") != undefined && $("#ItemTypeIM").length > 0) {
                        $("[name=multiselect_ItemTypeIM]").each(function () {
                            $(this).parent().parent().show();
                        });
                    }
                    document.getElementById('colQtyToPull').innerHTML = QtyToPull;
                }
                else if ($(this).val() == "Credit") {
                    $("#liStageLocationHeaderIM").hide();
                    $("#liStageLocationIM").hide();
                    $("#THStagingHeader").hide();
                    $("#TDStagingHeader").hide();
                    $("#btnPullAll").hide();
                    $("#btnPullAllNewFlow").hide();
                    $("#btnCreditAll").show();
                    $("#btnMSCreditAll").hide();
                    $('#CtabNew').find('#actionSelectAll3').hide();
                    if ($("#ItemTypeIM") != undefined && $("#ItemTypeIM").length > 0) {
                        $("[name=multiselect_ItemTypeIM]").each(function () {
                            if ($(this)[0].value > 2) {
                                $(this).parent().parent().hide();
                            }
                        });
                    }
                    document.getElementById('colQtyToPull').innerHTML = QtyToCredit;
                }
                else {
                    $("#liStageLocationHeaderIM").show();
                    $("#liStageLocationIM").show();
                    if (serIsIgnoreCreditRule == "true" || serIsIgnoreCreditRule == "True" || serIsIgnoreCreditRule == true) {
                        $("#THStagingHeader").show();
                        $("#TDStagingHeader").show();
                    }
                    $("#btnPullAll").hide();
                    $("#btnPullAllNewFlow").hide();
                    $("#btnCreditAll").hide();
                    $("#btnMSCreditAll").show();
                    $('#CtabNew').find('#actionSelectAll3').hide();
                    if ($("#ItemTypeIM") != undefined && $("#ItemTypeIM").length > 0) {
                        $("[name=multiselect_ItemTypeIM]").each(function () {
                            if ($(this)[0].value > 2) {
                                $(this).parent().parent().hide();
                            }
                        });
                    }
                    document.getElementById('colQtyToPull').innerHTML = QtytoMSCredit;
                }
            });



            $('input[type=radio]').live('change', function () {
                var selection = $(this).val(); // credit or pull
                if (selection == "credit")
                    $(this).parent().parent().find('#btnAdd').val('Credit');
                else if (selection == "creditms")
                    $(this).parent().parent().find('#btnAdd').val('CreditMS');
                else
                    $(this).parent().parent().find('#btnAdd').val('Pull');
            });


            $("#btnPullAll").live('click', function () {
                $('#DivLoading').show();
                if ($('#ItemModeDataTable tbody tr.row_selected').length <= 0) {
                    $('#dlgNoSelectErrorMsg').modal();
                }
                else {
                    $('#GlobalModalProcessing').modal();
                    setTimeout('AddSingleItemToPullList($("#btnPullAll"))', 1000);
                }
                $('#DivLoading').hide();
            });

            $("#btnPullAllQL").live('click', function () {
                $('#GlobalModalProcessing').modal();
                setTimeout('CheckValidPullData()', 1000);
            });

            $("#LocationDetails").dialog({
                autoOpen: false,
                show: "blind",
                hide: "explode",
                height: 700,
                title: "Item Locations",
                width: 900,
                modal: true,
                beforeClose: function () {
                    if (isDirtyForm) {
                        if (confirm(SaveConfirmationMSG)) {
                            //isDirtyForm = false;
                            return false;
                        }
                        isDirtyForm = false;
                    }
                },
                close: function () {
                    IsRefreshGrid = true;
                    $('#DivLoading').hide();
                    $("#LocationDetails").empty();
                    $('#ItemModeDataTable').dataTable().fnStandingRedraw();
                }
            });


            $("#project-spend-limit-basic-modal-content").on("click", "#btnModelYesPSLimit", function () {

                //TabItemClicked('RequisitionCreate','frmRequisitionMaster');
                var url = _NewConsumePull.urls.RequisitionListUrl;  //'@Url.Action("RequisitionList", "Consume")';
                url = url + '?fromPull=' + 'yes'
                window.location.href = url;
            });
            $("#dlgCommonErrorMsg").on("click", "#btnYesForPS", function () {
                var url = _NewConsumePull.urls.RequisitionListUrl;//'@Url.Action("RequisitionList", "Consume")';
                url = url + '?fromPull=' + 'yes'
                window.location.href = url;
            });

            //$("#dlgExpiredItemErrorMsg").on("click", "#btnYesForExpiredItem", function () {
            $("#btnYesForExpiredItem").click(function () {               
                $.modal.impl.close();
                if ($('#dlgExpiredItemErrorMsg').find("#btnPullExItemGuid").val() == '00000000-0000-0000-0000-000000000000') {
                    PullAllNewFlow(false);
                }
                else {
                    ConfirmPullExpiredItems($('#dlgExpiredItemErrorMsg').find("#btnPullExItemGuid").val(), $('#dlgExpiredItemErrorMsg').find("#btnPullExReqGuid").val());
                }
            });
            $("#ColumnOrderSetup_Context1").click(function () {
                //$("#ColumnSortableModalIM").dialog("open");
                $('#ColumnOrderSetupIM').click();

                return false;
            });

            $("#refreshGrid1").click(function () {
                $('#ItemModeDataTable').dataTable().fnDraw();
            });

            $("#tblPullCommonUDF").off('change', "input[type='text'][name^='txtMSPullStagingHeader']");
            $("#tblPullCommonUDF").on('change', "input[type='text'][name^='txtMSPullStagingHeader']", function (e) {
                var objCurtxt = $(this);
                var CurrentValue = $(objCurtxt).val();
                var vItemGUID = $(this).prop("id").split('_')[1];
                var BinID = $("#hdnBinIDValue").val();

                if ($.trim(CurrentValue) != null && $.trim(CurrentValue) != "") {
                    $.ajax({
                        type: "POST",
                        url: SaveMaterialStagingUrl,
                        contentType: 'application/json',
                        dataType: 'json',
                        data: "{ BinID: '" + 0 + "' , StagingName: '" + $.trim(CurrentValue) + "'}",
                        success: function (result) {
                            if (result.Status == "Success") {
                                if (result.MaterialStagingGuid != "")
                                    $("#tblPullCommonUDF").find("#MSPullStagingHeaderValue").val(result.MaterialStagingGuid);
                            }
                        },
                        error: function (err) {
                            console.log(err);
                        }
                    });
                }
            });

            $("#clear_QLItemModel_filter").click(funClearFilterIM);
            $('#PageNumberIM').keydown(function (e) {
                var code = (e.keyCode ? e.keyCode : e.which);
                if (code == 13) {
                    $("#GobtnIM").click();
                    return false;
                }
            });

            $("#GobtnIM").click(function () {
                var pval = $('#PageNumberIM').val();
                if (pval == "" || pval.match(/[^0-9]/)) {
                    return;
                }
                if (pval == 0)
                    return;
                $('#ItemModeDataTable').dataTable().fnPageChange(Number(pval - 1));
                $('#PageNumberIM').val('');
            });

            // used for refresh the grid manually...
            $('#refreshGridIM').live('click', function () {
                if ($("#divRefreshBlockIM").css("display").toUpperCase() == 'BLOCK')
                    $("#divRefreshBlockIM").toggle();

                $('#ItemModeDataTable').dataTable().fnDraw();
                //fillItemMaster(false);

            });

            $("#reordersettingIM").click(function () {
                $("#divRefreshBlockIM").toggle();
            });

            $(oTableItemModel).ajaxSuccess(function () {
                if (IsQLLoaded) {
                    //alert("IsQLLoaded");
                    IsQLLoaded = false;
                    setTimeout('CallForQLPull()', 1000);
                }
            });

        });// ready

        
    };



    self.GetUdfVal = function ($udf) {
        var val = "";
        if ($udf != null) {
            if ($udf.attr("class") == 'selectBox' || $udf.attr("class") == 'selectBox loadondemandudf') {
                val = $udf.find("option:selected").text(); //$("#UDF1PullCommon option:selected").text();
            }
            else {
                val = $udf.val();
            }
        }
        return val;
    }

    self.GetProjectId = function ($tre) {
        var eProjectID = self.getDataEleFromRow($tre, 'ProjectID');
        var val = "";
        if (typeof eProjectID !== 'undefined' && eProjectID !== null) {
            //vProjectID = $tr.find('#ProjectID')[0].value == "" ? "" : $(obj).parent().parent().find('#ProjectID')[0].value;
            val = eProjectID.attr("data-ProjectID");
        }

        return val;
    };

    self.getItemDataFromRow = function ($tr) {
        // tr with 'itemWisePONumber' contains many data attrs so return them in object
        var ele = self.getDataEleFromRow($tr, 'itemWisePONumber');
        var ret = {
            spnItemID: null,
            spnItemGUID: null,
            spnOn_HandQuantity: null,
            spnOrderItemType: null,
            itemWisePONumber: null,
            spnIsIgnoreCreditRule: null,
            spnItemNumber: null,
            spnQuickListGUID: null,
            hdnIsLoadMoreStaging : null
        };

        if (ele != null) {

            if (typeof ele.attr("data-spnItemID") !== 'undefined') {
                ret.spnItemID = ele.attr("data-spnItemID");
            }
            else {
                ret.spnItemID = 0;
            }
            if (typeof ele.attr("data-spnItemGUID") !== 'undefined') {
                ret.spnItemGUID = ele.attr("data-spnItemGUID");
            }

            ret.spnOn_HandQuantity = ele.attr("data-spnOn_HandQuantity");
            ret.spnOn_HandQuantity = ret.spnOn_HandQuantity == "" ? 0 : ret.spnOn_HandQuantity;

            ret.spnOrderItemType = ele.attr("data-spnOrderItemType");
            ret.itemWisePONumber = ele.attr("data-itemWisePONumber");
            if (typeof ele.attr("data-spnIsIgnoreCreditRule") !== 'undefined') {
                ret.spnIsIgnoreCreditRule = ele.attr("data-spnIsIgnoreCreditRule");
            }
            else {
                ret.spnIsIgnoreCreditRule = false;
            }

            ret.spnItemNumber = ele.attr("data-spnItemNumber");
            if (typeof ele.attr("data-spnQuickListGUID") !== 'undefined') {
                ret.spnQuickListGUID = ele.attr("data-spnQuickListGUID");
            }

            if (typeof ele.attr("data-hdnIsLoadMoreStaging") !== 'undefined') {
                ret.hdnIsLoadMoreStaging = ele.attr("data-hdnIsLoadMoreStaging");
            }
        }

        return ret;
    };

    // private functions

    function UDFfillEditableOptionsForGridNewPull() {
        var _EnterPriseId = $("#hdnEnterpriseId").val();

        var fnAutoComplete = function (currentObj) {

            //var currentObj = $(this);
            //setTimeout(function () {
            var ddl2 = $(currentObj).siblings(".udf-editable-dropdownbox");

            _AutoCompleteWrapper.init(ddl2, '/UDF/GetUDFEditableOptionsByUDF'
                , function (request) {
                    var _UDFID = ddl2.prev().val();
                    return {
                        maxRows: 1000,
                        name_startsWith: request.term,
                        UDFID: _UDFID,
                        EnterpriseID: _EnterPriseId
                    };
                }
                , function (data) {
                    return $.map(data, function (item) {
                        return {
                            label: item.UDFOption,
                            value: item.UDFOption,
                            selval: item.ID
                        };
                    });
                }
                , function (curVal, selectedItem) {
                    //$("#" + _UDFColumnName).val(ui.item.selval);                
                }
                , null
                , false, true
            );

            
        }

        $('#tblPullCommonUDF').on("click", "td .show-all-options", function () {
            fnAutoComplete($(this));
            var ddl2 = $(this).siblings(".udf-editable-dropdownbox");
            _AutoCompleteWrapper.searchHide(ddl2);
        });

        $('#tblPullCommonUDF').on("click", "input.udf-editable-dropdownbox", function () {
            var lnk = $($(this).siblings()[0]);
            fnAutoComplete(lnk);
        });

        
        $('#' + self.gridId).on("click", "td .show-all-options", function () {
            fnAutoComplete($(this));
            var ddl2 = $(this).siblings(".udf-editable-dropdownbox");
            _AutoCompleteWrapper.searchHide(ddl2);
        });

        $('#' + self.gridId).on("click", "input.udf-editable-dropdownbox", function () {
            var lnk = $($(this).siblings()[0]);
            fnAutoComplete(lnk);
        });
                

        //$('#' + self.gridId).on("click", ".udf-editable-autocomplete-dropdownbox", function () {

            
        //    //$ShowAllOptions = $(this).parent().find(".show-all-options"); //$('.show-all-options');
        //    //$ShowAllOptions.unbind("click");
        //    //$ShowAllOptions.click(function () {
        //    //    var currentObj = $(this);
        //    //    //setTimeout(function () {
        //    //    var ddl2 = $(currentObj).siblings(".udf-editable-dropdownbox");

        //    //    //ddl2.autocomplete("search", " ");
        //    //    //ddl2.trigger("focus");
        //    //    _AutoCompleteWrapper.searchHide(ddl2);

        //    //    ////setTimeout(function () {
        //    //    ////$('ul.ui-autocomplete').css('overflow-y', 'auto');
        //    //    ////$('ul.ui-autocomplete').css('max-height', '300px');
        //    //    ////$('ul.ui-autocomplete').css('z-index', '99999');
        //    //    ////}, 100);



        //    //    ////setTimeout(function(){

        //    //    ////},1000);

        //    //});

        //    var _UDFID = $(this).prev().val();
        //    var ddl = $(this);

        //    _AutoCompleteWrapper.init(ddl, '/UDF/GetUDFEditableOptionsByUDF'
        //        , function (request) {
        //            return {
        //                maxRows: 1000,
        //                name_startsWith: request.term,
        //                UDFID: _UDFID,
        //                EnterpriseID: _EnterPriseId
        //            };
        //        }
        //        , function (data) {
        //            return $.map(data, function (item) {
        //                return {
        //                    label: item.UDFOption,
        //                    value: item.UDFOption,
        //                    selval: item.ID
        //                };
        //            });
        //        }
        //        , function (curVal, selectedItem) {
        //            //$("#" + _UDFColumnName).val(ui.item.selval);                
        //        }
        //        ,
        //        null
        //    );
        //});


        //$('.udf-editable-autocomplete-dropdownbox').each(function () {

            
            

        //    //ddl.autocomplete({
        //    //    source: function (request, response) {
        //    //        $.ajax({
        //    //            url: '/UDF/GetUDFEditableOptionsByUDF',
        //    //            contentType: 'application/json',
        //    //            dataType: 'json',
        //    //            data: {
        //    //                maxRows: 1000,
        //    //                name_startsWith: request.term,
        //    //                UDFID: _UDFID,
        //    //                EnterpriseID: _EnterPriseId
        //    //            },
        //    //            cache: false,
        //    //            success: function (data) {
        //    //                response($.map(data, function (item) {
        //    //                    return {
        //    //                        label: item.UDFOption,
        //    //                        value: item.UDFOption,
        //    //                        selval: item.ID
        //    //                    };
        //    //                }));
        //    //            }
        //    //        });
        //    //    },
        //    //    autoFocus: false,
        //    //    minLength: 0,
        //    //    select: function (event, ui) {
        //    //        //$("#" + _UDFColumnName).val(ui.item.selval);
        //    //    },
        //    //    open: function () {
        //    //        ddl.removeClass("ui-corner-all").addClass("ui-corner-top");

        //    //        ddl.autocomplete('widget').css('z-index', '99999 !important');
        //    //    },
        //    //    close: function () {
        //    //        ddl.removeClass("ui-corner-top").addClass("ui-corner-all");
        //    //    }
        //    //});

        //});

        //$(document).on('click', 'a.show-all-options', function () {
        //    $(this).siblings(".udf-editable-dropdownbox").autocomplete("search", "");
        //    $(this).siblings('.udf-editable-dropdownbox').trigger("focus");
        //});


    }


    return self;

})(jQuery); // _NewConsumePull end




function ShowHideNewPullProgressBar(isShow) {

    if (isShow) {
        $('#DivLoading').show();
        $('#divNewPullProcessing').modal();
        //   window.confirm('in showhide');
        $('#divNewPullProcessing').parent().parent().find(".modalCloseImg").css('display', 'none');
    }
    else {
        $('#DivLoading').hide();
        $.modal.impl.close();
    }
}

function callbacknew() {
    return false;
}

function callbacknewFromReorder() {
    $('#DivLoading').show();
    window.location = _NewConsumePull.urls.NewPullUrl; //'@Url.Content("~/Pull/NewPull")';
}

function callbackhistory() {
    window.location = _NewConsumePull.urls.PullMasterListUrl; //'@Url.Content("~/Pull/PullMasterList")';
}

function closeModalNoSelectModel() {
    $.modal.impl.close();
}

function closeModalPSLimit() {
    $.modal.impl.close();
    $('#ItemModeDataTable').dataTable().fnStandingRedraw();
}


function onlyNumeric(event) {
    var charCode = (event.which) ? event.which : event.keyCode

    if (charCode > 31 && (charCode < 48 || charCode > 57 || charCode == 86))
        return false;

    return true;

}
function fnFilterGlobalIM() {
    //set filter only if more than 2 characters are pressed
    if (typeof $("#ItemModel_filter") != 'undefined' && ($("#ItemModel_filter").val().length > 2 || $("#ItemModel_filter").val().length == 0)) {
        FilterNCPGridDataFromGlobalFilter();
    }
    else {
        $('#ItemModeDataTable td').removeHighlight();
        $('#ItemModeDataTable td').highlight($("#ItemModel_filter").val());
    }
}
function fnFilterGlobalIM_OnEnter(KeyCode) {


    if (typeof $("#ItemModel_filter") != 'undefined' && (KeyCode == 13 || $("#ItemModel_filter").val().length == 0)) {
        FilterNCPGridDataFromGlobalFilter();
    }
    else {
        $('#ItemModeDataTable td').removeHighlight();
        $('#ItemModeDataTable td').highlight($("#ItemModel_filter").val());
    }
}

function FilterNCPGridDataFromGlobalFilter() {
    var searchtext = $("#ItemModel_filter").val().replace(/'/g, "''");
    //$('#ItemModeDataTable').dataTable().fnFilter(
    //                searchtext,
    //                null,
    //                null,
    //                null
    //            );
    DoNarrowSearchIM();
}

var IsCtrl = false;
var IsV = false;
var IsC = false;

var timeoutsc2;
$(document).on('propertychange input', "#ItemModel_filter", function () {
    clearTimeout(timeoutsc2);
    var SearchControl = this;
    timeoutsc2 = setTimeout(function () {
        if (SearchPattern == 2 || SearchPattern == "2") {
            fnFilterGlobalIM();
        }
        else {
            $('#ItemModel_filter').unbind("keypress");
            $('#ItemModel_filter').keypress(function (event) {
                var keycode = (event.keyCode ? event.keyCode : event.which);
                setTimeout(function () { fnFilterGlobalIM_OnEnter(keycode); }, 200);
            });
            if ($("#ItemModel_filter").val().length == 0) {
                setTimeout(function () { fnFilterGlobalIM_OnEnter(0); }, 200);
            }
        }
    }, 500);
});
if (SearchPattern == 2 || SearchPattern == "2") {
    $("#ItemModel_filter").keypress(function (e) {
        if (e.which == 13 && $("#ItemModel_filter").val().length >= 2) {
            FilterNCPGridDataFromGlobalFilter();
        }
    });
}
else {
    $("#ItemModel_filter").keypress(function (e) {
        setTimeout(function () { fnFilterGlobalIM_OnEnter(0); }, 200);
    });
}
//-------------BARCODE SEARCH------------
//
BarcodeSearch_SearchTextBoxId = 'ItemModel_filter';
BindEventsForBarcodeScanning();
function ExecuteOnBarcodeSearch() {
    fnFilterGlobalIM();
}
//$("#ItemModel_filter").keyup(function (e) {
//    var code = (e.keyCode ? e.keyCode : e.which);
//});

//$("#ItemModel_filter").keydown(function (e) {
//    var code = (e.keyCode ? e.keyCode : e.which);
//});

//$("#ItemModel_filter").keyup(function (e) {
//    var code = (e.keyCode ? e.keyCode : e.which);

//    if ((IsCtrl == true && IsV == true) || (IsCtrl == true && IsC == true) || code == 13 || code == 16 || code == 17 || code == 18 || code == 9)
//    { }
//    else {
//        SearchByBarcode(this);
//        fnFilterGlobalIM();
//    }

//    if (code == 17)
//        IsCtrl = false;

//    if (code == 86)
//        IsV = false;

//    if (code == 67)
//        IsC = false;
//});

//$("#ItemModel_filter").keydown(function (e) {
//    var code = (e.keyCode ? e.keyCode : e.which);

//    if (code == 17)
//        IsCtrl = true;

//    if (code == 86)
//        IsV = true;

//    if (code == 67)
//        IsC = true;

//    if (code == 13) {
//        var searchtext = $("#ItemModel_filter").val().replace(/'/g, "''");
//        //$('#ItemModeDataTable').dataTable().fnFilter(
//        //                searchtext,
//        //                null,
//        //                null,
//        //                null
//        //            );
//        DoNarrowSearchIM();
//        return false;
//    }
//});

//$(document).on('propertychange input', "#ItemModel_filter", function () {
//    fnFilterGlobalIM();
//    return false;
//});


function funClearFilterIM() {
    //Check length first
    if ($("#ItemModel_filter").val().length > 0) {
        $("#ItemModel_filter").val('');
        $('#ItemModeDataTable').dataTable().fnFilter(
            $("#ItemModel_filter").val(),
            null,
            null,
            null
        );
    }
    $("#ItemModel_filter").focus();
    return false;
}

function diabledPullbutton() {
    $('#DivLoading').show();
    $("table#ItemModeDataTable tbody tr").each(function () {
        $(this).children("td:first").children("input.pullbutton").prop('disabled', true);
        $('#btnAdd').attr("disabled", true);
    });
}
function enablePullbutton() {
    $('#DivLoading').hide();
    $("table#ItemModeDataTable tbody tr").each(function () {
        $(this).children("td:first").children("input.pullbutton").prop('disabled', false);
    });
}

function LoadQuickListData(OBJGridRow) {
    ////diabledPullbutton();
    $(OBJGridRow).attr("disabled", "disabled");
    var $tr = $(OBJGridRow).parent().parent();
    var vsQuickListGUID = _NewConsumePull.getItemDataFromRow($tr).spnQuickListGUID;  //$(OBJGridRow).parent().parent().find('#spnQuickListGUID').text() == "" ? "" : $(OBJGridRow).parent().parent().find('#spnQuickListGUID').text();
    QuickListPULLQty = parseInt($tr.find('#txtQty').val() == '' ? 0 : $(OBJGridRow).parent().parent().find('#txtQty').val());
    if (QuickListPULLQty > 0) {
        var searchText = "QLGuid=" + vsQuickListGUID + "#Qty=" + QuickListPULLQty.toString();
        $('#ItemModeDataTable').dataTable().fnFilter(searchText, null, null, null);
        IsQLLoaded = true;
    }
    else {
        $('div#target').fadeToggle();
        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
        $("#spanGlobalMessage").html(MsgProperQLQty);
        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
        ////enablePullbutton();
        $(OBJGridRow).removeAttr("disabled");
    }
    return false;
}



function CallForQLPull() {
    IsQLLoaded = false;
    if ($('#ItemModeDataTable').dataTable().find("tbody tr")[0].cells.length > 1) {
        $('#ItemModeDataTable').dataTable().find("tbody tr").removeClass("row_selected").addClass("row_selected");
        $("#btnPullAllQL").click();
    }
}

function LoadAllItemDataWithFilter() {
    $('#ItemModeDataTable').dataTable().fnFilter(narrowSearch, null, null, null);
}

function LoadAllItemData() {
    $('#ItemModeDataTable').dataTable().fnFilter("", null, null, null);
}

function CheckValidPullData() {

    var IsGlobalErrorMsgShow = false;
    var IsPSRedirectToReq = false;
    var IsQLSuccessfullyDone = true;
    var QLErrorMsg = '';
    $('#ItemModeDataTable tbody tr.row_selected').each(function (i) {

        var $tr = $(this);

        if ($tr.attr('class').indexOf('row_selected') != -1) {
            $tr.toggleClass('row_selected');
            var RowObject = $tr;
            var aPos = $('#ItemModeDataTable').dataTable().fnGetPosition($tr[0]);
            var aData = $('#ItemModeDataTable').dataTable().fnGetData(aPos);
            var errorMsg = '';
            var txxt = $tr.find('#txtQty');
            var vBinID,
                vProjectID;

            var itemData = _NewConsumePull.getItemDataFromRow($tr);
            var itemType = itemData.spnOrderItemType; //$tr.find('#spnOrderItemType').text();
            var txtQty = txxt.val();
                        

            if (itemType != '4') {
                vBinID = self.getDataFromRow($tr,"BinID");  //$(this).find('#BinID')[0].value == '' ? 0 : $(this).find('#BinID')[0].value;
                vBinID = vBinID == '' ? 0 : vBinID;

                if ($("#chkUsePullCommonUDF").is(":checked")) {
                    if ($('#ProjectIDCommon') != undefined) {
                        vProjectID = $('#ProjectIDCommon').val() == "" ? "" : $('#ProjectIDCommon').val();
                    }
                    else {
                        vProjectID = "";
                    }
                }
                else {
                    //var eProjectID = _NewConsumePull.getDataEleFromRow($tr,'ProjectID');
                    ////if ($tr.find('#ProjectID')[0] != undefined) {
                    //if (typeof eProjectID != 'undefined' && eProjectID != null) {
                    //    //vProjectID = $tr.find('#ProjectID')[0].value == "" ? "" : $tr.find('#ProjectID')[0].value;
                    //    vProjectID = eProjectID.val();
                    //}
                    //else {
                    //    vProjectID = "";
                    //}
                    vProjectID = _NewConsumePull.GetProjectId($tr);
                }

                //vProjectID = $(this).find('#ProjectID')[0].value == "" ? "" : $(this).find('#ProjectID')[0].value;

                if (!(!isNaN(parseFloat(txtQty)) && parseFloat(txtQty) > 0)) {
                    errorMsg += "<b style='color:Olive;'>" + aData.ItemNumber + ": " + MsgQtyToPullMandatory +"</b></br>";
                    IsGlobalErrorMsgShow = true;
                    IsQLSuccessfullyDone = false;
                }

                if (!(!isNaN(parseInt(vBinID)) && parseFloat(vBinID) > 0)) {
                    errorMsg += "<b style='color:Olive;'>" + aData.ItemNumber + ": " + MsgInventoryLocationMandatory +"</b></br>";
                    IsGlobalErrorMsgShow = true;
                    IsQLSuccessfullyDone = false;
                }
            }
            else {
                if (!(!isNaN(parseFloat(txtQty)) && parseFloat(txtQty) > 0)) {
                    errorMsg += "<b style='color:Olive;'>" + aData.ItemNumber + ": " + MsgLabourItemRequiredHours +"</b></br>";
                    IsGlobalErrorMsgShow = true;
                    IsQLSuccessfullyDone = false;
                }

                vBinID = 0;
                vProjectID = '';
            }

            if (errorMsg.length <= 0) {
                var itemData = _NewConsumePull.getItemDataFromRow($tr);
                var vItemID = itemData.spnItemID; //$tr.find('#spnItemID').text();
                var vItemGUID = itemData.spnItemGUID; //$tr.find('#spnItemGUID').text();
                var vspnOn_HandQuantity = itemData.spnOn_HandQuantity; //$tr.find('#spnOn_HandQuantity').text() == "" ? 0 : $tr.find('#spnOn_HandQuantity').text();
                var vPullCreditText = "pull"; //$(obj)[0].value;//$(obj).parent().parent().find('input[name=colors'+vItemID+']:checked')[0].value;
                var VspnDefaultPullQuantity = _NewConsumePull.getDataFromRow($tr, 'spnDefaultPullQuantity'); //$(this).find('#spnDefaultPullQuantity').text() == "" ? 0 : $(this).find('#spnDefaultPullQuantity').text();
                var vUDF1 = ''; var vUDF2 = ''; var vUDF3 = ''; var vUDF4 = ''; var vUDF5 = '';
                var vUDF1PullCommon = ''; var vUDF2PullCommon = ''; var vUDF3PullCommon = ''; var vUDF4PullCommon = ''; var vUDF5PullCommon = '';
                                

                vUDF1PullCommon = _NewConsumePull.GetUdfVal($('#UDF1PullCommon'));
                vUDF2PullCommon = _NewConsumePull.GetUdfVal($('#UDF2PullCommon'));
                vUDF3PullCommon = _NewConsumePull.GetUdfVal($('#UDF3PullCommon'));
                vUDF4PullCommon = _NewConsumePull.GetUdfVal($('#UDF4PullCommon'));
                vUDF5PullCommon = _NewConsumePull.GetUdfVal($('#UDF5PullCommon'));

                vUDF1 = _NewConsumePull.GetUdfVal($tr.find('#UDF1'));
                vUDF2 = _NewConsumePull.GetUdfVal($tr.find('#UDF2'));
                vUDF3 = _NewConsumePull.GetUdfVal($tr.find('#UDF3'));
                vUDF4 = _NewConsumePull.GetUdfVal($tr.find('#UDF4'));
                vUDF5 = _NewConsumePull.GetUdfVal($tr.find('#UDF5'));

                //if ($("#UDF1PullCommon") != null) {
                //    if ($("#UDF1PullCommon").attr("class") == 'selectBox') {
                //        vUDF1PullCommon = $("#UDF1PullCommon option:selected").text();
                //    }
                //    else {
                //        vUDF1PullCommon = $("#UDF1PullCommon").val();
                //    }
                //}

                //if ($("#UDF2PullCommon") != null) {
                //    if ($("#UDF2PullCommon").attr("class") == 'selectBox') {
                //        vUDF2PullCommon = $("#UDF2PullCommon option:selected").text();
                //    }
                //    else {
                //        vUDF2PullCommon = $("#UDF2PullCommon").val();
                //    }
                //}

                //if ($("#UDF3PullCommon") != null) {
                //    if ($("#UDF3PullCommon").attr("class") == 'selectBox') {
                //        vUDF3PullCommon = $("#UDF3PullCommon option:selected").text();
                //    }
                //    else {
                //        vUDF3PullCommon = $("#UDF3PullCommon").val();
                //    }
                //}

                //if ($("#UDF4PullCommon") != null) {
                //    if ($("#UDF4PullCommon").attr("class") == 'selectBox') {
                //        vUDF4PullCommon = $("#UDF4PullCommon option:selected").text();
                //    }
                //    else {
                //        vUDF4PullCommon = $("#UDF4PullCommon").val();
                //    }
                //}

                //if ($("#UDF5PullCommon") != null) {
                //    if ($("#UDF5PullCommon").attr("class") == 'selectBox') {
                //        vUDF5PullCommon = $("#UDF5PullCommon option:selected").text();
                //    }
                //    else {
                //        vUDF5PullCommon = $("#UDF5PullCommon").val();
                //    }
                //}

                //if ($tr.find('#UDF1') != null) {
                //    if ($tr.find('#UDF1').attr("class") == 'selectBox') {
                //        vUDF1 = $tr.find('#UDF1 option:selected').text();
                //    }
                //    else {
                //        vUDF1 = $tr.find('#UDF1').val();
                //    }
                //}

                //if ($tr.find('#UDF2') != null) {
                //    if ($tr.find('#UDF2').attr("class") == 'selectBox') {
                //        vUDF2 = $tr.find('#UDF2 option:selected').text();
                //    }
                //    else {
                //        vUDF2 = $tr.find('#UDF2').val();
                //    }
                //}

                //if ($tr.find('#UDF3') != null) {
                //    if ($tr.find('#UDF3').attr("class") == 'selectBox') {
                //        vUDF3 = $tr.find('#UDF3 option:selected').text();
                //    }
                //    else {
                //        vUDF3 = $tr.find('#UDF3').val();
                //    }
                //}

                //if ($tr.find('#UDF4') != null) {
                //    if ($tr.find('#UDF4').attr("class") == 'selectBox') {
                //        vUDF4 = $tr.find('#UDF4 option:selected').text();
                //    }
                //    else {
                //        vUDF4 = $tr.find('#UDF4').val();
                //    }
                //}

                //if ($tr.find('#UDF5') != null) {
                //    if ($tr.find('#UDF5').attr("class") == 'selectBox') {
                //        vUDF5 = $tr.find('#UDF5 option:selected').text();
                //    }
                //    else {
                //        vUDF5 = $tr.find('#UDF5').val();
                //    }
                //}

                if ($("#chkUsePullCommonUDF").is(":checked")) {
                    vUDF1 = vUDF1PullCommon;
                    vUDF2 = vUDF2PullCommon;
                    vUDF3 = vUDF3PullCommon;
                    vUDF4 = vUDF4PullCommon;
                    vUDF5 = vUDF5PullCommon;
                }

                ///////////////////////////////////// UPDATE PULL DATA CALL//////////////////////////////////////////START
                $.ajax({
                    "url": _NewConsumePull.urls.CheckValidPullDataUrl, //'@Url.Content("~/Pull/CheckValidPullData")',
                    //data: { ID: 0, ItemGUID: vItemGUID, ProjectGUID: vProjectID, BinID: vBinID, PullCreditQuantity: (txtQty * QuickListPULLQty), PullCredit: vPullCreditText, TempPullQTY: (txtQty * QuickListPULLQty), UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5, RequisitionDetailID: "", WorkOrderDetailGUID: '@TempData["WorkOrderGUID"]' },
                    data: { ID: 0, ItemGUID: vItemGUID, ProjectGUID: vProjectID, BinID: vBinID, PullCreditQuantity: (txtQty), PullCredit: vPullCreditText, TempPullQTY: (txtQty), UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5, RequisitionDetailID: "", WorkOrderDetailGUID: tmpdWorkOrderGUID },
                    "async": false,
                    cache: false,
                    "dataType": "json",
                    success: function (response) {
                        $('#DivLoading').hide();
                        if (response.Status == "ok") {
                            if (response.LocationMSG != "") {
                                if (response.PSLimitExceed) {
                                    IsPSRedirectToReq = true;
                                    IsGlobalErrorMsgShow = true;
                                    IsQLSuccessfullyDone = false;
                                    errorMsg += "<b style='color:Olive'>" + aData.ItemNumber + ": " + response.LocationMSG + " " + msgProjectSpendLimitConfirmation+"</b><br>"
                                    //return true;
                                }
                                else {
                                    IsPSRedirectToReq = false;
                                    IsGlobalErrorMsgShow = true;
                                    IsQLSuccessfullyDone = false;
                                    errorMsg += "<b style='color:Olive'>" + aData.ItemNumber + ": " + response.LocationMSG + "</b><br>"
                                    //return true;
                                }
                            }
                        }
                        else {
                            IsPSRedirectToReq = false;
                            IsGlobalErrorMsgShow = true;
                            IsQLSuccessfullyDone = false;
                            errorMsg += "<b style='color:Olive'>" + aData.ItemNumber + ": " + response.Message + "</b><br>"
                            //return true;
                        }
                        txxt.val(VspnDefaultPullQuantity);
                    },
                    error: function (response) {
                        IsPSRedirectToReq = false;
                        IsGlobalErrorMsgShow = true;
                        IsQLSuccessfullyDone = false;
                        errorMsg += "<b style='color:Olive'>" + aData.ItemNumber + ": " + response.Message + "</b><br>"
                        //return true;
                    }
                });
                ///////////////////////////////////// UPDATE PULL DATA CALL//////////////////////////////////////////END
            }
        }
        QLErrorMsg += errorMsg;
    });
    if (IsPSRedirectToReq) {
        $.modal.impl.close();
        //$('#project-spend-limit-basic-modal-content').modal();
        QLErrorMsg = "<b>" + SomeItemNotPulled +"</b><br /><br />" + QLErrorMsg;
        $('#dlgCommonErrorMsg').find("#pOkbtn").css('display', 'none');
        $('#dlgCommonErrorMsg').find("#pYesNobtn").css('display', '');
        $('#dlgCommonErrorMsg').find("#pErrMessage").html(QLErrorMsg);
        $('#dlgCommonErrorMsg').modal()
    }
    else if (IsGlobalErrorMsgShow) {
        $.modal.impl.close();
        //$('#noramal-global-msg-QL-basic-modal-content').modal();
        QLErrorMsg = "<b>" + SomeItemNotPulled +"</b><br /><br />" + QLErrorMsg;
        $('#dlgCommonErrorMsg').find("#pOkbtn").css('display', '');
        $('#dlgCommonErrorMsg').find("#pYesNobtn").css('display', 'none');
        $('#dlgCommonErrorMsg').find("#pErrMessage").html(QLErrorMsg);
        $('#dlgCommonErrorMsg').modal()
    }
    else {
        $('#ItemModeDataTable').dataTable().find("tbody tr").removeClass("row_selected").addClass("row_selected");
        setTimeout('AddSingleItemToPullList($("#btnPullAllQL"))', 1000);
    }
}


function BindNewPONumber(newPairString) {
    //alert(newPairString);
    //
    $("#hdnSuppPOPair").val(newPairString);
    if (newPairString.length > 0) {

        $('#ItemModeDataTable tbody tr').each(function (i) {

            var rowObj = $(this);
            var vSuppID = _NewConsumePull.getDataFromRow(rowObj,'supID'); //$(this).find("#supID").val();

            var itemWisePONumber = _NewConsumePull.getItemDataFromRow(rowObj).itemWisePONumber; //$(this).find("input#itemWisePONumber").val();

            $.each($.parseJSON(newPairString), function (idx, obj) {
                if (itemWisePONumber != '' && itemWisePONumber != "null") {
                    if (rowObj.find('#txtPullOrderNumber') != null) {
                        rowObj.find('#txtPullOrderNumber').val(itemWisePONumber);
                        return false;
                    }
                }
                else {
                    if (obj.SupplierID == vSuppID) {
                        if (rowObj.find('#txtPullOrderNumber') != null) {
                            rowObj.find('#txtPullOrderNumber').val(obj.PONumber);
                            return false;
                        }
                    }
                }
            });

        });
    }
    else {
        $('#ItemModeDataTable tbody tr').each(function (i) {
            var $thisR = $(this);
            var itemWisePONumber = _NewConsumePull.getItemDataFromRow($thisR).itemWisePONumber;//$(this).find("input#itemWisePONumber").val();


            if (itemWisePONumber != '' && itemWisePONumber != null) {
                if ($thisR.find('#txtPullOrderNumber') != null) {
                    $thisR.find('#txtPullOrderNumber').val(itemWisePONumber);
                    return false;
                }
            }
        });
    }
}

function AddSingleItemToPullList(obj) {
    ////diabledPullbutton();
    var $obj = $(obj),
        //$this = $(this),
        $td = $obj.parent(),
        $tr = $obj.parent().parent();
       

    if ($obj.length > 0) {
        $obj.attr("disabled", "disabled");
    }


    if ($obj[0].value == undefined) {
        return false;
    }

    var ProjectSpendName;
    //$('#DivLoading').show();

    if ($($(obj)[0]).data("val") == 'Pull QL') {

        var IsGlobalErrorMsgShow = false;
        var IsPSRedirectToReq = false;
        var IsQLSuccessfullyDone = true;
        var errGBLMsg = '';
        $('#ItemModeDataTable tbody tr').each(function (i) {
            var $thisTr = $(this);
            if ($thisTr.attr('class').indexOf('row_selected') != -1) {
                $thisTr.toggleClass('row_selected');
                var RowObject = $thisTr;
                var aPos = $('#ItemModeDataTable').dataTable().fnGetPosition($this[0]);
                var aData = $('#ItemModeDataTable').dataTable().fnGetData(aPos);
                var errorMsg = '';

                var txxt = $thisTr.find('#txtQty');

                var vBinID;
                var vProjectID;
                var itemData = _NewConsumePull.getItemDataFromRow($thisTr);
                var itemType = itemData.spnOrderItemType; //$thisTr.find('#spnOrderItemType').text();
                var txtQty = txxt.val();

                if (itemType != '4') {
                    vBinID = _NewConsumePull.getDataFromRow($thisTr, 'BinID'); //$thisTr.find('#BinID')[0].value == '' ? 0 : $thisTr.find('#BinID')[0].value;
                    vBinID = vBinID == '' ? 0 : vBinID;

                    if ($("#chkUsePullCommonUDF").is(":checked")) {
                        if ($('#ProjectIDCommon') != undefined)
                            vProjectID = $('#ProjectIDCommon').val() == "" ? "" : $('#ProjectIDCommon').val();
                        else
                            vProjectID = "";
                        if ($('#txtProjectSpentCommon').val() != '') {
                            ProjectSpendName = $('#txtProjectSpentCommon').val();
                        }
                    }
                    else {
                        //var eProjectID = _NewConsumePull.getDataEleFromRow($thisTr, 'ProjectID');

                        ////if ($thisTr.find('#ProjectID')[0] != undefined) {
                        //if (typeof eProjectID != 'undefined' && eProjectID != null) {
                        //    //vProjectID = $thisTr.find('#ProjectID')[0].value == "" ? "" : $thisTr.find('#ProjectID')[0].value;
                        //    vProjectID = eProjectID.val();
                        //}
                        //else {
                        //    vProjectID = "";
                        //}

                        vProjectID = _NewConsumePull.GetProjectId($thisTr);

                        if ($thisTr.find('#txtProjectSpent').val() != '') {
                            ProjectSpendName = $thisTr.find('#txtProjectSpent').val();
                        }
                    }

                    //vProjectID = $(this).find('#ProjectID')[0].value == "" ? "" : $(this).find('#ProjectID')[0].value;

                    if (!(!isNaN(parseFloat(txtQty)) && parseFloat(txtQty) > 0)) {
                        $thisTr.css('background-color', 'Olive');
                        IsGlobalErrorMsgShow = true;
                        errorMsg += "<b style='color:Olive;'>" + aData.ItemNumber + ": " + MsgreqQtyToPull+"</b><br/>"
                    }

                    if (!(!isNaN(parseInt(vBinID)) && parseInt(vBinID) > 0)) {
                        $thisTr.css('background-color', 'Olive');
                        IsGlobalErrorMsgShow = true;
                        errorMsg += "<b style='color:Olive;'>" + aData.ItemNumber + ": " + MsgInventoryLocationMandatory +"</b><br/>"
                    }


                }
                else {

                    if (!(!isNaN(parseFloat(txtQty)) && parseFloat(txtQty) > 0)) {
                        $thisTr.css('background-color', 'Olive');
                        IsGlobalErrorMsgShow = true;
                        errorMsg += "<b style='color:Olive;'>" + aData.ItemNumber + ": " + MsgLabourItemRequiredHours +"</b><br/>"
                    }
                    vBinID = 0;
                    vProjectID = '';
                    ProjectSpendName = '';
                }


                if (errorMsg.length <= 0) {
                    var itemData = _NewConsumePull.getItemDataFromRow($thisTr);
                    var vItemID = itemData.spnItemID; //$thisTr.find('#spnItemID').text();
                    var vItemGUID = itemData.spnItemGUID; //$thisTr.find('#spnItemGUID').text();
                    var vspnOn_HandQuantity = itemData.spnOn_HandQuantity; //$thisTr.find('#spnOn_HandQuantity').text() == "" ? 0 : $thisTr.find('#spnOn_HandQuantity').text();
                    var vPullCreditText = "pull"; //$(obj)[0].value;//$(obj).parent().parent().find('input[name=colors'+vItemID+']:checked')[0].value;
                    var VspnDefaultPullQuantity = _NewConsumePull.getDataFromRow($thisTr,'spnDefaultPullQuantity'); //$this.find('#spnDefaultPullQuantity').text() == "" ? 0 : $this.find('#spnDefaultPullQuantity').text();
                    var vUDF1 = ''; var vUDF2 = ''; var vUDF3 = ''; var vUDF4 = ''; var vUDF5 = '';
                    var vUDF1PullCommon = ''; var vUDF2PullCommon = ''; var vUDF3PullCommon = ''; var vUDF4PullCommon = ''; var vUDF5PullCommon = '';

                    //var vPullOrderNumber = "";
                    //if ($(this).find('#txtPullOrderNumber') != null) {
                    //    vPullOrderNumber = $(this).find('#txtPullOrderNumber').val();
                    //}
                    var vPullOrderNumber = "";

                    if ($("#chkUsePullCommonUDF").is(":checked")) {
                        if ($('#txtPullOrderNumberCommon') != null) {
                            if ($('#txtPullOrderNumberCommon').attr("class") == 'selectBox') {
                                vPullOrderNumber = $('#txtPullOrderNumberCommon option:selected').text();
                            }
                            else {
                                vPullOrderNumber = $('#txtPullOrderNumberCommon').val();
                            }
                        }
                    }
                    else {
                        if ($thisTr.find('#txtPullOrderNumber') != null) {
                            if ($thisTr.find('#txtPullOrderNumber').attr("class") == 'selectBox') {
                                vPullOrderNumber = $thisTr.find('#txtPullOrderNumber option:selected').text();
                            }
                            else {
                                vPullOrderNumber = $thisTr.find('#txtPullOrderNumber').val();
                            }
                        }
                    }

                    vUDF1PullCommon = _NewConsumePull.GetUdfVal($("#UDF1PullCommon"));
                    vUDF2PullCommon = _NewConsumePull.GetUdfVal($("#UDF2PullCommon"));
                    vUDF3PullCommon = _NewConsumePull.GetUdfVal($("#UDF3PullCommon"));
                    vUDF4PullCommon = _NewConsumePull.GetUdfVal($("#UDF4PullCommon"));
                    vUDF5PullCommon = _NewConsumePull.GetUdfVal($("#UDF5PullCommon"));

                    vUDF1 = _NewConsumePull.GetUdfVal($thisTr.find('#UDF1'));
                    vUDF2 = _NewConsumePull.GetUdfVal($thisTr.find('#UDF2'));
                    vUDF3 = _NewConsumePull.GetUdfVal($thisTr.find('#UDF3'));
                    vUDF4 = _NewConsumePull.GetUdfVal($thisTr.find('#UDF4'));
                    vUDF5 = _NewConsumePull.GetUdfVal($thisTr.find('#UDF5'));


                    //if ($("#UDF1PullCommon") != null) {
                    //    if ($("#UDF1PullCommon").attr("class") == 'selectBox') {
                    //        vUDF1PullCommon = $("#UDF1PullCommon option:selected").text();
                    //    }
                    //    else {
                    //        vUDF1PullCommon = $("#UDF1PullCommon").val();
                    //    }
                    //}

                    //if ($("#UDF2PullCommon") != null) {
                    //    if ($("#UDF2PullCommon").attr("class") == 'selectBox') {
                    //        vUDF2PullCommon = $("#UDF2PullCommon option:selected").text();
                    //    }
                    //    else {
                    //        vUDF2PullCommon = $("#UDF2PullCommon").val();
                    //    }
                    //}

                    //if ($("#UDF3PullCommon") != null) {
                    //    if ($("#UDF3PullCommon").attr("class") == 'selectBox') {
                    //        vUDF3PullCommon = $("#UDF3PullCommon option:selected").text();
                    //    }
                    //    else {
                    //        vUDF3PullCommon = $("#UDF3PullCommon").val();
                    //    }
                    //}

                    //if ($("#UDF4PullCommon") != null) {
                    //    if ($("#UDF4PullCommon").attr("class") == 'selectBox') {
                    //        vUDF4PullCommon = $("#UDF4PullCommon option:selected").text();
                    //    }
                    //    else {
                    //        vUDF4PullCommon = $("#UDF4PullCommon").val();
                    //    }
                    //}

                    //if ($("#UDF5PullCommon") != null) {
                    //    if ($("#UDF5PullCommon").attr("class") == 'selectBox') {
                    //        vUDF5PullCommon = $("#UDF5PullCommon option:selected").text();
                    //    }
                    //    else {
                    //        vUDF5PullCommon = $("#UDF5PullCommon").val();
                    //    }
                    //}

                    //if ($thisTr.find('#UDF1') != null) {
                    //    if ($thisTr.find('#UDF1').attr("class") == 'selectBox') {
                    //        vUDF1 = $thisTr.find('#UDF1 option:selected').text();
                    //    }
                    //    else {
                    //        vUDF1 = $thisTr.find('#UDF1').val();
                    //    }
                    //}

                    //if ($thisTr.find('#UDF2') != null) {
                    //    if ($thisTr.find('#UDF2').attr("class") == 'selectBox') {
                    //        vUDF2 = $thisTr.find('#UDF2 option:selected').text();
                    //    }
                    //    else {
                    //        vUDF2 = $thisTr.find('#UDF2').val();
                    //    }
                    //}

                    //if ($thisTr.find('#UDF3') != null) {
                    //    if ($thisTr.find('#UDF3').attr("class") == 'selectBox') {
                    //        vUDF3 = $thisTr.find('#UDF3 option:selected').text();
                    //    }
                    //    else {
                    //        vUDF3 = $thisTr.find('#UDF3').val();
                    //    }
                    //}

                    //if ($thisTr.find('#UDF4') != null) {
                    //    if ($thisTr.find('#UDF4').attr("class") == 'selectBox') {
                    //        vUDF4 = $thisTr.find('#UDF4 option:selected').text();
                    //    }
                    //    else {
                    //        vUDF4 = $thisTr.find('#UDF4').val();
                    //    }
                    //}

                    //if ($thisTr.find('#UDF5') != null) {
                    //    if ($thisTr.find('#UDF5').attr("class") == 'selectBox') {
                    //        vUDF5 = $thisTr.find('#UDF5 option:selected').text();
                    //    }
                    //    else {
                    //        vUDF5 = $thisTr.find('#UDF5').val();
                    //    }
                    //}

                    if ($("#chkUsePullCommonUDF").is(":checked")) {
                        vUDF1 = vUDF1PullCommon;
                        vUDF2 = vUDF2PullCommon;
                        vUDF3 = vUDF3PullCommon;
                        vUDF4 = vUDF4PullCommon;
                        vUDF5 = vUDF5PullCommon;
                    }
                    var vPullSupplierAccountNumber = "";
                    if ($thisTr.find('#hdnSupplierAccountNumber') != null) {
                        vPullSupplierAccountNumber = $thisTr.find('#hdnSupplierAccountNumber').val();
                    }
                    
                    $('#DivLoading').show();
                    ///////////////////////////////////// UPDATE PULL DATA CALL//////////////////////////////////////////START
                    $.ajax({
                        "url": _NewConsumePull.urls.UpdatePullDataUrl, //'@Url.Content("~/Pull/UpdatePullData")',
                        //data: { ID: 0, ItemGUID: vItemGUID, ProjectGUID: vProjectID, BinID: vBinID, PullCreditQuantity: (txtQty * QuickListPULLQty), PullCredit: vPullCreditText, TempPullQTY: (txtQty * QuickListPULLQty), UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5, RequisitionDetailID: "", WorkOrderDetailGUID: '@TempData["WorkOrderGUID"]', ProjectSpendName: ProjectSpendName },
                        //data: { ID: 0, ItemGUID: vItemGUID, ProjectGUID: vProjectID, BinID: vBinID, PullCreditQuantity: (txtQty), PullCredit: vPullCreditText, TempPullQTY: (txtQty), UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5, RequisitionDetailID: "", WorkOrderDetailGUID: tmpdWorkOrderGUID, ProjectSpendName: ProjectSpendName, PullOrderNumber: vPullOrderNumber },
                        data: { ID: 0, ItemGUID: vItemGUID, ProjectGUID: vProjectID, BinID: vBinID, PullCreditQuantity: (txtQty), PullCredit: vPullCreditText, TempPullQTY: (txtQty), UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5, RequisitionDetailID: "", WorkOrderDetailGUID: tmpdWorkOrderGUID, ProjectSpendName: ProjectSpendName, PullOrderNumber: vPullOrderNumber, SupplierAccountNumberGuid: vPullSupplierAccountNumber, PullType: PullType },
                        "async": false,
                        cache: false,
                        "dataType": "json",
                        success: function (response) {
                            
                            $('#DivLoading').hide();
                            if (response.Status == "ok") {
                                if (response.LocationMSG != "") {
                                    if (response.PSLimitExceed) {
                                        $(RowObject).css('background-color', 'Yellow');
                                        IsPSRedirectToReq = true;
                                        IsGlobalErrorMsgShow = true;
                                        IsQLSuccessfullyDone = false;
                                        errorMsg += "<b style='color:Olive'>" + aData.ItemNumber + ": " + response.LocationMSG + " " + msgProjectSpendLimitConfirmation+"</b><br>"
                                    }
                                    else {
                                        $(RowObject).css('background-color', 'Olive');
                                        //IsPSRedirectToReq = false;
                                        IsGlobalErrorMsgShow = true;
                                        IsQLSuccessfullyDone = false;
                                        errorMsg += "<b style='color:Olive'>" + aData.ItemNumber + ": " + response.LocationMSG + "</b><br>"
                                    }
                                }
                                else {

                                    UDFInsertNewForGrid(RowObject);
                                    $(RowObject).css('background-color', 'Green');
                                    errorMsg += "<b style='color:Green'>" + aData.ItemNumber + ": " + response.Message + "</b><br>"
                                }
                            }
                            else {
                                $(RowObject).css('background-color', 'Red');
                                IsPSRedirectToReq = false;
                                IsGlobalErrorMsgShow = true;
                                IsQLSuccessfullyDone = false;
                                errorMsg += "<b style='color:Red'>" + aData.ItemNumber + ": " + response.Message + "</b><br>"
                            }
                            txxt.val(VspnDefaultPullQuantity);
                            // $(obj).removeAttr("disabled");
                        },
                        error: function (response) {

                            $(RowObject).css('background-color', 'Red');
                            IsPSRedirectToReq = false;
                            IsGlobalErrorMsgShow = true;
                            IsQLSuccessfullyDone = false;
                            errorMsg += "<b style='color:Red'>" + aData.ItemNumber + ": " + MsgErrorInProcess+"</b><br>"
                            $(obj).removeAttr("disabled");
                        }
                    });
                    ///////////////////////////////////// UPDATE PULL DATA CALL//////////////////////////////////////////END
                }

                //////////////////////////////////////END//////////////////////////////////////
            }
            errGBLMsg += errorMsg;
        });

        $('#DivLoading').show();
        if (IsQLSuccessfullyDone) {
            $.modal.impl.close();
            $('#DivLoading').hide();
            LoadAllItemDataWithFilter();
        }
        else if (IsPSRedirectToReq) {
            $.modal.impl.close();
            errGBLMsg = "<b>" + SomeItemNotPulled +"</b><br /><br />" + errGBLMsg;
            $('#DivLoading').hide();
            $('#dlgCommonErrorMsg').find("#pOkbtn").css('display', 'none');
            $('#dlgCommonErrorMsg').find("#pYesNobtn").css('display', '');
            $('#dlgCommonErrorMsg').find("#pErrMessage").html(errGBLMsg);
            $('#dlgCommonErrorMsg').modal()
            $obj.removeAttr("disabled");
        }
        else if (IsGlobalErrorMsgShow) {
            $.modal.impl.close();
            $('#DivLoading').hide();
            errGBLMsg = "<b>" + SomeItemNotPulled +"</b><br /><br />" + errGBLMsg;
            $('#dlgCommonErrorMsg').find("#pOkbtn").css('display', '');
            $('#dlgCommonErrorMsg').find("#pYesNobtn").css('display', 'none');
            $('#dlgCommonErrorMsg').find("#pErrMessage").html(errGBLMsg);

            $('#dlgCommonErrorMsg').modal()
            $obj.removeAttr("disabled");
        }
    }
    else if ($($(obj)[0]).data("val") == 'Pull ALL') {

        var IsGlobalErrorMsgShow = false;
        var IsPSRedirectToReq = false;
        //$('#DivLoading').show();
        var multiplePullGUIDs = ",";
        var errGBLMsg = ''; $('#DivLoading').show();
        $('#ItemModeDataTable tbody tr.row_selected').each(function (i) {
            var $thisTr2 = $(this);
            if ($thisTr2.attr('class').indexOf('row_selected') != -1) {
                $thisTr2.toggleClass('row_selected');
                var errorMsg = '';
                var RowObject = $thisTr2;
                var aPos = $('#ItemModeDataTable').dataTable().fnGetPosition($thisTr2[0]);
                var aData = $('#ItemModeDataTable').dataTable().fnGetData(aPos);
                var txxt = $thisTr2.find('#txtQty');
                var vBinID;
                var vProjectID;
                var itemData2 = _NewConsumePull.getItemDataFromRow($thisTr2);
                var itemType = itemData2.spnOrderItemType; //$thisTr2.find('#spnOrderItemType').text();
                var txtQty = txxt.val();
                if (itemType != '4') {
                    vBinID = _NewConsumePull.getDataFromRow($thisTr2, 'BinID'); //$thisTr2.find('#BinID')[0].value == '' ? 0 : $thisTr2.find('#BinID')[0].value;
                    vBinID = vBinID == '' ? 0 : vBinID;

                     
                        if ($("#chkUsePullCommonUDF").is(":checked")) {
                            if ($('#ProjectIDCommon') != undefined) {
                                vProjectID = $('#ProjectIDCommon').val() == "" ? "" : $('#ProjectIDCommon').val();
                            }
                            else {
                                vProjectID = "";
                            }
                            if ($('#txtProjectSpentCommon').val() != '') {
                                ProjectSpendName = $('#txtProjectSpentCommon').val();
                            }
                        }
                        else {
                            //var eProjectID = _NewConsumePull.getDataEleFromRow($thisTr2, 'ProjectID');
    
                            ////if ($thisTr2.find('#ProjectID')[0] != undefined) {
                            //if (typeof eProjectID !== 'undefined' && eProjectID !== null) {
                            //    //vProjectID = $thisTr2.find('#ProjectID')[0].value == "" ? "" : $thisTr2.find('#ProjectID')[0].value;
                            //    vProjectID = eProjectID.val();
                            //}
                            //else {
                            //    vProjectID = "";
                            //}
                            vProjectID = _NewConsumePull.GetProjectId($thisTr2, 'ProjectID');

                            if ($thisTr2.find('#txtProjectSpent').val() != '') {
                                ProjectSpendName = $thisTr2.find('#txtProjectSpent').val();
                            }
                        }
                     
                     
                    

                    // vProjectID = $(this).find('#ProjectID')[0].value == "" ? "" : $(this).find('#ProjectID')[0].value;
                    
                    /*if ((vProjectID == undefined || typeof vProjectID == "undefined" || vProjectID == "") && (ProjectSpendName != undefined && ProjectSpendName != "")) {
		                $.ajax({
			                url: "/Pull/GetProjectSpend",
			                data: { "ProjectSpend": ProjectSpendName },
			                type: "Get",
			                dataType: 'json',
			                async: false,
			                cache: false,
			                success: function (data) {
				                if (data.vProjectID != null && data.vProjectID != undefined && data.vProjectID != "") {
					                vProjectID = data.vProjectID;
				                }
			                }
		                });
	                }*/
                     
                    
                    if((hasOnTheFlyEntryRight != "True" || isProjectSpendInsertAllow != "True") && (vProjectID == undefined || typeof vProjectID == "undefined" || vProjectID == "") && (ProjectSpendName != undefined && ProjectSpendName != "")) {
                        $thisTr2.css('background-color', 'Olive');
                        IsGlobalErrorMsgShow = true;  
                        //errorMsg += "<b style='color:Olive;'>" + aData.ItemNumber + ": No right to insert new projectspend or Ontheflyright.</b><br/>"
                        errorMsg += "<b style='color:Olive;'>" + aData.ItemNumber + ":" + noProjectspendOntheFlyRight +"</b><br/>"
                    }

                    if (!(!isNaN(parseFloat(txtQty)) && parseFloat(txtQty) > 0)) {
                        $thisTr2.css('background-color', 'Olive');
                        IsGlobalErrorMsgShow = true;
                        errorMsg += "<b style='color:Olive;'>" + aData.ItemNumber + ": " + MsgQtyToPullMandatory+"</b><br/>"
                    }

                    if (!(!isNaN(parseInt(vBinID)) && parseInt(vBinID) > 0)) {
                        $thisTr2.css('background-color', 'Olive');
                        IsGlobalErrorMsgShow = true;
                        errorMsg += "<b style='color:Olive;'>" + aData.ItemNumber + ": " + MsgInventoryLocationMandatory+"</b><br/>"
                    }

                }
                else {

                    if (!(!isNaN(parseFloat(txtQty)) && parseFloat(txtQty) > 0)) {
                        $thisTr2.css('background-color', 'Olive');
                        IsGlobalErrorMsgShow = true;
                        errorMsg += "<b style='color:Olive'>" + aData.ItemNumber + ": " + MsgLabourItemRequiredHours +"</b><br>"
                    }
                    vBinID = 0;
                    vProjectID = '';
                }


                var trimtxtVal = txtQty.replace(/ /g, '');
                //if (trimtxtVal.length > 0) {
                if (errorMsg.length <= 0) {
                    var itemData2 = _NewConsumePull.getItemDataFromRow($thisTr2);
                    var vItemID = itemData2.spnItemID; //$thisTr2.find('#spnItemID').text();
                    var vItemGUID = itemData2.spnItemGUID; //$thisTr2.find('#spnItemGUID').text();
                    var vspnOn_HandQuantity = itemData2.spnOn_HandQuantity; //$thisTr2.find('#spnOn_HandQuantity').text() == "" ? 0 : $thisTr2.find('#spnOn_HandQuantity').text();
                    var vPullCreditText = "pull"; //$(obj)[0].value;//$(obj).parent().parent().find('input[name=colors'+vItemID+']:checked')[0].value;

                    var VspnDefaultPullQuantity = _NewConsumePull.getDataFromRow($thisTr2, 'spnDefaultPullQuantity'); //$thisTr2.find('#spnDefaultPullQuantity').text() == "" ? 0 : $thisTr2.find('#spnDefaultPullQuantity').text();

                    var vUDF1 = ''; var vUDF2 = ''; var vUDF3 = ''; var vUDF4 = ''; var vUDF5 = '';
                    var vUDF1PullCommon = ''; var vUDF2PullCommon = ''; var vUDF3PullCommon = ''; var vUDF4PullCommon = ''; var vUDF5PullCommon = '';

                    //var vPullOrderNumber = "";
                    //if ($(this).find('#txtPullOrderNumber') != null) {
                    //    vPullOrderNumber = $(this).find('#txtPullOrderNumber').val();
                    //}
                    var vPullOrderNumber = "";
                    if ($("#chkUsePullCommonUDF").is(":checked")) {
                        if ($('#txtPullOrderNumberCommon') != null) {
                            if ($('#txtPullOrderNumberCommon').attr("class") == 'selectBox') {
                                vPullOrderNumber = $('#txtPullOrderNumberCommon option:selected').text();
                            }
                            else {
                                vPullOrderNumber = $('#txtPullOrderNumberCommon').val();
                            }
                        }
                    }
                    else {
                        if ($thisTr2.find('#txtPullOrderNumber') != null) {
                            if ($thisTr2.find('#txtPullOrderNumber').attr("class") == 'selectBox') {
                                vPullOrderNumber = $thisTr2.find('#txtPullOrderNumber option:selected').text();
                            }
                            else {
                                vPullOrderNumber = $thisTr2.find('#txtPullOrderNumber').val();
                            }
                        }
                    }

                    vUDF1PullCommon = _NewConsumePull.GetUdfVal($("#UDF1PullCommon"));
                    vUDF2PullCommon = _NewConsumePull.GetUdfVal($("#UDF2PullCommon"));
                    vUDF3PullCommon = _NewConsumePull.GetUdfVal($("#UDF3PullCommon"));
                    vUDF4PullCommon = _NewConsumePull.GetUdfVal($("#UDF4PullCommon"));
                    vUDF5PullCommon = _NewConsumePull.GetUdfVal($("#UDF5PullCommon"));

                    vUDF1 = _NewConsumePull.GetUdfVal($thisTr2.find('#UDF1'));
                    vUDF2 = _NewConsumePull.GetUdfVal($thisTr2.find('#UDF2'));
                    vUDF3 = _NewConsumePull.GetUdfVal($thisTr2.find('#UDF3'));
                    vUDF4 = _NewConsumePull.GetUdfVal($thisTr2.find('#UDF4'));
                    vUDF5 = _NewConsumePull.GetUdfVal($thisTr2.find('#UDF5'));


                    //if ($("#UDF1PullCommon") != null) {
                    //    if ($("#UDF1PullCommon").attr("class") == 'selectBox') {
                    //        vUDF1PullCommon = $("#UDF1PullCommon option:selected").text();
                    //    }
                    //    else {
                    //        vUDF1PullCommon = $("#UDF1PullCommon").val();
                    //    }
                    //}

                    //if ($("#UDF2PullCommon") != null) {
                    //    if ($("#UDF2PullCommon").attr("class") == 'selectBox') {
                    //        vUDF2PullCommon = $("#UDF2PullCommon option:selected").text();
                    //    }
                    //    else {
                    //        vUDF2PullCommon = $("#UDF2PullCommon").val();
                    //    }
                    //}

                    //if ($("#UDF3PullCommon") != null) {
                    //    if ($("#UDF3PullCommon").attr("class") == 'selectBox') {
                    //        vUDF3PullCommon = $("#UDF3PullCommon option:selected").text();
                    //    }
                    //    else {
                    //        vUDF3PullCommon = $("#UDF3PullCommon").val();
                    //    }
                    //}

                    //if ($("#UDF4PullCommon") != null) {
                    //    if ($("#UDF4PullCommon").attr("class") == 'selectBox') {
                    //        vUDF4PullCommon = $("#UDF4PullCommon option:selected").text();
                    //    }
                    //    else {
                    //        vUDF4PullCommon = $("#UDF4PullCommon").val();
                    //    }
                    //}

                    //if ($("#UDF5PullCommon") != null) {
                    //    if ($("#UDF5PullCommon").attr("class") == 'selectBox') {
                    //        vUDF5PullCommon = $("#UDF5PullCommon option:selected").text();
                    //    }
                    //    else {
                    //        vUDF5PullCommon = $("#UDF5PullCommon").val();
                    //    }
                    //}

                    //if ($thisTr2.find('#UDF1') != null) {
                    //    if ($thisTr2.find('#UDF1').attr("class") == 'selectBox') {
                    //        vUDF1 = $thisTr2.find('#UDF1 option:selected').text();
                    //    }
                    //    else {
                    //        vUDF1 = $thisTr2.find('#UDF1').val();
                    //    }
                    //}

                    //if ($thisTr2.find('#UDF2') != null) {
                    //    if ($thisTr2.find('#UDF2').attr("class") == 'selectBox') {
                    //        vUDF2 = $thisTr2.find('#UDF2 option:selected').text();
                    //    }
                    //    else {
                    //        vUDF2 = $thisTr2.find('#UDF2').val();
                    //    }
                    //}

                    //if ($thisTr2.find('#UDF3') != null) {
                    //    if ($thisTr2.find('#UDF3').attr("class") == 'selectBox') {
                    //        vUDF3 = $thisTr2.find('#UDF3 option:selected').text();
                    //    }
                    //    else {
                    //        vUDF3 = $thisTr2.find('#UDF3').val();
                    //    }
                    //}

                    //if ($thisTr2.find('#UDF4') != null) {
                    //    if ($thisTr2.find('#UDF4').attr("class") == 'selectBox') {
                    //        vUDF4 = $thisTr2.find('#UDF4 option:selected').text();
                    //    }
                    //    else {
                    //        vUDF4 = $thisTr2.find('#UDF4').val();
                    //    }
                    //}

                    //if ($thisTr2.find('#UDF5') != null) {
                    //    if ($thisTr2.find('#UDF5').attr("class") == 'selectBox') {
                    //        vUDF5 = $thisTr2.find('#UDF5 option:selected').text();
                    //    }
                    //    else {
                    //        vUDF5 = $thisTr2.find('#UDF5').val();
                    //    }
                    //}

                    if ($("#chkUsePullCommonUDF").is(":checked")) {
                        vUDF1 = vUDF1PullCommon;
                        vUDF2 = vUDF2PullCommon;
                        vUDF3 = vUDF3PullCommon;
                        vUDF4 = vUDF4PullCommon;
                        vUDF5 = vUDF5PullCommon;
                    }
                    var vPullSupplierAccountNumber = "";
                    if ($thisTr2.find('#hdnSupplierAccountNumber') != null) {
                        vPullSupplierAccountNumber = $thisTr2.find('#hdnSupplierAccountNumber').val();
                    }
                    
                    $('#DivLoading').show();
                    ///////////////////////////////////// UPDATE PULL DATA CALL//////////////////////////////////////////START
                    $.ajax({
                        "url": _NewConsumePull.urls.UpdatePullDataUrl, //'@Url.Content("~/Pull/UpdatePullData")',
                        //data: { ID: 0, ItemGUID: vItemGUID, ProjectGUID: vProjectID, BinID: vBinID, PullCreditQuantity: txtQty, PullCredit: vPullCreditText, TempPullQTY: txtQty, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5, RequisitionDetailID: "", WorkOrderDetailGUID: tmpdWorkOrderGUID, ProjectSpendName: ProjectSpendName, PullOrderNumber: vPullOrderNumber, callFrom: 'multiplepull' },
                        data: { ID: 0, ItemGUID: vItemGUID, ProjectGUID: vProjectID, BinID: vBinID, PullCreditQuantity: txtQty, PullCredit: vPullCreditText, TempPullQTY: txtQty, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5, RequisitionDetailID: "", WorkOrderDetailGUID: tmpdWorkOrderGUID, ProjectSpendName: ProjectSpendName, PullOrderNumber: vPullOrderNumber, SupplierAccountNumberGuid: vPullSupplierAccountNumber, callFrom: 'multiplepull' },
                        "async": false,
                        cache: false,
                        "dataType": "json",
                        success: function (response) {
                            
                            $('#DivLoading').hide();
                            if (response.Status == "ok") {
                                if (response.LocationMSG != "") {
                                    if (response.PSLimitExceed) {
                                        $(RowObject).css('background-color', 'Yellow');
                                        IsPSRedirectToReq = true;
                                        IsGlobalErrorMsgShow = true;
                                        errorMsg += "<b style='color:Olive'>" + aData.ItemNumber + ": " + response.LocationMSG + " " + msgProjectSpendLimitConfirmation+"</b><br>"
                                    }
                                    else {
                                        $(RowObject).css('background-color', 'Olive');
                                        IsGlobalErrorMsgShow = true;
                                        errorMsg += "<b style='color:Olive'>" + aData.ItemNumber + ": " + response.LocationMSG + "</b><br>"
                                    }
                                }
                                else {
                                    //if (response.SupPOParitList != "")
                                    {
                                        BindNewPONumber(response.SupPOParitList);
                                    }

                                    UDFInsertNewForGrid(RowObject);
                                    RefreshPullNarrowSearch(); // Maintain narrow search value - WI-329
                                    NSForItemModel_ExecuteOnDocReady();
                                    $(RowObject).css('background-color', 'Green');
                                    errorMsg += "<b style='color:Green'>" + aData.ItemNumber + ": " + response.Message + "</b><br>"
                                }
                                if (response.GeneratedPullGUID != '') {
                                    multiplePullGUIDs += response.GeneratedPullGUID + ",";
                                }


                            }
                            else {
                                $(RowObject).css('background-color', 'Red');
                                IsPSRedirectToReq = false;
                                IsGlobalErrorMsgShow = true;
                                errorMsg += "<b style='color:Red'>" + aData.ItemNumber + ": " + response.Message + "</b><br>"
                            }
                            txxt.val(VspnDefaultPullQuantity);
                            $obj.removeAttr("disabled");
                        },
                        error: function (response) {
                            $(RowObject).css('background-color', 'Red');
                            IsPSRedirectToReq = false;
                            IsGlobalErrorMsgShow = true;
                            errorMsg += "<b style='color:Red'>" + aData.ItemNumber + ": " + MsgErrorInProcess+"</b><br>"
                            $obj.removeAttr("disabled");
                        }
                    });
                    ///////////////////////////////////// UPDATE PULL DATA CALL//////////////////////////////////////////END
                }
                //////////////////////////////////////END//////////////////////////////////////
            }
            errGBLMsg += errorMsg;
        });
        if (multiplePullGUIDs.length > 2) {
            $.ajax({
                "url": _NewConsumePull.urls.SavePullGuidsInScheduleUrl, //'@Url.Content("~/Pull/SavePullGuidsInSchedule")',
                data: { DataGuids: multiplePullGUIDs },
                "async": false,
                cache: false,
                "dataType": "json",
                success: function (response) {

                    //if (response.Status == "ok") {

                    //}

                    $obj.removeAttr("disabled");
                }
            });
        }

        $('#DivLoading').show();
        $.modal.impl.close();
        if (IsPSRedirectToReq) {
            errGBLMsg = "<b>" + SomeItemNotPulled +"</b><br /><br />" + errGBLMsg;
            $('#dlgCommonErrorMsg').find("#pOkbtn").css('display', 'none');
            $('#DivLoading').hide();
            $('#dlgCommonErrorMsg').find("#pYesNobtn").css('display', '');

        }
        else if (IsGlobalErrorMsgShow) {
            errGBLMsg = "<b>" + SomeItemNotPulled +"</b><br /><br />" + errGBLMsg;
            $('#DivLoading').hide();
            $('#dlgCommonErrorMsg').find("#pOkbtn").css('display', '');
            $('#dlgCommonErrorMsg').find("#pYesNobtn").css('display', 'none');

        }
        else if (!IsGlobalErrorMsgShow) {
            errGBLMsg = "<b>" + MsgPullDoneSuccess +"</b><br /><br />" + errGBLMsg;
            $('#DivLoading').hide();
            $('#dlgCommonErrorMsg').find("#pOkbtn").css('display', '');
            $('#dlgCommonErrorMsg').find("#pYesNobtn").css('display', 'none');

        }
        $('#dlgCommonErrorMsg').find("#pErrMessage").html(errGBLMsg);
        $('#dlgCommonErrorMsg').modal({
            escClose: false,
            close: false
        });
        $(obj).removeAttr("disabled");
    }
    else if ($($(obj)[0]).data("val") == 'Pull') {
        //$('#DivLoading').show();

        ShowHideNewPullProgressBar(true);

        setTimeout(function () {


            //window.confirm("test");
            var txxt = $tr.find('#txtQty');
            var vBinID;
            var vProjectID;
            var itemData = _NewConsumePull.getItemDataFromRow($tr);
            var itemType = itemData.spnOrderItemType; //$td.find('#spnOrderItemType').text();
            var txtQty = txxt.val();
            if (itemType != '4') {
                
                vBinID = _NewConsumePull.getDataFromRow($tr, 'BinID'); //$tr.find('#BinID')[0].value == '' ? 0 : $tr.find('#BinID')[0].value;
                vBinID = vBinID == '' ? 0 : vBinID;
                 
                    if ($("#chkUsePullCommonUDF").is(":checked")) {
                        if ($('#ProjectIDCommon') != undefined) {
                            vProjectID = $('#ProjectIDCommon').val() == "" ? "" : $('#ProjectIDCommon').val();
                        }
                        else {
                            vProjectID = "";
                        }

                        if ($('#txtProjectSpentCommon').val() != '') {
                            ProjectSpendName = $('#txtProjectSpentCommon').val();
                        }
                    }
                    else {
                        //var eProjectID = _NewConsumePull.getDataEleFromRow($tr, 'ProjectID');

                        ////if ($tr.find('#ProjectID')[0] != undefined) {
                        //if (typeof eProjectID !== 'undefined' && eProjectID !== null) {
                        //    //vProjectID = $(obj).parent().parent().find('#ProjectID')[0].value == "" ? "" : $(obj).parent().parent().find('#ProjectID')[0].value;
                        //    vProjectID = eProjectID.val();
                        //}
                        //else {
                        //    vProjectID = "";
                        //}
                        vProjectID = _NewConsumePull.GetProjectId($tr);

                        if ($tr.find('#txtProjectSpent').val() != '') {
                            ProjectSpendName = $tr.find('#txtProjectSpent').val();
                        }
                    }
                 
                
                 
                /*if ((vProjectID == undefined || typeof vProjectID == "undefined" || vProjectID == "") && (ProjectSpendName != undefined && ProjectSpendName != "")) {
                    $.ajax({
                        url: "/Pull/GetProjectSpend",
                        data: { "ProjectSpend": ProjectSpendName },
                        type: "Get",
                        dataType: 'json',
                        async: false,
                        cache: false,
                        success: function (data) {
                            if (data.vProjectID != null && data.vProjectID != undefined && data.vProjectID != "") {
                                vProjectID = data.vProjectID;
                            }
                        }
                    });
                }*/

                if (isProjectSpendMandatoryInRoom == "True" && (vProjectID == undefined || typeof vProjectID == "undefined" || vProjectID == "") && (ProjectSpendName == undefined || ProjectSpendName == "")) {
                    ShowHideNewPullProgressBar(false);
                    $("#spanGlobalMessage").html(MsgProjectSpendMandatory);
                    $('div#target').fadeToggle();
                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    ////enablePullbutton();
                    //$('#DivLoading').hide();
                    $(obj).removeAttr("disabled");
                    return false;
                }
                if((hasOnTheFlyEntryRight != "True" || isProjectSpendInsertAllow != "True") && (vProjectID == undefined || typeof vProjectID == "undefined" || vProjectID == "") && (ProjectSpendName != undefined && ProjectSpendName != "")) {
                    ShowHideNewPullProgressBar(false);
                    //$("#spanGlobalMessage").text("No right to insert new projectspend or Ontheflyright");
                    $("#spanGlobalMessage").html(noProjectspendOntheFlyRight);
                    $('div#target').fadeToggle();
                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    $('#DivLoading').hide();
                    $(obj).removeAttr("disabled");
                    return false;
                }

                if ((txtQty == '' || txtQty == 'undefined') && txtQty.length == 0) {
                    ShowHideNewPullProgressBar(false);
                    $("#spanGlobalMessage").html(MsgQtyToPullMandatory);
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    showNotificationDialog();
                    $('#DivLoading').hide();
                    ////enablePullbutton();
                    $(obj).removeAttr("disabled");
                    return false;

                }
                if (vBinID == 0) {
                    ShowHideNewPullProgressBar(false);
                    $("#spanGlobalMessage").html(MsgInventoryLocationMandatory);
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    showNotificationDialog();
                    ////enablePullbutton();
                    $('#DivLoading').hide();
                    $(obj).removeAttr("disabled");
                    return false;
                }
            }
            else {

                if ($("#chkUsePullCommonUDF").is(":checked")) {
                    if ($('#ProjectIDCommon') != undefined)
                        vProjectID = $('#ProjectIDCommon').val() == "" ? "" : $('#ProjectIDCommon').val();
                    else
                        vProjectID = "";
                    if ($('#txtProjectSpentCommon').val() != '') {
                        ProjectSpendName = $('#txtProjectSpentCommon').val();
                    }
                }
                else {
                    //var eProjectID = _NewConsumePull.getDataEleFromRow($tr, 'ProjectID');
                    
                    ////if ($tr.find('#ProjectID')[0] != undefined) {
                    //if (typeof eProjectID !== 'undefined' && eProjectID !== null) {
                    //    //vProjectID = $tr.find('#ProjectID')[0].value == "" ? "" : $tr.find('#ProjectID')[0].value;
                    //    vProjectID = eProjectID.val();
                    //}
                    //else {
                    //    vProjectID = "";
                    //}

                    vProjectID = _NewConsumePull.GetProjectId($tr);

                    if ($tr.find('#txtProjectSpent').val() != '') {
                        ProjectSpendName = $(obj).parent().parent().find('#txtProjectSpent').val();
                    }
                }

                /*if ((vProjectID == undefined || typeof vProjectID == "undefined" || vProjectID == "") && (ProjectSpendName != undefined && ProjectSpendName != "")) {
                    $.ajax({
                        url: "/Pull/GetProjectSpend",
                        data: { "ProjectSpend": ProjectSpendName },
                        type: "Get",
                        dataType: 'json',
                        async: false,
                        cache: false,
                        success: function (data) {
                            if (data.vProjectID != null && data.vProjectID != undefined && data.vProjectID != "") {
                                vProjectID = data.vProjectID;
                            }
                        }
                    });
                }*/
                if (isProjectSpendMandatoryInRoom == "True" && (vProjectID == undefined || typeof vProjectID == "undefined" || vProjectID == "") && (ProjectSpendName == undefined || ProjectSpendName == "")) {
                    ShowHideNewPullProgressBar(false);
                    $("#spanGlobalMessage").html(MsgProjectSpendMandatory);
                    $('div#target').fadeToggle();
                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    ////enablePullbutton();
                    //$('#DivLoading').hide();
                    $obj.removeAttr("disabled");
                    return false;
                }

                if((hasOnTheFlyEntryRight != "True" || isProjectSpendInsertAllow != "True") && (vProjectID == undefined || typeof vProjectID == "undefined" || vProjectID == "") && (ProjectSpendName != undefined && ProjectSpendName != "")) {
                    ShowHideNewPullProgressBar(false);
                    $("#spanGlobalMessage").html(noProjectspendOntheFlyRight);
                    $('div#target').fadeToggle();
                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    ////enablePullbutton();
                    //$('#DivLoading').hide();
                    $obj.removeAttr("disabled");
                    return false; 
                }


                if (txtQty == 'undefined' || txtQty == '') {
                    ShowHideNewPullProgressBar(false);
                    alert(MsgLabourItemRequiredHours);
                    ////enablePullbutton();
                    //$('#DivLoading').hide();
                    $obj.removeAttr("disabled");
                    return false;
                }
                if (parseFloat(txtQty) <= 0) {
                    ShowHideNewPullProgressBar(false);
                    alert(MsgLabourItemRequiredHours);
                    ////enablePullbutton();
                    //$('#DivLoading').hide();
                    $obj.removeAttr("disabled");
                    return false;
                }
                if (parseFloat(txtQty) == NaN) {
                    ShowHideNewPullProgressBar(false);
                    alert(MsgLabourItemRequiredHours);
                    ////enablePullbutton();
                    //$('#DivLoading').hide();
                    $obj.removeAttr("disabled");
                    return false;
                }

                vBinID = 0;
                //vProjectID = '';
                //ProjectSpendName = '';

            }


            var trimtxtVal = txtQty.replace(/ /g, '');
            if (trimtxtVal.length > 0) {
                var itemData = _NewConsumePull.getItemDataFromRow($tr);
                var vItemID = itemData.spnItemID; //$td.find('#spnItemID').text();
                var vItemGUID = itemData.spnItemGUID; //$td.find('#spnItemGUID').text();
                var vspnOn_HandQuantity = itemData.spnOn_HandQuantity; //$tr.find('#spnOn_HandQuantity').text() == "" ? 0 : $(obj).parent().parent().find('#spnOn_HandQuantity').text();
                var vPullCreditText = "pull"; //$(obj)[0].value;//$(obj).parent().parent().find('input[name=colors'+vItemID+']:checked')[0].value;

                var VspnDefaultPullQuantity = _NewConsumePull.getDataFromRow($tr, 'spnDefaultPullQuantity');//$tr.find('#spnDefaultPullQuantity').text() == "" ? 0 : $(obj).parent().parent().find('#spnDefaultPullQuantity').text();
                //var vhdnPullQtyScanOverride = _NewConsumePull.getDataFromRow($tr,'hdnPullQtyScanOverride'); //$tr.find('#hdnPullQtyScanOverride').val();

                var vPullQuantity = parseFloat(txtQty);
                var vDefaultPullQuantity = parseFloat(VspnDefaultPullQuantity);

                //                if (vhdnPullQtyScanOverride.toLowerCase() == "true" && vDefaultPullQuantity > 0) {
                //                    if (vPullQuantity < vDefaultPullQuantity || vPullQuantity % vDefaultPullQuantity != 0) {
                //                        alert('Pull must be default pull qty (' + vDefaultPullQuantity + ') or multiples of the default pull qty.');
                //                        enablePullbutton();
                //                        $('#DivLoading').hide();
                //                        txxt.val(VspnDefaultPullQuantity);
                //                        return false;
                //                    }
                //                }

                $('#DivLoading').show();

                var vUDF1 = ''; var vUDF2 = ''; var vUDF3 = ''; var vUDF4 = ''; var vUDF5 = '';
                var vUDF1PullCommon = ''; var vUDF2PullCommon = ''; var vUDF3PullCommon = ''; var vUDF4PullCommon = ''; var vUDF5PullCommon = '';

                var vPullOrderNumber = "";
                if ($("#chkUsePullCommonUDF").is(":checked")) {
                    if ($('#txtPullOrderNumberCommon') != null) {
                        if ($('#txtPullOrderNumberCommon').attr("class") == 'selectBox') {
                            vPullOrderNumber = $('#txtPullOrderNumberCommon option:selected').text();
                        }
                        else {
                            vPullOrderNumber = $('#txtPullOrderNumberCommon').val();
                        }
                    }
                }
                else {
                    if ($tr.find('#txtPullOrderNumber') != null) {
                        if ($(obj).parent().parent().find('#txtPullOrderNumber').attr("class") == 'selectBox') {
                            vPullOrderNumber = $tr.find('#txtPullOrderNumber option:selected').text();
                        }
                        else {
                            vPullOrderNumber = $tr.find('#txtPullOrderNumber').val();
                        }
                    }
                }

                vUDF1PullCommon = _NewConsumePull.GetUdfVal($("#UDF1PullCommon"));
                vUDF2PullCommon = _NewConsumePull.GetUdfVal($("#UDF2PullCommon"));
                vUDF3PullCommon = _NewConsumePull.GetUdfVal($("#UDF3PullCommon"));
                vUDF4PullCommon = _NewConsumePull.GetUdfVal($("#UDF4PullCommon"));
                vUDF5PullCommon = _NewConsumePull.GetUdfVal($("#UDF5PullCommon"));

                vUDF1 = _NewConsumePull.GetUdfVal($tr.find('#UDF1'));
                vUDF2 = _NewConsumePull.GetUdfVal($tr.find('#UDF2'));
                vUDF3 = _NewConsumePull.GetUdfVal($tr.find('#UDF3'));
                vUDF4 = _NewConsumePull.GetUdfVal($tr.find('#UDF4'));
                vUDF5 = _NewConsumePull.GetUdfVal($tr.find('#UDF5'));



                //if ($("#UDF1PullCommon") != null) {
                //    if ($("#UDF1PullCommon").attr("class") == 'selectBox') {
                //        vUDF1PullCommon = $("#UDF1PullCommon option:selected").text();
                //    }
                //    else {
                //        vUDF1PullCommon = $("#UDF1PullCommon").val();
                //    }
                //}

                //if ($("#UDF2PullCommon") != null) {
                //    if ($("#UDF2PullCommon").attr("class") == 'selectBox') {
                //        vUDF2PullCommon = $("#UDF2PullCommon option:selected").text();
                //    }
                //    else {
                //        vUDF2PullCommon = $("#UDF2PullCommon").val();
                //    }
                //}

                //if ($("#UDF3PullCommon") != null) {
                //    if ($("#UDF3PullCommon").attr("class") == 'selectBox') {
                //        vUDF3PullCommon = $("#UDF3PullCommon option:selected").text();
                //    }
                //    else {
                //        vUDF3PullCommon = $("#UDF3PullCommon").val();
                //    }
                //}

                //if ($("#UDF4PullCommon") != null) {
                //    if ($("#UDF4PullCommon").attr("class") == 'selectBox') {
                //        vUDF4PullCommon = $("#UDF4PullCommon option:selected").text();
                //    }
                //    else {
                //        vUDF4PullCommon = $("#UDF4PullCommon").val();
                //    }
                //}

                //if ($("#UDF5PullCommon") != null) {
                //    if ($("#UDF5PullCommon").attr("class") == 'selectBox') {
                //        vUDF5PullCommon = $("#UDF5PullCommon option:selected").text();
                //    }
                //    else {
                //        vUDF5PullCommon = $("#UDF5PullCommon").val();
                //    }
                //}

                //if ($tr.find('#UDF1') != null) {
                //    if ($tr.find('#UDF1').attr("class") == 'selectBox') {
                //        vUDF1 = $tr.find('#UDF1 option:selected').text();
                //    }
                //    else {
                //        vUDF1 = $tr.find('#UDF1').val();
                //    }
                //}

                //if ($tr.find('#UDF2') != null) {
                //    if ($tr.find('#UDF2').attr("class") == 'selectBox') {
                //        vUDF2 = $tr.find('#UDF2 option:selected').text();
                //    }
                //    else {
                //        vUDF2 = $tr.find('#UDF2').val();
                //    }
                //}

                //if ($tr.find('#UDF3') != null) {
                //    if ($tr.find('#UDF3').attr("class") == 'selectBox') {
                //        vUDF3 = $tr.find('#UDF3 option:selected').text();
                //    }
                //    else {
                //        vUDF3 = $tr.find('#UDF3').val();
                //    }
                //}

                //if ($tr.find('#UDF4') != null) {
                //    if ($tr.find('#UDF4').attr("class") == 'selectBox') {
                //        vUDF4 = $tr.find('#UDF4 option:selected').text();
                //    }
                //    else {
                //        vUDF4 = $tr.find('#UDF4').val();
                //    }
                //}

                //if ($tr.find('#UDF5') != null) {
                //    if ($tr.find('#UDF5').attr("class") == 'selectBox') {
                //        vUDF5 = $tr.find('#UDF5 option:selected').text();
                //    }
                //    else {
                //        vUDF5 = $tr.find('#UDF5').val();
                //    }
                //}

                if ($("#chkUsePullCommonUDF").is(":checked")) {
                    vUDF1 = vUDF1PullCommon;
                    vUDF2 = vUDF2PullCommon;
                    vUDF3 = vUDF3PullCommon;
                    vUDF4 = vUDF4PullCommon;
                    vUDF5 = vUDF5PullCommon;
                }


                var vPullSupplierAccountNumber = "";
                if ($tr.find('#hdnSupplierAccountNumber') != null) {
                    vPullSupplierAccountNumber = $tr.find('#hdnSupplierAccountNumber').val();
                }
                
                ///////////////////////////////////// UPDATE PULL DATA CALL//////////////////////////////////////////START
                $.ajax({
                    "url": _NewConsumePull.urls.UpdatePullDataUrl, //'@Url.Content("~/Pull/UpdatePullData")',
                    //"data": { ID: 0, ItemGUID: vItemGUID, ProjectGUID: vProjectID, BinID: vBinID, PullCreditQuantity: txtQty, PullCredit: vPullCreditText, TempPullQTY: txtQty, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5, RequisitionDetailID: "", WorkOrderDetailGUID: tmpdWorkOrderGUID, ProjectSpendName: ProjectSpendName, PullOrderNumber: vPullOrderNumber, callFrom: 'singlepull' },
                    "data": { ID: 0, ItemGUID: vItemGUID, ProjectGUID: vProjectID, BinID: vBinID, PullCreditQuantity: txtQty, PullCredit: vPullCreditText, TempPullQTY: txtQty, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5, RequisitionDetailID: "", WorkOrderDetailGUID: tmpdWorkOrderGUID, ProjectSpendName: ProjectSpendName, PullOrderNumber: vPullOrderNumber, SupplierAccountNumberGuid: vPullSupplierAccountNumber, callFrom: 'singlepull' },
                    "async": false,
                    //  cache: false,
                    "dataType": "json",
                    //contentType: 'application/json; charset=utf-8',
                    success: function (response) {
                        
                        //$('#DivLoading').hide();
                        //$.modal.impl.close();
                        ////enablePullbutton();
                        ShowHideNewPullProgressBar(false);
                        if (response.Status == "ok") {
                            if (response.LocationMSG != "") {
                                if (response.PSLimitExceed) {
                                    // write code to redirect to requisition with confirm box
                                    $("#PSPlimit").text(response.LocationMSG + " " + msgProjectSpendLimitConfirmation);
                                    $('#project-spend-limit-basic-modal-content').modal();
                                }
                                else {
                                    alert(response.LocationMSG);
                                }
                            }
                            else {
                                //alert('single test');
                                // if (response.SupPOParitList != "")
                                {
                                    BindNewPONumber(response.SupPOParitList);
                                }

                                UDFInsertNewForGrid($(obj).parent().parent());
                                $('div#target').fadeToggle();
                                $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                                $("#spanGlobalMessage").html(response.Message);
                                $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');

                                //oTable.fnDraw();
                                // TOTOTO
                                //oTableItemModel.fnDraw();
                                $('#ItemModeDataTable').dataTable().fnStandingRedraw();
                                RefreshPullNarrowSearch(); // Maintain narrow search value - WI-329
                                NSForItemModel_ExecuteOnDocReady();
                            }
                        }
                        else {
                            $("#spanGlobalMessage").html(response.Message);
                            $('div#target').fadeToggle();
                            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                        }
                        txxt.val(VspnDefaultPullQuantity);
                        //$(obj).removeAttr("disabled");
                    },
                    error: function (response) {
                        ////   ShowHideNewPullProgressBar(false);
                        ////enablePullbutton();
                        $('#DivLoading').hide();
                        $("#spanGlobalMessage").html(response.message);
                        $('div#target').fadeToggle();
                        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                        $(obj).removeAttr("disabled");
                    },
                    complete: function () {
                        //ShowHideNewPullProgressBar(false);
                        $(obj).removeAttr("disabled");
                        //$('#DivLoading').hide();
                    }
                });
                ///////////////////////////////////// UPDATE PULL DATA CALL//////////////////////////////////////////END
            }
        }, 100);

    }
    else if ($obj[0].value == 'Credit') {
        $('#DivLoading').show();
        var itemData = _NewConsumePull.getItemDataFromRow($tr);
        var txtQty = $tr.find('#txtQty').val();
        var vBinName = $tr.find('#txtBinNumber').val();

        var vitemGUID = itemData.spnItemGUID; //$td.find('#spnItemGUID').text();
        var itemType = itemData.spnOrderItemType; //$td.find('#spnOrderItemType').text();
        var vProjectID = "";

        if ($("#chkUsePullCommonUDF").is(":checked")) {
            if ($('#ProjectIDCommon') != undefined) {
                vProjectID = $('#ProjectIDCommon').val() == "" ? "" : $('#ProjectIDCommon').val();
            }
            else {
                vProjectID = "";
            }

            if ($('#txtProjectSpentCommon').val() != '') {
                ProjectSpendName = $('#txtProjectSpentCommon').val();
            }
        }
        else {
            //var eProjectID = _NewConsumePull.getDataEleFromRow($tr, 'ProjectID');

            ////if ($tr.find('#ProjectID')[0] != undefined) {
            //if (typeof eProjectID !== 'undefined' && eProjectID !== null) {
            //    //vProjectID = $tr.find('#ProjectID')[0].value == "" ? "" : $(obj).parent().parent().find('#ProjectID')[0].value;
            //    vProjectID = eProjectID.val();
            //}
            //else {
            //    vProjectID = "";
            //}

            vProjectID = _NewConsumePull.GetProjectId($tr);

            if ($tr.find('#txtProjectSpent').val() != '') {
                ProjectSpendName = $tr.find('#txtProjectSpent').val();
            }
        }

        if (isProjectSpendMandatoryInRoom == "True" && (vProjectID == undefined || vProjectID == "")) {
            if (tmpdWorkOrderGUID != null && tmpdWorkOrderGUID.length > 0) {
                alert(MsgCreditBackProjectSpendValidation);
                $('#DivLoading').hide();
            }
            else {
                $("#spanGlobalMessage").html(MsgCreditBackProjectSpendValidation);
                $('div#target').fadeToggle();
                $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                ////enablePullbutton();
            }
            $('#DivLoading').hide();
            $(obj).removeAttr("disabled");
            return false;
        }
        else {
            var vIsConsignment = $td.find('#spnIsConsignment').text();

            var vConsignedQuantity = 0;
            var vCustomerOwnedQuantity = 0;
            if (vIsConsignment.toLowerCase() == "true") {
                vConsignedQuantity = txtQty;
            }
            else {
                vCustomerOwnedQuantity = txtQty;
            }

            var arrItems = new Array();
            var data = { "ItemGUID": vitemGUID, "IsCreditPull": true, "ProjectSpentGUID": vProjectID, "IsOnlyFromUI": true, "ConsignedQuantity": vConsignedQuantity, "CustomerOwnedQuantity": vCustomerOwnedQuantity, "BinNumber": vBinName };
            arrItems.push(data);

            $.ajax({
                "url": _NewConsumePull.urls.ItemLocationDetailsSaveForCreditPullUrl, //'@Url.Content("~/Inventory/ItemLocationDetailsSaveForCreditPull")',
                data: JSON.stringify({ 'objData': arrItems, 'PullCreditType': 'credit' }),
                type: "Post",
                "async": false,
                cache: false,
                "dataType": "json",
                contentType: 'application/json; charset=utf-8',
                success: function (response) {
                    $('#DivLoading').hide();
                    ////enablePullbutton();
                    if (response.Status == 'OK') {
                        //UDFInsertNewForGrid($(obj).parent().parent());
                        $('div#target').fadeToggle();
                        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                        $("#spanGlobalMessage").html(response.Message);
                        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');

                        $('#ItemModeDataTable').dataTable().fnStandingRedraw();
                    }
                    else {
                        $("#spanGlobalMessage").html(response.Message);
                        $('div#target').fadeToggle();
                        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                        $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    }
                },
                error: function (response) {
                    ////enablePullbutton();
                    $('#DivLoading').hide();
                    $("#spanGlobalMessage").html(response.message);
                    $('div#target').fadeToggle();
                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                }
            });

            $('#DivLoading').hide();
            return false;
        }
    }
    else if ($obj[0].value == 'Credit MS') {
        $('#DivLoading').show();
        var itemData = _NewConsumePull.getItemDataFromRow($tr);
        var itemId = itemData.spnItemGUID; //$td.find('#spnItemGUID').text();
        var itemType = itemData.spnOrderItemType; //$td.find('#spnOrderItemType').text();
        var vProjectID = "";

        if ($("#chkUsePullCommonUDF").is(":checked")) {
            if ($('#ProjectIDCommon') != undefined) {
                vProjectID = $('#ProjectIDCommon').val() == "" ? "" : $('#ProjectIDCommon').val();
            }
            else {
                vProjectID = "";
            }
        }
        else {
            //if ($tr.find('#ProjectID')[0] != undefined) {
            //    vProjectID = $tr.find('#ProjectID')[0].value == "" ? "" : $tr.find('#ProjectID')[0].value;
            //}
            //else {
            //    vProjectID = "";
            //}
            vProjectID = _NewConsumePull.GetProjectId($tr);
        }

        //var vProjectID = $(obj).parent().parent().find('#ProjectID')[0].value == "" ? "" : $(obj).parent().parent().find('#ProjectID')[0].value;

        if (isProjectSpendMandatoryInRoom == "True" && (vProjectID == undefined || vProjectID == "")) {
            if (tmpdWorkOrderGUID != null && tmpdWorkOrderGUID.length > 0) {
                alert(MsgMSCreditBackProjectSpendValidation);
                $('#DivLoading').hide();
            }
            else {
                $("#spanGlobalMessage").html(MsgMSCreditBackProjectSpendValidation);
                $('div#target').fadeToggle();
                $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                ////enablePullbutton();
            }
            $('#DivLoading').hide();
            return false;
        }
        else {
            var idtype = itemId + '%23' + itemType + '%23' + 'WorkOrderGuid-' + tmpdWorkOrderGUID; //+ '%23' +  'frompullcreditMS' ;
            $('#DivLoading').show();
            $('#LocationDetails').load('../Inventory/LocationDetailsMS?ItemID_ItemType=' + idtype, function () {
                $('#LocationDetails').dialog('open');
                $('#DivLoading').hide();
                IsRefreshGrid = true;
            });
            $('#DivLoading').hide();
            return false;
        }
    }
    else {
        $('#DivLoading').hide();
        return false;
    }
    // $('#DivLoading').hide();
}

function OpenCreditPopup(obj) {
    ////diabledPullbutton();
    if ($(obj)[0].value == undefined)
        return false;
    $('#DivLoading').show();

    if ($(obj)[0].value == 'Credit') {
        $('#DivLoading').show();
        var itemData = _NewConsumePull.getItemDataFromRow($(obj).parent());
        var itemId = itemData.spnItemGUID; //$(obj).parent().find('#spnItemGUID').text();
        var itemType = itemData.spnOrderItemType; //$(obj).parent().find('#spnOrderItemType').text();
        var txtQty = $(obj).parent().parent().find('#txtQty').val();

        var vProjectID = "";

        if ($("#chkUsePullCommonUDF").is(":checked")) {
            if ($('#ProjectIDCommon') != undefined)
                vProjectID = $('#ProjectIDCommon').val() == "" ? "" : $('#ProjectIDCommon').val();
            else
                vProjectID = "";
        }
        else {
            //if ($(obj).parent().parent().find('#ProjectID')[0] != undefined) {
            //    vProjectID = $(obj).parent().parent().find('#ProjectID')[0].value == "" ? "" : $(obj).parent().parent().find('#ProjectID')[0].value;
            //}
            //else {
            //    vProjectID = "";
            //}
            vProjectID = _NewConsumePull.GetProjectId($(obj).parent().parent());
        }

        if (isProjectSpendMandatoryInRoom == "True" && (vProjectID == undefined || vProjectID == "")) {
            if (tmpdWorkOrderGUID != null && tmpdWorkOrderGUID.length > 0) {
                alert(MsgCreditBackProjectSpendValidation);
                $('#DivLoading').hide();
            }
            else {
                $("#spanGlobalMessage").html(MsgCreditBackProjectSpendValidation);
                $('div#target').fadeToggle();
                $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                ////enablePullbutton();
            }
            $('#DivLoading').hide();
            return false;
        }
        else {
            var idtype = itemId + '%23' + itemType + '%23' + 'frompullcredit' + '%23' + tmpdWorkOrderGUID;
            $('#DivLoading').show();
            $('#LocationDetails').load('../Inventory/LocationDetailsNew?ItemID_ItemType=' + idtype + '&ProjectSpendGUID=' + vProjectID + '&QtyToCredit=' + txtQty + '', function () {
                $('#LocationDetails').dialog('open');
                $('#DivLoading').hide();
                IsRefreshGrid = true;
            });
            $('#DivLoading').hide();
            return false;
        }
    }
}

function GoToRequisition() {
    $.modal.impl.close();
    var url = _NewConsumePull.urls.RequisitionListUrl; //'@Url.Action("RequisitionList", "Consume")';
    url = url + '?fromPull=' + 'yes'
    window.location.href = url;
}

$('#actionSelectAll3').live('click', function () {
    var Alllen = 0;
    var selLen = 0;
    $('#ItemModeDataTable tbody tr').each(function (i) {
        //if ($(this).find('#BinID').length > 0) {
        var ele = _NewConsumePull.getDataEleFromRow($(this), 'BinID');
        if (ele !== null && ele.length > 0) {
            Alllen = parseInt(Alllen) + 1;
        }
    });
    $('#ItemModeDataTable tbody tr.row_selected').each(function (i) {
        //if ($(this).find('#BinID').length > 0) {
        var ele = _NewConsumePull.getDataEleFromRow($(this), 'BinID');
        if (ele !== null && ele.length > 0) {
            selLen = parseInt(selLen) + 1;
        }
    });

    if (selLen < Alllen) {
        $('#ItemModeDataTable tbody tr').each(function (i) {
            //if ($(this).find('#BinID').length > 0) {
            var ele = _NewConsumePull.getDataEleFromRow($(this), 'BinID');
            if (ele !== null && ele.length > 0) {
                $(this).removeClass("row_selected").addClass("row_selected");
            }
        });
    }
    else {
        $('#ItemModeDataTable').find("tbody tr").removeClass("row_selected");
    }
});
function callPullNew() {
    window.location.href = "../pull/newpull";
}

//////////////////////////////////// // new added for QL///////////////////////////////////START

function AddQuickListToSelectedModuleForPull(obj) { // new added for QL
    ////diabledPullbutton();
    // $('#DivLoading').show();
    SaveQuickListToSelectedModuleForPull(obj);
    return;
}

function SaveQuickListToSelectedModuleForPull(btn) { // new added for QL
    $('#DivLoading').show();
    var IsGlobalErrorMsgShow = false,
        errGBLMsg = '',
        arrItems = new Array(),
        qty = 0,
        tr = $(btn).parent().parent(),
        $tr = $(tr);

    var vUDF1 = ''; var vUDF2 = ''; var vUDF3 = ''; var vUDF4 = ''; var vUDF5 = '';
    var vUDF1PullCommon = ''; var vUDF2PullCommon = ''; var vUDF3PullCommon = ''; var vUDF4PullCommon = ''; var vUDF5PullCommon = '';
    var vPullOrderNumber = "";
    var vQuickListGUID = '';

    vQuickListGUID = _NewConsumePull.getItemDataFromRow($tr).spnQuickListGUID; //$tr.find('#spnQuickListGUID').text();
    var txtQty = $tr.find('#txtQty');
    if (txtQty != undefined || txtQty != null) {
        qty = parseFloat($(txtQty).val());
    }

    if ($("#chkUsePullCommonUDF").is(":checked")) {
        if ($('#txtPullOrderNumberCommon') != null) {
            if ($('#txtPullOrderNumberCommon').attr("class") == 'selectBox') {
                vPullOrderNumber = $('#txtPullOrderNumberCommon option:selected').text();
            }
            else {
                vPullOrderNumber = $('#txtPullOrderNumberCommon').val();
            }
        }
    }
    else {
        if ($tr.find('#txtPullOrderNumber') != null) {
            if ($tr.find('#txtPullOrderNumber').attr("class") == 'selectBox') {
                vPullOrderNumber = $tr.find('#txtPullOrderNumber option:selected').text();
            }
            else {
                vPullOrderNumber = $tr.find('#txtPullOrderNumber').val();
            }
        }
    }

    vUDF1PullCommon = _NewConsumePull.GetUdfVal($("#UDF1PullCommon"));
    vUDF2PullCommon = _NewConsumePull.GetUdfVal($("#UDF2PullCommon"));
    vUDF3PullCommon = _NewConsumePull.GetUdfVal($("#UDF3PullCommon"));
    vUDF4PullCommon = _NewConsumePull.GetUdfVal($("#UDF4PullCommon"));
    vUDF5PullCommon = _NewConsumePull.GetUdfVal($("#UDF5PullCommon"));

    vUDF1 = _NewConsumePull.GetUdfVal($tr.find('#UDF1'));
    vUDF2 = _NewConsumePull.GetUdfVal($tr.find('#UDF2'));
    vUDF3 = _NewConsumePull.GetUdfVal($tr.find('#UDF3'));
    vUDF4 = _NewConsumePull.GetUdfVal($tr.find('#UDF4'));
    vUDF5 = _NewConsumePull.GetUdfVal($tr.find('#UDF5'));



    //if ($("#UDF1PullCommon") != null) {
    //    if ($("#UDF1PullCommon").attr("class") == 'selectBox') {
    //        vUDF1PullCommon = $("#UDF1PullCommon option:selected").text();
    //    }
    //    else {
    //        vUDF1PullCommon = $("#UDF1PullCommon").val();
    //    }
    //}

    //if ($("#UDF2PullCommon") != null) {
    //    if ($("#UDF2PullCommon").attr("class") == 'selectBox') {
    //        vUDF2PullCommon = $("#UDF2PullCommon option:selected").text();
    //    }
    //    else {
    //        vUDF2PullCommon = $("#UDF2PullCommon").val();
    //    }
    //}

    //if ($("#UDF3PullCommon") != null) {
    //    if ($("#UDF3PullCommon").attr("class") == 'selectBox') {
    //        vUDF3PullCommon = $("#UDF3PullCommon option:selected").text();
    //    }
    //    else {
    //        vUDF3PullCommon = $("#UDF3PullCommon").val();
    //    }
    //}

    //if ($("#UDF4PullCommon") != null) {
    //    if ($("#UDF4PullCommon").attr("class") == 'selectBox') {
    //        vUDF4PullCommon = $("#UDF4PullCommon option:selected").text();
    //    }
    //    else {
    //        vUDF4PullCommon = $("#UDF4PullCommon").val();
    //    }
    //}

    //if ($("#UDF5PullCommon") != null) {
    //    if ($("#UDF5PullCommon").attr("class") == 'selectBox') {
    //        vUDF5PullCommon = $("#UDF5PullCommon option:selected").text();
    //    }
    //    else {
    //        vUDF5PullCommon = $("#UDF5PullCommon").val();
    //    }
    //}

    //if ($(tr).find('#UDF1') != null) {
    //    if ($(tr).find('#UDF1').attr("class") == 'selectBox') {
    //        vUDF1 = $(tr).find('#UDF1 option:selected').text();
    //    }
    //    else {
    //        vUDF1 = $(tr).find('#UDF1').val();
    //    }
    //}

    //if ($(tr).find('#UDF2') != null) {
    //    if ($(tr).find('#UDF2').attr("class") == 'selectBox') {
    //        vUDF2 = $(tr).find('#UDF2 option:selected').text();
    //    }
    //    else {
    //        vUDF2 = $(tr).find('#UDF2').val();
    //    }
    //}

    //if ($(tr).find('#UDF3') != null) {
    //    if ($(tr).find('#UDF3').attr("class") == 'selectBox') {
    //        vUDF3 = $(tr).find('#UDF3 option:selected').text();
    //    }
    //    else {
    //        vUDF3 = $(tr).find('#UDF3').val();
    //    }
    //}

    //if ($(tr).find('#UDF4') != null) {
    //    if ($(tr).find('#UDF4').attr("class") == 'selectBox') {
    //        vUDF4 = $(tr).find('#UDF4 option:selected').text();
    //    }
    //    else {
    //        vUDF4 = $(tr).find('#UDF4').val();
    //    }
    //}

    //if ($(tr).find('#UDF5') != null) {
    //    if ($(tr).find('#UDF5').attr("class") == 'selectBox') {
    //        vUDF5 = $(tr).find('#UDF5 option:selected').text();
    //    }
    //    else {
    //        vUDF5 = $(tr).find('#UDF5').val();
    //    }
    //}

    if ($("#chkUsePullCommonUDF").is(":checked")) {
        vUDF1 = vUDF1PullCommon;
        vUDF2 = vUDF2PullCommon;
        vUDF3 = vUDF3PullCommon;
        vUDF4 = vUDF4PullCommon;
        vUDF5 = vUDF5PullCommon;
    }
    $('#GlobalModalProcessing').modal({
        escClose: false,
        close: false
    });
    $.ajax({
        url: _NewConsumePull.urls.GetItemByQLGuidUrl, //'@Url.Content("~/Pull/GetItemByQLGuid")',
        data: { 'QuickListGUID': vQuickListGUID, 'PullQuantity': qty },
        dataType: 'json',
        async: false,
        cache: false,
        success: function (data) {
            var PullInfo = new Array();
            var isSerialLot = false;
            for (var i = 0; i < data.length; i++) {
                //                    $('#DivLoading').show();
                var errorMsg = '';
                var vBinID;
                var vProjectID;

                var itemType = data[i].ItemType;

                if (itemType != '4') {
                    vBinID = data[i].BinID == '' ? 0 : data[i].BinID;

                    if ($("#chkUsePullCommonUDF").is(":checked")) {
                        if ($('#ProjectIDCommon') != undefined) {
                            vProjectID = $('#ProjectIDCommon').val() == "" ? "" : $('#ProjectIDCommon').val();
                        }
                        else {
                            vProjectID = "";
                        }
                        if ($('#txtProjectSpentCommon').val() != '') {
                            ProjectSpendName = $('#txtProjectSpentCommon').val();
                        }
                    }
                    else {
                        //if ($tr.find('#ProjectID')[0] != undefined) {
                        //    vProjectID = $tr.find('#ProjectID').value == "" ? "" : $(tr).find('#ProjectID').value;
                        //}
                        //else {
                        //    vProjectID = "";
                        //}
                        vProjectID = _NewConsumePull.GetProjectId($tr);
                        if ($tr.find('#txtProjectSpent').val() != '') {
                            ProjectSpendName = $(tr).find('#txtProjectSpent').val();
                        }
                    }
                    if (!(!isNaN(parseFloat(qty)) && parseFloat(qty) > 0)) {
                        $tr.css('background-color', 'Olive');
                        IsGlobalErrorMsgShow = true;
                        errorMsg += "<b style='color:Olive;'>" + data[i].ItemNumber + ": " + MsgQtyToPullMandatory+"</b><br/>"
                    }

                    if (!(!isNaN(parseInt(vBinID)) && parseInt(vBinID) > 0)) {
                        $tr.css('background-color', 'Olive');
                        IsGlobalErrorMsgShow = true;
                        errorMsg += "<b style='color:Olive;'>" + data[i].ItemNumber + ": " + MsgInventoryLocationMandatory+"</b><br/>"
                    }
                }
                else {

                    if (!(!isNaN(parseFloat(qty)) && parseFloat(qty) > 0)) {
                        $tr.css('background-color', 'Olive');
                        IsGlobalErrorMsgShow = true;
                        errorMsg += "<b style='color:Olive;'>" + data[i].ItemNumber + ": " + MsgLabourItemRequiredHours+"</b><br/>"
                    }
                    vBinID = 0;
                    vProjectID = '';
                    ProjectSpendName = '';
                }

                if (errorMsg.length <= 0) {
                    if (data[i].SerialNumberTracking || data[i].LotNumberTracking || data[i].DateCodeTracking) {
                        isSerialLot = true;
                    }
                    var PullQty = 0;
                    var QLDefaultQuantity = (data[i].QuanityPulled != null ? data[i].QuanityPulled : 1);
                    PullQty = qty * QLDefaultQuantity;

                    PullInfo.push({ ID: 0, ItemID: data[i].ID, ItemNumber: data[i].ItemNumber, ItemType: data[i].ItemType, PoolQuantity: PullQty, ItemGUID: data[i].GUID, BinID: vBinID, DefaultPullQuantity: (data[i].DefaultPullQuantity == "" ? 0 : data[i].DefaultPullQuantity), ProjectSpendGUID: vProjectID, ProjectSpendName: ProjectSpendName, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5, WorkOrderDetailGUID: tmpdWorkOrderGUID, PullOrderNumber: vPullOrderNumber });
                }
            }
            if (isSerialLot == true) {
                $.modal.impl.close();
                if (errorMsg.length <= 0) {
                    $.ajax({
                        type: "POST",
                        url: PullItemQuantityUrl,
                        contentType: 'application/json',
                        dataType: 'html',
                        data: JSON.stringify(PullInfo),
                        success: function (RetData) {
                            $("#DivPullSelection").html("");
                            $("#DivPullSelection").html(RetData);
                            $("#DivPullSelection").dialog('open');
                            $('#DivLoading').hide();
                        },
                        error: function (response) {
                            $(tr).css('background-color', 'Red');
                            IsPSRedirectToReq = false;
                            IsGlobalErrorMsgShow = true;
                            errorMsg += "<b style='color:Red'>" + data[i].ItemNumber + " :  " + MsgErrorInProcess + "</b><br>"
                        }
                    });
                }
                return false;
            }
            else {
                for (var i = 0; i < PullInfo.length; i++) {
                    $('#DivLoading').show();
                    var errorMsg = '';
                    var vBinID;
                    var vProjectID;

                    var itemType = PullInfo[i].ItemType;

                    if (itemType != '4') {
                        vBinID = PullInfo[i].BinID == '' ? 0 : PullInfo[i].BinID;

                        if ($("#chkUsePullCommonUDF").is(":checked")) {
                            if ($('#ProjectIDCommon') != undefined) {
                                vProjectID = $('#ProjectIDCommon').val() == "" ? "" : $('#ProjectIDCommon').val();
                            }
                            else {
                                vProjectID = "";
                            }

                            if ($('#txtProjectSpentCommon').val() != '') {
                                ProjectSpendName = $('#txtProjectSpentCommon').val();
                            }
                        }
                        else {
                            //if ($(tr).find('#ProjectID')[0] != undefined) {
                            //    vProjectID = $(tr).find('#ProjectID').value == "" ? "" : $(tr).find('#ProjectID').value;
                            //}
                            //else {
                            //    vProjectID = "";
                            //}
                            vProjectID = _NewConsumePull.GetProjectId($(tr));

                            if ($(tr).find('#txtProjectSpent').val() != '') {
                                ProjectSpendName = $(tr).find('#txtProjectSpent').val();
                            }
                        }

                        if (isProjectSpendMandatoryInRoom == "True" && (vProjectID == undefined || typeof vProjectID == "undefined" || vProjectID == "") && (ProjectSpendName == undefined || ProjectSpendName == "")) {
                            $(tr).css('background-color', 'Olive');
                            IsGlobalErrorMsgShow = true;
                            errorMsg += "<b style='color:Olive;'>" + PullInfo[i].ItemNumber + ": " + MsgProjectSpendMandatory +"</b><br/>"
                        }

                        if (!(!isNaN(parseFloat(qty)) && parseFloat(qty) > 0)) {
                            $(tr).css('background-color', 'Olive');
                            IsGlobalErrorMsgShow = true;
                            errorMsg += "<b style='color:Olive;'>" + PullInfo[i].ItemNumber + ": " + MsgQtyToPullMandatory+"</b><br/>"
                        }

                        if (!(!isNaN(parseInt(vBinID)) && parseInt(vBinID) > 0)) {
                            $(tr).css('background-color', 'Olive');
                            IsGlobalErrorMsgShow = true;
                            errorMsg += "<b style='color:Olive;'>" + PullInfo[i].ItemNumber + ": " + MsgInventoryLocationMandatory+"</b><br/>"
                        }
                    }
                    else {

                        if (!(!isNaN(parseFloat(qty)) && parseFloat(qty) > 0)) {
                            $(tr).css('background-color', 'Olive');
                            IsGlobalErrorMsgShow = true;
                            errorMsg += "<b style='color:Olive;'>" + PullInfo[i].ItemNumber + ": " + MsgLabourItemRequiredHours+"</b><br/>"
                        }
                        vBinID = 0;
                        vProjectID = '';
                        ProjectSpendName = '';
                    }

                    if (errorMsg.length <= 0) {
                        //$('#GlobalModalProcessing').modal({
                        //    escClose: false,
                        //    close: false
                        //});

                        if (errorMsg.length <= 0) {
                            var vItemGUID = PullInfo[i].ItemGUID;

                            //var vspnOn_HandQuantity = data[i].OnHandQuantity == "" ? 0 : data[i].OnHandQuantity;
                            var vPullCreditText = "pull";
                            var VspnDefaultPullQuantity = PullInfo[i].DefaultPullQuantity == "" ? 0 : PullInfo[i].DefaultPullQuantity;

                            //$('#DivLoading').show();
                            ///////////////////////////////////// UPDATE PULL DATA CALL//////////////////////////////////////////START
                            $.ajax({
                                "url": _NewConsumePull.urls.UpdatePullDataUrl, //'@Url.Content("~/Pull/UpdatePullData")',
                                data: { ID: 0, ItemGUID: vItemGUID, ProjectGUID: vProjectID, BinID: vBinID, PullCreditQuantity: PullInfo[i].PoolQuantity, PullCredit: vPullCreditText, TempPullQTY: PullInfo[i].PoolQuantity, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4, UDF5: vUDF5, RequisitionDetailID: "", WorkOrderDetailGUID: tmpdWorkOrderGUID, ProjectSpendName: ProjectSpendName, PullOrderNumber: vPullOrderNumber },
                                "async": false,
                                cache: false,
                                "dataType": "json",
                                success: function (response) {
                                    
                                    $('#DivLoading').hide();
                                    if (response.Status == "ok") {
                                        if (response.LocationMSG != "") {
                                            if (response.PSLimitExceed) {
                                                $(tr).css('background-color', 'Yellow');
                                                IsPSRedirectToReq = true;
                                                IsGlobalErrorMsgShow = true;
                                                errorMsg += "<b style='color:Olive'>" + PullInfo[i].ItemNumber + " : " + response.LocationMSG + " " + msgProjectSpendLimitConfirmation+"</b><br>"
                                            }
                                            else {
                                                $(tr).css('background-color', 'Olive');
                                                IsGlobalErrorMsgShow = true;
                                                errorMsg += "<b style='color:Olive'>" + PullInfo[i].ItemNumber + " : " + response.LocationMSG + "</b><br>"
                                            }
                                        }
                                        else {
                                            //if (response.SupPOParitList != "")
                                            {
                                                BindNewPONumber(response.SupPOParitList);
                                            }

                                            UDFInsertNewForGrid(tr);
                                            $(tr).css('background-color', 'Green');
                                            errorMsg += "<b style='color:Green'>" + PullInfo[i].ItemNumber + " : " + response.Message + "</b><br>"
                                        }
                                    }
                                    else {
                                        $(tr).css('background-color', 'Red');
                                        IsPSRedirectToReq = false;
                                        IsGlobalErrorMsgShow = true;
                                        errorMsg += "<b style='color:Red'>" + PullInfo[i].ItemNumber + " : " + response.Message + "</b><br>"
                                    }
                                    txtQty.val('');
                                },
                                error: function (response) {
                                    $(tr).css('background-color', 'Red');
                                    IsPSRedirectToReq = false;
                                    IsGlobalErrorMsgShow = true;
                                    errorMsg += "<b style='color:Red'>" + PullInfo[i].ItemNumber + " :  " + MsgErrorInProcess + "</b><br>"
                                }
                            });
                            ///////////////////////////////////// UPDATE PULL DATA CALL//////////////////////////////////////////END
                        }
                    }
                    errGBLMsg += errorMsg;
                }
            }
            $.modal.impl.close();
            $('#DivLoading').hide();

            if (IsGlobalErrorMsgShow) {
                $.modal.impl.close();
                $('#DivLoading').hide();
                errGBLMsg = "<b>" + MsgQuickListPullReason+"</b><br /><br />" + errGBLMsg;
                $('#dlgCommonErrorMsg').find("#pOkbtn").css('display', '');
                $('#dlgCommonErrorMsg').find("#pYesNobtn").css('display', 'none');
                $('#dlgCommonErrorMsg').find("#pErrMessage").html(errGBLMsg);

                $('#dlgCommonErrorMsg').modal()
            }
        },
        error: function (xhr) {
            $('#DivLoading').hide();
            $(btn).removeAttr("disabled");
        }
    });
}


var newConsumePullDTO = function (serverObj) {
    var self = this;
    var tmpZeroCostQtyFormatted2 = FormatedCostQtyValues(0, 2);
    var tmpZeroCostQtyFormatted1 = FormatedCostQtyValues(0, 1);
    var tmpZeroCostQtyFormatted4 = FormatedCostQtyValues(0, 4);
    self.Action = serverObj.Action;
    self.AddedFrom = serverObj.AddedFrom;
    self.AppendedBarcodeString = serverObj.AppendedBarcodeString;
    self.AverageCost = serverObj.AverageCost;
    self.AverageUsage = serverObj.AverageUsage;
    self.BPONumber = serverObj.BPONumber;
    self.BinAutoComplete = serverObj.BinAutoComplete;
    self.BinDefaultPullQuantity = serverObj.BinDefaultPullQuantity;
    self.BinDefaultReorderQuantity = serverObj.BinDefaultReorderQuantity;
    self.BinID = serverObj.BinID;
    self.BinNumber = serverObj.BinNumber;
    self.BlanketOrderNumber = serverObj.BlanketOrderNumber;
    self.BondedInventory = serverObj.BondedInventory;
    self.CategoryColor = serverObj.CategoryColor;
    self.CategoryID = serverObj.CategoryID;
    self.CategoryName = serverObj.CategoryName;
    self.CompanyID = serverObj.CompanyID;
    self.ConsignedQuantity = serverObj.ConsignedQuantity;
    self.Consignment = serverObj.Consignment;
    self.Cost = serverObj.Cost;
    self.CostUOMID = serverObj.CostUOMID;
    self.CostUOMName = serverObj.CostUOMName;
    self.CostUOMValue = serverObj.CostUOMValue;
    self.CountConsignedQuantity = serverObj.CountConsignedQuantity;
    self.CountCustomerOwnedQuantity = serverObj.CountCustomerOwnedQuantity;
    self.CountLineItemDescriptionEntry = serverObj.CountLineItemDescriptionEntry;
    self.CountedDate = serverObj.CountedDate;
    self.CountedDateStr = serverObj.CountedDateStr;
    self.Created = serverObj.Created;
    self.CreatedBy = serverObj.CreatedBy;
    self.CreatedByName = serverObj.CreatedByName;
    self.CreatedDate = serverObj.CreatedDate;
    self.CriticalQuantity = serverObj.CriticalQuantity;
    self.CustomerOwnedQuantity = serverObj.CustomerOwnedQuantity;
    self.DateCodeTracking = serverObj.DateCodeTracking;
    self.DefaultLocation = serverObj.DefaultLocation;
    self.DefaultLocationGUID = serverObj.DefaultLocationGUID;
    self.DefaultLocationName = serverObj.DefaultLocationName;
    self.DefaultProjectSpend = serverObj.DefaultProjectSpend;
    self.DefaultProjectSpendGuid = serverObj.DefaultProjectSpendGuid;
    self.DefaultPullQuantity = serverObj.DefaultPullQuantity;
    self.DefaultReorderQuantity = serverObj.DefaultReorderQuantity;
    self.Description = serverObj.Description;
    self.EditedFrom = serverObj.EditedFrom;
    self.EnterpriseId = serverObj.EnterpriseId;
    self.ExtendedCost = serverObj.ExtendedCost;
    self.GLAccount = serverObj.GLAccount;
    self.GLAccountID = serverObj.GLAccountID;
    self.GUID = serverObj.GUID;
    self.HistoryDate = serverObj.HistoryDate;
    self.HistoryID = serverObj.HistoryID;
    self.HistoryOn = serverObj.HistoryOn;
    self.ID = serverObj.ID;
    self.ImagePath = serverObj.ImagePath;
    self.ImageType = serverObj.ImageType;
    self.InTransitquantity = serverObj.InTransitquantity;
    self.InventoryClassification = serverObj.InventoryClassification;
    self.InventoryClassificationName = serverObj.InventoryClassificationName;
    self.InventryLocation = serverObj.InventryLocation;
    self.IsActive = serverObj.IsActive;
    self.IsAllowOrderCostuom = serverObj.IsAllowOrderCostuom;
    self.IsArchived = serverObj.IsArchived;
    self.IsAutoInventoryClassification = serverObj.IsAutoInventoryClassification;
    self.IsBOMItem = serverObj.IsBOMItem;
    self.IsBuildBreak = serverObj.IsBuildBreak;
    self.IsDefaultProjectSpend = serverObj.IsDefaultProjectSpend;
    self.IsDeleted = serverObj.IsDeleted;
    self.IsEnforceDefaultReorderQuantity = serverObj.IsEnforceDefaultReorderQuantity;
    self.IsItemLevelMinMaxQtyRequired = serverObj.IsItemLevelMinMaxQtyRequired;
    self.IsLotSerialExpiryCost = serverObj.IsLotSerialExpiryCost;
    self.IsOnlyFromItemUI = serverObj.IsOnlyFromItemUI;
    self.IsPackslipMandatoryAtReceive = serverObj.IsPackslipMandatoryAtReceive;
    self.IsPurchase = serverObj.IsPurchase;
    self.IsTransfer = serverObj.IsTransfer;
    self.IsstagingLocation = serverObj.IsstagingLocation;
    self.ItemBlanketPO = serverObj.ItemBlanketPO;
    self.ItemDefaultPullQuantity = serverObj.ItemDefaultPullQuantity;
    self.ItemDefaultReorderQuantity = serverObj.ItemDefaultReorderQuantity;
    self.ItemDocExternalURL = serverObj.ItemDocExternalURL;
    self.ItemImageExternalURL = serverObj.ItemImageExternalURL;
    self.ItemIsActiveDate = serverObj.ItemIsActiveDate;
    self.ItemLink2ExternalURL = serverObj.ItemLink2ExternalURL;
    self.ItemLink2ImageType = serverObj.ItemLink2ImageType;
    self.ItemLocations = serverObj.ItemLocations;
    self.ItemNumber = serverObj.ItemNumber;
    self.ItemTraking = serverObj.ItemTraking;
    self.ItemType = serverObj.ItemType;
    self.ItemTypeName = serverObj.ItemTypeName;
    self.ItemUDF1 = serverObj.ItemUDF1;
    self.ItemUDF2 = serverObj.ItemUDF2;
    self.ItemUDF3 = serverObj.ItemUDF3;
    self.ItemUDF4 = serverObj.ItemUDF4;
    self.ItemUDF5 = serverObj.ItemUDF5;
    self.ItemUDF6 = serverObj.ItemUDF6;
    self.ItemUDF7 = serverObj.ItemUDF7;
    self.ItemUDF8 = serverObj.ItemUDF8;
    self.ItemUDF9 = serverObj.ItemUDF9;
    self.ItemUDF10 = serverObj.ItemUDF10;
    self.ItemUniqueNumber = serverObj.ItemUniqueNumber;
    self.ItemsLocations = serverObj.ItemsLocations;
    self.LastCost = serverObj.LastCost;
    self.LastUpdatedBy = serverObj.LastUpdatedBy;
    self.LeadTimeInDays = serverObj.LeadTimeInDays;
    self.Link1 = serverObj.Link1;
    self.Link2 = serverObj.Link2;
    self.LongDescription = serverObj.LongDescription;
    self.LotNumberTracking = serverObj.LotNumberTracking;
    self.ManufacturerID = serverObj.ManufacturerID;
    self.ManufacturerName = serverObj.ManufacturerName;
    self.ManufacturerNumber = serverObj.ManufacturerNumber;
    self.Markup = serverObj.Markup;
    self.MaximumQuantity = serverObj.MaximumQuantity;
    self.MinimumQuantity = serverObj.MinimumQuantity;
    self.MonthValue = serverObj.MonthValue;
    self.MonthlyAverageUsage = serverObj.MonthlyAverageUsage;
    self.OnHandQuantity = serverObj.OnHandQuantity;
    self.OnOrderInTransitQuantity = serverObj.OnOrderInTransitQuantity;
    self.OnOrderQuantity = serverObj.OnOrderQuantity;
    self.OnReturnQuantity = serverObj.OnReturnQuantity;
    self.OnTransferInTransitQuantity = serverObj.OnTransferInTransitQuantity;
    self.OnTransferQuantity = serverObj.OnTransferQuantity;
    self.OrderUOMID = serverObj.OrderUOMID;
    self.OrderUOMName = serverObj.OrderUOMName;
    self.OrderUOMValue = serverObj.OrderUOMValue;
    self.OrderedDate = serverObj.OrderedDate;
    self.OrderedDateStr = serverObj.OrderedDateStr;
    self.OutTransferQuantity = serverObj.OutTransferQuantity;
    self.PackingQuantity = serverObj.PackingQuantity;
    self.ParentBinId = serverObj.ParentBinId;
    self.ParentBinName = serverObj.ParentBinName;
    self.PerItemCost = serverObj.PerItemCost;
    self.PricePerTerm = serverObj.PricePerTerm;
    self.PriceSavedDate = serverObj.PriceSavedDate;
    self.PriceSavedDateStr = serverObj.PriceSavedDateStr;
    self.PullQtyScanOverride = serverObj.PullQtyScanOverride;
    self.PulledDate = serverObj.PulledDate;
    self.PulledDateStr = serverObj.PulledDateStr;
    self.QLCreditQuantity = serverObj.QLCreditQuantity;
    self.QtyToMeetDemand = serverObj.QtyToMeetDemand;
    self.QuanityPulled = serverObj.QuanityPulled;
    self.QuickListGUID = serverObj.QuickListGUID;
    self.QuickListItemQTY = serverObj.QuickListItemQTY;
    self.QuickListName = serverObj.QuickListName;
    self.QuickListType = serverObj.QuickListType;
    self.Reason = serverObj.Reason;
    self.ReceivedOn = serverObj.ReceivedOn;
    self.ReceivedOnDate = serverObj.ReceivedOnDate;
    self.ReceivedOnDateWeb = serverObj.ReceivedOnDateWeb;
    self.ReceivedOnWeb = serverObj.ReceivedOnWeb;
    self.RefBomI = serverObj.RefBomI;
    self.RefBomId = serverObj.RefBomId;
    self.RequisitionedQuantity = serverObj.RequisitionedQuantity;
    self.Room = serverObj.Room;
    self.RoomName = serverObj.RoomName;
    self.RownumberCost = serverObj.RownumberCost;
    self.SellPrice = serverObj.SellPrice;
    self.SerialNumberTracking = serverObj.SerialNumberTracking;
    self.StagedQuantity = serverObj.StagedQuantity;
    self.Status = serverObj.Status;
    self.StockOutCount = serverObj.StockOutCount;
    self.SuggestedOrderQuantity = serverObj.SuggestedOrderQuantity;
    self.SuggestedReturnQuantity = serverObj.SuggestedReturnQuantity;
    self.SuggestedTransferQuantity = serverObj.SuggestedTransferQuantity;
    self.SupplierID = serverObj.SupplierID;
    self.SupplierName = serverObj.SupplierName;
    self.SupplierPartNo = serverObj.SupplierPartNo;
    self.Taxable = serverObj.Taxable;
    self.TotalRecords = serverObj.TotalRecords;
    self.TrasnferedDate = serverObj.TrasnferedDate;
    self.TrasnferedDateStr = serverObj.TrasnferedDateStr;
    self.Trend = serverObj.Trend;
    self.TrendingSetting = serverObj.TrendingSetting;
    self.Turns = serverObj.Turns;
    self.UDF1 = serverObj.UDF1;
    self.UDF2 = serverObj.UDF2;
    self.UDF3 = serverObj.UDF3;
    self.UDF4 = serverObj.UDF4;
    self.UDF5 = serverObj.UDF5;
    self.UDF6 = serverObj.UDF6;
    self.UDF7 = serverObj.UDF7;
    self.UDF8 = serverObj.UDF8;
    self.UDF9 = serverObj.UDF9;
    self.UDF10 = serverObj.UDF10;
    self.UNSPSC = serverObj.UNSPSC;
    self.UOMID = serverObj.UOMID;
    self.UPC = serverObj.UPC;
    self.Unit = serverObj.Unit;
    self.Updated = serverObj.Updated;
    self.UpdatedByName = serverObj.UpdatedByName;
    self.UpdatedDate = serverObj.UpdatedDate;
    self.WeightPerPiece = serverObj.WeightPerPiece;
    self.WhatWhereAction = serverObj.WhatWhereAction;
    self.lstItemLocationQTY = serverObj.lstItemLocationQTY;
    self.lstItemLocations = serverObj.lstItemLocations;
    self.xmlItemLocations = serverObj.xmlItemLocations;
    var $NewPullAction = $('#NewPullAction');

    var fnDispImg = function () {

        var dataAttr = "";
        if (self.QuickListGUID == null || self.QuickListGUID == '') {

            dataAttr += " data-spnItemID='" + self.ID + "'";
            dataAttr += " data-spnItemGUID='" + self.GUID + "'";
            dataAttr += " data-spnOn_HandQuantity='" + self.OnHandQuantity + "'";
            dataAttr += " data-spnOrderItemType='" + self.ItemType + "'";
            dataAttr += " data-itemWisePONumber='" + self.ItemBlanketPO + "'";

            if ($NewPullAction.val() == "Pull") {
                if (viewNewPullbuttons == 'no') {

                    //return "<input type='button' value='Pull' onclick='return AddSingleItemToPullList(this);' id='btnAdd' class='CreateBtn pullbutton' style='float: none;padding: 2px 6px;margin-left: 5px;' />" + "<span id='spnItemID' style='display:none'>" + self.ID + "</span>" + "<span id='spnItemGUID' style='display:none'>" + self.GUID + "</span><span id='spnOn_HandQuantity' style='display:none'>" + self.OnHandQuantity + "</span>" + "<span id='spnOrderItemType'  style='display:none'>" + self.ItemType + "</span>" + "<input type='hidden' id='itemWisePONumber' value='" + self.ItemBlanketPO + "' />";
                    return "<input type='button' value='" + Pull + "' data-val='Pull' onclick='return AddSingleItemToPullList(this);' id='btnAdd' class='CreateBtn pullbutton' style='float: none;padding: 2px 6px;margin-left: 5px;' " + dataAttr + " />";
                }
                else {
                    if (self.SerialNumberTracking == true || self.LotNumberTracking == true || self.DateCodeTracking == true) {
                        //return "<input type='button' value='Pull' onclick='return OpenPullPopup(this);' id='btnAdd' class='CreateBtn pullbutton' style='float: none;padding: 2px 6px;margin-left: 5px;' />" + "<span id='spnItemID' style='display:none'>" + self.ID + "</span>" + "<span id='spnItemGUID' style='display:none'>" + self.GUID + "</span><span id='spnOn_HandQuantity' style='display:none'>" + self.OnHandQuantity + "</span>" + "<span id='spnOrderItemType'  style='display:none'>" + self.ItemType + "</span>" + "<input type='hidden' id='itemWisePONumber' value='" + self.ItemBlanketPO + "' />";
                        return "<input type='button' value='" + Pull + "' onclick='return OpenPullPopup(this);' id='btnAdd' class='CreateBtn pullbutton' style='float: none;padding: 2px 6px;margin-left: 5px;' " + dataAttr + " />";
                    }
                    else {
                        //return "<input type='button' value='Pull' onclick='return AddSingleItemToPullList(this);' id='btnAdd' class='CreateBtn pullbutton' style='float: none;padding: 2px 6px;margin-left: 5px;' />" + "<span id='spnItemID' style='display:none'>" + self.ID + "</span>" + "<span id='spnItemGUID' style='display:none'>" + self.GUID + "</span><span id='spnOn_HandQuantity' style='display:none'>" + self.OnHandQuantity + "</span>" + "<span id='spnOrderItemType'  style='display:none'>" + self.ItemType + "</span>" + "<input type='hidden' id='itemWisePONumber' value='" + self.ItemBlanketPO + "' />";
                        return "<input type='button' value='" + Pull + "' data-val='Pull' onclick='return AddSingleItemToPullList(this);' id='btnAdd' class='CreateBtn pullbutton' style='float: none;padding: 2px 6px;margin-left: 5px;' " + dataAttr + " />";
                    }
                }
            }
            else if ($NewPullAction.val() == "Credit") {
                //return "<input type='button' value='Credit' onclick='return CreditItems(this);' id='btnAdd' class='CreateBtn pullbutton' style='float: none;padding: 2px 6px;margin-left: 5px;' />" + "<span id='spnItemID' style='display:none'>" + self.ID + "</span>" + "<span id='spnItemGUID' style='display:none'>" + self.GUID + "</span><span id='spnOn_HandQuantity' style='display:none'>" + self.OnHandQuantity + "</span>" + "<span id='spnOrderItemType'  style='display:none'>" + self.ItemType + "</span>" + "<input type='hidden' id='itemWisePONumber' value='" + self.ItemBlanketPO + "' />" + "<span id='spnIsIgnoreCreditRule'  style='display:none'>" + serIsIgnoreCreditRule + "</span>" + "<span id='spnItemNumber' style='display:none'>" + self.ItemNumber + "</span>";                        
                dataAttr += " data-spnIsIgnoreCreditRule='" + serIsIgnoreCreditRule + "'";
                dataAttr += " data-spnItemNumber='" + self.ItemNumber + "'";
                return "<input type='button' value='" + Credit +"' onclick='return CreditItems(this);' id='btnAdd' class='CreateBtn pullbutton' style='float: none;padding: 2px 6px;margin-left: 5px;' " + dataAttr + " />";
            }
            else if ($NewPullAction.val() == "CreditMS") {
                //return "<input type='button' value='Credit MS'  onclick='return MsCreditItems(this);' id='btnAdd' class='CreateBtn pullbutton' style='float: none;padding: 2px 6px;margin-left: 5px;' />" + "<span id='spnItemID' style='display:none'>" + self.ID + "</span>" + "<span id='spnItemGUID' style='display:none'>" + self.GUID + "</span><span id='spnOn_HandQuantity' style='display:none'>" + self.OnHandQuantity + "</span>" + "<span id='spnOrderItemType'  style='display:none'>" + self.ItemType + "</span>" + "<input type='hidden' id='itemWisePONumber' value='" + self.ItemBlanketPO + "' />" + "<span id='spnIsIgnoreCreditRule'  style='display:none'>" + serIsIgnoreCreditRule + "</span>" + "<span id='spnItemNumber' style='display:none'>" + self.ItemNumber + "</span>";
                dataAttr += " data-spnIsIgnoreCreditRule='" + serIsIgnoreCreditRule + "'";
                dataAttr += " data-spnItemNumber='" + self.ItemNumber + "'";
                return "<input type='button' value='" + CreditMS +"'  onclick='return MsCreditItems(this);' id='btnAdd' class='CreateBtn pullbutton' style='float: none;padding: 2px 6px;margin-left: 5px;' " + dataAttr + " />";
            }
            else {
                //return "" + "<input type='hidden' id='itemWisePONumber' value='" + self.ItemBlanketPO + "' />";
                return "<input type='button' style='display:none' data-itemWisePONumber='" + self.ItemBlanketPO + "' />";
            }
        }
        else {
            dataAttr = "";
            dataAttr += " data-spnQuickListGUID='" + self.QuickListGUID + "'";
            dataAttr += " data-spnOrderItemType='" + self.ItemType + "'";
            dataAttr += " data-itemWisePONumber='" + self.ItemBlanketPO + "'";

            if ($NewPullAction.val() == "Pull") {
                //return "<input type='button' value='Pull' onclick='return LoadQuickListData(this);' id='btnLoad' class='CreateBtn pullbutton' style='float: none;padding: 2px 6px;margin-left: 5px;' />" + "<span id='spnQuickListGUID'  style='display:none'>" + self.QuickListGUID + "</span>" + "<span id='spnOrderItemType'  style='display:none'>" + self.ItemType + "</span>" + "<input type='hidden' id='itemWisePONumber' value='" + self.ItemBlanketPO + "' />";
                return "<input type='button' value='" + Pull + "' data-val='Pull' onclick='return LoadQuickListData(this);' id='btnLoad' class='CreateBtn pullbutton' style='float: none;padding: 2px 6px;margin-left: 5px;' " + dataAttr + "/>";
            }
            else if ($NewPullAction.val() == "Credit") {
                //return "<input type='button' value='Credit' onclick='return CreditItems(this);' id='btnAdd' class='CreateBtn pullbutton' style='float: none;padding: 2px 6px;margin-left: 5px;' />" + "<span id='spnQuickListGUID'  style='display:none'>" + self.QuickListGUID + "</span>" + "<span id='spnOrderItemType'  style='display:none'>" + self.ItemType + "</span>" + "<input type='hidden' id='itemWisePONumber' value='" + self.ItemBlanketPO + "' />" + "<span id='spnIsIgnoreCreditRule'  style='display:none'>" + serIsIgnoreCreditRule + "</span>" + "<span id='spnItemNumber' style='display:none'>" + self.ItemNumber + "</span>";
                dataAttr += " data-spnIsIgnoreCreditRule='" + serIsIgnoreCreditRule + "'";
                dataAttr += " data-spnItemNumber='" + self.ItemNumber + "'";

                return "<input type='button' value='" + Credit +"' onclick='return CreditItems(this);' id='btnAdd' class='CreateBtn pullbutton' style='float: none;padding: 2px 6px;margin-left: 5px;' " + dataAttr + " />";
            }
            else if ($NewPullAction.val() == "CreditMS") {
                dataAttr += " data-spnIsIgnoreCreditRule='" + serIsIgnoreCreditRule + "'";
                dataAttr += " data-spnItemNumber='" + self.ItemNumber + "'";

                //return "<input type='button' value='Credit MS' onclick='return MsCreditItems(this);' id='btnLoad' class='CreateBtn pullbutton' style='float: none;padding: 2px 6px;margin-left: 5px;' />" + "<span id='spnQuickListGUID'  style='display:none'>" + self.QuickListGUID + "</span>" + "<span id='spnOrderItemType'  style='display:none'>" + self.ItemType + "</span>" + "<input type='hidden' id='itemWisePONumber' value='" + self.ItemBlanketPO + "' />" + "<span id='spnIsIgnoreCreditRule'  style='display:none'>" + serIsIgnoreCreditRule + "</span>" + "<span id='spnItemNumber' style='display:none'>" + self.ItemNumber + "</span>";
                return "<input type='button' value='" + CreditMS +"' onclick='return MsCreditItems(this);' id='btnLoad' class='CreateBtn pullbutton' style='float: none;padding: 2px 6px;margin-left: 5px;' " + dataAttr + " />";
            }
            else {
                //return "" + "<input type='hidden' id='itemWisePONumber' value='" + self.ItemBlanketPO + "' />";
                return "<input type='button' style='display:none' data-itemWisePONumber='" + self.ItemBlanketPO + "' />";
            }
        }
    }

    self.sDispImg = fnDispImg();

    var fnTxtQty = function () {

        if ($NewPullAction.val() == "Credit" || $NewPullAction.val() == "CreditMS") {
            if (self.SerialNumberTracking == true) {
                if (self.QLCreditQuantity != null) {
                    var QuantityToCredit = 0;
                    if (QuickListPULLQty > 0) {
                        QuantityToCredit = self.QLCreditQuantity * QuickListPULLQty;
                    }
                    else {
                        QuantityToCredit = self.QLCreditQuantity;
                    }
                    return "<input maxlength='10' type='text' value='" + QuantityToCredit + "' class='text-boxinner text-boxQuantityFormatSR numericalign' id='txtQty' onkeypress = 'return onlyNumeric(event);' style='width:60px;' />";
                }
                else {
                    return "<input maxlength='10' type='text' value='0' class='text-boxinner text-boxQuantityFormatSR numericalign' id='txtQty' onkeypress = 'return onlyNumeric(event);' style='width:60px;' />";
                }
            }
            else {
                if (self.QLCreditQuantity != null) {
                    var QuantityToCredit = 0;
                    if (QuickListPULLQty > 0) {
                        QuantityToCredit = self.QLCreditQuantity * QuickListPULLQty;
                    }
                    else {
                        QuantityToCredit = self.QLCreditQuantity;
                    }
                    return "<input maxlength='10' type='text' value='" + QuantityToCredit + "' class='text-boxinner numericinput numericalign' id='txtQty' onkeypress = 'return onlyNumeric(event);' style='width:60px;' />";
                }
                else {
                    return "<input maxlength='10' type='text' value='0' class='text-boxinner numericinput numericalign' id='txtQty' onkeypress = 'return onlyNumeric(event);' style='width:60px;' />";
                }
            }
        }
        else {
            if (self.QuickListGUID == null || self.QuickListGUID == '') {
                var vStrReturn = "";
                var dataAttr = " data-hdnPullQtyScanOverride='" + self.PullQtyScanOverride + "'";

                if (self.SerialNumberTracking == true) {
                    var DefaultPullQTy = self.DefaultPullQuantity;
                    if (DefaultPullQTy != null) {
                        if (isNaN(DefaultPullQTy)) {
                            DefaultPullQTy = DefaultPullQTy.replace("<span>", "");
                            DefaultPullQTy = DefaultPullQTy.replace("</span>", "");
                        }
                        dataAttr += " data-spnDefaultPullQuantity='" + parseFloat(DefaultPullQTy).toFixed(0) + "'";
                        vStrReturn = "<input maxlength='10' type='text' value='" + DefaultPullQTy + "' class='text-boxinner text-boxQuantityFormatSR numericalign' id='txtQty' onkeypress = 'return onlyNumeric(event);' style='width:60px;' " + dataAttr + " />";
                        //vStrReturn = "<input maxlength='10' type='text' value='" + DefaultPullQTy + "' class='text-boxinner text-boxQuantityFormatSR numericalign' id='txtQty' style='width:60px;' />" + "<span id='spnDefaultPullQuantity'  style='display:none'>" + parseFloat(DefaultPullQTy).toFixed(0) + "</span>";
                    }
                    else {
                        vStrReturn = "<input maxlength='10' type='text' value='' class='text-boxinner text-boxQuantityFormatSR numericalign' id='txtQty' onkeypress = 'return onlyNumeric(event);' style='width:60px;' " + dataAttr + "/>";
                    }
                }
                else {
                    var DefaultPullQTy = self.DefaultPullQuantity;
                    if (DefaultPullQTy != null) {
                        if (isNaN(DefaultPullQTy)) {
                            DefaultPullQTy = DefaultPullQTy.replace("<span>", "");
                            DefaultPullQTy = DefaultPullQTy.replace("</span>", "");
                        }
                        ////vStrReturn = "<input maxlength='10' type='text' value='" + self.DefaultPullQuantity + "' class='text-boxinner numericinput numericalign' id='txtQty' style='width:60px;' />" + "<span id='spnDefaultPullQuantity'  style='display:none'>" + parseFloat(self.DefaultPullQuantity).toFixed(parseInt($('#hdQuantitycentsLimit').val(), 10)) + "</span>";
                        //vStrReturn = "<input maxlength='10' type='text' value='" + DefaultPullQTy + "' class='text-boxinner numericinput numericalign' id='txtQty' style='width:60px;' />" + "<span id='spnDefaultPullQuantity'  style='display:none'>" + parseFloat(DefaultPullQTy) + "</span>";
                        dataAttr += " data-spnDefaultPullQuantity='" + parseFloat(DefaultPullQTy) + "'";
                        vStrReturn = "<input maxlength='10' type='text' value='" + DefaultPullQTy + "' class='text-boxinner numericinput numericalign' id='txtQty' onkeypress = 'return onlyNumeric(event);' style='width:60px;' " + dataAttr + "/>";
                    }
                    else {
                        vStrReturn = "<input maxlength='10' type='text' value='' class='text-boxinner numericinput numericalign' id='txtQty' onkeypress = 'return onlyNumeric(event);' style='width:60px;' " + dataAttr + "/>";
                    }
                }
                //vStrReturn = vStrReturn + "<input id='hdnPullQtyScanOverride'  type='hidden' value='" + self.PullQtyScanOverride + "' />"; // use "data-" attribute
                return vStrReturn;
            }
            else {
                return "<input maxlength='10' type='text' value='' class='text-boxinner numericalign' id='txtQty' onkeypress = 'return onlyNumeric(event);' style='width:60px;' />";
            }
        }
    };

    self.txtQty = fnTxtQty();

    var fntxtProjectSpent =  function () {
            if (self.QuickListGUID == null || self.QuickListGUID == '') {
                var dataAttr = '';
                var strReturn = '';
                
            //if (self.ItemType != 4) {
                if (self.IsDefaultProjectSpend == false || (!ProjectSpendVisible)) {
                    if (ProjectSpendNameSCR == '') {
                            dataAttr += " data-ProjectID=''";
                            strReturn = '<span style="position:relative"><input type="text" id="txtProjectSpent" class="text-boxinner AutoPullProjectSpents" style = "width:93%;" value="" ' + dataAttr + ' />';
                            //strReturn = strReturn + ' <input type="hidden" id="ProjectID" value="" />';
                            strReturn = strReturn + ' <a id="lnkShowAllOptions" href="javascript:void(0);" style="position:absolute; right:5px; top:0px;" class="ShowAllOptions" ><img src="/Content/images/arrow_down_black.png" alt="select" /></a></span>';
                        }
                        else {
                        dataAttr += " data-ProjectID='" + ProjectSpendGuidSC + "'";
                        strReturn = '<span style="position:relative"><input type="text" id="txtProjectSpent" class="text-boxinner" disabled="disabled" readonly="readonly" style = "width:93%;" value="' + ProjectSpendNameSCR + '" ' + dataAttr + ' />';
                        //strReturn = strReturn + ' <input type="hidden" id="ProjectID" value="' + ProjectSpendGuidSCR + '" />';

                    }
                }
                else {
                        dataAttr += " data-ProjectID='" + self.DefaultProjectSpendGuid + "'";
                        strReturn = '<span style="position:relative"><input type="text" disabled="disabled" id="txtProjectSpent" class="text-boxinner AutoPullProjectSpents" style = "width:93%;" value="' + self.DefaultProjectSpend + '" ' + dataAttr + ' />';
                        //strReturn = strReturn + ' <input type="hidden" id="ProjectID" value="' + self.DefaultProjectSpendGuid + '" />';
                        strReturn = strReturn + ' <a id="lnkShowAllOptions" href="javascript:void(0);" disabled="disabled" style="position:absolute; right:5px; top:0px;" class="ShowAllOptions" ><img src="/Content/images/arrow_down_black.png" alt="select" /></a></span>';
                    }

                return strReturn
                //}
            }
            return "";
        }
    
    self.txtProjectSpentCol = fntxtProjectSpent();
    
    if (serIsIgnoreCreditRule == "true" || serIsIgnoreCreditRule == "True" || serIsIgnoreCreditRule == true) {

        var fnStagingHeaderName = function (obj, val) {
            if ($NewPullAction.val() == "CreditMS") {

                var vStagingHeaderName = '';
                var dataAttr = "";
                var strReturn = '<span style="display:none">"' + vStagingHeaderName + '"</span><span style="position:relative">';
                if (hasOnTheFlyEntryRight == "False") {
                    dataAttr += " data-hdnIsLoadMoreStaging = ''";
                    //strReturn = strReturn + '<input type="hidden" value="" id="hdnIsLoadMoreStaging" /><input type="text" id="txtMSStagingHeader" name="txtMSStagingHeader" class="text-boxinner AutoMSCreditStagingHeader" style = "width:93%;"/>';
                    strReturn = strReturn + '<input type="text" id="txtMSStagingHeader" name="txtMSStagingHeader" class="text-boxinner AutoMSCreditStagingHeader" style = "width:93%;"/>';
                }
                else {
                    dataAttr += " data-hdnIsLoadMoreStaging = ''";
                    //strReturn = strReturn + '<input type="hidden" id="hdnIsLoadMoreStaging" /><input type="text" id="txtMSStagingHeader" name="txtMSStagingHeader" class="text-boxinner AutoMSCreditStagingHeader" style = "width:93%;" />';
                    strReturn = strReturn + '<input type="text" id="txtMSStagingHeader" name="txtMSStagingHeader" class="text-boxinner AutoMSCreditStagingHeader" style = "width:93%;" />';
                }
                strReturn = strReturn + ' <a id="lnkShowAllOptionsCR" href="javascript:void(0);" style="position:absolute; right:5px; top:0px;" class="ShowAllOptionsStagingHeader" ><img src="/Content/images/arrow_down_black.png" alt="select" ' + dataAttr + ' /></a></span>';
                return strReturn;
            }
        }

        self.StagingHeaderName = fnStagingHeaderName();

        

    } // if serIsIgnoreCreditRule

    var fnImagePath = function () {
        var obj = self;
        if ((obj.ImagePath != '' && obj.ImagePath != null) || (obj.ItemImageExternalURL != '' && obj.ItemImageExternalURL != null)) {

            if (obj.ImageType != '' && obj.ImageType != null && obj.ImageType == 'ImagePath' && obj.ImagePath != '' && obj.ImagePath != null) {

                var path = logoPathItemImage;
                return '<img style="cursor:pointer;"  alt="' + (obj.ItemNumber) + '" id="ItemImageBox" width="120px" height="120px" src="' + path + "/" + obj.ID + "/" + obj.ImagePath + '">';
            }
            else if (obj.ImageType != '' && obj.ImageType != null && obj.ImageType == "ExternalImage" && obj.ItemImageExternalURL != '' && obj.ItemImageExternalURL != null) {
                return '<img style="cursor:pointer;"  alt="' + (obj.ItemNumber) + '" id="ItemImageBox" width="120px" height="120px" src="' + obj.ItemImageExternalURL + '">';
            }
            else {
                return "<img src='../Content/images/no-image.jpg' />";
            }
        }
        else {
            return "<img src='../Content/images/no-image.jpg' />";
        }

    }

    self.ImagePathDisp = fnImagePath();

    var fnDefaultLocationNameDisp = function () {
        var obj = self;
        if (obj.QuickListGUID == null || obj.QuickListGUID == '') {

            if ($('#NewPullAction').val() == "Credit") {
                var vDefaultLocationName = '';
                if (obj.ItemType != 4) {
                    vDefaultLocationName = obj.DefaultLocationName;
                }

                vDefaultLocationName = ((obj.BinNumber != undefined && obj.BinNumber != null && obj.BinNumber != "") ? obj.BinNumber : obj.DefaultLocationName);
                var dataAttr = " data-vDefaultLocationName='" + vDefaultLocationName + "'";
                var strReturn = '<span style="display:none">"' + vDefaultLocationName + '"</span><span style="position:relative">';
                if (hasOnTheFlyEntryRight == "False") {
                    //strReturn = strReturn + '<input type="hidden" value="" id="hdnIsLoadMoreLocations" /><input type="text" id="txtBinNumber" class="text-boxinner AutoCreditBins" style = "width:93%;" value="' + vDefaultLocationName + '" />';
                    dataAttr += " data-hdnIsLoadMoreLocations=''";
                    strReturn = strReturn + '<input type="text" id="txtBinNumber" class="text-boxinner AutoCreditBins" style = "width:93%;" value="' + vDefaultLocationName + '" />';
                }
                else {
                    //strReturn = strReturn + '<input type="hidden" value="false" id="hdnIsLoadMoreLocations" /><input type="text" id="txtBinNumber" class="text-boxinner AutoCreditBins" style = "width:93%;" value="' + vDefaultLocationName + '" />';
                    dataAttr += " data-hdnIsLoadMoreLocations='false'";
                    strReturn = strReturn + '<input type="text" id="txtBinNumber" class="text-boxinner AutoCreditBins" style = "width:93%;" value="' + vDefaultLocationName + '" />';
                }

                var binID = obj.DefaultLocation;

                binID = ((obj.BinID != undefined && obj.BinID != null && obj.BinID != "" && obj.BinID > 0) ? obj.BinID : obj.DefaultLocation);

                if (isNaN(parseInt(binID)) || parseInt(binID) <= 0) {
                    binID = '';
                }

                dataAttr += " data-BinID='" + binID + "'";
                //strReturn = strReturn + ' <input type="hidden" id="BinID" value="' + binID + '" />';

                var DefaultPullQTy1 = obj.ItemDefaultPullQuantity;
                if (DefaultPullQTy1 != null) {
                    if (isNaN(DefaultPullQTy1)) {
                        DefaultPullQTy1 = DefaultPullQTy1.replace("<span>", "");
                        DefaultPullQTy1 = DefaultPullQTy1.replace("</span>", "");
                    }
                }
                //strReturn = strReturn + ' <input type="hidden" id="hdnDPQ" value="' + DefaultPullQTy1 + '" />';
                dataAttr += " data-hdnDPQ='" + DefaultPullQTy1 + "'";
                strReturn = strReturn + ' <a id="lnkShowAllOptionsCR" href="javascript:void(0);" style="position:absolute; right:5px; top:0px;" class="ShowAllOptionsBinCR" ><img src="/Content/images/arrow_down_black.png" alt="select" ' + dataAttr + ' /></a></span>';
                return strReturn
            }
            else if ($('#NewPullAction').val() == "CreditMS") {

                var vDefaultLocationName = '';
                if (obj.ItemType != 4) {
                    vDefaultLocationName = obj.DefaultLocationName;
                }

                vDefaultLocationName = ((obj.BinNumber != undefined && obj.BinNumber != null && obj.BinNumber != "") ? obj.BinNumber : obj.DefaultLocationName);
                var dataAttrMS = " data-vDefaultLocationName='" + vDefaultLocationName + "'";
                var strReturn = '<span style="display:none">"' + vDefaultLocationName + '"</span><span style="position:relative">';
                if (hasOnTheFlyEntryRight == "False") {
                    //strReturn = strReturn + '<input type="hidden" value="" id="hdnIsLoadMoreLocations" /><input type="text" id="txtBinNumber" class="text-boxinner AutoMSCreditBins" style = "width:93%;" value="' + vDefaultLocationName + '" />';
                    dataAttrMS += " data-hdnIsLoadMoreLocations=''";
                    strReturn = strReturn + '<input type="text" id="txtBinNumber" class="text-boxinner AutoMSCreditBins" style = "width:93%;" value="' + vDefaultLocationName + '" />';
                }
                else {
                    //strReturn = strReturn + '<input type="hidden" value="false" id="hdnIsLoadMoreLocations" /><input type="text" id="txtBinNumber" class="text-boxinner AutoMSCreditBins" style = "width:93%;" value="' + vDefaultLocationName + '" />';
                    dataAttrMS += " data-hdnIsLoadMoreLocations='false'";
                    strReturn = strReturn + '<input type="text" id="txtBinNumber" class="text-boxinner AutoMSCreditBins" style = "width:93%;" value="' + vDefaultLocationName + '" />';
                }

                var binID = obj.DefaultLocation;

                binID = ((obj.BinID != undefined && obj.BinID != null && obj.BinID != "" && obj.BinID > 0) ? obj.BinID : obj.DefaultLocation);

                if (isNaN(parseInt(binID)) || parseInt(binID) <= 0) {
                    binID = '';
                }

                //strReturn = strReturn + ' <input type="hidden" id="BinID" value="' + binID + '" />';
                dataAttrMS += " data-BinID='" + binID + "'";
                var DefaultPullQTy1 = obj.ItemDefaultPullQuantity;
                if (DefaultPullQTy1 != null) {
                    if (isNaN(DefaultPullQTy1)) {
                        DefaultPullQTy1 = DefaultPullQTy1.replace("<span>", "");
                        DefaultPullQTy1 = DefaultPullQTy1.replace("</span>", "");
                    }
                }
                //strReturn = strReturn + ' <input type="hidden" id="hdnDPQ" value="' + DefaultPullQTy1 + '" />';
                dataAttrMS += " data-hdnDPQ='" + DefaultPullQTy1 + "'";
                strReturn = strReturn + ' <a id="lnkShowAllOptionsCR" href="javascript:void(0);" style="position:absolute; right:5px; top:0px;" class="ShowAllOptionsBinMCR" ><img src="/Content/images/arrow_down_black.png" alt="select" ' + dataAttrMS + ' /></a></span>';
                return strReturn
            }
            else {
                var vDefaultLocationName = '';
                if (obj.ItemType != 4) {
                    vDefaultLocationName = obj.DefaultLocationName;
                }

                vDefaultLocationName = ((obj.BinNumber != undefined && obj.BinNumber != null && obj.BinNumber != "") ? obj.BinNumber : obj.DefaultLocationName);
                var dataAttrP = " data-vDefaultLocationName='" + vDefaultLocationName + "'";
                var strReturn = "";
                if (AllowPullBeyondAvailableQty == "True"
                    && hasOnTheFlyEntryRight == "True"
                    && obj.SerialNumberTracking == false
                    && obj.LotNumberTracking == false
                    && obj.DateCodeTracking == false) {
                    strReturn = '<span style="display:none">"' + vDefaultLocationName + '"</span><span style="position:relative"><input type="text" id="txtBinNumber" class="text-boxinner AutoPullBins" style = "width:93%;" value="' + vDefaultLocationName + '" />';
                }
                else {
                    strReturn = '<span style="display:none">"' + vDefaultLocationName + '"</span><span style="position:relative"><input type="text" id="txtBinNumber" class="text-boxinner AutoPullBins bin-input-readonly" style = "width:93%;" value="' + vDefaultLocationName + '" />';
                }
                //var strReturn = '<span style="position:relative"><input type="text" id="txtBinNumber" class="text-boxinner AutoPullBins bin-input-readonly" style = "width:93%;" value="' + vDefaultLocationName + '" />';
                var binID = obj.DefaultLocation;

                binID = ((obj.BinID != undefined && obj.BinID != null && obj.BinID != "" && obj.BinID > 0) ? obj.BinID : obj.DefaultLocation);

                if (isNaN(parseInt(binID)) || parseInt(binID) <= 0) {
                    binID = '';
                }

                //strReturn = strReturn + ' <input type="hidden" id="BinID" value="' + binID + '" />';
                dataAttrP += " data-BinID='" + binID + "'";

                var DefaultPullQTy1 = obj.ItemDefaultPullQuantity;
                if (DefaultPullQTy1 != null) {
                    if (isNaN(DefaultPullQTy1)) {
                        DefaultPullQTy1 = DefaultPullQTy1.replace("<span>", "");
                        DefaultPullQTy1 = DefaultPullQTy1.replace("</span>", "");
                    }
                }
                //strReturn = strReturn + ' <input type="hidden" id="hdnDPQ" value="' + DefaultPullQTy1 + '" />';
                dataAttrP += " data-hdnDPQ='" + DefaultPullQTy1 + "'";
                strReturn = strReturn + ' <a id="lnkShowAllOptions" href="javascript:void(0);" style="position:absolute; right:5px; top:0px;" class="ShowAllOptionsBin" ><img src="/Content/images/arrow_down_black.png" alt="select" ' + dataAttrP + '/></a></span>';
                return strReturn
            }
        }
        else {
            return "";
        }
    };

    self.DefaultLocationNameDisp = fnDefaultLocationNameDisp();

    var fnSupplierNameDisp = function () {
        var obj = self;
        //return "<span>" + obj.aData.SupplierName + "</span><input type='hidden' id='supID' value=" + obj.aData.SupplierID + " />";
        var dataAttr = " data-supID='" + obj.SupplierID + "'";
        return "<span " + dataAttr + ">" + obj.SupplierName + "</span>";
    }

    self.SupplierNameDisp = fnSupplierNameDisp();

    var fnLongDescriptionDisp = function () {
        return "<div class='comment more'>" + self.LongDescription + "</div>";
    }

    self.LongDescriptionDisp = fnLongDescriptionDisp();

    self.fnGetFormattedQtyPrice = function (val, ftype) {

        if (val != null && val != NaN) {
            return FormatedCostQtyValues(val, ftype);
        }
        else {
            //return "<span>" + FormatedCostQtyValues(0, 2) + "</span>";
            return ftype == 1 ? tmpZeroCostQtyFormatted1 :
                (ftype == 2 ? tmpZeroCostQtyFormatted2 : tmpZeroCostQtyFormatted4);
        }
    }

    self.DefaultReorderQuantityDisp = self.fnGetFormattedQtyPrice(self.DefaultReorderQuantity,2);
    
    self.DefaultPullQuantityDisp = self.fnGetFormattedQtyPrice(self.DefaultPullQuantity,2);

    self.CostDisp = self.fnGetFormattedQtyPrice(self.Cost,1);

    //self.TrendDisp = GetBoolInFormat(self.Trend);

    //self.TaxableDisp = GetBoolInFormat(self.Taxable);

    //self.ConsignmentDisp = GetBoolInFormat(self.Consignment);

    self.StagedQuantityDisp = self.fnGetFormattedQtyPrice(self.StagedQuantity, 2);

    self.InTransitquantityDisp = self.fnGetFormattedQtyPrice(self.InTransitquantity, 2);

    self.OnOrderQuantityDisp = self.fnGetFormattedQtyPrice(self.OnOrderQuantity, 2);

    self.OnTransferQuantityDisp = self.fnGetFormattedQtyPrice(self.OnTransferQuantity, 2);
    
    self.SuggestedOrderQuantityDisp = self.fnGetFormattedQtyPrice(self.SuggestedOrderQuantity, 2);
    
    self.RequisitionedQuantityDisp = self.fnGetFormattedQtyPrice(self.RequisitionedQuantity, 2);
    
    self.AverageUsageDisp = self.fnGetFormattedQtyPrice(self.AverageUsage, 4);

    self.TurnsDisp = self.fnGetFormattedQtyPrice(self.Turns, 4);
        
    self.OnHandQuantityDisp = self.fnGetFormattedQtyPrice(self.OnHandQuantity, 2);
    
    self.CriticalQuantityDisp = self.fnGetFormattedQtyPrice(self.CriticalQuantity, 2);
    
    self.MinimumQuantityDisp = self.fnGetFormattedQtyPrice(self.MinimumQuantity, 2);

    self.MaximumQuantityDisp = self.fnGetFormattedQtyPrice(self.MaximumQuantity, 2);

    //self.IsTransferDisp = GetBoolInFormat(self, self.IsTransfer);

    //self.IsPurchaseDisp = GetBoolInFormat(self, self.IsPurchase);
        
    //self.SerialNumberTrackingDisp = GetBoolInFormat(self, self.SerialNumberTracking);
    
    //self.LotNumberTrackingDisp = GetBoolInFormat(self, self.LotNumberTracking);
    
    //self.DateCodeTrackingDisp = GetBoolInFormat(self, self.DateCodeTracking);

    var fnItemTypeDisp = function () {
        var val = self.ItemType;
        if (val == 1)
            return "Item";
        else if (val == 2)
            return "Quick List";
        else if (val == 3)
            return "Kit";
        else if (val == 4)
            return "Labor";
        else
            return "";
    }

    self.ItemTypeDisp = fnItemTypeDisp();
    

}; // dto


$(window).load(function () {
    if (window.location.href.toString().toLowerCase().indexOf("newpull") > 0) {
        TabEnableDisable("#tab1", false);
        TabEnableDisable("#tab5", true);
        $("#tab1").removeClass("unselected").addClass("selected");
        $("#tab5").removeClass("selected").addClass("unselected");
    }
    else if (window.location.href.toString().toLowerCase().indexOf("pullmasterlist") > 0) {
        TabEnableDisable("#tab1", true);
        TabEnableDisable("#tab5", false);
        $("#tab5").removeClass("unselected").addClass("selected");
        $("#tab1").removeClass("selected").addClass("unselected");
    }
});