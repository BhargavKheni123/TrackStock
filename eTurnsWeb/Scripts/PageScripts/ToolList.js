

var allowCheckinCheckOut = false;

var oTable;
var IsRefreshGrid = false;
var deleteURL = "/Assets/DeleteToolRecords";
var sImageUrl = "/Content/images/";
var anOpen = [];
var IsDeletePopupOpen = false;
var AllowDeletePopup = true;
var SelectedHistoryRecordID = 0;
var HistorySelected;
HasScheduleTab = true;
var objColumns = {};
var TopGlobalSearch = '';
//var AssetToolGridColumns = {};
var _ToolList = (function ($) {
    var self = {};
    var myDataTable = 'myDataTable';

    self.gridRowStartIndex = null;
    self.isSaveGridState = false;

    self.urls = {
        saveGridStateURL: null, loadGridStateUrl: null
        , toolListAjaxUrl: null, checkInCheckOutDataUrl : null
    };

    self.initUrls = function (saveGridStateURL, loadGridStateUrl
        , toolListAjaxUrl, checkInCheckOutDataUrl
    ) {
        self.urls.saveGridStateURL = saveGridStateURL;
        self.urls.loadGridStateUrl = loadGridStateUrl;       
        self.urls.toolListAjaxUrl = toolListAjaxUrl;
        self.urls.checkInCheckOutDataUrl = checkInCheckOutDataUrl;
    };

    self.init = function () {

        self.initDataTable();
        self.initEvents();
        
    };

    self.initDataTable = function () {
        var $myDataTable = $('#' + myDataTable);
        var ArrToolColumns = new Array();
        ArrToolColumns.push({ mDataProp: null, sClass: "read_only center NotHide RowNo", "bSortable": false, sDefaultContent: '' });
        ArrToolColumns.push({ mDataProp: "ID", sClass: "read_only" });
        ArrToolColumns.push({
            mDataProp: "imgExpand",//null,
            bSortable: false,
            sClass: "read_only control alignCenter",
            sDefaultContent: ''
            //,fnRender: function (obj, val) {
            //    var Qty = isNaN(parseInt(obj.aData.Quantity)) ? 0 : obj.aData.Quantity;
            //    var CheckedoutQty = isNaN(parseInt(obj.aData.CheckedOutQTY)) ? 0 : obj.aData.CheckedOutQTY;
            //    var CheckedOutMQTY = isNaN(parseInt(obj.aData.CheckedOutMQTY)) ? 0 : obj.aData.CheckedOutMQTY;
            //    var availQty = Qty - (parseInt(CheckedoutQty) + parseInt(CheckedOutMQTY));
            //    if (((Qty > availQty)) && !obj.aData.IsDeleted) {
            //        return '<img class="Expand" src="' + sImageUrl + 'drildown_open.jpg' + '">';
            //    }
            //    else {
            //        return '';
            //    }
            //}
        });
        ArrToolColumns.push({
            mDataProp: "btnCheckOut",//null,
            sClass: "read_only alignCenter",
            bSortable: false, sDefaultContent: '' 
            // fnRender: function (obj, val) {
            //    if (allowCheckinCheckOut) {

            //        var MCheckOutQTY = obj.aData.CheckedOutMQTY == null ? 0 : obj.aData.CheckedOutMQTY;
            //        var CheckOutQTY = obj.aData.CheckedOutQTY == null ? 0 : obj.aData.CheckedOutQTY;
            //        if ((parseInt(obj.aData.Quantity) - (parseInt(CheckOutQTY) + parseInt(MCheckOutQTY))) > 0 && !obj.aData.IsDeleted) {
            //            return "<span id='spnCheckOutStatus' style='display:none'>" + obj.aData.CheckOutStatus + "</span>" + "<span id='spnCheckedOutQTY' style='display:none'>" + obj.aData.CheckedOutQTY + "</span><span id='spnCheckedOutMQTY' style='display:none'>" + obj.aData.CheckedOutMQTY + "</span>" + "<span id='spnCheckInCheckOutID'  style='display:none'>" + obj.aData.CheckInCheckOutID + "</span>" + "<span id='spnQuantity'  style='display:none'>" + obj.aData.Quantity + "</span>" + "<span id='spnToolID'  style='display:none'>" + obj.aData.GUID + "</span>" + "<span id='spnToolPKID'  style='display:none'>" + obj.aData.ID + "</span>" + "<input type='button' value='Check Out' id='btnCheckOut' onclick='return PerformTCICO(this,2);' class='CreateBtn pull' />" + "<span id='spnToolType'  style='display:none'>" + obj.aData.Type + "</span><span id='spnToolIsBuildBreak' style='display:none'>" + obj.aData.IsBuildBreak + "</span>";
            //        }
            //        else {
            //            return "<span id='spnCheckOutStatus' style='display:none'>" + obj.aData.CheckOutStatus + "</span>" + "<span id='spnCheckedOutQTY' style='display:none'>" + obj.aData.CheckedOutQTY + "</span><span id='spnCheckedOutMQTY' style='display:none'>" + obj.aData.CheckedOutMQTY + "</span>" + "<span id='spnCheckInCheckOutID'  style='display:none'>" + obj.aData.CheckInCheckOutID + "</span>" + "<span id='spnQuantity'  style='display:none'>" + obj.aData.Quantity + "</span>" + "<span id='spnToolID'  style='display:none'>" + obj.aData.GUID + "</span>" + "<span id='spnToolPKID'  style='display:none'>" + obj.aData.ID + "</span>" + "<span id='spnToolType'  style='display:none'>" + obj.aData.Type + "</span><span id='spnToolIsBuildBreak' style='display:none'>" + obj.aData.IsBuildBreak + "</span>";
            //        }
            //    }
            //    else {
            //        return "<span id='spnCheckOutStatus' style='display:none'>" + obj.aData.CheckOutStatus + "</span>" + "<span id='spnCheckedOutQTY' style='display:none'>" + obj.aData.CheckedOutQTY + "</span><span id='spnCheckedOutMQTY' style='display:none'>" + obj.aData.CheckedOutMQTY + "</span>" + "<span id='spnCheckInCheckOutID'  style='display:none'>" + obj.aData.CheckInCheckOutID + "</span>" + "<span id='spnQuantity'  style='display:none'>" + obj.aData.Quantity + "</span>" + "<span id='spnToolID'  style='display:none'>" + obj.aData.GUID + "</span>" + "<span id='spnToolPKID'  style='display:none'>" + obj.aData.ID + "</span>" + "<span id='spnToolType'  style='display:none'>" + obj.aData.Type + "</span><span id='spnToolIsBuildBreak' style='display:none'>" + obj.aData.IsBuildBreak + "</span>";
            //    }
            //}
        });
        ArrToolColumns.push({
            mDataProp: "chkMaintance",//null,
            bSortable: false,
            sClass: "read_only alignCenter",
            sDefaultContent: ''
            //,fnRender: function (obj, val) {
            //    if (allowCheckinCheckOut) {
            //        var Qty = isNaN(parseInt(obj.aData.Quantity)) ? 0 : obj.aData.Quantity;
            //        var CheckedoutQty = isNaN(parseInt(obj.aData.CheckedOutQTY)) ? 0 : obj.aData.CheckedOutQTY;
            //        var CheckedOutMQTY = isNaN(parseInt(obj.aData.CheckedOutMQTY)) ? 0 : obj.aData.CheckedOutMQTY;

            //        if ((Qty - (parseInt(CheckedoutQty) + parseInt(CheckedOutMQTY))) > 0 && !obj.aData.IsDeleted) {
            //            return "<input type='checkbox' id='chkMaintance' name='manintancechk' value='maintanence' />";
            //        }
            //        else
            //            return "";
            //    }
            //}
        });
        ArrToolColumns.push({
            mDataProp: "txtQty",//null,
            bSortable: false,
            sClass: "read_only control center NotHide ",
            sDefaultContent: ''
            //,fnRender: function (obj, val) {

            //    var Qty = isNaN(parseInt(obj.aData.Quantity)) ? 0 : obj.aData.Quantity;
            //    var CheckedoutQty = isNaN(parseInt(obj.aData.CheckedOutQTY)) ? 0 : obj.aData.CheckedOutQTY;
            //    var CheckedOutMQTY = isNaN(parseInt(obj.aData.CheckedOutMQTY)) ? 0 : obj.aData.CheckedOutMQTY;
            //    if ((Qty - (CheckedoutQty + CheckedOutMQTY)) > 0) {
            //        if (obj.aData.IsGroupOfItems == 0) {
            //            return "<input type='text' value='" + 1 + "' class='numericinput numericalign' onkeypress='return false;'  id='txtQty' style='width:93%;disabled:true;' />";
            //        }
            //        else {
            //            return "<input type='text' value='1' class='numericinput numericalign' onkeypress='return onlyNumeric(event)'  id='txtQty' style='width:93%;' />";
            //        }
            //    }
            //    else
            //        return "";
            //}
        });
        ArrToolColumns.push({
            mDataProp: "Quantity",
            sClass: "read_only numericalign",
            sDefaultContent: ''
            ,fnRender: function (obj, val) {
              return FormatedCostQtyValues((obj.aData.Quantity), 2);
            }
        });
        ArrToolColumns.push({
            mDataProp: "AvailableQty",//null,
            bSortable: false,
            sClass: "read_only center numericalign",
            sDefaultContent: ''
            //,fnRender: function (obj, val) {

            //    //return FormatedCostQtyValues((obj.aData.Quantity - (obj.aData.CheckedOutQTY + obj.aData.CheckedOutMQTY)), 2);
            //    var Qty = parseFloat(obj.aData.Quantity);
            //    var CheckedOutQTY = parseFloat(obj.aData.CheckedOutQTY);
            //    var CheckedOutMQTY = parseFloat(obj.aData.CheckedOutMQTY);
            //    var ret = null;
            //    if (!isNaN(Qty) && !isNaN(CheckedOutQTY) && !isNaN(CheckedOutMQTY)) {
            //        ret = FormatedCostQtyValues((Qty - (CheckedOutQTY + CheckedOutMQTY)), 2);
            //    }
            //    else if (!isNaN(Qty) && !isNaN(CheckedOutQTY)) {
            //        ret = FormatedCostQtyValues((Qty - (CheckedOutQTY)), 2);
            //    }
            //    else if (!isNaN(Qty) && !isNaN(CheckedOutMQTY)) {
            //        ret = FormatedCostQtyValues((Qty - (CheckedOutMQTY)), 2);
            //    }
            //    else if (!isNaN(Qty)) {
            //        ret = FormatedCostQtyValues((Qty), 2);
            //    }
            //    else {
            //        ret = FormatedCostQtyValues(0, 2);
            //    }

            //    return "<span style='background-color:cyan'>" + ret + "</span>";

            //}
        });
        ArrToolColumns.push({
            mDataProp: "ToolName",//"ToolName",
            sClass: "read_only",
            sDefaultContent: '',
            bSortable: true,
            "bSearchable": false
            ,fnRender: function (obj, val) {
                return "<a onclick='return ShowEditTab(&quot;ToolEdit/" + obj.aData.ID + "&quot;,&quot;frmTool&quot;)' id='ToolName' href='JavaScript:void(0);'>" + obj.aData.ToolName + "</a>" + " <input type='hidden' id='hdnGUID' value='" + obj.aData.GUID.toString() + "' />" + "<span id='spnToolMasterID' style='display:none'>" + obj.aData.ID + "</span>";
            }
        });
        ArrToolColumns.push({
            mDataProp: "CheckedOutQTY",//"CheckedOutQTY"
            sClass: "read_only numericalign",
            sDefaultContent: ''
            ,fnRender: function (obj, val) {
                if (obj.aData.CheckedOutQTY != null) {
                    return FormatedCostQtyValues((obj.aData.CheckedOutQTY), 2);
                }
                else {
                    return FormatedCostQtyValues((0), 2);
                }
            }
        });
        ArrToolColumns.push({
            mDataProp: "CheckedOutMQTY",//"CheckedOutMQTY",
            sClass: "read_only numericalign",
            sDefaultContent: ''
            ,fnRender: function (obj, val) {
                if (obj.aData.CheckedOutMQTY != null) {
                    return FormatedCostQtyValues((obj.aData.CheckedOutMQTY), 2);
                }
                else {
                    return FormatedCostQtyValues((0), 2);
                }

            }
        });

        ArrToolColumns.push({ mDataProp: "Serial", sClass: "read_only" });
        ArrToolColumns.push({ mDataProp: "Description", sClass: "read_only" });
        ArrToolColumns.push({
            mDataProp: "IsGroupOfItems",//"IsGroupOfItems"
             sClass: "read_only"
            ,fnRender: function (obj, val) {
                if (val == 0)
                    return "No";
                else if (val == 1)
                    return "Yes";
                else
                    return "";
            }
        });

        ArrToolColumns.push({
            mDataProp:"Cost",   //"Cost",
            sClass: "read_only numericalign isCost"
            ,fnRender: function (obj, val) {
                //return GetDateInFullFormat(val);

                if (obj.aData.Cost != null && obj.aData.Cost != '') {
                    return FormatedCostQtyValues(obj.aData.Cost, 1);
                }
                else {
                    return '';
                }
            }
        });
        ArrToolColumns.push({ mDataProp: "ToolCategory", sClass: "read_only" });
        ArrToolColumns.push({ mDataProp: "Location", sClass: "read_only" });
        //ArrToolColumns.push({ mDataProp: "RoomName", sClass: "read_only" });
        ArrToolColumns.push({
            mDataProp: "Created", sClass: "read_only"
            //,fnRender: function (obj, val) {
            //    //return GetDateInFullFormat(val);
            //    return obj.aData.CreatedDate ;
            //}
        });
        ArrToolColumns.push({
            mDataProp: "Updated", sClass: "read_only"
            //,fnRender: function (obj, val) {
            //    //return GetDateInFullFormat(val);
            //    return obj.aData.UpdatedDate;
            //}
        });
        ArrToolColumns.push({ mDataProp: "UpdatedByName", sClass: "read_only" });
        ArrToolColumns.push({ mDataProp: "CreatedByName", sClass: "read_only" });
        ArrToolColumns.push({ mDataProp: "AddedFrom", sClass: "read_only" });
        ArrToolColumns.push({ mDataProp: "EditedFrom", sClass: "read_only" });
        ArrToolColumns.push({
            mDataProp: "ReceivedOn", sClass: "read_only"
            //,fnRender: function (obj, val) {
            //    return obj.aData.ReceivedOnDate;
            //}
        });
        ArrToolColumns.push({
            mDataProp: "ReceivedOnWeb", sClass: "read_only"
            //,fnRender: function (obj, val) {
            //    return obj.aData.ReceivedOnDateWeb;
            //}
        });
        ArrToolColumns.push({
            mDataProp: "TechnicianListDisp",//"TechnicianList",
            sClass: "read_only", bSortable: false
            //,fnRender: function (obj, val) {
            //    var strReturn = '<span style="position:relative"><input type="text" id="txtTechnician" class="text-boxinner AutoTechnician" style = "width:93%;" value="' + '@eTurns.DTO.Resources.ResCommon.SelectTechnicianText' + '" />';
            //    strReturn = strReturn + ' <input type="hidden" id="TechnicianGUID" value="" />';
            //    strReturn = strReturn + ' <a id="lnkShowAllOptions" href="javascript:void(0);" style="position:absolute; right:5px; top:0px;" class="ShowAllOptions" ><img src="/Content/images/arrow_down_black.png" alt="select" /></a></span>';
            //    return strReturn;

            //}
        });
        ArrToolColumns.push({
            "mDataProp": "ImagePath",
            Class: "read_only", "bVisible": false
            , "fnRender": function (obj, val) {

                if ((obj.aData.ImagePath != '' && obj.aData.ImagePath != null) || (obj.aData.ToolImageExternalURL != '' && obj.aData.ToolImageExternalURL != null)) {

                    if (obj.aData.ImagePath != '' && obj.aData.ImagePath != null) {

                        var path = logoPathImage;

                        return '<img style="cursor:pointer;"  alt="' + (obj.aData.ItemNumber) + '" id="ItemImageBox" width="120px" height="120px" src="' + path + "/" + obj.aData.ID + "/" + obj.aData.ImagePath + '">';
                    }
                    else if (obj.aData.ToolImageExternalURL != '' && obj.aData.ToolImageExternalURL != null) {
                        return '<img style="cursor:pointer;"  alt="' + (obj.aData.ItemNumber) + '" id="ItemImageBox" width="120px" height="120px" src="' + obj.aData.ToolImageExternalURL + '">';
                    }
                    else {
                        return "<img src='../Content/images/no-image.jpg' />";
                    }
                }
                else {
                    return "<img src='../Content/images/no-image.jpg' />";
                }
            }
        });
        ArrToolColumns.push({
            mDataProp: "txtCheckInQty",//null,
            bSortable: false,
            sClass: "read_only center",
            sDefaultContent: ''
            //, fnRender: function (obj, val) {

            //    var Qty = isNaN(parseInt(obj.aData.Quantity)) ? 0 : obj.aData.Quantity;
            //    var CheckedoutQty = isNaN(parseInt(obj.aData.CheckedOutQTY)) ? 0 : obj.aData.CheckedOutQTY;
            //    var CheckedOutMQTY = isNaN(parseInt(obj.aData.CheckedOutMQTY)) ? 0 : obj.aData.CheckedOutMQTY;
            //    var ret = null;
            //    //if ((Qty - (CheckedoutQty + CheckedOutMQTY)) < Qty) {
            //    if (CheckedoutQty > 0 || CheckedOutMQTY > 0) {
            //        if (obj.aData.IsGroupOfItems == 0) {
            //            ret = "<input type='text' value='" + 1 + "' class='numericinput numericalign' onkeypress='return false;'  id='txtCheckInQty' style='width:93%;disabled:true;display:none;' />";
            //        }
            //        else {

            //            if ((CheckedoutQty + CheckedOutMQTY) == 1 || obj.aData.CheckedOutQTYTotal == 1) {
            //                ret = " <input type='text' value='1' class='numericinput numericalign' onkeypress='return onlyNumeric(event)'  id='txtCheckInQty' style='width:93%;display:none;' />";
            //            }
            //            else {
            //                ret = " <input type='text' value='' class='numericinput numericalign' onkeypress='return onlyNumeric(event)'  id='txtCheckInQty' style='width:93%;display:none;' />";
            //            }
            //        }
            //    }
            //    else {
            //        ret = "";
            //    }

            //    return "<span style='background-color:cyan'>" + ret + "</span>";
            //}
        });
        ArrToolColumns.push({
            mDataProp: "ToolTypeName",//null,
            bSortable: false,
            sClass: "read_only center",
            sDefaultContent: ''
            //,fnRender: function (obj, val) {

            //    var ret = "";

            //    if (obj.aData.Type == 1) {
            //        ret = "Tool";
            //    }
            //    else {
            //        ret = "Kit Tool";
            //    }
            //    return ret;
            //}
        });

        //ArrToolColumns.push(arrToolMaster);
        $.each(arrToolMaster, function (index, val) {
            ArrToolColumns.push(val);
        });

        //ArrToolColumns.push(arrToolCheckInOutHistory);
        $.each(arrToolCheckInOutHistory, function (index, val) {
            ArrToolColumns.push(val);
        });
             

        $(document).ready(function () {

            $myDataTable.on('focus', "input.AutoTechnician", function (e) {
                //var ajaxURL = '/Assets/GetTechnician';
                var tr = $(this).parent().parent().parent();
                var itmGuid = $(tr).find('#spnItemGUID').text();
                var stagName = '';
                var ddl = $(this);

                _AutoCompleteWrapper.init(ddl, '/Assets/GetTechnician'
                    , function (request) {
                        return JSON.stringify({ 'NameStartWith': request.term });
                    }
                    , function (data) {
                        return $.map(data, function (Items) {
                            return {
                                label: Items.Value,
                                value: Items.Key,
                                id: Items.GUID
                            };
                        });
                    }
                    , function (curVal, selectedItem) {
                        var $TechnicianGUID = ddl.parent().find('#TechnicianGUID');
                        if ($.trim(selectedItem.value).length > 0) {

                            if (typeof $TechnicianGUID != 'undefined') {
                                $TechnicianGUID.val(selectedItem.id);
                            }
                        }
                        else {
                            $TechnicianGUID.val('');
                        }
                    }
                    , null,true
                );

            //    ddl.autocomplete({
            //        source: function (request, response) {
            //            //$('#DivLoading').show()
            //            $.ajax({
            //                url: '/Assets/GetTechnician',
            //                type: 'POST',
            //                data: JSON.stringify({ 'NameStartWith': request.term }),
            //                contentType: 'application/json',
            //                dataType: 'json',
            //                success: function (data) {
            //                    //$('#DivLoading').hide()
            //                    response($.map(data, function (Items) {
            //                        return {
            //                            label: Items.Value,
            //                            value: Items.Key,
            //                            id: Items.GUID
            //                        }
            //                    }));
            //                },
            //                error: function (err) {
            //                    //$('#DivLoading').hide();
            //                }
            //            });
            //        },
            //        autoFocus: false,
            //        minLength: 1,
            //        select: function (event, ui) {
            //            ddl.val(ui.item.value);
            //            if ($.trim(ui.item.value).length > 0) {

            //                if (typeof ddl.parent().find('#TechnicianGUID') != 'undefined') {
            //                    ddl.parent().find('#TechnicianGUID').val(ui.item.id);
            //                }
            //            }
            //            else {
            //                ddl.parent().find('#TechnicianGUID').val('');
            //            }
            //        },
            //        open: function () {
            //            ddl.removeClass("ui-corner-all").addClass("ui-corner-top");
            //        },
            //        close: function () {
            //            ddl.removeClass("ui-corner-top").addClass("ui-corner-all");
            //        },
            //        change: function (event, ui) {
            //            if (ui.item != null && ui.item != undefined && $.trim(ui.item.id).length > 0) {
            //                if (typeof ddl.parent().find('#TechnicianGUID') != 'undefined') {
            //                    ddl.parent().find('#TechnicianGUID').val(ui.item.id);
            //                }
            //            }
            //            else {
            //                ddl.parent().find('#TechnicianGUID').val('');
            //            }
            //        }
            //    });

            });

            objColumns = GetGridHeaderColumnsObject('myDataTable');
            masterGridColumns = objColumns;
            //AssetToolGridColumns = GetGridHeaderColumnsObject('myDataTable');
            LoadTabs();

            var gaiSelected = [];
            oTable = $myDataTable.dataTable({
                "bJQueryUI": true,
                "bScrollCollapse": true,
                "sScrollX": "100%",
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
                    if (typeof (ToolTechnicianValue) != undefined && typeof (ToolTechnicianValue) != "undefined") {
                        if (ToolTechnicianValue.length == 1) {
                            $nRow.find("input#txtCheckInQty").show();
                        }
                        else {
                            $nRow.find("input#txtCheckInQty").hide();
                        }
                    }
                    if (aData.IsDeleted == true && aData.IsArchived == true) {
                        // nRow.className = "GridDeleatedArchivedRow";
                        $nRow.css('background-color', '#B9BCBF');
                    }
                    else if (aData.IsDeleted == true) {
                        //nRow.className = "GridDeletedRow";
                        $nRow.css('background-color', '#FFCCCC');
                    }
                    else if (aData.IsArchived == true) {
                        // nRow.className = "GridArchivedRow";
                        $nRow.css('background-color', '#CCFFCC');
                    }
                    
                    if (self.gridRowStartIndex === null) {
                        self.gridRowStartIndex = this.fnSettings()._iDisplayStart;
                    }

                    $("td.RowNo:first", $nRow).html(self.gridRowStartIndex + iDisplayIndex + 1);
                    return nRow;
                },
                "fnStateSaveParams": self.grid_fnStateSaveParams, 
                "fnStateLoad": function (oSettings) {
                    var o;
                    $.ajax({
                        "url": self.urls.loadGridStateUrl,
                        "type": "POST",
                        data: { ListName: 'ToolList' },
                        "async": false,
                        cache: false,
                        "dataType": "json",
                        "success": function (json) {
                            if (json.jsonData != '')
                                o = JSON.parse(json.jsonData);
                        }
                    });
                    return o;
                },
                "bServerSide": true,
                "sAjaxSource": _ToolList.urls.toolListAjaxUrl,
                "fnServerData": self.grid_fnServerData, 
                "fnInitComplete": function () {
                    $('.ColVis').detach().appendTo(".setting-arrow");


                },
                "aoColumns": ArrToolColumns
            }).makeEditable();

            $('.DTTT_container').css('z-index', '-1');

            if (isCost == 'False') {

                //ColumnsToHideinPopUp.push(12);
                // ColumnsToHideinPopUp.push(13);

                HideColumnUsingClassName("myDataTable");
                // oTable.fnSetColumnVis(12, false);
                //oTable.fnSetColumnVis(13, false);


            }
            //   alert($('#IsDeletedRecords').is(':checked'));


            /*Functions used for nasted data binding START*/
            $myDataTable.on("click", "td.control", function (event) {

                if (!$(this).find("img").hasClass("Expand"))
                    return;

                var nTr = this.parentNode;
                var i = $.inArray(nTr, anOpen);

                if (i === -1) {

                    $('img', this).attr('src', sImageUrl + "drildown_close.jpg");
                    oTable.fnOpen(nTr, fnFormatDetails(oTable, nTr), '');
                    anOpen.push(nTr);
                }
                else {
                    $('img', this).attr('src', sImageUrl + "drildown_open.jpg");
                    oTable.fnClose(nTr);
                    anOpen.splice(i, 1);
                    //oTable.fnDraw(); // commented for WI-4229
                }
            });

            function fnFormatDetails(oTable, nTr) {
                var oData = oTable.fnGetData(nTr);
                var sOut = '';
                $('#DivLoading').show();
                $.ajax({
                    "url": _ToolList.urls.checkInCheckOutDataUrl,
                    data: { ToolGUID: oData.GUID },
                    "async": false,
                    cache: false,
                    "dataType": "text",
                    "success": function (json) {
                        sOut = json;
                        $('#DivLoading').hide();
                    },
                    error: function (response) {
                    }
                });

                return sOut;
            }
            /*Functions used for nasted data binding END*/
            var QueryStringParam1 = _Common.getParameterByName('fromdashboard');
            var QueryStringParam2 = _Common.getParameterByName('ToolGUID');
            if (QueryStringParam1 == 'yes' && QueryStringParam2 != '') {

                setTimeout(function () { ShowEditTab("ToolEdit/" + QueryStringParam2, "frmTool"); }, 4000);
            }

            $myDataTable.on('tap click', 'a[id^="ToolName"]', function () {
                var _search = _Common.getGlobalFilterVal(true, 'global_filter');
                TopGlobalSearch = '';
                
                if (_search != '' && _search != undefined) {
                    TopGlobalSearch = _search;
                }

                var tr = $(this).parent().parent();
                $myDataTable.find("tbody tr").removeClass("row_selected");
                $(tr).addClass('row_selected');
                $("#tabBuildBreak").show();
            });


        });
    };

    self.grid_fnServerData = function (sSource, aoData, fnCallback, oSettings) {

        var $myDataTable = $('#' + myDataTable);

        if (_Common.selectedGridOperation === _Common.gridOperations.Search
            || _Common.selectedGridOperation === _Common.gridOperations.IncludeDeleted
            || _Common.selectedGridOperation === _Common.gridOperations.IncludeArchived
            || _Common.selectedGridOperation === _Common.gridOperations.AutoRefresh
            || _Common.selectedGridOperation === _Common.gridOperations.PageChange
        ) {
            // prevent api calls
            //self.isGetReplinshRedCount = false;
            //self.isGetUDFEditableOptionsByUDF = false;
            self.isSaveGridState = false;
        }
        else if (_Common.selectedGridOperation === _Common.gridOperations.PageSizeChange) {
            //self.isGetReplinshRedCount = false;
            //self.isGetUDFEditableOptionsByUDF = false;
            self.isSaveGridState = true;
        }
        else if (_Common.selectedGridOperation === _Common.gridOperations.Sorting
            || _Common.selectedGridOperation === _Common.gridOperations.ColumnResize
        ) {
            //self.isGetReplinshRedCount = false;
            //self.isGetUDFEditableOptionsByUDF = false;
            self.isSaveGridState = true;
        }
        else if (_Common.selectedGridOperation === _Common.gridOperations.Refresh) {
            //self.isGetReplinshRedCount = true;
            //self.isGetUDFEditableOptionsByUDF = false;
            self.isSaveGridState = false;
        }


        self.gridRowStartIndex = null;
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
        if (oSettings.aaSorting.length != 0)
            aoData.push({ "name": "SortingField", "value": oSettings.aaSorting[0][3] });
        else
            aoData.push({ "name": "SortingField", "value": "0" });

        aoData.push({ "name": "IsArchived", "value": $('#IsArchivedRecords').is(':checked') });
        aoData.push({ "name": "IsDeleted", "value": $('#IsDeletedRecords').is(':checked') });
        aoData.push({ "name": "ToolType", "value": "1,2" });

        oSettings.jqXHR = _AjaxUtil.postJson(sSource, aoData
            , function (json) {

                var bindJson = [];

                $.each(json.aaData, function (index, obj) {
                    //bindJson.push(obj);
                    bindJson.push(new toolListDTO(obj));
                });

                json.aaData = bindJson;
                fnCallback(json);
            }
            , function () {
                $('.dataTables_scroll').css({ "opacity": 1 });
                $myDataTable.removeHighlight();
            }, true, false
            , function () {
                $myDataTable.removeHighlight();
                $('.dataTables_scroll').css({ "opacity": 0.2 });
            }
            , function () {
                $('.ShowAllOptions').click(function () {
                    var ddl = $(this).siblings('.AutoTechnician');
                    _AutoCompleteWrapper.searchHide(ddl);
                    //ddl.trigger("focus");
                    //ddl.autocomplete("search", " ");
                });
                $('.dataTables_scroll').css({ "opacity": 1 });
                if ($("#global_filter").val() != '') {
                    $myDataTable.highlight($("#global_filter").val());
                }
                $("input[type='radio']").filter('[value=ci]').attr('checked', 'checked');
                                

                UDFfillEditableOptionsForGrid();
            }
            , { "__RequestVerificationToken": $("input[name='__RequestVerificationToken'][type='hidden']").val() }
        );


        //oSettings.jqXHR = $.ajax({
        //    "dataType": 'json',
        //    "type": "POST",
        //    cache: false,
        //    "url": sSource,
        //    "data": aoData,
        //    "headers": { "__RequestVerificationToken": $("input[name='__RequestVerificationToken'][type='hidden']").val() },
        //    "success": fnCallback,
        //    beforeSend: function () {
        //        $myDataTable.removeHighlight();
        //        $('.dataTables_scroll').css({ "opacity": 0.2 });
        //    },
        //    complete: function () {
        //        $('.ShowAllOptions').click(function () {
        //            var ddl = $(this).siblings('.AutoTechnician');
        //            _AutoCompleteWrapper.searchHide(ddl);
        //            //ddl.trigger("focus");
        //            //ddl.autocomplete("search", " ");
        //        });
        //        $('.dataTables_scroll').css({ "opacity": 1 });
        //        if ($("#global_filter").val() != '') {
        //            $myDataTable.highlight($("#global_filter").val());
        //        }
        //        $("input[type='radio']").filter('[value=ci]').attr('checked', 'checked');

        //        //if ($("#global_filter").val().length > 0) {
        //        //    if ($myDataTable.dataTable().fnGetData().length <= 0) {
        //        //        //$('#cnfBarcodeAddmdl').modal();
        //        //    }
        //        //}

        //        UDFfillEditableOptionsForGrid();
        //    },
        //    error: function () {
        //        $('.dataTables_scroll').css({ "opacity": 1 });
        //        $myDataTable.removeHighlight();
        //    }
        //});
    };

    self.grid_fnStateSaveParams = function (oSettings, oData) {

        if (self.isSaveGridState) {
            oData.oSearch.sSearch = "";
            //if (PostCount > 1) {

            _AjaxUtil.postJson(self.urls.saveGridStateURL
                , { Data: JSON.stringify(oData), ListName: 'ToolList' }
                , function (json) {
                    o = json;
                }, null
                , true, false);

            //$.ajax({
            //    "url": self.urls.saveGridStateURL,
            //    "type": "POST",
            //    data: { Data: JSON.stringify(oData), ListName: 'ToolList' },
            //    "async": false,
            //    "dataType": "json",
            //    "success": function (json) {
            //        o = json;
            //    }
            //});

            //}
        }
        else {
            self.isSaveGridState = true;
        }
    };

    self.initEvents = function () {

        $(document).ready(function () {

            //$('body').on('focus', "input.UseThisAutoTechnician", function (e) {

                //var ajaxURL = '/Assets/GetTechnician';
            var ddl = $("input.UseThisAutoTechnician");
                var tr = $(this).parent().parent().parent();
                var itmGuid = $(tr).find('#spnItemGUID').text();
            var stagName = '';

            _AutoCompleteWrapper.init(ddl, '/Assets/GetTechnician'
                , function (request) {
                    return JSON.stringify({ 'NameStartWith': request.term });
                }
                , function (data) {
                    return $.map(data, function (Items) {
                        return {
                            label: Items.Value,
                            value: Items.Key,
                            id: Items.GUID
                        };
                    });
                }
                , function (curVal, selectedItem) {
                    
                    if ($.trim(selectedItem.value).length > 0) {
                        //if (ddl.parent().find('#UseThisTechnicianGUID') != undefined) {
                            $('#UseThisTechnicianGUID').val(selectedItem.id);
                        //}
                    }
                    else {
                        $('#UseThisTechnicianGUID').val('');
                    }
                }
                ,
                null
                ,true
                //function (selectedItem) {
                //    if (typeof selectedItem != 'undefined' && selectedItem != null && $.trim(selectedItem.id).length > 0) {
                //        //if (ddl.parent().find('#UseThisTechnicianGUID') != undefined) {
                //        $('#UseThisTechnicianGUID').val(selectedItem.id);
                //        //}
                //    }
                //    else {
                //        $('#UseThisTechnicianGUID').val('');
                //    }
                //}
            );
            
            //ddl.autocomplete({
            //    source: function (request, response) {
            //        //$('#DivLoading').show()
            //        $.ajax({
            //            url: '/Assets/GetTechnician',
            //            type: 'POST',
            //            data: JSON.stringify({ 'NameStartWith': request.term }),
            //            contentType: 'application/json',
            //            dataType: 'json',
            //            success: function (data) {
            //                //$('#DivLoading').hide()
            //                response($.map(data, function (Items) {
            //                    return {
            //                        label: Items.Value,
            //                        value: Items.Key,
            //                        id: Items.GUID
            //                    }
            //                }));
            //            },
            //            error: function (err) {
            //                //$('#DivLoading').hide();
            //            }
            //        });
            //    },
            //    autoFocus: false,
            //    minLength: 1,
            //    select: function (event, ui) {

            //        //$(this).val(ui.item.value);
            //        ddl.val(ui.item.value);

            //        if ($.trim(ui.item.value).length > 0) {

            //            //if ($(this).parent().find('#UseThisTechnicianGUID') != undefined) {
            //            //    $(this).parent().find('#UseThisTechnicianGUID').val(ui.item.id);
            //            //}
            //            if (ddl.parent().find('#UseThisTechnicianGUID') != undefined) {
            //                ddl.parent().find('#UseThisTechnicianGUID').val(ui.item.id);
            //            }
            //        }
            //        else {
            //            //$(this).parent().find('#UseThisTechnicianGUID').val('');
            //            ddl.parent().find('#UseThisTechnicianGUID').val('');
            //        }
            //    },
            //    open: function () {
            //        ddl.removeClass("ui-corner-all").addClass("ui-corner-top");
            //    },
            //    close: function () {
            //        ddl.removeClass("ui-corner-top").addClass("ui-corner-all");
            //    },
            //    change: function (event, ui) {
            //        if (ui.item != null && ui.item != undefined && $.trim(ui.item.id).length > 0) {
            //            if (ddl.parent().find('#UseThisTechnicianGUID') != undefined) {
            //                ddl.parent().find('#UseThisTechnicianGUID').val(ui.item.id);
            //            }
            //        }
            //        else {
            //            ddl.parent().find('#UseThisTechnicianGUID').val('');
            //        }
            //    }
            //});

            //});
            $('.UseThisShowAllOptions').click(function () {
                var ddl = $(this).siblings('.UseThisAutoTechnician');

                //ddl.trigger("focus");
                //ddl.autocomplete("search", " ");
                _AutoCompleteWrapper.searchHide(ddl);
            });

            

            
            var QueryStringParam1 = _Common.getParameterByName('fromdashboard');
            var QueryStringParam2 = _Common.getParameterByName('ToolGUID');

                if (QueryStringParam1 == 'yes' && QueryStringParam2 != '') {

                    $('#tab5').removeClass('selected');
                    $('#tabMaintenance').addClass('selected');
                    CurrentListTabID = 'tabMaintenance';
                    $("#" + CurrentListTabID).click();
                }
                $("#DivPullSelection").dialog({
                    autoOpen: false,
                    show: "blind",
                    hide: "explode",
                    height: 700,
                    title: "Written Off Tool",
                    width: 900,
                    modal: true,
                    open: function () {
                    },
                    beforeClose: function () {
                    },
                    close: function () {
                        //$('.ui-widget-overlay').css('position', 'absolute');
                        IsRefreshGrid = true;
                        $('#DivLoading').hide();
                        $("#DivPullSelection").empty();

                        $('#myDataTable').dataTable().fnStandingRedraw();

                    }
                });
            
                        
            if (window.location.hash.toLowerCase() == "#history") {
                $("#tabToolHistory").click();
            }

        }); // ready

    };


    return self;

})(jQuery);

