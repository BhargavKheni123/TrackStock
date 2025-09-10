var isDeleteSrLotRow = false;
var IsLoadMoreLotsClicked = false;
var LenBeforeRebind = 0;
var LenAfterRebind = 0;

function PrepareTransferDataTable(objTransferItemDTO) {
    
    var columnarrIL = new Array();
    columnarrIL.push({
        mDataProp: null, sClass: "read_only", sDefaultContent: '', fnRender: function (obj, val) {
            if (objTransferItemDTO.ViewRight == "ViewOverwrite") {
                var strReturn = "<span style='position:relative'>";
                strReturn = strReturn + "<input type='text' value='" + obj.aData.LotOrSerailNumber + "' id='txtLotOrSerailNumber' name='txtLotOrSerailNumber' class='text-boxinner AutoSerialLot' />";
                strReturn = strReturn + '<a id="lnkShowAllOptions" href="javascript:void(0);" style="position:absolute; right:5px; top:0px;" class="ShowAllOptionsSL" ><img src="/Content/images/arrow_down_black.png" alt="select" /></a></span>';
                return strReturn;
            }
            else if (objTransferItemDTO.ViewRight == "NoRight")
            {
                var strReturn = "<span style='position:relative'>";
                strReturn = strReturn + "<input type='text' value='" + obj.aData.LotOrSerailNumber + "' id='txtLotOrSerailNumberNoRight' name='txtLotOrSerailNumberNoRight' class='text-boxinner' />";
                return strReturn;
            }
            else if (objTransferItemDTO.ViewRight == "ViewOnly") {
                var strReturn = "<input type='text' value='" + obj.aData.LotOrSerailNumber + "' id='txtLotOrSerailNumberViewOnly' name='txtLotOrSerailNumberViewOnly' class='text-boxinner' />";
                return strReturn;
            }
            else {
                var strReturn = "<input type='text' value='" + obj.aData.LotOrSerailNumber + "' id='txtLotOrSerailNumberViewOnly' name='txtLotOrSerailNumberViewOnly' class='text-boxinner' />";
                return strReturn;
            }
        }
    });

    columnarrIL.push({
        mDataProp: null, sClass: "read_only", sDefaultContent: '', fnRender: function (obj, val) {
            var strReturn = "<span name='spnBinNumber' id='spnBinNumber_" + obj.aData.ID + "'>" + (obj.aData.BinNumber == "[|EmptyStagingBin|]" ? "" : obj.aData.BinNumber) + "</span>";
            return strReturn;
        }
    });
    columnarrIL.push({
        mDataProp: null, sClass: "read_only", sDefaultContent: '', fnRender: function (obj, val) {
            var strReturn = "<span name='spnLotSerialQuantity' id='spnLotSerialQuantity_" + obj.aData.ID + "'>" + obj.aData.LotSerialQuantity + "</span>";
            return strReturn;
        }
    });
    columnarrIL.push({
        mDataProp: null, sClass: "read_only", sDefaultContent: '', fnRender: function (obj, val) {

            var RequisitionDetailGUID = '';
            if (obj.aData.RequisitionDetailGUID != null && obj.aData.RequisitionDetailGUID != undefined)
                RequisitionDetailGUID = obj.aData.RequisitionDetailGUID;
            var strReturn = "<input type='hidden' name='hdnRowUniqueId' value='" + obj.aData.ID + "_" + obj.aData.ItemGUID + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnLotNumberTracking' value='" + obj.aData.LotNumberTracking + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnSerialNumberTracking' value='" + obj.aData.SerialNumberTracking + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnDateCodeTracking' value='" + obj.aData.DateCodeTracking + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnExpiration' value='" + obj.aData.Expiration + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnExpirationDate' value='" + obj.aData.strExpirationDate + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnBinNumber' value='" + obj.aData.BinNumber + "' />";
            strReturn = strReturn + "<input type='hidden' name='hdnItemNumber' value='" + obj.aData.ItemNumber + "' />";

            if (objTransferItemDTO.SerialNumberTracking == BoolTrueString) {
                strReturn = strReturn + "<input type='text' value='" + FormatedCostQtyValues(obj.aData.PullQuantity, 2) + "' id='txtTransferQty_" + obj.aData.ID + "' name='txtTransferQty' class='text-boxinner pull-quantity' readonly='readonly' />";
            }
            else {
                strReturn = strReturn + "<input type='text' value='" + FormatedCostQtyValues(obj.aData.PullQuantity, 2) + "' id='txtTransferQty_" + obj.aData.ID + "' name='txtTransferQty' class='text-boxinner pull-quantity' />";
            }
            return strReturn;
        }
    });
    columnarrIL.push({ mDataProp: "Received", sClass: "read_only" });
    columnarrIL.push({ mDataProp: "Expiration", sClass: "read_only" });

    var Curtable = $('#' + objTransferItemDTO.tableID).dataTable({
        "bPaginate": false,
        "bLengthChange": false,
        "bFilter": false,
        "bSort": false,
        "bInfo": false,
        "bAutoWidth": false,
        "sScrollX": "100%",
        "bRetrieve": true,
        "bDestroy": true,
        "bProcessing": true,
        "bServerSide": true,
        "aoColumns": columnarrIL,
        "sAjaxSource": urlTransferLotSrSelection,
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            if (aData.IsConsignedLotSerial == true) {
                nRow.className = "even trconsigned";
            }
        },
        "fnInitComplete": function (oSettings) {
            var strAllSelected = "";

            $("#hdnSelectedId_" + objTransferItemDTO.ItemGUID + "_" + objTransferItemDTO.RequisitionDetailGUID).val();
            if (objTransferItemDTO.LotNumberTracking != BoolTrueString && objTransferItemDTO.SerialNumberTracking != BoolTrueString) {
                $('#' + objTransferItemDTO.tableID).dataTable().fnSetColumnVis(0, false);
            }
            if (objTransferItemDTO.DateCodeTracking != BoolTrueString) {
                $('#' + objTransferItemDTO.tableID).dataTable().fnSetColumnVis(5, false);
            }
        },
        "fnServerData": function (sSource, aoData, fnCallback, oSettings) {
            aoData.push({ "name": "ItemGUID", "value": objTransferItemDTO.ItemGUID });
            aoData.push({ "name": "BinID", "value": objTransferItemDTO.BinID });

            if (objTransferItemDTO.ItemGUID != '00000000-0000-0000-0000-000000000000' && objTransferItemDTO.ItemGUID != '')
                aoData.push({ "name": "TransferQuantity", "value": FormatedCostQtyValues($("#txtTransferQuantity_" + objTransferItemDTO.ItemGUID + "_" + objTransferItemDTO.RequisitionDetailGUID).val(), 2) });
            else
                aoData.push({ "name": "TransferQuantity", "value": FormatedCostQtyValues($("#txtTransferQuantity_" + objTransferItemDTO.ToolGUID + "_" + objTransferItemDTO.RequisitionDetailGUID).val(), 2) });

            aoData.push({ "name": "CurrentLoaded", "value": $("#hdnCurrentLoadedIds_" + objTransferItemDTO.ItemGUID + "_" + objTransferItemDTO.RequisitionDetailGUID).val() });
            aoData.push({ "name": "ViewRight", "value": objTransferItemDTO.ViewRight });
            aoData.push({ "name": "IsDeleteRowMode", "value": isDeleteSrLotRow });
            oSettings.jqXHR = $.ajax({
                dataType: 'json',
                type: "POST",
                url: sSource,
                cache: false,
                data: aoData,
                headers: { "__RequestVerificationToken": $("input[name='__RequestVerificationToken'][type='hidden']").val() },
                success: fnCallback,
                beforeSend: function () {
                    LenBeforeRebind = $('#' + objTransferItemDTO.tableID).find("tbody").find("tr").length;
                    $('.dataTables_scroll').css({ "opacity": 0.2 });
                },
                complete: function () {
                    $('.dataTables_scroll').css({ "opacity": 1 });
                    isDeleteSrLotRow = false;
                    $('.ShowAllOptionsSL').click(function () {
                        $(this).siblings('.AutoSerialLot').trigger("focus");
                        $(this).siblings(".AutoSerialLot").autocomplete("search", "");
                    });

                    if (objTransferItemDTO.ViewRight == "ViewOnly") {
                        $("input[type='text'][name='txtLotOrSerailNumberViewOnly']").keypress(function () {
                            return false;
                        });

                        $("#DivTransferSelection input[type='text'][name='txtTransferQty']").keypress(function () {
                            return false;
                        });
                    }

                    LenAfterRebind = $('#' + objTransferItemDTO.tableID).find("tbody").find("tr").length;
                    if (LenBeforeRebind == LenAfterRebind && IsLoadMoreLotsClicked == true) {
                        alert(MsgNoLocationToAdd);
                    }
                    IsLoadMoreLotsClicked = false;
                }
            });
        }
    });
}

