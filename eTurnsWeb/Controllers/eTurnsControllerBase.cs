using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml.Linq;
using System.Reflection;
using eTurns.DTO;
using eTurnsWeb.Helper;

namespace eTurnsWeb.Controllers
{
    [NgRedirectActionFilter]
    [ValidateInput(false)]
    public class eTurnsControllerBase : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                string[] arrExceptActions = new string[] { "forgotpassword", "userloginauthanticatemasterdatabase", "resetpassword", "sendpasswordwithcaptcha", "GetNgNLFUrl" , "eturnsnewsblogs" };
                if (filterContext != null && filterContext.ActionDescriptor != null && !string.IsNullOrWhiteSpace(filterContext.ActionDescriptor.ActionName) && filterContext.ActionDescriptor.ActionName.ToLower() != "userloginauthanticatemasterdatabase" && !arrExceptActions.Contains(filterContext.ActionDescriptor.ActionName.ToLower()))
                {
                    if (Session["EnterPriseDBName"] == null)
                    {
                        FormsAuthentication.SignOut();
                        filterContext.Result = new JsonResult
                        {
                            Data = new
                            {
                                // put a message which sentto the client

                                message = "eturnssessiontimeoutoccur"
                            },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };
                    }
                }
            }
            if (HttpContext.Request.Cookies.AllKeys.Contains("timezoneoffset"))
            {
                Session["timezoneoffset"] =
                    HttpContext.Request.Cookies["timezoneoffset"].Value;
            }
            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {

            if (eTurnsWeb.Helper.SessionHelper.RoomID <= 0)
            {
                string from_settings = string.Empty;
                string[] arrExceptController = new string[] { "reportbuilder" };
                string[] arrExceptActions = new string[] { "createtemplate" };

                //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));

                from_settings = SiteSettingHelper.ExceptActionsForNoRoom; // Settinfile.Element("ExceptActionsForNoRoom").Value;

                if (!string.IsNullOrEmpty(from_settings))
                {
                    arrExceptActions = from_settings.Split(',');
                }

                if (filterContext != null && filterContext.ActionDescriptor != null
                && !string.IsNullOrWhiteSpace(filterContext.ActionDescriptor.ActionName)
                && filterContext.ActionDescriptor.ActionName.ToLower() != "userloginauthanticatemasterdatabase"
                && arrExceptActions.Contains(filterContext.ActionDescriptor.ActionName.ToLower()))
                {
                    var descriptor = filterContext.ActionDescriptor;
                    var controllerName = descriptor.ControllerDescriptor.ControllerName;

                    filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary { { "Controller", "Master" }, { "Action", "MyProfile" } }); ;

                }
            }

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                string[] arrExceptActions = new string[] { "forgotpassword", "userloginauthanticatemasterdatabase", "resetpassword", "sendpasswordwithcaptcha", "eturnsnewsblogs" };
                if (filterContext != null && filterContext.ActionDescriptor != null && !string.IsNullOrWhiteSpace(filterContext.ActionDescriptor.ActionName) && filterContext.ActionDescriptor.ActionName.ToLower() != "userloginauthanticatemasterdatabase" && !arrExceptActions.Contains(filterContext.ActionDescriptor.ActionName.ToLower()))
                {
                    if (Session["EnterPriseDBName"] == null)
                    {
                        FormsAuthentication.SignOut();
                        filterContext.Result = new JsonResult
                        {
                            Data = new
                            {
                                // put a message which sentto the client

                                message = "eturnssessiontimeoutoccur"
                            },
                            JsonRequestBehavior = JsonRequestBehavior.AllowGet
                        };
                    }
                }
            }
            base.OnActionExecuted(filterContext);
        }

        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonResult()
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                MaxJsonLength = Int32.MaxValue // Use this value to set your maximum size for all of your Requests
            };
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            var action = filterContext.RequestContext.RouteData.Values["action"] as string;
            var controller = filterContext.RequestContext.RouteData.Values["controller"] as string;

            if (filterContext.Exception is HttpAntiForgeryException)
            {
                // redirect to login page if any HttpAntiForgeryException
                string redirectUrl = FormsAuthentication.LoginUrl;
                if (string.IsNullOrWhiteSpace(redirectUrl))
                {
                    redirectUrl = "/Master/UserLogin";
                }

                filterContext.ExceptionHandled = true;

                //// case 1 user is trying to login again in new tab
                //if (action == "UserLogin" &&
                //controller == "Master" &&
                //filterContext.RequestContext.HttpContext.User != null &&
                //filterContext.RequestContext.HttpContext.User.Identity.IsAuthenticated &&
                //SessionHelper.UserID > 0)
                //{
                //    // redirect/show error/whatever?
                //    using (UserMasterBAL userMasterBAL = new UserMasterBAL())
                //    {
                //        redirectUrl = userMasterBAL.GetRedirectUrl(SessionHelper.UserID, true);
                //    }
                //}

                filterContext.Result = new RedirectResult(redirectUrl);
            }
        }

        protected string getResourceName(string resourceFileName, string resourceKey)
        {
            try
            {
                if (!string.IsNullOrEmpty(resourceFileName))
                {
                    Assembly SampleAssembly = Assembly.LoadFrom(Server.MapPath("~\\bin\\eTurns.DTO.dll"));
                    var arrTypes = SampleAssembly.GetTypes();
                    int index = arrTypes.ToList().FindIndex(s => s.Name == resourceFileName);
                    PropertyInfo prop = arrTypes[index].GetProperty(resourceKey);
                    if (prop != null)
                    {
                        return Convert.ToString(prop.GetValue(SampleAssembly, null));
                    }
                    else
                    {
                        return resourceKey;
                    }
                }
                else
                {
                    return resourceKey;
                }
            }
            catch (Exception ex)
            {
                return resourceKey;
            }
        }
    }
}