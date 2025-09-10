using eTurns.DAL;
using System;
using System.Web.Mvc;

namespace eTurnsWeb.Controllers
{
    public class SessionHandlerController : eTurnsControllerBase
    {

        int SESSION_TIMEOUT_SECONDS = 1200;
        int SESSION_TIMEOUT_WARNING_WITHIN_SECONDS = 1100;

        [HttpGet]
        [OutputCache(Duration = 0, VaryByParam = "None")]
        public ActionResult CheckSession(bool keepAlive)
        {
            var model = new CheckSessionModel();

            var now = DateTimeUtility.DateTimeNow;
            var expireDate = (DateTime)Session["PageTimeoutDate"];
            var nearExpireDate = expireDate.AddSeconds(-(SESSION_TIMEOUT_SECONDS - SESSION_TIMEOUT_WARNING_WITHIN_SECONDS));

            if (keepAlive)
            {
                Session["PageTimeoutDate"] = DateTimeUtility.DateTimeNow.AddSeconds(SESSION_TIMEOUT_SECONDS); ;
                model.Success = true;
            }
            else
            {
                model.TimedOut = now > expireDate;
                model.NearTimedOut = now > nearExpireDate;
            }

            model.Now = now.ToString("HH:mm:ss");
            model.ExpireDate = expireDate.ToString("HH:mm:ss");
            model.NearExpireDate = nearExpireDate.ToString("HH:mm:ss");

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult Timeout()
        {

            Session["Timeout"] = true;
            return RedirectToRoute(new
            {
                controller = "Master",
                action = "UserLogin"
            });
        }

    }
    public class CheckSessionModel
    {
        public bool Authenticated { get; set; }
        public bool TimedOut { get; set; }
        public bool NearTimedOut { get; set; }
        public bool Success { get; set; }
        public string Now { get; set; }
        public string ExpireDate { get; set; }
        public string NearExpireDate { get; set; }
    }
}