function IsLotSerialExistsInCurrentLoaded(strIds, SerialLot) {
    if (SerialLot.trim() == '')
        return true;

    if (strIds.trim() == '')
        return false

    var ArrIds = strIds.split(',');
    var i = 0;
    for (i = 0; i < ArrIds.length; i++) {
        if (ArrIds[i].split('_')[0].toLowerCase() == SerialLot.toLowerCase()) {
            return true;
        }
    }

    return false;
}

$(document).ready(function () {
    if (rights == "ViewOnly") {
        $("input[type='text'][name^='txtTransferQuantity_']").keypress(function () {
            return false;
        });
    }

    $("#DivTransferSelection").off('change', "input[type='text'][name^='txtLotOrSerailNumber']");
    $("#DivTransferSelection").on('change', "input[type='text'][name^='txtLotOrSerailNumber']", function (e) {
          var objCurtxt = $(this);
          var oldValue = $(objCurtxt).val();
          var ids = $(this).parent().parent().parent().parent().parent().parent().parent().parent().parent().find("[id^='hdnPullIds_']").val().split('_');
          var aPos = $("#tblItemTransfer_" + ids[1].toString() + "_" + ids[2].toString()).dataTable().fnGetPosition($(this).parent().parent().parent()[0]);
          var aData = $("#tblItemTransfer_" + ids[1].toString() + "_" + ids[2].toString()).dataTable().fnGetData(aPos);

          var dtThisItem = $("#tblItemTransfer_" + ids[1].toString() + "_" + ids[2].toString()).dataTable();
          var currentTR = $(objCurtxt).parent().parent().parent()[0];
          var row_id = dtThisItem.fnGetPosition(currentTR);
          if ($.trim(oldValue) == '')
          {
              var itemLocationLotSerialForBlankSerialLot = JSON.parse($("#hdnItemLocationLotSerialDTO").val());
              var obj = itemLocationLotSerialForBlankSerialLot;
              obj.BinID = aData.BinID;
              obj.ID = aData.BinID;
              obj.ItemGuid = ids[1].toString();
              obj.LotOrSerailNumber = "";
              obj.Expiration = "";
              obj.BinNumber = "";
              dtThisItem.fnUpdate(obj, row_id, undefined, false, false);
              return;              
          }

          var isDuplicateEntry = false;
          var OtherPullQuantity = 0;
          $("#tblItemTransfer_" + ids[1].toString() + "_" + ids[2].toString() + " tbody tr").each(function (i) {
              if (i != row_id) {
                  var tr = $(this);
                  var SerialOrLotNumber = $(tr).find('#' + objCurtxt.prop("id")).val();
                  if (SerialOrLotNumber == $(objCurtxt).val()) {
                      isDuplicateEntry = true;
                  }
                  else {
                      var txtTransferQty = $(tr).find("input[type='text'][name='txtTransferQty']").val();
                      OtherPullQuantity = OtherPullQuantity + parseFloat(txtTransferQty);
                  }
              }
          });

          if (isDuplicateEntry == true) {

              if ($("#hdnTrackingType_" + ids[1].toString() + "_" + ids[2].toString()).val() == "LotNumberTracking")
                  alert(MsgDuplicateLotNumber);
              else if ($("#hdnTrackingType_" + ids[1].toString() + "_" + ids[2].toString()).val() == "SerialNumberTracking")
                  alert(MsgDuplicateSerialNumberValidation);
              else
                  alert(DuplicateNumber);

              $(objCurtxt).val("");
              $(objCurtxt).focus();
          }
          else {
              $.ajax({
                  type: "POST",
                  url: ValidateSerialLotNumberUrl,
                  contentType: 'application/json',
                  dataType: 'json',
                  data: "{ ItemGuid: '" + ids[1].toString() + "', SerialOrLotNumber: '" + $.trim($(objCurtxt).val()) + "',BinID: '" + aData.BinID + "' }",
                  success: function (RetData) {
                      if (RetData.ID > 0) {
                          IsCheckViewRight = false;

                          var spnPoolQuantity = parseFloat($("#txtTransferQuantity_" + ids[1].toString() + "_" + ids[2].toString()).val());
                          if ((spnPoolQuantity - OtherPullQuantity) > 0) {
                              if ((spnPoolQuantity - OtherPullQuantity) < RetData.PullQuantity)
                                  RetData.PullQuantity = spnPoolQuantity - OtherPullQuantity;
                          }
                          else {
                              RetData.PullQuantity = 0;
                          }

                          dtThisItem.fnUpdate(RetData, row_id, undefined, false, false);
                          IsCheckViewRight = true;

                          $('.ShowAllOptionsSL').click(function () {
                              $(this).siblings('.AutoSerialLot').trigger("focus");
                              $(this).siblings(".AutoSerialLot").autocomplete("search", "");
                          });

                          if (RetData.IsConsignedLotSerial) {
                              $(currentTR).addClass("trconsigned");
                          }
                          else {
                              $(currentTR).removeClass("trconsigned");
                          }
                      }
                      else {
                          $(objCurtxt).val("");
                          $(objCurtxt).focus();
                      }
                  },
                  error: function (err) {
                      console.log(err);
                  }
              });
          }
      });

    $("#DivTransferSelection").off('focus', "input[type='text'][name^='txtLotOrSerailNumber']");
    $("#DivTransferSelection").on('focus', "input[type='text'][name^='txtLotOrSerailNumber']", function (e) {
          var objCurtxt = $(this);
          var ids = $(this).parent().parent().parent().parent().parent().parent().parent().parent().parent().find("[id^='hdnPullIds_']").val().split('_');
          var aPos = $("#tblItemTransfer_" + ids[1].toString() + "_" + ids[2].toString()).dataTable().fnGetPosition($(this).parent().parent().parent()[0]);
          var aData = $("#tblItemTransfer_" + ids[1].toString() + "_" + ids[2].toString()).dataTable().fnGetData(aPos);

          var dtItemPull = "#tblItemTransfer_" + ids[1].toString() + "_" + ids[2].toString();
          var strSerialLotNos = "";

          $(dtItemPull).find("tbody").find("tr").each(function (index, tr) {

              if (index != aPos) {
                  var hdnLotNumberTracking = $(tr).find("input[name='hdnLotNumberTracking']").val();
                  var hdnSerialNumberTracking = $(tr).find("input[name='hdnSerialNumberTracking']").val();
                  var hdnDateCodeTracking = $(tr).find("input[name='hdnDateCodeTracking']").val();

                  if (hdnLotNumberTracking == "true" || hdnSerialNumberTracking == "true") {
                      var txtLotOrSerailNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                      if (txtLotOrSerailNumber != undefined)
                          strSerialLotNos = strSerialLotNos + txtLotOrSerailNumber + "|#|";
                  }
                  else if (hdnDateCodeTracking == "true") {
                      var hdnExpiration = $(tr).find("input[name='hdnExpiration']").val();
                      if (hdnExpiration != undefined)
                          strSerialLotNos = strSerialLotNos + hdnExpiration + "|#|";
                  }
                  else {
                      var hdnBinNumber = $(tr).find("input[name='hdnBinNumber']").val();
                      if (hdnBinNumber != undefined)
                          strSerialLotNos = strSerialLotNos + hdnBinNumber + "|#|";
                  }
              }

          });

          if ($(this).hasClass("AutoSerialLot")) {
              $(this).autocomplete({
                  source: function (request, response) {
                      $.ajax({
                          url: '/Pull/GetLotOrSerailNumberList',
                          contentType: 'application/json',
                          dataType: 'json',
                          data: {
                              maxRows: 1000,
                              name_startsWith: request.term,
                              ItemGuid: ids[1].toString(),
                              BinID: aData.BinID,
                              prmSerialLotNos: strSerialLotNos
                          },
                          success: function (data) {
                              response($.map(data, function (item) {
                                  return {
                                      label: item.LotOrSerailNumber,
                                      value: item.LotOrSerailNumber,
                                      selval: item.LotOrSerailNumber
                                  }
                              }));
                          },
                          error: function (err) {

                          }
                      });
                  },
                  autoFocus: false,
                  minLength: 0,
                  select: function (event, ui) {                      
                  },
                  open: function () {
                      $(this).removeClass("ui-corner-all").addClass("ui-corner-top");
                      $(this).autocomplete('widget').css('z-index', 9000);
                      $('ul.ui-autocomplete').css('overflow-y', 'auto');
                      $('ul.ui-autocomplete').css('max-height', '300px');
                  },
                  close: function () {
                      $(this).removeClass("ui-corner-top").addClass("ui-corner-all");
                      $(objCurtxt).trigger("change");
                  }
              });
          }
      });

    $("#DivTransferSelection").off("change", "input[type='text'][name='txtTransferQty']");
    $("#DivTransferSelection").on("change", "input[type='text'][name='txtTransferQty']", function () {
          var ids = $(this).parent().parent().parent().parent().parent().parent().parent().parent().find("[id^='hdnPullIds_']").val().split('_');
          var aPos = $("#tblItemTransfer_" + ids[1].toString() + "_" + ids[2].toString()).dataTable().fnGetPosition($(this).parent().parent()[0]);
          $("#tblItemTransfer_" + ids[1].toString() + "_" + ids[2].toString()).dataTable().fnGetData(aPos).PullQuantity = $(this).val();
    });

    $("#DivTransferSelection").off("click", "input[type='button'][name='btnLoadMoreLots']");
    $("#DivTransferSelection").on("click", "input[type='button'][name='btnLoadMoreLots']", function () {
        var vItemGUID = $(this).prop("id").split('_')[1];
        var vRequisitionDetailGUID = $(this).prop("id").split('_')[2];

        var dtID = "#tblItemTransfer_" + vItemGUID + "_" + vRequisitionDetailGUID;
        var strIds = "";

        var MaxQuantity = $("#txtTransferQuantity_" + vItemGUID + "_" + vRequisitionDetailGUID)[0].value;
        var TotalQuantity = 0;
        $("#tblItemTransfer_" + vItemGUID + "_" + vRequisitionDetailGUID).find("[id*='txtTransferQty_']").each(function () {
            TotalQuantity = TotalQuantity + parseInt($(this)[0].value);
        });

        if (MaxQuantity > TotalQuantity) {
            IsLoadMoreLotsClicked = true;
            $(dtID).find("tbody").find("tr").each(function (index, tr) {

                var hdnLotNumberTracking = $(tr).find("input[name='hdnLotNumberTracking']").val();
                var hdnSerialNumberTracking = $(tr).find("input[name='hdnSerialNumberTracking']").val();
                var hdnDateCodeTracking = $(tr).find("input[name='hdnDateCodeTracking']").val();
                var txtTransferQty = $(tr).find("input[type='text'][name='txtTransferQty']").val();

                if (txtTransferQty != undefined) {
                    if (txtTransferQty == "") {
                        txtTransferQty = "0";
                    }
                    if ((hdnLotNumberTracking == "true" && hdnDateCodeTracking == "false")
                        || (hdnSerialNumberTracking == "true" && hdnDateCodeTracking == "false")) {
                        var txtLotOrSerailNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                        if (txtLotOrSerailNumber != undefined && !IsLotSerialExistsInCurrentLoaded(strIds, txtLotOrSerailNumber))
                            strIds = strIds + txtLotOrSerailNumber + "_" + txtTransferQty + ",";
                    }
                    else if ((hdnLotNumberTracking == "true" && hdnDateCodeTracking == "true")
                        || (hdnSerialNumberTracking == "true" && hdnDateCodeTracking == "true")) {
                        var hdnExpiration = $(tr).find("input[name='hdnExpirationDate']").val();
                        var txtLotOrSerailNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                        if (txtLotOrSerailNumber != undefined && hdnExpiration != undefined && !IsLotSerialExistsInCurrentLoaded(strIds, hdnExpiration))
                            strIds = strIds + txtLotOrSerailNumber + "_" + hdnExpiration + "_" + txtTransferQty + ",";
                    }
                    else if (hdnLotNumberTracking == "false" && hdnSerialNumberTracking == "false" && hdnDateCodeTracking == "true") {
                        var hdnExpiration = $(tr).find("input[name='hdnExpirationDate']").val();
                        if (hdnExpiration != undefined)
                            strIds = strIds + hdnExpiration + "_" + txtTransferQty + ",";
                    }
                    else {
                        var hdnBinNumber = $(tr).find("input[name='hdnBinNumber']").val();
                        if (hdnBinNumber != undefined && !IsLotSerialExistsInCurrentLoaded(strIds, hdnBinNumber))
                            strIds = strIds + hdnBinNumber + "_" + txtTransferQty + ",";
                    }
                }
            });

            $("#hdnCurrentLoadedIds_" + vItemGUID + "_" + vRequisitionDetailGUID).val(strIds);

            var dt = $(dtID).dataTable();
            dt.fnStandingRedraw();
        }
        else {
            alert(MsgNewRowTransferQuantityValidation);
        }
    });

    $("#DivTransferSelection").off("click", "input[type='button'][name='btnTransferPopup']");
    $("#DivTransferSelection").on("click", "input[type='button'][name='btnTransferPopup']", function () {
        $('#DivLoading').show();
        var errorMsg = '';
        var iserror = 0;
        var ErrorSrNo = 0;
        var vItemGUID = $(this).prop("id").split('_')[1];
        var vRequisitionDetailGUID = $(this).prop("id").split('_')[2];
        var dtID = "#tblItemTransfer_" + vItemGUID + "_" + vRequisitionDetailGUID;
        var ArrItem = new Array();
        var arrItemDetails;
        var ErrorMessage = ValidateSingleTransfer(vItemGUID, vRequisitionDetailGUID);

        if (ErrorMessage == "") {
            arrItemDetails = new Array();
            var ID = vItemGUID;
            var SpanQty = $("#DivTransferSelection").find("#txtTransferQuantity_" + vItemGUID + "_" + vRequisitionDetailGUID);
            var dt = $("#tblItemTransfer_" + vItemGUID + "_" + vRequisitionDetailGUID).dataTable();
            var currentData = dt.fnGetData();
            var strtransferobj = JSON.parse($("#hdnTransferMasterDTO_" + vItemGUID + "_" + vRequisitionDetailGUID).val());
            $("#tblItemTransfer_" + vItemGUID + "_" + vRequisitionDetailGUID).find("tbody").find("tr").each(function (index, tr) {
                var txtTransferQty = $(tr).find("input[type='text'][name='txtTransferQty']").val();
                var hdnLotNumberTracking = $(tr).find("input[name='hdnLotNumberTracking']").val();
                var hdnSerialNumberTracking = $(tr).find("input[name='hdnSerialNumberTracking']").val();
                var hdnDateCodeTracking = $(tr).find("input[name='hdnDateCodeTracking']").val();
                var hdnBinNumber = $(tr).find("input[name='hdnBinNumber']").val();
                var hdnExpiration = $(tr).find("input[name='hdnExpiration']").val();

                if (txtTransferQty != "") {
                    var txtLotOrSerailNumber = "";
                    if (hdnLotNumberTracking == "true" || hdnSerialNumberTracking == "true")
                        var txtLotOrSerailNumber = $(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val();

                    var vSerialNumber = "";
                    var vLotNumber = "";
                    var vExpiration = "";

                    if (hdnSerialNumberTracking == "true")
                        vSerialNumber = txtLotOrSerailNumber;
                    if (hdnLotNumberTracking == "true")
                        vLotNumber = txtLotOrSerailNumber;
                    if (hdnDateCodeTracking == "true")
                        vExpiration = hdnExpiration;

                    var obj = {
                        "LotOrSerailNumber": txtLotOrSerailNumber, "BinNumber": hdnBinNumber, "PullQuantity": parseFloat(txtTransferQty.toString())
                                    , "LotNumberTracking": hdnLotNumberTracking, "SerialNumberTracking": hdnSerialNumberTracking, "DateCodeTracking": hdnDateCodeTracking
                                    , "Expiration": hdnExpiration, "SerialNumber": $.trim(vSerialNumber), "LotNumber": $.trim(vLotNumber)
                                    , "ItemGUID": strtransferobj.ItemGUID, "BinID": strtransferobj.BinID, "ID": strtransferobj.BinID
                    };

                    arrItemDetails.push(obj);
                }
            });

            if (arrItemDetails.length > 0) {                
                var qty = parseFloat($(SpanQty).val().toString());
                var receiveqty = parseFloat(strtransferobj.ReceivedQuantity);
                var inTransintqty = parseFloat(strtransferobj.IntransitQuantity);
                var qty = qty - inTransintqty;
                var arrLocWiseQty = new Array();
                var objLocWiseQty = { 'LocationID': 0, 'Quantity': qty };
                arrLocWiseQty.push(objLocWiseQty);

                var objMoveQty = { 'BinWiseQty': arrLocWiseQty, 'TotalQty': qty, 'ItemGUID': strtransferobj.ItemGUID, 'DetailGUID': strtransferobj.GUID, 'ReceivedQty': receiveqty, 'ItemTransferDetails': arrItemDetails };
                var obj = { 'objMoveQty': objMoveQty, 'InventoryConsuptionMethod': $("#hdnInventoryConsuptionMethod").val() };

                $.ajax({
                    url: 'ApproveQuantityToInTransit',
                    data: JSON.stringify(obj),
                    type: 'POST',
                    async: false,
                    contentType: "application/json",
                    dataType: "json",
                    success: function (response) {
                        if (response.Status === "ok") {
                            ErrorSrNo = parseInt(ErrorSrNo) + 1;
                            errorMsg = errorMsg + '<b style="color: Green;">' + ErrorSrNo + ') ' + strtransferobj.ItemGUID + ': ' + MsgTransferedSuccess +'</b> <br />';
                        }
                        else {
                            iserror = 1;
                            ErrorSrNo = parseInt(ErrorSrNo) + 1;
                            errorMsg = errorMsg + '<b style="color: Olive;">' + ErrorSrNo + ') ' + strtransferobj.ItemGUID + ': ' + response.Message + ' </b> <br />';

                        }
                    },
                    error: function (xhr) {
                        iserror = 1;
                        ErrorSrNo = parseInt(ErrorSrNo) + 1;
                        errorMsg = errorMsg + '<b style="color: Olive;">' + ErrorSrNo + ') ' + strtransferobj.ItemGUID + ': ' + ErrorInProcess  +' </b> <br />';
                    }
                });

                setTimeout(function () {
                    $('#DivLoading').hide();
                    if (iserror >= 1) {
                        var requestTypeElement = $("#hdnRequestType");
                        closeMessageModalInPopup();
                        errorMsg = '<b>' + MsgApprovedReasons +'</b><br />' + errorMsg;
                        $('#TrnReceivedInfoDialog').find("#TrnReceivedMSG").html(errorMsg);
                        $('#TrnReceivedInfoDialog').modal();
                        $("#DivTransferSelection").empty();
                        $('#DivTransferSelection').dialog('close');
                        var transferId = window.parent.$("#hdnTransferId").val();
                        var editLink = window.parent.$("a.editTrfLink[data-id=" + transferId + "]")
                        if ((editLink === undefined || editLink == null || editLink.length < 1)) {
                            if (requestTypeElement !== undefined && requestTypeElement.val() !== undefined && requestTypeElement.val() != null && requestTypeElement.val().length > 0 && requestTypeElement.val() == "1") {
                                ShowTransferOutEditTab("TransferEdit/" + transferId, "frmTransfer", transferId);
                            }
                        }
                        else {
                            window.parent.$("a.editTrfLink[data-id=" + transferId + "]").trigger('click');
                        }
                    }
                    else {
                        var requestTypeElement = $("#hdnRequestType");
                        closeMessageModalInPopup();
                        errorMsg = '<b>Transfered successfully</b><br />'
                        $('#TrnReceivedInfoDialog').find("#TrnReceivedMSG").html(errorMsg);
                        $('#TrnReceivedInfoDialog').modal();
                        $("#DivTransferSelection").empty();
                        $('#DivTransferSelection').dialog('close');                       
                        var transferId = window.parent.$("#hdnTransferId").val();
                        var editLink = window.parent.$("a.editTrfLink[data-id=" + transferId + "]")
                        if ((editLink === undefined || editLink == null || editLink.length < 1)) {
                            if (requestTypeElement !== undefined && requestTypeElement.val() !== undefined && requestTypeElement.val() != null && requestTypeElement.val().length > 0 && requestTypeElement.val() == "1") {
                                ShowTransferOutEditTab("TransferEdit/" + transferId, "frmTransfer", transferId);
                            }
                        }
                        else {
                            window.parent.$("a.editTrfLink[data-id=" + transferId + "]").trigger('click');
                        }
                    }
                }, 1000);
            }
        }
        else {
            $('#DivLoading').hide();
            alert(ErrorMessage);
        }
    });
    
    $("#DivTransferSelection").off("click", "input[type='button'][name='btnTransferAllPopUp']");
    $("#DivTransferSelection").on("click", "input[type='button'][name='btnTransferAllPopUp']", function () {
        TransferAllNewFlow();
    });

    $("#DivTransferSelection").off("click", "input[type='button'][name='btnCancelTransferPopup']");
    $("#DivTransferSelection").on("click", "input[type='button'][name='btnCancelTransferPopup']", function () {
        $("#DivTransferSelection").empty();
        $('#DivTransferSelection').dialog('close');
    });

    $("#DivTransferSelection").off("tap click", ".tbl-item-pull tbody tr");
    $("#DivTransferSelection").on("tap click", ".tbl-item-pull tbody tr", function (e) {
        if (e.target.type == "checkbox" || e.target.type == "radio" || e.target.type == "text") {
            e.stopPropagation();
        }
        else if (e.currentTarget.getElementsByTagName("input").btnLoad != undefined) {
            e.stopPropagation();
        }
        else {
            if ((e.metaKey || e.ctrlKey)) {
                $(this).toggleClass('row_selected');
            } else {
                $(this).toggleClass('row_selected');
            }
        }
        return false;
    });

    $("#DivTransferSelection").off('click', "input[type='button'][name='btnDeleteLots']");
    $("#DivTransferSelection").on('click', "input[type='button'][name='btnDeleteLots']", function (e) {

        var vItemGUID = $(this).prop("id").split('_')[1];
        var vRequisitionDetailGUID = $(this).prop("id").split('_')[2];
        var dtID = "#tblItemTransfer_" + vItemGUID + "_" + vRequisitionDetailGUID;
        var TotalRows = $(dtID + ' tbody tr').length;
        var SelectedRows = $(dtID + ' tbody tr.row_selected').length;
        var RemainingRows = TotalRows - SelectedRows;

        if (SelectedRows <= 0) {
            alert(MsgSelectRowToDelete);
        }
        else {
            if (RemainingRows >= 1) {
                $(dtID).find("tbody").find("tr.row_selected").each(function (index, tr) {
                    $(tr).remove();
                });

                var strIds = "";
                $(dtID).find("tbody").find("tr").each(function (index, tr) {

                    var hdnLotNumberTracking = $(tr).find("input[name='hdnLotNumberTracking']").val();
                    var hdnSerialNumberTracking = $(tr).find("input[name='hdnSerialNumberTracking']").val();
                    var hdnDateCodeTracking = $(tr).find("input[name='hdnDateCodeTracking']").val();
                    var txtTransferQty = $(tr).find("input[type='text'][name='txtTransferQty']").val();

                    if (txtTransferQty == "")
                        txtTransferQty = "0";

                    if ((hdnLotNumberTracking == "true" && hdnDateCodeTracking == "false")
                        || (hdnSerialNumberTracking == "true" && hdnDateCodeTracking == "false")) {
                        var txtLotOrSerailNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                        if (txtLotOrSerailNumber != undefined && !IsLotSerialExistsInCurrentLoaded(strIds, txtLotOrSerailNumber))
                            strIds = strIds + txtLotOrSerailNumber + "_" + txtTransferQty + ",";
                    }
                    else if ((hdnLotNumberTracking == "true" && hdnDateCodeTracking == "true")
                        || (hdnSerialNumberTracking == "true" && hdnDateCodeTracking == "true")) {
                        var hdnExpiration = $(tr).find("input[name='hdnExpirationDate']").val();
                        var txtLotOrSerailNumber = $.trim($(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val());
                        if (txtLotOrSerailNumber != undefined && hdnExpiration != undefined && !IsLotSerialExistsInCurrentLoaded(strIds, hdnExpiration))
                            strIds = strIds + txtLotOrSerailNumber + "_" + hdnExpiration + "_" + txtTransferQty + ",";
                    }
                    else if (hdnLotNumberTracking == "false" && hdnSerialNumberTracking == "false" && hdnDateCodeTracking == "true") {
                        var hdnExpiration = $(tr).find("input[name='hdnExpirationDate']").val();
                        if (hdnExpiration != undefined)
                            strIds = strIds + hdnExpiration + "_" + txtTransferQty + ",";
                    }
                    else {
                        var hdnBinNumber = $(tr).find("input[name='hdnBinNumber']").val();
                        if (hdnBinNumber != undefined && !IsLotSerialExistsInCurrentLoaded(strIds, hdnBinNumber))
                            strIds = strIds + hdnBinNumber + "_" + txtTransferQty + ",";
                    }

                });

                $("#hdnCurrentLoadedIds_" + vItemGUID + "_" + vRequisitionDetailGUID).val(strIds);
                isDeleteSrLotRow = true;
                var dtThisItem = $(dtID).dataTable();
                dtThisItem.fnStandingRedraw();
            }
            else {
                alert(MsgRowShouldExists);
            }

        }
    });

});


function closeMessageModalInPopup() {
    $.modal.impl.close();
}

function ValidateSingleTransfer(vItemGUID, RequisitionDetailGUID) {

    var returnVal = true;
    var errormsg = "";
    var isMoreQty = false;
    var dtID = "#spnLotSerialQuantity" + vItemGUID + "_" + RequisitionDetailGUID;

    var SpanQty = $("#DivTransferSelection").find("#txtTransferQuantity_" + vItemGUID + "_" + RequisitionDetailGUID);

    var TotalEntered = 0;
    $("#tblItemTransfer_" + vItemGUID + "_" + RequisitionDetailGUID).find("tbody").find("tr").each(function (index, tr) {
        var txtTransferQty = $(tr).find("input[type='text'][name='txtTransferQty']").val();
        var spnLotSerialQuantity = $(tr).find("span[name='spnLotSerialQuantity']").text();

        if (parseFloat(txtTransferQty) > parseFloat(spnLotSerialQuantity)) {
            //errormsg = "\nYou can not transfer more QTY than available QTY.";
            //isMoreQty = true;
            //return errormsg;
        }

        TotalEntered = TotalEntered + parseFloat(txtTransferQty);
    });

    if (isMoreQty == false) {
        var transferQty = parseFloat($(SpanQty).val().toString());
        if (TotalEntered != transferQty) {
            //errormsg = errormsg + "\n You have entered :" + TotalEntered + " QTY. You had entered Transfered Qty :" + transferQty;
        }
    }
    else {
        //errormsg = "You can not transfer more QTY than available QTY.";
    }

    return errormsg;
}

function TransferAllNewFlow() {
    $('#DivLoading').show();
    var errorMsg = '';
    var iserror = 0;
    var ErrorSrNo = 0;
    var arrItemDetails;
    var ErrorMessage = ValidateAllTransfer();

    if (ErrorMessage == "") {
        $("#DivTransferSelection").find("table[id^='tblItemTransferheader']").each(function (indx, tblHeader) {
            var strtransferobj = JSON.parse($(tblHeader).find("input[name='hdnTransferMasterDTO']").val());
            arrItemDetails = new Array();
            var ID = $(tblHeader).prop("id").split('_')[1];
            var vToolGUID = strtransferobj.ToolGUID;
            var RequisitionDetailGUID = $(tblHeader).prop("id").split('_')[2];
            var SpanQty = 0;
            var itemQuantity = 0;
            var itemNumber = "";

            if (ID != '00000000-0000-0000-0000-000000000000')
                SpanQty = $(tblHeader).find("#txtTransferQuantity_" + ID + "_" + RequisitionDetailGUID);
            else
                SpanQty = $(tblHeader).find("#txtTransferQuantity_" + vToolGUID + "_" + RequisitionDetailGUID);

            var dt = null;

            if ($("#tblItemTransfer_" + ID + "_" + RequisitionDetailGUID).length > 0)
                dt = $("#tblItemTransfer_" + ID + "_" + RequisitionDetailGUID).dataTable();

            if ($("#tblItemTransfer_" + ID + "_" + RequisitionDetailGUID).length > 0) {
                var currentData = dt.fnGetData();
                $("#tblItemTransfer_" + ID + "_" + RequisitionDetailGUID).find("tbody").find("tr").each(function (index, tr) {
                    var txtTransferQty = $(tr).find("input[type='text'][name='txtTransferQty']").val();
                    var tmpItemNumber = strtransferobj.ItemNumber;
                    if (typeof(txtTransferQty) !== "undefined") {
                        var hdnLotNumberTracking = $(tr).find("input[name='hdnLotNumberTracking']").val();
                        var hdnSerialNumberTracking = $(tr).find("input[name='hdnSerialNumberTracking']").val();
                        var hdnDateCodeTracking = $(tr).find("input[name='hdnDateCodeTracking']").val();
                        var hdnBinNumber = $(tr).find("input[name='hdnBinNumber']").val();
                        var hdnExpiration = $(tr).find("input[name='hdnExpiration']").val();
                        
                        if (txtTransferQty != "") {
                            itemQuantity += parseFloat(txtTransferQty.toString());
                            var txtLotOrSerailNumber = "";
                            if (hdnLotNumberTracking == "true" || hdnSerialNumberTracking == "true")
                                var txtLotOrSerailNumber = $(tr).find("input[type='text'][name^='txtLotOrSerailNumber']").val();

                            var vSerialNumber = "";
                            var vLotNumber = "";
                            var vExpiration = "";

                            if (hdnSerialNumberTracking == "true")
                                vSerialNumber = $.trim(txtLotOrSerailNumber);
                            if (hdnLotNumberTracking == "true")
                                vLotNumber = $.trim(txtLotOrSerailNumber);
                            if (hdnDateCodeTracking == "true")
                                vExpiration = hdnExpiration;

                            var obj = {
                                "LotOrSerailNumber": $.trim(txtLotOrSerailNumber), "BinNumber": hdnBinNumber, "PullQuantity": parseFloat(txtTransferQty.toString())
                                , "LotNumberTracking": hdnLotNumberTracking, "SerialNumberTracking": hdnSerialNumberTracking, "DateCodeTracking": hdnDateCodeTracking
                                , "Expiration": hdnExpiration, "SerialNumber": $.trim(vSerialNumber), "LotNumber": vLotNumber
                                , "ItemGUID": strtransferobj.ItemGUID, "BinID": strtransferobj.BinID, "ID": strtransferobj.BinID
                            };

                            arrItemDetails.push(obj);
                        }
                    }
                    else
                    {
                        iserror = 1;
                        ErrorSrNo = parseInt(ErrorSrNo) + 1;
                        errorMsg = errorMsg + '<b style="color: Olive;">' + ErrorSrNo + ') ' + tmpItemNumber + ': ' + MsgQuantityNotAvailableToTransfer +  '</b> <br />';
                    }
                });
                if (arrItemDetails.length > 0) {

                    var qty = itemQuantity; //var qty = parseFloat($(SpanQty).val().toString());
                    var receiveqty = parseFloat(strtransferobj.ReceivedQuantity);
                    //var inTransintqty = parseFloat(strtransferobj.IntransitQuantity);
                    //var qty = qty - inTransintqty;

                    var arrLocWiseQty = new Array();
                    var objLocWiseQty = { 'LocationID': 0, 'Quantity': qty };
                    arrLocWiseQty.push(objLocWiseQty);

                    var objMoveQty = { 'BinWiseQty': arrLocWiseQty, 'TotalQty': qty, 'ItemGUID': strtransferobj.ItemGUID, 'DetailGUID': strtransferobj.GUID, 'ReceivedQty': receiveqty, 'ItemTransferDetails': arrItemDetails };
                    var obj = { 'objMoveQty': objMoveQty, 'InventoryConsuptionMethod': $("#hdnInventoryConsuptionMethod").val() };

                    $.ajax({
                        url: 'ApproveQuantityToInTransit',
                        data: JSON.stringify(obj),
                        type: 'POST',
                        async: false,
                        contentType: "application/json",
                        dataType: "json",
                        success: function (response) {
                            if (response.Status === "ok") {
                                ErrorSrNo = parseInt(ErrorSrNo) + 1;
                                errorMsg = errorMsg + '<b style="color: Green;">' + ErrorSrNo + ') ' + itemNumber + ': Transfered Successfully. </b> <br />';
                            }
                            else {
                                iserror = 1;
                                ErrorSrNo = parseInt(ErrorSrNo) + 1;
                                errorMsg = errorMsg + '<b style="color: Olive;">' + ErrorSrNo + ') ' + itemNumber + ': ' + response.Message + ' </b> <br />';

                            }
                        },
                        error: function (xhr) {
                            iserror = 1;
                            ErrorSrNo = parseInt(ErrorSrNo) + 1;
                            errorMsg = errorMsg + '<b style="color: Olive;">' + ErrorSrNo + ') ' + itemNumber + ': Internal Server Error. </b> <br />';
                        }
                    });
                }
            }
        });
        setTimeout(function () {
            $('#DivLoading').hide();
            if (iserror >= 1) {
                var requestTypeElement = $("#hdnRequestType");
                closeMessageModalInPopup();
                errorMsg = '<b>' + MsgApprovedReasons +'</b><br />' + errorMsg;
                $('#TrnReceivedInfoDialog').find("#TrnReceivedMSG").html(errorMsg);
                $('#TrnReceivedInfoDialog').modal();
                $("#DivTransferSelection").empty();
                $('#DivTransferSelection').dialog('close');
                var transferId = window.parent.$("#hdnTransferId").val();
                var editLink = window.parent.$("a.editTrfLink[data-id=" + transferId + "]")
                if ((editLink === undefined || editLink == null || editLink.length < 1)) {
                    if (requestTypeElement !== undefined && requestTypeElement.val() !== undefined && requestTypeElement.val() != null && requestTypeElement.val().length > 0 && requestTypeElement.val() == "1") {
                        ShowTransferOutEditTab("TransferEdit/" + transferId, "frmTransfer", transferId);
                    }
                }
                else {
                    window.parent.$("a.editTrfLink[data-id=" + transferId + "]").trigger('click');
                }
            }
            else {
                var requestTypeElement = $("#hdnRequestType");
                closeMessageModalInPopup();
                errorMsg = '<b>' + MsgTransferedSuccess +'</b><br />'
                $('#TrnReceivedInfoDialog').find("#TrnReceivedMSG").html(errorMsg);
                $('#TrnReceivedInfoDialog').modal();
                $("#DivTransferSelection").empty();
                $('#DivTransferSelection').dialog('close');
                var transferId = window.parent.$("#hdnTransferId").val();
                var editLink = window.parent.$("a.editTrfLink[data-id=" + transferId + "]")
                if ((editLink === undefined || editLink == null || editLink.length < 1)) {
                    if (requestTypeElement !== undefined && requestTypeElement.val() !== undefined && requestTypeElement.val() != null && requestTypeElement.val().length > 0 && requestTypeElement.val() == "1") {
                        ShowTransferOutEditTab("TransferEdit/" + transferId, "frmTransfer", transferId);
                    }
                }
                else {
                    window.parent.$("a.editTrfLink[data-id=" + transferId + "]").trigger('click');
                }
            }
        }, 1000);
    }
    else {
        $('#DivLoading').hide();
        $('#dlgCommonErrorMsgPopup').find("#pOkbtn").css('display', '');
        $('#dlgCommonErrorMsgPopup').find("#pErrMessage").html(ErrorMessage);
        $('#dlgCommonErrorMsgPopup').modal();
        $('#dlgCommonErrorMsgPopup').css("z-index", "1104");
        $('#simplemodal-overlay').css("z-index", "1103");
        $('#simplemodal-container').css("z-index", "1104");
        //alert(ErrorMessage);
    }
}

function ValidateAllTransfer() {
    var returnVal = true;
    var errormsg = "";
    
    $("#DivTransferSelection").find("table[id^='tblItemTransferheader']").each(function (indx, tblHeader) {
        var isMoreQty = false;
        var ID = $(tblHeader).prop("id").split('_')[1];
        var RequisitionDetailGUID = $(tblHeader).prop("id").split('_')[2];
        var SpanQty = $(tblHeader).find("#txtTransferQuantity_" + ID + "_" + RequisitionDetailGUID);
        var itemName = $($(tblHeader).find("#spnItemNumber_" + ID + "_" + RequisitionDetailGUID)).text();    
        var TotalEntered = 0;

        if ($("#tblItemTransfer_" + ID + "_" + RequisitionDetailGUID).length > 0) {
            $("#tblItemTransfer_" + ID + "_" + RequisitionDetailGUID).find("tbody").find("tr").each(function (index, tr) {
                if ($(tr).find("input[type='text'][name='txtTransferQty']").length > 0) {
                    var txtTransferQty = $(tr).find("input[type='text'][name='txtTransferQty']").val();
                    var spnLotSerialQuantity = $(tr).find("span[name='spnLotSerialQuantity']").text();

                    if (txtTransferQty < 1)
                    {
                        errormsg += "<br>" + MsgQtyItemValidation.replace("{0}", itemName);
                    }

                    if (parseFloat(txtTransferQty) > parseFloat(spnLotSerialQuantity)) {
                        errormsg += "<br>" + MsgQtyLocationLotValidation.replace("{0}", itemName);
                        isMoreQty = true;
                        return errormsg;
                    }

                    TotalEntered = TotalEntered + parseFloat(txtTransferQty);
                }
            });
            if (isMoreQty == false) {
                var transferQty = parseFloat($(SpanQty).val().toString());
                if (TotalEntered > transferQty)
                {
                    errormsg += "<br>" + MsgTransferQtyValidation.replace("{0}", itemName);
                }
                
                if (TotalEntered != transferQty) {
                    //errormsg = errormsg + "\n You have entered :" + TotalEntered + " QTY. You had entered Transfered Qty :" + transferQty;
                }
            }
            //else {
                //errormsg += "<br> You can not transfer more QTY than available QTY for Item: " + itemName + ".";
            //}
        }
    });

    return errormsg;
}
