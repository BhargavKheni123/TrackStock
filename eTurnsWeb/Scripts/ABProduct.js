$(document).ready(function () {

    $("#AddToItemOrCartQtyPopUp").dialog({
        autoOpen: false,
        show: "blind",
        hide: "explode",
        height: 500,
        title: LblProductQuantityPopUpTitle,
        width: 900,
        modal: true,
        open: function () {
        },
        beforeClose: function () {
        },
        close: function () {
            $('#DivLoading').hide();
            $("#AddToItemOrCartQtyPopUp").empty();
        }
    });

});

function AddtoRoom(asin, Operation, IsExistingItem) {
    
    if (asin != null && asin != "")
    {
        var data = {
            "ASIN": asin,
            "CallFor": Operation,
            "IsExistingItem": IsExistingItem
        };

        $.ajax({
            type: "POST",
            url: "GetItemQuantityPopUp",
            dataType: 'html',
            data: JSON.stringify(data) ,
            async: false,
            //dataType: 'json',
            contentType: "application/json",
            success: function (RetData) {
                $('#DivLoading').hide();
                $.modal.impl.close();
                $("#AddToItemOrCartQtyPopUp").html("").html(RetData)
                $("#AddToItemOrCartQtyPopUp").dialog('open');

            },
            error: function (err) {
                console.log(err);
            }
        });
    }
}

//function AddProductToItemOrCart()
//{
//    var callFor = $("#CallFor").val()

//    if (typeof (callFor) != "undefined" && CallFor != null && CallFor != "")
//    {
//        var obj = { 'ASIN': $("#ASIN").val(), 'CallFor': $("#CallFor").val(), 'CriticalQty': $("#CriticalQty").val(), 'MinimumQty': $("#MinimumQty").val(), 'MaximumQty': $("#MaximumQty").val(), 'CartQty': $("#CartQty").val() };
//    }

//    if (asin != null && asin != "") {
//        $.ajax({
//            "url": "SaveItemToRoom",
//            "type": "POST",
//            data: { Asin: asin },
//            //"async": false,
//            "dataType": "json",
//            "success": function (response) {
//                if (response.Status == "fail") {
//                    $('div#target').fadeToggle();
//                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
//                    $('div#target').css("z-index", 100000);
//                    $("#spanGlobalMessage").html(response.Message);
//                    $("#spanGlobalMessage").removeClass('succesIcon WarningIcon').addClass('errorIcon');
//                }
//                else {
//                    $('div#target').fadeToggle();
//                    $("div#target").delay(DelayTime).fadeOut(FadeOutTime);
//                    $('div#target').css("z-index", 100000);
//                    $("#spanGlobalMessage").html(response.Message);
//                    $("#spanGlobalMessage").removeClass('errorIcon WarningIcon').addClass('succesIcon');
//                    $('a[data-asin="' + $("#ASIN").val() + '"]').hide();
//                }
//            }
//        });
//    }
//}

function ShowProductOffer(asin) {   
    $('#DivLoading').show();
    if (asin != null && asin != "") {
        var data = {
            "asin": asin
        };
        $.ajax({
            type: "POST",
            url: "ShowProductOffers",
            dataType: 'html',
            data: JSON.stringify(data),
            async: false,
            //dataType: 'json',
            contentType: "application/json",
            success: function (RetData) {
                $('#DivLoading').hide();
                if (RetData != null) {
                    $("#divProductOffer").html("").html(RetData)
                    $('body').addClass('filterBlockShown');
                    $("body").append("<div id='overlay'></div>");
                }
                else {
                    $("#overlay").remove("#overlay");
                    $('body').removeClass('filterBlockShown');
                    $("#divProductOffer").html("");                   
                }
            },
            error: function (err) {
                $('#DivLoading').hide();
                console.log(err);
            }
        });
    }
}