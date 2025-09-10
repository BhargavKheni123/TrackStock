using Dynamite.Data.Extensions;
using Dynamite.Extensions;
using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsWeb.Helper;
using eTurnsWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace eTurnsWeb.Controllers
{
    public partial class InventoryController : eTurnsControllerBase
    {
        #region [Material Staging]

        UDFController objUDFDAL = new UDFController();

        public ActionResult MaterialStagingList()
        {

            //BinMasterDAL objBinMasterController = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            //List<SelectListItem> lst = new List<SelectListItem>();
            //IEnumerable<BinMasterDTO> lstBins = objBinMasterController.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
            //lstBins.ToList().ForEach(t =>
            //{
            //    lst.Add(new SelectListItem()
            //    {
            //        Text = t.BinNumber,
            //        Value = t.ID.ToString(),
            //        Selected = false
            //    });
            //});

            //ViewBag.StagLocs = lst;
            return View();
        }

        public ActionResult MaterialStagingListAjax(JQueryDataTableParamModel param)
        {
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

            // set the default column sorting here, if first time then required to set 

            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "ID desc";
            }
            else
                sortColumnName = "ID desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            MaterialStagingDAL objMaterialStagingAPIController = new MaterialStagingDAL(SessionHelper.EnterPriseDBName);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            IEnumerable<MaterialStagingDTO> DataFromDB = objMaterialStagingAPIController.GetPagedMaterialStagingsFromDB(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone);

            DataFromDB.ToList().ForEach(t =>
                {
                    //t.CreatedDate = CommonUtility.ConvertDateByTimeZone(t.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                    // t.UpdatedDate = CommonUtility.ConvertDateByTimeZone(t.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                    t.StagingLocationName = Convert.ToString(t.StagingLocationName) == "[|EmptyStagingBin|]" ? string.Empty : t.StagingLocationName;
                    t.BinName = Convert.ToString(t.BinName) == "[|EmptyStagingBin|]" ? string.Empty : t.BinName;
                });


            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount
            ,
                aaData = DataFromDB
            }, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 0, VaryByParam = "None")]
        //public ActionResult MaterialStagingEdit(long? ID)
        public ActionResult MaterialStagingEdit(string MSGUID)
        {
            MaterialStagingDAL objMaterialStagingAPIController = new MaterialStagingDAL(SessionHelper.EnterPriseDBName);
            //BinMasterDAL objBinMasterController = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            MaterialStagingDTO objDTO = new MaterialStagingDTO();
            //List<BinMasterDTO> lstBins = objBinMasterController.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
            ////List<BinMasterDTO> lstBins = objBinMasterController.GetBinMasterByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
            //ViewBag.Bins = lstBins;
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("MaterialStaging");
            ViewBag.LineItemCount = 0;

            //if (ID.HasValue && ID.Value > 0)
            if (MSGUID != "")
            {
                objDTO = objMaterialStagingAPIController.GetRecord(Guid.Parse(MSGUID), SessionHelper.RoomID, SessionHelper.CompanyID);
            }
            if (objDTO == null)
            {
                objDTO = new MaterialStagingDTO()
                {
                    ID = 0,
                    Created = DateTimeUtility.DateTimeNow,
                    Updated = DateTimeUtility.DateTimeNow,
                    CreatedBy = SessionHelper.UserID,
                    CreatedByName = SessionHelper.UserName,
                    LastUpdatedBy = SessionHelper.UserID,
                    Room = SessionHelper.RoomID,
                    CompanyID = SessionHelper.CompanyID,
                    RoomName = SessionHelper.RoomName,
                    UpdatedByName = SessionHelper.UserName,
                    CreatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                    UpdatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                    IsOnlyFromItemUI = true,
                };

                return PartialView("_MaterialStagingDetails", objDTO);
            }
            else
            {
                ViewData["UDF1"] = objDTO.UDF1;
                ViewData["UDF2"] = objDTO.UDF2;
                ViewData["UDF3"] = objDTO.UDF3;
                ViewData["UDF4"] = objDTO.UDF4;
                ViewData["UDF5"] = objDTO.UDF5;
                objDTO.IsOnlyFromItemUI = true;

                objDTO.CreatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                objDTO.UpdatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);

                int totalcount = 0;
                IEnumerable<MaterialStagingDetailDTO> lstItems = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName).GetPagedStagingLineItems(0, 2000, out totalcount, string.Empty, "StagingId DESC", SessionHelper.RoomID, SessionHelper.CompanyID, false, false, objDTO.GUID, SessionHelper.UserSupplierIds);
                ViewBag.LineItemCount = totalcount;
                if (Convert.ToString(objDTO.StagingLocationName) == "[|EmptyStagingBin|]")
                {
                    objDTO.StagingLocationName = string.Empty;
                }
                return PartialView("_MaterialStagingDetails", objDTO);
            }
        }

        public ActionResult MaterialStagingCreate(long? ID)
        {
            ViewBag.LineItemCount = 0;
            MaterialStagingDAL objMaterialStagingAPIController = new MaterialStagingDAL(SessionHelper.EnterPriseDBName);
            //BinMasterDAL objBinMasterController = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            MaterialStagingDTO objDTO = new MaterialStagingDTO();
            //List<BinMasterDTO> lstBins = objBinMasterController.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
            //List<BinMasterDTO> lstBins = objBinMasterController.GetBinMasterByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
            AutoOrderNumberGenerate objAutoNumber = null;
            //ViewBag.Bins = lstBins;
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("MaterialStaging");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            //ClearMsSession();
            if (ID.HasValue && ID.Value > 0)
            {
                objDTO = objMaterialStagingAPIController.GetRecord(ID.Value, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (objDTO != null)
                    objDTO.IsOnlyFromItemUI = true;
            }

            Session["ItemMasterList"] = null;

            if (objDTO == null || objDTO.ID == 0)
            {
                objAutoNumber = new AutoSequenceDAL(SessionHelper.EnterPriseDBName).GetNextStagingNumber(RoomID, CompanyID,SessionHelper.EnterPriceID);
                string NewNumber = objAutoNumber.OrderNumber;
                if (NewNumber != null && (!string.IsNullOrEmpty(NewNumber)))
                {
                    NewNumber = NewNumber.Length > 22 ? NewNumber.Substring(0, 22) : NewNumber;
                }
                //AutoSequenceNumbersDTO objAutoSequenceDTO = new AutoSequenceDAL(SessionHelper.EnterPriseDBName).GetLastGeneratedID(SessionHelper.RoomID, SessionHelper.CompanyID, "#S");
                //string NewNumber = objAutoSequenceDTO.Prefix + "-" + SessionHelper.CompanyID + "-" + SessionHelper.RoomID + "-" + DateTime.Now.ToString("MMddyy") + "-" + objAutoSequenceDTO.LastGenereted;
                //string NewNumber = string.Empty; //new AutoSequenceDAL(SessionHelper.EnterPriseDBName).GetLastGeneratedROOMID("NextStagingNo", SessionHelper.RoomID, SessionHelper.CompanyID).ToString();
                //NewNumber = new AutoSequenceDAL(SessionHelper.EnterPriseDBName).GetNextAutoNumberByModule("nextstagingno", SessionHelper.RoomID, SessionHelper.CompanyID);
                objDTO = new MaterialStagingDTO()
                {
                    ID = 0,
                    //StagingName = "#S" + NewNumber,
                    StagingName = NewNumber,
                    Created = DateTimeUtility.DateTimeNow,
                    Updated = DateTimeUtility.DateTimeNow,
                    CreatedBy = SessionHelper.UserID,
                    CreatedByName = SessionHelper.UserName,
                    LastUpdatedBy = SessionHelper.UserID,
                    Room = SessionHelper.RoomID,
                    CompanyID = SessionHelper.CompanyID,
                    RoomName = SessionHelper.RoomName,
                    UpdatedByName = SessionHelper.UserName,
                    StagingStatus = 1,
                    IsOnlyFromItemUI = true,
                };

                return PartialView("_MaterialStagingDetails", objDTO);
            }
            else
            {
                return PartialView("_MaterialStagingDetails", objDTO);
            }
        }

        //public ActionResult LoadStagingLineItems(long? MsID)
        public ActionResult LoadStagingLineItems(string MSGUID)
        {
            MaterialStagingDAL objMaterialStagingAPIController = new MaterialStagingDAL(SessionHelper.EnterPriseDBName);
            MaterialStagingDTO objMaterialStagingDTO = objMaterialStagingAPIController.GetRecord(Guid.Parse(MSGUID), SessionHelper.RoomID, SessionHelper.CompanyID);
            if (objMaterialStagingDTO == null)
            {
                objMaterialStagingDTO = new MaterialStagingDTO();
            }
            return PartialView("StagingLineItems", objMaterialStagingDTO);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult SaveMaterialStaging(MaterialStagingDTO objDTO)
        {
            MaterialStagingDAL objMaterialStagingAPIController = new MaterialStagingDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            string message = "";
            string status = "";
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.IsArchived = false;
            objDTO.IsDeleted = false;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.RoomName = SessionHelper.RoomName;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.CreatedByName = SessionHelper.UserName;
            if (objDTO.StagingLocationName == ResMaterialStaging.HintStagingName)
            {
                objDTO.StagingLocationName = string.Empty;
            }
            if (objDTO.StagingLocationName == string.Empty || string.IsNullOrEmpty(objDTO.StagingLocationName) || objDTO.StagingLocationName == null)
            {
                objDTO.StagingLocationName = "[|EmptyStagingBin|]";
            }
            if (string.IsNullOrWhiteSpace(objDTO.StagingName))
            {
                message = string.Format(ResMessage.Required, ResMaterialStaging.StagingName);
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string strOK = string.Empty;
                long SessionUserId = SessionHelper.UserID;
                if (objDTO.ID == 0)
                {
                    var isDuplicate = objMaterialStagingAPIController.MaterialStagingDuplicatecheck(objDTO.GUID, objDTO.StagingName, SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (isDuplicate)
                    {
                        message = string.Format(ResMessage.DuplicateMessage, ResMaterialStaging.StagingName, objDTO.StagingName);
                        status = "duplicate";
                    }
                    else
                    {
                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        long ReturnVal = objMaterialStagingAPIController.SaveMaterialStaging(objDTO, SessionUserId);
                        if (ReturnVal > 0)
                        {
                            message = ResMessage.SaveMessage;
                            status = "ok";
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);
                            status = "fail";
                        }

                    }
                }
                else
                {
                    var isDuplicate = objMaterialStagingAPIController.MaterialStagingDuplicatecheck(objDTO.GUID, objDTO.StagingName, SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (isDuplicate)
                    {
                        message = string.Format(ResMessage.DuplicateMessage, ResMaterialStaging.StagingName, objDTO.StagingName);
                        status = "duplicate";
                    }
                    else
                    {
                        if (objDTO.IsOnlyFromItemUI)
                        {
                            objDTO.EditedFrom = "Web";
                            objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        }
                        long ReturnVal = objMaterialStagingAPIController.SaveMaterialStaging(objDTO, SessionUserId);
                        if (ReturnVal > 0)
                        {
                            message = ResMessage.SaveMessage;
                            status = "ok";
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);
                            status = "fail";
                        }
                    }
                }
            }

            Session["IsInsert"] = "True";

            return Json(new { Message = message, Status = status, UpdatedDTO = objDTO }, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult LoadItemMasterModel(string MatStagID)
        public ActionResult LoadItemMasterModel(string ParentId, string ParentGuid)
        {
            MaterialStagingDTO objMaterialStagingDTO = new MaterialStagingDTO();
            ViewBag.MatStagDTO = objMaterialStagingDTO;
            //if (!string.IsNullOrWhiteSpace(MatStagID))
            if (!string.IsNullOrWhiteSpace(ParentId))
            {
                long MatStageID = 0;
                //if (long.TryParse(MatStagID, out MatStageID))
                if (long.TryParse(ParentId, out MatStageID))
                {
                    MaterialStagingDAL objMaterialStagingAPIController = new MaterialStagingDAL(SessionHelper.EnterPriseDBName);
                    objMaterialStagingDTO = objMaterialStagingAPIController.GetRecord(MatStageID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    ViewBag.MatStagDTO = objMaterialStagingDTO;
                }
            }
            ItemModelPerameter obj = new ItemModelPerameter()
            {
                AjaxURLAddItemToSession = "~/Inventory/AddItemToMSSession/",
                //PerentID = MatStagID,
                PerentID = ParentId,
                PerentGUID = ParentGuid,
                ModelHeader = ResMaterialStaging.AddItemsForMaterialStaging, 
                AjaxURLAddMultipleItemToSession = "~/Inventory/AddItemToMSSessionMultiple/",
                AjaxURLToFillItemGrid = "~/Inventory/GetItemsModelMethodMS/"
            };

            int TotalRecordCount = 0; 
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            Session["ItemMasterList"] = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetPagedItemsForModel(0, Int32.MaxValue, out TotalRecordCount, string.Empty, "ID ASC", SessionHelper.RoomID, SessionHelper.CompanyID, false, false, SessionHelper.UserSupplierIds, true, true, true, 0, "staging", Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone, true, null, null).ToList();

            return PartialView("_ItemMasterModel", obj);
        }

        public ActionResult GetItemsModelMethodMS(JQueryDataTableParamModel param)
        {
            Guid MSID = Guid.Empty;
            Guid.TryParse(Convert.ToString(Request["MSGUID"]), out MSID);
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

            bool IsArchived = false;
            bool IsDeleted = false;

            // set the default column sorting here, if first time then required to set 
            if (sortColumnName == "0" || sortColumnName == "undefined")
                sortColumnName = "ID";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;
            int TotalRecordCount = 0;
            string ItemsIDs = string.Empty;

            MaterialStagingDetailDAL objMaterialStagingDetailAPIController = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName);
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            ItemLocationDetailsDAL objLocationDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            BarcodeMasterDAL objBarcodeMasterDAL = new BarcodeMasterDAL(SessionHelper.EnterPriseDBName);
            // .Where(x=>x.ItemType != 4); , as Labour Type item not required in this module
            List<ItemMasterDTO> DataFromDB = new List<ItemMasterDTO>();
            if (param.sSearch != null && param.sSearch.Contains("QLGuid="))
            {
                string QLGuid = "";
                int QLQty = 1;
                if (param.sSearch.Contains("#"))
                {
                    QLGuid = param.sSearch.Split('#')[0].Replace("QLGuid=", "");
                    if (!Int32.TryParse(param.sSearch.Split('#')[1].Replace("Qty=", ""), out QLQty))
                    {
                        QLQty = 1;
                    }
                }
                else
                {
                    QLGuid = param.sSearch.Replace("QLGuid=", "");
                    QLQty = 1;
                }

                QuickListDAL objQLDtlDAL = new QuickListDAL(SessionHelper.EnterPriseDBName);
                List<QuickListDetailDTO> objQLDtlDTO = objQLDtlDAL.GetQuickListItemsRecords(SessionHelper.RoomID, SessionHelper.CompanyID, param.sSearch.Replace("QLGuid=", "").ToString(), SessionHelper.UserSupplierIds).ToList();
                ItemMasterDAL objItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                if (objQLDtlDTO.Count > 0)
                {
                    TotalRecordCount = objQLDtlDTO.Count;
                    
                    foreach (QuickListDetailDTO qlItem in objQLDtlDTO)
                    {
                        qlItem.Quantity = qlItem.Quantity * QLQty;
                        ItemMasterDTO tempItemDTO = new ItemMasterDTO();
                        tempItemDTO = objItemDAL.GetItemWithMasterTableJoins(null, (qlItem.ItemGUID ?? Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                    
                        if (tempItemDTO != null && tempItemDTO.ItemType != 4)
                        {
                            tempItemDTO.DefaultLocationName = qlItem.BinName;
                            tempItemDTO.DefaultLocation = qlItem.BinID;
                            tempItemDTO.DefaultPullQuantity = qlItem.Quantity;
                            DataFromDB.Add(tempItemDTO);
                        }
                    }
                }
            }
            else
            {
                TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
                DataFromDB = objItemMasterDAL.GetPagedItemsForModelMS(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, SessionHelper.UserSupplierIds, true, true, true, SessionHelper.UserID, "staging", Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone, true).ToList();
            }

            if (DataFromDB != null)
            {
                DataFromDB.ToList().ForEach(t =>
                {
                    t.CreatedDate = CommonUtility.ConvertDateByTimeZone(t.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                    t.UpdatedDate = CommonUtility.ConvertDateByTimeZone(t.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);

                });
            }
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMsLineItems(JQueryDataTableParamModel param)
        {
            //Int64 MSID = 0;
            //Int64.TryParse(Convert.ToString(Request["MSID"]), out MSID);

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
            sortColumnName = Request["SortingField"].ToString().Trim();

            // set the default column sorting here, if first time then required to set 
            //if (string.IsNullOrWhiteSpace(sortColumnName) || sortColumnName == "0" || sortColumnName == "undefined" || sortColumnName.Trim() == "ID")
            //    sortColumnName = "ItemNumber";
            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "ItemNumber";
            }
            else
                sortColumnName = "ItemNumber";

            if (sortDirection == "asc")
                sortColumnName += " asc";
            else
                sortColumnName += " desc";


            string searchQuery = string.Empty;
            Int64 MaterialStagingID = 0;
            Int64.TryParse(Request["ParentID"], out MaterialStagingID);

            int TotalRecordCount = 0;

            MaterialStagingDetailDAL objMaterialStagingDetailAPIController = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName);
            bool IsArchived = false;
            bool IsDeleted = false;

            IEnumerable<MaterialStagingDetailDTO> DataFromDB = objMaterialStagingDetailAPIController.GetPagedStagingLineItems(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, Guid.Parse(Request["MSGUID"].ToString()), SessionHelper.UserSupplierIds);

            if (DataFromDB.Any())
            {
                DataFromDB = (from itm in DataFromDB
                              select new MaterialStagingDetailDTO
                              {
                                  GUID = itm.GUID,
                                  BinName = itm.BinName,
                                  StagingBinName = itm.StagingBinName == "[|EmptyStagingBin|]" ? string.Empty : itm.StagingBinName,
                                  // comment below line to resolve object ref error while move.
                                  //StagingBinName = itm.StagingBinName == "[|EmptyStagingBin|]" ? string.Empty : itm.StagingBinName,
                                  BinID = itm.BinID,
                                  ItemGUID = itm.ItemGUID,
                                  ItemNumber = itm.ItemNumber,
                                  Description = itm.Description,
                                  SerialNumberTracking = itm.SerialNumberTracking,
                                  LotNumberTracking = itm.LotNumberTracking,
                                  DateCodeTracking = itm.DateCodeTracking,
                                  Quantity = itm.Quantity,
                                  MaterialStagingGUID = itm.MaterialStagingGUID,
                                  RoomId = itm.RoomId,
                                  CompanyID = itm.CompanyID,
                                  StagingBinID = itm.StagingBinID,
                                  RoomName = itm.StagingBinName == "[|EmptyStagingBin|]" ? string.Empty : itm.StagingBinName,
                              });
            }

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ItemLocDet(string ItemGUID, string MSGUID, long? StageBinId, string staLocName)
        {
            ItemLocationDetailsDAL objItemLocationDetailsController = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            BinMasterDAL objBinMasterController = new BinMasterDAL(SessionHelper.EnterPriseDBName);

            List<ItemLocationDetailsDTO> lstBins = new List<ItemLocationDetailsDTO>();
            ViewBag.Itembins = lstBins;
            ViewBag.ItemGUID = ItemGUID;// ?? Guid.Empty;
            ViewBag.MsId = MSGUID;
            ViewBag.StageBinId = StageBinId ?? 0;
            ViewBag.staLocName = staLocName;

            //get item SerialNumberTracking
            ItemMasterDTO objItem = null;
            Guid ItemGUID1 = Guid.Empty;
            if (Guid.TryParse(ItemGUID, out ItemGUID1))
            {
                objItem = (new ItemMasterDAL(SessionHelper.EnterPriseDBName)).GetItemWithoutJoins(null, ItemGUID1);
            }

            ViewBag.SerialNumberTracking = objItem.SerialNumberTracking;

            ItemLocationQTYDAL objItemLocationQTYDAL = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName);

            if (ItemGUID != "")
            {
                lstBins = objItemLocationQTYDAL.GetItemBinsHaveQty(Guid.Parse(ItemGUID), SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
                ViewBag.Itembins = lstBins;
            }
            return PartialView("_ItemLocationsForStagging");
        }

        public ActionResult ItemLocDetHistory(string ItemGUID, string MsId, long? StageBinId, string staLocName)
        {
            ItemLocationDetailsDAL objItemLocationDetailsController = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            BinMasterDAL objBinMasterController = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            List<ItemLocationDetailsDTO> lstBins = new List<ItemLocationDetailsDTO>();
            ViewBag.Itembins = lstBins;
            ViewBag.ItemGUID = ItemGUID;
            ViewBag.MsId = MsId ?? "";
            ViewBag.StageBinId = StageBinId ?? 0;
            ViewBag.staLocName = staLocName;
            ItemLocationQTYDAL objItemLocationQTYDAL = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName);

            if (ItemGUID != "")
            {
                lstBins = objItemLocationQTYDAL.GetItemBinsHaveQty(Guid.Parse(ItemGUID), SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
                ViewBag.Itembins = lstBins;
            }
            return PartialView("_ItemLocationsForStagging_History");
        }

        public JsonResult GetBins(string featureClass, string style, string maxRows, string name_startsWith, Guid? StagingGUID, Guid? ItemGUID)
        {
            BinMasterDAL objBinMasterController = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            List<BinMasterDTO> lstBins = objBinMasterController.GetStagingBinByItem(SessionHelper.RoomID, SessionHelper.CompanyID, (ItemGUID ?? Guid.Empty)).ToList();
            //List<BinMasterDTO> lstBins = objBinMasterController.GetBinMasterByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
            //remove condition of MaterialStagingGUID to resolve an issue Wi-852
            if (name_startsWith == " ")
            {
                //lstBins = lstBins.Where(t => t.BinNumber != null && t.MaterialStagingGUID == (StagingGUID ?? Guid.Empty) && t.ItemGUID == (ItemGUID ?? Guid.Empty) && (t.IsArchived ?? false) == false && t.IsStagingLocation == true && (t.IsDeleted ?? false) == false).OrderBy(t => t.BinNumber).ToList();
                lstBins = lstBins.Where(t => t.BinNumber != null && t.BinNumber != "[|EmptyStagingBin|]" && t.ItemGUID == (ItemGUID ?? Guid.Empty) && (t.IsArchived ?? false) == false && t.IsStagingLocation == true && (t.IsDeleted ?? false) == false).OrderBy(t => t.BinNumber).ToList();
            }
            else
            {
                //lstBins = lstBins.Where(t => t.BinNumber != null && t.MaterialStagingGUID == (StagingGUID ?? Guid.Empty) && t.ItemGUID == (ItemGUID ?? Guid.Empty) && (t.IsArchived ?? false) == false && t.IsStagingLocation == true && (t.IsDeleted ?? false) == false && t.BinNumber.ToLower().Contains(name_startsWith.ToLower())).OrderBy(t => t.BinNumber).ToList();
                lstBins = lstBins.Where(t => t.BinNumber != null && t.BinNumber != "[|EmptyStagingBin|]" && t.ItemGUID == (ItemGUID ?? Guid.Empty) && (t.IsArchived ?? false) == false && t.IsStagingLocation == true && (t.IsDeleted ?? false) == false && t.BinNumber.ToLower().Contains(name_startsWith.ToLower())).OrderBy(t => t.BinNumber).ToList();
            }
            return Json(lstBins, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAvailableQty(long? BinId, string ItemGUID, string MsdtlGUID)
        {
            MaterialStagingDetailDTO objMaterialStagingDetailDTO = null;
            MaterialStagingDetailDAL objMaterialStagingDetailDAL = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName);
            ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
            ItemLocationDetailsDAL objItemLocationDetailsDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            double msdCustOwnQty = 0, msdConsignQty = 0, msdAvalQTY = 0;
            if (MsdtlGUID != "")
            {
                //objMaterialStagingDetailDTO = objMaterialStagingDetailDAL.GetRecord(Guid.Parse(MsdtlGUID), SessionHelper.RoomID, SessionHelper.CompanyID);
                objMaterialStagingDetailDTO = objMaterialStagingDetailDAL.GetMaterialStagingDetailByGUID(Guid.Parse(MsdtlGUID), SessionHelper.RoomID, SessionHelper.CompanyID);

                if (objMaterialStagingDetailDTO != null)
                {
                    if (objMaterialStagingDetailDTO.BinID == (BinId ?? 0))
                    {
                        List<MaterialStagingPullDetailDTO> listdetails = new MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName).GetMsPullDetailsByMsDetailsId(Guid.Parse(MsdtlGUID));
                        if (listdetails != null && listdetails.Count > 0)
                        {
                            msdCustOwnQty = listdetails.Sum(t => (t.CustomerOwnedQuantity ?? 0));
                            msdConsignQty = listdetails.Sum(t => (t.ConsignedQuantity ?? 0));
                            msdAvalQTY = listdetails.Sum(t => (t.PoolQuantity ?? 0));
                        }
                    }
                }
            }
            double CustOwnQty = 0, ConsignQty = 0, AvalQTY = 0;
            objItemMasterDTO = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, Guid.Parse(ItemGUID));
            if (objItemMasterDTO != null)
            {
                List<ItemLocationDetailsDTO> lstitmqty = objItemLocationDetailsDAL.GetItemQuantityByLocation((BinId ?? 0), Guid.Parse(ItemGUID), SessionHelper.RoomID, SessionHelper.CompanyID);
                if (lstitmqty != null && lstitmqty.Count > 0)
                {
                    CustOwnQty = lstitmqty.Sum(t => (t.CustomerOwnedQuantity ?? 0));
                    ConsignQty = lstitmqty.Sum(t => (t.ConsignedQuantity ?? 0));
                    if (objItemMasterDTO.Consignment)
                    {
                        AvalQTY = CustOwnQty + ConsignQty;
                    }
                    else
                    {
                        AvalQTY = CustOwnQty;
                    }
                }
            }
            return Json(new { CustOwnQty = (CustOwnQty + msdCustOwnQty), ConsignQty = (ConsignQty + msdConsignQty), AvalQTY = (AvalQTY + msdAvalQTY) });
        }

        public JsonResult AddupdateLineItems(long? MsdtlId, long? ItemID, Guid? MsGUID)
        {
            return Json(null);
        }

        public ActionResult MSItemDetailsAJAX(JQueryDataTableParamModel param)
        {
            //Int64 ItemID = 0;
            //Int64 MSID = 0;
            Int64 MSLineItemStagingBinID = 0;
            //Int64.TryParse(Request["ItemID"].ToString(), out ItemID);
            //Int64.TryParse(Request["MSID"].ToString(), out MSID);
            Int64.TryParse(Request["MSLIBinID"].ToString(), out MSLineItemStagingBinID);


            ///////////// requried when paging needs in this method /////////////////
            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"].ToString();

            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());


            // set the default column sorting here, if first time then required to set 
            if (sortColumnName == "0" || sortColumnName == "undefined" || sortColumnName == "ShippingMethod")
                sortColumnName = "ID";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            ///////////// requried when paging needs in this method /////////////////

            ViewBag.ItemGUID = Guid.Parse(Request["ItemGUID"].ToString());
            MaterialStagingDetailDAL objAPI = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName);
            int TotalRecordCount = 0;
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var objModel = objAPI.GetPagedRecordsByItem(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, Guid.Parse(Request["ItemGUID"].ToString()), Guid.Parse(Request["MSGUID"].ToString()), MSLineItemStagingBinID, Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = objModel
            },
                        JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveItemToStaging(string msGUID, string msdtlGUID, string ItemGUID, double? Qty, long? BinID, long? StagingBinID, string StagingBinName)
        {
            string message = "";
            string status = "";
            try
            {
                long SessionUserId = SessionHelper.UserID;
                if (msdtlGUID != "")
                {
                    MaterialStagingDetailDAL objMaterialStagingDetailDAL = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName);
                    //MaterialStagingDetailDTO objMaterialStagingDetailDTO = objMaterialStagingDetailDAL.GetRecord(Guid.Parse(msdtlGUID), SessionHelper.RoomID, SessionHelper.CompanyID);
                    MaterialStagingDetailDTO objMaterialStagingDetailDTO = objMaterialStagingDetailDAL.GetMaterialStagingDetailByGUID(Guid.Parse(msdtlGUID), SessionHelper.RoomID, SessionHelper.CompanyID);
                    MaterialStagingDetailDTO objMaterialStagingDetailDTOUpdated = (MaterialStagingDetailDTO)objMaterialStagingDetailDTO.Clone();
                    objMaterialStagingDetailDTOUpdated.BinID = BinID ?? 0;
                    objMaterialStagingDetailDTOUpdated.Quantity = Qty ?? 0;
                    objMaterialStagingDetailDTOUpdated.StagingBinName = StagingBinName;

                    if (objMaterialStagingDetailDTO != null)
                    {
                        if (objMaterialStagingDetailDTOUpdated.Quantity > 0)
                        {
                            ItemMasterDTO objItemMasterDTO = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, objMaterialStagingDetailDTOUpdated.ItemGUID);
                            if (objItemMasterDTO != null)
                            {
                                MaterialStagingPullDetailDTO objMaterialStagingPullDtl = objMaterialStagingDetailDAL.GetAvailableQty(objMaterialStagingDetailDTOUpdated, objItemMasterDTO);

                                if (objMaterialStagingDetailDTO.BinID == objMaterialStagingDetailDTOUpdated.BinID)
                                {
                                    double msdCustOwnQty = 0, msdConsignQty = 0, msdAvalQTY = 0;
                                    List<MaterialStagingPullDetailDTO> listdetails = new MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName).GetMsPullDetailsByMsDetailsId(Guid.Parse(msdtlGUID));

                                    if (listdetails != null && listdetails.Count > 0)
                                    {
                                        msdCustOwnQty = listdetails.Sum(t => (t.CustomerOwnedQuantity ?? 0));
                                        msdConsignQty = listdetails.Sum(t => (t.ConsignedQuantity ?? 0));
                                        msdAvalQTY = listdetails.Sum(t => (t.PoolQuantity ?? 0));
                                        objMaterialStagingPullDtl.ActualAvailableQuantity = objMaterialStagingPullDtl.ActualAvailableQuantity + msdAvalQTY;
                                    }
                                }
                                if (objMaterialStagingPullDtl.ActualAvailableQuantity >= objMaterialStagingDetailDTOUpdated.Quantity)
                                {
                                    string MSG = string.Empty;
                                    if (objMaterialStagingDetailDAL.MSDeleteManageInventory(objMaterialStagingDetailDTO, SessionHelper.RoomID, SessionHelper.CompanyID, out MSG, SessionUserId, SessionHelper.EnterPriceID))
                                    {
                                        StagingActionResult objStagingActionResult = objMaterialStagingDetailDAL.AddQuantityToStagingBin(objMaterialStagingDetailDTOUpdated, SessionUserId,SessionHelper.EnterPriceID);
                                        if (objStagingActionResult.ReturnCode == 1)
                                        {
                                            double totalqty = objMaterialStagingDetailDAL.GetTotalQtyonStagingLineItem(objMaterialStagingDetailDTOUpdated.MaterialStagingGUID.Value, objMaterialStagingDetailDTOUpdated.ItemGUID.Value, objMaterialStagingDetailDTOUpdated.StagingBinName);
                                            message = ResMessage.SaveMessage;
                                            status = "ok";
                                            return Json(new { Message = message, Status = status, TotalQuantity = totalqty }, JsonRequestBehavior.AllowGet);
                                        }
                                        else
                                        {
                                            message = objStagingActionResult.ReturnMessage;
                                            status = "fail";
                                            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                                        }
                                    }
                                    else
                                    {
                                        message = ResMaterialStaging.DeleteFailed; 
                                        status = "fail";
                                        return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                                    }

                                }
                                else
                                {
                                    message = ResMaterialStagingDetail.errAvailableQty;
                                    status = "fail";
                                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                                }

                            }
                            else
                            {
                                message = ResMaterialStagingDetail.errNOQtyZero;
                                status = "fail";
                                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            message = ResMaterialStagingDetail.errNOQtyZero;
                            status = "fail";
                            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                        }

                    }
                    else
                    {
                        message = ResMaterialStagingDetail.errNOQtyZero;
                        status = "fail";
                        return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    MaterialStagingDetailDTO objMaterialStagingDetailDTO = new MaterialStagingDetailDTO();
                    objMaterialStagingDetailDTO.MaterialStagingGUID = Guid.Parse(msGUID);
                    objMaterialStagingDetailDTO.GUID = Guid.NewGuid(); //Guid.Parse(msdtlGUID);
                    objMaterialStagingDetailDTO.ItemGUID = Guid.Parse(ItemGUID);
                    objMaterialStagingDetailDTO.Quantity = Qty ?? 0;
                    objMaterialStagingDetailDTO.BinID = BinID ?? 0;
                    objMaterialStagingDetailDTO.StagingBinID = StagingBinID ?? 0;
                    objMaterialStagingDetailDTO.StagingBinName = StagingBinName;
                    objMaterialStagingDetailDTO.RoomId = SessionHelper.RoomID;
                    objMaterialStagingDetailDTO.CompanyID = SessionHelper.CompanyID;
                    objMaterialStagingDetailDTO.Created = DateTimeUtility.DateTimeNow;
                    objMaterialStagingDetailDTO.LastUpdatedBy = SessionHelper.UserID;
                    objMaterialStagingDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                    objMaterialStagingDetailDTO.CreatedBy = SessionHelper.UserID;
                    objMaterialStagingDetailDTO.RoomName = SessionHelper.RoomName;
                    objMaterialStagingDetailDTO.CreatedByName = SessionHelper.UserName;
                    objMaterialStagingDetailDTO.UpdatedByName = SessionHelper.UserName;
                    if (objMaterialStagingDetailDTO.Quantity > 0)
                    {
                        MaterialStagingDetailDAL objMaterialStagingDetailDAL = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName);
                        ItemMasterDTO objItemMasterDTO = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, objMaterialStagingDetailDTO.ItemGUID);
                        if (objItemMasterDTO != null)
                        {
                            MaterialStagingPullDetailDTO objMaterialStagingPullDtl = objMaterialStagingDetailDAL.GetAvailableQty(objMaterialStagingDetailDTO, objItemMasterDTO);
                            if (objMaterialStagingPullDtl.ActualAvailableQuantity >= objMaterialStagingDetailDTO.Quantity)
                            {
                                StagingActionResult objStagingActionResult = objMaterialStagingDetailDAL.AddQuantityToStagingBin(objMaterialStagingDetailDTO, SessionUserId,SessionHelper.EnterPriceID);
                                if (objStagingActionResult.ReturnCode == 1)
                                {
                                    double totalqty = objMaterialStagingDetailDAL.GetTotalQtyonStagingLineItem(objMaterialStagingDetailDTO.MaterialStagingGUID.Value, objMaterialStagingDetailDTO.ItemGUID.Value, objMaterialStagingDetailDTO.StagingBinName);
                                    message = ResMessage.SaveMessage;
                                    status = "ok";
                                    return Json(new { Message = message, Status = status, TotalQuantity = totalqty }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    message = objStagingActionResult.ReturnMessage;
                                    status = "fail";
                                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                                }

                            }
                            else
                            {
                                message = ResMaterialStagingDetail.errAvailableQty;
                                status = "fail";
                                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            message = ResMaterialStagingDetail.errNOQtyAvailable;
                            status = "fail";
                            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        message = ResMaterialStagingDetail.errNOQtyZero;
                        status = "fail";
                        return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                    }
                }

            }
            catch (Exception ex)
            {
                message = ex.Message;
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult DeleteMSDtlItem(string ids)
        {
            string message = string.Empty;
            string status = string.Empty;
            try
            {
                long SessionUserId = SessionHelper.UserID;
                if (!string.IsNullOrWhiteSpace(ids))
                {
                    ids = ids.TrimEnd(',');
                    string[] arrItems = ids.Split(',').ToArray();
                    MaterialStagingDetailDAL objMaterialStagingDetailDAL = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName);
                    var enterprisId = SessionHelper.EnterPriceID;
                    foreach (var item in arrItems)
                    {
                        //MaterialStagingDetailDTO objMaterialStagingDetailDTO = objMaterialStagingDetailDAL.GetRecord(Guid.Parse(item.ToString()), SessionHelper.RoomID, SessionHelper.CompanyID);
                        MaterialStagingDetailDTO objMaterialStagingDetailDTO = objMaterialStagingDetailDAL.GetMaterialStagingDetailByGUID(Guid.Parse(item.ToString()), SessionHelper.RoomID, SessionHelper.CompanyID);
                        if (objMaterialStagingDetailDTO != null)
                        {
                            objMaterialStagingDetailDAL.DeleteSingleMSDtlItem(objMaterialStagingDetailDTO, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, Convert.ToString(SessionHelper.RoomDateFormat), SessionUserId,enterprisId);
                        }
                    }
                    status = "ok";
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    message = string.Format(ResMessage.SaveErrorMsg, 0);
                    status = "fail";
                    return Json(status, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                status = "fail";
                return Json(status, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetMSDtlItem(string GUID)
        {
            //MaterialStagingDetailAPIController objMaterialStagingDetailAPIController = new MaterialStagingDetailAPIController();
            MaterialStagingDetailDAL objMaterialStagingDetailAPIController = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName);

            if (GUID != "")
            {
                //MaterialStagingDetailDTO objMaterialStagingDetailDTO = objMaterialStagingDetailAPIController.GetRecord(Guid.Parse(GUID), SessionHelper.RoomID, SessionHelper.CompanyID);
                MaterialStagingDetailDTO objMaterialStagingDetailDTO = objMaterialStagingDetailAPIController.GetMaterialStagingDetailByGUID(Guid.Parse(GUID), SessionHelper.RoomID, SessionHelper.CompanyID);
                if (objMaterialStagingDetailDTO != null)
                {
                    objMaterialStagingDetailDTO.BinName = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(objMaterialStagingDetailDTO.BinID.GetValueOrDefault(0), SessionHelper.RoomID, SessionHelper.CompanyID).BinNumber;
                    //objMaterialStagingDetailDTO.BinName = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocation( SessionHelper.RoomID, SessionHelper.CompanyID, false, false,Guid.Empty, objMaterialStagingDetailDTO.BinID.GetValueOrDefault(0),null,null).FirstOrDefault().BinNumber;
                    double CustOwnQty = 0, ConsignQty = 0, AvalQTY = 0;
                    double msdCustOwnQty = 0, msdConsignQty = 0, msdAvalQTY = 0;
                    List<MaterialStagingPullDetailDTO> listdetails = new MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName).GetMsPullDetailsByMsDetailsId(Guid.Parse(GUID));

                    if (listdetails != null && listdetails.Count > 0)
                    {
                        msdCustOwnQty = listdetails.Sum(t => (t.CustomerOwnedQuantity ?? 0));
                        msdConsignQty = listdetails.Sum(t => (t.ConsignedQuantity ?? 0));
                        msdAvalQTY = msdCustOwnQty + msdConsignQty;
                        //msdAvalQTY = listdetails.Sum(t => (t.PoolQuantity ?? 0));
                    }
                    ItemMasterDTO objItemMasterDTO = new ItemMasterDTO();
                    ItemLocationDetailsDAL objItemLocationDetailsDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);

                    objItemMasterDTO = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, objMaterialStagingDetailDTO.ItemGUID);
                    if (objItemMasterDTO != null)
                    {
                        List<ItemLocationDetailsDTO> lstitmqty = objItemLocationDetailsDAL.GetItemQuantityByLocation(objMaterialStagingDetailDTO.BinID.GetValueOrDefault(0), objMaterialStagingDetailDTO.ItemGUID.Value, SessionHelper.RoomID, SessionHelper.CompanyID);
                        if (lstitmqty != null && lstitmqty.Count > 0)
                        {
                            CustOwnQty = lstitmqty.Sum(t => (t.CustomerOwnedQuantity ?? 0));
                            ConsignQty = lstitmqty.Sum(t => (t.ConsignedQuantity ?? 0));
                            if (objItemMasterDTO.Consignment)
                            {
                                AvalQTY = CustOwnQty + ConsignQty;
                            }
                            else
                            {
                                AvalQTY = CustOwnQty;
                            }
                        }
                    }
                    return Json(new { CustOwnQty = (CustOwnQty + msdCustOwnQty), ConsignQty = (ConsignQty + msdConsignQty), AvalQTY = (AvalQTY + msdAvalQTY), objDTO = objMaterialStagingDetailDTO });
                }

            }
            return Json(null);

        }

        [HttpPost]
        public JsonResult DeleteMaterialStagingLineItems(string ids, string MaterialStagingGUId)
        {
            try
            {
                //MaterialStagingDetailAPIController objMaterialStagingDetailAPIController = new MaterialStagingDetailAPIController();
                MaterialStagingDetailDAL objMaterialStagingDetailAPIController = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName);
                List<MaterialStagingDetailDTO> lstItemsToDelete = new List<MaterialStagingDetailDTO>();
                Guid ItmGUID = Guid.Empty;
                long MSBinLocID = 0;
                var enterpriseId = SessionHelper.EnterPriceID;
                long SessionUserId = SessionHelper.UserID;
                long[] itmids = new long[] { 0 };
                if (!string.IsNullOrWhiteSpace(ids))
                {
                    if (ids.Contains(','))
                    {
                        ids = ids.TrimEnd(',');
                        string[] strids = ids.Split(',');
                        foreach (var item in strids)
                        {
                            string[] arrinner = item.Split('_');
                            Guid.TryParse(arrinner[0], out ItmGUID);
                            long.TryParse(arrinner[1], out MSBinLocID);

                            MaterialStagingDetailDTO objMaterialStagingDetailDTO = new MaterialStagingDetailDTO();
                            objMaterialStagingDetailDTO.MaterialStagingGUID = Guid.Parse(MaterialStagingGUId);
                            objMaterialStagingDetailDTO.ItemGUID = ItmGUID;
                            objMaterialStagingDetailDTO.StagingBinID = MSBinLocID;
                            lstItemsToDelete.Add(objMaterialStagingDetailDTO);

                        }
                    }

                    objMaterialStagingDetailAPIController.DeleteMSLineItems(lstItemsToDelete, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, Convert.ToString(SessionHelper.RoomDateFormat), SessionUserId,enterpriseId);
                    return Json("ok");
                }
                return Json("fail");
            }
            catch (Exception)
            {

                return Json("fail");
            }

        }

        public JsonResult DeleteMaterialStagingRecords(string ids)
        {
            try
            {
                long SessionUserId = SessionHelper.UserID;
                MaterialStagingDetailDAL objMaterialStagingDetailAPIController = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName);
                objMaterialStagingDetailAPIController.DeleteMSHeaderItems(ids, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, Convert.ToString(SessionHelper.RoomDateFormat), SessionUserId,SessionHelper.EnterPriceID);
                //return "ok";
                //string response = string.Empty;
                //CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                //response = objCommonDAL.DeleteModulewise(ids, "MaterialStaging", true, SessionHelper.UserID);

                //eTurns.DAL.CacheHelper<IEnumerable<MaterialStagingDTO>>.InvalidateCache();
                //eTurns.DAL.CacheHelper<IEnumerable<MaterialStagingDetailDTO>>.InvalidateCache();

                return Json(new { Message = ResMessage.DeletedSuccessfully, Status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult UpdateStagingLineItem(string MsGUID, string ItmGUID, long? msbinID, string StagingBinName)
        {
            if ((MsGUID != "") && (ItmGUID != "") && !string.IsNullOrWhiteSpace(StagingBinName) && (msbinID ?? 0) > 0)
            {
                try
                {
                    //MaterialStagingAPIController objMaterialStagingAPIController = new MaterialStagingAPIController();
                    MaterialStagingDAL objMaterialStagingAPIController = new MaterialStagingDAL(SessionHelper.EnterPriseDBName);
                    objMaterialStagingAPIController.UpdateMaterialStagingLineItem(Guid.Parse(MsGUID), Guid.Parse(ItmGUID), msbinID.Value, StagingBinName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.UserName, SessionHelper.RoomName);
                    return Json("ok");
                }
                catch (Exception)
                {
                    return Json("fail");
                }
            }
            else
            {
                return Json("fail");
            }
        }

        [HttpPost]
        public JsonResult UpdateStagingLineItemBulk(List<MaterialStagingDetailDTO> lstDTO)
        {
            //MaterialStagingAPIController objMaterialStagingAPIController = new MaterialStagingAPIController();
            MaterialStagingDAL objMaterialStagingAPIController = new MaterialStagingDAL(SessionHelper.EnterPriseDBName);
            try
            {
                foreach (MaterialStagingDetailDTO objMaterialStagingDetailDTO in lstDTO)
                {
                    if (objMaterialStagingDetailDTO.BinName.Trim() != objMaterialStagingDetailDTO.RoomName.Trim())
                    {
                        objMaterialStagingAPIController.UpdateMaterialStagingLineItem(objMaterialStagingDetailDTO.MaterialStagingGUID.Value, objMaterialStagingDetailDTO.ItemGUID.Value, objMaterialStagingDetailDTO.StagingBinID, objMaterialStagingDetailDTO.RoomName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.UserName, SessionHelper.RoomName);
                    }

                }

                return Json("ok");
            }
            catch (Exception)
            {
                return Json("fail");
            }
        }

        public void BlankSession()
        {
            Session["IsInsert"] = "";
        }

        public ActionResult MaterialStagingHistory()
        {
            return PartialView("_MaterialStagingHistory");
        }

        public JsonResult GetAllStagingHeaders(string NameStartWith)
        {
            MaterialStagingDAL objStagingDAL = new MaterialStagingDAL(SessionHelper.EnterPriseDBName);
            //IEnumerable<MaterialStagingDTO> StagingHeaderList = objStagingDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).OrderBy(t => t.StagingName);
            IEnumerable<MaterialStagingDTO> StagingHeaderList = objStagingDAL.GetMaterialStaging(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, string.Empty, null).OrderBy(t => t.StagingName);
            if (StagingHeaderList != null && StagingHeaderList.Count() > 0)
            {
                if (!string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                {
                    StagingHeaderList = StagingHeaderList.Where(x => x.StagingName.ToLower().StartsWith(NameStartWith.ToLower())).OrderBy(t => t.StagingName);
                    //return Json(new { ReturnListItems = StagingHeaderList, Message = "success", Status = "ok" }, JsonRequestBehavior.AllowGet);
                    return Json(StagingHeaderList, JsonRequestBehavior.AllowGet);
                }
                else if (NameStartWith.Contains(" "))
                {
                    //return Json(new { ReturnListItems = StagingHeaderList, Message = "success", Status = "ok" }, JsonRequestBehavior.AllowGet);
                    return Json(StagingHeaderList, JsonRequestBehavior.AllowGet);
                }
            }
            //return Json(new { ReturnListItems = StagingHeaderList, Message = "no record found", Status = "fail" }, JsonRequestBehavior.AllowGet);
            return Json(StagingHeaderList, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Get Item Locations For NewPull Grid
        /// </summary>
        /// <param name="ItemGuid"></param>
        /// <param name="NameStartWith"></param>
        /// <returns></returns>
        public JsonResult GetItemLocationsForNewPullGrid(Guid ItemGuid, string NameStartWith)
        {
            BinMasterDAL objBinDAL = null;
            IEnumerable<BinMasterDTO> lstBinList = null;
            ItemLocationQTYDAL objLocationQtyDAL = null;
            ItemLocationQTYDTO objLocatQtyDTO = null;
            List<BinMasterDTO> retunList = new List<BinMasterDTO>();
            IEnumerable<MaterialStagingDetailDTO> lstMSDetailDTO = null;
            MaterialStagingDetailDAL objMSDAL = null;
            Int64 RoomID = SessionHelper.RoomID;
            Int64 CompanyID = SessionHelper.CompanyID;
            string qtyFormat = "N";
            List<DTOForAutoComplete> returnKeyValList = new List<DTOForAutoComplete>();
            try
            {
                //if (SessionHelper.CompanyConfig != null)
                //    qtyFormat = "N" + SessionHelper.CompanyConfig.QuantityDecimalPoints;

                if (!string.IsNullOrEmpty(SessionHelper.NumberDecimalDigits))
                    qtyFormat = "N" + SessionHelper.NumberDecimalDigits;
                objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                //lstBinList = objBinDAL.GetAllRecords(RoomID, CompanyID, false, false).Where(x => x.ItemGUID.GetValueOrDefault(Guid.Empty) == ItemGuid);
                lstBinList = objBinDAL.GetInventoryAndStagingBinsByItem(RoomID, CompanyID, ItemGuid);
                //lstBinList = objBinDAL.GetItemLocation(RoomID, CompanyID, false, false, ItemGuid,0,null,null);//.Where(x => x.ItemGUID.GetValueOrDefault(Guid.Empty) == ItemGuid)
                var stagingResourceValue = ResPullMaster.Staging;
                foreach (var item in lstBinList)
                {
                    if (item.IsStagingLocation)
                    {
                        objMSDAL = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName);
                        //lstMSDetailDTO = objMSDAL.GetStagingLocationByItem(ItemGuid, RoomID, CompanyID).Where(x => x.Quantity > 0 && x.StagingBinID == item.ID); 
                        lstMSDetailDTO = objMSDAL.GetStagingLocationByItem(ItemGuid, RoomID, CompanyID, item.ID, string.Empty, true);
                        if (lstMSDetailDTO != null && lstMSDetailDTO.Count() > 0 && lstMSDetailDTO.Sum(x => x.Quantity) > 0)
                        {
                            DTOForAutoComplete obj = new DTOForAutoComplete()
                            {
                                Key = item.BinNumber,
                                Value = item.BinNumber == "[|EmptyStagingBin|]" ? string.Format(stagingResourceValue, lstMSDetailDTO.Sum(x => x.Quantity)) : item.BinNumber + " " + string.Format(stagingResourceValue, lstMSDetailDTO.Sum(x => x.Quantity)),
                                ID = item.ID,
                                GUID = item.GUID,
                            };
                            returnKeyValList.Add(obj);
                        }
                    }
                    else
                    {
                        objLocationQtyDAL = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName);
                        objLocatQtyDTO = objLocationQtyDAL.GetRecordByBinItem(ItemGuid, item.ID, RoomID, CompanyID);
                        if (objLocatQtyDTO != null && objLocatQtyDTO.Quantity > 0)
                        {
                            DTOForAutoComplete obj = new DTOForAutoComplete()
                            {
                                Key = item.BinNumber,
                                Value = item.BinNumber + " (" + objLocatQtyDTO.Quantity.ToString(qtyFormat) + ")",
                                ID = item.ID,
                                GUID = item.GUID,
                            };
                            returnKeyValList.Add(obj);
                        }
                    }
                }

                if (returnKeyValList.Count > 0 && !string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                {
                    returnKeyValList = returnKeyValList.Where(x => x.Key.ToLower().StartsWith(NameStartWith.ToLower())).ToList();
                }

                return Json(returnKeyValList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(returnKeyValList, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                objBinDAL = null;
                lstBinList = null;
                retunList = null;
                objLocationQtyDAL = null;
                objLocatQtyDTO = null;
                lstMSDetailDTO = null;
                objMSDAL = null;
            }
        }


        [HttpPost]
        public ActionResult StagePulltemQuantity(List<MaterialStagingPullDetailDTO> lstPullRequestInfo)
        {
            List<MaterialStagingPullDetailDTO> lstPullRequest = new List<MaterialStagingPullDetailDTO>();
            foreach (MaterialStagingPullDetailDTO objPullMasterDTO in lstPullRequestInfo)
            {
                if (!lstPullRequest.Select(x => x.ItemGUID).Contains(objPullMasterDTO.ItemGUID))
                    lstPullRequest.Add(objPullMasterDTO);
            }

            MaterialStagingPullDetailDAL objPullMasterDAL = new MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName);
            lstPullRequestInfo = objPullMasterDAL.GetStagePullWithDetails(lstPullRequest, SessionHelper.RoomID, SessionHelper.CompanyID);
            return PartialView("StagLotSrSelection", lstPullRequestInfo);
        }



        [HttpPost]
        public JsonResult PullSerialsAndLotsNew(List<ItemStagePullInfo> objItemPullInfo)
        {
            MaterialStagingDetailDAL objMSDetailDAL = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName);
            List<ItemStagePullInfo> oReturn = new List<ItemStagePullInfo>();
            List<ItemStagePullInfo> oReturnError = new List<ItemStagePullInfo>();
            long SessionUserId = SessionHelper.UserID;
            var enterpriseId = SessionHelper.EnterPriceID;

            foreach (ItemStagePullInfo item in objItemPullInfo)
            {

                if (item.lstItemPullDetails != null && item.lstItemPullDetails.Count > 0)
                {
                    item.lstItemPullDetails = item.lstItemPullDetails.Where(x => x.PullQuantity > 0).ToList();
                    if (item.lstItemPullDetails != null && item.lstItemPullDetails.Count > 0)
                    {
                        ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                        ItemMasterDTO objItemmasterDTO = new ItemMasterDTO();
                        objItemmasterDTO = objItemMasterDAL.GetItemWithoutJoins(null, item.ItemGUID);

                        ItemStagePullInfo oItemPullInfo = item;
                        oItemPullInfo.CompanyId = SessionHelper.CompanyID;
                        oItemPullInfo.RoomId = SessionHelper.RoomID;
                        oItemPullInfo.CreatedBy = SessionHelper.UserID;
                        oItemPullInfo.LastUpdatedBy = SessionHelper.UserID;
                        oItemPullInfo.ErrorList = new List<PullErrorInfo>();
                        oItemPullInfo = ValidateLotAndSerial(oItemPullInfo);

                        if (oItemPullInfo.ErrorList.Count == 0)
                        {
                            BinMasterDTO objBINDTO = new BinMasterDTO();
                            BinMasterDAL objBINDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                            ItemLocationDetailsDAL objItemLocationDetailsDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                            if (item.lstItemPullDetails != null && item.lstItemPullDetails.Count > 0)
                            {
                                List<ItemLocationDetailsDTO> lstItemLocationDetailsTmp = null;
                                double CurrentPullQuantity = 0;
                                foreach (ItemLocationLotSerialDTO objItemLocationLotSerialDTO in item.lstItemPullDetails)
                                {
                                    objItemLocationLotSerialDTO.CustomerOwnedQuantity = 0;
                                    objItemLocationLotSerialDTO.CustomerOwnedTobePulled = 0;
                                    objItemLocationLotSerialDTO.ConsignedQuantity = 0;
                                    objItemLocationLotSerialDTO.ConsignedTobePulled = 0;

                                    string LotSerial = ((objItemLocationLotSerialDTO.LotNumber != null && objItemLocationLotSerialDTO.LotNumber.Trim() != "") ? objItemLocationLotSerialDTO.LotNumber.Trim()
                                                            : ((objItemLocationLotSerialDTO.SerialNumber != null && objItemLocationLotSerialDTO.SerialNumber.Trim() != "") ? objItemLocationLotSerialDTO.SerialNumber.Trim() : ""));

                                    if (objItemmasterDTO.SerialNumberTracking == true || objItemmasterDTO.LotNumberTracking == true || objItemmasterDTO.DateCodeTracking == true)
                                    {
                                        if (objItemmasterDTO.DateCodeTracking && !objItemmasterDTO.SerialNumberTracking && !objItemmasterDTO.LotNumberTracking)
                                            lstItemLocationDetailsTmp = objItemLocationDetailsDAL.GetRecordsByBinNumberAndDateCode(item.ItemGUID, objItemLocationLotSerialDTO.BinNumber, objItemLocationLotSerialDTO.ExpirationDate.GetValueOrDefault(DateTime.MinValue), SessionHelper.RoomID, SessionHelper.CompanyID);
                                        else
                                            lstItemLocationDetailsTmp = objItemLocationDetailsDAL.GetRecordsByBinNumberAndLotSerial(item.ItemGUID, objItemLocationLotSerialDTO.BinNumber, LotSerial, SessionHelper.RoomID, SessionHelper.CompanyID);
                                    }
                                    else if (lstItemLocationDetailsTmp == null || lstItemLocationDetailsTmp.Count <= 0)
                                        lstItemLocationDetailsTmp = objItemLocationDetailsDAL.GetRecordsByBinNumberAndLotSerial(item.ItemGUID, objItemLocationLotSerialDTO.BinNumber, LotSerial, SessionHelper.RoomID, SessionHelper.CompanyID);

                                    if (lstItemLocationDetailsTmp != null && lstItemLocationDetailsTmp.Count > 0)
                                    {
                                        double PullQty = objItemLocationLotSerialDTO.TotalTobePulled;

                                        foreach (ItemLocationDetailsDTO objItemLocationDetailsDTO in lstItemLocationDetailsTmp)
                                        {
                                            if (objItemLocationDetailsDTO.CustomerOwnedQuantity != null && objItemLocationDetailsDTO.CustomerOwnedQuantity != 0)
                                            {
                                                objItemLocationLotSerialDTO.CustomerOwnedQuantity = (objItemLocationLotSerialDTO.CustomerOwnedQuantity ?? 0) + (objItemLocationDetailsDTO.CustomerOwnedQuantity ?? 0);
                                                if (objItemLocationDetailsDTO.CustomerOwnedQuantity > 0 && PullQty > 0)
                                                {
                                                    CurrentPullQuantity = (objItemLocationDetailsDTO.CustomerOwnedQuantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.CustomerOwnedQuantity);
                                                    objItemLocationLotSerialDTO.CustomerOwnedTobePulled = objItemLocationLotSerialDTO.CustomerOwnedTobePulled + CurrentPullQuantity;
                                                    PullQty = PullQty - (double)objItemLocationDetailsDTO.CustomerOwnedQuantity;
                                                    objItemLocationDetailsDTO.CustomerOwnedQuantity = objItemLocationDetailsDTO.CustomerOwnedQuantity - CurrentPullQuantity;
                                                }
                                            }
                                        }

                                        foreach (ItemLocationDetailsDTO objItemLocationDetailsDTO in lstItemLocationDetailsTmp)
                                        {
                                            if (objItemLocationDetailsDTO.ConsignedQuantity != null && objItemLocationDetailsDTO.ConsignedQuantity != 0)
                                            {
                                                objItemLocationLotSerialDTO.ConsignedQuantity = (objItemLocationLotSerialDTO.ConsignedQuantity ?? 0) + (objItemLocationDetailsDTO.ConsignedQuantity ?? 0);
                                                if (objItemLocationDetailsDTO.ConsignedQuantity > 0 && PullQty > 0)
                                                {
                                                    CurrentPullQuantity = (objItemLocationDetailsDTO.ConsignedQuantity > PullQty ? PullQty : (double)objItemLocationDetailsDTO.ConsignedQuantity);
                                                    objItemLocationLotSerialDTO.ConsignedTobePulled = objItemLocationLotSerialDTO.ConsignedTobePulled + CurrentPullQuantity;
                                                    PullQty = PullQty - (double)objItemLocationDetailsDTO.ConsignedQuantity;
                                                    objItemLocationDetailsDTO.ConsignedQuantity = objItemLocationDetailsDTO.ConsignedQuantity - CurrentPullQuantity;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            oItemPullInfo.EnterpriseId = SessionHelper.EnterPriceID;
                            oItemPullInfo = objMSDetailDAL.AddItemQuantityToStage(oItemPullInfo, SessionUserId, enterpriseId);

                        }
                        oReturn.Add(oItemPullInfo);
                    }
                }
            }
            return Json(oReturn);


        }

        private ItemStagePullInfo ValidateLotAndSerial(ItemStagePullInfo objItemPullInfo)
        {
            ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            ItemMasterDTO objItem = objItemMasterDAL.GetItemWithoutJoins(null, objItemPullInfo.ItemGUID);
            double? _PullCost = null;
            double TotalPulled = 0, Diff = 0, ConsignedTaken = 0, CustownedTaken = 0, TotalCustOwned = 0, TotalConsigned = 0;
            double PullCost = 0;
            double AvailQty = 0;
            ItemLocationQTYDTO oLocQty = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName).GetRecordByBinItem(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
            if (oLocQty != null)
                AvailQty = (oLocQty.CustomerOwnedQuantity ?? 0) + (oLocQty.ConsignedQuantity ?? 0);

            PullTransactionDAL objPullMasterDAL = new PullTransactionDAL(SessionHelper.EnterPriseDBName);
            List<ItemLocationLotSerialDTO> lstAvailableQty = new List<ItemLocationLotSerialDTO>();
            if (AvailQty >= objItemPullInfo.PullQuantity)
            {
                if (!objItem.LotNumberTracking && !objItem.SerialNumberTracking && !objItem.DateCodeTracking)
                {
                    lstAvailableQty = objPullMasterDAL.GetItemLocationsLotSerials(objItemPullInfo.ItemGUID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId, objItemPullInfo.BinID, objItemPullInfo.PullQuantity, true);
                    lstAvailableQty.ForEach(il =>
                    {
                        il.PullQuantity = objItemPullInfo.PullQuantity;
                        ConsignedTaken = il.ConsignedQuantity ?? 0;
                        CustownedTaken = il.CustomerOwnedQuantity ?? 0;
                        TotalPulled += (ConsignedTaken + CustownedTaken);
                        PullCost += (TotalPulled * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                        Diff = (objItemPullInfo.PullQuantity - TotalPulled);
                        if (Diff < 0)
                        {
                            TotalPulled -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0));
                            PullCost -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0));
                            if (il.IsConsignedLotSerial)
                            {
                                ConsignedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                            }
                            else
                            {
                                CustownedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                            }
                            TotalPulled += (ConsignedTaken + CustownedTaken);
                            PullCost += (ConsignedTaken + CustownedTaken) * ((_PullCost != null ? _PullCost : il.Cost).GetValueOrDefault(0));

                        }
                        TotalCustOwned += CustownedTaken;
                        TotalConsigned += ConsignedTaken;
                        il.CustomerOwnedTobePulled = CustownedTaken;
                        il.ConsignedTobePulled = ConsignedTaken;
                        il.TotalTobePulled = CustownedTaken + ConsignedTaken;
                        il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                    });

                    objItemPullInfo.ItemCost = PullCost;
                    objItemPullInfo.TotalConsignedTobePulled = TotalConsigned;
                    objItemPullInfo.TotalCustomerOwnedTobePulled = TotalCustOwned;
                    objItemPullInfo.lstItemPullDetails = lstAvailableQty;

                }
                else
                {
                    if (objItem.LotNumberTracking)
                    {
                        lstAvailableQty = objItemPullInfo.lstItemPullDetails;
                        lstAvailableQty.ForEach(t =>
                        {
                            List<ItemLocationDetailsDTO> objItemLocationDetail = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetItemsLocationsSerLotQty(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, t.LotNumber, string.Empty, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                            if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                            {
                                var lstLotQty = (from il in objItemLocationDetail
                                                 group il by new { il.LotNumber } into grpms
                                                 select new
                                                 {
                                                     CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                     ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                     LotNumber = grpms.Key.LotNumber,
                                                 }).FirstOrDefault();

                                if (t.PullQuantity > (lstLotQty.CustomerOwnedQuantity) + ((lstLotQty.ConsignedQuantity)))
                                {
                                    t.ValidationMessage = ResPullMaster.msgInvalidQuantityLot;
                                }
                                else
                                {
                                    if ((lstLotQty.CustomerOwnedQuantity) > 0)
                                        t.CustomerOwnedQuantity = t.PullQuantity;
                                    else
                                        t.ConsignedQuantity = t.PullQuantity;
                                }
                            }
                            else
                            {
                                t.ValidationMessage = ResPullMaster.msgInvalidLot;
                            }
                        });

                        if (lstAvailableQty.Any(t => (t.ValidationMessage ?? string.Empty) != string.Empty))
                        {
                            objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "6", ErrorMessage = objItem.ItemNumber + ": " + ResPullMaster.msgInvalidQuantityLot });
                        }
                        else
                        {
                            lstAvailableQty.ForEach(il =>
                            {
                                ConsignedTaken = il.ConsignedQuantity ?? 0;
                                CustownedTaken = il.CustomerOwnedQuantity ?? 0;

                                TotalPulled += (ConsignedTaken + CustownedTaken);
                                PullCost += (TotalPulled * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                                Diff = (objItemPullInfo.PullQuantity - TotalPulled);
                                if (Diff < 0)
                                {
                                    TotalPulled -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0));
                                    PullCost -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0));
                                    if (il.IsConsignedLotSerial)
                                    {
                                        ConsignedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                                    }
                                    else
                                    {
                                        CustownedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                                    }
                                    TotalPulled += (ConsignedTaken + CustownedTaken);
                                    PullCost += ((ConsignedTaken + CustownedTaken) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));

                                }
                                TotalCustOwned += CustownedTaken;
                                TotalConsigned += ConsignedTaken;
                                il.CustomerOwnedTobePulled = CustownedTaken;
                                il.ConsignedTobePulled = ConsignedTaken;
                                il.TotalTobePulled = CustownedTaken + ConsignedTaken;
                                il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));

                            });
                            objItemPullInfo.ItemCost = PullCost;
                            objItemPullInfo.TotalConsignedTobePulled = TotalConsigned;
                            objItemPullInfo.TotalCustomerOwnedTobePulled = TotalCustOwned;
                            objItemPullInfo.lstItemPullDetails = lstAvailableQty;

                        }
                    }
                    else if (objItem.SerialNumberTracking)
                    {
                        lstAvailableQty = objItemPullInfo.lstItemPullDetails;
                        lstAvailableQty.ForEach(t =>
                        {
                            List<ItemLocationDetailsDTO> objItemLocationDetail = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetItemsLocationsSerLotQty(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, string.Empty, t.SerialNumber, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                            if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                            {
                                var lstLotQty = (from il in objItemLocationDetail
                                                 group il by new { il.SerialNumber } into grpms
                                                 select new
                                                 {
                                                     CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                     ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                     LotNumber = grpms.Key.SerialNumber,
                                                 }).FirstOrDefault();

                                if (t.PullQuantity > (lstLotQty.CustomerOwnedQuantity) + ((lstLotQty.ConsignedQuantity)))
                                {
                                    t.ValidationMessage = ResPullMaster.msgInvalidQuantityLot;
                                }
                                else
                                {
                                    if ((lstLotQty.CustomerOwnedQuantity) > 0)
                                        t.CustomerOwnedQuantity = t.PullQuantity;
                                    else
                                        t.ConsignedQuantity = t.PullQuantity;
                                }
                            }
                            else
                            {
                                t.ValidationMessage = ResPullMaster.msgInvalidLot;
                            }
                        });

                        if (lstAvailableQty.Any(t => (t.ValidationMessage ?? string.Empty) != string.Empty))
                        {
                            objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "6", ErrorMessage = objItem.ItemNumber + ": " + ResPullMaster.msgInvalidQuantityLot });
                        }
                        else
                        {
                            lstAvailableQty.ForEach(il =>
                            {

                                ConsignedTaken = il.ConsignedQuantity ?? 0;
                                CustownedTaken = il.CustomerOwnedQuantity ?? 0;

                                TotalPulled += (ConsignedTaken + CustownedTaken);
                                PullCost += (TotalPulled * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                                Diff = (objItemPullInfo.PullQuantity - TotalPulled);
                                if (Diff < 0)
                                {
                                    TotalPulled -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0));
                                    PullCost -= (((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                                    if (il.IsConsignedLotSerial)
                                    {
                                        ConsignedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                                    }
                                    else
                                    {
                                        CustownedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                                    }
                                    TotalPulled += (ConsignedTaken + CustownedTaken);
                                    PullCost += ((ConsignedTaken + CustownedTaken) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));

                                }
                                TotalCustOwned += CustownedTaken;
                                TotalConsigned += ConsignedTaken;
                                il.CustomerOwnedTobePulled = CustownedTaken;
                                il.ConsignedTobePulled = ConsignedTaken;
                                il.TotalTobePulled = CustownedTaken + ConsignedTaken;
                                il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));

                            });
                            objItemPullInfo.ItemCost = PullCost;
                            objItemPullInfo.TotalConsignedTobePulled = TotalConsigned;
                            objItemPullInfo.TotalCustomerOwnedTobePulled = TotalCustOwned;
                            objItemPullInfo.lstItemPullDetails = lstAvailableQty;

                        }

                    }
                    else if (objItem.DateCodeTracking)
                    {
                        lstAvailableQty = objItemPullInfo.lstItemPullDetails;
                        lstAvailableQty.ForEach(t =>
                        {
                            DateTime ExpirationDateUTC = Convert.ToDateTime(DateTime.ParseExact(t.Expiration, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult));
                            List<ItemLocationDetailsDTO> objItemLocationDetail = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetItemsLocationsDateCodeQty(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId, ExpirationDateUTC);
                            if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                            {
                                var lstLotQty = (from il in objItemLocationDetail
                                                 group il by new { il.ExpirationDate } into grpms
                                                 select new
                                                 {
                                                     CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                     ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                     ExpirationDate = grpms.Key.ExpirationDate,
                                                 }).FirstOrDefault();

                                if (t.PullQuantity > (lstLotQty.CustomerOwnedQuantity) + ((lstLotQty.ConsignedQuantity)))
                                {
                                    t.ValidationMessage = ResPullMaster.msgInvalidQuantityLot;
                                }
                                else
                                {
                                    if ((lstLotQty.CustomerOwnedQuantity) > 0)
                                        t.CustomerOwnedQuantity = t.PullQuantity;
                                    else
                                        t.ConsignedQuantity = t.PullQuantity;
                                    t.ExpirationDate = lstLotQty.ExpirationDate;
                                }
                            }
                            else
                            {
                                t.ValidationMessage = ResPullMaster.msgInvalidLot;
                            }
                        });

                        if (lstAvailableQty.Any(t => (t.ValidationMessage ?? string.Empty) != string.Empty))
                        {
                            objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "6", ErrorMessage = objItem.ItemNumber + ": " + ResPullMaster.msgInvalidQuantityLot });
                        }
                        else
                        {
                            lstAvailableQty.ForEach(il =>
                            {

                                ConsignedTaken = il.ConsignedQuantity ?? 0;
                                CustownedTaken = il.CustomerOwnedQuantity ?? 0;

                                TotalPulled += (ConsignedTaken + CustownedTaken);
                                PullCost += (TotalPulled * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                                Diff = (objItemPullInfo.PullQuantity - TotalPulled);
                                if (Diff < 0)
                                {
                                    TotalPulled -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0));
                                    PullCost -= (((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));
                                    if (il.IsConsignedLotSerial)
                                    {
                                        ConsignedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                                    }
                                    else
                                    {
                                        CustownedTaken = (objItemPullInfo.PullQuantity - TotalPulled);
                                    }
                                    TotalPulled += (ConsignedTaken + CustownedTaken);
                                    PullCost += ((ConsignedTaken + CustownedTaken) * (_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));

                                }
                                TotalCustOwned += CustownedTaken;
                                TotalConsigned += ConsignedTaken;
                                il.CustomerOwnedTobePulled = CustownedTaken;
                                il.ConsignedTobePulled = ConsignedTaken;
                                il.TotalTobePulled = CustownedTaken + ConsignedTaken;
                                il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((_PullCost != null ? (_PullCost ?? 0) : (il.Cost ?? 0)));

                            });
                            objItemPullInfo.ItemCost = PullCost;
                            objItemPullInfo.TotalConsignedTobePulled = TotalConsigned;
                            objItemPullInfo.TotalCustomerOwnedTobePulled = TotalCustOwned;
                            objItemPullInfo.lstItemPullDetails = lstAvailableQty;

                        }

                    }
                }
            }
            else
            {
                objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "1", ErrorMessage = objItem.ItemNumber + ": " + ResPullMaster.msgQuantityNotAvailable });
            }
            return objItemPullInfo;
        }


        public ActionResult MaterialStagingPullDetail_MS(string ItemGUID, string MSDTLGUID)
        {
            ItemLocationDetailsDAL objItemLocationDetailsController = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            BinMasterDAL objBinMasterController = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            List<ItemLocationDetailsDTO> lstBins = new List<ItemLocationDetailsDTO>();
            ViewBag.Itembins = lstBins;
            ViewBag.ItemGUID = ItemGUID;// ?? Guid.Empty;
            ViewBag.MaterialStagingdtlGUID = MSDTLGUID;
            ItemMasterDTO objItem = null;
            Guid ItemGUID1 = Guid.Empty;

            if (Guid.TryParse(ItemGUID, out ItemGUID1))
            {
                objItem = (new ItemMasterDAL(SessionHelper.EnterPriseDBName)).GetItemWithoutJoins(null, ItemGUID1);
            }

            if (objItem != null)
            {
                ViewBag.SerialNumberTracking = objItem.SerialNumberTracking;
                ViewBag.LotNumberTracking = objItem.LotNumberTracking;
                ViewBag.DateCodeTracking = objItem.DateCodeTracking;
            }
            else
            {
                ViewBag.SerialNumberTracking = false;
                ViewBag.LotNumberTracking = false;
                ViewBag.DateCodeTracking = false;
            }

            return PartialView("_MSPullDetail");
        }


        public ActionResult MaterialStagingPullDetailListAjax(JQueryDataTableParamModel param)
        {
            string ItemGUID = Request["ItemGUID"].ToString();
            Guid? MaterialStagingdtlGUID = null;

            if (!string.IsNullOrEmpty(Request["MaterialStagingdtlGUID"]) && Request["MaterialStagingdtlGUID"].Trim().Length > 0)
                MaterialStagingdtlGUID = Guid.Parse(Request["MaterialStagingdtlGUID"]);

            ///////////// requried when paging needs in this method /////////////////
            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
            sortColumnName = Request["SortingField"].ToString();

            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());

            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined") || sortColumnName == "ShippingMethod")
                    sortColumnName = "ID";
            }
            else
                sortColumnName = "ID";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            ///////////// requried when paging needs in this method /////////////////

            ViewBag.ItemGUID = ItemGUID;
            ViewBag.MaterialStagingdtlGUID = MaterialStagingdtlGUID;
            ItemMasterDAL objItemAPI = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            Guid ItemGUID1 = Guid.Empty;
            Guid.TryParse(ItemGUID, out ItemGUID1);
            var Objitem = objItemAPI.GetItemWithoutJoins(null, ItemGUID1);
            ViewBag.ItemID = Objitem.ID;
            MaterialStagingPullDetailDAL objAPI = new MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName);
            int TotalRecordCount = 0;
            var objModel = objAPI.GetMsPullDetailsByMsDetailsId_Page(MaterialStagingdtlGUID ?? Guid.Empty, param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, Guid.Parse(ItemGUID));

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = objModel
            },
            JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region Material Staging History
        //private object GetUDFDataPageWise(string PageName)
        //{
        //    UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
        //    IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetAllRecords(SessionHelper.CompanyID, PageName, SessionHelper.RoomID);

        //    var result = from c in DataFromDB
        //                 select new UDFDTO
        //                 {
        //                     ID = c.ID,
        //                     CompanyID = c.CompanyID,
        //                     Room = c.Room,
        //                     UDFTableName = c.UDFTableName,
        //                     UDFColumnName = c.UDFColumnName,
        //                     UDFDefaultValue = c.UDFDefaultValue,
        //                     UDFOptionsCSV = c.UDFOptionsCSV,
        //                     UDFControlType = c.UDFControlType,
        //                     UDFIsRequired = c.UDFIsRequired,
        //                     UDFIsSearchable = c.UDFIsRequired = c.UDFIsRequired,
        //                     Created = c.Created,
        //                     Updated = c.Updated,
        //                     UpdatedByName = c.UpdatedByName,
        //                     CreatedByName = c.CreatedByName,
        //                     IsDeleted = c.IsDeleted,
        //                 };
        //    return result;
        //}

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult MaterialStagingHistoryView(Int64 ID)
        {
            MaterialStagingDAL obj = new MaterialStagingDAL(SessionHelper.EnterPriseDBName);
            MaterialStagingDTO objDTO = obj.GetHistoryRecord(ID);

            if (objDTO != null)
            {
                objDTO.CreatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                objDTO.UpdatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
            }

            UDFController objUDFDAL = new UDFController();
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("ProjectMaster");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;

            int totalcount = 0;
            IEnumerable<MaterialStagingDetailDTO> lstItems = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName).GetPagedStagingLineItems(0, 2000, out totalcount, string.Empty, "StagingId DESC", SessionHelper.RoomID, SessionHelper.CompanyID, false, false, objDTO.GUID, SessionHelper.UserSupplierIds);
            ViewBag.LineItemCount = totalcount;

            return PartialView("_MaterialStagingDetails_History", objDTO);
        }

        /// <summary>
        /// LoadOrderLineItemsHistory
        /// </summary>
        /// <param name="historyID"></param>
        /// <returns></returns>
        public ActionResult LoadMaterialStagingLineItemsHistory(Int64 HistoryID)
        {
            MaterialStagingDTO objDTO = null;
            objDTO = new MaterialStagingDAL(SessionHelper.EnterPriseDBName).GetHistoryRecord(HistoryID);
            if (objDTO != null)
                objDTO.MaterialStagingItemList = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName).GetHistoryRecordbyMaterialStagingID(objDTO.GUID).ToList();

            objDTO.MaterialStagingItemList.ForEach(m => m.StagingBinName = Convert.ToString(m.StagingBinName) == "[|EmptyStagingBin|]" ? string.Empty : m.StagingBinName);

            return PartialView("StagingLineItems_History", objDTO);
        }

        #endregion
    }
}