/* HISTORY related data deleated and archived START */
//function GenerateColumnSortable() {
//    //clear the old elements     
//    var blankNUmber = 0;
//    $('#ColumnSortable li').each(function (index) {
//        $(this).remove();
//    });
//    var div = document.createElement('div');
//    div.id = "CheckAll";
//    //li.className = 'ui-state-default';
//    //    li.innerHTML = oColumn.sTitle.trim();
//    div.innerHTML = "<input type='hidden' value ='ColumnSortable' id='hiddenDivName' /><a href='javascript:;' onclick='CheckAll(this);'  id='CheckAll' >CheckAll</a>&nbsp;<a href='javascript:;' onclick='UnCheckAll(this);' id='UnCheckAll' >UnCheckAll</a>" +
//                    "&nbsp;&nbsp;&nbsp;<input type='checkbox' onclick='ShowAllHidden(this);' id='ShowAllHidden' checked='checked' />Show All Hidden";
//    $('div.sortableContainer').find("div#CheckAll").remove();
//    $('div.sortableContainer').prepend(div);
//    //get current columns order and generate li sortable accordingly

//    var oTableReorder = null;

//    if ((typeof oTable) === 'undefined' && (typeof oTableReorderId) === 'undefined')
//        return false;

//    if ((typeof oTable) === 'undefined' && oTableReorderId != null && oTableReorderId != undefined)
//        oTableReorder = $('#' + oTableReorderId).dataTable();
//    else
//        oTableReorder = oTable;

