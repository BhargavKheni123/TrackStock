using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsMaster.DAL;
using eTurnsWeb.Helper;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Svg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Enums = eTurns.DAL.Enums;

namespace eTurnsWeb.BAL
{
    public class RoleMasterBAL : IDisposable
    {

        string[] sepForRoleRoom = new string[] { "[!,!]" };
        string[] sepForRoleRoom2 = new string[] { "[!_!]" };

        #region RoleMasterSave
        public void RoleMasterSave(RoleMasterDTO objDTO, HttpSessionStateBase Session, out string message, out string status)
        {
            message = "";
            status = "";

            if (objDTO.UserType < SessionHelper.UserType)
            {
                message = ResRoleMaster.MsgRoleNotSavedForHigherUser;
                status = "fail";
                return; //Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }

            List<UserAccessDTO> objAccessList = new List<UserAccessDTO>();
            UserAccessDTO objUserAccessDTO = null;
            long enterId = 0, CompId = 0, RmId = 0;
            long RoleID = objDTO.ID;
            //RoleID = Convert.ToString(objDTO.ID);
            //string RoomID = string.Empty;
            //string EnterpriseID = string.Empty;
            //string CompanyID = string.Empty;
            if (objDTO.IsECRAccessUpdated)
            {
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
            }
            objDTO.lstAccess = objAccessList;
            //string message = "";
            //string status = "";



            if (string.IsNullOrEmpty(objDTO.RoleName))
            {
                message = string.Format(ResMessage.Required, ResRoleMaster.RoleName);// "Role name is required.";
                status = "fail";
                return; //Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
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
                status = "fail-roleaccess";
                return; //Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }


            if (objDTO.ID == 0)
            {
                this.AddRole(objDTO, Session, out message, out status);
            }
            else
            {
                this.EditRole(objDTO, Session, out message, out status);
            }
        }


        private void AddRole(RoleMasterDTO objDTO, HttpSessionStateBase Session, out string message, out string status)
        {
            objDTO.CreatedBy = SessionHelper.UserID;
            long enterpriseId = (objDTO.EnterpriseId > 0 ? objDTO.EnterpriseId : SessionHelper.EnterPriceID);
            if (objDTO.UserType == 1)
            {
                objDTO.EnterpriseId = 0;
                objDTO.CompanyID = 0;
            }
            else
            {
                objDTO.EnterpriseId = enterpriseId;
                objDTO.CompanyID = SessionHelper.CompanyID;
            }
            string strOK = "";
            objDTO.GUID = Guid.NewGuid();
            objDTO.IsActive = true;
            eTurnsMaster.DAL.RoleMasterDAL objRoleMasterDAL = new eTurnsMaster.DAL.RoleMasterDAL();
            long retRoleId = 0;

            bool Ret = objRoleMasterDAL.AddUpdateRoleDetails(objDTO, out strOK, out retRoleId);
            long ReturnVal = retRoleId;
            //CommonMasterDAL objCDAL = new CommonMasterDAL();
            //string strOK = objCDAL.RoleDuplicateCheck(objDTO.ID, objDTO.RoleName, objDTO.UserType, (objDTO.EnterpriseId > 0 ? objDTO.EnterpriseId : SessionHelper.EnterPriceID));

            if (strOK == "duplicate")
            {
                message = string.Format(ResMessage.DuplicateMessage, ResRoleMaster.RoleName, objDTO.RoleName);  //"Role Name \"" + objDTO.RoleName + "\" already exist! Try with Another!";
                status = "duplicate";
            }
            else
            {

                //long ReturnVal = objRoleMasterDAL.Insert(objDTO);
                objDTO.ID = ReturnVal;
                if (objDTO.UserType > (int)eTurns.DTO.Enums.UserType.SuperAdmin)
                {
                    //EnterpriseDTO oEnt = new EnterpriseMasterDAL().GetEnterpriseByID((objDTO.EnterpriseId > 0 ? objDTO.EnterpriseId : SessionHelper.EnterPriceID));
                    string strOKEnt = "";
                    long retRoleIdEnt = 0;
                    using (eTurns.DAL.RoleMasterDAL objRolecMasterDALEnt = new eTurns.DAL.RoleMasterDAL(new EnterpriseMasterDAL().GetEnterpriseDBNameByID(enterpriseId)))
                    {
                        //objRolecMasterDAL.Insert(objDTO);
                        bool ReturnValEnt = objRolecMasterDALEnt.AddUpdateRoleData(objDTO, Enums.DBOperation.INSERT, out strOKEnt, out retRoleIdEnt);
                    }

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

        private void EditRole(RoleMasterDTO objDTO, HttpSessionStateBase Session, out string message, out string status)
        {
            eTurnsMaster.DAL.RoleMasterDAL objRoleMasterDAL = new eTurnsMaster.DAL.RoleMasterDAL();
            long RoleID = objDTO.ID;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.IsActive = true;
            string strOK = "";
            long retRoleId = 0;
            bool ReturnVal = objRoleMasterDAL.AddUpdateRoleDetails(objDTO, out strOK, out retRoleId);

            //CommonMasterDAL objCDAL = new CommonMasterDAL();
            //string strOK = objCDAL.RoleDuplicateCheck(objDTO.ID, objDTO.RoleName, objDTO.UserType, (objDTO.EnterpriseId > 0 ? objDTO.EnterpriseId : SessionHelper.EnterPriceID));
            if (strOK == "duplicate")
            {
                message = string.Format(ResMessage.DuplicateMessage, ResRoleMaster.RoleName, objDTO.RoleName);  //"Role Name \"" + objDTO.RoleName + "\" already exist! Try with Another!";
                status = "duplicate";
            }
            else
            {

                //bool ReturnVal = objRoleMasterDAL.Edit(objDTO);
                if (objDTO.UserType >  (int)eTurns.DTO.Enums.UserType.SuperAdmin)
                {
                    long enterpriseId = (objDTO.EnterpriseId > 0 ? objDTO.EnterpriseId : SessionHelper.EnterPriceID);
                    ////EnterpriseDTO oEnt = new EnterpriseMasterDAL().GetEnterpriseByID((objDTO.EnterpriseId > 0 ? objDTO.EnterpriseId : SessionHelper.EnterPriceID));
                    //string enterpriseDBName = new EnterpriseMasterDAL().GetEnterpriseDBNameByID((objDTO.EnterpriseId > 0 ? objDTO.EnterpriseId : SessionHelper.EnterPriceID));

                    //eTurns.DAL.RoleMasterDAL objRolecMasterDAL = new eTurns.DAL.RoleMasterDAL(enterpriseDBName);
                    //objRolecMasterDAL.Edit(objDTO);
                    //eTurns.DAL.UserMasterDAL objUserMasterDAL = new eTurns.DAL.UserMasterDAL(enterpriseDBName);
                    //objUserMasterDAL.DeleteUserRoomAccess(RoleID);

                    string strOKEnt = "";
                    long retRoleIdEnt = 0;
                    using (eTurns.DAL.RoleMasterDAL objRolecMasterDALEnt = new eTurns.DAL.RoleMasterDAL(new EnterpriseMasterDAL().GetEnterpriseDBNameByID(enterpriseId)))
                    {
                        //objRolecMasterDAL.Insert(objDTO);
                        bool ReturnValEnt = objRolecMasterDALEnt.AddUpdateRoleData(objDTO, Enums.DBOperation.UPDATE, out strOKEnt, out retRoleIdEnt);
                    }
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

        #endregion

        public List<UserRoleDTO> GetRoleUsers(int userType, long roleId, string roomIdCSV)
        {
            List<UserRoleDTO> lstUsers = null;

            string roomIds = "";
            string entIds = "";
            long enterpriseID = 0;
            roomIdCSV = roomIdCSV.TrimEnd(",");

            string[] split = roomIdCSV.Split(',');

            foreach (string s in split)
            {
                string[] roomData = s.Split('_');

                if (roomData.Length >= 2 && !string.IsNullOrWhiteSpace(roomData[2]))
                {
                    entIds += roomData[0] + ",";
                    roomIds += roomData[2] + ",";

                    if (enterpriseID == 0)
                    {
                        // take first enterprise id
                        enterpriseID = Convert.ToInt64(roomData[0]);
                    }
                }
            }


            if (userType == (int)eTurns.DTO.Enums.UserType.SuperAdmin)
            {
                // super admin
                using (eTurnsMaster.DAL.RoleMasterDAL objRolecMasterDAL = new eTurnsMaster.DAL.RoleMasterDAL())
                {
                    lstUsers = objRolecMasterDAL.GetRoleUsers(userType, roleId, roomIds);
                }
            }
            else
            {
                // company or enterprise admin
                //long enterpriseId = SessionHelper.EnterPriceID;

                if (enterpriseID > 0)
                {
                    var entMDB = new EnterpriseMasterDAL();
                    var entDTO = entMDB.GetNonDeletedEnterpriseByIdPlain(enterpriseID);
                    entMDB.Dispose();
                    if (entDTO != null)
                    {
                        string dbConn = entDTO.EnterpriseDBName;

                        using (eTurns.DAL.RoleMasterDAL objRolecMasterDALEnt = new eTurns.DAL.RoleMasterDAL(dbConn))
                        {
                            lstUsers = objRolecMasterDALEnt.GetRoleUsers(userType, roleId, roomIds);
                        }
                    }
                }
            }

            if (lstUsers == null)
            {
                lstUsers = new List<UserRoleDTO>();
            }

            return lstUsers;
        }

        #region GetRoleDetailsInfo
        public RoleDetailsInfo GetRoleDetailsInfo(string RoleID, int? UserType, long? UserId, HttpSessionStateBase Session)
        {

            if (UserType.HasValue)
            {
                if ((UserType ?? 0) ==  (int)eTurns.DTO.Enums.UserType.SuperAdmin )
                {
                    return GetRoleDetailsForSuperAdmin(RoleID, UserType, UserId, Session);
                }
                // else if (UserType == 2 || UserType == 3)
                else if (UserType == (int)eTurns.DTO.Enums.UserType.EnterpriseAdmin || UserType == (int)eTurns.DTO.Enums.UserType.CompanyAdmin)
                {
                    return GetRoleDetailsForEntOrComp(RoleID, UserType, UserId, Session);
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

        private RoleDetailsInfo GetRoleDetailsForSuperAdmin(string RoleID, int? UserType, long? UserId, HttpSessionStateBase Session)
        {
            List<RolePermissionInfo> lstEnterPrises = new List<RolePermissionInfo>();
            List<RolePermissionInfo> lstCompanies = new List<RolePermissionInfo>();
            List<RolePermissionInfo> lstRooms = new List<RolePermissionInfo>();
            List<RolePermissionInfo> lstReplenishRooms = new List<RolePermissionInfo>();
            List<UserWiseRoomsAccessDetailsDTO> objRoomsAccessList = new List<UserWiseRoomsAccessDetailsDTO>();

            eTurnsMaster.DAL.RoleMasterDAL obj = new eTurnsMaster.DAL.RoleMasterDAL();
            //RoleMasterDTO objDTO = obj.GetRecord(int.Parse(RoleID));
            RoleMasterDTO objDTO = obj.GetRoleModuleDetails(int.Parse(RoleID));

            if (objDTO != null)
            {
                objDTO.lstAccess = new EnterpriseMasterDAL().GetRoleAccessWithNamesNew(objDTO.ID);
            }
            if (objDTO != null && objDTO.RoleWiseRoomsAccessDetails != null)
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
                        eTurnsMaster.DAL.UserMasterDAL objUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL();
                        UserMasterDTO objUserDTO = objUserMasterDAL.GetUserByIdPlain(UserId.Value);
                        if (objUserDTO != null)
                        {
                            if (objUserDTO.RoleID != long.Parse(RoleID))
                            {
                                Session["SelectedUserRoomsPermission"] = objRoomsAccessList;
                            }
                            else
                            {
                                var userAllRoomsAccessDetails = objUserMasterDAL.GetUserAllRoomsAccessDetails(objUserDTO.ID, objUserDTO.RoleID, objUserDTO.UserType); //
                                Session["SelectedUserRoomsPermission"] = userAllRoomsAccessDetails;
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
                                  }).OrderBy(o => (o.EnterPriseName ?? string.Empty).Trim()).ToList();

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
                                }).OrderBy(o => (o.CompanyName ?? string.Empty).Trim()).ToList();

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
                            }).OrderBy(o => (o.RoomName ?? string.Empty).Trim()).ToList();
                
                if ((UserId ?? 0) > 0)
                {
                    obj.SetUserRoomAccess(UserId.Value, ref lstEnterPrises, ref lstCompanies, ref lstRooms);
                    //lstEnterPrises.ForEach(t =>
                    //{
                    //    if (t.EnterPriseId > 0)
                    //    {
                    //        t.IsSelected = obj.UserHasEnterpriseAccess(UserId.Value, t.EnterPriseId);
                    //    }

                    //});

                    //lstCompanies.ForEach(t =>
                    //{
                    //    if (t.CompanyId > 0)
                    //    {
                    //        t.IsSelected = obj.UserHasCompanyAccess(UserId.Value, t.EnterPriseId, t.CompanyId);
                    //    }

                    //});

                    //lstRooms.ForEach(t =>
                    //{
                    //    if (t.RoomId > 0 && t.CompanyId > 0)
                    //    {
                    //        t.IsSelected = obj.UserHasRoomAccess(UserId.Value, t.EnterPriseId, t.CompanyId, t.RoomId);
                    //    }
                    //});
                }

            }
            //return Json(new { RoomList = lstRooms, CompanyList = lstCompanies, EnterPriseList = lstEnterPrises, ReplenishList = objDTO.SelectedRoomReplanishmentValue, }, JsonRequestBehavior.AllowGet);
            return new RoleDetailsInfo() { RoomList = lstRooms, CompanyList = lstCompanies, EnterpriceList = lstEnterPrises, SelectedRoomReplanishmentValue = objDTO.SelectedRoomReplanishmentValue };
        }

        /// <summary>
        /// Get RoleDetails For Enterprise Or Company
        /// </summary>
        /// <param name="RoleID"></param>
        /// <param name="UserType"></param>
        /// <param name="UserId"></param>
        /// <param name="Session"></param>
        /// <returns></returns>
        private RoleDetailsInfo GetRoleDetailsForEntOrComp(string RoleID, int? UserType, long? UserId, HttpSessionStateBase Session)
        {
            List<RolePermissionInfo> lstEnterPrises = new List<RolePermissionInfo>();
            List<RolePermissionInfo> lstCompanies = new List<RolePermissionInfo>();
            List<RolePermissionInfo> lstRooms = new List<RolePermissionInfo>();
            List<RolePermissionInfo> lstReplenishRooms = new List<RolePermissionInfo>();
            List<UserWiseRoomsAccessDetailsDTO> objRoomsAccessList = new List<UserWiseRoomsAccessDetailsDTO>();
            UserMasterDTO objUserMasterDTO = null;

            eTurns.DAL.RoleMasterDAL obj;
            eTurns.DAL.UserMasterDAL objUserMasterDAL;
            EnterpriseDTO oEnt = new EnterpriseMasterDAL().GetEnterpriseByUserIdNormal(UserId ?? 0);

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
                objUserMasterDTO = objUserMasterDAL.GetUserByIdPlain(UserId.Value);
            }

            //RoleMasterDTO objDTO = obj.GetRecord(long.Parse(RoleID));
            RoleMasterDTO objDTO = obj.GetRoleModuleDetails(long.Parse(RoleID));

            if (objDTO != null && objDTO.RoleWiseRoomsAccessDetails != null)
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
                        if (objUserMasterDTO != null)
                        {
                            if (objUserMasterDTO.RoleID != long.Parse(RoleID))
                            {
                                Session["SelectedUserRoomsPermission"] = objRoomsAccessList;
                            }
                            else
                            {
                                var userAllRoomsAccessDetails = objUserMasterDAL.GetUserAllRoomsAccessDetails(objUserMasterDTO.ID, objUserMasterDTO.RoleID);
                                Session["SelectedUserRoomsPermission"] = userAllRoomsAccessDetails;
                            }
                        }
                    }
                }

                lstEnterPrises = (from rItm in objDTO.lstAccess.Where(t => t.EnterpriseId > 0)
                                  group rItm by new { rItm.EnterpriseId, rItm.EnterpriseName } into gruopedentrprses
                                  select new RolePermissionInfo
                                  {
                                      EnterPriseId = gruopedentrprses.Key.EnterpriseId,
                                      EnterPriseName = gruopedentrprses.Key.EnterpriseName,
                                      IsSelected = true
                                  }).OrderBy(o => (o.EnterPriseName ?? string.Empty).Trim()).ToList();

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
                                }).OrderBy(o => (o.CompanyName ?? string.Empty).Trim()).ToList();

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
                            }).OrderBy(o => (o.RoomName ?? string.Empty).Trim()).ToList();

