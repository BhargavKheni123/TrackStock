using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Intuit.Ipp.OAuth2PlatformClient;
using System.Configuration;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using eTurns.DTO;
using eTurns.DAL;
using eTurnsWeb.Helper;
using Intuit.Ipp.Data;
using Intuit.Ipp.Core;
using Intuit.Ipp.Security;
using Intuit.Ipp.QueryFilter;

namespace eTurnsWeb.Controllers
{
    [AuthorizeHelper]
    public class QuickBookCallbackController : eTurnsControllerBase
    {
        string CtrlName = Convert.ToString(ConfigurationManager.AppSettings["CtrlName"]);
        string ActName = Convert.ToString(ConfigurationManager.AppSettings["ActName"]);

        public async Task<ActionResult> AuthTokenProcess()
        {
            if (!string.IsNullOrWhiteSpace(SiteSettingHelper.QBClientID) && !string.IsNullOrWhiteSpace(SiteSettingHelper.QBClientSecret) && !string.IsNullOrWhiteSpace(SiteSettingHelper.QBRedirectUrl)
                && !string.IsNullOrWhiteSpace(SiteSettingHelper.QBEnvironment)) 
            {
                //Sync the state info and update if it is not the same
                OAuth2Client auth2Client = new OAuth2Client(SiteSettingHelper.QBClientID, SiteSettingHelper.QBClientSecret, SiteSettingHelper.QBRedirectUrl, SiteSettingHelper.QBEnvironment);
                var state = Request.QueryString["state"];
                
                if (state.Equals(auth2Client.CSRFToken, StringComparison.Ordinal))
                    ViewBag.State = state + " (valid)";
                else
                    ViewBag.State = state + " (invalid)";

                string code = Request.QueryString["code"] ?? string.Empty;
                string realmId = Request.QueryString["realmId"] ?? string.Empty;
                string Error = Request.QueryString["error"] ?? string.Empty;
                QuickBookTokenDetailDTO objQBTokendtlDTO = await GetAuthTokensAsync(code, realmId, Error);
                objQBTokendtlDTO.AuthError = Request.QueryString["error"] ?? "";

                if (objQBTokendtlDTO.AccessToken != string.Empty)
                    TempData["AccessToken"] = "Success";
                else
                    TempData["AccessToken"] = string.Empty;

                if (objQBTokendtlDTO.AuthError != "")
                    TempData["AuthError"] = objQBTokendtlDTO.AuthError;
                else
                    TempData["AuthError"] = string.Empty;

                //
                eMailMasterDAL objeMailMasterDAL = new eMailMasterDAL(DbConnectionHelper.GeteTurnsDBName());
                eTurnsMaster.DAL.EnterpriseMasterDAL objEntDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
                EnterpriseDTO objEntDTo = objEntDAL.GetEnterpriseByIdPlain(SessionHelper.EnterPriceID);
                string strEmailTo = CommonUtility.GetQBEmailToAddress();
                string strCCAddress = CommonUtility.GetQBEmailCCAddress();
                if (objQBTokendtlDTO.IsSuccess == true)
                {
                    string strMessage = "Successfully setup for QuickBook" + Environment.NewLine +
                                        " Enterprise : " + objEntDTo.Name + " ,Company : " + SessionHelper.CompanyName + " ,Room : " + SessionHelper.RoomName;

                    objeMailMasterDAL.eMailToSend(strEmailTo, strCCAddress, "QuickBook Setup", strMessage, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, objEntDTo.EnterpriseUserID, null, "NIL");
                }
                else if (objQBTokendtlDTO.IsSuccess == false)
                {
                    string strMessage = "Error setup for QuickBook : " + objQBTokendtlDTO.AuthError + Environment.NewLine +
                                        " Enterprise : " + objEntDTo.Name + " ,Company : " + SessionHelper.CompanyName + " ,Room : " + SessionHelper.RoomName;

                    objeMailMasterDAL.eMailToSend(strEmailTo, strCCAddress, "QuickBook Setup", strMessage, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, objEntDTo.EnterpriseUserID, null, "NIL");
                }
                //

                //return RedirectToAction("SetupInfo", "QuickBookCallback");
                return RedirectToAction("QuickBookSetup", "Master");
            }
            else
            {
                return RedirectToAction(ActName, CtrlName);
            }
            
        }

