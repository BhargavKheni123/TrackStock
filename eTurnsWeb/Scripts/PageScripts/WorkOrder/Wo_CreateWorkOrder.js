
var formData = new FormData();

var _CreateWO = (function ($) {
    var self = {};
    self.workOrderID = null;
    self.isHistory = null;
    self.GUID = null;
    
    self.WOStatus = null;
    self.SupplierAccountGuid = null;
    

    self.urls = {
        LoadWOItemsUrl: null,
        LoadItemMasterModelUrl: null,
        CustomerMasterCreateUrl: null,
        LoadToolsOnModelUrl:null
    };

    self.initUrls = function (loadWOItemsUrl, loadItemMasterModelUrl
        , customerMasterCreateUrl, LoadToolsOnModelUrl
    ) {
        self.urls.LoadWOItemsUrl = loadWOItemsUrl;
        self.urls.LoadItemMasterModelUrl = loadItemMasterModelUrl;
        self.urls.CustomerMasterCreateUrl = customerMasterCreateUrl;
        self.urls.LoadToolsOnModelUrl = LoadToolsOnModelUrl;
    };

    self.init = function (workOrderID, isHistory, GUID, WOStatus,SupplierAccountGuid ) {
        self.workOrderID = workOrderID;
        self.isHistory = isHistory;
        self.GUID = GUID;
        self.WOStatus = WOStatus;
        self.SupplierAccountGuid = SupplierAccountGuid;
        self.initEvents();
    }

    self.initEvents = function () {

        $(document).ready(function () {
            //$('#divReorderPopup').find('#hdnReOrderExecuteFunctionString').val('_CreateWO.OpenItemPopup()');
            $("form").submit(function (e) {
                $(':input', '#frmWOMaster')
                    .removeAttr('disabled');
                $.validator.unobtrusive.parse("#frmWOMaster");
                if ($(this).valid()) {
                    rememberUDFValues($("#hdnPageName").val(), self.workOrderID);
                }
                e.preventDefault();
            });

            $("#ItemModelTemp").dialog({
                autoOpen: false,
                modal: true,
                draggable: true,
                resizable: true,
                width: '95%',
                height: 710,
                title: itemTitle,
                open: function () {
                    $('span#ui-dialog-title-ItemModelTemp').html(itemTitle + ' - ' + $('#txtWONAme').val());
                    //$('#DivLoading').show();
                    //$(this).css("top", "-1247");
                },
                close: function () {
                    //CallThisFunctionFromModel('success');
                    $(this).empty();
                    $("div#ItemModelTemp").empty();
                    $('#btnAddNewItemps, #btnAddNewTools').removeAttr('disabled');
                    ReDirectData();
                }
            });

            $('div#divToolModel').dialog({
                autoOpen: false,
                modal: true,
                draggable: true,
                resizable: true,
                width: '95%',
                height: 710,
                title: toolTitle,
                open: function () {
                    $('span#ui-dialog-title-divToolModel').text(toolTitle + ' - ' + $('#txtWONAme').val());
                },
                close: function () {
                    $(this).empty();
                    $("div#divToolModel").empty();
                    $('#btnAddNewItemps, #btnAddNewTools').removeAttr('disabled');
                    ReDirectData();
                }
            });

            //$("span#spnTotalCost").text("$ " + FormatedCostQtyValues($("span#spnTotalCost").text().toString().replace("$ ", ""), 1));
            $('form').areYouSure({ 'message': MsgLostChangesConfirmation });
            var IsFirstTime = true;

            $(".text-boxPriceFormat").priceFormat({
                prefix: '',
                thousandsSeparator: '',
                centsLimit: parseInt($('#hdCostcentsLimit').val(), 10)
            });
            $(".text-boxQuantityFormat").priceFormat({
                prefix: '',
                thousandsSeparator: '',
                centsLimit: parseInt($('#hdQuantitycentsLimit').val(), 10)
            });

            $(".odometer").priceFormat({
                prefix: '',
                thousandsSeparator: '',
                centsLimit: 2
            });


            $('#btnClose').click(function () {
                $(".ui-dialog-titlebar-close").click();
            });

            if (parseInt(_CreateWO.workOrderID, 10) <= 0) {
                $('#ExpandedContent').css('display', '');
                $('#ancHideShowContent').css('display', 'none');
            }
            else {
                $('#ExpandedContent').css('display', 'none');
                $('#ancHideShowContent').css('display', '');
            }

            $('#btnCancel').click(function (e) {
                if (self.isHistory != 'True') {

                    SwitchTextTab(0, 'WOCreate', 'frmWOMaster');
                    if (oTable !== undefined && oTable != null) {
                        oTable.fnDraw();
                    }
                    CallWONarrowFunctions();
                    $(".tab5").hide();
                }
                else {
                    $(".ui-dialog-titlebar-close").click();

                }
                $('#NarroSearchClear').click();
            });
            checkRememberUDFValues($("#hdnPageName").val(), _CreateWO.workOrderID);

            self.LoadWOLineItems();

            $('#btnAddNewItemps').click(function () {
                if (isInsertPull == 'True') {
                    return _CreateWO.OpenItemPopup();
                }
                else {
                    alert(DoNotPullInsertRights);
                }

            });


            $('#btnUncloseWO').on('click', function () {
                var WOguid = _CreateWO.GUID;
                $.ajax({
                    url: 'UncloseWorkOrder',
                    type: 'Post',
                    data: { 'WOGUID': WOguid },
                    dataType: 'json',
                    success: function (response) {

                        if (response.Status == "ok") {
                            $('div#target').fadeToggle();
                            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                            $("#spanGlobalMessage").html(response.Message);
                            $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                            clearControls('frmWOMaster');
                            oTable.fnDraw();
                            SwitchTextTab(0, 'WOCreate', 'frmWOMaster');
                        }
                        else {
                            $('div#target').fadeToggle();
                            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                            $("#spanGlobalMessage").html(response.Message);
                            $("#spanGlobalMessage").removeClass('errorIcon succesIcon').addClass('WarningIcon');
                        }
                    },
                    error: function (response) {
                        $('div#target').fadeToggle();
                        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
                        $("#spanGlobalMessage").html(response.Message);
                        $("#spanGlobalMessage").removeClass('errorIcon succesIcon').addClass('WarningIcon');
                    }
                });
            });

            $(".text-boxPriceFormat").priceFormat({
                prefix: '',
                thousandsSeparator: '',
                centsLimit: parseInt($('#hdCostcentsLimit').val(), 10)
            });
            $(".text-boxQuantityFormat").priceFormat({
                prefix: '',
                thousandsSeparator: '',
                centsLimit: parseInt($('#hdQuantitycentsLimit').val(), 10)
            });

            $('.selectBox').change(function () {
                var ID = this.id;
                if (ID == "CustomerGUID")
                    $("#Customer").val($("#" + ID + " option:selected").text());
                else if (ID == "TechnicianID")
                    $("#Technician").val($("#" + ID + " option:selected").text());
                else if (ID == "AssetGUID")
                    $("#Asset").val($("#" + ID + " option:selected").text());
                else if (ID == "ToolGUID")
                    $("#Tool").val($("#" + ID + " option:selected").text());
            });

            $('#ancHideShowContent').click(function () {
                if ($('#ExpandedContent').css('display') == 'none') {
                    $('#ExpandedContent').css('display', '');
                    $('#ancHideShowContent img').attr('src', '/Content/images/drildown_close.jpg');
                }
                else {
                    $('#ExpandedContent').css('display', 'none');
                    $('#ancHideShowContent img').attr('src', '/Content/images/drildown_open.jpg');
                }
            });


            // grab the file input and bind a change event onto it
            $('#file').bind("change", function () {
                // new html5 formdata object.
                //$("ul.FileUploadList").empty();
                $("table.FileUploadList tr td.dataTables_empty").remove();
                var Li = '';
                var WorkOrderAllowedFileExtension = $("#hdWorkOrderAllowedFileExtension").val();
                var validExts = WorkOrderAllowedFileExtension.split(',');
                var strValidationMessage = "";                

                for (var i = 0, len = document.getElementById('file').files.length, avalbleimg = $("table.FileUploadList tr").length; i < len; i++ , avalbleimg++) {
                    var IsValidFile = true;
                    var fileExt = document.getElementById('file').files[i].name;
                    if ($("#lblnofilechoosen").length > 0) {
                        $("#lblnofilechoosen").text(fileExt);
                    }
                    fileExt = fileExt.substring(fileExt.lastIndexOf('.'));
                    if (validExts.indexOf(fileExt.toLowerCase()) <= -1) {
                        IsValidFile = false;
                        strValidationMessage = strValidationMessage + document.getElementById('file').files[i].name + MsgInvalidFileSelected;
                    }
                    if (IsValidFile == true) {
                        var localdate = moment.utc().format('YYYY_MM_DD_HH_mm_ss');
                        localdate += localdate + avalbleimg;
                        Li += "<tr id=" + localdate + "  uploaded='false'>";
                        Li += "<td style='width:98%;text-align:left;'>" + document.getElementById('file').files[i].name + "</td><td style='width:2%'><a href='javascript:;' onclick='_CreateWO.DeleteCurrentFile(\"" + localdate + "\");'><img src='/content/images/delete.png' /></a></td>";
                        Li += "</tr>";
                        formData.append("file" + localdate, document.getElementById('file').files[i]);
                    }
                }
                if (strValidationMessage != "") {
                    alert(strValidationMessage + MsgvalidFileList.replace("{0}", validExts.toString()));
                }
                $("table.FileUploadList").append(Li);
                //$("table.FileUploadList").fnDestroy();
                //Disable sorting
                $("table.FileUploadList").dataTable();
                $("table.FileUploadList thead tr").empty();
                var resfileName = "File Name";
                if (typeof ResFileName != 'undefined') {
                    resfileName = ResFileName;
                }
                var resDelete = "File Name";
                if (typeof ResDelete != 'undefined') {
                    resDelete = ResDelete;
                }
                $("table.FileUploadList thead tr").append("<td style='width: 260px;padding-right: 195px;'><strong>" + resfileName + "</strong></td><td><strong>" + resDelete +"</strong></td>")
            });

            // Assuming that the div or any other HTML element has the ID = loading and it contains the necessary loading image.
            $('#DivLoading').hide();  //initially hide the loading icon

            // commented below by amit t on 1-apr-20
            //$('#DivLoading').ajaxStart(function () {
            //    $(this).show();
            //});
            //$("#DivLoading").ajaxStop(function () {
            //    $(this).hide();
            //});
            $('input#btnAddNewTools').on('click', function () {
                OpenToolPopup();
            });

            $("#NewMasterPopUP").dialog({
                autoOpen: false,
                show: "blind",
                hide: "explode",
                height: 600,
                width: 800,
                modal: true,
                open: function () {
                    $(this).parent().find("span.ui-dialog-title").html("Add New " + $(this).data("data"));
                },
                close: function () {
                    $('#DivLoading').show();
                    RefreshDropdownList($(this).data("data"), $(this).data("IDVal"));
                    $(this).data("data", null);
                    $(this).data("IDVal", null);
                    $(this).parent().find("span.ui-dialog-title").html('');
                }
            });

            self.setddlSupplier();
            _Common.addRequiredSign();
            $("#WOSignatureImage").change(function () {
                readURL(this);
            });

            //$("#WOSignatureImage").show();
            setImagePath();
            $("#btnDeleteImage").hide();
            if ((WOSignatureImagePath != undefined && WOSignatureImagePath != null && WOSignatureImagePath != '') || ($("input#currentpath").val() != '' && $("input#currentpath").val() != '/Content/images/no-image.jpg')) {
                $("#btnDeleteImage").show();
            }
        });// ready

    };// init events

    
    self.setddlSupplier = function () {

        $('#ddlSupplier').change(function () {
            if (!isNaN(parseInt($(this).val())) && parseInt($(this).val()) > 0) {
                $('#DivLoading').show();

                $('#ddlSupplierAccount').empty();
                $('#ddlSupplierAccount').html('');
                if ($("select#ddlSupplier").val() != '' && $("select#ddlSupplier").val() != null && $("select#ddlSupplier").val() != undefined) {
                    $.ajax({
                        url: "/Order/ShowData",
                        data: { "SupplierID": $("select#ddlSupplier").val() },
                        type: "Get",
                        success: function (data) {
                            var opt = new Option("Please Select", "");
                            $('#ddlSupplierAccount').append(opt);
                            for (var i = 0; i < data.length; i++) {
                                var opt = new Option(data[i].AccountNumnerMerge, data[i].GUID);
                                $('#ddlSupplierAccount').append(opt);

                            }
                            $('#ddlSupplierAccount').val(_CreateWO.SupplierAccountGuid);
                            $('#DivLoading').hide();
                        }, error: function () {
                            $('#DivLoading').hide();
                        }
                    });


                }
            }
        });

        if (self.WOStatus != "Open" && (self.SupplierAccountGuid == '' || self.SupplierAccountGuid == '00000000-0000-0000-0000-000000000000')) {
            $('#ddlSupplierAccount').empty();
            $('#ddlSupplierAccount').html('');
        }
        else {
            $('#ddlSupplierAccount').empty();
            $('#ddlSupplierAccount').html('');
            if ($("select#ddlSupplier").val() != '' && $("select#ddlSupplier").val() != null && $("select#ddlSupplier").val() != undefined) {
                $.ajax({
                    url: "/Order/ShowData",
                    data: { "SupplierID": $("select#ddlSupplier").val() },
                    type: "Get",
                    success: function (data) {
                        var opt = new Option("Please Select", "");
                        $('#ddlSupplierAccount').append(opt);
                        for (var i = 0; i < data.length; i++) {
                            var opt = new Option(data[i].AccountNumnerMerge, data[i].GUID);
                            $('#ddlSupplierAccount').append(opt);

                        }
                        $('#ddlSupplierAccount').val(_CreateWO.SupplierAccountGuid);
                        if ((_CreateWO.SupplierAccountGuid == '' || _CreateWO.SupplierAccountGuid == '00000000-0000-0000-0000-000000000000') && _CreateWO.workOrderID == 0) {

                            $.ajax({
                                url: "/Order/GetDefaultAccount",
                                data: { "SupplierID": $("select#ddlSupplier").val() },
                                type: "Get",
                                success: function (data) {
                                    $('#ddlSupplierAccount').val(data);

                                }
                            });
                        }
                    }
                });
            }
        }
    };

    self.LoadWOLineItems = function () {

        $('#WOLineItems').empty();

        if (parseFloat(_CreateWO.workOrderID) > 0) {
            if (_CreateWO.isHistory != 'True') {
                $('#WOLineItems').load(_CreateWO.urls.LoadWOItemsUrl, function () {
                    $('#DivLoading').show();
                });
                $.ajax({
                    url: "GetWorkOrderFiles",
                    type: 'post',
                    data: { 'WorkOrderGuid': _CreateWO.GUID },
                    dataType: 'json',
                    async: false,
                    success: function (data) {
                        var Li = '';
                        $.each(data.DDData, function (i, val) {
                            Li += "<tr id='" + val + "' uploaded='true'>";
                            Li += "<td style='width:98%;text-align:left;'>" +
                                "<a id='apreview" + val + "' class='preview' href='/Consume/Get?path=" + workOrderFilePath + "/" + _CreateWO.workOrderID + "/" + i + "' target='_blank'>" + i + "</a>" +
                                "</td><td style='width:2%'>" +
                                "<a href='javascript:;' onclick='_CreateWO.DeleteExistingFile(\"" + val + "\");'>" +
                                "<img src='/content/images/delete.png' /></a></td>";
                            //Li += "<td style='width:98%;text-align:left;'><a class='preview' href='" + workOrderFilePath + "/" + _CreateWO.workOrderID + "/" + i + "' target='_blank'>" + i + "</a></td><td style='width:2%'><a href='javascript:;' onclick='_CreateWO.DeleteExistingFile(\"" + val + "\");'><img src='/content/images/delete.png' /></a></td>";
                            Li += "</tr>";
                        });
                        $("table.FileUploadList").append(Li);

                        // starting the script on page load

                        imagePreview();

                       $("table.FileUploadList").dataTable({ "bPaginate": false, "bInfo": false, "oLanguage": { "sEmptyTable": MsgNoDataAvailableInTable } });
                        //Disable sorting
                        var resfileName = "File Name";
                        if (typeof ResFileName != 'undefined') {
                            resfileName = ResFileName;
                        }
                        var resDelete = "Delete";
                        if (typeof ResDelete != 'undefined') {
                            resDelete = ResDelete;
                        }
                       $("table.FileUploadList thead tr").empty();
                        $("table.FileUploadList thead tr").append("<td style='width: 260px;padding-right: 195px;'><strong>" + resfileName + "</strong></td><td><strong>" + resDelete + "</strong></td>")
                    },
                    error: function (request) {
                        console.log(request.responseText);
                    }
                });
                //// need to call below block in case of New Item saved and open POPUP Item ... START

                if (NeedToOpenItemPopupNow) {
                    NeedToOpenItemPopupNow = false;
                    setTimeout("_CreateWO.OpenItemPopup()", 3000);
                    return false;
                }
                //// need to call below block in case of New Item saved and open POPUP Item ... END
            }
            else {
                var action = _CreateWO.urls.LoadWOItemsUrl;
                var IsHistory = true;
                action += '&IsHistory=' + IsHistory;
                $('#WOLineItems').load(action, function () {
                });
                $("table.FileUploadList").dataTable({ "bPaginate": false, "bInfo": false,"oLanguage": { "sEmptyTable": MsgNoDataAvailableInTable } });
            }
        }
        else {
            $("table.FileUploadList").dataTable({ "bPaginate": false, "bInfo": false,"oLanguage": { "sEmptyTable": MsgNoDataAvailableInTable } });
        }
        //$('#DivLoading').hide();

        if (_CreateWO.isHistory == 'True') {
            $(':input', '#frmWOMaster').not('#btnClose').attr('disabled', 'disabled');
        }
    }


    self.DeleteExistingFiles = function (FileId, WorkOrderGuid) {
        $.ajax({
            url: 'DeleteExistingFiles',
            type: 'Post',
            data: { 'FileId': FileId, 'WorkOrderGuid': WorkOrderGuid },
            dataType: 'json',
            success: function (response) {
            }
        });
    }


    self.closeModalRequisition = function () {
        $.modal.impl.close();
        clearControls('frmWOMaster');
        SwitchTextTab(0, 'WOCreate', 'frmWOMaster');
    }


    self.CheckDuplicateFile = function () {
        var liText = '';
        var isDuplica = false;
        $("table.FileUploadList tbody tr").each(function () {

            var text = '';
            if ($(this).find("td:first").find("a").length > 0) {
                text = $(this).find("td:first").find("a").text();
            }
            else {
                text = $(this).find("td:first").html();
            }
            if (liText.indexOf('|' + text + '|') == -1) {
                liText += '|' + text + '|';
            }
            else {
                alert(MsgRemoveDuplicateFileName);
                isDuplica = true;
            }
        });
        if (isDuplica) {
            return false;
        }
        $('#NarroSearchClear').click();
    }

    self.AddNewFromPopup = function (PopupFor) {
        $('#DivLoading').show();
        var _URL = ''
        if (PopupFor == 'Customer') {
            _URL = _CreateWO.urls.CustomerMasterCreateUrl;
        }
        else {
            return false;
        }
        $('#NewMasterPopUP').load(_URL, function () {
            $('#NewMasterPopUP').data("data", PopupFor).dialog('open');
            $('#DivLoading').hide();
        });
    }

    self.DeleteCurrentFile = function (currentPos) {
        formData.delete("file" + currentPos);
        $("table.FileUploadList tr#" + currentPos + "").remove();
        //        $("table.FileUploadList").fnDestroy();
        $("table.FileUploadList").dataTable();
        //SaveWorkOrderImage();
    }

    self.DeleteExistingFile = function (FileId) {
        $("table.FileUploadList tr#" + FileId + "").remove();
        if (DeleteWoFileId != '') {
            DeleteWoFileId += ',' + FileId;
        }
        else {
            DeleteWoFileId = FileId;
        }
    }

    self.SaveWorkOrderImage = function (Id, Guid) {
        var IdGuid = Id + "$" + Guid;

        if ($("table.FileUploadList tr[uploaded='false']").length > 0) {

            //send formdata to server-side
            $.ajax({
                url: "/api/fileupload/WorkOrderFileUpload/" + Id,
                type: 'post',
                data: formData,
                dataType: 'html', // we return html from our php file
                async: true,
                processData: false,  // tell jQuery not to process the data
                contentType: false,   // tell jQuery not to set contentType
                success: function (data) {
                    formData = new FormData();
                    //$('#upload-result').append('<div class="alert alert-success"><p>File(s) uploaded successfully!</p><br />');
                    //$('#upload-result .alert').append(data);
                },
                error: function (request) {
                    console.log(request.responseText);
                }
            });
        }
    }


    self.onSuccess = function (response) {
        IsRefreshGrid = true;
        $('div#target').fadeToggle();
        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
        $("#spanGlobalMessage").html(response.Message);
        $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
        var idValue = $("#hiddenID").val();
        if (response.Status == "fail") {
            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            $("#RequisitionNumber").val("");
            $("#RequisitionNumber").focus();
        }
        else if (idValue == 0) {
            $("#RequisitionNumber").val("");
            $("#RequisitionNumber").focus();
            if (response.Status == "duplicate")
                $("#spanGlobalMessage").removeClass('errorIcon succesIcon').addClass('WarningIcon');
            else {
                //clearControls('frmWOMaster');
                if (isInsertPull == 'True') {
                    NeedToOpenItemPopupNow = true;
                }
                else {
                    NeedToOpenItemPopupNow = false;
                    $("#spanGlobalMessage").html(DoNotPullInsertRights);
                    $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
                }
                setDefaultUDFValues($("#hdnPageName").val(), _CreateWO.workOrderID);
                if (DeleteWoFileId != '') {
                    _CreateWO.DeleteExistingFiles(DeleteWoFileId, response.GUID);
                }
                CallWONarrowFunctions();
                _CreateWO.SaveWorkOrderImage(response.ID, response.GUID);
                if ($("#WOSignatureImage").val() != "") {
                    (ajaxFileUpload1(response, idValue));
                }
                ShowEditTabGUID("WOEdit?WorkOrderGUID=" + response.GUID, "frmWOMaster");
            }
        }
        else if (idValue > 0) {
            if (response.Status == "duplicate") {
                $("#spanGlobalMessage").removeClass('errorIcon succesIcon').addClass('WarningIcon');
                $("#RequisitionNumber").val("");
                $("#RequisitionNumber").focus();
            }
            else {
                if (DeleteWoFileId != '') {
                    _CreateWO.DeleteExistingFiles(DeleteWoFileId, response.GUID);
                }
                //IsRejectTheRequisition();
                CallWONarrowFunctions();
                _CreateWO.SaveWorkOrderImage(response.ID, response.GUID);
                var previewImageSrc = $('#previewHolder').attr('src');

                if ((WOSignatureImagePath != undefined && WOSignatureImagePath != null && WOSignatureImagePath != '' && WOSignatureImagePath.length > 0)
                    && ($("input#currentpath").val() == '' || $("input#currentpath").val() == '/Content/images/no-image.jpg')
                    && (typeof (previewImageSrc) == "undefined" || previewImageSrc == null || previewImageSrc == '' || previewImageSrc == '/Content/images/no-image.jpg')) {
                    DeleteWOSignature(response.ID, WOSignatureName);
                }

                if ($("#WOSignatureImage").val() != "") {
                    (ajaxFileUpload1(response, idValue));
                }
                
                clearControls('frmWOMaster');
                SwitchTextTab(0, 'WOCreate', 'frmWOMaster');
            }
        }
    }

    self.onFailure = function (message) {

        $("#spanGlobalMessage").html(message.statusText);
        $('div#target').fadeToggle();
        $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
        $("#WOName").focus();
    }

    self.OpenItemPopup =  function () {

        if (isInsertPull != 'True') {
            alert(DoNotPullInsertRights);
            return false;
        }
        else if (parseInt(_CreateWO.workOrderID, 10) > 0) {
            ClearVariables();
            var strUrl = _CreateWO.urls.LoadItemMasterModelUrl;
            strUrl = strUrl + '?ParentId=' + _CreateWO.workOrderID;
            strUrl = strUrl + '&ParentGuid=' + _CreateWO.GUID;

            $('#divReorderPopup').find('#hdnReOrderExecuteFunctionString').val('_CreateWO.OpenItemPopup()');
            $('#ItemModelTemp').load(strUrl, new function () {
                $('#ItemModelTemp').dialog('open').dialog("moveToTop");
                //style = "top:0;left:0;right:0;bottom:0"
            });
            if ($("input#global_filter").length > 0) {
                $("input#global_filter").focus();
                setTimeout(function () {
                    $("input#global_filter").focus();
                }, 600);
            }
            if ($("input.searchinput").length > 0) {
                $("input.searchinput").focus();
                setTimeout(function () {
                    $("input.searchinput").focus();
                }, 600);

            }
        }
        //return false;
    }


    /// private functions

    

    function RefreshDropdownList(PopupFor, IDVal) {
        if (IDVal != undefined) {
            var _ControlID = '';
            if (PopupFor == 'Customer') {
                _ControlID = "CustomerGUID";
            }
            var arrdata = IDVal.split("~");
            var listData = $('select[id*="' + _ControlID + '"]');
            $(listData).each(function () {
                $(this).append($("<option />").val(arrdata[0]).text(arrdata[1]));
            });
            var foption = $('select[id*="' + _ControlID + '"] option:first');
            var soptions = $('select[id*="' + _ControlID + '"] option:not(:first)').sort(function (a, b) {
                return a.text == b.text ? 0 : a.text < b.text ? -1 : 1
            });
            $(listData).html(soptions).prepend(foption);
        }
        $('#DivLoading').hide();
    }

    return self;

})(jQuery); // _CreateWO end

