using eTurns.DTO;
using eTurnsMaster.DAL;
using eTurnsWeb.Helper;
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.HttpModule;
using Sustainsys.Saml2.Mvc;
using Sustainsys.Saml2.WebSso;
using System;
using System.Collections.Generic;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Configuration;
namespace eTurnsWeb.Controllers.SSO
{
    [AllowAnonymous]
    public class OktaClientController : eTurnsControllerBase
    {


        public ActionResult SignIn()
        {
            EnterPriseUserMasterDAL objEnterPriseUserMasterDAL = new EnterPriseUserMasterDAL();
            string dbEnterpriseAppConfigKValue = objEnterPriseUserMasterDAL.GetEnterpriseSSOMapConfigurationKeyByID(0);
            if (!string.IsNullOrEmpty(dbEnterpriseAppConfigKValue))
            {
                Response.Cookies.Add(new HttpCookie("OktaRequestInitiated", "1"));
                SessionHelper.UserID = 0;
                string returnUrl = SiteSettingHelper.OktaReturnUrl;
                string url = SiteSettingHelper.OktaServiceUrl + "?enterprise=" + dbEnterpriseAppConfigKValue + "&ReturnUrl=" + StringCipher.Encrypt(SessionHelper.CurrentDomainURL + "/" + returnUrl);
                return Redirect(url);
        }
            return View();
        }

        /// <summary>
        /// Redirect url after login success
        /// </summary>
        /// <returns></returns>
        public ActionResult OktaLoginSuccessWithoutMap()
        {
            ViewBag.Message = "";

            if (TempData["SamlLogInSuccessCustomEventArgs"] != null)
            {
                string redirect = "";
                if (Request.Cookies["SSOReturnURL"] != null)
                {
                    string SSOReturnURL = Request.Cookies["SSOReturnURL"].Value;
                    string[] urlParts = SSOReturnURL.Split('?');
                    redirect = urlParts[0];
                }

                HttpCookie cookieUserEmailId = Request.Cookies["UserEmailId"];

                HttpCookie cookieOktaRequestInitiated = Request.Cookies["OktaRequestInitiated"];
                if (cookieOktaRequestInitiated != null)
                {
                    cookieOktaRequestInitiated.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(cookieOktaRequestInitiated);
                }


                ClaimsPrincipal samlLogIn1 = TempData["SamlLogInSuccessCustomEventArgs"] as ClaimsPrincipal;  //Session["SamlLogInSuccessEventArgs"] as SamlLogInSuccessEventArgs;

                SSOUsersMapDAL UsersMapDAL = new SSOUsersMapDAL();
                SSOAttributeMapDTO ssoAttributeMap = UsersMapDAL.GetSSOAttributeMap();


                if (samlLogIn1 != null && samlLogIn1.Claims != null && ssoAttributeMap != null && cookieUserEmailId != null)
                {
                    Dictionary<string, string> oktaAttributes = samlLogIn1.Claims.ToDictionary(x => x.Type, x => x.Value);
                    if (oktaAttributes.ContainsKey(ssoAttributeMap.UserEmail) && oktaAttributes[ssoAttributeMap.UserEmail] != cookieUserEmailId.Value)
                    {
                        SessionHelper.CompanyID = 0;
                        SessionHelper.CompanyList = null;
                        SessionHelper.CompanyName = null;
                        SessionHelper.EnterPriceID = 0;
                        SessionHelper.EnterPriceName = null;
                        //SessionHelper.EnterPriseDBConnectionString = null;
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
                        SessionHelper.isEVMI = null;
                        SessionHelper.AllowABIntegration = false;
                        Session.Abandon();
                        Session.RemoveAll();
                        FormsAuthentication.SignOut();

                        string OktaSSOErrorMessage = "Invalid user login " + cookieUserEmailId.Value + " at eTurns. Please logout in okta domain and re-login with different user.";
                        Response.Cookies.Add(new HttpCookie("OktaSSOErrorMessage", OktaSSOErrorMessage));
                        if (cookieUserEmailId != null)
                        {
                            cookieUserEmailId.Expires = DateTime.Now.AddDays(-1);
                            Response.Cookies.Add(cookieUserEmailId);
                        }
                        return Redirect("~/Master/UserLogin");

                    }
                }

                if (redirect != "" && !redirect.Contains("/OktaClient/OktaLoginSuccessWithoutMap"))
                {
                    return Redirect(redirect);
                }
                else
                {
                    return Redirect("/Master/Dashboard");
                }
            }

            // ETurns Login            
            return Content("Login Failed.");
        }
        public ActionResult OktaLoginWebSVC(string result)
        {
            if (result == null)
            {
                return Content("Invalid request.");
            }
            try
            {
                string dummyData = result.Trim().Replace(" ", "+");
                if (dummyData.Length % 4 > 0)
                    dummyData = dummyData.PadRight(dummyData.Length + 4 - dummyData.Length % 4, '=');

                string[] resultValue = StringCipher.Decrypt(dummyData.Replace(" ", "+")).Split('^');

                if (resultValue.Count() >= 2)
                {
                    if (resultValue[0].ToLower() == "success")
                    {
                        return OktaLoginWebSVCSuccessWithEmailId(resultValue[1]);
                    }
                    else
                    {
                        return OktaLoginWebSVCFail();
                    }
                }
                else
                {
                    if (StringCipher.Decrypt(result).ToLower() == "success")
                    {
                        return OktaLoginWebSVCSuccess();
                    }
                    else
                    {
                        return OktaLoginWebSVCFail();
                    }
                }
            }
            catch (Exception ex)
            {
                return Content("Invalid request.");
            }
        }
        /// <summary>
        /// Redirect url after login success
        /// </summary>
        /// <returns></returns>
        public ActionResult OktaLoginWebSVCSuccess()
        {
            ViewBag.Message = "";

            string redirect = "";
            if (Request.Cookies["SSOReturnURL"] != null)
            {
                string SSOReturnURL = Request.Cookies["SSOReturnURL"].Value;
                string[] urlParts = SSOReturnURL.Split('?');
                redirect = urlParts[0];
            }

            HttpCookie cookieUserEmailId = Request.Cookies["UserEmailId"];

            HttpCookie cookieOktaRequestInitiated = Request.Cookies["OktaRequestInitiated"];
            if (cookieOktaRequestInitiated != null)
            {
                cookieOktaRequestInitiated.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookieOktaRequestInitiated);
            }

