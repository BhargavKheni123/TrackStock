var BarcodeSearch_Chars = [];

//--------------------------New Consume Pull--------------------------
//
var BarcodeSearch_CurrentDataTableName = '';
var BarcodeSearch_sAjaxSource = '';
var BarcodeSearch_SearchText = '';
var BarcodeSearch_Location = '';
var BarcodeSearch_Quantity = '';
var BarcodeSearch_IsBarcodeSearch = false;
var BarcodeSearch_Barcode = '';
var BarcodeSearch_Pressed = false;
var BarcodeSearch_SearchTextBoxId = '';

function BindEventsForBarcodeScanning() {
    
    $(window).keypress(function (e) {        
        BarcodeSearch_Chars.push(String.fromCharCode(e.which));

        if (BarcodeSearch_Chars[0] == '#') {
            $('#' + BarcodeSearch_SearchTextBoxId).css('color', '#FFFFFF');
        }

        if (BarcodeSearch_Pressed == false) {
            setTimeout(function () {
                if (BarcodeSearch_Chars.length >= 10 && BarcodeSearch_Chars[0] == '#' && BarcodeSearch_Chars[1] != '#') {
                    BarcodeSearch_Barcode = BarcodeSearch_Chars.join('');
                    $('#' + BarcodeSearch_SearchTextBoxId).val('');
                    SearchByBarcode();
                    ExecuteOnBarcodeSearch();                    
                }
                else {
                    BarcodeSearch_Barcode = '';                    
                }
                BarcodeSearch_Chars = [];
                BarcodeSearch_Pressed = false;
                $('#' + BarcodeSearch_SearchTextBoxId).css('color', '#000000');
            }, 500);
        }
        BarcodeSearch_Pressed = true;
    });
};

