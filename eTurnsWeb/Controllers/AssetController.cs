using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsWeb.Helper;
using eTurnsWeb.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace eTurnsWeb.Controllers
{
    [AuthorizeHelper]
    public class AssetsController : eTurnsControllerBase
    {
        bool IsInsert = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Requisitions, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        bool IsUpdate = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Requisitions, eTurnsWeb.Helper.SessionHelper.PermissionType.Update);
        bool IsDelete = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Requisitions, eTurnsWeb.Helper.SessionHelper.PermissionType.Delete);
        bool IsApprove = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.RequisitionApproval, eTurnsWeb.Helper.SessionHelper.PermissionType.Approval);
        Int64 RoomID = SessionHelper.RoomID;
        Int64 CompanyID = SessionHelper.CompanyID;
        UDFController objUDFDAL = new UDFController();
        string ResourceBaseFilePath = CommonUtility.ResourceBaseFilePath;

        string enterPriseDBName;
        ToolMasterDAL objToolMasterDAL = null;

        #region Constructor
        public AssetsController()
        {
            enterPriseDBName = SessionHelper.EnterPriseDBName;
            objToolMasterDAL = new eTurns.DAL.ToolMasterDAL(enterPriseDBName);
        }

        #endregion

        #region Destuctor

        bool disposed = false;

        // Protected implementation of Dispose pattern.
        protected override void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                objToolMasterDAL.Dispose();
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //

            disposed = true;
            // Call base class implementation.
            base.Dispose(disposing);
        }

        #endregion

        #region "Tool"

        public ActionResult ToolList()
        {
            return View();
        }



        public PartialViewResult _CreateTool()
        {
            return PartialView();
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult ToolCreate()
        {
            //AutoSequenceNumbersDTO objAutoSequenceDTO = new AutoSequenceDAL(enterPriseDBName).GetLastGeneratedID(SessionHelper.RoomID, SessionHelper.CompanyID, "#T");
            //string NewNumber = objAutoSequenceDTO.Prefix + "-" + SessionHelper.CompanyID + "-" + SessionHelper.RoomID + "-" + DateTime.Now.ToString("MMddyy") + "-" + objAutoSequenceDTO.LastGenereted;
            //string NewNumber = new AutoSequenceDAL(enterPriseDBName).GetLastGeneratedROOMID("NextToolNo", SessionHelper.RoomID, SessionHelper.CompanyID).ToString();
            string NewNumber = new AutoSequenceDAL(enterPriseDBName).GetNextAutoNumberByModule("NextToolNo", SessionHelper.RoomID, SessionHelper.CompanyID);

            ToolMasterDTO objDTO = new ToolMasterDTO();
            objDTO.ID = 0;
            //objDTO.ToolName = "#T" + NewNumber;
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

            ToolCategoryMasterDAL objToolCategory = new ToolCategoryMasterDAL(enterPriseDBName);
            List<ToolCategoryMasterDTO> lstToolCategory = objToolCategory.GetToolCategoryByRoomIDPlain(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            //lstToolCategory.Insert(0, new ToolCategoryMasterDTO() { ID = 0, ToolCategory = "-- Select Category --" });
            ViewBag.ToolCategoryList = lstToolCategory;

            LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(enterPriseDBName);
            List<LocationMasterDTO> lstLocation = objLocationCntrl.GetLocationByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            lstLocation = lstLocation.Where(t => t.Location != string.Empty).ToList();
            //lstLocation.Insert(0, new LocationMasterDTO() { ID = 0, Location = "-- Select Location --" });
            ViewBag.LocationList = lstLocation;


            TechnicialMasterDAL objTechnicianCntrl = new eTurns.DAL.TechnicialMasterDAL(enterPriseDBName);
            List<TechnicianMasterDTO> lstTechnician = objTechnicianCntrl.GetTechnicianByRoomIDNormal(SessionHelper.RoomID, SessionHelper.CompanyID);
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

        private List<CommonDTO> GetGroupOfItems()
        {
            List<CommonDTO> ItemType = new List<CommonDTO>();

            ItemType.Add(new CommonDTO() { ID = 1, Text = "Yes" });
            ItemType.Add(new CommonDTO() { ID = 0, Text = "No" });

            return ItemType;
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult ToolSave(ToolMasterDTO objDTO)
        {
            Int64 TempToolID = 0;
            string message = "";
            string status = "";
            LocationMasterDAL objLocationMasterDAL = new LocationMasterDAL(enterPriseDBName);
            ToolMasterDAL obj = this.objToolMasterDAL;//new ToolMasterDAL(enterPriseDBName);
            ToolLocationDetailsDTO objToolLocationDetailsDTO = null;
            ToolLocationDetailsDAL objToolLocationDetailsDAL = new ToolLocationDetailsDAL(enterPriseDBName);
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

            CommonDAL objCommon = new CommonDAL(enterPriseDBName);
            bool AllowToolOrdering = SessionHelper.AllowToolOrdering;
            if (objDTO.ID == 0)
            {

                Guid? UsedToolGUId = Guid.Empty;
                //else
                //{
                //    objDTO.ToolTypeTracking = "2";
                //    objDTO.SerialNumberTracking = true;
                //}
                objDTO.CreatedBy = SessionHelper.UserID;
                if (SessionHelper.AllowToolOrdering)
                {
                    if (UsedToolGUId == Guid.Empty && objDTO.Quantity == 1 && (!string.IsNullOrWhiteSpace(objDTO.ToolTypeTracking)) && objDTO.ToolTypeTracking.Contains("2"))
                    {
                        objDTO.ToolTypeTracking = "2";
                        objDTO.SerialNumberTracking = true;
                    }
                }
                //string strOK = objCommon.DuplicateCheck(objDTO.Serial ?? string.Empty, "add", objDTO.ID, "ToolMaster", "Serial", SessionHelper.RoomID, SessionHelper.CompanyID);
                string strOK = "ok";// obj.ToolSerialDuplicateCheck((objDTO.Serial ?? string.Empty).Trim(), objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID);

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
                    TempToolID = obj.Insert(objDTO, AllowToolOrdering);

                    ToolMasterDTO objToolMasterDTO = obj.GetToolByIDPlain(TempToolID);
                    if (UsedToolGUId == Guid.Empty && objToolMasterDTO != null)
                    {
                        UsedToolGUId = objToolMasterDTO.GUID;
                    }
                    if (SessionHelper.AllowToolOrdering)
                    {
                        if (objDTO.LocationID.GetValueOrDefault(0) <= 0)
                        {
                            objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(UsedToolGUId ?? Guid.Empty, string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "AssetController>>ToolSave");
                        }

                        LocationMasterDTO objLocationMasterDTO = null;
                        if (objDTO.LocationID.GetValueOrDefault(0) > 0)
                        {
                            LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(enterPriseDBName);

                            objLocationMasterDTO = objLocationCntrl.GetLocationByIDPlain(objDTO.LocationID.GetValueOrDefault(0), SessionHelper.RoomID, SessionHelper.CompanyID);
                            objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(UsedToolGUId ?? Guid.Empty, objLocationMasterDTO.Location, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "AssetController>>ToolSave");

                        }
                        if (!objDTO.SerialNumberTracking)
                        {
                            ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO = new ToolAssetQuantityDetailDTO();
                            objToolAssetQuantityDetailDTO.ToolGUID = UsedToolGUId;

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
                            objToolAssetQuantityDetailDTO.WhatWhereAction = "AssetController>>ToolSave";
                            objToolAssetQuantityDetailDTO.ReceivedDate = null;
                            objToolAssetQuantityDetailDTO.InitialQuantityWeb = objDTO.Quantity;
                            objToolAssetQuantityDetailDTO.InitialQuantityPDA = 0;
                            objToolAssetQuantityDetailDTO.ExpirationDate = null;
                            objToolAssetQuantityDetailDTO.EditedOnAction = "Tool Created From Web Page. insert Entry of Tool.";
                            objToolAssetQuantityDetailDTO.CreatedBy = SessionHelper.UserID;
                            objToolAssetQuantityDetailDTO.UpdatedBy = SessionHelper.UserID;
                            objToolAssetQuantityDetailDTO.IsDeleted = false;
                            objToolAssetQuantityDetailDTO.IsArchived = false;
                            objToolAssetQuantityDetailDTO.SerialNumber = objDTO.Serial;
                            ToolAssetQuantityDetailDAL objToolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(enterPriseDBName);
                            objToolAssetQuantityDetailDAL.Insert(objToolAssetQuantityDetailDTO, false, "AdjCredit", ReferalAction: "Initial Tool Create", SerialNumber: objDTO.Serial);
                        }
                    }
                    if (TempToolID > 0)
                    {
                        if (SessionHelper.AllowToolOrdering)
                        {
                            if (Session["ToolBinReplanish"] != null)
                            {
                                List<LocationMasterDTO> lstAllLocation = (List<LocationMasterDTO>)Session["ToolBinReplanish"];

                                if (lstAllLocation != null && lstAllLocation.Count() > 0 && lstAllLocation.Any())
                                {
                                    for (int i = 0; i < lstAllLocation.Count(); i++)
                                    {
                                        objLocationMasterDAL.GetToolLocation(UsedToolGUId ?? Guid.Empty, lstAllLocation[i].Location, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "AssetController>>ToolSave");
                                    }
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
                string strOK = "ok";// obj.ToolSerialDuplicateCheck((objDTO.Serial ?? string.Empty).Trim(), objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID);
                                    //string strOK = objCommon.DuplicateCheck(objDTO.Serial ?? string.Empty, "edit", objDTO.ID, "ToolMaster", "Serial", SessionHelper.RoomID, SessionHelper.CompanyID);

                if (AllowToolOrdering)
                {
                    if (objDTO.SerialNumberTracking)
                    {
                        strOK = obj.ToolNameDuplicateCheck((objDTO.ToolName ?? string.Empty).Trim(), objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    }
                    else
                    {
                        strOK = obj.ToolNameSerialDuplicateCheck((objDTO.ToolName ?? string.Empty).Trim() + "$" + (objDTO.Serial ?? string.Empty).Trim(), objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    }
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
                    LocationMasterDTO objLocationMasterDTO = null;
                    Guid? UsedToolGUId = Guid.Empty;
                    if (!string.IsNullOrWhiteSpace(objDTO.ToolTypeTracking) && objDTO.ToolTypeTracking.Contains("2"))
                    {
                        objDTO.SerialNumberTracking = true;
                    }
                    if (obj.Edit(objDTO, AllowToolOrdering))
                    {
                        if (SessionHelper.AllowToolOrdering)
                        {
                            // UsedToolGUId = obj.GetUsedToolGuidinQuantity(SessionHelper.RoomID, SessionHelper.CompanyID, objDTO.GUID, objDTO.ToolName);
                            UsedToolGUId = objDTO.GUID;
                            if ((UsedToolGUId ?? Guid.Empty) == Guid.Empty)
                            {
                                UsedToolGUId = objDTO.GUID;
                            }


                            if (objDTO.LocationID.GetValueOrDefault(0) <= 0)
                            {
                                objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(UsedToolGUId ?? objDTO.GUID, string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "AssetController>>ToolSave");
                            }
                            // ToolMasterDTO objToolMasterDTO = obj.GetToolNameBySerial(objDTO.Serial, SessionHelper.RoomID, SessionHelper.CompanyID);



                            if (objDTO.LocationID.GetValueOrDefault(0) > 0)
                            {
                                LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(enterPriseDBName);
                                //List<LocationMasterDTO> lstLocation = objLocationCntrl.GetLocationForRoomWise(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
                                objLocationMasterDTO = objLocationCntrl.GetLocationByIDPlain(objDTO.LocationID.GetValueOrDefault(0), SessionHelper.RoomID, SessionHelper.CompanyID);
                                //lstLocation.Where(i => i.ID == objDTO.LocationID).FirstOrDefault();
                                objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(UsedToolGUId ?? objDTO.GUID, objLocationMasterDTO.Location, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "AssetController>>ToolSave");

                            }
                            if (tempobjDTO.Quantity != objDTO.Quantity && (!objDTO.SerialNumberTracking))
                            {
                                ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO = new ToolAssetQuantityDetailDTO();
                                objToolAssetQuantityDetailDTO.ToolGUID = UsedToolGUId;

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
                                objToolAssetQuantityDetailDTO.WhatWhereAction = "AssetController>>ToolSave";
                                objToolAssetQuantityDetailDTO.ReceivedDate = null;
                                objToolAssetQuantityDetailDTO.InitialQuantityWeb = objDTO.Quantity;
                                objToolAssetQuantityDetailDTO.InitialQuantityPDA = 0;
                                objToolAssetQuantityDetailDTO.ExpirationDate = null;
                                objToolAssetQuantityDetailDTO.EditedOnAction = "Tool Update From Web Page. Update Entry of Tool.";
                                objToolAssetQuantityDetailDTO.CreatedBy = SessionHelper.UserID;
                                objToolAssetQuantityDetailDTO.UpdatedBy = SessionHelper.UserID;
                                objToolAssetQuantityDetailDTO.IsDeleted = false;
                                objToolAssetQuantityDetailDTO.IsArchived = false;
                                objToolAssetQuantityDetailDTO.SerialNumber = objDTO.Serial;
                                ToolAssetQuantityDetailDAL objToolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(enterPriseDBName);
                                objToolAssetQuantityDetailDAL.UpdateOrInsert(objToolAssetQuantityDetailDTO, null, ReferalAction: "Initial Tool Create", SerialNumber: objDTO.Serial);
                            }
                            if (tempobjDTO.Serial != objDTO.Serial)
                            {
                                ToolAssetQuantityDetailDAL objToolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(enterPriseDBName);
                                objToolAssetQuantityDetailDAL.UpdateSerial(UsedToolGUId ?? Guid.Empty, tempobjDTO.Serial, objDTO.Serial);
                            }
                        }
                        TempToolID = objDTO.ID;
                        if (SessionHelper.AllowToolOrdering)
                        {
                            if (Session["ToolBinReplanish"] != null)
                            {
                                List<LocationMasterDTO> lstAllLocation = (List<LocationMasterDTO>)Session["ToolBinReplanish"];

                                if (lstAllLocation != null && lstAllLocation.Count() > 0 && lstAllLocation.Any())
                                {
                                    for (int i = 0; i < lstAllLocation.Count(); i++)
                                    {
                                        if (!string.IsNullOrWhiteSpace(lstAllLocation[i].Location))
                                        {
                                            objLocationMasterDTO = objLocationMasterDAL.GetToolLocation(UsedToolGUId ?? Guid.Empty, lstAllLocation[i].Location, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "AssetController>>ToolSave");
                                            lstAllLocation[i].GUID = objLocationMasterDTO.GUID;
                                        }
                                    }
                                }



                                ToolLocationDetailsDAL objToolLocationDetailDAL = new ToolLocationDetailsDAL(enterPriseDBName);
                                List<ToolLocationDetailsDTO> lstToolLocationDetailsDTO = objToolLocationDetailDAL.GetToolLocationsByToolGUID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, objDTO.GUID).ToList();

                                //List<LocationMasterDTO> lstToolLocation = new List<LocationMasterDTO>(lstLocation.ToList());
                                List<Guid> objGUIDList = lstAllLocation.Select(u => u.GUID).ToList();

                                // List<ToolLocationDetailsDTO> objExcludeGUIDList = lstToolLocationDetailsDTO.Select(u => (!objGUIDList.Contains(u.ToolLocationGuid)).
                                lstToolLocationDetailsDTO = lstToolLocationDetailsDTO.Where(l => (!objGUIDList.Contains(l.LocationGUID))).ToList();
                                if (lstToolLocationDetailsDTO != null && lstToolLocationDetailsDTO.Count() > 0 && lstToolLocationDetailsDTO.Any())
                                {
                                    foreach (ToolLocationDetailsDTO t in lstToolLocationDetailsDTO)
                                    {
                                        if (!string.IsNullOrWhiteSpace(t.ToolLocationName))
                                        {
                                            objToolLocationDetailDAL.DeleteByToolLocationGuid(t.LocationGUID, SessionHelper.UserID, "ToolController>>ToolSave", "Web", UsedToolGUId ?? Guid.Empty);
                                        }
                                    }
                                }
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

        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>
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


            ToolMasterDAL obj = this.objToolMasterDAL;//new ToolMasterDAL(enterPriseDBName);
            ToolMasterDTO objDTO = new ToolMasterDTO();
            if (!IsChangeLog)
                objDTO = obj.GetToolByIDNormal(ID);
            else
                objDTO = obj.GetHistoryRecord(ID, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted);

            objDTO.IsOnlyFromItemUI = true;

            if ((objDTO.Type ?? 1) == 1)
            {
                ToolCategoryMasterDAL objToolCategory = new ToolCategoryMasterDAL(enterPriseDBName);
                List<ToolCategoryMasterDTO> lstToolCategory = objToolCategory.GetToolCategoryByRoomIDPlain(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
                //lstToolCategory.Insert(0, new ToolCategoryMasterDTO() { ID = 0, ToolCategory = "-- Select Category --" });
                ViewBag.ToolCategoryList = lstToolCategory;

                LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(enterPriseDBName);
                List<LocationMasterDTO> lstLocation = objLocationCntrl.GetLocationByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
                lstLocation = lstLocation.Where(t => (!string.IsNullOrWhiteSpace(t.Location))).ToList();
                //lstLocation.Insert(0, new LocationMasterDTO() { ID = 0, Location = "-- Select Location --" });
                ViewBag.LocationList = lstLocation;
                if (SessionHelper.AllowToolOrdering)
                {
                    ToolLocationDetailsDAL objToolLocationDetailDAL = new ToolLocationDetailsDAL(enterPriseDBName);
                    List<ToolLocationDetailsDTO> lstToolLocationDetailsDTO = objToolLocationDetailDAL.GetToolLocationsByToolGUID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, objDTO.GUID).ToList();

                    List<LocationMasterDTO> lstAllLocation = new List<LocationMasterDTO>(lstLocation.ToList());
                    lstToolLocationDetailsDTO = lstToolLocationDetailsDTO.Where(t => (!string.IsNullOrWhiteSpace(t.ToolLocationName))).ToList();
                    List<Guid> objGUIDList = lstToolLocationDetailsDTO.Select(u => u.LocationGUID).ToList();
                    lstAllLocation = lstAllLocation.Where(l => objGUIDList.Contains(l.GUID)).ToList();

                    Session["ToolBinReplanish"] = lstAllLocation;
                }
            }
            else
            {
                objDTO.KitToolQuantity = objDTO.Quantity;
                objDTO.KitToolName = objDTO.ToolName;
                objDTO.KitToolSerial = objDTO.Serial;
                ToolDetailDAL objToolDetailDAL = new ToolDetailDAL(enterPriseDBName);
                Session["ToolKitDetail"] = objToolDetailDAL.GetAllRecordsByKitGUID(objDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, true).ToList();

                objDTO.IsOnlyFromItemUI = true;
                LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(enterPriseDBName);
                List<LocationMasterDTO> lstLocation = objLocationCntrl.GetLocationByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
                lstLocation = lstLocation.Where(t => (!string.IsNullOrWhiteSpace(t.Location))).ToList();
                lstLocation.Insert(0, new LocationMasterDTO() { ID = 0, Location = ResCommon.MsgSelectLocation });
                ViewBag.LocationList = lstLocation;

                ToolLocationDetailsDAL objToolLocationDetailDAL = new ToolLocationDetailsDAL(enterPriseDBName);
                List<ToolLocationDetailsDTO> lstToolLocationDetailsDTO = objToolLocationDetailDAL.GetToolLocationsByToolGUID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, objDTO.GUID).ToList();

                List<LocationMasterDTO> lstAllLocation = new List<LocationMasterDTO>(lstLocation.ToList());
                List<Guid> objGUIDList = lstToolLocationDetailsDTO.Select(u => u.LocationGUID).ToList();
                lstAllLocation = lstAllLocation.Where(l => objGUIDList.Contains(l.GUID)).ToList();

                Session["ToolBinReplanish"] = lstAllLocation;
            }
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("ToolMaster");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;

            ViewBag.GropOfItemsBag = GetGroupOfItems();



            List<CommonDTO> lstcommon = obj.GetAllToolWrittenOffCategories(SessionHelper.CompanyID, SessionHelper.RoomID);
            lstcommon.Insert(0, new CommonDTO { ID = 0, Text = "" });
            ViewBag.ToolWrittenOffCategories = lstcommon;
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
                return PartialView("~/Views/Kit/_KitToolCreate.cshtml", objDTO);
            }
        }

        #region Ajax Data Provider

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult ToolListAjax(JQueryDataTableParamModel param)
        {
            ToolMasterDAL obj = this.objToolMasterDAL;//new eTurns.DAL.ToolMasterDAL(enterPriseDBName);

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
            string ToolType = Request["ToolType"].ToString();
            Session["ToolType"] = ToolType;
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetPagedTools(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, null, null, RoomDateFormat, CurrentTimeZone, Type: ToolType);

            //TechnicialMasterDAL objTechnicianCntrl = new eTurns.DAL.TechnicialMasterDAL(enterPriseDBName);
            //List<TechnicianMasterDTO> lstTechnician = objTechnicianCntrl.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
            //lstTechnician.Insert(0, new TechnicianMasterDTO() { GUID = Guid.Empty, Technician = Convert.ToString(eTurns.DTO.Resources.ResCommon.SelectTechnicianText), TechnicianCode = string.Empty });
            //ViewBag.TechnicianList = lstTechnician;

            //DataFromDB.ForEach(
            //        t => t.TechnicianList = lstTechnician
            //    );


            //var gridData = (from g in DataFromDB
            //                   select new
            //                   {
            //                       ID = g.ID,
            //                       Quantity = g.Quantity,
            //                       ToolName = g.ToolName
            //                   }).ToList();

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
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public JsonResult DeleteToolRecords(string ids)
        {
            try
            {
                string response = string.Empty;
                //WorkOrderDAL obj = new WorkOrderDAL(enterPriseDBName);
                //response=obj.DeleteRecords(ids, SessionHelper.UserID, SessionHelper.CompanyID, SessionHelper.RoomID);
                CommonDAL objCommonDAL = new CommonDAL(enterPriseDBName);
                response = objCommonDAL.DeleteModulewise(ids, "Tool", false, SessionHelper.UserID, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);
                //return "ok";
                eTurns.DAL.CacheHelper<IEnumerable<ToolMasterDTO>>.InvalidateCache();
                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);

                //ToolMasterDAL obj = new ToolMasterDAL(enterPriseDBName);
                //string MSG = "";
                //obj.DeleteRecords(ids, SessionHelper.UserID, SessionHelper.CompanyID, SessionHelper.RoomID, out MSG);

                //if (MSG == string.Empty)
                //    return "ok";
                //else
                //    return "Few Record(s) not fully checked in i.e. " + MSG.ToString();
            }
            catch (Exception)
            {
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public string DeleteCheckInCheckOutRecords(string ids)
        {
            try
            {
                string MSG = "";
                ToolCheckInOutHistoryDAL obj = new ToolCheckInOutHistoryDAL(enterPriseDBName);
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

        #endregion
        public string CheckOutAll(string arrItems)
        {
            string message = string.Empty;
            try
            {
                JavaScriptSerializer s = new JavaScriptSerializer();
                List<ToolCheckoutAll> toochkoutlist = s.Deserialize<List<ToolCheckoutAll>>(arrItems);

                if (toochkoutlist.Count == 0)
                {
                    message = ResCommon.MsgSomethingWrong;
                }
                else
                {
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
                    
                    message = message.TrimEnd2("<br />");
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
        public string CheckInAllNew(string arrItems)
        {
            string message = string.Empty;
            try
            {
                JavaScriptSerializer s = new JavaScriptSerializer();
                List<ToolCheckoutAll> toochkoutlist = s.Deserialize<List<ToolCheckoutAll>>(arrItems);
                var checkInMsg = ResToolMaster.MsgCheckInWithQty;
                foreach (ToolCheckoutAll tool in toochkoutlist)
                {
                    ToolMasterDAL objToolDAL = this.objToolMasterDAL;//new ToolMasterDAL(enterPriseDBName);
                    ToolCheckInHistoryDAL objCIDAL = new ToolCheckInHistoryDAL(enterPriseDBName);
                    ToolCheckInOutHistoryDAL objCICODAL = new ToolCheckInOutHistoryDAL(enterPriseDBName);
                    ToolCheckInHistoryDTO objCIDTO = new ToolCheckInHistoryDTO();
                    ToolCheckInOutHistoryDTO objCICODTO = new ToolCheckInOutHistoryDTO();

                    //List<ToolCheckInOutHistoryDTO> objToolCheckInOutHistoryDTOList = objCICODAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();

                    if (tool.ToolGUID == null || tool.ToolGUID == Guid.Empty)
                    {
                        message += (" " + ResToolMaster.MsgInvalidToolGuid + " <br />");
                        continue;
                    }
                    
                    ToolMasterDTO objtool = objToolDAL.GetToolByGUIDPlain(tool.ToolGUID);
                    
                    if (objtool == null || objtool.ID < 1)
                    {
                        message += (" " + ResToolMaster.MsgInvalidToolGuid + " <br />");
                        continue;
                    }
                    //objToolCheckInOutHistoryDTOList = objToolCheckInOutHistoryDTOList.Where(co => co.TechnicianGuid == tool.TechnicianGuid && co.ToolGUID == tool.ToolGUID).ToList();
                    List<ToolCheckInOutHistoryDTO> objToolCheckInOutHistoryDTOList = objCICODAL.GetTCIOHsByTechToolGUIDWithToolInfo(tool.ToolGUID, tool.TechnicianGuid ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID);
                    objCIDTO.CompanyID = SessionHelper.CompanyID;
                    objCIDTO.Created = DateTimeUtility.DateTimeNow;
                    objCIDTO.CreatedBy = SessionHelper.UserID;
                    objCIDTO.CreatedByName = SessionHelper.UserName;
                    objCIDTO.IsArchived = false;
                    objCIDTO.IsDeleted = false;
                    objCIDTO.LastUpdatedBy = SessionHelper.UserID;
                    objCIDTO.Room = SessionHelper.RoomID;
                    objCIDTO.RoomName = SessionHelper.RoomName;

                    //if (CheckInCheckOutGUID != "" && Guid.Parse(CheckInCheckOutGUID) != Guid.Empty)
                    //    objCIDTO.CheckInCheckOutGUID = Guid.Parse(CheckInCheckOutGUID);
                    objCIDTO.Updated = DateTimeUtility.DateTimeNow;
                    objCIDTO.UpdatedByName = SessionHelper.UserName;


                    objCIDTO.CheckedOutMQTY = 0;

                    objCIDTO.CheckInDate = DateTimeUtility.DateTimeNow;
                    objCIDTO.CheckOutStatus = "Check In";
                    objCIDTO.IsOnlyFromItemUI = true;
                    objCIDTO.AddedFrom = "Web";
                    objCIDTO.EditedFrom = "Web";
                    objCIDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objCIDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    //objCIDTO.TechnicianGuid = item.TechnicianGuid;
                    //if (!string.IsNullOrWhiteSpace(item.Technician))
                    //{
                    //    if ((from p in objTechnicialMasterDALList
                    //         where (p.TechnicianCode.ToLower().Trim() == (item.Technician.ToLower().Trim()) && p.IsDeleted == false && p.Room == SessionHelper.RoomID && p.CompanyID == SessionHelper.CompanyID)
                    //         select p).Any())
                    //    {
                    //        objCIDTO.TechnicianGuid = (from p in objTechnicialMasterDALList
                    //                                   where (p.TechnicianCode.ToLower().Trim() == (item.Technician.ToLower().Trim()) && p.IsDeleted == false && p.Room == SessionHelper.RoomID && p.CompanyID == SessionHelper.CompanyID)
                    //                                   select p).FirstOrDefault().GUID;
                    //    }
                    //}
                    objCIDTO.TechnicianGuid = tool.TechnicianGuid;
                    double checkinQty = tool.Quantity;
                    double CheckedoutQty = 0;
                    double CheckedoutMQty = 0;
                    foreach (var COHistroy in objToolCheckInOutHistoryDTOList.Where(o => (o.CheckedOutQTY > o.CheckedOutQTYCurrent || o.CheckedOutMQTY > o.CheckedOutMQTYCurrent) && o.TechnicianGuid == objCIDTO.TechnicianGuid && o.ToolGUID == tool.ToolGUID))
                    {
                        if (checkinQty > 0)
                        {
                            objCIDTO.CheckedOutQTY = 0;
                            objCIDTO.CheckedOutMQTY = 0;
                            if (COHistroy.CheckedOutQTY > 0)
                            {
                                objCIDTO.CheckedOutQTY = COHistroy.CheckedOutQTY - COHistroy.CheckedOutQTYCurrent >= checkinQty ? checkinQty : COHistroy.CheckedOutQTY - COHistroy.CheckedOutQTYCurrent;
                                CheckedoutQty += (objCIDTO.CheckedOutQTY ?? 0);
                                checkinQty = checkinQty - objCIDTO.CheckedOutQTY ?? 0;

                            }
                            else
                            {
                                objCIDTO.CheckedOutMQTY = COHistroy.CheckedOutMQTY - COHistroy.CheckedOutMQTYCurrent >= checkinQty ? checkinQty : COHistroy.CheckedOutMQTY - COHistroy.CheckedOutMQTYCurrent;
                                CheckedoutMQty += (objCIDTO.CheckedOutMQTY ?? 0);
                                checkinQty = checkinQty - objCIDTO.CheckedOutMQTY ?? 0;

                            }
                            objCIDTO.CheckInCheckOutGUID = COHistroy.GUID;
                            objCIDTO.TechnicianGuid = COHistroy.TechnicianGuid;
                            objCIDAL.Insert(objCIDTO);


                            ToolCheckInOutHistoryDTO objPrvCICODTO = objCICODAL.GetTCIOHByGUIDPlain(COHistroy.GUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                            if (objPrvCICODTO != null)
                            {
                                if (objCIDTO.CheckedOutQTY.GetValueOrDefault(0) > 0)
                                {
                                    objPrvCICODTO.CheckedOutQTYCurrent = objPrvCICODTO.CheckedOutQTYCurrent.GetValueOrDefault(0) + objCIDTO.CheckedOutQTY;
                                }
                                else
                                {
                                    objPrvCICODTO.CheckedOutMQTYCurrent = objPrvCICODTO.CheckedOutMQTYCurrent.GetValueOrDefault(0) + objCIDTO.CheckedOutMQTY;
                                }

                                objPrvCICODTO.IsOnlyFromItemUI = true;
                                objPrvCICODTO.EditedFrom = "Web";
                                objPrvCICODTO.ReceivedOn = DateTimeUtility.DateTimeNow;

                                objCICODAL.Edit(objPrvCICODTO);
                            }
                        }
                    }
                    checkinQty = (tool.Quantity) - checkinQty;
                    if (objtool != null && objtool.ID > 0)
                    {
                        objtool.IsOnlyFromItemUI = true;
                        objtool.CheckedOutQTY = objtool.CheckedOutQTY - CheckedoutQty;
                        objtool.CheckedOutMQTY = objtool.CheckedOutMQTY - CheckedoutMQty;
                        objtool.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objtool.EditedFrom = "Web";
                        message += (string.Format(checkInMsg, tool.ToolName, Convert.ToString(CheckedoutQty + CheckedoutMQty) + "<br />"));

                        // objtool.CheckedOutQTY = objtool.CheckedOutQTY > checkinQty ? objtool.CheckedOutQTY - checkinQty : objtool.CheckedOutQTY - objtool.CheckedOutQTY;
                        objToolDAL.Edit(objtool);
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
        public string CheckOutCheckIn(string ActionType, Int32 Quantity, bool IsForMaintance, Guid ToolGUID, Double AQty, Double CQty, Double CMQty, string UDF1, string UDF2, string UDF3, string UDF4, string UDF5, string CheckInCheckOutGUID, bool IsOnlyFromUI, string TechnicianName, Guid? RequisitionDetailGUID, Guid? WorkOrderGUID)
        {
            try
            {
                Guid TechnicianGuid = Guid.Empty;
                ToolCheckInOutHistoryDAL objCICODAL = new ToolCheckInOutHistoryDAL(enterPriseDBName);
                ToolCheckInOutHistoryDTO objCICODTO = new ToolCheckInOutHistoryDTO();

                ToolCheckInHistoryDAL objCIDAL = new ToolCheckInHistoryDAL(enterPriseDBName);
                ToolCheckInHistoryDTO objCIDTO = new ToolCheckInHistoryDTO();

                ToolMasterDAL objToolDAL = this.objToolMasterDAL;//new ToolMasterDAL(enterPriseDBName);
                ToolMasterDTO objToolDTO = null;

                if (SessionHelper.AllowToolOrdering)
                {
                    objToolDTO = objToolDAL.GetToolByGUIDFull(ToolGUID);
                }
                else
                {
                    objToolDTO = objToolDAL.GetToolByGUIDPlain(ToolGUID);
                }

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

                TechnicialMasterDAL objTechnicialMasterDAL = new TechnicialMasterDAL(enterPriseDBName);
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
                    UDFDAL objUDFApiController = new UDFDAL(enterPriseDBName);
                    IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetRequiredUDFsByUDFTableNamePlain("ToolCheckInOutHistory", SessionHelper.RoomID, SessionHelper.CompanyID);
                    string udfRequier = string.Empty;
                    var msgRequired = ResMessage.MsgRequired;
                    foreach (var i in DataFromDB)
                    {
                        
                            if (i.UDFColumnName == "UDF1" &&  string.IsNullOrEmpty(objCICODTO.UDF1))
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
                    if (TotalToolQty < (TotalCheckOutQty + TotalCheckMQty + TotalCheckOutForQty))
                    {
                        return ResToolMaster.MsgCheckoutQtyExceed; 
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
                    objCICODTO.SerialNumber = objToolDTO.Serial;
                    long CheckOutID = objCICODAL.Insert(objCICODTO);
                    Guid ToolCheckInOutHistoryGUID = objCICODAL.GetToolCheckinOutGUID(CheckOutID);
                    //ToolCheckInOutHistoryDTO objToolCheckInOutHistoryDTO = objCICODAL.GetRecordById(CheckOutID, SessionHelper.RoomID, SessionHelper.CompanyID);

                    objToolDTO.IsOnlyFromItemUI = IsOnlyFromUI;
                    if (IsOnlyFromUI)
                    {
                        objToolDTO.EditedFrom = "Web";
                        objToolDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    }
                    objToolDAL.Edit(objToolDTO);
                    if (SessionHelper.AllowToolOrdering)
                    {
                        //Guid? UsedToolGUId = objToolDAL.GetUsedToolGuidinQuantity(SessionHelper.RoomID, SessionHelper.CompanyID, objToolDTO.GUID, objToolDTO.ToolName);
                        Guid? UsedToolGUId = objToolDTO.GUID;

                        ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO = new ToolAssetQuantityDetailDTO();
                        objToolAssetQuantityDetailDTO.ToolGUID = UsedToolGUId;

                        objToolAssetQuantityDetailDTO.AssetGUID = null;


                        objToolAssetQuantityDetailDTO.ToolBinID = objToolDTO.ToolLocationDetailsID;
                        objToolAssetQuantityDetailDTO.Quantity = objToolDTO.Quantity;
                        objToolAssetQuantityDetailDTO.RoomID = SessionHelper.RoomID;
                        objToolAssetQuantityDetailDTO.CompanyID = SessionHelper.CompanyID;
                        objToolAssetQuantityDetailDTO.Created = DateTimeUtility.DateTimeNow;
                        objToolAssetQuantityDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                        objToolAssetQuantityDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objToolAssetQuantityDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objToolAssetQuantityDetailDTO.AddedFrom = "Web";
                        objToolAssetQuantityDetailDTO.EditedFrom = "Web";
                        objToolAssetQuantityDetailDTO.WhatWhereAction = "AssetController>>CheckOutCheckIn";
                        objToolAssetQuantityDetailDTO.ReceivedDate = null;
                        objToolAssetQuantityDetailDTO.InitialQuantityWeb = objToolDTO.Quantity;
                        objToolAssetQuantityDetailDTO.InitialQuantityPDA = 0;
                        objToolAssetQuantityDetailDTO.ExpirationDate = null;
                        objToolAssetQuantityDetailDTO.EditedOnAction = "Tool was CheckOut from Web.";
                        objToolAssetQuantityDetailDTO.CreatedBy = SessionHelper.UserID;
                        objToolAssetQuantityDetailDTO.UpdatedBy = SessionHelper.UserID;
                        objToolAssetQuantityDetailDTO.IsDeleted = false;
                        objToolAssetQuantityDetailDTO.IsArchived = false;

                        ToolAssetQuantityDetailDAL objToolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(enterPriseDBName);
                        objToolAssetQuantityDetailDAL.UpdateOrInsert(objToolAssetQuantityDetailDTO, Quantity, CheckoutGUID: ToolCheckInOutHistoryGUID, ReferalAction: "Check Out", SerialNumber: objToolDTO.Serial);
                    }

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
                    objCIDTO.SerialNumber = objToolDTO.Serial;
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
                    long objCOID = objCIDAL.Insert(objCIDTO);
                    //ToolCheckInHistoryDTO objToolCheckInHistoryDTO = objCIDAL.GetRecord(objCOID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    ToolCheckInHistoryDTO objToolCheckInHistoryDTO = objCIDAL.GetToolCheckInByIDPlain(objCOID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    objToolDTO.IsOnlyFromItemUI = IsOnlyFromUI;

                    if (IsOnlyFromUI)
                    {
                        objToolDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objToolDTO.EditedFrom = "Web";
                    }
                    objToolDAL.Edit(objToolDTO);

                    #endregion
                    if (SessionHelper.AllowToolOrdering)
                    {
                        //Guid? UsedToolGUId = objToolDAL.GetUsedToolGuidinQuantity(SessionHelper.RoomID, SessionHelper.CompanyID, objToolDTO.GUID, objToolDTO.ToolName);
                        Guid? UsedToolGUId = objToolDTO.GUID;
                        ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO = new ToolAssetQuantityDetailDTO();
                        objToolAssetQuantityDetailDTO.ToolGUID = UsedToolGUId ?? Guid.Empty;

                        objToolAssetQuantityDetailDTO.AssetGUID = null;


                        objToolAssetQuantityDetailDTO.ToolBinID = objToolDTO.ToolLocationDetailsID;
                        objToolAssetQuantityDetailDTO.Quantity = Quantity;
                        objToolAssetQuantityDetailDTO.RoomID = SessionHelper.RoomID;
                        objToolAssetQuantityDetailDTO.CompanyID = SessionHelper.CompanyID;
                        objToolAssetQuantityDetailDTO.Created = DateTimeUtility.DateTimeNow;
                        objToolAssetQuantityDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                        objToolAssetQuantityDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objToolAssetQuantityDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objToolAssetQuantityDetailDTO.AddedFrom = "Web";
                        objToolAssetQuantityDetailDTO.EditedFrom = "Web";
                        objToolAssetQuantityDetailDTO.WhatWhereAction = "AssetController>>CheckoutCheckIn";
                        objToolAssetQuantityDetailDTO.ReceivedDate = null;
                        objToolAssetQuantityDetailDTO.InitialQuantityWeb = objToolDTO.Quantity;
                        objToolAssetQuantityDetailDTO.InitialQuantityPDA = 0;
                        objToolAssetQuantityDetailDTO.ExpirationDate = null;
                        objToolAssetQuantityDetailDTO.EditedOnAction = "Tool was Checkin from Web.";
                        objToolAssetQuantityDetailDTO.CreatedBy = SessionHelper.UserID;
                        objToolAssetQuantityDetailDTO.UpdatedBy = SessionHelper.UserID;
                        objToolAssetQuantityDetailDTO.IsDeleted = false;
                        objToolAssetQuantityDetailDTO.IsArchived = false;

                        ToolAssetQuantityDetailDAL objToolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(enterPriseDBName);
                        objToolAssetQuantityDetailDAL.Insert(objToolAssetQuantityDetailDTO, false, CheckoutGUID: objToolCheckInHistoryDTO.CheckInCheckOutGUID, CheckinGUID: objToolCheckInHistoryDTO.GUID, ReferalAction: "Check In", SerialNumber: objToolDTO.Serial);
                    }
                    // Entry Create for # of checkouts
                    if (objToolDTO != null && objToolDTO.IsGroupOfItems.GetValueOrDefault(0) == 0 && objCIDTO != null && objCIDTO.ID > 0)
                        MaintCreateForNoOfCheckoutAtCheckIn(objToolDTO, objCIDTO);

                }

                if (CheckInCheckOutGUID != "" && Guid.Parse(CheckInCheckOutGUID) != Guid.Empty)
                {
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

        private void MaintCreateForNoOfCheckout(ToolMasterDTO objToolDTO, ToolCheckInOutHistoryDTO objCICODTO)
        {
            AssetMasterDAL objAssetMasterDAL = new AssetMasterDAL(enterPriseDBName);
            ToolsSchedulerDAL ToolsSchedulerDAL = new ToolsSchedulerDAL(enterPriseDBName);
            ToolsSchedulerDTO objToolsSchedulerDTO1 = null;
            List<ToolsSchedulerMappingDTO> lstToolsSchedulerMappingDTO = objAssetMasterDAL.GetSchedulerMappingRecordforTool_SchedularGUID(objToolDTO.GUID, SessionHelper.CompanyID, SessionHelper.RoomID, false, false);
            ToolsMaintenanceDAL objToolsMainDAL = new ToolsMaintenanceDAL(enterPriseDBName);
            ToolCheckInOutHistoryDAL objCICODAL = new ToolCheckInOutHistoryDAL(enterPriseDBName);

            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(enterPriseDBName);
            DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);

            foreach (var ToolsSchedulerMappingDTO in lstToolsSchedulerMappingDTO)
            {
                objToolsSchedulerDTO1 = ToolsSchedulerDAL.GetToolsSchedulerByGuidPlain(ToolsSchedulerMappingDTO.ToolSchedulerGuid.GetValueOrDefault(Guid.Empty));
                if (objToolsSchedulerDTO1 != null && objToolsSchedulerDTO1.SchedulerType == (int)MaintenanceScheduleType.CheckOuts)
                {
                    List<ToolCheckInOutHistoryDTO> lstToolCheckInOutHistoryDTO = null;
                    //lstToolCheckInOutHistoryDTO = objCICODAL.GetRecordByTool(objToolDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
                    lstToolCheckInOutHistoryDTO = objCICODAL.GetTCIOHsByToolGUIDWithToolInfo(objToolDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (lstToolCheckInOutHistoryDTO.Count() > objToolsSchedulerDTO1.CheckOuts) // Entery in Maintenance
                    {
                        ToolsMaintenanceDTO objToolsMainDTO = objToolsMainDAL.GetToolsMaintenanceSchedulerMappingPlain(null, objToolDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, objToolsSchedulerDTO1.GUID, ToolsSchedulerMappingDTO.GUID);
                        if (objToolsMainDTO == null) // First Entery in ToolsMaintenance
                        {
                            lstToolCheckInOutHistoryDTO = lstToolCheckInOutHistoryDTO.Where(x => x.GUID != objCICODTO.GUID).ToList();
                            double CalcScheduleDay = CalculateToolCheckoutDays(lstToolCheckInOutHistoryDTO, true);
                            ToolsMaintenanceDTO objTMDTO = new ToolsMaintenanceDTO();
                            objTMDTO.ToolGUID = objToolDTO.GUID;
                            objTMDTO.ScheduleDate = datetimetoConsider.Date.AddDays(CalcScheduleDay);
                            objTMDTO.SchedulerGUID = objToolsSchedulerDTO1.GUID;
                            objTMDTO.MaintenanceName = objToolsSchedulerDTO1.SchedulerName;
                            objTMDTO.AssetGUID = null;
                            objTMDTO.CompanyID = SessionHelper.CompanyID;
                            objTMDTO.Created = objCICODTO.Created.Value.AddSeconds(-1); //DateTimeUtility.DateTimeNow;
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
                            lstToolCheckInOutHistoryDTO = lstToolCheckInOutHistoryDTO.Where(x => x.GUID != objCICODTO.GUID && x.Created > objToolsMainDTO.Created).ToList();
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
                                objTMDTO.Created = objCICODTO.Created.Value.AddSeconds(-1); //DateTimeUtility.DateTimeNow;
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

        private void MaintCreateForNoOfCheckoutAtCheckIn(ToolMasterDTO objToolDTO, ToolCheckInHistoryDTO objCIDTO)
        {
            AssetMasterDAL objAssetMasterDAL = new AssetMasterDAL(enterPriseDBName);
            ToolsSchedulerDAL ToolsSchedulerDAL = new ToolsSchedulerDAL(enterPriseDBName);
            ToolsSchedulerDTO objToolsSchedulerDTO1 = null;
            List<ToolsSchedulerMappingDTO> lstToolsSchedulerMappingDTO = objAssetMasterDAL.GetSchedulerMappingRecordforTool_SchedularGUID(objToolDTO.GUID, SessionHelper.CompanyID, SessionHelper.RoomID, false, false);
            ToolsMaintenanceDAL objToolsMainDAL = new ToolsMaintenanceDAL(enterPriseDBName);
            ToolCheckInOutHistoryDAL objCICODAL = new ToolCheckInOutHistoryDAL(enterPriseDBName);
            ToolCheckInHistoryDAL objCIDAL = new ToolCheckInHistoryDAL(enterPriseDBName);

            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(enterPriseDBName);
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

        public string CheckInCheckOutData(Guid ToolGUID)
        {
            ViewBag.ToolGUID = ToolGUID;
            ToolCheckInOutHistoryDAL objDAL = new ToolCheckInOutHistoryDAL(enterPriseDBName);
            //var objModel = objDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.ToolGUID == ToolGUID).ToList();
            var objModel = objDAL.GetTCIOHsByToolGUIDFull(ToolGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
            return RenderRazorViewToString("_CheckInCheckOutData", objModel);
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
            ToolCheckInOutHistoryDAL objDAL = new ToolCheckInOutHistoryDAL(enterPriseDBName);
            int TotalRecordCount = 0;
            //var objModel = objDAL.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, ToolGUID, reqDetGUID, WOGUID, TCGUID);
            var objModel = objDAL.GetPagedToolCheckouts(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, ToolGUID, reqDetGUID, WOGUID, TCGUID, "list");



            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = objModel
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
        public void InsertCheckOutUDf(string UDFOption, string UDFColumn)
        {
            string UdfTablename = ImportMastersDTO.TableName.ToolCheckInOutHistory.ToString();
            Int64 UDFID = 0;
            //List<UDFDTO> objUDFDT = new List<UDFDTO>();
            UDFDAL objUDFDAL = new UDFDAL(enterPriseDBName);
            //int TotalRecordCount = 0;
            var udf = objUDFDAL.GetUDFByUDFColumnNamePlain(UDFColumn, UdfTablename, SessionHelper.RoomID, SessionHelper.CompanyID);
            
            if (udf != null && udf.ID > 0)
            {
                UDFID = udf.ID;
                CommonDAL objCDAL = new CommonDAL(enterPriseDBName);

                string strOK = objCDAL.DuplicateUFDOptionCheck(UDFOption, "add", 0, "UDFOptions", "UDFOption", UDFID);
                if (strOK == "duplicate")
                {

                }
                else
                {
                    //UDFOptionApiController obj = new UDFOptionApiController();
                    UDFOptionDAL obj = new UDFOptionDAL(enterPriseDBName);

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
        #endregion

        #region "Asset"

        public PartialViewResult _CreateAsset()
        {
            return PartialView();
        }

        public ActionResult AssetList()
        {
            return View();
        }






        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult AssetCreate()
        {
            //AutoSequenceNumbersDTO objAutoSequenceDTO = new AutoSequenceDAL(enterPriseDBName).GetLastGeneratedID(SessionHelper.RoomID, SessionHelper.CompanyID, "#A");
            //string NewNumber = objAutoSequenceDTO.Prefix + "-" + SessionHelper.CompanyID + "-" + SessionHelper.RoomID + "-" + DateTime.Now.ToString("MMddyy") + "-" + objAutoSequenceDTO.LastGenereted;
            // string NewNumber = new AutoSequenceDAL(enterPriseDBName).GetLastGeneratedROOMID("NextAssetNo", SessionHelper.RoomID, SessionHelper.CompanyID).ToString();
            string NewNumber = new AutoSequenceDAL(enterPriseDBName).GetNextAutoNumberByModule("NextAssetNo", SessionHelper.RoomID, SessionHelper.CompanyID).ToString();


            AssetMasterDTO objDTO = new AssetMasterDTO();
            objDTO.ID = 0;
            //objDTO.AssetName = "#A" + NewNumber;
            objDTO.AssetName = NewNumber;

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

            //Dropdown list

            //ToolCategoryMasterDAL objToolCategory = new eTurns.DAL.ToolCategoryMasterDAL(enterPriseDBName);
            //List<ToolCategoryMasterDTO> lstToolCategory = objToolCategory.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            //ViewBag.ToolCategoryList = lstToolCategory;

            //get asset categorylist data in viewbag
            AssetCategoryMasterDAL objAssetCategory = new AssetCategoryMasterDAL(enterPriseDBName);
            List<AssetCategoryMasterDTO> lstAssetCategory = objAssetCategory.GetAssetCategoryByRoom(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            ViewBag.AssetCategoryList = lstAssetCategory;

            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("AssetMaster");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            objDTO.PurchaseDateStr = objDTO.PurchaseDate.HasValue ? objDTO.PurchaseDate.Value.ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture) : string.Empty;
            objDTO.SuggestedMaintenanceDateStr = objDTO.SuggestedMaintenanceDate.HasValue ? objDTO.SuggestedMaintenanceDate.Value.ToString(SessionHelper.RoomDateFormat) : string.Empty;
            objDTO.ImageType = "ExternalImage";
            return PartialView("_CreateAsset", objDTO);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult AssetSave(AssetMasterDTO objDTO)
        {
            Int64 TempAssetID = 0;
            string message = "";
            string status = "";
            AssetMasterDAL obj = new AssetMasterDAL(enterPriseDBName);
            CommonDAL objCommonDAL = new CommonDAL(enterPriseDBName);

            if (string.IsNullOrEmpty(objDTO.AssetName))
            {
                message = string.Format(ResMessage.Required, ResAssetMaster.AssetName);// "Tool is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.Room = SessionHelper.RoomID;
            if (!string.IsNullOrEmpty(objDTO.PurchaseDateStr))
                objDTO.PurchaseDate = DateTime.ParseExact(objDTO.PurchaseDateStr, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult);
            if (!string.IsNullOrEmpty(objDTO.SuggestedMaintenanceDateStr))
                objDTO.SuggestedMaintenanceDate = DateTime.ParseExact(objDTO.SuggestedMaintenanceDateStr, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult);
            if (objDTO.ID == 0)
            {
                objDTO.CreatedBy = SessionHelper.UserID;

                string strOK = objCommonDAL.DuplicateCheck(objDTO.AssetName, "add", objDTO.ID, "AssetMaster", "AssetName", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResAssetMaster.AssetName, objDTO.AssetName);  //"Tool \"" + objDTO.ToolName + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    long ReturnVal = obj.Insert(objDTO);
                    if (ReturnVal > 0)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                        status = "fail";
                    }
                    objDTO.ID = ReturnVal;
                }
            }
            else
            {
                objDTO.LastUpdatedBy = SessionHelper.UserID;

                string strOK = objCommonDAL.DuplicateCheck(objDTO.AssetName, "edit", objDTO.ID, "AssetMaster", "AssetName", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResAssetMaster.AssetName, objDTO.AssetName);  //"Tool \"" + objDTO.ToolName + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    if (string.IsNullOrEmpty(objDTO.AddedFrom))
                    {
                        AssetMasterDTO tempdto = new AssetMasterDTO();
                        tempdto = obj.GetAssetById(objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                        if (tempdto != null)
                        {
                            objDTO.AddedFrom = tempdto.AddedFrom;
                            objDTO.ReceivedOnWeb = tempdto.ReceivedOnWeb;
                        }
                    }

                    if (objDTO.IsOnlyFromItemUI)
                    {
                        objDTO.EditedFrom = "Web";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    }
                    bool ReturnVal = obj.Edit(objDTO);
                    if (ReturnVal)
                    {
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
            TempAssetID = objDTO.ID;

            return Json(new { Message = message, Status = status, AssetID = TempAssetID }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>
        public ActionResult AssetEdit(Int64 ID)
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

            AssetMasterDAL obj = new AssetMasterDAL(enterPriseDBName);
            AssetMasterDTO objDTO = new AssetMasterDTO();
            if (!IsChangeLog)
                objDTO = obj.GetAssetById(ID, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted);
            else
                objDTO = obj.GetHistoryRecord(ID, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted);

            objDTO.IsOnlyFromItemUI = true;
            //Dropdown list
            //ToolCategoryMasterDAL objToolCategory = new eTurns.DAL.ToolCategoryMasterDAL(enterPriseDBName);
            //List<ToolCategoryMasterDTO> lstToolCategory = objToolCategory.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            //ViewBag.ToolCategoryList = lstToolCategory;

            //get asset categorylist data in viewbag
            AssetCategoryMasterDAL objAssetCategory = new AssetCategoryMasterDAL(enterPriseDBName);
            List<AssetCategoryMasterDTO> lstAssetCategory = objAssetCategory.GetAssetCategoryByRoom(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            ViewBag.AssetCategoryList = lstAssetCategory;

            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("AssetMaster");
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

            objDTO.PurchaseDateStr = objDTO.PurchaseDate.HasValue ? objDTO.PurchaseDate.Value.ToString(SessionHelper.RoomDateFormat, SessionHelper.RoomCulture) : string.Empty;
            objDTO.SuggestedMaintenanceDateStr = objDTO.SuggestedMaintenanceDate.HasValue ? objDTO.SuggestedMaintenanceDate.Value.ToString(SessionHelper.RoomDateFormat) : string.Empty;
            return PartialView("_CreateAsset", objDTO);
        }

        #region Ajax Data Provider

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult AssetListAjax(JQueryDataTableParamModel param)
        {
            AssetMasterDAL obj = new AssetMasterDAL(enterPriseDBName);

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
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            //var DataFromDB = obj.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, Convert.ToString(SessionHelper.RoomDateFormat));
            var DataFromDB = obj.GetPagedAssetMaster(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone);
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
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public string DeleteAssetRecords(string ids)
        {
            try
            {
                AssetMasterDAL obj = new AssetMasterDAL(enterPriseDBName);
                obj.DeleteRecords(ids, SessionHelper.UserID, SessionHelper.CompanyID);
                return "ok";
                //return "not ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #endregion

        #region "Check IN History"
        public string CheckInData(Guid CheckInCheckOutGUID)
        {
            ViewBag.CheckInCheckOutGUID = CheckInCheckOutGUID;
            ToolCheckInHistoryDAL objDAL = new ToolCheckInHistoryDAL(enterPriseDBName);
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
            ToolCheckInHistoryDAL objDAL = new ToolCheckInHistoryDAL(enterPriseDBName);
            int TotalRecordCount = 0;
            var objModel = objDAL.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, CheckInCheckOutGUID);

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
                ToolCheckInHistoryDAL obj = new ToolCheckInHistoryDAL(enterPriseDBName);
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
        #endregion

        #region "Tools/Assets Schedule"

        //ScheduleCreate
        public PartialViewResult _CreateScheduler()
        {
            return PartialView();
        }

        public ActionResult SchedulerList()
        {
            return View();
        }
        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult SchedulerCreate(Guid? AssetGUID, Guid? ToolGUID)
        {
            ToolsSchedulerDTO objDTO = new ToolsSchedulerDTO();
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
                // objDTO.AssetGUID = AssetGUID.Value;
                AssetMasterDAL objAsset = new AssetMasterDAL(enterPriseDBName);
                ViewBag.ToolAssetName = objAsset.GetRecord(AssetGUID.Value, SessionHelper.RoomID, SessionHelper.CompanyID, false, false).AssetName;
            }

            if (ToolGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
            {
                //  objDTO.ToolGUID = ToolGUID.Value;
                ToolMasterDAL objTool = this.objToolMasterDAL;//new eTurns.DAL.ToolMasterDAL(enterPriseDBName);
                ToolMasterDTO objToolMasterDTO = new ToolMasterDTO();
                objToolMasterDTO = objTool.GetToolByGUIDPlain(ToolGUID.GetValueOrDefault(Guid.Empty));
                if (objToolMasterDTO != null && objToolMasterDTO.ID > 0)
                {
                    ViewBag.ToolAssetName = objToolMasterDTO.ToolName;
                }


            }

            objDTO.SchedulerType = (int)MaintenanceScheduleType.TimeBase;
            objDTO.MainScheduleType = (int)MaintenanceScheduleType.TimeBase;

            //Dropdown list
            ViewBag.ScheduleGUID = objDTO.GUID;
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("ToolsScheduler");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            return PartialView("_CreateScheduler", objDTO);
        }

        [HttpPost]
        public JsonResult SchedulerSave(ToolsSchedulerDTO objDTO)
        {
            string message = "";
            string status = "";

            ToolsSchedulerDAL obj = new ToolsSchedulerDAL(enterPriseDBName);
            CommonDAL objCDal = new CommonDAL(enterPriseDBName);

            if (string.IsNullOrEmpty(objDTO.SchedulerName))
            {
                message = string.Format(ResMessage.Required, ResToolsOdometer.EntryDate);
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.Room = SessionHelper.RoomID;


            //Radio selection logic
            if (objDTO.OperationalHours != null)
                objDTO.SchedulerType = (int)MaintenanceScheduleType.OperationalHours;

            if (objDTO.Mileage != null)
                objDTO.SchedulerType = (int)MaintenanceScheduleType.Mileage;


            if (objDTO.ID == 0)
            {
                objDTO.CreatedBy = SessionHelper.UserID;

                string strOK = objCDal.DuplicateCheck(objDTO.SchedulerName, "add", objDTO.ID, "ToolsScheduler", "SchedulerName", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResToolsScheduler.SchedulerName, objDTO.SchedulerName);  //"Tool \"" + objDTO.ToolName + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    //objDTO.GUID = Guid.NewGuid();
                    objDTO.ID = obj.Insert(objDTO);
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
                string strOK = objCDal.DuplicateCheck(objDTO.SchedulerName, "edit", objDTO.ID, "ToolsScheduler", "SchedulerName", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResToolsScheduler.SchedulerName, objDTO.SchedulerName);  //"Tool \"" + objDTO.ToolName + "\" already exist! Try with Another!";
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
            return Json(new { Message = message, Status = status, GUID = objDTO.GUID }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>
        public ActionResult ScheduleEdit(Guid GUID)
        {
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            if (IsDeleted || IsArchived)
            {
                ViewBag.ViewOnly = true;
            }

            ToolsSchedulerDAL obj = new eTurns.DAL.ToolsSchedulerDAL(enterPriseDBName);
            ToolsSchedulerDTO objDTO = obj.GetToolsSchedulerByGuidPlain(GUID);

            if (objDTO != null)
            {
                ViewBag.ScheduleGUID = objDTO.GUID;
                ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("ToolsScheduler");
                ViewData["UDF1"] = objDTO.UDF1;
                ViewData["UDF2"] = objDTO.UDF2;
                ViewData["UDF3"] = objDTO.UDF3;
                ViewData["UDF4"] = objDTO.UDF4;
                ViewData["UDF5"] = objDTO.UDF5;
            }
            return PartialView("_CreateScheduler", objDTO);
        }

        public ActionResult LoadScheduleList()
        {
            ToolsSchedulerDAL obj = new ToolsSchedulerDAL(enterPriseDBName);
            List<ToolsSchedulerMappingDTO> lstSchedule = null;
            if (!String.IsNullOrEmpty(Request.QueryString["AssetGUID"]))
            {
                Guid AssetGUID = Guid.Empty;
                if (Guid.TryParse(Request.QueryString["AssetGUID"].ToString(), out AssetGUID))
                {
                    AssetMasterDAL objAsset = new AssetMasterDAL(enterPriseDBName);
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
                    ToolMasterDAL objTool = this.objToolMasterDAL;//new eTurns.DAL.ToolMasterDAL(enterPriseDBName);

                    ToolMasterDTO objToolMasterDTO = new ToolMasterDTO();
                    objToolMasterDTO = objTool.GetToolByGUIDPlain(ToolGUID);
                    if (objToolMasterDTO != null && objToolMasterDTO.ID > 0)
                    {
                        ViewBag.ToolAssetName = objToolMasterDTO.ToolName;
                    }

                    lstSchedule = obj.GetAllSchedulesforToolAsset(ToolGUID, null, null);
                }
            }
            return PartialView("_SchedulerList", lstSchedule);
        }

        #region "Schedule Items"

        /// <summary>
        /// Create/Edit Page's Items
        /// </summary>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        public ActionResult LoadScheduleItems(Guid ScheduleGUID)
        {
            ViewBag.ScheduleGUID = ScheduleGUID;

            ToolsSchedulerDetailsDAL obj = new eTurns.DAL.ToolsSchedulerDetailsDAL(enterPriseDBName);
            //List<ToolsSchedulerDetailsDTO> lst = obj.GetAllRecords(ScheduleGUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
            List<ToolsSchedulerDetailsDTO> lst = obj.GetToolScheduleLineItemsNormal(ScheduleGUID, SessionHelper.RoomID, SessionHelper.CompanyID);

            return PartialView("_CreateSchedulerItems", lst);
        }

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public string ScheduleItemsDelete(string ids)
        {
            try
            {
                ToolsSchedulerDetailsDAL obj = new eTurns.DAL.ToolsSchedulerDetailsDAL(enterPriseDBName);
                obj.DeleteRecords(ids, SessionHelper.UserID, SessionHelper.CompanyID);
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public ActionResult LoadItemMasterModel(string ParentId, string ParentGuid)
        {
            ItemModelPerameter obj = new ItemModelPerameter()
            {
                AjaxURLAddItemToSession = "~/Assets/AddItemToDetailTable/",
                PerentID = ParentId,
                PerentGUID = ParentGuid,
                ModelHeader = eTurns.DTO.ResProjectMaster.ModelHeader,
                AjaxURLAddMultipleItemToSession = "~/Assets/AddItemToDetailTable/",
                AjaxURLToFillItemGrid = "~/Assets/GetItemsModelMethod/",
                CallingFromPageName = "AS",
                LoadNarrowSearchAfterListBind = true
            };

            return PartialView("ItemMasterModel", obj);
        }

        /// <summary>
        /// AddDetailItem
        /// </summary>
        /// <param name="para"></param>
        /// <param name="ItemID"></param>
        /// <param name="ItemGUID"></param>
        /// <param name="pQuentity"></param>
        /// <param name="QuickListID"></param>
        /// <param name="QuickListGuid"></param>
        /// <returns></returns>
        ///public JsonResult AddDetailItem(string para, Int64 ItemID, string ItemGUID, double pQuentity, Int64 QuickListID, string QuickListGuid)
        public JsonResult AddItemToDetailTable(string para)
        {
            string message = "";
            string status = "";
            JavaScriptSerializer s = new JavaScriptSerializer();
            ToolsSchedulerDetailsDTO[] QLDetails = s.Deserialize<ToolsSchedulerDetailsDTO[]>(para);
            ToolsSchedulerDetailsDAL objDAL = new eTurns.DAL.ToolsSchedulerDetailsDAL(enterPriseDBName);

            foreach (ToolsSchedulerDetailsDTO item in QLDetails)
            {
                ViewBag.ScheduleGUID = item.ScheduleGUID;
                item.Room = SessionHelper.RoomID;
                item.RoomName = SessionHelper.RoomName;
                item.CreatedBy = SessionHelper.UserID;
                item.CreatedByName = SessionHelper.UserName;
                item.UpdatedByName = SessionHelper.UserName;
                item.LastUpdatedBy = SessionHelper.UserID;
                item.CompanyID = SessionHelper.CompanyID;
                item.LastUpdated = DateTimeUtility.DateTimeNow;

                if (item.GUID != null && item.GUID != Guid.Empty)
                {
                    objDAL.Edit(item);
                }
                else
                {
                    item.Created = DateTimeUtility.DateTimeNow;
                    //List<ToolsSchedulerDetailsDTO> tempDTO = objDAL.GetCachedData(item.ScheduleGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(x => x.ItemGUID == item.ItemGUID).ToList();
                    List<ToolsSchedulerDetailsDTO> tempDTO = objDAL.GetToolScheduleLineItemByItemGUIDNormal(item.ScheduleGUID.GetValueOrDefault(Guid.Empty), item.ItemGUID ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (tempDTO == null || tempDTO.Count == 0)
                        objDAL.Insert(item);
                }
            }

            message = ResAssetMaster.MsgItemQtyUpdated;
            status = "ok";
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetItemsModelMethod(JQueryDataTableParamModel param)
        {
            //ItemMasterController obj = new ItemMasterController();
            ItemMasterDAL obj = new eTurns.DAL.ItemMasterDAL(enterPriseDBName);

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
            int ListAjaxRequestCount = Convert.ToInt32(Request["ListAjaxRequestCount"].ToString());

            // set the default column sorting here, if first time then required to set 
            //if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID";

            //make changes to resolve an issue of Sort (WI-431)
            if (sortColumnName == "0" || sortColumnName.Contains("undefined"))
                sortColumnName = "ItemNumber Asc";

            //if (sortDirection == "asc")
            //    sortColumnName = sortColumnName + " asc";
            //else
            //    sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;

            Guid QLID = Guid.Empty;
            Guid.TryParse(Convert.ToString(Request["ParentGUID"]), out QLID);

            int TotalRecordCount = 0;

            //object objQLItems = SessionHelper.Get(QuickListSessionKey + param.QuickListID.ToString());
            //List<ToolsSchedulerDetailsDTO> objQLItems = new ToolsSchedulerDetailsDAL(enterPriseDBName).GetAllRecords(QLID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
            List<ToolsSchedulerDetailsDTO> objQLItems = new ToolsSchedulerDetailsDAL(enterPriseDBName).GetToolScheduleLineItemsNormal(QLID, SessionHelper.RoomID, SessionHelper.CompanyID);
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

            IEnumerable<ItemMasterDTO> DataFromDB = null;
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            if (ListAjaxRequestCount == 1)
            {

                DataFromDB = obj.GetPagedRecordsForModel(param.iDisplayStart, int.MaxValue, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, ItemsIDs, "labor", "1,2", SessionHelper.UserSupplierIds, RoomDateFormat, CurrentTimeZone, "", true);
                Session["AssetItemNarrowSearch"] = DataFromDB;
                DataFromDB = DataFromDB.Take(param.iDisplayLength);
            }
            else
            {
                DataFromDB = obj.GetPagedRecordsForModel(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, ItemsIDs, "labor", "1,2", SessionHelper.UserSupplierIds, RoomDateFormat, CurrentTimeZone, "", true);
            }

            // .Where(x=>x.ItemType != 4); , as Labour Type item not required in this module


            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);
        }

        public ActionResult NarrowSearchForItemModel(CommonDTO objCommonDTO)
        {
            return PartialView("NarrowSearchForItemModel", objCommonDTO);
        }

        #endregion

        #region Ajax Data Provider


        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public string DeleteScheduleRecords(string ids)
        {
            try
            {
                ToolsSchedulerDAL obj = new ToolsSchedulerDAL(enterPriseDBName);
                obj.DeleteToolsSchedulersByGuids(ids, SessionHelper.UserID);
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #endregion

        #region "Tools/Assets Maintenance"

        public ActionResult LoadMaintenanceList()
        {
            ToolsMaintenanceDAL obj = new eTurns.DAL.ToolsMaintenanceDAL(enterPriseDBName);
            List<ToolsMaintenanceDTO> lstMaintenance = null;
            if (!String.IsNullOrEmpty(Request.QueryString["AssetGUID"]))
            {
                Guid AssetGUID = Guid.Empty;
                if (Guid.TryParse(Request.QueryString["AssetGUID"].ToString(), out AssetGUID))
                {
                    AssetMasterDAL objAsset = new AssetMasterDAL(enterPriseDBName);
                    ViewBag.ToolAssetName = objAsset.GetAssetMasterByGUID(AssetGUID, false, false).FirstOrDefault().AssetName;
                    lstMaintenance = obj.GetToolsMaintenanceByFilterNormal(AssetGUID, null, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
                }
            }
            else if (!String.IsNullOrEmpty(Request.QueryString["ToolGUID"]))
            {
                Guid ToolGUID = Guid.Empty;
                if (Guid.TryParse(Request.QueryString["ToolGUID"].ToString(), out ToolGUID))
                {
                    ToolMasterDAL objTool = this.objToolMasterDAL;//new eTurns.DAL.ToolMasterDAL(enterPriseDBName);

                    ToolMasterDTO objToolMasterDTO = new ToolMasterDTO();
                    objToolMasterDTO = objTool.GetToolByGUIDPlain(ToolGUID);
                    if (objToolMasterDTO != null && objToolMasterDTO.ID > 0)
                    {
                        ViewBag.ToolAssetName = objToolMasterDTO.ToolName;
                    }


                    lstMaintenance = obj.GetToolsMaintenanceByFilterNormal(null, ToolGUID, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
                }
            }
            return PartialView("_MaintenanceList", lstMaintenance);
        }

        public string MaintenanceItems(Guid MaintenanceGUID)
        {
            ViewBag.MaintenanceGUID = MaintenanceGUID;
            ToolMaintenanceDetailsDAL obj = new ToolMaintenanceDetailsDAL(enterPriseDBName);
            var DataFromDB = obj.GetMainteNanceDetails(MaintenanceGUID);
            return RenderRazorViewToString("_MaintenanceItems", DataFromDB);
        }

        public ActionResult CloseMaintenance(Guid MaintenanceGUID)
        {
            ToolsMaintenanceDAL objToolMaiDAL = new ToolsMaintenanceDAL(enterPriseDBName);
            ToolsMaintenanceDTO objToolMaiDTO = objToolMaiDAL.GetToolsMaintenanceByGuidPlain(MaintenanceGUID);
            if (objToolMaiDTO != null)
            {
                objToolMaiDTO.Status = "close";
                objToolMaiDAL.Edit(objToolMaiDTO);
                return Json(new { Status = "Ok" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Status = "Error" }, JsonRequestBehavior.AllowGet);
            }

        }

        public string DeleteMaintenanceRecords(string ids)
        {
            try
            {
                ToolsMaintenanceDAL obj = new eTurns.DAL.ToolsMaintenanceDAL(enterPriseDBName);
                obj.DeleteToolsMaintenanceByGuids(ids, SessionHelper.UserID);
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region "Odometer"
        //OdometerCreate
        public PartialViewResult _CreateOdometer()
        {
            return PartialView();
        }

        public ActionResult OdometerList()
        {
            return View();
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
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
                AssetMasterDAL objAsset = new AssetMasterDAL(enterPriseDBName);
                ViewBag.ToolAssetName = objAsset.GetAssetMasterByGUID(AssetGUID.Value, false, false).FirstOrDefault().AssetName;
            }

            if (ToolGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
            {
                objDTO.ToolGUID = ToolGUID.Value;
                ToolMasterDAL objTool = this.objToolMasterDAL;//new eTurns.DAL.ToolMasterDAL(enterPriseDBName);
                ToolMasterDTO objToolMasterDTO = new ToolMasterDTO();
                objToolMasterDTO = objTool.GetToolByGUIDPlain(ToolGUID.GetValueOrDefault(Guid.Empty));
                if (objToolMasterDTO != null && objToolMasterDTO.ID > 0)
                {
                    ViewBag.ToolAssetName = objToolMasterDTO.ToolName;
                }
            }


            //Dropdown list

            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("ToolsOdometer");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            return PartialView("_CreateOdometer", objDTO);
        }

        [HttpPost]
        public JsonResult OdometerSave(ToolsMaintenanceDTO objDTO)
        {
            string message = "";
            string status = "";

            objDTO.MaintenanceDate = DateTime.ParseExact(objDTO.EntryDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult);

            ToolsMaintenanceDAL obj = new ToolsMaintenanceDAL(enterPriseDBName);
            CommonDAL objCDal = new CommonDAL(enterPriseDBName);

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
                    ToolsSchedulerDAL objToolsSchedulerDAL = new ToolsSchedulerDAL(enterPriseDBName);
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

        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>
        public ActionResult OdometerEdit(Guid GUID)
        {
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            if (IsDeleted || IsArchived)
            {
                ViewBag.ViewOnly = true;
            }

            ToolsOdometerDAL obj = new eTurns.DAL.ToolsOdometerDAL(enterPriseDBName);
            ToolsOdometerDTO objDTO = obj.GetRecord(GUID, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted);

            return PartialView("_CreateOdometer", objDTO);
        }

        public ActionResult LoadOdometerList()
        {
            ToolsOdometerDAL obj = new ToolsOdometerDAL(enterPriseDBName);
            List<ToolsOdometerDTO> lstSchedule = null;
            if (!String.IsNullOrEmpty(Request.QueryString["AssetGUID"]))
            {
                Guid AssetGUID = Guid.Empty;
                if (Guid.TryParse(Request.QueryString["AssetGUID"].ToString(), out AssetGUID))
                {
                    AssetMasterDAL objAsset = new AssetMasterDAL(enterPriseDBName);
                    ViewBag.ToolAssetName = objAsset.GetRecord(AssetGUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false).AssetName;
                    lstSchedule = obj.GetAllRecords(AssetGUID, null, SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
                }
            }
            else if (!String.IsNullOrEmpty(Request.QueryString["ToolGUID"]))
            {
                Guid ToolGUID = Guid.Empty;
                if (Guid.TryParse(Request.QueryString["ToolGUID"].ToString(), out ToolGUID))
                {
                    ToolMasterDAL objTool = this.objToolMasterDAL;//new eTurns.DAL.ToolMasterDAL(enterPriseDBName);

                    ToolMasterDTO objToolMasterDTO = new ToolMasterDTO();
                    objToolMasterDTO = objTool.GetToolByGUIDPlain(ToolGUID);
                    if (objToolMasterDTO != null && objToolMasterDTO.ID > 0)
                    {
                        ViewBag.ToolAssetName = objToolMasterDTO.ToolName;
                    }

                    lstSchedule = obj.GetAllRecords(null, ToolGUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
                }
            }
            return PartialView("_OdometerList", lstSchedule);
        }

        public string DeleteOdometerRecords(string ids)
        {
            try
            {
                ToolsOdometerDAL obj = new eTurns.DAL.ToolsOdometerDAL(enterPriseDBName);
                obj.DeleteRecords(ids, SessionHelper.UserID, SessionHelper.CompanyID);
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        public ContentResult JobStart(DateTime Rundate)
        {
            AssetMasterDAL objAsset = new AssetMasterDAL(enterPriseDBName);
            if (objAsset.ExecuteJob(Rundate))
            {
                return Content("Job Executed Successfully");
            }
            else
            {
                return Content("ERROR");
            }
        }

        #region "Tool Asset New Schedule Master"
        //ScheduleCreate

        public PartialViewResult _AssetToolCreateScheduler()
        {
            return PartialView();
        }

        public ActionResult AssetToolSchedulerList()
        {
            return View();
        }
        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult AssetToolSchedulerCreate()
        {
            ToolsSchedulerDTO objDTO = new ToolsSchedulerDTO();
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
            objDTO.SchedulerType = (int)MaintenanceScheduleType.None;
            objDTO.MainScheduleType = (int)MaintenanceScheduleType.None;

            //Dropdown list
            ViewBag.ScheduleGUID = objDTO.GUID;
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("ToolsScheduler");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            return PartialView("_AssetToolCreateScheduler", objDTO);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult AssetToolSchedulerSave(ToolsSchedulerDTO objDTO)
        {
            string message = "";
            string status = "";

            ToolsSchedulerDAL obj = new ToolsSchedulerDAL(enterPriseDBName);
            SupplierMasterDAL objSupp = new SupplierMasterDAL(enterPriseDBName);
            CommonDAL objCDal = new CommonDAL(enterPriseDBName);

            if (string.IsNullOrEmpty(objDTO.SchedulerName))
            {
                message = string.Format(ResMessage.Required, ResToolsOdometer.EntryDate);
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.Room = SessionHelper.RoomID;

            //Radio selection logic
            //if (objDTO.OperationalHours != null)
            //    objDTO.ScheduleType = (int)MaintenanceScheduleType.OperationalHours;

            //if (objDTO.Mileage != null)
            //    objDTO.ScheduleType = (int)MaintenanceScheduleType.Mileage;

            //if (objDTO.ScheduleType == null)
            //    objDTO.ScheduleType = (int)MaintenanceScheduleType.TimeBase;

            if (objDTO.ID == 0)
            {
                objDTO.CreatedBy = SessionHelper.UserID;
                //string strOK = objCDal.DuplicateCheck(objDTO.SchedulerName, "add", objDTO.ID, "ToolsScheduler", "SchedulerName", SessionHelper.RoomID, SessionHelper.CompanyID);
                string strOK = string.Empty;
                ToolsSchedulerDTO objToolsSchedulerDTONew = obj.GetAssetToolSchedulerByName(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, objDTO.SchedulerName);
                if (objToolsSchedulerDTONew != null)
                {
                    if (objToolsSchedulerDTONew.ScheduleFor == objDTO.ScheduleFor)
                    {
                        strOK = "duplicate";
                    }
                }
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResToolsScheduler.SchedulerName, objDTO.SchedulerName);  //"Tool \"" + objDTO.ToolName + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    //objDTO.GUID = Guid.NewGuid();
                    objDTO.ID = obj.Insert(objDTO);
                    //ToolsSchedulerDTO objDTONew = obj.GetRecord(objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                    if (objDTO.SchedulerType == (int)MaintenanceScheduleType.TimeBase)
                    {
                        //objSupp.SaveSupplierScheduleAsset(SupplierScheduleDTO);
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
                string strOK = objCDal.DuplicateCheck(objDTO.SchedulerName, "edit", objDTO.ID, "ToolsScheduler", "SchedulerName", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResToolsScheduler.SchedulerName, objDTO.SchedulerName);  //"Tool \"" + objDTO.ToolName + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    bool result = obj.Edit(objDTO);


                    //Start: Delete Current Active and yet to start schedule.
                    //AssetMasterDAL assetDAL = new AssetMasterDAL(enterPriseDBName);
                    //ToolsMaintenanceDAL objToolMaintDAL = new ToolsMaintenanceDAL(enterPriseDBName);
                    //List<ToolsSchedulerMappingDTO> lstSchedulers = assetDAL.GetSchedulerMappingRecord(SessionHelper.CompanyID, SessionHelper.RoomID, false, false);
                    //lstSchedulers = lstSchedulers.Where(x => x.ToolSchedulerGuid.GetValueOrDefault(Guid.Empty) == objDTO.GUID).ToList();
                    //foreach (var schedule in lstSchedulers)
                    //{
                    //    IEnumerable<ToolsMaintenanceDTO> lstMaintenance = objToolMaintDAL.GetAllRecords(schedule.AssetGUID, schedule.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                    //    lstMaintenance = lstMaintenance.Where(x => x.SchedulerGUID == objDTO.GUID && x.MaintenanceDate == null
                    //                                            && x.TrackngMeasurement != (int)MaintenanceScheduleType.TimeBase
                    //                                            && x.Status == MaintenanceStatus.Open.ToString());
                    //    foreach (var maintaince in lstMaintenance)
                    //    {
                    //        objToolMaintDAL.DeleteRecords(maintaince.GUID.ToString(), SessionHelper.UserID);
                    //    }
                    //    objToolMaintDAL.CreateNewMaintenanceAuto(schedule.AssetGUID, schedule.ToolGUID, objDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                    //}

                    //End: Delete Current Active and yet to start schedule.


                    if (result)
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

            Session["IsInsert"] = "True";

            return Json(new { Message = message, Status = status, GUID = objDTO.GUID }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>
        public ActionResult AssetToolScheduleEdit(Guid GUID)
        {
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            ViewBag.ScheduleGUID = "";
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            if (IsDeleted || IsArchived)
            {
                ViewBag.ViewOnly = true;
            }

            ToolsSchedulerDAL obj = new eTurns.DAL.ToolsSchedulerDAL(enterPriseDBName);
            ToolsSchedulerDTO objDTO = obj.GetToolsSchedulerByGuidFull(GUID);

            if (objDTO != null)
            {
                ViewBag.ScheduleGUID = objDTO.GUID;
                ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("ToolsScheduler");
                ViewData["UDF1"] = objDTO.UDF1;
                ViewData["UDF2"] = objDTO.UDF2;
                ViewData["UDF3"] = objDTO.UDF3;
                ViewData["UDF4"] = objDTO.UDF4;
                ViewData["UDF5"] = objDTO.UDF5;
            }
            SchedulerDTO objSchDTo = new SchedulerDTO();
            objSchDTo = new SupplierMasterDAL(enterPriseDBName).GetRoomScheduleForAsset(GUID, SessionHelper.RoomID, Convert.ToInt32(objDTO.ScheduleFor));

            ViewBag.objSchDTo = objSchDTo;

            return PartialView("_AssetToolCreateScheduler", objDTO);
        }

        public ActionResult AssetToolLoadScheduleList()
        {
            ToolsSchedulerDAL obj = new ToolsSchedulerDAL(enterPriseDBName);
            List<ToolsSchedulerDTO> lstSchedule = null;
            if (!String.IsNullOrEmpty(Request.QueryString["AssetGUID"]))
            {
                Guid AssetGUID = Guid.Empty;
                if (Guid.TryParse(Request.QueryString["AssetGUID"].ToString(), out AssetGUID))
                {
                    AssetMasterDAL objAsset = new AssetMasterDAL(enterPriseDBName);
                    ViewBag.ToolAssetName = objAsset.GetRecord(AssetGUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false).AssetName;
                    lstSchedule = obj.GetToolsSchedulerByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
                }
            }
            else if (!String.IsNullOrEmpty(Request.QueryString["ToolGUID"]))
            {
                Guid ToolGUID = Guid.Empty;
                if (Guid.TryParse(Request.QueryString["ToolGUID"].ToString(), out ToolGUID))
                {
                    ToolMasterDAL objTool = this.objToolMasterDAL;//new eTurns.DAL.ToolMasterDAL(enterPriseDBName);

                    ToolMasterDTO objToolMasterDTO = new ToolMasterDTO();
                    objToolMasterDTO = objTool.GetToolByGUIDPlain(ToolGUID);
                    if (objToolMasterDTO != null && objToolMasterDTO.ID > 0)
                    {
                        ViewBag.ToolAssetName = objToolMasterDTO.ToolName;
                    }

                    lstSchedule = obj.GetToolsSchedulerByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
                }
            }
            return PartialView("_AssetToolSchedulerList", lstSchedule);
        }

        #region "Schedule Items"

        /// <summary>
        /// Create/Edit Page's Items
        /// </summary>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        public ActionResult LoadAssetToolScheduleItems(Guid ScheduleGUID)
        {
            ViewBag.ScheduleGUID = ScheduleGUID;

            ToolsSchedulerDetailsDAL obj = new eTurns.DAL.ToolsSchedulerDetailsDAL(enterPriseDBName);
            //List<ToolsSchedulerDetailsDTO> lst = obj.GetAllRecords(ScheduleGUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
            List<ToolsSchedulerDetailsDTO> lst = obj.GetToolScheduleLineItemsNormal(ScheduleGUID, SessionHelper.RoomID, SessionHelper.CompanyID);

            return PartialView("_AssetToolCreateSchedulerItems", lst);
        }

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public string AssetToolScheduleItemsDelete(string ids)
        {
            try
            {
                ToolsSchedulerDetailsDAL obj = new eTurns.DAL.ToolsSchedulerDetailsDAL(enterPriseDBName);
                obj.DeleteRecords(ids, SessionHelper.UserID, SessionHelper.CompanyID);
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public ActionResult LoadAssetToolItemMasterModel(string ParentId, string ParentGuid)
        {
            ItemModelPerameter obj = new ItemModelPerameter()
            {
                AjaxURLAddItemToSession = "~/Assets/AddItemToDetailTable/",
                PerentID = ParentId,
                PerentGUID = ParentGuid,
                ModelHeader = eTurns.DTO.ResProjectMaster.ModelHeader,
                AjaxURLAddMultipleItemToSession = "~/Assets/AddItemToDetailTable/",
                AjaxURLToFillItemGrid = "~/Assets/GetItemsModelMethod/",
                CallingFromPageName = "AS"
            };

            return PartialView("ItemMasterModel", obj);
        }

        /// <summary>
        /// AddDetailItem
        /// </summary>
        /// <param name="para"></param>
        /// <param name="ItemID"></param>
        /// <param name="ItemGUID"></param>
        /// <param name="pQuentity"></param>
        /// <param name="QuickListID"></param>
        /// <param name="QuickListGuid"></param>
        /// <returns></returns>
        ///public JsonResult AddDetailItem(string para, Int64 ItemID, string ItemGUID, double pQuentity, Int64 QuickListID, string QuickListGuid)
        public JsonResult AddAssetToolItemToDetailTable(string para)
        {
            string message = "";
            string status = "";
            JavaScriptSerializer s = new JavaScriptSerializer();
            ToolsSchedulerDetailsDTO[] QLDetails = s.Deserialize<ToolsSchedulerDetailsDTO[]>(para);
            ToolsSchedulerDetailsDAL objDAL = new eTurns.DAL.ToolsSchedulerDetailsDAL(enterPriseDBName);

            foreach (ToolsSchedulerDetailsDTO item in QLDetails)
            {
                ViewBag.ScheduleGUID = item.ScheduleGUID;
                item.Room = SessionHelper.RoomID;
                item.RoomName = SessionHelper.RoomName;
                item.CreatedBy = SessionHelper.UserID;
                item.CreatedByName = SessionHelper.UserName;
                item.UpdatedByName = SessionHelper.UserName;
                item.LastUpdatedBy = SessionHelper.UserID;
                item.CompanyID = SessionHelper.CompanyID;
                item.LastUpdated = DateTimeUtility.DateTimeNow;

                if (item.GUID != null && item.GUID != Guid.Empty)
                {
                    objDAL.Edit(item);
                }
                else
                {
                    item.Created = DateTimeUtility.DateTimeNow;
                    List<ToolsSchedulerDetailsDTO> tempDTO = objDAL.GetToolScheduleLineItemByItemGUIDNormal(item.ScheduleGUID.GetValueOrDefault(Guid.Empty), item.ItemGUID ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (tempDTO == null || tempDTO.Count == 0)
                        objDAL.Insert(item);
                }
            }

            message = ResAssetMaster.MsgItemQtyUpdated;
            status = "ok";
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAssetToolItemsModelMethod(JQueryDataTableParamModel param)
        {
            //ItemMasterController obj = new ItemMasterController();
            ItemMasterDAL obj = new eTurns.DAL.ItemMasterDAL(enterPriseDBName);

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
            if (sortColumnName == "0" || sortColumnName == "undefined")
                sortColumnName = "ID";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;

            Guid QLID = Guid.Empty;
            Guid.TryParse(Convert.ToString(Request["ParentGUID"]), out QLID);

            int TotalRecordCount = 0;

            //object objQLItems = SessionHelper.Get(QuickListSessionKey + param.QuickListID.ToString());
            //List<ToolsSchedulerDetailsDTO> objQLItems = new ToolsSchedulerDetailsDAL(enterPriseDBName).GetAllRecords(QLID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
            List<ToolsSchedulerDetailsDTO> objQLItems = new ToolsSchedulerDetailsDAL(enterPriseDBName).GetToolScheduleLineItemsNormal(QLID, SessionHelper.RoomID, SessionHelper.CompanyID);
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
            var DataFromDB = obj.GetPagedRecordsForModel(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, ItemsIDs, "labor", "2", SessionHelper.UserSupplierIds, RoomDateFormat, CurrentTimeZone, "", true);

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

        #region Ajax Data Provider

        public ActionResult AssetToolSchedulerListAjax(JQueryDataTableParamModel param)
        {
            ToolsSchedulerDAL obj = new eTurns.DAL.ToolsSchedulerDAL(enterPriseDBName);
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
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetPagedToolsScheduler(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, 
                                                Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone);
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);
        }

        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public JsonResult DeleteAssetToolScheduleRecords(string ids)
        {
            try
            {
                CommonDAL objCommonDAL = new CommonDAL(enterPriseDBName);
                string response = objCommonDAL.DeleteModulewise(ids, "ToolSchedules", false, SessionHelper.UserID, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);
                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Message = "fail", Status = "fail" }, JsonRequestBehavior.AllowGet);
            }

            //try
            //{
            //    ToolsSchedulerDAL obj = new eTurns.DAL.ToolsSchedulerDAL(enterPriseDBName);
            //    obj.DeleteRecords(ids, SessionHelper.UserID, SessionHelper.CompanyID);
            //    return "ok";

            //    //return "not ok";
            //}
            //catch (Exception ex)
            //{
            //    return ex.Message;
            //}
        }

        #endregion

        #endregion

        #region "Schedule Mapping"
        public ActionResult ScheduleMapping()
        {
            return View();
        }
        public ActionResult ToolScheduleMappingListAjax(JQueryDataTableParamModel param)
        {
            AssetMasterDAL obj = new AssetMasterDAL(enterPriseDBName);

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
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetPagedRecordsToolScheduleMapping(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.CompanyID, SessionHelper.RoomID, IsArchived, IsDeleted, Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone);
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);
        }
        public ActionResult ScheduleMappingCreate(Guid? ToolGUID, Guid? AssetGUID)
        {
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
            //clslist obj = new clslist();
            //obj.Text = "";
            //obj.Guid = "";

            //lstItem.Add(obj);
            return lstItem;
        }
        public JsonResult GetToolAsset(string ScheduleType)
        {
            List<SelectListItem> lstItem = null;// new List<SelectListItem>();
            try
            {
                if (ScheduleType == "1")
                {
                    AssetMasterDAL objAssetMasterDAL = new AssetMasterDAL(enterPriseDBName);
                    IEnumerable<AssetMasterDTO> IEAssetMasterDTO = null;
                    IEAssetMasterDTO = objAssetMasterDAL.GetAllAssetsByRoom(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).OrderBy(e => e.AssetName);
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
                    ToolMasterDAL objToolMasterDAL = this.objToolMasterDAL;//new ToolMasterDAL(enterPriseDBName);
                    List<ToolMasterDTO> IEToolMasterDTO = null;

                    IEToolMasterDTO = objToolMasterDAL.GetToolByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(e => e.ToolName).ToList();
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

        public JsonResult GetAssets(string ScheduleType)
        {
            List<SelectListItem> lstItem = null;// new List<SelectListItem>();
            try
            {

                AssetMasterDAL objAssetMasterDAL = new AssetMasterDAL(enterPriseDBName);
                IEnumerable<AssetMasterDTO> IEAssetMasterDTO = null;

                IEAssetMasterDTO = objAssetMasterDAL.GetAllAssetsByRoom(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).OrderBy(x => x.AssetName).ToList();
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

                ToolMasterDAL objToolMasterDAL = this.objToolMasterDAL;//new ToolMasterDAL(enterPriseDBName);
                List<ToolMasterDTO> IEToolMasterDTO = null;

                IEToolMasterDTO = objToolMasterDAL.GetToolByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(e => e.ToolName).ToList();
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
        public JsonResult GetToolsForMaintenance(string ScheduleType)
        {
            List<SelectListItem> lstItem = null;
            try
            {
                ToolMasterDAL objToolMasterDAL = this.objToolMasterDAL;//new ToolMasterDAL(enterPriseDBName);
                List<ToolMasterDTO> IEToolMasterDTO = null;
                IEToolMasterDTO = objToolMasterDAL.GetToolByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(c => c.ToolName).ToList();
                lstItem = new List<SelectListItem>();
                if (IEToolMasterDTO != null && IEToolMasterDTO.Count() > 0)
                {
                    foreach (var item in IEToolMasterDTO.Where(m => (!string.IsNullOrWhiteSpace(m.ToolName))).Select(m => new { m.ToolName }).Distinct().ToList())
                    {
                        SelectListItem obj = new SelectListItem();
                        obj.Text = item.ToolName;
                        obj.Value = item.ToolName.ToString();
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
        public JsonResult GetToolAssetsScheduler(string ScheduleType, Guid? ToolGUID, Guid? AssetGUID)
        {
            List<SelectListItem> lstItem = null;// new List<SelectListItem>();
            try
            {
                int MaintenanceType = 0;
                if (AssetGUID.HasValue)
                {
                    AssetMasterDTO objAssetMasterDTO = new AssetMasterDAL(enterPriseDBName).GetRecord(AssetGUID ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                    MaintenanceType = objAssetMasterDTO.MaintenanceType;
                }
                else if (ToolGUID.HasValue)
                {
                    //ToolMasterDTO objToolMasterDTO = new ToolMasterDAL(enterPriseDBName).GetToolByGUIDPlain(ToolGUID ?? Guid.Empty);
                    ToolMasterDTO objToolMasterDTO = this.objToolMasterDAL.GetToolByGUIDPlain(ToolGUID ?? Guid.Empty);
                    MaintenanceType = objToolMasterDTO.MaintenanceType;
                }
                ToolsSchedulerDAL objToolsSchedulerDAL = new ToolsSchedulerDAL(enterPriseDBName);
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
        public JsonResult GetToolAssetsSchedulerAutoComplete(string NameStartWith, string ScheduleType, Guid? ToolGUID, Guid? AssetGUID)
        {
            List<SelectListItem> lstItem = null;// new List<SelectListItem>();
            try
            {
                int MaintenanceType = 0;
                if (AssetGUID.HasValue)
                {
                    AssetMasterDTO objAssetMasterDTO = new AssetMasterDAL(enterPriseDBName).GetRecord(AssetGUID ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                    MaintenanceType = objAssetMasterDTO.MaintenanceType;
                }
                else if (ToolGUID.HasValue)
                {
                    //ToolMasterDTO objToolMasterDTO = new ToolMasterDAL(enterPriseDBName).GetToolByGUIDPlain(ToolGUID ?? Guid.Empty);
                    ToolMasterDTO objToolMasterDTO = this.objToolMasterDAL.GetToolByGUIDPlain(ToolGUID ?? Guid.Empty);
                    MaintenanceType = objToolMasterDTO.MaintenanceType;
                }
                ToolsSchedulerDAL objToolsSchedulerDAL = new ToolsSchedulerDAL(enterPriseDBName);
                IEnumerable<ToolsSchedulerDTO> IEToolsSchedulerDTO = null;
                if (!string.IsNullOrWhiteSpace(NameStartWith))
                {
                    NameStartWith = NameStartWith.Trim();
                }
                IEToolsSchedulerDTO = objToolsSchedulerDAL.GetAssetToolSchedulerAutoComplete(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Convert.ToInt16(ScheduleType), NameStartWith);
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
        public JsonResult GetSerialsByToolName(string NameStartWith, string ToolName)
        {
            List<SelectListItem> lstItem = null;// new List<SelectListItem>();
            try
            {

                ToolsSchedulerDAL objToolsSchedulerDAL = new ToolsSchedulerDAL(enterPriseDBName);
                IEnumerable<ToolMasterDTO> IEToolMasterDTO = null;
                if (!string.IsNullOrWhiteSpace(NameStartWith))
                {
                    NameStartWith = NameStartWith.Trim();
                }
                //IEToolMasterDTO = new ToolMasterDAL(enterPriseDBName).GetToolBySerialSearchAndName(NameStartWith, ToolName, SessionHelper.RoomID, SessionHelper.CompanyID);
                IEToolMasterDTO = this.objToolMasterDAL.GetToolBySerialSearchAndName(NameStartWith, ToolName, SessionHelper.RoomID, SessionHelper.CompanyID);

                lstItem = new List<SelectListItem>();
                if (IEToolMasterDTO != null && IEToolMasterDTO.Count() > 0)
                {
                    foreach (var item in IEToolMasterDTO)
                    {
                        SelectListItem obj = new SelectListItem();
                        obj.Text = item.Serial;
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
        public JsonResult GetToolAssetsScheduler(string NameStartWith, string ScheduleType, Guid? ToolGUID, Guid? AssetGUID)
        {
            List<SelectListItem> lstItem = null;// new List<SelectListItem>();
            try
            {
                int MaintenanceType = 0;
                if (AssetGUID.HasValue)
                {
                    AssetMasterDTO objAssetMasterDTO = new AssetMasterDAL(enterPriseDBName).GetRecord(AssetGUID ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                    MaintenanceType = objAssetMasterDTO.MaintenanceType;
                }
                else if (ToolGUID.HasValue)
                {
                    //ToolMasterDTO objToolMasterDTO = new ToolMasterDAL(enterPriseDBName).GetToolByGUIDPlain(ToolGUID ?? Guid.Empty);
                    ToolMasterDTO objToolMasterDTO = this.objToolMasterDAL.GetToolByGUIDPlain(ToolGUID ?? Guid.Empty);
                    MaintenanceType = objToolMasterDTO.MaintenanceType;
                }
                ToolsSchedulerDAL objToolsSchedulerDAL = new ToolsSchedulerDAL(enterPriseDBName);
                IEnumerable<ToolsSchedulerDTO> IEToolsSchedulerDTO = null;
                if (!string.IsNullOrWhiteSpace(NameStartWith))
                {
                    NameStartWith = NameStartWith.Trim();
                }
                IEToolsSchedulerDTO = objToolsSchedulerDAL.GetAssetToolSchedulerAutoComplete(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Convert.ToInt16(ScheduleType), NameStartWith);
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
        public JsonResult GetToolsScheduler(string ScheduleType, Guid? ToolGUID, Guid? AssetGUID)
        {
            List<SelectListItem> lstItem = null;// new List<SelectListItem>();
            try
            {
                int MaintenanceType = 0;
                if (AssetGUID.HasValue)
                {
                    AssetMasterDTO objAssetMasterDTO = new AssetMasterDAL(enterPriseDBName).GetRecord(AssetGUID ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                    MaintenanceType = objAssetMasterDTO.MaintenanceType;
                }
                else if (ToolGUID.HasValue)
                {
                    //ToolMasterDTO objToolMasterDTO = new ToolMasterDAL(enterPriseDBName).GetToolByGUIDPlain(ToolGUID ?? Guid.Empty);
                    ToolMasterDTO objToolMasterDTO = this.objToolMasterDAL.GetToolByGUIDPlain(ToolGUID ?? Guid.Empty);
                    MaintenanceType = objToolMasterDTO.MaintenanceType;
                }
                ToolsSchedulerDAL objToolsSchedulerDAL = new ToolsSchedulerDAL(enterPriseDBName);
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
        public JsonResult GetToolsSchedulerAutoComplete(string NameStartWith, string ScheduleType, Guid? ToolGUID, Guid? AssetGUID)
        {
            List<SelectListItem> lstItem = null;// new List<SelectListItem>();
            try
            {
                int MaintenanceType = 0;
                if (AssetGUID.HasValue)
                {
                    AssetMasterDTO objAssetMasterDTO = new AssetMasterDAL(enterPriseDBName).GetRecord(AssetGUID ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                    MaintenanceType = objAssetMasterDTO.MaintenanceType;
                }
                else if (ToolGUID.HasValue)
                {
                    //ToolMasterDTO objToolMasterDTO = new ToolMasterDAL(enterPriseDBName).GetToolByGUIDPlain(ToolGUID ?? Guid.Empty);
                    ToolMasterDTO objToolMasterDTO = this.objToolMasterDAL.GetToolByGUIDPlain(ToolGUID ?? Guid.Empty);
                    MaintenanceType = objToolMasterDTO.MaintenanceType;
                }
                ToolsSchedulerDAL objToolsSchedulerDAL = new ToolsSchedulerDAL(enterPriseDBName);
                IEnumerable<ToolsSchedulerDTO> IEToolsSchedulerDTO = null;
                if (!string.IsNullOrWhiteSpace(NameStartWith))
                {
                    NameStartWith = NameStartWith.Trim();
                }
                IEToolsSchedulerDTO = objToolsSchedulerDAL.GetAssetToolSchedulerAutoComplete(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Convert.ToInt16(ScheduleType), NameStartWith);

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
        public JsonResult GetAssetsScheduler(string ScheduleType, Guid? ToolGUID, Guid? AssetGUID)
        {
            List<SelectListItem> lstItem = null;// new List<SelectListItem>();
            try
            {
                int MaintenanceType = 0;
                if (AssetGUID.HasValue)
                {
                    AssetMasterDTO objAssetMasterDTO = new AssetMasterDAL(enterPriseDBName).GetRecord(AssetGUID ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                    MaintenanceType = objAssetMasterDTO.MaintenanceType;
                }
                else if (ToolGUID.HasValue)
                {
                    //ToolMasterDTO objToolMasterDTO = new ToolMasterDAL(enterPriseDBName).GetToolByGUIDPlain(ToolGUID ?? Guid.Empty);
                    ToolMasterDTO objToolMasterDTO = this.objToolMasterDAL.GetToolByGUIDPlain(ToolGUID ?? Guid.Empty);
                    MaintenanceType = objToolMasterDTO.MaintenanceType;
                }
                ToolsSchedulerDAL objToolsSchedulerDAL = new ToolsSchedulerDAL(enterPriseDBName);
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
        public JsonResult GetAssetsSchedulerAutoComplete(string NameStartWith, string ScheduleType, Guid? ToolGUID, Guid? AssetGUID)
        {
            List<SelectListItem> lstItem = null;// new List<SelectListItem>();
            try
            {
                int MaintenanceType = 0;
                if (AssetGUID.HasValue)
                {
                    AssetMasterDTO objAssetMasterDTO = new AssetMasterDAL(enterPriseDBName).GetRecord(AssetGUID ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                    MaintenanceType = objAssetMasterDTO.MaintenanceType;
                }
                else if (ToolGUID.HasValue)
                {
                    //ToolMasterDTO objToolMasterDTO = new ToolMasterDAL(enterPriseDBName).GetToolByGUIDPlain(ToolGUID ?? Guid.Empty);
                    ToolMasterDTO objToolMasterDTO = this.objToolMasterDAL.GetToolByGUIDPlain(ToolGUID ?? Guid.Empty);
                    MaintenanceType = objToolMasterDTO.MaintenanceType;
                }
                ToolsSchedulerDAL objToolsSchedulerDAL = new ToolsSchedulerDAL(enterPriseDBName);
                IEnumerable<ToolsSchedulerDTO> IEToolsSchedulerDTO = null;
                if (!string.IsNullOrWhiteSpace(NameStartWith))
                {
                    NameStartWith = NameStartWith.Trim();
                }
                IEToolsSchedulerDTO = objToolsSchedulerDAL.GetAssetToolSchedulerAutoComplete(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Convert.ToInt16(ScheduleType), NameStartWith);
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
            AssetMasterDAL objAssetMasterDAL = new AssetMasterDAL(enterPriseDBName);
            ToolsMaintenanceDAL objToolMaintDAL = new ToolsMaintenanceDAL(enterPriseDBName);

            if (objDTO.SchedulerFor == 1)
            {
                ToolsSchedulerDAL ToolsSchedulerDAL = new ToolsSchedulerDAL(enterPriseDBName);
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
                ToolMasterDAL objToolMasterDAL = this.objToolMasterDAL; //new ToolMasterDAL(enterPriseDBName);
                ToolsSchedulerDAL ToolsSchedulerDAL = new ToolsSchedulerDAL(enterPriseDBName);
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

                //if (objDTO.SchedulerType == 2)
                //{

                //    //Start: Delete Current Active and yet to start schedule.
                //    IEnumerable<ToolsMaintenanceDTO> lstMaintenance = objToolMaintDAL.GetAllRecords(objDTO.AssetGUID, objDTO.ToolGUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                //    lstMaintenance = lstMaintenance.Where(x => x.SchedulerGUID == objDTO.ToolSchedulerGuid && x.Status != "close");
                //    foreach (var item in lstMaintenance)
                //    {
                //        objToolMaintDAL.DeleteRecords(item.GUID.ToString(), SessionHelper.UserID);
                //    }
                //    //End: Delete Current Active and yet to start schedule.
                //    ToolsMaintenanceDTO objToolsMaintenanceDTO = new ToolsMaintenanceDTO();
                //    objToolsMaintenanceDTO.AssetGUID = objDTO.AssetGUID;
                //    objToolsMaintenanceDTO.Room = SessionHelper.RoomID;
                //    objToolsMaintenanceDTO.CompanyID = SessionHelper.CompanyID;
                //    objToolsMaintenanceDTO.CreatedBy = objDTO.CreatedBy;
                //    objToolsMaintenanceDTO.Created = DateTimeUtility.DateTimeNow;
                //    objToolsMaintenanceDTO.Updated = DateTimeUtility.DateTimeNow;
                //    objToolsMaintenanceDTO.LastUpdatedBy = objDTO.LastUpdatedBy;
                //    objToolsMaintenanceDTO.IsArchived = false;
                //    objToolsMaintenanceDTO.IsDeleted = false;
                //    objToolsMaintenanceDTO.MaintenanceDate = DateTimeUtility.DateTimeNow;
                //    objToolsMaintenanceDTO.MaintenanceName = objDTO.MaintenanceName;
                //    objToolsMaintenanceDTO.MaintenanceType = "unscheduled";
                //    objToolsMaintenanceDTO.ScheduleFor = objDTO.SchedulerFor;
                //    objToolsMaintenanceDTO.SchedulerType = objDTO.SchedulerType;
                //    objToolsMaintenanceDTO.Status = "start";
                //    objToolsMaintenanceDTO.ToolGUID = objDTO.ToolGUID;
                //    objToolMaintDAL.Insert(objToolsMaintenanceDTO);
                //}
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
        public ActionResult MappingEdit(Guid MappingGUID)
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

            AssetMasterDAL obj = new AssetMasterDAL(enterPriseDBName);
            ToolsSchedulerMappingDTO objDTO = new ToolsSchedulerMappingDTO();
            if (!IsChangeLog)
                objDTO = obj.GetSchedulerMappingRecord(MappingGUID, SessionHelper.CompanyID, SessionHelper.RoomID, IsArchived, IsDeleted);
            else
                objDTO = obj.GetSchedulerMappingRecord(MappingGUID, SessionHelper.CompanyID, SessionHelper.RoomID, IsArchived, IsDeleted);
            ViewBag.SheduleItemType = GetSheduleItemTypeOptions();

            if (objDTO.SchedulerFor == 1)
            {
                objDTO.AssetToolGUID = objDTO.AssetGUID;
            }
            else if (objDTO.SchedulerFor == 2)
            {
                objDTO.AssetToolGUID = objDTO.ToolGUID;
            }
            ViewBag.SheduleAssetTool = GetToolAssetList(Convert.ToString(objDTO.SchedulerFor));
            ViewBag.SchedulerForName = GetToolsSchedulerList(Convert.ToString(objDTO.SchedulerFor), objDTO.AssetGUID, objDTO.ToolGUID);
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("toolschedulemapping");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;
            return PartialView("_CreateScheduleMapping", objDTO);

        }
        public List<clslist> GetToolAssetList(string ScheduleType)
        {
            List<clslist> lstItem = new List<clslist>();
            try
            {
                if (ScheduleType == "1")
                {
                    AssetMasterDAL objAssetMasterDAL = new AssetMasterDAL(enterPriseDBName);
                    IEnumerable<AssetMasterDTO> IEAssetMasterDTO = null;

                    IEAssetMasterDTO = objAssetMasterDAL.GetAllAssetsByRoom(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

                    if (IEAssetMasterDTO != null && IEAssetMasterDTO.Count() > 0)
                    {
                        foreach (var item in IEAssetMasterDTO)
                        {
                            clslist obj = new clslist();
                            obj.Text = item.AssetName;
                            obj.Guid = item.GUID.ToString();
                            lstItem.Add(obj);
                        }
                    }
                }
                else if (ScheduleType == "2")
                {
                    ToolMasterDAL objToolMasterDAL = this.objToolMasterDAL; //new ToolMasterDAL(enterPriseDBName);
                    List<ToolMasterDTO> IEToolMasterDTO = null;

                    IEToolMasterDTO = objToolMasterDAL.GetToolByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID);

                    if (IEToolMasterDTO != null && IEToolMasterDTO.Count() > 0)
                    {
                        foreach (var item in IEToolMasterDTO.Where(t => (!string.IsNullOrWhiteSpace(t.ToolName))).Select(m => new { m.ToolName }).Distinct().ToList())
                        {
                            clslist obj = new clslist();
                            obj.Text = item.ToolName;
                            obj.Guid = item.ToolName.ToString();
                            lstItem.Add(obj);
                        }
                    }
                }



                return lstItem;

            }
            catch (Exception ex)
            {
                return lstItem;
                throw ex;
            }
            finally
            {
                lstItem = null;
            }
        }
        public List<clslist> GetToolsSchedulerList(string ScheduleType, Guid? AssetGUID, Guid? ToolGUID)
        {
            List<clslist> lstItem = new List<clslist>();
            try
            {

                ToolsSchedulerDAL objToolsSchedulerDAL = new ToolsSchedulerDAL(enterPriseDBName);
                IEnumerable<ToolsSchedulerDTO> IEToolsSchedulerDTO = null;

                IEToolsSchedulerDTO = objToolsSchedulerDAL.GetAssetToolScheduler(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Convert.ToInt16(ScheduleType));

                if (IEToolsSchedulerDTO != null && IEToolsSchedulerDTO.Count() > 0)
                {
                    foreach (var item in IEToolsSchedulerDTO)
                    {
                        clslist obj = new clslist();
                        obj.Text = item.SchedulerName;
                        obj.Guid = item.GUID.ToString();
                        lstItem.Add(obj);
                    }
                }

                return lstItem;

            }
            catch (Exception ex)
            {
                return lstItem;
                throw ex;
            }
            finally
            {
                lstItem = null;
            }
        }
        public JsonResult DeleteMappingRecords(string ids)
        {
            string response = string.Empty;
            try
            {
                AssetMasterDAL obj = new AssetMasterDAL(enterPriseDBName);
                response = obj.DeleteMappingRecords(ids, SessionHelper.UserID, SessionHelper.CompanyID);
                string currentCulture = "en-US";
                if (System.Web.HttpContext.Current != null)
                {
                    if (eTurns.DTO.Resources.ResourceHelper.CurrentCult != null)
                    {
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(eTurns.DTO.Resources.ResourceHelper.CurrentCult)))
                        {
                            currentCulture = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                        }
                    }
                }
                else
                {
                    eTurns.DTO.eTurnsRegionInfo objeTurnsRegionInfo = new RegionSettingDAL(enterPriseDBName).GetRegionSettingsById(RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                    currentCulture = objeTurnsRegionInfo.CultureName;
                }
                string ResourceFileCommon = ResourceRead.GetResourceFileFullPath(ResourceBaseFilePath, "ResCommon", currentCulture, SessionHelper.EnterPriceID, SessionHelper.CompanyID);
                string MsgRecordsDeleteSuccess = ResourceHelper.GetResourceValueByKeyAndFullFilePath("MsgRecordsDeleteSuccess", ResourceFileCommon);
                response = response + " " + MsgRecordsDeleteSuccess;
                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ScheduleMappingpart()
        {

            return View();
        }
        #endregion

        #region "Maintenance"

        public ActionResult ToolMaintenanceListAjax(JQueryDataTableParamModel param)
        {
            AssetMasterDAL obj = new AssetMasterDAL(enterPriseDBName);

            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
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

            Guid AssetGUID, ToolGUID;
            Guid.TryParse(Convert.ToString(Request["AssetGUID"]), out AssetGUID);
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

            int TotalRecordCount = 0;
            param.iDisplayLength = param.iDisplayLength == -1 ? int.MaxValue : param.iDisplayLength;
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetPagedRecordsToolMaintenance(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.CompanyID, SessionHelper.RoomID, IsArchived, IsDeleted, Convert.ToString(SessionHelper.RoomDateFormat), tabname, ToolGUID, AssetGUID, null, CurrentTimeZone);
            DataFromDB.ForEach(x =>
            {
                x.LastMaintenanceDateStr = FnCommon.ConvertDateByTimeZone(x.LastMaintenanceDate, true, true);
            });
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);
        }
        public ActionResult Maintenance()
        {
            return View();
            //ToolsMaintenanceDAL obj = new eTurns.DAL.ToolsMaintenanceDAL(enterPriseDBName);
            //List<ToolsMaintenanceDTO> lstMaintenance = null;
            ////if (!String.IsNullOrEmpty(Request.QueryString["AssetGUID"]))
            ////{
            ////    Guid AssetGUID = Guid.Empty;
            ////    if (Guid.TryParse(Request.QueryString["AssetGUID"].ToString(), out AssetGUID))
            ////    {
            ////        AssetMasterDAL objAsset = new AssetMasterDAL(enterPriseDBName);
            ////        ViewBag.ToolAssetName = objAsset.GetRecord(AssetGUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false).AssetName;
            ////        lstMaintenance = obj.GetAllRecords(AssetGUID, null, SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
            ////    }
            ////}
            ////else if (!String.IsNullOrEmpty(Request.QueryString["ToolGUID"]))
            ////{
            ////    Guid ToolGUID = Guid.Empty;
            ////    if (Guid.TryParse(Request.QueryString["ToolGUID"].ToString(), out ToolGUID))
            ////    {

            //lstMaintenance = obj.GetAllmaintenaceList(null, null, SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
            ////    }
            ////}
            //return View("Maintenance", lstMaintenance);
        }

        public ActionResult Maintenancepart(string TabName)
        {
            ViewBag.TabName = TabName;
            return View();
        }

        public ActionResult CreateMaintenance(string mt)
        {

            ToolsMaintenanceDTO objDTO = new ToolsMaintenanceDTO();
            ViewBag.SheduleItemType = GetSheduleItemTypeOptions();
            ViewBag.SheduleAssetTool = GetEmptyOption();
            ViewBag.ScheduleName = GetEmptyOption();
            objDTO.GUID = Guid.NewGuid();
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.IsArchived = false;
            objDTO.IsDeleted = false;
            //objDTO.SchedulerType = 2;
            if (!string.IsNullOrWhiteSpace(mt) && mt == "past")
            {
                objDTO.Status = MaintenanceStatus.Close.ToString();
                objDTO.MaintenanceType = MaintenanceType.Past.ToString();
            }
            else if (!string.IsNullOrWhiteSpace(mt) && mt == "unscheduled")
            {
                objDTO.Status = MaintenanceStatus.Open.ToString();
                objDTO.MaintenanceType = MaintenanceType.UnScheduled.ToString();
            }
            else
            {
                objDTO.Status = MaintenanceStatus.Close.ToString();
                objDTO.MaintenanceType = MaintenanceType.UnScheduled.ToString();
            }
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("toolsMaintenance");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            return PartialView("_Createmaintenance", objDTO);
        }
        
        public ActionResult MaintenanceEdit(Int64 ID)
        {
            ToolsMaintenanceDTO objDTO = new ToolsMaintenanceDTO();
            ToolsMaintenanceDAL objToolsMaintenanceDAL = new ToolsMaintenanceDAL(enterPriseDBName);
            objDTO = objToolsMaintenanceDAL.GetToolsMaintenanceByIdNormal(ID);
            ToolsSchedulerDAL objToolsSchedulerDALNew = new ToolsSchedulerDAL(enterPriseDBName);
            ToolsSchedulerDTO objToolsSchedulerDTONew = objToolsSchedulerDALNew.GetToolsSchedulerByGuidPlain(objDTO.SchedulerGUID ?? Guid.Empty);
            if (objToolsSchedulerDTONew != null)
            {
                objDTO.SchedulerName = objToolsSchedulerDTONew.SchedulerName;
            }
            if (objDTO.ScheduleFor == 2)
            {
                //ToolMasterDTO objToolMasterDTO = new ToolMasterDAL(enterPriseDBName).GetToolByGUIDPlain(objDTO.ToolGUID ?? Guid.Empty);
                ToolMasterDTO objToolMasterDTO = this.objToolMasterDAL.GetToolByGUIDPlain(objDTO.ToolGUID ?? Guid.Empty);

                if (objToolMasterDTO != null)
                {
                    objDTO.Serial = objToolMasterDTO.Serial;
                    objDTO.ToolName = objToolMasterDTO.ToolName;
                }
                objDTO.AssetToolGUID = objDTO.ToolGUID;
            }
            else
            {
                objDTO.AssetToolGUID = objDTO.AssetGUID;
            }
            ViewBag.SheduleItemType = GetSheduleItemTypeOptions();
            ViewBag.SheduleAssetTool = GetEmptyOption();
            ViewBag.ScheduleName = GetEmptyOption();

            ToolMaintenanceDetailsDAL obj = new eTurns.DAL.ToolMaintenanceDetailsDAL(enterPriseDBName);
            List<ToolMaintenanceDetailsDTO> lst = obj.GetToolMaintenanceDetailsByMaintenanceIdNormal(ID, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            if (objDTO != null)
            {
                objDTO.ToolMaintenanceListItem = lst;
            }
            ViewBag.NofLineItem = lst.Count();
            ViewBag.Cost = lst.Sum(x => double.Parse((x.Quantity.GetValueOrDefault(0) == 0 ? x.Quantity.GetValueOrDefault(0) : x.Quantity.GetValueOrDefault(0)).ToString()) * ((x.ItemCost.GetValueOrDefault(0)) / ((x.CostUOMValue ?? 0) == 0 ? 1 : (x.CostUOMValue ?? 1))));

            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("toolsMaintenance");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            return PartialView("_Createmaintenance", objDTO);
        }
        public JsonResult GetTrackingMeasurement(Guid ScheduleGUID)
        {
            string response = string.Empty;
            try
            {
                AssetMasterDAL obj = new AssetMasterDAL(enterPriseDBName);
                response = obj.GetTrackingMeasurement(ScheduleGUID);

                return Json(new { Message = response }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Message = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult MaintenanceSave(ToolsMaintenanceDTO objDTO)
        {
            int TrackingMeasurementValue;
            int.TryParse(objDTO.TrackingMeasurementValue, out TrackingMeasurementValue);
            string message = "";
            string status = "ok";
            if (objDTO.ScheduleFor == 2)
            {
                if (!string.IsNullOrWhiteSpace(objDTO.Serial))
                {
                    ToolMasterDAL objToolMasterDALNew = this.objToolMasterDAL; //new ToolMasterDAL(enterPriseDBName);
                    ToolMasterDTO objToolMasterNew = new ToolMasterDTO();
                    Guid objToolGuid = new Guid();
                    Guid.TryParse(objDTO.AssetToolGUID.ToString(), out objToolGuid);
                    if (objToolGuid != Guid.Empty)
                    {
                        objToolMasterNew = objToolMasterDALNew.GetToolByGUIDPlain(objToolGuid);
                        if (objToolMasterNew != null)
                        {
                            if (objToolMasterNew.Serial != objDTO.Serial)
                            {
                                ToolMasterDTO lstToolMasterDTO = objToolMasterDALNew.GetToolBySerialAndNamePlain(objDTO.ToolName, objDTO.Serial, SessionHelper.RoomID, SessionHelper.CompanyID);
                                ToolMasterDTO ToolMasterDTOForGroupOfItem = objToolMasterDALNew.GetToolByName(objToolMasterNew.ToolName, SessionHelper.RoomID, SessionHelper.CompanyID);
                                if (lstToolMasterDTO == null)
                                {
                                    objToolMasterNew = new ToolMasterDTO();
                                    objToolMasterNew.Serial = objDTO.Serial;
                                    objToolMasterNew.ToolName = objDTO.ToolName;
                                    objToolMasterNew.Created = DateTimeUtility.DateTimeNow;
                                    objToolMasterNew.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    objToolMasterNew.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                    objToolMasterNew.CreatedBy = SessionHelper.UserID;
                                    objToolMasterNew.Updated = DateTimeUtility.DateTimeNow;
                                    objToolMasterNew.LastUpdatedBy = SessionHelper.UserID;
                                    objToolMasterNew.Room = SessionHelper.RoomID;
                                    objToolMasterNew.IsArchived = false;
                                    objToolMasterNew.IsDeleted = false;
                                    objToolMasterNew.GUID = Guid.NewGuid();
                                    objToolMasterNew.CompanyID = SessionHelper.CompanyID;
                                    objToolMasterNew.UDF1 = string.Empty;
                                    objToolMasterNew.UDF2 = string.Empty;
                                    objToolMasterNew.UDF3 = string.Empty;
                                    objToolMasterNew.UDF4 = string.Empty;
                                    objToolMasterNew.UDF5 = string.Empty;
                                    objToolMasterNew.IsGroupOfItems = ToolMasterDTOForGroupOfItem.IsGroupOfItems;
                                    objToolMasterNew.Quantity = 1;
                                    objToolMasterNew.AddedFrom = "Web";
                                    objToolMasterNew.EditedFrom = "Web";
                                    objToolMasterNew.Description = string.Empty;
                                    objToolMasterDALNew.Insert(objToolMasterNew);
                                    lstToolMasterDTO = objToolMasterDALNew.GetToolBySerialAndNamePlain(objDTO.ToolName, objDTO.Serial, SessionHelper.RoomID, SessionHelper.CompanyID);
                                    if (lstToolMasterDTO != null)
                                    {
                                        objDTO.AssetToolGUID = lstToolMasterDTO.GUID;
                                    }

                                }
                            }
                        }
                    }
                    else
                    {
                        ToolMasterDTO lstToolMasterDTO = objToolMasterDALNew.GetToolBySerialAndNamePlain(objDTO.ToolName, objDTO.Serial, SessionHelper.RoomID, SessionHelper.CompanyID);
                        ToolMasterDTO ToolMasterDTOForGroupOfItem = objToolMasterDALNew.GetToolByName(objDTO.ToolName, SessionHelper.RoomID, SessionHelper.CompanyID);
                        if (lstToolMasterDTO == null)
                        {
                            objToolMasterNew = new ToolMasterDTO();
                            objToolMasterNew.Serial = objDTO.Serial;
                            objToolMasterNew.ToolName = objDTO.ToolName;
                            objToolMasterNew.Created = DateTimeUtility.DateTimeNow;
                            objToolMasterNew.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objToolMasterNew.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            objToolMasterNew.CreatedBy = SessionHelper.UserID;
                            objToolMasterNew.Updated = DateTimeUtility.DateTimeNow;
                            objToolMasterNew.LastUpdatedBy = SessionHelper.UserID;
                            objToolMasterNew.Room = SessionHelper.RoomID;
                            objToolMasterNew.IsArchived = false;
                            objToolMasterNew.IsDeleted = false;
                            objToolMasterNew.GUID = Guid.NewGuid();
                            objToolMasterNew.CompanyID = SessionHelper.CompanyID;
                            objToolMasterNew.UDF1 = string.Empty;
                            objToolMasterNew.UDF2 = string.Empty;
                            objToolMasterNew.UDF3 = string.Empty;
                            objToolMasterNew.UDF4 = string.Empty;
                            objToolMasterNew.UDF5 = string.Empty;
                            objToolMasterNew.AddedFrom = "Web";
                            objToolMasterNew.EditedFrom = "Web";
                            objToolMasterNew.IsGroupOfItems = ToolMasterDTOForGroupOfItem.IsGroupOfItems;
                            objToolMasterNew.Quantity = 1;
                            objToolMasterNew.Description = string.Empty;
                            objToolMasterDALNew.Insert(objToolMasterNew);
                            lstToolMasterDTO = objToolMasterDALNew.GetToolBySerialAndNamePlain(objDTO.ToolName, objDTO.Serial, SessionHelper.RoomID, SessionHelper.CompanyID);
                            if (lstToolMasterDTO != null)
                            {
                                objDTO.AssetToolGUID = lstToolMasterDTO.GUID;
                            }

                        }
                    }
                }
                else
                {
                    message = ResToolMaster.Serial + " is Required";
                    status = "fail";
                    return Json(new { Message = message, Status = status, DTO = objDTO }, JsonRequestBehavior.AllowGet);
                }
            }
            if (!string.IsNullOrWhiteSpace(objDTO.SchedulerName))
            {
                ToolsSchedulerDAL objToolsSchedulerDALNew = new ToolsSchedulerDAL(enterPriseDBName);
                ToolsSchedulerDTO objToolsSchedulerDTONew = new ToolsSchedulerDTO();
                Guid objGuid = new Guid();
                Guid.TryParse(objDTO.ToolSchedulerGuid.ToString(), out objGuid);
                if (objGuid != Guid.Empty)
                {
                    objToolsSchedulerDTONew = objToolsSchedulerDALNew.GetToolsSchedulerByGuidPlain(objGuid);
                    if (objToolsSchedulerDTONew != null)
                    {
                        if (objToolsSchedulerDTONew.SchedulerName != objDTO.SchedulerName)
                        {
                            objToolsSchedulerDTONew = objToolsSchedulerDALNew.GetAssetToolSchedulerByName(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, objDTO.SchedulerName);
                            if (objToolsSchedulerDTONew != null)
                            {
                                if (objToolsSchedulerDTONew.ScheduleFor != objDTO.ScheduleFor)
                                {
                                    objToolsSchedulerDTONew = null;
                                }
                            }
                            if (objToolsSchedulerDTONew == null)
                            {
                                objToolsSchedulerDTONew = new ToolsSchedulerDTO();
                                objToolsSchedulerDTONew.SchedulerName = objDTO.SchedulerName;
                                objToolsSchedulerDTONew.SchedulerType = (int)MaintenanceScheduleType.None;
                                objToolsSchedulerDTONew.Created = DateTimeUtility.DateTimeNow;
                                objToolsSchedulerDTONew.CreatedBy = SessionHelper.UserID;
                                objToolsSchedulerDTONew.Updated = DateTimeUtility.DateTimeNow;
                                objToolsSchedulerDTONew.LastUpdatedBy = SessionHelper.UserID;
                                objToolsSchedulerDTONew.Room = SessionHelper.RoomID;
                                objToolsSchedulerDTONew.IsArchived = false;
                                objToolsSchedulerDTONew.IsDeleted = false;
                                objToolsSchedulerDTONew.GUID = Guid.NewGuid();
                                objToolsSchedulerDTONew.CompanyID = SessionHelper.CompanyID;
                                objToolsSchedulerDTONew.UDF1 = string.Empty;
                                objToolsSchedulerDTONew.UDF2 = string.Empty;
                                objToolsSchedulerDTONew.UDF3 = string.Empty;
                                objToolsSchedulerDTONew.UDF4 = string.Empty;
                                objToolsSchedulerDTONew.UDF5 = string.Empty;
                                objToolsSchedulerDTONew.ScheduleFor = objDTO.ScheduleFor;
                                objToolsSchedulerDTONew.Description = string.Empty;
                                objToolsSchedulerDTONew.SchedulerType = 0;
                                objToolsSchedulerDTONew.TimeBasedFrequency = 0;
                                objToolsSchedulerDTONew.TimeBaseUnit = 0;
                                objToolsSchedulerDTONew.RecurringDays = 0;
                                objToolsSchedulerDTONew.CheckOuts = null;
                                objToolsSchedulerDALNew.Insert(objToolsSchedulerDTONew);
                                objDTO.ToolSchedulerGuid = objToolsSchedulerDTONew.GUID;
                            }
                        }
                    }
                }
                else
                {
                    objToolsSchedulerDTONew = objToolsSchedulerDALNew.GetAssetToolSchedulerByName(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, objDTO.SchedulerName);
                    if (objToolsSchedulerDTONew != null)
                    {
                        if (objToolsSchedulerDTONew.ScheduleFor != objDTO.ScheduleFor)
                        {
                            objToolsSchedulerDTONew = null;
                        }
                    }
                    if (objToolsSchedulerDTONew == null)
                    {
                        objToolsSchedulerDTONew = new ToolsSchedulerDTO();
                        objToolsSchedulerDTONew.SchedulerName = objDTO.SchedulerName;
                        objToolsSchedulerDTONew.SchedulerType = (int)MaintenanceScheduleType.None;
                        objToolsSchedulerDTONew.Created = DateTimeUtility.DateTimeNow;
                        objToolsSchedulerDTONew.CreatedBy = SessionHelper.UserID;
                        objToolsSchedulerDTONew.Updated = DateTimeUtility.DateTimeNow;
                        objToolsSchedulerDTONew.LastUpdatedBy = SessionHelper.UserID;
                        objToolsSchedulerDTONew.Room = SessionHelper.RoomID;
                        objToolsSchedulerDTONew.IsArchived = false;
                        objToolsSchedulerDTONew.IsDeleted = false;
                        objToolsSchedulerDTONew.GUID = Guid.NewGuid();
                        objToolsSchedulerDTONew.CompanyID = SessionHelper.CompanyID;
                        objToolsSchedulerDTONew.UDF1 = string.Empty;
                        objToolsSchedulerDTONew.UDF2 = string.Empty;
                        objToolsSchedulerDTONew.UDF3 = string.Empty;
                        objToolsSchedulerDTONew.UDF4 = string.Empty;
                        objToolsSchedulerDTONew.UDF5 = string.Empty;


                        objToolsSchedulerDTONew.ScheduleFor = objDTO.ScheduleFor;
                        objToolsSchedulerDTONew.Description = string.Empty;
                        objToolsSchedulerDTONew.SchedulerType = 0;
                        objToolsSchedulerDTONew.TimeBasedFrequency = 0;
                        objToolsSchedulerDTONew.TimeBaseUnit = 0;
                        objToolsSchedulerDTONew.RecurringDays = 0;
                        objToolsSchedulerDTONew.CheckOuts = null;
                        objToolsSchedulerDALNew.Insert(objToolsSchedulerDTONew);
                        objDTO.ToolSchedulerGuid = objToolsSchedulerDTONew.GUID;
                    }
                    else
                    {
                        objDTO.ToolSchedulerGuid = objToolsSchedulerDTONew.GUID;
                    }
                }
            }
            else
            {
                message = ResToolsScheduler.SchedulerName + " is Required";
                status = "fail";
                return Json(new { Message = message, Status = status, DTO = objDTO }, JsonRequestBehavior.AllowGet);
            }
            int TrackngMeasurement = objDTO.TrackngMeasurement;
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(enterPriseDBName);
            DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
            if (!string.IsNullOrWhiteSpace(objDTO.MaintenanceDateStr))
            {
                objDTO.MaintenanceDate = DateTime.ParseExact(objDTO.MaintenanceDateStr, SessionHelper.RoomDateFormat, SessionHelper.RoomCulture);
            }

            ToolsSchedulerDTO objToolsSchedulerDTO = new ToolsSchedulerDTO();


            if (objDTO.ScheduleFor == 1)
                objDTO.AssetGUID = objDTO.AssetToolGUID;
            else if (objDTO.ScheduleFor == 2)
                objDTO.ToolGUID = objDTO.AssetToolGUID;

            if (objDTO.ScheduleFor == 2)
            {
                ToolMasterDAL objToolMasterDAL = this.objToolMasterDAL; //new ToolMasterDAL(enterPriseDBName);
                ToolsSchedulerDAL ToolsSchedulerDAL = new ToolsSchedulerDAL(enterPriseDBName);
                ToolMasterDTO objTool = new ToolMasterDTO();
                ToolsSchedulerDTO objToolsSchedulerDTO1 = new ToolsSchedulerDTO();
                if ((objDTO.ToolGUID ?? Guid.Empty) != Guid.Empty)
                {
                    objTool = objToolMasterDAL.GetToolByGUIDPlain((objDTO.ToolGUID ?? Guid.Empty));
                    objToolsSchedulerDTO1 = ToolsSchedulerDAL.GetToolsSchedulerByGuidPlain(objDTO.ToolSchedulerGuid.GetValueOrDefault(Guid.Empty));
                }

                if (objToolsSchedulerDTO1.SchedulerType == (int)MaintenanceScheduleType.CheckOuts && (objTool.IsGroupOfItems ?? 0) == 1)
                {
                    message = ResToolsSchedulerMapping.errToolMappingNotAllowed;
                    status = "fail";
                    return Json(new { Message = message, Status = status, DTO = objDTO }, JsonRequestBehavior.AllowGet);
                }
            }


            objDTO.ScheduleDate = objDTO.MaintenanceDate;
            objDTO.SchedulerGUID = objDTO.ToolSchedulerGuid;
            objToolsSchedulerDTO = new ToolsSchedulerDAL(enterPriseDBName).GetToolsSchedulerByGuidPlain(objDTO.SchedulerGUID ?? Guid.Empty);
            if (string.IsNullOrWhiteSpace(objDTO.MaintenanceName))
            {

                if (objToolsSchedulerDTO != null)
                {
                    objDTO.MaintenanceName = objToolsSchedulerDTO.SchedulerName;
                }
            }
            ToolsMaintenanceDAL objToolMaintenanceDAL = new ToolsMaintenanceDAL(enterPriseDBName);
            AssetMasterDAL objAssetMasterDAL = new AssetMasterDAL(enterPriseDBName);
            ToolsSchedulerDAL objToolsSchedulerDAL = new ToolsSchedulerDAL(enterPriseDBName);
            List<ToolsSchedulerMappingDTO> lstMappings = objToolsSchedulerDAL.GetScheduleMapping(objDTO.ToolGUID, objDTO.AssetGUID, objDTO.SchedulerGUID, objDTO.SchedulerType);
            if (lstMappings == null || lstMappings.Count() == 0)
            {
                ToolsSchedulerMappingDTO objToolsSchedulerMappingDTO = new ToolsSchedulerMappingDTO();
                objToolsSchedulerMappingDTO.SchedulerFor = objDTO.ScheduleFor;
                objToolsSchedulerMappingDTO.SchedulerType = objDTO.SchedulerType;
                objToolsSchedulerMappingDTO.ToolSchedulerGuid = objDTO.ToolSchedulerGuid;
                objToolsSchedulerMappingDTO.ToolGUID = objDTO.ToolGUID;
                objToolsSchedulerMappingDTO.AssetGUID = objDTO.AssetGUID;
                objToolsSchedulerMappingDTO.Created = DateTimeUtility.DateTimeNow;
                objToolsSchedulerMappingDTO.CreatedBy = SessionHelper.UserID;
                objToolsSchedulerMappingDTO.Updated = DateTimeUtility.DateTimeNow;
                objToolsSchedulerMappingDTO.LastUpdatedBy = SessionHelper.UserID;
                objToolsSchedulerMappingDTO.Room = SessionHelper.RoomID;
                objToolsSchedulerMappingDTO.IsArchived = false;
                objToolsSchedulerMappingDTO.IsDeleted = false;
                objToolsSchedulerMappingDTO.GUID = Guid.NewGuid();
                objToolsSchedulerMappingDTO.CompanyID = SessionHelper.CompanyID;
                objToolsSchedulerMappingDTO.UDF1 = null;
                objToolsSchedulerMappingDTO.UDF2 = null;
                objToolsSchedulerMappingDTO.UDF3 = null;
                objToolsSchedulerMappingDTO.UDF4 = null;
                objToolsSchedulerMappingDTO.UDF5 = null;
                objToolsSchedulerMappingDTO.MaintenanceName = objDTO.MaintenanceName;
                objAssetMasterDAL.InsertToolmapping(objToolsSchedulerMappingDTO);
                lstMappings = objToolsSchedulerDAL.GetScheduleMapping(objDTO.ToolGUID, objDTO.AssetGUID, objDTO.SchedulerGUID, objDTO.SchedulerType);
            }

            if (objDTO.MaintenanceType == MaintenanceType.Past.ToString())
            {
                objDTO.ScheduleFor = lstMappings.First().SchedulerFor;
                objDTO.MappingGUID = lstMappings.First().GUID;
                objDTO.TrackngMeasurement = objToolsSchedulerDTO.SchedulerType;
                objDTO.SchedulerType = (byte)objToolsSchedulerDTO.SchedulerType;
                if (objDTO.ID == 0)
                {
                    if (objDTO.Status.ToLower().Equals(MaintenanceStatus.Open.ToString().ToLower()))
                        objDTO.Status = Convert.ToString(MaintenanceStatus.Close);
                    objDTO = objToolMaintenanceDAL.Insert(objDTO);
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(objDTO.LastMaintenanceDateStr))
                    {
                        objDTO.LastMaintenanceDate = DateTime.ParseExact(objDTO.LastMaintenanceDateStr, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult);
                    }
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.Updated = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objToolMaintenanceDAL.Edit(objDTO);
                }
                objToolMaintenanceDAL.CreateNewMaintenanceAuto(objDTO.AssetGUID, objDTO.ToolGUID, objDTO.ToolSchedulerGuid.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                if (objDTO.SchedulerType == (int)MaintenanceScheduleType.TimeBase && TrackngMeasurement > 0 && TrackingMeasurementValue > 0)
                {
                    ToolsMaintenanceDTO UnScheduledMaintaince = new ToolsMaintenanceDTO();
                    UnScheduledMaintaince.AssetGUID = objDTO.AssetGUID;
                    UnScheduledMaintaince.CompanyID = SessionHelper.CompanyID;
                    UnScheduledMaintaince.Created = DateTimeUtility.DateTimeNow;
                    UnScheduledMaintaince.CreatedBy = SessionHelper.UserID;
                    UnScheduledMaintaince.GUID = Guid.NewGuid();
                    UnScheduledMaintaince.IsArchived = false;
                    UnScheduledMaintaince.IsDeleted = false;
                    UnScheduledMaintaince.LastMaintenanceDate = null;
                    UnScheduledMaintaince.LastMeasurementValue = null;
                    UnScheduledMaintaince.MaintenanceDate = objDTO.MaintenanceDate;
                    UnScheduledMaintaince.LastUpdatedBy = SessionHelper.UserID;
                    UnScheduledMaintaince.MaintenanceName = "Tracking Entry";
                    UnScheduledMaintaince.MaintenanceType = MaintenanceType.Past.ToString();
                    UnScheduledMaintaince.MappingGUID = null;
                    UnScheduledMaintaince.RequisitionGUID = null;
                    UnScheduledMaintaince.Room = SessionHelper.RoomID;
                    UnScheduledMaintaince.ScheduleDate = objDTO.MaintenanceDate;
                    UnScheduledMaintaince.ScheduleFor = lstMappings.First().SchedulerFor;
                    UnScheduledMaintaince.SchedulerGUID = null;
                    UnScheduledMaintaince.SchedulerType = (byte)TrackngMeasurement;
                    UnScheduledMaintaince.Status = MaintenanceStatus.Close.ToString();
                    UnScheduledMaintaince.ToolGUID = objDTO.ToolGUID;
                    UnScheduledMaintaince.TrackingMeasurementValue = objDTO.TrackingMeasurementValue;
                    UnScheduledMaintaince.TrackngMeasurement = TrackngMeasurement;
                    UnScheduledMaintaince.UDF1 = null;
                    UnScheduledMaintaince.UDF2 = null;
                    UnScheduledMaintaince.UDF3 = null;
                    UnScheduledMaintaince.UDF4 = null;
                    UnScheduledMaintaince.UDF5 = null;
                    UnScheduledMaintaince.Updated = DateTimeUtility.DateTimeNow;
                    UnScheduledMaintaince.WorkorderGUID = null;
                    UnScheduledMaintaince = objToolMaintenanceDAL.Insert(UnScheduledMaintaince);

                    List<ToolsSchedulerMappingDTO> lstAllSchedules = new List<ToolsSchedulerMappingDTO>();
                    lstAllSchedules = objToolsSchedulerDAL.GetAllSchedulesforToolAsset(objDTO.ToolGUID, objDTO.AssetGUID, objDTO.TrackngMeasurement);
                    foreach (var item in lstAllSchedules)
                    {
                        objToolMaintenanceDAL.CreateNewMaintenanceAuto(item.AssetGUID, item.ToolGUID, item.ToolSchedulerGuid.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                    }
                }
            }
            else if (objDTO.MaintenanceType == MaintenanceType.UnScheduled.ToString())
            {
                List<ToolsMaintenanceDTO> lstmnts = objToolMaintenanceDAL.GetMntsUnClose(lstMappings.First());
                if (lstmnts.Any(t => t.Status == MaintenanceStatus.Start.ToString()))
                {
                    status = "exists";
                }
                else if (lstmnts.Any(t => t.Status == MaintenanceStatus.Open.ToString()))
                {
                    var guids = string.Join(",", lstmnts.Select(t => t.GUID).ToArray());
                    objToolMaintenanceDAL.DeleteToolsMaintenanceByGuids(guids, SessionHelper.UserID);
                    //objToolMaintenanceDAL.DeleteOpenmaintanance(lstMappings.First());
                }
                if (status != "exists")
                {
                    ToolsMaintenanceDTO UnScheduledMaintaince = new ToolsMaintenanceDTO();
                    UnScheduledMaintaince.AssetGUID = objDTO.AssetGUID;
                    UnScheduledMaintaince.CompanyID = SessionHelper.CompanyID;
                    UnScheduledMaintaince.Created = DateTimeUtility.DateTimeNow;
                    UnScheduledMaintaince.CreatedBy = SessionHelper.UserID;
                    UnScheduledMaintaince.GUID = Guid.NewGuid();
                    UnScheduledMaintaince.IsArchived = false;
                    UnScheduledMaintaince.IsDeleted = false;
                    UnScheduledMaintaince.LastMaintenanceDate = null;
                    UnScheduledMaintaince.LastMeasurementValue = null;
                    UnScheduledMaintaince.MaintenanceDate = datetimetoConsider.Date;
                    UnScheduledMaintaince.LastUpdatedBy = SessionHelper.UserID;
                    UnScheduledMaintaince.MaintenanceName = objDTO.MaintenanceName;
                    UnScheduledMaintaince.MaintenanceType = MaintenanceType.UnScheduled.ToString();
                    UnScheduledMaintaince.MappingGUID = lstMappings.First().GUID;
                    UnScheduledMaintaince.RequisitionGUID = null;
                    UnScheduledMaintaince.Room = SessionHelper.RoomID;
                    UnScheduledMaintaince.ScheduleDate = datetimetoConsider.Date;
                    UnScheduledMaintaince.ScheduleFor = lstMappings.First().SchedulerFor;
                    UnScheduledMaintaince.SchedulerGUID = objDTO.SchedulerGUID;
                    UnScheduledMaintaince.SchedulerType = (byte)objToolsSchedulerDTO.SchedulerType;
                    UnScheduledMaintaince.Status = MaintenanceStatus.Start.ToString();
                    UnScheduledMaintaince.ToolGUID = objDTO.ToolGUID;
                    UnScheduledMaintaince.TrackingMeasurementValue = objDTO.TrackingMeasurementValue;
                    UnScheduledMaintaince.TrackngMeasurement = objToolsSchedulerDTO.SchedulerType;
                    UnScheduledMaintaince.UDF1 = null;
                    UnScheduledMaintaince.UDF2 = null;
                    UnScheduledMaintaince.UDF3 = null;
                    UnScheduledMaintaince.UDF4 = null;
                    UnScheduledMaintaince.UDF5 = null;
                    UnScheduledMaintaince.Updated = DateTimeUtility.DateTimeNow;
                    UnScheduledMaintaince.WorkorderGUID = null;
                    objDTO = objToolMaintenanceDAL.Insert(UnScheduledMaintaince);
                    if (objDTO.SchedulerType == (int)MaintenanceScheduleType.TimeBase && TrackngMeasurement > 0 && TrackingMeasurementValue > 0)
                    {
                        UnScheduledMaintaince = new ToolsMaintenanceDTO();
                        UnScheduledMaintaince.AssetGUID = objDTO.AssetGUID;
                        UnScheduledMaintaince.CompanyID = SessionHelper.CompanyID;
                        UnScheduledMaintaince.Created = DateTimeUtility.DateTimeNow;
                        UnScheduledMaintaince.CreatedBy = SessionHelper.UserID;
                        UnScheduledMaintaince.GUID = Guid.NewGuid();
                        UnScheduledMaintaince.IsArchived = false;
                        UnScheduledMaintaince.IsDeleted = false;
                        UnScheduledMaintaince.LastMaintenanceDate = null;
                        UnScheduledMaintaince.LastMeasurementValue = null;
                        UnScheduledMaintaince.MaintenanceDate = objDTO.MaintenanceDate;
                        UnScheduledMaintaince.LastUpdatedBy = SessionHelper.UserID;
                        UnScheduledMaintaince.MaintenanceName = "Tracking Entry";
                        UnScheduledMaintaince.MaintenanceType = MaintenanceType.Past.ToString();
                        UnScheduledMaintaince.MappingGUID = null;
                        UnScheduledMaintaince.RequisitionGUID = null;
                        UnScheduledMaintaince.Room = SessionHelper.RoomID;
                        UnScheduledMaintaince.ScheduleDate = objDTO.MaintenanceDate;
                        UnScheduledMaintaince.ScheduleFor = lstMappings.First().SchedulerFor;
                        UnScheduledMaintaince.SchedulerGUID = null;
                        UnScheduledMaintaince.SchedulerType = (byte)TrackngMeasurement;
                        UnScheduledMaintaince.Status = MaintenanceStatus.Close.ToString();
                        UnScheduledMaintaince.ToolGUID = objDTO.ToolGUID;
                        UnScheduledMaintaince.TrackingMeasurementValue = objDTO.TrackingMeasurementValue;
                        UnScheduledMaintaince.TrackngMeasurement = TrackngMeasurement;
                        UnScheduledMaintaince.UDF1 = null;
                        UnScheduledMaintaince.UDF2 = null;
                        UnScheduledMaintaince.UDF3 = null;
                        UnScheduledMaintaince.UDF4 = null;
                        UnScheduledMaintaince.UDF5 = null;
                        UnScheduledMaintaince.Updated = DateTimeUtility.DateTimeNow;
                        UnScheduledMaintaince.WorkorderGUID = null;
                        UnScheduledMaintaince = objToolMaintenanceDAL.Insert(UnScheduledMaintaince);

                        List<ToolsSchedulerMappingDTO> lstAllSchedules = new List<ToolsSchedulerMappingDTO>();
                        lstAllSchedules = objToolsSchedulerDAL.GetAllSchedulesforToolAsset(objDTO.ToolGUID, objDTO.AssetGUID, objDTO.TrackngMeasurement);
                        foreach (var item in lstAllSchedules)
                        {
                            objToolMaintenanceDAL.CreateNewMaintenanceAuto(item.AssetGUID, item.ToolGUID, item.ToolSchedulerGuid.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                        }
                    }
                    //CreateAssetToolRequisition(UnScheduledMaintaince.GUID, (float)Convert.ToDouble(objDTO.TrackingMeasurementValue));
                }

            }
            message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
            return Json(new { Message = message, Status = status, DTO = objDTO }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteToolMaintenanceRecords(string ids)
        {
            try
            {
                string response = string.Empty;
                CommonDAL objCommonDAL = new CommonDAL(enterPriseDBName);
                response = objCommonDAL.DeleteModulewise(ids, "Maintenance", false, SessionHelper.UserID, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);
                //return Json(new { Message = "ok", Status = "ok" }, JsonRequestBehavior.AllowGet);
                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Message = "fail", Status = "fail" }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult UnDeleteToolMaintenanceRecords(string ids)
        {
            try
            {
                string response = string.Empty;
                CommonDAL objCommonDAL = new CommonDAL(enterPriseDBName);
                ModuleUnDeleteDTO objModuleUnDeleteDTO = new ModuleUnDeleteDTO();
                objModuleUnDeleteDTO = objCommonDAL.UnDeleteModulewise(ids, "Maintenance", false, SessionHelper.UserID, true,SessionHelper.EnterPriceID,SessionHelper.CompanyID,SessionHelper.RoomID);
                response = objModuleUnDeleteDTO.CommonMessage;
                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Message = "fail", Status = "fail" }, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion

        #region [Asset/Tool Requisition]

        [HttpPost]
        public JsonResult CreateAssetToolRequisition(Guid MntnsGUID, string Reading, string ReadingType)
        {
            string message = ResMessage.SaveMessage;// "Tool is required.";
            string status = "ok";
            bool HasPullRights = SessionHelper.GetModulePermission(SessionHelper.ModuleList.PullMaster, SessionHelper.PermissionType.Insert);
            bool HasWORights = SessionHelper.GetModulePermission(SessionHelper.ModuleList.WorkOrders, SessionHelper.PermissionType.Insert);
            bool HasReqRights = SessionHelper.GetModulePermission(SessionHelper.ModuleList.Requisitions, SessionHelper.PermissionType.Insert);
            float floatval = 0;
            float.TryParse(Reading, out floatval);
            try
            {
                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(enterPriseDBName);
                DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                AssetMasterDAL objAssetMasterDAL = new AssetMasterDAL(enterPriseDBName);
                ToolsMaintenanceDTO objToolsMaintenanceDTO = objAssetMasterDAL.GetMaintenanceByGUID(MntnsGUID);
                long SessionUserId = SessionHelper.UserID;
                if (objToolsMaintenanceDTO.TrackngMeasurement == 1)
                {
                    if (!string.IsNullOrWhiteSpace(Reading) && !string.IsNullOrWhiteSpace(ReadingType))
                    {

                        int tempReadingType = 0;
                        int.TryParse(ReadingType, out tempReadingType);
                        if (tempReadingType == 2 || tempReadingType == 3)
                        {
                            ToolsMaintenanceDTO objDTOreading = new ToolsMaintenanceDTO();
                            objDTOreading.TrackngMeasurement = tempReadingType;
                            objDTOreading.TrackingMeasurementValue = Convert.ToString(floatval);
                            objDTOreading.AssetGUID = objToolsMaintenanceDTO.AssetGUID;
                            objDTOreading.ToolGUID = objToolsMaintenanceDTO.ToolGUID;
                            objDTOreading.MaintenanceDate = DateTime.UtcNow;
                            objDTOreading.Room = SessionHelper.RoomID;
                            objDTOreading.CompanyID = SessionHelper.CompanyID;
                            objDTOreading.CreatedBy = SessionHelper.UserID;
                            objDTOreading.LastUpdatedBy = SessionHelper.UserID;
                            objDTOreading.GUID = Guid.NewGuid();
                            objDTOreading.Created = DateTime.UtcNow;
                            objDTOreading.Updated = DateTime.UtcNow;
                            objDTOreading.IsDeleted = false;
                            objDTOreading.IsArchived = false;
                            objDTOreading.EntryDate = DateTime.UtcNow.ToString(SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult);
                            OdometerSave(objDTOreading);
                        }

                    }
                }
                ToolsSchedulerDTO objToolsScheduler = objAssetMasterDAL.GetToolSchedulebyGUID(objToolsMaintenanceDTO.ToolSchedulerGuid ?? Guid.Empty);
                RequisitionMasterDAL objRequisitionMasterDAL = new RequisitionMasterDAL(enterPriseDBName);
                WorkOrderDAL objWODAL = new WorkOrderDAL(enterPriseDBName);

                if (HasPullRights && HasWORights)
                {
                    AutoOrderNumberGenerate objAutoNumber = null;
                    AutoSequenceDAL objAutoSeqDAL = null;
                    objAutoSeqDAL = new AutoSequenceDAL(enterPriseDBName);
                    //string nextWONo = new AutoSequenceDAL(enterPriseDBName).GetNextAutoNumberByModule("NextWorkOrderNo", SessionHelper.RoomID, SessionHelper.CompanyID);
                    objAutoNumber = objAutoSeqDAL.GetNextWorkOrderNumber(SessionHelper.RoomID, SessionHelper.CompanyID,SessionHelper.EnterPriceID);

                    string nextWONo = objAutoNumber.OrderNumber;
                    if (nextWONo != null && (!string.IsNullOrEmpty(nextWONo)))
                    {
                        nextWONo = nextWONo.Length > 22 ? nextWONo.Substring(0, 22) : nextWONo;
                    }
                    string _ReleaseNumber = objWODAL.GenerateAndGetReleaseNumber(nextWONo, 0, SessionHelper.RoomID, SessionHelper.CompanyID);
                    WorkOrderDTO objWODTO = new WorkOrderDTO()
                    {
                        Created = DateTimeUtility.DateTimeNow,
                        Updated = DateTimeUtility.DateTimeNow,
                        CreatedBy = SessionHelper.UserID,
                        CreatedByName = SessionHelper.UserName,
                        LastUpdatedBy = SessionHelper.UserID,
                        Room = SessionHelper.RoomID,
                        CompanyID = SessionHelper.CompanyID,
                        RoomName = SessionHelper.RoomName,
                        UpdatedByName = SessionHelper.UserName,
                        IsArchived = false,
                        IsDeleted = false,
                        //WOName = "#W" + nextWONo,
                        WOName = nextWONo,
                        WOStatus = "Open",
                        WOType = "Maint",
                        AssetGUID = objToolsMaintenanceDTO.AssetGUID,
                        ToolGUID = objToolsMaintenanceDTO.ToolGUID,
                        Odometer_OperationHours = floatval,
                        WhatWhereAction = "Work Order Maintainance",
                        ReleaseNumber = _ReleaseNumber
                    };
                    Guid WorkOrderGUID = objWODAL.Insert(objWODTO);
                    objToolsMaintenanceDTO.LastUpdatedBy = SessionHelper.UserID;
                    objToolsMaintenanceDTO.MaintenanceDate = datetimetoConsider.Date;
                    objToolsMaintenanceDTO.RequisitionGUID = null;
                    objToolsMaintenanceDTO.Status = MaintenanceStatus.Start.ToString();
                    objToolsMaintenanceDTO.TrackingMeasurementValue = Convert.ToString(Reading);
                    objToolsMaintenanceDTO.Updated = DateTime.UtcNow;
                    objToolsMaintenanceDTO.WorkorderGUID = WorkOrderGUID;
                    objToolsMaintenanceDTO = objAssetMasterDAL.UpdateMaintenanceParams(objToolsMaintenanceDTO);
                    status = "wo";
                    return Json(new { Message = message, Status = status, objDTO = objToolsMaintenanceDTO });
                }
                else if (HasReqRights && HasWORights)
                {
                    string nextReqNo = new AutoSequenceDAL(enterPriseDBName).GetNextAutoNumberByModule("NextRequisitionNo", SessionHelper.RoomID, SessionHelper.CompanyID);

                    AutoOrderNumberGenerate objAutoNumber = null;
                    AutoSequenceDAL objAutoSeqDAL = null;
                    objAutoSeqDAL = new AutoSequenceDAL(enterPriseDBName);
                    //string nextWONo = new AutoSequenceDAL(enterPriseDBName).GetNextAutoNumberByModule("NextWorkOrderNo", SessionHelper.RoomID, SessionHelper.CompanyID);
                    objAutoNumber = objAutoSeqDAL.GetNextWorkOrderNumber(SessionHelper.RoomID, SessionHelper.CompanyID,SessionHelper.EnterPriceID);

                    string nextWONo = objAutoNumber.OrderNumber;
                    if (nextWONo != null && (!string.IsNullOrEmpty(nextWONo)))
                    {
                        nextWONo = nextWONo.Length > 22 ? nextWONo.Substring(0, 22) : nextWONo;
                    }

                    string _ReleaseNumber = objWODAL.GenerateAndGetReleaseNumber(nextWONo, 0, SessionHelper.RoomID, SessionHelper.CompanyID);
                    WorkOrderDTO objWODTO = new WorkOrderDTO()
                    {
                        Created = DateTimeUtility.DateTimeNow,
                        Updated = DateTimeUtility.DateTimeNow,
                        CreatedBy = SessionHelper.UserID,
                        CreatedByName = SessionHelper.UserName,
                        LastUpdatedBy = SessionHelper.UserID,
                        Room = SessionHelper.RoomID,
                        CompanyID = SessionHelper.CompanyID,
                        RoomName = SessionHelper.RoomName,
                        UpdatedByName = SessionHelper.UserName,
                        IsArchived = false,
                        IsDeleted = false,
                        //WOName = "#W" + nextWONo,
                        WOName = nextWONo,
                        WOStatus = "Open",
                        WOType = "Maint",
                        AssetGUID = objToolsMaintenanceDTO.AssetGUID,
                        ToolGUID = objToolsMaintenanceDTO.ToolGUID,
                        Odometer_OperationHours = floatval,
                        WhatWhereAction = "Work Order Maintainance",
                        ReleaseNumber = _ReleaseNumber
                    };
                    Guid WorkOrderGUID = objWODAL.Insert(objWODTO);
                    RequisitionMasterDTO objRequisitionMasterDTO = new RequisitionMasterDTO()
                    {
                        Action = "insert",
                        AddedFrom = "web",
                        AppendedBarcodeString = string.Empty,
                        ApprovedQuantity = 0,
                        BillingAccount = string.Empty,
                        BillingAccountID = null,
                        CompanyID = SessionHelper.CompanyID,
                        Created = DateTime.UtcNow,
                        CreatedBy = SessionHelper.UserID,
                        CreatedByName = SessionHelper.UserName,
                        CustomerGUID = Guid.Empty,
                        Description = string.Empty,
                        Customer = string.Empty,
                        CreatedDate = string.Empty,
                        CustomerID = null,
                        EditedFrom = "web",
                        GUID = Guid.NewGuid(),
                        ID = 0,
                        IsArchived = false,
                        IsDeleted = false,
                        LastUpdatedBy = SessionHelper.UserID,
                        NumberofItemsrequisitioned = 0,
                        ReceivedOn = DateTime.UtcNow,
                        ProjectSpendGUID = null,
                        ReceivedOnWeb = DateTime.UtcNow,
                        RequiredDate = datetimetoConsider.Date,
                        RequisitionNumber = nextReqNo,
                        RequisitionStatus = "Unsubmitted",
                        RequisitionType = "tool",
                        Room = SessionHelper.RoomID,
                        WorkorderGUID = null,
                        Updated = DateTime.UtcNow,
                        WhatWhereAction = "Requisition Maintainance",
                    };
                    //var releaseNumber = objRequisitionMasterDAL.GetRequisitionReleaseNumber(objRequisitionMasterDTO.ID, objRequisitionMasterDTO.RequisitionNumber, SessionHelper.CompanyID, SessionHelper.RoomID);
                    //objRequisitionMasterDTO.ReleaseNumber = Convert.ToString(releaseNumber);

                    Guid RequisitionGUID = objRequisitionMasterDAL.Insert(objRequisitionMasterDTO).GUID;
                    RequisitionDetailsDAL objRequisitionDetailsDAL = new RequisitionDetailsDAL(enterPriseDBName);
                    if (objToolsScheduler != null && objToolsScheduler.lstItems != null && objToolsScheduler.lstItems.Count() > 0)
                    {
                        foreach (var item in objToolsScheduler.lstItems)
                        {
                            RequisitionDetailsDTO objRequisitionDetailsDTO = new RequisitionDetailsDTO()
                            {
                                Action = "insert",
                                AddedFrom = "web",
                                CompanyID = SessionHelper.CompanyID,
                                EditedFrom = "web",
                                GUID = Guid.NewGuid(),
                                ID = 0,
                                QuantityApproved = 0,
                                QuantityPulled = 0,
                                QuantityRequisitioned = item.Quantity,
                                Room = SessionHelper.RoomID,
                                RequisitionGUID = RequisitionGUID,
                                ItemGUID = item.ItemGUID,
                                ItemCost = 0
                            };
                            objRequisitionDetailsDAL.Insert(objRequisitionDetailsDTO, SessionUserId);
                        }
                        if (RequisitionGUID != Guid.Empty)
                        {
                            objRequisitionDetailsDAL.UpdateRequisitionTotalCost(RequisitionGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                        }
                    }

                    objToolsMaintenanceDTO.LastUpdatedBy = SessionHelper.UserID;
                    objToolsMaintenanceDTO.MaintenanceDate = datetimetoConsider.Date;
                    objToolsMaintenanceDTO.RequisitionGUID = RequisitionGUID;
                    objToolsMaintenanceDTO.Status = MaintenanceStatus.Start.ToString();
                    objToolsMaintenanceDTO.TrackingMeasurementValue = Convert.ToString(Reading);
                    objToolsMaintenanceDTO.Updated = DateTime.UtcNow;
                    objToolsMaintenanceDTO.WorkorderGUID = WorkOrderGUID;
                    objToolsMaintenanceDTO = objAssetMasterDAL.UpdateMaintenanceParams(objToolsMaintenanceDTO);
                    status = "Reqwithwo";
                    return Json(new { Message = message, Status = status, objDTO = objToolsMaintenanceDTO });
                }
                else
                {
                    status = "rightslimits";
                    return Json(new { Message = message, Status = status, objDTO = objToolsMaintenanceDTO });
                }




            }
            catch (Exception)
            {
                message = ResMessage.SaveMessage;// "Tool is required.";
                status = "error";
                return Json(new
                {
                    Message = message,
                    Status = status
                });
            }
        }

        #endregion


        [NonAction]
        private List<CommonDTO> GetWOType()
        {
            List<CommonDTO> ItemType = new List<CommonDTO>();
            ItemType.Add(new CommonDTO() { Text = ResAssetMaster.WOTypeWorkorder }); 
            ItemType.Add(new CommonDTO() { Text = ResAssetMaster.WOTypeRequisition });
            ItemType.Add(new CommonDTO() { Text = ResAssetMaster.WOTypeToolService });
            ItemType.Add(new CommonDTO() { Text = ResAssetMaster.WOTypeAssetService });
            return ItemType;
        }
        [NonAction]
        private List<CommonDTO> GetWOStaus()
        {
            List<CommonDTO> ItemType = new List<CommonDTO>();
            ItemType.Add(new CommonDTO() { Text = ResWorkOrder.Open, Value = "Open" });
            ItemType.Add(new CommonDTO() { Text = ResWorkOrder.Close, Value = "Close" });
            return ItemType;
        }
        public ActionResult MntsWOEdit(string WorkOrderGUID)
        {
            Guid mntsGUID = Guid.Empty;
            if (Request["mntsGUID"] != null)
            {
                Guid.TryParse(Request["mntsGUID"].ToString(), out mntsGUID);
            }
            ToolsMaintenanceDTO objToolsMaintenanceDTO = new ToolsMaintenanceDAL(enterPriseDBName).GetToolsMaintenanceByGuidPlain(mntsGUID);
            if (string.IsNullOrWhiteSpace(WorkOrderGUID))
            {
                WorkOrderGUID = objToolsMaintenanceDTO.WorkorderGUID.HasValue ? objToolsMaintenanceDTO.WorkorderGUID.Value.ToString() : string.Empty;
            }
            ViewBag.MntsDTO = objToolsMaintenanceDTO;
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            bool IsHitory = false;
            if (Request["IsHistory"] != null && !string.IsNullOrEmpty(Request["IsHistory"].ToString()))
                IsHitory = bool.Parse(Request["IsHistory"].ToString());



            WorkOrderDAL obj = new WorkOrderDAL(enterPriseDBName);
            //RequisitionDetailsDAL objReqDDAL = new RequisitionDetailsDAL(enterPriseDBName);

            WorkOrderDTO objDTO = obj.GetWorkOrdersByGUIDFullJoins(Guid.Parse(WorkOrderGUID));
            objDTO.MaintenanceGUID = mntsGUID;


            if (IsHitory)
                objDTO.IsHistory = true;

            if (IsDeleted || IsArchived || objDTO.WOStatus == "Close")
            {
                ViewBag.ViewOnly = true;
            }

            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("WorkOrder");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;


            CustomerMasterDAL objCustApi = new CustomerMasterDAL(enterPriseDBName);
            ViewBag.CustomerBAG = objCustApi.GetCustomersByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(x => x.Customer.Trim()).ToList();
            TechnicialMasterDAL objTechMasterApi = new TechnicialMasterDAL(enterPriseDBName);
            List<TechnicianMasterDTO> technicianlist = objTechMasterApi.GetTechnicianByRoomIDPlain(SessionHelper.RoomID, SessionHelper.CompanyID);
            technicianlist.ForEach(t =>
            {
                if (!string.IsNullOrEmpty(t.Technician))
                {
                    t.Technician = Convert.ToString(t.TechnicianCode + " --- " + t.Technician);
                }
                else
                {
                    t.Technician = Convert.ToString(t.TechnicianCode);
                }
            });

            ViewBag.TechnicianBAG = technicianlist;

            //GXPRConsignedJobController objGXPRMasterApi = new GXPRConsignedJobController();
            GXPRConsignedJobMasterDAL objGXPRMasterApi = new GXPRConsignedJobMasterDAL(enterPriseDBName);
            ViewBag.GXPRConsigmentBAG = objGXPRMasterApi.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            AssetMasterDAL objAssetApi = new AssetMasterDAL(enterPriseDBName);
            ViewBag.AssetBAG = objAssetApi.GetAllAssetsByRoom(SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted);

            ToolMasterDAL objToolApi = this.objToolMasterDAL; //new ToolMasterDAL(enterPriseDBName);
            ViewBag.ToolBAG = objToolApi.GetToolByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID);

            JobTypeMasterDAL objJobTypeApi = new JobTypeMasterDAL(enterPriseDBName);
            ViewBag.JobTypeBAG = objJobTypeApi.GetJobTypeByRoomNormal(SessionHelper.RoomID, SessionHelper.CompanyID);

            ViewBag.WOStatusBag = GetWOStaus();
            ViewBag.WOTypeBag = GetWOType();

            return PartialView("_CreateWorkOrder", objDTO);
        }

        public ActionResult MntsReqEdit(string RequisitionGUID)
        {
            bool IsHitory = false;
            bool IsArchived = false;
            bool IsDeleted = false;
            ToolsMaintenanceDTO objToolsMaintenanceDTO = new ToolsMaintenanceDTO();
            Guid mntsGUID = Guid.Empty;
            if (Request["mntsGUID"] != null)
            {
                if (Guid.TryParse(Request["mntsGUID"].ToString(), out mntsGUID))
                {
                    objToolsMaintenanceDTO = new ToolsMaintenanceDAL(enterPriseDBName).GetToolsMaintenanceByGuidPlain(mntsGUID);
                }
            }
            ViewBag.objToolsMaintenanceDTO = objToolsMaintenanceDTO;
            if (Request["IsArchived"] != null && !string.IsNullOrEmpty(Request["IsArchived"].ToString()))
                IsArchived = bool.Parse(Request["IsArchived"].ToString());
            if (Request["IsDeleted"] != null && !string.IsNullOrEmpty(Request["IsDeleted"].ToString()))
                IsDeleted = bool.Parse(Request["IsDeleted"].ToString());

            if (Request["IsHistory"] != null && !string.IsNullOrEmpty(Request["IsHistory"].ToString()))
                IsHitory = bool.Parse(Request["IsHistory"].ToString());

            RequisitionMasterDAL obj = new RequisitionMasterDAL(enterPriseDBName);
            RequisitionDetailsDAL objReqDDAL = new RequisitionDetailsDAL(enterPriseDBName);

            RequisitionMasterDTO objDTO = obj.GetRequisitionByGUIDFull(Guid.Parse(RequisitionGUID));

            if (objDTO.WorkorderGUID != null && objDTO.WorkorderGUID != Guid.Empty)
            {
                if (objDTO.WorkorderGUID.HasValue)
                {
                    objDTO.WorkorderName = new WorkOrderDAL(enterPriseDBName).GetWorkOrderByGUIDPlain(objDTO.WorkorderGUID.Value).WOName;
                }
            }

            if (IsHitory)
            {
                objDTO.IsRecordEditable = false;
                objDTO.IsHistory = true;
            }
            else
                objDTO.IsRecordEditable = IsRecordEditable(objDTO);

            if (objDTO != null)
            {
                Double PullSum = objReqDDAL.GetReqLinesByReqGUIDPlain(objDTO.GUID, 0, SessionHelper.RoomID, SessionHelper.CompanyID).Select(x => x.QuantityPulled.GetValueOrDefault(0)).Sum();//.Where(x => x.RequisitionGUID == objDTO.GUID)
                ViewBag.PulledQuantity = PullSum;
            }

            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("RequisitionMaster");

            //string nextWONo = new AutoSequenceDAL(enterPriseDBName).GetLastGeneratedROOMID("NextWorkOrderNo", SessionHelper.RoomID, SessionHelper.CompanyID).ToString();
            //string nextWONo = new AutoSequenceDAL(enterPriseDBName).GetNextAutoNumberByModule("NextWorkOrderNo", SessionHelper.RoomID, SessionHelper.CompanyID);
            AutoOrderNumberGenerate objAutoNumber = null;
            AutoSequenceDAL objAutoSeqDAL = null;
            objAutoSeqDAL = new AutoSequenceDAL(enterPriseDBName);
            objAutoNumber = objAutoSeqDAL.GetNextWorkOrderNumber(SessionHelper.RoomID, SessionHelper.CompanyID,SessionHelper.EnterPriceID);

            string nextWONo = objAutoNumber.OrderNumber;
            if (nextWONo != null && (!string.IsNullOrEmpty(nextWONo)))
            {
                nextWONo = nextWONo.Length > 22 ? nextWONo.Substring(0, 22) : nextWONo;
            }

            //ViewBag.WOName = "#R" + nextWONo;
            ViewBag.WOName = nextWONo;
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;

            //BinMasterDAL objBinMasterApi = new BinMasterDAL(enterPriseDBName);
            //ViewBag.BinMaster = objBinMasterApi.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
            //ViewBag.BinMaster = objBinMasterApi.GetBinMasterByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            CustomerMasterDAL objCustApi = new CustomerMasterDAL(enterPriseDBName);
            ViewBag.Customer = objCustApi.GetCustomersByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(x => x.Customer.Trim()).ToList();

            CommonDAL objCommon = new CommonDAL(enterPriseDBName);
            ViewBag.SupplierAccountBag = objCommon.GetDDData("SupplierAccountDetails", "AccountName", "SupplierID = (Select DefaultSupplierID from Room where ID = " + SessionHelper.RoomID.ToString() + ") AND ", SessionHelper.CompanyID, SessionHelper.RoomID);


            #region "Project Spend DropDownList"

            ProjectMasterDAL objProjectApi = new ProjectMasterDAL(enterPriseDBName);
            List<ProjectMasterDTO> lstProject = new List<ProjectMasterDTO>();
            var projectTrackAllUsageAgainstThis = objProjectApi.GetDefaultProjectSpendRecord(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

            if (projectTrackAllUsageAgainstThis != null && projectTrackAllUsageAgainstThis.GUID != Guid.Empty)
            {
                lstProject.Add(projectTrackAllUsageAgainstThis);

                if (objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {
                    var currentProject = objProjectApi.GetRecord(objDTO.ProjectSpendGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                    lstProject.Add(currentProject);
                }
                ViewBag.ProjectSpent = lstProject;
            }
            else
            {
                lstProject = objProjectApi.GetAllProjectMasterByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
                if (lstProject != null && lstProject.Any())
                {
                    ViewBag.IsClosedFalse = 1;
                }
                ViewBag.ProjectSpent = lstProject;
            }

            #endregion

            ViewBag.RequisitionStatusBag = GetRequisitionStatus(objDTO.RequisitionStatus);
            ViewBag.RequisitionTypeBag = GetRequisitionType();
            objDTO.rdoApprovelChoice = 3;
            objDTO.RequiredDateStr = objDTO.RequiredDate.HasValue ? objDTO.RequiredDate.Value.ToString(SessionHelper.RoomDateFormat) : string.Empty;
            //List<MaterialStagingDTO> lstStaging = new MaterialStagingDAL(enterPriseDBName).GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).OrderBy(t => t.StagingName).ToList();
            List<MaterialStagingDTO> lstStaging = new MaterialStagingDAL(enterPriseDBName).GetMaterialStaging(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, string.Empty, null).OrderBy(t => t.StagingName).ToList();
            lstStaging.Insert(0, new MaterialStagingDTO());
            ViewBag.StagingList = lstStaging;
            return PartialView("_CreateRequisition", objDTO);
        }
        [NonAction]
        private List<CommonDTO> GetRequisitionStatus(string RequisitionStatus)
        {
            List<CommonDTO> ItemType = new List<CommonDTO>();

            if (RequisitionStatus == "Unsubmitted")
            {
                ItemType.Add(new CommonDTO() { Text = "Unsubmitted" });
                ItemType.Add(new CommonDTO() { Text = "Submitted" });
                if (IsApprove)
                {
                    ItemType.Add(new CommonDTO() { Text = "Approved" });
                    ItemType.Add(new CommonDTO() { Text = "Closed" });
                }
            }
            else if (RequisitionStatus == "Submitted")
            {
                ItemType.Add(new CommonDTO() { Text = "Submitted" });
                if (IsApprove)
                {
                    ItemType.Add(new CommonDTO() { Text = "Approved" });
                    ItemType.Add(new CommonDTO() { Text = "Closed" });
                }

            }
            else if (RequisitionStatus == "Approved")
            {
                ItemType.Add(new CommonDTO() { Text = "Approved" });
                ItemType.Add(new CommonDTO() { Text = "Closed" });
            }
            else if (RequisitionStatus == "Closed")
            {
                ItemType.Add(new CommonDTO() { Text = "Closed" });
            }

            return ItemType;
        }

        [NonAction]
        private List<CommonDTO> GetRequisitionType()
        {
            List<CommonDTO> ItemType = new List<CommonDTO>();
            ItemType.Add(new CommonDTO() { Text = ResAssetMaster.WOTypeRequisition });
            ItemType.Add(new CommonDTO() { Text = ResAssetMaster.WOTypeWorkorder });
            ItemType.Add(new CommonDTO() { Text = ResAssetMaster.WOTypeToolService });
            ItemType.Add(new CommonDTO() { Text = ResAssetMaster.WOTypeAssetService });
            return ItemType;
        }
        public bool IsRecordEditable(RequisitionMasterDTO objDTO)
        {
            bool isEditable = true;

            if (objDTO.IsArchived.GetValueOrDefault(false) || objDTO.IsDeleted.GetValueOrDefault(false))
            {
                isEditable = false;
                return isEditable;
            }

            if (!(IsInsert || IsUpdate || IsDelete || IsApprove))
            {
                isEditable = false;
                return isEditable;
            }

            if (objDTO.ID <= 0 && !IsInsert)
            {
                isEditable = false;
            }
            else if (IsUpdate || IsApprove || Session["IsInsert"].ToString() == "True" || IsInsert)
            {
                if (objDTO.RequisitionStatus == "Unsubmitted")
                {
                    if (Convert.ToString(Session["IsInsert"]) == "")
                    {
                        if (IsUpdate) // Update only 
                        {
                            isEditable = true;
                        }
                        else
                        {
                            if (objDTO.ID <= 0 && IsInsert) // Insert only  first time
                                isEditable = true;
                            else if (objDTO.ID > 0 && IsInsert)// Edit mode with View only 
                                isEditable = false;
                            else if (objDTO.ID > 0 && !IsInsert)
                            {
                                isEditable = false;
                                if (IsApprove)
                                    objDTO.IsOnlyStatusUpdate = true;
                            }
                        }
                    }
                    else
                    {


                        isEditable = true;
                    }
                }
                else if (objDTO.RequisitionStatus == "Submitted")
                {
                    if (IsUpdate && IsApprove)
                        isEditable = true;
                    else if (!IsUpdate && IsApprove)
                    {
                        isEditable = false;
                        objDTO.IsOnlyStatusUpdate = true;
                    }
                    else if (!IsUpdate && !IsApprove)
                        isEditable = false;
                }
                else if (objDTO.RequisitionStatus == "Approved")
                {
                    isEditable = true;
                    //if (IsApprove)
                    //    objDTO.IsOnlyStatusUpdate = true;
                }
                else if (objDTO.RequisitionStatus == "Closed")
                {
                    isEditable = false;
                }
            }

            return isEditable;
        }

        [HttpPost]
        public JsonResult CreateRequisitionForMaintenance(Guid mntsGUID)
        {
            string strmntsGUID = string.Empty;
            //Guid mntsGUID = Guid.Empty;
            //Guid.TryParse(strmntsGUID, out mntsGUID);
            string message = string.Empty, status = "ok";
            ToolsMaintenanceDTO objToolsMaintenanceDTO = new ToolsMaintenanceDTO();
            RequisitionMasterDTO objRequisitionMasterDTO = new RequisitionMasterDTO();
            ToolsMaintenanceDAL objToolsMaintenanceDAL = new ToolsMaintenanceDAL(enterPriseDBName);
            RequisitionMasterDAL objRequisitionMasterDAL = new RequisitionMasterDAL(enterPriseDBName);

            if (mntsGUID != Guid.Empty)
            {
                objToolsMaintenanceDTO = objToolsMaintenanceDAL.GetToolsMaintenanceByGuidPlain(mntsGUID);
            }
            if (objToolsMaintenanceDTO != null)
            {
                if (objToolsMaintenanceDTO.RequisitionGUID.HasValue)
                {
                    objRequisitionMasterDTO = objRequisitionMasterDAL.GetRequisitionByGUIDFull(objToolsMaintenanceDTO.RequisitionGUID.Value);
                }
                if (objRequisitionMasterDTO == null || objRequisitionMasterDTO.ID == 0)
                {
                    RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(enterPriseDBName);
                    DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);

                    string nextReqNo = new AutoSequenceDAL(enterPriseDBName).GetNextAutoNumberByModule("NextRequisitionNo", SessionHelper.RoomID, SessionHelper.CompanyID);
                    objRequisitionMasterDTO = new RequisitionMasterDTO()
                    {
                        Action = "insert",
                        AddedFrom = "web",
                        AppendedBarcodeString = string.Empty,
                        ApprovedQuantity = 0,
                        BillingAccount = string.Empty,
                        BillingAccountID = null,
                        CompanyID = SessionHelper.CompanyID,
                        Created = DateTime.UtcNow,
                        CreatedBy = SessionHelper.UserID,
                        CreatedByName = SessionHelper.UserName,
                        CustomerGUID = Guid.Empty,
                        Description = string.Empty,
                        Customer = string.Empty,
                        CreatedDate = string.Empty,
                        CustomerID = null,
                        EditedFrom = "web",
                        GUID = Guid.NewGuid(),
                        ID = 0,
                        IsArchived = false,
                        IsDeleted = false,
                        LastUpdatedBy = SessionHelper.UserID,
                        NumberofItemsrequisitioned = 0,
                        ReceivedOn = DateTime.UtcNow,
                        ProjectSpendGUID = null,
                        ReceivedOnWeb = DateTime.UtcNow,
                        RequiredDate = datetimetoConsider.Date,
                        RequisitionNumber = nextReqNo,
                        RequisitionStatus = "Unsubmitted",
                        RequisitionType = "tool",
                        Room = SessionHelper.RoomID,
                        WorkorderGUID = objToolsMaintenanceDTO.WorkorderGUID,
                        Updated = DateTime.UtcNow,
                        WhatWhereAction = "Requisition Maintainance",
                    };

                    //var releaseNumber = objRequisitionMasterDAL.GetRequisitionReleaseNumber(objRequisitionMasterDTO.ID, objRequisitionMasterDTO.RequisitionNumber, SessionHelper.CompanyID, SessionHelper.RoomID);
                    //objRequisitionMasterDTO.ReleaseNumber = Convert.ToString(releaseNumber);

                    objRequisitionMasterDTO.GUID = objRequisitionMasterDAL.Insert(objRequisitionMasterDTO).GUID;
                    objToolsMaintenanceDTO.RequisitionGUID = objRequisitionMasterDTO.GUID;
                    objToolsMaintenanceDAL.Edit(objToolsMaintenanceDTO);
                }

            }
            return Json(new { Message = message, Status = status, ReqDTO = objRequisitionMasterDTO, mntsDTO = objToolsMaintenanceDTO });
        }
        public JsonResult GetAssetList(string NameStartWith)
        {
            AssetMasterDAL objCtrl = new AssetMasterDAL(enterPriseDBName);
            List<AssetMasterDTO> lstDTO;
            Int64 RoomID = SessionHelper.RoomID;
            Int64 CompanyID = SessionHelper.CompanyID;
            List<DTOForAutoComplete> returnKeyValList = new List<DTOForAutoComplete>();
            try
            {
                lstDTO = objCtrl.GetAllAssetsByRoom(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).OrderBy(i => i.AssetName).ToList();





                if (lstDTO != null && lstDTO.Count() > 0)
                {
                    foreach (var item in lstDTO)
                    {
                        if (!string.IsNullOrEmpty(item.AssetName))
                        {
                            DTOForAutoComplete obj = new DTOForAutoComplete()
                            {

                                Key = Convert.ToString(item.AssetName),
                                Value = item.AssetName,
                                ID = item.ID,
                                GUID = item.GUID,
                            };

                            returnKeyValList.Add(obj);
                        }
                        else
                        {
                            DTOForAutoComplete obj = new DTOForAutoComplete()
                            {

                                Key = Convert.ToString(item.AssetName),
                                Value = item.AssetName,
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
        public JsonResult GetToolList(string NameStartWith)
        {
            ToolMasterDAL objCtrl = this.objToolMasterDAL; //new ToolMasterDAL(enterPriseDBName);
            List<ToolMasterDTO> lstDTO;
            Int64 RoomID = SessionHelper.RoomID;
            Int64 CompanyID = SessionHelper.CompanyID;
            List<DTOForAutoComplete> returnKeyValList = new List<DTOForAutoComplete>();
            try
            {
                lstDTO = objCtrl.GetToolByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(i => i.ToolName).ToList();

                if (lstDTO != null && lstDTO.Count() > 0)
                {
                    foreach (var item in lstDTO)
                    {
                        if (!string.IsNullOrEmpty(item.ToolName))
                        {
                            DTOForAutoComplete obj = new DTOForAutoComplete()
                            {

                                Key = Convert.ToString(item.Serial),
                                Value = item.ToolName,
                                ID = item.ID,
                                GUID = item.GUID,
                            };

                            returnKeyValList.Add(obj);
                        }
                        else
                        {
                            DTOForAutoComplete obj = new DTOForAutoComplete()
                            {

                                Key = Convert.ToString(item.Serial),
                                Value = item.ToolName,
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
        public JsonResult GetTechnician(string NameStartWith)
        {
            TechnicialMasterDAL objTechnicianCntrl = new eTurns.DAL.TechnicialMasterDAL(enterPriseDBName);
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
        public JsonResult UnDeleteScheduleRecords(string ids)
        {
            try
            {
                AssetMasterDAL obj = new AssetMasterDAL(enterPriseDBName);
                obj.UnDeleteMappingRecords(ids, SessionHelper.UserID, SessionHelper.CompanyID);

                return Json(new { Message = ResMessage.DeletedSuccessfully, Status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message.ToString(), Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetSchedulByGUID(Guid? ScheduleGUID)
        {
            try
            {
                AssetMasterDAL objAssetMasterDAL = new AssetMasterDAL(enterPriseDBName);
                return Json(objAssetMasterDAL.GetToolSchedulebyGUID(ScheduleGUID ?? Guid.Empty));
            }
            catch (Exception ex)
            {
                return Json(ex);
            }
        }
        public string MoveAssetImages()
        {
            try
            {
                string errorWhileCopy = " ";

                eTurnsMaster.DAL.EnterpriseMasterDAL obj = new eTurnsMaster.DAL.EnterpriseMasterDAL();
                List<EnterpriseDTO> lstEnterpriseList = obj.GetAllEnterprisesPlain();
                foreach (EnterpriseDTO e in lstEnterpriseList)
                {
                    List<AssetMasterDTO> objAssetMasterDTO = new List<AssetMasterDTO>();
                    AssetMasterDAL objAssetMasterDAL = new AssetMasterDAL(e.EnterpriseDBName);
                    objAssetMasterDTO = objAssetMasterDAL.GetAllRecordsOnlyImages().ToList();
                    foreach (AssetMasterDTO a in objAssetMasterDTO)
                    {
                        try
                        {
                            Int64 AssetId = a.ID;
                            Int64 EnterpriseId = e.ID;
                            Int64 CompanyId = a.CompanyID ?? 0;
                            Int64 RoomId = a.Room ?? 0;

                            //XElement Settinfile = XElement.Load(Server.MapPath("/SiteSettings.xml"));
                            string UNCPathRoot = SiteSettingHelper.AssetPhoto; // Settinfile.Element("AssetPhoto").Value;
                            //string UNCPathRoot = System.Configuration.ConfigurationManager.AppSettings["AssetPhoto"].ToString();

                            string LogoPath = Server.MapPath(UNCPathRoot + EnterpriseId + "/" + CompanyId + "/" + RoomId + "/" + a.ID + "/");
                            if (!Directory.Exists(LogoPath))
                            {
                                Directory.CreateDirectory(LogoPath);
                            }
                            string ActualDirectory = Server.MapPath("~/Uploads/AssetPhoto/" + CompanyId + "/" + a.ImagePath);
                            string CopyToDirectory = LogoPath + a.ImagePath;
                            System.IO.File.Copy(ActualDirectory, CopyToDirectory, true);
                        }
                        catch (Exception ex)
                        {
                            errorWhileCopy += " " + ex.Message.ToString();
                        }
                    }
                }
                return ResAssetMaster.MsgImageMoved + errorWhileCopy; 
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public string MoveItemFiles()
        {
            try
            {
                string errorWhileCopy = " ";

                eTurnsMaster.DAL.EnterpriseMasterDAL obj = new eTurnsMaster.DAL.EnterpriseMasterDAL();
                List<EnterpriseDTO> lstEnterpriseList = obj.GetAllEnterprisesPlain();
                foreach (EnterpriseDTO e in lstEnterpriseList)
                {
                    List<ItemMasterDTO> objItemMasterDTO = new List<ItemMasterDTO>();
                    ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(e.EnterpriseDBName);
                    objItemMasterDTO = objItemMasterDAL.GetAllRecordsOnlyImages().ToList();
                    foreach (ItemMasterDTO a in objItemMasterDTO)
                    {
                        try
                        {
                            if (!string.IsNullOrWhiteSpace(a.Link2))
                            {
                                try
                                {
                                    Int64 AssetId = a.ID;
                                    Int64 EnterpriseId = e.ID;
                                    Int64 CompanyId = a.CompanyID ?? 0;
                                    Int64 RoomId = a.Room ?? 0;
                                    //XElement Settinfile = XElement.Load(Server.MapPath("/SiteSettings.xml"));
                                    string UNCPathRoot = SiteSettingHelper.InventoryLink2; // Settinfile.Element("InventoryLink2").Value;

                                    //string UNCPathRoot = System.Configuration.ConfigurationManager.AppSettings["ItemImageLink2"].ToString();

                                    string LogoPath = Server.MapPath(UNCPathRoot + EnterpriseId + "/" + CompanyId + "/" + RoomId + "/" + a.ID + "/");
                                    if (!Directory.Exists(LogoPath))
                                    {
                                        Directory.CreateDirectory(LogoPath);
                                    }
                                    string ActualDirectory = Server.MapPath("~/Uploads/InventoryPhoto/Link2/" + a.ID + "/" + a.Link2);
                                    string CopyToDirectory = LogoPath + a.Link2;
                                    if (System.IO.File.Exists(ActualDirectory))
                                    {

                                        System.IO.File.Copy(ActualDirectory, CopyToDirectory, true);
                                    }
                                    else
                                    {
                                        ActualDirectory = Server.MapPath("~/Uploads/Link2/" + a.ID + "/" + a.Link2);
                                        System.IO.File.Copy(ActualDirectory, CopyToDirectory, true);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    errorWhileCopy += " " + ex.Message.ToString();
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(a.ImagePath))
                            {
                                try
                                {
                                    Int64 AssetId = a.ID;
                                    Int64 EnterpriseId = e.ID;
                                    Int64 CompanyId = a.CompanyID ?? 0;
                                    Int64 RoomId = a.Room ?? 0;
                                    //System.Xml.Linq.XElement Settinfile = System.Xml.Linq.XElement.Load(Server.MapPath("/SiteSettings.xml"));
                                    string UNCPathRoot = SiteSettingHelper.InventoryPhoto; // Settinfile.Element("InventoryPhoto").Value;
                                    //string UNCPathRoot = System.Configuration.ConfigurationManager.AppSettings["ItemImage"].ToString();

                                    string LogoPath = Server.MapPath(UNCPathRoot + EnterpriseId + "/" + CompanyId + "/" + RoomId + "/" + a.ID + "/");
                                    if (!Directory.Exists(LogoPath))
                                    {
                                        Directory.CreateDirectory(LogoPath);
                                    }
                                    string ActualDirectory = Server.MapPath("~/Uploads/InventoryPhoto/" + CompanyId + "/" + a.ImagePath);
                                    string CopyToDirectory = LogoPath + a.ImagePath;
                                    System.IO.File.Copy(ActualDirectory, CopyToDirectory, true);
                                }
                                catch (Exception ex)
                                {
                                    errorWhileCopy += " " + ex.Message.ToString();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            errorWhileCopy += " " + ex.Message.ToString();
                        }
                    }
                }
                return ResAssetMaster.MsgImageMoved + errorWhileCopy;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        //public string MoveSupplierFiles()
        //{
        //    try
        //    {
        //        string errorWhileCopy = " ";

        //        eTurnsMaster.DAL.EnterpriseMasterDAL obj = new eTurnsMaster.DAL.EnterpriseMasterDAL();
        //        List<EnterpriseDTO> lstEnterpriseList = obj.GetAllEnterprise(false);
        //        foreach (EnterpriseDTO e in lstEnterpriseList)
        //        {
        //            List<SupplierMasterDTO> objSupplierMasterDTO = new List<SupplierMasterDTO>();
        //            SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(e.EnterpriseDBName);
        //            objSupplierMasterDTO = objSupplierMasterDAL.GetAllRecordsOnlyImages().ToList();
        //            foreach (SupplierMasterDTO a in objSupplierMasterDTO)
        //            {
        //                try
        //                {

        //                    if (!string.IsNullOrWhiteSpace(a.SupplierImage))
        //                    {
        //                        Int64 AssetId = a.ID;
        //                        Int64 EnterpriseId = e.ID;
        //                        Int64 CompanyId = a.CompanyID ?? 0;
        //                        Int64 RoomId = a.Room ?? 0;
        //                        //string UNCPathRoot = System.Configuration.ConfigurationManager.AppSettings["SupplierPhoto"].ToString();
        //                        System.Xml.Linq.XElement Settinfile = System.Xml.Linq.XElement.Load(Server.MapPath("/SiteSettings.xml"));
        //                        string UNCPathRoot = Settinfile.Element("SupplierPhoto").Value;
        //                        string LogoPath = Server.MapPath(UNCPathRoot + EnterpriseId + "/" + CompanyId + "/" + RoomId + "/" + a.ID + "/");
        //                        if (!Directory.Exists(LogoPath))
        //                        {
        //                            Directory.CreateDirectory(LogoPath);
        //                        }
        //                        string ActualDirectory = Server.MapPath("~/Uploads/SupplierLogos/" + a.ID + "/" + a.SupplierImage);
        //                        string CopyToDirectory = LogoPath + a.SupplierImage;
        //                        System.IO.File.Copy(ActualDirectory, CopyToDirectory, true);
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    errorWhileCopy += " " + ex.Message.ToString();
        //                }
        //            }
        //        }
        //        return "Image moved properly." + errorWhileCopy;
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message.ToString();
        //    }
        //}
        //public string MoveToolFiles()
        //{
        //    try
        //    {
        //        string errorWhileCopy = " ";

        //        eTurnsMaster.DAL.EnterpriseMasterDAL obj = new eTurnsMaster.DAL.EnterpriseMasterDAL();
        //        List<EnterpriseDTO> lstEnterpriseList = obj.GetAllEnterprise(false);
        //        foreach (EnterpriseDTO e in lstEnterpriseList)
        //        {
        //            List<ToolMasterDTO> objSupplierMasterDTO = new List<ToolMasterDTO>();
        //            ToolMasterDAL objToolMasterDAL = new ToolMasterDAL(e.EnterpriseDBName);
        //            objSupplierMasterDTO = objToolMasterDAL.GetAllRecordsOnlyImages().ToList();
        //            foreach (ToolMasterDTO a in objSupplierMasterDTO)
        //            {
        //                try
        //                {

        //                    if (!string.IsNullOrWhiteSpace(a.ImagePath))
        //                    {
        //                        Int64 AssetId = a.ID;
        //                        Int64 EnterpriseId = e.ID;
        //                        Int64 CompanyId = a.CompanyID ?? 0;
        //                        Int64 RoomId = a.Room ?? 0;
        //                        //string UNCPathRoot = System.Configuration.ConfigurationManager.AppSettings["ToolPhoto"].ToString();
        //                        System.Xml.Linq.XElement Settinfile = System.Xml.Linq.XElement.Load(Server.MapPath("/SiteSettings.xml"));
        //                        string UNCPathRoot = Settinfile.Element("ToolPhoto").Value;


        //                        string LogoPath = Server.MapPath(UNCPathRoot + EnterpriseId + "/" + CompanyId + "/" + RoomId + "/" + a.ID + "/");
        //                        if (!Directory.Exists(LogoPath))
        //                        {
        //                            Directory.CreateDirectory(LogoPath);
        //                        }
        //                        string ActualDirectory = Server.MapPath("~/Uploads/ToolPhoto/" + a.CompanyID + "/" + a.ImagePath);
        //                        string CopyToDirectory = LogoPath + a.ImagePath;
        //                        System.IO.File.Copy(ActualDirectory, CopyToDirectory, true);
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    errorWhileCopy += " " + ex.Message.ToString();
        //                }
        //            }
        //        }
        //        return "Image moved properly." + errorWhileCopy;
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message.ToString();
        //    }
        //}
        public string MoveWorkOrderFiles()
        {
            try
            {
                string errorWhileCopy = " ";

                eTurnsMaster.DAL.EnterpriseMasterDAL obj = new eTurnsMaster.DAL.EnterpriseMasterDAL();
                List<EnterpriseDTO> lstEnterpriseList = obj.GetAllEnterprisesPlain();
                foreach (EnterpriseDTO e in lstEnterpriseList)
                {
                    List<WorkOrderDTO> objWorkOrderDTO = new List<WorkOrderDTO>();
                    WorkOrderDAL objWorkOrderDAL = new WorkOrderDAL(e.EnterpriseDBName);
                    objWorkOrderDTO = objWorkOrderDAL.GetAllRecordsOnlyImages();
                    foreach (WorkOrderDTO a in objWorkOrderDTO)
                    {
                        try
                        {
                            List<WorkOrderImageDetail> objWorkOrderImageDetail = new List<WorkOrderImageDetail>();

                            objWorkOrderImageDetail = objWorkOrderDAL.GetWorkorderImagesByWOGuidPlain(a.GUID).ToList();
                            foreach (WorkOrderImageDetail b in objWorkOrderImageDetail)
                            {
                                try
                                {
                                    if (!string.IsNullOrWhiteSpace(b.WOImageName))
                                    {
                                        Int64 AssetId = a.ID;
                                        Int64 EnterpriseId = e.ID;
                                        Int64 CompanyId = a.CompanyID ?? 0;
                                        Int64 RoomId = a.Room ?? 0;
                                        //string UNCPathRoot = System.Configuration.ConfigurationManager.AppSettings["WorkOrderFilePaths"].ToString();
                                        //System.Xml.Linq.XElement Settinfile = System.Xml.Linq.XElement.Load(Server.MapPath("/SiteSettings.xml"));
                                        string UNCPathRoot = SiteSettingHelper.WorkOrderFilePaths; // Settinfile.Element("WorkOrderFilePaths").Value;

                                        string LogoPath = Server.MapPath(UNCPathRoot + EnterpriseId + "/" + CompanyId + "/" + RoomId + "/" + a.ID + "/");
                                        if (!Directory.Exists(LogoPath))
                                        {
                                            Directory.CreateDirectory(LogoPath);
                                        }
                                        string ActualDirectory = Server.MapPath("~/Uploads/WorkOrderFile/" + a.CompanyID + "/" + a.ID + "/" + b.WOImageName);
                                        string CopyToDirectory = LogoPath + b.WOImageName;
                                        System.IO.File.Copy(ActualDirectory, CopyToDirectory, true);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    errorWhileCopy += " " + ex.Message.ToString();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            errorWhileCopy += " " + ex.Message.ToString();
                        }
                    }
                }
                return ResAssetMaster.MsgImageMoved + errorWhileCopy;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public ActionResult LoadItemMasterModelMaintenance(Int64 ParentId)
        {
            ToolsMaintenanceDTO objDTO = new ToolsMaintenanceDAL(enterPriseDBName).GetToolsMaintenanceByIdPlain(ParentId);

            string modelHeader = eTurns.DTO.ResToolsMaintenance.ItemModelHeader;

            ItemModelPerameter obj = null;

            obj = new ItemModelPerameter()
            {
                AjaxURLAddItemToSession = "~/Order/AddItemsToOrder/", // Not Used
                PerentID = ParentId.ToString(),
                PerentGUID = objDTO.GUID.ToString(),
                ModelHeader = modelHeader,
                AjaxURLAddMultipleItemToSession = "~/Assets/AddItemsToMaintenance/",
                AjaxURLToFillItemGrid = "~/Assets/GetItemsModelMethodMaintenance/",
                CallingFromPageName = "MNTNANCE",
                OrdStagingID = "",
                OrdRequeredDate = objDTO.ScheduleDate.Value.ToString("MM/dd/yyyy"),

            };

            return PartialView("ItemMasterModel", obj);
        }

        public JsonResult AddItemsToMaintenance(ToolMaintenanceDetailsDTO[] objNewItems, Int64? MaintenanceId)
        {
            string message = ResMessage.SaveMessage;
            string status = "ok";
            ToolsMaintenanceDTO objToolsMaintenanceDTO = null;
            ToolsMaintenanceDAL objToolsMaintenanceDAL = null;
            //BinMasterDTO objBINDTO = null;
            BinMasterDAL objBinDAL = null;
            ItemMasterDAL objItemDAL = null;
            List<ToolMaintenanceDetailsDTO> lstReturnsForSameItemWithBin = null;
            List<ToolMaintenanceDetailsDTO> lstSaveItem = new List<ToolMaintenanceDetailsDTO>();
            List<DTOForAutoComplete> lstAddedItemsBin = null;
            if (objNewItems != null && objNewItems.Count() > 0)
            {
                MaintenanceId = objNewItems.FirstOrDefault().MaintenanceId ?? 0;
            }
            try
            {
                objToolsMaintenanceDAL = new ToolsMaintenanceDAL(enterPriseDBName);
                List<ToolMaintenanceDetailsDTO> lstDetails = new List<ToolMaintenanceDetailsDTO>();// GetLineItemsFromSession(OrderID);
                objToolsMaintenanceDTO = objToolsMaintenanceDAL.GetToolsMaintenanceByIdPlain(MaintenanceId ?? 0);
                string binName = string.Empty;

                objBinDAL = new BinMasterDAL(enterPriseDBName);
                objItemDAL = new ItemMasterDAL(enterPriseDBName);



                if (objNewItems != null && objNewItems.Length > 0)
                {
                    lstReturnsForSameItemWithBin = new List<ToolMaintenanceDetailsDTO>();
                    lstSaveItem = new List<ToolMaintenanceDetailsDTO>();
                    lstAddedItemsBin = new List<DTOForAutoComplete>();
                    foreach (var item in objNewItems)
                    {
                        if (lstDetails != null && lstDetails.Count > 0 && lstDetails.Where(a => a.ItemGUID == item.ItemGUID.GetValueOrDefault(Guid.Empty)).Count() > 0)
                        {
                            lstReturnsForSameItemWithBin.Add(item);
                            // continue;
                        }
                        else
                        {
                            lstSaveItem.Add(item);
                        }


                    }
                    ItemMasterDAL objItemmasterDAL = new ItemMasterDAL(enterPriseDBName);
                    List<ItemMasterDTO> lstItemMasterDTO = new List<ItemMasterDTO>();
                    lstItemMasterDTO = objItemmasterDAL.GetAllItemsWithoutJoins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, null);
                    foreach (ToolMaintenanceDetailsDTO t in lstSaveItem)
                    {

                        ItemMasterDTO objItem = lstItemMasterDTO.Where(i => i.GUID == t.ItemGUID).FirstOrDefault();
                        if (t.ItemCost == null)
                        {
                            t.ItemCost = objItem.Cost;
                        }

                        ToolMaintenanceDetailsDAL objToolMaintenanceDetailsDAL = new ToolMaintenanceDetailsDAL(enterPriseDBName);
                        ToolMaintenanceDetailsDTO objToolMaintenanceDetailsDTO = new ToolMaintenanceDetailsDTO();
                        objToolMaintenanceDetailsDTO.CompanyID = SessionHelper.CompanyID;
                        objToolMaintenanceDetailsDTO.Created = DateTimeUtility.DateTimeNow;
                        objToolMaintenanceDetailsDTO.CreatedBy = SessionHelper.UserID;
                        objToolMaintenanceDetailsDTO.GUID = Guid.NewGuid();
                        objToolMaintenanceDetailsDTO.ID = 0;
                        objToolMaintenanceDetailsDTO.IsArchived = false;
                        objToolMaintenanceDetailsDTO.IsDeleted = false;
                        objToolMaintenanceDetailsDTO.ItemCost = t.ItemCost;
                        objToolMaintenanceDetailsDTO.ItemGUID = t.ItemGUID;
                        objToolMaintenanceDetailsDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        objToolMaintenanceDetailsDTO.LastUpdatedBy = SessionHelper.UserID;
                        objToolMaintenanceDetailsDTO.MaintenanceGUID = t.MaintenanceGUID;
                        objToolMaintenanceDetailsDTO.Quantity = t.Quantity;
                        objToolMaintenanceDetailsDTO.Room = SessionHelper.RoomID;
                        objToolMaintenanceDetailsDAL.Insert(objToolMaintenanceDetailsDTO);

                        ToolsSchedulerDetailsDAL objToolsSchedulerDetailsDAL = new ToolsSchedulerDetailsDAL(enterPriseDBName);
                        ToolsSchedulerDetailsDTO objToolsSchedulerDetailsDTO = new ToolsSchedulerDetailsDTO();
                        //List<ToolsSchedulerDetailsDTO> lstToolsSchedulerDetailsDTO = objToolsSchedulerDetailsDAL.GetAllRecords(objToolsMaintenanceDTO.SchedulerGUID ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
                        List<ToolsSchedulerDetailsDTO> lstToolsSchedulerDetailsDTO = objToolsSchedulerDetailsDAL.GetToolScheduleLineItemsNormal(objToolsMaintenanceDTO.SchedulerGUID ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID);
                        if (lstToolsSchedulerDetailsDTO == null || lstToolsSchedulerDetailsDTO.Count() == 0 || lstToolsSchedulerDetailsDTO.Where(l => l.ItemGUID == t.ItemGUID).Count() == 0)
                        {

                            objToolsSchedulerDetailsDTO.CompanyID = SessionHelper.CompanyID;
                            objToolsSchedulerDetailsDTO.Created = DateTimeUtility.DateTimeNow;
                            objToolsSchedulerDetailsDTO.CreatedBy = SessionHelper.UserID;
                            objToolsSchedulerDetailsDTO.GUID = Guid.NewGuid();
                            objToolsSchedulerDetailsDTO.ID = 0;
                            objToolsSchedulerDetailsDTO.IsArchived = false;
                            objToolsSchedulerDetailsDTO.IsDeleted = false;
                            objToolsSchedulerDetailsDTO.ItemCost = t.ItemCost;
                            objToolsSchedulerDetailsDTO.ItemGUID = t.ItemGUID;
                            objToolsSchedulerDetailsDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                            objToolsSchedulerDetailsDTO.LastUpdatedBy = SessionHelper.UserID;
                            objToolsSchedulerDetailsDTO.Quantity = t.Quantity;
                            objToolsSchedulerDetailsDTO.Room = SessionHelper.RoomID;
                            objToolsSchedulerDetailsDTO.ScheduleGUID = objToolsMaintenanceDTO.SchedulerGUID;
                            objToolsSchedulerDetailsDAL.Insert(objToolsSchedulerDetailsDTO);
                        }
                    }
                    //SessionHelper.Add(GetSessionKey(MaintenanceId ?? 0), lstDetails);
                    //if (lstReturnsForSameItemWithBin.Count <= 0)
                    //    message = objNewItems.Length + " Items are added to Maintenance";
                    //else if (objNewItems.Length - lstReturnsForSameItemWithBin.Count > 0)
                    //    message = (objNewItems.Length - lstReturnsForSameItemWithBin.Count) + " Item(s) are added, and " + lstReturnsForSameItemWithBin.Count + " Items already available in maintenance.";
                    //else
                    //    message = "Not added, items already exists in maintenance.";

                    //status = "ok";
                    return Json(new { Message = message, Status = status, AlreadyExistsItems = lstReturnsForSameItemWithBin, AddedBins = lstAddedItemsBin }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
                message = "Error";
                status = "fail";
                return Json(new { Message = ex.Message, Status = status }, JsonRequestBehavior.AllowGet);
                //throw;
            }
            finally
            {
                objToolsMaintenanceDTO = null;
                objToolsMaintenanceDAL = null;

                //objBINDTO = null;
                objBinDAL = null;
                objItemDAL = null;
            }
        }
        private string GetSessionKey(Int64 MaintenanceId = 0)
        {
            string strKey = "MaintenanceLineItem_" + SessionHelper.EnterPriceID + "_" + CompanyID + "_" + RoomID;
            return strKey;
        }
        public ActionResult GetItemsModelMethodMaintenance(QuickListJQueryDataTableParamModel param)
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

            //make changes to resolve an issue of Sort (WI-431)
            if (sortColumnName == "0" || sortColumnName.Contains("undefined"))
                sortColumnName = "ItemNumber Asc";

            if (Request["sSearch_0"] != null && !string.IsNullOrEmpty(Request["sSearch_0"]))
            {
                param.sSearch = Request["sSearch_0"].Trim(',');
            }

            string searchQuery = string.Empty;
            int TotalRecordCount = 0;
            Int64 MaintenanceID = 0;
            Int64.TryParse(Request["ParentID"], out MaintenanceID);
            List<ToolMaintenanceDetailsDTO> lstItems = GetLineItemsFromSession(MaintenanceID);
            string ExclueBinMasterGUIDs = "";

            if (lstItems != null && lstItems.Count > 0)
            {
                ExclueBinMasterGUIDs = String.Join(",", lstItems.Select(x => (x.ItemGUID == null ? "" : x.ItemGUID.ToString())));
            }

            string ItemsIDs = "";
            string modelPopupFor = "maintenance";
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = new ItemMasterDAL(enterPriseDBName).GetPagedItemsForModel(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, RoomID, CompanyID, IsArchived, IsDeleted, SessionHelper.UserSupplierIds, true, true, true, SessionHelper.UserID, modelPopupFor, Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone, true, ItemsIDs, null, ExclueBinMasterGUIDs).ToList();
            var jsonResult = Json(new { sEcho = param.sEcho, iTotalRecords = TotalRecordCount, iTotalDisplayRecords = TotalRecordCount, aaData = DataFromDB }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;
        }
        private List<ToolMaintenanceDetailsDTO> GetLineItemsFromSession(Int64 MaintenanceId)
        {

            List<ToolMaintenanceDetailsDTO> lstDetailDTO = new List<ToolMaintenanceDetailsDTO>();

            List<ToolMaintenanceDetailsDTO> lstDetails = (List<ToolMaintenanceDetailsDTO>)SessionHelper.Get(GetSessionKey(MaintenanceId));
            if (lstDetails != null && lstDetails.Count > 0)
            {
                lstDetailDTO = lstDetails;
            }

            return lstDetailDTO;
        }

        public ActionResult LoadMaintenanceItems(Int64 MaintenanceID)
        {
            ViewBag.MaintenanceID = MaintenanceID;

            ToolMaintenanceDetailsDAL obj = new eTurns.DAL.ToolMaintenanceDetailsDAL(enterPriseDBName);
            List<ToolMaintenanceDetailsDTO> lst = obj.GetToolMaintenanceDetailsByMaintenanceIdNormal(MaintenanceID, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            ViewBag.NofLineItem = lst.Count();
            ViewBag.Cost = lst.Sum(x => double.Parse((x.Quantity.GetValueOrDefault(0) == 0 ? x.Quantity.GetValueOrDefault(0) : x.Quantity.GetValueOrDefault(0)).ToString()) * ((x.ItemCost.GetValueOrDefault(0)) / ((x.CostUOMValue ?? 0) == 0 ? 1 : (x.CostUOMValue ?? 1))));
            return PartialView("_CreateMaintenanceItems", lst);
        }
        public string DeleteToolMaintenance(string ids)
        {
            try
            {
                ToolMaintenanceDetailsDAL obj = new eTurns.DAL.ToolMaintenanceDetailsDAL(enterPriseDBName);
                obj.DeleteToolMaintenanceDetailsByids(ids, SessionHelper.UserID);
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public ActionResult LoadToolHistoryList()
        {
            return PartialView("_ToolHistoryListForDisplay");
        }

        public ActionResult ToolHistoryListAjax(JQueryDataTableParamModel param)
        {
            ToolMasterDAL obj = this.objToolMasterDAL; //new eTurns.DAL.ToolMasterDAL(enterPriseDBName);

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
            var DataFromDB = obj.GetPagedToolHistory(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, null, null, RoomDateFormat, CurrentTimeZone);


            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);
        }

        public string CheckInCheckOutHistoryData(Guid ToolGUID)
        {
            ViewBag.ToolGUID = ToolGUID;
            ToolCheckInOutHistoryDAL objDAL = new ToolCheckInOutHistoryDAL(enterPriseDBName);
            //var objModel = objDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.ToolGUID == ToolGUID).ToList();
            var objModel = objDAL.GetTCIOHsByToolGUIDFull(ToolGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
            return RenderRazorViewToString("_CheckInCheckOutHistoryData", objModel);
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
            ToolCheckInOutHistoryDAL objDAL = new ToolCheckInOutHistoryDAL(enterPriseDBName);
            int TotalRecordCount = 0;
            //var objModel = objDAL.GetPagedRecords_History(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, ToolGUID, reqDetGUID, WOGUID, TCGUID);
            var objModel = objDAL.GetPagedToolCheckouts(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, ToolGUID, reqDetGUID, WOGUID, TCGUID, "hist");

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = objModel
            },
                        JsonRequestBehavior.AllowGet);
        }


        public JsonResult BlankSession()
        {
            Session["IsInsert"] = "";
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult WrittenOffTool(List<ToolWrittenOffDTO> WrittenOffTools)
        {
            List<ItemPullInfo> oReturn = new List<ItemPullInfo>();

            if (WrittenOffTools != null && WrittenOffTools.Any())
            {
                foreach (var ToolWrittenOff in WrittenOffTools)
                {
                    if (ToolWrittenOff != null && ToolWrittenOff.ToolGUID != Guid.Empty && ToolWrittenOff.Quantity > 0)
                    {
                        ToolWrittenOffDAL toolWrittenOffDAL = new ToolWrittenOffDAL(enterPriseDBName);
                        ToolWrittenOff.CompanyID = SessionHelper.CompanyID;
                        ToolWrittenOff.Room = SessionHelper.RoomID;
                        ToolWrittenOff.CreatedBy = SessionHelper.UserID;
                        ToolWrittenOff.LastUpdatedBy = SessionHelper.UserID;
                        long id = toolWrittenOffDAL.Insert(ToolWrittenOff);
                        var errorMessage = string.Empty;
                        string reasonToFail = string.Empty;
                        string reasonToFailErrorCode = string.Empty;

                        if (id > 0)
                        {
                            ToolMasterDAL toolMasterDAL = this.objToolMasterDAL; //new ToolMasterDAL(enterPriseDBName);
                            var result = toolMasterDAL.UpdateToolQuantityOnToolWrittenOff(ToolWrittenOff.ToolGUID.GetValueOrDefault(Guid.Empty), ToolWrittenOff.Quantity.GetValueOrDefault(0), SessionHelper.UserID, SessionHelper.CompanyID, SessionHelper.RoomID, out reasonToFailErrorCode, false);

                            if (!string.IsNullOrEmpty(reasonToFailErrorCode))
                            {
                                reasonToFail = SetErrorMessage(reasonToFailErrorCode);
                            }

                            if (!result && reasonToFail != string.Empty)
                                errorMessage = reasonToFail;
                            else if (!result)
                                errorMessage = ResToolMaster.FailToWrittenOffTool;
                        }
                        else
                        {
                            errorMessage = ResToolMaster.FailToWrittenOffTool;
                        }
                        var responseObject = new ItemPullInfo
                        {
                            ItemGUID = ToolWrittenOff.ToolGUID.GetValueOrDefault(Guid.Empty),
                            ErrorMessage = errorMessage
                        };
                        oReturn.Add(responseObject);
                    }
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

        public ActionResult LoadWrittenOffToolList()
        {
            return PartialView("_WrittenOffToolList");
        }

        public ActionResult GetWrittenOffToolList(JQueryDataTableParamModel param)
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

            ToolWrittenOffDAL toolWrittenOffDAL = new ToolWrittenOffDAL(enterPriseDBName);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            IEnumerable<ToolWrittenOffDTO> DataFromDB = toolWrittenOffDAL.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);
        }

        public string WrittenOffToolData(Guid ToolGUID)
        {
            ViewBag.ToolGUID = ToolGUID;
            Session["WOTDetailToolGuid"] = ToolGUID;
            var objModel = new List<ToolWrittenOffDTO>();
            return RenderRazorViewToString("_WrittenOffToolDetails", objModel);
        }

        public ActionResult GetWrittenOffToolDetail(JQueryDataTableParamModel param)
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
            Guid ToolGuid = Guid.Parse(Request["ToolGUID"].ToString());

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

            ToolWrittenOffDAL toolWrittenOffDAL = new ToolWrittenOffDAL(enterPriseDBName);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            IEnumerable<ToolWrittenOffDTO> DataFromDB = toolWrittenOffDAL.GetWrittenOffToolDetail(ToolGuid, param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is used to delete written off tool details.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public JsonResult DeleteWrittenOffToolRecordsByToolGuids(string ids)
        {
            try
            {
                ToolWrittenOffDAL toolWrittenOffDAL = new ToolWrittenOffDAL(enterPriseDBName);
                var result = toolWrittenOffDAL.DeleteWrittenOffToolRecordsByToolGuids(ids, SessionHelper.UserID, SessionHelper.CompanyID, SessionHelper.RoomID);
                Session["RoomAllWrittenOffTool"] = null; //Used in narrow search
                return Json(new { Message = ResMessage.DeletedSuccessfully, Status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Message = ResMessage.FailToDelete, Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UnWrittenOffToolByToolGuids(string ids)
        {
            try
            {
                ToolWrittenOffDAL toolWrittenOffDAL = new ToolWrittenOffDAL(enterPriseDBName);
                var result = toolWrittenOffDAL.UnWrittenOffToolsByToolGuids(ids, SessionHelper.UserID);
                //Session["RoomAllWrittenOffTool"] = null; //Used in narrow search
                return Json(new { Message = ResMessage.UnWrittenOffSuccessfully, Status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Message = ResMessage.FailToUnWrittenOff, Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// This method is used to delete written off tool details.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public JsonResult DeleteWrittenOffToolRecordsByIds(string ids)
        {
            try
            {
                ToolWrittenOffDAL toolWrittenOffDAL = new ToolWrittenOffDAL(enterPriseDBName);
                var result = toolWrittenOffDAL.DeleteWrittenOffToolRecords(ids, SessionHelper.UserID, SessionHelper.CompanyID, SessionHelper.RoomID);
                Session["RoomAllWrittenOffTool"] = null; //Used in narrow search
                return Json(new { Message = ResMessage.DeletedSuccessfully, Status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Message = ResMessage.FailToDelete, Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteAssetImage(string AssetGUID)
        {
            AssetMasterDAL objAssetMasterDAL = new AssetMasterDAL(enterPriseDBName);
            bool IsUpdate = objAssetMasterDAL.RemoveAssetImage(Guid.Parse(AssetGUID), "web", SessionHelper.UserID);
            if (IsUpdate)
                return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
        }
    }

}
