
var _ReceiveItemDetail = (function ($) {
    //function () {
    var self = {};

    self.model = { OrderDetailID: null };
    self.urls = { OpenEditReceiveDialogURL: null };

    self.init = function (orderDetailID, openEditReceiveDialogURL) {
        self.model.OrderDetailID = orderDetailID;
        self.urls.OpenEditReceiveDialogURL = openEditReceiveDialogURL;

        self.initEvents();
    }

    self.initEvents = function () {
        $(document).ready(function () {

            if (typeof _ReceiveList !== 'undefined' && _ReceiveList !== null) {
                _ReceiveList.fillReceivedItemDetailGrid(self.model.OrderDetailID);
            }
            else {
                console.log('_ReceiveList not found');
            }

            $('#deleteRows1').attr("onclick", "return deleteRowsReceived('" + self.model.OrderDetailID + "')");
            $('#deleteRows1').attr("id", "deleteRowsRevd_" + self.model.OrderDetailID);

            var $divEditReceive = $('#divEditReceive');
            var $DivLoading = $('#DivLoading');

            $divEditReceive.dialog({
                autoOpen: false,
                modal: true,
                draggable: true,
                resizable: true,
                width: '90%',
                height: 220,
                title: "Edit Received",
                open: function () {
                    $DivLoading.show();
                    var receiveGUID = $(this).data("ReceivedGUID");
                    _AjaxUtil.postText(self.urls.OpenEditReceiveDialogURL
                        , { 'ReceivedGUID': receiveGUID }
                        , function (result) {
                            $DivLoading.hide();
                            $divEditReceive.html(result)
                        }
                        , function (xhr) {
                            $DivLoading.hide();
                        }, false, false)
                    //$.ajax({
                    //    url: self.urls.OpenEditReceiveDialogURL,
                    //    data: { 'ReceivedGUID': receiveGUID },
                    //    type: 'Post',
                    //    "async": false,
                    //    "cache": false,
                    //    "dataType": "text",
                    //    success: function (result) {
                    //        $('#DivLoading').hide();
                    //        $('#divEditReceive').html(result)
                    //    },
                    //    error: function (xhr) {
                    //        $('#DivLoading').hide();
                    //    }

                    //});
                },
                close: function () {
                    $(this).empty();
                    $divEditReceive.empty();
                    if ($(this).data("Success")) {
                        ImageIDToOpen = "#imgPlusMinus_" + self.model.OrderDetailID;
                        $('#myDataTable').dataTable().fnStandingRedraw();
                        $DivLoading.hide();
                    }
                }
            });


            $('#ReceivedItemDetail_' + self.model.OrderDetailID + ' tbody tr').on('click', function (e) {
                var id = $(this).attr('id').replace('tr_', '');
                var td = $(this).find('#tdEDISent_' + id);
                if ($.trim(td[0].innerHTML).toLowerCase() === 'yes') {
                    return false;
                }
                $(this).toggleClass('row_selected');
                return false;
            });

            //$('#btnEditReceive').on('click', function () {
            self.OpenEditReceive = function (obj) {
                var receivedGUID = $(obj).parent().parent().find('#hdnGUID').val();
                $divEditReceive.data({ "ReceivedGUID": receivedGUID }).dialog('open');
                return false;
            }

        }); // ready
    };

    return self;
    }
) (jQuery);

//if (typeof _ReceiveItemDetail !== 'undefined' && _ReceiveItemDetail !== null) {
//    _ReceiveItemDetail.init(orderDetailID, openEditReceiveDialogURL);
//}
//else {
//}