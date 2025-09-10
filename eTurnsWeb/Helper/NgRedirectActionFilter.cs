using eTurns.DTO;
using eTurnsMaster.DAL;
using System.Collections.Generic;
using System.Web.Mvc;

namespace eTurnsWeb.Helper
{
    public class NgRedirectActionFilter : ActionFilterAttribute, IActionFilter
    {
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            var Controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var Action = filterContext.ActionDescriptor.ActionName;
            var currentMVCURL = "/" + Controller + "/" + Action;
            //List<string> redirectAngularURLs = new List<string> { "/pull/pullmasterlist", "/pull/newpull", "/master/inventoryanalysis", "/master/dashboard" };
            var strictRedirction = SiteSettingHelper.StrictRediretion;
            
            if (!string.IsNullOrEmpty(strictRedirction) && !string.IsNullOrWhiteSpace(strictRedirction) && strictRedirction.ToLower() == "yes")
            {
                CommonMasterDAL commonMasterDAL = new CommonMasterDAL();
                var result = commonMasterDAL.GetSiteUrlSetting(currentMVCURL);

                if (result != null && !string.IsNullOrEmpty(result.NgNLFUrl) && !string.IsNullOrWhiteSpace(result.NgNLFUrl) && result.StrictRediretion && result.NgNLFUrl.ToLower() != currentMVCURL.ToLower())
                {
                    filterContext.Result = new RedirectResult(result.NgNLFUrl);
                    return;
                }
            }

            OnActionExecuting(filterContext);

        }
    }
}
