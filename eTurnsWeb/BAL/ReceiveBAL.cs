using eTurns.DAL;
using eTurns.DTO;
using eTurnsWeb.Controllers;
using eTurnsWeb.Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;

namespace eTurnsWeb.BAL
{
    public class SaveReceiveInfo
    {
        public bool Status { get; set; }
        public string Message { get; set; }

        public List<ReceiveErrors> Errors { get; set; }

        public int OrderStatus { get; set; }
        public List<ReceivedOrderOrderDetails> OrderDetailReceivedGuids   { get; set; }
    }


    public class ReceiveBAL : IDisposable
    {
        #region Properties
        private char[] commaTrim = { ',' };
        public Int64 RoomID { get; set; }
        public Int64 CompanyID { get; set; }
        public long UserID { get; set; }

        public long EnterpriceID { get; set; }

        public string EnterpriceDBName { get; set; }

        public string RoomDateFormat { get; set; }

        public CultureInfo RoomCulture { get; set; }

        public string UserName { get; set; }

        public string RoomName { get; set; }

        public TimeZoneInfo CurrentTimeZone { get; set; }
        public string RoomTimeFormat { get; set; }

        #endregion

        #region Contructor
        public ReceiveBAL()
        {
            this.RoomID = SessionHelper.RoomID;
            this.CompanyID = SessionHelper.CompanyID;
            this.UserID = SessionHelper.UserID;
            this.EnterpriceID = SessionHelper.EnterPriceID;
            this.EnterpriceDBName = SessionHelper.EnterPriseDBName;
            this.RoomDateFormat = SessionHelper.RoomDateFormat;
            this.RoomCulture = SessionHelper.RoomCulture;
            this.UserName = SessionHelper.UserName;
            this.RoomName = SessionHelper.RoomName;

            this.CurrentTimeZone = SessionHelper.CurrentTimeZone;
            this.RoomTimeFormat = SessionHelper.RoomTimeFormat;
        }

        #endregion

