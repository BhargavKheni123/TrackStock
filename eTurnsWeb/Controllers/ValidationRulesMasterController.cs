using eTurns.DTO;
using eTurnsWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eTurnsWeb.BAL;
using Org.BouncyCastle.Crypto.Digests;
using eTurnsWeb.Helper;
using eTurns.DAL;
using eTurns.DTO.Resources;

namespace eTurnsWeb.Controllers
{
   
    public class ValidationRulesMasterController : Controller
    {

        public ValidationRulesMasterController()
        { 
        
        }


        //
        // GET: /ValidationRulesMaster/

        public ActionResult Index()
        {

            if (!(SessionHelper.UserType == 1 && SessionHelper.RoleID == -1))
            {
                return RedirectToAction("MyProfile", "Master");
            }

            ValidationRulesMasterModel model = new ValidationRulesMasterModel();

            
            using (ValidationRuleModulesBAL modulesBAL = new ValidationRuleModulesBAL())
            {

                List<ValidationRuleModulesDTO> moduleList = modulesBAL.GetAll();
                var selList = (from m in moduleList
                              select
                              new { ID = m.ID,
                                  DTOName = m.ModulePage + " ( " + m.DTOName.Split('_')[0] + " )"
                              }).ToList();

                model.DTOList = new SelectList(selList, "ID", "DTOName", "");
            }

            string[] resFiles = Enum.GetNames(typeof(ResourceFilesEnum));
            List<SelectListItem> resFilesList = new List<SelectListItem>();
            for (int i = 0; i < resFiles.Length; i++)
            {
                resFilesList.Add(new SelectListItem() { Text = resFiles[i], Value = resFiles[i] });
            }

            model.ResourceFiles = new SelectList(resFilesList.OrderBy( o => o.Text), "Value", "Text", "");

            //string[] vnames = Enum.GetNames(typeof(ValidationRuleType));

            //List<SelectListItem> vlist = new List<SelectListItem>();
            //for (int i = 0; i < vnames.Length; i++)
            //{
            //    vlist.Add(new SelectListItem() { Text = vnames[i], Value = vnames[i] });
            //}
            //model.RuleTypeList = vlist;

            return View(model);
        }

        [HttpGet]
        public ActionResult GetPropertyByModuleId(int moduleId )
        {
            using (ValidationRulesMasterBAL rulesMasterBAL = new ValidationRulesMasterBAL())
            {
                var list = rulesMasterBAL.GetByModuleID(moduleId);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetRulesMasterByID(long id)
        {
            using (ValidationRulesMasterBAL rulesMasterBAL = new ValidationRulesMasterBAL())
            {
                var obj = rulesMasterBAL.GetByID(id);
                return Json(new { data = obj }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AddUpdate(ValidationRulesMasterDTO dto)
        {
            if (dto.DTOName == "None" || string.IsNullOrWhiteSpace(dto.DTOProperty))
            {
                return Json(new { data = ResCommon.InvalidDateToSave });
            }

            //try
            //{
                using (ValidationRulesMasterBAL rulesMasterBAL = new ValidationRulesMasterBAL())
                {
                    rulesMasterBAL.InsertUpdate(dto);
                    return Json(new { data = "success" });
                }
            //}
            //catch (Exception ex)
            //{
            //    throw;
            //    //return Json(new { data = "fail" });
            //}
        }

        [HttpPost]
        public JsonResult DeleteRule(long id)
        {
            if (id <= 0)
            {
                return Json(new { status = string.Format(ResCommon.MsgInvalid, ResItemMaster.ID) });
            }

            using (ValidationRulesMasterBAL rulesMasterBAL = new ValidationRulesMasterBAL())
            {
                rulesMasterBAL.Delete(id);
                return Json(new { status = ResMessage.DeletedSuccessfully });
            }
        }

        [HttpPost]
        public JsonResult RefreshRoomRules(int moduleId)
        {
            if (moduleId <= 0)
            {
                return Json(new { status = ResCommon.InvalidModuleId }); 
            }

            using (ValidationRulesMasterBAL rulesMasterBAL = new ValidationRulesMasterBAL())
            {
                int roomsUpdated = rulesMasterBAL.SyncRoomRules(moduleId, DateTimeUtility.DateTimeNow, SessionHelper.LoggedinUser.ID);
                return Json(new { status = string.Format(ResCommon.RoomRecordsUpdated, roomsUpdated) }); 
            }
        }

    }
}
