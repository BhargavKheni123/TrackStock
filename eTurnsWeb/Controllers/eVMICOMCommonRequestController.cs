using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsWeb.Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eTurnsWeb.Controllers
{
    public class eVMICOMCommonRequestController : eTurnsControllerBase
    {
        //
        // GET: /eVMICOMCommonRequest/
        string CtrlName = Convert.ToString(ConfigurationManager.AppSettings["CtrlName"]);
        string ActName = Convert.ToString(ConfigurationManager.AppSettings["ActName"]);

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult eVMICOMCommonRequestList()
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

        public ActionResult GeteVMICOMCommonRequestList(JQueryDataTableParamModel param)
        {
            eVMICOMCommonRequestDTO objDTO = new eVMICOMCommonRequestDTO();
            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            string sDirection = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"].ToString();
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());

            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "ID desc";
            }
            else
                sortColumnName = "ID desc";

            string searchQuery = string.Empty;
            int TotalRecordCount = 0;

            eVMICOMCommonRequestDAL ovjDAL = new eVMICOMCommonRequestDAL(SessionHelper.EnterPriseDBName);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            IEnumerable<eVMICOMCommonRequestDTO> DataFromDB = ovjDAL.GeteVMICOMCommonRequestRecordsByPaging(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone);

            var result = from u in DataFromDB
                         select new
                         {
                             ID = u.ID,
                             //ItemNumber = u.ItemNumber,
                             //BinNumber = u.BinNumber,
                             ComPortMasterID = u.ComPortMasterID,
                             Version = u.Version,
                             ComPortName = u.ComPortName,
                             ScaleID = u.ScaleID,
                             IsComReqStarted = u.IsComReqStarted,
                             IsComCompleted = u.IsComCompleted,
                             //IsPostProcessDone = u.IsPostProcessDone,
                             ComStartTime = FnCommon.ConvertDateByTimeZone(u.ComStartTime, true, false),
                             ComCompletionTime = FnCommon.ConvertDateByTimeZone(u.ComCompletionTime, true, false),
                             //WeightReading = u.WeightReading,
                             ErrorDescription = u.ErrorDescription,
                             Created = u.Created,
                             Updated = u.Updated,
                             CreatedBy = u.CreatedBy,
                             UpdatedBy = u.UpdatedBy,
                             RoomID = u.RoomID,
                             CompanyID = u.CompanyID,
                             CreatedByName = u.CreatedByName,
                             UpdatedByName = u.UpdatedByName,
                             RoomName = u.RoomName,
                             CreatedDate = FnCommon.ConvertDateByTimeZone(u.Created, true, false),
                             UpdatedDate = FnCommon.ConvertDateByTimeZone(u.Updated, true, false),
                             RequestTypes = u.RequestTypes,
                         };

            return Json(new { sEcho = param.sEcho, iTotalRecords = TotalRecordCount, iTotalDisplayRecords = TotalRecordCount, aaData = result, }, JsonRequestBehavior.AllowGet);
       
        }

        public ActionResult GeteVMICOMCommonRequestNarrowSearchHTML()
        {
            return PartialView("_eVMICOMCommonNarrowSearch");

        }
        public JsonResult GetNarrowSearchData(bool IsDeleted, bool IsArchived)
        {
            CommonDAL objCommonCtrl = new CommonDAL(SessionHelper.EnterPriseDBName);
            var tmpsupplierIds = new List<long>();
            NarrowSearchData objNarrowSearchData = objCommonCtrl.GetNarrowSearchDataFromCache("eVMICOMCommonListMaster", SessionHelper.CompanyID, SessionHelper.RoomID, IsArchived, IsDeleted, "", tmpsupplierIds);
            return Json(new { Success = true, Message = ResCommon.MsgDataSuccessfullyGet, Data = objNarrowSearchData }, JsonRequestBehavior.AllowGet);
        }

    }
}
