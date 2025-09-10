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
using Newtonsoft.Json;

namespace eTurnsWeb.Helper
{
    public static class ABAPIHelper
    {
        #region [Public Method]
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
        public static ABProductOffers SearchOffersRequest(string asin, long CompanyID, long RoomID, long UserID, string EnterPriseDBName)
        {
            if (!string.IsNullOrWhiteSpace(asin))
            {
                SearchABProductsRequestInfo objDetailABProductsRequestInfo = PrepareABProductOffersRequest(asin, CompanyID, RoomID, UserID, EnterPriseDBName);
                string result = GetABProductOffers(objDetailABProductsRequestInfo);
                ABProductOffers objABProductOffers = JsonConvert.DeserializeObject<ABProductOffers>(result);
                return objABProductOffers;
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
                SearchABProductsRequestInfo objDetailABProductsRequestInfo = PrepareABProductsSearchByASINsRequest(ASINs, CompanyID, RoomID, UserID, EnterPriseDBName,out PgSize,out RegionCurrencySymbol);
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
            OrderSearchQueryReqInfo orderByOrderDateQueryReqInfo = ValidateGetOrderByOrderDateQueryParams(StartDate, EndDate, IncludeLineItems, IncludeShipments, IncludeCharges,NextPageToken,PurchaseOrderNumber , CompanyID, RoomID, UserID, EnterPriseDBName);
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
                    result = ABAPIHelper.GetProductsByAsins(productASINs, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.EnterPriseDBName);
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
                            var currentASINsResult = ABAPIHelper.GetProductsByAsins(currentRequestASINs, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.EnterPriseDBName);

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
                ABTokenDetailDTO objABTokenDetailDTODB = objABSetUpDAL.GetABTokenDetailByAuthCode(objABRoomStoreSettingDTO.AuthCode);
                if (objABRoomStoreSettingDTO != null && objABRoomStoreSettingDTO.ID > 0)
                {
                    ABMarketPlaceDTO objABMarketPlaceDTO = objABSetUpDAL.GetABMarketPlaceByMPID(objABRoomStoreSettingDTO.MarketplaceID);
                    string endPoint = objABMarketPlaceDTO.OAuthAuthAPIURI + objABDevAppSettingDTO.TokenRequestURL;
                    //string RetURL = CurrentDomainURL + objABDevAppSettingDTO.RedirectURL;
                    var client = new HttpClient();
                    var data = new[] {
                        new KeyValuePair<string, string>("client_secret", objABDevAppSettingDTO.LWAClientSecret),
                        new KeyValuePair<string, string>("client_id", objABDevAppSettingDTO.LWAClientID),
                        new KeyValuePair<string, string>("grant_type", "refresh_token"),
                        new KeyValuePair<string, string>("refresh_token", objABTokenDetailDTODB.RefreshToken)
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
                    objSearchABProductsRequestInfo.AccessToken = GetValidABAccessToken(objProductSearchQueryReqInfo.RoomID, objProductSearchQueryReqInfo.CompanyID, objProductSearchQueryReqInfo.UserID, objProductSearchQueryReqInfo.EnterPriseDBName);
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
                    objSearchABProductsRequestInfo.AccessToken = GetValidABAccessToken(RoomID, CompanyID, UserID, EnterPriseDBName);
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
        private static string GetValidABAccessToken(long RoomID, long CompanyID, long UserID, string EnterPriseDBName)
        {
            string AbAccessToken = string.Empty;
            try
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
                            DateTime AccessTokenExpireDate = objABTokenDetailDTO.AccessTokenExpireDate.Value.AddMinutes(-5);
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
            catch (Exception ex)
            {
                CommonFunctions.SaveExceptionInTextFile("ABAPIHelper.GetValidABAccessToken", ex);
            }
            return AbAccessToken;
        }
        private static SearchABProductsRequestInfo PrepareABProductOffersRequest(string ASIN, long CompanyID, long RoomID, long UserID, string EnterPriseDBName)
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
                    ABAPIPathConfigDTO objABAPIPathConfigDTO = objABSetUpDAL.GetABAPIPathConfig("PRODUCT SEARCH API", "PRODUCT", "searchOffersRequest");
                    FullApipathwithQuery = objABMarketPlaceDTO.ABAPIEndPoint + (objABAPIPathConfigDTO.ABAPIPath.Replace("{productId}", ASIN)) + "?" + SearchQuery;

                    objSearchABProductsRequestInfo.ApiQueryURL = FullApipathwithQuery;
                    objSearchABProductsRequestInfo.IAMAccessKey = objABDevAppSettingDTO.ABIAMAccessKey;
                    objSearchABProductsRequestInfo.IAMSecretKey = objABDevAppSettingDTO.ABIAMAccessKeySecret;
                    objSearchABProductsRequestInfo.AWSRegion = objABMarketPlaceDTO.AWSRegion;
                    objSearchABProductsRequestInfo.AccessToken = GetValidABAccessToken(RoomID, CompanyID, UserID, EnterPriseDBName);
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
                    objSearchABProductsRequestInfo.AccessToken = GetValidABAccessToken(RoomID, CompanyID, UserID, EnterPriseDBName);
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
            string NextPageToken , string PurchaseOrderNumber,long CompanyID, long RoomID, long UserID, string EnterPriseDBName)
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
                    objSearchABOrdersRequestInfo.AccessToken = GetValidABAccessToken(objProductSearchQueryReqInfo.RoomID, objProductSearchQueryReqInfo.CompanyID, objProductSearchQueryReqInfo.UserID, objProductSearchQueryReqInfo.EnterPriseDBName);
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
                    objSearchABOrdersRequestInfo.AccessToken = GetValidABAccessToken(objProductSearchQueryReqInfo.RoomID, objProductSearchQueryReqInfo.CompanyID, objProductSearchQueryReqInfo.UserID, objProductSearchQueryReqInfo.EnterPriseDBName);
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

        #endregion
    }
}