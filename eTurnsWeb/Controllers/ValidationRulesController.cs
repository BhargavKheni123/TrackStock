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
    public class ValidationRulesController : eTurnsControllerBase
    {
        //
        // GET: /ValidationRules/
        [HttpGet]
        public ActionResult RoomRules()
        {
            if (!(SessionHelper.UserType == 1 && SessionHelper.RoleID == -1))
            {
                return RedirectToAction("MyProfile", "Master");
            }

            RoomRulesModel model = new RoomRulesModel();

            using (ValidationRuleModulesBAL modulesBAL = new ValidationRuleModulesBAL())
            {
                model.ModuleList = new SelectList(modulesBAL.GetAll(), "ID", "ModulePage", "");
            }
 
            return View(model);
        }

        [HttpGet]
        public JsonResult GetColumns(int moduleId)
        {
            bool IsAccess = (SessionHelper.RoleID < 0 && SessionHelper.UserType == 1);

            if (IsAccess)
            {
                string ColumnName = string.Empty;
                ValidationRuleModulesDTO objModuleDTO = null;
                List<ValidationRulesDTO> validationRulesDTOList = null;

                using (ValidationRuleModulesBAL modulesBAL = new ValidationRuleModulesBAL())
                {
                    objModuleDTO = modulesBAL.GetByID(moduleId);
                }

                using (ValidationRulesBAL bal = new ValidationRulesBAL())
                {
                    validationRulesDTOList = bal.GetByModuleId(SessionHelper.EnterPriceID, SessionHelper.CompanyID,
                        SessionHelper.RoomID, moduleId).ToList();
                }

                if (validationRulesDTOList != null)
                {

                    //JQueryTableJSONDTO myDeserializedObjList = (JQueryTableJSONDTO)Newtonsoft.Json.JsonConvert.DeserializeObject(objSiteListMasterDTO.JSONDATA, typeof(JQueryTableJSONDTO));
                    for (int i = 0; i < validationRulesDTOList.Count(); i++)
                    {
                        var col = validationRulesDTOList.ToList()[i];
                        col.ColumnResName = getResourceName(Convert.ToString(col.ResourceFileName), Convert.ToString(col.DTOProperty));
                        if (string.IsNullOrWhiteSpace(col.ColumnResName))
                        {
                            col.ColumnResName = col.DTOProperty;
                        }
                    }
                }

                return Json(new { ColumnList = validationRulesDTOList, ModuleDetail = objModuleDTO }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { ColumnList = new List<ValidationRulesDTO>(), ModuleDetail = new ValidationRuleModulesDTO() }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveRules(List<ValidationRulesDTO> rulesDTOs)
        {
            if (rulesDTOs == null || rulesDTOs.Count() == 0 || string.IsNullOrWhiteSpace(rulesDTOs[0].DTOProperty))
            {
                return Json(new { status = ResCommon.InvalidDateToSave }); 
            }

            using (ValidationRulesBAL rulesBAL = new ValidationRulesBAL())
            {
                var isSave = rulesBAL.SaveRules(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, rulesDTOs,SessionHelper.LoggedinUser.ID,DateTimeUtility.DateTimeNow);
                string status = isSave ? ResCommon.RecordsSavedSuccessfully : ResNarrowSearch.SomeErrorHasOccurred;
                return Json(new { status = status });
            }
        }

        [HttpPost]
        public JsonResult DeleteRule(long id)
        {
            if (id <= 0)
            {
                return Json(new { status = string.Format(ResCommon.MsgInvalid, ResItemMaster.ID) }); 
            }

            using (ValidationRulesBAL rulesBAL = new ValidationRulesBAL())
            {
                rulesBAL.Delete(id, SessionHelper.RoomID);
                return Json(new { status = ResMessage.DeletedSuccessfully });
            }
        }


    }//class
}//ns
