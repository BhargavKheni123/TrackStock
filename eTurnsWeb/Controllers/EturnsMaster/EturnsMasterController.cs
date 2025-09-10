using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eTurnsWeb.Controllers.WebAPI;
using eTurns.DTO;
using System.Net.Http;
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
using System.Data.OleDb;
using System.Data.SqlClient;
using eTurns.DAL;
using System.Net;
using eTurnsMaster.DAL;
//using eTurnsWeb.Controllers.WebAPI.EturnsMaster;

namespace eTurnsWeb.Controllers
{
    public class EturnsMasterController : Controller
    {

        public ActionResult UserLogin()
        {
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
            EnterPriseUserMasterDAL objEnterPriseUserMasterDAL = new EnterPriseUserMasterDAL(SessionHelper.EnterPriseDBName);
            UserMasterController objUserMasterController = new UserMasterController();
            IEnumerable<UserMasterDTO> lstUsers = null;
            if (ViewBag.IsMasterDbLogin == "1")
            {
                lstUsers = objEnterPriseUserMasterDAL.GetAllUsers();
                ViewBag.AllUsers = lstUsers.ToList();
            }
            else
            {
                lstUsers = objUserMasterController.GetAllUsers();
                ViewBag.AllUsers = lstUsers.ToList();
            }
            return View();
        }