        public SaveReceiveInfo SaveReceiveInformation(List<ToFillPreReceiveDTO> SavePreReceiveData)
        {

            OrderMasterDAL orderDAL = null;
            OrderDetailsDAL orderDetailDAL = null;
            OrderMasterDTO orderDTO = null;
            OrderDetailsDTO orderDetailDTO = null;
            //List<ItemLocationDetailsDTO> lstROTD = null;
            List<ReceivedOrderTransferDetailDTO> lstROTD = null;
            List<MaterialStagingPullDetailDTO> lstMSPD = null;
            List<ReceiveErrors> listReceiveErrors = null;
            ReceiveErrors receiveError = null;
            List<Guid> listOrderGuids = null;
            int ordStatus = 0;
            InventoryController objInvCtrl = null;
            listReceiveErrors = new List<ReceiveErrors>();
            ItemMasterDAL ItemDAL = null;
            List<ReceivedOrderOrderDetails> receivedOrderTransferDetails = new List<ReceivedOrderOrderDetails>();
            try
            {

                if (SavePreReceiveData == null || SavePreReceiveData.Count <= 0)
                {
                    receiveError = new ReceiveErrors()
                    {
                        ErrorColor = string.Empty,
                        ErrorMassage = ResReceiveOrderDetails.MsgSelectDataToReceive,
                        ErrorTitle = "",
                        OrderDetailGuid = Guid.Empty,
                    };
                    listReceiveErrors.Add(receiveError);

                    return new SaveReceiveInfo() { Status = false, Message = "Error", Errors = listReceiveErrors };
                    //return Json(new { Status = false, Message = "Error", Errors = listReceiveErrors }, JsonRequestBehavior.AllowGet);
                }


                // lstROTD = new List<ItemLocationDetailsDTO>();
                lstROTD = new List<ReceivedOrderTransferDetailDTO>();
                lstMSPD = new List<MaterialStagingPullDetailDTO>();
                orderDAL = new OrderMasterDAL(this.EnterpriceDBName);
                orderDetailDAL = new OrderDetailsDAL(this.EnterpriceDBName);
                ItemDAL = new ItemMasterDAL(this.EnterpriceDBName);
                listOrderGuids = new List<Guid>();
                List<ToFillPreReceiveDTO> lstOrdRecSerial = null;
                var enterpriseId = SessionHelper.EnterPriceID;

                lstOrdRecSerial = (from t in SavePreReceiveData
                                   select new ToFillPreReceiveDTO
                                   {
                                       ItemGUID = t.ItemGUID
                                   }).OrderBy(x => x.ItemNumber).GroupBy(x => new { x.ItemGUID })
                                   .Select(x => new ToFillPreReceiveDTO
                                   {
                                       ItemGUID = x.Key.ItemGUID,
                                       TotalRecord = x.Count()
                                   }).ToList();
                //}
                foreach (var item in SavePreReceiveData)
                {
                    orderDetailDTO = orderDetailDAL.GetOrderDetailByGuidFull(item.OrderDetailGUID, RoomID, CompanyID);
                    orderDTO = orderDAL.GetOrderByGuidPlain(orderDetailDTO.OrderGUID.GetValueOrDefault(Guid.Empty));

                    if (orderDetailDTO.IsCloseItem.GetValueOrDefault(false))
                    {
                        AddReceiveError(ref listReceiveErrors, item.OrderDetailGUID, ref orderDTO, ref orderDetailDTO, (" " + ResReceiveOrderDetails.MsgOrderLineItemClosed));
                        continue;
                    }
                    if (orderDTO.OrderStatus == (int)OrderStatus.Closed)
                    {
                        AddReceiveError(ref listReceiveErrors, item.OrderDetailGUID, ref orderDTO, ref orderDetailDTO, (" " + ResReceiveOrderDetails.MsgOrderClosed));
                        continue;
                    }
                    if (string.IsNullOrEmpty(item.BinNumber))
                    {
                        AddReceiveError(ref listReceiveErrors, item.OrderDetailGUID, ref orderDTO, ref orderDetailDTO, (" " + ResReceiveOrderDetails.MsgBinNumberMandatory));
                        continue;
                    }

                    if (orderDetailDTO.IsPackslipMandatoryAtReceive && string.IsNullOrEmpty(item.PackSlipNumber))
                    {
                        AddReceiveError(ref listReceiveErrors, item.OrderDetailGUID, ref orderDTO, ref orderDetailDTO, (" " + ResReceiveOrderDetails.MsgPackslipMandatory));
                        continue;
                    }

                    string udfError = ValidateReceiveUDF(item);
                    if (!string.IsNullOrEmpty(udfError))
                    {
                        AddReceiveError(ref listReceiveErrors, item.OrderDetailGUID, ref orderDTO, ref orderDetailDTO, " " + udfError);
                        continue;
                    }


                    if (!(item.MakePreReceiveDetail != null && item.MakePreReceiveDetail.Count > 0))
                    {
                        AddReceiveError(ref listReceiveErrors, item.OrderDetailGUID, ref orderDTO, ref orderDetailDTO, (" " + ResReceiveOrderDetails.MsgEnterDataToReceive));
                        continue;
                    }
                    else
                    {
                        foreach (var innerItem in item.MakePreReceiveDetail)
                        {
                            if (innerItem.Quantity <= 0)
                            {
                                AddReceiveError(ref listReceiveErrors, item.OrderDetailGUID, ref orderDTO, ref orderDetailDTO, (" " + ResReceiveOrderDetails.MsgEnterQtyToReceive));
                                continue;
                            }

                            if (orderDetailDTO.SerialNumberTracking)
                            {
                                listReceiveErrors = ValidateSerials(listReceiveErrors, orderDTO, orderDetailDTO, innerItem, item);
                                //if (lstROTD != null && lstROTD.Select(x => x.SerialNumber.Trim()).Contains(innerItem.SerialNumber.Trim())
                                if (lstROTD != null && lstROTD.Where(x => x.SerialNumber.Trim() == innerItem.SerialNumber.Trim() && x.ItemGUID.GetValueOrDefault(Guid.Empty) == item.ItemGUID).Count() > 0)
                                {
                                    AddReceiveError(ref listReceiveErrors, ref orderDTO, ref orderDetailDTO, eTurns.DTO.Resources.ResMessage.MsgDuplicateSerialFound);
                                }

                                if (lstMSPD != null && lstMSPD.Select(x => x.SerialNumber).Contains(innerItem.SerialNumber))
                                {
                                    AddReceiveError(ref listReceiveErrors, ref orderDTO, ref orderDetailDTO, eTurns.DTO.Resources.ResMessage.MsgDuplicateSerialFound);
                                }

                                ItemMasterDTO IMDTO = ItemDAL.GetItemByGuidNormal(item.ItemGUID, this.RoomID, this.CompanyID);
                                if (IMDTO != null && (IMDTO.OrderUOMValue == null || IMDTO.OrderUOMValue <= 0))
                                    IMDTO.OrderUOMValue = 1;

                                ToFillPreReceiveDTO objToFillPreReceiveDTO = lstOrdRecSerial.Where(x => x.ItemGUID == item.ItemGUID).FirstOrDefault();
                                if (objToFillPreReceiveDTO != null && IMDTO.IsAllowOrderCostuom)
                                {
                                    double TotReceived = orderDetailDTO.ReceivedQuantity.GetValueOrDefault(0);
                                    double TotToFillPreReceive = objToFillPreReceiveDTO.TotalRecord.GetValueOrDefault(0);
                                    //if ((objToFillPreReceiveDTO.TotalRecord % IMDTO.OrderUOMValue) != 0)
                                    if (((TotReceived + TotToFillPreReceive) % IMDTO.OrderUOMValue) != 0)
                                    {
                                        AddReceiveError(ref listReceiveErrors, ref orderDTO, ref orderDetailDTO, ResReceiveOrderDetails.MsgQtyReceiveEqualsOrderUOM);
                                    }
                                    //else if ((TotReceived + TotToFillPreReceive) > orderDetailDTO.ApprovedQuantity.GetValueOrDefault(0))
                                    //{
                                    //    receiveError = new ReceiveErrors()
                                    //    {
                                    //        ErrorColor = "Red",
                                    //        ErrorMassage = "qty not receive more then approved qty.",
                                    //        ErrorTitle = orderDTO.OrderNumber + "-" + orderDTO.ReleaseNumber + "-" + orderDetailDTO.ItemNumber,
                                    //        OrderDetailGuid = orderDetailDTO.GUID,
                                    //    };

                                    //    listReceiveErrors.Add(receiveError);
                                    //}

                                }
                            }

                            else if (orderDetailDTO.LotNumberTracking && orderDetailDTO.DateCodeTracking)
                            {
                                listReceiveErrors = ValidateLotAndDateCodes(listReceiveErrors, orderDTO, orderDetailDTO, innerItem, item);
                            }
                            else if (orderDetailDTO.LotNumberTracking)
                            {
                                listReceiveErrors = ValidateLots(listReceiveErrors, orderDTO, orderDetailDTO, innerItem, item);
                            }

                            if (orderDetailDTO.DateCodeTracking && !orderDetailDTO.LotNumberTracking)
                            {
                                listReceiveErrors = ValidateExpiration(listReceiveErrors, orderDTO, orderDetailDTO, innerItem, item);
                            }

                            if (listReceiveErrors.FindIndex(x => x.OrderDetailGuid == item.OrderDetailGUID) < 0)
                            {
                                if (!orderDTO.MaterialStagingGUID.HasValue)
                                {
                                    //ItemLocationDetailsDTO rotd = GetReceiveOrderTransferDetailDTO(orderDTO, orderDetailDTO, innerItem, item);
                                    ReceivedOrderTransferDetailDTO rotd = GetROTDDTO(orderDTO, orderDetailDTO, innerItem, item);
                                    rotd.PackSlipNumber = item.PackSlipNumber;
                                    rotd.POLineItemNumber = item.POLineItemNumber;
                                    lstROTD.Add(rotd);
                                }
                                else
                                {
                                    ItemLocationDetailsDTO ildd = GetReceiveOrderTransferDetailDTO(orderDTO, orderDetailDTO, innerItem, item);
                                    MaterialStagingPullDetailDTO mspdd = GetMaterialStagingPullDetails(orderDTO, ildd);
                                    ildd.PackSlipNumber = item.PackSlipNumber;
                                    mspdd.PackSlipNumber = item.PackSlipNumber;
                                    lstMSPD.Add(mspdd);
                                }

                                if (listOrderGuids.FindIndex(x => x == orderDTO.GUID) < 0)
                                    listOrderGuids.Add(orderDTO.GUID);
                            }
                        }
                    }

                    orderDetailDTO.Comment = item.Comment;

                    orderDetailDAL.UpdateLineComment(orderDetailDTO, this.UserID);
                }// for loop

                long SessionUserId = this.UserID;
                if (lstROTD != null && lstROTD.Count > 0)
                {
                    //ItemLocationDetailsDAL itmLocDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                    //itmLocDAL.InsertItemLocationDetailsFromRecieve(lstROTD);
                    string receiveGUIDs = string.Empty;
                    ReceivedOrderTransferDetailDAL itmLocDAL = new ReceivedOrderTransferDetailDAL(this.EnterpriceDBName);
                    foreach (var item in lstROTD)
                    {
                        item.OrderStatus = orderDTO.OrderStatus;

                        OrderUOMMasterDAL objOrderUOMDAL = new OrderUOMMasterDAL(this.EnterpriceDBName);
                        ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(this.EnterpriceDBName);
                        ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                        objItemMasterDTO = objItemMasterDAL.GetItemByGuidNormal(item.ItemGUID.GetValueOrDefault(Guid.Empty), this.RoomID, this.CompanyID);
                        if (objItemMasterDTO.SerialNumberTracking == false && objItemMasterDTO.IsAllowOrderCostuom)
                        {
                            if (item.CustomerOwnedQuantity != null && item.CustomerOwnedQuantity >= 0)
                                item.CustomerOwnedQuantity = objOrderUOMDAL.GetOrderUOMQty(objItemMasterDTO, item.CustomerOwnedQuantity);
                            if (item.ConsignedQuantity != null && item.ConsignedQuantity >= 0)
                                item.ConsignedQuantity = objOrderUOMDAL.GetOrderUOMQty(objItemMasterDTO, item.ConsignedQuantity);
                        }

                        //WI-6215//
                        itmLocDAL.InsertReceive(item, SessionUserId, item.IsReceivedCostChange.GetValueOrDefault(false),enterpriseId);

                        OrderDetailsDAL ordDetailDAL = new OrderDetailsDAL(this.EnterpriceDBName);
                        OrderDetailsDTO OrdDetailDTO = ordDetailDAL.GetOrderDetailByGuidFull(item.OrderDetailGUID.GetValueOrDefault(Guid.Empty), item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0));
                        if (OrdDetailDTO.ReceivedQuantity >= OrdDetailDTO.ApprovedQuantity)
                        {
                            OrderController orderController = new OrderController();
                            orderController.CloseOrderDetailLineItems(OrdDetailDTO.ID.ToString(), "Order");
                        }

                        receiveGUIDs += "," + Convert.ToString(item.GUID);
                        ReceivedOrderOrderDetails orderOrderDetails = new ReceivedOrderOrderDetails();
                        orderOrderDetails.OrderDetailsGuid = Convert.ToString(item.OrderDetailGUID);
                        orderOrderDetails.ReceivedDetailGuid = Convert.ToString(item.GUID);
                        receivedOrderTransferDetails.Add(orderOrderDetails); 
                        //QuickBookItemDAL objQBItemDAL = new QuickBookItemDAL(this.EnterpriceDBName);
                        //objQBItemDAL.InsertQuickBookItem(item.ItemGUID.GetValueOrDefault(Guid.Empty), this.EnterpriceID, this.CompanyID, this.RoomID, "Update", false, this.UserID, "Web", null, "Receive");

                    }

                    try
                    {

                        string dataGUIDs = "<DataGuids>" + receiveGUIDs.Trim(commaTrim) + "</DataGuids>";
                        string eventName = "ORECC";
                        string eTurnsScheduleDBName = (Convert.ToString(ConfigurationManager.AppSettings["eTurnsScheduleDBName"]) ?? "eTurnsSchedule");
                        NotificationDAL objNotificationDAL = new NotificationDAL(this.EnterpriceDBName);
                        List<NotificationDTO> lstNotification = objNotificationDAL.GetCurrentNotificationListByEventName(eventName, this.RoomID, this.CompanyID, this.UserID);
                        if (lstNotification != null && lstNotification.Count > 0)
                        {
                            objNotificationDAL.SendMailForImmediate(lstNotification, this.RoomID, this.CompanyID, this.UserID, this.EnterpriceID, eTurnsScheduleDBName, dataGUIDs);
                        }
                    }
                    catch (Exception ex)
                    {

                        CommonUtility.LogError(ex, this.RoomID, this.CompanyID, this.EnterpriceID);
                    }
                }

                if (lstMSPD != null && lstMSPD.Count > 0)
                {
                    objInvCtrl = new InventoryController();
                    foreach (var item in lstMSPD)
                    {
                        List<MaterialStagingPullDetailDTO> mspdDTO = new List<MaterialStagingPullDetailDTO>();
                        mspdDTO.Add(item);
                        objInvCtrl.ItemLocationDetailsSaveForMSCredit(mspdDTO);
                    }

                }

                if (listOrderGuids != null && listOrderGuids.Count > 0)
                {
                    foreach (var ordGuid in listOrderGuids)
                    {
                        ordStatus = (int)orderDetailDAL.UpdateOrderStatusByReceiveNew(ordGuid, this.RoomID, this.CompanyID
                            , this.UserID, true);
                    }
                    //if (listOrderGuids.Count == 1)
                    //{
                    //    orderDTO = orderDAL.GetRecord(listOrderGuids[0], RoomID, CompanyID);
                    //    ordStatus = orderDTO.OrderStatus;
                    //}
                }

                return new SaveReceiveInfo() { Status = true, Message = "", Errors = listReceiveErrors, OrderStatus = ordStatus , OrderDetailReceivedGuids = receivedOrderTransferDetails };
                //return Json(new { Status = true, Message = "", Errors = listReceiveErrors, OrderStatus = ordStatus }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, this.RoomID, this.CompanyID, this.EnterpriceID);
                receiveError = new ReceiveErrors()
                {
                    ErrorColor = "Red",
                    ErrorMassage = ex.Message,
                    ErrorTitle = "Exception",
                    OrderDetailGuid = Guid.Empty,
                };
                listReceiveErrors.Add(receiveError);

                return new SaveReceiveInfo() { Status = false, Message = "Exception", OrderStatus = ordStatus };

            }

        }

