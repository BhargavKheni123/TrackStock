using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsWeb.Helper;
using eTurnsWeb.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using eTurnsWeb.BAL;
using eTurns.DTO.Helper;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Configuration;
using Newtonsoft.Json;

namespace eTurnsWeb.Controllers
{
    [AuthorizeHelper]
    public partial class InventoryController : eTurnsControllerBase
    {
        #region [Item CRUD]

        public ActionResult ItemMasterList()
        {
            //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));
            string ItemDetailsNew = SiteSettingHelper.ItemDetailsNew; //Settinfile.Element("ItemDetailsNew").Value;
            Session["ItemMasterList"] = null;

            if ((ItemDetailsNew ?? string.Empty) == "yes")
            {
                return View("ItemMasterListIM");
            }
            else
            {
                return View();
            }
        }
        public ActionResult ItemMasterListAll()
        {
            //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));
            string ItemDetailsNew = SiteSettingHelper.ItemDetailsNew; //Settinfile.Element("ItemDetailsNew").Value;
            Session["ItemMasterList"] = null;

            if ((ItemDetailsNew ?? string.Empty) == "yes")
            {
                return View("ItemMasterListIM");
            }
            else
            {
                return View();
            }
        }
        public ActionResult ItemCreate(SupplierCatalogItemDTO SupplierCatalogItem)
        {
            ItemMasterDTO objDTO = new ItemMasterDTO();
            objDTO.ID = 0;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.CreatedByName = SessionHelper.UserName;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.RoomName = SessionHelper.RoomName;
            objDTO.GUID = Guid.NewGuid();
            objDTO.Trend = false;
            objDTO.IsAutoInventoryClassification = false;
            objDTO.Taxable = false;
            objDTO.Consignment = false;
            objDTO.IsTransfer = false;
            objDTO.IsPurchase = true;
            objDTO.ItemType = 1;
            objDTO.SerialNumberTracking = false;
            objDTO.LotNumberTracking = false;
            objDTO.DateCodeTracking = false;
            objDTO.Unit = "";
            objDTO.Cost = 0;
            objDTO.IsActive = true;
            objDTO.IsAllowOrderCostuom = false;
            objDTO.IsOrderable = true;

            string itemGuidString = Convert.ToString(objDTO.GUID);
            Session["BinReplanish" + itemGuidString] = null;
            Session["ItemManufacture" + itemGuidString] = null;
            Session["ItemSupplier" + itemGuidString] = null;
            Session["ItemKitDetail" + itemGuidString] = null;

            //get room's configuration status for replanishment type and consignment
            RoomDAL objRoomDal = new RoomDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            //RoomDTO ROOMDTO = objRoomDal.GetRoomByIDPlain(SessionHelper.RoomID);
            string columnList = "ID,RoomName,MethodOfValuingInventory,GlobMarkupParts,GlobMarkupLabor,IsConsignment,DefaultSupplierID,DefaultBinName,ReplenishmentType,IsAllowOrderCostuom";
            RoomDTO ROOMDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");


            if (ROOMDTO != null)
            {
                ViewBag.MethodOfValuingInventory = ROOMDTO.MethodOfValuingInventory;

                if (ROOMDTO.GlobMarkupParts.GetValueOrDefault(0) > 0)
                {
                    objDTO.Markup = ROOMDTO.GlobMarkupParts.GetValueOrDefault(0);
                    objDTO.RoomGlobMarkupParts = ROOMDTO.GlobMarkupParts.GetValueOrDefault(0);
                }
                else
                    objDTO.RoomGlobMarkupParts = 0;

                if (ROOMDTO.GlobMarkupLabor.GetValueOrDefault(0) > 0)
                    objDTO.RoomGlobMarkupLabor = ROOMDTO.GlobMarkupLabor.GetValueOrDefault(0);
                else
                    objDTO.RoomGlobMarkupLabor = 0;
                if (ROOMDTO.IsConsignment)
                {
                    objDTO.Consignment = true;
                }
                ////////////////////////////////default supplier logic/////////////////////////////////////////
                List<ItemSupplierDetailsDTO> lstItemSupplier = new List<ItemSupplierDetailsDTO>();
                if (ROOMDTO.DefaultSupplierID > 0)
                {
                    objDTO.SupplierID = ROOMDTO.DefaultSupplierID.GetValueOrDefault(0);

                    //get supplier name from supplier id
                    SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                    SupplierMasterDTO objSupplierMasterDTO = new SupplierMasterDTO();

                    if (SessionHelper.UserSupplierIds != null && SessionHelper.UserSupplierIds.Any())
                    {
                        string strSupplierIds = string.Empty;
                        strSupplierIds = string.Join(",", SessionHelper.UserSupplierIds);
                        var suppliers = objSupplierMasterDAL.GetSupplierByIDsPlain(strSupplierIds, SessionHelper.RoomID, SessionHelper.CompanyID);

                        if (suppliers != null && suppliers.Any())
                        {
                            var tmpSuppliers = suppliers.Where(e => e.ID == ROOMDTO.DefaultSupplierID.GetValueOrDefault(0)).FirstOrDefault();
                            if (!(tmpSuppliers != null && tmpSuppliers.ID > 0))
                            {
                                tmpSuppliers = suppliers.OrderBy(e => e.SupplierName).FirstOrDefault();
                            }

                            lstItemSupplier.Add(new ItemSupplierDetailsDTO()
                            {
                                ID = 0,
                                SupplierName = tmpSuppliers.SupplierName,
                                SupplierID = tmpSuppliers.ID,
                                IsDefault = true,
                                SessionSr = lstItemSupplier.Count + 1,
                                ItemGUID = objDTO.GUID,
                                Room = SessionHelper.RoomID,
                                CompanyID = SessionHelper.CompanyID,
                                Updated = DateTimeUtility.DateTimeNow,
                                LastUpdatedBy = SessionHelper.UserID,
                                Created = DateTimeUtility.DateTimeNow,
                                CreatedBy = SessionHelper.UserID
                            });

                        }
                    }
                    else
                    {
                        objSupplierMasterDTO = objSupplierMasterDAL.GetSupplierByIDPlain(ROOMDTO.DefaultSupplierID.GetValueOrDefault(0));
                        lstItemSupplier.Add(new ItemSupplierDetailsDTO()
                        {
                            ID = 0,
                            SupplierName = objSupplierMasterDTO.SupplierName,
                            SupplierID = ROOMDTO.DefaultSupplierID.GetValueOrDefault(0),
                            IsDefault = true,
                            SessionSr = lstItemSupplier.Count + 1,
                            ItemGUID = objDTO.GUID,
                            Room = SessionHelper.RoomID,
                            CompanyID = SessionHelper.CompanyID,
                            Updated = DateTimeUtility.DateTimeNow,
                            LastUpdatedBy = SessionHelper.UserID,
                            Created = DateTimeUtility.DateTimeNow,
                            CreatedBy = SessionHelper.UserID
                        });
                    }
                }

                Session["ItemSupplier" + itemGuidString] = lstItemSupplier;
                ///////////////////////////////////////////////////////////////////

                if (ROOMDTO.DefaultBinName != null && ROOMDTO.DefaultBinName.Length > 0)
                {
                    objDTO.DefaultLocation = 0;
                    objDTO.DefaultLocationName = ROOMDTO.DefaultBinName;
                }
                else
                {
                    objDTO.DefaultLocation = 0;
                    objDTO.DefaultLocationName = "";
                }
                //Replenishment Type
                if (ROOMDTO.ReplenishmentType != null)
                {
                    if (ROOMDTO.ReplenishmentType == "3")
                    {
                        ViewBag.LockReplenishmentType = false;
                        objDTO.IsItemLevelMinMaxQtyRequired = true;
                    }
                    else
                    {
                        ViewBag.LockReplenishmentType = true;
                        if (ROOMDTO.ReplenishmentType == "1")
                        {
                            objDTO.IsItemLevelMinMaxQtyRequired = true;
                        }
                        else
                        {
                            objDTO.IsItemLevelMinMaxQtyRequired = false;
                        }
                    }
                }
                else
                {
                    objDTO.IsItemLevelMinMaxQtyRequired = true;
                    ViewBag.LockReplenishmentType = true;
                }

                //consignemtn
                if (ROOMDTO.IsConsignment)
                {
                    ViewBag.LockConsignment = false;
                }
                else
                {
                    ViewBag.LockConsignment = true;
                }
                if (ROOMDTO.IsAllowOrderCostuom)
                {
                    ViewBag.LockIsAllowOrderCostuom = false;
                }
                else
                {
                    ViewBag.LockIsAllowOrderCostuom = true;
                }
            }
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
            else if (SupplierCatalogItem != null && !string.IsNullOrEmpty(SupplierCatalogItem.OrderSupplier) && SupplierCatalogItem.OrderGUID != Guid.Empty)
            {
                ViewBag.DestinationModule = "OrderItemPopup";
            }

            UDFController objUDFController = new UDFController();
            ViewBag.UDFs = objUDFController.GetUDFDataPageWise("ItemMaster");

            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }

            CommonDAL objCommon = new CommonDAL(SessionHelper.EnterPriseDBName);

            var ddData = objCommon.GetDDDataAll(SessionHelper.CompanyID, SessionHelper.RoomID);

            ViewBag.ManufacturerIDBag = ddData.Where(x => x.Source == "Manufacturer").ToList();
            ViewBag.SupplierIDBag = ddData.Where(x => x.Source == "SupplierName").ToList();
            ViewBag.CategoryIDBag = ddData.Where(x => x.Source == "Category").ToList();
            ViewBag.GLAccountIDBag = ddData.Where(x => x.Source == "GLAccount").ToList();
            ViewBag.UOMIDBag = ddData.Where(x => x.Source == "Unit").ToList();
            ViewBag.OrderUOMBag = ddData.Where(x => x.Source == "OrderUOM").ToList();
            ViewBag.InventoryClassificationBag = ddData.Where(x => x.Source == "InventoryClassification").ToList();
            ViewBag.DefaultLocationBag = ddData.Where(x => x.Source == "BinNumber").ToList();
            ViewBag.ItemTypeBag = GetItemTypeOptions();
            ViewBag.TrendingSettingBag = GetTrendingSettings();
            ViewBag.CostUOMBag = objCommon.GetDDDataWithValue("CostUOMMaster", "CostUOM", SessionHelper.CompanyID, SessionHelper.RoomID, "CostUOMValue");
            objDTO.IsOnlyFromItemUI = true;
            objDTO.DefaultReorderQuantity = 1;
            objDTO.DefaultPullQuantity = 1;
            CostUOMMasterDAL oCostUOMDal = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName);

            try
            {
                CostUOMMasterDTO oCostUOMMaster = oCostUOMDal.GetCostUOMByName("E", SessionHelper.RoomID, SessionHelper.CompanyID);
                objDTO.CostUOMID = oCostUOMMaster.ID;
                objDTO.CostUOMName = oCostUOMMaster.CostUOM;
            }
            catch { }

            OrderUOMMasterDAL oOrderUOMDal = new OrderUOMMasterDAL(SessionHelper.EnterPriseDBName);
            try
            {
                OrderUOMMasterDTO oOrderUOMMaster = oOrderUOMDal.GetRecord("E", SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                objDTO.OrderUOMID = oOrderUOMMaster.ID;
                objDTO.OrderUOMName = oOrderUOMMaster.OrderUOM;
            }
            catch { }

            UnitMasterDAL objUnitDal = new UnitMasterDAL(SessionHelper.EnterPriseDBName);
            UnitMasterDTO objUnit = objUnitDal.GetUnitByNamePlain(SessionHelper.RoomID, SessionHelper.CompanyID, false, "EA");

            if (objUnit != null && objUnit.ID > 0)
            {
                objDTO.UOMID = objUnit.ID;
                objDTO.Unit = objUnit.Unit;
            }

            if (!string.IsNullOrWhiteSpace(objDTO.ItemImageExternalURL))
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(objDTO.ItemImageExternalURL);
                    request.Method = "GET";
                    try
                    {
                        WebResponse response = request.GetResponse();
                        if (response.ContentType.ToString().ToLower().IndexOf("image") >= 0)
                        {

                        }
                        else
                        {
                            objDTO.ItemImageExternalURL = string.Empty;
                        }
                    }
                    catch
                    {
                        objDTO.ItemImageExternalURL = string.Empty;
                    }
                }
                catch
                {
                    objDTO.ItemImageExternalURL = string.Empty;
                }
            }

            objDTO.ID = 0;
            objDTO.ImageType = "ExternalImage";
            objDTO.ItemLink2ImageType = "InternalLink";
            string ViewName = "_CreateItem";
            //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));
            string ItemDetailsNew = SiteSettingHelper.ItemDetailsChange; // Settinfile.Element("ItemDetailsChange").Value;
            ViewBag.IsRoomELabelEnable = objRoomDal.GetRoomByIDFull(SessionHelper.RoomID).IsELabel;
            if (ItemDetailsNew == "new")
            {
                ViewName = "_CreateItemDetails";
            }

            return PartialView(ViewName, objDTO);
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
            }

            if (IsDeleted || IsArchived || IsHistory)
            {
                ViewBag.ViewOnly = true;
            }

            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            ItemMasterDTO objDTO = obj.GetItemWithMasterTableJoins(null, Guid.Parse(ItemGUID), SessionHelper.RoomID, SessionHelper.CompanyID);

            if (objDTO != null)
            {
                objDTO.CreatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                objDTO.UpdatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);

                #region Check is item added from amazon
                objDTO.IsItemAddedFromAB = false;
                if (objDTO.ID > 0 && !string.IsNullOrWhiteSpace(objDTO.SupplierPartNo))
                {
                    ProductDetailsDAL objProductDetailsDAL = new ProductDetailsDAL(SessionHelper.EnterPriseDBName);
                    Int64 ABItemMappingID = objProductDetailsDAL.CheckItemAddedFromAB(objDTO.SupplierPartNo, objDTO.GUID, SessionHelper.CompanyID, SessionHelper.RoomID);
                    if (ABItemMappingID > 0)
                    {
                        objDTO.IsItemAddedFromAB = true;
                    }
                }
                #endregion

            }
            UDFController objUDFController = new UDFController();
            ViewBag.UDFs = objUDFController.GetUDFDataPageWise("ItemMaster");

            if (objDTO != null)
            {
                ViewData["UDF1"] = objDTO.UDF1;
                ViewData["UDF2"] = objDTO.UDF2;
                ViewData["UDF3"] = objDTO.UDF3;
                ViewData["UDF4"] = objDTO.UDF4;
                ViewData["UDF5"] = objDTO.UDF5;
                ViewData["UDF6"] = objDTO.UDF6;
                ViewData["UDF7"] = objDTO.UDF7;
                ViewData["UDF8"] = objDTO.UDF8;
                ViewData["UDF9"] = objDTO.UDF9;
                ViewData["UDF10"] = objDTO.UDF10;
            }

            CommonDAL objCommon = new CommonDAL(SessionHelper.EnterPriseDBName);

            var ddData = objCommon.GetDDDataAll(SessionHelper.CompanyID, SessionHelper.RoomID);

            ViewBag.ManufacturerIDBag = ddData.Where(x => x.Source == "Manufacturer").ToList();
            ViewBag.SupplierIDBag = ddData.Where(x => x.Source == "SupplierName").ToList();
            ViewBag.CategoryIDBag = ddData.Where(x => x.Source == "Category").ToList();
            ViewBag.GLAccountIDBag = ddData.Where(x => x.Source == "GLAccount").ToList();
            ViewBag.UOMIDBag = ddData.Where(x => x.Source == "Unit").ToList();
            ViewBag.OrderUOMBag = ddData.Where(x => x.Source == "OrderUOM").ToList();
            ViewBag.InventoryClassificationBag = ddData.Where(x => x.Source == "InventoryClassification").ToList();
            ViewBag.DefaultLocationBag = ddData.Where(x => x.Source == "BinNumber").ToList();

            ViewBag.CostUOMBag = objCommon.GetDDDataWithValue("CostUOMMaster", "CostUOM", SessionHelper.CompanyID, SessionHelper.RoomID, "CostUOMValue");
            ViewBag.ItemTypeBag = GetItemTypeOptions();
            ViewBag.TrendingSettingBag = GetTrendingSettings();
            
            if (objDTO == null)
            {
                objDTO = new ItemMasterDTO();
                ViewBag.LockReplenishmentType = false;
            }

            if (objDTO != null)
            {
                if (objDTO.IsItemLevelMinMaxQtyRequired == null)
                {
                    objDTO.IsItemLevelMinMaxQtyRequired = false;
                }

                objDTO.Unit = "";
                objDTO.IsOnlyFromItemUI = true;

                //Edit mode --- Bin Replanish fill in to session....
                if (objDTO.ItemType != 4)
                {
                    BinMasterDAL objItemLocationLevelQuanityDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                    List<BinMasterDTO> lstBinReplanish = objItemLocationLevelQuanityDAL.GetItemLocations(objDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    lstBinReplanish = lstBinReplanish.Where(t => t.IsStagingLocation == false).ToList();
                    for (int i = 0; i < lstBinReplanish.Count(); i++)
                    {
                        if (lstBinReplanish[i].ID == objDTO.DefaultLocation)
                            lstBinReplanish[i].IsDefault = true;
                        else
                            lstBinReplanish[i].IsDefault = false;
                    }
                    Session["BinReplanish" + ItemGUID] = lstBinReplanish;
                }

                if (objDTO.ItemType == 3)
                {
                    KitDetailDAL objKitDetailDAL = new KitDetailDAL(SessionHelper.EnterPriseDBName);
                    Session["ItemKitDetail" + ItemGUID] = objKitDetailDAL.GetAllRecordsByKitGUID(objDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, true).ToList();
                }

                ItemManufacturerDetailsDAL objItemManufacturerDetailsDAL = new ItemManufacturerDetailsDAL(SessionHelper.EnterPriseDBName);
                Session["ItemManufacture" + ItemGUID] = objItemManufacturerDetailsDAL.GetManufacturerByItemGuidNormal(SessionHelper.RoomID, SessionHelper.CompanyID, objDTO.GUID, false);

                ItemSupplierDetailsDAL objItemSupplierDAL = new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName);
                Session["ItemSupplier" + ItemGUID] = objItemSupplierDAL.GetSuppliersByItemGuidNormal(SessionHelper.RoomID, SessionHelper.CompanyID, objDTO.GUID);

            }

            RoomDAL objRoomDal = new RoomDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            //RoomDTO ROOMDTO = objRoomDal.GetRoomByIDPlain(SessionHelper.RoomID);
            string columnList = "ID,RoomName,MethodOfValuingInventory,GlobMarkupParts,GlobMarkupLabor,IsConsignment,IsAllowOrderCostuom";
            RoomDTO ROOMDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");

            if (ROOMDTO != null)
            {
                if (objDTO != null)
                {
                    if (ROOMDTO.GlobMarkupParts.GetValueOrDefault(0) > 0)
                    {
                        objDTO.RoomGlobMarkupParts = ROOMDTO.GlobMarkupParts.GetValueOrDefault(0);
                    }
                    else
                        objDTO.RoomGlobMarkupParts = 0;

                    if (ROOMDTO.GlobMarkupLabor.GetValueOrDefault(0) > 0)
                        objDTO.RoomGlobMarkupLabor = ROOMDTO.GlobMarkupLabor.GetValueOrDefault(0);
                    else
                        objDTO.RoomGlobMarkupLabor = 0;
                }

                if (ROOMDTO.IsConsignment)
                {
                    ViewBag.LockConsignment = false;
                }
                else
                {
                    ViewBag.LockConsignment = true;
                }

                if (ROOMDTO.IsAllowOrderCostuom)
                {
                    ViewBag.LockIsAllowOrderCostuom = false;
                }
                else
                {
                    ViewBag.LockIsAllowOrderCostuom = true;
                }

                ViewBag.MethodOfValuingInventory = ROOMDTO.MethodOfValuingInventory;
            }

            bool IsConsignedEditable = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowChangeConsignedItems);

            if (!IsConsignedEditable && objDTO != null && objDTO.Consignment)
            {
                ViewBag.ViewOnly = true;
            }

            if (objDTO != null && objDTO.IsAutoInventoryClassification)
            {
                //if (objDTO.Turns != null || objDTO.ExtendedCost != null)
                //{
                int Icost = checkInventoryclassificationTurn(objDTO.Turns.GetValueOrDefault(0), objDTO.ExtendedCost.GetValueOrDefault(0));
                if (Icost > 0)
                {
                    objDTO.InventoryClassification = Icost;
                }
                //}
            }

            if (objDTO != null && !string.IsNullOrWhiteSpace(objDTO.ItemImageExternalURL))
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(objDTO.ItemImageExternalURL);
                    request.Method = "GET";
                    try
                    {
                        WebResponse response = request.GetResponse();
                        if (response.ContentType.ToString().ToLower().IndexOf("image") >= 0 || response.ContentType.ToString().ToLower().IndexOf("text/html") >= 0)
                        {

                        }
                        else
                        {
                            objDTO.ItemImageExternalURL = string.Empty;
                        }
                    }
                    catch
                    {
                        objDTO.ItemImageExternalURL = string.Empty;
                    }
                }
                catch
                {
                    objDTO.ItemImageExternalURL = string.Empty;
                }
            }



            string ViewName = "_CreateItem";
            //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));
            string ItemDetailsNew = SiteSettingHelper.ItemDetailsChange;  //Settinfile.Element("ItemDetailsChange").Value;

            if (objDTO != null)
            {
                ViewBag.IsItemInUsed = objDTO.IsItemInUse;
                ViewBag.IsRoomELabelEnable = objDTO.IsELabel;
            }
            else
            {
                ViewBag.IsItemInUsed = obj.checkItemInUsed(objDTO.GUID);
                ViewBag.IsRoomELabelEnable = objRoomDal.GetRoomByIDFull(SessionHelper.RoomID).IsELabel;
            }

            if (ItemDetailsNew == "new")
            {
                ViewName = "_CreateItemDetails";
            }

            return PartialView(ViewName, objDTO);
        }

        public ActionResult ItemEditByRoomAndCompany(string ItemGUID, string RoomId, string CompanyId)
        {
            bool IsArchived = false;
            bool IsDeleted = false;
            bool IsHistory = false;
            int RoomID = Convert.ToInt32(RoomId);
            int CompanyID = Convert.ToInt32(CompanyId);

            if (Request["IsHistory"] != null && Request["IsHistory"].ToString() != "")
                IsHistory = bool.Parse(Request["IsHistory"].ToString());

            if (!string.IsNullOrEmpty(Request["IsArchived"]) && !string.IsNullOrEmpty(Request["IsDeleted"]))
            {
                IsArchived = bool.Parse(Request["IsArchived"].ToString());
                IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            }

            if (IsDeleted || IsArchived || IsHistory)
            {
                ViewBag.ViewOnly = true;
            }

            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            ItemMasterDTO objDTO = obj.GetItemWithMasterTableJoins(null, Guid.Parse(ItemGUID), RoomID, CompanyID);

            if (objDTO != null)
            {
                objDTO.CreatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                objDTO.UpdatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);

                #region Check is item added from amazon
                objDTO.IsItemAddedFromAB = false;
                if (objDTO.ID > 0 && !string.IsNullOrWhiteSpace(objDTO.SupplierPartNo))
                {
                    ProductDetailsDAL objProductDetailsDAL = new ProductDetailsDAL(SessionHelper.EnterPriseDBName);
                    Int64 ABItemMappingID = objProductDetailsDAL.CheckItemAddedFromAB(objDTO.SupplierPartNo, objDTO.GUID, CompanyID, RoomID);
                    if (ABItemMappingID > 0)
                    {
                        objDTO.IsItemAddedFromAB = true;
                    }
                }
                #endregion

            }
            UDFController objUDFController = new UDFController();
            ViewBag.UDFs = objUDFController.GetUDFDataPageAndRoomCompanyWise("ItemMaster", RoomID, CompanyID);

            if (objDTO != null)
            {
                ViewData["UDF1"] = objDTO.UDF1;
                ViewData["UDF2"] = objDTO.UDF2;
                ViewData["UDF3"] = objDTO.UDF3;
                ViewData["UDF4"] = objDTO.UDF4;
                ViewData["UDF5"] = objDTO.UDF5;
                ViewData["UDF6"] = objDTO.UDF6;
                ViewData["UDF7"] = objDTO.UDF7;
                ViewData["UDF8"] = objDTO.UDF8;
                ViewData["UDF9"] = objDTO.UDF9;
                ViewData["UDF10"] = objDTO.UDF10;
            }

            CommonDAL objCommon = new CommonDAL(SessionHelper.EnterPriseDBName);
            ViewBag.ManufacturerIDBag = objCommon.GetDDData("ManufacturerMaster", "Manufacturer", CompanyID, RoomID);
            ViewBag.SupplierIDBag = objCommon.GetDDData("SupplierMaster", "SupplierName", CompanyID, RoomID);
            ViewBag.CategoryIDBag = objCommon.GetDDData("CategoryMaster", "Category", CompanyID, RoomID);
            ViewBag.GLAccountIDBag = objCommon.GetDDData("GLAccountMaster", "GLAccount", CompanyID, RoomID, false);
            ViewBag.CostUOMBag = objCommon.GetDDDataWithValue("CostUOMMaster", "CostUOM", CompanyID, RoomID, "CostUOMValue");
            ViewBag.OrderUOMBag = objCommon.GetDDData("OrderUOMMaster", "OrderUOM", CompanyID, RoomID);
            ViewBag.UOMIDBag = objCommon.GetDDData("UnitMaster", "Unit", CompanyID, RoomID);
            ViewBag.ItemTypeBag = GetItemTypeOptions();
            ViewBag.TrendingSettingBag = GetTrendingSettings();
            ViewBag.InventoryClassificationBag = objCommon.GetDDData("InventoryClassificationMaster", "InventoryClassification", CompanyID, RoomID, false);
            ViewBag.DefaultLocationBag = objCommon.GetDDData("BinMaster", "BinNumber", " IsStagingLocation != 1 AND ", CompanyID, RoomID);

            if (objDTO == null)
            {
                objDTO = new ItemMasterDTO();
                ViewBag.LockReplenishmentType = false;
            }

            if (objDTO != null)
            {
                if (objDTO.IsItemLevelMinMaxQtyRequired == null)
                {
                    objDTO.IsItemLevelMinMaxQtyRequired = false;
                }

                objDTO.Unit = "";
                objDTO.IsOnlyFromItemUI = true;

                //Edit mode --- Bin Replanish fill in to session....
                if (objDTO.ItemType != 4)
                {
                    BinMasterDAL objItemLocationLevelQuanityDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                    List<BinMasterDTO> lstBinReplanish = objItemLocationLevelQuanityDAL.GetItemLocations(objDTO.GUID, RoomID, CompanyID);
                    lstBinReplanish = lstBinReplanish.Where(t => t.IsStagingLocation == false).ToList();
                    for (int i = 0; i < lstBinReplanish.Count(); i++)
                    {
                        if (lstBinReplanish[i].ID == objDTO.DefaultLocation)
                            lstBinReplanish[i].IsDefault = true;
                        else
                            lstBinReplanish[i].IsDefault = false;
                    }
                    Session["BinReplanish" + ItemGUID] = lstBinReplanish;
                }

                if (objDTO.ItemType == 3)
                {
                    KitDetailDAL objKitDetailDAL = new KitDetailDAL(SessionHelper.EnterPriseDBName);
                    Session["ItemKitDetail" + ItemGUID] = objKitDetailDAL.GetAllRecordsByKitGUID(objDTO.GUID, RoomID, CompanyID, false, false, true).ToList();
                }

                ItemManufacturerDetailsDAL objItemManufacturerDetailsDAL = new ItemManufacturerDetailsDAL(SessionHelper.EnterPriseDBName);
                Session["ItemManufacture" + ItemGUID] = objItemManufacturerDetailsDAL.GetManufacturerByItemGuidNormal(RoomID, CompanyID, objDTO.GUID, false);

                ItemSupplierDetailsDAL objItemSupplierDAL = new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName);
                Session["ItemSupplier" + ItemGUID] = objItemSupplierDAL.GetSuppliersByItemGuidNormal(RoomID, CompanyID, objDTO.GUID);

            }

            RoomDAL objRoomDal = new RoomDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            //RoomDTO ROOMDTO = objRoomDal.GetRoomByIDPlain(SessionHelper.RoomID);
            string columnList = "ID,RoomName,MethodOfValuingInventory,GlobMarkupParts,GlobMarkupLabor,IsConsignment,IsAllowOrderCostuom";
            RoomDTO ROOMDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");

            if (ROOMDTO != null)
            {
                if (objDTO != null)
                {
                    if (ROOMDTO.GlobMarkupParts.GetValueOrDefault(0) > 0)
                    {
                        objDTO.RoomGlobMarkupParts = ROOMDTO.GlobMarkupParts.GetValueOrDefault(0);
                    }
                    else
                        objDTO.RoomGlobMarkupParts = 0;

                    if (ROOMDTO.GlobMarkupLabor.GetValueOrDefault(0) > 0)
                        objDTO.RoomGlobMarkupLabor = ROOMDTO.GlobMarkupLabor.GetValueOrDefault(0);
                    else
                        objDTO.RoomGlobMarkupLabor = 0;
                }

                if (ROOMDTO.IsConsignment)
                {
                    ViewBag.LockConsignment = false;
                }
                else
                {
                    ViewBag.LockConsignment = true;
                }

                if (ROOMDTO.IsAllowOrderCostuom)
                {
                    ViewBag.LockIsAllowOrderCostuom = false;
                }
                else
                {
                    ViewBag.LockIsAllowOrderCostuom = true;
                }

                ViewBag.MethodOfValuingInventory = ROOMDTO.MethodOfValuingInventory;
            }

            bool IsConsignedEditable = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowChangeConsignedItems);

            if (!IsConsignedEditable && objDTO != null && objDTO.Consignment)
            {
                ViewBag.ViewOnly = true;
            }

            if (objDTO != null && objDTO.IsAutoInventoryClassification)
            {
                //if (objDTO.Turns != null || objDTO.ExtendedCost != null)
                //{
                int Icost = checkInventoryclassificationTurn(objDTO.Turns.GetValueOrDefault(0), objDTO.ExtendedCost.GetValueOrDefault(0));
                if (Icost > 0)
                {
                    objDTO.InventoryClassification = Icost;
                }
                //}
            }

            if (objDTO != null && !string.IsNullOrWhiteSpace(objDTO.ItemImageExternalURL))
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(objDTO.ItemImageExternalURL);
                    request.Method = "GET";
                    try
                    {
                        WebResponse response = request.GetResponse();
                        if (response.ContentType.ToString().ToLower().IndexOf("image") >= 0)
                        {

                        }
                        else
                        {
                            objDTO.ItemImageExternalURL = string.Empty;
                        }
                    }
                    catch
                    {
                        objDTO.ItemImageExternalURL = string.Empty;
                    }
                }
                catch
                {
                    objDTO.ItemImageExternalURL = string.Empty;
                }
            }



            string ViewName = "_CreateItem";
            //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));
            string ItemDetailsNew = SiteSettingHelper.ItemDetailsChange;  //Settinfile.Element("ItemDetailsChange").Value;
            if (ItemDetailsNew == "new")
            {
                ViewName = "_CreateItemDetails";
            }

            return PartialView(ViewName, objDTO);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult ItemSave(ItemMasterDTO objDTO, string DestinationModule, double? SupplierCatalogQty, Guid? SupplierCatalogOrderGUID)
        {
            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            List<string> uniques = new List<string>();
            string SolumMappedLabels = string.Empty;
            string SolumUnmappedLabels = string.Empty;
            if (objDTO.SolumMappedLabels != null)
            {
                uniques = objDTO.SolumMappedLabels.Split(',').Distinct().ToList();
                SolumMappedLabels = string.Join(",", uniques);
            }
            if (objDTO.SolumUnMappedLabels != null)
            {
                uniques = objDTO.SolumUnMappedLabels.Split(',').Distinct().ToList();
                SolumUnmappedLabels = string.Join(",", uniques);
            }
           
            if (objDTO.ID > 0 && objDTO.IsOrderable == false)
            {
                bool Res = true;
                string Result = obj.CheckItemExistsForInActive(objDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, ref Res, SessionHelper.EnterPriceID, ResourceHelper.CurrentCult.Name);
                if (!Res)
                {
                    return Json(new { Message = ResMessage.ActiveFailMessage, Status = "Fa", ErrorMessage = Result }, JsonRequestBehavior.AllowGet);
                }
            }
            long SessionUserId = SessionHelper.UserID;
            var enterpriseId = SessionHelper.EnterPriceID;
            string itemGUIDString = Convert.ToString(objDTO.GUID);
            string CostUOMValue = string.Empty;
            string defaultSupplierPartNo = string.Empty;
            CostUOMMasterDAL objCostUOMDAL = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName);

            if (Session["BinReplanish" + itemGUIDString] != null)
            {
                Session["SaveBinCreatedDate"] = obj.GetDBUTCTime(); //DateTimeUtility.DateTimeNow.AddMinutes(-1); //Note: difference in db utc time and c# utc time was causing issue in display bin change history warning that's why DB utc time is used at both the end.
            }
            else
            {
                Session["SaveBinCreatedDate"] = null;
            }

            if (objDTO.ItemType != 4) // if labour in that case no need to check modal validation as it always invalid
            {
                if (objDTO.IsItemLevelMinMaxQtyRequired == true)
                {
                    bool va = ModelState.IsValid;
                    if (!ModelState.IsValid)
                    {
                        var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();

                        if (ModelState["CriticalQuantity"].Errors.Count > 0)
                        {
                            return Json(new { Message = ModelState["CriticalQuantity"].Errors[0].ErrorMessage, Status = "Fa" }, JsonRequestBehavior.AllowGet);
                        }
                        else if (ModelState["MinimumQuantity"].Errors.Count > 0)
                        {
                            return Json(new { Message = ModelState["MinimumQuantity"].Errors[0].ErrorMessage, Status = "Fa" }, JsonRequestBehavior.AllowGet);
                        }
                        else if (ModelState["DefaultReorderQuantity"].Errors.Count > 0)
                        {
                            return Json(new { Message = ModelState["DefaultReorderQuantity"].Errors[0].ErrorMessage, Status = "Fa" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { Message = ResMessage.InvalidModel, Status = "Fa" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(objDTO.ItemImageExternalURL))
            {
                bool Isvalid = CommonUtility.ValidateExternalImage(objDTO.ItemImageExternalURL);
                if (!Isvalid)
                    return Json(new { Message = ResItemMaster.InvalidImageURL, Status = "Fa" }, JsonRequestBehavior.AllowGet);
            }
            
            if (!string.IsNullOrWhiteSpace(objDTO.ItemLink2ExternalURL))
            {
                bool Isvalid = CommonUtility.ValidateExternalImage(objDTO.ItemLink2ExternalURL);
                if (!Isvalid)
                    return Json(new { Message = ResItemMaster.MsgInvalidURlForLink, Status = "Fa" }, JsonRequestBehavior.AllowGet);
            }

            string message = "";
            string status = "";
            Int64 TempItemID = 0;
            CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

            if (string.IsNullOrEmpty(objDTO.ItemNumber))
            {
                message = string.Format(ResMessage.Required, ResItemMaster.ItemNumber);
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }

            if (objDTO.ItemType != 4)
            {
                ItemSupplierDetailsDAL objItemSupplierDetailsDAL = new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName);
            }

            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.EnhancedDescription = (objDTO.EnhancedDescription == null ? "" : objDTO.EnhancedDescription);

            //Category new entry logic
            if (!string.IsNullOrEmpty(objDTO.CategoryName))
            {
                CategoryMasterDAL objCategoryMasterDAL = new CategoryMasterDAL(SessionHelper.EnterPriseDBName);
                //CategoryMasterDTO objCategoryMasterDTO = objCategoryMasterDAL.GetRecord(objDTO.CategoryName, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false);
                CategoryMasterDTO objCategoryMasterDTO = objCategoryMasterDAL.GetSingleCategoryByNameByRoomID(objDTO.CategoryName, SessionHelper.RoomID, SessionHelper.CompanyID);
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
                    objCategoryMasterDTO.Room = SessionHelper.RoomID;
                    objCategoryMasterDTO.RoomName = SessionHelper.RoomName;
                    objCategoryMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                    objCategoryMasterDTO.UpdatedByName = SessionHelper.UserName;
                    objCategoryMasterDTO.Category = objDTO.CategoryName;
                    objCategoryMasterDTO.CategoryColor = get_random_color();
                    objCategoryMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objCategoryMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objCategoryMasterDTO.AddedFrom = "Web";
                    objCategoryMasterDTO.EditedFrom = "Web";
                    objCategoryMasterDTO.ID = objCategoryMasterDAL.Insert(objCategoryMasterDTO);
                    objDTO.CategoryID = objCategoryMasterDTO.ID;
                }
                else { objDTO.CategoryID = objCategoryMasterDTO.ID; }
            }
            //End- Category entry logic
            // if serial number item then round the quantity fields to zero
            if (objDTO.SerialNumberTracking)
            {
                if (objDTO.IsItemLevelMinMaxQtyRequired == true)
                {
                    objDTO.MinimumQuantity = Math.Round(objDTO.MinimumQuantity.GetValueOrDefault(0), 0);
                    objDTO.MaximumQuantity = Math.Round(objDTO.MaximumQuantity.GetValueOrDefault(0), 0);
                    objDTO.CriticalQuantity = Math.Round(objDTO.CriticalQuantity ?? 0, 0);
                }
                objDTO.DefaultPullQuantity = Math.Round(objDTO.DefaultPullQuantity.GetValueOrDefault(0), 0);
                objDTO.DefaultReorderQuantity = Math.Round(objDTO.DefaultReorderQuantity.GetValueOrDefault(0), 0);
            }

            //Save New Entry to Manufacturer Master and set item dto for Default manufacturer
            List<ItemManufacturerDetailsDTO> lstItemManufactuerEntry = null;
            if (Session["ItemManufacture" + itemGUIDString] != null)
            {
                //Wi-1505
                lstItemManufactuerEntry = ((List<ItemManufacturerDetailsDTO>)Session["ItemManufacture" + itemGUIDString]).Where(t => t.ItemGUID == objDTO.GUID).ToList();
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
                        objManufacturerMasterDTO = objManuMasterDAL.GetManufacturerByNameNormal(itembr.ManufacturerName ?? string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, false);
                        //else
                        //  objManufacturerMasterDTO = objManuMasterDAL.GetRecord(itembr.ManufacturerName, 0, 0, false, false, false);

                        if (objManufacturerMasterDTO == null)
                        {
                            objManufacturerMasterDTO = new ManufacturerMasterDTO();
                            objManufacturerMasterDTO.ID = 0;
                            objManufacturerMasterDTO.Manufacturer = itembr.ManufacturerName ?? string.Empty;
                            objManufacturerMasterDTO.CompanyID = SessionHelper.CompanyID;
                            objManufacturerMasterDTO.Room = SessionHelper.RoomID;
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
                        //Session["ItemManufacture" + itemGUIDString] = lstItemManufactuerEntry;
                    }

                    Session["ItemManufacture" + itemGUIDString] = lstItemManufactuerEntry;

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
                        if (firstManufacturer != null && firstManufacturer.Room.GetValueOrDefault(0) > 0 && dictManufacturer.ContainsKey(firstManufacturer.ManufacturerName ?? string.Empty))
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
               // eTurns.DAL.CacheHelper<IEnumerable<ManufacturerMasterDTO>>.AddCacheItem("Cached_ManufacturerMasterDTO_" + SessionHelper.CompanyID.ToString() + "_" + SessionHelper.RoomID, new List<ManufacturerMasterDTO>());
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
                if (Session["ItemSupplier" + itemGUIDString] != null)
                {
                    lstItemSupplier = ((List<ItemSupplierDetailsDTO>)Session["ItemSupplier" + itemGUIDString]).Where(t => !string.IsNullOrEmpty(t.SupplierName) || !string.IsNullOrEmpty(t.SupplierNumber)).ToList();
                    foreach (var itembr in lstItemSupplier)
                    {
                        itembr.ItemGUID = objDTO.GUID;

                        if (!string.IsNullOrEmpty(itembr.SupplierName))
                        {
                            /// - logic for Adding supplier if Newly added...
                            SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                            SupplierMasterDTO objSupplierMasterDTO = objSupplierMasterDAL.GetSupplierByNamePlain(SessionHelper.RoomID, SessionHelper.CompanyID, false, itembr.SupplierName);

                            if (objSupplierMasterDTO == null)
                            {
                                objSupplierMasterDTO = new SupplierMasterDTO();
                                objSupplierMasterDTO.ID = 0;
                                objSupplierMasterDTO.SupplierName = itembr.SupplierName;
                                objSupplierMasterDTO.SupplierColor = get_random_color();
                                objSupplierMasterDTO.CompanyID = SessionHelper.CompanyID;
                                objSupplierMasterDTO.Room = SessionHelper.RoomID;
                                objSupplierMasterDTO.RoomName = SessionHelper.RoomName;
                                objSupplierMasterDTO.UpdatedByName = SessionHelper.UserName;
                                objSupplierMasterDTO.CreatedByName = SessionHelper.UserName;
                                objSupplierMasterDTO.CreatedBy = SessionHelper.UserID;
                                objSupplierMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                                objSupplierMasterDTO.Created = DateTimeUtility.DateTimeNow;
                                objSupplierMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                                objSupplierMasterDTO.AddedFrom = "Web";
                                objSupplierMasterDTO.EditedFrom = "Web";
                                objSupplierMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                objSupplierMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                objSupplierMasterDTO.ID = objSupplierMasterDAL.Insert(objSupplierMasterDTO);
                                itembr.SupplierID = objSupplierMasterDTO.ID;

                                if (itembr.SupplierID > 0)
                                {
                                    // insert into Room Schedule for PullSchedule type is Immediate 
                                    SchedulerDTO objPullSchedulerDTO = new SchedulerDTO();
                                    objPullSchedulerDTO.SupplierId = itembr.SupplierID;
                                    objPullSchedulerDTO.CompanyId = itembr.CompanyID.GetValueOrDefault(0);
                                    objPullSchedulerDTO.RoomId = itembr.Room.GetValueOrDefault(0);
                                    objPullSchedulerDTO.LoadSheduleFor = 7;
                                    objPullSchedulerDTO.ScheduleMode = 5;
                                    objPullSchedulerDTO.IsScheduleActive = true;
                                    objPullSchedulerDTO.MonthlyDayOfMonth = 2;
                                    objSupplierMasterDAL.SaveSupplierSchedule(objPullSchedulerDTO);
                                }
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
                    Session["ItemSupplier" + itemGUIDString] = lstItemSupplier;
                  //  eTurns.DAL.CacheHelper<IEnumerable<SupplierMasterDTO>>.AddCacheItem("Cached_SupplierMasterDTO_" + SessionHelper.CompanyID.ToString() + "_" + SessionHelper.RoomID, new List<SupplierMasterDTO>());
                }
            }

            DashboardParameterDTO objDashboardParameterDTO = new DashboardDAL(SessionHelper.EnterPriseDBName).GetDashboardParameters(SessionHelper.RoomID, SessionHelper.CompanyID);

            if (objDashboardParameterDTO == null || objDashboardParameterDTO.ID <= 0)
            {
                objDashboardParameterDTO = new DashboardParameterDTO();
            }

            if (!objDashboardParameterDTO.IsTrendingEnabled)
            {
                objDTO.TrendingSetting = 0;
            }

            if (objDTO.ID == 0)
            {
                #region "Insert"

                //string strOK = objCDAL.CheckItemDuplication(objDTO.ItemNumber, "add", objDTO.ID, "ItemMaster", "ItemNumber", SessionHelper.RoomID, SessionHelper.CompanyID,objDTO.ItemType);
                string strOK = objCDAL.DuplicateCheck(objDTO.ItemNumber, "add", objDTO.ID, "ItemMaster", "ItemNumber", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResItemMaster.ItemNumber, objDTO.ItemNumber);  //"BinNumber \"" + objDTO.BinNumber + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    if (objDTO.IsAllowOrderCostuom)
                    {
                        objDTO.IsEnforceDefaultReorderQuantity = true;

                        /*
                        bool blFlag = objCDAL.CheckItemCostUOM(Convert.ToInt64(objDTO.CostUOMID), Convert.ToInt64(objDTO.DefaultReorderQuantity));
                        if (blFlag == false)
                        {
                            message = string.Format(ResItemMaster.CostUOMReorderQTY, ResItemMaster.DefaultReorderQuantity, objDTO.ItemNumber);
                            status = "Fa";
                            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                        }*/


                        //CostUOMMasterDAL objCostUOMDAL = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName);
                        CostUOMMasterDTO CostUOMDTO = objCostUOMDAL.GetCostUOMByID(objDTO.CostUOMID.GetValueOrDefault(0));
                        if (CostUOMDTO != null && CostUOMDTO.ID > 0)
                        {

                            //int calcDefaultReorderQuantity = 0;

                            //CostUOMValue = CostUOMDTO.CostUOMValue.ToString();
                            if ((CostUOMDTO.CostUOMValue ?? 1) > Convert.ToInt32(objDTO.DefaultReorderQuantity ?? 1))
                            {
                                objDTO.DefaultReorderQuantity = Convert.ToDouble(CostUOMDTO.CostUOMValue ?? 1);
                                /*calcDefaultReorderQuantity = (CostUOMDTO.CostUOMValue ?? 1);
                                message = ResMessage.ReorderQtyAsPerCUOM;
                                status = "fail";
                                return Json(new { Message = message, Status = status, calcDefaultReorderQuantity = calcDefaultReorderQuantity }, JsonRequestBehavior.AllowGet);
                                */

                            }
                            else
                            {
                                if ((Convert.ToInt32(objDTO.DefaultReorderQuantity ?? 1) % (CostUOMDTO.CostUOMValue ?? 1)) == 0)
                                {

                                }
                                else
                                {
                                    objDTO.DefaultReorderQuantity = Convert.ToDouble(CostUOMDTO.CostUOMValue ?? 1);
                                    /*
                                    calcDefaultReorderQuantity = Convert.ToInt32(objDTO.DefaultReorderQuantity ?? 1) + ((CostUOMDTO.CostUOMValue ?? 1) - (Convert.ToInt32(objDTO.DefaultReorderQuantity ?? 1) % (CostUOMDTO.CostUOMValue ?? 1)));
                                    message = ResMessage.ReorderQtyAsPerCUOM;
                                    status = "fail";
                                    return Json(new { Message = message, Status = status, calcDefaultReorderQuantity = calcDefaultReorderQuantity }, JsonRequestBehavior.AllowGet);
                                    */
                                }
                            }


                            /*
                            if ((CostUOMDTO.CostUOMValue ?? 1) != Convert.ToInt32(objDTO.DefaultReorderQuantity ?? 1))
                            {
                                calcDefaultReorderQuantity = (CostUOMDTO.CostUOMValue ?? 1);
                                message = ResMessage.ReorderQtyAsPerCUOMValue;
                                status = "fail";
                                return Json(new { Message = message, Status = status, calcDefaultReorderQuantity = calcDefaultReorderQuantity }, JsonRequestBehavior.AllowGet);

                            }
                            */

                        }

                        /*
                        if (objDTO.IsItemLevelMinMaxQtyRequired == true && objDTO.MinimumQuantity != null && objDTO.MinimumQuantity > 0)
                        {
                            bool blFlagMin = objCDAL.CheckItemCostUOMWithMinQty(Convert.ToInt64(objDTO.CostUOMID), Convert.ToInt64(objDTO.MinimumQuantity));
                            if (blFlagMin == false)
                            {
                                message = string.Format(ResItemMaster.CostUOMMinQTY, ResItemMaster.MinimumQuantity, objDTO.ItemNumber);
                                status = "Fa";
                                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        */
                    }



                    if (objDTO.ItemTraking == 1)
                    {
                        objDTO.LotNumberTracking = true;
                        objDTO.SerialNumberTracking = false;
                    }
                    else if (objDTO.ItemTraking == 2)
                    {
                        objDTO.LotNumberTracking = false;
                        objDTO.SerialNumberTracking = true;
                    }
                    objDTO.CreatedByName = SessionHelper.UserName;
                    objDTO.GUID = Guid.NewGuid();
                    //Rename image if uploaded

                    //  objDTO.ItemUniqueNumber = UniqueId.Get();

                    if (objDTO.DefaultLocation == null)
                        objDTO.DefaultLocation = 0;

                    objDTO.WhatWhereAction = "ItemDetails";


                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    objDTO.EnhancedDescription = (objDTO.EnhancedDescription == null ? "" : objDTO.EnhancedDescription);
                    if (objDTO.IsOrderable)
                    {
                        objDTO.ItemIsActiveDate = DateTimeUtility.DateTimeNow;
                    }

                    obj.Insert(objDTO);
                    TempItemID = objDTO.ID;

                    // Add Location  Code 

                    // End Location Code

                    if (Session["Temp_FileName"] != null)
                    {
                        string UNCPathRoot = HttpRuntime.AppDomainAppPath + System.Configuration.ConfigurationManager.AppSettings["ItemImagePath"].ToString();
                        string strPath = Path.Combine(UNCPathRoot, SessionHelper.CompanyID.ToString() + "/" + Convert.ToString(Session["Temp_FileName"]));
                        FileInfo fi = new FileInfo(strPath);
                        try
                        {
                            fi.MoveTo(Path.Combine(UNCPathRoot, SessionHelper.CompanyID.ToString() + "/" + TempItemID + "." + Convert.ToString(Session["Temp_FileName"]).Split('.')[1]));
                        }
                        catch { }
                        objDTO.ImagePath = TempItemID + "." + Convert.ToString(Session["Temp_FileName"]).Split('.')[1];
                        Session["Temp_FileName"] = null;
                    }

                    //HttpResponseMessage hrmResult = obj.PutRecord(TempItemID, objDTO);
                    objDTO.WhatWhereAction = "itemDetails";
                    status = "ok";
                    //bool ReturnVal = obj.Edit(objDTO, SessionUserId, SessionHelper.EnterPriceID, true);
                    //if (ReturnVal)
                    //{
                    //    message = ResMessage.SaveMessage; //ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                    //    status = "ok";
                    //}
                    //else
                    //{
                    //    message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // string.Format(ResMessage.SaveErrorMsg, hrmResult.StatusCode); // "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                    //    status = "fail";
                    //}

                    if (status == "ok")
                    {
                        //Kit component
                        if (objDTO.ItemType == 3)
                        {
                            ///Save itemlocationwise quantity to database from the session
                            List<KitDetailDTO> lstKitDetailDTO = null;
                            if (Session["ItemKitDetail" + itemGUIDString] != null)
                            {
                                lstKitDetailDTO = ((List<KitDetailDTO>)Session["ItemKitDetail" + itemGUIDString]).Where(t => t.ItemGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty).ToList();
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
                                        objKitDetailDAL.Insert(itembr, SessionUserId, SessionHelper.EnterPriceID);
                                    }
                                }

                                Session["ItemKitDetail" + itemGUIDString] = null;
                            }
                        }

                        ///Save itemlocationwise quantity to database from the session
                        List<ItemManufacturerDetailsDTO> lstItemManufactuer = null;
                        if (Session["ItemManufacture" + itemGUIDString] != null)
                        {
                            //Wi-1505 
                            lstItemManufactuer = ((List<ItemManufacturerDetailsDTO>)Session["ItemManufacture" + itemGUIDString]).Where(t => !string.IsNullOrEmpty(t.ManufacturerName) || !string.IsNullOrEmpty(t.ManufacturerNumber)).ToList();
                            //   lstItemManufactuer = ((List<ItemManufacturerDetailsDTO>)Session["ItemManufacture"]).Where(t => !string.IsNullOrEmpty(t.ManufacturerName) || !string.IsNullOrEmpty(t.ManufacturerName)).ToList();

                            foreach (var itembr in lstItemManufactuer)
                            {
                                /// - logic for Adding supplier if Newly added...
                                if (itembr.ManufacturerID == 0)
                                {
                                    ManufacturerMasterDAL objManuMasterDAL = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName);
                                    ManufacturerMasterDTO objManufacturerMasterDTO = new ManufacturerMasterDTO();
                                    //if (!string.IsNullOrEmpty(itembr.ManufacturerName))
                                    objManufacturerMasterDTO = objManuMasterDAL.GetManufacturerByNameNormal(itembr.ManufacturerName, SessionHelper.RoomID, SessionHelper.CompanyID, false);
                                    //else
                                    //objManufacturerMasterDTO = objManuMasterDAL.GetRecord(itembr.ManufacturerName, 0, 0, false, false, false);

                                    if (objManufacturerMasterDTO == null)
                                    {
                                        objManufacturerMasterDTO = new ManufacturerMasterDTO();
                                        objManufacturerMasterDTO.ID = 0;
                                        objManufacturerMasterDTO.Manufacturer = itembr.ManufacturerName ?? string.Empty;
                                        objManufacturerMasterDTO.CompanyID = SessionHelper.CompanyID;
                                        objManufacturerMasterDTO.Room = SessionHelper.RoomID;
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
                            Session["ItemManufacture" + itemGUIDString] = null;
                        }

                        if (objDTO.ItemType != 4)
                        {
                            ///Save itemlocationwise quantity to database from the session
                            List<ItemSupplierDetailsDTO> lstItemSupplier = null;
                            if (Session["ItemSupplier" + itemGUIDString] != null)
                            {
                                lstItemSupplier = ((List<ItemSupplierDetailsDTO>)Session["ItemSupplier" + itemGUIDString]).Where(t => !string.IsNullOrEmpty(t.SupplierName) || !string.IsNullOrEmpty(t.SupplierNumber)).ToList();
                                foreach (var itembr in lstItemSupplier)
                                {
                                    itembr.ItemGUID = objDTO.GUID;

                                    if (!string.IsNullOrEmpty(itembr.SupplierName))
                                    {
                                        /// End- logic for Adding supplier if Newly added...
                                    }
                                    if (itembr.IsDefault.GetValueOrDefault(false))
                                    {
                                        defaultSupplierPartNo = itembr.SupplierNumber;
                                    }

                                    ItemSupplierDetailsDAL objItemSupplierDetailsDAL = new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName);
                                    if (itembr.SupplierID == 0)
                                    {
                                        if (!string.IsNullOrEmpty(itembr.SupplierName))
                                        {
                                            /// - logic for Adding supplier if Newly added...
                                            SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                                            SupplierMasterDTO objSupplierMasterDTO = objSupplierMasterDAL.GetSupplierByNamePlain(SessionHelper.RoomID, SessionHelper.CompanyID, false, itembr.SupplierName);
                                            if (objSupplierMasterDTO == null)
                                            {
                                                objSupplierMasterDTO = new SupplierMasterDTO();
                                                objSupplierMasterDTO.ID = 0;
                                                objSupplierMasterDTO.SupplierName = itembr.SupplierName;
                                                objSupplierMasterDTO.SupplierColor = get_random_color();
                                                objSupplierMasterDTO.CompanyID = SessionHelper.CompanyID;
                                                objSupplierMasterDTO.Room = SessionHelper.RoomID;
                                                objSupplierMasterDTO.RoomName = SessionHelper.RoomName;
                                                objSupplierMasterDTO.UpdatedByName = SessionHelper.UserName;
                                                objSupplierMasterDTO.CreatedByName = SessionHelper.UserName;
                                                objSupplierMasterDTO.CreatedBy = SessionHelper.UserID;
                                                objSupplierMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                                                objSupplierMasterDTO.Created = DateTimeUtility.DateTimeNow;
                                                objSupplierMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                                                objSupplierMasterDTO.AddedFrom = "Web";
                                                objSupplierMasterDTO.EditedFrom = "Web";
                                                objSupplierMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                                objSupplierMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                                objSupplierMasterDTO.ID = objSupplierMasterDAL.Insert(objSupplierMasterDTO);
                                                itembr.SupplierID = objSupplierMasterDTO.ID;

                                                if (itembr.SupplierID > 0)
                                                {
                                                    // insert into Room Schedule for PullSchedule type is Immediate 

                                                    SchedulerDTO objPullSchedulerDTO = new SchedulerDTO();
                                                    objPullSchedulerDTO.SupplierId = itembr.SupplierID;
                                                    objPullSchedulerDTO.CompanyId = itembr.CompanyID.GetValueOrDefault(0);
                                                    objPullSchedulerDTO.RoomId = itembr.Room.GetValueOrDefault(0);
                                                    objPullSchedulerDTO.LoadSheduleFor = 7;
                                                    objPullSchedulerDTO.ScheduleMode = 5;
                                                    objPullSchedulerDTO.IsScheduleActive = true;
                                                    objPullSchedulerDTO.MonthlyDayOfMonth = 2;
                                                    objSupplierMasterDAL.SaveSupplierSchedule(objPullSchedulerDTO);

                                                    //objSchedulerDTO.IsScheduleActive= itemb
                                                    //objSchedulerDTO.IsScheduleChanged
                                                    //objSchedulerDTO.ModuleName
                                                    //objSchedulerDTO.ModuleNameResource
                                                    //objSchedulerDTO.MonthlyDayOfMonth


                                                }
                                            }

                                            if (itembr.IsDefault == true)
                                            {
                                                itembr.SupplierID = objSupplierMasterDTO.ID;
                                            }
                                            /// End- logic for Adding supplier if Newly added...
                                        }
                                    }
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
                                    //obj.UpdateSupplierDetails(SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);
                                }
                                Session["ItemSupplier" + itemGUIDString] = null;
                            }
                        }

                        ///Save itemlocationwise quantity to database from the session
                        List<BinMasterDTO> lstBinReplanish = null;
                        if (Session["BinReplanish" + itemGUIDString] != null)
                        {
                            lstBinReplanish = ((List<BinMasterDTO>)Session["BinReplanish" + itemGUIDString]).Where(t => !string.IsNullOrEmpty(t.BinNumber)).ToList();
                            BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);

                            double? UpdatedOnHandQuantity = 0;
                            List<long> insertedBinIds = new List<long>();
                            lstBinReplanish = objBinMasterDAL.AssignUpdateItemLocations(lstBinReplanish, objDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, false, out UpdatedOnHandQuantity, out insertedBinIds, SessionUserId);
                            if (UpdatedOnHandQuantity != null)
                                objDTO.OnHandQuantity = UpdatedOnHandQuantity;

                            if (lstBinReplanish != null && lstBinReplanish.Any(t => t.IsDefault == true))
                            {
                                objDTO.DefaultLocation = lstBinReplanish.First(t => t.IsDefault == true).ID;
                                objDTO.BinNumber = lstBinReplanish.First(t => t.IsDefault == true).BinNumber;
                            }
                            UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
                            UDFController objUDFController = new UDFController();
                            IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain("BinUDF", SessionHelper.RoomID, SessionHelper.CompanyID);
                            string udfRequier = string.Empty;
                            //string message = string.Empty;
                            string ErrorMessage = string.Empty;
                            foreach (var item in lstBinReplanish)
                            {
                                foreach (var i in DataFromDB)
                                {
                                    if (i.UDFControlType.ToLower().Contains("dropdown editable"))
                                    {
                                        if (i.UDFColumnName == "UDF1" && !string.IsNullOrWhiteSpace(item.BinUDF1))
                                        {
                                            objUDFController.InsertUDFOption(i.ID, item.BinUDF1, "BinUDF", null, false, false, "");
                                        }
                                        else if (i.UDFColumnName == "UDF2" && !string.IsNullOrWhiteSpace(item.BinUDF2))
                                        {
                                            objUDFController.InsertUDFOption(i.ID, item.BinUDF2, "BinUDF", null, false, false, "");
                                        }
                                        else if (i.UDFColumnName == "UDF3" && !string.IsNullOrWhiteSpace(item.BinUDF3))
                                        {
                                            objUDFController.InsertUDFOption(i.ID, item.BinUDF3, "BinUDF", null, false, false, "");
                                        }
                                        else if (i.UDFColumnName == "UDF4" && !string.IsNullOrWhiteSpace(item.BinUDF4))
                                        {
                                            objUDFController.InsertUDFOption(i.ID, item.BinUDF4, "BinUDF", null, false, false, "");
                                        }
                                        else if (i.UDFColumnName == "UDF5" && !string.IsNullOrWhiteSpace(item.BinUDF5))
                                        {
                                            objUDFController.InsertUDFOption(i.ID, item.BinUDF5, "BinUDF", null, false, false, "");
                                        }
                                    }
                                }
                            }
                            //foreach (var itembr in lstBinReplanish)
                            //{
                            //    itembr.ItemGUID = objDTO.GUID;
                            //    itembr.IsStagingHeader = false;
                            //    itembr.IsStagingLocation = false;

                            //    BinMasterDAL objItemLocationLevelQuanityDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                            //    if (itembr.ID > 0)
                            //    {
                            //        objItemLocationLevelQuanityDAL.Edit(itembr);
                            //    }
                            //    else
                            //    {
                            //        Int64 objId = objItemLocationLevelQuanityDAL.Insert(itembr);
                            //        itembr.ID = objId;
                            //    }
                            //    if (itembr.IsDefault == true)
                            //    {
                            //        objDTO.DefaultLocation = itembr.ID;
                            //    }

                            //}

                            #region "ItemeVMI Setup"

                            //ItemLocationeVMISetupDAL objItemLocationeVMISetupDAL = new ItemLocationeVMISetupDAL(SessionHelper.EnterPriseDBName);
                            //ItemLocationeVMISetupDTO objItemLocationeVMISetupDTO = new ItemLocationeVMISetupDTO();
                            //if (lstBinReplanish.Count > 0)
                            //{
                            //    foreach (var itembr in lstBinReplanish)
                            //    {
                            //        if (itembr.IsDefault ?? false)
                            //        {
                            //            objDTO.DefaultLocation = itembr.ID;
                            //        }
                            //        objItemLocationeVMISetupDTO = objItemLocationeVMISetupDAL.GetRecordByItemGUID(itembr.ItemGUID, itembr.ID, SessionHelper.RoomID, SessionHelper.CompanyID);
                            //        if (objItemLocationeVMISetupDTO != null)
                            //        {
                            //            ItemLocationeVMISetupDTO objsetp = new ItemLocationeVMISetupDTO();
                            //            objsetp.ID = objItemLocationeVMISetupDTO.ID;
                            //            objsetp.eVMISensorID = itembr.eVMISensorID;
                            //            objsetp.eVMISensorPort = itembr.eVMISensorPort;
                            //            objsetp.BinID = itembr.ID;
                            //            objsetp.IsDeleted = false;
                            //            objsetp.IsArchived = false;
                            //            objsetp.CompanyID = itembr.CompanyID;
                            //            objsetp.Room = itembr.Room;
                            //            objsetp.ItemGUID = itembr.ItemGUID;
                            //            objItemLocationeVMISetupDAL.Edit(objsetp);
                            //        }
                            //        else
                            //        {
                            //            ItemLocationeVMISetupDTO objsetp = new ItemLocationeVMISetupDTO();
                            //            objsetp.ID = 0;
                            //            objsetp.eVMISensorID = itembr.eVMISensorID;
                            //            objsetp.eVMISensorPort = itembr.eVMISensorPort;
                            //            objsetp.BinID = itembr.ID;
                            //            objsetp.IsDeleted = false;
                            //            objsetp.IsArchived = false;
                            //            objsetp.CompanyID = itembr.CompanyID;
                            //            objsetp.Room = itembr.Room;
                            //            objsetp.ItemGUID = itembr.ItemGUID;
                            //            objItemLocationeVMISetupDAL.Insert(objsetp);
                            //        }
                            //    }
                            //}
                            #endregion

                            Session["BinReplanish" + itemGUIDString] = null;
                        }
                        objDTO.OnOrderQuantity = new CartItemDAL(SessionHelper.EnterPriseDBName).getOnOrderQty(objDTO.GUID);
                        objDTO.OnOrderInTransitQuantity = new ItemMasterDAL(SessionHelper.EnterPriseDBName).getOnOrderInTransitQty(objDTO.GUID);
                        bool ReturnVal = obj.Edit(objDTO, SessionUserId, SessionHelper.EnterPriceID, true,IsAutoSOTLater : true, IgnoreAutoSOT : true);
                        //Auto cart entry
                        //if (objDTO.ItemType == 3)
                        //{
                        //    IEnumerable<KitDetailDTO> listKitDtl = null;
                        //    KitDetailDAL objKitDetl = new KitDetailDAL(SessionHelper.EnterPriseDBName);
                        //    listKitDtl = objKitDetl.GetCachedData(SessionHelper.RoomID, CompanyID, false, false, true);
                        //    if (listKitDtl != null && listKitDtl.Count() > 0)
                        //    {
                        //        listKitDtl = listKitDtl.Where(x => x.KitGUID == objDTO.GUID).ToList();
                        //        if (listKitDtl != null && listKitDtl.Count() > 0)
                        //        {
                        //            foreach (var item in listKitDtl)
                        //            {
                        //                new CartItemDAL(SessionHelper.EnterPriseDBName).AutoCartUpdateByCode((Guid)item.ItemGUID, objDTO.CreatedBy ?? 0, SessionHelper.EnterPriceID, objDTO.IsOnlyFromItemUI);
                        //            }
                        //        }
                        //    }
                        //}
                        //else
                        //{


                        if ((objDTO.OnHandQuantity ?? 0) > 0)
                        {
                            foreach (var bin in lstBinReplanish)
                            {
                                string InventryLocation = bin.BinNumber;
                                ItemLocationDetailsDAL objItemLocationDetailsDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                                BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                                BinMasterDTO objBin = new BinMasterDTO();
                                if (!string.IsNullOrWhiteSpace(InventryLocation))
                                {
                                    objBin = objBinMasterDAL.GetItemBinPlain(objDTO.GUID, InventryLocation, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, false);
                                    if (objBin == null)
                                    {
                                        objBin = new BinMasterDTO();
                                    }
                                }

                                if ((bin.CustomerOwnedQuantity.GetValueOrDefault(0) + bin.ConsignedQuantity.GetValueOrDefault(0)) > 0)
                                {
                                    ItemLocationDetailsDTO objItemLocationDetailsDTO = new ItemLocationDetailsDTO();
                                    objItemLocationDetailsDTO.Room = SessionHelper.RoomID;
                                    objItemLocationDetailsDTO.CompanyID = SessionHelper.CompanyID;
                                    objItemLocationDetailsDTO.ItemGUID = objDTO.GUID;
                                    objItemLocationDetailsDTO.BinID = objBin.ID;
                                    objItemLocationDetailsDTO.BinNumber = objBin.BinNumber;
                                    objItemLocationDetailsDTO.CustomerOwnedQuantity = bin.CustomerOwnedQuantity.GetValueOrDefault(0);
                                    objItemLocationDetailsDTO.ConsignedQuantity = bin.ConsignedQuantity.GetValueOrDefault(0);
                                    //objItemLocationDetailsDTO.CustomerOwnedQuantity = objDTO.Consignment ? 0 : objDTO.OnHandQuantity;
                                    //objItemLocationDetailsDTO.ConsignedQuantity = objDTO.Consignment ? objDTO.OnHandQuantity : 0;
                                    objItemLocationDetailsDTO.SerialNumber = null;
                                    objItemLocationDetailsDTO.LotNumber = null;
                                    objItemLocationDetailsDTO.Expiration = null;
                                    objItemLocationDetailsDTO.Received = DateTime.UtcNow.ToString("MM/dd/yyyy");
                                    objItemLocationDetailsDTO.ReceivedDate = DateTime.UtcNow;
                                    objItemLocationDetailsDTO.IsDeleted = false;
                                    objItemLocationDetailsDTO.IsArchived = false;
                                    objItemLocationDetailsDTO.Created = DateTimeUtility.DateTimeNow;
                                    objItemLocationDetailsDTO.Updated = DateTimeUtility.DateTimeNow;
                                    objItemLocationDetailsDTO.LastUpdatedBy = SessionHelper.UserID;
                                    objItemLocationDetailsDTO.CreatedBy = SessionHelper.UserID;
                                    objItemLocationDetailsDTO.GUID = Guid.NewGuid();
                                    objItemLocationDetailsDTO.InsertedFrom = "itemMasterOnHandInsert-DirectImport";
                                    objItemLocationDetailsDTO.Cost = objDTO.Cost;
                                    objItemLocationDetailsDAL.ItemLocationDetailsImportSave(objItemLocationDetailsDTO, SessionUserId,enterpriseId);
                                }
                            }
                        }
                        //new CartItemDAL(SessionHelper.EnterPriseDBName).AutoCartUpdateByCode(objDTO.GUID, objDTO.LastUpdatedBy ?? 0, "Web", "Inventorycontroller>> ItemSave");
                        new CartItemDAL(SessionHelper.EnterPriseDBName).AutoCartUpdateByCode(objDTO.GUID, objDTO.LastUpdatedBy ?? 0, "Web", "Inventory >> Create Save", SessionUserId);
                        //}

                        //obj.EditItemQtyCache(objDTO.ID, SessionHelper.CompanyID, SessionHelper.RoomID);
                    }
                }
                #endregion
            }
            else
            {
                #region "Edit"

                //string strOK = objCDAL.CheckItemDuplication(objDTO.ItemNumber, "edit", objDTO.ID, "ItemMaster", "ItemNumber", SessionHelper.RoomID, SessionHelper.CompanyID,objDTO.ItemType);
                string strOK = objCDAL.DuplicateCheck(objDTO.ItemNumber, "edit", objDTO.ID, "ItemMaster", "ItemNumber", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResItemMaster.ItemNumber, objDTO.ItemNumber);  //"BinNumber \"" + objDTO.BinNumber + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    double Qty = 0;
                    Qty = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName).GetItemaConsignedQty(objDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    ItemMasterDTO objTemp = new ItemMasterDTO();
                    objTemp = obj.GetItemWithoutJoins(objDTO.ID, null);
                    if (objTemp != null && objTemp.Consignment && !objDTO.Consignment && Qty > 0)
                    {
                        message = string.Format(ResMessage.ConsingedCanNotchange, ResItemMaster.Consignment, objDTO.Consignment);
                        status = "Fa";
                        return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                    }
                    
                    if (objDTO.IsAllowOrderCostuom)
                    {

                        objDTO.IsEnforceDefaultReorderQuantity = true;
                        /*                   
                        bool blFlag = objCDAL.CheckItemCostUOM(Convert.ToInt64(objDTO.CostUOMID), Convert.ToInt64(objDTO.DefaultReorderQuantity));
                        if (blFlag == false)
                        {
                            message = string.Format(ResItemMaster.CostUOMReorderQTY, ResItemMaster.DefaultReorderQuantity, objDTO.ItemNumber);
                            status = "Fa";
                            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                        }
                        */

                        //CostUOMMasterDAL objCostUOMDAL = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName);
                        CostUOMMasterDTO CostUOMDTO = objCostUOMDAL.GetCostUOMByID(objDTO.CostUOMID.GetValueOrDefault(0));
                        if (CostUOMDTO != null && CostUOMDTO.ID > 0)
                        {
                            //int calcDefaultReorderQuantity = 0;

                            //CostUOMValue = CostUOMDTO.CostUOMValue.ToString();
                            if ((CostUOMDTO.CostUOMValue ?? 1) > Convert.ToInt32(objDTO.DefaultReorderQuantity ?? 1))
                            {
                                objDTO.DefaultReorderQuantity = CostUOMDTO.CostUOMValue ?? 1;
                                //calcDefaultReorderQuantity = (CostUOMDTO.CostUOMValue ?? 1);
                                //message = ResMessage.ReorderQtyAsPerCUOM;
                                //status = "fail";
                                //return Json(new { Message = message, Status = status, calcDefaultReorderQuantity = calcDefaultReorderQuantity }, JsonRequestBehavior.AllowGet);

                            }
                            else
                            {
                                if ((Convert.ToInt32(objDTO.DefaultReorderQuantity ?? 1) % (CostUOMDTO.CostUOMValue ?? 1)) == 0)
                                {

                                }
                                else
                                {
                                    objDTO.DefaultReorderQuantity = CostUOMDTO.CostUOMValue ?? 1;
                                    //calcDefaultReorderQuantity = Convert.ToInt32(objDTO.DefaultReorderQuantity ?? 1) + ((CostUOMDTO.CostUOMValue ?? 1) - (Convert.ToInt32(objDTO.DefaultReorderQuantity ?? 1) % (CostUOMDTO.CostUOMValue ?? 1)));
                                    //message = ResMessage.ReorderQtyAsPerCUOM;
                                    //status = "fail";
                                    //return Json(new { Message = message, Status = status, calcDefaultReorderQuantity = calcDefaultReorderQuantity }, JsonRequestBehavior.AllowGet);
                                }
                            }

                            /*
                            if ((CostUOMDTO.CostUOMValue ?? 1) != Convert.ToInt32(objDTO.DefaultReorderQuantity ?? 1))
                            {
                                calcDefaultReorderQuantity = (CostUOMDTO.CostUOMValue ?? 1);
                                message = ResMessage.ReorderQtyAsPerCUOMValue;
                                status = "fail";
                                return Json(new { Message = message, Status = status, calcDefaultReorderQuantity = calcDefaultReorderQuantity }, JsonRequestBehavior.AllowGet);

                            }
                            */

                        }

                        /*
                        if (objDTO.IsItemLevelMinMaxQtyRequired == true && objDTO.MinimumQuantity != null && objDTO.MinimumQuantity > 0)
                        {
                            bool blFlagMin = objCDAL.CheckItemCostUOMWithMinQty(Convert.ToInt64(objDTO.CostUOMID), Convert.ToInt64(objDTO.MinimumQuantity));
                            if (blFlagMin == false)
                            {
                                message = string.Format(ResItemMaster.CostUOMMinQTY, ResItemMaster.MinimumQuantity, objDTO.ItemNumber);
                                status = "Fa";
                                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        */
                    }

                    ItemMasterDTO ExistItemDTO = obj.GetRecordByItemGUID(objDTO.GUID, RoomID, CompanyID);
                    if (ExistItemDTO != null && ExistItemDTO.IsAllowOrderCostuom != objDTO.IsAllowOrderCostuom)
                    {
                        if (ExistItemDTO.OnOrderQuantity != null && ExistItemDTO.OnOrderQuantity > 0)
                        {
                            message = string.Format(ResItemMaster.CostUOMOnOrderQTY, ResItemMaster.OnOrderQuantity, objDTO.ItemNumber);
                            status = "Fa";
                            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                        }
                        else if (ExistItemDTO.OnOrderInTransitQuantity != null && ExistItemDTO.OnOrderInTransitQuantity > 0)
                        {
                            message = string.Format(ResItemMaster.CostUOMOnOrderInTransitQuantity, ResItemMaster.OnOrderQuantity, objDTO.ItemNumber);
                            status = "Fa";
                            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    if (objDTO.CreatedBy == null)
                        objDTO.CreatedBy = SessionHelper.UserID;
                    if (objDTO.LastUpdatedBy == null)
                        objDTO.LastUpdatedBy = SessionHelper.UserID;

                    if (objDTO.IsOnlyFromItemUI)
                    {
                        objDTO.EditedFrom = "Web";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    }


                    if (!String.IsNullOrEmpty(Request.Form["txtImagePath"]))
                    {
                        objDTO.ImagePath = Request.Form["txtImagePath"];
                    }

                    if (objTemp != null && !objTemp.IsOrderable && objDTO.IsOrderable)
                    {
                        objDTO.ItemIsActiveDate = DateTimeUtility.DateTimeNow;
                    }
                    else if (objTemp != null && objTemp.IsOrderable && objDTO.IsOrderable)
                    {
                        objDTO.ItemIsActiveDate = objTemp.ItemIsActiveDate;
                    }
                    else if (!objDTO.IsOrderable)
                    {
                        objDTO.ItemIsActiveDate = null;
                    }

                    bool IsForQBChange = false;
                    Int64? QB_QOH = null;
                    if ((objDTO.ItemImageExternalURL != ExistItemDTO.ItemImageExternalURL) ||
                       //(objDTO.OnHandQuantity != ExistItemDTO.OnHandQuantity) ||
                       (objDTO.ImageType != ExistItemDTO.ImageType)
                       )
                    {
                        IsForQBChange = true;
                        QB_QOH = (Int64)objDTO.OnHandQuantity.GetValueOrDefault(0);
                    }
                    if (objDTO.ItemTraking == 1)
                    {
                        objDTO.LotNumberTracking = true;
                        objDTO.SerialNumberTracking = false;
                    }
                    else if (objDTO.ItemTraking == 2)
                    {
                        objDTO.LotNumberTracking = false;
                        objDTO.SerialNumberTracking = true;
                    }
                    bool ReturnVal = obj.Edit(objDTO, SessionUserId, SessionHelper.EnterPriceID, true,IsAutoSOTLater:true,IgnoreAutoSOT:true);

                    if (ReturnVal)
                    {
                        message = ResMessage.SaveMessage;  //ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                        TempItemID = objDTO.ID;

                        if (IsForQBChange)
                        {
                            QuickBookItemDAL objQBItemDAL = new QuickBookItemDAL(SessionHelper.EnterPriseDBName);
                            objQBItemDAL.InsertQuickBookItem(objDTO.GUID, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, "Update", objDTO.IsDeleted, SessionHelper.UserID, "Web", QB_QOH, "Item Edit");
                        }
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
                            if (Session["ItemKitDetail" + itemGUIDString] != null)
                            {
                                lstKitDetailDTO = ((List<KitDetailDTO>)Session["ItemKitDetail" + itemGUIDString]).Where(t => t.ItemGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty).ToList();
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
                                Session["ItemKitDetail" + itemGUIDString] = null;
                            }
                        }


                        ///Save itemlocationwise quantity to database from the session
                        List<ItemManufacturerDetailsDTO> lstItemManufactuer = null;
                        if (Session["ItemManufacture" + itemGUIDString] != null)
                        {
                            lstItemManufactuer = ((List<ItemManufacturerDetailsDTO>)Session["ItemManufacture" + itemGUIDString]).Where(t => !string.IsNullOrEmpty(t.ManufacturerNumber) || !string.IsNullOrEmpty(t.ManufacturerName)).ToList();
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
                                    objManufacturerMasterDTO = objManuMasterDAL.GetManufacturerByNameNormal(itembr.ManufacturerName, SessionHelper.RoomID, SessionHelper.CompanyID, false);
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
                            Session["ItemManufacture" + itemGUIDString] = null;
                        }
                        

                        if (objDTO.ItemType != 4)
                        {
                            ///Save itemlocationwise quantity to database from the session
                            List<ItemSupplierDetailsDTO> lstItemSupplier = null;
                            if (Session["ItemSupplier" + itemGUIDString] != null)
                            {
                                lstItemSupplier = ((List<ItemSupplierDetailsDTO>)Session["ItemSupplier" + itemGUIDString]).Where(t => (!string.IsNullOrEmpty(t.SupplierName)) || !string.IsNullOrEmpty(t.SupplierNumber)).ToList();

                                if (lstItemSupplier.Count > 0)
                                {
                                    string ManuIDs = "";
                                    ItemSupplierDetailsDAL objItemSupplierDetailsDAL = new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName);
                                    foreach (var itembr in lstItemSupplier)
                                    {
                                        itembr.ItemGUID = objDTO.GUID;
                                        
                                        if (itembr.IsDefault.GetValueOrDefault(false))
                                        {
                                            defaultSupplierPartNo = itembr.SupplierNumber;
                                        }

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
                                Session["ItemSupplier" + itemGUIDString] = null;
                            }
                        }

                        ///Save itemlocationwise quantity to database from the session
                        List<BinMasterDTO> lstBinReplanish = null;
                        if (Session["BinReplanish" + itemGUIDString] != null)
                        {
                            BinMasterDAL objItemLocationLevelQuanityDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);

                            lstBinReplanish = ((List<BinMasterDTO>)Session["BinReplanish" + itemGUIDString]).Where(t => !string.IsNullOrEmpty(t.BinNumber)).ToList();
                            BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);

                            double? UpdatedOnHandQuantity = 0;
                            List<long> insertedBinIds = new List<long>();
                            lstBinReplanish = objBinMasterDAL.AssignUpdateItemLocations(lstBinReplanish, objDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, false, out UpdatedOnHandQuantity, out insertedBinIds, SessionUserId);
                            if (UpdatedOnHandQuantity != null)
                                objDTO.OnHandQuantity = UpdatedOnHandQuantity;

                            if (lstBinReplanish != null && lstBinReplanish.Any(t => t.IsDefault == true))
                            {
                                objDTO.DefaultLocation = lstBinReplanish.First(t => t.IsDefault == true).ID;
                            }

                            UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
                            UDFController objUDFController = new UDFController();
                            IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain("BinUDF", SessionHelper.RoomID, SessionHelper.CompanyID);
                            string udfRequier = string.Empty;
                            //string message = string.Empty;
                            string ErrorMessage = string.Empty;
                            foreach (var item in lstBinReplanish)
                            {
                                foreach (var i in DataFromDB)
                                {
                                    if (i.UDFControlType.ToLower().Contains("dropdown editable"))
                                    {
                                        if (i.UDFColumnName == "UDF1" && !string.IsNullOrWhiteSpace(item.BinUDF1))
                                        {
                                            objUDFController.InsertUDFOption(i.ID, item.BinUDF1, "BinUDF", null, false, false, "");
                                        }
                                        else if (i.UDFColumnName == "UDF2" && !string.IsNullOrWhiteSpace(item.BinUDF2))
                                        {
                                            objUDFController.InsertUDFOption(i.ID, item.BinUDF2, "BinUDF", null, false, false, "");
                                        }
                                        else if (i.UDFColumnName == "UDF3" && !string.IsNullOrWhiteSpace(item.BinUDF3))
                                        {
                                            objUDFController.InsertUDFOption(i.ID, item.BinUDF3, "BinUDF", null, false, false, "");
                                        }
                                        else if (i.UDFColumnName == "UDF4" && !string.IsNullOrWhiteSpace(item.BinUDF4))
                                        {
                                            objUDFController.InsertUDFOption(i.ID, item.BinUDF4, "BinUDF", null, false, false, "");
                                        }
                                        else if (i.UDFColumnName == "UDF5" && !string.IsNullOrWhiteSpace(item.BinUDF5))
                                        {
                                            objUDFController.InsertUDFOption(i.ID, item.BinUDF5, "BinUDF", null, false, false, "");
                                        }
                                    }
                                }
                            }
                            //if (lstBinReplanish.Count > 0)
                            //{
                            //    string BinIDs = "";
                            //    foreach (var itembr in lstBinReplanish)
                            //    {
                            //        itembr.ItemGUID = objDTO.GUID;
                            //        itembr.IsStagingHeader = false;
                            //        itembr.IsStagingLocation = false;

                            //        if (itembr.ID > 0)
                            //        {
                            //            objItemLocationLevelQuanityDAL.Edit(itembr);
                            //        }
                            //        else
                            //        {
                            //            Int64 insertedId = objItemLocationLevelQuanityDAL.Insert(itembr);
                            //            itembr.ID = insertedId;
                            //        }
                            //        BinIDs += itembr.ID + ",";
                            //        if (itembr.IsDefault == true)
                            //        {
                            //            objDTO.DefaultLocation = itembr.ID;
                            //        }

                            //    }
                            //    //Delete except session record....
                            //    objItemLocationLevelQuanityDAL.DeleteRecordsExcept(BinIDs, objDTO.GUID, SessionHelper.UserID, SessionHelper.CompanyID);
                            //}

                            #region "ItemeVMI Setup"

                            //ItemLocationeVMISetupDAL objItemLocationeVMISetupDAL = new ItemLocationeVMISetupDAL(SessionHelper.EnterPriseDBName);
                            //ItemLocationeVMISetupDTO objItemLocationeVMISetupDTO = new ItemLocationeVMISetupDTO();
                            //if (lstBinReplanish.Count > 0)
                            //{
                            //    foreach (var itembr in lstBinReplanish)
                            //    {
                            //        if (itembr.IsDefault ?? false)
                            //        {
                            //            objDTO.DefaultLocation = itembr.ID;
                            //        }

                            //        objItemLocationeVMISetupDTO = objItemLocationeVMISetupDAL.GetRecordByItemGUID(itembr.ItemGUID, itembr.ID, SessionHelper.RoomID, SessionHelper.CompanyID);
                            //        if (objItemLocationeVMISetupDTO != null)
                            //        {
                            //            ItemLocationeVMISetupDTO objsetp = new ItemLocationeVMISetupDTO();
                            //            objsetp.ID = objItemLocationeVMISetupDTO.ID;
                            //            objsetp.eVMISensorID = itembr.eVMISensorID;
                            //            objsetp.eVMISensorPort = itembr.eVMISensorPort;
                            //            objsetp.BinID = itembr.ID;
                            //            objsetp.IsDeleted = false;
                            //            objsetp.IsArchived = false;
                            //            objsetp.CompanyID = itembr.CompanyID;
                            //            objsetp.Room = itembr.Room;
                            //            objsetp.ItemGUID = itembr.ItemGUID;
                            //            objItemLocationeVMISetupDAL.Edit(objsetp);
                            //        }
                            //        else
                            //        {
                            //            ItemLocationeVMISetupDTO objsetp = new ItemLocationeVMISetupDTO();
                            //            objsetp.ID = 0;
                            //            objsetp.eVMISensorID = itembr.eVMISensorID;
                            //            objsetp.eVMISensorPort = itembr.eVMISensorPort;
                            //            objsetp.BinID = itembr.ID;
                            //            objsetp.IsDeleted = false;
                            //            objsetp.IsArchived = false;
                            //            objsetp.CompanyID = itembr.CompanyID;
                            //            objsetp.Room = itembr.Room;
                            //            objsetp.ItemGUID = itembr.ItemGUID;
                            //            objItemLocationeVMISetupDAL.Insert(objsetp);
                            //        }
                            //    }
                            //}

                            #endregion

                            Session["BinReplanish" + itemGUIDString] = null;

                            if (insertedBinIds != null && insertedBinIds.Any())
                            {
                                foreach (var binId in insertedBinIds)
                                {
                                    var bin = lstBinReplanish.Where(e => e.ID.Equals(binId)).FirstOrDefault();
                                    string InventryLocation = bin.BinNumber;
                                    ItemLocationDetailsDAL objItemLocationDetailsDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                                    BinMasterDTO objBin = new BinMasterDTO();
                                    if (!string.IsNullOrWhiteSpace(InventryLocation))
                                    {
                                        objBin = objBinMasterDAL.GetItemBinPlain(objDTO.GUID, InventryLocation, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, false);
                                        if (objBin == null)
                                        {
                                            objBin = new BinMasterDTO();
                                        }
                                    }

                                    if ((bin.CustomerOwnedQuantity.GetValueOrDefault(0) + bin.ConsignedQuantity.GetValueOrDefault(0)) > 0)
                                    {
                                        ItemLocationDetailsDTO objItemLocationDetailsDTO = new ItemLocationDetailsDTO();
                                        objItemLocationDetailsDTO.Room = SessionHelper.RoomID;
                                        objItemLocationDetailsDTO.CompanyID = SessionHelper.CompanyID;
                                        objItemLocationDetailsDTO.ItemGUID = objDTO.GUID;
                                        objItemLocationDetailsDTO.BinID = objBin.ID;
                                        objItemLocationDetailsDTO.BinNumber = objBin.BinNumber;
                                        objItemLocationDetailsDTO.CustomerOwnedQuantity = bin.CustomerOwnedQuantity.GetValueOrDefault(0);
                                        objItemLocationDetailsDTO.ConsignedQuantity = bin.ConsignedQuantity.GetValueOrDefault(0);
                                        objItemLocationDetailsDTO.SerialNumber = null;
                                        objItemLocationDetailsDTO.LotNumber = null;
                                        objItemLocationDetailsDTO.Expiration = null;
                                        objItemLocationDetailsDTO.Received = DateTime.UtcNow.ToString("MM/dd/yyyy");
                                        objItemLocationDetailsDTO.ReceivedDate = DateTime.UtcNow;
                                        objItemLocationDetailsDTO.IsDeleted = false;
                                        objItemLocationDetailsDTO.IsArchived = false;
                                        objItemLocationDetailsDTO.Created = DateTimeUtility.DateTimeNow;
                                        objItemLocationDetailsDTO.Updated = DateTimeUtility.DateTimeNow;
                                        objItemLocationDetailsDTO.LastUpdatedBy = SessionHelper.UserID;
                                        objItemLocationDetailsDTO.CreatedBy = SessionHelper.UserID;
                                        objItemLocationDetailsDTO.GUID = Guid.NewGuid();
                                        objItemLocationDetailsDTO.InsertedFrom = "itemMasterOnHandInsert-DirectImport";
                                        objItemLocationDetailsDTO.Cost = objDTO.Cost;
                                        objItemLocationDetailsDAL.ItemLocationDetailsImportSave(objItemLocationDetailsDTO, SessionUserId,enterpriseId);
                                    }
                                }
                            }

                            //Auto cart entry
                            //if (objDTO.ItemType == 3)
                            //{
                            //    IEnumerable<KitDetailDTO> listKitDtl = null;
                            //    KitDetailDAL objKitDetl = new KitDetailDAL(SessionHelper.EnterPriseDBName);
                            //    listKitDtl = objKitDetl.GetCachedData(SessionHelper.RoomID, CompanyID, false, false, true);
                            //    if (listKitDtl != null && listKitDtl.Count() > 0)
                            //    {
                            //        listKitDtl = listKitDtl.Where(x => x.KitGUID == objDTO.GUID).ToList();
                            //        if (listKitDtl != null && listKitDtl.Count() > 0)
                            //        {
                            //            foreach (var item in listKitDtl)
                            //            {
                            //                new CartItemDAL(SessionHelper.EnterPriseDBName).AutoCartUpdateByCode((Guid)item.ItemGUID, objDTO.CreatedBy ?? 0, SessionHelper.EnterPriceID, objDTO.IsOnlyFromItemUI);
                            //            }
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //new CartItemDAL(SessionHelper.EnterPriseDBName).AutoCartUpdateByCode(objDTO.GUID, objDTO.LastUpdatedBy ?? 0, "Web", "InventoryControler>> ItemSave");
                           // new CartItemDAL(SessionHelper.EnterPriseDBName).AutoCartUpdateByCode(objDTO.GUID, objDTO.LastUpdatedBy ?? 0, "Web", "Inventory >> Create Item", SessionUserId);

                            //}

                            //new CartItemDAL(SessionHelper.EnterPriseDBName).AutoCartUpdateByCode(objDTO.GUID, objDTO.CreatedBy ?? 0, SessionHelper.EnterPriceID, objDTO.IsOnlyFromItemUI);
                            //obj.EditItemQtyCache(objDTO.ID, SessionHelper.CompanyID, SessionHelper.RoomID);
                        }

                        ReturnVal = obj.Edit(objDTO, SessionUserId, SessionHelper.EnterPriceID, true,IsAutoSOTLater:true);
                        //try
                        //{
                        //    if (!string.IsNullOrEmpty(defaultSupplierPartNo) && !string.IsNullOrWhiteSpace(defaultSupplierPartNo))
                        //    {

                        //        var quickBookDBName = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["eTurnsQuickBookDBName"]);
                        //        SolumTokenDetailDAL solumTokenDetailDAL = new SolumTokenDetailDAL(quickBookDBName);
                        //        var solumStore = solumTokenDetailDAL.GetSolumStoreByRoomId(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);

                        //        if (solumStore != null && solumStore.ID > 0)
                        //        {
                        //            string solumAIMSBaseURL = "https://eastus.common.solumesl.com/common/api/";

                        //            if (!string.IsNullOrWhiteSpace(Convert.ToString(ConfigurationManager.AppSettings["SolumAIMSBaseURL"])))
                        //            {
                        //                solumAIMSBaseURL = Convert.ToString(ConfigurationManager.AppSettings["SolumAIMSBaseURL"]);
                        //            }

                        //            HttpClient client = new HttpClient();
                        //            client.BaseAddress = new Uri(string.Format(solumAIMSBaseURL));
                        //            client.DefaultRequestHeaders.Accept.Clear();
                        //            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        //            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", solumStore.AccessToken);
                        //            var requestURL = "v2/common/config/article/info?company=" + solumStore.CompanyName + "&store=" + solumStore.StationCode + "&articleId=" + defaultSupplierPartNo.Trim();
                        //            HttpResponseMessage responseMessage = client.GetAsync(requestURL).Result;
                        //            string respstr = responseMessage.Content.ReadAsStringAsync().Result;
                        //            CommonFunctions.SaveLogInTextFile(" ItemSave >> article info response string of request URL: " + requestURL + " , Response: " + respstr + ": " + System.DateTime.UtcNow);
                        //            var article = JsonConvert.DeserializeObject<ArticleInfoDTO>(respstr);

                        //            if (article != null && !string.IsNullOrEmpty(article.responseMessage) && !string.IsNullOrWhiteSpace(article.responseMessage) && article.responseMessage.ToLower() == "success")
                        //            {
                        //                var articles = article.articleList;
                        //                if (articles != null && articles.Any() && articles.Count() > 0 && articles[0].data != null)
                        //                {
                        //                    if ((objDTO.OnOrderQuantity.GetValueOrDefault(0) > 0 && articles[0].data.ON_ORDER != "1") || (objDTO.OnOrderQuantity.GetValueOrDefault(0) <= 0 
                        //                        && articles[0].data.ON_ORDER != "0"))
                        //                    {
                        //                        var onOrder = objDTO.OnOrderQuantity.GetValueOrDefault(0) > 0 ? "1" : "0";
                        //                        articles[0].data.ON_ORDER = onOrder;

                        //                        //var articleData = new SolumArticle();
                        //                        List<Article> articleList = new List<Article>();
                        //                        var item = new Article();
                        //                        //item.companyCode = solumStore.CompanyName;
                        //                        //item.stationCode = articles[0].stationCode;
                        //                        item.articleId = defaultSupplierPartNo;
                        //                        //item.articleName = objDTO.ItemNumber;
                        //                        item.data = articles[0].data;
                        //                        articleList.Add(item);
                        //                        //articleData.dataList = articleList;

                        //                        var postURL = "v2/common/articles?company=" + solumStore.CompanyName + "&store=" + solumStore.StationCode;
                        //                        HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(articleList.ToArray()), Encoding.UTF8);
                        //                        //HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(articleData), Encoding.UTF8);
                        //                        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        //                        HttpResponseMessage postResponseMessage = client.PostAsync(postURL, httpContent).Result;
                        //                        //string response = postResponseMessage.Content.ReadAsStringAsync().Result;
                        //                        //var authenticationResponse = JsonConvert.DeserializeObject<SolumArticlePostResponse>(response);
                        //                    }
                        //                }
                        //            }
                        //        }

                        //    }
                        //}
                        //catch(Exception ex)
                        //{
                        //    CommonFunctions.SaveLogInTextFile(" Error on ItemSave >> solum article info exception: " + ex.Message ?? string.Empty + " : " + System.DateTime.UtcNow);
                        //}
                        
                    }

                }

                #endregion
            }

            Session["ItemMasterList"] = null;
            Session["BinReplanish" + itemGUIDString] = null;

            if (!string.IsNullOrWhiteSpace(DestinationModule) && status == "ok")
            {
                if (DestinationModule == "CartItemMaster")
                {
                    CartItemDTO objCartItemDTO = new CartItemDTO();
                    objCartItemDTO.Created = DateTimeUtility.DateTimeNow;
                    objCartItemDTO.CreatedByName = SessionHelper.UserName;
                    objCartItemDTO.CreatedBy = SessionHelper.UserID;
                    objCartItemDTO.Updated = DateTimeUtility.DateTimeNow;
                    objCartItemDTO.UpdatedByName = SessionHelper.UserName;
                    objCartItemDTO.LastUpdatedBy = SessionHelper.UserID;
                    objCartItemDTO.ID = 0;
                    objCartItemDTO.ItemNumber = objDTO.ItemNumber;
                    objCartItemDTO.ItemGUID = objDTO.GUID;
                    objCartItemDTO.UDF1 = "";
                    objCartItemDTO.UDF2 = "";
                    objCartItemDTO.UDF3 = "";
                    objCartItemDTO.UDF4 = "";
                    objCartItemDTO.UDF5 = "";
                    objCartItemDTO.ReplenishType = "";
                    objCartItemDTO.ItemGUID = objDTO.GUID;
                    objCartItemDTO.CompanyID = SessionHelper.CompanyID;
                    objCartItemDTO.Room = SessionHelper.RoomID;
                    objCartItemDTO.Quantity = SupplierCatalogQty ?? 0;
                    objCartItemDTO.IsDeleted = false;
                    objCartItemDTO.IsArchived = false;
                    objCartItemDTO.WhatWhereAction = "Inventory";
                    new CartItemDAL(SessionHelper.EnterPriseDBName).SaveCartItem(objCartItemDTO, SessionUserId,SessionHelper.EnterPriceID);
                }
                if (DestinationModule == "OrderMaster")
                {
                    if (SupplierCatalogOrderGUID != null)
                    {
                        OrderDetailsDAL objOrderDetailsDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                        OrderDetailsDTO objOrderDetailsDTO = new OrderDetailsDTO();
                        objOrderDetailsDTO.Room = SessionHelper.RoomID;
                        objOrderDetailsDTO.RoomName = SessionHelper.RoomName;
                        objOrderDetailsDTO.CreatedBy = SessionHelper.UserID;
                        objOrderDetailsDTO.CreatedByName = SessionHelper.UserName;
                        objOrderDetailsDTO.UpdatedByName = SessionHelper.UserName;
                        objOrderDetailsDTO.LastUpdatedBy = SessionHelper.UserID;
                        objOrderDetailsDTO.CompanyID = SessionHelper.CompanyID;
                        // need to disscuss this... CP & ND
                        objOrderDetailsDTO.OrderGUID = Guid.Parse(SupplierCatalogOrderGUID.ToString());
                        //objOrderDetailsDTO.ItemID = objDTO.ID;
                        objOrderDetailsDTO.ItemGUID = objDTO.GUID;
                        //objOrderDetailsDTO.ItemDetail = objDTO;
                        objOrderDetailsDTO.RequestedQuantity = SupplierCatalogQty ?? 0;
                        objOrderDetailsDTO.AddedFrom = "Web";
                        objOrderDetailsDTO.EditedFrom = "Web";
                        objOrderDetailsDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objOrderDetailsDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objOrderDetailsDAL.Insert(objOrderDetailsDTO, SessionUserId,SessionHelper.EnterPriceID);
                    }
                }
            }

            //#region WI-7657 Solum Add/Update Item details
            //CostUOMValue = Convert.ToString(objCostUOMDAL.GetCostUOMByID(objDTO.CostUOMID.GetValueOrDefault(0)).CostUOMValue);
            //if (defaultSupplierPartNo != null && defaultSupplierPartNo != string.Empty)
            //{
            //    obj.AddUpdateSolumnProduct(defaultSupplierPartNo, objDTO.ItemNumber, objDTO.Description, objDTO.MinimumQuantity.ToString(), objDTO.MaximumQuantity.ToString(), objDTO.DefaultReorderQuantity.ToString(), CostUOMValue, objDTO.DefaultLocationName, (objDTO.OnOrderQuantity > 0 ? "1" : "0"), SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);
            //}
            //#endregion

            UDFDAL objUDFApiCnt = new UDFDAL(SessionHelper.EnterPriseDBName);
            UDFController objUDFCnt = new UDFController();
            IEnumerable<UDFDTO> ItemMasterDB = objUDFApiCnt.GetNonDeletedUDFsByUDFTableNamePlain("ItemMaster", SessionHelper.RoomID, SessionHelper.CompanyID);
 
            foreach (var i in ItemMasterDB)
             {
                    if (i.UDFControlType.ToLower().Contains("dropdown editable"))
                    {
                        if (i.UDFColumnName == "UDF1" && !string.IsNullOrWhiteSpace(objDTO.UDF1))
                        {
                        objUDFCnt.InsertUDFOption(i.ID, objDTO.UDF1, "ItemMaster", null, false, false, "");
                        }
                        else if (i.UDFColumnName == "UDF2" && !string.IsNullOrWhiteSpace(objDTO.UDF2))
                        {
                        objUDFCnt.InsertUDFOption(i.ID, objDTO.UDF2, "ItemMaster", null, false, false, "");
                        }
                        else if (i.UDFColumnName == "UDF3" && !string.IsNullOrWhiteSpace(objDTO.UDF3))
                        {
                        objUDFCnt.InsertUDFOption(i.ID, objDTO.UDF3, "ItemMaster", null, false, false, "");
                        }
                        else if (i.UDFColumnName == "UDF4" && !string.IsNullOrWhiteSpace(objDTO.UDF4))
                        {
                        objUDFCnt.InsertUDFOption(i.ID, objDTO.UDF4, "ItemMaster", null, false, false, "");
                        }
                        else if (i.UDFColumnName == "UDF5" && !string.IsNullOrWhiteSpace(objDTO.UDF5))
                        {
                        objUDFCnt.InsertUDFOption(i.ID, objDTO.UDF5, "ItemMaster", null, false, false, "");
                        }
                        else if (i.UDFColumnName == "UDF6" && !string.IsNullOrWhiteSpace(objDTO.UDF6))
                        {
                            objUDFCnt.InsertUDFOption(i.ID, objDTO.UDF6, "ItemMaster", null, false, false, "");
                        }
                        else if (i.UDFColumnName == "UDF7" && !string.IsNullOrWhiteSpace(objDTO.UDF7))
                        {
                            objUDFCnt.InsertUDFOption(i.ID, objDTO.UDF7, "ItemMaster", null, false, false, "");
                        }
                        else if (i.UDFColumnName == "UDF8" && !string.IsNullOrWhiteSpace(objDTO.UDF8))
                        {
                            objUDFCnt.InsertUDFOption(i.ID, objDTO.UDF8, "ItemMaster", null, false, false, "");
                        }
                        else if (i.UDFColumnName == "UDF9" && !string.IsNullOrWhiteSpace(objDTO.UDF9))
                        {
                            objUDFCnt.InsertUDFOption(i.ID, objDTO.UDF9, "ItemMaster", null, false, false, "");
                        }
                        else if (i.UDFColumnName == "UDF10" && !string.IsNullOrWhiteSpace(objDTO.UDF10))
                        {
                            objUDFCnt.InsertUDFOption(i.ID, objDTO.UDF10, "ItemMaster", null, false, false, "");
                        }
                }
            }

            if (TempItemID > 0)
                objDTO = obj.GetItemWithMasterTableJoins(TempItemID, null, SessionHelper.RoomID, SessionHelper.CompanyID);

            if(status == "ok" && (SolumMappedLabels != null && SolumMappedLabels != "") || (SolumUnmappedLabels != null && SolumUnmappedLabels != ""))
            {
                obj.AssignUnAssignSolumLabelsToItem(SessionHelper.EnterPriceID,SessionHelper.CompanyID,SessionHelper.RoomID , defaultSupplierPartNo, SolumMappedLabels, SolumUnmappedLabels);
                #region Insert History
                obj.InsertAssignUnAssignHistory(objDTO.GUID, defaultSupplierPartNo, SolumMappedLabels, SolumUnmappedLabels, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID); 
                #endregion
            }
            return Json(new { Message = message, Status = status, DestinationModule = DestinationModule, ItemID = TempItemID, ItemDTO = objDTO }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ItemIMCreate(SupplierCatalogItemDTO SupplierCatalogItem)
        {
            ItemMasterDTO objDTO = new ItemMasterDTO();
            //objDTO.ItemNumber = "#I" + NewNumber;
            objDTO.ID = 0;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.CreatedByName = SessionHelper.UserName;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.RoomName = SessionHelper.RoomName;
            objDTO.GUID = Guid.NewGuid();
            objDTO.Trend = false;
            objDTO.IsAutoInventoryClassification = false;
            objDTO.Taxable = false;
            objDTO.Consignment = false;
            objDTO.IsTransfer = false;
            objDTO.IsPurchase = true;
            objDTO.ItemType = 1;
            objDTO.SerialNumberTracking = false;
            objDTO.LotNumberTracking = false;
            objDTO.DateCodeTracking = false;
            objDTO.Unit = "";
            objDTO.Cost = 0;
            //objDTO.ItemUniqueNumber = UniqueId.Get();

            string itemGuidString = Convert.ToString(objDTO.GUID);
            Session["BinReplanish" + itemGuidString] = null;
            Session["ItemManufacture" + itemGuidString] = null;
            Session["ItemSupplier" + itemGuidString] = null;
            Session["ItemKitDetail" + itemGuidString] = null;

            //get room's configuration status for replanishment type and consignment
            RoomDAL objRoomDal = new RoomDAL(SessionHelper.EnterPriseDBName);
            //RoomDTO ROOMDTO = objRoomDal.GetRoomByIDPlain(SessionHelper.RoomID);
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            string columnList = "ID,RoomName,MethodOfValuingInventory,GlobMarkupParts,DefaultSupplierID,DefaultBinName,ReplenishmentType,IsConsignment,IsAllowOrderCostuom";
            RoomDTO ROOMDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");

            if (ROOMDTO != null)
            {
                ViewBag.MethodOfValuingInventory = ROOMDTO.MethodOfValuingInventory;

                if (ROOMDTO.GlobMarkupParts.GetValueOrDefault(0) > 0)
                    objDTO.Markup = ROOMDTO.GlobMarkupParts.GetValueOrDefault(0);

                ////////////////////////////////default supplier logic/////////////////////////////////////////
                List<ItemSupplierDetailsDTO> lstItemSupplier = new List<ItemSupplierDetailsDTO>();
                if (ROOMDTO.DefaultSupplierID > 0)
                {
                    objDTO.SupplierID = ROOMDTO.DefaultSupplierID.GetValueOrDefault(0);

                    //get supplier name from supplier id
                    SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                    SupplierMasterDTO objSupplierMasterDTO = new SupplierMasterDTO();

                    if (SessionHelper.UserSupplierIds != null && SessionHelper.UserSupplierIds.Any())
                    {
                        string strSupplierIds = string.Empty;
                        strSupplierIds = string.Join(",", SessionHelper.UserSupplierIds);

                        var suppliers = objSupplierMasterDAL.GetSupplierByIDsPlain(strSupplierIds, SessionHelper.RoomID, SessionHelper.CompanyID);

                        foreach (var supplier in suppliers)
                        {
                            lstItemSupplier.Add(new ItemSupplierDetailsDTO()
                            {
                                ID = 0,
                                SupplierName = supplier.SupplierName,
                                SupplierID = supplier.ID,
                                IsDefault = true,
                                SessionSr = lstItemSupplier.Count + 1,
                                ItemGUID = objDTO.GUID,
                                Room = SessionHelper.RoomID,
                                CompanyID = SessionHelper.CompanyID,
                                Updated = DateTimeUtility.DateTimeNow,
                                LastUpdatedBy = SessionHelper.UserID,
                                Created = DateTimeUtility.DateTimeNow,
                                CreatedBy = SessionHelper.UserID
                            });
                        }
                    }
                    else
                    {
                        objSupplierMasterDTO = objSupplierMasterDAL.GetSupplierByIDPlain(ROOMDTO.DefaultSupplierID.GetValueOrDefault(0));
                        lstItemSupplier.Add(new ItemSupplierDetailsDTO()
                        {
                            ID = 0,
                            SupplierName = objSupplierMasterDTO.SupplierName,
                            SupplierID = ROOMDTO.DefaultSupplierID.GetValueOrDefault(0),
                            IsDefault = true,
                            SessionSr = lstItemSupplier.Count + 1,
                            ItemGUID = objDTO.GUID,
                            Room = SessionHelper.RoomID,
                            CompanyID = SessionHelper.CompanyID,
                            Updated = DateTimeUtility.DateTimeNow,
                            LastUpdatedBy = SessionHelper.UserID,
                            Created = DateTimeUtility.DateTimeNow,
                            CreatedBy = SessionHelper.UserID
                        });
                    }
                }
                Session["ItemSupplier" + itemGuidString] = lstItemSupplier;
                ///////////////////////////////////////////////////////////////////

                if (ROOMDTO.DefaultBinName != null && ROOMDTO.DefaultBinName.Length > 0)
                {
                    objDTO.DefaultLocation = 0;
                    objDTO.DefaultLocationName = ROOMDTO.DefaultBinName;
                }
                else
                {
                    objDTO.DefaultLocation = 0;
                    objDTO.DefaultLocationName = "";
                }

                if (ROOMDTO.ReplenishmentType != null)
                {
                    if (ROOMDTO.ReplenishmentType == "3")
                    {
                        ViewBag.LockReplenishmentType = false;
                        objDTO.IsItemLevelMinMaxQtyRequired = true;
                    }
                    else
                    {
                        ViewBag.LockReplenishmentType = true;
                        if (ROOMDTO.ReplenishmentType == "1")
                        {
                            objDTO.IsItemLevelMinMaxQtyRequired = true;
                        }
                        else
                        {
                            objDTO.IsItemLevelMinMaxQtyRequired = false;
                        }
                    }
                }
                else
                {
                    objDTO.IsItemLevelMinMaxQtyRequired = true;
                    ViewBag.LockReplenishmentType = true;
                }

                //consignemtn
                if (ROOMDTO.IsConsignment)
                {
                    ViewBag.LockConsignment = false;
                }
                else
                {
                    ViewBag.LockConsignment = true;
                }
                if (ROOMDTO.IsAllowOrderCostuom)
                {
                    ViewBag.LockIsAllowOrderCostuom = false;
                }
                else
                {
                    ViewBag.LockIsAllowOrderCostuom = true;
                }
            }
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
            ViewBag.UDFs = objUDFController.GetUDFDataPageWise("ItemMaster");

            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }

            CommonDAL objCommon = new CommonDAL(SessionHelper.EnterPriseDBName);
            ViewBag.ManufacturerIDBag = objCommon.GetDDData("ManufacturerMaster", "Manufacturer", SessionHelper.CompanyID, SessionHelper.RoomID);
            ViewBag.SupplierIDBag = objCommon.GetDDData("SupplierMaster", "SupplierName", SessionHelper.CompanyID, SessionHelper.RoomID);
            ViewBag.CategoryIDBag = objCommon.GetDDData("CategoryMaster", "Category", SessionHelper.CompanyID, SessionHelper.RoomID);
            ViewBag.GLAccountIDBag = objCommon.GetDDData("GLAccountMaster", "GLAccount", SessionHelper.CompanyID, SessionHelper.RoomID, false);
            ViewBag.UOMIDBag = objCommon.GetDDData("UnitMaster", "Unit", SessionHelper.CompanyID, SessionHelper.RoomID);
            ViewBag.CostUOMBag = objCommon.GetDDData("CostUOMMaster", "CostUOM", SessionHelper.CompanyID, SessionHelper.RoomID);
            ViewBag.ItemTypeBag = GetItemTypeOptions();
            ViewBag.TrendingSettingBag = GetTrendingSettings();
            ViewBag.InventoryClassificationBag = objCommon.GetDDData("InventoryClassificationMaster", "InventoryClassification", SessionHelper.CompanyID, SessionHelper.RoomID, false);
            ViewBag.DefaultLocationBag = objCommon.GetDDData("BinMaster", "BinNumber", " IsStagingLocation != 1 AND ", SessionHelper.CompanyID, SessionHelper.RoomID);
            objDTO.IsOnlyFromItemUI = true;
            objDTO.DefaultReorderQuantity = 1;
            objDTO.DefaultPullQuantity = 1;

            CostUOMMasterDAL oCostUOMDal = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName);
            try
            {
                CostUOMMasterDTO oCostUOMMaster = oCostUOMDal.GetCostUOMByName("E", SessionHelper.RoomID, SessionHelper.CompanyID);
                objDTO.CostUOMID = oCostUOMMaster.ID;
                objDTO.CostUOMName = oCostUOMMaster.CostUOM;
            }
            catch { }

            UnitMasterDAL objUnitDal = new UnitMasterDAL(SessionHelper.EnterPriseDBName);
            UnitMasterDTO objUnit = objUnitDal.GetUnitByNamePlain(SessionHelper.RoomID, SessionHelper.CompanyID, false, "EA");
            if (objDTO != null && objDTO.ID > 0)
            {
                objDTO.UOMID = objUnit.ID;
                objDTO.Unit = objUnit.Unit;
            }

            if (!string.IsNullOrWhiteSpace(objDTO.ItemImageExternalURL))
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(objDTO.ItemImageExternalURL);
                    request.Method = "GET";
                    try
                    {
                        WebResponse response = request.GetResponse();
                        if (response.ContentType.ToString().ToLower().IndexOf("image") >= 0)
                        {

                        }
                        else
                        {
                            objDTO.ItemImageExternalURL = string.Empty;
                        }
                    }
                    catch
                    {
                        objDTO.ItemImageExternalURL = string.Empty;
                    }
                }
                catch
                {
                    objDTO.ItemImageExternalURL = string.Empty;
                }
            }

            objDTO.ID = 0;
            objDTO.ImageType = "ExternalImage";
            objDTO.ItemLink2ImageType = "InternalLink";

            return PartialView("_CreateItemIM", objDTO);



        }

        public ActionResult ItemIMEdit(string ItemGUID)
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
            }

            if (IsDeleted || IsArchived || IsHistory)
            {
                ViewBag.ViewOnly = true;
            }

            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            ItemMasterDTO objDTO = obj.GetItemWithMasterTableJoins(null, Guid.Parse(ItemGUID), SessionHelper.RoomID, SessionHelper.CompanyID);

            if (objDTO != null)
            {
                objDTO.CreatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                objDTO.UpdatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
            }
            UDFController objUDFController = new UDFController();
            ViewBag.UDFs = objUDFController.GetUDFDataPageWise("ItemMaster");
            if (objDTO != null)
            {
                ViewData["UDF1"] = objDTO.UDF1;
                ViewData["UDF2"] = objDTO.UDF2;
                ViewData["UDF3"] = objDTO.UDF3;
                ViewData["UDF4"] = objDTO.UDF4;
                ViewData["UDF5"] = objDTO.UDF5;
            }

            CommonDAL objCommon = new CommonDAL(SessionHelper.EnterPriseDBName);
            ViewBag.ManufacturerIDBag = objCommon.GetDDData("ManufacturerMaster", "Manufacturer", SessionHelper.CompanyID, SessionHelper.RoomID);
            ViewBag.SupplierIDBag = objCommon.GetDDData("SupplierMaster", "SupplierName", SessionHelper.CompanyID, SessionHelper.RoomID);
            ViewBag.CategoryIDBag = objCommon.GetDDData("CategoryMaster", "Category", SessionHelper.CompanyID, SessionHelper.RoomID);
            ViewBag.GLAccountIDBag = objCommon.GetDDData("GLAccountMaster", "GLAccount", SessionHelper.CompanyID, SessionHelper.RoomID, false);
            ViewBag.CostUOMBag = objCommon.GetDDData("CostUOMMaster", "CostUOM", SessionHelper.CompanyID, SessionHelper.RoomID);
            ViewBag.UOMIDBag = objCommon.GetDDData("UnitMaster", "Unit", SessionHelper.CompanyID, SessionHelper.RoomID);
            ViewBag.ItemTypeBag = GetItemTypeOptions();
            ViewBag.TrendingSettingBag = GetTrendingSettings();
            ViewBag.InventoryClassificationBag = objCommon.GetDDData("InventoryClassificationMaster", "InventoryClassification", SessionHelper.CompanyID, SessionHelper.RoomID, false);
            ViewBag.DefaultLocationBag = objCommon.GetDDData("BinMaster", "BinNumber", " IsStagingLocation != 1 AND ", SessionHelper.CompanyID, SessionHelper.RoomID);

            if (objDTO.IsItemLevelMinMaxQtyRequired == null)
            {
                objDTO.IsItemLevelMinMaxQtyRequired = false;
            }
            objDTO.Unit = "";
            objDTO.IsOnlyFromItemUI = true;

            //Edit mode --- Bin Replanish fill in to session....
            if (objDTO.ItemType != 4)
            {
                BinMasterDAL objItemLocationLevelQuanityDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                List<BinMasterDTO> lstBinReplanish = objItemLocationLevelQuanityDAL.GetItemLocations(objDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                lstBinReplanish = lstBinReplanish.Where(t => t.IsStagingLocation == false).ToList();
                for (int i = 0; i < lstBinReplanish.Count(); i++)
                {
                    if (lstBinReplanish[i].ID == objDTO.DefaultLocation)
                        lstBinReplanish[i].IsDefault = true;
                    else
                        lstBinReplanish[i].IsDefault = false;
                }
                Session["BinReplanish" + ItemGUID] = lstBinReplanish;
            }

            if (objDTO.ItemType == 3)
            {
                KitDetailDAL objKitDetailDAL = new KitDetailDAL(SessionHelper.EnterPriseDBName);
                Session["ItemKitDetail" + ItemGUID] = objKitDetailDAL.GetAllRecordsByKitGUID(objDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, true).ToList();
            }

            ItemManufacturerDetailsDAL objItemManufacturerDetailsDAL = new ItemManufacturerDetailsDAL(SessionHelper.EnterPriseDBName);
            Session["ItemManufacture" + ItemGUID] = objItemManufacturerDetailsDAL.GetManufacturerByItemGuidNormal(SessionHelper.RoomID, SessionHelper.CompanyID, objDTO.GUID, false);

            ItemSupplierDetailsDAL objItemSupplierDAL = new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName);
            Session["ItemSupplier" + ItemGUID] = objItemSupplierDAL.GetSuppliersByItemGuidNormal(SessionHelper.RoomID, SessionHelper.CompanyID, objDTO.GUID);

            RoomDAL objRoomDal = new RoomDAL(SessionHelper.EnterPriseDBName);
            //RoomDTO ROOMDTO = objRoomDal.GetRoomByIDPlain(SessionHelper.RoomID);
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            string columnList = "ID,RoomName,MethodOfValuingInventory,IsConsignment,IsAllowOrderCostuom";
            RoomDTO ROOMDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");

            if (ROOMDTO != null)
            {
                //consignemtn
                if (ROOMDTO.IsConsignment)
                {
                    ViewBag.LockConsignment = false;
                }
                else
                {
                    ViewBag.LockConsignment = true;
                }
                if (ROOMDTO.IsAllowOrderCostuom)
                {
                    ViewBag.LockIsAllowOrderCostuom = false;
                }
                else
                {
                    ViewBag.LockIsAllowOrderCostuom = true;
                }
                ViewBag.MethodOfValuingInventory = ROOMDTO.MethodOfValuingInventory;
            }

            bool IsConsignedEditable = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowChangeConsignedItems);
            if (!IsConsignedEditable && objDTO.Consignment)
            {
                ViewBag.ViewOnly = true;
            }

            if (objDTO != null && objDTO.IsAutoInventoryClassification)
            {
                //if (objDTO.Turns != null || objDTO.ExtendedCost != null)
                //{
                int Icost = checkInventoryclassificationTurn(objDTO.Turns.GetValueOrDefault(0), objDTO.ExtendedCost.GetValueOrDefault(0));
                if (Icost > 0)
                {
                    objDTO.InventoryClassification = Icost;
                }
                //}
            }

            if (!string.IsNullOrWhiteSpace(objDTO.ItemImageExternalURL))
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(objDTO.ItemImageExternalURL);
                    request.Method = "GET";
                    try
                    {
                        WebResponse response = request.GetResponse();
                        if (response.ContentType.ToString().ToLower().IndexOf("image") >= 0)
                        {

                        }
                        else
                        {
                            objDTO.ItemImageExternalURL = string.Empty;
                        }
                    }
                    catch
                    {
                        objDTO.ItemImageExternalURL = string.Empty;
                    }
                }
                catch
                {
                    objDTO.ItemImageExternalURL = string.Empty;
                }
            }

            return PartialView("_CreateItemIM", objDTO);
        }

        [HttpPost]
        public JsonResult ItemIMSave(ItemMasterDTO objDTO, string DestinationModule, double? SupplierCatalogQty, Guid? SupplierCatalogOrderGUID)
        {
            if (objDTO.ItemType != 4) // if labour in that case no need to check modal validation as it always invalid
            {
                if (objDTO.IsItemLevelMinMaxQtyRequired == true)
                {
                    bool va = ModelState.IsValid;
                    if (!ModelState.IsValid)
                    {
                        var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();

                        if (ModelState["CriticalQuantity"].Errors.Count > 0)
                        {
                            return Json(new { Message = ModelState["CriticalQuantity"].Errors[0].ErrorMessage, Status = "Fa" }, JsonRequestBehavior.AllowGet);
                        }
                        else if (ModelState["MinimumQuantity"].Errors.Count > 0)
                        {
                            return Json(new { Message = ModelState["MinimumQuantity"].Errors[0].ErrorMessage, Status = "Fa" }, JsonRequestBehavior.AllowGet);
                        }
                        else if (ModelState["DefaultReorderQuantity"].Errors.Count > 0)
                        {
                            return Json(new { Message = ModelState["DefaultReorderQuantity"].Errors[0].ErrorMessage, Status = "Fa" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { Message = ResMessage.InvalidModel, Status = "Fa" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            long SessionUserId = SessionHelper.UserID;
            var enterpriseId = SessionHelper.EnterPriceID;
            string message = "";
            string status = "";
            Int64 TempItemID = 0;
            //ItemMasterController obj = new ItemMasterController();
            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

            if (string.IsNullOrEmpty(objDTO.ItemNumber))
            {
                message = string.Format(ResMessage.Required, ResItemMaster.ItemNumber);
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }

            string itemGUIDString = Convert.ToString(objDTO.GUID);

            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.Room = SessionHelper.RoomID;

            // Commented By Esha on 24-09-2014 We have put dropdown instand of auto complete textbox. So now no need to check for new entry. 

            ////Unit new entry logic
            //UnitMasterDAL objUnitMasterDAL = new UnitMasterDAL(SessionHelper.EnterPriseDBName);
            //UnitMasterDTO objUnitMasterDTO = objUnitMasterDAL.GetRecord(objDTO.Unit, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false);
            //if (objUnitMasterDTO == null) // new entry - not matching id and uom
            //{
            //    objUnitMasterDTO = new UnitMasterDTO();
            //    objUnitMasterDTO.ID = 0;
            //    objUnitMasterDTO.CompanyID = SessionHelper.CompanyID;
            //    objUnitMasterDTO.Created = System.DateTime.Now;
            //    objUnitMasterDTO.CreatedBy = SessionHelper.UserID;
            //    objUnitMasterDTO.CreatedByName = SessionHelper.UserName;
            //    objUnitMasterDTO.IsArchived = false;
            //    objUnitMasterDTO.IsDeleted = false;
            //    objUnitMasterDTO.LastUpdatedBy = SessionHelper.UserID;
            //    objUnitMasterDTO.Room = SessionHelper.RoomID;
            //    objUnitMasterDTO.RoomName = SessionHelper.RoomName;
            //    objUnitMasterDTO.Updated = System.DateTime.Now;
            //    objUnitMasterDTO.UpdatedByName = SessionHelper.UserName;
            //    objUnitMasterDTO.Unit = objDTO.Unit;
            //    objUnitMasterDTO.ID = objUnitMasterDAL.Insert(objUnitMasterDTO);
            //    objDTO.UOMID = objUnitMasterDTO.ID;
            //}
            //else { objDTO.UOMID = objUnitMasterDTO.ID; }
            ////End- Unit entry logic

            //UDF New Entry Logic
            //UDFOptionDAL oUDFOptionDAL = new UDFOptionDAL(SessionHelper.EnterPriseDBName);
            //oUDFOptionDAL.InsertNewDropdownEditableOptions(objDTO.UDF1, objDTO.UDF2, objDTO.UDF3, objDTO.UDF4, objDTO.UDF5, "ItemMaster", SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);

            //Category new entry logic
            if (!string.IsNullOrEmpty(objDTO.CategoryName))
            {
                CategoryMasterDAL objCategoryMasterDAL = new CategoryMasterDAL(SessionHelper.EnterPriseDBName);
                //CategoryMasterDTO objCategoryMasterDTO = objCategoryMasterDAL.GetRecord(objDTO.CategoryName, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false);
                CategoryMasterDTO objCategoryMasterDTO = objCategoryMasterDAL.GetSingleCategoryByNameByRoomID(objDTO.CategoryName, SessionHelper.RoomID, SessionHelper.CompanyID);
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
                    objCategoryMasterDTO.Room = SessionHelper.RoomID;
                    objCategoryMasterDTO.RoomName = SessionHelper.RoomName;
                    objCategoryMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                    objCategoryMasterDTO.UpdatedByName = SessionHelper.UserName;
                    objCategoryMasterDTO.Category = objDTO.CategoryName;
                    objCategoryMasterDTO.CategoryColor = get_random_color();
                    objCategoryMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objCategoryMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objCategoryMasterDTO.AddedFrom = "Web";
                    objCategoryMasterDTO.EditedFrom = "Web";
                    objCategoryMasterDTO.ID = objCategoryMasterDAL.Insert(objCategoryMasterDTO);
                    objDTO.CategoryID = objCategoryMasterDTO.ID;
                }
                else { objDTO.CategoryID = objCategoryMasterDTO.ID; }
            }
            //End- Category entry logic
            // if serial number item then round the quantity fields to zero
            if (objDTO.SerialNumberTracking)
            {
                if (objDTO.IsItemLevelMinMaxQtyRequired == true)
                {
                    objDTO.MinimumQuantity = Math.Round(objDTO.MinimumQuantity.GetValueOrDefault(0), 0);
                    objDTO.MaximumQuantity = Math.Round(objDTO.MaximumQuantity.GetValueOrDefault(0), 0);
                    objDTO.CriticalQuantity = Math.Round(objDTO.CriticalQuantity ?? 0, 0);
                }
                objDTO.DefaultPullQuantity = Math.Round(objDTO.DefaultPullQuantity.GetValueOrDefault(0), 0);
                objDTO.DefaultReorderQuantity = Math.Round(objDTO.DefaultReorderQuantity.GetValueOrDefault(0), 0);
            }

            //Save New Entry to Manufacturer Master and set item dto for Default manufacturer
            List<ItemManufacturerDetailsDTO> lstItemManufactuerEntry = null;

            if (Session["ItemManufacture" + itemGUIDString] != null)
            {
                //Wi-1505
                lstItemManufactuerEntry = ((List<ItemManufacturerDetailsDTO>)Session["ItemManufacture" + itemGUIDString]).Where(t => t.ItemGUID == objDTO.GUID).ToList();
                //lstItemManufactuerEntry = ((List<ItemManufacturerDetailsDTO>)Session["ItemManufacture"]).Where(t => !string.IsNullOrEmpty(t.ManufacturerName)).ToList();

                foreach (var itembr in lstItemManufactuerEntry)
                {

                    /// - logic for Adding supplier if Newly added...
                    ManufacturerMasterDAL objManuMasterDAL = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName);
                    ManufacturerMasterDTO objManufacturerMasterDTO = new ManufacturerMasterDTO();
                    //if (!string.IsNullOrEmpty(itembr.ManufacturerName))
                    objManufacturerMasterDTO = objManuMasterDAL.GetManufacturerByNameNormal(itembr.ManufacturerName, SessionHelper.RoomID, SessionHelper.CompanyID, false);
                    //else
                    //  objManufacturerMasterDTO = objManuMasterDAL.GetRecord(itembr.ManufacturerName, 0, 0, false, false, false);

                    if (objManufacturerMasterDTO == null)
                    {
                        objManufacturerMasterDTO = new ManufacturerMasterDTO();
                        objManufacturerMasterDTO.ID = 0;
                        objManufacturerMasterDTO.Manufacturer = itembr.ManufacturerName;
                        objManufacturerMasterDTO.CompanyID = SessionHelper.CompanyID;
                        objManufacturerMasterDTO.Room = SessionHelper.RoomID;
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

                    if (itembr.IsDefault == true)
                    {
                        objDTO.ManufacturerID = objManufacturerMasterDTO.ID;
                        objDTO.ManufacturerNumber = itembr.ManufacturerNumber;
                    }
                    /// End- logic for Adding supplier if Newly added...
                }

                Session["ItemManufacture" + itemGUIDString] = lstItemManufactuerEntry;
            }


            if (objDTO.ItemType != 4)
            {
                ///Save itemlocationwise quantity to database from the session
                List<ItemSupplierDetailsDTO> lstItemSupplier = null;
                if (Session["ItemSupplier" + itemGUIDString] != null)
                {
                    lstItemSupplier = ((List<ItemSupplierDetailsDTO>)Session["ItemSupplier" + itemGUIDString]).Where(t => !string.IsNullOrEmpty(t.SupplierName) || !string.IsNullOrEmpty(t.SupplierNumber)).ToList();
                    foreach (var itembr in lstItemSupplier)
                    {
                        //if (itembr.ID == 0)
                        //{ 

                        //}
                        itembr.ItemGUID = objDTO.GUID;

                        if (!string.IsNullOrEmpty(itembr.SupplierName))
                        {
                            /// - logic for Adding supplier if Newly added...
                            SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                            SupplierMasterDTO objSupplierMasterDTO = objSupplierMasterDAL.GetSupplierByNamePlain(SessionHelper.RoomID, SessionHelper.CompanyID, false, itembr.SupplierName);
                            if (objSupplierMasterDTO == null)
                            {
                                objSupplierMasterDTO = new SupplierMasterDTO();
                                objSupplierMasterDTO.ID = 0;
                                objSupplierMasterDTO.SupplierName = itembr.SupplierName;
                                objSupplierMasterDTO.SupplierColor = get_random_color();
                                objSupplierMasterDTO.CompanyID = SessionHelper.CompanyID;
                                objSupplierMasterDTO.Room = SessionHelper.RoomID;
                                objSupplierMasterDTO.RoomName = SessionHelper.RoomName;
                                objSupplierMasterDTO.UpdatedByName = SessionHelper.UserName;
                                objSupplierMasterDTO.CreatedByName = SessionHelper.UserName;
                                objSupplierMasterDTO.CreatedBy = SessionHelper.UserID;
                                objSupplierMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                                objSupplierMasterDTO.Created = DateTimeUtility.DateTimeNow;
                                objSupplierMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                                objSupplierMasterDTO.AddedFrom = "Web";
                                objSupplierMasterDTO.EditedFrom = "Web";
                                objSupplierMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                objSupplierMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                objSupplierMasterDTO.ID = objSupplierMasterDAL.Insert(objSupplierMasterDTO);
                                itembr.SupplierID = objSupplierMasterDTO.ID;


                            }

                            if (itembr.IsDefault == true)
                            {
                                objDTO.SupplierID = objSupplierMasterDTO.ID;
                                objDTO.SupplierPartNo = itembr.SupplierNumber;
                            }
                            /// End- logic for Adding supplier if Newly added...
                        }
                    }
                    Session["ItemSupplier" + itemGUIDString] = lstItemSupplier;
                }
            }
            if (objDTO.Cost.HasValue && objDTO.SellPrice.HasValue && objDTO.Markup.HasValue)
            {
                objDTO.Cost = objDTO.Cost;
                objDTO.SellPrice = objDTO.SellPrice;
                if (objDTO.Cost > 0)
                {
                    objDTO.Markup = ((objDTO.SellPrice - objDTO.Cost) / objDTO.Cost) * 100;//((objDTO.SellPrice * 100) / objDTO.Cost) - 100;
                }
                else
                {
                    objDTO.Markup = null;
                }

            }
            else if (objDTO.Cost.HasValue && objDTO.SellPrice.HasValue)
            {
                objDTO.Cost = objDTO.Cost;
                objDTO.SellPrice = objDTO.SellPrice;
                if (objDTO.Cost > 0)
                {
                    objDTO.Markup = ((objDTO.SellPrice - objDTO.Cost) / objDTO.Cost) * 100; // ((objDTO.SellPrice * 100) / objDTO.Cost) - 100;
                }
                else
                {
                    objDTO.Markup = null;
                }

            }
            else if (objDTO.Cost.HasValue && objDTO.Markup.HasValue)
            {
                objDTO.Cost = objDTO.Cost;
                objDTO.Markup = objDTO.Markup;
                objDTO.SellPrice = objDTO.Cost + ((objDTO.Cost * objDTO.Markup) / 100);
            }
            else if (objDTO.Cost.HasValue)
            {
                objDTO.Cost = objDTO.Cost;
                objDTO.Markup = null;
                objDTO.SellPrice = objDTO.Cost;
            }
            objDTO.Markup = null;
            if (objDTO.ID == 0)
            {
                #region "Insert"

                //string strOK = objCDAL.CheckItemDuplication(objDTO.ItemNumber, "add", objDTO.ID, "ItemMaster", "ItemNumber", SessionHelper.RoomID, SessionHelper.CompanyID,objDTO.ItemType);
                string strOK = objCDAL.DuplicateCheck(objDTO.ItemNumber, "add", objDTO.ID, "ItemMaster", "ItemNumber", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResItemMaster.ItemNumber, objDTO.ItemNumber);  //"BinNumber \"" + objDTO.BinNumber + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    if (objDTO.ItemTraking == 1)
                    {
                        objDTO.LotNumberTracking = true;
                        objDTO.SerialNumberTracking = false;
                    }
                    else if (objDTO.ItemTraking == 2)
                    {
                        objDTO.LotNumberTracking = false;
                        objDTO.SerialNumberTracking = true;
                    }
                    objDTO.CreatedByName = SessionHelper.UserName;
                    objDTO.GUID = Guid.NewGuid();
                    //Rename image if uploaded

                    //  objDTO.ItemUniqueNumber = UniqueId.Get();

                    if (objDTO.DefaultLocation == null)
                        objDTO.DefaultLocation = 0;

                    objDTO.WhatWhereAction = "ItemDetails";


                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";


                    obj.Insert(objDTO);
                    TempItemID = objDTO.ID;

                    // Add Location  Code 

                    // End Location Code

                    if (Session["Temp_FileName"] != null)
                    {
                        string UNCPathRoot = HttpRuntime.AppDomainAppPath + System.Configuration.ConfigurationManager.AppSettings["ItemImagePath"].ToString();
                        string strPath = Path.Combine(UNCPathRoot, SessionHelper.CompanyID.ToString() + "/" + Convert.ToString(Session["Temp_FileName"]));
                        FileInfo fi = new FileInfo(strPath);
                        try
                        {
                            fi.MoveTo(Path.Combine(UNCPathRoot, SessionHelper.CompanyID.ToString() + "/" + TempItemID + "." + Convert.ToString(Session["Temp_FileName"]).Split('.')[1]));
                        }
                        catch { }
                        objDTO.ImagePath = TempItemID + "." + Convert.ToString(Session["Temp_FileName"]).Split('.')[1];
                        Session["Temp_FileName"] = null;
                    }

                    //HttpResponseMessage hrmResult = obj.PutRecord(TempItemID, objDTO);
                    objDTO.WhatWhereAction = "itemDetails";
                    bool ReturnVal = obj.Edit(objDTO, SessionUserId, SessionHelper.EnterPriceID);
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
                            if (Session["ItemKitDetail" + itemGUIDString] != null)
                            {
                                lstKitDetailDTO = ((List<KitDetailDTO>)Session["ItemKitDetail" + itemGUIDString]).Where(t => t.ItemGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty).ToList();
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
                                        objKitDetailDAL.Insert(itembr, SessionUserId, enterpriseId);
                                    }
                                }

                                Session["ItemKitDetail" + itemGUIDString] = null;
                            }
                        }

                        ///Save itemlocationwise quantity to database from the session
                        List<ItemManufacturerDetailsDTO> lstItemManufactuer = null;
                        if (Session["ItemManufacture" + itemGUIDString] != null)
                        {
                            //Wi-1505 
                            lstItemManufactuer = ((List<ItemManufacturerDetailsDTO>)Session["ItemManufacture" + itemGUIDString]).Where(t => !string.IsNullOrEmpty(t.ManufacturerName) || !string.IsNullOrEmpty(t.ManufacturerNumber)).ToList();
                            //   lstItemManufactuer = ((List<ItemManufacturerDetailsDTO>)Session["ItemManufacture"]).Where(t => !string.IsNullOrEmpty(t.ManufacturerName) || !string.IsNullOrEmpty(t.ManufacturerName)).ToList();

                            foreach (var itembr in lstItemManufactuer)
                            {
                                /// - logic for Adding supplier if Newly added...
                                if (itembr.ManufacturerID == 0)
                                {
                                    ManufacturerMasterDAL objManuMasterDAL = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName);
                                    ManufacturerMasterDTO objManufacturerMasterDTO = new ManufacturerMasterDTO();
                                    //if (!string.IsNullOrEmpty(itembr.ManufacturerName))
                                    objManufacturerMasterDTO = objManuMasterDAL.GetManufacturerByNameNormal(itembr.ManufacturerName, SessionHelper.RoomID, SessionHelper.CompanyID, false);
                                    //else
                                    //objManufacturerMasterDTO = objManuMasterDAL.GetRecord(itembr.ManufacturerName, 0, 0, false, false, false);

                                    if (objManufacturerMasterDTO == null)
                                    {
                                        objManufacturerMasterDTO = new ManufacturerMasterDTO();
                                        objManufacturerMasterDTO.ID = 0;
                                        objManufacturerMasterDTO.Manufacturer = itembr.ManufacturerName;
                                        objManufacturerMasterDTO.CompanyID = SessionHelper.CompanyID;
                                        objManufacturerMasterDTO.Room = SessionHelper.RoomID;
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
                            Session["ItemManufacture" + itemGUIDString] = null;
                        }

                        if (objDTO.ItemType != 4)
                        {
                            ///Save itemlocationwise quantity to database from the session
                            List<ItemSupplierDetailsDTO> lstItemSupplier = null;
                            if (Session["ItemSupplier" + itemGUIDString] != null)
                            {
                                lstItemSupplier = ((List<ItemSupplierDetailsDTO>)Session["ItemSupplier" + itemGUIDString]).Where(t => !string.IsNullOrEmpty(t.SupplierName) || !string.IsNullOrEmpty(t.SupplierNumber)).ToList();
                                foreach (var itembr in lstItemSupplier)
                                {
                                    itembr.ItemGUID = objDTO.GUID;

                                    if (!string.IsNullOrEmpty(itembr.SupplierName))
                                    {
                                        /// End- logic for Adding supplier if Newly added...
                                    }

                                    ItemSupplierDetailsDAL objItemSupplierDetailsDAL = new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName);
                                    if (itembr.SupplierID == 0)
                                    {
                                        if (!string.IsNullOrEmpty(itembr.SupplierName))
                                        {
                                            /// - logic for Adding supplier if Newly added...
                                            SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                                            SupplierMasterDTO objSupplierMasterDTO = objSupplierMasterDAL.GetSupplierByNamePlain(SessionHelper.RoomID, SessionHelper.CompanyID, false, itembr.SupplierName);
                                            if (objSupplierMasterDTO == null)
                                            {
                                                objSupplierMasterDTO = new SupplierMasterDTO();
                                                objSupplierMasterDTO.ID = 0;
                                                objSupplierMasterDTO.SupplierName = itembr.SupplierName;
                                                objSupplierMasterDTO.SupplierColor = get_random_color();
                                                objSupplierMasterDTO.CompanyID = SessionHelper.CompanyID;
                                                objSupplierMasterDTO.Room = SessionHelper.RoomID;
                                                objSupplierMasterDTO.RoomName = SessionHelper.RoomName;
                                                objSupplierMasterDTO.UpdatedByName = SessionHelper.UserName;
                                                objSupplierMasterDTO.CreatedByName = SessionHelper.UserName;
                                                objSupplierMasterDTO.CreatedBy = SessionHelper.UserID;
                                                objSupplierMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                                                objSupplierMasterDTO.Created = DateTimeUtility.DateTimeNow;
                                                objSupplierMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                                                objSupplierMasterDTO.AddedFrom = "Web";
                                                objSupplierMasterDTO.EditedFrom = "Web";
                                                objSupplierMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                                objSupplierMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                                objSupplierMasterDTO.ID = objSupplierMasterDAL.Insert(objSupplierMasterDTO);
                                                itembr.SupplierID = objSupplierMasterDTO.ID;
                                            }

                                            if (itembr.IsDefault == true)
                                            {
                                                itembr.SupplierID = objSupplierMasterDTO.ID;
                                            }
                                            /// End- logic for Adding supplier if Newly added...
                                        }
                                    }
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
                                Session["ItemSupplier" + itemGUIDString] = null;
                            }
                        }

                        ///Save itemlocationwise quantity to database from the session
                        List<BinMasterDTO> lstBinReplanish = null;
                        if (Session["BinReplanish" + itemGUIDString] != null)
                        {
                            lstBinReplanish = ((List<BinMasterDTO>)Session["BinReplanish" + itemGUIDString]).Where(t => !string.IsNullOrEmpty(t.BinNumber)).ToList();
                            BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);

                            double? UpdatedOnHandQuantity = 0;
                            List<long> insertedBinIds = new List<long>();
                            lstBinReplanish = objBinMasterDAL.AssignUpdateItemLocations(lstBinReplanish, objDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, false, out UpdatedOnHandQuantity, out insertedBinIds, SessionUserId);
                            if (UpdatedOnHandQuantity != null)
                                objDTO.OnHandQuantity = UpdatedOnHandQuantity;

                            if (lstBinReplanish != null && lstBinReplanish.Any(t => t.IsDefault == true))
                            {
                                objDTO.DefaultLocation = lstBinReplanish.First(t => t.IsDefault == true).ID;
                            }
                            //foreach (var itembr in lstBinReplanish)
                            //{
                            //    itembr.ItemGUID = objDTO.GUID;
                            //    itembr.IsStagingHeader = false;
                            //    itembr.IsStagingLocation = false;

                            //    BinMasterDAL objItemLocationLevelQuanityDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                            //    if (itembr.ID > 0)
                            //    {
                            //        objItemLocationLevelQuanityDAL.Edit(itembr);
                            //    }
                            //    else
                            //    {
                            //        Int64 objId = objItemLocationLevelQuanityDAL.Insert(itembr);
                            //        itembr.ID = objId;
                            //    }
                            //    if (itembr.IsDefault == true)
                            //    {
                            //        objDTO.DefaultLocation = itembr.ID;
                            //    }

                            //}

                            #region "ItemeVMI Setup"

                            //ItemLocationeVMISetupDAL objItemLocationeVMISetupDAL = new ItemLocationeVMISetupDAL(SessionHelper.EnterPriseDBName);
                            //ItemLocationeVMISetupDTO objItemLocationeVMISetupDTO = new ItemLocationeVMISetupDTO();
                            //if (lstBinReplanish.Count > 0)
                            //{
                            //    foreach (var itembr in lstBinReplanish)
                            //    {
                            //        if (itembr.IsDefault ?? false)
                            //        {
                            //            objDTO.DefaultLocation = itembr.ID;
                            //        }
                            //        objItemLocationeVMISetupDTO = objItemLocationeVMISetupDAL.GetRecordByItemGUID(itembr.ItemGUID, itembr.ID, SessionHelper.RoomID, SessionHelper.CompanyID);
                            //        if (objItemLocationeVMISetupDTO != null)
                            //        {
                            //            ItemLocationeVMISetupDTO objsetp = new ItemLocationeVMISetupDTO();
                            //            objsetp.ID = objItemLocationeVMISetupDTO.ID;
                            //            objsetp.eVMISensorID = itembr.eVMISensorID;
                            //            objsetp.eVMISensorPort = itembr.eVMISensorPort;
                            //            objsetp.BinID = itembr.ID;
                            //            objsetp.IsDeleted = false;
                            //            objsetp.IsArchived = false;
                            //            objsetp.CompanyID = itembr.CompanyID;
                            //            objsetp.Room = itembr.Room;
                            //            objsetp.ItemGUID = itembr.ItemGUID;
                            //            objItemLocationeVMISetupDAL.Edit(objsetp);
                            //        }
                            //        else
                            //        {
                            //            ItemLocationeVMISetupDTO objsetp = new ItemLocationeVMISetupDTO();
                            //            objsetp.ID = 0;
                            //            objsetp.eVMISensorID = itembr.eVMISensorID;
                            //            objsetp.eVMISensorPort = itembr.eVMISensorPort;
                            //            objsetp.BinID = itembr.ID;
                            //            objsetp.IsDeleted = false;
                            //            objsetp.IsArchived = false;
                            //            objsetp.CompanyID = itembr.CompanyID;
                            //            objsetp.Room = itembr.Room;
                            //            objsetp.ItemGUID = itembr.ItemGUID;
                            //            objItemLocationeVMISetupDAL.Insert(objsetp);
                            //        }
                            //    }
                            //}
                            #endregion

                            Session["BinReplanish" + itemGUIDString] = null;
                        }
                        objDTO.OnOrderQuantity = new CartItemDAL(SessionHelper.EnterPriseDBName).getOnOrderQty(objDTO.GUID);
                        objDTO.OnOrderInTransitQuantity = new ItemMasterDAL(SessionHelper.EnterPriseDBName).getOnOrderInTransitQty(objDTO.GUID);
                        ReturnVal = obj.Edit(objDTO, SessionUserId, SessionHelper.EnterPriceID);
                        //Auto cart entry
                        //if (objDTO.ItemType == 3)
                        //{
                        //    IEnumerable<KitDetailDTO> listKitDtl = null;
                        //    KitDetailDAL objKitDetl = new KitDetailDAL(SessionHelper.EnterPriseDBName);
                        //    listKitDtl = objKitDetl.GetCachedData(SessionHelper.RoomID, CompanyID, false, false, true);
                        //    if (listKitDtl != null && listKitDtl.Count() > 0)
                        //    {
                        //        listKitDtl = listKitDtl.Where(x => x.KitGUID == objDTO.GUID).ToList();
                        //        if (listKitDtl != null && listKitDtl.Count() > 0)
                        //        {
                        //            foreach (var item in listKitDtl)
                        //            {
                        //                new CartItemDAL(SessionHelper.EnterPriseDBName).AutoCartUpdateByCode((Guid)item.ItemGUID, objDTO.CreatedBy ?? 0, SessionHelper.EnterPriceID, objDTO.IsOnlyFromItemUI);
                        //            }
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //new CartItemDAL(SessionHelper.EnterPriseDBName).AutoCartUpdateByCode(objDTO.GUID, objDTO.LastUpdatedBy ?? 0, "Web", "InventoryControler>> ItemImSave");
                        new CartItemDAL(SessionHelper.EnterPriseDBName).AutoCartUpdateByCode(objDTO.GUID, objDTO.LastUpdatedBy ?? 0, "Web", "Inventory >> Create Item", SessionUserId);

                        //}

                        //obj.EditItemQtyCache(objDTO.ID, SessionHelper.CompanyID, SessionHelper.RoomID);
                    }
                }
                #endregion
            }
            else
            {
                #region "Edit"

                //string strOK = objCDAL.CheckItemDuplication(objDTO.ItemNumber, "edit", objDTO.ID, "ItemMaster", "ItemNumber", SessionHelper.RoomID, SessionHelper.CompanyID,objDTO.ItemType);
                string strOK = objCDAL.DuplicateCheck(objDTO.ItemNumber, "edit", objDTO.ID, "ItemMaster", "ItemNumber", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResItemMaster.ItemNumber, objDTO.ItemNumber);  //"BinNumber \"" + objDTO.BinNumber + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    double Qty = 0;
                    Qty = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName).GetItemaConsignedQty(objDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    ItemMasterDTO objTemp = new ItemMasterDTO();
                    objTemp = obj.GetItemWithoutJoins(objDTO.ID, null);
                    if (objTemp != null && objTemp.Consignment && !objDTO.Consignment && Qty > 0)
                    {
                        message = string.Format(ResMessage.ConsingedCanNotchange, ResItemMaster.Consignment, objDTO.Consignment);
                        status = "Fa";
                        return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                    }

                    if (objDTO.CreatedBy == null)
                        objDTO.CreatedBy = SessionHelper.UserID;
                    if (objDTO.LastUpdatedBy == null)
                        objDTO.LastUpdatedBy = SessionHelper.UserID;

                    if (objDTO.IsOnlyFromItemUI)
                    {
                        objDTO.EditedFrom = "Web";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    }


                    if (!String.IsNullOrEmpty(Request.Form["txtImagePath"]))
                    {
                        objDTO.ImagePath = Request.Form["txtImagePath"];
                    }
                    bool ReturnVal = obj.Edit(objDTO, SessionUserId, SessionHelper.EnterPriceID);

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
                            if (Session["ItemKitDetail" + itemGUIDString] != null)
                            {
                                lstKitDetailDTO = ((List<KitDetailDTO>)Session["ItemKitDetail" + itemGUIDString]).Where(t => t.ItemGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty).ToList();
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
                                Session["ItemKitDetail" + itemGUIDString] = null;
                            }
                        }


                        ///Save itemlocationwise quantity to database from the session
                        List<ItemManufacturerDetailsDTO> lstItemManufactuer = null;
                        if (Session["ItemManufacture" + itemGUIDString] != null)
                        {
                            lstItemManufactuer = ((List<ItemManufacturerDetailsDTO>)Session["ItemManufacture" + itemGUIDString]).Where(t => !string.IsNullOrEmpty(t.ManufacturerNumber) || !string.IsNullOrEmpty(t.ManufacturerName)).ToList();
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
                                    objManufacturerMasterDTO = objManuMasterDAL.GetManufacturerByNameNormal(itembr.ManufacturerName, SessionHelper.RoomID, SessionHelper.CompanyID, false);
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
                            Session["ItemManufacture" + itemGUIDString] = null;
                        }

                        if (objDTO.ItemType != 4)
                        {
                            ///Save itemlocationwise quantity to database from the session
                            List<ItemSupplierDetailsDTO> lstItemSupplier = null;
                            if (Session["ItemSupplier" + itemGUIDString] != null)
                            {
                                lstItemSupplier = ((List<ItemSupplierDetailsDTO>)Session["ItemSupplier" + itemGUIDString]).Where(t => !string.IsNullOrEmpty(t.SupplierName) || !string.IsNullOrEmpty(t.SupplierNumber)).ToList();

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
                                Session["ItemSupplier" + itemGUIDString] = null;
                            }
                        }

                        ///Save itemlocationwise quantity to database from the session
                        List<BinMasterDTO> lstBinReplanish = null;
                        if (Session["BinReplanish" + itemGUIDString] != null)
                        {
                            BinMasterDAL objItemLocationLevelQuanityDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);

                            lstBinReplanish = ((List<BinMasterDTO>)Session["BinReplanish" + itemGUIDString]).Where(t => !string.IsNullOrEmpty(t.BinNumber)).ToList();
                            BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);

                            double? UpdatedOnHandQuantity = 0;
                            List<long> insertedBinIds = new List<long>();
                            lstBinReplanish = objBinMasterDAL.AssignUpdateItemLocations(lstBinReplanish, objDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, false, out UpdatedOnHandQuantity, out insertedBinIds, SessionUserId);
                            if (UpdatedOnHandQuantity != null)
                                objDTO.OnHandQuantity = UpdatedOnHandQuantity;

                            if (lstBinReplanish != null && lstBinReplanish.Any(t => t.IsDefault == true))
                            {
                                objDTO.DefaultLocation = lstBinReplanish.First(t => t.IsDefault == true).ID;
                            }

                            //if (lstBinReplanish.Count > 0)
                            //{
                            //    string BinIDs = "";
                            //    foreach (var itembr in lstBinReplanish)
                            //    {
                            //        itembr.ItemGUID = objDTO.GUID;
                            //        itembr.IsStagingHeader = false;
                            //        itembr.IsStagingLocation = false;

                            //        if (itembr.ID > 0)
                            //        {
                            //            objItemLocationLevelQuanityDAL.Edit(itembr);
                            //        }
                            //        else
                            //        {
                            //            Int64 insertedId = objItemLocationLevelQuanityDAL.Insert(itembr);
                            //            itembr.ID = insertedId;
                            //        }
                            //        BinIDs += itembr.ID + ",";
                            //        if (itembr.IsDefault == true)
                            //        {
                            //            objDTO.DefaultLocation = itembr.ID;
                            //        }

                            //    }
                            //    //Delete except session record....
                            //    objItemLocationLevelQuanityDAL.DeleteRecordsExcept(BinIDs, objDTO.GUID, SessionHelper.UserID, SessionHelper.CompanyID);
                            //}

                            #region "ItemeVMI Setup"

                            //ItemLocationeVMISetupDAL objItemLocationeVMISetupDAL = new ItemLocationeVMISetupDAL(SessionHelper.EnterPriseDBName);
                            //ItemLocationeVMISetupDTO objItemLocationeVMISetupDTO = new ItemLocationeVMISetupDTO();
                            //if (lstBinReplanish.Count > 0)
                            //{
                            //    foreach (var itembr in lstBinReplanish)
                            //    {
                            //        if (itembr.IsDefault ?? false)
                            //        {
                            //            objDTO.DefaultLocation = itembr.ID;
                            //        }

                            //        objItemLocationeVMISetupDTO = objItemLocationeVMISetupDAL.GetRecordByItemGUID(itembr.ItemGUID, itembr.ID, SessionHelper.RoomID, SessionHelper.CompanyID);
                            //        if (objItemLocationeVMISetupDTO != null)
                            //        {
                            //            ItemLocationeVMISetupDTO objsetp = new ItemLocationeVMISetupDTO();
                            //            objsetp.ID = objItemLocationeVMISetupDTO.ID;
                            //            objsetp.eVMISensorID = itembr.eVMISensorID;
                            //            objsetp.eVMISensorPort = itembr.eVMISensorPort;
                            //            objsetp.BinID = itembr.ID;
                            //            objsetp.IsDeleted = false;
                            //            objsetp.IsArchived = false;
                            //            objsetp.CompanyID = itembr.CompanyID;
                            //            objsetp.Room = itembr.Room;
                            //            objsetp.ItemGUID = itembr.ItemGUID;
                            //            objItemLocationeVMISetupDAL.Edit(objsetp);
                            //        }
                            //        else
                            //        {
                            //            ItemLocationeVMISetupDTO objsetp = new ItemLocationeVMISetupDTO();
                            //            objsetp.ID = 0;
                            //            objsetp.eVMISensorID = itembr.eVMISensorID;
                            //            objsetp.eVMISensorPort = itembr.eVMISensorPort;
                            //            objsetp.BinID = itembr.ID;
                            //            objsetp.IsDeleted = false;
                            //            objsetp.IsArchived = false;
                            //            objsetp.CompanyID = itembr.CompanyID;
                            //            objsetp.Room = itembr.Room;
                            //            objsetp.ItemGUID = itembr.ItemGUID;
                            //            objItemLocationeVMISetupDAL.Insert(objsetp);
                            //        }
                            //    }
                            //}

                            #endregion

                            Session["BinReplanish" + itemGUIDString] = null;

                            //Auto cart entry
                            //if (objDTO.ItemType == 3)
                            //{
                            //    IEnumerable<KitDetailDTO> listKitDtl = null;
                            //    KitDetailDAL objKitDetl = new KitDetailDAL(SessionHelper.EnterPriseDBName);
                            //    listKitDtl = objKitDetl.GetCachedData(SessionHelper.RoomID, CompanyID, false, false, true);
                            //    if (listKitDtl != null && listKitDtl.Count() > 0)
                            //    {
                            //        listKitDtl = listKitDtl.Where(x => x.KitGUID == objDTO.GUID).ToList();
                            //        if (listKitDtl != null && listKitDtl.Count() > 0)
                            //        {
                            //            foreach (var item in listKitDtl)
                            //            {
                            //                new CartItemDAL(SessionHelper.EnterPriseDBName).AutoCartUpdateByCode((Guid)item.ItemGUID, objDTO.CreatedBy ?? 0, SessionHelper.EnterPriceID, objDTO.IsOnlyFromItemUI);
                            //            }
                            //        }
                            //    }
                            //}
                            //else
                            //{
                            //new CartItemDAL(SessionHelper.EnterPriseDBName).AutoCartUpdateByCode(objDTO.GUID, objDTO.LastUpdatedBy ?? 0, "Web", "InventoryControler>> ItemImSave");
                            new CartItemDAL(SessionHelper.EnterPriseDBName).AutoCartUpdateByCode(objDTO.GUID, objDTO.LastUpdatedBy ?? 0, "Web", "Inventory >> Create Item", SessionUserId);
                            //}

                            //new CartItemDAL(SessionHelper.EnterPriseDBName).AutoCartUpdateByCode(objDTO.GUID, objDTO.CreatedBy ?? 0, SessionHelper.EnterPriceID, objDTO.IsOnlyFromItemUI);
                            //obj.EditItemQtyCache(objDTO.ID, SessionHelper.CompanyID, SessionHelper.RoomID);
                        }

                        ReturnVal = obj.Edit(objDTO, SessionUserId, SessionHelper.EnterPriceID);
                    }

                }

                #endregion
            }

            Session["ItemMasterList"] = null;
            Session["BinReplanish" + itemGUIDString] = null;

            if (!string.IsNullOrWhiteSpace(DestinationModule) && status == "ok")
            {
                if (DestinationModule == "CartItemMaster")
                {
                    CartItemDTO objCartItemDTO = new CartItemDTO();
                    objCartItemDTO.Created = DateTimeUtility.DateTimeNow;
                    objCartItemDTO.CreatedByName = SessionHelper.UserName;
                    objCartItemDTO.CreatedBy = SessionHelper.UserID;
                    objCartItemDTO.Updated = DateTimeUtility.DateTimeNow;
                    objCartItemDTO.UpdatedByName = SessionHelper.UserName;
                    objCartItemDTO.LastUpdatedBy = SessionHelper.UserID;
                    objCartItemDTO.ID = 0;
                    objCartItemDTO.ItemNumber = objDTO.ItemNumber;
                    objCartItemDTO.ItemGUID = objDTO.GUID;
                    objCartItemDTO.UDF1 = "";
                    objCartItemDTO.UDF2 = "";
                    objCartItemDTO.UDF3 = "";
                    objCartItemDTO.UDF4 = "";
                    objCartItemDTO.UDF5 = "";
                    objCartItemDTO.ReplenishType = "";
                    objCartItemDTO.ItemGUID = objDTO.GUID;
                    objCartItemDTO.CompanyID = SessionHelper.CompanyID;
                    objCartItemDTO.Room = SessionHelper.RoomID;
                    objCartItemDTO.Quantity = SupplierCatalogQty ?? 0;
                    objCartItemDTO.IsDeleted = false;
                    objCartItemDTO.IsArchived = false;
                    objCartItemDTO.WhatWhereAction = "Inventory";
                    new CartItemDAL(SessionHelper.EnterPriseDBName).SaveCartItem(objCartItemDTO, SessionUserId,SessionHelper.EnterPriceID);
                }
                if (DestinationModule == "OrderMaster")
                {
                    if (SupplierCatalogOrderGUID != null)
                    {
                        OrderDetailsDAL objOrderDetailsDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                        OrderDetailsDTO objOrderDetailsDTO = new OrderDetailsDTO();
                        objOrderDetailsDTO.Room = SessionHelper.RoomID;
                        objOrderDetailsDTO.RoomName = SessionHelper.RoomName;
                        objOrderDetailsDTO.CreatedBy = SessionHelper.UserID;
                        objOrderDetailsDTO.CreatedByName = SessionHelper.UserName;
                        objOrderDetailsDTO.UpdatedByName = SessionHelper.UserName;
                        objOrderDetailsDTO.LastUpdatedBy = SessionHelper.UserID;
                        objOrderDetailsDTO.CompanyID = SessionHelper.CompanyID;
                        // need to disscuss this... CP & ND
                        objOrderDetailsDTO.OrderGUID = Guid.Parse(SupplierCatalogOrderGUID.ToString());
                        //objOrderDetailsDTO.ItemID = objDTO.ID;
                        objOrderDetailsDTO.ItemGUID = objDTO.GUID;
                        //objOrderDetailsDTO.ItemDetail = objDTO;
                        objOrderDetailsDTO.RequestedQuantity = SupplierCatalogQty ?? 0;
                        objOrderDetailsDTO.AddedFrom = "Web";
                        objOrderDetailsDTO.EditedFrom = "Web";
                        objOrderDetailsDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objOrderDetailsDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objOrderDetailsDAL.Insert(objOrderDetailsDTO, SessionUserId,SessionHelper.EnterPriceID);
                    }
                }
            }

            if (TempItemID > 0)
                objDTO = obj.GetItemWithMasterTableJoins(TempItemID, null, SessionHelper.RoomID, SessionHelper.CompanyID);
            return Json(new { Message = message, Status = status, DestinationModule = DestinationModule, ItemID = TempItemID, ItemDTO = objDTO }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult GetItemSuppliers(Guid ItemGUID, long? ItemID)
        {
            try
            {
                if ((ItemID ?? 0) > 0)
                {
                    return Json(new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName).GetItemSupplersByItemGuidNormal(ItemGUID));
                }
                else
                {
                    ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                    List<ItemSupplierDetailsDTO> lstItemSupplier = new List<ItemSupplierDetailsDTO>();
                    SupplierMasterDTO objSupplierMasterDTO = objItemMasterDAL.GetRoomDefaultSupplier(SessionHelper.RoomID);
                    lstItemSupplier.Add(new ItemSupplierDetailsDTO() { ID = 0, SupplierName = objSupplierMasterDTO.SupplierName, SupplierID = objSupplierMasterDTO.ID, IsDefault = true, ItemGUID = ItemGUID, Room = SessionHelper.RoomID, CompanyID = SessionHelper.CompanyID, Updated = DateTimeUtility.DateTimeNow, LastUpdatedBy = SessionHelper.UserID, Created = DateTimeUtility.DateTimeNow, CreatedBy = SessionHelper.UserID, IsDeleted = false });
                    return Json(lstItemSupplier);
                }

            }
            catch (Exception)
            {
                return Json(new List<ItemSupplierDetailsDTO>());
            }
        }

        [HttpPost]
        public JsonResult GetitemMans(Guid ItemGUID, long? ItemID)
        {
            try
            {
                if ((ItemID ?? 0) > 0)
                {
                    return Json(new ItemManufacturerDetailsDAL(SessionHelper.EnterPriseDBName).GetItemManufacturerByGUIDNormal(ItemGUID));
                }
                else
                {
                    return Json(new List<ItemManufacturerDetailsDTO>());
                }

            }
            catch (Exception)
            {
                return Json(new List<ItemManufacturerDetailsDTO>());
            }
        }
        [HttpPost]
        public JsonResult GetItemLocations(Guid ItemGUID, long? ItemID)
        {
            try
            {
                if ((ItemID ?? 0) > 0)
                {
                    return Json(new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocations(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID));
                }
                else
                {
                    ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                    List<BinMasterDTO> lstBins = new List<BinMasterDTO>();
                    BinMasterDTO objBinMasterDTO = objItemMasterDAL.GetRoomDefaultBin(SessionHelper.RoomID);
                    lstBins.Add(new BinMasterDTO() { BinNumber = objBinMasterDTO.BinNumber, IsDeleted = false });
                    return Json(lstBins);
                }

            }
            catch (Exception)
            {

                return Json(new List<BinMasterDTO>());
            }
        }

        #endregion



        private static readonly ThreadLocal<Random> appRandom = new ThreadLocal<Random>(() => new Random());

        public static List<ItemLocationSendMailDTO> lstItemLocationSendMailDTO;

        string AlterNativeRowStyle = "Style='background:#DBD9D9;'";

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

        public ActionResult ItemHistory(Guid ItemGUID)
        {
            ViewBag.ItemGuid = ItemGUID;
            return PartialView("_ItemHistory");
        }

        [HttpPost]
        public JsonResult ClearSession()
        {
            Session["ItemMasterList"] = null;
            Session["ItemMasterListForCount"] = null;
            return Json(true);
        }

        public ActionResult ItemMasterPictureView()
        {

            //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));
            string ItemDetailsNew = SiteSettingHelper.ItemDetailsNew; // Settinfile.Element("ItemDetailsNew").Value;

            if ((ItemDetailsNew ?? string.Empty) == "yes")
            {
                return View("ItemMasterListIM");
            }
            else
            {
                return View();
            }
        }

        public ActionResult ItemMasterListAllPictureView()
        {

            //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));
            string ItemDetailsNew = SiteSettingHelper.ItemDetailsNew; // Settinfile.Element("ItemDetailsNew").Value;

            if ((ItemDetailsNew ?? string.Empty) == "yes")
            {
                return View("ItemMasterListIM");
            }
            else
            {
                return View();
            }
        }
        public ActionResult ItemMastereVMIView()
        {
            string CurrentRoomFullId = eTurnsWeb.Helper.SessionHelper.EnterPriceID + "_" + eTurnsWeb.Helper.SessionHelper.CompanyID + "_" + eTurnsWeb.Helper.SessionHelper.RoomID;
            bool IseVMiSetup = true;
            if ((SiteSettingHelper.eVMIRooms ?? string.Empty).ToLower().Contains(CurrentRoomFullId.ToLower()))
            {
                IseVMiSetup = false;
            }
            if (SessionHelper.isEVMI.GetValueOrDefault(false) && IseVMiSetup)
            {
                return View();
            }
            else
            {
                return RedirectToAction("MyProfile", "Master");
            }
        }

        public ActionResult ItemMastereVMIListAjax(string SearchValue, int ItemStatus)
        {
            string CurrentRoomFullId = eTurnsWeb.Helper.SessionHelper.EnterPriceID + "_" + eTurnsWeb.Helper.SessionHelper.CompanyID + "_" + eTurnsWeb.Helper.SessionHelper.RoomID;
            bool IseVMiSetup = true;
            if ((SiteSettingHelper.eVMIRooms ?? string.Empty).ToLower().Contains(CurrentRoomFullId.ToLower()))
            {
                IseVMiSetup = false;
            }
            if (SessionHelper.isEVMI.GetValueOrDefault(false) && IseVMiSetup)
            {
                ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                var DataFromDB = obj.GeteVMIItemsPagedRecord(SessionHelper.RoomID, SessionHelper.CompanyID, SearchValue, ItemStatus);

                List<ItemMasterDTO> lstItemMaster = (from u in DataFromDB
                                                     select new ItemMasterDTO
                                                     {
                                                         ID = u.ID,
                                                         ItemNumber = u.ItemNumber,
                                                         BinNumber = u.BinNumber,
                                                         DefaultReorderQuantity = u.DefaultReorderQuantity,
                                                         OnHandQuantity = u.OnHandQuantity,
                                                         CriticalQuantity = u.CriticalQuantity,
                                                         MinimumQuantity = u.MinimumQuantity,
                                                         MaximumQuantity = u.MaximumQuantity,
                                                         WeightPerPiece = u.WeightPerPiece,
                                                         DefaultLocation = u.DefaultLocation,
                                                         GUID = u.GUID,
                                                         IsItemLevelMinMaxQtyRequired = u.IsItemLevelMinMaxQtyRequired,
                                                         BinID = u.BinID,
                                                         BinGUID = u.BinGUID,
                                                         IsActive = u.IsActive,
                                                         eVMISensorPort = u.eVMISensorPort,
                                                         eVMISensorID = u.eVMISensorID,
                                                         IsRed = u.IsRed,
                                                         IsYellow = u.IsYellow,
                                                         IsGreen = u.IsGreen
                                                     }).ToList();
                return PartialView("_eVMIItemLocationList", lstItemMaster);
            }
            else
            {
                return PartialView("_eVMIItemLocationList", null);
            }
        }

        public ActionResult ItemBinMasterView()
        {

            //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));
            string ItemDetailsNew = SiteSettingHelper.ItemDetailsNew; // Settinfile.Element("ItemDetailsNew").Value;

            if ((ItemDetailsNew ?? string.Empty) == "yes")
            {
                return View("ItemMasterListIM");
            }
            else
            {
                return View();
            }
        }

        public ActionResult ItemBinMasterByRoomAndCompanyView()
        {

            //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));
            string ItemDetailsNew = SiteSettingHelper.ItemDetailsNew; // Settinfile.Element("ItemDetailsNew").Value;

            if ((ItemDetailsNew ?? string.Empty) == "yes")
            {
                return View("ItemMasterListIM");
            }
            else
            {
                return View();
            }
        }
        public PartialViewResult CreateItem()
        {
            return PartialView();
        }


        public JsonResult GetAllRecordsByItemLocationLevelQuanity(int maxRows, string name_startsWith, string ItemGUID)
        {
            BinMasterDAL objCommon = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            List<BinMasterDTO> objBinDTO = new List<BinMasterDTO>();
            if (name_startsWith.Trim() != "")
            {
                //objBinDTO = objCommon.GetAllRecordsByItemLocationLevelQuanity(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGUID).Where(x => x.IsStagingLocation == false).Where(t => t.BinNumber.ToLower().Contains(name_startsWith.ToLower().Trim())).Take(maxRows).ToList();
                objBinDTO = objCommon.GetAllRecordsByItemLocationLevelQuanity(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGUID, false, name_startsWith.ToLower().Trim(), null, null).Take(maxRows).ToList();
            }
            else
            {
                //objBinDTO = objCommon.GetAllRecordsByItemLocationLevelQuanity(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGUID).Where(x => x.IsStagingLocation == false).Take(maxRows).ToList();
                objBinDTO = objCommon.GetAllRecordsByItemLocationLevelQuanity(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGUID, false, string.Empty, null, null).Take(maxRows).ToList();
            }

            if (objBinDTO.Count == 0)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

            return Json(objBinDTO, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetItemIsSerialOrLot(string ItemGUID)
        {
            ItemMasterDTO objItemDto = new ItemMasterDTO();
            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            Guid ItemGUID1 = Guid.Empty;
            Guid.TryParse(ItemGUID, out ItemGUID1);
            objItemDto = obj.GetItemWithoutJoins(null, ItemGUID1);
            if (objItemDto != null)
            {
                if (objItemDto.SerialNumberTracking)
                    return Json("serial", JsonRequestBehavior.AllowGet);
                else if (objItemDto.LotNumberTracking)
                    return Json("lot", JsonRequestBehavior.AllowGet);
                else
                    return Json("", JsonRequestBehavior.AllowGet);
            }
            else
                return Json("", JsonRequestBehavior.AllowGet);
        }
        public ActionResult LocationDetails(string ItemID_ItemType, string ProjectSpendGUID = "")
        {
            string ItemGUID = ItemID_ItemType.Split('#')[0];
            Int32 ItemType = Int32.Parse(ItemID_ItemType.Split('#')[1]);
            Guid? OrderDetailGUID = null;
            Guid ICDtlGuid = Guid.Empty;
            long CountBinID = 0;
            double CountQuantity = 0;
            int count = 10;
            bool isPullCredit = false;

            if (ItemID_ItemType.Contains("frompullcredit"))
            {
                string[] arrSplited = ItemID_ItemType.Split('#');
                isPullCredit = true;
                ViewBag.IsPullCredit = true;
                ViewBag.ForCreditPull = "ForCreditPull";
                ViewBag.ProjectSpendGUID = ProjectSpendGUID;
                if (ItemID_ItemType.Contains("forcount"))
                {
                    long.TryParse(arrSplited[3], out CountBinID);
                    ViewBag.CountBinID = CountBinID;
                    Guid.TryParse(arrSplited[6], out ICDtlGuid);
                    ViewBag.ICDtlGuid = CountBinID;
                    double.TryParse(arrSplited[5], out CountQuantity);
                    ViewBag.CountQuantity = CountQuantity;
                    count = Convert.ToInt32(CountQuantity);
                }
            }
            else
            {
                ViewBag.IsPullCredit = false;
                ViewBag.ForCreditPull = "";

                if (!ItemID_ItemType.Contains("FROMKITMASTER") && ItemID_ItemType.Split('#').Length > 2 && !string.IsNullOrEmpty(ItemID_ItemType.Split('#')[2]) && Guid.Parse(ItemID_ItemType.Split('#')[2]) != Guid.Empty)
                {
                    OrderDetailGUID = Guid.Parse(ItemID_ItemType.Split('#')[2]);
                    ViewBag.OrderDetailGUID = OrderDetailGUID;
                    double dblReceivedQty = 0;
                    double dblApproveQty = 0;
                    OrderDetailsDTO OrdDetailDTO = new OrderDetailsDAL(SessionHelper.EnterPriseDBName).GetOrderDetailByGuidNormal(OrderDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (OrdDetailDTO != null)
                    {
                        dblReceivedQty = OrdDetailDTO.ReceivedQuantity.GetValueOrDefault(0);
                        dblApproveQty = OrdDetailDTO.ApprovedQuantity.GetValueOrDefault(0);
                    }
                    //OrderMasterDTO ordDTO = new OrderMasterDAL(SessionHelper.EnterPriseDBName).GetOrderByGuidPlain(OrdDetailDTO.OrderGUID.GetValueOrDefault(Guid.Empty));
                    ViewBag.OrdType = OrdDetailDTO.OrderType.GetValueOrDefault(1);
                    ViewBag.ReceivedQty = dblReceivedQty;
                    ViewBag.ApprovedQty = dblApproveQty;
                }

                if (!ItemID_ItemType.Contains("FROMKITMASTER") && ItemID_ItemType.Split('#').Length > 3 && !string.IsNullOrEmpty(ItemID_ItemType.Split('#')[3]) && Int64.Parse(ItemID_ItemType.Split('#')[3]) > 0)
                {
                    ViewBag.OrderBinLocationID = int.Parse(ItemID_ItemType.Split('#')[3]);
                }
                else if (ItemID_ItemType.Contains("FROMKITMASTER") && ItemID_ItemType.Split('#').Length > 3 && !string.IsNullOrEmpty(ItemID_ItemType.Split('#')[3]) && Int64.Parse(ItemID_ItemType.Split('#')[3]) > 0)
                {
                    ViewBag.KitBinLocationID = int.Parse(ItemID_ItemType.Split('#')[3]);
                    count = int.Parse(ItemID_ItemType.Split('#')[5]);
                    ViewBag.ForCreditPull = "ForSerailKitBuild";
                }
            }

            ViewBag.ItemGUID = ItemGUID;
            //ItemMasterController objItemAPI = new ItemMasterController();
            ItemMasterDAL objItemAPI = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            Guid ItemGUID1 = Guid.Empty;
            Guid.TryParse(ItemGUID, out ItemGUID1);
            var Objitem = objItemAPI.GetItemWithoutJoins(null, ItemGUID1);
            ViewBag.ItemGUID_ItemType = ItemID_ItemType;//ItemGUID + "#" + Objitem.ItemType;
            //if (Objitem.IsItemLevelMinMaxQtyRequired == true)
            //{
            BinMasterDAL objCommon = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            //ViewBag.BinLocations = objCommon.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(x => x.IsStagingLocation == false);
            ViewBag.BinLocations = objCommon.GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Guid.Empty, 0, null, false).ToList();
            var itemDefaultBinForKit = new BinMasterDTO();

            if (ItemID_ItemType.ToLower().Contains("forcount"))
            {
                List<BinMasterDTO> lstBins = new List<BinMasterDTO>();
                BinMasterDTO CountBin = objCommon.GetBinByID(CountBinID, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (CountBin != null && CountBin.ID > 0)
                {
                    lstBins.Add(CountBin);
                }
                ViewBag.BinLocations = lstBins;
                //ViewBag.BinLocations = objCommon.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(x => x.IsStagingLocation == false && x.ID == CountBinID);
                //ViewBag.BinLocations = objCommon.GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false,Guid.Empty, CountBinID, null, false);//.Where(x => x.IsStagingLocation == false && x.ID == CountBinID);
            }
            if (ItemID_ItemType.Contains("FROMKITMASTER"))
            {
                List<BinMasterDTO> lstBins = new List<BinMasterDTO>();
                BinMasterDTO objItemBin = new BinMasterDTO();
                objItemBin = objCommon.GetItemDefaultBin(Objitem.GUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (objItemBin != null && objItemBin.ID > 0)
                {
                    lstBins.Add(objItemBin);
                    itemDefaultBinForKit = objItemBin;
                }
                ViewBag.BinLocations = lstBins;
                //ViewBag.BinLocations = objCommon.GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Guid.Empty, Objitem.DefaultLocation, null, false).Where(x => x.IsStagingLocation == false && x.ID == Objitem.DefaultLocation);
            }

            //}
            //else
            //{
            //    ItemLocationLevelQuanityDAL objILQDal = new ItemLocationLevelQuanityDAL();
            //    ViewBag.BinLocations =  objILQDal.GetAllRecordsItemWise(Objitem.GUID, SessionHelper.RoomID, SessionHelper.CompanyID);
            //}

            //ItemLocationDetailsController objAPI = new ItemLocationDetailsController();
            ItemLocationDetailsDAL objAPI = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            List<ItemLocationDetailsDTO> lstData = new List<ItemLocationDetailsDTO>();
            if (!isPullCredit && !ItemID_ItemType.Contains("FROMKITMASTER"))
            {
                lstData = objAPI.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, Guid.Parse(ItemGUID), OrderDetailGUID, "ID DESC").ToList();
            }
            //Add empty Rows to the list
            for (int i = 0; i < count; i++)
            {
                ItemLocationDetailsDTO objEmplty = new ItemLocationDetailsDTO();
                //objEmplty.ID = i;
                objEmplty.ItemGUID = Objitem.GUID;
                objEmplty.ItemNumber = Objitem.ItemNumber;
                objEmplty.Cost = Objitem.Cost;
                objEmplty.Created = DateTimeUtility.DateTimeNow;
                objEmplty.ItemGUID = Objitem.GUID;
                objEmplty.SerialNumberTracking = Objitem.SerialNumberTracking;

                if (Objitem.SerialNumberTracking)
                {
                    if (Objitem.Consignment)
                    {
                        objEmplty.ConsignedQuantity = 1;
                    }
                    else
                    {
                        objEmplty.CustomerOwnedQuantity = 1;
                    }
                }

                objEmplty.LotNumberTracking = Objitem.LotNumberTracking;
                objEmplty.OrderDetailGUID = OrderDetailGUID;
                //objEmplty.Expiration = DateTime.Now.ToString("MM-dd-yy");
                //objEmplty.Received = DateTime.Now.ToString("MM-dd-yy");

                if (isPullCredit)
                    objEmplty.IsCreditPull = true;
                else
                    objEmplty.IsCreditPull = false;

                if (ProjectSpendGUID != "")
                    objEmplty.ProjectSpentGUID = Guid.Parse(ProjectSpendGUID);
                else
                    objEmplty.ProjectSpentGUID = Guid.Empty;

                objEmplty.IsOnlyFromUI = true;

                if (ItemID_ItemType.Contains("FROMKITMASTER") && itemDefaultBinForKit != null && itemDefaultBinForKit.ID > 0)
                {
                    objEmplty.BinID = itemDefaultBinForKit.ID;
                    objEmplty.BinNumber = itemDefaultBinForKit.BinNumber;
                }
                lstData.Add(objEmplty);
            }

            //Set default SerialNumberTracking OR LotNumberTracking - for all records... assuming it is can't be edited after one location add
            lstData = lstData.Select(c => { if (Objitem.SerialNumberTracking) { c.SerialNumberTracking = true; } if (Objitem.LotNumberTracking) { c.LotNumberTracking = true; } return c; }).ToList();
            ViewBag.IsSerialNumberTracking = Objitem.SerialNumberTracking;
            ViewBag.IsLotNumberTracking = Objitem.LotNumberTracking;
            ViewBag.IsDateCodeTracking = Objitem.DateCodeTracking;
            ViewBag.ItemNumber = Objitem.ItemNumber;
            ViewBag.Consignment = Objitem.Consignment;
            ViewBag.IsItemLevelMinMaxQtyRequired = Objitem.IsItemLevelMinMaxQtyRequired;
            if (Objitem.DefaultLocation > 0)
            {
                ViewBag.DefaultLocationBag = Objitem.DefaultLocation;
            }
            return PartialView("_LocationDetails", lstData);
        }

        //public JsonResult LocationDetailsForApplyAllCount(string ItemID_ItemType, string ProjectSpendGUID)
        //{
        //    string ItemGUID = ItemID_ItemType.Split('#')[0];
        //    Int32 ItemType = Int32.Parse(ItemID_ItemType.Split('#')[1]);
        //    Guid? OrderDetailGUID = null;
        //    Guid? WorkOrderGUID = null;
        //    int count = 10;
        //    bool isPullCredit = false;
        //    long CountBinID = 0;
        //    double CountQuantity = 0;
        //    Guid ICDtlGuid = Guid.Empty;
        //    double CountConsQty = 0;
        //    double CountCustQty = 0;
        //    if (ItemID_ItemType.Contains("frompullcredit"))
        //    {
        //        string[] arrSplited = ItemID_ItemType.Split('#');
        //        isPullCredit = true;
        //        //ViewBag.IsPullCredit = true;
        //        //ViewBag.ForCreditPull = "ForCreditPull";
        //        //ViewBag.ProjectSpendGUID = ProjectSpendGUID;
        //        string[] ss = ItemID_ItemType.Split(new char[1] { '#' }, StringSplitOptions.RemoveEmptyEntries);
        //        if (ss.Length > 3)
        //        {
        //            Guid tmp = Guid.Empty;
        //            if (Guid.TryParse(ss[3], out tmp))
        //            {
        //                WorkOrderGUID = tmp;
        //            }
        //        }
        //        if (ItemID_ItemType.Contains("forcount"))
        //        {
        //            long.TryParse(arrSplited[3], out CountBinID);
        //            //ViewBag.CountBinID = CountBinID;
        //            Guid.TryParse(arrSplited[6], out ICDtlGuid);
        //            //ViewBag.ICDtlGuid = ICDtlGuid;
        //            double.TryParse(arrSplited[5], out CountQuantity);
        //            //ViewBag.CountQuantity = CountQuantity;
        //            count = Convert.ToInt32(CountQuantity);

        //            double.TryParse(arrSplited[7], out CountCustQty);
        //            double.TryParse(arrSplited[8], out CountConsQty);


        //            //ViewBag.CountTotalQty = (CountConsQty + CountCustQty);

        //        }
        //    }

        //    //ViewBag.ItemGUID = ItemGUID;



        //    //ItemMasterController objItemAPI = new ItemMasterController();
        //    ItemMasterDAL objItemAPI = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
        //    Guid ItemGUID1 = Guid.Empty;
        //    ItemMasterDTO Objitem = null;
        //    if (Guid.TryParse(ItemGUID, out ItemGUID1))
        //    {
        //        Objitem = objItemAPI.GetItemWithoutJoins(null, ItemGUID1);
        //    }


        //    BinMasterDAL objCommon = new BinMasterDAL(SessionHelper.EnterPriseDBName);
        //    List<BinMasterDTO> objBinDTO = new List<BinMasterDTO>();
        //    objBinDTO = objCommon.GetAllRecordsByItemLocationLevelQuanity(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGUID).Where(x => x.IsStagingLocation == false).ToList();
        //    //objBinDTO = objCommon.GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Guid.Parse(ItemGUID),0,null,false).ToList();//.Where(x => x.IsStagingLocation == false)
        //    //ViewBag.BinLocations = objBinDTO;


        //    if (ItemID_ItemType.ToLower().Contains("forcount"))
        //    {
        //        ViewBag.BinLocations = objCommon.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(x => x.IsStagingLocation == false && x.ID == CountBinID);
        //        //ViewBag.BinLocations = objCommon.GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Guid.Empty, CountBinID, null, false);//.Where(x => x.IsStagingLocation == false && x.ID == CountBinID);
        //    }

        //    if (ItemID_ItemType.Contains("FROMKITMASTER"))
        //    {
        //        ViewBag.BinLocations = objCommon.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(x => x.IsStagingLocation == false && x.ID == Objitem.DefaultLocation);
        //        //ViewBag.BinLocations = objCommon.GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Guid.Empty, Objitem.DefaultLocation, null, false);//.Where(x => x.IsStagingLocation == false && x.ID == Objitem.DefaultLocation);
        //    }


        //    ItemLocationDetailsDAL objAPI = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
        //    List<ItemLocationDetailsDTO> lstData = new List<ItemLocationDetailsDTO>();

        //    //for (int i = 0; i < count; i++)
        //    //{
        //    ItemLocationDetailsDTO objEmplty = new ItemLocationDetailsDTO();
        //    //objEmplty.ID = i;
        //    objEmplty.ItemGUID = Objitem.GUID;
        //    objEmplty.ItemNumber = Objitem.ItemNumber;
        //    objEmplty.Cost = Objitem.Cost;
        //    objEmplty.Created = DateTimeUtility.DateTimeNow;
        //    objEmplty.ItemGUID = Objitem.GUID;
        //    objEmplty.SerialNumberTracking = Objitem.SerialNumberTracking;

        //    //display bin number for which click on apply
        //    if ((ItemID_ItemType.Contains("forcount")))
        //    {
        //        objEmplty.ReceivedDate = DateTimeUtility.DateTimeNow;
        //        objEmplty.CountCustOrConsQty = (CountConsQty + CountCustQty);
        //        if (objBinDTO != null && objBinDTO.Count > 0 && CountBinID > 0)
        //        {
        //            BinMasterDTO objBin = new BinMasterDTO();
        //            objBin = objBinDTO.Where(t => t.ID == CountBinID).FirstOrDefault();
        //            if (objBin != null)
        //                objEmplty.BinNumber = objBin.BinNumber;
        //            else
        //                objEmplty.BinNumber = Objitem.DefaultLocationName;
        //        }
        //        if (!Objitem.SerialNumberTracking)
        //        {
        //            if (Objitem.Consignment)
        //            {
        //                objEmplty.ConsignedQuantity = CountQuantity;
        //            }
        //            else
        //            {
        //                objEmplty.CustomerOwnedQuantity = CountQuantity;
        //            }
        //        }
        //    }
        //    else
        //        objEmplty.BinNumber = Objitem.DefaultLocationName;

        //    if (Objitem.SerialNumberTracking)
        //    {
        //        if (Objitem.Consignment)
        //        {
        //            objEmplty.ConsignedQuantity = 1;
        //        }
        //        else
        //        {
        //            objEmplty.CustomerOwnedQuantity = 1;
        //        }
        //    }


        //    objEmplty.LotNumberTracking = Objitem.LotNumberTracking;
        //    objEmplty.OrderDetailGUID = OrderDetailGUID;

        //    if (isPullCredit)
        //        objEmplty.IsCreditPull = true;
        //    else
        //        objEmplty.IsCreditPull = false;

        //    if (ProjectSpendGUID != "")
        //        objEmplty.ProjectSpentGUID = Guid.Parse(ProjectSpendGUID);
        //    else
        //        objEmplty.ProjectSpentGUID = Guid.Empty;

        //    if (ICDtlGuid != Guid.Empty)
        //        objEmplty.CountLineItemDtlGUID = ICDtlGuid;
        //    else
        //        objEmplty.CountLineItemDtlGUID = Guid.Empty;

        //    objEmplty.WorkOrderGUID = WorkOrderGUID;
        //    //lstData.Add(objEmplty);

        //    return Json(objEmplty, JsonRequestBehavior.AllowGet);

        //    //}

        //    //Set default SerialNumberTracking OR LotNumberTracking - for all records... assuming it is can't be edited after one location add
        //    //lstData = lstData.Select(c => { if (Objitem.SerialNumberTracking) { c.SerialNumberTracking = true; } if (Objitem.LotNumberTracking) { c.LotNumberTracking = true; } return c; }).ToList();
        //    //ViewBag.IsSerialNumberTracking = Objitem.SerialNumberTracking;
        //    //ViewBag.IsLotNumberTracking = Objitem.LotNumberTracking;
        //    //ViewBag.IsDateCodeTracking = Objitem.DateCodeTracking;
        //    //ViewBag.ItemNumber = Objitem.ItemNumber;
        //    //ViewBag.Consignment = Objitem.Consignment;
        //    //ViewBag.IsItemLevelMinMaxQtyRequired = Objitem.IsItemLevelMinMaxQtyRequired;
        //    //if (Objitem.DefaultLocation > 0)
        //    //{
        //    //    ViewBag.DefaultLocationBag = Objitem.DefaultLocation;
        //    //}

        //    //SetSendmailList(lstData, Objitem.ItemNumber);

        //    //return PartialView("_LocationDetails", lstData);
        //}

        public ActionResult LocationDetailsNew(string ItemID_ItemType, string ProjectSpendGUID = "", double? QtyToCredit = null)
        {
            string ItemGUID = ItemID_ItemType.Split('#')[0];
            Int32 ItemType = Int32.Parse(ItemID_ItemType.Split('#')[1]);
            Guid? OrderDetailGUID = null;
            Guid? WorkOrderGUID = null;
            int count = 10;
            bool isPullCredit = false;
            long CountBinID = 0;
            double CountQuantity = 0;
            Guid ICDtlGuid = Guid.Empty;

            //WI-1039 related changes
            double CountedConsQty = 0;
            double CountedCustQty = 0;

            double ActualConsQty = 0;
            double ActualCustoQty = 0;

            double DiffConsQty = 0;
            double DiffCustQty = 0;

            ViewBag.PullCreditType = "credit";
            if (ItemID_ItemType.Contains("frompullcredit"))
            {
                string[] arrSplited = ItemID_ItemType.Split('#');
                if (arrSplited.Length > 13)
                    ViewBag.PullCreditType = arrSplited[13];
                isPullCredit = true;
                ViewBag.IsPullCredit = true;
                ViewBag.ForCreditPull = "ForCreditPull";
                ViewBag.ProjectSpendGUID = ProjectSpendGUID;
                string[] ss = ItemID_ItemType.Split(new char[1] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                if (ss.Length > 3)
                {
                    Guid tmp = Guid.Empty;
                    if (Guid.TryParse(ss[3], out tmp))
                    {
                        WorkOrderGUID = tmp;
                    }
                }
                if (ItemID_ItemType.Contains("forcount"))
                {
                    long.TryParse(arrSplited[3], out CountBinID);
                    ViewBag.CountBinID = CountBinID;

                    Guid.TryParse(arrSplited[6], out ICDtlGuid);
                    ViewBag.ICDtlGuid = ICDtlGuid;

                    double.TryParse(arrSplited[5], out CountQuantity);
                    ViewBag.CountQuantity = CountQuantity;
                    count = Convert.ToInt32(CountQuantity);

                    //double tempVar1 = 0;
                    //double TempVar2 = 0;


                    double.TryParse(arrSplited[7], out DiffCustQty);
                    double.TryParse(arrSplited[8], out DiffConsQty);
                    double.TryParse(arrSplited[9], out CountedConsQty);
                    double.TryParse(arrSplited[10], out CountedCustQty);
                    double.TryParse(arrSplited[11], out ActualConsQty);
                    double.TryParse(arrSplited[12], out ActualCustoQty);



                    //Display Qty Value if >0 
                    if (DiffConsQty > 0)
                    {
                        ViewBag.CountedConsQty = CountedConsQty;
                        ViewBag.ActualConsQty = ActualConsQty;
                        ViewBag.DiffConsQty = DiffConsQty;
                    }
                    else
                    {
                        ViewBag.CountedConsQty = 0;
                        ViewBag.ActualConsQty = 0;
                        ViewBag.DiffConsQty = 0;
                    }
                    if (DiffCustQty > 0)
                    {
                        ViewBag.CountedCustQty = CountedCustQty;
                        ViewBag.ActualCustoQty = ActualCustoQty;
                        ViewBag.DiffCustQty = DiffCustQty;
                    }
                    else
                    {
                        ViewBag.CountedCustQty = 0;
                        ViewBag.ActualCustoQty = 0;
                        ViewBag.DiffCustQty = 0;
                    }
                }
            }
            else
            {
                ViewBag.IsPullCredit = false;
                ViewBag.ForCreditPull = "";

                if (!ItemID_ItemType.Contains("FROMKITMASTER") && ItemID_ItemType.Split('#').Length > 2 && !string.IsNullOrEmpty(ItemID_ItemType.Split('#')[2]) && Guid.Parse(ItemID_ItemType.Split('#')[2]) != Guid.Empty)
                {
                    OrderDetailGUID = Guid.Parse(ItemID_ItemType.Split('#')[2]);
                    ViewBag.OrderDetailGUID = OrderDetailGUID;
                }

                if (!ItemID_ItemType.Contains("FROMKITMASTER") && ItemID_ItemType.Split('#').Length > 3 && !string.IsNullOrEmpty(ItemID_ItemType.Split('#')[3]) && Int64.Parse(ItemID_ItemType.Split('#')[3]) > 0)
                {
                    ViewBag.OrderBinLocationID = int.Parse(ItemID_ItemType.Split('#')[3]);
                }
                else if (ItemID_ItemType.Contains("FROMKITMASTER") && ItemID_ItemType.Split('#').Length > 3 && !string.IsNullOrEmpty(ItemID_ItemType.Split('#')[3]) && Int64.Parse(ItemID_ItemType.Split('#')[3]) > 0)
                {
                    ViewBag.KitBinLocationID = int.Parse(ItemID_ItemType.Split('#')[3]);
                    count = int.Parse(ItemID_ItemType.Split('#')[5]);
                    ViewBag.ForCreditPull = "ForSerailKitBuild";
                }

            }

            ViewBag.ItemGUID = ItemGUID;



            //ItemMasterController objItemAPI = new ItemMasterController();
            ItemMasterDAL objItemAPI = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            Guid ItemGUID1 = Guid.Empty;
            ItemMasterDTO Objitem = null;
            if (Guid.TryParse(ItemGUID, out ItemGUID1))
            {
                Objitem = objItemAPI.GetItemWithoutJoins(null, ItemGUID1);
            }


            ViewBag.ItemGUID_ItemType = ItemID_ItemType;//ItemGUID + "#" + Objitem.ItemType;
            double QtyForSerialCust = 0;
            double QtyForSerialCon = 0;
            bool IsBothQtyForSerial = false;
            if (ItemID_ItemType.Contains("frompullcredit"))
            {
                if (!Objitem.LotNumberTracking && !Objitem.DateCodeTracking && !Objitem.SerialNumberTracking)
                    count = 1;
                else if (Objitem.LotNumberTracking || Objitem.DateCodeTracking) // Lot Number Or DateCodeTracking Type Item
                {
                    count = 1;
                }

                //general item
                if (ItemID_ItemType.ToLower().Contains("forcount"))
                {
                    if (Objitem.SerialNumberTracking && ((DiffCustQty > 0 && DiffConsQty > 0)))
                    {
                        QtyForSerialCust = DiffCustQty;
                        QtyForSerialCon = DiffConsQty;
                        IsBothQtyForSerial = true;
                    }
                    //    count = 1;
                }
                else
                {
                    if (Objitem.SerialNumberTracking && (QtyToCredit.HasValue && QtyToCredit.Value > 0))
                        count = Convert.ToInt32(QtyToCredit.Value);
                    else if (Objitem.SerialNumberTracking && (QtyToCredit.HasValue && QtyToCredit.Value == 0))
                        count = 1;
                }
            }

            //if (Objitem.IsItemLevelMinMaxQtyRequired == true)
            //{
            BinMasterDAL objCommon = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            List<BinMasterDTO> objBinDTO = new List<BinMasterDTO>();
            //objBinDTO = objCommon.GetAllRecordsByItemLocationLevelQuanity(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGUID).Where(x => x.IsStagingLocation == false).ToList();
            objBinDTO = objCommon.GetAllRecordsByItemLocationLevelQuanity(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGUID, false, string.Empty, null, null).ToList();
            //objBinDTO = objCommon.GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false,Guid.Parse(ItemGUID),0,null,false).ToList();//.Where(x => x.IsStagingLocation == false)
            ViewBag.BinLocations = objBinDTO;
            if (ItemID_ItemType.ToLower().Contains("forcount"))
            {
                List<BinMasterDTO> lstBins = new List<BinMasterDTO>();
                BinMasterDTO CountBin = objCommon.GetBinByID(CountBinID, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (CountBin != null && CountBin.ID > 0)
                {
                    lstBins.Add(CountBin);
                }
                ViewBag.BinLocations = lstBins;
                //ViewBag.BinLocations = objCommon.GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Guid.Empty, CountBinID, null, false);//.Where(x => x.IsStagingLocation == false && x.ID == CountBinID);
            }

            if (ItemID_ItemType.Contains("FROMKITMASTER"))
            {
                List<BinMasterDTO> lstBins = new List<BinMasterDTO>();
                //ViewBag.BinLocations = objCommon.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(x => x.IsStagingLocation == false && x.ID == Objitem.DefaultLocation);
                //ViewBag.BinLocations = objCommon.GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Guid.Empty, Objitem.DefaultLocation, null, false);//.Where(x => x.IsStagingLocation == false && x.ID == Objitem.DefaultLocation);
                if ((Objitem.DefaultLocation ?? 0) > 0)
                {
                    lstBins = new List<BinMasterDTO>();
                    BinMasterDTO objdefaultBin = objCommon.GetItemDefaultBin(Objitem.GUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (objdefaultBin != null && objdefaultBin.ID > 0)
                    {
                        lstBins.Add(objdefaultBin);
                    }

                    ViewBag.BinLocations = lstBins;
                }

            }


            ItemLocationDetailsDAL objAPI = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            List<ItemLocationDetailsDTO> lstData = new List<ItemLocationDetailsDTO>();


            //Add empty Rows to the list
            for (int i = 0; i < count; i++)
            {
                ItemLocationDetailsDTO objEmplty = new ItemLocationDetailsDTO();
                //objEmplty.ID = i;
                objEmplty.ItemGUID = Objitem.GUID;
                objEmplty.ItemNumber = Objitem.ItemNumber;
                objEmplty.Cost = Objitem.Cost;
                objEmplty.Created = DateTimeUtility.DateTimeNow;
                objEmplty.ItemGUID = Objitem.GUID;
                objEmplty.SerialNumberTracking = Objitem.SerialNumberTracking;

                //display bin number for which click on apply
                if ((ItemID_ItemType.Contains("forcount")))
                {
                    objEmplty.ReceivedDate = DateTimeUtility.DateTimeNow;
                    if (objBinDTO != null && objBinDTO.Count > 0 && CountBinID > 0)
                    {
                        BinMasterDTO objBin = new BinMasterDTO();
                        objBin = objBinDTO.Where(t => t.ID == CountBinID).FirstOrDefault();
                        if (objBin != null)
                            objEmplty.BinNumber = objBin.BinNumber;
                        else
                            objEmplty.BinNumber = Objitem.DefaultLocationName;
                    }

                    if (Objitem.SerialNumberTracking)
                    {
                        if (IsBothQtyForSerial)
                        {
                            if (QtyForSerialCon > 0)
                            {
                                objEmplty.ConsignedQuantity = 1;
                                QtyForSerialCon = QtyForSerialCon - 1;
                            }
                            else if (QtyForSerialCust > 0)
                            {
                                objEmplty.CustomerOwnedQuantity = 1;
                                QtyForSerialCust = QtyForSerialCust - 1;
                            }
                        }
                        else
                        {
                            if (Objitem.Consignment)
                                objEmplty.ConsignedQuantity = 1;
                            else
                                objEmplty.CustomerOwnedQuantity = 1;
                        }
                    }
                    else
                    {

                        //if (Objitem.Consignment)
                        //{
                        //    objEmplty.ConsignedQuantity = DiffConsQty;
                        //    objEmplty.CountCustOrConsQty = DiffConsQty;
                        //}
                        //else
                        //{
                        //    objEmplty.CountCustOrConsQty = DiffCustQty;
                        //    objEmplty.CustomerOwnedQuantity = DiffCustQty;
                        //}

                        //objEmplty.CountCustOrConsQty = (CountConsQty + CountCustQty);
                        if (ItemID_ItemType.Contains("forcount"))
                        {
                            objEmplty.ConsignedQuantity = DiffConsQty;
                            objEmplty.CustomerOwnedQuantity = DiffCustQty;
                        }
                        else
                        {
                            if (Objitem.LotNumberTracking || Objitem.DateCodeTracking)
                            {
                                if (i == 0)
                                {

                                    if (Objitem.Consignment)
                                    {
                                        objEmplty.ConsignedQuantity = DiffConsQty;
                                    }
                                    else
                                    {
                                        objEmplty.CustomerOwnedQuantity = DiffCustQty;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    objEmplty.BinNumber = Objitem.DefaultLocationName;
                    objEmplty.ReceivedDate = DateTimeUtility.DateTimeNow;
                    if (!Objitem.SerialNumberTracking || !Objitem.LotNumberTracking || !Objitem.DateCodeTracking)
                    {
                        if (i == 0)
                        {
                            if (!ItemID_ItemType.Contains("forcount"))
                            {

                                if (Objitem.Consignment)
                                {
                                    objEmplty.ConsignedQuantity = DiffConsQty;
                                }
                                else
                                {
                                    objEmplty.CustomerOwnedQuantity = DiffCustQty;
                                }
                            }
                        }
                    }
                }

                if (Objitem.SerialNumberTracking)
                {
                    if (!ItemID_ItemType.Contains("forcount"))
                    //{
                    //    if (DiffConsQty > 0)
                    //        objEmplty.ConsignedQuantity = 1;
                    //    else
                    //        objEmplty.ConsignedQuantity = 0;
                    //    if (DiffCustQty > 0)
                    //        objEmplty.CustomerOwnedQuantity = 1;
                    //    else
                    //        objEmplty.CustomerOwnedQuantity = 0;
                    //}
                    //else
                    {
                        if (Objitem.Consignment)
                        {
                            objEmplty.ConsignedQuantity = 1;
                        }
                        else
                        {
                            objEmplty.CustomerOwnedQuantity = 1;
                        }
                    }
                }


                objEmplty.LotNumberTracking = Objitem.LotNumberTracking;
                objEmplty.OrderDetailGUID = OrderDetailGUID;
                //objEmplty.Expiration = DateTime.Now.ToString("MM-dd-yy");
                //objEmplty.Received = DateTime.Now.ToString("MM-dd-yy");

                if (isPullCredit)
                    objEmplty.IsCreditPull = true;
                else
                    objEmplty.IsCreditPull = false;

                if (ProjectSpendGUID != "")
                    objEmplty.ProjectSpentGUID = Guid.Parse(ProjectSpendGUID);
                else
                    objEmplty.ProjectSpentGUID = Guid.Empty;

                if (ICDtlGuid != Guid.Empty)
                    objEmplty.CountLineItemDtlGUID = ICDtlGuid;
                else
                    objEmplty.CountLineItemDtlGUID = Guid.Empty;

                objEmplty.WorkOrderGUID = WorkOrderGUID;
                objEmplty.IsOnlyFromUI = true;
                lstData.Add(objEmplty);
            }

            //Set default SerialNumberTracking OR LotNumberTracking - for all records... assuming it is can't be edited after one location add
            lstData = lstData.Select(c => { if (Objitem.SerialNumberTracking) { c.SerialNumberTracking = true; } if (Objitem.LotNumberTracking) { c.LotNumberTracking = true; } return c; }).ToList();
            ViewBag.IsSerialNumberTracking = Objitem.SerialNumberTracking;
            ViewBag.IsLotNumberTracking = Objitem.LotNumberTracking;
            ViewBag.IsDateCodeTracking = Objitem.DateCodeTracking;
            ViewBag.ItemNumber = Objitem.ItemNumber;
            ViewBag.Consignment = Objitem.Consignment;
            ViewBag.IsItemLevelMinMaxQtyRequired = Objitem.IsItemLevelMinMaxQtyRequired;
            if (Objitem.DefaultLocation > 0)
            {
                ViewBag.DefaultLocationBag = Objitem.DefaultLocation;
            }

            SetSendmailList(lstData, Objitem.ItemNumber);

            return PartialView("_LocationDetails", lstData);
        }

        public void SetSendmailList(List<ItemLocationDetailsDTO> lstData, string ItemNumber)
        {

            if (lstData != null && lstData.Count > 0)
            {
                int SrNo = 0;
                lstItemLocationSendMailDTO = new List<ItemLocationSendMailDTO>();
                foreach (ItemLocationDetailsDTO objlistDTO in lstData)
                {
                    ItemLocationSendMailDTO objItemLocationSendMailDTO = new ItemLocationSendMailDTO();
                    objItemLocationSendMailDTO.SrNo = SrNo;
                    objItemLocationSendMailDTO.BinNumber = objlistDTO.BinNumber;
                    objItemLocationSendMailDTO.CustomerOwnedQuantity = objlistDTO.CustomerOwnedQuantity;
                    objItemLocationSendMailDTO.ConsignedQuantity = objlistDTO.ConsignedQuantity;
                    objItemLocationSendMailDTO.Cost = objlistDTO.Cost;
                    objItemLocationSendMailDTO.ReceivedDate = objlistDTO.ReceivedDate;
                    objItemLocationSendMailDTO.ItemNumber = ItemNumber;
                    lstItemLocationSendMailDTO.Add(objItemLocationSendMailDTO);
                    SrNo += 1;
                }
            }
            else
            {
                lstItemLocationSendMailDTO = null;
            }
        }

        public void CheckValueSendMail(List<ItemLocationDetailsDTO> lstData)
        {
            bool IsChange = false;
            StringBuilder sb = new StringBuilder();
            string htmlTabl = string.Empty;
            if (lstData != null && lstData.Count > 0 && lstItemLocationSendMailDTO != null && lstItemLocationSendMailDTO.Count > 0)
            {

                htmlTabl = @"<table style=""margin-left: 0px; width: 99%;""  border=""1"" cellpadding=""0""
                            cellspacing=""0"">
                            <thead>
                                <tr role=""row"">
                                    <th  style=""width: 10%; text-align: left;"">
                                       Item Number
                                    </th>
                                    <th  style=""width: 5%; text-align: left;"">
                                       Item Location
                                    </th>
                                    <th  style=""width: 15%; text-align: left;"">
                                       Old On Hand Quantity
                                    </th>
                                    <th  style=""width: 10%; text-align: left;"">
                                        New On Hand Quantity
                                    </th>                                    
                                </tr>
                            </thead>
                            <tbody>
                            ##TRS##
                            </tbody>
                        </table>";
                int cntrow = 1;
                string trs = "";
                int SrNo = 0;
                foreach (ItemLocationDetailsDTO objlistDTO in lstData)
                {
                    ItemLocationSendMailDTO objItemLocationSendMailDTO = lstItemLocationSendMailDTO[SrNo];

                    bool IsCheck = false;
                    if (objItemLocationSendMailDTO.CustomerOwnedQuantity != objlistDTO.CustomerOwnedQuantity)
                    {
                        IsChange = true;
                        IsCheck = true;
                    }
                    if (objItemLocationSendMailDTO.ConsignedQuantity != objlistDTO.ConsignedQuantity)
                    {
                        IsChange = true;
                        IsCheck = true;
                    }

                    if (IsCheck)
                    {
                        string RowStyle = string.Empty;
                        if (cntrow % 2 == 0)
                        {
                            RowStyle = AlterNativeRowStyle;
                        }
                        else
                        {

                        }
                        trs += @"<tr " + RowStyle + @" >
                        <td>
                            " + objItemLocationSendMailDTO.ItemNumber + @"
                        </td>
                        <td>
                            " + objlistDTO.BinNumber + @"
                        </td>
                        <td>
                            " + Convert.ToDouble(Convert.ToDouble(objItemLocationSendMailDTO.CustomerOwnedQuantity) + Convert.ToDouble(objItemLocationSendMailDTO.ConsignedQuantity)) + @"
                        </td>
                        <td>
                            " + Convert.ToDouble(Convert.ToDouble(objlistDTO.CustomerOwnedQuantity) + Convert.ToDouble(objlistDTO.ConsignedQuantity)) + @"
                        </td>
                       
                    </tr>";

                        cntrow += 1;
                    }
                    SrNo += 1;
                }
                htmlTabl = htmlTabl.Replace("##TRS##", trs);
            }
            if (IsChange)
            {
                SendMailForItemQuantityAdj(htmlTabl);
            }
        }

        private void SendMailForItemQuantityAdj(string HtmlTable)
        {
            StringBuilder MessageBody = null;
            EmailTemplateDAL objEmailTemplateDAL = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
            eTurnsUtility objUtils = null;
            eMailMasterDAL objEmailDAL = null;
            try
            {
                string StrSubject = ResItemMaster.ItemQtyAdjustment;

                List<UserMasterDTO> objUserMasterDTO = new List<UserMasterDTO>();
                string strToAddress = CommonUtility.GetEmailToAddress(objUserMasterDTO, "ItemQtyAdjust");

                if (!string.IsNullOrEmpty(strToAddress))
                {
                    string strCCAddress = "";
                    MessageBody = new StringBuilder();

                    objEmailTemplateDAL = new EmailTemplateDAL(SessionHelper.EnterPriseDBName);
                    objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();
                    objEmailTemplateDetailDTO = objEmailTemplateDAL.GetEmailTemplate("ItemQtyAdjust", ResourceHelper.CurrentCult.ToString(), SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (objEmailTemplateDetailDTO != null)
                    {
                        MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
                        StrSubject = objEmailTemplateDetailDTO.MailSubject;
                    }
                    else
                    {
                        return;
                    }

                    MessageBody.Replace("@@TABLE@@", HtmlTable);
                    MessageBody.Replace("@@CountORMan@@", SessionHelper.UserName);
                    MessageBody.Replace("@@ROOMNAME@@", SessionHelper.RoomName);
                    MessageBody.Replace("@@USERNAME@@", SessionHelper.UserName);
                    MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);
                    objUtils = new eTurnsUtility();
                    objUtils.SendMail(strToAddress, strCCAddress, StrSubject, MessageBody.ToString());
                    objEmailDAL = new eMailMasterDAL(SessionHelper.EnterPriseDBName);
                    RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
                    eTurnsRegionInfo objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(SessionHelper.RoomID, SessionHelper.CompanyID, -1);
                    string DateTimeFormat = "MM/dd/yyyy";
                    DateTime TZDateTimeNow = DateTime.UtcNow;
                    if (objeTurnsRegionInfo != null)
                    {
                        DateTimeFormat = objeTurnsRegionInfo.ShortDatePattern;// + " " + objeTurnsRegionInfo.ShortTimePattern;
                        TZDateTimeNow = objeTurnsRegionInfo.TZDateTimeNow ?? DateTime.UtcNow;
                    }
                    if (StrSubject != null)
                    {
                        string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                        // EmailSubject = EmailSubject.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                        StrSubject = Regex.Replace(StrSubject, "@@DATE@@", CurrentDate, RegexOptions.IgnoreCase);
                        if (!string.IsNullOrWhiteSpace(SessionHelper.CompanyName))
                        {
                            StrSubject = Regex.Replace(StrSubject, "@@COMPANYNAME@@", SessionHelper.CompanyName, RegexOptions.IgnoreCase);
                        }
                        if (!string.IsNullOrWhiteSpace(SessionHelper.RoomName))
                        {
                            StrSubject = Regex.Replace(StrSubject, "@@ROOMNAME@@", SessionHelper.RoomName, RegexOptions.IgnoreCase);
                        }
                        StrSubject = Regex.Replace(StrSubject, "@@Year@@", Convert.ToString(DateTime.UtcNow.Year), RegexOptions.IgnoreCase);
                    }

                    if (MessageBody != null)
                    {
                        string CurrentDate = TZDateTimeNow.ToString(DateTimeFormat);
                        //EmailBody = EmailBody.Replace("@@DATE@@", CurrentDate).Replace("@@date@@", CurrentDate).Replace("@@Date@@", CurrentDate);
                        MessageBody = MessageBody.Replace("@@DATE@@", CurrentDate);
                        if (!string.IsNullOrWhiteSpace(SessionHelper.CompanyName))
                        {
                            MessageBody = MessageBody.Replace("@@COMPANYNAME@@", SessionHelper.CompanyName);
                        }
                        if (!string.IsNullOrWhiteSpace(SessionHelper.RoomName))
                        {
                            MessageBody = MessageBody.Replace("@@ROOMNAME@@", SessionHelper.RoomName);
                        }
                        MessageBody = MessageBody.Replace("@@Year@@", Convert.ToString(DateTime.UtcNow.Year));

                    }
                    objEmailDAL.eMailToSend(strToAddress, strCCAddress, StrSubject, MessageBody.ToString(), SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, null, "Web => Inventory => SendMailForItemQuantityAdj");

                }
            }
            catch (Exception)
            {
                // throw ex;
            }
            finally
            {
                MessageBody = null;
                objEmailTemplateDAL = null;
                objEmailTemplateDetailDTO = null;
                objUtils = null;
                objEmailDAL = null;
            }
        }

        public ActionResult LocationDetailsEdit(Guid LocationGUID)
        {
            ItemLocationDetailsDAL objItemLocationDetailsDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            ItemLocationDetailsDTO objEmplty = new ItemLocationDetailsDTO();
            objEmplty = objItemLocationDetailsDAL.GetRecord(LocationGUID, SessionHelper.RoomID, SessionHelper.CompanyID);

            Guid ItemGUID = objEmplty.ItemGUID.GetValueOrDefault(Guid.Empty);
            ViewBag.ItemGUID = ItemGUID;
            ItemMasterDAL objItemAPI = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            var Objitem = objItemAPI.GetItemWithoutJoins(null, ItemGUID);

            BinMasterDAL objCommon = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            //ViewBag.BinLocations = objCommon.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(x => x.IsStagingLocation == false);
            ViewBag.BinLocations = objCommon.GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Guid.Empty, 0, null, false);//.Where(x => x.IsStagingLocation == false);

            //ItemLocationDetailsController objAPI = new ItemLocationDetailsController();
            ItemLocationDetailsDAL objAPI = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            List<ItemLocationDetailsDTO> lstData = new List<ItemLocationDetailsDTO>();
            //Add empty Rows to the list

            //objEmplty.ID = i;
            objEmplty.ItemGUID = Objitem.GUID;
            objEmplty.ItemNumber = Objitem.ItemNumber;
            // objEmplty.Cost = Objitem.Cost;
            objEmplty.Created = DateTimeUtility.DateTimeNow;
            objEmplty.ItemGUID = Objitem.GUID;
            objEmplty.SerialNumberTracking = Objitem.SerialNumberTracking;

            if (Objitem.SerialNumberTracking)
            {
                if (Objitem.Consignment)
                {
                    objEmplty.ConsignedQuantity = 1;
                }
                else
                {
                    objEmplty.CustomerOwnedQuantity = 1;
                }
            }

            objEmplty.LotNumberTracking = Objitem.LotNumberTracking;
            //objEmplty.Expiration = DateTime.Now.ToString("MM-dd-yy");
            //objEmplty.Received = DateTime.Now.ToString("MM-dd-yy");
            objEmplty.IsCreditPull = false;
            lstData.Add(objEmplty);

            //Set default SerialNumberTracking OR LotNumberTracking - for all records... assuming it is can't be edited after one location add
            lstData = lstData.Select(c => { if (Objitem.SerialNumberTracking) { c.SerialNumberTracking = true; } if (Objitem.LotNumberTracking) { c.LotNumberTracking = true; } return c; }).ToList();
            ViewBag.IsSerialNumberTracking = Objitem.SerialNumberTracking;
            ViewBag.IsLotNumberTracking = Objitem.LotNumberTracking;
            ViewBag.IsDateCodeTracking = Objitem.DateCodeTracking;
            ViewBag.ItemNumber = Objitem.ItemNumber;
            ViewBag.Consignment = Objitem.Consignment;
            ViewBag.IsItemLevelMinMaxQtyRequired = Objitem.IsItemLevelMinMaxQtyRequired;
            if (Objitem.DefaultLocation > 0)
            {
                ViewBag.DefaultLocationBag = Objitem.DefaultLocation;
            }
            SetSendmailList(lstData, Objitem.ItemNumber);
            return PartialView("_LocationDetails", lstData);
        }

        public ActionResult LocationDetailsMS(string ItemID_ItemType)
        {
            string ItemGUID = ItemID_ItemType.Split('#')[0];
            Int32 ItemType = Int32.Parse(ItemID_ItemType.Split('#')[1]);
            Guid? OrderDetailGUID = null;
            Int64? BinID = null;
            Guid? WorkOrderGuid = null;
            string stageHeaderName = string.Empty;
            string selectStagingBinName = string.Empty;

            int count = 10;

            ViewBag.PullCreditType = "MS Credit";
            string[] arrSplited = ItemID_ItemType.Split('#');
            if (arrSplited.Length > 2 && (arrSplited[2] == "MS Credit" || arrSplited[2] == "AdMSCredit"))
                ViewBag.PullCreditType = arrSplited[2];


            if (ItemID_ItemType.Split('#').Length > 3 && !string.IsNullOrEmpty(ItemID_ItemType.Split('#')[3]) && Int64.Parse(ItemID_ItemType.Split('#')[3]) > 0)
            {
                BinID = Int64.Parse(ItemID_ItemType.Split('#')[3]);
                ViewBag.OrderBinLocationID = BinID;
            }

            if (ItemID_ItemType.Contains("WorkOrderGuid-"))
            {
                string[] ss = ItemID_ItemType.Split(new char[1] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in ss)
                {
                    if (item.Contains("WorkOrderGuid-"))
                    {
                        Guid tempwoid = Guid.Empty;
                        if (Guid.TryParse(item.Replace("WorkOrderGuid-", ""), out tempwoid))
                        {
                            WorkOrderGuid = tempwoid;
                        }

                        //WorkOrderGuid = Guid.Parse(item.Replace("WorkOrderGuid-", ""));
                    }
                }
            }
            else if (ItemID_ItemType.Split('#').Length > 2 && !string.IsNullOrEmpty(ItemID_ItemType.Split('#')[2]) && Guid.Parse(ItemID_ItemType.Split('#')[2]) != Guid.Empty)
            {
                OrderDetailGUID = Guid.Parse(ItemID_ItemType.Split('#')[2]);
                ViewBag.OrderDetailGUID = OrderDetailGUID;
                OrderDetailsDTO OrdDetailDTO = new OrderDetailsDAL(SessionHelper.EnterPriseDBName).GetOrderDetailByGuidFull(OrderDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);

                if (BinID.GetValueOrDefault(0) <= 0)
                {
                    BinID = OrdDetailDTO.Bin;
                }

                double dblReceivedQty = 0;
                double dblApproveQty = 0;

                dblReceivedQty = OrdDetailDTO.ReceivedQuantity.GetValueOrDefault(0);
                dblApproveQty = OrdDetailDTO.ApprovedQuantity.GetValueOrDefault(0);

                OrderMasterDTO ordDTO = new OrderMasterDAL(SessionHelper.EnterPriseDBName).GetOrderByGuidNormal(OrdDetailDTO.OrderGUID.GetValueOrDefault(Guid.Empty));
                ViewBag.OrdType = OrdDetailDTO.OrderType.GetValueOrDefault(1);
                ViewBag.ReceivedQty = dblReceivedQty;
                ViewBag.ApprovedQty = dblApproveQty;
                stageHeaderName = ordDTO.StagingName;
                selectStagingBinName = OrdDetailDTO.BinName;

            }
            ViewBag.MSName = stageHeaderName;

            ViewBag.ItemGUID = ItemGUID;
            ItemMasterDAL objItemAPI = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            Guid ItemGUID1 = Guid.Empty;
            Guid.TryParse(ItemGUID, out ItemGUID1);
            var Objitem = objItemAPI.GetItemWithoutJoins(null, ItemGUID1);
            if (ItemID_ItemType.Contains("WorkOrderGuid-"))
            {
                if (!Objitem.SerialNumberTracking)
                {
                    count = 1;
                }
            }
            ViewBag.ItemGUID_ItemType = ItemID_ItemType;//ItemGUID + "#" + Objitem.ItemType;
            BinMasterDAL objCommon = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            //IEnumerable<BinMasterDTO> lstBinMaster = objCommon.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(x => x.IsStagingLocation == true);
            IEnumerable<BinMasterDTO> lstBinMaster = objCommon.GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Guid.Empty, 0, null, true);//.Where(x => x.IsStagingLocation == true);
            if (BinID > 0)
            {
                lstBinMaster = lstBinMaster.Where(x => x.ID == BinID.GetValueOrDefault(0));
            }

            ViewBag.BinLocations = lstBinMaster;
            MaterialStagingPullDetailDAL objAPI = new MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName);
            List<MaterialStagingPullDetailDTO> lstData = new List<MaterialStagingPullDetailDTO>();
            if (OrderDetailGUID != null && OrderDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
            {
                //lstData = new MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName).GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => (x.IsDeleted ?? false) == false && (x.IsArchived ?? false) == false && x.ItemGUID == Guid.Parse(ItemGUID) && x.OrderDetailGUID.GetValueOrDefault(Guid.Empty) == OrderDetailGUID.GetValueOrDefault(Guid.Empty)).ToList();
                lstData = new MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName).GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, string.Empty, 0, Convert.ToString(ItemGUID), false, false, Convert.ToString(OrderDetailGUID.GetValueOrDefault(Guid.Empty))).ToList();
            }
            //Add empty Rows to the list
            for (int i = 0; i < count; i++)
            {
                MaterialStagingPullDetailDTO objEmplty = new MaterialStagingPullDetailDTO();
                //objEmplty.ID = i;
                objEmplty.ItemGUID = Objitem.GUID;
                objEmplty.ItemNumber = Objitem.ItemNumber;
                objEmplty.ItemCost = Objitem.Cost;
                objEmplty.Created = DateTimeUtility.DateTimeNow;
                objEmplty.ItemGUID = Objitem.GUID;
                objEmplty.SerialNumberTracking = Objitem.SerialNumberTracking;

                if (Objitem.SerialNumberTracking)
                {
                    if (Objitem.Consignment)
                    {
                        objEmplty.ConsignedQuantity = 1;
                    }
                    else
                    {
                        objEmplty.CustomerOwnedQuantity = 1;
                    }
                }

                objEmplty.LotNumberTracking = Objitem.LotNumberTracking;
                objEmplty.OrderDetailGUID = OrderDetailGUID;
                if (!string.IsNullOrEmpty(stageHeaderName))
                    objEmplty.BinNumber = selectStagingBinName;

                objEmplty.WorkOrderGuid = WorkOrderGuid;

                lstData.Add(objEmplty);
            }

            //Set default SerialNumberTracking OR LotNumberTracking - for all records... assuming it is can't be edited after one location add
            lstData = lstData.Select(c => { if (Objitem.SerialNumberTracking) { c.SerialNumberTracking = true; } if (Objitem.LotNumberTracking) { c.LotNumberTracking = true; } return c; }).ToList();
            ViewBag.IsSerialNumberTracking = Objitem.SerialNumberTracking;
            ViewBag.IsLotNumberTracking = Objitem.LotNumberTracking;
            ViewBag.IsDateCodeTracking = Objitem.DateCodeTracking;
            ViewBag.ItemNumber = Objitem.ItemNumber;
            ViewBag.Consignment = Objitem.Consignment;
            ViewBag.IsItemLevelMinMaxQtyRequired = Objitem.IsItemLevelMinMaxQtyRequired;
            if (Objitem.DefaultLocation > 0)
            {
                ViewBag.DefaultLocationBag = Objitem.DefaultLocation;
            }
            return PartialView("_LocationDetailsMS", lstData);
        }





        private List<CommonDTO> GetItemTypeOptions()
        {
            List<CommonDTO> ItemType = new List<CommonDTO>();
            ItemType.Add(new CommonDTO() { ID = 1, Text = "Item" });
            ItemType.Add(new CommonDTO() { ID = 3, Text = "Kit" });
            ItemType.Add(new CommonDTO() { ID = 4, Text = "Labor" });

            return ItemType;
        }

        private List<CommonDTO> GetCountTypeOptions(string CType)
        {
            List<CommonDTO> CountType = new List<CommonDTO>();
            CountType.Add(new CommonDTO() { Value = "A", Text = ResInventoryCount.Adjustment });
            if ((CType ?? string.Empty).ToLower() == "c")
            {
                CountType.Add(new CommonDTO() { Value = "C", Text = ResInventoryCount.Cycle });
            }
            CountType.Add(new CommonDTO() { Value = "M", Text = ResInventoryCount.Manual });
            return CountType;
        }

        private void SetProjectViewBag(Guid ProjectSpendGUID)
        {
            ProjectMasterDAL objProjectApi = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
            List<ProjectMasterDTO> lstProject = new List<ProjectMasterDTO>();
            var projectTrackAllUsageAgainstThis = objProjectApi.GetDefaultProjectSpendRecord(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            if (projectTrackAllUsageAgainstThis != null && projectTrackAllUsageAgainstThis.GUID != Guid.Empty)
            {
                lstProject.Add(projectTrackAllUsageAgainstThis);

                if (ProjectSpendGUID != null && ProjectSpendGUID != Guid.Empty)
                {
                    var currentProject = objProjectApi.GetRecord(ProjectSpendGUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                    lstProject.Add(currentProject);
                }
                ViewBag.ProjectBag = lstProject.OrderBy(x => x.ProjectSpendName).ToList();
            }
            else
            {
                lstProject = objProjectApi.GetAllProjectMasterByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
                ViewBag.ProjectBag = lstProject;
            }

        }

        private List<DTOForAutoComplete> GetPullOrderNumberlist()
        {
            PullMasterDAL objPull = new PullMasterDAL(SessionHelper.EnterPriseDBName);
            return objPull.GetPullOrderNumberForNewPullGrid(string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
        }

        private List<CommonDTO> GetTrendingSettings()
        {
            List<CommonDTO> ItemType = new List<CommonDTO>();
            ItemType.Add(new CommonDTO() { ID = 0, Text = @eTurns.DTO.ResItemMaster.TrendingSettingNone });
            ItemType.Add(new CommonDTO() { ID = 1, Text = @eTurns.DTO.ResItemMaster.TrendingSettingManual });
            ItemType.Add(new CommonDTO() { ID = 2, Text = @eTurns.DTO.ResItemMaster.TrendingSettingAutomatic });
            return ItemType;
        }

        private List<CommonDTO> GetInventoryClassificationOptions()
        {
            List<CommonDTO> ItemType = new List<CommonDTO>();
            ItemType.Add(new CommonDTO() { ID = 1, Text = "A" });
            ItemType.Add(new CommonDTO() { ID = 2, Text = "B" });
            ItemType.Add(new CommonDTO() { ID = 3, Text = "C" });
            ItemType.Add(new CommonDTO() { ID = 4, Text = "D" });
            ItemType.Add(new CommonDTO() { ID = 5, Text = "E" });

            return ItemType;
        }





        public JsonResult CheckManufacturerRoomWise(string ManufacturerNumber, Int64? ID)
        {
            ItemManufacturerDetailsDAL objitemManuDAL = new ItemManufacturerDetailsDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<ItemManufacturerDetailsDTO> LstTempList = null;
            LstTempList = objitemManuDAL.GetItemManufacturerByRoomNormal(SessionHelper.RoomID, SessionHelper.CompanyID);
            //Check Manufacture Number is Unique or Not
            if (LstTempList != null && LstTempList.Count() > 0)
            {
                LstTempList = LstTempList.Where(t => t.ManufacturerNumber == ManufacturerNumber && t.IsDeleted == false && t.IsArchived == false && t.ID != ID).ToList();
                if (LstTempList != null && LstTempList.Count() > 0)
                {
                    return Json(new { status = "false" }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { status = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { status = "true" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckSuppNumberRoomWise(string SuppNumber, Int64? ID)
        {
            ItemSupplierDetailsDAL objitemManuDAL = new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<ItemSupplierDetailsDTO> LstTempList = null;
            LstTempList = objitemManuDAL.GetSuppliersByRoomNormal(SessionHelper.RoomID, SessionHelper.CompanyID);
            //Check Supplier Number is Unique or Not
            if (LstTempList != null && LstTempList.Count() > 0)
            {
                LstTempList = LstTempList.Where(t => t.SupplierNumber == SuppNumber && t.IsDeleted == false && t.IsArchived == false && t.ID != ID).ToList();
                if (LstTempList != null && LstTempList.Count() > 0)
                {
                    return Json(new { status = "false" }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { status = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { status = "true" }, JsonRequestBehavior.AllowGet);
        }


        public PartialViewResult MediaUploadPartial()
        {
            return PartialView(new eTurns.DTO.ItemImageUploadDTO { FileName = "", ParentID = "txtfileupload" });
        }

        [HttpPost]
        public PartialViewResult ItemFileUpload(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                Int64 ItemID = Convert.ToInt64(Request.Form["id"]);
                string Parenturlid = Request.Form["urlid"];
                string UNCPathRoot = HttpRuntime.AppDomainAppPath + System.Configuration.ConfigurationManager.AppSettings["ItemImagePath"].ToString();
                var fileName = Path.GetFileName(file.FileName);
                var strWebfileExt = Path.GetExtension(file.FileName);
                // store the file inside ~/App_Data/uploads folder

                // if company's path not exist then create company's path
                if (!Directory.Exists(Path.Combine(UNCPathRoot, SessionHelper.CompanyID.ToString())))
                {
                    Directory.CreateDirectory(Path.Combine(UNCPathRoot, SessionHelper.CompanyID.ToString()));
                }


                string strPath = "";
                if (ItemID == 0)
                {
                    Session["Temp_FileName"] = "Temp_" + fileName;
                    strPath = Path.Combine(UNCPathRoot, SessionHelper.CompanyID.ToString() + "/Temp_" + fileName);
                }
                else
                {
                    strPath = Path.Combine(UNCPathRoot, SessionHelper.CompanyID.ToString() + "/" + Convert.ToString(ItemID) + strWebfileExt);
                }
                file.SaveAs(strPath);
                Session["FileUploaded"] = fileName;
                Session["Parenturlid"] = Parenturlid;

                if (ItemID == 0)
                {
                    Session["FileUploadedPATH"] = "/InventoryPhoto/" + eTurnsWeb.Helper.SessionHelper.CompanyID.ToString() + "/Temp_" + fileName;
                    Session["FileUploadedItemNumber"] = fileName;
                }
                else
                {
                    Session["FileUploadedPATH"] = "/InventoryPhoto/" + eTurnsWeb.Helper.SessionHelper.CompanyID.ToString() + "/" + Convert.ToString(ItemID) + strWebfileExt + "?" + DateTimeUtility.DateTimeNow.Ticks;
                    Session["FileUploadedItemNumber"] = Convert.ToString(Request.Form["ItemNumber"]).Replace(" ", "") + strWebfileExt;
                }

                //ViewBag.filename = fileName;
                //wait for few seconds
                //System.Threading.Thread.Sleep(3000);

                //return Json(new
                //{
                //    Message = fileName
                //}, JsonRequestBehavior.AllowGet);
                return PartialView("MediaUploadPartial", new ItemImageUploadDTO { FileName = fileName, ParentID = Parenturlid });
            }

            //return Json(new
            //{
            //    Message = "ERROR"
            //}, JsonRequestBehavior.AllowGet);
            return PartialView("MediaUploadPartial");

        }

        [HttpPost]
        public ActionResult ItemFileUploadWithModal(string FileName, HttpPostedFile fileControl, string id, string urlid, string itemNumber)
        {
            if (!string.IsNullOrEmpty(FileName))
            {
                Int64 ItemID = 0;
                if (!string.IsNullOrEmpty(id))
                    ItemID = Convert.ToInt64(id);

                string Parenturlid = urlid;
                string UNCPathRoot = HttpRuntime.AppDomainAppPath + System.Configuration.ConfigurationManager.AppSettings["ItemImagePath"].ToString();
                var fileName = Path.GetFileName(FileName);
                var strWebfileExt = Path.GetExtension(FileName);
                // store the file inside ~/App_Data/uploads folder

                string strPath = "";
                if (ItemID == 0)
                {
                    strPath = Path.Combine(UNCPathRoot, SessionHelper.CompanyID.ToString() + "/Temp_" + fileName);
                }
                else
                {
                    strPath = Path.Combine(UNCPathRoot, SessionHelper.CompanyID.ToString() + "/" + itemNumber + strWebfileExt);
                }
                //file.SaveAs(strPath);
                //System.IO.File.Create(strPath);


                Session["FileUploaded"] = fileName;
                Session["Parenturlid"] = Parenturlid;

                ViewBag.filename = fileName;

                FileInfo file = new FileInfo(FileName);

                FileInfo fileExist = new FileInfo(strPath);
                if (fileExist.Exists)
                    fileExist.Delete();

                file.CopyTo(strPath);

                var fileName1 = Path.GetFileName(strPath);
                var strWebfileExt1 = Path.GetExtension(strPath);
                string FinalUploadePath = "/InventoryPhoto/" + eTurnsWeb.Helper.SessionHelper.CompanyID.ToString() + "/" + fileName1;

                return Json(new
                {
                    Message = fileName1,
                    UploadedPath = FinalUploadePath
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                Message = "ERROR"
            }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult ItemLocationDetailsSave(List<ItemLocationDetailsDTO> objData)
        {
            List<ItemLocationDetailsDTO> objTempItemLocationDetailsDTO = new List<ItemLocationDetailsDTO>();
            foreach (var item in objData)
            {
                objTempItemLocationDetailsDTO.Add(item);
            }

            //Guid? OrderDetailGUID = null;
            //double ReceivedQty = 0;
            Guid? ItemGUID = null;

            //bool isSRTracking = false;
            foreach (var item in objData)
            {
                ItemGUID = item.ItemGUID.Value;
                item.CompanyID = SessionHelper.CompanyID;
                item.Room = SessionHelper.RoomID;

                if (item.ID == 0)
                {
                    item.Created = DateTimeUtility.DateTimeNow;
                    item.CreatedBy = SessionHelper.UserID;
                    item.CreatedByName = SessionHelper.UserName;
                }
                item.Updated = DateTimeUtility.DateTimeNow;
                item.LastUpdatedBy = SessionHelper.UserID;
                item.UpdatedByName = SessionHelper.UserName;

                if (string.IsNullOrEmpty(item.Action))
                    item.Action = "Receive";

                ////TODO: Added By CP
                //if (item.OrderDetailGUID != null)
                //{
                //    OrderDetailGUID = item.OrderDetailGUID;

                //    if (!item.SerialNumberTracking)
                //    {
                //        if (item.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                //        {
                //            ReceivedQty += double.Parse(item.CustomerOwnedQuantity.ToString());
                //        }
                //        else if (item.ConsignedQuantity.GetValueOrDefault(0) > 0)
                //        {
                //            ReceivedQty += double.Parse(item.ConsignedQuantity.ToString());
                //        }
                //        else
                //        {
                //            ReceivedQty += 0;
                //        }
                //    }
                //    else
                //    {
                //        if (!string.IsNullOrEmpty(item.SerialNumber) && item.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                //        {
                //            ReceivedQty += double.Parse(item.CustomerOwnedQuantity.ToString());
                //        }
                //        else if (!string.IsNullOrEmpty(item.SerialNumber) && item.ConsignedQuantity.GetValueOrDefault(0) > 0)
                //        {
                //            ReceivedQty += double.Parse(item.ConsignedQuantity.ToString());
                //        }
                //        else
                //        {
                //            ReceivedQty += 0;
                //        }

                //    }
                //}
                ////End: Added By CP
            }

            //add locations details which are not displayed but it's available.
            //if (OrderDetailGUID != null)
            //{
            //    ItemLocationDetailsDAL objAPI = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            //    List<ItemLocationDetailsDTO> lstTemp = objAPI.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, ItemGUID.Value, null, "ID DESC", OrderDetailGUID).ToList();

            //    ItemMasterDAL objItemAPI = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            //    var Objitem = objItemAPI.GetRecord(ItemGUID.Value.ToString(), SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
            //    isSRTracking = Objitem.SerialNumberTracking;

            //    lstTemp.Select(c =>
            //    {
            //        c.SerialNumberTracking = Objitem.SerialNumberTracking;
            //        c.LotNumberTracking = Objitem.LotNumberTracking;
            //        c.DateCodeTracking = Objitem.DateCodeTracking;
            //        return c;
            //    }).ToList();

            //    foreach (var iTemp in lstTemp)
            //    {
            //        if (objData.FindIndex(x => x.ID == iTemp.ID) < 0)
            //            objData.Add(iTemp);
            //    }
            //}
            //else
            //{
            //<Added-by-AmitP> Get Previous Records, which are stored...
            ItemLocationDetailsDAL objAPI = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            //List<ItemLocationDetailsDTO> lstTemp = objAPI.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, ItemGUID.Value, OrderDetailGUID, "ID DESC").ToList();
            List<ItemLocationDetailsDTO> lstTemp = objData;
            ItemMasterDAL objItemAPI = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            var Objitem = objItemAPI.GetItemWithoutJoins(null, ItemGUID);

            lstTemp.Select(c =>
            {
                c.SerialNumberTracking = Objitem.SerialNumberTracking;
                c.LotNumberTracking = Objitem.LotNumberTracking;
                c.DateCodeTracking = Objitem.DateCodeTracking;
                return c;
            }).ToList();

            foreach (var iTemp in lstTemp)
            {
                if (objData.FindIndex(x => x.ID == iTemp.ID) < 0)
                    objData.Add(iTemp);
            }
            //</Added-by-AmitP>
            //}
            ItemLocationDetailsDAL obj = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            long SessionUserId = SessionHelper.UserID;
            if (obj.ItemLocationDetailsSave(objData, RoomDateFormat, SessionUserId, SessionHelper.EnterPriceID))
            {
                ////TODO: Added By CP
                //if (OrderDetailGUID != null)
                //{
                //    OrderDetailsDAL ordDetailCtrl = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                //    OrderDetailsDTO objOrdDetailDTO = ordDetailCtrl.GetRecord(OrderDetailGUID.Value, SessionHelper.RoomID, SessionHelper.CompanyID);
                //    if (!isSRTracking)
                //        objOrdDetailDTO.ReceivedQuantity = objOrdDetailDTO.ReceivedQuantity.GetValueOrDefault(0) + ReceivedQty;
                //    else
                //        objOrdDetailDTO.ReceivedQuantity = ReceivedQty;

                //    ordDetailCtrl.Edit(objOrdDetailDTO);
                //    ordDetailCtrl.UpdateOrderStatusByReceive(objOrdDetailDTO.OrderGUID.GetValueOrDefault(Guid.Empty), objOrdDetailDTO.Room.GetValueOrDefault(0), objOrdDetailDTO.CompanyID.GetValueOrDefault(0), objOrdDetailDTO.LastUpdatedBy.GetValueOrDefault(0));

                //}
                //////TODO: END Added By CP
                CheckValueSendMail(objTempItemLocationDetailsDTO);
                lstItemLocationSendMailDTO = null;

                return Json(new { Message = ResCommon.RecordsSavedSuccessfully, Status = "OK" }, JsonRequestBehavior.AllowGet);

            }
            else
                return Json(new { Message = "Fail", Status = "Fail" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ItemLocationDetailsImportSave(ItemLocationDetailsDTO objData)
        {
            List<ItemLocationDetailsDTO> objTempItemLocationDetailsDTO = new List<ItemLocationDetailsDTO>();
            // foreach (var item in objData)
            // {
            objTempItemLocationDetailsDTO.Add(objData);
            // }

            Guid? OrderDetailGUID = null;
            //double ReceivedQty = 0;
            Guid? ItemGUID = null;

            //bool isSRTracking = false;
            // foreach (var item in objData)
            // {
            ItemGUID = objData.ItemGUID.Value;
            objData.CompanyID = SessionHelper.CompanyID;
            objData.Room = SessionHelper.RoomID;

            if (objData.ID == 0)
            {
                objData.Created = DateTimeUtility.DateTimeNow;
                objData.CreatedBy = SessionHelper.UserID;
                objData.CreatedByName = SessionHelper.UserName;
            }
            objData.Updated = DateTimeUtility.DateTimeNow;
            objData.LastUpdatedBy = SessionHelper.UserID;
            objData.UpdatedByName = SessionHelper.UserName;

            ////TODO: Added By CP
            //if (objData.OrderDetailGUID != null)
            //{
            //    OrderDetailGUID = objData.OrderDetailGUID;

            //    if (!objData.SerialNumberTracking)
            //    {
            //        if (objData.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
            //        {
            //            ReceivedQty += double.Parse(objData.CustomerOwnedQuantity.ToString());
            //        }
            //        else if (objData.ConsignedQuantity.GetValueOrDefault(0) > 0)
            //        {
            //            ReceivedQty += double.Parse(objData.ConsignedQuantity.ToString());
            //        }
            //        else
            //        {
            //            ReceivedQty += 0;
            //        }
            //    }
            //    else
            //    {
            //        if (!string.IsNullOrEmpty(objData.SerialNumber) && objData.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
            //        {
            //            ReceivedQty += double.Parse(objData.CustomerOwnedQuantity.ToString());
            //        }
            //        else if (!string.IsNullOrEmpty(objData.SerialNumber) && objData.ConsignedQuantity.GetValueOrDefault(0) > 0)
            //        {
            //            ReceivedQty += double.Parse(objData.ConsignedQuantity.ToString());
            //        }
            //        else
            //        {
            //            ReceivedQty += 0;
            //        }

            //    }
            //}
            ////End: Added By CP


            ////add locations details which are not displayed but it's available.
            //if (OrderDetailGUID != null)
            //{
            //    ItemLocationDetailsDAL objAPI = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            //    List<ItemLocationDetailsDTO> lstTemp = objAPI.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, ItemGUID.Value, null, "ID DESC", OrderDetailGUID).ToList();

            //    ItemMasterDAL objItemAPI = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            //    var Objitem = objItemAPI.GetRecord(ItemGUID.Value.ToString(), SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
            //    isSRTracking = Objitem.SerialNumberTracking;

            //    lstTemp.Select(c =>
            //    {
            //        c.SerialNumberTracking = Objitem.SerialNumberTracking;
            //        c.LotNumberTracking = Objitem.LotNumberTracking;
            //        c.DateCodeTracking = Objitem.DateCodeTracking;
            //        return c;
            //    }).ToList();

            //    //foreach (var iTemp in lstTemp)
            //    //{
            //    //    if ((objData.ID == iTemp.ID) < 0)
            //    //        objData.Add(iTemp);
            //    //}
            //}
            //else
            //{
            //<Added-by-AmitP> Get Previous Records, which are stored...
            ItemLocationDetailsDAL objAPI = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            List<ItemLocationDetailsDTO> lstTemp = objAPI.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, ItemGUID.Value, OrderDetailGUID, "ID DESC").ToList();

            ItemMasterDAL objItemAPI = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            var Objitem = objItemAPI.GetItemWithoutJoins(null, ItemGUID);

            lstTemp.Select(c =>
            {
                c.SerialNumberTracking = Objitem.SerialNumberTracking;
                c.LotNumberTracking = Objitem.LotNumberTracking;
                c.DateCodeTracking = Objitem.DateCodeTracking;
                return c;
            }).ToList();

            //foreach (var iTemp in lstTemp)
            //{
            //    if (objData.FindIndex(x => x.ID == iTemp.ID) < 0)
            //        objData.Add(iTemp);
            //}
            //</Added-by-AmitP>
            // }
            ItemLocationDetailsDAL obj = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            long SessionUserId = SessionHelper.UserID;
            if (obj.ItemLocationDetailsImportSave(objData, SessionUserId,SessionHelper.EnterPriceID))
            {
                ////TODO: Added By CP
                //if (OrderDetailGUID != null)
                //{
                //    OrderDetailsDAL ordDetailCtrl = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                //    OrderDetailsDTO objOrdDetailDTO = ordDetailCtrl.GetRecord(OrderDetailGUID.Value, SessionHelper.RoomID, SessionHelper.CompanyID);
                //    if (!isSRTracking)
                //        objOrdDetailDTO.ReceivedQuantity = objOrdDetailDTO.ReceivedQuantity.GetValueOrDefault(0) + ReceivedQty;
                //    else
                //        objOrdDetailDTO.ReceivedQuantity = ReceivedQty;

                //    ordDetailCtrl.Edit(objOrdDetailDTO);
                //    ordDetailCtrl.UpdateOrderStatusByReceive(objOrdDetailDTO.OrderGUID.GetValueOrDefault(Guid.Empty), objOrdDetailDTO.Room.GetValueOrDefault(0), objOrdDetailDTO.CompanyID.GetValueOrDefault(0), objOrdDetailDTO.LastUpdatedBy.GetValueOrDefault(0));

                //}
                //////TODO: END Added By CP
                CheckValueSendMail(objTempItemLocationDetailsDTO);
                lstItemLocationSendMailDTO = null;

                return Json(new { Message = ResCommon.RecordsSavedSuccessfully, Status = "OK" }, JsonRequestBehavior.AllowGet);

            }
            else
                return Json(new { Message = "Fail", Status = "Fail" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ItemLocationDetailsSaveForSerailKitBuild(List<ItemLocationDetailsDTO> objData)
        {
            Guid? OrderDetailGUID = null;
            double ReceivedQty = 0;
            Guid? ItemGUID = null;
            bool isSRTracking = false;

            ItemMasterDTO Objitem = null;
            foreach (var item in objData)
            {
                ItemGUID = item.ItemGUID.Value;
                item.CompanyID = SessionHelper.CompanyID;
                item.Room = SessionHelper.RoomID;

                if (item.ID == 0)
                {
                    item.Created = DateTimeUtility.DateTimeNow;
                    item.CreatedBy = SessionHelper.UserID;
                    item.CreatedByName = SessionHelper.UserName;

                    if (!string.IsNullOrEmpty(item.SerialNumber) && item.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                    {
                        ReceivedQty += double.Parse(item.CustomerOwnedQuantity.ToString());
                    }
                    else if (!string.IsNullOrEmpty(item.SerialNumber) && item.ConsignedQuantity.GetValueOrDefault(0) > 0)
                    {
                        ReceivedQty += double.Parse(item.ConsignedQuantity.ToString());
                    }
                    else
                    {
                        ReceivedQty += 0;
                    }
                }
                item.Updated = DateTimeUtility.DateTimeNow;
                item.LastUpdatedBy = SessionHelper.UserID;
                item.UpdatedByName = SessionHelper.UserName;
                item.Action = "Receive";

            }


            //<Added-by-AmitP> Get Previous Records, which are stored...
            ItemLocationDetailsDAL objAPI = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            List<ItemLocationDetailsDTO> lstTemp = objAPI.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, ItemGUID.Value, OrderDetailGUID, "ID DESC").ToList();

            ItemMasterDAL objItemAPI = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            Objitem = objItemAPI.GetItemWithoutJoins(null, ItemGUID);
            isSRTracking = Objitem.SerialNumberTracking;
            lstTemp.Select(c =>
            {
                c.SerialNumberTracking = Objitem.SerialNumberTracking;
                c.LotNumberTracking = Objitem.LotNumberTracking;
                c.DateCodeTracking = Objitem.DateCodeTracking;
                return c;
            }).ToList();

            foreach (var iTemp in lstTemp)
            {
                if (objData.FindIndex(x => x.ID == iTemp.ID) < 0)
                    objData.Add(iTemp);
            }
            //</Added-by-AmitP>

            ItemLocationDetailsDAL obj = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            long SessionUserId = SessionHelper.UserID;
            var enterpriseId = SessionHelper.EnterPriceID;

            if (obj.ItemLocationDetailsSave(objData, RoomDateFormat, SessionUserId,enterpriseId))
            {
                if (Objitem.ItemType == 3 && Objitem.IsBuildBreak.GetValueOrDefault(false) && Objitem.SerialNumberTracking)
                {
                    KitDetailDAL objKitDetailctrl = new KitDetailDAL(SessionHelper.EnterPriseDBName);
                    List<KitDetailDTO> lstKitDetailDTO = objKitDetailctrl.GetAllRecordsByKitGUID(Objitem.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, true).ToList();
                    foreach (var item in lstKitDetailDTO)
                    {
                        if (item.ItemType != 4)//if (item?.ItemDetail.ItemType != 4)
                        {
                            item.AvailableItemsInWIP = item.AvailableItemsInWIP.GetValueOrDefault(0) - (item.QuantityPerKit * ReceivedQty);
                            item.LastUpdatedBy = SessionHelper.UserID;
                            item.ReceivedOn = DateTimeUtility.DateTimeNow;
                            item.EditedFrom = "Web";
                            objKitDetailctrl.Edit(item, SessionUserId,enterpriseId);
                        }
                    }
                }
                ////TODO: END Added By CP

                return Json(new { Message = ResCommon.RecordsSavedSuccessfully, Status = "OK" }, JsonRequestBehavior.AllowGet);

            }
            else
                return Json(new { Message = "Fail", Status = "Fail" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ItemLocationDetailsSaveOrder(List<ItemLocationDetailsDTO> objData)
        {
            string msg = string.Empty;
            string status = string.Empty;
            string OrdStatusText = string.Empty;
            var enterpriseId = SessionHelper.EnterPriceID;
            BinMasterDAL objItemLocationDetailsDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetRequiredUDFsByUDFTableNamePlain("ReceivedOrderTransferDetail", SessionHelper.RoomID, SessionHelper.CompanyID);
            string udfRequier = string.Empty;

            foreach (var baseItem in objData)
            {
                foreach (var i in DataFromDB)
                {
                    if (i.UDFColumnName == "UDF1" && string.IsNullOrEmpty(baseItem.UDF1))
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;
                        udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                    }
                    else if (i.UDFColumnName == "UDF2" && string.IsNullOrEmpty(baseItem.UDF2))
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;
                        udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                    }
                    else if (i.UDFColumnName == "UDF3" && string.IsNullOrEmpty(baseItem.UDF3))
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;
                        udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                    }
                    else if (i.UDFColumnName == "UDF4" && string.IsNullOrEmpty(baseItem.UDF4))
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;
                        udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                    }
                    else if (i.UDFColumnName == "UDF5" && string.IsNullOrEmpty(baseItem.UDF5))
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;
                        udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                    }

                    if (!string.IsNullOrEmpty(udfRequier))
                        break;

                }

                if (!string.IsNullOrEmpty(udfRequier))
                    break;
            }

            if (!string.IsNullOrEmpty(udfRequier))
            {
                return Json(new { Message = udfRequier, Status = "UDFError" }, JsonRequestBehavior.AllowGet);
            }

            bool IsFromUI = false;
            foreach (var baseItem in objData)
            {
                baseItem.Action = "Receive";
                if (!IsFromUI && baseItem.IsOnlyFromUI)
                    IsFromUI = true;

                Guid? OrderDetailGUID = null;
                double ReceivedQty = 0;
                Guid? ItemGUID = null;

                //bool isSRTracking = false;
                List<ItemLocationDetailsDTO> lstData = new List<ItemLocationDetailsDTO>();
                lstData.Add(baseItem);

                if (!string.IsNullOrEmpty(baseItem.BinNumber) && baseItem.BinID.GetValueOrDefault(0) <= 0)
                {
                    BinMasterDTO objBinMasterDTO = objItemLocationDetailsDAL.GetItemBinPlain(baseItem.ItemGUID ?? Guid.Empty, baseItem.BinNumber, RoomID, CompanyID, SessionHelper.UserID, false);
                    baseItem.BinID = objBinMasterDTO.ID;
                    //baseItem.BinID = new CommonDAL(SessionHelper.EnterPriseDBName).GetOrInsertBinIDByName(baseItem.ItemGUID.GetValueOrDefault(Guid.Empty), baseItem.BinNumber, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, false);
                }

                foreach (var item in lstData)
                {
                    ItemGUID = item.ItemGUID.Value;
                    item.CompanyID = SessionHelper.CompanyID;
                    item.Room = SessionHelper.RoomID;

                    if (item.ID == 0)
                    {
                        item.Created = DateTimeUtility.DateTimeNow;
                        item.CreatedBy = SessionHelper.UserID;
                        item.CreatedByName = SessionHelper.UserName;
                    }
                    item.ReceivedDate = item.Received != null ? DateTime.ParseExact(item.Received, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult) : Convert.ToDateTime(item.Received);
                    item.Updated = DateTimeUtility.DateTimeNow;
                    item.LastUpdatedBy = SessionHelper.UserID;
                    item.UpdatedByName = SessionHelper.UserName;

                    //TODO: Added By CP
                    if (item.OrderDetailGUID != null)
                    {
                        OrderDetailGUID = item.OrderDetailGUID;

                        if (!item.SerialNumberTracking)
                        {
                            if (item.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                            {
                                ReceivedQty += double.Parse(item.CustomerOwnedQuantity.ToString());
                            }
                            else if (item.ConsignedQuantity.GetValueOrDefault(0) > 0)
                            {
                                ReceivedQty += double.Parse(item.ConsignedQuantity.ToString());
                            }
                            else
                            {
                                ReceivedQty += 0;
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(item.SerialNumber) && item.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                            {
                                ReceivedQty += double.Parse(item.CustomerOwnedQuantity.ToString());
                            }
                            else if (!string.IsNullOrEmpty(item.SerialNumber) && item.ConsignedQuantity.GetValueOrDefault(0) > 0)
                            {
                                ReceivedQty += double.Parse(item.ConsignedQuantity.ToString());
                            }
                            else
                            {
                                ReceivedQty += 0;
                            }

                        }
                    }
                    //End: Added By CP
                }

                //add locations details which are not displayed but it's available.
                if (OrderDetailGUID != null)
                {
                    ItemLocationDetailsDAL objAPI = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                    List<ItemLocationDetailsDTO> lstTemp = new List<ItemLocationDetailsDTO>();// objAPI.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, ItemGUID.Value, null, "ID DESC", OrderDetailGUID).ToList();

                    //ItemMasterDAL objItemAPI = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                    //var Objitem = objItemAPI.GetRecord(ItemGUID.Value.ToString(), SessionHelper.RoomID, SessionHelper.CompanyID);
                    //isSRTracking = Objitem.SerialNumberTracking;

                    //lstTemp.Select(c =>
                    //{
                    //    c.SerialNumberTracking = Objitem.SerialNumberTracking;
                    //    c.LotNumberTracking = Objitem.LotNumberTracking;
                    //    c.DateCodeTracking = Objitem.DateCodeTracking;
                    //    return c;
                    //}).ToList();

                    foreach (var iTemp in lstTemp)
                    {
                        if (lstData.FindIndex(x => x.ID == iTemp.ID) < 0)
                            lstData.Add(iTemp);
                    }
                }

                ItemLocationDetailsDAL obj = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
                long SessionUserId = SessionHelper.UserID;
                if (obj.ItemLocationDetailsSave(lstData, RoomDateFormat, SessionUserId, SessionHelper.EnterPriceID))
                {
                    //TODO: Added By CP
                    if (OrderDetailGUID != null)
                    {
                        OrderDetailsDAL ordDetailCtrl = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                        OrderDetailsDTO objOrdDetailDTO = ordDetailCtrl.GetOrderDetailByGuidPlain(OrderDetailGUID.Value, SessionHelper.RoomID, SessionHelper.CompanyID);

                        ReceivedOrderTransferDetailDAL objDAL = new ReceivedOrderTransferDetailDAL(SessionHelper.EnterPriseDBName);
                        IEnumerable<ReceivedOrderTransferDetailDTO> lst = objDAL.GetROTDByOrderDetailGUIDPlain(OrderDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID).OrderByDescending(x => x.ID).ToList();

                        double rcvQty = lst.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0));
                        rcvQty += lst.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0));
                        objOrdDetailDTO.ReceivedQuantity = rcvQty;
                        if (IsFromUI)
                            objOrdDetailDTO.IsOnlyFromUI = true;


                        if (IsFromUI)
                        {
                            objOrdDetailDTO.EditedFrom = "Web";
                            objOrdDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        }
                        objOrdDetailDTO.LastUpdatedBy = SessionHelper.UserID;
                        ordDetailCtrl.Edit(objOrdDetailDTO, SessionUserId,enterpriseId);
                        ordDetailCtrl.UpdateOrderStatusByReceive(objOrdDetailDTO.OrderGUID.GetValueOrDefault(Guid.Empty), objOrdDetailDTO.Room.GetValueOrDefault(0), objOrdDetailDTO.CompanyID.GetValueOrDefault(0), objOrdDetailDTO.LastUpdatedBy.GetValueOrDefault(0), IsFromUI);

                        OrderMasterDAL objOrdDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                        OrderMasterDTO order = objOrdDAL.GetOrderByGuidPlain(objOrdDetailDTO.OrderGUID.GetValueOrDefault(Guid.Empty));

                        if (order != null)
                        {
                            OrdStatusText = order.OrderStatusText; //objDTO.OrderStatusText;
                        }
                    }
                    ////TODO: END Added By CP

                    msg = ResCommon.RecordsSavedSuccessfully;
                    status = "OK";
                }
                else
                {
                    msg = "Fail";
                    status = "Fail";
                    break;
                }
            }
            return Json(new { Message = msg, Status = status, OrderStatusText = OrdStatusText }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ItemLocationDetailSaveNewReceiveOrderForSerial(ItemLocationDetailsDTO objData)
        {
            string msg = string.Empty;
            string status = string.Empty;
            double ReceivedQty = 0;
            List<ItemLocationDetailsDTO> lstData = new List<ItemLocationDetailsDTO>();


            if (!string.IsNullOrEmpty(objData.SerialNumber) && objData.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
            {
                ReceivedQty += double.Parse(objData.CustomerOwnedQuantity.ToString());
            }
            else if (!string.IsNullOrEmpty(objData.SerialNumber) && objData.ConsignedQuantity.GetValueOrDefault(0) > 0)
            {
                ReceivedQty += double.Parse(objData.ConsignedQuantity.ToString());
            }
            else
            {
                ReceivedQty += 0;
            }

            objData.Action = "Receive";

            lstData.Add(objData);

            ItemLocationDetailsDAL objAPI = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            List<ItemLocationDetailsDTO> lstTemp = new List<ItemLocationDetailsDTO>(); // objAPI.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, objData.ItemGUID.GetValueOrDefault(Guid.Empty), null, "ID DESC", objData.OrderDetailGUID.GetValueOrDefault(Guid.Empty)).ToList();

            ItemMasterDAL objItemAPI = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            var Objitem = objItemAPI.GetItemWithoutJoins(null, objData.ItemGUID.GetValueOrDefault(Guid.Empty));
            lstTemp.Select(c =>
            {
                c.SerialNumberTracking = Objitem.SerialNumberTracking;
                c.LotNumberTracking = Objitem.LotNumberTracking;
                c.DateCodeTracking = Objitem.DateCodeTracking;
                return c;
            }).ToList();

            foreach (var iTemp in lstTemp)
            {
                if (lstData.FindIndex(x => x.ID == iTemp.ID) < 0)
                    lstData.Add(iTemp);
            }


            ItemLocationDetailsDAL obj = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            long SessionUserId = SessionHelper.UserID;
            if (obj.ItemLocationDetailsSave(lstData, RoomDateFormat, SessionUserId, SessionHelper.EnterPriceID))
            {
                OrderDetailsDAL ordDetailCtrl = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                OrderDetailsDTO objOrdDetailDTO = ordDetailCtrl.GetOrderDetailByGuidPlain(objData.OrderDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                objOrdDetailDTO.ReceivedQuantity = objOrdDetailDTO.ReceivedQuantity.GetValueOrDefault(0) + ReceivedQty;
                objOrdDetailDTO.IsEDISent = true;
                objOrdDetailDTO.LastEDIDate = DateTimeUtility.DateTimeNow;
                if (objData.IsOnlyFromUI)
                {
                    objOrdDetailDTO.IsOnlyFromUI = true;
                    objOrdDetailDTO.EditedFrom = "Web";
                    objOrdDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                }
                ordDetailCtrl.Edit(objOrdDetailDTO, SessionUserId,SessionHelper.EnterPriceID);
                ordDetailCtrl.UpdateOrderStatusByReceive(objOrdDetailDTO.OrderGUID.GetValueOrDefault(Guid.Empty), objOrdDetailDTO.Room.GetValueOrDefault(0), objOrdDetailDTO.CompanyID.GetValueOrDefault(0), objOrdDetailDTO.LastUpdatedBy.GetValueOrDefault(0), objOrdDetailDTO.IsOnlyFromUI);

                msg = ResCommon.RecordsSavedSuccessfully;
                status = "OK";
            }
            else
            {
                msg = "Fail";
                status = "Fail";
            }

            return Json(new { Message = msg, Status = status }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ItemLocationDetailSaveNewReceiveOrderForNotSerial(ItemLocationDetailsDTO objData)
        {
            string msg = string.Empty;
            string status = string.Empty;
            double ReceivedQty = 0;

            List<ItemLocationDetailsDTO> lstData = new List<ItemLocationDetailsDTO>();

            if (objData.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
            {
                ReceivedQty += double.Parse(objData.CustomerOwnedQuantity.ToString());
            }
            else if (objData.ConsignedQuantity.GetValueOrDefault(0) > 0)
            {
                ReceivedQty += double.Parse(objData.ConsignedQuantity.ToString());
            }
            else
            {
                ReceivedQty += 0;
            }
            objData.Action = "Receive";
            lstData.Add(objData);

            ItemLocationDetailsDAL objAPI = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            List<ItemLocationDetailsDTO> lstTemp = new List<ItemLocationDetailsDTO>();

            //objAPI.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, objData.ItemGUID.GetValueOrDefault(Guid.Empty), null, "ID DESC", objData.OrderDetailGUID.GetValueOrDefault(Guid.Empty)).ToList();

            ItemMasterDAL objItemAPI = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            var Objitem = objItemAPI.GetItemWithoutJoins(null, objData.ItemGUID.GetValueOrDefault(Guid.Empty));

            lstTemp.Select(c =>
            {
                c.SerialNumberTracking = Objitem.SerialNumberTracking;
                c.LotNumberTracking = Objitem.LotNumberTracking;
                c.DateCodeTracking = Objitem.DateCodeTracking;
                return c;
            }).ToList();

            foreach (var iTemp in lstTemp)
            {
                if (lstData.FindIndex(x => x.ID == iTemp.ID) < 0)
                    lstData.Add(iTemp);
            }


            ItemLocationDetailsDAL obj = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            long SessionUserId = SessionHelper.UserID;
            if (obj.ItemLocationDetailsSave(lstData, RoomDateFormat, SessionUserId,SessionHelper.EnterPriceID))
            {
                OrderDetailsDAL ordDetailCtrl = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                OrderDetailsDTO objOrdDetailDTO = ordDetailCtrl.GetOrderDetailByGuidPlain(objData.OrderDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                objOrdDetailDTO.ReceivedQuantity = objOrdDetailDTO.ReceivedQuantity.GetValueOrDefault(0) + ReceivedQty;
                objOrdDetailDTO.IsEDISent = true;
                objOrdDetailDTO.LastEDIDate = DateTimeUtility.DateTimeNow;
                if (objData.IsOnlyFromUI)
                {
                    objOrdDetailDTO.IsOnlyFromUI = true;
                    objOrdDetailDTO.EditedFrom = "Web";
                    objOrdDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                }

                ordDetailCtrl.Edit(objOrdDetailDTO, SessionUserId,SessionHelper.EnterPriceID);
                ordDetailCtrl.UpdateOrderStatusByReceive(objOrdDetailDTO.OrderGUID.GetValueOrDefault(Guid.Empty), objOrdDetailDTO.Room.GetValueOrDefault(0), objOrdDetailDTO.CompanyID.GetValueOrDefault(0), objOrdDetailDTO.LastUpdatedBy.GetValueOrDefault(0), objOrdDetailDTO.IsOnlyFromUI);

                msg = ResCommon.RecordsSavedSuccessfully;
                status = "OK";
            }
            else
            {
                msg = "Fail";
                status = "Fail";
            }

            return Json(new { Message = msg, Status = status }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ItemLocationDetailsSaveForCreditPull(List<ItemLocationDetailsDTO> objData, string PullCreditType = "credit", string CountType = null)
        {
            UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetRequiredUDFsByUDFTableNamePlain("PullMaster", SessionHelper.RoomID, SessionHelper.CompanyID);
            string udfRequier = string.Empty;

            foreach (var item in objData)
            {
                if (string.IsNullOrWhiteSpace(item.BinNumber) || item.BinNumber.ToLower() == "null")
                {
                    return Json(new { Message = ResItemMaster.EnterInventoryLocation, Status = "Fail" }, JsonRequestBehavior.AllowGet);
                }
                if (item.CountLineItemDtlGUID == null)
                {
                    foreach (var i in DataFromDB)
                    {
                        if (i.UDFColumnName == "UDF1" && string.IsNullOrWhiteSpace(item.UDF1))
                        {
                            string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                            string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                            if (!string.IsNullOrEmpty(val))
                                i.UDFDisplayColumnName = val;
                            else
                                i.UDFDisplayColumnName = i.UDFColumnName;
                            udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                        }
                        else if (i.UDFColumnName == "UDF2" && string.IsNullOrWhiteSpace(item.UDF2))
                        {
                            string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                            string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                            if (!string.IsNullOrEmpty(val))
                                i.UDFDisplayColumnName = val;
                            else
                                i.UDFDisplayColumnName = i.UDFColumnName;
                            udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                        }
                        else if (i.UDFColumnName == "UDF3" && string.IsNullOrWhiteSpace(item.UDF3))
                        {
                            string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                            string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                            if (!string.IsNullOrEmpty(val))
                                i.UDFDisplayColumnName = val;
                            else
                                i.UDFDisplayColumnName = i.UDFColumnName;
                            udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                        }
                        else if (i.UDFColumnName == "UDF4" && string.IsNullOrWhiteSpace(item.UDF4))
                        {
                            string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                            string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                            if (!string.IsNullOrEmpty(val))
                                i.UDFDisplayColumnName = val;
                            else
                                i.UDFDisplayColumnName = i.UDFColumnName;
                            udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                        }
                        else if (i.UDFColumnName == "UDF5" && string.IsNullOrWhiteSpace(item.UDF5))
                        {
                            string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                            string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                            if (!string.IsNullOrEmpty(val))
                                i.UDFDisplayColumnName = val;
                            else
                                i.UDFDisplayColumnName = i.UDFColumnName;
                            udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                        }

                        if (!string.IsNullOrEmpty(udfRequier))
                            break;

                    }
                }
                if (!string.IsNullOrEmpty(udfRequier))
                {
                    return Json(new { Message = udfRequier, Status = "Fail" }, JsonRequestBehavior.AllowGet);
                }
            }

            //Guid? OrderDetailGUID = null;
            //double ReceivedQty = 0;
            Guid? ItemGUID = null;
            List<PullMasterViewDTO> PullMstViewDTOList = null;
            List<PullDetailsDTO> oPullDetailsToUpdate = null;
            PullMasterDAL pullMasterDAL = null;
            if (Request != null && Request.UrlReferrer != null &&
                (Request.UrlReferrer.ToString().ToLower().Contains("/pull/pullmasterlist")
                || Request.UrlReferrer.ToString().ToLower().Contains("/pull/newpull")
                || Request.UrlReferrer.ToString().ToLower().Contains("/workorder/workorderlist")
                || Request.UrlReferrer.ToString().ToLower().Contains("/assets/maintenance")))
            {
                objData = objData.Where(x => (x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0)) > 0).ToList();
                string errormsg = ResMessage.AddQuantityRequired;

                if (objData == null || objData.Count <= 0)
                {
                    return Json(new { Message = errormsg, Status = "Fail" }, JsonRequestBehavior.AllowGet);
                }

                pullMasterDAL = new PullMasterDAL(SessionHelper.EnterPriseDBName);
                PullMstViewDTOList = pullMasterDAL.GetPullByItemGuidAndPullCreditType(SessionHelper.RoomID, SessionHelper.CompanyID, objData[0].ItemGUID.GetValueOrDefault(Guid.Empty)).ToList();
                List<ItemLocationDetailsDTO> oDataFinal = new List<ItemLocationDetailsDTO>();
                oPullDetailsToUpdate = new List<PullDetailsDTO>();
                string msg = GetItemLocationDetailDataForCreditPull(PullMstViewDTOList, objData, oDataFinal, oPullDetailsToUpdate);
                objData = oDataFinal.Where(x => x.ConsignedQuantity.GetValueOrDefault(0) > 0 || x.CustomerOwnedQuantity.GetValueOrDefault(0) > 0).ToList();

                if (msg != "ok")
                {
                    return Json(new { Message = msg, Status = "Fail" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ItemMasterDAL objItemAPI = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                foreach (var item in objData)
                {
                    var Objitem = objItemAPI.GetItemWithoutJoins(null, item.ItemGUID);
                    item.Cost = Objitem.Cost;
                }
            }

            BinMasterDTO objBinMasterDTO = new BinMasterDTO();
            BinMasterDAL objItemLocationDetailsDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            foreach (var item in objData)
            {
                ItemGUID = item.ItemGUID;
                item.CompanyID = SessionHelper.CompanyID;
                item.Room = SessionHelper.RoomID;

                //item.BinID = new CommonDAL(SessionHelper.EnterPriseDBName).GetOrInsertBinIDByNameItemGuid(ItemGUID.GetValueOrDefault(Guid.Empty), item.BinNumber, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, false);
                if (!string.IsNullOrEmpty(item.BinNumber) && item.BinID.GetValueOrDefault(0) <= 0)
                {
                    objBinMasterDTO = new BinMasterDTO();
                    objBinMasterDTO = objItemLocationDetailsDAL.GetItemBinPlain(item.ItemGUID.GetValueOrDefault(Guid.Empty), item.BinNumber, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, false);
                    item.BinID = objBinMasterDTO.ID;
                    //item.BinID = new CommonDAL(SessionHelper.EnterPriseDBName).GetOrInsertBinIDByName(item.ItemGUID.GetValueOrDefault(Guid.Empty), item.BinNumber, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, false);
                }
                if (item.ID == 0)
                {
                    item.Created = DateTimeUtility.DateTimeNow;
                    item.CreatedBy = SessionHelper.UserID;
                    item.CreatedByName = SessionHelper.UserName;
                }
                item.Updated = DateTimeUtility.DateTimeNow;
                item.LastUpdatedBy = SessionHelper.UserID;
                item.UpdatedByName = SessionHelper.UserName;

                if (!item.ReceivedDate.HasValue)
                {
                    item.ReceivedDate = DateTimeUtility.DateTimeNow;
                    item.Received = item.ReceivedDate.Value.ToString("MM/dd/yyyy");
                }

                ////TODO: Added By CP
                //if (item.OrderDetailGUID != null)
                //{
                //    OrderDetailGUID = item.OrderDetailGUID;
                //    if (item.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                //    {
                //        ReceivedQty += double.Parse(item.CustomerOwnedQuantity.ToString());
                //    }
                //    else if (item.ConsignedQuantity.GetValueOrDefault(0) > 0)
                //    {
                //        ReceivedQty += double.Parse(item.ConsignedQuantity.ToString());
                //    }
                //    else
                //    {
                //        ReceivedQty += 0;
                //    }
                //}
                ////End: Added By CP
            }

            //<Added-by-AmitP> Get Previous Records, which are stored...
            //ItemLocationDetailsDAL objAPI = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            //List<ItemLocationDetailsDTO> lstTemp = objAPI.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, ItemGUID.Value, OrderDetailGUID, "ID DESC").ToList();

            //ItemMasterDAL objItemAPI = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            //var Objitem = objItemAPI.GetRecord(ItemGUID.Value.ToString(), SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            //lstTemp.Select(c =>
            //        {
            //            c.SerialNumberTracking = Objitem.SerialNumberTracking;
            //            c.LotNumberTracking = Objitem.LotNumberTracking;
            //            c.DateCodeTracking = Objitem.DateCodeTracking;
            //            return c;
            //        }).ToList();

            //foreach (var iTemp in lstTemp)
            //{
            //    if (objData.FindIndex(x => x.ID == iTemp.ID) < 0)
            //        objData.Add(iTemp);
            //}
            ////</Added-by-AmitP>
            //if (string.IsNullOrEmpty(PullCreditType))
            //    PullCreditType = "credit";
            ItemLocationDetailsDAL obj = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);

            List<CreditHistory> lstCreditGuids = new List<CreditHistory>();
            long SessionUserId = SessionHelper.UserID;
            if (obj.ItemLocationDetailsSaveForCreditPullnew(objData, "controller", SessionHelper.RoomDateFormat, out lstCreditGuids, SessionUserId,SessionHelper.EnterPriceID, PullCreditType))
            //if (obj.ItemLocationDetailsSaveForCreditPull(objData, "controller", PullCreditType))
            {
                ////TODO: Added By CP
                //if (OrderDetailGUID != null)
                //{
                //    OrderDetailsDAL ordDetailCtrl = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                //    OrderDetailsDTO objOrdDetailDTO = ordDetailCtrl.GetRecord(OrderDetailGUID.Value, SessionHelper.RoomID, SessionHelper.CompanyID);
                //    objOrdDetailDTO.ReceivedQuantity = ReceivedQty;
                //    ordDetailCtrl.Edit(objOrdDetailDTO);
                //}
                //////TODO: END Added By CP

                if (PullMstViewDTOList != null && PullMstViewDTOList.Count > 0)
                {
                    PullMstViewDTOList = PullMstViewDTOList.Where(x => (x.CreditConsignedQuantity.GetValueOrDefault(0) + x.CreditCustomerOwnedQuantity.GetValueOrDefault(0)) > 0).ToList();

                    foreach (var item in PullMstViewDTOList)
                    {
                        item.LastUpdatedBy = SessionHelper.UserID;
                        item.Updated = DateTimeUtility.DateTimeNow;
                        item.WhatWhereAction = "Update Credit Qty";
                        pullMasterDAL.Edit(item);

                        if (item != null && item.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                        {
                            WorkOrderLineItemsDAL objWOLDAL = new WorkOrderLineItemsDAL(SessionHelper.EnterPriseDBName);
                            objWOLDAL.UpdateWOItemAndTotalCost(item.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty).ToString(), item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0));
                        }
                    }
                }

                if (oPullDetailsToUpdate != null && oPullDetailsToUpdate.Count > 0)
                {
                    foreach (var item in oPullDetailsToUpdate)
                    {
                        PullDetailsDAL oPullDetailsDAL = new PullDetailsDAL(SessionHelper.EnterPriseDBName);
                        item.LastUpdatedBy = SessionHelper.UserID;
                        item.Updated = DateTimeUtility.DateTimeNow;
                        oPullDetailsDAL.EditCreditQuantity(item);
                    }
                }

                #region "Update Ext Cost And Avg Cost"
                new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetAndUpdateExtCostAndAvgCost(objData[0].ItemGUID ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID);
                #endregion

                return Json(new { Message = "Success", Status = "OK" }, JsonRequestBehavior.AllowGet);

            }
            else
                return Json(new { Message = "Fail", Status = "Fail" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ItemLocationDetailsSaveForMSCredit(List<MaterialStagingPullDetailDTO> objData, string PullCreditType = "MS Credit")
        {
            string OrdStatusText = string.Empty;
            Guid? OrderDetailGUID = null;
            double ReceivedQty = 0;
            Guid? ItemGUID = null;
            List<PullMasterViewDTO> PullMstViewDTOList = null;
            List<PullDetailsDTO> oPullDetailsToUpdate = null;
            PullMasterDAL pullMasterDAL = null;

            if (Request != null && Request.UrlReferrer != null && (Request.UrlReferrer.ToString().Contains("/Pull/PullMasterList") || Request.UrlReferrer.ToString().Contains("/WorkOrder/WorkOrderList")))
            {
                objData = objData.Where(x => (x.CustomerOwnedQuantity.GetValueOrDefault(0) + x.ConsignedQuantity.GetValueOrDefault(0)) > 0).ToList();
                string errormsg = ResMessage.AddQuantityRequired;

                if (objData == null || objData.Count <= 0)
                {
                    return Json(new { Message = errormsg, Status = "Fail" }, JsonRequestBehavior.AllowGet);
                }

                pullMasterDAL = new PullMasterDAL(SessionHelper.EnterPriseDBName);
                PullMstViewDTOList = pullMasterDAL.GetPullByItemGuidAndPullCreditType(SessionHelper.RoomID, SessionHelper.CompanyID, objData[0].ItemGUID).ToList();
                List<MaterialStagingPullDetailDTO> oDataFinal = new List<MaterialStagingPullDetailDTO>();
                oPullDetailsToUpdate = new List<PullDetailsDTO>();
                string msg = GetItemLocationDetailDataForCreditPullMS(PullMstViewDTOList, objData, oDataFinal, oPullDetailsToUpdate);
                objData = oDataFinal.Where(x => x.ConsignedQuantity.GetValueOrDefault(0) > 0 || x.CustomerOwnedQuantity.GetValueOrDefault(0) > 0).ToList();

                if (msg != "ok")
                {
                    return Json(new { Message = msg, Status = "Fail" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
                IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetRequiredUDFsByUDFTableNamePlain("ReceivedOrderTransferDetail", SessionHelper.RoomID, SessionHelper.CompanyID);
                string udfRequier = string.Empty;
                foreach (var baseItem in objData)
                {
                    foreach (var i in DataFromDB)
                    {
                        if (i.UDFColumnName == "UDF1" && string.IsNullOrEmpty(baseItem.UDF1))
                        {
                            string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                            string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                            if (!string.IsNullOrEmpty(val))
                                i.UDFDisplayColumnName = val;
                            else
                                i.UDFDisplayColumnName = i.UDFColumnName;
                            udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                        }
                        else if (i.UDFColumnName == "UDF2" && string.IsNullOrEmpty(baseItem.UDF2))
                        {
                            string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                            string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                            if (!string.IsNullOrEmpty(val))
                                i.UDFDisplayColumnName = val;
                            else
                                i.UDFDisplayColumnName = i.UDFColumnName;
                            udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                        }
                        else if (i.UDFColumnName == "UDF3" && string.IsNullOrEmpty(baseItem.UDF3))
                        {
                            string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                            string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                            if (!string.IsNullOrEmpty(val))
                                i.UDFDisplayColumnName = val;
                            else
                                i.UDFDisplayColumnName = i.UDFColumnName;
                            udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                        }
                        else if (i.UDFColumnName == "UDF4" && string.IsNullOrEmpty(baseItem.UDF4))
                        {
                            string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                            string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                            if (!string.IsNullOrEmpty(val))
                                i.UDFDisplayColumnName = val;
                            else
                                i.UDFDisplayColumnName = i.UDFColumnName;
                            udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                        }
                        else if (i.UDFColumnName == "UDF5" && string.IsNullOrEmpty(baseItem.UDF5))
                        {
                            string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                            string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                            if (!string.IsNullOrEmpty(val))
                                i.UDFDisplayColumnName = val;
                            else
                                i.UDFDisplayColumnName = i.UDFColumnName;
                            udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                        }

                        if (!string.IsNullOrEmpty(udfRequier))
                            break;

                    }

                    if (!string.IsNullOrEmpty(udfRequier))
                        break;
                }

                if (!string.IsNullOrEmpty(udfRequier))
                {
                    return Json(new { Message = udfRequier, Status = "UDFError" }, JsonRequestBehavior.AllowGet);
                }
            }
            BinMasterDTO objBinMasterDTO = new BinMasterDTO();
            BinMasterDAL objItemLocationDetailsDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            foreach (var item in objData)
            {
                ItemGUID = item.ItemGUID;
                item.CompanyID = SessionHelper.CompanyID;
                item.Room = SessionHelper.RoomID;

                if (string.IsNullOrEmpty(item.BinNumber))
                {
                    item.BinNumber = "[|EmptyStagingBin|]";
                }

                if (!string.IsNullOrEmpty(item.BinNumber) && item.BinID.GetValueOrDefault(0) <= 0)
                {
                    objBinMasterDTO = objItemLocationDetailsDAL.GetItemBinPlain(item.ItemGUID, item.BinNumber, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, true);
                    item.BinID = objBinMasterDTO.ID;
                    //item.BinID = new CommonDAL(SessionHelper.EnterPriseDBName).GetOrInsertBinIDByName(item.ItemGUID, item.BinNumber, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, true);
                }

                if (item.ID == 0)
                {
                    item.Created = DateTimeUtility.DateTimeNow;
                    item.CreatedBy = SessionHelper.UserID;
                    item.CreatedByName = SessionHelper.UserName;
                }
                item.Updated = DateTimeUtility.DateTimeNow;
                item.LastUpdatedBy = SessionHelper.UserID;
                item.UpdatedByName = SessionHelper.UserName;

                //TODO: Added By CP
                if (item.OrderDetailGUID != null)
                {
                    OrderDetailGUID = item.OrderDetailGUID;

                    if (!item.SerialNumberTracking)
                    {
                        if (item.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                        {
                            ReceivedQty += double.Parse(item.CustomerOwnedQuantity.ToString());
                        }
                        else if (item.ConsignedQuantity.GetValueOrDefault(0) > 0)
                        {
                            ReceivedQty += double.Parse(item.ConsignedQuantity.ToString());
                        }
                        else
                        {
                            ReceivedQty += 0;
                        }
                    }
                    else
                    {

                        if (item.ID <= 0 && !string.IsNullOrEmpty(item.SerialNumber) && item.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                        {
                            ReceivedQty += double.Parse(item.CustomerOwnedQuantity.ToString());
                        }
                        else if (item.ID <= 0 && !string.IsNullOrEmpty(item.SerialNumber) && item.ConsignedQuantity.GetValueOrDefault(0) > 0)
                        {
                            ReceivedQty += double.Parse(item.ConsignedQuantity.ToString());
                        }
                        else
                        {
                            ReceivedQty += 0;
                        }

                    }
                }
                //End: Added By CP
            }

            if (string.IsNullOrEmpty(PullCreditType))
                PullCreditType = "MS Credit";
            ItemLocationDetailsDAL obj = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            long SessionUserId = SessionHelper.UserID;
            if (obj.ItemLocationDetailsSaveForMSCredit(objData, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, RoomDateFormat, SessionUserId,SessionHelper.EnterPriceID, PullCreditType))
            {
                //TODO: Added By CP
                if (OrderDetailGUID != null)
                {
                    OrderDetailsDAL ordDetailCtrl = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                    OrderDetailsDTO objOrdDetailDTO = ordDetailCtrl.GetOrderDetailByGuidPlain(OrderDetailGUID.Value, SessionHelper.RoomID, SessionHelper.CompanyID);
                    objOrdDetailDTO.ReceivedQuantity = objOrdDetailDTO.ReceivedQuantity.GetValueOrDefault(0) + ReceivedQty;
                    ordDetailCtrl.Edit(objOrdDetailDTO, SessionUserId,SessionHelper.EnterPriceID);

                    ordDetailCtrl.UpdateOrderStatusByReceive(objOrdDetailDTO.OrderGUID.GetValueOrDefault(Guid.Empty), objOrdDetailDTO.Room.GetValueOrDefault(0), objOrdDetailDTO.CompanyID.GetValueOrDefault(0), objOrdDetailDTO.LastUpdatedBy.GetValueOrDefault(0));

                    OrderMasterDAL objOrdDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                    OrderMasterDTO order = objOrdDAL.GetOrderByGuidPlain(objOrdDetailDTO.OrderGUID.GetValueOrDefault(Guid.Empty));

                    if (order != null)
                    {
                        OrdStatusText = order.OrderStatusText;
                    }
                }
                ////TODO: END Added By CP

                if (PullMstViewDTOList != null && PullMstViewDTOList.Count > 0)
                {
                    PullMstViewDTOList = PullMstViewDTOList.Where(x => (x.CreditConsignedQuantity.GetValueOrDefault(0) + x.CreditCustomerOwnedQuantity.GetValueOrDefault(0)) > 0).ToList();

                    foreach (var item in PullMstViewDTOList)
                    {
                        item.LastUpdatedBy = SessionHelper.UserID;
                        item.Updated = DateTimeUtility.DateTimeNow;
                        item.WhatWhereAction = "Update Credit Qty";
                        pullMasterDAL.Edit(item);

                        if (item != null && item.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                        {
                            WorkOrderLineItemsDAL objWOLDAL = new WorkOrderLineItemsDAL(SessionHelper.EnterPriseDBName);
                            objWOLDAL.UpdateWOItemAndTotalCost(item.WorkOrderDetailGUID.GetValueOrDefault(Guid.Empty).ToString(), item.Room.GetValueOrDefault(0), item.CompanyID.GetValueOrDefault(0));
                        }
                    }
                }

                if (oPullDetailsToUpdate != null && oPullDetailsToUpdate.Count > 0)
                {
                    foreach (var item in oPullDetailsToUpdate)
                    {
                        PullDetailsDAL oPullDetailsDAL = new PullDetailsDAL(SessionHelper.EnterPriseDBName);
                        item.LastUpdatedBy = SessionHelper.UserID;
                        item.Updated = DateTimeUtility.DateTimeNow;
                        oPullDetailsDAL.EditCreditQuantity(item);
                    }
                }

                return Json(new { Message = "Success", Status = "OK", OrderStatusText = "" }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { Message = "Fail", Status = "Fail", OrderStatusText = "" }, JsonRequestBehavior.AllowGet);
        }

        ///// <summary>
        ///// ItemLocationDetailsSaveForKitCredit
        ///// </summary>
        ///// <param name="objData"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public JsonResult ItemLocationDetailsSaveForKitCredit(List<ItemLocationDetailsDTO> objData)
        //{
        //    string message = "Fail";
        //    string status = "Fail";

        //    Guid KitDetailGUID = Guid.Empty;
        //    Guid? ItemGuid = Guid.Empty;
        //    double CustomerQty = 0;
        //    double ConsignedQty = 0;
        //    double TotalQty = 0;

        //    List<KitMoveInOutDetailDTO> objListKitMoveInoutDTO = new List<KitMoveInOutDetailDTO>();
        //    KitDetailDAL objKitDetailCtrl = new KitDetailDAL(SessionHelper.EnterPriseDBName);
        //    KitDetailDTO objKitDetailDTO = null;

        //    KitMoveInOutDetailDAL objKitMoveInOutCtrl = new KitMoveInOutDetailDAL(SessionHelper.EnterPriseDBName);
        //    CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
        //    ItemLocationDetailsDAL objItemLocationDetailsDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
        //    BinMasterDTO objBinMasterDTO;
        //    foreach (var item in objData)
        //    {

        //        CustomerQty = 0;
        //        ConsignedQty = 0;

        //        item.CompanyID = SessionHelper.CompanyID;
        //        item.Room = SessionHelper.RoomID;
        //        if (!string.IsNullOrEmpty(item.BinNumber) && item.BinID.GetValueOrDefault(0) <= 0)
        //        {
        //            objBinMasterDTO = new BinMasterDTO();
        //            objBinMasterDTO = objItemLocationDetailsDAL.GetItemBin(item.ItemGUID.GetValueOrDefault(Guid.Empty), item.BinNumber, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, false);
        //            item.BinID = objBinMasterDTO.ID;
        //            //item.BinID = objCommonDAL.GetOrInsertBinIDByName(item.ItemGUID.GetValueOrDefault(Guid.Empty), item.BinNumber, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, false);
        //        }
        //        if (item.ID == 0)
        //        {
        //            item.Created = DateTimeUtility.DateTimeNow;
        //            item.CreatedBy = SessionHelper.UserID;
        //            item.CreatedByName = SessionHelper.UserName;
        //        }
        //        item.Updated = DateTimeUtility.DateTimeNow;
        //        item.LastUpdatedBy = SessionHelper.UserID;
        //        item.UpdatedByName = SessionHelper.UserName;

        //        item.ItemGUID = item.ItemGUID;
        //        ItemGuid = item.ItemGUID;
        //        //TODO: Added By CP
        //        if (item.KitDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
        //        {
        //            KitDetailGUID = item.KitDetailGUID.GetValueOrDefault(Guid.Empty);

        //            if (objKitDetailDTO == null)
        //            {
        //                objKitDetailDTO = objKitDetailCtrl.GetRecord(KitDetailGUID.ToString(), SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false);
        //            }

        //            if (!item.SerialNumberTracking)
        //            {
        //                if (item.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
        //                {
        //                    CustomerQty += double.Parse(item.CustomerOwnedQuantity.ToString());
        //                }

        //                else if (item.ConsignedQuantity.GetValueOrDefault(0) > 0)
        //                {
        //                    ConsignedQty += double.Parse(item.ConsignedQuantity.ToString());
        //                }
        //                else
        //                {
        //                    CustomerQty += 0;
        //                    ConsignedQty += 0;
        //                }
        //            }
        //            else
        //            {
        //                if (!string.IsNullOrEmpty(item.SerialNumber) && item.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
        //                {
        //                    CustomerQty += double.Parse(item.CustomerOwnedQuantity.ToString());
        //                }
        //                else if (!string.IsNullOrEmpty(item.SerialNumber) && item.ConsignedQuantity.GetValueOrDefault(0) > 0)
        //                {
        //                    ConsignedQty += double.Parse(item.ConsignedQuantity.ToString());
        //                }
        //                else
        //                {
        //                    CustomerQty += 0;
        //                    ConsignedQty += 0;
        //                }

        //            }
        //            if (CustomerQty + ConsignedQty > 0)
        //            {
        //                KitMoveInOutDetailDTO KitMoveInOutDTO = new KitMoveInOutDetailDTO()
        //                {
        //                    BinID = item.BinID,
        //                    CompanyID = SessionHelper.CompanyID,
        //                    ConsignedQuantity = ConsignedQty,
        //                    Created = DateTimeUtility.DateTimeNow,
        //                    CreatedBy = SessionHelper.UserID,
        //                    CreatedByName = SessionHelper.UserName,
        //                    CustomerOwnedQuantity = CustomerQty,
        //                    GUID = Guid.Empty,
        //                    ID = 0,
        //                    IsArchived = false,
        //                    IsDeleted = false,
        //                    ItemGUID = item.ItemGUID,
        //                    KitDetailGUID = objKitDetailDTO.GUID,
        //                    LastUpdatedBy = SessionHelper.UserID,
        //                    MoveInOut = "OUT",
        //                    Room = SessionHelper.RoomID,
        //                    RoomName = SessionHelper.RoomName,
        //                    TotalQuantity = CustomerQty + ConsignedQty,
        //                    Updated = DateTimeUtility.DateTimeNow,
        //                    UpdatedByName = SessionHelper.UserName

        //                };

        //                objListKitMoveInoutDTO.Add(KitMoveInOutDTO);
        //            }
        //        }
        //        //End: Added By CP
        //    }


        //    ItemLocationDetailsDAL obj = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
        //    if (KitDetailGUID != Guid.Empty && objListKitMoveInoutDTO != null && objListKitMoveInoutDTO.Count > 0 && objKitDetailDTO != null)
        //    {
        //        TotalQty = objListKitMoveInoutDTO.Sum(x => x.TotalQuantity);
        //        if (TotalQty <= objKitDetailDTO.AvailableItemsInWIP)
        //        {

        //            //<Added-by-AmitP> Get Previous Records, which are stored...
        //            ItemLocationDetailsDAL objAPI = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
        //            List<ItemLocationDetailsDTO> lstTemp = objAPI.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, ItemGuid.Value, null, "ID DESC").ToList();

        //            ItemMasterDAL objItemAPI = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
        //            var Objitem = objItemAPI.GetRecord(ItemGuid.Value.ToString(), SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

        //            lstTemp.Select(c =>
        //            {
        //                c.SerialNumberTracking = Objitem.SerialNumberTracking;
        //                c.LotNumberTracking = Objitem.LotNumberTracking;
        //                c.DateCodeTracking = Objitem.DateCodeTracking;
        //                return c;
        //            }).ToList();

        //            foreach (var iTemp in lstTemp)
        //            {
        //                if (objData.FindIndex(x => x.ID == iTemp.ID) < 0)
        //                    objData.Add(iTemp);
        //            }
        //            //</Added-by-AmitP>

        //            if (obj.ItemLocationDetailsSaveForCreditKit(objData))
        //            {
        //                objKitDetailDTO.AvailableItemsInWIP = objKitDetailDTO.AvailableItemsInWIP - TotalQty;
        //                objKitDetailDTO.UpdatedByName = SessionHelper.UserName;
        //                objKitDetailDTO.LastUpdated = DateTimeUtility.DateTimeNow;
        //                objKitDetailDTO.LastUpdatedBy = SessionHelper.UserID;
        //                objKitDetailCtrl.Edit(objKitDetailDTO);

        //                foreach (var item in objListKitMoveInoutDTO)
        //                {
        //                    objKitMoveInOutCtrl.Insert(item);
        //                }

        //                message = "Success";
        //                status = "OK";
        //            }
        //            else
        //                message = "Not updated.";
        //        }
        //        else
        //            message = "Move out quantity is less than or equal to Item Available WIP Quantity.";

        //    }


        //    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// GetItemLocationDetailDataForCreditPull
        /// </summary>
        /// <param name="objData"></param>
        /// <returns></returns>
        private string GetItemLocationDetailDataForCreditPull(List<PullMasterViewDTO> objPullMstViewDTOList, List<ItemLocationDetailsDTO> objData, List<ItemLocationDetailsDTO> oDataFinal, List<PullDetailsDTO> oPullDetailsToUpdate)
        {
            ItemMasterDAL itemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            ItemMasterDTO objItemDTO = itemDAL.GetItemWithoutJoins(null, objData[0].ItemGUID);
            double totalItemQty = 0;
            if (objItemDTO.SerialNumberTracking)
            {
                totalItemQty = objData.Where(y => !string.IsNullOrEmpty(y.SerialNumber) && !string.IsNullOrEmpty(y.Received)).Sum(x => (x.ConsignedQuantity.GetValueOrDefault(0) + x.CustomerOwnedQuantity.GetValueOrDefault(0)));
            }
            else
            {
                totalItemQty = objData.Sum(x => (x.ConsignedQuantity.GetValueOrDefault(0) + x.CustomerOwnedQuantity.GetValueOrDefault(0)));
            }

            double totalAllCreditQty = objPullMstViewDTOList.Sum(x => (x.CreditConsignedQuantity.GetValueOrDefault(0))) + objPullMstViewDTOList.Sum(x => (x.CreditCustomerOwnedQuantity.GetValueOrDefault(0)));
            double totalPullQty = objPullMstViewDTOList.Sum(x => (x.PoolQuantity.GetValueOrDefault(0))) - totalAllCreditQty;
            if (totalPullQty < totalItemQty)
                return ResPullMaster.CreditQtyGreaterThanTotalPullQty;

            foreach (var item in objData)
            {
                double itmCustQty = item.CustomerOwnedQuantity.GetValueOrDefault(0);
                double itmConsQty = item.ConsignedQuantity.GetValueOrDefault(0);

                //double itmNewCustQty = 0;
                //double itmNewConsQty = 0;

                int costUOMValue = 1;
                if (objItemDTO != null)
                {
                    CostUOMMasterDTO oCostUOMMasterDTO = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName).GetCostUOMByID(objItemDTO.CostUOMID.GetValueOrDefault(0));
                    if (oCostUOMMasterDTO != null)
                    {
                        costUOMValue = oCostUOMMasterDTO.CostUOMValue.GetValueOrDefault(0);
                        if (costUOMValue == 0)
                            costUOMValue = 1;
                    }
                }

                foreach (var PullItem in objPullMstViewDTOList)
                {
                    double pullCustQty = PullItem.CustomerOwnedQuantity.GetValueOrDefault(0);
                    double pullConsQty = PullItem.ConsignedQuantity.GetValueOrDefault(0);
                    double creditPullCustQty = PullItem.CreditCustomerOwnedQuantity.GetValueOrDefault(0);
                    double creditPullConsQty = PullItem.CreditConsignedQuantity.GetValueOrDefault(0);

                    totalPullQty = PullItem.PoolQuantity.GetValueOrDefault(0);
                    double totalCreditQty = creditPullCustQty + creditPullConsQty;

                    if (totalPullQty > totalCreditQty)
                    {
                        List<PullDetailsDTO> oPullDetailsLst = new PullDetailsDAL(SessionHelper.EnterPriseDBName)
                            .GetPullDetailsHavingPendingCredit(PullItem.GUID, SessionHelper.RoomID, SessionHelper.CompanyID)
                            .OrderByDescending(x => x.ID).ToList();

                        double pendingCreditCustQty = pullCustQty - creditPullCustQty;
                        double pendingCreditConsQty = pullConsQty - creditPullConsQty;

                        if (objItemDTO.Consignment && itmConsQty > 0)
                        {
                            if (pendingCreditConsQty > 0 && itmConsQty > 0)
                            {
                                double reduceQuantity = 0;
                                if (pendingCreditConsQty > itmConsQty)
                                    reduceQuantity = itmConsQty;
                                else if (pendingCreditConsQty <= itmConsQty)
                                    reduceQuantity = pendingCreditConsQty;

                                if (oPullDetailsLst != null && oPullDetailsLst.Count() > 0)
                                {
                                    foreach (var oPD in oPullDetailsLst)
                                    {
                                        double pendingCreditConsQty_Detail = oPD.ConsignedQuantity.GetValueOrDefault(0) - oPD.CreditConsignedQuantity.GetValueOrDefault(0);
                                        if (pendingCreditConsQty_Detail > 0)
                                        {
                                            ItemLocationDetailsDTO oILD = new ItemLocationDetailsDTO();
                                            if (pendingCreditConsQty_Detail >= reduceQuantity)
                                            {
                                                creditPullConsQty += reduceQuantity;
                                                oILD.ConsignedQuantity = reduceQuantity;
                                                itmConsQty -= reduceQuantity;
                                                reduceQuantity = 0;
                                            }
                                            else
                                            {
                                                creditPullConsQty += pendingCreditConsQty_Detail;
                                                oILD.ConsignedQuantity = pendingCreditConsQty_Detail;
                                                itmConsQty -= pendingCreditConsQty_Detail;
                                                reduceQuantity -= pendingCreditConsQty_Detail;
                                            }

                                            oILD.Cost = oPD.ItemCost.GetValueOrDefault(0) * costUOMValue;
                                            oILD.InitialQuantity = oILD.ConsignedQuantity;
                                            oILD.InitialQuantityWeb = oILD.ConsignedQuantity;
                                            new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).CopyItemLocationDetailsObjects(item, oILD);
                                            oDataFinal.Add(oILD);

                                            oPD.CreditConsignedQuantity = oPD.CreditConsignedQuantity.GetValueOrDefault(0) + oILD.ConsignedQuantity;
                                            oPullDetailsToUpdate.Add(oPD);
                                            if (reduceQuantity == 0)
                                                break;
                                        }
                                    }
                                }
                            }

                            if (pendingCreditCustQty > 0 && itmConsQty > 0)
                            {
                                double reduceQuantity = 0;
                                if (pendingCreditCustQty > itmConsQty)
                                    reduceQuantity = itmConsQty;
                                else if (pendingCreditCustQty <= itmConsQty)
                                    reduceQuantity = pendingCreditCustQty;

                                if (oPullDetailsLst != null && oPullDetailsLst.Count() > 0)
                                {
                                    foreach (var oPD in oPullDetailsLst)
                                    {
                                        double pendingCreditCustQty_Detail = oPD.CustomerOwnedQuantity.GetValueOrDefault(0) - oPD.CreditCustomerOwnedQuantity.GetValueOrDefault(0);
                                        if (pendingCreditCustQty_Detail > 0)
                                        {
                                            ItemLocationDetailsDTO oILD = new ItemLocationDetailsDTO();
                                            if (pendingCreditCustQty_Detail >= reduceQuantity)
                                            {
                                                creditPullCustQty += reduceQuantity;
                                                oILD.CustomerOwnedQuantity = reduceQuantity;
                                                itmConsQty -= reduceQuantity;
                                                reduceQuantity = 0;
                                            }
                                            else
                                            {
                                                creditPullCustQty += pendingCreditCustQty_Detail;
                                                oILD.CustomerOwnedQuantity = pendingCreditCustQty_Detail;
                                                itmConsQty -= pendingCreditCustQty_Detail;
                                                reduceQuantity -= pendingCreditCustQty_Detail;
                                            }

                                            oILD.Cost = oPD.ItemCost.GetValueOrDefault(0) * costUOMValue;
                                            oILD.InitialQuantity = oILD.CustomerOwnedQuantity;
                                            oILD.InitialQuantityWeb = oILD.CustomerOwnedQuantity;
                                            new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).CopyItemLocationDetailsObjects(item, oILD);
                                            oDataFinal.Add(oILD);

                                            oPD.CreditCustomerOwnedQuantity = oPD.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + oILD.CustomerOwnedQuantity;
                                            oPullDetailsToUpdate.Add(oPD);
                                            if (reduceQuantity == 0)
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                        else if (!objItemDTO.Consignment && itmCustQty > 0)
                        {
                            if (pendingCreditConsQty > 0 && itmCustQty > 0)
                            {
                                double reduceQuantity = 0;
                                if (pendingCreditConsQty > itmCustQty)
                                    reduceQuantity = itmCustQty;
                                else if (pendingCreditConsQty <= itmCustQty)
                                    reduceQuantity = pendingCreditConsQty;

                                if (oPullDetailsLst != null && oPullDetailsLst.Count() > 0)
                                {
                                    foreach (var oPD in oPullDetailsLst)
                                    {
                                        double pendingCreditConsQty_Detail = oPD.ConsignedQuantity.GetValueOrDefault(0) - oPD.CreditConsignedQuantity.GetValueOrDefault(0);
                                        if (pendingCreditConsQty_Detail > 0)
                                        {
                                            ItemLocationDetailsDTO oILD = new ItemLocationDetailsDTO();
                                            if (pendingCreditConsQty_Detail >= reduceQuantity)
                                            {
                                                creditPullConsQty += reduceQuantity;
                                                oILD.ConsignedQuantity = reduceQuantity;
                                                itmCustQty -= reduceQuantity;
                                                reduceQuantity = 0;
                                            }
                                            else
                                            {
                                                creditPullConsQty += pendingCreditConsQty_Detail;
                                                oILD.ConsignedQuantity = pendingCreditConsQty_Detail;
                                                itmCustQty -= pendingCreditConsQty_Detail;
                                                reduceQuantity -= pendingCreditConsQty_Detail;
                                            }

                                            oILD.Cost = oPD.ItemCost.GetValueOrDefault(0) * costUOMValue;
                                            oILD.InitialQuantity = oILD.ConsignedQuantity;
                                            oILD.InitialQuantityWeb = oILD.ConsignedQuantity;
                                            new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).CopyItemLocationDetailsObjects(item, oILD);
                                            oDataFinal.Add(oILD);

                                            oPD.CreditConsignedQuantity = oPD.CreditConsignedQuantity.GetValueOrDefault(0) + oILD.ConsignedQuantity;
                                            oPullDetailsToUpdate.Add(oPD);
                                            if (reduceQuantity == 0)
                                                break;
                                        }
                                    }
                                }
                            }

                            if (pendingCreditCustQty > 0 && itmCustQty > 0)
                            {
                                double reduceQuantity = 0;
                                if (pendingCreditCustQty > itmCustQty)
                                    reduceQuantity = itmCustQty;
                                else if (pendingCreditCustQty <= itmCustQty)
                                    reduceQuantity = pendingCreditCustQty;

                                if (oPullDetailsLst != null && oPullDetailsLst.Count() > 0)
                                {
                                    foreach (var oPD in oPullDetailsLst)
                                    {
                                        double pendingCreditCustQty_Detail = oPD.CustomerOwnedQuantity.GetValueOrDefault(0) - oPD.CreditCustomerOwnedQuantity.GetValueOrDefault(0);
                                        if (pendingCreditCustQty_Detail > 0)
                                        {
                                            ItemLocationDetailsDTO oILD = new ItemLocationDetailsDTO();
                                            if (pendingCreditCustQty_Detail >= reduceQuantity)
                                            {
                                                creditPullCustQty += reduceQuantity;
                                                oILD.CustomerOwnedQuantity = reduceQuantity;
                                                itmCustQty -= reduceQuantity;
                                                reduceQuantity = 0;
                                            }
                                            else
                                            {
                                                creditPullCustQty += pendingCreditCustQty_Detail;
                                                oILD.CustomerOwnedQuantity = pendingCreditCustQty_Detail;
                                                itmCustQty -= pendingCreditCustQty_Detail;
                                                reduceQuantity -= pendingCreditCustQty_Detail;
                                            }

                                            oILD.Cost = oPD.ItemCost.GetValueOrDefault(0) * costUOMValue;
                                            oILD.InitialQuantity = oILD.CustomerOwnedQuantity;
                                            oILD.InitialQuantityWeb = oILD.CustomerOwnedQuantity;
                                            new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).CopyItemLocationDetailsObjects(item, oILD);
                                            oDataFinal.Add(oILD);

                                            oPD.CreditCustomerOwnedQuantity = oPD.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + oILD.CustomerOwnedQuantity;
                                            oPullDetailsToUpdate.Add(oPD);

                                            if (reduceQuantity == 0)
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            break;
                        }

                        PullItem.CreditConsignedQuantity = creditPullConsQty;
                        PullItem.CreditCustomerOwnedQuantity = creditPullCustQty;
                    }
                } // Pull Item loop

            } // Item Loop

            return "ok";
        }

        /// <summary>
        /// GetItemLocationDetailDataForCreditPullMs
        /// </summary>
        /// <param name="objData"></param>
        /// <returns></returns>
        private string GetItemLocationDetailDataForCreditPullMS(List<PullMasterViewDTO> objPullMstViewDTOList, List<MaterialStagingPullDetailDTO> objData, List<MaterialStagingPullDetailDTO> oDataFinal, List<PullDetailsDTO> oPullDetailsToUpdate)
        {

            ItemMasterDAL itemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            ItemMasterDTO objItemDTO = itemDAL.GetItemWithoutJoins(null, objData[0].ItemGUID);

            double totalItemQty = 0;
            if (objItemDTO.SerialNumberTracking)
            {
                totalItemQty = objData.Where(y => !string.IsNullOrEmpty(y.SerialNumber) && !string.IsNullOrEmpty(y.Received)).Sum(x => (x.ConsignedQuantity.GetValueOrDefault(0) + x.CustomerOwnedQuantity.GetValueOrDefault(0)));
            }
            else
            {
                totalItemQty = objData.Sum(x => (x.ConsignedQuantity.GetValueOrDefault(0) + x.CustomerOwnedQuantity.GetValueOrDefault(0)));
            }

            double totalAllCreditQty = objPullMstViewDTOList.Sum(x => (x.CreditConsignedQuantity.GetValueOrDefault(0))) + objPullMstViewDTOList.Sum(x => (x.CreditCustomerOwnedQuantity.GetValueOrDefault(0)));
            double totalPullQty = objPullMstViewDTOList.Sum(x => (x.PoolQuantity.GetValueOrDefault(0))) - totalAllCreditQty;
            if (totalPullQty < totalItemQty)
                return ResPullMaster.CreditQtyGreaterThanTotalPullQty;


            foreach (var item in objData)
            {
                double itmCustQty = item.CustomerOwnedQuantity.GetValueOrDefault(0);
                double itmConsQty = item.ConsignedQuantity.GetValueOrDefault(0);

                foreach (var PullItem in objPullMstViewDTOList)
                {
                    double pullCustQty = PullItem.CustomerOwnedQuantity.GetValueOrDefault(0);
                    double pullConsQty = PullItem.ConsignedQuantity.GetValueOrDefault(0);
                    double creditPullCustQty = PullItem.CreditCustomerOwnedQuantity.GetValueOrDefault(0);
                    double creditPullConsQty = PullItem.CreditConsignedQuantity.GetValueOrDefault(0);

                    totalPullQty = PullItem.PoolQuantity.GetValueOrDefault(0);
                    double totalCreditQty = creditPullCustQty + creditPullConsQty;

                    if (totalPullQty > totalCreditQty)
                    {
                        List<PullDetailsDTO> oPullDetailsLst = new PullDetailsDAL(SessionHelper.EnterPriseDBName)
                                             .GetPullDetailsHavingPendingCredit(PullItem.GUID, SessionHelper.RoomID, SessionHelper.CompanyID)
                                             .OrderByDescending(x => x.ID).ToList();

                        double pendingCreditCustQty = pullCustQty - creditPullCustQty;
                        double pendingCreditConsQty = pullConsQty - creditPullConsQty;

                        if (objItemDTO.Consignment && itmConsQty > 0)
                        {
                            if (pendingCreditConsQty > 0 && itmConsQty > 0)
                            {
                                double reduceQuantity = 0;
                                if (pendingCreditConsQty > itmConsQty)
                                    reduceQuantity = itmConsQty;
                                else if (pendingCreditConsQty <= itmConsQty)
                                    reduceQuantity = pendingCreditConsQty;

                                if (oPullDetailsLst != null && oPullDetailsLst.Count() > 0)
                                {
                                    foreach (var oPD in oPullDetailsLst)
                                    {
                                        double pendingCreditConsQty_Detail = oPD.ConsignedQuantity.GetValueOrDefault(0) - oPD.CreditConsignedQuantity.GetValueOrDefault(0);
                                        if (pendingCreditConsQty_Detail > 0)
                                        {
                                            MaterialStagingPullDetailDTO oILD = new MaterialStagingPullDetailDTO();
                                            if (pendingCreditConsQty_Detail >= reduceQuantity)
                                            {
                                                creditPullConsQty += reduceQuantity;
                                                oILD.ConsignedQuantity = reduceQuantity;
                                                itmConsQty -= reduceQuantity;
                                                reduceQuantity = 0;
                                            }
                                            else
                                            {
                                                creditPullConsQty += pendingCreditConsQty_Detail;
                                                oILD.ConsignedQuantity = pendingCreditConsQty_Detail;
                                                itmConsQty -= pendingCreditConsQty_Detail;
                                                reduceQuantity -= pendingCreditConsQty_Detail;
                                            }

                                            oILD.ItemCost = oPD.ItemCost;
                                            new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).CopyItemLocationDetailsObjectsMS(item, oILD);
                                            oDataFinal.Add(oILD);

                                            oPD.CreditConsignedQuantity = oPD.CreditConsignedQuantity.GetValueOrDefault(0) + oILD.ConsignedQuantity;
                                            oPullDetailsToUpdate.Add(oPD);
                                            if (reduceQuantity == 0)
                                                break;
                                        }
                                    }
                                }
                            }

                            if (pendingCreditCustQty > 0 && itmConsQty > 0)
                            {
                                double reduceQuantity = 0;
                                if (pendingCreditCustQty > itmConsQty)
                                    reduceQuantity = itmConsQty;
                                else if (pendingCreditCustQty <= itmConsQty)
                                    reduceQuantity = pendingCreditCustQty;

                                if (oPullDetailsLst != null && oPullDetailsLst.Count() > 0)
                                {
                                    foreach (var oPD in oPullDetailsLst)
                                    {
                                        double pendingCreditCustQty_Detail = oPD.CustomerOwnedQuantity.GetValueOrDefault(0) - oPD.CreditCustomerOwnedQuantity.GetValueOrDefault(0);
                                        if (pendingCreditCustQty_Detail > 0)
                                        {
                                            MaterialStagingPullDetailDTO oILD = new MaterialStagingPullDetailDTO();
                                            if (pendingCreditCustQty_Detail >= reduceQuantity)
                                            {
                                                creditPullCustQty += reduceQuantity;
                                                oILD.CustomerOwnedQuantity = reduceQuantity;
                                                itmConsQty -= reduceQuantity;
                                                reduceQuantity = 0;
                                            }
                                            else
                                            {
                                                creditPullCustQty += pendingCreditCustQty_Detail;
                                                oILD.CustomerOwnedQuantity = pendingCreditCustQty_Detail;
                                                itmConsQty -= pendingCreditCustQty_Detail;
                                                reduceQuantity -= pendingCreditCustQty_Detail;
                                            }

                                            oILD.ItemCost = oPD.ItemCost;
                                            new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).CopyItemLocationDetailsObjectsMS(item, oILD);
                                            oDataFinal.Add(oILD);

                                            oPD.CreditCustomerOwnedQuantity = oPD.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + oILD.CustomerOwnedQuantity;
                                            oPullDetailsToUpdate.Add(oPD);
                                            if (reduceQuantity == 0)
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                        else if (!objItemDTO.Consignment && itmCustQty > 0)
                        {
                            if (pendingCreditConsQty > 0 && itmCustQty > 0)
                            {
                                double reduceQuantity = 0;
                                if (pendingCreditConsQty > itmCustQty)
                                    reduceQuantity = itmCustQty;
                                else if (pendingCreditConsQty <= itmCustQty)
                                    reduceQuantity = pendingCreditConsQty;

                                if (oPullDetailsLst != null && oPullDetailsLst.Count() > 0)
                                {
                                    foreach (var oPD in oPullDetailsLst)
                                    {
                                        double pendingCreditConsQty_Detail = oPD.ConsignedQuantity.GetValueOrDefault(0) - oPD.CreditConsignedQuantity.GetValueOrDefault(0);
                                        if (pendingCreditConsQty_Detail > 0)
                                        {
                                            MaterialStagingPullDetailDTO oILD = new MaterialStagingPullDetailDTO();
                                            if (pendingCreditConsQty_Detail >= reduceQuantity)
                                            {
                                                creditPullConsQty += reduceQuantity;
                                                oILD.ConsignedQuantity = reduceQuantity;
                                                itmCustQty -= reduceQuantity;
                                                reduceQuantity = 0;
                                            }
                                            else
                                            {
                                                creditPullConsQty += pendingCreditConsQty_Detail;
                                                oILD.ConsignedQuantity = pendingCreditConsQty_Detail;
                                                itmCustQty -= pendingCreditConsQty_Detail;
                                                reduceQuantity -= pendingCreditConsQty_Detail;
                                            }

                                            oILD.ItemCost = oPD.ItemCost;
                                            new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).CopyItemLocationDetailsObjectsMS(item, oILD);
                                            oDataFinal.Add(oILD);

                                            oPD.CreditConsignedQuantity = oPD.CreditConsignedQuantity.GetValueOrDefault(0) + oILD.ConsignedQuantity;
                                            oPullDetailsToUpdate.Add(oPD);
                                            if (reduceQuantity == 0)
                                                break;
                                        }
                                    }
                                }
                            }

                            if (pendingCreditCustQty > 0 && itmCustQty > 0)
                            {
                                double reduceQuantity = 0;
                                if (pendingCreditCustQty > itmCustQty)
                                    reduceQuantity = itmCustQty;
                                else if (pendingCreditCustQty <= itmCustQty)
                                    reduceQuantity = pendingCreditCustQty;

                                if (oPullDetailsLst != null && oPullDetailsLst.Count() > 0)
                                {
                                    foreach (var oPD in oPullDetailsLst)
                                    {
                                        double pendingCreditCustQty_Detail = oPD.CustomerOwnedQuantity.GetValueOrDefault(0) - oPD.CreditCustomerOwnedQuantity.GetValueOrDefault(0);
                                        if (pendingCreditCustQty_Detail > 0)
                                        {
                                            MaterialStagingPullDetailDTO oILD = new MaterialStagingPullDetailDTO();
                                            if (pendingCreditCustQty_Detail >= reduceQuantity)
                                            {
                                                creditPullCustQty += reduceQuantity;
                                                oILD.CustomerOwnedQuantity = reduceQuantity;
                                                itmCustQty -= reduceQuantity;
                                                reduceQuantity = 0;
                                            }
                                            else
                                            {
                                                creditPullCustQty += pendingCreditCustQty_Detail;
                                                oILD.CustomerOwnedQuantity = pendingCreditCustQty_Detail;
                                                itmCustQty -= pendingCreditCustQty_Detail;
                                                reduceQuantity -= pendingCreditCustQty_Detail;
                                            }

                                            oILD.ItemCost = oPD.ItemCost;
                                            new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).CopyItemLocationDetailsObjectsMS(item, oILD);
                                            oDataFinal.Add(oILD);

                                            oPD.CreditCustomerOwnedQuantity = oPD.CreditCustomerOwnedQuantity.GetValueOrDefault(0) + oILD.CustomerOwnedQuantity;
                                            oPullDetailsToUpdate.Add(oPD);

                                            if (reduceQuantity == 0)
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            break;
                        }

                        PullItem.CreditConsignedQuantity = creditPullConsQty;
                        PullItem.CreditCustomerOwnedQuantity = creditPullCustQty;
                    }
                } // Pull Item loop

            } // Item Loop

            return "ok";
        }

        #region Ajax Data Provider

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult ItemMasterListAjax(QuickListJQueryDataTableParamModel param)
        {
            Session["NSItemLocation"] = null;
            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            //int PageIndex = int.Parse(param.sEcho);
            //int PageSize = param.iDisplayLength;
            //var sortDirection = Request["sSortDir_0"];
            //var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            //var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            //var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            //var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            //string sortColumnName = string.Empty;
            //string sDirection = string.Empty;
            //int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            string sortColumnName = Request["SortingField"].ToString();
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());

            // set the default column sorting here, if first time then required to set 
            if (string.IsNullOrEmpty(sortColumnName) || sortColumnName == "undefine" || sortColumnName == "0" || sortColumnName.Trim().Contains("null") || string.IsNullOrEmpty(sortColumnName.Replace("asc", "").Replace("null", "").Replace("desc", "").Trim()))
                sortColumnName = "ID asc";

            //string searchQuery = string.Empty;
            int TotalRecordCount = 0;
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);

            string sSearch = WebUtility.HtmlDecode(param.sSearch);

            //if (string.IsNullOrWhiteSpace(sSearch) && param.IsNarroSearchClear == false)
            //{
            //    using (UserNarrowSearchSettingsBAL searchSettingsBAL = new UserNarrowSearchSettingsBAL())
            //    {
            //        sSearch = searchSettingsBAL.GetSavedSearchStr(NarrowSearchSaveListEnum.ItemMaster);
            //    }
            //}


            if (sSearch != null && (!string.IsNullOrWhiteSpace(sSearch)))
            {
                string SearchText = sSearch; //WebUtility.HtmlDecode(sSearch);

                if (SearchText != null && !string.IsNullOrWhiteSpace(SearchText))
                {
                    if (SearchText.Contains("[###]"))
                    {
                        string[] stringSeparators = new string[] { "[###]" };
                        string[] Fields = SearchText.Split(stringSeparators, StringSplitOptions.None);
                        string[] FieldsPara = Fields[1].Split('@');
                        if (!string.IsNullOrWhiteSpace(FieldsPara[55]))
                        {
                            string ItemLocations = FieldsPara[55].TrimEnd(',');
                            Session["NSItemLocation"] = ItemLocations;
                        }
                    }
                }
            }


            var result = obj.GetPagedRecordsNew(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.UserSupplierIds, CurrentTimeZone, callFrom: "ItemMaster");

            //var result = from u in DataFromDB
            //             select new ItemMasterDTO
            //             {
            //                 ID = u.ID,
            //                 ItemNumber = u.ItemNumber,
            //                 ManufacturerID = u.ManufacturerID,
            //                 ManufacturerNumber = u.ManufacturerNumber,
            //                 ManufacturerName = u.ManufacturerName,
            //                 SupplierID = u.SupplierID,
            //                 SupplierPartNo = u.SupplierPartNo,
            //                 SupplierName = u.SupplierName,
            //                 UPC = u.UPC,
            //                 UNSPSC = u.UNSPSC,
            //                 Description = u.Description,
            //                 LongDescription = u.LongDescription,
            //                 CategoryID = u.CategoryID,
            //                 GLAccountID = u.GLAccountID,
            //                 UOMID = u.UOMID,
            //                 PricePerTerm = u.PricePerTerm,
            //                 CostUOMID = u.CostUOMID,
            //                 DefaultReorderQuantity = u.DefaultReorderQuantity,
            //                 DefaultPullQuantity = u.DefaultPullQuantity,
            //                 Cost = u.Cost,
            //                 Markup = u.Markup,
            //                 SellPrice = u.SellPrice,
            //                 ExtendedCost = u.ExtendedCost,
            //                 AverageCost = u.AverageCost,
            //                 PerItemCost = u.PerItemCost,
            //                 LeadTimeInDays = u.LeadTimeInDays,
            //                 Link1 = u.Link1,
            //                 Link2 = u.Link2,
            //                 Trend = u.Trend,
            //                 IsAutoInventoryClassification = u.IsAutoInventoryClassification,
            //                 Taxable = u.Taxable,
            //                 Consignment = u.Consignment,
            //                 StagedQuantity = u.StagedQuantity,
            //                 InTransitquantity = u.InTransitquantity,
            //                 OnOrderQuantity = u.OnOrderQuantity,
            //                 OnReturnQuantity = u.OnReturnQuantity,
            //                 OnTransferQuantity = u.OnTransferQuantity,
            //                 // OnTransferInTransitQuantity=u.OnTransferInTransitQuantity,
            //                 //  OnTransferInTransitQuantity=u.OnTransferInTransitQuantity,
            //                 SuggestedOrderQuantity = u.SuggestedOrderQuantity,
            //                 SuggestedTransferQuantity = u.SuggestedTransferQuantity,
            //                 RequisitionedQuantity = u.RequisitionedQuantity,
            //                 PackingQuantity = u.PackingQuantity,
            //                 AverageUsage = u.AverageUsage,
            //                 Turns = u.Turns,
            //                 OnHandQuantity = u.OnHandQuantity,
            //                 CriticalQuantity = u.CriticalQuantity,
            //                 MinimumQuantity = u.MinimumQuantity,
            //                 MaximumQuantity = u.MaximumQuantity,
            //                 WeightPerPiece = u.WeightPerPiece,
            //                 ItemUniqueNumber = u.ItemUniqueNumber,
            //                 IsPurchase = u.IsPurchase,
            //                 IsTransfer = u.IsTransfer,
            //                 DefaultLocation = u.DefaultLocation,
            //                 DefaultLocationName = u.DefaultLocationName,
            //                 InventoryClassification = u.InventoryClassification,
            //                 SerialNumberTracking = u.SerialNumberTracking,
            //                 LotNumberTracking = u.LotNumberTracking,
            //                 DateCodeTracking = u.DateCodeTracking,
            //                 ItemType = u.ItemType,
            //                 ImagePath = u.ImagePath,
            //                 UDF1 = u.UDF1,
            //                 UDF2 = u.UDF2,
            //                 UDF3 = u.UDF3,
            //                 UDF4 = u.UDF4,
            //                 UDF5 = u.UDF5,
            //                 UDF6 = u.UDF6,
            //                 UDF7 = u.UDF7,
            //                 UDF8 = u.UDF8,
            //                 UDF9 = u.UDF9,
            //                 UDF10 = u.UDF10,
            //                 //for item grid display purpose - CART, PUll  
            //                 ItemUDF1 = u.UDF1,
            //                 ItemUDF2 = u.UDF2,
            //                 ItemUDF3 = u.UDF3,
            //                 ItemUDF4 = u.UDF4,
            //                 ItemUDF5 = u.UDF5,
            //                 ItemUDF6 = u.UDF6,
            //                 ItemUDF7 = u.UDF7,
            //                 ItemUDF8 = u.UDF8,
            //                 ItemUDF9 = u.UDF9,
            //                 ItemUDF10 = u.UDF10,
            //                 GUID = u.GUID,
            //                 Created = u.Created,
            //                 Updated = u.Updated,

            //                 CreatedBy = u.CreatedBy,
            //                 LastUpdatedBy = u.LastUpdatedBy,
            //                 IsDeleted = u.IsDeleted,
            //                 IsArchived = u.IsArchived,
            //                 CompanyID = u.CompanyID,
            //                 Room = u.Room,
            //                 CreatedByName = u.CreatedByName,
            //                 UpdatedByName = u.UpdatedByName,
            //                 RoomName = u.RoomName,
            //                 IsLotSerialExpiryCost = u.IsLotSerialExpiryCost,
            //                 CategoryName = u.CategoryName,
            //                 Unit = u.Unit,
            //                 GLAccount = u.GLAccount,
            //                 IsItemLevelMinMaxQtyRequired = u.IsItemLevelMinMaxQtyRequired,
            //                 IsBuildBreak = u.IsBuildBreak,
            //                 BondedInventory = u.BondedInventory,
            //                 IsEnforceDefaultReorderQuantity = u.IsEnforceDefaultReorderQuantity,
            //                 InventoryClassificationName = u.InventoryClassificationName,
            //                 IsBOMItem = u.IsBOMItem,
            //                 RefBomId = u.RefBomId,
            //                 CostUOMName = u.CostUOMName,
            //                 PullQtyScanOverride = u.PullQtyScanOverride,
            //                 TrendingSetting = u.TrendingSetting,
            //                 IsPackslipMandatoryAtReceive = u.IsPackslipMandatoryAtReceive,
            //                 // CreatedDate = CommonUtility.ConvertDateByTimeZone(u.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
            //                 // UpdatedDate = CommonUtility.ConvertDateByTimeZone(u.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
            //                 // CreatedDate = u.Created.Value.ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture),
            //                 // UpdatedDate = u.Updated.Value.ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture),

            //                 //ReceivedOnDate = CommonUtility.ConvertDateByTimeZone(u.ReceivedOn, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
            //                 AddedFrom = u.AddedFrom,
            //                 EditedFrom = u.EditedFrom,
            //                 ReceivedOnWeb = u.ReceivedOnWeb,
            //                 ReceivedOn = u.ReceivedOn,
            //                 BPONumber = u.BPONumber,
            //                 PriceSavedDateStr = u.PriceSavedDateStr,
            //                 PulledDate = u.PulledDate,
            //                 OrderedDate = u.OrderedDate,
            //                 CountedDate = u.CountedDate,
            //                 TrasnferedDate = u.TrasnferedDate,
            //                 ItemImageExternalURL = u.ItemImageExternalURL,
            //                 ItemLink2ExternalURL = u.ItemLink2ExternalURL,
            //                 ItemLink2ImageType = u.ItemLink2ImageType,
            //                 ItemDocExternalURL = u.ItemDocExternalURL,
            //                 QtyToMeetDemand = u.QtyToMeetDemand,
            //                 OutTransferQuantity = u.OutTransferQuantity,
            //                 OnOrderInTransitQuantity = u.OnOrderInTransitQuantity,
            //                 IsActive = u.IsActive,
            //                 ImageType = u.ImageType,
            //                 MonthlyAverageUsage = u.MonthlyAverageUsage,
            //                 IsAllowOrderCostuom = u.IsAllowOrderCostuom,
            //                 SuggestedReturnQuantity = u.SuggestedReturnQuantity,
            //                 IsOrderable = u.IsOrderable,
            //                 OnQuotedQuantity = u.OnQuotedQuantity,
            //                 EnhancedDescription = u.EnhancedDescription,
            //                 EnrichedProductData = u.EnrichedProductData,
            //                 eLabelKey = u.eLabelKey
            //                 //ItemImageExternalURL=u.ItemImageExternalURL
            //             };

            Session["BinReplanish"] = null;
            JsonResult jsresult = Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = result
            }, JsonRequestBehavior.AllowGet);
            jsresult.MaxJsonLength = int.MaxValue;
            return jsresult;
        }


        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult ItemMasterListAllAjax(QuickListJQueryDataTableParamModel param)
        {
            Session["NSItemLocation"] = null;
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
            if (string.IsNullOrEmpty(sortColumnName) || sortColumnName == "undefine" || sortColumnName == "0" || sortColumnName.Trim().Contains("null") || string.IsNullOrEmpty(sortColumnName.Replace("asc", "").Replace("null", "").Replace("desc", "").Trim()))
                sortColumnName = "ID asc";

            string searchQuery = string.Empty;
            int TotalRecordCount = 0;
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);

            string sSearch = WebUtility.HtmlDecode(param.sSearch);

            //if (string.IsNullOrWhiteSpace(sSearch) && param.IsNarroSearchClear == false)
            //{
            //    using (UserNarrowSearchSettingsBAL searchSettingsBAL = new UserNarrowSearchSettingsBAL())
            //    {
            //        sSearch = searchSettingsBAL.GetSavedSearchStr(NarrowSearchSaveListEnum.ItemMaster);
            //    }
            //}


            if (sSearch != null && (!string.IsNullOrWhiteSpace(sSearch)))
            {
                string SearchText = sSearch; //WebUtility.HtmlDecode(sSearch);

                if (SearchText != null && !string.IsNullOrWhiteSpace(SearchText))
                {
                    if (SearchText.Contains("[###]"))
                    {
                        string[] stringSeparators = new string[] { "[###]" };
                        string[] Fields = SearchText.Split(stringSeparators, StringSplitOptions.None);
                        string[] FieldsPara = Fields[1].Split('@');
                        if (!string.IsNullOrWhiteSpace(FieldsPara[55]))
                        {
                            string ItemLocations = FieldsPara[55].TrimEnd(',');
                            Session["NSItemLocation"] = ItemLocations;
                        }
                    }
                }
            }


            var DataFromDB = obj.GetPagedRecordsAll(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, sSearch, sortColumnName, IsArchived, IsDeleted, Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.UserSupplierIds, CurrentTimeZone, callFrom: "ItemMaster");

            var result = from u in DataFromDB
                         select new ItemMasterDTO
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
                             PerItemCost = u.PerItemCost,
                             LeadTimeInDays = u.LeadTimeInDays,
                             Link1 = u.Link1,
                             Link2 = u.Link2,
                             Trend = u.Trend,
                             IsAutoInventoryClassification = u.IsAutoInventoryClassification,
                             Taxable = u.Taxable,
                             Consignment = u.Consignment,
                             StagedQuantity = u.StagedQuantity,
                             InTransitquantity = u.InTransitquantity,
                             OnOrderQuantity = u.OnOrderQuantity,
                             OnReturnQuantity = u.OnReturnQuantity,
                             OnTransferQuantity = u.OnTransferQuantity,
                             // OnTransferInTransitQuantity=u.OnTransferInTransitQuantity,
                             //  OnTransferInTransitQuantity=u.OnTransferInTransitQuantity,
                             SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                             SuggestedTransferQuantity = u.SuggestedTransferQuantity,
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
                             UDF6 = u.UDF6,
                             UDF7 = u.UDF7,
                             UDF8 = u.UDF8,
                             UDF9 = u.UDF9,
                             UDF10 = u.UDF10,
                             //for item grid display purpose - CART, PUll  
                             ItemUDF1 = u.UDF1,
                             ItemUDF2 = u.UDF2,
                             ItemUDF3 = u.UDF3,
                             ItemUDF4 = u.UDF4,
                             ItemUDF5 = u.UDF5,
                             ItemUDF6 = u.UDF6,
                             ItemUDF7 = u.UDF7,
                             ItemUDF8 = u.UDF8,
                             ItemUDF9 = u.UDF9,
                             ItemUDF10 = u.UDF10,
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
                             RefBomId = u.RefBomId,
                             CostUOMName = u.CostUOMName,
                             PullQtyScanOverride = u.PullQtyScanOverride,
                             TrendingSetting = u.TrendingSetting,
                             IsPackslipMandatoryAtReceive = u.IsPackslipMandatoryAtReceive,
                             // CreatedDate = CommonUtility.ConvertDateByTimeZone(u.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             // UpdatedDate = CommonUtility.ConvertDateByTimeZone(u.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             // CreatedDate = u.Created.Value.ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture),
                             // UpdatedDate = u.Updated.Value.ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture),

                             //ReceivedOnDate = CommonUtility.ConvertDateByTimeZone(u.ReceivedOn, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             AddedFrom = u.AddedFrom,
                             EditedFrom = u.EditedFrom,
                             ReceivedOnWeb = u.ReceivedOnWeb,
                             ReceivedOn = u.ReceivedOn,
                             BPONumber = u.BPONumber,
                             PriceSavedDateStr = u.PriceSavedDateStr,
                             PulledDate = u.PulledDate,
                             OrderedDate = u.OrderedDate,
                             CountedDate = u.CountedDate,
                             TrasnferedDate = u.TrasnferedDate,
                             ItemImageExternalURL = u.ItemImageExternalURL,
                             ItemLink2ExternalURL = u.ItemLink2ExternalURL,
                             ItemLink2ImageType = u.ItemLink2ImageType,
                             ItemDocExternalURL = u.ItemDocExternalURL,
                             QtyToMeetDemand = u.QtyToMeetDemand,
                             OutTransferQuantity = u.OutTransferQuantity,
                             OnOrderInTransitQuantity = u.OnOrderInTransitQuantity,
                             IsActive = u.IsActive,
                             ImageType = u.ImageType,
                             MonthlyAverageUsage = u.MonthlyAverageUsage,
                             IsAllowOrderCostuom = u.IsAllowOrderCostuom,
                             SuggestedReturnQuantity = u.SuggestedReturnQuantity,
                             IsOrderable = u.IsOrderable,
                             OnQuotedQuantity = u.OnQuotedQuantity
                             //ItemImageExternalURL=u.ItemImageExternalURL
                         };

            Session["BinReplanish"] = null;
            JsonResult jsresult = Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = result
            }, JsonRequestBehavior.AllowGet);
            jsresult.MaxJsonLength = int.MaxValue;
            return jsresult;
        }



        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult ItemMaster_ChangeLogListAjax(QuickListJQueryDataTableParamModel param)
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
            var DataFromDB = obj.GetPagedRecordsNew_ChnageLog(ItemGuid, param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, SessionHelper.UserSupplierIds);

            JsonResult jsresult = Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB
            }, JsonRequestBehavior.AllowGet);
            jsresult.MaxJsonLength = int.MaxValue;
            return jsresult;

        }

        public string DuplicateCheck(string TechnicianName, string ActionMode, int ID)
        {
            InventoryController obj = new InventoryController();
            return obj.DuplicateCheck(TechnicianName, ActionMode, ID);
        }

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public JsonResult DeleteRecords(string ids)
        {
            try
            {
                ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                bool IsConsignedEditable = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowChangeConsignedItems);
                int consignditems = 0;

                if (!IsConsignedEditable && !string.IsNullOrEmpty(ids) && !string.IsNullOrWhiteSpace(ids))
                {
                    int count1 = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Count();
                    ids = objItemMasterDAL.GetnonConsigneditemsbeforeDelete(ids);
                    int count2 = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Count();
                    consignditems = count1 - count2;
                }
                string response = string.Empty;
                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                ModuleDeleteDTO objModuleDeleteDTO = new ModuleDeleteDTO();
                objModuleDeleteDTO = objCommonDAL.DeleteModulewise(ids, ImportMastersDTO.TableName.ItemMaster.ToString(), true, SessionHelper.UserID, true, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);
                response = objModuleDeleteDTO.CommonMessage;
                if (consignditems > 0)
                {
                    response = response + "\r\t" + consignditems + " " + ResItemMaster.CannotDeleteConsignedItems;
                }

                //---------------------------------------------
                //
                string[] lstItemIds = ids.Split(',');
                BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                //List<long> DeletedBinIds = null;
                //string ItemBinIDs = "";
                List<string> DeletedBinAndIds = null;
                objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);

                DeletedBinAndIds = objItemMasterDAL.DeleteItemData(string.Join(",", objModuleDeleteDTO.SuccessItems.Select(t => Guid.Parse(t.Id))), SessionHelper.UserID, "Delete From Web");
                long SessionUserId = 0;
                SessionUserId = SessionHelper.UserID;
                foreach (string ItemBindIds in DeletedBinAndIds)
                {
                    objBinMasterDAL.CSP_DeleteBinDataById(ItemBindIds, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, "Inventory >> Delete Item Bin", SessionUserId);
                }

                if (objModuleDeleteDTO != null && objModuleDeleteDTO.SuccessItems != null && objModuleDeleteDTO.SuccessItems.Count > 0)
                {
                    GlobalHost.ConnectionManager.GetHubContext<eTurnsHub>().Clients.Group(SessionHelper.EnterPriceID + "_" + SessionHelper.CompanyID + "_" + SessionHelper.RoomID).UpdateRedCircleCountInClients();
                }
                eTurns.DAL.CacheHelper<IEnumerable<ItemMasterDTO>>.InvalidateCache();

                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);

            }
            catch(Exception ex)
            {
                return Json(new { Message = ex.Message, Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        //Undelete Records
        public JsonResult UnDeleteRecords(string ids, string ModuleName)
        {
            try
            {

                string response = string.Empty;
                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                ModuleUnDeleteDTO objModuleUnDeleteDTO = new ModuleUnDeleteDTO();
                var enterpriseId = SessionHelper.EnterPriceID;

                if (ModuleName.ToLower() != "assetcategorys" && ModuleName.ToLower() != "binlist" && ModuleName.ToLower() != "categorylist" && ModuleName.ToLower() != "costuomlist" && ModuleName.ToLower() != "customermasterlist" && ModuleName.ToLower() != "freighttypelist" && ModuleName.ToLower() != "glaccountlist" && ModuleName.ToLower() != "inventoryclassificationlist" && ModuleName.ToLower() != "measurementtermlist" && ModuleName.ToLower() != "manufacturerlist"
                    && ModuleName.ToLower() != "shipvialist" && ModuleName.ToLower() != "supplierlist" && ModuleName.ToLower() != "technicianlist" && ModuleName.ToLower() != "toolcategorylist" && ModuleName.ToLower() != "unitlist" && ModuleName.ToLower() != "vendermasterlist"
                    && ModuleName.ToLower() != "assetlist" && ModuleName.ToLower() != "room" && ModuleName.ToLower() != "notificationmaster" && ModuleName.ToLower() != "templateconfiguration"
                    && ModuleName.ToLower() != "quotemaster" && ModuleName.ToLower() != "toolwrittenoffcategory")
                {
                    objModuleUnDeleteDTO = objCommonDAL.UnDeleteModulewise(ids, ModuleName, true, SessionHelper.UserID, true, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);
                }
                else
                {
                    objModuleUnDeleteDTO = objCommonDAL.UnDeleteModulewise(ids, ModuleName, false, SessionHelper.UserID, true, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);
                }
                response = objModuleUnDeleteDTO.CommonMessage;
                long SessionUserId = SessionHelper.UserID;

                if (objModuleUnDeleteDTO != null && objModuleUnDeleteDTO.SuccessItems != null && objModuleUnDeleteDTO.SuccessItems.Count > 0)
                {
                    foreach (var item in objModuleUnDeleteDTO.SuccessItems)
                    {
                        if (ModuleName.ToLower() == "itemmaster")
                        {
                            //*******Create/Update Cart Automated entry after undelete item ***********

                            //new CartItemDAL(SessionHelper.EnterPriseDBName).AutoCartUpdateByCode(Guid.Parse(item.Id), SessionHelper.UserID, "Web", "InventoryControler>> UnDeleteRecords");
                            new CartItemDAL(SessionHelper.EnterPriseDBName).AutoCartUpdateByCode(Guid.Parse(item.Id), SessionHelper.UserID, "Web", "Inventory >> UnDelete Item", SessionUserId);

                            new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName).UnDeleteItemSupplierByItemGUID(Guid.Parse(item.Id));
                            new ItemManufacturerDetailsDAL(SessionHelper.EnterPriseDBName).UnDeleteItemManufacturerDetailsByItemGUID(Guid.Parse(item.Id));
                            eTurns.DAL.CacheHelper<IEnumerable<ItemMasterDTO>>.InvalidateCache();
                        }
                        else if (ModuleName.ToLower() == "ordermaster")
                        {
                            //*************Update onorderqty of item and lineitem,ordermaster data related effect after successful undelete order and its lineitem**
                            Guid OrdGuid = Guid.Parse(item.Id);
                            OrderMasterDTO objOrd = new OrderMasterDTO();
                            OrderMasterDAL objOrdDAL = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                            objOrd = objOrdDAL.GetOrderByGuidPlain(OrdGuid);

                            if (objOrd != null)
                            {
                                OrderDetailsDAL objOrdDtlDAL = new OrderDetailsDAL(SessionHelper.EnterPriseDBName);
                                List<OrderDetailsDTO> listOrdDtl = new List<OrderDetailsDTO>();
                                listOrdDtl = objOrdDtlDAL.GetOrderDetailByOrderGUIDPlain(OrdGuid, Convert.ToInt64(objOrd.Room), Convert.ToInt64(objOrd.CompanyID));

                                if (listOrdDtl != null && listOrdDtl.Count() > 0)
                                {
                                    //Line Order Item Related Updation 
                                    foreach (var ordDtl in listOrdDtl)
                                    {
                                        objOrdDtlDAL.Edit(ordDtl, SessionUserId, enterpriseId);
                                    }
                                }
                                //Update OrderMaster
                                objOrdDAL.Edit(objOrd);
                            }
                        }
                        else if (ModuleName.ToLower() == "pull")
                        {
                            //reverse qty in itemlocdetail and locqty after undelete from pullmaster and pulldetail
                            bool AllowUnDelete = false;
                            string StrMsg = string.Empty;
                            Guid PullGuid = Guid.Parse(item.Id);
                            PullMasterDAL objPullDAL = new PullMasterDAL(SessionHelper.EnterPriseDBName);
                            PullMasterViewDTO objPullDTO = new PullMasterViewDTO();
                            objPullDTO = objPullDAL.GetPullByGuidPlain(PullGuid);
                            if (objPullDTO != null)
                            {
                                AllowUnDelete = objPullDAL.PullCreditAfterUndelete(objPullDTO, SessionHelper.RoomID, SessionHelper.CompanyID, out StrMsg, SessionUserId,SessionHelper.EnterPriceID);
                                if (AllowUnDelete)
                                {
                                    if (objPullDTO.RequisitionDetailGUID != null)
                                    {
                                        new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName).UpdateItemOnRequisitionQty(objPullDTO.ItemGUID.GetValueOrDefault(Guid.Empty), objPullDTO.Room.GetValueOrDefault(0), objPullDTO.CompanyID.GetValueOrDefault(0), objPullDTO.LastUpdatedBy.GetValueOrDefault(0));
                                        RequisitionDetailsDAL objReqDtlDal = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName);
                                        RequisitionDetailsDTO objRequDetDTO = objReqDtlDal.GetRequisitionDetailsByGUIDPlain(objPullDTO.RequisitionDetailGUID.GetValueOrDefault(Guid.Empty));
                                        if (objRequDetDTO != null)
                                        {
                                            objRequDetDTO.QuantityPulled = objRequDetDTO.QuantityPulled.GetValueOrDefault(0) + objPullDTO.PoolQuantity.GetValueOrDefault(0);
                                            objReqDtlDal.Edit(objRequDetDTO, SessionUserId);
                                        }
                                    }
                                    if (objPullDTO.ItemGUID != null && objPullDTO.ItemGUID != Guid.Empty)
                                    {
                                        //*******Create/Update Cart Automated entry after undelete Pull ***********
                                        new CartItemDAL(SessionHelper.EnterPriseDBName).AutoCartUpdateByCode(objPullDTO.ItemGUID.GetValueOrDefault(Guid.Empty), SessionHelper.UserID, "Web", "Pull >> UnDelete Pull", SessionUserId);
                                    }
                                    return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                    return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else if (ModuleName.ToLower() == "assetcategorys")
                        {
                            eTurns.DAL.CacheHelper<IEnumerable<AssetCategoryMasterDTO>>.InvalidateCache();
                        }
                        else if (ModuleName.ToLower() == "categorylist")
                        {
                            eTurns.DAL.CacheHelper<IEnumerable<CategoryMasterDTO>>.InvalidateCache();
                        }
                        else if (ModuleName.ToLower() == "customermasterlist")
                        {
                            eTurns.DAL.CacheHelper<IEnumerable<CustomerMasterDTO>>.InvalidateCache();
                        }
                        else if (ModuleName.ToLower() == "freighttypelist")
                        {
                            eTurns.DAL.CacheHelper<IEnumerable<FreightTypeMasterDTO>>.InvalidateCache();
                        }
                        else if (ModuleName.ToLower() == "glaccountlist")
                        {
                            //eTurns.DAL.CacheHelper<IEnumerable<GLAccountMasterDTO>>.InvalidateCache();
                        }
                        else if (ModuleName.ToLower() == "inventoryclassificationlist")
                        {
                            eTurns.DAL.CacheHelper<IEnumerable<InventoryClassificationMasterDTO>>.InvalidateCache();
                        }
                        else if (ModuleName.ToLower() == "manufacturerlist")
                        {
                            eTurns.DAL.CacheHelper<IEnumerable<ManufacturerMasterDTO>>.InvalidateCache();
                        }
                        else if (ModuleName.ToLower() == "shipvialist")
                        {
                            eTurns.DAL.CacheHelper<IEnumerable<ShipViaDTO>>.InvalidateCache();
                        }
                        else if (ModuleName.ToLower() == "supplierlist")
                        {
                            eTurns.DAL.CacheHelper<IEnumerable<SupplierMasterDTO>>.InvalidateCache();
                        }
                        else if (ModuleName.ToLower() == "technicianlist")
                        {
                            eTurns.DAL.CacheHelper<IEnumerable<TechnicianMasterDTO>>.InvalidateCache();
                        }
                        else if (ModuleName.ToLower() == "toolcategorylist")
                        {
                            eTurns.DAL.CacheHelper<IEnumerable<ToolCategoryMasterDTO>>.InvalidateCache();
                        }
                        else if (ModuleName.ToLower() == "unitlist")
                        {
                            eTurns.DAL.CacheHelper<IEnumerable<UnitMasterDTO>>.InvalidateCache();
                        }
                        else if (ModuleName.ToLower() == "vendermasterlist")
                        {
                            eTurns.DAL.CacheHelper<IEnumerable<VenderMasterDTO>>.InvalidateCache();
                        }
                        else if (ModuleName.ToLower() == "measurementtermlist")
                        {

                            eTurns.DAL.CacheHelper<IEnumerable<MeasurementTermMasterDTO>>.InvalidateCache();
                        }
                        else if (ModuleName.ToLower() == "requisition")
                        {
                            Guid RequisitionGuid = Guid.Parse(item.Id);
                            RequisitionMasterDAL objReqDal = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
                            RequisitionDetailsDAL objReqDtlDal = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName);
                            RequisitionMasterDTO ReqDTO = objReqDal.GetRequisitionByGUIDPlain(RequisitionGuid);

                            if (ReqDTO != null)
                            {
                                objReqDtlDal.UpdateRequisitionTotalCost(RequisitionGuid, SessionHelper.RoomID, SessionHelper.CompanyID);

                                List<RequisitionDetailsDTO> lst = objReqDtlDal.GetReqLinesByReqGUIDPlain(ReqDTO.GUID, 0, Convert.ToInt64(ReqDTO.Room), Convert.ToInt64(ReqDTO.CompanyID));
                                if (lst != null && lst.Count > 0)
                                {
                                    foreach (var reqdtlitem in lst)
                                    {
                                        objReqDtlDal.UpdateItemOnRequisitionQty(reqdtlitem.ItemGUID.GetValueOrDefault(Guid.Empty), reqdtlitem.Room, reqdtlitem.CompanyID, reqdtlitem.LastUpdatedBy);
                                    }
                                }
                            }
                        }
                        else if (ModuleName.ToLower() == "materialstaging")
                        {
                            eTurns.DAL.CacheHelper<IEnumerable<MaterialStagingDTO>>.InvalidateCache();
                            eTurns.DAL.CacheHelper<IEnumerable<MaterialStagingDetailDTO>>.InvalidateCache();
                            eTurns.DAL.CacheHelper<IEnumerable<MaterialStagingPullDetailDTO>>.InvalidateCache();


                            //Undelete Line Item and update stage Qty
                            if (new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName).UnDeleteMSDetails(ids, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, SessionUserId,enterpriseId))
                                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);
                            else
                                return Json(new { Message = response, Status = "Fail" }, JsonRequestBehavior.AllowGet);
                        }
                        else if (ModuleName.ToLower() == "quicklist")
                        {
                            eTurns.DAL.CacheHelper<IEnumerable<QuickListMasterDTO>>.InvalidateCache();
                            eTurns.DAL.CacheHelper<IEnumerable<QuickListDetailDTO>>.InvalidateCache();
                        }
                        else if (ModuleName.ToLower() == "transfer")
                        {
                            eTurns.DAL.CacheHelper<IEnumerable<TransferMasterDTO>>.InvalidateCache();
                            eTurns.DAL.CacheHelper<IEnumerable<TransferDetailDTO>>.InvalidateCache();
                        }
                        else if (ModuleName.ToLower() == "toollist")
                        {
                            eTurns.DAL.CacheHelper<IEnumerable<ToolMasterDTO>>.InvalidateCache();

                        }
                        else if (ModuleName.ToLower() == "workorder")
                        {
                            //eTurns.DAL.CacheHelper<IEnumerable<WorkOrderDTO>>.InvalidateCache();
                        }
                        else if (ModuleName.ToLower() == "cartitem")
                        {
                            //Update Suggested ord qty 
                            CartItemDTO objcart = new CartItemDTO();
                            objcart = new CartItemDAL(SessionHelper.EnterPriseDBName).GetCartByGUIDPlain(Guid.Parse(item.Id), SessionHelper.RoomID, SessionHelper.CompanyID);
                            if (objcart != null)
                            {
                                new CartItemDAL(SessionHelper.EnterPriseDBName).UpdateSuggestedQtyOfItem(Guid.Parse(objcart.ItemGUID.ToString()), SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionUserId,SessionHelper.EnterPriceID);
                            }
                            eTurns.DAL.CacheHelper<IEnumerable<CartItemDTO>>.InvalidateCache();

                        }
                    }
                    //GlobalHost.ConnectionManager.GetHubContext<eTurnsHub>().Clients.All.UpdateRedCircleCountInClients();
                    GlobalHost.ConnectionManager.GetHubContext<eTurnsHub>().Clients.Group(SessionHelper.EnterPriceID + "_" + SessionHelper.CompanyID + "_" + SessionHelper.RoomID).UpdateRedCircleCountInClients();

                    if (ModuleName.ToLower() == "quotemaster")
                    {
                        var quoteMasterDAL = new QuoteMasterDAL(SessionHelper.EnterPriseDBName);

                        string[] strArrIDs = ids.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        string strIDs = string.Join(",", strArrIDs);
                        var quotes = quoteMasterDAL.GetUnapprovedQuotesByIdsPlain(strIDs);
                        var quoteDetailDAL = new QuoteDetailDAL(SessionHelper.EnterPriseDBName);
                        var itemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                        foreach (var item in quotes)
                        {
                            var itemGuids = quoteDetailDAL.GetItemGuidsByQuoteGuid(item.GUID);

                            foreach (var itemGuid in itemGuids)
                            {
                                var objItemDTO = itemMasterDAL.GetItemWithoutJoins(null, itemGuid);
                                objItemDTO.LastUpdatedBy = item.LastUpdatedBy;
                                objItemDTO.IsOnlyFromItemUI = false;
                                itemMasterDAL.Edit(objItemDTO, SessionHelper.UserID, SessionHelper.EnterPriceID);
                            }
                        }
                    }
                }


                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SubmitItemManufacturerDetails(string para)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            ItemManufacturerDetails[] ManufacturerData = s.Deserialize<ItemManufacturerDetails[]>(para);

            if (ManufacturerData != null && ManufacturerData.Length > 0)
            {
                List<ItemManufacturerDetailsDTO> lstManuf = new List<ItemManufacturerDetailsDTO>();
                foreach (ItemManufacturerDetails item in ManufacturerData)
                {
                    ItemManufacturerDetailsDTO tempManufDTO = new ItemManufacturerDetailsDTO();
                    tempManufDTO.CompanyID = SessionHelper.CompanyID;
                    tempManufDTO.Room = SessionHelper.RoomID;
                    tempManufDTO.RoomName = SessionHelper.RoomName;
                    tempManufDTO.Created = DateTimeUtility.DateTimeNow;
                    tempManufDTO.CreatedBy = SessionHelper.UserID;
                    tempManufDTO.CreatedByName = SessionHelper.UserName;
                    tempManufDTO.Updated = DateTimeUtility.DateTimeNow;
                    tempManufDTO.LastUpdatedBy = SessionHelper.UserID;
                    tempManufDTO.UpdatedByName = SessionHelper.UserName;
                    tempManufDTO.IsArchived = false;
                    tempManufDTO.IsDeleted = false;
                    tempManufDTO.ID = 0;
                    tempManufDTO.ItemGUID = Guid.Parse(item.ItemGUID);
                    tempManufDTO.ManufacturerID = Int64.Parse(item.ManufacturerID);
                    tempManufDTO.ManufacturerName = item.ManufacturerName;
                    tempManufDTO.ManufacturerNumber = item.ManufacturerNumber;
                    if (item.IsDefault == "Yes")
                        tempManufDTO.IsDefault = true;
                    else
                        tempManufDTO.IsDefault = false;

                    lstManuf.Add(tempManufDTO);
                }
                ItemLocationDetailsDAL obj = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                obj.ItemManufacturerDetailsSave(lstManuf);
            }
            return Json(new { Message = "Saved" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteItemManufacturerDetails(string ItemGUID)
        {
            ItemManufacturerDetailsDAL objItemManufacturerDetailsDAL = new ItemManufacturerDetailsDAL(SessionHelper.EnterPriseDBName);
            objItemManufacturerDetailsDAL.DeleteItemManufacturerDetailsByItemGUID(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID);

            //using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(SessionHelper.EnterPriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
            //{
            //    try
            //    {
            //        string tempDeleteQuery = @"delete ItemManufacturerDetails where ItemGUID = '" + ItemGUID + "' and Room = " + SessionHelper.RoomID + " and CompanyID = " + SessionHelper.CompanyID + "";
            //        context.Database.ExecuteSqlCommand(tempDeleteQuery);

            //        // cache delete code 
            //        //IEnumerable<ItemManufacturerDetailsDTO> ObjCache = CacheHelper<IEnumerable<ItemManufacturerDetailsDTO>>.GetCacheItem("Cached_ItemManufacturerDetails_" + SessionHelper.CompanyID.ToString());
            //        //if (ObjCache != null)
            //        //{
            //        //    List<ItemManufacturerDetailsDTO> objTemp = ObjCache.ToList();
            //        //    objTemp.RemoveAll(i => i.ItemGUID == Guid.Parse(ItemGUID) && i.Room == SessionHelper.RoomID && i.CompanyID == SessionHelper.CompanyID);
            //        //    ObjCache = objTemp.AsEnumerable();
            //        //    CacheHelper<IEnumerable<ItemManufacturerDetailsDTO>>.AppendToCacheItem("Cached_ItemManufacturerDetails_" + SessionHelper.CompanyID.ToString(), ObjCache);
            //        //}
            //    }
            //    catch
            //    {

            //    }
            //}
            return Json(new { Message = "Saved" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SubmitItemSupplierDetails(string para)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            ItemSupplierDetails[] SupplierData = s.Deserialize<ItemSupplierDetails[]>(para);
            if (SupplierData != null && SupplierData.Length > 0)
            {
                List<ItemSupplierDetailsDTO> lstSuppl = new List<ItemSupplierDetailsDTO>();
                foreach (ItemSupplierDetails item in SupplierData)
                {
                    ItemSupplierDetailsDTO tempSupplDTO = new ItemSupplierDetailsDTO();
                    tempSupplDTO.CompanyID = SessionHelper.CompanyID;
                    tempSupplDTO.Room = SessionHelper.RoomID;
                    tempSupplDTO.RoomName = SessionHelper.RoomName;

                    tempSupplDTO.Created = DateTimeUtility.DateTimeNow;
                    tempSupplDTO.CreatedBy = SessionHelper.UserID;
                    tempSupplDTO.CreatedByName = SessionHelper.UserName;
                    tempSupplDTO.Updated = DateTimeUtility.DateTimeNow;
                    tempSupplDTO.LastUpdatedBy = SessionHelper.UserID;
                    tempSupplDTO.UpdatedByName = SessionHelper.UserName;

                    tempSupplDTO.IsArchived = false;
                    tempSupplDTO.IsDeleted = false;

                    tempSupplDTO.ID = 0;
                    tempSupplDTO.ItemGUID = Guid.Parse(item.ItemGUID);
                    tempSupplDTO.SupplierID = Int64.Parse(item.SupplierID);
                    tempSupplDTO.SupplierName = item.SupplierName;
                    tempSupplDTO.SupplierNumber = item.SupplierNumber;
                    if (item.IsDefault == "Yes")
                        tempSupplDTO.IsDefault = true;
                    else
                        tempSupplDTO.IsDefault = false;

                    lstSuppl.Add(tempSupplDTO);
                }
                ItemLocationDetailsDAL obj = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                obj.ItemSupplierDetailsSave(lstSuppl);
            }
            return Json(new { Message = "Saved" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteItemLocations(string ItemGUID)
        {
            //using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(SessionHelper.EnterPriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
            //{
            //    // if exists then first delete 
            //    if ((!string.IsNullOrEmpty(ItemGUID)) && Guid.Parse(ItemGUID) != Guid.Empty)
            //    {

            //        string tempDeleteQuery = @"delete ItemLocationLevelQuanity where ItemGUID = '" + ItemGUID + "' and Room = " + SessionHelper.RoomID + " and CompanyID = " + SessionHelper.CompanyID + "";
            //        context.Database.ExecuteSqlCommand(tempDeleteQuery);
            //        // cache delete code 
            //        IEnumerable<ItemLocationLevelQuanityDTO> ObjCache = CacheHelper<IEnumerable<ItemLocationLevelQuanityDTO>>.GetCacheItem("Cached_ItemLocationLevelQuanity_" + SessionHelper.CompanyID);
            //        if (ObjCache != null)
            //        {
            //            List<ItemLocationLevelQuanityDTO> objTemp = ObjCache.ToList();
            //            objTemp.RemoveAll(i => i.ItemGUID == Guid.Parse(ItemGUID) && i.Room == SessionHelper.RoomID && i.CompanyID == SessionHelper.CompanyID);
            //            ObjCache = objTemp.AsEnumerable();
            //            CacheHelper<IEnumerable<ItemLocationLevelQuanityDTO>>.AppendToCacheItem("Cached_ItemLocationLevelQuanity_" + SessionHelper.CompanyID.ToString(), ObjCache);
            //        }
            //    }
            //}
            return Json(new { Message = "ok" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetItemManufacturerDetails(Guid ItemGUID)
        {
            if (ItemGUID != Guid.Empty)
            {
                ItemManufacturerDetailsDAL objData = new ItemManufacturerDetailsDAL(SessionHelper.EnterPriseDBName);
                var result = objData.GetManufacturerByItemGuidNormal(SessionHelper.RoomID, SessionHelper.CompanyID, ItemGUID, false);
                return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult GetItemSupplierDetails(Guid ItemGUID)
        {
            if (ItemGUID != Guid.Empty)
            {
                ItemSupplierDetailsDAL objData = new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName);
                var result = objData.GetSuppliersByItemGuidNormal(SessionHelper.RoomID, SessionHelper.CompanyID, ItemGUID);
                return Json(new { Result = result }, JsonRequestBehavior.AllowGet);

            }
            return null;
        }

        public ActionResult GetMoreLocations(Guid ItemGUID)
        {
            if (ItemGUID != Guid.Empty)
            {
                BinMasterDAL objData = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                var result = objData.GetItemLocationQty(SessionHelper.RoomID, SessionHelper.CompanyID, ItemGUID);
                //var result = objData.GetItemLocationQty(SessionHelper.RoomID, SessionHelper.CompanyID,ItemGUID, 0,null,null);//.Where(x => x.ItemGUID == ItemGUID);
                return Json(new { Result = result }, JsonRequestBehavior.AllowGet);

            }
            return null;
        }

        public JsonResult GetLatestQTYfromItem(Guid ItemGUID)
        {
            ItemMasterDAL objItemAPI = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            ItemMasterDTO Objitem = objItemAPI.GetItemWithoutJoins(null, ItemGUID);

            if (Objitem != null)
                return Json(Objitem, JsonRequestBehavior.AllowGet);
            else
                return Json(new ItemMasterDTO(), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region "ItemBinWiseSummary"

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ItemBinWiseSummary(Int64 ItemID, string ItemGUID)
        {
            ViewBag.ItemID = ItemID;
            ViewBag.ItemGUID = ItemGUID;

            ItemMasterDAL objItemAPI = new ItemMasterDAL(SessionHelper.EnterPriseDBName);

            //var Objitem = objItemAPI.GetRecord(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
            //if (Objitem == null)
            //    Objitem = objItemAPI.GetRecord(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, true, true);
            Guid ItemGUID1 = Guid.Empty;
            Guid.TryParse(ItemGUID, out ItemGUID1);
            var Objitem = objItemAPI.GetItemWithoutJoins(null, ItemGUID1);

            ViewBag.ItemNumber = Objitem.ItemNumber;
            ViewBag.Consignment = Objitem.Consignment;
            ViewBag.ItemGUID_ItemType = ItemGUID + "#" + Objitem.ItemType;


            bool IsConsignedEditable = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowChangeConsignedItems);

            if (!IsConsignedEditable && Objitem.Consignment)
            {
                ViewBag.ViewOnly = true;
            }
            else
            {
                ViewBag.ViewOnly = false;
            }

            return RenderRazorViewToString("_ItemBinWiseSummary", Objitem);
        }

        public string ItemBinWiseSummaryByRoomAndCompany(Int64 ItemID, string ItemGUID)
        {
            ViewBag.ItemID = ItemID;
            ViewBag.ItemGUID = ItemGUID;

            ItemMasterDAL objItemAPI = new ItemMasterDAL(SessionHelper.EnterPriseDBName);

            //var Objitem = objItemAPI.GetRecord(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
            //if (Objitem == null)
            //    Objitem = objItemAPI.GetRecord(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, true, true);
            Guid ItemGUID1 = Guid.Empty;
            Guid.TryParse(ItemGUID, out ItemGUID1);
            var Objitem = objItemAPI.GetItemWithoutJoins(null, ItemGUID1);

            ViewBag.ItemNumber = Objitem.ItemNumber;
            ViewBag.Consignment = Objitem.Consignment;
            ViewBag.ItemGUID_ItemType = ItemGUID + "#" + Objitem.ItemType;


            bool IsConsignedEditable = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowChangeConsignedItems);

            if (!IsConsignedEditable && Objitem.Consignment)
            {
                ViewBag.ViewOnly = true;
            }
            else
            {
                ViewBag.ViewOnly = false;
            }

            return RenderRazorViewToString("_ItemBinWiseSummaryByRoomAndCompany", Objitem);
        }
        //ItemBinWiseSummaryListAjax
        public ActionResult ItemBinWiseSummaryListAjax(JQueryDataTableParamModel param)
        {
            string ItemGUID = Request["ItemGUID"].ToString();
            ///////////// requried when paging needs in this method /////////////////
            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"].ToString();

            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());

            // set the default column sorting here, if first time then required to set 
            //if (sortColumnName == "0" || sortColumnName == "undefined" || sortColumnName == "ShippingMethod")
            //    sortColumnName = "ID";

            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined") || sortColumnName == "ShippingMethod")
                    sortColumnName = "ID";
            }
            else
                sortColumnName = "ID";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            ///////////// requried when paging needs in this method /////////////////
            ViewBag.ItemGUID = ItemGUID;
            ItemMasterDAL objItemAPI = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            Guid ItemGUID1 = Guid.Empty;
            Guid.TryParse(ItemGUID, out ItemGUID1);
            var Objitem = objItemAPI.GetItemWithoutJoins(null, ItemGUID1);
            ViewBag.ItemID = Objitem.ID;
            ItemLocationQTYDAL objAPI = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName);
            int TotalRecordCount = 0;

            var objModel = objAPI.GetBinsByItemDBWithTransfer(Objitem.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, sortColumnName, false);
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<ItemQuantityDetail> lstItemQtyDetail = objCommonDAL.GetOnOrderDetailFromCache(Objitem.GUID, SessionHelper.RoomID, SessionHelper.CompanyID);
            var result = from u in objModel
                         select new ItemLocationQTYDTO
                         {
                             ID = u.ID,
                             BinID = u.BinID,
                             CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                             ConsignedQuantity = u.ConsignedQuantity,
                             Quantity = u.Quantity,
                             LotNumber = u.LotNumber,
                             GUID = u.GUID,
                             ItemGUID = u.ItemGUID,
                             Created = u.Created,
                             LastUpdated = u.LastUpdated,
                             CreatedByName = u.CreatedByName,
                             UpdatedByName = u.UpdatedByName,
                             Room = u.Room,
                             CompanyID = u.CompanyID,
                             BinNumber = u.BinNumber,
                             ItemNumber = u.ItemNumber,
                             CriticalQuantity = u.CriticalQuantity,
                             MaximumQuantity = u.MaximumQuantity,
                             MinimumQuantity = u.MinimumQuantity,
                             SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                             Cost = u.Cost,
                             Markup = u.Markup,
                             SellPrice = u.SellPrice,
                             Averagecost = u.Averagecost,
                             ExtendedCost = u.ExtendedCost,
                             CostUOMID = u.CostUOMID,
                             eVMISensorID = u.eVMISensorID,
                             eVMISensorPort = u.eVMISensorPort,
                             CostUOMName = u.CostUOMName,
                             IsItemLevelMinMaxQtyRequired = u.IsItemLevelMinMaxQtyRequired,
                             CreatedDate = CommonUtility.ConvertDateByTimeZone(u.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             UpdatedDate = CommonUtility.ConvertDateByTimeZone(u.LastUpdated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             AddedFrom = u.AddedFrom,
                             EditedFrom = u.EditedFrom,
                             ReceivedOn = u.ReceivedOn,
                             ReceivedOnWeb = u.ReceivedOnWeb,
                             OnOrderQty = Convert.ToDecimal(lstItemQtyDetail.Where(l => l.BinNumber == u.BinNumber).Sum(l => l.qty)),
                             SuggestedTransferQuantity = u.SuggestedTransferQuantity,
                             SerialNumberTracking = u.SerialNumberTracking,
                             LotNumberTracking = u.LotNumberTracking,
                             BinUDF1 = u.BinUDF1,
                             BinUDF2 = u.BinUDF2,
                             BinUDF3 = u.BinUDF3,
                             BinUDF4 = u.BinUDF4,
                             BinUDF5 = u.BinUDF5,
                         };
            //Guid deGUID = new Guid();
            //if (objModel != null)
            //    TotalRecordCount = objModel.Count;

            //foreach (var mod in result)
            //{
            //    var Modbin = objItemAPI.GetExtCostAndAvgCostLocationWise(mod.ItemGUID ?? deGUID, Convert.ToInt64(mod.BinID), Convert.ToInt64(mod.Room), Convert.ToInt64(mod.CompanyID));
            //    mod.Cost = Modbin.Cost;
            //    mod.ExtendedCost = Modbin.ExtCost;
            //    mod.Averagecost = Modbin.AvgCost;
            //    mod.SellPrice = Modbin.SellPrice;
            //    mod.Markup = Modbin.Markup;
            //    mod.SuggestedOrderQuantity = objItemAPI.GetSuggestedOrderQtyByBinID(mod.ItemGUID ?? deGUID, Convert.ToInt64(mod.BinID));
            //    if (mod.Markup > 0)
            //    {
            //        decimal? d = (mod.Cost * mod.Markup) / 100;
            //        mod.SellPrice = mod.Cost + Convert.ToDecimal(d);
            //    }
            //    else
            //        mod.SellPrice = mod.Cost;
            //}
            TotalRecordCount = result.Count();
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = result.Count(),
                aaData = result.Skip(param.iDisplayStart).Take(PageSize),
            }, JsonRequestBehavior.AllowGet);
        }



        //ItemBinWiseSummaryListAjax
        public ActionResult ItemBinWiseSummaryListByRoomAndCompanyAjax(JQueryDataTableParamModel param)
        {
            string ItemGUID = Request["ItemGUID"].ToString();
            ///////////// requried when paging needs in this method /////////////////
            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"].ToString();

            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());

            // set the default column sorting here, if first time then required to set 
            //if (sortColumnName == "0" || sortColumnName == "undefined" || sortColumnName == "ShippingMethod")
            //    sortColumnName = "ID";

            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined") || sortColumnName == "ShippingMethod")
                    sortColumnName = "ID";
            }
            else
                sortColumnName = "ID";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            ///////////// requried when paging needs in this method /////////////////
            ViewBag.ItemGUID = ItemGUID;
            ItemMasterDAL objItemAPI = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            Guid ItemGUID1 = Guid.Empty;
            Guid.TryParse(ItemGUID, out ItemGUID1);
            var Objitem = objItemAPI.GetItemWithoutJoins(null, ItemGUID1);
            ViewBag.ItemID = Objitem.ID;
            ItemLocationQTYDAL objAPI = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName);
            int TotalRecordCount = 0;

            var objModel = objAPI.GetBinsByItemDBWithTransfer(Objitem.GUID, Objitem.Room ?? 0, Objitem.CompanyID ?? 0, sortColumnName, false);
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<ItemQuantityDetail> lstItemQtyDetail = objCommonDAL.GetOnOrderDetailFromCache(Objitem.GUID, Objitem.Room ?? 0, Objitem.CompanyID ?? 0);
            var result = from u in objModel
                         select new ItemLocationQTYDTO
                         {
                             ID = u.ID,
                             BinID = u.BinID,
                             CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                             ConsignedQuantity = u.ConsignedQuantity,
                             Quantity = u.Quantity,
                             LotNumber = u.LotNumber,
                             GUID = u.GUID,
                             ItemGUID = u.ItemGUID,
                             Created = u.Created,
                             LastUpdated = u.LastUpdated,
                             CreatedByName = u.CreatedByName,
                             UpdatedByName = u.UpdatedByName,
                             Room = u.Room,
                             CompanyID = u.CompanyID,
                             BinNumber = u.BinNumber,
                             ItemNumber = u.ItemNumber,
                             CriticalQuantity = u.CriticalQuantity,
                             MaximumQuantity = u.MaximumQuantity,
                             MinimumQuantity = u.MinimumQuantity,
                             SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                             Cost = u.Cost,
                             Markup = u.Markup,
                             SellPrice = u.SellPrice,
                             Averagecost = u.Averagecost,
                             ExtendedCost = u.ExtendedCost,
                             CostUOMID = u.CostUOMID,
                             eVMISensorID = u.eVMISensorID,
                             eVMISensorPort = u.eVMISensorPort,
                             CostUOMName = u.CostUOMName,
                             IsItemLevelMinMaxQtyRequired = u.IsItemLevelMinMaxQtyRequired,
                             CreatedDate = CommonUtility.ConvertDateByTimeZone(u.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             UpdatedDate = CommonUtility.ConvertDateByTimeZone(u.LastUpdated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             AddedFrom = u.AddedFrom,
                             EditedFrom = u.EditedFrom,
                             ReceivedOn = u.ReceivedOn,
                             ReceivedOnWeb = u.ReceivedOnWeb,
                             OnOrderQty = Convert.ToDecimal(lstItemQtyDetail.Where(l => l.BinNumber == u.BinNumber).Sum(l => l.qty)),
                             SuggestedTransferQuantity = u.SuggestedTransferQuantity,
                             SerialNumberTracking = u.SerialNumberTracking,
                             LotNumberTracking = u.LotNumberTracking
                         };
            TotalRecordCount = result.Count();
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = result.Count(),
                aaData = result.Skip(param.iDisplayStart).Take(PageSize),
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region "Item Locations"

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ItemLocations(Int64 ItemID, string ItemGUID)
        {
            ViewBag.ItemID = ItemID;
            ViewBag.ItemGUID = ItemGUID;

            Guid? OrderDetailGUID = null;

            ItemMasterDAL objItemAPI = new ItemMasterDAL(SessionHelper.EnterPriseDBName);

            //var Objitem = objItemAPI.GetRecord(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
            //if (Objitem == null)
            //    Objitem = objItemAPI.GetRecord(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, true, true);
            Guid ItemGUID1 = Guid.Empty;
            Guid.TryParse(ItemGUID, out ItemGUID1);
            var Objitem = objItemAPI.GetItemWithoutJoins(null, ItemGUID1);

            ItemLocationDetailsDAL objAPI = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            var objModel = objAPI.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, Guid.Parse(ItemGUID), OrderDetailGUID, "BinNumber Asc");

            ViewBag.IsSerialNumberTracking = Objitem.SerialNumberTracking;
            ViewBag.IsLotNumberTracking = Objitem.LotNumberTracking;
            ViewBag.IsDateCodeTracking = Objitem.DateCodeTracking;
            ViewBag.ItemNumber = Objitem.ItemNumber;
            ViewBag.Consignment = Objitem.Consignment;
            ViewBag.IsItemLevelMinMaxQtyRequired = Objitem.IsItemLevelMinMaxQtyRequired;
            ViewBag.ItemGUID_ItemType = ItemGUID + "#" + Objitem.ItemType;
            return RenderRazorViewToString("_ItemLocations", objModel);
        }

        public string ItemLocations2(Int64 BinID, string ItemGUID)
        {
            ViewBag.ItemGUID = ItemGUID;
            ViewBag.BinID = BinID;

            Guid? OrderDetailGUID = null;

            ItemMasterDAL objItemAPI = new ItemMasterDAL(SessionHelper.EnterPriseDBName);

            //var Objitem = objItemAPI.GetRecord(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
            //if (Objitem == null)
            //    Objitem = objItemAPI.GetRecord(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, true, true);
            Guid itemGUID1 = Guid.Empty;
            Guid.TryParse(ItemGUID, out itemGUID1);
            var Objitem = objItemAPI.GetItemWithoutJoins(null, itemGUID1);
            ItemLocationDetailsDAL objAPI = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            var objModel = objAPI.GetAllRecordsBinWise(BinID, SessionHelper.RoomID, SessionHelper.CompanyID, Guid.Parse(ItemGUID), OrderDetailGUID, "ID DESC");

            foreach (var mod in objModel)
            {
                if (Objitem.Markup > 0)
                {
                    mod.Markup = (mod.Cost * Objitem.Markup) / 100;
                    double? d = (mod.Cost * Objitem.Markup) / 100;
                    mod.SellPrice = mod.Cost + d;
                }
                else
                {
                    mod.Markup = 0;
                    mod.SellPrice = mod.Cost;

                }
            }

            ViewBag.IsSerialNumberTracking = Objitem.SerialNumberTracking;
            ViewBag.IsLotNumberTracking = Objitem.LotNumberTracking;
            ViewBag.IsDateCodeTracking = Objitem.DateCodeTracking;
            ViewBag.ItemNumber = Objitem.ItemNumber;
            ViewBag.Consignment = Objitem.Consignment;
            ViewBag.IsItemLevelMinMaxQtyRequired = Objitem.IsItemLevelMinMaxQtyRequired;
            ViewBag.ItemGUID_ItemType = ItemGUID + "#" + Objitem.ItemType;

            bool IsConsignedEditable = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowChangeConsignedItems);

            if (!IsConsignedEditable && Objitem.Consignment)
            {
                ViewBag.ViewOnly = true;
            }
            else
            {
                ViewBag.ViewOnly = false;
            }

            return RenderRazorViewToString("_ItemLocations", objModel);
        }

        public string ItemLocationsByRoomAndCompany(Int64 BinID, string ItemGUID)
        {
            ViewBag.ItemGUID = ItemGUID;
            ViewBag.BinID = BinID;

            Guid? OrderDetailGUID = null;

            ItemMasterDAL objItemAPI = new ItemMasterDAL(SessionHelper.EnterPriseDBName);

            //var Objitem = objItemAPI.GetRecord(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
            //if (Objitem == null)
            //    Objitem = objItemAPI.GetRecord(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, true, true);
            Guid itemGUID1 = Guid.Empty;
            Guid.TryParse(ItemGUID, out itemGUID1);
            var Objitem = objItemAPI.GetItemWithoutJoins(null, itemGUID1);
            ItemLocationDetailsDAL objAPI = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            var objModel = objAPI.GetAllRecordsBinWise(BinID, Objitem.Room ?? 0, Objitem.CompanyID ?? 0, Guid.Parse(ItemGUID), OrderDetailGUID, "ID DESC");

            foreach (var mod in objModel)
            {
                if (Objitem.Markup > 0)
                {
                    mod.Markup = (mod.Cost * Objitem.Markup) / 100;
                    double? d = (mod.Cost * Objitem.Markup) / 100;
                    mod.SellPrice = mod.Cost + d;
                }
                else
                {
                    mod.Markup = 0;
                    mod.SellPrice = mod.Cost;

                }
            }

            ViewBag.IsSerialNumberTracking = Objitem.SerialNumberTracking;
            ViewBag.IsLotNumberTracking = Objitem.LotNumberTracking;
            ViewBag.IsDateCodeTracking = Objitem.DateCodeTracking;
            ViewBag.ItemNumber = Objitem.ItemNumber;
            ViewBag.Consignment = Objitem.Consignment;
            ViewBag.IsItemLevelMinMaxQtyRequired = Objitem.IsItemLevelMinMaxQtyRequired;
            ViewBag.ItemGUID_ItemType = ItemGUID + "#" + Objitem.ItemType;

            bool IsConsignedEditable = eTurnsWeb.Helper.SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowChangeConsignedItems);

            if (!IsConsignedEditable && Objitem.Consignment)
            {
                ViewBag.ViewOnly = true;
            }
            else
            {
                ViewBag.ViewOnly = false;
            }

            return RenderRazorViewToString("_ItemLocationsByRoomAndCompany", objModel);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ItemPictureView(string ItemGUID, bool IsArchivedRecord, bool IsDeletedRecord)
        {
            //ItemMasterController objItemAPI = new ItemMasterController();
            ItemMasterDAL objItemAPI = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            Guid itemGUID1 = Guid.Empty;
            Guid.TryParse(ItemGUID, out itemGUID1);
            var Objitem = objItemAPI.GetItemWithMasterTableJoins(null, itemGUID1, SessionHelper.RoomID, SessionHelper.CompanyID);

            if (!string.IsNullOrWhiteSpace(Objitem.ItemImageExternalURL))
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Objitem.ItemImageExternalURL);
                    request.Method = "GET";
                    try
                    {
                        WebResponse response = request.GetResponse();
                        if (response.ContentType.ToString().IndexOf("image") >= 0)
                        {

                        }
                        else
                        {
                            Objitem.ItemImageExternalURL = string.Empty;
                        }
                    }
                    catch
                    {
                        Objitem.ItemImageExternalURL = string.Empty;
                    }
                }
                catch
                {
                    Objitem.ItemImageExternalURL = string.Empty;
                }
            }
            //if (!string.IsNullOrWhiteSpace(Objitem.ItemLink2ExternalURL))
            //{
            //    try
            //    {
            //        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Objitem.ItemLink2ExternalURL);
            //        request.Method = "GET";
            //        try
            //        {
            //            WebResponse response = request.GetResponse();
            //            if (response.ContentType.ToString().IndexOf("image") >= 0)
            //            {

            //            }
            //            else
            //            {
            //                Objitem.ItemLink2ExternalURL = string.Empty;
            //            }
            //        }
            //        catch
            //        {
            //            Objitem.ItemLink2ExternalURL = string.Empty;
            //        }
            //    }
            //    catch
            //    {
            //        Objitem.ItemLink2ExternalURL = string.Empty;
            //    }
            //}
            #region "Get Cust and Cons QTY"
            ItemLocationQTYDAL objQTYDAL = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName);
            var QtyDATA = objQTYDAL.GetRecordByItem(Guid.Parse(ItemGUID), SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            if (QtyDATA != null && QtyDATA.Count > 0)
            {
                Objitem.CustomerOwnedQuantity = QtyDATA.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0));
                Objitem.ConsignedQuantity = QtyDATA.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0));
            }
            Objitem.AverageUsage = Objitem.AverageUsage ?? 0;
            Objitem.Turns = Objitem.Turns ?? 0;
            #endregion

            return RenderRazorViewToString("_ItemPictureView", Objitem);
        }

        public string ItemPictureByRoomAndCompanyView(string ItemGUID, int RoomID, int CompanyID, bool IsArchivedRecord, bool IsDeletedRecord)
        {
            //ItemMasterController objItemAPI = new ItemMasterController();
            ItemMasterDAL objItemAPI = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            Guid itemGUID1 = Guid.Empty;
            Guid.TryParse(ItemGUID, out itemGUID1);
            var Objitem = objItemAPI.GetItemWithMasterTableJoins(null, itemGUID1, RoomID, CompanyID);

            if (!string.IsNullOrWhiteSpace(Objitem.ItemImageExternalURL))
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Objitem.ItemImageExternalURL);
                    request.Method = "GET";
                    try
                    {
                        WebResponse response = request.GetResponse();
                        if (response.ContentType.ToString().IndexOf("image") >= 0)
                        {

                        }
                        else
                        {
                            Objitem.ItemImageExternalURL = string.Empty;
                        }
                    }
                    catch
                    {
                        Objitem.ItemImageExternalURL = string.Empty;
                    }
                }
                catch
                {
                    Objitem.ItemImageExternalURL = string.Empty;
                }
            }
            #region "Get Cust and Cons QTY"
            ItemLocationQTYDAL objQTYDAL = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName);
            var QtyDATA = objQTYDAL.GetRecordByItem(Guid.Parse(ItemGUID), RoomID, CompanyID).ToList();
            if (QtyDATA != null && QtyDATA.Count > 0)
            {
                Objitem.CustomerOwnedQuantity = QtyDATA.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault(0));
                Objitem.ConsignedQuantity = QtyDATA.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0));
            }
            Objitem.AverageUsage = Objitem.AverageUsage ?? 0;
            Objitem.Turns = Objitem.Turns ?? 0;
            #endregion

            return RenderRazorViewToString("_ItemPictureByRoomAndCompanyView", Objitem);
        }

        public ActionResult ItemLocationsListAjax(JQueryDataTableParamModel param)
        {
            string ItemGUID = Request["ItemGUID"].ToString();
            Int64 BinID = Convert.ToInt64(Request["BinID"].ToString());
            Guid? OrderDetailGUID = null;
            if (!string.IsNullOrEmpty(Request["OrderDetailGUID"]) && Request["OrderDetailGUID"].Trim().Length > 0)
                OrderDetailGUID = Guid.Parse(Request["OrderDetailGUID"]);


            ///////////// requried when paging needs in this method /////////////////
            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"].ToString();

            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());


            // set the default column sorting here, if first time then required to set 
            //if (sortColumnName == "0" || sortColumnName == "undefined" || sortColumnName == "ShippingMethod")
            //    sortColumnName = "ID";
            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined") || sortColumnName == "ShippingMethod")
                    sortColumnName = "ID";
            }
            else
                sortColumnName = "ID";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            ///////////// requried when paging needs in this method /////////////////

            ViewBag.ItemGUID = ItemGUID;
            ItemMasterDAL objItemAPI = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            Guid ItemGUID1 = Guid.Empty;
            Guid.TryParse(ItemGUID, out ItemGUID1);
            var Objitem = objItemAPI.GetItemWithoutJoins(null, ItemGUID1);


            ViewBag.ItemID = Objitem.ID;

            if (Objitem.IsItemLevelMinMaxQtyRequired == null)
            {
                ViewBag.IsItemLevelMinMaxQtyRequired = false;
            }
            else
            {
                ViewBag.IsItemLevelMinMaxQtyRequired = Objitem.IsItemLevelMinMaxQtyRequired;
            }

            ItemLocationDetailsDAL objAPI = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);

            int TotalRecordCount = 0;

            List<ItemLocationDetailsDTO> objModel = objAPI.GetPagedRecords_NoCache(BinID, param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, Guid.Parse(ItemGUID), OrderDetailGUID).ToList();

            var result = from u in objModel.ToList()
                         select new ItemLocationDetailsDTO
                         {
                             ID = u.ID,
                             BinID = u.BinID,
                             CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                             ConsignedQuantity = u.ConsignedQuantity,
                             MeasurementID = u.MeasurementID,
                             LotNumber = u.LotNumber,
                             SerialNumber = u.SerialNumber,
                             Expiration = u.Expiration,
                             //ExpirationDate = u.ExpirationDate.GetValueOrDefault(DateTime.MinValue) == DateTime.MinValue ? Convert.ToDateTime(DateTime.Now.ToString(SessionHelper.DateTimeFormat)) : u.ExpirationDate,
                             //ExpirationStr = u.ExpirationDate.GetValueOrDefault(DateTime.MinValue) == DateTime.MinValue ? DateTime.Now.ToString(SessionHelper.DateTimeFormat) : Convert.ToDateTime(u.ExpirationDate).ToString(SessionHelper.DateTimeFormat),
                             //ReceivedDate = u.ReceivedDate,
                             //ReceivedDateStr = u.ReceivedDate.GetValueOrDefault(DateTime.MinValue) == DateTime.MinValue ? DateTime.Now.ToString(SessionHelper.DateTimeFormat) : Convert.ToDateTime(u.ReceivedDate).ToString(SessionHelper.DateTimeFormat),
                             ExpirationDate = u.ExpirationDate,
                             ExpirationStr = u.ExpirationDate.GetValueOrDefault(DateTime.MinValue) == DateTime.MinValue ? string.Empty : Convert.ToDateTime(u.ExpirationDate).ToString(SessionHelper.RoomDateFormat),
                             ReceivedDate = u.ReceivedDate,
                             //ReceivedDateStr = u.ReceivedDate.GetValueOrDefault(DateTime.MinValue) == DateTime.MinValue ? string.Empty : Convert.ToDateTime(u.ReceivedDate).ToString(SessionHelper.DateTimeFormat),
                             ReceivedDateStr = CommonUtility.ConvertDateByTimeZone(u.ReceivedDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             Received = u.Received,
                             Cost = u.Cost,
                             Markup = u.Markup,
                             SellPrice = u.SellPrice,
                             eVMISensorPort = u.eVMISensorPort,
                             eVMISensorID = u.eVMISensorID,
                             GUID = u.GUID,
                             ItemGUID = u.ItemGUID,
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
                             BinNumber = u.BinNumber,
                             ItemNumber = u.ItemNumber,
                             SerialNumberTracking = u.SerialNumberTracking,
                             LotNumberTracking = u.LotNumberTracking,
                             DateCodeTracking = u.DateCodeTracking,
                             OrderDetailGUID = u.OrderDetailGUID,
                             TransferDetailGUID = u.TransferDetailGUID,
                             KitDetailGUID = u.KitDetailGUID,
                             CriticalQuantity = u.CriticalQuantity,
                             MinimumQuantity = u.MinimumQuantity,
                             MaximumQuantity = u.MaximumQuantity,
                             SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                             CreatedDate = CommonUtility.ConvertDateByTimeZone(u.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             UpdatedDate = CommonUtility.ConvertDateByTimeZone(u.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             ReceivedOn = u.ReceivedOn,
                             ReceivedOnWeb = u.ReceivedOnWeb,
                             AddedFrom = u.AddedFrom,
                             EditedFrom = u.EditedFrom,
                             ReceivedOnDate = CommonUtility.ConvertDateByTimeZone(u.ReceivedOn, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             ReceivedOnDateWeb = CommonUtility.ConvertDateByTimeZone(u.ReceivedOnWeb, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                         };

            foreach (var mod in objModel)
            {
                if (Objitem.Markup > 0)
                {
                    // mod.Markup = (mod.Cost * Objitem.Markup) / 100;
                    double? d = (mod.Cost * Objitem.Markup) / 100;
                    mod.SellPrice = mod.Cost + d;
                }
                else
                {
                    mod.Markup = 0;
                    mod.SellPrice = mod.Cost;

                }
            }


            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);
        }
        public ActionResult ItemLocationsListByRoomAndCompanyAjax(JQueryDataTableParamModel param)
        {
            string ItemGUID = Request["ItemGUID"].ToString();
            Int64 BinID = Convert.ToInt64(Request["BinID"].ToString());
            Guid? OrderDetailGUID = null;
            if (!string.IsNullOrEmpty(Request["OrderDetailGUID"]) && Request["OrderDetailGUID"].Trim().Length > 0)
                OrderDetailGUID = Guid.Parse(Request["OrderDetailGUID"]);


            ///////////// requried when paging needs in this method /////////////////
            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"].ToString();

            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());


            // set the default column sorting here, if first time then required to set 
            //if (sortColumnName == "0" || sortColumnName == "undefined" || sortColumnName == "ShippingMethod")
            //    sortColumnName = "ID";
            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined") || sortColumnName == "ShippingMethod")
                    sortColumnName = "ID";
            }
            else
                sortColumnName = "ID";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            ///////////// requried when paging needs in this method /////////////////

            ViewBag.ItemGUID = ItemGUID;
            ItemMasterDAL objItemAPI = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            Guid ItemGUID1 = Guid.Empty;
            Guid.TryParse(ItemGUID, out ItemGUID1);
            var Objitem = objItemAPI.GetItemWithoutJoins(null, ItemGUID1);


            ViewBag.ItemID = Objitem.ID;

            if (Objitem.IsItemLevelMinMaxQtyRequired == null)
            {
                ViewBag.IsItemLevelMinMaxQtyRequired = false;
            }
            else
            {
                ViewBag.IsItemLevelMinMaxQtyRequired = Objitem.IsItemLevelMinMaxQtyRequired;
            }

            ItemLocationDetailsDAL objAPI = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);

            int TotalRecordCount = 0;

            List<ItemLocationDetailsDTO> objModel = objAPI.GetPagedRecords_NoCache(BinID, param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, Objitem.Room ?? 0, Objitem.CompanyID ?? 0, IsArchived, IsDeleted, Guid.Parse(ItemGUID), OrderDetailGUID).ToList();

            var result = from u in objModel.ToList()
                         select new ItemLocationDetailsDTO
                         {
                             ID = u.ID,
                             BinID = u.BinID,
                             CustomerOwnedQuantity = u.CustomerOwnedQuantity,
                             ConsignedQuantity = u.ConsignedQuantity,
                             MeasurementID = u.MeasurementID,
                             LotNumber = u.LotNumber,
                             SerialNumber = u.SerialNumber,
                             Expiration = u.Expiration,
                             //ExpirationDate = u.ExpirationDate.GetValueOrDefault(DateTime.MinValue) == DateTime.MinValue ? Convert.ToDateTime(DateTime.Now.ToString(SessionHelper.DateTimeFormat)) : u.ExpirationDate,
                             //ExpirationStr = u.ExpirationDate.GetValueOrDefault(DateTime.MinValue) == DateTime.MinValue ? DateTime.Now.ToString(SessionHelper.DateTimeFormat) : Convert.ToDateTime(u.ExpirationDate).ToString(SessionHelper.DateTimeFormat),
                             //ReceivedDate = u.ReceivedDate,
                             //ReceivedDateStr = u.ReceivedDate.GetValueOrDefault(DateTime.MinValue) == DateTime.MinValue ? DateTime.Now.ToString(SessionHelper.DateTimeFormat) : Convert.ToDateTime(u.ReceivedDate).ToString(SessionHelper.DateTimeFormat),
                             ExpirationDate = u.ExpirationDate,
                             ExpirationStr = u.ExpirationDate.GetValueOrDefault(DateTime.MinValue) == DateTime.MinValue ? string.Empty : Convert.ToDateTime(u.ExpirationDate).ToString(SessionHelper.RoomDateFormat),
                             ReceivedDate = u.ReceivedDate,
                             //ReceivedDateStr = u.ReceivedDate.GetValueOrDefault(DateTime.MinValue) == DateTime.MinValue ? string.Empty : Convert.ToDateTime(u.ReceivedDate).ToString(SessionHelper.DateTimeFormat),
                             ReceivedDateStr = CommonUtility.ConvertDateByTimeZone(u.ReceivedDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             Received = u.Received,
                             Cost = u.Cost,
                             Markup = u.Markup,
                             SellPrice = u.SellPrice,
                             eVMISensorPort = u.eVMISensorPort,
                             eVMISensorID = u.eVMISensorID,
                             GUID = u.GUID,
                             ItemGUID = u.ItemGUID,
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
                             BinNumber = u.BinNumber,
                             ItemNumber = u.ItemNumber,
                             SerialNumberTracking = u.SerialNumberTracking,
                             LotNumberTracking = u.LotNumberTracking,
                             DateCodeTracking = u.DateCodeTracking,
                             OrderDetailGUID = u.OrderDetailGUID,
                             TransferDetailGUID = u.TransferDetailGUID,
                             KitDetailGUID = u.KitDetailGUID,
                             CriticalQuantity = u.CriticalQuantity,
                             MinimumQuantity = u.MinimumQuantity,
                             MaximumQuantity = u.MaximumQuantity,
                             SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                             CreatedDate = CommonUtility.ConvertDateByTimeZone(u.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             UpdatedDate = CommonUtility.ConvertDateByTimeZone(u.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             ReceivedOn = u.ReceivedOn,
                             ReceivedOnWeb = u.ReceivedOnWeb,
                             AddedFrom = u.AddedFrom,
                             EditedFrom = u.EditedFrom,
                             ReceivedOnDate = CommonUtility.ConvertDateByTimeZone(u.ReceivedOn, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             ReceivedOnDateWeb = CommonUtility.ConvertDateByTimeZone(u.ReceivedOnWeb, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                         };

            foreach (var mod in objModel)
            {
                if (Objitem.Markup > 0)
                {
                    // mod.Markup = (mod.Cost * Objitem.Markup) / 100;
                    double? d = (mod.Cost * Objitem.Markup) / 100;
                    mod.SellPrice = mod.Cost + d;
                }
                else
                {
                    mod.Markup = 0;
                    mod.SellPrice = mod.Cost;

                }
            }


            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);
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

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public string ItemLocationsDelete(string ids)
        {
            try
            {
                long SessionUserId = SessionHelper.UserID;
                ItemLocationDetailsDAL obj = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                if (obj.DeleteRecords(ids, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, "Inventory >> Delete ItemLocation", SessionUserId) == true)
                {
                    return ResCommon.Ok;
                }
                else
                {
                    return ResCommon.NotOk;
                }
                //return "not ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public JsonResult SaveLocationsDetails(List<ItemLocationDetailsDTO> obj)
        {
            return null;
        }

        public string DuplicateCheckSrNumber(string SrNumber, int ID, Guid? ItemGUID)
        {
            CommonDAL objCDal = new CommonDAL(SessionHelper.EnterPriseDBName);
            //string ActionMode = "";
            //if (ID > 0)
            //{
            //    ActionMode = "edit";
            //}
            //else
            //{
            //    ActionMode = "add";
            //}

            return objCDal.CheckDuplicateSerialNumbers(SrNumber, ID, SessionHelper.RoomID, SessionHelper.CompanyID, ItemGUID ?? Guid.Empty);
            //return objCDal.DuplicateCheck(SrNumber, ActionMode, ID, "ItemLocationDetails", "SerialNumber", SessionHelper.RoomID, SessionHelper.CompanyID);
        }

        public string DuplicateCheckSrNumberforCreditPull(string SrNumber, int ID, bool IsConsignment)
        {
            string ExtraWhereCond = "";
            //if(IsConsignment)
            //    ExtraWhereCond = " AND isnull(ConsignedQuantity,0) = 0 AND IsDeleted = 0 AND IsArchived = 0 ";
            //else
            //    ExtraWhereCond = " AND ISNULL(CustomerOwnedQuantity,0) = 0 AND IsDeleted = 0 AND IsArchived = 0 ";

            ExtraWhereCond = " AND (ISNULL(CustomerOwnedQuantity,0) = 0 AND isnull(ConsignedQuantity,0) = 0)  AND IsDeleted = 0 AND IsArchived = 0 ";

            string ActionMode = "";
            if (ID > 0)
            {
                ActionMode = "edit";
            }
            else
            {
                ActionMode = "add";
            }
            CommonDAL objCDal = new CommonDAL(SessionHelper.EnterPriseDBName);
            return objCDal.DuplicateCheckForCreditPull(SrNumber, ActionMode, ID, "ItemLocationDetails", "SerialNumber", SessionHelper.RoomID, SessionHelper.CompanyID, ExtraWhereCond);
        }

        public string DuplicateCheckLotNumber(string LotNumber, int ID)
        {
            string ActionMode = "";
            if (ID > 0)
            {
                ActionMode = "edit";
            }
            else
            {
                ActionMode = "add";
            }
            CommonDAL objCDal = new CommonDAL(SessionHelper.EnterPriseDBName);
            return objCDal.DuplicateCheck(LotNumber, ActionMode, ID, "ItemLocationDetails", "LotNumber", SessionHelper.RoomID, SessionHelper.CompanyID);
        }

        public string DuplicateCheckUPC(string UPC, int ID)
        {
            string ActionMode = "";
            if (ID > 0)
            {
                ActionMode = "edit";
            }
            else
            {
                ActionMode = "add";
            }
            CommonDAL objCDal = new CommonDAL(SessionHelper.EnterPriseDBName);
            return objCDal.DuplicateCheck(UPC, ActionMode, ID, "ItemMaster", "UPC", SessionHelper.RoomID, SessionHelper.CompanyID);
        }

        public ActionResult ItemLocationHistoryView(Guid LGUID)
        {
            ViewBag.LocationDGUID = LGUID;
            return PartialView("_ItemLocation_History");
        }

        public ActionResult GetHistoryDataForLocationDetails(JQueryDataTableParamModel param)
        {

            string dateFormat = "MM/dd/yyyy";
            //if (SessionHelper.CompanyConfig != null && !string.IsNullOrEmpty(SessionHelper.CompanyConfig.DateFormatCSharp))
            //    dateFormat = SessionHelper.CompanyConfig.DateFormatCSharp;

            if (string.IsNullOrEmpty(SessionHelper.DateTimeFormat))
                dateFormat = SessionHelper.DateTimeFormat;
            int PageIndex = 0;
            if (param.sEcho == null)
                PageIndex = 0;
            else
                PageIndex = int.Parse(param.sEcho);

            int PageSize = 100; //param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = "HistoryID";
            string sDirection = "DESC";
            int StartWith = (PageSize - param.iDisplayStart) + 1;
            int TotalRecordCount = 0;
            Guid LGUID = Guid.Parse(Request["PageID"].ToString());
            //TotalRecordCount = 0;
            var RoomDateFormat = SessionHelper.RoomDateFormat;
            var CurrentTimeZone = SessionHelper.CurrentTimeZone;
            var DateTimeFormat = SessionHelper.DateTimeFormat;
            var RoomCulture = SessionHelper.RoomCulture;
            var itemLocationDetailsDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            var DataFromDB = itemLocationDetailsDAL.GetItemLocationDetailsHistoryByGuidFull(LGUID);

            if (DataFromDB != null && DataFromDB.Any())
            {
                TotalRecordCount = DataFromDB.Count();
                DataFromDB.ForEach(u =>
                {
                    u.Expiration = u.ExpirationDate.GetValueOrDefault(DateTime.MinValue) == DateTime.MinValue ? string.Empty : Convert.ToDateTime(u.ExpirationDate).ToString(SessionHelper.RoomDateFormat);
                    u.Received = CommonUtility.ConvertDateByTimeZone(u.ReceivedDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                });
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

        #endregion

        #region "Bin Replanishment"

        /// <summary>
        /// Load BinReplanish
        /// </summary>
        /// <param name="ItemGUID"></param>
        /// <returns></returns>
        public ActionResult LoadLocationsofItem(Guid ItemGUID, int? AddCount)
        {
            ViewBag.ItemGUID = ItemGUID;
            BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            //ViewBag.DefaultLocationBag = objBinMasterDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(t => t.IsStagingLocation != true);
            string itemGuidString = Convert.ToString(ItemGUID);
            List<BinMasterDTO> lstBinReplanish = new List<BinMasterDTO>();
            List<ComPortRoomMappingDTO> lstmappings = new List<ComPortRoomMappingDTO>();
            if ((SessionHelper.isEVMI ?? false))
            {
                lstmappings = new ComPortRoomMappingDAL(SessionHelper.EnterPriseDBName).GetComPortMappingByCompanyRoomID(SessionHelper.CompanyID, SessionHelper.RoomID);
            }
            List<SelectListItem> lstComMappings = new List<SelectListItem>();
            foreach (var item in lstmappings)
            {
                lstComMappings.Add(new SelectListItem() { Text = item.ComPortName, Value = item.ComPortName });
            }
            ViewBag.RoomCOMMappings = lstComMappings;

            if (Session["BinReplanish" + itemGuidString] != null)
            {
                lstBinReplanish = (List<BinMasterDTO>)Session["BinReplanish" + itemGuidString];
            }
            else
            {
                RoomDAL objRoomDal = new RoomDAL(SessionHelper.EnterPriseDBName);
                BinMasterDTO objBinDTO = new BinMasterDTO();
                RoomDTO ROOMDTO = objRoomDal.GetRoomByIDFull(SessionHelper.RoomID);
                if (ROOMDTO != null && !string.IsNullOrWhiteSpace(ROOMDTO.DefaultBinName))
                {
                    BinMasterDTO objBinMasterDTO = objBinMasterDAL.GetBinByID(ROOMDTO.DefaultBinID ?? 0, SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (objBinMasterDTO == null)
                    {
                        objBinMasterDTO = objBinMasterDAL.InsertRoomDefaultBin(ROOMDTO.DefaultBinName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                    }

                    objBinMasterDTO.CreatedBy = SessionHelper.UserID;
                    objBinMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                    objBinMasterDTO.GUID = Guid.NewGuid();
                    objBinMasterDTO.IsDefault = true;
                    objBinMasterDTO.Created = DateTimeUtility.DateTimeNow;
                    objBinMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objBinMasterDTO.IsOnlyFromItemUI = true;
                    //if (Convert.ToInt64(ROOMDTO.DefaultBinID) > 0)
                    //{
                    //    objBinDTO = objBinMasterDAL.GetRecord(Convert.ToInt64(ROOMDTO.DefaultBinID), SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                    //}
                    // List<ItemLocationLevelQuanityDTO> lstBinReplanish = null;
                    //BinMasterDTO objDTO = new BinMasterDTO();
                    lstBinReplanish = new List<BinMasterDTO>();
                    //objDTO.ID = ROOMDTO.DefaultBinID ?? 0;
                    //// objDTO.ID = ROOMDTO.DefaultBinID.Value;
                    //objDTO.BinNumber = ROOMDTO.DefaultBinName;
                    //objDTO.CriticalQuantity = 0;
                    //objDTO.MinimumQuantity = 0;
                    //objDTO.MaximumQuantity = 0;
                    //objDTO.ItemGUID = ItemGUID;
                    //objDTO.CreatedBy = SessionHelper.UserID;
                    //objDTO.Created = DateTimeUtility.DateTimeNow;
                    //objDTO.LastUpdatedBy = SessionHelper.UserID;
                    //objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    //objDTO.GUID = Guid.NewGuid();
                    //objDTO.Room = SessionHelper.RoomID;
                    //objDTO.CompanyID = SessionHelper.CompanyID;
                    //objDTO.RoomName = SessionHelper.RoomName;
                    //objDTO.UpdatedByName = SessionHelper.UserName;
                    //objDTO.CreatedByName = SessionHelper.UserName;
                    //objDTO.IsDefault = true;
                    if (lstBinReplanish == null)
                        objBinMasterDTO.SessionSr = 1;
                    else
                        objBinMasterDTO.SessionSr = lstBinReplanish.Count + 1;
                    objBinMasterDTO.ID = 0;
                    lstBinReplanish.Add(objBinMasterDTO);
                    Session["BinReplanish" + itemGuidString] = lstBinReplanish;
                    lstBinReplanish = (List<BinMasterDTO>)Session["BinReplanish" + itemGuidString];

                }

            }

            if (AddCount != null && AddCount > 0)
            {
                for (int i = 0; i < AddCount; i++)
                {
                    lstBinReplanish.Add(new BinMasterDTO() { ID = 0, SessionSr = lstBinReplanish.Count + 1, ItemGUID = ItemGUID, Room = SessionHelper.RoomID, CompanyID = SessionHelper.CompanyID, LastUpdated = DateTimeUtility.DateTimeNow, LastUpdatedBy = SessionHelper.UserID, Created = DateTimeUtility.DateTimeNow, CreatedBy = SessionHelper.UserID, GUID = Guid.NewGuid() });
                }
            }

            return PartialView("_BinReplanishLocations", lstBinReplanish.OrderBy(t => t.BinNumber).ToList());


        }

        public JsonResult SavetoSeesionBinReplanishSingle(Int64 ID, Int32 SessionSr, Guid GUID, Guid ITEMGUID, Int64 BinID, string BinLocation, float CriticalQuanity, float MinimumQuantity, float MaximumQuantity, bool IsDefault, string eVMISensorPort, double eVMISensorID, float? customerOwnedQuantity, float? consignedQuantity, bool? isEnforceDefaultPullQuantity, float? defaultPullQuantity, bool? isEnforceDefaultReorderQuantity, float? defaultReorderQuantity, string UDF1Bin, string UDF2Bin, string UDF3Bin, string UDF4Bin, string UDF5Bin)
        {
            string itemGuidString = Convert.ToString(ITEMGUID);
            List<BinMasterDTO> lstBinReplanish = null;
            if (Session["BinReplanish" + itemGuidString] != null)
            {
                lstBinReplanish = (List<BinMasterDTO>)Session["BinReplanish" + itemGuidString];
            }
            else
            {
                lstBinReplanish = new List<BinMasterDTO>();
            }

            ///Deplicate checking......
            //if (lstBinReplanish.Count > 0)
            //{
            //    List<ItemLocationLevelQuanityDTO> previoussaved = lstBinReplanish.Where(t => t.BinID == BinID).ToList();
            //    if (previoussaved != null && previoussaved.Count >= 1)
            //    {
            //        return Json(new { status = "duplicate" }, JsonRequestBehavior.AllowGet);
            //    }
            //}



            if (ID > 0 && SessionSr == 0)
            {
                BinMasterDTO objDTO = lstBinReplanish.Where(t => t.ID == ID).FirstOrDefault();
                if (objDTO != null)
                {
                    lstBinReplanish.Remove(lstBinReplanish.Where(t => t.ID == ID).FirstOrDefault());
                    objDTO.ID = ID;
                    //  objDTO.BinID = BinID;
                    objDTO.BinNumber = BinLocation;
                    objDTO.CriticalQuantity = CriticalQuanity;
                    objDTO.MinimumQuantity = MinimumQuantity;
                    objDTO.MaximumQuantity = MaximumQuantity;
                    objDTO.ItemGUID = ITEMGUID;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objDTO.GUID = Guid.NewGuid();

                    objDTO.Room = SessionHelper.RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.RoomName = SessionHelper.RoomName;
                    objDTO.UpdatedByName = SessionHelper.UserName;
                    objDTO.CreatedByName = SessionHelper.UserName;
                    objDTO.IsDefault = IsDefault;
                    //   if (eVMISensorID != 0)
                    objDTO.eVMISensorID = eVMISensorID;
                    objDTO.eVMISensorPort = eVMISensorPort;
                    objDTO.ConsignedQuantity = consignedQuantity.GetValueOrDefault(0);
                    objDTO.CustomerOwnedQuantity = customerOwnedQuantity.GetValueOrDefault(0);

                    objDTO.DefaultPullQuantity = defaultPullQuantity;
                    objDTO.DefaultReorderQuantity = defaultReorderQuantity;
                    objDTO.IsEnforceDefaultPullQuantity = isEnforceDefaultPullQuantity;
                    objDTO.IsEnforceDefaultReorderQuantity = isEnforceDefaultReorderQuantity;
                    objDTO.BinUDF1 = UDF1Bin;
                    objDTO.BinUDF2 = UDF2Bin;
                    objDTO.BinUDF3 = UDF3Bin;
                    objDTO.BinUDF4 = UDF4Bin;
                    objDTO.BinUDF5 = UDF5Bin;
                    lstBinReplanish.Add(objDTO);
                }
            }
            else
            {
                if (SessionSr > 0)
                {
                    BinMasterDTO objDTO = lstBinReplanish.Where(t => t.SessionSr == SessionSr).FirstOrDefault();
                    if (objDTO != null)
                    {
                        lstBinReplanish.Remove(lstBinReplanish.Where(t => t.SessionSr == SessionSr).FirstOrDefault());
                        objDTO.ID = ID;
                        // objDTO.BinID = BinID;
                        objDTO.BinNumber = BinLocation;
                        objDTO.CriticalQuantity = CriticalQuanity;
                        objDTO.MinimumQuantity = MinimumQuantity;
                        objDTO.MaximumQuantity = MaximumQuantity;
                        objDTO.ItemGUID = ITEMGUID;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        objDTO.GUID = Guid.NewGuid();

                        objDTO.Room = SessionHelper.RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.RoomName = SessionHelper.RoomName;
                        objDTO.UpdatedByName = SessionHelper.UserName;
                        objDTO.CreatedByName = SessionHelper.UserName;
                        objDTO.IsDefault = IsDefault;
                        // if (eVMISensorID != 0)
                        objDTO.eVMISensorID = eVMISensorID;
                        objDTO.eVMISensorPort = eVMISensorPort;
                        objDTO.ConsignedQuantity = consignedQuantity.GetValueOrDefault(0);
                        objDTO.CustomerOwnedQuantity = customerOwnedQuantity.GetValueOrDefault(0);

                        objDTO.DefaultPullQuantity = defaultPullQuantity;
                        objDTO.DefaultReorderQuantity = defaultReorderQuantity;
                        objDTO.IsEnforceDefaultPullQuantity = isEnforceDefaultPullQuantity;
                        objDTO.IsEnforceDefaultReorderQuantity = isEnforceDefaultReorderQuantity;
                        objDTO.BinUDF1 = UDF1Bin;
                        objDTO.BinUDF2 = UDF2Bin;
                        objDTO.BinUDF3 = UDF3Bin;
                        objDTO.BinUDF4 = UDF4Bin;
                        objDTO.BinUDF5 = UDF5Bin;
                        lstBinReplanish.Add(objDTO);
                    }
                    else
                    {
                        objDTO = new BinMasterDTO();
                        objDTO.ID = 0;
                        // objDTO.BinID = BinID;
                        objDTO.BinNumber = BinLocation;
                        objDTO.CriticalQuantity = CriticalQuanity;
                        objDTO.MinimumQuantity = MinimumQuantity;
                        objDTO.MaximumQuantity = MaximumQuantity;
                        objDTO.ItemGUID = ITEMGUID;
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.Created = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        objDTO.GUID = GUID;

                        objDTO.Room = SessionHelper.RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.RoomName = SessionHelper.RoomName;
                        objDTO.UpdatedByName = SessionHelper.UserName;
                        objDTO.CreatedByName = SessionHelper.UserName;
                        objDTO.SessionSr = lstBinReplanish.Count + 1;
                        objDTO.IsDefault = IsDefault;
                        //if (eVMISensorID != 0)
                        objDTO.eVMISensorID = eVMISensorID;
                        objDTO.eVMISensorPort = eVMISensorPort;
                        objDTO.ConsignedQuantity = consignedQuantity.GetValueOrDefault(0);
                        objDTO.CustomerOwnedQuantity = customerOwnedQuantity.GetValueOrDefault(0);

                        objDTO.DefaultPullQuantity = defaultPullQuantity;
                        objDTO.DefaultReorderQuantity = defaultReorderQuantity;
                        objDTO.IsEnforceDefaultPullQuantity = isEnforceDefaultPullQuantity;
                        objDTO.IsEnforceDefaultReorderQuantity = isEnforceDefaultReorderQuantity;
                        objDTO.BinUDF1 = UDF1Bin;
                        objDTO.BinUDF2 = UDF2Bin;
                        objDTO.BinUDF3 = UDF3Bin;
                        objDTO.BinUDF4 = UDF4Bin;
                        objDTO.BinUDF5 = UDF5Bin;
                        lstBinReplanish.Add(objDTO);
                    }
                }
                else
                {
                    BinMasterDTO objDTO = new BinMasterDTO();
                    objDTO.ID = 0;
                    //   objDTO.ID = BinID;
                    objDTO.BinNumber = BinLocation;
                    objDTO.CriticalQuantity = CriticalQuanity;
                    objDTO.MinimumQuantity = MinimumQuantity;
                    objDTO.MaximumQuantity = MaximumQuantity;
                    objDTO.ItemGUID = ITEMGUID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objDTO.GUID = GUID;
                    objDTO.Room = SessionHelper.RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.RoomName = SessionHelper.RoomName;
                    objDTO.UpdatedByName = SessionHelper.UserName;
                    objDTO.CreatedByName = SessionHelper.UserName;
                    objDTO.SessionSr = lstBinReplanish.Count + 1;
                    objDTO.IsDefault = IsDefault;
                    //if (eVMISensorID != 0)
                    objDTO.eVMISensorID = eVMISensorID;
                    objDTO.eVMISensorPort = eVMISensorPort;
                    objDTO.ConsignedQuantity = consignedQuantity.GetValueOrDefault(0);
                    objDTO.CustomerOwnedQuantity = customerOwnedQuantity.GetValueOrDefault(0);

                    objDTO.DefaultPullQuantity = defaultPullQuantity;
                    objDTO.DefaultReorderQuantity = defaultReorderQuantity;
                    objDTO.IsEnforceDefaultPullQuantity = isEnforceDefaultPullQuantity;
                    objDTO.IsEnforceDefaultReorderQuantity = isEnforceDefaultReorderQuantity;
                    objDTO.BinUDF1 = UDF1Bin;
                    objDTO.BinUDF2 = UDF2Bin;
                    objDTO.BinUDF3 = UDF3Bin;
                    objDTO.BinUDF4 = UDF4Bin;
                    objDTO.BinUDF5 = UDF5Bin;
                    lstBinReplanish.Add(objDTO);
                }
            }
            Session["BinReplanish" + itemGuidString] = lstBinReplanish;

            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SavetoSeesionBinReplanishSingleNew(Int64 ID, Int32 SessionSr, Guid GUID, Guid ITEMGUID, Int64 BinID, string BinLocation, float CriticalQuanity, float MinimumQuantity, float MaximumQuantity, bool IsDefault, string eVMISensorPort, double eVMISensorID, float? customerOwnedQuantity, float? consignedQuantity, bool? isEnforceDefaultPullQuantity, float? defaultPullQuantity, bool? isEnforceDefaultReorderQuantity, float? defaultReorderQuantity)
        {
            Guid newGUID = new Guid();
            string itemGuidString = Convert.ToString(ITEMGUID);
            List<BinMasterDTO> lstBinReplanish = null;

            if (Session["BinReplanish" + itemGuidString] != null)
            {
                lstBinReplanish = (List<BinMasterDTO>)Session["BinReplanish" + itemGuidString];
            }
            else
            {
                lstBinReplanish = new List<BinMasterDTO>();
            }

            ///Deplicate checking......
            //if (lstBinReplanish.Count > 0)
            //{
            //    List<ItemLocationLevelQuanityDTO> previoussaved = lstBinReplanish.Where(t => t.BinID == BinID).ToList();
            //    if (previoussaved != null && previoussaved.Count >= 1)
            //    {
            //        return Json(new { status = "duplicate" }, JsonRequestBehavior.AllowGet);
            //    }
            //}



            if (ID > 0 && SessionSr == 0)
            {
                BinMasterDTO objDTO = lstBinReplanish.Where(t => t.ID == ID).FirstOrDefault();
                if (objDTO != null)
                {
                    lstBinReplanish.Remove(lstBinReplanish.Where(t => t.ID == ID).FirstOrDefault());
                    objDTO.ID = ID;
                    //  objDTO.BinID = BinID;
                    objDTO.BinNumber = BinLocation;
                    objDTO.CriticalQuantity = CriticalQuanity;
                    objDTO.MinimumQuantity = MinimumQuantity;
                    objDTO.MaximumQuantity = MaximumQuantity;
                    objDTO.ItemGUID = ITEMGUID;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objDTO.GUID = Guid.NewGuid();
                    newGUID = objDTO.GUID;
                    objDTO.Room = SessionHelper.RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.RoomName = SessionHelper.RoomName;
                    objDTO.UpdatedByName = SessionHelper.UserName;
                    objDTO.CreatedByName = SessionHelper.UserName;
                    objDTO.IsDefault = IsDefault;
                    //if (eVMISensorID != 0)
                    objDTO.eVMISensorID = eVMISensorID;
                    objDTO.eVMISensorPort = eVMISensorPort;
                    objDTO.ConsignedQuantity = consignedQuantity.GetValueOrDefault(0);
                    objDTO.CustomerOwnedQuantity = customerOwnedQuantity.GetValueOrDefault(0);

                    objDTO.DefaultPullQuantity = defaultPullQuantity;
                    objDTO.DefaultReorderQuantity = defaultReorderQuantity;
                    objDTO.IsEnforceDefaultPullQuantity = isEnforceDefaultPullQuantity;
                    objDTO.IsEnforceDefaultReorderQuantity = isEnforceDefaultReorderQuantity;

                    lstBinReplanish.Add(objDTO);
                }
            }
            else
            {
                if (SessionSr > 0)
                {
                    BinMasterDTO objDTO = lstBinReplanish.Where(t => t.SessionSr == SessionSr).FirstOrDefault();
                    if (objDTO != null)
                    {
                        lstBinReplanish.Remove(lstBinReplanish.Where(t => t.SessionSr == SessionSr).FirstOrDefault());
                        objDTO.ID = ID;
                        // objDTO.BinID = BinID;
                        objDTO.BinNumber = BinLocation;
                        objDTO.CriticalQuantity = CriticalQuanity;
                        objDTO.MinimumQuantity = MinimumQuantity;
                        objDTO.MaximumQuantity = MaximumQuantity;
                        objDTO.ItemGUID = ITEMGUID;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        objDTO.GUID = Guid.NewGuid();
                        newGUID = objDTO.GUID;
                        objDTO.Room = SessionHelper.RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.RoomName = SessionHelper.RoomName;
                        objDTO.UpdatedByName = SessionHelper.UserName;
                        objDTO.CreatedByName = SessionHelper.UserName;
                        objDTO.IsDefault = IsDefault;
                        //if (eVMISensorID != 0)
                        objDTO.eVMISensorID = eVMISensorID;
                        objDTO.eVMISensorPort = eVMISensorPort;
                        objDTO.ConsignedQuantity = consignedQuantity.GetValueOrDefault(0);
                        objDTO.CustomerOwnedQuantity = customerOwnedQuantity.GetValueOrDefault(0);

                        objDTO.DefaultPullQuantity = defaultPullQuantity;
                        objDTO.DefaultReorderQuantity = defaultReorderQuantity;
                        objDTO.IsEnforceDefaultPullQuantity = isEnforceDefaultPullQuantity;
                        objDTO.IsEnforceDefaultReorderQuantity = isEnforceDefaultReorderQuantity;

                        lstBinReplanish.Add(objDTO);
                    }
                    else
                    {
                        objDTO = new BinMasterDTO();
                        objDTO.ID = 0;
                        // objDTO.BinID = BinID;
                        objDTO.BinNumber = BinLocation;
                        objDTO.CriticalQuantity = CriticalQuanity;
                        objDTO.MinimumQuantity = MinimumQuantity;
                        objDTO.MaximumQuantity = MaximumQuantity;
                        objDTO.ItemGUID = ITEMGUID;
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.Created = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        objDTO.GUID = GUID;
                        newGUID = objDTO.GUID;
                        objDTO.Room = SessionHelper.RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.RoomName = SessionHelper.RoomName;
                        objDTO.UpdatedByName = SessionHelper.UserName;
                        objDTO.CreatedByName = SessionHelper.UserName;
                        objDTO.SessionSr = lstBinReplanish.Count + 1;
                        objDTO.IsDefault = IsDefault;
                        //if (eVMISensorID != 0)
                        objDTO.eVMISensorID = eVMISensorID;
                        objDTO.eVMISensorPort = eVMISensorPort;
                        objDTO.ConsignedQuantity = consignedQuantity.GetValueOrDefault(0);
                        objDTO.CustomerOwnedQuantity = customerOwnedQuantity.GetValueOrDefault(0);

                        objDTO.DefaultPullQuantity = defaultPullQuantity;
                        objDTO.DefaultReorderQuantity = defaultReorderQuantity;
                        objDTO.IsEnforceDefaultPullQuantity = isEnforceDefaultPullQuantity;
                        objDTO.IsEnforceDefaultReorderQuantity = isEnforceDefaultReorderQuantity;

                        lstBinReplanish.Add(objDTO);
                    }
                }
                else
                {
                    BinMasterDTO objDTO = new BinMasterDTO();
                    objDTO.ID = 0;
                    //   objDTO.ID = BinID;
                    objDTO.BinNumber = BinLocation;
                    objDTO.CriticalQuantity = CriticalQuanity;
                    objDTO.MinimumQuantity = MinimumQuantity;
                    objDTO.MaximumQuantity = MaximumQuantity;
                    objDTO.ItemGUID = ITEMGUID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objDTO.GUID = GUID;
                    newGUID = objDTO.GUID;
                    objDTO.Room = SessionHelper.RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.RoomName = SessionHelper.RoomName;
                    objDTO.UpdatedByName = SessionHelper.UserName;
                    objDTO.CreatedByName = SessionHelper.UserName;
                    objDTO.SessionSr = lstBinReplanish.Count + 1;
                    objDTO.IsDefault = IsDefault;
                    //if (eVMISensorID != 0)
                    objDTO.eVMISensorID = eVMISensorID;
                    objDTO.eVMISensorPort = eVMISensorPort;
                    objDTO.ConsignedQuantity = consignedQuantity.GetValueOrDefault(0);
                    objDTO.CustomerOwnedQuantity = customerOwnedQuantity.GetValueOrDefault(0);

                    objDTO.DefaultPullQuantity = defaultPullQuantity;
                    objDTO.DefaultReorderQuantity = defaultReorderQuantity;
                    objDTO.IsEnforceDefaultPullQuantity = isEnforceDefaultPullQuantity;
                    objDTO.IsEnforceDefaultReorderQuantity = isEnforceDefaultReorderQuantity;

                    lstBinReplanish.Add(objDTO);
                }
            }
            Session["BinReplanish" + itemGuidString] = lstBinReplanish;

            return Json(new { status = "ok", newGUID = newGUID }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeletetoSeesionBinReplanishSingle(Int64 ID, Guid GUID, Guid ITEMGUID, Int64 BinID)
        {
            List<BinMasterDTO> lstBinReplanish = null;
            string itemGuidString = Convert.ToString(ITEMGUID);

            if (Session["BinReplanish" + itemGuidString] != null)
            {
                lstBinReplanish = (List<BinMasterDTO>)Session["BinReplanish" + itemGuidString];
            }
            else
            {
                lstBinReplanish = new List<BinMasterDTO>();
            }
            ///Delete the Records......
            if (ID > 0)
            {
                try
                {
                    lstBinReplanish.Remove(lstBinReplanish.Where(t => t.ID == ID).FirstOrDefault());
                    Session["BinReplanish" + itemGuidString] = lstBinReplanish;
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
                    Session["BinReplanish" + itemGuidString] = lstBinReplanish;
                }
                catch
                {
                    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "deleted" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CheckifanyCartEntryExist(Guid ITEMGUID, Int64 BinID)
        {
            if (ITEMGUID == Guid.Empty)
            {
                return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            try
            {


                BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                List<BinMasterDTO> objBinMasterList = objBinMasterDAL.CheckBinInUse_New(SessionHelper.RoomID, SessionHelper.CompanyID, BinID, ITEMGUID);
                string ErrorMessage = string.Empty;
                string status = "ok";
                foreach (var item in objBinMasterList)
                {
                    //if (((item.CustomerOwnedQuantity ?? 0) + (item.ConsignedQuantity ?? 0) + (item.MinimumQuantity ?? 0) + (item.MaximumQuantity ?? 0) + (item.CriticalQuantity ?? 0) + (item.SuggestedOrderQuantity ?? 0)) != 0)
                    if (((item.MinimumQuantity ?? 0) + (item.MaximumQuantity ?? 0) + (item.CriticalQuantity ?? 0) + (item.SuggestedOrderQuantity ?? 0) + (item.CountQuantity ?? 0) + (item.KitMoveInOutQuantity ?? 0)) != 0)
                    {
                        //if ((item.CustomerOwnedQuantity ?? 0) > 0)
                        //{
                        //    ErrorMessage = eTurns.DTO.ResBin.CustomerOwnedQuantity;
                        //    status = "referencecount";
                        //}
                        //if ((item.ConsignedQuantity ?? 0) > 0)
                        //{
                        //    status = "referencecount";
                        //    if (string.IsNullOrWhiteSpace(ErrorMessage))
                        //    {
                        //        ErrorMessage = eTurns.DTO.ResBin.ConsignedQuantity;

                        //    }
                        //    else
                        //    {
                        //        ErrorMessage += ", "+ eTurns.DTO.ResBin.ConsignedQuantity;
                        //    }
                        //}
                        if ((item.MinimumQuantity ?? 0) > 0)
                        {
                            status = "referencecount";
                            if (string.IsNullOrWhiteSpace(ErrorMessage))
                            {
                                ErrorMessage = eTurns.DTO.ResBin.RequisitionedQuantity;
                            }
                            else
                            {
                                ErrorMessage += ", " + eTurns.DTO.ResBin.RequisitionedQuantity;
                            }
                        }
                        if ((item.MaximumQuantity ?? 0) > 0)
                        {
                            status = "referencecount";
                            if (string.IsNullOrWhiteSpace(ErrorMessage))
                            {
                                ErrorMessage = ResItemMaster.RequestedTransferQuantity;
                            }
                            else
                            {
                                ErrorMessage += ", " + ResItemMaster.RequestedTransferQuantity;
                            }
                        }
                        if ((item.CriticalQuantity ?? 0) > 0)
                        {
                            status = "referencecount";
                            if (string.IsNullOrWhiteSpace(ErrorMessage))
                            {
                                ErrorMessage = eTurns.DTO.ResBin.RequestedQuantity;

                            }
                            else
                            {
                                ErrorMessage += ", " + eTurns.DTO.ResBin.RequestedQuantity;
                            }

                        }
                        if ((item.SuggestedOrderQuantity ?? 0) > 0)
                        {
                            status = "referencecount";
                            if (string.IsNullOrWhiteSpace(ErrorMessage))
                            {
                                ErrorMessage = ResItemMaster.SuggestedOrderQuantity;
                            }
                            else
                            {
                                ErrorMessage += ", " + ResItemMaster.SuggestedOrderQuantity;
                            }
                        }
                        if ((item.CountQuantity ?? 0) > 0)
                        {
                            status = "referencecount";
                            if (string.IsNullOrWhiteSpace(ErrorMessage))
                            {
                                ErrorMessage = ResItemMaster.CountQuantity;
                            }
                            else
                            {
                                ErrorMessage += ", " + ResItemMaster.CountQuantity;
                            }
                        }
                        if ((item.KitMoveInOutQuantity ?? 0) > 0)
                        {
                            status = "referencecount";
                            if (string.IsNullOrWhiteSpace(ErrorMessage))
                            {
                                ErrorMessage = ResItemMaster.KitMoveInOutQuantity;
                            }
                            else
                            {
                                ErrorMessage += ", " + ResItemMaster.KitMoveInOutQuantity;
                            }
                        }
                        break;
                    }
                }
                //if (IsBinInUse)
                //{
                return Json(new { status = status, ErrorMessage = ErrorMessage }, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    return Json(new { status = "ok",ErrorMessage=string.Empty }, JsonRequestBehavior.AllowGet);
                //}
            }
            catch (Exception)
            {
                return Json(new { status = "referencecount" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region "Item Manufacturer"

        /// <summary>
        /// Load BinReplanish
        /// </summary>
        /// <param name="ItemGUID"></param>
        /// <returns></returns>
        public ActionResult LoadManufaturerofItem(Guid ItemGUID, int? AddCount)
        {
            ViewBag.ItemGUID = ItemGUID;
            string itemGuidString = Convert.ToString(ItemGUID);
            ManufacturerMasterDAL objManufacturerMasterDAL = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName);

            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            ItemMasterDTO objDTO = obj.GetItemWithoutJoins(null, ItemGUID);

            if (objDTO != null && objDTO.IsBOMItem == false && objDTO.RefBomId.GetValueOrDefault(0) > 0)
            {
                ViewBag.IsBOMItem = true;
            }
            else
            {
                ViewBag.IsBOMItem = false;
            }

            //ViewBag.IsItemAddedFromAB = false;
            //if (objDTO != null && objDTO.ID > 0 && !string.IsNullOrWhiteSpace(objDTO.ItemUniqueNumber))
            //{
            //    ProductDetailsDAL objProductDetailsDAL = new ProductDetailsDAL(SessionHelper.EnterPriseDBName);
            //    Int64 ABItemMappingID = objProductDetailsDAL.CheckItemAddedFromAB(objDTO.ItemUniqueNumber, objDTO.GUID, SessionHelper.CompanyID, SessionHelper.RoomID);
            //    if (ABItemMappingID > 0)
            //    {
            //        ViewBag.IsItemAddedFromAB = true;
            //    }
            //}

            var lstManu = new List<ManufacturerMasterDTO>();
            lstManu = objManufacturerMasterDAL.GetManufacturerByRoomNormal(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false, "ID DESC");

            ManufacturerMasterDTO objBlankManu = new ManufacturerMasterDTO();
            objBlankManu = new ManufacturerMasterDTO();

            if (lstManu.Count() == 0)
                lstManu = new List<ManufacturerMasterDTO>();
            lstManu.Add(objBlankManu);

            ViewBag.ManufacturerBag = lstManu;
            //ViewBag.ManufacturerBag = objManufacturerMasterDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false).ToList();

            List<ItemManufacturerDetailsDTO> lstItemManufacture = null;
            if (Session["ItemManufacture" + itemGuidString] != null)
            {
                lstItemManufacture = (List<ItemManufacturerDetailsDTO>)Session["ItemManufacture" + itemGuidString];
                //lstItemManufacture = lstItemManufacture.Where(i => i.IsDeleted == false).ToList();
                //Delete blank rows
                //lstItemManufacture.Remove(lstItemManufacture.Where(t => t.GUID == Guid.Empty && t.ManufacturerID == 0).FirstOrDefault());
            }
            else
            {
                lstItemManufacture = new List<ItemManufacturerDetailsDTO>();
            }

            //if (lstBinReplanish.Count == 0 && AddCount ==0)
            //{
            //    AddCount = 1;
            //}


            if (AddCount != null && AddCount > 0)
            {
                for (int i = 0; i < AddCount; i++)
                {
                    lstItemManufacture.Add(new ItemManufacturerDetailsDTO() { ID = 0, SessionSr = lstItemManufacture.Count + 1, ItemGUID = ItemGUID, Room = SessionHelper.RoomID, CompanyID = SessionHelper.CompanyID, Updated = DateTimeUtility.DateTimeNow, LastUpdatedBy = SessionHelper.UserID, Created = DateTimeUtility.DateTimeNow, CreatedBy = SessionHelper.UserID });
                }
                Session["ItemManufacture" + itemGuidString] = (List<ItemManufacturerDetailsDTO>)lstItemManufacture;
            }

            return PartialView("_ItemManufacturer", lstItemManufacture.OrderByDescending(t => t.ID).ToList());
        }

        public JsonResult SavetoSeesionItemManufacture(Int64 ID, Int32 SessionSr, Guid GUID, Guid ITEMGUID, Int64 ManufacturerID, string ManufactureName, string ManufacturerNumber, bool IsDefault)
        {
            List<ItemManufacturerDetailsDTO> lstBinReplanish = null;
            string itemGuidString = Convert.ToString(ITEMGUID);
            if (Session["ItemManufacture" + itemGuidString] != null)
            {
                lstBinReplanish = (List<ItemManufacturerDetailsDTO>)Session["ItemManufacture" + itemGuidString];
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

                    Session["ItemManufacture" + itemGuidString] = lstBinReplanish;
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
                        objDTO.Room = SessionHelper.RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.RoomName = SessionHelper.RoomName;
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
                    objDTO.Room = SessionHelper.RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.RoomName = SessionHelper.RoomName;
                    objDTO.UpdatedByName = SessionHelper.UserName;
                    objDTO.CreatedByName = SessionHelper.UserName;
                    objDTO.SessionSr = lstBinReplanish.Count + 1;
                    lstBinReplanish.Add(objDTO);
                }
            }
            Session["ItemManufacture" + itemGuidString] = lstBinReplanish;

            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeletetoSeesionItemManufactureSingle(Int64 ID, Int32 SessionSr, Guid GUID, Guid ITEMGUID, Int64 ManufacturerID, string ManufacturerNumber, bool IsDefault)
        {
            string itemGuidString = Convert.ToString(ITEMGUID);
            List<ItemManufacturerDetailsDTO> lstBinReplanish = null;
            if (Session["ItemManufacture" + itemGuidString] != null)
            {
                lstBinReplanish = (List<ItemManufacturerDetailsDTO>)Session["ItemManufacture" + itemGuidString];
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
                    Session["ItemManufacture" + itemGuidString] = lstBinReplanish;
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
                    Session["ItemManufacture" + itemGuidString] = lstBinReplanish;
                }
                catch
                {
                    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "deleted" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult checkInventoryclassification(double ID)
        {
            double InvId = 0;
            string[] InvIdLoc;
            double CostUomval = 0;
            InventoryClassificationMasterDAL obj1 = new InventoryClassificationMasterDAL(SessionHelper.EnterPriseDBName);
            List<InventoryClassificationMasterDTO> lstUnit1 = obj1.GetInventoryClassificationByRoomNormal(SessionHelper.RoomID, SessionHelper.CompanyID, false).ToList();
            List<InventoryClassificationMasterDTO> lstUnit2;
            RoomDAL objRoomDal = new RoomDAL(SessionHelper.EnterPriseDBName);
            //RoomDTO ROOMDTO = objRoomDal.GetRoomByIDPlain(SessionHelper.RoomID);
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            string columnList = "ID,RoomName,BaseOfInventory";
            RoomDTO ROOMDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");

            if (ROOMDTO != null)
            {
                if (ROOMDTO.BaseOfInventory == 1)
                {
                    InvIdLoc = ID.ToString().Split('.');
                    if (InvIdLoc.Count() == 2)
                    {
                        if (Convert.ToDouble(InvIdLoc[1]) > 0)
                        {
                            CostUomval = Convert.ToDouble(InvIdLoc[0]) + 1;
                        }
                    }
                    else
                    {
                        CostUomval = ID;
                    }
                    // double CostUomval = ID;
                    lstUnit2 = lstUnit1.Where(t => (t.RangeStart ?? 0) <= CostUomval && (t.RangeEnd ?? 0) >= CostUomval && t.RangeEnd != null).ToList();
                    if (lstUnit2 != null && lstUnit2.Any())
                    {
                        InvId = lstUnit2[0].ID;
                    }
                    if (InvId == 0)
                    {
                        lstUnit2 = lstUnit1.Where(t => (t.RangeStart ?? 0) <= CostUomval && t.RangeEnd == null).ToList();
                        if (lstUnit2 != null && lstUnit2.Any())
                        {
                            InvId = lstUnit2[0].ID;
                        }
                    }
                }
            }
            return Json(new { status = InvId.ToString() }, JsonRequestBehavior.AllowGet);
        }

        public int checkInventoryclassificationTurn(double turns, double exCost)
        {
            int InvId = 0;
            string[] InvIdLoc;
            double turnsorexcostval = 0;
            InventoryClassificationMasterDAL obj1 = new InventoryClassificationMasterDAL(SessionHelper.EnterPriseDBName);
            List<InventoryClassificationMasterDTO> lstIC1 = obj1.GetInventoryClassificationByRoomNormal(SessionHelper.RoomID, SessionHelper.CompanyID, false).ToList();
            List<InventoryClassificationMasterDTO> lstIC12;
            RoomDAL objRoomDal = new RoomDAL(SessionHelper.EnterPriseDBName);
            //  RoomDTO ROOMDTO = objRoomDal.GetRoomByIDPlain(SessionHelper.RoomID);
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            string columnList = "ID,RoomName,BaseOfInventory";
            RoomDTO ROOMDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");
            if (ROOMDTO != null)
            {
                double calculateValue = 0;
                if (ROOMDTO.BaseOfInventory == 2)
                {
                    calculateValue = turns;
                    InvIdLoc = calculateValue.ToString().Split('.');
                }
                else if (ROOMDTO.BaseOfInventory == 1)
                {
                    calculateValue = exCost;
                    InvIdLoc = calculateValue.ToString().Split('.');
                }
                else
                {
                    InvIdLoc = null;
                }

                if (InvIdLoc != null && InvIdLoc.Count() == 2)
                {
                    if (Convert.ToDouble(InvIdLoc[1]) > 0)
                    {
                        turnsorexcostval = Convert.ToDouble(InvIdLoc[0]) + 1;
                    }
                }
                else
                {
                    turnsorexcostval = calculateValue;
                }

                lstIC12 = lstIC1.Where(t => (t.RangeStart ?? 0) <= turnsorexcostval && (t.RangeEnd ?? 0) >= turnsorexcostval && t.RangeEnd != null).ToList();
                if (lstIC12 != null && lstIC12.Any())
                {
                    InvId = lstIC12[0].ID;
                }
                if (InvId == 0)
                {
                    lstIC12 = lstIC1.Where(t => (t.RangeStart ?? 0) <= turnsorexcostval && t.RangeEnd == null).ToList();
                    if (lstIC12 != null && lstIC12.Any())
                    {
                        InvId = lstIC12[0].ID;
                    }
                }

            }
            return InvId;
        }

        #endregion

        #region "Item Supplier"

        /// <summary>
        /// Load BinReplanish
        /// </summary>
        /// <param name="ItemGUID"></param>
        /// <returns></returns>       

        public ActionResult LoadSupplierofItem(Guid ItemGUID, int? AddCount)
        {
            ViewBag.ItemGUID = ItemGUID;
            string itemGuidString = Convert.ToString(ItemGUID);
            SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);

            if (SessionHelper.UserSupplierIds != null && SessionHelper.UserSupplierIds.Any())
            {
                List<SupplierMasterDTO> lstSupplier = new List<SupplierMasterDTO>();
                string strSupplierIds = string.Join(",", SessionHelper.UserSupplierIds);
                var suppliers = objSupplierMasterDAL.GetSupplierByIDsPlain(strSupplierIds, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (suppliers != null && suppliers.Any())
                {
                    lstSupplier.AddRange(suppliers);
                }
                ViewBag.SupplierBag = lstSupplier;
            }
            else
                ViewBag.SupplierBag = objSupplierMasterDAL.GetSupplierByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID, false).OrderBy(x => x.SupplierName).ToList();

            bool isSupplierInsert = false;
            isSupplierInsert = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.SupplierMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
            ViewBag.CanSupplierInsert = isSupplierInsert;
            List<ItemSupplierDetailsDTO> lstItemSupplier = null;

            if (Session["ItemSupplier" + itemGuidString] != null)
            {
                lstItemSupplier = (List<ItemSupplierDetailsDTO>)Session["ItemSupplier" + itemGuidString];
            }
            else
            {
                lstItemSupplier = new List<ItemSupplierDetailsDTO>();
            }

            //get default supplier...
            RoomDAL objRoomDal = new RoomDAL(SessionHelper.EnterPriseDBName);
            //   RoomDTO objRoomDTO = objRoomDal.GetRoomByIDPlain(SessionHelper.RoomID);
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            string columnList = "ID,RoomName,DefaultSupplierID";
            RoomDTO objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");


            if (AddCount != null && AddCount > 0)
            {
                for (int i = 0; i < AddCount; i++)
                {
                    //check if it not contains default supplier then suggest default supplier from room
                    if (objRoomDTO.DefaultSupplierID > 0 && lstItemSupplier.Where(t => t.SupplierID == objRoomDTO.DefaultSupplierID || t.IsDefault == true).Count() == 0)
                    {
                        SupplierMasterDTO objSupplierMasterDTO = objSupplierMasterDAL.GetSupplierByIDPlain(objRoomDTO.DefaultSupplierID.GetValueOrDefault(0));
                        lstItemSupplier.Add(new ItemSupplierDetailsDTO() { ID = 0, SupplierName = objSupplierMasterDTO.SupplierName, SupplierID = objRoomDTO.DefaultSupplierID.GetValueOrDefault(0), IsDefault = true, SessionSr = lstItemSupplier.Count + 1, ItemGUID = ItemGUID, Room = SessionHelper.RoomID, CompanyID = SessionHelper.CompanyID, Updated = DateTimeUtility.DateTimeNow, LastUpdatedBy = SessionHelper.UserID, Created = DateTimeUtility.DateTimeNow, CreatedBy = SessionHelper.UserID });
                    }
                    else
                    {
                        lstItemSupplier.Add(new ItemSupplierDetailsDTO() { ID = 0, SessionSr = lstItemSupplier.Count + 1, ItemGUID = ItemGUID, Room = SessionHelper.RoomID, CompanyID = SessionHelper.CompanyID, Updated = DateTimeUtility.DateTimeNow, LastUpdatedBy = SessionHelper.UserID, Created = DateTimeUtility.DateTimeNow, CreatedBy = SessionHelper.UserID });
                    }
                }
                Session["ItemSupplier" + itemGuidString] = (List<ItemSupplierDetailsDTO>)lstItemSupplier;
            }

            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            ItemMasterDTO objDTO = obj.GetItemWithoutJoins(null, ItemGUID);
            //ViewBag.IsItemAddedFromAB = false;
            //if (objDTO != null && objDTO.ID > 0 && !string.IsNullOrWhiteSpace(objDTO.ItemUniqueNumber))
            //{
            //    ProductDetailsDAL objProductDetailsDAL = new ProductDetailsDAL(SessionHelper.EnterPriseDBName);
            //    Int64 ABItemMappingID = objProductDetailsDAL.CheckItemAddedFromAB(objDTO.ItemUniqueNumber, objDTO.GUID, SessionHelper.CompanyID, SessionHelper.RoomID);
            //    if (ABItemMappingID > 0)
            //    {
            //        ViewBag.IsItemAddedFromAB = true;
            //    }
            //}
            return PartialView("_ItemSupplier", lstItemSupplier.OrderByDescending(t => t.ID).ToList());
        }

        public JsonResult SavetoSeesionItemSupplier(Int64 ID, Int32 SessionSr, Guid GUID, Guid ITEMGUID, Int64 SupplierID, string SupplierName, string SupplierNumber, bool IsDefault, Nullable<Int64> BlanketPOID)
        {
            string itemGuidString = Convert.ToString(ITEMGUID);
            List<ItemSupplierDetailsDTO> lstBinReplanish = null;

            if (Session["ItemSupplier" + itemGuidString] != null)
            {
                lstBinReplanish = (List<ItemSupplierDetailsDTO>)Session["ItemSupplier" + itemGuidString];
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
                    Session["ItemSupplier" + itemGuidString] = lstBinReplanish;
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
                        objDTO.Room = SessionHelper.RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.RoomName = SessionHelper.RoomName;
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
                    objDTO.Room = SessionHelper.RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.RoomName = SessionHelper.RoomName;
                    objDTO.UpdatedByName = SessionHelper.UserName;
                    objDTO.CreatedByName = SessionHelper.UserName;
                    objDTO.SessionSr = lstBinReplanish.Count + 1;
                    lstBinReplanish.Add(objDTO);
                }
            }
            Session["ItemSupplier" + itemGuidString] = lstBinReplanish;
            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeletetoSeesionItemSupplierSingle(Int64 ID, Int32 SessionSr, Guid GUID, Guid ITEMGUID, Int64 SupplierID, string SupplierNumber, bool IsDefault, Nullable<Int64> BlanketPOID)
        {
            string itemGuidString = Convert.ToString(ITEMGUID);
            List<ItemSupplierDetailsDTO> lstBinReplanish = null;
            if (Session["ItemSupplier" + itemGuidString] != null)
            {
                lstBinReplanish = (List<ItemSupplierDetailsDTO>)Session["ItemSupplier" + itemGuidString];
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
                    Session["ItemSupplier" + itemGuidString] = lstBinReplanish;
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
                    Session["ItemSupplier" + itemGuidString] = lstBinReplanish;
                }
                catch
                {
                    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "deleted" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CheckDefaultExistinSession()
        {
            List<ItemSupplierDetailsDTO> lstBinReplanish = null;
            if (Session["ItemSupplier"] != null)
            {
                lstBinReplanish = (List<ItemSupplierDetailsDTO>)Session["ItemSupplier"];
            }

            if (lstBinReplanish != null)
            {
                try
                {
                    if (lstBinReplanish.Where(t => t.IsDefault == true && !string.IsNullOrEmpty(t.SupplierNumber)).Count() > 0)
                    {
                        return Json(new { status = "exist" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch
                {
                    return Json(new { status = "notexist" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { status = "notexist" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSupplierBlanketPO(Int64 SupplierID, string SupplierName)
        {
            SupplierMasterDAL objSupDal = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            SupplierMasterDTO objDTO = objSupDal.GetSupplierByNamePlain(SessionHelper.RoomID, SessionHelper.CompanyID, false, SupplierName.Trim());

            if (objDTO == null || objDTO.ID == 0)
            {
                return Json(new { status = "Not Found" }, JsonRequestBehavior.AllowGet);
            }

            SupplierBlanketPODetailsDAL objDAL = new SupplierBlanketPODetailsDAL(SessionHelper.EnterPriseDBName);
            List<SupplierBlanketPODetailsDTO> result = objDAL.GetBlanketPOBySupplierIDPlain(SupplierID, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();

            if (result == null || result.Count == 0)
            {
                return Json(new { status = "Not Found" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = "Found", result }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetSupplierBlanketPOItemGUID(Int64 SupplierID, string SupplierName, bool IsForBOM)
        {
            SupplierMasterDAL objSupDal = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            SupplierMasterDTO objDTO = objSupDal.GetSupplierByNamePlain(SessionHelper.RoomID, SessionHelper.CompanyID, IsForBOM, SupplierName.Trim());

            if (objDTO == null || objDTO.ID == 0)
            {
                return Json(new { status = "Not Found" }, JsonRequestBehavior.AllowGet);
            }

            SupplierBlanketPODetailsDAL objDAL = new SupplierBlanketPODetailsDAL(SessionHelper.EnterPriseDBName);
            List<SupplierBlanketPODetailsDTO> result = objDAL.GetBlanketPOBySupplierIDAndItemGUID(SupplierID, SessionHelper.RoomID, SessionHelper.CompanyID, ItemGUID: Guid.Empty).ToList();
            result = result.OrderBy(x => x.BlanketPO.Trim()).ToList();
            if (result == null || result.Count == 0)
            {
                return Json(new { status = "Not Found" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = "Found", result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetNewSupplier(Int64 SupplierID, string SupplierName, bool IsForBOM)
        {
            SupplierMasterDAL objSupDal = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            SupplierMasterDTO objDTO = objSupDal.GetSupplierByNamePlain(SessionHelper.RoomID, SessionHelper.CompanyID, IsForBOM, SupplierName.Trim());

            if (objDTO == null || objDTO.ID == 0)
            {
                return Json(new { status = "Not Found" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = "Ok" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region "Kit Component"

        public ActionResult LoadItemKitModel(string Parentid, string ParentGuid)
        {
            ItemModelPerameter obj = new ItemModelPerameter()
            {
                AjaxURLAddItemToSession = "~/Inventory/AddItemToDetailTableKit/",
                PerentID = Parentid,
                PerentGUID = ParentGuid,
                ModelHeader = ResKitMaster.MsgAddKitComponentToKit,
                AjaxURLAddMultipleItemToSession = "~/Inventory/AddItemToDetailTableKit/",
                AjaxURLToFillItemGrid = "~/Inventory/GetItemsModelMethodKit/",
                CallingFromPageName = "KIT"
            };

            return PartialView("ItemMasterModel", obj);
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
                item.Room = SessionHelper.RoomID;
                item.RoomName = SessionHelper.RoomName;
                item.CreatedBy = SessionHelper.UserID;
                item.CreatedByName = SessionHelper.UserName;
                item.UpdatedByName = SessionHelper.UserName;
                item.LastUpdatedBy = SessionHelper.UserID;
                item.CompanyID = SessionHelper.CompanyID;
                item.LastUpdated = DateTimeUtility.DateTimeNow;

                string itemGuidString = Convert.ToString(item.KitGUID.GetValueOrDefault(Guid.Empty));

                if (!(item.GUID != null && item.GUID != Guid.Empty))
                {
                    item.Created = DateTimeUtility.DateTimeNow;
                    ItemMasterDTO ObjItemDTO = objItemDAL.GetItemWithoutJoins(null, item.ItemGUID);
                    /* Commented for WI-4660 */
                    //if (ObjItemDTO != null && ObjItemDTO.ItemType != 3)
                    //{
                    List<KitDetailDTO> lstBinReplanish = null;
                    if (Session["ItemKitDetail" + itemGuidString] != null)
                    {
                        lstBinReplanish = (List<KitDetailDTO>)Session["ItemKitDetail" + itemGuidString];
                        item.SessionSr = lstBinReplanish.Count + 1;
                    }
                    else
                    {
                        item.SessionSr = sessionsr + 1;
                        lstBinReplanish = new List<KitDetailDTO>();
                    }
                    lstBinReplanish.Add(item);
                    Session["ItemKitDetail" + itemGuidString] = lstBinReplanish;
                    //}
                }
            }

            message = ResAssetMaster.MsgItemQtyUpdated;
            status = "ok";
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetItemsModelMethodKit(JQueryDataTableParamModel param)
        {
            //ItemMasterController obj = new ItemMasterController();
            ItemMasterDAL obj = new eTurns.DAL.ItemMasterDAL(SessionHelper.EnterPriseDBName);

            //LoadTestEntities entity = new LoadTestEntities();
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
            //if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID";

            //if (sortDirection == "asc")
            //    sortColumnName = sortColumnName + " asc";
            //else
            //    sortColumnName = sortColumnName + " desc";

            //make changes to resolve an issue of Sort (WI-431)
            if (sortColumnName == "0" || sortColumnName.Contains("undefined"))
                sortColumnName = "ItemNumber Asc";

            string searchQuery = string.Empty;

            Guid QLID = Guid.Empty;
            Guid.TryParse(Convert.ToString(Request["ParentGUID"]), out QLID);

            int TotalRecordCount = 0;
            string itemGuidString = Convert.ToString(QLID);
            //object objQLItems = SessionHelper.Get(QuickListSessionKey + param.QuickListID.ToString());
            //List<KitDetailDTO> objQLItems = new KitDetailDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByKitGUID(QLID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, true).ToList();
            List<KitDetailDTO> objQLItems = null;
            if (Session["ItemKitDetail" + itemGuidString] != null)
            {
                objQLItems = (List<KitDetailDTO>)Session["ItemKitDetail" + itemGuidString];
            }

            string ItemsIDs = "";
            if (objQLItems != null && objQLItems.Count > 0)
            {
                foreach (var item in objQLItems)
                {
                    if (!string.IsNullOrEmpty(ItemsIDs))
                        ItemsIDs += ",";

                    ItemsIDs += item.ItemGUID.ToString();
                }
            }

            // .Where(x=>x.ItemType != 4); , as Labour Type item not required in this module
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetPagedRecordsForModel(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, ItemsIDs, "kit", "1", SessionHelper.UserSupplierIds, RoomDateFormat, CurrentTimeZone, "", true, true);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveKitQty(Guid ItemGUID, Guid KitItemGuid, double? QuantityPerKit)
        {
            string itemGuidString = Convert.ToString(ItemGUID);
            List<KitDetailDTO> lstKitDetailDTO = (List<KitDetailDTO>)Session["ItemKitDetail" + itemGuidString];

            if (lstKitDetailDTO != null && lstKitDetailDTO.Count > 0)
            {
                KitDetailDTO objKitDetailDTO = lstKitDetailDTO.Where(t => t.ItemGUID == KitItemGuid).FirstOrDefault();
                if (objKitDetailDTO != null)
                {
                    objKitDetailDTO.QuantityPerKit = QuantityPerKit;
                }
                lstKitDetailDTO = lstKitDetailDTO.Where(t => t.ItemGUID != KitItemGuid).ToList();
                lstKitDetailDTO.Add(objKitDetailDTO);
                Session["ItemKitDetail" + itemGuidString] = lstKitDetailDTO;
            }
            return Json(true);

        }

        //LoadKitofItem
        public ActionResult LoadKitComponentofItem(Guid ItemGUID, int? AddCount)
        {
            ViewBag.ItemGUID = ItemGUID;
            List<KitDetailDTO> lstKitDetailDTO = null;
            string itemGuidString = Convert.ToString(ItemGUID);
            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            ItemMasterDTO objDTO = obj.GetItemWithoutJoins(null, ItemGUID);

            if (objDTO != null && objDTO.IsBOMItem == false && objDTO.RefBomId.GetValueOrDefault(0) > 0)
            {
                ViewBag.IsBOMItem = true;
            }
            else
            {
                ViewBag.IsBOMItem = false;
            }

            if (Session["ItemKitDetail" + itemGuidString] != null)
            {
                lstKitDetailDTO = (List<KitDetailDTO>)Session["ItemKitDetail" + itemGuidString];
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
            return PartialView("_ItemKitComponent", lstKitDetailDTO.OrderBy(t => t.ItemNumber).ToList());
        }

        public JsonResult SavetoSeesionItemKitComponent(Int64 ID, Int32 SessionSr, Guid GUID, Guid ITEMGUID, Guid KitGUID, double? QuantityPerKit)
        {
            string itemGuidString = Convert.ToString(KitGUID);
            List<KitDetailDTO> lstBinReplanish = null;

            if (Session["ItemKitDetail" + itemGuidString] != null)
            {
                lstBinReplanish = (List<KitDetailDTO>)Session["ItemKitDetail" + itemGuidString];
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
            Session["ItemKitDetail" + itemGuidString] = lstBinReplanish;

            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeletetoSeesionItemKitComponentSingle(Int64 ID, Int32 SessionSr, Guid GUID, Guid ITEMGUID, Guid KitGUID, double QuantityPerKit)
        {
            string itemGuidString = Convert.ToString(KitGUID);
            List<KitDetailDTO> lstBinReplanish = null;
            if (Session["ItemKitDetail" + itemGuidString] != null)
            {
                lstBinReplanish = (List<KitDetailDTO>)Session["ItemKitDetail" + itemGuidString];
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
                    if (objKitDetailDAL.IsKitItemDeletable(GUID.ToString(), SessionHelper.RoomID, SessionHelper.CompanyID))
                    {
                        lstBinReplanish.Remove(lstBinReplanish.Where(t => t.ID == ID).FirstOrDefault());

                        Session["ItemKitDetail" + itemGuidString] = lstBinReplanish;
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

                    Session["ItemKitDetail" + itemGuidString] = lstBinReplanish;
                }
                catch
                {
                    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "deleted" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        public PartialViewResult ItemOnOrderQty(Guid ItemGuid)
        {
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<ItemQuantityDetail> lstItemQtyDetail = objCommonDAL.GetOnOrderDetailFromCache(ItemGuid, SessionHelper.RoomID, SessionHelper.CompanyID);
            return PartialView("_ItemOnOrderQty", lstItemQtyDetail);
        }
        public PartialViewResult ItemOnQuoteQty(Guid ItemGuid)
        {
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<ItemQuantityDetail> lstItemQtyDetail = objCommonDAL.GetOnQuoteDetailFromCache(ItemGuid, SessionHelper.RoomID, SessionHelper.CompanyID);
            return PartialView("_ItemOnQuoteQty", lstItemQtyDetail);
        }
        public PartialViewResult ItemOnOrderQtyWithBin(Guid ItemGuid, Int64 BinId)
        {
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<ItemQuantityDetail> lstItemQtyDetail = objCommonDAL.GetOnOrderDetailFromCache(ItemGuid, SessionHelper.RoomID, SessionHelper.CompanyID, BinId);
            return PartialView("_ItemOnOrderQty", lstItemQtyDetail);
        }
        public PartialViewResult ItemOnOrderInTransitQty(Guid ItemGuid)
        {
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<ItemQuantityDetail> lstItemQtyDetail = objCommonDAL.GetOnOrderDetailInTransitFromCache(ItemGuid, SessionHelper.RoomID, SessionHelper.CompanyID);
            return PartialView("_ItemOnOrderQty", lstItemQtyDetail);
        }

        public PartialViewResult ItemOnOrderInTransitQtyWithBin(Guid ItemGuid, Int64 BinId)
        {
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<ItemQuantityDetail> lstItemQtyDetail = objCommonDAL.GetOnOrderDetailInTransitFromCache(ItemGuid, SessionHelper.RoomID, SessionHelper.CompanyID, BinId);
            return PartialView("_ItemOnOrderQty", lstItemQtyDetail);
        }

        public PartialViewResult ItemTurnsQty(Guid ItemGuid)
        {
            DashboardDAL objCommonDAL = new DashboardDAL(SessionHelper.EnterPriseDBName);
            var measurementPullTransfer = ResDashboard.MeasurementPullTransfer;
            var measurementPullTransferValue = ResDashboard.MeasurementPullTransferValue;
            var measurementOrder = ResDashboard.MeasurementOrders;
            ItemTransationInfo lstItemQtyDetail = objCommonDAL.GetItemTxnHistory("turns", SessionHelper.RoomID, SessionHelper.CompanyID, 0, ItemGuid,measurementPullTransfer,measurementPullTransferValue,measurementOrder);
            return PartialView("_ItemTurnsQty", lstItemQtyDetail);
        }

        public PartialViewResult ItemAverageUsageQty(Guid ItemGuid)
        {
            DashboardDAL objCommonDAL = new DashboardDAL(SessionHelper.EnterPriseDBName);
            var measurementPullTransfer = ResDashboard.MeasurementPullTransfer;
            var measurementPullTransferValue = ResDashboard.MeasurementPullTransferValue;
            var measurementOrder = ResDashboard.MeasurementOrders;
            ItemTransationInfo lstItemQtyDetail = objCommonDAL.GetItemTxnHistory("averageusage", SessionHelper.RoomID, SessionHelper.CompanyID, 0, ItemGuid, measurementPullTransfer, measurementPullTransferValue, measurementOrder);
            return PartialView("_ItemTurnsQty", lstItemQtyDetail);
        }

        public PartialViewResult ItemOnReturnQty(Guid ItemGuid)
        {
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<ItemQuantityDetail> lstItemQtyDetail = objCommonDAL.GetOnReturnOrderDetailFromCache(ItemGuid, SessionHelper.RoomID, SessionHelper.CompanyID);
            return PartialView("_ItemOnOrderQty", lstItemQtyDetail);
        }

        public PartialViewResult ItemOnReturnQtyWithBin(Guid ItemGuid, Int64 BinId)
        {
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<ItemQuantityDetail> lstItemQtyDetail = objCommonDAL.GetOnReturnOrderDetailFromCache(ItemGuid, SessionHelper.RoomID, SessionHelper.CompanyID, BinId);
            return PartialView("_ItemOnOrderQty", lstItemQtyDetail);
        }

        public PartialViewResult ItemOnRequisationQty(Guid ItemGuid)
        {
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<ItemQuantityDetail> lstItemQtyDetail = objCommonDAL.GetOnRequisitionDetailFromCache(ItemGuid, SessionHelper.RoomID, SessionHelper.CompanyID);
            return PartialView("_ItemOnRequesitionedQty", lstItemQtyDetail);
        }

        public PartialViewResult ItemOnRequisationQtyWithBin(Guid ItemGuid, Int64 BinId)
        {
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<ItemQuantityDetail> lstItemQtyDetail = objCommonDAL.GetOnRequisitionDetailFromCache(ItemGuid, SessionHelper.RoomID, SessionHelper.CompanyID, BinId);
            return PartialView("_ItemOnRequesitionedQty", lstItemQtyDetail);
        }

        public PartialViewResult ItemOnTransferQty(Guid ItemGuid)
        {
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<ItemQuantityDetail> lstItemQtyDetail = objCommonDAL.GetOnTransferQtyDetailFromCache(ItemGuid, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserSupplierIds);
            return PartialView("_ItemOnTransferQty", lstItemQtyDetail);
        }
        public PartialViewResult ItemOnTransferQtyWithBin(Guid ItemGuid, Int64 BinId)
        {
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<ItemQuantityDetail> lstItemQtyDetail = objCommonDAL.GetOnTransferQtyDetailFromCacheWithBin(ItemGuid, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserSupplierIds, BinId);
            return PartialView("_ItemOnTransferQty", lstItemQtyDetail);
        }
        public PartialViewResult ItemOnTransferInTransitQty(Guid ItemGuid)
        {
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<ItemQuantityDetail> lstItemQtyDetail = objCommonDAL.GetOnTransferQtyInTransitDetailFromCache(ItemGuid, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserSupplierIds);
            return PartialView("_ItemOnTransferQty", lstItemQtyDetail);
        }
        public PartialViewResult ItemOnTransferInTransitQtyWithBin(Guid ItemGuid, Int64 BinId)
        {
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<ItemQuantityDetail> lstItemQtyDetail = objCommonDAL.GetOnTransferQtyInTransitDetailFromCache(ItemGuid, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserSupplierIds, BinId);
            return PartialView("_ItemOnTransferQty", lstItemQtyDetail);
        }
        public PartialViewResult ItemInTransitQty(Guid ItemGuid)
        {
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<ItemQuantityDetail> lstItemQtyDetail = objCommonDAL.GetInTransitQtyDetailFromCache(ItemGuid, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserSupplierIds);
            return PartialView("_ItemOnTransferQty", lstItemQtyDetail);
        }

        public PartialViewResult ItemStagedQuantity(Guid ItemGuid)
        {
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<ItemQuantityDetail> lstItemQtyDetail = objCommonDAL.GetOnStageQuantityDetailFromCache(ItemGuid, SessionHelper.RoomID, SessionHelper.CompanyID);
            return PartialView("_ItemStagedQuantity", lstItemQtyDetail);
        }

        //[HttpGet]
        //public PartialViewResult MoveMaterialView(Int64 BinId, Guid ItemGuid)
        //{
        //    //ItemLocationDetailsDTO objLocationDetailDTO = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetRecord(LocationGuid, SessionHelper.RoomID, SessionHelper.CompanyID);
        //    ItemLocationQTYDTO objItemLocationQty = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName).GetRecordByBinItem(ItemGuid, BinId, SessionHelper.RoomID, SessionHelper.CompanyID);
        //    ItemLocationDetailsDTO objLocationDetailDTO = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetAllRecordsBinWise(BinId, SessionHelper.RoomID, SessionHelper.CompanyID, ItemGuid, null, "ID Desc").FirstOrDefault();

        //    if (objLocationDetailDTO != null)
        //    {
        //        objItemLocationQty.ItemNumber = objLocationDetailDTO.ItemNumber;
        //        objItemLocationQty.BinNumber = objLocationDetailDTO.BinNumber;
        //        objItemLocationQty.SerialNumberTracking = objLocationDetailDTO.SerialNumberTracking;

        //        //List<BinMasterDTO> listBinDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(x => x.IsStagingLocation == false && x.ID != BinId).ToList();
        //        //listBinDTO.Insert(0, new BinMasterDTO() { BinNumber = "--Select Bin--", ID = 0 });
        //        //ViewBag.BinLocations = listBinDTO;
        //    }
        //    return PartialView("_MoveMaterialPopup", objItemLocationQty);

        //}

        //[HttpPost]
        //public JsonResult SaveMoveQuantityToLocation(Int64 BinId, Guid ItemGuid, Int64 MoveBinID, double Quantity, string LocationName)
        //{
        //    if (MoveBinID <= 0 && !string.IsNullOrEmpty(LocationName))
        //    {
        //        BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
        //        BinMasterDTO objBinMasterDTO = objBinMasterDAL.GetRecord(LocationName, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
        //        //BinMasterDTO objBinMasterDTO = objBinMasterDAL.GetBinMasterByLocationName( SessionHelper.RoomID, SessionHelper.CompanyID, false, false, LocationName).FirstOrDefault();
        //        if (objBinMasterDTO == null) // new entry - not matching id and uom
        //        {
        //            objBinMasterDTO = new BinMasterDTO();
        //            objBinMasterDTO.ID = 0;
        //            objBinMasterDTO.CompanyID = SessionHelper.CompanyID;
        //            objBinMasterDTO.Created = DateTimeUtility.DateTimeNow;
        //            objBinMasterDTO.CreatedBy = SessionHelper.UserID;
        //            objBinMasterDTO.CreatedByName = SessionHelper.UserName;
        //            objBinMasterDTO.IsArchived = false;
        //            objBinMasterDTO.IsDeleted = false;
        //            objBinMasterDTO.LastUpdatedBy = SessionHelper.UserID;
        //            objBinMasterDTO.Room = SessionHelper.RoomID;
        //            objBinMasterDTO.RoomName = SessionHelper.RoomName;
        //            objBinMasterDTO.LastUpdated = DateTimeUtility.DateTimeNow;
        //            objBinMasterDTO.UpdatedByName = SessionHelper.UserName;
        //            objBinMasterDTO.BinNumber = LocationName;
        //            objBinMasterDTO.IsStagingLocation = false;
        //            objBinMasterDTO.AddedFrom = "Web";
        //            objBinMasterDTO.EditedFrom = "Web";
        //            objBinMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
        //            objBinMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
        //            objBinMasterDTO.ID = objBinMasterDAL.Insert(objBinMasterDTO);
        //            MoveBinID = objBinMasterDTO.ID;
        //        }
        //    }
        //    else if (LocationName.Trim().Length > 0)
        //    {
        //        BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
        //        BinMasterDTO objBinMasterDTO = objBinMasterDAL.GetRecord(LocationName, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
        //        //BinMasterDTO objBinMasterDTO = objBinMasterDAL.GetBinMasterByLocationName( SessionHelper.RoomID, SessionHelper.CompanyID, false, false, LocationName).FirstOrDefault();
        //        MoveBinID = objBinMasterDTO.ID;
        //    }

        //    ItemLocationDetailsDAL objItemLocDetailDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
        //    bool isSaved = objItemLocDetailDAL.MoveQuanityToLocation(BinId, ItemGuid, MoveBinID, Quantity, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);

        //    return Json(new { Message = "Save Success fully", Status = "ok" }, JsonRequestBehavior.AllowGet);
        //}

        public bool GetIsIncludePermission(long CompanyId)
        {
            CompanyMasterDAL objCompanyMasterDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
            return objCompanyMasterDAL.GetIsIncludePermission(CompanyId);
        }

        public ActionResult AddLocationsFromGrid(string ItemID_ItemType)
        {
            string ItemGUID = ItemID_ItemType.Split('#')[0];
            Int32 ItemType = Int32.Parse(ItemID_ItemType.Split('#')[1]);
            ViewBag.ItemGUID = ItemGUID;
            ViewBag.ItemType = ItemType;
            Guid ItemGUID1 = Guid.Empty;
            ItemMasterDTO objItem = null;
            if (Guid.TryParse(ItemGUID, out ItemGUID1))
            {
                objItem = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, ItemGUID1);
            }

            ViewBag.someval = "Übersetzung";
            return PartialView("AddLocationsFromGrid", objItem);
        }

        public ActionResult AddLocationsFromGridByRoomAndCompany(string ItemID_ItemType)
        {
            string ItemGUID = ItemID_ItemType.Split('#')[0];
            Int32 ItemType = Int32.Parse(ItemID_ItemType.Split('#')[1]);
            ViewBag.ItemGUID = ItemGUID;
            ViewBag.ItemType = ItemType;
            Guid ItemGUID1 = Guid.Empty;
            ItemMasterDTO objItem = null;
            if (Guid.TryParse(ItemGUID, out ItemGUID1))
            {
                objItem = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, ItemGUID1);
            }

            ViewBag.someval = "Übersetzung";
            return PartialView("AddLocationsFromGridByRoomAndCompany", objItem);
        }

        [HttpPost]
        public JsonResult GetItemBins(Guid ItemGUID)
        {
            List<BinMasterDTO> lstBins = new List<BinMasterDTO>();
            BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            lstBins = objBinMasterDAL.GetItemLocations(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
            return Json(lstBins);

        }

        [HttpPost]
        public JsonResult GetItemBinsByRoomAndCompany(Guid ItemGUID)
        {
            List<BinMasterDTO> lstBins = new List<BinMasterDTO>();
            BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            ItemMasterDTO objItem = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, ItemGUID);
            lstBins = objBinMasterDAL.GetItemLocations(ItemGUID, objItem.Room ?? 0, objItem.CompanyID ?? 0);
            return Json(lstBins);

        }
        [HttpPost]
        public JsonResult SaveItemLocations(List<BinMasterDTO> lstBinReplanish, Guid? ItemGUID)
        {

            if (lstBinReplanish != null && lstBinReplanish.Count > 0)
            {
                long SessionUserId = SessionHelper.UserID;
                BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                double? UpdatedOnHandQuantity = 0;
                List<long> insertedBinIds = new List<long>();
                lstBinReplanish = objBinMasterDAL.AssignUpdateItemLocations(lstBinReplanish, ItemGUID ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, false, out UpdatedOnHandQuantity, out insertedBinIds, SessionUserId);
                if (UpdatedOnHandQuantity != null)
                {
                    ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                    objItemMasterDAL.UpdateOnHandQuantity((Guid)ItemGUID, (double)UpdatedOnHandQuantity);
                }
                UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
                UDFController objUDFController = new UDFController();
                IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetNonDeletedUDFsByUDFTableNamePlain("BinUDF", SessionHelper.RoomID, SessionHelper.CompanyID);
                string udfRequier = string.Empty;
                //string message = string.Empty;
                string ErrorMessage = string.Empty;
                foreach (var item in lstBinReplanish)
                {
                    foreach (var i in DataFromDB)
                    {
                        if (i.UDFControlType.ToLower().Contains("dropdown editable"))
                        {
                            if (i.UDFColumnName == "UDF1" && !string.IsNullOrWhiteSpace(item.BinUDF1))
                            {
                                objUDFController.InsertUDFOption(i.ID, item.BinUDF1, "BinUDF", null, false, false, "");
                            }
                            else if (i.UDFColumnName == "UDF2" && !string.IsNullOrWhiteSpace(item.BinUDF2))
                            {
                                objUDFController.InsertUDFOption(i.ID, item.BinUDF2, "BinUDF", null, false, false, "");
                            }
                            else if (i.UDFColumnName == "UDF3" && !string.IsNullOrWhiteSpace(item.BinUDF3))
                            {
                                objUDFController.InsertUDFOption(i.ID, item.BinUDF3, "BinUDF", null, false, false, "");
                            }
                            else if (i.UDFColumnName == "UDF4" && !string.IsNullOrWhiteSpace(item.BinUDF4))
                            {
                                objUDFController.InsertUDFOption(i.ID, item.BinUDF4, "BinUDF", null, false, false, "");
                            }
                            else if (i.UDFColumnName == "UDF5" && !string.IsNullOrWhiteSpace(item.BinUDF5))
                            {
                                objUDFController.InsertUDFOption(i.ID, item.BinUDF5, "BinUDF", null, false, false, "");
                            }
                        }
                    }
                }

            }
            return Json(lstBinReplanish);
        }

        [HttpPost]
        public JsonResult SaveItemLocationsByRoomAndCompany(List<BinMasterDTO> lstBinReplanish, int RoomId, int CompanyID, Guid? ItemGUID)
        {

            if (lstBinReplanish != null && lstBinReplanish.Count > 0)
            {
                long SessionUserId = SessionHelper.UserID;
                BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                double? UpdatedOnHandQuantity = 0;
                List<long> insertedBinIds = new List<long>();
                lstBinReplanish = objBinMasterDAL.AssignUpdateItemLocations(lstBinReplanish, ItemGUID ?? Guid.Empty, RoomId, CompanyID, SessionHelper.UserID, false, out UpdatedOnHandQuantity, out insertedBinIds, SessionUserId);
                if (UpdatedOnHandQuantity != null)
                {
                    ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                    objItemMasterDAL.UpdateOnHandQuantity((Guid)ItemGUID, (double)UpdatedOnHandQuantity);
                }
            }
            return Json(lstBinReplanish);
        }

        /// <summary>
        /// ItemLocationDetailsSaveForKitCredit
        /// </summary>
        /// <param name="objData"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ItemLocationDetailsSaveForKitCredit(List<ItemLocationDetailsDTO> objData)
        {
            string message = "Fail";
            string status = "Fail";

            Guid KitDetailGUID = Guid.Empty;
            Guid? ItemGuid = Guid.Empty;
            double CustomerQty = 0;
            double ConsignedQty = 0;
            double TotalQty = 0;
            foreach (ItemLocationDetailsDTO objItemLocationDetails in objData)
            {
                if (string.IsNullOrEmpty(objItemLocationDetails.BinNumber.Trim()))
                {
                    message = string.Format(ResMessage.Required, ResBin.BinNumber);// "Please Select Bin Number";
                    status = "Fail";
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                }
            }

            List<KitMoveInOutDetailDTO> objListKitMoveInoutDTO = new List<KitMoveInOutDetailDTO>();
            KitDetailDAL objKitDetailCtrl = new KitDetailDAL(SessionHelper.EnterPriseDBName);
            KitDetailDTO objKitDetailDTO = null;

            KitMoveInOutDetailDAL objKitMoveInOutCtrl = new KitMoveInOutDetailDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            BinMasterDAL objItemLocationDetailsDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            BinMasterDTO objBinMasterDTO;
            foreach (var item in objData)
            {

                CustomerQty = 0;
                ConsignedQty = 0;

                item.CompanyID = SessionHelper.CompanyID;
                item.Room = SessionHelper.RoomID;
                if (!string.IsNullOrEmpty(item.BinNumber) && item.BinID.GetValueOrDefault(0) <= 0)
                {
                    objBinMasterDTO = new BinMasterDTO();
                    objBinMasterDTO = objItemLocationDetailsDAL.GetItemBinPlain(item.ItemGUID.GetValueOrDefault(Guid.Empty), item.BinNumber, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, false);
                    item.BinID = objBinMasterDTO.ID;
                    //item.BinID = objCommonDAL.GetOrInsertBinIDByName(item.ItemGUID.GetValueOrDefault(Guid.Empty), item.BinNumber, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, false);
                }
                if (item.ID == 0)
                {
                    item.Created = DateTimeUtility.DateTimeNow;
                    item.CreatedBy = SessionHelper.UserID;
                    item.CreatedByName = SessionHelper.UserName;
                }
                item.Updated = DateTimeUtility.DateTimeNow;
                item.LastUpdatedBy = SessionHelper.UserID;
                item.UpdatedByName = SessionHelper.UserName;

                item.ItemGUID = item.ItemGUID;
                ItemGuid = item.ItemGUID;
                //TODO: Added By CP
                if (item.KitDetailGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {
                    KitDetailGUID = item.KitDetailGUID.GetValueOrDefault(Guid.Empty);

                    if (objKitDetailDTO == null)
                    {
                        objKitDetailDTO = objKitDetailCtrl.GetRecord(KitDetailGUID.ToString(), SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false);
                    }

                    if (!item.SerialNumberTracking)
                    {
                        if (item.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                        {
                            CustomerQty += double.Parse(item.CustomerOwnedQuantity.ToString());
                        }

                        else if (item.ConsignedQuantity.GetValueOrDefault(0) > 0)
                        {
                            ConsignedQty += double.Parse(item.ConsignedQuantity.ToString());
                        }
                        else
                        {
                            CustomerQty += 0;
                            ConsignedQty += 0;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(item.SerialNumber) && item.CustomerOwnedQuantity.GetValueOrDefault(0) > 0)
                        {
                            CustomerQty += double.Parse(item.CustomerOwnedQuantity.ToString());
                        }
                        else if (!string.IsNullOrEmpty(item.SerialNumber) && item.ConsignedQuantity.GetValueOrDefault(0) > 0)
                        {
                            ConsignedQty += double.Parse(item.ConsignedQuantity.ToString());
                        }
                        else
                        {
                            CustomerQty += 0;
                            ConsignedQty += 0;
                        }

                    }
                    if (CustomerQty + ConsignedQty > 0)
                    {
                        KitMoveInOutDetailDTO KitMoveInOutDTO = new KitMoveInOutDetailDTO()
                        {
                            BinID = item.BinID,
                            CompanyID = SessionHelper.CompanyID,
                            ConsignedQuantity = ConsignedQty,
                            Created = DateTimeUtility.DateTimeNow,
                            CreatedBy = SessionHelper.UserID,
                            CreatedByName = SessionHelper.UserName,
                            CustomerOwnedQuantity = CustomerQty,
                            GUID = Guid.Empty,
                            ID = 0,
                            IsArchived = false,
                            IsDeleted = false,
                            ItemGUID = item.ItemGUID,
                            KitDetailGUID = objKitDetailDTO.GUID,
                            LastUpdatedBy = SessionHelper.UserID,
                            MoveInOut = "OUT",
                            Room = SessionHelper.RoomID,
                            RoomName = SessionHelper.RoomName,
                            TotalQuantity = CustomerQty + ConsignedQty,
                            Updated = DateTimeUtility.DateTimeNow,
                            UpdatedByName = SessionHelper.UserName

                        };

                        objListKitMoveInoutDTO.Add(KitMoveInOutDTO);
                    }
                }
                //End: Added By CP
            }


            ItemLocationDetailsDAL obj = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            if (KitDetailGUID != Guid.Empty && objListKitMoveInoutDTO != null && objListKitMoveInoutDTO.Count > 0 && objKitDetailDTO != null)
            {
                TotalQty = objListKitMoveInoutDTO.Sum(x => x.TotalQuantity);
                if (TotalQty <= objKitDetailDTO.AvailableItemsInWIP)
                {

                    // Comment code 
                    /*
                    //<Added-by-AmitP> Get Previous Records, which are stored...
                    ItemLocationDetailsDAL objAPI = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                    List<ItemLocationDetailsDTO> lstTemp = objAPI.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, ItemGuid.Value, null, "ID DESC").ToList();

                    ItemMasterDAL objItemAPI = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                    var Objitem = objItemAPI.GetRecord(ItemGuid.Value.ToString(), SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

                    lstTemp.Select(c =>
                    {
                        c.SerialNumberTracking = Objitem.SerialNumberTracking;
                        c.LotNumberTracking = Objitem.LotNumberTracking;
                        c.DateCodeTracking = Objitem.DateCodeTracking;
                        return c;
                    }).ToList();

                    foreach (var iTemp in lstTemp)
                    {
                        if (objData.FindIndex(x => x.ID == iTemp.ID) < 0)
                            objData.Add(iTemp);
                    }
                    //</Added-by-AmitP>
                    */
                    long SessionUserId = SessionHelper.UserID;
                    if (obj.ItemLocationDetailsSaveForCreditKit(objData, SessionUserId,SessionHelper.EnterPriceID))
                    {
                        objKitDetailDTO.AvailableItemsInWIP = objKitDetailDTO.AvailableItemsInWIP - TotalQty;
                        objKitDetailDTO.UpdatedByName = SessionHelper.UserName;
                        objKitDetailDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        objKitDetailDTO.LastUpdatedBy = SessionHelper.UserID;
                        objKitDetailCtrl.Edit(objKitDetailDTO, SessionUserId,SessionHelper.EnterPriceID);

                        foreach (var item in objListKitMoveInoutDTO)
                        {
                            objKitMoveInOutCtrl.Insert(item);
                        }

                        message = "Success";
                        status = "OK";
                    }
                    else
                        message = ResCommon.NotUpdated;
                }
                else
                    message = ResKitMaster.MoveOutQtyLessThanEqualsWIP;

            }


            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }
        public string DeletePermissiontemplates(string ids)
        {
            try
            {
                PermissionTemplateDAL objInventoryCountDAL = new PermissionTemplateDAL(SessionHelper.EnterPriseDBName);
                objInventoryCountDAL.DeleteRecords(ids, SessionHelper.UserID, SessionHelper.CompanyID);
                return "ok";
            }
            catch (Exception)
            {

                return "fail";
            }

        }
        public JsonResult UnDeletePermissiontemplates(string ids, string ModuleName)
        {
            try
            {
                PermissionTemplateDAL objInventoryCountDAL = new PermissionTemplateDAL(SessionHelper.EnterPriseDBName);
                objInventoryCountDAL.UnDeleteRecords(ids, SessionHelper.UserID, SessionHelper.CompanyID);
                return Json(new { Message = "Success", Status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new { Message = "fail", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetItemsBinForLabels(string itemsguids)
        {
            //System.Xml.Linq.XElement Settinfile = System.Xml.Linq.XElement.Load(System.Web.HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
            //string BOMInvPhotoPathRoot = Settinfile.Element("ItemBinBarcodeNewPage") != null ? Settinfile.Element("ItemBinBarcodeNewPage").Value : "false";
            string BOMInvPhotoPathRoot = SiteSettingHelper.ItemBinBarcodeNewPage != string.Empty ? SiteSettingHelper.ItemBinBarcodeNewPage : "false";
            if (!Convert.ToBoolean(BOMInvPhotoPathRoot))
            {

                BinMasterDAL binDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                List<ItemsBins> lst = binDAL.GetItemsBinForLabels(itemsguids);
                return PartialView("_ItemsBinForLabels", lst);
            }
            else
            {
                ViewBag.itemsguids = itemsguids;
                return PartialView("_ItemsBinForLabelsNew");
            }
        }
        public ActionResult GetItemsBinForLabelsListAjax(QuickListJQueryDataTableParamModel param)
        {

            //BinMasterDAL binDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            //List<ItemsBins> lst = binDAL.GetItemsBinForLabels(itemsguids);
            string sortColumnName = string.Empty;
            sortColumnName = Request["SortingField"].ToString();
            if (string.IsNullOrEmpty(sortColumnName) || sortColumnName == "undefine" || sortColumnName == "0" || sortColumnName.Trim().Contains("null") || string.IsNullOrEmpty(sortColumnName.Replace("asc", "").Replace("null", "").Replace("desc", "").Trim()))
                sortColumnName = "ItemNumber desc";

            string ItemGuids = Request["ItemGuids"].ToString();
            BinMasterDAL binDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            int TotalRecordCount = 0;
            string _strNSItemLocation = string.Empty;

            if (Session["NSItemLocation"] != null && !string.IsNullOrEmpty(Convert.ToString(Session["NSItemLocation"])))
            {
                _strNSItemLocation = Convert.ToString(Session["NSItemLocation"]);
            }

            List<ItemsBins> lst = binDAL.GetItemsBinForLabelsUsingSort(ItemGuids, sortColumnName, strNSItemLocation: _strNSItemLocation);

            TotalRecordCount = lst.Count();
            var result = from u in lst
                         select new ItemsBins
                         {
                             BinID = u.BinID,
                             BinCriticalQuantity = u.BinCriticalQuantity,
                             BinGuid = u.BinGuid,
                             BinMaximumQuantity = u.BinMaximumQuantity,
                             BinMinimumQuantity = u.BinMinimumQuantity,
                             BinNumber = u.BinNumber,
                             BinQuantity = u.BinQuantity,
                             ItemGuid = u.ItemGuid,
                             ItemID = u.ItemID,
                             ItemNumber = u.ItemNumber,
                             OnHandQuantity = u.OnHandQuantity,
                         };
            JsonResult jsresult = Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                //aaData = DataFromDB
                aaData = result
            }, JsonRequestBehavior.AllowGet);
            jsresult.MaxJsonLength = int.MaxValue;
            return jsresult;
        }

        public ActionResult GetItemsSerialOrLotForLabels(string itemsguids)
        {
            ItemLocationDetailsDAL ildDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            string[] arrItemGuid = itemsguids.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<ItemLocationDetailsDTO> lst = ildDAL.GetRecordsByItemGuidsForLabels(itemsguids, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            return PartialView("_ItemsSerailLotForLabels", lst);
        }

        public ActionResult GetItemsBinSerialOrLotForLabels(string itemsguids, string binids)
        {
            ItemLocationDetailsDAL ildDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            string[] arrItemGuid = itemsguids.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<ItemLocationDetailsDTO> lst = ildDAL.GetRecordsByItemGuidsForLabels(itemsguids, SessionHelper.RoomID, SessionHelper.CompanyID, binids).ToList();
            return PartialView("_ItemsSerailLotForLabels", lst);
        }

        public JsonResult GetItemList(string NameStartWith)
        {
            ItemMasterDAL objCtrl = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            List<ItemMasterDTO> lstDTO;
            Int64 RoomID = SessionHelper.RoomID;
            Int64 CompanyID = SessionHelper.CompanyID;
            List<DTOForAutoComplete> returnKeyValList = new List<DTOForAutoComplete>();
            try
            {
                lstDTO = objCtrl.GetAllItemsPlain(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(i => i.ItemNumber).ToList();

                if (lstDTO != null && lstDTO.Count() > 0)
                {
                    foreach (var item in lstDTO)
                    {
                        if (!string.IsNullOrEmpty(item.ItemNumber))
                        {
                            DTOForAutoComplete obj = new DTOForAutoComplete()
                            {

                                Key = Convert.ToString(item.ItemNumber),
                                Value = item.ItemNumber,
                                ID = item.ID,
                                GUID = item.GUID,
                            };

                            returnKeyValList.Add(obj);
                        }
                        else
                        {
                            DTOForAutoComplete obj = new DTOForAutoComplete()
                            {

                                Key = Convert.ToString(item.ItemNumber),
                                Value = item.ItemNumber,
                                ID = item.ID,
                                GUID = item.GUID,
                            };

                            returnKeyValList.Add(obj);
                        }
                    }
                }


                if (returnKeyValList.Count > 0 && !string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                {
                    returnKeyValList = returnKeyValList.Where(x => x.Key.ToLower().StartsWith(NameStartWith.ToLower())).ToList();
                }

                return Json(returnKeyValList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(returnKeyValList, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                objCtrl = null;
                lstDTO = null;
            }
        }
        public JsonResult GetItemBinList(string NameStartWith, Guid ItemGuid)
        {
            BinMasterDAL objCtrl = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            List<BinMasterDTO> lstDTO;
            Int64 RoomID = SessionHelper.RoomID;
            Int64 CompanyID = SessionHelper.CompanyID;
            List<DTOForAutoComplete> returnKeyValList = new List<DTOForAutoComplete>();
            try
            {

                //lstDTO = objCtrl.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(l => l.ItemGUID == ItemGuid && l.IsStagingLocation == false).OrderBy(i => i.BinNumber).ToList();
                lstDTO = objCtrl.GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGuid, 0, null, false).OrderBy(i => i.BinNumber).ToList();
                if (lstDTO != null && lstDTO.Count() > 0)
                {
                    foreach (var item in lstDTO)
                    {
                        if (!string.IsNullOrEmpty(item.ItemNumber))
                        {
                            DTOForAutoComplete obj = new DTOForAutoComplete()
                            {

                                Key = Convert.ToString(item.BinNumber),
                                Value = item.BinNumber,
                                ID = item.ID,
                                GUID = item.GUID,
                            };

                            returnKeyValList.Add(obj);
                        }
                        else
                        {
                            DTOForAutoComplete obj = new DTOForAutoComplete()
                            {

                                Key = Convert.ToString(item.BinNumber),
                                Value = item.BinNumber,
                                ID = item.ID,
                                GUID = item.GUID,
                            };

                            returnKeyValList.Add(obj);
                        }
                    }
                }


                if (returnKeyValList.Count > 0 && !string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                {
                    returnKeyValList = returnKeyValList.Where(x => x.Key.ToLower().StartsWith(NameStartWith.ToLower())).ToList();
                }

                return Json(returnKeyValList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(returnKeyValList, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                objCtrl = null;
                lstDTO = null;
            }
        }
        public JsonResult GetItemDefaultBin(Guid ItemGuid)
        {
            BinMasterDAL objCtrl = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            BinMasterDTO lstDTO;
            Int64 RoomID = SessionHelper.RoomID;
            Int64 CompanyID = SessionHelper.CompanyID;
            List<DTOForAutoComplete> returnKeyValList = new List<DTOForAutoComplete>();
            try
            {
                //lstDTO = objCtrl.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(l => l.ItemGUID == ItemGuid && l.IsDefault == true).OrderBy(i => i.BinNumber).ToList();
                lstDTO = objCtrl.GetItemDefaultBin(ItemGuid, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (lstDTO != null)
                {
                    return Json(lstDTO.GUID, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ItemBinMasterListAjax(QuickListJQueryDataTableParamModel param)
        {
            Session["NSItemLocation"] = null;
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
            if (string.IsNullOrEmpty(sortColumnName) || sortColumnName == "undefine" || sortColumnName == "0" || sortColumnName.Trim().Contains("null") || string.IsNullOrEmpty(sortColumnName.Replace("asc", "").Replace("null", "").Replace("desc", "").Trim()))
                sortColumnName = "ID asc";

            string searchQuery = string.Empty;
            int TotalRecordCount = 0;
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);


            string sSearch = WebUtility.HtmlDecode(param.sSearch);

            //if (param.IsNarroSearchClear == false  && 
            //    (string.IsNullOrWhiteSpace(sSearch) 
            //    //|| SearchStrings.IsEmptySearchString(sSearch)
            //    )
            //    )
            //{
            //    using (UserNarrowSearchSettingsBAL searchSettingsBAL = new UserNarrowSearchSettingsBAL())
            //    {
            //        sSearch = searchSettingsBAL.GetSavedSearchStr(NarrowSearchSaveListEnum.ItemBinMaster);
            //    }
            //}

            if (sSearch != null && (!string.IsNullOrWhiteSpace(sSearch)))
            {
                string SearchText = sSearch; //WebUtility.HtmlDecode(param.sSearch);

                if (SearchText != null && !string.IsNullOrWhiteSpace(SearchText))
                {
                    if (SearchText.Contains("[###]"))
                    {
                        string[] stringSeparators = new string[] { "[###]" };
                        string[] Fields = SearchText.Split(stringSeparators, StringSplitOptions.None);
                        string[] FieldsPara = Fields[1].Split('@');
                        if (!string.IsNullOrWhiteSpace(FieldsPara[55]))
                        {
                            string ItemLocations = FieldsPara[55].TrimEnd(',');
                            Session["NSItemLocation"] = ItemLocations;
                        }
                    }
                }
            }
            var DataFromDB = obj.GetItemBinPagedRecord(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.UserSupplierIds, CurrentTimeZone, callFrom: "ItemMaster");

            var result = from u in DataFromDB
                         select new ItemMasterDTO
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
                             PerItemCost = u.PerItemCost,
                             LeadTimeInDays = u.LeadTimeInDays,
                             Link1 = u.Link1,
                             Link2 = u.Link2,
                             Trend = u.Trend,
                             IsAutoInventoryClassification = u.IsAutoInventoryClassification,
                             Taxable = u.Taxable,
                             Consignment = u.Consignment,
                             StagedQuantity = u.StagedQuantity,
                             InTransitquantity = u.InTransitquantity,
                             OnOrderQuantity = u.OnOrderQuantity,
                             OnReturnQuantity = u.OnReturnQuantity,
                             OnTransferQuantity = u.OnTransferQuantity,
                             // OnTransferInTransitQuantity=u.OnTransferInTransitQuantity,
                             //  OnTransferInTransitQuantity=u.OnTransferInTransitQuantity,
                             SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                             SuggestedTransferQuantity = u.SuggestedTransferQuantity,
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
                             //DefaultLocationName = u.DefaultLocationName,
                             BinID = u.BinID,
                             BinNumber = u.BinNumber,
                             BinGUID = u.BinGUID,
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
                             UDF6 = u.UDF6,
                             UDF7 = u.UDF7,
                             UDF8 = u.UDF8,
                             UDF9 = u.UDF9,
                             UDF10 = u.UDF10,
                             //for item grid display purpose - CART, PUll  
                             ItemUDF1 = u.UDF1,
                             ItemUDF2 = u.UDF2,
                             ItemUDF3 = u.UDF3,
                             ItemUDF4 = u.UDF4,
                             ItemUDF5 = u.UDF5,
                             ItemUDF6 = u.UDF6,
                             ItemUDF7 = u.UDF7,
                             ItemUDF8 = u.UDF8,
                             ItemUDF9 = u.UDF9,
                             ItemUDF10 = u.UDF10,
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
                             RefBomId = u.RefBomId,
                             CostUOMName = u.CostUOMName,
                             PullQtyScanOverride = u.PullQtyScanOverride,
                             TrendingSetting = u.TrendingSetting,
                             IsPackslipMandatoryAtReceive = u.IsPackslipMandatoryAtReceive,
                             // CreatedDate = CommonUtility.ConvertDateByTimeZone(u.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             // UpdatedDate = CommonUtility.ConvertDateByTimeZone(u.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             // CreatedDate = u.Created.Value.ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture),
                             // UpdatedDate = u.Updated.Value.ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture),

                             //ReceivedOnDate = CommonUtility.ConvertDateByTimeZone(u.ReceivedOn, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             AddedFrom = u.AddedFrom,
                             EditedFrom = u.EditedFrom,
                             ReceivedOnWeb = u.ReceivedOnWeb,
                             ReceivedOn = u.ReceivedOn,
                             BPONumber = u.BPONumber,
                             PriceSavedDateStr = u.PriceSavedDateStr,
                             PulledDate = u.PulledDate,
                             OrderedDate = u.OrderedDate,
                             CountedDate = u.CountedDate,
                             TrasnferedDate = u.TrasnferedDate,
                             ItemImageExternalURL = u.ItemImageExternalURL,
                             ItemLink2ExternalURL = u.ItemLink2ExternalURL,
                             ItemLink2ImageType = u.ItemLink2ImageType,
                             ItemDocExternalURL = u.ItemDocExternalURL,
                             QtyToMeetDemand = u.QtyToMeetDemand,
                             OutTransferQuantity = u.OutTransferQuantity,
                             OnOrderInTransitQuantity = u.OnOrderInTransitQuantity,
                             IsActive = u.IsActive,
                             ImageType = u.ImageType,
                             MonthlyAverageUsage = u.MonthlyAverageUsage,
                             IsAllowOrderCostuom = u.IsAllowOrderCostuom,
                             SuggestedReturnQuantity = u.SuggestedReturnQuantity,
                             //ItemImageExternalURL=u.ItemImageExternalURL
                             eVMISensorPort = u.eVMISensorPort,
                             eVMISensorID = u.eVMISensorID,
                             IsOrderable = u.IsOrderable,
                             BinUDF1 = u.BinUDF1,
                             BinUDF2 = u.BinUDF2,
                             BinUDF3 = u.BinUDF3,
                             BinUDF4 = u.BinUDF4,
                             BinUDF5 = u.BinUDF5,
                         };

            Session["BinReplanish"] = null;
            JsonResult jsresult = Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = result
            }, JsonRequestBehavior.AllowGet);
            jsresult.MaxJsonLength = int.MaxValue;
            return jsresult;
        }

        public JsonResult RemoveItemImage(string ItemGUID)
        {
            ItemMasterDAL objDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            bool IsUpdate = objDAL.RemoveItemImage(Guid.Parse(ItemGUID), "web", SessionHelper.UserID);
            if (IsUpdate)
            {
                QuickBookItemDAL objQBItemDAL = new QuickBookItemDAL(SessionHelper.EnterPriseDBName);
                ItemMasterDTO ImDto = objDAL.GetItemByGuidPlain(Guid.Parse(ItemGUID), SessionHelper.RoomID, SessionHelper.CompanyID);
                if (ImDto != null)
                {
                    objQBItemDAL.InsertQuickBookItem(Guid.Parse(ItemGUID), SessionHelper.EnterPriceID, CompanyID, RoomID, "Update", false, SessionHelper.UserID, "Web", Convert.ToInt64(ImDto.OnHandQuantity), "RemoveItemImage");
                }
                return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveItemLink(string ItemGUID)
        {
            ItemMasterDAL objDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            bool IsUpdate = objDAL.RemoveItemLink(Guid.Parse(ItemGUID), "web", SessionHelper.UserID);
            if (IsUpdate)
                return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveItemPollRequest(Int64 BinId, string ItemGuid)
        {
            using (HttpClient client = new HttpClient())
            {
                eVMIRequestDTO objeVMIRequestDTO = new eVMIRequestDTO();
                client.BaseAddress = new Uri(eTurnsAppConfig.eVMIWebAPIURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                objeVMIRequestDTO.EnterPriceID = SessionHelper.EnterPriceID;
                objeVMIRequestDTO.CompanyID = SessionHelper.CompanyID;
                objeVMIRequestDTO.RoomID = SessionHelper.RoomID;
                objeVMIRequestDTO.UserID = SessionHelper.UserID;
                objeVMIRequestDTO.isEVMI = SessionHelper.isEVMI;
                objeVMIRequestDTO.EnterPriseDBName = SessionHelper.EnterPriseDBName;
                objeVMIRequestDTO.BinId = BinId;
                objeVMIRequestDTO.ItemGuid = ItemGuid;
                objeVMIRequestDTO.SensorBinRoomSettings = SessionHelper.SensorBinRoomSettings;

                HttpResponseMessage response = client.PostAsJsonAsync("api/SmartShelves/SaveItemPollRequest", objeVMIRequestDTO).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {

                    return Json(new { Status = "ok", IsRefresh = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Status = "fail", IsRefresh = false }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        [HttpPost]
        public JsonResult SaveItemPollRequest1(Int64 BinId, string ItemGuid)
        {
            bool isRefresh = false;
            if (SessionHelper.isEVMI == true)
            {
                bool IsRequestImmediate = SessionHelper.SensorBinRoomSettings.IsPollRequestImmediate();
                ItemLocationPollRequestDAL objILPollRequestDAL = new ItemLocationPollRequestDAL(SessionHelper.EnterPriseDBName);
                ItemLocationPollRequestDTO objILPollRequestDTO = new ItemLocationPollRequestDTO();
                objILPollRequestDTO.ItemGUID = Guid.Parse(ItemGuid);
                objILPollRequestDTO.BinID = BinId;
                objILPollRequestDTO.RoomID = SessionHelper.RoomID;
                objILPollRequestDTO.CompanyID = SessionHelper.CompanyID;
                objILPollRequestDTO.RequestType = IsRequestImmediate ? (int)PollRequestType.PollImmediate : (int)PollRequestType.Poll;
                objILPollRequestDTO.IsPollStarted = false;
                objILPollRequestDTO.CreatedBy = SessionHelper.UserID;
                InventoryCountDTO objInventoryCountDTO = new InventoryCountDAL(SessionHelper.EnterPriseDBName).InsertPollCount(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "web");
                if (objInventoryCountDTO != null && objInventoryCountDTO.ID > 0)
                {
                    objILPollRequestDTO.CountGUID = objInventoryCountDTO.GUID;
                }

                objILPollRequestDAL.InsertItemLocationPollRequest(objILPollRequestDTO);
                isRefresh = IsRequestImmediate;
                if (IsRequestImmediate)
                {
                    eTurns.eVMIBAL.PollRequest pollRequest = new eTurns.eVMIBAL.PollRequest();
                    pollRequest.ProcessGetWightForItem(objILPollRequestDTO.ID, SessionHelper.EnterPriseDBName, SessionHelper.EnterPriceID
                        , SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);
                }

            }

            return Json(new { Status = "ok", IsRefresh = isRefresh }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult SaveItemPollRequestByRoomAndCompany(Int64 BinId, string ItemGuid)
        {
            bool isRefresh = false;
            if (SessionHelper.isEVMI == true)
            {
                ItemMasterDAL objItemAPI = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                var Objitem = objItemAPI.GetItemWithoutJoins(null, Guid.Parse(ItemGuid));
                Int64 RoomID = Objitem.Room ?? 0;
                Int64 CompanyID = Objitem.CompanyID ?? 0;

                bool IsRequestImmediate = SessionHelper.SensorBinRoomSettings.IsPollRequestImmediate();
                ItemLocationPollRequestDAL objILPollRequestDAL = new ItemLocationPollRequestDAL(SessionHelper.EnterPriseDBName);
                ItemLocationPollRequestDTO objILPollRequestDTO = new ItemLocationPollRequestDTO();
                objILPollRequestDTO.ItemGUID = Guid.Parse(ItemGuid);
                objILPollRequestDTO.BinID = BinId;
                objILPollRequestDTO.RoomID = RoomID;
                objILPollRequestDTO.CompanyID = CompanyID;
                objILPollRequestDTO.RequestType = IsRequestImmediate ? (int)PollRequestType.PollImmediate : (int)PollRequestType.Poll;
                objILPollRequestDTO.IsPollStarted = false;
                objILPollRequestDTO.CreatedBy = SessionHelper.UserID;
                InventoryCountDTO objInventoryCountDTO = new InventoryCountDAL(SessionHelper.EnterPriseDBName).InsertPollCount(RoomID, CompanyID, SessionHelper.UserID, "web");
                if (objInventoryCountDTO != null && objInventoryCountDTO.ID > 0)
                {
                    objILPollRequestDTO.CountGUID = objInventoryCountDTO.GUID;
                }

                objILPollRequestDAL.InsertItemLocationPollRequest(objILPollRequestDTO);
                isRefresh = IsRequestImmediate;
                if (IsRequestImmediate)
                {
                    eTurns.eVMIBAL.PollRequest pollRequest = new eTurns.eVMIBAL.PollRequest();
                    pollRequest.ProcessGetWightForItem(objILPollRequestDTO.ID, SessionHelper.EnterPriseDBName, SessionHelper.EnterPriceID
                        , SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);
                }

            }

            return Json(new { Status = "ok", IsRefresh = isRefresh }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult SaveItemTareRequest(Int64 BinId, string ItemGuid)
        {
            using (HttpClient client = new HttpClient())
            {
                eVMIRequestDTO objeVMIRequestDTO = new eVMIRequestDTO();
                client.BaseAddress = new Uri(eTurnsAppConfig.eVMIWebAPIURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                objeVMIRequestDTO.EnterPriceID = SessionHelper.EnterPriceID;
                objeVMIRequestDTO.CompanyID = SessionHelper.CompanyID;
                objeVMIRequestDTO.RoomID = SessionHelper.RoomID;
                objeVMIRequestDTO.UserID = SessionHelper.UserID;
                objeVMIRequestDTO.isEVMI = SessionHelper.isEVMI;
                objeVMIRequestDTO.EnterPriseDBName = SessionHelper.EnterPriseDBName;
                objeVMIRequestDTO.BinId = BinId;
                objeVMIRequestDTO.ItemGuid = ItemGuid;
                objeVMIRequestDTO.SensorBinRoomSettings = SessionHelper.SensorBinRoomSettings;

                HttpResponseMessage response = client.PostAsJsonAsync("api/SmartShelves/SaveItemTareRequest", objeVMIRequestDTO).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return Json(new { Status = "ok", IsRefresh = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Status = "fail", IsRefresh = false }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost]
        public JsonResult SaveItemTareRequest1(Int64 BinId, string ItemGuid)
        {
            if (SessionHelper.isEVMI == true)
            {
                bool IsTareRequestImmediate = SessionHelper.SensorBinRoomSettings.IsTareRequestImmediate();

                ItemLocationTareRequestDAL objILPollRequestDAL = new ItemLocationTareRequestDAL(SessionHelper.EnterPriseDBName);
                ItemLocationTareRequestDTO objILPollRequestDTO = new ItemLocationTareRequestDTO();
                objILPollRequestDTO.ItemGUID = Guid.Parse(ItemGuid);
                objILPollRequestDTO.BinID = BinId;
                objILPollRequestDTO.RoomID = SessionHelper.RoomID;
                objILPollRequestDTO.CompanyID = SessionHelper.CompanyID;
                objILPollRequestDTO.RequestType = IsTareRequestImmediate ? (int)TareRequestType.TareImmediate : (int)TareRequestType.Tare;
                objILPollRequestDTO.IsTareStarted = false;
                objILPollRequestDTO.CreatedBy = SessionHelper.UserID;

                objILPollRequestDAL.InsertItemLocationTareRequest(objILPollRequestDTO);

                if (IsTareRequestImmediate)
                {
                    // immediate process request

                    eTurns.eVMIBAL.TareRequest tareRequest = new eTurns.eVMIBAL.TareRequest();
                    tareRequest.TareProcessForItem(objILPollRequestDTO.ID, SessionHelper.EnterPriseDBName, SessionHelper.EnterPriceID
                        , SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);
                }

            }

            return Json(new { Status = "ok" }, JsonRequestBehavior.AllowGet);

        }

        //[HttpPost]
        //public JsonResult SaveItemTareDirectRequest(Int64 BinId, string ItemGuid)
        //{
        //    if (SessionHelper.isEVMI == true)
        //    {
        //        RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
        //        eTurnsRegionInfo objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(SessionHelper.RoomID, SessionHelper.CompanyID, 0);
        //        string CurrentRoomTimeZone = "UTC";
        //        if (objRegionalSettings != null)
        //        {
        //            CurrentRoomTimeZone = objRegionalSettings.TimeZoneName ?? "UTC";
        //        }
        //        DateTime CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);

        //        ItemLocationTareRequestDAL objTareRequestDAL = new ItemLocationTareRequestDAL(SessionHelper.EnterPriseDBName);
        //        ItemLocationTareRequestDTO objILTareRequestDTO = new ItemLocationTareRequestDTO();
        //        objILTareRequestDTO.ItemGUID = Guid.Parse(ItemGuid);
        //        objILTareRequestDTO.BinID = BinId;
        //        objILTareRequestDTO.RoomID = SessionHelper.RoomID;
        //        objILTareRequestDTO.CompanyID = SessionHelper.CompanyID;
        //        objILTareRequestDTO.RequestType = (int)TareRequestType.Tare;
        //        objILTareRequestDTO.IsTareStarted = true;
        //        objILTareRequestDTO.TareStartTime = CurrentTimeofTimeZone;
        //        objILTareRequestDTO.CreatedBy = SessionHelper.UserID;

        //        //objTareRequestDAL.InsertItemLocationTareRequest(objILTareRequestDTO);

        //        ILTareRequestDTO tareRequest = new ILTareRequestDTO();
        //        BinMasterDAL objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
        //        ItemMasterDAL objItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
        //        ComPortMasterDAL objComDAL = new ComPortMasterDAL(SessionHelper.EnterPriseDBName);
        //        BinMasterDTO objBinDTO = objBinDAL.GetBinByID(BinId, SessionHelper.RoomID, SessionHelper.CompanyID);
        //        ItemMasterDTO objItemDTO = objItemDAL.GetItemByGuidPlain(Guid.Parse(ItemGuid), SessionHelper.RoomID, SessionHelper.CompanyID);
        //        ComPortMasterDTO objComDTO = objComDAL.GetComPortMasterByRoom(SessionHelper.CompanyID, SessionHelper.RoomID);
        //        int _ScaleID = 0;
        //        int _ChannelID = 0;
        //        if(objBinDTO != null && objBinDTO.eVMISensorID != null && objBinDTO.eVMISensorID.ToString().Contains("."))
        //        {
        //            string[] ScaleChannel = objBinDTO.eVMISensorID.ToString().Split('.');
        //            if(ScaleChannel != null && ScaleChannel.Length > 0)
        //            {
        //                _ScaleID = Convert.ToInt32(ScaleChannel[0]);
        //                _ChannelID = Convert.ToInt32(ScaleChannel[1]);
        //            }
        //        }
        //        tareRequest.BinID = BinId;
        //        tareRequest.ScaleID = _ScaleID;
        //        tareRequest.ChannelID = _ChannelID;
        //        tareRequest.ItemGUID = Guid.Parse(ItemGuid); 
        //        tareRequest.CompanyID = SessionHelper.CompanyID; 
        //        tareRequest.RoomID = SessionHelper.RoomID;
        //        tareRequest.RoomName = SessionHelper.RoomName;
        //        tareRequest.CompanyName = SessionHelper.CompanyName;
        //        tareRequest.BinNumber = objBinDTO.BinNumber;
        //        tareRequest.ItemNumber = objItemDTO.ItemNumber;
        //        tareRequest.ComPortName = objComDTO.ComPortName;
        //        tareRequest.ComPortMasterID = objComDTO.ID;



        //        //double dbWeight = 0;
        //        bool isTareCompleted = true;
        //        string errorDescription = "";

        //        using (COMWrapper comWrapper = new COMWrapper(tareRequest.ComPortName))
        //        {
        //            try
        //            {
        //                var res = comWrapper.ZeroWeight(tareRequest.ScaleID, tareRequest.ChannelID);

        //                if (!res.IsSuccess)
        //                {
        //                    errorDescription = res.CommandData.ErrorInfo;
        //                    //tareEmailDTO.ComError = errorDescription;
        //                    isTareCompleted = false;

        //                    //WriteComErrorLog("Tare Process Error", EnterpriseID, tareRequest.CompanyID,
        //                    //   tareRequest.RoomID, tareRequest.ItemGUID, tareRequest.ComPortName
        //                    //   , errorDescription);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                isTareCompleted = false;
        //                //errorDescription = Logger.GetExceptionDetails(ex);
        //                //tareEmailDTO.ComError = ex.Message;

        //                //WriteComErrorLog("Tare Process Error", EnterpriseID, tareRequest.CompanyID,
        //                //      tareRequest.RoomID, tareRequest.ItemGUID, tareRequest.ComPortName
        //                //      , errorDescription, ex);

        //            }
        //        }
        //        objILTareRequestDTO.IsTareCompleted = true;

        //        if (!isTareCompleted)
        //        {
        //            objILTareRequestDTO.ErrorDescription = errorDescription;
        //        }
        //        else
        //        {
        //            objILTareRequestDTO.ErrorDescription = null;
        //        }

        //        // update Tare completed status
        //        CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);
        //        //tareRequest.TareCompletionTime = CurrentTimeofTimeZone;
        //        //tareRequest.UpdatedBy = SessionHelper.UserID;
        //        //objTareRequestDAL.UpdateItemLocationTareRequest(tareRequest);

        //        objILTareRequestDTO.TareCompletionTime = CurrentTimeofTimeZone;
        //        objILTareRequestDTO.UpdatedBy = SessionHelper.UserID;
        //        //objTareRequestDAL.InsertItemLocationTareRequest(tareRequest);

        //    }

        //    return Json(new { Status = "ok" }, JsonRequestBehavior.AllowGet);

        //}


        [HttpPost]
        public JsonResult SaveItemWeightPerPieceRequest(string ItemGuid, double? TotalQty)
        {
            string Msg = string.Empty;
            double itemWeightPerPiece = 0;
            if (SessionHelper.isEVMI == true)
            {
                bool IsRequestImmediate = SessionHelper.SensorBinRoomSettings.IsItemWeightPerPieceRequestImmediate();
                ItemWeightRequestDAL objWeightRequestDAL = new ItemWeightRequestDAL(SessionHelper.EnterPriseDBName);
                ItemWeightPerPieceRequestDTO objItemWeightReqDTO = new ItemWeightPerPieceRequestDTO();
                objItemWeightReqDTO.ItemGUID = Guid.Parse(ItemGuid);
                objItemWeightReqDTO.RoomID = SessionHelper.RoomID;
                objItemWeightReqDTO.CompanyID = SessionHelper.CompanyID;
                objItemWeightReqDTO.IsWeightStarted = false;
                objItemWeightReqDTO.TotalQty = TotalQty;
                objItemWeightReqDTO.CreatedBy = SessionHelper.UserID;
                objItemWeightReqDTO.RequestType = IsRequestImmediate ? (int)eVMIWeightPerPieceRequestType.WeightPerPieceImmediate : (int)eVMIWeightPerPieceRequestType.WeightPerPiece;

                var res = objWeightRequestDAL.InsertItemWeightPerPieceRequest(objItemWeightReqDTO);
                Int64 Flag = res.ReturnFlag;

                if (Flag == 1)
                {
                    Msg = "ok";
                }
                else if (Flag == 2)
                {
                    Msg = ResItemMaster.DefaultBinSensor;
                }

                if (IsRequestImmediate)
                {
                    eTurns.eVMIBAL.ItemWeightPerPieceRequest weightPerPieceRequest = new eTurns.eVMIBAL.ItemWeightPerPieceRequest();
                    itemWeightPerPiece = weightPerPieceRequest.GetItemWeightPerPieceProcessForItem(objItemWeightReqDTO.ID, SessionHelper.EnterPriseDBName
                        , SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);
                }
            }

            return Json(new { Status = Msg, itemWeightPerPiece = itemWeightPerPiece }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult SaveItemCalibrateRequest(Int64 BinId, string ItemGuid)
        {
            if (SessionHelper.isEVMI == true)
            {
                ItemLocationCalibrateRequestDAL objILPollRequestDAL = new ItemLocationCalibrateRequestDAL(SessionHelper.EnterPriseDBName);
                ItemLocationCalibrateRequestDTO objILPollRequestDTO = new ItemLocationCalibrateRequestDTO();
                objILPollRequestDTO.ItemGUID = Guid.Parse(ItemGuid);
                objILPollRequestDTO.BinID = BinId;
                objILPollRequestDTO.RoomID = SessionHelper.RoomID;
                objILPollRequestDTO.CompanyID = SessionHelper.CompanyID;
                objILPollRequestDTO.RequestType = (int)CalibrateRequestType.Calibrate;
                objILPollRequestDTO.IsStep1Started = false;
                objILPollRequestDTO.CreatedBy = SessionHelper.UserID;

                objILPollRequestDAL.InsertItemLocationCalibrateRequest(objILPollRequestDTO);
            }

            return Json(new { Status = "ok" }, JsonRequestBehavior.AllowGet);

        }

        #region
        public PartialViewResult ItemOnOrderQtyByRoomAndCompany(Guid ItemGuid, int RoomID, int CompanyID)
        {
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<ItemQuantityDetail> lstItemQtyDetail = objCommonDAL.GetOnOrderDetailFromCache(ItemGuid, RoomID, CompanyID);
            return PartialView("_ItemOnOrderQty", lstItemQtyDetail);
        }
        public PartialViewResult ItemOnQuoteQtyByRoomAndCompany(Guid ItemGuid, int RoomID, int CompanyID)
        {
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<ItemQuantityDetail> lstItemQtyDetail = objCommonDAL.GetOnQuoteDetailFromCache(ItemGuid, RoomID, CompanyID);
            return PartialView("_ItemOnQuoteQty", lstItemQtyDetail);
        }
        public PartialViewResult ItemOnOrderQtyWithBinByRoomAndCompany(Guid ItemGuid, int RoomID, int CompanyID, Int64 BinId)
        {
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<ItemQuantityDetail> lstItemQtyDetail = objCommonDAL.GetOnOrderDetailFromCache(ItemGuid, RoomID, CompanyID, BinId);
            return PartialView("_ItemOnOrderQty", lstItemQtyDetail);
        }
        public PartialViewResult ItemOnOrderInTransitQtyByRoomAndCompany(Guid ItemGuid, int RoomID, int CompanyID)
        {
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<ItemQuantityDetail> lstItemQtyDetail = objCommonDAL.GetOnOrderDetailInTransitFromCache(ItemGuid, RoomID, CompanyID);
            return PartialView("_ItemOnOrderQty", lstItemQtyDetail);
        }

        public PartialViewResult ItemOnOrderInTransitQtyWithBinByRoomAndCompany(Guid ItemGuid, int RoomID, int CompanyID, Int64 BinId)
        {
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<ItemQuantityDetail> lstItemQtyDetail = objCommonDAL.GetOnOrderDetailInTransitFromCache(ItemGuid, RoomID, CompanyID, BinId);
            return PartialView("_ItemOnOrderQty", lstItemQtyDetail);
        }

        public PartialViewResult ItemTurnsQtyByRoomAndCompany(Guid ItemGuid, int RoomID, int CompanyID)
        {
            DashboardDAL objCommonDAL = new DashboardDAL(SessionHelper.EnterPriseDBName);
            var measurementPullTransfer = ResDashboard.MeasurementPullTransfer;
            var measurementPullTransferValue = ResDashboard.MeasurementPullTransferValue;
            var measurementOrder = ResDashboard.MeasurementOrders;
            ItemTransationInfo lstItemQtyDetail = objCommonDAL.GetItemTxnHistory("turns", RoomID, CompanyID, 0, ItemGuid, measurementPullTransfer, measurementPullTransferValue, measurementOrder);
            return PartialView("_ItemTurnsQty", lstItemQtyDetail);
        }

        public PartialViewResult ItemAverageUsageQtyByRoomAndCompany(Guid ItemGuid, int RoomID, int CompanyID)
        {
            DashboardDAL objCommonDAL = new DashboardDAL(SessionHelper.EnterPriseDBName);
            var measurementPullTransfer = ResDashboard.MeasurementPullTransfer;
            var measurementPullTransferValue = ResDashboard.MeasurementPullTransferValue;
            var measurementOrder = ResDashboard.MeasurementOrders;
            ItemTransationInfo lstItemQtyDetail = objCommonDAL.GetItemTxnHistory("averageusage", RoomID, CompanyID, 0, ItemGuid, measurementPullTransfer, measurementPullTransferValue, measurementOrder);
            return PartialView("_ItemTurnsQty", lstItemQtyDetail);
        }

        public PartialViewResult ItemOnReturnQtyByRoomAndCompany(Guid ItemGuid, int RoomID, int CompanyID)
        {
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<ItemQuantityDetail> lstItemQtyDetail = objCommonDAL.GetOnReturnOrderDetailFromCache(ItemGuid, RoomID, CompanyID);
            return PartialView("_ItemOnOrderQty", lstItemQtyDetail);
        }

        public PartialViewResult ItemOnReturnQtyWithBinByRoomAndCompany(Guid ItemGuid, int RoomID, int CompanyID, Int64 BinId)
        {
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<ItemQuantityDetail> lstItemQtyDetail = objCommonDAL.GetOnReturnOrderDetailFromCache(ItemGuid, RoomID, CompanyID, BinId);
            return PartialView("_ItemOnOrderQty", lstItemQtyDetail);
        }

        public PartialViewResult ItemOnRequisationQtyByRoomAndCompany(Guid ItemGuid, int RoomID, int CompanyID)
        {
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<ItemQuantityDetail> lstItemQtyDetail = objCommonDAL.GetOnRequisitionDetailFromCache(ItemGuid, RoomID, CompanyID);
            return PartialView("_ItemOnRequesitionedQty", lstItemQtyDetail);
        }

        public PartialViewResult ItemOnRequisationQtyWithBinByRoomAndCompany(Guid ItemGuid, int RoomID, int CompanyID, Int64 BinId)
        {
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<ItemQuantityDetail> lstItemQtyDetail = objCommonDAL.GetOnRequisitionDetailFromCache(ItemGuid, RoomID, CompanyID, BinId);
            return PartialView("_ItemOnRequesitionedQty", lstItemQtyDetail);
        }

        public PartialViewResult ItemOnTransferQtyByRoomAndCompany(Guid ItemGuid, int RoomID, int CompanyID)
        {
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<ItemQuantityDetail> lstItemQtyDetail = objCommonDAL.GetOnTransferQtyDetailFromCache(ItemGuid, RoomID, CompanyID, SessionHelper.UserSupplierIds);
            return PartialView("_ItemOnTransferQty", lstItemQtyDetail);
        }
        public PartialViewResult ItemOnTransferQtyWithBinByRoomAndCompany(Guid ItemGuid, int RoomID, int CompanyID, Int64 BinId)
        {
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<ItemQuantityDetail> lstItemQtyDetail = objCommonDAL.GetOnTransferQtyDetailFromCacheWithBin(ItemGuid, RoomID, CompanyID, SessionHelper.UserSupplierIds, BinId);
            return PartialView("_ItemOnTransferQty", lstItemQtyDetail);
        }
        public PartialViewResult ItemOnTransferInTransitQtyByRoomAndCompany(Guid ItemGuid, int RoomID, int CompanyID)
        {
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<ItemQuantityDetail> lstItemQtyDetail = objCommonDAL.GetOnTransferQtyInTransitDetailFromCache(ItemGuid, RoomID, CompanyID, SessionHelper.UserSupplierIds);
            return PartialView("_ItemOnTransferQty", lstItemQtyDetail);
        }
        public PartialViewResult ItemOnTransferInTransitQtyWithBinByRoomAndCompany(Guid ItemGuid, int RoomID, int CompanyID, Int64 BinId)
        {
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<ItemQuantityDetail> lstItemQtyDetail = objCommonDAL.GetOnTransferQtyInTransitDetailFromCache(ItemGuid, RoomID, CompanyID, SessionHelper.UserSupplierIds, BinId);
            return PartialView("_ItemOnTransferQty", lstItemQtyDetail);
        }
        public PartialViewResult ItemInTransitQtyByRoomAndCompany(Guid ItemGuid, int RoomID, int CompanyID)
        {
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<ItemQuantityDetail> lstItemQtyDetail = objCommonDAL.GetInTransitQtyDetailFromCache(ItemGuid, RoomID, CompanyID, SessionHelper.UserSupplierIds);
            return PartialView("_ItemOnTransferQty", lstItemQtyDetail);
        }

        public PartialViewResult ItemStagedQuantityByRoomAndCompany(Guid ItemGuid, int RoomID, int CompanyID)
        {
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<ItemQuantityDetail> lstItemQtyDetail = objCommonDAL.GetOnStageQuantityDetailFromCache(ItemGuid, RoomID, CompanyID);
            return PartialView("_ItemStagedQuantity", lstItemQtyDetail);
        }

        #region "Solum Labels"

        /// <summary>
        /// Load LoadSolumLabels
        /// </summary>
        /// <returns></returns>    
        #endregion
        public ActionResult LoadSolumLabels(string SupplierPartNumber,string strExistingLabelsList, int? AddCount,bool? IsBOMItem)
        {
            SolumnLabelsDTO solumnLabelsDTO = new SolumnLabelsDTO();
            ItemMasterDAL objDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            LabelListDTO allLabelsList = new LabelListDTO();
            if (!string.IsNullOrEmpty(SupplierPartNumber))
            {
                allLabelsList = objDAL.GetAllSolumLables(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SupplierPartNumber);
                //string strNonExistingLabels = string.Empty;
                if (allLabelsList != null && allLabelsList.labelsList != null)
                {
                    foreach (var item in allLabelsList.labelsList)
                    {
                        if ((!strExistingLabelsList.Split(',').Contains(item.labelCode)) && AddCount < 1)
                        {
                            strExistingLabelsList += item.labelCode + ",";
                        }
                    }
                }
            }
            solumnLabelsDTO.EntireList = allLabelsList;
            solumnLabelsDTO.ExistingLabels = strExistingLabelsList;
            //solumnLabelsDTO.NonExistingLabels = strNonExistingLabels;
            solumnLabelsDTO.AddCount = AddCount;
            solumnLabelsDTO.ISBomItem = IsBOMItem;
            return PartialView("_SolumnLabelsList", solumnLabelsDTO);
        }

        [HttpPost]
        public JsonResult VerifyLabelCheckSUM(string LabelCode)
        {
            LabelVerification labelVerification = new LabelVerification();
            using (ItemMasterDAL itemMasterDAL= new ItemMasterDAL(SessionHelper.EnterPriseDBName))
            {
              labelVerification = itemMasterDAL.verifySolumLables(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID,LabelCode);
            }
            return Json(new { Status = "ok" , returnCode = labelVerification.returnCode , returnMsg = labelVerification.returnMsg }, JsonRequestBehavior.AllowGet);

        }

        #endregion
    }


    //public static class UniqueId
    //{
    //    static private int _InternalCounter = 0;
    //    static public string Get()
    //    {
    //        //var now = DateTime.Now;
    //        //var days = (int)(now - new DateTime(2000,1, 1)).TotalDays;
    //        //var seconds = (int)(now - DateTime.Today).TotalSeconds;
    //        //var counter = _InternalCounter++ % 100;
    //        //Random ran = new Random(DateTime.Now.Millisecond);
    //        //int key = 0;
    //        //key = ran.Next(0, 1000000000);
    //        ////return (days.ToString("0000") + seconds.ToString("0000") + counter.ToString("00"));
    //        //return key.ToString("0000000000");
    //        return String.Format("{0:0000000000}", (DateTime.Now.Ticks / 10) % 1000000000);
    //    }
    //}

    public class ItemManufacturerDetails
    {
        public string ID { get; set; }
        public string ManufacturerID { get; set; }
        public string ItemGUID { get; set; }
        public string ManufacturerName { get; set; }
        public string ManufacturerNumber { get; set; }
        public string IsDefault { get; set; }
    }

    public class ItemSupplierDetails
    {
        public string ID { get; set; }
        public string SupplierID { get; set; }
        public string ItemGUID { get; set; }
        public string SupplierName { get; set; }
        public string SupplierNumber { get; set; }
        public string IsDefault { get; set; }
    }

    public class ItemLocations
    {
        public string ID { get; set; }
        public string BinID { get; set; }
        public string BinNumber { get; set; }
        public string ItemGUID { get; set; }
        public float CriticalQuantity { get; set; }
        public float MinimumQuantity { get; set; }
        public float MaximumQuantity { get; set; }
    }
}
