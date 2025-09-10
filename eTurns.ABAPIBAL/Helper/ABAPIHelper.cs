using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using Newtonsoft.Json;
using eTurnsMaster.DAL;

namespace eTurns.ABAPIBAL.Helper
{
    public static class ABAPIHelper
    {
        #region [Public Method]

        public static string SaveItemToRoom(ProductToItemQtyDTO SaveItemRequest, long RoomID, long CompanyID, long UserID, string UserName, string EnterPriseDBName,long EnterpriseId, out string Message)
        {
            Message = string.Empty;
            ABProductDetails objSearchResult = ProductsRequest(SaveItemRequest.ASIN, CompanyID, RoomID, UserID, EnterPriseDBName);

            if (objSearchResult != null && !string.IsNullOrWhiteSpace(objSearchResult.asin))
            {
                ProductDetailsDAL objProductDetailsDAL = new ProductDetailsDAL(EnterPriseDBName);
                long ABItemMappingID = objProductDetailsDAL.CheckABItemExistInRoomByAsin(objSearchResult.asin, CompanyID, RoomID);

                if (ABItemMappingID <= 0)
                {
                    ABRoomStoreSettingDAL objABRoomStoreSettingDAL = new ABRoomStoreSettingDAL(EnterPriseDBName);
                    ABRoomStoreSettingDTO objABRoomStoreSettingDTO = objABRoomStoreSettingDAL.GetABRoomStoreSetting(CompanyID, RoomID);

                    string ManufacturerName = "";
                    if (objSearchResult != null
                        && objSearchResult.productOverview != null
                        && objSearchResult.productOverview.Count > 0
                        && objSearchResult.productOverview.Where(x => x.Key.ToLower() == "manufacturer").Count() > 0)
                    {
                        KeyValuePair<string, string> productManufacturerName = new KeyValuePair<string, string>();
                        productManufacturerName = objSearchResult.productOverview.Where(x => x.Key.ToLower() == "manufacturer").FirstOrDefault();
                        ManufacturerName = productManufacturerName.Value;
                    }
                    string SupplierName = "AMAZON";

                    if (objABRoomStoreSettingDTO != null && !string.IsNullOrWhiteSpace(objABRoomStoreSettingDTO.AbSupplierName))
                    {
                        SupplierName = objABRoomStoreSettingDTO.AbSupplierName;
                    }

                    string UPCValue = "";

                    if (objSearchResult != null && objSearchResult.productOverview != null && objSearchResult.productOverview.Count > 0
                            && objSearchResult.productOverview.Where(x => x.Key.ToLower() == "upc").Count() > 0)
                    {
                        KeyValuePair<string, string> productUPCValue = new KeyValuePair<string, string>();
                        productUPCValue = objSearchResult.productOverview.Where(x => x.Key.ToLower() == "upc").FirstOrDefault();
                        UPCValue = productUPCValue.Value;
                    }

                    string UNSPSCValue = "";

                    if (objSearchResult != null && objSearchResult.taxonomies != null && objSearchResult.taxonomies.Count() > 0
                            && !string.IsNullOrWhiteSpace(objSearchResult.taxonomies[0].type) && objSearchResult.taxonomies[0].type == "UNSPSC"
                            && !string.IsNullOrWhiteSpace(objSearchResult.taxonomies[0].taxonomyCode))
                    {
                        UNSPSCValue = objSearchResult.taxonomies[0].taxonomyCode;
                    }

                    string features = "";

                    if (objSearchResult != null && objSearchResult.features != null && objSearchResult.features.Count() > 0)
                    {
                        for (var i = 0; i < objSearchResult.features.Count(); i++)
                        {
                            features = features + objSearchResult.features[i];
                        }
                    }

                    double Cost = 0;

                    if (objSearchResult != null && objSearchResult.includedDataTypes.OFFERS.Count > 0)
                    {
                        if (objSearchResult.includedDataTypes.OFFERS[0].price != null && objSearchResult.includedDataTypes.OFFERS[0].price.value != null
                        && objSearchResult.includedDataTypes.OFFERS[0].price.value.amount > 0)
                        {
                            Cost = objSearchResult.includedDataTypes.OFFERS[0].price.value.amount;
                        }
                    }

                    string ItemImageExternalURL = "";
                    string ItemLink2ExternalURL = "";

                    if (objSearchResult != null && objSearchResult.includedDataTypes.IMAGES != null && objSearchResult.includedDataTypes.IMAGES.Count() > 0
                        && objSearchResult.includedDataTypes.IMAGES[0].large != null && !string.IsNullOrWhiteSpace(objSearchResult.includedDataTypes.IMAGES[0].large.url))
                    {
                        ItemImageExternalURL = objSearchResult.includedDataTypes.IMAGES[0].large.url;
                    }

                    if (objSearchResult != null && objSearchResult.includedDataTypes.IMAGES != null && objSearchResult.includedDataTypes.IMAGES.Count() > 1
                        && objSearchResult.includedDataTypes.IMAGES[1].large != null && !string.IsNullOrWhiteSpace(objSearchResult.includedDataTypes.IMAGES[1].large.url))
                    {
                        ItemLink2ExternalURL = objSearchResult.includedDataTypes.IMAGES[1].large.url;
                    }

                    string ItemNumber = "";
                    if (objSearchResult != null && objSearchResult.productOverview != null && objSearchResult.productOverview.Count > 0
                            && objSearchResult.productOverview.Where(x => x.Key.ToLower() == "item model number").Count() > 0)
                    {
                        KeyValuePair<string, string> productItemmodelNumber = new KeyValuePair<string, string>();
                        productItemmodelNumber = objSearchResult.productOverview.Where(x => x.Key.ToLower() == "item model number").FirstOrDefault();
                        ItemNumber = productItemmodelNumber.Value;
                    }
                    else
                    {
                        ItemNumber = objSearchResult.asin;
                    }

                    string SupplierNameForUDF = "";
                    if (objSearchResult != null
                        && objSearchResult.includedDataTypes.OFFERS.Count > 0
                        && objSearchResult.includedDataTypes.OFFERS[0].merchant != null
                        && !string.IsNullOrWhiteSpace(objSearchResult.includedDataTypes.OFFERS[0].merchant.name))
                    {
                        SupplierNameForUDF = objSearchResult.includedDataTypes.OFFERS[0].merchant.name;
                    }

                    string ItemUniqueNumber = "";
                    CommonDAL objCommonDAL = new CommonDAL(EnterPriseDBName);
                    ItemUniqueNumber = objCommonDAL.GetItemUniqueIdByRoom(RoomID, CompanyID);
                    var objItemMasterDTO = new
                    {
                        ItemNumber = ItemNumber,
                        ManufacturerName = ManufacturerName,
                        SupplierName = SupplierName,
                        SupplierPartNo = objSearchResult.asin,
                        UPC = UPCValue,
                        UNSPSC = UNSPSCValue,
                        Description = objSearchResult.title,
                        LongDescription = features,
                        Cost = Cost,
                        CriticalQuantity = SaveItemRequest.CriticalQty.GetValueOrDefault(0),
                        MinimumQuantity = SaveItemRequest.MinimumQty.GetValueOrDefault(0),
                        MaximumQuantity = SaveItemRequest.MaximumQty.GetValueOrDefault(0),
                        ItemUniqueNumber = ItemUniqueNumber,
                        ItemImageExternalURL = ItemImageExternalURL,
                        ItemLink2ExternalURL = ItemLink2ExternalURL,
                        SupplierNameForUDF = SupplierNameForUDF
                    };

                    var insertedItem = objProductDetailsDAL.InsertABItemToRoom(JsonConvert.SerializeObject(objItemMasterDTO), UserID, UserID, CompanyID, RoomID);

                    if (insertedItem != null && insertedItem.ID > 0)
                    {
                        if (SaveItemRequest.CallFor == ProductToItemCallFor.AddToCart && SaveItemRequest.CartQty.GetValueOrDefault(0) > 0)
                        {
                            string tmpMessage = string.Empty;
                            var itemAddToCartStatus = AddABItemToCart(insertedItem.GUID, insertedItem.ItemNumber, SaveItemRequest.CartQty.GetValueOrDefault(0), insertedItem.DefaultLocationName, "Purchase", RoomID, CompanyID, UserID, UserName, EnterPriseDBName,EnterpriseId, out tmpMessage);
                            Message = tmpMessage;
                            return itemAddToCartStatus;
                            //return Json(new { Message = Message, Status = itemAddToCartStatus }, JsonRequestBehavior.AllowGet);
                        }

                        if (SaveItemRequest.CallFor == ProductToItemCallFor.AddToItem)
                        {
                            CartItemDAL objCartItemDAL = new CartItemDAL(EnterPriseDBName);
                            objCartItemDAL.AutoCartUpdateByCode(insertedItem.GUID, UserID, "Web", "AB Product >> Add To Item", UserID);
                            //GlobalHost.ConnectionManager.GetHubContext<eTurnsHub>().Clients.Group(EnterPriceID + "_" + CompanyID + "_" + RoomID).UpdateRedCircleCountInClients();
                        }

                        Message = "ResABProducts.ItemaddtoroomSuccessfully";
                        return "success";
                        //return Json(new { Message = ResABProducts.ItemaddtoroomSuccessfully, Status = "success" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        Message = "ResABProducts.ItemaddtoroomFail";
                        return "fail";
                        //return Json(new { Message = ResABProducts.ItemaddtoroomFail, Status = "fail" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    if (SaveItemRequest.CallFor == ProductToItemCallFor.AddToCart && SaveItemRequest.CartQty.GetValueOrDefault(0) > 0)
                    {
                        var existingItem = objProductDetailsDAL.GetItemByABItemMappingId(ABItemMappingID, CompanyID, RoomID);

                        if (existingItem != null && existingItem.ID > 0)
                        {
                            string tmpMessage = string.Empty;
                            var itemAddToCartStatus = AddABItemToCart(existingItem.GUID, existingItem.ItemNumber, SaveItemRequest.CartQty.GetValueOrDefault(0), existingItem.DefaultLocationName, "Purchase", RoomID, CompanyID, UserID, UserName, EnterPriseDBName,EnterpriseId, out tmpMessage);
                            Message = tmpMessage;
                            return itemAddToCartStatus;
                            //return Json(new { Message = Message, Status = itemAddToCartStatus }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    Message = "ResABProducts.AsinAlreadyExist";
                    return "fail";
                    //return Json(new { Message = ResABProducts.AsinAlreadyExist, Status = "fail" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                Message = "ResABProducts.ASINNotAvailableinResponse";
                return "fail";
                //return Json(new { Message = ResABProducts.ASINNotAvailableinResponse, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        private static string AddABItemToCart(Guid ItemGUID, string ItemNumber, double? CartQty, string BinName, string ReplenishType, long RoomID, long CompanyID, long UserID, string UserName,
            string EnterPriseDBName,long EnterpriseId, out string Message)
        {
            long retid = 0;
            Message = string.Empty;
            string message = ResMessage.SaveMessage;
            string status = "";
            CartItemDAL objCartItemDAL = new CartItemDAL(EnterPriseDBName);
            CartItemDTO objCartItemDTO = new CartItemDTO();
            BinMasterDAL objBinMasterDAL = new BinMasterDAL(EnterPriseDBName);
            objCartItemDTO.Created = DateTimeUtility.DateTimeNow;
            objCartItemDTO.CreatedByName = UserName;
            objCartItemDTO.CreatedBy = UserID;
            objCartItemDTO.Updated = DateTimeUtility.DateTimeNow;
            objCartItemDTO.UpdatedByName = UserName;
            objCartItemDTO.LastUpdatedBy = UserID;
            objCartItemDTO.ID = 0;
            objCartItemDTO.IsOnlyFromItemUI = true;
            objCartItemDTO.IsAutoMatedEntry = false;

            if (!string.IsNullOrWhiteSpace(BinName))
            {
                BinMasterDTO objBinMasterDTO = objBinMasterDAL.GetItemBinPlain(ItemGUID, BinName, RoomID, CompanyID, UserID, false);
                objCartItemDTO.BinId = objBinMasterDTO.ID;
                objCartItemDTO.BinGUID = objBinMasterDTO.GUID;
            }
            else
            {
                objCartItemDTO.BinId = null;
                objCartItemDTO.BinGUID = null;
            }

            objCartItemDTO.ItemNumber = ItemNumber;
            objCartItemDTO.ReplenishType = ReplenishType;

            if (ItemGUID != Guid.Empty)
                objCartItemDTO.ItemGUID = ItemGUID;

            objCartItemDTO.CompanyID = CompanyID;
            objCartItemDTO.Room = RoomID;
            objCartItemDTO.Quantity = CartQty;
            objCartItemDTO.IsDeleted = false;
            objCartItemDTO.IsArchived = false;
            long SessionUserId = UserID;

            try
            {
                ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                objItemMasterDTO = new ItemMasterDAL(EnterPriseDBName).GetItemWithoutJoins(null, objCartItemDTO.ItemGUID);

                if (objItemMasterDTO.IsEnforceDefaultReorderQuantity ?? false)
                {
                    double newOrderQty = 0;
                    int devideval = 0;
                    double tempQty = objCartItemDTO.Quantity ?? 0;
                    double drq = objItemMasterDTO.DefaultReorderQuantity ?? 0;

                    if (tempQty > 0 && drq > 0)
                    {
                        if ((tempQty % drq) != 0)
                        {
                            devideval = Convert.ToInt32((Convert.ToInt32(tempQty) / Convert.ToInt32(drq))) + 1;
                            newOrderQty = drq * devideval;
                            objCartItemDTO.EnforsedCartQuanity = true;
                        }
                        else
                        {
                            newOrderQty = tempQty;
                        }
                    }

                    objCartItemDTO.Quantity = newOrderQty;
                }

                objCartItemDTO.WhatWhereAction = "Replenish";
                objCartItemDTO.AddedFrom = "Web";
                objCartItemDTO.EditedFrom = "Web";
                objCartItemDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                objCartItemDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                if (string.IsNullOrEmpty(objCartItemDTO.ReplenishType))
                {
                    if (objItemMasterDTO.IsPurchase && objItemMasterDTO.IsTransfer)
                    {
                        objCartItemDTO.ReplenishType = "Purchase";
                    }
                    else if (objItemMasterDTO.IsPurchase)
                    {
                        objCartItemDTO.ReplenishType = "Purchase";
                    }
                    else if (objItemMasterDTO.IsTransfer)
                    {
                        objCartItemDTO.ReplenishType = "Transfer";
                    }
                    else
                    {
                        objCartItemDTO.ReplenishType = "Purchase";
                    }
                }

                objCartItemDTO = objCartItemDAL.SaveCartItem(objCartItemDTO, SessionUserId, EnterpriseId);
                objCartItemDAL.AutoCartUpdateByCode(ItemGUID, UserID, "Web", "AB Product >> Add To Cart", SessionUserId);

                message = "ResCartItem.QuantityAdjustmentMessage";
                status = "success";

                if (objCartItemDTO != null && objCartItemDTO.EnforsedCartQuanity == true)
                {
                    message = "ResCartItem.QuantityAdjustmentMessage";
                }
                else
                {
                    message = "ResABProducts.ItemAddedToCartSuccessfully";
                }
            }
            catch (Exception)
            {
                message = "ResABProducts.FailToAddItemToCart";
                status = "fail";
            }

            Message = message;

            return status;
            //return Json(new { Message = message, Status = status, TotalQty = SumperItem, CartObj = objCartItemDTO }, JsonRequestBehavior.AllowGet);
        }

        public static ABTokenDetailDTO GenerateTokensbyAuthCode(long RoomID, long CompanyID, long UserID, string EnterPriseDBName, string CurrentDomainURL)
        {
            ABTokenDetailDTO objABTokenDetailDTO = new ABTokenDetailDTO();
            try
            {
                ABRoomStoreSettingDAL objABRoomStoreSettingDAL = new ABRoomStoreSettingDAL(EnterPriseDBName);
                ABSetUpDAL objABSetUpDAL = new ABSetUpDAL(DbConnectionHelper.GeteTurnsQuickBookDBName());
                ABDevAppSettingDTO objABDevAppSettingDTO = objABSetUpDAL.GetABDevAppSetting();
                ABRoomStoreSettingDTO objABRoomStoreSettingDTO = objABRoomStoreSettingDAL.GetABRoomStoreSetting(CompanyID, RoomID);
                if (objABRoomStoreSettingDTO != null && objABRoomStoreSettingDTO.ID > 0)
                {
                    ABMarketPlaceDTO objABMarketPlaceDTO = objABSetUpDAL.GetABMarketPlaceByMPID(objABRoomStoreSettingDTO.MarketplaceID);
                    //string endPoint = "https://api.amazon.com/auth/o2/token";
                    string endPoint = objABMarketPlaceDTO.OAuthAuthAPIURI + objABDevAppSettingDTO.TokenRequestURL;
                    string RetURL = CurrentDomainURL + objABDevAppSettingDTO.RedirectURL;
                    //RetURL = "https://rfid.eturns.com/api/Master/CallbackPostResp";
                    var client = new HttpClient();
                    var data = new[] {
                        new KeyValuePair<string, string>("client_secret", objABDevAppSettingDTO.LWAClientSecret),
                        new KeyValuePair<string, string>("client_id", objABDevAppSettingDTO.LWAClientID),
                        new KeyValuePair<string, string>("grant_type", "authorization_code"),
                        new KeyValuePair<string, string>("code", objABRoomStoreSettingDTO.AuthCode),
                        new KeyValuePair<string, string>("redirect_uri", RetURL)
                    };
                    //CommonFunctions.SaveLogInTextFile(endPoint + RetURL);
                    HttpResponseMessage objres = client.PostAsync(endPoint, new FormUrlEncodedContent(data)).GetAwaiter().GetResult();
                    if (objres.StatusCode == HttpStatusCode.OK)
                    {
                        string rejresp = objres.Content.ReadAsStringAsync().Result;
                        ABTokenInfo objABTokenInfo = JsonConvert.DeserializeObject<ABTokenInfo>(rejresp);
                        if (objABTokenInfo != null)
                        {
                            objABTokenDetailDTO = new ABTokenDetailDTO();
                            objABTokenDetailDTO.ABAppAuthCode = objABRoomStoreSettingDTO.AuthCode;
                            objABTokenDetailDTO.AccessToken = objABTokenInfo.access_token;
                            objABTokenDetailDTO.AccessTokenExpireDate = DateTime.UtcNow.AddSeconds(objABTokenInfo.expires_in);
                            objABTokenDetailDTO.RefreshToken = objABTokenInfo.refresh_token;
                            objABTokenDetailDTO.RefreshTokenExpireDate = DateTime.UtcNow.AddDays(365);
                            objABTokenDetailDTO.Created = DateTime.UtcNow;
                            objABTokenDetailDTO.LastUpdated = DateTime.UtcNow;
                            objABTokenDetailDTO.CreatedBy = UserID;
                            objABTokenDetailDTO.LastUpdatedBy = UserID;
                            objABTokenDetailDTO.AddedFrom = "web";
                            objABTokenDetailDTO.EditedFrom = "web";
                            objABTokenDetailDTO = objABSetUpDAL.InsertABTokenDetail(objABTokenDetailDTO);
                        }
                    }
                    else
                    {
                        string rejresp = objres.Content.ReadAsStringAsync().Result;
                        //CommonFunctions.SaveLogInTextFile(rejresp);
                    }
                }
                return objABTokenDetailDTO;
            }
            catch (Exception ex)
            {
                CommonFunctions.SaveExceptionInTextFile("ABAPIHelper.GenerateTokensbyAuthCode", ex);
                return objABTokenDetailDTO;
            }
        }
        public static ProductSearchResultDTO SearchProductsRequest(string k, string page, string PrimeEligible, string EligibleForFreeShipping, string DeliveryDay, string Category, string Availability, string SubCategory, long CompanyID, long RoomID, long UserID, string pageSize, string EnterPriseDBName)
        {
            int PgSize = 0;
            string RegionCurrencySymbol = "$";
            ProductSearchQueryReqInfo objProductSearchQueryReqInfo = ValidateProductSearchQueryParams(k, page, PrimeEligible, EligibleForFreeShipping, DeliveryDay, Category, Availability, SubCategory, CompanyID, RoomID, UserID, pageSize, EnterPriseDBName);
            SearchABProductsRequestInfo objSearchABProductsRequestInfo = PrepareABProductSearchRequest(objProductSearchQueryReqInfo, out PgSize, out RegionCurrencySymbol);
            string result = SearchABProducts(objSearchABProductsRequestInfo);
            ProductSearchResultDTO objSearchResult = JsonConvert.DeserializeObject<ProductSearchResultDTO>(result);

            if (objSearchResult != null)
            {
                objSearchResult.pageSize = PgSize;
                objSearchResult.RegionCurrencySymbol = RegionCurrencySymbol;
            }

            return objSearchResult;
        }
        public static ABProductDetails ProductsRequest(string asin, long CompanyID, long RoomID, long UserID, string EnterPriseDBName)
        {
            if (!string.IsNullOrWhiteSpace(asin))
            {
                SearchABProductsRequestInfo objDetailABProductsRequestInfo = PrepareABProductDetailSearchRequest(asin, CompanyID, RoomID, UserID, EnterPriseDBName);
                string result = GetABProductsDetails(objDetailABProductsRequestInfo);
                ABProductDetails objSearchResult = JsonConvert.DeserializeObject<ABProductDetails>(result);
                return objSearchResult;
            }
            else
            {
                return null;
            }
        }
        public static ABProductOffers SearchOffersRequest(string asin, long CompanyID, long RoomID, long UserID, string EnterPriseDBName, Int64 PageNumber)
        {
            if (!string.IsNullOrWhiteSpace(asin))
            {
                SearchABProductsRequestInfo objDetailABProductsRequestInfo = PrepareABProductOffersRequest(asin, CompanyID, RoomID, UserID, EnterPriseDBName, PageNumber);
                string result = GetABProductOffers(objDetailABProductsRequestInfo);
                ABProductOffers objABProductOffers = JsonConvert.DeserializeObject<ABProductOffers>(result);
                return objABProductOffers;
            }
            else
            {
                return null;
            }
        }
        public static ABProductOffers SearchAllOffersRequest(string asin, long CompanyID, long RoomID, long UserID, string EnterPriseDBName)
        {
            if (!string.IsNullOrWhiteSpace(asin))
            {
                ABProductOffers objABProductOffers = SearchOffersRequest(asin, CompanyID, RoomID, UserID, EnterPriseDBName, 0);
                if (objABProductOffers.numberOfPages <= 1)
                {
                    return objABProductOffers;
                }
                else
                {
                    for (Int64 iPagenumber = 1; iPagenumber < objABProductOffers.numberOfPages; iPagenumber++)
                    {
                        ABProductOffers objABProductOffersAll = SearchOffersRequest(asin, CompanyID, RoomID, UserID, EnterPriseDBName, iPagenumber);
                        if (objABProductOffersAll != null && objABProductOffersAll.offerCount > 0)
                        {
                            objABProductOffers.offers.AddRange(objABProductOffersAll.offers);
                        }
                    }
                    return objABProductOffers;
                }
            }
            else
            {
                return null;
            }
        }
        public static ABProductByASINDTO GetProductsByAsins(List<string> ASINs, long CompanyID, long RoomID, long UserID, string EnterPriseDBName)
        {
            if (ASINs != null && ASINs.Count > 0)
            {
                int PgSize = 0;
                string RegionCurrencySymbol = "$";
                SearchABProductsRequestInfo objDetailABProductsRequestInfo = PrepareABProductsSearchByASINsRequest(ASINs, CompanyID, RoomID, UserID, EnterPriseDBName, out PgSize, out RegionCurrencySymbol);
                string result = GetABProductsByASINs(objDetailABProductsRequestInfo);
                ABProductByASINDTO objABProductByASINDTO = JsonConvert.DeserializeObject<ABProductByASINDTO>(result);

                if (objABProductByASINDTO != null)
                {
                    objABProductByASINDTO.pageSize = PgSize;
                    objABProductByASINDTO.RegionCurrencySymbol = RegionCurrencySymbol;
                }

                return objABProductByASINDTO;
            }
            else
            {
                return null;
            }

        }

        public static ABOrderDTO GetOrdersByOrderDate(DateTime StartDate, DateTime EndDate, bool IncludeLineItems, bool IncludeShipments, bool IncludeCharges, string NextPageToken, string PurchaseOrderNumber, long CompanyID, long RoomID, long UserID, string EnterPriseDBName)
        {
            OrderSearchQueryReqInfo orderByOrderDateQueryReqInfo = ValidateGetOrderByOrderDateQueryParams(StartDate, EndDate, IncludeLineItems, IncludeShipments, IncludeCharges, NextPageToken, PurchaseOrderNumber, CompanyID, RoomID, UserID, EnterPriseDBName);
            SearchABProductsRequestInfo orderByOrderDateRequestInfo = PrepareABOrderSearchRequest(orderByOrderDateQueryReqInfo);
            string result = GetABOrders(orderByOrderDateRequestInfo);
            var orders = JsonConvert.DeserializeObject<ABOrderDTO>(result);
            return orders;
        }

        public static ABOrderDTO GetOrdersByOrderId(string OrderId, bool IncludeLineItems, bool IncludeShipments, bool IncludeCharges, long CompanyID, long RoomID, long UserID, string EnterPriseDBName)
        {
            var ordersByOrderIdQueryReqInfo = ValidateGetOrderByIdQueryParams(OrderId, IncludeLineItems, IncludeShipments, IncludeCharges, CompanyID, RoomID, UserID, EnterPriseDBName);
            SearchABProductsRequestInfo objSearchABProductsRequestInfo = PrepareGetOrderByOrderIdSearchRequest(ordersByOrderIdQueryReqInfo);
            string result = GetABOrders(objSearchABProductsRequestInfo);
            var orders = JsonConvert.DeserializeObject<ABOrderDTO>(result);
            return orders;
        }


        public static ABProductByASINDTO GetProductsByAsinsAll(List<string> productASINs, long CompanyID, long RoomID, long UserID, string EnterPriseDBName)
        {
            if (productASINs != null && productASINs.Count > 0)
            {
                ABProductByASINDTO result;
                if (productASINs.Count <= 30)
                {
                    result = ABAPIHelper.GetProductsByAsins(productASINs, CompanyID, RoomID, UserID, EnterPriseDBName);
                    return result;
                }
                else
                {
                    result = new ABProductByASINDTO();
                    double looCountapprox = (float)productASINs.Count / 30;
                    int loopCount = Convert.ToInt32(Math.Ceiling(looCountapprox));

                    for (int counter = 1; counter <= loopCount; counter++)
                    {
                        int startIndex = (counter - 1) * 30;
                        var currentRequestASINs = productASINs.Skip(startIndex).Take(30).ToList();

                        if (currentRequestASINs != null && currentRequestASINs.Any() && currentRequestASINs.Count > 0)
                        {
                            var currentASINsResult = ABAPIHelper.GetProductsByAsins(currentRequestASINs, CompanyID, RoomID, UserID, EnterPriseDBName);

                            if (currentASINsResult != null)
                            {
                                if (counter == 1)
                                {
                                    result = currentASINsResult;
                                }
                                else
                                {
                                    if (currentASINsResult.products != null && currentASINsResult.products.Any() && currentASINsResult.products.Count > 0)
                                    {
                                        if (result != null && result.products != null)
                                        {
                                            result.products.AddRange(currentASINsResult.products);
                                            result.matchingProductCount = result.matchingProductCount + currentASINsResult.matchingProductCount;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return result;
                }
            }
            else
            {
                return null;
            }

        }

        public static ABOrderDTO GetOrderByNextPageToken(string NextPageToken, long CompanyID, long RoomID, long UserID, string EnterPriseDBName)
        {
            OrderSearchQueryReqInfo orderByOrderByNextPagetokenQueryReqInfo = ValidateGetOrderByNextPageTokenQueryParams(NextPageToken, CompanyID, RoomID, UserID, EnterPriseDBName);
            SearchABProductsRequestInfo orderByOrderDateRequestInfo = PrepareABOrderByPageTokenSearchRequest(orderByOrderByNextPagetokenQueryReqInfo);
            string result = GetABOrders(orderByOrderDateRequestInfo);
            var orders = JsonConvert.DeserializeObject<ABOrderDTO>(result);
            return orders;
        }

        public static void ImportABOrderToLocalDB(DateTime StartDate, DateTime EndDate, long CompanyId, long RoomId, long UserId, string EnterpriseDBName)
        {
            var roomMasterDAL = new RoomDAL(EnterpriseDBName);
            var room = roomMasterDAL.GetRoomByIDPlain(RoomId);
            var abOrderDAL = new ABOrderDAL(EnterpriseDBName);

            if (room != null)
            {
                for (var day = StartDate.Date; day.Date <= EndDate.Date; day = day.AddDays(1))
                {
                    try
                    {
                        var startDate = day;
                        var endDate = startDate.AddHours(23);
                        endDate = endDate.AddMinutes(59);
                        endDate = endDate.AddSeconds(59);

                        var order = GetOrdersByOrderDate(startDate, endDate, true, true, true, null, null, CompanyId, RoomId, UserId, EnterpriseDBName);

                        if (order != null && order.orders != null && order.orders.Any() && order.orders.Count > 0)
                        {
                            foreach (var currentOrder in order.orders)
                            {
                                var abOrder = GetInsertABOrderDTOFromABOrder(currentOrder, RoomId, CompanyId);

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
                                var subsequentOrder = GetOrderByNextPageToken(HttpUtility.UrlEncode(nextPageToken), CompanyId, RoomId, UserId, EnterpriseDBName);

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
                                            var ssABOrder = GetInsertABOrderDTOFromABOrder(currentSSOrder, RoomId, CompanyId);

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
                    catch (Exception ex)
                    {

                    }

                }
            }

        }

        public static void SyncABOrderedItemsToRoomItems(long CompanyId, long RoomId, long UserId, string UserName, string EnterpriseDBName,long EnterpriseId)
        {
            var roomMasterDAL = new eTurns.DAL.RoomDAL(EnterpriseDBName);
            var room = roomMasterDAL.GetRoomByIDPlain(RoomId);
            var abOrderDAL = new ABOrderDAL(EnterpriseDBName);

            if (room != null)
            {
                var roomOrders = abOrderDAL.GetABOrdersByRoomId(RoomId, CompanyId);

                if (roomOrders != null && roomOrders.Any() && roomOrders.Count > 0)
                {
                    var productDetailDAL = new ProductDetailsDAL(EnterpriseDBName);
                    var roomItemsASIN = productDetailDAL.GetABItemsAsinByRoomId(room.ID);

                    foreach (var order in roomOrders)
                    {
                        try
                        {
                            var encryptedOrderJson = order.OrderJsonEncrypted;
                            var decryptedOrderJson = AESEncryptDecrypt.DecryptAes(encryptedOrderJson);

                            if (!string.IsNullOrEmpty(decryptedOrderJson) && !string.IsNullOrWhiteSpace(decryptedOrderJson))
                            {
                                var orderObject = JsonConvert.DeserializeObject<Order>(decryptedOrderJson);

                                if (orderObject != null && orderObject.lineItems != null && orderObject.lineItems.Any() && orderObject.lineItems.Count > 0)
                                {
                                    var asins = orderObject.lineItems.Where(e => !string.IsNullOrEmpty(e.asin) && !string.IsNullOrWhiteSpace(e.asin)).Select(e => e.asin).Distinct().ToList();

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
                                            var itemSaveStatus = SaveItemToRoom(saveItemRequest, RoomId, CompanyId, UserId, UserName, EnterpriseDBName, EnterpriseId, out message);

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
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }

        }

        public static Dictionary<List<ItemMasterDTO>, string> ItemSyncToRoom(List<string> ASINs, long CompanyID, long RoomID, long UserID, string EnterPriseDBName)
        {
            Dictionary<List<ItemMasterDTO>, string> lstNonOrderableItems = new Dictionary<List<ItemMasterDTO>, string>();
            try
            {
                if (ASINs.Count > 0)
                {
                    ABProductByASINDTO objResult;
                    objResult = GetProductsByAsinsAll(ASINs, CompanyID, RoomID, UserID, EnterPriseDBName);
                    var lstItemMasterDTO = new List<dynamic>();
                    if (objResult != null
                        && objResult.products != null
                        && objResult.products.Count > 0)
                    {
                        ABRoomStoreSettingDAL objABRoomStoreSettingDAL = new ABRoomStoreSettingDAL(EnterPriseDBName);
                        ABRoomStoreSettingDTO objABRoomStoreSettingDTO = objABRoomStoreSettingDAL.GetABRoomStoreSetting(CompanyID, RoomID);

                        foreach (Product objSearchResult in objResult.products)
                        {
                            string ManufacturerName = "";
                            if (objSearchResult != null
                                && objSearchResult.productOverview != null
                                && objSearchResult.productOverview.Count > 0
                                && objSearchResult.productOverview.Where(x => x.Key.ToLower() == "manufacturer").Count() > 0)
                            {
                                KeyValuePair<string, string> productManufacturerName = new KeyValuePair<string, string>();
                                productManufacturerName = objSearchResult.productOverview.Where(x => x.Key.ToLower() == "manufacturer").FirstOrDefault();
                                ManufacturerName = productManufacturerName.Value;
                            }
                            string SupplierName = "AMAZON";
                            if (objABRoomStoreSettingDTO != null
                                && !string.IsNullOrWhiteSpace(objABRoomStoreSettingDTO.AbSupplierName))
                            {
                                SupplierName = objABRoomStoreSettingDTO.AbSupplierName;
                            }
                            string UPCValue = "";
                            if (objSearchResult != null && objSearchResult.productOverview != null && objSearchResult.productOverview.Count > 0
                                    && objSearchResult.productOverview.Where(x => x.Key.ToLower() == "upc").Count() > 0)
                            {
                                KeyValuePair<string, string> productUPCValue = new KeyValuePair<string, string>();
                                productUPCValue = objSearchResult.productOverview.Where(x => x.Key.ToLower() == "upc").FirstOrDefault();
                                UPCValue = productUPCValue.Value;
                            }
                            string UNSPSCValue = "";
                            if (objSearchResult != null && objSearchResult.taxonomies != null && objSearchResult.taxonomies.Count() > 0
                                    && !string.IsNullOrWhiteSpace(objSearchResult.taxonomies[0].type) && objSearchResult.taxonomies[0].type == "UNSPSC"
                                    && !string.IsNullOrWhiteSpace(objSearchResult.taxonomies[0].taxonomyCode))
                            {
                                UNSPSCValue = objSearchResult.taxonomies[0].taxonomyCode;
                            }

                            string features = "";
                            if (objSearchResult != null && objSearchResult.features != null && objSearchResult.features.Count() > 0)
                            {
                                for (var i = 0; i < objSearchResult.features.Count(); i++)
                                {
                                    features = features + objSearchResult.features[i];
                                }
                            }
                            double Cost = 0;
                            if (objSearchResult != null
                                && objSearchResult.includedDataTypes.OFFERS.Count > 0)
                            {
                                if (objSearchResult.includedDataTypes.OFFERS[0].price != null
                                && objSearchResult.includedDataTypes.OFFERS[0].price.value != null
                                && objSearchResult.includedDataTypes.OFFERS[0].price.value.amount > 0)
                                {
                                    Cost = objSearchResult.includedDataTypes.OFFERS[0].price.value.amount;
                                }
                            }
                            string ItemImageExternalURL = "";
                            string ItemLink2ExternalURL = "";

                            if (objSearchResult != null
                                && objSearchResult.includedDataTypes.IMAGES != null
                                && objSearchResult.includedDataTypes.IMAGES.Count() > 0
                                && objSearchResult.includedDataTypes.IMAGES[0].large != null
                                && !string.IsNullOrWhiteSpace(objSearchResult.includedDataTypes.IMAGES[0].large.url))
                            {
                                ItemImageExternalURL = objSearchResult.includedDataTypes.IMAGES[0].large.url;
                            }
                            if (objSearchResult != null
                                && objSearchResult.includedDataTypes.IMAGES != null
                                && objSearchResult.includedDataTypes.IMAGES.Count() > 1
                                && objSearchResult.includedDataTypes.IMAGES[1].large != null
                                && !string.IsNullOrWhiteSpace(objSearchResult.includedDataTypes.IMAGES[1].large.url))
                            {
                                ItemLink2ExternalURL = objSearchResult.includedDataTypes.IMAGES[1].large.url;
                            }
                            int OfferCount = 1;
                            if (objSearchResult != null
                                && objSearchResult.includedDataTypes.OFFERS.Count <= 0)
                            {
                                OfferCount = 0;
                            }
                            string ItemNumber = "";
                            if (objSearchResult != null && objSearchResult.productOverview != null && objSearchResult.productOverview.Count > 0
                                    && objSearchResult.productOverview.Where(x => x.Key.ToLower() == "item model number").Count() > 0)
                            {
                                KeyValuePair<string, string> productItemmodelNumber = new KeyValuePair<string, string>();
                                productItemmodelNumber = objSearchResult.productOverview.Where(x => x.Key.ToLower() == "item model number").FirstOrDefault();
                                ItemNumber = productItemmodelNumber.Value;
                            }
                            else
                            {
                                ItemNumber = objSearchResult.asin;
                            }

                            var objItemMasterDTO = new
                            {
                                ItemNumber = ItemNumber,
                                ManufacturerName = ManufacturerName,
                                SupplierName = SupplierName,
                                SupplierPartNo = objSearchResult.asin,
                                UPC = UPCValue,
                                UNSPSC = UNSPSCValue,
                                Description = objSearchResult.title,
                                LongDescription = features,
                                Cost = Cost,
                                ItemUniqueNumber = objSearchResult.asin,
                                ItemImageExternalURL = ItemImageExternalURL,
                                ItemLink2ExternalURL = ItemLink2ExternalURL,
                                OfferCount = OfferCount
                            };
                            lstItemMasterDTO.Add(objItemMasterDTO);
                        }
                        if (lstItemMasterDTO.Count > 0)
                        {
                            ProductDetailsDAL productDetailsDAL = new ProductDetailsDAL(EnterPriseDBName);
                            List<ItemMasterDTO> lstItems = new List<ItemMasterDTO>();
                            lstItems = productDetailsDAL.SyncABItemToRoom(JsonConvert.SerializeObject(lstItemMasterDTO), UserID, CompanyID, RoomID);
                            lstNonOrderableItems.Add(lstItems, "success");
                            return lstNonOrderableItems;
                        }
                    }
                }
                lstNonOrderableItems.Add(new List<ItemMasterDTO>(), "fail");
                return lstNonOrderableItems;
            }
            catch (Exception)
            {
                lstNonOrderableItems.Add(new List<ItemMasterDTO>(), "fail");
                return lstNonOrderableItems;
            }
        }

        public static bool SendAbIntimationOfAuthCode(string callbackURL, string amazon_state, string status)
        {
            ABTokenDetailDTO objABTokenDetailDTO = new ABTokenDetailDTO();
            try
            {
                string endPoint = callbackURL + "?amazon_state=" + amazon_state + "&status=" + status;
                var client = new HttpClient();                
                HttpResponseMessage objres = client.GetAsync(endPoint).GetAwaiter().GetResult();
                if (objres.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                CommonFunctions.SaveExceptionInTextFile("ABAPIHelper.SendAbIntimationOfAuthCode", ex);
                return false;
            }
        }

        #endregion

        #region [Private methods]
        private static ABTokenDetailDTO GenerateTokensbyRefreshToken(long RoomID, long CompanyID, long UserID, string EnterPriseDBName)
        {
            ABTokenDetailDTO objABTokenDetailDTO = new ABTokenDetailDTO();
            try
            {
                ABRoomStoreSettingDAL objABRoomStoreSettingDAL = new ABRoomStoreSettingDAL(EnterPriseDBName);
                ABSetUpDAL objABSetUpDAL = new ABSetUpDAL(DbConnectionHelper.GeteTurnsQuickBookDBName());
                ABDevAppSettingDTO objABDevAppSettingDTO = objABSetUpDAL.GetABDevAppSetting();
                ABRoomStoreSettingDTO objABRoomStoreSettingDTO = objABRoomStoreSettingDAL.GetABRoomStoreSetting(CompanyID, RoomID);
                string refresh_token = string.Empty;
                ABTokenDetailDTO objABTokenDetailDTODB = new ABTokenDetailDTO();
                if (objABRoomStoreSettingDTO != null && objABRoomStoreSettingDTO.ID > 0 && !string.IsNullOrWhiteSpace(objABRoomStoreSettingDTO.AuthCode))
                {
                    objABTokenDetailDTODB = objABSetUpDAL.GetABTokenDetailByAuthCode(objABRoomStoreSettingDTO.AuthCode);
                    if (objABTokenDetailDTODB != null && objABTokenDetailDTODB.ID > 0)
                    {
                        refresh_token = objABTokenDetailDTODB.RefreshToken;
                    }
                }

                if (objABRoomStoreSettingDTO != null && objABRoomStoreSettingDTO.ID > 0 && !string.IsNullOrWhiteSpace(refresh_token))
                {
                    ABMarketPlaceDTO objABMarketPlaceDTO = objABSetUpDAL.GetABMarketPlaceByMPID(objABRoomStoreSettingDTO.MarketplaceID);
                    string endPoint = objABMarketPlaceDTO.OAuthAuthAPIURI + objABDevAppSettingDTO.TokenRequestURL;
                    //string RetURL = CurrentDomainURL + objABDevAppSettingDTO.RedirectURL;
                    var client = new HttpClient();
                    var data = new[] {
                        new KeyValuePair<string, string>("client_secret", objABDevAppSettingDTO.LWAClientSecret),
                        new KeyValuePair<string, string>("client_id", objABDevAppSettingDTO.LWAClientID),
                        new KeyValuePair<string, string>("grant_type", "refresh_token"),
                        new KeyValuePair<string, string>("refresh_token", refresh_token)
                    };
                    //CommonFunctions.SaveLogInTextFile(endPoint + RetURL);
                    HttpResponseMessage objres = client.PostAsync(endPoint, new FormUrlEncodedContent(data)).GetAwaiter().GetResult();
                    if (objres.StatusCode == HttpStatusCode.OK)
                    {
                        string rejresp = objres.Content.ReadAsStringAsync().Result;
                        ABTokenInfo objABTokenInfo = JsonConvert.DeserializeObject<ABTokenInfo>(rejresp);
                        if (objABTokenInfo != null)
                        {
                            objABTokenDetailDTO = new ABTokenDetailDTO();
                            objABTokenDetailDTO.ID = objABTokenDetailDTODB.ID;
                            objABTokenDetailDTO.AccessToken = objABTokenInfo.access_token;
                            objABTokenDetailDTO.AccessTokenExpireDate = DateTime.UtcNow.AddSeconds(objABTokenInfo.expires_in);
                            objABTokenDetailDTO.RefreshToken = objABTokenInfo.refresh_token;
                            objABTokenDetailDTO.RefreshTokenExpireDate = DateTime.UtcNow.AddDays(365);
                            objABTokenDetailDTO.Created = DateTime.UtcNow;
                            objABTokenDetailDTO.LastUpdated = DateTime.UtcNow;
                            objABTokenDetailDTO.CreatedBy = UserID;
                            objABTokenDetailDTO.LastUpdatedBy = UserID;
                            objABTokenDetailDTO.AddedFrom = "web";
                            objABTokenDetailDTO.EditedFrom = "web";
                            objABTokenDetailDTO = objABSetUpDAL.EditABTokenDetailAccessCodeInfo(objABTokenDetailDTO);
                        }
                    }
                    else if (objres.StatusCode == HttpStatusCode.BadRequest)
                    {
                        try
                        {
                            string rejresp = objres.Content.ReadAsStringAsync().Result;
                            ABTokenInfo objABTokenInfo = JsonConvert.DeserializeObject<ABTokenInfo>(rejresp);
                            if (objABTokenInfo.error == "invalid_grant")
                            {
                                objABRoomStoreSettingDAL.EditABRoomStoreSettingAuthCode(objABRoomStoreSettingDTO.ID, String.Empty);
                            }
                        }
                        catch (Exception ex)
                        {
                            CommonFunctions.SaveExceptionInTextFile("ABAPIHelper.GenerateTokensbyRefreshToken", ex);
                        }
                    }
                    else
                    {
                        string rejresp = objres.Content.ReadAsStringAsync().Result;
                        //CommonFunctions.SaveLogInTextFile(rejresp);
                    }
                }
                return objABTokenDetailDTO;
            }
            catch (Exception ex)
            {
                CommonFunctions.SaveExceptionInTextFile("ABAPIHelper.GenerateTokensbyRefreshToken", ex);
                return objABTokenDetailDTO;
            }
        }
        private static string SearchABProducts(SearchABProductsRequestInfo objSearchABProductsRequestInfo)
        {
            string ReturnJSon = string.Empty;
            try
            {
                // 0. Prepare request message.
                HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Get, objSearchABProductsRequestInfo.ApiQueryURL);
                msg.Headers.Host = msg.RequestUri.Host;

                // Get and save dates ready for further use.
                DateTimeOffset utcNowSaved = DateTimeOffset.UtcNow;
                string amzLongDate = utcNowSaved.ToString("yyyyMMddTHHmmssZ");
                string amzShortDate = utcNowSaved.ToString("yyyyMMdd");

                // Add to headers. 
                msg.Headers.Add("x-amz-date", amzLongDate);
                msg.Headers.Add("x-amz-access-token", objSearchABProductsRequestInfo.AccessToken); // My API call needs an x-api-key passing also for function security.
                msg.Headers.Add("x-amz-user-email", objSearchABProductsRequestInfo.EmailAddress);
                msg.Headers.Add("EmptyHeaderCheck", SiteSettingHelper.EmptyHeaderCheck);
                // **************************************************** SIGNING PORTION ****************************************************
                // 1. Create Canonical Request            
                var canonicalRequest = new StringBuilder();
                canonicalRequest.Append(msg.Method + "\n");
                canonicalRequest.Append(string.Join("/", msg.RequestUri.AbsolutePath.Split('/').Select(Uri.EscapeDataString)) + "\n");

                canonicalRequest.Append(GetCanonicalQueryParams(msg) + "\n"); // Query params to do.

                var headersToBeSigned = new List<string>();
                foreach (var header in msg.Headers.OrderBy(a => a.Key.ToLowerInvariant(), StringComparer.OrdinalIgnoreCase))
                {
                    canonicalRequest.Append(header.Key.ToLowerInvariant());
                    canonicalRequest.Append(":");
                    canonicalRequest.Append(string.Join(",", header.Value.Select(s => s.Trim())));
                    canonicalRequest.Append("\n");
                    headersToBeSigned.Add(header.Key.ToLowerInvariant());
                }
                canonicalRequest.Append("\n");

                var signedHeaders = string.Join(";", headersToBeSigned);
                canonicalRequest.Append(signedHeaders + "\n");
                canonicalRequest.Append(objSearchABProductsRequestInfo.EmptyBodySignature); // Signature for empty body.
                                                                                            // Hash(msg.Content.ReadAsByteArrayAsync().Result);

                // 2. String to sign.            
                string stringToSign = objSearchABProductsRequestInfo.AWSHMACSignPrefix + "\n" + amzLongDate + "\n" + amzShortDate + "/" + objSearchABProductsRequestInfo.AWSRegion + "/" + objSearchABProductsRequestInfo.AWSServiceName + "/" + objSearchABProductsRequestInfo.AWSRequestType + "\n" + Hash(Encoding.UTF8.GetBytes(canonicalRequest.ToString()));

                // 3. Signature with compounded elements.
                var dateKey = HmacSha256(Encoding.UTF8.GetBytes(objSearchABProductsRequestInfo.AWSSigningVersion + objSearchABProductsRequestInfo.IAMSecretKey), amzShortDate);
                var dateRegionKey = HmacSha256(dateKey, objSearchABProductsRequestInfo.AWSRegion);
                var dateRegionServiceKey = HmacSha256(dateRegionKey, objSearchABProductsRequestInfo.AWSServiceName);
                var signingKey = HmacSha256(dateRegionServiceKey, objSearchABProductsRequestInfo.AWSRequestType);

                var signature = ToHexString(HmacSha256(signingKey, stringToSign.ToString()));

                // **************************************************** END SIGNING PORTION ****************************************************

                // Add the Header to the request.
                var credentialScope = amzShortDate + "/" + objSearchABProductsRequestInfo.AWSRegion + "/" + objSearchABProductsRequestInfo.AWSServiceName + "/" + objSearchABProductsRequestInfo.AWSRequestType;
                msg.Headers.TryAddWithoutValidation("Authorization", objSearchABProductsRequestInfo.AWSHMACSignPrefix + " Credential=" + objSearchABProductsRequestInfo.IAMAccessKey + "/" + credentialScope + ", SignedHeaders=" + signedHeaders + ", Signature=" + signature);

                // Invoke the request with HttpClient.
                HttpClient client = new HttpClient();
                HttpResponseMessage result = client.SendAsync(msg).Result;
                if (result.IsSuccessStatusCode)
                {
                    ReturnJSon = result.Content.ReadAsStringAsync().Result;
                    // Inspect the result and payload returned.
                    //Console.WriteLine(result.Headers);
                    //Console.WriteLine(result.Content.ReadAsStringAsync().Result);
                    // Wait on user.
                    //Console.ReadLine();
                }

            }
            catch (Exception ex)
            {
                CommonFunctions.SaveExceptionInTextFile("ABAPIHelper.SearchABProducts >>", ex);
            }
            return ReturnJSon;
        }
        private static SearchABProductsRequestInfo PrepareABProductSearchRequest(ProductSearchQueryReqInfo objProductSearchQueryReqInfo, out int PageSize, out string RegionCurrencySymbol)
        {
            PageSize = 0;
            RegionCurrencySymbol = "$";
            SearchABProductsRequestInfo objSearchABProductsRequestInfo = new SearchABProductsRequestInfo();
            string SearchQuery = string.Empty;
            string FullApipathwithQuery = string.Empty;
            try
            {
                ABSetUpDAL objABSetUpDAL = new ABSetUpDAL(DbConnectionHelper.GeteTurnsQuickBookDBName());
                ABRoomStoreSettingDAL objABRoomStoreSettingDAL = new ABRoomStoreSettingDAL(objProductSearchQueryReqInfo.EnterPriseDBName);
                ABRoomStoreSettingDTO objABRoomStoreSettingDTO = objABRoomStoreSettingDAL.GetABRoomStoreSetting(objProductSearchQueryReqInfo.CompanyID, objProductSearchQueryReqInfo.RoomID);
                ABDevAppSettingDTO objABDevAppSettingDTO = objABSetUpDAL.GetABDevAppSetting();
                if (objABRoomStoreSettingDTO != null && objABRoomStoreSettingDTO.ID > 0)
                {
                    int pageNum = 0;
                    int.TryParse(objProductSearchQueryReqInfo.PageNumber, out pageNum);
                    if (pageNum < 1)
                    {
                        pageNum = 1;
                    }
                    pageNum = pageNum - 1;
                    if (objProductSearchQueryReqInfo.pageSize < 1 || objProductSearchQueryReqInfo.pageSize > 24)
                    {
                        if (objABRoomStoreSettingDTO.ProductSearchPageSize < 1 || objABRoomStoreSettingDTO.ProductSearchPageSize > 24)
                        {
                            PageSize = 24;
                        }
                        else
                        {
                            PageSize = objABRoomStoreSettingDTO.ProductSearchPageSize;
                        }
                    }
                    else
                    {
                        PageSize = objProductSearchQueryReqInfo.pageSize;
                    }

                    if (!string.IsNullOrEmpty(objABRoomStoreSettingDTO.RegionCurrencySymbol) && !string.IsNullOrWhiteSpace(objABRoomStoreSettingDTO.RegionCurrencySymbol))
                    {
                        RegionCurrencySymbol = objABRoomStoreSettingDTO.RegionCurrencySymbol;
                    }

                    SearchQuery = SearchQuery + "keywords=" + (objProductSearchQueryReqInfo.keywords ?? string.Empty);
                    SearchQuery = SearchQuery + "&locale=" + (objABRoomStoreSettingDTO.ABLocale ?? "en_US");
                    SearchQuery = SearchQuery + "&productRegion=" + (objABRoomStoreSettingDTO.ABCountryCode ?? "US");
                    SearchQuery = SearchQuery + "&facets=" + (objABRoomStoreSettingDTO.ProductSearchFacets ?? "OFFERS,IMAGES");
                    SearchQuery = SearchQuery + "&pageSize=" + PageSize;
                    SearchQuery = SearchQuery + "&pageNumber=" + pageNum;

                    if (!string.IsNullOrEmpty(objProductSearchQueryReqInfo.PrimeEligible) && !string.IsNullOrWhiteSpace(objProductSearchQueryReqInfo.PrimeEligible))
                    {
                        SearchQuery = SearchQuery + "&primeEligible=" + objProductSearchQueryReqInfo.PrimeEligible.Trim();
                    }

                    if (!string.IsNullOrEmpty(objProductSearchQueryReqInfo.EligibleForFreeShipping) && !string.IsNullOrWhiteSpace(objProductSearchQueryReqInfo.EligibleForFreeShipping))
                    {
                        SearchQuery = SearchQuery + "&eligibleForFreeShipping=" + objProductSearchQueryReqInfo.EligibleForFreeShipping.Trim();
                    }

                    if (!string.IsNullOrEmpty(objProductSearchQueryReqInfo.DeliveryDay) && !string.IsNullOrWhiteSpace(objProductSearchQueryReqInfo.DeliveryDay))
                    {
                        SearchQuery = SearchQuery + "&deliveryDay=" + objProductSearchQueryReqInfo.DeliveryDay.Trim();
                    }

                    if (!string.IsNullOrEmpty(objProductSearchQueryReqInfo.Category) && !string.IsNullOrWhiteSpace(objProductSearchQueryReqInfo.Category))
                    {
                        SearchQuery = SearchQuery + "&category=" + objProductSearchQueryReqInfo.Category.Trim();
                    }

                    if (!string.IsNullOrEmpty(objProductSearchQueryReqInfo.Availability) && !string.IsNullOrWhiteSpace(objProductSearchQueryReqInfo.Availability))
                    {
                        SearchQuery = SearchQuery + "&availability=" + objProductSearchQueryReqInfo.Availability.Trim();
                    }

                    if (!string.IsNullOrEmpty(objProductSearchQueryReqInfo.SubCategory) && !string.IsNullOrWhiteSpace(objProductSearchQueryReqInfo.SubCategory))
                    {
                        SearchQuery = SearchQuery + "&subCategory=" + objProductSearchQueryReqInfo.SubCategory.Trim();
                    }

                    //SearchQuery = SearchQuery + "&category=ELECTRONICS";

                    ABMarketPlaceDTO objABMarketPlaceDTO = objABSetUpDAL.GetABMarketPlaceByMPID(objABRoomStoreSettingDTO.MarketplaceID);
                    ABAPIPathConfigDTO objABAPIPathConfigDTO = objABSetUpDAL.GetABAPIPathConfig("PRODUCT SEARCH API", "PRODUCT", "searchProductsRequest");
                    FullApipathwithQuery = objABMarketPlaceDTO.ABAPIEndPoint + objABAPIPathConfigDTO.ABAPIPath + "?" + SearchQuery;

                    objSearchABProductsRequestInfo.ApiQueryURL = FullApipathwithQuery;
                    objSearchABProductsRequestInfo.IAMAccessKey = objABDevAppSettingDTO.ABIAMAccessKey;
                    objSearchABProductsRequestInfo.IAMSecretKey = objABDevAppSettingDTO.ABIAMAccessKeySecret;
                    objSearchABProductsRequestInfo.AWSRegion = objABMarketPlaceDTO.AWSRegion;
                    objSearchABProductsRequestInfo.AccessToken = GetValidABAccessToken(objProductSearchQueryReqInfo.RoomID, objProductSearchQueryReqInfo.CompanyID, objProductSearchQueryReqInfo.UserID, objProductSearchQueryReqInfo.EnterPriseDBName, false);
                    objSearchABProductsRequestInfo.EmailAddress = objABRoomStoreSettingDTO.EmailAddress;
                }
            }
            catch (Exception ex)
            {
                CommonFunctions.SaveExceptionInTextFile("ABAPIHelper.PrepareABProductSearchQuery >>" + (objProductSearchQueryReqInfo.keywords ?? String.Empty), ex);
            }
            return objSearchABProductsRequestInfo;
        }
        private static string GetABProductsDetails(SearchABProductsRequestInfo objSearchABProductsRequestInfo)
        {
            string ReturnJSon = string.Empty;
            try
            {
                // 0. Prepare request message.
                HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Get, objSearchABProductsRequestInfo.ApiQueryURL);
                msg.Headers.Host = msg.RequestUri.Host;

                // Get and save dates ready for further use.
                DateTimeOffset utcNowSaved = DateTimeOffset.UtcNow;
                string amzLongDate = utcNowSaved.ToString("yyyyMMddTHHmmssZ");
                string amzShortDate = utcNowSaved.ToString("yyyyMMdd");

                // Add to headers. 
                msg.Headers.Add("x-amz-date", amzLongDate);
                msg.Headers.Add("x-amz-access-token", objSearchABProductsRequestInfo.AccessToken); // My API call needs an x-api-key passing also for function security.
                msg.Headers.Add("x-amz-user-email", objSearchABProductsRequestInfo.EmailAddress);
                // **************************************************** SIGNING PORTION ****************************************************
                // 1. Create Canonical Request            
                var canonicalRequest = new StringBuilder();
                canonicalRequest.Append(msg.Method + "\n");
                canonicalRequest.Append(string.Join("/", msg.RequestUri.AbsolutePath.Split('/').Select(Uri.EscapeDataString)) + "\n");

                canonicalRequest.Append(GetCanonicalQueryParams(msg) + "\n"); // Query params to do.

                var headersToBeSigned = new List<string>();
                foreach (var header in msg.Headers.OrderBy(a => a.Key.ToLowerInvariant(), StringComparer.OrdinalIgnoreCase))
                {
                    canonicalRequest.Append(header.Key.ToLowerInvariant());
                    canonicalRequest.Append(":");
                    canonicalRequest.Append(string.Join(",", header.Value.Select(s => s.Trim())));
                    canonicalRequest.Append("\n");
                    headersToBeSigned.Add(header.Key.ToLowerInvariant());
                }
                canonicalRequest.Append("\n");

                var signedHeaders = string.Join(";", headersToBeSigned);
                canonicalRequest.Append(signedHeaders + "\n");
                canonicalRequest.Append(objSearchABProductsRequestInfo.EmptyBodySignature); // Signature for empty body.
                                                                                            // Hash(msg.Content.ReadAsByteArrayAsync().Result);

                // 2. String to sign.            
                string stringToSign = objSearchABProductsRequestInfo.AWSHMACSignPrefix + "\n" + amzLongDate + "\n" + amzShortDate + "/" + objSearchABProductsRequestInfo.AWSRegion + "/" + objSearchABProductsRequestInfo.AWSServiceName + "/" + objSearchABProductsRequestInfo.AWSRequestType + "\n" + Hash(Encoding.UTF8.GetBytes(canonicalRequest.ToString()));

                // 3. Signature with compounded elements.
                var dateKey = HmacSha256(Encoding.UTF8.GetBytes(objSearchABProductsRequestInfo.AWSSigningVersion + objSearchABProductsRequestInfo.IAMSecretKey), amzShortDate);
                var dateRegionKey = HmacSha256(dateKey, objSearchABProductsRequestInfo.AWSRegion);
                var dateRegionServiceKey = HmacSha256(dateRegionKey, objSearchABProductsRequestInfo.AWSServiceName);
                var signingKey = HmacSha256(dateRegionServiceKey, objSearchABProductsRequestInfo.AWSRequestType);

                var signature = ToHexString(HmacSha256(signingKey, stringToSign.ToString()));

                // **************************************************** END SIGNING PORTION ****************************************************

                // Add the Header to the request.
                var credentialScope = amzShortDate + "/" + objSearchABProductsRequestInfo.AWSRegion + "/" + objSearchABProductsRequestInfo.AWSServiceName + "/" + objSearchABProductsRequestInfo.AWSRequestType;
                msg.Headers.TryAddWithoutValidation("Authorization", objSearchABProductsRequestInfo.AWSHMACSignPrefix + " Credential=" + objSearchABProductsRequestInfo.IAMAccessKey + "/" + credentialScope + ", SignedHeaders=" + signedHeaders + ", Signature=" + signature);

                // Invoke the request with HttpClient.
                HttpClient client = new HttpClient();
                HttpResponseMessage result = client.SendAsync(msg).Result;
                if (result.IsSuccessStatusCode)
                {
                    ReturnJSon = result.Content.ReadAsStringAsync().Result;
                    // Inspect the result and payload returned.
                    //Console.WriteLine(result.Headers);
                    //Console.WriteLine(result.Content.ReadAsStringAsync().Result);
                    // Wait on user.
                    //Console.ReadLine();
                }

            }
            catch (Exception ex)
            {
                CommonFunctions.SaveExceptionInTextFile("ABAPIHelper.GetABProductsDetails >>", ex);
            }
            return ReturnJSon;
        }
        private static SearchABProductsRequestInfo PrepareABProductDetailSearchRequest(string ASIN, long CompanyID, long RoomID, long UserID, string EnterPriseDBName)
        {
            SearchABProductsRequestInfo objSearchABProductsRequestInfo = new SearchABProductsRequestInfo();
            string SearchQuery = string.Empty;
            string FullApipathwithQuery = string.Empty;
            try
            {
                ABSetUpDAL objABSetUpDAL = new ABSetUpDAL(DbConnectionHelper.GeteTurnsQuickBookDBName());
                ABRoomStoreSettingDAL objABRoomStoreSettingDAL = new ABRoomStoreSettingDAL(EnterPriseDBName);
                ABRoomStoreSettingDTO objABRoomStoreSettingDTO = objABRoomStoreSettingDAL.GetABRoomStoreSetting(CompanyID, RoomID);
                ABDevAppSettingDTO objABDevAppSettingDTO = objABSetUpDAL.GetABDevAppSetting();
                if (objABRoomStoreSettingDTO != null && objABRoomStoreSettingDTO.ID > 0)
                {
                    SearchQuery = SearchQuery + "locale=" + (objABRoomStoreSettingDTO.ABLocale ?? "en_US");
                    SearchQuery = SearchQuery + "&productRegion=" + (objABRoomStoreSettingDTO.ABCountryCode ?? "US");
                    SearchQuery = SearchQuery + "&facets=" + (objABRoomStoreSettingDTO.ProductSearchFacets ?? "OFFERS,IMAGES");

                    ABMarketPlaceDTO objABMarketPlaceDTO = objABSetUpDAL.GetABMarketPlaceByMPID(objABRoomStoreSettingDTO.MarketplaceID);
                    ABAPIPathConfigDTO objABAPIPathConfigDTO = objABSetUpDAL.GetABAPIPathConfig("PRODUCT SEARCH API", "PRODUCT", "productsRequest");
                    FullApipathwithQuery = objABMarketPlaceDTO.ABAPIEndPoint + (objABAPIPathConfigDTO.ABAPIPath.Replace("{productId}", ASIN)) + "?" + SearchQuery;

                    objSearchABProductsRequestInfo.ApiQueryURL = FullApipathwithQuery;
                    objSearchABProductsRequestInfo.IAMAccessKey = objABDevAppSettingDTO.ABIAMAccessKey;
                    objSearchABProductsRequestInfo.IAMSecretKey = objABDevAppSettingDTO.ABIAMAccessKeySecret;
                    objSearchABProductsRequestInfo.AWSRegion = objABMarketPlaceDTO.AWSRegion;
                    objSearchABProductsRequestInfo.AccessToken = GetValidABAccessToken(RoomID, CompanyID, UserID, EnterPriseDBName, false);
                    objSearchABProductsRequestInfo.EmailAddress = objABRoomStoreSettingDTO.EmailAddress;
                }
            }
            catch (Exception ex)
            {
                CommonFunctions.SaveExceptionInTextFile("ABAPIHelper.PrepareABProductDetailSearchRequest >>" + ASIN, ex);
            }
            return objSearchABProductsRequestInfo;
        }
        private static ProductSearchQueryReqInfo ValidateProductSearchQueryParams(string k, string page, string PrimeEligible, string EligibleForFreeShipping, string DeliveryDay, string Category, string Availability, string SubCategory, long CompanyID, long RoomID, long UserID, string pageSize, string EnterPriseDBName)
        {
            int ItemsPerPage = 0;
            int.TryParse(pageSize, out ItemsPerPage);

            ProductSearchQueryReqInfo objSearchABProductsRequestInfo = new ProductSearchQueryReqInfo();
            objSearchABProductsRequestInfo.keywords = k;
            objSearchABProductsRequestInfo.PageNumber = page;
            objSearchABProductsRequestInfo.CompanyID = CompanyID;
            objSearchABProductsRequestInfo.RoomID = RoomID;
            objSearchABProductsRequestInfo.UserID = UserID;
            objSearchABProductsRequestInfo.PrimeEligible = PrimeEligible;
            objSearchABProductsRequestInfo.EligibleForFreeShipping = EligibleForFreeShipping;
            objSearchABProductsRequestInfo.DeliveryDay = DeliveryDay;
            objSearchABProductsRequestInfo.Category = Category;
            objSearchABProductsRequestInfo.Availability = Availability;
            objSearchABProductsRequestInfo.SubCategory = SubCategory;
            objSearchABProductsRequestInfo.pageSize = ItemsPerPage;
            objSearchABProductsRequestInfo.EnterPriseDBName = EnterPriseDBName;
            return objSearchABProductsRequestInfo;
        }
        public static string GetValidABAccessToken(long RoomID, long CompanyID, long UserID, string EnterPriseDBName, bool ForceGetFromAPI)
        {
            string AbAccessToken = string.Empty;
            try
            {
                if (!ForceGetFromAPI)
                {
                    ABRoomStoreSettingDAL objABRoomStoreSettingDAL = new ABRoomStoreSettingDAL(EnterPriseDBName);
                    ABSetUpDAL objABSetUpDAL = new ABSetUpDAL(DbConnectionHelper.GeteTurnsQuickBookDBName());
                    ABRoomStoreSettingDTO objABRoomStoreSettingDTO = objABRoomStoreSettingDAL.GetABRoomStoreSetting(CompanyID, RoomID);
                    if (objABRoomStoreSettingDTO != null && objABRoomStoreSettingDTO.ID > 0)
                    {
                        ABTokenDetailDTO objABTokenDetailDTO = objABSetUpDAL.GetABTokenDetailByAuthCode(objABRoomStoreSettingDTO.AuthCode);
                        if (objABTokenDetailDTO != null && !string.IsNullOrWhiteSpace(objABTokenDetailDTO.AccessToken))
                        {
                            if (objABTokenDetailDTO.AccessTokenExpireDate.HasValue)
                            {
                                DateTime AccessTokenExpireDate = objABTokenDetailDTO.AccessTokenExpireDate.Value.AddMinutes(-2);
                                if (AccessTokenExpireDate > DateTime.UtcNow)
                                {
                                    AbAccessToken = objABTokenDetailDTO.AccessToken;
                                }
                                else
                                {
                                    objABTokenDetailDTO = GenerateTokensbyRefreshToken(RoomID, CompanyID, UserID, EnterPriseDBName);
                                    AbAccessToken = objABTokenDetailDTO.AccessToken;
                                }
                            }
                        }
                    }
                }
                else
                {
                    ABTokenDetailDTO objABTokenDetailDTO = GenerateTokensbyRefreshToken(RoomID, CompanyID, UserID, EnterPriseDBName);
                    AbAccessToken = objABTokenDetailDTO.AccessToken;
                }
            }
            catch (Exception ex)
            {
                CommonFunctions.SaveExceptionInTextFile("ABAPIHelper.GetValidABAccessToken", ex);
            }
            return AbAccessToken;
        }
        private static SearchABProductsRequestInfo PrepareABProductOffersRequest(string ASIN, long CompanyID, long RoomID, long UserID, string EnterPriseDBName, Int64 pageNumber)
        {
            SearchABProductsRequestInfo objSearchABProductsRequestInfo = new SearchABProductsRequestInfo();
            string SearchQuery = string.Empty;
            string FullApipathwithQuery = string.Empty;
            try
            {
                ABSetUpDAL objABSetUpDAL = new ABSetUpDAL(DbConnectionHelper.GeteTurnsQuickBookDBName());
                ABRoomStoreSettingDAL objABRoomStoreSettingDAL = new ABRoomStoreSettingDAL(EnterPriseDBName);
                ABRoomStoreSettingDTO objABRoomStoreSettingDTO = objABRoomStoreSettingDAL.GetABRoomStoreSetting(CompanyID, RoomID);
                ABDevAppSettingDTO objABDevAppSettingDTO = objABSetUpDAL.GetABDevAppSetting();
                if (objABRoomStoreSettingDTO != null && objABRoomStoreSettingDTO.ID > 0)
                {
                    SearchQuery = SearchQuery + "locale=" + (objABRoomStoreSettingDTO.ABLocale ?? "en_US");
                    SearchQuery = SearchQuery + "&productRegion=" + (objABRoomStoreSettingDTO.ABCountryCode ?? "US");
                    SearchQuery = SearchQuery + "&facets=" + (objABRoomStoreSettingDTO.ProductSearchFacets ?? "OFFERS,IMAGES");
                    SearchQuery = SearchQuery + "&pageNumber=" + pageNumber;

                    ABMarketPlaceDTO objABMarketPlaceDTO = objABSetUpDAL.GetABMarketPlaceByMPID(objABRoomStoreSettingDTO.MarketplaceID);
                    ABAPIPathConfigDTO objABAPIPathConfigDTO = objABSetUpDAL.GetABAPIPathConfig("PRODUCT SEARCH API", "PRODUCT", "searchOffersRequest");
                    FullApipathwithQuery = objABMarketPlaceDTO.ABAPIEndPoint + (objABAPIPathConfigDTO.ABAPIPath.Replace("{productId}", ASIN)) + "?" + SearchQuery;

                    objSearchABProductsRequestInfo.ApiQueryURL = FullApipathwithQuery;
                    objSearchABProductsRequestInfo.IAMAccessKey = objABDevAppSettingDTO.ABIAMAccessKey;
                    objSearchABProductsRequestInfo.IAMSecretKey = objABDevAppSettingDTO.ABIAMAccessKeySecret;
                    objSearchABProductsRequestInfo.AWSRegion = objABMarketPlaceDTO.AWSRegion;
                    objSearchABProductsRequestInfo.AccessToken = GetValidABAccessToken(RoomID, CompanyID, UserID, EnterPriseDBName, false);
                    objSearchABProductsRequestInfo.EmailAddress = objABRoomStoreSettingDTO.EmailAddress;
                }
            }
            catch (Exception ex)
            {
                CommonFunctions.SaveExceptionInTextFile("ABAPIHelper.PrepareABProductOffersRequest >>" + ASIN, ex);
            }
            return objSearchABProductsRequestInfo;
        }
        private static string GetABProductOffers(SearchABProductsRequestInfo objSearchABProductsRequestInfo)
        {
            string ReturnJSon = string.Empty;
            try
            {
                // 0. Prepare request message.
                HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Get, objSearchABProductsRequestInfo.ApiQueryURL);
                msg.Headers.Host = msg.RequestUri.Host;

                // Get and save dates ready for further use.
                DateTimeOffset utcNowSaved = DateTimeOffset.UtcNow;
                string amzLongDate = utcNowSaved.ToString("yyyyMMddTHHmmssZ");
                string amzShortDate = utcNowSaved.ToString("yyyyMMdd");

                // Add to headers. 
                msg.Headers.Add("x-amz-date", amzLongDate);
                msg.Headers.Add("x-amz-access-token", objSearchABProductsRequestInfo.AccessToken); // My API call needs an x-api-key passing also for function security.
                msg.Headers.Add("x-amz-user-email", objSearchABProductsRequestInfo.EmailAddress);
                // **************************************************** SIGNING PORTION ****************************************************
                // 1. Create Canonical Request            
                var canonicalRequest = new StringBuilder();
                canonicalRequest.Append(msg.Method + "\n");
                canonicalRequest.Append(string.Join("/", msg.RequestUri.AbsolutePath.Split('/').Select(Uri.EscapeDataString)) + "\n");

                canonicalRequest.Append(GetCanonicalQueryParams(msg) + "\n"); // Query params to do.

                var headersToBeSigned = new List<string>();
                foreach (var header in msg.Headers.OrderBy(a => a.Key.ToLowerInvariant(), StringComparer.OrdinalIgnoreCase))
                {
                    canonicalRequest.Append(header.Key.ToLowerInvariant());
                    canonicalRequest.Append(":");
                    canonicalRequest.Append(string.Join(",", header.Value.Select(s => s.Trim())));
                    canonicalRequest.Append("\n");
                    headersToBeSigned.Add(header.Key.ToLowerInvariant());
                }
                canonicalRequest.Append("\n");

                var signedHeaders = string.Join(";", headersToBeSigned);
                canonicalRequest.Append(signedHeaders + "\n");
                canonicalRequest.Append(objSearchABProductsRequestInfo.EmptyBodySignature); // Signature for empty body.
                                                                                            // Hash(msg.Content.ReadAsByteArrayAsync().Result);

                // 2. String to sign.            
                string stringToSign = objSearchABProductsRequestInfo.AWSHMACSignPrefix + "\n" + amzLongDate + "\n" + amzShortDate + "/" + objSearchABProductsRequestInfo.AWSRegion + "/" + objSearchABProductsRequestInfo.AWSServiceName + "/" + objSearchABProductsRequestInfo.AWSRequestType + "\n" + Hash(Encoding.UTF8.GetBytes(canonicalRequest.ToString()));

                // 3. Signature with compounded elements.
                var dateKey = HmacSha256(Encoding.UTF8.GetBytes(objSearchABProductsRequestInfo.AWSSigningVersion + objSearchABProductsRequestInfo.IAMSecretKey), amzShortDate);
                var dateRegionKey = HmacSha256(dateKey, objSearchABProductsRequestInfo.AWSRegion);
                var dateRegionServiceKey = HmacSha256(dateRegionKey, objSearchABProductsRequestInfo.AWSServiceName);
                var signingKey = HmacSha256(dateRegionServiceKey, objSearchABProductsRequestInfo.AWSRequestType);

                var signature = ToHexString(HmacSha256(signingKey, stringToSign.ToString()));

                // **************************************************** END SIGNING PORTION ****************************************************

                // Add the Header to the request.
                var credentialScope = amzShortDate + "/" + objSearchABProductsRequestInfo.AWSRegion + "/" + objSearchABProductsRequestInfo.AWSServiceName + "/" + objSearchABProductsRequestInfo.AWSRequestType;
                msg.Headers.TryAddWithoutValidation("Authorization", objSearchABProductsRequestInfo.AWSHMACSignPrefix + " Credential=" + objSearchABProductsRequestInfo.IAMAccessKey + "/" + credentialScope + ", SignedHeaders=" + signedHeaders + ", Signature=" + signature);

                // Invoke the request with HttpClient.
                HttpClient client = new HttpClient();
                HttpResponseMessage result = client.SendAsync(msg).Result;
                if (result.IsSuccessStatusCode)
                {
                    ReturnJSon = result.Content.ReadAsStringAsync().Result;
                    // Inspect the result and payload returned.
                    //Console.WriteLine(result.Headers);
                    //Console.WriteLine(result.Content.ReadAsStringAsync().Result);
                    // Wait on user.
                    //Console.ReadLine();
                }

            }
            catch (Exception ex)
            {
                CommonFunctions.SaveExceptionInTextFile("ABAPIHelper.GetABProductsDetails >>", ex);
            }
            return ReturnJSon;
        }
        private static SearchABProductsRequestInfo PrepareABProductsSearchByASINsRequest(List<string> ASINs, long CompanyID, long RoomID, long UserID, string EnterPriseDBName
            , out int PageSize, out string RegionCurrencySymbol)
        {
            PageSize = 0;
            RegionCurrencySymbol = "$";
            SearchABProductsRequestInfo objSearchABProductsRequestInfo = new SearchABProductsRequestInfo();
            string SearchQuery = string.Empty;
            string FullApipathwithQuery = string.Empty;
            try
            {
                ABSetUpDAL objABSetUpDAL = new ABSetUpDAL(DbConnectionHelper.GeteTurnsQuickBookDBName());
                ABRoomStoreSettingDAL objABRoomStoreSettingDAL = new ABRoomStoreSettingDAL(EnterPriseDBName);
                ABRoomStoreSettingDTO objABRoomStoreSettingDTO = objABRoomStoreSettingDAL.GetABRoomStoreSetting(CompanyID, RoomID);
                ABDevAppSettingDTO objABDevAppSettingDTO = objABSetUpDAL.GetABDevAppSetting();
                if (objABRoomStoreSettingDTO != null && objABRoomStoreSettingDTO.ID > 0)
                {

                    if (objABRoomStoreSettingDTO.ProductSearchPageSize < 1 || objABRoomStoreSettingDTO.ProductSearchPageSize > 24)
                    {
                        PageSize = 24;
                    }
                    else
                    {
                        PageSize = objABRoomStoreSettingDTO.ProductSearchPageSize;
                    }

                    if (!string.IsNullOrEmpty(objABRoomStoreSettingDTO.RegionCurrencySymbol) && !string.IsNullOrWhiteSpace(objABRoomStoreSettingDTO.RegionCurrencySymbol))
                    {
                        RegionCurrencySymbol = objABRoomStoreSettingDTO.RegionCurrencySymbol;
                    }

                    ABMarketPlaceDTO objABMarketPlaceDTO = objABSetUpDAL.GetABMarketPlaceByMPID(objABRoomStoreSettingDTO.MarketplaceID);
                    ABAPIPathConfigDTO objABAPIPathConfigDTO = objABSetUpDAL.GetABAPIPathConfig("PRODUCT SEARCH API", "PRODUCT", "getProductsByAsins");
                    FullApipathwithQuery = objABMarketPlaceDTO.ABAPIEndPoint + (objABAPIPathConfigDTO.ABAPIPath);

                    objSearchABProductsRequestInfo.ApiQueryURL = FullApipathwithQuery;
                    objSearchABProductsRequestInfo.IAMAccessKey = objABDevAppSettingDTO.ABIAMAccessKey;
                    objSearchABProductsRequestInfo.IAMSecretKey = objABDevAppSettingDTO.ABIAMAccessKeySecret;
                    objSearchABProductsRequestInfo.AWSRegion = objABMarketPlaceDTO.AWSRegion;
                    objSearchABProductsRequestInfo.AccessToken = GetValidABAccessToken(RoomID, CompanyID, UserID, EnterPriseDBName, false);
                    objSearchABProductsRequestInfo.EmailAddress = objABRoomStoreSettingDTO.EmailAddress;
                    ABProductByASINPostReqInfo objABProductByASINPostReqInfo = new ABProductByASINPostReqInfo();
                    objABProductByASINPostReqInfo.productIds = ASINs;
                    objABProductByASINPostReqInfo.facets = (objABRoomStoreSettingDTO.ProductSearchFacets ?? "OFFERS,IMAGES").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    objABProductByASINPostReqInfo.productRegion = objABMarketPlaceDTO.AWSRegion;
                    objABProductByASINPostReqInfo.locale = (objABRoomStoreSettingDTO.ABLocale ?? "en_US");
                    objABProductByASINPostReqInfo.productRegion = (objABRoomStoreSettingDTO.ABCountryCode ?? "US");
                    objSearchABProductsRequestInfo.RequstBodyJSON = JsonConvert.SerializeObject(objABProductByASINPostReqInfo);
                }
            }
            catch (Exception ex)
            {
                CommonFunctions.SaveExceptionInTextFile("ABAPIHelper.PrepareABProductsSearchByASINsRequest >>", ex);
            }
            return objSearchABProductsRequestInfo;
        }
        private static string GetABProductsByASINs(SearchABProductsRequestInfo objSearchABProductsRequestInfo)
        {
            string ReturnJSon = string.Empty;
            try
            {
                // 0. Prepare request message.
                HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, objSearchABProductsRequestInfo.ApiQueryURL);
                msg.Headers.Host = msg.RequestUri.Host;

                // Get and save dates ready for further use.
                DateTimeOffset utcNowSaved = DateTimeOffset.UtcNow;
                string amzLongDate = utcNowSaved.ToString("yyyyMMddTHHmmssZ");
                string amzShortDate = utcNowSaved.ToString("yyyyMMdd");
                msg.Content = new StringContent(objSearchABProductsRequestInfo.RequstBodyJSON, Encoding.UTF8, "application/json");
                var hashedRequestPayload = HexEncode1(Hash1(ToBytes1(objSearchABProductsRequestInfo.RequstBodyJSON)));
                // Add to headers. 
                msg.Headers.Add("x-amz-date", amzLongDate);
                msg.Headers.Add("x-amz-access-token", objSearchABProductsRequestInfo.AccessToken); // My API call needs an x-api-key passing also for function security.
                msg.Headers.Add("x-amz-user-email", objSearchABProductsRequestInfo.EmailAddress);
                msg.Headers.Add("x-amz-content-sha256", hashedRequestPayload);
                // **************************************************** SIGNING PORTION ****************************************************
                // 1. Create Canonical Request            
                var canonicalRequest = new StringBuilder();
                canonicalRequest.Append(msg.Method + "\n");
                canonicalRequest.Append(string.Join("/", msg.RequestUri.AbsolutePath.Split('/').Select(Uri.EscapeDataString)) + "\n");

                canonicalRequest.Append(GetCanonicalQueryParams(msg) + "\n"); // Query params to do.

                var headersToBeSigned = new List<string>();
                foreach (var header in msg.Headers.OrderBy(a => a.Key.ToLowerInvariant(), StringComparer.OrdinalIgnoreCase))
                {
                    canonicalRequest.Append(header.Key.ToLowerInvariant());
                    canonicalRequest.Append(":");
                    canonicalRequest.Append(string.Join(",", header.Value.Select(s => s.Trim())));
                    canonicalRequest.Append("\n");
                    headersToBeSigned.Add(header.Key.ToLowerInvariant());
                }
                canonicalRequest.Append("\n");

                var signedHeaders = string.Join(";", headersToBeSigned);
                canonicalRequest.Append(signedHeaders + "\n");
                canonicalRequest.Append(hashedRequestPayload); // Signature for empty body.
                                                               // Hash(msg.Content.ReadAsByteArrayAsync().Result);

                // 2. String to sign.            
                string stringToSign = objSearchABProductsRequestInfo.AWSHMACSignPrefix + "\n" + amzLongDate + "\n" + amzShortDate + "/" + objSearchABProductsRequestInfo.AWSRegion + "/" + objSearchABProductsRequestInfo.AWSServiceName + "/" + objSearchABProductsRequestInfo.AWSRequestType + "\n" + Hash(Encoding.UTF8.GetBytes(canonicalRequest.ToString()));

                // 3. Signature with compounded elements.
                var dateKey = HmacSha256(Encoding.UTF8.GetBytes(objSearchABProductsRequestInfo.AWSSigningVersion + objSearchABProductsRequestInfo.IAMSecretKey), amzShortDate);
                var dateRegionKey = HmacSha256(dateKey, objSearchABProductsRequestInfo.AWSRegion);
                var dateRegionServiceKey = HmacSha256(dateRegionKey, objSearchABProductsRequestInfo.AWSServiceName);
                var signingKey = HmacSha256(dateRegionServiceKey, objSearchABProductsRequestInfo.AWSRequestType);



                var signature = ToHexString(HmacSha256(signingKey, stringToSign.ToString()));

                // **************************************************** END SIGNING PORTION ****************************************************

                // Add the Header to the request.
                var credentialScope = amzShortDate + "/" + objSearchABProductsRequestInfo.AWSRegion + "/" + objSearchABProductsRequestInfo.AWSServiceName + "/" + objSearchABProductsRequestInfo.AWSRequestType;
                msg.Headers.TryAddWithoutValidation("Authorization", objSearchABProductsRequestInfo.AWSHMACSignPrefix + " Credential=" + objSearchABProductsRequestInfo.IAMAccessKey + "/" + credentialScope + ", SignedHeaders=" + signedHeaders + ", Signature=" + signature);
                //msg.Headers.Add("Content-Type", "application/json");
                // Invoke the request with HttpClient.
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage result = client.SendAsync(msg).Result;
                if (result.IsSuccessStatusCode)
                {
                    ReturnJSon = result.Content.ReadAsStringAsync().Result;
                    // Inspect the result and payload returned.
                    //Console.WriteLine(result.Headers);
                    //Console.WriteLine(result.Content.ReadAsStringAsync().Result);
                    // Wait on user.
                    //Console.ReadLine();
                }

            }
            catch (Exception ex)
            {
                CommonFunctions.SaveExceptionInTextFile("ABAPIHelper.GetABProductsDetails >>", ex);
            }
            return ReturnJSon;
        }

        private static OrderSearchQueryReqInfo ValidateGetOrderByOrderDateQueryParams(DateTime StartDate, DateTime EndDate, bool IncludeLineItems, bool IncludeShipments, bool IncludeCharges,
            string NextPageToken, string PurchaseOrderNumber, long CompanyID, long RoomID, long UserID, string EnterPriseDBName)
        {
            OrderSearchQueryReqInfo orderByDateRequestInfo = new OrderSearchQueryReqInfo();
            orderByDateRequestInfo.startDate = StartDate.ToString("yyyy-MM-dd'T'HH:mm:ss");
            orderByDateRequestInfo.endDate = EndDate.ToString("yyyy-MM-dd'T'HH:mm:ss");
            orderByDateRequestInfo.CompanyID = CompanyID;
            orderByDateRequestInfo.RoomID = RoomID;
            orderByDateRequestInfo.UserID = UserID;
            orderByDateRequestInfo.includeLineItems = IncludeLineItems;
            orderByDateRequestInfo.includeShipments = IncludeShipments;
            orderByDateRequestInfo.includeCharges = IncludeCharges;
            orderByDateRequestInfo.EnterPriseDBName = EnterPriseDBName;
            orderByDateRequestInfo.nextPageToken = NextPageToken;
            orderByDateRequestInfo.purchaseOrderNumber = PurchaseOrderNumber;
            return orderByDateRequestInfo;
        }

        private static OrderByIdQueryReqInfo ValidateGetOrderByIdQueryParams(string OrderId, bool IncludeLineItems, bool IncludeShipments, bool IncludeCharges, long CompanyID, long RoomID,
                long UserID, string EnterPriseDBName)
        {
            OrderByIdQueryReqInfo orderByIdRequestInfo = new OrderByIdQueryReqInfo();
            orderByIdRequestInfo.OrderId = OrderId;
            orderByIdRequestInfo.includeLineItems = IncludeLineItems;
            orderByIdRequestInfo.includeShipments = IncludeShipments;
            orderByIdRequestInfo.includeCharges = IncludeCharges;
            orderByIdRequestInfo.CompanyID = CompanyID;
            orderByIdRequestInfo.RoomID = RoomID;
            orderByIdRequestInfo.UserID = UserID;
            orderByIdRequestInfo.EnterPriseDBName = EnterPriseDBName;
            return orderByIdRequestInfo;
        }


        private static SearchABProductsRequestInfo PrepareABOrderSearchRequest(OrderSearchQueryReqInfo objProductSearchQueryReqInfo)
        {
            SearchABProductsRequestInfo objSearchABOrdersRequestInfo = new SearchABProductsRequestInfo();
            string SearchQuery = string.Empty;
            string FullApipathwithQuery = string.Empty;

            try
            {
                ABSetUpDAL objABSetUpDAL = new ABSetUpDAL(DbConnectionHelper.GeteTurnsQuickBookDBName());
                ABRoomStoreSettingDAL objABRoomStoreSettingDAL = new ABRoomStoreSettingDAL(objProductSearchQueryReqInfo.EnterPriseDBName);
                ABRoomStoreSettingDTO objABRoomStoreSettingDTO = objABRoomStoreSettingDAL.GetABRoomStoreSetting(objProductSearchQueryReqInfo.CompanyID, objProductSearchQueryReqInfo.RoomID);
                ABDevAppSettingDTO objABDevAppSettingDTO = objABSetUpDAL.GetABDevAppSetting();

                if (objABRoomStoreSettingDTO != null && objABRoomStoreSettingDTO.ID > 0)
                {
                    SearchQuery = SearchQuery + "startDate=" + (objProductSearchQueryReqInfo.startDate ?? string.Empty);
                    SearchQuery = SearchQuery + "&endDate=" + (objProductSearchQueryReqInfo.endDate ?? string.Empty);

                    if (objProductSearchQueryReqInfo.includeLineItems)
                    {
                        SearchQuery = SearchQuery + "&includeLineItems=true";
                    }

                    if (objProductSearchQueryReqInfo.includeShipments)
                    {
                        SearchQuery = SearchQuery + "&includeShipments=true";
                    }

                    if (objProductSearchQueryReqInfo.includeCharges)
                    {
                        SearchQuery = SearchQuery + "&includeCharges=true";
                    }

                    ABMarketPlaceDTO objABMarketPlaceDTO = objABSetUpDAL.GetABMarketPlaceByMPID(objABRoomStoreSettingDTO.MarketplaceID);
                    ABAPIPathConfigDTO objABAPIPathConfigDTO = objABSetUpDAL.GetABAPIPathConfig("REPORTING API", "ORDER", "getOrdersByOrderDate");
                    FullApipathwithQuery = objABMarketPlaceDTO.ABAPIEndPoint + objABAPIPathConfigDTO.ABAPIPath + "?" + SearchQuery;

                    objSearchABOrdersRequestInfo.ApiQueryURL = FullApipathwithQuery;
                    objSearchABOrdersRequestInfo.IAMAccessKey = objABDevAppSettingDTO.ABIAMAccessKey;
                    objSearchABOrdersRequestInfo.IAMSecretKey = objABDevAppSettingDTO.ABIAMAccessKeySecret;
                    objSearchABOrdersRequestInfo.AWSRegion = objABMarketPlaceDTO.AWSRegion;
                    objSearchABOrdersRequestInfo.AccessToken = GetValidABAccessToken(objProductSearchQueryReqInfo.RoomID, objProductSearchQueryReqInfo.CompanyID, objProductSearchQueryReqInfo.UserID, objProductSearchQueryReqInfo.EnterPriseDBName, false);
                    objSearchABOrdersRequestInfo.EmailAddress = objABRoomStoreSettingDTO.EmailAddress;
                }
            }
            catch (Exception ex)
            {
                CommonFunctions.SaveExceptionInTextFile("ABAPIHelper.PrepareABOrderSearchRequest >>" + (objProductSearchQueryReqInfo.startDate ?? string.Empty), ex);
            }
            return objSearchABOrdersRequestInfo;
        }

        private static SearchABProductsRequestInfo PrepareGetOrderByOrderIdSearchRequest(OrderByIdQueryReqInfo objProductSearchQueryReqInfo)
        {
            SearchABProductsRequestInfo objSearchABOrdersRequestInfo = new SearchABProductsRequestInfo();
            string SearchQuery = string.Empty;
            string FullApipathwithQuery = string.Empty;

            try
            {
                ABSetUpDAL objABSetUpDAL = new ABSetUpDAL(DbConnectionHelper.GeteTurnsQuickBookDBName());
                ABRoomStoreSettingDAL objABRoomStoreSettingDAL = new ABRoomStoreSettingDAL(objProductSearchQueryReqInfo.EnterPriseDBName);
                ABRoomStoreSettingDTO objABRoomStoreSettingDTO = objABRoomStoreSettingDAL.GetABRoomStoreSetting(objProductSearchQueryReqInfo.CompanyID, objProductSearchQueryReqInfo.RoomID);
                ABDevAppSettingDTO objABDevAppSettingDTO = objABSetUpDAL.GetABDevAppSetting();

                if (objABRoomStoreSettingDTO != null && objABRoomStoreSettingDTO.ID > 0)
                {
                    if (objProductSearchQueryReqInfo.includeLineItems)
                    {
                        SearchQuery += (string.IsNullOrEmpty(SearchQuery) || string.IsNullOrWhiteSpace(SearchQuery)) ? "includeLineItems=true" : "&includeLineItems=true";
                    }

                    if (objProductSearchQueryReqInfo.includeShipments)
                    {
                        //SearchQuery = SearchQuery + "&includeShipments=true";
                        SearchQuery += (string.IsNullOrEmpty(SearchQuery) || string.IsNullOrWhiteSpace(SearchQuery)) ? "includeShipments=true" : "&includeShipments=true";
                    }

                    if (objProductSearchQueryReqInfo.includeCharges)
                    {
                        //SearchQuery = SearchQuery + "&includeCharges=true";
                        SearchQuery += (string.IsNullOrEmpty(SearchQuery) || string.IsNullOrWhiteSpace(SearchQuery)) ? "includeCharges=true" : "&includeCharges=true";
                    }

                    ABMarketPlaceDTO objABMarketPlaceDTO = objABSetUpDAL.GetABMarketPlaceByMPID(objABRoomStoreSettingDTO.MarketplaceID);
                    ABAPIPathConfigDTO objABAPIPathConfigDTO = objABSetUpDAL.GetABAPIPathConfig("REPORTING API", "ORDER", "getOrdersByOrderId");
                    FullApipathwithQuery = objABMarketPlaceDTO.ABAPIEndPoint + objABAPIPathConfigDTO.ABAPIPath.Replace("{orderId}", objProductSearchQueryReqInfo.OrderId) + "?" + SearchQuery;

                    objSearchABOrdersRequestInfo.ApiQueryURL = FullApipathwithQuery;
                    objSearchABOrdersRequestInfo.IAMAccessKey = objABDevAppSettingDTO.ABIAMAccessKey;
                    objSearchABOrdersRequestInfo.IAMSecretKey = objABDevAppSettingDTO.ABIAMAccessKeySecret;
                    objSearchABOrdersRequestInfo.AWSRegion = objABMarketPlaceDTO.AWSRegion;
                    objSearchABOrdersRequestInfo.AccessToken = GetValidABAccessToken(objProductSearchQueryReqInfo.RoomID, objProductSearchQueryReqInfo.CompanyID, objProductSearchQueryReqInfo.UserID, objProductSearchQueryReqInfo.EnterPriseDBName, false);
                    objSearchABOrdersRequestInfo.EmailAddress = objABRoomStoreSettingDTO.EmailAddress;
                }
            }
            catch (Exception ex)
            {
                CommonFunctions.SaveExceptionInTextFile("ABAPIHelper.PrepareGetOrderByOrderIdSearchRequest >>" + (objProductSearchQueryReqInfo.OrderId ?? string.Empty), ex);
            }
            return objSearchABOrdersRequestInfo;
        }

        private static string GetABOrders(SearchABProductsRequestInfo objSearchABProductsRequestInfo)
        {
            string ReturnJSon = string.Empty;
            try
            {
                // 0. Prepare request message.
                HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Get, objSearchABProductsRequestInfo.ApiQueryURL);
                msg.Headers.Host = msg.RequestUri.Host;

                // Get and save dates ready for further use.
                DateTimeOffset utcNowSaved = DateTimeOffset.UtcNow;
                string amzLongDate = utcNowSaved.ToString("yyyyMMddTHHmmssZ");
                string amzShortDate = utcNowSaved.ToString("yyyyMMdd");

                // Add to headers. 
                msg.Headers.Add("x-amz-date", amzLongDate);
                msg.Headers.Add("x-amz-access-token", objSearchABProductsRequestInfo.AccessToken); // My API call needs an x-api-key passing also for function security.
                msg.Headers.Add("x-amz-user-email", objSearchABProductsRequestInfo.EmailAddress);
                // **************************************************** SIGNING PORTION ****************************************************
                // 1. Create Canonical Request            
                var canonicalRequest = new StringBuilder();
                canonicalRequest.Append(msg.Method + "\n");
                canonicalRequest.Append(string.Join("/", msg.RequestUri.AbsolutePath.Split('/').Select(Uri.EscapeDataString)) + "\n");

                canonicalRequest.Append(GetCanonicalQueryParams(msg) + "\n"); // Query params to do.

                var headersToBeSigned = new List<string>();
                foreach (var header in msg.Headers.OrderBy(a => a.Key.ToLowerInvariant(), StringComparer.OrdinalIgnoreCase))
                {
                    canonicalRequest.Append(header.Key.ToLowerInvariant());
                    canonicalRequest.Append(":");
                    canonicalRequest.Append(string.Join(",", header.Value.Select(s => s.Trim())));
                    canonicalRequest.Append("\n");
                    headersToBeSigned.Add(header.Key.ToLowerInvariant());
                }
                canonicalRequest.Append("\n");

                var signedHeaders = string.Join(";", headersToBeSigned);
                canonicalRequest.Append(signedHeaders + "\n");
                canonicalRequest.Append(objSearchABProductsRequestInfo.EmptyBodySignature); // Signature for empty body.
                                                                                            // Hash(msg.Content.ReadAsByteArrayAsync().Result);

                // 2. String to sign.            
                string stringToSign = objSearchABProductsRequestInfo.AWSHMACSignPrefix + "\n" + amzLongDate + "\n" + amzShortDate + "/" + objSearchABProductsRequestInfo.AWSRegion + "/" + objSearchABProductsRequestInfo.AWSServiceName + "/" + objSearchABProductsRequestInfo.AWSRequestType + "\n" + Hash(Encoding.UTF8.GetBytes(canonicalRequest.ToString()));

                // 3. Signature with compounded elements.
                var dateKey = HmacSha256(Encoding.UTF8.GetBytes(objSearchABProductsRequestInfo.AWSSigningVersion + objSearchABProductsRequestInfo.IAMSecretKey), amzShortDate);
                var dateRegionKey = HmacSha256(dateKey, objSearchABProductsRequestInfo.AWSRegion);
                var dateRegionServiceKey = HmacSha256(dateRegionKey, objSearchABProductsRequestInfo.AWSServiceName);
                var signingKey = HmacSha256(dateRegionServiceKey, objSearchABProductsRequestInfo.AWSRequestType);

                var signature = ToHexString(HmacSha256(signingKey, stringToSign.ToString()));

                // **************************************************** END SIGNING PORTION ****************************************************

                // Add the Header to the request.
                var credentialScope = amzShortDate + "/" + objSearchABProductsRequestInfo.AWSRegion + "/" + objSearchABProductsRequestInfo.AWSServiceName + "/" + objSearchABProductsRequestInfo.AWSRequestType;
                msg.Headers.TryAddWithoutValidation("Authorization", objSearchABProductsRequestInfo.AWSHMACSignPrefix + " Credential=" + objSearchABProductsRequestInfo.IAMAccessKey + "/" + credentialScope + ", SignedHeaders=" + signedHeaders + ", Signature=" + signature);

                // Invoke the request with HttpClient.
                HttpClient client = new HttpClient();
                HttpResponseMessage result = client.SendAsync(msg).Result;
                if (result.IsSuccessStatusCode)
                {
                    ReturnJSon = result.Content.ReadAsStringAsync().Result;
                }

            }
            catch (Exception ex)
            {
                CommonFunctions.SaveExceptionInTextFile("ABAPIHelper.GetABOrders >>", ex);
            }
            return ReturnJSon;
        }

        private static OrderSearchQueryReqInfo ValidateGetOrderByNextPageTokenQueryParams(string NextPageToken, long CompanyID, long RoomID, long UserID, string EnterPriseDBName)
        {
            OrderSearchQueryReqInfo orderByDateRequestInfo = new OrderSearchQueryReqInfo();
            orderByDateRequestInfo.CompanyID = CompanyID;
            orderByDateRequestInfo.RoomID = RoomID;
            orderByDateRequestInfo.UserID = UserID;
            orderByDateRequestInfo.EnterPriseDBName = EnterPriseDBName;
            orderByDateRequestInfo.nextPageToken = NextPageToken;
            return orderByDateRequestInfo;
        }

        private static SearchABProductsRequestInfo PrepareABOrderByPageTokenSearchRequest(OrderSearchQueryReqInfo objProductSearchQueryReqInfo)
        {
            SearchABProductsRequestInfo objSearchABOrdersRequestInfo = new SearchABProductsRequestInfo();
            string SearchQuery = string.Empty;
            string FullApipathwithQuery = string.Empty;

            try
            {
                ABSetUpDAL objABSetUpDAL = new ABSetUpDAL(DbConnectionHelper.GeteTurnsQuickBookDBName());
                ABRoomStoreSettingDAL objABRoomStoreSettingDAL = new ABRoomStoreSettingDAL(objProductSearchQueryReqInfo.EnterPriseDBName);
                ABRoomStoreSettingDTO objABRoomStoreSettingDTO = objABRoomStoreSettingDAL.GetABRoomStoreSetting(objProductSearchQueryReqInfo.CompanyID, objProductSearchQueryReqInfo.RoomID);
                ABDevAppSettingDTO objABDevAppSettingDTO = objABSetUpDAL.GetABDevAppSetting();

                if (objABRoomStoreSettingDTO != null && objABRoomStoreSettingDTO.ID > 0)
                {
                    SearchQuery = SearchQuery + "nextPageToken=" + (objProductSearchQueryReqInfo.nextPageToken ?? string.Empty);
                    ABMarketPlaceDTO objABMarketPlaceDTO = objABSetUpDAL.GetABMarketPlaceByMPID(objABRoomStoreSettingDTO.MarketplaceID);
                    ABAPIPathConfigDTO objABAPIPathConfigDTO = objABSetUpDAL.GetABAPIPathConfig("REPORTING API", "ORDER", "getOrdersByOrderDate");
                    FullApipathwithQuery = objABMarketPlaceDTO.ABAPIEndPoint + objABAPIPathConfigDTO.ABAPIPath + "?" + SearchQuery;

                    objSearchABOrdersRequestInfo.ApiQueryURL = FullApipathwithQuery;
                    objSearchABOrdersRequestInfo.IAMAccessKey = objABDevAppSettingDTO.ABIAMAccessKey;
                    objSearchABOrdersRequestInfo.IAMSecretKey = objABDevAppSettingDTO.ABIAMAccessKeySecret;
                    objSearchABOrdersRequestInfo.AWSRegion = objABMarketPlaceDTO.AWSRegion;
                    objSearchABOrdersRequestInfo.AccessToken = GetValidABAccessToken(objProductSearchQueryReqInfo.RoomID, objProductSearchQueryReqInfo.CompanyID, objProductSearchQueryReqInfo.UserID, objProductSearchQueryReqInfo.EnterPriseDBName, false);
                    objSearchABOrdersRequestInfo.EmailAddress = objABRoomStoreSettingDTO.EmailAddress;
                }
            }
            catch (Exception ex)
            {
                CommonFunctions.SaveExceptionInTextFile("ABAPIHelper.PrepareABOrderSearchRequest >>" + (objProductSearchQueryReqInfo.startDate ?? string.Empty), ex);
            }
            return objSearchABOrdersRequestInfo;
        }

        private static string GetCanonicalQueryParams(HttpRequestMessage request)
        {
            var values = new SortedDictionary<string, string>();

            var querystring = HttpUtility.ParseQueryString(request.RequestUri.Query);
            foreach (var key in querystring.AllKeys)
            {
                if (key == null)//Handles keys without values
                {
                    values.Add(Uri.EscapeDataString(querystring[key]), $"{Uri.EscapeDataString(querystring[key])}=");
                }
                else
                {
                    // Escape to upper case. Required.
                    values.Add(Uri.EscapeDataString(key), $"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(querystring[key])}");
                }
            }
            // Put in order - this is important.
            var queryParams = values.Select(a => a.Value);
            return string.Join("&", queryParams);
        }
        private static string Hash(byte[] bytesToHash)
        {
            return ToHexString(SHA256.Create().ComputeHash(bytesToHash));
        }
        private static string ToHexString(IReadOnlyCollection<byte> array)
        {
            var hex = new StringBuilder(array.Count * 2);
            foreach (var b in array)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }
        private static byte[] HmacSha256(byte[] key, string data)
        {
            return new HMACSHA256(key).ComputeHash(Encoding.UTF8.GetBytes(data));
        }

        private static byte[] ToBytes(string str)
        {
            return Encoding.UTF8.GetBytes(str.ToCharArray());
        }

        private static string HexEncode(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", string.Empty).ToLowerInvariant();
        }



        private static byte[] ToBytes1(string str)
        {
            return Encoding.UTF8.GetBytes(str.ToCharArray());
        }

        private static string HexEncode1(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", string.Empty).ToLowerInvariant();
        }

        private static byte[] Hash1(byte[] bytes)
        {
            return SHA256.Create().ComputeHash(bytes);
        }

        private static byte[] HmacSha2561(String data, byte[] key)
        {
            return new HMACSHA256(key).ComputeHash(ToBytes(data));
        }

        private static InsertABOrderDTO GetInsertABOrderDTOFromABOrder(Order ABOrder, long RoomId, long CompanyId)
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