function SearchByBarcode() {
    BarcodeSearch_sAjaxSource = $('#' + BarcodeSearch_CurrentDataTableName).dataTable().fnSettings().sAjaxSource;
    BarcodeSearch_IsBarcodeSearch = false;

    if (BarcodeSearch_sAjaxSource == '/Pull/GetItemsModelMethod/') {
        var SearchText = BarcodeSearch_Barcode;
        var HashCount = (SearchText.match(/#/g) || []).length;
        
        if (SearchText.charAt(0) == '#' && SearchText.charAt(1) == 'I' && HashCount == 3) {
            BarcodeSearch_IsBarcodeSearch = true;            
            var SplitText = SearchText.split('#');
            
            //--------Set Search Text
            //
            BarcodeSearch_SearchText = SplitText[1].substring(1, SplitText[1].length);
            $('#' + BarcodeSearch_SearchTextBoxId).val('"' + BarcodeSearch_SearchText + '"');
                        
            //--------Set Location
            //
            BarcodeSearch_Location = SplitText[2].substring(1, SplitText[2].length);            
            
            //--------Set Location
            //
            BarcodeSearch_Quantity = SplitText[3].substring(1, SplitText[3].length);
        }
    }
    //alert('CurrentDataTableName : ' + BarcodeSearch_CurrentDataTableName + ', sAjaxSource : ' + BarcodeSearch_sAjaxSource);
}

function SetPullValuesAfterBarcodeSearch() {
    if (BarcodeSearch_IsBarcodeSearch != null && BarcodeSearch_IsBarcodeSearch != undefined && BarcodeSearch_IsBarcodeSearch == true) {

        if (BarcodeSearch_sAjaxSource == '/Pull/GetItemsModelMethod/') {
            var Tr = $('#' + BarcodeSearch_CurrentDataTableName).find('tbody').children('tr:first');
            Tr.find('#txtQty').val(BarcodeSearch_Quantity);

            //------------------------------------------------------------------------
            //
            var itmGuid = Tr.find('#spnItemGUID').text();
            var stagName = '';
            $.ajax({
                url: '/Pull/GetItemLocationsForNewPullGrid',
                type: 'POST',
                data: JSON.stringify({ 'ItemGuid': itmGuid, 'NameStartWith': BarcodeSearch_Location }),
                contentType: 'application/json',
                dataType: 'json',
                success: function (data) {
                    if (data != null && data != undefined && data.length > 0) {
                        Tr.find('input.AutoPullBins').val(data[0].Value);
                        Tr.find('#BinID').val(data[0].ID);
                        Tr.find('#btnAdd').click();
                    }
                    else {
                        alert(MsgLocationNotFound);
                    }
                },
                error: function (err) {
                    $('#DivLoading').hide();
                }
            });
        }
    }
}

//--------------------------Create Barcode--------------------------
//
var CreateBarcode_ModuleListId = '';
var CreateBarcode_ItemListId = '';
var CreateBarcode_BinListId = '';
var CreateBarcode_BarcodeStringId = '';
var CreateBarcode_btnSaveId = '';
var CreateBarcode_Format1 = new RegExp('^#[a-zA-Z0-9\\ \\s]+@[a-zA-Z0-9\\ \\s]+<[0-9]+$');
var CreateBarcode_Format2 = new RegExp('^#I[a-zA-Z0-9\\ \\s]+#B[a-zA-Z0-9\\ \\s]+#Q[0-9]+$');
var CreateBarcode_Text = '';
var CreateBarcode_ExecuteCompletedEvent = true;
var CreateBarcode_BinBindFromBarcode = false;
var ItemNumber = '';
var BinNumber = '';
var Quantity = '';

function CreateBarcode_BindEventsForBarcodeScanning() {
    
    $(window).unbind("keypress");
    $(window).bind("keypress", function (e) {
        KeyPressEvent(e);
    });

    function KeyPressEvent(e) {
        if (($('#' + CreateBarcode_ModuleListId).text().indexOf('Item') >= 0 || $('#' + CreateBarcode_ModuleListId).text().indexOf('ITEM') >= 0)
                && ($('#' + CreateBarcode_ModuleListId).text().indexOf('Master') >= 0 || $('#' + CreateBarcode_ModuleListId).text().indexOf('Master') >= 0)) {

            BarcodeSearch_Chars.push(String.fromCharCode(e.which));

            if (CreateBarcode_ExecuteCompletedEvent == true) {
                setTimeout(function () {
                    WindowKeyPressCompleted();
                    BarcodeSearch_Chars = [];
                    CreateBarcode_ExecuteCompletedEvent = true;
                }, 1000);
                CreateBarcode_ExecuteCompletedEvent = false;
            }

        }
    }
}

function WindowKeyPressCompleted() {
    CreateBarcode_Text = BarcodeSearch_Chars.join('').trim();
    
    ItemNumber = '';
    BinNumber = '';
    Quantity = '';

    if (CreateBarcode_Format1.test(CreateBarcode_Text)) {
        var CharInd1 = 0;
        var CharInd2 = CreateBarcode_Text.indexOf('@', 0);
        var CharInd3 = CreateBarcode_Text.indexOf('<', 0);

        ItemNumber = CreateBarcode_Text.substring(CharInd1 + 1, CharInd2);
        BinNumber = CreateBarcode_Text.substring(CharInd2 + 1, CharInd3);
        Quantity = CreateBarcode_Text.substring(CharInd3 + 1, CreateBarcode_Text.length);
    }
    else if (CreateBarcode_Format2.test(CreateBarcode_Text)) {
        var ArrStr = CreateBarcode_Text.split('#');
        if (ArrStr.length = 4) {
            ItemNumber = ArrStr[1].substring(1, ArrStr[1].length);
            BinNumber = ArrStr[2].substring(1, ArrStr[2].length);
            Quantity = ArrStr[3].substring(1, ArrStr[3].length);
        }
    }

    if (ItemNumber != null && ItemNumber != undefined && ItemNumber.trim() != ''
        && BinNumber != null && BinNumber != undefined && BinNumber.trim() != ''
        && Quantity != null && Quantity != undefined && Quantity.trim() != '') {

        if ($("#" + CreateBarcode_ItemListId + " option:contains(" + ItemNumber + ")").length > 0) {
            $("#" + CreateBarcode_ItemListId + " option:contains(" + ItemNumber + ")").attr('selected', 'selected');

            var ItemGUID = $("#" + CreateBarcode_ItemListId).val();
            CreateBarcode_BinBindFromBarcode = true;
            getBinList(ItemGUID);

        }
        else {
            $("#" + CreateBarcode_BarcodeStringId).val();
            alert(MsgItemNumberNotAvailable.replace("{0}", ItemNumber));
        }

    }

    window.scrollTo(0, 0);
    $(window).focus();
}

function AssigneScanedBin() {
    if ($("#" + CreateBarcode_BinListId + " option:contains(" + BinNumber + ")").length > 0) {
        $("#" + CreateBarcode_BinListId + " option:contains(" + BinNumber + ")").attr('selected', 'selected');

        var ItemGUID = $("#" + CreateBarcode_ItemListId).val();
        var BinId = $("#" + CreateBarcode_BinListId).val();
        $("#" + CreateBarcode_BarcodeStringId).val(CreateBarcode_Text);

        $('#' + CreateBarcode_btnSaveId).click();
    }
    else {
        $("#" + CreateBarcode_BarcodeStringId).val();
        alert(MsgBinNumberNotAvailable.replace("{0}", BinNumber));
    }

    CreateBarcode_Text = '';
    CreateBarcode_ExecuteCompletedEvent = true;
    CreateBarcode_BinBindFromBarcode = false;
    ItemNumber = '';
    BinNumber = '';
    Quantity = '';
}