                if ((UserId ?? 0) > 0 && objUserMasterDTO != null)
                {

                    //obj.UserHasCompanyAccess(UserId.Value, lstCompanies, long.Parse(RoleID), objUserMasterDTO.RoleID);

                    //lstCompanies.ForEach(t =>
                    //{
                    //    if (t.CompanyId > 0)
                    //    {
                    //        t.IsSelected = obj.UserHasCompanyAccess(UserId.Value, t.CompanyId, long.Parse(RoleID), objUserMasterDTO.RoleID);
                    //    }
                    //});

                    obj.UserHasCompanyRoomAccess(UserId.Value, lstCompanies, lstRooms, long.Parse(RoleID), objUserMasterDTO.RoleID);
                    //lstRooms.ForEach(t =>
                    //{
                    //    if (t.RoomId > 0 && t.CompanyId > 0)
                    //    {
                    //        t.IsSelected = obj.UserHasRoomAccess(UserId.Value, t.CompanyId, t.RoomId, long.Parse(RoleID), objUserMasterDTO.RoleID);
                    //    }
                    //});
                }
            }

            //return Json(new { RoomList = lstRooms, CompanyList = lstCompanies, EnterPriseList = lstEnterPrises, ReplenishList = lstReplenishRooms }, JsonRequestBehavior.AllowGet);
            return new RoleDetailsInfo() { RoomList = lstRooms, CompanyList = lstCompanies, EnterpriceList = lstEnterPrises, ReplenishList = lstReplenishRooms };
        }

        #endregion

        #region static methods
        public static List<UserWiseRoomsAccessDetailsDTO> ConvertRoleWiseRoomsAccessDetailsDTO(List<RoleWiseRoomsAccessDetailsDTO> objData)
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

        public static List<UserRoleModuleDetailsDTO> ConvertRoleModuleDetailsDTO(List<RoleModuleDetailsDTO> objData)
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
                    objRow.ToolTipResourceKey = item.ToolTipResourceKey;
                    objRow.ShowChangeLog = item.ShowChangeLog;
                    //objRow.UserID=0;
                    objResult.Add(objRow);
                }

            }
            return objResult;
        }

        #endregion


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~RoleMasterBAL()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }// class

    public class RoleDetailsInfo
    {
        public List<RolePermissionInfo> RoomList { get; set; }
        public List<RolePermissionInfo> CompanyList { get; set; }
        public List<RolePermissionInfo> EnterpriceList { get; set; }
        public List<RolePermissionInfo> ReplenishList { get; set; }

        public string SelectedRoomReplanishmentValue { get; set; }
    }


}//ns