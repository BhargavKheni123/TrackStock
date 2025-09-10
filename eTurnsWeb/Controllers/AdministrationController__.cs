using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eTurns.DTO;
using eTurnsWeb.Helper;
using eTurns.DTO.Resources;
using System.Text;
using System.Web.Script.Serialization;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Configuration;
using System.Globalization;
using System.Data;
using eTurns.DAL;
using System.Net;
using eTurnsMaster.DAL;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using System.Xml.Linq;
using System.Web.UI.DataVisualization.Charting;
using System.Web.Security;
using CaptchaMvc.Attributes;
using CaptchaMvc.HtmlHelpers;
using Microsoft.AspNet.SignalR;



namespace eTurnsWeb.Controllers
{

    public partial class MasterController : eTurnsControllerBase
    {

        #region [Global variables]
        string[] sepForRoleRoom = new string[] { "[!,!]" };
        string[] sepForRoleRoom2 = new string[] { "[!_!]" };

        #endregion

        #region "Module Master"

        public ActionResult ModuleList()
        {
            return View();
        }

        public PartialViewResult _CreateModule()
        {
            return PartialView();
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult ModuleCreate()
        {
            ModuleMasterDTO objDTO = new ModuleMasterDTO();
            objDTO.ID = 0;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTime.UtcNow;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.RoomName = SessionHelper.RoomName;
            objDTO.GUID = Guid.NewGuid();

            //ModuleMasterController obj = new ModuleMasterController();
            eTurns.DAL.ModuleMasterDAL obj = new eTurns.DAL.ModuleMasterDAL(SessionHelper.EnterPriseDBName);
            //Please check static refrence here
            List<ModuleMasterDTO> objList = obj.GetAllModuleRecords(1).ToList();
            objList.Insert(0, null);
            ViewBag.ParentModuleList = objList;

            var listGroup = new SelectList(new[]{
                          new {ID="1",Name="Support Tables Permissions"},new{ID="2",Name="Module Permissions"},
                          new{ID="3",Name="Admin Permissions"},new{ID="4",Name="Default Settings"},new{ID="5",Name="Special Permissions"},},
                "ID", "Name", 1);
            ViewBag.GroupList = listGroup;


            return PartialView("_CreateModule", objDTO);
        }


        [HttpPost]
        public JsonResult ModuleSave(ModuleMasterDTO objDTO)
        {
            string message = "";
            string status = "";
            //ModuleMasterController obj = new ModuleMasterController();
            eTurns.DAL.ModuleMasterDAL obj = new eTurns.DAL.ModuleMasterDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

            if (string.IsNullOrEmpty(objDTO.ModuleName))
            {
                message = string.Format(ResMessage.Required, ResModuleMaster.ModuleName);// "Module name is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }

            if (objDTO.ID == 0)
            {
                objDTO.CreatedBy = SessionHelper.UserID;

                string strOK = objCDAL.DuplicateCheck(objDTO.ModuleName, "add", objDTO.ID, "ModuleMaster", "ModuleName", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResModuleMaster.ModuleName, objDTO.ModuleName);  //"Module name\"" + objDTO.ModuleName + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    objDTO.GUID = Guid.NewGuid();
                    long ReturnVal = obj.Insert(objDTO);
                    if (ReturnVal > 0)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }
            else
            {
                objDTO.LastUpdatedBy = SessionHelper.UserID;
                objDTO.UpdatedByName = SessionHelper.UserName;
                string strOK = objCDAL.DuplicateCheck(objDTO.ModuleName, "edit", objDTO.ID, "ModuleMaster", "ModuleName", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResModuleMaster.ModuleName, objDTO.ModuleName);  //"Module name\"" + objDTO.ModuleName + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    bool ReturnVal = obj.Edit(objDTO);
                    if (ReturnVal)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }

            //if (status == "duplicate")
            //    throw new Exception("Duplicate Found", new Exception("Duplicate Found"));                
            //else if (status == "fail")
            //    throw new Exception("Error! Record Not Saved");
            //else
            //    return Content(message);
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>
        public ActionResult ModuleEdit(Int64 ID)
        {
            //ModuleMasterController obj = new ModuleMasterController();
            eTurns.DAL.ModuleMasterDAL obj = new eTurns.DAL.ModuleMasterDAL(SessionHelper.EnterPriseDBName);
            ModuleMasterDTO objDTO = obj.GetRecord(ID);

            List<ModuleMasterDTO> objList = obj.GetAllModuleRecords(1).ToList();
            objList.Insert(0, null);
            ViewBag.ParentModuleList = objList;

            var listGroup = new SelectList(new[]{
                          new {ID="1",Name="Support Tables Permissions"},new{ID="2",Name="Module Permissions"},
                          new{ID="3",Name="Admin Permissions"},new{ID="4",Name="Default Settings"},new{ID="5",Name="Special Permissions"},},
              "ID", "Name", 1);
            ViewBag.GroupList = listGroup;

            return PartialView("_CreateModule", objDTO);
            //return View("ModuleEdit",objDTO);
        }

        #region Ajax Data Provider

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult ModuleListAjax(JQueryDataTableParamModel param)
        {
            //ModuleMasterController obj = new ModuleMasterController();
            eTurns.DAL.ModuleMasterDAL obj = new eTurns.DAL.ModuleMasterDAL(SessionHelper.EnterPriseDBName);

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

            // set the default column sorting here, if first time then required to set 
            if (sortColumnName == "0" || sortColumnName == "undefined")
                sortColumnName = "ID";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            var DataFromDB = obj.GetPagedRecords(1, param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName);

            var result = from c in DataFromDB
                         select new ModuleMasterDTO
                         {
                             ID = c.ID,
                             ModuleName = c.ModuleName,
                             DisplayName = c.DisplayName,
                             //  ParentID = c.ParentID,
                             Value = c.Value,
                             //Priority = c.Priority,
                             IsModule = c.IsModule,
                             RoomName = c.RoomName,
                             Created = c.Created,
                             Updated = c.Updated,
                             UpdatedByName = c.UpdatedByName,
                             CreatedByName = c.CreatedByName
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
        public string UpdateModuleData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            //ModuleMasterController obj = new ModuleMasterController();
            eTurns.DAL.ModuleMasterDAL obj = new eTurns.DAL.ModuleMasterDAL(SessionHelper.EnterPriseDBName);
            obj.UpdateData(id, value, rowId, columnPosition, columnId, columnName);
            return value;
        }

        //public string DuplicateModuleCheck(string ModuleName, string ActionMode, int ID)
        //{
        //    ModuleMasterController obj = new ModuleMasterController();
        //    return obj.DuplicateCheck(ModuleName, ActionMode, ID);
        //}


        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public string DeleteModuleRecords(string ids)
        {
            try
            {
                //ModuleMasterController obj = new ModuleMasterController();
                eTurns.DAL.ModuleMasterDAL obj = new eTurns.DAL.ModuleMasterDAL(SessionHelper.EnterPriseDBName);
                obj.DeleteRecords(ids, 1);
                return "ok";

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string DeleteVenderMasterRecords(string ids)
        {
            try
            {
                VenderMasterDAL obj = new eTurns.DAL.VenderMasterDAL(SessionHelper.EnterPriseDBName);
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

        #region "Role Master"

        public ActionResult RoleList()
        {
            UserBAL objUserBAL = new UserBAL();
            List<RoleMasterDTO> lstUsers = new List<RoleMasterDTO>();
            List<RolePermissionInfo> outlstAllPermissions = new List<RolePermissionInfo>();
            lstUsers = objUserBAL.GetAllRolesBySQlHelper(SessionHelper.UserType, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, false, false, SessionHelper.UserID, SessionHelper.RoleID, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, out outlstAllPermissions);
            List<UserAccessDTO> lstUserAccess = new UserBAL().GetRoleAccess(SessionHelper.UserType, SessionHelper.RoleID, SessionHelper.UserID, SessionHelper.EnterPriceID, SessionHelper.CompanyID);
            Session["lstAllRoles"] = lstUsers;
            Session["AllRolePermissions"] = lstUserAccess;
            return View();
        }

        public PartialViewResult _CreateRole()
        {
            return PartialView();
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>

        [AjaxOrChildActionOnlyAttribute]
        public ActionResult RoleCreate()
        {
            if (SessionHelper.UserType == 1)
            {
                RoleMasterDTO objDTO = new RoleMasterDTO();
                objDTO.ID = 0;
                objDTO.Created = DateTimeUtility.DateTimeNow;
                objDTO.Updated = DateTimeUtility.DateTimeNow;
                objDTO.CreatedBy = SessionHelper.UserID;
                objDTO.LastUpdatedBy = SessionHelper.UserID;
                objDTO.Room = SessionHelper.RoomID;
                objDTO.CompanyID = SessionHelper.CompanyID;
                objDTO.RoomName = SessionHelper.RoomName;
                objDTO.GUID = Guid.NewGuid();

                //RoleMasterController obj = new RoleMasterController();
                eTurnsMaster.DAL.RoleMasterDAL obj = new eTurnsMaster.DAL.RoleMasterDAL(SessionHelper.EnterPriseDBName);
                var objList = obj.GetRoleModuleDetailsRecord(0, 0, 0, 0);

                var objMasterList = (from t in objList
                                     where t.GroupId.ToString() == "1" && t.IsModule == true
                                     select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);
                var objOtherModuleList = (from t in objList
                                          where t.GroupId.ToString() == "2" && t.IsModule == true
                                          select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                var objNonModuleList = (from t in objList
                                        where t.IsModule == false && t.GroupId.ToString() != "4"
                                        select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                var objOtherDefaultSettings = (from t in objList
                                               where t.GroupId.ToString() == "4" && t.IsModule == false
                                               select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);
                //objDTO.ModuleMasterList = objMasterList.ToList();
                //objDTO.OtherModuleList = objOtherModuleList.ToList();
                //objDTO.NonModuleList = objNonModuleList.ToList();

                foreach (var item in objNonModuleList)
                {
                    item.DisplayOrderName = ResourceHelper.GetResourceValue(Enum.GetName(typeof(RoleOrderStatus), item.DisplayOrderNumber).ToString(), "ResModuleName");
                }

                objDTO.SelectedModuleIDs = "";
                objDTO.SelectedNonModuleIDs = "";
                objDTO.SelectedRoomReplanishmentValue = "";

                RoleWiseRoomsAccessDetailsDTO objRoomAccessDTO = new RoleWiseRoomsAccessDetailsDTO();
                objRoomAccessDTO.PermissionList = objList.ToList();
                objRoomAccessDTO.ModuleMasterList = objMasterList.ToList();
                objRoomAccessDTO.OtherModuleList = objOtherModuleList.ToList();
                objRoomAccessDTO.NonModuleList = objNonModuleList.ToList();
                objRoomAccessDTO.OtherDefaultSettings = objOtherDefaultSettings.ToList();
                objRoomAccessDTO.EnterpriseID = 0;
                objRoomAccessDTO.CompanyID = 0;
                objRoomAccessDTO.RoomID = 0;
                objRoomAccessDTO.RoleID = 0;
                objDTO.RoleWiseRoomsAccessDetails = null;
                objDTO.RoleRoomsAccessDetail = objRoomAccessDTO;

                List<RoomDTO> objRoomList = new List<RoomDTO>();
                RoomDTO odjRoomDTO = new RoomDTO();
                odjRoomDTO.ID = 0;
                odjRoomDTO.RoomName = "";
                objRoomList.Insert(0, odjRoomDTO);
                ViewBag.RoomsList = objRoomList.OrderBy(e => e.ID);

                Session["SelectedRoomsPermission"] = null;

                return PartialView("_CreateRole", objDTO);
            }
            else
            {
                RoleMasterDTO objDTO = new RoleMasterDTO();
                objDTO.ID = 0;
                objDTO.Created = DateTimeUtility.DateTimeNow;
                objDTO.Updated = DateTimeUtility.DateTimeNow;
                objDTO.CreatedBy = SessionHelper.UserID;
                objDTO.LastUpdatedBy = SessionHelper.UserID;
                objDTO.Room = SessionHelper.RoomID;
                objDTO.CompanyID = SessionHelper.CompanyID;
                objDTO.RoomName = SessionHelper.RoomName;
                objDTO.GUID = Guid.NewGuid();

                //RoleMasterController obj = new RoleMasterController();
                eTurns.DAL.RoleMasterDAL obj = new eTurns.DAL.RoleMasterDAL(SessionHelper.EnterPriseDBName);
                var objList = obj.GetRoleModuleDetailsRecord(0, 0);

                var objMasterList = (from t in objList
                                     where t.GroupId.ToString() == "1" && t.IsModule == true
                                     select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);
                var objOtherModuleList = (from t in objList
                                          where t.GroupId.ToString() == "2" && t.IsModule == true
                                          select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                var objNonModuleList = (from t in objList
                                        where t.IsModule == false && t.GroupId.ToString() != "4"
                                        select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                var objOtherDefaultSettings = (from t in objList
                                               where t.GroupId.ToString() == "4" && t.IsModule == false
                                               select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);
                //var objMasterList = from t in objList
                //                    where t.GroupId.ToString() == "1" && t.IsModule == true
                //                    select t;
                //var objOtherModuleList = from t in objList
                //                         where t.GroupId.ToString() == "2" && t.IsModule == true
                //                         select t;

                //var objNonModuleList = from t in objList
                //                       where t.IsModule == false && t.GroupId.ToString() != "4"
                //                       select t;

                //var objOtherDefaultSettings = from t in objList
                //                              where t.GroupId.ToString() == "4" && t.IsModule == false
                //                              select t;
                //objDTO.ModuleMasterList = objMasterList.ToList();
                //objDTO.OtherModuleList = objOtherModuleList.ToList();
                //objDTO.NonModuleList = objNonModuleList.ToList();

                foreach (var item in objNonModuleList)
                {
                    item.DisplayOrderName = ResourceHelper.GetResourceValue(Enum.GetName(typeof(RoleOrderStatus), item.DisplayOrderNumber).ToString(), "ResModuleName");
                }
                objDTO.SelectedModuleIDs = "";
                objDTO.SelectedNonModuleIDs = "";
                objDTO.SelectedRoomReplanishmentValue = "";

                RoleWiseRoomsAccessDetailsDTO objRoomAccessDTO = new RoleWiseRoomsAccessDetailsDTO();
                objRoomAccessDTO.PermissionList = objList.ToList();
                objRoomAccessDTO.ModuleMasterList = objMasterList.ToList();
                objRoomAccessDTO.OtherModuleList = objOtherModuleList.ToList();
                objRoomAccessDTO.NonModuleList = objNonModuleList.ToList();
                objRoomAccessDTO.OtherDefaultSettings = objOtherDefaultSettings.ToList();

                objRoomAccessDTO.RoleID = 0;
                objRoomAccessDTO.RoomID = 0;

                objDTO.RoleWiseRoomsAccessDetails = null;
                objDTO.RoleRoomsAccessDetail = objRoomAccessDTO;

                List<RoomDTO> objRoomList = new List<RoomDTO>();
                RoomDTO odjRoomDTO = new RoomDTO();
                odjRoomDTO.ID = 0;
                odjRoomDTO.RoomName = "";
                objRoomList.Insert(0, odjRoomDTO);
                ViewBag.RoomsList = objRoomList.OrderBy(e => e.ID);

                Session["SelectedRoomsPermission"] = null;

                return PartialView("_CreateRole", objDTO);
            }

        }

        [HttpPost]
        public JsonResult RoleMasterSave(RoleMasterDTO objDTO)
        {
            List<UserAccessDTO> objAccessList = new List<UserAccessDTO>();
            UserAccessDTO objUserAccessDTO = null;
            long enterId = 0, CompId = 0, RmId = 0;
            string RoleID = string.Empty;
            RoleID = Convert.ToString(objDTO.ID);
            //string RoomID = string.Empty;
            //string EnterpriseID = string.Empty;
            //string CompanyID = string.Empty;
            if (!string.IsNullOrWhiteSpace(objDTO.SelectedRoomAccessValue))
            {
                string[] arrstr = objDTO.SelectedRoomAccessValue.Split(sepForRoleRoom, StringSplitOptions.RemoveEmptyEntries);
                foreach (string cdid in arrstr)
                {
                    if (!string.IsNullOrWhiteSpace(cdid))
                    {
                        string[] arrids = cdid.Split('_');
                        if (long.TryParse(arrids[0], out enterId) && long.TryParse(arrids[1], out CompId) && long.TryParse(arrids[2], out RmId))
                        {
                            objUserAccessDTO = new UserAccessDTO();
                            objUserAccessDTO.EnterpriseId = enterId;
                            objUserAccessDTO.CompanyId = CompId;
                            objUserAccessDTO.RoomId = RmId;

                            //RoomID += RmId + ",";
                            //CompanyID += CompId + ",";
                            //EnterpriseID += enterId + ",";
                            objAccessList.Add(objUserAccessDTO);
                        }
                    }
                }


                //RoomID = RoomID.TrimEnd(',');
                //CompanyID = CompanyID.TrimEnd(',');
                //EnterpriseID = EnterpriseID.TrimEnd(',');
            }

            if (!string.IsNullOrWhiteSpace(objDTO.SelectedCompanyAccessValue))
            {
                string[] arrstr = objDTO.SelectedCompanyAccessValue.Split(sepForRoleRoom, StringSplitOptions.RemoveEmptyEntries);
                foreach (string cdid in arrstr)
                {
                    if (!string.IsNullOrWhiteSpace(cdid))
                    {
                        string[] arrids = cdid.Split('_');
                        if (long.TryParse(arrids[0], out enterId) && long.TryParse(arrids[1], out CompId))
                        {
                            if (!objAccessList.Any(t => t.EnterpriseId == enterId && t.CompanyId == CompId))
                            {
                                objUserAccessDTO = new UserAccessDTO();
                                objUserAccessDTO.EnterpriseId = enterId;
                                objUserAccessDTO.CompanyId = CompId;
                                objAccessList.Add(objUserAccessDTO);
                            }
                        }
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(objDTO.SelectedEnterpriseAccessValue))
            {
                string[] arrstr = objDTO.SelectedEnterpriseAccessValue.Split(sepForRoleRoom, StringSplitOptions.RemoveEmptyEntries);
                foreach (string epid in arrstr)
                {
                    if (long.TryParse(epid, out enterId))
                    {
                        if (!objAccessList.Any(t => t.EnterpriseId == enterId))
                        {
                            objUserAccessDTO = new UserAccessDTO();
                            objUserAccessDTO.EnterpriseId = enterId;
                            objAccessList.Add(objUserAccessDTO);
                        }
                    }
                }
            }

            objDTO.lstAccess = objAccessList;
            string message = "";
            string status = "";
            eTurnsMaster.DAL.RoleMasterDAL objRoleMasterDAL = new eTurnsMaster.DAL.RoleMasterDAL(SessionHelper.EnterPriseDBName);
            CommonMasterDAL objCDAL = new CommonMasterDAL();

            if (string.IsNullOrEmpty(objDTO.RoleName))
            {
                message = string.Format(ResMessage.Required, ResRoleMaster.RoleName);// "Role name is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;

            if (Session["SelectedRoomsPermission"] != null)
            {
                List<RoleWiseRoomsAccessDetailsDTO> objRoomsAccessList = (List<RoleWiseRoomsAccessDetailsDTO>)Session["SelectedRoomsPermission"];
                if (objRoomsAccessList != null && objRoomsAccessList.Count > 0)
                    objDTO.RoleWiseRoomsAccessDetails = objRoomsAccessList;
            }
            else
            {
                message = string.Format(ResMessage.Required, "Room Access"); //"Room Access is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }

            if (objDTO.ID == 0)
            {

                objDTO.CreatedBy = SessionHelper.UserID;
                if (objDTO.UserType == 1)
                {
                    objDTO.EnterpriseId = 0;
                    objDTO.CompanyID = 0;
                }
                else
                {
                    objDTO.EnterpriseId = SessionHelper.EnterPriceID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                }
                string strOK = objCDAL.RoleDuplicateCheck(objDTO.ID, objDTO.RoleName, objDTO.UserType, SessionHelper.EnterPriceID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResRoleMaster.RoleName, objDTO.RoleName);  //"Role Name \"" + objDTO.RoleName + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.IsActive = true;
                    long ReturnVal = objRoleMasterDAL.Insert(objDTO);
                    objDTO.ID = ReturnVal;
                    if (objDTO.UserType > 1)
                    {
                        eTurns.DAL.RoleMasterDAL objRolecMasterDAL = new eTurns.DAL.RoleMasterDAL(SessionHelper.EnterPriseDBName);
                        objRolecMasterDAL.Insert(objDTO);


                    }
                    if (ReturnVal > 0)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }
            else
            {
                objDTO.LastUpdatedBy = SessionHelper.UserID;
                string strOK = objCDAL.RoleDuplicateCheck(objDTO.ID, objDTO.RoleName, objDTO.UserType, SessionHelper.EnterPriceID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResRoleMaster.RoleName, objDTO.RoleName);  //"Role Name \"" + objDTO.RoleName + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    objDTO.IsActive = true;
                    bool ReturnVal = objRoleMasterDAL.Edit(objDTO);
                    if (objDTO.UserType > 1)
                    {
                        eTurns.DAL.RoleMasterDAL objRolecMasterDAL = new eTurns.DAL.RoleMasterDAL(SessionHelper.EnterPriseDBName);
                        objRolecMasterDAL.Edit(objDTO);
                        eTurns.DAL.UserMasterDAL objUserMasterDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                        objUserMasterDAL.DeleteRoomFromUseAccess(RoleID);
                    }
                    if (ReturnVal)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                        Session["SelectedRoomsPermission"] = null;
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }

            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public JsonResult RoleSave(RoleMasterDTO objDTO)
        {
            List<UserAccessDTO> objAccessList = new List<UserAccessDTO>();
            UserAccessDTO objUserAccessDTO = null;
            long enterId = 0, CompId = 0, RmId = 0;
            if (!string.IsNullOrWhiteSpace(objDTO.SelectedRoomAccessValue))
            {
                string[] arrstr = objDTO.SelectedRoomAccessValue.Split(sepForRoleRoom, StringSplitOptions.RemoveEmptyEntries);
                foreach (string cdid in arrstr)
                {
                    if (!string.IsNullOrWhiteSpace(cdid))
                    {
                        string[] arrids = cdid.Split('_');
                        if (long.TryParse(arrids[0], out enterId) && long.TryParse(arrids[1], out CompId) && long.TryParse(arrids[2], out RmId))
                        {
                            objUserAccessDTO = new UserAccessDTO();
                            objUserAccessDTO.EnterpriseId = enterId;
                            objUserAccessDTO.CompanyId = CompId;
                            objUserAccessDTO.RoomId = RmId;
                            objAccessList.Add(objUserAccessDTO);
                        }
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(objDTO.SelectedCompanyAccessValue))
            {
                string[] arrstr = objDTO.SelectedCompanyAccessValue.Split(sepForRoleRoom, StringSplitOptions.RemoveEmptyEntries);
                foreach (string cdid in arrstr)
                {
                    if (!string.IsNullOrWhiteSpace(cdid))
                    {
                        string[] arrids = cdid.Split('_');
                        if (long.TryParse(arrids[0], out enterId) && long.TryParse(arrids[1], out CompId))
                        {
                            if (!objAccessList.Any(t => t.EnterpriseId == enterId && t.CompanyId == CompId))
                            {
                                objUserAccessDTO = new UserAccessDTO();
                                objUserAccessDTO.EnterpriseId = enterId;
                                objUserAccessDTO.CompanyId = CompId;
                                objAccessList.Add(objUserAccessDTO);
                            }
                        }
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(objDTO.SelectedEnterpriseAccessValue))
            {
                string[] arrstr = objDTO.SelectedEnterpriseAccessValue.Split(sepForRoleRoom, StringSplitOptions.RemoveEmptyEntries);
                foreach (string epid in arrstr)
                {
                    if (long.TryParse(epid, out enterId))
                    {
                        if (!objAccessList.Any(t => t.EnterpriseId == enterId))
                        {
                            objUserAccessDTO = new UserAccessDTO();
                            objUserAccessDTO.EnterpriseId = enterId;
                            objAccessList.Add(objUserAccessDTO);
                        }
                    }
                }
            }

            objDTO.lstAccess = objAccessList;



            if (objDTO.UserType == 1)
            {
                string message = "";
                string status = "";
                //RoleMasterController obj = new RoleMasterController();
                eTurnsMaster.DAL.RoleMasterDAL objRoleMasterDAL = new eTurnsMaster.DAL.RoleMasterDAL(SessionHelper.EnterPriseDBName);
                CommonMasterDAL objCDAL = new CommonMasterDAL();

                if (string.IsNullOrEmpty(objDTO.RoleName))
                {
                    message = string.Format(ResMessage.Required, ResRoleMaster.RoleName);// "Role name is required.";
                    status = "fail";
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                }
                objDTO.LastUpdatedBy = SessionHelper.UserID;
                objDTO.UpdatedByName = SessionHelper.UserName;
                objDTO.Room = SessionHelper.RoomID;

                //Set Room Replenishment

                //if (!string.IsNullOrEmpty(objDTO.SelectedRoomReplanishmentValue))
                //{
                //    List<RoleRoomReplanishmentDetailsDTO> objRoomRepIDs = new List<RoleRoomReplanishmentDetailsDTO>();

                //    string[] objRoomReplanishmentIDs = objDTO.SelectedRoomReplanishmentValue.Split(sepForRoleRoom, StringSplitOptions.RemoveEmptyEntries);
                //    for (int i = 0; i < objRoomReplanishmentIDs.Length; i++)
                //    {
                //        string[] arrSplittedIds = objRoomReplanishmentIDs[i].ToString().Split('_');
                //        RoleRoomReplanishmentDetailsDTO objRoleRoom = new RoleRoomReplanishmentDetailsDTO();
                //        objRoleRoom.RoomID = int.Parse(arrSplittedIds[2]);
                //        objRoleRoom.CompanyId = int.Parse(arrSplittedIds[1]);
                //        objRoleRoom.EnterpriseId = int.Parse(arrSplittedIds[0]);
                //        objRoomRepIDs.Add(objRoleRoom);
                //    }
                //    objDTO.ReplenishingRooms = objRoomRepIDs;
                //}
                // Set Permissions

                if (Session["SelectedRoomsPermission"] != null)
                {
                    List<RoleWiseRoomsAccessDetailsDTO> objRoomsAccessList = (List<RoleWiseRoomsAccessDetailsDTO>)Session["SelectedRoomsPermission"];
                    if (objRoomsAccessList != null && objRoomsAccessList.Count > 0)
                        objDTO.RoleWiseRoomsAccessDetails = objRoomsAccessList;
                }
                else
                {

                    message = string.Format(ResMessage.Required, "Room Access"); //"Room Access is required.";
                    status = "fail";
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                }

                if (objDTO.ID == 0)
                {
                    objDTO.CreatedBy = SessionHelper.UserID;
                    string strOK = objCDAL.RoleDuplicateCheck(objDTO.ID, objDTO.RoleName);
                    if (strOK == "duplicate")
                    {
                        message = string.Format(ResMessage.DuplicateMessage, ResRoleMaster.RoleName, objDTO.RoleName);  //"Role Name \"" + objDTO.RoleName + "\" already exist! Try with Another!";
                        status = "duplicate";
                    }
                    else
                    {
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.IsActive = true;
                        long ReturnVal = objRoleMasterDAL.Insert(objDTO);
                        if (ReturnVal > 0)
                        {
                            message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                            status = "ok";
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                            status = "fail";
                        }
                    }
                }
                else
                {
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    string strOK = objCDAL.RoleDuplicateCheck(objDTO.ID, objDTO.RoleName);
                    if (strOK == "duplicate")
                    {
                        message = string.Format(ResMessage.DuplicateMessage, ResRoleMaster.RoleName, objDTO.RoleName);  //"Role Name \"" + objDTO.RoleName + "\" already exist! Try with Another!";
                        status = "duplicate";
                    }
                    else
                    {
                        objDTO.IsActive = true;
                        bool ReturnVal = objRoleMasterDAL.Edit(objDTO);
                        if (ReturnVal)
                        {
                            message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                            status = "ok";
                            Session["SelectedRoomsPermission"] = null;
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                            status = "fail";
                        }
                    }
                }

                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string message = "";
                string status = "";
                //RoleMasterController obj = new RoleMasterController();
                eTurns.DAL.RoleMasterDAL obj = new eTurns.DAL.RoleMasterDAL(SessionHelper.EnterPriseDBName);
                CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

                if (string.IsNullOrEmpty(objDTO.RoleName))
                {
                    message = string.Format(ResMessage.Required, ResRoleMaster.RoleName);// "Role name is required.";
                    status = "fail";
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                }
                objDTO.LastUpdatedBy = SessionHelper.UserID;
                objDTO.UpdatedByName = SessionHelper.UserName;
                objDTO.Room = SessionHelper.RoomID;
                objDTO.EnterpriseId = SessionHelper.EnterPriceID;
                objDTO.CompanyID = SessionHelper.CompanyID;
                objDTO.Room = SessionHelper.RoomID;
                //Set Room Replenishment

                if (!string.IsNullOrEmpty(objDTO.SelectedRoomReplanishmentValue))
                {
                    List<RoleRoomReplanishmentDetailsDTO> objRoomRepIDs = new List<RoleRoomReplanishmentDetailsDTO>();

                    string[] objRoomReplanishmentIDs = objDTO.SelectedRoomReplanishmentValue.Split(sepForRoleRoom, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < objRoomReplanishmentIDs.Length; i++)
                    {
                        string[] arrSplittedIds = objRoomReplanishmentIDs[i].ToString().Split('_');
                        RoleRoomReplanishmentDetailsDTO objRoleRoom = new RoleRoomReplanishmentDetailsDTO();
                        objRoleRoom.RoomID = int.Parse(arrSplittedIds[2]);
                        objRoleRoom.CompanyId = int.Parse(arrSplittedIds[1]);
                        objRoleRoom.EnterpriseId = int.Parse(arrSplittedIds[0]);
                        objRoomRepIDs.Add(objRoleRoom);
                    }
                    objDTO.ReplenishingRooms = objRoomRepIDs;
                }
                // Set Permissions

                if (Session["SelectedRoomsPermission"] != null)
                {
                    List<RoleWiseRoomsAccessDetailsDTO> objRoomsAccessList = (List<RoleWiseRoomsAccessDetailsDTO>)Session["SelectedRoomsPermission"];
                    if (objRoomsAccessList != null && objRoomsAccessList.Count > 0)
                        objDTO.RoleWiseRoomsAccessDetails = objRoomsAccessList;
                }
                else
                {

                    message = string.Format(ResMessage.Required, "Room Access"); //"Room Access is required.";
                    status = "fail";
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                }

                if (objDTO.ID == 0)
                {
                    objDTO.CreatedBy = SessionHelper.UserID;
                    string strOK = objCDAL.RoleDuplicateCheck(objDTO.ID, objDTO.RoleName);
                    //string strOK = objCDAL.DuplicateCheck(objDTO.RoleName, "add", objDTO.ID, "RoleMaster", "RoleName", SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (strOK == "duplicate")
                    {
                        message = string.Format(ResMessage.DuplicateMessage, ResRoleMaster.RoleName, objDTO.RoleName);  //"Role Name \"" + objDTO.RoleName + "\" already exist! Try with Another!";
                        status = "duplicate";
                    }
                    else
                    {
                        objDTO.IsActive = true;
                        objDTO.GUID = Guid.NewGuid();
                        long ReturnVal = obj.Insert(objDTO);
                        if (ReturnVal > 0)
                        {
                            message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                            status = "ok";
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                            status = "fail";
                        }
                    }
                }
                else
                {
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    string strOK = objCDAL.RoleDuplicateCheck(objDTO.ID, objDTO.RoleName);
                    //string strOK = objCDAL.DuplicateCheck(objDTO.RoleName, "edit", objDTO.ID, "RoleMaster", "RoleName", SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (strOK == "duplicate")
                    {
                        message = string.Format(ResMessage.DuplicateMessage, ResRoleMaster.RoleName, objDTO.RoleName);  //"Role Name \"" + objDTO.RoleName + "\" already exist! Try with Another!";
                        status = "duplicate";
                    }
                    else
                    {
                        objDTO.IsActive = true;
                        bool ReturnVal = obj.Edit(objDTO);
                        if (ReturnVal)
                        {
                            message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                            status = "ok";
                            Session["SelectedRoomsPermission"] = null;
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                            status = "fail";
                        }
                    }
                }

                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }



        }

        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>

        [AjaxOrChildActionOnlyAttribute]
        public ActionResult RoleEdit(Int64 ID, long? UserType)
        {

            //RoleMasterController obj = new RoleMasterController();
            if ((UserType ?? 1) == 1)
            {
                eTurnsMaster.DAL.RoleMasterDAL obj = new eTurnsMaster.DAL.RoleMasterDAL(SessionHelper.EnterPriseDBName);
                RoleMasterDTO objDTO = obj.GetRecord(ID);
                if (objDTO != null)
                {
                    objDTO.lstAccess = new EnterpriseMasterDAL().GetRoleAccessWithNames(objDTO.ID);
                }
                var objMasterList = (from t in objDTO.PermissionList
                                     where t.GroupId.ToString() == "1" && t.IsModule == true
                                     select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);
                var objOtherModuleList = (from t in objDTO.PermissionList
                                          where t.GroupId.ToString() == "2" && t.IsModule == true
                                          select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                var objNonModuleList = (from t in objDTO.PermissionList
                                        where t.IsModule == false && t.GroupId.ToString() != "4"
                                        select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                var objOtherDefaultSettings = (from t in objDTO.PermissionList
                                               where t.GroupId.ToString() == "4" && t.IsModule == false
                                               select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                RoleWiseRoomsAccessDetailsDTO objRoomAccessDTO = new RoleWiseRoomsAccessDetailsDTO();
                objRoomAccessDTO.PermissionList = objDTO.PermissionList.ToList();
                objRoomAccessDTO.ModuleMasterList = objMasterList.ToList();
                objRoomAccessDTO.OtherModuleList = objOtherModuleList.ToList();
                objRoomAccessDTO.NonModuleList = objNonModuleList.ToList();
                objRoomAccessDTO.OtherDefaultSettings = objOtherDefaultSettings.ToList();

                objRoomAccessDTO.RoleID = 0;
                objRoomAccessDTO.RoomID = 0;

                objDTO.RoleRoomsAccessDetail = objRoomAccessDTO;


                if (objDTO.RoleWiseRoomsAccessDetails != null)
                {
                    Session["SelectedRoomsPermission"] = objDTO.RoleWiseRoomsAccessDetails;
                }

                foreach (var item in objRoomAccessDTO.NonModuleList)
                {
                    item.DisplayOrderName = ResourceHelper.GetResourceValue(Enum.GetName(typeof(RoleOrderStatus), item.DisplayOrderNumber).ToString(), "ResModuleName");
                }

                List<RoomDTO> objRoomList = new List<RoomDTO>();

                RoomDTO odjRoomDTO = new RoomDTO();
                odjRoomDTO.ID = 0;
                odjRoomDTO.RoomName = "";
                objRoomList.Insert(0, odjRoomDTO);
                ViewBag.RoomsList = objRoomList;

                objDTO.SelectedModuleIDs = "";
                objDTO.SelectedNonModuleIDs = "";
                if (objDTO.lstAccess != null && objDTO.lstAccess.Count > 0)
                {
                    objDTO.SelectedEnterpriseAccessValue = string.Join(sepForRoleRoom.First().ToString(), objDTO.lstAccess.Select(t => t.EnterpriseId).Distinct().ToArray());
                    objDTO.SelectedCompanyAccessValue = string.Join(sepForRoleRoom.First().ToString(), objDTO.lstAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId > 0).Select(t => t.EnterpriseId + "_" + t.CompanyId).Distinct().ToArray());
                    objDTO.SelectedRoomAccessValue = string.Join(sepForRoleRoom.First().ToString(), objDTO.lstAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId > 0 && t.RoomId > 0).Select(t => t.EnterpriseId + "_" + t.CompanyId + "_" + t.RoomId + "_" + t.RoomName).Distinct().ToArray());
                }
                //if (objDTO.PermissionList != null && objDTO.PermissionList.Count > 0)
                //{
                //    long[] arrteamp = objDTO.PermissionList.Where(t => t.RoomId > 0).ToList().Select(t => t.EnterpriseID).Distinct().ToArray();
                //    objDTO.SelectedEnterpriseAccessValue = string.Join(",", arrteamp);
                //    var qCompanies = (from itm in objDTO.PermissionList.Where(t => t.RoomId > 0)
                //                      group itm by new { itm.EnterpriseID, itm.CompanyID } into groupedEnterpriseCompanies
                //                      select new RolePermissionInfo
                //                      {
                //                          EnterPriseId = groupedEnterpriseCompanies.Key.EnterpriseID,
                //                          CompanyId = groupedEnterpriseCompanies.Key.CompanyID,
                //                          EnterPriseId_CompanyId = groupedEnterpriseCompanies.Key.EnterpriseID + "_" + groupedEnterpriseCompanies.Key.CompanyID
                //                      });
                //    string[] arrteampC = qCompanies.ToList().Select(t => t.EnterPriseId_CompanyId).Distinct().ToArray();
                //    objDTO.SelectedCompanyAccessValue = string.Join(",", arrteampC);
                //}
                //objDTO.SelectedRoomReplanishmentValue
                if (objDTO != null)
                {
                    objDTO.CreatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);

                    objDTO.UpdatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                }
                return PartialView("_CreateRole", objDTO);
            }
            else
            {
                eTurns.DAL.RoleMasterDAL obj = new eTurns.DAL.RoleMasterDAL(SessionHelper.EnterPriseDBName);
                RoleMasterDTO objDTO = obj.GetRecord(ID);

                //var objMasterList = from t in objDTO.PermissionList
                //                    where t.GroupId.ToString() == "1" && t.IsModule == true
                //                    select t;
                //var objOtherModuleList = from t in objDTO.PermissionList
                //                         where t.GroupId.ToString() == "2" && t.IsModule == true
                //                         select t;

                //var objNonModuleList = from t in objDTO.PermissionList
                //                       where t.IsModule == false && t.GroupId.ToString() != "4"
                //                       select t;

                //var objOtherDefaultSettings = from t in objDTO.PermissionList
                //                              where t.GroupId.ToString() == "4" && t.IsModule == false
                //                              select t;
                var objMasterList = (from t in objDTO.PermissionList
                                     where t.GroupId.ToString() == "1" && t.IsModule == true && t.ModuleID != 41
                                     select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);
                var objOtherModuleList = (from t in objDTO.PermissionList
                                          where t.GroupId.ToString() == "2" && t.IsModule == true
                                          select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                var objNonModuleList = (from t in objDTO.PermissionList
                                        where t.IsModule == false && t.GroupId.ToString() != "4"
                                        select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                var objOtherDefaultSettings = (from t in objDTO.PermissionList
                                               where t.GroupId.ToString() == "4" && t.IsModule == false
                                               select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                RoleWiseRoomsAccessDetailsDTO objRoomAccessDTO = new RoleWiseRoomsAccessDetailsDTO();
                objRoomAccessDTO.PermissionList = objDTO.PermissionList.ToList();
                objRoomAccessDTO.ModuleMasterList = objMasterList.ToList();
                objRoomAccessDTO.OtherModuleList = objOtherModuleList.ToList();
                objRoomAccessDTO.NonModuleList = objNonModuleList.ToList();
                objRoomAccessDTO.OtherDefaultSettings = objOtherDefaultSettings.ToList();

                objRoomAccessDTO.RoleID = 0;
                objRoomAccessDTO.RoomID = 0;

                objDTO.RoleRoomsAccessDetail = objRoomAccessDTO;


                foreach (var item in objRoomAccessDTO.NonModuleList)
                {
                    item.DisplayOrderName = ResourceHelper.GetResourceValue(Enum.GetName(typeof(RoleOrderStatus), item.DisplayOrderNumber).ToString(), "ResModuleName");
                }

                if (objDTO.RoleWiseRoomsAccessDetails != null)
                {
                    Session["SelectedRoomsPermission"] = objDTO.RoleWiseRoomsAccessDetails;
                }

                List<RoomDTO> objRoomList = new List<RoomDTO>();

                RoomDTO odjRoomDTO = new RoomDTO();
                odjRoomDTO.ID = 0;
                odjRoomDTO.RoomName = "";
                objRoomList.Insert(0, odjRoomDTO);
                ViewBag.RoomsList = objRoomList;

                objDTO.SelectedModuleIDs = "";
                objDTO.SelectedNonModuleIDs = "";
                if (objDTO.lstAccess != null && objDTO.lstAccess.Count > 0)
                {
                    objDTO.SelectedEnterpriseAccessValue = string.Join(sepForRoleRoom.First().ToString(), objDTO.lstAccess.Select(t => t.EnterpriseId).Distinct().ToArray());
                    objDTO.SelectedCompanyAccessValue = string.Join(sepForRoleRoom.First().ToString(), objDTO.lstAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId > 0).Select(t => t.EnterpriseId + "_" + t.CompanyId).Distinct().ToArray());
                    objDTO.SelectedRoomAccessValue = string.Join(sepForRoleRoom.First().ToString(), objDTO.lstAccess.Where(t => t.EnterpriseId > 0 && t.CompanyId > 0 && t.RoomId > 0).Select(t => t.EnterpriseId + "_" + t.CompanyId + "_" + t.RoomId + "_" + t.RoomName).Distinct().ToArray());
                }
                if (objDTO != null)
                {
                    objDTO.CreatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);

                    objDTO.UpdatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                }
                return PartialView("_CreateRole", objDTO);
            }

        }

        #region Ajax Data Provider

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult RoleListAjax(JQueryDataTableParamModel param)
        {
            //RoleMasterController obj = new RoleMasterController();
            UserBAL obj = new UserBAL();

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
            if (sortColumnName == "0" || sortColumnName == "undefined" || string.IsNullOrEmpty(sortColumnName))
                sortColumnName = "RoleName Asc";

            //if (sortDirection == "asc")
            //    sortColumnName = sortColumnName + " asc";
            //else
            //    sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            var DataFromDB = obj.GetPagedRolesBySQlHelper(SessionHelper.UserType, SessionHelper.EnterPriceID, SessionHelper.RoomID, SessionHelper.CompanyID, param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, IsArchived, IsDeleted, SessionHelper.UserID, SessionHelper.RoleID);
            //var DataFromDB = obj.GetPagedRecords(1, param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID);

            var result = from c in DataFromDB
                         select new RoleMasterDTO
                         {
                             ID = c.ID,
                             RoleName = c.RoleName,
                             UserType = c.UserType,
                             UserTypeName = c.UserTypeName,
                             Created = c.Created,
                             Updated = c.Updated,
                             IsDeleted = c.IsDeleted,
                             IsArchived = c.IsArchived,
                             UpdatedByName = c.UpdatedByName,
                             CreatedByName = c.CreatedByName,
                             Room = c.Room,
                             CompanyID = c.CompanyID,
                             EnterpriseId = c.EnterpriseId,
                             CreatedDate = CommonUtility.ConvertDateByTimeZone(c.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             UpdatedDate = CommonUtility.ConvertDateByTimeZone(c.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
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

        public string UpdateRoleData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            //RoleMasterController obj = new RoleMasterController();
            //obj.PutUpdateData(id, value, rowId, columnPosition, columnId, columnName);
            //return value;
            return null;
        }

        public string DuplicateRoleCheck(string RoleName, string ActionMode, int ID)
        {
            //RoleMasterController obj = new RoleMasterController();
            //return obj.DuplicateCheck(RoleName, ActionMode, ID);
            return null;
        }

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public ActionResult DeleteRoleRecords(string ids)
        {
            List<RoleMasterDTO> lstDeleteRequested = new List<RoleMasterDTO>();
            RoleMasterDTO objRoleMasterDTO;
            long RoleID = 0, EnterpriseId = 0, CompanyID = 0, RoomId = 0;
            int UserType = 0;
            try
            {
                if (!string.IsNullOrWhiteSpace(ids))
                {
                    string[] arrIds = new string[] { };
                    arrIds = ids.Split(',');
                    foreach (var item in arrIds)
                    {
                        if (!string.IsNullOrWhiteSpace(item))
                        {
                            objRoleMasterDTO = new RoleMasterDTO();
                            string[] arrSeperated = item.Split('_');
                            long.TryParse(arrSeperated[0], out RoleID);
                            objRoleMasterDTO.ID = RoleID;
                            int.TryParse(arrSeperated[1], out UserType);
                            objRoleMasterDTO.UserType = UserType;
                            long.TryParse(arrSeperated[2], out EnterpriseId);
                            objRoleMasterDTO.EnterpriseId = EnterpriseId;
                            long.TryParse(arrSeperated[3], out CompanyID);
                            objRoleMasterDTO.CompanyID = CompanyID;
                            long.TryParse(arrSeperated[4], out RoomId);
                            objRoleMasterDTO.Room = RoomId;
                            lstDeleteRequested.Add(objRoleMasterDTO);
                        }
                    }
                }
                List<RoleMasterDTO> lstSuperAdminRoles = new List<RoleMasterDTO>();
                List<RoleMasterDTO> lstLocalAdminRoles = new List<RoleMasterDTO>();
                if (lstDeleteRequested != null && lstDeleteRequested.Count > 0)
                {
                    lstSuperAdminRoles = lstDeleteRequested.Where(t => t.UserType == 1).ToList();
                    lstLocalAdminRoles = lstDeleteRequested.Where(t => t.UserType > 1).ToList();
                }
                eTurnsMaster.DAL.RoleMasterDAL objRoleMasterDAL = new eTurnsMaster.DAL.RoleMasterDAL(SessionHelper.EnterPriseDBName);
                eTurns.DAL.RoleMasterDAL objLocalRoleMasterDAL = new eTurns.DAL.RoleMasterDAL(SessionHelper.EnterPriseDBName);

                List<RoleMasterDTO> objMasterReturnResult = objRoleMasterDAL.ValidateBeforeDeleteRoles(lstSuperAdminRoles);
                List<RoleMasterDTO> objLocalReturnResult = objLocalRoleMasterDAL.ValidateBeforeDeleteRoles(lstLocalAdminRoles);
                int RecordscanbeDeleted = 0;
                int RecordsInUse = 0;
                RecordscanbeDeleted += lstDeleteRequested.Count(t => t.CanBeDeleted == true);
                RecordsInUse += objMasterReturnResult.Count(t => t.CanBeDeleted == false);
                if (objMasterReturnResult.Any(t => t.CanBeDeleted == true))
                {
                    objRoleMasterDAL.DeleteRoles(objMasterReturnResult.Where(t => t.CanBeDeleted == true).ToList());
                }
                string id1s = string.Empty;
                RecordscanbeDeleted += objLocalReturnResult.Count(t => t.CanBeDeleted == true);
                RecordsInUse += objLocalReturnResult.Count(t => t.CanBeDeleted == false);
                if (objLocalReturnResult.Any(t => t.CanBeDeleted == true))
                {
                    ids = string.Join(",", objLocalReturnResult.Where(t => t.CanBeDeleted == true).Select(t => t.ID).ToArray());
                    objLocalRoleMasterDAL.DeleteRecords(ids, SessionHelper.UserID);
                }

                //lstDeleteRequested = objRoleMasterDAL.DeleteRoles(lstDeleteRequested);
                //int InUse = lstDeleteRequested.Count(t => t.ActionExecuited == false);
                //int ToBeDeleted = lstDeleteRequested.Count(t => t.ActionExecuited == true);
                //RoleMasterController obj = new RoleMasterController();
                //obj.DeleteRecords(ids, 1);

                return Json(new { Message = RecordscanbeDeleted + " Records deleted SuccessFully." + RecordsInUse + " Records in Use.", Status = "ok", ActionExecuitResult = lstDeleteRequested }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Message = "Error", Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region "Role permission details"
        public PartialViewResult _CreateRolePermission()
        {
            return PartialView();
        }

        //public JsonResult SaveToRolePermissionsToSession(string RoomID, string RoleID, string SelectedModuleList, string SelectedNonModuleList, string SelectedDefaultSettings)
        //{
        //    if (!string.IsNullOrEmpty(RoomID) && !string.IsNullOrEmpty(RoleID))
        //    {
        //        string EnterpriseID = "";
        //        string CompanyID = "";
        //        string[] ecr = RoomID.Split('_');
        //        if (ecr.Length > 1)
        //        {
        //            EnterpriseID = ecr[0];
        //            CompanyID = ecr[1];
        //            RoomID = ecr[2];
        //        }

        //        List<RoleWiseRoomsAccessDetailsDTO> objRoomsAccessList = new List<RoleWiseRoomsAccessDetailsDTO>();
        //        List<RoleModuleDetailsDTO> objPermissionList = new List<RoleModuleDetailsDTO>();
        //        List<int> objPIDs = new List<int>();
        //        bool AddToList = false;

        //        if (!string.IsNullOrEmpty(SelectedModuleList))
        //        {
        //            string[] objIDs = SelectedModuleList.Split(',');
        //            for (int i = 0; i < objIDs.Length; i++)
        //            {
        //                string[] name = objIDs[i].Split('_');
        //                if (name.Length > 1)
        //                {
        //                    RoleModuleDetailsDTO objRDTO = new RoleModuleDetailsDTO();

        //                    objRDTO = objPermissionList.Find(element => element.ModuleID == int.Parse(name[0]));
        //                    if (objRDTO != null)
        //                    {
        //                        if (name[1] == "view")
        //                            objRDTO.IsView = true;

        //                        if (name[1] == "insert")
        //                            objRDTO.IsInsert = true;

        //                        if (name[1] == "update")
        //                            objRDTO.IsUpdate = true;

        //                        if (name[1] == "delete")
        //                            objRDTO.IsDelete = true;

        //                        if (name[1] == "ischecked")
        //                            objRDTO.IsChecked = true;

        //                        if (name[1] == "showdeleted")
        //                            objRDTO.ShowDeleted = true;
        //                        if (name[1] == "showarchived")
        //                            objRDTO.ShowArchived = true;
        //                        if (name[1] == "showudf")
        //                            objRDTO.ShowUDF = true;
        //                    }
        //                    else
        //                    {
        //                        objRDTO = new RoleModuleDetailsDTO();
        //                        objRDTO.ModuleID = int.Parse(name[0]);
        //                        objRDTO.RoomId = int.Parse(RoomID);

        //                        if (name[1] == "view")
        //                            objRDTO.IsView = true;

        //                        if (name[1] == "insert")
        //                            objRDTO.IsInsert = true;

        //                        if (name[1] == "update")
        //                            objRDTO.IsUpdate = true;

        //                        if (name[1] == "delete")
        //                            objRDTO.IsDelete = true;

        //                        if (name[1] == "ischecked")
        //                            objRDTO.IsChecked = true;

        //                        if (name[1] == "showdeleted")
        //                            objRDTO.ShowDeleted = true;
        //                        if (name[1] == "showarchived")
        //                            objRDTO.ShowArchived = true;
        //                        if (name[1] == "showudf")
        //                            objRDTO.ShowUDF = true;


        //                        objPermissionList.Add(objRDTO);
        //                    }
        //                }
        //            }
        //            AddToList = true;
        //        }
        //        if (!string.IsNullOrEmpty(SelectedNonModuleList))
        //        {
        //            string[] objIDs = SelectedNonModuleList.Split(',');
        //            for (int i = 0; i < objIDs.Length; i++)
        //            {
        //                string[] name = objIDs[i].Split('_');
        //                if (name.Length > 1)
        //                {
        //                    RoleModuleDetailsDTO objRDTO = new RoleModuleDetailsDTO();
        //                    objRDTO.ModuleID = int.Parse(name[0]);
        //                    objRDTO.RoomId = SessionHelper.RoomID;

        //                    if (name[1] == "ischecked")
        //                        objRDTO.IsChecked = true;

        //                    objPermissionList.Add(objRDTO);
        //                }
        //            }
        //            AddToList = true;
        //        }

        //        if (!string.IsNullOrEmpty(SelectedDefaultSettings))
        //        {

        //            string[] objIDs = SelectedDefaultSettings.Split(',');
        //            for (int i = 0; i < objIDs.Length; i++)
        //            {
        //                string[] name = objIDs[i].Split('#');
        //                if (name.Length > 1)
        //                {
        //                    RoleModuleDetailsDTO objRDTO = new RoleModuleDetailsDTO();

        //                    objRDTO = objPermissionList.Find(element => element.ModuleID == int.Parse(name[0]));
        //                    if (objRDTO != null)
        //                        objRDTO.ModuleValue = name[1];
        //                    else
        //                    {
        //                        objRDTO = new RoleModuleDetailsDTO();
        //                        objRDTO.ModuleID = int.Parse(name[0]);
        //                        objRDTO.RoomId = int.Parse(RoomID);
        //                        objRDTO.ModuleValue = name[1];
        //                        objPermissionList.Add(objRDTO);
        //                    }
        //                }
        //            }
        //            AddToList = true;
        //        }
        //        if (AddToList == true)
        //        {
        //            RoleWiseRoomsAccessDetailsDTO objRoomAccessDTO = new RoleWiseRoomsAccessDetailsDTO();
        //            objRoomAccessDTO.PermissionList = objPermissionList.ToList();
        //            objRoomAccessDTO.RoleID = int.Parse(RoleID);
        //            objRoomAccessDTO.RoomID = int.Parse(RoomID);

        //            if (Session["SelectedRoomsPermission"] != null)
        //            {
        //                objRoomsAccessList = (List<RoleWiseRoomsAccessDetailsDTO>)Session["SelectedRoomsPermission"];
        //                RoleWiseRoomsAccessDetailsDTO exsisting = objRoomsAccessList.Find(element => element.RoomID == int.Parse(RoomID));
        //                if (exsisting != null)
        //                    exsisting.PermissionList = objPermissionList.ToList();
        //                else
        //                    objRoomsAccessList.Add(objRoomAccessDTO);
        //            }
        //            else
        //            {
        //                objRoomsAccessList = new List<RoleWiseRoomsAccessDetailsDTO>();
        //                objRoomsAccessList.Add(objRoomAccessDTO);
        //            }
        //            Session["SelectedRoomsPermission"] = objRoomsAccessList;
        //        }
        //    }
        //    string Message = "success";
        //    string Status = "ok";

        //    return Json(new { Message = Message, Status = Status }, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult SaveToRolePermissionsToSession(string RoomID, string RoleID, string SelectedModuleList, string SelectedNonModuleList, string SelectedDefaultSettings)
        {
            if (!string.IsNullOrEmpty(RoomID) && !string.IsNullOrEmpty(RoleID))
            {
                string EnterpriseID = "";
                string CompanyID = "";
                string[] ecr = RoomID.Split('_');
                if (ecr.Length > 1)
                {
                    EnterpriseID = ecr[0];
                    CompanyID = ecr[1];
                    RoomID = ecr[2];
                }
                List<RoleWiseRoomsAccessDetailsDTO> objRoomsAccessList = new List<RoleWiseRoomsAccessDetailsDTO>();
                List<RoleModuleDetailsDTO> objPermissionList = new List<RoleModuleDetailsDTO>();
                List<int> objPIDs = new List<int>();
                bool AddToList = false;

                if (!string.IsNullOrEmpty(SelectedModuleList))
                {
                    string[] objIDs = SelectedModuleList.Split(',');
                    for (int i = 0; i < objIDs.Length; i++)
                    {
                        string[] name = objIDs[i].Split('_');
                        if (name.Length > 1)
                        {
                            RoleModuleDetailsDTO objRDTO = new RoleModuleDetailsDTO();

                            objRDTO = objPermissionList.Find(element => element.ModuleID == int.Parse(name[0]) && element.CompanyID == Int64.Parse(CompanyID) && element.EnterpriseID == Int64.Parse(EnterpriseID));
                            if (objRDTO != null)
                            {
                                if (name[1] == "view")
                                    objRDTO.IsView = true;

                                if (name[1] == "insert")
                                    objRDTO.IsInsert = true;

                                if (name[1] == "update")
                                    objRDTO.IsUpdate = true;

                                if (name[1] == "delete")
                                    objRDTO.IsDelete = true;

                                if (name[1] == "ischecked")
                                    objRDTO.IsChecked = true;

                                if (name[1] == "showdeleted")
                                    objRDTO.ShowDeleted = true;
                                if (name[1] == "showarchived")
                                    objRDTO.ShowArchived = true;
                                if (name[1] == "showudf")
                                    objRDTO.ShowUDF = true;
                            }
                            else
                            {
                                objRDTO = new RoleModuleDetailsDTO();
                                objRDTO.ModuleID = int.Parse(name[0]);
                                objRDTO.RoomId = int.Parse(RoomID);
                                objRDTO.CompanyID = Int64.Parse(CompanyID);
                                objRDTO.EnterpriseID = Int64.Parse(EnterpriseID);

                                if (name[1] == "view")
                                    objRDTO.IsView = true;

                                if (name[1] == "insert")
                                    objRDTO.IsInsert = true;

                                if (name[1] == "update")
                                    objRDTO.IsUpdate = true;

                                if (name[1] == "delete")
                                    objRDTO.IsDelete = true;

                                if (name[1] == "ischecked")
                                    objRDTO.IsChecked = true;

                                if (name[1] == "showdeleted")
                                    objRDTO.ShowDeleted = true;
                                if (name[1] == "showarchived")
                                    objRDTO.ShowArchived = true;
                                if (name[1] == "showudf")
                                    objRDTO.ShowUDF = true;


                                objPermissionList.Add(objRDTO);
                            }
                        }
                    }
                    AddToList = true;
                }
                if (!string.IsNullOrEmpty(SelectedNonModuleList))
                {
                    string[] objIDs = SelectedNonModuleList.Split(',');
                    for (int i = 0; i < objIDs.Length; i++)
                    {
                        string[] name = objIDs[i].Split('_');
                        if (name.Length > 1)
                        {
                            RoleModuleDetailsDTO objRDTO = new RoleModuleDetailsDTO();
                            objRDTO.ModuleID = int.Parse(name[0]);
                            objRDTO.RoomId = SessionHelper.RoomID;

                            objRDTO.CompanyID = Int64.Parse(CompanyID);
                            objRDTO.EnterpriseID = Int64.Parse(EnterpriseID);

                            if (name[1] == "ischecked")
                                objRDTO.IsChecked = true;

                            objPermissionList.Add(objRDTO);
                        }
                    }
                    AddToList = true;
                }

                if (!string.IsNullOrEmpty(SelectedDefaultSettings))
                {

                    string[] objIDs = SelectedDefaultSettings.Split(',');
                    for (int i = 0; i < objIDs.Length; i++)
                    {
                        string[] name = objIDs[i].Split('#');
                        if (name.Length > 1)
                        {
                            RoleModuleDetailsDTO objRDTO = new RoleModuleDetailsDTO();

                            //objRDTO = objPermissionList.Find(element => element.ModuleID == int.Parse(name[0]));
                            objRDTO = objPermissionList.Find(element => element.ModuleID == int.Parse(name[0]) && element.CompanyID == Int64.Parse(CompanyID) && element.EnterpriseID == Int64.Parse(EnterpriseID));
                            if (objRDTO != null)
                                objRDTO.ModuleValue = name[1];
                            else
                            {
                                objRDTO = new RoleModuleDetailsDTO();
                                objRDTO.ModuleID = int.Parse(name[0]);
                                objRDTO.RoomId = int.Parse(RoomID);
                                objRDTO.ModuleValue = name[1];
                                objRDTO.CompanyID = Int64.Parse(CompanyID);
                                objRDTO.EnterpriseID = Int64.Parse(EnterpriseID);
                                objPermissionList.Add(objRDTO);
                            }
                        }
                    }
                    AddToList = true;
                }
                if (AddToList == true)
                {
                    RoleWiseRoomsAccessDetailsDTO objRoomAccessDTO = new RoleWiseRoomsAccessDetailsDTO();
                    objRoomAccessDTO.PermissionList = objPermissionList.ToList();
                    objRoomAccessDTO.RoleID = int.Parse(RoleID);
                    objRoomAccessDTO.RoomID = int.Parse(RoomID);
                    objRoomAccessDTO.CompanyID = Int64.Parse(CompanyID);
                    objRoomAccessDTO.EnterpriseID = Int64.Parse(EnterpriseID);

                    if (Session["SelectedRoomsPermission"] != null)
                    {
                        objRoomsAccessList = (List<RoleWiseRoomsAccessDetailsDTO>)Session["SelectedRoomsPermission"];
                        //RoleWiseRoomsAccessDetailsDTO exsisting = objRoomsAccessList.Find(element => element.RoomID == int.Parse(RoomID));
                        RoleWiseRoomsAccessDetailsDTO exsisting = objRoomsAccessList.Find(element => element.RoomID == int.Parse(RoomID) && element.CompanyID == Int64.Parse(CompanyID) && element.EnterpriseID == Int64.Parse(EnterpriseID));
                        if (exsisting != null)
                            exsisting.PermissionList = objPermissionList.ToList();
                        else
                            objRoomsAccessList.Add(objRoomAccessDTO);
                    }
                    else
                    {
                        objRoomsAccessList = new List<RoleWiseRoomsAccessDetailsDTO>();
                        objRoomsAccessList.Add(objRoomAccessDTO);
                    }
                    Session["SelectedRoomsPermission"] = objRoomsAccessList;
                }
            }
            string Message = "success";
            string Status = "ok";

            return Json(new { Message = Message, Status = Status }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SetTemplatePermissionToSession(long EnterpriseID, long CompanyID, long RoomID, long RoleID, long templateID)
        {

            List<RoleWiseRoomsAccessDetailsDTO> lstSessionpermissions = new List<RoleWiseRoomsAccessDetailsDTO>();
            if (templateID > 0)
            {
                if (Session["SelectedRoomsPermission"] == null)
                {
                    Session["SelectedRoomsPermission"] = new List<RoleWiseRoomsAccessDetailsDTO>();
                }
                else
                {
                    lstSessionpermissions = (List<RoleWiseRoomsAccessDetailsDTO>)Session["SelectedRoomsPermission"];
                    RoleWiseRoomsAccessDetailsDTO objtoremove = lstSessionpermissions.FirstOrDefault(t => t.EnterpriseID == EnterpriseID && t.CompanyID == CompanyID && t.RoomID == RoomID);
                    if (objtoremove != null)
                    {
                        lstSessionpermissions.Remove(objtoremove);
                    }
                }
                PermissionTemplateDAL objPermissionTemplateDAL = new PermissionTemplateDAL(SessionHelper.EnterPriseDBName);
                RoleWiseRoomsAccessDetailsDTO objRoleWiseRoomsAccessDetailsDTO = new RoleWiseRoomsAccessDetailsDTO();

                List<RoleModuleDetailsDTO> lstPermissions = objPermissionTemplateDAL.GetPermissionsByTemplateForRole(templateID, RoleID, RoomID, CompanyID, EnterpriseID);
                var objMasterList = (from t in lstPermissions
                                     where t.GroupId.ToString() == "1" && t.IsModule == true
                                     select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);
                var objOtherModuleList = (from t in lstPermissions
                                          where t.GroupId.ToString() == "2" && t.IsModule == true
                                          select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                var objNonModuleList = (from t in lstPermissions
                                        where t.IsModule == false && t.GroupId.ToString() != "4"
                                        select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                var objOtherDefaultSettings = (from t in lstPermissions
                                               where t.GroupId.ToString() == "4" && t.IsModule == false
                                               select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                foreach (var item in objNonModuleList)
                {
                    item.DisplayOrderName = ResourceHelper.GetResourceValue(Enum.GetName(typeof(RoleOrderStatus), item.DisplayOrderNumber).ToString(), "ResModuleName");
                }
                objRoleWiseRoomsAccessDetailsDTO.PermissionList = lstPermissions;
                objRoleWiseRoomsAccessDetailsDTO.OtherModuleList = objOtherModuleList.ToList();
                objRoleWiseRoomsAccessDetailsDTO.ModuleMasterList = objMasterList.ToList();
                objRoleWiseRoomsAccessDetailsDTO.NonModuleList = objNonModuleList.ToList();
                objRoleWiseRoomsAccessDetailsDTO.OtherDefaultSettings = objOtherDefaultSettings.ToList();
                objRoleWiseRoomsAccessDetailsDTO.RoomID = RoomID;
                objRoleWiseRoomsAccessDetailsDTO.CompanyID = CompanyID;
                objRoleWiseRoomsAccessDetailsDTO.EnterpriseID = EnterpriseID;
                objRoleWiseRoomsAccessDetailsDTO.RoleID = RoleID;
                lstSessionpermissions.Add(objRoleWiseRoomsAccessDetailsDTO);
                Session["SelectedRoomsPermission"] = lstSessionpermissions;
            }
            else
            {

            }
            return Json(null);
        }

        public RoleWiseRoomsAccessDetailsDTO GetRolePermissionsFromSession(string RoomID, string RoleID)
        {
            List<RoleWiseRoomsAccessDetailsDTO> objRoomsAccessList = new List<RoleWiseRoomsAccessDetailsDTO>();

            if (!string.IsNullOrEmpty(RoomID) && !string.IsNullOrEmpty(RoleID) && Session["SelectedRoomsPermission"] != null)
            {
                objRoomsAccessList = (List<RoleWiseRoomsAccessDetailsDTO>)Session["SelectedRoomsPermission"];
                if (objRoomsAccessList != null && objRoomsAccessList.Count > 0)
                    return objRoomsAccessList.Find(element => element.RoomID == int.Parse(RoomID));
            }
            return null;
        }

        public RoleWiseRoomsAccessDetailsDTO GetRolePermissionsFromSession(string RoomID, string RoleID, string EnterpriseID, string CompanyID)
        {
            List<RoleWiseRoomsAccessDetailsDTO> objRoomsAccessList = new List<RoleWiseRoomsAccessDetailsDTO>();

            if (!string.IsNullOrEmpty(RoomID) && !string.IsNullOrEmpty(RoleID) && Session["SelectedRoomsPermission"] != null)
            {
                objRoomsAccessList = (List<RoleWiseRoomsAccessDetailsDTO>)Session["SelectedRoomsPermission"];
                if (objRoomsAccessList != null && objRoomsAccessList.Count > 0)
                    return objRoomsAccessList.Find(element => element.RoomID == int.Parse(RoomID) && element.CompanyID == int.Parse(CompanyID) && element.EnterpriseID == int.Parse(EnterpriseID));
            }
            return null;
        }

        public void CopyPermissionsToRooms(string ParentRoomID, string CopyToRoomIDs, string RoleID)
        {
            string EnterpriseID = "";
            string CompanyID = "";
            string RoomID = "";
            string[] ecr = ParentRoomID.Split('_');
            if (ecr.Length > 1)
            {
                EnterpriseID = ecr[0];
                CompanyID = ecr[1];
                RoomID = ecr[2];
            }
            if (!string.IsNullOrEmpty(ParentRoomID) && !string.IsNullOrEmpty(CopyToRoomIDs) && Session["SelectedRoomsPermission"] != null)
            {
                List<RoleWiseRoomsAccessDetailsDTO> objRoomsAccessList;
                if (Session["SelectedRoomsPermission"] != null)
                {
                    objRoomsAccessList = (List<RoleWiseRoomsAccessDetailsDTO>)Session["SelectedRoomsPermission"];
                    RoleWiseRoomsAccessDetailsDTO ParentRoomData = objRoomsAccessList.Find(element => element.EnterpriseID == int.Parse(EnterpriseID) && element.CompanyID == int.Parse(CompanyID) && element.RoomID == int.Parse(RoomID));
                    if (ParentRoomData != null)
                    {
                        string[] ids = CopyToRoomIDs.Split(sepForRoleRoom, StringSplitOptions.RemoveEmptyEntries);

                        for (int i = 0; i < ids.Length; i++)
                        {

                            if (!string.IsNullOrEmpty(ids[i]))
                            {
                                string CopyToEnterpriseID = "";
                                string CopyToCompanyID = "";
                                string CopyToRoomID = "";
                                string[] CopyToecr = ids[i].Split('_');
                                if (CopyToecr.Length > 2)
                                {
                                    CopyToEnterpriseID = CopyToecr[0];
                                    CopyToCompanyID = CopyToecr[1];
                                    CopyToRoomID = CopyToecr[2];
                                }
                                RoleWiseRoomsAccessDetailsDTO ChildRoomData = objRoomsAccessList.Find(element => element.RoomID == int.Parse(CopyToRoomID) && element.CompanyID == int.Parse(CopyToCompanyID) && element.EnterpriseID == int.Parse(CopyToEnterpriseID));
                                if (ChildRoomData != null)
                                {
                                    ChildRoomData.PermissionList = ParentRoomData.PermissionList;
                                }
                                else
                                {
                                    RoleWiseRoomsAccessDetailsDTO objRoomAccessDTO = new RoleWiseRoomsAccessDetailsDTO();
                                    objRoomAccessDTO.PermissionList = ParentRoomData.PermissionList;
                                    objRoomAccessDTO.RoleID = int.Parse(RoleID);
                                    objRoomAccessDTO.RoomID = int.Parse(CopyToRoomID);
                                    objRoomAccessDTO.CompanyID = int.Parse(CopyToCompanyID);
                                    objRoomAccessDTO.EnterpriseID = int.Parse(CopyToEnterpriseID);
                                    objRoomsAccessList.Add(objRoomAccessDTO);
                                }
                            }
                        }
                    }
                    Session["SelectedRoomsPermission"] = objRoomsAccessList;
                }
            }
        }


        //public void AddRemoveRoomsToSession(string RoomIDs)
        //{
        //    if (!string.IsNullOrEmpty(RoomIDs) && Session["SelectedRoomsPermission"] != null)
        //    {
        //        List<RoleWiseRoomsAccessDetailsDTO> objRoomsAccessList;
        //        List<RoleWiseRoomsAccessDetailsDTO> objTempRoomsAccessList = new List<RoleWiseRoomsAccessDetailsDTO>();
        //        objRoomsAccessList = (List<RoleWiseRoomsAccessDetailsDTO>)Session["SelectedRoomsPermission"];
        //        objTempRoomsAccessList = new List<RoleWiseRoomsAccessDetailsDTO>();

        //        string[] ids = RoomIDs.Split(',');
        //        for (int i = 0; i < ids.Length; i++)
        //        {
        //            RoleWiseRoomsAccessDetailsDTO RoomData = objRoomsAccessList.Find(element => element.RoomID == int.Parse(ids[i]));
        //            if (RoomData != null)
        //                objTempRoomsAccessList.Add(RoomData);

        //        }

        //        Session["SelectedRoomsPermission"] = objTempRoomsAccessList;
        //    }
        //    else
        //    {
        //        Session["SelectedRoomsPermission"] = null;
        //    }
        //}
        public void AddRemoveRoomsToSession(string RoomIDs)
        {
            if (!string.IsNullOrEmpty(RoomIDs) && Session["SelectedRoomsPermission"] != null)
            {
                List<RoleWiseRoomsAccessDetailsDTO> objRoomsAccessList;
                List<RoleWiseRoomsAccessDetailsDTO> objTempRoomsAccessList = new List<RoleWiseRoomsAccessDetailsDTO>();
                objRoomsAccessList = (List<RoleWiseRoomsAccessDetailsDTO>)Session["SelectedRoomsPermission"];
                objTempRoomsAccessList = new List<RoleWiseRoomsAccessDetailsDTO>();

                string[] ids = RoomIDs.Split(sepForRoleRoom, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < ids.Length; i++)
                {
                    string[] ecr = ids[i].Split('_');
                    if (ecr.Length > 1)
                    {
                        RoleWiseRoomsAccessDetailsDTO RoomData = objRoomsAccessList.Find(element => element.RoomID == int.Parse(ecr[2]) && element.CompanyID == Int64.Parse(ecr[1]) && element.EnterpriseID == Int64.Parse(ecr[0]));
                        if (RoomData != null)
                        {
                            objTempRoomsAccessList.Add(RoomData);
                        }
                    }



                }

                Session["SelectedRoomsPermission"] = objTempRoomsAccessList;
            }
            else
            {
                Session["SelectedRoomsPermission"] = null;
            }
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        //public ActionResult RolePermissionCreate(string RoomID, string RoleID)
        //{
        //    RoleWiseRoomsAccessDetailsDTO objRoomAccessDTO = new RoleWiseRoomsAccessDetailsDTO();
        //    objRoomAccessDTO = GetRolePermissionsFromSession(RoomID, RoleID);

        //    //RoleMasterController obj = new RoleMasterController();
        //    eTurns.DAL.RoleMasterDAL obj = new eTurns.DAL.RoleMasterDAL(SessionHelper.EnterPriseDBName);

        //    var objList = obj.GetRoleModuleDetailsRecord(0, 0);
        //    if (objRoomAccessDTO != null)
        //    {
        //        RoleModuleDetailsDTO objRoleModuleDetailsDTO;
        //        foreach (RoleModuleDetailsDTO item in objRoomAccessDTO.PermissionList)
        //        {
        //            objRoleModuleDetailsDTO = objList.ToList().Find(element => element.ModuleID == item.ModuleID);
        //            if (objRoleModuleDetailsDTO != null)
        //            {
        //                objRoleModuleDetailsDTO.IsChecked = item.IsChecked;
        //                objRoleModuleDetailsDTO.IsInsert = item.IsInsert;
        //                objRoleModuleDetailsDTO.IsDelete = item.IsDelete;
        //                objRoleModuleDetailsDTO.IsUpdate = item.IsUpdate;
        //                objRoleModuleDetailsDTO.IsView = item.IsView;

        //                objRoleModuleDetailsDTO.ShowDeleted = item.ShowDeleted;
        //                objRoleModuleDetailsDTO.ShowArchived = item.ShowArchived;
        //                objRoleModuleDetailsDTO.ShowUDF = item.ShowUDF;

        //                objRoleModuleDetailsDTO.ModuleValue = item.ModuleValue;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        objRoomAccessDTO = new RoleWiseRoomsAccessDetailsDTO();
        //        int SelectedRoomID = 0;
        //        SelectedRoomID = int.Parse(RoomID);
        //        objRoomAccessDTO.PermissionList = objList.ToList();
        //        objRoomAccessDTO.RoleID = int.Parse(RoleID);
        //        objRoomAccessDTO.RoomID = SelectedRoomID;
        //    }

        //    var objMasterList = from t in objList
        //                        where t.GroupId.ToString() == "1" && t.IsModule == true
        //                        select t;
        //    var objOtherModuleList = from t in objList
        //                             where t.GroupId.ToString() == "2" && t.IsModule == true
        //                             select t;

        //    var objNonModuleList = from t in objList
        //                           where t.IsModule == false && t.GroupId.ToString() != "4"
        //                           select t;

        //    var objOtherDefaultSettings = from t in objList
        //                                  where t.GroupId.ToString() == "4" && t.IsModule == false
        //                                  select t;


        //    objRoomAccessDTO.ModuleMasterList = objMasterList.ToList();
        //    objRoomAccessDTO.OtherModuleList = objOtherModuleList.ToList();
        //    objRoomAccessDTO.NonModuleList = objNonModuleList.ToList();
        //    objRoomAccessDTO.OtherDefaultSettings = objOtherDefaultSettings.ToList();

        //    List<RoomDTO> objRoomList = new List<RoomDTO>();
        //    RoomDTO odjRoomDTO = new RoomDTO();
        //    odjRoomDTO.ID = 0;
        //    odjRoomDTO.RoomName = "";
        //    objRoomList.Insert(0, odjRoomDTO);
        //    ViewBag.RoomsList = objRoomList;

        //    return PartialView("_CreateRolePermission", objRoomAccessDTO);
        //}
        public ActionResult RolePermissionCreate(string RoomID, string RoleID, int UserType)
        {

            string EnterpriseID = "0";
            string CompanyID = "0";
            string[] ecr = RoomID.Split('_');
            if (ecr.Length > 1)
            {
                EnterpriseID = ecr[0];
                CompanyID = ecr[1];
                RoomID = ecr[2];
            }
            if (UserType == 1)
            {
                RoleWiseRoomsAccessDetailsDTO objRoomAccessDTO = new RoleWiseRoomsAccessDetailsDTO();
                //objRoomAccessDTO = GetRolePermissionsFromSession(RoomID, RoleID);
                objRoomAccessDTO = GetRolePermissionsFromSession(RoomID, RoleID, EnterpriseID, CompanyID);
                //RoleMasterController obj = new RoleMasterController();

                eTurnsMaster.DAL.RoleMasterDAL obj = new eTurnsMaster.DAL.RoleMasterDAL(SessionHelper.EnterPriseDBName);

                var objList = obj.GetRoleModuleDetailsRecord(0, 0, 0, 0);
                if (objRoomAccessDTO != null)
                {
                    RoleModuleDetailsDTO objRoleModuleDetailsDTO;
                    foreach (RoleModuleDetailsDTO item in objRoomAccessDTO.PermissionList)
                    {
                        objRoleModuleDetailsDTO = objList.ToList().Find(element => element.ModuleID == item.ModuleID);
                        if (objRoleModuleDetailsDTO != null)
                        {
                            objRoleModuleDetailsDTO.IsChecked = item.IsChecked;
                            objRoleModuleDetailsDTO.IsInsert = item.IsInsert;
                            objRoleModuleDetailsDTO.IsDelete = item.IsDelete;
                            objRoleModuleDetailsDTO.IsUpdate = item.IsUpdate;
                            objRoleModuleDetailsDTO.IsView = item.IsView;

                            objRoleModuleDetailsDTO.ShowDeleted = item.ShowDeleted;
                            objRoleModuleDetailsDTO.ShowArchived = item.ShowArchived;
                            objRoleModuleDetailsDTO.ShowUDF = item.ShowUDF;

                            objRoleModuleDetailsDTO.ModuleValue = item.ModuleValue;
                        }
                    }
                }
                else
                {
                    objRoomAccessDTO = new RoleWiseRoomsAccessDetailsDTO();
                    int SelectedRoomID = 0;
                    SelectedRoomID = int.Parse(RoomID);
                    objRoomAccessDTO.PermissionList = objList.ToList();
                    objRoomAccessDTO.RoleID = int.Parse(RoleID);
                    objRoomAccessDTO.RoomID = SelectedRoomID;
                    objRoomAccessDTO.CompanyID = Int64.Parse(CompanyID);
                    objRoomAccessDTO.EnterpriseID = Int64.Parse(EnterpriseID);
                }

                //var objMasterList = from t in objList
                //                    where t.GroupId.ToString() == "1" && t.IsModule == true
                //                    select t;
                //var objOtherModuleList = from t in objList
                //                         where t.GroupId.ToString() == "2" && t.IsModule == true
                //                         select t;

                //var objNonModuleList = from t in objList
                //                       where t.IsModule == false && t.GroupId.ToString() != "4"
                //                       select t;

                //var objOtherDefaultSettings = from t in objList
                //                              where t.GroupId.ToString() == "4" && t.IsModule == false
                //                              select t;
                var objMasterList = (from t in objList
                                     where t.GroupId.ToString() == "1" && t.IsModule == true
                                     select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);
                var objOtherModuleList = (from t in objList
                                          where t.GroupId.ToString() == "2" && t.IsModule == true
                                          select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                var objNonModuleList = (from t in objList
                                        where t.IsModule == false && t.GroupId.ToString() != "4"
                                        select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                var objOtherDefaultSettings = (from t in objList
                                               where t.GroupId.ToString() == "4" && t.IsModule == false
                                               select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);



                foreach (var item in objNonModuleList)
                {
                    item.DisplayOrderName = ResourceHelper.GetResourceValue(Enum.GetName(typeof(RoleOrderStatus), item.DisplayOrderNumber).ToString(), "ResModuleName");
                }

                objRoomAccessDTO.ModuleMasterList = objMasterList.ToList();
                objRoomAccessDTO.OtherModuleList = objOtherModuleList.ToList();
                objRoomAccessDTO.NonModuleList = objNonModuleList.ToList();
                objRoomAccessDTO.OtherDefaultSettings = objOtherDefaultSettings.ToList();

                List<RoomDTO> objRoomList = new List<RoomDTO>();
                RoomDTO odjRoomDTO = new RoomDTO();
                odjRoomDTO.ID = 0;
                odjRoomDTO.RoomName = "";
                objRoomList.Insert(0, odjRoomDTO);
                ViewBag.RoomsList = objRoomList;

                return PartialView("_CreateRolePermission", objRoomAccessDTO);
            }
            else
            {
                RoleWiseRoomsAccessDetailsDTO objRoomAccessDTO = new RoleWiseRoomsAccessDetailsDTO();
                //objRoomAccessDTO = GetRolePermissionsFromSession(RoomID, RoleID);
                objRoomAccessDTO = GetRolePermissionsFromSession(RoomID, RoleID, EnterpriseID, CompanyID);
                //RoleMasterController obj = new RoleMasterController();
                eTurns.DAL.RoleMasterDAL obj = new eTurns.DAL.RoleMasterDAL(SessionHelper.EnterPriseDBName);

                var objList = obj.GetRoleModuleDetailsRecord(0, 0);
                if (objRoomAccessDTO != null)
                {
                    RoleModuleDetailsDTO objRoleModuleDetailsDTO;
                    foreach (RoleModuleDetailsDTO item in objRoomAccessDTO.PermissionList)
                    {
                        objRoleModuleDetailsDTO = objList.ToList().Find(element => element.ModuleID == item.ModuleID);
                        if (objRoleModuleDetailsDTO != null)
                        {
                            objRoleModuleDetailsDTO.IsChecked = item.IsChecked;
                            objRoleModuleDetailsDTO.IsInsert = item.IsInsert;
                            objRoleModuleDetailsDTO.IsDelete = item.IsDelete;
                            objRoleModuleDetailsDTO.IsUpdate = item.IsUpdate;
                            objRoleModuleDetailsDTO.IsView = item.IsView;

                            objRoleModuleDetailsDTO.ShowDeleted = item.ShowDeleted;
                            objRoleModuleDetailsDTO.ShowArchived = item.ShowArchived;
                            objRoleModuleDetailsDTO.ShowUDF = item.ShowUDF;

                            objRoleModuleDetailsDTO.ModuleValue = item.ModuleValue;
                        }
                    }
                }
                else
                {
                    objRoomAccessDTO = new RoleWiseRoomsAccessDetailsDTO();
                    int SelectedRoomID = 0;
                    SelectedRoomID = int.Parse(RoomID);
                    objRoomAccessDTO.PermissionList = objList.ToList();
                    objRoomAccessDTO.RoleID = int.Parse(RoleID);
                    objRoomAccessDTO.RoomID = SelectedRoomID;
                    objRoomAccessDTO.CompanyID = Int64.Parse(CompanyID);
                    objRoomAccessDTO.EnterpriseID = Int64.Parse(EnterpriseID);
                }

                //var objMasterList = from t in objList
                //                    where t.GroupId.ToString() == "1" && t.IsModule == true
                //                    select t;
                //var objOtherModuleList = from t in objList
                //                         where t.GroupId.ToString() == "2" && t.IsModule == true
                //                         select t;

                //var objNonModuleList = from t in objList
                //                       where t.IsModule == false && t.GroupId.ToString() != "4"
                //                       select t;

                //var objOtherDefaultSettings = from t in objList
                //                              where t.GroupId.ToString() == "4" && t.IsModule == false
                //                              select t;
                var objMasterList = (from t in objList
                                     where t.GroupId.ToString() == "1" && t.IsModule == true && t.ModuleID != 41
                                     select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);
                var objOtherModuleList = (from t in objList
                                          where t.GroupId.ToString() == "2" && t.IsModule == true
                                          select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                var objNonModuleList = (from t in objList
                                        where t.IsModule == false && t.GroupId.ToString() != "4"
                                        select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                var objOtherDefaultSettings = (from t in objList
                                               where t.GroupId.ToString() == "4" && t.IsModule == false
                                               select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                foreach (var item in objNonModuleList)
                {
                    item.DisplayOrderName = ResourceHelper.GetResourceValue(Enum.GetName(typeof(RoleOrderStatus), item.DisplayOrderNumber).ToString(), "ResModuleName");
                }

                objRoomAccessDTO.ModuleMasterList = objMasterList.ToList();
                objRoomAccessDTO.OtherModuleList = objOtherModuleList.ToList();
                objRoomAccessDTO.NonModuleList = objNonModuleList.ToList();
                objRoomAccessDTO.OtherDefaultSettings = objOtherDefaultSettings.ToList();

                List<RoomDTO> objRoomList = new List<RoomDTO>();
                RoomDTO odjRoomDTO = new RoomDTO();
                odjRoomDTO.ID = 0;
                odjRoomDTO.RoomName = "";
                objRoomList.Insert(0, odjRoomDTO);
                ViewBag.RoomsList = objRoomList;

                return PartialView("_CreateRolePermission", objRoomAccessDTO);
            }
        }
        #endregion

        #endregion

        #region [Reset Password]

        public ActionResult ResetPassword(string fp)
        {
            ChangePasswordDTO objChangePasswordDTO = new ChangePasswordDTO();
            string decrypedtext = string.Empty;
            long UserId = 0;
            Guid Token = Guid.Empty;
            EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
            if (!string.IsNullOrWhiteSpace(fp))
            {

                string[] arrsrt = new string[] { "__" };
                decrypedtext = CommonUtility.DecryptText(fp);
                string[] arr = decrypedtext.Split(arrsrt, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length == 2)
                {
                    long.TryParse(arr[0], out UserId);
                    Guid.TryParse(arr[1], out Token);
                    if (UserId > 0 && Token != Guid.Empty)
                    {
                        ForgotPasswordRequest objForgotPasswordRequest = objEnterpriseMasterDAL.GetForgotPasswordRequest(Token);
                        if (objForgotPasswordRequest != null)
                        {
                            objChangePasswordDTO.ConfirmPassword = string.Empty;
                            objChangePasswordDTO.FirstPassword = string.Empty;
                            objChangePasswordDTO.ID = objForgotPasswordRequest.UserId;
                            eTurnsMaster.DAL.EnterpriseMasterDAL objEturnsMaster = new eTurnsMaster.DAL.EnterpriseMasterDAL();
                            long enterpirseid = objEturnsMaster.GetEnterpriseIdByUserID(UserId);
                            SessionHelper.EnterPriceID = enterpirseid;
                            ResourceHelper.EnterpriseResourceFolder = Convert.ToString(enterpirseid);


                            objChangePasswordDTO.IsExpired = objForgotPasswordRequest.IsExpired;
                            objChangePasswordDTO.IsProcessed = objForgotPasswordRequest.IsProcessed;
                            objChangePasswordDTO.RequestToken = objForgotPasswordRequest.RequestToken;
                            objChangePasswordDTO.TokenGeneratedDate = objForgotPasswordRequest.TokenGeneratedDate;
                            objChangePasswordDTO.UserId = objForgotPasswordRequest.UserId;
                        }

                    }
                }

            }

            return View(objChangePasswordDTO);
        }

        [HttpPost]
        public JsonResult ResetPassword(string NewPassword, long? UserID, Guid? RequestToken)
        {
            if ((UserID ?? 0) > 0 && (RequestToken ?? Guid.Empty) != Guid.Empty && !string.IsNullOrWhiteSpace(NewPassword))
            {
                EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
                UserBAL objUserBAL = new UserBAL();
                ForgotPasswordRequest objForgotPasswordRequest = objEnterpriseMasterDAL.GetForgotPasswordRequest(RequestToken.Value);
                if (objForgotPasswordRequest == null)
                {
                    return Json(new { Message = "Link is not valid", Status = "fail" });
                }
                else if (objForgotPasswordRequest != null && DateTimeUtility.DateTimeNow > objForgotPasswordRequest.TokenGeneratedDate.AddDays(1))
                {
                    return Json(new { Message = "Link is expired.please try again", Status = "fail" });
                }
                else if (objForgotPasswordRequest != null && objForgotPasswordRequest.IsExpired)
                {
                    return Json(new { Message = "Link is expired.please try again", Status = "fail" });
                }
                else if (objForgotPasswordRequest != null && objForgotPasswordRequest.IsProcessed)
                {
                    return Json(new { Message = "Link is already Used once to set password.please try again", Status = "fail" });
                }
                else
                {
                    EnterPriseConfigDTO objDTO = new EnterPriseConfigDTO();
                    eTurnsMaster.DAL.EnterpriseMasterDAL objEturnsMaster = new eTurnsMaster.DAL.EnterpriseMasterDAL();
                    long enterpirseid = objEturnsMaster.GetEnterpriseIdByUserID(UserID ?? 0);
                    EnterpriseDTO objEnterpriseDTO = new EnterpriseMasterDAL().GetEnterprise(enterpirseid);
                    UserMasterDTO objUserMasterDTO = new UserMasterDTO();
                    eTurnsMaster.DAL.UserMasterDAL objUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                    objUserMasterDTO = objUserMasterDAL.GetRecord(UserID ?? 0);

                    EnterPriseConfigDAL obj = new EnterPriseConfigDAL(objEnterpriseDTO.EnterpriseDBName);
                    objDTO = obj.GetRecord(Convert.ToInt32(enterpirseid));

                    int PrevLastNoOfAllowPwd = objDTO.PreviousLastAllowedPWD ?? 0;
                    var result = true;
                    using (eTurns_MasterEntities context = new eTurns_MasterEntities())
                    {
                        IList<string> userPasswordChangeHistory = context.UserPasswordChangeHistories.Where(u => u.UserId == UserID).OrderByDescending(m => m.Id).Select(m => m.OldPassword).Distinct().Take(PrevLastNoOfAllowPwd).ToList();
                        if (userPasswordChangeHistory.Contains(NewPassword) || objUserMasterDTO.Password == NewPassword)
                        {
                            result = false;
                        }
                    }
                    if (result == false)
                    {

                        return Json(new { Message = "repeatpwd", Status = "fail" });
                    }
                    objUserBAL.ResetPassword(UserID ?? 0, NewPassword);
                    objForgotPasswordRequest.IsProcessed = true;
                    objEnterpriseMasterDAL.SaveForgotPasswordRequest(objForgotPasswordRequest);
                }
            }
            return Json(new { Message = "success", Status = "ok" });
        }

        #endregion

        #region "User Master"

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost, CaptchaVerify("Captcha is not valid")]
        public JsonResult SendPassword(string UserName)
        {


            string status = "";
            UserBAL objUserBAL = new UserBAL();
            UserMasterDTO objUserMasterDTO = null;
            try
            {
                if (this.IsCaptchaValid("Captcha is not valid"))
                {
                    objUserMasterDTO = objUserBAL.GetPasswordResetLink(UserName);
                    if (objUserMasterDTO != null && !string.IsNullOrEmpty(objUserMasterDTO.Email))
                    {
                        SendPasswordBYUserName(UserName, objUserMasterDTO.Password, objUserMasterDTO.Email, objUserMasterDTO);
                        EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
                        ForgotPasswordRequest objForgotPasswordRequest = new ForgotPasswordRequest();
                        objForgotPasswordRequest.IsExpired = false;
                        objForgotPasswordRequest.IsProcessed = false;
                        objForgotPasswordRequest.RequestToken = objUserMasterDTO.PasswordResetRequestID ?? Guid.Empty;
                        objForgotPasswordRequest.TokenGeneratedDate = DateTimeUtility.DateTimeNow;
                        objForgotPasswordRequest.UserId = objUserMasterDTO.ID;
                        objForgotPasswordRequest = objEnterpriseMasterDAL.SaveForgotPasswordRequest(objForgotPasswordRequest);
                        status = "ok";
                    }
                    else
                    {
                        status = "Fail";
                    }
                }
                else
                {
                    status = "Incorrect Captcha";
                }
                return Json(new { Message = status });

            }
            catch (Exception)
            {
                status = "Fail";
                return Json(new { Message = status });
            }
            finally
            {
                objUserMasterDTO = null;

            }

        }

        [HttpPost]
        public ActionResult ForgotPassword(string username)
        {
            string status = "";
            UserBAL objUserBAL = new UserBAL();
            UserMasterDTO objUserMasterDTO = null;
            try
            {
                if (this.IsCaptchaValid("Captcha is not valid"))
                {
                    objUserMasterDTO = objUserBAL.GetPasswordResetLink(username);
                    if (objUserMasterDTO != null && !string.IsNullOrEmpty(objUserMasterDTO.Email))
                    {
                        SessionHelper.EnterPriceID = objUserMasterDTO.EnterpriseId;
                        ResourceHelper.EnterpriseResourceFolder = Convert.ToString(objUserMasterDTO.EnterpriseId);

                        SendPasswordBYUserName(username, objUserMasterDTO.Password, objUserMasterDTO.Email, objUserMasterDTO);
                        EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
                        ForgotPasswordRequest objForgotPasswordRequest = new ForgotPasswordRequest();
                        objForgotPasswordRequest.IsExpired = false;
                        objForgotPasswordRequest.IsProcessed = false;
                        objForgotPasswordRequest.RequestToken = objUserMasterDTO.PasswordResetRequestID ?? Guid.Empty;
                        objForgotPasswordRequest.TokenGeneratedDate = DateTimeUtility.DateTimeNow;
                        objForgotPasswordRequest.UserId = objUserMasterDTO.ID;
                        objForgotPasswordRequest = objEnterpriseMasterDAL.SaveForgotPasswordRequest(objForgotPasswordRequest);
                        status = "ok";
                    }
                    else
                    {
                        status = "Fail";
                    }
                }
                else
                {
                    status = "Incorrect Captcha";
                }

                ViewBag.Status = status;
                ViewBag.Message = string.Empty;
                return View();
                //return Json(new { Message = status });

            }
            catch (Exception)
            {
                status = "Fail";
                return Json(new { Message = status });

            }
            finally
            {
                objUserMasterDTO = null;
            }

        }



        /// <summary>
        /// SendPasswordBYUserName
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <param name="ToEmailAddress"></param>
        private void SendPasswordBYUserName(string UserName, string Password, string ToEmailAddress, UserMasterDTO objUserMasterDTO)
        {
            StringBuilder MessageBody = null;
            EnterpriseDTO objEnterpriseDTO = new EnterpriseDTO();
            EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
            EmailTemplateDAL objEmailTemplateDAL = null;
            if (objUserMasterDTO.EnterpriseId > 0)
            {
                objEnterpriseDTO = objEnterpriseMasterDAL.GetEnterprise(objUserMasterDTO.EnterpriseId);
                objEmailTemplateDAL = new EmailTemplateDAL(objEnterpriseDTO.EnterpriseDBName);
            }
            else
            {
                objEmailTemplateDAL = new EmailTemplateDAL(DbConHelper.GetETurnsDBName());
            }

            eTurnsUtility objUtils = null;
            eTurnsMaster.DAL.eMailDAL objEmailDAL = null;
            string strPath = string.Empty;
            string strReplacepath = "";
            if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Request != null)
            {
                strPath = System.Web.HttpContext.Current.Request.Url.ToString();
                strReplacepath = System.Web.HttpContext.Current.Request.Url.PathAndQuery;
                strPath = strPath.Replace(strReplacepath, "/");
            }
            else if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["DomainName"]))
            {
                strPath = System.Configuration.ConfigurationManager.AppSettings["DomainName"];

            }
            try
            {
                string StrSubject = "Forgot Password";
                string strToAddress = ToEmailAddress;
                if (!string.IsNullOrEmpty(strToAddress))
                {
                    string FromAddress = ConfigurationManager.AppSettings["FromAddress"].ToString();
                    string strCCAddress = "";
                    MessageBody = new StringBuilder();
                    MessageBody = EmailTemplateReplaceURL("ForgotPassword-" + eTurns.DTO.Resources.ResourceHelper.CurrentCult + ".html");


                    EmailTemplateDetailDTO objEmailTemplateDetailDTO = objEmailTemplateDAL.GetEmailTemplate("ForgotPassword", "en-US", 0, 0);

                    if (objEmailTemplateDetailDTO != null)
                    {
                        StrSubject = objEmailTemplateDetailDTO.MailSubject;
                        MessageBody = new StringBuilder();
                        MessageBody = MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
                    }

                    //MessageBody = MessageBody.Replace("@@ETURNSLOGO@@", GeteTurnsImage(strPath, "Content/images/logo.jpg"));
                    MessageBody = MessageBody.Replace("@@Year@@", DateTimeUtility.DateTimeNow.Year.ToString());
                    MessageBody = MessageBody.Replace("@@UserName@@", UserName);
                    MessageBody = MessageBody.Replace("@@username@@", UserName);
                    MessageBody = MessageBody.Replace("@@PasswordResetLink@@", GetResetLink(objUserMasterDTO.PasswordResetLink));
                    MessageBody = MessageBody.Replace("/CKUpload/", strPath + "CKUpload/");
                    //CommonUtility.SendMail(FromAddress, strToAddress, strCCAddress, strBCCAddress, strNotificationAddress, StrSubject, MessageBody.ToString(), true);
                    objUtils = new eTurnsUtility();
                    objUtils.SendMail(strToAddress, strCCAddress, StrSubject, MessageBody.ToString());
                    objEmailDAL = new eTurnsMaster.DAL.eMailDAL();
                    objEmailDAL.eMailToSend(strToAddress, strCCAddress, StrSubject, MessageBody.ToString(), objUserMasterDTO.EnterpriseId, objUserMasterDTO.CompanyID, objUserMasterDTO.Room ?? 0, 0, null, "Web => EmailLink => SendPasswordByUsername");

                }
            }
            finally
            {
                MessageBody = null;
                objUtils = null;
                objEmailDAL = null;
            }
        }


        //public bool SendPasswordBYUserName(string UserName, string Password, string ToEmailAddress, UserMasterDTO objUserMasterDTO)
        //{
        //    StringBuilder MessageBody = null;
        //    EmailTemplateDAL objEmailTemplateDAL = null;
        //    EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
        //    CompanyMasterDAL objCompDAL = null;
        //    CompanyMasterDTO objCompany = null;
        //    CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
        //    eTurnsUtility objUtils = null;
        //    try
        //    {
        //        string QtyFormat = "N";
        //        string FromAddress = System.Configuration.ConfigurationManager.AppSettings["FromAddress"].ToString();
        //        string strCCAddress = "";
        //        string strBCCAddress = "";
        //        string strNotificationAddress = "";
        //        string strToAddress = objUserMasterDTO.Email;
        //        if (string.IsNullOrEmpty(strToAddress))
        //            return false;


        //        string StrSubject = string.Empty;
        //        MessageBody = new StringBuilder();
        //        objEmailTemplateDAL = new EmailTemplateDAL(SessionHelper.EnterPriseDBName);
        //        objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();
        //        string currentCulture = "en-US";

        //        if (eTurns.DTO.Resources.ResourceHelper.CurrentCult != null)
        //        {
        //            if (!string.IsNullOrWhiteSpace(Convert.ToString(eTurns.DTO.Resources.ResourceHelper.CurrentCult)))
        //            {
        //                currentCulture = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
        //            }

        //        }

        //        objEmailTemplateDetailDTO = objEmailTemplateDAL.GetEmailTemplate("ForgotPassword", currentCulture, SessionHelper.RoomID, SessionHelper.CompanyID);

        //        if (objEmailTemplateDetailDTO != null)
        //        {
        //            MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
        //            StrSubject = objEmailTemplateDetailDTO.MailSubject;
        //        }
        //        else
        //        {
        //            return false;
        //        }

        //        string strPath = string.Empty;
        //        string strReplacepath = "";
        //        if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Request != null)
        //        {
        //            strPath = System.Web.HttpContext.Current.Request.Url.ToString();
        //            strReplacepath = System.Web.HttpContext.Current.Request.Url.PathAndQuery;
        //            strPath = strPath.Replace(strReplacepath, "/");
        //        }
        //        else if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["DomainName"]))
        //        {
        //            strPath = System.Configuration.ConfigurationManager.AppSettings["DomainName"];

        //        }

        //        MessageBody = MessageBody.Replace("@@ETURNSLOGO@@", GeteTurnsImage(strPath, "Content/images/logo.jpg"));
        //        MessageBody = MessageBody.Replace("@@Year@@", DateTime.Now.Year.ToString());
        //        MessageBody.Replace("@@UserName@@", UserName);
        //        MessageBody.Replace("@@PasswordResetLink@@", objUserMasterDTO.PasswordResetLink);
        //        MessageBody = MessageBody.Replace("/CKUpload/", strPath + "CKUpload/");
        //        objUtils = new eTurnsUtility();
        //        objUtils.SendMail(strToAddress, strCCAddress, StrSubject, MessageBody.ToString());
        //        string MasterDbConection = System.Configuration.ConfigurationManager.ConnectionStrings["eTurnsMasterDbConnection"].ConnectionString.ToString();
        //        eTurnsMaster.DAL.SqlHelper.ExecuteNonQuery(MasterDbConection, "MailSendEntry", MessageBody.ToString(), 0, DateTime.Now, false, 0, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, UserId, strToAddress, string.Empty, string.Empty, StrSubject, string.Empty);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //    finally
        //    {
        //        MessageBody = null;
        //        objEmailTemplateDAL = null;
        //        objEmailTemplateDetailDTO = null;
        //        objCompDAL = null;
        //        objCompany = null;
        //        objUtils = null;
        //    }
        //}
        public string GeteTurnsImage(string path, string imagePath)
        {
            string str = string.Empty;

            str = @"<a href='" + path + @"' title=""E Turns Powered""> <img alt=""E Turns Powered"" src='" + (path + imagePath) + @"' style=""border: 0px currentColor; border-image: none;"" /></a>";
            return str;
        }
        public string GetResetLink(string LinkToReset)
        {
            string str = string.Empty;

            str = @"<a href='" + LinkToReset + @"' title=""Reset"">Click Here</a>";
            return str;
        }
        public ActionResult UserList()
        {
            //UserBAL objUserBAL = new UserBAL();
            //List<UserMasterDTO> lstUsers = new List<UserMasterDTO>();
            //List<RolePermissionInfo> outlstAllPermissions = new List<RolePermissionInfo>();
            //List<UserAccessDTO> lstUserAccess = new UserBAL().GetUserAccessForUserList(SessionHelper.UserType, SessionHelper.RoleID, SessionHelper.UserID, SessionHelper.EnterPriceID, SessionHelper.CompanyID);
            //Session["AllUserPermissions"] = lstUserAccess;
            //lstUsers = objUserBAL.GetAllUsersBySQlHelper(SessionHelper.UserType, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, false, false, SessionHelper.UserID, SessionHelper.RoleID, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, out outlstAllPermissions);
            //Session["lstAllUsers"] = lstUsers;

            eTurnsMaster.DAL.UserMasterDAL objUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
            Session["AllUserPermissions"] = objUserMasterDAL.GetPagedUsersNS(SessionHelper.UserID);

            return View();
        }

        public PartialViewResult _CreateUser()
        {
            return PartialView();
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        [AjaxOrChildActionOnlyAttribute]
        public ActionResult UserCreate()
        {
            UserMasterDTO objDTO = new UserMasterDTO();
            objDTO.ID = 0;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.LastUpdatedBy = SessionHelper.UserID;

            objDTO.CreatedByName = SessionHelper.UserName;
            objDTO.UpdatedByName = SessionHelper.UserName;

            objDTO.Room = SessionHelper.RoomID;
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.RoomName = SessionHelper.RoomName;
            objDTO.GUID = Guid.NewGuid();

            //UserMasterController obj = new UserMasterController();
            eTurns.DAL.UserMasterDAL obj = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
            var objList = obj.GetUserRoleModuleDetailsRecord(0, 0, 0);

            //var objMasterList = from t in objList
            //                    where t.GroupId.ToString() == "1" && t.IsModule == true
            //                    select t;
            //var objOtherModuleList = from t in objList
            //                         where t.GroupId.ToString() == "2" && t.IsModule == true
            //                         select t;

            //var objNonModuleList = from t in objList
            //                       where t.IsModule == false && t.GroupId.ToString() != "4"
            //                       select t;

            //var objOtherDefaultSettings = from t in objList
            //                              where t.GroupId.ToString() == "4" && t.IsModule == false
            //                              select t;

            var objMasterList = (from t in objList
                                 where t.GroupId.ToString() == "1" && t.IsModule == true
                                 select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);
            var objOtherModuleList = (from t in objList
                                      where t.GroupId.ToString() == "2" && t.IsModule == true
                                      select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

            var objNonModuleList = (from t in objList
                                    where t.IsModule == false && t.GroupId.ToString() != "4"
                                    select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

            var objOtherDefaultSettings = (from t in objList
                                           where t.GroupId.ToString() == "4" && t.IsModule == false
                                           select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);
            //objDTO.ModuleMasterList = objMasterList.ToList();
            //objDTO.OtherModuleList = objOtherModuleList.ToList();
            //objDTO.NonModuleList = objNonModuleList.ToList();

            foreach (var item in objNonModuleList)
            {
                item.DisplayOrderName = ResourceHelper.GetResourceValue(Enum.GetName(typeof(RoleOrderStatus), item.DisplayOrderNumber).ToString(), "ResModuleName");
            }

            objDTO.SelectedModuleIDs = "";
            objDTO.SelectedNonModuleIDs = "";
            objDTO.SelectedRoomReplanishmentValue = "";

            UserWiseRoomsAccessDetailsDTO objRoomAccessDTO = new UserWiseRoomsAccessDetailsDTO();
            objRoomAccessDTO.PermissionList = objList.ToList();
            objRoomAccessDTO.ModuleMasterList = objMasterList.ToList();
            objRoomAccessDTO.OtherModuleList = objOtherModuleList.ToList();
            objRoomAccessDTO.NonModuleList = objNonModuleList.ToList();
            objRoomAccessDTO.OtherDefaultSettings = objOtherDefaultSettings.ToList();

            objRoomAccessDTO.RoleID = 0;
            objRoomAccessDTO.RoomID = 0;

            objDTO.UserWiseAllRoomsAccessDetails = null;
            objDTO.UserwiseRoomsAccessDetail = objRoomAccessDTO;

            List<RoomDTO> objRoomList = new List<RoomDTO>();
            RoomDTO odjRoomDTO = new RoomDTO();
            odjRoomDTO.ID = 0;
            odjRoomDTO.RoomName = "";
            objRoomList.Insert(0, odjRoomDTO);
            ViewBag.RoomsList = objRoomList.OrderBy(e => e.ID);

            Session["SelectedUserRoomsPermission"] = null;

            objDTO.IseTurnsAdmin = false;

            return PartialView("_CreateUser", objDTO);
        }

        [HttpPost]
        public JsonResult UserSave(UserMasterDTO objDTO)
        {
            string message = "";
            string status = "";
            string passwordBeforeHash = "";
            if (objDTO.UserType < SessionHelper.UserType)
            {
                message = "You can not create user!";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }

            if (objDTO.UserType != 1)
            {
                objDTO.IseTurnsAdmin = false;
            }

            if (!string.IsNullOrWhiteSpace(objDTO.Password))
            {
                passwordBeforeHash = objDTO.Password;
                objDTO.Password = CommonUtility.getSHA15Hash(objDTO.Password);
            }

            List<UserAccessDTO> objAccessList = new List<UserAccessDTO>();
            UserAccessDTO objUserAccessDTO = null;
            long enterId = 0, CompId = 0, RmId = 0;
            if (!string.IsNullOrWhiteSpace(objDTO.SelectedRoomAccessValue))
            {
                string[] arrstr = objDTO.SelectedRoomAccessValue.Split(sepForRoleRoom, StringSplitOptions.RemoveEmptyEntries);
                foreach (string cdid in arrstr)
                {
                    if (!string.IsNullOrWhiteSpace(cdid))
                    {
                        string[] arrids = cdid.Split('_');
                        if (long.TryParse(arrids[0], out enterId) && long.TryParse(arrids[1], out CompId) && long.TryParse(arrids[2], out RmId))
                        {
                            objUserAccessDTO = new UserAccessDTO();
                            objUserAccessDTO.EnterpriseId = enterId;
                            objUserAccessDTO.CompanyId = CompId;
                            objUserAccessDTO.RoomId = RmId;
                            objAccessList.Add(objUserAccessDTO);
                        }
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(objDTO.SelectedCompanyAccessValue))
            {
                string[] arrstr = objDTO.SelectedCompanyAccessValue.Split(sepForRoleRoom, StringSplitOptions.RemoveEmptyEntries);
                foreach (string cdid in arrstr)
                {
                    if (!string.IsNullOrWhiteSpace(cdid))
                    {
                        string[] arrids = cdid.Split('_');
                        if (long.TryParse(arrids[0], out enterId) && long.TryParse(arrids[1], out CompId))
                        {
                            if (!objAccessList.Any(t => t.EnterpriseId == enterId && t.CompanyId == CompId))
                            {
                                objUserAccessDTO = new UserAccessDTO();
                                objUserAccessDTO.EnterpriseId = enterId;
                                objUserAccessDTO.CompanyId = CompId;
                                objAccessList.Add(objUserAccessDTO);
                            }
                        }
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(objDTO.SelectedEnterpriseAccessValue))
            {
                string[] arrstr = objDTO.SelectedEnterpriseAccessValue.Split(sepForRoleRoom, StringSplitOptions.RemoveEmptyEntries);
                foreach (string epid in arrstr)
                {
                    if (long.TryParse(epid, out enterId))
                    {
                        if (!objAccessList.Any(t => t.EnterpriseId == enterId))
                        {
                            objUserAccessDTO = new UserAccessDTO();
                            objUserAccessDTO.EnterpriseId = enterId;
                            objAccessList.Add(objUserAccessDTO);
                        }
                    }
                }
            }

            objDTO.lstAccess = objAccessList;




            eTurnsMaster.DAL.UserMasterDAL obj = new eTurnsMaster.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
            CommonMasterDAL objCDAL = new CommonMasterDAL();
            if (string.IsNullOrEmpty(objDTO.UserName))
            {
                message = "User name is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(objDTO.UserName))
            {
                message = "User name is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }

            //if (!string.IsNullOrEmpty(objDTO.SelectedRoomReplanishmentValue))
            //{
            //    List<UserRoomReplanishmentDetailsDTO> objRoomRepIDs = new List<UserRoomReplanishmentDetailsDTO>();

            //    string[] objRoomReplanishmentIDs = objDTO.SelectedRoomReplanishmentValue.Split(',');
            //    for (int i = 0; i < objRoomReplanishmentIDs.Length; i++)
            //    {
            //        string[] arrids = objRoomReplanishmentIDs[i].Split('_');
            //        UserRoomReplanishmentDetailsDTO objRoleRoom = new UserRoomReplanishmentDetailsDTO();
            //        objRoleRoom.EnterpriseId = Convert.ToInt64(arrids[0]);
            //        objRoleRoom.CompanyId = Convert.ToInt64(arrids[1]);
            //        objRoleRoom.RoomID = Convert.ToInt64(arrids[2]);
            //        //objRoleRoom.RoomID = int.Parse(objRoomReplanishmentIDs[i].ToString());
            //        objRoomRepIDs.Add(objRoleRoom);
            //    }
            //    objDTO.ReplenishingRooms = objRoomRepIDs;
            //}
            if (Session["SelectedUserRoomsPermission"] != null)
            {
                List<UserWiseRoomsAccessDetailsDTO> objRoomsAccessList = (List<UserWiseRoomsAccessDetailsDTO>)Session["SelectedUserRoomsPermission"];
                List<UserWiseRoomsAccessDetailsDTO> objSelectedRooms = new List<UserWiseRoomsAccessDetailsDTO>();
                if (objRoomsAccessList != null && objRoomsAccessList.Count > 0)
                {
                    if (string.IsNullOrEmpty(objDTO.SelectedRoomAccessValue))
                    {
                        objSelectedRooms = objRoomsAccessList;
                    }
                    else
                    {
                        string[] RoomAccessIDs = objDTO.SelectedRoomAccessValue.Split(sepForRoleRoom, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < RoomAccessIDs.Length; i++)
                        {
                            string[] RoomIDs = RoomAccessIDs[i].Split('_');
                            long RoomID = 0, CompanyID = 0, EnterpriseID = 0;
                            long.TryParse(RoomIDs[0], out EnterpriseID);
                            long.TryParse(RoomIDs[1], out CompanyID);
                            long.TryParse(RoomIDs[2], out RoomID);

                            if (RoomIDs.Length > 1)
                            {

                                UserWiseRoomsAccessDetailsDTO objRoom = objRoomsAccessList.Find(element => element.EnterpriseId == EnterpriseID && element.CompanyId == CompanyID && element.RoomID == RoomID);
                                if (objRoom != null)
                                {
                                    objSelectedRooms.Add(objRoom);
                                }
                            }
                        }
                    }
                }
                objDTO.UserWiseAllRoomsAccessDetails = objSelectedRooms;
            }
            else
            {

                message = "Room Access is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            string strOK = objCDAL.UserDuplicateCheckUserName(objDTO.ID, objDTO.UserName);
            if (strOK == "duplicate")
            {
                message = "User name \"" + objDTO.UserName + "\" is already exist! Try with Another!";
                status = "duplicate";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (objDTO.ID == 0)
                {
                    objDTO.Room = SessionHelper.RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.EnterpriseId = SessionHelper.EnterPriceID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;

                    objDTO.GUID = Guid.NewGuid();
                    UserMasterDTO objUserMasterDTO = obj.Insert(objDTO);

                    if (objUserMasterDTO.ID > 0)
                    {
                        if (objUserMasterDTO.UserType != 1)
                        {
                            eTurns.DAL.UserMasterDAL objUserMasterDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                            objUserMasterDAL.Insert(objUserMasterDTO);
                        }
                        //Code here insert my string
                        //eTurnsMaster.DAL.UsersUISettingsDAL objUserUiSettingDAL = new eTurnsMaster.DAL.UsersUISettingsDAL(SessionHelper.EnterPriseDBName);
                        //IEnumerable<SiteListMasterDTO> objSiteListMasterDTOList;
                        //SiteListMasterDAL objSiteListMasterDAL = new SiteListMasterDAL(SessionHelper.EnterPriseDBName);
                        //objSiteListMasterDTOList = objSiteListMasterDAL.GetAllItems();
                        //for (int i = 0; i < objSiteListMasterDTOList.Count(); i++)
                        //{
                        //    UsersUISettingsDTO objUserUISettingDTO = new UsersUISettingsDTO();

                        //    SiteListMasterDTO objSiteListMasterDTO = new SiteListMasterDTO();
                        //    //SiteListMasterDAL objSiteListMasterDAL = new SiteListMasterDAL(SessionHelper.EnterPriseDBName);
                        //    objSiteListMasterDTO = objSiteListMasterDAL.GetAllItemsById(Convert.ToInt64(objSiteListMasterDTOList.ToList()[i].ID));
                        //    objUserUISettingDTO.ListName = objSiteListMasterDTO.ListName;
                        //    objUserUISettingDTO.UserID = objUserMasterDTO.ID;
                        //    objUserUISettingDTO.JSONDATA = objSiteListMasterDTOList.ToList()[i].JSONDATA;
                        //    objUserUiSettingDAL.SaveUserListViewSettings(objUserUISettingDTO);
                        //}
                        SendMailToCreateNewUser(objDTO.UserName, passwordBeforeHash, objDTO.Email, objUserMasterDTO);
                        message = "Record Saved Sucessfully...";
                        status = "ok";
                        return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                        return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    //objDTO.Room = SessionHelper.RoomID;
                    //objDTO.CompanyID = SessionHelper.CompanyID;
                    //objDTO.EnterpriseId = SessionHelper.EnterPriceID;

                    //objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;

                    UserMasterDTO objUserMasterDTO = obj.Edit(objDTO);
                    if (objUserMasterDTO.ID > 0)
                    {
                        if (objUserMasterDTO.UserType != 1)
                        {
                            eTurns.DAL.UserMasterDAL objUserMasterDAL;
                            EnterpriseDTO oEnt = new EnterpriseMasterDAL().GetEnterprise(objDTO.EnterpriseId);
                            if (oEnt != null)
                                objUserMasterDAL = new eTurns.DAL.UserMasterDAL(oEnt.EnterpriseDBName);
                            else
                                objUserMasterDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);

                            objUserMasterDAL.Edit(objUserMasterDTO);
                            if (!string.IsNullOrWhiteSpace(objDTO.Password))
                            {
                                UserPasswordChangeHistoryDTO objUserPasswordChangeHistoryDTO = new UserPasswordChangeHistoryDTO();

                                objUserPasswordChangeHistoryDTO.UserId = objDTO.ID;
                                objUserPasswordChangeHistoryDTO.oldPassword = objDTO.Password;

                                objUserPasswordChangeHistoryDTO.LastUpdated = DateTime.UtcNow;
                                eTurnsMaster.DAL.UserMasterDAL objeTurnsMaster = new eTurnsMaster.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                                objeTurnsMaster.SaveChangePassword(objUserPasswordChangeHistoryDTO);
                            }
                        }
                        message = "Record Saved Sucessfully...";
                        status = "ok";
                        Session["SelectedRoomsPermission"] = null;
                        return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        message = "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                        return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            //if (strOK == "duplicate")
            //    {
            //        message = "User Email \"" + objDTO.Email + "\" already exist! Try with Another!";
            //        status = "duplicate";
            //    }



            //if (objDTO.UserType == 1)
            //{
            //    string message = "";
            //    string status = "";
            //    //UserMasterController obj = new UserMasterController();
            //    eTurnsMaster.DAL.UserMasterDAL obj = new eTurnsMaster.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
            //    CommonMasterDAL objCDAL = new CommonMasterDAL();

            //    if (string.IsNullOrEmpty(objDTO.UserName))
            //    {
            //        message = "User name is required.";
            //        status = "fail";
            //        return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            //    }
            //    objDTO.LastUpdatedBy = SessionHelper.UserID;
            //    objDTO.UpdatedByName = SessionHelper.UserName;
            //    objDTO.Room = SessionHelper.RoomID;

            //    //Set Room Replenishment

            //    if (!string.IsNullOrEmpty(objDTO.SelectedRoomReplanishmentValue))
            //    {
            //        List<UserRoomReplanishmentDetailsDTO> objRoomRepIDs = new List<UserRoomReplanishmentDetailsDTO>();

            //        string[] objRoomReplanishmentIDs = objDTO.SelectedRoomReplanishmentValue.Split(',');
            //        for (int i = 0; i < objRoomReplanishmentIDs.Length; i++)
            //        {
            //            UserRoomReplanishmentDetailsDTO objRoleRoom = new UserRoomReplanishmentDetailsDTO();
            //            objRoleRoom.RoomID = int.Parse(objRoomReplanishmentIDs[i].ToString());
            //            objRoomRepIDs.Add(objRoleRoom);
            //        }
            //        objDTO.ReplenishingRooms = objRoomRepIDs;
            //    }
            //    // Set Permissions

            //    if (Session["SelectedUserRoomsPermission"] != null)
            //    {
            //        List<UserWiseRoomsAccessDetailsDTO> objRoomsAccessList = (List<UserWiseRoomsAccessDetailsDTO>)Session["SelectedUserRoomsPermission"];
            //        List<UserWiseRoomsAccessDetailsDTO> objSelectedRooms = new List<UserWiseRoomsAccessDetailsDTO>();
            //        if (objRoomsAccessList != null && objRoomsAccessList.Count > 0)
            //        {
            //            if (string.IsNullOrEmpty(objDTO.SelectedRoomAccessValue))
            //            {

            //                objSelectedRooms = objRoomsAccessList;
            //            }
            //            else
            //            {
            //                string[] RoomAccessIDs = objDTO.SelectedRoomAccessValue.Split(',');
            //                for (int i = 0; i < RoomAccessIDs.Length; i++)
            //                {
            //                    string[] RoomIDs = RoomAccessIDs[i].Split('_');
            //                    if (RoomIDs.Length > 1)
            //                    {
            //                        UserWiseRoomsAccessDetailsDTO objRoom = objRoomsAccessList.Find(element => element.RoomID == int.Parse(RoomIDs[0]));
            //                        if (objRoom != null)
            //                        {
            //                            objSelectedRooms.Add(objRoom);
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        objDTO.UserWiseAllRoomsAccessDetails = objSelectedRooms;
            //    }
            //    else
            //    {

            //        message = "Room Access is required.";
            //        status = "fail";
            //        return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            //    }

            //    if (objDTO.ID == 0)
            //    {
            //        objDTO.CreatedBy = SessionHelper.UserID;
            //        string strOK = objCDAL.DuplicateCheck(objDTO.Email, "add", objDTO.ID, "UserMaster", "Email");
            //        //string strOK = objCDAL.DuplicateCheck(objDTO.Email, "add", objDTO.ID, "UserMaster", "Email", SessionHelper.RoomID, SessionHelper.CompanyID);
            //        if (strOK == "duplicate")
            //        {
            //            message = "User Email \"" + objDTO.Email + "\" already exist! Try with Another!";
            //            status = "duplicate";
            //        }
            //        else
            //        {
            //            string strOK1 = objCDAL.DuplicateCheck(objDTO.Email, "add", objDTO.ID, "UserMaster", "Email");
            //            //string strOK1 = objCDAL.DuplicateCheck(objDTO.UserName, "add", objDTO.ID, "UserMaster", "UserName", SessionHelper.RoomID, SessionHelper.CompanyID);
            //            if (strOK1 == "duplicate")
            //            {
            //                message = "User Name \"" + objDTO.UserName + "\" already exist! Try with Another!";
            //                status = "duplicate";
            //            }
            //            else
            //            {
            //                objDTO.GUID = Guid.NewGuid();
            //                long ReturnVal = obj.Insert(objDTO);
            //                if (ReturnVal > 0)
            //                {
            //                    message = "Record Saved Sucessfully...";
            //                    status = "ok";
            //                }
            //                else
            //                {
            //                    message = "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
            //                    status = "fail";
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        objDTO.LastUpdatedBy = SessionHelper.UserID;
            //        string strOK = objCDAL.DuplicateCheck(objDTO.Email, "edit", objDTO.ID, "UserMaster", "Email");
            //        //string strOK = objCDAL.DuplicateCheck(objDTO.Email, "edit", objDTO.ID, "UserMaster", "Email", SessionHelper.RoomID, SessionHelper.CompanyID);
            //        if (strOK == "duplicate")
            //        {
            //            //message = "Role Name \"" + objDTO.UserName + "\" already exist! Try with Another!";
            //            message = "User Email \"" + objDTO.Email + "\" already exist! Try with Another!";
            //            status = "duplicate";
            //        }
            //        else
            //        {
            //            string strOK1 = objCDAL.DuplicateCheck(objDTO.Email, "edit", objDTO.ID, "UserMaster", "Email");
            //            //string strOK1 = objCDAL.DuplicateCheck(objDTO.UserName, "edit", objDTO.ID, "UserMaster", "UserName", SessionHelper.RoomID, SessionHelper.CompanyID);
            //            if (strOK1 == "duplicate")
            //            {
            //                message = "User Name \"" + objDTO.UserName + "\" already exist! Try with Another!";
            //                status = "duplicate";
            //            }
            //            else
            //            {

            //                bool ReturnVal = obj.Edit(objDTO);
            //                if (ReturnVal)
            //                {
            //                    message = "Record Saved Sucessfully...";
            //                    status = "ok";
            //                    Session["SelectedRoomsPermission"] = null;
            //                }
            //                else
            //                {
            //                    message = "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
            //                    status = "fail";
            //                }
            //            }
            //        }
            //    }

            //    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    string message = "";
            //    string status = "";
            //    //UserMasterController obj = new UserMasterController();
            //    eTurns.DAL.UserMasterDAL obj = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
            //    CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

            //    if (string.IsNullOrEmpty(objDTO.UserName))
            //    {
            //        message = "User name is required.";
            //        status = "fail";
            //        return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            //    }
            //    objDTO.LastUpdatedBy = SessionHelper.UserID;
            //    objDTO.UpdatedByName = SessionHelper.UserName;
            //    objDTO.Room = SessionHelper.RoomID;

            //    //Set Room Replenishment

            //    if (!string.IsNullOrEmpty(objDTO.SelectedRoomReplanishmentValue))
            //    {
            //        List<UserRoomReplanishmentDetailsDTO> objRoomRepIDs = new List<UserRoomReplanishmentDetailsDTO>();

            //        string[] objRoomReplanishmentIDs = objDTO.SelectedRoomReplanishmentValue.Split(',');
            //        for (int i = 0; i < objRoomReplanishmentIDs.Length; i++)
            //        {
            //            UserRoomReplanishmentDetailsDTO objRoleRoom = new UserRoomReplanishmentDetailsDTO();
            //            objRoleRoom.RoomID = int.Parse(objRoomReplanishmentIDs[i].ToString());
            //            objRoomRepIDs.Add(objRoleRoom);
            //        }
            //        objDTO.ReplenishingRooms = objRoomRepIDs;
            //    }
            //    // Set Permissions

            //    if (Session["SelectedUserRoomsPermission"] != null)
            //    {
            //        List<UserWiseRoomsAccessDetailsDTO> objRoomsAccessList = (List<UserWiseRoomsAccessDetailsDTO>)Session["SelectedUserRoomsPermission"];
            //        List<UserWiseRoomsAccessDetailsDTO> objSelectedRooms = new List<UserWiseRoomsAccessDetailsDTO>();
            //        if (objRoomsAccessList != null && objRoomsAccessList.Count > 0)
            //        {
            //            if (string.IsNullOrEmpty(objDTO.SelectedRoomAccessValue))
            //            {

            //                objSelectedRooms = objRoomsAccessList;
            //            }
            //            else
            //            {
            //                string[] RoomAccessIDs = objDTO.SelectedRoomAccessValue.Split(',');
            //                for (int i = 0; i < RoomAccessIDs.Length; i++)
            //                {
            //                    string[] RoomIDs = RoomAccessIDs[i].Split('_');
            //                    if (RoomIDs.Length > 1)
            //                    {
            //                        UserWiseRoomsAccessDetailsDTO objRoom = objRoomsAccessList.Find(element => element.RoomID == int.Parse(RoomIDs[0]));
            //                        if (objRoom != null)
            //                        {
            //                            objSelectedRooms.Add(objRoom);
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        objDTO.UserWiseAllRoomsAccessDetails = objSelectedRooms;
            //    }
            //    else
            //    {

            //        message = "Room Access is required.";
            //        status = "fail";
            //        return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            //    }

            //    if (objDTO.ID == 0)
            //    {
            //        objDTO.CreatedBy = SessionHelper.UserID;

            //        string strOK = objCDAL.DuplicateCheck(objDTO.Email, "add", objDTO.ID, "UserMaster", "Email", SessionHelper.RoomID, SessionHelper.CompanyID);
            //        if (strOK == "duplicate")
            //        {
            //            message = "User Email \"" + objDTO.Email + "\" already exist! Try with Another!";
            //            status = "duplicate";
            //        }
            //        else
            //        {
            //            string strOK1 = objCDAL.DuplicateCheck(objDTO.UserName, "add", objDTO.ID, "UserMaster", "UserName", SessionHelper.RoomID, SessionHelper.CompanyID);
            //            if (strOK1 == "duplicate")
            //            {
            //                message = "User Name \"" + objDTO.UserName + "\" already exist! Try with Another!";
            //                status = "duplicate";
            //            }
            //            else
            //            {
            //                objDTO.GUID = Guid.NewGuid();
            //                long ReturnVal = obj.Insert(objDTO);
            //                if (ReturnVal > 0)
            //                {
            //                    message = "Record Saved Sucessfully...";
            //                    status = "ok";
            //                }
            //                else
            //                {
            //                    message = "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
            //                    status = "fail";
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        objDTO.LastUpdatedBy = SessionHelper.UserID;
            //        string strOK = objCDAL.DuplicateCheck(objDTO.Email, "edit", objDTO.ID, "UserMaster", "Email", SessionHelper.RoomID, SessionHelper.CompanyID);
            //        if (strOK == "duplicate")
            //        {
            //            //message = "Role Name \"" + objDTO.UserName + "\" already exist! Try with Another!";
            //            message = "User Email \"" + objDTO.Email + "\" already exist! Try with Another!";
            //            status = "duplicate";
            //        }
            //        else
            //        {
            //            string strOK1 = objCDAL.DuplicateCheck(objDTO.UserName, "edit", objDTO.ID, "UserMaster", "UserName", SessionHelper.RoomID, SessionHelper.CompanyID);
            //            if (strOK1 == "duplicate")
            //            {
            //                message = "User Name \"" + objDTO.UserName + "\" already exist! Try with Another!";
            //                status = "duplicate";
            //            }
            //            else
            //            {

            //                bool ReturnVal = obj.Edit(objDTO);
            //                if (ReturnVal)
            //                {
            //                    message = "Record Saved Sucessfully...";
            //                    status = "ok";
            //                    Session["SelectedRoomsPermission"] = null;
            //                }
            //                else
            //                {
            //                    message = "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
            //                    status = "fail";
            //                }
            //            }
            //        }
            //    }

            //    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            //}


        }
        public ActionResult Hashtest()
        {
            return View();
        }


        public void SendMailToCreateNewUser(string UserName, string Password, string ToEmailAddress, UserMasterDTO objUserMasterDTO)
        {
            eTurnsUtility objUtils = null;
            StringBuilder MessageBody = null;
            EmailTemplateDAL objEmailTemplateDAL = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
            eTurnsMaster.DAL.eMailDAL objEmailDAL = null;
            try
            {
                string StrSubject = ResOrder.OrderApprovalleMailSubject;
                string strTemplateName = "CreateNewUser";

                string strToAddress = ToEmailAddress;
                if (!string.IsNullOrEmpty(strToAddress))
                {
                    string strCCAddress = "";
                    MessageBody = new StringBuilder();

                    objEmailTemplateDAL = new EmailTemplateDAL(SessionHelper.EnterPriseDBName);
                    objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();
                    if (objUserMasterDTO.UserType != 1) // For usertype = Enterprise Admin, Company Admin
                    {
                        // Get Email template which is not depends on room and company
                        objEmailTemplateDetailDTO = objEmailTemplateDAL.GetEmailTemplate(strTemplateName, ResourceHelper.CurrentCult.ToString(), 0, 0);
                    }
                    else // For Super Admin type user
                    {
                        if (!string.IsNullOrWhiteSpace(objUserMasterDTO.SelectedEnterpriseAccessValue))
                        {
                            string[] arrstr = objUserMasterDTO.SelectedEnterpriseAccessValue.Split(sepForRoleRoom, StringSplitOptions.RemoveEmptyEntries);
                            EnterpriseMasterDAL objEnterpriseDAL = new EnterpriseMasterDAL();
                            long enterId = 0;
                            string EnterpriseDBName = string.Empty;
                            foreach (string epid in arrstr)
                            {
                                EnterpriseDBName = string.Empty;
                                if (long.TryParse(epid, out enterId))
                                {
                                    EnterpriseDTO objEnterpriseDTO = objEnterpriseDAL.GetEnterprise(enterId);
                                    if (objEnterpriseDTO != null)
                                    {
                                        EnterpriseDBName = objEnterpriseDTO.EnterpriseDBName;
                                    }
                                }

                                if (!string.IsNullOrWhiteSpace(EnterpriseDBName))
                                {
                                    objEmailTemplateDAL = new EmailTemplateDAL(EnterpriseDBName);
                                    objEmailTemplateDetailDTO = objEmailTemplateDAL.GetEmailTemplate(strTemplateName, ResourceHelper.CurrentCult.ToString(), 0, 0);
                                }

                                if (objEmailTemplateDetailDTO != null && !string.IsNullOrWhiteSpace(objEmailTemplateDetailDTO.MailBodyText))
                                {
                                    break;
                                }
                            }
                        }
                    }

                    if (objEmailTemplateDetailDTO == null || string.IsNullOrWhiteSpace(objEmailTemplateDetailDTO.MailBodyText))
                    {
                        return;
                    }

                    MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
                    StrSubject = objEmailTemplateDetailDTO.MailSubject;

                    string strPath = string.Empty;
                    string strReplacepath = "";
                    if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Request != null)
                    {
                        strPath = System.Web.HttpContext.Current.Request.Url.ToString();
                        strReplacepath = System.Web.HttpContext.Current.Request.Url.PathAndQuery;
                        strPath = strPath.Replace(strReplacepath, "/");
                    }
                    else if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["DomainName"]))
                    {
                        strPath = System.Configuration.ConfigurationManager.AppSettings["DomainName"];
                    }
                    MessageBody.Replace("@@username@@", UserName);
                    MessageBody.Replace("@@password@@", Password);
                    MessageBody.Replace("@@passwordrules@@", ResUserMaster.lblPasswordRules);
                    //MessageBody = MessageBody.Replace("@@ETURNSLOGO@@", GeteTurnsImage(strPath, "Content/images/logo.jpg"));
                    //MessageBody = MessageBody.Replace("@@Year@@", DateTime.Now.Year.ToString());
                    MessageBody = MessageBody.Replace("/CKUpload/", strPath + "CKUpload/");

                    string urlPart = Request.Url.ToString();
                    string replacePart = urlPart.Split('/')[0] + "//" + urlPart.Split('/')[2];

                    objUtils = new eTurnsUtility();
                    objEmailDAL = new eTurnsMaster.DAL.eMailDAL();
                    objEmailDAL.eMailToSend(strToAddress, strCCAddress, StrSubject, MessageBody.ToString(), SessionHelper.EnterPriceID, objUserMasterDTO.CompanyID, objUserMasterDTO.Room.GetValueOrDefault(0), SessionHelper.UserID, null, "Web => EmailLink => SendMailToCreateNewUser");
                }
            }
            finally
            {
                objUtils = null;
                objEmailDAL = null;
                MessageBody = null;
                objEmailTemplateDAL = null;
                objEmailTemplateDetailDTO = null;
            }
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult UserLogin()
        {

            //DateTime StartDate = new DateTime(2014, 1, 1);
            //DateTime EndDate = new DateTime(2015, 12, 31);
            //List<SelectListItem> lst = new List<SelectListItem>();
            //while (StartDate <= EndDate)
            //{
            //    SelectListItem obj = new SelectListItem();
            //    obj.Text = StartDate.ToShortDateString();
            //    obj.Value = Convert.ToString(TimeZone.CurrentTimeZone.GetUtcOffset(StartDate).TotalMinutes);
            //    lst.Add(obj);
            //    StartDate = StartDate.AddDays(1);
            //}
            //ViewBag.dates = lst;
            //
            //List<ASD> lst = new List<ASD>();
            //for (int i = 0; i < 5; i++)
            //{
            //    ASD objASD = new ASD();
            //    objASD.ID = i;
            //    objASD.ItemGuid = Guid.NewGuid();
            //    objASD.ItemNumber = "ItemNumber" + i;
            //    List<ASDN> lstdtl = new List<ASDN>();

            //    for (int j = 0; j < 5; j++)
            //    {
            //        ASDN objASDN = new ASDN();
            //        objASDN.ConsignedQty = j * 20;
            //        objASDN.CustOwnedQty = j * 30;
            //        objASDN.ID = i * j;
            //        objASDN.lotNumber = "lotNumber " + i.ToString() + j.ToString();
            //        objASDN.ReceivedDate = DateTime.Now.AddDays(i + j);
            //        objASDN.SerialNumber = "SerialNumber " + i.ToString() + j.ToString();
            //        lstdtl.Add(objASDN);
            //    }
            //    objASD.lstASDN = lstdtl;
            //    lst.Add(objASD);
            //}
            //string strSerialized = Newtonsoft.Json.JsonConvert.SerializeObject(lst);
            //strSerialized = strSerialized;
            //HttpWebRequest webRequest = null;
            //string requestUri = string.Format("http://202.131.117.224:2020/GenerateCache/GenerateCacheCompany?UserID={0}&CompanyId={1}", 13, 1);
            //webRequest = (HttpWebRequest)WebRequest.Create(requestUri.ToString());
            //// Set the method type
            //webRequest.Method = "GET";
            //webRequest.Accept = "application/atom+xml";
            //// webRequest.ContentType = "application/atom+xml";objConfig-www-form-urlencoded
            //webRequest.KeepAlive = false;
            //webRequest.ContentType = "application/atom+xml";
            ////webRequest.ContentLength = 23750;

            ////webRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0; SLCC1; .NET CLR 2.0.50727; InfoPath.2; .NET CLR 3.5.21022;";
            //webRequest.Timeout = 100000000;
            //// Get the response

            //HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

            //if (webResponse.StatusCode == HttpStatusCode.OK)
            //{ 

            //}

            string msg = string.Empty;

            ViewBag.LoginWithSelection = "0";
            ViewBag.IsMasterDbLogin = "0";
            if (ConfigurationManager.AppSettings.AllKeys.Contains("loginwithselection"))
            {
                ViewBag.LoginWithSelection = ConfigurationManager.AppSettings["loginwithselection"];
            }

            if (ConfigurationManager.AppSettings.AllKeys.Contains("EnableMasterLogin"))
            {
                ViewBag.IsMasterDbLogin = ConfigurationManager.AppSettings["EnableMasterLogin"];
            }
            XElement Settinfile = XElement.Load(Server.MapPath("/SiteSettings.xml"));
            string LoginWithSelection = Settinfile.Element("LoginWithSelection").Value;
            int LoginWithUserSelection = 0;
            if (Convert.ToInt32(Request.QueryString["LoginWithUserSelection"]) == 1)
            {
                LoginWithUserSelection = 1;
            }

            if (LoginWithSelection == "1" && LoginWithUserSelection == 1)
            {
                ViewBag.LoginWithSelection = LoginWithSelection;
            }
            else
            {
                ViewBag.LoginWithSelection = "0";
            }

            EnterPriseUserMasterDAL objEnterPriseUserMasterDAL = new EnterPriseUserMasterDAL(SessionHelper.EnterPriseDBName);
            //UserMasterController objUserMasterController = new UserMasterController();
            List<UserMasterDTO> lstUsers = null;
            if (ViewBag.IsMasterDbLogin == "1")
            {
                lstUsers = objEnterPriseUserMasterDAL.GetAllUsers().OrderBy(o => o.UserName).ToList();

            }
            else
            {
                eTurns.DAL.UserMasterDAL objUserMasterController = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                lstUsers = objUserMasterController.GetAllUsers().ToList();

            }
            //lstUsers.ForEach(t =>
            //{
            //    t.Password = CommonUtility.getSHA15Hash(t.Password);
            //});
            ViewBag.AllUsers = lstUsers.ToList();
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult UserLogin(LoginModel objLoginModel)
        {
            string loginwithselection = Convert.ToString(ConfigurationManager.AppSettings["loginwithselection"]);
            bool isvalidcapcha = this.IsCaptchaValid("Captcha is not valid");
            //if (loginwithselection == "1")
            //{
            isvalidcapcha = true;
            //}

            if (isvalidcapcha)
            {
                UserBAL objUserBAL = new UserBAL();
                EnterPriseUserMasterDAL objEnterPriseUserMasterDAL = new EnterPriseUserMasterDAL(SessionHelper.EnterPriseDBName);
                EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
                UserMasterDTO objDTO = objEnterPriseUserMasterDAL.CheckAuthanticationUserName(objLoginModel.Email, objLoginModel.Password);
                bool IsUserLocked = objUserBAL.CheckSetAcountLockout(objLoginModel.Email, objLoginModel.Password, objDTO);
                eTurnsMaster.DAL.UserMasterDAL objUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                long PermissionCount = 1;
                if (objDTO != null)
                {
                    if (objDTO.RoleID > 0)
                    {
                        List<eTurnsMaster.DAL.UserRoomAccess> lstAccess = objEnterPriseUserMasterDAL.getUserPermission(objDTO.ID);
                        if (lstAccess == null || lstAccess.Count == 0)
                        {
                            PermissionCount = 0;
                        }
                        else
                        {
                            PermissionCount = lstAccess.Count();
                        }
                    }
                    if (PermissionCount > 0)
                    {
                        if (!IsUserLocked)
                        {
                            bool IsPasswordExpired = false;

                            FormsAuthentication.SetAuthCookie(objDTO.UserName, false);

                            //HttpCookie remUserName = new HttpCookie("eturnsUserName");
                            //HttpCookie remPassword = new HttpCookie("eturnsPassword");
                            //remUserName.HttpOnly = false;
                            //remPassword.HttpOnly = false;
                            //if (objLoginModel.IsRemember)
                            //{
                            //    remUserName.Value = objLoginModel.Email;
                            //    remPassword.Value = objLoginModel.Password; //encryption is required here   
                            //}
                            //else
                            //{
                            //    remUserName.Value = string.Empty;
                            //    remPassword.Value = string.Empty;
                            //}
                            //remUserName.Expires = DateTimeUtility.DateTimeNow.AddDays(15);
                            //remPassword.Expires = DateTimeUtility.DateTimeNow.AddDays(15);
                            //if (Response != null && Response.Cookies != null)
                            //{

                            //    Response.Cookies.Add(remUserName);
                            //    Response.Cookies.Add(remPassword);
                            //}

                            SessionHelper.UserID = objDTO.ID;
                            SessionHelper.RoleID = objDTO.RoleID;
                            SessionHelper.UserType = objDTO.UserType;
                            SessionHelper.EnterPriceID = objDTO.EnterpriseId;
                            SessionHelper.CompanyID = objDTO.CompanyID;
                            //SessionHelper.RoomID = objDTO.Room ?? 0;
                            SessionHelper.UserName = objDTO.UserName;
                            SessionHelper.LoggedinUser = objDTO;
                            SessionHelper.IsLicenceAccepted = objDTO.IsLicenceAccepted;

                            SessionHelper.HasPasswordChanged = objDTO.HasChangedFirstPassword;
                            SessionHelper.NewEulaAccept = objDTO.NewEulaAccept;
                            SetSessions(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.UserType, SessionHelper.RoleID, "onlogin", SessionHelper.EnterPriceName, SessionHelper.CompanyName, SessionHelper.RoomName);
                            SessionHelper.AnotherLicenceAccepted = objDTO.IsLicenceAccepted;
                            if (objDTO.LastLicenceAccept == null)
                            {
                                SessionHelper.IsLicenceAccepted = false;
                                SessionHelper.AnotherLicenceAccepted = false;
                                SessionHelper.NewEulaAccept = false;
                            }
                            XElement Settinfile = XElement.Load(Server.MapPath("/SiteSettings.xml"));
                            string AcceptLicence = Settinfile.Element("AcceptLicence").Value;
                            if (AcceptLicence == "1")
                            {
                                EnterPriseConfigDTO objEnterpriseConfig = new EnterPriseConfigDTO();
                                EnterPriseConfigDAL objEnterPriseConfigDAL = new EnterPriseConfigDAL(SessionHelper.EnterPriseDBName);
                                if (SessionHelper.EnterPriceID > 0 && objDTO.UserType != 1)
                                {
                                    objEnterpriseConfig = objEnterPriseConfigDAL.GetRecord(objDTO.EnterpriseId);
                                    if (objDTO.EnterpriseId > 0)
                                    {
                                        if (objDTO.IsLicenceAccepted && (objEnterpriseConfig.DisplayAgreePopupDays < objDTO.DaysRemains))
                                        {
                                            SessionHelper.AnotherLicenceAccepted = false;
                                        }
                                    }
                                }
                            }
                            if (SessionHelper.EnterPriceID > 0 && SessionHelper.CompanyID > 0 && SessionHelper.RoomID > 0)
                            {


                            }
                            IsPasswordExpired = CheckpasswordExpired();
                            if (IsPasswordExpired)
                            {
                                SessionHelper.HasPasswordChanged = false;
                            }
                            //ResourceHelper.CompanyResourceFolder = SessionHelper.EnterPriceID.ToString() + "_" + SessionHelper.CompanyID.ToString();
                            //if (eTurns.DTO.Resources.ResourceHelper.CurrentCult == null)
                            //    eTurns.DTO.Resources.ResourceHelper.CurrentCult = System.Threading.Thread.CurrentThread.CurrentCulture;
                            //return Json(new { Message = "success", Status = "ok", HasLicenceAccepted = objDTO.IsLicenceAccepted }, JsonRequestBehavior.AllowGet);
                            ViewBag.Status = "ok";
                            ViewBag.Message = "success";

                            // check user default redirect page
                            //UserSettingDTO objUserSettingDTO = new UserSettingDTO();
                            //UserSettingDAL objUserSettingDAL = new UserSettingDAL(SessionHelper.EnterPriseDBName);
                            //objUserSettingDTO = objUserSettingDAL.GetByUserId(SessionHelper.UserID);
                            //if (objUserSettingDTO != null)
                            //{
                            //    if (objUserSettingDTO.IsRemember)
                            //    {
                            //        return Redirect(objUserSettingDTO.RedirectURL);
                            //    }
                            //}


                            string retURL = FormsAuthentication.GetRedirectUrl(objLoginModel.Email, false);
                            if (string.IsNullOrWhiteSpace(retURL))
                            {
                                retURL = objLoginModel.ReturnUrl;
                            }
                            if (!string.IsNullOrWhiteSpace(retURL) && retURL != "/")
                            {
                                try
                                {
                                    Uri uri = new Uri(Request.Url.ToString());
                                    string requested = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;
                                    requested = requested + retURL;
                                    uri = new Uri(requested);
                                    if (string.IsNullOrWhiteSpace(uri.Query))
                                    {
                                        return Redirect(retURL + "?FromLogin=yes");
                                    }
                                    else
                                    {
                                        return Redirect(retURL + "&FromLogin=yes");
                                    }
                                }
                                catch (Exception)
                                {
                                    if (retURL.IndexOf('?') >= 0)
                                    {
                                        return Redirect(retURL + "&FromLogin=yes");
                                    }
                                    else
                                    {
                                        return Redirect(retURL + "?FromLogin=yes");
                                    }

                                }
                            }
                            else
                            {
                                eTurnsMaster.DAL.UserSettingDAL objUserSettingDAL = new eTurnsMaster.DAL.UserSettingDAL(SessionHelper.EnterPriseDBName);
                                eTurns.DTO.UserSettingDTO objUserSettingDTO = objUserSettingDAL.GetByUserId(objDTO.ID);
                                if (objUserSettingDTO == null || (string.IsNullOrEmpty(objUserSettingDTO.RedirectURL)))
                                {
                                    string CtrlName = Convert.ToString(ConfigurationManager.AppSettings["CtrlName"]);
                                    string ActName = Convert.ToString(ConfigurationManager.AppSettings["ActName"]);
                                    return RedirectToAction(ActName, CtrlName, new { FromLogin = "yes" });
                                }
                                else
                                {
                                    return Redirect(objUserSettingDTO.RedirectURL + "?FromLogin=yes");
                                }
                            }
                        }
                        else
                        {
                            ViewBag.Status = "fail";
                            ViewBag.Message = "locked";
                            return View();
                            //return Json(new { Message = "fail", Status = "locked" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        ViewBag.Status = "fail";
                        ViewBag.Message = "Atleast one room should be assigned to you to get in.";
                        return View();
                    }
                }
                else
                {
                    ViewBag.Status = "fail";
                    ViewBag.Message = "fail";
                    return View();
                    //return Json(new { Message = "fail", Status = "locked" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                ViewBag.Status = "fail";
                ViewBag.Message = "Invalid capcha";
                return View();
                //return Json(new { Message = "fail", Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Updatepassword()
        {
            using (var context = new eTurns_MasterEntities())
            {
                var AllUsers = context.UserMasters.AsQueryable<eTurnsMaster.DAL.UserMaster>();
                foreach (var item in AllUsers)
                {
                    item.Password = CommonUtility.getSHA15Hash(item.Password);
                }
                context.SaveChanges();
            }
            return RedirectToAction("UserLogin", "Master");
        }

        private bool CheckpasswordExpired()
        {
            if (SessionHelper.UserType == 1)
                return false;
            bool NeedResetPassword = SessionHelper.GetAdminPermission(SessionHelper.ModuleList.PasswordResetRule);
            if (NeedResetPassword)
            {

                eTurnsMaster.DAL.UserMasterDAL objeTurnsMaster = new eTurnsMaster.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                EnterPriseConfigDAL objDAL = new EnterPriseConfigDAL(SessionHelper.EnterPriseDBName);
                EnterPriseConfigDTO objDTO = objDAL.GetRecord(SessionHelper.EnterPriceID);
                int ExpiryDays = 0;
                if (objDTO != null)
                {
                    ExpiryDays = objDTO.PasswordExpiryDays ?? 0;
                }

                if (objeTurnsMaster.CheckPasswordChange(SessionHelper.UserID, ExpiryDays))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }






        public ActionResult LogoutUser(string CompanyID)
        {
            eTurnsMaster.DAL.UserMasterDAL objUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
            UserLoginHistoryDTO objUserLoginHistoryDTO = new UserLoginHistoryDTO();
            objUserLoginHistoryDTO.CompanyId = SessionHelper.CompanyID;
            objUserLoginHistoryDTO.EnterpriseId = SessionHelper.EnterPriceID;
            objUserLoginHistoryDTO.EventDate = DateTimeUtility.DateTimeNow;
            objUserLoginHistoryDTO.EventType = 2;
            objUserLoginHistoryDTO.ID = 0;
            objUserLoginHistoryDTO.IpAddress = string.Empty;
            objUserLoginHistoryDTO.RoomId = SessionHelper.RoomID;
            objUserLoginHistoryDTO.UserId = SessionHelper.UserID;
            objUserMasterDAL.SaveUserActions(objUserLoginHistoryDTO);
            //string lastURL = Request.UrlReferrer.AbsolutePath;
            //Int64 CurrentUserId = SessionHelper.UserID;
            //eTurnsMaster.DAL.UserSettingDAL objUserSettingDAL = new eTurnsMaster.DAL.UserSettingDAL(SessionHelper.EnterPriseDBName);
            //eTurns.DTO.UserSettingDTO objUserSettingDTO = objUserSettingDAL.GetByUserId(CurrentUserId);
            //if (objUserSettingDTO == null || (string.IsNullOrEmpty(objUserSettingDTO.RedirectURL)))
            //{
            //    objUserSettingDTO = new UserSettingDTO();
            //    objUserSettingDTO.UserId = CurrentUserId;
            //    objUserSettingDTO.RedirectURL = lastURL;
            //    objUserSettingDTO.IsRemember = true;
            //    objUserSettingDAL.Insert(objUserSettingDTO);
            //}
            //else
            //{
            //    objUserSettingDTO.RedirectURL = lastURL;
            //    objUserSettingDAL.Update(objUserSettingDTO);
            //}
            SessionHelper.CompanyID = 0;
            SessionHelper.CompanyList = null;
            SessionHelper.CompanyName = null;
            SessionHelper.EnterPriceID = 0;
            SessionHelper.EnterPriceName = null;
            SessionHelper.EnterPriseDBConnectionString = null;
            SessionHelper.EnterPriseDBName = null;
            SessionHelper.EnterPriseList = null;
            SessionHelper.LoggedinUser = null;
            SessionHelper.RoleID = 0;
            SessionHelper.RoomID = 0;
            SessionHelper.RoomList = null;
            SessionHelper.RoomName = null;
            SessionHelper.RoomPermissions = null;
            SessionHelper.UserID = 0;
            SessionHelper.UserType = 0;
            SessionHelper.UserName = null;
            Session.Abandon();
            Session.RemoveAll();

            FormsAuthentication.SignOut();
            return RedirectToAction("UserLogin", "Master");
            //return Json(new { Message = "success", Status = "ok" }, JsonRequestBehavior.AllowGet);

        }

        //public JsonResult UserLoginAuthanticate(string Email, string Password, bool IsRemember)
        //{

        //    //UserMasterController obj = new UserMasterController();
        //    eTurns.DAL.UserMasterDAL obj = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
        //    UserMasterDTO objDTO = obj.CheckAuthantication(Email, Password);
        //    if (objDTO != null)
        //    {

        //        /*START - SET COOKIE */
        //        HttpCookie remUserName = new HttpCookie("UserName");
        //        //HttpCookie remPassword = new HttpCookie("Password");
        //        if (IsRemember)
        //        {
        //            remUserName.Value = Email;
        //            //remPassword.Value = Password; //encryption is required here   
        //        }
        //        else
        //        {
        //            remUserName.Value = string.Empty;
        //            //remPassword.Value = string.Empty;
        //        }
        //        remUserName.Expires = DateTime.Now.AddDays(30);
        //        //remPassword.Expires = DateTime.Now.AddDays(15);
        //        Response.Cookies.Add(remUserName);
        //        //Response.Cookies.Add(remPassword);
        //        /*END - SET COOKIE */

        //        SessionHelper.UserID = objDTO.ID;
        //        SessionHelper.UserName = objDTO.UserName;
        //        SessionHelper.CompanyID = objDTO.CompanyID;
        //        SessionHelper.RoleID = objDTO.RoleID;
        //        SessionHelper.RoomPermissions = objDTO.UserWiseAllRoomsAccessDetails;
        //        List<KeyValuePair<long, string>> TempRoomList = null;

        //        if (!string.IsNullOrEmpty(objDTO.SelectedRoomAccessValue))
        //        {
        //            TempRoomList = new List<KeyValuePair<long, string>>();

        //            string[] RoomAccessIDs = objDTO.SelectedRoomAccessValue.Split(',');
        //            for (int i = 0; i < RoomAccessIDs.Length; i++)
        //            {
        //                string[] RoomIDs = RoomAccessIDs[i].Split('_');
        //                if (RoomIDs.Length > 1)
        //                {
        //                    TempRoomList.Add(new KeyValuePair<long, string>(int.Parse(RoomIDs[0]), RoomIDs[1]));
        //                }
        //            }
        //            SessionHelper.RoomList = TempRoomList;
        //            if (SessionHelper.RoomList != null)
        //            {
        //                SessionHelper.RoomID = (int)SessionHelper.RoomList[0].Key;
        //                SessionHelper.RoomName = SessionHelper.RoomList[0].Value;
        //            }
        //        }

        //        return Json(new { Message = "success", Status = "ok" }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(new { Message = "fail", Status = "fail" }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        public JsonResult UserLoginAuthanticateMasterDB(string Email, string Password, bool IsRemember)
        {
            EnterPriseUserMasterDAL objEnterPriseUserMasterDAL = new EnterPriseUserMasterDAL(SessionHelper.EnterPriseDBName);
            EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
            UserMasterDTO objDTO = objEnterPriseUserMasterDAL.CheckAuthantication(Email, Password);
            if (objDTO != null)
            {
                try
                {
                    /*START - SET COOKIE */
                    HttpCookie remUserName = new HttpCookie("UserName");
                    HttpCookie remPassword = new HttpCookie("Password");
                    if (IsRemember)
                    {
                        remUserName.Value = Email;
                        remPassword.Value = Password; //encryption is required here   
                    }
                    else
                    {
                        remUserName.Value = string.Empty;
                        remPassword.Value = string.Empty;
                    }
                    remUserName.Expires = DateTimeUtility.DateTimeNow.AddYears(1);
                    remPassword.Expires = DateTimeUtility.DateTimeNow.AddYears(1);
                    Response.Cookies.Add(remUserName);
                    Response.Cookies.Add(remPassword);
                    /*END - SET COOKIE */
                }
                catch (Exception)
                {

                }

                SessionHelper.EnterPriseDBName = objDTO.EnterpriseDbName;
                SessionHelper.UserID = objDTO.ID;
                SessionHelper.RoleID = objDTO.RoleID;
                SessionHelper.UserName = objDTO.UserName;
                SessionHelper.CompanyID = objDTO.CompanyID;
                SessionHelper.EnterPriceID = objDTO.EnterpriseId;
                SessionHelper.UserType = objDTO.UserType;

                if (SessionHelper.UserType == 1)
                {
                    SessionHelper.EnterPriseList = objEnterpriseMasterDAL.GetEnterPriseByUser(objDTO.ID, objDTO.RoleID, SessionHelper.UserType);
                    if (SessionHelper.EnterPriseList != null && SessionHelper.EnterPriseList.Count > 0)
                    {
                        CompanyMasterDAL objCompanyMasterDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
                        SessionHelper.EnterPriceID = SessionHelper.EnterPriseList.First().ID;
                        SessionHelper.EnterPriseDBName = SessionHelper.EnterPriseList.First().EnterpriseDBName;

                        if (objDTO.RoleID == -1)
                        {
                            SessionHelper.CompanyList = objCompanyMasterDAL.GetAllRecords().ToList();
                        }
                        else
                        {
                            List<long> lstcompany = objEnterpriseMasterDAL.GetDistinctMasterCompanyID(objDTO.ID, objDTO.RoleID, SessionHelper.UserType, SessionHelper.EnterPriceID);

                            List<CompanyMasterDTO> lstFinal = new List<CompanyMasterDTO>();
                            //lstFinal = objCompanyMasterDAL.GetAllRecords().ToList();
                            lstFinal = (from s in objCompanyMasterDAL.GetAllRecords().ToList()
                                        where lstcompany.Contains(s.ID)
                                        select s).ToList();

                            SessionHelper.CompanyList = lstFinal;// objCompanyMasterDAL.GetAllRecords().ToList();
                        }

                        if (SessionHelper.CompanyList != null && SessionHelper.CompanyList.Count > 0)
                        {
                            SessionHelper.CompanyID = SessionHelper.CompanyList.First().ID;
                            SessionHelper.CompanyName = SessionHelper.CompanyList.First().Name;


                            List<RoomDTO> lstRoomDTO = new List<RoomDTO>();
                            RoomDAL objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
                            if (objDTO.RoleID == -1)
                            {
                                lstRoomDTO = objRoomDAL.GetAllRecords(SessionHelper.CompanyID, false, false).ToList();
                            }
                            else
                            {

                                List<long> lstdistinctRoom = objEnterpriseMasterDAL.GetDistinctMasterRoomID(objDTO.ID, objDTO.RoleID, SessionHelper.UserType, SessionHelper.EnterPriceID, SessionHelper.CompanyID);
                                lstRoomDTO = (from s in objRoomDAL.GetAllRecords(SessionHelper.CompanyID, false, false).ToList()
                                              where lstdistinctRoom.Contains(s.ID)
                                              select s).ToList();
                            }

                            //List<KeyValuePair<long, string>> TempRoomList = new List<KeyValuePair<long, string>>();
                            //foreach (var item in lstRoomDTO)
                            //{
                            //    TempRoomList.Add(new KeyValuePair<long, string>(item.ID, item.RoomName));
                            //}
                            SessionHelper.RoomList = lstRoomDTO;
                            if (lstRoomDTO != null && lstRoomDTO.Count > 0)
                            {
                                SessionHelper.RoomID = lstRoomDTO.First().ID;
                                SessionHelper.RoomName = lstRoomDTO.First().RoomName;

                            }
                        }
                        else
                        {
                            SessionHelper.RoomID = 0;
                            SessionHelper.RoomName = "";
                        }

                        eTurnsMaster.DAL.UserMasterDAL objUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                        objDTO.PermissionList = objUserMasterDAL.GetUserRoleModuleDetailsRecord(objDTO.ID, objDTO.RoleID, SessionHelper.UserType);
                        if (objDTO.RoleID == -1)
                        {
                            objDTO.PermissionList.ForEach(t => t.RoomId = SessionHelper.RoomID);
                        }

                        string RoomLists = "";
                        if (objDTO.PermissionList != null && objDTO.PermissionList.Count > 0)
                        {
                            objDTO.UserWiseAllRoomsAccessDetails = objUserMasterDAL.ConvertUserPermissions(objDTO.PermissionList, objDTO.RoleID, ref RoomLists);
                            if (objDTO.RoleID == -1)
                            {
                                objDTO.UserWiseAllRoomsAccessDetails.ForEach(t => t.RoomID = SessionHelper.RoomID);
                            }
                            objDTO.SelectedRoomAccessValue = RoomLists;
                        }
                        //objDTO.ReplenishingRooms = objUserMasterDAL.GetUserRoomReplanishmentDetailsRecord(objDTO.ID);
                        SessionHelper.RoomPermissions = objDTO.UserWiseAllRoomsAccessDetails;


                        //List<KeyValuePair<long, string>> TempRoomList = null;

                        //if (!string.IsNullOrEmpty(objDTO.SelectedRoomAccessValue))
                        //{
                        //    TempRoomList = new List<KeyValuePair<long, string>>();

                        //    string[] RoomAccessIDs = objDTO.SelectedRoomAccessValue.Split(',');
                        //    for (int i = 0; i < RoomAccessIDs.Length; i++)
                        //    {
                        //        string[] RoomIDs = RoomAccessIDs[i].Split('_');
                        //        if (RoomIDs.Length > 1)
                        //        {
                        //            TempRoomList.Add(new KeyValuePair<long, string>(int.Parse(RoomIDs[0]), RoomIDs[1]));
                        //        }
                        //    }
                        //    SessionHelper.RoomList = TempRoomList;
                        //    if (SessionHelper.RoomList != null)
                        //    {
                        //        SessionHelper.RoomID = (int)SessionHelper.RoomList[0].Key;
                        //        SessionHelper.RoomName = SessionHelper.RoomList[0].Value;
                        //    }
                        //}


                    }
                    else
                    {
                        eTurnsMaster.DAL.UserMasterDAL objUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                        objDTO.PermissionList = objUserMasterDAL.GetUserRoleModuleDetailsRecord(objDTO.ID, objDTO.RoleID, SessionHelper.UserType);
                        string RoomLists = "";
                        if (objDTO.PermissionList != null && objDTO.PermissionList.Count > 0)
                        {
                            objDTO.UserWiseAllRoomsAccessDetails = objUserMasterDAL.ConvertUserPermissions(objDTO.PermissionList, objDTO.RoleID, ref RoomLists);
                            objDTO.SelectedRoomAccessValue = RoomLists;
                        }
                        //objDTO.ReplenishingRooms = objUserMasterDAL.GetUserRoomReplanishmentDetailsRecord(objDTO.ID);
                        SessionHelper.RoomPermissions = objDTO.UserWiseAllRoomsAccessDetails;

                        List<RoomDTO> TempRoomList = null;

                        if (!string.IsNullOrEmpty(objDTO.SelectedRoomAccessValue))
                        {
                            TempRoomList = new List<RoomDTO>();

                            string[] RoomAccessIDs = objDTO.SelectedRoomAccessValue.Split(sepForRoleRoom, StringSplitOptions.RemoveEmptyEntries);
                            for (int i = 0; i < RoomAccessIDs.Length; i++)
                            {
                                string[] RoomIDs = RoomAccessIDs[i].Split('_');
                                if (RoomIDs.Length > 1)
                                {
                                    TempRoomList.Add(new RoomDTO() { ID = long.Parse(RoomIDs[0]), RoomName = RoomIDs[1] });
                                    //TempRoomList.Add(new KeyValuePair<long, string>(int.Parse(RoomIDs[0]), RoomIDs[1]));
                                }
                            }
                            SessionHelper.RoomList = TempRoomList;
                            if (SessionHelper.RoomList != null && SessionHelper.RoomList.Any())
                            {
                                SessionHelper.RoomID = SessionHelper.RoomList.First().ID;
                                SessionHelper.RoomName = SessionHelper.RoomList.First().RoomName;
                            }
                        }
                    }

                }
                else if (SessionHelper.UserType == 2)
                {

                    SessionHelper.EnterPriseList = objEnterpriseMasterDAL.GetEnterPriseByUser(objDTO.ID, objDTO.RoleID, SessionHelper.UserType).Where(t => t.ID == SessionHelper.EnterPriceID).ToList();
                    if (SessionHelper.EnterPriseList != null && SessionHelper.EnterPriseList.Count > 0)
                    {
                        CompanyMasterDAL objCompanyMasterDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
                        SessionHelper.EnterPriceID = SessionHelper.EnterPriseList.First().ID;
                        SessionHelper.EnterPriseDBName = SessionHelper.EnterPriseList.First().EnterpriseDBName;
                        //SessionHelper.CompanyList = objCompanyMasterDAL.GetAllRecords().ToList();
                        List<CompanyMasterDTO> lstCompanyAll = objCompanyMasterDAL.GetAllRecords().ToList();

                        if (objDTO.RoleID == -2)
                        {
                            SessionHelper.CompanyList = lstCompanyAll;
                        }
                        else
                        {
                            eTurns.DAL.UserMasterDAL objuser = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                            List<long> lstcompany = objuser.GetDistinctChildCompanyID(objDTO.ID, objDTO.RoleID, SessionHelper.UserType);
                            List<CompanyMasterDTO> lstFinal = new List<CompanyMasterDTO>();
                            lstFinal = (from s in lstCompanyAll
                                        where lstcompany.Contains(s.ID)
                                        select s).ToList();

                            SessionHelper.CompanyList = lstFinal;
                        }


                        if (SessionHelper.CompanyList != null && SessionHelper.CompanyList.Count > 0)
                        {
                            SessionHelper.CompanyID = SessionHelper.CompanyList.First().ID;
                            SessionHelper.CompanyName = SessionHelper.CompanyList.First().Name;
                            RoomDAL objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
                            List<RoomDTO> lstRoomDTO = new List<RoomDTO>();
                            //lstRoomDTO = objRoomDAL.GetAllRecords(SessionHelper.CompanyID, false, false).ToList();

                            if (objDTO.RoleID == -2)
                            {
                                lstRoomDTO = objRoomDAL.GetAllRoomByCompany(SessionHelper.CompanyID, false, false).ToList();
                                //lstRoomDTO = objRoomDAL.GetAllRecords(SessionHelper.CompanyID, false, false).ToList();
                            }
                            else
                            {
                                eTurns.DAL.UserMasterDAL objuser = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                                lstRoomDTO = objRoomDAL.GetAllRoomByCompany(SessionHelper.CompanyID, false, false).ToList();
                                List<long> lstdistinctRoom = objuser.GetDistinctChildRoomID(objDTO.ID, objDTO.RoleID, SessionHelper.UserType, SessionHelper.CompanyID);
                                lstRoomDTO = (from s in lstRoomDTO
                                              where lstdistinctRoom.Contains(s.ID)
                                              select s).ToList();
                            }

                            //List<KeyValuePair<long, string>> TempRoomList = new List<KeyValuePair<long, string>>();

                            //foreach (var item in lstRoomDTO)
                            //{
                            //    TempRoomList.Add(new KeyValuePair<long, string>(item.ID, item.RoomName));
                            //}
                            SessionHelper.RoomList = lstRoomDTO;
                            if (lstRoomDTO != null && lstRoomDTO.Count > 0)
                            {
                                SessionHelper.RoomID = lstRoomDTO.First().ID;
                                SessionHelper.RoomName = lstRoomDTO.First().RoomName;

                            }
                        }


                        eTurns.DAL.UserMasterDAL objUserMasterDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                        objDTO.PermissionList = objUserMasterDAL.GetUserRoleModuleDetailsRecord(objDTO.ID, objDTO.RoleID, SessionHelper.RoomID, SessionHelper.UserType);
                        if (objDTO.RoleID == -2)
                        {
                            objDTO.PermissionList.ForEach(t => t.RoomId = SessionHelper.RoomID);
                        }


                        string RoomLists = "";
                        if (objDTO.PermissionList != null && objDTO.PermissionList.Count > 0)
                        {
                            objDTO.UserWiseAllRoomsAccessDetails = objUserMasterDAL.ConvertUserPermissions(objDTO.PermissionList, objDTO.RoleID, ref RoomLists);
                            if (objDTO.RoleID == -2)
                            {
                                objDTO.UserWiseAllRoomsAccessDetails.ForEach(t => t.RoomID = SessionHelper.RoomID);
                            }
                            objDTO.SelectedRoomAccessValue = RoomLists;
                        }
                        objDTO.ReplenishingRooms = objUserMasterDAL.GetUserRoomReplanishmentDetailsRecord(objDTO.ID);
                        SessionHelper.RoomPermissions = objDTO.UserWiseAllRoomsAccessDetails;


                        List<RoomDTO> TempRoomList1 = null;

                        //if (!string.IsNullOrEmpty(objDTO.SelectedRoomAccessValue))
                        //{
                        //    TempRoomList1 = new List<KeyValuePair<long, string>>();

                        //    string[] RoomAccessIDs = objDTO.SelectedRoomAccessValue.Split(',');
                        //    for (int i = 0; i < RoomAccessIDs.Length; i++)
                        //    {
                        //        string[] RoomIDs = RoomAccessIDs[i].Split('_');
                        //        if (RoomIDs.Length > 1)
                        //        {
                        //            TempRoomList1.Add(new KeyValuePair<long, string>(int.Parse(RoomIDs[2]), RoomIDs[3]));
                        //        }
                        //    }
                        //    SessionHelper.RoomList = TempRoomList1;
                        //    if (SessionHelper.RoomList != null)
                        //    {
                        //        SessionHelper.RoomID = (int)SessionHelper.RoomList[0].Key;
                        //        SessionHelper.RoomName = SessionHelper.RoomList[0].Value;
                        //    }
                        //}
                        if (!string.IsNullOrEmpty(objDTO.SelectedRoomAccessValue))
                        {
                            TempRoomList1 = new List<RoomDTO>();

                            string[] RoomAccessIDs = objDTO.SelectedRoomAccessValue.Split(sepForRoleRoom, StringSplitOptions.RemoveEmptyEntries);
                            for (int i = 0; i < RoomAccessIDs.Length; i++)
                            {
                                string[] RoomIDs = RoomAccessIDs[i].Split('_');
                                if (RoomIDs.Length > 1)
                                {
                                    TempRoomList1.Add(new RoomDTO() { ID = long.Parse(RoomIDs[0]), RoomName = RoomIDs[1] });
                                    //TempRoomList.Add(new KeyValuePair<long, string>(int.Parse(RoomIDs[0]), RoomIDs[1]));
                                }
                            }
                            SessionHelper.RoomList = TempRoomList1;
                            if (SessionHelper.RoomList != null && SessionHelper.RoomList.Any())
                            {
                                SessionHelper.RoomID = SessionHelper.RoomList.First().ID;
                                SessionHelper.RoomName = SessionHelper.RoomList.First().RoomName;
                            }
                        }

                    }
                    else
                    {
                        eTurnsMaster.DAL.UserMasterDAL objUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                        objDTO.PermissionList = objUserMasterDAL.GetUserRoleModuleDetailsRecord(objDTO.ID, objDTO.RoleID, SessionHelper.UserType);
                        string RoomLists = "";
                        if (objDTO.PermissionList != null && objDTO.PermissionList.Count > 0)
                        {
                            objDTO.UserWiseAllRoomsAccessDetails = objUserMasterDAL.ConvertUserPermissions(objDTO.PermissionList, objDTO.RoleID, ref RoomLists);
                            objDTO.SelectedRoomAccessValue = RoomLists;
                        }
                        //objDTO.ReplenishingRooms = objUserMasterDAL.GetUserRoomReplanishmentDetailsRecord(objDTO.ID);
                        SessionHelper.RoomPermissions = objDTO.UserWiseAllRoomsAccessDetails;

                        //List<KeyValuePair<long, string>> TempRoomList = null;

                        //if (!string.IsNullOrEmpty(objDTO.SelectedRoomAccessValue))
                        //{
                        //    TempRoomList = new List<KeyValuePair<long, string>>();

                        //    string[] RoomAccessIDs = objDTO.SelectedRoomAccessValue.Split(',');
                        //    for (int i = 0; i < RoomAccessIDs.Length; i++)
                        //    {
                        //        string[] RoomIDs = RoomAccessIDs[i].Split('_');
                        //        if (RoomIDs.Length > 1)
                        //        {
                        //            TempRoomList.Add(new KeyValuePair<long, string>(int.Parse(RoomIDs[0]), RoomIDs[1]));
                        //        }
                        //    }
                        //    SessionHelper.RoomList = TempRoomList;
                        //    if (SessionHelper.RoomList != null)
                        //    {
                        //        SessionHelper.RoomID = (int)SessionHelper.RoomList[0].Key;
                        //        SessionHelper.RoomName = SessionHelper.RoomList[0].Value;
                        //    }
                        //}
                        List<RoomDTO> TempRoomList = null;

                        if (!string.IsNullOrEmpty(objDTO.SelectedRoomAccessValue))
                        {
                            TempRoomList = new List<RoomDTO>();

                            string[] RoomAccessIDs = objDTO.SelectedRoomAccessValue.Split(sepForRoleRoom, StringSplitOptions.RemoveEmptyEntries);
                            for (int i = 0; i < RoomAccessIDs.Length; i++)
                            {
                                string[] RoomIDs = RoomAccessIDs[i].Split('_');
                                if (RoomIDs.Length > 1)
                                {
                                    TempRoomList.Add(new RoomDTO() { ID = long.Parse(RoomIDs[0]), RoomName = RoomIDs[1] });
                                    //TempRoomList.Add(new KeyValuePair<long, string>(int.Parse(RoomIDs[0]), RoomIDs[1]));
                                }
                            }
                            SessionHelper.RoomList = TempRoomList;
                            if (SessionHelper.RoomList != null && SessionHelper.RoomList.Any())
                            {
                                SessionHelper.RoomID = SessionHelper.RoomList.First().ID;
                                SessionHelper.RoomName = SessionHelper.RoomList.First().RoomName;
                            }
                        }
                    }

                }
                else
                {


                    SessionHelper.EnterPriseList = objEnterpriseMasterDAL.GetEnterPriseByUser(objDTO.ID, objDTO.RoleID, SessionHelper.UserType).Where(t => t.ID == SessionHelper.EnterPriceID).ToList();
                    if (SessionHelper.EnterPriseList != null && SessionHelper.EnterPriseList.Count > 0)
                    {
                        CompanyMasterDAL objCompanyMasterDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
                        SessionHelper.EnterPriceID = SessionHelper.EnterPriseList.First().ID;
                        SessionHelper.EnterPriseDBName = SessionHelper.EnterPriseList.First().EnterpriseDBName;
                        //SessionHelper.CompanyList = objCompanyMasterDAL.GetAllRecords().ToList();
                        List<CompanyMasterDTO> lstCompanyAll = objCompanyMasterDAL.GetAllRecords().ToList();


                        eTurns.DAL.UserMasterDAL objuser = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                        List<long> lstcompany = objuser.GetDistinctChildCompanyID(objDTO.ID, objDTO.RoleID, SessionHelper.UserType);
                        List<CompanyMasterDTO> lstFinal = new List<CompanyMasterDTO>();
                        lstFinal = (from s in lstCompanyAll
                                    where lstcompany.Contains(s.ID)
                                    select s).ToList();

                        SessionHelper.CompanyList = lstFinal;

                        if (SessionHelper.CompanyList != null && SessionHelper.CompanyList.Count > 0)
                        {
                            SessionHelper.CompanyID = SessionHelper.CompanyList.First().ID;
                            SessionHelper.CompanyName = SessionHelper.CompanyList.First().Name;
                            RoomDAL objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
                            List<RoomDTO> lstRoomDTO = new List<RoomDTO>();
                            //lstRoomDTO = objRoomDAL.GetAllRecords(SessionHelper.CompanyID, false, false).ToList();


                            eTurns.DAL.UserMasterDAL objuser1 = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                            List<long> lstdistinctRoom = objuser1.GetDistinctChildRoomID(objDTO.ID, objDTO.RoleID, SessionHelper.UserType, SessionHelper.CompanyID);
                            lstRoomDTO = (from s in objRoomDAL.GetAllRecords(SessionHelper.CompanyID, false, false).ToList()
                                          where lstdistinctRoom.Contains(s.ID)
                                          select s).ToList();


                            //List<KeyValuePair<long, string>> TempRoomList = new List<KeyValuePair<long, string>>();

                            //foreach (var item in lstRoomDTO)
                            //{
                            //    TempRoomList.Add(new KeyValuePair<long, string>(item.ID, item.RoomName));
                            //}
                            SessionHelper.RoomList = lstRoomDTO;
                            if (lstRoomDTO != null && lstRoomDTO.Count > 0)
                            {
                                SessionHelper.RoomID = lstRoomDTO.First().ID;
                                SessionHelper.RoomName = lstRoomDTO.First().RoomName;

                            }


                            eTurns.DAL.UserMasterDAL objUserMasterDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                            objDTO.PermissionList = objUserMasterDAL.GetUserRoleModuleDetailsRecord(objDTO.ID, objDTO.RoleID, SessionHelper.RoomID, SessionHelper.UserType);

                            string RoomLists = "";
                            if (objDTO.PermissionList != null && objDTO.PermissionList.Count > 0)
                            {
                                objDTO.UserWiseAllRoomsAccessDetails = objUserMasterDAL.ConvertUserPermissions(objDTO.PermissionList, objDTO.RoleID, ref RoomLists);
                                objDTO.SelectedRoomAccessValue = RoomLists;
                            }
                            objDTO.ReplenishingRooms = objUserMasterDAL.GetUserRoomReplanishmentDetailsRecord(objDTO.ID);
                            SessionHelper.RoomPermissions = objDTO.UserWiseAllRoomsAccessDetails;
                        }

                    }

                    List<RoomDTO> TempRoomList1 = null;

                    if (!string.IsNullOrEmpty(objDTO.SelectedRoomAccessValue))
                    {
                        TempRoomList1 = new List<RoomDTO>();

                        string[] RoomAccessIDs = objDTO.SelectedRoomAccessValue.Split(sepForRoleRoom, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < RoomAccessIDs.Length; i++)
                        {
                            string[] RoomIDs = RoomAccessIDs[i].Split('_');
                            if (RoomIDs.Length > 1)
                            {
                                TempRoomList1.Add(new RoomDTO() { ID = long.Parse(RoomIDs[0]), RoomName = RoomIDs[1] });
                                //TempRoomList1.Add(new KeyValuePair<long, string>(int.Parse(RoomIDs[0]), RoomIDs[1]));
                            }
                        }
                        SessionHelper.RoomList = TempRoomList1;
                        if (SessionHelper.RoomList != null || SessionHelper.RoomList.Any())
                        {
                            SessionHelper.RoomID = (int)SessionHelper.RoomList.First().ID;
                            SessionHelper.RoomName = SessionHelper.RoomList.First().RoomName;
                        }
                    }
                }

                //ResourceHelper.CompanyResourceFolder = SessionHelper.EnterPriceID.ToString() + "_" + SessionHelper.CompanyID.ToString();
                //if (eTurns.DTO.Resources.ResourceHelper.CurrentCult == null)
                //    eTurns.DTO.Resources.ResourceHelper.CurrentCult = System.Threading.Thread.CurrentThread.CurrentCulture;

                return Json(new { Message = "success", Status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Message = "fail", Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SetTemplatePermissionToUserSession(long EnterpriseID, long CompanyID, long RoomID, long RoleID, long UserID, long templateID)
        {
            List<UserWiseRoomsAccessDetailsDTO> lstSessionpermissions = new List<UserWiseRoomsAccessDetailsDTO>();
            if (templateID > 0)
            {
                if (Session["SelectedUserRoomsPermission"] == null)
                {
                    Session["SelectedUserRoomsPermission"] = new List<UserWiseRoomsAccessDetailsDTO>();
                }
                else
                {
                    lstSessionpermissions = (List<UserWiseRoomsAccessDetailsDTO>)Session["SelectedUserRoomsPermission"];
                    UserWiseRoomsAccessDetailsDTO objtoremove = lstSessionpermissions.FirstOrDefault(t => t.EnterpriseId == EnterpriseID && t.CompanyId == CompanyID && t.RoomID == RoomID);
                    if (objtoremove != null)
                    {
                        lstSessionpermissions.Remove(objtoremove);
                    }
                }
                PermissionTemplateDAL objPermissionTemplateDAL = new PermissionTemplateDAL(SessionHelper.EnterPriseDBName);
                UserWiseRoomsAccessDetailsDTO objRoleWiseRoomsAccessDetailsDTO = new UserWiseRoomsAccessDetailsDTO();

                List<UserRoleModuleDetailsDTO> lstPermissions = objPermissionTemplateDAL.GetPermissionsByTemplateForUser(templateID, RoleID, UserID, RoomID, CompanyID, EnterpriseID);
                var objMasterList = (from t in lstPermissions
                                     where t.GroupId.ToString() == "1" && t.IsModule == true
                                     select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);
                var objOtherModuleList = (from t in lstPermissions
                                          where t.GroupId.ToString() == "2" && t.IsModule == true
                                          select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                var objNonModuleList = (from t in lstPermissions
                                        where t.IsModule == false && t.GroupId.ToString() != "4"
                                        select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                var objOtherDefaultSettings = (from t in lstPermissions
                                               where t.GroupId.ToString() == "4" && t.IsModule == false
                                               select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                foreach (var item in objNonModuleList)
                {
                    item.DisplayOrderName = ResourceHelper.GetResourceValue(Enum.GetName(typeof(RoleOrderStatus), item.DisplayOrderNumber).ToString(), "ResModuleName");
                }
                objRoleWiseRoomsAccessDetailsDTO.PermissionList = lstPermissions;
                objRoleWiseRoomsAccessDetailsDTO.OtherModuleList = objOtherModuleList.ToList();
                objRoleWiseRoomsAccessDetailsDTO.ModuleMasterList = objMasterList.ToList();
                objRoleWiseRoomsAccessDetailsDTO.NonModuleList = objNonModuleList.ToList();
                objRoleWiseRoomsAccessDetailsDTO.OtherDefaultSettings = objOtherDefaultSettings.ToList();
                objRoleWiseRoomsAccessDetailsDTO.RoomID = RoomID;
                objRoleWiseRoomsAccessDetailsDTO.CompanyId = CompanyID;
                objRoleWiseRoomsAccessDetailsDTO.EnterpriseId = EnterpriseID;
                objRoleWiseRoomsAccessDetailsDTO.RoleID = RoleID;

                lstSessionpermissions.Add(objRoleWiseRoomsAccessDetailsDTO);
                Session["SelectedUserRoomsPermission"] = lstSessionpermissions;
            }
            else
            {

            }
            return Json(null);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult UserLoginAuthanticateMasterDataBase(string Email, string Password, bool? IsRemember)
        {
            UserBAL objUserBAL = new UserBAL();
            EnterPriseUserMasterDAL objEnterPriseUserMasterDAL = new EnterPriseUserMasterDAL(SessionHelper.EnterPriseDBName);
            EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
            UserMasterDTO objDTO = objEnterPriseUserMasterDAL.CheckAuthanticationUserName(Email, Password);
            bool IsUserLocked = objUserBAL.CheckSetAcountLockout(Email, Password, objDTO);

            eTurnsMaster.DAL.UserMasterDAL objUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
            if (objDTO != null)
            {
                if (!IsUserLocked)
                {
                    FormsAuthentication.SetAuthCookie(objDTO.UserName, true);
                    HttpCookie remUserName = new HttpCookie("eturnsUserName");
                    HttpCookie remPassword = new HttpCookie("eturnsPassword");
                    remUserName.HttpOnly = false;
                    remPassword.HttpOnly = false;
                    if (IsRemember ?? false)
                    {
                        remUserName.Value = Email;
                        remPassword.Value = Password; //encryption is required here   
                    }
                    else
                    {
                        remUserName.Value = string.Empty;
                        remPassword.Value = string.Empty;
                    }
                    remUserName.Expires = DateTimeUtility.DateTimeNow.AddDays(15);
                    remPassword.Expires = DateTimeUtility.DateTimeNow.AddDays(15);
                    if (Response != null && Response.Cookies != null)
                    {

                        Response.Cookies.Add(remUserName);
                        Response.Cookies.Add(remPassword);
                    }

                    SessionHelper.UserID = objDTO.ID;
                    SessionHelper.RoleID = objDTO.RoleID;
                    SessionHelper.UserType = objDTO.UserType;
                    SessionHelper.EnterPriceID = objDTO.EnterpriseId;
                    SessionHelper.CompanyID = objDTO.CompanyID;
                    //SessionHelper.RoomID = objDTO.Room ?? 0;
                    SessionHelper.UserName = objDTO.UserName;
                    SessionHelper.LoggedinUser = objDTO;
                    SessionHelper.IsLicenceAccepted = objDTO.IsLicenceAccepted;
                    SessionHelper.HasPasswordChanged = objDTO.HasChangedFirstPassword;
                    SessionHelper.NewEulaAccept = objDTO.NewEulaAccept;
                    SetSessions(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.UserType, SessionHelper.RoleID, "onlogin", SessionHelper.EnterPriceName, SessionHelper.CompanyName, SessionHelper.RoomName);

                    //ResourceHelper.CompanyResourceFolder = SessionHelper.EnterPriceID.ToString() + "_" + SessionHelper.CompanyID.ToString();
                    //if (eTurns.DTO.Resources.ResourceHelper.CurrentCult == null)
                    //    eTurns.DTO.Resources.ResourceHelper.CurrentCult = System.Threading.Thread.CurrentThread.CurrentCulture;

                    return Json(new { Message = "success", Status = "ok", HasLicenceAccepted = objDTO.IsLicenceAccepted }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Message = "fail", Status = "locked" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Message = "fail", Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult EditUserProfile(Int64 ID, int? UserType)
        {
            UserMasterDTO objUserMasterDTO = new UserMasterDTO();
            eTurnsMaster.DAL.UserMasterDAL objUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
            objUserMasterDTO = objUserMasterDAL.GetUserByID(ID);
            return PartialView("_UserProfile", objUserMasterDTO);
        }

        [HttpPost]
        public JsonResult SaveUserProfile(UserMasterDTO objDTO)
        {
            string message = "";
            string status = "";
            eTurnsMaster.DAL.UserMasterDAL obj = new eTurnsMaster.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
            CommonMasterDAL objCDAL = new CommonMasterDAL();
            if (string.IsNullOrEmpty(objDTO.UserName))
            {
                message = "User name is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(objDTO.UserName))
            {
                message = "User name is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            objDTO.LastUpdatedBy = SessionHelper.UserID;


            string strOK = objCDAL.UserDuplicateCheckUserName(objDTO.ID, objDTO.UserName);
            if (strOK == "duplicate")
            {
                message = "User name \"" + objDTO.UserName + "\" already exist! Try with Another!";
                status = "duplicate";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(objDTO.Password))
                {
                    objDTO.Password = CommonUtility.getSHA15Hash(objDTO.Password);
                }
                UserMasterDTO objUserMasterDTO = obj.EditProfile(objDTO, false);
                if (objUserMasterDTO.ID > 0)
                {
                    if (objUserMasterDTO.UserType != 1)
                    {
                        eTurns.DAL.UserMasterDAL objUserMasterDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                        objUserMasterDAL.EditProfile(objUserMasterDTO, false);
                    };
                    message = "Record Saved Sucessfully...";
                    status = "ok";
                    Session["SelectedRoomsPermission"] = null;
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    message = "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                    status = "fail";
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                }

            }
        }

        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>
        [AjaxOrChildActionOnlyAttribute]
        public ActionResult UserEdit(Int64 ID, int? UserType, long? EnterpriseID)
        {
            if ((UserType ?? 1) == 1)
            {
                eTurnsMaster.DAL.UserMasterDAL obj = new eTurnsMaster.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                UserMasterDTO objDTO = obj.GetRecord(ID);

                if (objDTO != null)
                {

                    objDTO.CreatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                    objDTO.UpdatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                }

                //var objMasterList = from t in objDTO.PermissionList
                //                    where t.GroupId.ToString() == "1" && t.IsModule == true
                //                    select t;
                //var objOtherModuleList = from t in objDTO.PermissionList
                //                         where t.GroupId.ToString() == "2" && t.IsModule == true
                //                         select t;

                //var objNonModuleList = from t in objDTO.PermissionList
                //                       where t.IsModule == false && t.GroupId.ToString() != "4"
                //                       select t;

                //var objOtherDefaultSettings = from t in objDTO.PermissionList
                //                              where t.GroupId.ToString() == "4" && t.IsModule == false
                //                              select t;
                var objMasterList = (from t in objDTO.PermissionList
                                     where t.GroupId.ToString() == "1" && t.IsModule == true
                                     select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);
                var objOtherModuleList = (from t in objDTO.PermissionList
                                          where t.GroupId.ToString() == "2" && t.IsModule == true
                                          select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                var objNonModuleList = (from t in objDTO.PermissionList
                                        where t.IsModule == false && t.GroupId.ToString() != "4"
                                        select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                var objOtherDefaultSettings = (from t in objDTO.PermissionList
                                               where t.GroupId.ToString() == "4" && t.IsModule == false
                                               select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                foreach (var item in objNonModuleList)
                {
                    item.DisplayOrderName = ResourceHelper.GetResourceValue(Enum.GetName(typeof(RoleOrderStatus), item.DisplayOrderNumber).ToString(), "ResModuleName");
                }

                UserWiseRoomsAccessDetailsDTO objRoomAccessDTO = new UserWiseRoomsAccessDetailsDTO();
                objRoomAccessDTO.PermissionList = objDTO.PermissionList.ToList();
                objRoomAccessDTO.ModuleMasterList = objMasterList.ToList();
                objRoomAccessDTO.OtherModuleList = objOtherModuleList.ToList();
                objRoomAccessDTO.NonModuleList = objNonModuleList.ToList();
                objRoomAccessDTO.OtherDefaultSettings = objOtherDefaultSettings.ToList();
                objRoomAccessDTO.UserType = UserType ?? 1;
                objRoomAccessDTO.RoleID = objDTO.RoleID;
                objRoomAccessDTO.RoomID = 0;
                objRoomAccessDTO.CompanyId = 0;
                objRoomAccessDTO.EnterpriseId = 0;
                objDTO.UserwiseRoomsAccessDetail = objRoomAccessDTO;


                if (objDTO.UserWiseAllRoomsAccessDetails != null)
                {
                    Session["SelectedUserRoomsPermission"] = objDTO.UserWiseAllRoomsAccessDetails;
                }

                List<RoomDTO> objRoomList = new List<RoomDTO>();

                RoomDTO odjRoomDTO = new RoomDTO();
                odjRoomDTO.ID = 0;
                odjRoomDTO.RoomName = "";
                objRoomList.Insert(0, odjRoomDTO);
                ViewBag.RoomsList = objRoomList;

                objDTO.SelectedModuleIDs = "";
                objDTO.SelectedNonModuleIDs = "";
                //if (objDTO.ReplenishingRooms != null && objDTO.ReplenishingRooms.Count > 0)
                //{
                //    foreach (UserRoomReplanishmentDetailsDTO item in objDTO.ReplenishingRooms)
                //    {
                //        if (string.IsNullOrEmpty(objDTO.SelectedRoomReplanishmentValue))
                //            objDTO.SelectedRoomReplanishmentValue = item.RoomID.ToString();
                //        else
                //            objDTO.SelectedRoomReplanishmentValue += ',' + item.RoomID.ToString();
                //    }
                //}
                //objDTO.SelectedRoomReplanishmentValue
                return PartialView("_CreateUser", objDTO);
            }
            else
            {
                eTurns.DAL.UserMasterDAL obj;
                EnterpriseDTO oEnt = new EnterpriseMasterDAL().GetEnterprise(EnterpriseID ?? 0);
                if (oEnt != null)
                    obj = new eTurns.DAL.UserMasterDAL(oEnt.EnterpriseDBName);
                else
                    obj = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);

                UserMasterDTO objDTO = obj.GetRecord(ID);
                objDTO.EnterpriseId = EnterpriseID ?? 0;
                //var objMasterList = from t in objDTO.PermissionList
                //                    where t.GroupId.ToString() == "1" && t.IsModule == true
                //                    select t;
                //var objOtherModuleList = from t in objDTO.PermissionList
                //                         where t.GroupId.ToString() == "2" && t.IsModule == true
                //                         select t;

                //var objNonModuleList = from t in objDTO.PermissionList
                //                       where t.IsModule == false && t.GroupId.ToString() != "4"
                //                       select t;

                //var objOtherDefaultSettings = from t in objDTO.PermissionList
                //                              where t.GroupId.ToString() == "4" && t.IsModule == false
                //                              select t;

                var objMasterList = (from t in objDTO.PermissionList
                                     where t.GroupId.ToString() == "1" && t.IsModule == true && t.ModuleID != 41
                                     select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);
                var objOtherModuleList = (from t in objDTO.PermissionList
                                          where t.GroupId.ToString() == "2" && t.IsModule == true
                                          select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                var objNonModuleList = (from t in objDTO.PermissionList
                                        where t.IsModule == false && t.GroupId.ToString() != "4"
                                        select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                var objOtherDefaultSettings = (from t in objDTO.PermissionList
                                               where t.GroupId.ToString() == "4" && t.IsModule == false
                                               select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                foreach (var item in objNonModuleList)
                {
                    item.DisplayOrderName = ResourceHelper.GetResourceValue(Enum.GetName(typeof(RoleOrderStatus), item.DisplayOrderNumber).ToString(), "ResModuleName");
                }

                UserWiseRoomsAccessDetailsDTO objRoomAccessDTO = new UserWiseRoomsAccessDetailsDTO();
                objRoomAccessDTO.PermissionList = objDTO.PermissionList.ToList();
                objRoomAccessDTO.ModuleMasterList = objMasterList.ToList();
                objRoomAccessDTO.OtherModuleList = objOtherModuleList.ToList();
                objRoomAccessDTO.NonModuleList = objNonModuleList.ToList();
                objRoomAccessDTO.OtherDefaultSettings = objOtherDefaultSettings.ToList();

                objRoomAccessDTO.RoleID = objDTO.RoleID;
                objRoomAccessDTO.RoomID = 0;
                objRoomAccessDTO.CompanyId = 0;
                objRoomAccessDTO.EnterpriseId = EnterpriseID ?? 0;
                objRoomAccessDTO.UserType = UserType ?? 1;
                objDTO.UserwiseRoomsAccessDetail = objRoomAccessDTO;


                if (objDTO.UserWiseAllRoomsAccessDetails != null)
                {
                    Session["SelectedUserRoomsPermission"] = objDTO.UserWiseAllRoomsAccessDetails;
                }

                List<RoomDTO> objRoomList = new List<RoomDTO>();

                RoomDTO odjRoomDTO = new RoomDTO();
                odjRoomDTO.ID = 0;
                odjRoomDTO.RoomName = "";
                objRoomList.Insert(0, odjRoomDTO);
                ViewBag.RoomsList = objRoomList;

                objDTO.SelectedModuleIDs = "";
                objDTO.SelectedNonModuleIDs = "";
                //if (objDTO.ReplenishingRooms != null && objDTO.ReplenishingRooms.Count > 0)
                //{
                //    foreach (UserRoomReplanishmentDetailsDTO item in objDTO.ReplenishingRooms)
                //    {
                //        if (string.IsNullOrEmpty(objDTO.SelectedRoomReplanishmentValue))
                //            objDTO.SelectedRoomReplanishmentValue = item.RoomID.ToString();
                //        else
                //            objDTO.SelectedRoomReplanishmentValue += ',' + item.RoomID.ToString();
                //    }
                //}
                //objDTO.SelectedRoomReplanishmentValue
                if (objDTO != null)
                {

                    objDTO.CreatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                    objDTO.UpdatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                }
                return PartialView("_CreateUser", objDTO);
            }
        }

        #region Ajax Data Provider

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult UserListAjax(JQueryDataTableParamModel param)
        {
            UserBAL obj = new UserBAL();
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
            // set the default column sorting here, if first time then required to set 
            if (sortColumnName == "0" || sortColumnName == "undefined" || string.IsNullOrEmpty(sortColumnName))
                sortColumnName = "ID Desc";

            //if (sortDirection == "asc")
            //    sortColumnName = sortColumnName + " asc";
            //else
            //    sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            var DataFromDB = obj.GetPagedUsersBySQlHelper(SessionHelper.UserType, SessionHelper.EnterPriceID, SessionHelper.RoomID, SessionHelper.CompanyID, param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, IsArchived, IsDeleted, SessionHelper.UserID, SessionHelper.RoleID);
            DataFromDB.ForEach(t =>
            {
                t.Password = string.Empty;
                t.CreatedDate = CommonUtility.ConvertDateByTimeZone(t.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                t.UpdatedDate = CommonUtility.ConvertDateByTimeZone(t.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
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

        public string UpdateUserData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            //UserMasterController obj = new UserMasterController();
            //obj.PutUpdateData(id, value, rowId, columnPosition, columnId, columnName);
            //return value;
            return null;
        }

        //public string DuplicateUserCheck(string UserName, string ActionMode, int ID)
        //{
        //    UserMasterController obj = new UserMasterController();
        //    return obj.DuplicateCheck(UserName, ActionMode, ID);

        //}

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public string DeleteUserRoleRecords(string ids)
        {
            try
            {
                //UserMasterController obj = new UserMasterController();
                //obj.DeleteRecords(ids, 1);
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private List<UserRoleModuleDetailsDTO> ConvertRoleModuleDetailsDTO(List<RoleModuleDetailsDTO> objData)
        {
            List<UserRoleModuleDetailsDTO> objResult = new List<UserRoleModuleDetailsDTO>();
            if (objData != null)
            {
                foreach (RoleModuleDetailsDTO item in objData)
                {
                    UserRoleModuleDetailsDTO objRow = new UserRoleModuleDetailsDTO();
                    objRow.CreatedRoom = item.CreatedRoom;
                    objRow.GroupId = item.GroupId;
                    objRow.GUID = item.GUID;
                    objRow.ID = item.ID;
                    objRow.IsChecked = item.IsChecked;
                    objRow.IsDelete = item.IsDelete;
                    objRow.IsInsert = item.IsInsert;
                    objRow.IsModule = item.IsModule;
                    objRow.IsUpdate = item.IsUpdate;
                    objRow.IsView = item.IsView;
                    objRow.ShowDeleted = item.ShowDeleted;
                    objRow.ShowArchived = item.ShowArchived;
                    objRow.ShowUDF = item.ShowUDF;
                    objRow.ModuleID = item.ModuleID;
                    objRow.ModuleName = item.ModuleName;
                    objRow.ModuleValue = item.ModuleValue;
                    objRow.RoleID = item.RoleID;
                    objRow.RoomId = item.RoomId;
                    objRow.RoomName = item.RoomName;
                    objRow.CompanyId = item.CompanyID;
                    objRow.EnteriseId = item.EnterpriseID;
                    objRow.DisplayOrderNumber = item.DisplayOrderNumber;
                    objRow.resourcekey = item.resourcekey;
                    //objRow.UserID=0;
                    objResult.Add(objRow);
                }

            }
            return objResult;
        }

        private List<UserWiseRoomsAccessDetailsDTO> ConvertRoleWiseRoomsAccessDetailsDTO(List<RoleWiseRoomsAccessDetailsDTO> objData)
        {
            List<UserWiseRoomsAccessDetailsDTO> objResult = new List<UserWiseRoomsAccessDetailsDTO>();
            if (objData != null)
            {

                foreach (RoleWiseRoomsAccessDetailsDTO item in objData)
                {
                    if (item.RoomID > 0)
                    {
                        UserWiseRoomsAccessDetailsDTO objRow = new UserWiseRoomsAccessDetailsDTO();
                        objRow.EnterpriseId = item.EnterpriseID;
                        objRow.CompanyId = item.CompanyID;
                        objRow.RoleID = item.RoleID;
                        objRow.RoomID = item.RoomID;
                        objRow.RoomName = item.RoomName;
                        objRow.PermissionList = ConvertRoleModuleDetailsDTO(item.PermissionList);
                        objResult.Add(objRow);
                    }
                }
            }
            return objResult;
        }
        [HttpPost]
        public JsonResult GetRoleDetailsInfo(string RoleID, int? UserType, long? UserId)
        {
            List<RolePermissionInfo> lstEnterPrises = new List<RolePermissionInfo>();
            List<RolePermissionInfo> lstCompanies = new List<RolePermissionInfo>();
            List<RolePermissionInfo> lstRooms = new List<RolePermissionInfo>();
            List<RolePermissionInfo> lstReplenishRooms = new List<RolePermissionInfo>();
            List<UserWiseRoomsAccessDetailsDTO> objRoomsAccessList = new List<UserWiseRoomsAccessDetailsDTO>();
            UserMasterDTO objUserMasterDTO = null;


            if (UserType.HasValue)
            {
                if ((UserType ?? 0) == 1)
                {
                    eTurnsMaster.DAL.RoleMasterDAL obj = new eTurnsMaster.DAL.RoleMasterDAL(SessionHelper.EnterPriseDBName);
                    RoleMasterDTO objDTO = obj.GetRecord(int.Parse(RoleID));
                    if (objDTO != null)
                    {
                        objDTO.lstAccess = new EnterpriseMasterDAL().GetRoleAccessWithNames(objDTO.ID);
                    }
                    if (objDTO.RoleWiseRoomsAccessDetails != null)
                    {
                        //    objRoomsAccessList = new List<UserWiseRoomsAccessDetailsDTO>();
                        //    objRoomsAccessList.Add(objRoomAccessDTO);
                        //}
                        //Session["SelectedUserRoomsPermission"] = objRoomsAccessList;

                        objRoomsAccessList = ConvertRoleWiseRoomsAccessDetailsDTO(objDTO.RoleWiseRoomsAccessDetails);
                        //Session["SelectedUserRoomsPermission"] = objRoomsAccessList;
                        Session["SelectedRolesRoomsPermission"] = objRoomsAccessList;
                        if ((UserId ?? 0) < 1)
                        {
                            Session["SelectedUserRoomsPermission"] = objRoomsAccessList;
                        }
                        else
                        {
                            if (Session["SelectedUserRoomsPermission"] != null)
                            {
                                eTurnsMaster.DAL.UserMasterDAL objUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                                UserMasterDTO objUserDTO = objUserMasterDAL.GetRecord(UserId.Value);
                                if (objUserDTO != null)
                                {
                                    if (objUserDTO.RoleID != long.Parse(RoleID))
                                    {
                                        Session["SelectedUserRoomsPermission"] = objRoomsAccessList;
                                    }
                                    else
                                    {
                                        Session["SelectedUserRoomsPermission"] = objUserDTO.UserWiseAllRoomsAccessDetails;
                                    }
                                }

                            }
                        }
                        //List<RoleWiseRoomsAccessDetailsDTO> lstRoleWise = objDTO.RoleWiseRoomsAccessDetails.Where(t => t.RoomID > 0).ToList();
                        lstEnterPrises = (from rItm in objDTO.lstAccess.Where(t => t.EnterpriseId > 0)
                                          group rItm by new { rItm.EnterpriseId, rItm.EnterpriseName } into gruopedentrprses
                                          select new RolePermissionInfo
                                          {
                                              EnterPriseId = gruopedentrprses.Key.EnterpriseId,
                                              EnterPriseName = gruopedentrprses.Key.EnterpriseName,
                                              IsSelected = true
                                          }).OrderBy(o => o.EnterPriseName.Trim()).ToList();

                        lstCompanies = (from rItm in objDTO.lstAccess.Where(t => t.CompanyId > 0)
                                        group rItm by new { rItm.EnterpriseId, rItm.EnterpriseName, rItm.CompanyId, rItm.CompanyName } into gruopedentrprses
                                        select new RolePermissionInfo
                                        {
                                            EnterPriseId = gruopedentrprses.Key.EnterpriseId,
                                            EnterPriseName = gruopedentrprses.Key.EnterpriseName,
                                            CompanyId = gruopedentrprses.Key.CompanyId,
                                            CompanyName = gruopedentrprses.Key.CompanyName,
                                            EnterPriseId_CompanyId = gruopedentrprses.Key.EnterpriseId + "_" + gruopedentrprses.Key.CompanyId,
                                            IsSelected = true
                                        }).OrderBy(o => o.CompanyName.Trim()).ToList();

                        lstRooms = (from rItm in objDTO.lstAccess.Where(t => t.RoomId > 0)
                                    group rItm by new { rItm.EnterpriseId, rItm.EnterpriseName, rItm.CompanyId, rItm.CompanyName, rItm.RoomId, rItm.RoomName } into gruopedentrprses
                                    select new RolePermissionInfo
                                    {
                                        EnterPriseId = gruopedentrprses.Key.EnterpriseId,
                                        EnterPriseName = gruopedentrprses.Key.EnterpriseName,
                                        CompanyId = gruopedentrprses.Key.CompanyId,
                                        CompanyName = gruopedentrprses.Key.CompanyName,
                                        RoomId = gruopedentrprses.Key.RoomId,
                                        RoomName = gruopedentrprses.Key.RoomName,
                                        EnterPriseId_CompanyId_RoomId = gruopedentrprses.Key.EnterpriseId + "_" + gruopedentrprses.Key.CompanyId + "_" + gruopedentrprses.Key.RoomId,
                                        IsSelected = true
                                    }).OrderBy(o => o.RoomName.Trim()).ToList();
                        if ((UserId ?? 0) > 0)
                        {

                            lstEnterPrises.ForEach(t =>
                            {
                                if (t.EnterPriseId > 0)
                                {
                                    t.IsSelected = obj.UserHasEnterpriseAccess(UserId.Value, t.EnterPriseId);
                                }

                            });

                            lstCompanies.ForEach(t =>
                            {
                                if (t.CompanyId > 0)
                                {
                                    t.IsSelected = obj.UserHasCompanyAccess(UserId.Value, t.EnterPriseId, t.CompanyId);
                                }

                            });

                            lstRooms.ForEach(t =>
                            {
                                if (t.RoomId > 0 && t.CompanyId > 0)
                                {
                                    t.IsSelected = obj.UserHasRoomAccess(UserId.Value, t.EnterPriseId, t.CompanyId, t.RoomId);
                                }
                            });
                        }

                    }
                    return Json(new { RoomList = lstRooms, CompanyList = lstCompanies, EnterPriseList = lstEnterPrises, ReplenishList = objDTO.SelectedRoomReplanishmentValue, }, JsonRequestBehavior.AllowGet);
                }
                else if (UserType == 2 || UserType == 3)
                {
                    eTurns.DAL.RoleMasterDAL obj;
                    eTurns.DAL.UserMasterDAL objUserMasterDAL;
                    EnterpriseDTO oEnt = new EnterpriseMasterDAL().GetEnterpriseByUserID(UserId ?? 0);
                    if (oEnt != null)
                    {
                        obj = new eTurns.DAL.RoleMasterDAL(oEnt.EnterpriseDBName);
                        objUserMasterDAL = new eTurns.DAL.UserMasterDAL(oEnt.EnterpriseDBName);
                    }
                    else
                    {
                        obj = new eTurns.DAL.RoleMasterDAL(SessionHelper.EnterPriseDBName);
                        objUserMasterDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                    }

                    if ((UserId ?? 0) > 0)
                    {
                        objUserMasterDTO = objUserMasterDAL.GetUserDetails(UserId.Value);
                    }
                    RoleMasterDTO objDTO = obj.GetRecord(long.Parse(RoleID));
                    if (objDTO.RoleWiseRoomsAccessDetails != null)
                    {
                        objRoomsAccessList = ConvertRoleWiseRoomsAccessDetailsDTO(objDTO.RoleWiseRoomsAccessDetails);
                        Session["SelectedRolesRoomsPermission"] = objRoomsAccessList;
                        if ((UserId ?? 0) < 1)
                        {
                            Session["SelectedUserRoomsPermission"] = objRoomsAccessList;
                        }
                        else
                        {
                            if (Session["SelectedUserRoomsPermission"] != null)
                            {

                                UserMasterDTO objUserDTO = objUserMasterDAL.GetRecord(UserId.Value);
                                if (objUserDTO != null)
                                {
                                    if (objUserDTO.RoleID != long.Parse(RoleID))
                                    {
                                        Session["SelectedUserRoomsPermission"] = objRoomsAccessList;
                                    }
                                    else
                                    {
                                        Session["SelectedUserRoomsPermission"] = objUserDTO.UserWiseAllRoomsAccessDetails;
                                    }
                                }

                            }
                        }

                        //List<RoleWiseRoomsAccessDetailsDTO> lstRoleWise = objDTO.RoleWiseRoomsAccessDetails.Where(t => t.RoomID > 0).ToList();
                        lstEnterPrises = (from rItm in objDTO.lstAccess.Where(t => t.EnterpriseId > 0)
                                          group rItm by new { rItm.EnterpriseId, rItm.EnterpriseName } into gruopedentrprses
                                          select new RolePermissionInfo
                                          {
                                              EnterPriseId = gruopedentrprses.Key.EnterpriseId,
                                              EnterPriseName = gruopedentrprses.Key.EnterpriseName,
                                              IsSelected = true
                                          }).OrderBy(o => o.EnterPriseName.Trim()).ToList();

                        lstCompanies = (from rItm in objDTO.lstAccess.Where(t => t.CompanyId > 0)
                                        group rItm by new { rItm.EnterpriseId, rItm.EnterpriseName, rItm.CompanyId, rItm.CompanyName } into gruopedentrprses
                                        select new RolePermissionInfo
                                        {
                                            EnterPriseId = gruopedentrprses.Key.EnterpriseId,
                                            EnterPriseName = gruopedentrprses.Key.EnterpriseName,
                                            CompanyId = gruopedentrprses.Key.CompanyId,
                                            CompanyName = gruopedentrprses.Key.CompanyName,
                                            EnterPriseId_CompanyId = gruopedentrprses.Key.EnterpriseId + "_" + gruopedentrprses.Key.CompanyId,
                                            IsSelected = true
                                        }).OrderBy(o => o.CompanyName.Trim()).ToList();

                        lstRooms = (from rItm in objDTO.lstAccess.Where(t => t.RoomId > 0)
                                    group rItm by new { rItm.EnterpriseId, rItm.EnterpriseName, rItm.CompanyId, rItm.CompanyName, rItm.RoomId, rItm.RoomName } into gruopedentrprses
                                    select new RolePermissionInfo
                                    {
                                        EnterPriseId = gruopedentrprses.Key.EnterpriseId,
                                        EnterPriseName = gruopedentrprses.Key.EnterpriseName,
                                        CompanyId = gruopedentrprses.Key.CompanyId,
                                        CompanyName = gruopedentrprses.Key.CompanyName,
                                        RoomId = gruopedentrprses.Key.RoomId,
                                        RoomName = gruopedentrprses.Key.RoomName,
                                        EnterPriseId_CompanyId_RoomId = gruopedentrprses.Key.EnterpriseId + "_" + gruopedentrprses.Key.CompanyId + "_" + gruopedentrprses.Key.RoomId,
                                        IsSelected = true
                                    }).OrderBy(o => o.RoomName.Trim()).ToList();
                        if ((UserId ?? 0) > 0 && objUserMasterDTO != null)
                        {

                            lstCompanies.ForEach(t =>
                            {
                                if (t.CompanyId > 0)
                                {
                                    t.IsSelected = obj.UserHasCompanyAccess(UserId.Value, t.CompanyId, long.Parse(RoleID), objUserMasterDTO.RoleID);
                                }

                            });

                            lstRooms.ForEach(t =>
                            {
                                if (t.RoomId > 0 && t.CompanyId > 0)
                                {
                                    t.IsSelected = obj.UserHasRoomAccess(UserId.Value, t.CompanyId, t.RoomId, long.Parse(RoleID), objUserMasterDTO.RoleID);
                                }
                            });
                        }


                        //if (objDTO.ReplenishingRooms != null && objDTO.ReplenishingRooms.Count > 0)
                        //{
                        //    foreach (var item in objDTO.ReplenishingRooms)
                        //    {
                        //        lstReplenishRooms.Add(new RolePermissionInfo()
                        //        {
                        //            EnterPriseId = item.EnterpriseId,
                        //            CompanyId = item.CompanyId,
                        //            RoomId = item.RoomID,
                        //            RoomName = item.RoomName,
                        //            CompanyName = item.CompanyName,
                        //            EnterPriseId_CompanyId_RoomId = item.EnterpriseId + "_" + item.CompanyId + "_" + item.RoomID
                        //        });
                        //    }
                        //}
                    }
                    return Json(new { RoomList = lstRooms, CompanyList = lstCompanies, EnterPriseList = lstEnterPrises, ReplenishList = lstReplenishRooms }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(null);
                }

            }
            else
            {
                return Json(null);
            }


        }

        public JsonResult GetRolewiseRoomInfo(string RoleID, int? UserType)
        {
            Session["SelectedRolesRoomsPermission"] = null;
            if ((UserType ?? 0) == 1)
            {
                eTurnsMaster.DAL.RoleMasterDAL obj = new eTurnsMaster.DAL.RoleMasterDAL(SessionHelper.EnterPriseDBName);
                RoleMasterDTO objDTO = obj.GetRecord(int.Parse(RoleID));
                List<string> objRoomsList = new List<string>();
                string RoomIDs = "";


                if (objDTO.RoleWiseRoomsAccessDetails != null)
                {
                    Session["SelectedRolesRoomsPermission"] = ConvertRoleWiseRoomsAccessDetailsDTO(objDTO.RoleWiseRoomsAccessDetails);

                    foreach (RoleWiseRoomsAccessDetailsDTO item in objDTO.RoleWiseRoomsAccessDetails)
                    {
                        if (item.RoomID != 0)
                        {
                            if (RoomIDs == "")
                            {
                                RoomIDs = "#" + item.EnterpriseID.ToString() + "_" + item.CompanyID.ToString() + "_" + item.RoomID.ToString() + "#";
                                objRoomsList.Add(item.EnterpriseID + "_" + item.CompanyID + "_" + item.RoomID + "_" + item.RoomName);
                            }
                            else
                            {
                                if (RoomIDs.Contains("#" + item.RoomID.ToString() + "#") == false)
                                {
                                    RoomIDs += ", #" + item.EnterpriseID.ToString() + "_" + item.CompanyID.ToString() + "_" + item.RoomID.ToString() + "#";
                                    objRoomsList.Add(item.EnterpriseID + "_" + item.CompanyID + "_" + item.RoomID + "_" + item.RoomName);
                                }
                            }
                        }
                    }
                    if (objDTO.PermissionList != null && objDTO.PermissionList.Count > 0)
                    {
                        long[] arrteamp = objDTO.PermissionList.Where(t => t.RoomId > 0).ToList().Select(t => t.EnterpriseID).Distinct().ToArray();
                        List<EnterpriseDTO> lstEnterPrises = new List<EnterpriseDTO>();

                        objDTO.SelectedEnterpriseAccessValue = string.Join(sepForRoleRoom.First().ToString(), arrteamp);
                        var qCompanies = (from itm in objDTO.PermissionList.Where(t => t.RoomId > 0)
                                          group itm by new { itm.EnterpriseID, itm.CompanyID } into groupedEnterpriseCompanies
                                          select new RolePermissionInfo
                                          {
                                              EnterPriseId = groupedEnterpriseCompanies.Key.EnterpriseID,
                                              CompanyId = groupedEnterpriseCompanies.Key.CompanyID,
                                              EnterPriseId_CompanyId = groupedEnterpriseCompanies.Key.EnterpriseID + "_" + groupedEnterpriseCompanies.Key.CompanyID
                                          });
                        string[] arrteampC = qCompanies.ToList().Select(t => t.EnterPriseId_CompanyId).Distinct().ToArray();
                        objDTO.SelectedCompanyAccessValue = string.Join(sepForRoleRoom.First().ToString(), arrteampC);

                    }
                }
                return Json(new { DDData = objRoomsList, ReplenishList = objDTO.SelectedRoomReplanishmentValue, }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //RoleMasterController obj = new RoleMasterController();
                eTurns.DAL.RoleMasterDAL obj = new eTurns.DAL.RoleMasterDAL(SessionHelper.EnterPriseDBName);
                RoleMasterDTO objDTO = obj.GetRecord(int.Parse(RoleID));
                List<string> objRoomsList = new List<string>();
                string RoomIDs = "";


                if (objDTO.RoleWiseRoomsAccessDetails != null)
                {
                    Session["SelectedRolesRoomsPermission"] = ConvertRoleWiseRoomsAccessDetailsDTO(objDTO.RoleWiseRoomsAccessDetails);

                    foreach (RoleWiseRoomsAccessDetailsDTO item in objDTO.RoleWiseRoomsAccessDetails)
                    {
                        if (item.RoomID != 0)
                        {
                            if (RoomIDs == "")
                            {
                                RoomIDs = "#" + item.RoomID.ToString() + "#";
                                objRoomsList.Add(item.RoomID + "_" + item.RoomName);
                            }
                            else
                            {
                                if (RoomIDs.Contains("#" + item.RoomID.ToString() + "#") == false)
                                {
                                    RoomIDs += ", #" + item.RoomID.ToString() + "#";
                                    objRoomsList.Add(item.RoomID + "_" + item.RoomName);
                                }
                            }
                        }
                    }
                }
                return Json(new { DDData = objRoomsList, ReplenishList = objDTO.SelectedRoomReplanishmentValue }, JsonRequestBehavior.AllowGet);
            }

        }

        public PartialViewResult _CreateUserPermission()
        {
            return PartialView();
        }
        [HttpPost]
        public JsonResult SaveToUserPermissionsToSession(string RoomID, string RoleID, string SelectedModuleList, string SelectedNonModuleList, string SelectedDefaultSettings)
        {
            string EnterpriseID = "";
            string CompanyID = "";
            if (!string.IsNullOrEmpty(RoomID) && !string.IsNullOrEmpty(RoleID))
            {
                string[] ecr = RoomID.Split('_');
                if (ecr.Length > 1)
                {
                    EnterpriseID = ecr[0];
                    CompanyID = ecr[1];
                    RoomID = ecr[2];
                }
                List<UserWiseRoomsAccessDetailsDTO> objRoomsAccessList = new List<UserWiseRoomsAccessDetailsDTO>();
                List<UserRoleModuleDetailsDTO> objPermissionList = new List<UserRoleModuleDetailsDTO>();
                List<int> objPIDs = new List<int>();
                bool AddToList = false;

                if (!string.IsNullOrEmpty(SelectedModuleList))
                {
                    string[] objIDs = SelectedModuleList.Split(',');
                    for (int i = 0; i < objIDs.Length; i++)
                    {
                        string[] name = objIDs[i].Split('_');
                        if (name.Length > 1)
                        {
                            UserRoleModuleDetailsDTO objRDTO = new UserRoleModuleDetailsDTO();

                            objRDTO = objPermissionList.Find(element => element.ModuleID == int.Parse(name[0]));
                            if (objRDTO != null)
                            {
                                if (name[1] == "view")
                                    objRDTO.IsView = true;

                                if (name[1] == "insert")
                                    objRDTO.IsInsert = true;

                                if (name[1] == "update")
                                    objRDTO.IsUpdate = true;

                                if (name[1] == "delete")
                                    objRDTO.IsDelete = true;

                                if (name[1] == "ischecked")
                                    objRDTO.IsChecked = true;

                                if (name[1] == "showdeleted")
                                    objRDTO.ShowDeleted = true;

                                if (name[1] == "showarchived")
                                    objRDTO.ShowArchived = true;

                                if (name[1] == "showudf")
                                    objRDTO.ShowUDF = true;

                            }
                            else
                            {
                                objRDTO = new UserRoleModuleDetailsDTO();
                                objRDTO.ModuleID = long.Parse(name[0]);
                                objRDTO.RoomId = long.Parse(RoomID);
                                objRDTO.CompanyId = long.Parse(CompanyID);
                                objRDTO.EnteriseId = long.Parse(EnterpriseID);
                                if (name[1] == "view")
                                    objRDTO.IsView = true;

                                if (name[1] == "insert")
                                    objRDTO.IsInsert = true;

                                if (name[1] == "update")
                                    objRDTO.IsUpdate = true;

                                if (name[1] == "delete")
                                    objRDTO.IsDelete = true;

                                if (name[1] == "ischecked")
                                    objRDTO.IsChecked = true;

                                if (name[1] == "showdeleted")
                                    objRDTO.ShowDeleted = true;

                                if (name[1] == "showarchived")
                                    objRDTO.ShowArchived = true;

                                if (name[1] == "showudf")
                                    objRDTO.ShowUDF = true;

                                objPermissionList.Add(objRDTO);
                            }
                        }
                    }
                    AddToList = true;
                }
                if (!string.IsNullOrEmpty(SelectedNonModuleList))
                {
                    string[] objIDs = SelectedNonModuleList.Split(',');
                    for (int i = 0; i < objIDs.Length; i++)
                    {
                        string[] name = objIDs[i].Split('_');
                        if (name.Length > 1)
                        {
                            UserRoleModuleDetailsDTO objRDTO = new UserRoleModuleDetailsDTO();
                            objRDTO.ModuleID = int.Parse(name[0]);
                            objRDTO.RoomId = long.Parse(RoomID); ;
                            objRDTO.CompanyId = long.Parse(CompanyID); ;
                            objRDTO.EnteriseId = long.Parse(EnterpriseID);
                            objRDTO.GroupId = 3;
                            if (name[1] == "ischecked")
                                objRDTO.IsChecked = true;

                            objPermissionList.Add(objRDTO);
                        }
                    }
                    AddToList = true;
                }

                if (!string.IsNullOrEmpty(SelectedDefaultSettings))
                {

                    string[] objIDs = SelectedDefaultSettings.Split(',');
                    for (int i = 0; i < objIDs.Length; i++)
                    {
                        string[] name = objIDs[i].Split('#');
                        if (name.Length > 1)
                        {
                            UserRoleModuleDetailsDTO objRDTO = new UserRoleModuleDetailsDTO();

                            objRDTO = objPermissionList.Find(element => element.ModuleID == int.Parse(name[0]));
                            if (objRDTO != null)
                                objRDTO.ModuleValue = name[1];
                            //objRDTO.ModuleValue = name[1].Replace('@',',').TrimEnd(',');
                            else
                            {
                                objRDTO = new UserRoleModuleDetailsDTO();
                                objRDTO.ModuleID = int.Parse(name[0]);
                                objRDTO.RoomId = long.Parse(RoomID); ;
                                objRDTO.CompanyId = long.Parse(CompanyID); ;
                                objRDTO.EnteriseId = long.Parse(EnterpriseID);
                                //objRDTO.ModuleValue = name[1];
                                objRDTO.ModuleValue = name[1].Replace('@', ',').TrimEnd(',');
                                objPermissionList.Add(objRDTO);
                            }
                        }
                    }
                    AddToList = true;
                }
                if (AddToList == true)
                {
                    UserWiseRoomsAccessDetailsDTO objRoomAccessDTO = new UserWiseRoomsAccessDetailsDTO();
                    objRoomAccessDTO.PermissionList = objPermissionList.ToList();
                    objRoomAccessDTO.RoleID = int.Parse(RoleID);
                    objRoomAccessDTO.RoomID = int.Parse(RoomID);
                    objRoomAccessDTO.EnterpriseId = int.Parse(EnterpriseID);
                    objRoomAccessDTO.CompanyId = int.Parse(CompanyID);
                    if (Session["SelectedUserRoomsPermission"] != null)
                    {
                        objRoomsAccessList = (List<UserWiseRoomsAccessDetailsDTO>)Session["SelectedUserRoomsPermission"];
                        UserWiseRoomsAccessDetailsDTO exsisting = objRoomsAccessList.Find(element => element.RoomID == int.Parse(RoomID) && element.CompanyId == int.Parse(CompanyID) && element.EnterpriseId == int.Parse(EnterpriseID));
                        if (exsisting != null)
                            exsisting.PermissionList = objPermissionList.ToList();
                        else
                            objRoomsAccessList.Add(objRoomAccessDTO);
                    }
                    else
                    {
                        objRoomsAccessList = new List<UserWiseRoomsAccessDetailsDTO>();
                        objRoomsAccessList.Add(objRoomAccessDTO);
                    }
                    Session["SelectedUserRoomsPermission"] = objRoomsAccessList;
                }
            }
            return Json("success");
        }

        public UserWiseRoomsAccessDetailsDTO GetUserPermissionsFromSession(string EnterpriseId, string CompanyId, string RoomID, string RoleID)
        {
            List<UserWiseRoomsAccessDetailsDTO> objRoomsAccessList = new List<UserWiseRoomsAccessDetailsDTO>();

            if (!string.IsNullOrEmpty(EnterpriseId) && !string.IsNullOrEmpty(CompanyId) && !string.IsNullOrEmpty(RoomID) && !string.IsNullOrEmpty(RoleID) && Session["SelectedUserRoomsPermission"] != null)
            {
                objRoomsAccessList = (List<UserWiseRoomsAccessDetailsDTO>)Session["SelectedUserRoomsPermission"];
                if (objRoomsAccessList != null && objRoomsAccessList.Count > 0)
                    return objRoomsAccessList.Find(element => element.EnterpriseId == int.Parse(EnterpriseId) && element.CompanyId == int.Parse(CompanyId) && element.RoomID == int.Parse(RoomID));
            }
            return null;
        }

        public void CopyUserPermissionsToRooms(string ParentRoomID, string CopyToRoomIDs, string RoleID)
        {

            string EnterpriseID = "";
            string CompanyID = "";
            string RoomID = "";
            if (!string.IsNullOrEmpty(ParentRoomID) && !string.IsNullOrEmpty(CopyToRoomIDs) && Session["SelectedUserRoomsPermission"] != null)
            {
                string[] ecr = ParentRoomID.Split('_');
                if (ecr.Length > 1)
                {
                    EnterpriseID = ecr[0];
                    CompanyID = ecr[1];
                    RoomID = ecr[2];
                }
                List<UserWiseRoomsAccessDetailsDTO> objRoomsAccessList;
                if (Session["SelectedUserRoomsPermission"] != null)
                {
                    objRoomsAccessList = (List<UserWiseRoomsAccessDetailsDTO>)Session["SelectedUserRoomsPermission"];
                    UserWiseRoomsAccessDetailsDTO ParentRoomData = objRoomsAccessList.Find(element => element.RoomID == int.Parse(RoomID) && element.CompanyId == int.Parse(CompanyID) && element.EnterpriseId == int.Parse(EnterpriseID));
                    if (ParentRoomData != null)
                    {
                        string[] ids = CopyToRoomIDs.Split(sepForRoleRoom, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < ids.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(ids[i]))
                            {
                                string CopyToEnterpriseID = "";
                                string CopyToCompanyID = "";
                                string CopyToRoomID = "";
                                string[] CopyToecr = ids[i].Split('_');
                                if (CopyToecr.Length > 2)
                                {
                                    CopyToEnterpriseID = CopyToecr[0];
                                    CopyToCompanyID = CopyToecr[1];
                                    CopyToRoomID = CopyToecr[2];
                                }
                                UserWiseRoomsAccessDetailsDTO ChildRoomData = objRoomsAccessList.Find(element => element.RoomID == long.Parse(CopyToRoomID) && element.CompanyId == int.Parse(CopyToCompanyID) && element.EnterpriseId == int.Parse(EnterpriseID));
                                if (ChildRoomData != null)
                                {
                                    List<UserRoleModuleDetailsDTO> lstNewPermissions = new List<UserRoleModuleDetailsDTO>();

                                    //lstNewPermissions.AddRange(ParentRoomData.PermissionList);



                                    foreach (var someitem in ParentRoomData.PermissionList)
                                    {
                                        UserRoleModuleDetailsDTO objNew = new UserRoleModuleDetailsDTO();
                                        objNew.CompanyId = long.Parse(CopyToCompanyID);
                                        objNew.CreatedRoom = someitem.CreatedRoom;
                                        objNew.DisplayOrderName = someitem.DisplayOrderName;
                                        objNew.DisplayOrderNumber = someitem.DisplayOrderNumber;
                                        objNew.EnteriseId = long.Parse(CopyToEnterpriseID);
                                        objNew.GroupId = someitem.GroupId;
                                        objNew.GUID = someitem.GUID;
                                        objNew.ID = someitem.ID;
                                        objNew.ImageName = someitem.ImageName;
                                        objNew.IsChecked = someitem.IsChecked;
                                        objNew.IsDelete = someitem.IsDelete;
                                        objNew.IsInsert = someitem.IsInsert;
                                        objNew.IsModule = someitem.IsModule;
                                        objNew.IsRoomActive = someitem.IsRoomActive;
                                        objNew.IsUpdate = someitem.IsUpdate;
                                        objNew.IsView = someitem.IsView;
                                        objNew.ModuleID = someitem.ModuleID;
                                        objNew.ModuleName = someitem.ModuleName;
                                        objNew.ModuleURL = someitem.ModuleURL;
                                        objNew.ModuleValue = someitem.ModuleValue;
                                        objNew.ParentID = someitem.ParentID;
                                        objNew.resourcekey = someitem.resourcekey;
                                        objNew.RoleID = someitem.RoleID;
                                        objNew.RoomId = long.Parse(CopyToRoomID);
                                        objNew.RoomName = someitem.RoomName;
                                        objNew.ShowArchived = someitem.ShowArchived;
                                        objNew.ShowDeleted = someitem.ShowDeleted;
                                        objNew.ShowUDF = someitem.ShowUDF;
                                        objNew.UserID = someitem.UserID;
                                        lstNewPermissions.Add(objNew);

                                    }


                                    //lstNewPermissions.ForEach(t =>
                                    //{
                                    //    t.EnteriseId = long.Parse(CopyToEnterpriseID);
                                    //    t.CompanyId = long.Parse(CopyToCompanyID);
                                    //    t.RoomId = long.Parse(CopyToRoomID);
                                    //});
                                    ChildRoomData.PermissionList = lstNewPermissions;
                                }
                                else
                                {
                                    UserWiseRoomsAccessDetailsDTO objRoomAccessDTO = new UserWiseRoomsAccessDetailsDTO();
                                    List<UserRoleModuleDetailsDTO> lstNewPermissions = new List<UserRoleModuleDetailsDTO>();
                                    //ParentRoomData.PermissionList
                                    foreach (var someitem in ParentRoomData.PermissionList)
                                    {
                                        UserRoleModuleDetailsDTO objNew = new UserRoleModuleDetailsDTO();
                                        objNew.CompanyId = long.Parse(CopyToCompanyID);
                                        objNew.CreatedRoom = someitem.CreatedRoom;
                                        objNew.DisplayOrderName = someitem.DisplayOrderName;
                                        objNew.DisplayOrderNumber = someitem.DisplayOrderNumber;
                                        objNew.EnteriseId = long.Parse(CopyToEnterpriseID);
                                        objNew.GroupId = someitem.GroupId;
                                        objNew.GUID = someitem.GUID;
                                        objNew.ID = someitem.ID;
                                        objNew.ImageName = someitem.ImageName;
                                        objNew.IsChecked = someitem.IsChecked;
                                        objNew.IsDelete = someitem.IsDelete;
                                        objNew.IsInsert = someitem.IsInsert;
                                        objNew.IsModule = someitem.IsModule;
                                        objNew.IsRoomActive = someitem.IsRoomActive;
                                        objNew.IsUpdate = someitem.IsUpdate;
                                        objNew.IsView = someitem.IsView;
                                        objNew.ModuleID = someitem.ModuleID;
                                        objNew.ModuleName = someitem.ModuleName;
                                        objNew.ModuleURL = someitem.ModuleURL;
                                        objNew.ModuleValue = someitem.ModuleValue;
                                        objNew.ParentID = someitem.ParentID;
                                        objNew.resourcekey = someitem.resourcekey;
                                        objNew.RoleID = someitem.RoleID;
                                        objNew.RoomId = long.Parse(CopyToRoomID);
                                        objNew.RoomName = someitem.RoomName;
                                        objNew.ShowArchived = someitem.ShowArchived;
                                        objNew.ShowDeleted = someitem.ShowDeleted;
                                        objNew.ShowUDF = someitem.ShowUDF;
                                        objNew.UserID = someitem.UserID;
                                        lstNewPermissions.Add(objNew);

                                    }

                                    objRoomAccessDTO.PermissionList = lstNewPermissions;
                                    objRoomAccessDTO.RoleID = int.Parse(RoleID);
                                    objRoomAccessDTO.RoomID = int.Parse(CopyToRoomID);
                                    objRoomAccessDTO.CompanyId = int.Parse(CopyToCompanyID);
                                    objRoomAccessDTO.EnterpriseId = int.Parse(CopyToEnterpriseID);
                                    objRoomsAccessList.Add(objRoomAccessDTO);
                                }
                            }
                        }
                    }
                    Session["SelectedUserRoomsPermission"] = objRoomsAccessList;
                }
            }
        }


        public void AddRemoveUserRoomsToSession(string RoomIDs)
        {
            List<UserWiseRoomsAccessDetailsDTO> objTempRoomsAccessList = new List<UserWiseRoomsAccessDetailsDTO>();
            UserWiseRoomsAccessDetailsDTO RoomData = null;

            if (!string.IsNullOrEmpty(RoomIDs) && Session["SelectedUserRoomsPermission"] != null)
            {
                List<UserWiseRoomsAccessDetailsDTO> objRoomsAccessList;

                objRoomsAccessList = (List<UserWiseRoomsAccessDetailsDTO>)Session["SelectedUserRoomsPermission"];
                objTempRoomsAccessList = new List<UserWiseRoomsAccessDetailsDTO>();

                string[] ids = RoomIDs.Split(sepForRoleRoom, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < ids.Length; i++)
                {
                    string EnterpriseID = string.Empty;
                    string CompanyID = string.Empty;
                    string RoomID = string.Empty;
                    string[] ecr = ids[i].Split('_');
                    if (ecr.Length > 1)
                    {
                        EnterpriseID = ecr[0];
                        CompanyID = ecr[1];
                        RoomID = ecr[2];
                    }

                    RoomData = objRoomsAccessList.Find(element => element.RoomID == int.Parse(RoomID) && element.CompanyId == int.Parse(CompanyID) && element.EnterpriseId == int.Parse(EnterpriseID));
                    if (RoomData != null)
                        objTempRoomsAccessList.Add(RoomData);
                    else
                    {
                        RoomData = RoomsDefaultPermission(long.Parse(RoomID), long.Parse(CompanyID), long.Parse(EnterpriseID));
                        if (RoomData != null)
                            objTempRoomsAccessList.Add(RoomData);
                    }
                }

                Session["SelectedUserRoomsPermission"] = objTempRoomsAccessList;
            }
            else if (!string.IsNullOrEmpty(RoomIDs) && Session["SelectedRolesRoomsPermission"] != null)
            {
                string[] ids = RoomIDs.Split(sepForRoleRoom, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < ids.Length; i++)
                {
                    string EnterpriseID = string.Empty;
                    string CompanyID = string.Empty;
                    string RoomID = string.Empty;
                    string[] ecr = ids[i].Split('_');
                    if (ecr.Length > 1)
                    {
                        EnterpriseID = ecr[0];
                        CompanyID = ecr[1];
                        RoomID = ecr[2];
                    }
                    RoomData = RoomsDefaultPermission(long.Parse(RoomID), long.Parse(CompanyID), long.Parse(EnterpriseID));
                    if (RoomData != null)
                        objTempRoomsAccessList.Add(RoomData);

                }
                Session["SelectedUserRoomsPermission"] = objTempRoomsAccessList;

            }
            else
            {
                Session["SelectedUserRoomsPermission"] = null;
            }
        }


        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult UserRolePermissionCreate(string RoomID, string RoleID, string UserID, int UserType)
        {
            string EnterpriseID = "0";
            string CompanyID = "0";
            string[] ecr = RoomID.Split('_');
            if (ecr.Length > 1)
            {
                EnterpriseID = ecr[0];
                CompanyID = ecr[1];
                RoomID = ecr[2];
            }
            if (UserType == 1)
            {
                UserWiseRoomsAccessDetailsDTO objRoomAccessDTO = new UserWiseRoomsAccessDetailsDTO();

                //RoleMasterController objRole = new RoleMasterController();
                eTurnsMaster.DAL.RoleMasterDAL objRole = new eTurnsMaster.DAL.RoleMasterDAL(SessionHelper.EnterPriseDBName);

                objRoomAccessDTO = GetUserPermissionsFromSession(EnterpriseID, CompanyID, RoomID, RoleID);

                //UserMasterController obj = new UserMasterController();
                eTurnsMaster.DAL.UserMasterDAL obj = new eTurnsMaster.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                List<UserRoleModuleDetailsDTO> objList = new List<UserRoleModuleDetailsDTO>();
                List<RoleModuleDetailsDTO> objRoomsList = null;
                if (int.Parse(UserID) > 0)
                {
                    objRoomsList = objRole.GetRoleModuleDetailsRecord(0, 0, 0, 0).ToList();
                }
                else
                {
                    objRoomsList = objRole.GetRoleModuleDetailsRecord(int.Parse(RoomID) > 0 ? int.Parse(RoleID) : 0, int.Parse(RoomID), int.Parse(CompanyID), int.Parse(EnterpriseID)).ToList();
                }
                if (objRoomsList != null)
                    objList = ConvertRoleModuleDetailsDTO(objRoomsList.ToList());

                if (objRoomAccessDTO != null)
                {
                    UserRoleModuleDetailsDTO objRoleModuleDetailsDTO;
                    foreach (UserRoleModuleDetailsDTO item in objRoomAccessDTO.PermissionList)
                    {
                        objRoleModuleDetailsDTO = objList.ToList().Find(element => element.ModuleID == item.ModuleID);
                        if (objRoleModuleDetailsDTO != null)
                        {
                            objRoleModuleDetailsDTO.IsChecked = item.IsChecked;
                            objRoleModuleDetailsDTO.IsInsert = item.IsInsert;
                            objRoleModuleDetailsDTO.IsDelete = item.IsDelete;
                            objRoleModuleDetailsDTO.IsUpdate = item.IsUpdate;
                            objRoleModuleDetailsDTO.IsView = item.IsView;
                            objRoleModuleDetailsDTO.ShowDeleted = item.ShowDeleted;
                            objRoleModuleDetailsDTO.ShowArchived = item.ShowArchived;
                            objRoleModuleDetailsDTO.ShowUDF = item.ShowUDF;
                            objRoleModuleDetailsDTO.ModuleValue = item.ModuleValue;

                        }
                    }
                }
                else
                {
                    objRoomAccessDTO = new UserWiseRoomsAccessDetailsDTO();
                    int SelectedRoomID = 0;
                    SelectedRoomID = int.Parse(RoomID);
                    objRoomAccessDTO.PermissionList = objList.ToList();
                    objRoomAccessDTO.RoleID = int.Parse(RoleID);
                    objRoomAccessDTO.RoomID = SelectedRoomID;
                    objRoomAccessDTO.CompanyId = 0;
                    objRoomAccessDTO.EnterpriseId = 0;
                }

                //var objMasterList = from t in objList
                //                    where t.GroupId.ToString() == "1" && t.IsModule == true
                //                    select t;
                //var objOtherModuleList = from t in objList
                //                         where t.GroupId.ToString() == "2" && t.IsModule == true
                //                         select t;

                //var objNonModuleList = from t in objList
                //                       where t.IsModule == false && t.GroupId.ToString() != "4"
                //                       select t;

                //var objOtherDefaultSettings = from t in objList
                //                              where t.GroupId.ToString() == "4" && t.IsModule == false
                //                              select t;

                var objMasterList = (from t in objList
                                     where t.GroupId.ToString() == "1" && t.IsModule == true
                                     select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);
                var objOtherModuleList = (from t in objList
                                          where t.GroupId.ToString() == "2" && t.IsModule == true
                                          select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                var objNonModuleList = (from t in objList
                                        where t.IsModule == false && t.GroupId.ToString() != "4"
                                        select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                var objOtherDefaultSettings = (from t in objList
                                               where t.GroupId.ToString() == "4" && t.IsModule == false
                                               select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                foreach (var item in objNonModuleList)
                {
                    item.DisplayOrderName = ResourceHelper.GetResourceValue(Enum.GetName(typeof(RoleOrderStatus), item.DisplayOrderNumber).ToString(), "ResModuleName");
                }

                objRoomAccessDTO.ModuleMasterList = objMasterList.ToList();
                objRoomAccessDTO.OtherModuleList = objOtherModuleList.ToList();
                objRoomAccessDTO.NonModuleList = objNonModuleList.ToList();
                objRoomAccessDTO.OtherDefaultSettings = objOtherDefaultSettings.ToList();

                List<RoomDTO> objRoomList = new List<RoomDTO>();
                RoomDTO odjRoomDTO = new RoomDTO();
                odjRoomDTO.ID = 0;
                odjRoomDTO.RoomName = "";
                objRoomList.Insert(0, odjRoomDTO);
                ViewBag.RoomsList = objRoomList;
                objRoomAccessDTO.UserType = UserType;
                return PartialView("_CreateUserPermission", objRoomAccessDTO);
            }
            else
            {
                UserWiseRoomsAccessDetailsDTO objRoomAccessDTO = new UserWiseRoomsAccessDetailsDTO();

                //RoleMasterController objRole = new RoleMasterController();
                eTurns.DAL.RoleMasterDAL objRole = new eTurns.DAL.RoleMasterDAL(SessionHelper.EnterPriseDBName);

                objRoomAccessDTO = GetUserPermissionsFromSession(EnterpriseID, CompanyID, RoomID, RoleID);

                //UserMasterController obj = new UserMasterController();
                eTurns.DAL.UserMasterDAL obj = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                List<UserRoleModuleDetailsDTO> objList = new List<UserRoleModuleDetailsDTO>();
                List<RoleModuleDetailsDTO> objRoomsList = null;
                if (int.Parse(UserID) > 0)
                {
                    objRoomsList = objRole.GetRoleModuleDetailsRecord(0, 0).ToList();
                }
                else
                {
                    objRoomsList = objRole.GetRoleModuleDetailsRecord(int.Parse(RoomID) > 0 ? int.Parse(RoleID) : 0, int.Parse(RoomID)).ToList();
                }
                if (objRoomsList != null)
                    objList = ConvertRoleModuleDetailsDTO(objRoomsList.ToList());

                if (objRoomAccessDTO != null)
                {
                    UserRoleModuleDetailsDTO objRoleModuleDetailsDTO;
                    foreach (UserRoleModuleDetailsDTO item in objRoomAccessDTO.PermissionList)
                    {
                        objRoleModuleDetailsDTO = objList.ToList().Find(element => element.ModuleID == item.ModuleID);
                        if (objRoleModuleDetailsDTO != null)
                        {
                            objRoleModuleDetailsDTO.IsChecked = item.IsChecked;
                            objRoleModuleDetailsDTO.IsInsert = item.IsInsert;
                            objRoleModuleDetailsDTO.IsDelete = item.IsDelete;
                            objRoleModuleDetailsDTO.IsUpdate = item.IsUpdate;
                            objRoleModuleDetailsDTO.IsView = item.IsView;
                            objRoleModuleDetailsDTO.ShowDeleted = item.ShowDeleted;
                            objRoleModuleDetailsDTO.ShowArchived = item.ShowArchived;
                            objRoleModuleDetailsDTO.ShowUDF = item.ShowUDF;
                            objRoleModuleDetailsDTO.ModuleValue = item.ModuleValue;
                        }
                    }
                }
                else
                {
                    objRoomAccessDTO = new UserWiseRoomsAccessDetailsDTO();
                    int SelectedRoomID = 0;
                    SelectedRoomID = int.Parse(RoomID);
                    objRoomAccessDTO.PermissionList = objList.ToList();
                    objRoomAccessDTO.RoleID = int.Parse(RoleID);
                    objRoomAccessDTO.RoomID = SelectedRoomID;
                }

                //var objMasterList = from t in objList
                //                    where t.GroupId.ToString() == "1" && t.IsModule == true
                //                    select t;
                //var objOtherModuleList = from t in objList
                //                         where t.GroupId.ToString() == "2" && t.IsModule == true
                //                         select t;

                //var objNonModuleList = from t in objList
                //                       where t.IsModule == false && t.GroupId.ToString() != "4"
                //                       select t;

                //var objOtherDefaultSettings = from t in objList
                //                              where t.GroupId.ToString() == "4" && t.IsModule == false
                //                              select t;

                var objMasterList = (from t in objList
                                     where t.GroupId.ToString() == "1" && t.IsModule == true && t.ModuleID != 41
                                     select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);
                var objOtherModuleList = (from t in objList
                                          where t.GroupId.ToString() == "2" && t.IsModule == true
                                          select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                var objNonModuleList = (from t in objList
                                        where t.IsModule == false && t.GroupId.ToString() != "4"
                                        select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);

                var objOtherDefaultSettings = (from t in objList
                                               where t.GroupId.ToString() == "4" && t.IsModule == false
                                               select t).OrderBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleName);
                foreach (var item in objNonModuleList)
                {
                    item.DisplayOrderName = ResourceHelper.GetResourceValue(Enum.GetName(typeof(RoleOrderStatus), item.DisplayOrderNumber).ToString(), "ResModuleName");
                }


                objRoomAccessDTO.ModuleMasterList = objMasterList.ToList();
                objRoomAccessDTO.OtherModuleList = objOtherModuleList.ToList();
                objRoomAccessDTO.NonModuleList = objNonModuleList.ToList();
                objRoomAccessDTO.OtherDefaultSettings = objOtherDefaultSettings.ToList();

                List<RoomDTO> objRoomList = new List<RoomDTO>();
                RoomDTO odjRoomDTO = new RoomDTO();
                odjRoomDTO.ID = 0;
                odjRoomDTO.RoomName = "";
                objRoomList.Insert(0, odjRoomDTO);
                ViewBag.RoomsList = objRoomList;
                objRoomAccessDTO.UserType = UserType;
                return PartialView("_CreateUserPermission", objRoomAccessDTO);
            }

        }

        private UserWiseRoomsAccessDetailsDTO RoomsDefaultPermission(long RoomID, long CompanyId, long EnterpriseId)
        {
            if (RoomID > 0 && Session["SelectedRolesRoomsPermission"] != null)
            {
                List<UserWiseRoomsAccessDetailsDTO> objRoomsAccessList;
                List<UserWiseRoomsAccessDetailsDTO> objTempRoomsAccessList = new List<UserWiseRoomsAccessDetailsDTO>();

                objRoomsAccessList = (List<UserWiseRoomsAccessDetailsDTO>)Session["SelectedRolesRoomsPermission"];
                objTempRoomsAccessList = new List<UserWiseRoomsAccessDetailsDTO>();
                return objRoomsAccessList.Find(element => element.RoomID == RoomID && element.CompanyId == CompanyId && element.EnterpriseId == EnterpriseId);
            }
            return null;
        }
        #endregion

        public JsonResult SetStockRoom(string RoomID, string RoomText)
        {
            if (!string.IsNullOrEmpty(RoomID))
            {
                SetSessions(SessionHelper.EnterPriceID, SessionHelper.CompanyID, long.Parse(RoomID), SessionHelper.UserID, SessionHelper.UserType, SessionHelper.RoleID, "room", SessionHelper.EnterPriceName, SessionHelper.CompanyName, HttpUtility.UrlDecode(RoomText));
                //SessionHelper.RoomID = int.Parse(RoomID);
                //SessionHelper.RoomName = RoomText;
                //if (SessionHelper.RoleID == -1 || SessionHelper.RoleID == -2)
                //{
                //    SessionHelper.RoomPermissions.ForEach(t => t.RoomID = SessionHelper.RoomID);
                //}
                //List<UserRoleModuleDetailsDTO> PermissionList = new List<UserRoleModuleDetailsDTO>();
                //List<UserWiseRoomsAccessDetailsDTO> UserWiseAllRoomsAccessDetails = new List<UserWiseRoomsAccessDetailsDTO>();
                //List<UserRoomReplanishmentDetailsDTO> ReplenishingRooms = new List<UserRoomReplanishmentDetailsDTO>();

                //if (SessionHelper.UserType == 1)
                //{
                //    eTurnsMaster.DAL.UserMasterDAL objUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                //    PermissionList = objUserMasterDAL.GetUserRoleModuleDetailsRecord(SessionHelper.UserID, SessionHelper.RoleID, SessionHelper.UserType);
                //}
                //else if (SessionHelper.UserType == 2)
                //{
                //    eTurns.DAL.UserMasterDAL objUserMasterDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                //    PermissionList = objUserMasterDAL.GetUserRoleModuleDetailsRecord(SessionHelper.UserID, SessionHelper.RoleID, SessionHelper.UserType);
                //}
                //else
                //{
                //    eTurns.DAL.UserMasterDAL objUserMasterDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                //    PermissionList = objUserMasterDAL.GetUserRoleModuleDetailsRecord(SessionHelper.UserID, SessionHelper.RoleID, SessionHelper.UserType);
                //}
                //if (SessionHelper.RoleID == -1 || SessionHelper.RoleID == -2)
                //    PermissionList.ForEach(t => t.RoomId = SessionHelper.RoomID);

                //string RoomLists = "";
                //if (PermissionList != null && PermissionList.Count > 0)
                //{
                //    eTurns.DAL.UserMasterDAL objUserMasterDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                //    UserWiseAllRoomsAccessDetails = objUserMasterDAL.ConvertUserPermissions(PermissionList, SessionHelper.RoleID, ref RoomLists);
                //    if (SessionHelper.RoleID == -1 || SessionHelper.RoleID == -2)
                //    {
                //        UserWiseAllRoomsAccessDetails.ForEach(t => t.RoomID = SessionHelper.RoomID);
                //    }
                //    // objDTO.SelectedRoomAccessValue = RoomLists;
                //}
                ////objDTO.ReplenishingRooms = objUserMasterDAL.GetUserRoomReplanishmentDetailsRecord(objDTO.ID);
                //SessionHelper.RoomPermissions = UserWiseAllRoomsAccessDetails;
            }
            return Json(new { Message = "success", Status = "ok" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SetEnterprise(long? EnterpriseID, string EnterpriseText)
        {
            if (EnterpriseID.HasValue && EnterpriseID.Value > 0)
            {
                SetSessions(EnterpriseID.Value, 0, 0, SessionHelper.UserID, SessionHelper.UserType, SessionHelper.RoleID, "enterprise", HttpUtility.UrlDecode(EnterpriseText), string.Empty, string.Empty);

                //ResourceHelper.CompanyResourceFolder = SessionHelper.EnterPriceID.ToString() + "_" + SessionHelper.CompanyID.ToString();
                //if (eTurns.DTO.Resources.ResourceHelper.CurrentCult == null)
                //    eTurns.DTO.Resources.ResourceHelper.CurrentCult = System.Threading.Thread.CurrentThread.CurrentCulture;


                //EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
                //EnterpriseDTO objEnterpriseDTO = objEnterpriseMasterDAL.GetEnterprise(EnterpriseID.Value);
                //if (objEnterpriseDTO != null)
                //{
                //    CompanyMasterDAL objCompanyMasterDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
                //    SessionHelper.EnterPriceID = objEnterpriseDTO.ID;
                //    SessionHelper.EnterPriseDBName = objEnterpriseDTO.EnterpriseDBName;
                //    SessionHelper.CompanyList = objCompanyMasterDAL.GetAllRecords().ToList();
                //    if (SessionHelper.CompanyList != null && SessionHelper.CompanyList.Count > 0)
                //    {
                //        SessionHelper.CompanyID = SessionHelper.CompanyList.First().ID;
                //        SessionHelper.CompanyName = SessionHelper.CompanyList.First().Name;
                //    }
                //    else
                //    {
                //        SessionHelper.CompanyID = 0;
                //        SessionHelper.CompanyName = "";
                //    }
                //    SetCompany(SessionHelper.CompanyID, SessionHelper.CompanyName);

                //}
            }


            return Json(new { Message = "success", Status = "ok" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SetCompany(long? CompanyID, string CompanyName)
        {
            if (CompanyID.HasValue)
            {
                SetSessions(SessionHelper.EnterPriceID, CompanyID.Value, 0, SessionHelper.UserID, SessionHelper.UserType, SessionHelper.RoleID, "company", SessionHelper.EnterPriceName, HttpUtility.UrlDecode(CompanyName), string.Empty);
                //ResourceHelper.CompanyResourceFolder = SessionHelper.EnterPriceID.ToString() + "_" + SessionHelper.CompanyID.ToString();
                //if (eTurns.DTO.Resources.ResourceHelper.CurrentCult == null)
                //    eTurns.DTO.Resources.ResourceHelper.CurrentCult = System.Threading.Thread.CurrentThread.CurrentCulture;

            }
            return Json(new { Message = "success", Status = "ok" }, JsonRequestBehavior.AllowGet);
            //List<RoomDTO> lstRoomDTO = new List<RoomDTO>();
            //RoomDAL objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
            //if (CompanyID.HasValue && CompanyID.Value > 0)
            //{
            //    SessionHelper.CompanyID = (Int64)CompanyID;
            //    SessionHelper.CompanyName = CompanyName;



            //    if (SessionHelper.UserType == 1)
            //    {
            //        if (SessionHelper.RoleID == -1)
            //        {
            //            lstRoomDTO = objRoomDAL.GetAllRoomByCompany(SessionHelper.CompanyID, false, false).ToList();
            //        }
            //        else
            //        {
            //            EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
            //            List<long> lstdistinctRoom = objEnterpriseMasterDAL.GetDistinctMasterRoomID(SessionHelper.UserID, SessionHelper.RoleID, SessionHelper.UserType, SessionHelper.EnterPriceID, SessionHelper.CompanyID);
            //            lstRoomDTO = (from s in objRoomDAL.GetAllRecords(SessionHelper.CompanyID, false, false).ToList()
            //                          where lstdistinctRoom.Contains(s.ID)
            //                          select s).ToList();
            //        }
            //    }
            //    else if (SessionHelper.UserType == 2)
            //    {
            //        if (SessionHelper.RoleID == -2)
            //        {
            //            lstRoomDTO = objRoomDAL.GetAllRoomByCompany(SessionHelper.CompanyID, false, false).ToList();
            //        }
            //        else
            //        {
            //            eTurns.DAL.UserMasterDAL objuser = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
            //            List<long> lstdistinctRoom = objuser.GetDistinctChildRoomID(SessionHelper.UserID, SessionHelper.RoleID, SessionHelper.UserType, SessionHelper.CompanyID);
            //            lstRoomDTO = (from s in objRoomDAL.GetAllRecords(SessionHelper.CompanyID, false, false).ToList()
            //                          where lstdistinctRoom.Contains(s.ID)
            //                          select s).ToList();
            //        }

            //    }
            //    else
            //    {
            //        lstRoomDTO = objRoomDAL.GetAllRoomByCompany(SessionHelper.CompanyID, false, false).ToList();
            //    }

            //}
            //else
            //{
            //    lstRoomDTO = objRoomDAL.GetAllRoomByCompany(SessionHelper.CompanyID, false, false).ToList();
            //}
            ////List<KeyValuePair<long, string>> TempRoomList = new List<KeyValuePair<long, string>>();

            ////foreach (var item in lstRoomDTO)
            ////{
            ////    TempRoomList.Add(new KeyValuePair<long, string>(item.ID, item.RoomName));
            ////}
            //SessionHelper.RoomList = lstRoomDTO;
            //if (lstRoomDTO != null && lstRoomDTO.Count > 0)
            //{
            //    SessionHelper.RoomID = lstRoomDTO.First().ID;
            //    SessionHelper.RoomName = lstRoomDTO.First().RoomName;

            //}
            //else
            //{
            //    SessionHelper.RoomID = 0;
            //    SessionHelper.RoomName = string.Empty;
            //}

            //if (SessionHelper.RoleID == -1 || SessionHelper.RoleID == -2)
            //    SessionHelper.RoomPermissions.ForEach(t => t.RoomID = SessionHelper.RoomID);

            //List<UserRoleModuleDetailsDTO> PermissionList = new List<UserRoleModuleDetailsDTO>();
            //List<UserWiseRoomsAccessDetailsDTO> UserWiseAllRoomsAccessDetails = new List<UserWiseRoomsAccessDetailsDTO>();
            //List<UserRoomReplanishmentDetailsDTO> ReplenishingRooms = new List<UserRoomReplanishmentDetailsDTO>();

            //eTurnsMaster.DAL.UserMasterDAL objUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
            //PermissionList = objUserMasterDAL.GetUserRoleModuleDetailsRecord(SessionHelper.UserID, SessionHelper.RoleID, SessionHelper.UserType);

            //if (SessionHelper.RoleID == -1 || SessionHelper.RoleID == -2)
            //    PermissionList.ForEach(t => t.RoomId = SessionHelper.RoomID);

            //string RoomLists = "";
            //if (PermissionList != null && PermissionList.Count > 0)
            //{
            //    UserWiseAllRoomsAccessDetails = objUserMasterDAL.ConvertUserPermissions(PermissionList, SessionHelper.RoleID, ref RoomLists);
            //    if (SessionHelper.RoleID == -1 || SessionHelper.RoleID == -2)
            //        UserWiseAllRoomsAccessDetails.ForEach(t => t.RoomID = SessionHelper.RoomID);
            //    // objDTO.SelectedRoomAccessValue = RoomLists;
            //}


            //return Json(new { Message = "success", Status = "ok" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetEnterpriseData()
        {
            EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
            var data = objEnterpriseMasterDAL.GetAllEnterprise(false).OrderBy(e => e.Name).ToList();
            return Json(data);
        }

        [HttpPost]
        public JsonResult GetCompanyData(string EnterpriseIds, long? RoleType)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();

            long[] EIDS = new long[] { };
            if (!string.IsNullOrWhiteSpace(EnterpriseIds))
            {

                EIDS = s.Deserialize<long[]>(EnterpriseIds);
                if (EIDS.Length > 0)
                {
                    EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
                    List<CompanyMasterDTO> lstCompany = objEnterpriseMasterDAL.GetCompaniesOfEnterPrise(EIDS).OrderBy(t => t.Name).ToList();
                    lstCompany = lstCompany.Where(t => (t.IsDeleted ?? false) == false).ToList();
                    return Json(lstCompany);
                }
                else
                {
                    return Json(new List<CompanyMasterDTO>());
                }
            }
            else
            {
                return Json(new List<CompanyMasterDTO>());
            }
        }

        [HttpPost]
        public JsonResult GetRoomData(string companyids)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();

            string[] EIDS = new string[] { };
            //RoomController obj = new RoomController();
            if (!string.IsNullOrWhiteSpace(companyids))
            {
                List<CompanyMasterDTO> lstCompany = new List<CompanyMasterDTO>();
                EIDS = s.Deserialize<string[]>(companyids);
                foreach (string item in EIDS)
                {
                    CompanyMasterDTO objCompanyMasterDTO = new CompanyMasterDTO();
                    long[] arr = item.Split('_').ToIntArray();
                    objCompanyMasterDTO.EnterPriseId = arr[0];
                    objCompanyMasterDTO.ID = arr[1];
                    lstCompany.Add(objCompanyMasterDTO);
                }
                if (lstCompany.Count > 0)
                {
                    EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
                    List<RoomDTO> data = objEnterpriseMasterDAL.GetRoomsByCompany(lstCompany).OrderBy(t => t.RoomName).ToList();
                    data = data.Where(t => (t.IsDeleted ?? false) == false).ToList();
                    return Json(new { DDData = data }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { DDData = new List<RoomDTO>() }, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Json(new { DDData = new List<RoomDTO>() }, JsonRequestBehavior.AllowGet);

            }
        }
        public JsonResult GetRoomList(int CompanyID)
        {
            //RoomController obj = new RoomController();
            RoomDAL obj = new RoomDAL(SessionHelper.EnterPriseDBName);
            //var data = obj.GetAllRecords(CompanyID).OrderBy(e => e.ID);
            var data = obj.GetAllRecords(CompanyID, false, false).OrderBy(e => e.RoomName);

            return Json(new { DDData = data }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRoomListFromArray(string CompanyID = "")
        {
            //RoomController obj = new RoomController();
            RoomDAL obj = new RoomDAL(SessionHelper.EnterPriseDBName);
            JavaScriptSerializer s = new JavaScriptSerializer();
            List<RoomDTO> finaldata = new List<RoomDTO>();
            if (CompanyID != "")
            {
                int[] intCompanyID = s.Deserialize<int[]>(CompanyID);
                //var data = obj.GetAllRecords(CompanyID).OrderBy(e => e.ID);

                foreach (int item in intCompanyID)
                {
                    List<RoomDTO> data = new List<RoomDTO>();
                    data = obj.GetAllRecords(item, false, false).OrderBy(e => e.RoomName).ToList();
                    foreach (RoomDTO iteminner in data)
                    {
                        finaldata.Add(iteminner);

                    }

                }
            }

            return Json(new { DDData = finaldata }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCompanyListForRole(int EnterPriceID)
        {
            //CompanyMasterController obj = new CompanyMasterController();
            CompanyMasterDAL obj = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
            //var data = obj.GetAllRecords(CompanyID).OrderBy(e => e.ID);
            var data = obj.GetAllRecords().OrderBy(e => e.Name);

            return Json(new { DDData = data }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRoleList(string CompanyID)
        {
            //RoleMasterController obj = new RoleMasterController();
            eTurns.DAL.RoleMasterDAL obj = new eTurns.DAL.RoleMasterDAL(SessionHelper.EnterPriseDBName);
            //List<RoleMasterDTO> data = obj.GetAllRecord(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(e => e.ID).ToList();
            List<RoleMasterDTO> data = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(e => e.RoleName).ToList();
            RoleMasterDTO objrole = new RoleMasterDTO();
            objrole.ID = 0;
            objrole.RoleName = "Select Role";
            data.Insert(0, objrole);

            return Json(new { DDData = data }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult getRoleListByUserType(int? UserType, long? CompanyId, long? EnterpriseId)
        {
            List<RoleMasterDTO> lstRoles = new List<RoleMasterDTO>();
            if (UserType.HasValue && UserType.Value > 0)
            {
                if (UserType.Value == 1)
                {
                    eTurnsMaster.DAL.RoleMasterDAL objRoleMasterDAL = new eTurnsMaster.DAL.RoleMasterDAL(SessionHelper.EnterPriseDBName);
                    lstRoles = objRoleMasterDAL.GetAllRoles(1).OrderBy(e => e.RoleName).ToList();
                }
                if (UserType.Value == 2 || UserType.Value == 3)
                {
                    eTurns.DAL.RoleMasterDAL obj;
                    EnterpriseDTO oEnt = new EnterpriseMasterDAL().GetEnterprise(EnterpriseId ?? 0);
                    if (oEnt != null)
                        obj = new eTurns.DAL.RoleMasterDAL(oEnt.EnterpriseDBName);
                    else
                        obj = new eTurns.DAL.RoleMasterDAL(SessionHelper.EnterPriseDBName);

                    lstRoles = obj.GetAllRoleByUserType(UserType.Value, CompanyId).OrderBy(e => e.RoleName).ToList();
                }
            }
            RoleMasterDTO objrole = new RoleMasterDTO();
            objrole.ID = 0;
            objrole.RoleName = "Select Role";
            lstRoles.Insert(0, objrole);
            return Json(lstRoles);
        }

        public JsonResult DeleteUserRecords(string ids)
        {
            try
            {

                UserBAL objUserMasterDAL = new UserBAL();
                objUserMasterDAL.DeleteRecords(ids, (int)SessionHelper.UserID);
                //return "ok";
                //string response = string.Empty;
                //CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                //response = objCommonDAL.DeleteModulewise(ids, "User", false, SessionHelper.UserID);
                //eTurns.DAL.CacheHelper<IEnumerable<UserMasterDTO>>.InvalidateCache();
                return Json(new { Message = "ok", Status = "ok" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// redirect user to default url
        /// </summary>
        /// <returns></returns>
        public ActionResult RedirectDefault()
        {
            return View();
        }

        #endregion

        #region "EnterpriseMaster"

        public ActionResult EnterpriseList()
        {
            return View();
        }
        [AjaxOrChildActionOnlyAttribute]
        public PartialViewResult _CreateEnterprise()
        {
            return PartialView();
        }

        [AjaxOrChildActionOnlyAttribute]
        public ActionResult EnterpriseCreate()
        {
            EnterpriseDTO objDTO = new EnterpriseDTO();
            ResourceDAL objDAL = new ResourceDAL(DbConHelper.GetETurnsDBName());
            objDTO.ID = 0;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.GUID = Guid.NewGuid();
            objDTO.CreatedByName = SessionHelper.UserName;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.IsActive = true;
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("Enterprise");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            objDTO.lstResourceAll = objDAL.GetETurnsResourceLanguageData().ToList();
            return PartialView("_CreateEnterprise", objDTO);
        }

        [HttpPost]
        public JsonResult EnterpriseSave(EnterpriseDTO objDTO)
        {
            string message = "";
            string status = "";
            EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
            CommonMasterDAL objCommonMasterDAL = new CommonMasterDAL();

            if (string.IsNullOrEmpty(objDTO.Name))
            {
                message = string.Format(ResMessage.Required, ResEnterprise.EnterpriseName); // "Enterprise name is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }

            if (objDTO.ID == 0)
            {
                objDTO.CreatedBy = SessionHelper.UserID;

                string strOK = objCommonMasterDAL.EnterPriseDuplicateCheck(objDTO.ID, objDTO.Name);

                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResEnterprise.EnterpriseName, objDTO.Name);  //  "Enterprise \"" + objDTO.Name + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    strOK = objCommonMasterDAL.UserDuplicateCheckUserName(objDTO.EnterpriseUserID, objDTO.UserName);
                    if (strOK == "duplicate")
                    {
                        message = "User name \"" + objDTO.UserName + "\" already exist! Try with Another!";
                        status = "duplicate";
                        return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                    }
                    objDTO.GUID = Guid.NewGuid();

                    long ID = 0;
                    string PasswordWithoutHash = objDTO.EnterpriseUserPassword;
                    objDTO.EnterpriseUserPassword = CommonUtility.getSHA15Hash(objDTO.EnterpriseUserPassword);
                    objDTO = objEnterpriseMasterDAL.Insert(objDTO);
                    ID = objDTO.ID;

                    if (SessionHelper.RoleID == -1)
                    {
                        List<EnterpriseDTO> lstEnterpriseDTO = SessionHelper.EnterPriseList;
                        if (lstEnterpriseDTO != null && lstEnterpriseDTO.Count > 0)
                        {
                            lstEnterpriseDTO.Add(objDTO);
                        }
                        else
                        {
                            lstEnterpriseDTO = new List<EnterpriseDTO>();
                            lstEnterpriseDTO.Add(objDTO);
                            SessionHelper.EnterPriceID = objDTO.ID;
                            SessionHelper.EnterPriceName = objDTO.Name;
                            SessionHelper.EnterPriseDBName = objDTO.EnterpriseDBName;
                            SessionHelper.EnterPriseDBConnectionString = objDTO.EnterpriseDBConnectionString;
                        }
                        SessionHelper.EnterPriseList = lstEnterpriseDTO;
                    }
                    else if (SessionHelper.UserType == 1)
                    {
                        UserBAL objUserBAL = new UserBAL();
                        List<EnterpriseDTO> lstEnterpriseDTO = SessionHelper.EnterPriseList;
                        if (lstEnterpriseDTO != null && lstEnterpriseDTO.Count > 0)
                        {
                            lstEnterpriseDTO.Add(objDTO);
                        }
                        else
                        {
                            lstEnterpriseDTO = new List<EnterpriseDTO>();
                            lstEnterpriseDTO.Add(objDTO);
                            SessionHelper.EnterPriceID = objDTO.ID;
                            SessionHelper.EnterPriceName = objDTO.Name;
                            SessionHelper.EnterPriseDBName = objDTO.EnterpriseDBName;
                            SessionHelper.EnterPriseDBConnectionString = objDTO.EnterpriseDBConnectionString;
                        }
                        objUserBAL.AddNewEPPermissions(objDTO.ID, SessionHelper.UserID, SessionHelper.RoleID, SessionHelper.UserType);
                        SessionHelper.EnterPriseList = lstEnterpriseDTO;
                    }

                    if (ID > 0)
                    {
                        try
                        {
                            if (objDTO.ID > 0)
                            {
                                EnterpriseDAL objEnterpriseDAL = new EnterpriseDAL(objDTO.EnterpriseDBName);
                                objEnterpriseDAL.Insert(objDTO);
                                UserMasterDTO objEnterpriseUser = new eTurnsMaster.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName).GetUserByName(objDTO.UserName);
                                if (SessionHelper.EnterPriceID > 0)
                                {
                                    SendMailToCreateNewUser(objDTO.UserName, PasswordWithoutHash, objDTO.EnterpriseUserEmail ?? string.Empty, objEnterpriseUser);
                                }
                            }
                            string appPath1 = ResourceHelper.ResourceDirectoryBasePath + @"\";
                            string resourceDirectory = appPath1 + ID;
                            if (!System.IO.Directory.Exists(resourceDirectory))
                            {
                                System.IO.Directory.CreateDirectory(resourceDirectory);
                            }
                            string[] files = System.IO.Directory.GetFiles(resourceDirectory);
                            if (files == null || files.Length <= 0)
                            {
                                #region "EnterpriseResources"

                                if (!System.IO.Directory.Exists(resourceDirectory + "\\EnterpriseResources"))
                                {
                                    System.IO.Directory.CreateDirectory(resourceDirectory + "\\EnterpriseResources");
                                }
                                foreach (var file in System.IO.Directory.GetFiles(appPath1 + "MasterResources\\EnterpriseResources"))
                                {
                                    System.IO.File.Copy(file, System.IO.Path.Combine(resourceDirectory + "\\EnterpriseResources", System.IO.Path.GetFileName(file)));
                                }

                                # endregion

                                #region "CompanyResources"

                                if (!System.IO.Directory.Exists(resourceDirectory + "\\CompanyResources"))
                                {
                                    System.IO.Directory.CreateDirectory(resourceDirectory + "\\CompanyResources");
                                }
                                foreach (var file in System.IO.Directory.GetFiles(appPath1 + "MasterResources\\CompanyResources"))
                                {
                                    System.IO.File.Copy(file, System.IO.Path.Combine(resourceDirectory + "\\CompanyResources", System.IO.Path.GetFileName(file)));
                                }

                                # endregion
                                #region "RoomResources"

                                if (!System.IO.Directory.Exists(resourceDirectory + "\\RoomResources"))
                                {
                                    System.IO.Directory.CreateDirectory(resourceDirectory + "\\RoomResources");
                                }
                                foreach (var file in System.IO.Directory.GetFiles(appPath1 + "MasterResources\\RoomResources"))
                                {
                                    System.IO.File.Copy(file, System.IO.Path.Combine(resourceDirectory + "\\RoomResources", System.IO.Path.GetFileName(file)));
                                }

                                # endregion
                            }

                            #region "CopyMobileResource"
                            //wi-1309
                            ReportBuilderController objRDLC = new ReportBuilderController();
                            if (objDTO != null)
                                objRDLC.UpdateChildEntReportForSingle(true, objDTO);

                            bool bResult = objEnterpriseMasterDAL.UpdateResource(DbConnectionHelper.GetETurnsMasterDBName(), objDTO.EnterpriseDBName);

                            #endregion

                            #region "RDLC Report"
                            string BaseReportPath = string.Empty;
                            string EntReportPath = string.Empty;
                            BaseReportPath = ResourceHelper.RDLReportDirectoryBasePath + @"\MasterReport";
                            EntReportPath = ResourceHelper.RDLReportDirectoryBasePath + @"\" + ID + @"\" + @"\BaseReport";
                            if (System.IO.Directory.Exists(BaseReportPath))
                            {
                                if (!System.IO.Directory.Exists(EntReportPath))
                                {
                                    System.IO.Directory.CreateDirectory(EntReportPath);
                                }
                                foreach (var file in System.IO.Directory.GetFiles(BaseReportPath))
                                {
                                    System.IO.File.Copy(file, System.IO.Path.Combine(EntReportPath, System.IO.Path.GetFileName(file)));
                                }
                            }
                            objEnterpriseMasterDAL.UpdateRDLCReportMaster(DbConnectionHelper.GetETurnsMasterDBName(), objDTO.EnterpriseDBName, SessionHelper.UserID, false);

                            #endregion
                        }
                        catch (Exception)
                        {
                        }
                        finally
                        {
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
                objDTO.UpdatedByName = SessionHelper.UserName;
                string strOK = objCommonMasterDAL.EnterPriseDuplicateCheck(objDTO.ID, objDTO.Name);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResEnterprise.EnterpriseName, objDTO.Name);  //"Enterprise \"" + objDTO.Name + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    //strOK = objCommonMasterDAL.UserDuplicateCheckUserName(objDTO.EnterpriseUserID, objDTO.UserName);
                    //if (strOK == "duplicate")
                    //{
                    //    message = "User name \"" + objDTO.UserName + "\" already exist! Try with Another!";
                    //    status = "duplicate";
                    //    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                    //}
                    //else
                    //{


                    //if (objDTO.IsPasswordChanged == "1")
                    //{
                    objDTO.EnterpriseUserPassword = CommonUtility.getSHA15Hash(objDTO.EnterpriseUserPassword);
                    //}                    
                    objDTO.EnterpriseDBName = objEnterpriseMasterDAL.GetEnterprise(objDTO.ID).EnterpriseDBName;


                    objEnterpriseMasterDAL.SetEnterpriseSuperAdmin(objDTO);
                    eTurns.DAL.UserMasterDAL objUserMasterDal = new eTurns.DAL.UserMasterDAL(objDTO.EnterpriseDBName);
                    Int64 NewEnterpriseSuperadmin = objUserMasterDal.SetEnterpriseSuperAdmin(objDTO);

                    objDTO.EnterpriseSuperAdmin = NewEnterpriseSuperadmin;



                    objDTO = objEnterpriseMasterDAL.Edit(objDTO);
                    if (objDTO.ID > 0)
                    {
                        EnterpriseDAL objEnterpriseDAL = new EnterpriseDAL(objDTO.EnterpriseDBName);
                        objEnterpriseDAL.Edit(objDTO);
                    }
                    if (SessionHelper.EnterPriseList.Any(t => t.ID == objDTO.ID))
                    {
                        List<EnterpriseDTO> lstCurrentEPList = SessionHelper.EnterPriseList;
                        lstCurrentEPList = lstCurrentEPList.Where(t => t.ID != objDTO.ID).ToList();
                        lstCurrentEPList.Add(objDTO);
                        SessionHelper.EnterPriseList = lstCurrentEPList;
                    }
                    if (objDTO != null)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                        status = "fail";
                    }
                    //}
                }
            }

            return Json(new { Message = message, Status = status, retDto = objDTO }, JsonRequestBehavior.AllowGet);
        }

        private CompanyConfigDTO AssignValue(EnterPriseConfigDTO objConfig)
        {
            CompanyConfigDTO obj = new CompanyConfigDTO();

            obj.ID = 0;
            obj.ScheduleDaysBefore = objConfig.ScheduleDaysBefore;
            obj.OperationalHoursBefore = objConfig.OperationalHoursBefore;
            obj.MileageBefore = objConfig.MileageBefore;
            obj.ProjectAmountExceed = objConfig.ProjectAmountExceed;
            obj.ProjectItemQuantitExceed = objConfig.ProjectItemQuantitExceed;
            obj.ProjectItemAmountExceed = objConfig.ProjectItemAmountExceed;
            obj.CostDecimalPoints = objConfig.CostDecimalPoints;
            obj.QuantityDecimalPoints = objConfig.QuantityDecimalPoints;
            obj.DateFormat = objConfig.DateFormat;
            obj.NOBackDays = objConfig.NOBackDays;
            obj.NODaysAve = objConfig.NODaysAve;
            obj.NOTimes = objConfig.NOTimes;
            obj.MinPer = objConfig.MinPer;
            obj.MaxPer = objConfig.MaxPer;
            obj.CurrencySymbol = objConfig.CurrencySymbol;
            obj.AEMTPndOrders = objConfig.AEMTPndOrders;
            obj.AEMTPndRequisition = objConfig.AEMTPndRequisition;
            obj.AEMTPndTransfer = objConfig.AEMTPndTransfer;
            obj.AEMTSggstOrdMin = objConfig.AEMTSggstOrdMin;
            obj.AEMTSggstOrdCrt = objConfig.AEMTSggstOrdCrt;
            obj.AEMTAssetMntDue = objConfig.AEMTAssetMntDue;
            obj.AEMTToolsMntDue = objConfig.AEMTToolsMntDue;
            obj.AEMTItemStockOut = objConfig.AEMTItemStockOut;
            obj.AEMTCycleCount = objConfig.AEMTCycleCount;
            obj.AEMTCycleCntItmMiss = objConfig.AEMTCycleCntItmMiss;
            obj.AEMTItemUsageRpt = objConfig.AEMTItemUsageRpt;
            obj.AEMTItemReceiveRpt = objConfig.AEMTItemReceiveRpt;
            obj.GridRefreshTimeInSecond = objConfig.GridRefreshTimeInSecond;
            obj.DateFormatCSharp = objConfig.DateFormatCSharp;
            obj.IsPackSlipRequired = false;

            return obj;
        }

        [AjaxOrChildActionOnlyAttribute]
        public ActionResult EnterpriseEdit(Int64 ID)
        {
            //EnterpriseController obj = new EnterpriseController();
            eTurnsMaster.DAL.EnterpriseMasterDAL obj = new eTurnsMaster.DAL.EnterpriseMasterDAL();
            ResourceDAL objDAL = new ResourceDAL(SessionHelper.EnterPriseDBName);

            EnterpriseDTO objDTO = obj.GetEnterprise(ID);
            if (objDTO != null)
            {

                objDTO.CreatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                objDTO.UpdatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
            }
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("Enterprise", ID);
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;
            objDTO.EnterpriseUserPassword = string.Empty;
            objDTO.lstResourceAll = objDAL.GetETurnsResourceLanguageData().ToList();
            if (objDTO != null && objDTO.EnterpriseDBName != string.Empty)
            {
                ResourceDAL objEnterprise = new ResourceDAL(objDTO.EnterpriseDBName);
                objDTO.strResourceSelected = objEnterprise.GetResourceLanguageData().Select(x => x.Culture).ToArray();


            }

            objDTO.EnterpriseUserPassword = "DVlp@@123";
            return PartialView("_CreateEnterprise", objDTO);
            //return View("EnterpriseEdit",objDTO);
        }

        [HttpPost]
        public JsonResult GetEnterprises()
        {
            StringBuilder objstr = new StringBuilder("");
            if (SessionHelper.EnterPriseList != null && SessionHelper.EnterPriseList.Count() > 0)
            {
                foreach (var item in SessionHelper.EnterPriseList.OrderBy(t => t.Name))
                {
                    if (SessionHelper.EnterPriceID == item.ID)
                    {
                        objstr.Append("<option selected='selected' value='" + item.ID + "'>" + item.Name + "</option>");
                    }
                    else
                    {
                        objstr.Append("<option value='" + item.ID + "'>" + item.Name + "</option>");
                    }
                }
            }
            return Json(objstr.ToString());
        }

        [HttpPost]
        public JsonResult GetCompanies()
        {
            StringBuilder objstr = new StringBuilder("");
            if (SessionHelper.CompanyList != null && SessionHelper.CompanyList.Count(t => t.EnterPriseId == SessionHelper.EnterPriceID) > 0)
            {
                foreach (var item in SessionHelper.CompanyList.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).OrderBy(t => t.Name))
                {
                    if (SessionHelper.CompanyID == item.ID)
                    {
                        if (item.IsActive)
                        {
                            objstr.Append("<option selected='selected' value='" + item.ID + "'>" + item.Name + "</option>");
                        }
                        else
                        {
                            objstr.Append("<option selected='selected' style='background:lightgrey;' value='" + item.ID + "'>" + item.Name + "</option>");
                        }
                    }
                    else
                    {
                        if (item.IsActive)
                        {
                            objstr.Append("<option value='" + item.ID + "'>" + item.Name + "</option>");
                        }
                        else
                        {
                            objstr.Append("<option  style='background:lightgrey;' value='" + item.ID + "'>" + item.Name + "</option>");
                        }
                    }
                }
            }
            return Json(objstr.ToString());
        }

        [HttpPost]
        public JsonResult GetRoomList()
        {
            StringBuilder objstr = new StringBuilder("");

            if (SessionHelper.RoomList != null && SessionHelper.RoomList.Any(t => t.CompanyID == SessionHelper.CompanyID && t.EnterpriseId == SessionHelper.EnterPriceID))
            {
                foreach (var item in SessionHelper.RoomList.Where(t => t.CompanyID == SessionHelper.CompanyID && t.EnterpriseId == SessionHelper.EnterPriceID).OrderBy(t => t.RoomName))
                {
                    if (SessionHelper.RoomID == item.ID)
                    {
                        if (item.IsRoomActive)
                        {
                            objstr.Append("<option selected='selected' value='" + item.ID + "'>" + item.RoomName + "</option>");
                        }
                        else
                        {
                            objstr.Append("<option selected='selected' style='background:lightgrey;' style='background:lightgrey;' value='" + item.ID + "'>" + item.RoomName + "</option>");
                        }
                    }
                    else
                    {
                        if (item.IsRoomActive)
                        {
                            objstr.Append("<option value='" + item.ID + "'>" + item.RoomName + "</option>");
                        }
                        else
                        {
                            objstr.Append("<option  style='background:lightgrey;' value='" + item.ID + "'>" + item.RoomName + "</option>");
                        }
                    }
                }
            }
            return Json(objstr.ToString());
        }

        #region Ajax Data Provider

        public ActionResult EnterpriseListAjax(JQueryDataTableParamModel param)
        {
            //EnterpriseController obj = new EnterpriseController();
            EnterpriseMasterDAL obj = new EnterpriseMasterDAL();
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

            //if (sortDirection == "asc")
            //    sortColumnName = sortColumnName + " asc";
            //else
            //    sortColumnName = sortColumnName + " desc";

            if (sortColumnName == "0" || sortColumnName == "undefined" || string.IsNullOrEmpty(sortColumnName))
                sortColumnName = "ID Desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            var DataFromDB = obj.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, IsArchived, IsDeleted);
            //if (DataFromDB != null)
            //{
            //    DataFromDB.ToList().ForEach(t =>
            //    {
            //        t.CreatedDate = CommonUtility.ConvertDateByTimeZone(t.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
            //        t.UpdatedDate = CommonUtility.ConvertDateByTimeZone(t.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
            //    });
            //}

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            }, JsonRequestBehavior.AllowGet);


        }

        public string UpdateEnterpriseData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            //EnterpriseController obj = new EnterpriseController();
            EnterpriseMasterDAL obj = new EnterpriseMasterDAL();
            obj.UpdateData(id, value, rowId, columnPosition, columnId, columnName);
            return value;
        }

        public string DuplicateEnterpriseCheck(string EnterpriseName, string ActionMode, int ID)
        {
            //EnterpriseController obj = new EnterpriseController();
            CommonDAL obj = new CommonDAL(SessionHelper.EnterPriseDBName);
            return obj.DuplicateCheck(EnterpriseName, ActionMode, ID, "EnterpriseMaster", "Name", SessionHelper.RoomID, SessionHelper.CompanyID);
        }

        public string DeleteEnterpriseRecords(string ids)
        {
            try
            {
                //EnterpriseController obj = new EnterpriseController();
                EnterpriseMasterDAL obj = new EnterpriseMasterDAL();
                obj.DeleteRecords(ids, (int)SessionHelper.UserID);
                if (eTurnsWeb.Helper.SessionHelper.EnterPriseList != null && eTurnsWeb.Helper.SessionHelper.EnterPriseList.Count > 0)
                {
                    List<EnterpriseDTO> lstEnterPrises = eTurnsWeb.Helper.SessionHelper.EnterPriseList.ToList();
                    foreach (var item in ids.Split(','))
                    {
                        if (!string.IsNullOrEmpty(item.Trim()))
                        {
                            lstEnterPrises.RemoveAll(t => t.ID == Convert.ToInt64(item));
                        }
                    }
                    eTurnsWeb.Helper.SessionHelper.EnterPriseList = lstEnterPrises;
                }
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string UnDeleteEnterpriseRecords(string ids)
        {
            try
            {
                //EnterpriseController obj = new EnterpriseController();
                EnterpriseMasterDAL obj = new EnterpriseMasterDAL();
                obj.UnDeleteRecords(ids, (int)SessionHelper.UserID);
                EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
                SessionHelper.EnterPriseList = objEnterpriseMasterDAL.GetEnterPriseByUser(SessionHelper.UserID, SessionHelper.RoleID, SessionHelper.UserType);
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion

        public ActionResult EnterpriseUDF(long? id)
        {
            string UDFTableName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFTableFromKey("enterprises");
            ViewBag.UDFTableName = UDFTableName;
            ViewBag.UDFTableNameKey = "enterprises";
            //UDFApiController obj = new UDFApiController();
            eTurnsMaster.DAL.UDFDAL obj = new eTurnsMaster.DAL.UDFDAL(SessionHelper.EnterPriseDBName);
            eTurnsMaster.DAL.UDFDAL objUDFDALMaster = new eTurnsMaster.DAL.UDFDAL(SessionHelper.EnterPriseDBName);
            objUDFDALMaster.InsertDefaultUDFs(UDFTableName, SessionHelper.UserID, id ?? 0);
            return PartialView("_EnterpriseUDF");
        }

        [HttpPost]
        public JsonResult EditEnterprisesResource(string EnterpriseID, string NewResource)
        {
            string strResult = string.Empty;
            try
            {
                string[] strNewResource = NewResource.Split(',');
                var NewEnterpriseRequest = strNewResource;

                var RemoveColumn = (IEnumerable<string>)null;
                var AddColumn = (IEnumerable<string>)null;

                eTurnsMaster.DAL.EnterpriseMasterDAL obj = new eTurnsMaster.DAL.EnterpriseMasterDAL();
                EnterpriseDTO objDTO = obj.GetEnterprise(Convert.ToInt64(EnterpriseID));
                ResourceDAL objEnterprise = new ResourceDAL(objDTO.EnterpriseDBName);
                ResourceDAL objeTurns = new ResourceDAL(SessionHelper.EnterPriseDBName);

                List<ResourceLanguageDTO> lstEnterpriseResourceDTO = objEnterprise.GetResourceLanguageData().ToList();
                List<ResourceLanguageDTO> lsteTurnsResourceDTO = objeTurns.GetETurnsResourceLanguageData().ToList();

                var AllEnterpriseID = from f in lstEnterpriseResourceDTO select f.Culture.ToString();
                AddColumn = NewEnterpriseRequest.Except(AllEnterpriseID);
                RemoveColumn = AllEnterpriseID.Except(NewEnterpriseRequest);

                if (RemoveColumn != (IEnumerable<string>)null || AddColumn != (IEnumerable<string>)null)
                {
                    if (AddColumn != (IEnumerable<string>)null) // en-US Not allowed to add secind time
                    {
                        AddColumn = from item in AddColumn
                                    where item != "en-US"
                                    select item;
                    }
                    if (RemoveColumn != (IEnumerable<string>)null) // en-US Not allowed to Remove
                    {
                        RemoveColumn = from item in RemoveColumn
                                       where item != "en-US"
                                       select item;
                    }

                    string BasePathResource = System.Web.HttpContext.Current.Server.MapPath(@"\Resources\");
                    string BaseCompanyResourcesFolder = BasePathResource + "MasterResources\\CompanyResources";
                    string BaseEnterpriseResourcesFolder = BasePathResource + "MasterResources\\EnterpriseResources";
                    string BaseRoomResourcesFolder = BasePathResource + "MasterResources\\RoomResources";


                    DirectoryInfo BaseCompanyResource = new DirectoryInfo(BaseCompanyResourcesFolder);
                    DirectoryInfo BaseEnterpriseResource = new DirectoryInfo(BaseEnterpriseResourcesFolder);
                    DirectoryInfo BaseRoomResource = new DirectoryInfo(BaseRoomResourcesFolder);

                    foreach (var item in AddColumn) // Add Operation 
                    {
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(item)))
                        {
                            ResourceLanguageDTO ResourceDTO = lsteTurnsResourceDTO.Where(x => x.Culture == item).FirstOrDefault();
                            FileInfo[] BaseCompanyFiles = BaseCompanyResource.GetFiles("*.resx");
                            FileInfo[] BaseEnterPriseFiles = BaseEnterpriseResource.GetFiles("*.resx");
                            FileInfo[] BaseRoomFiles = BaseRoomResource.GetFiles("*.resx");

                            foreach (var CFiles in BaseCompanyFiles)
                            {
                                string FileName = CFiles.Name.Split('.')[0];
                                if (!string.IsNullOrEmpty(FileName))
                                {
                                    if (!(System.IO.File.Exists(BaseCompanyResource + "\\" + FileName + ".resx")))
                                    {
                                        System.IO.File.Copy(BaseCompanyResource + "\\" + CFiles.Name, BaseCompanyResource + "\\" + FileName + ".resx");
                                    }
                                    if (!(System.IO.File.Exists(BaseCompanyResource + "\\" + FileName + "." + ResourceDTO.Culture + ".resx")))
                                    {
                                        System.IO.File.Copy(BaseCompanyResource + "\\" + FileName + ".resx", BaseCompanyResource + "\\" + FileName + "." + ResourceDTO.Culture + ".resx");
                                    }
                                }
                            }
                            foreach (var CFiles in BaseEnterPriseFiles)
                            {
                                string FileName = CFiles.Name.Split('.')[0];
                                if (!string.IsNullOrEmpty(FileName))
                                {
                                    if (!(System.IO.File.Exists(BaseEnterpriseResource + "\\" + FileName + ".resx")))
                                    {
                                        System.IO.File.Copy(BaseEnterpriseResource + "\\" + CFiles.Name, BaseEnterpriseResource + "\\" + FileName + ".resx");
                                    }
                                    if (!(System.IO.File.Exists(BaseEnterpriseResource + "\\" + FileName + "." + ResourceDTO.Culture + ".resx")))
                                    {
                                        System.IO.File.Copy(BaseEnterpriseResource + "\\" + FileName + ".resx", BaseEnterpriseResource + "\\" + FileName + "." + ResourceDTO.Culture + ".resx");
                                    }
                                }
                            }
                            foreach (var CFiles in BaseRoomFiles)
                            {
                                string FileName = CFiles.Name.Split('.')[0];
                                if (!string.IsNullOrEmpty(FileName))
                                {
                                    if (!(System.IO.File.Exists(BaseRoomResource + "\\" + FileName + ".resx")))
                                    {
                                        System.IO.File.Copy(BaseRoomResource + "\\" + CFiles.Name, BaseRoomResource + "\\" + FileName + ".resx");
                                    }
                                    if (!(System.IO.File.Exists(BaseRoomResource + "\\" + FileName + "." + ResourceDTO.Culture + ".resx")))
                                    {
                                        System.IO.File.Copy(BaseRoomResource + "\\" + FileName + ".resx", BaseRoomResource + "\\" + FileName + "." + ResourceDTO.Culture + ".resx");
                                    }
                                }
                            }
                        }
                    }
                    string E_CompanyResourcesFolder = BasePathResource + EnterpriseID + "\\CompanyResources";
                    string E_EnterpriseResourcesFolder = BasePathResource + EnterpriseID + "\\EnterpriseResources";
                    string E_RoomResourcesFolder = BasePathResource + EnterpriseID + "\\RoomResources";

                    DirectoryInfo E_CompanyResource = new DirectoryInfo(E_CompanyResourcesFolder);
                    DirectoryInfo E_EnterpriseResource = new DirectoryInfo(E_EnterpriseResourcesFolder);
                    DirectoryInfo E_RoomResource = new DirectoryInfo(E_RoomResourcesFolder);

                    #region  Add Operation
                    foreach (var item in AddColumn) // Add Operation 
                    {
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(item)))
                        {
                            ResourceLanguageDTO ResourceDTO = lsteTurnsResourceDTO.Where(x => x.Culture == item).FirstOrDefault();
                            string strSource, strDest;

                            FileInfo[] BaseCompanyFiles = BaseCompanyResource.GetFiles("*." + ResourceDTO.Culture + ".resx");
                            FileInfo[] BaseEnterPriseFiles = BaseEnterpriseResource.GetFiles("*." + ResourceDTO.Culture + ".resx");
                            FileInfo[] BaseRoomFiles = BaseRoomResource.GetFiles("*." + ResourceDTO.Culture + ".resx");

                            foreach (var CFiles in BaseCompanyFiles) // Add CompanyResources
                            {
                                if (CFiles.Name.Contains(ResourceDTO.Culture))
                                {
                                    strSource = BaseCompanyResource + "\\" + CFiles;
                                    strDest = E_CompanyResource + "\\" + CFiles;
                                    if (System.IO.File.Exists(strDest))
                                    {
                                        System.IO.File.Delete(strDest);
                                    }
                                    System.IO.File.Copy(strSource, strDest);
                                }
                            }
                            foreach (var CFiles in BaseRoomFiles) // Add RoomResources
                            {
                                if (CFiles.Name.Contains(ResourceDTO.Culture))
                                {
                                    strSource = BaseRoomResource + "\\" + CFiles;
                                    strDest = E_RoomResource + "\\" + CFiles;
                                    if (System.IO.File.Exists(strDest))
                                    {
                                        System.IO.File.Delete(strDest);
                                    }
                                    System.IO.File.Copy(strSource, strDest);
                                }
                            }

                            foreach (var EFiles in BaseEnterPriseFiles) // Add EnterpriseResources
                            {
                                if (EFiles.Name.Contains(ResourceDTO.Culture))
                                {
                                    strSource = BaseEnterpriseResource + "\\" + EFiles;
                                    strDest = E_EnterpriseResource + "\\" + EFiles;
                                    if (System.IO.File.Exists(strDest))
                                    {
                                        System.IO.File.Delete(strDest);
                                    }
                                    System.IO.File.Copy(strSource, strDest);
                                }
                            }

                            //foreach (var EmailFiles in EmailtemplateFiles) // Add EmailTemplate 
                            //{
                            //    if (EmailFiles.Name.Contains("-en-US"))
                            //    {
                            //        strSource = EmailTemplateResource + "\\" + EmailFiles;
                            //        strDest = EmailTemplateResource + "\\" + EmailFiles.Name.Replace("-en-US", "-" + item);
                            //        if (System.IO.File.Exists(strDest) == false)
                            //        {
                            //            System.IO.File.Copy(strSource, strDest);
                            //        }

                            //    }
                            //}

                            ResourceLanguageDTO NewResourceDTO = new ResourceLanguageDTO();
                            NewResourceDTO.Language = ResourceDTO.Language;
                            NewResourceDTO.Culture = ResourceDTO.Culture;
                            Int64 NewLanguageID = objEnterprise.InsertResourceLanguage(NewResourceDTO);

                            string strCommand = string.Empty;
                            strCommand = @"INSERT INTO [MobileResources] ([ResourcePageID],[ResourceKey],[ResourceValue],[LanguageID],[CompanyID],[CreatedBy],[UpdatedBy],[CreatedOn],[UpdatedOn])
                                   select [ResourcePageID],[ResourceKey],[ResourceValue]," + NewLanguageID + ",[CompanyID],1,1,getutcdate(),getutcdate() from [eTurns].[dbo].[MobileResources] where CompanyID = 0 And LanguageID =" + ResourceDTO.ID;
                            objEnterprise.AddResource(strCommand);

                            // Remain to add Email Data in database.

                            string[] AllCompanyfolders = System.IO.Directory.GetDirectories(BasePathResource + EnterpriseID);
                            foreach (var Companyfolder in AllCompanyfolders)
                            {
                                if (!Companyfolder.Contains("CompanyResources") && !Companyfolder.Contains("EnterpriseResources") && !Companyfolder.Contains("RoomResources"))
                                {
                                    string CompanyName = Companyfolder.Substring(Companyfolder.LastIndexOf("\\"), Companyfolder.Length - Companyfolder.LastIndexOf("\\")).Replace("\\", "");
                                    foreach (var CFiles in BaseCompanyFiles) // Add CompanyResources at company level
                                    {
                                        if (CFiles.Name.Contains(ResourceDTO.Culture))
                                        {
                                            strSource = BaseCompanyResource + "\\" + CFiles;
                                            strDest = Companyfolder + "\\" + CFiles;
                                            if (System.IO.File.Exists(strDest))
                                            {
                                                System.IO.File.Delete(strDest);
                                            }
                                            System.IO.File.Copy(strSource, strDest);
                                        }
                                    }

                                    strCommand = @"INSERT INTO [MobileResources] ([ResourcePageID],[ResourceKey],[ResourceValue],[LanguageID],[CompanyID],[CreatedBy],[UpdatedBy],[CreatedOn],[UpdatedOn])
                                   select [ResourcePageID],[ResourceKey],[ResourceValue]," + NewLanguageID + "," + Convert.ToInt64(CompanyName) + ",1,1,getutcdate(),getutcdate() from [eTurns].[dbo].[MobileResources] where CompanyID = 0 And LanguageID =" + ResourceDTO.ID;
                                    objEnterprise.AddResource(strCommand);

                                }
                            }
                        }
                    }

                    #endregion

                    #region Remove Operation
                    foreach (var item in RemoveColumn) // Remove Operation
                    {
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(item)))
                        {
                            ResourceLanguageDTO RLDTO = lstEnterpriseResourceDTO.Where(x => x.Culture == item).FirstOrDefault();

                            FileInfo[] E_CompanyFiles = E_CompanyResource.GetFiles("*." + RLDTO.Culture + ".resx");
                            FileInfo[] E_EnterPriseFiles = E_EnterpriseResource.GetFiles("*." + RLDTO.Culture + ".resx");
                            FileInfo[] E_RoomFiles = E_RoomResource.GetFiles("*." + RLDTO.Culture + ".resx");


                            foreach (var CFiles in E_CompanyFiles) // Remove CompanyResources
                            {
                                if (CFiles.Name.Contains(RLDTO.Culture))
                                {
                                    if (System.IO.File.Exists(E_CompanyResource + "\\" + CFiles))
                                    {
                                        System.IO.File.Delete(E_CompanyResource + "\\" + CFiles);
                                    }
                                }
                            }

                            foreach (var EFiles in E_EnterPriseFiles) // Remove EnterpriseResources
                            {
                                if (EFiles.Name.Contains(RLDTO.Culture))
                                {
                                    if (System.IO.File.Exists(E_EnterpriseResource + "\\" + EFiles))
                                    {
                                        System.IO.File.Delete(E_EnterpriseResource + "\\" + EFiles);
                                    }
                                }
                            }
                            foreach (var EFiles in E_RoomFiles) // Remove EnterpriseResources
                            {
                                if (EFiles.Name.Contains(RLDTO.Culture))
                                {
                                    if (System.IO.File.Exists(E_RoomResource + "\\" + EFiles))
                                    {
                                        System.IO.File.Delete(E_RoomResource + "\\" + EFiles);
                                    }
                                }
                            }

                            ////foreach (var EmailFiles in EmailtemplateFiles) // Remove EmailtemplateFiles 
                            ////{
                            ////    if (EmailFiles.Name.Contains(item))
                            ////    {
                            ////        if (System.IO.File.Exists(EmailTemplateResource + "//" + EmailFiles))
                            ////        {
                            ////            System.IO.File.Delete(EmailTemplateResource + "//" + EmailFiles);
                            ////        }
                            ////    }
                            ////}



                            // Remain to delete Email Data from database.

                            string[] AllCompanyfolders = System.IO.Directory.GetDirectories(BasePathResource + EnterpriseID);
                            foreach (var Companyfolder in AllCompanyfolders)
                            {
                                string CompanyName = Companyfolder.Substring(Companyfolder.LastIndexOf("\\"), Companyfolder.Length - Companyfolder.LastIndexOf("\\")).Replace("\\", "");
                                if (!Companyfolder.Contains("CompanyResources") && !Companyfolder.Contains("EnterpriseResources") && !Companyfolder.Contains("RoomResources"))
                                {
                                    DirectoryInfo CompanyResource = new DirectoryInfo(Companyfolder);
                                    FileInfo[] CompanyFiles = CompanyResource.GetFiles("*." + RLDTO.Culture + ".resx");

                                    foreach (var CFiles in CompanyFiles) // delete CompanyResources at company level
                                    {
                                        if (CFiles.Name.Contains(RLDTO.Culture))
                                        {
                                            if (System.IO.File.Exists(CompanyResource + "\\" + CFiles))
                                            {
                                                System.IO.File.Delete(CompanyResource + "\\" + CFiles);
                                            }
                                        }
                                    }
                                    objEnterprise.DeleteResourceLanguageData(RLDTO, Convert.ToInt32(CompanyName));
                                }
                            }

                            objEnterprise.DeleteResourceLanguageData(RLDTO, 0);

                        }

                        #endregion

                        strResult = "ok";
                    }
                }
            }
            catch
            {
                strResult = "error";
            }

            return Json(strResult);
        }

        #endregion

        #region "Company Master"
        /// <summary>
        ///GET ALL: /Master/ 
        /// </summary>
        /// <returns></returns>
        public ActionResult CompanyList()
        {
            CompanyMasterDAL objCompanyMasterDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
            //Session["AllCompanies"] = objCompanyMasterDAL.GetAllCompanies(false, false, SessionHelper.CompanyList).ToList();
            Session["AllCompanies"] = objCompanyMasterDAL.GetAllCompaniesFromETurnsMaster(false, false, SessionHelper.CompanyList).ToList();
            return View();
        }

        [AjaxOrChildActionOnlyAttribute]
        public PartialViewResult _CreateCompany()
        {
            return PartialView();
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        [AjaxOrChildActionOnlyAttribute]
        public ActionResult CompanyCreate()
        {
            CompanyMasterDTO objDTO = new CompanyMasterDTO()
            {
                Created = DateTimeUtility.DateTimeNow,
                Updated = DateTimeUtility.DateTimeNow,
                CreatedBy = SessionHelper.UserID,
                CreatedByName = SessionHelper.UserName,
                LastUpdatedBy = SessionHelper.UserID,
                UpdatedByName = SessionHelper.UserName,
                IsActive = true,
            };
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("CompanyMaster");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            return PartialView("_CreateCompany", objDTO);

        }

        /// <summary>
        /// JSON Record Save - Enter key Save/Update
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Technician"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CompanySave(CompanyMasterDTO objDTO)
        {
            string message = "";
            string status = "";
            CompanyMasterDAL obj = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            if (string.IsNullOrEmpty(objDTO.Name))
            {
                message = string.Format(ResMessage.Required, ResCompany.CompanyName); // "Company name is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }

            if (objDTO.ID == 0)
            {
                string strOK = objCDAL.CompanyDuplicateCheck(objDTO.ID, objDTO.Name);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResCompany.CompanyName, objDTO.Name);  //"Company name \"" + objDTO.Name + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.GUID = Guid.NewGuid();

                    long ReturnVal = obj.Insert(objDTO);

                    // To Add EnterPrise Config Data to Company Config File (Added By Esha on 15/09/2014)   

                    EnterPriseConfigDTO objConfig = new EnterPriseConfigDTO();
                    EnterpriseMasterDAL objDAL = new EnterpriseMasterDAL();
                    objConfig = objDAL.GetEnterpriseConfig(SessionHelper.EnterPriceID);

                    if (objConfig != null)
                    {
                        CompanyConfigDTO objCompanyConfig = AssignValue(objConfig);
                        objCompanyConfig.CompanyID = ReturnVal;
                        CompanyConfigDAL objComDAL = new CompanyConfigDAL(SessionHelper.EnterPriseDBName);
                        objComDAL.Insert(objCompanyConfig);
                    }
                    else
                    {
                        CompanyConfigDTO objCompanyConfig = new CompanyConfigDTO();
                        objCompanyConfig.CompanyID = ReturnVal;
                        CompanyConfigDAL objComDAL = new CompanyConfigDAL(SessionHelper.EnterPriseDBName);
                        objComDAL.Insert(objCompanyConfig);
                    }

                    // End Code
                    // Add to UserRoomAccess for Super Admin Users

                    eTurnsMaster.DAL.UserMasterDAL oUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                    List<UserMasterDTO> oUserMasterDTO = oUserMasterDAL.GetAllRecords().Where(x => x.UserType == 1 && x.RoleID > 0).ToList();
                    List<UserAccessDTO> lstAccess = new List<UserAccessDTO>();
                    foreach (var item in oUserMasterDTO)
                    {
                        if (SessionHelper.UserID != item.ID)
                        {
                            UserAccessDTO oUserAccessDTO = new UserAccessDTO();
                            oUserAccessDTO.UserId = item.ID;
                            oUserAccessDTO.RoleId = item.RoleID;
                            oUserAccessDTO.EnterpriseId = SessionHelper.EnterPriceID;
                            oUserAccessDTO.CompanyId = ReturnVal;
                            oUserAccessDTO.RoomId = 0;
                            lstAccess.Add(oUserAccessDTO);
                        }
                    }
                    oUserMasterDAL.AddNewCompRoomToRoleUserRoomAccess(lstAccess);

                    // End Code
                    objDTO.ID = ReturnVal;
                    if (SessionHelper.RoleID == -1 || SessionHelper.RoleID == -2)
                    {
                        List<CompanyMasterDTO> lstCompanies = SessionHelper.CompanyList;
                        if (lstCompanies != null && lstCompanies.Count > 0)
                        {
                            objDTO.EnterPriseId = SessionHelper.EnterPriceID;
                            objDTO.EnterPriseName = SessionHelper.EnterPriceName;
                            lstCompanies.Add(objDTO);
                        }
                        else
                        {
                            lstCompanies = new List<CompanyMasterDTO>();
                            objDTO.EnterPriseId = SessionHelper.EnterPriceID;
                            objDTO.EnterPriseName = SessionHelper.EnterPriceName;
                            SetSessions(SessionHelper.EnterPriceID, objDTO.ID, 0, SessionHelper.UserID, SessionHelper.UserType, SessionHelper.RoleID, "company", SessionHelper.EnterPriceName, objDTO.Name, string.Empty);
                            lstCompanies.Add(objDTO);
                        }
                        SessionHelper.CompanyList = lstCompanies;
                    }
                    else if (SessionHelper.UserType < 3)
                    {
                        UserBAL objUserBAL = new UserBAL();
                        List<CompanyMasterDTO> lstCompanies = SessionHelper.CompanyList;
                        if (lstCompanies != null && lstCompanies.Count > 0)
                        {
                            objDTO.EnterPriseId = SessionHelper.EnterPriceID;
                            objDTO.EnterPriseName = SessionHelper.EnterPriceName;
                            lstCompanies.Add(objDTO);
                            objUserBAL.AddNewCompanyPermissions(SessionHelper.EnterPriceID, ReturnVal, SessionHelper.UserID, SessionHelper.RoleID, SessionHelper.UserType);
                        }
                        else
                        {
                            lstCompanies = new List<CompanyMasterDTO>();
                            objDTO.EnterPriseId = SessionHelper.EnterPriceID;
                            objDTO.EnterPriseName = SessionHelper.EnterPriceName;
                            objUserBAL.AddNewCompanyPermissions(SessionHelper.EnterPriceID, ReturnVal, SessionHelper.UserID, SessionHelper.RoleID, SessionHelper.UserType);
                            SetSessions(SessionHelper.EnterPriceID, objDTO.ID, 0, SessionHelper.UserID, SessionHelper.UserType, SessionHelper.RoleID, "company", SessionHelper.EnterPriceName, objDTO.Name, string.Empty);
                            lstCompanies.Add(objDTO);
                        }
                        SessionHelper.CompanyList = lstCompanies;
                    }

                    if (ReturnVal > 0)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";

                        try
                        {
                            //string id = hrmResult.ReasonPhrase;
                            string id = ReturnVal.ToString();

                            eTurns.DAL.LabelPrintingDAL.LabelFieldModuleTemplateDetailDAL objLabelTemplateDetailDAL = new eTurns.DAL.LabelPrintingDAL.LabelFieldModuleTemplateDetailDAL(SessionHelper.EnterPriseDBName);
                            objLabelTemplateDetailDAL.CopyAllBaseTemplate(objDTO.ID, SessionHelper.UserID, SessionHelper.RoomID);

                            //string appPath1 = typeof(MasterController).Assembly.CodeBase;
                            //appPath1 = appPath1.Substring(0, appPath1.IndexOf("/bin")).Replace(@"file:///", "") + @"\Resources\";

                            //string resourceDirectory = appPath1 + SessionHelper.EnterPriceID + "_" + id;
                            string appPath1 = ResourceHelper.ResourceDirectoryBasePath + @"\";
                            string resourceDirectory = appPath1 + SessionHelper.EnterPriceID + "\\" + id;
                            if (!System.IO.Directory.Exists(resourceDirectory))
                            {
                                System.IO.Directory.CreateDirectory(resourceDirectory);
                            }
                            string[] files = System.IO.Directory.GetFiles(resourceDirectory);
                            if (files == null || files.Length <= 0)
                            {
                                foreach (var file in System.IO.Directory.GetFiles(appPath1 + SessionHelper.EnterPriceID + "\\CompanyResources"))
                                {
                                    System.IO.File.Copy(file, System.IO.Path.Combine(resourceDirectory, System.IO.Path.GetFileName(file)));
                                }

                            }
                            foreach (var file in System.IO.Directory.GetFiles(appPath1 + "MasterResources\\EnterpriseResources"))
                            {
                                System.IO.File.Copy(file, System.IO.Path.Combine(resourceDirectory, System.IO.Path.GetFileName(file)), true);
                            }
                            // TODO: Uncomment below two line For Enable Copy Default Label template functinality


                            MobileResourcesDAL mobResDAL = new MobileResourcesDAL(SessionHelper.EnterPriseDBName);
                            mobResDAL.InsertMobileResourceForCompany(SessionHelper.UserID, objDTO.ID);

                            //wi-1309 company save takes much time so during enterprise save time only 
                            //ReportBuilderController objRDLC = new ReportBuilderController();
                            //EnterpriseDTO objEntDTO = new EnterpriseDTO();
                            //EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
                            //objEntDTO = objEnterpriseMasterDAL.GetEnterprise(SessionHelper.EnterPriceID);
                            //if (objEntDTO != null)
                            //    objRDLC.UpdateChildEntReportForSingle(true, objEntDTO);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                        }

                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }
            else
            {
                objDTO.LastUpdatedBy = SessionHelper.UserID;

                string EnterpriseDBName = string.Empty;
                EnterpriseDTO oEnterpriseDTO = new EnterpriseDAL(DbConnectionHelper.GetETurnsMasterDBName()).GetEnterprise(objDTO.EnterPriseId);
                if (oEnterpriseDTO != null)
                    EnterpriseDBName = oEnterpriseDTO.EnterpriseDBName;

                CommonDAL oCommonDAL = new CommonDAL(EnterpriseDBName);
                string strOK = oCommonDAL.DuplicateCheck(objDTO.Name, "edit", objDTO.ID, "CompanyMaster", "Name");
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResCompany.CompanyName, objDTO.Name);  //"Company name \"" + objDTO.Name + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {


                    CompanyMasterDAL oCompanyMaster = new CompanyMasterDAL(EnterpriseDBName);
                    CompanyMasterDTO RetDTO = new CompanyMasterDTO();
                    RetDTO = oCompanyMaster.Edit(objDTO);
                    if (SessionHelper.CompanyList.Any(t => t.ID == RetDTO.ID && t.EnterPriseId == RetDTO.EnterPriseId))
                    {
                        List<CompanyMasterDTO> lstCurrentList = SessionHelper.CompanyList;
                        if (oEnterpriseDTO != null)
                        {
                            RetDTO.EnterPriseId = oEnterpriseDTO.ID;
                            RetDTO.EnterPriseName = oEnterpriseDTO.Name;
                        }
                        else
                        {
                            RetDTO.EnterPriseId = SessionHelper.EnterPriceID;
                            RetDTO.EnterPriseName = SessionHelper.EnterPriceName;
                        }
                        lstCurrentList = lstCurrentList.Where(t => t.ID != RetDTO.ID).ToList();
                        lstCurrentList.Add(RetDTO);
                        SessionHelper.CompanyList = lstCurrentList.OrderBy(c => c.Name).ToList();
                    }
                    if (RetDTO.IsStatusChanged)
                    {
                        if (objDTO.IsActive)
                        {

                        }

                    }
                    message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                    status = "ok";
                }
            }

            return Json(new { Message = message, Status = status, retDto = objDTO }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateIncludeCommonBOM(long CompanyId, bool Status)
        {
            string message = "";
            string status = "";
            CompanyMasterDAL obj = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
            if (obj.UpdateIncludeCommonBOM(CompanyId, Status))
            {
                message = ResMessage.SaveMessage;
                status = "ok";
            }
            else
            {
                message = ResMessage.SaveErrorMsg;
                status = "Fail";
            }

            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateCommonBOM(long CompanyId, bool Status)
        {
            string message = "";
            string status = "";
            CompanyMasterDAL obj = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
            status = obj.UpdateCommonBOM(CompanyId, Status, SessionHelper.UserID);
            if (status == "ok")
            {
                message = ResMessage.SaveMessage;
                status = "ok";
            }
            else
            {
                message = ResMessage.SaveErrorMsg;
                //status = status;
            }

            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }
        //[HttpPost]
        //public async Task<JsonResult> CompanySaveAsync(CompanyMasterDTO objDTO)
        //{
        //    string message = "";
        //    string status = "";
        //    CompanyMasterDAL obj = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
        //    CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
        //    if (string.IsNullOrEmpty(objDTO.Name))
        //    {
        //        message = string.Format(ResMessage.Required, ResCompany.CompanyName); // "Company name is required.";
        //        status = "fail";
        //        return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        //    }

        //    if (objDTO.ID == 0)
        //    {

        //        string strOK = objCDAL.CompanyDuplicateCheck(objDTO.ID, objDTO.Name);
        //        if (strOK == "duplicate")
        //        {
        //            message = string.Format(ResMessage.DuplicateMessage, ResCompany.CompanyName, objDTO.Name);  //"Company name \"" + objDTO.Name + "\" already exist! Try with Another!";
        //            status = "duplicate";
        //        }
        //        else
        //        {
        //            objDTO.CreatedBy = SessionHelper.UserID;
        //            objDTO.LastUpdatedBy = SessionHelper.UserID;
        //            objDTO.GUID = Guid.NewGuid();
        //            long ReturnVal = obj.Insert(objDTO);

        //            //CopyReportForComkpany(ReturnVal);
        //            objDTO.ID = ReturnVal;
        //            if (SessionHelper.RoleID == -1 || SessionHelper.RoleID == -2)
        //            {
        //                List<CompanyMasterDTO> lstCompanies = SessionHelper.CompanyList;
        //                if (lstCompanies != null && lstCompanies.Count > 0)
        //                {
        //                    objDTO.EnterPriseId = SessionHelper.EnterPriceID;
        //                    objDTO.EnterPriseName = SessionHelper.EnterPriceName;
        //                    lstCompanies.Add(objDTO);
        //                }
        //                else
        //                {
        //                    lstCompanies = new List<CompanyMasterDTO>();
        //                    objDTO.EnterPriseId = SessionHelper.EnterPriceID;
        //                    objDTO.EnterPriseName = SessionHelper.EnterPriceName;
        //                    SetSessions(SessionHelper.EnterPriceID, objDTO.ID, 0, SessionHelper.UserID, SessionHelper.UserType, SessionHelper.RoleID, "company", SessionHelper.EnterPriceName, objDTO.Name, string.Empty);
        //                    lstCompanies.Add(objDTO);
        //                }
        //                SessionHelper.CompanyList = lstCompanies;
        //            }

        //            if (ReturnVal > 0)
        //            {
        //                message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
        //                status = "ok";
        //                //Task<long> getidTask1 = CopyReportForComkpany(ReturnVal);
        //                Task<long> getidTask2 = CopyResourcesandMObileResources(ReturnVal);
        //                //await getidTask1;
        //                int i = 2;
        //                i = 4;

        //                await getidTask2;

        //            }
        //            else
        //            {
        //                message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
        //                status = "fail";
        //            }


        //        }
        //    }
        //    else
        //    {
        //        objDTO.LastUpdatedBy = SessionHelper.UserID;
        //        string strOK = objCDAL.DuplicateCheck(objDTO.Name, "edit", objDTO.ID, "CompanyMaster", "Name");
        //        if (strOK == "duplicate")
        //        {
        //            message = string.Format(ResMessage.DuplicateMessage, ResCompany.CompanyName, objDTO.Name);  //"Company name \"" + objDTO.Name + "\" already exist! Try with Another!";
        //            status = "duplicate";
        //        }
        //        else
        //        {

        //            bool ReturnVal = obj.Edit(objDTO);
        //            if (ReturnVal)
        //            {
        //                message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
        //                status = "ok";
        //            }
        //            else
        //            {
        //                message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
        //                status = "fail";
        //            }
        //        }
        //    }
        //    //await getidTask1;
        //    //await getidTask2;
        //    //await CopyResourcesandMObileResources;
        //    return Json(new { Message = message, Status = status, retDto = objDTO }, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>
        [AjaxOrChildActionOnlyAttribute]
        public ActionResult CompanyEdit(string ID)
        {
            long CompanyID = 0;
            long EnterpriseId = 0;
            string[] arystrIds = ID.Split("_".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (arystrIds.Length > 0)
                CompanyID = Convert.ToInt64(arystrIds[0]);
            if (arystrIds.Length > 1)
                EnterpriseId = Convert.ToInt64(arystrIds[1]);

            string EnterpriseDBName = new EnterpriseDAL(SessionHelper.EnterPriseDBName).GetEnterpriseDBName(EnterpriseId);

            //CompanyMasterController obj = new CompanyMasterController();
            CompanyMasterDAL obj = new CompanyMasterDAL(EnterpriseDBName);
            CompanyMasterDTO objDTO = obj.GetRecord(CompanyID);
            if (objDTO != null)
            {
                objDTO.CreatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                objDTO.UpdatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                objDTO.EnterPriseId = EnterpriseId;
            }
            ViewBag.UDFs = objUDFDAL.GetCompanyMasterUDFDataPageWise(EnterpriseDBName, objDTO.ID, "CompanyMaster");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;
            ModelState.Remove("ID");

            return PartialView("_CreateCompany", objDTO);
        }

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public JsonResult DeleteCompanyMasterRecords(string ids)
        {
            string response = string.Empty;
            try
            {
                Dictionary<long, string> dictionary = new Dictionary<long, string>();
                string[] arystrIds = ids.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in arystrIds)
                {
                    long CompanyID = 0;
                    long EnterpriseId = 0;
                    string[] aryStr = item.Split("_".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (aryStr.Length > 0)
                        CompanyID = Convert.ToInt64(aryStr[0]);
                    if (aryStr.Length > 1)
                        EnterpriseId = Convert.ToInt64(aryStr[1]);

                    if (dictionary.ContainsKey(EnterpriseId))
                    {
                        string value = dictionary[EnterpriseId];
                        value += "," + CompanyID.ToString();
                        dictionary[EnterpriseId] = value;
                    }
                    else
                    {
                        dictionary.Add(EnterpriseId, CompanyID.ToString());
                    }
                }

                string EntSuccessIds = string.Empty;
                List<DeleteStatusDTO> listDeleteStatusDTO = new List<DeleteStatusDTO>();
                foreach (var pair in dictionary)
                {
                    long EnterpriseId = pair.Key;
                    string CompanyIds = pair.Value;

                    string EnterpriseDBName = new EnterpriseDAL(SessionHelper.EnterPriseDBName).GetEnterpriseDBName(EnterpriseId);

                    CommonDAL objCommonDAL = new CommonDAL(EnterpriseDBName);
                    List<DeleteStatusDTO> olistDeleteStatusDTO = objCommonDAL.DeleteModulewiseRetList(CompanyIds, "Company", false, SessionHelper.UserID);
                    listDeleteStatusDTO.AddRange(olistDeleteStatusDTO);

                    if (EnterpriseId == SessionHelper.EnterPriceID)
                    {
                        EntSuccessIds += string.Join(",", olistDeleteStatusDTO.Where(t => t.Status == "Success").Select(objConfig => objConfig.Id).ToArray());
                    }
                }

                int Failcnt = 0;
                int Successcnt = 0;

                Failcnt = listDeleteStatusDTO.Where(t => t.Status == "Fail").Count();
                Successcnt = listDeleteStatusDTO.Where(t => t.Status == "Success").Count();
                //string[] SuccessIds = listDeleteStatusDTO.Where(t => t.Status == "Success").Select(objConfig => objConfig.Id).ToArray();
                if (Successcnt > 0)
                {
                    response = Successcnt + " record(s) deleted successfully.";
                }
                if (Failcnt > 0)
                {
                    if (string.IsNullOrEmpty(response))
                    {
                        response = Failcnt + " record(s) used in another module.";
                    }
                    else
                    {
                        response = response + " " + Failcnt + " record(s) used in another module.";
                    }
                }

                //string scsids = string.Join(",", SuccessIds);
                if (!string.IsNullOrWhiteSpace(EntSuccessIds))
                {
                    string[] arrstr = EntSuccessIds.Split(',');
                    long[] arrids = arrstr.ToIntArray().Where(t => t > 0).ToArray();
                    List<CompanyMasterDTO> lstCompanies = SessionHelper.CompanyList;
                    lstCompanies = (from cm in lstCompanies
                                    where !arrids.Contains(cm.ID)
                                    select cm).ToList();
                    SessionHelper.CompanyList = lstCompanies.OrderBy(t => t.Name).ToList();
                    if (arrids.Contains(SessionHelper.CompanyID))
                    {
                        if (SessionHelper.CompanyList.Count > 0)
                        {
                            CompanyMasterDTO objCompany = SessionHelper.CompanyList.OrderBy(t => t.Name).First();
                            SetSessions(objCompany.EnterPriseId, objCompany.ID, 0, SessionHelper.UserID, SessionHelper.UserType, SessionHelper.RoleID, "company", SessionHelper.EnterPriceName, objCompany.Name, string.Empty);
                        }
                        else
                        {
                            SetSessions(SessionHelper.EnterPriceID, 0, 0, SessionHelper.UserID, SessionHelper.UserType, SessionHelper.RoleID, "company", SessionHelper.EnterPriceName, string.Empty, string.Empty);
                        }

                    }
                }

                eTurns.DAL.CacheHelper<IEnumerable<CompanyMasterDTO>>.InvalidateCache();
                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Update Records
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <param name="rowId"></param>
        /// <param name="columnPosition"></param>
        /// <param name="columnId"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string UpdateCompanyData(Int32 id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            //CompanyMasterController obj = new CompanyMasterController();
            CompanyMasterDAL obj = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
            obj.UpdateData(id, value, rowId, columnPosition, columnId, columnName);
            return value;
        }

        //private long CopyReportForComkpany(long ReturnVal)
        //{
        //    eTurnsWeb.localhost2012.ReportingService2010 rs = new eTurnsWeb.localhost2012.ReportingService2010();
        //    ReportServices objReportServices;
        //    string Username = ConfigurationManager.AppSettings["NetworkUser"];
        //    string Password = ConfigurationManager.AppSettings["NetworkPassword"];
        //    string Domain = ConfigurationManager.AppSettings["NetworkDomain"];
        //    string DatabaseUsername = ConfigurationManager.AppSettings["DbUserName"];
        //    string DatabasePassword = ConfigurationManager.AppSettings["DbPassword"];
        //    string ReportServerUrl = ConfigurationManager.AppSettings["ReportServerURL"];
        //    string ReportsUrl = ConfigurationManager.AppSettings["ReportsUrl"];
        //    objReportServices = new ReportServices(rs, Domain, Username, Password, DatabaseUsername, DatabasePassword);
        //    objReportServices.CopyAllFilesAndFolder("/Master", ReturnVal.ToString());
        //    objReportServices.CopyAllFilesAndFolder("/Transaction", "/" + ReturnVal.ToString());
        //    return ReturnVal;
        //}

        private long CopyResourcesandMObileResources(long ReturnVal)
        {
            int milliseconds = 7000;
            Thread.Sleep(milliseconds);

            try
            {
                string id = ReturnVal.ToString();
                string appPath1 = ResourceHelper.ResourceDirectoryBasePath + @"\";
                string resourceDirectory = appPath1 + SessionHelper.EnterPriceID + "_" + id;
                if (!System.IO.Directory.Exists(resourceDirectory))
                {
                    System.IO.Directory.CreateDirectory(resourceDirectory);
                }
                string[] files = System.IO.Directory.GetFiles(resourceDirectory);
                if (files == null || files.Length <= 0)
                {
                    foreach (var file in System.IO.Directory.GetFiles(appPath1 + "MasterResources"))
                    {
                        System.IO.File.Copy(file, System.IO.Path.Combine(resourceDirectory, System.IO.Path.GetFileName(file)));
                    }
                }

                MobileResourcesDAL mobResDAL = new MobileResourcesDAL(SessionHelper.EnterPriseDBName);
                mobResDAL.InsertMobileResourceForCompany(SessionHelper.UserID, SessionHelper.CompanyID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
            return ReturnVal;
        }

        #region Data Provider

        /// <summary>
        /// Below method used to Locationd the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult GetCompanyList(JQueryDataTableParamModel param)
        {
            CompanyMasterDTO entity = new CompanyMasterDTO();
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
            if (sortColumnName == "0" || sortColumnName == "undefined" || string.IsNullOrEmpty(sortColumnName))
                sortColumnName = "ID Desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;

            //CompanyMasterController controller = new CompanyMasterController();
            string UserCompanies = null;
            if (SessionHelper.RoleID != -1)
                UserCompanies = string.Join(",", SessionHelper.CompanyList.Select(t => t.ID).ToArray());

            CompanyMasterDAL controller = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
            if (string.IsNullOrWhiteSpace(UserCompanies) && SessionHelper.RoleID != -1)
            {
                UserCompanies = "0,0";
            }
            List<CompanyMasterDTO> DataFromDB = controller.GetPagedCompanies(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, IsArchived, IsDeleted, UserCompanies);
            //if (DataFromDB != null)
            //{
            //    DataFromDB.ToList().ForEach(t =>
            //    {
            //        t.CreatedDate = CommonUtility.ConvertDateByTimeZone(t.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
            //        t.UpdatedDate = CommonUtility.ConvertDateByTimeZone(t.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
            //    });
            //}
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

        #endregion

        #region "Room Master"

        public ActionResult RoomList()
        {
            RoomDAL ObjRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);

            List<RoomDTO> lstRooms = ObjRoomDAL.GetAllRoomsFromETurnsMaster(SessionHelper.CompanyID, false, false, SessionHelper.RoomList, string.Empty);
            Session["AllRooms"] = lstRooms;
            return View();
        }

        [AjaxOrChildActionOnlyAttribute]
        public PartialViewResult _CreateRoom()
        {
            return PartialView();
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        [AjaxOrChildActionOnlyAttribute]
        public ActionResult RoomCreate()
        {
            RoomDTO objDTO = new RoomDTO();
            objDTO.ID = 0;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.CreatedByName = SessionHelper.UserName;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.GUID = Guid.NewGuid();

            var list1 = new SelectList(new[]{
                          new {ID="0",Name=""},new {ID="1",Name="1"},new{ID="2",Name="2"},
                          new{ID="3",Name="3"},new {ID="4",Name="4"},new{ID="5",Name="5"},new{ID="6",Name="6"},
                          new {ID="7",Name="7"},new{ID="8",Name="8"},new{ID="9",Name="9"},new{ID="10",Name="10"},},
                           "ID", "Name", 1);
            ViewBag.Days = list1;

            var list2 = new SelectList(new[]{
                        new {ID="0",Name=""},new {ID="5",Name="5"},new{ID="10",Name="10"},new {ID="15",Name="15"},new{ID="20",Name="20"},new {ID="25",Name="25"},new{ID="30",Name="30"},new {ID="35",Name="35"},new{ID="40",Name="40"},new {ID="45",Name="45"},new{ID="50",Name="50"},
                        new {ID="55",Name="55"},new{ID="60",Name="60"},new {ID="65",Name="65"},new{ID="70",Name="70"},new {ID="75",Name="75"},new{ID="80",Name="80"},new {ID="85",Name="85"},new{ID="90",Name="90"},new {ID="95",Name="95"},new{ID="100",Name="100"},
                        new {ID="105",Name="105"},new{ID="110",Name="110"},new {ID="115",Name="115"},new{ID="120",Name="120"},new {ID="125",Name="125"},new{ID="130",Name="130"},new {ID="135",Name="135"},new{ID="140",Name="140"},new {ID="145",Name="145"},new{ID="150",Name="150"},
                        new {ID="155",Name="155"},new{ID="160",Name="160"},new {ID="165",Name="165"},new{ID="170",Name="170"},new {ID="175",Name="175"},new{ID="180",Name="180"},},
                          "ID", "Name", 1);
            ViewBag.AvgDays = list2;

            //var listMethodOfValuing = new SelectList(new[]{
            //              new {ID="0",Name=""},new {ID="1",Name="FIFO"},new{ID="2",Name="LIFO"},
            //              new{ID="3",Name="Average cost"},new{ID="4",Name="Last cost"}}, "ID", "Name", 1);
            var listMethodOfValuing = new SelectList(new[]{
                          new{ID="3",Name="Average cost"},new{ID="4",Name="Last cost"}}, "ID", "Name", 1);

            ViewBag.MethodOfValuing = listMethodOfValuing;

            objDTO.MethodOfValuingInventory = "4";

            var listWeeks = new SelectList(new[]{
                          new {ID="1",Name="First"},new{ID="2",Name="Second"},
                          new{ID="3",Name="Third"},new{ID="4",Name="Fourth"},new{ID="5",Name="Fifth"},},
                    "ID", "Name", 1);
            ViewBag.Weeks = listWeeks;

            var listdayName = new SelectList(new[]{
                          new {ID="Monday",Name="Monday"},new{ID="Tuesday",Name="Tuesday"},
                          new{ID="Wednesday",Name="Wednesday"},new{ID="Thursday",Name="Thursday"},new{ID="Friday",Name="Friday"},
            new{ID="Saturday",Name="Saturday"},new{ID="Sunday",Name="Sunday"},
            },
                  "ID", "Name", 1);
            ViewBag.DayNames = listdayName;

            ViewBag.InventoryConsuptionMethodBAG = InventoryConsuptionOptions();



            ViewBag.DefaultLocationList = null;
            ViewBag.DefaultSupplierList = null;

            //RoomController objRoom = new RoomController();
            RoomDAL objRoom = new RoomDAL(SessionHelper.EnterPriseDBName);
            List<RoomDTO> objList = objRoom.GetAllRecords(SessionHelper.CompanyID, false, false).ToList();
            objList.Insert(0, null);
            ViewBag.ReplineshmentRoomList = objList;
            ViewBag.ReplenishmentTypeList = GetReplanishmentTypeOptions();
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("Room");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            objDTO.IsRoomActive = true;
            objDTO.SuggestedOrder = true;
            objDTO.IsAllowRequisitionDuplicate = true;
            return PartialView("_CreateRoom", objDTO);
        }

        private List<CommonDTO> InventoryConsuptionOptions()
        {
            List<CommonDTO> ItemType = new List<CommonDTO>();
            ItemType.Add(new CommonDTO() { Text = "Lifo" });
            ItemType.Add(new CommonDTO() { Text = "Fifo" });
            return ItemType;
        }

        private List<CommonDTO> GetReplanishmentTypeOptions()
        {
            List<CommonDTO> ItemType = new List<CommonDTO>();
            ItemType.Add(new CommonDTO() { ID = 1, Text = "Item replenish" });
            ItemType.Add(new CommonDTO() { ID = 2, Text = "Location replenish" });
            ItemType.Add(new CommonDTO() { ID = 3, Text = "Both" });

            return ItemType;
        }

        [HttpPost]
        public JsonResult UserSettingSave(UserSettingDTO objDTO)
        {
            string message = "";
            string status = "";
            Int64 ReturnVal = 0;
            if (objDTO != null)
            {
                eTurns.DAL.UserSettingDAL objUserSettingDAL = new eTurns.DAL.UserSettingDAL(SessionHelper.EnterPriseDBName);
                objDTO.UserId = SessionHelper.UserID;
                objDTO.IsRemember = objDTO.IsRemember;
                objDTO.RedirectURL = objDTO.RedirectURL;

                if (objDTO.Id != 0)
                {
                    ReturnVal = objUserSettingDAL.Update(objDTO);
                }
                else
                {
                    ReturnVal = objUserSettingDAL.Insert(objDTO);
                }
                if (ReturnVal > 0)
                {
                    message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                    status = "ok";
                }
            }
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RoomSave(RoomDTO objDTO, SchedulerDTO SupplierScheduleDTO)
        {
            string message = "";
            string status = "";
            bool NeedRefress = false;
            //RoomController obj = new RoomController();
            RoomDAL obj = new RoomDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

            if (string.IsNullOrEmpty(objDTO.RoomName))
            {
                message = string.Format(ResMessage.Required, ResRoomMaster.RoomName);// "Room name is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            if (objDTO.AllowInsertingItemOnScan == true && string.IsNullOrWhiteSpace(objDTO.DefaultBinName))
            {

                message = string.Format(ResMessage.Required, ResRoomMaster.DefaultBinID);// "Bin is required. if allow scan true";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            if (objDTO.AllowInsertingItemOnScan == true && string.IsNullOrWhiteSpace(objDTO.DefaultSupplierName))
            {

                message = string.Format(ResMessage.Required, "DefaultSupplierID");// "Supplier is required. if allow scan true";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            if (!objDTO.IsConsignment)
            {
                if (obj.HasConsignedItems(objDTO.ID))
                {
                    message = ResRoomMaster.errConsignedItemsAvailable;
                    status = "fail";
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                }
            }
            if (!objDTO.IsAllowRequisitionDuplicate)
            {
                if (obj.CheckDuplicateREquisition(objDTO.ID))
                {
                    message = ResRoomMaster.errDuplicateRequisitionAvailable;
                    status = "fail";
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                }
            }

            if (objDTO.ID == 0)
            {
                objDTO.EnterpriseId = SessionHelper.EnterPriceID;
                objDTO.CreatedBy = SessionHelper.UserID;
                objDTO.CompanyID = SessionHelper.CompanyID;
                objDTO.LastUpdatedBy = SessionHelper.UserID;
                objDTO.UpdatedByName = SessionHelper.UserName;
                string strOK = obj.RoomDuplicateCheck(objDTO.ID, objDTO.RoomName, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResRoomMaster.RoomName, objDTO.RoomName); // "Room \"" + objDTO.RoomName + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    //string strOK1 = obj.RoomEmailDuplicateCheck(objDTO.ID, objDTO.Email, SessionHelper.CompanyID);
                    string strOK1 = "ok";
                    if (strOK1 == "duplicate")
                    {
                        message = "Email \"" + objDTO.Email + "\" already exist! Try with Another!";
                        status = "duplicate";
                    }
                    else
                    {
                        objDTO.GUID = Guid.NewGuid();
                        long ReturnVal = obj.Insert(objDTO);

                        // To Add EnterPrise Config Data to Company Config File (Added By Esha on 15/09/2014)   

                        DashboardParameterDTO objConfig = new DashboardParameterDTO();
                        EnterpriseMasterDAL objDAL = new EnterpriseMasterDAL();
                        objConfig = objDAL.GetDashboardParameterById(0, 0);

                        if (objConfig != null)
                        {
                            DashboardParameterDTO objDashboard = AssignValueDashboard(objConfig);
                            objDashboard.ID = 0;
                            objDashboard.EnterpriseId = SessionHelper.EnterPriceID;
                            objDashboard.CompanyId = SessionHelper.CompanyID;
                            objDashboard.RoomId = ReturnVal;
                            objDAL.InsertDashboardParameter(objDashboard);
                            DashboardDAL objDashboardDAL = new DashboardDAL(SessionHelper.EnterPriseDBName);
                            objDashboard.ID = 0;
                            objDashboardDAL.SaveDashboardParameters(objDashboard);
                        }
                        else
                        {
                            DashboardParameterDTO objDashboard = new DashboardParameterDTO();
                            objDashboard.ID = 0;
                            objDashboard.EnterpriseId = SessionHelper.EnterPriceID;
                            objDashboard.CompanyId = SessionHelper.CompanyID;
                            objDashboard.RoomId = ReturnVal;
                            objDashboard.CreatedOn = DateTime.UtcNow;
                            objDashboard.UpdatedOn = DateTime.UtcNow;
                            objDAL.InsertDashboardParameter(objDashboard);
                            DashboardDAL objDashboardDAL = new DashboardDAL(SessionHelper.EnterPriseDBName);
                            objDashboard.ID = 0;
                            objDashboardDAL.SaveDashboardParameters(objDashboard);
                        }

                        // End Code
                        // Add to UserRoomAccess for Super Admin Users

                        //----------------------ADD PERMISSION FOR NEW ROOM TO ALL USERS----------------------
                        //
                        string MasterDBName = DbConnectionHelper.GetETurnsMasterDBName();
                        (new UserBAL()).AddNewRoomPermissionsNew(SessionHelper.EnterPriceID, SessionHelper.CompanyID, ReturnVal, SessionHelper.UserID, MasterDBName);

                        //eTurnsMaster.DAL.UserMasterDAL oUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                        //List<UserMasterDTO> oUserMasterDTO = oUserMasterDAL.GetAllRecords().Where(x => x.UserType == 1 && x.RoleID > 0).ToList();
                        //foreach (var item in oUserMasterDTO)
                        //{
                        //    if (SessionHelper.UserID != item.ID)
                        //        new UserBAL().AddNewRoomPermissions(SessionHelper.EnterPriceID, SessionHelper.CompanyID, ReturnVal, item.ID, item.RoleID, item.UserType);
                        //}

                        // End Code

                        SupplierScheduleDTO.RoomId = ReturnVal;
                        SupplierScheduleDTO.SupplierId = 0;
                        SupplierScheduleDTO.CreatedBy = SessionHelper.UserID;
                        if (SupplierScheduleDTO.IsScheduleChanged == 1)
                        {
                            SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
                            objSupplierMasterDAL.SaveSupplierSchedule(SupplierScheduleDTO);
                        }

                        if (SessionHelper.RoleID == -2 || SessionHelper.RoleID == -1)
                        {
                            //----------------------------FILL ROOM LIST SESSION-------------------------------------
                            //
                            List<RoomDTO> lst = SessionHelper.RoomList;
                            if (lst != null && lst.Count > 0)
                            {
                                lst.Add(new RoomDTO() { EnterpriseId = SessionHelper.EnterPriceID, CompanyID = SessionHelper.CompanyID, ID = ReturnVal, RoomName = objDTO.RoomName, IsRoomActive = objDTO.IsRoomActive });
                            }
                            else
                            {
                                lst = new List<RoomDTO>();
                                lst.Add(new RoomDTO() { EnterpriseId = SessionHelper.EnterPriceID, CompanyID = SessionHelper.CompanyID, ID = ReturnVal, RoomName = objDTO.RoomName, IsRoomActive = objDTO.IsRoomActive });
                                SessionHelper.RoomList = lst;
                                SetSessions(SessionHelper.EnterPriceID, SessionHelper.CompanyID, ReturnVal, SessionHelper.UserID, SessionHelper.UserType, SessionHelper.RoleID, "room", SessionHelper.EnterPriceName, SessionHelper.CompanyName, objDTO.RoomName);
                            }
                            SessionHelper.RoomList = lst;
                        }
                        else if (SessionHelper.UserType < 4)
                        {
                            //----------------------------FILL ROOM LIST SESSION-------------------------------------
                            //
                            List<RoomDTO> lst = SessionHelper.RoomList;
                            if (lst != null && lst.Count > 0)
                            {
                                lst.Add(new RoomDTO() { EnterpriseId = SessionHelper.EnterPriceID, CompanyID = SessionHelper.CompanyID, ID = ReturnVal, RoomName = objDTO.RoomName, IsRoomActive = objDTO.IsRoomActive });
                                if (lst.Count(t => t.CompanyID == SessionHelper.CompanyID) == 1)
                                {
                                    SessionHelper.RoomID = ReturnVal;
                                    SessionHelper.RoomName = objDTO.RoomName;
                                }
                            }
                            else
                            {
                                lst = new List<RoomDTO>();
                                lst.Add(new RoomDTO() { EnterpriseId = SessionHelper.EnterPriceID, CompanyID = SessionHelper.CompanyID, ID = ReturnVal, RoomName = objDTO.RoomName, IsRoomActive = objDTO.IsRoomActive });
                                SessionHelper.RoomList = lst;
                                SetSessions(SessionHelper.EnterPriceID, SessionHelper.CompanyID, ReturnVal, SessionHelper.UserID, SessionHelper.UserType, SessionHelper.RoleID, "room", SessionHelper.EnterPriceName, SessionHelper.CompanyName, objDTO.RoomName);
                            }
                            SessionHelper.RoomList = lst;

                            //------------------------FILL ROOM PERMISSIONS SESSION---------------------------------
                            //
                            if (SessionHelper.UserType >= 0 && SessionHelper.UserType < 4)
                            {
                                if (SessionHelper.UserType == 3)
                                {
                                    eTurns.DAL.UserMasterDAL objUserMasterDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                                    List<UserRoleModuleDetailsDTO> lstUserRoleModuleDetailsDTO = objUserMasterDAL.GetUserRoleModuleDetailsRecord(SessionHelper.UserID, SessionHelper.RoleID, SessionHelper.UserType);
                                    string strRoomList = string.Empty;
                                    SessionHelper.RoomPermissions = objUserMasterDAL.ConvertUserPermissions(lstUserRoleModuleDetailsDTO, SessionHelper.RoleID, ref strRoomList);
                                }
                                else
                                {
                                    eTurnsMaster.DAL.UserMasterDAL objUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                                    List<UserRoleModuleDetailsDTO> lstUserRoleModuleDetailsDTO = objUserMasterDAL.GetUserRoleModuleDetailsRecord(SessionHelper.UserID, SessionHelper.RoleID, SessionHelper.UserType);
                                    string strRoomList = string.Empty;
                                    SessionHelper.RoomPermissions = objUserMasterDAL.ConvertUserPermissions(lstUserRoleModuleDetailsDTO, SessionHelper.RoleID, ref strRoomList);
                                }

                                if (SessionHelper.RoomID == 0)
                                {
                                    SessionHelper.RoomID = ReturnVal;
                                }
                            }
                        }

                        //if (SessionHelper.RoleID == -2 || SessionHelper.RoleID == -1)
                        //{
                        //    List<RoomDTO> lst = SessionHelper.RoomList;
                        //    if (lst != null && lst.Count > 0)
                        //    {
                        //        lst.Add(new RoomDTO() { EnterpriseId = SessionHelper.EnterPriceID, CompanyID = SessionHelper.CompanyID, ID = ReturnVal, RoomName = objDTO.RoomName, IsRoomActive = objDTO.IsRoomActive });
                        //        //lst.Add(new KeyValuePair<long, string>(ReturnVal, objDTO.RoomName));
                        //    }
                        //    else
                        //    {
                        //        lst = new List<RoomDTO>();
                        //        lst.Add(new RoomDTO() { EnterpriseId = SessionHelper.EnterPriceID, CompanyID = SessionHelper.CompanyID, ID = ReturnVal, RoomName = objDTO.RoomName, IsRoomActive = objDTO.IsRoomActive });
                        //        SessionHelper.RoomList = lst;
                        //        SetSessions(SessionHelper.EnterPriceID, SessionHelper.CompanyID, ReturnVal, SessionHelper.UserID, SessionHelper.UserType, SessionHelper.RoleID, "room", SessionHelper.EnterPriceName, SessionHelper.CompanyName, objDTO.RoomName);
                        //
                        //
                        //    }
                        //    SessionHelper.RoomList = lst;
                        //}
                        //else if (SessionHelper.UserType < 4)
                        //{
                        //    List<RoomDTO> lst = SessionHelper.RoomList;
                        //    UserBAL objUserBAL = new UserBAL();
                        //    if (lst != null && lst.Count > 0)
                        //    {
                        //        lst.Add(new RoomDTO() { EnterpriseId = SessionHelper.EnterPriceID, CompanyID = SessionHelper.CompanyID, ID = ReturnVal, RoomName = objDTO.RoomName, IsRoomActive = objDTO.IsRoomActive });
                        //        //lst.Add(new KeyValuePair<long, string>(ReturnVal, objDTO.RoomName));
                        //        List<UserWiseRoomsAccessDetailsDTO> lstrepermissions = objUserBAL.AddNewRoomPermissions(SessionHelper.EnterPriceID, SessionHelper.CompanyID, ReturnVal, SessionHelper.UserID, SessionHelper.RoleID, SessionHelper.UserType);
                        //        SessionHelper.RoomPermissions = lstrepermissions;
                        //    }
                        //    else
                        //    {
                        //        lst = new List<RoomDTO>();
                        //        lst.Add(new RoomDTO() { EnterpriseId = SessionHelper.EnterPriceID, CompanyID = SessionHelper.CompanyID, ID = ReturnVal, RoomName = objDTO.RoomName, IsRoomActive = objDTO.IsRoomActive });
                        //        List<UserWiseRoomsAccessDetailsDTO> lstrepermissions = objUserBAL.AddNewRoomPermissions(SessionHelper.EnterPriceID, SessionHelper.CompanyID, ReturnVal, SessionHelper.UserID, SessionHelper.RoleID, SessionHelper.UserType);
                        //        //lstrepermissions = SetPermissionsForsuperadmin(lstrepermissions, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.RoleID, SessionHelper.UserType);
                        //        SessionHelper.RoomList = lst;
                        //        SessionHelper.RoomPermissions = lstrepermissions;
                        //        SetSessions(SessionHelper.EnterPriceID, SessionHelper.CompanyID, ReturnVal, SessionHelper.UserID, SessionHelper.UserType, SessionHelper.RoleID, "room", SessionHelper.EnterPriceName, SessionHelper.CompanyName, objDTO.RoomName);
                        //    }
                        //    SessionHelper.RoomList = lst;
                        //}
                        //UserBAL objUserBAL1 = new UserBAL();
                        //objUserBAL1.AddRoomPermissionToEpRoleUsers(SessionHelper.EnterPriceID, SessionHelper.CompanyID, ReturnVal, SessionHelper.UserID, SessionHelper.RoleID, SessionHelper.UserType);
                        if (ReturnVal > 0)
                        {

                            eTurns.DAL.LabelPrintingDAL.LabelFieldModuleTemplateDetailDAL objLabelTemplateDetailDAL = new eTurns.DAL.LabelPrintingDAL.LabelFieldModuleTemplateDetailDAL(SessionHelper.EnterPriseDBName);
                            objLabelTemplateDetailDAL.CopyAllBaseTemplateRoomLevel(SessionHelper.CompanyID, SessionHelper.UserID, ReturnVal);


                            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
                            eTurnsRegionInfo objeTurnsRegionInfo = new eTurnsRegionInfo();
                            objeTurnsRegionInfo.CompanyId = SessionHelper.CompanyID;
                            objeTurnsRegionInfo.CultureCode = "en-US";
                            objeTurnsRegionInfo.CultureDisplayName = "en-US";
                            objeTurnsRegionInfo.CultureName = "en-US";
                            objeTurnsRegionInfo.CurrencyDecimalDigits = 2;
                            objeTurnsRegionInfo.CurrencyGroupSeparator = null;
                            objeTurnsRegionInfo.EnterpriseId = SessionHelper.EnterPriceID;
                            objeTurnsRegionInfo.ID = 0;
                            objeTurnsRegionInfo.LongDatePattern = null;
                            objeTurnsRegionInfo.LongTimePattern = null;
                            objeTurnsRegionInfo.NumberDecimalDigits = 0;
                            objeTurnsRegionInfo.NumberDecimalSeparator = null;
                            objeTurnsRegionInfo.NumberGroupSeparator = null;
                            objeTurnsRegionInfo.RoomId = ReturnVal;
                            objeTurnsRegionInfo.ShortDatePattern = "M/d/yyyy";
                            objeTurnsRegionInfo.ShortTimePattern = "h:mm:ss tt";
                            objeTurnsRegionInfo.TimeZoneName = TimeZoneInfo.Utc.StandardName;
                            objeTurnsRegionInfo.TimeZoneOffSet = 0;
                            objeTurnsRegionInfo.UserId = SessionHelper.UserID;
                            objeTurnsRegionInfo.CurrencySymbol = null;
                            objeTurnsRegionInfo.GridRefreshTimeInSecond = 60;
                            objeTurnsRegionInfo.WeightDecimalPoints = 0;
                            objeTurnsRegionInfo.TurnsAvgDecimalPoints = 0;
                            objeTurnsRegionInfo.NumberOfBackDaysToSyncOverPDA = 10;

                            objeTurnsRegionInfo = objRegionSettingDAL.SaveRegionalSettings(objeTurnsRegionInfo);
                            SaveRoomWiseEmailTemplate(objDTO.CompanyID, ReturnVal);

                            message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                            status = "ok";
                            #region "RoomResources"
                            string appPath1 = ResourceHelper.ResourceDirectoryBasePath + @"\";
                            string resourceDirectory = appPath1 + objDTO.EnterpriseId + "\\RoomResources";//
                            if (!System.IO.Directory.Exists(appPath1 + objDTO.EnterpriseId + "\\" + objDTO.CompanyID + "\\" + objDTO.ID))
                            {
                                System.IO.Directory.CreateDirectory(appPath1 + objDTO.EnterpriseId + "\\" + objDTO.CompanyID + "\\" + objDTO.ID);
                            }
                            foreach (var file in System.IO.Directory.GetFiles(resourceDirectory))
                            {
                                System.IO.File.Copy(file, System.IO.Path.Combine(appPath1 + objDTO.EnterpriseId + "\\" + objDTO.CompanyID + "\\" + objDTO.ID, System.IO.Path.GetFileName(file)));
                            }

                            # endregion
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                            status = "fail";
                        }
                    }
                }
            }
            else
            {
                objDTO.LastUpdatedBy = SessionHelper.UserID;
                objDTO.UpdatedByName = SessionHelper.UserName;

                string EnterpriseDBName = new EnterpriseDAL(SessionHelper.EnterPriseDBName).GetEnterpriseDBName(objDTO.EnterpriseId);

                RoomDAL oRoomDAL = new RoomDAL(EnterpriseDBName);
                string strOK = oRoomDAL.RoomDuplicateCheck(objDTO.ID, objDTO.RoomName, objDTO.CompanyID ?? 0);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResRoomMaster.RoomName, objDTO.RoomName); // "Room \"" + objDTO.RoomName + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    //string strOK1 = obj.RoomEmailDuplicateCheck(objDTO.ID, objDTO.Email, SessionHelper.CompanyID);
                    string strOK1 = "ok";
                    if (strOK1 == "duplicate")
                    {
                        message = "Email \"" + objDTO.Email + "\" already exist! Try with Another!";
                        status = "duplicate";
                    }
                    else
                    {
                        objDTO = oRoomDAL.Edit(objDTO);
                        SupplierScheduleDTO.LastUpdatedBy = SessionHelper.UserID;

                        if (SupplierScheduleDTO.IsScheduleChanged == 1)
                        {
                            SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(EnterpriseDBName);
                            objSupplierMasterDAL.SaveSupplierSchedule(SupplierScheduleDTO);
                        }

                        if (objDTO.RoomActiveStatus > 0 || objDTO.RoomNameChange > 0)
                        {
                            NeedRefress = true;
                            if (SessionHelper.EnterPriceID == objDTO.EnterpriseId && SessionHelper.CompanyID == objDTO.CompanyID)
                            {
                                List<RoomDTO> lst = SessionHelper.RoomList;
                                if (lst != null && lst.Count > 0)
                                {
                                    lst = lst.Where(t => t.ID != objDTO.ID).ToList();
                                    lst.Add(new RoomDTO() { EnterpriseId = objDTO.EnterpriseId, CompanyID = objDTO.CompanyID, ID = objDTO.ID, RoomName = objDTO.RoomName, IsRoomActive = objDTO.IsRoomActive });
                                }
                                SessionHelper.RoomList = lst;
                            }

                            if (objDTO.RoomActiveStatus > 0)
                            {
                                if (objDTO.IsRoomActive)
                                {
                                    List<UserWiseRoomsAccessDetailsDTO> lstSessoion = SessionHelper.RoomPermissions;
                                    if (lstSessoion != null && lstSessoion.Count > 0 && lstSessoion.Where(t => t.EnterpriseId == objDTO.EnterpriseId && t.CompanyId == objDTO.CompanyID && t.RoomID == objDTO.ID).Any())
                                    {
                                        UserWiseRoomsAccessDetailsDTO objUserWiseRoomsAccessDetailsDTO = lstSessoion.FirstOrDefault(t => t.EnterpriseId == objDTO.EnterpriseId && t.CompanyId == objDTO.CompanyID && t.RoomID == objDTO.ID);
                                        if (objUserWiseRoomsAccessDetailsDTO != null)
                                        {
                                            if (SessionHelper.UserType > 1)
                                            {
                                                eTurns.DAL.UserMasterDAL objinterUserDAL = new eTurns.DAL.UserMasterDAL(EnterpriseDBName);
                                                List<UserRoleModuleDetailsDTO> lstUserRoleModuleDetailsDTO = objinterUserDAL.GetUserRoleModuleDetailsRecord(SessionHelper.UserID, SessionHelper.RoleID, SessionHelper.UserType);
                                                string strRoomList = string.Empty;
                                                List<UserWiseRoomsAccessDetailsDTO> lstUserWiseRoomsAccessDetails = objinterUserDAL.ConvertUserPermissions(lstUserRoleModuleDetailsDTO, SessionHelper.RoleID, ref strRoomList);
                                                SessionHelper.RoomPermissions = lstUserWiseRoomsAccessDetails;
                                            }
                                            else
                                            {
                                                if (SessionHelper.RoleID == -1 || SessionHelper.RoleID == -2)
                                                {
                                                    eTurnsMaster.DAL.UserMasterDAL objinterUserDAL = new eTurnsMaster.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                                                    //List<UserRoleModuleDetailsDTO> lstUserRoleModuleDetailsDTO = objinterUserDAL.GetUserRoleModuleDetailsRecord(SessionHelper.UserID, SessionHelper.RoleID, SessionHelper.UserType);
                                                    string strRoomList = string.Empty;
                                                    List<UserWiseRoomsAccessDetailsDTO> lstUserWiseRoomsAccessDetails = new eTurnsMaster.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName).GetSuperAdminRoomPermissions(objDTO.EnterpriseId, objDTO.CompanyID ?? 0, objDTO.ID, SessionHelper.RoleID, SessionHelper.UserID);
                                                    lstUserWiseRoomsAccessDetails = SetPermissionsForsuperadmin(lstUserWiseRoomsAccessDetails, objDTO.EnterpriseId, objDTO.CompanyID ?? 0, objDTO.ID, SessionHelper.RoleID, SessionHelper.UserType);
                                                    SessionHelper.RoomPermissions = lstUserWiseRoomsAccessDetails;
                                                }
                                                else
                                                {
                                                    eTurnsMaster.DAL.UserMasterDAL objinterUserDAL = new eTurnsMaster.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                                                    List<UserRoleModuleDetailsDTO> lstUserRoleModuleDetailsDTO = objinterUserDAL.GetUserRoleModuleDetailsRecord(SessionHelper.UserID, SessionHelper.RoleID, SessionHelper.UserType);
                                                    string strRoomList = string.Empty;
                                                    List<UserWiseRoomsAccessDetailsDTO> lstUserWiseRoomsAccessDetails = objinterUserDAL.ConvertUserPermissions(lstUserRoleModuleDetailsDTO, SessionHelper.RoleID, ref strRoomList);

                                                    //lstUserWiseRoomsAccessDetails = SetPermissionsForsuperadmin(lstUserWiseRoomsAccessDetails, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.RoleID, SessionHelper.UserType);
                                                    SessionHelper.RoomPermissions = lstUserWiseRoomsAccessDetails;
                                                }

                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (SessionHelper.RoomID == objDTO.ID)
                                    {
                                        SessionHelper.RoomID = objDTO.ID;
                                    }
                                    //set view rigths
                                }
                            }
                        }
                        if (objDTO.SOSettingChanged == 1 || objDTO.STSettingChanged == 1)
                        {
                            new CartItemDAL(EnterpriseDBName).SuggestedOrderRoom(objDTO.ID, objDTO.CompanyID ?? 0, SessionHelper.UserID, objDTO.EnterpriseId);
                        }
                        if (objDTO != null)
                        {
                            message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                            status = "ok";
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                            status = "fail";
                        }
                    }
                }
            }

            //Create resource directory for room
            string ResourcePath = System.Web.HttpContext.Current.Server.MapPath(@"\Resources\");
            if (!System.IO.Directory.Exists(ResourcePath + @"\" + objDTO.EnterpriseId.ToString()))
                System.IO.Directory.CreateDirectory(ResourcePath + @"\" + objDTO.EnterpriseId.ToString());
            if (!System.IO.Directory.Exists(ResourcePath + @"\" + objDTO.EnterpriseId.ToString() + @"\" + objDTO.CompanyID.ToString()))
                System.IO.Directory.CreateDirectory(ResourcePath + @"\" + objDTO.EnterpriseId.ToString() + @"\" + objDTO.CompanyID.ToString());
            if (!System.IO.Directory.Exists(ResourcePath + @"\" + objDTO.EnterpriseId.ToString() + @"\" + objDTO.CompanyID.ToString() + @"\" + objDTO.ID.ToString()))
                System.IO.Directory.CreateDirectory(ResourcePath + @"\" + objDTO.EnterpriseId.ToString() + @"\" + objDTO.CompanyID.ToString() + @"\" + objDTO.ID.ToString());
            //Create resource directory for room


            //if (status == "duplicate")
            //    throw new Exception("Duplicate Found", new Exception("Duplicate Found"));                
            //else if (status == "fail")
            //    throw new Exception("Error! Record Not Saved");
            //else
            //    return Content(message);

            return Json(new { Message = message, Status = status, refressPage = NeedRefress }, JsonRequestBehavior.AllowGet);
        }

        private DashboardParameterDTO AssignValueDashboard(DashboardParameterDTO objDTO)
        {
            DashboardParameterDTO obj = new DashboardParameterDTO();
            obj.ID = 0;
            obj.RoomId = objDTO.RoomId;
            obj.CompanyId = objDTO.CompanyId;
            obj.CreatedOn = objDTO.CreatedOn;
            obj.CreatedBy = objDTO.CreatedBy;
            obj.UpdatedOn = objDTO.UpdatedOn;
            obj.UpdatedBy = objDTO.UpdatedBy;
            obj.TurnsMeasureMethod = objDTO.TurnsMeasureMethod;
            obj.TurnsMonthsOfUsageToSample = objDTO.TurnsMonthsOfUsageToSample;
            obj.AUDayOfUsageToSample = objDTO.AUDayOfUsageToSample;
            obj.AUMeasureMethod = objDTO.AUMeasureMethod;
            obj.AUDaysOfDailyUsage = objDTO.AUDaysOfDailyUsage;
            obj.MinMaxMeasureMethod = objDTO.MinMaxMeasureMethod;
            obj.MinMaxDayOfUsageToSample = objDTO.MinMaxDayOfUsageToSample;
            obj.MinMaxDayOfAverage = objDTO.MinMaxDayOfAverage;
            obj.MinMaxMinNumberOfTimesMax = objDTO.MinMaxMinNumberOfTimesMax;
            obj.MinMaxOptValue1 = objDTO.MinMaxOptValue1;
            obj.MinMaxOptValue2 = objDTO.MinMaxOptValue2;
            obj.GraphFromMonth = objDTO.GraphFromMonth;
            obj.GraphToMonth = objDTO.GraphToMonth;
            obj.GraphFromYear = objDTO.GraphFromYear;
            obj.GraphToYear = objDTO.GraphToYear;
            obj.IsTrendingEnabled = objDTO.IsTrendingEnabled;
            obj.PieChartmetricOn = objDTO.PieChartmetricOn;
            obj.TurnsCalculatedStockRoomTurn = objDTO.TurnsCalculatedStockRoomTurn;
            obj.AUCalculatedDailyUsageOverSample = objDTO.AUCalculatedDailyUsageOverSample;
            obj.MinMaxCalculatedDailyUsageOverSample = objDTO.MinMaxCalculatedDailyUsageOverSample;
            obj.MinMaxCalcAvgPullByDay = objDTO.MinMaxCalcAvgPullByDay;
            obj.MinMaxCalcualtedMax = objDTO.MinMaxCalcualtedMax;
            obj.AutoClassification = objDTO.AutoClassification;

            return obj;
        }

        public void SaveRoomWiseEmailTemplate(long? CompanyId, long RoomId)
        {
            ResourceDAL objResourceDAL = new ResourceDAL(SessionHelper.EnterPriseDBName);
            objResourceDAL.SaveRoomWiseEmailTemplate(CompanyId ?? 0, RoomId, SessionHelper.UserID);
            //long CompanyID = 0;
            //CompanyID = CompanyId ?? 0;
            //IEnumerable<ResourceLanguageDTO> lstResourceLanguageDTO = null;
            //ResourceDAL objResourceDAL = new ResourceDAL(SessionHelper.EnterPriseDBName);
            //lstResourceLanguageDTO = objResourceDAL.GetCachedResourceLanguageData(CompanyID);
            //foreach (var item in lstResourceLanguageDTO)
            //{
            //    List<EmailTemplateDTO> lstEmailTemplate = new List<EmailTemplateDTO>();
            //    lstEmailTemplate = GetEmailTemplateFromDirectory(item.Culture);
            //    RoomDAL objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
            //    objRoomDAL.SaveEmailTemplateByRoom(CompanyID, RoomId, lstEmailTemplate, SessionHelper.UserID, item.ID);
            //}
        }



        public List<EmailTemplateDTO> GetEmailTemplateFromDirectory(string CurCulture)
        {
            List<EmailTemplateDTO> lstItem = new List<EmailTemplateDTO>();
            try
            {
                // CultureInfo str = eTurns.DTO.Resources.ResourceHelper.CurrentCult;

                DirectoryInfo drInfo = null;
                drInfo = new DirectoryInfo(Server.MapPath("../Content/EmailTemplates"));
                if (drInfo.Exists)
                {

                    foreach (FileInfo objFileInfo in drInfo.GetFiles("*" + CurCulture + "*"))
                    {
                        if (objFileInfo.Extension == ".html")
                        {

                            EmailTemplateDTO obj = new EmailTemplateDTO();
                            obj.TemplateName = objFileInfo.Name.Split('-')[0].ToString(); //objFileInfo.Name;
                            if (System.IO.File.Exists(Server.MapPath("../") + "Content\\EmailTemplates\\" + obj.TemplateName + "-" + CurCulture + ".html"))
                            {
                                obj.MailBodyText = (System.IO.File.ReadAllText(Server.MapPath("../") + "Content\\EmailTemplates/" + obj.TemplateName + "-" + CurCulture + ".html"));
                                obj.MailSubject = obj.TemplateName;
                                lstItem.Add(obj);
                            }

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

        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>
        [AjaxOrChildActionOnlyAttribute]
        public ActionResult RoomEdit(string ID)
        {
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            if (IsDeleted || IsArchived)
            {
                ViewBag.ViewOnly = true;
            }

            var list1 = new SelectList(new[]{
                          new {ID="0",Name=""},new {ID="1",Name="1"},new{ID="2",Name="2"},
                          new{ID="3",Name="3"},new {ID="4",Name="4"},new{ID="5",Name="5"},new{ID="6",Name="6"},
                          new {ID="7",Name="7"},new{ID="8",Name="8"},new{ID="9",Name="9"},new{ID="10",Name="10"},},
                         "ID", "Name", 1);
            ViewBag.Days = list1;

            var list2 = new SelectList(new[]{
                        new {ID="0",Name=""},new {ID="5",Name="5"},new{ID="10",Name="10"},new {ID="15",Name="15"},new{ID="20",Name="20"},new {ID="25",Name="25"},new{ID="30",Name="30"},new {ID="35",Name="35"},new{ID="40",Name="40"},new {ID="45",Name="45"},new{ID="50",Name="50"},
                        new {ID="55",Name="55"},new{ID="60",Name="60"},new {ID="65",Name="65"},new{ID="70",Name="70"},new {ID="75",Name="75"},new{ID="80",Name="80"},new {ID="85",Name="85"},new{ID="90",Name="90"},new {ID="95",Name="95"},new{ID="100",Name="100"},
                        new {ID="105",Name="105"},new{ID="110",Name="110"},new {ID="115",Name="115"},new{ID="120",Name="120"},new {ID="125",Name="125"},new{ID="130",Name="130"},new {ID="135",Name="135"},new{ID="140",Name="140"},new {ID="145",Name="145"},new{ID="150",Name="150"},
                        new {ID="155",Name="155"},new{ID="160",Name="160"},new {ID="165",Name="165"},new{ID="170",Name="170"},new {ID="175",Name="175"},new{ID="180",Name="180"},},
                          "ID", "Name", 1);
            ViewBag.AvgDays = list2;

            var listMethodOfValuing = new SelectList(new[]{
                          new{ID="3",Name="Average cost"},new{ID="4",Name="Last cost"}}, "ID", "Name", 1);
            ViewBag.MethodOfValuing = listMethodOfValuing;

            var listWeeks = new SelectList(new[]{
                          new {ID="1",Name="First"},new{ID="2",Name="Second"},
                          new{ID="3",Name="Third"},new{ID="4",Name="Fourth"},new{ID="5",Name="Fifth"},},
                 "ID", "Name", 1);
            ViewBag.Weeks = listWeeks;

            var listdayName = new SelectList(new[]{
                          new {ID="Monday",Name="Monday"},new{ID="Tuesday",Name="Tuesday"},
                          new{ID="Wednesday",Name="Wednesday"},new{ID="Thursday",Name="Thursday"},new{ID="Friday",Name="Friday"},
            new{ID="Saturday",Name="Saturday"},new{ID="Sunday",Name="Sunday"},
            },
                  "ID", "Name", 1);
            ViewBag.DayNames = listdayName;

            ViewBag.InventoryConsuptionMethodBAG = InventoryConsuptionOptions();

            long RoomID = 0;
            long EnterpriseId = 0;
            string[] arystrIds = ID.Split("_".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (arystrIds.Length > 0)
                RoomID = Convert.ToInt64(arystrIds[0]);
            if (arystrIds.Length > 1)
                EnterpriseId = Convert.ToInt64(arystrIds[1]);

            string EnterpriseDBName = new EnterpriseDAL(SessionHelper.EnterPriseDBName).GetEnterpriseDBName(EnterpriseId);

            RoomDAL objRoom = new RoomDAL(EnterpriseDBName);
            RoomDTO objDTO = objRoom.GetRoomByID(RoomID);

            //RoomController objRoom = new RoomController();
            List<RoomDTO> objList = objRoom.GetAllRecords(objDTO.CompanyID ?? 0, false, false).Where(t => t.ID != RoomID).ToList();
            objList.Insert(0, null);
            ViewBag.ReplineshmentRoomList = objList;
            ViewBag.ReplenishmentTypeList = GetReplanishmentTypeOptions();
            //RoomController obj = new RoomController();

            if (objDTO != null)
            {
                objDTO.CreatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                objDTO.UpdatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                objDTO.EnterpriseId = EnterpriseId;
            }
            BinMasterDAL objBinMasterDAL = new BinMasterDAL(EnterpriseDBName);
            ViewBag.DefaultLocationList = objBinMasterDAL.GetAllRecords(objDTO.ID, objDTO.CompanyID ?? 0, false, false).Where(t => t.IsStagingLocation != true);

            SupplierMasterDAL objSupplier = new SupplierMasterDAL(EnterpriseDBName);
            if (objDTO != null)
            {
                ViewBag.DefaultSupplierList = objSupplier.GetAllRecords(objDTO.ID, objDTO.CompanyID ?? 0, false, false, false);
            }
            else
            {
                ViewBag.DefaultSupplierList = new List<SupplierMasterDTO>();
            }

            ViewBag.UDFs = objUDFDAL.GetRoomMasterUDFDataPageWise(EnterpriseDBName, objDTO.CompanyID ?? 0, "Room");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;
            ModelState.Remove("ID");

            return PartialView("_CreateRoom", objDTO);
            //return View("TechnicianEdit",objDTO);
        }

        [HttpGet]
        public JsonResult GetRoomSuppliers(string NameStartWith, long RoomId)
        {
            SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<SupplierMasterDTO> SupplierList = objSupplierMasterDAL.GetAllRecords(RoomId, SessionHelper.CompanyID, false, false, false).OrderBy(t => t.SupplierName);
            if (SupplierList != null && SupplierList.Count() > 0)
            {
                if (!string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                {
                    SupplierList = SupplierList.Where(objConfig => objConfig.SupplierName.ToLower().StartsWith(NameStartWith.ToLower())).OrderBy(t => t.SupplierName);
                    return Json(SupplierList, JsonRequestBehavior.AllowGet);
                }
                else if (NameStartWith.Contains(" "))
                {
                    return Json(SupplierList, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(SupplierList, JsonRequestBehavior.AllowGet);
        }

        #region Ajax Data Provider

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult RoomListAjax(JQueryDataTableParamModel param)
        {
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            if (IsDeleted || IsArchived)
            {
                ViewBag.ViewOnly = true;
            }

            //RoomController obj = new RoomController();
            RoomDAL obj = new RoomDAL(SessionHelper.EnterPriseDBName);

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

            // set the default column sorting here, if first time then required to set 
            //if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID";

            //if (sortDirection == "asc")
            //    sortColumnName = sortColumnName + " asc";
            //else
            //    sortColumnName = sortColumnName + " desc";
            if (sortColumnName == "0" || sortColumnName == "undefined" || string.IsNullOrEmpty(sortColumnName))
                sortColumnName = "ID Desc";

            string searchQuery = string.Empty;
            string UserRooms = string.Empty;
            int TotalRecordCount = 0;

            if (SessionHelper.RoleID != -1)
            {
                List<RoomDTO> lstRooms = new List<RoomDTO>();
                if (Session["AllRooms"] != null)
                {
                    lstRooms = (List<RoomDTO>)Session["AllRooms"];
                }
                else
                {
                    lstRooms = new RoomDAL(SessionHelper.EnterPriseDBName).GetAllRoomsFromETurnsMaster(SessionHelper.CompanyID, false, false, SessionHelper.RoomList, string.Empty).ToList();
                }

                UserRooms = string.Join(",", lstRooms.Select(t => t.ID).ToArray());
            }

            if (IsDeleted)
            {
                UserRooms = string.Empty;
            }
            if (string.IsNullOrWhiteSpace(UserRooms) && SessionHelper.RoleID != -1)
            {
                UserRooms = "0,0";
            }
            var DataFromDB = obj.GetPagedRooms(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, IsArchived, IsDeleted, SessionHelper.CompanyID, UserRooms);

            //if (DataFromDB != null)
            //{
            //    DataFromDB.ToList().ForEach(t =>
            //    {
            //        t.CreatedDate = CommonUtility.ConvertDateByTimeZone(t.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
            //        t.UpdatedDate = CommonUtility.ConvertDateByTimeZone(t.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);

            //        t.LastOrderDateStr = CommonUtility.ConvertDateByTimeZone(t.LastOrderDate, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
            //        t.LicenseBilledStr = CommonUtility.ConvertDateByTimeZone(t.LicenseBilled, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);

            //        t.LastPullDateStr = CommonUtility.ConvertDateByTimeZone(t.LastPullDate, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);

            //    });

            //}
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);
        }

        public string UpdateRoomData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            //RoomController obj = new RoomController();
            RoomDAL obj = new RoomDAL(SessionHelper.EnterPriseDBName);
            obj.UpdateData(id, value, rowId, columnPosition, columnId, columnName);
            return value;
        }

        //public string DuplicateRoomCheck(string RoomName, string ActionMode, int ID)
        //{
        //    RoomController obj = new RoomController();
        //    return obj.DuplicateCheck(RoomName, ActionMode, ID);
        //}

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public JsonResult DeleteRoomRecords(string ids)
        {
            try
            {
                //string deleteIDs = "";
                //if (ids.LastIndexOf(",") != -1)
                //{
                //    foreach (var item in ids.Split(','))
                //    {
                //        if (!String.IsNullOrEmpty(item))
                //        {
                //            deleteIDs += "'" + item + "',";
                //        }
                //    }
                //    deleteIDs = deleteIDs.Substring(0, deleteIDs.LastIndexOf(","));
                //}

                //RoomController obj = new RoomController();
                //RoomDAL obj = new RoomDAL(SessionHelper.EnterPriseDBName);
                //obj.DeleteRecords(ids, SessionHelper.UserID, SessionHelper.CompanyID);

                //Dictionary<long, string> dictionary = new Dictionary<long, string>();
                List<EntpCompanyRoom> oEntpCompanyRoomList = new List<EntpCompanyRoom>();
                string[] arystrIds = ids.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in arystrIds)
                {
                    long RoomID = 0;
                    long EnterpriseId = 0;
                    long CompanyId = 0;
                    string[] aryStr = item.Split("_".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (aryStr.Length > 0)
                        RoomID = Convert.ToInt64(aryStr[0]);
                    if (aryStr.Length > 1)
                        EnterpriseId = Convert.ToInt64(aryStr[1]);
                    if (aryStr.Length > 2)
                        CompanyId = Convert.ToInt64(aryStr[2]);

                    EntpCompanyRoom oEntpCompanyRoom = oEntpCompanyRoomList.Where(x => x.EnterpriseID == EnterpriseId && x.CompanyID == CompanyId).FirstOrDefault();
                    if (oEntpCompanyRoom == null)
                    {
                        oEntpCompanyRoom = new EntpCompanyRoom();
                        oEntpCompanyRoom.EnterpriseID = EnterpriseId;
                        oEntpCompanyRoom.CompanyID = CompanyId;
                        oEntpCompanyRoom.RoomIDs = RoomID.ToString();
                        oEntpCompanyRoomList.Add(oEntpCompanyRoom);
                    }
                    else
                    {
                        oEntpCompanyRoom.RoomIDs += "," + RoomID.ToString();
                    }
                }

                string response = string.Empty;
                List<DeleteStatusDTO> listDeleteStatusDTO = new List<DeleteStatusDTO>();
                List<EntpCompanyRoom> oSuccessEntpCompanyRoomList = new List<EntpCompanyRoom>();
                foreach (EntpCompanyRoom itemEntpCompanyRoom in oEntpCompanyRoomList)
                {
                    long EnterpriseId = itemEntpCompanyRoom.EnterpriseID;
                    long CompanyId = itemEntpCompanyRoom.CompanyID;
                    string RoomIds = itemEntpCompanyRoom.RoomIDs;

                    string EnterpriseDBName = new EnterpriseDAL(SessionHelper.EnterPriseDBName).GetEnterpriseDBName(EnterpriseId);

                    CommonDAL objCommonDAL = new CommonDAL(EnterpriseDBName);
                    List<DeleteStatusDTO> olistDeleteStatusDTO = objCommonDAL.DeleteModulewiseRetList(RoomIds, ImportMastersDTO.TableName.Room.ToString(), false, SessionHelper.UserID);
                    listDeleteStatusDTO.AddRange(olistDeleteStatusDTO);

                    string[] EntSuccessIds = olistDeleteStatusDTO.Where(t => t.Status == "Success").Select(objConfig => objConfig.Id).ToArray();
                    string entStrIds = string.Join(",", EntSuccessIds);

                    EntpCompanyRoom oEntpCompanyRoom = oSuccessEntpCompanyRoomList.Where(x => x.EnterpriseID == EnterpriseId && x.CompanyID == CompanyId).FirstOrDefault();
                    if (oEntpCompanyRoom == null)
                    {
                        oEntpCompanyRoom = new EntpCompanyRoom();
                        oEntpCompanyRoom.EnterpriseID = EnterpriseId;
                        oEntpCompanyRoom.CompanyID = CompanyId;
                        oEntpCompanyRoom.RoomIDs = entStrIds;
                        oSuccessEntpCompanyRoomList.Add(oEntpCompanyRoom);
                    }
                    else
                    {
                        oEntpCompanyRoom.RoomIDs += "," + entStrIds;
                    }
                }

                int Failcnt = 0;
                int Successcnt = 0;

                Failcnt = listDeleteStatusDTO.Where(t => t.Status == "Fail").Count();
                Successcnt = listDeleteStatusDTO.Where(t => t.Status == "Success").Count();
                //string[] SuccessIds = listDeleteStatusDTO.Where(t => t.Status == "Success").Select(objConfig => objConfig.Id).ToArray();

                foreach (EntpCompanyRoom itemSuccEntpCompanyRoom in oSuccessEntpCompanyRoomList)
                {
                    string EnterpriseDBName = new EnterpriseDAL(SessionHelper.EnterPriseDBName).GetEnterpriseDBName(itemSuccEntpCompanyRoom.EnterpriseID);

                    string[] SuccessIds = itemSuccEntpCompanyRoom.RoomIDs.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    UserBAL objUserBAL = new UserBAL();
                    objUserBAL.DeleteRoomReferences(itemSuccEntpCompanyRoom.EnterpriseID, itemSuccEntpCompanyRoom.CompanyID, SuccessIds, EnterpriseDBName);

                    if (SessionHelper.EnterPriceID == itemSuccEntpCompanyRoom.EnterpriseID && SessionHelper.CompanyID == itemSuccEntpCompanyRoom.CompanyID)
                    {
                        long[] arrids = SuccessIds.ToIntArray().Where(t => t > 0).ToArray();
                        List<RoomDTO> lstRooms = SessionHelper.RoomList;
                        lstRooms = (from ri in lstRooms
                                    where !arrids.Contains(ri.ID)
                                    select ri).ToList();
                        SessionHelper.RoomList = lstRooms.OrderBy(t => t.RoomName).ToList();
                        if (arrids.Contains(SessionHelper.RoomID))
                        {
                            if (SessionHelper.RoomList.Count > 0)
                            {
                                RoomDTO objRoomDTO = SessionHelper.RoomList.First();
                                SetSessions(objRoomDTO.EnterpriseId, objRoomDTO.CompanyID ?? 0, objRoomDTO.ID, SessionHelper.UserID, SessionHelper.UserType, SessionHelper.RoleID, "room", SessionHelper.EnterPriceName, objRoomDTO.CompanyName, objRoomDTO.RoomName);
                            }
                            else
                            {
                                SetSessions(SessionHelper.EnterPriceID, SessionHelper.CompanyID, 0, SessionHelper.UserID, SessionHelper.UserType, SessionHelper.RoleID, "room", SessionHelper.EnterPriceName, SessionHelper.CompanyName, string.Empty);
                            }
                        }
                    }
                }

                if (Successcnt > 0)
                    response = Successcnt + " record(s) deleted successfully.";

                if (Failcnt > 0)
                {
                    if (string.IsNullOrEmpty(response))
                    {
                        response = Failcnt + " record(s) used in another module.";
                    }
                    else
                    {
                        response = response + " " + Failcnt + " record(s) used in another module.";
                    }
                }

                eTurns.DAL.CacheHelper<IEnumerable<RoomDTO>>.InvalidateCache();

                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);

                //return "not ok";
            }
            catch (Exception)
            {
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Method called but plugin when a row is undeleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if undelete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public JsonResult UnDeleteRoomRecords(string ids)
        {
            try
            {
                RoomDAL objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
                string response = string.Empty;
                List<EntpCompanyRoom> oEntpCompanyRoomList = new List<EntpCompanyRoom>();
                string[] arystrIds = ids.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in arystrIds)
                {
                    long RoomID = 0;
                    long EnterpriseId = 0;
                    long CompanyId = 0;
                    string[] aryStr = item.Split("_".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (aryStr.Length > 0)
                        RoomID = Convert.ToInt64(aryStr[0]);
                    if (aryStr.Length > 1)
                        EnterpriseId = Convert.ToInt64(aryStr[1]);
                    if (aryStr.Length > 2)
                        CompanyId = Convert.ToInt64(aryStr[2]);
                    if (EnterpriseId == SessionHelper.EnterPriceID && CompanyId == SessionHelper.CompanyID)
                    {
                        eTurnsMaster.DAL.UserMasterDAL oUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                        List<UserMasterDTO> oUserMasterDTO = oUserMasterDAL.GetAllRecords().Where(x => x.UserType == 1 && x.RoleID > 0).ToList();
                        foreach (var items in oUserMasterDTO)
                        {
                            if (SessionHelper.UserID != items.ID)
                                new UserBAL().AddNewRoomPermissions(EnterpriseId, CompanyId, RoomID, items.ID, items.RoleID, items.UserType);
                        }
                        if (EnterpriseId == SessionHelper.EnterPriceID && CompanyId == SessionHelper.CompanyID && SessionHelper.RoomID == 0)
                        {
                            SessionHelper.RoomID = RoomID;
                        }
                    }
                    EntpCompanyRoom oEntpCompanyRoom = oEntpCompanyRoomList.Where(x => x.EnterpriseID == EnterpriseId && x.CompanyID == CompanyId).FirstOrDefault();
                    if (oEntpCompanyRoom == null)
                    {
                        oEntpCompanyRoom = new EntpCompanyRoom();
                        oEntpCompanyRoom.EnterpriseID = EnterpriseId;
                        oEntpCompanyRoom.CompanyID = CompanyId;
                        oEntpCompanyRoom.RoomIDs = RoomID.ToString();
                        oEntpCompanyRoomList.Add(oEntpCompanyRoom);
                    }
                    else
                    {
                        oEntpCompanyRoom.RoomIDs += "," + RoomID.ToString();
                    }
                }

                List<DeleteStatusDTO> listDeleteStatusDTO = new List<DeleteStatusDTO>();
                List<EntpCompanyRoom> oSuccessEntpCompanyRoomList = new List<EntpCompanyRoom>();
                foreach (EntpCompanyRoom itemEntpCompanyRoom in oEntpCompanyRoomList)
                {
                    long EnterpriseId = itemEntpCompanyRoom.EnterpriseID;
                    long CompanyId = itemEntpCompanyRoom.CompanyID;
                    string RoomIds = itemEntpCompanyRoom.RoomIDs;

                    string EnterpriseDBName = new EnterpriseDAL(SessionHelper.EnterPriseDBName).GetEnterpriseDBName(EnterpriseId);

                    CommonDAL objCommonDAL = new CommonDAL(EnterpriseDBName);
                    List<DeleteStatusDTO> olistDeleteStatusDTO = objCommonDAL.UnDeleteModulewiseRetList(RoomIds, ImportMastersDTO.TableName.Room.ToString(), false, SessionHelper.UserID);
                    listDeleteStatusDTO.AddRange(olistDeleteStatusDTO);

                    string[] EntSuccessIds = olistDeleteStatusDTO.Where(t => t.Status == "Success").Select(objConfig => objConfig.Id).ToArray();
                    string entStrIds = string.Join(",", EntSuccessIds);

                    EntpCompanyRoom oEntpCompanyRoom = oSuccessEntpCompanyRoomList.Where(x => x.EnterpriseID == EnterpriseId && x.CompanyID == CompanyId).FirstOrDefault();
                    if (oEntpCompanyRoom == null)
                    {
                        oEntpCompanyRoom = new EntpCompanyRoom();
                        oEntpCompanyRoom.EnterpriseID = EnterpriseId;
                        oEntpCompanyRoom.CompanyID = CompanyId;
                        oEntpCompanyRoom.RoomIDs = entStrIds;
                        oSuccessEntpCompanyRoomList.Add(oEntpCompanyRoom);
                    }
                    else
                    {
                        oEntpCompanyRoom.RoomIDs += "," + entStrIds;
                    }
                }

                int Failcnt = 0;
                int Successcnt = 0;

                Failcnt = listDeleteStatusDTO.Where(t => t.Status == "Fail").Count();
                Successcnt = listDeleteStatusDTO.Where(t => t.Status == "Success").Count();
                //string[] SuccessIds = listDeleteStatusDTO.Where(t => t.Status == "Success").Select(objConfig => objConfig.Id).ToArray();

                foreach (EntpCompanyRoom itemSuccEntpCompanyRoom in oSuccessEntpCompanyRoomList)
                {
                    string[] SuccessIds = itemSuccEntpCompanyRoom.RoomIDs.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    if (SessionHelper.EnterPriceID == itemSuccEntpCompanyRoom.EnterpriseID && SessionHelper.CompanyID == itemSuccEntpCompanyRoom.CompanyID)
                    {
                        long[] arrids = SuccessIds.ToIntArray().Where(t => t > 0).ToArray();
                        List<RoomDTO> lstRooms = SessionHelper.RoomList;
                        lstRooms = (from ri in lstRooms
                                    where !arrids.Contains(ri.ID)
                                    select ri).ToList();
                        foreach (long UnDeleteRoomID in arrids)
                        {
                            RoomDTO objRoomDTOAdd = null;
                            objRoomDTOAdd = objRoomDAL.GetRecord(UnDeleteRoomID, SessionHelper.CompanyID, false, false);
                            if (objRoomDTOAdd != null)
                            {
                                objRoomDTOAdd.EnterpriseId = SessionHelper.EnterPriceID;
                                objRoomDTOAdd.EnterpriseName = SessionHelper.EnterPriceName;
                                lstRooms.Add(objRoomDTOAdd);
                            }
                        }
                        SessionHelper.RoomList = lstRooms.OrderBy(t => t.RoomName).ToList();
                        if (arrids.Contains(SessionHelper.RoomID))
                        {
                            if (SessionHelper.RoomList.Count > 0)
                            {
                                RoomDTO objRoomDTO = SessionHelper.RoomList.First();
                                SetSessions(objRoomDTO.EnterpriseId, objRoomDTO.CompanyID ?? 0, objRoomDTO.ID, SessionHelper.UserID, SessionHelper.UserType, SessionHelper.RoleID, "room", SessionHelper.EnterPriceName, objRoomDTO.CompanyName, objRoomDTO.RoomName);
                            }
                            else
                            {
                                SetSessions(SessionHelper.EnterPriceID, SessionHelper.CompanyID, 0, SessionHelper.UserID, SessionHelper.UserType, SessionHelper.RoleID, "room", SessionHelper.EnterPriceName, SessionHelper.CompanyName, string.Empty);
                            }
                        }
                    }
                }

                if (Successcnt > 0)
                {
                    response = Successcnt + " record(s) restored successfully.";
                }
                if (Failcnt > 0)
                {
                    if (string.IsNullOrEmpty(response))
                    {
                        response = Failcnt + " record(s) used in another module.";
                    }
                    else
                    {
                        response = response + " " + Failcnt + " record(s) used in another module.";
                    }
                }

                eTurns.DAL.CacheHelper<IEnumerable<RoomDTO>>.InvalidateCache();
                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }



        #endregion

        #endregion

        #region "SupplierMaster"

        [HttpPost]
        public JsonResult GetSupplierBPOS(long SupplierID)
        {
            SupplierBlanketPODetailsDAL objSupplierBlanketPODetailsDAL = new SupplierBlanketPODetailsDAL(SessionHelper.EnterPriseDBName);
            List<SupplierBlanketPODetailsDTO> lstItemSupplier = objSupplierBlanketPODetailsDAL.GetAllBlanketPOBySupplierID(SupplierID, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            return Json(lstItemSupplier);
        }
        [HttpPost]
        public JsonResult GetSupplierACS(long SupplierID)
        {
            SupplierAccountDetailsDAL objSupplierAccountDetailsDAL = new SupplierAccountDetailsDAL(SessionHelper.EnterPriseDBName);
            List<SupplierAccountDetailsDTO> lstItemSupplier = objSupplierAccountDetailsDAL.GetAllAccountsBySupplierID(SupplierID, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            return Json(lstItemSupplier);
        }
        public ActionResult SupplierList()
        {
            //CartItemDAL objCartItemDAL = new CartItemDAL(SessionHelper.EnterPriseDBName);
            //IList<OrderMasterDTO> lstOrders = objCartItemDAL.GetOrdersByCartIds(null, 3, 201180000, 2, "10059_E1770276-DA6A-4C63-8805-9E5F76C86429", 457);
            //objCartItemDAL.CreateOrdersByCart(lstOrders.ToList(), 3, 201180000, 2, "10059_E1770276-DA6A-4C63-8805-9E5F76C86429", 2);
            return View();
        }

        public PartialViewResult _CreateSupplier()
        {
            return PartialView();
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult SupplierCreate(bool isforbom, Guid? ITEMGUID)
        {
            Session["SupplierBlanketPO"] = null;
            Session["SupplierAccount"] = null;

            SupplierMasterDTO objDTO = new SupplierMasterDTO();
            objDTO.ID = 0;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.RoomName = SessionHelper.RoomName;
            objDTO.CreatedByName = SessionHelper.UserName;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.isForBOM = isforbom;
            objDTO.GUID = Guid.NewGuid();
            objDTO.IsOnlyFromItemUI = true;
            if (ITEMGUID != null)
            {
                objDTO.ItemGUID = ITEMGUID;
            }

            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("SupplierMaster");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }


            ViewBag.POSequenceList = null;

            objDTO.ImageType = "ExternalImage";
            return PartialView("_CreateSupplier", objDTO);
            //return View();
        }

        /// <summary>
        /// JSON Record Save - Enter key Save/Update
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Technician"></param>
        /// <returns></returns>
        public JsonResult SupplierSave(SupplierMasterDTO objDTO, SchedulerDTO SupplierScheduleDTO, PullSchedulerDTO PullSupplierScheduleDTO)
        {
            string message = "";
            string status = "";
            //SupplierMasterController obj = new SupplierMasterController();
            SupplierMasterDAL obj = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

            if (string.IsNullOrEmpty(objDTO.SupplierName))
            {
                message = string.Format(ResMessage.Required, ResSupplierMaster.Supplier);// "SupplierName name is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(objDTO.SupplierColor))
            {
                message = string.Format(ResMessage.Required, ResSupplierMaster.SupplierColor);// "SupplierName name is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }

            Int64 _NewIDForPopUp = 0;
            Int64 SupplierId = 0;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.MaximumOrderSize = objDTO.MaxOrderSize;
            if (objDTO.ID == 0)
            {
                #region "Insert Mode"

                objDTO.CreatedBy = SessionHelper.UserID;
                long RoomId = SessionHelper.RoomID;
                if (objDTO.isForBOM)
                {
                    RoomId = 0;
                }
                string strOK = objCDAL.DuplicateCheck(objDTO.SupplierName.ToLower().Trim(), "add", objDTO.ID, "SupplierMaster", "SupplierName", RoomId, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    SupplierMasterDTO objSupplierMasterDTO = new SupplierMasterDTO();
                    objSupplierMasterDTO = obj.GetAllRecords(RoomId, SessionHelper.CompanyID, false, false, false).Where(t => t.SupplierName.ToLower().Trim() == objDTO.SupplierName.ToLower().Trim() && t.isForBOM == true).ToList().FirstOrDefault();
                    if (objSupplierMasterDTO != null && objDTO.isForBOM == false)
                    {
                        objSupplierMasterDTO.isForBOM = false;
                        SupplierMasterDTO tempdto = new SupplierMasterDTO();
                        tempdto = obj.GetRecord(objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, objDTO.isForBOM);
                        if (tempdto != null)
                        {
                            objSupplierMasterDTO.AddedFrom = tempdto.AddedFrom;
                            objSupplierMasterDTO.ReceivedOnWeb = tempdto.ReceivedOnWeb;
                        }

                        if (obj.Edit(objSupplierMasterDTO))
                        {
                            message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                            status = "ok";
                            _NewIDForPopUp = objSupplierMasterDTO.ID;
                            return Json(new { Message = message, Status = status, NewIDForPopUp = _NewIDForPopUp }, JsonRequestBehavior.AllowGet);
                        }
                    }

                }
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResSupplierMaster.Supplier, objDTO.SupplierName);  // "SupplierName \"" + objDTO.SupplierName + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    string strOK1 = objCDAL.DuplicateCheck(objDTO.SupplierColor, "add", objDTO.ID, "SupplierMaster", "SupplierColor", SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (strOK1 == "duplicate")
                    {
                        message = string.Format(ResMessage.DuplicateMessage, ResSupplierMaster.SupplierColor, objDTO.SupplierColor);  // "Category \"" + objDTO.Category + "\" already exist! Try with Another!";
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
                        SupplierScheduleDTO.SupplierId = ReturnVal;
                        if (SupplierScheduleDTO.IsScheduleChanged == 1)
                        {
                            obj.SaveSupplierSchedule(SupplierScheduleDTO);
                        }
                        PullSupplierScheduleDTO.Pull_SupplierId = ReturnVal;
                        if (PullSupplierScheduleDTO.Pull_IsScheduleChanged == 1)
                        {
                            SchedulerDTO objPullSchedulerDTO = new SchedulerDTO();
                            objPullSchedulerDTO = ConvertFromPullSchedule(PullSupplierScheduleDTO);
                            objPullSchedulerDTO.CompanyId = SessionHelper.CompanyID;
                            obj.SaveSupplierSchedule(objPullSchedulerDTO);
                        }
                        if (ReturnVal > 0)
                        {

                            //if it comes from item screen then add it to session
                            if (objDTO.ItemGUID != null)
                            {
                                List<ItemSupplierDetailsDTO> lstItemSupplier = null;
                                if (Session["ItemSupplier"] != null)
                                {
                                    lstItemSupplier = (List<ItemSupplierDetailsDTO>)Session["ItemSupplier"];
                                }
                                else
                                {
                                    lstItemSupplier = new List<ItemSupplierDetailsDTO>();
                                }
                                lstItemSupplier.Add(new ItemSupplierDetailsDTO() { ID = 0, SupplierID = objDTO.ID, SessionSr = lstItemSupplier.Count + 1, ItemGUID = objDTO.ItemGUID, Room = SessionHelper.RoomID, CompanyID = SessionHelper.CompanyID, Updated = DateTimeUtility.DateTimeNow, LastUpdatedBy = SessionHelper.UserID, Created = DateTimeUtility.DateTimeNow, CreatedBy = SessionHelper.UserID });
                                Session["ItemSupplier"] = lstItemSupplier;
                                /// 
                            }


                            ///Save itemlocationwise quantity to database from the session
                            List<SupplierBlanketPODetailsDTO> lstSupplierBPOs = null;
                            if (objDTO != null && objDTO.SupplierBlanketPODetails != null && objDTO.SupplierBlanketPODetails.Count > 0 && objDTO.SupplierBlanketDirty == 1)
                            {
                                //lstItemManufactuer = objDTO.SupplierBlanketPODetails.Where(t => t.IsDeleted == false && t.ID > 0).ToList();
                                lstSupplierBPOs = objDTO.SupplierBlanketPODetails.Where(t => (t.IsDeleted == false && t.ID == 0) || (t.ID > 0)).ToList();
                                //ID == 0 and isdeletded=true
                                // ID==12 and isdeletded=true

                                foreach (var itembr in lstSupplierBPOs)
                                {
                                    itembr.SupplierID = ReturnVal;
                                    SupplierBlanketPODetailsDAL objSupplierBlanketPODetailsDAL = new SupplierBlanketPODetailsDAL(SessionHelper.EnterPriseDBName);
                                    if (itembr.ID > 0)
                                    {
                                        itembr.IsOnlyFromItemUI = true;
                                        itembr.EditedFrom = "Web";
                                        itembr.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        itembr.Created = DateTime.UtcNow;
                                        itembr.LastUpdatedBy = SessionHelper.UserID;
                                        itembr.Updated = DateTime.UtcNow;
                                        itembr.CreatedBy = SessionHelper.UserID;
                                        itembr.SupplierID = objDTO.ID;
                                        itembr.CompanyID = SessionHelper.CompanyID;
                                        itembr.Room = SessionHelper.RoomID;
                                        if (!String.IsNullOrWhiteSpace(itembr.ValidStartDate))
                                        {
                                            //DateTime.ParseExact(objDTO.RequiredDateStr, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult)
                                            DateTime ValidStartDate = DateTime.MinValue;
                                            if (DateTime.TryParseExact(itembr.ValidStartDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult, DateTimeStyles.None, out ValidStartDate))
                                            {
                                                if (ValidStartDate != DateTime.MinValue)
                                                {
                                                    itembr.StartDate = ValidStartDate;
                                                }
                                                else
                                                {
                                                    itembr.StartDate = null;
                                                }
                                            }
                                        }
                                        if (!String.IsNullOrWhiteSpace(itembr.ValidEndDate))
                                        {
                                            //DateTime.ParseExact(objDTO.RequiredDateStr, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult)
                                            DateTime ValidEndDate = DateTime.MinValue;
                                            if (DateTime.TryParseExact(itembr.ValidEndDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult, DateTimeStyles.None, out ValidEndDate))
                                            {
                                                if (ValidEndDate != DateTime.MinValue)
                                                {
                                                    itembr.Enddate = ValidEndDate;
                                                }
                                                else
                                                {
                                                    itembr.Enddate = null;
                                                }
                                            }
                                        }
                                        objSupplierBlanketPODetailsDAL.Edit(itembr);
                                    }
                                    else
                                    {
                                        itembr.AddedFrom = "Web";
                                        itembr.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                        itembr.EditedFrom = "Web";
                                        itembr.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        itembr.Created = DateTime.UtcNow;
                                        itembr.LastUpdatedBy = SessionHelper.UserID;
                                        itembr.Updated = DateTime.UtcNow;
                                        itembr.CreatedBy = SessionHelper.UserID;
                                        itembr.SupplierID = objDTO.ID;
                                        itembr.CompanyID = SessionHelper.CompanyID;
                                        itembr.Room = SessionHelper.RoomID;
                                        if (!String.IsNullOrWhiteSpace(itembr.ValidStartDate))
                                        {
                                            //DateTime.ParseExact(objDTO.RequiredDateStr, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult)
                                            DateTime ValidStartDate = DateTime.MinValue;
                                            if (DateTime.TryParseExact(itembr.ValidStartDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult, DateTimeStyles.None, out ValidStartDate))
                                            {
                                                if (ValidStartDate != DateTime.MinValue)
                                                {
                                                    itembr.StartDate = ValidStartDate;
                                                }
                                                else
                                                {
                                                    itembr.StartDate = null;
                                                }
                                            }
                                        }
                                        if (!String.IsNullOrWhiteSpace(itembr.ValidEndDate))
                                        {
                                            //DateTime.ParseExact(objDTO.RequiredDateStr, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult)
                                            DateTime ValidEndDate = DateTime.MinValue;
                                            if (DateTime.TryParseExact(itembr.ValidEndDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult, DateTimeStyles.None, out ValidEndDate))
                                            {
                                                if (ValidEndDate != DateTime.MinValue)
                                                {
                                                    itembr.Enddate = ValidEndDate;
                                                }
                                                else
                                                {
                                                    itembr.Enddate = null;
                                                }
                                            }
                                        }
                                        objSupplierBlanketPODetailsDAL.Insert(itembr);
                                    }
                                }
                            }
                            //
                            ///Save itemlocationwise quantity to database from the session
                            List<SupplierAccountDetailsDTO> lstSuppAccount = null;
                            if (objDTO != null && objDTO.SupplierAccountDetails != null && objDTO.SupplierAccountDetails.Count > 0 && objDTO.SupplierAccountDirty == 1)
                            {
                                //lstSuppAccount = ((List<SupplierAccountDetailsDTO>)Session["SupplierAccount"]).Where(t => !string.IsNullOrEmpty(t.AccountNo)).ToList();
                                lstSuppAccount = objDTO.SupplierAccountDetails.Where(t => (t.IsDeleted == false && t.ID == 0) || (t.ID > 0)).ToList();
                                foreach (var itembr in lstSuppAccount)
                                {
                                    itembr.SupplierID = ReturnVal;
                                    SupplierAccountDetailsDAL objSAD = new SupplierAccountDetailsDAL(SessionHelper.EnterPriseDBName);
                                    if (itembr.ID > 0)
                                    {
                                        itembr.Created = DateTime.UtcNow;
                                        itembr.LastUpdatedBy = SessionHelper.UserID;
                                        itembr.Updated = DateTime.UtcNow;
                                        itembr.CreatedBy = SessionHelper.UserID;
                                        itembr.SupplierID = objDTO.ID;
                                        itembr.CompanyID = SessionHelper.CompanyID;
                                        itembr.Room = SessionHelper.RoomID;
                                        objSAD.Edit(itembr);
                                    }
                                    else
                                    {
                                        itembr.Created = DateTime.UtcNow;
                                        itembr.LastUpdatedBy = SessionHelper.UserID;
                                        itembr.Updated = DateTime.UtcNow;
                                        itembr.CreatedBy = SessionHelper.UserID;
                                        itembr.SupplierID = objDTO.ID;
                                        itembr.CompanyID = SessionHelper.CompanyID;
                                        itembr.Room = SessionHelper.RoomID;
                                        objSAD.Insert(itembr);
                                    }
                                }

                                Session["SupplierAccount"] = null;
                            }

                            message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                            status = "ok";
                            _NewIDForPopUp = ReturnVal;
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                            status = "fail";
                        }
                    }
                }
                SupplierId = _NewIDForPopUp;
                #endregion
            }
            else
            {
                #region "Edit Mode"
                long RoomId = SessionHelper.RoomID;
                if (objDTO.isForBOM)
                {
                    RoomId = 0;
                }
                string strOK = objCDAL.DuplicateCheck(objDTO.SupplierName.ToLower().Trim(), "edit", objDTO.ID, "SupplierMaster", "SupplierName", RoomId, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResSupplierMaster.Supplier, objDTO.SupplierName);  //"SupplierName \"" + objDTO.SupplierName + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    string strOK1 = objCDAL.DuplicateCheck(objDTO.SupplierColor, "edit", objDTO.ID, "SupplierMaster", "SupplierColor", SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (strOK1 == "duplicate")
                    {
                        message = string.Format(ResMessage.DuplicateMessage, ResSupplierMaster.SupplierColor, objDTO.SupplierColor);  // "Category \"" + objDTO.Category + "\" already exist! Try with Another!";
                        status = "duplicate";
                    }
                    else
                    {
                        SupplierMasterDTO tempdto = new SupplierMasterDTO();
                        tempdto = obj.GetRecord(objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, objDTO.isForBOM);
                        if (tempdto != null)
                        {
                            objDTO.AddedFrom = tempdto.AddedFrom;
                            objDTO.ReceivedOnWeb = tempdto.ReceivedOnWeb;
                        }
                        if (objDTO.IsOnlyFromItemUI)
                        {
                            objDTO.EditedFrom = "Web";
                            objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        }
                        bool ReturnVal = obj.Edit(objDTO);
                        if (objDTO.isForBOM == true)
                        {
                            BOMItemMasterDAL objBOMItemMasterDAL = new BOMItemMasterDAL(SessionHelper.EnterPriseDBName);
                            objBOMItemMasterDAL.UpdateBOMMasterReference(objDTO.ID, "SupplierMaster", SessionHelper.UserID);
                        }
                        SupplierScheduleDTO.LastUpdatedBy = SessionHelper.UserID;
                        if (SupplierScheduleDTO.IsScheduleChanged == 1)
                        {
                            obj.SaveSupplierSchedule(SupplierScheduleDTO);
                        }

                        PullSupplierScheduleDTO.Pull_LastUpdatedBy = SessionHelper.UserID;
                        if (PullSupplierScheduleDTO.Pull_IsScheduleChanged == 1)
                        {
                            SchedulerDTO objPullSchedulerDTO = new SchedulerDTO();
                            objPullSchedulerDTO = ConvertFromPullSchedule(PullSupplierScheduleDTO);
                            objPullSchedulerDTO.CompanyId = SessionHelper.CompanyID;
                            obj.SaveSupplierSchedule(objPullSchedulerDTO);
                        }

                        if (ReturnVal)
                        {
                            ///Save itemlocationwise quantity to database from the session
                            List<SupplierBlanketPODetailsDTO> lstSupplierBPOs = null;
                            if (objDTO != null && objDTO.SupplierBlanketPODetails != null && objDTO.SupplierBlanketPODetails.Count > 0 && objDTO.SupplierBlanketDirty == 1)
                            {
                                //lstItemManufactuer = objDTO.SupplierBlanketPODetails.Where(t => t.IsDeleted == false && t.ID > 0).ToList();
                                lstSupplierBPOs = objDTO.SupplierBlanketPODetails.Where(t => (t.IsDeleted == false && t.ID == 0) || (t.ID > 0)).ToList();
                                //ID == 0 and isdeletded=true
                                // ID==12 and isdeletded=true

                                foreach (var itembr in lstSupplierBPOs)
                                {
                                    SupplierBlanketPODetailsDAL objSupplierBlanketPODetailsDAL = new SupplierBlanketPODetailsDAL(SessionHelper.EnterPriseDBName);
                                    if (itembr.ID > 0)
                                    {
                                        if (itembr.BlanketPO != null)
                                            itembr.BlanketPO = itembr.BlanketPO.Length > 22 ? itembr.BlanketPO.Substring(0, 22) : itembr.BlanketPO;
                                        itembr.IsOnlyFromItemUI = true;
                                        itembr.EditedFrom = "Web";
                                        itembr.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        itembr.Created = DateTime.UtcNow;
                                        itembr.LastUpdatedBy = SessionHelper.UserID;
                                        itembr.Updated = DateTime.UtcNow;
                                        itembr.CreatedBy = SessionHelper.UserID;
                                        itembr.SupplierID = objDTO.ID;
                                        itembr.CompanyID = SessionHelper.CompanyID;
                                        itembr.Room = SessionHelper.RoomID;
                                        if (!String.IsNullOrWhiteSpace(itembr.ValidStartDate))
                                        {
                                            //DateTime.ParseExact(objDTO.RequiredDateStr, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult)
                                            DateTime ValidStartDate = DateTime.MinValue;
                                            if (DateTime.TryParseExact(itembr.ValidStartDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult, DateTimeStyles.None, out ValidStartDate))
                                            {
                                                if (ValidStartDate != DateTime.MinValue)
                                                {
                                                    itembr.StartDate = ValidStartDate;
                                                }
                                                else
                                                {
                                                    itembr.StartDate = null;
                                                }
                                            }
                                        }
                                        if (!String.IsNullOrWhiteSpace(itembr.ValidEndDate))
                                        {
                                            //DateTime.ParseExact(objDTO.RequiredDateStr, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult)
                                            DateTime ValidEndDate = DateTime.MinValue;
                                            if (DateTime.TryParseExact(itembr.ValidEndDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult, DateTimeStyles.None, out ValidEndDate))
                                            {
                                                if (ValidEndDate != DateTime.MinValue)
                                                {
                                                    itembr.Enddate = ValidEndDate;
                                                }
                                                else
                                                {
                                                    itembr.Enddate = null;
                                                }
                                            }
                                        }

                                        objSupplierBlanketPODetailsDAL.Edit(itembr);
                                    }
                                    else
                                    {
                                        if (itembr.BlanketPO != null)
                                            itembr.BlanketPO = itembr.BlanketPO.Length > 22 ? itembr.BlanketPO.Substring(0, 22) : itembr.BlanketPO;
                                        itembr.AddedFrom = "Web";
                                        itembr.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                        itembr.EditedFrom = "Web";
                                        itembr.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        itembr.Created = DateTime.UtcNow;
                                        itembr.LastUpdatedBy = SessionHelper.UserID;
                                        itembr.Updated = DateTime.UtcNow;
                                        itembr.CreatedBy = SessionHelper.UserID;
                                        itembr.SupplierID = objDTO.ID;
                                        itembr.CompanyID = SessionHelper.CompanyID;
                                        itembr.Room = SessionHelper.RoomID;
                                        if (!String.IsNullOrWhiteSpace(itembr.ValidStartDate))
                                        {
                                            //DateTime.ParseExact(objDTO.RequiredDateStr, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult)
                                            DateTime ValidStartDate = DateTime.MinValue;
                                            if (DateTime.TryParseExact(itembr.ValidStartDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult, DateTimeStyles.None, out ValidStartDate))
                                            {
                                                if (ValidStartDate != DateTime.MinValue)
                                                {
                                                    itembr.StartDate = ValidStartDate;
                                                }
                                                else
                                                {
                                                    itembr.StartDate = null;
                                                }
                                            }
                                        }
                                        if (!String.IsNullOrWhiteSpace(itembr.ValidEndDate))
                                        {
                                            //DateTime.ParseExact(objDTO.RequiredDateStr, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult)
                                            DateTime ValidEndDate = DateTime.MinValue;
                                            if (DateTime.TryParseExact(itembr.ValidEndDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult, DateTimeStyles.None, out ValidEndDate))
                                            {
                                                if (ValidEndDate != DateTime.MinValue)
                                                {
                                                    itembr.Enddate = ValidEndDate;
                                                }
                                                else
                                                {
                                                    itembr.Enddate = null;
                                                }
                                            }
                                        }
                                        objSupplierBlanketPODetailsDAL.Insert(itembr);
                                    }
                                }
                            }
                            //
                            ///Save itemlocationwise quantity to database from the session
                            List<SupplierAccountDetailsDTO> lstSuppAccount = null;
                            if (objDTO != null && objDTO.SupplierAccountDetails != null && objDTO.SupplierAccountDetails.Count > 0 && objDTO.SupplierAccountDirty == 1)
                            {
                                //lstSuppAccount = ((List<SupplierAccountDetailsDTO>)Session["SupplierAccount"]).Where(t => !string.IsNullOrEmpty(t.AccountNo)).ToList();
                                lstSuppAccount = objDTO.SupplierAccountDetails.Where(t => (t.IsDeleted == false && t.ID == 0) || (t.ID > 0)).ToList();
                                foreach (var itembr in lstSuppAccount)
                                {
                                    SupplierAccountDetailsDAL objSAD = new SupplierAccountDetailsDAL(SessionHelper.EnterPriseDBName);
                                    if (itembr.ID > 0)
                                    {

                                        itembr.Created = DateTime.UtcNow;
                                        itembr.LastUpdatedBy = SessionHelper.UserID;
                                        itembr.Updated = DateTime.UtcNow;
                                        itembr.CreatedBy = SessionHelper.UserID;
                                        itembr.SupplierID = objDTO.ID;
                                        itembr.CompanyID = SessionHelper.CompanyID;
                                        itembr.Room = SessionHelper.RoomID;
                                        objSAD.Edit(itembr);
                                    }
                                    else
                                    {

                                        itembr.Created = DateTime.UtcNow;
                                        itembr.LastUpdatedBy = SessionHelper.UserID;
                                        itembr.Updated = DateTime.UtcNow;
                                        itembr.CreatedBy = SessionHelper.UserID;
                                        itembr.SupplierID = objDTO.ID;
                                        itembr.CompanyID = SessionHelper.CompanyID;
                                        itembr.Room = SessionHelper.RoomID;
                                        objSAD.Insert(itembr);
                                    }
                                }

                                Session["SupplierAccount"] = null;
                            }


                            message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                            status = "ok";
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                            status = "fail";
                        }
                    }
                }
                SupplierId = objDTO.ID;
                #endregion
            }

            return Json(new { Message = message, Status = status, NewIDForPopUp = _NewIDForPopUp, SupplierId = SupplierId }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>
        public ActionResult SupplierEdit(Int64 ID)
        {
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            if (IsDeleted || IsArchived)
            {
                ViewBag.ViewOnly = true;
            }
            //SupplierMasterController obj = new SupplierMasterController();
            SupplierMasterDAL obj = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            SupplierMasterDTO objDTO = obj.GetRecord(ID, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, null);
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("SupplierMaster");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;

            //SupplierBlanketPODetailsDAL objSupplierBlanketPODetailsDAL = new SupplierBlanketPODetailsDAL(SessionHelper.EnterPriseDBName);
            //Session["SupplierBlanketPO"] = objSupplierBlanketPODetailsDAL.GetAllBlanketPOBySupplierID(objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();

            //SupplierAccountDetailsDAL objSupplierAccountDetailsDAL = new SupplierAccountDetailsDAL(SessionHelper.EnterPriseDBName);
            //Session["SupplierAccount"] = objSupplierAccountDetailsDAL.GetAllAccountsBySupplierID(objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            //if (!string.IsNullOrEmpty(Convert.ToString(objDTO.MaximumOrderSize)))
            //    objDTO.MaximumOrderSize = objDTO.MaximumOrderSize;

            decimal maxos = 0;
            if (decimal.TryParse(Convert.ToString(objDTO.MaximumOrderSize), out maxos))
            {
                objDTO.MaxOrderSize = Convert.ToInt32(maxos);
            }
            objDTO.IsOnlyFromItemUI = true;
            objDTO.ImageType = (string.IsNullOrEmpty(objDTO.ImageType) ? "ImagePath" : objDTO.ImageType);
            objDTO.CreatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
            objDTO.UpdatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.LastUpdated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
            return PartialView("_CreateSupplier", objDTO);
        }

        /// <summary>
        /// Update Supplier 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <param name="rowId"></param>
        /// <param name="columnPosition"></param>
        /// <param name="columnId"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string UpdateSupplierData(Int64 id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            //SupplierMasterController obj = new SupplierMasterController();
            SupplierMasterDAL obj = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            obj.UpdateData(id, value, rowId, columnPosition, columnId, columnName);
            return value;
        }

        ///// <summary>
        ///// Duplicate Supplier Check
        ///// </summary>
        ///// <param name="SupplierName"></param>
        ///// <param name="ActionMode"></param>
        ///// <param name="ID"></param>
        ///// <returns></returns>
        //public string DuplicateSupplierCheck(string SupplierName, string ActionMode, int ID)
        //{
        //    SupplierMasterController obj = new SupplierMasterController();
        //    return obj.DuplicateCheck(SupplierName, ActionMode, ID);
        //}

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public JsonResult DeleteSupplierRecords(string ids)
        {
            try
            {
                //eTurns.DAL.CommonDAL _repository = new eTurns.DAL.CommonDAL(SessionHelper.EnterPriseDBName);
                //return _repository.DeleteRecords(ImportMastersDTO.TableName.SupplierMaster.ToString(), ids, SessionHelper.RoomID, SessionHelper.CompanyID);



                string response = string.Empty;
                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                response = objCommonDAL.DeleteModulewise(ids, ImportMastersDTO.TableName.SupplierMaster.ToString(), false, SessionHelper.UserID);
                eTurns.DAL.CacheHelper<IEnumerable<SupplierMasterDTO>>.InvalidateCache();
                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        #region Ajax Data Provider

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult GetSupplierList(JQueryDataTableParamModel param)
        {
            //SupplierMasterController obj = new SupplierMasterController();
            SupplierMasterDAL obj = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            bool IsForBom = false;
            bool.TryParse(Convert.ToString(Request["IsForBom"]), out IsForBom);
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

            if (sortColumnName == "0" || sortColumnName == "undefined" || string.IsNullOrEmpty(sortColumnName))
                sortColumnName = "ID Desc";
            // set the default column sorting here, if first time then required to set 
            //if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID";

            //if (sortDirection == "asc")
            //    sortColumnName = sortColumnName + " asc";
            //else
            //    sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            //var DataFromDB = obj.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, IsForBom);
            IEnumerable<SupplierMasterDTO> DataFromDB;

            DataFromDB = obj.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, IsForBom);
            if (SessionHelper.UserSupplierID > 0)
            {
                DataFromDB = DataFromDB.Where(s => s.ID == SessionHelper.UserSupplierID);
                TotalRecordCount = DataFromDB.Count();
                //Below code when multiple Supplier will allow to user

                //long  UserSupplierID = SessionHelper.UserSupplierID;

                //string[] UserSupplierString;

                //UserSupplierString = Convert.ToString(UserSupplierID).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                //if (UserSupplierString != null)
                //{

                //    foreach (var item in UserSupplierString)
                //    {
                //        long tempid = 0;

                //        if (long.TryParse(item, out tempid))
                //        {
                //            DataFromDB = DataFromDB.Where(s => s.ID == tempid);
                //        }
                //    }
                //    //UserSuppliers = (UserSupplierString.Select(t => long.Parse(t)).ToList());
                //}
            }
            //DataFromDB = obj.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, IsForBom);


            var result = from u in DataFromDB
                         select new SupplierMasterDTO
                         {

                             ID = u.ID,
                             SupplierName = u.SupplierName,
                             Description = u.Description,
                             //AccountNo = u.AccountNo,
                             ReceiverID = u.ReceiverID,
                             Address = u.Address,
                             City = u.City,
                             State = u.State,
                             ZipCode = u.ZipCode,
                             Country = u.Country,
                             Contact = u.Contact,
                             Phone = u.Phone,
                             Fax = u.Fax,
                             Email = u.Email,
                             IsEmailPOInBody = u.IsEmailPOInBody,
                             IsEmailPOInPDF = u.IsEmailPOInPDF,
                             IsEmailPOInCSV = u.IsEmailPOInCSV,
                             IsEmailPOInX12 = u.IsEmailPOInX12,
                             Created = u.Created,
                             LastUpdated = u.LastUpdated,
                             CreatedBy = u.CreatedBy,
                             LastUpdatedBy = u.LastUpdatedBy,
                             Room = u.Room,
                             GUID = u.GUID,
                             IsDeleted = u.IsDeleted,
                             IsArchived = u.IsArchived,
                             CreatedByName = u.CreatedByName,
                             UpdatedByName = u.UpdatedByName,
                             RoomName = u.RoomName,
                             UDF1 = u.UDF1,
                             UDF2 = u.UDF2,
                             UDF3 = u.UDF3,
                             UDF4 = u.UDF4,
                             UDF5 = u.UDF5,
                             CreatedDate = CommonUtility.ConvertDateByTimeZone(u.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             UpdatedDate = CommonUtility.ConvertDateByTimeZone(u.LastUpdated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             ReceivedOn = u.ReceivedOn,
                             ReceivedOnWeb = u.ReceivedOnWeb,
                             AddedFrom = u.AddedFrom,
                             EditedFrom = u.EditedFrom,
                             SupplierImage = u.SupplierImage,
                             ImageType = u.ImageType,
                             ImageExternalURL = u.ImageExternalURL
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

        public ActionResult SupplierBlanketAjax(JQueryDataTableParamModel param)
        {
            //SupplierMasterController obj = new SupplierMasterController();
            SupplierMasterDAL obj = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            long SupplierID = 0;
            long.TryParse(Convert.ToString(Request["SupplierID"]), out SupplierID);
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

            if (sortColumnName == "0" || sortColumnName == "undefined" || string.IsNullOrEmpty(sortColumnName))
                sortColumnName = "ID Desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            SupplierBlanketPODetailsDAL objSupplierBlanketPODetailsDAL = new SupplierBlanketPODetailsDAL(SessionHelper.EnterPriseDBName);
            List<SupplierBlanketPODetailsDTO> lstItemSupplier = objSupplierBlanketPODetailsDAL.GetAllBlanketPOBySupplierID(SupplierID, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();


            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = lstItemSupplier
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// // below function used to load the User wise saved filter data for jquery data table
        /// </summary>
        /// <param name="UserID"></param> will have userid for currently logged in user
        /// <returns></returns>
        public ActionResult LoadSupplierGridState(int UserID, string ListName)
        {
            //string jsonData = @"{""iCreate"":1350639486123,""iStart"":0,""iEnd"":0,""iLength"":10,""aaSorting"":[[0,""asc"",0,""Name""]],""oSearch"":{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true},""aoSearchCols"":[{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true},{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true},{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true},{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}],""abVisCols"":[false,true,true,true,true],""ColReorder"":[0,1,3,2,4]}";
            //UsersUISettingsController obj = new UsersUISettingsController();
            eTurnsMaster.DAL.UsersUISettingsDAL obj = new eTurnsMaster.DAL.UsersUISettingsDAL(SessionHelper.EnterPriseDBName);
            UsersUISettingsDTO objDTO = new UsersUISettingsDTO();
            objDTO = obj.GetRecord(UserID, ListName);
            string jsonData = objDTO.JSONDATA;
            return Json(new { jsonData }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// // below function used to Save the User wise filter data for jquery data table
        /// </summary>
        /// <param name="UserID"></param> will have userid for currently logged in user
        /// <param name="Data"></param> will have json string of filter criteria 
        /// <returns></returns>
        public ActionResult SaveSupplierGridState(int UserID, string Data, string ListName)
        {
            //UsersUISettingsController obj = new UsersUISettingsController();
            eTurnsMaster.DAL.UsersUISettingsDAL obj = new eTurnsMaster.DAL.UsersUISettingsDAL(SessionHelper.EnterPriseDBName);
            UsersUISettingsDTO objDTO = new UsersUISettingsDTO();
            objDTO.UserID = UserID;
            objDTO.JSONDATA = Data;
            objDTO.ListName = ListName;
            obj.SaveUserListViewSettings(objDTO);

            return Json(new { objDTO.JSONDATA }, JsonRequestBehavior.AllowGet);

        }


        public JsonResult GetSupplier(int maxRows, string name_startsWith)
        {
            SupplierMasterDAL obj = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            List<SupplierMasterDTO> lstUnit = new List<SupplierMasterDTO>();

            if (SessionHelper.UserSupplierID > 0)
                lstUnit = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false).Where(t => t.SupplierName.ToLower().Contains(name_startsWith.ToLower().Trim()) && t.ID == SessionHelper.UserSupplierID).Take(maxRows).ToList();
            else
                lstUnit = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false).Where(t => t.SupplierName.ToLower().Contains(name_startsWith.ToLower().Trim())).Take(maxRows).ToList();
            lstUnit = lstUnit.OrderBy(m => m.SupplierName.Trim()).ToList();
            if (lstUnit.Count == 0)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            return Json(lstUnit, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetManufacturer(int maxRows, string name_startsWith)
        {
            ManufacturerMasterDAL obj = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName);
            List<ManufacturerMasterDTO> lstUnit = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false).Where(t => t.Manufacturer != null).Where(t => t.Manufacturer.ToLower().Contains(name_startsWith.ToLower().Trim())).Take(maxRows).ToList();
            //  lstUnit = lstUnit.Where(b => b.Manufacturer != null).ToList();
            lstUnit = lstUnit.OrderBy(m => m.Manufacturer.Trim()).ToList();
            if (lstUnit.Count == 0)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            return Json(lstUnit, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBinLocation(int maxRows, string name_startsWith)
        {
            BinMasterDAL obj = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            List<BinMasterDTO> lstUnit;
            //List<BinMasterDTO> lstUnit = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(t => t.BinNumber.ToLower().Contains(name_startsWith.ToLower().Trim())).Take(maxRows).ToList();
            if (name_startsWith.Trim().Count() > 0)
            {
                lstUnit = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(t => t.IsStagingLocation == false && t.BinNumber.ToLower().Contains(name_startsWith.ToLower().Trim())).ToList();
            }
            else
            {
                lstUnit = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(t => t.IsStagingLocation == false).ToList();
            }
            if (lstUnit.Count == 0)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            return Json(lstUnit, JsonRequestBehavior.AllowGet);
        }


        #endregion


        #region "BlanketPO"

        /// <summary>
        /// Load BinReplanish
        /// </summary>
        /// <param name="ItemGUID"></param>
        /// <returns></returns>
        public ActionResult LoadBlanketPOofSupplier(Int64 SupplierID, int? AddCount)
        {
            ViewBag.SupplierID = SupplierID;
            List<SupplierBlanketPODetailsDTO> lstItemSupplier = null;
            if (Session["SupplierBlanketPO"] != null)
            {
                lstItemSupplier = (List<SupplierBlanketPODetailsDTO>)Session["SupplierBlanketPO"];

                //Delete blank rows
                //lstItemSupplier.Remove(lstItemSupplier.Where(t => t.GUID == Guid.Empty && t.SupplierID == 0).FirstOrDefault());
            }
            else
            {
                lstItemSupplier = new List<SupplierBlanketPODetailsDTO>();
            }

            //if (lstBinReplanish.Count == 0 && AddCount ==0)
            //{
            //    AddCount = 1;
            //}

            if (AddCount != null && AddCount > 0)
            {
                for (int i = 0; i < AddCount; i++)
                {
                    lstItemSupplier.Add(new SupplierBlanketPODetailsDTO() { ID = 0, IsDeleted = false, SessionSr = lstItemSupplier.Count + 1, SupplierID = SupplierID, Room = SessionHelper.RoomID, CompanyID = SessionHelper.CompanyID, Updated = DateTimeUtility.DateTimeNow, LastUpdatedBy = SessionHelper.UserID, Created = DateTimeUtility.DateTimeNow, CreatedBy = SessionHelper.UserID, GUID = new Guid(), PulledQty = 0, OrderedQty = 0 });
                }
            }
            return PartialView("_SupplierBlanketPO", lstItemSupplier.OrderByDescending(t => t.ID).ToList());
            //return PartialView("_SupplierBlanketPODetails", lstItemSupplier.Where(m => m.IsDeleted == false).OrderByDescending(t => t.ID).ToList());
        }

        //SupplierBlanketDelete
        //data: { 'ID': vsuphdnID, 'SessionSr': vsuphdnSessionSr, 'GUID': vsuphdnGUID, 'SupplierID': vhdnSupplierID, 'BlanketPO': vtxtBlanketPO, 'StartDate': vtxtStartDate, 'Enddate': vtxtEnddate },
        [HttpPost]
        public JsonResult SavetoSeesionSupplierBlanketPO(Int64 ID, Int32 SessionSr, Guid GUID, Int64 SupplierID, string BlanketPO, string StartDate, string Enddate, float? MaxLimit, bool IsNotExceed)
        {

            List<SupplierBlanketPODetailsDTO> lstBinReplanish = null;
            if (Session["SupplierBlanketPO"] != null)
            {
                lstBinReplanish = (List<SupplierBlanketPODetailsDTO>)Session["SupplierBlanketPO"];
            }
            else
            {
                lstBinReplanish = new List<SupplierBlanketPODetailsDTO>();
            }


            if (ID > 0 && SessionSr == 0)
            {
                SupplierBlanketPODetailsDTO objDTO = lstBinReplanish.Where(t => t.ID == ID).FirstOrDefault();
                if (objDTO != null)
                {
                    lstBinReplanish.Remove(lstBinReplanish.Where(t => t.ID == ID).FirstOrDefault());
                    objDTO.ID = ID;
                    objDTO.SupplierID = SupplierID;
                    objDTO.BlanketPO = BlanketPO;
                    if (!string.IsNullOrEmpty(StartDate))
                        objDTO.StartDate = DateTime.ParseExact(StartDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult);
                    if (!string.IsNullOrEmpty(Enddate))
                        objDTO.Enddate = DateTime.ParseExact(Enddate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult);
                    objDTO.MaxLimit = MaxLimit;
                    objDTO.IsNotExceed = IsNotExceed;
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
                    SupplierBlanketPODetailsDTO objDTO = lstBinReplanish.Where(t => t.SessionSr == SessionSr).FirstOrDefault();
                    if (objDTO != null)
                    {
                        lstBinReplanish.Remove(lstBinReplanish.Where(t => t.SessionSr == SessionSr).FirstOrDefault());
                        objDTO.ID = ID;
                        objDTO.SupplierID = SupplierID;
                        objDTO.BlanketPO = BlanketPO;
                        if (!string.IsNullOrEmpty(StartDate))
                            objDTO.StartDate = DateTime.ParseExact(StartDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult);
                        if (!string.IsNullOrEmpty(Enddate))
                            objDTO.Enddate = DateTime.ParseExact(Enddate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult);
                        objDTO.MaxLimit = MaxLimit;
                        objDTO.IsNotExceed = IsNotExceed;
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
                        objDTO = new SupplierBlanketPODetailsDTO();
                        objDTO.ID = 0;

                        objDTO.SupplierID = SupplierID;
                        objDTO.BlanketPO = BlanketPO;
                        if (!string.IsNullOrEmpty(StartDate))
                            objDTO.StartDate = DateTime.ParseExact(StartDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult);
                        if (!string.IsNullOrEmpty(Enddate))
                            objDTO.Enddate = DateTime.ParseExact(Enddate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult);
                        objDTO.MaxLimit = MaxLimit;
                        objDTO.IsNotExceed = IsNotExceed;
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
                    SupplierBlanketPODetailsDTO objDTO = new SupplierBlanketPODetailsDTO();
                    objDTO.ID = 0;
                    objDTO.SupplierID = SupplierID;
                    objDTO.BlanketPO = BlanketPO;
                    if (!string.IsNullOrEmpty(StartDate))
                        objDTO.StartDate = DateTime.ParseExact(StartDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult);
                    if (!string.IsNullOrEmpty(Enddate))
                        objDTO.Enddate = DateTime.ParseExact(Enddate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult);
                    objDTO.MaxLimit = MaxLimit;
                    objDTO.IsNotExceed = IsNotExceed;
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
            Session["SupplierBlanketPO"] = lstBinReplanish;

            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeletetoSeesionItemSupplierSingle(Int64 ID, Int32 SessionSr, Guid GUID, Int64 SupplierID, string BlanketPO, DateTime? StartDate, DateTime? Enddate)
        {
            List<SupplierBlanketPODetailsDTO> lstBinReplanish = null;
            //List<ItemSupplierDetailsDTO> lstItemSupp = null;
            if (Session["SupplierBlanketPO"] != null)
            {
                lstBinReplanish = (List<SupplierBlanketPODetailsDTO>)Session["SupplierBlanketPO"];
            }
            else
            {
                lstBinReplanish = new List<SupplierBlanketPODetailsDTO>();
            }

            ///Delete the Records......
            if (ID > 0)
            {
                try
                {
                    //lstBinReplanish.Remove(lstBinReplanish.Where(t => t.ID == ID).FirstOrDefault());
                    lstBinReplanish.Where(t => t.ID == ID).FirstOrDefault().IsDeleted = true;
                    Session["SupplierBlanketPO"] = lstBinReplanish;

                    //Update Ref of ItemSupplierDetails WI-801
                    //ItemSupplierDetailsDAL objItemSuppDAL = new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName);
                    //bool result = false;
                    //result = objItemSuppDAL.UpdateItemSuppForBlanketPO(ID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    //if (!result)
                    //    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);

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
                    //Int64 BlanketPOID = 0;
                    //BlanketPOID = lstBinReplanish.Where(t => t.GUID == GUID).FirstOrDefault().ID;

                    lstBinReplanish.Remove(lstBinReplanish.Where(t => t.GUID == GUID).FirstOrDefault());
                    Session["SupplierBlanketPO"] = lstBinReplanish;

                    //Update Ref of ItemSupplierDetails WI-801
                    //if (BlanketPOID > 0)
                    //{
                    //    ItemSupplierDetailsDAL objItemSuppDAL = new ItemSupplierDetailsDAL(SessionHelper.EnterPriseDBName);
                    //    bool result = false;
                    //    result = objItemSuppDAL.UpdateItemSuppForBlanketPO(BlanketPOID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    //    if (!result)
                    //        return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                    //}
                }
                catch
                {
                    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "deleted" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region "Accounts"

        /// <summary>
        /// Load BinReplanish
        /// </summary>
        /// <param name="ItemGUID"></param>
        /// <returns></returns>
        public ActionResult LoadAccountofSupplier(Int64 SupplierID, int? AddCount)
        {
            List<SupplierAccountDetailsDTO> lstItemSupplier = null;
            if (Session["SupplierAccount"] != null)
            {
                lstItemSupplier = (List<SupplierAccountDetailsDTO>)Session["SupplierAccount"];

                //Delete blank rows
                //lstItemSupplier.Remove(lstItemSupplier.Where(t => t.GUID == Guid.Empty && t.SupplierID == 0).FirstOrDefault());
            }
            else
            {
                lstItemSupplier = new List<SupplierAccountDetailsDTO>();
            }

            //if (lstBinReplanish.Count == 0 && AddCount ==0)
            //{
            //    AddCount = 1;
            //}

            if (AddCount != null && AddCount > 0)
            {
                for (int i = 0; i < AddCount; i++)
                {
                    lstItemSupplier.Add(new SupplierAccountDetailsDTO() { ID = 0, SessionSr = lstItemSupplier.Count + 1, SupplierID = SupplierID, Room = SessionHelper.RoomID, CompanyID = SessionHelper.CompanyID, Updated = DateTimeUtility.DateTimeNow, LastUpdatedBy = SessionHelper.UserID, Created = DateTimeUtility.DateTimeNow, CreatedBy = SessionHelper.UserID, GUID = new Guid() });
                }
            }

            return PartialView("_SupplierAccounts", lstItemSupplier.OrderByDescending(t => t.ID).ToList());
        }

        //SupplierBlanketDelete
        //data: { 'ID': vsuphdnID, 'SessionSr': vsuphdnSessionSr, 'GUID': vsuphdnGUID, 'SupplierID': vhdnSupplierID, 'BlanketPO': vtxtBlanketPO, 'StartDate': vtxtStartDate, 'Enddate': vtxtEnddate },
        [HttpPost]
        public JsonResult SavetoSeesionSupplierAccount(Int64 ID, Int32 SessionSr, Guid GUID, Int64 SupplierID, string AccountNo, string AccountName)
        {
            List<SupplierAccountDetailsDTO> lstBinReplanish = null;
            if (Session["SupplierAccount"] != null)
            {
                lstBinReplanish = (List<SupplierAccountDetailsDTO>)Session["SupplierAccount"];
            }
            else
            {
                lstBinReplanish = new List<SupplierAccountDetailsDTO>();
            }


            if (ID > 0 && SessionSr == 0)
            {
                SupplierAccountDetailsDTO objDTO = lstBinReplanish.Where(t => t.ID == ID).FirstOrDefault();
                if (objDTO != null)
                {
                    lstBinReplanish.Remove(lstBinReplanish.Where(t => t.ID == ID).FirstOrDefault());
                    objDTO.ID = ID;
                    objDTO.SupplierID = SupplierID;
                    objDTO.AccountNo = AccountNo;
                    objDTO.AccountName = AccountName;
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
                    SupplierAccountDetailsDTO objDTO = lstBinReplanish.Where(t => t.SessionSr == SessionSr).FirstOrDefault();
                    if (objDTO != null)
                    {
                        lstBinReplanish.Remove(lstBinReplanish.Where(t => t.SessionSr == SessionSr).FirstOrDefault());
                        objDTO.ID = ID;
                        objDTO.SupplierID = SupplierID;
                        objDTO.AccountNo = AccountNo;
                        objDTO.AccountName = AccountName;
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
                        objDTO = new SupplierAccountDetailsDTO();
                        objDTO.ID = 0;

                        objDTO.SupplierID = SupplierID;
                        objDTO.AccountNo = AccountNo;
                        objDTO.AccountName = AccountName;
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
                    SupplierAccountDetailsDTO objDTO = new SupplierAccountDetailsDTO();
                    objDTO.ID = 0;
                    objDTO.SupplierID = SupplierID;
                    objDTO.AccountNo = AccountNo;
                    objDTO.AccountName = AccountName;
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
            Session["SupplierAccount"] = lstBinReplanish;

            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeletetoSeesionSupplierAccountSingle(Int64 ID, Int32 SessionSr, Guid GUID, Int64 SupplierID, string AccountNo)
        {
            List<SupplierAccountDetailsDTO> lstBinReplanish = null;
            if (Session["SupplierAccount"] != null)
            {
                lstBinReplanish = (List<SupplierAccountDetailsDTO>)Session["SupplierAccount"];
            }
            else
            {
                lstBinReplanish = new List<SupplierAccountDetailsDTO>();
            }

            ///Delete the Records......
            if (ID > 0)
            {
                try
                {
                    lstBinReplanish.Remove(lstBinReplanish.Where(t => t.ID == ID).FirstOrDefault());
                    Session["SupplierAccount"] = lstBinReplanish;
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
                    Session["SupplierAccount"] = lstBinReplanish;
                }
                catch
                {
                    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "deleted" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #endregion

        #region [SupplierBlanketPOMaster]

        public ActionResult SupplierBlanketPOList()
        {
            return View();

        }

        public PartialViewResult _CreateSupplierBlanketPO()
        {
            return PartialView();
        }



        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult SupplierBlanketPOCreate()
        {
            TechnicianMasterDTO objDTO = new TechnicianMasterDTO();
            objDTO.ID = 0;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.RoomName = SessionHelper.RoomName;
            objDTO.GUID = Guid.NewGuid();
            objDTO.CreatedByName = SessionHelper.UserName;
            objDTO.UpdatedByName = SessionHelper.UserName;
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("SupplierBlankePOMaster");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            return PartialView("_CreateTechnician", objDTO);
            //return View();
        }

        /// <summary>
        /// JSON Record Save - Enter key Save/Update
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Technician"></param>
        /// <returns></returns>
        public JsonResult SupplierBlanketPOSave(Int64 ID, string PO)
        {
            string message = "";
            SupplierBlanketPOMasterDTO objDTO = new SupplierBlanketPOMasterDTO();
            //SupplierBlanketPOMasterController obj = new SupplierBlanketPOMasterController();
            SupplierBlanketPOMasterDAL obj = new SupplierBlanketPOMasterDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            objDTO.ID = ID;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.RoomName = SessionHelper.RoomName;
            objDTO.BlanketPO = PO;
            objDTO.HighPO = 100;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;
            if (ID == 0)
            {
                objDTO.CreatedBy = SessionHelper.UserID;

                string strOK = objCDAL.DuplicateCheck(PO, "add", ID, "SupplierBlanketPOMaster", "BlanketPO", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK != "ok")
                {
                    message = "BlanketPO \"" + PO + "\" already exist! Try with Another!";
                }
                else
                {
                    objDTO.GUID = Guid.NewGuid();
                    long ReturnVal = obj.Insert(objDTO);
                    if (ReturnVal > 0)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                    }
                }
            }
            else
            {
                objDTO.LastUpdatedBy = SessionHelper.UserID;
                string strOK = objCDAL.DuplicateCheck(PO, "edit", ID, "SupplierBlanketPOMaster", "BlanketPO", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK != "ok")
                {
                    message = "Technician \"" + PO + "\" already exist! Try with Another!";
                }
                else
                {
                    bool ReturnVal = obj.Edit(objDTO);
                    if (ReturnVal)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                    }
                }
            }


            return Json(new { Message = message }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult SupplierBlanketPOCreate(SupplierBlanketPOMasterDTO objDTO)
        {
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = SessionHelper.UserID;
            //SupplierBlanketPOMasterController obj = new SupplierBlanketPOMasterController();
            SupplierBlanketPOMasterDAL obj = new SupplierBlanketPOMasterDAL(SessionHelper.EnterPriseDBName);
            long ReturnVal = obj.Insert(objDTO);

            if (ReturnVal > 0)
            {
                ViewBag.GlobalMessage = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
            }
            else
            {
                ViewBag.GlobalMessage = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
            }

            return View();
        }

        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>
        public ActionResult SupplierBlanketPOEdit(Int64 ID)
        {
            //SupplierBlanketPOMasterController obj = new SupplierBlanketPOMasterController();
            SupplierBlanketPOMasterDAL obj = new SupplierBlanketPOMasterDAL(SessionHelper.EnterPriseDBName);
            SupplierBlanketPOMasterDTO objDTO = obj.GetRecord(ID, SessionHelper.RoomID, SessionHelper.CompanyID);
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("SupplierBlankePOMaster");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;
            return PartialView("_CreateSupplierBlanketPO", objDTO);
        }

        [HttpPost]
        public ActionResult SupplierBlanketPOEdit(Int64 ID, SupplierBlanketPOMasterDTO objDTO)
        {
            //SupplierBlanketPOMasterController obj = new SupplierBlanketPOMasterController();
            SupplierBlanketPOMasterDAL obj = new SupplierBlanketPOMasterDAL(SessionHelper.EnterPriseDBName);
            obj.Edit(objDTO);

            return RedirectToAction("SupplierBlanketPOList");
        }


        #region Ajax Data Provider

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult SupplierBlanketPOListAjax(JQueryDataTableParamModel param)
        {
            //SupplierBlanketPOMasterController obj = new SupplierBlanketPOMasterController();
            SupplierBlanketPOMasterDAL obj = new SupplierBlanketPOMasterDAL(SessionHelper.EnterPriseDBName);

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
            if (param.sSearch != null && param.sSearch != "")
            {
                searchQuery = "WHERE BlanketPO like '%" + param.sSearch + "%'" + @"
                    OR RoomName like '%" + param.sSearch + "%'" + @" 
                    OR CreatedBy like '%" + param.sSearch + "%'";
            }

            int TotalRecordCount = 0;
            var DataFromDB = obj.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted);

            var result = from u in DataFromDB
                         select new SupplierBlanketPOMasterDTO
                         {
                             BlanketPO = u.BlanketPO,
                             SupplierID = u.SupplierID,
                             IsArchived = u.IsArchived,
                             GUID = u.GUID,
                             HighPO = u.HighPO,
                             IsDeleted = u.IsDeleted,
                             Created = u.Created,
                             CreatedBy = u.CreatedBy,
                             ID = u.ID,
                             LastUpdatedBy = u.LastUpdatedBy,
                             Room = u.Room,
                             LastUpdated = u.LastUpdated,
                             UpdatedByName = u.UpdatedByName,
                             CreatedByName = u.CreatedByName,
                             RoomName = u.RoomName,
                             UDF1 = u.UDF1,
                             UDF2 = u.UDF2,
                             UDF3 = u.UDF3,
                             UDF4 = u.UDF4,
                             UDF5 = u.UDF5
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

        public string UpdateSupplierBlanketPOData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            //SupplierBlanketPOMasterController obj = new SupplierBlanketPOMasterController();
            SupplierBlanketPOMasterDAL obj = new SupplierBlanketPOMasterDAL(SessionHelper.EnterPriseDBName);
            obj.UpdateData(id, value, rowId, columnPosition, columnId, columnName);
            return value;
        }

        public string DuplicateSupplierBlanketPOCheck(string TechnicianName, string ActionMode, int ID)
        {
            //SupplierBlanketPOMasterController obj = new SupplierBlanketPOMasterController();            
            CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            return objCDAL.DuplicateCheck(TechnicianName, ActionMode, ID, "SupplierBlanketPOMaster", "BlanketPO", SessionHelper.RoomID, SessionHelper.CompanyID);
        }

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public string DeleteSupplierBlanketPORecords(string ids)
        {
            try
            {
                string deleteIDs = "";
                if (ids.LastIndexOf(",") != -1)
                {
                    foreach (var item in ids.Split(','))
                    {
                        if (!String.IsNullOrEmpty(item))
                        {
                            deleteIDs += "'" + item + "',";
                        }
                    }
                    deleteIDs = deleteIDs.Substring(0, deleteIDs.LastIndexOf(","));
                }

                //SupplierBlanketPOMasterController obj = new SupplierBlanketPOMasterController();
                SupplierBlanketPOMasterDAL obj = new SupplierBlanketPOMasterDAL(SessionHelper.EnterPriseDBName);
                obj.DeleteRecords(deleteIDs, SessionHelper.UserID, SessionHelper.CompanyID);
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

        #region Private Methods

        private SchedulerDTO ConvertFromPullSchedule(PullSchedulerDTO objPullSchedulerDTO)
        {
            SchedulerDTO objSchedulerDTO = new SchedulerDTO();
            if (objPullSchedulerDTO != null)
            {
                objSchedulerDTO.ScheduleID = objPullSchedulerDTO.Pull_ScheduleID;
                objSchedulerDTO.ScheduleMode = objPullSchedulerDTO.Pull_ScheduleMode;
                objSchedulerDTO.DailyRecurringType = objPullSchedulerDTO.Pull_DailyRecurringType;
                objSchedulerDTO.DailyRecurringDays = objPullSchedulerDTO.Pull_DailyRecurringDays;
                objSchedulerDTO.WeeklyRecurringWeeks = objPullSchedulerDTO.Pull_WeeklyRecurringWeeks;
                objSchedulerDTO.HourlyRecurringHours = objPullSchedulerDTO.Pull_HourlyRecurringHours;
                objSchedulerDTO.HourlyAtWhatMinute = objPullSchedulerDTO.Pull_HourlyAtWhatMinute;
                objSchedulerDTO.WeeklyOnMonday = objPullSchedulerDTO.Pull_WeeklyOnMonday;
                objSchedulerDTO.WeeklyOnTuesday = objPullSchedulerDTO.Pull_WeeklyOnTuesday;
                objSchedulerDTO.WeeklyOnWednesday = objPullSchedulerDTO.Pull_WeeklyOnWednesday;
                objSchedulerDTO.WeeklyOnThursday = objPullSchedulerDTO.Pull_WeeklyOnThursday;
                objSchedulerDTO.WeeklyOnFriday = objPullSchedulerDTO.Pull_WeeklyOnFriday;
                objSchedulerDTO.WeeklyOnSaturday = objPullSchedulerDTO.Pull_WeeklyOnSaturday;
                objSchedulerDTO.WeeklyOnSunday = objPullSchedulerDTO.Pull_WeeklyOnSunday;
                objSchedulerDTO.WeeklySelectedDays = objPullSchedulerDTO.Pull_WeeklySelectedDays;
                objSchedulerDTO.MonthlyRecurringType = objPullSchedulerDTO.Pull_MonthlyRecurringType;
                objSchedulerDTO.MonthlyDateOfMonth = objPullSchedulerDTO.Pull_MonthlyDateOfMonth;
                objSchedulerDTO.MonthlyRecurringMonths = objPullSchedulerDTO.Pull_MonthlyRecurringMonths;
                objSchedulerDTO.MonthlyDayOfMonth = objPullSchedulerDTO.Pull_MonthlyDayOfMonth;
                objSchedulerDTO.SubmissionMethod = objPullSchedulerDTO.Pull_SubmissionMethod;
                objSchedulerDTO.ScheduleRunTime = objPullSchedulerDTO.Pull_ScheduleRunTime;
                objSchedulerDTO.ScheduleRunDateTime = objPullSchedulerDTO.Pull_ScheduleRunDateTime;
                objSchedulerDTO.SupplierId = objPullSchedulerDTO.Pull_SupplierId;
                objSchedulerDTO.SupplierName = objPullSchedulerDTO.Pull_SupplierName;
                objSchedulerDTO.IsScheduleActive = objPullSchedulerDTO.Pull_IsScheduleActive;
                objSchedulerDTO.RoomId = objPullSchedulerDTO.Pull_RoomId;
                objSchedulerDTO.RoomName = objPullSchedulerDTO.Pull_RoomName;
                objSchedulerDTO.LoadSheduleFor = objPullSchedulerDTO.Pull_LoadSheduleFor;
                objSchedulerDTO.CompanyId = objPullSchedulerDTO.Pull_CompanyId;
                objSchedulerDTO.CreatedBy = objPullSchedulerDTO.Pull_CreatedBy;
                objSchedulerDTO.LastUpdatedBy = objPullSchedulerDTO.Pull_LastUpdatedBy;
                objSchedulerDTO.NextRunDate = objPullSchedulerDTO.Pull_NextRunDate;
                objSchedulerDTO.NextRunDateTime = objPullSchedulerDTO.Pull_NextRunDateTime;
                objSchedulerDTO.IsScheduleChanged = objPullSchedulerDTO.Pull_IsScheduleChanged;
                objSchedulerDTO.AssetToolID = objPullSchedulerDTO.Pull_AssetToolID;
                objSchedulerDTO.ReportID = objPullSchedulerDTO.Pull_ReportID;
                objSchedulerDTO.AttachmentTypes = objPullSchedulerDTO.Pull_AttachmentTypes;
                objSchedulerDTO.AttachmentReportIDs = objPullSchedulerDTO.Pull_AttachmentReportIDs;
                objSchedulerDTO.EmailAddress = objPullSchedulerDTO.Pull_EmailAddress;
                objSchedulerDTO.BinNumber = objPullSchedulerDTO.Pull_BinNumber;
                objSchedulerDTO.EmailTemplateDetail = objPullSchedulerDTO.Pull_EmailTemplateDetail;
                objSchedulerDTO.ModuleName = objPullSchedulerDTO.Pull_ModuleName;
                objSchedulerDTO.ReportDataSelectionType = objPullSchedulerDTO.Pull_ReportDataSelectionType;
                objSchedulerDTO.ReportDataSince = objPullSchedulerDTO.Pull_ReportDataSince;
                objSchedulerDTO.ScheduledBy = objPullSchedulerDTO.Pull_ScheduledBy;
                objSchedulerDTO.ScheduleName = objPullSchedulerDTO.Pull_ScheduleName;
                objSchedulerDTO.TotalRecords = objPullSchedulerDTO.Pull_TotalRecords;
                objSchedulerDTO.ReportName = objPullSchedulerDTO.Pull_ReportName;
                objSchedulerDTO.CreatedByName = objPullSchedulerDTO.Pull_CreatedByName;
                objSchedulerDTO.UpdatedByName = objPullSchedulerDTO.Pull_UpdatedByName;
                objSchedulerDTO.Created = objPullSchedulerDTO.Pull_Created;
                objSchedulerDTO.lstTemplateDtls = objPullSchedulerDTO.Pull_lstTemplateDtls;
                objSchedulerDTO.EmailTempateId = objPullSchedulerDTO.Pull_EmailTempateId;
                objSchedulerDTO.MailBodyText = objPullSchedulerDTO.Pull_MailBodyText;
                objSchedulerDTO.MailSubject = objPullSchedulerDTO.Pull_MailSubject;
                objSchedulerDTO.ReportIds = objPullSchedulerDTO.Pull_ReportIds;
                objSchedulerDTO.EmailTemplateIds = objPullSchedulerDTO.Pull_EmailTemplateIds;
                objSchedulerDTO.EmailTemplateDetails = objPullSchedulerDTO.Pull_EmailTemplateDetails;
                objSchedulerDTO.IsDeleted = objPullSchedulerDTO.Pull_IsDeleted;
                objSchedulerDTO.Updated = objPullSchedulerDTO.Pull_Updated;
                objSchedulerDTO.RecalcSchedule = objPullSchedulerDTO.Pull_RecalcSchedule;

            }
            return objSchedulerDTO;
        }

        public string RenderRazorPartialViewToString(string viewName)
        {
            //ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                try
                {
                    var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                    var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                    viewResult.View.Render(viewContext, sw);
                    viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                    return sw.GetStringBuilder().ToString();
                }
                catch { return ""; }
            }
        }

        public void SetSessions(long EnterpriseId, long CompanyId, long RoomId, long UserId, int UserType, long RoleId, string EventFiredOn, string enterpriseName, string CompanyName, string RoomName)
        {
            eTurnsMaster.DAL.EnterpriseMasterDAL objEnterpriseDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
            eTurnsMaster.DAL.UserMasterDAL objUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
            CompanyMasterDAL objCompanyMasterDAL;
            RoomDAL objRoomDAL;
            eTurns.DAL.UserMasterDAL objinterUserDAL;
            EnterpriseDTO objEnterpriseDTO = new EnterpriseDTO();
            CompanyMasterDTO objCompanyMasterDTO = new CompanyMasterDTO();
            RoomDTO objRoomDTO = new RoomDTO();
            switch (EventFiredOn)
            {
                case "onlogin":
                    UserLoginHistoryDTO objUserLoginHistoryDTO = new UserLoginHistoryDTO();
                    objUserLoginHistoryDTO = objUserMasterDAL.GetUserLastActionDetail(UserId);
                    switch (UserType)
                    {

                        case 1:
                            if (RoleId == -1)
                            {
                                List<EnterpriseDTO> lstAllEnterPrises = (from em in objEnterpriseDAL.GetAllEnterprise(false).Where(t => t.IsArchived == false && t.IsDeleted == false)
                                                                         orderby em.IsActive descending, em.Name ascending
                                                                         select em).ToList();
                                //objEnterpriseDAL.GetAllEnterprise().Where(t => t.IsArchived == false && t.IsDeleted == false).OrderByDescending(t => t.IsActive).ToList();

                                if (lstAllEnterPrises != null && lstAllEnterPrises.Count() > 0)
                                {
                                    if (objUserLoginHistoryDTO != null && objUserLoginHistoryDTO.EnterpriseId > 0 && lstAllEnterPrises.Any(t => t.ID == objUserLoginHistoryDTO.EnterpriseId))
                                    {
                                        objEnterpriseDTO = lstAllEnterPrises.First(t => t.ID == objUserLoginHistoryDTO.EnterpriseId);
                                        SessionHelper.EnterPriceID = objEnterpriseDTO.ID;
                                        SessionHelper.EnterpriseLogoUrl = objEnterpriseDTO.EnterpriseLogo;
                                        SessionHelper.EnterPriseDBName = objEnterpriseDTO.EnterpriseDBName;
                                        SessionHelper.EnterPriseDBConnectionString = objEnterpriseDTO.EnterpriseDBConnectionString;
                                    }
                                    else
                                    {
                                        SessionHelper.EnterPriceID = lstAllEnterPrises.First().ID;
                                        SessionHelper.EnterpriseLogoUrl = lstAllEnterPrises.First().EnterpriseLogo;
                                        SessionHelper.EnterPriseDBName = lstAllEnterPrises.First().EnterpriseDBName;
                                        SessionHelper.EnterPriseDBConnectionString = lstAllEnterPrises.First().EnterpriseDBConnectionString;
                                    }
                                    SessionHelper.EnterPriseList = lstAllEnterPrises;
                                    objCompanyMasterDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
                                    //List<CompanyMasterDTO> lstAllCompanies = objCompanyMasterDAL.GetAllRecords().OrderBy(t => t.Name).ToList();
                                    List<CompanyMasterDTO> lstAllCompanies = (from cm in objCompanyMasterDAL.GetAllRecords()
                                                                              orderby cm.IsActive descending, cm.Name ascending
                                                                              select cm).ToList();
                                    if (lstAllCompanies != null && lstAllCompanies.Count() > 0)
                                    {
                                        lstAllCompanies.ForEach(t =>
                                        {
                                            t.EnterPriseId = SessionHelper.EnterPriceID;
                                            t.EnterPriseName = SessionHelper.EnterPriceName;
                                        });
                                        if (objUserLoginHistoryDTO != null && objUserLoginHistoryDTO.CompanyId > 0 && lstAllCompanies.Any(t => t.EnterPriseId == SessionHelper.EnterPriceID && t.ID == objUserLoginHistoryDTO.CompanyId))
                                        {
                                            objCompanyMasterDTO = lstAllCompanies.First(t => t.EnterPriseId == SessionHelper.EnterPriceID && t.ID == objUserLoginHistoryDTO.CompanyId);
                                            SessionHelper.CompanyID = objCompanyMasterDTO.ID;
                                            SessionHelper.CompanyLogoUrl = objCompanyMasterDTO.CompanyLogo;
                                            SessionHelper.CompanyName = objCompanyMasterDTO.Name;
                                        }
                                        else
                                        {
                                            SessionHelper.CompanyID = lstAllCompanies.First().ID;
                                            SessionHelper.CompanyLogoUrl = lstAllCompanies.First().CompanyLogo;
                                            SessionHelper.CompanyName = lstAllCompanies.First().Name;
                                        }
                                        SessionHelper.CompanyList = lstAllCompanies;
                                        objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
                                        List<RoomDTO> lstRooms = (from rm in objRoomDAL.GetAllRoomByCompany(SessionHelper.CompanyID, false, false)
                                                                  orderby rm.IsRoomActive descending, rm.RoomName ascending
                                                                  select rm).ToList();

                                        if (lstRooms != null && lstRooms.Count() > 0)
                                        {
                                            lstRooms.ForEach(t =>
                                            {
                                                t.EnterpriseId = SessionHelper.EnterPriceID;
                                                t.EnterpriseName = SessionHelper.EnterPriceName;
                                            });
                                            if (objUserLoginHistoryDTO != null && objUserLoginHistoryDTO.RoomId > 0 && lstRooms.Any(t => t.EnterpriseId == objUserLoginHistoryDTO.EnterpriseId && t.CompanyID == objUserLoginHistoryDTO.CompanyId && t.ID == objUserLoginHistoryDTO.RoomId))
                                            {
                                                objRoomDTO = lstRooms.First(t => t.EnterpriseId == objUserLoginHistoryDTO.EnterpriseId && t.CompanyID == objUserLoginHistoryDTO.CompanyId && t.ID == objUserLoginHistoryDTO.RoomId);
                                                SessionHelper.RoomID = objRoomDTO.ID;
                                                SessionHelper.RoomName = objRoomDTO.RoomName;
                                            }
                                            else
                                            {
                                                SessionHelper.RoomID = lstRooms.First().ID;
                                                SessionHelper.RoomName = lstRooms.First().RoomName;
                                            }
                                            SessionHelper.RoomList = lstRooms;
                                        }
                                        else
                                        {
                                            SessionHelper.RoomID = 0;
                                            SessionHelper.RoomName = string.Empty;
                                            SessionHelper.RoomList = new List<RoomDTO>();
                                        }
                                    }
                                    else
                                    {
                                        SessionHelper.CompanyID = 0;
                                        SessionHelper.CompanyName = string.Empty;
                                        SessionHelper.CompanyLogoUrl = string.Empty;
                                        SessionHelper.CompanyList = new List<CompanyMasterDTO>();
                                        SessionHelper.RoomID = 0;
                                        SessionHelper.RoomName = string.Empty;
                                        SessionHelper.RoomList = new List<RoomDTO>();
                                    }
                                }
                                else
                                {
                                    SessionHelper.EnterPriceID = 0;
                                    SessionHelper.EnterpriseLogoUrl = string.Empty;
                                    SessionHelper.EnterPriseDBName = string.Empty;
                                    SessionHelper.EnterPriseDBConnectionString = string.Empty;
                                    SessionHelper.EnterPriseList = new List<EnterpriseDTO>();
                                    SessionHelper.CompanyID = 0;
                                    SessionHelper.CompanyName = string.Empty;
                                    SessionHelper.CompanyList = new List<CompanyMasterDTO>();
                                }

                                List<UserWiseRoomsAccessDetailsDTO> lstUserWiseRoomsAccessDetailsDTO = objUserMasterDAL.GetSuperAdminRoomPermissions(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.RoleID, SessionHelper.UserID);
                                SessionHelper.RoomPermissions = SetPermissionsForsuperadmin(lstUserWiseRoomsAccessDetailsDTO, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.RoleID, SessionHelper.UserType);

                            }
                            else
                            {
                                string strRoomList = string.Empty;

                                List<UserRoleModuleDetailsDTO> lstUserRoleModuleDetailsDTO = objUserMasterDAL.GetUserRoleModuleDetailsRecord(UserId, RoleId, UserType);
                                List<UserWiseRoomsAccessDetailsDTO> lstUserWiseRoomsAccessDetails = objUserMasterDAL.ConvertUserPermissions(lstUserRoleModuleDetailsDTO, RoleId, ref strRoomList);
                                List<UserAccessDTO> lstAccess = objEnterpriseDAL.GetUserAccessWithNames(UserId);
                                if (lstAccess != null && lstAccess.Count > 0)
                                {
                                    SessionHelper.RoomPermissions = lstUserWiseRoomsAccessDetails;
                                    //List<UserRoleModuleDetailsDTO> lstddd = lstUserWiseRoomsAccessDetails.Where(t => t.EnterpriseId == 18 && t.CompanyId == 1 && t.RoomID == 1).FirstOrDefault().PermissionList.Where(t => t.ModuleID == 77).ToList();
                                    List<EnterpriseDTO> lstAllEnterPrises = (from itm in lstAccess.Where(t => t.EnterpriseId > 0)
                                                                             orderby itm.IsEnterpriseActive descending, itm.EnterpriseName ascending
                                                                             group itm by new { itm.EnterpriseId, itm.EnterpriseName, itm.IsEnterpriseActive } into gropedentrprs
                                                                             select new EnterpriseDTO
                                                                             {
                                                                                 ID = gropedentrprs.Key.EnterpriseId,
                                                                                 Name = gropedentrprs.Key.EnterpriseName,
                                                                                 IsActive = gropedentrprs.Key.IsEnterpriseActive
                                                                             }).ToList();
                                    if (lstAllEnterPrises != null && lstAllEnterPrises.Count() > 0)
                                    {

                                        //List<EnterpriseDTO> lstActiveEps = objUserMasterDAL.GetActiveEnterprises(lstAllEnterPrises.Select(t => t.ID).ToArray());
                                        if (objUserLoginHistoryDTO != null && objUserLoginHistoryDTO.EnterpriseId > 0 && lstAllEnterPrises.Any(t => t.ID == objUserLoginHistoryDTO.EnterpriseId))
                                        {
                                            objEnterpriseDTO = lstAllEnterPrises.First(t => t.ID == objUserLoginHistoryDTO.EnterpriseId);
                                            SessionHelper.EnterPriceID = objEnterpriseDTO.ID;

                                        }
                                        else
                                        {
                                            SessionHelper.EnterPriceID = lstAllEnterPrises.First().ID;

                                        }
                                        SessionHelper.EnterPriseList = lstAllEnterPrises;
                                        objEnterpriseDTO = objEnterpriseDAL.GetEnterprise(SessionHelper.EnterPriceID);
                                        SessionHelper.EnterPriceName = objEnterpriseDTO.Name;
                                        SessionHelper.EnterpriseLogoUrl = objEnterpriseDTO.EnterpriseLogo;
                                        SessionHelper.EnterPriseDBName = objEnterpriseDTO.EnterpriseDBName;
                                        SessionHelper.EnterPriseDBConnectionString = objEnterpriseDTO.EnterpriseDBConnectionString;
                                        List<CompanyMasterDTO> lstAllCompanies = (from itm in lstAccess.Where(t => t.CompanyId > 0)
                                                                                  orderby itm.IsCompanyActive descending, itm.CompanyName ascending
                                                                                  group itm by new { itm.EnterpriseId, itm.EnterpriseName, itm.CompanyId, itm.CompanyName, itm.IsCompanyActive } into gropedentrprs
                                                                                  select new CompanyMasterDTO
                                                                                  {
                                                                                      EnterPriseId = gropedentrprs.Key.EnterpriseId,
                                                                                      EnterPriseName = gropedentrprs.Key.EnterpriseName,
                                                                                      ID = gropedentrprs.Key.CompanyId,
                                                                                      Name = gropedentrprs.Key.CompanyName,
                                                                                      IsActive = gropedentrprs.Key.IsCompanyActive
                                                                                  }).ToList();
                                        SessionHelper.CompanyList = lstAllCompanies;
                                        if (lstAllCompanies != null && lstAllCompanies.Count(t => t.EnterPriseId == SessionHelper.EnterPriceID) > 0)
                                        {
                                            if (objUserLoginHistoryDTO != null && objUserLoginHistoryDTO.EnterpriseId > 0 && lstAllEnterPrises.Any(t => t.ID == objUserLoginHistoryDTO.EnterpriseId))
                                            {
                                                objCompanyMasterDTO = lstAllCompanies.First(t => t.EnterPriseId == SessionHelper.EnterPriceID && t.ID == objUserLoginHistoryDTO.CompanyId);
                                                SessionHelper.CompanyID = objCompanyMasterDTO.ID;
                                                SessionHelper.CompanyLogoUrl = objCompanyMasterDTO.CompanyLogo;
                                                SessionHelper.CompanyName = objCompanyMasterDTO.Name;
                                                //SessionHelper.CompanyID = lstAllCompanies.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).OrderBy(t => t.Name).First().ID;
                                                //SessionHelper.CompanyName = lstAllCompanies.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).OrderBy(t => t.Name).First().Name;
                                                //SessionHelper.CompanyLogoUrl = lstAllCompanies.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).OrderBy(t => t.Name).First().CompanyLogo;
                                            }
                                            else
                                            {
                                                SessionHelper.CompanyID = lstAllCompanies.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).OrderBy(t => t.Name).First().ID;
                                                SessionHelper.CompanyName = lstAllCompanies.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).OrderBy(t => t.Name).First().Name;
                                                SessionHelper.CompanyLogoUrl = lstAllCompanies.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).OrderBy(t => t.Name).First().CompanyLogo;
                                            }
                                            List<RoomDTO> lstAllRooms = (from itm in lstAccess.Where(t => t.RoomId > 0)
                                                                         orderby itm.IsRoomActive descending, itm.RoomName ascending
                                                                         group itm by new { itm.EnterpriseId, itm.EnterpriseName, itm.CompanyId, itm.CompanyName, itm.RoomId, itm.RoomName, itm.IsRoomActive } into gropedentrprs
                                                                         select new RoomDTO
                                                                         {
                                                                             EnterpriseId = gropedentrprs.Key.EnterpriseId,
                                                                             EnterpriseName = gropedentrprs.Key.EnterpriseName,
                                                                             CompanyID = gropedentrprs.Key.CompanyId,
                                                                             CompanyName = gropedentrprs.Key.CompanyName,
                                                                             ID = gropedentrprs.Key.RoomId,
                                                                             RoomName = gropedentrprs.Key.RoomName,
                                                                             IsRoomActive = gropedentrprs.Key.IsRoomActive
                                                                         }).ToList();

                                            SessionHelper.RoomList = lstAllRooms;
                                            if(lstAllRooms != null && lstAllRooms.Count(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID) > 0)
                                            {
                                                if (objUserLoginHistoryDTO != null && objUserLoginHistoryDTO.RoomId > 0 && lstAllRooms.Any(t => t.EnterpriseId == objUserLoginHistoryDTO.EnterpriseId && t.CompanyID == objUserLoginHistoryDTO.CompanyId && t.ID == objUserLoginHistoryDTO.RoomId))
                                                {
                                                    objRoomDTO = lstAllRooms.First(t => t.EnterpriseId == objUserLoginHistoryDTO.EnterpriseId && t.CompanyID == objUserLoginHistoryDTO.CompanyId && t.ID == objUserLoginHistoryDTO.RoomId);
                                                    SessionHelper.RoomID = objRoomDTO.ID;
                                                    SessionHelper.RoomName = objRoomDTO.RoomName;
                                                }
                                                else
                                                {
                                                    SessionHelper.RoomID = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().ID;
                                                    SessionHelper.RoomName = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().RoomName;
                                                }
                                            }
                                            else
                                            {
                                                SessionHelper.RoomID = 0;
                                                SessionHelper.RoomName = string.Empty;
                                            }
                                        }
                                        else
                                        {
                                            SessionHelper.CompanyID = 0;
                                            SessionHelper.CompanyName = string.Empty;
                                            SessionHelper.RoomID = 0;
                                            SessionHelper.RoomName = string.Empty;
                                        }
                                    }
                                    else
                                    {
                                        SessionHelper.EnterPriceID = 0;
                                        SessionHelper.EnterPriceName = string.Empty;
                                        SessionHelper.CompanyID = 0;
                                        SessionHelper.CompanyName = string.Empty;
                                        SessionHelper.RoomID = 0;
                                        SessionHelper.RoomName = string.Empty;
                                        SessionHelper.EnterPriseList = new List<EnterpriseDTO>();
                                        SessionHelper.CompanyList = new List<CompanyMasterDTO>();
                                        SessionHelper.RoomList = new List<RoomDTO>();
                                    }
                                }
                            }
                            if (!objUserMasterDAL.IsSAdminUserExist(SessionHelper.UserID, SessionHelper.EnterPriceID))
                            {
                                UserMasterDTO objUserMasterDTO = new UserMasterDTO();
                                objUserMasterDTO.CompanyID = 0;
                                objUserMasterDTO.Created = DateTimeUtility.DateTimeNow;
                                objUserMasterDTO.CreatedBy = SessionHelper.UserID;
                                objUserMasterDTO.CreatedByName = SessionHelper.UserName;
                                objUserMasterDTO.Email = SessionHelper.UserName;
                                objUserMasterDTO.EnterpriseDbName = string.Empty;
                                objUserMasterDTO.EnterpriseId = 0;
                                objUserMasterDTO.GUID = Guid.NewGuid();
                                objUserMasterDTO.IsArchived = false;
                                objUserMasterDTO.IsDeleted = false;
                                objUserMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                                objUserMasterDTO.Password = "password";
                                objUserMasterDTO.Phone = "[!!AdminPbone!!]";
                                objUserMasterDTO.RoleID = SessionHelper.RoleID;
                                objUserMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                                objUserMasterDTO.UserType = SessionHelper.UserType;
                                objUserMasterDTO.UserName = SessionHelper.UserName;
                                objUserMasterDAL.InsertSAdminUserInChildDB(SessionHelper.UserID, objUserMasterDTO, SessionHelper.EnterPriceID);
                            }
                            break;
                        case 2:
                            if (RoleId == -2)
                            {
                                SessionHelper.EnterPriceID = EnterpriseId;
                                objEnterpriseDTO = objEnterpriseDAL.GetEnterprise(SessionHelper.EnterPriceID);
                                SessionHelper.EnterpriseLogoUrl = objEnterpriseDTO.EnterpriseLogo;
                                SessionHelper.EnterPriceName = objEnterpriseDTO.Name;
                                SessionHelper.EnterPriseDBName = objEnterpriseDTO.EnterpriseDBName;
                                SessionHelper.EnterPriseDBConnectionString = objEnterpriseDTO.EnterpriseDBConnectionString;
                                List<EnterpriseDTO> lstEnterprise = new List<EnterpriseDTO>();
                                lstEnterprise.Add(objEnterpriseDTO);
                                SessionHelper.EnterPriseList = lstEnterprise;
                                objCompanyMasterDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
                                List<CompanyMasterDTO> lstAllCompanies = (from cm in objCompanyMasterDAL.GetAllRecords()
                                                                          orderby cm.IsActive descending, cm.Name ascending
                                                                          select cm).ToList();
                                //List<CompanyMasterDTO> lstAllCompanies = objCompanyMasterDAL.GetAllRecords().OrderBy(t => t.Name).ToList();
                                if (lstAllCompanies != null && lstAllCompanies.Count() > 0)
                                {
                                    lstAllCompanies.ForEach(t =>
                                    {
                                        t.EnterPriseId = SessionHelper.EnterPriceID;
                                        t.EnterPriseName = SessionHelper.EnterPriceName;
                                    });
                                    if (objUserLoginHistoryDTO != null && objUserLoginHistoryDTO.CompanyId > 0 && lstAllCompanies.Any(t => t.EnterPriseId == SessionHelper.EnterPriceID && t.ID == objUserLoginHistoryDTO.CompanyId))
                                    {
                                        objCompanyMasterDTO = lstAllCompanies.First(t => t.EnterPriseId == SessionHelper.EnterPriceID && t.ID == objUserLoginHistoryDTO.CompanyId);
                                        SessionHelper.CompanyID = objCompanyMasterDTO.ID;
                                        SessionHelper.CompanyLogoUrl = objCompanyMasterDTO.CompanyLogo;
                                        SessionHelper.CompanyName = objCompanyMasterDTO.Name;
                                    }
                                    else
                                    {
                                        SessionHelper.CompanyID = lstAllCompanies.First().ID;
                                        SessionHelper.CompanyName = lstAllCompanies.First().Name;
                                        SessionHelper.CompanyLogoUrl = lstAllCompanies.First().CompanyLogo;
                                    }
                                    SessionHelper.CompanyList = lstAllCompanies;
                                    objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
                                    //List<RoomDTO> lstRooms = objRoomDAL.GetAllRoomByCompany(SessionHelper.CompanyID, false, false).OrderBy(t => t.RoomName).ToList();
                                    List<RoomDTO> lstRooms = (from rm in objRoomDAL.GetAllRoomByCompany(SessionHelper.CompanyID, false, false)
                                                              orderby rm.IsRoomActive descending, rm.RoomName ascending
                                                              select rm).ToList();
                                    if (lstRooms != null && lstRooms.Count > 0)
                                    {
                                        lstRooms.ForEach(t =>
                                        {
                                            t.EnterpriseId = SessionHelper.EnterPriceID;
                                            t.EnterpriseName = SessionHelper.EnterPriceName;
                                        });
                                        if (objUserLoginHistoryDTO != null && objUserLoginHistoryDTO.RoomId > 0 && lstRooms.Any(t => t.EnterpriseId == objUserLoginHistoryDTO.EnterpriseId && t.CompanyID == objUserLoginHistoryDTO.CompanyId && t.ID == objUserLoginHistoryDTO.RoomId))
                                        {
                                            objRoomDTO = lstRooms.First(t => t.EnterpriseId == objUserLoginHistoryDTO.EnterpriseId && t.CompanyID == objUserLoginHistoryDTO.CompanyId && t.ID == objUserLoginHistoryDTO.RoomId);
                                            SessionHelper.RoomID = objRoomDTO.ID;
                                            SessionHelper.RoomName = objRoomDTO.RoomName;
                                        }
                                        else
                                        {
                                            SessionHelper.RoomID = lstRooms.First().ID;
                                            SessionHelper.RoomName = lstRooms.First().RoomName;
                                        }
                                        SessionHelper.RoomList = lstRooms;
                                    }
                                    else
                                    {
                                        SessionHelper.RoomID = 0;
                                        SessionHelper.RoomName = string.Empty;
                                        SessionHelper.RoomList = new List<RoomDTO>();
                                    }
                                }
                                else
                                {
                                    SessionHelper.CompanyID = 0;
                                    SessionHelper.CompanyLogoUrl = string.Empty;
                                    SessionHelper.CompanyName = string.Empty;
                                    SessionHelper.CompanyList = new List<CompanyMasterDTO>();
                                    SessionHelper.RoomID = 0;
                                    SessionHelper.RoomName = string.Empty;
                                    SessionHelper.RoomList = new List<RoomDTO>();
                                }
                                objinterUserDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                                List<UserWiseRoomsAccessDetailsDTO> lstUserWiseRoomsAccessDetailsDTO = objinterUserDAL.GetEnterpriseAdminRoomPermissions(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.RoleID, SessionHelper.UserID);
                                SessionHelper.RoomPermissions = SetPermissionsForsuperadmin(lstUserWiseRoomsAccessDetailsDTO, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.RoleID, SessionHelper.UserType);
                            }
                            else
                            {
                                SessionHelper.EnterPriceID = EnterpriseId;
                                objEnterpriseDTO = objEnterpriseDAL.GetEnterprise(SessionHelper.EnterPriceID);
                                SessionHelper.EnterpriseLogoUrl = objEnterpriseDTO.EnterpriseLogo;
                                SessionHelper.EnterPriseDBName = objEnterpriseDTO.EnterpriseDBName;
                                SessionHelper.EnterPriseDBConnectionString = objEnterpriseDTO.EnterpriseDBConnectionString;
                                List<EnterpriseDTO> lstEnterprise = new List<EnterpriseDTO>();
                                lstEnterprise.Add(objEnterpriseDTO);
                                SessionHelper.EnterPriseList = lstEnterprise;
                                string strRoomList = string.Empty;
                                objinterUserDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                                List<UserRoleModuleDetailsDTO> lstUserRoleModuleDetailsDTO = objinterUserDAL.GetUserRoleModuleDetailsRecord(UserId, RoleId, UserType);
                                List<UserWiseRoomsAccessDetailsDTO> lstUserWiseRoomsAccessDetails = objinterUserDAL.ConvertUserPermissions(lstUserRoleModuleDetailsDTO, RoleId, ref strRoomList);
                                List<UserAccessDTO> lstAccess = objinterUserDAL.GetUserAccessWithNames(UserId, objEnterpriseDTO.Name, objEnterpriseDTO.IsActive);
                                if (lstAccess != null && lstAccess.Count > 0)
                                {
                                    SessionHelper.RoomPermissions = lstUserWiseRoomsAccessDetails;
                                    //List<CompanyMasterDTO> lstAllCompanies = (from itm in lstUserWiseRoomsAccessDetails
                                    //                                          group itm by new { itm.EnterpriseId, itm.EnterpriseName, itm.CompanyId, itm.CompanyName } into gropedentrprs
                                    //                                          select new CompanyMasterDTO
                                    //                                          {
                                    //                                              EnterPriseId = gropedentrprs.Key.EnterpriseId,
                                    //                                              EnterPriseName = gropedentrprs.Key.EnterpriseName,
                                    //                                              ID = gropedentrprs.Key.CompanyId,
                                    //                                              Name = gropedentrprs.Key.CompanyName
                                    //                                          }).OrderBy(t => t.Name).ToList();
                                    List<CompanyMasterDTO> lstAllCompanies = (from itm in lstAccess.Where(t => t.CompanyId > 0)
                                                                              orderby itm.IsCompanyActive descending, itm.CompanyName ascending
                                                                              group itm by new { itm.EnterpriseId, itm.EnterpriseName, itm.CompanyId, itm.CompanyName, itm.CompanyLogo, itm.IsCompanyActive } into gropedentrprs
                                                                              select new CompanyMasterDTO
                                                                              {
                                                                                  EnterPriseId = gropedentrprs.Key.EnterpriseId,
                                                                                  EnterPriseName = gropedentrprs.Key.EnterpriseName,
                                                                                  ID = gropedentrprs.Key.CompanyId,
                                                                                  Name = gropedentrprs.Key.CompanyName,
                                                                                  IsActive = gropedentrprs.Key.IsCompanyActive,
                                                                                  CompanyLogo = gropedentrprs.Key.CompanyLogo
                                                                              }).ToList();
                                    SessionHelper.CompanyList = lstAllCompanies;
                                    if (lstAllCompanies != null && lstAllCompanies.Count(t => t.EnterPriseId == SessionHelper.EnterPriceID) > 0)
                                    {
                                        if (objUserLoginHistoryDTO != null && objUserLoginHistoryDTO.CompanyId > 0 && lstAllCompanies.Any(t => t.ID == objUserLoginHistoryDTO.CompanyId))
                                        {
                                            objCompanyMasterDTO = lstAllCompanies.First(t => t.ID == objUserLoginHistoryDTO.CompanyId);
                                            SessionHelper.CompanyID = objCompanyMasterDTO.ID;
                                            SessionHelper.CompanyName = objCompanyMasterDTO.Name;
                                        }
                                        else
                                        {
                                            SessionHelper.CompanyID = lstAllCompanies.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).OrderBy(t => t.Name).First().ID;
                                            SessionHelper.CompanyName = lstAllCompanies.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).OrderBy(t => t.Name).First().Name;
                                        }
                                        SessionHelper.CompanyLogoUrl = lstAllCompanies.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).OrderBy(t => t.Name).First().CompanyLogo;
                                        List<RoomDTO> lstAllRooms = (from itm in lstAccess.Where(t => t.RoomId > 0)
                                                                     orderby itm.IsRoomActive descending, itm.RoomName ascending
                                                                     group itm by new { itm.EnterpriseId, itm.EnterpriseName, itm.CompanyId, itm.CompanyName, itm.RoomId, itm.RoomName, itm.IsRoomActive } into gropedentrprs
                                                                     select new RoomDTO
                                                                     {
                                                                         EnterpriseId = gropedentrprs.Key.EnterpriseId,
                                                                         EnterpriseName = gropedentrprs.Key.EnterpriseName,
                                                                         CompanyID = gropedentrprs.Key.CompanyId,
                                                                         CompanyName = gropedentrprs.Key.CompanyName,
                                                                         ID = gropedentrprs.Key.RoomId,
                                                                         RoomName = gropedentrprs.Key.RoomName,
                                                                         IsRoomActive = gropedentrprs.Key.IsRoomActive
                                                                     }).ToList();

                                        SessionHelper.RoomList = lstAllRooms;
                                        if (lstAllRooms != null && lstAllRooms.Count(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID) > 0)
                                        {
                                            if (objUserLoginHistoryDTO != null && objUserLoginHistoryDTO.CompanyId > 0 && lstAllCompanies.Any(t => t.ID == objUserLoginHistoryDTO.RoomId))
                                            {
                                                SessionHelper.RoomID = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().ID;
                                                SessionHelper.RoomName = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().RoomName;
                                            }
                                            else
                                            {
                                                SessionHelper.RoomID = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().ID;
                                                SessionHelper.RoomName = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().RoomName;
                                            }
                                        }
                                        else
                                        {
                                            SessionHelper.RoomID = 0;
                                            SessionHelper.RoomName = string.Empty;
                                        }
                                    }
                                }

                            }
                            break;
                        case 3:
                            SessionHelper.EnterPriceID = EnterpriseId;
                            objEnterpriseDTO = objEnterpriseDAL.GetEnterprise(SessionHelper.EnterPriceID);
                            SessionHelper.EnterpriseLogoUrl = objEnterpriseDTO.EnterpriseLogo;
                            SessionHelper.EnterPriseDBName = objEnterpriseDTO.EnterpriseDBName;
                            SessionHelper.EnterPriseDBConnectionString = objEnterpriseDTO.EnterpriseDBConnectionString;
                            List<EnterpriseDTO> lstEnterprise3 = new List<EnterpriseDTO>();
                            lstEnterprise3.Add(objEnterpriseDTO);
                            SessionHelper.EnterPriseList = lstEnterprise3;
                            SessionHelper.CompanyID = CompanyId;
                            string strRoomList3 = string.Empty;
                            objinterUserDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                            List<UserRoleModuleDetailsDTO> lstUserRoleModuleDetailsDTO3 = objinterUserDAL.GetUserRoleModuleDetailsRecord(UserId, RoleId, UserType);
                            List<UserWiseRoomsAccessDetailsDTO> lstUserWiseRoomsAccessDetails3 = objinterUserDAL.ConvertUserPermissions(lstUserRoleModuleDetailsDTO3, RoleId, ref strRoomList3);
                            List<UserAccessDTO> lstAccess1 = objinterUserDAL.GetUserAccessWithNames(UserId, objEnterpriseDTO.Name, objEnterpriseDTO.IsActive);
                            if (lstAccess1 != null && lstAccess1.Count > 0)
                            {
                                SessionHelper.RoomPermissions = lstUserWiseRoomsAccessDetails3;
                                List<CompanyMasterDTO> lstAllCompanies = (from itm in lstAccess1
                                                                          orderby itm.IsCompanyActive descending, itm.CompanyName ascending
                                                                          group itm by new { itm.EnterpriseId, itm.EnterpriseName, itm.CompanyId, itm.CompanyName, itm.CompanyLogo, itm.IsCompanyActive } into gropedentrprs
                                                                          select new CompanyMasterDTO
                                                                          {
                                                                              EnterPriseId = gropedentrprs.Key.EnterpriseId,
                                                                              EnterPriseName = gropedentrprs.Key.EnterpriseName,
                                                                              ID = gropedentrprs.Key.CompanyId,
                                                                              Name = gropedentrprs.Key.CompanyName,
                                                                              CompanyLogo = gropedentrprs.Key.CompanyLogo,
                                                                              IsActive = gropedentrprs.Key.IsCompanyActive
                                                                          }).ToList();
                                SessionHelper.CompanyList = lstAllCompanies;
                                if (lstAllCompanies != null && lstAllCompanies.Count(t => t.EnterPriseId == SessionHelper.EnterPriceID) > 0)
                                {
                                    SessionHelper.CompanyID = lstAllCompanies.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).OrderBy(t => t.Name).First().ID;
                                    SessionHelper.CompanyName = lstAllCompanies.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).OrderBy(t => t.Name).First().Name;
                                    SessionHelper.CompanyLogoUrl = lstAllCompanies.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).OrderBy(t => t.Name).First().CompanyLogo;
                                    List<RoomDTO> lstAllRooms = (from itm in lstAccess1
                                                                 orderby itm.IsRoomActive descending, itm.RoomName ascending
                                                                 group itm by new { itm.EnterpriseId, itm.EnterpriseName, itm.CompanyId, itm.CompanyName, itm.RoomId, itm.RoomName, itm.IsRoomActive } into gropedentrprs
                                                                 select new RoomDTO
                                                                 {
                                                                     EnterpriseId = gropedentrprs.Key.EnterpriseId,
                                                                     EnterpriseName = gropedentrprs.Key.EnterpriseName,
                                                                     CompanyID = gropedentrprs.Key.CompanyId,
                                                                     CompanyName = gropedentrprs.Key.CompanyName,
                                                                     ID = gropedentrprs.Key.RoomId,
                                                                     RoomName = gropedentrprs.Key.RoomName,
                                                                     IsRoomActive = gropedentrprs.Key.IsRoomActive
                                                                 }).ToList();

                                    SessionHelper.RoomList = lstAllRooms;
                                    if (lstAllRooms != null && lstAllRooms.Count(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID) > 0)
                                    {
                                        if (objUserLoginHistoryDTO != null && objUserLoginHistoryDTO.RoomId > 0 && lstAllRooms.Any(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID && t.ID == objUserLoginHistoryDTO.RoomId))
                                        {
                                            SessionHelper.RoomID = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID && t.ID == objUserLoginHistoryDTO.RoomId).First().ID;
                                            SessionHelper.RoomName = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID && t.ID == objUserLoginHistoryDTO.RoomId).First().RoomName;
                                        }
                                        else
                                        {
                                            SessionHelper.RoomID = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().ID;
                                            SessionHelper.RoomName = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().RoomName;
                                        }
                                    }
                                }
                            }

                            break;
                        default:
                            break;
                    }
                    break;
                case "enterprise":
                    switch (UserType)
                    {
                        case 1:
                            if (RoleId == -1)
                            {
                                SessionHelper.EnterPriceID = EnterpriseId;
                                objEnterpriseDTO = objEnterpriseDAL.GetEnterprise(SessionHelper.EnterPriceID);
                                SessionHelper.EnterpriseLogoUrl = objEnterpriseDTO.EnterpriseLogo;
                                SessionHelper.EnterPriseDBName = objEnterpriseDTO.EnterpriseDBName;
                                SessionHelper.EnterPriseDBConnectionString = objEnterpriseDTO.EnterpriseDBConnectionString;
                                objCompanyMasterDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
                                List<CompanyMasterDTO> lstAllCompanies = (from cm in objCompanyMasterDAL.GetAllRecords()
                                                                          orderby cm.IsActive descending, cm.Name ascending
                                                                          select cm).ToList();
                                if (lstAllCompanies != null && lstAllCompanies.Count() > 0)
                                {
                                    //List<CompanyMasterDTO> lstAllCompanies = objCompanyMasterDAL.GetAllRecords().OrderBy(t => t.Name).ToList();
                                    //if (lstAllCompanies != null && lstAllCompanies.Count > 0)
                                    //{
                                    lstAllCompanies.ForEach(t =>
                                    {
                                        t.EnterPriseId = SessionHelper.EnterPriceID;
                                        t.EnterPriseName = SessionHelper.EnterPriceName;
                                    });
                                    SessionHelper.CompanyID = lstAllCompanies.First().ID;
                                    SessionHelper.CompanyName = lstAllCompanies.First().Name;
                                    SessionHelper.CompanyLogoUrl = lstAllCompanies.First().CompanyLogo;
                                    SessionHelper.CompanyList = lstAllCompanies;
                                    //List<RoomDTO> lstRooms = objRoomDAL.GetAllRoomByCompany(SessionHelper.CompanyID, false, false).OrderBy(t => t.RoomName).ToList();
                                    //if (lstRooms != null && lstRooms.Count > 0)
                                    //{ 
                                    objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
                                    List<RoomDTO> lstRooms = (from rm in objRoomDAL.GetAllRoomByCompany(SessionHelper.CompanyID, false, false)
                                                              orderby rm.IsRoomActive descending, rm.RoomName ascending
                                                              select rm).ToList();

                                    if (lstRooms != null && lstRooms.Count() > 0)
                                    {
                                        lstRooms.ForEach(t =>
                                        {
                                            t.EnterpriseId = SessionHelper.EnterPriceID;
                                            t.EnterpriseName = SessionHelper.EnterPriceName;
                                        });
                                        SessionHelper.RoomID = lstRooms.First().ID;
                                        SessionHelper.RoomName = lstRooms.First().RoomName;
                                        SessionHelper.RoomList = lstRooms;
                                    }
                                    else
                                    {
                                        SessionHelper.RoomID = 0;
                                        SessionHelper.RoomName = string.Empty;
                                        SessionHelper.RoomList = new List<RoomDTO>();
                                    }
                                }
                                else
                                {
                                    SessionHelper.CompanyID = 0;
                                    SessionHelper.CompanyName = string.Empty;
                                    SessionHelper.CompanyLogoUrl = string.Empty;
                                    SessionHelper.CompanyList = new List<CompanyMasterDTO>();
                                    SessionHelper.RoomID = 0;
                                    SessionHelper.RoomName = string.Empty;
                                    SessionHelper.RoomList = new List<RoomDTO>();
                                }
                                List<UserWiseRoomsAccessDetailsDTO> lstpermissionfromsession = SessionHelper.RoomPermissions;
                                lstpermissionfromsession.ForEach(t =>
                                {
                                    t.EnterpriseId = SessionHelper.EnterPriceID;
                                    t.CompanyId = SessionHelper.CompanyID;
                                    t.RoomID = SessionHelper.RoomID;
                                    t.PermissionList.ForEach(et =>
                                    {
                                        et.EnteriseId = SessionHelper.EnterPriceID;
                                        et.CompanyId = SessionHelper.CompanyID;
                                        et.RoomId = SessionHelper.RoomID;
                                    });
                                });
                                SessionHelper.RoomPermissions = SetPermissionsForsuperadmin(lstpermissionfromsession, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.RoleID, SessionHelper.UserType);
                            }
                            else
                            {
                                SessionHelper.EnterPriceID = EnterpriseId;
                                objEnterpriseDTO = objEnterpriseDAL.GetEnterprise(SessionHelper.EnterPriceID);
                                SessionHelper.EnterpriseLogoUrl = objEnterpriseDTO.EnterpriseLogo;
                                SessionHelper.EnterPriseDBName = objEnterpriseDTO.EnterpriseDBName;
                                SessionHelper.EnterPriseDBConnectionString = objEnterpriseDTO.EnterpriseDBConnectionString;
                                SessionHelper.EnterPriceName = objEnterpriseDTO.Name;

                                if (SessionHelper.CompanyList != null && SessionHelper.CompanyList.Any(t => t.EnterPriseId == SessionHelper.EnterPriceID))
                                {
                                    SessionHelper.CompanyID = SessionHelper.CompanyList.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).OrderBy(t => t.Name).First().ID;
                                    SessionHelper.CompanyName = SessionHelper.CompanyList.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).OrderBy(t => t.Name).First().Name;
                                    if (SessionHelper.RoomList != null && SessionHelper.RoomList.Any(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID))
                                    {
                                        SessionHelper.RoomID = SessionHelper.RoomList.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().ID;
                                        SessionHelper.RoomName = SessionHelper.RoomList.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().RoomName;
                                    }
                                    else
                                    {
                                        SessionHelper.RoomID = 0;
                                        SessionHelper.RoomName = string.Empty;
                                    }
                                }
                                else
                                {
                                    SessionHelper.CompanyID = 0;
                                    SessionHelper.CompanyName = string.Empty;
                                    SessionHelper.RoomID = 0;
                                    SessionHelper.RoomName = string.Empty;
                                }

                            }
                            break;
                    }
                    if (!objUserMasterDAL.IsSAdminUserExist(SessionHelper.UserID, SessionHelper.EnterPriceID))
                    {
                        UserMasterDTO objUserMasterDTO = new UserMasterDTO();
                        objUserMasterDTO.CompanyID = 0;
                        objUserMasterDTO.Created = DateTimeUtility.DateTimeNow;
                        objUserMasterDTO.CreatedBy = SessionHelper.UserID;
                        objUserMasterDTO.CreatedByName = SessionHelper.UserName;
                        objUserMasterDTO.Email = SessionHelper.UserName;
                        objUserMasterDTO.EnterpriseDbName = string.Empty;
                        objUserMasterDTO.EnterpriseId = 0;
                        objUserMasterDTO.GUID = Guid.NewGuid();
                        objUserMasterDTO.IsArchived = false;
                        objUserMasterDTO.IsDeleted = false;
                        objUserMasterDTO.LastUpdatedBy = SessionHelper.UserID;
                        objUserMasterDTO.Password = "password";
                        objUserMasterDTO.Phone = "[!!AdminPbone!!]";
                        objUserMasterDTO.RoleID = SessionHelper.RoleID;
                        objUserMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                        objUserMasterDTO.UserType = SessionHelper.UserType;
                        objUserMasterDTO.UserName = SessionHelper.UserName;
                        objUserMasterDAL.InsertSAdminUserInChildDB(SessionHelper.UserID, objUserMasterDTO, SessionHelper.EnterPriceID);
                    }

                    break;
                case "company":
                    switch (UserType)
                    {
                        case 1:
                            if (RoleId == -1)
                            {
                                SessionHelper.CompanyID = CompanyId;
                                SessionHelper.CompanyName = CompanyName;
                                objCompanyMasterDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
                                CompanyMasterDTO objCompanyMaster = objCompanyMasterDAL.GetRecord(CompanyId);
                                SessionHelper.CompanyLogoUrl = objCompanyMaster.CompanyLogo;
                                objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
                                List<RoomDTO> lstRooms = (from rm in objRoomDAL.GetAllRoomByCompany(SessionHelper.CompanyID, false, false)
                                                          orderby rm.IsRoomActive descending, rm.RoomName ascending
                                                          select rm).ToList();

                                if (lstRooms != null && lstRooms.Count() > 0)
                                {
                                    lstRooms.ForEach(t =>
                                    {
                                        t.EnterpriseId = SessionHelper.EnterPriceID;
                                        t.EnterpriseName = SessionHelper.EnterPriceName;
                                    });

                                    SessionHelper.RoomID = lstRooms.First().ID;
                                    SessionHelper.RoomName = lstRooms.First().RoomName;
                                    SessionHelper.RoomList = lstRooms;
                                }
                                else
                                {
                                    SessionHelper.RoomID = 0;
                                    SessionHelper.RoomName = string.Empty;
                                    SessionHelper.RoomList = new List<RoomDTO>();
                                }
                                List<UserWiseRoomsAccessDetailsDTO> lstpermissionfromsession = SessionHelper.RoomPermissions;
                                lstpermissionfromsession.ForEach(t =>
                                {
                                    t.EnterpriseId = SessionHelper.EnterPriceID;
                                    t.CompanyId = SessionHelper.CompanyID;
                                    t.RoomID = SessionHelper.RoomID;
                                    t.PermissionList.ForEach(et =>
                                    {
                                        et.EnteriseId = SessionHelper.EnterPriceID;
                                        et.CompanyId = SessionHelper.CompanyID;
                                        et.RoomId = SessionHelper.RoomID;
                                    });
                                });
                                SessionHelper.RoomPermissions = SetPermissionsForsuperadmin(lstpermissionfromsession, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.RoleID, SessionHelper.UserType);
                            }
                            else
                            {
                                SessionHelper.CompanyID = CompanyId;
                                SessionHelper.CompanyName = CompanyName;
                                objCompanyMasterDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
                                CompanyMasterDTO objCompanyMaster = objCompanyMasterDAL.GetRecord(CompanyId);
                                SessionHelper.CompanyLogoUrl = objCompanyMaster.CompanyLogo;
                                List<RoomDTO> lstAllrooms = SessionHelper.RoomList;
                                if (SessionHelper.RoomList != null && SessionHelper.RoomList.Any(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID))
                                {
                                    SessionHelper.RoomID = SessionHelper.RoomList.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().ID;
                                    SessionHelper.RoomName = SessionHelper.RoomList.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().RoomName;
                                }
                                else
                                {
                                    SessionHelper.RoomID = 0;
                                    SessionHelper.RoomName = string.Empty;
                                }
                            }
                            break;
                        case 2:
                            if (RoleId == -2)
                            {
                                SessionHelper.CompanyID = CompanyId;
                                SessionHelper.CompanyName = CompanyName;
                                objCompanyMasterDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
                                CompanyMasterDTO objCompanyMaster = objCompanyMasterDAL.GetRecord(CompanyId);
                                SessionHelper.CompanyLogoUrl = objCompanyMaster.CompanyLogo;
                                objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
                                List<RoomDTO> lstRooms = (from rm in objRoomDAL.GetAllRoomByCompany(SessionHelper.CompanyID, false, false)
                                                          orderby rm.IsRoomActive descending, rm.RoomName ascending
                                                          select rm).ToList();

                                if (lstRooms != null && lstRooms.Count() > 0)
                                {
                                    lstRooms.ForEach(t =>
                                    {
                                        t.EnterpriseId = SessionHelper.EnterPriceID;
                                        t.EnterpriseName = SessionHelper.EnterPriceName;
                                    });
                                    SessionHelper.RoomID = lstRooms.First().ID;
                                    SessionHelper.RoomName = lstRooms.First().RoomName;
                                    SessionHelper.RoomList = lstRooms;
                                }
                                else
                                {
                                    SessionHelper.RoomID = 0;
                                    SessionHelper.RoomName = string.Empty;
                                    SessionHelper.RoomList = new List<RoomDTO>();
                                }
                                List<UserWiseRoomsAccessDetailsDTO> lstpermissionfromsession = SessionHelper.RoomPermissions;
                                lstpermissionfromsession.ForEach(t =>
                                {
                                    t.EnterpriseId = SessionHelper.EnterPriceID;
                                    t.CompanyId = SessionHelper.CompanyID;
                                    t.RoomID = SessionHelper.RoomID;
                                    t.PermissionList.ForEach(et =>
                                    {
                                        et.EnteriseId = SessionHelper.EnterPriceID;
                                        et.CompanyId = SessionHelper.CompanyID;
                                        et.RoomId = SessionHelper.RoomID;
                                    });
                                });
                                SessionHelper.RoomPermissions = SetPermissionsForsuperadmin(lstpermissionfromsession, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.RoleID, SessionHelper.UserType);
                            }
                            else
                            {
                                SessionHelper.CompanyID = CompanyId;
                                SessionHelper.CompanyName = CompanyName;
                                objCompanyMasterDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
                                CompanyMasterDTO objCompanyMaster = objCompanyMasterDAL.GetRecord(CompanyId);
                                SessionHelper.CompanyLogoUrl = objCompanyMaster.CompanyLogo;
                                List<RoomDTO> lstAllrooms = SessionHelper.RoomList;
                                if (lstAllrooms != null && lstAllrooms.Any(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID))
                                {
                                    SessionHelper.RoomID = lstAllrooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().ID;
                                    SessionHelper.RoomName = lstAllrooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().RoomName;
                                }
                                else
                                {
                                    SessionHelper.RoomID = 0;
                                    SessionHelper.RoomName = string.Empty;
                                }
                            }
                            break;
                    }
                    break;
                case "room":
                    SessionHelper.RoomID = RoomId;
                    SessionHelper.RoomName = RoomName;
                    switch (UserType)
                    {
                        case 1:
                            if (RoleId == -1)
                            {
                                List<UserWiseRoomsAccessDetailsDTO> lstpermissionfromsession = SessionHelper.RoomPermissions;
                                lstpermissionfromsession.ForEach(t =>
                                {
                                    t.EnterpriseId = SessionHelper.EnterPriceID;
                                    t.CompanyId = SessionHelper.CompanyID;
                                    t.RoomID = SessionHelper.RoomID;
                                    t.PermissionList.ForEach(et =>
                                    {
                                        et.EnteriseId = SessionHelper.EnterPriceID;
                                        et.CompanyId = SessionHelper.CompanyID;
                                        et.RoomId = SessionHelper.RoomID;
                                    });
                                });
                                SessionHelper.RoomPermissions = SetPermissionsForsuperadmin(lstpermissionfromsession, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.RoleID, SessionHelper.UserType);

                            }
                            break;
                        case 2:
                            if (RoleId == -2)
                            {
                                List<UserWiseRoomsAccessDetailsDTO> lstpermissionfromsession = SessionHelper.RoomPermissions;
                                lstpermissionfromsession.ForEach(t =>
                                {
                                    t.EnterpriseId = SessionHelper.EnterPriceID;
                                    t.CompanyId = SessionHelper.CompanyID;
                                    t.RoomID = SessionHelper.RoomID;
                                    t.PermissionList.ForEach(et =>
                                    {
                                        et.EnteriseId = SessionHelper.EnterPriceID;
                                        et.CompanyId = SessionHelper.CompanyID;
                                        et.RoomId = SessionHelper.RoomID;
                                    });
                                });
                                SessionHelper.RoomPermissions = SetPermissionsForsuperadmin(lstpermissionfromsession, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.RoleID, SessionHelper.UserType);
                            }
                            break;
                    }

                    break;
            }
            eTurnsRegionInfo RegionInfo = null;
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
            if (SessionHelper.RoomID > 0)
            {
                RegionInfo = objRegionSettingDAL.GetRegionSettingsById(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
            }
            if (RegionInfo != null)
            {
                CultureInfo roomculture = CultureInfo.CreateSpecificCulture(RegionInfo.CultureCode);
                TimeZoneInfo roomTimeZone = TimeZoneInfo.FindSystemTimeZoneById(RegionInfo.TimeZoneName);

                SessionHelper.RoomCulture = roomculture;
                SessionHelper.CurrentTimeZone = roomTimeZone;
                SessionHelper.DateTimeFormat = RegionInfo.ShortDatePattern + " " + RegionInfo.ShortTimePattern;
                SessionHelper.RoomDateFormat = RegionInfo.ShortDatePattern;
                SessionHelper.NumberDecimalDigits = Convert.ToString(RegionInfo.NumberDecimalDigits);
                SessionHelper.CurrencyDecimalDigits = Convert.ToString(RegionInfo.CurrencyDecimalDigits);
                SessionHelper.WeightDecimalPoints = Convert.ToString(RegionInfo.WeightDecimalPoints);
                SessionHelper.NumberAvgDecimalPoints = Convert.ToString(RegionInfo.TurnsAvgDecimalPoints);

                string strQuantityFormat = "{0:0}";
                if (!string.IsNullOrWhiteSpace(SessionHelper.NumberDecimalDigits))
                {
                    if (Convert.ToInt32(Convert.ToString(eTurnsWeb.Helper.SessionHelper.NumberDecimalDigits)) > 0)
                    {
                        int iQCount = Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.NumberDecimalDigits);
                        strQuantityFormat = "{0:";
                        for (int iq = 0; iq <= iQCount; iq++)
                        {
                            if (iq == 0)
                            {
                                strQuantityFormat += "0.";
                            }
                            else
                            {
                                strQuantityFormat += "0";
                            }

                        }
                        strQuantityFormat += "}";
                    }
                    SessionHelper.QuantityFormat = strQuantityFormat;
                }
                else
                    SessionHelper.QuantityFormat = strQuantityFormat;

                string strPriceFormat = "{0:0}";
                if (!string.IsNullOrWhiteSpace(SessionHelper.CurrencyDecimalDigits))
                {
                    //if ((int)eTurnsWeb.Helper.SessionHelper.CompanyConfig.CurrencySymbol > 0)
                    if (Convert.ToInt32(Convert.ToString(eTurnsWeb.Helper.SessionHelper.CurrencyDecimalDigits)) > 0)
                    {
                        int iQCount = Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.CurrencyDecimalDigits);
                        strPriceFormat = "{0:";
                        for (int iq = 0; iq <= iQCount; iq++)
                        {
                            if (iq == 0)
                            {
                                strPriceFormat += "0.";
                            }
                            else
                            {
                                strPriceFormat += "0";
                            }

                        }
                        strPriceFormat += "}";
                    }
                    SessionHelper.PriceFormat = strPriceFormat;
                }
                else
                    SessionHelper.PriceFormat = strPriceFormat;

                string strTurnsAvgFormat = "{0:0}";
                if (!string.IsNullOrWhiteSpace(SessionHelper.NumberAvgDecimalPoints))
                {
                    if (Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.NumberAvgDecimalPoints) > 0)
                    {
                        int iQCount = Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.NumberAvgDecimalPoints);
                        strTurnsAvgFormat = "{0:";
                        for (int iq = 0; iq <= iQCount; iq++)
                        {
                            if (iq == 0)
                            {
                                strTurnsAvgFormat += "0.";
                            }
                            else
                            {
                                strTurnsAvgFormat += "0";
                            }

                        }
                        strTurnsAvgFormat += "}";
                    }
                    SessionHelper.TurnUsageFormat = strTurnsAvgFormat;
                }
                else
                    SessionHelper.TurnUsageFormat = strTurnsAvgFormat;

                string strWghtFormat = "{0:0}";
                if (!string.IsNullOrWhiteSpace(SessionHelper.WeightDecimalPoints))
                {
                    if (Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.WeightDecimalPoints) > 0)
                    {
                        int iQCount = Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.WeightDecimalPoints);
                        strWghtFormat = "{0:";
                        for (int iq = 0; iq <= iQCount; iq++)
                        {
                            if (iq == 0)
                            {
                                strWghtFormat += "0.";
                            }
                            else
                            {
                                strWghtFormat += "0";
                            }

                        }
                        strWghtFormat += "}";
                    }
                    SessionHelper.WeightFormat = strWghtFormat;
                }
                else
                    SessionHelper.WeightFormat = strWghtFormat;

                //SessionHelper.TurnUsageFormat = Convert.ToString(RegionInfo.TurnsAvgDecimalPoints);
                //SessionHelper.WeightDecimalPoints = Convert.ToString(RegionInfo.WeightDecimalPoints);

            }
            else
            {
                SessionHelper.RoomCulture = CultureInfo.CreateSpecificCulture("en-US");
                SessionHelper.CurrentTimeZone = TimeZoneInfo.Utc;
                SessionHelper.DateTimeFormat = "M/d/yyyy" + " " + "h:mm:ss tt";
                SessionHelper.RoomDateFormat = "M/d/yyyy";
                SessionHelper.NumberDecimalDigits = "0";
                SessionHelper.CurrencyDecimalDigits = "0";
                SessionHelper.NumberAvgDecimalPoints = "0";
                SessionHelper.WeightDecimalPoints = "0";
                SessionHelper.QuantityFormat = "{0:0}";
                SessionHelper.PriceFormat = "{0:0}";
                SessionHelper.TurnUsageFormat = "{0:0}";
                SessionHelper.WeightFormat = "{0:0}";

            }


            //if (SessionHelper.EnterPriceID > 0)
            //{
            //    List<EnterpriseDTO> lstAllEnterprise = new EnterpriseMasterDAL().GetAllEnterprise().Where(t => t.IsActive == true).ToList();
            //    SessionHelper.EnterPriseList = (from ssnel in SessionHelper.EnterPriseList
            //                                    join allep in lstAllEnterprise on ssnel.ID equals allep.ID
            //                                    select ssnel).OrderBy(t => t.Name).ToList();
            //    if (SessionHelper.EnterPriseList.Any())
            //    {
            //        SessionHelper.EnterPriceID = SessionHelper.EnterPriseList.First().ID;
            //        SessionHelper.EnterPriceName = SessionHelper.EnterPriseList.First().Name;
            //    }
            //}


            ResourceHelper.EnterpriseResourceFolder = SessionHelper.EnterPriceID.ToString();
            ResourceHelper.CompanyResourceFolder = SessionHelper.EnterPriceID.ToString() + @"\" + SessionHelper.CompanyID.ToString();
            ResourceHelper.RoomResourceFolder = SessionHelper.EnterPriceID.ToString() + @"\" + SessionHelper.CompanyID.ToString() + @"\" + SessionHelper.RoomID.ToString();
            EnterPriseResourceHelper.EnterpriseResourceFolder = SessionHelper.EnterPriceID.ToString();
            EnterPriseResourceHelper.CompanyResourceFolder = SessionHelper.EnterPriceID.ToString() + @"\CompanyResources";
            //ResourceHelper.CompanyResourceFolder = SessionHelper.CompanyID.ToString();
            if (eTurns.DTO.Resources.ResourceHelper.CurrentCult == null)
                eTurns.DTO.Resources.ResourceHelper.CurrentCult = System.Threading.Thread.CurrentThread.CurrentCulture;

            if (SessionHelper.CompanyID > 0)
            {
                //if (SessionHelper.CompanyConfig == null)
                //{
                //    CompanyConfigDAL objCompanyConfigDAL = new CompanyConfigDAL(SessionHelper.EnterPriseDBName);
                //    CompanyConfigDTO objCompanyConfigDTO = objCompanyConfigDAL.GetRecord(SessionHelper.CompanyID);
                //    if (objCompanyConfigDTO != null)
                //        SessionHelper.CompanyConfig = objCompanyConfigDTO;
                //}
                //else
                //{
                //    if (SessionHelper.CompanyConfig.CompanyID != SessionHelper.CompanyID)
                //    {
                //        CompanyConfigDAL objCompanyConfigDAL = new CompanyConfigDAL(SessionHelper.EnterPriseDBName);
                //        CompanyConfigDTO objCompanyConfigDTO = objCompanyConfigDAL.GetRecord(SessionHelper.CompanyID);
                //        if (objCompanyConfigDTO != null)
                //            SessionHelper.CompanyConfig = objCompanyConfigDTO;
                //    }
                //}

                string strQuantityFormat = "{0:0}";
                if (!string.IsNullOrWhiteSpace(SessionHelper.NumberDecimalDigits))
                {
                    if (Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.NumberDecimalDigits) > 0)
                    {
                        int iQCount = Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.NumberDecimalDigits);
                        strQuantityFormat = "{0:";
                        for (int iq = 0; iq <= iQCount; iq++)
                        {
                            if (iq == 0)
                            {
                                strQuantityFormat += "0.";
                            }
                            else
                            {
                                strQuantityFormat += "0";
                            }

                        }
                        strQuantityFormat += "}";
                    }
                    SessionHelper.QuantityFormat = strQuantityFormat;
                }
                else
                    SessionHelper.QuantityFormat = strQuantityFormat;

                string strPriceFormat = "{0:0}";
                if (!string.IsNullOrWhiteSpace(SessionHelper.CurrencyDecimalDigits))
                {
                    //if ((int)eTurnsWeb.Helper.SessionHelper.CompanyConfig.CurrencySymbol > 0)
                    if (Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.CurrencyDecimalDigits) > 0)
                    {
                        int iQCount = Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.CurrencyDecimalDigits);
                        strPriceFormat = "{0:";
                        for (int iq = 0; iq <= iQCount; iq++)
                        {
                            if (iq == 0)
                            {
                                strPriceFormat += "0.";
                            }
                            else
                            {
                                strPriceFormat += "0";
                            }

                        }
                        strPriceFormat += "}";
                    }
                    SessionHelper.PriceFormat = strPriceFormat;
                }
                else
                    SessionHelper.PriceFormat = strPriceFormat;

                string strTurnsAvgFormat = "{0:0}";
                if (!string.IsNullOrWhiteSpace(SessionHelper.NumberAvgDecimalPoints))
                {
                    if (Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.NumberAvgDecimalPoints) > 0)
                    {
                        int iQCount = Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.NumberAvgDecimalPoints);
                        strTurnsAvgFormat = "{0:";
                        for (int iq = 0; iq <= iQCount; iq++)
                        {
                            if (iq == 0)
                            {
                                strTurnsAvgFormat += "0.";
                            }
                            else
                            {
                                strTurnsAvgFormat += "0";
                            }

                        }
                        strTurnsAvgFormat += "}";
                    }
                    SessionHelper.TurnUsageFormat = strTurnsAvgFormat;
                }
                else
                    SessionHelper.TurnUsageFormat = strTurnsAvgFormat;

                string strWghtFormat = "{0:0}";
                if (!string.IsNullOrWhiteSpace(SessionHelper.WeightDecimalPoints))
                {
                    if (Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.WeightDecimalPoints) > 0)
                    {
                        int iQCount = Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.WeightDecimalPoints);
                        strWghtFormat = "{0:";
                        for (int iq = 0; iq <= iQCount; iq++)
                        {
                            if (iq == 0)
                            {
                                strWghtFormat += "0.";
                            }
                            else
                            {
                                strWghtFormat += "0";
                            }

                        }
                        strWghtFormat += "}";
                    }
                    SessionHelper.WeightFormat = strWghtFormat;
                }
                else
                    SessionHelper.WeightFormat = strWghtFormat;

            }
            UserLoginHistoryDTO objUserLoginHistory = new UserLoginHistoryDTO();
            objUserLoginHistory.CompanyId = SessionHelper.CompanyID;
            objUserLoginHistory.EnterpriseId = SessionHelper.EnterPriceID;
            objUserLoginHistory.EventDate = DateTimeUtility.DateTimeNow;
            switch (EventFiredOn)
            {
                case "onlogin":
                    objUserLoginHistory.EventType = 1;
                    break;
                case "enterprise":
                    objUserLoginHistory.EventType = 3;
                    break;
                case "company":
                    objUserLoginHistory.EventType = 4;
                    break;
                case "room":
                    objUserLoginHistory.EventType = 5;
                    break;
                default:
                    objUserLoginHistory.EventType = 0;
                    break;
            }
            objUserLoginHistory.IpAddress = string.Empty;
            objUserLoginHistory.CountryName = string.Empty;
            objUserLoginHistory.RegionName = string.Empty;
            objUserLoginHistory.CityName = string.Empty;
            objUserLoginHistory.ZipCode = string.Empty;
            objUserLoginHistory.ID = 0;
            if (HttpContext != null)
            {
                objUserLoginHistory.IpAddress = Request.UserHostAddress;
                //UserLoginHistoryDTO IPInfo = getLocationInfo(Request.UserHostAddress);
                UserLoginHistoryDTO IPInfo = new UserLoginHistoryDTO();
                if (IPInfo == null)
                {
                    IPInfo = new UserLoginHistoryDTO();
                }
                objUserLoginHistory.CountryName = IPInfo.CountryName;
                objUserLoginHistory.RegionName = IPInfo.RegionName;
                objUserLoginHistory.CityName = IPInfo.CityName;
                objUserLoginHistory.ZipCode = IPInfo.ZipCode;
            }
            objUserLoginHistory.RoomId = SessionHelper.RoomID;
            objUserLoginHistory.UserId = SessionHelper.UserID;


            objUserMasterDAL.SaveUserActions(objUserLoginHistory);

        }

        private UserLoginHistoryDTO getLocationInfo(string ipaddress)
        {
            try
            {
                //return null;
                if (Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["logiphist"]) == "1")
                {
                    if (ipaddress == "::1" || ipaddress == "127.0.0.1")
                    {
                        ipaddress = "14.141.36.210";
                    }
                    String url = String.Empty;
                    UserLoginHistoryDTO objUserLoginHistoryDTO = new UserLoginHistoryDTO();
                    if (!string.IsNullOrWhiteSpace(ipaddress))
                    {
                        if (ipaddress != "::1" && ipaddress != "127.0.0.1")
                        {
                            url = String.Format("http://freegeoip.net/xml/{0}", ipaddress.Trim());
                            XDocument xDoc = XDocument.Load(url);
                            if (xDoc == null | xDoc.Root == null)
                            {
                                return null;
                            }
                            else
                            {
                                objUserLoginHistoryDTO = (from b in xDoc.Descendants("Response")
                                                          select new UserLoginHistoryDTO
                                                          {
                                                              CountryName = b.Element("CountryName").Value,
                                                              RegionName = b.Element("RegionName").Value,
                                                              CityName = b.Element("City").Value,
                                                              ZipCode = b.Element("ZipCode").Value
                                                          }).FirstOrDefault();
                                return objUserLoginHistoryDTO;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {

                return null;
            }
            //return objUserLoginHistoryDTO;
        }

        public List<UserWiseRoomsAccessDetailsDTO> SetPermissionsForsuperadmin(List<UserWiseRoomsAccessDetailsDTO> lstRowpermission, long EnterpriseId, long CompanyId, long RoomId, long RoleId, long RoleType)
        {
            bool IsroomActv = false;
            if (SessionHelper.RoomList != null && SessionHelper.RoomList.Any(t => t.ID == RoomId))
            {
                IsroomActv = SessionHelper.RoomList.First(t => t.ID == RoomId).IsRoomActive;
            }
            List<UserWiseRoomsAccessDetailsDTO> lstConvertedpermission = lstRowpermission;
            long[] arr = new long[] { };
            lstRowpermission.ForEach(t =>
            {
                if (EnterpriseId > 0 && CompanyId > 0 && RoomId < 1)
                {
                    t.PermissionList.ForEach(it =>
                    {
                        if (RoleId == -1)
                        {
                            arr = new long[] { 41, 39, 52 };
                        }
                        else
                        {
                            arr = new long[] { 39, 52 };
                        }
                        if (arr.Contains(it.ModuleID))
                        {
                            it.IsView = true;
                            it.IsDelete = true;
                            it.IsInsert = true;
                            it.IsUpdate = true;
                            it.IsChecked = true;
                        }
                        else
                        {
                            it.IsView = false;
                            it.IsDelete = false;
                            it.IsInsert = false;
                            it.IsUpdate = false;
                            it.IsChecked = false;
                        }
                    });
                }
                else if (EnterpriseId > 0 && CompanyId < 1)
                {
                    t.PermissionList.ForEach(it =>
                    {
                        if (it.ModuleID == 39)
                        {
                            it.IsView = true;
                            it.IsDelete = true;
                            it.IsInsert = true;
                            it.IsUpdate = true;
                            it.IsChecked = true;
                        }
                        else
                        {
                            it.IsView = false;
                            it.IsDelete = false;
                            it.IsInsert = false;
                            it.IsUpdate = false;
                            it.IsChecked = false;
                        }
                    });
                }
                else if (EnterpriseId < 1)
                {
                    t.PermissionList.ForEach(it =>
                    {
                        if (RoleId == -1)
                        {
                            arr = new long[] { 41, 39, 58, 59 };
                        }
                        else
                        {
                            arr = new long[] { 39, 58, 59 };
                        }
                        if (arr.Contains(it.ModuleID))
                        {
                            it.IsView = true;
                            it.IsDelete = true;
                            it.IsInsert = true;
                            it.IsUpdate = true;
                            it.IsChecked = true;
                        }
                        else
                        {
                            it.IsView = false;
                            it.IsDelete = false;
                            it.IsInsert = false;
                            it.IsUpdate = false;
                            it.IsChecked = false;
                        }
                    });
                }
                else
                {
                    t.PermissionList.ForEach(it =>
                    {
                        it.IsView = true;
                        it.IsDelete = true;
                        it.IsInsert = true;
                        it.IsUpdate = true;
                        it.IsChecked = true;
                        if (it.ModuleID == (long)SessionHelper.ModuleList.PreventTransmittedOrdersFromDisplayingInRedCount)
                            it.IsChecked = false;
                    });

                }
            });
            List<UserWiseRoomsAccessDetailsDTO> lstSessoion = lstConvertedpermission;
            if (!IsroomActv)
            {
                long[] arrNotToSet = new long[] { 6, 61, 81, 114, 101, 72, 50 };
                if (lstSessoion != null && lstSessoion.Count > 0 && lstSessoion.Where(t => t.EnterpriseId == EnterpriseId && t.CompanyId == CompanyId && t.RoomID == RoomId).Any())
                {
                    UserWiseRoomsAccessDetailsDTO objUserWiseRoomsAccessDetailsDTO = lstSessoion.FirstOrDefault(t => t.EnterpriseId == EnterpriseId && t.CompanyId == CompanyId && t.RoomID == RoomId);
                    if (objUserWiseRoomsAccessDetailsDTO != null)
                    {
                        List<UserRoleModuleDetailsDTO> lstlocalpermissions = objUserWiseRoomsAccessDetailsDTO.PermissionList;
                        PermissionTemplateDAL objPermissionTemplateDAL = new PermissionTemplateDAL(SessionHelper.EnterPriseDBName);
                        List<UserRoleModuleDetailsDTO> lstInactiveRoomMap = objPermissionTemplateDAL.GetPermissionsByTemplateInactiveRoom();
                        lstlocalpermissions.ForEach(ob =>
                        {

                            UserRoleModuleDetailsDTO objInactiveUserRoleModuleDetailsDTO = lstInactiveRoomMap.FirstOrDefault(d => d.ModuleID == ob.ModuleID);
                            if (objInactiveUserRoleModuleDetailsDTO == null)
                            {
                                objInactiveUserRoleModuleDetailsDTO = new UserRoleModuleDetailsDTO();
                            }

                            ob.IsDelete = objInactiveUserRoleModuleDetailsDTO.IsDelete;
                            ob.IsInsert = objInactiveUserRoleModuleDetailsDTO.IsInsert;
                            ob.IsUpdate = objInactiveUserRoleModuleDetailsDTO.IsUpdate;
                            ob.IsChecked = objInactiveUserRoleModuleDetailsDTO.IsChecked;
                            ob.IsView = objInactiveUserRoleModuleDetailsDTO.IsView;
                            ob.ShowArchived = objInactiveUserRoleModuleDetailsDTO.ShowArchived;
                            ob.ShowDeleted = objInactiveUserRoleModuleDetailsDTO.ShowDeleted;
                            ob.ShowUDF = objInactiveUserRoleModuleDetailsDTO.ShowUDF;
                        });
                        objUserWiseRoomsAccessDetailsDTO.PermissionList = lstlocalpermissions;
                        lstSessoion = lstSessoion.Where(t => t.EnterpriseId == EnterpriseId && t.CompanyId == CompanyId && t.RoomID != RoomId).ToList();
                        lstSessoion.Add(objUserWiseRoomsAccessDetailsDTO);

                        //if (objUserWiseRoomsAccessDetailsDTO.PermissionList.Any(t => t.ModuleID == 99))
                        //{
                        //    UserRoleModuleDetailsDTO objUserRoleModuleDetailsDTO = objUserWiseRoomsAccessDetailsDTO.PermissionList.FirstOrDefault(t => t.ModuleID == 99);
                        //    long usersupplier = 0;
                        //    long.TryParse(objUserRoleModuleDetailsDTO.ModuleValue, out usersupplier);
                        //    UserSupplierID = usersupplier;
                        //}
                    }

                }
            }
            lstConvertedpermission = lstSessoion;
            return lstConvertedpermission;
        }


        #endregion

        #region [User Profile]


        public ActionResult MyProfile()
        {
            UserMasterDTO objUserMasterDTO = new UserMasterDTO();
            eTurnsMaster.DAL.UserMasterDAL objUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
            objUserMasterDTO = objUserMasterDAL.GetRecord(SessionHelper.UserID);
            return View(objUserMasterDTO);
        }

        [HttpPost]
        public JsonResult MyProfile(UserMasterDTO objDTO)
        {
            string message = "";
            string status = "";
            eTurnsMaster.DAL.UserMasterDAL obj = new eTurnsMaster.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
            CommonMasterDAL objCDAL = new CommonMasterDAL();
            if (string.IsNullOrEmpty(objDTO.UserName))
            {
                message = "User name is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(objDTO.UserName))
            {
                message = "User name is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            objDTO.LastUpdatedBy = SessionHelper.UserID;


            string strOK = objCDAL.UserDuplicateCheckUserName(objDTO.ID, objDTO.UserName);
            if (strOK == "duplicate")
            {
                message = "User name \"" + objDTO.UserName + "\" already exist! Try with Another!";
                status = "duplicate";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                UserMasterDTO objUserMasterDTO = obj.EditProfile(objDTO, false);
                if (objUserMasterDTO.ID > 0)
                {
                    if (objUserMasterDTO.UserType != 1)
                    {
                        eTurns.DAL.UserMasterDAL objUserMasterDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                        objUserMasterDAL.EditProfile(objUserMasterDTO, false);
                    }
                    message = "Record Saved Sucessfully...";
                    status = "ok";
                    Session["SelectedRoomsPermission"] = null;
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    message = "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                    status = "fail";
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                }

            }
        }
        #endregion

        #region [Region Settings]

        public ActionResult RegionalInfoSettings()
        {
            ViewBag.Cultures = RegionHelper.GetAllCultures();
            ViewBag.DatePatterns = new List<RegionLanguage>();
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
            eTurnsRegionInfo objeTurnsRegionInfo = objRegionSettingDAL.GetRegionSettingsById(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
            return View(objeTurnsRegionInfo);
        }

        [HttpPost]
        public ActionResult GetLocaleInfo(string CultureCode)
        {
            eTurnsRegionInfo objeTurnsRegionInfo = RegionHelper.GetFormates(CultureCode);
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
            eTurnsRegionInfo DBRegionInfo = objRegionSettingDAL.GetRegionSettingsById(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
            if (DBRegionInfo != null && CultureCode == DBRegionInfo.CultureCode)
            {
                objeTurnsRegionInfo.ShortDatePattern = DBRegionInfo.ShortDatePattern;
                objeTurnsRegionInfo.ShortTimePattern = DBRegionInfo.ShortTimePattern;
                objeTurnsRegionInfo.NumberDecimalDigits = DBRegionInfo.NumberDecimalDigits;
                objeTurnsRegionInfo.CurrencyDecimalDigits = DBRegionInfo.CurrencyDecimalDigits;
            }
            return Json(objeTurnsRegionInfo);
        }

        [HttpPost]
        public JsonResult SaveRegionalSettings(eTurnsRegionInfo objeTurnsRegionInfo)
        {
            string message = string.Empty;
            string status = string.Empty;
            try
            {
                eTurnsRegionInfo OldRegionInfo = new eTurnsRegionInfo();
                NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
                TimeZoneInfo objtz = TimeZoneInfo.FindSystemTimeZoneById(objeTurnsRegionInfo.TimeZoneName);
                //objtz.BaseUtcOffset.Minutes;
                DateTime? dtstart = null;
                DateTime? dtEnd = null;
                if (objtz.SupportsDaylightSavingTime)
                {
                    TimeZoneInfo.AdjustmentRule objAdjustmentRule = objtz.GetAdjustmentRuleForYear(DateTime.UtcNow.Year);
                    if (objAdjustmentRule != null)
                    {
                        dtstart = objAdjustmentRule.GetDaylightTransitionStartForYear(DateTime.UtcNow.Year);
                        dtEnd = objAdjustmentRule.GetDaylightTransitionEndForYear(DateTime.UtcNow.Year);
                    }
                }
                objeTurnsRegionInfo.DayLightStartTime = dtstart;
                objeTurnsRegionInfo.DayLightEndTime = dtEnd;

                //TimeZoneInfo.AdjustmentRule objAdjustmentRule = DatetimeHelper.GetAdjustmentRuleForYear(objtz, DateTime.Now.Year);



                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
                OldRegionInfo = objRegionSettingDAL.GetRegionSettingsById(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);

                objeTurnsRegionInfo.TimeZoneOffSet = Convert.ToInt32(objtz.BaseUtcOffset.TotalMinutes);
                objeTurnsRegionInfo.EnterpriseId = SessionHelper.EnterPriceID;
                objeTurnsRegionInfo.CompanyId = SessionHelper.CompanyID;
                objeTurnsRegionInfo.RoomId = SessionHelper.RoomID;
                objeTurnsRegionInfo.TZSupportDayLight = objtz.SupportsDaylightSavingTime;
                objeTurnsRegionInfo = objRegionSettingDAL.SaveRegionalSettings(objeTurnsRegionInfo);
                CultureInfo roomculture = CultureInfo.CreateSpecificCulture(objeTurnsRegionInfo.CultureCode);
                TimeZoneInfo roomTimeZone = TimeZoneInfo.FindSystemTimeZoneById(objeTurnsRegionInfo.TimeZoneName);
                TimeZoneInfo.AdjustmentRule[] adjustments = objtz.GetAdjustmentRules();
                SessionHelper.CurrentTimeZone = roomTimeZone;
                SessionHelper.RoomCulture = roomculture;
                SessionHelper.DateTimeFormat = objeTurnsRegionInfo.ShortDatePattern + " " + objeTurnsRegionInfo.ShortTimePattern;
                SessionHelper.RoomDateFormat = objeTurnsRegionInfo.ShortDatePattern;
                SessionHelper.CurrencyDecimalDigits = Convert.ToString(objeTurnsRegionInfo.CurrencyDecimalDigits);
                SessionHelper.NumberDecimalDigits = Convert.ToString(objeTurnsRegionInfo.NumberDecimalDigits);
                SessionHelper.WeightDecimalPoints = Convert.ToString(objeTurnsRegionInfo.WeightDecimalPoints);
                SessionHelper.NumberAvgDecimalPoints = Convert.ToString(objeTurnsRegionInfo.TurnsAvgDecimalPoints);

                string strQuantityFormat = "{0:0}";
                if (!string.IsNullOrWhiteSpace(SessionHelper.NumberDecimalDigits))
                {
                    if (Convert.ToInt32(Convert.ToString(eTurnsWeb.Helper.SessionHelper.NumberDecimalDigits)) > 0)
                    {
                        int iQCount = Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.NumberDecimalDigits);
                        strQuantityFormat = "{0:";
                        for (int iq = 0; iq <= iQCount; iq++)
                        {
                            if (iq == 0)
                            {
                                strQuantityFormat += "0.";
                            }
                            else
                            {
                                strQuantityFormat += "0";
                            }

                        }
                        strQuantityFormat += "}";
                    }
                    SessionHelper.QuantityFormat = strQuantityFormat;
                }
                else
                    SessionHelper.QuantityFormat = strQuantityFormat;

                string strPriceFormat = "{0:0}";
                if (!string.IsNullOrWhiteSpace(SessionHelper.CurrencyDecimalDigits))
                {
                    //if ((int)eTurnsWeb.Helper.SessionHelper.CompanyConfig.CurrencySymbol > 0)
                    if (Convert.ToInt32(Convert.ToString(eTurnsWeb.Helper.SessionHelper.CurrencyDecimalDigits)) > 0)
                    {
                        int iQCount = Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.CurrencyDecimalDigits);
                        strPriceFormat = "{0:";
                        for (int iq = 0; iq <= iQCount; iq++)
                        {
                            if (iq == 0)
                            {
                                strPriceFormat += "0.";
                            }
                            else
                            {
                                strPriceFormat += "0";
                            }

                        }
                        strPriceFormat += "}";
                    }
                    SessionHelper.PriceFormat = strPriceFormat;
                }
                else
                    SessionHelper.PriceFormat = strPriceFormat;

                string strTurnsAvgFormat = "{0:0}";
                if (!string.IsNullOrWhiteSpace(SessionHelper.NumberAvgDecimalPoints))
                {
                    if (Convert.ToInt32(Convert.ToString(eTurnsWeb.Helper.SessionHelper.NumberAvgDecimalPoints)) > 0)
                    {
                        int iQCount = Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.NumberAvgDecimalPoints);
                        strTurnsAvgFormat = "{0:";
                        for (int iq = 0; iq <= iQCount; iq++)
                        {
                            if (iq == 0)
                            {
                                strTurnsAvgFormat += "0.";
                            }
                            else
                            {
                                strTurnsAvgFormat += "0";
                            }

                        }
                        strTurnsAvgFormat += "}";
                    }
                    SessionHelper.TurnUsageFormat = strTurnsAvgFormat;
                }
                else
                    SessionHelper.TurnUsageFormat = strTurnsAvgFormat;

                string strWghtFormat = "{0:0}";

                if (!string.IsNullOrWhiteSpace(SessionHelper.WeightDecimalPoints))
                {
                    if (Convert.ToInt32(Convert.ToString(eTurnsWeb.Helper.SessionHelper.WeightDecimalPoints)) > 0)
                    {
                        int iQCount = Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.WeightDecimalPoints);
                        strWghtFormat = "{0:";
                        for (int iq = 0; iq <= iQCount; iq++)
                        {
                            if (iq == 0)
                            {
                                strWghtFormat += "0.";
                            }
                            else
                            {
                                strWghtFormat += "0";
                            }

                        }
                        strWghtFormat += "}";
                    }
                    SessionHelper.WeightFormat = strWghtFormat;
                }
                else
                    SessionHelper.WeightFormat = strWghtFormat;

                if (OldRegionInfo != null)
                {
                    TimeZoneInfo roomOldTimeZone = TimeZoneInfo.FindSystemTimeZoneById(OldRegionInfo.TimeZoneName);
                    if (OldRegionInfo.TimeZoneName != objeTurnsRegionInfo.TimeZoneName)
                    {
                        objNotificationDAL.UpdateAllScheduleAfterTZChange(roomOldTimeZone, roomTimeZone, SessionHelper.RoomID, SessionHelper.CompanyID);
                    }
                }
                message = ResMessage.SaveMessage;
                status = "ok";
                SessionHelper.RoomID = SessionHelper.RoomID;
                return Json(new { Message = message, Status = status, retDto = objeTurnsRegionInfo });
            }
            catch (Exception)
            {
                message = "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region [Other Methods]


        public string GenerateScript()
        {
            Server srv = new Server("192.168.3.102\\sql2012");

            // Reference the database.  
            Database db = srv.Databases[DbConHelper.GetETurnsDBName()];

            Scripter scrp = new Scripter(srv);
            scrp.Options.IncludeIfNotExists = true;
            scrp.Options.ScriptDrops = false;
            scrp.Options.IncludeHeaders = false;
            scrp.Options.ScriptSchema = true;
            scrp.Options.WithDependencies = false;
            scrp.Options.Indexes = true;   // To include indexes
            scrp.Options.DriAllConstraints = true;   // to include referential constraints in the script
            scrp.Options.Triggers = true;
            scrp.Options.FullTextIndexes = true;
            scrp.PrefetchObjects = true; // some sources suggest this may speed things up

            var urns = new List<Urn>();

            // Iterate through the tables in database and script each one   
            foreach (Table tb in db.Tables)
            {
                // check if the table is not a system table
                if (tb.IsSystemObject == false)
                {
                    urns.Add(tb.Urn);
                }
            }

            // Iterate through the views in database and script each one. Display the script.   
            foreach (View view in db.Views)
            {
                // check if the view is not a system object
                if (view.IsSystemObject == false)
                {
                    urns.Add(view.Urn);
                }
            }

            // Iterate through the stored procedures in database and script each one. Display the script.   
            foreach (StoredProcedure sp in db.StoredProcedures)
            {
                // check if the procedure is not a system object
                if (sp.IsSystemObject == false)
                {
                    urns.Add(sp.Urn);
                }
            }

            StringBuilder builder = new StringBuilder();
            System.Collections.Specialized.StringCollection sc = scrp.Script(urns.ToArray());
            foreach (string st in sc)
            {
                // It seems each string is a sensible batch, and putting GO after it makes it work in tools like SSMS.
                // Wrapping each string in an 'exec' statement would work better if using SqlCommand to run the script.
                builder.AppendLine(st);
                //builder.AppendLine("GO");
            }
            string strscr = builder.ToString();
            return strscr;
        }

        [HttpPost]
        public ActionResult ClearUserListSession()
        {
            Session["lstAllUsers"] = null;
            Session["AllUserPermissions"] = null;
            return Json(true);
        }
        [HttpPost]
        public ActionResult ClearRoleListSession()
        {
            Session["lstAllRoles"] = null;
            Session["AllRolePermissions"] = null;

            return Json(true);
        }
        [HttpPost]
        public ActionResult ClearSelectedRoleListSession()
        {
            Session["SelectedRoomsPermission"] = null;

            return Json(true);
        }


        [HttpPost]
        public ActionResult ClearCompanyListSession()
        {
            Session["AllCompanies"] = null;
            return Json(true);
        }

        [HttpPost]
        public ActionResult ClearRoomListSession()
        {
            Session["AllRooms"] = null;
            return Json(true);
        }

        [HttpPost]
        public JsonResult GetSuppliersByRoom(long? RoomId, long? CompanyId)
        {
            SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
            List<SupplierMasterDTO> lstSuplliers = objSupplierMasterDAL.GetAllRecords(RoomId ?? 0, CompanyId ?? 0, false, false, false).ToList();
            lstSuplliers = lstSuplliers.Where(t => t.SupplierName != null && t.IsArchived == false && t.IsDeleted == false).OrderBy(t => t.SupplierName).ToList();
            return Json(lstSuplliers);
        }

        #endregion

        #region [Dashboard]

        public ActionResult Dashboard1()
        {
            return View();
        }

        public JsonResult GenerateDashBoard()
        {
            string strHTML = string.Empty;
            string dvMaster = string.Empty;
            string dvAuthentication = string.Empty;
            string dvAssets = string.Empty;
            string dvReports = string.Empty;

            string dvInventry = string.Empty;
            string dvConsume = string.Empty;
            string dvReplenish = string.Empty;
            string dvReceive = string.Empty;
            string dvKits = string.Empty;
            string dvConfiguration = string.Empty;

            //string dvCategoryCost = string.Empty;
            //Inventory
            //Consume
            //Replenish
            //Receive
            //Kits
            //Configuration

            string LeftDiv = string.Empty;
            string RightDiv = string.Empty;
            ClickFromMenuNotification(false);
            if (eTurnsWeb.Helper.SessionHelper.RoomPermissions != null)
            {

                eTurns.DTO.UserWiseRoomsAccessDetailsDTO lstPermission = eTurnsWeb.Helper.SessionHelper.RoomPermissions.Find(element => element.EnterpriseId == eTurnsWeb.Helper.SessionHelper.EnterPriceID && element.CompanyId == SessionHelper.CompanyID && element.RoomID == SessionHelper.RoomID);

                //ModuleMasterController obj = new ModuleMasterController();
                eTurns.DAL.ModuleMasterDAL obj = new eTurns.DAL.ModuleMasterDAL(SessionHelper.EnterPriseDBName);
                eTurnsMaster.DAL.ModuleMasterDAL objModuleMasterDAL = new eTurnsMaster.DAL.ModuleMasterDAL(SessionHelper.EnterPriseDBName);
                List<ParentModuleMasterDTO> lstmasterModule = new List<ParentModuleMasterDTO>();
                if (SessionHelper.EnterPriceID < 1)
                {
                    lstmasterModule = objModuleMasterDAL.GetParentRecord().ToList();
                }
                else
                {
                    lstmasterModule = obj.GetParentRecord().ToList();
                }

                StringBuilder str = new StringBuilder();


                #region  UserType 1 -- Supper Admin
                if (SessionHelper.UserType == 1 || SessionHelper.UserType == 2)
                {
                    //bool OlyEnterPrice = false;
                    if (SessionHelper.EnterPriseList != null && SessionHelper.EnterPriseList.Count > 0)
                    {
                        if (SessionHelper.CompanyList != null && SessionHelper.CompanyList.Count > 0)
                        {
                            if (SessionHelper.RoomID > 0)
                            {
                                foreach (ParentModuleMasterDTO item in lstmasterModule)
                                {
                                    if (item.ID != Convert.ToInt64(SessionHelper.ParentModuleList.Master))
                                    {
                                        if (lstPermission != null)
                                        {
                                            List<eTurns.DTO.UserRoleModuleDetailsDTO> objChild = (from m in lstPermission.PermissionList
                                                                                                  where m.ParentID == Convert.ToInt32(item.ID)
                                                                                                  select m).OrderBy(c => c.ModuleName).ToList();


                                            if (objChild != null)
                                            {
                                                objChild = objChild.Where(c => c.ModuleID != (Int64)SessionHelper.ModuleList.ResetAutoNumbers && c.ModuleID != (Int64)SessionHelper.ModuleList.Synch && c.ModuleID != (Int64)SessionHelper.ModuleList.Barcode).ToList();
                                                if (objChild.Count > 0)
                                                {
                                                    str.Append("<div style=\"float: left;width:100%;\">");
                                                    int dvcnt = 0;
                                                    foreach (var Childitem in objChild)
                                                    {
                                                        if (Childitem.ModuleID != (Int64)SessionHelper.ModuleList.Barcode && Childitem.ModuleID != (Int64)SessionHelper.ModuleList.ResetAutoNumbers && Childitem.ModuleID != (Int64)SessionHelper.ModuleList.Synch)
                                                        {
                                                            string ModuleName = SessionHelper.GetModuleName(Convert.ToInt32(Childitem.ModuleID));
                                                            string TotalNotification = obj.GetModuleNotification(Convert.ToInt32(Childitem.ModuleID), SessionHelper.RoomID, SessionHelper.UserID);
                                                            if (Convert.ToInt32(Childitem.ModuleID) == (int)eTurnsWeb.Helper.SessionHelper.ModuleList.Kits)
                                                            {
                                                                TotalNotification = new KitController().GetWIPKitCountForRedCircle();
                                                            }
                                                            TotalNotification = TotalNotification == null ? "0" : TotalNotification;

                                                            str.Append("<div class=\"imgclass\">");
                                                            str.Append("<a href=\"" + Childitem.ModuleURL + "\" runat=\"server\" title=\"" + ModuleName + "\">");
                                                            str.Append("<img src=\"../Content/images/" + Childitem.ImageName + "\"  class=\"imgHoverable\" alt=\"" + "" + "\"  />" + (TotalNotification != "0" ? "<span>" + TotalNotification + "</span>" : "") + "<br />" + ModuleName + "</a>");
                                                            str.Append("</div>");

                                                            dvcnt = dvcnt + 1;
                                                            if (dvcnt == 5)
                                                            {
                                                                str.Append("</div>");
                                                                str.Append("<div style=\"float: left;width:100%;\">");
                                                                dvcnt = 0;
                                                            }
                                                        }
                                                    }
                                                    str.Append("</div>");

                                                    string strWidgetTitle = string.Empty;
                                                    if (Convert.ToInt64(SessionHelper.ParentModuleList.Authentication) == item.ID)// Bind Athentication Modules
                                                    {
                                                        strWidgetTitle = ResDashboard.Authentication;
                                                        dvAuthentication = "<div id=\"Authentication\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                                                                   "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                                                                   "</div><div class=\"portlet-content\" ><div id=\"dvAuthentication\">" + str.ToString() + "</div></div></div>";
                                                    }
                                                    if (Convert.ToInt64(SessionHelper.ParentModuleList.Master) == item.ID)// Bind Master Module
                                                    {
                                                        strWidgetTitle = ResDashboard.Masters;
                                                        dvMaster = "<div id=\"Master\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                                                                   "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                                                                   "</div><div class=\"portlet-content\" ><div id=\"dvMaster\">" + str.ToString() + "</div></div></div>";
                                                    }
                                                    if (Convert.ToInt64(SessionHelper.ParentModuleList.Assets) == item.ID)// Bind Assets Module
                                                    {
                                                        strWidgetTitle = ResDashboard.Assets;
                                                        dvAssets = "<div id=\"Assets\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                                                                   "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                                                                   "</div><div class=\"portlet-content\" ><div id=\"dvAssets\">" + RenderRazorPartialViewToString("ToolsAssetsDashboard") + "</div></div></div>";
                                                    }
                                                    if (Convert.ToInt64(SessionHelper.ParentModuleList.Reports) == item.ID)// Bind Reports Module
                                                    {
                                                        strWidgetTitle = ResDashboard.Reports;
                                                        dvReports = "<div id=\"Reports\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                                                                   "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                                                                   "</div><div class=\"portlet-content\" ><div id=\"dvReports\">" + str.ToString() + "</div></div></div>";
                                                    }
                                                    if (Convert.ToInt64(SessionHelper.ParentModuleList.Inventory) == item.ID)// Bind Reports Module
                                                    {
                                                        strWidgetTitle = ResDashboard.Inventry;
                                                        dvInventry = "<div id=\"Inventory\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                                                                   "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                                                                   "</div><div class=\"portlet-content\" ><div id=\"dvInventry\">" + RenderRazorPartialViewToString("InventoryDashboard") + "</div></div></div>";
                                                    }
                                                    if (Convert.ToInt64(SessionHelper.ParentModuleList.Consume) == item.ID)// Bind Reports Module
                                                    {
                                                        strWidgetTitle = ResDashboard.Consume;
                                                        dvConsume = "<div id=\"Consume\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                                                                   "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                                                                   "</div><div class=\"portlet-content\" ><div id=\"dvConsume\">" + RenderRazorPartialViewToString("ConsumeDashboard") + "</div></div></div>";
                                                    }
                                                    if (Convert.ToInt64(SessionHelper.ParentModuleList.Replenish) == item.ID)// Bind Reports Module
                                                    {
                                                        strWidgetTitle = ResDashboard.Replenish;
                                                        dvReplenish = "<div id=\"Replenish\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                                                                   "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                                                                   "</div><div class=\"portlet-content\" ><div id=\"dvReplenish\">" + RenderRazorPartialViewToString("ReplenishDashboard") + "</div></div></div>";
                                                    }
                                                    if (Convert.ToInt64(SessionHelper.ParentModuleList.Receive) == item.ID)// Bind Reports Module
                                                    {
                                                        strWidgetTitle = ResDashboard.Receive;
                                                        dvReceive = "<div id=\"Receive\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                                                                   "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                                                                   "</div><div class=\"portlet-content\" ><div id=\"dvReceive\">" + str.ToString() + "</div></div></div>";
                                                    }
                                                    if (Convert.ToInt64(SessionHelper.ParentModuleList.Kits) == item.ID)// Bind Reports Module
                                                    {
                                                        strWidgetTitle = ResDashboard.Kits;
                                                        dvKits = "<div id=\"Kits\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                                                                   "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                                                                   "</div><div class=\"portlet-content\" ><div id=\"dvKits\">" + str.ToString() + "</div></div></div>";
                                                    }
                                                    if (Convert.ToInt64(SessionHelper.ParentModuleList.Configuration) == item.ID)// Bind Reports Module
                                                    {
                                                        strWidgetTitle = ResDashboard.Configuration;
                                                        dvConfiguration = "<div id=\"Configuration\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                                                                   "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                                                                   "</div><div class=\"portlet-content\" ><div id=\"dvConfiguration\">" + str.ToString() + "</div></div></div>";
                                                    }

                                                    str.Clear();
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                            else
                            {
                                foreach (ParentModuleMasterDTO item in lstmasterModule)
                                {
                                    if (lstPermission != null)
                                    {
                                        List<eTurns.DTO.UserRoleModuleDetailsDTO> objChild = (from m in lstPermission.PermissionList
                                                                                              where m.ParentID == Convert.ToInt32(item.ID)
                                                                                              select m).OrderBy(c => c.ModuleName).ToList();
                                        if (objChild != null)
                                        {
                                            if (objChild.Count > 0)
                                            {
                                                str.Append("<div style=\"float: left;width:100%;\">");
                                                int dvcnt = 0;
                                                foreach (var Childitem in objChild)
                                                {
                                                    if (Childitem.ModuleID == (Int64)SessionHelper.ModuleList.EnterpriseMaster || Childitem.ModuleID == (Int64)SessionHelper.ModuleList.CompanyMaster || Childitem.ModuleID == (Int64)SessionHelper.ModuleList.RoomMaster)
                                                    {
                                                        string ModuleName = SessionHelper.GetModuleName(Convert.ToInt32(Childitem.ModuleID));
                                                        string TotalNotification = obj.GetModuleNotification(Convert.ToInt32(Childitem.ModuleID), SessionHelper.RoomID, SessionHelper.UserID);
                                                        if (Convert.ToInt32(Childitem.ModuleID) == (int)eTurnsWeb.Helper.SessionHelper.ModuleList.Kits)
                                                        {
                                                            TotalNotification = new KitController().GetWIPKitCountForRedCircle();
                                                        }

                                                        TotalNotification = TotalNotification == null ? "0" : TotalNotification;

                                                        str.Append("<div class=\"imgclass\">");
                                                        str.Append("<a href=\"" + Childitem.ModuleURL + "\" runat=\"server\" title=\"" + ModuleName + "\">");
                                                        str.Append("<img src=\"../Content/images/" + Childitem.ImageName + "\"  class=\"imgHoverable\" alt=\"" + "" + "\"  />" + (TotalNotification != "0" ? "<span>" + TotalNotification + "</span>" : "") + "<br />" + ModuleName + "</a>");
                                                        str.Append("</div>");

                                                        dvcnt = dvcnt + 1;
                                                        if (dvcnt == 5)
                                                        {
                                                            str.Append("</div>");
                                                            str.Append("<div style=\"float: left;width:100%;\">");
                                                            dvcnt = 0;
                                                        }
                                                    }

                                                }
                                                str.Append("</div>");

                                                string strWidgetTitle = string.Empty;
                                                if (Convert.ToInt64(SessionHelper.ParentModuleList.Authentication) == item.ID)// Bind Athentication Modules
                                                {
                                                    strWidgetTitle = ResDashboard.Authentication;
                                                    dvAuthentication = "<div id=\"Authentication\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                                                               "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                                                               "</div><div class=\"portlet-content\" ><div id=\"dvAuthentication\">" + str.ToString() + "</div></div></div>";
                                                }
                                                //if (Convert.ToInt64(SessionHelper.ParentModuleList.Master) == item.ID)// Bind Master Module
                                                //{
                                                //    strWidgetTitle = ResDashboard.Masters;
                                                //    dvMaster = "<div id=\"Master\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                                                //               "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                                                //               "</div><div class=\"portlet-content\" ><div id=\"dvMaster\">" + str.ToString() + "</div></div></div>";
                                                //}
                                                str.Clear();
                                            }
                                        }
                                    }
                                }
                            }

                        }
                        else
                        {
                            foreach (ParentModuleMasterDTO item in lstmasterModule)
                            {
                                if (lstPermission != null)
                                {
                                    List<eTurns.DTO.UserRoleModuleDetailsDTO> objChild = (from m in lstPermission.PermissionList
                                                                                          where m.ParentID == Convert.ToInt32(item.ID)
                                                                                          select m).OrderBy(c => c.ModuleName).ToList();
                                    if (objChild != null)
                                    {
                                        if (objChild.Count > 0)
                                        {
                                            str.Append("<div style=\"float: left;width:100%;\">");
                                            int dvcnt = 0;
                                            foreach (var Childitem in objChild)
                                            {
                                                if (Childitem.ModuleID == (Int64)SessionHelper.ModuleList.EnterpriseMaster || Childitem.ModuleID == (Int64)SessionHelper.ModuleList.CompanyMaster)
                                                {
                                                    string ModuleName = SessionHelper.GetModuleName(Convert.ToInt32(Childitem.ModuleID));
                                                    string TotalNotification = obj.GetModuleNotification(Convert.ToInt32(Childitem.ModuleID), SessionHelper.RoomID, SessionHelper.UserID);
                                                    if (Convert.ToInt32(Childitem.ModuleID) == (int)eTurnsWeb.Helper.SessionHelper.ModuleList.Kits)
                                                    {
                                                        TotalNotification = new KitController().GetWIPKitCountForRedCircle();
                                                    }
                                                    TotalNotification = TotalNotification == null ? "0" : TotalNotification;

                                                    str.Append("<div class=\"imgclass\">");
                                                    str.Append("<a href=\"" + Childitem.ModuleURL + "\" runat=\"server\" title=\"" + ModuleName + "\">");
                                                    str.Append("<img src=\"../Content/images/" + Childitem.ImageName + "\"  class=\"imgHoverable\" alt=\"" + "" + "\"  />" + (TotalNotification != "0" ? "<span>" + TotalNotification + "</span>" : "") + "<br />" + ModuleName + "</a>");
                                                    str.Append("</div>");

                                                    dvcnt = dvcnt + 1;
                                                    if (dvcnt == 5)
                                                    {
                                                        str.Append("</div>");
                                                        str.Append("<div style=\"float: left;width:100%;\">");
                                                        dvcnt = 0;
                                                    }
                                                }

                                            }
                                            str.Append("</div>");

                                            string strWidgetTitle = string.Empty;
                                            if (Convert.ToInt64(SessionHelper.ParentModuleList.Authentication) == item.ID)// Bind Athentication Modules
                                            {
                                                strWidgetTitle = ResDashboard.Authentication;
                                                dvAuthentication = "<div id=\"Authentication\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                                                           "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                                                           "</div><div class=\"portlet-content\" ><div id=\"dvAuthentication\">" + str.ToString() + "</div></div></div>";
                                            }
                                            str.Clear();
                                        }
                                    }
                                }
                            }
                        }

                    }
                    else
                    {
                        foreach (ParentModuleMasterDTO item in lstmasterModule)
                        {
                            if (lstPermission != null)
                            {
                                List<eTurns.DTO.UserRoleModuleDetailsDTO> objChild = (from m in lstPermission.PermissionList
                                                                                      where m.ParentID == Convert.ToInt32(item.ID)
                                                                                      select m).OrderBy(c => c.ModuleName).ToList();
                                if (objChild != null)
                                {
                                    if (objChild.Count > 0)
                                    {
                                        str.Append("<div style=\"float: left;width:100%;\">");
                                        int dvcnt = 0;
                                        foreach (var Childitem in objChild)
                                        {
                                            if (Childitem.ModuleID == (Int64)SessionHelper.ModuleList.EnterpriseMaster)
                                            {
                                                string ModuleName = SessionHelper.GetModuleName(Convert.ToInt32(Childitem.ModuleID));
                                                string TotalNotification = string.Empty;
                                                if (SessionHelper.EnterPriceID > 0)
                                                {
                                                    TotalNotification = obj.GetModuleNotification(Convert.ToInt32(Childitem.ModuleID), SessionHelper.RoomID, SessionHelper.UserID);
                                                }

                                                TotalNotification = string.IsNullOrEmpty(TotalNotification) ? "0" : TotalNotification;

                                                str.Append("<div class=\"imgclass\">");
                                                str.Append("<a href=\"" + Childitem.ModuleURL + "\" runat=\"server\" title=\"" + ModuleName + "\">");
                                                str.Append("<img src=\"../Content/images/" + Childitem.ImageName + "\"  class=\"imgHoverable\" alt=\"" + "" + "\"  />" + (TotalNotification != "0" ? "<span>" + TotalNotification + "</span>" : "") + "<br />" + ModuleName + "</a>");
                                                str.Append("</div>");

                                                dvcnt = dvcnt + 1;
                                                if (dvcnt == 5)
                                                {
                                                    str.Append("</div>");
                                                    str.Append("<div style=\"float: left;width:100%;\">");
                                                    dvcnt = 0;
                                                }
                                            }

                                        }
                                        str.Append("</div>");

                                        string strWidgetTitle = string.Empty;
                                        if (Convert.ToInt64(SessionHelper.ParentModuleList.Authentication) == item.ID)// Bind Athentication Modules
                                        {
                                            strWidgetTitle = ResDashboard.Authentication;
                                            dvAuthentication = "<div id=\"Authentication\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                                                       "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                                                       "</div><div class=\"portlet-content\" ><div id=\"dvAuthentication\">" + str.ToString() + "</div></div></div>";
                                        }
                                        str.Clear();
                                    }
                                }
                            }
                        }
                    }

                }
                #endregion
                //#region  UserType 2 --Company Admin
                //else if (SessionHelper.UserType == 2)
                //{
                //    foreach (ParentModuleMasterDTO item in lstmasterModule)
                //    {
                //        List<eTurns.DTO.UserRoleModuleDetailsDTO> objChild = (from m in lstPermission.PermissionList
                //                                                              where m.ParentID == Convert.ToInt32(item.ID)
                //                                                              select m).OrderBy(c => c.ModuleName).ToList();
                //        if (objChild != null)
                //        {
                //            if (objChild.Count > 0)
                //            {
                //                str.Append("<div style=\"float: left;width:100%;\">");
                //                int dvcnt = 0;
                //                foreach (var Childitem in objChild)
                //                {
                //                    string ModuleName = SessionHelper.GetModuleName(Convert.ToInt32(Childitem.ModuleID));
                //                    string TotalNotification = obj.GetModuleNotification(Convert.ToInt32(Childitem.ModuleID), SessionHelper.RoomID, SessionHelper.UserID);
                //                    TotalNotification = TotalNotification == null ? "0" : TotalNotification;

                //                    str.Append("<div class=\"imgclass\">");
                //                    str.Append("<a href=\"" + Childitem.ModuleURL + "\" runat=\"server\" title=\"" + ModuleName + "\">");
                //                    str.Append("<img src=\"../Content/images/" + Childitem.ImageName + "\"  class=\"imgHoverable\" alt=\"" + "" + "\"  />" + (TotalNotification != "0" ? "<span>" + TotalNotification + "</span>" : "") + "<br />" + ModuleName + "</a>");
                //                    str.Append("</div>");

                //                    dvcnt = dvcnt + 1;
                //                    if (dvcnt == 5)
                //                    {
                //                        str.Append("</div>");
                //                        str.Append("<div style=\"float: left;width:100%;\">");
                //                        dvcnt = 0;
                //                    }
                //                }
                //                str.Append("</div>");

                //                string strWidgetTitle = string.Empty;
                //                if (Convert.ToInt64(SessionHelper.ParentModuleList.Authentication) == item.ID)// Bind Athentication Modules
                //                {
                //                    strWidgetTitle = ResDashboard.Authentication;
                //                    dvAuthentication = "<div id=\"Authentication\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                //                               "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                //                               "</div><div class=\"portlet-content\" ><div id=\"dvAuthentication\">" + str.ToString() + "</div></div></div>";
                //                }
                //                if (Convert.ToInt64(SessionHelper.ParentModuleList.Master) == item.ID)// Bind Master Module
                //                {
                //                    strWidgetTitle = ResDashboard.Masters;
                //                    dvMaster = "<div id=\"Master\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                //                               "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                //                               "</div><div class=\"portlet-content\" ><div id=\"dvMaster\">" + str.ToString() + "</div></div></div>";
                //                }
                //                if (Convert.ToInt64(SessionHelper.ParentModuleList.Assets) == item.ID)// Bind Assets Module
                //                {
                //                    strWidgetTitle = ResDashboard.Assets;
                //                    dvAssets = "<div id=\"Assets\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                //                               "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                //                               "</div><div class=\"portlet-content\" ><div id=\"dvAssets\">" + str.ToString() + "</div></div></div>";
                //                }
                //                if (Convert.ToInt64(SessionHelper.ParentModuleList.Reports) == item.ID)// Bind Reports Module
                //                {
                //                    strWidgetTitle = ResDashboard.Reports;
                //                    dvReports = "<div id=\"Reports\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                //                               "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                //                               "</div><div class=\"portlet-content\" ><div id=\"dvReports\">" + str.ToString() + "</div></div></div>";
                //                }
                //                if (Convert.ToInt64(SessionHelper.ParentModuleList.Inventory) == item.ID)// Bind Reports Module
                //                {
                //                    strWidgetTitle = ResDashboard.Inventory;
                //                    dvInventry = "<div id=\"Inventory\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                //                               "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                //                               "</div><div class=\"portlet-content\" ><div id=\"dvInventry\">" + str.ToString() + "</div></div></div>";
                //                }
                //                if (Convert.ToInt64(SessionHelper.ParentModuleList.Consume) == item.ID)// Bind Reports Module
                //                {
                //                    strWidgetTitle = ResDashboard.Consume;
                //                    dvConsume = "<div id=\"Consume\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                //                               "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                //                               "</div><div class=\"portlet-content\" ><div id=\"dvConsume\">" + str.ToString() + "</div></div></div>";
                //                }
                //                if (Convert.ToInt64(SessionHelper.ParentModuleList.Replenish) == item.ID)// Bind Reports Module
                //                {
                //                    strWidgetTitle = ResDashboard.Replenish;
                //                    dvReplenish = "<div id=\"Replenish\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                //                               "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                //                               "</div><div class=\"portlet-content\" ><div id=\"dvReplenish\">" + str.ToString() + "</div></div></div>";
                //                }
                //                if (Convert.ToInt64(SessionHelper.ParentModuleList.Receive) == item.ID)// Bind Reports Module
                //                {
                //                    strWidgetTitle = ResDashboard.Receive;
                //                    dvReceive = "<div id=\"Receive\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                //                               "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                //                               "</div><div class=\"portlet-content\" ><div id=\"dvReceive\">" + str.ToString() + "</div></div></div>";
                //                }
                //                if (Convert.ToInt64(SessionHelper.ParentModuleList.Kits) == item.ID)// Bind Reports Module
                //                {
                //                    strWidgetTitle = ResDashboard.Kits;
                //                    dvKits = "<div id=\"Kits\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                //                               "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                //                               "</div><div class=\"portlet-content\" ><div id=\"dvKits\">" + str.ToString() + "</div></div></div>";
                //                }
                //                if (Convert.ToInt64(SessionHelper.ParentModuleList.Configuration) == item.ID)// Bind Reports Module
                //                {
                //                    strWidgetTitle = ResDashboard.Configuration;
                //                    dvConfiguration = "<div id=\"Configuration\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                //                               "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                //                               "</div><div class=\"portlet-content\" ><div id=\"dvConfiguration\">" + str.ToString() + "</div></div></div>";
                //                }

                //                str.Clear();
                //            }
                //        }
                //    }
                //}
                //#endregion
                //#region UserType 3 -- Normal Users
                else
                {
                    foreach (ParentModuleMasterDTO item in lstmasterModule)
                    {
                        if (item.ID != Convert.ToInt64(SessionHelper.ParentModuleList.Master))
                        {
                            List<eTurns.DTO.UserRoleModuleDetailsDTO> objChild = (from m in lstPermission.PermissionList
                                                                                  where m.ParentID == Convert.ToInt32(item.ID)
                                                                                  select m).OrderBy(c => c.ModuleName).ToList();
                            if (objChild != null)
                            {
                                objChild = objChild.Where(c => c.ModuleID != (Int64)SessionHelper.ModuleList.ResetAutoNumbers && c.ModuleID != (Int64)SessionHelper.ModuleList.Synch && c.ModuleID != (Int64)SessionHelper.ModuleList.Barcode).ToList();
                                if (objChild.Count > 0)
                                {
                                    str.Append("<div style=\"float: left;width:100%;\">");
                                    int dvcnt = 0;
                                    foreach (var Childitem in objChild)
                                    {
                                        if (Childitem.ModuleID != (Int64)SessionHelper.ModuleList.EnterpriseMaster)
                                        {
                                            string ModuleName = SessionHelper.GetModuleName(Convert.ToInt32(Childitem.ModuleID));
                                            string TotalNotification = obj.GetModuleNotification(Convert.ToInt32(Childitem.ModuleID), SessionHelper.RoomID, SessionHelper.UserID);
                                            if (Convert.ToInt32(Childitem.ModuleID) == (int)eTurnsWeb.Helper.SessionHelper.ModuleList.Kits)
                                            {
                                                TotalNotification = new KitController().GetWIPKitCountForRedCircle();
                                            }
                                            TotalNotification = TotalNotification == null ? "0" : TotalNotification;

                                            str.Append("<div class=\"imgclass\">");
                                            str.Append("<a href=\"" + Childitem.ModuleURL + "\" runat=\"server\" title=\"" + ModuleName + "\">");
                                            str.Append("<img src=\"../Content/images/" + Childitem.ImageName + "\"  class=\"imgHoverable\" alt=\"" + "" + "\"  />" + (TotalNotification != "0" ? "<span>" + TotalNotification + "</span>" : "") + "<br />" + ModuleName + "</a>");
                                            str.Append("</div>");

                                            dvcnt = dvcnt + 1;
                                            if (dvcnt == 5)
                                            {
                                                str.Append("</div>");
                                                str.Append("<div style=\"float: left;width:100%;\">");
                                                dvcnt = 0;
                                            }
                                        }
                                    }
                                    str.Append("</div>");

                                    string strWidgetTitle = string.Empty;
                                    if (Convert.ToInt64(SessionHelper.ParentModuleList.Authentication) == item.ID)// Bind Athentication Modules
                                    {
                                        strWidgetTitle = ResDashboard.Authentication;
                                        dvAuthentication = "<div id=\"Authentication\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                                                   "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                                                   "</div><div class=\"portlet-content\" ><div id=\"dvAuthentication\">" + str.ToString() + "</div></div></div>";
                                    }
                                    if (Convert.ToInt64(SessionHelper.ParentModuleList.Master) == item.ID)// Bind Master Module
                                    {
                                        strWidgetTitle = ResDashboard.Masters;
                                        dvMaster = "<div id=\"Master\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                                                   "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                                                   "</div><div class=\"portlet-content\" ><div id=\"dvMaster\">" + str.ToString() + "</div></div></div>";
                                    }
                                    if (Convert.ToInt64(SessionHelper.ParentModuleList.Assets) == item.ID)// Bind Assets Module
                                    {
                                        strWidgetTitle = ResDashboard.Assets;
                                        dvAssets = "<div id=\"Assets\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                                                   "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                                                   "</div><div class=\"portlet-content\" ><div id=\"dvAssets\">" + RenderRazorPartialViewToString("ToolsAssetsDashboard") + "</div></div></div>";
                                    }
                                    if (Convert.ToInt64(SessionHelper.ParentModuleList.Reports) == item.ID)// Bind Reports Module
                                    {
                                        strWidgetTitle = ResDashboard.Reports;
                                        dvReports = "<div id=\"Reports\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                                                   "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                                                   "</div><div class=\"portlet-content\" ><div id=\"dvReports\">" + str.ToString() + "</div></div></div>";
                                    }
                                    if (Convert.ToInt64(SessionHelper.ParentModuleList.Inventory) == item.ID)// Bind Reports Module
                                    {
                                        strWidgetTitle = ResDashboard.Inventry;
                                        dvInventry = "<div id=\"Inventory\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                                                   "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                                                   "</div><div class=\"portlet-content\" ><div id=\"dvInventry\">" + RenderRazorPartialViewToString("InventoryDashboard") + "</div></div></div>";
                                    }
                                    if (Convert.ToInt64(SessionHelper.ParentModuleList.Consume) == item.ID)// Bind Reports Module
                                    {
                                        strWidgetTitle = ResDashboard.Consume;
                                        dvConsume = "<div id=\"Consume\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                                                   "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                                                   "</div><div class=\"portlet-content\" ><div id=\"dvConsume\">" + RenderRazorPartialViewToString("ConsumeDashboard") + "</div></div></div>";
                                    }
                                    if (Convert.ToInt64(SessionHelper.ParentModuleList.Replenish) == item.ID)// Bind Reports Module
                                    {
                                        strWidgetTitle = ResDashboard.Replenish;
                                        dvReplenish = "<div id=\"Replenish\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                                                   "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                                                   "</div><div class=\"portlet-content\" ><div id=\"dvReplenish\">" + RenderRazorPartialViewToString("ReplenishDashboard") + "</div></div></div>";
                                    }
                                    if (Convert.ToInt64(SessionHelper.ParentModuleList.Receive) == item.ID)// Bind Reports Module
                                    {
                                        strWidgetTitle = ResDashboard.Receive;
                                        dvReceive = "<div id=\"Receive\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                                                   "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                                                   "</div><div class=\"portlet-content\" ><div id=\"dvReceive\">" + str.ToString() + "</div></div></div>";
                                    }
                                    if (Convert.ToInt64(SessionHelper.ParentModuleList.Kits) == item.ID)// Bind Reports Module
                                    {
                                        strWidgetTitle = ResDashboard.Kits;
                                        dvKits = "<div id=\"Kits\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                                                   "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                                                   "</div><div class=\"portlet-content\" ><div id=\"dvKits\">" + str.ToString() + "</div></div></div>";
                                    }
                                    if (Convert.ToInt64(SessionHelper.ParentModuleList.Configuration) == item.ID)// Bind Reports Module
                                    {
                                        strWidgetTitle = ResDashboard.Configuration;
                                        dvConfiguration = "<div id=\"Configuration\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                                                   "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                                                   "</div><div class=\"portlet-content\" ><div id=\"dvConfiguration\">" + str.ToString() + "</div></div></div>";
                                    }

                                    str.Clear();
                                }
                            }
                        }

                    }
                }
                //#endregion

                //if (SessionHelper.UserType != 1 && SessionHelper.UserType != 2)
                //{
                //    //DashboardWidgetController obdashboard = new DashboardWidgetController();
                //    eTurns.DAL.DashboardWidgetDAL obdashboard = new eTurns.DAL.DashboardWidgetDAL(SessionHelper.EnterPriseDBName);
                //    List<ItemMasterDTO> lstItemMaster = new List<ItemMasterDTO>();
                //    lstItemMaster = obdashboard.GetCategoryWiseCost();
                //    if (lstItemMaster.Count > 0)
                //    {
                //        string CostVal = string.Empty;// "1,2,3,4,5,6,7,8,9,10";
                //        string CategoryName = string.Empty;//",First,Second,Third,Four,Five,Six,Seven,Eight,Nine,Ten";
                //        string CategoryColor = string.Empty;//"#3366cc,#dc3912,#ff9900,#109618,#66aa00,#dd4477,#0099c6,#990099,#dd6500,#ff5512";
                //        foreach (ItemMasterDTO item in lstItemMaster)
                //        {
                //            CostVal = CostVal == "" ? (item.Cost == null ? "0" : item.Cost.ToString()) : (CostVal + "," + (item.Cost == null ? "0" : item.Cost.ToString()));
                //            CategoryName = CategoryName + "," + item.CategoryName;
                //            CategoryColor = CategoryColor == "" ? (string.IsNullOrEmpty(item.CategoryColor) ? get_random_color() : item.CategoryColor) : (CategoryColor + "," + (string.IsNullOrEmpty(item.CategoryColor) ? get_random_color() : item.CategoryColor));

                //        }
                //        //CostVal = "1,2,3,4,5,6,7,8,9,10";
                //        //CategoryName = ",First,Second,Third,Four,Five,Six,Seven,Eight,Nine,Ten";
                //        //CategoryColor = "#3366cc,#dc3912,#ff9900,#109618,#66aa00,#dd4477,#0099c6,#990099,#dd6500,#ff5512";

                //        Array CNameArrya = CategoryName.Split(',');
                //        Array CColorarr = CategoryColor.Split(',');
                //        string strlegendinner = string.Empty;
                //        int countCategory = 0;
                //        for (int i = 0; i < CNameArrya.Length - 1; i++)
                //        {
                //            if (countCategory == 0)
                //                strlegendinner = strlegendinner + "<div style=\"width:120px;float:left;\">";

                //            strlegendinner = strlegendinner + "<div style=\"clear:both;padding-top:2px\" ><div style=\"padding-left:10px;float:left;border:1px solid;height:10px;background-color:" + CColorarr.GetValue(i) + ";\"></div><div style=\"padding-left:20px;margin-top:-3px;\">" + CNameArrya.GetValue(i + 1) + "</div></div>";
                //            countCategory += 1;
                //            if (countCategory == 10)
                //            {
                //                countCategory = 0;
                //                strlegendinner = strlegendinner + "</div>";
                //            }
                //        }
                //        if (countCategory != 0)
                //            strlegendinner = strlegendinner + "</div>";

                //        String StrLegent = string.Empty;
                //        StrLegent = "<div style=\"padding-left:8px;float:right;\" id=\"dvCategoryCostLegend\">" + strlegendinner + "</div>";

                //        dvCategoryCost = " <div id=\"CategoryCost\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                //                               "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + "Category Wise Cost" + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                //                               "</div><div class=\"portlet-content\" ><div id=\"dvCategoryCost\" style=\"float:left; padding-bottom: 10px;\"><input id=\"hdnCost\" type=\"hidden\" value=\"" + CostVal + "\"/><input id=\"hdnCategory\" type=\"hidden\" value=\"" + CategoryName + "\"/><input id=\"hdnCategoryColor\" type=\"hidden\" value=\"" + CategoryColor + "\"/> <span id=\"spCategoryCost\" >" + CostVal + "</span>" +
                //                               StrLegent + "</div></div></div>";
                //    }
                //}


            }


            #region[Set Wiget in Order]

            StringBuilder strdivs1 = new StringBuilder();
            StringBuilder strdivs2 = new StringBuilder();
            //DashboardWidgetController objwidorder = new DashboardWidgetController();
            eTurnsMaster.DAL.DashboardWidgetDAL objwidorder = new eTurnsMaster.DAL.DashboardWidgetDAL(SessionHelper.EnterPriseDBName);
            DashboardWidgeDTO objdashboardorder = null;
            if (SessionHelper.EnterPriceID > 0)
            {
                objdashboardorder = objwidorder.GetUserWidget(SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID, 1);
            }


            if (objdashboardorder != null && !string.IsNullOrEmpty(objdashboardorder.WidgetOrder))
            {
                string[] widgetorder = objdashboardorder.WidgetOrder.Split(':');
                for (int k = 0; k < widgetorder.Length; k++)
                {
                    string[] strorder = widgetorder[k].Split(',');
                    foreach (var widgetname in strorder)
                    {
                        if (SessionHelper.ParentModuleList.Authentication.ToString() == widgetname)
                        {
                            if (k == 0)
                                strdivs1.Append(dvAuthentication);
                            else
                                strdivs2.Append(dvAuthentication);
                        }
                        else if (SessionHelper.ParentModuleList.Master.ToString() == widgetname)
                        {
                            if (k == 0)
                                strdivs1.Append(dvMaster);
                            else
                                strdivs2.Append(dvMaster);
                        }

                        else if (SessionHelper.ParentModuleList.Assets.ToString() == widgetname)
                        {
                            if (k == 0)
                                strdivs1.Append(dvAssets);
                            else
                                strdivs2.Append(dvAssets);
                        }
                        else if (SessionHelper.ParentModuleList.Reports.ToString() == widgetname)
                        {
                            if (k == 0)
                                strdivs1.Append(dvReports);
                            else
                                strdivs2.Append(dvReports);
                        }
                        else if (SessionHelper.ParentModuleList.Inventory.ToString() == widgetname)
                        {
                            if (k == 0)
                                strdivs1.Append(dvInventry);
                            else
                                strdivs2.Append(dvInventry);
                        }
                        else if (SessionHelper.ParentModuleList.Consume.ToString() == widgetname)
                        {
                            if (k == 0)
                                strdivs1.Append(dvConsume);
                            else
                                strdivs2.Append(dvConsume);
                        }
                        else if (SessionHelper.ParentModuleList.Replenish.ToString() == widgetname)
                        {
                            if (k == 0)
                                strdivs1.Append(dvReplenish);
                            else
                                strdivs2.Append(dvReplenish);
                        }
                        else if (SessionHelper.ParentModuleList.Receive.ToString() == widgetname)
                        {
                            if (k == 0)
                                strdivs1.Append(dvReceive);
                            else
                                strdivs2.Append(dvReceive);
                        }
                        else if (SessionHelper.ParentModuleList.Kits.ToString() == widgetname)
                        {
                            if (k == 0)
                                strdivs1.Append(dvKits);
                            else
                                strdivs2.Append(dvKits);
                        }
                        else if (SessionHelper.ParentModuleList.Configuration.ToString() == widgetname)
                        {
                            if (k == 0)
                                strdivs1.Append(dvConfiguration);
                            else
                                strdivs2.Append(dvConfiguration);
                        }
                        //else if (SessionHelper.ParentModuleList.CategoryCost.ToString() == widgetname)
                        //{
                        //    if (k == 0)
                        //        strdivs1.Append(dvCategoryCost);
                        //    else
                        //        strdivs2.Append(dvCategoryCost);
                        //}
                    }
                }
                LeftDiv = strdivs1.ToString();
                RightDiv = strdivs2.ToString();
            }
            else
            {
                LeftDiv = dvMaster + dvKits + dvConsume + dvReports;
                RightDiv = dvAuthentication + dvInventry + dvAssets + dvReceive + dvReplenish + dvConfiguration;
            }
            #endregion

            //LeftDiv = dvAuthentication + dvMaster;
            //RightDiv = dvAssets + dvReports;

            return Json(new { LeftWidget = LeftDiv, RightWidget = RightDiv, Status = "ok" }, JsonRequestBehavior.AllowGet);

        }


        public JsonResult GenerateCategoryChart()
        {
            string LeftDiv = string.Empty;
            string dvCategoryCost = string.Empty;
            eTurns.DAL.DashboardWidgetDAL obdashboard = new eTurns.DAL.DashboardWidgetDAL(SessionHelper.EnterPriseDBName);
            List<ItemMasterDTO> lstItemMaster = new List<ItemMasterDTO>();
            byte piechartMetricon = 1;
            lstItemMaster = obdashboard.GetCategoryWiseCost(SessionHelper.CompanyID, SessionHelper.RoomID, piechartMetricon, 10);
            if (lstItemMaster.Count > 0)
            {
                string CostVal = string.Empty;// "1,2,3,4,5,6,7,8,9,10";
                string CategoryName = string.Empty;//",First,Second,Third,Four,Five,Six,Seven,Eight,Nine,Ten";
                string CategoryColor = string.Empty;//"#3366cc,#dc3912,#ff9900,#109618,#66aa00,#dd4477,#0099c6,#990099,#dd6500,#ff5512";
                foreach (ItemMasterDTO item in lstItemMaster)
                {
                    CostVal = CostVal == "" ? (item.Cost == null ? "0" : item.Cost.ToString()) : (CostVal + "," + (item.Cost == null ? "0" : item.Cost.ToString()));
                    CategoryName = CategoryName + "$" + item.CategoryName;
                    CategoryColor = CategoryColor == "" ? (string.IsNullOrEmpty(item.CategoryColor) ? get_random_color() : item.CategoryColor) : (CategoryColor + "," + (string.IsNullOrEmpty(item.CategoryColor) ? get_random_color() : item.CategoryColor));

                }
                //CostVal = "1,2,3,4,5,6,7,8,9,10";
                //CategoryName = ",First,Second,Third,Four,Five,Six,Seven,Eight,Nine,Ten";
                //CategoryColor = "#3366cc,#dc3912,#ff9900,#109618,#66aa00,#dd4477,#0099c6,#990099,#dd6500,#ff5512";

                Array CNameArrya = CategoryName.Split('$');
                Array CColorarr = CategoryColor.Split(',');
                string strlegendinner = string.Empty;
                int countCategory = 0;
                for (int i = 0; i < CNameArrya.Length - 1; i++)
                {
                    if (countCategory == 0)
                        strlegendinner = strlegendinner + "<div style=\"width:120px;float:left;\">";

                    strlegendinner = strlegendinner + "<div style=\"clear:both;padding-top:2px\" ><div style=\"padding-left:10px;float:left;border:1px solid;height:10px;background-color:" + CColorarr.GetValue(i) + ";\"></div><div style=\"padding-left:20px;margin-top:-3px;\">" + CNameArrya.GetValue(i + 1) + "</div></div>";
                    countCategory += 1;
                    if (countCategory == 10)
                    {
                        countCategory = 0;
                        strlegendinner = strlegendinner + "</div>";
                    }
                }
                if (countCategory != 0)
                    strlegendinner = strlegendinner + "</div>";

                String StrLegent = string.Empty;
                StrLegent = "<div style=\"padding-left:8px;float:right;\" id=\"dvCategoryCostLegend\">" + strlegendinner + "</div>";

                dvCategoryCost = " <div id=\"dvCategoryCost\" style=\"float:left; padding-bottom: 10px;\"><input id=\"hdnCost\" type=\"hidden\" value=\"" + CostVal + "\"/><input id=\"hdnCategory\" type=\"hidden\" value=\"" + CategoryName + "\"/><input id=\"hdnCategoryColor\" type=\"hidden\" value=\"" + CategoryColor + "\"/> <span id=\"spCategoryCost\" >" + CostVal + "</span>" +
                                       StrLegent + "</div>";
            }

            return Json(new { LeftWidget = dvCategoryCost, Status = "ok" }, JsonRequestBehavior.AllowGet);
        }
        //METHOD#1
        //private static readonly Random random = new Random();
        //private static readonly object syncLock = new object();
        //public static int RandomNumber(int max)
        //{
        //    lock (syncLock)
        //    { //synchronize
        //        return random.Next(max);
        //    }
        //}
        //color = color + letters[RandomNumber(15)];               

        //METHOD#2
        private static readonly ThreadLocal<Random> appRandom = new ThreadLocal<Random>(() => new Random());

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

        public void SaveWidgetOrder(byte? dtype)
        {
            eTurnsMaster.DAL.DashboardWidgetDAL obj = new eTurnsMaster.DAL.DashboardWidgetDAL(SessionHelper.EnterPriseDBName);
            if (dtype == 1)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(Request.QueryString["saveorder"])))
                {
                    if (Convert.ToInt32((Request.QueryString["saveorder"])) == 1)
                    {
                        string Worderleft = Convert.ToString(Request.QueryString["sort1[]"]);
                        string Worderright = Convert.ToString(Request.QueryString["sort2[]"]);
                        string WidgetOrder = Worderleft + ":" + Worderright;

                        //DashboardWidgetController obj = new DashboardWidgetController();
                        obj.SaveUserWidget(SessionHelper.UserID, WidgetOrder, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID, dtype ?? 0);

                        //SendMail("email.html", "Subject", ConfigurationManager.AppSettings["FromAddress"].ToString(), ConfigurationManager.AppSettings["FromAddress"].ToString(), string.Empty, string.Empty, string.Empty);

                        //SendMailToUser();
                    }
                }
            }
            if (dtype == 2)
            {
                string widgetorder = Convert.ToString(Request.QueryString["widgetorder"]);
                obj.SaveUserWidget(SessionHelper.UserID, widgetorder, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID, dtype ?? 0);
            }
        }

        private void SendMailToUser()
        {
            CultureInfo curCult = null;
            EmailTemplateDAL objEmailTemplateDAL = null;
            EmailTemplateDetailDTO objEmailTemplateDetailDTO = null;
            StringBuilder MessageBody = null;
            eTurnsUtility objUtils = null;
            eTurnsMaster.DAL.eMailDAL objEmailDAL = null;
            try
            {
                string StrSubject = "Subject";
                string strToAddress = ConfigurationManager.AppSettings["FromAddress"].ToString();
                string strCCAddress = "";
                curCult = eTurns.DTO.Resources.ResourceHelper.CurrentCult;
                objEmailTemplateDAL = new EmailTemplateDAL(SessionHelper.EnterPriseDBName);
                objEmailTemplateDetailDTO = new EmailTemplateDetailDTO();
                objEmailTemplateDetailDTO = objEmailTemplateDAL.GetEmailTemplate("email", ResourceHelper.CurrentCult.ToString(), SessionHelper.RoomID, SessionHelper.CompanyID);
                if (objEmailTemplateDetailDTO != null)
                {
                    MessageBody.Append(objEmailTemplateDetailDTO.MailBodyText);
                    StrSubject = objEmailTemplateDetailDTO.MailSubject;
                }

                MessageBody.Replace("@@UserName@@", "Test XYZ");
                objUtils = new eTurnsUtility();
                objUtils.SendMail(strToAddress, strCCAddress, StrSubject, MessageBody.ToString());
                objEmailDAL = new eTurnsMaster.DAL.eMailDAL();
                objEmailDAL.eMailToSend(strToAddress, strCCAddress, StrSubject, MessageBody.ToString(), SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, null, " Web => Administrative.Master => SendMailToUser");

            }
            finally
            {
                curCult = null;
                objEmailTemplateDAL = null;
                objEmailTemplateDetailDTO = null;
                MessageBody = null;
                objUtils = null;
                objEmailDAL = null;
            }
        }

        private StringBuilder EmailTemplateReplaceURL(string EmailTemplateName)
        {
            StringBuilder MessageBody = new StringBuilder();
            if (System.IO.File.Exists(Server.MapPath("../") + "Content\\EmailTemplates\\" + EmailTemplateName))
            {
                MessageBody.Append(System.IO.File.ReadAllText(Server.MapPath("../") + "Content\\EmailTemplates/" + EmailTemplateName));
            }

            string urlPart = Request.Url.ToString();
            string replacePart = urlPart.Split('/')[0] + "//" + urlPart.Split('/')[2];

            //MessageBody = MessageBody.Replace("../CommonImages/logo.gif", replacePart + "CommonImages/logo.gif");
            if (Request.ApplicationPath != "/")
                MessageBody = MessageBody.Replace("src=\"", "src=\"" + replacePart + Request.ApplicationPath);
            else
                MessageBody = MessageBody.Replace("src=\"", "src=\"" + replacePart);
            return MessageBody;
        }

        public ActionResult Dashboard2()
        {
            Session["ItemQuantityGrid"] = null;
            Session["ItemQuantityGridCount"] = null;
            return View();
        }

        public ActionResult InventoryAnalysis()
        {
            eTurnsMaster.DAL.DashboardWidgetDAL objDashboardWidgetDAL = new eTurnsMaster.DAL.DashboardWidgetDAL(SessionHelper.EnterPriseDBName);
            DashboardWidgeDTO objDashboardWidgeDTO = objDashboardWidgetDAL.GetUserWidget(SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID, 2);
            long[] arrOrderWidget = new long[] { 1, 2, 3 };
            if (objDashboardWidgeDTO != null)
            {
                if (!string.IsNullOrWhiteSpace(objDashboardWidgeDTO.WidgetOrder))
                {
                    arrOrderWidget = objDashboardWidgeDTO.WidgetOrder.Split(',').ToIntArray();
                }
            }
            Session["SessionMinMaxTable"] = null;
            ViewBag.arrOrderWidget = arrOrderWidget;
            DashboardDAL objDashboardDAL = new DashboardDAL(SessionHelper.EnterPriseDBName);
            ViewBag.RoomTurnsInventoryValue = objDashboardDAL.GetRoomTurnsInventoryValue(SessionHelper.RoomID, SessionHelper.CompanyID);

            //ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            //DashboardDAL objDashboardDAL = new DashboardDAL(SessionHelper.EnterPriseDBName);
            //List<ItemMaster> lstItems = objItemMasterDAL.GetItems();
            //lstItems = lstItems.Where(t => t.Room == SessionHelper.RoomID && t.CompanyID == SessionHelper.CompanyID).ToList();
            //foreach (var itm in lstItems)
            //{
            //    objDashboardDAL.UpdateTurnsByItemGUID(itm.Room ?? 0, itm.CompanyID ?? 0, itm.GUID);
            //}
            return View();
        }

        public ActionResult DashboardParameters()
        {
            DashboardDAL objDashboardDAL = new DashboardDAL(SessionHelper.EnterPriseDBName);
            DashboardParameterDTO objDashboardParameterDTO = new DashboardParameterDTO();
            objDashboardParameterDTO = objDashboardDAL.GetDashboardParameters(SessionHelper.RoomID, SessionHelper.CompanyID);
            return PartialView("_DashboardParameters", objDashboardParameterDTO);
        }

        [HttpPost]
        public JsonResult SaveDashboardParameters(DashboardParameterDTO objDashboardParameterDTO)
        {
            DashboardDAL objDashboardDAL = new DashboardDAL(SessionHelper.EnterPriseDBName);
            objDashboardParameterDTO.CreatedBy = SessionHelper.UserID;
            objDashboardParameterDTO.CreatedOn = DateTimeUtility.DateTimeNow;
            objDashboardParameterDTO.UpdatedBy = SessionHelper.UserID;
            objDashboardParameterDTO.UpdatedOn = DateTimeUtility.DateTimeNow;
            objDashboardParameterDTO.RoomId = SessionHelper.RoomID;
            objDashboardParameterDTO.CompanyId = SessionHelper.CompanyID;
            objDashboardParameterDTO.EnterpriseId = SessionHelper.EnterPriceID;
            switch (objDashboardParameterDTO.ParameterType)
            {
                case 1:
                    objDashboardParameterDTO = objDashboardDAL.SaveDashboardTurnParameters(objDashboardParameterDTO);
                    break;
                case 2:
                    objDashboardParameterDTO = objDashboardDAL.SaveDashboardAUParameters(objDashboardParameterDTO);
                    break;
                case 3:
                    objDashboardParameterDTO = objDashboardDAL.SaveDashboardMinMaxParameters(objDashboardParameterDTO);
                    break;
                case 4:
                    objDashboardParameterDTO = objDashboardDAL.SaveDashboardOtherParameters(objDashboardParameterDTO);
                    break;
                default:
                    objDashboardParameterDTO = objDashboardDAL.SaveDashboardParameters(objDashboardParameterDTO);
                    break;
            }
            return Json(objDashboardParameterDTO);
        }

        [ChildActionOnly]
        public PartialViewResult ChartAndSlider()
        {

            return PartialView("_ChartAndSlider");
        }
        [ChildActionOnly]
        public PartialViewResult MinMaxAnalysis()
        {
            return PartialView("_MinMaxAnalysis");
        }
        [ChildActionOnly]
        public PartialViewResult TurnsAnalysis()
        {
            return PartialView("_TurnsAnalysis");
        }


        public FileResult MinSliderChart()
        {
            string areaname = "Min Optimization Paramters";
            SeriesChartType chartType = SeriesChartType.Column;
            Chart chart = new Chart();
            chart.Height = 300;
            chart.Width = 300;
            chart.BackColor = Color.FromArgb(211, 223, 240);
            chart.BorderlineDashStyle = ChartDashStyle.Solid;
            chart.BackSecondaryColor = Color.White;
            chart.BackGradientStyle = GradientStyle.TopBottom;
            chart.BorderlineWidth = 1;
            chart.Palette = ChartColorPalette.BrightPastel;
            chart.BorderlineColor = Color.FromArgb(26, 59, 105);
            chart.RenderType = RenderType.BinaryStreaming;
            chart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;
            chart.AntiAliasing = AntiAliasingStyles.All;
            chart.TextAntiAliasingQuality = TextAntiAliasingQuality.Normal;
            chart.Titles.Add(ChartHelper.CreateTitle(areaname));
            //chart.Legends.Add(CreateLegend());
            int cntGreen = 60;
            int cntYellow = 30;
            int cntRed = 45;
            if (Session["SessionMinMaxTable"] != null)
            {
                List<MinMaxDataTableInfo> DataFromDB = (List<MinMaxDataTableInfo>)Session["SessionMinMaxTable"];
                cntGreen = DataFromDB.Count(t => t.MinAnalysis == "Green");
                cntYellow = DataFromDB.Count(t => t.MinAnalysis == "Yellow");
                cntRed = DataFromDB.Count(t => t.MinAnalysis == "Red");
            }
            else
            {
                cntGreen = 0;
                cntYellow = 0;
                cntRed = 0;
            }
            chart.Series.Add(ChartHelper.CreateSeries(chartType, "Above optimization", 1, cntRed, Color.Red, areaname));
            chart.Series.Add(ChartHelper.CreateSeries(chartType, "Between Optimization", 2, cntYellow, Color.Yellow, areaname));
            chart.Series.Add(ChartHelper.CreateSeries(chartType, "Below Optimization", 3, cntGreen, Color.Green, areaname));
            chart.ChartAreas.Add(ChartHelper.CreateChartArea(areaname));

            MemoryStream ms = new MemoryStream();
            chart.SaveImage(ms);
            return File(ms.GetBuffer(), @"image/png");
        }

        public FileResult MaxSliderChart()
        {
            string areaname = "Max Optimization Paramters";
            SeriesChartType chartType = SeriesChartType.Column;
            Chart chart = new Chart();
            chart.Height = 300;
            chart.Width = 300;
            chart.BackColor = Color.FromArgb(211, 223, 240);
            chart.BorderlineDashStyle = ChartDashStyle.Solid;
            chart.BackSecondaryColor = Color.White;
            chart.BackGradientStyle = GradientStyle.TopBottom;
            chart.BorderlineWidth = 1;
            chart.Palette = ChartColorPalette.BrightPastel;
            chart.BorderlineColor = Color.FromArgb(26, 59, 105);
            chart.RenderType = RenderType.BinaryStreaming;
            chart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;
            chart.AntiAliasing = AntiAliasingStyles.All;
            chart.TextAntiAliasingQuality = TextAntiAliasingQuality.Normal;
            chart.Titles.Add(ChartHelper.CreateTitle(areaname));
            //chart.Legends.Add(CreateLegend());

            int cntGreen = 60;
            int cntYellow = 30;
            int cntRed = 45;
            if (Session["SessionMinMaxTable"] != null)
            {
                List<MinMaxDataTableInfo> DataFromDB = (List<MinMaxDataTableInfo>)Session["SessionMinMaxTable"];
                cntGreen = DataFromDB.Count(t => t.MaxAnalysis == "Green");
                cntYellow = DataFromDB.Count(t => t.MaxAnalysis == "Yellow");
                cntRed = DataFromDB.Count(t => t.MaxAnalysis == "Red");
            }
            else
            {
                cntGreen = 0;
                cntYellow = 0;
                cntRed = 0;
            }

            chart.Series.Add(ChartHelper.CreateSeries(chartType, "Above optimization", 1, cntRed, Color.Red, areaname));
            chart.Series.Add(ChartHelper.CreateSeries(chartType, "Between Optimization", 2, cntYellow, Color.Yellow, areaname));
            chart.Series.Add(ChartHelper.CreateSeries(chartType, "Below Optimization", 3, cntGreen, Color.Green, areaname));
            chart.ChartAreas.Add(ChartHelper.CreateChartArea(areaname));

            MemoryStream ms = new MemoryStream();
            chart.SaveImage(ms);
            return File(ms.GetBuffer(), @"image/png");
        }

        public FileResult ItemcategoryPieChart()
        {
            string areaname = "Category Metric";
            SeriesChartType chartType = SeriesChartType.Pie;
            Chart chart = new Chart();
            chart.Height = 300;
            chart.Width = 300;
            chart.BackColor = Color.FromArgb(211, 223, 240);
            chart.BorderlineDashStyle = ChartDashStyle.Solid;
            chart.BackSecondaryColor = Color.White;
            chart.BackGradientStyle = GradientStyle.TopBottom;
            chart.BorderlineWidth = 1;
            chart.Palette = ChartColorPalette.BrightPastel;
            chart.BorderlineColor = Color.FromArgb(26, 59, 105);
            chart.RenderType = RenderType.BinaryStreaming;
            chart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;
            chart.AntiAliasing = AntiAliasingStyles.All;
            chart.TextAntiAliasingQuality = TextAntiAliasingQuality.Normal;
            chart.Titles.Add(ChartHelper.CreateTitle(areaname));

            List<ItemMasterDTO> lstItemMaster = new List<ItemMasterDTO>();
            eTurns.DAL.DashboardWidgetDAL obdashboard = new eTurns.DAL.DashboardWidgetDAL(SessionHelper.EnterPriseDBName);
            byte piechartmetricon = 1;
            lstItemMaster = obdashboard.GetCategoryWiseCost(SessionHelper.CompanyID, SessionHelper.RoomID, piechartmetricon, 10);
            //Series seriesDetail = new Series("srscat");
            //chart.Series.Add(seriesDetail);



            //if (lstItemMaster.Count > 0)
            //{
            //    foreach (var item in lstItemMaster)
            //    {
            //        seriesDetail = new Series();
            //        seriesDetail.Name = item.CategoryName;
            //        seriesDetail.IsValueShownAsLabel = false;
            //        Color clr = GetNextRandomColor();
            //        if (!string.IsNullOrWhiteSpace(item.CategoryColor))
            //        {
            //            try
            //            {
            //                string colorcode = item.CategoryColor;
            //                int argb = Int32.Parse(colorcode.Replace("#", ""), NumberStyles.HexNumber);
            //                clr = Color.FromArgb(argb);
            //            }
            //            catch (Exception)
            //            {
            //                clr = GetNextRandomColor();
            //            }
            //        }

            //        seriesDetail.Color = clr;
            //        seriesDetail.ChartType = chartType;
            //        seriesDetail.BorderWidth = 2;
            //        seriesDetail["PieDrawingStyle"] = "SoftEdge";
            //        seriesDetail.ChartArea = "Optimization Paramters";
            //        DataPoint point;
            //        point = new DataPoint();
            //        point.AxisLabel = item.CategoryName;
            //        point.YValues = new double[] { (double)(item.Cost ?? 0) };
            //        seriesDetail.Points.Add(point);
            //    }
            //}
            chart.Series.Add(ChartHelper.CreatepieSeries(chartType, "Categories", 1, 45, Color.Red, areaname));
            if (lstItemMaster.Count > 0)
            {
                int i = 0;
                foreach (var item in lstItemMaster)
                {
                    ColorConverter obj = new ColorConverter();
                    Color catcolor = new Color();
                    try
                    {
                        catcolor = (Color)obj.ConvertFromString(item.CategoryColor);
                    }
                    catch (Exception)
                    {
                        catcolor = GetNextRandomColor();
                    }

                    chart.Series["Categories"].Points.AddY(item.Cost);
                    chart.Series["Categories"].Points[i].Color = catcolor;
                    chart.Series["Categories"].Points[i].ToolTip = item.CategoryName;
                    //chart.Series["Categories"].Points[i].Label = item.CategoryName;
                    //chart.Series["Categories"].Points[i].IsValueShownAsLabel = true;
                    i++;
                }
            }
            chart.ChartAreas.Add(ChartHelper.CreateChartArea(areaname));

            MemoryStream ms = new MemoryStream();
            chart.SaveImage(ms);
            return File(ms.GetBuffer(), @"image/png");
        }

        public Color GetNextRandomColor()
        {
            Random randonGen = new Random();
            Color randomColor = Color.FromArgb(randonGen.Next(255), randonGen.Next(255), randonGen.Next(255));
            return randomColor;
        }

        public ActionResult ItemCategoryChart()
        {
            DashboardParameterDTO objDashboardParameterDTO = new DashboardParameterDTO();
            objDashboardParameterDTO = new DashboardDAL(SessionHelper.EnterPriseDBName).GetDashboardParameters(SessionHelper.RoomID, SessionHelper.CompanyID);
            if (objDashboardParameterDTO != null)
            {
                ViewBag.PieChartmetricOn = objDashboardParameterDTO.PieChartmetricOn ?? 1;
            }
            else
            {
                ViewBag.PieChartmetricOn = 1;
            }

            return PartialView("_ItemCategoryChart");
        }

        public ActionResult MinChart()
        {
            ViewBag.MINGreenCounts = 0;
            ViewBag.MINRedCounts = 0;
            ViewBag.MINYelloCounts = 0;
            if (Session["SessionMinMaxTable"] != null)
            {
                List<MinMaxDataTableInfo> DataFromDB = (List<MinMaxDataTableInfo>)Session["SessionMinMaxTable"];
                ViewBag.MINGreenCounts = DataFromDB.Count(t => t.MinAnalysis == "Green");
                ViewBag.MINRedCounts = DataFromDB.Count(t => t.MinAnalysis == "Red");
                ViewBag.MINYelloCounts = DataFromDB.Count(t => t.MinAnalysis == "Yellow");
            }
            return PartialView("_MinChart");
        }

        public ActionResult MaxChart()
        {
            ViewBag.MaxGreenCounts = 0;
            ViewBag.MaxRedCounts = 0;
            ViewBag.MaxYelloCounts = 0;
            if (Session["SessionMinMaxTable"] != null)
            {
                List<MinMaxDataTableInfo> DataFromDB = (List<MinMaxDataTableInfo>)Session["SessionMinMaxTable"];
                ViewBag.MaxGreenCounts = DataFromDB.Count(t => t.MaxAnalysis == "Green");
                ViewBag.MaxRedCounts = DataFromDB.Count(t => t.MaxAnalysis == "Red");
                ViewBag.MaxYelloCounts = DataFromDB.Count(t => t.MaxAnalysis == "Yellow");
            }
            return PartialView("_MaxChart");
        }

        public JsonResult UpdateInventoryQuantity(string MinMaxOrCrit, string GUID, string Automin, string CalMin, string Automax, string CalMax, string MinPer, string MaxPer)
        {
            string message = "";
            List<InventoryDashboardDTO> lst = (List<InventoryDashboardDTO>)Session["SessionMinMaxTable"];

            InventoryDashboardDTO obj = new InventoryDashboardDTO();
            obj = lst.Where(c => c.GUID.ToString() == GUID).SingleOrDefault();
            decimal dMinPer = 0;
            decimal.TryParse(MinPer, out dMinPer);

            decimal dMaxPer = 0;
            decimal.TryParse(MaxPer, out dMaxPer);

            if (MinMaxOrCrit == "AutoCurrentMin")
            {
                decimal dCalMin = 0;
                decimal.TryParse(CalMin, out dCalMin);

                double dAutomin = 0;
                double.TryParse(Automin, out dAutomin);

                obj.AutoCurrentMin = dAutomin;

                decimal MinAutoPer = Convert.ToDecimal(((dCalMin - Convert.ToDecimal(dAutomin)) / Convert.ToDecimal(dAutomin)) * 100);
                obj.AutoMinPercentage = MinAutoPer < 0 ? Math.Round((MinAutoPer * (-1)), 2) : Math.Round(MinAutoPer, 2);

                obj.MinAnalysis = (obj.AutoMinPercentage < dMinPer) ? "Green" : (obj.AutoMinPercentage > dMaxPer) ? "Red" : "Yellow";
            }
            else if (MinMaxOrCrit == "AutoCurrentMax")
            {
                decimal dCalMax = 0;
                decimal.TryParse(CalMax, out dCalMax);

                decimal dAutomax = 0;
                decimal.TryParse(Automax, out dAutomax);

                obj.AutoCurrentMax = dAutomax;

                decimal MaxAutoPer = Convert.ToDecimal(((dCalMax - Convert.ToDecimal(dAutomax)) / Convert.ToDecimal(dAutomax)) * 100);

                obj.AutoMaxPercentage = MaxAutoPer < 0 ? Math.Round((MaxAutoPer * (-1)), 2) : Math.Round(MaxAutoPer, 2);

                obj.MaxAnalysis = (obj.AutoMaxPercentage < dMinPer) ? "Green" : (obj.AutoMaxPercentage > dMaxPer) ? "Red" : "Yellow";
            }
            List<InventoryDashboardDTO> lstnew = new List<InventoryDashboardDTO>();

            foreach (InventoryDashboardDTO item in lst)
            {
                if (item.GUID.ToString() == GUID)
                {
                    lstnew.Add(obj);
                }
                else
                {
                    lstnew.Add(item);
                }
            }
            Session["SessionMinMaxTable"] = lstnew;
            return Json(new { Message = message }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult IframeTest()
        {
            return View();
        }

        public ActionResult MinMaxTuningListAjax(JQueryDataTableParamModel param)
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

            int AutoMinPer = 0;
            Int32.TryParse(Convert.ToString(Request["AutoMinPer"]), out AutoMinPer);
            int AutoMaxPer = 0;
            Int32.TryParse(Convert.ToString(Request["AutoMaxPer"]), out AutoMaxPer);
            bool IsItemLevelMinMax = true;
            bool.TryParse(Convert.ToString(Request["IsItemLevelMinMax"]), out IsItemLevelMinMax);

            // set the default column sorting here, if first time then required to set 
            if (sortColumnName == "0" || sortColumnName == "undefined")
                sortColumnName = "ID";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            int TotalDisplayRecordCount = 0;
            DashboardDAL objDashboardDAL = new DashboardDAL(SessionHelper.EnterPriseDBName);
            List<MinMaxDataTableInfo> DataFromDB = new List<MinMaxDataTableInfo>();
            if (Session["SessionMinMaxTable"] != null)
            {
                DataFromDB = (List<MinMaxDataTableInfo>)Session["SessionMinMaxTable"];
                TotalRecordCount = DataFromDB.Count;
            }
            else
            {
                //DataFromDB = objDashboardDAL.GetMinMaxTable(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, AutoMinPer, AutoMaxPer);
                DataFromDB = objDashboardDAL.GetMinMaxTable(0, 1000000, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, AutoMinPer, AutoMaxPer);
                Session["SessionMinMaxTable"] = DataFromDB;
            }

            int MINGreenCounts = DataFromDB.Count(t => t.MinAnalysis == "Green");
            int MINRedCounts = DataFromDB.Count(t => t.MinAnalysis == "Red");
            int MINYelloCounts = DataFromDB.Count(t => t.MinAnalysis == "Yellow");
            int MaxGreenCounts = DataFromDB.Count(t => t.MaxAnalysis == "Green");
            int MaxRedCounts = DataFromDB.Count(t => t.MaxAnalysis == "Red");
            int MaxYelloCounts = DataFromDB.Count(t => t.MaxAnalysis == "Yellow");
            DataFromDB = objDashboardDAL.GetMinMaxFilterdTable(param.sSearch, param.iDisplayStart, param.iDisplayLength, sortColumnName, DataFromDB);
            DataFromDB = DataFromDB.OrderBy(t => t.MinAnalysisOrderNumber).ThenBy(t => t.MaxAnalysisOrderNumber).ToList();

            DataFromDB = DataFromDB.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            TotalDisplayRecordCount = DataFromDB.Count();

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB,
                MinGreenCounts = MINGreenCounts,
                MinRedCounts = MINRedCounts,
                MinYelloCounts = MINYelloCounts,
                MaxGreenCounts = MaxGreenCounts,
                MaxRedCounts = MaxRedCounts,
                MaxYelloCounts = MaxYelloCounts
            }, JsonRequestBehavior.AllowGet);
        }

        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonResult()
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult TurnsListAjax(JQueryDataTableParamModel param)
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
                sortColumnName = "ID";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            DashboardDAL objDashboardDAL = new DashboardDAL(SessionHelper.EnterPriseDBName);
            List<TurnsDataTableInfo> DataFromDB = objDashboardDAL.GetTurnsTable(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID);
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ResetMinMaxTableSessions()
        {
            Session["SessionMinMaxTable"] = null;
            return Json(null);
        }

        [HttpPost]
        public JsonResult SaveCalculatedMinMax(Guid ItemGUID, double MinmumQuantity, double MaximumQuantity)
        {
            string message = "";
            string status = "";
            Session["SessionMinMaxTable"] = null;
            try
            {
                ItemMasterDAL objItem = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                ItemMasterDTO objDTO = new ItemMasterDTO();
                objDTO = objItem.GetItemWithoutJoins(null, ItemGUID);
                objDTO.MinimumQuantity = MinmumQuantity;
                objDTO.MaximumQuantity = MaximumQuantity;
                objDTO.WhatWhereAction = "Dashboard";
                objItem.Edit(objDTO);

            }
            catch (Exception)
            {
                message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);// "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                status = "fail";
            }
            finally
            {
                // resHelper = null;
            }
            message = "Record saved successfully.";
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveCalculatedMinMaxAll()
        {
            string message = "";
            string status = "";
            List<MinMaxDataTableInfo> DataFromDB = (List<MinMaxDataTableInfo>)Session["SessionMinMaxTable"];
            try
            {
                foreach (var item in DataFromDB)
                {
                    ItemMasterDAL objItem = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                    ItemMasterDTO objDTO = new ItemMasterDTO();
                    objDTO = objItem.GetItemWithoutJoins(null, item.GUID);
                    objDTO.MinimumQuantity = item.AutoCurrentMin;
                    objDTO.MaximumQuantity = item.AutoCurrentMax;
                    objDTO.WhatWhereAction = "Dashboard";
                    objItem.Edit(objDTO);
                }
                Session["SessionMinMaxTable"] = null;
                message = "Records saved successfully.";
            }
            catch (Exception)
            {
                message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);// "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                status = "fail";
            }
            finally
            {
                // resHelper = null;
            }
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveCalculatedMinMaxInSelected(string[] ItemGuid, string UpdateMode)
        {
            string message = "";
            string status = "";
            if (UpdateMode.Trim().ToUpper() == "MIN" || UpdateMode.Trim().ToUpper() == "MAX" || UpdateMode.Trim().ToUpper() == "BOTH")
            {
                List<MinMaxDataTableInfo> DataFromDB = (List<MinMaxDataTableInfo>)Session["SessionMinMaxTable"];

                try
                {
                    if (DataFromDB != null && DataFromDB.Count > 0)
                    {
                        DataFromDB = DataFromDB.Where(x => ItemGuid.Contains(x.GUID.ToString())).ToList();

                        foreach (var item in DataFromDB)
                        {
                            ItemMasterDAL objItem = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                            ItemMasterDTO objDTO = new ItemMasterDTO();
                            objDTO = objItem.GetItemWithoutJoins(null, item.GUID);

                            if (UpdateMode.Trim().ToUpper() == "MIN" || UpdateMode.Trim().ToUpper() == "BOTH")
                                objDTO.MinimumQuantity = item.AutoCurrentMin;

                            if (UpdateMode.Trim().ToUpper() == "MAX" || UpdateMode.Trim().ToUpper() == "BOTH")
                                objDTO.MaximumQuantity = item.AutoCurrentMax;

                            objDTO.WhatWhereAction = "Dashboard";
                            objItem.Edit(objDTO);

                        }
                        Session["SessionMinMaxTable"] = null;
                        message = "Records saved successfully.";
                    }
                }
                catch (Exception)
                {
                    message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);// "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                    status = "fail";
                }
                finally
                {
                    // resHelper = null;
                }
            }
            else
            {
                message = "Invalid update mode";
                status = "fail";
            }

            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ResetMinMaxTuningGrid()
        {
            Session["SessionMinMaxTable"] = null;
            return Json(true);
        }
        #endregion

        #region [New Dashboard]

        public ActionResult Dashboard()
        {
            if (SessionHelper.EnterPriceID > 0)
            {
                var objdal = new eTurnsMaster.DAL.DashboardWidgetDAL(SessionHelper.EnterPriseDBName);
                var result = objdal.GetUserWidget(SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID,
                                                  SessionHelper.EnterPriceID, 1);
                if (result != null && !string.IsNullOrEmpty(result.WidgetOrder))
                {
                    var widgetorder = result.WidgetOrder;
                    string[] arr = widgetorder.Split(':');
                    if (arr.Length > 0)
                    {
                        List<string> leftids = arr[0].Split(',').ToList();
                        List<string> rightids = arr[1].Split(',').ToList();
                        ViewBag.LeftWidget = leftids;
                        ViewBag.RightWidget = rightids;
                    }
                }
                else
                {
                    string left = SessionHelper.ParentModuleList.Inventory + "-1" + "," +
                                         SessionHelper.ParentModuleList.Consume + "-1" + "," +
                                         SessionHelper.ModuleList.Count + "-1";

                    string right = SessionHelper.ParentModuleList.Assets + "-1" + "," +
                                           SessionHelper.ParentModuleList.Replenish + "-1" + "," +
                                           SessionHelper.ModuleList.Cart + "-1";


                    List<string> leftids = left.Split(',').ToList();
                    List<string> rightids = right.Split(',').ToList();
                    ViewBag.LeftWidget = leftids;
                    ViewBag.RightWidget = rightids;
                }
            }
            return View();
        }

        [HttpPost]
        public void SaveDashboardWidget(string leftwidget, string rightwidget)
        {
            var objdal = new eTurnsMaster.DAL.DashboardWidgetDAL(SessionHelper.EnterPriseDBName);
            string widgetorder = string.Empty;
            if (!string.IsNullOrEmpty(leftwidget) && !string.IsNullOrEmpty(rightwidget))
            {
                widgetorder = leftwidget + ":" + rightwidget;
            }
            else if (string.IsNullOrEmpty(leftwidget) && !string.IsNullOrEmpty(rightwidget))
            {
                widgetorder = ":" + rightwidget;
            }
            else if (!string.IsNullOrEmpty(leftwidget) && string.IsNullOrEmpty(rightwidget))
            {
                widgetorder = leftwidget + ":";
            }
            objdal.SaveUserWidget(SessionHelper.UserID, widgetorder, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID, 1);


        }

        public ActionResult AuthenticationDb()
        {
            List<UserRoleModuleDetailsDTO> obj = GetModulePermission(Convert.ToInt32(SessionHelper.ParentModuleList.Authentication));
            ViewBag.AuthenticationList = obj;
            return PartialView("AuthenticationDb");
        }
        public ActionResult MasterDb()
        {
            List<UserRoleModuleDetailsDTO> obj = GetModulePermission(Convert.ToInt32(SessionHelper.ParentModuleList.Master));
            ViewBag.MasterList = obj;
            return PartialView("MasterDb");
        }
        public ActionResult AssetsDb()
        {
            bool IsToolView = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ToolMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            bool IsAssetsView = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Assets, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            if (IsToolView || IsAssetsView)
            {
                return PartialView("AssetsDb");
            }
            else
            {
                return null;
            }
        }
        public ActionResult ReportsDb()
        {
            List<UserRoleModuleDetailsDTO> obj = GetModulePermission(Convert.ToInt32(SessionHelper.ParentModuleList.Reports));
            ViewBag.ReportsList = obj;
            return PartialView("ReportsDb");
        }
        public ActionResult InventoryDb()
        {
            bool IsItemView = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ItemMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            if (IsItemView)
            {
                return PartialView("InventoryDb");
            }
            else
            {
                return null;

            }
        }
        public ActionResult ConsumeDb()
        {
            bool IsRequisitionView = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Requisitions, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            bool IsProjectSpendView = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.ProjectMaster, eTurnsWeb.Helper.SessionHelper.PermissionType.View);


            if (IsRequisitionView || IsProjectSpendView)
            {
                return PartialView("ConsumeDb");
            }
            else
            {
                return null;
            }
        }
        public ActionResult ReplenishDb()
        {
            bool IsOrderView = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Orders, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            bool IsTransferView = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Transfer, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            if (IsOrderView || IsTransferView)
            {
                return PartialView("ReplenishDb");
            }
            else
            {
                return null;
            }
        }
        public ActionResult ReceiveDb()
        {
            List<UserRoleModuleDetailsDTO> obj = GetModulePermission(Convert.ToInt32(SessionHelper.ParentModuleList.Receive));
            ViewBag.ReceiveList = obj;
            return PartialView("ReceiveDb");
        }
        public ActionResult KitsDb()
        {
            List<UserRoleModuleDetailsDTO> obj = GetModulePermission(Convert.ToInt32(SessionHelper.ParentModuleList.Kits));
            ViewBag.KitsList = obj;
            return PartialView("KitsDb");
        }
        public ActionResult ConfigurationDb()
        {
            List<UserRoleModuleDetailsDTO> obj = GetModulePermission(Convert.ToInt32(SessionHelper.ParentModuleList.Configuration));
            ViewBag.ConfigurationList = obj;

            return PartialView("ConfigurationDb");
        }
        public ActionResult CategoryCostDb()
        {
            return PartialView("CategoryCostDb");
        }
        public ActionResult CountDb()
        {
            bool IsCountView = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Count, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            if (IsCountView)
            {
                return PartialView("CountDb");
            }
            else
            {
                return null;
            }
        }
        public ActionResult CartDb()
        {
            bool IsCartView = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Cart, eTurnsWeb.Helper.SessionHelper.PermissionType.View);
            if (IsCartView)
            {
                return PartialView("CartDb");
            }
            else
            {
                return null;
            }
        }
        private List<UserRoleModuleDetailsDTO> GetModulePermission(int itemid)
        {
            var lstPermission = SessionHelper.RoomPermissions.Find(element => element.EnterpriseId == SessionHelper.EnterPriceID && element.CompanyId == SessionHelper.CompanyID && element.RoomID == SessionHelper.RoomID);

            List<UserRoleModuleDetailsDTO> returval = (from m in lstPermission.PermissionList
                                                       where m.ParentID == itemid && m.IsView == true
                                                       select m).OrderBy(c => c.DisplayOrderNumber).ThenBy(t => t.ModuleName).ToList();
            if (returval.Any())
                returval = returval.Where(c => c.ModuleID != (Int64)SessionHelper.ModuleList.ResetAutoNumbers && c.ModuleID != (Int64)SessionHelper.ModuleList.Synch && c.ModuleID != (Int64)SessionHelper.ModuleList.Barcode).ToList();
            return returval;
        }

        #endregion

        #region [Datatable scrolling]

        public ActionResult PagingOnScroll()
        {
            return View();
        }
        public ActionResult PagingOnScrollAjax()
        {


            int TotalRecordCount = 0;
            DashboardDAL objDashboardDAL = new DashboardDAL(SessionHelper.EnterPriseDBName);
            List<MinMaxDataTableInfo> DataFromDB = new List<MinMaxDataTableInfo>();
            if (Session["SessionMinMaxTable"] != null)
            {
                DataFromDB = (List<MinMaxDataTableInfo>)Session["SessionMinMaxTable"];
            }
            else
            {
                DataFromDB = objDashboardDAL.GetMinMaxTable(0, 10, out TotalRecordCount, string.Empty, "ID DESC", SessionHelper.RoomID, SessionHelper.CompanyID, 12, 34);
                Session["SessionMinMaxTable"] = DataFromDB;
            }
            int MINGreenCounts = DataFromDB.Count(t => t.MinAnalysis == "Green");
            int MINRedCounts = DataFromDB.Count(t => t.MinAnalysis == "Red");
            int MINYelloCounts = DataFromDB.Count(t => t.MinAnalysis == "Yellow");
            int MaxGreenCounts = DataFromDB.Count(t => t.MaxAnalysis == "Green");
            int MaxRedCounts = DataFromDB.Count(t => t.MaxAnalysis == "Red");
            int MaxYelloCounts = DataFromDB.Count(t => t.MaxAnalysis == "Yellow");
            DataFromDB = objDashboardDAL.GetMinMaxFilterdTable("", 0, 10, "ID DESC", DataFromDB);
            return Json(
                new
                {

                    iTotalRecords = TotalRecordCount,
                    iTotalDisplayRecords = TotalRecordCount,
                    aaData = DataFromDB,
                    MinGreenCounts = MINGreenCounts,
                    MinRedCounts = MINRedCounts,
                    MinYelloCounts = MINYelloCounts,
                    MaxGreenCounts = MaxGreenCounts,
                    MaxRedCounts = MaxRedCounts,
                    MaxYelloCounts = MaxYelloCounts
                }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region [Test Method]

        public ActionResult SpecialCharactersTest()
        {
            return View();
        }

        [HttpPost]
        public JsonResult RetriveSecialCharacters(List<UserTypeInfo> obj)
        {
            return Json(null);
        }

        public ActionResult RecalcWholeRoomItemTurnsUsage()
        {
            DashboardDAL objDashboardDAL = new DashboardDAL(SessionHelper.EnterPriseDBName);
            RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);

            DashboardParameterDTO objDashboardParameterDTO = objDashboardDAL.GetDashboardParameters(SessionHelper.RoomID, SessionHelper.CompanyID);
            eTurnsRegionInfo objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(SessionHelper.RoomID, SessionHelper.CompanyID, 0);
            using (var context = new eTurnsEntities(WebConnectionHelper.DataBaseEntityConnectionString))
            {
                if (objDashboardParameterDTO != null)
                {
                    Guid[] objItem = context.ItemMasters.Where(t => t.Room == SessionHelper.RoomID && t.CompanyID == SessionHelper.CompanyID && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false).Select(t => t.GUID).ToArray();
                    if (objItem.Any())
                    {
                        foreach (var item in objItem)
                        {
                            objDashboardDAL.UpdateTurnsByItemGUIDAfterTxn(SessionHelper.RoomID, SessionHelper.CompanyID, item, SessionHelper.UserID, objDashboardParameterDTO, objRegionalSettings);
                            objDashboardDAL.UpdateAvgUsageByItemGUIDAfterTxn(SessionHelper.RoomID, SessionHelper.CompanyID, item, SessionHelper.UserID, objDashboardParameterDTO, objRegionalSettings);
                        }
                    }
                }
            }


            return RedirectToAction("ItemMasterList", "Inventory");
        }

        //public ActionResult ShowTwoFactorSecret()
        //{
        //    string secret = TwoFactorProfile.CurrentUser.TwoFactorSecret;

        //    if (string.IsNullOrEmpty(secret))
        //    {
        //        byte[] buffer = new byte[9];

        //        using (RandomNumberGenerator rng = RNGCryptoServiceProvider.Create())
        //        {
        //            rng.GetBytes(buffer);
        //        }

        //        // Generates a 10 character string of A-Z, a-z, 0-9
        //        // Don't need to worry about any = padding from the
        //        // Base64 encoding, since our input buffer is divisible by 3
        //        TwoFactorProfile.CurrentUser.TwoFactorSecret = Convert.ToBase64String(buffer).Substring(0, 10).Replace('/', '0').Replace('+', '1');

        //        secret = TwoFactorProfile.CurrentUser.TwoFactorSecret;
        //    }

        //    var enc = new Base32Encoder().Encode(Encoding.ASCII.GetBytes(secret));

        //    return View(new TwoFactorSecret { EncodedSecret = enc });
        //}
        #endregion

        #region [Permission Templates]

        public ActionResult PermissionTemplatesList()
        {
            return View();
        }

        public ActionResult PermissionTemplateCreate()
        {
            PermissionTemplateDTO objDTO = new PermissionTemplateDTO();
            objDTO.ID = 0;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.UpdatedBy = SessionHelper.UserID;
            eTurns.DAL.PermissionTemplateDAL obj = new eTurns.DAL.PermissionTemplateDAL(SessionHelper.EnterPriseDBName);
            List<PermissionTemplateDetailDTO> objList = obj.GetPermissionDetailsRecord(objDTO.ID);
            foreach (var item in objList)
            {
                if (item.GroupId == 3)
                {
                    item.DisplayOrderName = ResourceHelper.GetResourceValue(Enum.GetName(typeof(RoleOrderStatus), item.DisplayOrderNumber).ToString(), "ResModuleName");
                }
            }
            objDTO.lstPermissions = objList.OrderBy(t => t.GroupId).ThenBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleNameFromResource).ToList();
            objDTO.IlstPermissions = objList.OrderBy(t => t.GroupId).ThenBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleNameFromResource).ToList();
            return PartialView("_CreatePermissionTemplate", objDTO);
        }

        public ActionResult PermissionTemplatesEdit(Int64 ID)
        {
            PermissionTemplateDAL objPermissionTemplateDAL = new PermissionTemplateDAL(SessionHelper.EnterPriseDBName);
            PermissionTemplateDTO objDTO = objPermissionTemplateDAL.GetTemplateByID(ID);
            List<PermissionTemplateDetailDTO> objList = objPermissionTemplateDAL.GetPermissionDetailsRecord(objDTO.ID);
            if (objList == null || objList.Count == 0)
            {
                objList = objPermissionTemplateDAL.GetPermissionDetailsRecord(0);
            }
            foreach (var item in objList)
            {
                if (item.GroupId == 3)
                {
                    item.DisplayOrderName = ResourceHelper.GetResourceValue(Enum.GetName(typeof(RoleOrderStatus), item.DisplayOrderNumber).ToString(), "ResModuleName");
                }
            }
            objDTO.lstPermissions = objList.OrderBy(t => t.GroupId).ThenBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleNameFromResource).ToList();
            objDTO.IlstPermissions = objList.OrderBy(t => t.GroupId).ThenBy(t => t.DisplayOrderNumber).ThenBy(t => t.ModuleNameFromResource).ToList();
            if (objDTO != null)
            {
                objDTO.CreatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                objDTO.UpdatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
            }
            return PartialView("_CreatePermissionTemplate", objDTO);
        }

        [HttpPost]
        public JsonResult PermissionTemplatesSave(PermissionTemplateDTO objPermissionTemplateDTO, List<eTurns.DTO.PermissionTemplateDetailDTO> lstpermissions)
        {
            string message = string.Empty, status = string.Empty;
            objPermissionTemplateDTO.lstPermissions = lstpermissions;
            PermissionTemplateDAL objPermissionTemplateDAL = new PermissionTemplateDAL(SessionHelper.EnterPriseDBName);
            objPermissionTemplateDTO.EnterpriseID = SessionHelper.EnterPriceID;
            objPermissionTemplateDTO.CreatedBy = SessionHelper.UserID;
            objPermissionTemplateDTO.UpdatedBy = SessionHelper.UserID;

            try
            {
                bool Result = true;

                Result = objPermissionTemplateDAL.CheckPermissionTemplateDuplicate(objPermissionTemplateDTO.TemplateName.Trim(), SessionHelper.EnterPriceID, objPermissionTemplateDTO.ID);
                if (!Result)
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResModuleMaster.TemplateName, objPermissionTemplateDTO.TemplateName);  //"Module name\"" + objDTO.ModuleName + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    objPermissionTemplateDTO = objPermissionTemplateDAL.SaveTemplate(objPermissionTemplateDTO);
                    message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                    status = "ok";
                }
            }
            catch (Exception)
            {
                message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                status = "fail";
            }
            return Json(new { Message = message, Status = status, objDTO = objPermissionTemplateDAL }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PermissionTemplateListAjax(JQueryDataTableParamModel param)
        {
            PermissionTemplateDAL objPermissionTemplateDAL = new PermissionTemplateDAL(SessionHelper.EnterPriseDBName);
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

            if (sortColumnName == "0" || sortColumnName == "undefined" || string.IsNullOrEmpty(sortColumnName))
                sortColumnName = "TemplateName Asc";



            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            List<PermissionTemplateDTO> DataFromDB = objPermissionTemplateDAL.GetPagedTemplates(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, IsDeleted);

            if (DataFromDB != null)
            {
                DataFromDB.ToList().ForEach(t =>
                {
                    t.CreatedDate = CommonUtility.ConvertDateByTimeZone(t.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                    t.UpdatedDate = CommonUtility.ConvertDateByTimeZone(t.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                });
            }

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            }, JsonRequestBehavior.AllowGet);


        }

        public ActionResult LoadTemplatePermission(long TemlatePermissionID)
        {
            List<PermissionTemplateDetailDTO> lstPermissionMap = new List<PermissionTemplateDetailDTO>();
            return PartialView("_CreatePermissionTemplateDetails", lstPermissionMap);
        }


        #endregion

        #region [Global Settings]


        public ActionResult GlobalSettings()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GlobalSettings(GlobalSettingDTO objGlobalSettingDTO)
        {
            return View();
        }

        public ActionResult GetIlist()
        {
            List<SelectListItem> lstItems = new List<SelectListItem>();
            lstItems.Add(new SelectListItem() { Text = "txt1", Value = "1", Selected = false });
            lstItems.Add(new SelectListItem() { Text = "txt1", Value = "1", Selected = false });
            lstItems.Add(new SelectListItem() { Text = "txt2", Value = "2", Selected = false });
            lstItems.Add(new SelectListItem() { Text = "txt3", Value = "3", Selected = false });
            lstItems.Add(new SelectListItem() { Text = "txt4", Value = "4", Selected = false });
            lstItems.Add(new SelectListItem() { Text = "txt5", Value = "5", Selected = false });
            IList<SelectListItem> IlstItems = lstItems;
            IlstItems = (from itm in IlstItems
                         group itm by new { itm.Text, itm.Value } into Groupeditems
                         select new SelectListItem
                         {
                             Text = Groupeditems.Key.Text,
                             Value = Groupeditems.Key.Value
                         }).ToList();

            return View(IlstItems);
        }

        public ActionResult SiteSettings()
        {
            if (SessionHelper.RoleID == -1)
            {
                XElement Settinfile = XElement.Load(Server.MapPath("/SiteSettings.xml"));
                string LoginWithSelection = Settinfile.Element("LoginWithSelection").Value;
                ViewBag.chkLoginWithSelection = LoginWithSelection == "1" ? true : false;
                return View();
            }
            else
            {
                string CtrlName = Convert.ToString(ConfigurationManager.AppSettings["CtrlName"]);
                string ActName = Convert.ToString(ConfigurationManager.AppSettings["ActName"]);
                return RedirectToAction(ActName, CtrlName);
            }
        }

        [HttpPost]
        public JsonResult UpdateSettings(bool OverwriteExisting)
        {
            string msg = string.Empty;
            if (SessionHelper.RoleID == -1)
            {

                string valueToUpdate = OverwriteExisting ? "1" : "0";
                XElement Settinfile = XElement.Load(Server.MapPath("/SiteSettings.xml"));
                Settinfile.Element("LoginWithSelection").Value = valueToUpdate;
                Settinfile.Save(Server.MapPath("/SiteSettings.xml"));
                msg = "Updated Successfully.";
                return Json(new { Status = true, Message = msg }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                msg = "You do not have permission.";
                return Json(new { Status = "norights", Message = msg }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region [Eturns Email templates]


        public ActionResult eTurnsEmailConfiguration()
        {
            List<SelectListItem> lstLanguage = GeteturnsLanguage();
            ViewBag.DDLanguage = lstLanguage;
            //List<SelectListItem> lstEmailTemp = GetEmailTemplateList(lstLanguage[0].Value);

            EmailTemplateDAL objEmailTemplate = new EmailTemplateDAL(DbConHelper.GetETurnsDBName());
            List<EmailTemplateDTO> lstEmailTemp = objEmailTemplate.GetAllEmailTemplate();
            ViewBag.DDLEmailTemplate = lstEmailTemp;
            return View();
        }

        public JsonResult SaveeturnsEmailTemplate(string EmailTemplateName, string EmailText, string CurCulture, string EmailSubject)
        {
            string message = "";
            EmailTemplateDetailDTO objEmailTemplateDeailDTO = new EmailTemplateDetailDTO();
            EmailTemplateDAL objEmailTempateDetail = new EmailTemplateDAL(DbConHelper.GetETurnsDBName());

            objEmailTemplateDeailDTO.EmailTempateId = Convert.ToInt64(EmailTemplateName);
            objEmailTemplateDeailDTO.MailBodyText = HttpUtility.UrlDecode(EmailText);
            objEmailTemplateDeailDTO.ResourceLaguageId = objEmailTempateDetail.GeteturnsLanguageId(CurCulture);
            objEmailTemplateDeailDTO.MailSubject = EmailSubject;
            objEmailTemplateDeailDTO.RoomId = 0;
            objEmailTemplateDeailDTO.CompanyID = 0;
            objEmailTemplateDeailDTO.CreatedBy = SessionHelper.UserID;
            objEmailTemplateDeailDTO.LastUpdatedBy = SessionHelper.UserID;
            objEmailTemplateDeailDTO.Created = DateTimeUtility.DateTimeNow;
            objEmailTemplateDeailDTO.Updated = DateTimeUtility.DateTimeNow;

            if (objEmailTempateDetail.SaveEmailTemplate(objEmailTemplateDeailDTO))
                message = ResMessage.SaveMessage; //ResMessage.SaveMessage; //"Record Saved Sucessfully...";
            else
                message = ResMessage.SaveErrorMsg;
            return Json(new { Message = message }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GeteTurnsEmailTemplateText(string EmailTemplateName, string CurCulture)
        {
            StringBuilder MessageBody = new StringBuilder();
            string EmailSubject = string.Empty;
            //if (EmailTemplateName.Contains("OrderApproval") && CurCulture == "en-US")
            //{
            //    MessageBody.Append(System.IO.File.ReadAllText(Server.MapPath("../") + "Content\\EmailTemplates/" + EmailTemplateName + "-" + CurCulture + ".xslt"));
            //}
            //else
            //long TemplateId = GetTemplateId(EmailTemplateName);

            EmailTemplateDAL objEmailTemplate = new EmailTemplateDAL(DbConHelper.GetETurnsDBName());
            EmailTemplateDetail objEmailTempateDetail = new EmailTemplateDetail();
            long TemplateId = objEmailTemplate.GetTemplateId(EmailTemplateName);

            objEmailTempateDetail = objEmailTemplate.GetTemplateDetail(TemplateId, objEmailTemplate.GeteturnsLanguageId(CurCulture), 0, 0);
            if (objEmailTempateDetail != null && objEmailTempateDetail.EmailTemplateId > 0)
            {
                MessageBody.Append(Convert.ToString(objEmailTempateDetail.MailBodyText));
                EmailSubject = objEmailTempateDetail.MailSubject;
            }
            else
            {
                if (System.IO.File.Exists(Server.MapPath("../") + "Content\\EmailTemplates\\" + EmailTemplateName + "-" + CurCulture + ".html"))
                {
                    MessageBody.Append(System.IO.File.ReadAllText(Server.MapPath("../") + "Content\\EmailTemplates/" + EmailTemplateName + "-" + CurCulture + ".html"));
                }

            }

            return Json(new { MessageBody = MessageBody.ToString(), EmailSubject = EmailSubject }, JsonRequestBehavior.AllowGet);

        }

        private List<SelectListItem> GeteturnsLanguage()
        {
            //ResourceController resApiController = null;
            ResourceDAL resApiController = null;
            IEnumerable<ResourceLanguageDTO> resLangDTO = null;
            List<SelectListItem> lstItem = null;
            try
            {
                resApiController = new ResourceDAL(SessionHelper.EnterPriseDBName);
                resLangDTO = resApiController.GeteTurnsLanguages();
                lstItem = new List<SelectListItem>();
                foreach (var item in resLangDTO)
                {
                    SelectListItem obj = new SelectListItem();
                    obj.Text = item.Language;
                    obj.Value = item.Culture;
                    lstItem.Add(obj);
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
                resApiController = null;
                resLangDTO = null;
                lstItem = null;

            }

        }
        #endregion

        #region [Enterprise Email templates]

        public ActionResult EpEmailConfiguration()
        {
            List<SelectListItem> lstLanguage = GeteturnsLanguage();
            ViewBag.DDLanguage = lstLanguage;
            //List<SelectListItem> lstEmailTemp = GetEmailTemplateList(lstLanguage[0].Value);
            EmailTemplateDAL objEmailTemplate = new EmailTemplateDAL(SessionHelper.EnterPriseDBName);
            List<EmailTemplateDTO> lstEmailTemp = objEmailTemplate.GetAllEmailTemplate();
            ViewBag.DDLEmailTemplate = lstEmailTemp;
            return View();
        }

        public JsonResult SaveEpEmailTemplate(string EmailTemplateName, string EmailText, string CurCulture, string EmailSubject)
        {
            string message = "";
            EmailTemplateDetailDTO objEmailTemplateDeailDTO = new EmailTemplateDetailDTO();
            EmailTemplateDAL objEmailTempateDetail = new EmailTemplateDAL(SessionHelper.EnterPriseDBName);

            objEmailTemplateDeailDTO.EmailTempateId = Convert.ToInt64(EmailTemplateName);
            objEmailTemplateDeailDTO.MailBodyText = HttpUtility.UrlDecode(EmailText);
            objEmailTemplateDeailDTO.ResourceLaguageId = objEmailTempateDetail.GeteturnsLanguageId(CurCulture);
            objEmailTemplateDeailDTO.MailSubject = EmailSubject;
            objEmailTemplateDeailDTO.RoomId = 0;
            objEmailTemplateDeailDTO.CompanyID = 0;
            objEmailTemplateDeailDTO.CreatedBy = SessionHelper.UserID;
            objEmailTemplateDeailDTO.LastUpdatedBy = SessionHelper.UserID;
            objEmailTemplateDeailDTO.Created = DateTimeUtility.DateTimeNow;
            objEmailTemplateDeailDTO.Updated = DateTimeUtility.DateTimeNow;

            if (objEmailTempateDetail.SaveEmailTemplate(objEmailTemplateDeailDTO))
                message = ResMessage.SaveMessage; //ResMessage.SaveMessage; //"Record Saved Sucessfully...";
            else
                message = ResMessage.SaveErrorMsg;
            return Json(new { Message = message }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEpEmailTemplateText(string EmailTemplateName, string CurCulture)
        {
            StringBuilder MessageBody = new StringBuilder();
            string EmailSubject = string.Empty;
            //if (EmailTemplateName.Contains("OrderApproval") && CurCulture == "en-US")
            //{
            //    MessageBody.Append(System.IO.File.ReadAllText(Server.MapPath("../") + "Content\\EmailTemplates/" + EmailTemplateName + "-" + CurCulture + ".xslt"));
            //}
            //else
            //long TemplateId = GetTemplateId(EmailTemplateName);

            EmailTemplateDAL objEmailTemplate = new EmailTemplateDAL(SessionHelper.EnterPriseDBName);
            EmailTemplateDetail objEmailTempateDetail = new EmailTemplateDetail();
            long TemplateId = objEmailTemplate.GetTemplateId(EmailTemplateName);

            objEmailTempateDetail = objEmailTemplate.GetTemplateDetail(TemplateId, objEmailTemplate.GeteturnsLanguageId(CurCulture), 0, 0);
            if (objEmailTempateDetail != null && objEmailTempateDetail.EmailTemplateId > 0)
            {
                MessageBody.Append(Convert.ToString(objEmailTempateDetail.MailBodyText));
                EmailSubject = objEmailTempateDetail.MailSubject;
            }
            else
            {
                if (System.IO.File.Exists(Server.MapPath("../") + "Content\\EmailTemplates\\" + EmailTemplateName + "-" + CurCulture + ".html"))
                {
                    MessageBody.Append(System.IO.File.ReadAllText(Server.MapPath("../") + "Content\\EmailTemplates/" + EmailTemplateName + "-" + CurCulture + ".html"));
                }

            }

            return Json(new { MessageBody = MessageBody.ToString(), EmailSubject = EmailSubject }, JsonRequestBehavior.AllowGet);

        }

        private List<SelectListItem> GetEpLanguage()
        {
            //ResourceController resApiController = null;
            ResourceDAL resApiController = null;
            IEnumerable<ResourceLanguageDTO> resLangDTO = null;
            List<SelectListItem> lstItem = null;
            try
            {
                resApiController = new ResourceDAL(SessionHelper.EnterPriseDBName);
                resLangDTO = resApiController.GeteTurnsLanguages();
                lstItem = new List<SelectListItem>();
                foreach (var item in resLangDTO)
                {
                    SelectListItem obj = new SelectListItem();
                    obj.Text = item.Language;
                    obj.Value = item.Culture;
                    lstItem.Add(obj);
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
                resApiController = null;
                resLangDTO = null;
                lstItem = null;

            }

        }

        #endregion

        #region [FTP Master]

        public ActionResult SFTPList()
        {
            return View();
        }

        public ActionResult SFTPCreate()
        {
            FTPMasterDTO objFTPMasterDTO = new FTPMasterDTO();
            objFTPMasterDTO.Port = 21;
            return PartialView("_CreateFTP", objFTPMasterDTO);
        }

        public ActionResult SFTPEdit(Int64 ID)
        {
            SFTPDAL objSFTPDAL = new SFTPDAL(SessionHelper.EnterPriseDBName);
            FTPMasterDTO objFTPMasterDTO = objSFTPDAL.GetFtpByID(ID, SessionHelper.RoomID, SessionHelper.CompanyID);
            return PartialView("_CreateFTP", objFTPMasterDTO);
        }

        public ActionResult SFTPSave(FTPMasterDTO objFTPMasterDTO)
        {
            SFTPDAL objSFTPDAL = new SFTPDAL(SessionHelper.EnterPriseDBName);
            string message = string.Empty;
            string status = string.Empty;
            try
            {
                objFTPMasterDTO.CompanyId = SessionHelper.CompanyID;
                objFTPMasterDTO.CreatedBy = SessionHelper.UserID;
                objFTPMasterDTO.Created = DateTime.UtcNow;
                objFTPMasterDTO.EnterpriseID = SessionHelper.EnterPriceID;
                objFTPMasterDTO.GUID = Guid.NewGuid();
                objFTPMasterDTO.IsArchived = false;
                objFTPMasterDTO.IsDeleted = false;
                objFTPMasterDTO.RoomId = SessionHelper.RoomID;
                objFTPMasterDTO.UpdatedBy = SessionHelper.UserID;
                objFTPMasterDTO.LastUpdated = DateTime.UtcNow;
                objFTPMasterDTO.UserID = SessionHelper.UserID;

                string strOK = objSFTPDAL.FTPDuplicateCheck(objFTPMasterDTO.ID, objFTPMasterDTO.SFtpName, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResFTPMaster.SFtpName, objFTPMasterDTO.SFtpName);
                    status = "duplicate";
                }
                else
                {
                    strOK = objSFTPDAL.FTPServerDuplicateCheck(objFTPMasterDTO.ID, objFTPMasterDTO.ServerAddress, SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (strOK == "duplicate")
                    {
                        message = string.Format(ResMessage.DuplicateMessage, ResFTPMaster.ServerAddress, objFTPMasterDTO.ServerAddress);
                        status = "duplicate";
                    }
                    else
                    {
                        objFTPMasterDTO = objSFTPDAL.SaveFTPDetails(objFTPMasterDTO);
                        message = ResMessage.SaveMessage;
                        status = "ok";
                    }
                }
                return Json(new { Message = message, Status = status, objFTPDTO = objFTPMasterDTO }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                message = ResMessage.SaveErrorMsg;
                status = "fail";
                return Json(new { Message = message, Status = status, objFTPDTO = objFTPMasterDTO }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetFtpListAjax(JQueryDataTableParamModel param)
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
            if (sortColumnName == "0" || sortColumnName == "undefined")
                sortColumnName = "FtpName Asc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;

            SFTPDAL objSFTPDAL = new SFTPDAL(SessionHelper.EnterPriseDBName);

            List<FTPMasterDTO> DataFromDB = objSFTPDAL.GetPagedFTPRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted);


            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteFTPMasterRecords(string ids)
        {
            try
            {
                string response = string.Empty;
                SFTPDAL objSFTPDAL = new SFTPDAL(SessionHelper.EnterPriseDBName);
                Dictionary<string, string> result = objSFTPDAL.DeleteFtpRecords(ids, SessionHelper.RoomID, SessionHelper.CompanyID);
                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult UnDeleteFTPMasterRecords(string ids)
        {
            try
            {
                string response = string.Empty;
                SFTPDAL objSFTPDAL = new SFTPDAL(SessionHelper.EnterPriseDBName);
                Dictionary<string, string> result = objSFTPDAL.UnDeleteFtpRecords(ids, SessionHelper.RoomID, SessionHelper.CompanyID);
                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        public JsonResult HideMessage(bool isChecked)
        {
            try
            {

                eTurnsMaster.DAL.UserMasterDAL objeTurnsMaster = new eTurnsMaster.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                objeTurnsMaster.HideMessage(isChecked, Convert.ToInt32(SessionHelper.UserID));
                return Json(new { Status = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = ex.Message.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public void ConvertentEnterpriseAdmin(Int64 UserId, Int64? EnterpriseId)
        {
            eTurnsMaster.DAL.UserMasterDAL objUserMastereTurnsMasterDAL = new eTurnsMaster.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
            objUserMastereTurnsMasterDAL.ConvertentEnterpriseAdmin(UserId);

            eTurns.DAL.UserMasterDAL objUserMasterDAL;
            EnterpriseDTO oEnt = new EnterpriseMasterDAL().GetEnterprise(EnterpriseId ?? 0);
            if (oEnt != null)
                objUserMasterDAL = new eTurns.DAL.UserMasterDAL(oEnt.EnterpriseDBName);
            else
                objUserMasterDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);

            objUserMasterDAL.EditRoleByUserId(UserId, -2);
        }
        [HttpPost]
        public JsonResult AssignNormalRole(Int64 UserId, Int64? EnterpriseId)
        {
            eTurnsMaster.DAL.UserMasterDAL objUserMastereTurnsMasterDAL = new eTurnsMaster.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
            Int64 RoleId = objUserMastereTurnsMasterDAL.AssignNormalRole(UserId);
            if (RoleId != -999 && RoleId != -998)
            {
                eTurns.DAL.UserMasterDAL objUserMasterDAL;
                EnterpriseDTO oEnt = new EnterpriseMasterDAL().GetEnterprise(EnterpriseId ?? 0);
                if (oEnt != null)
                    objUserMasterDAL = new eTurns.DAL.UserMasterDAL(oEnt.EnterpriseDBName);
                else
                    objUserMasterDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);

                objUserMasterDAL.EditRoleByUserId(UserId, RoleId);
                return Json(new { Status = "Success" }, JsonRequestBehavior.AllowGet);
            }
            else if (RoleId == -998)
            {
                return Json(new { Status = "cantrevert" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = "fail" }, JsonRequestBehavior.AllowGet);
        }

        #region EULA Module

        public ActionResult EulaList()
        {
            if (SessionHelper.RoleID == -1)
            {
                return View();
            }
            else
            {
                return RedirectToAction(ActName, CtrlName);
            }
        }
        [HttpPost]
        public JsonResult InserNewEula(string EulaName, string EulaDescription, Int64 EulaID)
        {
            if (SessionHelper.RoleID == -1)
            {
                EulaMasterDAL objEulaMasterController = new EulaMasterDAL();
                EulaMasterDTO objEulaMasterDTO = new EulaMasterDTO();
                objEulaMasterController = new EulaMasterDAL();
                List<EulaMasterDTO> objEulaList = objEulaMasterController.GetAllRecords(false, false).Where(x => x.EulaName.ToLower() == EulaName.ToLower()).ToList();

                if (objEulaList != null && objEulaList.Count() > 0)
                {
                    return Json(new { IsSuccess = false, Massage = string.Format(ResMessage.DuplicateMessage, "Eula Name", EulaName), RetrunDTO = objEulaMasterDTO }, JsonRequestBehavior.AllowGet);

                }
                if (EulaID == 0)
                {
                    objEulaMasterDTO.EulaName = EulaName;
                    objEulaMasterDTO.EulaDescription = EulaDescription;
                    objEulaMasterDTO.CreatedBy = SessionHelper.UserID;
                    objEulaMasterDTO.UpdatedBy = SessionHelper.UserID;
                    objEulaMasterDTO.CreatedOn = DateTimeUtility.DateTimeNow;
                    objEulaMasterDTO.UpdatedOn = DateTimeUtility.DateTimeNow;
                    objEulaMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objEulaMasterDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objEulaMasterDTO.AddedFrom = "Web";
                    objEulaMasterDTO.EditedFrom = "Web";
                    objEulaMasterDTO.ID = objEulaMasterController.Insert(objEulaMasterDTO);
                }
                return Json(new { IsSuccess = true, Massage = ResMessage.SaveMessage, RetrunDTO = objEulaMasterDTO }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { IsSuccess = false, Massage = "You do not have rights. Please contact administrator.", RetrunDTO = new EulaMasterDTO() }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]

        public PartialViewResult GetEulaList(string SearchText)
        {
            if (SessionHelper.RoleID == -1)
            {
                //BarcodeMasterController objBarCodeMasterController = new BarcodeMasterController();
                EulaMasterDAL objEulaMasterController = new EulaMasterDAL();
                List<EulaMasterDTO> objDTO = null;
                objDTO = objEulaMasterController.GetAllRecords(false, false).ToList();
                if (SearchText != null && (!string.IsNullOrEmpty(SearchText)))
                {
                    objDTO = objDTO.Where(o => o.EulaName.Contains(SearchText) || o.EulaDescription.Contains(SearchText)).ToList();
                }
                if (objDTO != null && objDTO.Count() <= 0)
                    objDTO = null;

                if (objDTO != null && objDTO.Count() > 0)
                {
                    objDTO.ToList().ForEach(t =>
                    {
                        t.CreatedDate = CommonUtility.ConvertDateByTimeZone(t.CreatedOn, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                        t.UpdatedDate = CommonUtility.ConvertDateByTimeZone(t.UpdatedOn, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                    });

                }
                return PartialView("_EulaList", objDTO);
            }
            else
            {
                return PartialView(ActName, CtrlName);
            }
        }
        [HttpPost]
        public JsonResult DeleteEulaRecords(string IDs)
        {
            if (SessionHelper.RoleID == -1)
            {
                EulaMasterDAL objEulaController = null;
                try
                {
                    objEulaController = new EulaMasterDAL();
                    objEulaController.DeleteRecords(IDs, SessionHelper.UserID);
                    return Json(new { IsSuccess = true, Massage = "Success" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { IsSuccess = false, Massage = ex.Message }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    objEulaController = null;
                }
            }
            else
            {
                return Json(new { IsSuccess = false, Massage = "You do not have rights. Please contact administrator.", RetrunDTO = new EulaMasterDTO() }, JsonRequestBehavior.AllowGet);
            }

        }
        #endregion
        public string ValidateUserName(string UserName, Int64 UserID)
        {
            try
            {
                CommonMasterDAL objCDAL = new CommonMasterDAL();
                string strOK = objCDAL.UserDuplicateCheckUserName(UserID, UserName);
                return strOK;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public ActionResult ResourceLanguage()
        {
            if (SessionHelper.RoleID == -1)
            {
                ResourceDAL objResourceDAL = new ResourceDAL(SessionHelper.EnterPriseDBName);
                List<ResourceLanguageDTO> objResourceLanguageList = new List<ResourceLanguageDTO>();
                objResourceLanguageList = objResourceDAL.GetResourceLanguageDataFromXML(Convert.ToString(Server.MapPath("/Content/LanguageResource.xml"))).ToList();
                List<ResourceLanguageDTO> objList = objResourceDAL.GetETurnsResourceLanguageData().ToList();

                objResourceLanguageList.RemoveAll(x => objList.Any(y => y.Culture == x.Culture));

                objResourceLanguageList.Insert(0, new ResourceLanguageDTO { Culture = "", Language = "Select Language" });
                ViewBag.ResourceLanguage = objResourceLanguageList;
                return View();
            }
            else
            {
                return RedirectToAction(ActName, CtrlName);
            }
        }
        [HttpPost]
        public PartialViewResult GetAddedLanguage(string SearchText)
        {
            if (SessionHelper.RoleID == -1)
            {
                ResourceDAL objResourceDAL = new ResourceDAL(SessionHelper.EnterPriseDBName);
                List<ResourceLanguageDTO> objList = objResourceDAL.GetETurnsResourceLanguageData().ToList();
                if (!string.IsNullOrEmpty(SearchText))
                {
                    objList = objList.Where(l => l.Culture.ToLower().Contains(SearchText.ToLower()) || l.Language.ToLower().Contains(SearchText.ToLower())).ToList();
                }
                return PartialView("_ResoruceLanguage", objList);
            }
            else
            {
                return PartialView(ActName, CtrlName);
            }
        }
        public string AddLanguageeTurns(string Culture, string Language)
        {
            if (SessionHelper.RoleID == -1)
            {
                string res = string.Empty;
                try
                {
                    Language = Language.Replace("\t", "");
                    ResourceDAL objResourceDAL = new ResourceDAL(DbConHelper.GetETurnsDBName());
                    ResourceLanguageDTO objResourceLanguageDTO = new ResourceLanguageDTO();
                    objResourceLanguageDTO.Culture = Culture;
                    objResourceLanguageDTO.Language = Language;
                    objResourceDAL.InsertLanguageeTurns(objResourceLanguageDTO);

                    res = "ok";
                }
                catch (Exception ex)
                {
                    return ex.Message.ToString();
                }
                return res;
            }
            else
            {
                return "User does not have rights";
            }
        }

        public ActionResult UserSetting()
        {
            UserSettingDTO objUserSettingDTO = new UserSettingDTO();
            eTurnsMaster.DAL.UserSettingDAL objUserSettingDAL = new eTurnsMaster.DAL.UserSettingDAL(SessionHelper.EnterPriseDBName);
            objUserSettingDTO = objUserSettingDAL.GetByUserId(SessionHelper.UserID);
            return View(objUserSettingDTO);
        }
        [HttpPost]
        public JsonResult UserSettingPageSave(eTurns.DTO.UserSettingDTO objUserSettingDTO)
        {

            try
            {
                eTurnsMaster.DAL.UserSettingDAL objUserMasterDAL = new eTurnsMaster.DAL.UserSettingDAL(SessionHelper.EnterPriseDBName);
                objUserMasterDAL.Update(objUserSettingDTO);
                return Json(new { IsSuccess = true, Message = "User Setting saved Successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Message = ex.Message.ToString() });
            }
        }
    }



    public class TwoFactorSecret
    {
        public string EncodedSecret { get; set; }
    }

    public class ASDN
    {
        public long ID { get; set; }
        public string lotNumber { get; set; }
        public string SerialNumber { get; set; }
        public double ConsignedQty { get; set; }
        public double CustOwnedQty { get; set; }
        public DateTime ReceivedDate { get; set; }
    }

    public class ASD
    {
        public int ID { get; set; }
        public Guid ItemGuid { get; set; }
        public string ItemNumber { get; set; }
        public List<ASDN> lstASDN { get; set; }
    }
}