        public JsonResult LogoutUser(string CompanyID)
        {
            SessionHelper.UserID = 0;
            SessionHelper.UserName = "";
            SessionHelper.CompanyID = 0;
            SessionHelper.RoomPermissions = null;
            SessionHelper.RoomList = null;
            SessionHelper.RoomID = 0;
            Session.Abandon();
            Session.RemoveAll();
            return Json(new { Message = "success", Status = "ok" }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult UserLoginAuthanticateMasterDB(string Email, string Password, bool IsRemember)
        {
            EnterPriseUserMasterDAL objEnterPriseUserMasterDAL = new EnterPriseUserMasterDAL(SessionHelper.EnterPriseDBName);
            eTurns.DAL.UserMasterDAL objUserMasterDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
            UserMasterDTO objDTO = objEnterPriseUserMasterDAL.CheckAuthantication(Email, Password);
            if (objDTO != null)
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
                remUserName.Expires = DateTime.Now.AddDays(15);
                remPassword.Expires = DateTime.Now.AddDays(15);
                Response.Cookies.Add(remUserName);
                Response.Cookies.Add(remPassword);
                /*END - SET COOKIE */

                SessionHelper.EnterPriseDBName = objDTO.EnterpriseDbName;
                SessionHelper.UserID = objDTO.ID;
                SessionHelper.RoleID = objDTO.RoleID;
                SessionHelper.UserName = objDTO.UserName;
                SessionHelper.CompanyID = objDTO.CompanyID;
                SessionHelper.EnterPriceID = objDTO.EnterpriseId;
                if (SessionHelper.RoleID != -1)
                {
                    objDTO.PermissionList = objUserMasterDAL.GetUserRoleModuleDetailsRecord(objDTO.ID, objDTO.RoleID, 0);
                    string RoomLists = "";
                    if (objDTO.PermissionList != null && objDTO.PermissionList.Count > 0)
                    {
                        objDTO.UserWiseAllRoomsAccessDetails = objUserMasterDAL.ConvertUserPermissions(objDTO.PermissionList, objDTO.RoleID, ref RoomLists);
                        objDTO.SelectedRoomAccessValue = RoomLists;
                    }
                    objDTO.ReplenishingRooms = objUserMasterDAL.GetUserRoomReplanishmentDetailsRecord(objDTO.ID);
                    SessionHelper.RoomPermissions = objDTO.UserWiseAllRoomsAccessDetails;

                    List<KeyValuePair<long, string>> TempRoomList = null;

                    if (!string.IsNullOrEmpty(objDTO.SelectedRoomAccessValue))
                    {
                        TempRoomList = new List<KeyValuePair<long, string>>();

                        string[] RoomAccessIDs = objDTO.SelectedRoomAccessValue.Split(',');
                        for (int i = 0; i < RoomAccessIDs.Length; i++)
                        {
                            string[] RoomIDs = RoomAccessIDs[i].Split('_');
                            if (RoomIDs.Length > 1)
                            {
                                TempRoomList.Add(new KeyValuePair<long, string>(int.Parse(RoomIDs[0]), RoomIDs[1]));
                            }
                        }
                        SessionHelper.RoomList = TempRoomList;
                        if (SessionHelper.RoomList != null)
                        {
                            SessionHelper.RoomID = (int)SessionHelper.RoomList[0].Key;
                            SessionHelper.RoomName = SessionHelper.RoomList[0].Value;
                        }
                    }
                }

                return Json(new { Message = "success", Status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Message = "fail", Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UserLoginAuthanticate(string Email, string Password, bool IsRemember)
        {

            UserMasterController obj = new UserMasterController();
            UserMasterDTO objDTO = obj.CheckAuthantication(Email, Password);
            if (objDTO != null)
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
                remUserName.Expires = DateTime.Now.AddDays(15);
                remPassword.Expires = DateTime.Now.AddDays(15);
                Response.Cookies.Add(remUserName);
                Response.Cookies.Add(remPassword);
                /*END - SET COOKIE */

                SessionHelper.UserID = objDTO.ID;
                SessionHelper.UserName = objDTO.UserName;
                SessionHelper.CompanyID = objDTO.CompanyID;

                SessionHelper.RoomPermissions = objDTO.UserWiseAllRoomsAccessDetails;
                List<KeyValuePair<long, string>> TempRoomList = null;

                if (!string.IsNullOrEmpty(objDTO.SelectedRoomAccessValue))
                {
                    TempRoomList = new List<KeyValuePair<long, string>>();

                    string[] RoomAccessIDs = objDTO.SelectedRoomAccessValue.Split(',');
                    for (int i = 0; i < RoomAccessIDs.Length; i++)
                    {
                        string[] RoomIDs = RoomAccessIDs[i].Split('_');
                        if (RoomIDs.Length > 1)
                        {
                            TempRoomList.Add(new KeyValuePair<long, string>(int.Parse(RoomIDs[0]), RoomIDs[1]));
                        }
                    }
                    SessionHelper.RoomList = TempRoomList;
                    if (SessionHelper.RoomList != null)
                    {
                        SessionHelper.RoomID = (int)SessionHelper.RoomList[0].Key;
                        SessionHelper.RoomName = SessionHelper.RoomList[0].Value;
                    }
                }

                return Json(new { Message = "success", Status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Message = "fail", Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        #region "Role Master"

        public ActionResult RoleList()
        {
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
        public ActionResult RoleCreate()
        {
            RoleMasterDTO objDTO = new RoleMasterDTO();
            objDTO.ID = 0;
            objDTO.Created = DateTime.Now;
            objDTO.Updated = DateTime.Now;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            //objDTO.Room = SessionHelper.RoomID;
            //objDTO.CompanyID = SessionHelper.CompanyID;
            //objDTO.RoomName = SessionHelper.RoomName;
            objDTO.GUID = Guid.NewGuid();

            eTurnsWeb.Controllers.WebAPI.EturnsMaster.RoleMasterController obj = new eTurnsWeb.Controllers.WebAPI.EturnsMaster.RoleMasterController();
            var objList = obj.GetRoleModuleDetailsRecord(0);

            var objMasterList = from t in objList
                                where t.GroupId.ToString() == "1" && t.IsModule == true
                                select t;
            var objOtherModuleList = from t in objList
                                     where t.GroupId.ToString() == "2" && t.IsModule == true
                                     select t;

            var objNonModuleList = from t in objList
                                   where t.IsModule == false && t.GroupId.ToString() != "4"
                                   select t;

            var objOtherDefaultSettings = from t in objList
                                          where t.GroupId.ToString() == "4" && t.IsModule == false
                                          select t;
            //objDTO.ModuleMasterList = objMasterList.ToList();
            //objDTO.OtherModuleList = objOtherModuleList.ToList();
            //objDTO.NonModuleList = objNonModuleList.ToList();

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
            //objRoomAccessDTO.RoomID = 0;

            objDTO.RoleWiseRoomsAccessDetails = null;
            objDTO.RoleRoomsAccessDetail = objRoomAccessDTO;

            //List<RoomDTO> objRoomList = new List<RoomDTO>();
            //RoomDTO odjRoomDTO = new RoomDTO();
            //odjRoomDTO.ID = 0;
            //odjRoomDTO.RoomName = "";
            //objRoomList.Insert(0, odjRoomDTO);
            //ViewBag.RoomsList = objRoomList.OrderBy(e => e.ID);

            Session["SelectedRoomsPermission"] = null;

            return PartialView("_CreateRole", objDTO);
        }

        [HttpPost]
        public JsonResult RoleSave(RoleMasterDTO objDTO)
        {
            string message = "";
            string status = "";
            eTurnsWeb.Controllers.WebAPI.EturnsMaster.RoleMasterController obj = new eTurnsWeb.Controllers.WebAPI.EturnsMaster.RoleMasterController();

            if (string.IsNullOrEmpty(objDTO.RoleName))
            {
                message = string.Format(ResMessage.Required, ResRoleMaster.RoleName);// "Role name is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;
            //objDTO.Room = SessionHelper.RoomID;

            //Set Room Replenishment

            //if (!string.IsNullOrEmpty(objDTO.SelectedRoomReplanishmentValue))
            //{
            //    List<RoleRoomReplanishmentDetailsDTO> objRoomRepIDs = new List<RoleRoomReplanishmentDetailsDTO>();

            //    string[] objRoomReplanishmentIDs = objDTO.SelectedRoomReplanishmentValue.Split(',');
            //    for (int i = 0; i < objRoomReplanishmentIDs.Length; i++)
            //    {
            //        RoleRoomReplanishmentDetailsDTO objRoleRoom = new RoleRoomReplanishmentDetailsDTO();
            //        objRoleRoom.RoomID = int.Parse(objRoomReplanishmentIDs[i].ToString());
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
            //else
            //{

            //    message = string.Format(ResMessage.Required, "Room Access"); //"Room Access is required.";
            //    status = "fail";
            //    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            //}

            if (objDTO.ID == 0)
            {
                objDTO.CreatedBy = SessionHelper.UserID;

                string strOK = obj.DuplicateCheck(objDTO.RoleName, "add", objDTO.ID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResRoleMaster.RoleName, objDTO.RoleName);  //"Role Name \"" + objDTO.RoleName + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    objDTO.GUID = Guid.NewGuid();
                    HttpResponseMessage hrmResult = obj.PostRecord(objDTO);
                    if (hrmResult.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, hrmResult.StatusCode); // "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }
            else
            {
                objDTO.LastUpdatedBy = SessionHelper.UserID;
                string strOK = obj.DuplicateCheck(objDTO.RoleName, "edit", objDTO.ID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResRoleMaster.RoleName, objDTO.RoleName);  //"Role Name \"" + objDTO.RoleName + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    HttpResponseMessage hrmResult = obj.PutRecord(objDTO.ID, objDTO);

                    if (hrmResult.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                        Session["SelectedRoomsPermission"] = null;
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, hrmResult.StatusCode); // "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }

            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>
        public ActionResult RoleEdit(Int64 ID)
        {
            eTurnsWeb.Controllers.WebAPI.EturnsMaster.RoleMasterController obj = new eTurnsWeb.Controllers.WebAPI.EturnsMaster.RoleMasterController();
            RoleMasterDTO objDTO = obj.GetRecord(ID);

            var objMasterList = from t in objDTO.PermissionList
                                where t.GroupId.ToString() == "1" && t.IsModule == true
                                select t;
            var objOtherModuleList = from t in objDTO.PermissionList
                                     where t.GroupId.ToString() == "2" && t.IsModule == true
                                     select t;

            var objNonModuleList = from t in objDTO.PermissionList
                                   where t.IsModule == false && t.GroupId.ToString() != "4"
                                   select t;

            var objOtherDefaultSettings = from t in objDTO.PermissionList
                                          where t.GroupId.ToString() == "4" && t.IsModule == false
                                          select t;

            RoleWiseRoomsAccessDetailsDTO objRoomAccessDTO = new RoleWiseRoomsAccessDetailsDTO();
            objRoomAccessDTO.PermissionList = objDTO.PermissionList.ToList();
            objRoomAccessDTO.ModuleMasterList = objMasterList.ToList();
            objRoomAccessDTO.OtherModuleList = objOtherModuleList.ToList();
            objRoomAccessDTO.NonModuleList = objNonModuleList.ToList();
            objRoomAccessDTO.OtherDefaultSettings = objOtherDefaultSettings.ToList();

            objRoomAccessDTO.RoleID = 0;
            //objRoomAccessDTO.RoomID = 0;

            objDTO.RoleRoomsAccessDetail = objRoomAccessDTO;


            if (objDTO.RoleWiseRoomsAccessDetails != null)
            {
                Session["SelectedRoomsPermission"] = objDTO.RoleWiseRoomsAccessDetails;
            }

            //List<RoomDTO> objRoomList = new List<RoomDTO>();

            //RoomDTO odjRoomDTO = new RoomDTO();
            //odjRoomDTO.ID = 0;
            //odjRoomDTO.RoomName = "";
            //objRoomList.Insert(0, odjRoomDTO);
            //ViewBag.RoomsList = objRoomList;

            objDTO.SelectedModuleIDs = "";
            objDTO.SelectedNonModuleIDs = "";
            //if (objDTO.ReplenishingRooms != null && objDTO.ReplenishingRooms.Count > 0)
            //{
            //    foreach (RoleRoomReplanishmentDetailsDTO item in objDTO.ReplenishingRooms)
            //    {
            //        if (string.IsNullOrEmpty(objDTO.SelectedRoomReplanishmentValue))
            //            objDTO.SelectedRoomReplanishmentValue = item.RoomID.ToString();
            //        else
            //            objDTO.SelectedRoomReplanishmentValue += ',' + item.RoomID.ToString();
            //    }
            //}
            //objDTO.SelectedRoomReplanishmentValue
            return PartialView("_CreateRole", objDTO);

        }

        #region Ajax Data Provider

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult RoleListAjax(JQueryDataTableParamModel param)
        {
            eTurnsWeb.Controllers.WebAPI.EturnsMaster.RoleMasterController obj = new eTurnsWeb.Controllers.WebAPI.EturnsMaster.RoleMasterController();

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
            var DataFromDB = obj.GetAllRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName);

            var result = from c in DataFromDB
                         select new RoleMasterDTO
                         {
                             ID = c.ID,
                             RoleName = c.RoleName,
                             Created = c.Created,
                             Updated = c.Updated,
                             IsDeleted = c.IsDeleted,
                             IsArchived = c.IsArchived,
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
        public string DeleteRoleRecords(string ids)
        {
            try
            {
                //RoleMasterController obj = new RoleMasterController();
                //obj.DeleteRecords(ids, 1);
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region "Role permission details"
        public PartialViewResult _CreateRolePermission()
        {
            return PartialView();
        }

        public JsonResult SaveToRolePermissionsToSession(string RoleID, string SelectedModuleList, string SelectedNonModuleList, string SelectedDefaultSettings)
        {
            if (!string.IsNullOrEmpty(RoleID))
            {
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

                            objRDTO = objPermissionList.Find(element => element.ModuleID == int.Parse(name[0]));
                            if (objRDTO != null)
                            {
                                //if (name[1] == "view")
                                //    objRDTO.IsView = true;

                                //if (name[1] == "insert")
                                //    objRDTO.IsInsert = true;

                                //if (name[1] == "update")
                                //    objRDTO.IsUpdate = true;

                                //if (name[1] == "delete")
                                //    objRDTO.IsDelete = true;

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
                                //objRDTO.RoomId = int.Parse(RoomID);

                                //if (name[1] == "view")
                                //    objRDTO.IsView = true;

                                //if (name[1] == "insert")
                                //    objRDTO.IsInsert = true;

                                //if (name[1] == "update")
                                //    objRDTO.IsUpdate = true;

                                //if (name[1] == "delete")
                                //    objRDTO.IsDelete = true;

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

                            objRDTO = objPermissionList.Find(element => element.ModuleID == int.Parse(name[0]));
                            if (objRDTO != null)
                                objRDTO.ModuleValue = name[1];
                            else
                            {
                                objRDTO = new RoleModuleDetailsDTO();
                                objRDTO.ModuleID = int.Parse(name[0]);
                               // objRDTO.RoomId = int.Parse(RoomID);
                                objRDTO.ModuleValue = name[1];
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
                    //objRoomAccessDTO.RoomID = int.Parse(RoomID);

                    if (Session["SelectedRoomsPermission"] != null)
                    {
                        //objRoomsAccessList = (List<RoleWiseRoomsAccessDetailsDTO>)Session["SelectedRoomsPermission"];
                        //RoleWiseRoomsAccessDetailsDTO exsisting = objRoomsAccessList.Find(element => element.RoomID == int.Parse(RoomID));
                        //if (exsisting != null)
                        //    exsisting.PermissionList = objPermissionList.ToList();
                        //else
                        objRoomsAccessList = new List<RoleWiseRoomsAccessDetailsDTO>();
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

        public RoleWiseRoomsAccessDetailsDTO GetRolePermissionsFromSession(string RoleID)
        {
            List<RoleWiseRoomsAccessDetailsDTO> objRoomsAccessList = new List<RoleWiseRoomsAccessDetailsDTO>();

            if (!string.IsNullOrEmpty(RoleID) && Session["SelectedRoomsPermission"] != null)
            {
                objRoomsAccessList = (List<RoleWiseRoomsAccessDetailsDTO>)Session["SelectedRoomsPermission"];
                if (objRoomsAccessList != null && objRoomsAccessList.Count > 0)
                    return objRoomsAccessList.FirstOrDefault();
            }
            return null;
        }

        public void CopyPermissionsToRooms(string ParentRoomID, string CopyToRoomIDs, string RoleID)
        {
            if (!string.IsNullOrEmpty(ParentRoomID) && !string.IsNullOrEmpty(CopyToRoomIDs) && Session["SelectedRoomsPermission"] != null)
            {
                List<RoleWiseRoomsAccessDetailsDTO> objRoomsAccessList;
                if (Session["SelectedRoomsPermission"] != null)
                {
                    objRoomsAccessList = (List<RoleWiseRoomsAccessDetailsDTO>)Session["SelectedRoomsPermission"];
                    RoleWiseRoomsAccessDetailsDTO ParentRoomData = objRoomsAccessList.Find(element => element.RoomID == int.Parse(ParentRoomID));
                    if (ParentRoomData != null)
                    {
                        string[] ids = CopyToRoomIDs.Split(',');
                        for (int i = 0; i < ids.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(ids[i]) && int.Parse(ids[i]) > 0)
                            {
                                RoleWiseRoomsAccessDetailsDTO ChildRoomData = objRoomsAccessList.Find(element => element.RoomID == int.Parse(ids[i]));
                                if (ChildRoomData != null)
                                {
                                    ChildRoomData.PermissionList = ParentRoomData.PermissionList;
                                }
                                else
                                {
                                    RoleWiseRoomsAccessDetailsDTO objRoomAccessDTO = new RoleWiseRoomsAccessDetailsDTO();
                                    objRoomAccessDTO.PermissionList = ParentRoomData.PermissionList;
                                    objRoomAccessDTO.RoleID = int.Parse(RoleID);
                                    objRoomAccessDTO.RoomID = int.Parse(ids[i]);
                                    objRoomsAccessList.Add(objRoomAccessDTO);
                                }
                            }
                        }
                    }
                    Session["SelectedRoomsPermission"] = objRoomsAccessList;
                }
            }
        }


        public void AddRemoveRoomsToSession(string RoomIDs)
        {
            if (!string.IsNullOrEmpty(RoomIDs) && Session["SelectedRoomsPermission"] != null)
            {
                List<RoleWiseRoomsAccessDetailsDTO> objRoomsAccessList;
                List<RoleWiseRoomsAccessDetailsDTO> objTempRoomsAccessList = new List<RoleWiseRoomsAccessDetailsDTO>();
                objRoomsAccessList = (List<RoleWiseRoomsAccessDetailsDTO>)Session["SelectedRoomsPermission"];
                objTempRoomsAccessList = new List<RoleWiseRoomsAccessDetailsDTO>();

                string[] ids = RoomIDs.Split(',');
                for (int i = 0; i < ids.Length; i++)
                {
                    RoleWiseRoomsAccessDetailsDTO RoomData = objRoomsAccessList.Find(element => element.RoomID == int.Parse(ids[i]));
                    if (RoomData != null)
                        objTempRoomsAccessList.Add(RoomData);

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
        public ActionResult RolePermissionCreate( string RoleID)
        {
            RoleWiseRoomsAccessDetailsDTO objRoomAccessDTO = new RoleWiseRoomsAccessDetailsDTO();
            objRoomAccessDTO = GetRolePermissionsFromSession(RoleID);

            eTurnsWeb.Controllers.WebAPI.EturnsMaster.RoleMasterController obj = new eTurnsWeb.Controllers.WebAPI.EturnsMaster.RoleMasterController();

            var objList = obj.GetRoleModuleDetailsRecord(0);
            if (objRoomAccessDTO != null)
            {
                RoleModuleDetailsDTO objRoleModuleDetailsDTO;
                foreach (RoleModuleDetailsDTO item in objRoomAccessDTO.PermissionList)
                {
                    objRoleModuleDetailsDTO = objList.ToList().Find(element => element.ModuleID == item.ModuleID);
                    if (objRoleModuleDetailsDTO != null)
                    {
                        objRoleModuleDetailsDTO.IsChecked = item.IsChecked;
                        //objRoleModuleDetailsDTO.IsInsert = item.IsInsert;
                        //objRoleModuleDetailsDTO.IsDelete = item.IsDelete;
                        //objRoleModuleDetailsDTO.IsUpdate = item.IsUpdate;
                        //objRoleModuleDetailsDTO.IsView = item.IsView;

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
                //int SelectedRoomID = 0;
                //SelectedRoomID = int.Parse(RoomID);
                objRoomAccessDTO.PermissionList = objList.ToList();
                objRoomAccessDTO.RoleID = int.Parse(RoleID);
                //objRoomAccessDTO.RoomID = SelectedRoomID;
            }

            var objMasterList = from t in objList
                                where t.GroupId.ToString() == "1" && t.IsModule == true
                                select t;
            var objOtherModuleList = from t in objList
                                     where t.GroupId.ToString() == "2" && t.IsModule == true
                                     select t;

            var objNonModuleList = from t in objList
                                   where t.IsModule == false && t.GroupId.ToString() != "4"
                                   select t;

            var objOtherDefaultSettings = from t in objList
                                          where t.GroupId.ToString() == "4" && t.IsModule == false
                                          select t;


            objRoomAccessDTO.ModuleMasterList = objMasterList.ToList();
            objRoomAccessDTO.OtherModuleList = objOtherModuleList.ToList();
            objRoomAccessDTO.NonModuleList = objNonModuleList.ToList();
            objRoomAccessDTO.OtherDefaultSettings = objOtherDefaultSettings.ToList();

            //List<RoomDTO> objRoomList = new List<RoomDTO>();
            //RoomDTO odjRoomDTO = new RoomDTO();
            //odjRoomDTO.ID = 0;
            //odjRoomDTO.RoomName = "";
            //objRoomList.Insert(0, odjRoomDTO);
            //ViewBag.RoomsList = objRoomList;

            return PartialView("_CreateRolePermission", objRoomAccessDTO);
        }

        #endregion


        #endregion

        #region "User Master"
        public ActionResult UserList()
        {
            return View();
        }
        public PartialViewResult _CreateUser()
        {
            return PartialView();
        }
        public ActionResult UserCreate()
        {
            UserMasterDTO objDTO = new UserMasterDTO();
            objDTO.ID = 0;
            objDTO.Created = DateTime.Now;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.Updated = DateTime.Now;
            objDTO.LastUpdatedBy = SessionHelper.UserID;

            objDTO.CreatedByName = SessionHelper.UserName;
            objDTO.UpdatedByName = SessionHelper.UserName;

            objDTO.Room = 0;//SessionHelper.RoomID;
            objDTO.CompanyID = 0;// SessionHelper.CompanyID;
            objDTO.RoomName = "";// SessionHelper.RoomName;
            objDTO.GUID = Guid.NewGuid();

            eTurnsWeb.Controllers.WebAPI.EturnsMaster.UserMasterController obj = new eTurnsWeb.Controllers.WebAPI.EturnsMaster.UserMasterController();
            var objList = obj.GetUserRoleModuleDetailsRecord(0, 0,1);

            var objMasterList = from t in objList
                                where t.GroupId.ToString() == "1" && t.IsModule == true
                                select t;
            var objOtherModuleList = from t in objList
                                     where t.GroupId.ToString() == "2" && t.IsModule == true
                                     select t;

            var objNonModuleList = from t in objList
                                   where t.IsModule == false && t.GroupId.ToString() != "4"
                                   select t;

            var objOtherDefaultSettings = from t in objList
                                          where t.GroupId.ToString() == "4" && t.IsModule == false
                                          select t;
            //objDTO.ModuleMasterList = objMasterList.ToList();
            //objDTO.OtherModuleList = objOtherModuleList.ToList();
            //objDTO.NonModuleList = objNonModuleList.ToList();

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

            //List<RoomDTO> objRoomList = new List<RoomDTO>();
            //RoomDTO odjRoomDTO = new RoomDTO();
            //odjRoomDTO.ID = 0;
            //odjRoomDTO.RoomName = "";
            //objRoomList.Insert(0, odjRoomDTO);
            //ViewBag.RoomsList = objRoomList.OrderBy(e => e.ID);

            Session["SelectedUserRoomsPermission"] = null;

            return PartialView("_CreateUser", objDTO);
        }
        [HttpPost]
        public JsonResult UserSave(UserMasterDTO objDTO)
        {
            string message = "";
            string status = "";
            eTurnsWeb.Controllers.WebAPI.EturnsMaster.UserMasterController obj = new eTurnsWeb.Controllers.WebAPI.EturnsMaster.UserMasterController();

            if (string.IsNullOrEmpty(objDTO.UserName))
            {
                message = "User name is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.Room = 0;// SessionHelper.RoomID;

            //Set Room Replenishment

            //if (!string.IsNullOrEmpty(objDTO.SelectedRoomReplanishmentValue))
            //{
            //    List<UserRoomReplanishmentDetailsDTO> objRoomRepIDs = new List<UserRoomReplanishmentDetailsDTO>();

            //    string[] objRoomReplanishmentIDs = objDTO.SelectedRoomReplanishmentValue.Split(',');
            //    for (int i = 0; i < objRoomReplanishmentIDs.Length; i++)
            //    {
            //        UserRoomReplanishmentDetailsDTO objRoleRoom = new UserRoomReplanishmentDetailsDTO();
            //        objRoleRoom.RoomID = int.Parse(objRoomReplanishmentIDs[i].ToString());
            //        objRoomRepIDs.Add(objRoleRoom);
            //    }
            //    objDTO.ReplenishingRooms = objRoomRepIDs;
            //}
            // Set Permissions

            if (Session["SelectedUserRoomsPermission"] != null)
            {
                //List<UserWiseRoomsAccessDetailsDTO> objRoomsAccessList = (List<UserWiseRoomsAccessDetailsDTO>)Session["SelectedUserRoomsPermission"];
                //List<UserWiseRoomsAccessDetailsDTO> objSelectedRooms = new List<UserWiseRoomsAccessDetailsDTO>();
                //if (objRoomsAccessList != null && objRoomsAccessList.Count > 0)
                //{
                //    if (string.IsNullOrEmpty(objDTO.SelectedRoomAccessValue))
                //    {

                //        objSelectedRooms = objRoomsAccessList;
                //    }
                //    else
                //    {
                //        string[] RoomAccessIDs = objDTO.SelectedRoomAccessValue.Split(',');
                //        for (int i = 0; i < RoomAccessIDs.Length; i++)
                //        {
                //            string[] RoomIDs = RoomAccessIDs[i].Split('_');
                //            if (RoomIDs.Length > 1)
                //            {
                //                UserWiseRoomsAccessDetailsDTO objRoom = objRoomsAccessList.Find(element => element.RoomID == int.Parse(RoomIDs[0]));
                //                if (objRoom != null)
                //                {
                //                    objSelectedRooms.Add(objRoom);
                //                }
                //            }
                //        }
                //    }
                //}
                //objDTO.UserWiseAllRoomsAccessDetails = objSelectedRooms;

                List<UserWiseRoomsAccessDetailsDTO> objRoomsAccessList = (List<UserWiseRoomsAccessDetailsDTO>)Session["SelectedUserRoomsPermission"];
                if (objRoomsAccessList != null && objRoomsAccessList.Count > 0)
                    objDTO.UserWiseAllRoomsAccessDetails = objRoomsAccessList;
            }
            else
            {

                message = "Room Access is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }

            if (objDTO.ID == 0)
            {
                objDTO.CreatedBy = SessionHelper.UserID;

                string strOK = obj.DuplicateCheck(objDTO.Email, "add", "Email", objDTO.ID);
                if (strOK == "duplicate")
                {
                    message = "User Email \"" + objDTO.Email + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    string strOK1 = obj.DuplicateCheck(objDTO.UserName, "add", "UserName", objDTO.ID);
                    if (strOK1 == "duplicate")
                    {
                        message = "User Name \"" + objDTO.UserName + "\" already exist! Try with Another!";
                        status = "duplicate";
                    }
                    else
                    {
                        objDTO.GUID = Guid.NewGuid();
                        HttpResponseMessage hrmResult = obj.PostRecord(objDTO);
                        if (hrmResult.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            message = "Record Saved Sucessfully...";
                            status = "ok";
                        }
                        else
                        {
                            message = "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                            status = "fail";
                        }
                    }
                }
            }
            else
            {
                objDTO.LastUpdatedBy = SessionHelper.UserID;
                string strOK = obj.DuplicateCheck(objDTO.Email, "edit", "Email", objDTO.ID);
                if (strOK == "duplicate")
                {
                    //message = "Role Name \"" + objDTO.UserName + "\" already exist! Try with Another!";
                    message = "User Email \"" + objDTO.Email + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    string strOK1 = obj.DuplicateCheck(objDTO.UserName, "edit", "UserName", objDTO.ID);
                    if (strOK1 == "duplicate")
                    {
                        message = "User Name \"" + objDTO.UserName + "\" already exist! Try with Another!";
                        status = "duplicate";
                    }
                    else
                    {

                        HttpResponseMessage hrmResult = obj.PutRecord(objDTO.ID, objDTO);
                        if (hrmResult.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            message = "Record Saved Sucessfully...";
                            status = "ok";
                            Session["SelectedUserRoomsPermission"] = null;
                        }
                        else
                        {
                            message = "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                            status = "fail";
                        }
                    }
                }
            }

            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult UserEdit(Int64 ID)
        {
            eTurnsWeb.Controllers.WebAPI.EturnsMaster.UserMasterController obj = new eTurnsWeb.Controllers.WebAPI.EturnsMaster.UserMasterController();
            UserMasterDTO objDTO = obj.GetRecord(ID);

            var objMasterList = from t in objDTO.PermissionList
                                where t.GroupId.ToString() == "1" && t.IsModule == true
                                select t;
            var objOtherModuleList = from t in objDTO.PermissionList
                                     where t.GroupId.ToString() == "2" && t.IsModule == true
                                     select t;

            var objNonModuleList = from t in objDTO.PermissionList
                                   where t.IsModule == false && t.GroupId.ToString() != "4"
                                   select t;

            var objOtherDefaultSettings = from t in objDTO.PermissionList
                                          where t.GroupId.ToString() == "4" && t.IsModule == false
                                          select t;

            UserWiseRoomsAccessDetailsDTO objRoomAccessDTO = new UserWiseRoomsAccessDetailsDTO();
            objRoomAccessDTO.PermissionList = objDTO.PermissionList.ToList();
            objRoomAccessDTO.ModuleMasterList = objMasterList.ToList();
            objRoomAccessDTO.OtherModuleList = objOtherModuleList.ToList();
            objRoomAccessDTO.NonModuleList = objNonModuleList.ToList();
            objRoomAccessDTO.OtherDefaultSettings = objOtherDefaultSettings.ToList();

            objRoomAccessDTO.RoleID = objDTO.RoleID;
            objRoomAccessDTO.RoomID = 0;

            objDTO.UserwiseRoomsAccessDetail = objRoomAccessDTO;


            if (objDTO.UserWiseAllRoomsAccessDetails != null)
            {
                Session["SelectedUserRoomsPermission"] = objDTO.UserWiseAllRoomsAccessDetails;
            }

            //List<RoomDTO> objRoomList = new List<RoomDTO>();

            //RoomDTO odjRoomDTO = new RoomDTO();
            //odjRoomDTO.ID = 0;
            //odjRoomDTO.RoomName = "";
            //objRoomList.Insert(0, odjRoomDTO);
            //ViewBag.RoomsList = objRoomList;

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

        #region Ajax Data Provider

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult UserListAjax(JQueryDataTableParamModel param)
        {
            eTurnsWeb.Controllers.WebAPI.EturnsMaster.UserMasterController obj = new eTurnsWeb.Controllers.WebAPI.EturnsMaster.UserMasterController();

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
            var DataFromDB = obj.GetAllRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName);

            var result = from c in DataFromDB
                         select new UserMasterDTO
                         {
                             ID = c.ID,
                             UserName = c.UserName,
                             Password = c.Password,
                             Phone = c.Phone,
                             RoleName = c.RoleName,
                             Email = c.Email,
                             Created = c.Created,
                             Updated = c.Updated,
                             IsDeleted = c.IsDeleted,
                             IsArchived = c.IsArchived,
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
        public string UpdateUserData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            //UserMasterController obj = new UserMasterController();
            //obj.PutUpdateData(id, value, rowId, columnPosition, columnId, columnName);
            //return value;
            return null;
        }
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
                    objRow.ShowDeleted = item.ShowDeleted;
                    objRow.ShowArchived = item.ShowArchived;
                    objRow.ShowUDF = item.ShowUDF;
                    objRow.ModuleID = item.ModuleID;
                    objRow.ModuleName = item.ModuleName;
                    objRow.ModuleValue = item.ModuleValue;
                    objRow.RoleID = item.RoleID;
                    objRow.IsModule = item.IsModule;
                    //objRow.UserID=0;
                    objResult.Add(objRow);
                }

            }
            return objResult;
        }
        public PartialViewResult _CreateUserPermission()
        {
            return PartialView();
        }
        public string SaveToUserPermissionsToSession( string RoleID, string SelectedModuleList, string SelectedNonModuleList, string SelectedDefaultSettings)
        {
            if ( !string.IsNullOrEmpty(RoleID))
            {
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
                                //if (name[1] == "view")
                                //    objRDTO.IsView = true;

                                //if (name[1] == "insert")
                                //    objRDTO.IsInsert = true;

                                //if (name[1] == "update")
                                //    objRDTO.IsUpdate = true;

                                //if (name[1] == "delete")
                                //    objRDTO.IsDelete = true;

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
                                objRDTO.ModuleID = int.Parse(name[0]);
                                //objRDTO.RoomId = int.Parse(RoomID);

                                //if (name[1] == "view")
                                //    objRDTO.IsView = true;

                                //if (name[1] == "insert")
                                //    objRDTO.IsInsert = true;

                                //if (name[1] == "update")
                                //    objRDTO.IsUpdate = true;

                                //if (name[1] == "delete")
                                //    objRDTO.IsDelete = true;

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
                            //objRDTO.RoomId = SessionHelper.RoomID;

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
                            else
                            {
                                objRDTO = new UserRoleModuleDetailsDTO();
                                objRDTO.ModuleID = int.Parse(name[0]);
                                //objRDTO.RoomId = int.Parse(RoomID);
                                objRDTO.ModuleValue = name[1];
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
                    //objRoomAccessDTO.RoomID = int.Parse(RoomID);

                    if (Session["SelectedUserRoomsPermission"] != null)
                    {
                        //objRoomsAccessList = (List<UserWiseRoomsAccessDetailsDTO>)Session["SelectedUserRoomsPermission"];
                        //UserWiseRoomsAccessDetailsDTO exsisting = objRoomsAccessList.Find(element => element.RoomID == int.Parse(RoomID));
                        //if (exsisting != null)
                        //    exsisting.PermissionList = objPermissionList.ToList();
                        //else
                        //    objRoomsAccessList.Add(objRoomAccessDTO);

                        objRoomsAccessList = new List<UserWiseRoomsAccessDetailsDTO>();
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
            return "success";
        }
        public UserWiseRoomsAccessDetailsDTO GetUserPermissionsFromSession(string RoleID)
        {
            List<UserWiseRoomsAccessDetailsDTO> objRoomsAccessList = new List<UserWiseRoomsAccessDetailsDTO>();

            if (!string.IsNullOrEmpty(RoleID) && Session["SelectedUserRoomsPermission"] != null)
            {
                objRoomsAccessList = (List<UserWiseRoomsAccessDetailsDTO>)Session["SelectedUserRoomsPermission"];
                if (objRoomsAccessList != null && objRoomsAccessList.Count > 0)
                    return objRoomsAccessList.FirstOrDefault();//.Find(element => element.RoomID == int.Parse(RoomID));
            }
            return null;
        }
        public ActionResult UserRolePermissionCreate(string RoleID, string UserID)
        {
            UserWiseRoomsAccessDetailsDTO objRoomAccessDTO = new UserWiseRoomsAccessDetailsDTO();
            eTurnsWeb.Controllers.WebAPI.EturnsMaster.RoleMasterController objRole = new eTurnsWeb.Controllers.WebAPI.EturnsMaster.RoleMasterController();
            objRoomAccessDTO = GetUserPermissionsFromSession(RoleID);

            UserMasterController obj = new UserMasterController();
            List<UserRoleModuleDetailsDTO> objList = new List<UserRoleModuleDetailsDTO>();
            List<RoleModuleDetailsDTO> objRoomsList = null;
            if (int.Parse(UserID) > 0)
            {
                objRoomsList = objRole.GetRoleModuleDetailsRecord(0).ToList();
            }
            else
            {
                objRoomsList = objRole.GetRoleModuleDetailsRecord(int.Parse(RoleID)).ToList();
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
                        //objRoleModuleDetailsDTO.IsInsert = item.IsInsert;
                        //objRoleModuleDetailsDTO.IsDelete = item.IsDelete;
                        //objRoleModuleDetailsDTO.IsUpdate = item.IsUpdate;
                        //objRoleModuleDetailsDTO.IsView = item.IsView;
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
                //int SelectedRoomID = 0;
                //SelectedRoomID = int.Parse(RoomID);
                objRoomAccessDTO.PermissionList = objList.ToList();
                objRoomAccessDTO.RoleID = int.Parse(RoleID);
                //objRoomAccessDTO.RoomID = SelectedRoomID;
            }

            var objMasterList = from t in objList
                                where t.GroupId.ToString() == "1" && t.IsModule == true
                                select t;
            var objOtherModuleList = from t in objList
                                     where t.GroupId.ToString() == "2" && t.IsModule == true
                                     select t;

            var objNonModuleList = from t in objList
                                   where t.IsModule == false && t.GroupId.ToString() != "4"
                                   select t;

            var objOtherDefaultSettings = from t in objList
                                          where t.GroupId.ToString() == "4" && t.IsModule == false
                                          select t;


            objRoomAccessDTO.ModuleMasterList = objMasterList.ToList();
            objRoomAccessDTO.OtherModuleList = objOtherModuleList.ToList();
            objRoomAccessDTO.NonModuleList = objNonModuleList.ToList();
            objRoomAccessDTO.OtherDefaultSettings = objOtherDefaultSettings.ToList();

            //List<RoomDTO> objRoomList = new List<RoomDTO>();
            //RoomDTO odjRoomDTO = new RoomDTO();
            //odjRoomDTO.ID = 0;
            //odjRoomDTO.RoomName = "";
            //objRoomList.Insert(0, odjRoomDTO);
            //ViewBag.RoomsList = objRoomList;

            return PartialView("EturnsMaster/_CreateUserPermission", objRoomAccessDTO);
        }
        public JsonResult GetRolewiseRoomInfo(string RoleID)
        {
            Session["SelectedRolesRoomsPermission"] = null;

            eTurnsWeb.Controllers.WebAPI.EturnsMaster.RoleMasterController obj = new eTurnsWeb.Controllers.WebAPI.EturnsMaster.RoleMasterController();
            RoleMasterDTO objDTO = obj.GetRecord(int.Parse(RoleID));
            List<string> objRoomsList = new List<string>();
            string RoomIDs = "";


            if (objDTO.RoleWiseRoomsAccessDetails != null)
            {
                //Session["SelectedRolesRoomsPermission"] = ConvertRoleWiseRoomsAccessDetailsDTO(objDTO.RoleWiseRoomsAccessDetails);

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
        //private UserWiseRoomsAccessDetailsDTO RoomsDefaultPermission(int RoomID)
        //{
        //    if (RoomID > 0 && Session["SelectedRolesRoomsPermission"] != null)
        //    {
        //        List<UserWiseRoomsAccessDetailsDTO> objRoomsAccessList;
        //        List<UserWiseRoomsAccessDetailsDTO> objTempRoomsAccessList = new List<UserWiseRoomsAccessDetailsDTO>();

        //        objRoomsAccessList = (List<UserWiseRoomsAccessDetailsDTO>)Session["SelectedRolesRoomsPermission"];
        //        objTempRoomsAccessList = new List<UserWiseRoomsAccessDetailsDTO>();
        //        return objRoomsAccessList.Find(element => element.RoomID == RoomID);
        //    }
        //    return null;
        //}
        #endregion
        #endregion

        #region "Dashboard"
        public ActionResult Dashboard()
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

            string dvCategoryCost = string.Empty;
            //Inventry
            //Consume
            //Replenish
            //Receive
            //Kits
            //Configuration

            string LeftDiv = string.Empty;
            string RightDiv = string.Empty;

            if (eTurnsWeb.Helper.SessionHelper.RoomPermissions != null)
            {

                eTurns.DTO.UserWiseRoomsAccessDetailsDTO lstPermission = eTurnsWeb.Helper.SessionHelper.RoomPermissions.Find(element => element.RoomID == eTurnsWeb.Helper.SessionHelper.RoomID);

                eTurnsWeb.Controllers.WebAPI.EturnsMaster.ModuleMasterController obj = new eTurnsWeb.Controllers.WebAPI.EturnsMaster.ModuleMasterController();
                List<ParentModuleMasterDTO> lstmasterModule = obj.GetParentRecord().ToList();

                StringBuilder str = new StringBuilder();
                foreach (ParentModuleMasterDTO item in lstmasterModule)
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
                                string ModuleName = SessionHelper.GetModuleName(Convert.ToInt32(Childitem.ModuleID));
                                string TotalNotification = "0";// obj.GetModuleNotification(Convert.ToInt32(Childitem.ModuleID), SessionHelper.RoomID, SessionHelper.UserID);
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
                                           "</div><div class=\"portlet-content\" ><div id=\"dvAssets\">" + str.ToString() + "</div></div></div>";
                            }
                            if (Convert.ToInt64(SessionHelper.ParentModuleList.Reports) == item.ID)// Bind Reports Module
                            {
                                strWidgetTitle = ResDashboard.Reports;
                                dvReports = "<div id=\"Reports\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                                           "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                                           "</div><div class=\"portlet-content\" ><div id=\"dvReports\">" + str.ToString() + "</div></div></div>";
                            }
                            if (Convert.ToInt64(SessionHelper.ParentModuleList.Inventry) == item.ID)// Bind Reports Module
                            {
                                strWidgetTitle = ResDashboard.Inventry;
                                dvInventry = "<div id=\"Inventry\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                                           "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                                           "</div><div class=\"portlet-content\" ><div id=\"dvInventry\">" + str.ToString() + "</div></div></div>";
                            }
                            if (Convert.ToInt64(SessionHelper.ParentModuleList.Consume) == item.ID)// Bind Reports Module
                            {
                                strWidgetTitle = ResDashboard.Consume;
                                dvConsume = "<div id=\"Consume\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                                           "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                                           "</div><div class=\"portlet-content\" ><div id=\"dvConsume\">" + str.ToString() + "</div></div></div>";
                            }
                            if (Convert.ToInt64(SessionHelper.ParentModuleList.Replenish) == item.ID)// Bind Reports Module
                            {
                                strWidgetTitle = ResDashboard.Replenish;
                                dvReplenish = "<div id=\"Replenish\" class=\"portlet ui-widget-content ui-helper-clearfix ui-corner-all\">" +
                                           "<div class=\"portlet-header ui-widget-header ui-corner-top\" ><span class=\"forcolor\">" + strWidgetTitle + "</span><a href=\"#\" class=\"closeEl ui-icon ui-icon-triangle-1-n\"></a>" +
                                           "</div><div class=\"portlet-content\" ><div id=\"dvReplenish\">" + str.ToString() + "</div></div></div>";
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


            #region[Set Wiget in Order]

            StringBuilder strdivs1 = new StringBuilder();
            StringBuilder strdivs2 = new StringBuilder();
            eTurnsWeb.Controllers.WebAPI.EturnsMaster.DashboardWidgetController objwidorder = new eTurnsWeb.Controllers.WebAPI.EturnsMaster.DashboardWidgetController();
            DashboardWidgeDTO objdashboardorder = objwidorder.GetUserWidget(Convert.ToInt32(SessionHelper.UserID));

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
                        else if (SessionHelper.ParentModuleList.Inventry.ToString() == widgetname)
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
                        else if (SessionHelper.ParentModuleList.CategoryCost.ToString() == widgetname)
                        {
                            if (k == 0)
                                strdivs1.Append(dvCategoryCost);
                            else
                                strdivs2.Append(dvCategoryCost);
                        }
                    }
                }
                LeftDiv = strdivs1.ToString();
                RightDiv = strdivs2.ToString();
            }
            else
            {
                LeftDiv = dvCategoryCost + dvMaster + dvKits + dvConsume + dvReports;
                RightDiv = dvAuthentication + dvInventry + dvAssets + dvReceive + dvReplenish + dvConfiguration;
            }
            #endregion

            //LeftDiv = dvAuthentication + dvMaster;
            //RightDiv = dvAssets + dvReports;

            return Json(new { LeftWidget = LeftDiv, RightWidget = RightDiv, Status = "ok" }, JsonRequestBehavior.AllowGet);

        }

        public void SaveWidgetOrder()
        {

            if (!string.IsNullOrEmpty(Convert.ToString(Request.QueryString["saveorder"])))
            {
                if (Convert.ToInt32((Request.QueryString["saveorder"])) == 1)
                {
                    string Worderleft = Convert.ToString(Request.QueryString["sort1[]"]);
                    string Worderright = Convert.ToString(Request.QueryString["sort2[]"]);
                    string WidgetOrder = Worderleft + ":" + Worderright;
                    eTurnsWeb.Controllers.WebAPI.EturnsMaster.DashboardWidgetController obj = new eTurnsWeb.Controllers.WebAPI.EturnsMaster.DashboardWidgetController();
                    obj.SaveUserWidget(Convert.ToInt32(SessionHelper.UserID), WidgetOrder);
                }
            }
        }
        #endregion

        public JsonResult GetRoleList()
        {
            eTurnsWeb.Controllers.WebAPI.EturnsMaster.RoleMasterController obj = new eTurnsWeb.Controllers.WebAPI.EturnsMaster.RoleMasterController();
            //List<RoleMasterDTO> data = obj.GetAllRecord(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(e => e.ID).ToList();
            List<RoleMasterDTO> data = obj.GetAllRecord().OrderBy(e => e.RoleName).ToList();
            RoleMasterDTO objrole = new RoleMasterDTO();
            objrole.ID = 0;
            objrole.RoleName = "Select Role";
            data.Insert(0, objrole);

            return Json(new { DDData = data }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEnterpriceList()
        {
            EnterpriseController obj = new EnterpriseController();
            var data = obj.GetAllRecord();

            return Json(new { DDData = data }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCompanyListFromEnterPrise(string EnterpriseID = "")
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            if (EnterpriseID != "")
            {
                Int64[] intCompanyID = s.Deserialize<Int64[]>(EnterpriseID);
                EnterpriseController obj = new EnterpriseController();
                var data = obj.GetAllRecord();

                
                foreach (Int64 item in intCompanyID)
                {
                    EnterpriseDTO objEnter;// = new EnterpriseDTO();
                    objEnter = data.Where(c => c.ID == item).SingleOrDefault();
                    if (objEnter != null)
                    {
                        string DBName = "Eturns";
                    }
                    //data = obj.GetAllRecords(item).OrderBy(e => e.RoomName).ToList();
                    //foreach (RoomDTO iteminner in data)
                    //{
                    //    finaldata.Add(iteminner);

                    //}

                }
            }

            return Json(new { DDData = "" }, JsonRequestBehavior.AllowGet);
        }
    }
}
