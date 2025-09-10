using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsMaster.DAL;
using eTurnsWeb.BAL;
using eTurnsWeb.Helper;
using eTurnsWeb.Models;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace eTurnsWeb.Controllers
{
    public class UserNarrowSearchSettingsController : Controller
    {
        //
        // GET: /UserNarrowSearchSettings/

        [HttpGet]
        public JsonResult GetUserNarrowSearchSettings(string listName)
        {
            using (UserNarrowSearchSettingsBAL searchSettingsBAL = new UserNarrowSearchSettingsBAL())
            {
                var obj = searchSettingsBAL.GetUserNarrowSearchSettings(listName);
                return Json(new { obj = obj }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveUserNarrowSearchSettings(UserNarrowSearchSettingsDTO obj)
        {
            using (UserNarrowSearchSettingsBAL searchSettingsBAL = new UserNarrowSearchSettingsBAL())
            {
                var ret = searchSettingsBAL.SaveUserNarrowSearchSettings(obj);
                bool isSave = ret.Item1;
                long id = ret.Item2;
                string status = isSave ? ResNarrowSearch.FilterCriteriaSaved : ResNarrowSearch.SomeErrorHasOccurred; 
                return Json(new { status = status , ID = id }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