            if (redirect != "" && !redirect.Contains("/OktaClient/OktaLoginWebSVCSuccess"))
            {
                return Redirect(redirect);
            }
            else
            {
                return Redirect("/Master/Dashboard");
            }

        }
        /// <summary>
        /// Redirect url after login success
        /// </summary>
        /// <returns></returns>
        public ActionResult OktaLoginWebSVCFail()
        {
            HttpCookie cookieSSOReturnURL = Request.Cookies["SSOReturnURL"];
            if (cookieSSOReturnURL != null)
            {
                cookieSSOReturnURL.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookieSSOReturnURL);
            }
            HttpCookie cookieOktaRequestInitiated = Request.Cookies["OktaRequestInitiated"];
            if (cookieOktaRequestInitiated != null)
            {
                cookieOktaRequestInitiated.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookieOktaRequestInitiated);
            }

            SessionHelper.CompanyID = 0;
            SessionHelper.CompanyList = null;
            SessionHelper.CompanyName = null;
            SessionHelper.EnterPriceID = 0;
            SessionHelper.EnterPriceName = null;
            //SessionHelper.EnterPriseDBConnectionString = null;
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
            SessionHelper.isEVMI = null;
            SessionHelper.AllowABIntegration = false;
            Session.Abandon();
            Session.RemoveAll();
            FormsAuthentication.SignOut();
            string OktaSSOErrorMessage = "Invalid user login at eTurns.";
            Response.Cookies.Add(new HttpCookie("OktaSSOErrorMessage", OktaSSOErrorMessage));
            return Redirect("~/Master/UserLogin");
        }

        /// <summary>
        /// Redirect url after login success
        /// </summary>
        /// <returns></returns>
        public ActionResult OktaLoginWebSVCSuccessWithEmailId(string userEmailId)
        {
            ViewBag.Message = "";

            if (!string.IsNullOrEmpty(userEmailId))
            {
                string returnUrl = SignInToETurnsWithWebSVC(userEmailId);

                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    string OktaSSOErrorMessage = "Login Failed - " + ViewBag.Message;
                    Response.Cookies.Add(new HttpCookie("OktaSSOErrorMessage", OktaSSOErrorMessage));
                    return Redirect("~/Master/UserLogin");
                }

            }

            // ETurns Login            
            return Content("Login Failed.");
        }
        /// <summary>
        /// Redirect url after login success
        /// </summary>
        /// <returns></returns>
        public ActionResult OktaLoginSuccess()
        {
            SamlLogInSuccessEventArgs samlLogIn = null;
            ViewBag.Message = "";

            if (TempData["SamlLogInSuccessEventArgs"] != null)
            {
                string SSOReturnURL = "";

                SSOReturnURL = Request.Cookies["SSOReturnURL"].Value;

                string client = "";

                string[] urlParts = SSOReturnURL.Split('?');
                string[] queryStrings = urlParts[1].Split('&');

                for (int i = 0; i < queryStrings.Length; i++)
                {
                    string q = queryStrings[i];
                    if (!string.IsNullOrWhiteSpace(q))
                    {
                        string[] keyVal = q.Split('=');

                        if (keyVal[0] == "client")
                        {
                            client = keyVal[1];
                        }
                    }
                }

                samlLogIn = TempData["SamlLogInSuccessEventArgs"] as SamlLogInSuccessEventArgs;  //Session["SamlLogInSuccessEventArgs"] as SamlLogInSuccessEventArgs;
                bool isEturnLogin = SignInToETurns(samlLogIn, client, SSOReturnURL);
                string redirect = urlParts[0];

                if (isEturnLogin)
                {
                    return Redirect(redirect);
                }
                else
                {
                    return Content("Login Failed - " + ViewBag.Message);
                }

            }

            // ETurns Login            
            return Content("Login Failed.");
        }

        ///// <summary>
        ///// Assertion consumer Url that accepts the incoming Saml response
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult Acs()
        //{
        //    var result = CommandFactory.GetCommand(CommandFactory.AcsCommandName).Run(
        //        Request.ToHttpRequestData(),
        //        Options);

        //    if (result.HandledResult)
        //    {
        //        throw new NotSupportedException("The MVC controller doesn't support setting CommandResult.HandledResult.");
        //    }

        //    bool isLogin = SignInToETurns(result);

        //    if (isLogin)
        //    {
        //        string url = FormsAuthentication.GetRedirectUrl("", false);
        //        return Redirect(url);
        //    }
        //    else
        //    {
        //        return Redirect(FormsAuthentication.LoginUrl);
        //    }
        //}

        private bool SignInToETurns(SamlLogInSuccessEventArgs samlLogIn, string ssoClient, string returnUrl)
        {

            if (string.IsNullOrWhiteSpace(samlLogIn.UserEmail))
            {
                return false;
            }

            EnterPriseUserMasterDAL objEnterPriseUserMasterDAL = new EnterPriseUserMasterDAL();
            EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();

            SSOUsersMapDAL UsersMapDAL = new SSOUsersMapDAL();
            var userMap = UsersMapDAL.GetSSOUserMap(ssoClient, "okta", samlLogIn.UserEmail);

            if (userMap == null)
            {
                string user = "";

                if (!string.IsNullOrWhiteSpace(samlLogIn.UserFirstName) && !string.IsNullOrWhiteSpace(samlLogIn.UserLastName))
                {
                    user = samlLogIn.UserFirstName + " " + samlLogIn.UserLastName;
                }

                if (!string.IsNullOrWhiteSpace(user))
                {
                    user += " ( " + samlLogIn.UserEmail + " )";
                }
                else
                {
                    user = samlLogIn.UserEmail;
                }

                string oktaUrl = @"https://dev-328912.okta.com/";
                ViewBag.Status = "fail";
                ViewBag.Message = "User mapping not found for user '<b>" + user + "</b>' at eTurns. <br/>Please logout in okta domain <b><a href=" + oktaUrl + ">re-login</a></b> with different user.";
                return false;
            }

            // login process of eTurns
            // get eTurns user mapped with Okta, set session variables
            UserBAL objUserBAL = new UserBAL();
            UserMasterDTO objDTO = objEnterPriseUserMasterDAL.CheckAuthanticationUserNameOnly(userMap.eTurnsUserName);
            int PermissionCount = 1;
            bool IsUserLocked = objUserBAL.CheckSetAcountLockout(samlLogIn.UserEmail, "", objDTO);

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
                        if (!objDTO.IsNgNLFAllowed)
                        {
                            var ngNLFURL = System.Configuration.ConfigurationManager.AppSettings["NgNLFURL"];

                            if (!string.IsNullOrEmpty(ngNLFURL) && !string.IsNullOrWhiteSpace(ngNLFURL) && ngNLFURL.Length > 0)
                            {
                                var currentURLWithoutParam = Request.Url.GetLeftPart(UriPartial.Path);

                                if (!string.IsNullOrEmpty(currentURLWithoutParam) && !string.IsNullOrWhiteSpace(currentURLWithoutParam)
                                    && currentURLWithoutParam.Length > 0)
                                {
                                    if (currentURLWithoutParam.ToLower().Contains(ngNLFURL.ToLower()))
                                    {
                                        ViewBag.Status = "fail";
                                        ViewBag.Message = "Not allowed to access";//ResLoginForms.NotAllowedToAccess;//"Not allowed to access";
                                        return false; //View();
                                    }
                                }
                            }
                        }


                        string retURL = LoginHelper.SetFormsAuthenticationCookie(Response, new FormsLoginCookieData()
                        {
                            UserName = objDTO.UserName,
                            IsRememberCookie = true,
                            Email = objDTO.Email,
                            CurrentLoggedInUserName = objDTO.UserName,
                            ReturnUrl = returnUrl,
                            IsSSO = true,
                            SSOClient = ssoClient

                        });

                        eTurns.DTO.UserSettingDTO objUserSettingDTO = LoginHelper.SetSessionDataAtLogin(Request, Session, HttpContext, objDTO);

                        ViewBag.Status = "ok";
                        ViewBag.Message = "success";
                        return true;

                        //string retURL = FormsAuthentication.GetRedirectUrl(objLoginModel.Email, false);
                        //if (string.IsNullOrWhiteSpace(retURL))
                        //{
                        //    retURL = objLoginModel.ReturnUrl;
                        //}

                        //var tmpNonRedirectablUrls = SiteSettingHelper.NonRedirectablUrls;
                        //var nonRedirectableUrls = !string.IsNullOrEmpty(tmpNonRedirectablUrls) && !string.IsNullOrWhiteSpace(tmpNonRedirectablUrls)
                        //         ? tmpNonRedirectablUrls.ToLower() : string.Empty;
                        //var nonRedirectableUrlList = nonRedirectableUrls.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToList();

                        //var tmpReturnURL = retURL;
                        //if (!string.IsNullOrEmpty(retURL) && !string.IsNullOrWhiteSpace(retURL) && retURL != "/" && retURL != "/default.aspx"
                        //        && !retURL.ToLower().Contains("logoutuser"))
                        //{
                        //    tmpReturnURL = retURL.Split(new[] { '?' })[0];
                        //}

                        //if (!string.IsNullOrEmpty(tmpReturnURL) && !string.IsNullOrWhiteSpace(tmpReturnURL) && retURL != "/" && tmpReturnURL != "/default.aspx"
                        //        && !tmpReturnURL.ToLower().Contains("logoutuser") && !nonRedirectableUrlList.Contains(tmpReturnURL.ToLower()))
                        //{
                        //    try
                        //    {
                        //        Uri uri = new Uri(Request.Url.ToString());
                        //        string requested = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;
                        //        requested = requested + retURL;
                        //        uri = new Uri(requested);
                        //        if (string.IsNullOrWhiteSpace(uri.Query))
                        //        {
                        //            return Redirect(retURL + "?FromLogin=yes");
                        //        }
                        //        else
                        //        {
                        //            return Redirect(retURL + "&FromLogin=yes");
                        //        }
                        //    }
                        //    catch (Exception)
                        //    {
                        //        if (retURL.IndexOf('?') >= 0)
                        //        {
                        //            return Redirect(retURL + "&FromLogin=yes");
                        //        }
                        //        else
                        //        {
                        //            return Redirect(retURL + "?FromLogin=yes");
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //    if (objUserSettingDTO == null || (string.IsNullOrEmpty(objUserSettingDTO.RedirectURL)))
                        //    {
                        //        string CtrlName = Convert.ToString(ConfigurationManager.AppSettings["CtrlName"]);
                        //        string ActName = Convert.ToString(ConfigurationManager.AppSettings["ActName"]);
                        //        return RedirectToAction(ActName, CtrlName, new { FromLogin = "yes" });
                        //    }
                        //    else
                        //    {
                        //        return Redirect(objUserSettingDTO.RedirectURL + "?FromLogin=yes");
                        //    }
                        //}
                    }
                    else
                    {
                        ViewBag.Status = "fail";
                        ViewBag.Message = "locked";
                        return false; //View();
                        //return Json(new { Message = "fail", Status = "locked" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    ViewBag.Status = "fail";
                    ViewBag.Message = "Atleast one room should be assigned to you to get in.";
                    return false; //View();
                }
            }

            bool isLogin = true;
            return isLogin;
        }

        private string SignInToETurnsWithWebSVC(string userEmailId)
        {
            if (string.IsNullOrWhiteSpace(userEmailId))
            {
                return "";
            }

            EnterPriseUserMasterDAL objEnterPriseUserMasterDAL = new EnterPriseUserMasterDAL();
            EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();

            SSOUsersMapDAL UsersMapDAL = new SSOUsersMapDAL();
            var userMap = UsersMapDAL.GetSSOUserMapWithWebSVC(userEmailId);
            if (userMap == null)
            {
                ViewBag.Status = "fail";
                ViewBag.Message = "User not found in Border States VMI. Please email bsehelpdesk@borderstates.com";//ResLoginForms.NotAllowedToAccess;//"Not allowed to access";
                return ""; //View();
    }
            UserBAL objUserBAL = new UserBAL();
            UserMasterDTO objDTO = objEnterPriseUserMasterDAL.CheckAuthanticationUserNameOnly(userMap.eTurnsUserName);
            int PermissionCount = 1;

            if (objDTO != null)
            {
                bool IsUserLocked = objUserBAL.CheckSetAcountLockout(userEmailId, "", objDTO);

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
                        if (!objDTO.IsNgNLFAllowed)
                        {
                            var ngNLFURL = System.Configuration.ConfigurationManager.AppSettings["NgNLFURL"];

                            if (!string.IsNullOrEmpty(ngNLFURL) && !string.IsNullOrWhiteSpace(ngNLFURL) && ngNLFURL.Length > 0)
                            {
                                var currentURLWithoutParam = Request.Url.GetLeftPart(UriPartial.Path);

                                if (!string.IsNullOrEmpty(currentURLWithoutParam) && !string.IsNullOrWhiteSpace(currentURLWithoutParam)
                                    && currentURLWithoutParam.Length > 0)
                                {
                                    if (currentURLWithoutParam.ToLower().Contains(ngNLFURL.ToLower()))
                                    {
                                        ViewBag.Status = "fail";
                                        ViewBag.Message = "Not allowed to access";//ResLoginForms.NotAllowedToAccess;//"Not allowed to access";
                                        return ""; //View();
                                    }
                                }
                            }
                        }


                        string retURL = LoginHelper.SetFormsAuthenticationCookie(Response, new FormsLoginCookieData()
                        {
                            UserName = objDTO.UserName,
                            IsRememberCookie = true,
                            Email = objDTO.Email,
                            CurrentLoggedInUserName = objDTO.UserName,
                            ReturnUrl = "",
                            IsSSO = true,
                            SSOClient = ""

                        });

                        eTurns.DTO.UserSettingDTO objUserSettingDTO = LoginHelper.SetSessionDataAtLogin(Request, Session, HttpContext, objDTO);
                        SessionHelper.HasPasswordChanged = true;
                        ViewBag.Status = "ok";
                        ViewBag.Message = "success";
                        string OktaReturnURL = "";


                        var tmpNonRedirectablUrls = SiteSettingHelper.NonRedirectablUrls;
                        var nonRedirectableUrls = !string.IsNullOrEmpty(tmpNonRedirectablUrls) && !string.IsNullOrWhiteSpace(tmpNonRedirectablUrls)
                                 ? tmpNonRedirectablUrls.ToLower() : string.Empty;
                        var nonRedirectableUrlList = nonRedirectableUrls.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToList();

                        var tmpReturnURL = retURL;
                        if (!string.IsNullOrEmpty(retURL) && !string.IsNullOrWhiteSpace(retURL) && retURL != "/" && retURL != "/default.aspx"
                                && !retURL.ToLower().Contains("logoutuser"))
                        {
                            tmpReturnURL = retURL.Split(new[] { '?' })[0];
                        }

                        if (!string.IsNullOrEmpty(tmpReturnURL) && !string.IsNullOrWhiteSpace(tmpReturnURL) && retURL != "/" && tmpReturnURL != "/default.aspx"
                                && !tmpReturnURL.ToLower().Contains("logoutuser") && !nonRedirectableUrlList.Contains(tmpReturnURL.ToLower()))
                        {
                            try
                            {
                                Uri uri = new Uri(Request.Url.ToString());
                                string requested = uri.Scheme + Uri.SchemeDelimiter + uri.Host + ":" + uri.Port;
                                requested = requested + retURL;
                                uri = new Uri(requested);
                                if (string.IsNullOrWhiteSpace(uri.Query))
                                {
                                    //return Redirect(retURL + "?FromLogin=yes");
                                    OktaReturnURL = retURL + "?FromLogin=yes";
                                }
                                else
                                {
                                    //return Redirect(retURL + "&FromLogin=yes");
                                    OktaReturnURL = retURL + "&FromLogin=yes";
                                }
                            }
                            catch (Exception)
                            {
                                if (retURL.IndexOf('?') >= 0)
                                {
                                    //return Redirect(retURL + "&FromLogin=yes");
                                    OktaReturnURL = retURL + "&FromLogin=yes";
                                }
                                else
                                {
                                    //return Redirect(retURL + "?FromLogin=yes");
                                    OktaReturnURL = retURL + "&FromLogin=yes";
                                }
                            }
                        }
                        else
                        {
                            if (objUserSettingDTO == null || (string.IsNullOrEmpty(objUserSettingDTO.RedirectURL)))
                            {
                                string CtrlName = Convert.ToString(ConfigurationManager.AppSettings["CtrlName"]);
                                string ActName = Convert.ToString(ConfigurationManager.AppSettings["ActName"]);
                                //return RedirectToAction(ActName, CtrlName, new { FromLogin = "yes" });
                                OktaReturnURL = "/" + CtrlName + "/" + ActName + "?FromLogin=yes";
                            }
                            else
                            {
                                eTurnsMaster.DAL.UserMasterDAL userMasterDAL = new eTurnsMaster.DAL.UserMasterDAL();
                                var DefaultRedirectURLs = userMasterDAL.FetchRedirectURL(objUserSettingDTO.RedirectURL);
                                if (DefaultRedirectURLs != null && (!string.IsNullOrEmpty(DefaultRedirectURLs.SapphireURLs)) && (!string.IsNullOrEmpty(DefaultRedirectURLs.TrackstockURLs)))
                                {
                                    if (SessionHelper.CurrentDomainURL.ToLower().Contains("sapphire"))
                                    {
                                        objUserSettingDTO.RedirectURL = DefaultRedirectURLs.SapphireURLs;
                                    }
                                    else if (SessionHelper.CurrentDomainURL.ToLower().Contains("trackstock"))
                                    {
                                        objUserSettingDTO.RedirectURL = DefaultRedirectURLs.TrackstockURLs;
                                    }
                                }
                                //return Redirect(objUserSettingDTO.RedirectURL + "?FromLogin=yes");
                                OktaReturnURL = objUserSettingDTO.RedirectURL + "?FromLogin=yes";
                            }
                        }
                        //return Redirect(OktaReturnURL);
                        return OktaReturnURL;
                    }
                    else
                    {
                        ViewBag.Status = "fail";
                        ViewBag.Message = "User account is locked";
                        return ""; //View();
                        //return Json(new { Message = "fail", Status = "locked" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    ViewBag.Status = "fail";
                    ViewBag.Message = "Atleast one room should be assigned to you to get in.";
                    return ""; //View();
                }
            }

            bool isLogin = true;
            return "/Master/MyProfile";
        }

    }

}