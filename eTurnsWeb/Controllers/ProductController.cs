using System.Web.Mvc;
using eTurns.DTO;
using eTurns.DAL;
using eTurnsWeb.Helper;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using eTurns.DTO.Resources;
using Microsoft.AspNet.SignalR;
using eTurns.ABAPIBAL.Helper;

namespace eTurnsWeb.Controllers
{
    [AuthorizeHelper]
    public partial class ProductController : eTurnsControllerBase
    {
        public ActionResult ProductList(string k, string page, string PrimeEligible, string EligibleForFreeShipping, string DeliveryDay, string Category, string Availability, string SubCategory,
            string pageSize, string CategoryName, string SubCategoryName)
        {
            if (SessionHelper.AllowABIntegration)
            {
                ABRoomStoreSettingDAL abRoomStoreSettingDAL = new ABRoomStoreSettingDAL(SessionHelper.EnterPriseDBName);
                ABRoomStoreSettingDTO abRoomStoreSettingDTO = abRoomStoreSettingDAL.GetABRoomStoreSetting(SessionHelper.CompanyID, SessionHelper.RoomID);

                if (abRoomStoreSettingDTO == null || abRoomStoreSettingDTO.ID < 1 || string.IsNullOrEmpty(abRoomStoreSettingDTO.AuthCode) || string.IsNullOrWhiteSpace(abRoomStoreSettingDTO.AuthCode))
                {
                    return RedirectToAction("ABAccountSetup","Master", new { FromPage = "ProductList" });
                }

                if (!string.IsNullOrWhiteSpace(k))
                {
                    ProductSearchResultDTO objSearchResult = ABAPIHelper.SearchProductsRequest(k, page, PrimeEligible, EligibleForFreeShipping, DeliveryDay, Category, Availability, SubCategory, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, pageSize, SessionHelper.EnterPriseDBName);
                    ViewBag.CurrentFilter = k;
                    int tmpPageNumber = 1;

                    if (!string.IsNullOrWhiteSpace(page) && !string.IsNullOrEmpty(page))
                    {
                        int.TryParse(page, out tmpPageNumber);
                    }

                    var pageNumber = (string.IsNullOrWhiteSpace(page) || string.IsNullOrEmpty(page)) ? 1 : tmpPageNumber;
                    int startingRecord = 0;

                    if (objSearchResult != null && objSearchResult.products != null && objSearchResult.products.Any() && objSearchResult.products.Count > 0)
                    {
                        startingRecord = (objSearchResult.pageSize * pageNumber) - (objSearchResult.pageSize - 1);
                        var productDetailDAL = new ProductDetailsDAL(SessionHelper.EnterPriseDBName);
                        objSearchResult.RoomItemsASIN = productDetailDAL.GetABItemsAsinByRoomId(SessionHelper.RoomID);
                    }

                    int endingRecord = startingRecord;

                    if (objSearchResult != null && objSearchResult.products != null && objSearchResult.products.Any() && objSearchResult.products.Count > 1)
                    {
                        endingRecord = startingRecord + (objSearchResult.products.Count - 1);
                    }

                    ViewBag.StartingRecord = startingRecord;
                    ViewBag.EndingRecord = endingRecord;
                    ViewBag.PrimeEligible = PrimeEligible;
                    ViewBag.EligibleForFreeShipping = EligibleForFreeShipping;
                    ViewBag.DeliveryDay = DeliveryDay;
                    ViewBag.Category = Category;
                    ViewBag.CategoryName = CategoryName;
                    ViewBag.Availability = Availability;
                    //ViewBag.AvailabilityName = AvailabilityName;
                    ViewBag.SubCategory = SubCategory;
                    ViewBag.SubCategoryName = SubCategoryName;

                    if (string.IsNullOrEmpty(SubCategory) || string.IsNullOrWhiteSpace(SubCategory))
                    {
                        Session["ProductListSubCategories"] = null;
                    }

                    return View(objSearchResult);
                }
                else
                {
                    Session["ProductListSubCategories"] = null;
                    ViewBag.StartingRecord = 0;
                    ViewBag.EndingRecord = 0;
                    return View();
                }
            }
            else
            {
                return RedirectToAction("MyProfile", "Master");
            }

        }
        public ActionResult ProductListGrid(string k, string page, string PrimeEligible, string EligibleForFreeShipping, string DeliveryDay, string Category, string Availability, string SubCategory,
            string pageSize, string CategoryName, string SubCategoryName)
        {
            if (SessionHelper.AllowABIntegration)
            {
                ABRoomStoreSettingDAL abRoomStoreSettingDAL = new ABRoomStoreSettingDAL(SessionHelper.EnterPriseDBName);
                ABRoomStoreSettingDTO abRoomStoreSettingDTO = abRoomStoreSettingDAL.GetABRoomStoreSetting(SessionHelper.CompanyID, SessionHelper.RoomID);

                if (abRoomStoreSettingDTO == null || abRoomStoreSettingDTO.ID < 1 || string.IsNullOrEmpty(abRoomStoreSettingDTO.AuthCode) || string.IsNullOrWhiteSpace(abRoomStoreSettingDTO.AuthCode))
                {
                    return RedirectToAction("ABAccountSetup", "Master", new { FromPage = "ProductListGrid" });
                }

                if (!string.IsNullOrWhiteSpace(k))
                {
                    ProductSearchResultDTO objSearchResult = ABAPIHelper.SearchProductsRequest(k, page, PrimeEligible, EligibleForFreeShipping, DeliveryDay, Category, Availability, SubCategory, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, pageSize, SessionHelper.EnterPriseDBName);
                    ViewBag.CurrentFilter = k;
                    int tmpPageNumber = 1;

                    if (!string.IsNullOrWhiteSpace(page) && !string.IsNullOrEmpty(page))
                    {
                        int.TryParse(page, out tmpPageNumber);
                    }

                    var pageNumber = (string.IsNullOrWhiteSpace(page) || string.IsNullOrEmpty(page)) ? 1 : tmpPageNumber;
                    int startingRecord = 0;

                    if (objSearchResult != null && objSearchResult.products != null && objSearchResult.products.Any() && objSearchResult.products.Count > 0)
                    {
                        startingRecord = (objSearchResult.pageSize * pageNumber) - (objSearchResult.pageSize - 1);
                        var productDetailDAL = new ProductDetailsDAL(SessionHelper.EnterPriseDBName);
                        objSearchResult.RoomItemsASIN = productDetailDAL.GetABItemsAsinByRoomId(SessionHelper.RoomID);
                    }

                    int endingRecord = startingRecord;

                    if (objSearchResult != null && objSearchResult.products != null && objSearchResult.products.Any() && objSearchResult.products.Count > 1)
                    {
                        endingRecord = startingRecord + (objSearchResult.products.Count - 1);
                    }

                    ViewBag.StartingRecord = startingRecord;
                    ViewBag.EndingRecord = endingRecord;
                    ViewBag.PrimeEligible = PrimeEligible;
                    ViewBag.EligibleForFreeShipping = EligibleForFreeShipping;
                    ViewBag.DeliveryDay = DeliveryDay;
                    ViewBag.Category = Category;
                    ViewBag.CategoryName = CategoryName;
                    ViewBag.Availability = Availability;
                    //ViewBag.AvailabilityName = AvailabilityName;
                    ViewBag.SubCategory = SubCategory;
                    ViewBag.SubCategoryName = SubCategoryName;

                    if (string.IsNullOrEmpty(SubCategory) || string.IsNullOrWhiteSpace(SubCategory))
                    {
                        Session["ProductListGridSubCategories"] = null;
                    }

                    return View(objSearchResult);
                }
                else
                {
                    Session["ProductListGridSubCategories"] = null;
                    ViewBag.StartingRecord = 0;
                    ViewBag.EndingRecord = 0;
                    return View();
                }
            }
            else
            {
                return RedirectToAction("MyProfile", "Master");
            }
        }
        public ActionResult ProductEdit(string asin,string PageName)
        {
            if (SessionHelper.AllowABIntegration)
            {
                ABRoomStoreSettingDAL abRoomStoreSettingDAL = new ABRoomStoreSettingDAL(SessionHelper.EnterPriseDBName);
                ABRoomStoreSettingDTO abRoomStoreSettingDTO = abRoomStoreSettingDAL.GetABRoomStoreSetting(SessionHelper.CompanyID, SessionHelper.RoomID);

                if (abRoomStoreSettingDTO == null || abRoomStoreSettingDTO.ID < 1 || string.IsNullOrEmpty(abRoomStoreSettingDTO.AuthCode) || string.IsNullOrWhiteSpace(abRoomStoreSettingDTO.AuthCode))
                {
                    return RedirectToAction("ABAccountSetup", "Master", new { FromPage = "ProductEdit" });
                }

                ABProductDetails objSearchResult = ABAPIHelper.ProductsRequest(asin, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.EnterPriseDBName);
                //ViewBag.QueryString = QueryString;
                ViewBag.PageName = PageName;
                //ViewBag.SelectedABIndex = SelectedABIndex;
                //ViewBag.SelectedABValue = SelectedABValue;
                ABRoomStoreSettingDAL objABRoomStoreSettingDAL = new ABRoomStoreSettingDAL(SessionHelper.EnterPriseDBName);
                ABRoomStoreSettingDTO objABRoomStoreSettingDTO = objABRoomStoreSettingDAL.GetABRoomStoreSetting(SessionHelper.CompanyID, SessionHelper.RoomID);
                int ProductVariantLimit = (objABRoomStoreSettingDTO != null ? objABRoomStoreSettingDTO.ProductVariantLimit : 0);
                if (objABRoomStoreSettingDTO != null && objABRoomStoreSettingDTO.ID > 0
                    && !string.IsNullOrWhiteSpace(objABRoomStoreSettingDTO.RegionCurrencySymbol))
                {
                    ViewBag.RegionCurrencySymbol = objABRoomStoreSettingDTO.RegionCurrencySymbol;
                }
                else
                {
                    ViewBag.RegionCurrencySymbol = "$";
                }

                ViewBag.isAsinAddedToRoom = false;
                ViewBag.isDisplayBox = true;
                if (objSearchResult != null && !string.IsNullOrWhiteSpace(objSearchResult.asin))
                {
                    ProductDetailsDAL objProductDetailsDAL = new ProductDetailsDAL(SessionHelper.EnterPriseDBName);
                    Int64 ABItemMappingID = objProductDetailsDAL.CheckABItemExistInRoomByAsin(objSearchResult.asin, SessionHelper.CompanyID, SessionHelper.RoomID);
                    if (ABItemMappingID > 0)
                    {
                        ViewBag.isAsinAddedToRoom = true;
                    }
                    if (objSearchResult.productVariations != null)
                    {
                        Int64 dimensionValuesCount = (objSearchResult.productVariations.dimensions != null
                                                        ? objSearchResult.productVariations.dimensions.SelectMany(x => x.dimensionValues).Count()
                                                        : 0);
                        Int64 dimensionCount = (objSearchResult.productVariations.dimensions != null
                                                        ? objSearchResult.productVariations.dimensions.Count()
                                                        : 0);
                        if (dimensionValuesCount > ProductVariantLimit && dimensionCount <= 1)
                        {
                            ViewBag.isDisplayBox = false;
                        }
                        if (ViewBag.isDisplayBox == true)
                        {
                            List<string> lstAsin = new List<string>();
                            lstAsin = objSearchResult.productVariations.variations.Select(x => x.asin).ToList();
                            if (lstAsin != null && lstAsin.Count > 0)
                            {
                                ABProductByASINDTO abProductByASINDTO = ABAPIHelper.GetProductsByAsinsAll(lstAsin, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.EnterPriseDBName);
                                objSearchResult.variationWithPrice = new List<VariationWithPrice>();
                                if (abProductByASINDTO != null && abProductByASINDTO.products != null)
                                {
                                    foreach (Product product in abProductByASINDTO.products)
                                    {
                                        VariationWithPrice objVariationWithPrice = new VariationWithPrice();
                                        if (product.includedDataTypes != null && product.includedDataTypes.OFFERS != null
                                            && product.includedDataTypes.OFFERS.Count > 0)
                                        {
                                            double OfferPrice = 0;
                                            if (product.includedDataTypes.OFFERS.First().price != null
                                                && product.includedDataTypes.OFFERS.First().price.value != null)
                                            {
                                                OfferPrice = product.includedDataTypes.OFFERS.First().price.value.amount;
                                            }
                                            objVariationWithPrice.asin = product.asin;
                                            Value value = new Value();
                                            value.amount = OfferPrice;
                                            value.currencyCode = ViewBag.RegionCurrencySymbol;
                                            objVariationWithPrice.variationPrice = value;

                                            objSearchResult.variationWithPrice.Add(objVariationWithPrice);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    #region Get all offers

                    ABProductOffers objABProductOffers = ABAPIHelper.SearchAllOffersRequest(asin, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.EnterPriseDBName);
                    if (objABProductOffers != null && objABProductOffers.offerCount > 0)
                    {
                        objSearchResult.offerCount = objABProductOffers.offerCount;
                        objSearchResult.numberOfPages = objABProductOffers.numberOfPages;
                        objSearchResult.featuredOffer = objABProductOffers.featuredOffer;
                        objSearchResult.offers = objABProductOffers.offers;
                        objSearchResult.filterGroups = objABProductOffers.filterGroups;
                    }
                    #endregion
                }
                return View(objSearchResult);
            }
            else
            {
                return RedirectToAction("MyProfile", "Master");
            }
        }

        [HttpPost]
        public ActionResult SetProductListSubCategories(bool IsForClearSession, string SubCategoryId, string SubCategoryName)
        {
            if (IsForClearSession)
            {
                Session["ProductListSubCategories"] = null;
            }
            else
            {
                if (Session["ProductListSubCategories"] != null && ((List<Category>)Session["ProductListSubCategories"]).Any() && ((List<Category>)Session["ProductListSubCategories"]).Count > 0)
                {
                    var productSubCategories = (List<Category>)Session["ProductListSubCategories"];

                    if (productSubCategories != null)
                    {
                        var categoryExist = productSubCategories.Where(e => e.id == SubCategoryId).Count();

                        if (categoryExist > 0)
                        {
                            int index = productSubCategories.FindIndex(a => a.id == SubCategoryId);
                            var totalSubCategories = productSubCategories.Count;

                            if (productSubCategories.Count > (index + 1))
                            {
                                for (int i = (index + 1); i < totalSubCategories; i++)
                                {
                                    if (productSubCategories.Count > (index + 1))
                                    {
                                        productSubCategories.RemoveAt(index + 1);
                                    }
                                }
                            }

                        }
                        else
                        {
                            var subCategory = new Category { id = SubCategoryId, displayName = SubCategoryName };
                            productSubCategories.Add(subCategory);
                        }
                    }

                    Session["ProductListSubCategories"] = productSubCategories;
                }
                else
                {
                    var subCategory = new Category { id = SubCategoryId, displayName = SubCategoryName };
                    var productSubCategories = new List<Category>();
                    productSubCategories.Add(subCategory);
                    Session["ProductListSubCategories"] = productSubCategories;

                }
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SetProductListGridSubCategories(bool IsForClearSession, string SubCategoryId, string SubCategoryName)
        {
            if (IsForClearSession)
            {
                Session["ProductListGridSubCategories"] = null;
            }
            else
            {
                if (Session["ProductListGridSubCategories"] != null && ((List<Category>)Session["ProductListGridSubCategories"]).Any() && ((List<Category>)Session["ProductListGridSubCategories"]).Count > 0)
                {
                    var productSubCategories = (List<Category>)Session["ProductListGridSubCategories"];

                    if (productSubCategories != null)
                    {
                        var categoryExist = productSubCategories.Where(e => e.id == SubCategoryId).Count();

                        if (categoryExist > 0)
                        {
                            int index = productSubCategories.FindIndex(a => a.id == SubCategoryId);
                            var totalSubCategories = productSubCategories.Count;

                            if (productSubCategories.Count > (index + 1))
                            {
                                for (int i = (index + 1); i < totalSubCategories; i++)
                                {
                                    if (productSubCategories.Count > (index + 1))
                                    {
                                        productSubCategories.RemoveAt(index + 1);
                                    }
                                }
                            }

                        }
                        else
                        {
                            var subCategory = new Category { id = SubCategoryId, displayName = SubCategoryName };
                            productSubCategories.Add(subCategory);
                        }
                    }

                    Session["ProductListGridSubCategories"] = productSubCategories;
                }
                else
                {
                    var subCategory = new Category { id = SubCategoryId, displayName = SubCategoryName };
                    var productSubCategories = new List<Category>();
                    productSubCategories.Add(subCategory);
                    Session["ProductListGridSubCategories"] = productSubCategories;

                }
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveItemToRoom(ProductToItemQtyDTO SaveItemRequest)
        {
            if (ModelState.IsValid)
            {
                string returnMessage = string.Empty;
                var status = ABAPIHelper.SaveItemToRoom(SaveItemRequest,SessionHelper.RoomID,SessionHelper.CompanyID,SessionHelper.UserID,SessionHelper.UserName,SessionHelper.EnterPriseDBName,SessionHelper.EnterPriceID,out returnMessage);

                var Message = string.Empty;

                if (!string.IsNullOrEmpty(returnMessage) && !string.IsNullOrWhiteSpace(returnMessage))
                {
                    switch(returnMessage)
                    {
                        case "ResABProducts.ItemaddtoroomSuccessfully":
                            Message = ResABProducts.ItemaddtoroomSuccessfully;
                            break;
                        case "ResABProducts.ItemaddtoroomFail":
                            Message = ResABProducts.ItemaddtoroomFail;
                            break;
                        case "ResABProducts.AsinAlreadyExist":
                            Message = ResABProducts.AsinAlreadyExist;
                            break;
                        case "ResABProducts.ASINNotAvailableinResponse":
                            Message = ResABProducts.ASINNotAvailableinResponse;
                            break;
                        case "ResMessage.SaveMessage":
                            Message = ResMessage.SaveMessage;
                            break;
                        case "ResCartItem.QuantityAdjustmentMessage":
                            Message = ResCartItem.QuantityAdjustmentMessage;
                            break;
                        case "ResABProducts.ItemAddedToCartSuccessfully":
                            Message = ResABProducts.ItemAddedToCartSuccessfully;
                            break;
                        case "ResABProducts.FailToAddItemToCart":
                            Message = ResABProducts.FailToAddItemToCart;
                            break;

                    }
                }

                if (SaveItemRequest.CallFor == ProductToItemCallFor.AddToCart && SaveItemRequest.CartQty.GetValueOrDefault(0) > 0)
                {
                    GlobalHost.ConnectionManager.GetHubContext<eTurnsHub>().Clients.Group(SessionHelper.EnterPriceID + "_" + SessionHelper.CompanyID + "_" + SessionHelper.RoomID).UpdateRedCircleCountInClients();
                }

                return Json(new { Message = Message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            else
            {

                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();

                if (ModelState["CriticalQty"].Errors.Count > 0)
                {
                    return Json(new { Message = ModelState["CriticalQty"].Errors[0].ErrorMessage, Status = "InvalidModel" }, JsonRequestBehavior.AllowGet);
                }
                else if (ModelState["MinimumQty"].Errors.Count > 0)
                {
                    return Json(new { Message = ModelState["MinimumQty"].Errors[0].ErrorMessage, Status = "InvalidModel" }, JsonRequestBehavior.AllowGet);
                }
                else if (ModelState["MaximumQty"].Errors.Count > 0)
                {
                    return Json(new { Message = ModelState["MaximumQty"].Errors[0].ErrorMessage, Status = "InvalidModel" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Message = ResMessage.InvalidModel, Status = "InvalidModel" }, JsonRequestBehavior.AllowGet);
                }

                //return Json(new { Message = ResABProducts.ASINNotAvailableinResponse, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult RoomProductList(string page, string pageSize)
        {
            if (SessionHelper.AllowABIntegration)
            {
                ABRoomStoreSettingDAL abRoomStoreSettingDAL = new ABRoomStoreSettingDAL(SessionHelper.EnterPriseDBName);
                ABRoomStoreSettingDTO abRoomStoreSettingDTO = abRoomStoreSettingDAL.GetABRoomStoreSetting(SessionHelper.CompanyID, SessionHelper.RoomID);

                if (abRoomStoreSettingDTO == null || abRoomStoreSettingDTO.ID < 1 || string.IsNullOrEmpty(abRoomStoreSettingDTO.AuthCode) || string.IsNullOrWhiteSpace(abRoomStoreSettingDTO.AuthCode))
                {
                    return RedirectToAction("ABAccountSetup", "Master", new { FromPage = "RoomProductList" });
                }

                ABProductByASINDTO roomItems;

                if ((string.IsNullOrEmpty(page) || string.IsNullOrWhiteSpace(page)))
                {
                    var productDetailDAL = new ProductDetailsDAL(SessionHelper.EnterPriseDBName);
                    var productASINs = productDetailDAL.GetABItemsAsinByRoomId(SessionHelper.RoomID);
                    Session["RoomProductList"] = null;

                    if (productASINs != null && productASINs.Any() && productASINs.Count > 0)
                    {
                        ABProductByASINDTO result;
                        result = ABAPIHelper.GetProductsByAsinsAll(productASINs, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.EnterPriseDBName);

                        Session["RoomProductList"] = result;
                        roomItems = new ABProductByASINDTO { matchingProductCount = 0, numberOfPages = 0 };

                        if (result != null)
                        {
                            roomItems = (ABProductByASINDTO)result.Clone();

                            if (roomItems != null && roomItems.products != null && roomItems.products.Any() && roomItems.products.Count > 0)
                            {
                                if (roomItems.products.Count > roomItems.pageSize)
                                {
                                    double totalNoOfPages = (float)roomItems.products.Count / roomItems.pageSize;
                                    roomItems.products = roomItems.products.Take(roomItems.pageSize).ToList();
                                    roomItems.numberOfPages = Convert.ToInt32(Math.Ceiling(totalNoOfPages));
                                    result.numberOfPages = Convert.ToInt32(Math.Ceiling(totalNoOfPages));
                                }
                                else
                                {
                                    roomItems.numberOfPages = 1;
                                    result.numberOfPages = 1;
                                }
                            }
                        }
                    }
                    else
                    {
                        roomItems = new ABProductByASINDTO { matchingProductCount = 0, numberOfPages = 0 };
                    }
                }
                else
                {
                    if ((ABProductByASINDTO)Session["RoomProductList"] != null)
                    {
                        int tmpPageNumber = 1;
                        var items = (ABProductByASINDTO)Session["RoomProductList"];
                        roomItems = (ABProductByASINDTO)items.Clone();

                        if (!string.IsNullOrWhiteSpace(page) && !string.IsNullOrEmpty(page))
                        {
                            int.TryParse(page, out tmpPageNumber);
                        }

                        int currentPageSize = roomItems.pageSize;

                        if (!string.IsNullOrEmpty(pageSize) && !string.IsNullOrWhiteSpace(pageSize))
                        {
                            //int tmpPageSize = currentPageSize;
                            int.TryParse(pageSize, out currentPageSize);
                        }

                        if (roomItems != null && roomItems.products != null && roomItems.products.Any() && roomItems.products.Count > 0)
                        {
                            try
                            {
                                int startIndex = tmpPageNumber > 0 ? ((tmpPageNumber - 1) * currentPageSize) : 0;
                                roomItems.products = roomItems.products.Skip(startIndex).Take(currentPageSize).ToList();

                                double totalNoOfPages = (float)items.products.Count / currentPageSize;
                                roomItems.numberOfPages = Convert.ToInt32(Math.Ceiling(totalNoOfPages));
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                    else
                    {
                        roomItems = new ABProductByASINDTO { matchingProductCount = 0, numberOfPages = 0 };
                    }
                }
                return View(roomItems);
            }
            else
            {
                return RedirectToAction("MyProfile", "Master");
            }
        }

        public ActionResult RoomProductGrid(string page, string pageSize)
        {
            if (SessionHelper.AllowABIntegration)
            {
                ABRoomStoreSettingDAL abRoomStoreSettingDAL = new ABRoomStoreSettingDAL(SessionHelper.EnterPriseDBName);
                ABRoomStoreSettingDTO abRoomStoreSettingDTO = abRoomStoreSettingDAL.GetABRoomStoreSetting(SessionHelper.CompanyID, SessionHelper.RoomID);

                if (abRoomStoreSettingDTO == null || abRoomStoreSettingDTO.ID < 1 || string.IsNullOrEmpty(abRoomStoreSettingDTO.AuthCode) || string.IsNullOrWhiteSpace(abRoomStoreSettingDTO.AuthCode))
                {
                    return RedirectToAction("ABAccountSetup", "Master", new { FromPage = "RoomProductGrid" });
                }

                ABProductByASINDTO roomItems;

                if (string.IsNullOrEmpty(page) || string.IsNullOrWhiteSpace(page))
                {
                    var productDetailDAL = new ProductDetailsDAL(SessionHelper.EnterPriseDBName);
                    var productASINs = productDetailDAL.GetABItemsAsinByRoomId(SessionHelper.RoomID);
                    Session["RoomProductGrid"] = null;

                    if (productASINs != null && productASINs.Any() && productASINs.Count > 0)
                    {
                        ABProductByASINDTO result;
                        result = ABAPIHelper.GetProductsByAsinsAll(productASINs, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.EnterPriseDBName);
                        

                        Session["RoomProductGrid"] = result;
                        roomItems = new ABProductByASINDTO { matchingProductCount = 0, numberOfPages = 0 };

                        if (result != null)
                        {
                            roomItems = (ABProductByASINDTO)result.Clone();

                            if (roomItems != null && roomItems.products != null && roomItems.products.Any() && roomItems.products.Count > 0)
                            {
                                if (roomItems.products.Count > roomItems.pageSize)
                                {
                                    double totalNoOfPages = (float)roomItems.products.Count / roomItems.pageSize;
                                    roomItems.products = roomItems.products.Take(roomItems.pageSize).ToList();
                                    roomItems.numberOfPages = Convert.ToInt32(Math.Ceiling(totalNoOfPages));
                                    result.numberOfPages = Convert.ToInt32(Math.Ceiling(totalNoOfPages));
                                }
                                else
                                {
                                    roomItems.numberOfPages = 1;
                                    result.numberOfPages = 1;
                                }
                            }
                        }
                    }
                    else
                    {
                        roomItems = new ABProductByASINDTO { matchingProductCount = 0, numberOfPages = 0 };
                    }
                }
                else
                {
                    if ((ABProductByASINDTO)Session["RoomProductGrid"] != null)
                    {
                        int tmpPageNumber = 1;
                        var items = (ABProductByASINDTO)Session["RoomProductGrid"];
                        roomItems = (ABProductByASINDTO)items.Clone();

                        if (!string.IsNullOrWhiteSpace(page) && !string.IsNullOrEmpty(page))
                        {
                            int.TryParse(page, out tmpPageNumber);
                        }

                        int currentPageSize = roomItems.pageSize;

                        if (!string.IsNullOrEmpty(pageSize) && !string.IsNullOrWhiteSpace(pageSize))
                        {
                            //int tmpPageSize = currentPageSize;
                            int.TryParse(pageSize, out currentPageSize);
                        }

                        if (roomItems != null && roomItems.products != null && roomItems.products.Any() && roomItems.products.Count > 0)
                        {
                            try
                            {
                                int startIndex = tmpPageNumber > 0 ? ((tmpPageNumber - 1) * currentPageSize) : 0;
                                roomItems.products = roomItems.products.Skip(startIndex).Take(currentPageSize).ToList();
                                double totalNoOfPages = (float)items.products.Count / currentPageSize;
                                roomItems.numberOfPages = Convert.ToInt32(Math.Ceiling(totalNoOfPages));

                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                    else
                    {
                        roomItems = new ABProductByASINDTO { matchingProductCount = 0, numberOfPages = 0 };
                    }
                }
                return View(roomItems);
            }
            else
            {
                return RedirectToAction("MyProfile", "Master");
            }
        }

        [HttpPost]
        public ActionResult GetItemQuantityPopUp(ProductToItemQtyDTO PopUpRequest)
        {
            if (PopUpRequest != null && !string.IsNullOrEmpty(PopUpRequest.ASIN) && !string.IsNullOrWhiteSpace(PopUpRequest.ASIN))
            {
                ProductDetailsDAL objProductDetailsDAL = new ProductDetailsDAL(SessionHelper.EnterPriseDBName);
                long ABItemMappingID = objProductDetailsDAL.CheckABItemExistInRoomByAsin(PopUpRequest.ASIN, SessionHelper.CompanyID, SessionHelper.RoomID);
                PopUpRequest.IsExistingItem = ABItemMappingID > 0;
            }

            return PartialView("_ItemQuantities", PopUpRequest);
        }

        [HttpPost]
        public JsonResult ItemSyncToRoom()
        {
            ProductDetailsDAL productDetailsDAL = new ProductDetailsDAL(SessionHelper.EnterPriseDBName);
            List<string> ASINs = productDetailsDAL.GetAllABItemsAsinByRoomId(SessionHelper.RoomID, SessionHelper.CompanyID);
            if (ASINs.Count > 0)
            {
                Dictionary<List<ItemMasterDTO>, string> ReTurnMessage = ABAPIHelper.ItemSyncToRoom(ASINs, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.EnterPriseDBName);
                if (ReTurnMessage != null && ReTurnMessage.Values.Contains("success"))
                {
                    return Json(new { Message = ResABProducts.ItemsSyncSuccessfully, Status = "success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Message = ResABProducts.FailToSyncItems, Status = "fail" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { Message = "" });
        }
    
        public ActionResult SyncABOrders()
        {
            if (SessionHelper.AllowABIntegration) 
            {
                ABRoomStoreSettingDAL abRoomStoreSettingDAL = new ABRoomStoreSettingDAL(SessionHelper.EnterPriseDBName);
                ABRoomStoreSettingDTO abRoomStoreSettingDTO = abRoomStoreSettingDAL.GetABRoomStoreSetting(SessionHelper.CompanyID, SessionHelper.RoomID);

                if (abRoomStoreSettingDTO == null || abRoomStoreSettingDTO.ID < 1 || string.IsNullOrEmpty(abRoomStoreSettingDTO.AuthCode) || string.IsNullOrWhiteSpace(abRoomStoreSettingDTO.AuthCode))
                {
                    return RedirectToAction("ABAccountSetup", "Master", new { FromPage = "SyncABOrders" });
                }

                return View();
            }
            else
            {
                return RedirectToAction("MyProfile", "Master");
            }
        }

        [HttpPost]
        public ActionResult SyncABOrders(SyncABOrderDTO SyncABOrder)
        {
            try 
            {
                var startDate = DateTime.ParseExact(SyncABOrder.StartDateStr, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult);
                var endDate = DateTime.ParseExact(SyncABOrder.EndDateStr, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult);
                var abOrderDAL = new ABOrderDAL(DbConnectionHelper.GeteTurnsLoggingDBName());

                byte syncMode = (byte)ABOrderSyncMode.Online;

                bool isOfflineMode = false;
                if (!string.IsNullOrEmpty(SiteSettingHelper.ABOrderSyncMode) && !string.IsNullOrWhiteSpace(SiteSettingHelper.ABOrderSyncMode)
                    && (SiteSettingHelper.ABOrderSyncMode == ((byte)ABOrderSyncMode.Online).ToString() || SiteSettingHelper.ABOrderSyncMode == ((byte)ABOrderSyncMode.Offline).ToString()))
                {
                    syncMode = Convert.ToByte(SiteSettingHelper.ABOrderSyncMode);
                    //var enumDisplayStatus = (ABOrderSyncMode) Convert.ToByte(SiteSettingHelper.ABOrderSyncMode);
                    //string stringValue = enumDisplayStatus.ToString();
                }

                var id = abOrderDAL.InsertABOrderSyncRequest(startDate,endDate, syncMode,SessionHelper.EnterPriceID,SessionHelper.CompanyID,SessionHelper.RoomID,SessionHelper.UserID);
                var status = string.Empty;

                if (id > 0)
                {
                    if (syncMode == (byte)ABOrderSyncMode.Online)
                    {
                        ABAPIHelper.ImportABOrderToLocalDB(startDate, endDate, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.EnterPriseDBName);
                        ABAPIHelper.SyncABOrderedItemsToRoomItems(SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.UserName, SessionHelper.EnterPriseDBName,SessionHelper.EnterPriceID);
                        status = "success";
                    }
                    else 
                    {
                        status = "success";
                        isOfflineMode = true;
                    }
                }
                else
                {
                    status = "fail";
                }
                
                return Json(new { Message = ResItemMaster.OrderedItemsSyncSuccessfully, Status = status,ID = id, IsOfflineMode = isOfflineMode }, JsonRequestBehavior.AllowGet);
                
            }
            catch(Exception ex)
            {
                return Json(new { Message = ResItemMaster.FailToSyncOrderedItems, Status = "fail", ID = 0, IsOfflineMode = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult CheckABOrderSyncOfflineStatus(long Id)
        {
            var abOrderDAL = new ABOrderDAL(DbConnectionHelper.GeteTurnsLoggingDBName());
            bool isProcessCompleted = abOrderDAL.GetStatusOfABOrdersSyncOrNotById(Id);
            return Json(new { IsCompleted = isProcessCompleted }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ShowProductOffers(string asin)
        {
            if (SessionHelper.AllowABIntegration)
            {
                ABProductDetails objSearchResult = ABAPIHelper.ProductsRequest(asin, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.EnterPriseDBName);
                //ABProductOffers objABProductOffers = ABAPIHelper.SearchAllOffersRequest(asin, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.EnterPriseDBName);
                if (objSearchResult != null && !string.IsNullOrWhiteSpace(objSearchResult.asin))
                {
                    //objABProductOffers.offers = objABProductOffers.offers.OrderBy(x => x.price.value.amount).ToList();
                    //ViewBag.ProductImageAltText = ProductImageAltText;
                    //ViewBag.ProductImageURL = ProductImageURL;
                    //ViewBag.title = title;

                    ABRoomStoreSettingDAL objABRoomStoreSettingDAL = new ABRoomStoreSettingDAL(SessionHelper.EnterPriseDBName);
                    ABRoomStoreSettingDTO objABRoomStoreSettingDTO = objABRoomStoreSettingDAL.GetABRoomStoreSetting(SessionHelper.CompanyID, SessionHelper.RoomID);
                    if (objABRoomStoreSettingDTO != null && objABRoomStoreSettingDTO.ID > 0
                        && !string.IsNullOrWhiteSpace(objABRoomStoreSettingDTO.RegionCurrencySymbol))
                    {
                        ViewBag.RegionCurrencySymbol = objABRoomStoreSettingDTO.RegionCurrencySymbol;
                    }
                    else
                    {
                        ViewBag.RegionCurrencySymbol = "$";
                    }
                    #region Get all offers

                    ABProductOffers objABProductOffers = ABAPIHelper.SearchAllOffersRequest(asin, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.EnterPriseDBName);
                    if (objABProductOffers != null && objABProductOffers.offerCount > 0)
                    {
                        objSearchResult.offerCount = objABProductOffers.offerCount;
                        objSearchResult.numberOfPages = objABProductOffers.numberOfPages;
                        objSearchResult.featuredOffer = objABProductOffers.featuredOffer;
                        objSearchResult.offers = objABProductOffers.offers.OrderBy(x => x.price.value.amount).ToList(); ;
                        objSearchResult.filterGroups = objABProductOffers.filterGroups;
                    }
                    #endregion

                    return PartialView("_ProductOffers", objSearchResult);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return RedirectToAction("MyProfile", "Master");
            }
        }
    }
}
