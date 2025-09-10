using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsWeb.Helper;
using System.Net;
using System.Web.Mvc;

namespace eTurnsWeb.Controllers
{
    [AuthorizeHelper]
    public class eVMISetupController : eTurnsControllerBase
    {
        public ActionResult eVMISetup()
        {
            eVMISetupDAL objeVMIDAL = new eVMISetupDAL(SessionHelper.EnterPriseDBName);
            return View("eVMISetup", objeVMIDAL.GetRecord(SessionHelper.RoomID, SessionHelper.CompanyID));
        }


        public JsonResult eVMISetupSave(eVMISetupDTO objDTO)
        {
            string message = "";
            string status = "";
            eVMISetupDAL obj = new eVMISetupDAL(SessionHelper.EnterPriseDBName);
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.Room = SessionHelper.RoomID;

            if (!ModelState.IsValid)
            {
                message = ResMessage.InvalidModel;// "Invalid Data!";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            if (objDTO.ID > 0)
            {
                if (obj.Edit(objDTO))
                {
                    message = ResMessage.SaveMessage;
                    status = "ok";
                }
                else
                {
                    message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                    status = "fail";
                }
            }
            else
            {
                if (obj.Insert(objDTO))
                {
                    message = ResMessage.SaveMessage;
                    status = "ok";
                }
                else
                {
                    message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                    status = "fail";
                }
            }
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult LoadScheduleParams(int LoadSheduleFor)
        {
            SchedulerDTO objSchedulerDTO = null;
            NotificationDAL objNotificationDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
            eVMISetupDAL objeVMIDAL = new eVMISetupDAL(SessionHelper.EnterPriseDBName);
            eVMISetupDTO objeVMIDTO = objeVMIDAL.GetRecord(SessionHelper.RoomID, SessionHelper.CompanyID);

            objSchedulerDTO = objNotificationDAL.GetScheduleByRoomScheduleFor(SessionHelper.RoomID, SessionHelper.CompanyID, (int)eVMIScheduleFor.eVMISchedule);

            if (objeVMIDTO != null && objSchedulerDTO != null && objeVMIDTO.ID > 0 && objeVMIDTO.NextPollDate != null)
            {
                eTurns.DAL.RegionSettingDAL objRegionSettingDAL = new eTurns.DAL.RegionSettingDAL(SessionHelper.EnterPriseDBName);
                objSchedulerDTO.NextRunDateTime = eTurnsWeb.Helper.CommonUtility.ConvertDateByTimeZone(objeVMIDTO.NextPollDate, eTurnsWeb.Helper.SessionHelper.CurrentTimeZone, eTurnsWeb.Helper.SessionHelper.DateTimeFormat, eTurnsWeb.Helper.SessionHelper.RoomCulture, true);
                objeVMIDTO.NextRunDateTime = objSchedulerDTO.NextRunDateTime;
                objeVMIDTO.IsActiveSchedule = objSchedulerDTO.IsScheduleActive;
                //objSchedulerDTO.ScheduleRunTime = objeVMIDTO.NextPollDate.HasValue ? objeVMIDTO.NextPollDate.Value.ToString(@"hh\:mm") : "00:00";
                objSchedulerDTO.ScheduleRunTime = objSchedulerDTO.ScheduleTime.HasValue ? objSchedulerDTO.ScheduleTime.Value.ToString(@"hh\:mm") : "00:00";
            }
            
            
            if (objSchedulerDTO == null)
            {
                objSchedulerDTO = new SchedulerDTO();
                objSchedulerDTO.RoomId = SessionHelper.RoomID;
                objSchedulerDTO.CompanyId = SessionHelper.CompanyID;
                objSchedulerDTO.LoadSheduleFor = (int)eVMIScheduleFor.eVMISchedule;
                objSchedulerDTO.ScheduleMode = 4;

            }
            else
            {
                objSchedulerDTO.LoadSheduleFor = (int)eVMIScheduleFor.eVMISchedule;
            }

            return PartialView("eVMISchedulerInfo", objSchedulerDTO);
        }

    }
}


