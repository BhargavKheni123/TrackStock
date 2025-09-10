using System.Web.Mvc;
using System.Web.Routing;

namespace eTurnsWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.aspx/{*pathInfo}");
            routes.IgnoreRoute("{*chartimg}", new { chartimg = @".*/ChartImg\.axd(/.*)?" });
            routes.MapPageRoute("ReportRoute", "Reports/{foldername}/{reportname}", "~/Reports/ReportViewer.aspx");
            routes.MapRoute(
                name: "ForCache",
                url: "GenerateCache",
                defaults: new { controller = "GenerateCache", action = "GenerateCache", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { enterprisename = UrlParameter.Optional, controller = "Master", action = "UserDefaultLogin", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "epDefault",
                url: "ep/{enterprisename}/{controller}/{action}/{id}",
                defaults: new { enterprisename = string.Empty, controller = "Master", action = "Dashboard", id = UrlParameter.Optional }
            );

            //AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
        }
    }
}