        private async Task<QuickBookTokenDetailDTO> GetAuthTokensAsync(string code, string realmId, string Error)
        {
            OAuth2Client auth2Client = new OAuth2Client(SiteSettingHelper.QBClientID, SiteSettingHelper.QBClientSecret, SiteSettingHelper.QBRedirectUrl, SiteSettingHelper.QBEnvironment);
            Request.GetOwinContext().Authentication.SignOut("TempState");
            var tokenResponse = await auth2Client.GetBearerTokenAsync(code);

            //var claims = new List<Claim>();

            //if (!string.IsNullOrWhiteSpace(realmId))
            //    claims.Add(new Claim("realmId", realmId));

            //var id = new ClaimsIdentity(claims, "Cookies");
            //Request.GetOwinContext().Authentication.SignIn(id);

            //DiscoveryPolicy dpolicy = new DiscoveryPolicy();
            //dpolicy.RequireHttps = true;
            //dpolicy.ValidateIssuerName = true;

            QuickBookTokenDetailDAL objQBTokenDetailDAL = new QuickBookTokenDetailDAL(DbConnectionHelper.GeteTurnsQuickBookDBName());
            QuickBookTokenDetailDTO objQBTokendtlDTO = null;
            objQBTokendtlDTO = objQBTokenDetailDAL.GetTokenDetail(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);

            if (objQBTokendtlDTO == null)
            {
                objQBTokendtlDTO = new QuickBookTokenDetailDTO();
                objQBTokendtlDTO.GUID = Guid.NewGuid();
                objQBTokendtlDTO.EnterpriseID = SessionHelper.EnterPriceID;
                objQBTokendtlDTO.CompanyID = SessionHelper.CompanyID;
                objQBTokendtlDTO.RoomID = SessionHelper.RoomID;
                if (!string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
                {
                    //claims.Add(new Claim("access_token", tokenResponse.AccessToken));
                    //claims.Add(new Claim("access_token_expires_at", (DateTime.UtcNow.AddSeconds(tokenResponse.AccessTokenExpiresIn)).ToString()));

                    objQBTokendtlDTO.AccessToken = tokenResponse.AccessToken;
                    objQBTokendtlDTO.AccessTokenExpireDate = DateTime.UtcNow.AddSeconds(tokenResponse.AccessTokenExpiresIn);
                }
                if (!string.IsNullOrWhiteSpace(tokenResponse.RefreshToken))
                {
                    //claims.Add(new Claim("refresh_token", tokenResponse.RefreshToken));
                    //claims.Add(new Claim("refresh_token_expires_at", (DateTime.UtcNow.AddSeconds(tokenResponse.RefreshTokenExpiresIn)).ToString()));

                    objQBTokendtlDTO.RefreshToken = tokenResponse.RefreshToken;
                    objQBTokendtlDTO.RefreshTokenExpireDate = DateTime.UtcNow.AddSeconds(tokenResponse.RefreshTokenExpiresIn);
                }
                if (!string.IsNullOrWhiteSpace(code))
                    objQBTokendtlDTO.Code = code;
                else
                    objQBTokendtlDTO.Code = string.Empty;
                if (!string.IsNullOrWhiteSpace(realmId))
                    objQBTokendtlDTO.RealmCompanyID = Convert.ToInt64(realmId);
                else
                    objQBTokendtlDTO.RealmCompanyID = 0;

                objQBTokendtlDTO.Created = DateTimeUtility.DateTimeNow;
                objQBTokendtlDTO.LastUpdated = DateTimeUtility.DateTimeNow;

                objQBTokendtlDTO.CreatedBy = SessionHelper.UserID;
                objQBTokendtlDTO.LastUpdatedBy = SessionHelper.UserID;
                objQBTokendtlDTO.AddedFrom = "Web";
                objQBTokendtlDTO.EditedFrom = "Web";
                objQBTokendtlDTO.IsDeleted = false;
                objQBTokendtlDTO.IsArchived = false;
                objQBTokendtlDTO.IsSuccess = true;

                if (!string.IsNullOrWhiteSpace(tokenResponse.AccessToken) && objQBTokendtlDTO.RealmCompanyID > 0)
                    objQBTokendtlDTO.QuickBookCompanyName = GetQBCompanyName(objQBTokendtlDTO);
                else
                    objQBTokendtlDTO.QuickBookCompanyName = string.Empty;

                if (!string.IsNullOrWhiteSpace(objQBTokendtlDTO.AccessToken) && !string.IsNullOrWhiteSpace(objQBTokendtlDTO.RefreshToken))
                {
                    objQBTokenDetailDAL.InsertTokenDetail(objQBTokendtlDTO);
                    // Insert Item for NewSetup
                    QuickBookItemDAL objQBItemDAL = new QuickBookItemDAL(SessionHelper.EnterPriseDBName);
                    objQBItemDAL.InsertItemQuickBookSetup(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);

                    // Insert WorkOrder for NewSetup
                    QuickBookWorkOrderDAL objQBWorkOrderDAL = new QuickBookWorkOrderDAL(SessionHelper.EnterPriseDBName);
                    objQBWorkOrderDAL.InsertWorkOrderQuickBookSetup(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID,"From Quick Book Setup","Insert WorkORder");

                    QuickBookOrderDAL objQBOrderDAL = new QuickBookOrderDAL(SessionHelper.EnterPriseDBName);
                    objQBOrderDAL.InsertOrderQuickBookSetup(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, "From Quick Book Setup", "Insert Order");

                    //Insert Last Sync History ID if New setup enterprise wise
                    QuickBookTokenDetailDAL objQBLastSyncDetailDAL = new QuickBookTokenDetailDAL(SessionHelper.EnterPriseDBName);
                    objQBLastSyncDetailDAL.InsertQBLastSyncDetails(SessionHelper.EnterPriceID);
                }
            }
            else
            {
                objQBTokendtlDTO.IsSuccess = true;
                if (!string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
                {
                    objQBTokendtlDTO.AccessToken = tokenResponse.AccessToken;
                    objQBTokendtlDTO.AccessTokenExpireDate = DateTime.UtcNow.AddSeconds(tokenResponse.AccessTokenExpiresIn);
                }
                else
                {
                    objQBTokendtlDTO.AccessToken = string.Empty;
                    //objQBTokendtlDTO.AccessTokenExpireDate = null;
                    objQBTokendtlDTO.IsSuccess = false;
                }
                if (!string.IsNullOrWhiteSpace(tokenResponse.RefreshToken))
                {
                    objQBTokendtlDTO.RefreshToken = tokenResponse.RefreshToken;
                    objQBTokendtlDTO.RefreshTokenExpireDate = DateTime.UtcNow.AddSeconds(tokenResponse.RefreshTokenExpiresIn);
                }
                else
                {
                    objQBTokendtlDTO.RefreshToken = string.Empty;
                    //objQBTokendtlDTO.RefreshTokenExpireDate = null;
                    objQBTokendtlDTO.IsSuccess = false;
                }

                if (!string.IsNullOrWhiteSpace(code))
                    objQBTokendtlDTO.Code = code;
                else
                    objQBTokendtlDTO.Code = string.Empty;

                if (!string.IsNullOrWhiteSpace(realmId))
                    objQBTokendtlDTO.RealmCompanyID = Convert.ToInt64(realmId);
                else
                    objQBTokendtlDTO.RealmCompanyID = 0;

                objQBTokendtlDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                objQBTokendtlDTO.LastUpdatedBy = SessionHelper.UserID;
                objQBTokendtlDTO.EditedFrom = "Web";
                if(objQBTokendtlDTO.IsSuccess == false && !string.IsNullOrWhiteSpace(Error))
                    objQBTokendtlDTO.AuthError = Error;
                else
                    objQBTokendtlDTO.AuthError = string.Empty;
                objQBTokendtlDTO.IsDeleted = false;
                objQBTokendtlDTO.IsArchived = false;

                if (!string.IsNullOrWhiteSpace(tokenResponse.AccessToken) && objQBTokendtlDTO.RealmCompanyID > 0)
                    objQBTokendtlDTO.QuickBookCompanyName = GetQBCompanyName(objQBTokendtlDTO);
                else
                    objQBTokendtlDTO.QuickBookCompanyName = string.Empty;

                objQBTokenDetailDAL.UpdateTokenDetail(objQBTokendtlDTO);
            }

            
            return objQBTokendtlDTO;

        }

        public string GetQBCompanyName(QuickBookTokenDetailDTO objQBTokendtlDTO)
        {
            string StrCompanyName = string.Empty;
            try
            {

                if (objQBTokendtlDTO != null && !string.IsNullOrWhiteSpace(objQBTokendtlDTO.AccessToken))
                {
                    OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(objQBTokendtlDTO.AccessToken);

                    ServiceContext serviceContext = new ServiceContext(objQBTokendtlDTO.RealmCompanyID.ToString(), IntuitServicesType.QBO, oauthValidator);
                    serviceContext.IppConfiguration.MinorVersion.Qbo = SiteSettingHelper.QBVersion;

                    QueryService<CompanyInfo> querySvc = new QueryService<CompanyInfo>(serviceContext);
                    CompanyInfo objCompanyInfo = querySvc.ExecuteIdsQuery("SELECT * FROM CompanyInfo").FirstOrDefault();
                    if (objCompanyInfo != null)
                        StrCompanyName = objCompanyInfo.CompanyName;
                }
            }
            catch (Exception ex)
            {
                CommonUtility.LogError(ex, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);
            }
            return StrCompanyName;
        }

        public ActionResult SetupInfo()
        {
            if (SessionHelper.UserType == 1 || SessionHelper.RoleID == -1)
            {
                string strAuthError = TempData["AuthError"] == null ? string.Empty : Convert.ToString(TempData["AuthError"]);
                if (strAuthError.ToLower() == "access_denied")
                    strAuthError = "Access Denied Permission";

                ViewBag.AccessToken = TempData["AccessToken"];
                ViewBag.AuthError = strAuthError;

                if (ViewBag.AccessToken == null && ViewBag.AuthError == string.Empty)
                    return RedirectToAction("QuickBookSetup", "Master");
                else
                    return View();
            }
            else
            {
                return RedirectToAction(ActName, CtrlName);
            }

        }

    }
}