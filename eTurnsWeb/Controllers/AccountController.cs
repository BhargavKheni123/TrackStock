using eTurns.ABAPIBAL.Helper;
using eTurns.DAL;
using eTurns.DTO;
using eTurnsWeb.Helper;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Web;
using System.Web.Mvc;
namespace eTurnsWeb.Controllers
{

    public class AccountController : Controller
    {
        public ActionResult AccessDeniedPage()
        {
            //RegionInfo obj = new RegionInfo()

            return View();
        }
        public ActionResult GenericError()
        {
            //RegionInfo obj = new RegionInfo()

            return View();
        }
        public ActionResult InternalError()
        {
            return View();
        }
        public ActionResult PageNotFound()
        {
            return View();
        }

        public ActionResult knowkoutDemo()
        {
            return View();
        }

        public ActionResult DemoDTJq()
        {

            return View();
        }
        public ActionResult DropDownListEdit()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetProducts(string term)
        {
            term = term ?? string.Empty;
            List<SelectListItem> lstValues = new List<SelectListItem>();

            for (int i = 0; i < 200; i++)
            {

                lstValues.Add(new SelectListItem() { Text = RandomString(7, false), Value = Convert.ToString(i) });
            }
            lstValues = lstValues.Where(t => t.Text.ToLower().Contains(term.ToLower())).ToList();
            return Json(lstValues);

        }
        private string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 1; i < size + 1; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            else
                return builder.ToString();
        }
        public ActionResult ABGetApp(string amazon_callback_uri, string amazon_state, string auth_code, string state, string applicationId)
        {
            string messgeLog = string.Empty;
            messgeLog = messgeLog + "amazon_callback_uri:" + (amazon_callback_uri ?? "NA") + Environment.NewLine;
            messgeLog = messgeLog + "amazon_state:" + (amazon_state ?? "NA") + Environment.NewLine;
            messgeLog = messgeLog + "auth_code:" + (auth_code ?? "NA") + Environment.NewLine;
            messgeLog = messgeLog + "state:" + (state ?? "NA") + Environment.NewLine;
            eTurns.ABAPIBAL.Helper.CommonFunctions.SaveLogInTextFile(messgeLog);
            try
            {
                ABSetUpDAL objABSetUpDALQB = new ABSetUpDAL(DbConnectionHelper.GeteTurnsQuickBookDBName());
                ABSetUpDAL objABSetUpDALLogging = new ABSetUpDAL(DbConnectionHelper.GeteTurnsLoggingDBName());
                GetABAppRequestLogDTO objGetABAppRequestLogDTO = new GetABAppRequestLogDTO();
                ABRoomStoreSettingDTO objABRoomStoreSettingDTO = new ABRoomStoreSettingDTO();
                ABMarketPlaceDTO objABMarketPlaceDTO = new ABMarketPlaceDTO();
                if (!string.IsNullOrWhiteSpace(auth_code) && !string.IsNullOrWhiteSpace(state))
                {
                    objGetABAppRequestLogDTO = objABSetUpDALLogging.UpdateGetABAppRequestLogAuthCode(auth_code, state);
                    if (objGetABAppRequestLogDTO.ID > 0)
                    {
                        amazon_callback_uri = amazon_callback_uri ?? objGetABAppRequestLogDTO.amazon_callback_uri;
                        amazon_state = amazon_state ?? objGetABAppRequestLogDTO.amazon_state;
                        string enterpriseDBName = ((SessionHelper.EnterPriseDBName ?? string.Empty) == string.Empty) ? DbConnectionHelper.GeteTurnsDBName() : (SessionHelper.EnterPriseDBName ?? string.Empty);
                        ABRoomStoreSettingDAL objABRoomStoreSettingDAL = new ABRoomStoreSettingDAL(enterpriseDBName);

                        objABRoomStoreSettingDTO = objABRoomStoreSettingDAL.GetABRoomStoreSetting(SessionHelper.CompanyID, SessionHelper.RoomID);
                        if (objABRoomStoreSettingDTO == null || objABRoomStoreSettingDTO.ID < 1)
                        {
                            objABRoomStoreSettingDTO = objABRoomStoreSettingDAL.InsertABRoomStoreSettingDefaults(SessionHelper.CompanyID, SessionHelper.RoomID);
                        }                        
                        Uri myUri = new Uri(amazon_callback_uri);
                        string Abhost = myUri.Host;
                        Abhost = "https://" + Abhost;

                        objABMarketPlaceDTO = objABSetUpDALQB.GetABMarketPlaceByDomain(Abhost);
                        objABRoomStoreSettingDTO = objABRoomStoreSettingDAL.EditABRoomStoreSettingMPAndAuth(objABRoomStoreSettingDTO.ID, auth_code, objABMarketPlaceDTO.ABLocale, objABMarketPlaceDTO.CountryCode, objABMarketPlaceDTO.MarketplaceID);

                        //objABRoomStoreSettingDTO = objABRoomStoreSettingDAL.EditABRoomStoreSettingAuthCode(objABRoomStoreSettingDTO.ID, auth_code,);
                        ABTokenDetailDTO objABTokenDetailDTO = ABAPIHelper.GenerateTokensbyAuthCode(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriseDBName, SessionHelper.CurrentDomainURL);
                        //ABAPIHelper.SendAbIntimationOfAuthCode(amazon_callback_uri, amazon_state, "auth_code_use_successful");
                        if (SessionHelper.RoomID > 0)
                        {
                            RoomDAL objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
                            objRoomDAL.SetRoomAllowABIntegration(SessionHelper.RoomID, true);
                            SessionHelper.AllowABIntegration = true;
                        }
                        if (objABTokenDetailDTO.ID > 0)
                        {
                            return RedirectToAction("ABAccountSetup", "Master", new { amazon_callback_uri = amazon_callback_uri, amazon_state = amazon_state, status = "auth_code_use_successful" });
                        }
                        else
                        {
                            return RedirectToAction("ABAccountSetup", "Master", new { amazon_callback_uri = amazon_callback_uri, amazon_state = amazon_state, status = "auth_code_use_failure" });
                        }

                    }
                }
                //else
                //{
                //string eTurns_state = Guid.NewGuid().ToString("N");
                //if (!string.IsNullOrWhiteSpace(HttpContext.User.Identity.Name) && SessionHelper.EnterPriceID > 0)
                //{
                //    if (!string.IsNullOrWhiteSpace(amazon_callback_uri) && !string.IsNullOrWhiteSpace(amazon_state) && !string.IsNullOrWhiteSpace(eTurns_state) && !string.IsNullOrWhiteSpace(applicationId))
                //    {
                //        string AbCentralOauthURL = SessionHelper.CurrentDomainURL + Url.Action("ABGetApp", "Account");
                //        objGetABAppRequestLogDTO = new GetABAppRequestLogDTO();
                //        objGetABAppRequestLogDTO.Updated = DateTime.UtcNow;
                //        objGetABAppRequestLogDTO.Created = DateTime.UtcNow;
                //        objGetABAppRequestLogDTO.eTurns_state = eTurns_state;
                //        objGetABAppRequestLogDTO.auth_code = string.Empty;
                //        objGetABAppRequestLogDTO.UserName = HttpContext.User.Identity.Name;
                //        objGetABAppRequestLogDTO.amazon_callback_uri = amazon_callback_uri;
                //        objGetABAppRequestLogDTO.amazon_state = amazon_state;
                //        objGetABAppRequestLogDTO.applicationId = applicationId;
                //        objGetABAppRequestLogDTO.UserID = SessionHelper.UserID;
                //        objGetABAppRequestLogDTO.EnterpriseID = SessionHelper.EnterPriceID;
                //        objGetABAppRequestLogDTO.CompanyID = SessionHelper.CompanyID;
                //        objGetABAppRequestLogDTO.RoomID = SessionHelper.RoomID;
                //        objGetABAppRequestLogDTO = objABSetUpDALLogging.InsertOrUpdateGetABAppRequestLogByUserID(objGetABAppRequestLogDTO);
                //        if (SessionHelper.IsLicenceAccepted && SessionHelper.HasPasswordChanged && SessionHelper.NewEulaAccept && SessionHelper.AnotherLicenceAccepted)
                //        {
                //            string RedirectbackToAbURL = amazon_callback_uri + "?version=beta&redirect_uri=" + AbCentralOauthURL + "&amazon_state=" + amazon_state + "&state=" + eTurns_state + "&status=authentication_successful";
                //            return Redirect(RedirectbackToAbURL);
                //        }
                //    }
                //}
                //else
                //{
                //    eTurns.ABAPIBAL.Helper.CommonFunctions.SaveLogInTextFile("NOT LOGED IN USER");
                //    if (!string.IsNullOrWhiteSpace(amazon_callback_uri) && !string.IsNullOrWhiteSpace(amazon_state) && !string.IsNullOrWhiteSpace(applicationId))
                //    {
                //        objGetABAppRequestLogDTO = new GetABAppRequestLogDTO();
                //        objGetABAppRequestLogDTO.Updated = DateTime.UtcNow;
                //        objGetABAppRequestLogDTO.Created = DateTime.UtcNow;
                //        objGetABAppRequestLogDTO.eTurns_state = eTurns_state;
                //        objGetABAppRequestLogDTO.auth_code = string.Empty;
                //        objGetABAppRequestLogDTO.UserName = HttpContext.User.Identity.Name;
                //        objGetABAppRequestLogDTO.amazon_callback_uri = amazon_callback_uri;
                //        objGetABAppRequestLogDTO.amazon_state = amazon_state;
                //        objGetABAppRequestLogDTO.applicationId = applicationId;
                //        objGetABAppRequestLogDTO.UserID = SessionHelper.UserID;
                //        objGetABAppRequestLogDTO.EnterpriseID = SessionHelper.EnterPriceID;
                //        objGetABAppRequestLogDTO.CompanyID = SessionHelper.CompanyID;
                //        objGetABAppRequestLogDTO.RoomID = SessionHelper.RoomID;
                //        objGetABAppRequestLogDTO = objABSetUpDALLogging.InsertGetABAppRequestLog(objGetABAppRequestLogDTO);
                //        string eTurnslandingPage = TSAppConfigHelper.AbSignupLandingPageURL + "?amazon_callback_uri=" + amazon_callback_uri + "&amazon_state=" + amazon_state + "&applicationId=" + applicationId + "&state=" + eTurns_state;
                //        return Redirect(eTurnslandingPage);
                //    }
                //}
                //}
                return View();
            }
            catch (Exception ex)
            {
                eTurns.ABAPIBAL.Helper.CommonFunctions.SaveExceptionInTextFile("AccountController.ABGetApp", ex);
                return View();
            }
        }