//    for (i = 0, iLen = oTableReorder.fnSettings().aoColumns.length; i < iLen; i++) {

//        var oColumn = oTableReorder.fnSettings().aoColumns[i];
//        var style = '';
//        var LiStyle = 'display:block';
//        var LiClassName = 'ui-state-default'
//        if (oColumn.sClass.indexOf('NotHide') >= 0) {
//            //style = ' style="display:none" ';
//            style = ' disabled="disabled" ';
//            //LiStyle = 'display:none';
//            LiClassName = LiClassName + ' HiddenLi';
//        }

//        var colindxbyname = '';
//        if (oColumn.sTitle.trim() != '') {
//            colindxbyname = GetAssetToolColumnIndex(oColumn.sTitle.trim());
//        }
//        else {
//            colindxbyname = GetAssetToolColumnIndex('blankFieldName' + blankNUmber);
//        }


//        if (gblColumnsToHideinPopUp == 'True') {

//            var li = document.createElement('li');
//            li.id = colindxbyname;
//            li.className = LiClassName;
//            li.innerHTML = oColumn.sTitle.trim();
//            li.style = LiStyle;
//            if (oColumn.bVisible || oColumn.sClass.indexOf('NotHide') >= 0) {
//                if (oColumn.sTitle.trim() != '') {
//                    li.innerHTML = '<input type="checkbox" ' + style + ' class="checkBox" checked="checked" id="' + GetAssetToolColumnIndex(oColumn.sTitle.trim()) + '_" />' + oColumn.sTitle.trim();
//                }
//                else {
//                    li.innerHTML = '<input type="checkbox" ' + style + ' class="checkBox" checked="checked" id="' + GetAssetToolColumnIndex('blankFieldName' + blankNUmber) + '_" />' + oColumn.sTitle.trim();
//                    blankNUmber++;
//                }
//            }
//            else {

