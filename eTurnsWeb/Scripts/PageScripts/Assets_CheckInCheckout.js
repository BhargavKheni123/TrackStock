var sImageUrl1 = "/Content/images/";
var anOpen1 = [];
var vCheckOutStatus = "";
var vCheckedOutQTY = 0;
var vCheckedOutMQT = 0;
var vCheckInCheckOutID = "";
var IsCheckoutM = false;
var vQuantity = 0;
var vAvalQuantity = 0;
var vToolID = 0;
var vCheckedOutMQTYCurrent = 0;
var vCheckedOutQTYCurrent = 0;
var TechnicianGuid = '';
var TechnicianName = '';
var HistoryGUID;

_AssetsCheckInCheckout = (function ($) {
    var self = {};
        
    self.gridRowStartIndex = null;
    self.isSaveGridState = false;

    self.urls = {
        checkInCheckOutListAjaxUrl: null, checkInDataUrl: null
        , deleteCheckInCheckOutRecordsUrl: null, checkOutCheckInUrl: null
        , checkInAllUrl: null
    };

    self.initUrls = function (checkInCheckOutListAjaxUrl, checkInDataUrl, deleteCheckInCheckOutRecordsUrl 
        , checkOutCheckInUrl, checkInAllUrl
    ) {
        self.urls.checkInCheckOutListAjaxUrl = checkInCheckOutListAjaxUrl;
        self.urls.checkInDataUrl = checkInDataUrl;
        self.urls.deleteCheckInCheckOutRecordsUrl = deleteCheckInCheckOutRecordsUrl;
        self.urls.checkOutCheckInUrl = checkOutCheckInUrl;
        self.urls.checkInAllUrl = checkInAllUrl;
    };

    self.init = function (toolGUID) {

        self.initDataTable(toolGUID);
        self.initEvents(toolGUID);

    };

    self.initDataTable = function (toolGUID) {

        $(function (event) {

            //AllowDeletePopup = false;
            var tempCount = 0;
            //ColumnObjectAC = GetGridHeaderColumnsObject('ToolChekinCheckoutTable' + toolGUID);

            var ColumnObjectAC = new Array();            
            ColumnObjectAC.push(
                {
                    "mDataProp": null,
                    "bSortable": false,

                    "sClass": "read_only control1 center",
                    "sDefaultContent": '<img src="' + sImageUrl1 + 'drildown_open.jpg' + '">'
                });
            ColumnObjectAC.push(
                {
                    mDataProp: "btnCheckIn",//null, 
                    sClass: "read_only", bSortable: false, sDefaultContent: ''
                    //fnRender: function (obj, val) {

                    //    if (obj.aData.CheckOutStatus == "Check Out" && obj.aData.ToolDetailGUID == null) {

                    //        if (obj.aData.CheckedOutMQTY > 0) {
                    //            tempCount = obj.aData.CheckedOutMQTY - obj.aData.CheckedOutMQTYCurrent;
                    //        }
                    //        else {
                    //            tempCount = obj.aData.CheckedOutQTY - obj.aData.CheckedOutQTYCurrent;
                    //        }
                    //        if (tempCount == 0 || (!allowCheckinCheckOut))
                    //            return "";
                    //        else
                    //            return "<span id='spnCheckOutStatus' style='display:none'>" + obj.aData.CheckOutStatus + "</span>" + "<span id='spnCheckedOutQTY' style='display:none'>" + obj.aData.CheckedOutQTY + "</span><span id='spnCheckedOutMQTY' style='display:none'>" + obj.aData.CheckedOutMQTY + "</span>" + "<span id='spnCheckInCheckOutID'  style='display:none'>" + obj.aData.GUID + "</span>" + "<span id='spnQuantity'  style='display:none'>" + obj.aData.Quantity + "</span>" + "<span id='spnToolID'  style='display:none'>" + obj.aData.ToolGUID + "</span>" + "<input type='button' value='Check In' id='btnCheckIn' onclick='return PerformTCICOInner(this,1,'" + toolGUID + "');' class='CreateBtn pull'  />";
                    //    }
                    //    else {

                    //        return "";
                    //    }
                    //}
                });
            ColumnObjectAC.push({
                "mDataProp": "CheckinQuantity",//"Checkin Quantity",
                "bSortable": false,
                "sClass": "read_only",
                "sDefaultContent": ''
                //,"fnRender": function (obj, val) {

                //    if (obj.aData.CheckOutStatus == "Check Out") {
                //        if (obj.aData.CheckedOutMQTY > 0) {
                //            tempCount = obj.aData.CheckedOutMQTY - obj.aData.CheckedOutMQTYCurrent;
                //        }
                //        else {
                //            tempCount = obj.aData.CheckedOutQTY - obj.aData.CheckedOutQTYCurrent;
                //        }
                //        if (tempCount == 0) {
                //            return "";
                //        }
                //        else {

                //            if (obj.aData.IsGroupOfItems == 0) {
                //                return "<input type='text' value='" + 1 + "' class='numericinput  text-boxinner' onkeypress='return false;'  id='txtQty' style='width:93%;disabled:true;' />";
                //            }
                //            else {
                //                var AvailableQty = '0';
                //                if (obj.aData.CheckedOutMQTY > 0) {
                //                    AvailableQty = (obj.aData.CheckedOutMQTY - obj.aData.CheckedOutMQTYCurrent);
                //                }
                //                else {
                //                    AvailableQty = (obj.aData.CheckedOutQTY - obj.aData.CheckedOutQTYCurrent);
                //                }
                //                if (AvailableQty == '1') {
                //                    return "<input type='text' value='1' class='text-boxinner' id='txtQty' onkeypress='return onlyNumeric(event)' style='width:93%;' />";
                //                }
                //                else {
                //                    return "<input type='text' value='' class='text-boxinner' id='txtQty' onkeypress='return onlyNumeric(event)' style='width:93%;' />";
                //                }
                //            }
                //        }
                //    }
                //    else
                //        return "";
                //}
            });
            ColumnObjectAC.push({
                mDataProp: "CheckOutDate",//"CheckOutDate",
                sClass: "read_only"
                , fnRender: function (obj, val) { //return GetDateInFullFormat(val);
                    return obj.aData.CheckOutedDate;
                }
            });
            ColumnObjectAC.push({
                mDataProp: "CheckedOutQTY",//"CheckedOutQTY",
                sClass: "read_only"
                ,"fnRender": function (obj, val) {
                    if (obj.aData.CheckedOutQTY > 0) {
                        return obj.aData.CheckedOutQTY == null ? FormatedCostQtyValues(0, 2) : FormatedCostQtyValues(obj.aData.CheckedOutQTY, 2);
                    }
                    else {
                        return obj.aData.CheckedOutMQTY == null ? 0 : FormatedCostQtyValues(obj.aData.CheckedOutMQTY, 2);
                    }
                }
            });
            ColumnObjectAC.push({
                "mDataProp": "spnCheckedOutMQTYCurrent",//null,
                "bSortable": false,
                "sClass": "read_only",
                "sDefaultContent": ''
                //,"fnRender": function (obj, val) {
                //    if (obj.aData.CheckedOutMQTY > 0) {
                //        return obj.aData.CheckedOutMQTYCurrent == null ? FormatedCostQtyValues(0, 2) : FormatedCostQtyValues(obj.aData.CheckedOutMQTYCurrent, 2) + "<span id='spnCheckedOutMQTYCurrent' style='display:none'>" + obj.aData.CheckedOutMQTYCurrent + "</span>" + "<span id='spnCheckedOutQTYCurrent' style='display:none'>" + obj.aData.CheckedOutQTYCurrent + "</span>";
                //    }
                //    else {
                //        return obj.aData.CheckedOutQTYCurrent == null ? FormatedCostQtyValues(0, 2) : FormatedCostQtyValues(obj.aData.CheckedOutQTYCurrent, 2) + "<span id='spnCheckedOutMQTYCurrent' style='display:none'>" + obj.aData.CheckedOutMQTYCurrent + "</span>" + "<span id='spnCheckedOutQTYCurrent' style='display:none'>" + obj.aData.CheckedOutQTYCurrent + "</span>";;
                //    }
                //}
            });
            ColumnObjectAC.push({
                mDataProp: "RemainintQtyDisp",//null,
                "bSortable": false, sClass: "read_only", "sDefaultContent": ''
                //,"fnRender": function (obj, val) {

                //    var ret = null;
                //    if (obj.aData.CheckedOutMQTY > 0) {
                //        ret = FormatedCostQtyValues(obj.aData.CheckedOutMQTY - obj.aData.CheckedOutMQTYCurrent, 2);
                //    }
                //    else {
                //        ret =  FormatedCostQtyValues(obj.aData.CheckedOutQTY - obj.aData.CheckedOutQTYCurrent, 2);
                //    }

                //    //return "<span style='background-color:cyan'>" + ret + "</span>";
                //    return ret;
                //}
            });
            ColumnObjectAC.push({
                mDataProp: "ForMaintananceDisp", // null,
                "bSortable": false, sClass: "read_only", "sDefaultContent": ''
                //,ForMaintananceDisp = "fnRender": function (obj, val) {
                //    var ret = "";
                //    if (obj.aData.CheckedOutMQTY > 0) {
                //        ret = "Yes";
                //    }
                //    else {
                //        ret = "No";
                //    }


                ////return "<span style='background-color:cyan'>" + ret + "</span>";
                //    return ret;
                //}
            });
            ColumnObjectAC.push({ mDataProp: "UpdatedByName", sClass: "read_only" });
            //ColumnObject.push({ mDataProp: "Updated", sClass: "read_only", fnRender: function (obj, val) { return GetDateInFullFormat(val); } });
            ColumnObjectAC.push({
                mDataProp: "Updated",
                    sClass: "read_only"
                , fnRender: function (obj, val) { return obj.aData.UpdatedDate; }
            });

            ColumnObjectAC.push({ mDataProp: "UpdatedByName", sClass: "read_only" });
            ColumnObjectAC.push({ mDataProp: "AddedFrom", sClass: "read_only" });
            ColumnObjectAC.push({ mDataProp: "EditedFrom", sClass: "read_only" });
            ColumnObjectAC.push({
                mDataProp: "ReceivedOn",
                sClass: "read_only"
                ,fnRender: function (obj, val) { return obj.aData.ReceivedOnDate; }
            });
            ColumnObjectAC.push({
                mDataProp: "ReceivedOnWeb",//"ReceivedOnWeb",
                sClass: "read_only"
                , fnRender: function (obj, val) { return obj.aData.ReceivedOnDateWeb; }
            });
            //        ColumnObject.push({ mDataProp: "ReceivedOnDate", sClass: "read_only" });
            //        ColumnObject.push({ mDataProp: "ReceivedOnDateWeb", sClass: "read_only" });

            ColumnObjectAC.push({
                mDataProp: "TechnicianDisp",//"Technician",
                sClass: "read_only", bSortable: false
                //,fnRender: function (obj, val) {
                //    var Technician = obj.aData.Technician;
                //    if (Technician == '' || Technician == null) {
                //        Technician = '';
                //    }
                //    var TechnicianCode = obj.aData.TechnicianCode;
                //    if (TechnicianCode == '' || TechnicianCode == null) {
                //        TechnicianCode = '';
                //    }
                //    if (Technician != '') {
                //        return "<span id='spanTechName'>" + TechnicianCode + " --- " + Technician + "</span><input type='hidden' value='" + obj.aData.TechnicianGuid + "' id='hdntechGuid'/>";
                //    }
                //    else {
                //        return "<span id='spanTechName'>" + TechnicianCode + "</span><input type='hidden' value='" + obj.aData.TechnicianGuid + "' id='hdntechGuid'/>";
                //    }
                //}
            });
            ColumnObjectAC.push(
                {
                    mDataProp: "CheckoutFromKitDisp",//null, 
                    sClass: "read_only", bSortable: false, sDefaultContent: ''
                    //, fnRender: function (obj, val) {
                    //    var ret = null;
                    //    if (obj.aData.ToolDetailGUID != null) {
                    //        ret = "Yes";
                    //    }
                    //    else {
                    //        ret = "No";
                    //    }

                    //    //return "<span style='background-color:cyan'>" + ret + "</span>";
                    //return ret;
                    //}
                });

            //ColumnObject.push(arrToolCheckInOutHistory);
            $.each(arrToolCheckInOutHistory, function (index, val) {
                ColumnObjectAC.push(val);
            });

            PrepareItemLocationDataTable("ToolChekinCheckoutTable", toolGUID, self.urls.checkInCheckOutListAjaxUrl
                , 'CheckinCheckOutList', ColumnObjectAC, null, null, function (json) {
                    var bindJson = [];

                    $.each(json.aaData, function (index, obj) {
                        //bindJson.push(obj);
                        bindJson.push(new assets_CheckInCheckoutDTO(obj));
                    });

                    return bindJson;
                });

          

            /*Functions used for nasted data binding START*/
            $("#" + "ToolChekinCheckoutTable" + toolGUID).on("click", "td.control1", function (event) {

                var nTr1 = this.parentNode;
                var i1 = $.inArray(nTr1, anOpen1);

                if (i1 === -1) {

                    $('img', this).attr('src', sImageUrl1 + "drildown_close.jpg");
                    $("#" + "ToolChekinCheckoutTable" + toolGUID).DataTable().fnOpen(nTr1, fnFormatDetails1($("#" + "ToolChekinCheckoutTable" + toolGUID).DataTable(), nTr1), '');
                    anOpen1.push(nTr1);
                }
                else {
                    $('img', this).attr('src', sImageUrl1 + "drildown_open.jpg");
                    $("#" + "ToolChekinCheckoutTable" + toolGUID).DataTable().fnClose(nTr1);
                    anOpen1.splice(i1, 1);
                    //$("#" + "ToolChekinCheckoutTable" + '@ViewBag.ToolGUID').DataTable().fnDraw(); // commented for WI-4229
                }
            });

            function fnFormatDetails1(oTable1, nTr1) {
                HistoryGUID = toolGUID;
                var oData1 = $("#" + "ToolChekinCheckoutTable" + toolGUID).DataTable().fnGetData(nTr1);
                var sOut1 = '';
                $('#DivLoading').show();
                $.ajax({
                    "url": self.urls.checkInDataUrl,
                    data: { CheckInCheckOutGUID: oData1.GUID },
                    "async": false,
                    cache: false,
                    "dataType": "text",
                    "success": function (json) {
                        sOut1 = json;
                        $('#DivLoading').hide();
                    },
                    error: function (response) {
                    }
                });

                return sOut1;
            }
            /*Functions used for nasted data binding END*/

            $("#" + "ToolChekinCheckoutTable" + toolGUID + '_wrapper .ColVis').css({ 'left': '780px' });

        });
    };

    self.initEvents = function (toolGUID) {
        $(document).ready(function () {
            $("input#btnToolCheckInAllNewFlow").unbind("click");
            $('body').off('click', "input#btnToolCheckInAllNewFlow");
            $('body').on('click', "input#btnToolCheckInAllNewFlow", function (event) {
                $(this).prop('disabled', true);
                SuccessMessage = '';
                ErrorMessage = '';
                arrItems = new Array();

                if ($("#" + "ToolChekinCheckoutTable" + toolGUID + ' tr.row_selected').length <= 0) {
                    alert(MsgCheckInValidation);
                    $(this).prop('disabled', false);
                }
                else {
                    $("#" + "ToolChekinCheckoutTable" + toolGUID + '  tr.row_selected').each(function (i) {

                        if ($(this).attr('class').indexOf('row_selected') != -1 && $(this).find("#txtQty").length > 0 && ($(this).find("#txtQty").val() != '' || $(this).find("#txtQty").val() != '0')) {
                            $(this).find("btnCheckOut").prop('disabled', true);
                            if (PerformTCINew($(this).find("input#btnCheckIn"), 1) == false) {
                                return false;
                            }

                        }
                    });

                    if (ErrorMessage != '') {
                        $("#" + "ToolChekinCheckoutTable" + toolGUID + '  tr.row_selected').each(function (i) {
                            if ($(this).attr('class').indexOf('row_selected') != -1) {
                                $(this).prop('disabled', false);
                            }
                        });
                        $("#btnToolCheckInAllNewFlow").prop('disabled', false);
                        showNotificationDialog();
                        $("#spanGlobalMessage").html(ErrorMessage);
                        $("#spanGlobalMessage").removeClass('succesIcon').addClass('errorIcon WarningIcon');
                    }
                    else {
                        if (arrItems.length > 0) {
                            $.ajax({
                                "url": _AssetsCheckInCheckout.urls.checkInAllUrl,
                                data: { "arrItems": JSON.stringify(arrItems) },
                                "async": false,
                                "cache": false,
                                dataType: "text",
                                "success": function (json) {
                                    $("#btnToolCheckoutAllNewFlow").prop('disabled', false);
                                    //if (json == "ok") {

                                    showNotificationDialog();
                                    //SuccessMessage=" Checkout Successfully Updated." ;
                                    $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                                    $("#spanGlobalMessage").html(json);
                                    IsClearGlobalFilter = false;
                                    ResetNarrowSearchTechnicianList();

                                    //}
                                    //else {

                                    //    ErrorMessage = json;
                                    //    showNotificationDialog();
                                    //    $("#spanGlobalMessage").text(ErrorMessage);
                                    //    $("#spanGlobalMessage").removeClass('succesIcon').addClass('errorIcon WarningIcon');
                                    //    ResetNarrowSearchTechnicianList();
                                    //}
                                },
                                error: function (response) {
                                    $("#btnToolCheckoutAllNewFlow").prop('disabled', false);
                                }
                            });
                        }
                        else {
                            ErrorMessage = MsgCheckedOutQuantityValidation;
                            showNotificationDialog();
                            $("#spanGlobalMessage").html(ErrorMessage);
                            $("#spanGlobalMessage").removeClass('succesIcon').addClass('errorIcon WarningIcon');
                        }
                    }
                    $("#btnToolCheckoutAllNewFlow").prop('disabled', false);

                }
            });
        });
    };

    
        
    
    return self;

})(jQuery);