//--------------------


function closeModalRequisitionApprove() {
    $.modal.impl.close();
    return false;
}

function OpenItemPopup() {
    if (isInsertPull != 'True') {
        alert(DoNotPullInsertRights);
        return false;
    }
    else if (parseInt(_CreateWO.workOrderID, 10) > 0) {
        ClearVariables();
        var strUrl = _CreateWO.urls.LoadItemMasterModelUrl;
        strUrl = strUrl + '?ParentId=' + _CreateWO.workOrderID;
        strUrl = strUrl + '&ParentGuid=' + _CreateWO.GUID;

        $('#divReorderPopup').find('#hdnReOrderExecuteFunctionString').val('_CreateWO.OpenItemPopup()');
        $('#ItemModelTemp').load(strUrl, new function () {
            $('#ItemModelTemp').dialog('open').dialog("moveToTop");
            //style = "top:0;left:0;right:0;bottom:0"
        });
        if ($("input#global_filter").length > 0) {
            $("input#global_filter").focus();
            setTimeout(function () {
                $("input#global_filter").focus();
            }, 600);
        }
        if ($("input.searchinput").length > 0) {
            $("input.searchinput").focus();
            setTimeout(function () {
                $("input.searchinput").focus();
            }, 600);

        }
    }
    //return false;
}


