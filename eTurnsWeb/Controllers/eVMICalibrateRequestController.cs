using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eTurns.DTO;
using eTurnsWeb.Helper;
using eTurnsWeb.Controllers.WebAPI;
using eTurns.DTO.Resources;
using System.Net.Http;
using System.Collections;
using eTurnsWeb.Models;
using System.Web.Script.Serialization;
using System.Text;
using System.Reflection;
using Dynamite;
using Dynamite.Extensions;
using Dynamite.Data.Extensions;
using eTurns.DAL;
using System.Net;
using System.Configuration;

namespace eTurnsWeb.Controllers
{
    public class eVMICalibrateRequestController : eTurnsControllerBase
    {
        string CtrlName = Convert.ToString(ConfigurationManager.AppSettings["CtrlName"]);
        string ActName = Convert.ToString(ConfigurationManager.AppSettings["ActName"]);
        //
        // GET: /TareRequest/

        [HttpGet]
        public ActionResult CalibrateRequestList()
        {
            var isSensorBinsRFIDeTags = SessionHelper.GetAdminPermission(SessionHelper.ModuleList.SensorBinsRFIDeTags);

            if (SessionHelper.UserType == 1 && SessionHelper.RoleID == -1 && isSensorBinsRFIDeTags)
            {
                return View();
            }
            else
            {
                return RedirectToAction(ActName, CtrlName);
            }
        }

        public ActionResult GetCalibrateRequestList(JQueryDataTableParamModel param)
        {

            //int PageIndex = int.Parse(param.sEcho);
            //int PageSize = param.iDisplayLength;
            //var sortDirection = Request["sSortDir_0"];
            //var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            //var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            //var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            //var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            // string sDirection = string.Empty;
            // int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"].ToString();
            // bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            // bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());

            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "ID desc";
            }
            else
            {
                sortColumnName = "ID desc";
            }

            //string searchQuery = string.Empty;
            int TotalRecordCount = 0;

            //QuickListDAL controller = new QuickListDAL(SessionHelper.EnterPriseDBName);
            ItemLocationCalibrateRequestDAL objDAL = new ItemLocationCalibrateRequestDAL(SessionHelper.EnterPriseDBName);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            List<ItemLocationCalibrateRequestDTO> DataFromDB = objDAL.GetCalibrateRequestListByPaging(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone);

            //return Json(new { sEcho = param.sEcho, iTotalRecords = TotalRecordCount, iTotalDisplayRecords = TotalRecordCount, aaData = result, }, JsonRequestBehavior.AllowGet);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetCalibrateRequestNarrwSearchHTML()
        {
            return PartialView("_CalibrateNarrowSearch");

        }

        public JsonResult GetNarrowSearchData()
        {

            ItemLocationCalibrateRequestDAL ILPRDAL = new ItemLocationCalibrateRequestDAL(SessionHelper.EnterPriseDBName);

            var objNarrowSearch = ILPRDAL.GetCalibrateListNarrowSearchData(SessionHelper.CompanyID, SessionHelper.RoomID);
            return Json(new { Success = true, Message = ResCommon.MsgDataSuccessfullyGet, Data = objNarrowSearch }, JsonRequestBehavior.AllowGet);
        }

    }// class

}// controller
