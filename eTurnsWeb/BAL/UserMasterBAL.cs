using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsMaster.DAL;
using eTurnsWeb.Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Enums = eTurns.DAL.Enums;

namespace eTurnsWeb.BAL
{
    public class UserMasterBAL : IDisposable
    {
        public UserMasterDTO GetEditDTOForSuperAdmin(Int64 ID, int? UserType)
        {
            UserMasterDTO objDTO = null;
            if ((UserType ?? 1) == (int)eTurns.DTO.Enums.UserType.SuperAdmin)
            {
                eTurnsMaster.DAL.UserMasterDAL obj = new eTurnsMaster.DAL.UserMasterDAL();
                objDTO = obj.GetUserByIdFull(ID);

                if (objDTO != null)
                {

                    objDTO.CreatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                    objDTO.UpdatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);

                    if (!string.IsNullOrWhiteSpace(objDTO.FirstName))
                    {
                        objDTO.FullName = objDTO.FirstName;
                    }
                    if (!string.IsNullOrWhiteSpace(objDTO.MiddleName))
                    {
                        if (!string.IsNullOrWhiteSpace(objDTO.FullName))
                        {
                            objDTO.FullName = objDTO.FullName + " " + objDTO.MiddleName;
                        }
                        else
                        {
                            objDTO.FullName = objDTO.MiddleName;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(objDTO.LastName))
                    {
                        if (!string.IsNullOrWhiteSpace(objDTO.FullName))
                        {
                            objDTO.FullName = objDTO.FullName + " " + objDTO.LastName;
                        }
                        else
                        {
                            objDTO.FullName = objDTO.LastName;
                        }
                    }

                }

                UserWiseRoomsAccessDetailsDTO objRoomAccessDTO = new UserWiseRoomsAccessDetailsDTO();

                if (objDTO.PermissionList != null)
                {
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

                    objRoomAccessDTO.PermissionList = objDTO.PermissionList.ToList();
                    objRoomAccessDTO.ModuleMasterList = objMasterList.ToList();
                    objRoomAccessDTO.OtherModuleList = objOtherModuleList.ToList();
                    objRoomAccessDTO.NonModuleList = objNonModuleList.ToList();
                    objRoomAccessDTO.OtherDefaultSettings = objOtherDefaultSettings.ToList();

                }

                objRoomAccessDTO.UserType = UserType ?? 1;
                objRoomAccessDTO.RoleID = objDTO.RoleID;
                objRoomAccessDTO.RoomID = 0;
                objRoomAccessDTO.CompanyId = 0;
                objRoomAccessDTO.EnterpriseId = 0;
                objDTO.UserwiseRoomsAccessDetail = objRoomAccessDTO;


                objDTO.SelectedModuleIDs = "";
                objDTO.SelectedNonModuleIDs = "";

                //
                List<UserAccessDTO> lstUserRoomAccess = obj.GetUserRoomAccessByUserIdPlain(objDTO.ID);
                if (objDTO.UserWiseAllRoomsAccessDetails != null && objDTO.UserWiseAllRoomsAccessDetails.Count > 0)
                {
                    if (lstUserRoomAccess != null && lstUserRoomAccess.Count > 0)
                    {
                        foreach (var item in objDTO.UserWiseAllRoomsAccessDetails)
                        {
                            UserAccessDTO UserRoomAccess = lstUserRoomAccess.Where(x => x.EnterpriseId == item.EnterpriseId && x.CompanyId == item.CompanyId && x.RoomId == item.RoomID).FirstOrDefault();
                            if (UserRoomAccess != null)
                                item.SupplierIDs = UserRoomAccess.SupplierIDs;
                        }
                    }
                }

            }

            return objDTO;
        }

