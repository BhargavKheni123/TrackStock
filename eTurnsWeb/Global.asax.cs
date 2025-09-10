using CaptchaMvc.Infrastructure;
using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsMaster.DAL;
using eTurnsWeb.Helper;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;
using eTurnsWeb.validation;
using System.Data.Entity.Infrastructure.Interception;

namespace eTurnsWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        System.Timers.Timer perSecTimer = new System.Timers.Timer();
        protected void Application_Start()
        {
            MvcHandler.DisableMvcResponseHeader = true;
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ModelValidatorProviders.Providers.Add(new DynamicModelValidatorProvider());
            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;

            BundleTable.EnableOptimizations = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["EnableOptimizations"]);
            perSecTimer = new System.Timers.Timer(10000);
            perSecTimer.Elapsed += perSecTimer_Elapsed;
            perSecTimer.Enabled = true;

            DbInterception.Add(new EFCommandInterceptor());
            //HttpConfiguration config = GlobalConfiguration.Configuration;
            //config.Formatters.JsonFormatter.SerializerSettings.StringEscapeHandling = Newtonsoft.Json.StringEscapeHandling.EscapeHtml;

            // Increase max Json length  
            //foreach (var factory in ValueProviderFactories.Factories)
            //{
            //    if (factory is JsonValueProviderFactory)
            //    {
            //        ValueProviderFactories.Factories.Remove(factory as JsonValueProviderFactory);
            //        break;
            //    }
            //}
            //ValueProviderFactories.Factories.Add(new CustomJsonValueProviderFactory());
        }

        void perSecTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (RedCircleStatic.SignalGroups != null)
                {
                    if (RedCircleStatic.SignalGroups.Where(t => t.IsProcessed == false).Any())
                    {
                        RedCircleStatic.SignalGroups.ForEach(it =>
                        {
                            GlobalHost.ConnectionManager.GetHubContext<eTurnsHub>().Clients.Group(it.EID + "_" + it.CID + "_" + it.RID).UpdateRedCircleCountInClients();
                            it.IsProcessed = true;
                        });
                        RedCircleStatic.SignalGroups.RemoveAll(t => t.IsProcessed == true);
                    }
                }

                //if (HttpRuntime.Cache["LastCalledEach"] != null)
                //{
                //    if (HttpRuntime.Cache["LastCalledEach"].ToString() != "0")
                //    {

                //        XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));
                //        string DelayRedCount = Settinfile.Element("DelayRedCount").Value;
                //        DateTime LastCalledEach = Convert.ToDateTime(HttpRuntime.Cache["LastCalledEach"]);
                //        double delayToset = string.IsNullOrWhiteSpace(Convert.ToString(DelayRedCount)) ? 5000 : Convert.ToDouble(DelayRedCount);
                //        if ((DateTime.UtcNow - LastCalledEach).TotalMilliseconds > 5000)
                //        {
                //            long EID = 0;
                //            long CID = 0;
                //            long RID = 0;

                //            long.TryParse(Convert.ToString(HttpRuntime.Cache["Signal_EnterPriceID"]), out EID);
                //            long.TryParse(Convert.ToString(HttpRuntime.Cache["Signal_CompanyID"]), out CID);
                //            long.TryParse(Convert.ToString(HttpRuntime.Cache["Signal_RoomID"]), out RID);
                //            if (RedCircleStatic.SignalGroups != null)
                //            {
                //                RedCircleStatic.SignalGroups.Where(t => t.IsProcessed == false).ToList().ForEach(it =>
                //                {
                //                    GlobalHost.ConnectionManager.GetHubContext<eTurnsHub>().Clients.Group(it.EID + "_" + it.CID + "_" + it.RID).UpdateRedCircleCountInClients();
                //                    it.IsProcessed = true;
                //                });
                //                RedCircleStatic.SignalGroups = RedCircleStatic.SignalGroups.Where(t => t.IsProcessed == false).ToList();
                //            }
                //            //GlobalHost.ConnectionManager.GetHubContext<eTurnsHub>().Clients.All.UpdateRedCircleCountInClients();

                //            HttpRuntime.Cache["LastCalledEach"] = 0;
                //        }
                //    }
                //}
            }
            catch
            {

            }
        }
        protected void Session_Start(object src, EventArgs e)
        {
            CaptchaUtils.CaptchaManager.StorageProvider = new CookieStorageProvider();
            EnterpriseMasterDAL objEnterpriseMasterDAL = new EnterpriseMasterDAL();
            List<EnterpriseDomainDTO> lstDomains = objEnterpriseMasterDAL.GetAllEnterpriseDomains();

            if (HttpContext.Current != null && lstDomains != null && lstDomains.Count > 0)
            {
                string currentdomain = string.Empty;
                if (HttpContext.Current.Request != null && HttpContext.Current.Request.Url.Host != null)
                {
                    currentdomain = HttpContext.Current.Request.Url.Host.ToLower();
                    if (lstDomains.Any(t => t.DomainURL.ToLower() == currentdomain))
                    {
                        EnterpriseDomainDTO objEnterpriseDomainDTO = lstDomains.First(t => t.DomainURL.ToLower() == currentdomain);

                        if (HttpContext.Current.Session != null)
                        {
                            HttpContext.Current.Session.Add("EnterpriseResourceFolder", Convert.ToString(objEnterpriseDomainDTO.EnterpriseID));
                            ResourceHelper.EnterpriseResourceFolder = Convert.ToString(objEnterpriseDomainDTO.EnterpriseID);
                        }
                    }
                }

            }
        }

        protected void Session_End(object src, EventArgs e)
        {
            //ConnectionMapping<string> _connections = new ConnectionMapping<string>();

            //IEnumerable<string> lstcids = _connections.GetConnections(HttpContext.Current.User.Identity.Name);
            //if (lstcids != null && lstcids.Count() > 0)
            //{
            //    GlobalHost.ConnectionManager.GetHubContext<eTurnsHub>().Clients.Client(lstcids.First()).OnSapphireSessionTimout();
            //}

        }
        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            //if (!Request.IsSecureConnection && !Request.IsLocal)
            //{
            //    Response.Redirect(Request.Url.AbsoluteUri.Replace("http://", "https://"));
            //}
            SameSiteCookieRewriter.FilterSameSiteNone(sender);
        }
        protected void Application_PostAuthorizeRequest()
        {
            if (IsWebApiRequest())
            {
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            }
        }

        private const string _WebApiPrefix = "api";
        private static string _WebApiExecutionPath = String.Format("~/{0}", _WebApiPrefix);
        private static bool IsWebApiRequest()
        {
            return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith(_WebApiExecutionPath);
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception Ex = Server.GetLastError();
            ReportSchedulerError objReportSchedulerError = new ReportSchedulerError();
            Int64 RoomID = -1;
            Int64 CompID = -1;
            Int64 EntID = -1;
            Int64 UserID = -1;
            if (HttpContext.Current.Session != null)
            {
                RoomID = SessionHelper.RoomID;
                CompID = SessionHelper.CompanyID;
                EntID = SessionHelper.EnterPriceID;
                UserID = SessionHelper.UserID;
            }
            objReportSchedulerError.CompanyID = CompID;
            objReportSchedulerError.EnterpriseID = EntID;
            objReportSchedulerError.RoomID = RoomID;
            objReportSchedulerError.UserID = UserID;
            objReportSchedulerError.Exception = Convert.ToString(Ex) ?? "Null exception";
            if (HttpContext.Current.Request != null && HttpContext.Current.Request.Url != null)
            {
                if (HttpContext.Current.Request.UrlReferrer != null && !string.IsNullOrWhiteSpace(HttpContext.Current.Request.UrlReferrer.AbsoluteUri))
                {
                    objReportSchedulerError.Exception = objReportSchedulerError.Exception + "UrlReferrer.AbsoluteUri" + "------" + (HttpContext.Current.Request.UrlReferrer.AbsoluteUri ?? string.Empty);
                    objReportSchedulerError.Exception = objReportSchedulerError.Exception + "UrlReferrer.Absolutepath:" + "------" + (HttpContext.Current.Request.UrlReferrer.AbsolutePath ?? string.Empty);
                }
                objReportSchedulerError.Exception = objReportSchedulerError.Exception + " " + HttpContext.Current.Request.Url.ToString();
            }
            objReportSchedulerError.ID = 0;
            objReportSchedulerError.NotificationID = 0;
            objReportSchedulerError.ScheduleFor = 0;
            new eTurnsMaster.DAL.CommonMasterDAL().SaveNotificationError(objReportSchedulerError);
            if (Ex != null)
            {
                Type exptype = Ex.GetType();
                if (exptype != null)
                {
                    if (exptype.Name == "HttpAntiForgeryException")
                    {

                    }
                }
            }

        }


        //protected void Application_AcquireRequestState(Object sender, EventArgs e)
        //{
        //    var currentPath = HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.ToLower();

        //    // filter out requests we are not interested in
        //    switch (Path.GetExtension(currentPath))
        //    {
        //        //case "": // no extension
        //        //    if (!currentPath.ToLower().Contains("sessionhandler/checksession"))
        //        //        //Session["PageTimeoutDate"] = DateTime.Now.AddSeconds(SESSION_TIMEOUT_SECONDS);
        //        //    break;

        //        //default:
        //        //    break;
        //    }
        //}

        //public void Application_AcquireRequestState(object sender, EventArgs e)
        //{
        //    if (HttpContext.Current.Session != null && eTurns.DTO.Resources.ResourceHelper.CurrentCult != null)
        //    {
        //        string currentCulture = Convert.ToString(eTurns.DTO.Resources.ResourceHelper.CurrentCult);
        //        if (String.Compare(currentCulture, System.Threading.Thread.CurrentThread.CurrentCulture.ToString(), StringComparison.OrdinalIgnoreCase) != 0)
        //        {
        //            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
        //            try
        //            {
        //                System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture(currentCulture);
        //            }
        //            catch
        //            {
        //                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-us");
        //            }
        //        }
        //    }
        //}
    }
}