        private void AddReceiveError(ref List<ReceiveErrors> listReceiveErrors, Guid orderDetailGUID, ref OrderMasterDTO orderDTO, ref OrderDetailsDTO orderDetailDTO, object p)
        {
            throw new NotImplementedException();
        }

        private void AddReceiveError(ref List<ReceiveErrors> listReceiveErrors, Guid OrderDetailGUID
            , ref OrderMasterDTO orderDTO, ref OrderDetailsDTO orderDetailDTO, string errMessage, string errColor = "Red")
        {
            ReceiveErrors receiveError = listReceiveErrors.FirstOrDefault(x => x.OrderDetailGuid == OrderDetailGUID);
            if (receiveError != null)
            {
                receiveError.ErrorMassage += errMessage;//"Order line item is closed.";
            }
            else
            {
                AddReceiveError(ref listReceiveErrors, ref orderDTO, ref orderDetailDTO, errMessage, errColor);
            }
        }

        private void AddReceiveError(ref List<ReceiveErrors> listReceiveErrors, ref OrderMasterDTO orderDTO,
            ref OrderDetailsDTO orderDetailDTO, string errMessage, string errColor = "Red")
        {
            ReceiveErrors receiveError = new ReceiveErrors()
            {
                ErrorColor = errColor,//"Red",
                ErrorMassage = errMessage.Trim(),//"Order line item is closed.",
                ErrorTitle = orderDTO.OrderNumber + "-" + orderDTO.ReleaseNumber + "-" + orderDetailDTO.ItemNumber,
                OrderDetailGuid = orderDetailDTO.GUID,
            };
            listReceiveErrors.Add(receiveError);
        }

