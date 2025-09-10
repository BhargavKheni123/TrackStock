using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsMaster.DAL;
using eTurnsWeb.BAL;
using eTurnsWeb.Helper;
using eTurnsWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static eTurns.DTO.Enums;

namespace eTurnsWeb.Controllers
{
    [AuthorizeHelper]
    public class BillingTypeModulesController : eTurnsControllerBase
    {
        //
        // GET: /RoomBillingModules/

        [HttpGet]
        public ActionResult Index()
        {
            if (!((SessionHelper.UserType == 1 && SessionHelper.RoleID == -1) || (SessionHelper.UserType == 2 && SessionHelper.RoleID == -2)))
            {
                return RedirectToAction("MyProfile", "Master");
            }

            BillingTypeModuleViewModel model = new BillingTypeModuleViewModel();
            model.BillingRoomType = 3; //(int)BillingRoomTypeEnum.Manage;

            using (BillingRoomTypeMasterBAL bal = new BillingRoomTypeMasterBAL())
            {
                List<EnterpriseDTO> entList = bal.GetAllEnterprise();
                if (SessionHelper.UserType == 1 && SessionHelper.RoleID == -1)
                {
                    model.EnterpriseList = new SelectList(entList, "ID", "Name");

                    List<BillingRoomTypeMasterDTO> list = bal.GetBillingRoomTypeMaster(entList[0].ID);
                    model.BillingRoomTypeList = new SelectList(list, "ID", "ResourceValue");
                }
                else if (SessionHelper.UserType == 2 && SessionHelper.RoleID == -2)
                {
                    entList = entList.Where(x => x.ID == Convert.ToInt64(SessionHelper.EnterPriceID)).ToList();
                    List<BillingRoomTypeMasterDTO> list = bal.GetBillingRoomTypeMaster(entList[0].ID);

                    model.BillingRoomTypeList = new SelectList(list, "ID", "ResourceValue");

                    model.EnterpriseList = new SelectList(entList, "ID", "Name");
                }
            }


            using (BillingRoomTypeModulesMapBAL billRoomMap = new BillingRoomTypeModulesMapBAL())
            {
                model.ModuleMapping = billRoomMap.GetBillingRoomTypeModulesMap(model.BillingRoomType.Value);
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult GetBillingRoomTypeModulesMap(int billRoomType)
        {

            using (BillingRoomTypeModulesMapBAL billRoomMap = new BillingRoomTypeModulesMapBAL())
            {
                List<BillingRoomTypeModulesMapDTO> list = billRoomMap.GetBillingRoomTypeModulesMap(billRoomType);
                return Json(new
                {
                    list = list
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetBillingRoomTypeByEnt(long EntId)
        {
            using (BillingRoomTypeMasterBAL bal = new BillingRoomTypeMasterBAL())
            {
                List<BillingRoomTypeMasterDTO> list = bal.GetBillingRoomTypeMaster(EntId);
                return Json(new
                {
                    list = list
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SaveBillingRoomTypeMaster(Int64 EnterpriseID, string newBillingRoomTypeName, List<BillingRoomTypeModulesMapDTO> list)
        {
            using (BillingRoomTypeModulesMapBAL billRoomMap = new BillingRoomTypeModulesMapBAL())
            {
                int BillingRoomTypeID = billRoomMap.SaveBillingRoomTypeMaster(newBillingRoomTypeName, Convert.ToInt64(EnterpriseID));
                //string message = BillingRoomTypeID > 0 ? ResCommon.MsgSaveBillingType : ResCommon.ErrorInProcess;
                bool b = false;
                string message = ResCommon.ErrorInProcess;
                string status = "";
                if (BillingRoomTypeID == -2)
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResRoomMaster.BillingRoomTypeName, newBillingRoomTypeName);  //"BinNumber \"" + objDTO.BinNumber + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else  if (BillingRoomTypeID > 0)
                {
                    list.ForEach(i => i.BillingRoomTypeID = BillingRoomTypeID);
                    b = billRoomMap.SaveBillingRoomModuleMap(list);
                    message = b ? ResCommon.MsgSaveBillingType : ResCommon.ErrorInProcess;
                    status = b ? "0" : "-1";
                }
                return Json(new
                {
                    status = status, // -1 => error
                    message = message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SaveBillingRoomModuleMap(List<BillingRoomTypeModulesMapDTO> list)
        {
            using (BillingRoomTypeModulesMapBAL billRoomMap = new BillingRoomTypeModulesMapBAL())
            {
                bool b = billRoomMap.SaveBillingRoomModuleMap(list);
                string message = b ? ResCommon.MsgSaveBillingType : ResCommon.ErrorInProcess;
                return Json(new
                {
                    status = b ? 0 : -1, // -1 => error
                    message = message
                }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public ActionResult GetBillingRoomModules(long roomID, long compId, long entId)
        {

            using (BillingRoomTypeModulesMapBAL billRoomMap = new BillingRoomTypeModulesMapBAL())
            {
                var list = billRoomMap.GetBillingRoomModules(roomID, compId, entId);
                return Json(new
                {
                    list = list
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult eTurnsBilling(int? BillingRoomTypeId, long? EnterpriseId)
        {
            if (!((SessionHelper.UserType == 1 && SessionHelper.RoleID == -1) || (SessionHelper.UserType == 2 && SessionHelper.RoleID == -2)))
            {
                return RedirectToAction("MyProfile", "Master");
            }

            eTurnsBillingDAL eTurnsBillingDAL = new eTurnsBillingDAL(MasterDbConnectionHelper.GeteTurnsDBName());
            eTurnsBillingViewModel model = new eTurnsBillingViewModel();
            model.BillingRoomTypeId = BillingRoomTypeId.HasValue ? BillingRoomTypeId.Value : 3; //(int)BillingRoomTypeEnum.Manage;
            
            if (EnterpriseId.HasValue)
            {
                model.EnterpriseID = EnterpriseId.Value;
            }

            var billingRoomTypeCostDetail = eTurnsBillingDAL.GetBillingRoomTypeCostMaster(model.BillingRoomTypeId.Value);

            if (billingRoomTypeCostDetail != null && billingRoomTypeCostDetail.BillingRoomTypeId.HasValue && billingRoomTypeCostDetail.BillingRoomTypeId > 0)
            {
                model.BaseCost = billingRoomTypeCostDetail.BaseCost;
                model.OneTimeLicenceFee = billingRoomTypeCostDetail.OneTimeLicenceFee;
                model.Grouping = billingRoomTypeCostDetail.Grouping;
                model.EnterpriseID=billingRoomTypeCostDetail.EnterpriseID;
            }

            using (BillingRoomTypeMasterBAL bal = new BillingRoomTypeMasterBAL())
            {
                List<EnterpriseDTO> entList = bal.GetAllEnterprise();

                if (SessionHelper.UserType == 1 && SessionHelper.RoleID == -1)
                {
                    model.EnterpriseList = new SelectList(entList, "ID", "Name");
                    List<BillingRoomTypeMasterDTO> list = bal.GetBillingRoomTypeMaster(entList[0].ID);
                    model.BillingRoomTypeList = new SelectList(list, "ID", "ResourceValue");
                }
                else if (SessionHelper.UserType == 2 && SessionHelper.RoleID == -2)
                {
                    entList = entList.Where(x => x.ID == Convert.ToInt64(SessionHelper.EnterPriceID)).ToList();
                    List<BillingRoomTypeMasterDTO> list = bal.GetBillingRoomTypeMaster(entList[0].ID);
                    model.BillingRoomTypeList = new SelectList(list, "ID", "ResourceValue");
                    model.EnterpriseList = new SelectList(entList, "ID", "Name");
                }
            }

            using (BillingRoomTypeModulesMapBAL billRoomMap = new BillingRoomTypeModulesMapBAL())
            {
                model.ModuleMapping = eTurnsBillingDAL.GetBillingRoomTypeModuleMaster(model.BillingRoomTypeId.Value);
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult GetBillingRoomTypeModuleMaster(int billRoomType)
        {
            eTurnsBillingDAL eTurnsBillingDAL = new eTurnsBillingDAL(MasterDbConnectionHelper.GeteTurnsDBName());

            using (BillingRoomTypeModulesMapBAL billRoomMap = new BillingRoomTypeModulesMapBAL())
            {
                var list = eTurnsBillingDAL.GetBillingRoomTypeModuleMaster(billRoomType);
                return Json(new
                {
                    list = list
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SaveeTurnsBilling(int BillingRoomTypeId, List<BillingRoomTypeModuleMasterDTO> List, double BaseCost, double OneTimeLicenceFee , byte Grouping)
        {
            eTurnsBillingDAL eTurnsBillingDAL = new eTurnsBillingDAL(MasterDbConnectionHelper.GeteTurnsDBName());
            bool b = eTurnsBillingDAL.SaveBillingRoomModuleAndCost(BillingRoomTypeId,BaseCost,OneTimeLicenceFee,Grouping, List, SessionHelper.UserID);
            string message = b ? ResCommon.MsgSaveBillingType : ResCommon.ErrorInProcess;

            return Json(new
            {
                status = b ? 0 : -1, // -1 => error
                message = message
            }, JsonRequestBehavior.AllowGet);
            
        }
    }
}
