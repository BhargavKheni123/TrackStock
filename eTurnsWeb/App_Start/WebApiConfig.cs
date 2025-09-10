using System.Web.Http;

namespace eTurnsWeb
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
              name: "APIWithActionMethods",
              routeTemplate: "api/{controller}/{action}/{id}",
              defaults: new { id = RouteParameter.Optional }
          );
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

        }
    }
}
