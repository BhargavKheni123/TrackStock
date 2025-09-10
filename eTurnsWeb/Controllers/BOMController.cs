using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsWeb.Helper;
using eTurnsWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace eTurnsWeb.Controllers
{
    [AuthorizeHelper]
    public class BOMController : eTurnsControllerBase
    {
        private static readonly ThreadLocal<Random> appRandom = new ThreadLocal<Random>(() => new Random());
        public ActionResult BOMLanding()
        {
            return View();
        }

        public ActionResult GetBOM(string PageName, bool? Openpopup, string Buttonname)
        {
            BOMItemDTO objBOMItemDTO = new BOMItemDTO();
            objBOMItemDTO.DestinationModule = PageName;
            objBOMItemDTO.SourcePageName = PageName;
            objBOMItemDTO.OpenPopup = Openpopup ?? false;
            objBOMItemDTO.ButtonText = Buttonname;
            return PartialView("BOM", objBOMItemDTO);
        }

        [HttpPost]
        public JsonResult AddItemFromBOMBack(BOMItemDTO objBOMItemDTO)
        {
            BOMItemDTO objItemMasterDTO = new BOMItemDTO();
            BOMItemMasterDAL objBOMItemMasterDAL = new BOMItemMasterDAL(SessionHelper.EnterPriseDBName);
            objItemMasterDTO = objBOMItemMasterDAL.GetItemByItemName(objBOMItemDTO.ItemNumber, SessionHelper.CompanyID, SessionHelper.RoomID);
            SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            SupplierMasterDTO objSupplierMasterDTO = objSupplierMasterDAL.GetSupplierByNamePlain(SessionHelper.RoomID, SessionHelper.CompanyID, false, objBOMItemDTO.SupplierName);
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
                objSupplierMasterDTO.SupplierName = objBOMItemDTO.SupplierName;
                objSupplierMasterDTO.UpdatedByName = SessionHelper.UserName;
                objSupplierMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                objSupplierMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                objSupplierMasterDTO.EditedFrom = "Web";
                objSupplierMasterDTO.ID = objSupplierMasterDAL.Insert(objSupplierMasterDTO);


            }
            if (objSupplierMasterDTO != null)
            {
                objBOMItemDTO.SupplierID = objSupplierMasterDTO.ID;
            }

            ManufacturerMasterDAL objManufacturerMasterDAL = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName);
            ManufacturerMasterDTO objManufacturerMasterDTO = new ManufacturerMasterDTO();
            //Wi-1505
            //if (!string.IsNullOrEmpty(objBOMItemDTO.ManufacturerName))
            objManufacturerMasterDTO = objManufacturerMasterDAL.GetManufacturerByNameNormal(objBOMItemDTO.ManufacturerName, SessionHelper.RoomID, SessionHelper.CompanyID, false);
            //else
            //    objManufacturerMasterDTO = objManufacturerMasterDAL.GetBlankManuMaster();

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
                objManufacturerMasterDTO.Manufacturer = objBOMItemDTO.ManufacturerName;
                objManufacturerMasterDTO.UpdatedByName = SessionHelper.UserName;
                objManufacturerMasterDTO.AddedFrom = "Web";
                objManufacturerMasterDTO.EditedFrom = "Web";
                objManufacturerMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                objManufacturerMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                objManufacturerMasterDTO.ID = objManufacturerMasterDAL.Insert(objManufacturerMasterDTO);
            }
            if (objManufacturerMasterDTO != null)
            {
                objBOMItemDTO.ManufacturerID = objManufacturerMasterDTO.ID;
            }

            if (objItemMasterDTO != null)
            {
                switch (objBOMItemDTO.DestinationModule)
                {
                    case "ItemMaster":
                        ItemSupplierDetailsDAL objItemSupplierDetailsDAL = new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName);
                        Guid ItemGuid = Guid.Empty;
                        Guid.TryParse(objItemMasterDTO.GUID.ToString(), out ItemGuid);
                        ItemSupplierDetailsDTO objItemSupplierDetailsDTO = objItemSupplierDetailsDAL.GetSupplierByItemGuidBySupplierID(SessionHelper.RoomID, SessionHelper.CompanyID, ItemGuid, objBOMItemDTO.SupplierID);
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
                            objItemSupplierDetailsDTO.SupplierID = objBOMItemDTO.SupplierID ?? 0;
                            objItemSupplierDetailsDTO.SupplierName = objBOMItemDTO.SupplierName;
                            objItemSupplierDetailsDTO.SupplierNumber = objBOMItemDTO.SupplierPartNumber;
                            objItemSupplierDetailsDTO.Updated = DateTimeUtility.DateTimeNow;
                            objItemSupplierDetailsDTO.UpdatedByName = SessionHelper.UserName;
                            objItemSupplierDetailsDTO.EditedFrom = "Web";
                            objItemSupplierDetailsDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objItemSupplierDetailsDTO.AddedFrom = "Web";
                            objItemSupplierDetailsDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            objItemSupplierDetailsDTO.ID = objItemSupplierDetailsDAL.Insert(objItemSupplierDetailsDTO);
                        }
                        ItemManufacturerDetailsDAL objItemManufacturerDetailsDAL = new ItemManufacturerDetailsDAL(SessionHelper.EnterPriseDBName);
                        ItemManufacturerDetailsDTO objItemManufacturerDetailsDTO = objItemManufacturerDetailsDAL.GetManufacturerByItemGuidByManufacturerID(SessionHelper.RoomID, SessionHelper.CompanyID, ItemGuid, false, objBOMItemDTO.ManufacturerID);
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
                            objItemManufacturerDetailsDTO.ManufacturerID = objBOMItemDTO.ManufacturerID ?? 0;
                            objItemManufacturerDetailsDTO.ManufacturerName = objBOMItemDTO.ManufacturerName;
                            objItemManufacturerDetailsDTO.ManufacturerNumber = objBOMItemDTO.ManufacturerPartNumber;
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
                            BOMItem = objBOMItemDTO
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
                        objCartItemDTO.Quantity = objBOMItemDTO.Quantity;
                        objCartItemDTO.IsDeleted = false;
                        objCartItemDTO.IsArchived = false;
                        objCartItemDTO.WhatWhereAction = "Bill of material";
                        new CartItemDAL(SessionHelper.EnterPriseDBName).SaveCartItem(objCartItemDTO, SessionUserId,SessionHelper.EnterPriceID);
                        return Json(new
                        {
                            IsItemExist = true,
                            ItemID = 0,
                            BOMItem = objBOMItemDTO
                        });
                    case "OrderMaster":

                        OrderDetailsDAL objOrderDetailsDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                        List<OrderDetailsDTO> lstOrderItems = objOrderDetailsDAL.GetOrderDetailByOrderGUIDPlain(objBOMItemDTO.OrderGUID, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
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
                            objOrderDetailsDTO.OrderGUID = objBOMItemDTO.OrderGUID;
                            objOrderDetailsDTO.ItemGUID = objItemMasterDTO.GUID;
                            objOrderDetailsDTO.ItemGUID = objItemMasterDTO.GUID;
                            //objOrderDetailsDTO.ItemDetail = objItemMasterDTO;
                            objOrderDetailsDTO.RequestedQuantity = objBOMItemDTO.Quantity;
                            objOrderDetailsDTO.AddedFrom = "Web";
                            objOrderDetailsDTO.EditedFrom = "Web";
                            objOrderDetailsDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objOrderDetailsDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            objOrderDetailsDAL.Insert(objOrderDetailsDTO, SessionUserId,SessionHelper.EnterPriceID);
                        }
                        return Json(new
                        {
                            IsItemExist = true,
                            ItemID = 0,
                            BOMItem = objBOMItemDTO
                        });
                    default:
                        return Json(new
                        {
                            IsItemExist = true,
                            ItemID = 0,
                            BOMItem = objBOMItemDTO
                        });


                }

            }
            else
            {
                switch (objBOMItemDTO.DestinationModule)
                {
                    case "ItemMaster":
                        return Json(new
                        {
                            IsItemExist = false,
                            ItemID = 0,
                            BOMItem = objBOMItemDTO
                        });
                    case "CartItemMaster":
                        return Json(new
                        {
                            IsItemExist = false,
                            ItemID = 0,
                            BOMItem = objBOMItemDTO
                        });
                    case "OrderMaster":
                        return Json(new
                        {
                            IsItemExist = false,
                            ItemID = 0,
                            BOMItem = objBOMItemDTO
                        });
                    default:
                        return Json(new
                        {
                            IsItemExist = false,
                            ItemID = 0,
                            BOMItem = objBOMItemDTO
                        });
                }

            }


        }

        [HttpPost]
        public JsonResult AddItemFromBOM(BOMItemDTO objBOMItemDTO)
        {
            BOMItemDTO objItemMasterDTO = new BOMItemDTO();
            BOMItemMasterDAL objBOMItemMasterDAL = new BOMItemMasterDAL(SessionHelper.EnterPriseDBName);
            objItemMasterDTO = objBOMItemMasterDAL.GetItemByItemID(objBOMItemDTO.ID);
            SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            SupplierMasterDTO objSupplierMasterDTO = objSupplierMasterDAL.GetSupplierByNamePlain(SessionHelper.RoomID, SessionHelper.CompanyID, false, objBOMItemDTO.SupplierName);
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
                objSupplierMasterDTO.SupplierName = objBOMItemDTO.SupplierName;
                objSupplierMasterDTO.UpdatedByName = SessionHelper.UserName;
                objSupplierMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                objSupplierMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                objSupplierMasterDTO.AddedFrom = "Web";
                objSupplierMasterDTO.EditedFrom = "Web";
                objSupplierMasterDTO.ID = objSupplierMasterDAL.Insert(objSupplierMasterDTO);


            }
            if (objSupplierMasterDTO != null)
            {
                objBOMItemDTO.SupplierID = objSupplierMasterDTO.ID;
            }

            ManufacturerMasterDAL objManufacturerMasterDAL = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName);
            ManufacturerMasterDTO objManufacturerMasterDTO = objManufacturerMasterDAL.GetManufacturerByNameNormal(objBOMItemDTO.ManufacturerName, SessionHelper.RoomID, SessionHelper.CompanyID, false);




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
                objManufacturerMasterDTO.Manufacturer = objBOMItemDTO.ManufacturerName;
                objManufacturerMasterDTO.UpdatedByName = SessionHelper.UserName;
                objManufacturerMasterDTO.AddedFrom = "Web";
                objManufacturerMasterDTO.EditedFrom = "Web";
                objManufacturerMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                objManufacturerMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                objManufacturerMasterDTO.ID = objManufacturerMasterDAL.Insert(objManufacturerMasterDTO);
            }
            if (objManufacturerMasterDTO != null)
            {
                objBOMItemDTO.ManufacturerID = objManufacturerMasterDTO.ID;
            }

            if (objItemMasterDTO != null)
            {
                switch (objBOMItemDTO.DestinationModule)
                {
                    case "ItemMaster":
                        ItemSupplierDetailsDAL objItemSupplierDetailsDAL = new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName);
                        Guid ItemGuid = Guid.Empty;
                        Guid.TryParse(objItemMasterDTO.GUID.ToString(), out ItemGuid);
                        ItemSupplierDetailsDTO objItemSupplierDetailsDTO = objItemSupplierDetailsDAL.GetSupplierByItemGuidBySupplierID(SessionHelper.RoomID, SessionHelper.CompanyID, objItemMasterDTO.GUID, objBOMItemDTO.SupplierID);
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
                            objItemSupplierDetailsDTO.SupplierID = objBOMItemDTO.SupplierID ?? 0;
                            objItemSupplierDetailsDTO.SupplierName = objBOMItemDTO.SupplierName;
                            objItemSupplierDetailsDTO.SupplierNumber = objBOMItemDTO.SupplierPartNumber;
                            objItemSupplierDetailsDTO.Updated = DateTimeUtility.DateTimeNow;
                            objItemSupplierDetailsDTO.UpdatedByName = SessionHelper.UserName;
                            objItemSupplierDetailsDTO.EditedFrom = "Web";
                            objItemSupplierDetailsDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objItemSupplierDetailsDTO.AddedFrom = "Web";
                            objItemSupplierDetailsDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            objItemSupplierDetailsDTO.ID = objItemSupplierDetailsDAL.Insert(objItemSupplierDetailsDTO);
                        }
                        ItemManufacturerDetailsDAL objItemManufacturerDetailsDAL = new ItemManufacturerDetailsDAL(SessionHelper.EnterPriseDBName);
                        ItemManufacturerDetailsDTO objItemManufacturerDetailsDTO = objItemManufacturerDetailsDAL.GetManufacturerByItemGuidByManufacturerID(SessionHelper.RoomID, SessionHelper.CompanyID, ItemGuid, false, objBOMItemDTO.ManufacturerID);
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
                            objItemManufacturerDetailsDTO.ManufacturerID = objBOMItemDTO.ManufacturerID ?? 0;
                            objItemManufacturerDetailsDTO.ManufacturerName = objBOMItemDTO.ManufacturerName;
                            objItemManufacturerDetailsDTO.ManufacturerNumber = objBOMItemDTO.ManufacturerPartNumber;
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
                            BOMItem = objBOMItemDTO
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
                        objCartItemDTO.Quantity = objBOMItemDTO.Quantity;
                        objCartItemDTO.IsDeleted = false;
                        objCartItemDTO.IsArchived = false;
                        objCartItemDTO.WhatWhereAction = "Bill of material";
                        new CartItemDAL(SessionHelper.EnterPriseDBName).SaveCartItem(objCartItemDTO, SessionUserId,SessionHelper.EnterPriceID);
                        return Json(new
                        {
                            IsItemExist = true,
                            ItemID = 0,
                            BOMItem = objBOMItemDTO
                        });
                    case "OrderMaster":

                        OrderDetailsDAL objOrderDetailsDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                        List<OrderDetailsDTO> lstOrderItems = objOrderDetailsDAL.GetOrderDetailByOrderGUIDPlain(objBOMItemDTO.OrderGUID, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
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
                            objOrderDetailsDTO.OrderGUID = objBOMItemDTO.OrderGUID;
                            objOrderDetailsDTO.ItemGUID = objItemMasterDTO.GUID;
                            objOrderDetailsDTO.ItemGUID = objItemMasterDTO.GUID;
                            //objOrderDetailsDTO.ItemDetail = objItemMasterDTO;
                            objOrderDetailsDTO.RequestedQuantity = objBOMItemDTO.Quantity;
                            objOrderDetailsDTO.AddedFrom = "Web";
                            objOrderDetailsDTO.EditedFrom = "Web";
                            objOrderDetailsDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objOrderDetailsDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            objOrderDetailsDAL.Insert(objOrderDetailsDTO, SessionUserId,SessionHelper.EnterPriceID);
                        }
                        return Json(new
                        {
                            IsItemExist = true,
                            ItemID = 0,
                            BOMItem = objBOMItemDTO
                        });
                    default:
                        return Json(new
                        {
                            IsItemExist = true,
                            ItemID = 0,
                            BOMItem = objBOMItemDTO
                        });


                }

            }
            else
            {
                switch (objBOMItemDTO.DestinationModule)
                {
                    case "ItemMaster":
                        return Json(new
                        {
                            IsItemExist = false,
                            ItemID = 0,
                            BOMItem = objBOMItemDTO
                        });
                    case "CartItemMaster":
                        return Json(new
                        {
                            IsItemExist = false,
                            ItemID = 0,
                            BOMItem = objBOMItemDTO
                        });
                    case "OrderMaster":
                        return Json(new
                        {
                            IsItemExist = false,
                            ItemID = 0,
                            BOMItem = objBOMItemDTO
                        });
                    default:
                        return Json(new
                        {
                            IsItemExist = false,
                            ItemID = 0,
                            BOMItem = objBOMItemDTO
                        });
                }

            }


        }

        public ActionResult LoadBOMItems(string SourcePage, string OrderGUID, string OrderSupplier)
        {
            switch (SourcePage)
            {
                default:
                    break;
            }
            BOMItemMasterDAL obj = new BOMItemMasterDAL(SessionHelper.EnterPriseDBName);
            ItemModelPerameter objItemModelPerameter = new ItemModelPerameter()
            {
                AjaxURLAddItemToSession = "~/BOM/AddItemToMSSession/",
                ModelHeader = ResItemMaster.MsgSelectItemFromBOM, 
                AjaxURLAddMultipleItemToSession = "~/BOM/AddAllItemFromBom/",
                AjaxURLToFillItemGrid = "~/BOM/BOMItemPOPUPListAjax/",
                CallingFromPageName = SourcePage
            };
            ViewBag.OrderGUID = OrderGUID;
            ViewBag.OrderSupplier = OrderSupplier;
            //Session["lstBomIterms"] = new BOMItemMasterDAL(SessionHelper.EnterPriseDBName).GetAllBOMItems(SessionHelper.CompanyID);
            int TotalRecordCount = 0;
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            Session["lstBomIterms"] = obj.GetPagedRecords(0, int.MaxValue, out TotalRecordCount, string.Empty, string.Empty, SessionHelper.CompanyID, false, false, SessionHelper.RoomID, "popup", RoomDateFormat, CurrentTimeZone);
            return PartialView("_BOMItemModel", objItemModelPerameter);
        }

        public ActionResult GetAllBOMItems(JQueryDataTableParamModel param)
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
            sortColumnName = Request["SortingField"].ToString();

            if (sortColumnName == "0" || sortColumnName == "undefined")
                sortColumnName = "BOMItemID";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;

            string ItemsIDs = string.Empty;
            BOMItemMasterDAL objBOMItemMasterDAL = new BOMItemMasterDAL(SessionHelper.EnterPriseDBName);
            string RoomIds = string.Join(",", SessionHelper.RoomList.Where(t => t.IsRoomActive == true && t.CompanyID == SessionHelper.CompanyID && t.ID != SessionHelper.RoomID).Select(t => t.ID).ToArray());
            List<BOMItemDTO> lstBOMItems = new List<BOMItemDTO>();
            if (!string.IsNullOrEmpty(RoomIds))
            {
                lstBOMItems = objBOMItemMasterDAL.GetPagedRecordsByDB(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, OrderSupplier, RoomIds);
            }


            //var DataFromDB = objBOMItemMasterDAL.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = lstBOMItems
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddAllItemFromBom(List<BOMItemDTO> lstItems)
        {
            string strCsv = string.Join(",", lstItems.Select(t => t.ID).ToArray());
            BOMItemMasterDAL objBOMItemMasterDAL = new BOMItemMasterDAL(SessionHelper.EnterPriseDBName);
            objBOMItemMasterDAL.AddItemsToRoom(strCsv, SessionHelper.RoomID, SessionHelper.UserID);
            try
            {
                CommonUtility.SaveBOMImageToRoomItem(strCsv, "both");
            }
            catch (Exception ex){}
            string strConsMsg = CommonUtility.ValidateBOMConsignToRoomItem(strCsv);
            CacheHelper<IEnumerable<ItemMasterDTO>>.InvalidateCache();
            CacheHelper<IEnumerable<CategoryMasterDTO>>.InvalidateCache();
            CacheHelper<IEnumerable<UnitMasterDTO>>.InvalidateCache();
            CacheHelper<IEnumerable<ManufacturerMasterDTO>>.InvalidateCache();
            CacheHelper<IEnumerable<ItemManufacturerDetailsDTO>>.InvalidateCache();
            CacheHelper<IEnumerable<ItemSupplierDetailsDTO>>.InvalidateCache();
            CacheHelper<IEnumerable<SupplierMasterDTO>>.InvalidateCache();
            //CacheHelper<IEnumerable<GLAccountMasterDTO>>.InvalidateCache();
            CacheHelper<IEnumerable<InventoryClassificationMasterDTO>>.InvalidateCache();
            //CacheHelper<IEnumerable<CostUOMMasterDTO>>.InvalidateCache();
            CacheHelper<IEnumerable<ItemLocationQTYDTO>>.InvalidateCache();
            Session["lstBomIterms"] = null;
            if (strConsMsg == string.Empty)
                return Json(new { Message = string.Empty, Status = "ok" }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Message = strConsMsg, Status = "warning" }, JsonRequestBehavior.AllowGet);
        }

        #region [Bom Master]

        public ActionResult CategoryList()
        {
            List<eTurns.DTO.CategoryMasterDTO> lstcategories = new List<CategoryMasterDTO>();
            return View("~/Views/Master/CategoryList.cshtml", lstcategories);

        }
        public ActionResult ManufacturerList()
        {
            List<eTurns.DTO.ManufacturerMasterDTO> lstManufacturer = new List<ManufacturerMasterDTO>();
            return View("~/Views/Master/ManufacturerList.cshtml", lstManufacturer);

        }
        public ActionResult GLAccountList()
        {
            List<eTurns.DTO.GLAccountMasterDTO> lstGLAccountMaster = new List<GLAccountMasterDTO>();
            return View("~/Views/Master/GLAccountList.cshtml", lstGLAccountMaster);

        }
        public ActionResult InventoryClassificationList()
        {
            List<eTurns.DTO.InventoryClassificationMasterDTO> lstInventoryClassification = new List<InventoryClassificationMasterDTO>();
            return View("~/Views/Master/InventoryClassificationList.cshtml", lstInventoryClassification);

        }
        public ActionResult UnitList()
        {
            List<eTurns.DTO.UnitMasterDTO> lstInventoryClassification = new List<UnitMasterDTO>();
            return View("~/Views/Master/UnitList.cshtml", lstInventoryClassification);

        }

        public ActionResult CostUOMList()
        {
            List<eTurns.DTO.CostUOMMasterDTO> lstInventoryClassification = new List<CostUOMMasterDTO>();
            return View("~/Views/Master/CostUOMList.cshtml", lstInventoryClassification);

        }
        public ActionResult SupplierList()
        {
            List<eTurns.DTO.SupplierMasterDTO> lstSupplierMasterDTO = new List<SupplierMasterDTO>();
            return View("~/Views/Master/SupplierList.cshtml", lstSupplierMasterDTO);

        }

        #endregion
        public ActionResult BOMItemList()
        {

            return View("~/Views/BOM/BOMItemMasterList.cshtml");

        }
        public ActionResult ItemCreate(SupplierCatalogItemDTO SupplierCatalogItem)
        {
            ViewBag.LockReplenishmentType = false;
            BOMItemDTO objDTO = new BOMItemDTO();
            //objDTO.ItemNumber = "#I" + NewNumber;
            objDTO.ID = 0;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.CreatedByName = SessionHelper.UserName;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.RoomName = string.Empty;
            objDTO.GUID = Guid.NewGuid();
            objDTO.Trend = false;
            objDTO.Taxable = false;
            objDTO.Consignment = false;
            objDTO.IsTransfer = false;
            objDTO.IsPurchase = true;
            objDTO.ItemType = 1;
            objDTO.SerialNumberTracking = false;
            objDTO.LotNumberTracking = false;
            objDTO.DateCodeTracking = false;
            objDTO.DefaultPullQuantity = 1;
            objDTO.DefaultReorderQuantity = 1;
            // objDTO.ItemUniqueNumber = UniqueId.Get();
            objDTO.IsBOMItem = true;
            objDTO.IsItemLevelMinMaxQtyRequired = true;
            string itemGuidString = Convert.ToString(objDTO.GUID);
            Session["BOMItemManufacture" + itemGuidString] = null;
            Session["BOMItemSupplier" + itemGuidString] = null;
            Session["BOMItemKitDetail" + itemGuidString] = null;
            //get room's configuration status for replanishment type and consignment

            ////////////////////////////////default supplier logic/////////////////////////////////////////
            List<ItemSupplierDetailsDTO> lstItemSupplier = new List<ItemSupplierDetailsDTO>();

            //get supplier name from supplier id
            SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            SupplierMasterDTO objSupplierMasterDTO = new SupplierMasterDTO();
            Session["BOMItemSupplier" + itemGuidString] = lstItemSupplier;
            ///////////////////////////////////////////////////////////////////
            ViewBag.LockConsignment = true;

            if (SupplierCatalogItem != null && SupplierCatalogItem.SupplierCatalogItemID > 0)
            {
                ItemSupplierDetailsDTO objItemSupplierDetailsDTO = new ItemSupplierDetailsDTO();
                objItemSupplierDetailsDTO.SupplierID = SupplierCatalogItem.SupplierID;
                objItemSupplierDetailsDTO.SupplierName = SupplierCatalogItem.SupplierName;
                objItemSupplierDetailsDTO.SupplierNumber = SupplierCatalogItem.SupplierPartNumber;
                objItemSupplierDetailsDTO.IsDefault = true;
                ViewBag.strItemSupplierDetailsDTO = objItemSupplierDetailsDTO;

                ItemManufacturerDetailsDTO objItemManufacturerDetailsDTO = new ItemManufacturerDetailsDTO();
                objItemManufacturerDetailsDTO.ManufacturerID = SupplierCatalogItem.ManufacturerID.GetValueOrDefault(0);
                objItemManufacturerDetailsDTO.ManufacturerName = SupplierCatalogItem.ManufacturerName;
                objItemManufacturerDetailsDTO.ManufacturerNumber = SupplierCatalogItem.ManufacturerPartNumber;
                objItemManufacturerDetailsDTO.IsDefault = true;
                ViewBag.strItemManufacturerDetailsDTO = objItemManufacturerDetailsDTO;

                ViewBag.DestinationModule = SupplierCatalogItem.DestinationModule;
                ViewBag.SupplierCatalogQty = SupplierCatalogItem.Quantity;
                ViewBag.SupplierCatalogOrderGUID = SupplierCatalogItem.OrderGUID;

                objDTO.ItemNumber = SupplierCatalogItem.ItemNumber;
                objDTO.UPC = SupplierCatalogItem.UPC;
                objDTO.Description = SupplierCatalogItem.Description;
                objDTO.SellPrice = SupplierCatalogItem.SellPrice;
            }
            UDFController objUDFController = new UDFController();
            ViewBag.UDFs = objUDFController.GetUDFDataPageWise("BOMItemMaster");

            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            //CommonController objCommon = new CommonController();
            CommonDAL objCommon = new CommonDAL(SessionHelper.EnterPriseDBName);
            ViewBag.ManufacturerIDBag = objCommon.GetDDData("ManufacturerMaster", "Manufacturer", SessionHelper.CompanyID, 0);
            ViewBag.SupplierIDBag = objCommon.GetDDData("SupplierMaster", "SupplierName", SessionHelper.CompanyID, 0);
            ViewBag.CategoryIDBag = objCommon.GetDDData("CategoryMaster", "Category", SessionHelper.CompanyID, 0);
            ViewBag.GLAccountIDBag = objCommon.GetDDData("GLAccountMaster", "GLAccount", SessionHelper.CompanyID, 0);
            ViewBag.UOMIDBag = objCommon.GetDDData("UnitMaster", "Unit", SessionHelper.CompanyID, 0);
            ViewBag.CostUOMBag = objCommon.GetDDData("CostUOMMaster", "CostUOM", SessionHelper.CompanyID, 0);

            ViewBag.ItemTypeBag = GetItemTypeOptions();
            //ViewBag.InventoryClassificationBag = GetInventoryClassificationOptions();
            ViewBag.InventoryClassificationBag = objCommon.GetDDData("InventoryClassificationMaster", "InventoryClassification", SessionHelper.CompanyID, 0);
            // ViewBag.DefaultLocationBag = objCommon.GetDDData("BinMaster", "BinNumber", " IsStagingLocation != 1 AND ", SessionHelper.CompanyID, SessionHelper.RoomID);

            UnitMasterDAL objUnitDal = new UnitMasterDAL(SessionHelper.EnterPriseDBName);

            UnitMasterDTO objUnit = objUnitDal.GetUnitByNamePlain(0, SessionHelper.CompanyID, true, "EA");
            if (objUnit != null && objUnit.ID > 0)
            {
                objDTO.UOMID = objUnit.ID;
                objDTO.Unit = objUnit.Unit;
            }


            objDTO.ID = 0;
            objDTO.ImageType = "ExternalImage";
            objDTO.ItemLink2ImageType = "InternalLink";

            //return PartialView("_CreateItem", objDTO);
            //List<eTurns.DTO.ItemMasterDTO> lstItemMasterDTO = new List<ItemMasterDTO>();
            return View("~/Views/BOM/_CreateBomItem.cshtml", objDTO);

        }
        public ActionResult ItemEdit(string ItemGUID)
        {
            bool IsArchived = false;
            bool IsDeleted = false;
            bool IsHistory = false;

            if (Request["IsHistory"] != null && Request["IsHistory"].ToString() != "")
                IsHistory = bool.Parse(Request["IsHistory"].ToString());

            if (!string.IsNullOrEmpty(Request["IsArchived"]) && !string.IsNullOrEmpty(Request["IsDeleted"]))
            {
                IsArchived = bool.Parse(Request["IsArchived"].ToString());
                IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
                //if (IsDeleted || IsArchived)
                //{
                //    ViewBag.ViewOnly = true;
                //}
            }

            if (IsDeleted || IsArchived || IsHistory)
            {
                ViewBag.ViewOnly = true;
            }

            //ItemMasterController obj = new ItemMasterController();
            BOMItemMasterDAL obj = new BOMItemMasterDAL(SessionHelper.EnterPriseDBName);
            BOMItemDTO objDTO = obj.BOMItemByGuid(Guid.Parse(ItemGUID), SessionHelper.CompanyID, IsArchived, IsDeleted);
            UDFController objUDFController = new UDFController();
            ViewBag.UDFs = objUDFController.GetUDFDataPageWise("BOMItemMaster");
            if (objDTO != null)
            {
                ViewData["UDF1"] = objDTO.UDF1;
                ViewData["UDF2"] = objDTO.UDF2;
                ViewData["UDF3"] = objDTO.UDF3;
                ViewData["UDF4"] = objDTO.UDF4;
                ViewData["UDF5"] = objDTO.UDF5;
            }
            //CommonController objCommon = new CommonController();
            CommonDAL objCommon = new CommonDAL(SessionHelper.EnterPriseDBName);
            ViewBag.ManufacturerIDBag = objCommon.GetDDData("ManufacturerMaster", "Manufacturer", SessionHelper.CompanyID, 0);
            ViewBag.SupplierIDBag = objCommon.GetDDData("SupplierMaster", "SupplierName", SessionHelper.CompanyID, 0);
            ViewBag.CategoryIDBag = objCommon.GetDDData("CategoryMaster", "Category", SessionHelper.CompanyID, 0);
            ViewBag.GLAccountIDBag = objCommon.GetDDData("GLAccountMaster", "GLAccount", SessionHelper.CompanyID, 0);

            ViewBag.UOMIDBag = objCommon.GetDDData("UnitMaster", "Unit", SessionHelper.CompanyID, 0);
            ViewBag.CostUOMBag = objCommon.GetDDData("CostUOMMaster", "CostUOM", SessionHelper.CompanyID, 0);
            ViewBag.ItemTypeBag = GetItemTypeOptions();
            //ViewBag.InventoryClassificationBag = GetInventoryClassificationOptions();
            ViewBag.InventoryClassificationBag = objCommon.GetDDData("InventoryClassificationMaster", "InventoryClassification", SessionHelper.CompanyID, 0);


            if (objDTO.ItemType == 3)
            {
                KitDetailDAL objKitDetailDAL = new KitDetailDAL(SessionHelper.EnterPriseDBName);
                Session["BOMItemKitDetail" + ItemGUID] = objKitDetailDAL.GetAllRecordsByKitGUID(objDTO.GUID, 0, SessionHelper.CompanyID, false, false, true).ToList();
            }

            //Session["BOMItemManufacture"] = obj.GetAllRecordsManuBOM(0, SessionHelper.CompanyID).Where(t => t.ItemGUID == objDTO.GUID).ToList();
            Session["BOMItemManufacture" + ItemGUID] = obj.GetAllRecordsManuBOMForItem(0, SessionHelper.CompanyID, objDTO.GUID).ToList();


            //Session["BOMItemSupplier"] = obj.GetAllRecordsSuppBOM(0, SessionHelper.CompanyID).Where(t => t.ItemGUID == objDTO.GUID).ToList();
            Session["BOMItemSupplier" + ItemGUID] = obj.GetAllRecordsSupplierBOMForItem(0, SessionHelper.CompanyID, objDTO.GUID).ToList();


            bool IsConsignedEditable = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowedToChangeConsignedQuantityItems, eTurnsWeb.Helper.SessionHelper.PermissionType.AllowPull);

            if (!IsConsignedEditable && objDTO.Consignment)
            {
                ViewBag.ViewOnly = true;
            }

            return PartialView("~/Views/BOM/_CreateBomItem.cshtml", objDTO);
        }
        private List<CommonDTO> GetItemTypeOptions()
        {
            List<CommonDTO> ItemType = new List<CommonDTO>();
            ItemType.Add(new CommonDTO() { ID = 1, Text = "Item" });
            ItemType.Add(new CommonDTO() { ID = 3, Text = "Kit" });
            ItemType.Add(new CommonDTO() { ID = 4, Text = "Labor" });

            return ItemType;
        }
        public string get_random_color()
        {
            string[] letters = "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F".Split(',');
            string color = "#";
            for (int i = 0; i < 6; i++)
            {
                color = color + letters[appRandom.Value.Next(15)];
            }
            //Thread.Sleep(1000);
            return color;
        }


        [HttpPost]
        public JsonResult ItemSave(BOMItemDTO objDTO, string DestinationModule, double? SupplierCatalogQty, Guid? SupplierCatalogOrderGUID)
        {
            string message = "";
            string status = "";
            Int64 TempItemID = 0;
            //ItemMasterController obj = new ItemMasterController();
            BOMItemMasterDAL obj = new BOMItemMasterDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

            if (string.IsNullOrEmpty(objDTO.ItemNumber))
            {
                message = string.Format(ResMessage.Required, ResItemMaster.ItemNumber);
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            long SessionUserId = SessionHelper.UserID;
            var enterpriseId = SessionHelper.EnterPriceID;

            if (objDTO.ItemType != 4)
            {
                bool va = ModelState.IsValid;
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();

                    if (ModelState["MinimumQuantity"].Errors.Count > 0)
                    {
                        return Json(new { Message = ModelState["MinimumQuantity"].Errors[0].ErrorMessage, Status = "Fa" }, JsonRequestBehavior.AllowGet);
                    }
                    //else if (ModelState["DefaultReorderQuantity"].Errors.Count > 0)
                    //{
                    //    return Json(new { Message = ModelState["DefaultReorderQuantity"].Errors[0].ErrorMessage, Status = "Fa" }, JsonRequestBehavior.AllowGet);
                    //}
                    //else
                    //{
                    //    return Json(new { Message = ResMessage.InvalidModel, Status = "Fa" }, JsonRequestBehavior.AllowGet);
                    //}
                }
            }
            List<ItemManufacturerDetailsDTO> lstItemManufactuers = null;
            ItemManufacturerDetailsDAL objItemManufacturerDAL = new ItemManufacturerDetailsDAL(SessionHelper.EnterPriseDBName);
            string itemGUIDString = Convert.ToString(objDTO.GUID);

            if (Session["BOMItemManufacture" + itemGUIDString] != null)
            {
                bool SaveItem = true;
                lstItemManufactuers = ((List<ItemManufacturerDetailsDTO>)Session["BOMItemManufacture" + itemGUIDString]).Where(t => !string.IsNullOrEmpty(t.ManufacturerName) || !string.IsNullOrEmpty(t.ManufacturerNumber)).ToList();
                //foreach (var itembr in lstItemManufactuers)
                //{
                //    bool Result = objItemManufacturerDAL.CheckBOMManufacturerDuplicate(itembr.ManufacturerNumber.Trim(), objDTO.GUID, true);
                //    if (!Result)
                //    {
                //        message += string.Format(ResMessage.DuplicateMessage, ResItemMaster.ManufacturerNumber, itembr.ManufacturerNumber.Trim());
                //        status = "duplicate";
                //        SaveItem = false;
                //    }
                //    else
                //    {
                //    }
                //}
                if (!SaveItem)
                {
                    status = "Fa";
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                }
            }
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;

            //Unit new entry logic
            UnitMasterDAL objUnitMasterDAL = new UnitMasterDAL(SessionHelper.EnterPriseDBName);
            UnitMasterDTO objUnitMasterDTO = objUnitMasterDAL.GetUnitByNamePlain(0, SessionHelper.CompanyID, true, objDTO.Unit);

            if (objUnitMasterDTO == null) // new entry - not matching id and uom
            {
                objUnitMasterDTO = new UnitMasterDTO();
                objUnitMasterDTO.ID = 0;
                objUnitMasterDTO.CompanyID = SessionHelper.CompanyID;
                objUnitMasterDTO.Created = DateTimeUtility.DateTimeNow;
                objUnitMasterDTO.CreatedBy = SessionHelper.UserID;
                objUnitMasterDTO.CreatedByName = SessionHelper.UserName;
                objUnitMasterDTO.IsArchived = false;
                objUnitMasterDTO.IsDeleted = false;
                objUnitMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                objUnitMasterDTO.Room = 0;
                objUnitMasterDTO.RoomName = string.Empty;
                objUnitMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                objUnitMasterDTO.UpdatedByName = SessionHelper.UserName;
                objUnitMasterDTO.Unit = objDTO.Unit;
                objUnitMasterDTO.isForBOM = true;
                objUnitMasterDTO.AddedFrom = "Web";
                objUnitMasterDTO.EditedFrom = "Web";
                objUnitMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                objUnitMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                objUnitMasterDTO.ID = objUnitMasterDAL.Insert(objUnitMasterDTO);
                objDTO.UOMID = objUnitMasterDTO.ID;
            }
            else
            {
                objDTO.UOMID = objUnitMasterDTO.ID;
            }
            //End- Unit entry logic

            //Category new entry logic
            if (!string.IsNullOrEmpty(objDTO.CategoryName))
            {
                CategoryMasterDAL objCategoryMasterDAL = new CategoryMasterDAL(SessionHelper.EnterPriseDBName);
                //CategoryMasterDTO objCategoryMasterDTO = objCategoryMasterDAL.GetRecord(objDTO.CategoryName, 0, SessionHelper.CompanyID, false, false, true);
                CategoryMasterDTO objCategoryMasterDTO = objCategoryMasterDAL.GetSingleCategoryByNameByCompanyIDBOM(objDTO.CategoryName, SessionHelper.CompanyID);
                if (objCategoryMasterDTO == null) // new entry - not matching id and uom
                {
                    objCategoryMasterDTO = new CategoryMasterDTO();
                    objCategoryMasterDTO.ID = 0;
                    objCategoryMasterDTO.CompanyID = SessionHelper.CompanyID;
                    objCategoryMasterDTO.Created = DateTimeUtility.DateTimeNow;
                    objCategoryMasterDTO.CreatedBy = SessionHelper.UserID;
                    objCategoryMasterDTO.CreatedByName = SessionHelper.UserName;
                    objCategoryMasterDTO.IsArchived = false;
                    objCategoryMasterDTO.IsDeleted = false;
                    objCategoryMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                    objCategoryMasterDTO.Room = 0;
                    objCategoryMasterDTO.RoomName = string.Empty;
                    objCategoryMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                    objCategoryMasterDTO.UpdatedByName = SessionHelper.UserName;
                    objCategoryMasterDTO.Category = objDTO.CategoryName;
                    objCategoryMasterDTO.CategoryColor = get_random_color();
                    objCategoryMasterDTO.isForBOM = true;
                    objCategoryMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objCategoryMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objCategoryMasterDTO.AddedFrom = "Web";
                    objCategoryMasterDTO.EditedFrom = "Web";
                    objCategoryMasterDTO.ID = objCategoryMasterDAL.Insert(objCategoryMasterDTO);
                    objDTO.CategoryID = objCategoryMasterDTO.ID;
                }
                else
                {
                    objDTO.CategoryID = objCategoryMasterDTO.ID;
                }
            }
            //End- Category entry logic

            //Save New Entry to Manufacturer Master and set item dto for Default manufacturer
            /*
            List<ItemManufacturerDetailsDTO> lstItemManufactuerEntry = null;
            if (Session["BOMItemManufacture"] != null)
            {
                //lstItemManufactuerEntry = ((List<ItemManufacturerDetailsDTO>)Session["BOMItemManufacture"]).Where(t => !string.IsNullOrEmpty(t.ManufacturerName)).ToList();
                //Wi-1505
                lstItemManufactuerEntry = ((List<ItemManufacturerDetailsDTO>)Session["BOMItemManufacture"]).Where(t => !string.IsNullOrEmpty(t.ManufacturerName) || !string.IsNullOrEmpty(t.ManufacturerNumber)).ToList();

                if (lstItemManufactuerEntry != null && lstItemManufactuerEntry.Count > 0)
                {
                    foreach (var itembr in lstItemManufactuerEntry)
                    {

                        /// - logic for Adding supplier if Newly added...
                        ManufacturerMasterDAL objManuMasterDAL = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName);
                        ManufacturerMasterDTO objManufacturerMasterDTO = new ManufacturerMasterDTO();

                        objManufacturerMasterDTO = objManuMasterDAL.GetRecord(itembr.ManufacturerName, 0, SessionHelper.CompanyID, false, false, true);
                        if (objManufacturerMasterDTO == null && !string.IsNullOrEmpty(itembr.ManufacturerName))
                        {
                            objManufacturerMasterDTO = new ManufacturerMasterDTO();
                            objManufacturerMasterDTO.ID = 0;
                            objManufacturerMasterDTO.Manufacturer = itembr.ManufacturerName;
                            objManufacturerMasterDTO.CompanyID = SessionHelper.CompanyID;
                            objManufacturerMasterDTO.Room = 0;
                            objManufacturerMasterDTO.RoomName = string.Empty;
                            objManufacturerMasterDTO.UpdatedByName = SessionHelper.UserName;
                            objManufacturerMasterDTO.CreatedByName = SessionHelper.UserName;
                            objManufacturerMasterDTO.CreatedBy = SessionHelper.UserID;
                            objManufacturerMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                            objManufacturerMasterDTO.Created = DateTimeUtility.DateTimeNow;
                            objManufacturerMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                            objManufacturerMasterDTO.isForBOM = true;
                            objManufacturerMasterDTO.AddedFrom = "Web";
                            objManufacturerMasterDTO.EditedFrom = "Web";
                            objManufacturerMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objManufacturerMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            objManufacturerMasterDTO.ID = objManuMasterDAL.Insert(objManufacturerMasterDTO);
                            itembr.ManufacturerID = objManufacturerMasterDTO.ID;
                        }

                        if (itembr.IsDefault == true)
                        {
                            objDTO.ManufacturerID = objManufacturerMasterDTO.ID;
                            objDTO.ManufacturerNumber = itembr.ManufacturerNumber;
                        }
                        /// End- logic for Adding supplier if Newly added...
                    }
                }
                else
                {
                    objDTO.ManufacturerID = null;
                    objDTO.ManufacturerNumber = null;
                }

                Session["BOMItemManufacture" ] = lstItemManufactuerEntry;
            }

            ///END - Item Manufacturer
            */

            List<ItemManufacturerDetailsDTO> lstItemManufactuerEntry = null;
            if (Session["BOMItemManufacture" + itemGUIDString] != null)
            {
                //Wi-1505
                lstItemManufactuerEntry = ((List<ItemManufacturerDetailsDTO>)Session["BOMItemManufacture" + itemGUIDString]).Where(t => t.ItemGUID == objDTO.GUID).ToList();
                //lstItemManufactuerEntry = ((List<ItemManufacturerDetailsDTO>)Session["ItemManufacture"]).Where(t => !string.IsNullOrEmpty(t.ManufacturerName)).ToList();
                if (lstItemManufactuerEntry != null && lstItemManufactuerEntry.Count > 0)
                {
                    Int64? DefaultManufacture = null;
                    Dictionary<string, long> dictManufacturer = new Dictionary<string, long>();
                    foreach (var itembr in lstItemManufactuerEntry)
                    {

                        /// - logic for Adding supplier if Newly added...
                        ManufacturerMasterDAL objManuMasterDAL = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName);
                        ManufacturerMasterDTO objManufacturerMasterDTO = new ManufacturerMasterDTO();
                        //if (!string.IsNullOrEmpty(itembr.ManufacturerName))
                        objManufacturerMasterDTO = objManuMasterDAL.GetManufacturerByNameNormal(itembr.ManufacturerName ?? string.Empty, 0, SessionHelper.CompanyID, true);
                        //else
                        //  objManufacturerMasterDTO = objManuMasterDAL.GetRecord(itembr.ManufacturerName, 0, 0, false, false, false);

                        if (objManufacturerMasterDTO == null)
                        {
                            objManufacturerMasterDTO = new ManufacturerMasterDTO();
                            objManufacturerMasterDTO.ID = 0;
                            objManufacturerMasterDTO.Manufacturer = itembr.ManufacturerName ?? string.Empty;
                            objManufacturerMasterDTO.CompanyID = SessionHelper.CompanyID;
                            objManufacturerMasterDTO.Room = 0;
                            objManufacturerMasterDTO.RoomName = SessionHelper.RoomName;
                            objManufacturerMasterDTO.UpdatedByName = SessionHelper.UserName;
                            objManufacturerMasterDTO.CreatedByName = SessionHelper.UserName;
                            objManufacturerMasterDTO.CreatedBy = SessionHelper.UserID;
                            objManufacturerMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                            objManufacturerMasterDTO.Created = DateTimeUtility.DateTimeNow;
                            objManufacturerMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                            objManufacturerMasterDTO.AddedFrom = "Web";
                            objManufacturerMasterDTO.EditedFrom = "Web";
                            objManufacturerMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            objManufacturerMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objManufacturerMasterDTO.ID = objManuMasterDAL.Insert(objManufacturerMasterDTO);
                            itembr.ManufacturerID = objManufacturerMasterDTO.ID;
                        }

                        if (objManufacturerMasterDTO != null && objManufacturerMasterDTO.ID > 0)
                        {
                            if (!dictManufacturer.ContainsKey(itembr.ManufacturerName ?? string.Empty))
                            {
                                dictManufacturer.Add(itembr.ManufacturerName ?? string.Empty, objManufacturerMasterDTO.ID);
                            }
                        }

                        if (itembr.IsDefault == true)
                        {
                            DefaultManufacture = objManufacturerMasterDTO.ID;
                            //    objDTO.ManufacturerNumber = itembr.ManufacturerNumber;
                        }
                        /// End- logic for Adding supplier if Newly added...                    }
                        Session["BOMItemManufacture" + itemGUIDString] = lstItemManufactuerEntry;
                    }
                    var DefaultRecord = lstItemManufactuerEntry.Where(l => (l.IsDefault ?? false) == true && (l.IsDeleted ?? false) == false && (l.IsArchived ?? false) == false && l.ItemGUID == objDTO.GUID).FirstOrDefault();
                    if (DefaultRecord != null)
                    {
                        objDTO.ManufacturerNumber = DefaultRecord.ManufacturerNumber;
                        objDTO.ManufacturerID = DefaultManufacture;
                    }
                    else
                    {
                        var firstManufacturer = lstItemManufactuerEntry.Where(l => (l.IsDeleted ?? false) == false && (l.IsArchived ?? false) == false && l.ItemGUID == objDTO.GUID).OrderBy(l => l.ManufacturerName).ThenBy(l => l.ManufacturerNumber).FirstOrDefault();

                        //if (firstManufacturer != null && !string.IsNullOrEmpty(firstManufacturer.ManufacturerName) && !string.IsNullOrWhiteSpace(firstManufacturer.ManufacturerName)
                        if (firstManufacturer != null && firstManufacturer.CompanyID.GetValueOrDefault(0) > 0 && dictManufacturer.ContainsKey(firstManufacturer.ManufacturerName ?? string.Empty))
                        {
                            objDTO.ManufacturerID = dictManufacturer[firstManufacturer.ManufacturerName ?? string.Empty];
                            objDTO.ManufacturerNumber = firstManufacturer.ManufacturerNumber;
                        }
                        else
                        {
                            objDTO.ManufacturerID = null;
                            objDTO.ManufacturerNumber = null;
                        }
                    }
                }
                else
                {
                    objDTO.ManufacturerID = null;
                    objDTO.ManufacturerNumber = null;
                }
            }
            else
            {
                objDTO.ManufacturerID = null;
                objDTO.ManufacturerNumber = null;
            }

            if (objDTO.ItemType != 4)
            {
                ///Save itemlocationwise quantity to database from the session
                List<ItemSupplierDetailsDTO> lstItemSupplier = null;

                if (Session["BOMItemSupplier" + itemGUIDString] != null)
                {
                    lstItemSupplier = ((List<ItemSupplierDetailsDTO>)Session["BOMItemSupplier" + itemGUIDString]).Where(t => !string.IsNullOrEmpty(t.SupplierName) || !string.IsNullOrEmpty(t.SupplierNumber)).ToList();

                    foreach (var itembr in lstItemSupplier)
                    {
                        itembr.ItemGUID = objDTO.GUID;

                        if (!string.IsNullOrEmpty(itembr.SupplierName))
                        {
                            /// - logic for Adding supplier if Newly added...
                            SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                            SupplierMasterDTO objSupplierMasterDTO = objSupplierMasterDAL.GetSupplierByNamePlain(0, SessionHelper.CompanyID, true, itembr.SupplierName);

                            if (objSupplierMasterDTO == null)
                            {
                                objSupplierMasterDTO = new SupplierMasterDTO();
                                objSupplierMasterDTO.ID = 0;
                                objSupplierMasterDTO.SupplierName = itembr.SupplierName;
                                objSupplierMasterDTO.SupplierColor = get_random_color();
                                objSupplierMasterDTO.CompanyID = SessionHelper.CompanyID;
                                objSupplierMasterDTO.Room = 0;
                                objSupplierMasterDTO.RoomName = string.Empty;
                                objSupplierMasterDTO.UpdatedByName = SessionHelper.UserName;
                                objSupplierMasterDTO.CreatedByName = SessionHelper.UserName;
                                objSupplierMasterDTO.CreatedBy = SessionHelper.UserID;
                                objSupplierMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                                objSupplierMasterDTO.Created = DateTimeUtility.DateTimeNow;
                                objSupplierMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                                objSupplierMasterDTO.EditedFrom = "Web";
                                objSupplierMasterDTO.AddedFrom = "Web";
                                objSupplierMasterDTO.isForBOM = true;
                                objSupplierMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                objSupplierMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                objSupplierMasterDTO.ID = objSupplierMasterDAL.Insert(objSupplierMasterDTO);
                                itembr.SupplierID = objSupplierMasterDTO.ID;
                            }
                            else
                            {
                                if (objSupplierMasterDTO.ID != itembr.ID && itembr.SupplierID == 0)
                                {
                                    itembr.SupplierID = objSupplierMasterDTO.ID;
                                }
                            }
                            if (itembr.IsDefault == true)
                            {
                                objDTO.SupplierID = objSupplierMasterDTO.ID;
                                objDTO.SupplierPartNo = itembr.SupplierNumber;
                            }
                            /// End- logic for Adding supplier if Newly added...
                        }
                    }
                    Session["BOMItemSupplier" + itemGUIDString] = lstItemSupplier;
                }
            }

            if (string.IsNullOrEmpty(objDTO.AddedFrom))
            {
                objDTO.AddedFrom = "Web";
            }
            if (string.IsNullOrEmpty(objDTO.EditedFrom))
            {
                objDTO.EditedFrom = "Web";
            }

            if (objDTO.ID == 0)
            {
                #region "Insert"

                string strOK = obj.BOMDuplicateCheck(0, objDTO.ItemNumber, SessionHelper.CompanyID); //objCDAL.DuplicateCheck(objDTO.ItemNumber, "add", objDTO.ID, "ItemMaster", "ItemNumber",0, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResItemMaster.ItemNumber, objDTO.ItemNumber);  //"BinNumber \"" + objDTO.BinNumber + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    objDTO.CreatedByName = SessionHelper.UserName;
                    objDTO.GUID = Guid.NewGuid();
                    //Rename image if uploaded
                    // objDTO.ItemUniqueNumber = UniqueId.Get();
                    objDTO.DefaultLocation = 0;
                    objDTO.IsBOMItem = true;
                    obj.Insert(objDTO);
                    TempItemID = objDTO.ID;
                    //HttpResponseMessage hrmResult = obj.PutRecord(TempItemID, objDTO);
                    bool ReturnVal = false;// = obj.Edit(objDTO);

                    if (objDTO.ID > 0)
                    {
                        ReturnVal = true;
                    }
                    if (ReturnVal)
                    {
                        message = ResMessage.SaveMessage; //ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // string.Format(ResMessage.SaveErrorMsg, hrmResult.StatusCode); // "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                        status = "fail";
                    }

                    if (status == "ok")
                    {
                        //Kit component
                        if (objDTO.ItemType == 3)
                        {
                            ///Save itemlocationwise quantity to database from the session
                            List<KitDetailDTO> lstKitDetailDTO = null;
                            if (Session["BOMItemKitDetail" + itemGUIDString] != null)
                            {
                                lstKitDetailDTO = ((List<KitDetailDTO>)Session["BOMItemKitDetail" + itemGUIDString]).Where(t => t.ItemGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty).ToList();
                                foreach (var itembr in lstKitDetailDTO)
                                {
                                    itembr.KitGUID = objDTO.GUID;
                                    //  itembr.ItemGUID =  objDTO.GUID;
                                    KitDetailDAL objKitDetailDAL = new KitDetailDAL(SessionHelper.EnterPriseDBName);
                                    if (itembr.ID > 0)
                                    {
                                        itembr.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        itembr.EditedFrom = "Web";
                                        objKitDetailDAL.Edit(itembr, SessionUserId,enterpriseId);
                                    }
                                    else
                                    {
                                        objKitDetailDAL.Insert(itembr, SessionUserId,SessionHelper.EnterPriceID);
                                    }
                                }

                                Session["BOMItemKitDetail" + itemGUIDString] = null;
                            }
                        }

                        ///Save itemlocationwise quantity to database from the session
                        List<ItemManufacturerDetailsDTO> lstItemManufactuer = null;
                        if (Session["BOMItemManufacture" + itemGUIDString] != null)
                        {
                            lstItemManufactuer = ((List<ItemManufacturerDetailsDTO>)Session["BOMItemManufacture" + itemGUIDString]).Where(t => !string.IsNullOrEmpty(t.ManufacturerName) || !string.IsNullOrEmpty(t.ManufacturerNumber)).ToList();

                            foreach (var itembr in lstItemManufactuer)
                            {
                                /// - logic for Adding supplier if Newly added...
                                if (itembr.ManufacturerID == 0)
                                {
                                    ManufacturerMasterDAL objManuMasterDAL = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName);
                                    ManufacturerMasterDTO objManufacturerMasterDTO = new ManufacturerMasterDTO();
                                    //if (!string.IsNullOrEmpty(itembr.ManufacturerName))
                                    objManufacturerMasterDTO = objManuMasterDAL.GetManufacturerByNameNormal(itembr.ManufacturerName, 0, SessionHelper.CompanyID, true);
                                    //else
                                    //objManufacturerMasterDTO = objManuMasterDAL.GetRecord(itembr.ManufacturerName, 0, 0, false, false, false);

                                    if (objManufacturerMasterDTO == null)
                                    {
                                        objManufacturerMasterDTO = new ManufacturerMasterDTO();
                                        objManufacturerMasterDTO.ID = 0;
                                        objManufacturerMasterDTO.Manufacturer = itembr.ManufacturerName ?? string.Empty;
                                        objManufacturerMasterDTO.CompanyID = SessionHelper.CompanyID;
                                        objManufacturerMasterDTO.Room = 0;
                                        objManufacturerMasterDTO.RoomName = SessionHelper.RoomName;
                                        objManufacturerMasterDTO.UpdatedByName = SessionHelper.UserName;
                                        objManufacturerMasterDTO.CreatedByName = SessionHelper.UserName;
                                        objManufacturerMasterDTO.CreatedBy = SessionHelper.UserID;
                                        objManufacturerMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                                        objManufacturerMasterDTO.Created = DateTimeUtility.DateTimeNow;
                                        objManufacturerMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                                        objManufacturerMasterDTO.AddedFrom = "Web";
                                        objManufacturerMasterDTO.EditedFrom = "Web";
                                        objManufacturerMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                        objManufacturerMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        objManufacturerMasterDTO.ID = objManuMasterDAL.Insert(objManufacturerMasterDTO);
                                        itembr.ManufacturerID = objManufacturerMasterDTO.ID;
                                    }

                                    // if (itembr.IsDefault == true)
                                    //{
                                    itembr.ManufacturerID = objManufacturerMasterDTO.ID;
                                    //}
                                }

                                itembr.ItemGUID = objDTO.GUID;
                                ItemManufacturerDetailsDAL objItemLocationLevelQuanityDAL = new ItemManufacturerDetailsDAL(SessionHelper.EnterPriseDBName);
                                if (itembr.ID > 0)
                                {
                                    itembr.EditedFrom = "Web";
                                    itembr.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    itembr.AddedFrom = "Web";
                                    itembr.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                    objItemLocationLevelQuanityDAL.Edit(itembr);
                                }
                                else
                                {
                                    itembr.EditedFrom = "Web";
                                    itembr.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    itembr.AddedFrom = "Web";
                                    itembr.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                    objItemLocationLevelQuanityDAL.Insert(itembr);
                                }
                            }
                            Session["BOMItemManufacture" + itemGUIDString] = null;
                        }

                        if (objDTO.ItemType != 4)
                        {
                            ///Save itemlocationwise quantity to database from the session
                            List<ItemSupplierDetailsDTO> lstItemSupplier = null;
                            if (Session["BOMItemSupplier" + itemGUIDString] != null)
                            {
                                lstItemSupplier = ((List<ItemSupplierDetailsDTO>)Session["BOMItemSupplier" + itemGUIDString]).Where(t => !string.IsNullOrEmpty(t.SupplierName) || !string.IsNullOrEmpty(t.SupplierNumber)).ToList();
                                foreach (var itembr in lstItemSupplier)
                                {
                                    itembr.ItemGUID = objDTO.GUID;

                                    if (!string.IsNullOrEmpty(itembr.SupplierName))
                                    {
                                        /// End- logic for Adding supplier if Newly added...
                                    }

                                    ItemSupplierDetailsDAL objItemSupplierDetailsDAL = new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName);
                                    if (itembr.ID > 0)
                                    {
                                        itembr.EditedFrom = "Web";
                                        itembr.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        itembr.AddedFrom = "Web";
                                        itembr.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                        objItemSupplierDetailsDAL.Edit(itembr);
                                    }
                                    else
                                    {
                                        itembr.EditedFrom = "Web";
                                        itembr.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        itembr.AddedFrom = "Web";
                                        itembr.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                        objItemSupplierDetailsDAL.Insert(itembr);
                                    }
                                    //set default if any
                                    obj.UpdateSupplierDetails(SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);
                                }
                                Session["BOMItemSupplier" + itemGUIDString] = null;
                            }
                        }
                    }
                }
                #endregion
            }
            else
            {
                #region "Edit"

                string strOK = obj.BOMDuplicateCheck(objDTO.ID, objDTO.ItemNumber, SessionHelper.CompanyID); //objCDAL.DuplicateCheck(objDTO.ItemNumber, "edit", objDTO.ID, "ItemMaster", "ItemNumber", 0, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResItemMaster.ItemNumber, objDTO.ItemNumber);  //"BinNumber \"" + objDTO.BinNumber + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    if (objDTO.CreatedBy == null)
                        objDTO.CreatedBy = SessionHelper.UserID;
                    if (objDTO.LastUpdatedBy == null)
                        objDTO.LastUpdatedBy = SessionHelper.UserID;

                    bool ReturnVal = obj.Edit(objDTO);

                    if (ReturnVal)
                    {
                        message = ResMessage.SaveMessage;  //ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                        TempItemID = objDTO.ID;
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); //string.Format(ResMessage.SaveErrorMsg, hrmResult.StatusCode); // "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                        status = "fail";
                    }

                    if (status == "ok")
                    {
                        //Kit component
                        if (objDTO.ItemType == 3)
                        {
                            ///Save itemlocationwise quantity to database from the session
                            List<KitDetailDTO> lstKitDetailDTO = null;
                            if (Session["BOMItemKitDetail" + itemGUIDString] != null)
                            {
                                lstKitDetailDTO = ((List<KitDetailDTO>)Session["BOMItemKitDetail" + itemGUIDString]).Where(t => t.ItemGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty).ToList();
                                if (lstKitDetailDTO.Count > 0)
                                {
                                    string KitIDs = "";
                                    KitDetailDAL objKitDetailDAL = new KitDetailDAL(SessionHelper.EnterPriseDBName);

                                    foreach (var itembr in lstKitDetailDTO)
                                    {
                                        itembr.KitGUID = objDTO.GUID;

                                        if (itembr.ID > 0)
                                        {
                                            itembr.ReceivedOn = DateTimeUtility.DateTimeNow;
                                            itembr.EditedFrom = "Web";
                                            objKitDetailDAL.Edit(itembr, SessionUserId,enterpriseId);
                                        }
                                        else
                                        {
                                            objKitDetailDAL.Insert(itembr, SessionUserId, SessionHelper.EnterPriceID);
                                        }
                                        KitIDs += itembr.ID + ",";
                                    }
                                    //Delete except session record....

                                    objKitDetailDAL.DeleteRecordsExcept(KitIDs, objDTO.GUID, SessionHelper.UserID, SessionHelper.CompanyID);
                                }
                                Session["BOMItemKitDetail" + itemGUIDString] = null;
                            }
                        }

                        /////Save itemlocationwise quantity to database from the session
                        //List<ItemManufacturerDetailsDTO> lstItemManufactuer = null;
                        //if (Session["BOMItemManufacture"] != null)
                        //{
                        //    string ManuIDs = "";
                        //    ItemManufacturerDetailsDAL objItemLocationLevelQuanityDAL = new ItemManufacturerDetailsDAL(SessionHelper.EnterPriseDBName);
                        //    lstItemManufactuer = ((List<ItemManufacturerDetailsDTO>)Session["BOMItemManufacture"]).Where(t => !string.IsNullOrEmpty(t.ManufacturerName) || !string.IsNullOrEmpty(t.ManufacturerNumber)).ToList();
                        //    if (lstItemManufactuer.Count > 0)
                        //    {


                        //        foreach (var itembr in lstItemManufactuer)
                        //        {
                        //            itembr.ItemGUID = objDTO.GUID;

                        //            if (itembr.ID > 0)
                        //            {
                        //                itembr.EditedFrom = "Web";
                        //                itembr.ReceivedOn = DateTimeUtility.DateTimeNow;
                        //                itembr.AddedFrom = "Web";
                        //                itembr.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        //                objItemLocationLevelQuanityDAL.Edit(itembr);
                        //            }
                        //            else
                        //            {
                        //                itembr.EditedFrom = "Web";
                        //                itembr.ReceivedOn = DateTimeUtility.DateTimeNow;
                        //                itembr.AddedFrom = "Web";
                        //                itembr.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        //                objItemLocationLevelQuanityDAL.Insert(itembr);
                        //            }
                        //            ManuIDs += itembr.ID + ",";
                        //        }
                        //        //Delete except session record....

                        //    }

                        //    objItemLocationLevelQuanityDAL.DeleteRecordsExcept(ManuIDs, objDTO.GUID, SessionHelper.UserID, SessionHelper.CompanyID);

                        //    Session["BOMItemManufacture"] = null;
                        //}

                        ///Save itemlocationwise quantity to database from the session
                        List<ItemManufacturerDetailsDTO> lstItemManufactuer = null;
                        if (Session["BOMItemManufacture" + itemGUIDString] != null)
                        {
                            lstItemManufactuer = ((List<ItemManufacturerDetailsDTO>)Session["BOMItemManufacture" + itemGUIDString]).Where(t => !string.IsNullOrEmpty(t.ManufacturerNumber) || !string.IsNullOrEmpty(t.ManufacturerName)).ToList();
                            //if (lstItemManufactuer.Count > 0)
                            //{
                            string ManuIDs = "";
                            ItemManufacturerDetailsDAL objItemLocationLevelQuanityDAL = new ItemManufacturerDetailsDAL(SessionHelper.EnterPriseDBName);
                            foreach (var itembr in lstItemManufactuer)
                            {
                                itembr.ItemGUID = objDTO.GUID;
                                if (itembr.ManufacturerID == 0)
                                {
                                    ManufacturerMasterDAL objManuMasterDAL = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName);
                                    ManufacturerMasterDTO objManufacturerMasterDTO = new ManufacturerMasterDTO();
                                    objManufacturerMasterDTO = objManuMasterDAL.GetManufacturerByNameNormal(itembr.ManufacturerName, 0, SessionHelper.CompanyID, false);
                                    if (objManufacturerMasterDTO != null)
                                    {
                                        itembr.ManufacturerID = objManufacturerMasterDTO.ID;
                                    }
                                }
                                if (itembr.ID > 0)
                                {
                                    itembr.EditedFrom = "Web";
                                    itembr.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    itembr.AddedFrom = "Web";
                                    itembr.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                    objItemLocationLevelQuanityDAL.Edit(itembr);
                                }
                                else
                                {
                                    itembr.EditedFrom = "Web";
                                    itembr.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    itembr.AddedFrom = "Web";
                                    itembr.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                    objItemLocationLevelQuanityDAL.Insert(itembr);
                                }
                                ManuIDs += itembr.ID + ",";
                            }
                            //Delete except session record....
                            objItemLocationLevelQuanityDAL.DeleteItemManufacturerRecordsExcept(ManuIDs, objDTO.GUID, SessionHelper.UserID);
                            //}
                            Session["BOMItemManufacture" + itemGUIDString] = null;
                        }

                        if (objDTO.ItemType != 4)
                        {
                            ///Save itemlocationwise quantity to database from the session
                            List<ItemSupplierDetailsDTO> lstItemSupplier = null;
                            if (Session["BOMItemSupplier" + itemGUIDString] != null)
                            {
                                lstItemSupplier = ((List<ItemSupplierDetailsDTO>)Session["BOMItemSupplier" + itemGUIDString]).Where(t => !string.IsNullOrEmpty(t.SupplierName) || !string.IsNullOrEmpty(t.SupplierNumber)).ToList();

                                if (lstItemSupplier.Count > 0)
                                {
                                    string ManuIDs = "";
                                    ItemSupplierDetailsDAL objItemSupplierDetailsDAL = new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName);
                                    foreach (var itembr in lstItemSupplier)
                                    {
                                        itembr.ItemGUID = objDTO.GUID;

                                        if (itembr.ID > 0)
                                        {
                                            itembr.EditedFrom = "Web";
                                            itembr.ReceivedOn = DateTimeUtility.DateTimeNow;
                                            itembr.AddedFrom = "Web";
                                            itembr.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                            objItemSupplierDetailsDAL.Edit(itembr);
                                        }
                                        else
                                        {
                                            itembr.EditedFrom = "Web";
                                            itembr.ReceivedOn = DateTimeUtility.DateTimeNow;
                                            itembr.AddedFrom = "Web";
                                            itembr.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                            objItemSupplierDetailsDAL.Insert(itembr);
                                        }
                                        ManuIDs += itembr.ID + ",";
                                    }
                                    //Delete except session record....
                                    objItemSupplierDetailsDAL.DeleteItemSupplierRecordsExcept(ManuIDs, objDTO.GUID, SessionHelper.UserID);
                                }
                                Session["BOMItemSupplier" + itemGUIDString] = null;
                            }
                        }

                        List<bomUpdateResp> objList = null;
                        objList = obj.UpdateReferenceBOMItem(objDTO.ID, SessionHelper.UserID);
                        CacheHelper<IEnumerable<ManufacturerMasterDTO>>.InvalidateCache();
                        CacheHelper<IEnumerable<SupplierMasterDTO>>.InvalidateCache();
                        CacheHelper<IEnumerable<ItemSupplierDetailsDTO>>.InvalidateCache();
                        CacheHelper<IEnumerable<ItemManufacturerDetailsDTO>>.InvalidateCache();
                        CacheHelper<IEnumerable<ItemMasterDTO>>.InvalidateCache();
                        CacheHelper<IEnumerable<CategoryMasterDTO>>.InvalidateCache();
                        CacheHelper<IEnumerable<UnitMasterDTO>>.InvalidateCache();
                        //CacheHelper<IEnumerable<GLAccountMasterDTO>>.InvalidateCache();
                        CacheHelper<IEnumerable<InventoryClassificationMasterDTO>>.InvalidateCache();
                        //CacheHelper<IEnumerable<CostUOMMasterDTO>>.InvalidateCache();
                        CacheHelper<IEnumerable<ItemLocationQTYDTO>>.InvalidateCache();
                    }

                }

                #endregion
            }

            return Json(new { Message = message, Status = status, DestinationModule = DestinationModule, ItemID = TempItemID, ItemDTO = objDTO }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ItemMasterListAjax(QuickListJQueryDataTableParamModel param)
        {
            BOMItemMasterDAL obj = new BOMItemMasterDAL(SessionHelper.EnterPriseDBName);
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

            if (sortColumnName == "0" || sortColumnName == "undefined")
                sortColumnName = "ID desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.CompanyID, IsArchived, IsDeleted, SessionHelper.RoomID, "page", RoomDateFormat, CurrentTimeZone);

            var result = from u in DataFromDB
                         select new BOMItemDTO
                         {
                             ID = u.ID,
                             ItemNumber = u.ItemNumber,
                             ManufacturerID = u.ManufacturerID,
                             ManufacturerNumber = u.ManufacturerNumber,
                             ManufacturerName = u.ManufacturerName,
                             SupplierID = u.SupplierID,
                             SupplierPartNo = u.SupplierPartNo,
                             SupplierName = u.SupplierName,
                             UPC = u.UPC,
                             UNSPSC = u.UNSPSC,
                             Description = u.Description,
                             LongDescription = u.LongDescription,
                             CategoryID = u.CategoryID,
                             GLAccountID = u.GLAccountID,
                             UOMID = u.UOMID,
                             PricePerTerm = u.PricePerTerm,
                             CostUOMID = u.CostUOMID,
                             DefaultReorderQuantity = u.DefaultReorderQuantity,
                             DefaultPullQuantity = u.DefaultPullQuantity,
                             Cost = u.Cost,
                             Markup = u.Markup,
                             SellPrice = u.SellPrice,
                             ExtendedCost = u.ExtendedCost,
                             AverageCost = u.AverageCost,
                             LeadTimeInDays = u.LeadTimeInDays,
                             Link1 = u.Link1,
                             Link2 = u.Link2,
                             Trend = u.Trend,
                             Taxable = u.Taxable,
                             Consignment = u.Consignment,
                             StagedQuantity = u.StagedQuantity,
                             InTransitquantity = u.InTransitquantity,
                             OnOrderQuantity = u.OnOrderQuantity,
                             OnReturnQuantity = u.OnReturnQuantity,
                             OnTransferQuantity = u.OnTransferQuantity,
                             SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                             RequisitionedQuantity = u.RequisitionedQuantity,
                             PackingQuantity = u.PackingQuantity,
                             AverageUsage = u.AverageUsage,
                             Turns = u.Turns,
                             OnHandQuantity = u.OnHandQuantity,
                             CriticalQuantity = u.CriticalQuantity,
                             MinimumQuantity = u.MinimumQuantity,
                             MaximumQuantity = u.MaximumQuantity,
                             WeightPerPiece = u.WeightPerPiece,
                             ItemUniqueNumber = u.ItemUniqueNumber,
                             IsPurchase = u.IsPurchase,
                             IsTransfer = u.IsTransfer,
                             DefaultLocation = u.DefaultLocation,
                             DefaultLocationName = u.DefaultLocationName,
                             InventoryClassification = u.InventoryClassification,
                             SerialNumberTracking = u.SerialNumberTracking,
                             LotNumberTracking = u.LotNumberTracking,
                             DateCodeTracking = u.DateCodeTracking,
                             ItemType = u.ItemType,
                             ImagePath = u.ImagePath,
                             UDF1 = u.UDF1,
                             UDF2 = u.UDF2,
                             UDF3 = u.UDF3,
                             UDF4 = u.UDF4,
                             UDF5 = u.UDF5,
                             GUID = u.GUID,
                             Created = u.Created,
                             Updated = u.Updated,
                             CreatedBy = u.CreatedBy,
                             LastUpdatedBy = u.LastUpdatedBy,
                             IsDeleted = u.IsDeleted,
                             IsArchived = u.IsArchived,
                             CompanyID = u.CompanyID,
                             Room = u.Room,
                             CreatedByName = u.CreatedByName,
                             UpdatedByName = u.UpdatedByName,
                             RoomName = u.RoomName,
                             IsLotSerialExpiryCost = u.IsLotSerialExpiryCost,
                             CategoryName = u.CategoryName,
                             Unit = u.Unit,
                             GLAccount = u.GLAccount,
                             IsItemLevelMinMaxQtyRequired = u.IsItemLevelMinMaxQtyRequired,
                             IsBuildBreak = u.IsBuildBreak,
                             BondedInventory = u.BondedInventory,
                             IsEnforceDefaultReorderQuantity = u.IsEnforceDefaultReorderQuantity,
                             InventoryClassificationName = u.InventoryClassificationName,
                             IsBOMItem = u.IsBOMItem,
                             CreatedDate = CommonUtility.ConvertDateByTimeZone(u.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             UpdatedDate = CommonUtility.ConvertDateByTimeZone(u.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             ItemTypeName = u.ItemTypeName,
                             ItemLink2ImageType = u.ItemLink2ImageType,
                             ImageType = u.ImageType,
                             ItemLink2ExternalURL = u.ItemLink2ExternalURL,
                             ItemImageExternalURL = u.ItemImageExternalURL,
                             ItemDocExternalURL = u.ItemDocExternalURL,
                             IsActive = u.IsActive,
                             EnhancedDescription = u.EnhancedDescription,
                             EnrichedProductData = u.EnrichedProductData
                         };
            Session["BinReplanish"] = null;

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);

        }
        public ActionResult BOMItemPOPUPListAjax(QuickListJQueryDataTableParamModel param)
        {
            BOMItemMasterDAL obj = new BOMItemMasterDAL(SessionHelper.EnterPriseDBName);
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
            string CallingFrom = "popup";

            if (Request["CallingFrom"] != null && !string.IsNullOrEmpty(Request["CallingFrom"].ToString()) && Request["CallingFrom"].ToString() == "Kit")
            {
                CallingFrom = "Kit";
            }

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

            Guid QLID = Guid.Empty;
            Guid.TryParse(Convert.ToString(Request["ParentGUID"]), out QLID);
            string itemGuidString = Convert.ToString(QLID);
            int TotalRecordCount = 0;
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.CompanyID, IsArchived, IsDeleted, SessionHelper.RoomID, CallingFrom, RoomDateFormat, CurrentTimeZone);

            if (CallingFrom == "Kit")
            {
                List<KitDetailDTO> objQLItems = null;
                if (Session["BOMItemKitDetail" + itemGuidString] != null)
                {
                    objQLItems = (List<KitDetailDTO>)Session["BOMItemKitDetail" + itemGuidString];
                }


                string ItemGUIDs = "";

                if (objQLItems != null && objQLItems.Count > 0)
                {
                    foreach (var item in objQLItems)
                    {
                        if (!string.IsNullOrEmpty(ItemGUIDs))
                            ItemGUIDs += ",";

                        ItemGUIDs += item.ItemGUID.ToString();
                    }
                }

                if (!string.IsNullOrEmpty(ItemGUIDs))
                {
                    string[] arrGUIDs = ItemGUIDs.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    List<BOMItemDTO> objB = (from x in DataFromDB
                                             where !arrGUIDs.Contains(x.GUID.ToString())
                                             select x).ToList();
                    DataFromDB = objB;
                }
            }

            var result = from u in DataFromDB
                         select new BOMItemDTO
                         {

                             ID = u.ID,
                             ItemNumber = u.ItemNumber,
                             ManufacturerID = u.ManufacturerID,
                             ManufacturerNumber = u.ManufacturerNumber,
                             ManufacturerName = u.ManufacturerName,
                             SupplierID = u.SupplierID,
                             SupplierPartNo = u.SupplierPartNo,
                             SupplierName = u.SupplierName,
                             UPC = u.UPC,
                             UNSPSC = u.UNSPSC,
                             Description = u.Description,
                             LongDescription = u.LongDescription,
                             CategoryID = u.CategoryID,
                             GLAccountID = u.GLAccountID,
                             UOMID = u.UOMID,
                             PricePerTerm = u.PricePerTerm,
                             CostUOMID = u.CostUOMID,
                             DefaultReorderQuantity = u.DefaultReorderQuantity,
                             DefaultPullQuantity = u.DefaultPullQuantity,
                             Cost = u.Cost,
                             Markup = u.Markup,
                             SellPrice = u.SellPrice,
                             ExtendedCost = u.ExtendedCost,
                             AverageCost = u.AverageCost,
                             LeadTimeInDays = u.LeadTimeInDays,
                             Link1 = u.Link1,
                             Link2 = u.Link2,
                             Trend = u.Trend,
                             Taxable = u.Taxable,
                             Consignment = u.Consignment,
                             StagedQuantity = u.StagedQuantity,
                             InTransitquantity = u.InTransitquantity,
                             OnOrderQuantity = u.OnOrderQuantity,
                             OnReturnQuantity = u.OnReturnQuantity,
                             OnTransferQuantity = u.OnTransferQuantity,
                             SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                             RequisitionedQuantity = u.RequisitionedQuantity,
                             PackingQuantity = u.PackingQuantity,
                             AverageUsage = u.AverageUsage,
                             Turns = u.Turns,
                             OnHandQuantity = u.OnHandQuantity,
                             CriticalQuantity = u.CriticalQuantity,
                             MinimumQuantity = u.MinimumQuantity,
                             MaximumQuantity = u.MaximumQuantity,
                             WeightPerPiece = u.WeightPerPiece,
                             ItemUniqueNumber = u.ItemUniqueNumber,
                             IsPurchase = u.IsPurchase,
                             IsTransfer = u.IsTransfer,
                             DefaultLocation = u.DefaultLocation,
                             DefaultLocationName = u.DefaultLocationName,
                             InventoryClassification = u.InventoryClassification,
                             SerialNumberTracking = u.SerialNumberTracking,
                             LotNumberTracking = u.LotNumberTracking,
                             DateCodeTracking = u.DateCodeTracking,
                             ItemType = u.ItemType,
                             ImagePath = u.ImagePath,
                             UDF1 = u.UDF1,
                             UDF2 = u.UDF2,
                             UDF3 = u.UDF3,
                             UDF4 = u.UDF4,
                             UDF5 = u.UDF5,

                             GUID = u.GUID,
                             Created = u.Created,
                             Updated = u.Updated,
                             CreatedBy = u.CreatedBy,
                             LastUpdatedBy = u.LastUpdatedBy,
                             IsDeleted = u.IsDeleted,
                             IsArchived = u.IsArchived,
                             CompanyID = u.CompanyID,
                             Room = u.Room,
                             CreatedByName = u.CreatedByName,
                             UpdatedByName = u.UpdatedByName,
                             RoomName = u.RoomName,
                             IsLotSerialExpiryCost = u.IsLotSerialExpiryCost,
                             CategoryName = u.CategoryName,
                             Unit = u.Unit,
                             GLAccount = u.GLAccount,
                             IsItemLevelMinMaxQtyRequired = u.IsItemLevelMinMaxQtyRequired,
                             IsBuildBreak = u.IsBuildBreak,
                             BondedInventory = u.BondedInventory,
                             IsEnforceDefaultReorderQuantity = u.IsEnforceDefaultReorderQuantity,
                             InventoryClassificationName = u.InventoryClassificationName,
                             IsBOMItem = u.IsBOMItem,
                             CreatedDate = CommonUtility.ConvertDateByTimeZone(u.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             UpdatedDate = CommonUtility.ConvertDateByTimeZone(u.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             EnhancedDescription = u.EnhancedDescription,
                             EnrichedProductData = u.EnrichedProductData
                         };

            Session["BinReplanish"] = null;

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);

        }
        public ActionResult LoadManufaturerofItem(Guid ItemGUID, int? AddCount)
        {
            ViewBag.ItemGUID = ItemGUID;
            string itemGuidString = Convert.ToString(ItemGUID);
            ManufacturerMasterDAL objManufacturerMasterDAL = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.ManufacturerBag = objManufacturerMasterDAL.GetManufacturerByRoomNormal(0, SessionHelper.CompanyID, false, false, true, "ID DESC");
            List<ItemManufacturerDetailsDTO> lstItemManufacture = null;

            if (Session["BOMItemManufacture" + itemGuidString] != null)
            {
                lstItemManufacture = (List<ItemManufacturerDetailsDTO>)Session["BOMItemManufacture" + itemGuidString];
                //Delete blank rows
                //lstItemManufacture.Remove(lstItemManufacture.Where(t => t.GUID == Guid.Empty && t.ManufacturerID == 0).FirstOrDefault());
            }
            else
            {
                lstItemManufacture = new List<ItemManufacturerDetailsDTO>();
            }

            if (AddCount != null && AddCount > 0)
            {
                for (int i = 0; i < AddCount; i++)
                {
                    lstItemManufacture.Add(new ItemManufacturerDetailsDTO() { ID = 0, SessionSr = lstItemManufacture.Count + 1, ItemGUID = ItemGUID, CompanyID = SessionHelper.CompanyID, Updated = DateTimeUtility.DateTimeNow, LastUpdatedBy = SessionHelper.UserID, Created = DateTimeUtility.DateTimeNow, CreatedBy = SessionHelper.UserID });
                }
                Session["BOMItemManufacture" + itemGuidString] = lstItemManufacture;
            }

            return PartialView("~/Views/Inventory/_ItemManufacturer.cshtml", lstItemManufacture.OrderByDescending(t => t.ID).ToList());
        }
        public ActionResult LoadSupplierofItem(Guid ItemGUID, int? AddCount)
        {
            ViewBag.ItemGUID = ItemGUID;
            string itemGuidString = Convert.ToString(ItemGUID);
            SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.SupplierBag = objSupplierMasterDAL.GetSupplierByRoomPlain(0, SessionHelper.CompanyID, true).ToList();
            bool isSupplierInsert = false;
            isSupplierInsert = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.SupplierMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
            ViewBag.CanSupplierInsert = isSupplierInsert;
            List<ItemSupplierDetailsDTO> lstItemSupplier = null;
            if (Session["BOMItemSupplier" + itemGuidString] != null)
            {
                lstItemSupplier = (List<ItemSupplierDetailsDTO>)Session["BOMItemSupplier" + itemGuidString];
            }
            else
            {
                lstItemSupplier = new List<ItemSupplierDetailsDTO>();
            }

            //get default supplier...

            if (AddCount != null && AddCount > 0)
            {
                for (int i = 0; i < AddCount; i++)
                {
                    lstItemSupplier.Add(new ItemSupplierDetailsDTO() { ID = 0, SessionSr = lstItemSupplier.Count + 1, ItemGUID = ItemGUID, CompanyID = SessionHelper.CompanyID, Updated = DateTimeUtility.DateTimeNow, LastUpdatedBy = SessionHelper.UserID, Created = DateTimeUtility.DateTimeNow, CreatedBy = SessionHelper.UserID });
                }
            }

            return PartialView("~/Views/Inventory/_ItemSupplier.cshtml", lstItemSupplier.OrderByDescending(t => t.ID).ToList());
        }
        public JsonResult SavetoSeesionItemSupplier(Int64 ID, Int32 SessionSr, Guid GUID, Guid ITEMGUID, Int64 SupplierID, string SupplierName, string SupplierNumber, bool IsDefault, Nullable<Int64> BlanketPOID)
        {
            string itemGuidString = Convert.ToString(ITEMGUID);
            List<ItemSupplierDetailsDTO> lstBinReplanish = null;

            if (Session["BOMItemSupplier" + itemGuidString] != null)
            {
                lstBinReplanish = (List<ItemSupplierDetailsDTO>)Session["BOMItemSupplier" + itemGuidString];
            }
            else
            {
                lstBinReplanish = new List<ItemSupplierDetailsDTO>();
            }

            if (IsDefault)
            {
                if (lstBinReplanish != null && lstBinReplanish.Count > 0)
                {
                    foreach (var lsitem in lstBinReplanish)
                    {
                        lsitem.IsDefault = false;
                    }
                    Session["BOMItemSupplier" + itemGuidString] = lstBinReplanish;
                }
            }

            if (ID > 0 && SessionSr == 0)
            {
                ItemSupplierDetailsDTO objDTO = lstBinReplanish.Where(t => t.ID == ID).FirstOrDefault();

                if (objDTO != null)
                {
                    lstBinReplanish.Remove(lstBinReplanish.Where(t => t.ID == ID).FirstOrDefault());
                    objDTO.ID = ID;
                    objDTO.SupplierID = SupplierID;
                    objDTO.SupplierName = SupplierName;
                    objDTO.SupplierNumber = SupplierNumber;
                    objDTO.IsDefault = IsDefault;
                    objDTO.BlanketPOID = BlanketPOID;
                    objDTO.ItemGUID = ITEMGUID;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.UpdatedByName = SessionHelper.UserName;
                    objDTO.CreatedByName = SessionHelper.UserName;
                    lstBinReplanish.Add(objDTO);
                }
            }
            else
            {
                if (SessionSr > 0)
                {
                    ItemSupplierDetailsDTO objDTO = lstBinReplanish.Where(t => t.SessionSr == SessionSr).FirstOrDefault();

                    if (objDTO != null)
                    {
                        lstBinReplanish.Remove(lstBinReplanish.Where(t => t.SessionSr == SessionSr).FirstOrDefault());
                        objDTO.ID = ID;
                        objDTO.SupplierID = SupplierID;
                        objDTO.SupplierName = SupplierName;
                        objDTO.SupplierNumber = SupplierNumber;
                        objDTO.IsDefault = IsDefault;
                        objDTO.BlanketPOID = BlanketPOID;
                        objDTO.ItemGUID = ITEMGUID;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.Updated = DateTimeUtility.DateTimeNow;
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.UpdatedByName = SessionHelper.UserName;
                        objDTO.CreatedByName = SessionHelper.UserName;
                        lstBinReplanish.Add(objDTO);
                    }
                    else
                    {
                        objDTO = new ItemSupplierDetailsDTO();
                        objDTO.ID = 0;
                        objDTO.SupplierID = SupplierID;
                        objDTO.SupplierName = SupplierName;
                        objDTO.SupplierNumber = SupplierNumber;
                        objDTO.IsDefault = IsDefault;
                        objDTO.BlanketPOID = BlanketPOID;
                        objDTO.ItemGUID = ITEMGUID;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.Updated = DateTimeUtility.DateTimeNow;
                        objDTO.GUID = GUID;
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.UpdatedByName = SessionHelper.UserName;
                        objDTO.CreatedByName = SessionHelper.UserName;
                        objDTO.SessionSr = lstBinReplanish.Count + 1;
                        lstBinReplanish.Add(objDTO);
                    }
                }
                else
                {
                    ItemSupplierDetailsDTO objDTO = new ItemSupplierDetailsDTO();
                    objDTO.ID = 0;
                    objDTO.SupplierID = SupplierID;
                    objDTO.SupplierName = SupplierName;
                    objDTO.SupplierNumber = SupplierNumber;
                    objDTO.IsDefault = IsDefault;
                    objDTO.BlanketPOID = BlanketPOID;
                    objDTO.ItemGUID = ITEMGUID;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.GUID = GUID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.UpdatedByName = SessionHelper.UserName;
                    objDTO.CreatedByName = SessionHelper.UserName;
                    objDTO.SessionSr = lstBinReplanish.Count + 1;
                    lstBinReplanish.Add(objDTO);
                }
            }
            Session["BOMItemSupplier" + itemGuidString] = lstBinReplanish;
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SavetoSeesionItemManufacture(Int64 ID, Int32 SessionSr, Guid GUID, Guid ITEMGUID, Int64 ManufacturerID, string ManufactureName, string ManufacturerNumber, bool IsDefault)
        {
            List<ItemManufacturerDetailsDTO> lstBinReplanish = null;
            string itemGuidString = Convert.ToString(ITEMGUID);

            if (Session["BOMItemManufacture" + itemGuidString] != null)
            {
                lstBinReplanish = (List<ItemManufacturerDetailsDTO>)Session["BOMItemManufacture" + itemGuidString];
            }
            else
            {
                lstBinReplanish = new List<ItemManufacturerDetailsDTO>();
            }

            if (IsDefault)
            {
                if (lstBinReplanish != null && lstBinReplanish.Count > 0)
                {
                    foreach (var lsitem in lstBinReplanish)
                    {
                        lsitem.IsDefault = false;
                    }

                    Session["BOMItemManufacture" + itemGuidString] = lstBinReplanish;
                }
            }
            if (string.IsNullOrEmpty(ManufactureName) && ManufacturerID == 0)
            {
                ManufacturerMasterDTO objManu = new ManufacturerMasterDTO();
                objManu = new ManufacturerMasterDTO();
                if (objManu != null)
                {
                    ManufactureName = objManu.Manufacturer;
                    ManufacturerID = objManu.ID;
                }
            }
            if (ID > 0 && SessionSr == 0)
            {
                ItemManufacturerDetailsDTO objDTO = lstBinReplanish.Where(t => t.ID == ID).FirstOrDefault();
                if (objDTO != null)
                {
                    lstBinReplanish.Remove(lstBinReplanish.Where(t => t.ID == ID).FirstOrDefault());
                    objDTO.ID = ID;
                    objDTO.ManufacturerID = ManufacturerID;
                    objDTO.ManufacturerName = ManufactureName;
                    objDTO.ManufacturerNumber = ManufacturerNumber;
                    objDTO.IsDefault = IsDefault;
                    objDTO.ItemGUID = ITEMGUID;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.GUID = Guid.NewGuid();

                    objDTO.CompanyID = SessionHelper.CompanyID;

                    objDTO.UpdatedByName = SessionHelper.UserName;
                    objDTO.CreatedByName = SessionHelper.UserName;
                    lstBinReplanish.Add(objDTO);
                }
            }
            else
            {
                if (SessionSr > 0)
                {
                    ItemManufacturerDetailsDTO objDTO = lstBinReplanish.Where(t => t.SessionSr == SessionSr).FirstOrDefault();
                    if (objDTO != null)
                    {
                        lstBinReplanish.Remove(lstBinReplanish.Where(t => t.SessionSr == SessionSr).FirstOrDefault());
                        objDTO.ID = ID;
                        objDTO.ManufacturerID = ManufacturerID;
                        objDTO.ManufacturerName = ManufactureName;
                        objDTO.ManufacturerNumber = ManufacturerNumber;
                        objDTO.IsDefault = IsDefault;
                        objDTO.ItemGUID = ITEMGUID;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.Updated = DateTimeUtility.DateTimeNow;
                        objDTO.GUID = Guid.NewGuid();

                        objDTO.CompanyID = SessionHelper.CompanyID;

                        objDTO.UpdatedByName = SessionHelper.UserName;
                        objDTO.CreatedByName = SessionHelper.UserName;
                        lstBinReplanish.Add(objDTO);
                    }
                    else
                    {
                        objDTO = new ItemManufacturerDetailsDTO();
                        objDTO.ID = 0;

                        objDTO.ManufacturerID = ManufacturerID;
                        objDTO.ManufacturerName = ManufactureName;
                        objDTO.ManufacturerNumber = ManufacturerNumber;
                        objDTO.IsDefault = IsDefault;
                        objDTO.ItemGUID = ITEMGUID;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.Updated = DateTimeUtility.DateTimeNow;
                        objDTO.GUID = GUID;

                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.UpdatedByName = SessionHelper.UserName;
                        objDTO.CreatedByName = SessionHelper.UserName;
                        objDTO.SessionSr = lstBinReplanish.Count + 1;
                        lstBinReplanish.Add(objDTO);
                    }
                }
                else
                {
                    ItemManufacturerDetailsDTO objDTO = new ItemManufacturerDetailsDTO();
                    objDTO.ID = 0;
                    objDTO.ManufacturerID = ManufacturerID;
                    objDTO.ManufacturerName = ManufactureName;
                    objDTO.ManufacturerNumber = ManufacturerNumber;
                    objDTO.IsDefault = IsDefault;
                    objDTO.ItemGUID = ITEMGUID;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.GUID = GUID;

                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.UpdatedByName = SessionHelper.UserName;
                    objDTO.CreatedByName = SessionHelper.UserName;
                    objDTO.SessionSr = lstBinReplanish.Count + 1;
                    lstBinReplanish.Add(objDTO);
                }
            }
            Session["BOMItemManufacture" + itemGuidString] = lstBinReplanish;

            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult checkInventoryclassification(double ID)
        {
            double InvId = 0;
            // CostUOMMasterDAL obj = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName);
            // List<CostUOMMasterDTO> lstUnit = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(t => t.ID == ID).ToList();

            InventoryClassificationMasterDAL obj1 = new InventoryClassificationMasterDAL(SessionHelper.EnterPriseDBName);
            List<InventoryClassificationMasterDTO> lstUnit1 = obj1.GetInventoryClassificationByRoomNormal(0, SessionHelper.CompanyID, true).ToList();
            double CostUomval = ID;
            lstUnit1 = lstUnit1.Where(t => (t.RangeStart ?? 0) <= CostUomval && (t.RangeEnd ?? 0) >= CostUomval).ToList();
            // lstUnit1 = lstUnit1.Where(t => ).ToList();  

            if (lstUnit1 != null && lstUnit1.Any())
            {
                InvId = lstUnit1[0].ID;
            }
            return Json(new { status = InvId.ToString() }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeletetoSeesionItemSupplierSingle(Int64 ID, Int32 SessionSr, Guid GUID, Guid ITEMGUID, Int64 SupplierID, string SupplierNumber, bool IsDefault, Nullable<Int64> BlanketPOID)
        {
            string itemGuidString = Convert.ToString(ITEMGUID);
            List<ItemSupplierDetailsDTO> lstBinReplanish = null;

            if (Session["BOMItemSupplier" + itemGuidString] != null)
            {
                lstBinReplanish = (List<ItemSupplierDetailsDTO>)Session["BOMItemSupplier" + itemGuidString];
            }
            else
            {
                lstBinReplanish = new List<ItemSupplierDetailsDTO>();
            }

            ///Delete the Records......
            if (ID > 0)
            {
                try
                {
                    lstBinReplanish.Remove(lstBinReplanish.Where(t => t.ID == ID).FirstOrDefault());
                    Session["BOMItemSupplier" + itemGuidString] = lstBinReplanish;
                }
                catch
                {
                    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "deleted" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    lstBinReplanish.Remove(lstBinReplanish.Where(t => t.GUID == GUID).FirstOrDefault());
                    Session["BOMItemSupplier" + itemGuidString] = lstBinReplanish;
                }
                catch
                {
                    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "deleted" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult DeletetoSeesionItemManufactureSingle(Int64 ID, Int32 SessionSr, Guid GUID, Guid ITEMGUID, Int64 ManufacturerID, string ManufacturerNumber, bool IsDefault)
        {
            string itemGuidString = Convert.ToString(ITEMGUID);
            List<ItemManufacturerDetailsDTO> lstBinReplanish = null;

            if (Session["BOMItemManufacture" + itemGuidString] != null)
            {
                lstBinReplanish = (List<ItemManufacturerDetailsDTO>)Session["BOMItemManufacture" + itemGuidString];
            }
            else
            {
                lstBinReplanish = new List<ItemManufacturerDetailsDTO>();
            }

            ///Delete the Records......
            if (ID > 0)
            {
                try
                {
                    lstBinReplanish.Remove(lstBinReplanish.Where(t => t.ID == ID).FirstOrDefault());
                    Session["BOMItemManufacture" + itemGuidString] = lstBinReplanish;
                }
                catch
                {
                    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "deleted" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    lstBinReplanish.Remove(lstBinReplanish.Where(t => t.GUID == GUID).FirstOrDefault());
                    Session["BOMItemManufacture" + itemGuidString] = lstBinReplanish;
                }
                catch
                {
                    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "deleted" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetCategory(int maxRows, string name_startsWith)
        {
            CategoryMasterDAL obj = new CategoryMasterDAL(SessionHelper.EnterPriseDBName);
            //List<CategoryMasterDTO> lstUnit = obj.GetAllRecords(0, SessionHelper.CompanyID, false, false, true).Where(t => t.Category.ToLower().Contains(name_startsWith.ToLower().Trim())).Take(maxRows).ToList();
            List<CategoryMasterDTO> lstUnit = obj.GetBOMCategoryListSearch(SessionHelper.CompanyID, (name_startsWith ?? string.Empty).ToLower().Trim()).Take(maxRows).ToList();
            if (lstUnit.Count == 0)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            return Json(lstUnit, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUnits(int maxRows, string name_startsWith)
        {
            UnitMasterDAL obj = new UnitMasterDAL(SessionHelper.EnterPriseDBName);
            List<UnitMasterDTO> lstUnit = obj.GetUnitByNameSearch(name_startsWith, 0, SessionHelper.CompanyID, true).Take(maxRows).ToList();
            if (lstUnit == null || lstUnit.Count == 0)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            return Json(lstUnit, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetManufacturer(int maxRows, string name_startsWith)
        {
            ManufacturerMasterDAL obj = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName);
            List<ManufacturerMasterDTO> lstUnit = obj.GetManufacturerByNameSearch(name_startsWith.ToLower().Trim(), 0, SessionHelper.CompanyID, maxRows, true);
            if (lstUnit.Count == 0)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            return Json(lstUnit, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSupplier(int maxRows, string name_startsWith)
        {
            SupplierMasterDAL obj = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            List<SupplierMasterDTO> lstUnit = new List<SupplierMasterDTO>();

            lstUnit = obj.GetSupplierByNameSearch(name_startsWith,0, SessionHelper.CompanyID, true).OrderBy(x => x.SupplierName).Take(maxRows).ToList();

            if (lstUnit.Count == 0)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            return Json(lstUnit, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNarrowSearchData(bool IsDeleted, bool IsArchived, string OrderStatus)
        {

            CommonDAL objCommonCtrl = new CommonDAL(SessionHelper.EnterPriseDBName);
            var tmpsupplierIds = new List<long>();
            NarrowSearchData objNarrowSearchData = objCommonCtrl.GetNarrowSearchDataFromCache("BOMMasterList", SessionHelper.CompanyID, 0, IsArchived, IsDeleted, string.Empty, tmpsupplierIds);

            return Json(new { Success = true, Message = ResCommon.MsgDataSuccessfullyGet, Data = objNarrowSearchData }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult ClearBomSession()
        {
            Session["lstBomIterms"] = null;
            return Json(true);
        }
        public JsonResult DeleteRecords(string ids)
        {
            try
            {



                ////BOMItemMasterDAL _repository = new BOMItemMasterDAL(SessionHelper.EnterPriseDBName);
                ////return _repository.DeleteRecordsItem(ImportMastersDTO.TableName.ItemMaster.ToString(), ids, SessionHelper.CompanyID, SessionHelper.UserID);

                string response = string.Empty;
                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                response = objCommonDAL.DeleteModulewise(ids, ImportMastersDTO.TableName.BOMItemMaster.ToString(), true, SessionHelper.UserID, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);
                eTurns.DAL.CacheHelper<IEnumerable<ItemMasterDTO>>.InvalidateCache();
                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UnDeleteRecords(string ids)
        {
            try
            {
                string response = string.Empty;
                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                ModuleUnDeleteDTO objModuleUnDeleteDTO = new ModuleUnDeleteDTO();
                objModuleUnDeleteDTO = objCommonDAL.UnDeleteModulewise(ids, ImportMastersDTO.TableName.BOMItemMaster.ToString(), true, SessionHelper.UserID, true,SessionHelper.EnterPriceID, SessionHelper.CompanyID,SessionHelper.RoomID);
                eTurns.DAL.CacheHelper<IEnumerable<ItemMasterDTO>>.InvalidateCache();
                response = objModuleUnDeleteDTO.CommonMessage;
                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ItemHistory(Guid ItemGUID)
        {
            ViewBag.ItemGuid = ItemGUID;
            return PartialView("_BOMItemHistory");
        }

        public ActionResult ItemMaster_ChangeLogListAjax(QuickListJQueryDataTableParamModel param)
        {
            BOMItemMasterDAL obj = new BOMItemMasterDAL(SessionHelper.EnterPriseDBName);
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
            sortColumnName = Convert.ToString(Request["SortingField"]);
            bool IsArchived = bool.Parse(Request["IsArchived"]);
            bool IsDeleted = bool.Parse(Request["IsDeleted"]);
            Guid ItemGuid = Guid.Parse(Request["ItemGuid"]);

            // set the default column sorting here, if first time then required to set 
            if (string.IsNullOrEmpty(sortColumnName) || sortColumnName == "0" || sortColumnName == "undefined")
                sortColumnName = "Updated";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            var DataFromDB = obj.GetBOMPagedRecordsNew_ChnageLog(ItemGuid, param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, 0, SessionHelper.CompanyID, IsArchived, IsDeleted, SessionHelper.UserSupplierIds);

            JsonResult jsresult = Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                //aaData = DataFromDB
                aaData = DataFromDB
            }, JsonRequestBehavior.AllowGet);
            jsresult.MaxJsonLength = int.MaxValue;
            return jsresult;
        }

        #region "Kit Component"

        public ActionResult LoadItemKitModel(string Parentid, string ParentGuid)
        {
            //ItemModelPerameter obj = new ItemModelPerameter()
            //{
            //    AjaxURLAddItemToSession = "~/BOM/AddItemToDetailTableKit/",
            //    PerentID = Parentid,
            //    PerentGUID = ParentGuid,
            //    ModelHeader = "add kit component to kit",
            //    AjaxURLAddMultipleItemToSession = "~/BOM/AddItemToDetailTableKit/",
            //    AjaxURLToFillItemGrid = "~/BOM/GetItemsModelMethodKit/",
            //    CallingFromPageName = "KIT"
            //};

            //return PartialView("ItemMasterModel", obj);
            
            BOMItemMasterDAL obj = new BOMItemMasterDAL(SessionHelper.EnterPriseDBName);
            ItemModelPerameter objItemModelPerameter = new ItemModelPerameter()
            {
                AjaxURLAddItemToSession = "~/BOM/AddItemToDetailTableKit/",
                ModelHeader = ResKitMaster.MsgAddKitComponentToKit, 
                PerentGUID = ParentGuid,
                PerentID = Parentid,
                AjaxURLAddMultipleItemToSession = "~/BOM/AddItemToDetailTableKit/",
                AjaxURLToFillItemGrid = "~/BOM/BOMItemPOPUPListAjax/",
                CallingFromPageName = "Kit"
            };
            ViewBag.ItemGUID = ParentGuid;
            //ViewBag.OrderGUID = OrderGUID;
            //ViewBag.OrderSupplier = OrderSupplier;

            int TotalRecordCount = 0;
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            Session["lstBomIterms"] = obj.GetPagedRecords(0, int.MaxValue, out TotalRecordCount, string.Empty, string.Empty, SessionHelper.CompanyID, false, false, SessionHelper.RoomID, "Kit", RoomDateFormat, CurrentTimeZone);
            return PartialView("_BOMItemModel", objItemModelPerameter);
        }

        public JsonResult AddItemToDetailTableKit(string para)
        {
            string message = "";
            string status = "";
            JavaScriptSerializer s = new JavaScriptSerializer();
            KitDetailDTO[] QLDetails = s.Deserialize<KitDetailDTO[]>(para);
            KitDetailDAL objDAL = new eTurns.DAL.KitDetailDAL(SessionHelper.EnterPriseDBName);
            int sessionsr = 0;
            ItemMasterDAL objItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);

            foreach (KitDetailDTO item in QLDetails)
            {
                item.Room = 0;//SessionHelper.RoomID;
                item.RoomName = string.Empty;//SessionHelper.RoomName;
                item.CreatedBy = SessionHelper.UserID;
                item.CreatedByName = SessionHelper.UserName;
                item.UpdatedByName = SessionHelper.UserName;
                item.LastUpdatedBy = SessionHelper.UserID;
                item.CompanyID = SessionHelper.CompanyID;
                item.LastUpdated = DateTimeUtility.DateTimeNow;
                string kitGuidString = Convert.ToString(item.KitGUID.GetValueOrDefault(Guid.Empty));

                if (!(item.GUID != null && item.GUID != Guid.Empty))
                {
                    item.Created = DateTimeUtility.DateTimeNow;
                    ItemMasterDTO ObjItemDTO = objItemDAL.GetItemWithoutJoins(null, item.ItemGUID);

                    if (ObjItemDTO != null && ObjItemDTO.ItemType != 3)
                    {
                        List<KitDetailDTO> lstBinReplanish = null;
                        if (Session["BOMItemKitDetail" + kitGuidString] != null)
                        {
                            lstBinReplanish = (List<KitDetailDTO>)Session["BOMItemKitDetail" + kitGuidString];
                            item.SessionSr = lstBinReplanish.Count + 1;
                        }
                        else
                        {
                            //sessionsr = sessionsr + 1;
                            item.SessionSr = sessionsr + 1;
                            lstBinReplanish = new List<KitDetailDTO>();
                        }
                        lstBinReplanish.Add(item);
                        Session["BOMItemKitDetail" + kitGuidString] = lstBinReplanish;
                        // }
                    }
                }
            }

            message = ResAssetMaster.MsgItemQtyUpdated;
            status = "ok";
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult SaveKitQty(Guid ItemGUID, Guid KitItemGuid, double? QuantityPerKit)
        {
            string itemGuidString = Convert.ToString(ItemGUID);
            List<KitDetailDTO> lstKitDetailDTO = (List<KitDetailDTO>)Session["BOMItemKitDetail" + itemGuidString];

            if (lstKitDetailDTO != null && lstKitDetailDTO.Count > 0)
            {
                KitDetailDTO objKitDetailDTO = lstKitDetailDTO.Where(t => t.ItemGUID == KitItemGuid).FirstOrDefault();
                if (objKitDetailDTO != null)
                {
                    objKitDetailDTO.QuantityPerKit = QuantityPerKit;
                }
                lstKitDetailDTO = lstKitDetailDTO.Where(t => t.ItemGUID != KitItemGuid).ToList();
                lstKitDetailDTO.Add(objKitDetailDTO);
                Session["BOMItemKitDetail" + itemGuidString] = lstKitDetailDTO;
            }
            return Json(true);

        }


        //LoadKitofItem
        public ActionResult LoadKitComponentofItem(Guid ItemGUID, int? AddCount)
        {
            ViewBag.ItemGUID = ItemGUID;
            List<KitDetailDTO> lstKitDetailDTO = null;
            string itemGuidString = Convert.ToString(ItemGUID);

            if (Session["BOMItemKitDetail" + itemGuidString] != null)
            {
                lstKitDetailDTO = (List<KitDetailDTO>)Session["BOMItemKitDetail" + itemGuidString];
                //Delete blank rows
                lstKitDetailDTO.Remove(lstKitDetailDTO.Where(t => t.GUID == Guid.Empty && t.ItemGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty).FirstOrDefault());
            }
            else
            {
                lstKitDetailDTO = new List<KitDetailDTO>();
            }

            if (AddCount != null && AddCount > 0)
            {
                for (int i = 0; i < AddCount; i++)
                {
                    lstKitDetailDTO.Add(new KitDetailDTO() { ID = 0, SessionSr = lstKitDetailDTO.Count + 1, KitGUID = ItemGUID, Room = SessionHelper.RoomID, CompanyID = SessionHelper.CompanyID, LastUpdated = DateTimeUtility.DateTimeNow, LastUpdatedBy = SessionHelper.UserID, Created = DateTimeUtility.DateTimeNow, CreatedBy = SessionHelper.UserID });
                }
            }
            return PartialView("_BOMItemKitComponent", lstKitDetailDTO.OrderBy(t => t.ItemNumber).ToList());
        }

        public JsonResult SavetoSeesionItemKitComponent(Int64 ID, Int32 SessionSr, Guid GUID, Guid ITEMGUID, Guid KitGUID, double QuantityPerKit)
        {
            string itemGuidString = Convert.ToString(KitGUID);
            List<KitDetailDTO> lstBinReplanish = null;

            if (Session["BOMItemKitDetail" + itemGuidString] != null)
            {
                lstBinReplanish = (List<KitDetailDTO>)Session["BOMItemKitDetail" + itemGuidString];
            }
            else
            {
                lstBinReplanish = new List<KitDetailDTO>();
            }

            if (ID > 0 && SessionSr == 0)
            {
                KitDetailDTO objDTO = lstBinReplanish.Where(t => t.GUID == GUID).FirstOrDefault();
                if (objDTO != null)
                {
                    lstBinReplanish.Remove(lstBinReplanish.Where(t => t.GUID == GUID).FirstOrDefault());
                    objDTO.ID = ID;
                    objDTO.KitGUID = KitGUID;
                    objDTO.ItemGUID = ITEMGUID;
                    objDTO.QuantityPerKit = QuantityPerKit;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objDTO.GUID = GUID;//Guid.NewGuid();
                    objDTO.Room = SessionHelper.RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.RoomName = SessionHelper.RoomName;
                    objDTO.UpdatedByName = SessionHelper.UserName;
                    objDTO.CreatedByName = SessionHelper.UserName;
                    lstBinReplanish.Add(objDTO);
                }
            }
            else
            {
                if (SessionSr > 0)
                {
                    KitDetailDTO objDTO = lstBinReplanish.Where(t => t.SessionSr == SessionSr).FirstOrDefault();
                    if (objDTO != null)
                    {
                        lstBinReplanish.Remove(lstBinReplanish.Where(t => t.SessionSr == SessionSr).FirstOrDefault());
                        objDTO.ID = ID;
                        objDTO.KitGUID = KitGUID;
                        objDTO.ItemGUID = ITEMGUID;
                        objDTO.QuantityPerKit = QuantityPerKit;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.Room = SessionHelper.RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.RoomName = SessionHelper.RoomName;
                        objDTO.UpdatedByName = SessionHelper.UserName;
                        objDTO.CreatedByName = SessionHelper.UserName;
                        lstBinReplanish.Add(objDTO);
                    }
                    else
                    {
                        objDTO = new KitDetailDTO();
                        objDTO.ID = 0;
                        objDTO.KitGUID = KitGUID;
                        objDTO.ItemGUID = ITEMGUID;
                        objDTO.QuantityPerKit = QuantityPerKit;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        objDTO.GUID = GUID;
                        objDTO.Room = SessionHelper.RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.RoomName = SessionHelper.RoomName;
                        objDTO.UpdatedByName = SessionHelper.UserName;
                        objDTO.CreatedByName = SessionHelper.UserName;
                        objDTO.SessionSr = lstBinReplanish.Count + 1;
                        lstBinReplanish.Add(objDTO);
                    }
                }
                else
                {
                    KitDetailDTO objDTO = new KitDetailDTO();
                    objDTO.ID = 0;
                    objDTO.KitGUID = KitGUID;
                    objDTO.ItemGUID = ITEMGUID;
                    objDTO.QuantityPerKit = QuantityPerKit;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objDTO.GUID = GUID;
                    objDTO.Room = SessionHelper.RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.RoomName = SessionHelper.RoomName;
                    objDTO.UpdatedByName = SessionHelper.UserName;
                    objDTO.CreatedByName = SessionHelper.UserName;
                    objDTO.SessionSr = lstBinReplanish.Count + 1;
                    lstBinReplanish.Add(objDTO);
                }
            }
            Session["BOMItemKitDetail" + itemGuidString] = lstBinReplanish;

            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeletetoSeesionItemKitComponentSingle(Int64 ID, Int32 SessionSr, Guid GUID, Guid ITEMGUID, Guid KitGUID, double QuantityPerKit)
        {
            string itemGuidString = Convert.ToString(KitGUID);
            List<KitDetailDTO> lstBinReplanish = null;

            if (Session["BOMItemKitDetail" + itemGuidString] != null)
            {
                lstBinReplanish = (List<KitDetailDTO>)Session["BOMItemKitDetail" + itemGuidString];
            }
            else
            {
                lstBinReplanish = new List<KitDetailDTO>();
            }

            ///Delete the Records......
            if (ID > 0)
            {
                try
                {

                    //check the kit deletable logic if it is allow or nots
                    KitDetailDAL objKitDetailDAL = new KitDetailDAL(SessionHelper.EnterPriseDBName);
                    if (objKitDetailDAL.IsKitItemDeletable(GUID.ToString(), 0, SessionHelper.CompanyID))
                    {
                        lstBinReplanish.Remove(lstBinReplanish.Where(t => t.ID == ID).FirstOrDefault());

                        Session["BOMItemKitDetail" + itemGuidString] = lstBinReplanish;
                    }
                    else
                    {
                        return Json(new { status = "reference" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch
                {
                    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "deleted" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    if (GUID == Guid.Empty && ITEMGUID != Guid.Empty)
                    {
                        lstBinReplanish.Remove(lstBinReplanish.Where(t => t.ItemGUID == ITEMGUID && t.SessionSr == SessionSr).FirstOrDefault());
                    }
                    else
                    {
                        lstBinReplanish.Remove(lstBinReplanish.Where(t => t.GUID == GUID && t.SessionSr == SessionSr).FirstOrDefault());
                    }

                    Session["BOMItemKitDetail" + itemGuidString] = lstBinReplanish;
                }
                catch
                {
                    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "deleted" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region [BOM ItemList]

        public ActionResult BOMItemMasterList()
        {
            return View("BOMItemMasterListIM");
        }

        #endregion

    }
}
