using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsWeb.Helper;
using eTurnsWeb.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace eTurnsWeb.Controllers
{
    [AuthorizeHelper]
    public class ToolKitController : eTurnsControllerBase
    {
        UDFController objUDFDAL = new UDFController();
        bool isInsert = SessionHelper.GetModulePermission(SessionHelper.ModuleList.Kits, SessionHelper.PermissionType.Insert);
        bool isUpdate = SessionHelper.GetModulePermission(SessionHelper.ModuleList.Kits, SessionHelper.PermissionType.Update);
        Int64 RoomID = SessionHelper.RoomID;
        Int64 CompanyID = SessionHelper.CompanyID;
        Int64 UserID = SessionHelper.UserID;

        /****************************************** Tool Kit methods start *******************************/
        public ActionResult KitToolList()
        {
            Session["ToolBinReplanish"] = null;
            Session["ToolORDType"] = null;
            return View();
        }

        public JsonResult KitToolMasterListAjax(JQueryDataTableParamModel param)
        {
            Session["ToolKitDetail"] = null;
            ToolMasterDAL obj = new eTurns.DAL.ToolMasterDAL(SessionHelper.EnterPriseDBName);
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

            if (sortColumnName.Trim() == "ToolUDF1")
                sortColumnName = "UDF1";
            else if (sortColumnName.Trim() == "ToolUDF2")
                sortColumnName = "UDF2";
            else if (sortColumnName.Trim() == "ToolUDF3")
                sortColumnName = "UDF3";
            else if (sortColumnName.Trim() == "ToolUDF4")
                sortColumnName = "UDF4";
            else if (sortColumnName.Trim() == "ToolUDF5")
                sortColumnName = "UDF5";

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
            {
                sortColumnName = "ID desc";

            }
            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            string ToolType = Request["ToolType"].ToString();
            Session["ToolType"] = ToolType;
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetPagedToolsNew(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, null, null, RoomDateFormat, CurrentTimeZone, Type: ToolType);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreateToolKit()
        {
            string NewNumber = new AutoSequenceDAL(SessionHelper.EnterPriseDBName).GetNextAutoNumberByModule("NextToolNo", SessionHelper.RoomID, SessionHelper.CompanyID);
            Session["ToolKitDetail"] = null;
            Session["ToolBinReplanish"] = null;
            ToolMasterDTO objDTO = new ToolMasterDTO();
            objDTO.ID = 0;
            objDTO.ToolName = NewNumber;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
            objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.RoomName = SessionHelper.RoomName;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.GUID = Guid.NewGuid();
            objDTO.IsOnlyFromItemUI = true;
            objDTO.Type = 2;
            objDTO.KitToolQuantity = 0;
            objDTO.KitToolName = objDTO.ToolName;
            objDTO.KitToolSerial = objDTO.Serial;
            LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(SessionHelper.EnterPriseDBName);
            List<LocationMasterDTO> lstLocation = objLocationCntrl.GetLocationByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID);
            lstLocation = lstLocation.Where(t => (!string.IsNullOrWhiteSpace(t.Location))).ToList();
            lstLocation.Insert(0, new LocationMasterDTO() { ID = 0, Location = ResCommon.MsgSelectLocation });
            ViewBag.LocationList = lstLocation;
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("ToolMaster");

            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }

            ViewBag.GropOfItemsBag = GetGroupOfItems();
            objDTO.ImageType = "ExternalImage";

            return PartialView("_KitToolCreate", objDTO);
        }
        public ActionResult ToolEdit(Int64 ID)
        {
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            bool IsHistory = false;

            if (Request["IsHistory"] != null && Request["IsHistory"].ToString() != "")
                IsHistory = bool.Parse(Request["IsHistory"].ToString());
            bool IsChangeLog = false;

            if (Request["IsChangeLog"] != null && Request["IsChangeLog"].ToString() != "")
                IsChangeLog = bool.Parse(Request["IsChangeLog"].ToString());

            if (IsDeleted || IsArchived || IsHistory || IsChangeLog)
            {
                ViewBag.ViewOnly = true;
            }

            ToolMasterDAL obj = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            ToolMasterDTO objDTO = new ToolMasterDTO();

            if (!IsChangeLog)
                objDTO = obj.GetToolByIDNormal(ID);
            else
                objDTO = obj.GetHistoryRecordNew(ID);

            objDTO.KitToolQuantity = objDTO.Quantity;
            objDTO.KitToolName = objDTO.ToolName;
            objDTO.KitToolSerial = objDTO.Serial;
            ToolDetailDAL objToolDetailDAL = new ToolDetailDAL(SessionHelper.EnterPriseDBName);
            Session["ToolKitDetail"] = objToolDetailDAL.GetAllRecordsByKitGUIDNew(objDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, true).ToList();
            objDTO.IsOnlyFromItemUI = true;
            LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(SessionHelper.EnterPriseDBName);
            List<LocationMasterDTO> lstLocation = objLocationCntrl.GetLocationByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID);
            lstLocation = lstLocation.Where(t => (!string.IsNullOrWhiteSpace(t.Location))).ToList();
            lstLocation.Insert(0, new LocationMasterDTO() { ID = 0, Location = ResCommon.MsgSelectLocation });
            ViewBag.LocationList = lstLocation;
            ToolLocationDetailsDAL objToolLocationDetailDAL = new ToolLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            List<ToolLocationDetailsDTO> lstToolLocationDetailsDTO = objToolLocationDetailDAL.GetToolLocationsByToolGUID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, objDTO.GUID).ToList();
            List<LocationMasterDTO> lstAllLocation = new List<LocationMasterDTO>(lstLocation.ToList());
            lstToolLocationDetailsDTO = lstToolLocationDetailsDTO.Where(t => (!string.IsNullOrWhiteSpace(t.ToolLocationName))).ToList();
            List<Guid> objGUIDList = lstToolLocationDetailsDTO.Select(u => u.LocationGUID).ToList();
            lstAllLocation = lstAllLocation.Where(l => objGUIDList.Contains(l.GUID)).ToList();

            if ((objDTO.LocationID ?? 0) > 0)
            {
                LocationMasterDTO objLocationMasterDTO = lstAllLocation.Where(t => t.ID == objDTO.LocationID).FirstOrDefault();
                if (objLocationMasterDTO != null)
                {
                    objLocationMasterDTO.IsDefault = true;
                }
            }

            Session["ToolBinReplanish"] = lstAllLocation;
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("ToolMaster");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;
            ViewBag.GropOfItemsBag = GetGroupOfItems();

            return PartialView("_KitToolCreate", objDTO);
        }

        public ActionResult LoadToolKitComponentofItem(Guid ToolGUID, int? AddCount)
        {
            ViewBag.ToolGUID = ToolGUID;
            List<ToolDetailDTO> lstToolDetailDTO = null;
            if (Session["ToolKitDetail"] != null)
            {
                lstToolDetailDTO = (List<ToolDetailDTO>)Session["ToolKitDetail"];
                //Delete blank rows
                lstToolDetailDTO.Remove(lstToolDetailDTO.Where(t => t.GUID == Guid.Empty && t.ToolGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty).FirstOrDefault());
            }
            else
            {
                lstToolDetailDTO = new List<ToolDetailDTO>();
            }

            if (AddCount != null && AddCount > 0)
            {
                for (int i = 0; i < AddCount; i++)
                {
                    lstToolDetailDTO.Add(new ToolDetailDTO() { ID = 0, SessionSr = lstToolDetailDTO.Count + 1, ToolGUID = ToolGUID, Room = SessionHelper.RoomID, CompanyID = SessionHelper.CompanyID, Updated = DateTimeUtility.DateTimeNow, LastUpdatedBy = SessionHelper.UserID, Created = DateTimeUtility.DateTimeNow, CreatedBy = SessionHelper.UserID });
                }
            }
            return PartialView("_ToolKitComponent", lstToolDetailDTO.OrderBy(t => t.Serial).ToList());
        }
        public ActionResult LoadToolKitModel(string Parentid, string ParentGuid, string ToolType)
        {
            Session["ToolType"] = ToolType;
            ItemModelPerameter obj = new ItemModelPerameter()
            {
                AjaxURLAddItemToSession = "~/ToolKit/AddToolToDetailTableKit/",
                PerentID = Parentid,
                PerentGUID = ParentGuid,
                ModelHeader = ResKitMaster.AddToolKitComponentToToolKitFromKit,
                AjaxURLAddMultipleItemToSession = "~/ToolKit/AddToolToDetailTableKit/",
                AjaxURLToFillItemGrid = "~/ToolKit/GetToolsModelMethodKit/",
                CallingFromPageName = "KITTool",
            };

            return PartialView("ToolMasterModel", obj);
        }
        public JsonResult AddToolToDetailTableKit(string para)
        {
            string message = "";
            string status = "";
            JavaScriptSerializer s = new JavaScriptSerializer();
            ToolDetailDTO[] ToolDetails = s.Deserialize<ToolDetailDTO[]>(para);
            int sessionsr = 0;
            ToolMasterDAL objToolDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);

            foreach (ToolDetailDTO item in ToolDetails)
            {
                item.Room = SessionHelper.RoomID;
                item.RoomName = SessionHelper.RoomName;
                item.CreatedBy = SessionHelper.UserID;
                item.CreatedByName = SessionHelper.UserName;
                item.UpdatedByName = SessionHelper.UserName;
                item.LastUpdatedBy = SessionHelper.UserID;
                item.CompanyID = SessionHelper.CompanyID;
                item.Updated = DateTimeUtility.DateTimeNow;

                if (!(item.GUID != null && item.GUID != Guid.Empty))
                {
                    item.Created = DateTimeUtility.DateTimeNow;

                    ToolMasterDTO ObjToolDTO = objToolDAL.GetToolByGUIDPlain(item.ToolItemGUID.GetValueOrDefault(Guid.Empty));
                    if (ObjToolDTO != null && ObjToolDTO.Type != 2)
                    {
                        item.ToolItemGUID = ObjToolDTO.GUID;
                        List<ToolDetailDTO> lstToolDetail = null;

                        if (Session["ToolKitDetail"] != null)
                        {
                            lstToolDetail = (List<ToolDetailDTO>)Session["ToolKitDetail"];
                            item.SessionSr = lstToolDetail.Count + 1;
                        }
                        else
                        {
                            item.SessionSr = sessionsr + 1;
                            lstToolDetail = new List<ToolDetailDTO>();
                        }

                        lstToolDetail.Add(item);
                        Session["ToolKitDetail"] = lstToolDetail;
                    }
                }
            }

            message = ResKitMaster.ToolQtyUpdatedSuccessfully;
            status = "ok";
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetToolsModelMethodKit(JQueryDataTableParamModel param)
        {
            ToolMasterDAL obj = new eTurns.DAL.ToolMasterDAL(SessionHelper.EnterPriseDBName);
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

            if (sortColumnName == "0" || sortColumnName.Contains("undefined"))
                sortColumnName = "Serial Asc";

            string searchQuery = string.Empty;
            int TotalRecordCount = 0;
            List<ToolDetailDTO> objQLItems = null;

            if (Session["ToolKitDetail"] != null)
            {
                objQLItems = (List<ToolDetailDTO>)Session["ToolKitDetail"];
            }

            string ToolIDs = "";

            if (objQLItems != null && objQLItems.Count > 0)
            {
                foreach (var item in objQLItems)
                {
                    if (!string.IsNullOrEmpty(ToolIDs))
                        ToolIDs += ",";

                    ToolIDs += item.ToolItemGUID.ToString();
                }
            }

            string ToolType = Request["ToolType"].ToString();
            Session["ToolType"] = ToolType;
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetPagedToolsNew(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, "", "", RoomDateFormat, CurrentTimeZone, ToolIDs, Type: ToolType, CalledPage: "FromToolKitPage");

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
        public JsonResult SaveToolKitQty(Guid ToolItemGuid, double? QuantityPerKit)
        {
            List<ToolDetailDTO> lstToolDetailDTO = (List<ToolDetailDTO>)Session["ToolKitDetail"];

            if (lstToolDetailDTO != null && lstToolDetailDTO.Count > 0)
            {
                ToolDetailDTO objToolDetailDTO = lstToolDetailDTO.Where(t => t.ToolItemGUID == ToolItemGuid).FirstOrDefault();
                if (objToolDetailDTO != null)
                {
                    objToolDetailDTO.QuantityPerKit = Convert.ToInt64(QuantityPerKit);
                }
                lstToolDetailDTO = lstToolDetailDTO.Where(t => t.ToolItemGUID != ToolItemGuid).ToList();
                lstToolDetailDTO.Add(objToolDetailDTO);
                Session["ToolKitDetail"] = lstToolDetailDTO;
            }
            return Json(true);

        }

        [ValidateAntiForgeryToken]
        public JsonResult ToolKitSave(ToolMasterDTO objDTO)
        {
            Int64 TempToolID = 0;
            string message = "";
            string status = "";
            ToolMasterDAL obj = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            LocationMasterDAL objLocationMasterDAL = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
            ToolLocationDetailsDAL objToolLocationDetailsDAL = new ToolLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            objDTO.Quantity = objDTO.KitToolQuantity ?? 0;
            objDTO.ToolName = objDTO.KitToolName;
            objDTO.Serial = objDTO.KitToolSerial;
            ToolLocationDetailsDTO objToolLocationDetailsDTO = null;

            if ((List<ToolDetailDTO>)Session["ToolKitDetail"] == null || ((List<ToolDetailDTO>)Session["ToolKitDetail"]).Count == 0)
            {
                message = ResKitMaster.AddKitComponentRequired;
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(objDTO.ToolName))
            {
                message = string.Format(ResMessage.Required, ResKitToolMaster.ToolName);// "Tool is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            if (objDTO.ID == 0)
            {
                if (!string.IsNullOrWhiteSpace(objDTO.ToolTypeTracking) && objDTO.ToolTypeTracking.Contains("2"))
                {
                    objDTO.SerialNumberTracking = true;
                }

            }
            if (objDTO.SerialNumberTracking == false && string.IsNullOrEmpty(objDTO.Serial))
            {
                message = string.Format(ResMessage.Required, ResKitToolMaster.Serial);// "Tool is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }

            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.CompanyID = SessionHelper.CompanyID;

            CommonDAL objCommon = new CommonDAL(SessionHelper.EnterPriseDBName);
            bool AllowToolOrdering = SessionHelper.AllowToolOrdering;
            if (objDTO.ID == 0)
            {

                objDTO.CreatedBy = SessionHelper.UserID;
                if (!string.IsNullOrWhiteSpace(objDTO.ToolTypeTracking) && objDTO.ToolTypeTracking.Contains("2"))
                {
                    objDTO.SerialNumberTracking = true;
                }

                string strOK = "ok";

                if (AllowToolOrdering)
                {
                    strOK = obj.ToolNameSerialDuplicateCheck((objDTO.ToolName ?? string.Empty).Trim() + "$" + (objDTO.Serial ?? string.Empty).Trim(), objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID);
                }
                else
                {
                    strOK = obj.ToolSerialDuplicateCheck((objDTO.Serial ?? string.Empty).Trim(), objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID);
                }
                if (strOK == "duplicate")
                {
                    if (AllowToolOrdering)
                    {
                        message = string.Format(ResMessage.DuplicateMessage, ResToolMaster.ToolName, (objDTO.ToolName ?? string.Empty).Trim());  //"Tool \"" + objDTO.ToolName + "\" already exist! Try with Another!";
                        status = "duplicate";
                    }
                    else
                    {

                        message = string.Format(ResMessage.DuplicateMessage, ResToolMaster.Serial, (objDTO.Serial ?? string.Empty).Trim());  //"Tool \"" + objDTO.ToolName + "\" already exist! Try with Another!";
                        status = "duplicate";
                    }
                }
                else
                {
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.CreatedByName = SessionHelper.UserName;
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    if (objDTO.LocationID == 0)
                    {
                        if (Session["ToolBinReplanish"] != null)
                        {
                            List<LocationMasterDTO> lstAllLocation = (List<LocationMasterDTO>)Session["ToolBinReplanish"];

                            if (lstAllLocation != null && lstAllLocation.Count() > 0 && lstAllLocation.Any())
                            {
                                for (int i = 0; i < lstAllLocation.Count(); i++)
                                {
                                    if ((lstAllLocation[i].IsDefault ?? false) == true)
                                    {
                                        LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(SessionHelper.EnterPriseDBName);
                                        LocationMasterDTO LocationDTO = new LocationMasterDTO();
                                        LocationDTO = objLocationCntrl.GetLocationOrInsert(lstAllLocation[i].Location, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "AssetController>>ToolSave");
                                        objDTO.LocationID = LocationDTO.ID;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    TempToolID = obj.Insert(objDTO, AllowToolOrdering);
                    if (TempToolID > 0)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                        List<ToolDetailDTO> lstKitDetailDTO = null;
                        if (Session["ToolKitDetail"] != null)
                        {
                            lstKitDetailDTO = ((List<ToolDetailDTO>)Session["ToolKitDetail"]).Where(t => t.ToolItemGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty).ToList();
                            foreach (var itembr in lstKitDetailDTO)
                            {
                                itembr.ToolGUID = objDTO.GUID;
                                //  itembr.ItemGUID =  objDTO.GUID;
                                itembr.WhatWhereAction = "KitController---> ToolKitSave";
                                ToolDetailDAL objToolDetailDAL = new ToolDetailDAL(SessionHelper.EnterPriseDBName);
                                if (itembr.ID > 0)
                                {
                                    itembr.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    itembr.EditedFrom = "Web";

                                    objToolDetailDAL.Edit(itembr);
                                }
                                else
                                {

                                    objToolDetailDAL.Insert(itembr);
                                }
                            }
                            ToolMasterDTO objToolMasterDTO = obj.GetToolByIDPlain(TempToolID);

                            LocationMasterDTO objLocationMasterDTO = null;
                            if (AllowToolOrdering)
                            {

                                if (Session["ToolBinReplanish"] != null)
                                {
                                    List<LocationMasterDTO> lstAllLocation = (List<LocationMasterDTO>)Session["ToolBinReplanish"];

                                    if (lstAllLocation != null && lstAllLocation.Count() > 0 && lstAllLocation.Any())
                                    {
                                        for (int i = 0; i < lstAllLocation.Count(); i++)
                                        {
                                            if ((lstAllLocation[i].IsDefault ?? false) == true)
                                            {
                                                LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(SessionHelper.EnterPriseDBName);
                                                objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(objDTO.GUID, lstAllLocation[i].Location, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "AssetController>>ToolSave", true);
                                                objLocationMasterDTO = objLocationCntrl.GetLocationByNamePlain(lstAllLocation[i].Location, SessionHelper.RoomID, SessionHelper.CompanyID);

                                                break;
                                            }
                                            else
                                            {
                                                LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(SessionHelper.EnterPriseDBName);
                                                objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(objDTO.GUID, lstAllLocation[i].Location, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "AssetController>>ToolSave");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(SessionHelper.EnterPriseDBName);
                                        objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(objDTO.GUID, string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "ToolController>>ToolSave", true);
                                        objLocationMasterDTO = objLocationCntrl.GetLocationByNamePlain(string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID);
                                    }
                                }
                                else
                                {
                                    LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(SessionHelper.EnterPriseDBName);
                                    objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(objDTO.GUID, string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "ToolController>>ToolSave", true);
                                    objLocationMasterDTO = objLocationCntrl.GetLocationByNamePlain(string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID);
                                }

                                if (objLocationMasterDTO == null)
                                {
                                    LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(SessionHelper.EnterPriseDBName);
                                    objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(objDTO.GUID, string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "ToolController>>ToolSave", true);
                                    objLocationMasterDTO = objLocationCntrl.GetLocationByNamePlain(string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID);
                                }
                                if (objDTO.Quantity > 0 && (!objDTO.SerialNumberTracking))
                                {
                                    ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO = new ToolAssetQuantityDetailDTO();
                                    objToolAssetQuantityDetailDTO.ToolGUID = objToolMasterDTO.GUID;
                                    objToolAssetQuantityDetailDTO.AssetGUID = null;
                                    objToolAssetQuantityDetailDTO.ToolBinID = objToolLocationDetailsDTO != null ? objToolLocationDetailsDTO.ID : ((objLocationMasterDTO != null) ? objLocationMasterDTO.ID : 0);
                                    objToolAssetQuantityDetailDTO.Quantity = objDTO.Quantity;
                                    objToolAssetQuantityDetailDTO.RoomID = SessionHelper.RoomID;
                                    objToolAssetQuantityDetailDTO.CompanyID = SessionHelper.CompanyID;
                                    objToolAssetQuantityDetailDTO.Created = objDTO.Created;
                                    objToolAssetQuantityDetailDTO.Updated = objDTO.Updated ?? DateTimeUtility.DateTimeNow;
                                    objToolAssetQuantityDetailDTO.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                                    objToolAssetQuantityDetailDTO.ReceivedOn = objDTO.ReceivedOn;
                                    objToolAssetQuantityDetailDTO.AddedFrom = "Web";
                                    objToolAssetQuantityDetailDTO.EditedFrom = "Web";
                                    objToolAssetQuantityDetailDTO.WhatWhereAction = "ToolKitController>>ToolKitSave";
                                    objToolAssetQuantityDetailDTO.ReceivedDate = null;
                                    objToolAssetQuantityDetailDTO.InitialQuantityWeb = objDTO.Quantity;
                                    objToolAssetQuantityDetailDTO.InitialQuantityPDA = 0;
                                    objToolAssetQuantityDetailDTO.ExpirationDate = null;
                                    objToolAssetQuantityDetailDTO.EditedOnAction = "Tool Kit Created From Web Page. insert Entry of Tool Kit.";
                                    objToolAssetQuantityDetailDTO.CreatedBy = SessionHelper.UserID;
                                    objToolAssetQuantityDetailDTO.UpdatedBy = SessionHelper.UserID;
                                    objToolAssetQuantityDetailDTO.IsDeleted = false;
                                    objToolAssetQuantityDetailDTO.IsArchived = false;
                                    ToolAssetQuantityDetailDAL objToolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
                                    objToolAssetQuantityDetailDAL.Insert(objToolAssetQuantityDetailDTO, false, "AdjCredit", ReferalAction: "Initial Tool Create");
                                }

                                if (Session["ToolBinReplanish"] != null)
                                {
                                    List<LocationMasterDTO> lstAllLocation = (List<LocationMasterDTO>)Session["ToolBinReplanish"];

                                    if (lstAllLocation != null && lstAllLocation.Count() > 0 && lstAllLocation.Any())
                                    {
                                        for (int i = 0; i < lstAllLocation.Count(); i++)
                                        {
                                            objLocationMasterDAL.GetToolLocation(objToolMasterDTO.GUID, lstAllLocation[i].Location, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "ToolKitController>>ToolKitSave", lstAllLocation[i].IsDefault);
                                        }
                                    }
                                }
                            }

                            Session["ToolKitDetail"] = null;
                        }
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                        status = "fail";
                    }

                }
            }
            else
            {
                objDTO.LastUpdatedBy = SessionHelper.UserID;
                string strOK = "";
                if (AllowToolOrdering)
                {
                    strOK = obj.ToolNameSerialDuplicateCheck((objDTO.ToolName ?? string.Empty).Trim() + "$" + (objDTO.Serial ?? string.Empty).Trim(), objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID);
                }
                else
                {
                    strOK = objCommon.DuplicateCheck(objDTO.Serial ?? string.Empty, "edit", objDTO.ID, "ToolMaster", "Serial", SessionHelper.RoomID, SessionHelper.CompanyID);
                }

                if (strOK == "duplicate")
                {
                    if (AllowToolOrdering)
                    {
                        message = string.Format(ResMessage.DuplicateMessage, ResToolMaster.ToolName, (objDTO.ToolName ?? string.Empty).Trim());  //"Tool \"" + objDTO.ToolName + "\" already exist! Try with Another!";
                        status = "duplicate";
                    }
                    else
                    {

                        message = string.Format(ResMessage.DuplicateMessage, ResToolMaster.Serial, (objDTO.Serial ?? string.Empty).Trim());  //"Tool \"" + objDTO.ToolName + "\" already exist! Try with Another!";
                        status = "duplicate";
                    }
                }
                else
                {
                    if (objDTO.IsOnlyFromItemUI)
                    {
                        objDTO.EditedFrom = "Web";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    }
                    ToolMasterDTO tempobjDTO = new ToolMasterDTO();

                    tempobjDTO = obj.GetToolByIDPlain(objDTO.ID);
                    if (tempobjDTO != null)
                    {
                        objDTO.AddedFrom = tempobjDTO.AddedFrom;
                        objDTO.ReceivedOnWeb = tempobjDTO.ReceivedOnWeb;
                    }
                    if (!string.IsNullOrWhiteSpace(objDTO.ToolTypeTracking) && objDTO.ToolTypeTracking.Contains("2"))
                    {
                        objDTO.SerialNumberTracking = true;
                    }
                    if (objDTO.LocationID == 0)
                    {
                        if (Session["ToolBinReplanish"] != null)
                        {
                            List<LocationMasterDTO> lstAllLocation = (List<LocationMasterDTO>)Session["ToolBinReplanish"];

                            if (lstAllLocation != null && lstAllLocation.Count() > 0 && lstAllLocation.Any())
                            {
                                for (int i = 0; i < lstAllLocation.Count(); i++)
                                {
                                    if ((lstAllLocation[i].IsDefault ?? false) == true)
                                    {
                                        LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(SessionHelper.EnterPriseDBName);
                                        LocationMasterDTO LocationDTO = new LocationMasterDTO();
                                        LocationDTO = objLocationCntrl.GetLocationOrInsert(lstAllLocation[i].Location, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "AssetController>>ToolSave");
                                        objDTO.LocationID = LocationDTO.ID;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (obj.Edit(objDTO, AllowToolOrdering))
                    {
                        TempToolID = objDTO.ID;
                        message = ResMessage.SaveMessage;
                        status = "ok";
                        List<ToolDetailDTO> lstToolDetailDTO = null;

                        if (Session["ToolKitDetail"] != null)
                        {
                            lstToolDetailDTO = ((List<ToolDetailDTO>)Session["ToolKitDetail"]).Where(t => t.ToolGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty).ToList();

                            if (lstToolDetailDTO.Count > 0)
                            {
                                string KitIDs = "";
                                ToolDetailDAL objKitDetailDAL = new ToolDetailDAL(SessionHelper.EnterPriseDBName);

                                foreach (var itembr in lstToolDetailDTO)
                                {
                                    itembr.WhatWhereAction = "KitController---> ToolKitSave";

                                    if (itembr.ID > 0)
                                    {
                                        itembr.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        itembr.EditedFrom = "Web";

                                        objKitDetailDAL.Edit(itembr);
                                    }
                                    else
                                    {

                                        objKitDetailDAL.Insert(itembr);
                                    }
                                    KitIDs += itembr.ID + ",";
                                }
                                //Delete except session record....

                                objKitDetailDAL.DeleteRecordsExcept(KitIDs, objDTO.GUID, SessionHelper.UserID, SessionHelper.CompanyID);
                            }
                            Session["ToolKitDetail"] = null;
                            ToolMasterDTO objToolMasterDTO = obj.GetToolBySerialPlain(objDTO.Serial, SessionHelper.RoomID, SessionHelper.CompanyID);
                            LocationMasterDTO objLocationMasterDTO = null;

                            if (AllowToolOrdering)
                            {
                                if (Session["ToolBinReplanish"] != null)
                                {
                                    List<LocationMasterDTO> lstAllLocation = (List<LocationMasterDTO>)Session["ToolBinReplanish"];

                                    if (lstAllLocation != null && lstAllLocation.Count() > 0 && lstAllLocation.Any())
                                    {
                                        for (int i = 0; i < lstAllLocation.Count(); i++)
                                        {
                                            if ((lstAllLocation[i].IsDefault ?? false) == true)
                                            {
                                                LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(SessionHelper.EnterPriseDBName);
                                                objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(objDTO.GUID, lstAllLocation[i].Location, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "AssetController>>ToolSave");
                                                objLocationMasterDTO = objLocationCntrl.GetLocationByNamePlain(lstAllLocation[i].Location, SessionHelper.RoomID, SessionHelper.CompanyID);
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(SessionHelper.EnterPriseDBName);
                                        objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(objDTO.GUID, string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "ToolController>>ToolSave");
                                        objLocationMasterDTO = objLocationCntrl.GetLocationByNamePlain(string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID);
                                    }
                                }
                                else
                                {
                                    LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(SessionHelper.EnterPriseDBName);
                                    objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(objDTO.GUID, string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "ToolController>>ToolSave");
                                    objLocationMasterDTO = objLocationCntrl.GetLocationByNamePlain(string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID);
                                }
                                if (objLocationMasterDTO == null)
                                {
                                    LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(SessionHelper.EnterPriseDBName);
                                    objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(objDTO.GUID, string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "ToolController>>ToolSave");
                                    objLocationMasterDTO = objLocationCntrl.GetLocationByNamePlain(string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID);
                                    objLocationMasterDAL.UpdateToolWithDefault(objDTO.GUID, objLocationMasterDTO.ID, objLocationMasterDTO.GUID);
                                }
                                if (!objDTO.SerialNumberTracking)
                                {
                                    ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO = new ToolAssetQuantityDetailDTO();
                                    objToolAssetQuantityDetailDTO.ToolGUID = objDTO.GUID;

                                    objToolAssetQuantityDetailDTO.AssetGUID = null;


                                    objToolAssetQuantityDetailDTO.ToolBinID = objToolLocationDetailsDTO != null ? objToolLocationDetailsDTO.ID : ((objLocationMasterDTO != null) ? objLocationMasterDTO.ID : 0);
                                    objToolAssetQuantityDetailDTO.Quantity = objDTO.Quantity;
                                    objToolAssetQuantityDetailDTO.RoomID = SessionHelper.RoomID;
                                    objToolAssetQuantityDetailDTO.CompanyID = SessionHelper.CompanyID;
                                    objToolAssetQuantityDetailDTO.Created = objDTO.Created;
                                    objToolAssetQuantityDetailDTO.Updated = objDTO.Updated ?? DateTimeUtility.DateTimeNow;
                                    objToolAssetQuantityDetailDTO.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                                    objToolAssetQuantityDetailDTO.ReceivedOn = objDTO.ReceivedOn;
                                    objToolAssetQuantityDetailDTO.AddedFrom = "Web";
                                    objToolAssetQuantityDetailDTO.EditedFrom = "Web";
                                    objToolAssetQuantityDetailDTO.WhatWhereAction = "ToolKitController>>ToolKitSave";
                                    objToolAssetQuantityDetailDTO.ReceivedDate = null;
                                    objToolAssetQuantityDetailDTO.InitialQuantityWeb = objDTO.Quantity;
                                    objToolAssetQuantityDetailDTO.InitialQuantityPDA = 0;
                                    objToolAssetQuantityDetailDTO.ExpirationDate = null;
                                    objToolAssetQuantityDetailDTO.EditedOnAction = "Tool Kit Update From Web Page. Update Entry of Tool Kit.";
                                    objToolAssetQuantityDetailDTO.CreatedBy = SessionHelper.UserID;
                                    objToolAssetQuantityDetailDTO.UpdatedBy = SessionHelper.UserID;
                                    objToolAssetQuantityDetailDTO.IsDeleted = false;
                                    objToolAssetQuantityDetailDTO.IsArchived = false;

                                    ToolAssetQuantityDetailDAL objToolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
                                    objToolAssetQuantityDetailDAL.UpdateOrInsert(objToolAssetQuantityDetailDTO, null);
                                }
                                if (Session["ToolBinReplanish"] != null)
                                {
                                    List<LocationMasterDTO> lstAllLocation = (List<LocationMasterDTO>)Session["ToolBinReplanish"];

                                    if (lstAllLocation != null && lstAllLocation.Count() > 0 && lstAllLocation.Any())
                                    {
                                        for (int i = 0; i < lstAllLocation.Count(); i++)
                                        {
                                            objLocationMasterDTO = objLocationMasterDAL.GetToolLocation(objDTO.GUID, lstAllLocation[i].Location, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "AssetController>>ToolSave", lstAllLocation[i].IsDefault);
                                            lstAllLocation[i].GUID = objLocationMasterDTO.GUID;
                                        }
                                    }

                                    ToolLocationDetailsDAL objToolLocationDetailDAL = new ToolLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                                    List<ToolLocationDetailsDTO> lstToolLocationDetailsDTO = objToolLocationDetailDAL.GetToolLocationsByToolGUID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, objDTO.GUID).ToList();

                                    List<Guid> objGUIDList = lstAllLocation.Select(u => u.GUID).ToList();

                                    lstToolLocationDetailsDTO = lstToolLocationDetailsDTO.Where(l => (!objGUIDList.Contains(l.LocationGUID))).ToList();
                                    if (lstToolLocationDetailsDTO != null && lstToolLocationDetailsDTO.Count() > 0 && lstToolLocationDetailsDTO.Any())
                                    {
                                        foreach (ToolLocationDetailsDTO t in lstToolLocationDetailsDTO)
                                        {
                                            if (!string.IsNullOrWhiteSpace(t.ToolLocationName))
                                            {
                                                objToolLocationDetailDAL.DeleteByToolLocationGuid(t.LocationGUID, SessionHelper.UserID, "ToolController>>ToolSave", "Web", objDTO.GUID);
                                            }
                                        }
                                    }
                                    if (lstAllLocation == null || lstAllLocation.Count() == 0 || (!lstAllLocation.Any()))
                                    {

                                        objLocationMasterDAL.UpdateToolWithDefault(objDTO.GUID, 0, Guid.Empty);

                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);
                        status = "fail";
                    }
                }
            }

            return Json(new { Message = message, Status = status, ToolID = TempToolID }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeletetoSeesionToolKitComponentSingle(Int64 ID, Int32 SessionSr, Guid GUID, Guid ITEMGUID, Guid KitGUID, double QuantityPerKit)
        {

            List<ToolDetailDTO> lstBinReplanish = null;
            if (Session["ToolKitDetail"] != null)
            {
                lstBinReplanish = (List<ToolDetailDTO>)Session["ToolKitDetail"];
            }
            else
            {
                lstBinReplanish = new List<ToolDetailDTO>();
            }

            ///Delete the Records......
            if (ID > 0)
            {
                try
                {

                    //check the kit deletable logic if it is allow or nots
                    ToolDetailDAL objKitDetailDAL = new ToolDetailDAL(SessionHelper.EnterPriseDBName);
                    if (objKitDetailDAL.IsKitItemDeletable(GUID.ToString(), SessionHelper.RoomID, SessionHelper.CompanyID))
                    {
                        lstBinReplanish.Remove(lstBinReplanish.Where(t => t.ID == ID).FirstOrDefault());

                        Session["ToolKitDetail"] = lstBinReplanish;
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
                        lstBinReplanish.Remove(lstBinReplanish.Where(t => t.ToolItemGUID == ITEMGUID && t.SessionSr == SessionSr).FirstOrDefault());
                    }
                    else
                    {
                        lstBinReplanish.Remove(lstBinReplanish.Where(t => t.GUID == GUID && t.SessionSr == SessionSr).FirstOrDefault());
                    }

                    Session["ToolKitDetail"] = lstBinReplanish;
                }
                catch
                {
                    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "deleted" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult ToolQtyToMoveIn(List<ToolMoveInOutDetailDTO> MoveInDTO)
        {
            ToolMoveInOutDetailDAL ToolMoveInOutDtlDAL = null;
            ToolMasterDAL objToolMasterDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            string msg = string.Empty;
            bool AllowToolOrdering = SessionHelper.AllowToolOrdering;
            try
            {
                ToolMoveInOutDtlDAL = new ToolMoveInOutDetailDAL(SessionHelper.EnterPriseDBName);
                foreach (var item in MoveInDTO)
                {
                    item.CreatedBy = UserID;
                    item.LastUpdatedBy = UserID;
                    item.Room = RoomID;
                    item.CompanyID = CompanyID;

                    if (item.MoveInOut == "IN")
                    {
                        msg += ToolMoveInOutDtlDAL.QtyToMoveIn(item, RoomID, CompanyID, UserID, SessionHelper.EnterPriceID, ResourceHelper.CurrentCult.Name, AllowToolOrdering);



                        //ToolMasterDTO objToolMasterDTO = objToolMasterDAL.GetToolListByID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, item.ToolItemGUID, null);
                        //ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO = new ToolAssetQuantityDetailDTO();
                        //objToolAssetQuantityDetailDTO.ToolGUID = objToolMasterDTO.GUID;

                        //objToolAssetQuantityDetailDTO.AssetGUID = null;

                        //ToolLocationDetailsDAL objToolLocationDetailsDAL = new ToolLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                        //ToolLocationDetailsDTO objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolDefaultLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, objToolMasterDTO.GUID, SessionHelper.UserID, "Web", "KitController>>ToolQtytomovein");

                        //objToolAssetQuantityDetailDTO.ToolBinID = objToolLocationDetailsDTO != null ? objToolLocationDetailsDTO.ID : objToolMasterDTO.ToolLocationDetailsID;
                        //objToolAssetQuantityDetailDTO.Quantity = 0;
                        //objToolAssetQuantityDetailDTO.RoomID = SessionHelper.RoomID;
                        //objToolAssetQuantityDetailDTO.CompanyID = SessionHelper.CompanyID;
                        //objToolAssetQuantityDetailDTO.Created = DateTimeUtility.DateTimeNow;
                        //objToolAssetQuantityDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                        //objToolAssetQuantityDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        //objToolAssetQuantityDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        //objToolAssetQuantityDetailDTO.AddedFrom = "Web";
                        //objToolAssetQuantityDetailDTO.EditedFrom = "Web";
                        //objToolAssetQuantityDetailDTO.WhatWhereAction = "KitController>>ToolQtyToMoveIn";
                        //objToolAssetQuantityDetailDTO.ReceivedDate = null;
                        //objToolAssetQuantityDetailDTO.InitialQuantityWeb = 0;
                        //objToolAssetQuantityDetailDTO.InitialQuantityPDA = 0;
                        //objToolAssetQuantityDetailDTO.ExpirationDate = null;
                        //objToolAssetQuantityDetailDTO.EditedOnAction = "Tool was CheckOut from Web While Kit movein.";
                        //objToolAssetQuantityDetailDTO.CreatedBy = SessionHelper.UserID;
                        //objToolAssetQuantityDetailDTO.UpdatedBy = SessionHelper.UserID;
                        //objToolAssetQuantityDetailDTO.IsDeleted = false;
                        //objToolAssetQuantityDetailDTO.IsArchived = false;

                        //ToolAssetQuantityDetailDAL objToolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
                        //double Quantity = 0;
                        //Quantity = item.Quantity;
                        //objToolAssetQuantityDetailDAL.UpdateOrInsert(objToolAssetQuantityDetailDTO, Quantity, ReferalAction: "Move In",SerialNumber:(string.IsNullOrWhiteSpace(item.SerialNumber))?string.Empty: item.SerialNumber);
                    }
                    else if (item.MoveInOut == "OUT")
                    {


                        msg += ToolMoveInOutDtlDAL.QtyToMoveOut(item, RoomID, CompanyID, UserID, SessionHelper.EnterPriceID, ResourceHelper.CurrentCult.Name, AllowToolOrdering);
                        //if (string.IsNullOrWhiteSpace(msg))
                        //{
                        //    ToolMasterDTO objToolMasterDTO = objToolMasterDAL.GetToolListByID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, item.ToolItemGUID, null);

                        //    ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO = new ToolAssetQuantityDetailDTO();
                        //    objToolAssetQuantityDetailDTO.ToolGUID = objToolMasterDTO.GUID;

                        //    objToolAssetQuantityDetailDTO.AssetGUID = null;


                        //    ToolLocationDetailsDAL objToolLocationDetailsDAL = new ToolLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                        //    ToolLocationDetailsDTO objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolDefaultLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, objToolMasterDTO.GUID, SessionHelper.UserID, "Web", "KitController>>ToolQtytomovein");


                        //    objToolAssetQuantityDetailDTO.ToolBinID = objToolLocationDetailsDTO != null ? objToolLocationDetailsDTO.ID : objToolMasterDTO.ToolLocationDetailsID;
                        //    objToolAssetQuantityDetailDTO.Quantity = item.Quantity;
                        //    objToolAssetQuantityDetailDTO.RoomID = SessionHelper.RoomID;
                        //    objToolAssetQuantityDetailDTO.CompanyID = SessionHelper.CompanyID;
                        //    objToolAssetQuantityDetailDTO.Created = DateTimeUtility.DateTimeNow;
                        //    objToolAssetQuantityDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                        //    objToolAssetQuantityDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        //    objToolAssetQuantityDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        //    objToolAssetQuantityDetailDTO.AddedFrom = "Web";
                        //    objToolAssetQuantityDetailDTO.EditedFrom = "Web";
                        //    objToolAssetQuantityDetailDTO.WhatWhereAction = "KitController>>ToolQtyMoveIn";
                        //    objToolAssetQuantityDetailDTO.ReceivedDate = null;
                        //    objToolAssetQuantityDetailDTO.InitialQuantityWeb = objToolMasterDTO.Quantity;
                        //    objToolAssetQuantityDetailDTO.InitialQuantityPDA = 0;
                        //    objToolAssetQuantityDetailDTO.ExpirationDate = null;
                        //    objToolAssetQuantityDetailDTO.EditedOnAction = "Tool was Checkin from Web While Kit moveout.";
                        //    objToolAssetQuantityDetailDTO.CreatedBy = SessionHelper.UserID;
                        //    objToolAssetQuantityDetailDTO.UpdatedBy = SessionHelper.UserID;
                        //    objToolAssetQuantityDetailDTO.IsDeleted = false;
                        //    objToolAssetQuantityDetailDTO.IsArchived = false;

                        //    ToolAssetQuantityDetailDAL objToolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
                        //    objToolAssetQuantityDetailDAL.Insert(objToolAssetQuantityDetailDTO, false, ReferalAction: "Move Out", SerialNumber: item.SerialNumber);
                        //}
                    }
                }

                if (!string.IsNullOrEmpty(msg))
                    return Json(new { Status = false, Message = msg }, JsonRequestBehavior.AllowGet);

                return Json(new { Status = true, Message = "Success" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                ToolMoveInOutDtlDAL = null;
            }
        }
        public JsonResult ToolQtyToMoveBulk(double qty, Guid ToolKitGuid, string MoveType, List<Guid> KitDetailGuids)
        {
            string msg = string.Empty;
            ToolMoveInOutDetailDAL objToolMoveInOutDetailDAL = new ToolMoveInOutDetailDAL(SessionHelper.EnterPriseDBName);
            List<ToolMoveInOutDetailDTO> MoveInDTO = new List<ToolMoveInOutDetailDTO>();
            ToolMoveInOutDetailDAL ToolMoveInOutDtlDAL = new ToolMoveInOutDetailDAL(SessionHelper.EnterPriseDBName);
            bool AllowToolOrdering = SessionHelper.AllowToolOrdering;
            ToolMasterDAL objToolMasterDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            foreach (Guid guid in KitDetailGuids)
            {

                ToolMoveInOutDetailDTO item = new ToolMoveInOutDetailDTO();
                ToolDetailDAL objToolDetailDAL = new ToolDetailDAL(SessionHelper.EnterPriseDBName);
                ToolDetailDTO objToolDetailDTO = objToolDetailDAL.GetRecord(guid.ToString(), SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false);

                double totalMoveQty = qty * objToolDetailDTO.QuantityPerKit.GetValueOrDefault(0);

                if (MoveType == "IN")
                {

                    item.GUID = guid;
                    item.CreatedBy = UserID;
                    item.LastUpdatedBy = UserID;
                    item.ToolDetailGUID = ToolKitGuid;
                    item.ToolItemGUID = objToolDetailDTO.ToolItemGUID;
                    item.Quantity = totalMoveQty;
                    item.Room = RoomID;
                    item.ReasonFromMove = "From Kit Page";
                    item.CompanyID = CompanyID;
                    msg += ToolMoveInOutDtlDAL.QtyToMoveIn(item, RoomID, CompanyID, UserID, SessionHelper.EnterPriceID, ResourceHelper.CurrentCult.Name, AllowToolOrdering);

                    //ToolMasterDTO objToolMasterDTO = objToolMasterDAL.GetToolListByID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, item.ToolItemGUID, null);
                    //ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO = new ToolAssetQuantityDetailDTO();
                    //objToolAssetQuantityDetailDTO.ToolGUID = objToolMasterDTO.GUID;

                    //objToolAssetQuantityDetailDTO.AssetGUID = null;


                    //objToolAssetQuantityDetailDTO.ToolBinID = objToolMasterDTO.ToolLocationDetailsID;
                    //objToolAssetQuantityDetailDTO.Quantity = 0;
                    //objToolAssetQuantityDetailDTO.RoomID = SessionHelper.RoomID;
                    //objToolAssetQuantityDetailDTO.CompanyID = SessionHelper.CompanyID;
                    //objToolAssetQuantityDetailDTO.Created = DateTimeUtility.DateTimeNow;
                    //objToolAssetQuantityDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                    //objToolAssetQuantityDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    //objToolAssetQuantityDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    //objToolAssetQuantityDetailDTO.AddedFrom = "Web";
                    //objToolAssetQuantityDetailDTO.EditedFrom = "Web";
                    //objToolAssetQuantityDetailDTO.WhatWhereAction = "KitController>>ToolQtyToMoveInBulk";
                    //objToolAssetQuantityDetailDTO.ReceivedDate = null;
                    //objToolAssetQuantityDetailDTO.InitialQuantityWeb = 0;
                    //objToolAssetQuantityDetailDTO.InitialQuantityPDA = 0;
                    //objToolAssetQuantityDetailDTO.ExpirationDate = null;
                    //objToolAssetQuantityDetailDTO.EditedOnAction = "Tool was CheckOut from Web While Kit movein Bulk.";
                    //objToolAssetQuantityDetailDTO.CreatedBy = SessionHelper.UserID;
                    //objToolAssetQuantityDetailDTO.UpdatedBy = SessionHelper.UserID;
                    //objToolAssetQuantityDetailDTO.IsDeleted = false;
                    //objToolAssetQuantityDetailDTO.IsArchived = false;

                    //ToolAssetQuantityDetailDAL objToolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
                    //double Quantity = 0;
                    //Quantity = item.Quantity;
                    //objToolAssetQuantityDetailDAL.UpdateOrInsert(objToolAssetQuantityDetailDTO, Quantity, ReferalAction: "Move In");

                }
                else if (MoveType == "OUT")
                {
                    ToolMasterDTO objToolMasterDTO = null;
                    item = objToolMoveInOutDetailDAL.GetToolMoveInOutDetailByToolDetailGUID(guid, false, false);
                    if (item != null)
                    {

                        item.GUID = guid;
                        item.CreatedBy = UserID;
                        item.LastUpdatedBy = UserID;
                        item.ToolDetailGUID = ToolKitGuid;
                        item.ToolItemGUID = objToolDetailDTO.ToolItemGUID;
                        item.LastUpdatedBy = UserID;
                        item.Room = RoomID;
                        item.ReasonFromMove = "From Kit Page";
                        item.CompanyID = CompanyID;
                        item.Quantity = totalMoveQty;

                        msg += ToolMoveInOutDtlDAL.QtyToMoveOut(item, RoomID, CompanyID, UserID, SessionHelper.EnterPriceID, ResourceHelper.CurrentCult.Name, AllowToolOrdering);


                    }
                    else
                    {

                        List<ToolMoveInOutDetailDTO> lstToolMoveInOutDetailDTO = new ToolMoveInOutDetailDAL(SessionHelper.EnterPriseDBName).GetAllToolMoveInOutDetailByToolDetailGUID(item.GUID, false, false);
                        double? MoveInTotal = lstToolMoveInOutDetailDTO.Where(t => t.MoveInOut.ToLower() == "in").Sum(t => t.Quantity);
                        double? MoveOutTotal = lstToolMoveInOutDetailDTO.Where(t => t.MoveInOut.ToLower() == "out").Sum(t => t.Quantity);
                        objToolMasterDTO = objToolMasterDAL.GetToolByGUIDPlain(objToolDetailDTO.ToolItemGUID ?? Guid.Empty);
                        double checkinQty = qty;
                        if (MoveInTotal >= MoveOutTotal)
                        {
                            foreach (ToolMoveInOutDetailDTO t in lstToolMoveInOutDetailDTO)
                            {
                                if (checkinQty > 0)
                                {
                                    double Qty = (t.Quantity >= checkinQty ? checkinQty : t.Quantity);
                                    objToolMoveInOutDetailDAL = new ToolMoveInOutDetailDAL(SessionHelper.EnterPriseDBName);
                                    ToolMoveInOutDetailDTO objToolMoveInDTO = new ToolMoveInOutDetailDTO();
                                    objToolMoveInDTO.GUID = Guid.NewGuid();
                                    objToolMoveInDTO.ToolDetailGUID = item.GUID;
                                    objToolMoveInDTO.ToolItemGUID = item.ToolItemGUID;
                                    objToolMoveInDTO.MoveInOut = "OUT";
                                    objToolMoveInDTO.Quantity = Qty;
                                    objToolMoveInDTO.Created = DateTimeUtility.DateTimeNow;
                                    objToolMoveInDTO.Updated = DateTimeUtility.DateTimeNow;
                                    objToolMoveInDTO.CreatedBy = item.CreatedBy;
                                    objToolMoveInDTO.LastUpdatedBy = item.LastUpdatedBy;
                                    objToolMoveInDTO.CompanyID = CompanyID;
                                    objToolMoveInDTO.Room = RoomID;
                                    objToolMoveInDTO.IsDeleted = false;
                                    objToolMoveInDTO.IsArchived = false;
                                    objToolMoveInDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                    objToolMoveInDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    objToolMoveInDTO.AddedFrom = "web";
                                    objToolMoveInDTO.EditedFrom = "web";
                                    objToolMoveInDTO.ReasonFromMove = "From Kit Page";
                                    //objToolMoveInDTO.ToolDetailGUID = InOutDTO.ToolDetailGUID;
                                    objToolMoveInDTO.WhatWhereAction = "ToolMoveInOutDetailDAL-->QtyToMoveOut";
                                    new ToolMoveInOutDetailDAL(SessionHelper.EnterPriseDBName).Insert(objToolMoveInDTO);

                                    objToolMasterDTO = objToolMasterDAL.GetToolByGUIDFull(item.ToolItemGUID.GetValueOrDefault(Guid.Empty));
                                    if (AllowToolOrdering)
                                    {
                                        ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO = new ToolAssetQuantityDetailDTO();
                                        objToolAssetQuantityDetailDTO.ToolGUID = objToolMasterDTO.GUID;

                                        objToolAssetQuantityDetailDTO.AssetGUID = null;


                                        ToolLocationDetailsDAL objToolLocationDetailsDAL = new ToolLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                                        ToolLocationDetailsDTO objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolDefaultLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, objToolMasterDTO.GUID, SessionHelper.UserID, "Web", "ToolKitController>>ToolQtytomovein");


                                        objToolAssetQuantityDetailDTO.ToolBinID = objToolLocationDetailsDTO != null ? objToolLocationDetailsDTO.ID : objToolMasterDTO.ToolLocationDetailsID;
                                        objToolAssetQuantityDetailDTO.Quantity = item.Quantity;
                                        objToolAssetQuantityDetailDTO.RoomID = SessionHelper.RoomID;
                                        objToolAssetQuantityDetailDTO.CompanyID = SessionHelper.CompanyID;
                                        objToolAssetQuantityDetailDTO.Created = DateTimeUtility.DateTimeNow;
                                        objToolAssetQuantityDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                                        objToolAssetQuantityDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                        objToolAssetQuantityDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        objToolAssetQuantityDetailDTO.AddedFrom = "Web";
                                        objToolAssetQuantityDetailDTO.EditedFrom = "Web";
                                        objToolAssetQuantityDetailDTO.WhatWhereAction = "ToolKitController>>ToolQtyMoveIn";
                                        objToolAssetQuantityDetailDTO.ReceivedDate = null;
                                        objToolAssetQuantityDetailDTO.InitialQuantityWeb = objToolMasterDTO.Quantity;
                                        objToolAssetQuantityDetailDTO.InitialQuantityPDA = 0;
                                        objToolAssetQuantityDetailDTO.ExpirationDate = null;
                                        objToolAssetQuantityDetailDTO.EditedOnAction = "Tool was Checkin from Web While Kit moveout.";
                                        objToolAssetQuantityDetailDTO.CreatedBy = SessionHelper.UserID;
                                        objToolAssetQuantityDetailDTO.UpdatedBy = SessionHelper.UserID;
                                        objToolAssetQuantityDetailDTO.IsDeleted = false;
                                        objToolAssetQuantityDetailDTO.IsArchived = false;

                                        ToolAssetQuantityDetailDAL objToolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);

                                        if (t.ReasonFromMove == "Order Kit Received")
                                        {
                                            objToolMasterDTO.Quantity = (objToolMasterDTO.Quantity) + Qty;
                                            objToolAssetQuantityDetailDAL.Insert(objToolAssetQuantityDetailDTO, false, ReferalAction: "Move Out", SerialNumber: item.SerialNumber);
                                        }
                                        else
                                        {
                                            objToolAssetQuantityDetailDAL.Insert(objToolAssetQuantityDetailDTO, false, ReferalAction: "Move Out", SerialNumber: item.SerialNumber);
                                        }
                                    }
                                    checkinQty = checkinQty - Qty;
                                }
                            }
                        }
                        objToolDetailDTO.AvailableItemsInWIP = objToolDetailDTO.AvailableItemsInWIP.GetValueOrDefault(0) - ((objToolDetailDTO.QuantityPerKit ?? 0) * qty);
                        objToolDetailDTO.LastUpdatedBy = SessionHelper.UserID;
                        objToolDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objToolDetailDTO.EditedFrom = "Web";
                        objToolDetailDTO.WhatWhereAction = "KitController-->WithoutMovein- out Entry";
                        objToolDetailDAL.Edit(objToolDetailDTO);


                        objToolMasterDTO.EditedFrom = "Web";
                        objToolMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                        objToolMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objToolMasterDTO.Quantity = (objToolMasterDTO.Quantity) + ((objToolDetailDTO.QuantityPerKit ?? 0) * qty);
                        objToolMasterDAL.Edit(objToolMasterDTO);
                    }
                    //if (objToolMasterDTO != null)
                    //{
                    //    ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO = new ToolAssetQuantityDetailDTO();

                    //    objToolAssetQuantityDetailDTO.ToolGUID = objToolMasterDTO.GUID;

                    //    objToolAssetQuantityDetailDTO.AssetGUID = null;


                    //    objToolAssetQuantityDetailDTO.ToolBinID = objToolMasterDTO.ToolLocationDetailsID;
                    //    objToolAssetQuantityDetailDTO.Quantity = ((objToolDetailDTO.QuantityPerKit ?? 0) * qty);
                    //    objToolAssetQuantityDetailDTO.RoomID = SessionHelper.RoomID;
                    //    objToolAssetQuantityDetailDTO.CompanyID = SessionHelper.CompanyID;
                    //    objToolAssetQuantityDetailDTO.Created = DateTimeUtility.DateTimeNow;
                    //    objToolAssetQuantityDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                    //    objToolAssetQuantityDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    //    objToolAssetQuantityDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    //    objToolAssetQuantityDetailDTO.AddedFrom = "Web";
                    //    objToolAssetQuantityDetailDTO.EditedFrom = "Web";
                    //    objToolAssetQuantityDetailDTO.WhatWhereAction = "KitController>>ToolQtyToMoveBulk";
                    //    objToolAssetQuantityDetailDTO.ReceivedDate = null;
                    //    objToolAssetQuantityDetailDTO.InitialQuantityWeb = objToolMasterDTO.Quantity;
                    //    objToolAssetQuantityDetailDTO.InitialQuantityPDA = 0;
                    //    objToolAssetQuantityDetailDTO.ExpirationDate = null;
                    //    objToolAssetQuantityDetailDTO.EditedOnAction = "Tool was Checkin from Web While Kit moveout bulk.";
                    //    objToolAssetQuantityDetailDTO.CreatedBy = SessionHelper.UserID;
                    //    objToolAssetQuantityDetailDTO.UpdatedBy = SessionHelper.UserID;
                    //    objToolAssetQuantityDetailDTO.IsDeleted = false;
                    //    objToolAssetQuantityDetailDTO.IsArchived = false;

                    //    ToolAssetQuantityDetailDAL objToolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
                    //    objToolAssetQuantityDetailDAL.Insert(objToolAssetQuantityDetailDTO, false, ReferalAction: "Move Out");
                    //}
                }
            }
            if (!string.IsNullOrEmpty(msg))
                return Json(new { Status = false, Message = msg }, JsonRequestBehavior.AllowGet);
            return Json(new { Status = true, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }
        private List<CommonDTO> GetGroupOfItems()
        {
            List<CommonDTO> ItemType = new List<CommonDTO>();

            ItemType.Add(new CommonDTO() { ID = 1, Text = "Yes" });
            ItemType.Add(new CommonDTO() { ID = 0, Text = "No" });

            return ItemType;
        }
        public ActionResult LoadItemMasterModel(KitMasterDTO KitDTO)
        {
            ItemModelPerameter obj = new ItemModelPerameter()
            {
                AjaxURLAddMultipleItemToSession = "~/ToolKit/AddItemToDetailTable/",
                AjaxURLToFillItemGrid = "~/ToolKit/GetItemsModelMethod/",
                CallingFromPageName = "KIT",
                AjaxURLAddItemToSession = "~/ToolKit/AddItemToDetailTable/",
                ModelHeader = eTurns.DTO.ResKitMaster.PageHeader,
                PerentID = KitDTO.ID.ToString(),
                PerentGUID = KitDTO.GUID.ToString(),
            };

            return PartialView("ItemMasterModel", obj);
        }
        public JsonResult AddItemToDetailTable(string para)
        {
            string message = "";
            string status = "";
            try
            {
                JavaScriptSerializer s = new JavaScriptSerializer();
                KitDetailDTO[] QLDetails = s.Deserialize<KitDetailDTO[]>(para);
                //KitDetailController objApi = new KitDetailController();
                KitDetailDAL objApi = new KitDetailDAL(SessionHelper.EnterPriseDBName);
                long SessionUserId = SessionHelper.UserID;
                var enterpriseId = SessionHelper.EnterPriceID;

                foreach (KitDetailDTO item in QLDetails)
                {
                    item.Room = SessionHelper.RoomID;
                    item.RoomName = SessionHelper.RoomName;
                    item.CreatedBy = SessionHelper.UserID;
                    item.CreatedByName = SessionHelper.UserName;
                    item.UpdatedByName = SessionHelper.UserName;
                    item.LastUpdatedBy = SessionHelper.UserID;
                    item.CompanyID = SessionHelper.CompanyID;
                    if (item.ID > 0)
                    {
                        item.ReceivedOn = DateTimeUtility.DateTimeNow;
                        item.EditedFrom = "Web";
                        objApi.Edit(item, SessionUserId,enterpriseId);
                    }
                    else
                    {
                        item.QuantityReadyForAssembly = 0;
                        item.AvailableItemsInWIP = 0;
                        //List<KitDetailDTO> tempDTO = objApi.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false).Where(x => x.KitGUID == item.KitGUID && x.ItemGUID == item.ItemGUID).ToList();
                        List<KitDetailDTO> tempDTO = objApi.GetKitDetail(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false, Convert.ToString(item.KitGUID), Convert.ToString(item.ItemGUID)).ToList();
                        if (tempDTO == null || tempDTO.Count == 0)
                            objApi.Insert(item, SessionUserId,enterpriseId);
                    }
                }

                message = ResCommon.RecordsSavedSuccessfully;
                status = "ok";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                message = "Error";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                //throw;
            }
        }
        public ActionResult ToolKitBuildBreak(Guid KitToolGUID)
        {
            ToolMasterDAL obj = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            ToolMasterDTO objDTO = obj.GetToolByGUIDFull(KitToolGUID);
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("ToolMaster");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }

            ViewBag.ReOrderTypeList = GetReorderTypeOrKitCategory(0);
            ViewBag.KitCategoryList = GetReorderTypeOrKitCategory(1);
            //objDTO.KitCost = objDTO.KitCost.GetValueOrDefault(0) * objDTO.AvailableKitQuantity.GetValueOrDefault(0);
            ToolDetailDAL kitDtlDAL = new ToolDetailDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<ToolDetailDTO> objKits = kitDtlDAL.GetAllRecordsByKitGUID(KitToolGUID, RoomID, CompanyID, false, false, false);
            objDTO.NoOfItemsInKit = objKits.Where(x => x.QuantityReadyForAssembly.GetValueOrDefault(0) > 0).Count();
            objDTO.WIPKitCost = objKits.Where(x => x.AvailableItemsInWIP.GetValueOrDefault(0) > 0).Sum(x => x.AvailableItemsInWIP.GetValueOrDefault(0) * x.Cost.GetValueOrDefault(0));

            return PartialView("_KitToolBuildBreak", objDTO);
        }
        public ActionResult LoadToolKitLineItemsByToolKitMasterDTO(Guid ToolKitGUID)
        {

            ToolMasterDAL objToolMasterDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            ToolMasterDTO ToolMasterDTO = objToolMasterDAL.GetToolByGUIDFull(ToolKitGUID);
            ToolMasterDTO.ToolKitItemList = new ToolDetailDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByKitGUIDNew(ToolKitGUID, SessionHelper.RoomID, SessionHelper.CompanyID, ToolMasterDTO.IsArchived.GetValueOrDefault(false), ToolMasterDTO.IsDeleted.GetValueOrDefault(false), true).ToList();
            return PartialView("_ToolKitLineItemsForBuildBreak", ToolMasterDTO);
            //return PartialView("_ToolKitLineItemsForBuildBreak", ToolMasterDTO);
        }
        public JsonResult ToolUpdateKitCost(ToolMasterDTO KitDTO)
        {
            try
            {
                string message = ResMessage.SaveMessage;
                string status = "ok";
                ToolMasterDAL objToolMstCtrl = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                //KitMasterController objKitMstCtrl = new KitMasterController();
                KitDTO = objToolMstCtrl.GetToolByGUIDFull(KitDTO.GUID);
                if (KitDTO.IsDeleted.GetValueOrDefault(false) || KitDTO.IsArchived.GetValueOrDefault(false) || (!isUpdate && Convert.ToString(Session["IsInsert"]) != "True"))
                {
                    return Json(new { Message = message, Status = status, KitDTO = KitDTO }, JsonRequestBehavior.AllowGet);
                }

                //KitMasterDTO objKitMasterDTO = objKitMstCtrl.UpdateKitCost(KitDTO, SessionHelper.RoomID, SessionHelper.CompanyID);
                ToolDetailDAL kitDtlDAL = new ToolDetailDAL(SessionHelper.EnterPriseDBName);
                IEnumerable<ToolDetailDTO> objKits = kitDtlDAL.GetAllRecordsByKitGUID(KitDTO.GUID, RoomID, CompanyID, false, false, false);
                KitDTO.NoOfItemsInKit = objKits.Where(x => x.QuantityReadyForAssembly.GetValueOrDefault(0) > 0).Count();
                KitDTO.WIPKitCost = objKits.Where(x => x.AvailableItemsInWIP.GetValueOrDefault(0) > 0).Sum(x => x.AvailableItemsInWIP.GetValueOrDefault(0) * x.Cost.GetValueOrDefault(0));


                return Json(new { Message = message, Status = status, KitDTO = KitDTO }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new { Message = "fail", Status = "fail", KitDTO = new KitMasterDTO() }, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult ToolLocationDetails(string ToolID_ToolType)
        {
            string ToolGUID = ToolID_ToolType.Split('#')[0];
            Int32 Type = Int32.Parse(ToolID_ToolType.Split('#')[1]);
            Guid? ToolAssetOrderDetailGUID = null;
            Guid ICDtlGuid = Guid.Empty;
            long CountBinID = 0;
            double CountQuantity = 0;
            int count = 10;
            bool isPullCredit = false;

            if (ToolID_ToolType.Contains("frompullcredit"))
            {
                string[] arrSplited = ToolID_ToolType.Split('#');
                isPullCredit = true;
                ViewBag.IsPullCredit = true;
                ViewBag.ForCreditPull = "ForCreditPull";

                if (ToolID_ToolType.Contains("forcount"))
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

                if (!ToolID_ToolType.Contains("FROMTOOLKITMASTER") && ToolID_ToolType.Split('#').Length > 2 && !string.IsNullOrEmpty(ToolID_ToolType.Split('#')[2]) && Guid.Parse(ToolID_ToolType.Split('#')[2]) != Guid.Empty)
                {
                    ToolAssetOrderDetailGUID = Guid.Parse(ToolID_ToolType.Split('#')[2]);
                    ViewBag.ToolAssetOrderDetailGUID = ToolAssetOrderDetailGUID;
                    double dblReceivedQty = 0;
                    double dblApproveQty = 0;
                    ToolAssetOrderDetailsDTO OrdDetailDTO = new ToolAssetOrderDetailsDAL(SessionHelper.EnterPriseDBName).GetRecord(ToolAssetOrderDetailGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (OrdDetailDTO != null)
                    {
                        dblReceivedQty = OrdDetailDTO.ReceivedQuantity.GetValueOrDefault(0);
                        dblApproveQty = OrdDetailDTO.ApprovedQuantity.GetValueOrDefault(0);
                    }
                    ToolAssetOrderMasterDTO ordDTO = new ToolAssetOrderMasterDAL(SessionHelper.EnterPriseDBName).GetRecord(OrdDetailDTO.ToolAssetOrderGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                    ViewBag.OrdType = ordDTO.OrderType.GetValueOrDefault(1);
                    ViewBag.ReceivedQty = dblReceivedQty;
                    ViewBag.ApprovedQty = dblApproveQty;
                }

                if (!ToolID_ToolType.Contains("FROMTOOLKITMASTER") && ToolID_ToolType.Split('#').Length > 3 && !string.IsNullOrEmpty(ToolID_ToolType.Split('#')[3]) && Int64.Parse(ToolID_ToolType.Split('#')[3]) > 0)
                {
                    ViewBag.OrderBinLocationID = int.Parse(ToolID_ToolType.Split('#')[3]);
                }
                else if (ToolID_ToolType.Contains("FROMTOOLKITMASTER") && ToolID_ToolType.Split('#').Length > 3 && !string.IsNullOrEmpty(ToolID_ToolType.Split('#')[3]) && Int64.Parse(ToolID_ToolType.Split('#')[3]) > 0)
                {
                    ViewBag.KitBinLocationID = int.Parse(ToolID_ToolType.Split('#')[3]);
                    count = int.Parse(ToolID_ToolType.Split('#')[5]);
                    ViewBag.ForCreditPull = "ForSerailKitBuild";
                }
            }

            ViewBag.ToolGUID = ToolGUID;
            //ItemMasterController objItemAPI = new ItemMasterController();
            ToolMasterDAL objItemAPI = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            Guid ItemGUID1 = Guid.Empty;
            Guid.TryParse(ToolGUID, out ItemGUID1);
            var Objitem = objItemAPI.GetToolByGUIDPlain(ItemGUID1);
            ViewBag.ToolID_ToolType = ToolID_ToolType;//ItemGUID + "#" + Objitem.ItemType;
            //if (Objitem.IsItemLevelMinMaxQtyRequired == true)
            //{
            LocationMasterDAL objCommon = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.BinLocations = objCommon.GetLocationByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            if (ToolID_ToolType.ToLower().Contains("forcount"))
            {
                List<LocationMasterDTO> lstLocation = new List<LocationMasterDTO>();
                LocationMasterDTO objLoc = objCommon.GetLocationByIDPlain(CountBinID, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (objLoc != null)
                {
                    lstLocation.Add(objLoc);
                }
                ViewBag.BinLocations = lstLocation;
                //ViewBag.BinLocations = objCommon.GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false,Guid.Empty, CountBinID, null, false);//.Where(x => x.IsStagingLocation == false && x.ID == CountBinID);
            }
            if (ToolID_ToolType.Contains("FROMTOOLKITMASTER"))
            {

                List<LocationMasterDTO> lstLocation = new List<LocationMasterDTO>();
                LocationMasterDTO objLoc = objCommon.GetLocationByIDPlain(Objitem.DefaultLocation.GetValueOrDefault(0), SessionHelper.RoomID, SessionHelper.CompanyID);
                if (objLoc != null)
                {
                    lstLocation.Add(objLoc);
                }
                ViewBag.BinLocations = lstLocation;
            }

            //}
            //else
            //{
            //    ItemLocationLevelQuanityDAL objILQDal = new ItemLocationLevelQuanityDAL();
            //    ViewBag.BinLocations =  objILQDal.GetAllRecordsItemWise(Objitem.GUID, SessionHelper.RoomID, SessionHelper.CompanyID);
            //}

            //ItemLocationDetailsController objAPI = new ItemLocationDetailsController();
            ToolAssetQuantityDetailDAL objAPI = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
            List<ToolAssetQuantityDetailDTO> lstData = new List<ToolAssetQuantityDetailDTO>();
            if (!isPullCredit && !ToolID_ToolType.Contains("FROMTOOLKITMASTER"))
            {
                lstData = null;// objAPI.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, Guid.Parse(ToolGUID), ToolAssetOrderDetailGUID, "ID DESC").ToList();
            }
            //Add empty Rows to the list
            for (int i = 0; i < count; i++)
            {
                ToolAssetQuantityDetailDTO objEmplty = new ToolAssetQuantityDetailDTO();
                //objEmplty.ID = i;
                objEmplty.ToolGUID = Objitem.GUID;
                objEmplty.ToolName = Objitem.ToolName;
                objEmplty.Cost = Objitem.Cost;
                objEmplty.Created = DateTimeUtility.DateTimeNow;
                objEmplty.ToolGUID = Objitem.GUID;
                objEmplty.SerialNumberTracking = Objitem.SerialNumberTracking;

                if (Objitem.SerialNumberTracking)
                {

                    objEmplty.Quantity = 1;

                }

                objEmplty.LotNumberTracking = Objitem.LotNumberTracking;
                objEmplty.ToolAssetOrderDetailGUID = ToolAssetOrderDetailGUID;
                //objEmplty.Expiration = DateTime.Now.ToString("MM-dd-yy");
                //objEmplty.Received = DateTime.Now.ToString("MM-dd-yy");

                //if (isPullCredit)
                //    objEmplty.IsCreditPull = true;
                //else
                //    objEmplty.IsCreditPull = false;



                // objEmplty.IsOnlyFromUI = true;
                lstData.Add(objEmplty);
            }

            //Set default SerialNumberTracking OR LotNumberTracking - for all records... assuming it is can't be edited after one location add
            lstData = lstData.Select(c => { if (Objitem.SerialNumberTracking) { c.SerialNumberTracking = true; } if (Objitem.LotNumberTracking) { c.LotNumberTracking = true; } return c; }).ToList();
            ViewBag.IsSerialNumberTracking = Objitem.SerialNumberTracking;
            ViewBag.IsLotNumberTracking = Objitem.LotNumberTracking;
            ViewBag.IsDateCodeTracking = Objitem.DateCodeTracking;
            ViewBag.ToolName = Objitem.ToolName;

            if (Objitem.DefaultLocation > 0)
            {
                ViewBag.DefaultLocationBag = Objitem.DefaultLocation;
            }
            return PartialView("_ToolLocationDetails", lstData);
        }
        public ActionResult ShowMoveOutQtyModel(string KitDetailGUID, Int64 Qty)
        {
            ToolDetailDTO kitDetailDTO = new ToolDetailDAL(SessionHelper.EnterPriseDBName).GetRecord(KitDetailGUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false);

            Guid ToolGUID = kitDetailDTO.ToolItemGUID.GetValueOrDefault(Guid.Empty);
            //Int32 ItemType = kitDetailDTO.ItemDetail.ItemType;

            ViewBag.ForCreditPull = "ForKitCredit";
            ViewBag.KitDetailID = kitDetailDTO.ID;
            ViewBag.KitDetailGUID = kitDetailDTO.GUID;
            ViewBag.ToolGUID = ToolGUID;
            ViewBag.IsPullCredit = "True";
            LocationMasterDAL objCommon = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<LocationMasterDTO> lstBinDTO = objCommon.GetLocationByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();

            ViewBag.BinLocations = lstBinDTO;

            //ItemMasterController objItemAPI = new ItemMasterController();
            ToolMasterDAL objItemDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            var Objitem = objItemDAL.GetToolByGUIDPlain(ToolGUID);

            ViewBag.ItemGUID_ItemType = ToolGUID.ToString() + "#" + Objitem.Type;

            List<ToolAssetQuantityDetailDTO> lstData = new List<ToolAssetQuantityDetailDTO>();
            ViewBag.AvailableInWIP = kitDetailDTO.AvailableItemsInWIP.GetValueOrDefault(0).ToString("N" + SessionHelper.NumberDecimalDigits);

            List<ToolCheckInOutHistoryDTO> lstToolCheckInOutHistory = new List<ToolCheckInOutHistoryDTO>();
            ToolCheckInOutHistoryDAL objToolCheckInOutHistoryDAL = new ToolCheckInOutHistoryDAL(SessionHelper.EnterPriseDBName);
            lstToolCheckInOutHistory = objToolCheckInOutHistoryDAL.GetRecordByToolDetailGUID(kitDetailDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID).Where(t => t.CheckedOutMQTYCurrent == 0 && t.CheckedOutQTYCurrent == 0).ToList();

            double Count = 0;
            Count = kitDetailDTO.AvailableItemsInWIP.GetValueOrDefault(0);
            if (Objitem != null)
            {
                if (Objitem.SerialNumberTracking == false && Objitem.LotNumberTracking == false && Objitem.DateCodeTracking == false)
                {
                    Count = 1;
                }
            }
            if (Qty != 0)
            {
                Count = Qty;
            }
            //Add empty Rows to the list
            for (int i = 0; i < Count; i++)
            {
                ToolAssetQuantityDetailDTO objEmplty = new ToolAssetQuantityDetailDTO();
                //objEmplty.ID = i;
                objEmplty.ToolName = Objitem.ToolName;
                objEmplty.Cost = Objitem.Cost;
                objEmplty.Created = DateTimeUtility.DateTimeNow;
                objEmplty.ToolGUID = Objitem.GUID;
                objEmplty.SerialNumberTracking = Objitem.SerialNumberTracking;

                if (Objitem.SerialNumberTracking)
                {
                    objEmplty.Quantity = 1;
                }
                objEmplty.SerialNumber = lstToolCheckInOutHistory[i].SerialNumber;
                objEmplty.LotNumberTracking = Objitem.LotNumberTracking;

                //objEmplty.IsCreditPull = true;
                //objEmplty.KitDetailID = kitDetailDTO.ID;
                // objEmplty.KitDetailGUID = Guid.Parse(KitDetailGUID);
                if (!string.IsNullOrEmpty(Objitem.Location))
                {
                    objEmplty.Location = Objitem.Location;
                }
                else if (lstBinDTO != null && lstBinDTO.Count() > 0)
                {
                    objEmplty.Location = lstBinDTO.OrderBy("Location asc").FirstOrDefault().Location;
                }
                objEmplty.ToolDetailsGUID = kitDetailDTO.GUID;
                lstData.Add(objEmplty);
            }

            //Set default SerialNumberTracking OR LotNumberTracking - for all records... assuming it is can't be edited after one location add
            lstData = lstData.Select(c => { if (Objitem.SerialNumberTracking) { c.SerialNumberTracking = true; } if (Objitem.LotNumberTracking) { c.LotNumberTracking = true; } return c; }).ToList();

            ViewBag.IsSerialNumberTracking = Objitem.SerialNumberTracking;
            ViewBag.IsLotNumberTracking = Objitem.LotNumberTracking;
            ViewBag.IsDateCodeTracking = Objitem.DateCodeTracking;
            ViewBag.ItemNumber = Objitem.ToolName;

            return PartialView("_ToolLocationDetails", lstData);
        }
        public string CheckSerialCheckedOut(string SrNumber, Guid? ToolDetailsGUID)
        {
            List<ToolCheckInOutHistoryDTO> lstToolCheckInOutHistory = new List<ToolCheckInOutHistoryDTO>();
            ToolCheckInOutHistoryDAL objToolCheckInOutHistoryDAL = new ToolCheckInOutHistoryDAL(SessionHelper.EnterPriseDBName);
            lstToolCheckInOutHistory = objToolCheckInOutHistoryDAL.GetRecordByToolDetailGUID(ToolDetailsGUID ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID).Where(t => t.CheckedOutMQTYCurrent == 0 && t.CheckedOutQTYCurrent == 0).ToList();
            string Result = string.Empty;
            ToolCheckInOutHistoryDTO objToolCheckInOutHistoryDTO = lstToolCheckInOutHistory.Where(t => t.SerialNumber == SrNumber).FirstOrDefault();
            if (objToolCheckInOutHistoryDTO == null)
            {
                Result = "notexists";
            }
            return Result;
        }

        [HttpPost]
        public JsonResult CheckMoveInQuantity(Int64 BinID, string ItemGUID, double Qty)
        {
            string message = "";
            string status = "ok";
            try
            {
                //CommonController objCommonCtrl = new CommonController();
                CommonDAL objCommonCtrl = new CommonDAL(SessionHelper.EnterPriseDBName);
                ResponseMessage ResponseMsg = objCommonCtrl.CheckQuantityByLocation(BinID, Guid.Parse(ItemGUID), Qty, SessionHelper.RoomID, SessionHelper.CompanyID,SessionHelper.EnterPriceID,SessionHelper.UserID);

                if (ResponseMsg.IsSuccess)
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { Message = ResponseMsg.Message, Status = "fail" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, Status = "fail", KitDTO = new KitMasterDTO() }, JsonRequestBehavior.AllowGet);
            }


        }
        public string KitLineItemsDelete(string Ids)
        {
            try
            {
                //KitDetailController kitDetailCtrl = new KitDetailController();
                KitDetailDAL kitDetailCtrl = new KitDetailDAL(SessionHelper.EnterPriseDBName);
                kitDetailCtrl.DeleteRecords(Ids, SessionHelper.UserID, SessionHelper.CompanyID);

                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpPost]
        public JsonResult BuildNewToolKit(string objDTO)
        {
            try
            {
                JavaScriptSerializer s = new JavaScriptSerializer();
                BuildBreakKitDetailNew QLDetails = s.Deserialize<BuildBreakKitDetailNew>(objDTO);

                ToolMasterDAL objToolDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                ToolMasterDTO objToolDTO = objToolDAL.GetToolByGUIDPlain(Guid.Parse(QLDetails.KitGuid));
                objToolDTO.LastUpdatedBy = SessionHelper.UserID;
                objToolDTO.Updated = DateTimeUtility.DateTimeNow;
                objToolDTO.Quantity = objToolDTO.Quantity + Convert.ToDouble(QLDetails.Quantity);
                objToolDAL.Edit(objToolDTO);
                ToolDetailDAL objKitDetailctrl = new ToolDetailDAL(SessionHelper.EnterPriseDBName);
                List<ToolDetailDTO> lstKitDetailDTO = objKitDetailctrl.GetAllRecordsByKitGUID(Guid.Parse(QLDetails.KitGuid), SessionHelper.RoomID, SessionHelper.CompanyID, false, false, true).ToList();
                foreach (var item in lstKitDetailDTO)
                {
                    //if (item.ItemType != 4)
                    //{
                    item.AvailableItemsInWIP = item.AvailableItemsInWIP.GetValueOrDefault(0) - (item.QuantityPerKit * double.Parse(QLDetails.Quantity));
                    item.LastUpdatedBy = SessionHelper.UserID;
                    item.ReceivedOn = DateTimeUtility.DateTimeNow;
                    item.EditedFrom = "Web";
                    objKitDetailctrl.Edit(item);
                    //}
                }

                ToolMoveInOutDetailDTO objToolMoveInOutDetailDTO = new ToolMoveInOutDetailDTO()
                {
                    MoveInOut = "BuildKit",
                    ToolItemGUID = objToolDTO.GUID,

                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    CreatedBy = UserID,
                    LastUpdatedBy = UserID,
                    CompanyID = CompanyID,
                    Room = RoomID,

                    IsArchived = false,
                    IsDeleted = false,

                    Quantity = Convert.ToDouble(QLDetails.Quantity),
                    WhatWhereAction = "ToolMoveInOutDetailDAL-->BuildNewToolKit",
                };

                ToolMoveInOutDetailDAL KitMoveInOut = new ToolMoveInOutDetailDAL(SessionHelper.EnterPriseDBName);
                KitMoveInOut.Insert(objToolMoveInOutDetailDTO);

                // ToolMasterDTO objToolMasterDTO = objToolDAL.GetToolNameBySerial(objToolDTO.Serial, SessionHelper.RoomID, SessionHelper.CompanyID);

                ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO = new ToolAssetQuantityDetailDTO();
                objToolAssetQuantityDetailDTO.ToolGUID = objToolDTO.GUID;

                objToolAssetQuantityDetailDTO.AssetGUID = null;


                objToolAssetQuantityDetailDTO.ToolBinID = objToolDTO.ToolLocationDetailsID;
                objToolAssetQuantityDetailDTO.Quantity = Convert.ToDouble(QLDetails.Quantity);
                objToolAssetQuantityDetailDTO.RoomID = SessionHelper.RoomID;
                objToolAssetQuantityDetailDTO.CompanyID = SessionHelper.CompanyID;
                objToolAssetQuantityDetailDTO.Created = objToolDTO.Created;
                objToolAssetQuantityDetailDTO.Updated = objToolDTO.Updated ?? DateTimeUtility.DateTimeNow;
                objToolAssetQuantityDetailDTO.ReceivedOnWeb = objToolDTO.ReceivedOnWeb;
                objToolAssetQuantityDetailDTO.ReceivedOn = objToolDTO.ReceivedOn;
                objToolAssetQuantityDetailDTO.AddedFrom = "Web";
                objToolAssetQuantityDetailDTO.EditedFrom = "Web";
                objToolAssetQuantityDetailDTO.WhatWhereAction = "ToolKitController>>BuildNewToolKit";
                objToolAssetQuantityDetailDTO.ReceivedDate = null;
                objToolAssetQuantityDetailDTO.InitialQuantityWeb = Convert.ToDouble(QLDetails.Quantity);
                objToolAssetQuantityDetailDTO.InitialQuantityPDA = 0;
                objToolAssetQuantityDetailDTO.ExpirationDate = null;
                objToolAssetQuantityDetailDTO.EditedOnAction = "Tool Kit Update From Web Page. Tool Kit Quantity Build.";
                objToolAssetQuantityDetailDTO.CreatedBy = SessionHelper.UserID;
                objToolAssetQuantityDetailDTO.UpdatedBy = SessionHelper.UserID;
                objToolAssetQuantityDetailDTO.IsDeleted = false;
                objToolAssetQuantityDetailDTO.IsArchived = false;

                ToolAssetQuantityDetailDAL objToolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
                objToolAssetQuantityDetailDAL.Insert(objToolAssetQuantityDetailDTO, false, "AdjCredit", ReferalAction: "Kit Build");

                return Json(new { Message = ResCommon.RecordsSavedSuccessfully, Status = "OK" }, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                return Json(new { Status = "fail", Message = "fail", ReturnDTO = objDTO }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult BreakNewToolKit(MoveInOutQtyDetail objMoveQty)
        {
            try
            {
                string status = "fail";
                //KitMoveInOutDetailController kitMoveInOutCtrl = new KitMoveInOutDetailController();
                ToolMoveInOutDetailDAL kitMoveInOutCtrl = new ToolMoveInOutDetailDAL(SessionHelper.EnterPriseDBName);

                ToolMasterDAL objToolMasterDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                ResponseMessage RespMsg = kitMoveInOutCtrl.BreakToolKit(objMoveQty, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriceID, ResourceHelper.CurrentCult.Name);
                if (RespMsg.IsSuccess)
                {
                    status = "ok";

                    ToolMasterDTO objDTO = objToolMasterDAL.GetToolByGUIDFull(Guid.Parse(objMoveQty.ItemGUID));

                    ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO = new ToolAssetQuantityDetailDTO();
                    objToolAssetQuantityDetailDTO.ToolGUID = objDTO.GUID;

                    objToolAssetQuantityDetailDTO.AssetGUID = null;


                    objToolAssetQuantityDetailDTO.ToolBinID = objDTO.ToolLocationDetailsID;
                    objToolAssetQuantityDetailDTO.Quantity = objMoveQty.TotalQty;
                    objToolAssetQuantityDetailDTO.RoomID = SessionHelper.RoomID;
                    objToolAssetQuantityDetailDTO.CompanyID = SessionHelper.CompanyID;
                    objToolAssetQuantityDetailDTO.Created = objDTO.Created;
                    objToolAssetQuantityDetailDTO.Updated = objDTO.Updated ?? DateTimeUtility.DateTimeNow;
                    objToolAssetQuantityDetailDTO.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                    objToolAssetQuantityDetailDTO.ReceivedOn = objDTO.ReceivedOn;
                    objToolAssetQuantityDetailDTO.AddedFrom = "Web";
                    objToolAssetQuantityDetailDTO.EditedFrom = "Web";
                    objToolAssetQuantityDetailDTO.WhatWhereAction = "ToolKitController>>BreakNewToolKit";
                    objToolAssetQuantityDetailDTO.ReceivedDate = null;
                    objToolAssetQuantityDetailDTO.InitialQuantityWeb = objMoveQty.TotalQty;
                    objToolAssetQuantityDetailDTO.InitialQuantityPDA = 0;
                    objToolAssetQuantityDetailDTO.ExpirationDate = null;
                    objToolAssetQuantityDetailDTO.EditedOnAction = "Tool Kit Update From Web Page. Tool Kit Quantity Break.";
                    objToolAssetQuantityDetailDTO.CreatedBy = SessionHelper.UserID;
                    objToolAssetQuantityDetailDTO.UpdatedBy = SessionHelper.UserID;
                    objToolAssetQuantityDetailDTO.IsDeleted = false;
                    objToolAssetQuantityDetailDTO.IsArchived = false;

                    ToolAssetQuantityDetailDAL objToolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
                    objToolAssetQuantityDetailDAL.UpdateOrInsert(objToolAssetQuantityDetailDTO, objMoveQty.TotalQty);
                }
                return Json(new { Message = RespMsg.Message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult LoadLocationsofTool(Guid ToolGUID, int? AddCount)
        {
            ViewBag.ToolGUID = ToolGUID;
            LocationMasterDAL objLocationMasterDAL = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
            //ViewBag.DefaultLocationBag = objBinMasterDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(t => t.IsStagingLocation != true);

            List<LocationMasterDTO> lstBinReplanish = new List<LocationMasterDTO>();
            if (Session["ToolBinReplanish"] != null)
            {
                lstBinReplanish = (List<LocationMasterDTO>)Session["ToolBinReplanish"];
            }
            else
            {
                RoomDAL objRoomDal = new RoomDAL(SessionHelper.EnterPriseDBName);
                LocationMasterDTO objLocationmasterDTO = new LocationMasterDTO();

            }

            if (AddCount != null && AddCount > 0)
            {
                for (int i = 0; i < AddCount; i++)
                {
                    lstBinReplanish.Add(new LocationMasterDTO() { ID = 0, SessionSr = lstBinReplanish.Count + 1, ToolGUID = ToolGUID, Room = SessionHelper.RoomID, CompanyID = SessionHelper.CompanyID, LastUpdated = DateTimeUtility.DateTimeNow, LastUpdatedBy = SessionHelper.UserID, Created = DateTimeUtility.DateTimeNow, CreatedBy = SessionHelper.UserID, GUID = Guid.NewGuid() });
                }
            }

            return PartialView("_BinReplanishLocations", lstBinReplanish.OrderBy(t => t.Location).ToList());


        }
        public JsonResult SavetoSeesionBinReplanishSingle(Int64 ID, Int32 SessionSr, Guid GUID, Guid ToolGUID, Int64 BinID, string BinLocation, bool? IsDefault)
        {

            List<LocationMasterDTO> lstBinReplanish = null;
            if (Session["ToolBinReplanish"] != null)
            {
                lstBinReplanish = (List<LocationMasterDTO>)Session["ToolBinReplanish"];
            }
            else
            {
                lstBinReplanish = new List<LocationMasterDTO>();
            }




            if (ID > 0 && SessionSr == 0)
            {
                LocationMasterDTO objDTO = lstBinReplanish.Where(t => t.ID == ID).FirstOrDefault();
                if (objDTO != null)
                {
                    lstBinReplanish.Remove(lstBinReplanish.Where(t => t.ID == ID).FirstOrDefault());
                    objDTO.ID = ID;
                    //  objDTO.BinID = BinID;
                    objDTO.Location = BinLocation;

                    objDTO.ToolGUID = ToolGUID;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objDTO.GUID = GUID;
                    objDTO.IsDefault = IsDefault;
                    objDTO.Room = SessionHelper.RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.RoomName = SessionHelper.RoomName;
                    objDTO.UpdatedByName = SessionHelper.UserName;
                    objDTO.CreatedByName = SessionHelper.UserName;

                    //   if (eVMISensorID != 0)

                    lstBinReplanish.Add(objDTO);
                }
            }
            else
            {
                if (SessionSr > 0)
                {
                    LocationMasterDTO objDTO = lstBinReplanish.Where(t => t.SessionSr == SessionSr).FirstOrDefault();
                    if (objDTO != null)
                    {
                        lstBinReplanish.Remove(lstBinReplanish.Where(t => t.SessionSr == SessionSr).FirstOrDefault());
                        objDTO.ID = ID;
                        // objDTO.BinID = BinID;
                        objDTO.Location = BinLocation;
                        objDTO.IsDefault = IsDefault;
                        objDTO.ToolGUID = ToolGUID;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        objDTO.GUID = Guid.NewGuid();

                        objDTO.Room = SessionHelper.RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.RoomName = SessionHelper.RoomName;
                        objDTO.UpdatedByName = SessionHelper.UserName;
                        objDTO.CreatedByName = SessionHelper.UserName;

                        // if (eVMISensorID != 0)

                        lstBinReplanish.Add(objDTO);
                    }
                    else
                    {
                        objDTO = new LocationMasterDTO();
                        objDTO.ID = 0;
                        // objDTO.BinID = BinID;
                        objDTO.Location = BinLocation;
                        objDTO.IsDefault = IsDefault;
                        objDTO.ToolGUID = ToolGUID;
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

                        lstBinReplanish.Add(objDTO);
                    }
                }
                else
                {
                    LocationMasterDTO objDTO = new LocationMasterDTO();
                    objDTO.ID = 0;
                    //   objDTO.ID = BinID;
                    objDTO.Location = BinLocation;
                    objDTO.IsDefault = IsDefault;
                    objDTO.ToolGUID = ToolGUID;
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

                    lstBinReplanish.Add(objDTO);
                }
            }
            Session["ToolBinReplanish"] = lstBinReplanish;

            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SavetoSeesionBinReplanishSingleNew(Int64 ID, Int32 SessionSr, Guid GUID, Guid ToolGUID, Int64 BinID, string BinLocation, bool? IsDefault)
        {
            Guid newGUID = new Guid();
            List<LocationMasterDTO> lstBinReplanish = null;
            if (Session["ToolBinReplanish"] != null)
            {
                lstBinReplanish = (List<LocationMasterDTO>)Session["ToolBinReplanish"];
            }
            else
            {
                lstBinReplanish = new List<LocationMasterDTO>();
            }



            if (ID > 0 && SessionSr == 0)
            {
                LocationMasterDTO objDTO = lstBinReplanish.Where(t => t.ID == ID).FirstOrDefault();
                if (objDTO != null)
                {
                    lstBinReplanish.Remove(lstBinReplanish.Where(t => t.ID == ID).FirstOrDefault());
                    objDTO.ID = ID;
                    //  objDTO.BinID = BinID;
                    objDTO.Location = BinLocation;
                    objDTO.IsDefault = IsDefault;
                    objDTO.ToolGUID = ToolGUID;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objDTO.GUID = GUID;
                    newGUID = objDTO.GUID;
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
                    LocationMasterDTO objDTO = lstBinReplanish.Where(t => t.SessionSr == SessionSr).FirstOrDefault();
                    if (objDTO != null)
                    {
                        lstBinReplanish.Remove(lstBinReplanish.Where(t => t.SessionSr == SessionSr).FirstOrDefault());
                        objDTO.ID = ID;
                        // objDTO.BinID = BinID;
                        objDTO.Location = BinLocation;
                        objDTO.IsDefault = IsDefault;
                        objDTO.ToolGUID = ToolGUID;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        objDTO.GUID = GUID;
                        newGUID = objDTO.GUID;
                        objDTO.Room = SessionHelper.RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.RoomName = SessionHelper.RoomName;
                        objDTO.UpdatedByName = SessionHelper.UserName;
                        objDTO.CreatedByName = SessionHelper.UserName;

                        lstBinReplanish.Add(objDTO);
                    }
                    else
                    {
                        objDTO = new LocationMasterDTO();
                        objDTO.ID = 0;
                        // objDTO.BinID = BinID;
                        objDTO.Location = BinLocation;
                        objDTO.IsDefault = IsDefault;
                        objDTO.ToolGUID = ToolGUID;
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

                        lstBinReplanish.Add(objDTO);
                    }
                }
                else
                {
                    LocationMasterDTO objDTO = new LocationMasterDTO();
                    objDTO.ID = 0;
                    //   objDTO.ID = BinID;
                    objDTO.Location = BinLocation;
                    objDTO.IsDefault = IsDefault;
                    objDTO.ToolGUID = ToolGUID;
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

                    lstBinReplanish.Add(objDTO);
                }
            }
            Session["ToolBinReplanish"] = lstBinReplanish;

            return Json(new { status = "ok", newGUID = newGUID }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeletetoSeesionBinReplanishSingle(Int64 ID, Guid GUID, Guid ToolGUID, Int64 BinID)
        {
            List<LocationMasterDTO> lstBinReplanish = null;
            if (Session["ToolBinReplanish"] != null)
            {
                lstBinReplanish = (List<LocationMasterDTO>)Session["ToolBinReplanish"];
            }
            else
            {
                lstBinReplanish = new List<LocationMasterDTO>();
            }
            ///Delete the Records......
            if (ID > 0)
            {
                try
                {
                    lstBinReplanish.Remove(lstBinReplanish.Where(t => t.ID == ID).FirstOrDefault());
                    Session["ToolBinReplanish"] = lstBinReplanish;
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
                    Session["ToolBinReplanish"] = lstBinReplanish;
                }
                catch
                {
                    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "deleted" }, JsonRequestBehavior.AllowGet);
            }
        }
        /****************************************** Tool Kit methods end *******************************/
        private List<ReorderTypeAndKitCategoryNew> GetReorderTypeOrKitCategory(int Type)
        {
            List<ReorderTypeAndKitCategoryNew> lst = new List<ReorderTypeAndKitCategoryNew>();
            if (Type == 0)
            {
                lst.Add(new ReorderTypeAndKitCategoryNew() { ReOrderType = "Re-Order", typeValue = true });
                lst.Add(new ReorderTypeAndKitCategoryNew() { ReOrderType = "Transfer", typeValue = false });
            }
            else
            {
                lst.Add(new ReorderTypeAndKitCategoryNew() { KitCategory = "WIP", CategoryValue = 0 });
                lst.Add(new ReorderTypeAndKitCategoryNew() { KitCategory = "Direct", CategoryValue = 1 });
            }
            return lst;

        }
        public ActionResult KitToolLineItemAdded(List<ToolAssetPullMasterDTO> lstCheckOutRequestInfo)
        {
            List<ToolAssetPullMasterDTO> lstPullRequest = new List<ToolAssetPullMasterDTO>();
            foreach (ToolAssetPullMasterDTO objPullMasterDTO in lstCheckOutRequestInfo)
            {
                if (!lstPullRequest.Select(x => x.ToolGUID).Contains(objPullMasterDTO.ToolGUID))
                    lstPullRequest.Add(objPullMasterDTO);
            }

            ToolAssetCICOTransactionDAL objPullMasterDAL = new ToolAssetCICOTransactionDAL(SessionHelper.EnterPriseDBName);
            lstCheckOutRequestInfo = objPullMasterDAL.GetPullWithDetails(lstPullRequest, SessionHelper.RoomID, SessionHelper.CompanyID);
            return PartialView("KitToolLineItemAdded", lstCheckOutRequestInfo);
        }
        [HttpPost]
        public JsonResult AddToolKitSerialsAndLotsNew(List<ToolAssetPullInfo> objItemPullInfo)
        {
            ToolAssetCICOTransactionDAL objToolAssetCICOTransactionDAL = new ToolAssetCICOTransactionDAL(SessionHelper.EnterPriseDBName);
            List<ToolAssetPullInfo> oReturn = new List<ToolAssetPullInfo>();
            List<ToolAssetPullInfo> oReturnError = new List<ToolAssetPullInfo>();

            foreach (ToolAssetPullInfo item in objItemPullInfo)
            {


                if (item.lstToolPullDetails != null && item.lstToolPullDetails.Count > 0)
                {
                    item.lstToolPullDetails = item.lstToolPullDetails.Where(x => x.PullQuantity > 0).ToList();
                    if (item.lstToolPullDetails != null && item.lstToolPullDetails.Count > 0)
                    {

                        ToolMasterDAL objToolMasterDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);


                        ToolAssetPullInfo oItemPullInfo = item;
                        oItemPullInfo.CompanyId = SessionHelper.CompanyID;
                        oItemPullInfo.RoomId = SessionHelper.RoomID;
                        oItemPullInfo.CreatedBy = SessionHelper.UserID;
                        oItemPullInfo.LastUpdatedBy = SessionHelper.UserID;
                        oItemPullInfo.CanOverrideProjectLimits = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowOverrideProjectSpendLimits, eTurnsWeb.Helper.SessionHelper.PermissionType.Approval);
                        // oItemPullInfo.ValidateProjectSpendLimits = true;
                        oItemPullInfo.ErrorList = new List<PullToolAssetErrorInfo>();
                        oItemPullInfo = ValidateLotAndSerial(oItemPullInfo);

                        if (oItemPullInfo.ErrorList.Count == 0)
                        {
                            ToolMasterDAL objToolMasterDAL1 = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                            LocationMasterDTO objBINDTO = new LocationMasterDTO();
                            LocationMasterDAL objBINDAL = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
                            ToolAssetQuantityDetailDAL objItemLocationDetailsDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);


                            //if (objToolMasterDTO.Type != 4)
                            {



                                oItemPullInfo.EnterpriseId = SessionHelper.EnterPriceID;
                                {
                                    AddToolLineItem(oItemPullInfo);
                                }

                            }


                        }

                        oReturn.Add(oItemPullInfo);
                    }

                }
                else if (item.ToolGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {

                }
            }

            return Json(oReturn);


        }
        public ToolAssetPullInfo AddToolLineItem(ToolAssetPullInfo oToolAssetPullInfo)
        {
            if (oToolAssetPullInfo != null)
            {
                ToolMasterDAL objToolMasterDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);




                //string para: [{ "ToolID":"1592","Serial":"test serial11","ToolItemGUID":"cfe5a154-4f42-467c-81d0-04f1354cb803","QuantityPerKit":1,"ToolGUID":"fd9d970b-6a73-4f69-9ac6-6088c5d5b98f","ToolName":"test111","Type":""}]
                //oToolAssetPullInfo.lstToolPullDetails.ForEach(t =>
                //    AddToolToDetailTableKit()
                //);
                foreach (ToolLocationLotSerialDTO t in oToolAssetPullInfo.lstToolPullDetails)
                {
                    ToolMasterDTO obj = objToolMasterDAL.GetToolBySerialPlain(t.SerialNumber, SessionHelper.RoomID, SessionHelper.CompanyID);
                    string para = "[{ 'ToolID':'" + obj.ID + "','Serial':'" + obj.Serial + "','ToolItemGUID':'" + obj.GUID + "','QuantityPerKit':1,'ToolGUID':'" + oToolAssetPullInfo.PullGUID + "','ToolName':'" + obj.ToolName + "','Type':''}]";
                    AddToolToDetailTableKit(para);
                }

            }
            return oToolAssetPullInfo;

        }
        private ToolAssetPullInfo ValidateLotAndSerial(ToolAssetPullInfo objToolPullInfo)
        {
            ToolMasterDAL objToolMasterDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            //ToolMasterDTO objTool = objToolMasterDAL.GetRecordNew(0, SessionHelper.RoomID, SessionHelper.CompanyID, objToolPullInfo.ToolGUID ?? Guid.Empty);
            double? _PullCost = null;
            if (objToolPullInfo.RequisitionDetailsGUID != null && objToolPullInfo.RequisitionDetailsGUID != Guid.Empty)
            {
                _PullCost = objToolMasterDAL.GetToolPriceByRoomModuleSettings(SessionHelper.CompanyID, SessionHelper.RoomID, (long)eTurnsWeb.Helper.SessionHelper.ModuleList.Requisitions, (Guid)objToolPullInfo.ToolGUID, false);
            }
            else if (objToolPullInfo.WorkOrderDetailGUID != null && objToolPullInfo.WorkOrderDetailGUID != Guid.Empty)
            {
                _PullCost = objToolMasterDAL.GetToolPriceByRoomModuleSettings(SessionHelper.CompanyID, SessionHelper.RoomID, (long)eTurnsWeb.Helper.SessionHelper.ModuleList.WorkOrders, (Guid)objToolPullInfo.ToolGUID, false);
            }

            return objToolPullInfo;
        }
        public ActionResult OpenPopupToMoveInItem(List<ToolKitToolToMoveDTO> returnInfo)
        {


            return PartialView("_KitToolToMove", returnInfo);
        }
        public ActionResult OpenPopupToMoveOutItem(List<ToolKitToolToMoveDTO> returnInfo)
        {


            return PartialView("_KitToolToMoveOut", returnInfo);
        }
        public ActionResult OpenPopupToWrittenOff(List<ToolKitToolToMoveDTO> returnInfo)
        {
            ToolMasterDAL obj = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.ToolWrittenOffCategories = obj.GetAllToolWrittenOffCategories(SessionHelper.CompanyID, SessionHelper.RoomID);
            return PartialView("_KitToolToWrittenOff", returnInfo);
        }

        [HttpPost]
        public JsonResult PullToolSerialsAndLotsNew(List<ToolAssetPullInfo> objItemPullInfo)
        {
            ToolAssetCICOTransactionDAL objToolAssetCICOTransactionDAL = new ToolAssetCICOTransactionDAL(SessionHelper.EnterPriseDBName);
            List<ToolAssetPullInfo> oReturn = new List<ToolAssetPullInfo>();
            List<ToolAssetPullInfo> oReturnError = new List<ToolAssetPullInfo>();

            foreach (ToolAssetPullInfo item in objItemPullInfo)
            {
                //RequisitionDetailsDTO objReqDetailsDTO = null;
                //if (item.RequisitionDetailsGUID != null && item.RequisitionDetailsGUID != Guid.Empty)
                //{
                //    objReqDetailsDTO = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName).GetRecord((Guid)item.RequisitionDetailsGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                //    if (objReqDetailsDTO != null && objReqDetailsDTO.QuantityApproved.GetValueOrDefault(0) < (item.PullQuantity + objReqDetailsDTO.QuantityPulled.GetValueOrDefault(0)))
                //    {
                //        item.ErrorMessage = "Pull quantity is greater than approve quantinty.";
                //        oReturn.Add(item);
                //        continue;
                //    }
                //}

                if (item.lstToolPullDetails != null && item.lstToolPullDetails.Count > 0)
                {
                    item.lstToolPullDetails = item.lstToolPullDetails.Where(x => x.PullQuantity > 0).ToList();
                    if (item.lstToolPullDetails != null && item.lstToolPullDetails.Count > 0)
                    {
                        //-------------------------------------Get Item Master-------------------------------------
                        //
                        ToolMasterDAL objToolMasterDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);



                        //-----------------------------------------------------------------------------------------
                        //
                        ToolAssetPullInfo oItemPullInfo = item;
                        oItemPullInfo.CompanyId = SessionHelper.CompanyID;
                        oItemPullInfo.RoomId = SessionHelper.RoomID;
                        oItemPullInfo.CreatedBy = SessionHelper.UserID;
                        oItemPullInfo.LastUpdatedBy = SessionHelper.UserID;
                        oItemPullInfo.CanOverrideProjectLimits = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowOverrideProjectSpendLimits, eTurnsWeb.Helper.SessionHelper.PermissionType.Approval);
                        // oItemPullInfo.ValidateProjectSpendLimits = true;
                        oItemPullInfo.ErrorList = new List<PullToolAssetErrorInfo>();
                        oItemPullInfo = ValidateLotAndSerial(oItemPullInfo);

                        if (oItemPullInfo.ErrorList.Count == 0)
                        {
                            ToolMasterDAL objToolMasterDAL1 = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                            LocationMasterDTO objBINDTO = new LocationMasterDTO();
                            LocationMasterDAL objBINDAL = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
                            ToolAssetQuantityDetailDAL objItemLocationDetailsDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
                            // MaterialStagingPullDetailDAL objMSPDetailsDAL = new MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName);

                            //-----------Project Span---------------
                            //


                            //if (objToolMasterDTO.Type != 4)
                            {


                                {
                                    if (item.lstToolPullDetails != null && item.lstToolPullDetails.Count > 0)
                                    {
                                        //List<MaterialStagingPullDetailDTO> lstMSPDetailsTmp = new List<MaterialStagingPullDetailDTO>();
                                        //List<MaterialStagingPullDetailDTO> lstMSPDetails = new List<MaterialStagingPullDetailDTO>();
                                        List<ToolAssetQuantityDetailDTO> lstItemLocationDetailsTmp = null;
                                        double CurrentPullQuantity = 0;
                                        foreach (ToolLocationLotSerialDTO objItemLocationLotSerialDTO in item.lstToolPullDetails)
                                        {
                                            objItemLocationLotSerialDTO.Quantity = 0;
                                            objItemLocationLotSerialDTO.TobePulled = 0;

                                            string LotSerial = ((objItemLocationLotSerialDTO.LotNumber != null && objItemLocationLotSerialDTO.LotNumber.Trim() != "") ? objItemLocationLotSerialDTO.LotNumber.Trim()
                                                                    : ((objItemLocationLotSerialDTO.SerialNumber != null && objItemLocationLotSerialDTO.SerialNumber.Trim() != "") ? objItemLocationLotSerialDTO.SerialNumber.Trim() : ""));


                                            // else
                                            {
                                                lstItemLocationDetailsTmp = objItemLocationDetailsDAL.GetRecordsByBinNumberAndLotSerial(item.ToolGUID ?? Guid.Empty, objItemLocationLotSerialDTO.Location, LotSerial, SessionHelper.RoomID, SessionHelper.CompanyID);
                                                //------------------------------------------------------------------------
                                                //
                                                if (lstItemLocationDetailsTmp != null && lstItemLocationDetailsTmp.Count > 0)
                                                {
                                                    //TODO: Commented by CP for Pull issue, Wrong quantity pulled for normal item. on 2017-08-31
                                                    //double PullQty = objItemLocationLotSerialDTO.PullQuantity;
                                                    double PullQty = objItemLocationLotSerialDTO.TotalTobePulled;

                                                    foreach (ToolAssetQuantityDetailDTO objItemLocationDetailsDTO in lstItemLocationDetailsTmp)
                                                    {
                                                        if (objItemLocationDetailsDTO.Quantity != 0)
                                                        {
                                                            objItemLocationLotSerialDTO.Quantity = (objItemLocationLotSerialDTO.Quantity ?? 0) + (objItemLocationDetailsDTO.Quantity);
                                                            if (objItemLocationDetailsDTO.Quantity > 0 && PullQty > 0)
                                                            {
                                                                CurrentPullQuantity = (objItemLocationDetailsDTO.Quantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.Quantity);
                                                                objItemLocationLotSerialDTO.TobePulled = objItemLocationLotSerialDTO.TobePulled + CurrentPullQuantity;
                                                                PullQty = PullQty - (double)objItemLocationDetailsDTO.Quantity;
                                                                objItemLocationDetailsDTO.Quantity = objItemLocationDetailsDTO.Quantity - CurrentPullQuantity;
                                                            }
                                                        }
                                                    }

                                                }

                                            }
                                        }
                                    }
                                }

                                //--------------------------------------
                                //

                                oItemPullInfo.EnterpriseId = SessionHelper.EnterPriceID;
                                //if (oItemPullInfo.RequisitionDetailsGUID != null && oItemPullInfo.RequisitionDetailsGUID != Guid.Empty)
                                //{
                                //    oItemPullInfo = objPullMasterDAL.PullItemQuantity(oItemPullInfo, (long)eTurnsWeb.Helper.SessionHelper.ModuleList.Requisitions, ActionType1);
                                //}
                                //else if (oItemPullInfo.WorkOrderDetailGUID != null && oItemPullInfo.WorkOrderDetailGUID != Guid.Empty)
                                //{
                                //    oItemPullInfo = objPullMasterDAL.PullItemQuantity(oItemPullInfo, (long)eTurnsWeb.Helper.SessionHelper.ModuleList.WorkOrders, ActionType1);
                                //}
                                //else if (oItemPullInfo.IsStatgingLocationPull)
                                //{
                                //    oItemPullInfo = objPullMasterDAL.PullItemQuantity(oItemPullInfo, (long)eTurnsWeb.Helper.SessionHelper.ModuleList.Materialstaging, ActionType1);
                                //}
                                //else
                                {
                                    //   oItemPullInfo = objToolMasterDAL1.PullItemQuantity(oItemPullInfo, 0, ActionType1);
                                    MoveInAll(oItemPullInfo);
                                }

                            }


                        }

                        oReturn.Add(oItemPullInfo);
                    }
                    
                    List<UDFDTO> objUDFDTO = new List<UDFDTO>();
                    UDFDAL objUDFDAL = new UDFDAL(SessionHelper.EnterPriseDBName);
                    objUDFDTO = objUDFDAL.GetUDFsByUDFTableNamePlain("Checkout", SessionHelper.RoomID, SessionHelper.CompanyID);
                    
                    if (objUDFDTO != null && objUDFDTO.Count > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(item.ToolCheckoutUDF1))
                        {
                            if (objUDFDTO.Where(u => u.UDFColumnName == "UDF1" && u.IsDeleted == false).Any())
                            {
                                Int64 UDFId = objUDFDTO.Where(u => u.UDFColumnName == "UDF1").FirstOrDefault().ID;
                                if (objUDFDTO.Where(u => u.UDFColumnName == "UDF1").FirstOrDefault().UDFControlType == "Dropdown Editable")
                                {
                                    UDFController objUDFController = new UDFController();
                                    objUDFController.InsertUDFOption(UDFId, item.ToolCheckoutUDF1, "CheckOut", SessionHelper.EnterPriceID);
                                }
                            }

                        }
                        if (!string.IsNullOrWhiteSpace(item.ToolCheckoutUDF2))
                        {
                            if (objUDFDTO.Where(u => u.UDFColumnName == "UDF2" && u.IsDeleted == false).Any())
                            {
                                Int64 UDFId = objUDFDTO.Where(u => u.UDFColumnName == "UDF2").FirstOrDefault().ID;
                                if (objUDFDTO.Where(u => u.UDFColumnName == "UDF2").FirstOrDefault().UDFControlType == "Dropdown Editable")
                                {
                                    UDFController objUDFController = new UDFController();
                                    objUDFController.InsertUDFOption(UDFId, item.ToolCheckoutUDF2, "CheckOut", SessionHelper.EnterPriceID);
                                }
                            }

                        }
                        if (!string.IsNullOrWhiteSpace(item.ToolCheckoutUDF3))
                        {
                            if (objUDFDTO.Where(u => u.UDFColumnName == "UDF3" && u.IsDeleted == false).Any())
                            {
                                Int64 UDFId = objUDFDTO.Where(u => u.UDFColumnName == "UDF3").FirstOrDefault().ID;
                                if (objUDFDTO.Where(u => u.UDFColumnName == "UDF3").FirstOrDefault().UDFControlType == "Dropdown Editable")
                                {
                                    UDFController objUDFController = new UDFController();
                                    objUDFController.InsertUDFOption(UDFId, item.ToolCheckoutUDF3, "CheckOut", SessionHelper.EnterPriceID);
                                }
                            }

                        }
                        if (!string.IsNullOrWhiteSpace(item.ToolCheckoutUDF4))
                        {
                            if (objUDFDTO.Where(u => u.UDFColumnName == "UDF4" && u.IsDeleted == false).Any())
                            {
                                Int64 UDFId = objUDFDTO.Where(u => u.UDFColumnName == "UDF4").FirstOrDefault().ID;
                                if (objUDFDTO.Where(u => u.UDFColumnName == "UDF4").FirstOrDefault().UDFControlType == "Dropdown Editable")
                                {
                                    UDFController objUDFController = new UDFController();
                                    objUDFController.InsertUDFOption(UDFId, item.ToolCheckoutUDF4, "CheckOut", SessionHelper.EnterPriceID);
                                }
                            }

                        }
                        if (!string.IsNullOrWhiteSpace(item.ToolCheckoutUDF5))
                        {
                            if (objUDFDTO.Where(u => u.UDFColumnName == "UDF5" && u.IsDeleted == false).Any())
                            {
                                Int64 UDFId = objUDFDTO.Where(u => u.UDFColumnName == "UDF5").FirstOrDefault().ID;
                                if (objUDFDTO.Where(u => u.UDFColumnName == "UDF5").FirstOrDefault().UDFControlType == "Dropdown Editable")
                                {
                                    UDFController objUDFController = new UDFController();
                                    objUDFController.InsertUDFOption(UDFId, item.ToolCheckoutUDF5, "CheckOut", SessionHelper.EnterPriceID);
                                }
                            }

                        }
                    }
                }
                else if (item.ToolGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {

                }
            }

            return Json(oReturn);


        }
        public ToolAssetPullInfo MoveInAll(ToolAssetPullInfo oToolAssetPullInfo)
        {
            if (oToolAssetPullInfo != null)
            {
                ToolMasterDAL objToolMasterDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                RoomDAL objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
                int CheckOutQty = 0;
                CheckOutQty = Convert.ToInt32(oToolAssetPullInfo.lstToolPullDetails.Sum(t => t.TobePulled));
                bool IsSerialNumberTracking = oToolAssetPullInfo.lstToolPullDetails[0].SerialNumberTracking;
                List<ToolMoveInOutDetailDTO> lstToolMoveInOutDetailDTO = new List<ToolMoveInOutDetailDTO>();
                //oToolAssetPullInfo.lstToolPullDetails.ForEach(t =>
                foreach (ToolLocationLotSerialDTO t in oToolAssetPullInfo.lstToolPullDetails)
                {
                    ToolMoveInOutDetailDTO objToolMoveInOutDetailDTO = new ToolMoveInOutDetailDTO();
                    objToolMoveInOutDetailDTO.AddedFrom = "Web";
                    objToolMoveInOutDetailDTO.CompanyID = SessionHelper.CompanyID;
                    objToolMoveInOutDetailDTO.Created = DateTimeUtility.DateTimeNow;
                    objToolMoveInOutDetailDTO.CreatedBy = SessionHelper.UserID;
                    objToolMoveInOutDetailDTO.EditedFrom = "Web";
                    objToolMoveInOutDetailDTO.GUID = oToolAssetPullInfo.ToolDetailsGUID ?? Guid.Empty;
                    objToolMoveInOutDetailDTO.IsArchived = false;
                    objToolMoveInOutDetailDTO.IsDeleted = false;
                    objToolMoveInOutDetailDTO.LastUpdatedBy = SessionHelper.UserID;
                    objToolMoveInOutDetailDTO.MoveInOut = "IN";
                    objToolMoveInOutDetailDTO.MoveOutQuntity = t.PullQuantity;
                    if (IsSerialNumberTracking)
                    {
                        objToolMoveInOutDetailDTO.Quantity = 1;
                    }
                    else
                    {
                        objToolMoveInOutDetailDTO.Quantity = t.PullQuantity;
                    }
                    objToolMoveInOutDetailDTO.ReasonFromMove = "From Kit Page";
                    objToolMoveInOutDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objToolMoveInOutDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objToolMoveInOutDetailDTO.RefMoveInOutGUID = null;
                    objToolMoveInOutDetailDTO.Room = SessionHelper.RoomID;
                    objToolMoveInOutDetailDTO.ToolDetailGUID = oToolAssetPullInfo.ToolDetailsGUID;
                    objToolMoveInOutDetailDTO.ToolItemGUID = oToolAssetPullInfo.ToolGUID;
                    objToolMoveInOutDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                    objToolMoveInOutDetailDTO.WhatWhereAction = "ToolMoveInOutDetailDAL-->MoveINAll";
                    if (IsSerialNumberTracking)
                    {
                        objToolMoveInOutDetailDTO.SerialNumber = t.SerialNumber;
                    }
                    lstToolMoveInOutDetailDTO.Add(objToolMoveInOutDetailDTO);
                }


                if (!IsSerialNumberTracking)
                {
                    ToolQtyToMoveIn(lstToolMoveInOutDetailDTO);

                }
                if (IsSerialNumberTracking)
                {
                    JsonResult objjson = ToolQtyToMoveIn(lstToolMoveInOutDetailDTO);

                    JObject gridStateJS = new Newtonsoft.Json.Linq.JObject();


                    // jsonData = objDTO.JSONDATA;
                    /*////////CODE FOR UPDATE JSON STRING/////////*/
                    // JObject gridStaeJS = new JObject();
                    //gridStateJS = JObject.Parse(objjson.Data.ToString());
                    int errocnt = oToolAssetPullInfo.ErrorList.Count;
                    PullToolAssetErrorInfo objPullToolAssetErrorInfo = new PullToolAssetErrorInfo();
                    string Errormessage = JObject.Parse(new JavaScriptSerializer().Serialize(objjson.Data))["Message"].ToString();
                    if (Errormessage != "Success")
                    {
                        objPullToolAssetErrorInfo.ErrorMessage = Errormessage;
                        oToolAssetPullInfo.ErrorList.Add(objPullToolAssetErrorInfo);
                    }
                    //string msg = obj.Message;
                }
            }
            return oToolAssetPullInfo;

        }
        [HttpPost]
        public JsonResult PullToolSerialsAndLotsNewOut(List<ToolAssetPullInfo> objItemPullInfo)
        {
            ToolAssetCICOTransactionDAL objToolAssetCICOTransactionDAL = new ToolAssetCICOTransactionDAL(SessionHelper.EnterPriseDBName);
            List<ToolAssetPullInfo> oReturn = new List<ToolAssetPullInfo>();
            List<ToolAssetPullInfo> oReturnError = new List<ToolAssetPullInfo>();

            foreach (ToolAssetPullInfo item in objItemPullInfo)
            {


                if (item.lstToolPullDetails != null && item.lstToolPullDetails.Count > 0)
                {
                    item.lstToolPullDetails = item.lstToolPullDetails.Where(x => x.PullQuantity > 0).ToList();
                    if (item.lstToolPullDetails != null && item.lstToolPullDetails.Count > 0)
                    {

                        //ToolMasterDAL objToolMasterDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                        //ToolMasterDTO objToolMasterDTO = new ToolMasterDTO();
                        //objToolMasterDTO = objToolMasterDAL.GetRecordNew(0, SessionHelper.RoomID, SessionHelper.CompanyID, item.ToolGUID ?? Guid.Empty);


                        ToolAssetPullInfo oItemPullInfo = item;
                        oItemPullInfo.CompanyId = SessionHelper.CompanyID;
                        oItemPullInfo.RoomId = SessionHelper.RoomID;
                        oItemPullInfo.CreatedBy = SessionHelper.UserID;
                        oItemPullInfo.LastUpdatedBy = SessionHelper.UserID;
                        //oItemPullInfo.CanOverrideProjectLimits = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowOverrideProjectSpendLimits, eTurnsWeb.Helper.SessionHelper.PermissionType.Approval);
                        // oItemPullInfo.ValidateProjectSpendLimits = true;
                        oItemPullInfo.ErrorList = new List<PullToolAssetErrorInfo>();
                        //oItemPullInfo = ValidateLotAndSerial(oItemPullInfo);

                        //if (oItemPullInfo.ErrorList.Count == 0)
                        //{
                        //    ToolMasterDAL objToolMasterDAL1 = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                        //    LocationMasterDTO objBINDTO = new LocationMasterDTO();
                        //    LocationMasterDAL objBINDAL = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
                        //    ToolAssetQuantityDetailDAL objItemLocationDetailsDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);


                        //    if (objToolMasterDTO.Type != 4)
                        //    {


                        //        {
                        //            if (item.lstToolPullDetails != null && item.lstToolPullDetails.Count > 0)
                        //            {

                        //                List<ToolAssetQuantityDetailDTO> lstItemLocationDetailsTmp = null;
                        //                double CurrentPullQuantity = 0;
                        //                foreach (ToolLocationLotSerialDTO objItemLocationLotSerialDTO in item.lstToolPullDetails)
                        //                {
                        //                    objItemLocationLotSerialDTO.Quantity = 0;
                        //                    objItemLocationLotSerialDTO.TobePulled = 0;

                        //                    string LotSerial = ((objItemLocationLotSerialDTO.LotNumber != null && objItemLocationLotSerialDTO.LotNumber.Trim() != "") ? objItemLocationLotSerialDTO.LotNumber.Trim()
                        //                                            : ((objItemLocationLotSerialDTO.SerialNumber != null && objItemLocationLotSerialDTO.SerialNumber.Trim() != "") ? objItemLocationLotSerialDTO.SerialNumber.Trim() : ""));



                        //                    {
                        //                        lstItemLocationDetailsTmp = objItemLocationDetailsDAL.GetRecordsByBinNumberAndLotSerial(item.ToolGUID ?? Guid.Empty, objItemLocationLotSerialDTO.Location, LotSerial, SessionHelper.RoomID, SessionHelper.CompanyID);

                        //                        if (lstItemLocationDetailsTmp != null && lstItemLocationDetailsTmp.Count > 0)
                        //                        {

                        //                            double PullQty = objItemLocationLotSerialDTO.TotalTobePulled;

                        //                            foreach (ToolAssetQuantityDetailDTO objItemLocationDetailsDTO in lstItemLocationDetailsTmp)
                        //                            {
                        //                                if (objItemLocationDetailsDTO.Quantity != null && objItemLocationDetailsDTO.Quantity != 0)
                        //                                {
                        //                                    objItemLocationLotSerialDTO.Quantity = (objItemLocationLotSerialDTO.Quantity ?? 0) + (objItemLocationDetailsDTO.Quantity);
                        //                                    if (objItemLocationDetailsDTO.Quantity > 0 && PullQty > 0)
                        //                                    {
                        //                                        CurrentPullQuantity = (objItemLocationDetailsDTO.Quantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.Quantity);
                        //                                        objItemLocationLotSerialDTO.TobePulled = objItemLocationLotSerialDTO.TobePulled + CurrentPullQuantity;
                        //                                        PullQty = PullQty - (double)objItemLocationDetailsDTO.Quantity;
                        //                                        objItemLocationDetailsDTO.Quantity = objItemLocationDetailsDTO.Quantity - CurrentPullQuantity;
                        //                                    }
                        //                                }
                        //                            }

                        //                        }

                        //                    }
                        //                }
                        //            }
                        //        }


                        //        string ActionType1 = "Check Out";

                        oItemPullInfo.EnterpriseId = SessionHelper.EnterPriceID;

                        //        {

                        MoveOutAll(oItemPullInfo);



                        oReturn.Add(oItemPullInfo);
                    }
                }
                List<UDFDTO> objUDFDTO = new List<UDFDTO>();
                UDFDAL objUDFDAL = new UDFDAL(SessionHelper.EnterPriseDBName);
                objUDFDTO = objUDFDAL.GetUDFsByUDFTableNamePlain("Checkout", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (objUDFDTO != null && objUDFDTO.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(item.ToolCheckoutUDF1))
                    {
                        if (objUDFDTO.Where(u => u.UDFColumnName == "UDF1" && u.IsDeleted == false).Any())
                        {
                            Int64 UDFId = objUDFDTO.Where(u => u.UDFColumnName == "UDF1").FirstOrDefault().ID;
                            if (objUDFDTO.Where(u => u.UDFColumnName == "UDF1").FirstOrDefault().UDFControlType == "Dropdown Editable")
                            {
                                UDFController objUDFController = new UDFController();
                                objUDFController.InsertUDFOption(UDFId, item.ToolCheckoutUDF1, "CheckOut", SessionHelper.EnterPriceID);
                            }
                        }

                    }
                    if (!string.IsNullOrWhiteSpace(item.ToolCheckoutUDF2))
                    {
                        if (objUDFDTO.Where(u => u.UDFColumnName == "UDF2" && u.IsDeleted == false).Any())
                        {
                            Int64 UDFId = objUDFDTO.Where(u => u.UDFColumnName == "UDF2").FirstOrDefault().ID;
                            if (objUDFDTO.Where(u => u.UDFColumnName == "UDF2").FirstOrDefault().UDFControlType == "Dropdown Editable")
                            {
                                UDFController objUDFController = new UDFController();
                                objUDFController.InsertUDFOption(UDFId, item.ToolCheckoutUDF2, "CheckOut", SessionHelper.EnterPriceID);
                            }
                        }

                    }
                    if (!string.IsNullOrWhiteSpace(item.ToolCheckoutUDF3))
                    {
                        if (objUDFDTO.Where(u => u.UDFColumnName == "UDF3" && u.IsDeleted == false).Any())
                        {
                            Int64 UDFId = objUDFDTO.Where(u => u.UDFColumnName == "UDF3").FirstOrDefault().ID;
                            if (objUDFDTO.Where(u => u.UDFColumnName == "UDF3").FirstOrDefault().UDFControlType == "Dropdown Editable")
                            {
                                UDFController objUDFController = new UDFController();
                                objUDFController.InsertUDFOption(UDFId, item.ToolCheckoutUDF3, "CheckOut", SessionHelper.EnterPriceID);
                            }
                        }

                    }
                    if (!string.IsNullOrWhiteSpace(item.ToolCheckoutUDF4))
                    {
                        if (objUDFDTO.Where(u => u.UDFColumnName == "UDF4" && u.IsDeleted == false).Any())
                        {
                            Int64 UDFId = objUDFDTO.Where(u => u.UDFColumnName == "UDF4").FirstOrDefault().ID;
                            if (objUDFDTO.Where(u => u.UDFColumnName == "UDF4").FirstOrDefault().UDFControlType == "Dropdown Editable")
                            {
                                UDFController objUDFController = new UDFController();
                                objUDFController.InsertUDFOption(UDFId, item.ToolCheckoutUDF4, "CheckOut", SessionHelper.EnterPriceID);
                            }
                        }

                    }
                    if (!string.IsNullOrWhiteSpace(item.ToolCheckoutUDF5))
                    {
                        if (objUDFDTO.Where(u => u.UDFColumnName == "UDF5" && u.IsDeleted == false).Any())
                        {
                            Int64 UDFId = objUDFDTO.Where(u => u.UDFColumnName == "UDF5").FirstOrDefault().ID;
                            if (objUDFDTO.Where(u => u.UDFColumnName == "UDF5").FirstOrDefault().UDFControlType == "Dropdown Editable")
                            {
                                UDFController objUDFController = new UDFController();
                                objUDFController.InsertUDFOption(UDFId, item.ToolCheckoutUDF5, "CheckOut", SessionHelper.EnterPriceID);
                            }
                        }

                    }
                }

                else if (item.ToolGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {

                }
            }

            return Json(oReturn);


        }
        public ToolAssetPullInfo MoveOutAll(ToolAssetPullInfo oToolAssetPullInfo)
        {
            if (oToolAssetPullInfo != null)
            {
                ToolMasterDAL objToolMasterDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                RoomDAL objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
                int CheckOutQty = 0;
                CheckOutQty = Convert.ToInt32(oToolAssetPullInfo.lstToolPullDetails.Sum(t => t.TobePulled));
                bool IsSerialNumberTracking = oToolAssetPullInfo.lstToolPullDetails[0].SerialNumberTracking;
                List<ToolMoveInOutDetailDTO> lstToolMoveInOutDetailDTO = new List<ToolMoveInOutDetailDTO>();
                //oToolAssetPullInfo.lstToolPullDetails.ForEach(t =>
                foreach (ToolLocationLotSerialDTO t in oToolAssetPullInfo.lstToolPullDetails)
                {
                    ToolMoveInOutDetailDTO objToolMoveInOutDetailDTO = new ToolMoveInOutDetailDTO();
                    objToolMoveInOutDetailDTO.AddedFrom = "Web";
                    objToolMoveInOutDetailDTO.CompanyID = SessionHelper.CompanyID;
                    objToolMoveInOutDetailDTO.Created = DateTimeUtility.DateTimeNow;
                    objToolMoveInOutDetailDTO.CreatedBy = SessionHelper.UserID;
                    objToolMoveInOutDetailDTO.EditedFrom = "Web";
                    objToolMoveInOutDetailDTO.GUID = oToolAssetPullInfo.ToolDetailsGUID ?? Guid.Empty;
                    objToolMoveInOutDetailDTO.IsArchived = false;
                    objToolMoveInOutDetailDTO.IsDeleted = false;
                    objToolMoveInOutDetailDTO.LastUpdatedBy = SessionHelper.UserID;
                    objToolMoveInOutDetailDTO.MoveInOut = "OUT";
                    objToolMoveInOutDetailDTO.MoveOutQuntity = t.PullQuantity;
                    if (IsSerialNumberTracking)
                    {
                        objToolMoveInOutDetailDTO.Quantity = 1;
                    }
                    else
                    {
                        objToolMoveInOutDetailDTO.Quantity = t.PullQuantity;
                    }
                    objToolMoveInOutDetailDTO.ReasonFromMove = "From Kit Page";
                    objToolMoveInOutDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objToolMoveInOutDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objToolMoveInOutDetailDTO.RefMoveInOutGUID = null;
                    objToolMoveInOutDetailDTO.Room = SessionHelper.RoomID;
                    objToolMoveInOutDetailDTO.ToolDetailGUID = oToolAssetPullInfo.ToolDetailsGUID;
                    objToolMoveInOutDetailDTO.ToolItemGUID = oToolAssetPullInfo.ToolGUID;
                    objToolMoveInOutDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                    objToolMoveInOutDetailDTO.WhatWhereAction = "ToolMoveInOutDetailDAL-->MoveINAll";
                    if (IsSerialNumberTracking)
                    {
                        objToolMoveInOutDetailDTO.SerialNumber = t.SerialNumber;
                    }
                    lstToolMoveInOutDetailDTO.Add(objToolMoveInOutDetailDTO);
                }


                if (!IsSerialNumberTracking)
                {
                    ToolQtyToMoveIn(lstToolMoveInOutDetailDTO);

                }
                if (IsSerialNumberTracking)
                {
                    JsonResult objjson = ToolQtyToMoveIn(lstToolMoveInOutDetailDTO);

                    JObject gridStateJS = new Newtonsoft.Json.Linq.JObject();



                    int errocnt = oToolAssetPullInfo.ErrorList.Count;
                    PullToolAssetErrorInfo objPullToolAssetErrorInfo = new PullToolAssetErrorInfo();
                    string Errormessage = JObject.Parse(new JavaScriptSerializer().Serialize(objjson.Data))["Message"].ToString();
                    if (Errormessage != "Success")
                    {
                        objPullToolAssetErrorInfo.ErrorMessage = Errormessage;
                        oToolAssetPullInfo.ErrorList.Add(objPullToolAssetErrorInfo);
                    }
                    //string msg = obj.Message;
                }
            }
            return oToolAssetPullInfo;

        }
    }

    public class BuildBreakKitDetailNew
    {
        public string LotNumber { get; set; }
        public string ExpirationDate { get; set; }
        public string Quantity { get; set; }
        public string KitGuid { get; set; }
        public string LocationID { get; set; }
        public string ReceiveDate { get; set; }
        public string Cost { get; set; }
        public bool Consignment { get; set; }
    }

    public class ReorderTypeAndKitCategoryNew
    {
        public string ReOrderType { get; set; }
        public bool typeValue { get; set; }
        public string KitCategory { get; set; }
        public int CategoryValue { get; set; }
    }
}
