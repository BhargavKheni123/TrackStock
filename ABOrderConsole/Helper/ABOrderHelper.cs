using eTurns.DTO;
using eTurnsMaster.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eTurns.DAL;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using System.Web;
using System.Security.Cryptography;
using eTurns.ABAPIBAL;
using eTurns.ABAPIBAL.Helper;
namespace ABOrderConsole.Helper
{
    public class ABOrderHelper
    {
        public void ImportABOrderToLocalDB()
        {
            EnterpriseMasterDAL objEntDal = new EnterpriseMasterDAL();
            List<EnterpriseDTO> enterprises = objEntDal.GetAllEnterprisesPlain().ToList();

            if (enterprises != null && enterprises.Any() && enterprises.Count() > 0)
            {
                foreach (var enterprise in enterprises)
                {
                    var roomMasterDAL = new eTurns.DAL.RoomDAL(enterprise.EnterpriseDBName);
                    var rooms = roomMasterDAL.GetAllABIntegrationRooms();
                    var abOrderDAL = new ABOrderDAL(enterprise.EnterpriseDBName);

                    if (rooms != null && rooms.Any() && rooms.Count > 0)
                    {
                        foreach (var room in rooms)
                        {
                            var endDate = DateTime.UtcNow;
                            //endDate = endDate.AddDays(-1);
                            var startDate = endDate.AddHours(-23);
                            int minutesToAdd = 59 - startDate.Minute;
                            startDate = startDate.AddMinutes(-59);
                            int secondsToAdd = 59 - startDate.Second;
                            startDate = startDate.AddSeconds(-59);

                            var order = ABAPIHelper.GetOrdersByOrderDate(startDate, endDate, true, true, true, null, null, room.CompanyID.GetValueOrDefault(0), room.ID, 0, enterprise.EnterpriseDBName);

                            if (order != null && order.orders != null && order.orders.Any() && order.orders.Count > 0)
                            {
                                foreach (var currentOrder in order.orders)
                                {
                                    var abOrder = GetInsertABOrderDTOFromABOrder(currentOrder, room.ID, room.CompanyID.GetValueOrDefault(0));

                                    if (abOrder != null && !string.IsNullOrEmpty(abOrder.OrderId) && !string.IsNullOrWhiteSpace(abOrder.OrderId))
                                    {
                                        abOrderDAL.InsertABOrder(abOrder);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void ImportABOrderToLocalDB(DateTime StartDate, DateTime EndDate)
        {
            EnterpriseMasterDAL objEntDal = new EnterpriseMasterDAL();
            List<EnterpriseDTO> enterprises = objEntDal.GetAllEnterprisesPlain().ToList();

            if (enterprises != null && enterprises.Any() && enterprises.Count() > 0)
            {
                foreach (var enterprise in enterprises)
                {
                    var roomMasterDAL = new eTurns.DAL.RoomDAL(enterprise.EnterpriseDBName);
                    var rooms = roomMasterDAL.GetAllABIntegrationRooms();
                    var abOrderDAL = new ABOrderDAL(enterprise.EnterpriseDBName);

                    if (rooms != null && rooms.Any() && rooms.Count > 0)
                    {
                        foreach (var room in rooms)
                        {
                            for (var day = StartDate.Date; day.Date <= EndDate.Date; day = day.AddDays(1))
                            {
                                var startDate = day;
                                var endDate = startDate.AddHours(23);
                                endDate = endDate.AddMinutes(59);
                                endDate = endDate.AddSeconds(59);

                                var order = ABAPIHelper.GetOrdersByOrderDate(startDate, endDate, true, true, true, null, null, room.CompanyID.GetValueOrDefault(0), room.ID, 0, enterprise.EnterpriseDBName);

                                if (order != null && order.orders != null && order.orders.Any() && order.orders.Count > 0)
                                {
                                    foreach (var currentOrder in order.orders)
                                    {
                                        var abOrder = GetInsertABOrderDTOFromABOrder(currentOrder, room.ID, room.CompanyID.GetValueOrDefault(0));

                                        if (abOrder != null && !string.IsNullOrEmpty(abOrder.OrderId) && !string.IsNullOrWhiteSpace(abOrder.OrderId))
                                        {
                                            abOrderDAL.InsertABOrder(abOrder);
                                        }
                                    }
                                }

                                if (order != null && !string.IsNullOrEmpty(order.nextPageToken) && !string.IsNullOrWhiteSpace(order.nextPageToken))
                                {
                                    bool isExtraCallRequireForTheCurrentDuration = true;
                                    string nextPageToken = order.nextPageToken;

                                    while (isExtraCallRequireForTheCurrentDuration)
                                    {
                                        var subsequentOrder = ABAPIHelper.GetOrderByNextPageToken(HttpUtility.UrlEncode(nextPageToken), room.CompanyID.GetValueOrDefault(0), room.ID, 0, enterprise.EnterpriseDBName);

                                        if (subsequentOrder != null)
                                        {
                                            isExtraCallRequireForTheCurrentDuration = !string.IsNullOrEmpty(subsequentOrder.nextPageToken) && !string.IsNullOrWhiteSpace(subsequentOrder.nextPageToken);

                                            if (isExtraCallRequireForTheCurrentDuration)
                                            {
                                                nextPageToken = subsequentOrder.nextPageToken;
                                            }

                                            if (subsequentOrder.orders != null && subsequentOrder.orders.Any() && subsequentOrder.orders.Count > 0)
                                            {
                                                foreach (var currentSSOrder in subsequentOrder.orders)
                                                {
                                                    var ssABOrder = GetInsertABOrderDTOFromABOrder(currentSSOrder, room.ID, room.CompanyID.GetValueOrDefault(0));

                                                    if (ssABOrder != null && !string.IsNullOrEmpty(ssABOrder.OrderId) && !string.IsNullOrWhiteSpace(ssABOrder.OrderId))
                                                    {
                                                        abOrderDAL.InsertABOrder(ssABOrder);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            isExtraCallRequireForTheCurrentDuration = false;
                                        }
                                    }
                                }

                            }
                        }

                    }
                }
            }


        }

        public void GetInsertedABOrder()
        {
            var abOrderDAL = new ABOrderDAL("10059_E1770276-DA6A-4C63-8805-9E5F76C86429");
            var order = abOrderDAL.GetABOrderByOrderIdAndRoom("114-7685179-3933033", 140696, 201230170);
            var orderJson = order.OrderJson;

            var decryptedjson = AESEncryptDecrypt.DecryptAes(order.OrderJsonEncrypted);

            if (order.OrderJson == decryptedjson)
            {
                var r = true;
            }
        }
        
        public void SyncABOrderedItemsToRoomItems()
        {
            EnterpriseMasterDAL objEntDal = new EnterpriseMasterDAL();
            List<EnterpriseDTO> enterprises = objEntDal.GetAllEnterprisesPlain().ToList();

            if (enterprises != null && enterprises.Any() && enterprises.Count() > 0)
            {
                long userId = 0;
                string userName = "";

                foreach (var enterprise in enterprises)
                {
                    var enterpriseDBName = enterprise.EnterpriseDBName;
                    var roomMasterDAL = new eTurns.DAL.RoomDAL(enterpriseDBName);
                    var rooms = roomMasterDAL.GetAllABIntegrationRooms();
                    var abOrderDAL = new ABOrderDAL(enterpriseDBName);
                    var productDetailDAL = new ProductDetailsDAL(enterpriseDBName);

                    if (rooms != null && rooms.Any() && rooms.Count > 0)
                    {
                        foreach (var room in rooms)
                        {
                            var roomId = room.ID;
                            var companyId = room.CompanyID.GetValueOrDefault(0);
                            var roomOrders = abOrderDAL.GetABOrdersByRoomId(room.ID,room.CompanyID.GetValueOrDefault(0));

                            if (roomOrders != null && roomOrders.Any() && roomOrders.Count > 0 )
                            {                                
                                var roomItemsASIN = productDetailDAL.GetABItemsAsinByRoomId(room.ID);

                                foreach (var order in roomOrders)
                                {
                                    var encryptedOrderJson = order.OrderJsonEncrypted;
                                    var decryptedOrderJson = AESEncryptDecrypt.DecryptAes(encryptedOrderJson);

                                    if (!string.IsNullOrEmpty(decryptedOrderJson) && !string.IsNullOrWhiteSpace(decryptedOrderJson))
                                    {
                                        var orderObject = JsonConvert.DeserializeObject<Order>(decryptedOrderJson);

                                        if (orderObject != null && orderObject.lineItems != null && orderObject.lineItems.Any() && orderObject.lineItems.Count > 0 )
                                        {
                                            var asins = orderObject.lineItems.Where(e=> !string.IsNullOrEmpty(e.asin) && !string.IsNullOrWhiteSpace(e.asin)).Select(e=> e.asin).Distinct().ToList();

                                            var notInsertedItemsASIN = (roomItemsASIN != null && roomItemsASIN.Any() && roomItemsASIN.Count > 0) ? asins.Except(roomItemsASIN).ToList() : asins;

                                            if (notInsertedItemsASIN != null && notInsertedItemsASIN.Any() && notInsertedItemsASIN.Count() > 0)
                                            {
                                                foreach (var asin in asins)
                                                {
                                                    
                                                    var saveItemRequest = new ProductToItemQtyDTO 
                                                    {
                                                        ASIN = asin.Trim(),
                                                        CallFor = ProductToItemCallFor.AddToItem,
                                                        IsExistingItem = false
                                                    };

                                                    string message = string.Empty;
                                                    var itemSaveStatus = ABAPIHelper.SaveItemToRoom(saveItemRequest,roomId, companyId, userId, userName,enterpriseDBName,enterprise.ID, out message);

                                                    if (!string.IsNullOrEmpty(message) && !string.IsNullOrWhiteSpace(message) && (message.ToLower() == "resabproducts.itemaddtoroomsuccessfully" || message.ToLower() == "resabproducts.asinalreadyexist"))
                                                    {
                                                        if (roomItemsASIN == null)
                                                        {
                                                            roomItemsASIN = new List<string>();                                                                
                                                        }

                                                        roomItemsASIN.Add(asin.Trim());
                                                    }                                                    
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                
            }
        }

        public void SyncABOrderedItemsToRoom()
        {
            EnterpriseMasterDAL objEntDal = new EnterpriseMasterDAL();
            var abOrderDAL = new ABOrderDAL(DbConnectionHelper.GeteTurnsLoggingDBName());
            var syncRequests = abOrderDAL.GetABOrdersToSync();
            List<EnterpriseDTO> lstEnterprise = null;
            
            if (syncRequests != null && syncRequests.Any() && syncRequests.Count > 0)
            {
                List<SyncABOrderRequestDTO> lstOrdersToSync;
                lstEnterprise = objEntDal.GetEnterprisesByIds(string.Join(",", syncRequests.Select(t => t.EnterpriseID).Distinct().ToArray()));

                if (lstEnterprise != null && lstEnterprise.Any() && lstEnterprise.Count > 0)
                {
                    foreach (EnterpriseDTO enterprise in lstEnterprise)
                    {
                        try
                        {
                            lstOrdersToSync = syncRequests.Where(t => t.EnterpriseID == enterprise.ID).ToList();
                            
                            if (lstOrdersToSync != null && lstOrdersToSync.Any() && lstOrdersToSync.Count > 0)
                            {
                                foreach(var syncOrder in lstOrdersToSync)
                                {
                                    try
                                    {
                                        if (syncOrder != null && syncOrder.StartDate != DateTime.MinValue && syncOrder.EndDate != DateTime.MinValue)
                                        {
                                            try
                                            {
                                                ABAPIHelper.ImportABOrderToLocalDB(syncOrder.StartDate, syncOrder.EndDate, syncOrder.CompanyID, syncOrder.RoomID, syncOrder.CreatedBy.GetValueOrDefault(0), enterprise.EnterpriseDBName);
                                                ABAPIHelper.SyncABOrderedItemsToRoomItems(syncOrder.CompanyID, syncOrder.RoomID, syncOrder.CreatedBy.GetValueOrDefault(0), "", enterprise.EnterpriseDBName,enterprise.ID);

                                                abOrderDAL.SetSyncABOrderRequestCompletedWithSuccess(syncOrder.ID);
                                            }
                                            catch (Exception ex)
                                            {
                                                CommonFunctions.SaveExceptionInTextFile("Error in process for SyncABOrderedItemsToRoom fail", ex, GeneralHelper.DoSendLogsInMail);
                                                abOrderDAL.SetSyncABOrderRequestCompletedWithError(syncOrder.ID, (Convert.ToString(ex) ?? "Exception not found"));
                                            }
                                        }
                                        else
                                        {
                                            CommonFunctions.SaveExceptionInTextFile("SyncABOrderedItemsToRoom fail due to invalid data", null, GeneralHelper.DoSendLogsInMail);
                                            abOrderDAL.SetSyncABOrderRequestCompletedWithError(syncOrder.ID, "SyncABOrderedItemsToRoom fail due to invalid data");
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        CommonFunctions.SaveExceptionInTextFile("Error in process for SyncABOrderedItemsToRoom fail", ex, GeneralHelper.DoSendLogsInMail);
                                        abOrderDAL.SetSyncABOrderRequestCompletedWithError(syncOrder.ID, (Convert.ToString(ex) ?? "Exception not found"));
                                    }

                                    #region commented

                                    //if (syncOrder != null && syncOrder.StartDate != DateTime.MinValue && syncOrder.EndDate != DateTime.MinValue)
                                    //{
                                    //    DateTime StartDate = syncOrder.StartDate;
                                    //    DateTime EndDate = syncOrder.EndDate;
                                    //    var enterpriseId = syncOrder.EnterpriseID;
                                    //    var companyId = syncOrder.CompanyID;
                                    //    var roomId = syncOrder.RoomID;

                                    //    for (var day = StartDate.Date; day.Date <= EndDate.Date; day = day.AddDays(1))
                                    //    {
                                    //        var startDate = day;
                                    //        var endDate = startDate.AddHours(23);
                                    //        endDate = endDate.AddMinutes(59);
                                    //        endDate = endDate.AddSeconds(59);

                                    //        var order = ABAPIHelper.GetOrdersByOrderDate(startDate, endDate, true, true, true, null, null, companyId, roomId, 0, enterprise.EnterpriseDBName);

                                    //        if (order != null && order.orders != null && order.orders.Any() && order.orders.Count > 0)
                                    //        {
                                    //            foreach (var currentOrder in order.orders)
                                    //            {
                                    //                var abOrder = GetInsertABOrderDTOFromABOrder(currentOrder, roomId, companyId);

                                    //                if (abOrder != null && !string.IsNullOrEmpty(abOrder.OrderId) && !string.IsNullOrWhiteSpace(abOrder.OrderId))
                                    //                {
                                    //                    abOrderDAL.InsertABOrder(abOrder);
                                    //                }
                                    //            }
                                    //        }

                                    //        if (order != null && !string.IsNullOrEmpty(order.nextPageToken) && !string.IsNullOrWhiteSpace(order.nextPageToken))
                                    //        {
                                    //            bool isExtraCallRequireForTheCurrentDuration = true;
                                    //            string nextPageToken = order.nextPageToken;

                                    //            while (isExtraCallRequireForTheCurrentDuration)
                                    //            {
                                    //                var subsequentOrder = ABAPIHelper.GetOrderByNextPageToken(HttpUtility.UrlEncode(nextPageToken), companyId, roomId, 0, enterprise.EnterpriseDBName);

                                    //                if (subsequentOrder != null)
                                    //                {
                                    //                    isExtraCallRequireForTheCurrentDuration = !string.IsNullOrEmpty(subsequentOrder.nextPageToken) && !string.IsNullOrWhiteSpace(subsequentOrder.nextPageToken);

                                    //                    if (isExtraCallRequireForTheCurrentDuration)
                                    //                    {
                                    //                        nextPageToken = subsequentOrder.nextPageToken;
                                    //                    }

                                    //                    if (subsequentOrder.orders != null && subsequentOrder.orders.Any() && subsequentOrder.orders.Count > 0)
                                    //                    {
                                    //                        foreach (var currentSSOrder in subsequentOrder.orders)
                                    //                        {
                                    //                            var ssABOrder = GetInsertABOrderDTOFromABOrder(currentSSOrder, roomId, companyId);

                                    //                            if (ssABOrder != null && !string.IsNullOrEmpty(ssABOrder.OrderId) && !string.IsNullOrWhiteSpace(ssABOrder.OrderId))
                                    //                            {
                                    //                                abOrderDAL.InsertABOrder(ssABOrder);
                                    //                            }
                                    //                        }
                                    //                    }
                                    //                }
                                    //                else
                                    //                {
                                    //                    isExtraCallRequireForTheCurrentDuration = false;
                                    //                }
                                    //            }
                                    //        }

                                    //    }
                                    //}

                                    #endregion
                                }
                            }
                        }
                        catch(Exception ex)
                        {

                        }
                    }
                }
            }
            
        }

        #region Private Methods

        private InsertABOrderDTO GetInsertABOrderDTOFromABOrder(Order ABOrder, long RoomId, long CompanyId)
        {
            InsertABOrderDTO insertABOrderDTO;

            if (ABOrder != null)
            {
                var orderJson = JsonConvert.SerializeObject(ABOrder);
                var encryptedJson = AESEncryptDecrypt.EncryptAes(orderJson);

                insertABOrderDTO = new InsertABOrderDTO
                {
                    OrderId = ABOrder.orderId,
                    OrderCreated = ABOrder.orderDate,
                    OrderLastUpdated = ABOrder.orderDate,
                    OrderJson = orderJson,
                    OrderJsonEncrypted = encryptedJson,
                    RoomId = RoomId,
                    CompanyId = CompanyId
                };
            }
            else
            {
                insertABOrderDTO = new InsertABOrderDTO();
            }

            return insertABOrderDTO;
        }

        #endregion
    }
}
