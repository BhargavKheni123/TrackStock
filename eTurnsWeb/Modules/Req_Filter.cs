using eTurnsWeb.Helper;
using System.Web.Mvc;

namespace eTurnsWeb.Modules
{
    public class LoggingFilterAttribute : System.Web.Mvc.ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.ActionDescriptor.ActionName.ToLower() != "userlogin" && SessionHelper.CompanyID < 1)
            {

                filterContext.HttpContext.Server.Transfer("~/Master/UserLogin");

            }
            //filterContext.HttpContext.Trace.Write("(Logging Filter)Action Executing: " +
            //    filterContext.ActionDescriptor.ActionName);

            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Exception != null)
                filterContext.HttpContext.Trace.Write("(Logging Filter)Exception thrown");

            base.OnActionExecuted(filterContext);
        }
    }
}