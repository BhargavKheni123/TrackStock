using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsWeb.Helper;
using eTurnsWeb.Models;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using eTurns.DTO.Utils;

namespace eTurnsWeb.Controllers
{
    [AuthorizeHelper]
    public class ReplenishController : eTurnsControllerBase
    {
        #region [Cart Item]

        public ActionResult CartItems()
        {
            List<BinMasterDTO> lstAllBins = new List<BinMasterDTO>();
            ViewBag.AllBinsOfRoom = lstAllBins;
            string CartCoockiename = "DefaultCartListview_" + eTurnsWeb.Helper.SessionHelper.UserID;

            if (Request.Cookies[CartCoockiename] != null)
            {
                string coockievalue = Convert.ToString(Request.Cookies[CartCoockiename].Value);
                if (coockievalue.Contains("CartItemList"))
                {
                    return RedirectToAction("CartItemList", "Replenish");
                }
            }

            return View();
        }

        public ActionResult CartItemList()
        {
            List<BinMasterDTO> lstAllBins = new List<BinMasterDTO>();
            ViewBag.AllBinsOfRoom = lstAllBins;
            string CartCoockiename = "DefaultCartListview_" + eTurnsWeb.Helper.SessionHelper.UserID;

            if (Request.Cookies[CartCoockiename] != null)
            {
                string coockievalue = Convert.ToString(Request.Cookies[CartCoockiename].Value);
                if (coockievalue.Contains("CartItems"))
                {
                    return RedirectToAction("CartItems", "Replenish");
                }
            }

            Session["AllCartItems"] = new CartItemDAL(SessionHelper.EnterPriseDBName).GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, true, SessionHelper.UserSupplierIds, SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowOrderToConsignedItem));

            RoomDAL objRoom = new RoomDAL(SessionHelper.EnterPriseDBName);
            RoomDTO objRoomDTO = new RoomDTO();
            //   objRoomDTO = objRoom.GetRoomByIDPlain(SessionHelper.RoomID);
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            string columnList = "ID,RoomName,SuggestedOrder,SuggestedTransfer,SuggestedReturn";
            objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");

            if (objRoomDTO != null && objRoomDTO.ID == SessionHelper.RoomID)
            {
                ViewBag.IsCartOrderTab = objRoomDTO.SuggestedOrder;
                ViewBag.IsCartTransferTab = objRoomDTO.SuggestedTransfer;
                ViewBag.IsCartSuggestedReturnTab = objRoomDTO.SuggestedReturn;
            }

