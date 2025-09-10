using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsWeb.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using System.Data;
using System.Web;

namespace eTurnsWeb.Controllers
{
    [AuthorizeHelper]
    public partial class ToolController : eTurnsControllerBase
    {
        Int64 RoomID = SessionHelper.RoomID;
        Int64 CompanyID = SessionHelper.CompanyID;
        string QtyFromate = "N2";
        Int64 UserID = SessionHelper.UserID;

        #region "Tool"
        public ActionResult ToolList()
        {
            return View();
        }
        public ActionResult ToolCreate()
        {
            UDFController objUDFDAL = new UDFController();
            string NewNumber = new AutoSequenceDAL(SessionHelper.EnterPriseDBName).GetNextAutoNumberByModule("NextToolNo", SessionHelper.RoomID, SessionHelper.CompanyID);

            ToolMasterDTO objDTO = new ToolMasterDTO();
            objDTO.ID = 0;

            objDTO.ToolName = NewNumber;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.RoomName = SessionHelper.RoomName;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.GUID = Guid.NewGuid();
            objDTO.IsOnlyFromItemUI = true;
            objDTO.Type = 1;

            ToolCategoryMasterDAL objToolCategory = new ToolCategoryMasterDAL(SessionHelper.EnterPriseDBName);
            List<ToolCategoryMasterDTO> lstToolCategory = objToolCategory.GetToolCategoryByRoomIDPlain(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            lstToolCategory.Insert(0, new ToolCategoryMasterDTO() { ID = 0, ToolCategory = ResCategoryMaster.SelectCategory });
            ViewBag.ToolCategoryList = lstToolCategory;

            LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(SessionHelper.EnterPriseDBName);
            List<LocationMasterDTO> lstLocation = objLocationCntrl.GetLocationByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            lstLocation = lstLocation.Where(t => (!string.IsNullOrWhiteSpace(t.Location))).ToList();
            lstLocation.Insert(0, new LocationMasterDTO() { ID = 0, Location = ResCommon.MsgSelectLocation });
            ViewBag.LocationList = lstLocation;

            //List<LocationMasterDTO> lstAllLocation = lstLocation.ToList();
            ViewBag.DefaultLocationBag = null;
            Session["ToolBinReplanish"] = null;

            TechnicialMasterDAL objTechnicianCntrl = new eTurns.DAL.TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
            List<TechnicianMasterDTO> lstTechnician = objTechnicianCntrl.GetTechnicianByRoomIDPlain(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            lstTechnician.Insert(0, new TechnicianMasterDTO() { GUID = Guid.Empty, Technician = Convert.ToString(eTurns.DTO.Resources.ResCommon.SelectTechnicianText), TechnicianCode = string.Empty });
            ViewBag.TechnicianList = lstTechnician;

            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("ToolMaster");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            ViewBag.GropOfItemsBag = GetGroupOfItems();
            if (!string.IsNullOrWhiteSpace(objDTO.ToolImageExternalURL))
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(objDTO.ToolImageExternalURL);
                    request.Method = "GET";
                    try
                    {
                        WebResponse response = request.GetResponse();
                        if (response.ContentType.ToString().ToLower().IndexOf("image") >= 0)
                        {

                        }
                        else
                        {
                            objDTO.ToolImageExternalURL = string.Empty;
                        }
                    }
                    catch
                    {
                        objDTO.ToolImageExternalURL = string.Empty;
                    }
                }
                catch
                {
                    objDTO.ToolImageExternalURL = string.Empty;
                }
            }
            objDTO.ImageType = "ExternalImage";
            return PartialView("_CreateTool", objDTO);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult ToolSave(ToolMasterDTO objDTO)
        {
            Int64 TempToolID = 0;
            string message = "";
            string status = "";
            LocationMasterDAL objLocationMasterDAL = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
            ToolMasterDAL obj = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            ToolLocationDetailsDTO objToolLocationDetailsDTO = null;
            ToolLocationDetailsDAL objToolLocationDetailsDAL = new ToolLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            if (string.IsNullOrEmpty(objDTO.ToolName))
            {
                message = string.Format(ResMessage.Required, ResToolMaster.ToolName);// "Tool is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }

            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.CompanyID = SessionHelper.CompanyID;
            bool AllowToolOrdering = SessionHelper.AllowToolOrdering;
            CommonDAL objCommon = new CommonDAL(SessionHelper.EnterPriseDBName);
            if (objDTO.ID == 0)
            {
                if (!string.IsNullOrWhiteSpace(objDTO.ToolTypeTracking) && objDTO.ToolTypeTracking.Contains("2"))
                {
                    objDTO.SerialNumberTracking = true;
                    objDTO.IsGroupOfItems = 1;
                }
                objDTO.CreatedBy = SessionHelper.UserID;


                //string 
                string strOK = "ok";
                if (AllowToolOrdering)
                {
                    //if (objDTO.SerialNumberTracking)
                    //{
                    //    strOK = obj.ToolNameDuplicateCheck((objDTO.ToolName ?? string.Empty).Trim(), objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    //}
                    //else
                    {
                        strOK = obj.ToolNameSerialDuplicateCheck((objDTO.ToolName ?? string.Empty).Trim() + "$" + (objDTO.Serial ?? string.Empty).Trim(), objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    }
                }
                else
                {
                    strOK = objCommon.DuplicateCheck(objDTO.Serial ?? string.Empty, "add", objDTO.ID, "ToolMaster", "Serial", SessionHelper.RoomID, SessionHelper.CompanyID);
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
                    //objDTO.GUID = Guid.NewGuid();
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
                                        LocationDTO = objLocationCntrl.GetLocationOrInsert(lstAllLocation[i].Location, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "Toolcontroller>>ToolSave");
                                        objDTO.LocationID = LocationDTO.ID;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    TempToolID = obj.Insert(objDTO, AllowToolOrdering);

                    ToolMasterDTO objToolMasterDTO = obj.GetToolByIDPlain(TempToolID);
                    //if (objDTO.LocationID.GetValueOrDefault(0) <= 0)
                    //{
                    //    objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(objToolMasterDTO.GUID, string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "Toolcontroller>>ToolSave");
                    //}
                    LocationMasterDTO objLocationMasterDTO = null;
                    //if (objDTO.LocationID.GetValueOrDefault(0) > 0)
                    //{
                    //    LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(SessionHelper.EnterPriseDBName);
                    //    List<LocationMasterDTO> lstLocation = objLocationCntrl.GetLocationForRoomWise(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
                    //    objLocationMasterDTO = lstLocation.Where(i => i.ID == objDTO.LocationID).FirstOrDefault();
                    //    objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(objDTO.GUID, objLocationMasterDTO.Location, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "Toolcontroller>>ToolSave");

                    //}
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
                                    objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(objDTO.GUID, lstAllLocation[i].Location, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "Toolcontroller>>ToolSave", true);
                                    objLocationMasterDTO = objLocationCntrl.GetLocationByNamePlain(lstAllLocation[i].Location, SessionHelper.RoomID, SessionHelper.CompanyID);

                                    break;
                                }
                                else
                                {
                                    LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(SessionHelper.EnterPriseDBName);
                                    objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(objDTO.GUID, lstAllLocation[i].Location, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "Toolcontroller>>ToolSave");

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
                        objToolAssetQuantityDetailDTO.WhatWhereAction = "Toolcontroller>>ToolSave";
                        objToolAssetQuantityDetailDTO.ReceivedDate = null;
                        objToolAssetQuantityDetailDTO.InitialQuantityWeb = objDTO.Quantity;
                        objToolAssetQuantityDetailDTO.InitialQuantityPDA = 0;
                        objToolAssetQuantityDetailDTO.ExpirationDate = null;
                        objToolAssetQuantityDetailDTO.EditedOnAction = "Tool Created From Web Page. insert Entry of Tool.";
                        objToolAssetQuantityDetailDTO.CreatedBy = SessionHelper.UserID;
                        objToolAssetQuantityDetailDTO.UpdatedBy = SessionHelper.UserID;
                        objToolAssetQuantityDetailDTO.IsDeleted = false;
                        objToolAssetQuantityDetailDTO.IsArchived = false;

                        ToolAssetQuantityDetailDAL objToolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
                        objToolAssetQuantityDetailDAL.Insert(objToolAssetQuantityDetailDTO, false, "AdjCredit", ReferalAction: "Initial Tool Create");
                    }
                    if (TempToolID > 0)
                    {
                        if (Session["ToolBinReplanish"] != null)
                        {
                            List<LocationMasterDTO> lstAllLocation = (List<LocationMasterDTO>)Session["ToolBinReplanish"];

                            if (lstAllLocation != null && lstAllLocation.Count() > 0 && lstAllLocation.Any())
                            {
                                for (int i = 0; i < lstAllLocation.Count(); i++)
                                {
                                    objLocationMasterDAL.GetToolLocation(objToolMasterDTO.GUID, lstAllLocation[i].Location, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "Toolcontroller>>ToolSave", lstAllLocation[i].IsDefault);
                                }
                            }
                        }

                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
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
                string strOK = "ok";
                if (!string.IsNullOrWhiteSpace(objDTO.ToolTypeTracking) && objDTO.ToolTypeTracking.Contains("2"))
                {
                    objDTO.SerialNumberTracking = true;
                    objDTO.IsGroupOfItems = 1;
                }
                if (AllowToolOrdering)
                {
                    //if (objDTO.SerialNumberTracking)
                    //{
                    //    strOK = obj.ToolNameDuplicateCheck((objDTO.ToolName ?? string.Empty).Trim(), objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    //}
                    //else
                    {
                        strOK = obj.ToolNameSerialDuplicateCheck((objDTO.ToolName ?? string.Empty).Trim() + "$" + (objDTO.Serial ?? string.Empty).Trim(), objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    }
                }
                else
                {
                    strOK = objCommon.DuplicateCheck(objDTO.Serial ?? string.Empty, "add", objDTO.ID, "ToolMaster", "Serial", SessionHelper.RoomID, SessionHelper.CompanyID);
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
                                        LocationDTO = objLocationCntrl.GetLocationOrInsert(lstAllLocation[i].Location, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "Toolcontroller>>ToolSave");
                                        objDTO.LocationID = LocationDTO.ID;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (obj.Edit(objDTO))
                    {
                        //if (objDTO.LocationID.GetValueOrDefault(0) <= 0)
                        //{
                        //    objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(objDTO.GUID, string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "Toolcontroller>>ToolSave");
                        //}
                        LocationMasterDTO objLocationMasterDTO = null;
                        //if (objDTO.LocationID.GetValueOrDefault(0) > 0)
                        //{
                        //    LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(SessionHelper.EnterPriseDBName);
                        //    List<LocationMasterDTO> lstLocation = objLocationCntrl.GetLocationForRoomWise(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
                        //    objLocationMasterDTO = lstLocation.Where(i => i.ID == objDTO.LocationID).FirstOrDefault();
                        //    objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(objDTO.GUID, objLocationMasterDTO.Location, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "Toolcontroller>>ToolSave");

                        //}
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

                                        objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(objDTO.GUID, lstAllLocation[i].Location, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "Toolcontroller>>ToolSave", true);
                                        objLocationMasterDTO = objLocationCntrl.GetLocationByNamePlain(lstAllLocation[i].Location, SessionHelper.RoomID, SessionHelper.CompanyID);
                                        break;
                                    }
                                    else
                                    {
                                        LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(SessionHelper.EnterPriseDBName);
                                        objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(objDTO.GUID, lstAllLocation[i].Location, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "Toolcontroller>>ToolSave");
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
                            objLocationMasterDAL.UpdateToolWithDefault(objDTO.GUID, objLocationMasterDTO.ID, objLocationMasterDTO.GUID);
                        }
                        //if (objDTO.Quantity > 0)
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
                            objToolAssetQuantityDetailDTO.WhatWhereAction = "Toolcontroller>>ToolSave";
                            objToolAssetQuantityDetailDTO.ReceivedDate = null;
                            objToolAssetQuantityDetailDTO.InitialQuantityWeb = objDTO.Quantity;
                            objToolAssetQuantityDetailDTO.InitialQuantityPDA = 0;
                            objToolAssetQuantityDetailDTO.ExpirationDate = null;
                            objToolAssetQuantityDetailDTO.EditedOnAction = "Tool Update From Web Page. Update Entry of Tool.";
                            objToolAssetQuantityDetailDTO.CreatedBy = SessionHelper.UserID;
                            objToolAssetQuantityDetailDTO.UpdatedBy = SessionHelper.UserID;
                            objToolAssetQuantityDetailDTO.IsDeleted = false;
                            objToolAssetQuantityDetailDTO.IsArchived = false;

                            ToolAssetQuantityDetailDAL objToolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
                            objToolAssetQuantityDetailDAL.UpdateOrInsert(objToolAssetQuantityDetailDTO, null, false, "AdjCredit", ReferalAction: "Tool Edit");

                        }
                        TempToolID = objDTO.ID;
                        if (Session["ToolBinReplanish"] != null)
                        {
                            List<LocationMasterDTO> lstAllLocation = (List<LocationMasterDTO>)Session["ToolBinReplanish"];

                            if (lstAllLocation != null && lstAllLocation.Count() > 0 && lstAllLocation.Any())
                            {
                                for (int i = 0; i < lstAllLocation.Count(); i++)
                                {
                                    if (!string.IsNullOrWhiteSpace(lstAllLocation[i].Location))
                                    {
                                        objLocationMasterDTO = objLocationMasterDAL.GetToolLocation(objDTO.GUID, lstAllLocation[i].Location, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "Toolcontroller>>ToolSave", lstAllLocation[i].IsDefault);
                                        lstAllLocation[i].GUID = objLocationMasterDTO.GUID;
                                    }
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

                        message = ResMessage.SaveMessage;
                        status = "ok";
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

            objDTO.IsOnlyFromItemUI = true;

            if ((objDTO.Type ?? 1) == 1)
            {
                ToolCategoryMasterDAL objToolCategory = new ToolCategoryMasterDAL(SessionHelper.EnterPriseDBName);
                List<ToolCategoryMasterDTO> lstToolCategory = objToolCategory.GetToolCategoryByRoomIDPlain(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
                lstToolCategory.Insert(0, new ToolCategoryMasterDTO() { ID = 0, ToolCategory = ResCategoryMaster.SelectCategory });
                ViewBag.ToolCategoryList = lstToolCategory;

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
            }
            else
            {
                objDTO.KitToolQuantity = objDTO.Quantity;
                objDTO.KitToolName = objDTO.ToolName;
                objDTO.KitToolSerial = objDTO.Serial;
                ToolDetailDAL objToolDetailDAL = new ToolDetailDAL(SessionHelper.EnterPriseDBName);
                Session["ToolKitDetail"] = objToolDetailDAL.GetAllRecordsByKitGUID(objDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, true).ToList();

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
            }
            UDFController objUDFDAL = new UDFController();
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("ToolMaster");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;

            ViewBag.GropOfItemsBag = GetGroupOfItems();
            ViewBag.ToolWrittenOffCategories = obj.GetAllToolWrittenOffCategories(SessionHelper.CompanyID,SessionHelper.RoomID);

            if (objDTO.ToolTypeTrackingStr != "General")
            {
                ToolAssetQuantityDetailDAL toolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
                ViewBag.ToolSerialForWrittenOff = toolAssetQuantityDetailDAL.GetToolSerialForWrittenOff(objDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID);
            }

            if (!string.IsNullOrWhiteSpace(objDTO.ToolImageExternalURL))
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(objDTO.ToolImageExternalURL);
                    request.Method = "GET";
                    try
                    {
                        WebResponse response = request.GetResponse();
                        if (response.ContentType.ToString().ToLower().IndexOf("image") >= 0)
                        {

                        }
                        else
                        {
                            objDTO.ToolImageExternalURL = string.Empty;
                        }
                    }
                    catch
                    {
                        objDTO.ToolImageExternalURL = string.Empty;
                    }
                }
                catch
                {
                    objDTO.ToolImageExternalURL = string.Empty;
                }
            }

            if ((objDTO.Type ?? 1) == 1)
            {
                return PartialView("_CreateTool", objDTO);
            }
            else
            {
                return PartialView("~/Views/ToolKit/_KitToolCreate.cshtml", objDTO);
            }
        }
        public ActionResult ToolHistory()
        {
            return PartialView("~/Views/Tool/_ToolHistory.cshtml");
        }

        #region Ajax Data Provider
        public ActionResult ToolListAjax(JQueryDataTableParamModel param)
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
            var DataFromDB = obj.GetPagedToolsNew(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, null, null, RoomDateFormat, CurrentTimeZone, Type: ToolType, CalledPage: "FromNewToolPage");

            Session["ToolBinReplanish"] = null;
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
        #region ToolCommonMethod
        public JsonResult DeleteToolRecords(string ids)
        {
            try
            {
                string response = string.Empty;
                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                response = objCommonDAL.DeleteModulewise(ids, "Tool", false, SessionHelper.UserID, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);
                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception)
            {
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetTechnician(string NameStartWith)
        {
            TechnicialMasterDAL objTechnicianCntrl = new eTurns.DAL.TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
            List<TechnicianMasterDTO> lstDTO = new List<TechnicianMasterDTO>();
            Int64 RoomID = SessionHelper.RoomID;
            Int64 CompanyID = SessionHelper.CompanyID;
            List<DTOForAutoComplete> returnKeyValList = new List<DTOForAutoComplete>();
            try
            {

                lstDTO = objTechnicianCntrl.GetTechnicianListSearch(SessionHelper.RoomID, SessionHelper.CompanyID, NameStartWith ?? string.Empty).OrderBy(i => i.TechnicianCode).ToList();
                if (string.IsNullOrWhiteSpace((NameStartWith ?? string.Empty).Trim()))
                    lstDTO.Insert(0, new TechnicianMasterDTO() { GUID = Guid.Empty, Technician = string.Empty, TechnicianCode = Convert.ToString(eTurns.DTO.Resources.ResCommon.SelectTechnicianText) });



                if (lstDTO != null && lstDTO.Count() > 0)
                {
                    foreach (var item in lstDTO)
                    {
                        if (!string.IsNullOrEmpty(item.Technician))
                        {
                            DTOForAutoComplete obj = new DTOForAutoComplete()
                            {

                                Key = Convert.ToString(item.TechnicianCode + " --- " + item.Technician),
                                Value = item.TechnicianCode + " --- " + item.Technician,
                                ID = item.ID,
                                GUID = item.GUID,
                            };

                            returnKeyValList.Add(obj);
                        }
                        else
                        {
                            DTOForAutoComplete obj = new DTOForAutoComplete()
                            {

                                Key = Convert.ToString(item.TechnicianCode),
                                Value = item.TechnicianCode,
                                ID = item.ID,
                                GUID = item.GUID,
                            };

                            returnKeyValList.Add(obj);
                        }
                    }
                }


                //if (returnKeyValList.Count > 0 && !string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                //{
                //    returnKeyValList = returnKeyValList.Where(x => x.Key.ToLower().Contains(NameStartWith.ToLower())).ToList();
                //}

                return Json(returnKeyValList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(returnKeyValList, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                objTechnicianCntrl = null;
                lstDTO = null;
            }
        }
        private List<CommonDTO> GetGroupOfItems()
        {
            List<CommonDTO> ItemType = new List<CommonDTO>();

            ItemType.Add(new CommonDTO() { ID = 1, Text = "Yes" });
            ItemType.Add(new CommonDTO() { ID = 0, Text = "No" });

            return ItemType;
        }
        public string GetLocationsMainGrid(Int64 ToolID, string ToolGUID, string _isSerialAvail = "No", string _searchTerm = "")
        {
            ViewBag.ToolID = ToolID;
            ViewBag.ToolGUID = ToolGUID;
            ViewBag.IsSerialAvail = _isSerialAvail;
            ViewBag.SearchTerm = _searchTerm;

            ToolMasterDAL objToolAPI = new ToolMasterDAL(SessionHelper.EnterPriseDBName);

            Guid ToolGUID1 = Guid.Empty;
            Guid.TryParse(ToolGUID, out ToolGUID1);
            var Objitem = objToolAPI.GetToolByGUIDPlain(ToolGUID1);

            ViewBag.ToolName = Objitem.ToolName;

            ViewBag.ToolGUID_ToolType = ToolGUID + "#" + Objitem.Type;


            return RenderRazorViewToString("_ToolBinWiseSummary", Objitem);
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
        private List<CommonDTO> GetSheduleItemTypeOptions()
        {
            List<CommonDTO> ItemType = new List<CommonDTO>();
            ItemType.Add(new CommonDTO() { ID = 1, Text = "Asset" });
            ItemType.Add(new CommonDTO() { ID = 2, Text = "Tool" });


            return ItemType;
        }
        private List<clslist> GetEmptyOption()
        {
            List<clslist> lstItem = new List<clslist>();

            return lstItem;
        }
        #endregion
        #region "Tool CheckInCheckOut"
        public string CheckInCheckOutData(Guid ToolGUID, string _isSerialAvail = "No", string _searchTerm = "")
        {
            ViewBag.ToolGUID = ToolGUID;

            ViewBag.IsSerialAvail = _isSerialAvail;
            ViewBag.SearchTerm = _searchTerm;

            ToolCheckInOutHistoryDAL objDAL = new ToolCheckInOutHistoryDAL(SessionHelper.EnterPriseDBName);
            var objModel = objDAL.GetToolCheckInOutHistoryNew(SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.ToolGUID == ToolGUID).ToList();
            return RenderRazorViewToString("_CheckInCheckOutData", objModel);
        }
        public string CheckOutCheckIn(string ActionType, Int32 Quantity, bool IsForMaintance, Guid ToolGUID, Double AQty, Double CQty, Double CMQty, string UDF1, string UDF2, string UDF3, string UDF4, string UDF5, string CheckInCheckOutGUID, bool IsOnlyFromUI, string TechnicianName, Guid? RequisitionDetailGUID, Guid? WorkOrderGUID, string SerialNumber = null, long ToolBinID = 0)
        {
            try
            {

                Guid TechnicianGuid = Guid.Empty;
                if (string.IsNullOrWhiteSpace(SerialNumber) || SerialNumber == "null")
                {
                    SerialNumber = string.Empty;
                }
                ToolCheckInOutHistoryDAL objCICODAL = new ToolCheckInOutHistoryDAL(SessionHelper.EnterPriseDBName);
                ToolCheckInOutHistoryDTO objCICODTO = new ToolCheckInOutHistoryDTO();

                ToolCheckInHistoryDAL objCIDAL = new ToolCheckInHistoryDAL(SessionHelper.EnterPriseDBName);
                ToolCheckInHistoryDTO objCIDTO = new ToolCheckInHistoryDTO();

                ToolMasterDAL objToolDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);

                Guid? toolguidLatest = null;
                ToolMasterDTO objToolDTO = null;
                if (!string.IsNullOrWhiteSpace(SerialNumber))
                {
                    objToolDTO = objToolDAL.GetToolByGUIDFull(ToolGUID);
                    toolguidLatest = ToolGUID;// objToolDAL.GetRecordBySerialAndTool(objToolDTO.ToolName , SessionHelper.RoomID, SessionHelper.CompanyID, SerialNumber).GUID;
                }
                else
                {
                    toolguidLatest = ToolGUID;
                }
                if (toolguidLatest != null && toolguidLatest.HasValue && toolguidLatest.Value != Guid.Empty)
                {
                    ToolGUID = toolguidLatest ?? Guid.Empty;
                }
                objToolDTO = objToolDAL.GetToolByGUIDFull(ToolGUID);

                //-----------------------INSERT NEW TECHNICIAN IF REQUIRED-----------------------
                //
                string TechnicianCode = "";
                TechnicianName = TechnicianName.Trim();
                if (TechnicianName.Contains(" --- "))
                {
                    TechnicianCode = TechnicianName.Split(new string[1] { " --- " }, StringSplitOptions.RemoveEmptyEntries)[0];
                    TechnicianName = TechnicianName.Split(new string[1] { " --- " }, StringSplitOptions.RemoveEmptyEntries)[1];
                }
                else
                {
                    TechnicianCode = TechnicianName;
                    TechnicianName = "";
                }

                if (TechnicianCode.Trim() == "")
                {
                    return ResMessage.InvalidTechnician;
                }

                if ((!string.IsNullOrEmpty(TechnicianCode) && TechnicianCode.IndexOf("---") >= 0)
                    || (!string.IsNullOrEmpty(TechnicianName) && TechnicianName.IndexOf("---") >= 0))
                {
                    return ResToolCheckInOutHistory.MsgRemoveInvalidValueFromTechnician;
                }

                TechnicialMasterDAL objTechnicialMasterDAL = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
                TechnicianMasterDTO objTechnicianMasterDTO = objTechnicialMasterDAL.GetTechnicianByCodePlain(TechnicianCode, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (objTechnicianMasterDTO == null)
                {
                    objTechnicianMasterDTO = new TechnicianMasterDTO();
                    objTechnicianMasterDTO.TechnicianCode = TechnicianCode;
                    objTechnicianMasterDTO.Technician = TechnicianName;
                    objTechnicianMasterDTO.Room = SessionHelper.RoomID;
                    objTechnicianMasterDTO.CompanyID = SessionHelper.CompanyID;
                    objTechnicianMasterDTO.CreatedBy = SessionHelper.UserID;
                    objTechnicianMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                    objTechnicianMasterDTO.Created = DateTimeUtility.DateTimeNow;
                    objTechnicianMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                    objTechnicianMasterDTO.GUID = Guid.NewGuid();
                    objTechnicianMasterDTO.IsArchived = false;
                    objTechnicianMasterDTO.IsDeleted = false;
                    Int64 TechnicanID = objTechnicialMasterDAL.Insert(objTechnicianMasterDTO);

                    objTechnicianMasterDTO = objTechnicialMasterDAL.GetTechnicianByIDPlain(TechnicanID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    TechnicianGuid = objTechnicianMasterDTO.GUID;
                }
                else
                {
                    TechnicianGuid = objTechnicianMasterDTO.GUID;
                }

                //-----------------------------------------------------------------------------
                //
                objCICODTO.CompanyID = SessionHelper.CompanyID;
                objCICODTO.Created = DateTimeUtility.DateTimeNow;
                objCICODTO.CreatedBy = SessionHelper.UserID;
                objCICODTO.CreatedByName = SessionHelper.UserName;
                objCICODTO.IsArchived = false;
                objCICODTO.IsDeleted = false;
                objCICODTO.LastUpdatedBy = SessionHelper.UserID;
                objCICODTO.Room = SessionHelper.RoomID;
                objCICODTO.RoomName = SessionHelper.RoomName;
                objCICODTO.ToolGUID = ToolGUID;
                objCICODTO.Updated = DateTimeUtility.DateTimeNow;
                objCICODTO.UpdatedByName = SessionHelper.UserName;
                objCICODTO.TechnicianGuid = TechnicianGuid;

                //Save CheckOut UDF
                objCICODTO.UDF1 = UDF1;
                objCICODTO.UDF2 = UDF2;
                objCICODTO.UDF3 = UDF3;
                objCICODTO.UDF4 = UDF4;
                objCICODTO.UDF5 = UDF5;

                objCICODTO.AddedFrom = "Web";
                objCICODTO.EditedFrom = "Web";
                objCICODTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                objCICODTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                objCICODTO.IsOnlyFromItemUI = IsOnlyFromUI;


                Double CurrentMQTYCICO = 0.0;
                Double CurrentQTYCICO = 0.0;

                if (ActionType == "co")
                {
                    //Check Out UDF is mendatory or not 
                    UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
                    IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetRequiredUDFsByUDFTableNamePlain("ToolCheckInOutHistory", SessionHelper.RoomID, SessionHelper.CompanyID);
                    string udfRequier = string.Empty;
                    var msgRequired = ResMessage.MsgRequired;
                    foreach (var i in DataFromDB)
                    {
                            if (i.UDFColumnName == "UDF1"  && string.IsNullOrEmpty(objCICODTO.UDF1))
                            {
                                string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                                string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                                if (!string.IsNullOrEmpty(val))
                                    i.UDFDisplayColumnName = val;
                                else
                                    i.UDFDisplayColumnName = i.UDFColumnName;
                                udfRequier = string.Format(msgRequired, i.UDFDisplayColumnName); 
                            }
                            else if (i.UDFColumnName == "UDF2"  && string.IsNullOrEmpty(objCICODTO.UDF2))
                            {
                                string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                                string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                                if (!string.IsNullOrEmpty(val))
                                    i.UDFDisplayColumnName = val;
                                else
                                    i.UDFDisplayColumnName = i.UDFColumnName;
                                udfRequier = string.Format(msgRequired, i.UDFDisplayColumnName); 
                            }
                            else if (i.UDFColumnName == "UDF3"  && string.IsNullOrEmpty(objCICODTO.UDF3))
                            {
                                string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                                string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                                if (!string.IsNullOrEmpty(val))
                                    i.UDFDisplayColumnName = val;
                                else
                                    i.UDFDisplayColumnName = i.UDFColumnName;
                                udfRequier = string.Format(msgRequired, i.UDFDisplayColumnName); 
                            }
                            else if (i.UDFColumnName == "UDF4"  && string.IsNullOrEmpty(objCICODTO.UDF4))
                            {
                                string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                                string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                                if (!string.IsNullOrEmpty(val))
                                    i.UDFDisplayColumnName = val;
                                else
                                    i.UDFDisplayColumnName = i.UDFColumnName;
                                udfRequier = string.Format(msgRequired, i.UDFDisplayColumnName); 
                            }
                            else if (i.UDFColumnName == "UDF5"  && string.IsNullOrEmpty(objCICODTO.UDF5))
                            {
                                string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                                string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                                if (!string.IsNullOrEmpty(val))
                                    i.UDFDisplayColumnName = val;
                                else
                                    i.UDFDisplayColumnName = i.UDFColumnName;
                                udfRequier = string.Format(msgRequired, i.UDFDisplayColumnName); 
                            }

                            if (!string.IsNullOrEmpty(udfRequier))
                                break;
                        
                    }

                    if (!string.IsNullOrEmpty(udfRequier))
                    {
                        objCICODTO = null;
                        return udfRequier;
                    }

                    #region "Check out"
                    double TotalToolQty = 0;
                    double TotalCheckOutQty = 0;
                    double TotalCheckMQty = 0;
                    double TotalCheckOutForQty = Quantity;
                    TotalToolQty = objToolDTO.Quantity;
                    TotalCheckOutQty = objToolDTO.CheckedOutQTY ?? 0;
                    TotalCheckMQty = objToolDTO.CheckedOutMQTY ?? 0;
                    if (ToolBinID > 0 && objToolDTO.SerialNumberTracking == false)
                    {
                        ToolAssetQuantityDetailDAL objTAQtyDetail = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
                        ToolAssetQuantityDetailDTO TAQtyDtlDTO = objTAQtyDetail.GetLocationQuantityToolByToolBinId(objToolDTO.GUID, ToolBinID, SessionHelper.RoomID, SessionHelper.CompanyID);

                        if (TAQtyDtlDTO != null && TotalCheckOutForQty > TAQtyDtlDTO.Quantity)
                        { 
                            return string.Format(ResToolCheckInOutHistory.NotEnoughQtyToCheckOutOnLocation, TAQtyDtlDTO.Location, TAQtyDtlDTO.Quantity);
                        }
                    }
                    else
                    {
                        if (TotalToolQty < (TotalCheckOutQty + TotalCheckMQty + TotalCheckOutForQty))
                        {
                            return ResToolMaster.MsgCheckoutQtyExceed;
                        }
                    }

                    if (IsForMaintance)
                    {
                        objCICODTO.CheckedOutMQTY = Quantity;
                        objCICODTO.CheckedOutQTY = 0;
                        objToolDTO.CheckedOutMQTY = objToolDTO.CheckedOutMQTY.GetValueOrDefault(0) + Quantity;
                        objCICODTO.CheckedOutMQTYCurrent = 0;
                    }
                    else
                    {
                        objCICODTO.CheckedOutQTY = Quantity;
                        objCICODTO.CheckedOutMQTY = 0;
                        objToolDTO.CheckedOutQTY = objToolDTO.CheckedOutQTY.GetValueOrDefault(0) + Quantity;
                        objCICODTO.CheckedOutQTYCurrent = 0;
                    }
                    if (objToolDTO.SerialNumberTracking)
                    {
                        objCICODTO.SerialNumber = SerialNumber;
                    }

                    objCICODTO.CheckOutDate = DateTimeUtility.DateTimeNow;
                    objCICODTO.CheckOutStatus = "Check Out";
                    #endregion
                    if (objCICODTO.UDF1 != null && objCICODTO.UDF1 != string.Empty)
                        InsertCheckOutUDf(objCICODTO.UDF1, CommonUtility.ImportToolMasterColumn.UDF1.ToString());
                    if (objCICODTO.UDF2 != null && objCICODTO.UDF2 != string.Empty)
                        InsertCheckOutUDf(objCICODTO.UDF2, CommonUtility.ImportToolMasterColumn.UDF2.ToString());
                    if (objCICODTO.UDF3 != null && objCICODTO.UDF3 != string.Empty)
                        InsertCheckOutUDf(objCICODTO.UDF3, CommonUtility.ImportToolMasterColumn.UDF3.ToString());
                    if (objCICODTO.UDF4 != null && objCICODTO.UDF4 != string.Empty)
                        InsertCheckOutUDf(objCICODTO.UDF4, CommonUtility.ImportToolMasterColumn.UDF4.ToString());
                    if (objCICODTO.UDF5 != null && objCICODTO.UDF5 != string.Empty)
                        InsertCheckOutUDf(objCICODTO.UDF5, CommonUtility.ImportToolMasterColumn.UDF5.ToString());

                    objCICODTO.RequisitionDetailGuid = RequisitionDetailGUID;
                    objCICODTO.WorkOrderGuid = WorkOrderGUID;

                    objCICODTO.ToolBinID = ToolBinID;

                    long CheckOutID = objCICODAL.Insert(objCICODTO);
                    //ToolCheckInOutHistoryDTO objToolCheckInOutHistoryDTO = objCICODAL.GetRecordById(CheckOutID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    Guid ToolCheckInOutHistoryGUID = objCICODAL.GetToolCheckinOutGUID(CheckOutID);
                    objToolDTO.IsOnlyFromItemUI = IsOnlyFromUI;
                    if (IsOnlyFromUI)
                    {
                        objToolDTO.EditedFrom = "Web";
                        objToolDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    }
                    objToolDTO.WhatWhereAction = "Tool was CheckOut from Web.";
                    objToolDAL.Edit(objToolDTO);
                    Guid? UsedToolGUId = ToolGUID;// objToolDAL.GetUsedToolGuidinQuantity(SessionHelper.RoomID, SessionHelper.CompanyID, objToolDTO.GUID, objToolDTO.ToolName);

                    ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO = new ToolAssetQuantityDetailDTO();
                    objToolAssetQuantityDetailDTO.ToolGUID = UsedToolGUId ?? ToolGUID;

                    objToolAssetQuantityDetailDTO.AssetGUID = null;


                    objToolAssetQuantityDetailDTO.ToolBinID = ToolBinID;//objToolDTO.ToolLocationDetailsID;
                    objToolAssetQuantityDetailDTO.Quantity = objToolDTO.Quantity;
                    objToolAssetQuantityDetailDTO.RoomID = SessionHelper.RoomID;
                    objToolAssetQuantityDetailDTO.CompanyID = SessionHelper.CompanyID;
                    objToolAssetQuantityDetailDTO.Created = DateTimeUtility.DateTimeNow;
                    objToolAssetQuantityDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                    objToolAssetQuantityDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objToolAssetQuantityDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objToolAssetQuantityDetailDTO.AddedFrom = "Web";
                    objToolAssetQuantityDetailDTO.EditedFrom = "Web";
                    objToolAssetQuantityDetailDTO.WhatWhereAction = "Toolcontroller>>CheckOutCheckIn";
                    objToolAssetQuantityDetailDTO.ReceivedDate = null;
                    objToolAssetQuantityDetailDTO.InitialQuantityWeb = objToolDTO.Quantity;
                    objToolAssetQuantityDetailDTO.InitialQuantityPDA = 0;
                    objToolAssetQuantityDetailDTO.ExpirationDate = null;
                    objToolAssetQuantityDetailDTO.EditedOnAction = "Tool was CheckOut from Web.";
                    objToolAssetQuantityDetailDTO.CreatedBy = SessionHelper.UserID;
                    objToolAssetQuantityDetailDTO.UpdatedBy = SessionHelper.UserID;
                    objToolAssetQuantityDetailDTO.IsDeleted = false;
                    objToolAssetQuantityDetailDTO.IsArchived = false;

                    ToolAssetQuantityDetailDAL objToolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
                    objToolAssetQuantityDetailDAL.UpdateOrInsert(objToolAssetQuantityDetailDTO, Quantity, CheckoutGUID: ToolCheckInOutHistoryGUID, ReferalAction: "Check Out", SerialNumber: SerialNumber);
                    //// Entry Create for # of checkouts
                    //if (objToolDTO != null && objToolDTO.IsGroupOfItems.GetValueOrDefault(0) == 0 && objCICODTO != null && objCICODTO.ID > 0)
                    //    MaintCreateForNoOfCheckout(objToolDTO, objCICODTO);
                }
                else
                {
                    objCIDTO.CompanyID = SessionHelper.CompanyID;
                    objCIDTO.Created = DateTimeUtility.DateTimeNow;
                    objCIDTO.CreatedBy = SessionHelper.UserID;
                    objCIDTO.CreatedByName = SessionHelper.UserName;
                    objCIDTO.IsArchived = false;
                    objCIDTO.IsDeleted = false;
                    objCIDTO.LastUpdatedBy = SessionHelper.UserID;
                    objCIDTO.Room = SessionHelper.RoomID;
                    objCIDTO.RoomName = SessionHelper.RoomName;

                    if (CheckInCheckOutGUID != "" && Guid.Parse(CheckInCheckOutGUID) != Guid.Empty)
                        objCIDTO.CheckInCheckOutGUID = Guid.Parse(CheckInCheckOutGUID);
                    objCIDTO.Updated = DateTimeUtility.DateTimeNow;
                    objCIDTO.UpdatedByName = SessionHelper.UserName;

                    #region "Check in"
                    double TotalToolQty = 0;
                    double TotalCheckOutQty = 0;
                    double TotalCheckMQty = 0;
                    double TotalCheckInForQty = Quantity;
                    TotalToolQty = objToolDTO.Quantity;
                    TotalCheckOutQty = objToolDTO.CheckedOutQTY ?? 0;
                    TotalCheckMQty = objToolDTO.CheckedOutMQTY ?? 0;

                    if (CheckInCheckOutGUID != "" && Guid.Parse(CheckInCheckOutGUID) != Guid.Empty)
                    {
                        ToolCheckInOutHistoryDTO checkoutData = objCICODAL.GetTCIOHByGUIDPlain(objCIDTO.CheckInCheckOutGUID, SessionHelper.RoomID, SessionHelper.CompanyID);

                        if (checkoutData != null && checkoutData.ID > 0)
                        {
                            if (IsForMaintance)
                            {
                                var maxPossibleCheckInMQty = checkoutData.CheckedOutMQTY.GetValueOrDefault(0) - checkoutData.CheckedOutMQTYCurrent.GetValueOrDefault(0);
                                if (Quantity > maxPossibleCheckInMQty)
                                {
                                    return ResToolMaster.MsgCheckInQtyExceed;
                                }
                            }
                            else
                            {
                                var maxPossibleCheckInQty = checkoutData.CheckedOutQTY.GetValueOrDefault(0) - checkoutData.CheckedOutQTYCurrent.GetValueOrDefault(0);
                                if (Quantity > maxPossibleCheckInQty)
                                {
                                    return ResToolMaster.MsgCheckInQtyExceed;
                                }
                            }
                        }
                        else
                        {
                            return ResToolMaster.MsgCheckOutNotFound;
                        }
                    }

                    if (TotalCheckInForQty > (TotalCheckOutQty + TotalCheckMQty))
                    {
                        return ResToolMaster.MsgTotalCheckInQtyExceed;
                    }


                    if (IsForMaintance)
                    {
                        //objCICODTO.CheckedOutMQTY = Quantity;
                        //objCICODTO.CheckedOutQTY = 0;
                        objToolDTO.CheckedOutMQTY = objToolDTO.CheckedOutMQTY.GetValueOrDefault(0) - Quantity;
                        CurrentMQTYCICO = Quantity; //objToolDTO.CheckedOutMQTY.GetValueOrDefault(0);

                        objCIDTO.CheckedOutMQTY = Quantity;
                        objCIDTO.CheckedOutQTY = 0;
                    }
                    else
                    {
                        //objCICODTO.CheckedOutQTY = Quantity;
                        //objCICODTO.CheckedOutMQTY = 0;
                        objToolDTO.CheckedOutQTY = objToolDTO.CheckedOutQTY.GetValueOrDefault(0) - Quantity;
                        CurrentQTYCICO = Quantity;//objToolDTO.CheckedOutQTY.GetValueOrDefault(0);

                        objCIDTO.CheckedOutQTY = Quantity;
                        objCIDTO.CheckedOutMQTY = 0;
                    }

                    objCIDTO.CheckInDate = DateTimeUtility.DateTimeNow;
                    objCIDTO.CheckOutStatus = "Check In";
                    objCIDTO.IsOnlyFromItemUI = IsOnlyFromUI;
                    objCIDTO.AddedFrom = "Web";
                    objCIDTO.EditedFrom = "Web";
                    objCIDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objCIDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objCIDTO.TechnicianGuid = TechnicianGuid;
                    objCIDTO.SerialNumber = SerialNumber;
                    long objCOID = objCIDAL.Insert(objCIDTO);
                    ToolCheckInHistoryDTO objToolCheckInHistoryDTO = objCIDAL.GetToolCheckInByIDPlain(objCOID, SessionHelper.RoomID, SessionHelper.CompanyID);

                    objToolDTO.IsOnlyFromItemUI = IsOnlyFromUI;

                    if (IsOnlyFromUI)
                    {
                        objToolDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objToolDTO.EditedFrom = "Web";
                    }
                    objToolDAL.Edit(objToolDTO);

                    #endregion

                    toolguidLatest = ToolGUID; //objToolDAL.GetToolNameBySerial(SerialNumber, SessionHelper.RoomID, SessionHelper.CompanyID).GUID;
                    Guid? UsedToolGUId = ToolGUID;// objToolDAL.GetUsedToolGuidinQuantity(SessionHelper.RoomID, SessionHelper.CompanyID, objToolDTO.GUID, objToolDTO.ToolName);
                    ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO = new ToolAssetQuantityDetailDTO();
                    objToolAssetQuantityDetailDTO.ToolGUID = UsedToolGUId ?? ToolGUID;

                    objToolAssetQuantityDetailDTO.AssetGUID = null;


                    objToolAssetQuantityDetailDTO.ToolBinID = (ToolBinID > 0) ? ToolBinID : objToolDTO.ToolLocationDetailsID;
                    objToolAssetQuantityDetailDTO.Quantity = Quantity;
                    objToolAssetQuantityDetailDTO.RoomID = SessionHelper.RoomID;
                    objToolAssetQuantityDetailDTO.CompanyID = SessionHelper.CompanyID;
                    objToolAssetQuantityDetailDTO.Created = DateTimeUtility.DateTimeNow;
                    objToolAssetQuantityDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                    objToolAssetQuantityDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objToolAssetQuantityDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objToolAssetQuantityDetailDTO.AddedFrom = "Web";
                    objToolAssetQuantityDetailDTO.EditedFrom = "Web";
                    objToolAssetQuantityDetailDTO.WhatWhereAction = "Toolcontroller>>CheckoutCheckIn";
                    objToolAssetQuantityDetailDTO.ReceivedDate = null;
                    objToolAssetQuantityDetailDTO.InitialQuantityWeb = objToolDTO.Quantity;
                    objToolAssetQuantityDetailDTO.InitialQuantityPDA = 0;
                    objToolAssetQuantityDetailDTO.ExpirationDate = null;
                    objToolAssetQuantityDetailDTO.EditedOnAction = "Tool was Checkin from Web.";
                    objToolAssetQuantityDetailDTO.CreatedBy = SessionHelper.UserID;
                    objToolAssetQuantityDetailDTO.UpdatedBy = SessionHelper.UserID;
                    objToolAssetQuantityDetailDTO.IsDeleted = false;
                    objToolAssetQuantityDetailDTO.IsArchived = false;
                    objToolAssetQuantityDetailDTO.SerialNumber = SerialNumber;
                    ToolAssetQuantityDetailDAL objToolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
                    objToolAssetQuantityDetailDAL.Insert(objToolAssetQuantityDetailDTO, false, CheckoutGUID: objToolCheckInHistoryDTO.CheckInCheckOutGUID, CheckinGUID: objToolCheckInHistoryDTO.GUID, ReferalAction: "Check In", SerialNumber: SerialNumber);
                    // Entry Create for # of checkouts
                    if (objToolDTO != null && objToolDTO.IsGroupOfItems.GetValueOrDefault(0) == 0 && objCIDTO != null && objCIDTO.ID > 0)
                        MaintCreateForNoOfCheckoutAtCheckIn(objToolDTO, objCIDTO);

                }

                if (CheckInCheckOutGUID != "" && Guid.Parse(CheckInCheckOutGUID) != Guid.Empty)
                {
                    //ToolCheckInOutHistoryDTO objPrvCICODTO = objCICODAL.GetRecord(Guid.Parse(CheckInCheckOutGUID), SessionHelper.RoomID, SessionHelper.CompanyID);
                    ToolCheckInOutHistoryDTO objPrvCICODTO = objCICODAL.GetTCIOHByGUIDPlain(Guid.Parse(CheckInCheckOutGUID), SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (objPrvCICODTO != null)
                    {
                        if (IsForMaintance)
                            objPrvCICODTO.CheckedOutMQTYCurrent = objPrvCICODTO.CheckedOutMQTYCurrent.GetValueOrDefault(0) + CurrentMQTYCICO;
                        else
                            objPrvCICODTO.CheckedOutQTYCurrent = objPrvCICODTO.CheckedOutQTYCurrent.GetValueOrDefault(0) + CurrentQTYCICO;

                        objPrvCICODTO.IsOnlyFromItemUI = IsOnlyFromUI;
                        if (IsOnlyFromUI)
                        {
                            objPrvCICODTO.EditedFrom = "Web";
                            objPrvCICODTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        }
                        objCICODAL.Edit(objPrvCICODTO);
                    }
                }

                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string CheckOutAll(string arrItems)
        {
            string message = string.Empty;
            try
            {
                JavaScriptSerializer s = new JavaScriptSerializer();
                List<ToolCheckoutAll> toochkoutlist = s.Deserialize<List<ToolCheckoutAll>>(arrItems);
                var checkoutMsg = ResToolMaster.MsgCheckoutWithQty;
                foreach (ToolCheckoutAll t in toochkoutlist)
                {
                    string returnResult = CheckOutCheckIn(t.ActionType, t.Quantity, t.IsForMaintance, t.ToolGUID, t.AQty, t.CQty, t.CMQty, t.UDF1, t.UDF2, t.UDF3, t.UDF4, t.UDF5, t.CheckInCheckOutGUID, t.IsOnlyFromUI, t.TechnicianName, null, null);
                    if (returnResult != "ok")
                    {
                        message += returnResult + "<br />";
                    }
                    else
                    {
                        message += (string.Format(checkoutMsg, t.ToolName, Convert.ToString(t.Quantity)) + "<br />");
                    }
                }

                //if (string.IsNullOrEmpty(message))
                //{
                //    return "ok";
                //}
                //else
                //{
                return message;
                //}
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }

        }
        public string CheckInAll(string arrItems)
        {
            string message = string.Empty;
            try
            {
                JavaScriptSerializer s = new JavaScriptSerializer();
                List<ToolCheckoutAll> toochkoutlist = s.Deserialize<List<ToolCheckoutAll>>(arrItems);
                var checkInMsg = ResToolMaster.MsgCheckInWithQty;
                foreach (ToolCheckoutAll t in toochkoutlist)
                {
                    string returnResult = CheckOutCheckIn(t.ActionType, t.Quantity, t.IsForMaintance, t.ToolGUID, t.AQty, t.CQty, t.CMQty, t.UDF1, t.UDF2, t.UDF3, t.UDF4, t.UDF5, t.CheckInCheckOutGUID, t.IsOnlyFromUI, t.TechnicianName, null, null);
                    if (returnResult != "ok")
                    {
                        message += returnResult + "<br />";
                    }
                    else
                    {
                        message += (string.Format(checkInMsg, t.ToolName, Convert.ToString(t.Quantity)) + "<br />");
                    }
                }

                //if (string.IsNullOrEmpty(message))
                //{
                //    return "ok";
                //}
                //else
                //{
                return message;
                //}
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }

        }
        public ActionResult CheckInCheckOutListAjax(JQueryDataTableParamModel param)
        {
            Guid ToolGUID = Guid.Parse(Request["ItemID"].ToString());

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
            string parentSearch = string.Empty;
            parentSearch = Convert.ToString(Request["parentSearch"]);

            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());

            Guid reqDetailGuid = Guid.Empty;
            if (!string.IsNullOrEmpty(Request["RequisitionDetailGUID"]))
                Guid.TryParse(Request["RequisitionDetailGUID"], out reqDetailGuid);

            Guid? reqDetGUID = null;
            if (reqDetailGuid != Guid.Empty)
                reqDetGUID = reqDetailGuid;

            Guid WorkOrderGUID = Guid.Empty;
            if (!string.IsNullOrEmpty(Request["WorkOrderGUID"]))
                Guid.TryParse(Request["WorkOrderGUID"], out WorkOrderGUID);

            Guid? WOGUID = null;
            if (WOGUID != Guid.Empty)
                WOGUID = WorkOrderGUID;

            Guid ToolCheckoutGUID = Guid.Empty;
            if (!string.IsNullOrEmpty(Request["ToolCheckoutGUID"]))
                Guid.TryParse(Request["ToolCheckoutGUID"], out ToolCheckoutGUID);

            Guid? TCGUID = null;
            if (TCGUID != Guid.Empty)
                TCGUID = ToolCheckoutGUID;

            // set the default column sorting here, if first time then required to set 
            if (sortColumnName == "0" || sortColumnName == "undefined" || sortColumnName == "ShippingMethod")
                sortColumnName = "CheckOutDate";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            ///////////// requried when paging needs in this method /////////////////

            /*////////SET sSearch PARAMETER FROM REQUEST PARAMETER FOR CHILE GRID////////////////*/
            if (string.IsNullOrWhiteSpace(param.sSearch) && Request["sSearchInner"] != null)
            {
                param.sSearch = Convert.ToString(Request["sSearchInner"]);
            }
            /*////////SET sSearch PARAMETER FROM REQUEST PARAMETER FOR CHILE GRID////////////////*/

            ViewBag.ToolGUID = ToolGUID;
            ToolCheckInOutHistoryDAL objDAL = new ToolCheckInOutHistoryDAL(SessionHelper.EnterPriseDBName);
            int TotalRecordCount = 0;
            var objModel = objDAL.GetPagedRecordsNew(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, ToolGUID, reqDetGUID, WOGUID, TCGUID, parentSearch);



            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = objModel
            },
                        JsonRequestBehavior.AllowGet);
        }
        public string DeleteCheckInCheckOutRecords(string ids)
        {
            try
            {
                string MSG = "";
                ToolCheckInOutHistoryDAL obj = new ToolCheckInOutHistoryDAL(SessionHelper.EnterPriseDBName);
                obj.DeleteRecords(ids, SessionHelper.UserID, SessionHelper.CompanyID, SessionHelper.RoomID, out MSG);
                if (MSG == string.Empty)
                    return "ok";
                else
                    return string.Format(ResToolMaster.MsgRecordsNotFullyCheckin, MSG.ToString());
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public void InsertCheckOutUDf(string UDFOption, string UDFColumn)
        {
            string UdfTablename = ImportMastersDTO.TableName.ToolCheckInOutHistory.ToString();
            Int64 UDFID = 0;
            //List<UDFDTO> objUDFDTOList = new List<UDFDTO>();
            UDFDAL objUDFDAL = new UDFDAL(SessionHelper.EnterPriseDBName);
            //int TotalRecordCount = 0;
            var udf = objUDFDAL.GetUDFByUDFColumnNamePlain(UDFColumn,  UdfTablename, SessionHelper.RoomID, SessionHelper.CompanyID);
            
            if (udf != null && udf.ID > 0)
            {
                UDFID = udf.ID;
                CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

                string strOK = objCDAL.DuplicateUFDOptionCheck(UDFOption, "add", 0, "UDFOptions", "UDFOption", UDFID);
                if (strOK == "duplicate")
                {

                }
                else
                {
                    //UDFOptionApiController obj = new UDFOptionApiController();
                    UDFOptionDAL obj = new UDFOptionDAL(SessionHelper.EnterPriseDBName);

                    UDFOptionsDTO objDTO = new UDFOptionsDTO();
                    objDTO.ID = 0;
                    objDTO.UDFOption = UDFOption;
                    objDTO.UDFID = UDFID;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.UpdatedByName = SessionHelper.UserName;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;

                    objDTO.CompanyID = SessionHelper.CompanyID;
                    //objDTO.Roo = 0;

                    objDTO.GUID = Guid.NewGuid();

                    var ResponseStatus = obj.Insert(objDTO);

                }
            }
        }
        private void MaintCreateForNoOfCheckoutAtCheckIn(ToolMasterDTO objToolDTO, ToolCheckInHistoryDTO objCIDTO)
        {
            AssetMasterDAL objAssetMasterDAL = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
            ToolsSchedulerDAL ToolsSchedulerDAL = new ToolsSchedulerDAL(SessionHelper.EnterPriseDBName);
            ToolsSchedulerDTO objToolsSchedulerDTO1 = null;
            List<ToolsSchedulerMappingDTO> lstToolsSchedulerMappingDTO = objAssetMasterDAL.GetSchedulerMappingRecordforTool_SchedularGUID(objToolDTO.GUID, SessionHelper.CompanyID, SessionHelper.RoomID, false, false);
            ToolsMaintenanceDAL objToolsMainDAL = new ToolsMaintenanceDAL(SessionHelper.EnterPriseDBName);
            ToolCheckInOutHistoryDAL objCICODAL = new ToolCheckInOutHistoryDAL(SessionHelper.EnterPriseDBName);
            ToolCheckInHistoryDAL objCIDAL = new ToolCheckInHistoryDAL(SessionHelper.EnterPriseDBName);

            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
            DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);

            foreach (var ToolsSchedulerMappingDTO in lstToolsSchedulerMappingDTO)
            {
                objToolsSchedulerDTO1 = ToolsSchedulerDAL.GetToolsSchedulerByGuidPlain(ToolsSchedulerMappingDTO.ToolSchedulerGuid.GetValueOrDefault(Guid.Empty));
                if (objToolsSchedulerDTO1 != null && objToolsSchedulerDTO1.SchedulerType == (int)MaintenanceScheduleType.CheckOuts)
                {
                    List<ToolCheckInOutHistoryDTO> lstToolCheckInOutHistoryDTO = null;
                    //lstToolCheckInOutHistoryDTO = objCICODAL.GetRecordByTool(objToolDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
                    lstToolCheckInOutHistoryDTO = objCICODAL.GetTCIOHsByToolGUIDWithToolInfo(objToolDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (lstToolCheckInOutHistoryDTO.Count() >= objToolsSchedulerDTO1.CheckOuts) // Entery in Maintenance
                    {
                        ToolsMaintenanceDTO objToolsMainDTO = objToolsMainDAL.GetToolsMaintenanceSchedulerMappingPlain(null, objToolDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, objToolsSchedulerDTO1.GUID, ToolsSchedulerMappingDTO.GUID);
                        if (objToolsMainDTO == null) // First Entery in ToolsMaintenance
                        {
                            lstToolCheckInOutHistoryDTO = lstToolCheckInOutHistoryDTO.Where(x => x.GUID != objCIDTO.CheckInCheckOutGUID).ToList();
                            double CalcScheduleDay = CalculateToolCheckoutDays(lstToolCheckInOutHistoryDTO, true);
                            ToolsMaintenanceDTO objTMDTO = new ToolsMaintenanceDTO();
                            objTMDTO.ToolGUID = objToolDTO.GUID;
                            objTMDTO.ScheduleDate = datetimetoConsider.Date.AddDays(CalcScheduleDay);
                            objTMDTO.SchedulerGUID = objToolsSchedulerDTO1.GUID;
                            objTMDTO.MaintenanceName = objToolsSchedulerDTO1.SchedulerName;
                            objTMDTO.AssetGUID = null;
                            objTMDTO.CompanyID = SessionHelper.CompanyID;
                            objTMDTO.Created = objCIDTO.Created.Value.AddSeconds(-1); //DateTimeUtility.DateTimeNow;
                            objTMDTO.CreatedBy = SessionHelper.UserID;
                            objTMDTO.GUID = Guid.NewGuid();
                            objTMDTO.IsArchived = false;
                            objTMDTO.IsDeleted = false;
                            objTMDTO.LastMaintenanceDate = null;
                            objTMDTO.LastMeasurementValue = null;
                            objTMDTO.MaintenanceDate = datetimetoConsider.Date.AddDays(CalcScheduleDay);
                            objTMDTO.LastUpdatedBy = SessionHelper.UserID;
                            objTMDTO.MaintenanceType = MaintenanceType.UnScheduled.ToString();
                            objTMDTO.MappingGUID = ToolsSchedulerMappingDTO.GUID;
                            objTMDTO.RequisitionGUID = null;
                            objTMDTO.Room = SessionHelper.RoomID;
                            objTMDTO.ScheduleFor = objToolsSchedulerDTO1.ScheduleFor;
                            objTMDTO.SchedulerGUID = objToolsSchedulerDTO1.GUID;
                            objTMDTO.SchedulerType = (byte)objToolsSchedulerDTO1.SchedulerType;
                            objTMDTO.Status = MaintenanceStatus.Open.ToString();
                            objTMDTO.TrackngMeasurement = objToolsSchedulerDTO1.SchedulerType;
                            objTMDTO.UDF1 = null;
                            objTMDTO.UDF2 = null;
                            objTMDTO.UDF3 = null;
                            objTMDTO.UDF4 = null;
                            objTMDTO.UDF5 = null;
                            objTMDTO.Updated = DateTimeUtility.DateTimeNow;
                            objTMDTO.WorkorderGUID = null;
                            objToolsMainDAL.Insert(objTMDTO);
                        }
                        else
                        {
                            lstToolCheckInOutHistoryDTO = lstToolCheckInOutHistoryDTO.Where(x => x.GUID != objCIDTO.CheckInCheckOutGUID && x.Created > objToolsMainDTO.Created).ToList();
                            if (lstToolCheckInOutHistoryDTO.Count() >= objToolsSchedulerDTO1.CheckOuts)
                            {
                                double CalcScheduleDay = CalculateToolCheckoutDays(lstToolCheckInOutHistoryDTO, true);
                                ToolsMaintenanceDTO objTMDTO = new ToolsMaintenanceDTO();
                                objTMDTO.ToolGUID = objToolDTO.GUID;
                                objTMDTO.ScheduleDate = objToolsMainDTO.ScheduleDate.Value.AddDays(CalcScheduleDay);
                                objTMDTO.SchedulerGUID = objToolsSchedulerDTO1.GUID;
                                objTMDTO.MaintenanceName = objToolsSchedulerDTO1.SchedulerName;
                                objTMDTO.AssetGUID = null;
                                objTMDTO.CompanyID = SessionHelper.CompanyID;
                                objTMDTO.Created = objCIDTO.Created.Value.AddSeconds(-1); //DateTimeUtility.DateTimeNow;
                                objTMDTO.CreatedBy = SessionHelper.UserID;
                                objTMDTO.GUID = Guid.NewGuid();
                                objTMDTO.IsArchived = false;
                                objTMDTO.IsDeleted = false;
                                objTMDTO.LastMaintenanceDate = null;
                                objTMDTO.LastMeasurementValue = null;
                                objTMDTO.MaintenanceDate = objToolsMainDTO.MaintenanceDate.Value.AddDays(CalcScheduleDay);
                                objTMDTO.LastUpdatedBy = SessionHelper.UserID;
                                objTMDTO.MaintenanceType = MaintenanceType.UnScheduled.ToString();
                                objTMDTO.MappingGUID = ToolsSchedulerMappingDTO.GUID;
                                objTMDTO.RequisitionGUID = null;
                                objTMDTO.Room = SessionHelper.RoomID;
                                objTMDTO.ScheduleFor = objToolsSchedulerDTO1.ScheduleFor;
                                objTMDTO.SchedulerGUID = objToolsSchedulerDTO1.GUID;
                                objTMDTO.SchedulerType = (byte)objToolsSchedulerDTO1.SchedulerType;
                                objTMDTO.Status = MaintenanceStatus.Open.ToString();
                                objTMDTO.TrackngMeasurement = objToolsSchedulerDTO1.SchedulerType;
                                objTMDTO.UDF1 = null;
                                objTMDTO.UDF2 = null;
                                objTMDTO.UDF3 = null;
                                objTMDTO.UDF4 = null;
                                objTMDTO.UDF5 = null;
                                objTMDTO.Updated = DateTimeUtility.DateTimeNow;
                                objTMDTO.WorkorderGUID = null;
                                objToolsMainDAL.Insert(objTMDTO);
                            }

                        }
                    }
                }
            }
        }

        #endregion
        #region "Tool Scheudler Maintenance"
        public ActionResult ScheduleMappingCreate(Guid? ToolGUID, Guid? AssetGUID)
        {
            UDFController objUDFDAL = new UDFController();
            ToolsSchedulerMappingDTO objDTO = new ToolsSchedulerMappingDTO();
            ViewBag.SheduleItemType = GetSheduleItemTypeOptions();
            ViewBag.SheduleAssetTool = GetEmptyOption();
            ViewBag.SchedulerForName = GetEmptyOption();
            objDTO.GUID = Guid.NewGuid();
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.SchedulerType = 1;
            objDTO.IsArchived = false;
            objDTO.IsDeleted = false;
            objDTO.ToolGUID = ToolGUID;
            objDTO.AssetGUID = AssetGUID;
            if (objDTO.AssetGUID.HasValue)
            {
                objDTO.SchedulerFor = 1;
            }
            if (objDTO.ToolGUID.HasValue)
            {
                objDTO.SchedulerFor = 2;
            }
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("toolschedulemapping");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            return PartialView("_CreateScheduleMapping", objDTO);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult SchedulerMappingSave(ToolsSchedulerMappingDTO objDTO)
        {
            string message = "";
            string status = "";
            if (objDTO.SchedulerFor == 1)
            {
                objDTO.AssetGUID = objDTO.AssetToolGUID;
            }
            else if (objDTO.SchedulerFor == 2)
            {
                objDTO.ToolGUID = objDTO.AssetToolGUID;
            }
            AssetMasterDAL objAssetMasterDAL = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
            ToolsMaintenanceDAL objToolMaintDAL = new ToolsMaintenanceDAL(SessionHelper.EnterPriseDBName);

            if (objDTO.SchedulerFor == 1)
            {
                ToolsSchedulerDAL ToolsSchedulerDAL = new ToolsSchedulerDAL(SessionHelper.EnterPriseDBName);
                ToolsSchedulerDTO objToolsSchedulerDTO1 = ToolsSchedulerDAL.GetToolsSchedulerByGuidPlain(objDTO.ToolSchedulerGuid.GetValueOrDefault(Guid.Empty));
                if (objToolsSchedulerDTO1.SchedulerType == (int)MaintenanceScheduleType.CheckOuts && objDTO.SchedulerFor == 1)
                {
                    message = ResToolsSchedulerMapping.errToolMappingNotAllowedAsset;
                    status = "fail";
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                }
            }
            if (objDTO.SchedulerFor == 2)
            {
                ToolMasterDAL objToolMasterDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                ToolsSchedulerDAL ToolsSchedulerDAL = new ToolsSchedulerDAL(SessionHelper.EnterPriseDBName);
                ToolMasterDTO objTool = new ToolMasterDTO();
                ToolsSchedulerDTO objToolsSchedulerDTO1 = new ToolsSchedulerDTO();
                if ((objDTO.ToolGUID ?? Guid.Empty) != Guid.Empty)
                {
                    objTool = objToolMasterDAL.GetToolByGUIDPlain(objDTO.ToolGUID ?? Guid.Empty);
                    objToolsSchedulerDTO1 = ToolsSchedulerDAL.GetToolsSchedulerByGuidPlain(objDTO.ToolSchedulerGuid.GetValueOrDefault(Guid.Empty));
                }

                if (objToolsSchedulerDTO1.SchedulerType == (int)MaintenanceScheduleType.CheckOuts && (objTool.IsGroupOfItems ?? 0) == 1)
                {
                    message = ResToolsSchedulerMapping.errToolMappingNotAllowed;
                    status = "fail";
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                }
            }

            if (!objAssetMasterDAL.CheckScheduleMapping(objDTO))
            {
                ToolsSchedulerMappingDTO objToolsSchedulerMappingDTO = objAssetMasterDAL.InsertToolmapping(objDTO);
                ToolsSchedulerDTO objToolsSchedulerDTO = objAssetMasterDAL.GetToolSchedulebyGUID(objToolsSchedulerMappingDTO.ToolSchedulerGuid ?? Guid.Empty);
                if (objToolsSchedulerDTO.SchedulerType == (int)MaintenanceScheduleType.TimeBase)
                {
                    objToolMaintDAL.CreateNewMaintenanceAuto(objDTO.AssetGUID, objDTO.ToolGUID, objDTO.ToolSchedulerGuid.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                }
                else if (objToolsSchedulerDTO.SchedulerType == (int)MaintenanceScheduleType.OperationalHours)
                {
                    objToolMaintDAL.CreateNewMaintenanceAuto(objDTO.AssetGUID, objDTO.ToolGUID, objDTO.ToolSchedulerGuid.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                }
                else if (objToolsSchedulerDTO.SchedulerType == (int)MaintenanceScheduleType.Mileage)
                {
                    objToolMaintDAL.CreateNewMaintenanceAuto(objDTO.AssetGUID, objDTO.ToolGUID, objDTO.ToolSchedulerGuid.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                }


                message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                status = "ok";
            }
            else
            {
                message = string.Format(ResMessage.DuplicateMessage, "This", "mapping");
                status = "duplicate";
            }

            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetSchedulByGUID(Guid? ScheduleGUID)
        {
            try
            {
                AssetMasterDAL objAssetMasterDAL = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
                return Json(objAssetMasterDAL.GetToolSchedulebyGUID(ScheduleGUID ?? Guid.Empty));
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }
        public ActionResult LoadScheduleList()
        {
            ToolsSchedulerDAL obj = new ToolsSchedulerDAL(SessionHelper.EnterPriseDBName);
            List<ToolsSchedulerMappingDTO> lstSchedule = null;
            if (!String.IsNullOrEmpty(Request.QueryString["AssetGUID"]))
            {
                Guid AssetGUID = Guid.Empty;
                if (Guid.TryParse(Request.QueryString["AssetGUID"].ToString(), out AssetGUID))
                {
                    AssetMasterDAL objAsset = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
                    ViewBag.ToolAssetName = objAsset.GetRecord(AssetGUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false).AssetName;
                    //lstSchedule = obj.GetAllRecords(AssetGUID, null, SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
                    lstSchedule = obj.GetAllSchedulesforToolAsset(null, AssetGUID, null);
                }
            }
            else if (!String.IsNullOrEmpty(Request.QueryString["ToolGUID"]))
            {
                Guid ToolGUID = Guid.Empty;
                if (Guid.TryParse(Request.QueryString["ToolGUID"].ToString(), out ToolGUID))
                {
                    ToolMasterDAL objTool = new eTurns.DAL.ToolMasterDAL(SessionHelper.EnterPriseDBName);

                    ToolMasterDTO objToolMasterDTO = new ToolMasterDTO();
                    objToolMasterDTO = objTool.GetToolByGUIDPlain(ToolGUID);
                    if (objToolMasterDTO != null && objToolMasterDTO.ID > 0)
                    {
                        ViewBag.ToolAssetName = objToolMasterDTO.ToolName;
                    }

                    //ViewBag.ToolAssetName = objAsset.GetToolListByID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ToolGUID, null).ToolName;
                    //lstSchedule = obj.GetAllRecords(null, ToolGUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
                    lstSchedule = obj.GetAllSchedulesforToolAsset(ToolGUID, null, null);
                }
            }
            return PartialView("_SchedulerList", lstSchedule);
        }
        public ActionResult OdometerCreate(Guid? AssetGUID, Guid? ToolGUID)
        {

            ToolsMaintenanceDTO objDTO = new ToolsMaintenanceDTO();
            objDTO.ID = 0;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.RoomName = SessionHelper.RoomName;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.GUID = Guid.NewGuid();
            if (AssetGUID != null)
            {
                objDTO.AssetGUID = AssetGUID.Value;
                AssetMasterDAL objAsset = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
                ViewBag.ToolAssetName = objAsset.GetAssetMasterByGUID(AssetGUID.Value, false, false).FirstOrDefault().AssetName;
            }

            if (ToolGUID != null)
            {
                objDTO.ToolGUID = ToolGUID.Value;
                ToolMasterDAL objTool = new eTurns.DAL.ToolMasterDAL(SessionHelper.EnterPriseDBName);
                ToolMasterDTO objToolMasterDTO = new ToolMasterDTO();
                objToolMasterDTO = objTool.GetToolByGUIDPlain(ToolGUID.GetValueOrDefault(Guid.Empty));
                if (objToolMasterDTO != null && objToolMasterDTO.ID > 0)
                {
                    ViewBag.ToolAssetName = objToolMasterDTO.ToolName;
                }
                //ViewBag.ToolAssetName = objAsset.GetToolListByID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ToolGUID.Value, null).ToolName;
            }


            //Dropdown list
            UDFController objUDFDAL = new UDFController();
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("ToolsOdometer");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            return PartialView("_CreateOdometer", objDTO);
        }
        public ActionResult LoadOdometerList()
        {
            ToolsOdometerDAL obj = new ToolsOdometerDAL(SessionHelper.EnterPriseDBName);
            List<ToolsOdometerDTO> lstSchedule = null;
            if (!String.IsNullOrEmpty(Request.QueryString["AssetGUID"]))
            {
                Guid AssetGUID = Guid.Empty;
                if (Guid.TryParse(Request.QueryString["AssetGUID"].ToString(), out AssetGUID))
                {
                    AssetMasterDAL objAsset = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
                    ViewBag.ToolAssetName = objAsset.GetRecord(AssetGUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false).AssetName;
                    lstSchedule = obj.GetAllRecords(AssetGUID, null, SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
                }
            }
            else if (!String.IsNullOrEmpty(Request.QueryString["ToolGUID"]))
            {
                Guid ToolGUID = Guid.Empty;
                if (Guid.TryParse(Request.QueryString["ToolGUID"].ToString(), out ToolGUID))
                {
                    ToolMasterDAL objToolDAL = new eTurns.DAL.ToolMasterDAL(SessionHelper.EnterPriseDBName);
                    ToolMasterDTO objTool = objToolDAL.GetToolByGUIDPlain(ToolGUID);
                    ViewBag.ToolAssetName = objTool.ToolName;
                    lstSchedule = obj.GetAllRecords(null, ToolGUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
                }
            }
            return PartialView("_OdometerList", lstSchedule);
        }
        public ActionResult LoadMaintenanceList()
        {
            ToolsMaintenanceDAL obj = new eTurns.DAL.ToolsMaintenanceDAL(SessionHelper.EnterPriseDBName);
            List<ToolsMaintenanceDTO> lstMaintenance = null;
            if (!String.IsNullOrEmpty(Request.QueryString["AssetGUID"]))
            {
                Guid AssetGUID = Guid.Empty;
                if (Guid.TryParse(Request.QueryString["AssetGUID"].ToString(), out AssetGUID))
                {
                    AssetMasterDAL objAsset = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
                    ViewBag.ToolAssetName = objAsset.GetAssetMasterByGUID(AssetGUID, false, false).FirstOrDefault().AssetName;
                    lstMaintenance = obj.GetToolsMaintenanceByFilterNormal(AssetGUID, null, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
                }
            }
            else if (!String.IsNullOrEmpty(Request.QueryString["ToolGUID"]))
            {
                Guid ToolGUID = Guid.Empty;
                if (Guid.TryParse(Request.QueryString["ToolGUID"].ToString(), out ToolGUID))
                {
                    ToolMasterDAL objToolDAL = new eTurns.DAL.ToolMasterDAL(SessionHelper.EnterPriseDBName);
                    ToolMasterDTO objTool = objToolDAL.GetToolByGUIDPlain(ToolGUID);
                    ViewBag.ToolAssetName = objTool.ToolName;
                    lstMaintenance = obj.GetToolsMaintenanceByFilterNormal(null, ToolGUID, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
                }
            }
            return PartialView("_MaintenanceList", lstMaintenance);
        }
        #endregion
        private double CalculateToolCheckoutDays(List<ToolCheckInOutHistoryDTO> lstToolCheckInOutHistoryDTO, bool IsFirst)
        {
            double DayDiffer = 0;
            if (IsFirst)
            {
                ToolCheckInOutHistoryDTO objDTO = lstToolCheckInOutHistoryDTO.OrderBy(x => x.Created).FirstOrDefault();
                DateTime DtFirst = (DateTime)objDTO.CheckOutDate;

                objDTO = lstToolCheckInOutHistoryDTO.OrderBy(x => x.Created).LastOrDefault();
                DateTime DtLast = (DateTime)objDTO.CheckOutDate;

                DayDiffer = (DtLast - DtFirst).TotalDays;
            }

            return Math.Ceiling(DayDiffer);
        }

        #region "Tool History"
        public ActionResult LoadToolHistoryList()
        {
            return PartialView("_ToolHistoryListForDisplay");
        }

        public ActionResult ToolHistoryListAjax(JQueryDataTableParamModel param)
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

            // set the default column sorting here, if first time then required to set 
            //if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID";



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
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetPagedToolHistoryNew(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, null, null, RoomDateFormat, CurrentTimeZone);


            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);
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
                    objDTO.IsDefault = IsDefault ?? false;

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

                        objDTO.ToolGUID = ToolGUID;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.IsDefault = IsDefault ?? false;
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
                        objDTO.IsDefault = IsDefault ?? false;
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
                    objDTO.IsDefault = IsDefault ?? false;
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
                    objDTO.IsDefault = IsDefault ?? false;
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
                        objDTO.IsDefault = IsDefault ?? false;
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
                        objDTO.IsDefault = IsDefault ?? false;
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
                    objDTO.IsDefault = IsDefault ?? false;
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
                    ToolAssetQuantityDetailDAL objToolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
                    bool CheckQtyExists = false;
                    CheckQtyExists = objToolAssetQuantityDetailDAL.CheckQtyExistsOnLocation(GUID, ToolGUID);
                    if (!CheckQtyExists)
                    {
                        lstBinReplanish.Remove(lstBinReplanish.Where(t => t.ID == ID).FirstOrDefault());
                        Session["ToolBinReplanish"] = lstBinReplanish;
                    }
                    else
                    {
                        return Json(new { status = "qtyExists" }, JsonRequestBehavior.AllowGet);
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
        public ActionResult ToolBinWiseSummaryListAjax(JQueryDataTableParamModel param)
        {
            string ToolGUID = Request["ToolGUID"].ToString();
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

            string parentSearch = string.Empty;
            parentSearch = Convert.ToString(Request["parentSearch"]);

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
            ViewBag.ToolGUID = ToolGUID;
            ToolMasterDAL objToolAPI = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            Guid ToolGUID1 = Guid.Empty;
            Guid.TryParse(ToolGUID, out ToolGUID1);
            var ObjTool = objToolAPI.GetToolByGUIDPlain(ToolGUID1);
            ViewBag.ToolID = ObjTool.ID;
            LocationMasterDAL objAPI = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
            int TotalRecordCount = 0;

            var objModel = objAPI.GetToolBinWiseSummary(ObjTool.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, sortColumnName, false, parentSearch);

            var result = from u in objModel
                         select new ToolAssetQuantityDetailDTO
                         {
                             ID = u.ID,
                             ToolBinID = u.ToolBinID,
                             Quantity = u.Quantity,
                             GUID = u.GUID,
                             CreatedBy = u.CreatedBy,
                             Updated = u.Updated,
                             CreatedByName = u.CreatedByName,
                             UpdatedByName = u.UpdatedByName,
                             RoomID = u.RoomID,
                             CompanyID = u.CompanyID,
                             Location = u.Location,
                             ToolName = u.ToolName,
                             CreatedDt = CommonUtility.ConvertDateByTimeZone(u.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             UpdatedDt = CommonUtility.ConvertDateByTimeZone(u.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             AddedFrom = u.AddedFrom,
                             EditedFrom = u.EditedFrom,
                             ReceivedOn = u.ReceivedOn,
                             ReceivedOnWeb = u.ReceivedOnWeb,
                             LocationID = u.LocationID ?? 0,
                             ToolGUID = u.ToolGUID,
                             IsLocSerialAvailable = u.IsLocSerialAvailable,
                             SerialNumberTracking = u.SerialNumberTracking
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
        public string ToolLocations2(Int64 LocationID, string ToolGUID, string _isSerialAvail = "No", string _searchTerm = "")
        {
            ViewBag.ToolGUID = ToolGUID;
            ViewBag.LocationID = LocationID;

            ViewBag.IsSerialAvail = _isSerialAvail;
            ViewBag.SearchTerm = _searchTerm;

            Guid? ToolAssetOrderDetailGUID = null;

            ToolMasterDAL objToolAPI = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            Guid toolGUID1 = Guid.Empty;
            Guid.TryParse(ToolGUID, out toolGUID1);
            var Objitem = objToolAPI.GetToolByGUIDPlain(toolGUID1);
            ToolLocationDetailsDAL objAPI = new ToolLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            var objModel = objAPI.GetAllRecordsBinWise(LocationID, SessionHelper.RoomID, SessionHelper.CompanyID, Guid.Parse(ToolGUID), ToolAssetOrderDetailGUID, "ID DESC");


            ViewBag.IsSerialNumberTracking = Objitem.SerialNumberTracking;

            ViewBag.ToolName = Objitem.ToolName;

            ViewBag.ToolGUID_ToolType = ToolGUID + "#" + Objitem.Type;



            return RenderRazorViewToString("_ToolLocations", objModel);
        }
        public ActionResult ToolLocationsListAjax(JQueryDataTableParamModel param)
        {
            string ToolGUID = Request["ToolGUID"].ToString();
            Int64 LocationID = Convert.ToInt64(Request["LocationID"].ToString());
            Guid? ToolAssetOrderDetailGUID = null;
            if (!string.IsNullOrEmpty(Request["ToolAssetOrderDetailGUID"]) && Request["ToolAssetOrderDetailGUID"].Trim().Length > 0)
                ToolAssetOrderDetailGUID = Guid.Parse(Request["ToolAssetOrderDetailGUID"]);


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

            string parentLocSearch = Convert.ToString(Request["parentLocSearch"]);

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

            ViewBag.ToolGUID = ToolGUID;
            ToolMasterDAL objToolAPI = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            Guid ToolGUID1 = Guid.Empty;
            Guid.TryParse(ToolGUID, out ToolGUID1);
            var Objitem = objToolAPI.GetToolByGUIDPlain(ToolGUID1);


            ViewBag.ItemID = Objitem.ID;



            ToolLocationDetailsDAL objAPI = new ToolLocationDetailsDAL(SessionHelper.EnterPriseDBName);

            int TotalRecordCount = 0;

            List<ToolAssetQuantityDetailDTO> objModel = objAPI.GetPagedRecords_NoCache(LocationID, param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, Guid.Parse(ToolGUID), ToolAssetOrderDetailGUID, parentLocSearch).ToList();

            var result = from u in objModel.ToList()
                         select new ToolAssetQuantityDetailDTO
                         {
                             ID = u.ID,
                             ToolGUID = u.ToolGUID,
                             GUID = u.GUID,
                             IsArchived = u.IsArchived,
                             IsDeleted = u.IsDeleted,
                             Created = u.Created,
                             Updated = u.Updated,
                             CreatedBy = u.CreatedBy,
                             UpdatedBy = u.UpdatedBy,
                             ReceivedOn = u.ReceivedOn,
                             ReceivedOnWeb = u.ReceivedOnWeb,
                             WhatWhereAction = u.WhatWhereAction,
                             AddedFrom = u.AddedFrom,
                             EditedFrom = u.EditedFrom,
                             RoomID = u.RoomID,
                             CompanyID = u.CompanyID,
                             ToolName = u.ToolName,
                             SerialNumber = u.SerialNumber,

                             LocationID = u.LocationID,
                             Cost = u.Cost,

                             InitialQuantityWeb = u.InitialQuantityWeb,
                             InitialQuantityPDA = u.InitialQuantityPDA,
                             CreatedByName = u.CreatedByName,
                             UpdatedByName = u.UpdatedByName,
                             RoomName = u.RoomName,
                             Quantity = u.Quantity,

                             CreatedDt = CommonUtility.ConvertDateByTimeZone(u.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             UpdatedDt = CommonUtility.ConvertDateByTimeZone(u.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),


                             ReceivedOnDate = CommonUtility.ConvertDateByTimeZone(u.ReceivedOn, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             ReceivedOnDateWeb = CommonUtility.ConvertDateByTimeZone(u.ReceivedOnWeb, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             Description = u.Description,
                             AvailableQuantity = u.AvailableQuantity
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
        public JsonResult GetToolAsset(string ScheduleType)
        {
            List<SelectListItem> lstItem = null;// new List<SelectListItem>();
            try
            {
                if (ScheduleType == "1")
                {
                    AssetMasterDAL objAssetMasterDAL = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
                    IEnumerable<AssetMasterDTO> IEAssetMasterDTO = null;

                    IEAssetMasterDTO = objAssetMasterDAL.GetAllAssetsByRoom(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                    lstItem = new List<SelectListItem>();
                    if (IEAssetMasterDTO != null && IEAssetMasterDTO.Count() > 0)
                    {
                        foreach (var item in IEAssetMasterDTO)
                        {
                            SelectListItem obj = new SelectListItem();
                            obj.Text = item.AssetName;
                            obj.Value = item.GUID.ToString();
                            lstItem.Add(obj);
                        }
                    }
                }
                else if (ScheduleType == "2")
                {
                    ToolMasterDAL objToolMasterDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                    IEnumerable<ToolMasterDTO> IEToolMasterDTO = null;

                    IEToolMasterDTO = objToolMasterDAL.GetToolByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID);
                    lstItem = new List<SelectListItem>();
                    if (IEToolMasterDTO != null && IEToolMasterDTO.Count() > 0)
                    {
                        foreach (var item in IEToolMasterDTO)
                        {
                            SelectListItem obj = new SelectListItem();
                            obj.Text = item.ToolName;
                            obj.Value = item.GUID.ToString();
                            lstItem.Add(obj);
                        }
                    }
                }



                return Json(lstItem, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(lstItem, JsonRequestBehavior.AllowGet);
                throw ex;
            }
            finally
            {
                lstItem = null;
            }
        }
        public string CheckInCheckOutHistoryData(Guid ToolGUID)
        {
            ViewBag.ToolGUID = ToolGUID;
            ToolCheckInOutHistoryDAL objDAL = new ToolCheckInOutHistoryDAL(SessionHelper.EnterPriseDBName);
            //var objModel = objDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.ToolGUID == ToolGUID).ToList();
            var objModel = objDAL.GetTCIOHsByToolGUIDFull(ToolGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
            return RenderRazorViewToString("_CheckInCheckOutHistoryData", objModel);
        }
        public JsonResult GetToolAssetsScheduler(string ScheduleType, Guid? ToolGUID, Guid? AssetGUID)
        {
            List<SelectListItem> lstItem = null;// new List<SelectListItem>();
            try
            {
                int MaintenanceType = 0;
                if (AssetGUID.HasValue)
                {
                    AssetMasterDTO objAssetMasterDTO = new AssetMasterDAL(SessionHelper.EnterPriseDBName).GetRecord(AssetGUID ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                    MaintenanceType = objAssetMasterDTO.MaintenanceType;
                }
                else if (ToolGUID.HasValue)
                {
                    ToolMasterDTO objToolMasterDTO = new ToolMasterDAL(SessionHelper.EnterPriseDBName).GetToolByGUIDPlain(ToolGUID ?? Guid.Empty);
                    MaintenanceType = objToolMasterDTO.MaintenanceType;
                }
                ToolsSchedulerDAL objToolsSchedulerDAL = new ToolsSchedulerDAL(SessionHelper.EnterPriseDBName);
                IEnumerable<ToolsSchedulerDTO> IEToolsSchedulerDTO = null;

                IEToolsSchedulerDTO = objToolsSchedulerDAL.GetAssetToolScheduler(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Convert.ToInt16(ScheduleType));
                lstItem = new List<SelectListItem>();
                if (IEToolsSchedulerDTO != null && IEToolsSchedulerDTO.Count() > 0)
                {
                    foreach (var item in IEToolsSchedulerDTO)
                    {
                        SelectListItem obj = new SelectListItem();
                        obj.Text = item.SchedulerName;
                        obj.Value = item.GUID.ToString();
                        lstItem.Add(obj);
                    }
                }

                return Json(lstItem, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(lstItem, JsonRequestBehavior.AllowGet);
                throw ex;
            }
            finally
            {
                lstItem = null;
            }
        }
        public JsonResult GetTools(string ScheduleType)
        {
            List<SelectListItem> lstItem = null;// new List<SelectListItem>();
            try
            {

                ToolMasterDAL objToolMasterDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                IEnumerable<ToolMasterDTO> IEToolMasterDTO = null;

                IEToolMasterDTO = objToolMasterDAL.GetToolByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID);
                lstItem = new List<SelectListItem>();
                if (IEToolMasterDTO != null && IEToolMasterDTO.Count() > 0)
                {
                    foreach (var item in IEToolMasterDTO)
                    {
                        SelectListItem obj = new SelectListItem();
                        obj.Text = item.ToolName + "(" + item.Serial + ")";
                        obj.Value = item.GUID.ToString();
                        lstItem.Add(obj);
                    }
                }
                return Json(lstItem, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(lstItem, JsonRequestBehavior.AllowGet);
                throw ex;
            }
            finally
            {
                lstItem = null;
            }
        }
        public JsonResult GetToolsScheduler(string ScheduleType, Guid? ToolGUID, Guid? AssetGUID)
        {
            List<SelectListItem> lstItem = null;// new List<SelectListItem>();
            try
            {
                int MaintenanceType = 0;
                if (AssetGUID.HasValue)
                {
                    AssetMasterDTO objAssetMasterDTO = new AssetMasterDAL(SessionHelper.EnterPriseDBName).GetRecord(AssetGUID ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                    MaintenanceType = objAssetMasterDTO.MaintenanceType;
                }
                else if (ToolGUID.HasValue)
                {
                    ToolMasterDTO objToolMasterDTO = new ToolMasterDAL(SessionHelper.EnterPriseDBName).GetToolByGUIDPlain(ToolGUID ?? Guid.Empty);
                    MaintenanceType = objToolMasterDTO.MaintenanceType;
                }
                ToolsSchedulerDAL objToolsSchedulerDAL = new ToolsSchedulerDAL(SessionHelper.EnterPriseDBName);
                IEnumerable<ToolsSchedulerDTO> IEToolsSchedulerDTO = null;

                IEToolsSchedulerDTO = objToolsSchedulerDAL.GetAssetToolScheduler(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Convert.ToInt16(ScheduleType));

                lstItem = new List<SelectListItem>();
                if (IEToolsSchedulerDTO != null && IEToolsSchedulerDTO.Count() > 0)
                {
                    foreach (var item in IEToolsSchedulerDTO)
                    {
                        SelectListItem obj = new SelectListItem();
                        obj.Text = item.SchedulerName;
                        obj.Value = item.GUID.ToString();
                        lstItem.Add(obj);
                    }
                }

                return Json(lstItem, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(lstItem, JsonRequestBehavior.AllowGet);
                throw ex;
            }
            finally
            {
                lstItem = null;
            }
        }
        [HttpPost]
        public JsonResult OdometerSave(ToolsMaintenanceDTO objDTO)
        {
            string message = "";
            string status = "";

            objDTO.MaintenanceDate = DateTime.ParseExact(objDTO.EntryDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult);

            ToolsMaintenanceDAL obj = new ToolsMaintenanceDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCDal = new CommonDAL(SessionHelper.EnterPriseDBName);

            if (string.IsNullOrEmpty(objDTO.MaintenanceDate.ToString()))
            {
                message = string.Format(ResMessage.Required, ResToolsOdometer.Odometer);
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }

            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.MaintenanceName = "Tracking Entry";
            objDTO.MaintenanceType = MaintenanceType.Past.ToString();
            objDTO.SchedulerType = (byte)objDTO.TrackngMeasurement;
            if (objDTO.AssetGUID.HasValue)
            {
                objDTO.ScheduleFor = (byte)MaintenanceScheduleFor.Asset;
            }
            if (objDTO.ToolGUID.HasValue)
            {
                objDTO.ScheduleFor = (byte)MaintenanceScheduleFor.Tool;
            }
            objDTO.Status = Convert.ToString(MaintenanceStatus.Close);
            objDTO.ScheduleDate = objDTO.MaintenanceDate;
            //Extra where condition
            string ExtraWhereCondition = "";
            if (objDTO.AssetGUID != null)
            {
                ExtraWhereCondition = " AssetGUID='" + objDTO.AssetGUID.ToString() + "' ";
            }
            else if (objDTO.ToolGUID != null)
            {
                ExtraWhereCondition = " ToolGUID='" + objDTO.ToolGUID.ToString() + "' ";
            }


            if (objDTO.ID == 0)
            {
                objDTO.CreatedBy = SessionHelper.UserID;

                string strOK = objCDal.DuplicateCheck(objDTO.MaintenanceDate.ToString(), "add", objDTO.ID, "ToolsOdometer", "EntryDate", ExtraWhereCondition, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResToolsOdometer.EntryDate, objDTO.MaintenanceDate);  //"Tool \"" + objDTO.ToolName + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.ID = obj.Insert(objDTO).ID;
                    List<ToolsSchedulerMappingDTO> lstAllSchedules = new List<ToolsSchedulerMappingDTO>();
                    ToolsSchedulerDAL objToolsSchedulerDAL = new ToolsSchedulerDAL(SessionHelper.EnterPriseDBName);
                    lstAllSchedules = objToolsSchedulerDAL.GetAllSchedulesforToolAsset(objDTO.ToolGUID, objDTO.AssetGUID, objDTO.TrackngMeasurement);
                    foreach (var item in lstAllSchedules)
                    {
                        obj.CreateNewMaintenanceAuto(item.AssetGUID, item.ToolGUID, item.ToolSchedulerGuid.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                    }

                    if (objDTO.ID > 0)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg); // "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }
            else
            {
                objDTO.LastUpdatedBy = SessionHelper.UserID;
                string strOK = objCDal.DuplicateCheck(objDTO.MaintenanceDate.ToString(), "edit", objDTO.ID, "ToolsOdometer", "EntryDate", ExtraWhereCondition, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResToolsOdometer.EntryDate, objDTO.MaintenanceDate);  //"Tool \"" + objDTO.ToolName + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    if (obj.Edit(objDTO))
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg); // "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }
            return Json(new { Message = message, Status = status, ID = objDTO.ID }, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetToolLocationsForCheckOut(Guid ToolGuid, string NameStartWith)
        //{
        //    LocationMasterDAL objBinDAL = null;
        //    IEnumerable<LocationMasterDTO> lstBinList = null;
        //    ToolAssetQuantityDetailDAL objLocationQtyDAL = null;
        //    ToolAssetQuantityDetailDTO objLocatQtyDTO = null;
        //    List<LocationMasterDTO> retunList = new List<LocationMasterDTO>();

        //    Int64 RoomID = SessionHelper.RoomID;
        //    Int64 CompanyID = SessionHelper.CompanyID;
        //    string qtyFormat = "N";
        //    List<DTOForAutoComplete> returnKeyValList = new List<DTOForAutoComplete>();
        //    try
        //    {

        //        if (!string.IsNullOrEmpty(SessionHelper.NumberDecimalDigits))
        //            qtyFormat = "N" + SessionHelper.NumberDecimalDigits;
        //        objBinDAL = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
        //        lstBinList = objBinDAL.GetLocationRoomData(RoomID, CompanyID, false, false).ToList();

        //        //lstBinList = objBinDAL.GetItemLocation(RoomID, CompanyID, false, false, ItemGuid, 0, null, null);//.Where(x => x.ItemGUID.GetValueOrDefault(Guid.Empty) == ItemGuid);
        //        foreach (var item in lstBinList)
        //        {
        //            //Staging location condition
        //            //if (item.IsStagingLocation)
        //            //{
        //            //    objMSDAL = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName);
        //            //    lstMSDetailDTO = objMSDAL.GetStagingLocationByItem(ItemGuid, RoomID, CompanyID).Where(x => x.Quantity > 0 && x.StagingBinID == item.ID);
        //            //    if (lstMSDetailDTO != null && lstMSDetailDTO.Count() > 0 && lstMSDetailDTO.Sum(x => x.Quantity) > 0)
        //            //    {
        //            //        DTOForAutoComplete obj = new DTOForAutoComplete()
        //            //        {
        //            //            Key = item.BinNumber,
        //            //            Value = item.BinNumber == "[|EmptyStagingBin|]" ? "[Staging](" + lstMSDetailDTO.Sum(x => x.Quantity) + ")" : item.BinNumber + " [Staging](" + lstMSDetailDTO.Sum(x => x.Quantity) + ")",
        //            //            ID = item.ID,
        //            //            GUID = item.GUID,
        //            //        };
        //            //        returnKeyValList.Add(obj);
        //            //    }
        //            //}
        //            //else
        //            {
        //                objLocationQtyDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
        //                objLocatQtyDTO = objLocationQtyDAL.GetRecordByLocationTool(ToolGuid, item.GUID, RoomID, CompanyID);
        //                if (objLocatQtyDTO != null && objLocatQtyDTO.Quantity > 0)
        //                {
        //                    DTOForAutoComplete obj = new DTOForAutoComplete()
        //                    {
        //                        Key = item.Location,
        //                        Value = item.Location + " (" + objLocatQtyDTO.Quantity.ToString(qtyFormat) + ")",
        //                        ID = objLocatQtyDTO.ToolBinID ?? 0,
        //                        GUID = item.GUID,
        //                    };
        //                    returnKeyValList.Add(obj);
        //                }
        //            }
        //        }

        //        if (returnKeyValList.Count > 0 && !string.IsNullOrEmpty(NameStartWith.Trim()) && !string.IsNullOrWhiteSpace(NameStartWith))
        //        {
        //            returnKeyValList = returnKeyValList.Where(x => x.Key.ToLower().StartsWith(NameStartWith.ToLower().Trim())).ToList();
        //        }

        //        return Json(returnKeyValList, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception)
        //    {
        //        return Json(returnKeyValList, JsonRequestBehavior.AllowGet);
        //    }
        //    finally
        //    {
        //        objBinDAL = null;
        //        lstBinList = null;
        //        retunList = null;
        //        objLocationQtyDAL = null;
        //        objLocatQtyDTO = null;

        //    }
        //}
        public JsonResult GetToolLocationsForCheckOut(Guid ToolGuid, string NameStartWith)
        {
            LocationMasterDAL objBinDAL = null;
            ToolAssetQuantityDetailDAL objLocationQtyDAL = null;
            List<ToolAssetQuantityDetailDTO> lstLocatQtyDTO = null;
            List<LocationMasterDTO> retunList = new List<LocationMasterDTO>();

            Int64 RoomID = SessionHelper.RoomID;
            Int64 CompanyID = SessionHelper.CompanyID;
            string qtyFormat = "N";
            List<DTOForAutoComplete> returnKeyValList = new List<DTOForAutoComplete>();
            try
            {

                if (!string.IsNullOrEmpty(SessionHelper.NumberDecimalDigits))
                    qtyFormat = "N" + SessionHelper.NumberDecimalDigits;
                objBinDAL = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
                // lstBinList = objBinDAL.GetLocationRoomData(RoomID, CompanyID, false, false).ToList();

                //lstBinList = objBinDAL.GetItemLocation(RoomID, CompanyID, false, false, ItemGuid, 0, null, null);//.Where(x => x.ItemGUID.GetValueOrDefault(Guid.Empty) == ItemGuid);
                //foreach (var item in lstBinList)
                {

                    {
                        objLocationQtyDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
                        lstLocatQtyDTO = objLocationQtyDAL.GetRecordByLocationToolAll(ToolGuid, RoomID, CompanyID);
                        foreach (ToolAssetQuantityDetailDTO objLocatQtyDTO in lstLocatQtyDTO)
                        {
                            if (objLocatQtyDTO != null && objLocatQtyDTO.Quantity > 0)
                            {
                                DTOForAutoComplete obj = new DTOForAutoComplete()
                                {
                                    Key = objLocatQtyDTO.Location,
                                    Value = objLocatQtyDTO.Location + " (" + objLocatQtyDTO.Quantity.ToString(qtyFormat) + ")",
                                    ID = objLocatQtyDTO.ToolBinID ?? 0,
                                    GUID = objLocatQtyDTO.LocationGUID ?? Guid.Empty,
                                };
                                returnKeyValList.Add(obj);
                            }
                        }
                    }
                }

                if (returnKeyValList.Count > 0 && !string.IsNullOrEmpty(NameStartWith.Trim()) && !string.IsNullOrWhiteSpace(NameStartWith))
                {
                    returnKeyValList = returnKeyValList.Where(x => x.Key.ToLower().StartsWith(NameStartWith.ToLower().Trim())).ToList();
                }

                return Json(returnKeyValList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(returnKeyValList, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                objBinDAL = null;
                retunList = null;
                objLocationQtyDAL = null;
                lstLocatQtyDTO = null;

            }
        }
        #endregion
        public ActionResult CheckOutPullQuantity(List<ToolAssetPullMasterDTO> lstCheckOutRequestInfo)
        {
            List<ToolAssetPullMasterDTO> lstPullRequest = new List<ToolAssetPullMasterDTO>();
            foreach (ToolAssetPullMasterDTO objPullMasterDTO in lstCheckOutRequestInfo)
            {
                if (!lstPullRequest.Select(x => x.ToolGUID).Contains(objPullMasterDTO.ToolGUID))
                    lstPullRequest.Add(objPullMasterDTO);
            }

            ToolAssetCICOTransactionDAL objPullMasterDAL = new ToolAssetCICOTransactionDAL(SessionHelper.EnterPriseDBName);
            lstCheckOutRequestInfo = objPullMasterDAL.GetPullWithDetails(lstPullRequest, SessionHelper.RoomID, SessionHelper.CompanyID);
            return PartialView("ToolLotSrSelection", lstCheckOutRequestInfo);
        }
        public ActionResult PullToolLotSrSelection(JQueryDataTableParamModel param)
        {

            Guid ToolGUID = Guid.Empty;
            long BinID = 0;
            double PullQuantity = 0;

            Guid.TryParse(Convert.ToString(Request["ToolGUID"]), out ToolGUID);

            long.TryParse(Convert.ToString(Request["BinID"]), out BinID);
            double.TryParse(Convert.ToString(Request["PullQuantity"]), out PullQuantity);
            string InventoryConsuptionMethod = Convert.ToString(Request["InventoryConsuptionMethod"]);
            string CurrentLoaded = Convert.ToString(Request["CurrentLoaded"]);
            string ViewRight = Convert.ToString(Request["ViewRight"]);
            bool IsDeleteRowMode = Convert.ToBoolean(Request["IsDeleteRowMode"]);

            string[] arrIds = new string[] { };

            if (!string.IsNullOrWhiteSpace(CurrentLoaded))
            {
                arrIds = CurrentLoaded.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }
            ToolMasterDTO oItem = null;
            LocationMasterDTO objLocDTO = null;
            bool IsSerialTypeTool = false;
            if (ToolGUID != Guid.Empty)
            {
                oItem = new ToolMasterDAL(SessionHelper.EnterPriseDBName).GetToolByGUIDPlain(ToolGUID);
                objLocDTO = new LocationMasterDAL(SessionHelper.EnterPriseDBName).GetLocationByIDPlain(BinID, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (oItem != null && oItem.SerialNumberTracking)
                {
                    IsSerialTypeTool = true;
                }
            }

            int TotalRecordCount = 0;
            ToolAssetCICOTransactionDAL objPullDetails = new ToolAssetCICOTransactionDAL(SessionHelper.EnterPriseDBName);
            List<ToolQuantityLotSerialDTO> lstLotSrs = new List<ToolQuantityLotSerialDTO>();
            List<ToolQuantityLotSerialDTO> retlstLotSrs = new List<ToolQuantityLotSerialDTO>();
            Dictionary<string, double> dicSerialLots = new Dictionary<string, double>();
            string[] arrItem;



            {
                if (arrIds.Count() > 0)
                {
                    string[] arrSerialLots = new string[arrIds.Count()];
                    for (int i = 0; i < arrIds.Count(); i++)
                    {
                        if ((IsSerialTypeTool))
                        {
                            //string[] arrItem = arrIds[i].Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                            arrItem = new string[2];
                            arrItem[0] = arrIds[i].Substring(0, arrIds[i].LastIndexOf("_"));
                            arrItem[1] = arrIds[i].Replace(arrItem[0] + "_", "");
                            if (arrItem.Length > 1)
                            {
                                arrSerialLots[i] = arrItem[0];
                                dicSerialLots.Add(arrItem[0], Convert.ToDouble(arrItem[1]));
                            }
                        }
                        //else if ((oItem.SerialNumberTracking && oItem.DateCodeTracking)
                        //    || (oItem.LotNumberTracking && oItem.DateCodeTracking))
                        //{
                        //    arrItem = arrIds[i].Split('_');
                        //    if (arrItem.Length > 1)
                        //    {
                        //        arrSerialLots[i] = arrItem[0] + "_" + arrItem[1];
                        //        dicSerialLots.Add(arrItem[0], Convert.ToDouble(arrItem[2]));
                        //    }
                        //}
                        //else if (!oItem.SerialNumberTracking && !oItem.DateCodeTracking && oItem.DateCodeTracking)
                        //{
                        //    arrItem = arrIds[i].Split('_');
                        //    if (arrItem.Length > 1)
                        //    {
                        //        arrSerialLots[i] = arrItem[0];
                        //        dicSerialLots.Add(arrItem[0], Convert.ToDouble(arrItem[1]));
                        //    }
                        //}
                        else
                        {
                            arrItem = arrIds[i].Split('_');
                            if (arrItem.Length > 1)
                            {
                                arrSerialLots[i] = arrItem[0];
                                dicSerialLots.Add(arrItem[0], Convert.ToDouble(arrItem[1]));
                            }
                        }
                    }

                    lstLotSrs = objPullDetails.GetToolLocationsWithLotSerialsForPull(ToolGUID, SessionHelper.RoomID, SessionHelper.CompanyID, BinID, PullQuantity, "0", string.Empty);
                    retlstLotSrs = lstLotSrs.Where(t =>
                        (
                            (
                                arrSerialLots.Contains(t.LotOrSerailNumber, StringComparer.OrdinalIgnoreCase)
                                && (IsSerialTypeTool)
                                 )
                            ||
                            (
                                arrSerialLots.Contains(t.SerialLotExpirationcombin)
                                && (IsSerialTypeTool)
                                )
                        ||
                            (
                                arrSerialLots.Contains(t.SerialLotExpirationcombin)
                                && (!(IsSerialTypeTool))
                                )
                        || (arrSerialLots.Contains(t.Location) && !(IsSerialTypeTool)))).ToList();

                    if (!IsDeleteRowMode)
                    {
                        if (ViewRight == "NoRight" && (IsSerialTypeTool))
                        {
                            ToolQuantityLotSerialDTO oLotSr = new ToolQuantityLotSerialDTO();
                            oLotSr.BinID = BinID;
                            oLotSr.ID = BinID;
                            oLotSr.ToolGUID = ToolGUID;
                            oLotSr.LotOrSerailNumber = string.Empty;
                            oLotSr.Expiration = string.Empty;
                            oLotSr.Location = string.Empty;

                            if (objLocDTO != null && objLocDTO.ID > 0)
                            {
                                oLotSr.Location = objLocDTO.Location;
                            }
                            if (IsSerialTypeTool)
                            {
                                oLotSr.PullQuantity = 1;
                            }
                            //    oLotSr.LotNumberTracking = oItem.LotNumberTracking;
                            oLotSr.SerialNumberTracking = IsSerialTypeTool ? true : false;
                            //   oLotSr.DateCodeTracking = oItem.DateCodeTracking;
                            retlstLotSrs.Add(oLotSr);
                        }
                        else
                        {
                            //retlstLotSrs = retlstLotSrs.Union(lstLotSrs.Where(t =>
                            //    ((!arrSerialLots.Contains(t.LotOrSerailNumber) && (oItem.SerialNumberTracking || oItem.LotNumberTracking))
                            //|| (!arrSerialLots.Contains(t.SerialLotExpirationcombin) && oItem.DateCodeTracking)
                            //|| (!arrSerialLots.Contains(t.BinNumber) && !oItem.SerialNumberTracking && !oItem.LotNumberTracking && !oItem.DateCodeTracking))).Take(1)).ToList();

                            retlstLotSrs =
                                retlstLotSrs.Union
                                (
                                    lstLotSrs.Where(t =>
                                  (
                                        (
                                            !arrSerialLots.Contains(t.LotOrSerailNumber, StringComparer.OrdinalIgnoreCase)
                                            && (IsSerialTypeTool)

                                        )
                                    ||
                                        (
                                            !arrSerialLots.Contains(t.SerialLotExpirationcombin)
                                            && (IsSerialTypeTool)

                                        )
                                    ||
                                        (
                                            !arrSerialLots.Contains(t.SerialLotExpirationcombin)
                                            && (!(IsSerialTypeTool))

                                        )
                                    ||
                                        (
                                            !arrSerialLots.Contains(t.Location)
                                            && !(IsSerialTypeTool)
                                         //&& !oItem.LotNumberTracking
                                         //&& !oItem.DateCodeTracking
                                         )
                                 )).Take(1)
                              ).ToList();

                        }
                    }
                }
                else
                {
                    if (ViewRight == "NoRight" && (IsSerialTypeTool))
                    {
                        ToolQuantityLotSerialDTO oLotSr = new ToolQuantityLotSerialDTO();
                        oLotSr.BinID = BinID;
                        oLotSr.ID = BinID;
                        oLotSr.ToolGUID = ToolGUID;
                        oLotSr.LotOrSerailNumber = string.Empty;
                        oLotSr.Expiration = string.Empty;
                        oLotSr.Location = string.Empty;

                        if (objLocDTO != null && objLocDTO.ID > 0)
                        {
                            oLotSr.Location = objLocDTO.Location;
                        }
                        if (IsSerialTypeTool)
                        {
                            oLotSr.PullQuantity = 1;

                        }
                        //oLotSr.LotNumberTracking = oItem.LotNumberTracking;
                        oLotSr.SerialNumberTracking = IsSerialTypeTool ? true : false;
                        // oLotSr.DateCodeTracking = oItem.DateCodeTracking;

                        retlstLotSrs.Add(oLotSr);
                    }
                    else
                        retlstLotSrs = objPullDetails.GetToolLocationsWithLotSerialsForPull(ToolGUID, SessionHelper.RoomID, SessionHelper.CompanyID, BinID, PullQuantity, "1", string.Empty);
                }

                foreach (ToolQuantityLotSerialDTO item in retlstLotSrs)
                {
                    if (dicSerialLots.ContainsKey(item.LotOrSerailNumber) && (IsSerialTypeTool))
                    {
                        double value = dicSerialLots[item.LotOrSerailNumber];
                        item.PullQuantity = value;
                        PullQuantity -= item.PullQuantity;
                    }
                    else if (dicSerialLots.ContainsKey(item.Expiration ?? string.Empty) && false) //oItem.DateCodeTracking
                    {
                        double value = dicSerialLots[item.Expiration];
                        item.PullQuantity = value;
                        PullQuantity -= item.PullQuantity;
                    }
                    else if (dicSerialLots.ContainsKey(item.Location) && !(IsSerialTypeTool))
                    {
                        double value = dicSerialLots[item.Location];
                        item.PullQuantity = value;
                        PullQuantity -= item.PullQuantity;
                    }
                    else if (item.PullQuantity <= PullQuantity)
                    {
                        PullQuantity -= item.PullQuantity;
                    }
                    else if (PullQuantity >= 0)
                    {
                        item.PullQuantity = PullQuantity;
                        PullQuantity = 0;
                    }
                    else
                    {
                        item.PullQuantity = 0;
                    }
                    if (item.ExpirationDate != null && item.ExpirationDate.HasValue && item.ExpirationDate.Value != DateTime.MinValue)
                    {
                        item.Expiration = FnCommon.ConvertDateByTimeZone(item.ExpirationDate.Value, true, true);
                    }
                    if (item.ReceivedDate != null && item.ReceivedDate.HasValue && item.ReceivedDate.Value != DateTime.MinValue)
                    {
                        item.Received = FnCommon.ConvertDateByTimeZone(item.ReceivedDate.Value, true, true);
                    }
                    if (item.PullQuantity > 0)
                        item.IsSelected = true;
                }
            }

            if (!(ViewRight == "NoRight" && (IsSerialTypeTool)))//|| oItem.LotNumberTracking
                retlstLotSrs = retlstLotSrs.Where(x => x.PullQuantity > 0).ToList();

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = retlstLotSrs
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PullToolLotSrSelectionOut(JQueryDataTableParamModel param)
        {

            Guid ToolGUID = Guid.Empty;
            long BinID = 0;
            double PullQuantity = 0;

            Guid.TryParse(Convert.ToString(Request["ToolGUID"]), out ToolGUID);

            long.TryParse(Convert.ToString(Request["BinID"]), out BinID);
            double.TryParse(Convert.ToString(Request["PullQuantity"]), out PullQuantity);
            string InventoryConsuptionMethod = Convert.ToString(Request["InventoryConsuptionMethod"]);
            string CurrentLoaded = Convert.ToString(Request["CurrentLoaded"]);
            string ViewRight = Convert.ToString(Request["ViewRight"]);
            bool IsDeleteRowMode = Convert.ToBoolean(Request["IsDeleteRowMode"]);

            string[] arrIds = new string[] { };

            if (!string.IsNullOrWhiteSpace(CurrentLoaded))
            {
                arrIds = CurrentLoaded.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }
            ToolMasterDTO oItem = null;
            LocationMasterDTO objLocDTO = null;
            bool IsSerialTypeTool = false;
            if (ToolGUID != Guid.Empty)
            {
                oItem = new ToolMasterDAL(SessionHelper.EnterPriseDBName).GetToolByGUIDPlain(ToolGUID);
                objLocDTO = new LocationMasterDAL(SessionHelper.EnterPriseDBName).GetLocationByIDPlain(BinID, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (oItem != null && oItem.SerialNumberTracking)
                {
                    IsSerialTypeTool = true;
                }

            }

            int TotalRecordCount = 0;
            ToolAssetCICOTransactionDAL objPullDetails = new ToolAssetCICOTransactionDAL(SessionHelper.EnterPriseDBName);
            List<ToolQuantityLotSerialDTO> lstLotSrs = new List<ToolQuantityLotSerialDTO>();
            List<ToolQuantityLotSerialDTO> retlstLotSrs = new List<ToolQuantityLotSerialDTO>();
            Dictionary<string, double> dicSerialLots = new Dictionary<string, double>();
            string[] arrItem;



            {
                if (arrIds.Count() > 0)
                {
                    string[] arrSerialLots = new string[arrIds.Count()];
                    for (int i = 0; i < arrIds.Count(); i++)
                    {
                        if ((IsSerialTypeTool))
                        {

                            arrItem = new string[2];
                            arrItem[0] = arrIds[i].Substring(0, arrIds[i].LastIndexOf("_"));
                            arrItem[1] = arrIds[i].Replace(arrItem[0] + "_", "");
                            if (arrItem.Length > 1)
                            {
                                arrSerialLots[i] = arrItem[0];
                                dicSerialLots.Add(arrItem[0], Convert.ToDouble(arrItem[1]));
                            }
                        }

                        else
                        {
                            arrItem = arrIds[i].Split('_');
                            if (arrItem.Length > 1)
                            {
                                arrSerialLots[i] = arrItem[0];
                                dicSerialLots.Add(arrItem[0], Convert.ToDouble(arrItem[1]));
                            }
                        }
                    }

                    lstLotSrs = objPullDetails.GetToolLocationsWithLotSerialsForPull(ToolGUID, SessionHelper.RoomID, SessionHelper.CompanyID, BinID, PullQuantity, "0", string.Empty);
                    retlstLotSrs = lstLotSrs.Where(t =>
                        (
                            (
                                arrSerialLots.Contains(t.LotOrSerailNumber, StringComparer.OrdinalIgnoreCase)
                                && (IsSerialTypeTool)
                                 )
                            ||
                            (
                                arrSerialLots.Contains(t.SerialLotExpirationcombin)
                                && (IsSerialTypeTool)
                                )
                        ||
                            (
                                arrSerialLots.Contains(t.SerialLotExpirationcombin)
                                && (!(IsSerialTypeTool))
                                )
                        || (arrSerialLots.Contains(t.Location) && !(IsSerialTypeTool)))).ToList();

                    if (!IsDeleteRowMode)
                    {
                        if (ViewRight == "NoRight" && (IsSerialTypeTool))
                        {
                            ToolQuantityLotSerialDTO oLotSr = new ToolQuantityLotSerialDTO();
                            oLotSr.BinID = BinID;
                            oLotSr.ID = BinID;
                            oLotSr.ToolGUID = ToolGUID;
                            oLotSr.LotOrSerailNumber = string.Empty;
                            oLotSr.Expiration = string.Empty;
                            oLotSr.Location = string.Empty;

                            if (objLocDTO != null && objLocDTO.ID > 0)
                            {
                                oLotSr.Location = objLocDTO.Location;
                            }
                            if (IsSerialTypeTool)
                            {
                                oLotSr.PullQuantity = 1;
                            }

                            oLotSr.SerialNumberTracking = IsSerialTypeTool ? true : false;

                            retlstLotSrs.Add(oLotSr);
                        }
                        else
                        {

                            retlstLotSrs =
                                retlstLotSrs.Union
                                (
                                    lstLotSrs.Where(t =>
                                  (
                                        (
                                            !arrSerialLots.Contains(t.LotOrSerailNumber, StringComparer.OrdinalIgnoreCase)
                                            && (IsSerialTypeTool)

                                        )
                                    ||
                                        (
                                            !arrSerialLots.Contains(t.SerialLotExpirationcombin)
                                            && (IsSerialTypeTool)

                                        )
                                    ||
                                        (
                                            !arrSerialLots.Contains(t.SerialLotExpirationcombin)
                                            && (!(IsSerialTypeTool))

                                        )
                                    ||
                                        (
                                            !arrSerialLots.Contains(t.Location)
                                            && !(IsSerialTypeTool)

                                         )
                                 )).Take(1)
                              ).ToList();

                        }
                    }
                }
                else
                {
                    if (ViewRight == "NoRight" && (IsSerialTypeTool))
                    {
                        ToolQuantityLotSerialDTO oLotSr = new ToolQuantityLotSerialDTO();
                        oLotSr.BinID = BinID;
                        oLotSr.ID = BinID;
                        oLotSr.ToolGUID = ToolGUID;
                        oLotSr.LotOrSerailNumber = string.Empty;
                        oLotSr.Expiration = string.Empty;
                        oLotSr.Location = string.Empty;

                        if (objLocDTO != null && objLocDTO.ID > 0)
                        {
                            oLotSr.Location = objLocDTO.Location;
                        }
                        if (IsSerialTypeTool)
                        {
                            oLotSr.PullQuantity = 1;

                        }

                        oLotSr.SerialNumberTracking = IsSerialTypeTool ? true : false;

                        retlstLotSrs.Add(oLotSr);
                    }
                    else
                        retlstLotSrs = objPullDetails.GetToolLocationsWithLotSerialsForOut(ToolGUID, SessionHelper.RoomID, SessionHelper.CompanyID, BinID, PullQuantity, "1", string.Empty);
                }

                foreach (ToolQuantityLotSerialDTO item in retlstLotSrs)
                {
                    if (dicSerialLots.ContainsKey(item.LotOrSerailNumber) && (IsSerialTypeTool))
                    {
                        double value = dicSerialLots[item.LotOrSerailNumber];
                        item.PullQuantity = value;
                        PullQuantity -= item.PullQuantity;
                    }
                    else if (dicSerialLots.ContainsKey(item.Expiration ?? string.Empty) && false) //oItem.DateCodeTracking
                    {
                        double value = dicSerialLots[item.Expiration];
                        item.PullQuantity = value;
                        PullQuantity -= item.PullQuantity;
                    }
                    else if (dicSerialLots.ContainsKey(item.Location) && !(IsSerialTypeTool))
                    {
                        double value = dicSerialLots[item.Location];
                        item.PullQuantity = value;
                        PullQuantity -= item.PullQuantity;
                    }
                    else if (item.PullQuantity <= PullQuantity)
                    {
                        PullQuantity -= item.PullQuantity;
                    }
                    else if (PullQuantity >= 0)
                    {
                        item.PullQuantity = PullQuantity;
                        PullQuantity = 0;
                    }
                    else
                    {
                        item.PullQuantity = 0;
                    }
                    if (item.ExpirationDate != null && item.ExpirationDate.HasValue && item.ExpirationDate.Value != DateTime.MinValue)
                    {
                        item.Expiration = FnCommon.ConvertDateByTimeZone(item.ExpirationDate.Value, true, true);
                    }
                    if (item.ReceivedDate != null && item.ReceivedDate.HasValue && item.ReceivedDate.Value != DateTime.MinValue)
                    {
                        item.Received = FnCommon.ConvertDateByTimeZone(item.ReceivedDate.Value, true, true);
                    }
                    if (item.PullQuantity > 0)
                        item.IsSelected = true;
                }
            }

            if (!(ViewRight == "NoRight" && (IsSerialTypeTool)))//|| oItem.LotNumberTracking
                retlstLotSrs = retlstLotSrs.ToList();

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = retlstLotSrs
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult PullToolSerialsAndLotsNew(List<ToolAssetPullInfo> objItemPullInfo)
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



                            //if (objToolMasterDTO.Type != 4)
                            {



                                if (item.lstToolPullDetails != null && item.lstToolPullDetails.Count > 0)
                                {
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
                                    Checkout(oItemPullInfo);
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
                    //RequisitionItemsToPull obj = new RequisitionItemsToPull()
                    //{
                    //    ToolGUID = item.ToolGUID,
                    //    RequisitionDetailGUID = item.RequisitionDetailsGUID.GetValueOrDefault(Guid.Empty).ToString(),
                    //    RequisitionMasterGUID = objReqDetailsDTO.RequisitionGUID.GetValueOrDefault(Guid.Empty).ToString(),
                    //    PullCreditQuantity = item.PullQuantity,
                    //    PullCredit = "checkout",
                    //    TechnicianGUID = item.TechnicianGUID,
                    //    TempPullQTY = item.PullQuantity,
                    //    ToolCheckoutUDF1 = item.ToolCheckoutUDF1,
                    //    ToolCheckoutUDF2 = item.ToolCheckoutUDF2,
                    //    ToolCheckoutUDF3 = item.ToolCheckoutUDF3,
                    //    ToolCheckoutUDF4 = item.ToolCheckoutUDF4,
                    //    ToolCheckoutUDF5 = item.ToolCheckoutUDF5,
                    //};
                    //JsonResult repsonse = RequisitionToolCheckout(obj);
                    //JavaScriptSerializer serializer = new JavaScriptSerializer();
                    //ReqPullAllJsonResponse errMsg = serializer.Deserialize<ReqPullAllJsonResponse>(serializer.Serialize(repsonse.Data));
                    //ToolMasterDAL toolDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                    //ToolMasterDTO toolDTO = toolDAL.GetRecord(0, SessionHelper.RoomID, SessionHelper.CompanyID, item.ToolGUID.GetValueOrDefault(Guid.Empty));
                    //errMsg.ItemNumber = toolDTO.ToolName;
                    //if (errMsg.Message != "ok")
                    //{
                    //    item.ErrorMessage = toolDTO.ToolName + ": " + errMsg.Message;
                    //    item.ToolName = toolDTO.ToolName;
                    //}
                    //else
                    //{
                    //    item.ErrorMessage = "";
                    //}
                    //item.ErrorList = new List<PullErrorInfo>();
                    //oReturn.Add(item);
                }
            }

            return Json(oReturn);


        }
        [HttpPost]
        public JsonResult ValidateSerialLotNumberTool(Guid? ToolGuid, string SerialOrLotNumber, long BinID)
        {
            if (!string.IsNullOrWhiteSpace(SerialOrLotNumber))
            {
                SerialOrLotNumber = SerialOrLotNumber.Trim();
            }
            ToolAssetCICOTransactionDAL objPullDetails = new ToolAssetCICOTransactionDAL(SessionHelper.EnterPriseDBName);
            ToolQuantityLotSerialDTO objItemLocationLotSerialDTO = objPullDetails.GetToolLocationsWithLotSerialsForPull(ToolGuid ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, BinID, 0, "0", SerialOrLotNumber).FirstOrDefault();

            if (objItemLocationLotSerialDTO == null)
            {
                objItemLocationLotSerialDTO = new ToolQuantityLotSerialDTO();
                objItemLocationLotSerialDTO.BinID = BinID;
                objItemLocationLotSerialDTO.ID = BinID;
                objItemLocationLotSerialDTO.ToolGUID = ToolGuid;
                objItemLocationLotSerialDTO.LotOrSerailNumber = string.Empty;
                objItemLocationLotSerialDTO.Expiration = string.Empty;
                objItemLocationLotSerialDTO.Location = string.Empty;
            }
            return Json(objItemLocationLotSerialDTO);
        }
        public ToolAssetPullInfo Checkout(ToolAssetPullInfo oToolAssetPullInfo)
        {
            if (oToolAssetPullInfo != null)
            {
                ToolMasterDAL objToolMasterDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                RoomDAL objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
                int CheckOutQty = 0;
                CheckOutQty = Convert.ToInt32(oToolAssetPullInfo.lstToolPullDetails.Sum(t => t.TobePulled));
                bool IsSerialNumberTracking = oToolAssetPullInfo.lstToolPullDetails[0].SerialNumberTracking;

                if (!IsSerialNumberTracking)
                {
                    CheckOutCheckIn("co", CheckOutQty, oToolAssetPullInfo.IsMaintenance, oToolAssetPullInfo.ToolGUID ?? Guid.Empty, 0, 0, 0, oToolAssetPullInfo.ToolCheckoutUDF1, oToolAssetPullInfo.ToolCheckoutUDF2, oToolAssetPullInfo.ToolCheckoutUDF3, oToolAssetPullInfo.ToolCheckoutUDF4, oToolAssetPullInfo.ToolCheckoutUDF5, string.Empty, true, oToolAssetPullInfo.Technician, oToolAssetPullInfo.RequisitionDetailsGUID, oToolAssetPullInfo.WorkOrderItemGUID, string.Empty, oToolAssetPullInfo.BinID);

                }
                if (IsSerialNumberTracking)
                {
                    oToolAssetPullInfo.lstToolPullDetails.ForEach(t =>

                       CheckOutCheckIn("co", Convert.ToInt32(t.Quantity ?? 0), oToolAssetPullInfo.IsMaintenance, oToolAssetPullInfo.ToolGUID ?? Guid.Empty, 0, 0, 0, oToolAssetPullInfo.ToolCheckoutUDF1, oToolAssetPullInfo.ToolCheckoutUDF2, oToolAssetPullInfo.ToolCheckoutUDF3, oToolAssetPullInfo.ToolCheckoutUDF4, oToolAssetPullInfo.ToolCheckoutUDF5, string.Empty, true, oToolAssetPullInfo.Technician, oToolAssetPullInfo.RequisitionDetailsGUID, oToolAssetPullInfo.WorkOrderItemGUID, t.SerialNumber, oToolAssetPullInfo.BinID)
                    );
                }
            }
            return oToolAssetPullInfo;

        }

        public JsonResult GetLotOrSerailNumberList(int maxRows, string name_startsWith, Guid? ToolGuid, long BinID, string prmSerialLotNos = null)
        {
            ToolAssetCICOTransactionDAL objPullDetails = new ToolAssetCICOTransactionDAL(SessionHelper.EnterPriseDBName);
            List<ToolQuantityLotSerialDTO> objToolQuantityLotSerialDTO = objPullDetails.GetToolLocationsWithLotSerialsForPull(ToolGuid ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, BinID, 0, "0", String.Empty);

            string[] arrSerialLotNos = prmSerialLotNos.Split(new string[] { "|#|" }, StringSplitOptions.RemoveEmptyEntries);
            if (!string.IsNullOrWhiteSpace(name_startsWith))
            {
                name_startsWith = name_startsWith.Trim();
            }
            var lstLotSr =
                    objToolQuantityLotSerialDTO.Where(x => (x.LotOrSerailNumber.Contains(name_startsWith) && !arrSerialLotNos.Contains(x.LotOrSerailNumber))).Select(x => new { x.LotOrSerailNumber }).Distinct();

            if (lstLotSr.Count() == 0)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            return Json(lstLotSr, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ValidateSerialLotNumber(Guid? ToolGuid, string SerialOrLotNumber, long BinID)
        {
            if (!string.IsNullOrWhiteSpace(SerialOrLotNumber))
            {
                SerialOrLotNumber = SerialOrLotNumber.Trim();
            }
            ToolAssetCICOTransactionDAL objPullDetails = new ToolAssetCICOTransactionDAL(SessionHelper.EnterPriseDBName);
            ToolQuantityLotSerialDTO objItemLocationLotSerialDTO = objPullDetails.GetToolLocationsWithLotSerialsForPull(ToolGuid ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, BinID, 0, "0", SerialOrLotNumber).FirstOrDefault();

            if (objItemLocationLotSerialDTO == null)
            {
                objItemLocationLotSerialDTO = new ToolQuantityLotSerialDTO();
                objItemLocationLotSerialDTO.BinID = BinID;
                objItemLocationLotSerialDTO.ID = BinID;
                objItemLocationLotSerialDTO.ToolGUID = ToolGuid;
                objItemLocationLotSerialDTO.LotOrSerailNumber = string.Empty;
                objItemLocationLotSerialDTO.Expiration = string.Empty;
                objItemLocationLotSerialDTO.Location = string.Empty;
            }
            return Json(objItemLocationLotSerialDTO);
        }
        private ToolAssetPullInfo ValidateLotAndSerial(ToolAssetPullInfo objToolPullInfo)
        {
            ToolMasterDAL objToolMasterDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            ToolMasterDTO objTool = objToolMasterDAL.GetToolByGUIDPlain(objToolPullInfo.ToolGUID ?? Guid.Empty);
            double? _PullCost = null;
            if (objToolPullInfo.RequisitionDetailsGUID != null && objToolPullInfo.RequisitionDetailsGUID != Guid.Empty)
            {
                _PullCost = objToolMasterDAL.GetToolPriceByRoomModuleSettings(SessionHelper.CompanyID, SessionHelper.RoomID, (long)eTurnsWeb.Helper.SessionHelper.ModuleList.Requisitions, (Guid)objToolPullInfo.ToolGUID, false);
            }
            else if (objToolPullInfo.WorkOrderDetailGUID != null && objToolPullInfo.WorkOrderDetailGUID != Guid.Empty)
            {
                _PullCost = objToolMasterDAL.GetToolPriceByRoomModuleSettings(SessionHelper.CompanyID, SessionHelper.RoomID, (long)eTurnsWeb.Helper.SessionHelper.ModuleList.WorkOrders, (Guid)objToolPullInfo.ToolGUID, false);
            }



            #region UDF validation
            UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetRequiredUDFsByUDFTableNamePlain("ToolCheckInOutHistory", objToolPullInfo.RoomId, objToolPullInfo.CompanyId);
            string udfRequier = string.Empty;
            
            foreach (var i in DataFromDB)
            {
                    bool UDFIsRequired = false;

                    if (i.UDFColumnName == "UDF1"  && string.IsNullOrEmpty(objToolPullInfo.ToolCheckoutUDF1))
                        UDFIsRequired = true;
                    else if (i.UDFColumnName == "UDF2"  && string.IsNullOrEmpty(objToolPullInfo.ToolCheckoutUDF2))
                        UDFIsRequired = true;
                    else if (i.UDFColumnName == "UDF3"  && string.IsNullOrEmpty(objToolPullInfo.ToolCheckoutUDF3))
                        UDFIsRequired = true;
                    else if (i.UDFColumnName == "UDF4"  && string.IsNullOrEmpty(objToolPullInfo.ToolCheckoutUDF4))
                        UDFIsRequired = true;
                    else if (i.UDFColumnName == "UDF5"  && string.IsNullOrEmpty(objToolPullInfo.ToolCheckoutUDF5))
                        UDFIsRequired = true;

                    if (UDFIsRequired)
                    {
                        string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                        string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                        if (!string.IsNullOrEmpty(val))
                            i.UDFDisplayColumnName = val;
                        else
                            i.UDFDisplayColumnName = i.UDFColumnName;

                        if (string.IsNullOrWhiteSpace(udfRequier))
                            udfRequier = objTool.ToolName + ": ";
                        udfRequier += (string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName) + " ");
                    }
                
            }

            if (!string.IsNullOrEmpty(udfRequier))
            {
                objToolPullInfo.ErrorList.Add(new PullToolAssetErrorInfo() { ErrorCode = "6", ErrorMessage = udfRequier });
            }
            #endregion

            #region Requisition validation
            if (objToolPullInfo.RequisitionDetailsGUID.HasValue && objToolPullInfo.RequisitionDetailsGUID != Guid.Empty)
            {
                RequisitionDetailsDTO objRequisitionDetail = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName).GetRequisitionDetailsByGUIDPlain(objToolPullInfo.RequisitionDetailsGUID ?? Guid.Empty);
                //RequisitionDetail objRequisitionDetail = context.RequisitionDetails.FirstOrDefault(t => t.GUID == objItemPullInfo.RequisitionDetailsGUID);
                if (objRequisitionDetail != null)
                {
                    if (objRequisitionDetail != null && ((objRequisitionDetail.QuantityApproved ?? 0) < ((objToolPullInfo.PullQuantity) + (objRequisitionDetail.QuantityPulled ?? 0))))
                    {
                        objToolPullInfo.ErrorList.Add(new PullToolAssetErrorInfo() { ErrorCode = "5", ErrorMessage = objTool.ToolName + ": " + ResPullMaster.msgReqPullGreaterApproved });
                    }
                }
            }
            #endregion


            double TotalPulled = 0, Diff = 0, ConsignedTaken = 0, CustownedTaken = 0, TotalCustOwned = 0, TotalConsigned = 0;
            double PullCost = 0;
            //ItemMaster objItem = context.ItemMasters.FirstOrDefault(t => t.GUID == objItemPullInfo.ItemGUID);



            double AvailQty = 0;

            {
                {
                    ToolAssetQuantityDetailDTO oLocQty = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName).GetRecordByLocationToolByToolBinId(objToolPullInfo.ToolGUID ?? Guid.Empty, objToolPullInfo.BinID, objToolPullInfo.RoomId, objToolPullInfo.CompanyId);
                    if (oLocQty != null)
                    {
                        AvailQty = (oLocQty.Quantity);
                    }
                }
            }
            ToolAssetCICOTransactionDAL objPullMasterDAL = new ToolAssetCICOTransactionDAL(SessionHelper.EnterPriseDBName);
            //double AvailQty = context.ItemLocationDetails.Where(t => t.ItemGUID == objItemPullInfo.ItemGUID && t.BinID == objItemPullInfo.BinID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Sum(t => (t.CustomerOwnedQuantity ?? 0) + (t.ConsignedQuantity ?? 0));
            List<ToolLocationLotSerialDTO> lstAvailableQty = new List<ToolLocationLotSerialDTO>();
            if (AvailQty >= objToolPullInfo.PullQuantity)
            {
                if (!objTool.LotNumberTracking && !objTool.SerialNumberTracking)
                {
                    {
                        lstAvailableQty = objPullMasterDAL.GetItemLocationsLotSerials(objToolPullInfo.ToolGUID ?? Guid.Empty, objToolPullInfo.RoomId, objToolPullInfo.CompanyId, objToolPullInfo.BinID, objToolPullInfo.PullQuantity, "1");
                        lstAvailableQty.ForEach(il =>
                        {
                            il.PullQuantity = objToolPullInfo.PullQuantity;
                            ConsignedTaken = il.Quantity ?? 0;
                            TotalPulled += (ConsignedTaken);
                            PullCost += (TotalPulled * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                            Diff = (objToolPullInfo.PullQuantity - TotalPulled);
                            if (Diff < 0)
                            {
                                TotalPulled -= ((il.Quantity ?? 0));
                                PullCost -= ((il.Quantity ?? 0)) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0));

                                ConsignedTaken = (objToolPullInfo.PullQuantity - TotalPulled);


                                TotalPulled += (CustownedTaken);
                                PullCost += (CustownedTaken) * ((_PullCost != null ? _PullCost : il.Cost).GetValueOrDefault(0));

                            }
                            TotalCustOwned += ConsignedTaken;
                            il.TobePulled = ConsignedTaken;
                            il.TotalTobePulled = ConsignedTaken;
                            il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                        });

                        objToolPullInfo.PullCost = PullCost;
                        objToolPullInfo.TotalConsignedTobePulled = TotalConsigned;
                        objToolPullInfo.TotalCustomerOwnedTobePulled = TotalCustOwned;
                        objToolPullInfo.lstToolPullDetails = lstAvailableQty;

                    }
                }
                else
                {

                    if (objTool.SerialNumberTracking)
                    {
                        lstAvailableQty = objToolPullInfo.lstToolPullDetails;

                        lstAvailableQty.ForEach(t =>
                        {

                            //else
                            {
                                List<ToolAssetQuantityDetailDTO> objItemLocationDetail = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName).GetToolsLocationsSerLotQty(objToolPullInfo.ToolGUID ?? Guid.Empty, objToolPullInfo.BinID, string.Empty, t.SerialNumber, objToolPullInfo.RoomId, objToolPullInfo.CompanyId);
                                if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                                {
                                    var lstLotQty = (from il in objItemLocationDetail
                                                     group il by new { il.SerialNumber } into grpms
                                                     select new
                                                     {
                                                         Quantity = grpms.Sum(x => x.Quantity),
                                                         SerialNumber = grpms.Key.SerialNumber,
                                                     }).FirstOrDefault();

                                    if (t.PullQuantity > (lstLotQty.Quantity))
                                    {
                                        t.ValidationMessage = ResPullMaster.msgInvalidQuantityLot;
                                    }
                                    else
                                    {
                                        if ((lstLotQty.Quantity) > 0)
                                            t.Quantity = t.PullQuantity;

                                    }
                                }
                                else
                                {
                                    t.ValidationMessage = ResPullMaster.msgInvalidLot;
                                }
                            }
                        });

                        if (lstAvailableQty.Any(t => (t.ValidationMessage ?? string.Empty) != string.Empty))
                        {
                            objToolPullInfo.ErrorList.Add(new PullToolAssetErrorInfo() { ErrorCode = "6", ErrorMessage = objTool.ToolName + ": " + ResPullMaster.msgInvalidQuantityLot });
                        }
                        else
                        {
                            lstAvailableQty.ForEach(il =>
                            {

                                CustownedTaken = il.Quantity ?? 0;

                                TotalPulled += (CustownedTaken);
                                PullCost += (TotalPulled * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                                Diff = (objToolPullInfo.PullQuantity - TotalPulled);
                                if (Diff < 0)
                                {
                                    TotalPulled -= ((il.Quantity ?? 0));
                                    PullCost -= (((il.Quantity ?? 0)) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));

                                    CustownedTaken = (objToolPullInfo.PullQuantity - TotalPulled);

                                    TotalPulled += (CustownedTaken);
                                    PullCost += ((CustownedTaken) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));

                                }
                                TotalCustOwned += CustownedTaken;
                                il.TobePulled = CustownedTaken;
                                il.TotalTobePulled = CustownedTaken;
                                il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));

                            });
                            objToolPullInfo.PullCost = PullCost;
                            objToolPullInfo.TotalCustomerOwnedTobePulled = TotalCustOwned;
                            objToolPullInfo.lstToolPullDetails = lstAvailableQty;

                        }

                    }
                }
            }
            else
            {
                objToolPullInfo.ErrorList.Add(new PullToolAssetErrorInfo() { ErrorCode = "1", ErrorMessage = objTool.ToolName + ": " + ResPullMaster.msgQuantityNotAvailable });
            }

            return objToolPullInfo;
        }
        #region "Check IN History"
        public string CheckInData(Guid CheckInCheckOutGUID, Guid ToolGuid)
        {
            ViewBag.CheckInCheckOutGUID = CheckInCheckOutGUID;
            ViewBag.ToolGuid = ToolGuid;
            ToolCheckInHistoryDAL objDAL = new ToolCheckInHistoryDAL(SessionHelper.EnterPriseDBName);
            //var objModel = objDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.CheckInCheckOutGUID == CheckInCheckOutGUID).ToList();
            var objModel = objDAL.GetToolCheckInsByTCIOHGUIDFull(CheckInCheckOutGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
            return RenderRazorViewToString("_CheckInHistory", objModel);
        }
        public ActionResult CheckInListAjax(JQueryDataTableParamModel param)
        {
            Guid CheckInCheckOutGUID = Guid.Parse(Request["ItemID"].ToString());

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
            if (sortColumnName == "0" || sortColumnName == "undefined" || sortColumnName == "ShippingMethod")
                sortColumnName = "ID";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            ///////////// requried when paging needs in this method /////////////////

            ViewBag.CheckInCheckOutGUID = CheckInCheckOutGUID;
            ToolCheckInHistoryDAL objDAL = new ToolCheckInHistoryDAL(SessionHelper.EnterPriseDBName);
            int TotalRecordCount = 0;
            var objModel = objDAL.GetPagedRecordsNew(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, CheckInCheckOutGUID);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = objModel
            },
                        JsonRequestBehavior.AllowGet);
        }
        public string DeleteCheckInRecords(string ids)
        {
            try
            {
                string MSG = "";
                ToolCheckInHistoryDAL obj = new ToolCheckInHistoryDAL(SessionHelper.EnterPriseDBName);
                obj.DeleteRecords(ids, SessionHelper.UserID, SessionHelper.CompanyID, SessionHelper.RoomID);
                if (MSG == string.Empty)
                    return "ok";
                else
                    return string.Format(ResToolMaster.MsgRecordsNotFullyCheckin, MSG.ToString());
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public ActionResult CheckInCheckOutHistoryListAjax(JQueryDataTableParamModel param)
        {
            Guid ToolGUID = Guid.Parse(Request["ItemID"].ToString());

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

            Guid reqDetailGuid = Guid.Empty;
            if (!string.IsNullOrEmpty(Request["RequisitionDetailGUID"]))
                Guid.TryParse(Request["RequisitionDetailGUID"], out reqDetailGuid);

            Guid? reqDetGUID = null;
            if (reqDetailGuid != Guid.Empty)
                reqDetGUID = reqDetailGuid;

            Guid WorkOrderGUID = Guid.Empty;
            if (!string.IsNullOrEmpty(Request["WorkOrderGUID"]))
                Guid.TryParse(Request["WorkOrderGUID"], out WorkOrderGUID);

            Guid? WOGUID = null;
            if (WOGUID != Guid.Empty)
                WOGUID = WorkOrderGUID;

            Guid ToolCheckoutGUID = Guid.Empty;
            if (!string.IsNullOrEmpty(Request["ToolCheckoutGUID"]))
                Guid.TryParse(Request["ToolCheckoutGUID"], out ToolCheckoutGUID);

            Guid? TCGUID = null;
            if (TCGUID != Guid.Empty)
                TCGUID = ToolCheckoutGUID;

            // set the default column sorting here, if first time then required to set 
            if (sortColumnName == "0" || sortColumnName == "undefined" || sortColumnName == "ShippingMethod")
                sortColumnName = "CheckOutDate";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            ///////////// requried when paging needs in this method /////////////////

            /*////////SET sSearch PARAMETER FROM REQUEST PARAMETER FOR CHILE GRID////////////////*/
            if (string.IsNullOrWhiteSpace(param.sSearch) && Request["sSearchInner"] != null)
            {
                param.sSearch = Convert.ToString(Request["sSearchInner"]);
            }
            /*////////SET sSearch PARAMETER FROM REQUEST PARAMETER FOR CHILE GRID////////////////*/

            ViewBag.ToolGUID = ToolGUID;
            ToolCheckInOutHistoryDAL objDAL = new ToolCheckInOutHistoryDAL(SessionHelper.EnterPriseDBName);
            int TotalRecordCount = 0;
            var objModel = objDAL.GetPagedRecords_HistoryNew(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, ToolGUID, reqDetGUID, WOGUID, TCGUID);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = objModel
            },
                        JsonRequestBehavior.AllowGet);
        }

        #endregion
        #endregion

        /// <summary>
        /// This method is used to update serial description in ToolAssetQuantityDetails.
        /// </summary>
        /// <param name="ToolGUID"></param>
        /// <param name="SerialNumber"></param>
        /// <param name="Description"></param>
        /// <returns></returns>
        public JsonResult UpdateSerialDescription(Guid ToolGUID, string SerialNumber, string Description)
        {
            try
            {
                ToolAssetQuantityDetailDAL toolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
                bool result = toolAssetQuantityDetailDAL.UpdateSerialDescription(ToolGUID, SerialNumber, Description);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public string ToolAssetQuantityDetailsDelete(string ids)
        {
            try
            {
                ToolAssetQuantityDetailDAL obj = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
                if (obj.DeleteToolAssetQuantityDetailRecords(ids, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, "TA Quantity Detail Delete") == true)
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


        public JsonResult GetLatestQTYfromTool(Guid ToolGUID)
        {
            ToolMasterDAL objToolAPI = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            ToolMasterDTO ObjTool = objToolAPI.GetToolByGUIDPlain(ToolGUID);

            if (ObjTool != null)
            {
                ObjTool.AvailableToolQty = ObjTool.Quantity - (ObjTool.CheckedOutQTY.GetValueOrDefault(0) + ObjTool.CheckedOutMQTY.GetValueOrDefault(0));
                return Json(ObjTool, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(new ItemMasterDTO(), JsonRequestBehavior.AllowGet);
        }


        #region Tool Certification Image
        public ActionResult GetToolCertificationImages(Guid toolGuid)
        {
            ToolImageDetailDAL toolImageDetailDAL = new ToolImageDetailDAL(SessionHelper.EnterPriseDBName);
            List<ToolImageDetailDTO> toolCertificationImages = toolImageDetailDAL.GetToolImages(SessionHelper.RoomID, SessionHelper.CompanyID, toolGuid).ToList();
            Dictionary<string, string> retData = new Dictionary<string, string>();

            foreach (ToolImageDetailDTO img in toolCertificationImages)
            {
                retData.Add(Convert.ToString(img.ID), img.ImagePath);
            }
            return Json(new { DDData = retData }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ToolImagesListAjax(JQueryDataTableParamModel param)
        {
            ToolImageDetailDAL obj = new ToolImageDetailDAL(SessionHelper.EnterPriseDBName);

            //int PageIndex = int.Parse(param.sEcho);
            //int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            var tabname = Convert.ToString(Request["tabname"]);
            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"].ToString();

            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());

            Guid ToolGUID;
            //Guid.TryParse(Convert.ToString(Request["AssetGUID"]), out AssetGUID);
            Guid.TryParse(Convert.ToString(Request["ToolGUID"]), out ToolGUID);

            // set the default column sorting here, if first time then required to set 
            //if (sortColumnName == "0" || sortColumnName == "undefined" || string.IsNullOrEmpty(sortColumnName))
            //    sortColumnName = "ID Desc";
            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "ID desc";
            }
            else
                sortColumnName = "ID desc";

            string searchQuery = string.Empty;

            long TotalRecordCount = 0;
            param.iDisplayLength = param.iDisplayLength == -1 ? int.MaxValue : param.iDisplayLength;
            //var DataFromDB = obj.GetSerialToolImages(param.iDisplayStart, param.iDisplayLength, SessionHelper.RoomID, SessionHelper.CompanyID , ToolGUID,out TotalRecordCount);            
            var DataFromDB = obj.GetSerialToolImages(0, int.MaxValue, SessionHelper.RoomID, SessionHelper.CompanyID, ToolGUID, out TotalRecordCount);
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
            JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is used to delete tool images
        /// </summary>
        /// <param name="FileId"></param>
        /// <param name="ToolGuid"></param>
        /// <returns></returns>
        public JsonResult DeleteExistingFiles(string FileId, Guid ToolGuid)
        {
            try
            {
                ToolImageDetailDAL toolImageDetailDAL = new ToolImageDetailDAL(SessionHelper.EnterPriseDBName);
                var result = toolImageDetailDAL.DeleteRecords(FileId, SessionHelper.UserID, SessionHelper.CompanyID, SessionHelper.RoomID, ToolGuid);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult WrittenOffTool(List<ToolWrittenOffDTO> WrittenOffTools)
        {
            List<ItemPullInfo> oReturn = new List<ItemPullInfo>();
            ToolMoveInOutDetailDAL ToolMoveInOutDtlDAL = new ToolMoveInOutDetailDAL(SessionHelper.EnterPriseDBName);
            bool AllowToolOrdering = SessionHelper.AllowToolOrdering;
            string msg = string.Empty;

            if (WrittenOffTools != null && WrittenOffTools.Any())
            {
                foreach (var ToolWrittenOff in WrittenOffTools)
                {
                    if (ToolWrittenOff != null && ToolWrittenOff.ToolGUID != Guid.Empty && ToolWrittenOff.Quantity > 0)
                    {
                        if ((ToolWrittenOff.ToolKitDetailGUID == Guid.Empty || ToolWrittenOff.ToolKitDetailGUID == null) &&
                             (ToolWrittenOff.ToolKitGUID == Guid.Empty || ToolWrittenOff.ToolKitGUID == null))
                        {
                            #region Written Off Without KitDetail
                            ToolMasterDAL toolMasterDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                            bool isToolQtyUpdatedSuccessfully;
                            List<string> serialsUpdatedSuccessfully = new List<string>();
                            List<string> serialsFailToUpdate = new List<string>();
                            string reasonToFail = string.Empty;
                            string reasonToFailErrorCode = string.Empty;

                            if (string.IsNullOrEmpty(ToolWrittenOff.Serial))
                            {
                                isToolQtyUpdatedSuccessfully = toolMasterDAL.UpdateToolQuantityOnToolWrittenOff(ToolWrittenOff.ToolGUID.GetValueOrDefault(Guid.Empty), ToolWrittenOff.Quantity.GetValueOrDefault(0), SessionHelper.UserID, SessionHelper.CompanyID, SessionHelper.RoomID, out reasonToFailErrorCode, false);
                            }
                            else
                            {
                                serialsFailToUpdate = toolMasterDAL.UpdateToolQuantityOnToolWrittenOffForNewTool(ToolWrittenOff, SessionHelper.UserID, SessionHelper.CompanyID, SessionHelper.RoomID, out serialsUpdatedSuccessfully, out reasonToFailErrorCode, false);
                                isToolQtyUpdatedSuccessfully = serialsUpdatedSuccessfully.Any();
                            }

                            if (!string.IsNullOrEmpty(reasonToFailErrorCode))
                            {
                                reasonToFail = SetErrorMessage(reasonToFailErrorCode);
                            }

                            ToolWrittenOffDAL toolWrittenOffDAL = new ToolWrittenOffDAL(SessionHelper.EnterPriseDBName);
                            ToolWrittenOff.CompanyID = SessionHelper.CompanyID;
                            ToolWrittenOff.Room = SessionHelper.RoomID;
                            ToolWrittenOff.CreatedBy = SessionHelper.UserID;
                            ToolWrittenOff.LastUpdatedBy = SessionHelper.UserID;

                            long id = string.IsNullOrEmpty(ToolWrittenOff.Serial)
                                ? (isToolQtyUpdatedSuccessfully ? toolWrittenOffDAL.Insert(ToolWrittenOff) : 0)
                                : (isToolQtyUpdatedSuccessfully ? toolWrittenOffDAL.InsertForNewTool(ToolWrittenOff, serialsUpdatedSuccessfully) : 0);

                            if (id > 0)
                            {
                                var errorMessage = string.Empty;

                                if (!string.IsNullOrEmpty(ToolWrittenOff.Serial) && (serialsFailToUpdate.Any() && serialsUpdatedSuccessfully.Any()))
                                {
                                    errorMessage = ResToolMaster.SerialsNotAbleToWrittenOff + string.Join(",", serialsFailToUpdate);
                                }

                                var responseObject = new ItemPullInfo
                                {
                                    ItemGUID = ToolWrittenOff.ToolGUID.GetValueOrDefault(Guid.Empty),
                                    ErrorMessage = errorMessage
                                };
                                oReturn.Add(responseObject);
                            }
                            else
                            {
                                var responseObject = new ItemPullInfo
                                {
                                    ItemGUID = ToolWrittenOff.ToolGUID.GetValueOrDefault(Guid.Empty),
                                    ErrorMessage = reasonToFail == string.Empty ? ResToolMaster.FailToWrittenOffTool : reasonToFail
                                };
                                oReturn.Add(responseObject);
                            }
                            #endregion
                        }
                        else if (ToolWrittenOff.ToolKitDetailGUID != Guid.Empty && ToolWrittenOff.ToolKitGUID != Guid.Empty)
                        {
                            ToolMasterDAL toolMasterDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                            ToolWrittenOffDAL toolWrittenOffDAL = new ToolWrittenOffDAL(SessionHelper.EnterPriseDBName);
                            bool isToolQtyUpdatedSuccessfully;
                            List<string> serialsUpdatedSuccessfully = new List<string>();
                            List<string> serialsFailToUpdate = new List<string>();
                            string reasonToFail = string.Empty;
                            string reasonToFailErrorCode = string.Empty;

                            ToolMoveInOutDetailDTO MoveInDTO = new ToolMoveInOutDetailDTO();
                            MoveInDTO.CreatedBy = UserID;
                            MoveInDTO.LastUpdatedBy = UserID;
                            MoveInDTO.Room = RoomID;
                            MoveInDTO.CompanyID = CompanyID;
                            MoveInDTO.MoveInOut = "OUT";
                            MoveInDTO.ID = 0;
                            MoveInDTO.Quantity = ToolWrittenOff.Quantity.GetValueOrDefault(0);
                            MoveInDTO.GUID = ToolWrittenOff.ToolKitDetailGUID.GetValueOrDefault(Guid.Empty);
                            MoveInDTO.ToolDetailGUID = ToolWrittenOff.ToolKitGUID.GetValueOrDefault(Guid.Empty);
                            MoveInDTO.ToolItemGUID = ToolWrittenOff.ToolGUID.GetValueOrDefault(Guid.Empty);
                            MoveInDTO.SerialNumber = ToolWrittenOff.Serial;

                            msg += ToolMoveInOutDtlDAL.QtyToMoveOut(MoveInDTO, RoomID, CompanyID, UserID, SessionHelper.EnterPriceID, ResourceHelper.CurrentCult.Name, AllowToolOrdering);
                            if (string.IsNullOrEmpty(msg))
                            {

                                if (string.IsNullOrEmpty(ToolWrittenOff.Serial))
                                {
                                    isToolQtyUpdatedSuccessfully = toolMasterDAL.UpdateToolQuantityOnToolWrittenOff(ToolWrittenOff.ToolGUID.GetValueOrDefault(Guid.Empty), ToolWrittenOff.Quantity.GetValueOrDefault(0), SessionHelper.UserID, SessionHelper.CompanyID, SessionHelper.RoomID, out reasonToFailErrorCode, true);
                                }
                                else
                                {
                                    serialsFailToUpdate = toolMasterDAL.UpdateToolQuantityOnToolWrittenOffForNewTool(ToolWrittenOff, SessionHelper.UserID, SessionHelper.CompanyID, SessionHelper.RoomID, out serialsUpdatedSuccessfully, out reasonToFailErrorCode, true);
                                    isToolQtyUpdatedSuccessfully = serialsUpdatedSuccessfully.Any();
                                }

                                if (!string.IsNullOrEmpty(reasonToFailErrorCode))
                                {
                                    reasonToFail = SetErrorMessage(reasonToFailErrorCode);
                                }

                                ToolWrittenOff.CompanyID = SessionHelper.CompanyID;
                                ToolWrittenOff.Room = SessionHelper.RoomID;
                                ToolWrittenOff.CreatedBy = SessionHelper.UserID;
                                ToolWrittenOff.LastUpdatedBy = SessionHelper.UserID;

                                long id = string.IsNullOrEmpty(ToolWrittenOff.Serial)
                                        ? (isToolQtyUpdatedSuccessfully ? toolWrittenOffDAL.Insert(ToolWrittenOff) : 0)
                                        : (isToolQtyUpdatedSuccessfully ? toolWrittenOffDAL.InsertForNewTool(ToolWrittenOff, serialsUpdatedSuccessfully) : 0);

                                if (id > 0)
                                {
                                    var errorMessage = string.Empty;

                                    if (!string.IsNullOrEmpty(ToolWrittenOff.Serial) && (serialsFailToUpdate.Any() && serialsUpdatedSuccessfully.Any()))
                                    {
                                        errorMessage = ResToolMaster.SerialsNotAbleToWrittenOff + string.Join(",", serialsFailToUpdate);
                                    }

                                    var responseObject = new ItemPullInfo
                                    {
                                        ItemGUID = ToolWrittenOff.ToolGUID.GetValueOrDefault(Guid.Empty),
                                        ErrorMessage = errorMessage
                                    };
                                    oReturn.Add(responseObject);
                                }
                                else
                                {
                                    var responseObject = new ItemPullInfo
                                    {
                                        ItemGUID = ToolWrittenOff.ToolGUID.GetValueOrDefault(Guid.Empty),
                                        ErrorMessage = reasonToFail == string.Empty ? ResToolMaster.FailToWrittenOffTool : reasonToFail
                                    };
                                    oReturn.Add(responseObject);
                                }

                            }
                            else
                            {
                                var responseObject = new ItemPullInfo
                                {
                                    ItemGUID = ToolWrittenOff.ToolGUID.GetValueOrDefault(Guid.Empty),
                                    ErrorMessage = msg
                                };
                                oReturn.Add(responseObject);
                            }
                        }
                    } // if end
                }
            }

            Session["RoomAllWrittenOffTool"] = null; //Used in narrow search
            return Json(oReturn, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// This method is used to set error message based on error code.
        /// </summary>
        /// <param name="ErrorCode"></param>
        /// <returns></returns>
        private string SetErrorMessage(string ErrorCode)
        {
            string reasonToFail = string.Empty;

            if (!string.IsNullOrEmpty(ErrorCode))
            {
                switch (ErrorCode)
                {
                    case "1":
                        reasonToFail += ResToolMaster.NotEnoughQtyToWrittenOff;
                        break;
                    case "2":
                        reasonToFail += ResToolMaster.AvailableQtyCantBeLessThanOne;
                        break;
                }
            }
            return reasonToFail;
        }
        #endregion

        [HttpPost]
        public ActionResult WrittenOffToolQuantity(List<ToolMasterDTO> lstPullRequestInfo)
        {
            List<ToolMasterDTO> lstWrittenOffRequest = new List<ToolMasterDTO>();
            foreach (ToolMasterDTO objPullMasterDTO in lstPullRequestInfo)
            {
                if (!lstWrittenOffRequest.Select(x => x.GUID).Contains(objPullMasterDTO.GUID))
                    lstWrittenOffRequest.Add(objPullMasterDTO);
            }

            ToolMasterDAL obj = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            ViewBag.ToolWrittenOffCategories = obj.GetAllToolWrittenOffCategories(SessionHelper.CompanyID, SessionHelper.RoomID);

            return PartialView("ToolWrittenOffSrSelection", lstPullRequestInfo);
        }

        public ActionResult WrittenOffLotSrSelection(JQueryDataTableParamModel param)
        {
            Guid ItemGUID = Guid.Empty;
            Guid ToolGUID = Guid.Empty;
            Guid ToolKitDetailGUID = Guid.Empty;
            Guid ToolKitGUID = Guid.Empty;
            Guid MaterialStagingGUID = Guid.Empty;
            long BinID = 0;
            double PullQuantity = 0;
            Guid.TryParse(Convert.ToString(Request["ToolGUID"]), out ToolGUID);
            Guid.TryParse(Convert.ToString(Request["ToolKitDetailGUID"]), out ToolKitDetailGUID);
            Guid.TryParse(Convert.ToString(Request["ToolKitGUID"]), out ToolKitGUID);
            //long.TryParse(Convert.ToString(Request["BinID"]), out BinID);
            double.TryParse(Convert.ToString(Request["PullQuantity"]), out PullQuantity);
            string CurrentLoaded = Convert.ToString(Request["CurrentLoaded"]);
            string ViewRight = Convert.ToString(Request["ViewRight"]);
            bool IsDeleteRowMode = Convert.ToBoolean(Request["IsDeleteRowMode"]);
            bool IsSerialNumberTracking = Convert.ToBoolean(Request["SerialNumberTracking"]);
            string[] arrIds = new string[] { };

            if (!string.IsNullOrWhiteSpace(CurrentLoaded))
            {
                arrIds = CurrentLoaded.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }

            int TotalRecordCount = 0;
            ToolMasterDAL toolMasterDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            List<ItemLocationLotSerialDTO> lstLotSrs = new List<ItemLocationLotSerialDTO>();
            List<ItemLocationLotSerialDTO> retlstLotSrs = new List<ItemLocationLotSerialDTO>();
            Dictionary<string, double> dicSerialLots = new Dictionary<string, double>();
            string[] arrItem;

            if (arrIds.Count() > 0)
            {
                string[] arrSerialLots = new string[arrIds.Count()];
                for (int i = 0; i < arrIds.Count(); i++)
                {
                    if (IsSerialNumberTracking)
                    {
                        arrItem = new string[2];
                        arrItem[0] = arrIds[i].Substring(0, arrIds[i].LastIndexOf("_"));
                        arrItem[1] = arrIds[i].Replace(arrItem[0] + "_", "");
                        if (arrItem.Length > 1)
                        {
                            arrSerialLots[i] = arrItem[0];
                            dicSerialLots.Add(arrItem[0], Convert.ToDouble(arrItem[1]));
                        }
                    }
                    else
                    {
                        arrItem = arrIds[i].Split('_');
                        if (arrItem.Length > 1)
                        {
                            arrSerialLots[i] = arrItem[0];
                            dicSerialLots.Add(arrItem[0], Convert.ToDouble(arrItem[1]));
                        }
                    }
                }

                lstLotSrs = toolMasterDAL.GetItemLocationsWithLotSerialsForWrittenOff(ToolGUID, SessionHelper.RoomID, SessionHelper.CompanyID, string.Empty, PullQuantity);

                retlstLotSrs = lstLotSrs.Where(t =>
                    (
                        (
                            arrSerialLots.Contains(t.LotOrSerailNumber, StringComparer.OrdinalIgnoreCase)
                            && (IsSerialNumberTracking))
                    || (!IsSerialNumberTracking))).ToList();

                if (!IsDeleteRowMode)
                {
                    if (ViewRight == "NoRight" && (IsSerialNumberTracking))
                    {
                        ItemLocationLotSerialDTO oLotSr = new ItemLocationLotSerialDTO();
                        //oLotSr.BinID = BinID;
                        oLotSr.ID = BinID;
                        oLotSr.ItemGUID = ItemGUID;
                        oLotSr.LotOrSerailNumber = string.Empty;
                        //oLotSr.BinNumber = string.Empty;

                        //if (objLocDTO != null && objLocDTO.ID > 0)
                        //{
                        //    oLotSr.BinNumber = objLocDTO.BinNumber;
                        //}
                        if (IsSerialNumberTracking)
                        {
                            oLotSr.PullQuantity = 1;
                        }
                        oLotSr.SerialNumberTracking = IsSerialNumberTracking;
                        retlstLotSrs.Add(oLotSr);
                    }
                    else
                    {
                        retlstLotSrs =
                            retlstLotSrs.Union
                            (
                                lstLotSrs.Where(t =>
                                (
                                    (
                                        !arrSerialLots.Contains(t.LotOrSerailNumber, StringComparer.OrdinalIgnoreCase)
                                        && (IsSerialNumberTracking)

                                    )
                                ||
                                    (
                                        !IsSerialNumberTracking
                                        )
                                )).Take(1)
                            ).ToList();

                    }
                }
            }
            else
            {
                if (ViewRight == "NoRight" && (IsSerialNumberTracking))
                {
                    ItemLocationLotSerialDTO oLotSr = new ItemLocationLotSerialDTO();
                    //oLotSr.BinID = BinID;
                    oLotSr.ID = BinID;
                    oLotSr.ItemGUID = ItemGUID;
                    oLotSr.LotOrSerailNumber = string.Empty;
                    //oLotSr.BinNumber = string.Empty;

                    //if (objLocDTO != null && objLocDTO.ID > 0)
                    //{
                    //    oLotSr.BinNumber = objLocDTO.BinNumber;
                    //}
                    oLotSr.PullQuantity = 1;
                    oLotSr.SerialNumberTracking = IsSerialNumberTracking;
                    retlstLotSrs.Add(oLotSr);
                }
                else
                {
                    if ((ToolKitDetailGUID == Guid.Empty || ToolKitDetailGUID == null) && (ToolKitGUID == Guid.Empty || ToolKitGUID == null))
                    {
                        retlstLotSrs = toolMasterDAL.GetItemLocationsWithLotSerialsForWrittenOff(ToolGUID, SessionHelper.RoomID, SessionHelper.CompanyID, string.Empty, PullQuantity);
                    }
                    else
                    {
                        List<ToolQuantityLotSerialDTO> lstLotSrsWritten = new List<ToolQuantityLotSerialDTO>();
                        ToolAssetCICOTransactionDAL objPullDetails = new ToolAssetCICOTransactionDAL(SessionHelper.EnterPriseDBName);
                        lstLotSrsWritten = objPullDetails.GetToolLocationsWithLotSerialsForOut(ToolKitDetailGUID, SessionHelper.RoomID, SessionHelper.CompanyID, BinID, PullQuantity, "1", string.Empty);
                        foreach (var item in lstLotSrsWritten)
                        {
                            ToolMasterDTO ToolDto = toolMasterDAL.GetToolByGUIDPlain(ToolGUID);
                            var toolSerialDetails = new ItemLocationLotSerialDTO
                            {
                                ItemGUID = ToolDto.GUID,
                                ID = ToolDto.ID,
                                SerialNumberTracking = ToolDto.SerialNumberTracking,
                                Received = FnCommon.ConvertDateByTimeZone(ToolDto.ReceivedOn, true, true),
                                ReceivedDate = ToolDto.ReceivedOn,
                                SerialNumber = item.SerialNumber,
                                LotSerialQuantity = ToolDto.Quantity,
                                LotOrSerailNumber = item.LotOrSerailNumber,
                                PullQuantity = item.LotSerialQuantity
                            };
                            retlstLotSrs.Add(toolSerialDetails);
                        }
                    }

                }
            }

            foreach (ItemLocationLotSerialDTO item in retlstLotSrs)
            {
                if (dicSerialLots.ContainsKey(item.LotOrSerailNumber) && (IsSerialNumberTracking))
                {
                    double value = dicSerialLots[item.LotOrSerailNumber];
                    item.PullQuantity = value;
                    PullQuantity -= item.PullQuantity;
                }
                else if (item.PullQuantity <= PullQuantity)
                {
                    PullQuantity -= item.PullQuantity;
                }
                else if (PullQuantity >= 0)
                {
                    item.PullQuantity = PullQuantity;
                    PullQuantity = 0;
                }
                else
                {
                    item.PullQuantity = 0;
                }

                if (item.ReceivedDate != null && item.ReceivedDate.HasValue && item.ReceivedDate.Value != DateTime.MinValue)
                {
                    item.Received = FnCommon.ConvertDateByTimeZone(item.ReceivedDate.Value, true, true);
                }
                if (item.PullQuantity > 0)
                    item.IsSelected = true;
            }

            if (!(ViewRight == "NoRight" && (IsSerialNumberTracking)))
                retlstLotSrs = retlstLotSrs.Where(x => x.PullQuantity > 0).ToList();

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = retlstLotSrs
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ValidateSerialNumber(Guid? ToolGuid, string SerialOrLotNumber, Guid? ToolKitDetailGuid)
        {
            if (!string.IsNullOrWhiteSpace(SerialOrLotNumber))
            {
                SerialOrLotNumber = SerialOrLotNumber.Trim();
            }

            ItemLocationLotSerialDTO objItemLocationLotSerialDTO;

            if (ToolKitDetailGuid == null || ToolKitDetailGuid == Guid.Empty)
            {
                ToolMasterDAL toolMasterDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                objItemLocationLotSerialDTO = toolMasterDAL.GetItemLocationsWithLotSerialsForWrittenOff(ToolGuid ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, SerialOrLotNumber, 1).FirstOrDefault();

                if (objItemLocationLotSerialDTO == null)
                {
                    objItemLocationLotSerialDTO = new ItemLocationLotSerialDTO();
                    //objItemLocationLotSerialDTO.BinID = BinID;
                    objItemLocationLotSerialDTO.ID = 1;
                    objItemLocationLotSerialDTO.ItemGUID = ToolGuid;
                    objItemLocationLotSerialDTO.LotOrSerailNumber = string.Empty;
                    //objItemLocationLotSerialDTO.BinNumber = string.Empty;
                }
            }
            else
            {
                ToolQuantityLotSerialDTO moveOutSr = new ToolQuantityLotSerialDTO();
                ToolAssetCICOTransactionDAL toolAssetCICOTransactionDAL = new ToolAssetCICOTransactionDAL(SessionHelper.EnterPriseDBName);
                moveOutSr = toolAssetCICOTransactionDAL.GetToolLocationsWithLotSerialsForOut(ToolKitDetailGuid ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, 0, 1, "1", SerialOrLotNumber).FirstOrDefault();

                if (moveOutSr == null)
                {
                    objItemLocationLotSerialDTO = new ItemLocationLotSerialDTO();
                    //objItemLocationLotSerialDTO.BinID = BinID;
                    objItemLocationLotSerialDTO.ID = 1;
                    objItemLocationLotSerialDTO.ItemGUID = ToolGuid;
                    objItemLocationLotSerialDTO.LotOrSerailNumber = string.Empty;
                    //objItemLocationLotSerialDTO.BinNumber = string.Empty;
                }
                else
                {
                    ToolMasterDAL toolMasterDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                    ToolMasterDTO ToolDto = toolMasterDAL.GetToolByGUIDPlain(ToolGuid.GetValueOrDefault(Guid.Empty));

                    objItemLocationLotSerialDTO = new ItemLocationLotSerialDTO();
                    objItemLocationLotSerialDTO.ItemGUID = moveOutSr.ToolGUID;
                    objItemLocationLotSerialDTO.ID = ToolDto.ID;
                    objItemLocationLotSerialDTO.SerialNumberTracking = moveOutSr.SerialNumberTracking;
                    objItemLocationLotSerialDTO.Received = moveOutSr.Received;
                    objItemLocationLotSerialDTO.ReceivedDate = moveOutSr.ReceivedDate;
                    objItemLocationLotSerialDTO.SerialNumber = moveOutSr.SerialNumber;
                    objItemLocationLotSerialDTO.LotSerialQuantity = ToolDto.Quantity;
                    objItemLocationLotSerialDTO.LotOrSerailNumber = moveOutSr.SerialNumber;
                    objItemLocationLotSerialDTO.PullQuantity = 1;
                }
            }
            return Json(objItemLocationLotSerialDTO);
        }

        public JsonResult GetSerailNumberList(int maxRows, string name_startsWith, Guid? ToolGuid, string prmSerialNos = null, Guid? ToolKitDetailGuid = null)
        {
            ToolMasterDAL toolMasterDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            List<ItemLocationLotSerialDTO> objItemLocationLotSerialDTO = new List<ItemLocationLotSerialDTO>();
            List<ToolQuantityLotSerialDTO> moveOutSr = new List<ToolQuantityLotSerialDTO>();

            if (ToolKitDetailGuid == null || ToolKitDetailGuid == Guid.Empty)
            {
                objItemLocationLotSerialDTO = toolMasterDAL.GetItemLocationsWithLotSerialsForWrittenOff(ToolGuid ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, String.Empty, 1);
            }
            else
            {
                ToolAssetCICOTransactionDAL toolAssetCICOTransactionDAL = new ToolAssetCICOTransactionDAL(SessionHelper.EnterPriseDBName);
                moveOutSr = toolAssetCICOTransactionDAL.GetToolLocationsWithLotSerialsForOut(ToolKitDetailGuid ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, 0, 1, "1", string.Empty);
            }

            string[] arrSerialNos = prmSerialNos.Split(new string[] { "|#|" }, StringSplitOptions.RemoveEmptyEntries);

            if (!string.IsNullOrWhiteSpace(name_startsWith))
            {
                name_startsWith = name_startsWith.Trim();
            }

            var lstSr = (ToolKitDetailGuid == null || ToolKitDetailGuid == Guid.Empty)
                        ? objItemLocationLotSerialDTO.Where(x => x.LotOrSerailNumber.Contains(name_startsWith) && !arrSerialNos.Contains(x.LotOrSerailNumber)).Select(x => new { x.LotOrSerailNumber }).Distinct()
                        : moveOutSr.Where(x => x.LotOrSerailNumber.Contains(name_startsWith) && !arrSerialNos.Contains(x.LotOrSerailNumber)).Select(x => new { x.LotOrSerailNumber }).Distinct();

            if (lstSr.Count() == 0)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

            return Json(lstSr, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteToolImage(string ToolGUID)
        {
            ToolMasterDAL objDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            bool IsUpdate = objDAL.RemoveToolImage(Guid.Parse(ToolGUID), "web", SessionHelper.UserID);
            if (IsUpdate)
                return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult ReassignTechnician(List<ReassignCheckOutTechnicianDTO> ReassignCheckOutTechnician, string TechnicianName)
        public JsonResult ReassignTechnician(string ToolCheckInCheckOutGuids, string TechnicianName, string UDF1, string UDF2, string UDF3, string UDF4, string UDF5)        
        {
            try
            {
                Guid TechnicianGuid = Guid.Empty;
                ToolMasterDAL toolMasterDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);

                //-----------------------INSERT NEW TECHNICIAN IF REQUIRED-----------------------
                //
                string TechnicianCode = "";
                TechnicianName = TechnicianName.Trim();

                if (TechnicianName.Contains(" --- "))
                {
                    TechnicianCode = TechnicianName.Split(new string[1] { " --- " }, StringSplitOptions.RemoveEmptyEntries)[0];
                    TechnicianName = TechnicianName.Split(new string[1] { " --- " }, StringSplitOptions.RemoveEmptyEntries)[1];
                }
                else
                {
                    TechnicianCode = TechnicianName;
                    TechnicianName = "";
                }

                if (TechnicianCode.Trim() == "")
                {
                    return Json(new { Message = ResMessage.InvalidTechnician, Status = false, NotificationClass = "WarningIcon" }, JsonRequestBehavior.AllowGet);
                }

                if ((!string.IsNullOrEmpty(TechnicianCode) && TechnicianCode.IndexOf("---") >= 0)
                    || (!string.IsNullOrEmpty(TechnicianName) && TechnicianName.IndexOf("---") >= 0))
                {
                    return Json(new { Message = ResToolCheckInOutHistory.MsgRemoveInvalidValueFromTechnician, Status = false, NotificationClass = "WarningIcon" }, JsonRequestBehavior.AllowGet);
                }

                TechnicialMasterDAL objTechnicialMasterDAL = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
                TechnicianMasterDTO objTechnicianMasterDTO = objTechnicialMasterDAL.GetTechnicianByCodePlain(TechnicianCode, SessionHelper.RoomID, SessionHelper.CompanyID);

                if (objTechnicianMasterDTO == null)
                {
                    objTechnicianMasterDTO = new TechnicianMasterDTO();
                    objTechnicianMasterDTO.TechnicianCode = TechnicianCode;
                    objTechnicianMasterDTO.Technician = TechnicianName;
                    objTechnicianMasterDTO.Room = SessionHelper.RoomID;
                    objTechnicianMasterDTO.CompanyID = SessionHelper.CompanyID;
                    objTechnicianMasterDTO.CreatedBy = SessionHelper.UserID;
                    objTechnicianMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                    objTechnicianMasterDTO.Created = DateTimeUtility.DateTimeNow;
                    objTechnicianMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                    objTechnicianMasterDTO.GUID = Guid.NewGuid();
                    objTechnicianMasterDTO.IsArchived = false;
                    objTechnicianMasterDTO.IsDeleted = false;
                    Int64 TechnicanID = objTechnicialMasterDAL.Insert(objTechnicianMasterDTO);

                    objTechnicianMasterDTO = objTechnicialMasterDAL.GetTechnicianByIDPlain(TechnicanID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    TechnicianGuid = objTechnicianMasterDTO.GUID;
                }
                else
                {
                    TechnicianGuid = objTechnicianMasterDTO.GUID;
                }

                UDF1 = HttpUtility.UrlDecode(UDF1);
                UDF2 = HttpUtility.UrlDecode(UDF2);
                UDF3 = HttpUtility.UrlDecode(UDF3);
                UDF4 = HttpUtility.UrlDecode(UDF4);
                UDF5 = HttpUtility.UrlDecode(UDF5);

                UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
                List<UDFDTO> DataFromDB = objUDFApiController.GetRequiredUDFsByUDFTableNamePlain("ToolCheckInOutHistory", SessionHelper.RoomID, SessionHelper.CompanyID);
                string udfRequier = string.Empty;

                foreach (var i in DataFromDB)
                {
                        if (i.UDFColumnName == "UDF1"  && string.IsNullOrEmpty(UDF1))
                        {
                            string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                            string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                            if (!string.IsNullOrEmpty(val))
                                i.UDFDisplayColumnName = val;
                            else
                                i.UDFDisplayColumnName = i.UDFColumnName;
                            udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                        }
                        else if (i.UDFColumnName == "UDF2"  && string.IsNullOrEmpty(UDF2))
                        {
                            string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                            string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                            if (!string.IsNullOrEmpty(val))
                                i.UDFDisplayColumnName = val;
                            else
                                i.UDFDisplayColumnName = i.UDFColumnName;
                            udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                        }
                        else if (i.UDFColumnName == "UDF3"  && string.IsNullOrEmpty(UDF3))
                        {
                            string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                            string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                            if (!string.IsNullOrEmpty(val))
                                i.UDFDisplayColumnName = val;
                            else
                                i.UDFDisplayColumnName = i.UDFColumnName;
                            udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                        }
                        else if (i.UDFColumnName == "UDF4"  && string.IsNullOrEmpty(UDF4))
                        {
                            string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                            string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                            if (!string.IsNullOrEmpty(val))
                                i.UDFDisplayColumnName = val;
                            else
                                i.UDFDisplayColumnName = i.UDFColumnName;
                            udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                        }
                        else if (i.UDFColumnName == "UDF5"  && string.IsNullOrEmpty(UDF5))
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
                {
                    return Json(new { Message = udfRequier, Status = false, NotificationClass = "WarningIcon" }, JsonRequestBehavior.AllowGet);
                }

                if (UDF1 != null && UDF1 != string.Empty)
                    InsertCheckOutUDf(UDF1, CommonUtility.ImportToolMasterColumn.UDF1.ToString());
                if (UDF2 != null && UDF2 != string.Empty)
                    InsertCheckOutUDf(UDF2, CommonUtility.ImportToolMasterColumn.UDF2.ToString());
                if (UDF3 != null && UDF3 != string.Empty)
                    InsertCheckOutUDf(UDF3, CommonUtility.ImportToolMasterColumn.UDF3.ToString());
                if (UDF4 != null && UDF4 != string.Empty)
                    InsertCheckOutUDf(UDF4, CommonUtility.ImportToolMasterColumn.UDF4.ToString());
                if (UDF5 != null && UDF5 != string.Empty)
                    InsertCheckOutUDf(UDF5, CommonUtility.ImportToolMasterColumn.UDF5.ToString());

                //if (ReassignCheckOutTechnician != null && ReassignCheckOutTechnician.Any())
                //{
                //    ReassignCheckOutTechnician.Select(c => c.UDF1 = HttpUtility.UrlDecode(c.UDF1));
                //    ReassignCheckOutTechnician.Select(c => c.UDF2 = HttpUtility.UrlDecode(c.UDF2));
                //    ReassignCheckOutTechnician.Select(c => c.UDF3 = HttpUtility.UrlDecode(c.UDF3));
                //    ReassignCheckOutTechnician.Select(c => c.UDF4 = HttpUtility.UrlDecode(c.UDF4));
                //    ReassignCheckOutTechnician.Select(c => c.UDF5 = HttpUtility.UrlDecode(c.UDF5));
                //}

                //DataTable ReassignCheckOutTechnicianTable = new DataTable();
                //ReassignCheckOutTechnicianTable = CommonUtilityHelper.ToDataTable(ReassignCheckOutTechnician);
                toolMasterDAL.ReassignToolCheckOutTechnician(ToolCheckInCheckOutGuids, TechnicianGuid, SessionHelper.UserID,UDF1,UDF2,UDF3,UDF4,UDF5);

                return Json(new { Message = ResMessage.SaveMessage, Status = true , NotificationClass = "succesIcon" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ResMessage.SaveErrorMsg + (Convert.ToString(ex.InnerException) ?? string.Empty), Status = false, NotificationClass = "errorIcon" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}