        public UserMasterDTO GetEditDTOForEnterpriseUsers(Int64 ID, int? UserType, long? EnterpriseID)
        {
            UserMasterDTO objDTO = null;

            if ((UserType ?? 1) == (int)eTurns.DTO.Enums.UserType.SuperAdmin)
            {
                return objDTO;
            }

            eTurns.DAL.UserMasterDAL obj;
            EnterpriseDTO oEnt = new EnterpriseMasterDAL().GetEnterpriseByIdPlain(EnterpriseID ?? 0);
            if (oEnt != null)
            {
                obj = new eTurns.DAL.UserMasterDAL(oEnt.EnterpriseDBName);
            }
            else
            {
                obj = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
            }

            objDTO = obj.GetUserByIdFull(ID);
            objDTO.EnterpriseId = EnterpriseID ?? 0;

            UserWiseRoomsAccessDetailsDTO objRoomAccessDTO = new UserWiseRoomsAccessDetailsDTO();
            if (objDTO.PermissionList != null)
            {

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

                objRoomAccessDTO.PermissionList = objDTO.PermissionList.ToList();
                objRoomAccessDTO.ModuleMasterList = objMasterList.ToList();
                objRoomAccessDTO.OtherModuleList = objOtherModuleList.ToList();
                objRoomAccessDTO.NonModuleList = objNonModuleList.ToList();
                objRoomAccessDTO.OtherDefaultSettings = objOtherDefaultSettings.ToList();
            }


            objRoomAccessDTO.RoleID = objDTO.RoleID;
            objRoomAccessDTO.RoomID = 0;
            objRoomAccessDTO.CompanyId = 0;
            objRoomAccessDTO.EnterpriseId = EnterpriseID ?? 0;
            objRoomAccessDTO.UserType = UserType ?? 1;
            objDTO.UserwiseRoomsAccessDetail = objRoomAccessDTO;
            objDTO.SelectedModuleIDs = "";
            objDTO.SelectedNonModuleIDs = "";

            List<UserAccessDTO> lstUserRoomAccess = obj.GetUserRoomAccessesByUserId(objDTO.ID);
            if (objDTO.UserWiseAllRoomsAccessDetails != null && objDTO.UserWiseAllRoomsAccessDetails.Count > 0)
            {
                if (lstUserRoomAccess != null && lstUserRoomAccess.Count > 0)
                {
                    foreach (var item in objDTO.UserWiseAllRoomsAccessDetails)
                    {
                        UserAccessDTO UserRoomAccess = lstUserRoomAccess.Where(x => x.EnterpriseId == item.EnterpriseId && x.CompanyId == item.CompanyId && x.RoomId == item.RoomID).FirstOrDefault();
                        if (UserRoomAccess != null)
                            item.SupplierIDs = UserRoomAccess.SupplierIDs;
                    }
                }
            }

            if (objDTO != null)
            {

                objDTO.CreatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                objDTO.UpdatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);

                if (!string.IsNullOrWhiteSpace(objDTO.FirstName))
                {
                    objDTO.FullName = objDTO.FirstName;
                }
                if (!string.IsNullOrWhiteSpace(objDTO.MiddleName))
                {
                    if (!string.IsNullOrWhiteSpace(objDTO.FullName))
                    {
                        objDTO.FullName = objDTO.FullName + " " + objDTO.MiddleName;
                    }
                    else
                    {
                        objDTO.FullName = objDTO.MiddleName;
                    }
                }
                if (!string.IsNullOrWhiteSpace(objDTO.LastName))
                {
                    if (!string.IsNullOrWhiteSpace(objDTO.FullName))
                    {
                        objDTO.FullName = objDTO.FullName + " " + objDTO.LastName;
                    }
                    else
                    {
                        objDTO.FullName = objDTO.LastName;
                    }
                }
            }


            return objDTO;

        }

        public string GetRedirectUrl(long userId, bool isFromLogin)
        {
            string url = "/";
            UserSettingDAL objUserSettingDAL = new eTurnsMaster.DAL.UserSettingDAL();
            UserSettingDTO objUserSettingDTO = objUserSettingDAL.GetByUserId(userId);

            if (objUserSettingDTO == null || (string.IsNullOrEmpty(objUserSettingDTO.RedirectURL)))
            {
                string CtrlName = Convert.ToString(ConfigurationManager.AppSettings["CtrlName"]);
                string ActName = Convert.ToString(ConfigurationManager.AppSettings["ActName"]);
                //return RedirectToAction(ActName, CtrlName, new { FromLogin = "yes" });
                url = string.Format("/{0}/{1}", CtrlName, ActName);
            }
            else if (objUserSettingDTO != null)
            {
                url = objUserSettingDTO.RedirectURL;
            }

            if (isFromLogin)
            {
                url += "?FromLogin=yes";
            }

            return url;
        }


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
        // ~UserBAL()
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
    }
}