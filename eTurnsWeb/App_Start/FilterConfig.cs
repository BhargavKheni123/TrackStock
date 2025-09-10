using eTurnsWeb.Helper;
using System.Web.Mvc;

namespace eTurnsWeb
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ValidateAntiForgeryTokenOnAllPosts());
            //XElement Settinfile = XElement.Load(HttpContext.Current.Server.MapPath("/SiteSettings.xml"));
            //string ValidateAntiForgeryTokenOnAllPosts = Settinfile.Element("ValidateAntiForgeryTokenOnAllPosts").Value;
            //if (ValidateAntiForgeryTokenOnAllPosts == "yes")
            //{
            //    filters.Add(new ValidateAntiForgeryTokenOnAllPosts());
            //}

            //filters.Add(new LoggingFilterAttribute());
        }

        //internal static void RegisterHttpFilters(HttpFilterCollection filters)
        //{
        //    filters.Add(new ElmahHandledErrorLoggerFilter());

        //}
    }
}