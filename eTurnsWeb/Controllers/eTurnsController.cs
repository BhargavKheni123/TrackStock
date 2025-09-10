using eTurns.DTO;
using eTurnsMaster.DAL;
using eTurnsWeb.Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace eTurnsWeb.Controllers
{
    public class eTurnsController : eTurnsControllerBase
    {
        #region [Log in]

        public ActionResult SapphireLogin()
        {
            ViewBag.LoginWithSelection = "0";
            ViewBag.IsMasterDbLogin = "0";
            if (ConfigurationManager.AppSettings.AllKeys.Contains("loginwithselection"))
            {
                ViewBag.LoginWithSelection = ConfigurationManager.AppSettings["loginwithselection"];
            }
            if (ConfigurationManager.AppSettings.AllKeys.Contains("EnableMasterLogin"))
            {
                ViewBag.IsMasterDbLogin = ConfigurationManager.AppSettings["EnableMasterLogin"];
            }

            EnterPriseUserMasterDAL objEnterPriseUserMasterDAL = new EnterPriseUserMasterDAL();
            //UserMasterController objUserMasterController = new UserMasterController();
            IEnumerable<UserMasterDTO> lstUsers = null;
            if (ViewBag.IsMasterDbLogin == "1")
            {
                lstUsers = objEnterPriseUserMasterDAL.GetAllUsers();
                ViewBag.AllUsers = lstUsers.ToList();
            }
            else
            {
                eTurns.DAL.UserMasterDAL objUserMasterController = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                //lstUsers = objUserMasterController.GetAllUsers();
                ViewBag.AllUsers = lstUsers.ToList();
            }
            return View();
        }

        #endregion
    }
}