// ************ Tool Popup ********************//


function OpenToolPopup() {
    if (parseInt(_CreateWO.workOrderID, 10) > 0) {
        ClearVariables();
        var strUrl = _CreateWO.urls.LoadToolsOnModelUrl;
        strUrl = strUrl + '?ParentGuid=' + _CreateWO.GUID;
        $('#divToolModel').load(strUrl, new function () {
            $('#divToolModel').dialog('open');
        })
    }
}

function ajaxFileUpload1(Response, idValue) {
    //starting setting some animation when the ajax starts and completes
    $("#loading")
        .ajaxStart(function () {
            $(this).show();
        })
        .ajaxComplete(function () {
            $(this).hide();
        });
    $('#DivLoading').show();
    $.ajaxFileUpload
        (
            {
                url: '/api/fileupload/PostWOSignatureFile/' + Response.ID,
                secureuri: false,
                type: "POST",
                fileElementId: 'WOSignatureImage',
                dataType: 'json',
                async: false,
                success: function (data, status) {
                    return true;
                },
                error: function (data, status, e) {
                    return false;
                }
            }
        )
    return true;
}

function readURL(input) {

    if (input.files && input.files[0]) {

        var isError = false;
        var objFile = input.files[0];

        for (var n = 0; n < specialKeys.length; n++) {
            if (objFile.name.toString().lastIndexOf(String.fromCharCode(specialKeys[n])) >= 0) {
                isError = true;
                break;
            }
        }

        var WOSignatureAllowedFileExtension = ".jpg,.jpeg,.png";//$("#hdWorkOrderAllowedFileExtension").val();
        var validExts = WOSignatureAllowedFileExtension.split(',');
        var strValidationMessage = "";
        var IsValidFile = true;
        var fileExt = objFile.name;
        if ($("#WOSignatureImagenofile").length > 0) {
            $("#WOSignatureImagenofile").html(fileExt);
        }
        fileExt = fileExt.substring(fileExt.lastIndexOf('.'));

        if (validExts.indexOf(fileExt.toLowerCase()) <= -1) {
            IsValidFile = false;
            strValidationMessage = strValidationMessage + objFile.name + " " + MsgInvalidFileSelected + MsgvalidFileList.replace("{0}", validExts.toString());
        }

        if (isError == true || !IsValidFile ) {
            $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
            var msg = strValidationMessage != "" && strValidationMessage.length > 0 ? strValidationMessage : MsgValidFileName;
            $("#spanGlobalMessage").html(msg);
            $('div#target').fadeToggle();
            //$("div#target").delay(5000).fadeOut(200);
            $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
            $("input#currentpath").val('');
            $("input#WOSignatureImage").val('');
        }
        else {
            var reader = new FileReader();
            reader.onload = function (e) {
                var filePath = objFile.name.toString();//$("#currentpath").val().split('\\').pop();

                if (filePath.toString().indexOf("&") >= 0
                    || filePath.toString().indexOf("<") >= 0
                    || filePath.toString().indexOf(">") >= 0
                    || filePath.toString().indexOf("*") >= 0
                    || filePath.toString().indexOf(":") >= 0
                    || filePath.toString().indexOf("?") >= 0
                    || filePath.toString().indexOf("%") >= 0) {
                    //alert("Please select correct file name.");
                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
                    $("#spanGlobalMessage").html(MsgValidFileName);
                    $('div#target').fadeToggle();
                    //$("div#target").delay(5000).fadeOut(200);
                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);

                    $("input#currentpath").val('');
                    $("input#WOSignatureImage").val('');
                }
                else {
                    $('#previewHolder').attr('src', e.target.result);
                    $("input#currentpath").val(e.target.result);
                    $("#btnDeleteImage").show();
                }
            }

            reader.readAsDataURL(input.files[0]);
        }
    }
}

function DeleteWOSignatureImage() {
    $('#previewHolder').attr('src', '/Content/images/no-image.jpg');
    $("input#currentpath").val('/Content/images/no-image.jpg');
    $("input#WOSignatureImage").val('');
    $("#btnDeleteImage").hide();
    var nofilechoosentext = "No file chosen";
    if (typeof textNofilechosen != 'undefined') {
        nofilechoosentext = textNofilechosen;
    }
    $("#WOSignatureImagenofile").html(nofilechoosentext);
    //ItemImagePath = '';
}

function setImagePath() {
    $('#previewHolder').attr('src', $("input#currentpath").val());
}

function DeleteWOSignature(Id, FileName) {
    
    $.ajax({
        url: 'DeleteWorkorderSignature',
        type: 'Post',
        data: { 'Id': Id, 'FileName': FileName },
        dataType: 'json',
        success: function (response) {
        }
    });
}