//                if (oColumn.sTitle.trim() != '') {
//                    li.innerHTML = '<input type="checkbox" class="checkBox" id="' + GetAssetToolColumnIndex(oColumn.sTitle.trim()) + '_" />' + oColumn.sTitle.trim();
//                }
//                else {

//                    li.innerHTML = '<input type="checkbox" class="checkBox" id="' + GetAssetToolColumnIndex('blankFieldName' + blankNUmber) + '_" />' + oColumn.sTitle.trim();
//                    blankNUmber++;
//                }
//            }
//            $('#ColumnSortable').append(li);
//        }
//        else {

//            if (jQuery.inArray(colindxbyname, ColumnsToHideinPopUp) < 0) {

//                var li = document.createElement('li');
//                li.id = colindxbyname;
//                li.className = LiClassName;
//                li.innerHTML = oColumn.sTitle.trim();
//                li.style = LiStyle;
//                if (oColumn.bVisible || oColumn.sClass.indexOf('NotHide') >= 0) {
//                    if (oColumn.sTitle.trim() != '') {
//                        li.innerHTML = '<input type="checkbox" ' + style + ' class="checkBox" checked="checked" id="' + GetAssetToolColumnIndex(oColumn.sTitle.trim()) + '_" />' + oColumn.sTitle.trim();
//                    }
//                    else {
//                        li.innerHTML = '<input type="checkbox" ' + style + ' class="checkBox" checked="checked" id="' + GetAssetToolColumnIndex('blankFieldName' + blankNUmber) + '_" />' + oColumn.sTitle.trim();
//                    }
//                }

//                else {
//                    if (oColumn.sTitle.trim() != '') {
//                        li.innerHTML = '<input type="checkbox" ' + style + ' class="checkBox" id="' + GetAssetToolColumnIndex(oColumn.sTitle.trim()) + '_" />' + oColumn.sTitle.trim();
//                    }
//                    else {
//                        li.innerHTML = '<input type="checkbox" ' + style + ' class="checkBox" id="' + GetAssetToolColumnIndex('blankFieldName' + blankNUmber) + '_" />' + oColumn.sTitle.trim();
//                    }
//                }

