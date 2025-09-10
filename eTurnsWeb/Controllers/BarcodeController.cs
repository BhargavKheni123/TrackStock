using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsWeb.Helper;
using eTurnsWeb.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace eTurnsWeb.Controllers.WebAPI
{
    public class BarcodeController : eTurnsControllerBase
    {
        /// <summary>
        /// Barcodelist
        /// </summary>
        /// <returns></returns>
        public ActionResult BarcodeList()
        {
            List<ModuleMasterDTO> lstModuleMaster = new List<ModuleMasterDTO>();
            //ModuleMasterController moduleController = new ModuleMasterController();
            ModuleMasterDAL moduleController = new ModuleMasterDAL(SessionHelper.EnterPriseDBName);
            lstModuleMaster = moduleController.GetAllModuleNormal("ModuleName ASC");
            ModuleMasterDTO objModuleMasterDTO = new ModuleMasterDTO();
            lstModuleMaster = lstModuleMaster.Where(l => l.ModuleName == "Item Master" || l.ModuleName == "Assets" || l.ModuleName == "Tool Master").ToList();
            ViewBag.ModuleMasterList = lstModuleMaster;
            return View();
        }

        /// <summary>
        /// Barcodelist
        /// </summary>
        /// <returns></returns>
        public PartialViewResult AddNewBarcodeFromModel(string moduleName, string newBarcodeText)
        {
            List<ModuleMasterDTO> lstModuleMaster = new List<ModuleMasterDTO>();
            ModuleMasterDAL moduleController = new ModuleMasterDAL(SessionHelper.EnterPriseDBName);
            lstModuleMaster = moduleController.GetAllModuleNormal("ID DESC");
            lstModuleMaster.RemoveAll(x => x.ModuleName.ToLower() != moduleName.ToLower());
            ViewBag.ModuleMasterList = lstModuleMaster;
            ViewBag.BarcodeText = newBarcodeText;
            return PartialView();
        }

        /// <summary>
        /// Get barcode list
        /// </summary>
        /// <param name="ModuleGuid"></param>
        /// <param name="ItemGuid"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult GetBarcodeListByModuleID(string ModuleGuid, string ItemGuid, string Binguid)
        {
            //BarcodeMasterController objBarCodeMasterController = new BarcodeMasterController();
            BarcodeMasterDAL objBarCodeMasterController = new BarcodeMasterDAL(SessionHelper.EnterPriseDBName);
            List<BarcodeMasterDTO> objDTO = null;
            if (!string.IsNullOrEmpty(ItemGuid))
            {
                if (!string.IsNullOrEmpty(Binguid))
                {
                    objDTO = objBarCodeMasterController.GetAllActiveRecordsByModuleID(ModuleGuid, SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.RefGUID == Guid.Parse(ItemGuid) && ((x.BinGuid == Guid.Parse(Binguid) && x.BarcodeAdded.ToLower() == "manual") || x.BarcodeAdded.ToLower() != "manual")).ToList();
                }
                else
                {
                    objDTO = objBarCodeMasterController.GetAllActiveRecordsByModuleID(ModuleGuid, SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.RefGUID == Guid.Parse(ItemGuid)).ToList();
                }

            }
            else
                objDTO = objBarCodeMasterController.GetAllActiveRecordsByModuleID(ModuleGuid, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();

            if (objDTO != null && objDTO.Count() <= 0)
                objDTO = null;

            if (objDTO != null && objDTO.Count() > 0)
            {
                objDTO.ToList().ForEach(t =>
                {
                    t.CreatedDate = CommonUtility.ConvertDateByTimeZone(t.CreatedOn, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                    t.UpdatedDate = CommonUtility.ConvertDateByTimeZone(t.UpdatedOn, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                });

            }
            return PartialView("_ModuleBarcode", objDTO);
        }

        /// <summary>
        /// Get Item detail by module id
        /// </summary>
        /// <param name="ModuleGuid"></param>
        /// <param name="ModuleName"></param>
        /// <param name="SearchText"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetItemDetailByModuleID(string ModuleGuid, string ModuleName, string SearchText)
        {
            SearchItemDTO objDTO = new SearchItemDTO(); ;
            if (Convert.ToString(ModuleName).ToLower() == "item master")
            {

                //ItemMasterController objCtrl = new ItemMasterController();
                ItemMasterDAL objCtrl = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                int totalCnt = 0;
                TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
                IEnumerable<ItemMasterDTO> objList = objCtrl.GetPagedRecordsNew(0, 10, out totalCnt, SearchText, "ID", SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Convert.ToString(SessionHelper.RoomDateFormat), SessionHelper.UserSupplierIds, CurrentTimeZone);
                if (objList != null)
                {
                    if (objList.Count() == 1)
                    {
                        objDTO.itemID = objList.ToList()[0].ID.ToString();
                        objDTO.ItemText = objList.ToList()[0].ItemNumber;
                        objDTO.ItemGuid = objList.ToList()[0].GUID.ToString();
                        objDTO.DefaultLocationName = objList.ToList()[0].DefaultLocationName.ToString();
                        objDTO.DefaultLocationGuid = objList.ToList()[0].DefaultLocationGUID ?? Guid.Empty;
                        return Json(new { IsSuccess = true, Massage = "Success", RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        objList = objList.Where(i => i.ItemNumber == SearchText).ToList();
                        if (objList != null && objList.Count() == 1)
                        {
                            objDTO.itemID = objList.ToList()[0].ID.ToString();
                            objDTO.ItemText = objList.ToList()[0].ItemNumber;
                            objDTO.ItemGuid = objList.ToList()[0].GUID.ToString();
                            objDTO.DefaultLocationName = objList.ToList()[0].DefaultLocationName.ToString();
                            objDTO.DefaultLocationGuid = objList.ToList()[0].DefaultLocationGUID ?? Guid.Empty;
                            return Json(new { IsSuccess = true, Massage = "Success", RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { IsSuccess = false, Massage = ResCommon.MsgRecordNotFound, RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    return Json(new { IsSuccess = false, Massage = ResCommon.MsgRecordNotFound, RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
                }

            }
            else if (Convert.ToString(ModuleName).ToLower() == "assets")
            {

                //ItemMasterController objCtrl = new ItemMasterController();
                AssetMasterDAL objCtrl = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
                int totalCnt = 0;
                TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
                IEnumerable<AssetMasterDTO> objList = objCtrl.GetPagedAssetMaster(0, 10, out totalCnt, SearchText, "ID", SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone);
                if (objList != null)
                {
                    if (objList.Count() == 1)
                    {
                        objDTO.itemID = objList.ToList()[0].ID.ToString();
                        objDTO.ItemText = objList.ToList()[0].AssetName;
                        objDTO.ItemGuid = objList.ToList()[0].GUID.ToString();

                        return Json(new { IsSuccess = true, Massage = "Success", RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        objList = objList.Where(a => a.AssetName == SearchText).ToList();
                        if (objList != null && objList.Count() == 1)
                        {
                            objDTO.itemID = objList.ToList()[0].ID.ToString();
                            objDTO.ItemText = objList.ToList()[0].AssetName;
                            objDTO.ItemGuid = objList.ToList()[0].GUID.ToString();

                            return Json(new { IsSuccess = true, Massage = "Success", RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { IsSuccess = false, Massage = ResCommon.MsgRecordNotFound, RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    return Json(new { IsSuccess = false, Massage = ResCommon.MsgRecordNotFound, RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
                }

            }
            else if (Convert.ToString(ModuleName).ToLower() == "kits")
            {

                //ItemMasterController objCtrl = new ItemMasterController();
                KitMasterDAL objCtrl = new KitMasterDAL(SessionHelper.EnterPriseDBName);
                int totalCnt = 0;
                var tmpsupplierIds = new List<long>();
                TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
                IEnumerable<KitMasterDTO> objList = objCtrl.GetPagedRecords(0, 10, out totalCnt, SearchText, "ID", SessionHelper.RoomID, SessionHelper.CompanyID, false, false, SessionHelper.UserID, tmpsupplierIds, Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone);
                if (objList != null && objList.Count() == 1)
                {
                    objDTO.itemID = objList.ToList()[0].ID.ToString();
                    objDTO.ItemText = objList.ToList()[0].KitPartNumber;
                    objDTO.ItemGuid = objList.ToList()[0].GUID.ToString();

                    return Json(new { IsSuccess = true, Massage = "Success", RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsSuccess = false, Massage = ResCommon.MsgRecordNotFound, RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
                }

            }
            else if (Convert.ToString(ModuleName).ToLower() == "orders")
            {

                //ItemMasterController objCtrl = new ItemMasterController();
                OrderMasterDAL objCtrl = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                IEnumerable<OrderMasterDTO> objList = null;//objCtrl.GetPagedRecords(0, 10, out totalCnt, SearchText, "ID", SessionHelper.RoomID, SessionHelper.CompanyID, false, false, OrderType.Order, SessionHelper.RoomDateFormat);
                if (objList != null && objList.Count() == 1)
                {
                    objDTO.itemID = objList.ToList()[0].ID.ToString();
                    objDTO.ItemText = objList.ToList()[0].OrderNumber;
                    objDTO.ItemGuid = objList.ToList()[0].GUID.ToString();

                    return Json(new { IsSuccess = true, Massage = "Success", RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsSuccess = false, Massage = ResCommon.MsgRecordNotFound, RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
                }

            }
            else if (Convert.ToString(ModuleName).ToLower() == "project spent")
            {
                ProjectMasterDAL objCtrl = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
                //int totalCnt = 0;
                IEnumerable<ProjectMasterDTO> objList = null;//objCtrl.GetPagedRecords(0, 10, out totalCnt, SearchText, "ID", SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Convert.ToString(SessionHelper.RoomDateFormat));
                if (objList != null && objList.Count() == 1)
                {
                    objDTO.itemID = objList.ToList()[0].ID.ToString();
                    objDTO.ItemText = objList.ToList()[0].ProjectSpendName;
                    objDTO.ItemGuid = objList.ToList()[0].GUID.ToString();

                    return Json(new { IsSuccess = true, Massage = "Success", RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsSuccess = false, Massage = ResCommon.MsgRecordNotFound, RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
                }

            }
            else if (Convert.ToString(ModuleName).ToLower() == "pull master")
            {
                PullMasterDAL objCtrl = new PullMasterDAL(SessionHelper.EnterPriseDBName);
                bool UserConsignmentAllowed = SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowConsignedCreditPull);
                IEnumerable<PullMasterViewDTO> objList = null;

                if (objList != null && objList.Count() == 1)
                {
                    objDTO.itemID = objList.ToList()[0].ID.ToString();
                    objDTO.ItemText = objList.ToList()[0].ItemNumber;
                    objDTO.ItemGuid = objList.ToList()[0].GUID.ToString();

                    return Json(new { IsSuccess = true, Massage = "Success", RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsSuccess = false, Massage = ResCommon.MsgRecordNotFound, RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
                }

            }
            else if (Convert.ToString(ModuleName).ToLower() == "quick list permission")
            {
                QuickListDAL objCtrl = new QuickListDAL(SessionHelper.EnterPriseDBName);
                int totalCnt = 0;
                TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
                IEnumerable<QuickListMasterDTO> objList = objCtrl.GetPagedRecords(0, 10, out totalCnt, SearchText, "ID", SessionHelper.RoomID, SessionHelper.CompanyID, false, false, "1,2", Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone);

                if (objList != null && objList.Count() == 1)
                {
                    objDTO.itemID = objList.ToList()[0].ID.ToString();
                    objDTO.ItemText = objList.ToList()[0].Name;
                    objDTO.ItemGuid = objList.ToList()[0].GUID.ToString();

                    return Json(new { IsSuccess = true, Massage = "Success", RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsSuccess = false, Massage = ResCommon.MsgRecordNotFound, RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
                }

            }
            else if (Convert.ToString(ModuleName).ToLower() == "receive")
            {

                OrderMasterDAL objCtrl = new OrderMasterDAL(SessionHelper.EnterPriseDBName);
                IEnumerable<OrderMasterDTO> objList = null;//objCtrl.GetPagedRecords(0, 10, out totalCnt, SearchText, "ID", SessionHelper.RoomID, SessionHelper.CompanyID, false, false, OrderType.Order, Convert.ToString(SessionHelper.RoomDateFormat));
                if (objList != null && objList.Count() == 1)
                {
                    objDTO.itemID = objList.ToList()[0].ID.ToString();
                    objDTO.ItemText = objList.ToList()[0].OrderNumber;
                    objDTO.ItemGuid = objList.ToList()[0].GUID.ToString();

                    return Json(new { IsSuccess = true, Massage = "Success", RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsSuccess = false, Massage = ResCommon.MsgRecordNotFound, RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
                }

            }
            else if (Convert.ToString(ModuleName).ToLower() == "requisitions")
            {

                RequisitionMasterDAL objCtrl = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
                //IEnumerable<RequisitionMasterDTO> objList = objCtrl.GetPagedRecords(0, 10, out totalCnt, SearchText, "ID", SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                IEnumerable<RequisitionMasterDTO> objList = new List<RequisitionMasterDTO>();
                if (objList != null && objList.Count() == 1)
                {
                    objDTO.itemID = objList.ToList()[0].ID.ToString();
                    objDTO.ItemText = objList.ToList()[0].RequisitionNumber;
                    objDTO.ItemGuid = objList.ToList()[0].GUID.ToString();

                    return Json(new { IsSuccess = true, Massage = "Success", RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsSuccess = false, Massage = ResCommon.MsgRecordNotFound, RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
                }

            }
            else if (Convert.ToString(ModuleName).ToLower() == "material staging")
            {

                MaterialStagingDAL objCtrl = new MaterialStagingDAL(SessionHelper.EnterPriseDBName);
                int totalCnt = 0;
                TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
                IEnumerable<MaterialStagingDTO> objList = objCtrl.GetPagedMaterialStagingsFromDB(0, 10, out totalCnt, SearchText, "ID", SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone);
                if (objList != null && objList.Count() == 1)
                {
                    objDTO.itemID = objList.ToList()[0].ID.ToString();
                    objDTO.ItemText = objList.ToList()[0].StagingName;
                    objDTO.ItemGuid = objList.ToList()[0].GUID.ToString();

                    return Json(new { IsSuccess = true, Massage = "Success", RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsSuccess = false, Massage = ResCommon.MsgRecordNotFound, RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
                }

            }
            else if (Convert.ToString(ModuleName).ToLower() == "tool master")
            {

                ToolMasterDAL objCtrl = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                // IEnumerable<ToolMasterDTO> objList = objCtrl.GetPagedRecords(0, 10, out totalCnt, SearchText, "ID", SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Convert.ToString(SessionHelper.RoomDateFormat));
                List<ToolMasterDTO> objList = objCtrl.GetToolByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID);
                if (objList != null && objList.Count() == 1)
                {
                    objDTO.itemID = objList.ToList()[0].ID.ToString();
                    objDTO.ItemText = objList.ToList()[0].ToolName + " - " + objList.ToList()[0].Serial;
                    objDTO.ItemGuid = objList.ToList()[0].GUID.ToString();

                    return Json(new { IsSuccess = true, Massage = "Success", RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsSuccess = false, Massage = ResCommon.MsgRecordNotFound, RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
                }

            }
            else if (Convert.ToString(ModuleName).ToLower() == "transfer")
            {

                TransferMasterDAL objCtrl = new TransferMasterDAL(SessionHelper.EnterPriseDBName);
                int totalCnt = 0;
                TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
                IEnumerable<TransferMasterDTO> objList = null;//objCtrl.GetPagedRecords(0, 10, out totalCnt, SearchText, "ID", SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone);
                if (objList != null && objList.Count() == 1)
                {
                    objDTO.itemID = objList.ToList()[0].ID.ToString();
                    objDTO.ItemText = objList.ToList()[0].TransferNumber;
                    objDTO.ItemGuid = objList.ToList()[0].GUID.ToString();

                    return Json(new { IsSuccess = true, Massage = "Success", RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsSuccess = false, Massage = ResCommon.MsgRecordNotFound, RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
                }

            }
            else if (Convert.ToString(ModuleName).ToLower() == "work orders")
            {

                WorkOrderDAL objCtrl = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
                //IEnumerable<WorkOrderDTO> objList = objCtrl.GetPagedRecords(0, 10, out totalCnt, SearchText, "ID", SessionHelper.RoomID, SessionHelper.CompanyID, false, false, "WorkOrder");
                IEnumerable<WorkOrderDTO> objList = new List<WorkOrderDTO>();
                if (objList != null && objList.Count() == 1)
                {
                    objDTO.itemID = objList.ToList()[0].ID.ToString();
                    objDTO.ItemText = objList.ToList()[0].WOName;
                    objDTO.ItemGuid = objList.ToList()[0].GUID.ToString();

                    return Json(new { IsSuccess = true, Massage = "Success", RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsSuccess = false, Massage = ResCommon.MsgRecordNotFound, RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(new { IsSuccess = false, Massage = ResBarcodeMaster.MsgProperModuleNotSelected, RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Inser new barcode
        /// </summary>
        /// <param name="ModuleGuid"></param>
        /// <param name="ItemGuid"></param>
        /// <param name="BarcopdeString"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult InserNewBarcode(string ModuleGuid, string ItemGuid, string BarcopdeString, string strModuleName, Int64 BarcodeID = 0)
        {
            //BarcodeMasterController objBarCodeMasterController = null;
            BarcodeMasterDAL objBarCodeMasterController = null;
            BarcodeMasterDTO objDTO = null;

            try
            {

                if (BarcodeID == 0)
                {
                    objBarCodeMasterController = new BarcodeMasterDAL(SessionHelper.EnterPriseDBName);
                    List<BarcodeMasterDTO> objBarcodeList = objBarCodeMasterController.GetAllActiveRecordsByModuleID(ModuleGuid, SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.BarcodeString.ToLower() == BarcopdeString.ToLower()).ToList();

                    if (objBarcodeList != null && objBarcodeList.Count() > 0)
                    {
                        return Json(new { IsSuccess = false, Massage = string.Format(ResMessage.DuplicateMessage, "Barcode", BarcopdeString), RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);

                    }

                    objDTO = new BarcodeMasterDTO()
                    {
                        ID = 0,
                        UpdatedBy = SessionHelper.UserID,
                        RoomID = SessionHelper.RoomID,
                        RefGUID = Guid.Parse(ItemGuid),
                        ModuleGUID = Guid.Parse(ModuleGuid),
                        CreatedBy = SessionHelper.UserID,
                        CompanyID = SessionHelper.CompanyID,
                        BarcodeString = BarcopdeString,
                        Action = string.Empty,
                        CreatedByName = SessionHelper.UserName,
                        CreatedOn = DateTimeUtility.DateTimeNow,
                        GUID = Guid.Empty,
                        HistoryID = 0,
                        IsArchived = false,
                        IsDeleted = false,
                        ModuleName = strModuleName,
                        RefNumber = string.Empty,
                        RoomName = SessionHelper.RoomName,
                        UpdatedByName = SessionHelper.UserName,
                        UpdatedOn = DateTimeUtility.DateTimeNow,
                        //BarcodeAdded = barCodeAdded
                    };
                    objBarCodeMasterController.Insert(objDTO);
                    return Json(new { IsSuccess = true, Massage = ResMessage.SaveMessage, RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    objBarCodeMasterController = new BarcodeMasterDAL(SessionHelper.EnterPriseDBName);
                    List<BarcodeMasterDTO> objBarcodeList = objBarCodeMasterController.GetAllActiveRecordsByModuleID(ModuleGuid, SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.BarcodeString.ToLower() == BarcopdeString.ToLower() && x.ID != BarcodeID).ToList();

                    if (objBarcodeList != null && objBarcodeList.Count() > 0)
                    {
                        return Json(new { IsSuccess = false, Massage = string.Format(ResMessage.DuplicateMessage, "Barcode", BarcopdeString), RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);

                    }

                    objDTO = new BarcodeMasterDTO()
                    {
                        ID = BarcodeID,
                        UpdatedBy = SessionHelper.UserID,
                        RoomID = SessionHelper.RoomID,
                        RefGUID = Guid.Parse(ItemGuid),
                        ModuleGUID = Guid.Parse(ModuleGuid),
                        CreatedBy = SessionHelper.UserID,
                        CompanyID = SessionHelper.CompanyID,
                        BarcodeString = BarcopdeString,
                        Action = string.Empty,
                        CreatedByName = SessionHelper.UserName,
                        CreatedOn = DateTimeUtility.DateTimeNow,
                        GUID = Guid.Empty,
                        HistoryID = 0,
                        IsArchived = false,
                        IsDeleted = false,
                        ModuleName = strModuleName,
                        RefNumber = string.Empty,
                        RoomName = SessionHelper.RoomName,
                        UpdatedByName = SessionHelper.UserName,
                        UpdatedOn = DateTimeUtility.DateTimeNow,

                    };
                    objBarCodeMasterController.Edit(objDTO);
                    return Json(new { IsSuccess = true, Massage = ResMessage.SaveMessage, RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Massage = ex.Message, RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                objBarCodeMasterController = null;
                objDTO = null;
            }

        }
        [HttpPost]
        public JsonResult InserNewBarcodeWithBin(string ModuleGuid, string ItemGuid, string BarcopdeString, string strModuleName, string BinGuid, string BinName, Int64 BarcodeID = 0, string BarcodeAdded = "", string barcodestringOld = "")
        {
            //BarcodeMasterController objBarCodeMasterController = null;
            BarcodeMasterDAL objBarCodeMasterController = null;
            BarcodeMasterDTO objDTO = null;
            BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);

            if ((!string.IsNullOrEmpty(BinName)))
            {
                BinMasterDTO objBinMasterDTOToInsert = objBinMasterDAL.GetItemBinPlain(Guid.Parse(ItemGuid), BinName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, false, "web", false, null);
                //objBinMasterDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(l => l.ItemGUID == Guid.Parse(ItemGuid) && l.IsStagingLocation == false && l.BinNumber == BinName).OrderBy(i => i.BinNumber).FirstOrDefault();
                //BinMasterDTO objBinMasterDTOToInsert = objBinMasterDAL.GetBinMasterByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(l => l.ItemGUID == Guid.Parse(ItemGuid) && l.IsStagingLocation == false && l.BinNumber == BinName).OrderBy(i => i.BinNumber).FirstOrDefault();
                // BinMasterDTO objBinMasterDTOToInsert = objBinMasterDAL.GetBinByLocationNameItemGuid(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, BinName, Guid.Parse(ItemGuid)).Where(l => l.IsStagingLocation == false).OrderBy(i => i.BinNumber).FirstOrDefault();


                //if ((BinGuid == null || BinGuid == string.Empty || string.IsNullOrEmpty(BinGuid) || objBinMasterDTOToInsert == null || Convert.ToString(objBinMasterDTOToInsert.GUID) != BinGuid) && (!string.IsNullOrEmpty(BinName)))
                //{

                //    if (objBinMasterDTOToInsert == null)
                //    {
                //        objBinMasterDTOToInsert = new BinMasterDTO();
                //        objBinMasterDTOToInsert.BinNumber = BinName.Trim();
                //        objBinMasterDTOToInsert.ParentBinId = null;
                //        objBinMasterDTOToInsert.CreatedBy = SessionHelper.UserID;
                //        objBinMasterDTOToInsert.LastUpdatedBy = SessionHelper.UserID;
                //        objBinMasterDTOToInsert.Created = DateTimeUtility.DateTimeNow;
                //        objBinMasterDTOToInsert.LastUpdated = DateTimeUtility.DateTimeNow;
                //        objBinMasterDTOToInsert.Room = SessionHelper.RoomID;
                //        objBinMasterDTOToInsert.CompanyID = SessionHelper.CompanyID;
                //        objBinMasterDTOToInsert.AddedFrom = "Web";
                //        objBinMasterDTOToInsert.EditedFrom = "Web";
                //        objBinMasterDTOToInsert.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                //        objBinMasterDTOToInsert.ReceivedOn = DateTimeUtility.DateTimeNow;
                //        objBinMasterDTOToInsert.IsOnlyFromItemUI = true;
                //        objBinMasterDTOToInsert = objBinMasterDAL.InsertBin(objBinMasterDTOToInsert);

                //    }
                //    BinGuid = Convert.ToString(objBinMasterDTOToInsert.GUID);
                //    BinMasterDTO objInventoryLocation = objBinMasterDAL.GetInventoryLocation(objBinMasterDTOToInsert.ID, Guid.Parse(ItemGuid), SessionHelper.RoomID, SessionHelper.CompanyID);

                //    if (objInventoryLocation == null)
                //    {
                //        objInventoryLocation = new BinMasterDTO();
                //        objInventoryLocation.BinNumber = objBinMasterDTOToInsert.BinNumber;
                //        objInventoryLocation.ParentBinId = objBinMasterDTOToInsert.ID;
                //        objInventoryLocation.CreatedBy = SessionHelper.UserID;
                //        objInventoryLocation.LastUpdatedBy = SessionHelper.UserID;
                //        objInventoryLocation.Created = DateTimeUtility.DateTimeNow;
                //        objInventoryLocation.LastUpdated = DateTimeUtility.DateTimeNow;
                //        objInventoryLocation.MinimumQuantity = objBinMasterDTOToInsert.MinimumQuantity;
                //        objInventoryLocation.MaximumQuantity = objBinMasterDTOToInsert.MaximumQuantity;
                //        objInventoryLocation.CriticalQuantity = objBinMasterDTOToInsert.CriticalQuantity;
                //        objInventoryLocation.eVMISensorID = objBinMasterDTOToInsert.eVMISensorID;
                //        objInventoryLocation.eVMISensorPort = objBinMasterDTOToInsert.eVMISensorPort;
                //        objInventoryLocation.IsDefault = objBinMasterDTOToInsert.IsDefault;
                //        objInventoryLocation.ItemGUID = Guid.Parse(ItemGuid);
                //        objInventoryLocation.Room = SessionHelper.RoomID;
                //        objInventoryLocation.CompanyID = SessionHelper.CompanyID;
                //        objInventoryLocation.AddedFrom = "Web";
                //        objInventoryLocation.EditedFrom = "Web";
                //        objInventoryLocation.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                //        objInventoryLocation.ReceivedOn = DateTimeUtility.DateTimeNow;
                //        objInventoryLocation.IsOnlyFromItemUI = true;
                //        objInventoryLocation = objBinMasterDAL.InsertBin(objInventoryLocation);
                //        BinGuid = Convert.ToString(objInventoryLocation.GUID);
                //    }
                //    else
                //    {
                //        objInventoryLocation.LastUpdated = DateTimeUtility.DateTimeNow;
                //        objInventoryLocation.LastUpdatedBy = SessionHelper.UserID;
                //        objInventoryLocation.MinimumQuantity = objBinMasterDTOToInsert.MinimumQuantity;
                //        objInventoryLocation.MaximumQuantity = objBinMasterDTOToInsert.MaximumQuantity;
                //        objInventoryLocation.CriticalQuantity = objBinMasterDTOToInsert.CriticalQuantity;
                //        objInventoryLocation.eVMISensorID = objBinMasterDTOToInsert.eVMISensorID;
                //        objInventoryLocation.eVMISensorPort = objBinMasterDTOToInsert.eVMISensorPort;
                //        objInventoryLocation.IsDefault = objBinMasterDTOToInsert.IsDefault;
                //        objInventoryLocation.EditedFrom = "Web";
                //        objInventoryLocation.IsOnlyFromItemUI = true;
                //        objInventoryLocation.ReceivedOn = DateTimeUtility.DateTimeNow;
                //        objBinMasterDAL.Edit(objInventoryLocation);
                //    }
                //}
            }
            try
            {
                Guid? objBinGuid = null;
                if (!string.IsNullOrEmpty(BinGuid))
                {
                    objBinGuid = Guid.Parse(BinGuid);
                }
                else
                {
                    BinMasterDTO itemDefaultBin = objBinMasterDAL.GetItemDefaultBin(Guid.Parse(ItemGuid), SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (itemDefaultBin != null && itemDefaultBin.ID > 0)
                    {
                        objBinGuid = itemDefaultBin.GUID;
                    }
                }
                if (BarcodeID == 0)
                {
                    objBarCodeMasterController = new BarcodeMasterDAL(SessionHelper.EnterPriseDBName);
                    List<BarcodeMasterDTO> objBarcodeList = objBarCodeMasterController.GetAllActiveRecordsByModuleID(ModuleGuid, SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.BarcodeString.ToLower() == BarcopdeString.ToLower() && x.RefGUID == Guid.Parse(ItemGuid)).ToList();

                    if (objBarcodeList != null && objBarcodeList.Count() > 0)
                    {
                        return Json(new { IsSuccess = false, Massage = string.Format(ResMessage.DuplicateMessage, "Barcode", BarcopdeString), RetrunDTO = objDTO, Status = "fail" }, JsonRequestBehavior.AllowGet);

                    }

                    objDTO = new BarcodeMasterDTO()
                    {
                        ID = 0,
                        UpdatedBy = SessionHelper.UserID,
                        RoomID = SessionHelper.RoomID,
                        RefGUID = Guid.Parse(ItemGuid),
                        ModuleGUID = Guid.Parse(ModuleGuid),
                        CreatedBy = SessionHelper.UserID,
                        CompanyID = SessionHelper.CompanyID,
                        BarcodeString = BarcopdeString,
                        Action = string.Empty,
                        CreatedByName = SessionHelper.UserName,
                        CreatedOn = DateTimeUtility.DateTimeNow,
                        GUID = Guid.Empty,
                        HistoryID = 0,
                        IsArchived = false,
                        IsDeleted = false,
                        ModuleName = strModuleName,
                        RefNumber = string.Empty,
                        RoomName = SessionHelper.RoomName,
                        UpdatedByName = SessionHelper.UserName,
                        UpdatedOn = DateTimeUtility.DateTimeNow,
                        BinGuid = objBinGuid,
                        BarcodeAdded = "Manual",
                        AddedFrom = "Web",
                        EditedFrom = "Web",
                        ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                        ReceivedOn = DateTimeUtility.DateTimeNow,
                    };
                    objBarCodeMasterController.Insert(objDTO);
                    return Json(new { IsSuccess = true, Massage = ResMessage.SaveMessage, RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    objBarCodeMasterController = new BarcodeMasterDAL(SessionHelper.EnterPriseDBName);
                    List<BarcodeMasterDTO> objBarcodeList = objBarCodeMasterController.GetAllActiveRecordsByModuleID(ModuleGuid, SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.BarcodeString.ToLower() == BarcopdeString.ToLower() && x.ID != BarcodeID).ToList();

                    if (objBarcodeList != null && objBarcodeList.Count() > 0)
                    {
                        return Json(new { IsSuccess = false, Massage = string.Format(ResMessage.DuplicateMessage, "Barcode", BarcopdeString), RetrunDTO = objDTO, Status = "fail" }, JsonRequestBehavior.AllowGet);

                    }
                    if (BarcodeAdded.ToLower() == "manual")
                    {
                        objDTO = new BarcodeMasterDTO()
                        {
                            ID = BarcodeID,
                            UpdatedBy = SessionHelper.UserID,
                            RoomID = SessionHelper.RoomID,
                            RefGUID = Guid.Parse(ItemGuid),
                            ModuleGUID = Guid.Parse(ModuleGuid),
                            CreatedBy = SessionHelper.UserID,
                            CompanyID = SessionHelper.CompanyID,
                            BarcodeString = BarcopdeString,
                            Action = string.Empty,
                            CreatedByName = SessionHelper.UserName,
                            CreatedOn = DateTimeUtility.DateTimeNow,
                            GUID = Guid.Empty,
                            HistoryID = 0,
                            IsArchived = false,
                            IsDeleted = false,
                            ModuleName = strModuleName,
                            RefNumber = string.Empty,
                            RoomName = SessionHelper.RoomName,
                            UpdatedByName = SessionHelper.UserName,
                            BinGuid = objBinGuid,
                            UpdatedOn = DateTimeUtility.DateTimeNow,
                            EditedFrom = "Web",
                            ReceivedOn = DateTimeUtility.DateTimeNow,

                        };
                        objBarCodeMasterController.Edit(objDTO);
                    }
                    else
                    {
                        objBarCodeMasterController.UpdateBarodeDataToItem(barcodestringOld, SessionHelper.RoomID, SessionHelper.CompanyID, Guid.Parse(ItemGuid), BarcopdeString, SessionHelper.UserID, "Web", BarcodeAdded);
                        eTurns.DAL.CacheHelper<IEnumerable<ItemManufacturerDetailsDTO>>.InvalidateCache();
                        eTurns.DAL.CacheHelper<IEnumerable<ItemSupplierDetailsDTO>>.InvalidateCache();
                    }
                    return Json(new { IsSuccess = true, Massage = ResMessage.SaveMessage, RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Massage = ex.Message, RetrunDTO = objDTO }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                objBarCodeMasterController = null;
                objDTO = null;
            }

        }

        /// <summary>
        /// Delete barcode
        /// </summary>
        /// <param name="ModuleGuid"></param>
        /// <param name="ItemGuid"></param>
        /// <param name="BarcopdeString"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteRecords(string IDs, string ModuleName)
        {
            //BarcodeMasterController objBarCodeMasterController = null;
            BarcodeMasterDAL objBarCodeMasterController = null;
            try
            {
                objBarCodeMasterController = new BarcodeMasterDAL(SessionHelper.EnterPriseDBName);
                objBarCodeMasterController.DeleteRecords(IDs, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, ModuleName);
                return Json(new { IsSuccess = true, Massage = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Massage = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                objBarCodeMasterController = null;
            }

        }


        #region Open Item Model from ItemMaster for Add new Barcode

        /// <summary>
        /// LoadItemMasterModel
        /// </summary>
        /// <param name="ParentId"></param>
        /// <returns></returns>
        public ActionResult LoadItemMasterModel(string BarcodeText)
        {

            ItemModelPerameter obj = new ItemModelPerameter()
            {
                AjaxURLAddItemToSession = "~/Barcode/AddItemToDetailTable/",
                PerentID = string.Empty,
                PerentGUID = BarcodeText,
                ModelHeader = "Item Master",
                AjaxURLAddMultipleItemToSession = "~/Barcode/AddItemToDetailTable/",
                AjaxURLToFillItemGrid = "~/Barcode/GetItemsModelMethod/",
                CallingFromPageName = "AddNewBarCode",
                OrdStagingID = string.Empty,
                OrdRequeredDate = string.Empty,

            };

            return PartialView("ItemMasterModel", obj);
        }

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult GetItemsModelMethod(QuickListJQueryDataTableParamModel param)
        {
            //ItemMasterController obj = new ItemMasterController();
            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
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
            //Int64 SupplierID = param.SupplierID;

            // set the default column sorting here, if first time then required to set 
            //if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID";

            //make changes to resolve an issue of Sort (WI-431)
            if (sortColumnName == "0" || sortColumnName.Contains("undefined"))
                sortColumnName = "ItemNumber Asc";

            //if (sortDirection == "asc")
            //    sortColumnName = sortColumnName + " asc";
            //else
            //    sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            // .Where(x=>x.ItemType != 4); , as Labour Type item not required in this module
            //var DataFromDB = obj.GetPagedRecordsForModel(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, "", string.Empty).Where(x => x.ItemType != 4);
            var tmpsupplierIds = new List<long>();
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetPagedRecordsForModel(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, "newcart", string.Empty, tmpsupplierIds, RoomDateFormat, CurrentTimeZone);

            return Json(new { sEcho = param.sEcho, iTotalRecords = TotalRecordCount, iTotalDisplayRecords = TotalRecordCount, aaData = DataFromDB }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// AddItemToDetailTable
        /// </summary>
        /// <param name="ItemGuid"></param>
        /// <param name="BarcodeText"></param>
        /// <returns></returns>
        public JsonResult AddItemToDetailTable(string ItemGuid, string BarcodeText)
        {
            try
            {
                if (!string.IsNullOrEmpty(ItemGuid) && !string.IsNullOrEmpty(BarcodeText))
                {
                    List<ModuleMasterDTO> lstModuleMaster = new List<ModuleMasterDTO>();
                    //ModuleMasterController moduleController = new ModuleMasterController();
                    ModuleMasterDAL moduleController = new ModuleMasterDAL(SessionHelper.EnterPriseDBName);
                    lstModuleMaster = moduleController.GetAllModuleNormal("ID DESC");
                    ModuleMasterDTO ModuleDTO = lstModuleMaster.Where(x => x.ModuleName == "Item Master").SingleOrDefault();
                    if (ModuleDTO != null && ModuleDTO.ID > 0)
                        return InserNewBarcode(ModuleDTO.GUID.ToString(), ItemGuid, BarcodeText, ModuleDTO.ModuleName);
                }
                return Json(new { IsSuccess = false, Massage = ResBarcodeMaster.MsgNotProperData }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { IsSuccess = false, Massage = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion


        #region Barcode

        /// <summary>
        /// GetBarcode
        /// </summary>
        /// <param name="barcodeString"></param>
        public void GetBarcode(string barcodeString)
        {
            BarcodeLib.Barcode b = new BarcodeLib.Barcode();
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            Image img = null;

            try
            {
                int W = 600;
                int H = 40;
                //if (!string.IsNullOrEmpty(barcodeString))
                //{
                //    if (barcodeString.Length < 5)
                //    {
                //        W = 200;
                //    }
                //    else if (barcodeString.Length >= 5 && barcodeString.Length < 10)
                //    {
                //        W = 300;
                //    }
                //    else if (barcodeString.Length >= 10 && barcodeString.Length < 15)
                //    {
                //        W = 400;
                //    }
                //    else if (barcodeString.Length >= 15 && barcodeString.Length < 20)
                //    {
                //        W = 500;
                //    }
                //}

                b.Alignment = BarcodeLib.AlignmentPositions.LEFT;
                BarcodeLib.TYPE type = BarcodeLib.TYPE.CODE128;

                b.IncludeLabel = true;
                b.RotateFlipType = (RotateFlipType)Enum.Parse(typeof(RotateFlipType), "RotateNoneFlipNone", true);
                b.LabelPosition = BarcodeLib.LabelPositions.BOTTOMLEFT;
                img = b.Encode(type, barcodeString, Color.Black, Color.White, W, H);


                img.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                Response.ContentType = "image/gif";
                ms.WriteTo(Response.OutputStream);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                b.Dispose();
                ms.Dispose();
                if (img != null)
                    img.Dispose();
            }
        }

        /// <summary>
        /// GetBarcode
        /// </summary>
        /// <param name="barcodeString"></param>
        public void GetBarcodeWithoutText(string barcodeString)
        {
            BarcodeLib.Barcode b = new BarcodeLib.Barcode();
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            Image img = null;

            try
            {
                int W = 600;
                int H = 40;
                //if (!string.IsNullOrEmpty(barcodeString))
                //{
                //    if (barcodeString.Length < 5)
                //    {
                //        W = 200;
                //    }
                //    else if (barcodeString.Length >= 5 && barcodeString.Length < 10)
                //    {
                //        W = 300;
                //    }
                //    else if (barcodeString.Length >= 10 && barcodeString.Length < 15)
                //    {
                //        W = 400;
                //    }
                //    else if (barcodeString.Length >= 15 && barcodeString.Length < 20)
                //    {
                //        W = 500;
                //    }
                //}

                b.Alignment = BarcodeLib.AlignmentPositions.LEFT;
                BarcodeLib.TYPE type = BarcodeLib.TYPE.CODE128;
                b.IncludeLabel = false;
                b.RotateFlipType = (RotateFlipType)Enum.Parse(typeof(RotateFlipType), "RotateNoneFlipNone", true);
                b.LabelPosition = BarcodeLib.LabelPositions.BOTTOMLEFT;
                img = b.Encode(type, barcodeString, Color.Black, Color.White, W, H);
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                Response.ContentType = "image/gif";
                ms.WriteTo(Response.OutputStream);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                b.Dispose();
                ms.Dispose();
                if (img != null)
                    img.Dispose();
            }
        }

        /// <summary>
        /// GetBarcode
        /// </summary>
        /// <param name="barcodeString"></param>
        public void GetBarcodeImage(int W, int H, bool NeedLabel, string ModuleType, string BarcodeText)
        {
            BarcodeLib.Barcode b = new BarcodeLib.Barcode();
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            Image img = null;

            try
            {
                if (ModuleType.ToLower() == "order")
                {
                    if (!BarcodeText.StartsWith("#O"))
                        BarcodeText = "#O" + BarcodeText;
                }
                b.Alignment = BarcodeLib.AlignmentPositions.LEFT;
                BarcodeLib.TYPE type = BarcodeLib.TYPE.CODE128;
                b.IncludeLabel = NeedLabel;
                b.RotateFlipType = (RotateFlipType)Enum.Parse(typeof(RotateFlipType), "RotateNoneFlipNone", true);
                b.LabelPosition = BarcodeLib.LabelPositions.BOTTOMLEFT;
                img = b.Encode(type, BarcodeText, Color.Black, Color.White, W, H);
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                Response.ContentType = "image/gif";
                ms.WriteTo(Response.OutputStream);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                b.Dispose();
                ms.Dispose();
                if (img != null)
                    img.Dispose();
            }
        }

        /// <summary>
        /// GetBarcode
        /// </summary>
        /// <param name="barcodeString"></param>
        public void GetBarcodeByKey(string barcodekey, int W = 225, int H = 25, string optionalkey = "")
        {
            if (string.IsNullOrEmpty(barcodekey) && string.IsNullOrEmpty(optionalkey))
                return;

            if (string.IsNullOrEmpty(optionalkey))
                optionalkey = barcodekey;

            BarcodeLib.Barcode b = new BarcodeLib.Barcode();
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            Image img = null;
            string BarcodeText = string.Empty;
            BarcodeLib.TYPE type = BarcodeLib.TYPE.CODE128B;
            try
            {
                b.Alignment = BarcodeLib.AlignmentPositions.LEFT;

                b.IncludeLabel = false;
                b.RotateFlipType = (RotateFlipType)Enum.Parse(typeof(RotateFlipType), "RotateNoneFlipNone", true);
                b.LabelPosition = BarcodeLib.LabelPositions.BOTTOMLEFT;
                if (W <= 0)
                    W = 300;

                if (H <= 0)
                    H = 30;

                img = b.Encode(type, barcodekey, Color.Black, Color.White, W, H);
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                Response.ContentType = "image/png";
                ms.WriteTo(Response.OutputStream);
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(optionalkey) && ex.Message.Contains("EGENERATE_IMAGE-2: Image size specified not large enough to draw image."))
                {
                    W = 250;
                    try
                    {
                        img = b.Encode(type, optionalkey, System.Drawing.Color.Black, System.Drawing.Color.White, W, H);
                        if (img != null)
                        {
                            img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            Response.ContentType = "image/png";
                            ms.WriteTo(Response.OutputStream);
                        }
                    }
                    catch (Exception)
                    {
                        if (!string.IsNullOrEmpty(optionalkey) && ex.Message.Contains("EGENERATE_IMAGE-2: Image size specified not large enough to draw image."))
                        {
                            W = 300;
                            try
                            {
                                img = b.Encode(type, optionalkey, System.Drawing.Color.Black, System.Drawing.Color.White, W, H);
                                if (img != null)
                                {
                                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                    Response.ContentType = "image/png";
                                    ms.WriteTo(Response.OutputStream);
                                }
                            }
                            catch (Exception)
                            {
                                if (!string.IsNullOrEmpty(optionalkey) && ex.Message.Contains("EGENERATE_IMAGE-2: Image size specified not large enough to draw image."))
                                {
                                    W = 400;
                                    try
                                    {
                                        img = b.Encode(type, optionalkey, System.Drawing.Color.Black, System.Drawing.Color.White, W, H);
                                        if (img != null)
                                        {
                                            img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                            Response.ContentType = "image/png";
                                            ms.WriteTo(Response.OutputStream);
                                        }
                                    }
                                    catch (Exception)
                                    {

                                    }
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                b.Dispose();
                ms.Dispose();
                if (img != null)
                    img.Dispose();
            }
        }

        #endregion

        #region Display Barcode

        public void WriteBarcodeFromText(string BarcodeText)
        {
            try
            {

                PrivateFontCollection cFonts = new PrivateFontCollection();
                cFonts.AddFontFile(System.AppDomain.CurrentDomain.BaseDirectory + "\\Content\\BarcodeFonts\\IDAutomationHC39M.ttf");
                FontFamily c39Family = new FontFamily("IDAutomationHC39M", cFonts);
                Font c39Font = new Font(c39Family, 12);

                Response.ContentType = "text/png";
                BarCode bc = new BarCode();

                bc.Width = 350; bc.Height = 60;
                bc.Font = c39Font;
                bc.Text = "*" + BarcodeText + "*";
                GenerateBarcode(Response, bc);


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void GenerateBarcode(HttpResponseBase cResponse, BarCode codetoGenerate)
        {
            System.IO.MemoryStream mStream = new System.IO.MemoryStream();
            try
            {
                Bitmap barCodeImage = new Bitmap(codetoGenerate.Width, codetoGenerate.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Graphics barCodeContainer = Graphics.FromImage(barCodeImage);

                System.Drawing.PointF oPoint = new System.Drawing.PointF(2f, 2f);
                System.Drawing.SolidBrush oBrushWrite = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
                System.Drawing.SolidBrush oBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
                barCodeContainer.FillRectangle(oBrush, 0, 0, codetoGenerate.Width, codetoGenerate.Height);
                barCodeContainer.DrawString(codetoGenerate.Text, codetoGenerate.Font, oBrushWrite, oPoint);

                //barCodeContainer.DrawString(codetoGenerate.Text, codetoGenerate.Font, new SolidBrush(Color.Black), 0, 0);
                cResponse.ContentType = "image/png";
                // save image to memory stream
                //barCodeImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                barCodeImage.Save(mStream, System.Drawing.Imaging.ImageFormat.Png);
                // flush image to output stream of page response
                mStream.WriteTo(cResponse.OutputStream);
                //clean up
                barCodeImage.Dispose();
                barCodeContainer.Dispose();

            }
            catch (Exception ex)
            {
                throw ex;
                //ErrorSignal.FromContext(System.Web.HttpContext.Current).Raise(ex);
            }
            finally
            {
                mStream.Close();
                mStream.Dispose();
            }
        }
        private void GenerateBarcodeNumber(HttpResponse cResponse, BarCode codetoGenerate)
        {
            System.IO.MemoryStream mStream = new System.IO.MemoryStream();
            try
            {

                Bitmap bmp = new Bitmap(codetoGenerate.Width, codetoGenerate.Height,
               PixelFormat.Format32bppArgb);

                Graphics g = Graphics.FromImage(bmp);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(0, 0, codetoGenerate.Width, codetoGenerate.Height);

                HatchBrush brush = new HatchBrush(
                  HatchStyle.Weave,
                  Color.White,
                  Color.White);
                g.FillRectangle(brush, rect);

                g.DrawString(codetoGenerate.Text, new Font("Arial", 11.0f, FontStyle.Regular), Brushes.Black, 3, 2);
                bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                bmp.Save(mStream, System.Drawing.Imaging.ImageFormat.Png);
                mStream.WriteTo(cResponse.OutputStream);
                bmp.Dispose();
                g.Dispose();

            }
            catch (Exception ex)
            {
                throw ex;
                //ErrorSignal.FromContext(System.Web.HttpContext.Current).Raise(ex);
            }
            finally
            {
                mStream.Close();
                mStream.Dispose();
            }
        }
        private void AddNoise(Graphics g, BarCode codetoGenerate)
        {
            Random rand = new Random();

            for (int i = 1; i < 500; i++)
            {
                int posX = rand.Next(1, codetoGenerate.Width);
                int posY = rand.Next(1, codetoGenerate.Height);
                int width = rand.Next(3);
                int height = rand.Next(3);
                g.DrawEllipse(Pens.Silver, posX, posY,
                    width, height);
            }
        }
        public void WriteBarcodeFromText128(string BarcodeText)
        {
            PrivateFontCollection cFonts = new PrivateFontCollection();
            FontFamily c39Family = null;
            Font c39Font = null;
            BarCode bc = null;
            try
            {
                //cFonts.AddFontFile(System.AppDomain.CurrentDomain.BaseDirectory + "\\Content\\BarcodeFonts\\code128.ttf");
                //c39Family = new FontFamily("Code 128", cFonts);
                //c39Font = new Font(c39Family, 36);

                cFonts.AddFontFile(System.AppDomain.CurrentDomain.BaseDirectory + "\\Content\\BarcodeFonts\\code128.ttf");
                c39Family = new FontFamily("Code 128", cFonts);
                c39Font = new Font(c39Family, 36);

                Response.ContentType = "text/png";
                bc = new BarCode();

                bc.Width = 300; bc.Height = 60;
                bc.Font = c39Font;
                bc.Text = BarcodeText;
                GenerateBarcode128New(Response, bc);


            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                cFonts.Dispose();
            }
        }
        private void GenerateBarcode128(HttpResponseBase cResponse, BarCode codetoGenerate)
        {
            System.IO.MemoryStream mStream = new System.IO.MemoryStream();
            Bitmap barCodeImage = null;
            Graphics barCodeContainer = null;
            try
            {
                barCodeImage = new Bitmap(codetoGenerate.Width, codetoGenerate.Height + 20, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                barCodeContainer = Graphics.FromImage(barCodeImage);

                string s = codetoGenerate.Text;
                string drStr = string.Join(" ", s.ToCharArray());

                System.Drawing.PointF oPoint = new System.Drawing.PointF(2f, 2f);
                System.Drawing.SolidBrush oBrushWrite = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
                System.Drawing.SolidBrush oBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
                barCodeContainer.FillRectangle(oBrush, 0, 0, codetoGenerate.Width, codetoGenerate.Height);
                barCodeContainer.DrawString(codetoGenerate.Text, codetoGenerate.Font, oBrushWrite, oPoint);
                barCodeContainer.DrawString(drStr, new Font(new FontFamily(GenericFontFamilies.SansSerif), 10), oBrushWrite, 12f, (codetoGenerate.Height - 12f));
                cResponse.ContentType = "image/png";
                barCodeImage.Save(mStream, System.Drawing.Imaging.ImageFormat.Png);
                mStream.WriteTo(cResponse.OutputStream);


            }
            catch (Exception ex)
            {
                throw ex;
                //ErrorSignal.FromContext(System.Web.HttpContext.Current).Raise(ex);
            }
            finally
            {
                barCodeImage.Dispose();
                barCodeContainer.Dispose();
                mStream.Close();
                mStream.Dispose();
            }
        }

        private void GenerateBarcode128NewWithoutString(HttpResponseBase cResponse, BarCode codetoGenerate)
        {
            System.IO.MemoryStream mStream = new System.IO.MemoryStream();
            try
            {
                Bitmap barCodeImage = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Graphics barCodeContainer = Graphics.FromImage(barCodeImage);
                SizeF _barCodeSize = barCodeContainer.MeasureString(codetoGenerate.Text, codetoGenerate.Font);
                Font normalFont = new Font(new FontFamily(GenericFontFamilies.SansSerif), 9);
                int bcodeWidth = (int)_barCodeSize.Width;
                int bcodeHeight = (int)_barCodeSize.Height;
                int vpos = 0;

                barCodeImage = new Bitmap(bcodeWidth, bcodeHeight, PixelFormat.Format32bppArgb);
                barCodeContainer = Graphics.FromImage(barCodeImage);


                System.Drawing.SolidBrush oBrushWrite = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
                System.Drawing.SolidBrush oBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
                barCodeContainer.FillRectangle(oBrush, new Rectangle(0, 0, bcodeWidth, bcodeHeight));
                barCodeContainer.DrawString(codetoGenerate.Text, codetoGenerate.Font, oBrushWrite, XCentered((int)_barCodeSize.Width, bcodeWidth), vpos);
                cResponse.ContentType = "image/png";
                barCodeImage.Save(mStream, System.Drawing.Imaging.ImageFormat.Png);
                mStream.WriteTo(cResponse.OutputStream);
                barCodeImage.Dispose();
                barCodeContainer.Dispose();

            }
            catch (Exception ex)
            {
                throw ex;
                //ErrorSignal.FromContext(System.Web.HttpContext.Current).Raise(ex);
            }
            finally
            {
                mStream.Close();
                mStream.Dispose();
            }
        }
        private void GenerateBarcode128New(HttpResponseBase cResponse, BarCode codetoGenerate)
        {
            System.IO.MemoryStream mStream = new System.IO.MemoryStream();
            try
            {
                Bitmap barCodeImage = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Graphics barCodeContainer = Graphics.FromImage(barCodeImage);
                SizeF _barCodeSize = barCodeContainer.MeasureString(codetoGenerate.Text, codetoGenerate.Font);
                Font normalFont = new Font(new FontFamily(GenericFontFamilies.SansSerif), 8);

                string s = codetoGenerate.Text;
                string drStr = string.Join(" ", s.ToCharArray());
                int bcodeWidth = (int)_barCodeSize.Width;
                int bcodeHeight = (int)_barCodeSize.Height;

                SizeF _codeStringSize = barCodeContainer.MeasureString(drStr, normalFont);

                //bcodeWidth = Max(bcodeWidth, (int)_codeStringSize.Width);
                //bcodeHeight += ((int)_codeStringSize.Height);
                int vpos = 0;

                barCodeImage = new Bitmap(bcodeWidth, bcodeHeight, PixelFormat.Format32bppArgb);
                barCodeContainer = Graphics.FromImage(barCodeImage);


                System.Drawing.SolidBrush oBrushWrite = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
                System.Drawing.SolidBrush oBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
                barCodeContainer.FillRectangle(oBrush, new Rectangle(0, 0, bcodeWidth, bcodeHeight));
                barCodeContainer.DrawString(codetoGenerate.Text, codetoGenerate.Font, oBrushWrite, XCentered((int)_barCodeSize.Width, bcodeWidth), vpos);

                vpos += (((int)_barCodeSize.Height));
                barCodeContainer.FillRectangle(oBrush, new Rectangle(XCentered(((int)_codeStringSize.Width + 10), bcodeWidth), ((int)_barCodeSize.Height - ((int)_codeStringSize.Height + 4)), (int)_codeStringSize.Width, (int)_codeStringSize.Height));
                barCodeContainer.DrawString(drStr, normalFont, oBrushWrite, XCentered(((int)_codeStringSize.Width + 5), bcodeWidth), ((int)_barCodeSize.Height - ((int)_codeStringSize.Height + 3)));

                cResponse.ContentType = "image/png";
                barCodeImage.Save(mStream, System.Drawing.Imaging.ImageFormat.Png);
                mStream.WriteTo(cResponse.OutputStream);
                barCodeImage.Dispose();
                barCodeContainer.Dispose();

            }
            catch (Exception ex)
            {
                throw ex;
                //ErrorSignal.FromContext(System.Web.HttpContext.Current).Raise(ex);
            }
            finally
            {
                mStream.Close();
                mStream.Dispose();
            }
        }

        public void GetBarcodeImg(string strText)
        {
            try
            {

                PrivateFontCollection cFonts = new PrivateFontCollection();
                cFonts.AddFontFile(System.AppDomain.CurrentDomain.BaseDirectory + "\\Content\\BarcodeFonts\\IDAutomationHC39M.ttf");
                FontFamily fntFamily = new FontFamily("IDAutomationHC39M", cFonts);
                Font fnt = new Font(fntFamily, 12);

                BarCode bc = new BarCode();
                bc.Width = 350;
                bc.Height = 60;
                bc.Font = fnt;
                bc.Text = strText;
                GenerateBarcodeImg(Response, bc);


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void GenerateBarcodeImg(HttpResponseBase cResponse, BarCode codetoGenerate)
        {
            System.IO.MemoryStream mStream = new System.IO.MemoryStream();
            try
            {
                Bitmap barCodeImage = new Bitmap(codetoGenerate.Width, codetoGenerate.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                //barCodeImage.SetResolution(96f, 96f);
                Graphics barCodeContainer = Graphics.FromImage(barCodeImage);

                System.Drawing.PointF oPoint = new System.Drawing.PointF(2f, 2f);
                System.Drawing.SolidBrush oBrushWrite = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
                System.Drawing.SolidBrush oBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
                barCodeContainer.FillRectangle(oBrush, 0, 0, codetoGenerate.Width, codetoGenerate.Height);
                barCodeContainer.DrawString(codetoGenerate.Text, codetoGenerate.Font, oBrushWrite, oPoint);
                cResponse.ContentType = "image/png";
                barCodeImage.Save(mStream, System.Drawing.Imaging.ImageFormat.Png);
                mStream.WriteTo(cResponse.OutputStream);
                barCodeImage.Dispose();
                barCodeContainer.Dispose();

            }
            catch (Exception ex)
            {
                throw ex;
                //ErrorSignal.FromContext(System.Web.HttpContext.Current).Raise(ex);
            }
            finally
            {
                mStream.Close();
                mStream.Dispose();
            }
        }


        #region Auxiliary Methods

        private int Max(int v1, int v2)
        {
            return (v1 > v2 ? v1 : v2);
        }

        private int XCentered(int localWidth, int globalWidth)
        {
            return ((globalWidth - localWidth) / 2);
        }

        #endregion
        #endregion



        public PartialViewResult _CreateBarcode()
        {
            return PartialView();
        }
        public ActionResult BarcodeMasterList()
        {
            List<ModuleMasterDTO> lstModuleMaster = new List<ModuleMasterDTO>();
            ModuleMasterDAL moduleController = new ModuleMasterDAL(SessionHelper.EnterPriseDBName);
            lstModuleMaster = moduleController.GetAllModuleNormal("ModuleName ASC");

            lstModuleMaster = lstModuleMaster.Where(l => l.ModuleName == "Item Master" || l.ModuleName == "Assets" || l.ModuleName == "Tool Master").ToList();
            ViewBag.ModuleMasterList = lstModuleMaster;
            return View();
        }

        public ActionResult BarcodeCreate()
        {


            BarcodeMasterDTO objDTO = new BarcodeMasterDTO();
            objDTO.ID = 0;

            objDTO.ModuleName = string.Empty;
            objDTO.BinNumber = string.Empty;
            objDTO.BarcodeString = string.Empty;
            objDTO.BarcodeAdded = "Manual";
            objDTO.CreatedOn = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.RoomID = SessionHelper.RoomID;
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.RoomName = SessionHelper.RoomName;
            objDTO.UpdatedOn = DateTimeUtility.DateTimeNow;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.CreatedByName = SessionHelper.UserName;
            objDTO.UpdatedBy = SessionHelper.UserID;
            objDTO.GUID = Guid.NewGuid();
            objDTO.items = string.Empty;
            objDTO.IsDeleted = false;
            objDTO.IsArchived = false;

            List<ModuleMasterDTO> lstModuleMaster = new List<ModuleMasterDTO>();
            ModuleMasterDAL moduleController = new ModuleMasterDAL(SessionHelper.EnterPriseDBName);
            lstModuleMaster = moduleController.GetAllModuleNormal("ModuleName ASC");

            lstModuleMaster = lstModuleMaster.Where(l => l.ModuleName == "Item Master" || l.ModuleName == "Assets" || l.ModuleName == "Tool Master").ToList();
            if (lstModuleMaster.Count > 0)
            {
                objDTO.ModuleGUID = lstModuleMaster.Where(l => l.ModuleName == "Item Master").FirstOrDefault().GUID;
                objDTO.ModuleName = "Item Master";
            }
            ViewBag.ModuleMasterList = lstModuleMaster;

            List<ItemMasterDTO> lstItemMaster = new List<ItemMasterDTO>();
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            lstItemMaster = objItemMasterDAL.GetAllItemsPlain(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(t => t.ItemNumber).ToList();

            ViewBag.ItemMasterList = lstItemMaster;
            string ModuleName = "Item Master";
            if (Convert.ToInt32(Session["SetCurrentModule"]) == 1)
            {
                ModuleName = "Item Master";
            }
            if (Convert.ToInt32(Session["SetCurrentModule"]) == 2)
            {
                ModuleName = "Assets";
            }
            if (Convert.ToInt32(Session["SetCurrentModule"]) == 3)
            {
                ModuleName = "Tool Master";
            }



            objDTO.ModuleGUID = lstModuleMaster.Where(l => l.ModuleName == ModuleName).FirstOrDefault().GUID;

            return PartialView("_CreateBarcode", objDTO);
        }
        //public Guid GetItemDefaultBin(Guid ItemGuid)
        //{
        //    BinMasterDAL objCtrl = new BinMasterDAL(SessionHelper.EnterPriseDBName);
        //    List<BinMasterDTO> lstDTO;
        //    Int64 RoomID = SessionHelper.RoomID;
        //    Int64 CompanyID = SessionHelper.CompanyID;
        //    List<DTOForAutoComplete> returnKeyValList = new List<DTOForAutoComplete>();
        //    try
        //    {

        //        lstDTO = objCtrl.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(l => l.ItemGUID == ItemGuid && l.IsDefault == true).OrderBy(i => i.BinNumber).ToList();
        //        //lstDTO = objCtrl.GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGuid,0,true,null).OrderBy(i => i.BinNumber).ToList();//.Where(l => l.ItemGUID == ItemGuid && l.IsDefault == true)
        //        //lstDTO = objCtrl.GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGuid,0, true,null).OrderBy(i => i.BinNumber).ToList();
        //        if (lstDTO != null && lstDTO.Count > 0)
        //        {
        //            return lstDTO.FirstOrDefault().GUID;
        //        }
        //        else
        //        {
        //            return Guid.Empty;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return Guid.Empty;
        //    }
        //}
        public ActionResult BarcodeEdit(Int64 ID)
        {
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());

            bool IsHistory = false;
            if (Request["IsHistory"] != null && Request["IsHistory"].ToString() != "")
                IsHistory = bool.Parse(Request["IsHistory"].ToString());
            bool IsChangeLog = false;
            if (Request["IsChangeLog"] != null && Request["IsChangeLog"].ToString() != "")
                IsChangeLog = bool.Parse(Request["IsChangeLog"].ToString());

            if (IsDeleted || IsArchived || IsHistory || IsChangeLog)
            {
                ViewBag.ViewOnly = true;
            }

            BarcodeMasterDAL obj = new eTurns.DAL.BarcodeMasterDAL(SessionHelper.EnterPriseDBName);
            BarcodeMasterDTO objDTO = new BarcodeMasterDTO();
            //if (!IsChangeLog)
            //objDTO = obj.GetRecord(ID, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted);
            objDTO = obj.GetBarcodeMasterByID(ID, SessionHelper.RoomID, SessionHelper.CompanyID, IsDeleted, IsArchived);
            //else
            //    objDTO = obj.GetHistoryRecord(ID, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted);


            List<ModuleMasterDTO> lstModuleMaster = new List<ModuleMasterDTO>();
            ModuleMasterDAL moduleController = new ModuleMasterDAL(SessionHelper.EnterPriseDBName);
            lstModuleMaster = moduleController.GetAllModuleNormal("ModuleName ASC");

            lstModuleMaster = lstModuleMaster.Where(l => l.ModuleName == "Item Master" || l.ModuleName == "Assets" || l.ModuleName == "Tool Master").ToList();

            ViewBag.ModuleMasterList = lstModuleMaster;

            List<ItemMasterDTO> lstItemMaster = new List<ItemMasterDTO>();
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            lstItemMaster = objItemMasterDAL.GetAllItemsPlain(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(t => t.ItemNumber).ToList();
            ViewBag.ItemMasterList = lstItemMaster;

            return PartialView("_CreateBarcode", objDTO);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult BarcodeSave(BarcodeMasterDTO objDTO)
        {
            string message = "";
            string status = "";
            AssetMasterDAL obj = new AssetMasterDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            if (objDTO.BarcodeAdded == "ItemNumber")
            {
                ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                Guid? NewGuid = objItemMasterDAL.GetItemGuIDOnlyByItemNumber(objDTO.BarcodeString, SessionHelper.RoomID);
                if (NewGuid != null)
                {
                    if (objDTO.RefGUID != NewGuid.GetValueOrDefault(Guid.Empty))
                    {
                        // message = string.Format(ResMessage.DuplicateMessage, ResItemMaster.ItemNumber, objDTO.BarcodeString);  //"BinNumber \"" + objDTO.BinNumber + "\" already exist! Try with Another!";
                        return Json(new { IsSuccess = false, Massage = string.Format(ResMessage.DuplicateMessage, ResItemMaster.ItemNumber, objDTO.BarcodeString), RetrunDTO = objDTO, Status = "fail" }, JsonRequestBehavior.AllowGet);

                    }
                }
                if (objDTO.BarcodeString.Length > 100)
                {
                    return Json(new { IsSuccess = false, Massage = string.Format(ResMessage.MaxLength, ResItemMaster.ItemNumber, objDTO.BarcodeString), RetrunDTO = objDTO, Status = "fail" }, JsonRequestBehavior.AllowGet);

                }
            }
            if (string.IsNullOrEmpty(objDTO.BarcodeString))
            {
                message = string.Format(ResMessage.Required, ResBarcodeMaster.BarcodeString);
                status = "fail";
                return Json(new { Massage = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            if ((string.IsNullOrEmpty(Convert.ToString(objDTO.ModuleGUID))) || objDTO.ModuleGUID == Guid.Empty)
            {
                message = string.Format(ResMessage.Required, ResBarcodeMaster.ModuleName);
                status = "fail";
                return Json(new { Massage = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(Convert.ToString(objDTO.RefGUID)) || objDTO.RefGUID == Guid.Empty)
            {
                message = string.Format(ResMessage.Required, ResItemMaster.ItemNumber);
                status = "fail";
                return Json(new { Massage = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            objDTO.UpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.RoomID = SessionHelper.RoomID;
            if (objDTO.ID == 0)
            {
                objDTO.CreatedBy = SessionHelper.UserID;
                objDTO.AddedFrom = "Web";
                objDTO.EditedFrom = "Web";
                objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                //  return InserNewBarcodeWithBin(Convert.ToString(objDTO.ModuleGUID), Convert.ToString(objDTO.RefGUID), objDTO.BarcodeString, objDTO.ModuleName, Convert.ToString(objDTO.BinGuid), objDTO.BinNumber, objDTO.ID);
            }
            else
            {
                objDTO.EditedFrom = "Web";

            }
            return InserNewBarcodeWithBin(Convert.ToString(objDTO.ModuleGUID), Convert.ToString(objDTO.RefGUID), objDTO.BarcodeString, objDTO.ModuleName, Convert.ToString(objDTO.BinGuid), objDTO.BinNumber, objDTO.ID, objDTO.BarcodeAdded, objDTO.OldBarcodeString);
            //return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);

        }
        public PartialViewResult GetItemPartialView(List<BarcodeMasterDTO> model)
        {
            List<ModuleMasterDTO> lstModuleMaster = new List<ModuleMasterDTO>();
            ModuleMasterDAL moduleController = new ModuleMasterDAL(SessionHelper.EnterPriseDBName);
            lstModuleMaster = moduleController.GetAllModuleNormal("ModuleName ASC");

            lstModuleMaster = lstModuleMaster.Where(l => l.ModuleName == "Item Master" || l.ModuleName == "Assets").ToList();

            ViewBag.ModuleMasterList = lstModuleMaster;
            return PartialView("_ItemBarcodeList", model);
        }
        public PartialViewResult GetAssetPartialView(List<BarcodeMasterDTO> model)
        {
            List<ModuleMasterDTO> lstModuleMaster = new List<ModuleMasterDTO>();
            ModuleMasterDAL moduleController = new ModuleMasterDAL(SessionHelper.EnterPriseDBName);
            lstModuleMaster = moduleController.GetAllModuleNormal("ModuleName ASC");

            lstModuleMaster = lstModuleMaster.Where(l => l.ModuleName == "Item Master" || l.ModuleName == "Assets" || l.ModuleName == "Tool Master").ToList();

            ViewBag.ModuleMasterList = lstModuleMaster;
            return PartialView("_AssetsBarcodeList", model);
        }
        public PartialViewResult GetToolPartialView(List<BarcodeMasterDTO> model)
        {
            List<ModuleMasterDTO> lstModuleMaster = new List<ModuleMasterDTO>();
            ModuleMasterDAL moduleController = new ModuleMasterDAL(SessionHelper.EnterPriseDBName);
            lstModuleMaster = moduleController.GetAllModuleNormal("ModuleName ASC");

            lstModuleMaster = lstModuleMaster.Where(l => l.ModuleName == "Item Master" || l.ModuleName == "Assets" || l.ModuleName == "Tool Master").ToList();

            ViewBag.ModuleMasterList = lstModuleMaster;
            return PartialView("_ToolsBarcodeList", model);
        }
        public ActionResult BarcodeListAjax(JQueryDataTableParamModel param)
        {
            BarcodeMasterDAL obj = new eTurns.DAL.BarcodeMasterDAL(SessionHelper.EnterPriseDBName);

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
            Guid ModuleGuid = Guid.Parse(Request["ModuleGuid"]);
            // set the default column sorting here, if first time then required to set 
            //if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID";

            //if (sortDirection == "asc")
            //    sortColumnName = sortColumnName + " asc";
            //else
            //    sortColumnName = sortColumnName + " desc";

            //if (sortColumnName == "0" || sortColumnName.Contains("undefined"))
            //    sortColumnName = "ID Asc";

            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "ID desc";
            }
            else
                sortColumnName = "ID desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetBarcodeList(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, Convert.ToString(SessionHelper.RoomDateFormat), ModuleGuid, CurrentTimeZone);
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);
        }
        public string DeleteBarcodeRecords(string ids)
        {
            try
            {
                BarcodeMasterDAL obj = new eTurns.DAL.BarcodeMasterDAL(SessionHelper.EnterPriseDBName);
                obj.DeleteRecords(ids, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, string.Empty);
                string strMessage = obj.GetBarcodeStatus(ids, "deleted",SessionHelper.EnterPriceID,SessionHelper.CompanyID,SessionHelper.RoomID,SessionHelper.UserID);
                //return "ok";
                return strMessage;
                //return "not ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string UnDeleteRecords(string ids)
        {
            try
            {
                BarcodeMasterDAL obj = new eTurns.DAL.BarcodeMasterDAL(SessionHelper.EnterPriseDBName);
                obj.UnDeleteRecords(ids, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, string.Empty);
                string strMessage = obj.GetBarcodeStatus(ids, "Undeleted",SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);
                //return "ok";
                return strMessage;
                //return "not ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public void SetSelectedModule(string CurModule)
        {
            Session["SetCurrentModule"] = CurModule;
            if (CurModule != "1")
            {
                Session["SetCurrentModule"] = CurModule;
            }
            else
            {
                Session["SetCurrentModule"] = "1";
            }
        }
        public string ValidateBarcodeString(string NewBarcodeString, Int64 barCodeId)
        {
            try
            {
                BarcodeMasterDAL objCDAL = new BarcodeMasterDAL(SessionHelper.EnterPriseDBName);
                string strOK = objCDAL.DuplicateCheck(barCodeId, NewBarcodeString, SessionHelper.RoomID, SessionHelper.CompanyID);
                return strOK;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public string GenerateBarcodeForRoom(string ModuleName, string AddedFrom)
        {
            string Message = "success";
            try
            {
                BarcodeMasterDAL objCDAL = new BarcodeMasterDAL(SessionHelper.EnterPriseDBName);
                objCDAL.GenerateBarcodeForRoom(SessionHelper.RoomID, ModuleName, AddedFrom);

                return Message;
            }
            catch (Exception e)
            {
                return e.Message.ToString();
            }
        }
    }

    public class SearchItemDTO
    {
        public string itemID { get; set; }
        public string ItemText { get; set; }
        public string ItemGuid { get; set; }
        public string DefaultLocationName { get; set; }
        public Guid? DefaultLocationGuid { get; set; }
    }
    public class BarCode
    {
        public string Text;
        public int Width, Height;
        public Font Font;
    }
}
