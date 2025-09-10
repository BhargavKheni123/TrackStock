using eTurns.DAL;
using eTurns.DTO;
using eTurnsWeb.Helper;
using eTurnsWeb.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;


namespace eTurnsWeb.Controllers
{
    public class SupplierCatalogController : eTurnsControllerBase
    {
        public ActionResult SupplierCatalogLanding()
        {
            return View();
        }

        public ActionResult GetSupplierCatalog(string PageName, bool? Openpopup, string Buttonname, string OrderGUID)
        {
            SupplierCatalogItemDTO objSupplierCatalogItemDTO = new SupplierCatalogItemDTO();
            objSupplierCatalogItemDTO.DestinationModule = PageName;
            objSupplierCatalogItemDTO.SourcePageName = PageName;
            objSupplierCatalogItemDTO.OpenPopup = Openpopup ?? false;
            objSupplierCatalogItemDTO.ButtonText = Buttonname;
            if (!string.IsNullOrEmpty(OrderGUID))
            {
                objSupplierCatalogItemDTO.OrderGUID = Guid.Parse(OrderGUID);
            }
            return PartialView("SupplierCatalog", objSupplierCatalogItemDTO);
        }

        [HttpPost]
        public JsonResult AddItemFromCatalog(SupplierCatalogItemDTO objSupplierCatalogItemDTO)
        {
            ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
            CatalogItemMasterDAL objCatalogItemMasterDAL = new CatalogItemMasterDAL(SessionHelper.EnterPriseDBName);
            objItemMasterDTO = objCatalogItemMasterDAL.GetItemByItemName(objSupplierCatalogItemDTO.ItemNumber, SessionHelper.CompanyID, SessionHelper.RoomID);
            SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            SupplierMasterDTO objSupplierMasterDTO = objSupplierMasterDAL.GetSupplierByNamePlain(SessionHelper.RoomID, SessionHelper.CompanyID, false, objSupplierCatalogItemDTO.SupplierName);
            long SessionUserId = SessionHelper.UserID;
            if (objSupplierMasterDTO == null)
            {
                objSupplierMasterDTO = new SupplierMasterDTO();
                objSupplierMasterDTO.CompanyID = SessionHelper.CompanyID;
                objSupplierMasterDTO.Created = DateTimeUtility.DateTimeNow;
                objSupplierMasterDTO.CreatedBy = SessionHelper.UserID;
                objSupplierMasterDTO.CreatedByName = SessionHelper.UserName;
                objSupplierMasterDTO.IsArchived = false;
                objSupplierMasterDTO.IsDeleted = false;
                objSupplierMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                objSupplierMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                objSupplierMasterDTO.Room = SessionHelper.RoomID;
                objSupplierMasterDTO.RoomName = SessionHelper.RoomName;
                objSupplierMasterDTO.SupplierName = objSupplierCatalogItemDTO.SupplierName;
                objSupplierMasterDTO.UpdatedByName = SessionHelper.UserName;
                objSupplierMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                objSupplierMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                objSupplierMasterDTO.EditedFrom = "Web";
                objSupplierMasterDTO.AddedFrom = "Web";
                objSupplierMasterDTO.ID = objSupplierMasterDAL.Insert(objSupplierMasterDTO);


            }
            if (objSupplierMasterDTO != null)
            {
                objSupplierCatalogItemDTO.SupplierID = objSupplierMasterDTO.ID;
            }

            ManufacturerMasterDAL objManufacturerMasterDAL = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName);
            ManufacturerMasterDTO objManufacturerMasterDTO = objManufacturerMasterDAL.GetManufacturerByNameNormal(objSupplierCatalogItemDTO.ManufacturerName, SessionHelper.RoomID, SessionHelper.CompanyID, false);

            if (objManufacturerMasterDTO == null)
            {
                objManufacturerMasterDTO = new ManufacturerMasterDTO();
                objManufacturerMasterDTO.CompanyID = SessionHelper.CompanyID;
                objManufacturerMasterDTO.Created = DateTimeUtility.DateTimeNow;
                objManufacturerMasterDTO.CreatedBy = SessionHelper.UserID;
                objManufacturerMasterDTO.CreatedByName = SessionHelper.UserName;
                objManufacturerMasterDTO.IsArchived = false;
                objManufacturerMasterDTO.IsDeleted = false;
                objManufacturerMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                objManufacturerMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                objManufacturerMasterDTO.Room = SessionHelper.RoomID;
                objManufacturerMasterDTO.RoomName = SessionHelper.RoomName;
                objManufacturerMasterDTO.Manufacturer = objSupplierCatalogItemDTO.ManufacturerName;
                objManufacturerMasterDTO.UpdatedByName = SessionHelper.UserName;
                objManufacturerMasterDTO.AddedFrom = "Web";
                objManufacturerMasterDTO.EditedFrom = "Web";
                objManufacturerMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                objManufacturerMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                objManufacturerMasterDTO.ID = objManufacturerMasterDAL.Insert(objManufacturerMasterDTO);
            }
            if (objManufacturerMasterDTO != null)
            {
                objSupplierCatalogItemDTO.ManufacturerID = objManufacturerMasterDTO.ID;
            }