//                $('#ColumnSortable').append(li);
//            }
//            else {
//                var li = document.createElement('li');
//                li.id = colindxbyname;
//                li.className = LiClassName;
//                li.innerHTML = oColumn.sTitle.trim();
//                if (oColumn.sTitle.trim() != '') {
//                    li.innerHTML = '<input type="checkbox" disabled="disabled" class="checkBox" id="' + GetAssetToolColumnIndex(oColumn.sTitle.trim()) + '_" />' + oColumn.sTitle.trim();
//                }
//                else {
//                    li.innerHTML = '<input type="checkbox" disabled="disabled" class="checkBox" id="' + GetAssetToolColumnIndex('blankFieldName' + blankNUmber) + '_" />' + oColumn.sTitle.trim();
//                }
//                $(li).hide();
//                $('#ColumnSortable').append(li);
//            }
//        }

//    }

//}
//var GetAssetToolColumnIndex = function (ColumnName) {
//    if (typeof (AssetToolGridColumns) != "undefined") {
//        return AssetToolGridColumns[ColumnName];
//    }
//};
function fnGetSelected(oTableLocal) {
    return oTableLocal.$('tr.row_selected');
}
function HistoryTabClick() {
    GetHistoryData();
}
function GetHistoryData() {
    HistorySelected = fnGetSelected(oTable);
    if (HistorySelected != undefined && HistorySelected.length == 1) {
        //SelectedHistoryRecordID = HistorySelected[0].id;
        var ToolID = $(HistorySelected).find('#spnToolMasterID').text();
        SelectedHistoryRecordID = ToolID;
        $('#DivLoading').show();
        $('#CTab').hide();
        $('#CtabCL').show();
        $('#CtabCL').load('/Master/ToolHistory', function () { $('#DivLoading').hide(); });
    }
    else {
        $('#CtabCL').html('');
        $("#spanGlobalMessage").html(msgSelectForViewHistory);
        //$('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        showNotificationDialog();
        return false;
    }
}

function GetSchedulerData() {
    var HistorySelectedCL = fnGetSelected(oTable);
    if (HistorySelectedCL != undefined && HistorySelectedCL.length == 1) {
        var hdnguid = $(HistorySelectedCL[0]).find('#hdnGUID')[0].value;
        $('#DivLoading').show();
        $("#CTab").hide();
        $("#CtabSCH").empty();
        $("#CtabSCH").show();
        $('#CtabSCH').load('/Assets/ScheduleMappingCreate?ToolGUID=' + hdnguid, function () { $('#DivLoading').hide(); });
        // $('#tabScheduleList').click();
    }
    else {
        $('#CtabSCH').html('');
        $("#spanGlobalMessage").html(msgSelectForViewHistory);
        //$('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        showNotificationDialog();
        return false;
    }
}

function GetSchedulerListData() {
    var HistorySelectedCL = fnGetSelected(oTable);
    if (HistorySelectedCL != undefined && HistorySelectedCL.length == 1) {
        var hdnguid = $(HistorySelectedCL[0]).find('#hdnGUID')[0].value;
        $('#DivLoading').show();
        $("#CTab").hide();
        $("#CtabSchedulerList").empty();
        $("#CtabSchedulerList").show();
        $('#CtabSchedulerList').load('/Assets/LoadScheduleList?ToolGUID=' + hdnguid, function () {
            $('#DivLoading').hide();
        });
    }
    else {
        $('#CtabSchedulerList').html('');
        $("#spanGlobalMessage").html(MsgSelectRecordForScheduleList);
        //$('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        showNotificationDialog();
        return false;
    }
}

function GetOdometerData() {
    var HistorySelectedCL = fnGetSelected(oTable);
    if (HistorySelectedCL != undefined && HistorySelectedCL.length == 1) {
        var hdnguid = $(HistorySelectedCL[0]).find('#hdnGUID')[0].value;
        $('#DivLoading').show();
        $("#CTab").hide();
        $("#CtabOdometer").empty();
        $("#CtabOdometer").show();
        $('#CtabOdometer').load('/Assets/OdometerCreate?ToolGUID=' + hdnguid, function () { $('#DivLoading').hide(); });
    }
    else {
        $('#CtabOdometer').html('');
        $("#spanGlobalMessage").html(msgSelectForViewHistory);
        //$('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        showNotificationDialog();
        return false;
    }
}

function GetOdometerListData() {
    var HistorySelectedCL = fnGetSelected(oTable);
    if (HistorySelectedCL != undefined && HistorySelectedCL.length == 1) {
        var hdnguid = $(HistorySelectedCL[0]).find('#hdnGUID')[0].value;
        $('#DivLoading').show();
        $("#CTab").hide();
        $("#CtabOdometerList").empty();
        $("#CtabOdometerList").show();
        $('#CtabOdometerList').load('/Assets/LoadOdometerList?ToolGUID=' + hdnguid, function () { $('#DivLoading').hide(); });
    }
    else {
        $('#CtabOdometerList').html('');
        $("#spanGlobalMessage").html(msgSelectForViewHistory);
        //$('div#target').fadeToggle();
        //$("div#target").delay(2000).fadeOut(200);
        showNotificationDialog();
        return false;
    }
}

function GetMainenanceData() {
    var QueryStringParam1 = _Common.getParameterByName('fromdashboard');
    var QueryStringParam2 = _Common.getParameterByName('ToolGUID');
    if (QueryStringParam1 == 'yes' && QueryStringParam2 != '') {
        var hdnguid = QueryStringParam2;
        $('#DivLoading').show();
        $("#CTab").hide();
        $("#CtabMaintenance").empty();
        $("#CtabMaintenance").show();
        $('#CtabMaintenance').load('/Assets/LoadMaintenanceList?ToolGUID=' + hdnguid, function () { $('#DivLoading').hide(); });
    }
    else {
        var HistorySelectedCL = fnGetSelected(oTable);
        if (HistorySelectedCL != undefined && HistorySelectedCL.length == 1) {
            var hdnguid = $(HistorySelectedCL[0]).find('#hdnGUID')[0].value;
            $('#DivLoading').show();
            $("#CTab").hide();
            $("#CtabMaintenance").empty();
            $("#CtabMaintenance").show();
            $('#CtabMaintenance').load('/Assets/LoadMaintenanceList?ToolGUID=' + hdnguid, function () { $('#DivLoading').hide(); });
        }
        else {
            $('#CtabMaintenance').html('');
            $("#spanGlobalMessage").html(msgSelectForViewHistory);
            //$('div#target').fadeToggle();
            //$("div#target").delay(2000).fadeOut(200);
            showNotificationDialog();
            return false;
        }
    }
}

function ShowScheduleEditTab(action, formName) {
    var IsArchived = $('#IsArchivedRecords').is(':checked');
    var IsDeleted = $('#IsDeletedRecords').is(':checked');
    action += '&IsArchived=' + IsArchived + '&IsDeleted=' + IsDeleted;
    IsEditMode = true;
    IsShowHistory = true;
    AllowDeletePopup = false;
    $('#DivLoading').show();
    $('#tabSchedule').show();
    $('#tabSchedule').click();
    $(formName).append($('#CtabSCH').load(action, function () { $('#DivLoading').hide(); $("#" + formName + " :input:text:visible:first").focus(); }));
    $.validator.unobtrusive.parseDynamicContent('#' + formName + ' input:last');
}
/* HISTORY related data deleated and archived END */
/* Cost Narrow Search Related Code  START */
function CostNarroSearch(CostDDLObject) {
    //oTable.fnFilter($(CostDDLObject).val(),null,null,null);
    if ($(CostDDLObject).val() != "0_-1") {
        ToolCostValue = $(CostDDLObject).val();
        _NarrowSearchSave.objSearch.ToolsCost = ToolCostValue;
        _NarrowSearchSave.objSearch.ToolHistoryCost = ToolCostValue;
        DoNarrowSearch();
    }
    else {
        ToolCostValue = '';
        _NarrowSearchSave.objSearch.ToolsCost = ToolCostValue;
        _NarrowSearchSave.objSearch.ToolHistoryCost = ToolCostValue;
        DoNarrowSearch();
    }
}

function HistoryCostNarroSearch(CostDDLObject) {
    //oTable.fnFilter($(CostDDLObject).val(),null,null,null);
    if ($(CostDDLObject).val() != "0_-1") {
        THCostNarroSearchValue = $(CostDDLObject).val();
        _NarrowSearchSave.objSearch.ToolHistoryCost = THCostNarroSearchValue;
        DoNarrowSearchSC();
    }
    else {
        THCostNarroSearchValue = '';
        _NarrowSearchSave.objSearch.ToolHistoryCost = THCostNarroSearchValue;
        DoNarrowSearchSC();
    }
}

function SSNarroSearch(SSDDLObject) {
    //oTable.fnFilter($(CostDDLObject).val(),null,null,null);

    if ($(SSDDLObject).val() != "0") {
        ToolStatusValue = $(SSDDLObject).val();
        DoNarrowSearch();
    }
    else {
        ToolStatusValue = '';
        DoNarrowSearch();
    }
}

/* Cost Narrow Search Related Code  END */

function GetToolListHistory() {
    //alert("In GetToolListHistory");

    ToolListTab = 'ToolHistoryList';
    $("#CtabToolListHistory").show();
    $('#DivLoading').show();
    $.get('/Assets/LoadToolHistoryList',
        function (data) {
            $("#CtabToolListHistory").html(data);
            $('#DivLoading').hide();
        });
}

function GetWrittenOffToolList() {
    $('#DivLoading').show();
    $("#CTab").hide();
    //$("#CtabWrittenOffTool").html('');
    $("#CtabWrittenOffTool").empty().show();
    //$("#tabWrittenOffTool").removeClass("unselected").addClass("selected");
    //$("#CtabWrittenOffTool").show();
    $.get('/Assets/LoadWrittenOffToolList',
        function (data) {
            $("#CtabWrittenOffTool").html(data);
            $('#DivLoading').hide();
        });
}

function callbacknew() { window.location.hash = '#new'; ShowNewTab('ToolCreate', 'frmTool'); }
function callbackhistory() { window.location.hash = '#list'; ToolListTab = 'ToolList'; }
function callbackCL() { window.location.hash = ''; HistoryTabClick(); }
function callbackScheduler() { window.location.hash = ''; GetSchedulerData(); }
function callbackSchedulerList() { window.location.hash = ''; GetSchedulerListData(); }
function callbackOdometer() { window.location.hash = ''; GetOdometerData(); }
function callbackOdometerList() { window.location.hash = ''; GetOdometerListData(); }
function callbackMaintenance() { window.location.hash = ''; GetMainenanceData(); }
function callbacktoolhistory() { window.location.hash = '#history'; ToolListTab = 'ToolHistoryList'; $("#CtabToolHistoryList").html(''); GetToolListHistory(); }
function callbackWrittenOffTool() { window.location.hash = '#writtenoff'; $("#CtabWrittenOffTool").html(''); GetWrittenOffToolList(); }
function callKitToolBuildBreak() { window.location.hash = ''; GetBuildBreak(); }

