using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Xml.Linq;

namespace eTurnsWeb.Helper
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ValidateAntiForgeryTokenOnAllPosts : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            string ActionName = filterContext.ActionDescriptor.ActionName;
            string ControllerName = filterContext.Controller.ControllerContext.RouteData.Values["controller"].ToString();

            if (ControllerName == "Saml2" && ActionName == "Acs")
            {
                // If Okta single sign on response then ignore
                return;
            }
            if (ActionName.ToLower().Equals("refresh") && ControllerName.ToLower().Equals("defaultcaptcha"))
            {
                filterContext.Result = new RedirectResult("~/Master/MyProfile");
                return;
            }

            try
            {

                //XElement Settinfile = XElement.Load(filterContext.HttpContext.Server.MapPath("/SiteSettings.xml")); 
                string MethodsToIgnoreXSRF = eTurns.DTO.SiteSettingHelper.MethodsToIgnoreXSRF; //Settinfile.Element("MethodsToIgnoreXSRF").Value;
                string ValidateAntiForgeryTokenOnAllPosts = eTurns.DTO.SiteSettingHelper.ValidateAntiForgeryTokenOnAllPosts; // Settinfile.Element("ValidateAntiForgeryTokenOnAllPosts").Value;
                List<string> lstMethodsToIgnoreXSRF = (MethodsToIgnoreXSRF ?? string.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim().ToLower()).ToList();


                if ((ValidateAntiForgeryTokenOnAllPosts ?? string.Empty).ToLower() != "yes")
                {
                    return;
                }

                if (lstMethodsToIgnoreXSRF.Contains((ActionName ?? string.Empty).ToLower()))
                {
                    return;
                }

                // Only validate POSTs  
                if (request.HttpMethod == WebRequestMethods.Http.Post)
                {
                    // Ajax POSTs and normal form posts have to be treated differently when it comes  
                    // to validating the AntiForgeryToken  
                    if (request.IsAjaxRequest())
                    {
                        HttpCookie antiForgeryCookieAngular = request.Cookies.Get("__RequestVerificationToken_Angular");
                        HttpCookie antiForgeryCookie = request.Cookies.Get(AntiForgeryConfig.CookieName);
                        string applicationRequest = request.Headers.Get("ApplicationRequest");


                        var cookieValue = antiForgeryCookie != null ? antiForgeryCookie.Value : null;

                        if (antiForgeryCookieAngular != null && applicationRequest == "Angular")
                        {
                            cookieValue = antiForgeryCookieAngular.Value;
                        }
                        string tokenfromHiddenOrHeader = request.Headers["__RequestVerificationToken"];
                        if (String.IsNullOrWhiteSpace(tokenfromHiddenOrHeader))
                        {
                            tokenfromHiddenOrHeader = request.Form["__RequestVerificationToken"];
                        }
                        if (!string.IsNullOrWhiteSpace(tokenfromHiddenOrHeader))
                        {
                            AntiForgery.Validate(cookieValue, tokenfromHiddenOrHeader);
                        }
                        else
                        {
                            if (!ActionName.ToLower().Equals("Refresh".ToLower()) && !ControllerName.ToLower().Equals("DefaultCaptcha".ToLower()))
                                new ValidateAntiForgeryTokenAttribute().OnAuthorization(filterContext);
                        }
                    }
                    else
                    {
                        new ValidateAntiForgeryTokenAttribute().OnAuthorization(filterContext);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonUtility.SaveeTurnsAuthorizationError(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.UserName, ActionName, ControllerName, "AntiForgery", Convert.ToString(ex));
                throw;
            }
        }
    }
}