            if (objItemMasterDTO != null)
            {
                switch (objSupplierCatalogItemDTO.DestinationModule)
                {
                    case "ItemMaster":
                        ItemSupplierDetailsDAL objItemSupplierDetailsDAL = new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName);
                        Guid ItemGuid = Guid.Empty;
                        Guid.TryParse(objItemMasterDTO.GUID.ToString(), out ItemGuid);
                        ItemSupplierDetailsDTO objItemSupplierDetailsDTO = objItemSupplierDetailsDAL.GetSupplierByItemGuidBySupplierID(SessionHelper.RoomID, SessionHelper.CompanyID, ItemGuid, objSupplierCatalogItemDTO.SupplierID);
                        if (objItemSupplierDetailsDTO == null)
                        {
                            objItemSupplierDetailsDTO = new ItemSupplierDetailsDTO();
                            objItemSupplierDetailsDTO.CompanyID = SessionHelper.CompanyID;
                            objItemSupplierDetailsDTO.Created = DateTimeUtility.DateTimeNow;
                            objItemSupplierDetailsDTO.CreatedBy = SessionHelper.UserID;
                            objItemSupplierDetailsDTO.CreatedByName = SessionHelper.UserName;
                            objItemSupplierDetailsDTO.ID = 0;
                            objItemSupplierDetailsDTO.IsArchived = false;
                            objItemSupplierDetailsDTO.IsDeleted = false;
                            objItemSupplierDetailsDTO.ItemGUID = objItemMasterDTO.GUID;
                            objItemSupplierDetailsDTO.LastUpdatedBy = SessionHelper.UserID;
                            objItemSupplierDetailsDTO.Room = SessionHelper.RoomID;
                            objItemSupplierDetailsDTO.RoomName = SessionHelper.RoomName;
                            objItemSupplierDetailsDTO.SupplierID = objSupplierCatalogItemDTO.SupplierID;
                            objItemSupplierDetailsDTO.SupplierName = objSupplierCatalogItemDTO.SupplierName;
                            objItemSupplierDetailsDTO.SupplierNumber = objSupplierCatalogItemDTO.SupplierPartNumber;
                            objItemSupplierDetailsDTO.Updated = DateTimeUtility.DateTimeNow;
                            objItemSupplierDetailsDTO.UpdatedByName = SessionHelper.UserName;
                            objItemSupplierDetailsDTO.EditedFrom = "Web";
                            objItemSupplierDetailsDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objItemSupplierDetailsDTO.AddedFrom = "Web";
                            objItemSupplierDetailsDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            objItemSupplierDetailsDTO.ID = objItemSupplierDetailsDAL.Insert(objItemSupplierDetailsDTO);
                        }
                        ItemManufacturerDetailsDAL objItemManufacturerDetailsDAL = new ItemManufacturerDetailsDAL(SessionHelper.EnterPriseDBName);

                        ItemManufacturerDetailsDTO objItemManufacturerDetailsDTO = objItemManufacturerDetailsDAL.GetManufacturerByItemGuidByManufacturerID(SessionHelper.RoomID, SessionHelper.CompanyID, ItemGuid, false, objSupplierCatalogItemDTO.ManufacturerID);
                        if (objItemManufacturerDetailsDTO == null)
                        {
                            objItemManufacturerDetailsDTO = new ItemManufacturerDetailsDTO();
                            objItemManufacturerDetailsDTO.CompanyID = SessionHelper.CompanyID;
                            objItemManufacturerDetailsDTO.Created = DateTimeUtility.DateTimeNow;
                            objItemManufacturerDetailsDTO.CreatedBy = SessionHelper.UserID;
                            objItemManufacturerDetailsDTO.CreatedByName = SessionHelper.UserName;
                            objItemManufacturerDetailsDTO.ID = 0;
                            objItemManufacturerDetailsDTO.IsArchived = false;
                            objItemManufacturerDetailsDTO.IsDeleted = false;
                            objItemManufacturerDetailsDTO.ItemGUID = objItemMasterDTO.GUID;
                            objItemManufacturerDetailsDTO.LastUpdatedBy = SessionHelper.UserID;
                            objItemManufacturerDetailsDTO.Room = SessionHelper.RoomID;
                            objItemManufacturerDetailsDTO.RoomName = SessionHelper.RoomName;
                            objItemManufacturerDetailsDTO.ManufacturerID = objSupplierCatalogItemDTO.ManufacturerID.GetValueOrDefault(0);
                            objItemManufacturerDetailsDTO.ManufacturerName = objSupplierCatalogItemDTO.ManufacturerName;
                            objItemManufacturerDetailsDTO.ManufacturerNumber = objSupplierCatalogItemDTO.ManufacturerPartNumber;
                            objItemManufacturerDetailsDTO.Updated = DateTimeUtility.DateTimeNow;
                            objItemManufacturerDetailsDTO.UpdatedByName = SessionHelper.UserName;
                            objItemManufacturerDetailsDTO.EditedFrom = "Web";
                            objItemManufacturerDetailsDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objItemManufacturerDetailsDTO.AddedFrom = "Web";
                            objItemManufacturerDetailsDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            objItemManufacturerDetailsDTO.ID = objItemManufacturerDetailsDAL.Insert(objItemManufacturerDetailsDTO);
                        }
                        return Json(new
                        {
                            IsItemExist = true,
                            ItemID = objItemMasterDTO.ID,
                            SupplierCatalogItem = objSupplierCatalogItemDTO
                        });
                    case "CartItemMaster":
                        CartItemDTO objCartItemDTO = new CartItemDTO();
                        objCartItemDTO.Created = DateTimeUtility.DateTimeNow;
                        objCartItemDTO.CreatedByName = SessionHelper.UserName;
                        objCartItemDTO.CreatedBy = SessionHelper.UserID;
                        objCartItemDTO.Updated = DateTimeUtility.DateTimeNow;
                        objCartItemDTO.UpdatedByName = SessionHelper.UserName;
                        objCartItemDTO.LastUpdatedBy = SessionHelper.UserID;
                        objCartItemDTO.ID = 0;
                        objCartItemDTO.ItemNumber = objItemMasterDTO.ItemNumber;
                        objCartItemDTO.UDF1 = "";
                        objCartItemDTO.UDF2 = "";
                        objCartItemDTO.UDF3 = "";
                        objCartItemDTO.UDF4 = "";
                        objCartItemDTO.UDF5 = "";
                        objCartItemDTO.ReplenishType = "";
                        objCartItemDTO.ItemGUID = objItemMasterDTO.GUID;
                        objCartItemDTO.CompanyID = SessionHelper.CompanyID;
                        objCartItemDTO.Room = SessionHelper.RoomID;
                        objCartItemDTO.Quantity = objSupplierCatalogItemDTO.Quantity;
                        objCartItemDTO.IsDeleted = false;
                        objCartItemDTO.IsArchived = false;
                        objCartItemDTO.WhatWhereAction = "SupplierCatelog";
                        new CartItemDAL(SessionHelper.EnterPriseDBName).SaveCartItem(objCartItemDTO, SessionUserId, SessionHelper.EnterPriceID);
                        return Json(new
                        {
                            IsItemExist = true,
                            ItemID = 0,
                            SupplierCatalogItem = objSupplierCatalogItemDTO
                        });
                    case "OrderMaster":

                        OrderDetailsDAL objOrderDetailsDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                        List<OrderDetailsDTO> lstOrderItems = objOrderDetailsDAL.GetDeletedOrUnDeletedOrderDetailByOrderGUIDPlain(objSupplierCatalogItemDTO.OrderGUID, SessionHelper.RoomID, SessionHelper.CompanyID, false).ToList();
                        if (!lstOrderItems.Any(t => t.ItemGUID == objItemMasterDTO.GUID))
                        {
                            OrderDetailsDTO objOrderDetailsDTO = new OrderDetailsDTO();
                            objOrderDetailsDTO.Room = SessionHelper.RoomID;
                            objOrderDetailsDTO.RoomName = SessionHelper.RoomName;
                            objOrderDetailsDTO.CreatedBy = SessionHelper.UserID;
                            objOrderDetailsDTO.CreatedByName = SessionHelper.UserName;
                            objOrderDetailsDTO.UpdatedByName = SessionHelper.UserName;
                            objOrderDetailsDTO.LastUpdatedBy = SessionHelper.UserID;
                            objOrderDetailsDTO.CompanyID = SessionHelper.CompanyID;
                            objOrderDetailsDTO.OrderGUID = objSupplierCatalogItemDTO.OrderGUID;
                            objOrderDetailsDTO.ItemGUID = objItemMasterDTO.GUID;
                            objOrderDetailsDTO.ItemGUID = objItemMasterDTO.GUID;
                            //objOrderDetailsDTO.ItemDetail = objItemMasterDTO;
                            objOrderDetailsDTO.RequestedQuantity = objSupplierCatalogItemDTO.Quantity;
                            objOrderDetailsDTO.AddedFrom = "Web";
                            objOrderDetailsDTO.EditedFrom = "Web";
                            objOrderDetailsDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objOrderDetailsDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            objOrderDetailsDAL.Insert(objOrderDetailsDTO, SessionUserId, SessionHelper.EnterPriceID);
                        }
                        return Json(new
                        {
                            IsItemExist = true,
                            ItemID = 0,
                            SupplierCatalogItem = objSupplierCatalogItemDTO
                        });
                    default:
                        return Json(new
                        {
                            IsItemExist = true,
                            ItemID = 0,
                            SupplierCatalogItem = objSupplierCatalogItemDTO
                        });


                }

            }
            else
            {
                switch (objSupplierCatalogItemDTO.DestinationModule)
                {
                    case "ItemMaster":
                        return Json(new
                        {
                            IsItemExist = false,
                            ItemID = 0,
                            SupplierCatalogItem = objSupplierCatalogItemDTO
                        });
                    case "CartItemMaster":
                        return Json(new
                        {
                            IsItemExist = false,
                            ItemID = 0,
                            SupplierCatalogItem = objSupplierCatalogItemDTO
                        });
                    case "OrderMaster":
                        return Json(new
                        {
                            IsItemExist = false,
                            ItemID = 0,
                            SupplierCatalogItem = objSupplierCatalogItemDTO
                        });
                    default:
                        return Json(new
                        {
                            IsItemExist = false,
                            ItemID = 0,
                            SupplierCatalogItem = objSupplierCatalogItemDTO
                        });
                }

            }


        }

        [HttpPost]
        public JsonResult AddItemFromSupplierCatalog(SupplierCatalogItemDTO objSupplierCatalogItemDTO)
        {
            ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
            CatalogItemMasterDAL objCatalogItemMasterDAL = new CatalogItemMasterDAL(SessionHelper.EnterPriseDBName);
            objItemMasterDTO = objCatalogItemMasterDAL.GetItemByItemName(objSupplierCatalogItemDTO.ItemNumber, SessionHelper.CompanyID, SessionHelper.RoomID);
            SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            SupplierMasterDTO objSupplierMasterDTO = objSupplierMasterDAL.GetSupplierByNamePlain(SessionHelper.RoomID, SessionHelper.CompanyID, false, objSupplierCatalogItemDTO.SupplierName);


            RoomDAL objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
            //  RoomDTO objRoomDTO = objRoomDAL.GetRoomByIDPlain(SessionHelper.RoomID);
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            string columnList = "ID,RoomName,ReplenishmentType,DefaultBinName";
            RoomDTO objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");

            if (objSupplierMasterDTO == null)
            {
                objSupplierMasterDTO = new SupplierMasterDTO();
                objSupplierMasterDTO.CompanyID = SessionHelper.CompanyID;
                objSupplierMasterDTO.Created = DateTimeUtility.DateTimeNow;
                objSupplierMasterDTO.CreatedBy = SessionHelper.UserID;
                objSupplierMasterDTO.CreatedByName = SessionHelper.UserName;
                objSupplierMasterDTO.IsArchived = false;
                objSupplierMasterDTO.IsDeleted = false;
                objSupplierMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                objSupplierMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                objSupplierMasterDTO.Room = SessionHelper.RoomID;
                objSupplierMasterDTO.RoomName = SessionHelper.RoomName;
                objSupplierMasterDTO.SupplierName = objSupplierCatalogItemDTO.SupplierName;
                objSupplierMasterDTO.UpdatedByName = SessionHelper.UserName;
                objSupplierMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                objSupplierMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                objSupplierMasterDTO.EditedFrom = "Web";
                objSupplierMasterDTO.AddedFrom = "Web";
                objSupplierMasterDTO.ID = objSupplierMasterDAL.Insert(objSupplierMasterDTO);


            }
            if (objSupplierMasterDTO != null)
            {
                objSupplierCatalogItemDTO.SupplierID = objSupplierMasterDTO.ID;
            }

            UnitMasterDAL objUnitDAL = new UnitMasterDAL(SessionHelper.EnterPriseDBName);
            UnitMasterDTO objUnitDTO = objUnitDAL.GetUnitByNamePlain(SessionHelper.RoomID, SessionHelper.CompanyID, false, objSupplierCatalogItemDTO.UOM);
            if (objUnitDTO == null)
            {
                objUnitDTO = new UnitMasterDTO();
                objUnitDTO.IsDeleted = false;
                objUnitDTO.IsArchived = false;
                objUnitDTO.CreatedBy = SessionHelper.UserID;
                objUnitDTO.LastUpdatedBy = SessionHelper.UserID;
                objUnitDTO.Room = SessionHelper.RoomID;
                objUnitDTO.Unit = objSupplierCatalogItemDTO.UOM;
                objUnitDTO.CompanyID = SessionHelper.CompanyID;
                objUnitDTO.isForBOM = false;
                objUnitDTO.CreatedByName = SessionHelper.UserName;
                objUnitDTO.AddedFrom = "Web";
                objUnitDTO.EditedFrom = "Web";
                objUnitDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                objUnitDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                objUnitDTO.ID = objUnitDAL.Insert(objUnitDTO);
            }

            CostUOMMasterDAL objCostUOMDAL = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName);
            //CostUOMMasterDTO objCostUOMDTO = objCostUOMDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).FirstOrDefault(t => t.CostUOM == objSupplierCatalogItemDTO.CostUOM);
            CostUOMMasterDTO objCostUOMDTO = objCostUOMDAL.GetCostUOMByName(objSupplierCatalogItemDTO.CostUOM, SessionHelper.RoomID, SessionHelper.CompanyID);
            if (objCostUOMDTO == null)
            {
                objCostUOMDTO = new CostUOMMasterDTO();
                objCostUOMDTO.CostUOM = objSupplierCatalogItemDTO.CostUOM;
                objCostUOMDTO.CostUOMValue = 0;
                objCostUOMDTO.CreatedBy = SessionHelper.UserID;
                objCostUOMDTO.IsDeleted = false;
                objCostUOMDTO.IsDeleted = false;
                objCostUOMDTO.IsArchived = false;
                objCostUOMDTO.LastUpdatedBy = SessionHelper.UserID;
                objCostUOMDTO.Room = SessionHelper.RoomID;
                objCostUOMDTO.CompanyID = SessionHelper.CompanyID;
                objCostUOMDTO.CreatedByName = SessionHelper.UserName;
                objCostUOMDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                objCostUOMDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                objCostUOMDTO.AddedFrom = "Web";
                objCostUOMDTO.EditedFrom = "Web";
                objCostUOMDTO.isForBOM = false;
                objCostUOMDTO.ID = objCostUOMDAL.Insert(objCostUOMDTO);
            }

            ManufacturerMasterDAL objManufacturerMasterDAL = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName);
            ManufacturerMasterDTO objManufacturerMasterDTO = new ManufacturerMasterDTO();

            if (!string.IsNullOrEmpty(objSupplierCatalogItemDTO.ManufacturerName) && !string.IsNullOrWhiteSpace(objSupplierCatalogItemDTO.ManufacturerName))
            {
                objManufacturerMasterDTO = objManufacturerMasterDAL.GetManufacturerByNameNormal(objSupplierCatalogItemDTO.ManufacturerName, SessionHelper.RoomID, SessionHelper.CompanyID, false);

                if (objManufacturerMasterDTO == null)
                {
                    objManufacturerMasterDTO = new ManufacturerMasterDTO();
                    objManufacturerMasterDTO.CompanyID = SessionHelper.CompanyID;
                    objManufacturerMasterDTO.Created = DateTimeUtility.DateTimeNow;
                    objManufacturerMasterDTO.CreatedBy = SessionHelper.UserID;
                    objManufacturerMasterDTO.CreatedByName = SessionHelper.UserName;
                    objManufacturerMasterDTO.IsArchived = false;
                    objManufacturerMasterDTO.IsDeleted = false;
                    objManufacturerMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                    objManufacturerMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                    objManufacturerMasterDTO.Room = SessionHelper.RoomID;
                    objManufacturerMasterDTO.RoomName = SessionHelper.RoomName;
                    objManufacturerMasterDTO.Manufacturer = objSupplierCatalogItemDTO.ManufacturerName;
                    objManufacturerMasterDTO.UpdatedByName = SessionHelper.UserName;
                    objManufacturerMasterDTO.AddedFrom = "Web";
                    objManufacturerMasterDTO.EditedFrom = "Web";
                    objManufacturerMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objManufacturerMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objManufacturerMasterDTO.ID = objManufacturerMasterDAL.Insert(objManufacturerMasterDTO);
                }

                if (objManufacturerMasterDTO != null)
                {
                    objSupplierCatalogItemDTO.ManufacturerID = objManufacturerMasterDTO.ID;
                }
            }
            CategoryMasterDAL objCategoryMasterDAL = new CategoryMasterDAL(SessionHelper.EnterPriseDBName);
            CategoryMasterDTO objCategoryMasterDTO = new CategoryMasterDTO();

            if (!string.IsNullOrEmpty(objSupplierCatalogItemDTO.Category) && !string.IsNullOrWhiteSpace(objSupplierCatalogItemDTO.Category))
            {
                objCategoryMasterDTO = objCategoryMasterDAL.GetSingleCategoryByNameByRoomID(objSupplierCatalogItemDTO.Category, SessionHelper.RoomID, SessionHelper.CompanyID);

                if (objCategoryMasterDTO == null) // new entry - not matching id and uom
                {
                    objCategoryMasterDTO = new CategoryMasterDTO
                    {
                        ID = 0,
                        CompanyID = SessionHelper.CompanyID,
                        Created = DateTimeUtility.DateTimeNow,
                        CreatedBy = SessionHelper.UserID,
                        CreatedByName = SessionHelper.UserName,
                        IsArchived = false,
                        IsDeleted = false,
                        LastUpdatedBy = SessionHelper.UserID,
                        Room = SessionHelper.RoomID,
                        Updated = DateTimeUtility.DateTimeNow,
                        UpdatedByName = SessionHelper.UserName,
                        Category = objSupplierCatalogItemDTO.Category,
                        ReceivedOn = DateTimeUtility.DateTimeNow,
                        ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                        isForBOM = false,
                        AddedFrom = "Web",
                        EditedFrom = "Web"
                    };
                    objCategoryMasterDTO.ID = objCategoryMasterDAL.Insert(objCategoryMasterDTO);
                    objSupplierCatalogItemDTO.CategoryID = objCategoryMasterDTO.ID;
                }
                if (objSupplierCatalogItemDTO != null)
                {
                    objSupplierCatalogItemDTO.CategoryID = objCategoryMasterDTO.ID;
                }
            }
            if (objItemMasterDTO == null)
            {
                ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                // add records in item master
                if (objItemMasterDTO == null)
                {

                    objItemMasterDTO = new ItemMasterDTO();
                    objItemMasterDTO.ItemNumber = objSupplierCatalogItemDTO.ItemNumber;

                    if (objManufacturerMasterDTO != null && objManufacturerMasterDTO.ID > 0)
                    {
                        objItemMasterDTO.ManufacturerID = objManufacturerMasterDTO.ID;
                    }

                    objItemMasterDTO.ManufacturerNumber = objSupplierCatalogItemDTO.ManufacturerPartNumber;
                    objItemMasterDTO.SupplierID = objSupplierMasterDTO.ID;

                    if (string.IsNullOrEmpty(objSupplierCatalogItemDTO.SupplierPartNumber) || string.IsNullOrWhiteSpace(objSupplierCatalogItemDTO.SupplierPartNumber))
                    {
                        objSupplierCatalogItemDTO.SupplierPartNumber = objSupplierCatalogItemDTO.ItemNumber;
                    }

                    objItemMasterDTO.SupplierPartNo = objSupplierCatalogItemDTO.SupplierPartNumber;



                    objItemMasterDTO.UPC = objSupplierCatalogItemDTO.UPC;
                    objItemMasterDTO.Description = objSupplierCatalogItemDTO.Description;
                    objItemMasterDTO.ImagePath = objSupplierCatalogItemDTO.ImagePath;
                    objItemMasterDTO.PackingQuantity = objSupplierCatalogItemDTO.PackingQantity;
                    //objItemMasterDTO.Cost = objSupplierCatalogItemDTO.Cost;
                    objItemMasterDTO.UOMID = objUnitDTO.ID;
                    objItemMasterDTO.CostUOMID = objCostUOMDTO.ID;
                    objItemMasterDTO.DefaultPullQuantity = 1;
                    objItemMasterDTO.DefaultReorderQuantity = 1;
                    objItemMasterDTO.IsPurchase = true;
                    objItemMasterDTO.IsActive = true;
                    objItemMasterDTO.IsOrderable = true;
                    objItemMasterDTO.ItemType = 1;
                    if (objRoomDTO.ReplenishmentType != "2")
                        objItemMasterDTO.IsItemLevelMinMaxQtyRequired = true;
                    else
                        objItemMasterDTO.IsItemLevelMinMaxQtyRequired = false;

                    objItemMasterDTO.MinimumQuantity = 0;
                    objItemMasterDTO.MaximumQuantity = 0;
                    objItemMasterDTO.CriticalQuantity = 0;
                    objItemMasterDTO.Room = SessionHelper.RoomID;
                    objItemMasterDTO.CompanyID = SessionHelper.CompanyID;
                    objItemMasterDTO.Created = DateTimeUtility.DateTimeNow;
                    objItemMasterDTO.CreatedBy = SessionHelper.UserID;
                    objItemMasterDTO.LastUpdatedBy = SessionHelper.UserID;

                    objItemMasterDTO.CreatedByName = SessionHelper.UserName;
                    objItemMasterDTO.IsArchived = false;
                    objItemMasterDTO.IsDeleted = false;
                    objItemMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                    objItemMasterDTO.UpdatedByName = SessionHelper.UserName;
                    objItemMasterDTO.AddedFrom = "Web";
                    objItemMasterDTO.EditedFrom = "Web";
                    objItemMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objItemMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;

                    if (!string.IsNullOrEmpty(objSupplierCatalogItemDTO.ImagePath) && !string.IsNullOrWhiteSpace(objSupplierCatalogItemDTO.ImagePath))
                    {
                        objItemMasterDTO.ImageType = "ImagePath";
                    }

                    objItemMasterDTO.Cost = objSupplierCatalogItemDTO.Cost ?? 0;
                    objItemMasterDTO.SellPrice = objSupplierCatalogItemDTO.SellPrice ?? 0;
                    objItemMasterDTO.Markup = 0;

                    objItemMasterDTO.UNSPSC = objSupplierCatalogItemDTO.UNSPSC;
                    objItemMasterDTO.CategoryID = objSupplierCatalogItemDTO.CategoryID;
                    objItemMasterDTO.LongDescription = objSupplierCatalogItemDTO.LongDescription;

                    string columns = "ID,RoomName,IsAllowOrderCostuom,GlobMarkupParts,GlobMarkupLabor";
                    RoomDTO currentRoom = objCommonDAL.GetSingleRecord<RoomDTO>(columns, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");

                    if (currentRoom == null || currentRoom.ID <= 0)
                    {
                        currentRoom = new RoomDTO();
                    }

                    if (objItemMasterDTO.Cost != 0 && objItemMasterDTO.SellPrice != 0 && objItemMasterDTO.Markup != 0)
                    {
                        objItemMasterDTO.Markup = Convert.ToDouble(((Convert.ToDecimal(objItemMasterDTO.SellPrice) * Convert.ToDecimal(100)) / Convert.ToDecimal(objItemMasterDTO.Cost)) - Convert.ToDecimal(100));
                        // Calculate MArkup based on price and cost
                    }
                    else if (objItemMasterDTO.Cost != 0 && objItemMasterDTO.SellPrice == 0 && objItemMasterDTO.Markup == 0)
                    {
                        // Prise will become same as cost
                        objItemMasterDTO.SellPrice = objItemMasterDTO.Cost;
                    }
                    else if (objItemMasterDTO.Cost != 0 && objItemMasterDTO.SellPrice != 0 && objItemMasterDTO.Markup == 0)
                    {
                        // Calculate MArkup based on price and cost
                        objItemMasterDTO.Markup = Convert.ToDouble(((Convert.ToDecimal(objItemMasterDTO.SellPrice) * Convert.ToDecimal(100)) / Convert.ToDecimal(objItemMasterDTO.Cost)) - Convert.ToDecimal(100));
                    }
                    else if (objItemMasterDTO.Cost != 0 && objItemMasterDTO.SellPrice == 0 && objItemMasterDTO.Markup != 0)
                    {
                        //Calculate prise based on cost and markup
                        objItemMasterDTO.SellPrice = Convert.ToDouble(Convert.ToDecimal(objItemMasterDTO.Cost) + ((Convert.ToDecimal(objItemMasterDTO.Cost) * Convert.ToDecimal(objItemMasterDTO.Markup)) / Convert.ToDecimal(100)));
                    }
                    else if (objItemMasterDTO.Cost == 0 && objItemMasterDTO.SellPrice != 0 && objItemMasterDTO.Markup != 0)
                    {
                        // Calculate cost based on prise and markup                                 
                        //objItemMasterDTO.Cost = objItemMasterDTO.SellPrice - ((objItemMasterDTO.SellPrice * objItemMasterDTO.Markup) / 100);
                        objItemMasterDTO.Cost = Convert.ToDouble((Convert.ToDecimal(100) * Convert.ToDecimal(objItemMasterDTO.SellPrice)) / (Convert.ToDecimal(objItemMasterDTO.Markup) + Convert.ToDecimal(100)));
                        //Please follow Markup= (((sell-cost)/cost)*100)
                    }
                    else if (objItemMasterDTO.Cost == 0 && objItemMasterDTO.SellPrice == 0 && objItemMasterDTO.Markup == 0)
                    {
                        // All are zero so no calc
                    }
                    else if (objItemMasterDTO.Cost == 0 && objItemMasterDTO.SellPrice != 0 && objItemMasterDTO.Markup == 0)
                    {
                        // cost will become same as prise
                        objItemMasterDTO.Cost = objItemMasterDTO.SellPrice;
                    }
                    else if (objItemMasterDTO.Cost == 0 && objItemMasterDTO.SellPrice == 0 && objItemMasterDTO.Markup != 0)
                    {
                        bool isSetMarkupZero = true;
                        if (currentRoom != null)
                        {
                            if (currentRoom.GlobMarkupParts > 0)
                            {
                                isSetMarkupZero = false;
                            }
                        }
                        if (isSetMarkupZero == false)
                        {
                            objItemMasterDTO.Markup = objItemMasterDTO.Markup;
                        }
                        else
                            objItemMasterDTO.Markup = 0;
                        // Cost and prise will remain zero and save markup only Or make markup zero because no prise and cost
                    }

                    objItemMasterDTO.ID = objItemMasterDAL.Insert(objItemMasterDTO);

                    if (!string.IsNullOrEmpty(objSupplierCatalogItemDTO.ImagePath) && !string.IsNullOrWhiteSpace(objSupplierCatalogItemDTO.ImagePath) && objItemMasterDTO.ID > 0 && objSupplierCatalogItemDTO.SupplierCatalogItemID > 0)
                    {
                        var uploadFor = SiteSettingHelper.InventoryPhoto;
                        var destinationDirectory = System.Web.HttpContext.Current.Server.MapPath(uploadFor + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID.ToString() + "/" + SessionHelper.RoomID.ToString() + "/" + objItemMasterDTO.ID.ToString());
                        var destinationPath = System.Web.HttpContext.Current.Server.MapPath(uploadFor + SessionHelper.EnterPriceID.ToString() + "/" + SessionHelper.CompanyID.ToString() + "/" + SessionHelper.RoomID.ToString() + "/" + objItemMasterDTO.ID.ToString() + "/" + objItemMasterDTO.ImagePath);
                        var uploadForSupplierCatalog = "~/Uploads/CatalogItemImage/";
                        var sourcePath = System.Web.HttpContext.Current.Server.MapPath(uploadForSupplierCatalog + SessionHelper.EnterPriceID.ToString() + "/" + objSupplierCatalogItemDTO.SupplierCatalogItemID.ToString() + "/" + objItemMasterDTO.ImagePath);

                        if (!Directory.Exists(destinationDirectory))
                        {
                            Directory.CreateDirectory(destinationDirectory);
                        }
                        if (System.IO.File.Exists(sourcePath))
                        {
                            try
                            {
                                System.IO.File.Copy(sourcePath, destinationPath, true);
                            }
                            catch (Exception) { }
                        }
                        else
                        {
                            string noImagePath = SiteSettingHelper.NoImage; // Settinfile.Element("NoImage").Value;
                            if (!string.IsNullOrEmpty(noImagePath))
                            {
                                string noImage = System.Web.HttpContext.Current.Server.MapPath(noImagePath);
                                try
                                {
                                    System.IO.File.Copy(noImage, destinationPath, true);
                                }
                                catch (Exception) { }
                            }
                        }
                    }
                }

                ItemSupplierDetailsDAL objItemSupplierDetailsDAL = new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName);
                Guid ItemGuid = Guid.Empty;
                Guid.TryParse(objItemMasterDTO.GUID.ToString(), out ItemGuid);
                ItemSupplierDetailsDTO objItemSupplierDetailsDTO = objItemSupplierDetailsDAL.GetSupplierByItemGuidBySupplierID(SessionHelper.RoomID, SessionHelper.CompanyID, ItemGuid, objSupplierCatalogItemDTO.SupplierID);

                if (objItemSupplierDetailsDTO == null)
                {
                    objItemSupplierDetailsDTO = new ItemSupplierDetailsDTO();
                    objItemSupplierDetailsDTO.CompanyID = SessionHelper.CompanyID;
                    objItemSupplierDetailsDTO.Created = DateTimeUtility.DateTimeNow;
                    objItemSupplierDetailsDTO.CreatedBy = SessionHelper.UserID;
                    objItemSupplierDetailsDTO.CreatedByName = SessionHelper.UserName;
                    objItemSupplierDetailsDTO.ID = 0;
                    objItemSupplierDetailsDTO.IsArchived = false;
                    objItemSupplierDetailsDTO.IsDeleted = false;
                    objItemSupplierDetailsDTO.IsDefault = true;
                    objItemSupplierDetailsDTO.ItemGUID = objItemMasterDTO.GUID;
                    objItemSupplierDetailsDTO.LastUpdatedBy = SessionHelper.UserID;
                    objItemSupplierDetailsDTO.Room = SessionHelper.RoomID;
                    objItemSupplierDetailsDTO.RoomName = SessionHelper.RoomName;
                    objItemSupplierDetailsDTO.SupplierID = objSupplierCatalogItemDTO.SupplierID;
                    objItemSupplierDetailsDTO.SupplierName = objSupplierCatalogItemDTO.SupplierName;
                    objItemSupplierDetailsDTO.SupplierNumber = objSupplierCatalogItemDTO.SupplierPartNumber;
                    objItemSupplierDetailsDTO.Updated = DateTimeUtility.DateTimeNow;
                    objItemSupplierDetailsDTO.UpdatedByName = SessionHelper.UserName;
                    objItemSupplierDetailsDTO.EditedFrom = "Web";
                    objItemSupplierDetailsDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objItemSupplierDetailsDTO.AddedFrom = "Web";
                    objItemSupplierDetailsDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objItemSupplierDetailsDTO.ID = objItemSupplierDetailsDAL.Insert(objItemSupplierDetailsDTO);
                }

                if (!string.IsNullOrEmpty(objSupplierCatalogItemDTO.ManufacturerName) && !string.IsNullOrWhiteSpace(objSupplierCatalogItemDTO.ManufacturerName))
                {
                    ItemManufacturerDetailsDAL objItemManufacturerDetailsDAL = new ItemManufacturerDetailsDAL(SessionHelper.EnterPriseDBName);
                    ItemManufacturerDetailsDTO objItemManufacturerDetailsDTO = objItemManufacturerDetailsDAL.GetManufacturerByItemGuidByManufacturerID(SessionHelper.RoomID, SessionHelper.CompanyID, ItemGuid, false, objSupplierCatalogItemDTO.ManufacturerID.GetValueOrDefault(0));

                    if (objItemManufacturerDetailsDTO == null)
                    {
                        objItemManufacturerDetailsDTO = new ItemManufacturerDetailsDTO();
                        objItemManufacturerDetailsDTO.CompanyID = SessionHelper.CompanyID;
                        objItemManufacturerDetailsDTO.Created = DateTimeUtility.DateTimeNow;
                        objItemManufacturerDetailsDTO.CreatedBy = SessionHelper.UserID;
                        objItemManufacturerDetailsDTO.CreatedByName = SessionHelper.UserName;
                        objItemManufacturerDetailsDTO.ID = 0;
                        objItemManufacturerDetailsDTO.IsDefault = true;
                        objItemManufacturerDetailsDTO.IsArchived = false;
                        objItemManufacturerDetailsDTO.IsDeleted = false;
                        objItemManufacturerDetailsDTO.ItemGUID = objItemMasterDTO.GUID;
                        objItemManufacturerDetailsDTO.LastUpdatedBy = SessionHelper.UserID;
                        objItemManufacturerDetailsDTO.Room = SessionHelper.RoomID;
                        objItemManufacturerDetailsDTO.RoomName = SessionHelper.RoomName;
                        objItemManufacturerDetailsDTO.ManufacturerID = objSupplierCatalogItemDTO.ManufacturerID.GetValueOrDefault(0);
                        objItemManufacturerDetailsDTO.ManufacturerName = objSupplierCatalogItemDTO.ManufacturerName;
                        objItemManufacturerDetailsDTO.ManufacturerNumber = objSupplierCatalogItemDTO.ManufacturerPartNumber;
                        objItemManufacturerDetailsDTO.Updated = DateTimeUtility.DateTimeNow;
                        objItemManufacturerDetailsDTO.UpdatedByName = SessionHelper.UserName;
                        objItemManufacturerDetailsDTO.EditedFrom = "Web";
                        objItemManufacturerDetailsDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objItemManufacturerDetailsDTO.AddedFrom = "Web";
                        objItemManufacturerDetailsDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objItemManufacturerDetailsDTO.ID = objItemManufacturerDetailsDAL.Insert(objItemManufacturerDetailsDTO);
                    }
                }

                BinMasterDAL objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                BinMasterDTO objBinDTO = null;
                long SessionUserId = SessionHelper.UserID;
                if (!string.IsNullOrEmpty(objRoomDTO.DefaultBinName))
                {

                    objBinDTO = objBinDAL.GetBinByName(objRoomDTO.DefaultBinName, SessionHelper.RoomID, SessionHelper.CompanyID, false);
                    if (objBinDTO == null)
                    {
                        objBinDTO = new BinMasterDTO();
                        objBinDTO.BinNumber = objRoomDTO.DefaultBinName;
                        objBinDTO.ParentBinId = null;
                        objBinDTO.CreatedBy = SessionHelper.UserID;
                        objBinDTO.LastUpdatedBy = SessionHelper.UserID;
                        objBinDTO.Created = DateTimeUtility.DateTimeNow;
                        objBinDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        objBinDTO.Room = SessionHelper.RoomID;
                        objBinDTO.CompanyID = SessionHelper.CompanyID;
                        objBinDTO.AddedFrom = "Web";
                        objBinDTO.EditedFrom = "Web";
                        objBinDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objBinDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objBinDTO = objBinDAL.InsertBin(objBinDTO);
                    }

                    List<BinMasterDTO> lstBinlist = new List<BinMasterDTO>();
                    objBinDTO.IsDefault = true;
                    lstBinlist.Add(objBinDTO);

                    double? UpdatedOnHandQuantity = 0;
                    List<long> insertedBinIds = new List<long>();
                    lstBinlist = objBinDAL.AssignUpdateItemLocations(lstBinlist, objItemMasterDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, false, out UpdatedOnHandQuantity, out insertedBinIds, SessionUserId);
                    if (UpdatedOnHandQuantity != null)
                    {
                        objItemMasterDTO.OnHandQuantity = UpdatedOnHandQuantity;
                        objItemMasterDAL.UpdateOnHandQuantity(objItemMasterDTO.GUID, (double)UpdatedOnHandQuantity);
                    }
                }
                else
                {
                    objBinDTO = objBinDAL.GetBinByName("Bin1", SessionHelper.RoomID, SessionHelper.CompanyID, false);
                    if (objBinDTO == null)
                    {
                        objBinDTO = new BinMasterDTO();
                        objBinDTO.BinNumber = "Bin1";
                        objBinDTO.ParentBinId = null;
                        objBinDTO.CreatedBy = SessionHelper.UserID;
                        objBinDTO.LastUpdatedBy = SessionHelper.UserID;
                        objBinDTO.Created = DateTimeUtility.DateTimeNow;
                        objBinDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        objBinDTO.Room = SessionHelper.RoomID;
                        objBinDTO.CompanyID = SessionHelper.CompanyID;
                        objBinDTO.AddedFrom = "Web";
                        objBinDTO.EditedFrom = "Web";
                        objBinDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objBinDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objBinDTO = objBinDAL.InsertBin(objBinDTO);
                    }
                    List<BinMasterDTO> lstBinlist = new List<BinMasterDTO>();
                    objBinDTO.IsDefault = true;
                    lstBinlist.Add(objBinDTO);

                    double? UpdatedOnHandQuantity = 0;
                    List<long> insertedBinIds = new List<long>();
                    lstBinlist = objBinDAL.AssignUpdateItemLocations(lstBinlist, objItemMasterDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, false, out UpdatedOnHandQuantity, out insertedBinIds, SessionUserId);
                    if (UpdatedOnHandQuantity != null)
                    {
                        objItemMasterDTO.OnHandQuantity = UpdatedOnHandQuantity;
                        objItemMasterDAL.UpdateOnHandQuantity(objItemMasterDTO.GUID, (double)UpdatedOnHandQuantity);
                    }
                }
                switch (objSupplierCatalogItemDTO.DestinationModule)
                {
                    case "ItemMaster":
                        return Json(new
                        {
                            IsItemExist = true,
                            ItemID = objItemMasterDTO.ID,
                            SupplierCatalogItem = objSupplierCatalogItemDTO
                        });
                    case "CartItemMaster":
                        CartItemDTO objCartItemDTO = new CartItemDTO();
                        objCartItemDTO.Created = DateTimeUtility.DateTimeNow;
                        objCartItemDTO.CreatedByName = SessionHelper.UserName;
                        objCartItemDTO.CreatedBy = SessionHelper.UserID;
                        objCartItemDTO.Updated = DateTimeUtility.DateTimeNow;
                        objCartItemDTO.UpdatedByName = SessionHelper.UserName;
                        objCartItemDTO.LastUpdatedBy = SessionHelper.UserID;
                        objCartItemDTO.ID = 0;
                        objCartItemDTO.ItemNumber = objItemMasterDTO.ItemNumber;
                        objCartItemDTO.UDF1 = "";
                        objCartItemDTO.UDF2 = "";
                        objCartItemDTO.UDF3 = "";
                        objCartItemDTO.UDF4 = "";
                        objCartItemDTO.UDF5 = "";
                        objCartItemDTO.ReplenishType = "";
                        objCartItemDTO.ItemGUID = objItemMasterDTO.GUID;
                        objCartItemDTO.CompanyID = SessionHelper.CompanyID;
                        objCartItemDTO.Room = SessionHelper.RoomID;
                        objCartItemDTO.Quantity = objSupplierCatalogItemDTO.Quantity;
                        objCartItemDTO.IsDeleted = false;
                        objCartItemDTO.IsArchived = false;
                        objCartItemDTO.WhatWhereAction = "SupplierCatelog";
                        new CartItemDAL(SessionHelper.EnterPriseDBName).SaveCartItem(objCartItemDTO, SessionUserId, SessionHelper.EnterPriceID);
                        return Json(new
                        {
                            IsItemExist = true,
                            ItemID = 0,
                            SupplierCatalogItem = objSupplierCatalogItemDTO
                        });
                    case "OrderMaster":
                        if (objSupplierCatalogItemDTO.OrderGUID != Guid.Empty)
                        {
                            OrderMasterDAL objOrdDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);

                            OrderMasterDTO objOrdDTO = objOrdDAL.GetOrderByGuidPlain(objSupplierCatalogItemDTO.OrderGUID);
                            List<OrderDetailsDTO> lstOrdDetails = new List<OrderDetailsDTO>();

                            //OrderDetailsDAL objOrderDetailsDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                            //List<OrderDetailsDTO> lstOrderItems = objOrderDetailsDAL.GetOrderedRecord(objSupplierCatalogItemDTO.OrderGUID, SessionHelper.RoomID, SessionHelper.CompanyID).Where(t => t.IsArchived == false && t.IsDeleted == false).ToList();
                            //if (!lstOrderItems.Any(t => t.ItemGUID == objItemMasterDTO.GUID))
                            //{
                            OrderDetailsDTO objOrderDetailsDTO = new OrderDetailsDTO();
                            objOrderDetailsDTO.Room = SessionHelper.RoomID;
                            objOrderDetailsDTO.RequiredDate = DateTimeUtility.DateTimeNow;
                            objOrderDetailsDTO.CreatedBy = SessionHelper.UserID;
                            objOrderDetailsDTO.LastUpdatedBy = SessionHelper.UserID;
                            objOrderDetailsDTO.CompanyID = SessionHelper.CompanyID;
                            objOrderDetailsDTO.OrderGUID = objSupplierCatalogItemDTO.OrderGUID;
                            objOrderDetailsDTO.ItemGUID = objItemMasterDTO.GUID;
                            objOrderDetailsDTO.RequestedQuantity = objSupplierCatalogItemDTO.Quantity;
                            lstOrdDetails.Add(objOrderDetailsDTO);
                            OrderController ordCntrl = new OrderController();
                            //ordCntrl.AddNewItemsToOrder(lstOrdDetails.ToArray(), objOrdDTO.ID,null,null);
                            ordCntrl.AddItemsToOrder(lstOrdDetails.ToArray(), objOrdDTO.ID);
                            return Json(new
                            {
                                IsItemExist = true,
                                ItemID = objItemMasterDTO.ID,
                                SupplierCatalogItem = objSupplierCatalogItemDTO
                            });
                        }
                        return Json(new
                        {
                            IsItemExist = false,
                            ItemID = 0,
                            SupplierCatalogItem = objSupplierCatalogItemDTO
                        });
                    default:
                        return Json(new
                        {
                            IsItemExist = true,
                            ItemID = 0,
                            SupplierCatalogItem = objSupplierCatalogItemDTO
                        });


                }

            }
            else
            {
                switch (objSupplierCatalogItemDTO.DestinationModule)
                {
                    case "ItemMaster":
                        return Json(new
                        {
                            IsItemExist = false,
                            ItemID = objItemMasterDTO.ID,
                            SupplierCatalogItem = objSupplierCatalogItemDTO
                        });
                    case "CartItemMaster":
                        return Json(new
                        {
                            IsItemExist = false,
                            ItemID = 0,
                            SupplierCatalogItem = objSupplierCatalogItemDTO
                        });
                    case "OrderMaster":
                        return Json(new
                        {
                            IsItemExist = false,
                            ItemID = 0,
                            SupplierCatalogItem = objSupplierCatalogItemDTO
                        });
                    default:
                        return Json(new
                        {
                            IsItemExist = false,
                            ItemID = 0,
                            SupplierCatalogItem = objSupplierCatalogItemDTO
                        });
                }

            }

        }



        public ActionResult LoadSupplierCatalogItems(string SourcePage, string OrderGUID, string OrderSupplier, string OrderSupplierID = "")
        {
            switch (SourcePage)
            {
                default:
                    break;
            }
            ItemModelPerameter objItemModelPerameter = new ItemModelPerameter()
            {
                AjaxURLAddItemToSession = "~/SupplierCatalog/AddItemToMSSession/",
                ModelHeader = ResSupplierMaster.SelectItemsFromCatalog,
                AjaxURLAddMultipleItemToSession = "~/SupplierCatalog/AddItemToMSSessionMultiple/",
                AjaxURLToFillItemGrid = "~/SupplierCatalog/GetAllCatalogItems/",
                CallingFromPageName = SourcePage
            };
            ViewBag.OrderGUID = OrderGUID;
            if (string.IsNullOrWhiteSpace(OrderSupplier) && SourcePage == "OrderMaster" && !string.IsNullOrWhiteSpace(OrderGUID))
            {
                OrderMasterDAL objOrdDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                OrderMasterDTO objOrderDTO = objOrdDAL.GetOrderByGuidNormal(Guid.Parse(OrderGUID));
                if (objOrderDTO != null && !string.IsNullOrWhiteSpace(objOrderDTO.SupplierName))
                    OrderSupplier = objOrderDTO.SupplierName;
                if (objOrderDTO != null)
                    OrderSupplierID = Convert.ToString(objOrderDTO.Supplier);
            }
            ViewBag.OrderSupplier = OrderSupplier;
            ViewBag.OrderSupplierID = OrderSupplierID;
            return PartialView("_CatalogItemModel", objItemModelPerameter);
        }

        public ActionResult GetAllCatalogItems(JQueryDataTableParamModel param)
        {
            string OrderSupplier = Convert.ToString(Request["OrderSupplier"]);

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
            sortColumnName = Request["SortingField"].ToString().Trim();

            string sSearch = WebUtility.HtmlDecode(param.sSearch);

            if (string.IsNullOrEmpty(sortColumnName) || sortColumnName == "undefine" || sortColumnName == "0" || sortColumnName.Trim().Contains("null") || string.IsNullOrEmpty(sortColumnName.Replace("asc", "").Replace("null", "").Replace("desc", "").Trim()))
                sortColumnName = "SupplierCatalogItemID asc";
            //if (sortColumnName == "0" || sortColumnName == "undefined" || sortColumnName == "null")
            //    sortColumnName = "SupplierCatalogItemID";

            //if (sortDirection == "asc")
            //    sortColumnName = sortColumnName + " asc";
            //else
            //    sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;

            string ItemsIDs = string.Empty;
            CatalogItemMasterDAL objCatalogItemMasterDAL = new CatalogItemMasterDAL(SessionHelper.EnterPriseDBName);

            var DataFromDB = objCatalogItemMasterDAL.GetPagedRecordsByDB(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, "", Convert.ToInt64(OrderSupplier));
            //var DataFromDB = objCatalogItemMasterDAL.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
            //ViewBag.iTotalSupplierCatalogCount = DataFromDB.Count;
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB
            }, JsonRequestBehavior.AllowGet);
        }

    }
}