function GetBuildBreak() {

    HistorySelected = fnGetSelected(oTable);
    if (HistorySelected != undefined && HistorySelected.length == 1) {
        var ToolGUID = $(HistorySelected).find('#spnToolID').html();
        var ToolType = $(HistorySelected).find('#spnToolType').html();
        var ToolIsBuildBreak = $(HistorySelected).find('#spnToolIsBuildBreak').html();
        if (ToolType == "2") {
            if (ToolIsBuildBreak == "true") {
                SelectedHistoryRecordID = ToolGUID;
                $('#DivLoading').show();
                $("#CTab").hide();
                $("#CtabCL").show();
                //$('#CtabCL').load('/Master/ItemHistory', function () { $('#DivLoading').hide(); });
                $('#CtabCL').load('/Kit/ToolKitBuildBreak?KitToolGUID=' + ToolGUID + '', function () { $('#DivLoading').hide(); });
            }
            else {
                $('#CtabCL').html('');
                $("#spanGlobalMessage").html(ReqKitisBuildBreak);
                $('div#target').fadeToggle();
                $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                $("div#tab5").click();
                return false;
            }
        }
        else {
            $('#CtabCL').html('');
            $("#spanGlobalMessage").html(MsgSelectToolKitTypeOnly);
            $('div#target').fadeToggle();
            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
            $("div#tab5").click();
            return false;
        }
    }
    else {
        $('#CtabCL').html('');
        $("#spanGlobalMessage").html(msgSelectForViewHistory);
        $('div#target').fadeToggle();
        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
        $("div#tab5").click();
        return false;
    }
}

//$(document).ready(function (e) {
//    //if (window.location.hash.toLowerCase() == "#history") {
//    //    // alert('hi 1');
//    //    $("#tabToolHistory").click();
//    //}
//});

function ResetNarrowSearchTechnicianList() {
    
    var selectedToolTechnician = ToolTechnicianValue;//$("#ToolsTechnician").val(); //$("#ToolsTechnician").multiselect("getChecked");
    var wasTechnicianSelected = (typeof (selectedToolTechnician) != undefined && selectedToolTechnician !== undefined && selectedToolTechnician != null && selectedToolTechnician != '' && selectedToolTechnician.length > 0)
                                ? true : false;
    $("#ToolsTechnicianCollapse").html('');   
    $("#ToolsTechnician").empty();
    ToolTechnicianValue = '';

    if ($("#toolstechnician option").length == 0) {
        $.ajax({
            'url': '/master/getnarrowdddata',
            data: { tablename: 'ToolMaster', textfieldname: 'ToolsTechnician', isarchived: false, isdeleted: false, requisitioncurrenttab: "ToolList" },
            success: function (response) {
                var s = '';
                $.each(response.DDData, function (valdata, valcount) {
                    var arrdata = valdata.toString().split('[###]');
                    s += '<option value="' + arrdata[1] + '">' + arrdata[0] + ' (' + valcount + ')' + '</option>';
                });
                $("#ToolsTechnician").empty();
                $("#ToolsTechnician").multiselect('destroy');
                $("#ToolsTechnician").multiselectfilter('destroy');
                $("#ToolsTechnician").append(s);

                if (typeof (wasTechnicianSelected) != undefined && wasTechnicianSelected !== undefined && wasTechnicianSelected == true) {
                    for (var i = 0; i < selectedToolTechnician.length; i++)
                    {
                            $("#ToolsTechnician").multiselect("widget").find(":checkbox[value='" + selectedToolTechnician[i] + "']").attr("checked", "checked");
                            $("#ToolsTechnician option[value='" + selectedToolTechnician[i] + "']").attr("selected", 1);
                            var text = $("#ToolsTechnician option[value='" + selectedToolTechnician[i] + "']").text()
                            if ($("#ToolsTechnicianCollapse").text().indexOf(text) == -1) {
                                $("#ToolsTechnicianCollapse").append("<span>" + text + "</span>");
                            }
                    }
                    //$("select").multiselect('reload');
                    $("#ToolsTechnician").multiselect('reload');
                    //$("#ToolsTechnician").multiselect("refresh");                    
                }

                $("#ToolsTechnician").multiselect(
                            {
                                noneSelectedText: TechnicianList, selectedList: 5,
                                selectedText: function (numChecked, numTotal, checkedItems) {
                                    return TechnicianList + ': ' + numChecked + ' ' + selected;
                                }
                            },
                            {
                                checkAll: function (ui) {
                                    $("#ToolsTechnicianCollapse").html('');
                                    for (var i = 0; i <= ui.target.length - 1; i++) {
                                        if ($("#ToolsTechnicianCollapse").text().indexOf(ui.target[i].text) == -1) {
                                            $("#ToolsTechnicianCollapse").append("<span>" + ui.target[i].text + "</span>");
                                        }
                                    }
                                    $("#ToolsTechnicianCollapse").show();
                                }
                            }
                )
                .unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                    if (ui.checked) {
                        if ($("#ToolsTechnicianCollapse").text().indexOf(ui.text) == -1) {
                            $("#ToolsTechnicianCollapse").append("<span>" + ui.text + "</span>");
                        }
                    }
                    else {
                        if (ui.checked == undefined) {
                            $("#ToolsTechnicianCollapse").html('');
                        }
                        else if (!ui.checked) {
                            var text = $("#ToolsTechnicianCollapse").html();
                            text = text.replace("<span>" + ui.text + "</span>", '');
                            $("#ToolsTechnicianCollapse").html(text);
                        }
                        else
                            $("#ToolsTechnicianCollapse").html('');
                    }
                    ToolTechnicianValue = $.map($(this).multiselect("getChecked"), function (input) {
                        return input.value;
                    })

                    _NarrowSearchSave.objSearch.ToolsTechnician = ToolTechnicianValue;

                    if ($("#ToolsTechnicianCollapse").text().trim() != '')
                        $("#ToolsTechnicianCollapse").show();
                    else
                        $("#ToolsTechnicianCollapse").hide();


                    if ($("#ToolsTechnicianCollapse").find('span').length <= 2) {
                        $("#ToolsTechnicianCollapse").scrollTop(0).height(50);
                    }
                    else {
                        $("#ToolsTechnicianCollapse").scrollTop(0).height(100);
                    }
                    clearGlobalIfNotInFocus();

                    DoNarrowSearch();
                }).multiselectfilter();

                _NarrowSearchSave.setControlValue("ToolsTechnician");

                //if ($("select#ToolsTechnician").parent("li").find("div#ToolsTechnicianCollapse:visible").length != 0) {

                //    $("select#ToolsTechnician").parent("li").find(".downarrow").click();
                //    $("select#ToolsTechnician").parent("li").find("ToolsTechnicianCollapse").attr("style", "display: none; height: 100px; overflow: hidden;");
                //}
            },
            error: function (response) {
                // through errror message
            },
            complete: function () {
                ToolTechnicianValue = $.map($("#ToolsTechnician").multiselect("getChecked"), function (input) { return input.value; })
                if (ToolTechnicianValue.length == 1) {
                    $("input#btnToolCheckInAllBottom").show();
                }
                else {
                    $("input#btnToolCheckInAllBottom").hide();
                }
                DoNarrowSearch();
            }
        })
    }
            ////DoNarrowSearch();
    //$("input#NarroSearchClear").click(); //DoNarrowSearch();
    //DoNarrowSearch();
    var selectedToolCheckout = MaintenanceValue;//$("#ToolCheckout").val();
    var wasToolCheckoutSelected = (typeof (selectedToolCheckout) != undefined && selectedToolCheckout !== undefined && selectedToolCheckout != null && selectedToolCheckout != '' && selectedToolCheckout.length > 0)
                                ? true : false;
    MaintenanceValue = '';
    $.ajax({
        'url': '/Master/GetNarrowDDData',
        data: { TableName: 'ToolMaster', TextFieldName: 'ToolMaintenance', IsArchived: false, IsDeleted: false, "RequisitionCurrentTab": "ToolList" },
        success: function (response) {
            var s = '';
            $.each(response.DDData, function (ValData, ValCount) {
                var ArrData = ValData.toString().split('###');
                s += '<option value="' + ArrData[1] + '">' + ArrData[0] + ' (' + ValCount + ')' + '</option>';
            });

            //Destroy widgets before reapplying the filter
            $("#ToolCheckoutCollapse").html('');
            $("#ToolCheckout").empty();
            $("#ToolCheckout").multiselect('destroy');
            $("#ToolCheckout").multiselectfilter('destroy');
            $("#ToolCheckout").append(s);

            if (typeof (wasToolCheckoutSelected) != undefined && wasToolCheckoutSelected !== undefined && wasToolCheckoutSelected == true) {
                for (var i = 0; i < selectedToolCheckout.length; i++) {
                    $("#ToolCheckout").multiselect("widget").find(":checkbox[value='" + selectedToolCheckout[i] + "']").attr("checked", "checked");
                    $("#ToolCheckout option[value='" + selectedToolCheckout[i] + "']").attr("selected", 1);
                    var text = $("#ToolCheckout option[value='" + selectedToolCheckout[i] + "']").text()
                    if ($("#ToolCheckoutCollapse").text().indexOf(text) == -1) {
                        $("#ToolCheckoutCollapse").append("<span>" + text + "</span>");
                    }
                    //$("#ToolsTechnician").multiselect("refresh");
                }
                //$("select").multiselect('reload');
                $("#ToolCheckout").multiselect('reload');
                //$("#ToolsTechnician").multiselect("refresh");
            }

            $("#ToolCheckout").multiselect(
                {
                    noneSelectedText: ToolsMaintenance, selectedList: 5,
                    selectedText: function (numChecked, numTotal, checkedItems) {
                        return ToolsMaintenance + ': ' + numChecked + ' ' + selected;
                    }
                },
                {
                    checkAll: function (ui) {
                        $("#ToolCheckoutCollapse").html('');
                        for (var i = 0; i <= ui.target.length - 1; i++) {
                            if ($("#ToolCheckoutCollapse").text().indexOf(ui.target[i].text) == -1) {
                                $("#ToolCheckoutCollapse").append("<span>" + ui.target[i].text + "</span>");
                            }
                        }
                        $("#ToolCheckoutCollapse").show();
                    }
                }
            )
                .unbind("multiselectclick multiselectcheckall multiselectuncheckall")
                .bind("multiselectclick multiselectcheckall multiselectuncheckall", function (event, ui) {
                    if (ui.checked) {
                        if ($("#ToolCheckoutCollapse").text().indexOf(ui.text) == -1) {
                            $("#ToolCheckoutCollapse").append("<span>" + ui.text + "</span>");
                        }
                    }
                    else {
                        if (ui.checked == undefined) {
                            $("#ToolCheckoutCollapse").html('');
                        }
                        else if (!ui.checked) {
                            var text = $("#ToolCheckoutCollapse").html();
                            text = text.replace("<span>" + ui.text + "</span>", '');
                            $("#ToolCheckoutCollapse").html(text);
                        }
                        else
                            $("#ToolCheckoutCollapse").html('');
                    }
                    MaintenanceValue = $.map($(this).multiselect("getChecked"), function (input) {
                        return input.value;
                    })

                    _NarrowSearchSave.objSearch.ToolCheckout = MaintenanceValue;

                    if ($("#ToolCheckoutCollapse").text().trim() != '')
                        $("#ToolCheckoutCollapse").show();
                    else
                        $("#ToolCheckoutCollapse").hide();


                    if ($("#ToolCheckoutCollapse").find('span').length <= 2) {
                        $("#ToolCheckoutCollapse").scrollTop(0).height(50);
                    }
                    else {
                        $("#ToolCheckoutCollapse").scrollTop(0).height(100);
                    }
                    clearGlobalIfNotInFocus();

                    DoNarrowSearch();
                }).multiselectfilter();

            _NarrowSearchSave.setControlValue("ToolCheckout");

        },
        error: function (response) {
            // through errror message
        }
        , complete: function () {
            ToolTechnicianValue = $.map($("#ToolsTechnician").multiselect("getChecked"), function (input) { return input.value; })
            _NarrowSearchSave.objSearch.ToolsTechnician = ToolTechnicianValue;
            MaintenanceValue = $.map($("#ToolCheckout").multiselect("getChecked"), function (input) { return input.value; })
            _NarrowSearchSave.objSearch.ToolCheckout = MaintenanceValue;
            DoNarrowSearch();
        }

    });
    
    //DoNarrowSearch();
    CommonUDFNarrowSearch();
}