        private MaterialStagingPullDetailDTO GetMaterialStagingPullDetails(OrderMasterDTO orderDTO, ItemLocationDetailsDTO objDTO)
        {
            MaterialStagingPullDetailDTO obj = new MaterialStagingPullDetailDTO()
            {
                OrderDetailGUID = objDTO.OrderDetailGUID,
                BinID = objDTO.BinID,
                ConsignedQuantity = objDTO.ConsignedQuantity,
                CustomerOwnedQuantity = objDTO.CustomerOwnedQuantity,
                LotNumber = objDTO.LotNumber,
                SerialNumber = objDTO.SerialNumber,
                MaterialStagingGUID = orderDTO.MaterialStagingGUID,
                BinNumber = objDTO.BinNumber,
                CompanyID = this.CompanyID,
                ItemCost = objDTO.Cost,
                Created = DateTimeUtility.DateTimeNow,
                CreatedBy = this.UserID,
                CreatedByName = this.UserName,
                Expiration = objDTO.ExpirationDate.HasValue ? objDTO.ExpirationDate.GetValueOrDefault(DateTime.MinValue).ToString(this.RoomDateFormat) : "",
                ID = 0,
                IsArchived = false,
                GUID = Guid.Empty,
                IsDeleted = false,
                ItemGUID = objDTO.ItemGUID.GetValueOrDefault(Guid.Empty),
                ItemNumber = objDTO.ItemNumber,
                LastUpdatedBy = this.UserID,
                Received = objDTO.ReceivedDate.HasValue ? objDTO.ReceivedDate.GetValueOrDefault(DateTime.MinValue).ToString(this.RoomDateFormat) : "",
                Room = this.RoomID,
                RoomName = this.RoomName,
                Updated = DateTimeUtility.DateTimeNow,
                UpdatedByName = this.UserName,
                UDF1 = objDTO.UDF1,
                UDF2 = objDTO.UDF2,
                UDF3 = objDTO.UDF3,
                UDF4 = objDTO.UDF4,
                UDF5 = objDTO.UDF5,
                PackSlipNumber = objDTO.PackSlipNumber,

            };
            return obj;
        }