function DeleteItemLocatino(toolGUID) {
    DeleteDynemicTableData_New($("#" + "ToolChekinCheckoutTable" + toolGUID).DataTable(), _AssetsCheckInCheckout.urls.deleteCheckInCheckOutRecordsUrl);
}

function CheckInOperationData(toolGUID, btnObject) {

    if (IsCheckoutM && vQuantity > (vCheckedOutMQT - vCheckedOutMQTYCurrent)) {
        //alert('Check in quantity must be less then Check out quantity. i1.e. ' + (vCheckedOutMQT - vCheckedOutMQTYCurrent));
        //$('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        showNotificationDialog();
        $("#spanGlobalMessage").html(MsgCheckinCheckoutValidation.replace("{0}",(vCheckedOutMQT - vCheckedOutMQTYCurrent)));
        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
        if (btnObject !== undefined && btnObject != null) {
            $(btnObject).prop('disabled', false);
        }
        return false;
    }
    if (!IsCheckoutM && vQuantity > (vCheckedOutQTY - vCheckedOutQTYCurrent)) {
        //alert('Check in quantity must be less then Check out quantity. i1.e. ' + (vCheckedOutQTY - vCheckedOutQTYCurrent));
        //$('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        showNotificationDialog();
        $("#spanGlobalMessage").html(MsgCheckinCheckoutValidation.replace("{0}", (vCheckedOutMQT - vCheckedOutMQTYCurrent)));
        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
        if (btnObject !== undefined && btnObject != null) {
            $(btnObject).prop('disabled', false);
        }
        return false;
    }
    if (IsCheckoutM && vCheckedOutMQT == 0) {
        //alert('Invalid Operation!!! No check out performed.');
        //$('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        showNotificationDialog();
        $("#spanGlobalMessage").html(MsgInvalidOperationNoCheckout);
        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
        if (btnObject !== undefined && btnObject != null) {
            $(btnObject).prop('disabled', false);
        }
        return false;
    }
    if (!IsCheckoutM && vCheckedOutQTY == 0) {
        //alert('Invalid Operation!!! No check out performed.');
        //$('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        showNotificationDialog();
        $("#spanGlobalMessage").html(MsgInvalidOperationNoCheckout);
        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
        if (btnObject !== undefined && btnObject != null) {
            $(btnObject).prop('disabled', false);
        }
        return false;
    }
    $.ajax({
        "url": _AssetsCheckInCheckout.urls.checkOutCheckInUrl,
        data: { ActionType: 'ci', Quantity: vQuantity, IsForMaintance: IsCheckoutM, ToolGUID: vToolID, AQty: vAvalQuantity, CQty: vCheckedOutQTY, CMQty: vCheckedOutMQT, CheckInCheckOutGUID: vCheckInCheckOutID, IsOnlyFromUI: true, TechnicianName: TechnicianName },
        "async": false,
        cache: false,
        "dataType": "text",
        "success": function (json) {
            if (json == "ok") {
                //$('div#target').fadeToggle();
                //$("div#target").delay(2000).fadeOut(200);
                showNotificationDialog();
                $("#spanGlobalMessage").html(MsgRecordSucessfullyUpdated);
                $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                $("#" + "ToolChekinCheckoutTable" + toolGUID).DataTable().fnDraw();
                IsClearGlobalFilter = false;
                ResetNarrowSearchTechnicianList();
                oTable.fnDraw();
            }
            else {
                showNotificationDialog();
                $("#spanGlobalMessage").html(json);
                $("#spanGlobalMessage").removeClass('succesIcon').addClass('errorIcon WarningIcon');
                return false;
            }
            //$(btnObject).prop('disabled', true);
        },
        error: function (response) {
        },
        complete: function () {
            if (btnObject !== undefined && btnObject != null)
            {
                $(btnObject).prop('disabled', false);
            }
        }
    });

    return true;
}

