using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsMaster.DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace eTurnsWeb.Helper
{
    public class LoginHelper
    {
        public static string SetFormsAuthenticationCookie(HttpResponseBase Response,
            FormsLoginCookieData cookieData)
        {
            FormsAuthentication.SetAuthCookie(cookieData.UserName, cookieData.IsRememberCookie);
            FormsAuthenticationTicket formsAuthenticationTicket;
            string cookiestr;
            HttpCookie httpCookie;
            var userDataStr = cookieData.Email + "#" + cookieData.CurrentLoggedInUserName + "#" + cookieData.IsRememberCookie + "#" + cookieData.ReturnUrl;

            if(cookieData.IsSSO)
            {
                userDataStr += "#" + cookieData.IsSSO + "#" + cookieData.SSOClient;
            }

            formsAuthenticationTicket = new FormsAuthenticationTicket(1, cookieData.UserName, DateTime.Now, DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes), cookieData.IsRememberCookie, userDataStr);
            cookiestr = FormsAuthentication.Encrypt(formsAuthenticationTicket);
            httpCookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookiestr);
            httpCookie.HttpOnly = true;

            if (cookieData.IsRememberCookie)
            {
                httpCookie.Expires = formsAuthenticationTicket.Expiration.AddMinutes(FormsAuthentication.Timeout.TotalMinutes);
            }
            httpCookie.Path = formsAuthenticationTicket.CookiePath;
            Response.Cookies.Add(httpCookie);
            string redirectUrl = FormsAuthentication.GetRedirectUrl(cookieData.Email, false);

            if (string.IsNullOrWhiteSpace(redirectUrl))
            {
                redirectUrl = cookieData.ReturnUrl;
            }

            return redirectUrl;
        }

        public static UserSettingDTO SetSessionDataAtLogin(HttpRequestBase Request, HttpSessionStateBase Session, HttpContextBase HttpContext,
            UserMasterDTO objDTO)
        {
            bool IsPasswordExpired = false;
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
            SessionHelper.IsNgNLFAllowed = objDTO.IsNgNLFAllowed;
            SessionHelper.HasChangedFirstPassword = objDTO.HasChangedFirstPassword;

            SetSessions(Request, Session, HttpContext ,
                SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.UserType, SessionHelper.RoleID, "onlogin", SessionHelper.EnterPriceName, SessionHelper.CompanyName, SessionHelper.RoomName);

            SessionHelper.AnotherLicenceAccepted = objDTO.IsLicenceAccepted;
            if (objDTO.LastLicenceAccept == null)
            {
                SessionHelper.IsLicenceAccepted = false;
                SessionHelper.AnotherLicenceAccepted = false;
                SessionHelper.NewEulaAccept = false;
            }
            //XElement Settinfile = XElement.Load(Server.MapPath("/SiteSettings.xml"));
            string AcceptLicence = SiteSettingHelper.AcceptLicence; //Settinfile.Element("AcceptLicence").Value;
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

            eTurnsMaster.DAL.UserSettingDAL objUserSettingDAL = new eTurnsMaster.DAL.UserSettingDAL();
            eTurns.DTO.UserSettingDTO objUserSettingDTO = objUserSettingDAL.GetByUserId(objDTO.ID);
            bool isPassWordExpire = false;
            if (objUserSettingDTO != null)
            {
                isPassWordExpire = objUserSettingDTO.IsNeverExpirePwd;
                SessionHelper.ShowDateTime = objUserSettingDTO.ShowDateTime;
                SessionHelper.SearchPattern = objUserSettingDTO.SearchPattern;
            }
            else
            {
                SessionHelper.ShowDateTime = false;
                SessionHelper.SearchPattern = 2;
            }

            if (IsPasswordExpired && (!isPassWordExpire))
            {
                SessionHelper.HasPasswordChanged = false;
            }

            return objUserSettingDTO;
        }

        /// <summary>
        /// Check password expired for session UserID
        /// </summary>
        /// <returns></returns>
        private static bool CheckpasswordExpired()
        {
            if (SessionHelper.UserType == 1)
                return false;
            // bool NeedResetPassword = SessionHelper.GetAdminPermission(SessionHelper.ModuleList.PasswordResetRule);
            //  if (NeedResetPassword)
            {

                eTurnsMaster.DAL.UserMasterDAL objeTurnsMaster = new eTurnsMaster.DAL.UserMasterDAL();
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
        }

        public static void SetSessions(
            HttpRequestBase Request, HttpSessionStateBase Session, HttpContextBase HttpContext,
            long EnterpriseId, long CompanyId, long RoomId, long UserId, int UserType, long RoleId, string EventFiredOn, string enterpriseName, string CompanyName, string RoomName)
        {

            eTurnsMaster.DAL.EnterpriseMasterDAL objEnterpriseDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
            eTurnsMaster.DAL.UserMasterDAL objUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL();
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
                    SessionHelper.SetSessionCompleted = true;

                    switch (UserType)
                    {

                        case 1:
                            if (RoleId == -1)
                            {
                                List<EnterpriseDTO> lstAllEnterPrises = (from em in objEnterpriseDAL.GetAllEnterprisesPlain()
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
                                        SessionHelper.IsABEnterprise = objEnterpriseDTO.AllowABIntegration;
                                        //SessionHelper.EnterPriseDBConnectionString = objEnterpriseDTO.EnterpriseDBConnectionString;
                                    }
                                    else
                                    {
                                        SessionHelper.EnterPriceID = lstAllEnterPrises.First().ID;
                                        SessionHelper.EnterpriseLogoUrl = lstAllEnterPrises.First().EnterpriseLogo;
                                        SessionHelper.EnterPriseDBName = lstAllEnterPrises.First().EnterpriseDBName;
                                        SessionHelper.IsABEnterprise = lstAllEnterPrises.First().AllowABIntegration;
                                        //SessionHelper.EnterPriseDBConnectionString = lstAllEnterPrises.First().EnterpriseDBConnectionString;
                                    }
                                    SessionHelper.EnterPriseList = lstAllEnterPrises;
                                    objCompanyMasterDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
                                    //List<CompanyMasterDTO> lstAllCompanies = objCompanyMasterDAL.GetAllRecords().OrderBy(t => t.Name).ToList();
                                    List<CompanyMasterDTO> lstAllCompanies = (from cm in objCompanyMasterDAL.GetAllCompanies()
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
                                        List<RoomDTO> lstRooms = objRoomDAL.GetRoomByCompanyIDPlain(SessionHelper.CompanyID).OrderByDescending(x => x.IsRoomActive).ThenBy(x => x.RoomName).ToList();

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
                                                //SessionHelper.isEVMI = objRoomDTO.IseVMI;
                                            }
                                            else
                                            {
                                                SessionHelper.RoomID = lstRooms.First().ID;
                                                SessionHelper.RoomName = lstRooms.First().RoomName;
                                                //SessionHelper.isEVMI = lstRooms.First().IseVMI;
                                            }
                                            SessionHelper.RoomList = lstRooms;
                                        }
                                        else
                                        {
                                            SessionHelper.RoomID = 0;
                                            SessionHelper.RoomName = string.Empty;
                                            // SessionHelper.isEVMI = null;
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
                                        //SessionHelper.isEVMI = null;
                                    }
                                }
                                else
                                {
                                    SessionHelper.EnterPriceID = 0;
                                    SessionHelper.EnterpriseLogoUrl = string.Empty;
                                    SessionHelper.EnterPriseDBName = string.Empty;
                                    SessionHelper.IsABEnterprise = false;
                                    //SessionHelper.EnterPriseDBConnectionString = string.Empty;
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
                                SessionHelper.SetSessionCompleted = false;
                                string strRoomList = string.Empty;

                                //List<UserRoleModuleDetailsDTO> lstUserRoleModuleDetailsDTO = objUserMasterDAL.GetUserRoleModuleDetailsRecord(UserId, RoleId, UserType);
                                List<UserRoleModuleDetailsDTO> lstUserRoleModuleDetailsDTO = objUserMasterDAL.GetUserRoleModuleDetailsDefault(UserId, RoleId);
                                List<UserWiseRoomsAccessDetailsDTO> lstUserWiseRoomsAccessDetails = objUserMasterDAL.ConvertUserPermissions(lstUserRoleModuleDetailsDTO, RoleId, ref strRoomList);
                                List<UserAccessDTO> lstAccess = objUserMasterDAL.GetUserRoomAccessForSuperUserByUserIdPlain(UserId);
                                //List<UserAccessDTO> lstAccess = objEnterpriseDAL.GetUserAccessWithNames(UserId);



                                #region set single enterprise,company,room
                                //List<UserAccessDTO> lstAccess = new List<UserAccessDTO>();
                                //if (lstUserRoleModuleDetailsDTO != null && lstUserRoleModuleDetailsDTO.Any() && lstUserRoleModuleDetailsDTO.Count > 0)
                                //{
                                //    lstAccess = GetUserAccessFromUserRoleModule(lstUserRoleModuleDetailsDTO[0]);
                                //}
                                #endregion

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
                                        objEnterpriseDTO = objEnterpriseDAL.GetEnterpriseByIdPlain(SessionHelper.EnterPriceID);
                                        SessionHelper.EnterPriceName = objEnterpriseDTO.Name;
                                        SessionHelper.EnterpriseLogoUrl = objEnterpriseDTO.EnterpriseLogo;
                                        SessionHelper.EnterPriseDBName = objEnterpriseDTO.EnterpriseDBName;
                                        SessionHelper.IsABEnterprise = objEnterpriseDTO.AllowABIntegration;
                                        //SessionHelper.EnterPriseDBConnectionString = objEnterpriseDTO.EnterpriseDBConnectionString;
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
                                            objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
                                            List<RoomDTO> lstAllRooms = new List<RoomDTO>();

                                            //if (RoleId > 0)
                                            //{
                                            //    lstAccess.ForEach(z => z.UserId = UserId);
                                            //    EnterpriseMasterDAL enterPriseMasterDAL = new EnterpriseMasterDAL();
                                            //    var enterpriseIds = lstAccess.Select(e => e.EnterpriseId).Distinct().ToList();
                                            //    Dictionary<long, string> enterpriseList = enterPriseMasterDAL.GetEnterpriseListWithDBName(enterpriseIds);

                                            //    if (enterpriseList != null && enterpriseList.Any())
                                            //    {
                                            //        lstAllRooms = objRoomDAL.GetRoomsByUserAccessAndRoomSupplierFilterForSuperAdmin(lstAccess.Where(e => e.RoomId > 0).ToList(), enterpriseList);
                                            //    }
                                            //}
                                            //else
                                            //{
                                            lstAllRooms = (from itm in lstAccess.Where(t => t.RoomId > 0)
                                                           orderby itm.IsRoomActive descending, itm.RoomName ascending
                                                           group itm by new { itm.EnterpriseId, itm.EnterpriseName, itm.CompanyId, itm.CompanyName, itm.RoomId, itm.RoomName, itm.IsRoomActive, itm.isEVMI } into gropedentrprs
                                                           select new RoomDTO
                                                           {
                                                               EnterpriseId = gropedentrprs.Key.EnterpriseId,
                                                               EnterpriseName = gropedentrprs.Key.EnterpriseName,
                                                               CompanyID = gropedentrprs.Key.CompanyId,
                                                               CompanyName = gropedentrprs.Key.CompanyName,
                                                               ID = gropedentrprs.Key.RoomId,
                                                               RoomName = gropedentrprs.Key.RoomName,
                                                               IsRoomActive = gropedentrprs.Key.IsRoomActive,
                                                               IseVMI = gropedentrprs.Key.isEVMI
                                                           }).ToList();
                                            //}


                                            SessionHelper.RoomList = lstAllRooms;
                                            if (lstAllRooms != null && lstAllRooms.Count(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID) > 0)
                                            {
                                                if (objUserLoginHistoryDTO != null && objUserLoginHistoryDTO.RoomId > 0 && lstAllRooms.Any(t => t.EnterpriseId == objUserLoginHistoryDTO.EnterpriseId && t.CompanyID == objUserLoginHistoryDTO.CompanyId && t.ID == objUserLoginHistoryDTO.RoomId))
                                                {
                                                    objRoomDTO = lstAllRooms.First(t => t.EnterpriseId == objUserLoginHistoryDTO.EnterpriseId && t.CompanyID == objUserLoginHistoryDTO.CompanyId && t.ID == objUserLoginHistoryDTO.RoomId);
                                                    SessionHelper.RoomID = objRoomDTO.ID;
                                                    SessionHelper.RoomName = objRoomDTO.RoomName;
                                                    //SessionHelper.isEVMI = objRoomDTO.IseVMI;
                                                }
                                                else
                                                {
                                                    SessionHelper.RoomID = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().ID;
                                                    SessionHelper.RoomName = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().RoomName;
                                                    //SessionHelper.isEVMI = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().IseVMI;
                                                }
                                            }
                                            else
                                            {
                                                SessionHelper.RoomID = 0;
                                                SessionHelper.RoomName = string.Empty;
                                                //SessionHelper.isEVMI = null;
                                            }
                                        }
                                        else
                                        {
                                            SessionHelper.CompanyID = 0;
                                            SessionHelper.CompanyName = string.Empty;
                                            SessionHelper.RoomID = 0;
                                            SessionHelper.RoomName = string.Empty;
                                            //SessionHelper.isEVMI = null;
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
                                        SessionHelper.IsABEnterprise = false;
                                        SessionHelper.EnterPriseList = new List<EnterpriseDTO>();
                                        SessionHelper.CompanyList = new List<CompanyMasterDTO>();
                                        SessionHelper.RoomList = new List<RoomDTO>();
                                        //SessionHelper.isEVMI = null;
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
                                objEnterpriseDTO = objEnterpriseDAL.GetEnterpriseByIdPlain(SessionHelper.EnterPriceID);
                                SessionHelper.EnterpriseLogoUrl = objEnterpriseDTO.EnterpriseLogo;
                                SessionHelper.EnterPriceName = objEnterpriseDTO.Name;
                                SessionHelper.EnterPriseDBName = objEnterpriseDTO.EnterpriseDBName;
                                SessionHelper.IsABEnterprise = objEnterpriseDTO.AllowABIntegration;
                                //SessionHelper.EnterPriseDBConnectionString = objEnterpriseDTO.EnterpriseDBConnectionString;
                                List<EnterpriseDTO> lstEnterprise = new List<EnterpriseDTO>();
                                lstEnterprise.Add(objEnterpriseDTO);
                                SessionHelper.EnterPriseList = lstEnterprise;
                                objCompanyMasterDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
                                List<CompanyMasterDTO> lstAllCompanies = (from cm in objCompanyMasterDAL.GetAllCompanies()
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
                                    List<RoomDTO> lstRooms = objRoomDAL.GetRoomByCompanyIDPlain(SessionHelper.CompanyID).OrderByDescending(x => x.IsRoomActive).ThenBy(r => r.RoomName).ToList();
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
                                            //SessionHelper.isEVMI = objRoomDTO.IseVMI;
                                        }
                                        else
                                        {
                                            SessionHelper.RoomID = lstRooms.First().ID;
                                            SessionHelper.RoomName = lstRooms.First().RoomName;
                                            //SessionHelper.isEVMI = lstRooms.First().IseVMI;
                                        }
                                        SessionHelper.RoomList = lstRooms;
                                    }
                                    else
                                    {
                                        SessionHelper.RoomID = 0;
                                        SessionHelper.RoomName = string.Empty;
                                        SessionHelper.RoomList = new List<RoomDTO>();
                                        // SessionHelper.isEVMI = null;
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
                                    //SessionHelper.isEVMI = null;
                                }
                                objinterUserDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                                List<UserWiseRoomsAccessDetailsDTO> lstUserWiseRoomsAccessDetailsDTO = objinterUserDAL.GetEnterpriseAdminRoomPermissions(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.RoleID, SessionHelper.UserID);
                                SessionHelper.RoomPermissions = SetPermissionsForsuperadmin(lstUserWiseRoomsAccessDetailsDTO, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.RoleID, SessionHelper.UserType);
                            }
                            else
                            {
                                SessionHelper.EnterPriceID = EnterpriseId;
                                objEnterpriseDTO = objEnterpriseDAL.GetEnterpriseByIdPlain(SessionHelper.EnterPriceID);
                                SessionHelper.EnterpriseLogoUrl = objEnterpriseDTO.EnterpriseLogo;
                                SessionHelper.EnterPriseDBName = objEnterpriseDTO.EnterpriseDBName;
                                SessionHelper.IsABEnterprise = objEnterpriseDTO.AllowABIntegration;
                                //SessionHelper.EnterPriseDBConnectionString = objEnterpriseDTO.EnterpriseDBConnectionString;
                                List<EnterpriseDTO> lstEnterprise = new List<EnterpriseDTO>();
                                lstEnterprise.Add(objEnterpriseDTO);
                                SessionHelper.EnterPriseList = lstEnterprise;
                                string strRoomList = string.Empty;
                                objinterUserDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                                List<UserRoleModuleDetailsDTO> lstUserRoleModuleDetailsDTO = objinterUserDAL.GetUserRoleModuleDetailsByUserIdAndRoleId(UserId, RoleId);
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
                                            SessionHelper.CompanyLogoUrl = objCompanyMasterDTO.CompanyLogo;
                                        }
                                        else
                                        {
                                            SessionHelper.CompanyID = lstAllCompanies.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).OrderBy(t => t.Name).First().ID;
                                            SessionHelper.CompanyName = lstAllCompanies.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).OrderBy(t => t.Name).First().Name;
                                            SessionHelper.CompanyLogoUrl = lstAllCompanies.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).OrderBy(t => t.Name).First().CompanyLogo;
                                        }

                                        objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
                                        List<RoomDTO> lstAllRooms = new List<RoomDTO>();

                                        if (RoleId > 0)
                                        {
                                            lstAllRooms = objRoomDAL.GetRoomsByUserAccessAndRoomSupplierFilter(lstAccess);
                                        }
                                        else
                                        {
                                            lstAllRooms = (from itm in lstAccess.Where(t => t.RoomId > 0)
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
                                        }


                                        SessionHelper.RoomList = lstAllRooms;
                                        if (lstAllRooms != null && lstAllRooms.Count(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID) > 0)
                                        {
                                            if (objUserLoginHistoryDTO != null && objUserLoginHistoryDTO.RoomId > 0 && lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID && t.ID == objUserLoginHistoryDTO.RoomId).Any())
                                            {
                                                SessionHelper.RoomID = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID && t.ID == objUserLoginHistoryDTO.RoomId).OrderBy(t => t.RoomName).First().ID;
                                                SessionHelper.RoomName = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID && t.ID == objUserLoginHistoryDTO.RoomId).OrderBy(t => t.RoomName).First().RoomName;
                                                //SessionHelper.isEVMI = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID && t.ID == objUserLoginHistoryDTO.RoomId).OrderBy(t => t.RoomName).First().IseVMI;
                                            }
                                            else
                                            {
                                                SessionHelper.RoomID = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().ID;
                                                SessionHelper.RoomName = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().RoomName;
                                                //SessionHelper.isEVMI = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().IseVMI;
                                            }
                                        }
                                        else
                                        {
                                            SessionHelper.RoomID = 0;
                                            SessionHelper.RoomName = string.Empty;
                                            //SessionHelper.isEVMI = null;
                                        }
                                    }
                                }

                            }
                            break;
                        case 3:
                            SessionHelper.EnterPriceID = EnterpriseId;
                            objEnterpriseDTO = objEnterpriseDAL.GetEnterpriseByIdPlain(SessionHelper.EnterPriceID);
                            SessionHelper.EnterpriseLogoUrl = objEnterpriseDTO.EnterpriseLogo;
                            SessionHelper.EnterPriseDBName = objEnterpriseDTO.EnterpriseDBName;
                            SessionHelper.IsABEnterprise = objEnterpriseDTO.AllowABIntegration;
                            //SessionHelper.EnterPriseDBConnectionString = objEnterpriseDTO.EnterpriseDBConnectionString;
                            List<EnterpriseDTO> lstEnterprise3 = new List<EnterpriseDTO>();
                            lstEnterprise3.Add(objEnterpriseDTO);
                            SessionHelper.EnterPriseList = lstEnterprise3;
                            SessionHelper.CompanyID = CompanyId;
                            string strRoomList3 = string.Empty;
                            objinterUserDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                            List<UserRoleModuleDetailsDTO> lstUserRoleModuleDetailsDTO3 = objinterUserDAL.GetUserRoleModuleDetailsByUserIdAndRoleId(UserId, RoleId);
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

                                    objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
                                    List<RoomDTO> lstAllRooms = new List<RoomDTO>();

                                    if (RoleId > 0)
                                    {
                                        lstAllRooms = objRoomDAL.GetRoomsByUserAccessAndRoomSupplierFilter(lstAccess1);
                                    }
                                    else
                                    {
                                        lstAllRooms = (from itm in lstAccess1
                                                       orderby itm.IsRoomActive descending, itm.RoomName ascending
                                                       group itm by new { itm.EnterpriseId, itm.EnterpriseName, itm.CompanyId, itm.CompanyName, itm.RoomId, itm.RoomName, itm.IsRoomActive, itm.isEVMI } into gropedentrprs
                                                       select new RoomDTO
                                                       {
                                                           EnterpriseId = gropedentrprs.Key.EnterpriseId,
                                                           EnterpriseName = gropedentrprs.Key.EnterpriseName,
                                                           CompanyID = gropedentrprs.Key.CompanyId,
                                                           CompanyName = gropedentrprs.Key.CompanyName,
                                                           ID = gropedentrprs.Key.RoomId,
                                                           RoomName = gropedentrprs.Key.RoomName,
                                                           IsRoomActive = gropedentrprs.Key.IsRoomActive,
                                                           IseVMI = gropedentrprs.Key.isEVMI
                                                       }).ToList();
                                    }

                                    SessionHelper.RoomList = lstAllRooms;
                                    if (lstAllRooms != null && lstAllRooms.Count(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID) > 0)
                                    {
                                        if (objUserLoginHistoryDTO != null && objUserLoginHistoryDTO.RoomId > 0 && lstAllRooms.Any(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID && t.ID == objUserLoginHistoryDTO.RoomId))
                                        {
                                            SessionHelper.RoomID = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID && t.ID == objUserLoginHistoryDTO.RoomId).First().ID;
                                            SessionHelper.RoomName = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID && t.ID == objUserLoginHistoryDTO.RoomId).First().RoomName;
                                            // SessionHelper.isEVMI = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID && t.ID == objUserLoginHistoryDTO.RoomId).First().IseVMI;
                                        }
                                        else
                                        {
                                            SessionHelper.RoomID = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().ID;
                                            SessionHelper.RoomName = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().RoomName;
                                            //SessionHelper.isEVMI = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().IseVMI;
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
                                objEnterpriseDTO = objEnterpriseDAL.GetEnterpriseByIdPlain(SessionHelper.EnterPriceID);
                                SessionHelper.EnterpriseLogoUrl = objEnterpriseDTO.EnterpriseLogo;
                                SessionHelper.EnterPriseDBName = objEnterpriseDTO.EnterpriseDBName;
                                SessionHelper.IsABEnterprise = objEnterpriseDTO.AllowABIntegration;
                                //SessionHelper.EnterPriseDBConnectionString = objEnterpriseDTO.EnterpriseDBConnectionString;
                                objCompanyMasterDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
                                List<CompanyMasterDTO> lstAllCompanies = (from cm in objCompanyMasterDAL.GetAllCompanies()
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
                                    List<RoomDTO> lstRooms = objRoomDAL.GetRoomByCompanyIDPlain(SessionHelper.CompanyID).OrderByDescending(x => x.IsRoomActive).ThenBy(r => r.RoomName).ToList();

                                    if (lstRooms != null && lstRooms.Count() > 0)
                                    {
                                        lstRooms.ForEach(t =>
                                        {
                                            t.EnterpriseId = SessionHelper.EnterPriceID;
                                            t.EnterpriseName = SessionHelper.EnterPriceName;
                                        });
                                        SessionHelper.RoomID = lstRooms.First().ID;
                                        SessionHelper.RoomName = lstRooms.First().RoomName;
                                        //SessionHelper.isEVMI = lstRooms.First().IseVMI;
                                        SessionHelper.RoomList = lstRooms;
                                    }
                                    else
                                    {
                                        SessionHelper.RoomID = 0;
                                        SessionHelper.RoomName = string.Empty;
                                        //SessionHelper.isEVMI = null;
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
                                    //SessionHelper.isEVMI = null;
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
                                objEnterpriseDTO = objEnterpriseDAL.GetEnterpriseByIdPlain(SessionHelper.EnterPriceID);
                                SessionHelper.EnterpriseLogoUrl = objEnterpriseDTO.EnterpriseLogo;
                                SessionHelper.EnterPriseDBName = objEnterpriseDTO.EnterpriseDBName;
                                SessionHelper.IsABEnterprise = objEnterpriseDTO.AllowABIntegration;
                                //SessionHelper.EnterPriseDBConnectionString = objEnterpriseDTO.EnterpriseDBConnectionString;
                                SessionHelper.EnterPriceName = objEnterpriseDTO.Name;

                                if (SessionHelper.CompanyList != null && SessionHelper.CompanyList.Any(t => t.EnterPriseId == SessionHelper.EnterPriceID))
                                {
                                    SessionHelper.CompanyID = SessionHelper.CompanyList.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).OrderBy(t => t.Name).First().ID;
                                    SessionHelper.CompanyName = SessionHelper.CompanyList.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).OrderBy(t => t.Name).First().Name;
                                    if (SessionHelper.RoomList == null)
                                    {

                                        objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
                                        List<RoomDTO> lstAllRooms = new List<RoomDTO>();
                                        List<UserAccessDTO> lstAccess = objEnterpriseDAL.GetUserAccessWithNames(UserId);
                                        if (lstAccess != null && lstAccess.Count > 0)
                                        {
                                            if (RoleId > 0)
                                            {
                                                lstAccess.ForEach(z => z.UserId = UserId);
                                                EnterpriseMasterDAL enterPriseMasterDAL = new EnterpriseMasterDAL();
                                                var enterpriseIds = lstAccess.Select(e => e.EnterpriseId).Distinct().ToList();
                                                Dictionary<long, string> enterpriseList = enterPriseMasterDAL.GetEnterpriseListWithDBName(enterpriseIds);

                                                if (enterpriseList != null && enterpriseList.Any())
                                                {
                                                    lstAllRooms = objRoomDAL.GetRoomsByUserAccessAndRoomSupplierFilterForSuperAdmin(lstAccess.Where(e => e.RoomId > 0).ToList(), enterpriseList);
                                                }
                                            }
                                            else
                                            {
                                                lstAllRooms = (from itm in lstAccess.Where(t => t.RoomId > 0)
                                                               orderby itm.IsRoomActive descending, itm.RoomName ascending
                                                               group itm by new { itm.EnterpriseId, itm.EnterpriseName, itm.CompanyId, itm.CompanyName, itm.RoomId, itm.RoomName, itm.IsRoomActive, itm.isEVMI } into gropedentrprs
                                                               select new RoomDTO
                                                               {
                                                                   EnterpriseId = gropedentrprs.Key.EnterpriseId,
                                                                   EnterpriseName = gropedentrprs.Key.EnterpriseName,
                                                                   CompanyID = gropedentrprs.Key.CompanyId,
                                                                   CompanyName = gropedentrprs.Key.CompanyName,
                                                                   ID = gropedentrprs.Key.RoomId,
                                                                   RoomName = gropedentrprs.Key.RoomName,
                                                                   IsRoomActive = gropedentrprs.Key.IsRoomActive,
                                                                   IseVMI = gropedentrprs.Key.isEVMI
                                                               }).ToList();
                                            }
                                            SessionHelper.RoomList = lstAllRooms;

                                        }

                                    }
                                    if (SessionHelper.RoomList != null && SessionHelper.RoomList.Any(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID))
                                    {
                                        SessionHelper.RoomID = SessionHelper.RoomList.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().ID;
                                        SessionHelper.RoomName = SessionHelper.RoomList.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().RoomName;
                                        //SessionHelper.isEVMI = SessionHelper.RoomList.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().IseVMI;
                                    }
                                    else
                                    {
                                        SessionHelper.RoomID = 0;
                                        SessionHelper.RoomName = string.Empty;
                                        //SessionHelper.isEVMI = null;
                                    }
                                }
                                else
                                {
                                    SessionHelper.CompanyID = 0;
                                    SessionHelper.CompanyName = string.Empty;
                                    SessionHelper.RoomID = 0;
                                    SessionHelper.RoomName = string.Empty;
                                    //SessionHelper.isEVMI = null;
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
                    Session["ReportPara"] = null;
                    Session["IsFromSelectedDomain"] = null;
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
                                CompanyMasterDTO objCompanyMaster = objCompanyMasterDAL.GetCompanyByID(CompanyId);
                                SessionHelper.CompanyLogoUrl = objCompanyMaster.CompanyLogo;
                                objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
                                List<RoomDTO> lstRooms = objRoomDAL.GetRoomByCompanyIDPlain(SessionHelper.CompanyID).OrderByDescending(x => x.IsRoomActive).ThenBy(r => r.RoomName).ToList();

                                if (lstRooms != null && lstRooms.Count() > 0)
                                {
                                    lstRooms.ForEach(t =>
                                    {
                                        t.EnterpriseId = SessionHelper.EnterPriceID;
                                        t.EnterpriseName = SessionHelper.EnterPriceName;
                                    });

                                    SessionHelper.RoomID = lstRooms.First().ID;
                                    SessionHelper.RoomName = lstRooms.First().RoomName;
                                    //SessionHelper.isEVMI = lstRooms.First().IseVMI;
                                    SessionHelper.RoomList = lstRooms;
                                }
                                else
                                {
                                    SessionHelper.RoomID = 0;
                                    SessionHelper.RoomName = string.Empty;
                                    //SessionHelper.isEVMI = null;
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
                                CompanyMasterDTO objCompanyMaster = objCompanyMasterDAL.GetCompanyByID(CompanyId);
                                SessionHelper.CompanyLogoUrl = objCompanyMaster.CompanyLogo;
                                List<RoomDTO> lstAllrooms = SessionHelper.RoomList;
                                if (SessionHelper.RoomList != null && SessionHelper.RoomList.Any(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID))
                                {
                                    SessionHelper.RoomID = SessionHelper.RoomList.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().ID;
                                    SessionHelper.RoomName = SessionHelper.RoomList.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().RoomName;
                                    //SessionHelper.isEVMI = SessionHelper.RoomList.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().IseVMI;
                                }
                                else
                                {
                                    SessionHelper.RoomID = 0;
                                    SessionHelper.RoomName = string.Empty;
                                    //SessionHelper.isEVMI = null;
                                }
                            }
                            break;
                        case 2:
                            if (RoleId == -2)
                            {
                                SessionHelper.CompanyID = CompanyId;
                                SessionHelper.CompanyName = CompanyName;
                                objCompanyMasterDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
                                CompanyMasterDTO objCompanyMaster = objCompanyMasterDAL.GetCompanyByID(CompanyId);
                                SessionHelper.CompanyLogoUrl = objCompanyMaster.CompanyLogo;
                                objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
                                List<RoomDTO> lstRooms = objRoomDAL.GetRoomByCompanyIDPlain(SessionHelper.CompanyID).OrderByDescending(x => x.IsRoomActive).ThenBy(r => r.RoomName).ToList();

                                if (lstRooms != null && lstRooms.Count() > 0)
                                {
                                    lstRooms.ForEach(t =>
                                    {
                                        t.EnterpriseId = SessionHelper.EnterPriceID;
                                        t.EnterpriseName = SessionHelper.EnterPriceName;
                                    });
                                    SessionHelper.RoomID = lstRooms.First().ID;
                                    SessionHelper.RoomName = lstRooms.First().RoomName;
                                    //SessionHelper.isEVMI = lstRooms.First().IseVMI;
                                    SessionHelper.RoomList = lstRooms;
                                }
                                else
                                {
                                    SessionHelper.RoomID = 0;
                                    SessionHelper.RoomName = string.Empty;
                                    //SessionHelper.isEVMI = null;
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
                                CompanyMasterDTO objCompanyMaster = objCompanyMasterDAL.GetCompanyByID(CompanyId);
                                SessionHelper.CompanyLogoUrl = objCompanyMaster.CompanyLogo;
                                List<RoomDTO> lstAllrooms = SessionHelper.RoomList;
                                if (lstAllrooms != null && lstAllrooms.Any(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID))
                                {
                                    SessionHelper.RoomID = lstAllrooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().ID;
                                    SessionHelper.RoomName = lstAllrooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().RoomName;
                                    //SessionHelper.isEVMI = lstAllrooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().IseVMI;
                                }
                                else
                                {
                                    SessionHelper.RoomID = 0;
                                    SessionHelper.RoomName = string.Empty;
                                    //SessionHelper.isEVMI = null;
                                }
                            }
                            break;
                    }
                    Session["ReportPara"] = null;
                    break;
                case "room":
                    SessionHelper.RoomID = RoomId;
                    SessionHelper.RoomName = RoomName;
                    if (SessionHelper.CompanyID > 0)
                    {
                        string columnList = "ID,RoomName,IseVMI";
                        RoomDTO objRoom = new CommonDAL(SessionHelper.EnterPriseDBName).GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");
                        // RoomDTO objRoom = new RoomDAL(SessionHelper.EnterPriseDBName).GetRoomByIDPlain(SessionHelper.RoomID);
                        if (objRoom != null)
                            SessionHelper.isEVMI = objRoom.IseVMI;
                        else
                            SessionHelper.isEVMI = null;
                    }
                    else
                        SessionHelper.isEVMI = null;
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
                    Session["ReportPara"] = null;
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
                SessionHelper.RoomTimeFormat = RegionInfo.ShortTimePattern;
                SessionHelper.NumberDecimalDigits = Convert.ToString(RegionInfo.NumberDecimalDigits);
                SessionHelper.CurrencyDecimalDigits = Convert.ToString(RegionInfo.CurrencyDecimalDigits);
                SessionHelper.WeightDecimalPoints = Convert.ToString(RegionInfo.WeightDecimalPoints.GetValueOrDefault(0));
                SessionHelper.NumberAvgDecimalPoints = Convert.ToString(RegionInfo.TurnsAvgDecimalPoints.GetValueOrDefault(2));
                SessionHelper.CurrencySymbol = RegionInfo.CurrencySymbol ?? "";


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
                SessionHelper.RoomTimeFormat = "h:mm:ss tt";
                SessionHelper.NumberDecimalDigits = "0";
                SessionHelper.CurrencyDecimalDigits = "0";
                SessionHelper.NumberAvgDecimalPoints = "2";
                SessionHelper.WeightDecimalPoints = "0";
                SessionHelper.QuantityFormat = "{0:0}";
                SessionHelper.PriceFormat = "{0:0}";
                SessionHelper.TurnUsageFormat = "{0:0}";
                SessionHelper.WeightFormat = "{0:0}";
                SessionHelper.CurrencySymbol = "";

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
            ResourceModuleHelper.EnterpriseResourceFolder = SessionHelper.EnterPriceID.ToString();
            ResourceModuleHelper.CompanyResourceFolder = SessionHelper.EnterPriceID.ToString() + @"\" + SessionHelper.CompanyID.ToString();
            ResourceModuleHelper.RoomResourceFolder = SessionHelper.EnterPriceID.ToString() + @"\" + SessionHelper.CompanyID.ToString() + @"\" + SessionHelper.RoomID.ToString();
            EnterPriseResourceHelper.EnterpriseResourceFolder = SessionHelper.EnterPriceID.ToString();
            EnterPriseResourceHelper.CompanyResourceFolder = SessionHelper.EnterPriceID.ToString() + @"\CompanyResources";
            EnterPriseResourceModuleHelper.EnterpriseResourceFolder = SessionHelper.EnterPriceID.ToString();
            EnterPriseResourceModuleHelper.CompanyResourceFolder = SessionHelper.EnterPriceID.ToString() + @"\CompanyResources";
            //ResourceHelper.CompanyResourceFolder = SessionHelper.CompanyID.ToString();
            if (eTurns.DTO.Resources.ResourceHelper.CurrentCult == null)
                eTurns.DTO.Resources.ResourceHelper.CurrentCult = System.Threading.Thread.CurrentThread.CurrentCulture;
            if (eTurns.DTO.Resources.ResourceModuleHelper.CurrentCult == null)
                eTurns.DTO.Resources.ResourceModuleHelper.CurrentCult = System.Threading.Thread.CurrentThread.CurrentCulture;

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
            ////Set sensorbin setting information....

            CommonFunctions.SetSensorBinRoomSettings(SessionHelper.EnterPriseDBName, SessionHelper.RoomID, SessionHelper.EnterPriceID, SessionHelper.CompanyID);

            objUserMasterDAL.SaveUserActions(objUserLoginHistory);

        }



        //public static void SetSessions1(
        //    HttpRequestBase Request, HttpSessionStateBase Session,HttpContextBase HttpContext,
        //     long EnterpriseId, long CompanyId, long RoomId, long UserId, int UserType, long RoleId, string EventFiredOn, string enterpriseName, string CompanyName, string RoomName)
        //{

        //    eTurnsMaster.DAL.EnterpriseMasterDAL objEnterpriseDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
        //    eTurnsMaster.DAL.UserMasterDAL objUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL();
        //    CompanyMasterDAL objCompanyMasterDAL;
        //    RoomDAL objRoomDAL;
        //    eTurns.DAL.UserMasterDAL objinterUserDAL;
        //    EnterpriseDTO objEnterpriseDTO = new EnterpriseDTO();
        //    CompanyMasterDTO objCompanyMasterDTO = new CompanyMasterDTO();
        //    RoomDTO objRoomDTO = new RoomDTO();
        //    switch (EventFiredOn)
        //    {
        //        case "onlogin":
        //            UserLoginHistoryDTO objUserLoginHistoryDTO = new UserLoginHistoryDTO();
        //            objUserLoginHistoryDTO = objUserMasterDAL.GetUserLastActionDetail(UserId);
        //            SessionHelper.SetSessionCompleted = true;

        //            switch (UserType)
        //            {

        //                case 1:
        //                    if (RoleId == -1)
        //                    {
        //                        List<EnterpriseDTO> lstAllEnterPrises = (from em in objEnterpriseDAL.GetAllEnterprisesPlain()
        //                                                                 orderby em.IsActive descending, em.Name ascending
        //                                                                 select em).ToList();
        //                        //objEnterpriseDAL.GetAllEnterprise().Where(t => t.IsArchived == false && t.IsDeleted == false).OrderByDescending(t => t.IsActive).ToList();

        //                        if (lstAllEnterPrises != null && lstAllEnterPrises.Count() > 0)
        //                        {
        //                            if (objUserLoginHistoryDTO != null && objUserLoginHistoryDTO.EnterpriseId > 0 && lstAllEnterPrises.Any(t => t.ID == objUserLoginHistoryDTO.EnterpriseId))
        //                            {
        //                                objEnterpriseDTO = lstAllEnterPrises.First(t => t.ID == objUserLoginHistoryDTO.EnterpriseId);
        //                                SessionHelper.EnterPriceID = objEnterpriseDTO.ID;
        //                                SessionHelper.EnterpriseLogoUrl = objEnterpriseDTO.EnterpriseLogo;
        //                                SessionHelper.EnterPriseDBName = objEnterpriseDTO.EnterpriseDBName;
        //                                //SessionHelper.EnterPriseDBConnectionString = objEnterpriseDTO.EnterpriseDBConnectionString;
        //                            }
        //                            else
        //                            {
        //                                SessionHelper.EnterPriceID = lstAllEnterPrises.First().ID;
        //                                SessionHelper.EnterpriseLogoUrl = lstAllEnterPrises.First().EnterpriseLogo;
        //                                SessionHelper.EnterPriseDBName = lstAllEnterPrises.First().EnterpriseDBName;
        //                                //SessionHelper.EnterPriseDBConnectionString = lstAllEnterPrises.First().EnterpriseDBConnectionString;
        //                            }
        //                            SessionHelper.EnterPriseList = lstAllEnterPrises;
        //                            objCompanyMasterDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
        //                            //List<CompanyMasterDTO> lstAllCompanies = objCompanyMasterDAL.GetAllRecords().OrderBy(t => t.Name).ToList();
        //                            List<CompanyMasterDTO> lstAllCompanies = (from cm in objCompanyMasterDAL.GetAllCompanies()
        //                                                                      orderby cm.IsActive descending, cm.Name ascending
        //                                                                      select cm).ToList();
        //                            if (lstAllCompanies != null && lstAllCompanies.Count() > 0)
        //                            {
        //                                lstAllCompanies.ForEach(t =>
        //                                {
        //                                    t.EnterPriseId = SessionHelper.EnterPriceID;
        //                                    t.EnterPriseName = SessionHelper.EnterPriceName;
        //                                });
        //                                if (objUserLoginHistoryDTO != null && objUserLoginHistoryDTO.CompanyId > 0 && lstAllCompanies.Any(t => t.EnterPriseId == SessionHelper.EnterPriceID && t.ID == objUserLoginHistoryDTO.CompanyId))
        //                                {
        //                                    objCompanyMasterDTO = lstAllCompanies.First(t => t.EnterPriseId == SessionHelper.EnterPriceID && t.ID == objUserLoginHistoryDTO.CompanyId);
        //                                    SessionHelper.CompanyID = objCompanyMasterDTO.ID;
        //                                    SessionHelper.CompanyLogoUrl = objCompanyMasterDTO.CompanyLogo;
        //                                    SessionHelper.CompanyName = objCompanyMasterDTO.Name;
        //                                }
        //                                else
        //                                {
        //                                    SessionHelper.CompanyID = lstAllCompanies.First().ID;
        //                                    SessionHelper.CompanyLogoUrl = lstAllCompanies.First().CompanyLogo;
        //                                    SessionHelper.CompanyName = lstAllCompanies.First().Name;
        //                                }
        //                                SessionHelper.CompanyList = lstAllCompanies;
        //                                objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
        //                                List<RoomDTO> lstRooms = objRoomDAL.GetRoomByCompanyIDPlain(SessionHelper.CompanyID).OrderByDescending(x => x.IsRoomActive).ThenBy(x => x.RoomName).ToList();

        //                                if (lstRooms != null && lstRooms.Count() > 0)
        //                                {
        //                                    lstRooms.ForEach(t =>
        //                                    {
        //                                        t.EnterpriseId = SessionHelper.EnterPriceID;
        //                                        t.EnterpriseName = SessionHelper.EnterPriceName;
        //                                    });
        //                                    if (objUserLoginHistoryDTO != null && objUserLoginHistoryDTO.RoomId > 0 && lstRooms.Any(t => t.EnterpriseId == objUserLoginHistoryDTO.EnterpriseId && t.CompanyID == objUserLoginHistoryDTO.CompanyId && t.ID == objUserLoginHistoryDTO.RoomId))
        //                                    {
        //                                        objRoomDTO = lstRooms.First(t => t.EnterpriseId == objUserLoginHistoryDTO.EnterpriseId && t.CompanyID == objUserLoginHistoryDTO.CompanyId && t.ID == objUserLoginHistoryDTO.RoomId);
        //                                        SessionHelper.RoomID = objRoomDTO.ID;
        //                                        SessionHelper.RoomName = objRoomDTO.RoomName;
        //                                        //SessionHelper.isEVMI = objRoomDTO.IseVMI;
        //                                    }
        //                                    else
        //                                    {
        //                                        SessionHelper.RoomID = lstRooms.First().ID;
        //                                        SessionHelper.RoomName = lstRooms.First().RoomName;
        //                                        //SessionHelper.isEVMI = lstRooms.First().IseVMI;
        //                                    }
        //                                    SessionHelper.RoomList = lstRooms;
        //                                }
        //                                else
        //                                {
        //                                    SessionHelper.RoomID = 0;
        //                                    SessionHelper.RoomName = string.Empty;
        //                                    // SessionHelper.isEVMI = null;
        //                                    SessionHelper.RoomList = new List<RoomDTO>();
        //                                }
        //                            }
        //                            else
        //                            {
        //                                SessionHelper.CompanyID = 0;
        //                                SessionHelper.CompanyName = string.Empty;
        //                                SessionHelper.CompanyLogoUrl = string.Empty;
        //                                SessionHelper.CompanyList = new List<CompanyMasterDTO>();
        //                                SessionHelper.RoomID = 0;
        //                                SessionHelper.RoomName = string.Empty;
        //                                SessionHelper.RoomList = new List<RoomDTO>();
        //                                //SessionHelper.isEVMI = null;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            SessionHelper.EnterPriceID = 0;
        //                            SessionHelper.EnterpriseLogoUrl = string.Empty;
        //                            SessionHelper.EnterPriseDBName = string.Empty;
        //                            //SessionHelper.EnterPriseDBConnectionString = string.Empty;
        //                            SessionHelper.EnterPriseList = new List<EnterpriseDTO>();
        //                            SessionHelper.CompanyID = 0;
        //                            SessionHelper.CompanyName = string.Empty;
        //                            SessionHelper.CompanyList = new List<CompanyMasterDTO>();
        //                        }

        //                        List<UserWiseRoomsAccessDetailsDTO> lstUserWiseRoomsAccessDetailsDTO = objUserMasterDAL.GetSuperAdminRoomPermissions(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.RoleID, SessionHelper.UserID);
        //                        SessionHelper.RoomPermissions = SetPermissionsForsuperadmin(lstUserWiseRoomsAccessDetailsDTO, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.RoleID, SessionHelper.UserType);

        //                    }
        //                    else
        //                    {
        //                        SessionHelper.SetSessionCompleted = false;
        //                        string strRoomList = string.Empty;

        //                        //List<UserRoleModuleDetailsDTO> lstUserRoleModuleDetailsDTO = objUserMasterDAL.GetUserRoleModuleDetailsRecord(UserId, RoleId, UserType);
        //                        List<UserRoleModuleDetailsDTO> lstUserRoleModuleDetailsDTO = objUserMasterDAL.GetUserRoleModuleDetailsDefault(UserId, RoleId);
        //                        List<UserWiseRoomsAccessDetailsDTO> lstUserWiseRoomsAccessDetails = objUserMasterDAL.ConvertUserPermissions(lstUserRoleModuleDetailsDTO, RoleId, ref strRoomList);
        //                        List<UserAccessDTO> lstAccess = objUserMasterDAL.GetUserRoomAccessForSuperUserByUserIdPlain(UserId);
        //                        //List<UserAccessDTO> lstAccess = objEnterpriseDAL.GetUserAccessWithNames(UserId);



        //                        #region set single enterprise,company,room
        //                        //List<UserAccessDTO> lstAccess = new List<UserAccessDTO>();
        //                        //if (lstUserRoleModuleDetailsDTO != null && lstUserRoleModuleDetailsDTO.Any() && lstUserRoleModuleDetailsDTO.Count > 0)
        //                        //{
        //                        //    lstAccess = GetUserAccessFromUserRoleModule(lstUserRoleModuleDetailsDTO[0]);
        //                        //}
        //                        #endregion

        //                        if (lstAccess != null && lstAccess.Count > 0)
        //                        {
        //                            SessionHelper.RoomPermissions = lstUserWiseRoomsAccessDetails;
        //                            //List<UserRoleModuleDetailsDTO> lstddd = lstUserWiseRoomsAccessDetails.Where(t => t.EnterpriseId == 18 && t.CompanyId == 1 && t.RoomID == 1).FirstOrDefault().PermissionList.Where(t => t.ModuleID == 77).ToList();
        //                            List<EnterpriseDTO> lstAllEnterPrises = (from itm in lstAccess.Where(t => t.EnterpriseId > 0)
        //                                                                     orderby itm.IsEnterpriseActive descending, itm.EnterpriseName ascending
        //                                                                     group itm by new { itm.EnterpriseId, itm.EnterpriseName, itm.IsEnterpriseActive } into gropedentrprs
        //                                                                     select new EnterpriseDTO
        //                                                                     {
        //                                                                         ID = gropedentrprs.Key.EnterpriseId,
        //                                                                         Name = gropedentrprs.Key.EnterpriseName,
        //                                                                         IsActive = gropedentrprs.Key.IsEnterpriseActive
        //                                                                     }).ToList();
        //                            if (lstAllEnterPrises != null && lstAllEnterPrises.Count() > 0)
        //                            {

        //                                //List<EnterpriseDTO> lstActiveEps = objUserMasterDAL.GetActiveEnterprises(lstAllEnterPrises.Select(t => t.ID).ToArray());

        //                                if (objUserLoginHistoryDTO != null && objUserLoginHistoryDTO.EnterpriseId > 0 && lstAllEnterPrises.Any(t => t.ID == objUserLoginHistoryDTO.EnterpriseId))
        //                                {
        //                                    objEnterpriseDTO = lstAllEnterPrises.First(t => t.ID == objUserLoginHistoryDTO.EnterpriseId);
        //                                    SessionHelper.EnterPriceID = objEnterpriseDTO.ID;
        //                                }
        //                                else
        //                                {
        //                                    SessionHelper.EnterPriceID = lstAllEnterPrises.First().ID;
        //                                }

        //                                SessionHelper.EnterPriseList = lstAllEnterPrises;
        //                                objEnterpriseDTO = objEnterpriseDAL.GetEnterpriseByIdPlain(SessionHelper.EnterPriceID);
        //                                SessionHelper.EnterPriceName = objEnterpriseDTO.Name;
        //                                SessionHelper.EnterpriseLogoUrl = objEnterpriseDTO.EnterpriseLogo;
        //                                SessionHelper.EnterPriseDBName = objEnterpriseDTO.EnterpriseDBName;
        //                                //SessionHelper.EnterPriseDBConnectionString = objEnterpriseDTO.EnterpriseDBConnectionString;
        //                                List<CompanyMasterDTO> lstAllCompanies = (from itm in lstAccess.Where(t => t.CompanyId > 0)
        //                                                                          orderby itm.IsCompanyActive descending, itm.CompanyName ascending
        //                                                                          group itm by new { itm.EnterpriseId, itm.EnterpriseName, itm.CompanyId, itm.CompanyName, itm.IsCompanyActive } into gropedentrprs
        //                                                                          select new CompanyMasterDTO
        //                                                                          {
        //                                                                              EnterPriseId = gropedentrprs.Key.EnterpriseId,
        //                                                                              EnterPriseName = gropedentrprs.Key.EnterpriseName,
        //                                                                              ID = gropedentrprs.Key.CompanyId,
        //                                                                              Name = gropedentrprs.Key.CompanyName,
        //                                                                              IsActive = gropedentrprs.Key.IsCompanyActive
        //                                                                          }).ToList();
        //                                SessionHelper.CompanyList = lstAllCompanies;
        //                                if (lstAllCompanies != null && lstAllCompanies.Count(t => t.EnterPriseId == SessionHelper.EnterPriceID) > 0)
        //                                {
        //                                    if (objUserLoginHistoryDTO != null && objUserLoginHistoryDTO.EnterpriseId > 0 && lstAllEnterPrises.Any(t => t.ID == objUserLoginHistoryDTO.EnterpriseId))
        //                                    {
        //                                        objCompanyMasterDTO = lstAllCompanies.First(t => t.EnterPriseId == SessionHelper.EnterPriceID && t.ID == objUserLoginHistoryDTO.CompanyId);
        //                                        SessionHelper.CompanyID = objCompanyMasterDTO.ID;
        //                                        SessionHelper.CompanyLogoUrl = objCompanyMasterDTO.CompanyLogo;
        //                                        SessionHelper.CompanyName = objCompanyMasterDTO.Name;
        //                                        //SessionHelper.CompanyID = lstAllCompanies.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).OrderBy(t => t.Name).First().ID;
        //                                        //SessionHelper.CompanyName = lstAllCompanies.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).OrderBy(t => t.Name).First().Name;
        //                                        //SessionHelper.CompanyLogoUrl = lstAllCompanies.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).OrderBy(t => t.Name).First().CompanyLogo;
        //                                    }
        //                                    else
        //                                    {
        //                                        SessionHelper.CompanyID = lstAllCompanies.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).OrderBy(t => t.Name).First().ID;
        //                                        SessionHelper.CompanyName = lstAllCompanies.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).OrderBy(t => t.Name).First().Name;
        //                                        SessionHelper.CompanyLogoUrl = lstAllCompanies.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).OrderBy(t => t.Name).First().CompanyLogo;
        //                                    }
        //                                    objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
        //                                    List<RoomDTO> lstAllRooms = new List<RoomDTO>();

        //                                    //if (RoleId > 0)
        //                                    //{
        //                                    //    lstAccess.ForEach(z => z.UserId = UserId);
        //                                    //    EnterpriseMasterDAL enterPriseMasterDAL = new EnterpriseMasterDAL();
        //                                    //    var enterpriseIds = lstAccess.Select(e => e.EnterpriseId).Distinct().ToList();
        //                                    //    Dictionary<long, string> enterpriseList = enterPriseMasterDAL.GetEnterpriseListWithDBName(enterpriseIds);

        //                                    //    if (enterpriseList != null && enterpriseList.Any())
        //                                    //    {
        //                                    //        lstAllRooms = objRoomDAL.GetRoomsByUserAccessAndRoomSupplierFilterForSuperAdmin(lstAccess.Where(e => e.RoomId > 0).ToList(), enterpriseList);
        //                                    //    }
        //                                    //}
        //                                    //else
        //                                    //{
        //                                    lstAllRooms = (from itm in lstAccess.Where(t => t.RoomId > 0)
        //                                                   orderby itm.IsRoomActive descending, itm.RoomName ascending
        //                                                   group itm by new { itm.EnterpriseId, itm.EnterpriseName, itm.CompanyId, itm.CompanyName, itm.RoomId, itm.RoomName, itm.IsRoomActive, itm.isEVMI } into gropedentrprs
        //                                                   select new RoomDTO
        //                                                   {
        //                                                       EnterpriseId = gropedentrprs.Key.EnterpriseId,
        //                                                       EnterpriseName = gropedentrprs.Key.EnterpriseName,
        //                                                       CompanyID = gropedentrprs.Key.CompanyId,
        //                                                       CompanyName = gropedentrprs.Key.CompanyName,
        //                                                       ID = gropedentrprs.Key.RoomId,
        //                                                       RoomName = gropedentrprs.Key.RoomName,
        //                                                       IsRoomActive = gropedentrprs.Key.IsRoomActive,
        //                                                       IseVMI = gropedentrprs.Key.isEVMI
        //                                                   }).ToList();
        //                                    //}


        //                                    SessionHelper.RoomList = lstAllRooms;
        //                                    if (lstAllRooms != null && lstAllRooms.Count(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID) > 0)
        //                                    {
        //                                        if (objUserLoginHistoryDTO != null && objUserLoginHistoryDTO.RoomId > 0 && lstAllRooms.Any(t => t.EnterpriseId == objUserLoginHistoryDTO.EnterpriseId && t.CompanyID == objUserLoginHistoryDTO.CompanyId && t.ID == objUserLoginHistoryDTO.RoomId))
        //                                        {
        //                                            objRoomDTO = lstAllRooms.First(t => t.EnterpriseId == objUserLoginHistoryDTO.EnterpriseId && t.CompanyID == objUserLoginHistoryDTO.CompanyId && t.ID == objUserLoginHistoryDTO.RoomId);
        //                                            SessionHelper.RoomID = objRoomDTO.ID;
        //                                            SessionHelper.RoomName = objRoomDTO.RoomName;
        //                                            //SessionHelper.isEVMI = objRoomDTO.IseVMI;
        //                                        }
        //                                        else
        //                                        {
        //                                            SessionHelper.RoomID = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().ID;
        //                                            SessionHelper.RoomName = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().RoomName;
        //                                            //SessionHelper.isEVMI = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().IseVMI;
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        SessionHelper.RoomID = 0;
        //                                        SessionHelper.RoomName = string.Empty;
        //                                        //SessionHelper.isEVMI = null;
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    SessionHelper.CompanyID = 0;
        //                                    SessionHelper.CompanyName = string.Empty;
        //                                    SessionHelper.RoomID = 0;
        //                                    SessionHelper.RoomName = string.Empty;
        //                                    //SessionHelper.isEVMI = null;
        //                                }
        //                            }
        //                            else
        //                            {
        //                                SessionHelper.EnterPriceID = 0;
        //                                SessionHelper.EnterPriceName = string.Empty;
        //                                SessionHelper.CompanyID = 0;
        //                                SessionHelper.CompanyName = string.Empty;
        //                                SessionHelper.RoomID = 0;
        //                                SessionHelper.RoomName = string.Empty;
        //                                SessionHelper.EnterPriseList = new List<EnterpriseDTO>();
        //                                SessionHelper.CompanyList = new List<CompanyMasterDTO>();
        //                                SessionHelper.RoomList = new List<RoomDTO>();
        //                                //SessionHelper.isEVMI = null;
        //                            }
        //                        }
        //                    }

        //                    if (!objUserMasterDAL.IsSAdminUserExist(SessionHelper.UserID, SessionHelper.EnterPriceID))
        //                    {
        //                        UserMasterDTO objUserMasterDTO = new UserMasterDTO();
        //                        objUserMasterDTO.CompanyID = 0;
        //                        objUserMasterDTO.Created = DateTimeUtility.DateTimeNow;
        //                        objUserMasterDTO.CreatedBy = SessionHelper.UserID;
        //                        objUserMasterDTO.CreatedByName = SessionHelper.UserName;
        //                        objUserMasterDTO.Email = SessionHelper.UserName;
        //                        objUserMasterDTO.EnterpriseDbName = string.Empty;
        //                        objUserMasterDTO.EnterpriseId = 0;
        //                        objUserMasterDTO.GUID = Guid.NewGuid();
        //                        objUserMasterDTO.IsArchived = false;
        //                        objUserMasterDTO.IsDeleted = false;
        //                        objUserMasterDTO.LastUpdatedBy = SessionHelper.UserID;
        //                        objUserMasterDTO.Password = "password";
        //                        objUserMasterDTO.Phone = "[!!AdminPbone!!]";
        //                        objUserMasterDTO.RoleID = SessionHelper.RoleID;
        //                        objUserMasterDTO.Updated = DateTimeUtility.DateTimeNow;
        //                        objUserMasterDTO.UserType = SessionHelper.UserType;
        //                        objUserMasterDTO.UserName = SessionHelper.UserName;
        //                        objUserMasterDAL.InsertSAdminUserInChildDB(SessionHelper.UserID, objUserMasterDTO, SessionHelper.EnterPriceID);
        //                    }
        //                    break;
        //                case 2:
        //                    if (RoleId == -2)
        //                    {
        //                        SessionHelper.EnterPriceID = EnterpriseId;
        //                        objEnterpriseDTO = objEnterpriseDAL.GetEnterpriseByIdPlain(SessionHelper.EnterPriceID);
        //                        SessionHelper.EnterpriseLogoUrl = objEnterpriseDTO.EnterpriseLogo;
        //                        SessionHelper.EnterPriceName = objEnterpriseDTO.Name;
        //                        SessionHelper.EnterPriseDBName = objEnterpriseDTO.EnterpriseDBName;
        //                        //SessionHelper.EnterPriseDBConnectionString = objEnterpriseDTO.EnterpriseDBConnectionString;
        //                        List<EnterpriseDTO> lstEnterprise = new List<EnterpriseDTO>();
        //                        lstEnterprise.Add(objEnterpriseDTO);
        //                        SessionHelper.EnterPriseList = lstEnterprise;
        //                        objCompanyMasterDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
        //                        List<CompanyMasterDTO> lstAllCompanies = (from cm in objCompanyMasterDAL.GetAllCompanies()
        //                                                                  orderby cm.IsActive descending, cm.Name ascending
        //                                                                  select cm).ToList();
        //                        //List<CompanyMasterDTO> lstAllCompanies = objCompanyMasterDAL.GetAllRecords().OrderBy(t => t.Name).ToList();
        //                        if (lstAllCompanies != null && lstAllCompanies.Count() > 0)
        //                        {
        //                            lstAllCompanies.ForEach(t =>
        //                            {
        //                                t.EnterPriseId = SessionHelper.EnterPriceID;
        //                                t.EnterPriseName = SessionHelper.EnterPriceName;
        //                            });
        //                            if (objUserLoginHistoryDTO != null && objUserLoginHistoryDTO.CompanyId > 0 && lstAllCompanies.Any(t => t.EnterPriseId == SessionHelper.EnterPriceID && t.ID == objUserLoginHistoryDTO.CompanyId))
        //                            {
        //                                objCompanyMasterDTO = lstAllCompanies.First(t => t.EnterPriseId == SessionHelper.EnterPriceID && t.ID == objUserLoginHistoryDTO.CompanyId);
        //                                SessionHelper.CompanyID = objCompanyMasterDTO.ID;
        //                                SessionHelper.CompanyLogoUrl = objCompanyMasterDTO.CompanyLogo;
        //                                SessionHelper.CompanyName = objCompanyMasterDTO.Name;
        //                            }
        //                            else
        //                            {
        //                                SessionHelper.CompanyID = lstAllCompanies.First().ID;
        //                                SessionHelper.CompanyName = lstAllCompanies.First().Name;
        //                                SessionHelper.CompanyLogoUrl = lstAllCompanies.First().CompanyLogo;
        //                            }
        //                            SessionHelper.CompanyList = lstAllCompanies;
        //                            objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
        //                            //List<RoomDTO> lstRooms = objRoomDAL.GetAllRoomByCompany(SessionHelper.CompanyID, false, false).OrderBy(t => t.RoomName).ToList();
        //                            List<RoomDTO> lstRooms = objRoomDAL.GetRoomByCompanyIDPlain(SessionHelper.CompanyID).OrderByDescending(x => x.IsRoomActive).ThenBy(r => r.RoomName).ToList();
        //                            if (lstRooms != null && lstRooms.Count > 0)
        //                            {
        //                                lstRooms.ForEach(t =>
        //                                {
        //                                    t.EnterpriseId = SessionHelper.EnterPriceID;
        //                                    t.EnterpriseName = SessionHelper.EnterPriceName;
        //                                });
        //                                if (objUserLoginHistoryDTO != null && objUserLoginHistoryDTO.RoomId > 0 && lstRooms.Any(t => t.EnterpriseId == objUserLoginHistoryDTO.EnterpriseId && t.CompanyID == objUserLoginHistoryDTO.CompanyId && t.ID == objUserLoginHistoryDTO.RoomId))
        //                                {
        //                                    objRoomDTO = lstRooms.First(t => t.EnterpriseId == objUserLoginHistoryDTO.EnterpriseId && t.CompanyID == objUserLoginHistoryDTO.CompanyId && t.ID == objUserLoginHistoryDTO.RoomId);
        //                                    SessionHelper.RoomID = objRoomDTO.ID;
        //                                    SessionHelper.RoomName = objRoomDTO.RoomName;
        //                                    //SessionHelper.isEVMI = objRoomDTO.IseVMI;
        //                                }
        //                                else
        //                                {
        //                                    SessionHelper.RoomID = lstRooms.First().ID;
        //                                    SessionHelper.RoomName = lstRooms.First().RoomName;
        //                                    //SessionHelper.isEVMI = lstRooms.First().IseVMI;
        //                                }
        //                                SessionHelper.RoomList = lstRooms;
        //                            }
        //                            else
        //                            {
        //                                SessionHelper.RoomID = 0;
        //                                SessionHelper.RoomName = string.Empty;
        //                                SessionHelper.RoomList = new List<RoomDTO>();
        //                                // SessionHelper.isEVMI = null;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            SessionHelper.CompanyID = 0;
        //                            SessionHelper.CompanyLogoUrl = string.Empty;
        //                            SessionHelper.CompanyName = string.Empty;
        //                            SessionHelper.CompanyList = new List<CompanyMasterDTO>();
        //                            SessionHelper.RoomID = 0;
        //                            SessionHelper.RoomName = string.Empty;
        //                            SessionHelper.RoomList = new List<RoomDTO>();
        //                            //SessionHelper.isEVMI = null;
        //                        }
        //                        objinterUserDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
        //                        List<UserWiseRoomsAccessDetailsDTO> lstUserWiseRoomsAccessDetailsDTO = objinterUserDAL.GetEnterpriseAdminRoomPermissions(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.RoleID, SessionHelper.UserID);
        //                        SessionHelper.RoomPermissions = SetPermissionsForsuperadmin(lstUserWiseRoomsAccessDetailsDTO, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.RoleID, SessionHelper.UserType);
        //                    }
        //                    else
        //                    {
        //                        SessionHelper.EnterPriceID = EnterpriseId;
        //                        objEnterpriseDTO = objEnterpriseDAL.GetEnterpriseByIdPlain(SessionHelper.EnterPriceID);
        //                        SessionHelper.EnterpriseLogoUrl = objEnterpriseDTO.EnterpriseLogo;
        //                        SessionHelper.EnterPriseDBName = objEnterpriseDTO.EnterpriseDBName;
        //                        //SessionHelper.EnterPriseDBConnectionString = objEnterpriseDTO.EnterpriseDBConnectionString;
        //                        List<EnterpriseDTO> lstEnterprise = new List<EnterpriseDTO>();
        //                        lstEnterprise.Add(objEnterpriseDTO);
        //                        SessionHelper.EnterPriseList = lstEnterprise;
        //                        string strRoomList = string.Empty;
        //                        objinterUserDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
        //                        List<UserRoleModuleDetailsDTO> lstUserRoleModuleDetailsDTO = objinterUserDAL.GetUserRoleModuleDetailsByUserIdAndRoleId(UserId, RoleId);
        //                        List<UserWiseRoomsAccessDetailsDTO> lstUserWiseRoomsAccessDetails = objinterUserDAL.ConvertUserPermissions(lstUserRoleModuleDetailsDTO, RoleId, ref strRoomList);
        //                        List<UserAccessDTO> lstAccess = objinterUserDAL.GetUserAccessWithNames(UserId, objEnterpriseDTO.Name, objEnterpriseDTO.IsActive);
        //                        if (lstAccess != null && lstAccess.Count > 0)
        //                        {
        //                            SessionHelper.RoomPermissions = lstUserWiseRoomsAccessDetails;
        //                            //List<CompanyMasterDTO> lstAllCompanies = (from itm in lstUserWiseRoomsAccessDetails
        //                            //                                          group itm by new { itm.EnterpriseId, itm.EnterpriseName, itm.CompanyId, itm.CompanyName } into gropedentrprs
        //                            //                                          select new CompanyMasterDTO
        //                            //                                          {
        //                            //                                              EnterPriseId = gropedentrprs.Key.EnterpriseId,
        //                            //                                              EnterPriseName = gropedentrprs.Key.EnterpriseName,
        //                            //                                              ID = gropedentrprs.Key.CompanyId,
        //                            //                                              Name = gropedentrprs.Key.CompanyName
        //                            //                                          }).OrderBy(t => t.Name).ToList();
        //                            List<CompanyMasterDTO> lstAllCompanies = (from itm in lstAccess.Where(t => t.CompanyId > 0)
        //                                                                      orderby itm.IsCompanyActive descending, itm.CompanyName ascending
        //                                                                      group itm by new { itm.EnterpriseId, itm.EnterpriseName, itm.CompanyId, itm.CompanyName, itm.CompanyLogo, itm.IsCompanyActive } into gropedentrprs
        //                                                                      select new CompanyMasterDTO
        //                                                                      {
        //                                                                          EnterPriseId = gropedentrprs.Key.EnterpriseId,
        //                                                                          EnterPriseName = gropedentrprs.Key.EnterpriseName,
        //                                                                          ID = gropedentrprs.Key.CompanyId,
        //                                                                          Name = gropedentrprs.Key.CompanyName,
        //                                                                          IsActive = gropedentrprs.Key.IsCompanyActive,
        //                                                                          CompanyLogo = gropedentrprs.Key.CompanyLogo
        //                                                                      }).ToList();
        //                            SessionHelper.CompanyList = lstAllCompanies;
        //                            if (lstAllCompanies != null && lstAllCompanies.Count(t => t.EnterPriseId == SessionHelper.EnterPriceID) > 0)
        //                            {
        //                                if (objUserLoginHistoryDTO != null && objUserLoginHistoryDTO.CompanyId > 0 && lstAllCompanies.Any(t => t.ID == objUserLoginHistoryDTO.CompanyId))
        //                                {
        //                                    objCompanyMasterDTO = lstAllCompanies.First(t => t.ID == objUserLoginHistoryDTO.CompanyId);
        //                                    SessionHelper.CompanyID = objCompanyMasterDTO.ID;
        //                                    SessionHelper.CompanyName = objCompanyMasterDTO.Name;
        //                                    SessionHelper.CompanyLogoUrl = objCompanyMasterDTO.CompanyLogo;
        //                                }
        //                                else
        //                                {
        //                                    SessionHelper.CompanyID = lstAllCompanies.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).OrderBy(t => t.Name).First().ID;
        //                                    SessionHelper.CompanyName = lstAllCompanies.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).OrderBy(t => t.Name).First().Name;
        //                                    SessionHelper.CompanyLogoUrl = lstAllCompanies.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).OrderBy(t => t.Name).First().CompanyLogo;
        //                                }

        //                                objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
        //                                List<RoomDTO> lstAllRooms = new List<RoomDTO>();

        //                                if (RoleId > 0)
        //                                {
        //                                    lstAllRooms = objRoomDAL.GetRoomsByUserAccessAndRoomSupplierFilter(lstAccess);
        //                                }
        //                                else
        //                                {
        //                                    lstAllRooms = (from itm in lstAccess.Where(t => t.RoomId > 0)
        //                                                   orderby itm.IsRoomActive descending, itm.RoomName ascending
        //                                                   group itm by new { itm.EnterpriseId, itm.EnterpriseName, itm.CompanyId, itm.CompanyName, itm.RoomId, itm.RoomName, itm.IsRoomActive } into gropedentrprs
        //                                                   select new RoomDTO
        //                                                   {
        //                                                       EnterpriseId = gropedentrprs.Key.EnterpriseId,
        //                                                       EnterpriseName = gropedentrprs.Key.EnterpriseName,
        //                                                       CompanyID = gropedentrprs.Key.CompanyId,
        //                                                       CompanyName = gropedentrprs.Key.CompanyName,
        //                                                       ID = gropedentrprs.Key.RoomId,
        //                                                       RoomName = gropedentrprs.Key.RoomName,
        //                                                       IsRoomActive = gropedentrprs.Key.IsRoomActive
        //                                                   }).ToList();
        //                                }


        //                                SessionHelper.RoomList = lstAllRooms;
        //                                if (lstAllRooms != null && lstAllRooms.Count(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID) > 0)
        //                                {
        //                                    if (objUserLoginHistoryDTO != null && objUserLoginHistoryDTO.RoomId > 0 && lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID && t.ID == objUserLoginHistoryDTO.RoomId).Any())
        //                                    {
        //                                        SessionHelper.RoomID = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID && t.ID == objUserLoginHistoryDTO.RoomId).OrderBy(t => t.RoomName).First().ID;
        //                                        SessionHelper.RoomName = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID && t.ID == objUserLoginHistoryDTO.RoomId).OrderBy(t => t.RoomName).First().RoomName;
        //                                        //SessionHelper.isEVMI = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID && t.ID == objUserLoginHistoryDTO.RoomId).OrderBy(t => t.RoomName).First().IseVMI;
        //                                    }
        //                                    else
        //                                    {
        //                                        SessionHelper.RoomID = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().ID;
        //                                        SessionHelper.RoomName = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().RoomName;
        //                                        //SessionHelper.isEVMI = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().IseVMI;
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    SessionHelper.RoomID = 0;
        //                                    SessionHelper.RoomName = string.Empty;
        //                                    //SessionHelper.isEVMI = null;
        //                                }
        //                            }
        //                        }

        //                    }
        //                    break;
        //                case 3:
        //                    SessionHelper.EnterPriceID = EnterpriseId;
        //                    objEnterpriseDTO = objEnterpriseDAL.GetEnterpriseByIdPlain(SessionHelper.EnterPriceID);
        //                    SessionHelper.EnterpriseLogoUrl = objEnterpriseDTO.EnterpriseLogo;
        //                    SessionHelper.EnterPriseDBName = objEnterpriseDTO.EnterpriseDBName;
        //                    //SessionHelper.EnterPriseDBConnectionString = objEnterpriseDTO.EnterpriseDBConnectionString;
        //                    List<EnterpriseDTO> lstEnterprise3 = new List<EnterpriseDTO>();
        //                    lstEnterprise3.Add(objEnterpriseDTO);
        //                    SessionHelper.EnterPriseList = lstEnterprise3;
        //                    SessionHelper.CompanyID = CompanyId;
        //                    string strRoomList3 = string.Empty;
        //                    objinterUserDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
        //                    List<UserRoleModuleDetailsDTO> lstUserRoleModuleDetailsDTO3 = objinterUserDAL.GetUserRoleModuleDetailsByUserIdAndRoleId(UserId, RoleId);
        //                    List<UserWiseRoomsAccessDetailsDTO> lstUserWiseRoomsAccessDetails3 = objinterUserDAL.ConvertUserPermissions(lstUserRoleModuleDetailsDTO3, RoleId, ref strRoomList3);
        //                    List<UserAccessDTO> lstAccess1 = objinterUserDAL.GetUserAccessWithNames(UserId, objEnterpriseDTO.Name, objEnterpriseDTO.IsActive);
        //                    if (lstAccess1 != null && lstAccess1.Count > 0)
        //                    {
        //                        SessionHelper.RoomPermissions = lstUserWiseRoomsAccessDetails3;
        //                        List<CompanyMasterDTO> lstAllCompanies = (from itm in lstAccess1
        //                                                                  orderby itm.IsCompanyActive descending, itm.CompanyName ascending
        //                                                                  group itm by new { itm.EnterpriseId, itm.EnterpriseName, itm.CompanyId, itm.CompanyName, itm.CompanyLogo, itm.IsCompanyActive } into gropedentrprs
        //                                                                  select new CompanyMasterDTO
        //                                                                  {
        //                                                                      EnterPriseId = gropedentrprs.Key.EnterpriseId,
        //                                                                      EnterPriseName = gropedentrprs.Key.EnterpriseName,
        //                                                                      ID = gropedentrprs.Key.CompanyId,
        //                                                                      Name = gropedentrprs.Key.CompanyName,
        //                                                                      CompanyLogo = gropedentrprs.Key.CompanyLogo,
        //                                                                      IsActive = gropedentrprs.Key.IsCompanyActive
        //                                                                  }).ToList();
        //                        SessionHelper.CompanyList = lstAllCompanies;
        //                        if (lstAllCompanies != null && lstAllCompanies.Count(t => t.EnterPriseId == SessionHelper.EnterPriceID) > 0)
        //                        {
        //                            SessionHelper.CompanyID = lstAllCompanies.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).OrderBy(t => t.Name).First().ID;
        //                            SessionHelper.CompanyName = lstAllCompanies.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).OrderBy(t => t.Name).First().Name;
        //                            SessionHelper.CompanyLogoUrl = lstAllCompanies.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).OrderBy(t => t.Name).First().CompanyLogo;

        //                            objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
        //                            List<RoomDTO> lstAllRooms = new List<RoomDTO>();

        //                            if (RoleId > 0)
        //                            {
        //                                lstAllRooms = objRoomDAL.GetRoomsByUserAccessAndRoomSupplierFilter(lstAccess1);
        //                            }
        //                            else
        //                            {
        //                                lstAllRooms = (from itm in lstAccess1
        //                                               orderby itm.IsRoomActive descending, itm.RoomName ascending
        //                                               group itm by new { itm.EnterpriseId, itm.EnterpriseName, itm.CompanyId, itm.CompanyName, itm.RoomId, itm.RoomName, itm.IsRoomActive, itm.isEVMI } into gropedentrprs
        //                                               select new RoomDTO
        //                                               {
        //                                                   EnterpriseId = gropedentrprs.Key.EnterpriseId,
        //                                                   EnterpriseName = gropedentrprs.Key.EnterpriseName,
        //                                                   CompanyID = gropedentrprs.Key.CompanyId,
        //                                                   CompanyName = gropedentrprs.Key.CompanyName,
        //                                                   ID = gropedentrprs.Key.RoomId,
        //                                                   RoomName = gropedentrprs.Key.RoomName,
        //                                                   IsRoomActive = gropedentrprs.Key.IsRoomActive,
        //                                                   IseVMI = gropedentrprs.Key.isEVMI
        //                                               }).ToList();
        //                            }

        //                            SessionHelper.RoomList = lstAllRooms;
        //                            if (lstAllRooms != null && lstAllRooms.Count(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID) > 0)
        //                            {
        //                                if (objUserLoginHistoryDTO != null && objUserLoginHistoryDTO.RoomId > 0 && lstAllRooms.Any(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID && t.ID == objUserLoginHistoryDTO.RoomId))
        //                                {
        //                                    SessionHelper.RoomID = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID && t.ID == objUserLoginHistoryDTO.RoomId).First().ID;
        //                                    SessionHelper.RoomName = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID && t.ID == objUserLoginHistoryDTO.RoomId).First().RoomName;
        //                                    // SessionHelper.isEVMI = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID && t.ID == objUserLoginHistoryDTO.RoomId).First().IseVMI;
        //                                }
        //                                else
        //                                {
        //                                    SessionHelper.RoomID = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().ID;
        //                                    SessionHelper.RoomName = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().RoomName;
        //                                    //SessionHelper.isEVMI = lstAllRooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().IseVMI;
        //                                }
        //                            }
        //                        }
        //                    }

        //                    break;
        //                default:
        //                    break;
        //            }
        //            break;
        //        case "enterprise":
        //            switch (UserType)
        //            {
        //                case 1:
        //                    if (RoleId == -1)
        //                    {
        //                        SessionHelper.EnterPriceID = EnterpriseId;
        //                        objEnterpriseDTO = objEnterpriseDAL.GetEnterpriseByIdPlain(SessionHelper.EnterPriceID);
        //                        SessionHelper.EnterpriseLogoUrl = objEnterpriseDTO.EnterpriseLogo;
        //                        SessionHelper.EnterPriseDBName = objEnterpriseDTO.EnterpriseDBName;
        //                        //SessionHelper.EnterPriseDBConnectionString = objEnterpriseDTO.EnterpriseDBConnectionString;
        //                        objCompanyMasterDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
        //                        List<CompanyMasterDTO> lstAllCompanies = (from cm in objCompanyMasterDAL.GetAllCompanies()
        //                                                                  orderby cm.IsActive descending, cm.Name ascending
        //                                                                  select cm).ToList();
        //                        if (lstAllCompanies != null && lstAllCompanies.Count() > 0)
        //                        {
        //                            //List<CompanyMasterDTO> lstAllCompanies = objCompanyMasterDAL.GetAllRecords().OrderBy(t => t.Name).ToList();
        //                            //if (lstAllCompanies != null && lstAllCompanies.Count > 0)
        //                            //{
        //                            lstAllCompanies.ForEach(t =>
        //                            {
        //                                t.EnterPriseId = SessionHelper.EnterPriceID;
        //                                t.EnterPriseName = SessionHelper.EnterPriceName;
        //                            });
        //                            SessionHelper.CompanyID = lstAllCompanies.First().ID;
        //                            SessionHelper.CompanyName = lstAllCompanies.First().Name;
        //                            SessionHelper.CompanyLogoUrl = lstAllCompanies.First().CompanyLogo;
        //                            SessionHelper.CompanyList = lstAllCompanies;
        //                            //List<RoomDTO> lstRooms = objRoomDAL.GetAllRoomByCompany(SessionHelper.CompanyID, false, false).OrderBy(t => t.RoomName).ToList();
        //                            //if (lstRooms != null && lstRooms.Count > 0)
        //                            //{ 
        //                            objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
        //                            List<RoomDTO> lstRooms = objRoomDAL.GetRoomByCompanyIDPlain(SessionHelper.CompanyID).OrderByDescending(x => x.IsRoomActive).ThenBy(r => r.RoomName).ToList();

        //                            if (lstRooms != null && lstRooms.Count() > 0)
        //                            {
        //                                lstRooms.ForEach(t =>
        //                                {
        //                                    t.EnterpriseId = SessionHelper.EnterPriceID;
        //                                    t.EnterpriseName = SessionHelper.EnterPriceName;
        //                                });
        //                                SessionHelper.RoomID = lstRooms.First().ID;
        //                                SessionHelper.RoomName = lstRooms.First().RoomName;
        //                                //SessionHelper.isEVMI = lstRooms.First().IseVMI;
        //                                SessionHelper.RoomList = lstRooms;
        //                            }
        //                            else
        //                            {
        //                                SessionHelper.RoomID = 0;
        //                                SessionHelper.RoomName = string.Empty;
        //                                //SessionHelper.isEVMI = null;
        //                                SessionHelper.RoomList = new List<RoomDTO>();
        //                            }
        //                        }
        //                        else
        //                        {
        //                            SessionHelper.CompanyID = 0;
        //                            SessionHelper.CompanyName = string.Empty;
        //                            SessionHelper.CompanyLogoUrl = string.Empty;
        //                            SessionHelper.CompanyList = new List<CompanyMasterDTO>();
        //                            SessionHelper.RoomID = 0;
        //                            SessionHelper.RoomName = string.Empty;
        //                            //SessionHelper.isEVMI = null;
        //                            SessionHelper.RoomList = new List<RoomDTO>();
        //                        }
        //                        List<UserWiseRoomsAccessDetailsDTO> lstpermissionfromsession = SessionHelper.RoomPermissions;
        //                        lstpermissionfromsession.ForEach(t =>
        //                        {
        //                            t.EnterpriseId = SessionHelper.EnterPriceID;
        //                            t.CompanyId = SessionHelper.CompanyID;
        //                            t.RoomID = SessionHelper.RoomID;
        //                            t.PermissionList.ForEach(et =>
        //                            {
        //                                et.EnteriseId = SessionHelper.EnterPriceID;
        //                                et.CompanyId = SessionHelper.CompanyID;
        //                                et.RoomId = SessionHelper.RoomID;
        //                            });
        //                        });
        //                        SessionHelper.RoomPermissions = SetPermissionsForsuperadmin(lstpermissionfromsession, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.RoleID, SessionHelper.UserType);
        //                    }
        //                    else
        //                    {
        //                        SessionHelper.EnterPriceID = EnterpriseId;
        //                        objEnterpriseDTO = objEnterpriseDAL.GetEnterpriseByIdPlain(SessionHelper.EnterPriceID);
        //                        SessionHelper.EnterpriseLogoUrl = objEnterpriseDTO.EnterpriseLogo;
        //                        SessionHelper.EnterPriseDBName = objEnterpriseDTO.EnterpriseDBName;
        //                        //SessionHelper.EnterPriseDBConnectionString = objEnterpriseDTO.EnterpriseDBConnectionString;
        //                        SessionHelper.EnterPriceName = objEnterpriseDTO.Name;

        //                        if (SessionHelper.CompanyList != null && SessionHelper.CompanyList.Any(t => t.EnterPriseId == SessionHelper.EnterPriceID))
        //                        {
        //                            SessionHelper.CompanyID = SessionHelper.CompanyList.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).OrderBy(t => t.Name).First().ID;
        //                            SessionHelper.CompanyName = SessionHelper.CompanyList.Where(t => t.EnterPriseId == SessionHelper.EnterPriceID).OrderBy(t => t.Name).First().Name;
        //                            if (SessionHelper.RoomList == null)
        //                            {

        //                                objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
        //                                List<RoomDTO> lstAllRooms = new List<RoomDTO>();
        //                                List<UserAccessDTO> lstAccess = objEnterpriseDAL.GetUserAccessWithNames(UserId);
        //                                if (lstAccess != null && lstAccess.Count > 0)
        //                                {
        //                                    if (RoleId > 0)
        //                                    {
        //                                        lstAccess.ForEach(z => z.UserId = UserId);
        //                                        EnterpriseMasterDAL enterPriseMasterDAL = new EnterpriseMasterDAL();
        //                                        var enterpriseIds = lstAccess.Select(e => e.EnterpriseId).Distinct().ToList();
        //                                        Dictionary<long, string> enterpriseList = enterPriseMasterDAL.GetEnterpriseListWithDBName(enterpriseIds);

        //                                        if (enterpriseList != null && enterpriseList.Any())
        //                                        {
        //                                            lstAllRooms = objRoomDAL.GetRoomsByUserAccessAndRoomSupplierFilterForSuperAdmin(lstAccess.Where(e => e.RoomId > 0).ToList(), enterpriseList);
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        lstAllRooms = (from itm in lstAccess.Where(t => t.RoomId > 0)
        //                                                       orderby itm.IsRoomActive descending, itm.RoomName ascending
        //                                                       group itm by new { itm.EnterpriseId, itm.EnterpriseName, itm.CompanyId, itm.CompanyName, itm.RoomId, itm.RoomName, itm.IsRoomActive, itm.isEVMI } into gropedentrprs
        //                                                       select new RoomDTO
        //                                                       {
        //                                                           EnterpriseId = gropedentrprs.Key.EnterpriseId,
        //                                                           EnterpriseName = gropedentrprs.Key.EnterpriseName,
        //                                                           CompanyID = gropedentrprs.Key.CompanyId,
        //                                                           CompanyName = gropedentrprs.Key.CompanyName,
        //                                                           ID = gropedentrprs.Key.RoomId,
        //                                                           RoomName = gropedentrprs.Key.RoomName,
        //                                                           IsRoomActive = gropedentrprs.Key.IsRoomActive,
        //                                                           IseVMI = gropedentrprs.Key.isEVMI
        //                                                       }).ToList();
        //                                    }
        //                                    SessionHelper.RoomList = lstAllRooms;

        //                                }

        //                            }
        //                            if (SessionHelper.RoomList != null && SessionHelper.RoomList.Any(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID))
        //                            {
        //                                SessionHelper.RoomID = SessionHelper.RoomList.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().ID;
        //                                SessionHelper.RoomName = SessionHelper.RoomList.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().RoomName;
        //                                //SessionHelper.isEVMI = SessionHelper.RoomList.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().IseVMI;
        //                            }
        //                            else
        //                            {
        //                                SessionHelper.RoomID = 0;
        //                                SessionHelper.RoomName = string.Empty;
        //                                //SessionHelper.isEVMI = null;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            SessionHelper.CompanyID = 0;
        //                            SessionHelper.CompanyName = string.Empty;
        //                            SessionHelper.RoomID = 0;
        //                            SessionHelper.RoomName = string.Empty;
        //                            //SessionHelper.isEVMI = null;
        //                        }

        //                    }
        //                    break;
        //            }
        //            if (!objUserMasterDAL.IsSAdminUserExist(SessionHelper.UserID, SessionHelper.EnterPriceID))
        //            {
        //                UserMasterDTO objUserMasterDTO = new UserMasterDTO();
        //                objUserMasterDTO.CompanyID = 0;
        //                objUserMasterDTO.Created = DateTimeUtility.DateTimeNow;
        //                objUserMasterDTO.CreatedBy = SessionHelper.UserID;
        //                objUserMasterDTO.CreatedByName = SessionHelper.UserName;
        //                objUserMasterDTO.Email = SessionHelper.UserName;
        //                objUserMasterDTO.EnterpriseDbName = string.Empty;
        //                objUserMasterDTO.EnterpriseId = 0;
        //                objUserMasterDTO.GUID = Guid.NewGuid();
        //                objUserMasterDTO.IsArchived = false;
        //                objUserMasterDTO.IsDeleted = false;
        //                objUserMasterDTO.LastUpdatedBy = SessionHelper.UserID;
        //                objUserMasterDTO.Password = "password";
        //                objUserMasterDTO.Phone = "[!!AdminPbone!!]";
        //                objUserMasterDTO.RoleID = SessionHelper.RoleID;
        //                objUserMasterDTO.Updated = DateTimeUtility.DateTimeNow;
        //                objUserMasterDTO.UserType = SessionHelper.UserType;
        //                objUserMasterDTO.UserName = SessionHelper.UserName;
        //                objUserMasterDAL.InsertSAdminUserInChildDB(SessionHelper.UserID, objUserMasterDTO, SessionHelper.EnterPriceID);
        //            }
        //            Session["ReportPara"] = null;
        //            break;
        //        case "company":
        //            switch (UserType)
        //            {
        //                case 1:
        //                    if (RoleId == -1)
        //                    {
        //                        SessionHelper.CompanyID = CompanyId;
        //                        SessionHelper.CompanyName = CompanyName;
        //                        objCompanyMasterDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
        //                        CompanyMasterDTO objCompanyMaster = objCompanyMasterDAL.GetCompanyByID(CompanyId);
        //                        SessionHelper.CompanyLogoUrl = objCompanyMaster.CompanyLogo;
        //                        objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
        //                        List<RoomDTO> lstRooms = objRoomDAL.GetRoomByCompanyIDPlain(SessionHelper.CompanyID).OrderByDescending(x => x.IsRoomActive).ThenBy(r => r.RoomName).ToList();

        //                        if (lstRooms != null && lstRooms.Count() > 0)
        //                        {
        //                            lstRooms.ForEach(t =>
        //                            {
        //                                t.EnterpriseId = SessionHelper.EnterPriceID;
        //                                t.EnterpriseName = SessionHelper.EnterPriceName;
        //                            });

        //                            SessionHelper.RoomID = lstRooms.First().ID;
        //                            SessionHelper.RoomName = lstRooms.First().RoomName;
        //                            //SessionHelper.isEVMI = lstRooms.First().IseVMI;
        //                            SessionHelper.RoomList = lstRooms;
        //                        }
        //                        else
        //                        {
        //                            SessionHelper.RoomID = 0;
        //                            SessionHelper.RoomName = string.Empty;
        //                            //SessionHelper.isEVMI = null;
        //                            SessionHelper.RoomList = new List<RoomDTO>();
        //                        }
        //                        List<UserWiseRoomsAccessDetailsDTO> lstpermissionfromsession = SessionHelper.RoomPermissions;
        //                        lstpermissionfromsession.ForEach(t =>
        //                        {
        //                            t.EnterpriseId = SessionHelper.EnterPriceID;
        //                            t.CompanyId = SessionHelper.CompanyID;
        //                            t.RoomID = SessionHelper.RoomID;
        //                            t.PermissionList.ForEach(et =>
        //                            {
        //                                et.EnteriseId = SessionHelper.EnterPriceID;
        //                                et.CompanyId = SessionHelper.CompanyID;
        //                                et.RoomId = SessionHelper.RoomID;
        //                            });
        //                        });
        //                        SessionHelper.RoomPermissions = SetPermissionsForsuperadmin(lstpermissionfromsession, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.RoleID, SessionHelper.UserType);
        //                    }
        //                    else
        //                    {
        //                        SessionHelper.CompanyID = CompanyId;
        //                        SessionHelper.CompanyName = CompanyName;
        //                        objCompanyMasterDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
        //                        CompanyMasterDTO objCompanyMaster = objCompanyMasterDAL.GetCompanyByID(CompanyId);
        //                        SessionHelper.CompanyLogoUrl = objCompanyMaster.CompanyLogo;
        //                        List<RoomDTO> lstAllrooms = SessionHelper.RoomList;
        //                        if (SessionHelper.RoomList != null && SessionHelper.RoomList.Any(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID))
        //                        {
        //                            SessionHelper.RoomID = SessionHelper.RoomList.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().ID;
        //                            SessionHelper.RoomName = SessionHelper.RoomList.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().RoomName;
        //                            //SessionHelper.isEVMI = SessionHelper.RoomList.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().IseVMI;
        //                        }
        //                        else
        //                        {
        //                            SessionHelper.RoomID = 0;
        //                            SessionHelper.RoomName = string.Empty;
        //                            //SessionHelper.isEVMI = null;
        //                        }
        //                    }
        //                    break;
        //                case 2:
        //                    if (RoleId == -2)
        //                    {
        //                        SessionHelper.CompanyID = CompanyId;
        //                        SessionHelper.CompanyName = CompanyName;
        //                        objCompanyMasterDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
        //                        CompanyMasterDTO objCompanyMaster = objCompanyMasterDAL.GetCompanyByID(CompanyId);
        //                        SessionHelper.CompanyLogoUrl = objCompanyMaster.CompanyLogo;
        //                        objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
        //                        List<RoomDTO> lstRooms = objRoomDAL.GetRoomByCompanyIDPlain(SessionHelper.CompanyID).OrderByDescending(x => x.IsRoomActive).ThenBy(r => r.RoomName).ToList();

        //                        if (lstRooms != null && lstRooms.Count() > 0)
        //                        {
        //                            lstRooms.ForEach(t =>
        //                            {
        //                                t.EnterpriseId = SessionHelper.EnterPriceID;
        //                                t.EnterpriseName = SessionHelper.EnterPriceName;
        //                            });
        //                            SessionHelper.RoomID = lstRooms.First().ID;
        //                            SessionHelper.RoomName = lstRooms.First().RoomName;
        //                            //SessionHelper.isEVMI = lstRooms.First().IseVMI;
        //                            SessionHelper.RoomList = lstRooms;
        //                        }
        //                        else
        //                        {
        //                            SessionHelper.RoomID = 0;
        //                            SessionHelper.RoomName = string.Empty;
        //                            //SessionHelper.isEVMI = null;
        //                            SessionHelper.RoomList = new List<RoomDTO>();
        //                        }
        //                        List<UserWiseRoomsAccessDetailsDTO> lstpermissionfromsession = SessionHelper.RoomPermissions;
        //                        lstpermissionfromsession.ForEach(t =>
        //                        {
        //                            t.EnterpriseId = SessionHelper.EnterPriceID;
        //                            t.CompanyId = SessionHelper.CompanyID;
        //                            t.RoomID = SessionHelper.RoomID;
        //                            t.PermissionList.ForEach(et =>
        //                            {
        //                                et.EnteriseId = SessionHelper.EnterPriceID;
        //                                et.CompanyId = SessionHelper.CompanyID;
        //                                et.RoomId = SessionHelper.RoomID;
        //                            });
        //                        });
        //                        SessionHelper.RoomPermissions = SetPermissionsForsuperadmin(lstpermissionfromsession, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.RoleID, SessionHelper.UserType);
        //                    }
        //                    else
        //                    {
        //                        SessionHelper.CompanyID = CompanyId;
        //                        SessionHelper.CompanyName = CompanyName;
        //                        objCompanyMasterDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
        //                        CompanyMasterDTO objCompanyMaster = objCompanyMasterDAL.GetCompanyByID(CompanyId);
        //                        SessionHelper.CompanyLogoUrl = objCompanyMaster.CompanyLogo;
        //                        List<RoomDTO> lstAllrooms = SessionHelper.RoomList;
        //                        if (lstAllrooms != null && lstAllrooms.Any(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID))
        //                        {
        //                            SessionHelper.RoomID = lstAllrooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().ID;
        //                            SessionHelper.RoomName = lstAllrooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().RoomName;
        //                            //SessionHelper.isEVMI = lstAllrooms.Where(t => t.EnterpriseId == SessionHelper.EnterPriceID && t.CompanyID == SessionHelper.CompanyID).OrderBy(t => t.RoomName).First().IseVMI;
        //                        }
        //                        else
        //                        {
        //                            SessionHelper.RoomID = 0;
        //                            SessionHelper.RoomName = string.Empty;
        //                            //SessionHelper.isEVMI = null;
        //                        }
        //                    }
        //                    break;
        //            }
        //            Session["ReportPara"] = null;
        //            break;
        //        case "room":
        //            SessionHelper.RoomID = RoomId;
        //            SessionHelper.RoomName = RoomName;
        //            if (SessionHelper.CompanyID > 0)
        //            {
        //                string columnList = "ID,RoomName,IseVMI";
        //                RoomDTO objRoom = new CommonDAL(SessionHelper.EnterPriseDBName).GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");
        //                // RoomDTO objRoom = new RoomDAL(SessionHelper.EnterPriseDBName).GetRoomByIDPlain(SessionHelper.RoomID);
        //                if (objRoom != null)
        //                    SessionHelper.isEVMI = objRoom.IseVMI;
        //                else
        //                    SessionHelper.isEVMI = null;
        //            }
        //            else
        //                SessionHelper.isEVMI = null;
        //            switch (UserType)
        //            {
        //                case 1:
        //                    if (RoleId == -1)
        //                    {
        //                        List<UserWiseRoomsAccessDetailsDTO> lstpermissionfromsession = SessionHelper.RoomPermissions;
        //                        lstpermissionfromsession.ForEach(t =>
        //                        {
        //                            t.EnterpriseId = SessionHelper.EnterPriceID;
        //                            t.CompanyId = SessionHelper.CompanyID;
        //                            t.RoomID = SessionHelper.RoomID;
        //                            t.PermissionList.ForEach(et =>
        //                            {
        //                                et.EnteriseId = SessionHelper.EnterPriceID;
        //                                et.CompanyId = SessionHelper.CompanyID;
        //                                et.RoomId = SessionHelper.RoomID;
        //                            });
        //                        });
        //                        SessionHelper.RoomPermissions = SetPermissionsForsuperadmin(lstpermissionfromsession, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.RoleID, SessionHelper.UserType);

        //                    }
        //                    break;
        //                case 2:
        //                    if (RoleId == -2)
        //                    {
        //                        List<UserWiseRoomsAccessDetailsDTO> lstpermissionfromsession = SessionHelper.RoomPermissions;
        //                        lstpermissionfromsession.ForEach(t =>
        //                        {
        //                            t.EnterpriseId = SessionHelper.EnterPriceID;
        //                            t.CompanyId = SessionHelper.CompanyID;
        //                            t.RoomID = SessionHelper.RoomID;
        //                            t.PermissionList.ForEach(et =>
        //                            {
        //                                et.EnteriseId = SessionHelper.EnterPriceID;
        //                                et.CompanyId = SessionHelper.CompanyID;
        //                                et.RoomId = SessionHelper.RoomID;
        //                            });
        //                        });
        //                        SessionHelper.RoomPermissions = SetPermissionsForsuperadmin(lstpermissionfromsession, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.RoleID, SessionHelper.UserType);
        //                    }
        //                    break;
        //            }
        //            Session["ReportPara"] = null;
        //            break;
        //    }
        //    eTurnsRegionInfo RegionInfo = null;
        //    RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
        //    if (SessionHelper.RoomID > 0)
        //    {
        //        RegionInfo = objRegionSettingDAL.GetRegionSettingsById(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
        //    }
        //    if (RegionInfo != null)
        //    {
        //        CultureInfo roomculture = CultureInfo.CreateSpecificCulture(RegionInfo.CultureCode);
        //        TimeZoneInfo roomTimeZone = TimeZoneInfo.FindSystemTimeZoneById(RegionInfo.TimeZoneName);

        //        SessionHelper.RoomCulture = roomculture;
        //        SessionHelper.CurrentTimeZone = roomTimeZone;
        //        SessionHelper.DateTimeFormat = RegionInfo.ShortDatePattern + " " + RegionInfo.ShortTimePattern;
        //        SessionHelper.RoomDateFormat = RegionInfo.ShortDatePattern;
        //        SessionHelper.RoomTimeFormat = RegionInfo.ShortTimePattern;
        //        SessionHelper.NumberDecimalDigits = Convert.ToString(RegionInfo.NumberDecimalDigits);
        //        SessionHelper.CurrencyDecimalDigits = Convert.ToString(RegionInfo.CurrencyDecimalDigits);
        //        SessionHelper.WeightDecimalPoints = Convert.ToString(RegionInfo.WeightDecimalPoints.GetValueOrDefault(0));
        //        SessionHelper.NumberAvgDecimalPoints = Convert.ToString(RegionInfo.TurnsAvgDecimalPoints.GetValueOrDefault(2));
        //        SessionHelper.CurrencySymbol = RegionInfo.CurrencySymbol ?? "";


        //        string strQuantityFormat = "{0:0}";
        //        if (!string.IsNullOrWhiteSpace(SessionHelper.NumberDecimalDigits))
        //        {
        //            if (Convert.ToInt32(Convert.ToString(eTurnsWeb.Helper.SessionHelper.NumberDecimalDigits)) > 0)
        //            {
        //                int iQCount = Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.NumberDecimalDigits);
        //                strQuantityFormat = "{0:";
        //                for (int iq = 0; iq <= iQCount; iq++)
        //                {
        //                    if (iq == 0)
        //                    {
        //                        strQuantityFormat += "0.";
        //                    }
        //                    else
        //                    {
        //                        strQuantityFormat += "0";
        //                    }

        //                }
        //                strQuantityFormat += "}";
        //            }
        //            SessionHelper.QuantityFormat = strQuantityFormat;
        //        }
        //        else
        //            SessionHelper.QuantityFormat = strQuantityFormat;

        //        string strPriceFormat = "{0:0}";
        //        if (!string.IsNullOrWhiteSpace(SessionHelper.CurrencyDecimalDigits))
        //        {
        //            //if ((int)eTurnsWeb.Helper.SessionHelper.CompanyConfig.CurrencySymbol > 0)
        //            if (Convert.ToInt32(Convert.ToString(eTurnsWeb.Helper.SessionHelper.CurrencyDecimalDigits)) > 0)
        //            {
        //                int iQCount = Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.CurrencyDecimalDigits);
        //                strPriceFormat = "{0:";
        //                for (int iq = 0; iq <= iQCount; iq++)
        //                {
        //                    if (iq == 0)
        //                    {
        //                        strPriceFormat += "0.";
        //                    }
        //                    else
        //                    {
        //                        strPriceFormat += "0";
        //                    }

        //                }
        //                strPriceFormat += "}";
        //            }
        //            SessionHelper.PriceFormat = strPriceFormat;
        //        }
        //        else
        //            SessionHelper.PriceFormat = strPriceFormat;

        //        string strTurnsAvgFormat = "{0:0}";
        //        if (!string.IsNullOrWhiteSpace(SessionHelper.NumberAvgDecimalPoints))
        //        {
        //            if (Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.NumberAvgDecimalPoints) > 0)
        //            {
        //                int iQCount = Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.NumberAvgDecimalPoints);
        //                strTurnsAvgFormat = "{0:";
        //                for (int iq = 0; iq <= iQCount; iq++)
        //                {
        //                    if (iq == 0)
        //                    {
        //                        strTurnsAvgFormat += "0.";
        //                    }
        //                    else
        //                    {
        //                        strTurnsAvgFormat += "0";
        //                    }

        //                }
        //                strTurnsAvgFormat += "}";
        //            }
        //            SessionHelper.TurnUsageFormat = strTurnsAvgFormat;
        //        }
        //        else
        //            SessionHelper.TurnUsageFormat = strTurnsAvgFormat;

        //        string strWghtFormat = "{0:0}";
        //        if (!string.IsNullOrWhiteSpace(SessionHelper.WeightDecimalPoints))
        //        {
        //            if (Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.WeightDecimalPoints) > 0)
        //            {
        //                int iQCount = Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.WeightDecimalPoints);
        //                strWghtFormat = "{0:";
        //                for (int iq = 0; iq <= iQCount; iq++)
        //                {
        //                    if (iq == 0)
        //                    {
        //                        strWghtFormat += "0.";
        //                    }
        //                    else
        //                    {
        //                        strWghtFormat += "0";
        //                    }

        //                }
        //                strWghtFormat += "}";
        //            }
        //            SessionHelper.WeightFormat = strWghtFormat;
        //        }
        //        else
        //            SessionHelper.WeightFormat = strWghtFormat;

        //        //SessionHelper.TurnUsageFormat = Convert.ToString(RegionInfo.TurnsAvgDecimalPoints);
        //        //SessionHelper.WeightDecimalPoints = Convert.ToString(RegionInfo.WeightDecimalPoints);

        //    }
        //    else
        //    {
        //        SessionHelper.RoomCulture = CultureInfo.CreateSpecificCulture("en-US");
        //        SessionHelper.CurrentTimeZone = TimeZoneInfo.Utc;
        //        SessionHelper.DateTimeFormat = "M/d/yyyy" + " " + "h:mm:ss tt";
        //        SessionHelper.RoomDateFormat = "M/d/yyyy";
        //        SessionHelper.RoomTimeFormat = "h:mm:ss tt";
        //        SessionHelper.NumberDecimalDigits = "0";
        //        SessionHelper.CurrencyDecimalDigits = "0";
        //        SessionHelper.NumberAvgDecimalPoints = "2";
        //        SessionHelper.WeightDecimalPoints = "0";
        //        SessionHelper.QuantityFormat = "{0:0}";
        //        SessionHelper.PriceFormat = "{0:0}";
        //        SessionHelper.TurnUsageFormat = "{0:0}";
        //        SessionHelper.WeightFormat = "{0:0}";
        //        SessionHelper.CurrencySymbol = "";

        //    }


        //    //if (SessionHelper.EnterPriceID > 0)
        //    //{
        //    //    List<EnterpriseDTO> lstAllEnterprise = new EnterpriseMasterDAL().GetAllEnterprise().Where(t => t.IsActive == true).ToList();
        //    //    SessionHelper.EnterPriseList = (from ssnel in SessionHelper.EnterPriseList
        //    //                                    join allep in lstAllEnterprise on ssnel.ID equals allep.ID
        //    //                                    select ssnel).OrderBy(t => t.Name).ToList();
        //    //    if (SessionHelper.EnterPriseList.Any())
        //    //    {
        //    //        SessionHelper.EnterPriceID = SessionHelper.EnterPriseList.First().ID;
        //    //        SessionHelper.EnterPriceName = SessionHelper.EnterPriseList.First().Name;
        //    //    }
        //    //}


        //    ResourceHelper.EnterpriseResourceFolder = SessionHelper.EnterPriceID.ToString();
        //    ResourceHelper.CompanyResourceFolder = SessionHelper.EnterPriceID.ToString() + @"\" + SessionHelper.CompanyID.ToString();
        //    ResourceHelper.RoomResourceFolder = SessionHelper.EnterPriceID.ToString() + @"\" + SessionHelper.CompanyID.ToString() + @"\" + SessionHelper.RoomID.ToString();
        //    ResourceModuleHelper.EnterpriseResourceFolder = SessionHelper.EnterPriceID.ToString();
        //    ResourceModuleHelper.CompanyResourceFolder = SessionHelper.EnterPriceID.ToString() + @"\" + SessionHelper.CompanyID.ToString();
        //    ResourceModuleHelper.RoomResourceFolder = SessionHelper.EnterPriceID.ToString() + @"\" + SessionHelper.CompanyID.ToString() + @"\" + SessionHelper.RoomID.ToString();
        //    EnterPriseResourceHelper.EnterpriseResourceFolder = SessionHelper.EnterPriceID.ToString();
        //    EnterPriseResourceHelper.CompanyResourceFolder = SessionHelper.EnterPriceID.ToString() + @"\CompanyResources";
        //    EnterPriseResourceModuleHelper.EnterpriseResourceFolder = SessionHelper.EnterPriceID.ToString();
        //    EnterPriseResourceModuleHelper.CompanyResourceFolder = SessionHelper.EnterPriceID.ToString() + @"\CompanyResources";
        //    //ResourceHelper.CompanyResourceFolder = SessionHelper.CompanyID.ToString();
        //    if (eTurns.DTO.Resources.ResourceHelper.CurrentCult == null)
        //        eTurns.DTO.Resources.ResourceHelper.CurrentCult = System.Threading.Thread.CurrentThread.CurrentCulture;
        //    if (eTurns.DTO.Resources.ResourceModuleHelper.CurrentCult == null)
        //        eTurns.DTO.Resources.ResourceModuleHelper.CurrentCult = System.Threading.Thread.CurrentThread.CurrentCulture;

        //    if (SessionHelper.CompanyID > 0)
        //    {
        //        //if (SessionHelper.CompanyConfig == null)
        //        //{
        //        //    CompanyConfigDAL objCompanyConfigDAL = new CompanyConfigDAL(SessionHelper.EnterPriseDBName);
        //        //    CompanyConfigDTO objCompanyConfigDTO = objCompanyConfigDAL.GetRecord(SessionHelper.CompanyID);
        //        //    if (objCompanyConfigDTO != null)
        //        //        SessionHelper.CompanyConfig = objCompanyConfigDTO;
        //        //}
        //        //else
        //        //{
        //        //    if (SessionHelper.CompanyConfig.CompanyID != SessionHelper.CompanyID)
        //        //    {
        //        //        CompanyConfigDAL objCompanyConfigDAL = new CompanyConfigDAL(SessionHelper.EnterPriseDBName);
        //        //        CompanyConfigDTO objCompanyConfigDTO = objCompanyConfigDAL.GetRecord(SessionHelper.CompanyID);
        //        //        if (objCompanyConfigDTO != null)
        //        //            SessionHelper.CompanyConfig = objCompanyConfigDTO;
        //        //    }
        //        //}

        //        string strQuantityFormat = "{0:0}";
        //        if (!string.IsNullOrWhiteSpace(SessionHelper.NumberDecimalDigits))
        //        {
        //            if (Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.NumberDecimalDigits) > 0)
        //            {
        //                int iQCount = Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.NumberDecimalDigits);
        //                strQuantityFormat = "{0:";
        //                for (int iq = 0; iq <= iQCount; iq++)
        //                {
        //                    if (iq == 0)
        //                    {
        //                        strQuantityFormat += "0.";
        //                    }
        //                    else
        //                    {
        //                        strQuantityFormat += "0";
        //                    }

        //                }
        //                strQuantityFormat += "}";
        //            }
        //            SessionHelper.QuantityFormat = strQuantityFormat;
        //        }
        //        else
        //            SessionHelper.QuantityFormat = strQuantityFormat;

        //        string strPriceFormat = "{0:0}";
        //        if (!string.IsNullOrWhiteSpace(SessionHelper.CurrencyDecimalDigits))
        //        {
        //            //if ((int)eTurnsWeb.Helper.SessionHelper.CompanyConfig.CurrencySymbol > 0)
        //            if (Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.CurrencyDecimalDigits) > 0)
        //            {
        //                int iQCount = Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.CurrencyDecimalDigits);
        //                strPriceFormat = "{0:";
        //                for (int iq = 0; iq <= iQCount; iq++)
        //                {
        //                    if (iq == 0)
        //                    {
        //                        strPriceFormat += "0.";
        //                    }
        //                    else
        //                    {
        //                        strPriceFormat += "0";
        //                    }

        //                }
        //                strPriceFormat += "}";
        //            }
        //            SessionHelper.PriceFormat = strPriceFormat;
        //        }
        //        else
        //            SessionHelper.PriceFormat = strPriceFormat;

        //        string strTurnsAvgFormat = "{0:0}";
        //        if (!string.IsNullOrWhiteSpace(SessionHelper.NumberAvgDecimalPoints))
        //        {
        //            if (Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.NumberAvgDecimalPoints) > 0)
        //            {
        //                int iQCount = Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.NumberAvgDecimalPoints);
        //                strTurnsAvgFormat = "{0:";
        //                for (int iq = 0; iq <= iQCount; iq++)
        //                {
        //                    if (iq == 0)
        //                    {
        //                        strTurnsAvgFormat += "0.";
        //                    }
        //                    else
        //                    {
        //                        strTurnsAvgFormat += "0";
        //                    }

        //                }
        //                strTurnsAvgFormat += "}";
        //            }
        //            SessionHelper.TurnUsageFormat = strTurnsAvgFormat;
        //        }
        //        else
        //            SessionHelper.TurnUsageFormat = strTurnsAvgFormat;

        //        string strWghtFormat = "{0:0}";
        //        if (!string.IsNullOrWhiteSpace(SessionHelper.WeightDecimalPoints))
        //        {
        //            if (Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.WeightDecimalPoints) > 0)
        //            {
        //                int iQCount = Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.WeightDecimalPoints);
        //                strWghtFormat = "{0:";
        //                for (int iq = 0; iq <= iQCount; iq++)
        //                {
        //                    if (iq == 0)
        //                    {
        //                        strWghtFormat += "0.";
        //                    }
        //                    else
        //                    {
        //                        strWghtFormat += "0";
        //                    }

        //                }
        //                strWghtFormat += "}";
        //            }
        //            SessionHelper.WeightFormat = strWghtFormat;
        //        }
        //        else
        //            SessionHelper.WeightFormat = strWghtFormat;

        //    }
        //    UserLoginHistoryDTO objUserLoginHistory = new UserLoginHistoryDTO();
        //    objUserLoginHistory.CompanyId = SessionHelper.CompanyID;
        //    objUserLoginHistory.EnterpriseId = SessionHelper.EnterPriceID;
        //    objUserLoginHistory.EventDate = DateTimeUtility.DateTimeNow;
        //    switch (EventFiredOn)
        //    {
        //        case "onlogin":
        //            objUserLoginHistory.EventType = 1;
        //            break;
        //        case "enterprise":
        //            objUserLoginHistory.EventType = 3;
        //            break;
        //        case "company":
        //            objUserLoginHistory.EventType = 4;
        //            break;
        //        case "room":
        //            objUserLoginHistory.EventType = 5;
        //            break;
        //        default:
        //            objUserLoginHistory.EventType = 0;
        //            break;
        //    }
        //    objUserLoginHistory.IpAddress = string.Empty;
        //    objUserLoginHistory.CountryName = string.Empty;
        //    objUserLoginHistory.RegionName = string.Empty;
        //    objUserLoginHistory.CityName = string.Empty;
        //    objUserLoginHistory.ZipCode = string.Empty;
        //    objUserLoginHistory.ID = 0;
        //    if (HttpContext != null)
        //    {
        //        objUserLoginHistory.IpAddress = Request.UserHostAddress;
        //        //UserLoginHistoryDTO IPInfo = getLocationInfo(Request.UserHostAddress);
        //        UserLoginHistoryDTO IPInfo = new UserLoginHistoryDTO();
        //        if (IPInfo == null)
        //        {
        //            IPInfo = new UserLoginHistoryDTO();
        //        }
        //        objUserLoginHistory.CountryName = IPInfo.CountryName;
        //        objUserLoginHistory.RegionName = IPInfo.RegionName;
        //        objUserLoginHistory.CityName = IPInfo.CityName;
        //        objUserLoginHistory.ZipCode = IPInfo.ZipCode;
        //    }
        //    objUserLoginHistory.RoomId = SessionHelper.RoomID;
        //    objUserLoginHistory.UserId = SessionHelper.UserID;


        //    objUserMasterDAL.SaveUserActions(objUserLoginHistory);

        //}

        public static List<UserWiseRoomsAccessDetailsDTO> SetPermissionsForsuperadmin(List<UserWiseRoomsAccessDetailsDTO> lstRowpermission, long EnterpriseId, long CompanyId, long RoomId, long RoleId, long RoleType)
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
                            it.ShowArchived = true;
                            it.ShowDeleted = true;
                            it.ShowUDF = true;
                            it.ShowChangeLog = true;
                        }
                        else
                        {
                            it.IsView = false;
                            it.IsDelete = false;
                            it.IsInsert = false;
                            it.IsUpdate = false;
                            it.IsChecked = false;
                            it.ShowArchived = false;
                            it.ShowDeleted = false;
                            it.ShowUDF = false;
                            it.ShowChangeLog = false;
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
                            it.ShowArchived = true;
                            it.ShowDeleted = true;
                            it.ShowUDF = true;
                            it.ShowChangeLog = true;
                        }
                        else
                        {
                            it.IsView = false;
                            it.IsDelete = false;
                            it.IsInsert = false;
                            it.IsUpdate = false;
                            it.IsChecked = false;
                            it.ShowArchived = false;
                            it.ShowDeleted = false;
                            it.ShowUDF = false;
                            it.ShowChangeLog = false;
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
                            it.ShowArchived = true;
                            it.ShowDeleted = true;
                            it.ShowUDF = true;
                            it.ShowChangeLog = true;
                        }
                        else
                        {
                            it.IsView = false;
                            it.IsDelete = false;
                            it.IsInsert = false;
                            it.IsUpdate = false;
                            it.IsChecked = false;
                            it.ShowArchived = false;
                            it.ShowDeleted = false;
                            it.ShowUDF = false;
                            it.ShowChangeLog = true;
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
                        it.ShowArchived = true;
                        it.ShowDeleted = true;
                        it.ShowUDF = true;
                        it.ShowChangeLog = true;
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
                            ob.ShowChangeLog = objInactiveUserRoleModuleDetailsDTO.ShowChangeLog;
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



    }// class

    public class FormsLoginCookieData
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string CurrentLoggedInUserName { get; set; }
        public bool IsRememberCookie { get; set; }
        public string ReturnUrl { get; set; }
        public bool IsSSO { get; set; }
        public string SSOClient { get; set; }
    }
}