var toolListDTO = function (serverObj) {
    var self = this;
    self.ID = serverObj.ID;
    self.ToolName = serverObj.ToolName;
    self.Serial = serverObj.Serial;// null
    self.Description = serverObj.Description;// null
    self.Cost = serverObj.Cost;// null
    //self.IsCheckedOut = serverObj.IsCheckedOut;// null
    //self.ToolCategoryID = serverObj.ToolCategoryID;// null
    self.ToolCategory = serverObj.ToolCategory;// null

    self.Created = serverObj.CreatedDate; //serverObj.Created;// "/Date(1568275859783)/"
    //self.CreatedDate = serverObj.CreatedDate;// "9/12/2019 8:40:59 AM"
    self.Updated = serverObj.UpdatedDate; //serverObj.Updated;// "/Date(1568276769687)/"
    //self.UpdatedDate = serverObj.UpdatedDate;// "9/12/2019 8:56:09 AM"    


    //self.CreatedBy = serverObj.CreatedBy;// 2
    //self.LastUpdatedBy = serverObj.LastUpdatedBy;// 2
    self.CreatedByName = serverObj.CreatedByName;// "eTurns Admin"
    self.UpdatedByName = serverObj.UpdatedByName;// "eTurns Admin"
    //self.Room = serverObj.Room;// 50360
    //self.RoomName = serverObj.RoomName;// null
    self.GUID = serverObj.GUID;// "8f06b891-bc50-439d-99fd-c8f699419137"
    //self.IsArchived = serverObj.IsArchived;// false
    self.IsDeleted = serverObj.IsDeleted;
    //self.LocationID = serverObj.LocationID;// 100441
    //self.TechnicianGuID = serverObj.TechnicianGuID;// null
    self.Location = serverObj.Location;// ""
    //self.Technician = serverObj.Technician;// ""
    //self.CheckedOutQTYTotal = serverObj.CheckedOutQTYTotal;// null
    //self.CompanyID = serverObj.CompanyID;// 201210134
    //self.Action = serverObj.Action;// null
    //self.HistoryID = serverObj.HistoryID;// 0
    //self.UDF1 = serverObj.UDF1;// null
    //self.UDF2 = serverObj.UDF2;// null
    //self.UDF3 = serverObj.UDF3;// null
    //self.UDF4 = serverObj.UDF4;// null
    //self.UDF5 = serverObj.UDF5;// null
    //self.UDF6 = serverObj.UDF6;// null
    //self.UDF7 = serverObj.UDF7;// null
    //self.UDF8 = serverObj.UDF8;// null
    //self.UDF9 = serverOb.UDF9j;// null
    //self.UDF10 = serverObj.UDF10;// null
    self.Quantity = serverObj.Quantity;// 0
    //self.QuantityDisp = FormatedCostQtyValues(self.Quantity, 2);
    self.IsGroupOfItems = serverObj.IsGroupOfItems;// 1
    self.CheckOutStatus = serverObj.CheckOutStatus;// null
    self.CheckedOutQTY = serverObj.CheckedOutQTY;// 0
    //self.CheckedOutQTYDisp = serverObj.CheckedOutQTY === null ? 0.00 : FormatedCostQtyValues(serverObj.CheckedOutQTY, 2);


    self.CheckedOutMQTY = serverObj.CheckedOutMQTY;// 0
    //self.CheckedOutMQTYDisp = serverObj.CheckedOutMQTY === null ? 0.00 : FormatedCostQtyValues(serverObj.CheckedOutMQTY, 2);

    //self.CheckOutDate = serverObj.CheckOutDate;// null
    //self.CheckInDate = serverObj.CheckInDate;// null
    //self.CheckInCheckOutID = serverObj.CheckInCheckOutID;// null



    self.ReceivedOn = serverObj.ReceivedOnDate;//serverObj.ReceivedOn;// "/Date(1568275859783)/"
    //self.ReceivedOnDate = serverObj.ReceivedOnDate;

    self.ReceivedOnWeb = serverObj.ReceivedOnDateWeb;//serverObj.ReceivedOnWeb;// "/Date(1568275859783)/"
    //self.ReceivedOnDateWeb = serverObj.ReceivedOnDateWeb;

    self.AddedFrom = serverObj.AddedFrom;// "Web"
    self.EditedFrom = serverObj.EditedFrom;// "Web"
    //self.IsOnlyFromItemUI = serverObj.IsOnlyFromItemUI;// false
    //self.AppendedBarcodeString = serverObj.AppendedBarcodeString;//: null
    self.ToolUDF1 = serverObj.ToolUDF1;// "ToolUdf1.2"
    self.ToolUDF2 = serverObj.ToolUDF2;// "two"
    self.ToolUDF3 = serverObj.ToolUDF3;// "3.2"
    self.ToolUDF4 = serverObj.ToolUDF4;// "4.2"
    self.ToolUDF5 = serverObj.ToolUDF5;// "5.2"
    //self.Count = serverObj.Count;// 0


    //self.NoOfPastMntsToConsider = serverObj.NoOfPastMntsToConsider;
    //self.MaintenanceDueNoticeDays = serverObj.MaintenanceDueNoticeDays;
    //self.MaintenanceType = serverObj.MaintenanceType;
    //self.IsAutoMaintain = serverObj.IsAutoMaintain;
    self.TechnicianList = serverObj.TechnicianList;
    //self.CheckedOutQuantity = serverObj.CheckedOutQuantity;
    //self.CheckedInQuantity = serverObj.CheckedInQuantity;
    //self.DaysDiff = serverObj.DaysDiff;
    self.ToolImageExternalURL = serverObj.ToolImageExternalURL;
    //self.ImageType = serverObj.ImageType;
    self.ImagePath = serverObj.ImagePath;
    //self.IsBeforeCheckOutAndCheckIn = serverObj.IsBeforeCheckOutAndCheckIn;
    //self.Type = serverObj.Type;
    //self.IsBuildBreak = serverObj.IsBuildBreak;
    //self.NoOfItemsInKit = serverObj.NoOfItemsInKit;
    //self.WIPKitCost = serverObj.WIPKitCost;
    //self.KitToolQuantity = serverObj.KitToolQuantity;
    //self.KitToolName = serverObj.KitToolName;
    //self.KitToolSerial = serverObj.KitToolSerial;
    //self.AvailableWIPKit = serverObj.AvailableWIPKit;
    //self.AvailableInGeneralInventory = serverObj.AvailableInGeneralInventory;
    //self.ReOrderType = serverObj.ReOrderType;
    //self.KitCategory = serverObj.KitCategory;
    //self.AvailableKitQuantity = serverObj.AvailableKitQuantity;
    //self.ToolKitItemList = serverObj.ToolKitItemList;
    //self.IsAtleaseOneCheckOutCompleted = serverObj.IsAtleaseOneCheckOutCompleted;
    //self.ToolLocationDetailsID = serverObj.ToolLocationDetailsID;
    //self.AvailableToolQty = serverObj.AvailableToolQty;
    //self.ToolTypeTracking = serverObj.ToolTypeTracking;
    //self.ToolTypeTrackingStr = serverObj.ToolTypeTrackingStr;
    //self.DefaultLocation = serverObj.DefaultLocation;
    //self.DefaultLocationName = serverObj.DefaultLocationName;
    self.BinID = serverObj.BinID;
    self.SerialNumberTracking = serverObj.SerialNumberTracking;
    self.LotNumberTracking = serverObj.LotNumberTracking;
    self.DateCodeTracking = serverObj.DateCodeTracking;
    self.Type = serverObj.Type;
    self.IsBeforeCheckOutAndCheckIn = serverObj.IsBeforeCheckOutAndCheckIn;
    //self.WhatWhereAction = serverObj.WhatWhereAction;
    //self.IsSerialAvailable = serverObj.IsSerialAvailable;
    //self.IsCheckOutSerialAvailable = serverObj.IsCheckOutSerialAvailable;
    //self.ToolCheckoutGUID = serverObj.ToolCheckoutGUID;
    //self.LocationQty = serverObj.LocationQty;
    //self.ToolKitDetailID = serverObj.ToolKitDetailID;
    //self.ToolKitDetailGUID = serverObj.ToolKitDetailGUID;
    //self.ToolKitGuid = serverObj.ToolKitGuid;
    //self.KitPartNumber = serverObj.KitPartNumber;
    //self.TechnicianCode = serverObj.TechnicianCode;


    self.getAvailableQty = function () {

        //return FormatedCostQtyValues((obj.aData.Quantity - (obj.aData.CheckedOutQTY + obj.aData.CheckedOutMQTY)), 2);
        var Qty = parseFloat(self.Quantity);
        var CheckedOutQTY = parseFloat(self.CheckedOutQTY);
        var CheckedOutMQTY = parseFloat(self.CheckedOutMQTY);
        var ret = null;
        if (!isNaN(Qty) && !isNaN(CheckedOutQTY) && !isNaN(CheckedOutMQTY)) {
            ret = FormatedCostQtyValues((Qty - (CheckedOutQTY + CheckedOutMQTY)), 2);
        }
        else if (!isNaN(Qty) && !isNaN(CheckedOutQTY)) {
            ret = FormatedCostQtyValues((Qty - (CheckedOutQTY)), 2);
        }
        else if (!isNaN(Qty) && !isNaN(CheckedOutMQTY)) {
            ret = FormatedCostQtyValues((Qty - (CheckedOutMQTY)), 2);
        }
        else if (!isNaN(Qty)) {
            ret = FormatedCostQtyValues((Qty), 2);
        }
        else {
            ret = FormatedCostQtyValues(0, 2);
        }

        //return "<span style='background-color:cyan'>" + ret + "</span>";
        return ret;
    };

    self.AvailableQty = self.getAvailableQty();
    //self.getToolNameDisp = function () {
    //    return "<a onclick='return ShowEditTab(&quot;ToolEdit/" + self.ID
    //        + "&quot;,&quot;frmTool&quot;)' id='ToolName' href='JavaScript:void(0);'>" + self.ToolName
    //        + "</a>" + " <input type='hidden' id='hdnGUID' value='" + self.GUID.toString()
    //        + "' />" + "<span id='spnToolMasterID' style='display:none'>" + self.ID + "</span>";
    //};
    //self.ToolNameDisp = self.getToolNameDisp();

    //self.IsGroupOfItemsDisp = self.IsGroupOfItems === 0 ? "No" : (self.IsGroupOfItems === 1 ? "Yes" : "");

    //self.CostDisp = (self.Cost !== null && self.Cost !== '') ? FormatedCostQtyValues(self.Cost, 1) : '';

    self.getTechnicianListDisp = function () {
        var strReturn = '<span style="position:relative"><input type="text" id="txtTechnician" class="text-boxinner AutoTechnician" style = "width:93%;" value="' + SelectTechnicianText + '" />';
        strReturn = strReturn + ' <input type="hidden" id="TechnicianGUID" value="" />';
        strReturn = strReturn + ' <a id="lnkShowAllOptions" href="javascript:void(0);" style="position:absolute; right:5px; top:0px;" class="ShowAllOptions" ><img src="/Content/images/arrow_down_black.png" alt="select" /></a></span>';
        return strReturn;
    };

    self.TechnicianListDisp = self.getTechnicianListDisp();

    // checkout
    self.getIsCheckout = function () {
        var Qty = isNaN(parseInt(self.Quantity)) ? 0 : self.Quantity;
        var CheckedoutQty = isNaN(parseInt(self.CheckedOutQTY)) ? 0 : self.CheckedOutQTY;
        var CheckedOutMQTY = isNaN(parseInt(self.CheckedOutMQTY)) ? 0 : self.CheckedOutMQTY;
        var isCheckout = Qty - (CheckedoutQty + CheckedOutMQTY) > 0;

        return isCheckout;
    };

    self.getTxtQty = function () {

        //var Qty = isNaN(parseInt(self.Quantity)) ? 0 : self.Quantity;
        //var CheckedoutQty = isNaN(parseInt(self.CheckedOutQTY)) ? 0 : self.CheckedOutQTY;
        //var CheckedOutMQTY = isNaN(parseInt(self.CheckedOutMQTY)) ? 0 : self.CheckedOutMQTY;
        //if ((Qty - (CheckedoutQty + CheckedOutMQTY)) > 0) {
        if (self.getIsCheckout()) {
            if (self.IsGroupOfItems == 0) {
                return "<input type='text' value='" + 1 + "' class='numericinput numericalign' onkeypress='return false;'  id='txtQty' style='width:93%;disabled:true;' />";
            }
            else {
                return "<input type='text' value='1' class='numericinput numericalign' onkeypress='return onlyNumeric(event)'  id='txtQty' style='width:93%;' />";
            }
        }
        else {
            return "";
        }
    };

    self.txtQty = self.getTxtQty();

    self.getchkMaintance = function () {
        if (allowCheckinCheckOut) {
            //var Qty = isNaN(parseInt(self.Quantity)) ? 0 : self.Quantity;
            //var CheckedoutQty = isNaN(parseInt(self.CheckedOutQTY)) ? 0 : self.CheckedOutQTY;
            //var CheckedOutMQTY = isNaN(parseInt(self.CheckedOutMQTY)) ? 0 : self.CheckedOutMQTY;

            //if ((Qty - (parseInt(CheckedoutQty) + parseInt(CheckedOutMQTY))) > 0 && !self.IsDeleted) {
            if (self.getIsCheckout() && !self.IsDeleted) {
                return "<input type='checkbox' id='chkMaintance' name='manintancechk' value='maintanence' />";
            }
            else
                return "";
        }
    };

    self.chkMaintance = self.getchkMaintance();


    getbtnCheckOut = function () {
        if (allowCheckinCheckOut) {

            //var MCheckOutQTY = self.CheckedOutMQTY == null ? 0 : self.CheckedOutMQTY;
            //var CheckOutQTY = self.CheckedOutQTY == null ? 0 : self.CheckedOutQTY;
            //if ((parseInt(self.Quantity) - (parseInt(CheckOutQTY) + parseInt(MCheckOutQTY))) > 0 && !self.IsDeleted) {
            if (self.getIsCheckout() && !self.IsDeleted) {
                return "<span id='spnCheckOutStatus' style='display:none'>" + self.CheckOutStatus + "</span>" + "<span id='spnCheckedOutQTY' style='display:none'>" + self.CheckedOutQTY + "</span><span id='spnCheckedOutMQTY' style='display:none'>"
                    + self.CheckedOutMQTY + "</span>" + "<span id='spnCheckInCheckOutID'  style='display:none'>"
                    + self.CheckInCheckOutID + "</span>" + "<span id='spnQuantity'  style='display:none'>"
                    + self.Quantity + "</span>" + "<span id='spnToolID'  style='display:none'>"
                    + self.GUID + "</span>" + "<span id='spnToolPKID'  style='display:none'>"
                    + self.ID + "</span>"
                    + "<input type='button' value='Check Out' id='btnCheckOut' onclick='return PerformTCICO(this,2);' class='CreateBtn pull' />" + "<span id='spnToolType'  style='display:none'>"
                    + self.Type + "</span><span id='spnToolIsBuildBreak' style='display:none'>"
                    + self.IsBuildBreak + "</span>";
            }
            else {
                return "<span id='spnCheckOutStatus' style='display:none'>"
                    + self.CheckOutStatus + "</span>" + "<span id='spnCheckedOutQTY' style='display:none'>"
                    + self.CheckedOutQTY + "</span><span id='spnCheckedOutMQTY' style='display:none'>"
                    + self.CheckedOutMQTY + "</span>" + "<span id='spnCheckInCheckOutID'  style='display:none'>"
                    + self.CheckInCheckOutID + "</span>" + "<span id='spnQuantity'  style='display:none'>"
                    + self.Quantity + "</span>" + "<span id='spnToolID'  style='display:none'>"
                    + self.GUID + "</span>" + "<span id='spnToolPKID'  style='display:none'>"
                    + self.ID + "</span>" + "<span id='spnToolType'  style='display:none'>"
                    + self.Type + "</span><span id='spnToolIsBuildBreak' style='display:none'>"
                    + self.IsBuildBreak + "</span>";
            }
        }
        else {
            return "<span id='spnCheckOutStatus' style='display:none'>"
                + self.CheckOutStatus + "</span>" + "<span id='spnCheckedOutQTY' style='display:none'>"
                + self.CheckedOutQTY + "</span><span id='spnCheckedOutMQTY' style='display:none'>"
                + self.CheckedOutMQTY + "</span>" + "<span id='spnCheckInCheckOutID'  style='display:none'>"
                + self.CheckInCheckOutID + "</span>" + "<span id='spnQuantity'  style='display:none'>"
                + self.Quantity + "</span>" + "<span id='spnToolID'  style='display:none'>"
                + self.GUID + "</span>" + "<span id='spnToolPKID'  style='display:none'>"
                + self.ID + "</span>" + "<span id='spnToolType'  style='display:none'>"
                + self.Type + "</span><span id='spnToolIsBuildBreak' style='display:none'>"
                + self.IsBuildBreak + "</span>";
        }
    };

    self.btnCheckOut = getbtnCheckOut();

    //getImagePathDisp = function () {

    //    if ((self.ImagePath != '' && self.ImagePath != null) ||
    //        (self.ToolImageExternalURL != '' && self.ToolImageExternalURL != null)) {

    //        if (self.ImagePath != '' && self.ImagePath != null) {

    //            var path = logoPathImage;

    //            return '<img style="cursor:pointer;"  alt="' + (self.ItemNumber) + '" id="ItemImageBox" width="120px" height="120px" src="' + path + "/"
    //                + self.ID + "/" + self.ImagePath + '">';
    //        }
    //        else if (self.ToolImageExternalURL != '' && self.ToolImageExternalURL != null) {
    //            return '<img style="cursor:pointer;"  alt="' + (self.ItemNumber)
    //                + '" id="ItemImageBox" width="120px" height="120px" src="' + self.ToolImageExternalURL + '">';
    //        }
    //        else {
    //            return "<img src='../Content/images/no-image.jpg' />";
    //        }
    //    }
    //    else {
    //        return "<img src='../Content/images/no-image.jpg' />";
    //    }
    //};

    //self.ImagePathDisp = getImagePathDisp();

    gettxtCheckInQty = function () {

        var Qty = isNaN(parseInt(self.Quantity)) ? 0 : self.Quantity;
        var CheckedoutQty = isNaN(parseInt(self.CheckedOutQTY)) ? 0 : self.CheckedOutQTY;
        var CheckedOutMQTY = isNaN(parseInt(self.CheckedOutMQTY)) ? 0 : self.CheckedOutMQTY;
        var ret = null;
        //if ((Qty - (CheckedoutQty + CheckedOutMQTY)) < Qty) {
        if (CheckedoutQty > 0 || CheckedOutMQTY > 0) {
            if (self.IsGroupOfItems == 0) {
                ret = "<input type='text' value='" + 1 + "' class='numericinput numericalign' onkeypress='return false;'  id='txtCheckInQty' style='width:93%;disabled:true;display:none;' />";
            }
            else {

                if ((CheckedoutQty + CheckedOutMQTY) == 1 || self.CheckedOutQTYTotal == 1) {
                    ret = " <input type='text' value='1' class='numericinput numericalign' onkeypress='return onlyNumeric(event)'  id='txtCheckInQty' style='width:93%;display:none;' />";
                }
                else {
                    ret = " <input type='text' value='' class='numericinput numericalign' onkeypress='return onlyNumeric(event)'  id='txtCheckInQty' style='width:93%;display:none;' />";
                }
            }
        }
        else {
            ret = "";
        }

        return  ret ;
    };
    
    self.txtCheckInQty = gettxtCheckInQty();

    self.ToolTypeName = self.Type === 1 ? "Tool" : "Kit Tool";

    getimgExpand = function () {
        var Qty = isNaN(parseInt(self.Quantity)) ? 0 : self.Quantity;
        var CheckedoutQty = isNaN(parseInt(self.CheckedOutQTY)) ? 0 : self.CheckedOutQTY;
        var CheckedOutMQTY = isNaN(parseInt(self.CheckedOutMQTY)) ? 0 : self.CheckedOutMQTY;
        var availQty = Qty - (parseInt(CheckedoutQty) + parseInt(CheckedOutMQTY));
        if (((Qty > availQty)) && !self.IsDeleted) {
            return '<img class="Expand" src="' + sImageUrl + 'drildown_open.jpg' + '">';
        }
        else {
            return '';
        }
    };

    self.imgExpand = getimgExpand();

        
} // ;