        private ItemLocationDetailsDTO GetReceiveOrderTransferDetailDTO(OrderMasterDTO orderDTO, OrderDetailsDTO orderDetailDTO, ToFillReceiveDetailDTO innerItem, ToFillPreReceiveDTO item)
        {
            BinMasterDAL binDAL = null;
            binDAL = new BinMasterDAL(this.EnterpriceDBName);

            double? ConsignQty = null;
            double? CustQty = null;

            if (orderDetailDTO.Consignment)
                ConsignQty = innerItem.Quantity;
            else
                CustQty = innerItem.Quantity;


            DateTime? ExpDate = null;
            //DateTime recDate;
            if (!string.IsNullOrEmpty(innerItem.ExpirationDate))
            {
                //ExpDate = DateTime.ParseExact(innerItem.ExpirationDate, SessionHelper.RoomDateFormat, eTurns.DTO.Resources.ResourceHelper.CurrentCult);
                ExpDate = DateTime.ParseExact(innerItem.ExpirationDate, this.RoomDateFormat, this.RoomCulture);
            }

            //if (!string.IsNullOrEmpty(item.ReceivedDate))
            //{
            //    //recDate = DateTime.ParseExact(item.ReceivedDate, SessionHelper.RoomDateFormat, eTurns.DTO.Resources.ResourceHelper.CurrentCult);
            //    recDate = DateTime.ParseExact(item.ReceivedDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture);
            //}
            //else
            //    recDate = DateTimeUtility.DateTimeNow;

            DateTime currentDateAsPerRoom = new RegionSettingDAL(this.EnterpriceDBName).GetCurrentDatetimebyTimeZone(this.RoomID, this.CompanyID, 0);
            DateTime newReceiveTempDate = DateTimeUtility.GetNewReceivedDate(currentDateAsPerRoom, item.ReceivedDate, this.RoomDateFormat, this.RoomCulture, this.CurrentTimeZone, this.RoomTimeFormat);



            ItemLocationDetailsDTO rotd = new ItemLocationDetailsDTO()
            {
                BinID = binDAL.GetOrInsertBinIDByName(item.ItemGUID, item.BinNumber, UserID, RoomID, CompanyID, (orderDTO.MaterialStagingGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)),
                CompanyID = CompanyID,
                Room = RoomID,
                Cost = item.Cost,
                ItemGUID = item.ItemGUID,
                OrderDetailGUID = item.OrderDetailGUID,
                ReceivedDate = newReceiveTempDate, //recDate,
                Action = string.Empty,
                AddedFrom = "Web",
                BinNumber = item.BinNumber,
                ConsignedQuantity = ConsignQty,
                CustomerOwnedQuantity = CustQty,
                ExpirationDate = ExpDate,
                SerialNumber = innerItem.SerialNumber,
                LotNumber = innerItem.LotNumber,
                EditedFrom = "Web",
                CreatedBy = UserID,
                LastUpdatedBy = UserID,
                Created = DateTimeUtility.DateTimeNow,
                Updated = DateTimeUtility.DateTimeNow,
                UDF1 = item.UDF1,
                UDF2 = item.UDF2,
                UDF3 = item.UDF3,
                UDF4 = item.UDF4,
                UDF5 = item.UDF5,
                InitialQuantity = innerItem.Quantity,
                InitialQuantityWeb = innerItem.Quantity,
                ReceivedOn = DateTimeUtility.DateTimeNow,
                ReceivedOnWeb = DateTimeUtility.DateTimeNow,
            };


            return rotd;

        }