function PerformTCINew(obj, ActionType) {

    vCheckOutStatus = $(obj).parent().find('#spnCheckOutStatus').text() == "" ? "" : $(obj).parent().find('#spnCheckOutStatus').text();
    vCheckedOutQTY = $(obj).parent().find('#spnCheckedOutQTY').text() == "" ? 0 : $(obj).parent().find('#spnCheckedOutQTY').text();
    vCheckedOutMQT = $(obj).parent().find('#spnCheckedOutMQTY').text() == "" ? 0 : $(obj).parent().find('#spnCheckedOutMQTY').text();
    vCheckInCheckOutID = $(obj).parent().find('#spnCheckInCheckOutID').text() == "" ? 0 : $(obj).parent().find('#spnCheckInCheckOutID').text();

    TechnicianName = $(obj).parent().parent().find("#spanTechName").text() == "" ? "" : $(obj).parent().parent().find("#spanTechName").text();
    vToolID = $(obj).parent().find('#spnToolID').text() == "" ? 0 : $(obj).parent().find('#spnToolID').text();
    vCheckedOutMQTYCurrent = $(obj).parent().parent().find('#spnCheckedOutMQTYCurrent').text() == "" ? 0 : $(obj).parent().parent().find('#spnCheckedOutMQTYCurrent').text();
    vCheckedOutQTYCurrent = $(obj).parent().parent().find('#spnCheckedOutQTYCurrent').text() == "" ? 0 : $(obj).parent().parent().find('#spnCheckedOutQTYCurrent').text();

    //IsCheckoutM = $(obj).parent().parent().find('#chkMaintance').attr('checked') ? true : false;

    vQuantity = $(obj).parent().parent().find('#txtQty').val() == "" ? 0 : $(obj).parent().parent().find('#txtQty').val();
    vAvalQuantity = 0; //$(obj).parent().find('#spnQuantity').text() == "" ? 0 : $(obj).parent().find('#spnQuantity').text();

    vCheckedOutQTY = parseInt(vCheckedOutQTY, 10);
    vCheckedOutMQT = parseInt(vCheckedOutMQT, 10);
    //vCheckInCheckOutID = parseInt(vCheckInCheckOutID, 10);
    //vToolID = parseInt(vToolID, 10);
    vQuantity = parseInt(vQuantity, 10);

    if (vCheckedOutQTY > 0)
        IsCheckoutM = false;
    else if (vCheckedOutMQT > 0)
        IsCheckoutM = true;
    //vCheckInCheckOutID = parseInt(vCheckInCheckOutID, 10);
    //vToolID = parseInt(vToolID, 10);
    //vQuantity = parseInt(vQuantity, 10);


    if (vQuantity == 0) {
        //alert('Kindly insert proper quantity value to perform the operation.');
        //$('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);

        ErrorMessage += MsgInsertProperQuantityValue;
        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
        return false;
    }
    if ($("#txtUseThisTechnician") != null) {
        //vTechnician = $("body").find("input#UseThisTechnicianGUID").val();
        vTechnicianName = $("body").find("input#txtUseThisTechnician").val();
    }
    if ($("#chkUseToolCommonUDF").is(":checked")) {
        //TechnicianGuid = vTechnician;
        TechnicianName = vTechnicianName;
    }
    //if (TechnicianGuid == '00000000-0000-0000-0000-000000000000' || TechnicianGuid == '') {
    //    //alert('Kindly insert proper quantity value to perform the operation.');
    //    //$('div#target').fadeToggle();
    //    //$("div#target").delay(2000).fadeOut(200);
    //    //showNotificationDialog();
    //    ErrorMessage += 'Kindly select Technician from TechnicianList.';
    //    $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
    //    return false;
    //}
    if (TechnicianName == null || TechnicianName == undefined || TechnicianName.trim() == '' || TechnicianName.trim() == '@eTurns.DTO.Resources.ResCommon.SelectTechnicianText') {
        showNotificationDialog();
        $("#spanGlobalMessage").html(MsgKindlyFillTechnician);
        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
        $(obj).removeAttr('disabled');
        return false;
    }
    else if (TechnicianName.indexOf('-') >= 0) {
        if (TechnicianName.split('-')[0].trim() == '') {
            showNotificationDialog();
            $("#spanGlobalMessage").html(MsgInvalidTechnician);
            $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
            $(obj).removeAttr('disabled');
            ErrorMessage = ErrorMessage + (ErrorMessage != '' ? ' ' : '') + Invalidtechincian;
            return false;
        }
    }

    if ($(obj).parent().parent().find('#UDF1') != null) {
        if ($(obj).parent().parent().find('#UDF1').attr("class") == 'selectBox') {
            vUDF1 = $(obj).parent().parent().find('#UDF1 option:selected').text().replace(/'/g, "''");
        }
        else {
            vUDF1 = $(obj).parent().parent().find('#UDF1').val();
        }
    }

    if ($(obj).parent().parent().find('#UDF2') != null) {
        if ($(obj).parent().parent().find('#UDF2').attr("class") == 'selectBox') {
            vUDF2 = $(obj).parent().parent().find('#UDF2 option:selected').text().replace(/'/g, "''");
        }
        else {
            vUDF2 = $(obj).parent().parent().find('#UDF2').val();
        }
    }

    if ($(obj).parent().parent().find('#UDF3') != null) {
        if ($(obj).parent().parent().find('#UDF3').attr("class") == 'selectBox') {
            vUDF3 = $(obj).parent().parent().find('#UDF3 option:selected').text().replace(/'/g, "''");
        }
        else {
            vUDF3 = $(obj).parent().parent().find('#UDF3').val();
        }
    }

    if ($(obj).parent().parent().find('#UDF4') != null) {
        if ($(obj).parent().parent().find('#UDF4').attr("class") == 'selectBox') {
            vUDF4 = $(obj).parent().parent().find('#UDF4 option:selected').text().replace(/'/g, "''");
        }
        else {
            vUDF4 = $(obj).parent().parent().find('#UDF4').val();
        }
    }

    if ($(obj).parent().parent().find('#UDF5') != null) {
        if ($(obj).parent().parent().find('#UDF5').attr("class") == 'selectBox') {
            vUDF4 = $(obj).parent().parent().find('#UDF5 option:selected').text().replace(/'/g, "''");
        }
        else {
            vUDF5 = $(obj).parent().parent().find('#UDF5').val();
        }
    }
    if ($("#UDF1ToolCommon") != null) {
        if ($("#UDF1ToolCommon").attr("class") == 'selectBox') {
            vUDF1ToolCommon = $("#UDF1ToolCommon option:selected").text();
        }
        else {
            vUDF1ToolCommon = $("#UDF1ToolCommon").val();
        }
    }

    if ($("#UDF2ToolCommon") != null) {
        if ($("#UDF2ToolCommon").attr("class") == 'selectBox') {
            vUDF2ToolCommon = $("#UDF2ToolCommon option:selected").text();
        }
        else {
            vUDF2ToolCommon = $("#UDF2ToolCommon").val();
        }
    }

    if ($("#UDF3ToolCommon") != null) {
        if ($("#UDF3PullCommon").attr("class") == 'selectBox') {
            vUDF3ToolCommon = $("#UDF3ToolCommon option:selected").text();
        }
        else {
            vUDF3ToolCommon = $("#UDF3ToolCommon").val();
        }
    }

    if ($("#UDF4ToolCommon") != null) {
        if ($("#UDF4PullCommon").attr("class") == 'selectBox') {
            vUDF4ToolCommon = $("#UDF4ToolCommon option:selected").text();
        }
        else {
            vUDF4ToolCommon = $("#UDF4ToolCommon").val();
        }
    }

    if ($("#UDF5ToolCommon") != null) {
        if ($("#UDF5ToolCommon").attr("class") == 'selectBox') {
            vUDF5ToolCommon = $("#UDF5ToolCommon option:selected").text();
        }
        else {
            vUDF5ToolCommon = $("#UDF5ToolCommon").val();
        }
    }
    if ($("#chkUseToolCommonUDF").is(":checked")) {
        vUDF1 = vUDF1ToolCommon;
        vUDF2 = vUDF2ToolCommon;
        vUDF3 = vUDF3ToolCommon;
        vUDF4 = vUDF4ToolCommon;
        vUDF5 = vUDF5ToolCommon;
    }

    //if (ActionType == 1) // 1 = check in , 2 = check out (Action Type)
    //{
    return CheckInOperationNew();
    //}
    //else {
    //    return CheckOutOperationNew();
    //}
}
function CheckInOperationNew() {
    if (IsCheckoutM && vQuantity > (vCheckedOutMQT - vCheckedOutMQTYCurrent)) {
        //alert('Check in quantity must be less then Check out quantity. i1.e. ' + (vCheckedOutMQT - vCheckedOutMQTYCurrent));
        //$('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        showNotificationDialog();
        $("#spanGlobalMessage").html(MsgCheckinCheckoutValidation.replace("{0}", (vCheckedOutMQT - vCheckedOutMQTYCurrent)));
        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
        return false;
    }
    if (!IsCheckoutM && vQuantity > (vCheckedOutQTY - vCheckedOutQTYCurrent)) {
        //alert('Check in quantity must be less then Check out quantity. i1.e. ' + (vCheckedOutQTY - vCheckedOutQTYCurrent));
        //$('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        showNotificationDialog();
        $("#spanGlobalMessage").html(MsgCheckinCheckoutValidation.replace("{0}", (vCheckedOutMQT - vCheckedOutMQTYCurrent)));
        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
        return false;
    }
    if (IsCheckoutM && vCheckedOutMQT == 0) {
        //alert('Invalid Operation!!! No check out performed.');
        //$('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        showNotificationDialog();
        $("#spanGlobalMessage").html(MsgInvalidOperationNoCheckout);
        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
        return false;
    }
    if (!IsCheckoutM && vCheckedOutQTY == 0) {
        //alert('Invalid Operation!!! No check out performed.');
        //$('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        showNotificationDialog();
        $("#spanGlobalMessage").html(MsgInvalidOperationNoCheckout);
        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
        return false;
    }
    var data = {
        ActionType: 'ci', Quantity: vQuantity,
        IsForMaintance: IsCheckoutM, ToolGUID: vToolID, AQty: vAvalQuantity,
        CQty: vCheckedOutQTY, CMQty: vCheckedOutMQT, UDF1: vUDF1, UDF2: vUDF2, UDF3: vUDF3, UDF4: vUDF4,
        UDF5: vUDF5, CheckInCheckOutGUID: vCheckInCheckOutID, IsOnlyFromUI: true, TechnicianName: TechnicianName, "ToolName": ToolName
    };

    //alert(JSON.stringify(data));
    arrItems.push(data);

}

function PerformTCICOInner(obj, ActionType, toolGUID) {
    $(obj).prop('disabled', true);
    vCheckOutStatus = $(obj).parent().find('#spnCheckOutStatus').text() == "" ? "" : $(obj).parent().find('#spnCheckOutStatus').text();
    vCheckedOutQTY = $(obj).parent().find('#spnCheckedOutQTY').text() == "" ? 0 : $(obj).parent().find('#spnCheckedOutQTY').text();
    vCheckedOutMQT = $(obj).parent().find('#spnCheckedOutMQTY').text() == "" ? 0 : $(obj).parent().find('#spnCheckedOutMQTY').text();
    vCheckInCheckOutID = $(obj).parent().find('#spnCheckInCheckOutID').text() == "" ? 0 : $(obj).parent().find('#spnCheckInCheckOutID').text();
    TechnicianName = $(obj).parent().parent().find("#spanTechName").text() == "" ? "" : $(obj).parent().parent().find("#spanTechName").text();
    vToolID = $(obj).parent().find('#spnToolID').text() == "" ? 0 : $(obj).parent().find('#spnToolID').text();
    vCheckedOutMQTYCurrent = $(obj).parent().parent().find('#spnCheckedOutMQTYCurrent').text() == "" ? 0 : $(obj).parent().parent().find('#spnCheckedOutMQTYCurrent').text();
    vCheckedOutQTYCurrent = $(obj).parent().parent().find('#spnCheckedOutQTYCurrent').text() == "" ? 0 : $(obj).parent().parent().find('#spnCheckedOutQTYCurrent').text();

    //IsCheckoutM = $(obj).parent().parent().find('#chkMaintance').attr('checked') ? true : false;

    vQuantity = $(obj).parent().parent().find('#txtQty').val() == "" ? 0 : $(obj).parent().parent().find('#txtQty').val();
    vAvalQuantity = 0; //$(obj).parent().find('#spnQuantity').text() == "" ? 0 : $(obj).parent().find('#spnQuantity').text();

    vCheckedOutQTY = parseInt(vCheckedOutQTY, 10);
    vCheckedOutMQT = parseInt(vCheckedOutMQT, 10);
    //vCheckInCheckOutID = parseInt(vCheckInCheckOutID, 10);
    //vToolID = parseInt(vToolID, 10);
    vQuantity = parseInt(vQuantity, 10);

    if (vCheckedOutQTY > 0)
        IsCheckoutM = false;
    else if (vCheckedOutMQT > 0)
        IsCheckoutM = true;


    if (vQuantity == 0) {
        //alert('Kindly insert proper quantity value to perform the operation.');
        //$('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        showNotificationDialog();
        $("#spanGlobalMessage").html(MsgInsertProperQuantityValue);
        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
        if (obj !== undefined && obj != null) {
            $(obj).prop('disabled', false);
        }
        return false;
    }

    return CheckInOperationData(toolGUID, obj);
}

assets_CheckInCheckoutDTO = function (serverObj) {
    var self = this;

    self.ID = serverObj.ID;
    self.ToolGUID = serverObj.ToolGUID;
    self.CheckOutStatus = serverObj.CheckOutStatus;
    self.CheckedOutQTY = serverObj.CheckedOutQTY;
    self.CheckedOutMQTY = serverObj.CheckedOutMQTY;
    self.CheckOutDate = serverObj.CheckOutDate;
    //self.CheckInDate = serverObj.CheckInDate;
    //self.Created = serverObj.Created;
    //self.CreatedBy = serverObj.CreatedBy;
    self.Updated = serverObj.Updated;
    //self.LastUpdatedBy = serverObj.LastUpdatedBy;
    //self.Room = serverObj.Room;
    //self.IsArchived = serverObj.IsArchived;
    //self.IsDeleted = serverObj.IsDeleted;
    //self.CompanyID = serverObj.CompanyID;
    self.UDF1 = serverObj.UDF1;
    self.UDF2 = serverObj.UDF2;
    self.UDF3 = serverObj.UDF3;
    self.UDF4 = serverObj.UDF4;
    self.UDF5 = serverObj.UDF5;
    //self.RoomName = serverObj.RoomName;
    //self.CreatedByName = serverObj.CreatedByName;
    self.UpdatedByName = serverObj.UpdatedByName;
    self.CheckedOutQTYCurrent = serverObj.CheckedOutQTYCurrent;
    self.CheckedOutMQTYCurrent = serverObj.CheckedOutMQTYCurrent;
    self.GUID = serverObj.GUID;
    self.ReceivedOn = serverObj.ReceivedOn;
    self.ReceivedOnWeb = serverObj.ReceivedOnWeb;
    self.AddedFrom = serverObj.AddedFrom;
    self.EditedFrom = serverObj.EditedFrom;
    //self.IsOnlyFromItemUI = serverObj.IsOnlyFromItemUI;
    self.CreatedDate = serverObj.CreatedDate;
    self.CheckOutedDate = serverObj.CheckOutedDate;
    self.UpdatedDate = serverObj.UpdatedDate;
    self.ReceivedOnDate = serverObj.ReceivedOnDate;
    self.ReceivedOnDateWeb = serverObj.ReceivedOnDateWeb;
    self.IsGroupOfItems = serverObj.IsGroupOfItems;
    self.TechnicianGuid = serverObj.TechnicianGuid;
    self.TechnicianCode = serverObj.TechnicianCode;
    self.Technician = serverObj.Technician;
    //self.RequisitionDetailGuid = serverObj.RequisitionDetailGuid;
    //self.RequisitionNumber = serverObj.RequisitionNumber;
    //self.WorkOrderGuid = serverObj.WorkOrderGuid;
    //self.WorkOrderNumber = serverObj.WorkOrderNumber;
    self.ToolDetailGUID = serverObj.ToolDetailGUID;
    //self.SerialNumber = serverObj.SerialNumber;
    self.SerialNumberTracking = serverObj.SerialNumberTracking;
    self.ToolBinID = serverObj.ToolBinID;
    self.Location = serverObj.Location;
    //self.TotalRecords = serverObj.TotalRecords;

    getbtnCheckIn = function () {

        if (self.CheckOutStatus == "Check Out" && self.ToolDetailGUID == null) {

            if (self.CheckedOutMQTY > 0) {
                tempCount = self.CheckedOutMQTY - self.CheckedOutMQTYCurrent;
            }
            else {
                tempCount = self.CheckedOutQTY - self.CheckedOutQTYCurrent;
            }
            if (tempCount == 0 || (!allowCheckinCheckOut)) {
                return "";
            }
            else {
                var tmptoolGuid = self.ToolGUID;
                var ret = "<span id='spnCheckOutStatus' style='display:none'>"
                    + self.CheckOutStatus + "</span>" + "<span id='spnCheckedOutQTY' style='display:none'>"
                    + self.CheckedOutQTY + "</span><span id='spnCheckedOutMQTY' style='display:none'>"
                    + self.CheckedOutMQTY + "</span>" + "<span id='spnCheckInCheckOutID'  style='display:none'>"
                    + self.GUID + "</span>" + "<span id='spnQuantity'  style='display:none'>"
                    + self.Quantity + "</span>" + "<span id='spnToolID'  style='display:none'>"
                    + self.ToolGUID + "</span>";
                ret += "<input type='button' id='btnCheckIn' onclick = PerformTCICOInner(this,1,'" + tmptoolGuid + "');  value='Check In' class='CreateBtn pull'  />";
                return ret;
            }
        }
        else {

            return "";
        }
    };


    self.btnCheckIn = getbtnCheckIn();

    getCheckinQuantity = function () {

        if (self.CheckOutStatus == "Check Out") {
            if (self.CheckedOutMQTY > 0) {
                tempCount = self.CheckedOutMQTY - self.CheckedOutMQTYCurrent;
            }
            else {
                tempCount = self.CheckedOutQTY - self.CheckedOutQTYCurrent;
            }
            if (tempCount == 0) {
                return "";
            }
            else {

                if (self.IsGroupOfItems == 0) {
                    return "<input type='text' value='" + 1 + "' class='numericinput  text-boxinner' onkeypress='return false;'  id='txtQty' style='width:93%;disabled:true;' />";
                }
                else {
                    var AvailableQty = '0';
                    if (self.CheckedOutMQTY > 0) {
                        AvailableQty = (self.CheckedOutMQTY - self.CheckedOutMQTYCurrent);
                    }
                    else {
                        AvailableQty = (self.CheckedOutQTY - self.CheckedOutQTYCurrent);
                    }
                    if (AvailableQty == '1') {
                        return "<input type='text' value='1' class='text-boxinner' id='txtQty' onkeypress='return onlyNumeric(event)' style='width:93%;' />";
                    }
                    else {
                        return "<input type='text' value='' class='text-boxinner' id='txtQty' onkeypress='return onlyNumeric(event)' style='width:93%;' />";
                    }
                }
            }
        }
        else
            return "";
    };

    self.CheckinQuantity = getCheckinQuantity();

    //getCheckedOutQTYDisp = function () {
    //    if (self.CheckedOutQTY > 0) {
    //        return self.CheckedOutQTY == null ? FormatedCostQtyValues(0, 2) : FormatedCostQtyValues(self.CheckedOutQTY, 2);
    //    }
    //    else {
    //        return self.CheckedOutMQTY == null ? 0 : FormatedCostQtyValues(self.CheckedOutMQTY, 2);
    //    }
    //};

    //self.CheckedOutQTYDisp = getCheckedOutQTYDisp();

    getspnCheckedOutMQTYCurrent = function () {
        if (self.CheckedOutMQTY > 0) {
            return self.CheckedOutMQTYCurrent == null ? FormatedCostQtyValues(0, 2) : FormatedCostQtyValues(self.CheckedOutMQTYCurrent, 2) + "<span id='spnCheckedOutMQTYCurrent' style='display:none'>"
                + self.CheckedOutMQTYCurrent + "</span>" + "<span id='spnCheckedOutQTYCurrent' style='display:none'>" + self.CheckedOutQTYCurrent + "</span>";
        }
        else {
            return self.CheckedOutQTYCurrent == null ? FormatedCostQtyValues(0, 2) : FormatedCostQtyValues(self.CheckedOutQTYCurrent, 2)
                + "<span id='spnCheckedOutMQTYCurrent' style='display:none'>" + self.CheckedOutMQTYCurrent + "</span>" + "<span id='spnCheckedOutQTYCurrent' style='display:none'>"
                + self.CheckedOutQTYCurrent + "</span>";
        }
    };

    self.spnCheckedOutMQTYCurrent = getspnCheckedOutMQTYCurrent();

    getRemainintQtyDisp = function () {

        var ret = null;
        if (self.CheckedOutMQTY > 0) {
            ret = FormatedCostQtyValues(self.CheckedOutMQTY - self.CheckedOutMQTYCurrent, 2);
        }
        else {
            ret = FormatedCostQtyValues(self.CheckedOutQTY - self.CheckedOutQTYCurrent, 2);
        }

        //return "<span style='background-color:cyan'>" + ret + "</span>";
        return ret;
    };

    self.RemainintQtyDisp = getRemainintQtyDisp();

    self.ForMaintananceDisp = self.CheckedOutMQTY > 0 ? "Yes" : "No";

    getTechnicianDisp = function () {
        var Technician = self.Technician;
        if (Technician == '' || Technician == null) {
            Technician = '';
        }
        var TechnicianCode = self.TechnicianCode;
        if (TechnicianCode == '' || TechnicianCode == null) {
            TechnicianCode = '';
        }
        if (Technician != '') {
            return "<span id='spanTechName'>" + TechnicianCode + " --- " + Technician + "</span><input type='hidden' value='" + self.TechnicianGuid + "' id='hdntechGuid'/>";
        }
        else {
            return "<span id='spanTechName'>" + TechnicianCode + "</span><input type='hidden' value='" + self.TechnicianGuid + "' id='hdntechGuid'/>";
        }
    };

    self.TechnicianDisp = getTechnicianDisp();

    self.CheckoutFromKitDisp = self.ToolDetailGUID != null ? "Yes" : "No";
    

}; //dto