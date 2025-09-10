using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsWeb.Helper;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using eTurnsWeb.Helper;
using eTurns.DTO;
using eTurns.DAL;
using eTurns.DTO.Resources;


namespace eTurnsWeb.Controllers
{
    [AuthorizeHelper]
    public class DataArchivalController : eTurnsControllerBase
    {
        //
        // GET: /DataArchival/

        public ActionResult List()
        {
            return View();
        }

        public ActionResult CreateArchiveSchedule()
        {
            ViewBag.Modules = GetModuleOptions();
            var model = new ArchiveScheduleDTO { ModuleId = 1, RoomId = SessionHelper.RoomID, CompanyId = SessionHelper.CompanyID, LoadSheduleFor = 10 };
            return PartialView("_ArchiveSchedule", model);
        }

        public ActionResult EditArchiveSchedule(long ScheduleId)
        {
            ViewBag.Modules = GetModuleOptions();
            var archiveDal = new ArchiveDAL(SessionHelper.EnterPriseDBName);
            var archiveSchedule = archiveDal.GetArchiveScheduleByIdPlain(ScheduleId);

            if (archiveSchedule != null && archiveSchedule.ScheduleID > 0)
            {
                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
                archiveSchedule.NextRunDateTime = CommonUtility.ConvertDateByTimeZone(archiveSchedule.NextRunDate, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                archiveSchedule.ScheduleRunTime = !string.IsNullOrEmpty(archiveSchedule.ScheduleRunTime) && !string.IsNullOrWhiteSpace(archiveSchedule.ScheduleRunTime) ? archiveSchedule.ScheduleRunTime : "00:00";
            }

            return PartialView("_ArchiveSchedule", archiveSchedule);
        }

        public ActionResult DataArchival()
        {
            return View();
        }

        [HttpPost]
        public JsonResult DataArchivalSave(ArchiveScheduleDTO ArchiveSchedule)
        {
            string message;
            string status;

            try
            {
                if (ArchiveSchedule != null)
                {
                    var ArchiveDal = new ArchiveDAL(SessionHelper.EnterPriseDBName);

                    if (ArchiveSchedule.ScheduleID == 0)
                    {
                        ArchiveSchedule.RoomId = SessionHelper.RoomID;
                        ArchiveSchedule.CompanyId = SessionHelper.CompanyID;
                        ArchiveSchedule.CreatedBy = SessionHelper.UserID;
                        ArchiveSchedule.LastUpdatedBy = SessionHelper.UserID;
                        ArchiveSchedule.ScheduledBy = SessionHelper.UserID;
                        ArchiveSchedule.UserID = SessionHelper.UserID;
                        ArchiveSchedule.LoadSheduleFor = 10;
                        bool isScheduleExist = ArchiveDal.IsArchiveScheduleExist(ArchiveSchedule.RoomId, ArchiveSchedule.ModuleId);

                        if (isScheduleExist)
                        {
                            message = ResMessage.DuplicateArchiveSchedule;
                            status = "duplicate";
                        }
                        else
                        {
                            ArchiveDal.SaveArchiveSchedule(ArchiveSchedule);

                            if (ArchiveSchedule.ScheduleID > 0)
                            {
                                message = ResMessage.SaveMessage;
                                status = "ok";
                            }
                            else
                            {
                                message = ResMessage.SaveErrorMsg;
                                status = "fail";
                            }
                        }
                    }
                    else
                    {
                        bool isScheduleExist = ArchiveDal.IsArchiveScheduleDuplicateForEdit(ArchiveSchedule.RoomId, ArchiveSchedule.ModuleId, ArchiveSchedule.ScheduleID);

                        if (isScheduleExist)
                        {
                            message = ResMessage.DuplicateArchiveSchedule;
                            status = "duplicate";
                        }
                        else
                        {
                            ArchiveSchedule.LastUpdatedBy = SessionHelper.UserID;
                            ArchiveDal.SaveArchiveSchedule(ArchiveSchedule);
                            message = ResMessage.SaveMessage;
                            status = "ok";
                        }
                    }
                }
                else
                {
                    message = ResMessage.InvalidModel;
                    status = "fail";
                }
            }
            catch (Exception ex)
            {
                message = ResMessage.SaveErrorMsg + (Convert.ToString(ex.InnerException) ?? string.Empty);
                status = "fail";
            }

            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ArchiveScheduleListAjax(JQueryDataTableParamModel param)
        {
            UserBAL obj = new UserBAL();

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
                    sortColumnName = "ScheduleID desc";
            }
            else
                sortColumnName = "ScheduleID desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            ArchiveDAL toolWrittenOffDAL = new ArchiveDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<ArchiveScheduleDTO> DataFromDB = toolWrittenOffDAL.GetPagedArchiveSchedules(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsDeleted);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);
        }

        private List<CommonDTO> GetModuleOptions()
        {
            List<CommonDTO> ArchiveModules = new List<CommonDTO>();
            ArchiveModules.Add(new CommonDTO() { ID = 1, Text = ResourceRead.GetResourceValue("PullMaster", "ResModuleName") }); //
            ArchiveModules.Add(new CommonDTO() { ID = 2, Text = ResourceRead.GetResourceValue("Orders", "ResModuleName") }); //
            ArchiveModules.Add(new CommonDTO() { ID = 3, Text = ResourceRead.GetResourceValue("AuditTrail", "ResModuleName") });
            ArchiveModules.Add(new CommonDTO() { ID = 4, Text = ResourceRead.GetResourceValue("AuditTrailTransaction", "ResModuleName") });
            ArchiveModules.Add(new CommonDTO() { ID = 5, Text = ResourceRead.GetResourceValue("Requisitions", "ResModuleName") });
            ArchiveModules.Add(new CommonDTO() { ID = 6, Text = ResourceRead.GetResourceValue("Workorders", "ResModuleName") });
            ArchiveModules.Add(new CommonDTO() { ID = 7, Text = ResourceRead.GetResourceValue("Count", "ResModuleName") });

            return ArchiveModules;
        }

        public JsonResult ArchiveRecords(string Guids, string ModuleName)
        {
            try
            {
                ArchiveDAL archiveDAL = new ArchiveDAL(SessionHelper.EnterPriseDBName); ;
                string result = string.Empty;
                string NotificationClass = string.Empty;
                if (ModuleName.ToLower() != "pull")
                {
                    result = archiveDAL.ArchiveRecords(Guids, SessionHelper.RoomID, ModuleName, true, "Web", "Archived from " + ModuleName + " List", out NotificationClass, SessionHelper.EnterPriceID,SessionHelper.CompanyID, SessionHelper.UserID);
                }
                else
                {
                    result = archiveDAL.ArchiveRecords(Guids, SessionHelper.RoomID, ModuleName, false, "Web", "Archived from " + ModuleName + " List", out NotificationClass, SessionHelper.EnterPriceID, SessionHelper.CompanyID,SessionHelper.UserID);
                }

                return Json(new { Status = true, Message = result, NotificationClass = NotificationClass }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ResCommon.FailToArchive, NotificationClass = "errorIcon" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UnArchiveRecords(string Guids, string ModuleName)
        {
            try
            {
                ArchiveDAL archiveDAL = new ArchiveDAL(SessionHelper.EnterPriseDBName); ;
                string result = string.Empty;
                string NotificationClass = string.Empty;

                if (ModuleName.ToLower() != "pull")
                {
                    result = archiveDAL.UnarchiveRecords(Guids, SessionHelper.RoomID, ModuleName, true, out NotificationClass,SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.UserID);
                }
                else
                {
                    result = archiveDAL.UnarchiveRecords(Guids, SessionHelper.RoomID, ModuleName, false, out NotificationClass,SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.UserID);
                }
                return Json(new { Status = true, Message = result, NotificationClass = NotificationClass }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ResCommon.FailToUnarchive, NotificationClass = "errorIcon" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}