        private List<ReceiveErrors> ValidateLotAndDateCodes(List<ReceiveErrors> listReceiveErrors, OrderMasterDTO orderDTO, OrderDetailsDTO orderDetailDTO, ToFillReceiveDetailDTO innerItem, ToFillPreReceiveDTO item)
        {
            CommonDAL cmnDAL = null;
            cmnDAL = new CommonDAL(this.EnterpriceDBName);
            ReceiveErrors receiveError = null;

            if (string.IsNullOrEmpty(innerItem.LotNumber) || string.IsNullOrWhiteSpace(innerItem.ExpirationDate))
            {
                receiveError = listReceiveErrors.FirstOrDefault(x => x.OrderDetailGuid == item.OrderDetailGUID);
                if (receiveError != null)
                {
                    receiveError.ErrorMassage += (" " + ResReceiveOrderDetails.MsgEnterLotAndExpirationToReceive);
                }
                else
                {
                    receiveError = new ReceiveErrors()
                    {
                        ErrorColor = "Red",
                        ErrorMassage = ResReceiveOrderDetails.MsgEnterLotAndExpirationToReceive,
                        ErrorTitle = orderDTO.OrderNumber + "-" + orderDTO.ReleaseNumber + "-" + orderDetailDTO.ItemNumber,
                        OrderDetailGuid = orderDetailDTO.GUID,
                    };

                    listReceiveErrors.Add(receiveError);
                }
            }
            else
            {

                DateTime ExpDate = DateTime.MinValue;
                DateTime.TryParseExact(innerItem.ExpirationDate, this.RoomDateFormat, this.RoomCulture, DateTimeStyles.None, out ExpDate);
                string Expiration = ExpDate.ToString("MM/dd/yyyy");
                if (ExpDate != DateTime.MinValue)
                {
                    string msg = cmnDAL.CheckDuplicateLotAndExpiration(innerItem.LotNumber, Expiration, ExpDate, 0, this.RoomID, this.CompanyID, orderDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty),UserID,EnterpriceID);
                    if (string.IsNullOrWhiteSpace(msg) || (msg ?? string.Empty).ToLower() == "ok")
                    {

                    }
                    else
                    {
                        receiveError = listReceiveErrors.FirstOrDefault(x => x.OrderDetailGuid == item.OrderDetailGUID);
                        if (receiveError != null)
                        {
                            receiveError.ErrorMassage += " " + msg;
                        }
                        else
                        {
                            receiveError = new ReceiveErrors()
                            {
                                ErrorColor = "Red",
                                ErrorMassage = msg,
                                ErrorTitle = orderDTO.OrderNumber + "-" + orderDTO.ReleaseNumber + "-" + orderDetailDTO.ItemNumber,
                                OrderDetailGuid = orderDetailDTO.GUID,
                            };

                            listReceiveErrors.Add(receiveError);
                        }
                    }
                }

            }