            return View();
        }

        public ActionResult CartItemDetails(JQueryDataTableParamModel param)
        {
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            Guid ItemGUID = Guid.Parse(Request["ItemGUID"].ToString());
            ItemMasterDAL objItemMasterController = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"].ToString();

            // set the default column sorting here, if first time then required to set 
            if (sortColumnName == "0" || sortColumnName == "undefined")
                sortColumnName = "ID";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            bool UserConsignmentAllowed = SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowOrderToConsignedItem);
            CartItemDAL objCartItemDAL = new CartItemDAL(SessionHelper.EnterPriseDBName);
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            List<CartItemDTO> DataFromDB = objCartItemDAL.GetPagedRecordsCartListFromDB(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, ItemGUID.ToString(), SessionHelper.UserSupplierIds, UserConsignmentAllowed, string.Empty, RoomDateFormat, CurrentTimeZone);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);
        }

        public ActionResult CartItemListGroupedAjax(JQueryDataTableParamModel param)
        {
            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"].ToString();

            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());

            // set the default column sorting here, if first time then required to set 
            if (sortColumnName == "0" || sortColumnName == "undefined")
                sortColumnName = "ItemNumber";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;

            CartItemDAL objCartItemDAL = new CartItemDAL(SessionHelper.EnterPriseDBName);
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            List<CartItemDTO> DataFromDB = objCartItemDAL.GetPagedGroupedRecordsCartListFromDB(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, RoomDateFormat, CurrentTimeZone);

            ViewBag.TotalRecordCount = TotalRecordCount;

            var result = from c in DataFromDB
                         select new CartItemDTO
                         {
                             //ID = c.ItemID, / *ND*?
                             GUID = c.ItemGUID.GetValueOrDefault(Guid.Empty),
                             ItemGUID = c.ItemGUID,
                             ItemNumber = c.ItemNumber,
                             Quantity = c.Quantity,
                             ReplenishType = c.ReplenishType,
                             RoomName = c.RoomName,
                             Created = c.Created,
                             LastUpdatedBy = c.LastUpdatedBy,
                             UpdatedByName = c.UpdatedByName,
                             CreatedByName = c.CreatedByName,
                             IsDeleted = IsDeleted,
                             IsArchived = IsArchived,
                             UDF1 = c.UDF1,
                             UDF2 = c.UDF2,
                             UDF3 = c.UDF3,
                             UDF4 = c.UDF4,
                             UDF5 = c.UDF5,
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);
        }

        public ActionResult CartItemListAjax(JQueryDataTableParamModel param)
        {
            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"].ToString();
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            string ReplenishType = Convert.ToString(Request["ReplenishType"].ToString());

            //-------------Remove ReplenishType From Search If Filtered By Variable-------------
            //
            if (!String.IsNullOrEmpty(ReplenishType) && !String.IsNullOrWhiteSpace(ReplenishType))
            {
                if (ReplenishType == "List")
                    ReplenishType = "";

                if (param.sSearch != null && !string.IsNullOrEmpty(param.sSearch) && !string.IsNullOrWhiteSpace(param.sSearch))
                {
                    param.sSearch = CommonUtility.SetjQueryTableSearchValue(param.sSearch, "ReplenishType", ReplenishType);
                    ReplenishType = "";
                }
            }

            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "ID";

                if (sortDirection == "asc")
                    sortColumnName = sortColumnName + " asc";
                else
                    sortColumnName = sortColumnName + " desc";
            }
            else
                sortColumnName = "ID desc";


            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            bool UserConsignmentAllowed = SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowOrderToConsignedItem);
            CartItemDAL objCartItemDAL = new CartItemDAL(SessionHelper.EnterPriseDBName);
            ItemMasterDAL objItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            BinMasterDAL objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            List<CartItemDTO> DataFromDB = objCartItemDAL.GetPagedRecordsCartListFromDB(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, "", SessionHelper.UserSupplierIds, UserConsignmentAllowed, ReplenishType, RoomDateFormat, CurrentTimeZone);
            //  List<ItemMasterDTO> lstAllItems = objItemDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            List<BinMasterDTO> objBinMasterListDTO = objBinDAL.GetBinMasterByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
            foreach (CartItemDTO dr in DataFromDB)
            {
                BinMasterDTO objBinMasterDTO = new BinMasterDTO();
                ItemMasterDTO objItemDTO = objItemDAL.GetRecordByItemGUID(dr.ItemGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID,SessionHelper.CompanyID);
                //  ItemMasterDTO objItemDTO = lstAllItems.FirstOrDefault(p => p.GUID == dr.ItemGUID.GetValueOrDefault(Guid.Empty));
                List<BinMasterDTO> lstItemBins = objBinMasterListDTO.Where(p => p.ItemGUID == objItemDTO.GUID).OrderBy(x => x.BinNumber).ToList();
                if (objItemDTO != null)
                {
                    if (objItemDTO.OrderUOMValue == null || objItemDTO.OrderUOMValue <= 0)
                        objItemDTO.OrderUOMValue = 1;
                }
                dr.IsAllowOrderCostuom = objItemDTO.IsAllowOrderCostuom;
                dr.OrderUOMValue = objItemDTO.OrderUOMValue;
                dr.IsEnforceDefaultReorderQuantity = false;
                if (dr.BinId.HasValue && lstItemBins != null)
                {
                    objBinMasterDTO = lstItemBins.Where(x => x.ID == dr.BinId.GetValueOrDefault(0)).FirstOrDefault();
                }

                if (objBinMasterDTO != null && objBinMasterDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                {
                    dr.IsEnforceDefaultReorderQuantity = objBinMasterDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false);
                    double newOrderQty = 0;
                    int devideval = 0;
                    double tempQty = dr.Quantity ?? 0;
                    double drq = objBinMasterDTO.DefaultReorderQuantity ?? 0;
                    if (tempQty > 0 && drq > 0)
                    {
                        if ((tempQty % drq) != 0)
                        {
                            devideval = Convert.ToInt32((Convert.ToInt32(tempQty) / Convert.ToInt32(drq))) + 1;
                            newOrderQty = drq * devideval;
                        }
                        else
                        {
                            newOrderQty = tempQty;
                        }
                    }
                    dr.Quantity = newOrderQty;
                    if (objItemDTO.IsAllowOrderCostuom)
                    {
                        dr.Quantity = dr.Quantity.GetValueOrDefault(0) / objItemDTO.OrderUOMValue.GetValueOrDefault(0);
                    }
                    dr.DefaultReorderQuantity = objBinMasterDTO.DefaultReorderQuantity;
                }
                else if (objItemDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                {
                    dr.IsEnforceDefaultReorderQuantity = objItemDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false);
                    double newOrderQty = 0;
                    int devideval = 0;
                    double tempQty = dr.Quantity ?? 0;
                    double drq = objItemDTO.DefaultReorderQuantity ?? 0;
                    if (tempQty > 0 && drq > 0)
                    {
                        if ((tempQty % drq) != 0)
                        {
                            devideval = Convert.ToInt32((Convert.ToInt32(tempQty) / Convert.ToInt32(drq))) + 1;
                            newOrderQty = drq * devideval;
                        }
                        else
                        {
                            newOrderQty = tempQty;
                        }
                    }
                    dr.Quantity = newOrderQty;
                    if (objItemDTO.IsAllowOrderCostuom)
                    {
                        dr.Quantity = dr.Quantity.GetValueOrDefault(0) / objItemDTO.OrderUOMValue.GetValueOrDefault(0);
                    }
                    dr.DefaultReorderQuantity = objItemDTO.DefaultReorderQuantity;
                }
            }
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
                                    JsonRequestBehavior.AllowGet);
        }

        public string DeleteCartItemRecords(string ids)
        {
            try
            {
                long SessionUserId = SessionHelper.UserID;
                CartItemDAL objCartItemAPIController = new CartItemDAL(SessionHelper.EnterPriseDBName);
                objCartItemAPIController.DeleteRecords(ids, SessionHelper.UserID, SessionHelper.CompanyID, SessionHelper.EnterPriceID, SessionUserId);
                GlobalHost.ConnectionManager.GetHubContext<eTurnsHub>().Clients.Group(SessionHelper.EnterPriceID + "_" + SessionHelper.CompanyID + "_" + SessionHelper.RoomID).UpdateRedCircleCountInClients();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public ActionResult NewCartItem()
        {
            ItemModelPerameter obj = new ItemModelPerameter()
            {
                AjaxURLAddItemToSession = "~/QuickList/AddItemToSession/",
                ModelHeader = eTurns.DTO.ResQuickList.ModelHeader,
                AjaxURLAddMultipleItemToSession = "~/QuickList/AddItemToSessionMultiple/",
                AjaxURLToFillItemGrid = "~/Replenish/GetDataForCartNew/"
            };
            Session["ItemMasterList"] = null;

            return PartialView("_NewCartItem", obj);
        }

        public JsonResult UpdateCartItemData(Int64 ID, string ItemGUID, string ItemNumber, double? CartQty, string BinName, string ReplenishType, string UDF1, string UDF2, string UDF3, string UDF4, string UDF5, bool IsFromUI)
        {
            long retid = 0;
            string message = ResMessage.SaveMessage;
            string status = "";
            CartItemDAL objCartItemDAL = new CartItemDAL(SessionHelper.EnterPriseDBName);
            CartItemDTO objCartItemDTO = new CartItemDTO();
            BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            objCartItemDTO.Created = DateTimeUtility.DateTimeNow;
            objCartItemDTO.CreatedByName = SessionHelper.UserName;
            objCartItemDTO.CreatedBy = SessionHelper.UserID;
            objCartItemDTO.Updated = DateTimeUtility.DateTimeNow;
            objCartItemDTO.UpdatedByName = SessionHelper.UserName;
            objCartItemDTO.LastUpdatedBy = SessionHelper.UserID;
            objCartItemDTO.ID = ID;
            objCartItemDTO.IsOnlyFromItemUI = IsFromUI;
            objCartItemDTO.IsAutoMatedEntry = false;

            if (!string.IsNullOrWhiteSpace(BinName))
            {
                BinMasterDTO objBinMasterDTO = objBinMasterDAL.GetItemBinPlain(Guid.Parse(ItemGUID), BinName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, false);
                objCartItemDTO.BinId = objBinMasterDTO.ID;
                objCartItemDTO.BinGUID = objBinMasterDTO.GUID;
            }
            else
            {
                objCartItemDTO.BinId = null;
                objCartItemDTO.BinGUID = null;
            }
            objCartItemDTO.ItemNumber = ItemNumber;
            objCartItemDTO.UDF1 = UDF1;
            objCartItemDTO.UDF2 = UDF2;
            objCartItemDTO.UDF3 = UDF3;
            objCartItemDTO.UDF4 = UDF4;
            objCartItemDTO.UDF5 = UDF5;
            objCartItemDTO.ReplenishType = ReplenishType;
            if (!string.IsNullOrEmpty(ItemGUID))
                objCartItemDTO.ItemGUID = Guid.Parse(ItemGUID);
            objCartItemDTO.CompanyID = SessionHelper.CompanyID;
            objCartItemDTO.Room = SessionHelper.RoomID;
            objCartItemDTO.Quantity = CartQty;
            objCartItemDTO.IsDeleted = false;
            objCartItemDTO.IsArchived = false;
            long SessionUserId = SessionHelper.UserID;
            try
            {
                ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                objItemMasterDTO = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, objCartItemDTO.ItemGUID);

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
                if (ID > 0)
                {
                    if (IsFromUI)
                    {
                        objCartItemDTO.EditedFrom = "Web";
                        objCartItemDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    }
                }
                else
                {
                    objCartItemDTO.AddedFrom = "Web";
                    objCartItemDTO.EditedFrom = "Web";
                    objCartItemDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objCartItemDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                }
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
                objCartItemDTO = objCartItemDAL.SaveCartItem(objCartItemDTO, SessionUserId, SessionHelper.EnterPriceID);
                objCartItemDAL.AutoCartUpdateByCode(Guid.Parse(ItemGUID), SessionHelper.UserID, "Web", "Replenish >> Modified Cart", SessionUserId);

                message = ResCartItem.QuantityAdjustmentMessage;
                status = "ok";
                if (objCartItemDTO != null && objCartItemDTO.EnforsedCartQuanity == true)
                {
                    message = ResCartItem.QuantityAdjustmentMessage;
                }
                else
                {
                    message = ResMessage.SaveMessage;
                }
            }
            catch (Exception)
            {
                message = ResMessage.SaveErrorMsg;
                status = "fail";
            }
            double SumperItem = objCartItemDAL.GetCartsByItemGUIDPlain(Guid.Parse(ItemGUID), SessionHelper.RoomID, SessionHelper.CompanyID).Sum(t => t.Quantity) ?? 0;

            if (retid > 0)
            {
                objCartItemDTO = objCartItemDAL.GetCartByID(retid, SessionHelper.RoomID, SessionHelper.CompanyID);
            }

            GlobalHost.ConnectionManager.GetHubContext<eTurnsHub>().Clients.Group(SessionHelper.EnterPriceID + "_" + SessionHelper.CompanyID + "_" + SessionHelper.RoomID).UpdateRedCircleCountInClients();

            return Json(new { Message = message, Status = status, TotalQty = SumperItem, CartObj = objCartItemDTO }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateCartItemsBulk(string strCartItems, bool IsOnlyFromUI)
        {
            string message = ResMessage.SaveMessage;
            List<CartItemDTO> lstCartItems = (List<CartItemDTO>)JsonConvert.DeserializeObject(strCartItems, typeof(List<CartItemDTO>));
            lstCartItems.ForEach(t =>
            {
                t.CompanyID = SessionHelper.CompanyID;
                t.Created = DateTimeUtility.DateTimeNow;
                t.CreatedBy = SessionHelper.UserID;
                t.CreatedByName = SessionHelper.UserName;
                t.IsArchived = false;
                t.IsDeleted = false;
                t.IsKitComponent = false;
                t.LastUpdatedBy = SessionHelper.UserID;
                t.Room = SessionHelper.RoomID;
                t.RoomName = SessionHelper.RoomName;
                t.Status = "A";
                t.Updated = DateTimeUtility.DateTimeNow;
                t.UpdatedByName = SessionHelper.UserName;
                t.IsAutoMatedEntry = false;
                t.IsOnlyFromItemUI = IsOnlyFromUI;
            });

            string status = "";
            string locationMSG = "";
            CartItemDAL objCartItemAPIController = new CartItemDAL(SessionHelper.EnterPriseDBName);
            CartItemDTO objCartItemDTO = new CartItemDTO();
            long SessionUserId = SessionHelper.UserID;
            try
            {
                List<CartItemDTO> lstreturnitems = objCartItemAPIController.SaveCartItems(lstCartItems, SessionHelper.EnterPriceID, SessionUserId);
                if (lstreturnitems != null && lstreturnitems.Count > 0 && lstreturnitems.Count(t => t.EnforsedCartQuanity == true) > 0)
                {
                    message = ResCartItem.QuantityAdjustmentMessage;
                }
                else
                {
                    message = ResMessage.SaveMessage;
                }
                status = "ok";
            }
            catch
            {
                message = ResMessage.SaveErrorMsg;
                status = "fail";
            }
            GlobalHost.ConnectionManager.GetHubContext<eTurnsHub>().Clients.Group(SessionHelper.EnterPriceID + "_" + SessionHelper.CompanyID + "_" + SessionHelper.RoomID).UpdateRedCircleCountInClients();

            return Json(new { Message = message, Status = status, LocationMSG = locationMSG }, JsonRequestBehavior.AllowGet);
        }

        public string CartItemDetailsSub(string ItemGUID, bool IsDeleted, bool IsArchived)
        {
            List<BinMasterDTO> lstAllBins = new List<BinMasterDTO>();
            lstAllBins = new List<BinMasterDTO>();
            ViewBag.AllBinsOfRoom = lstAllBins;
            ViewBag.ItemGUID = ItemGUID;
            ViewBag.IsDeletedchked = IsDeleted;
            ViewBag.IsArchivedchked = IsArchived;
            CartItemDAL objCartItemAPIController = new CartItemDAL(SessionHelper.EnterPriseDBName);
            var objModel = objCartItemAPIController.GetCartsByItemGUID(Guid.Parse(ItemGUID), SessionHelper.RoomID, SessionHelper.CompanyID);

            return RenderRazorViewToString("_CartItemDetails", objModel);
        }

        public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        public ActionResult CartItemHistory(string GUID)
        {
            ViewBag.CartItemId = GUID ?? "";
            return PartialView("~/Views/Replenish/CartItemsHistory.cshtml");
        }

        public ActionResult GetItemsModelMethod(QuickListJQueryDataTableParamModel param)
        {
            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            bool UserConsignmentAllowed = SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowOrderToConsignedItem);
            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"].ToString();
            bool IsArchived = false; //bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = false; //bool.Parse(Request["IsDeleted"].ToString());

            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "ItemNumber desc";
            }
            else
                sortColumnName = "ItemNumber desc";

            string searchQuery = string.Empty;

            Int64 QLID = 0;
            Int64.TryParse(Request["ParentID"], out QLID);

            int TotalRecordCount = 0;
            List<ItemMasterDTO> DataFromDB = new List<ItemMasterDTO>();

            if (param.sSearch != null && param.sSearch.Contains("QLGuid="))
            {
                QuickListDAL objQLDtlDAL = new QuickListDAL(SessionHelper.EnterPriseDBName);
                string QLGuid = string.Empty;
                double QLQty = 0;
                string[] txtSearch = new string[2];
                if (param.sSearch != null && param.sSearch.Contains("QLQty="))
                {
                    txtSearch = param.sSearch.Split(',');
                    QLGuid = txtSearch[0].Split('=')[1];
                    QLQty = Convert.ToDouble(txtSearch[1].Split('=')[1]);
                }
                else
                {
                    QLGuid = param.sSearch.Replace("QLGuid=", "").ToString();
                }

                if (QLQty <= 0)
                    QLQty = 1;

                List<QuickListDetailDTO> objQLDtlDTO = objQLDtlDAL.GetQuickListItemsRecords(SessionHelper.RoomID, SessionHelper.CompanyID, QLGuid, SessionHelper.UserSupplierIds).Where(x => x.IsDeleted == false).ToList();

                double? ItemQuantity = 0;
                ItemMasterDAL objItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);

                if (objQLDtlDTO.Count > 0)
                {
                    foreach (QuickListDetailDTO qlItem in objQLDtlDTO)
                    {
                        ItemMasterDTO tempItemDTO = new ItemMasterDTO();
                        tempItemDTO = objItemDAL.GetItemWithoutJoins(null, qlItem.ItemGUID);
                        if (tempItemDTO.ItemType != 4)
                        {
                            tempItemDTO.DefaultPullQuantity = qlItem.Quantity;

                            ItemQuantity = qlItem.Quantity * QLQty;
                            if (tempItemDTO.IsEnforceDefaultReorderQuantity == true && tempItemDTO.DefaultReorderQuantity != null && tempItemDTO.DefaultReorderQuantity > 0)
                            {
                                while (ItemQuantity < tempItemDTO.DefaultReorderQuantity)
                                    ItemQuantity = ItemQuantity + 1;

                                while (ItemQuantity % tempItemDTO.DefaultReorderQuantity != 0)
                                    ItemQuantity = ItemQuantity + 1;
                            }
                            tempItemDTO.DefaultReorderQuantity = ItemQuantity;

                            DataFromDB.Add(tempItemDTO);
                        }
                    }
                }
            }
            else
            {
                string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
                TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
                DataFromDB = obj.GetPagedRecordsForModel(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, "", "newcart", "1", SessionHelper.UserSupplierIds, RoomDateFormat, CurrentTimeZone, "", true, true, IsAllowConsigeItem: UserConsignmentAllowed).ToList();
            }

            List<ItemMasterDTO> lstItemMaster = DataFromDB.ToList();
            lstItemMaster.ForEach(t =>
            {
                List<BinMasterDTO> lstItemLocation = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByItemLocationLevelQuanity(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, t.GUID.ToString());
                t.ItemsLocations = lstItemLocation;

            });
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = lstItemMaster
            },
                        JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDataForCartNew(QuickListJQueryDataTableParamModel param)
        {
            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            bool UserConsignmentAllowed = SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowOrderToConsignedItem);
            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"].ToString();
            bool IsArchived = false; //bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = false; //bool.Parse(Request["IsDeleted"].ToString());

            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "ItemNumber desc";
            }
            else
                sortColumnName = "ItemNumber desc";

            string searchQuery = string.Empty;

            Int64 QLID = 0;
            Int64.TryParse(Request["ParentID"], out QLID);

            int TotalRecordCount = 0;
            BinMasterDAL objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            List<ItemMasterDTO> DataFromDB = new List<ItemMasterDTO>();
            DataFromDB = obj.GetPagedItemsForCartNew(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID,
                             SessionHelper.CompanyID, IsArchived, IsDeleted, SessionHelper.UserSupplierIds, UserConsignmentAllowed,
                             Convert.ToString(Session["CurrentTimeZone"]), Convert.ToString(Session["RoomDateFormat"])).ToList();
            List<BinMasterDTO> objBinMasterListDTO = objBinDAL.GetBinMasterByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
            foreach (ItemMasterDTO dr in DataFromDB)
            {
                BinMasterDTO objBinMasterDTO = new BinMasterDTO();
                ItemMasterDTO objItemDTO = obj.GetRecordByItemGUID(dr.GUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                //  ItemMasterDTO objItemDTO = lstAllItems.FirstOrDefault(p => p.GUID == dr.ItemGUID.GetValueOrDefault(Guid.Empty));
                if (objItemDTO != null)
                {
                    List<BinMasterDTO> lstItemBins = objBinMasterListDTO.Where(p => p.ItemGUID == objItemDTO.GUID).OrderBy(x => x.BinNumber).ToList();
                    if (objItemDTO != null)
                    {
                        if (objItemDTO.OrderUOMValue == null || objItemDTO.OrderUOMValue <= 0)
                            objItemDTO.OrderUOMValue = 1;
                    }
                    if (dr.DefaultLocationName != null && lstItemBins != null)
                    {
                        objBinMasterDTO = lstItemBins.Where(x => x.BinNumber == dr.DefaultLocationName).FirstOrDefault();
                    }
                    if (objBinMasterDTO != null && objBinMasterDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                    {
                        double newOrderQty = 0;
                        int devideval = 0;
                        double tempQty = dr.DefaultReorderQuantity ?? 0;
                        double drq = objBinMasterDTO.DefaultReorderQuantity ?? 0;
                        if (tempQty > 0 && drq > 0)
                        {
                            if ((tempQty % drq) != 0)
                            {
                                devideval = Convert.ToInt32((Convert.ToInt32(tempQty) / Convert.ToInt32(drq))) + 1;
                                newOrderQty = drq * devideval;
                            }
                            else
                            {
                                //  devideval = Convert.ToInt32((Convert.ToInt32(tempQty) / Convert.ToInt32(drq)));
                                newOrderQty = tempQty;
                            }
                        }
                        dr.DefaultReorderQuantity = newOrderQty;
                        if (objItemDTO.IsAllowOrderCostuom)
                        {
                            dr.DefaultReorderQuantity = dr.DefaultReorderQuantity.GetValueOrDefault(0) / objItemDTO.OrderUOMValue.GetValueOrDefault(0);
                        }
                    }
                    else if (objItemDTO.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true)
                    {
                        double newOrderQty = 0;
                        int devideval = 0;
                        double tempQty = dr.DefaultReorderQuantity ?? 0;
                        double drq = objItemDTO.DefaultReorderQuantity ?? 0;
                        if (tempQty > 0 && drq > 0)
                        {
                            if ((tempQty % drq) != 0)
                            {
                                devideval = Convert.ToInt32((Convert.ToInt32(tempQty) / Convert.ToInt32(drq))) + 1;
                                newOrderQty = drq * devideval;
                            }
                            else
                            {
                                //  devideval = Convert.ToInt32((Convert.ToInt32(tempQty) / Convert.ToInt32(drq)));
                                newOrderQty = tempQty;
                            }
                        }
                        dr.DefaultReorderQuantity = newOrderQty;
                        if (objItemDTO.IsAllowOrderCostuom)
                        {
                            dr.DefaultReorderQuantity = dr.DefaultReorderQuantity.GetValueOrDefault(0) / objItemDTO.OrderUOMValue.GetValueOrDefault(0);
                        }
                    }
                }
            }
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
                    JsonRequestBehavior.AllowGet);
        }

        public ActionResult ItemMasterListAjax(QuickListJQueryDataTableParamModel param)
        {
            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"].ToString();
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            // set the default column sorting here, if first time then required to set 
            if (sortColumnName == "0" || sortColumnName == "undefined")
                sortColumnName = "ID";
            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";
            string searchQuery = string.Empty;
            int TotalRecordCount = 0;
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            List<ItemMasterDTO> lstItemMaster = obj.GetPagedRecordsNew(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.UserSupplierIds, CurrentTimeZone).ToList();
            lstItemMaster.ForEach(t =>
            {
                if (t.IsItemLevelMinMaxQtyRequired ?? false)
                {
                    List<ItemLocationDetailsDTO> lstItemLocation = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetItemsLocations(t.GUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    t.ItemLocations = lstItemLocation;
                }
                else
                {
                    List<ItemLocationDetailsDTO> lstItemLocation = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetItemsLocations(t.GUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    t.ItemLocations = lstItemLocation;
                }
            });
            Session["BinReplanish"] = null;
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = lstItemMaster
            },
                        JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ClearSession()
        {
            Session["AllCartItems"] = null;
            return Json(true);
        }

        [HttpPost]
        public JsonResult ReturnMessageOnActiononItems(int Action, string Ids)
        {
            string message = string.Empty;
            CartItemDAL objCartItemDAL = new CartItemDAL(SessionHelper.EnterPriseDBName);
            List<Guid> arrcartguids = new List<Guid>();
            List<CartItemDTO> lstCartItems = new List<CartItemDTO>();
            bool WithSelection;
            int OrderLineItemCount = 0;
            int TransferLineItemCount = 0;
            try
            {
                if (!string.IsNullOrWhiteSpace(Ids))
                {
                    string[] arrguids = Ids.Split(',');
                    foreach (string item in arrguids)
                    {
                        Guid objid = Guid.Empty;
                        if (Guid.TryParse(item, out objid))
                        {
                            arrcartguids.Add(objid);
                        }
                    }
                    WithSelection = true;
                }
                else
                {
                    WithSelection = false;
                }

                var tmpsupplierIds = new List<long>();
                lstCartItems = objCartItemDAL.GetCartItemsByGuids(arrcartguids, SessionHelper.RoomID, SessionHelper.CompanyID, WithSelection, tmpsupplierIds);

                if (lstCartItems != null && lstCartItems.Count > 0)
                {
                    switch (Action)
                    {
                        case 1:

                            OrderLineItemCount = (from ci in lstCartItems
                                                  where ci.ReplenishType == "Purchase"
                                                  group ci by new { ci.SupplierId, ci.BlanketPOID } into groupedci
                                                  select groupedci).Count();

                            TransferLineItemCount = (from ci in lstCartItems
                                                     where ci.ReplenishType == "Transfer"
                                                     group ci by new { ci.ItemGUID } into groupedci
                                                     select groupedci).Count();

                            if (OrderLineItemCount > 0)
                            {
                                message += string.Format(ResOrder.OrdersWillBeGenerated, OrderLineItemCount);
                            }
                            if (TransferLineItemCount > 0)
                            {
                                message += "1 Transfer will create with " + TransferLineItemCount + " Line Items.";
                            }
                            message += "Are you sure?";
                            break;
                        case 2:
                            OrderLineItemCount = (from ci in lstCartItems
                                                  where ci.ReplenishType == "Purchase" && ci.IsAutoMatedEntry == false
                                                  group ci by new { ci.SupplierId } into groupedci
                                                  select groupedci).Count();
                            TransferLineItemCount = (from ci in lstCartItems
                                                     where ci.ReplenishType == "Transfer" && ci.IsAutoMatedEntry == false
                                                     group ci by new { ci.ItemGUID } into groupedci
                                                     select groupedci).Count();

                            if (OrderLineItemCount > 0)
                            {
                                message += string.Format(ResOrder.OrdersWillBeGenerated, OrderLineItemCount);
                            }
                            if (TransferLineItemCount > 0)
                            {
                                message += "<br />1 Transfer will create with " + TransferLineItemCount + " Line Items.";
                            }
                            if (!string.IsNullOrWhiteSpace(message))
                            {
                                message += "<br /><br />Are you sure?";
                            }
                            else
                            {
                                message = "There is no any cart entry for this action applied";
                            }

                            break;
                        case 3:
                            TransferLineItemCount = (from ci in lstCartItems
                                                     where ci.ReplenishType == "Transfer"
                                                     group ci by new { ci.ItemGUID } into groupedci
                                                     select groupedci).Count();
                            if (TransferLineItemCount > 0)
                            {
                                message += "1 Transfer will create with " + TransferLineItemCount + " Line Items.";
                            }
                            else
                            {
                                message = "There is no any cart entry for this action applied";
                            }
                            break;
                        case 4:
                            OrderLineItemCount = (from ci in lstCartItems
                                                  where ci.ReplenishType == "Purchase"
                                                  group ci by new { ci.SupplierId, ci.BlanketPOID } into groupedci
                                                  select groupedci).Count();

                            if (OrderLineItemCount > 0)
                            {
                                message += string.Format(ResOrder.OrdersWillBeGenerated, OrderLineItemCount);
                            }
                            else
                            {
                                message = "There is no any cart entry for this action applied";
                            }
                            break;
                        default:
                            message = "There is no any cart entry for this action applied";
                            break;
                    }
                    return Json(new { Message = message, Status = "ok" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Message = "There is no any cart entry for this action applied", Status = "ok" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult OpenCreateOrderTransferPopup(int ActionType, string Ids, string OrderLineItemUDF1, string OrderLineItemUDF2, string OrderLineItemUDF3, string OrderLineItemUDF4, string OrderLineItemUDF5, string OrderItemQuantity)
        {
            bool isInsert = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Orders, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
            if (isInsert)
            {
                List<SelectListItem> returnList = new List<SelectListItem>();
                returnList.Add(new SelectListItem() { Text = ResOrder.GetOrderStatusText(OrderStatus.UnSubmitted.ToString()), Value = ((int)OrderStatus.UnSubmitted).ToString() });
                bool IsSubmit = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.OrderSubmit);
                bool IsApprove = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.OrderApproval);
                if (IsApprove)
                {
                    returnList.Add(new SelectListItem() { Text = ResOrder.GetOrderStatusText(OrderStatus.Submitted.ToString()), Value = ((int)OrderStatus.Submitted).ToString() });
                    returnList.Add(new SelectListItem() { Text = ResOrder.GetOrderStatusText(OrderStatus.Approved.ToString()), Value = ((int)OrderStatus.Approved).ToString() });
                    returnList.Add(new SelectListItem() { Text = ResOrder.GetOrderStatusText(OrderStatus.Transmitted.ToString()), Value = ((int)OrderStatus.Transmitted).ToString() });
                }
                else if (IsSubmit)
                {
                    returnList.Add(new SelectListItem() { Text = ResOrder.GetOrderStatusText(OrderStatus.Submitted.ToString()), Value = ((int)OrderStatus.Submitted).ToString() });
                }
                ViewBag.OrderStatusList = returnList;



                CartItemDAL objCartItemDAL = new CartItemDAL(SessionHelper.EnterPriseDBName);
                var tmpsupplierIds = new List<long>();
                IList<OrderMasterDTO> lstOrders = objCartItemDAL.GetOrdersByCartIds(Ids, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriseDBName, tmpsupplierIds, OrderLineItemUDF1, OrderLineItemUDF2, OrderLineItemUDF3, OrderLineItemUDF4, OrderLineItemUDF5, SessionHelper.EnterPriceID, OrderItemQuantity);

                if (lstOrders != null && lstOrders.Count > 0)
                {
                    lstOrders.ToList().ForEach(t =>
                    {
                        if (IsApprove)
                        {
                            t.OrderStatus = (int)OrderStatus.Approved;
                        }
                        else if (IsSubmit)
                        {
                            t.OrderStatus = (int)OrderStatus.Submitted;
                        }
                        else
                        {

                            t.OrderStatus = (int)OrderStatus.UnSubmitted;
                        }
                        SupplierAccountDetailsDAL objSupplierAccountDetailsDAL = new SupplierAccountDetailsDAL(SessionHelper.EnterPriseDBName);
                        System.Collections.Generic.List<SupplierAccountDetailsDTO> objSupplierAccount = objSupplierAccountDetailsDAL.GetAllAccountsBySupplierID(Convert.ToInt64(t.Supplier), SessionHelper.RoomID, SessionHelper.CompanyID).Where(s => s.IsDefault == true).ToList();
                        if (objSupplierAccount != null && objSupplierAccount.Count() > 0)
                        {
                            ViewBag.SupplierAccount = objSupplierAccount;
                            t.SupplierAccountGuid = objSupplierAccount.Where(s => s.IsDefault == true).FirstOrDefault().GUID;
                        }
                    });

                }
                return PartialView("OrdersFromCart", lstOrders);
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult OpenCreateTransferPopup(int ActionType, string Ids, string TransferItemQuantity)
        {
            bool isInsert = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Transfer, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
            if (isInsert)
            {
                List<SelectListItem> returnList = new List<SelectListItem>();
                returnList.Add(new SelectListItem() { Text = ResTransfer.GetTransferStatusText(TransferStatus.UnSubmitted.ToString()), Value = ((int)TransferStatus.UnSubmitted).ToString() });
                bool IsSubmit = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.TransferSubmit);

                if (IsSubmit)
                {
                    returnList.Add(new SelectListItem() { Text = ResTransfer.GetTransferStatusText(TransferStatus.Submitted.ToString()), Value = ((int)OrderStatus.Submitted).ToString() });
                }

                ViewBag.TrasnferStatusList = returnList;
                RoomDAL roomCtrl = new RoomDAL(SessionHelper.EnterPriseDBName);
                List<RoomDTO> roomDTOList = roomCtrl.GetRoomByCompanyIDPlain(SessionHelper.CompanyID).Where(x => x.ID != SessionHelper.RoomID).OrderBy(x => x.RoomName).ToList();
                roomDTOList.Insert(0, new RoomDTO { ID = 0, RoomName = string.Empty });
                ViewBag.ReplenishingRoom = roomDTOList;
                //  if (roomDTOList.Count > 1)
                {
                    CartItemDAL objCartItemDAL = new CartItemDAL(SessionHelper.EnterPriseDBName);
                    IList<TransferMasterDTO> lstTransfers = objCartItemDAL.GetTransfersByCartIdsNew(Ids, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriseDBName, 1, SessionHelper.EnterPriceID, TransferItemQuantity);

                    if (lstTransfers != null && lstTransfers.Count > 0)
                    {
                        lstTransfers.ToList().ForEach(t =>
                        {
                            if (IsSubmit)
                            {
                                t.TransferStatus = (int)OrderStatus.Submitted;
                            }
                            else
                            {

                                t.TransferStatus = (int)OrderStatus.UnSubmitted;
                            }

                        });
                        return PartialView("TransfersFromCart", lstTransfers);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public JsonResult CreateOrdersfromCart(List<OrderMasterDTO> lstOrders)
        {
            try
            {
                #region check dynamic validation rules
                List<OrderMasterDTO> inValidOrders = new List<OrderMasterDTO>();
                if (lstOrders != null && lstOrders.Count > 0)
                {
                    string validationMsg = "";
                    int cnt = 0;
                    foreach (OrderMasterDTO objOrderMasterDTO in lstOrders)
                    {
                        if (!string.IsNullOrWhiteSpace(objOrderMasterDTO.RequiredDateString))
                        {
                            objOrderMasterDTO.RequiredDate = DateTime.ParseExact(objOrderMasterDTO.RequiredDateString, Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult);
                        }

                        var validationResultList = DTOCommonUtils.ValidateDTO<OrderMasterDTO>(objOrderMasterDTO, ControllerContext, new List<string>() { "Supplier", "PackSlipNumber" });
                        if (validationResultList.HasErrors())
                        {

                            if (!string.IsNullOrWhiteSpace(validationMsg))
                            {
                                validationMsg += "<br/><br/>";
                            }

                            string msg = validationResultList.GetShortErrorMessage(typeof(RequiredAttributeAdapter));
                            validationMsg += (string.Format(ResQuoteMaster.ValidationFailedFor, objOrderMasterDTO.SupplierName) + "<br/>" + msg); //string.Format("Validation failed for {0}.<br/>{1}", objOrderMasterDTO.SupplierName, msg);
                            cnt++;
                        }

                        if (cnt >= 5)
                        {
                            // display message for 5 grid rows to shorten message
                            break;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(validationMsg))
                    {
                        return Json(new { Message = validationMsg, Status = "fail", lstOrders = lstOrders });
                    }
                }
                #endregion

                CartItemDAL objCartItemDAL = new CartItemDAL(SessionHelper.EnterPriseDBName);
                OrderMasterDAL objOrderMasterDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                bool IsSubmit = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.OrderSubmit);
                bool IsApprove = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.OrderApproval);
                OrderController objOrdContlr = new OrderController();

                int? PriseSelectionOption = 0;
                eTurns.DAL.RoomDAL onjRoomDAL = new eTurns.DAL.RoomDAL(SessionHelper.EnterPriseDBName);
                RoomModuleSettingsDTO objRoomModuleSettingsDTO = onjRoomDAL.GetRoomModuleSettings(eTurnsWeb.Helper.SessionHelper.CompanyID, eTurnsWeb.Helper.SessionHelper.RoomID, (long)eTurnsWeb.Helper.SessionHelper.ModuleList.Orders);
                if (objRoomModuleSettingsDTO != null)
                    PriseSelectionOption = objRoomModuleSettingsDTO.PriseSelectionOption;

                if (PriseSelectionOption != 1 && PriseSelectionOption != 2)
                    PriseSelectionOption = 1;

                if (lstOrders != null && lstOrders.Count > 0)
                {

                    //---------------------- Check For Order Number Duplication ----------------------
                    //
                    //RoomDTO roomDTO = new eTurns.DAL.RoomDAL(SessionHelper.EnterPriseDBName).GetRoomByIDPlain(eTurnsWeb.Helper.SessionHelper.RoomID);
                    CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                    string columnList = "ID,RoomName,IsAllowOrderDuplicate";
                    RoomDTO roomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");

                    if (roomDTO.IsAllowOrderDuplicate != true)
                    {
                        var OrdrGroup = (from O in lstOrders
                                         group O by new { OrderNumberGB = O.OrderNumber.Trim().ToLower() } into OGB
                                         select new
                                         {
                                             OrderNumber = OGB.Key.OrderNumberGB,
                                             TotalCount = OGB.Count()
                                         }).Where(x => x.TotalCount > 1).ToList();

                        if (OrdrGroup.Count > 0)
                        {
                            return Json(new { Message = string.Format(ResOrder.OrderNumberDuplicateInList, OrdrGroup[0].OrderNumber), Status = "fail", lstOrders = lstOrders });
                        }
                        var orderNoExistMsg = ResOrder.OrderNumberAlreadyExist;
                        foreach (OrderMasterDTO objOrderMasterDTO in lstOrders)
                        {
                            if (objOrderMasterDAL.IsOrderNumberDuplicateById(objOrderMasterDTO.OrderNumber, objOrderMasterDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID))
                            {
                                return Json(new { Message = string.Format(orderNoExistMsg, objOrderMasterDTO.OrderNumber), Status = "fail", lstOrders = lstOrders });
                            }
                        }
                    }
                    DollerApprovalLimitDTO objDollarLimt = null;
                    eTurns.DAL.UserMasterDAL userDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                    ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                    ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                    string approvalErrorMsg = string.Empty;
                    string OrdapprovalSuppErrorMsg = string.Empty;
                    CommonDAL commonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

                    double AllOrderCost = 0;
                    double AllOrderPrice = 0;
                    foreach (OrderMasterDTO objOrderMasterDTO in lstOrders)
                    {
                        objOrderMasterDTO.RequiredDate = DateTime.ParseExact(objOrderMasterDTO.RequiredDateString, Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult);
                        if (IsApprove)
                        {
                            double? UserApprovalLimit = null;
                            double UserUsedLimit = 0;
                            double OrderCost = 0;
                            double OrderPrice = 0;
                            objDollarLimt = null;

                            double? ItemApprovedQuantity = null;

                            objDollarLimt = userDAL.GetOrderLimitByUserId(SessionHelper.UserID, SessionHelper.CompanyID, SessionHelper.RoomID);
                            if (objDollarLimt != null)
                            {
                                UserApprovalLimit = objDollarLimt.DollerLimit > 0 ? objDollarLimt.DollerLimit : null;
                                UserUsedLimit = objDollarLimt.UsedLimit;
                                ItemApprovedQuantity = objDollarLimt.ItemApprovedQuantity > 0 ? objDollarLimt.ItemApprovedQuantity : null;
                            }

                            if (objOrderMasterDTO.OrderStatus == (int)OrderStatus.Submitted)
                            {
                                if (objOrderMasterDTO.Supplier != null && objOrderMasterDTO.Supplier.GetValueOrDefault(0) > 0)
                                {
                                    OrderController ordCntrl = new OrderController();
                                    bool IsSuppApprove = ordCntrl.CheckSupplierAproveRight(objOrderMasterDTO.Supplier.GetValueOrDefault(0));
                                    if (IsSuppApprove)
                                        objOrderMasterDTO.OrderStatus = (int)OrderStatus.Approved;
                                }
                            }

                            if (objOrderMasterDTO.OrderStatus == (int)OrderStatus.Approved || objOrderMasterDTO.OrderStatus == (int)OrderStatus.Transmitted)
                            {
                                List<Guid> lstids = new List<Guid>();
                                if (!string.IsNullOrWhiteSpace(objOrderMasterDTO.OrderLineItemsIds))
                                {
                                    foreach (var item in objOrderMasterDTO.OrderLineItemsIds.Split(','))
                                    {
                                        Guid tempid = Guid.Empty;
                                        if (Guid.TryParse(item, out tempid))
                                            lstids.Add(tempid);
                                    }
                                    var tmpsupplierIds = new List<long>();
                                    List<CartItemDTO> lstCartItems = objCartItemDAL.GetCartItemsByGuids(lstids, SessionHelper.RoomID, SessionHelper.CompanyID, true, tmpsupplierIds);

                                    List<string> ASINs = new List<string>();
                                    Dictionary<List<ItemMasterDTO>, string> lstNonOrderableItems = new Dictionary<List<ItemMasterDTO>, string>();
                                    if (lstCartItems != null && lstCartItems.Count > 0)
                                    {
                                        #region WI-7318	AB Integration | Sync Item cost when an item Added to Order line item.
                                        if (SessionHelper.AllowABIntegration)
                                        {
                                            foreach (var ABCartitem in lstCartItems)
                                            {
                                                ItemMasterDTO objABItemDTO = new ItemMasterDTO();
                                                objABItemDTO = objItemMasterDAL.GetItemWithMasterTableJoins(null, ABCartitem.ItemGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);

                                                if (objABItemDTO != null && !string.IsNullOrWhiteSpace(objABItemDTO.SupplierPartNo))
                                                {
                                                    ProductDetailsDAL objProductDetailsDAL = new ProductDetailsDAL(SessionHelper.EnterPriseDBName);
                                                    Int64 ABItemMappingID = objProductDetailsDAL.CheckItemAddedFromAB(objABItemDTO.SupplierPartNo, objABItemDTO.GUID, SessionHelper.CompanyID, SessionHelper.RoomID);
                                                    if (ABItemMappingID > 0)
                                                    {
                                                        ASINs.Add(objABItemDTO.SupplierPartNo);
                                                    }
                                                }
                                            }
                                            if (ASINs != null && ASINs.Count > 0)
                                            {
                                                lstNonOrderableItems = eTurns.ABAPIBAL.Helper.ABAPIHelper.ItemSyncToRoom(ASINs, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.EnterPriseDBName);
                                            }
                                        }
                                        #endregion
                                    }
                                    foreach (CartItemDTO cartitem in lstCartItems)
                                    {
                                        objItemMasterDTO = objItemMasterDAL.GetItemByGuidPlain((cartitem.ItemGUID ?? Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);

                                        if (SessionHelper.AllowABIntegration
                                            && ASINs != null && ASINs.Count > 0)
                                        {
                                            if (lstNonOrderableItems != null && lstNonOrderableItems.Count > 0
                                                && lstNonOrderableItems.Values.Contains("success")
                                                && lstNonOrderableItems.Keys.Count > 0
                                                && lstNonOrderableItems.Keys.SelectMany(c => c).ToList().Count > 0)
                                            {
                                                List<ItemMasterDTO> lstReturnsItems = new List<ItemMasterDTO>();
                                                lstReturnsItems = lstNonOrderableItems.Keys.SelectMany(c => c).ToList();

                                                if (lstReturnsItems.Where(x => x.GUID == objItemMasterDTO.GUID && x.ItemNumber == objItemMasterDTO.ItemNumber).Count() > 0)
                                                {
                                                    return Json(new { Message = string.Format(ResOrder.ItemnotOrderable, objItemMasterDTO.ItemNumber, ItemApprovedQuantity), Status = "fail" }, JsonRequestBehavior.AllowGet);
                                                }
                                            }
                                        }

                                        if (cartitem != null && cartitem.Quantity != null
                                                && cartitem.Quantity > 0)
                                        {
                                            if (objDollarLimt != null && objDollarLimt.OrderLimitType == OrderLimitType.PerOrder && IsApprove && objOrderMasterDTO.OrderStatus == (int)OrderStatus.Approved
                                               && ItemApprovedQuantity > 0 && cartitem.Quantity > (ItemApprovedQuantity))
                                            {
                                                return Json(new { Message = string.Format(ResOrder.CantApproveMoreThanPerOrderItemQtyApprovalLimit, cartitem.Quantity, ItemApprovedQuantity), Status = "fail" }, JsonRequestBehavior.AllowGet);
                                            }
                                        }

                                        CostUOMMasterDTO costUOM = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName).GetCostUOMByID(objItemMasterDTO.CostUOMID.GetValueOrDefault(0));
                                        if (costUOM == null)
                                            costUOM = new CostUOMMasterDTO() { CostUOMValue = 1 };

                                        if (objItemMasterDTO != null && cartitem.Quantity.GetValueOrDefault(0) > 0)
                                        {
                                            OrderCost += (objItemMasterDTO.Cost.GetValueOrDefault(0) * cartitem.Quantity.GetValueOrDefault(0))
                                                     / (costUOM.CostUOMValue.GetValueOrDefault(0) > 0 ? costUOM.CostUOMValue.GetValueOrDefault(1) : 1);

                                            OrderPrice += (objItemMasterDTO.SellPrice.GetValueOrDefault(0) * cartitem.Quantity.GetValueOrDefault(0))
                                                   / (costUOM.CostUOMValue.GetValueOrDefault(0) > 0 ? costUOM.CostUOMValue.GetValueOrDefault(1) : 1);

                                            AllOrderCost += OrderCost;
                                            AllOrderPrice += OrderPrice;

                                            objOrderMasterDTO.OrderPrice += OrderPrice;
                                            objOrderMasterDTO.OrderCost += OrderCost;

                                        }

                                        if (SessionHelper.RoleID > 0 && objDollarLimt != null)
                                        {
                                            if (PriseSelectionOption == 1)
                                            {
                                                if (objDollarLimt.OrderLimitType == OrderLimitType.All && (OrderPrice > (UserApprovalLimit - UserUsedLimit) || AllOrderPrice > (UserApprovalLimit - UserUsedLimit)))
                                                    approvalErrorMsg += "<br/>" + string.Format(ResOrder.SupplierOrderCantApproveMTRemainingOrderApprovalLimit, objOrderMasterDTO.SupplierName, objOrderMasterDTO.OrderNumber, OrderPrice, (UserApprovalLimit - (UserUsedLimit + (AllOrderPrice - OrderPrice)))) + "<br/>";
                                                else if (objDollarLimt.OrderLimitType == OrderLimitType.PerOrder && OrderPrice > (UserApprovalLimit))
                                                    approvalErrorMsg += "<br/>" + string.Format(ResOrder.SupplierOrderCantApproveMTPerOrderApprovalLimit, objOrderMasterDTO.SupplierName, objOrderMasterDTO.OrderNumber, OrderPrice, UserApprovalLimit) + "<br/>";
                                            }
                                            else
                                            {
                                                if (objDollarLimt.OrderLimitType == OrderLimitType.All && (OrderCost > (UserApprovalLimit - UserUsedLimit) || AllOrderCost > (UserApprovalLimit - UserUsedLimit)))
                                                    approvalErrorMsg += "<br/>" + string.Format(ResOrder.SupplierOrderCantApproveMTRemainingOrderApprovalLimit, objOrderMasterDTO.SupplierName, objOrderMasterDTO.OrderNumber, OrderCost, (UserApprovalLimit - (UserUsedLimit + (AllOrderCost - OrderCost)))) + "<br/>";
                                                else if (objDollarLimt.OrderLimitType == OrderLimitType.PerOrder && OrderCost > (UserApprovalLimit))
                                                    approvalErrorMsg += "<br/>" + string.Format(ResOrder.SupplierOrderCantApproveMTPerOrderApprovalLimit, objOrderMasterDTO.SupplierName, objOrderMasterDTO.OrderNumber, OrderCost, UserApprovalLimit) + "<br/>";
                                            }
                                        }

                                    }

                                }
                                //
                                if (objOrderMasterDTO.Supplier != null && objOrderMasterDTO.Supplier.GetValueOrDefault(0) > 0)
                                {
                                    UserAccessDTO objUserAccess = null;
                                    if (SessionHelper.UserType == 1)
                                    {
                                        eTurnsMaster.DAL.UserMasterDAL objUserdal = new eTurnsMaster.DAL.UserMasterDAL();
                                        objUserAccess = objUserdal.GetUserRoomAccessesByUserId(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);
                                    }
                                    else
                                    {
                                        eTurns.DAL.UserMasterDAL objUserdal = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                                        objUserAccess = objUserdal.GetUserRoomAccessesByUserId(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);
                                    }

                                    if (objUserAccess != null && !string.IsNullOrWhiteSpace(objUserAccess.SupplierIDs))
                                    {
                                        List<string> strSupplier = objUserAccess.SupplierIDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                        if (strSupplier != null && strSupplier.Count > 0)
                                        {
                                            if (strSupplier.Contains(objOrderMasterDTO.Supplier.ToString()) == false)
                                            {
                                                if (string.IsNullOrEmpty(OrdapprovalSuppErrorMsg))
                                                    OrdapprovalSuppErrorMsg = ResOrder.OrderNo + " " + objOrderMasterDTO.OrderNumber + ", " + ResOrder.SupplierApprove;
                                                else
                                                    OrdapprovalSuppErrorMsg += "<br/>" + ResOrder.OrderNo + " " + objOrderMasterDTO.OrderNumber + ", " + ResOrder.SupplierApprove;
                                            }
                                        }
                                    }
                                }
                                //
                            }

                            #region Comment
                            //if (SessionHelper.RoleID > 0 && objDollarLimt != null)
                            //{
                            //    if (PriseSelectionOption == 1)
                            //    {
                            //        if (objDollarLimt.OrderLimitType == OrderLimitType.All && OrderPrice > (UserApprovalLimit - UserUsedLimit))
                            //            approvalErrorMsg += "<br/>" + "'" + objOrderMasterDTO.SupplierName + "' Supplier's Order# : '" + objOrderMasterDTO.OrderNumber + "' Can not approve ($" + OrderPrice + ") more than remaining order approval limit($" + (UserApprovalLimit - UserUsedLimit) + ").<br/>";
                            //        else if (objDollarLimt.OrderLimitType == OrderLimitType.PerOrder && OrderPrice > (UserApprovalLimit))
                            //            approvalErrorMsg += "<br/>" + "'" + objOrderMasterDTO.SupplierName + "' Supplier's Order# : '" + objOrderMasterDTO.OrderNumber + "' Can not approve ($" + OrderPrice + ") more than per order approval limit($" + (UserApprovalLimit) + ").<br/>";
                            //    }
                            //    else
                            //    {
                            //        if (objDollarLimt.OrderLimitType == OrderLimitType.All && OrderCost > (UserApprovalLimit - UserUsedLimit))
                            //            approvalErrorMsg += "<br/>" + "'" + objOrderMasterDTO.SupplierName + "' Supplier's Order# : '" + objOrderMasterDTO.OrderNumber + "' Can not approve ($" + OrderCost + ") more than remaining order approval limit($" + (UserApprovalLimit - UserUsedLimit) + ").<br/>";
                            //        else if (objDollarLimt.OrderLimitType == OrderLimitType.PerOrder && OrderCost > (UserApprovalLimit))
                            //            approvalErrorMsg += "<br/>" + "'" + objOrderMasterDTO.SupplierName + "' Supplier's Order# : '" + objOrderMasterDTO.OrderNumber + "' Can not approve ($" + OrderCost + ") more than per order approval limit($" + (UserApprovalLimit) + ").<br/>";
                            //    }
                            //} 
                            #endregion
                        }
                        //WI-8417
                        if (objOrderMasterDTO.OrderStatus == (int)OrderStatus.Approved && objOrderMasterDTO.OrderStatus != (int)OrderStatus.Closed)
                        {
                            List<Guid> lstids = new List<Guid>();
                            List<CartItemDTO> lstCartItems = null;
                            
                            if (!string.IsNullOrWhiteSpace(objOrderMasterDTO.OrderLineItemsIds))
                            {
                                foreach (var item in objOrderMasterDTO.OrderLineItemsIds.Split(','))
                                {
                                    Guid tempid = Guid.Empty;
                                    if (Guid.TryParse(item, out tempid))
                                        lstids.Add(tempid);
                                }
                                var tmpsupplierIds = new List<long>();
                                lstCartItems = objCartItemDAL.GetCartItemsByGuids(lstids, SessionHelper.RoomID, SessionHelper.CompanyID, true, tmpsupplierIds);
                            }

                            if (lstCartItems != null && lstCartItems.Count > 0)
                            {
                                string validationMsgForOrder = string.Empty;
                                string[] AllGuid = objOrderMasterDTO.OrderLineItemsIds.Split(',').ToArray();
                                string[] cartItemQuantity;

                                if (objOrderMasterDTO.CartQuantityString != null && (!string.IsNullOrWhiteSpace(objOrderMasterDTO.CartQuantityString)))
                                {
                                    cartItemQuantity = objOrderMasterDTO.CartQuantityString.Split(',').ToArray();
                                }
                                else
                                {
                                    cartItemQuantity = null;
                                }
                                List<OrderDetailsDTO> lstOfItems = new List<OrderDetailsDTO>();

                                foreach (CartItemDTO cartitem in lstCartItems)
                                {
                                    objItemMasterDTO = objItemMasterDAL.GetItemByGuidPlain((cartitem.ItemGUID ?? Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                                    CostUOMMasterDTO costUOM = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName).GetCostUOMByID(objItemMasterDTO.CostUOMID.GetValueOrDefault(0));

                                    int Index = Array.FindIndex(AllGuid, row => row.Contains(cartitem.GUID.ToString()));
                                    double? QuanityToSet = cartitem.Quantity;
                                    if (cartItemQuantity != null && cartItemQuantity.Length > 0)
                                    {
                                        try
                                        {
                                            QuanityToSet = Convert.ToDouble(cartItemQuantity[Index]);
                                        }
                                        catch (Exception) { }
                                    }
                                    lstOfItems.Add(new OrderDetailsDTO
                                    {
                                        ApprovedQuantity = QuanityToSet,
                                        ItemGUID = cartitem.ItemGUID,
                                        OrderLineItemExtendedCost = (QuanityToSet.GetValueOrDefault(0) * (objItemMasterDTO.Cost.GetValueOrDefault(0) / costUOM.CostUOMValue.GetValueOrDefault(1)))
                                    });
                                }

                                var isValidOrderLineItems = commonDAL.ValidateOrderItemOnSupplierBlanketPO(objOrderMasterDTO.GUID, lstOfItems, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID, SessionHelper.UserID, out validationMsgForOrder);
                                if (!isValidOrderLineItems)
                                {
                                    objOrderMasterDTO.IsValid = false;
                                    objOrderMasterDTO.Status = "fail";
                                    objOrderMasterDTO.Message = validationMsgForOrder;
                                    inValidOrders.Add(objOrderMasterDTO);
                                    //return Json(new { Message = validationMsgForPO, Status = "fail", ID = objOrderMasterDTO.ID }, JsonRequestBehavior.AllowGet);
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(approvalErrorMsg))
                    {
                        return Json(new { Message = approvalErrorMsg, Status = "fail" }, JsonRequestBehavior.AllowGet);
                    }
                    if (!string.IsNullOrEmpty(OrdapprovalSuppErrorMsg))
                    {
                        return Json(new { Message = OrdapprovalSuppErrorMsg, Status = "fail" }, JsonRequestBehavior.AllowGet);
                    }
                }

                Dictionary<string, string> rejectedOrderLineItems = new Dictionary<string, string>();
                var validateCartItem = ValidateOrderLineItemsForMaxOrderQty(lstOrders, out rejectedOrderLineItems);
                Dictionary<string, string> rejectedOrderLineItemstest = new Dictionary<string, string>();
                long SessionUserId = SessionHelper.UserID;
                lstOrders = objCartItemDAL.CreateOrdersByCart(lstOrders, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriseDBName, 1, SessionHelper.EnterPriceID, out rejectedOrderLineItemstest, SessionUserId, "");

                if (lstOrders != null && lstOrders.Count > 0)
                {
                    SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                    eTurns.DAL.UserMasterDAL objUserDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                    OrderMasterDAL orderDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);

                    foreach (OrderMasterDTO Orditem in lstOrders.Where(x => x.IsValid))
                    {
                        if (Orditem.OrderStatus == (int)OrderStatus.UnSubmitted)
                        {
                            SupplierMasterDTO objSupplier = objSupplierMasterDAL.GetSupplierByIDPlain(Orditem.Supplier ?? 0);
                            objOrdContlr.SendMailOrderUnSubmitted(objSupplier, Orditem);
                        }

                        if (Orditem.OrderStatus == (int)OrderStatus.Submitted)
                        {
                            objOrdContlr.SendMailToApprovalAuthority(Orditem);
                        }

                        if (Orditem.OrderStatus == (int)OrderStatus.Approved || Orditem.OrderStatus == (int)OrderStatus.Transmitted)
                        {
                            SupplierMasterDTO objSupplier = objSupplierMasterDAL.GetSupplierByIDPlain(Orditem.Supplier ?? 0);
                            objOrdContlr.SendMailToSupplier(objSupplier, Orditem);
                            eTurns.DAL.UserMasterDAL userMasterDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);

                            eTurnsMaster.DAL.UserMasterDAL objReqRequesterUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL();
                            UserMasterDTO ReqUser = new UserMasterDTO();

                            string OrdRequesterEmailAddress = "";
                            string OrdApproverEmailAddress = "";
                            if (Orditem.RequesterID.GetValueOrDefault(0) > 0)
                            {
                                ReqUser = objReqRequesterUserMasterDAL.GetUserByIdPlain(Orditem.RequesterID.GetValueOrDefault(0));
                                if (ReqUser == null)
                                {
                                    ReqUser = userMasterDAL.GetUserByIdPlain(Orditem.RequesterID.GetValueOrDefault(0));
                                }
                                if (ReqUser != null && !string.IsNullOrWhiteSpace(ReqUser.Email))
                                {
                                    OrdRequesterEmailAddress = ReqUser.Email;
                                }
                            }
                            if (Orditem.ApproverID.GetValueOrDefault(0) > 0)
                            {
                                ReqUser = objReqRequesterUserMasterDAL.GetUserByIdPlain(Orditem.ApproverID.GetValueOrDefault(0));
                                if (ReqUser == null)
                                {
                                    ReqUser = userMasterDAL.GetUserByIdPlain(Orditem.ApproverID.GetValueOrDefault(0));
                                }
                                if (ReqUser != null && !string.IsNullOrWhiteSpace(ReqUser.Email))
                                {
                                    OrdApproverEmailAddress = ReqUser.Email;
                                }
                            }
                            objOrdContlr.SendMailForApprovedOrReject(Orditem, "approved", OrdRequesterEmailAddress, OrdApproverEmailAddress);
                            double? UserApprovalLimit = null;
                            double UserUsedLimit = 0;
                            double OrderCost = 0;
                            double OrderPrice = 0;
                            DollerApprovalLimitDTO objDollarLimt = null;
                            eTurns.DAL.UserMasterDAL userDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                            objDollarLimt = userDAL.GetOrderLimitByUserId(SessionHelper.UserID, Orditem.CompanyID ?? 0, Orditem.Room ?? 0);

                            if (SessionHelper.RoleID > 0 && objDollarLimt != null && objDollarLimt.OrderLimitType == OrderLimitType.All)
                            {
                                UserApprovalLimit = objDollarLimt.DollerLimit;
                                UserUsedLimit = objDollarLimt.UsedLimit;

                                if (PriseSelectionOption == 1)
                                {
                                    OrderPrice = Orditem.OrderPrice;

                                    if (OrderPrice <= (UserApprovalLimit - UserUsedLimit))
                                    {
                                        userDAL.SaveDollerUsedLimt(OrderPrice, SessionHelper.UserID, Orditem.CompanyID ?? 0, Orditem.Room ?? 0);
                                    }
                                }
                                else
                                {
                                    OrderCost = Orditem.OrderCost.GetValueOrDefault(0);

                                    if (OrderCost <= (UserApprovalLimit - UserUsedLimit))
                                    {
                                        userDAL.SaveDollerUsedLimt(OrderCost, SessionHelper.UserID, Orditem.CompanyID ?? 0, Orditem.Room ?? 0);
                                    }
                                }
                            }
                        }
                    }
                }
                string message = string.Empty;
                string status = string.Empty;

                if (inValidOrders != null && inValidOrders.Count > 0)
                {
                    foreach (var Order in inValidOrders.Where(x => x.IsValid == false))
                    {
                        return Json(new { Message = Order.Message, Status = Order.Status, ID = Order.ID }, JsonRequestBehavior.AllowGet);
                    }
                }
                if (!rejectedOrderLineItems.Any())
                {
                    message = ResMessage.SaveMessage;
                    status = "ok";
                }
                else
                {
                    message = ResQuoteMaster.BelowGivenItemsRejected;
                    var itemNamberResource = ResItemMaster.ItemNumber;
                    var reasonResource = ResCommon.Reason;
                    foreach (KeyValuePair<string, string> entry in rejectedOrderLineItems)
                    {
                        message += (itemNamberResource + ": " + entry.Key + " ," + reasonResource + ": " + entry.Value);
                    }

                    status = "fail";
                }
                
                GlobalHost.ConnectionManager.GetHubContext<eTurnsHub>().Clients.Group(SessionHelper.EnterPriceID + "_" + SessionHelper.CompanyID + "_" + SessionHelper.RoomID).UpdateRedCircleCountInClients();
                return Json(new { Message = message, Status = status, lstOrders = lstOrders });
            }
            catch (Exception ex)
            {
                string message = ResMessage.SaveErrorMsg;
                string status = "fail";
                return Json(new { Message = message, Status = status, lstOrders = lstOrders });
            }
        }

        private bool ValidateOrderLineItemsForMaxOrderQty(List<OrderMasterDTO> lstOrders, out Dictionary<string, string> rejectedOrderLineItems)
        {
            CartItemDAL objCartItemDAL = new CartItemDAL(SessionHelper.EnterPriseDBName);
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            OrderMasterDTO objOrder = new OrderMasterDTO();
            OrderDetailsDAL objDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
            BinMasterDAL objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            Dictionary<Guid, double> itemsOrderQtyForItemMinMax = new Dictionary<Guid, double>();
            Dictionary<string, double> itemsOrderQtyForBinMinMax = new Dictionary<string, double>();
            rejectedOrderLineItems = new Dictionary<string, string>();
            RoomDAL objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
            //RoomDTO objRoomDTO = objRoomDAL.GetRoomByIDPlain(SessionHelper.RoomID);
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            string columnList = "ID,RoomName,PreventMaxOrderQty";
            RoomDTO objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");


            List<int> ordertoBeDelete = new List<int>();

            if (objRoomDTO.PreventMaxOrderQty != (int)PreventMaxOrderQty.OnOrder)
            {
                return true;
            }
            for (int orderCount = 0; orderCount < lstOrders.Count; orderCount++)
            {
                var order = lstOrders[orderCount];
                if (order.IsValid)
                {
                    List<Guid> lstids = new List<Guid>();

                    if (!string.IsNullOrWhiteSpace(order.OrderLineItemsIds))
                    {
                        foreach (var item in order.OrderLineItemsIds.Split(','))
                        {
                            Guid tempid = Guid.Empty;
                            if (Guid.TryParse(item, out tempid))
                            {
                                lstids.Add(tempid);
                            }
                        }
                    }

                    if (lstids.Count > 0)
                    {
                        var tmpsupplierIds = new List<long>();
                        List<CartItemDTO> lstCartItems = objCartItemDAL.GetCartItemsByGuids(lstids, SessionHelper.RoomID, SessionHelper.CompanyID, true, tmpsupplierIds);
                        int rejectedDueToPreventMaxValidation = 0;
                        List<string> rejectedOrderLineItemsGuids = new List<string>();

                        foreach (CartItemDTO cartitem in lstCartItems)
                        {
                            ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                            objItemMasterDTO = objItemMasterDAL.GetItemByGuidPlain((cartitem.ItemGUID ?? Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);

                            if (objItemMasterDTO.IsItemLevelMinMaxQtyRequired.HasValue && objItemMasterDTO.IsItemLevelMinMaxQtyRequired.Value)
                            {
                                var tmpItemOnOrderQty = objItemMasterDTO.OnOrderQuantity.GetValueOrDefault(0);
                                double itemOrderQtySoFar = 0;

                                if (itemsOrderQtyForItemMinMax.ContainsKey(objItemMasterDTO.GUID))
                                {
                                    itemOrderQtySoFar += itemsOrderQtyForItemMinMax[objItemMasterDTO.GUID];
                                }

                                if (objItemMasterDTO.MaximumQuantity.HasValue && objItemMasterDTO.MaximumQuantity.Value > 0 && (tmpItemOnOrderQty + itemOrderQtySoFar + cartitem.Quantity.GetValueOrDefault(0)) > objItemMasterDTO.MaximumQuantity.Value)
                                {
                                    if (!(rejectedOrderLineItems.ContainsKey(objItemMasterDTO.ItemNumber)))
                                    {
                                        rejectedOrderLineItems[objItemMasterDTO.ItemNumber] = ResOrder.NotAddedItemMaxQtyReached;
                                    }
                                    rejectedOrderLineItemsGuids.Add(Convert.ToString(cartitem.GUID));
                                    rejectedDueToPreventMaxValidation++;
                                    continue;
                                }
                                else
                                {
                                    itemsOrderQtyForItemMinMax[objItemMasterDTO.GUID] = (
                                            itemsOrderQtyForItemMinMax.ContainsKey(objItemMasterDTO.GUID)
                                            ? (itemsOrderQtyForItemMinMax[objItemMasterDTO.GUID] + (cartitem.Quantity.GetValueOrDefault(0)))
                                            : cartitem.Quantity.GetValueOrDefault(0)
                                            );
                                }
                            }
                            else
                            {
                                List<BinMasterDTO> lstItemBins = objBinDAL.GetItemLocations(objItemMasterDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(x => x.BinNumber).ToList();
                                var maxQtyAtBinLevel = lstItemBins.Where(e => e.BinNumber.Equals(cartitem.BinName)).FirstOrDefault();
                                var tmpBinId = (maxQtyAtBinLevel != null && maxQtyAtBinLevel.ID > 0) ? maxQtyAtBinLevel.ID : cartitem.BinId.GetValueOrDefault(0);
                                var onOrderQtyAtBin = objDAL.GetOrderdQtyOfItemBinWise(SessionHelper.RoomID, SessionHelper.CompanyID, objItemMasterDTO.GUID, tmpBinId);
                                var tmponOrderQtyAtBin = onOrderQtyAtBin;
                                double itemOrderQtySoFar = 0;

                                if (itemsOrderQtyForBinMinMax.ContainsKey(Convert.ToString(objItemMasterDTO.GUID) + "_" + cartitem.BinName))
                                {
                                    itemOrderQtySoFar += itemsOrderQtyForBinMinMax[Convert.ToString(objItemMasterDTO.GUID) + "_" + cartitem.BinName];
                                }

                                if (maxQtyAtBinLevel != null && maxQtyAtBinLevel.MaximumQuantity.HasValue && maxQtyAtBinLevel.MaximumQuantity.Value > 0 && (tmponOrderQtyAtBin + itemOrderQtySoFar + (cartitem.Quantity.GetValueOrDefault(0))) > maxQtyAtBinLevel.MaximumQuantity.Value)
                                {
                                    if (!(rejectedOrderLineItems.ContainsKey(objItemMasterDTO.ItemNumber)))
                                    {
                                        rejectedOrderLineItems[objItemMasterDTO.ItemNumber] = ResOrder.NotAddedBinMaxQtyReached;
                                    }
                                    rejectedOrderLineItemsGuids.Add(Convert.ToString(cartitem.GUID));
                                    rejectedDueToPreventMaxValidation++;
                                    continue;
                                }
                                else
                                {
                                    itemsOrderQtyForBinMinMax[Convert.ToString(objItemMasterDTO.GUID) + "_" + cartitem.BinName] =
                                        (itemsOrderQtyForBinMinMax.ContainsKey(Convert.ToString(objItemMasterDTO.GUID) + "_" + cartitem.BinName)
                                        ? (itemsOrderQtyForBinMinMax[Convert.ToString(objItemMasterDTO.GUID) + "_" + cartitem.BinName] + (cartitem.Quantity.GetValueOrDefault(0)))
                                        : cartitem.Quantity.GetValueOrDefault(0)
                                        );
                                }
                            }
                        }

                        if (objRoomDTO.PreventMaxOrderQty == (int)PreventMaxOrderQty.OnOrder && rejectedOrderLineItemsGuids.Any())
                        {
                            List<string> Items = order.OrderLineItemsIds.Split(',').Select(i => i.Trim()).Where(i => i != string.Empty).ToList(); //Split them all and remove spaces
                            foreach (var guid in rejectedOrderLineItemsGuids)
                            {
                                Items.Remove(guid);
                            }
                            order.OrderLineItemsIds = string.Join(", ", Items.ToArray());
                        }

                        if (rejectedDueToPreventMaxValidation > 0)
                        {
                            if (rejectedDueToPreventMaxValidation == lstids.Count)
                            {
                                ordertoBeDelete.Add(orderCount);
                            }
                        }
                    }
                }
            }
            if (ordertoBeDelete != null && ordertoBeDelete.Any())
            {
                foreach (var index in ordertoBeDelete)
                {
                    lstOrders.RemoveAt(index);
                }
            }

            return true;
        }

        public ActionResult GenerateAllSO()
        {
            try
            {
                CartItemDAL CartItemDAL = new CartItemDAL(SessionHelper.EnterPriseDBName);

                long SessionUserId = SessionHelper.UserID;
                CartItemDAL.SuggestedOrderRoom(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriceID, SessionUserId);
                return RedirectToAction("CartItemList", "Replenish");
            }
            catch
            {

                return RedirectToAction("CartItemList", "Replenish");
            }
        }

        [HttpPost]
        public JsonResult CreateTransfers(int Action, string Ids)
        {
            // RoomDTO objRoom = new RoomDAL(SessionHelper.EnterPriseDBName).GetRoomByIDPlain(SessionHelper.RoomID);
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            string columnList = "ID,RoomName";
            RoomDTO objRoom = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");

            if (objRoom != null && objRoom.ID > 0)
            {
                long SessionUserId = SessionHelper.UserID;
                CartItemDAL objCartItemDAL = new CartItemDAL(SessionHelper.EnterPriseDBName);
                IList<TransferMasterDTO> lstTransfers = objCartItemDAL.GetTransfersByCartIds(Ids, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriseDBName, SessionHelper.UserSupplierIds, 1, SessionHelper.EnterPriceID, SessionUserId);
                GlobalHost.ConnectionManager.GetHubContext<eTurnsHub>().Clients.Group(SessionHelper.EnterPriceID + "_" + SessionHelper.CompanyID + "_" + SessionHelper.RoomID).UpdateRedCircleCountInClients();
                return Json(new { Message = ResMessage.ActionExecuted, Status = "ok" }, JsonRequestBehavior.AllowGet); //"Action execuited successfully"
            }
            else
            {
                return Json(new { Message = string.Format(ResMessage.MsgDoesNotExist, ResCommon.Room), Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult CreateTransfersNew(int Action, string Ids, string RequiredDateStr, Int64 ReplineshRoomID, string TransferNumber, int TransferStatus, string Comment, string StagingName, string UDF1, string UDF2, string UDF3, string UDF4, string UDF5, string TransferQuantityString)
        {
            // RoomDTO objRoom = new RoomDAL(SessionHelper.EnterPriseDBName).GetRoomByIDPlain(SessionHelper.RoomID);



            #region Dynamic Validation

            TransferMasterDTO TempDTO = new TransferMasterDTO()
            {
                RequestingRoomID = ReplineshRoomID,
                TransferNumber = TransferNumber,
                TransferStatus = TransferStatus,
                Comment = Comment,
                StagingName = StagingName,
            };

            if (!string.IsNullOrWhiteSpace(RequiredDateStr))
            {
                TempDTO.RequireDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(RequiredDateStr, (SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone);
            }

            var validationResultList = DTOCommonUtils.ValidateDTO<TransferMasterDTO>(TempDTO, ControllerContext, new List<string>() { });

            if (validationResultList.HasErrors())
            {
                string msg = validationResultList.GetShortErrorMessage(typeof(RequiredAttributeAdapter));
                return Json(new { Message = msg, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }

            #endregion

            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            string columnList = "ID,RoomName";
            RoomDTO objRoom = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");
            long SessionUserId = SessionHelper.UserID;
            if (objRoom != null)
            {
                if (ReplineshRoomID != 0)
                {

                    DateTime RequiredDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(RequiredDateStr, (SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone);
                    CartItemDAL objCartItemDAL = new CartItemDAL(SessionHelper.EnterPriseDBName);
                    bool IsApprove = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.TransferApproval, eTurnsWeb.Helper.SessionHelper.PermissionType.Approval);

                    if (IsApprove)
                    {

                        if (TransferStatus == (int)eTurns.DTO.TransferStatus.Submitted)
                        {
                            TransferStatus = (int)eTurns.DTO.TransferStatus.Transmitted;
                        }
                    }
                    IList<TransferMasterDTO> lstTransfers = objCartItemDAL.GetTransfersByCartIdsUsingReplinish(Ids, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriseDBName, SessionHelper.UserSupplierIds, 1, SessionHelper.EnterPriceID, ReplineshRoomID, RequiredDate, TransferNumber, TransferStatus, StagingName, Comment, UDF1, UDF2, UDF3, UDF4, UDF5, SessionUserId, TransferQuantityString);

                    if (lstTransfers == null || lstTransfers.Count() == 0)
                    {
                        return Json(new { Message = ResMessage.NotaSingleItemTransfer, Status = "NotASingleItem" }, JsonRequestBehavior.AllowGet); // "There is not a single Item available in replinesh Room."
                    }
                    else
                    {
                        TransferController objTransferContlr = new TransferController();
                        foreach (TransferMasterDTO objDTO in lstTransfers)
                        {
                            if (objDTO.TransferStatus == (int)eTurns.DTO.TransferStatus.Submitted)
                            {
                                objTransferContlr.SendMailToApprovalAuthority(objDTO);
                            }
                            if (objDTO.TransferStatus == (int)eTurns.DTO.TransferStatus.Transmitted)
                            {
                                objTransferContlr.SendMailForApprovedOrRejected(objDTO, "Approved");
                            }
                        }
                    }

                    List<Guid> arrcartguids = new List<Guid>();

                    if (!string.IsNullOrWhiteSpace(Ids))
                    {
                        string[] arrguids = Ids.Split(',');
                        foreach (string item in arrguids)
                        {
                            Guid objid = Guid.Empty;
                            if (Guid.TryParse(item, out objid))
                            {
                                arrcartguids.Add(objid);
                            }
                        }
                    }
                    int TotalTRansferedLineItem = 0;
                    if (lstTransfers != null && lstTransfers.Count > 0)
                    {
                        TotalTRansferedLineItem = lstTransfers.FirstOrDefault().NoOfItems ?? 0;
                    }

                    GlobalHost.ConnectionManager.GetHubContext<eTurnsHub>().Clients.Group(SessionHelper.EnterPriceID + "_" + SessionHelper.CompanyID + "_" + SessionHelper.RoomID).UpdateRedCircleCountInClients();

                    if (arrcartguids.Count != TotalTRansferedLineItem)
                    {
                        int totalUnTransferedItem = arrcartguids.Count - TotalTRansferedLineItem;
                        return Json(new { Message = totalUnTransferedItem + " " + ResTransfer.ItemsNotInsertedAsNotExistInReplenishRoom, Status = "partialtrasnfer" }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { Message = ResMessage.ActionExecuted, Status = "ok" }, JsonRequestBehavior.AllowGet); //"Action execuited successfully"
                }
                else
                {
                    return Json(new { Message = ResMessage.NoRepliNeshRoomMessage, Status = "fail" }, JsonRequestBehavior.AllowGet); //"You can not add Transfer. Please select replenish room for current Room."
                }
            }
            else
            {
                return Json(new { Message = string.Format(ResMessage.MsgDoesNotExist, ResCommon.Room), Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region RedCount For Replenish

        /// <summary>
        /// GetReplinshRedCount
        /// </summary>
        /// <param name="CurrentModule"></param>
        /// <returns></returns>
        public JsonResult GetReplinshRedCount()
        {
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<RedCountDTO> lstRedCount = objCommonDAL.GetRedCountByModuleType("Replenish", SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserSupplierIds, SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowOrderToConsignedItem));
            Int64 ReplenishMenuButtonCount = 0;
            Int64 CartMenuLinkCount = 0;
            Int64 CartTransMenuLinkCount = 0;
            Int64 OrderMenuLinkCount = 0;
            Int64 ReturnOrderMenuLinkCount = 0;
            Int64 ReceiveMenuLinkCount = 0;
            Int64 TransferMenuLinkCount = 0;
            Int64 CartOrderMenuLinkCount = 0;
            Int64 ToolReceiveMenuLinkCount = 0;
            Int64 ToolOrderMenuLinkCount = 0;
            Int64 CartSuggestedReturnOrderMenuLinkCount = 0;
            Int64 QuoteMenuLinkCount = 0;

            List<RedCountDTO> lstCartRedCount = lstRedCount.Where(x => x.ModuleName == "Cart").ToList();
            List<RedCountDTO> lstCartTransRedCount = lstRedCount.Where(x => x.ModuleName == "CartTransfer").ToList();
            List<RedCountDTO> lstOrderRedCount = lstRedCount.Where(x => x.ModuleName == "Order").ToList();
            List<RedCountDTO> lstReturnOrderRedCount = lstRedCount.Where(x => x.ModuleName == "ReturnOrder").ToList();
            List<RedCountDTO> lstReceiveRedCount = lstRedCount.Where(x => x.ModuleName == "Receive").ToList();
            List<RedCountDTO> lstTransferRedCount = lstRedCount.Where(x => x.ModuleName == "Transfer").ToList();
            List<RedCountDTO> lstToolReceiveRedCount = lstRedCount.Where(x => x.ModuleName == "ReceiveTool").ToList();
            List<RedCountDTO> lstToolOrderRedCount = lstRedCount.Where(x => x.ModuleName == "ToolOrder").ToList();
            List<RedCountDTO> lstQuoteRedCount = lstRedCount.Where(x => x.ModuleName == "Quote").ToList();

            bool PreventTransmittedOrdersInRedCount = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.PreventTransmittedOrdersFromDisplayingInRedCount);
            List<RedCountDTO> lstSuggestedReturnOrderMenuLinkCount = lstRedCount.Where(x => x.ModuleName == "CartSuggestedReturn").ToList();

            if (PreventTransmittedOrdersInRedCount)
            {
                //TODO: Commented by Chirag due to issue WI - 1953 and Same condition below.  on Dated :2016-Aug-01
                //TODO: Un-Commented by Chirag due to issue WI - 2156 and Same condition below. on Dated :2016-Oct-03
                lstOrderRedCount = lstOrderRedCount.Where(x => (x.Status == "UnSubmitted" || x.Status == "ToBeApproved")).ToList();
                lstReceiveRedCount = new List<RedCountDTO>();
                lstToolOrderRedCount = lstToolOrderRedCount.Where(x => (x.Status == "UnSubmitted" || x.Status == "ToBeApproved")).ToList();
            }


            CartOrderMenuLinkCount = lstCartRedCount.Sum(x => x.RecCircleCount);
            CartTransMenuLinkCount = lstCartTransRedCount.Sum(x => x.RecCircleCount);
            CartSuggestedReturnOrderMenuLinkCount = lstSuggestedReturnOrderMenuLinkCount.Sum(x => x.RecCircleCount);
            CartMenuLinkCount = CartOrderMenuLinkCount;
            //+ CartTransMenuLinkCount + CartSuggestedReturnOrderMenuLinkCount;
            OrderMenuLinkCount = lstOrderRedCount.Where(x => (x.Status == "UnSubmitted" || x.Status == "ToBeApproved")).Sum(x => x.RecCircleCount);
            ToolOrderMenuLinkCount = lstToolOrderRedCount.Where(x => (x.Status == "UnSubmitted" || x.Status == "ToBeApproved")).Sum(x => x.RecCircleCount);
            ReturnOrderMenuLinkCount = lstReturnOrderRedCount.Where(x => (x.Status == "UnSubmitted" || x.Status == "ToBeApproved")).Sum(x => x.RecCircleCount);
            QuoteMenuLinkCount = lstQuoteRedCount.Where(x => (x.Status == "UnSubmitted" || x.Status == "ToBeApproved")).Sum(x => x.RecCircleCount);

            ReceiveMenuLinkCount = lstReceiveRedCount.Where(x => x.Status == "InComplete").Sum(x => x.RecCircleCount);
            TransferMenuLinkCount = lstTransferRedCount.Where(x => (x.Status == "UnSubmitted" || x.Status == "ToBeApproved" || x.Status == "Receive")).Sum(x => x.RecCircleCount);
            ToolReceiveMenuLinkCount = lstToolReceiveRedCount.Where(x => x.Status == "InComplete").Sum(x => x.RecCircleCount);
            bool IsCart = SessionHelper.GetModulePermission(SessionHelper.ModuleList.Cart, SessionHelper.PermissionType.View);
            bool IsOrder = SessionHelper.GetModulePermission(SessionHelper.ModuleList.Orders, SessionHelper.PermissionType.View);
            bool IsTransfer = SessionHelper.GetModulePermission(SessionHelper.ModuleList.Transfer, SessionHelper.PermissionType.View);
            bool IsToolAssetOrder = SessionHelper.GetModulePermission(SessionHelper.ModuleList.ToolAssetOrder, SessionHelper.PermissionType.View);
            bool IsQuote = SessionHelper.GetModulePermission(SessionHelper.ModuleList.Quote, SessionHelper.PermissionType.View);

            if (!IsCart)
                CartMenuLinkCount = 0;

            if (!IsOrder)
                OrderMenuLinkCount = 0;

            if (!IsTransfer)
                TransferMenuLinkCount = 0;

            if (!IsToolAssetOrder)
                ToolOrderMenuLinkCount = 0;

            if (!IsQuote)
                QuoteMenuLinkCount = 0;

            ReplenishMenuButtonCount = CartMenuLinkCount + OrderMenuLinkCount + ReceiveMenuLinkCount + TransferMenuLinkCount + QuoteMenuLinkCount;

            return Json(new
            {
                Message = "ok",
                Status = "ok",
                ModuleType = "Replenish",
                ReplenishMenuButtonCount = ReplenishMenuButtonCount,
                CartMenuLinkCount = CartMenuLinkCount,
                CartTransMenuLinkCount = CartTransMenuLinkCount,
                CartOrderMenuLinkCount = CartOrderMenuLinkCount,
                OrderMenuLinkCount = OrderMenuLinkCount,
                QuoteMenuLinkCount = QuoteMenuLinkCount,
                ReturnOrderMenuLinkCount = ReturnOrderMenuLinkCount,
                ReceiveMenuLinkCount = ReceiveMenuLinkCount,
                TransferMenuLinkCount = TransferMenuLinkCount,
                CartRedCount = lstCartRedCount,
                OrderRedCount = lstOrderRedCount,
                ReceiveRedCount = lstReceiveRedCount,
                TransferRedCount = lstTransferRedCount,
                ReturnOrderRedCount = lstReturnOrderRedCount,
                ToolReceiveMenuLinkCount = ToolReceiveMenuLinkCount,
                ToolReceiveRedCount = lstToolReceiveRedCount,
                ToolOrderMenuLinkCount = ToolOrderMenuLinkCount,
                ToolOrderRedCount = lstToolOrderRedCount,
                CartSuggestedReturnOrderMenuLinkCount = CartSuggestedReturnOrderMenuLinkCount,
                QuoteRedCount = lstQuoteRedCount,
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Add Quick list item to direct to cart

        public JsonResult AddQLItemsToCart(string QuickListGUID, float Quantity, string UDF1, string UDF2, string UDF3, string UDF4, string UDF5)
        {
            string status = "";
            string message = ResMessage.SaveMessage;
            JsonResult returnJsonResult = new JsonResult();
            try
            {
                QuickListDAL objQLDtlDAL = new QuickListDAL(SessionHelper.EnterPriseDBName);
                string QLGuid = QuickListGUID;
                double QLQty = Quantity;

                if (QLQty <= 0)
                    QLQty = 1;
                List<QuickListDetailDTO> objQLDtlDTO = objQLDtlDAL.GetQuickListItemsRecords(SessionHelper.RoomID, SessionHelper.CompanyID, QLGuid, SessionHelper.UserSupplierIds).Where(x => x.IsDeleted == false).ToList();
                double? ItemQuantity = 0;
                ItemMasterDAL objItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                if (objQLDtlDTO.Count > 0)
                {
                    List<CartItemDTO> lstCartItems = new List<CartItemDTO>();

                    foreach (QuickListDetailDTO qlItem in objQLDtlDTO)
                    {
                        CartItemDTO objCartItem = new CartItemDTO();
                        ItemMasterDTO tempItemDTO = new ItemMasterDTO();
                        tempItemDTO = objItemDAL.GetItemWithoutJoins(null, qlItem.ItemGUID);
                        if (tempItemDTO != null && tempItemDTO.ItemType != 4
                            && tempItemDTO.IsOrderable == true)
                        {
                            ItemQuantity = qlItem.Quantity * QLQty;
                            if (tempItemDTO.IsEnforceDefaultReorderQuantity == true && tempItemDTO.DefaultReorderQuantity != null && tempItemDTO.DefaultReorderQuantity > 0)
                            {
                                while (ItemQuantity < tempItemDTO.DefaultReorderQuantity)
                                    ItemQuantity = ItemQuantity + 1;

                                while (ItemQuantity % tempItemDTO.DefaultReorderQuantity != 0)
                                    ItemQuantity = ItemQuantity + 1;
                            }

                            if (tempItemDTO.IsPurchase)
                                objCartItem.ReplenishType = "Purchase";
                            else if (tempItemDTO.IsTransfer)
                                objCartItem.ReplenishType = "Transfer";

                            objCartItem.ID = 0;
                            objCartItem.ItemGUID = qlItem.ItemGUID;
                            objCartItem.ItemNumber = qlItem.ItemNumber;
                            objCartItem.BinName = qlItem.BinName;
                            objCartItem.Quantity = ItemQuantity;
                            objCartItem.UDF1 = UDF1;
                            objCartItem.UDF2 = UDF2;
                            objCartItem.UDF3 = UDF3;
                            objCartItem.UDF4 = UDF4;
                            objCartItem.UDF5 = UDF5;
                            lstCartItems.Add(objCartItem);
                        }
                    }
                    if (lstCartItems.Count > 0)
                        returnJsonResult = UpdateQLCartItemsBulk(lstCartItems, true);
                    else
                    {
                        returnJsonResult = Json(new { Message = ResQuickList.NoItemFoundInQuickList, Status = "Fail" }, JsonRequestBehavior.AllowGet);
                    }
                }
                return returnJsonResult;
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                message = "Error";
                status = "fail";
                return Json(new { Message = ex.Message, Status = status }, JsonRequestBehavior.AllowGet);
                //throw;
            }
        }

        public JsonResult UpdateQLCartItemsBulk(List<CartItemDTO> lstCartItems, bool IsOnlyFromUI)
        {
            string message = ResMessage.SaveMessage;
            lstCartItems.ForEach(t =>
            {
                t.CompanyID = SessionHelper.CompanyID;
                t.Created = DateTimeUtility.DateTimeNow;
                t.CreatedBy = SessionHelper.UserID;
                t.CreatedByName = SessionHelper.UserName;
                t.IsArchived = false;
                t.IsDeleted = false;
                t.IsKitComponent = false;
                t.LastUpdatedBy = SessionHelper.UserID;
                t.Room = SessionHelper.RoomID;
                t.RoomName = SessionHelper.RoomName;
                t.Status = "A";
                t.Updated = DateTimeUtility.DateTimeNow;
                t.UpdatedByName = SessionHelper.UserName;
                t.IsAutoMatedEntry = false;
                t.IsOnlyFromItemUI = IsOnlyFromUI;
            });
            string status = "";
            string locationMSG = "";
            CartItemDAL objCartItemAPIController = new CartItemDAL(SessionHelper.EnterPriseDBName);
            CartItemDTO objCartItemDTO = new CartItemDTO();
            long SessionUserId = SessionHelper.UserID;
            try
            {
                List<CartItemDTO> lstreturnitems = objCartItemAPIController.SaveCartItems(lstCartItems, SessionHelper.EnterPriceID, SessionUserId);
                if (lstreturnitems != null && lstreturnitems.Count > 0 && lstreturnitems.Count(t => t.EnforsedCartQuanity == true) > 0)
                {
                    message = ResCartItem.QuantityAdjustmentMessage;
                }
                else
                {
                    message = ResMessage.SaveMessage;
                }
                status = "ok";
            }
            catch
            {
                message = ResMessage.SaveErrorMsg;
                status = "fail";
            }
            GlobalHost.ConnectionManager.GetHubContext<eTurnsHub>().Clients.Group(SessionHelper.EnterPriceID + "_" + SessionHelper.CompanyID + "_" + SessionHelper.RoomID).UpdateRedCircleCountInClients();
            return Json(new { Message = message, Status = status, LocationMSG = locationMSG }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Return for Cart Item 5330

        [HttpPost]
        public ActionResult OpenCreateReturnOrderPopup(string Ids, string OrderLineItemUDF1, string OrderLineItemUDF2, string OrderLineItemUDF3, string OrderLineItemUDF4, string OrderLineItemUDF5, string OrderItemQuantity)
        {
            bool isInsert = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ReturnOrder, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
            if (isInsert)
            {
                List<SelectListItem> returnList = new List<SelectListItem>();
                returnList.Add(new SelectListItem() { Text = ResOrder.GetOrderStatusText(OrderStatus.UnSubmitted.ToString()), Value = ((int)OrderStatus.UnSubmitted).ToString() });
                bool IsSubmit = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.OrderSubmit);
                bool IsApprove = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.OrderApproval);

                if (IsApprove)
                {
                    returnList.Add(new SelectListItem() { Text = ResOrder.GetOrderStatusText(OrderStatus.Submitted.ToString()), Value = ((int)OrderStatus.Submitted).ToString() });
                    returnList.Add(new SelectListItem() { Text = ResOrder.GetOrderStatusText(OrderStatus.Approved.ToString()), Value = ((int)OrderStatus.Approved).ToString() });
                    returnList.Add(new SelectListItem() { Text = ResOrder.GetOrderStatusText(OrderStatus.Transmitted.ToString()), Value = ((int)OrderStatus.Transmitted).ToString() });
                }
                else if (IsSubmit)
                {
                    returnList.Add(new SelectListItem() { Text = ResOrder.GetOrderStatusText(OrderStatus.Submitted.ToString()), Value = ((int)OrderStatus.Submitted).ToString() });
                }
                ViewBag.OrderStatusList = returnList;

                CartItemDAL objCartItemDAL = new CartItemDAL(SessionHelper.EnterPriseDBName);
                var tmpsupplierIds = new List<long>();
                IList<OrderMasterDTO> lstOrders = objCartItemDAL.GetReturnOrdersByCartIds(Ids, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriseDBName, tmpsupplierIds, OrderLineItemUDF1, OrderLineItemUDF2, OrderLineItemUDF3, OrderLineItemUDF4, OrderLineItemUDF5, SessionHelper.EnterPriceID, OrderItemQuantity);
                if (lstOrders != null && lstOrders.Count > 0)
                {
                    lstOrders.ToList().ForEach(t =>
                    {
                        if (IsApprove)
                        {
                            t.OrderStatus = (int)OrderStatus.Approved;
                        }
                        else if (IsSubmit)
                        {
                            t.OrderStatus = (int)OrderStatus.Submitted;
                        }
                        else
                        {

                            t.OrderStatus = (int)OrderStatus.UnSubmitted;
                        }
                        SupplierAccountDetailsDAL objSupplierAccountDetailsDAL = new SupplierAccountDetailsDAL(SessionHelper.EnterPriseDBName);
                        System.Collections.Generic.List<SupplierAccountDetailsDTO> objSupplierAccount = objSupplierAccountDetailsDAL.GetAllAccountsBySupplierID(Convert.ToInt64(t.Supplier), SessionHelper.RoomID, SessionHelper.CompanyID).Where(s => s.IsDefault == true).ToList();
                        if (objSupplierAccount != null && objSupplierAccount.Count() > 0)
                        {
                            ViewBag.SupplierAccount = objSupplierAccount;
                            t.SupplierAccountGuid = objSupplierAccount.Where(s => s.IsDefault == true).FirstOrDefault().GUID;
                        }
                    });

                }
                return PartialView("ReturnOrderFromCart", lstOrders);
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public JsonResult CreateReturnOrdersfromCart(List<OrderMasterDTO> lstOrders)
        {
            try
            {
                #region check dynamic validation rules
                if (lstOrders != null && lstOrders.Count > 0)
                {
                    string validationMsg = "";
                    int cnt = 0;
                    foreach (OrderMasterDTO objOrderMasterDTO in lstOrders)
                    {
                        if (!string.IsNullOrWhiteSpace(objOrderMasterDTO.RequiredDateString))
                        {
                            objOrderMasterDTO.RequiredDate = DateTime.ParseExact(objOrderMasterDTO.RequiredDateString, Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult);
                        }

                        var validationResultList = DTOCommonUtils.ValidateDTO<OrderMasterDTO>(objOrderMasterDTO, ControllerContext, new List<string>() { "Supplier", "PackSlipNumber", "CustomerName" });
                        if (validationResultList.HasErrors())
                        {

                            if (!string.IsNullOrWhiteSpace(validationMsg))
                            {
                                validationMsg += "<br/><br/>";
                            }

                            string msg = validationResultList.GetShortErrorMessage(typeof(RequiredAttributeAdapter));
                            validationMsg += (string.Format(ResQuoteMaster.ValidationFailedFor, objOrderMasterDTO.SupplierName) + "<br/>" + msg);
                            cnt++;
                        }

                        if (cnt >= 5)
                        {
                            // display message for 5 grid rows to shorten message
                            break;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(validationMsg))
                    {
                        return Json(new { Message = validationMsg, Status = "fail", lstOrders = lstOrders });
                    }
                }
                #endregion

                CartItemDAL objCartItemDAL = new CartItemDAL(SessionHelper.EnterPriseDBName);
                OrderMasterDAL objOrderMasterDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);

                bool IsSubmit = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.OrderSubmit);
                bool IsApprove = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.OrderApproval);

                int? PriseSelectionOption = 0;
                eTurns.DAL.RoomDAL onjRoomDAL = new eTurns.DAL.RoomDAL(SessionHelper.EnterPriseDBName);
                RoomModuleSettingsDTO objRoomModuleSettingsDTO = onjRoomDAL.GetRoomModuleSettings(eTurnsWeb.Helper.SessionHelper.CompanyID, eTurnsWeb.Helper.SessionHelper.RoomID, (long)eTurnsWeb.Helper.SessionHelper.ModuleList.ReturnOrder);
                if (objRoomModuleSettingsDTO != null)
                    PriseSelectionOption = objRoomModuleSettingsDTO.PriseSelectionOption;

                if (PriseSelectionOption != 1 && PriseSelectionOption != 2)
                    PriseSelectionOption = 1;

                if (lstOrders != null && lstOrders.Count > 0)
                {

                    //---------------------- Check For Order Number Duplication ----------------------
                    //
                    // RoomDTO roomDTO = new eTurns.DAL.RoomDAL(SessionHelper.EnterPriseDBName).GetRoomByIDPlain(eTurnsWeb.Helper.SessionHelper.RoomID);
                    CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                    string columnList = "ID,RoomName,IsAllowOrderDuplicate";
                    RoomDTO roomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");

                    if (roomDTO != null && roomDTO.IsAllowOrderDuplicate != true)
                    {
                        var OrdrGroup = (from O in lstOrders
                                         group O by new { OrderNumberGB = O.OrderNumber.Trim().ToLower() } into OGB
                                         select new
                                         {
                                             OrderNumber = OGB.Key.OrderNumberGB,
                                             TotalCount = OGB.Count()
                                         }).Where(x => x.TotalCount > 1).ToList();

                        if (OrdrGroup.Count > 0)
                        {
                            return Json(new { Message = string.Format(ResOrder.ReturnOrderNumberDuplicateInList, OrdrGroup[0].OrderNumber), Status = "fail", lstOrders = lstOrders });
                        }
                        var returnOrderNoExistMsg = ResOrder.ReturnOrderNumberAlreadyExist;
                        foreach (OrderMasterDTO objOrderMasterDTO in lstOrders)
                        {
                            if (objOrderMasterDAL.IsOrderNumberDuplicateById(objOrderMasterDTO.OrderNumber, objOrderMasterDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID))
                            {
                                return Json(new { Message = string.Format(returnOrderNoExistMsg, objOrderMasterDTO.OrderNumber), Status = "fail", lstOrders = lstOrders });
                            }
                        }
                    }
                    DollerApprovalLimitDTO objDollarLimt = null;
                    eTurns.DAL.UserMasterDAL userDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                    ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                    ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                    string approvalErrorMsg = string.Empty;

                    foreach (OrderMasterDTO objOrderMasterDTO in lstOrders)
                    {
                        objOrderMasterDTO.RequiredDate = DateTime.ParseExact(objOrderMasterDTO.RequiredDateString, Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult);
                        if (IsApprove)
                        {
                            double? UserApprovalLimit = null;
                            double UserUsedLimit = 0;
                            double OrderCost = 0;
                            double OrderPrice = 0;
                            objDollarLimt = null;

                            double? ItemApprovedQuantity = null;

                            objDollarLimt = userDAL.GetOrderLimitByUserId(SessionHelper.UserID, SessionHelper.CompanyID, SessionHelper.RoomID);
                            if (objDollarLimt != null)
                            {
                                UserApprovalLimit = objDollarLimt.DollerLimit > 0 ? objDollarLimt.DollerLimit : null;
                                UserUsedLimit = objDollarLimt.UsedLimit;
                                ItemApprovedQuantity = objDollarLimt.ItemApprovedQuantity > 0 ? objDollarLimt.ItemApprovedQuantity : null;
                            }

                            if (objOrderMasterDTO.OrderStatus == (int)OrderStatus.Submitted)
                            {
                                objOrderMasterDTO.OrderStatus = (int)OrderStatus.Approved;
                            }

                            if (objOrderMasterDTO.OrderStatus == (int)OrderStatus.Approved || objOrderMasterDTO.OrderStatus == (int)OrderStatus.Transmitted)
                            {
                                List<Guid> lstids = new List<Guid>();
                                if (!string.IsNullOrWhiteSpace(objOrderMasterDTO.OrderLineItemsIds))
                                {
                                    foreach (var item in objOrderMasterDTO.OrderLineItemsIds.Split(','))
                                    {
                                        Guid tempid = Guid.Empty;
                                        if (Guid.TryParse(item, out tempid))
                                            lstids.Add(tempid);
                                    }
                                    List<CartItemDTO> lstCartItems = objCartItemDAL.GetCartItemsByGuids(lstids, SessionHelper.RoomID, SessionHelper.CompanyID, true, null);
                                    foreach (CartItemDTO cartitem in lstCartItems)
                                    {
                                        objItemMasterDTO = objItemMasterDAL.GetItemByGuidPlain((cartitem.ItemGUID ?? Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);

                                        if (cartitem != null && cartitem.Quantity != null
                                                && cartitem.Quantity > 0)
                                        {
                                            if (objDollarLimt != null && objDollarLimt.OrderLimitType == OrderLimitType.PerOrder && IsApprove && objOrderMasterDTO.OrderStatus == (int)OrderStatus.Approved
                                               && ItemApprovedQuantity > 0 && cartitem.Quantity > (ItemApprovedQuantity))
                                            {
                                                return Json(new { Message = string.Format(ResOrder.CantApproveMoreThanPerOrderItemQtyApprovalLimit, cartitem.Quantity, ItemApprovedQuantity), Status = "fail" }, JsonRequestBehavior.AllowGet);
                                            }
                                        }

                                        CostUOMMasterDTO costUOM = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName).GetCostUOMByID(objItemMasterDTO.CostUOMID.GetValueOrDefault(0));
                                        if (costUOM == null)
                                            costUOM = new CostUOMMasterDTO() { CostUOMValue = 1 };

                                        if (objItemMasterDTO != null && cartitem.Quantity.GetValueOrDefault(0) > 0)
                                        {
                                            OrderCost += (objItemMasterDTO.Cost.GetValueOrDefault(0) * cartitem.Quantity.GetValueOrDefault(0))
                                                     / (costUOM.CostUOMValue.GetValueOrDefault(0) > 0 ? costUOM.CostUOMValue.GetValueOrDefault(1) : 1);

                                            OrderPrice += (objItemMasterDTO.SellPrice.GetValueOrDefault(0) * cartitem.Quantity.GetValueOrDefault(0))
                                                   / (costUOM.CostUOMValue.GetValueOrDefault(0) > 0 ? costUOM.CostUOMValue.GetValueOrDefault(1) : 1);

                                        }
                                    }
                                }
                            }


                            if (SessionHelper.RoleID > 0 && objDollarLimt != null)
                            {
                                if (PriseSelectionOption == 1)
                                {
                                    if (objDollarLimt.OrderLimitType == OrderLimitType.All && OrderPrice > (UserApprovalLimit - UserUsedLimit))
                                        approvalErrorMsg += "<br/>" + string.Format(ResOrder.SupplierOrderCantApproveMTRemainingOrderApprovalLimit, objOrderMasterDTO.SupplierName, objOrderMasterDTO.OrderNumber, OrderPrice, (UserApprovalLimit - UserUsedLimit)) + "<br/>";
                                    else if (objDollarLimt.OrderLimitType == OrderLimitType.PerOrder && OrderPrice > (UserApprovalLimit))
                                        approvalErrorMsg += "<br/>" + string.Format(ResOrder.SupplierOrderCantApproveMTPerOrderApprovalLimit, objOrderMasterDTO.SupplierName, objOrderMasterDTO.OrderNumber, OrderPrice, UserApprovalLimit) + "<br/>";
                                }
                                else
                                {
                                    if (objDollarLimt.OrderLimitType == OrderLimitType.All && OrderCost > (UserApprovalLimit - UserUsedLimit))
                                        approvalErrorMsg += "<br/>" + string.Format(ResOrder.SupplierOrderCantApproveMTRemainingOrderApprovalLimit, objOrderMasterDTO.SupplierName, objOrderMasterDTO.OrderNumber, OrderCost, (UserApprovalLimit - UserUsedLimit)) + "<br/>";
                                    else if (objDollarLimt.OrderLimitType == OrderLimitType.PerOrder && OrderCost > (UserApprovalLimit))
                                        approvalErrorMsg += "<br/>" + string.Format(ResOrder.SupplierOrderCantApproveMTPerOrderApprovalLimit, objOrderMasterDTO.SupplierName, objOrderMasterDTO.OrderNumber, OrderCost, UserApprovalLimit) + "<br/>";
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(approvalErrorMsg))
                    {
                        return Json(new { Message = approvalErrorMsg, Status = "fail" }, JsonRequestBehavior.AllowGet);
                    }
                }


                Dictionary<string, string> rejectedOrderLineItems = new Dictionary<string, string>();
                var validateCartItem = ValidateOrderLineItemsForMaxOrderQty(lstOrders, out rejectedOrderLineItems);
                Dictionary<string, string> rejectedOrderLineItemstest = new Dictionary<string, string>();
                long SessionUserId = SessionHelper.UserID;
                lstOrders = objCartItemDAL.CreateRetrunOrdersByCart(lstOrders, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriseDBName, 1, SessionHelper.EnterPriceID, out rejectedOrderLineItemstest, SessionUserId, "");

                if (lstOrders != null && lstOrders.Count > 0)
                {
                    OrderController objOrdContlr = new OrderController();
                    SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                    eTurns.DAL.UserMasterDAL objUserDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                    OrderMasterDAL orderDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);

                    foreach (OrderMasterDTO Orditem in lstOrders)
                    {
                        if (Orditem.OrderStatus == (int)OrderStatus.UnSubmitted)
                        {
                            SupplierMasterDTO objSupplier = objSupplierMasterDAL.GetSupplierByIDPlain(Orditem.Supplier ?? 0);
                            objOrdContlr.SendMailOrderUnSubmitted(objSupplier, Orditem);
                        }

                        if (Orditem.OrderStatus == (int)OrderStatus.Submitted)
                        {
                            objOrdContlr.SendMailToApprovalAuthority(Orditem);
                        }

                        if (Orditem.OrderStatus == (int)OrderStatus.Approved || Orditem.OrderStatus == (int)OrderStatus.Transmitted)
                        {
                            SupplierMasterDTO objSupplier = objSupplierMasterDAL.GetSupplierByIDPlain(Orditem.Supplier ?? 0);
                            objOrdContlr.SendMailToSupplier(objSupplier, Orditem);
                            double? UserApprovalLimit = null;
                            double UserUsedLimit = 0;
                            double OrderCost = 0;
                            double OrderPrice = 0;
                            DollerApprovalLimitDTO objDollarLimt = null;
                            eTurns.DAL.UserMasterDAL userDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                            objDollarLimt = userDAL.GetOrderLimitByUserId(SessionHelper.UserID, Orditem.CompanyID ?? 0, Orditem.Room ?? 0);

                            if (SessionHelper.RoleID > 0 && objDollarLimt != null && objDollarLimt.OrderLimitType == OrderLimitType.All)
                            {
                                UserApprovalLimit = objDollarLimt.DollerLimit;
                                UserUsedLimit = objDollarLimt.UsedLimit;

                                if (PriseSelectionOption == 1)
                                {
                                    OrderPrice = Orditem.OrderPrice;

                                    if (OrderPrice <= (UserApprovalLimit - UserUsedLimit))
                                    {
                                        userDAL.SaveDollerUsedLimt(OrderPrice, SessionHelper.UserID, Orditem.CompanyID ?? 0, Orditem.Room ?? 0);
                                    }
                                }
                                else
                                {
                                    OrderCost = Orditem.OrderCost.GetValueOrDefault(0);

                                    if (OrderCost <= (UserApprovalLimit - UserUsedLimit))
                                    {
                                        userDAL.SaveDollerUsedLimt(OrderCost, SessionHelper.UserID, Orditem.CompanyID ?? 0, Orditem.Room ?? 0);
                                    }
                                }
                            }
                        }
                    }
                }
                string message = string.Empty;
                string status = string.Empty;
                if (!rejectedOrderLineItems.Any())
                {
                    message = ResMessage.SaveMessage;
                    status = "ok";
                }
                else
                {
                    message = ResQuoteMaster.BelowGivenItemsRejected;
                    var itemNamberResource = ResItemMaster.ItemNumber;
                    var reasonResource = ResCommon.Reason;
                    foreach (KeyValuePair<string, string> entry in rejectedOrderLineItems)
                    {
                        message += (itemNamberResource + ": " + entry.Key + " ," + reasonResource + ": " + entry.Value);
                    }

                    status = "fail";
                }
                //GlobalHost.ConnectionManager.GetHubContext<eTurnsHub>().Clients.All.UpdateRedCircleCountInClients();
                GlobalHost.ConnectionManager.GetHubContext<eTurnsHub>().Clients.Group(SessionHelper.EnterPriceID + "_" + SessionHelper.CompanyID + "_" + SessionHelper.RoomID).UpdateRedCircleCountInClients();
                return Json(new { Message = message, Status = status, lstOrders = lstOrders });
            }
            catch
            {
                string message = ResMessage.SaveErrorMsg;
                string status = "fail";
                return Json(new { Message = message, Status = status, lstOrders = lstOrders });
            }
        }

        [OutputCache(Duration = 0, VaryByParam = "None")]
        public JsonResult GetNarrowCartDDData(string TextFieldName, bool IsArchived, bool IsDeleted, string ReplenishType = null)
        {
            Dictionary<string, int> retData = new Dictionary<string, int>();

            bool CartConsignedAllowed = true;
            if (Session["ConsignedAllowed"] != null)
            {
                CartConsignedAllowed = Convert.ToBoolean(Session["ConsignedAllowed"]);
            }
            int LoadDataCount = 0;
            //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));
            if (LoadDataCount == 0)
            {
                //LoadDataCount = Settinfile.Element("LoadNarrowDataCount") != null ? Convert.ToInt32(Settinfile.Element("LoadNarrowDataCount").Value) : 0;
                LoadDataCount = SiteSettingHelper.LoadNarrowDataCount != string.Empty ? Convert.ToInt32(SiteSettingHelper.LoadNarrowDataCount) : 0;
            }
            CartItemDAL objGLAccDAL = new CartItemDAL(SessionHelper.EnterPriseDBName);
            retData = objGLAccDAL.GetCartItemsByGuidsForNarrowSearch(SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, TextFieldName, SessionHelper.UserSupplierIds, CartConsignedAllowed, LoadDataCount, ReplenishType).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);

            return Json(new { DDData = retData }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNarrowCartUDFDDData(string UDFName, string UDFUniqueID, bool IsArchived, bool IsDeleted, string ReplenishType = null)
        {
            Dictionary<string, int> retData = new Dictionary<string, int>();

            bool CartConsignedAllowed = true;
            if (Session["ConsignedAllowed"] != null)
            {
                CartConsignedAllowed = Convert.ToBoolean(Session["ConsignedAllowed"]);
            }
            int LoadDataCount = 0;
            //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));
            if (LoadDataCount == 0)
            {
                //LoadDataCount = Settinfile.Element("LoadNarrowDataCount") != null ? Convert.ToInt32(Settinfile.Element("LoadNarrowDataCount").Value) : 0;
                LoadDataCount = SiteSettingHelper.LoadNarrowDataCount != string.Empty ? Convert.ToInt32(SiteSettingHelper.LoadNarrowDataCount) : 0;
            }
            CartItemDAL objGLAccDAL = new CartItemDAL(SessionHelper.EnterPriseDBName);
            retData = objGLAccDAL.GetCartItemsByGuidsForNarrowSearch(SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, UDFUniqueID, SessionHelper.UserSupplierIds, CartConsignedAllowed, LoadDataCount, ReplenishType).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);

            return Json(new { DDData = retData, UDFColName = UDFName }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Create Quote from Cart list page

        [HttpPost]
        public ActionResult OpenCreateQuotePopup(string Ids, string QuoteLineItemUDF1, string QuoteLineItemUDF2, string QuoteLineItemUDF3, string QuoteLineItemUDF4, string QuoteLineItemUDF5, string OrderItemQuantity,string QuoteSuppliers)
        {
            bool isInsert = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Quote, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
            if (isInsert)
            {
                List<SelectListItem> returnList = new List<SelectListItem>();
                returnList.Add(new SelectListItem() { Text = ResQuoteMaster.GetQuoteStatusText(QuoteStatus.UnSubmitted.ToString()), Value = ((int)QuoteStatus.UnSubmitted).ToString() });
                bool IsSubmit = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowanquotetobeSubmitted);
                bool IsApprove = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowanquotetobeApproved);
                if (IsApprove)
                {
                    returnList.Add(new SelectListItem() { Text = ResQuoteMaster.GetQuoteStatusText(QuoteStatus.Submitted.ToString()), Value = ((int)QuoteStatus.Submitted).ToString() });
                    returnList.Add(new SelectListItem() { Text = ResQuoteMaster.GetQuoteStatusText(QuoteStatus.Approved.ToString()), Value = ((int)QuoteStatus.Approved).ToString() });
                    CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                    string columnList = "ID,RoomName,IsAllowQuoteDuplicate,QuoteAutoSequence,DoSendQuotetoVendor";
                    RoomDTO roomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");
                    if (roomDTO != null && !roomDTO.DoSendQuotetoVendor)
                    {
                        returnList.Add(new SelectListItem() { Text = ResQuoteMaster.GetQuoteStatusText(QuoteStatus.Transmitted.ToString()), Value = ((int)QuoteStatus.Transmitted).ToString() });
                    }
                }
                else if (IsSubmit)
                {
                    returnList.Add(new SelectListItem() { Text = ResQuoteMaster.GetQuoteStatusText(QuoteStatus.Submitted.ToString()), Value = ((int)QuoteStatus.Submitted).ToString() });
                }
                ViewBag.QuoteStatusList = returnList;

                CartItemDAL objCartItemDAL = new CartItemDAL(SessionHelper.EnterPriseDBName);
                var tmpsupplierIds = new List<long>();
                IList<QuoteMasterDTO> lstQuotes = objCartItemDAL.GetQuotesByCartIds(Ids, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriseDBName, QuoteLineItemUDF1, QuoteLineItemUDF2, QuoteLineItemUDF3, QuoteLineItemUDF4, QuoteLineItemUDF5, SessionHelper.EnterPriceID, OrderItemQuantity, QuoteSuppliers);
                if (lstQuotes != null && lstQuotes.Count > 0)
                {
                    lstQuotes.ToList().ForEach(t =>
                    {
                        if (IsApprove)
                        {
                            t.QuoteStatus = (int)OrderStatus.Approved;
                        }
                        else if (IsSubmit)
                        {
                            t.QuoteStatus = (int)OrderStatus.Submitted;
                        }
                        else
                        {
                            t.QuoteStatus = (int)OrderStatus.UnSubmitted;
                        }
                    });

                }
                return PartialView("CreateQuoteFromCart", lstQuotes);
            }
            else
            {
                return null;
            }
        }
        [HttpPost]
        public JsonResult CreateQuotefromCart(List<QuoteMasterDTO> lstQuotes)
        {
            try
            {
                #region check dynamic validation rules
                if (lstQuotes != null && lstQuotes.Count > 0)
                {
                    string validationMsg = "";
                    int cnt = 0;
                    foreach (QuoteMasterDTO objQuoteMasterDTO in lstQuotes)
                    {
                        if (!string.IsNullOrWhiteSpace(objQuoteMasterDTO.RequiredDateString))
                        {
                            objQuoteMasterDTO.RequiredDate = DateTime.ParseExact(objQuoteMasterDTO.RequiredDateString, Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult);
                        }

                        var validationResultList = DTOCommonUtils.ValidateDTO<QuoteMasterDTO>(objQuoteMasterDTO, ControllerContext, new List<string>() { });
                        if (validationResultList.HasErrors())
                        {

                            if (!string.IsNullOrWhiteSpace(validationMsg))
                            {
                                validationMsg += "<br/><br/>";
                            }

                            string msg = validationResultList.GetShortErrorMessage(typeof(RequiredAttributeAdapter));
                            validationMsg += (string.Format(ResQuoteMaster.ValidationFailedFor, objQuoteMasterDTO.QuoteNumber) + "<br/>" + msg);
                            cnt++;
                        }

                        if (cnt >= 5)
                        {
                            // display message for 5 grid rows to shorten message
                            break;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(validationMsg))
                    {
                        return Json(new { Message = validationMsg, Status = "fail", lstQuotes = lstQuotes });
                    }
                }
                #endregion

                CartItemDAL objCartItemDAL = new CartItemDAL(SessionHelper.EnterPriseDBName);
                QuoteMasterDAL objQuoteMasterDAL = new QuoteMasterDAL(SessionHelper.EnterPriseDBName);
                bool IsSubmit = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowanquotetobeSubmitted);
                bool IsApprove = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowanquotetobeApproved);

                if (lstQuotes != null && lstQuotes.Count > 0)
                {
                    //---------------------- Check For Quote Number Duplication ----------------------
                    //
                    CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                    string columnList = "ID,RoomName,IsAllowQuoteDuplicate";
                    RoomDTO roomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");

                    if (roomDTO.IsAllowQuoteDuplicate != true)
                    {
                        var QuoteGroup = (from O in lstQuotes
                                          group O by new { QuoteNumberGB = O.QuoteNumber.Trim().ToLower() } into OGB
                                          select new
                                          {
                                              QuoteNumber = OGB.Key.QuoteNumberGB,
                                              TotalCount = OGB.Count()
                                          }).Where(x => x.TotalCount > 1).ToList();

                        if (QuoteGroup.Count > 0)
                        {
                            return Json(new { Message = string.Format(ResQuoteMaster.QuoteNumberDuplicateInList, QuoteGroup[0].QuoteNumber), Status = "fail", lstQuotes = lstQuotes });
                        }
                        var quoteNoExistMsg = ResQuoteMaster.QuoteNumberAlreadyExist;
                        foreach (QuoteMasterDTO objQuoteMasterDTO in lstQuotes)
                        {
                            if (objQuoteMasterDAL.IsQuoteNumberDuplicateById(objQuoteMasterDTO.QuoteNumber, objQuoteMasterDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID))
                            {
                                return Json(new { Message = string.Format(quoteNoExistMsg, objQuoteMasterDTO.QuoteNumber), Status = "fail", lstQuotes = lstQuotes });
                            }
                        }
                    }
                    eTurns.DAL.UserMasterDAL userDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                    ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                    ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                    string approvalErrorMsg = string.Empty;
                    string QuoteapprovalSuppErrorMsg = string.Empty;

                    double AllQuoteCost = 0;
                    double AllQuotePrice = 0;
                    foreach (QuoteMasterDTO objQuoteMasterDTO in lstQuotes)
                    {
                        if (!string.IsNullOrWhiteSpace(objQuoteMasterDTO.RequiredDateString))
                        {
                            objQuoteMasterDTO.RequiredDate = DateTime.ParseExact(objQuoteMasterDTO.RequiredDateString, Convert.ToString(SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult);
                        }
                        if (IsApprove)
                        {
                            double QuoteCost = 0;
                            double QuotePrice = 0;

                            if (objQuoteMasterDTO.QuoteStatus == (int)QuoteStatus.Approved || objQuoteMasterDTO.QuoteStatus == (int)QuoteStatus.Transmitted)
                            {
                                List<Guid> lstids = new List<Guid>();
                                if (!string.IsNullOrWhiteSpace(objQuoteMasterDTO.QuoteLineItemsIds))
                                {
                                    foreach (var item in objQuoteMasterDTO.QuoteLineItemsIds.Split(','))
                                    {
                                        Guid tempid = Guid.Empty;
                                        if (Guid.TryParse(item, out tempid))
                                            lstids.Add(tempid);
                                    }
                                    var tmpsupplierIds = new List<long>();
                                    List<CartItemDTO> lstCartItems = objCartItemDAL.GetCartItemsByGuids(lstids, SessionHelper.RoomID, SessionHelper.CompanyID, true, tmpsupplierIds);

                                    foreach (CartItemDTO cartitem in lstCartItems)
                                    {
                                        objItemMasterDTO = objItemMasterDAL.GetItemByGuidPlain((cartitem.ItemGUID ?? Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);

                                        CostUOMMasterDTO costUOM = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName).GetCostUOMByID(objItemMasterDTO.CostUOMID.GetValueOrDefault(0));
                                        if (costUOM == null)
                                            costUOM = new CostUOMMasterDTO() { CostUOMValue = 1 };

                                        if (objItemMasterDTO != null && cartitem.Quantity.GetValueOrDefault(0) > 0)
                                        {
                                            QuoteCost += (objItemMasterDTO.Cost.GetValueOrDefault(0) * cartitem.Quantity.GetValueOrDefault(0))
                                                     / (costUOM.CostUOMValue.GetValueOrDefault(0) > 0 ? costUOM.CostUOMValue.GetValueOrDefault(1) : 1);

                                            QuotePrice += (objItemMasterDTO.SellPrice.GetValueOrDefault(0) * cartitem.Quantity.GetValueOrDefault(0))
                                                   / (costUOM.CostUOMValue.GetValueOrDefault(0) > 0 ? costUOM.CostUOMValue.GetValueOrDefault(1) : 1);

                                            AllQuoteCost += QuoteCost;
                                            AllQuotePrice += QuotePrice;

                                            objQuoteMasterDTO.QuotePrice = objQuoteMasterDTO.QuotePrice.GetValueOrDefault(0) + QuotePrice;
                                            objQuoteMasterDTO.QuoteCost = objQuoteMasterDTO.QuoteCost.GetValueOrDefault(0) + QuoteCost;
                                        }

                                    }

                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(approvalErrorMsg))
                    {
                        return Json(new { Message = approvalErrorMsg, Status = "fail" }, JsonRequestBehavior.AllowGet);
                    }
                    //if (!string.IsNullOrEmpty(OrdapprovalSuppErrorMsg))
                    //{
                    //    return Json(new { Message = OrdapprovalSuppErrorMsg, Status = "fail" }, JsonRequestBehavior.AllowGet);
                    //}
                }

                Dictionary<string, string> rejectedquoteLineItems = new Dictionary<string, string>();
                Dictionary<string, string> rejectedquoteLineItemstest = new Dictionary<string, string>();
                long SessionUserId = SessionHelper.UserID;
                lstQuotes = objCartItemDAL.CreateQuotesByCart(lstQuotes, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriseDBName, 1, SessionHelper.EnterPriceID, out rejectedquoteLineItemstest, SessionUserId, "");
                QuoteController quoteController = new QuoteController();
                if (lstQuotes != null && lstQuotes.Count > 0)
                {
                    eTurns.DAL.UserMasterDAL objUserDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                    QuoteMasterDAL quoteDAL = new QuoteMasterDAL(SessionHelper.EnterPriseDBName);

                    foreach (QuoteMasterDTO Quoteitem in lstQuotes)
                    {
                        if (Quoteitem.QuoteStatus == (int)QuoteStatus.UnSubmitted)
                        {
                            quoteController.SetQuoteMailUnsubmitted(Quoteitem.ID);
                        }

                        if (Quoteitem.QuoteStatus == (int)QuoteStatus.Submitted)
                        {
                            quoteController.SendMailToApprovalAuthority(Quoteitem);
                        }

                        if (Quoteitem.QuoteStatus == (int)QuoteStatus.Approved || Quoteitem.QuoteStatus == (int)QuoteStatus.Transmitted)
                        {
                            quoteController.SendMailToSupplier(Quoteitem);
                            eTurns.DAL.UserMasterDAL userMasterDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);

                            eTurnsMaster.DAL.UserMasterDAL objReqRequesterUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL();
                            UserMasterDTO ReqUser = new UserMasterDTO();

                            string OrdRequesterEmailAddress = "";
                            string OrdApproverEmailAddress = "";
                            if (Quoteitem.RequesterID.GetValueOrDefault(0) > 0)
                            {
                                ReqUser = objReqRequesterUserMasterDAL.GetUserByIdPlain(Quoteitem.RequesterID.GetValueOrDefault(0));
                                if (ReqUser == null)
                                {
                                    ReqUser = userMasterDAL.GetUserByIdPlain(Quoteitem.RequesterID.GetValueOrDefault(0));
                                }
                                if (ReqUser != null && !string.IsNullOrWhiteSpace(ReqUser.Email))
                                {
                                    OrdRequesterEmailAddress = ReqUser.Email;
                                }
                            }
                            if (Quoteitem.ApproverID.GetValueOrDefault(0) > 0)
                            {
                                ReqUser = objReqRequesterUserMasterDAL.GetUserByIdPlain(Quoteitem.ApproverID.GetValueOrDefault(0));
                                if (ReqUser == null)
                                {
                                    ReqUser = userMasterDAL.GetUserByIdPlain(Quoteitem.ApproverID.GetValueOrDefault(0));
                                }
                                if (ReqUser != null && !string.IsNullOrWhiteSpace(ReqUser.Email))
                                {
                                    OrdApproverEmailAddress = ReqUser.Email;
                                }
                            }
                            quoteController.SendMailForQuoteApprovedOrReject(Quoteitem, "approved", OrdRequesterEmailAddress, OrdApproverEmailAddress);
                        }
                    }
                }
                string message = string.Empty;
                string status = string.Empty;
                if (!rejectedquoteLineItems.Any())
                {
                    message = ResMessage.SaveMessage;
                    status = "ok";
                }
                else
                {
                    message = ResQuoteMaster.BelowGivenItemsRejected;
                    var itemNamberResource = ResItemMaster.ItemNumber;
                    var reasonResource = ResCommon.Reason;
                    foreach (KeyValuePair<string, string> entry in rejectedquoteLineItems)
                    {
                        message += (itemNamberResource + ": " + entry.Key + " ," + reasonResource + ": " + entry.Value);
                    }

                    status = "fail";
                }

                GlobalHost.ConnectionManager.GetHubContext<eTurnsHub>().Clients.Group(SessionHelper.EnterPriceID + "_" + SessionHelper.CompanyID + "_" + SessionHelper.RoomID).UpdateRedCircleCountInClients();
                return Json(new { Message = message, Status = status, lstQuotes = lstQuotes });
            }
            catch
            {
                string message = ResMessage.SaveErrorMsg;
                string status = "fail";
                return Json(new { Message = message, Status = status, lstQuotes = lstQuotes });
            }
        }

        public JsonResult GetSupplierData()
        {
            string strSupplierIds = string.Empty;
            if (SessionHelper.UserSupplierIds != null && SessionHelper.UserSupplierIds.Any())
            {
                strSupplierIds = string.Join(",", SessionHelper.UserSupplierIds);
            }
            SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            List<SupplierMasterDTO> lstSuppliers = new List<SupplierMasterDTO>();
            lstSuppliers = objSupplierMasterDAL.GetNonDeletedSupplierByIDsNormal(strSupplierIds, SessionHelper.RoomID, SessionHelper.CompanyID);


            return Json(new { DDData = lstSuppliers }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getQuoteNumberForSupplier(long SupplierID)
        {
            CartItemDAL cartItemDAL = new CartItemDAL(SessionHelper.EnterPriseDBName);
            var QuoteAutonumberDetails = cartItemDAL.getQuoteNumber(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SupplierID);
            return Json(new { DDData = QuoteAutonumberDetails }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