        public ActionResult AbConsentReminder()
        {
            return View();
        }

        public ActionResult ChangePasswordDone()
        {
            if (SessionHelper.UserID > 0)
            {
                ABSetUpDAL objABSetUpDAL = new ABSetUpDAL(DbConnectionHelper.GeteTurnsLoggingDBName());
                GetABAppRequestLogDTO objGetABAppRequestLogDTO = objABSetUpDAL.FetchGetABAppRequestLog(SessionHelper.UserID);
                if (objGetABAppRequestLogDTO != null && objGetABAppRequestLogDTO.ID > 0)
                {
                    string AbCentralRedirectURL = SessionHelper.CurrentDomainURL + Url.Action("ABGetApp", "Account");
                    if (objGetABAppRequestLogDTO != null && objGetABAppRequestLogDTO.ID > 0 && !string.IsNullOrWhiteSpace(objGetABAppRequestLogDTO.amazon_state) && !string.IsNullOrWhiteSpace(objGetABAppRequestLogDTO.amazon_callback_uri) && !string.IsNullOrWhiteSpace(objGetABAppRequestLogDTO.eTurns_state))
                    {
                        string RedirectbackToAbURL = objGetABAppRequestLogDTO.amazon_callback_uri + "?redirect_uri=" + AbCentralRedirectURL + "&amazon_state=" + objGetABAppRequestLogDTO.amazon_state + "&state=" + objGetABAppRequestLogDTO.eTurns_state + "&status=authentication_successful";
                        return Redirect(RedirectbackToAbURL);
                    }
                }
                ZohoDAL zohoDAL = new ZohoDAL(DbConnectionHelper.GeteTurnsLoggingDBName());
                if (zohoDAL.izZohoUser(SessionHelper.UserID) && (!SessionHelper.HasChangedFirstPassword))
                {
                    return RedirectToAction("QuickStartGuide", "HelpDocument");
                }
                if(!SessionHelper.HasChangedFirstPassword)
                {
                    return RedirectToAction("ItemMasterList", "Inventory");
                }
            }

            return RedirectToAction("MyProfile", "Master");


        }

    }

    public class IDTEXT
    {
        public string id { get; set; }
        public string text { get; set; }
    }
}