            return listReceiveErrors;
        }

        private string ValidateReceiveUDF(ToFillPreReceiveDTO objDTO)
        {
            UDFDAL objUDFApiController = new UDFDAL(this.EnterpriceDBName);
            IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetRequiredUDFsByUDFTableNamePlain("ReceivedOrderTransferDetail", this.RoomID, this.CompanyID);
            string udfRequier = string.Empty;
            
            foreach (var i in DataFromDB)
            {
                    if (i.UDFColumnName == "UDF1"  && string.IsNullOrEmpty(objDTO.UDF1))
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;
                        udfRequier = string.Format(eTurns.DTO.Resources.ResMessage.MsgRequired, i.UDFDisplayColumnName);
                    }
                    else if (i.UDFColumnName == "UDF2"  && string.IsNullOrEmpty(objDTO.UDF2))
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;
                        udfRequier = string.Format(eTurns.DTO.Resources.ResMessage.MsgRequired, i.UDFDisplayColumnName);
                    }
                    else if (i.UDFColumnName == "UDF3"  && string.IsNullOrEmpty(objDTO.UDF3))
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;


                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;
                        udfRequier = string.Format(eTurns.DTO.Resources.ResMessage.MsgRequired, i.UDFDisplayColumnName);
                    }
                    else if (i.UDFColumnName == "UDF4"  && string.IsNullOrEmpty(objDTO.UDF4))
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;
                        udfRequier = string.Format(eTurns.DTO.Resources.ResMessage.MsgRequired, i.UDFDisplayColumnName);
                    }
                    else if (i.UDFColumnName == "UDF5"  && string.IsNullOrEmpty(objDTO.UDF5))
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;
                        udfRequier = string.Format(eTurns.DTO.Resources.ResMessage.MsgRequired, i.UDFDisplayColumnName);
                }

                    if (!string.IsNullOrEmpty(udfRequier))
                        break;
                
            }

            return udfRequier;
        }

        private List<ReceiveErrors> ValidateExpiration(List<ReceiveErrors> listReceiveErrors, OrderMasterDTO orderDTO, OrderDetailsDTO orderDetailDTO, ToFillReceiveDetailDTO innerItem, ToFillPreReceiveDTO item)
        {
            CommonDAL cmnDAL = null;
            cmnDAL = new CommonDAL(this.EnterpriceDBName);
            ReceiveErrors receiveError = null;
            if (orderDetailDTO.DateCodeTracking)
            {
                if (string.IsNullOrEmpty(innerItem.ExpirationDate))
                {
                    receiveError = listReceiveErrors.FirstOrDefault(x => x.OrderDetailGuid == item.OrderDetailGUID);
                    if (receiveError != null)
                    {
                        receiveError.ErrorMassage += (" " + ResReceiveOrderDetails.MsgEnterExpirationToReceive);
                    }
                    else
                    {
                        receiveError = new ReceiveErrors()
                        {
                            ErrorColor = "Red",
                            ErrorMassage = ResReceiveOrderDetails.MsgEnterExpirationToReceive,
                            ErrorTitle = orderDTO.OrderNumber + "-" + orderDTO.ReleaseNumber + "-" + orderDetailDTO.ItemNumber,
                            OrderDetailGuid = orderDetailDTO.GUID,
                        };

                        listReceiveErrors.Add(receiveError);
                    }
                }

            }
            return listReceiveErrors;
        }

        private ReceivedOrderTransferDetailDTO GetROTDDTO(OrderMasterDTO orderDTO, OrderDetailsDTO orderDetailDTO, ToFillReceiveDetailDTO innerItem, ToFillPreReceiveDTO item)
        {
            BinMasterDAL binDAL = null;
            binDAL = new BinMasterDAL(this.EnterpriceDBName);

            double? ConsignQty = null;
            double? CustQty = null;

            if (orderDetailDTO.Consignment)
                ConsignQty = innerItem.Quantity;
            else
                CustQty = innerItem.Quantity;


            DateTime? ExpDate = null;
            //DateTime recDate;
            if (!string.IsNullOrEmpty(innerItem.ExpirationDate))
            {
                //ExpDate = DateTime.ParseExact(innerItem.ExpirationDate, SessionHelper.RoomDateFormat, eTurns.DTO.Resources.ResourceHelper.CurrentCult);
                ExpDate = DateTime.ParseExact(innerItem.ExpirationDate, this.RoomDateFormat, this.RoomCulture);
            }

            //if (!string.IsNullOrEmpty(item.ReceivedDate))
            //{
            //    //recDate = DateTime.ParseExact(item.ReceivedDate, SessionHelper.RoomDateFormat, eTurns.DTO.Resources.ResourceHelper.CurrentCult);
            //    recDate = DateTime.ParseExact(item.ReceivedDate, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture);
            //}
            //else
            //    recDate = DateTimeUtility.DateTimeNow;

            DateTime currentDateAsPerRoom = new RegionSettingDAL(this.EnterpriceDBName).GetCurrentDatetimebyTimeZone(this.RoomID, this.CompanyID, 0);
            DateTime newReceiveTempDate = DateTimeUtility.GetNewReceivedDate(currentDateAsPerRoom, item.ReceivedDate, this.RoomDateFormat, this.RoomCulture, this.CurrentTimeZone, this.RoomTimeFormat);

            ReceivedOrderTransferDetailDTO rotd = new ReceivedOrderTransferDetailDTO()
            {
                BinID = binDAL.GetOrInsertBinIDByName(item.ItemGUID, item.BinNumber, UserID, RoomID, CompanyID, (orderDTO.MaterialStagingGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)),
                CompanyID = CompanyID,
                Room = RoomID,
                Cost = item.Cost,
                ItemGUID = item.ItemGUID,
                OrderDetailGUID = item.OrderDetailGUID,
                ReceivedDate = newReceiveTempDate,//recDate,
                Action = string.Empty,
                AddedFrom = "Web",
                BinNumber = item.BinNumber,
                ConsignedQuantity = ConsignQty,
                CustomerOwnedQuantity = CustQty,
                ExpirationDate = ExpDate,
                SerialNumber = (!string.IsNullOrWhiteSpace(innerItem.SerialNumber)) ? innerItem.SerialNumber.Trim() : string.Empty,
                LotNumber = (!string.IsNullOrWhiteSpace(innerItem.LotNumber)) ? innerItem.LotNumber.Trim() : string.Empty,
                EditedFrom = "Web",
                CreatedBy = UserID,
                LastUpdatedBy = UserID,
                Created = DateTimeUtility.DateTimeNow,
                Updated = DateTimeUtility.DateTimeNow,
                UDF1 = item.UDF1,
                UDF2 = item.UDF2,
                UDF3 = item.UDF3,
                UDF4 = item.UDF4,
                UDF5 = item.UDF5,
                ControlNumber = string.Empty,
                PackSlipNumber = item.PackSlipNumber,
                ReceivedOn = DateTimeUtility.DateTimeNow,
                ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                IsOnlyFromUI = true,
                IsEDISent = false,
                IsReceivedCostChange = item.IsReceivedCostChange,
                OrderDetailTrackingID = item.OrderDetailTrackingID
            };


            return rotd;

        }

        private List<ReceiveErrors> ValidateLots(List<ReceiveErrors> listReceiveErrors, OrderMasterDTO orderDTO, OrderDetailsDTO orderDetailDTO, ToFillReceiveDetailDTO innerItem, ToFillPreReceiveDTO item)
        {
            CommonDAL cmnDAL = null;
            cmnDAL = new CommonDAL(this.EnterpriceDBName);
            ReceiveErrors receiveError = null;

            if (string.IsNullOrEmpty(innerItem.LotNumber))
            {
                receiveError = listReceiveErrors.FirstOrDefault(x => x.OrderDetailGuid == item.OrderDetailGUID);
                if (receiveError != null)
                {
                    receiveError.ErrorMassage += (" "+ ResReceiveOrderDetails.MsgEnterLotToReceive);
                }
                else
                {
                    receiveError = new ReceiveErrors()
                    {
                        ErrorColor = "Red",
                        ErrorMassage = ResReceiveOrderDetails.MsgEnterLotToReceive,
                        ErrorTitle = orderDTO.OrderNumber + "-" + orderDTO.ReleaseNumber + "-" + orderDetailDTO.ItemNumber,
                        OrderDetailGuid = orderDetailDTO.GUID,
                    };

                    listReceiveErrors.Add(receiveError);
                }
            }

            return listReceiveErrors;
        }

        private List<ReceiveErrors> ValidateSerials(List<ReceiveErrors> listReceiveErrors, OrderMasterDTO orderDTO, OrderDetailsDTO orderDetailDTO, ToFillReceiveDetailDTO innerItem, ToFillPreReceiveDTO item)
        {
            CommonDAL cmnDAL = null;
            cmnDAL = new CommonDAL(this.EnterpriceDBName);
            ReceiveErrors receiveError = null;

            if (string.IsNullOrEmpty(innerItem.SerialNumber))
            {
                receiveError = listReceiveErrors.FirstOrDefault(x => x.OrderDetailGuid == item.OrderDetailGUID);
                if (receiveError != null)
                {
                    receiveError.ErrorMassage += (" " + ResReceiveOrderDetails.MsgEnterSerialToReceive);
                }
                else
                {
                    receiveError = new ReceiveErrors()
                    {
                        ErrorColor = "Red",
                        ErrorMassage = ResReceiveOrderDetails.MsgEnterSerialToReceive,
                        ErrorTitle = orderDTO.OrderNumber + "-" + orderDTO.ReleaseNumber + "-" + orderDetailDTO.ItemNumber,
                        OrderDetailGuid = orderDetailDTO.GUID,
                    };

                    listReceiveErrors.Add(receiveError);
                }
            }
            else
            {
                string dupSerial = cmnDAL.CheckDuplicateSerialNumbers(innerItem.SerialNumber, 0, RoomID, CompanyID, item.ItemGUID);
                if (dupSerial == "duplicate" || (item.MakePreReceiveDetail.Count(x => x.SerialNumber == innerItem.SerialNumber) > 1))
                {
                    receiveError = listReceiveErrors.FirstOrDefault(x => x.OrderDetailGuid == item.OrderDetailGUID);
                    if (receiveError != null)
                    {
                        receiveError.ErrorMassage += (" " + eTurns.DTO.Resources.ResMessage.MsgDuplicateSerialFound);
                    }
                    else
                    {
                        receiveError = new ReceiveErrors()
                        {
                            ErrorColor = "Red",
                            ErrorMassage = eTurns.DTO.Resources.ResMessage.MsgDuplicateSerialFound,
                            ErrorTitle = orderDTO.OrderNumber + "-" + orderDTO.ReleaseNumber + "-" + orderDetailDTO.ItemNumber,
                            OrderDetailGuid = orderDetailDTO.GUID,
                        };

                        listReceiveErrors.Add(receiveError);
                    }
                }

            }

            return listReceiveErrors;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ReceiveBAL()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}