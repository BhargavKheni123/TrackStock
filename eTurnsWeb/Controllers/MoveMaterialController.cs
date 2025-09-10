using eTurns.DAL;
using eTurns.DTO;
using eTurnsWeb.Helper;
using eTurnsWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace eTurnsWeb.Controllers
{
    public partial class InventoryController : eTurnsControllerBase
    {
        Int64 RoomID = SessionHelper.RoomID;
        Int64 CompanyID = SessionHelper.CompanyID;
        string QtyFromate = "N2";

        public ActionResult MoveMaterial()
        {
            ViewBag.MoveTypeList = GetMoveTypeList(MoveDialogOpenFrom.FromMove);
            return View();
        }

        /// <summary>
        /// LoadItemMasterModel
        /// </summary>
        /// <param name="ParentId"></param>
        /// <returns></returns>
        public ActionResult LoadItemMaster(int MoveType)
        {
            Session["ItemMasterList"] = null;
            ItemModelPerameter obj = new ItemModelPerameter()
            {
                AjaxURLAddItemToSession = "~/Inventory/ShowLocations/",
                AjaxURLAddMultipleItemToSession = "~/Inventory/ShowLocations/",
                AjaxURLToFillItemGrid = "~/Inventory/LoadItems/",
                CallingFromPageName = "MOVEMTR",
                ModelHeader = "",
                PerentID = MoveType.ToString(),
            };

            var regionalSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
            var regionInfo = regionalSettingDAL.GetRegionSettingsById(RoomID, CompanyID, -1);
            ViewBag.WeightDP = regionInfo.WeightDecimalPoints;

            return PartialView("ItemMasterModel", obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult OpenPopupForMoveMaterial(MoveMaterialDTO MoveMTRDTO)
        {

            //if (SessionHelper.CompanyConfig != null && SessionHelper.CompanyConfig.QuantityDecimalPoints.HasValue)
            //{
            //    QtyFromate = "N" + SessionHelper.CompanyConfig.QuantityDecimalPoints.GetValueOrDefault(0);
            //}

            if (!string.IsNullOrEmpty(SessionHelper.QuantityFormat))
            {
                QtyFromate = "N" + SessionHelper.QuantityFormat;
            }

            ItemMasterDTO itmDTO = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, MoveMTRDTO.ItemGUID);
            MoveMTRDTO.ItemNumber = itmDTO.ItemNumber;
            MoveMTRDTO.OnHandQuantity = string.Format(SessionHelper.QuantityFormat, itmDTO.OnHandQuantity.GetValueOrDefault(0));// itmDTO.OnHandQuantity.GetValueOrDefault(0).ToString(QtyFromate);
            MoveMTRDTO.StageQuantity = string.Format(SessionHelper.QuantityFormat, itmDTO.StagedQuantity.GetValueOrDefault(0)); //itmDTO.StagedQuantity.GetValueOrDefault(0).ToString(QtyFromate);
            if (MoveMTRDTO.SourceBinID > 0)
            {
                MoveMTRDTO.SourceLocation = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(MoveMTRDTO.SourceBinID, RoomID, CompanyID).BinNumber;
                //MoveMTRDTO.SourceLocation = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocation( RoomID, CompanyID, false, false,Guid.Empty, MoveMTRDTO.SourceBinID,null,null).FirstOrDefault().BinNumber;

                if (MoveMTRDTO.SourceStagingHeaderGuid.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {
                    MaterialStagingDetailDAL MSDetailDAL = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName);
                    //List<MaterialStagingDetailDTO> msDetailDTOList = MSDetailDAL.GetStagingLocationByItem(MoveMTRDTO.ItemGUID, RoomID, CompanyID).Where(x => x.MaterialStagingGUID.GetValueOrDefault(Guid.Empty) == MoveMTRDTO.SourceStagingHeaderGuid.GetValueOrDefault(Guid.Empty) && x.StagingBinID == MoveMTRDTO.SourceBinID && x.Quantity > 0).ToList();
                    List<MaterialStagingDetailDTO> msDetailDTOList = MSDetailDAL.GetStagingLocationByItem(MoveMTRDTO.ItemGUID, RoomID, CompanyID, MoveMTRDTO.SourceBinID, Convert.ToString(MoveMTRDTO.SourceStagingHeaderGuid.GetValueOrDefault(Guid.Empty)), true).ToList();
                    if (msDetailDTOList != null && msDetailDTOList.Count() > 0)
                    {
                        MoveMTRDTO.MoveQuanity = msDetailDTOList.Sum(x => x.Quantity);
                        MoveMTRDTO.SourceStagingHeader = msDetailDTOList[0].MaterialStagingName;
                    }
                    MSDetailDAL = null;
                    msDetailDTOList = null;
                }
                else
                {
                    ItemLocationQTYDAL itmLocQtyDAL = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName);
                    //ItemLocationQTYDTO itmLocQty = itmLocQtyDAL.GetRecordByItem(MoveMTRDTO.ItemGUID, RoomID, CompanyID).Where(x => x.BinID == MoveMTRDTO.SourceBinID).FirstOrDefault();
                    ItemLocationQTYDTO itmLocQty = itmLocQtyDAL.GetRecordByItemBIn(MoveMTRDTO.ItemGUID, RoomID, CompanyID, MoveMTRDTO.SourceBinID, null).FirstOrDefault();
                    if (itmLocQty != null)
                    {
                        MoveMTRDTO.MoveQuanity = itmLocQty.Quantity;
                    }
                    itmLocQtyDAL = null;
                    itmLocQty = null;
                }
            }
            ViewBag.ItemGUID = MoveMTRDTO.ItemGUID.ToString();
            MoveMTRDTO.IsOnlyFromItemUI = true;
            MoveMTRDTO.MoveTypeList = GetMoveTypeList(MoveMTRDTO.OpenFrom);
            return PartialView("_MoveMaterialLocationPopup", MoveMTRDTO);
        }
        public ActionResult OpenPopupForMoveMaterialByRoomAndCompany(MoveMaterialDTO MoveMTRDTO)
        {

            //if (SessionHelper.CompanyConfig != null && SessionHelper.CompanyConfig.QuantityDecimalPoints.HasValue)
            //{
            //    QtyFromate = "N" + SessionHelper.CompanyConfig.QuantityDecimalPoints.GetValueOrDefault(0);
            //}

            if (!string.IsNullOrEmpty(SessionHelper.QuantityFormat))
            {
                QtyFromate = "N" + SessionHelper.QuantityFormat;
            }

            ItemMasterDTO itmDTO = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, MoveMTRDTO.ItemGUID);
            MoveMTRDTO.ItemNumber = itmDTO.ItemNumber;
            MoveMTRDTO.OnHandQuantity = string.Format(SessionHelper.QuantityFormat, itmDTO.OnHandQuantity.GetValueOrDefault(0));// itmDTO.OnHandQuantity.GetValueOrDefault(0).ToString(QtyFromate);
            MoveMTRDTO.StageQuantity = string.Format(SessionHelper.QuantityFormat, itmDTO.StagedQuantity.GetValueOrDefault(0)); //itmDTO.StagedQuantity.GetValueOrDefault(0).ToString(QtyFromate);
            if (MoveMTRDTO.SourceBinID > 0)
            {
                MoveMTRDTO.SourceLocation = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(MoveMTRDTO.SourceBinID, itmDTO.Room ?? 0, itmDTO.CompanyID ?? 0).BinNumber;
                //MoveMTRDTO.SourceLocation = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocation( RoomID, CompanyID, false, false,Guid.Empty, MoveMTRDTO.SourceBinID,null,null).FirstOrDefault().BinNumber;

                if (MoveMTRDTO.SourceStagingHeaderGuid.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {
                    MaterialStagingDetailDAL MSDetailDAL = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName);
                    //List<MaterialStagingDetailDTO> msDetailDTOList = MSDetailDAL.GetStagingLocationByItem(MoveMTRDTO.ItemGUID, RoomID, CompanyID).Where(x => x.MaterialStagingGUID.GetValueOrDefault(Guid.Empty) == MoveMTRDTO.SourceStagingHeaderGuid.GetValueOrDefault(Guid.Empty) && x.StagingBinID == MoveMTRDTO.SourceBinID && x.Quantity > 0).ToList();
                    List<MaterialStagingDetailDTO> msDetailDTOList = MSDetailDAL.GetStagingLocationByItem(MoveMTRDTO.ItemGUID, itmDTO.Room ?? 0, itmDTO.CompanyID ?? 0, MoveMTRDTO.SourceBinID, Convert.ToString(MoveMTRDTO.SourceStagingHeaderGuid.GetValueOrDefault(Guid.Empty)), true).ToList();
                    if (msDetailDTOList != null && msDetailDTOList.Count() > 0)
                    {
                        MoveMTRDTO.MoveQuanity = msDetailDTOList.Sum(x => x.Quantity);
                        MoveMTRDTO.SourceStagingHeader = msDetailDTOList[0].MaterialStagingName;
                    }
                    MSDetailDAL = null;
                    msDetailDTOList = null;
                }
                else
                {
                    ItemLocationQTYDAL itmLocQtyDAL = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName);
                    //ItemLocationQTYDTO itmLocQty = itmLocQtyDAL.GetRecordByItem(MoveMTRDTO.ItemGUID, RoomID, CompanyID).Where(x => x.BinID == MoveMTRDTO.SourceBinID).FirstOrDefault();
                    ItemLocationQTYDTO itmLocQty = itmLocQtyDAL.GetRecordByItemBIn(MoveMTRDTO.ItemGUID, itmDTO.Room ?? 0, itmDTO.CompanyID ?? 0, MoveMTRDTO.SourceBinID, null).FirstOrDefault();
                    if (itmLocQty != null)
                    {
                        MoveMTRDTO.MoveQuanity = itmLocQty.Quantity;
                    }
                    itmLocQtyDAL = null;
                    itmLocQty = null;
                }
            }
            ViewBag.ItemGUID = MoveMTRDTO.ItemGUID.ToString();
            MoveMTRDTO.IsOnlyFromItemUI = true;
            MoveMTRDTO.MoveTypeList = GetMoveTypeList(MoveMTRDTO.OpenFrom);
            return PartialView("_MoveMaterialLocationPopupByRoomAndCompany", MoveMTRDTO);
        }
        public ActionResult OpenPopupForMoveMaterialLotSerial(MoveMaterialDTO MoveMTRDTO)
        {
            if (!string.IsNullOrEmpty(SessionHelper.QuantityFormat))
            {
                QtyFromate = "N" + SessionHelper.QuantityFormat;
            }

            ItemMasterDTO itmDTO = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, MoveMTRDTO.ItemGUID);
            MoveMTRDTO.ItemNumber = itmDTO.ItemNumber;
            if(itmDTO != null)
            {
                MoveMTRDTO.DateCodeTracking = itmDTO.DateCodeTracking;
            }
            MoveMTRDTO.OnHandQuantity = string.Format(SessionHelper.QuantityFormat, itmDTO.OnHandQuantity.GetValueOrDefault(0));// itmDTO.OnHandQuantity.GetValueOrDefault(0).ToString(QtyFromate);
            MoveMTRDTO.StageQuantity = string.Format(SessionHelper.QuantityFormat, itmDTO.StagedQuantity.GetValueOrDefault(0)); //itmDTO.StagedQuantity.GetValueOrDefault(0).ToString(QtyFromate);
            if (MoveMTRDTO.SourceBinID > 0)
            {
                MoveMTRDTO.SourceLocation = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(MoveMTRDTO.SourceBinID, RoomID, CompanyID).BinNumber;
                //MoveMTRDTO.SourceLocation = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocation(RoomID, CompanyID, false, false,Guid.Empty, MoveMTRDTO.SourceBinID, null,null).FirstOrDefault().BinNumber;

                if (MoveMTRDTO.SourceStagingHeaderGuid.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {
                    MaterialStagingDetailDAL MSDetailDAL = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName);
                    //List<MaterialStagingDetailDTO> msDetailDTOList = MSDetailDAL.GetStagingLocationByItem(MoveMTRDTO.ItemGUID, RoomID, CompanyID).Where(x => x.MaterialStagingGUID.GetValueOrDefault(Guid.Empty) == MoveMTRDTO.SourceStagingHeaderGuid.GetValueOrDefault(Guid.Empty) && x.StagingBinID == MoveMTRDTO.SourceBinID && x.Quantity > 0).ToList();
                    List<MaterialStagingDetailDTO> msDetailDTOList = MSDetailDAL.GetStagingLocationByItem(MoveMTRDTO.ItemGUID, RoomID, CompanyID, MoveMTRDTO.SourceBinID, Convert.ToString(MoveMTRDTO.SourceStagingHeaderGuid.GetValueOrDefault(Guid.Empty)), true).ToList();
                    if (msDetailDTOList != null && msDetailDTOList.Count() > 0)
                    {
                        MoveMTRDTO.MoveQuanity = msDetailDTOList.Sum(x => x.Quantity);
                        MoveMTRDTO.SourceStagingHeader = msDetailDTOList[0].MaterialStagingName;
                    }
                    MSDetailDAL = null;
                    msDetailDTOList = null;
                }
                else
                {
                    ItemLocationQTYDAL itmLocQtyDAL = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName);
                    //ItemLocationQTYDTO itmLocQty = itmLocQtyDAL.GetRecordByItem(MoveMTRDTO.ItemGUID, RoomID, CompanyID).Where(x => x.BinID == MoveMTRDTO.SourceBinID).FirstOrDefault();
                    ItemLocationQTYDTO itmLocQty = itmLocQtyDAL.GetRecordByItemBIn(MoveMTRDTO.ItemGUID, RoomID, CompanyID, MoveMTRDTO.SourceBinID, null).FirstOrDefault();
                    if (itmLocQty != null)
                    {
                        MoveMTRDTO.MoveQuanity = itmLocQty.Quantity;
                    }
                    itmLocQtyDAL = null;
                    itmLocQty = null;
                }
            }
            ViewBag.ItemGUID = MoveMTRDTO.ItemGUID.ToString();
            MoveMTRDTO.IsOnlyFromItemUI = true;
            MoveMTRDTO.MoveTypeList = GetMoveTypeList(MoveMTRDTO.OpenFrom);
            return PartialView("_MoveMaterialLocationPopupLotSr", MoveMTRDTO);
        }

        public ActionResult OpenPopupForMoveMaterialLotSerialByRoomAndCompany(MoveMaterialDTO MoveMTRDTO)
        {
            if (!string.IsNullOrEmpty(SessionHelper.QuantityFormat))
            {
                QtyFromate = "N" + SessionHelper.QuantityFormat;
            }

            ItemMasterDTO itmDTO = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, MoveMTRDTO.ItemGUID);
            MoveMTRDTO.ItemNumber = itmDTO.ItemNumber;
            if (itmDTO != null)
            {
                MoveMTRDTO.DateCodeTracking = itmDTO.DateCodeTracking;
            }
            MoveMTRDTO.OnHandQuantity = string.Format(SessionHelper.QuantityFormat, itmDTO.OnHandQuantity.GetValueOrDefault(0));// itmDTO.OnHandQuantity.GetValueOrDefault(0).ToString(QtyFromate);
            MoveMTRDTO.StageQuantity = string.Format(SessionHelper.QuantityFormat, itmDTO.StagedQuantity.GetValueOrDefault(0)); //itmDTO.StagedQuantity.GetValueOrDefault(0).ToString(QtyFromate);
            if (MoveMTRDTO.SourceBinID > 0)
            {
                MoveMTRDTO.SourceLocation = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(MoveMTRDTO.SourceBinID, itmDTO.Room ?? 0, itmDTO.CompanyID ?? 0).BinNumber;
                //MoveMTRDTO.SourceLocation = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocation(RoomID, CompanyID, false, false,Guid.Empty, MoveMTRDTO.SourceBinID, null,null).FirstOrDefault().BinNumber;

                if (MoveMTRDTO.SourceStagingHeaderGuid.GetValueOrDefault(Guid.Empty) != Guid.Empty)
                {
                    MaterialStagingDetailDAL MSDetailDAL = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName);
                    //List<MaterialStagingDetailDTO> msDetailDTOList = MSDetailDAL.GetStagingLocationByItem(MoveMTRDTO.ItemGUID, RoomID, CompanyID).Where(x => x.MaterialStagingGUID.GetValueOrDefault(Guid.Empty) == MoveMTRDTO.SourceStagingHeaderGuid.GetValueOrDefault(Guid.Empty) && x.StagingBinID == MoveMTRDTO.SourceBinID && x.Quantity > 0).ToList();
                    List<MaterialStagingDetailDTO> msDetailDTOList = MSDetailDAL.GetStagingLocationByItem(MoveMTRDTO.ItemGUID, itmDTO.Room ?? 0, itmDTO.CompanyID ?? 0, MoveMTRDTO.SourceBinID, Convert.ToString(MoveMTRDTO.SourceStagingHeaderGuid.GetValueOrDefault(Guid.Empty)), true).ToList();
                    if (msDetailDTOList != null && msDetailDTOList.Count() > 0)
                    {
                        MoveMTRDTO.MoveQuanity = msDetailDTOList.Sum(x => x.Quantity);
                        MoveMTRDTO.SourceStagingHeader = msDetailDTOList[0].MaterialStagingName;
                    }
                    MSDetailDAL = null;
                    msDetailDTOList = null;
                }
                else
                {
                    ItemLocationQTYDAL itmLocQtyDAL = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName);
                    //ItemLocationQTYDTO itmLocQty = itmLocQtyDAL.GetRecordByItem(MoveMTRDTO.ItemGUID, RoomID, CompanyID).Where(x => x.BinID == MoveMTRDTO.SourceBinID).FirstOrDefault();
                    ItemLocationQTYDTO itmLocQty = itmLocQtyDAL.GetRecordByItemBIn(MoveMTRDTO.ItemGUID, itmDTO.Room ?? 0, itmDTO.CompanyID ?? 0, MoveMTRDTO.SourceBinID, null).FirstOrDefault();
                    if (itmLocQty != null)
                    {
                        MoveMTRDTO.MoveQuanity = itmLocQty.Quantity;
                    }
                    itmLocQtyDAL = null;
                    itmLocQty = null;
                }
            }
            ViewBag.ItemGUID = MoveMTRDTO.ItemGUID.ToString();
            MoveMTRDTO.IsOnlyFromItemUI = true;
            MoveMTRDTO.MoveTypeList = GetMoveTypeList(MoveMTRDTO.OpenFrom);
            return PartialView("_MoveMaterialLocationPopupLotSr", MoveMTRDTO);
        }

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult LoadItems(QuickListJQueryDataTableParamModel param)
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

            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "ItemNumber desc";
            }
            else
                sortColumnName = "ItemNumber desc";

            string searchQuery = string.Empty;

            MoveType mvType = (MoveType)Enum.Parse(typeof(MoveType), Request["MoveType"]);
            int TotalRecordCount = 0;
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetPagedItemsForMoveMaterial(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted,  RoomDateFormat, SessionHelper.UserSupplierIds, CurrentTimeZone, (int)mvType,true).ToList();
            return Json(new { sEcho = param.sEcho, iTotalRecords = TotalRecordCount, iTotalDisplayRecords = TotalRecordCount, aaData = DataFromDB }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MoveMaterialListAjax(QuickListJQueryDataTableParamModel param)
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

            //if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ItemNumber Asc";
            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "ItemNumber Asc";
            }
            else
                sortColumnName = "ItemNumber Asc";

            //bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            //bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            bool IsArchived = false;
            bool IsDeleted = false;

            string searchQuery = string.Empty;
            int TotalRecordCount = 0;
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = new MoveMaterialDAL(SessionHelper.EnterPriseDBName).GetPagedRecordsNew(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID,SessionHelper.UserSupplierIds, IsArchived, IsDeleted, RoomDateFormat, CurrentTimeZone).ToList();
            //if (DataFromDB != null)
            //{
            //    DataFromDB.ToList().ForEach(t =>
            //    {
            //        t.CreatedDate = CommonUtility.ConvertDateByTimeZone(t.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
            //        t.UpdatedDate = CommonUtility.ConvertDateByTimeZone(t.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
            //    });
            //}
            return Json(new { sEcho = param.sEcho, iTotalRecords = TotalRecordCount, iTotalDisplayRecords = TotalRecordCount, aaData = DataFromDB }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GetLocationByType
        /// </summary>
        /// <param name="NameStartWith"></param>
        /// <param name="itemGuid"></param>
        /// <param name="locType"></param>
        /// <param name="moveType"></param>
        /// <param name="StageHeaderGUID"></param>
        /// <returns></returns>
        public JsonResult GetLocationByType(string NameStartWith, Guid itemGuid, string locType, MoveType moveType, string StageHeaderGUID, bool? IsLoadMoreLocations = null)
        {
            List<DTOForAutoComplete> locations = new List<DTOForAutoComplete>();
            MaterialStagingDetailDAL MSDetailDAL = null;
            MaterialStagingDAL MSDAL = null;
            BinMasterDAL ItmLocLevelQtyDAL = null;
            IEnumerable<BinMasterDTO> ItmLocLevelQtyDTOList = null;
            IEnumerable<MaterialStagingDTO> MSList = null;
            ItemLocationQTYDAL itmLocQtyDAL = null;
            IEnumerable<ItemLocationQTYDTO> itmLocQtyList = null;
            IEnumerable<MaterialStagingDetailDTO> msDetailDTOList = null;

            //if (SessionHelper.CompanyConfig != null && SessionHelper.CompanyConfig.QuantityDecimalPoints.HasValue)
            //{
            //    QtyFromate = "N" + SessionHelper.CompanyConfig.QuantityDecimalPoints.GetValueOrDefault(0);
            //}
            if (!string.IsNullOrEmpty(SessionHelper.NumberDecimalDigits))
            {
                QtyFromate = "N" + SessionHelper.NumberDecimalDigits;
            }
            try
            {
                if (locType == "SL")
                {
                    if (moveType != MoveType.StagToInv && moveType != MoveType.StagToStag)
                    {
                        itmLocQtyDAL = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName);
                        //itmLocQtyList = itmLocQtyDAL.GetRecordByItem(itemGuid, RoomID, CompanyID).Where(x => x.Quantity > 0);
                        itmLocQtyList = itmLocQtyDAL.GetRecordByItemBIn(itemGuid, RoomID, CompanyID, null, true);
                        foreach (var item in itmLocQtyList)
                        {
                            BinMasterDTO objBin = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(item.BinID, RoomID, CompanyID);
                            //BinMasterDTO objBin = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocation( RoomID, CompanyID, false, false,Guid.Empty, item.BinID,null,null).FirstOrDefault();
                            DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                            objAutoDTO.GUID = objBin.GUID;
                            objAutoDTO.ID = objBin.ID;
                            objAutoDTO.Key = objBin.BinNumber != "[|EmptyStagingBin|]" ? objBin.BinNumber + " (" + item.Quantity + ")" : string.Empty;
                            objAutoDTO.Value = objBin.BinNumber;
                            objAutoDTO.Quantity = item.Quantity;
                            locations.Add(objAutoDTO);
                        }
                    }
                    else
                    {
                        MSDetailDAL = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName);
                        //msDetailDTOList = MSDetailDAL.GetStagingLocationByItem(itemGuid, RoomID, CompanyID).Where(x => x.Quantity > 0);
                        msDetailDTOList = MSDetailDAL.GetStagingLocationByItem(itemGuid, RoomID, CompanyID, null, string.Empty, true);
                        foreach (var item in msDetailDTOList)
                        {
                            MSDAL = new MaterialStagingDAL(SessionHelper.EnterPriseDBName);
                            MaterialStagingDTO objMSDTO = MSDAL.GetRecord(item.MaterialStagingGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
                            DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                            objAutoDTO.GUID = item.MaterialStagingGUID.GetValueOrDefault(Guid.Empty);
                            objAutoDTO.ID = item.StagingBinID;
                            if (item.StagingBinName != "[|EmptyStagingBin|]")
                                objAutoDTO.Key = objMSDTO.StagingName + " - " + item.StagingBinName + " (" + item.Quantity.ToString(QtyFromate) + ")";
                            else
                                objAutoDTO.Key = objMSDTO.StagingName + " (" + item.Quantity.ToString(QtyFromate) + ")";
                            objAutoDTO.Value = item.StagingBinName;
                            objAutoDTO.Quantity = item.Quantity;
                            objAutoDTO.OtherInfo1 = objMSDTO.StagingName;
                            locations.Add(objAutoDTO);
                        }
                    }

                }
                else if (locType == "DL")
                {
                    if (moveType == MoveType.StagToInv || moveType == MoveType.InvToInv)
                    {
                        // Only Bind locations
                        ItmLocLevelQtyDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                        if (!string.IsNullOrEmpty(NameStartWith) && NameStartWith.Trim().Length > 0)
                        {
                            //ItmLocLevelQtyDTOList = ItmLocLevelQtyDAL.GetAllRecordsItemWise(itemGuid, RoomID, CompanyID).Where(x => !x.IsDeleted.GetValueOrDefault(false) && !x.IsArchived.GetValueOrDefault(false) && !x.IsStagingLocation && !string.IsNullOrWhiteSpace(x.BinNumber) && x.BinNumber.ToLower().StartsWith(NameStartWith.ToLower()));
                            ItmLocLevelQtyDTOList = ItmLocLevelQtyDAL.GetAllRecordsItemWise(itemGuid, RoomID, CompanyID,false,false,NameStartWith.ToLower());
                            //ItmLocLevelQtyDTOList = ItmLocLevelQtyDAL.GetAllRecordsItemWise(itemGuid, RoomID, CompanyID,null,false).Where(x => !x.IsDeleted.GetValueOrDefault(false) && !x.IsArchived.GetValueOrDefault(false)  && !string.IsNullOrWhiteSpace(x.BinNumber) && x.BinNumber.ToLower().StartsWith(NameStartWith.ToLower()));//&& !x.IsStagingLocation
                        }
                        else
                        {
                            //ItmLocLevelQtyDTOList = ItmLocLevelQtyDAL.GetAllRecordsItemWise(itemGuid, RoomID, CompanyID).Where(x => !string.IsNullOrWhiteSpace(x.BinNumber));// && !x.IsStagingLocation && !x.IsDeleted.GetValueOrDefault(false) && !x.IsArchived.GetValueOrDefault(false) &&
                            ItmLocLevelQtyDTOList = ItmLocLevelQtyDAL.GetAllRecordsItemWise(itemGuid, RoomID, CompanyID,null,false,string.Empty);
                            //ItmLocLevelQtyDTOList = ItmLocLevelQtyDAL.GetAllRecordsItemWise(itemGuid, RoomID, CompanyID, null, false).Where(x =>  !string.IsNullOrWhiteSpace(x.BinNumber));// && !x.IsStagingLocation && !x.IsDeleted.GetValueOrDefault(false) && !x.IsArchived.GetValueOrDefault(false) &&
                        }

                        //ItmLocLevelQtyDTOList = ItmLocLevelQtyDAL.GetAllBins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, NameStartWith, false).Where(x=>x.ItemGUID == itemGuid);

                        foreach (var item in ItmLocLevelQtyDTOList)
                        {
                            if (item.IsStagingLocation == false && locations.FindIndex(x => x.ID == item.ID) < 0)
                            {
                                DTOForAutoComplete objAutoDTO = new DTOForAutoComplete()
                                {
                                    GUID = item.GUID,
                                    ID = item.ID,
                                    Key = item.BinNumber,
                                    Value = item.BinNumber,
                                    Quantity = item.ConsignedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0),
                                };

                                locations.Add(objAutoDTO);
                            }
                        }
                        if (locations != null && locations.Count > 0)
                        {
                            locations = locations.OrderBy(x => x.Value).ToList();
                        }

                        if (IsLoadMoreLocations.HasValue)
                        {
                            if (IsLoadMoreLocations.Value)
                            {
                                //IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllBins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, NameStartWith.ToLower(), false).Where(x => !string.IsNullOrWhiteSpace(x.BinNumber)).OrderBy(x => x.BinNumber);
                                IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllBins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, NameStartWith.ToLower(), false, string.Empty, false, null).OrderBy(x => x.BinNumber);
                                if (objBinDTOList != null && objBinDTOList.Count() > 0)
                                {
                                    foreach (var item in objBinDTOList)
                                    {
                                        if (!locations.Any(x => x.Key == item.BinNumber))
                                        {
                                            DTOForAutoComplete objAutoDTO = new DTOForAutoComplete()
                                            {
                                                GUID = item.GUID,
                                                ID = item.ID,
                                                Key = item.BinNumber != "[|EmptyStagingBin|]" ? item.BinNumber : string.Empty,
                                                Value = item.BinNumber,
                                                Quantity = item.ConsignedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0),
                                            };
                                            locations.Add(objAutoDTO);
                                        }
                                    }
                                    if (locations != null && locations.Count > 0)
                                    {
                                        locations = locations.OrderBy(x => x.Value).ToList();
                                    }
                                }
                            }
                            else
                            {
                                DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                                objAutoDTO.GUID = Guid.Empty;
                                objAutoDTO.ID = 0;
                                objAutoDTO.Key = ResBin.MoreLocations;
                                objAutoDTO.Value = ResBin.MoreLocations;
                                objAutoDTO.Quantity = 0;
                                locations.Add(objAutoDTO);
                            }
                        }

                        //// All Location where qty availables
                        //itmLocQtyDAL = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName);
                        //itmLocQtyList = itmLocQtyDAL.GetRecordByItem(itemGuid, RoomID, CompanyID);
                        //foreach (var item in itmLocQtyList)
                        //{
                        //    BinMasterDTO objBin = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetRecord(item.BinID, RoomID, CompanyID, false, false);
                        //    if (locations.FindIndex(x => x.ID == objBin.ID) < 0)
                        //    {
                        //        DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                        //        objAutoDTO.GUID = item.GUID;
                        //        objAutoDTO.ID = objBin.ID;
                        //        objAutoDTO.Key = objBin.BinNumber + " (" + item.Quantity + ")";
                        //        objAutoDTO.Value = objBin.BinNumber;
                        //        objAutoDTO.Quantity = item.Quantity;
                        //        locations.Add(objAutoDTO);
                        //    }
                        //}
                    }
                    else if (moveType == MoveType.InvToStag || moveType == MoveType.StagToStag)
                    {
                        Guid stageHeadGUID = Guid.Empty;
                        Guid.TryParse(StageHeaderGUID, out stageHeadGUID);
                        MSDAL = new MaterialStagingDAL(SessionHelper.EnterPriseDBName);
                        MaterialStagingDTO objMSDTO = MSDAL.GetRecord(stageHeadGUID, RoomID, CompanyID);

                        if (objMSDTO != null && objMSDTO.BinID.GetValueOrDefault(0) <= 0)
                        {
                            MSDetailDAL = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName);
                            //msDetailDTOList = MSDetailDAL.GetStagingLocationByItemOnlyOpen(itemGuid, RoomID, CompanyID).Where(x => x.MaterialStagingGUID.GetValueOrDefault(Guid.Empty) == stageHeadGUID);
                            msDetailDTOList = MSDetailDAL.GetStagingLocationByItemOnlyOpen(itemGuid, RoomID, CompanyID, Convert.ToString(stageHeadGUID));
                            foreach (var item in msDetailDTOList)
                            {
                                DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                                objAutoDTO.GUID = item.MaterialStagingGUID.GetValueOrDefault(Guid.Empty);
                                objAutoDTO.ID = item.StagingBinID;
                                objAutoDTO.Key = item.StagingBinName != "[|EmptyStagingBin|]" ? item.StagingBinName + " (" + item.Quantity.ToString(QtyFromate) + ")" : string.Empty;
                                objAutoDTO.Value = item.StagingBinName;
                                objAutoDTO.Quantity = item.Quantity;
                                objAutoDTO.OtherInfo1 = objMSDTO.StagingName;
                                locations.Add(objAutoDTO);
                            }

                            if (locations != null && locations.Count > 0)
                            {
                                locations = locations.OrderBy(x => x.Value).ToList();
                            }
                        }
                        else if (objMSDTO != null)
                        {
                            BinMasterDTO objBin = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(objMSDTO.BinID.GetValueOrDefault(0), RoomID, CompanyID);
                            //BinMasterDTO objBin = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocation( RoomID, CompanyID, false, false,Guid.Empty, objMSDTO.BinID.GetValueOrDefault(0),null,null).FirstOrDefault();
                            MSDetailDAL = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName);
                            //msDetailDTOList = MSDetailDAL.GetStagingLocationByItemOnlyOpen(itemGuid, RoomID, CompanyID).Where(x => x.MaterialStagingGUID.GetValueOrDefault(Guid.Empty) == stageHeadGUID);
                            msDetailDTOList = MSDetailDAL.GetStagingLocationByItemOnlyOpen(itemGuid, RoomID, CompanyID, Convert.ToString(stageHeadGUID));
                            double qty = 0;
                            if (msDetailDTOList != null & msDetailDTOList.Count() > 0)
                            {
                                qty = msDetailDTOList.ToList()[0].Quantity;
                            }

                            DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                            objAutoDTO.GUID = objMSDTO.GUID;
                            objAutoDTO.ID = objMSDTO.BinID.GetValueOrDefault(0);
                            objAutoDTO.Key = objBin.BinNumber != "[|EmptyStagingBin|]" ? objBin.BinNumber + " (" + qty.ToString(QtyFromate) + ")" : string.Empty;
                            objAutoDTO.Value = objBin.BinNumber;
                            objAutoDTO.Quantity = qty;
                            objAutoDTO.OtherInfo1 = objMSDTO.StagingName;
                            locations.Add(objAutoDTO);
                        }
                        else
                        {
                            MSDetailDAL = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName);
                            //msDetailDTOList = MSDetailDAL.GetStagingLocationByItemOnlyOpen(itemGuid, RoomID, CompanyID);
                            msDetailDTOList = MSDetailDAL.GetStagingLocationByItemOnlyOpen(itemGuid, RoomID, CompanyID, string.Empty);
                            foreach (var item in msDetailDTOList)
                            {
                                DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                                objAutoDTO.GUID = item.MaterialStagingGUID.GetValueOrDefault(Guid.Empty);
                                objAutoDTO.ID = item.StagingBinID;
                                objAutoDTO.Key = item.StagingBinName != "[|EmptyStagingBin|]" ? item.StagingBinName + " (" + item.Quantity.ToString(QtyFromate) + ")" : string.Empty;
                                objAutoDTO.Value = item.StagingBinName;
                                objAutoDTO.Quantity = item.Quantity;
                                objAutoDTO.OtherInfo1 = item.MaterialStagingName;
                                locations.Add(objAutoDTO);
                            }

                            if (locations != null && locations.Count > 0)
                            {
                                locations = locations.OrderBy(x => x.Value).ToList();
                            }
                        }
                        if (locations != null && locations.Count > 0)
                        {
                            locations = locations.OrderBy(x => x.Value).ToList();
                        }

                        if (IsLoadMoreLocations.HasValue)
                        {
                            if (IsLoadMoreLocations.Value)
                            {
                                //IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllBins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, NameStartWith.ToLower(), true).Where(x => !string.IsNullOrWhiteSpace(x.BinNumber)).OrderBy(x => x.BinNumber);
                                IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllBins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, NameStartWith.ToLower(), true, string.Empty, false, null).OrderBy(x => x.BinNumber);
                                if (objBinDTOList != null && objBinDTOList.Count() > 0)
                                {
                                    foreach (var item in objBinDTOList)
                                    {
                                        if (item.IsStagingLocation == true && !locations.Any(x => x.Key == item.BinNumber))
                                        {
                                            DTOForAutoComplete objAutoDTO = new DTOForAutoComplete()
                                            {
                                                GUID = stageHeadGUID,
                                                ID = 0,
                                                Key = (item.BinNumber == "[|EmptyStagingBin|]" ? string.Empty : item.BinNumber),
                                                Value = item.BinNumber,
                                                Quantity = item.ConsignedQuantity.GetValueOrDefault(0) + item.CustomerOwnedQuantity.GetValueOrDefault(0),
                                                OtherInfo1 = string.Empty,
                                            };
                                            locations.Add(objAutoDTO);
                                        }
                                    }

                                    if (locations != null && locations.Count > 0)
                                    {
                                        locations = locations.OrderBy(x => x.Value).ToList();
                                    }
                                }

                            }
                            else
                            {
                                DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                                objAutoDTO.GUID = stageHeadGUID;
                                objAutoDTO.ID = 0;
                                objAutoDTO.Key = ResBin.MoreLocations;
                                objAutoDTO.Value = ResBin.MoreLocations;
                                objAutoDTO.Quantity = 0;
                                objAutoDTO.OtherInfo1 = string.Empty;
                                locations.Add(objAutoDTO);
                            }
                        }
                    }
                }
                else if (locType == "SH")
                {
                    MSDAL = new MaterialStagingDAL(SessionHelper.EnterPriseDBName);
                    //MSList = MSDAL.GetAllRecords(RoomID, CompanyID, false, false).Where(x => x.StagingStatus == 1);
                    MSList = MSDAL.GetMaterialStaging(RoomID, CompanyID, false, false, string.Empty, 1);
                    foreach (var item in MSList)
                    {
                        DTOForAutoComplete objAutoDTO = new DTOForAutoComplete()
                        {
                            GUID = item.GUID,
                            ID = item.BinID.GetValueOrDefault(0),
                            Key = item.StagingName != "[|EmptyStagingBin|]" ? item.StagingName : string.Empty,
                            Value = item.StagingName,
                            Quantity = 0,

                        };
                        if (item.BinID.GetValueOrDefault(0) > 0)
                        {
                            BinMasterDTO objBin = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(item.BinID.GetValueOrDefault(0), RoomID, CompanyID);
                            //BinMasterDTO objBin = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocation( RoomID, CompanyID, false, false,Guid.Empty, item.BinID.GetValueOrDefault(0),null,null).FirstOrDefault();
                            if (objBin != null)
                                objAutoDTO.OtherInfo2 = objBin.BinNumber;
                        }

                        locations.Add(objAutoDTO);

                    }
                    if (locations != null && locations.Count > 0)
                    {
                        locations = locations.OrderBy(x => x.Value).ToList();
                    }
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                MSDetailDAL = null;
                MSDAL = null;
                ItmLocLevelQtyDAL = null;
                ItmLocLevelQtyDTOList = null;
                MSList = null;
                itmLocQtyDAL = null;
                itmLocQtyList = null;
                msDetailDTOList = null;
            }

            if (!string.IsNullOrWhiteSpace(NameStartWith) && locations != null && locations.Count > 0)
            {
                locations = locations.Where(x => x.Value.ToLower().StartsWith(NameStartWith.Trim().ToLower())).ToList();
            }



            return Json(locations, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// MoveQuantitySourceToDestination
        /// </summary>
        /// <param name="moveMTRDTO"></param>
        /// <returns></returns>
        /// 

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult MoveQuantitySourceToDestination(MoveMaterialDTO moveMTRDTO)
        {
            List<string> lstErrors = new List<string>();
            try
            {
                if (moveMTRDTO.MoveType == (int)MoveType.InvToStag || moveMTRDTO.MoveType == (int)MoveType.StagToStag)
                {
                    if (string.IsNullOrWhiteSpace(moveMTRDTO.DestinationLocation))
                    {
                        moveMTRDTO.DestinationLocation = "[|EmptyStagingBin|]";
                    }

                }
                if (ValidateMove(moveMTRDTO, lstErrors))
                {
                    moveMTRDTO.CreatedBy = SessionHelper.UserID;
                    moveMTRDTO.UpdatedBy = SessionHelper.UserID;
                    moveMTRDTO.RoomID = RoomID;
                    moveMTRDTO.CompanyID = CompanyID;
                    moveMTRDTO.UpdatedOn = DateTimeUtility.DateTimeNow;
                    moveMTRDTO.CreatedOn = DateTimeUtility.DateTimeNow;

                    string[] stringSeparators = new string[] { };
                    if (!string.IsNullOrEmpty(moveMTRDTO.DestinationLocation) && moveMTRDTO.DestinationLocation.Contains("[||]"))
                    {
                        stringSeparators = moveMTRDTO.DestinationLocation.Split(new string[1] { "[||]" }, StringSplitOptions.RemoveEmptyEntries);
                        if (stringSeparators != null && stringSeparators.Count() > 0)
                            moveMTRDTO.DestinationLocation = stringSeparators[1];
                    }

                    if (!string.IsNullOrEmpty(moveMTRDTO.SourceLocation) && moveMTRDTO.SourceLocation.Contains("[||]"))
                    {
                        stringSeparators = new string[] { };
                        stringSeparators = moveMTRDTO.SourceLocation.Split(new string[1] { "[||]" }, StringSplitOptions.RemoveEmptyEntries);
                        if (stringSeparators != null && stringSeparators.Count() > 0)
                            moveMTRDTO.SourceLocation = stringSeparators[1];
                    }

                    MoveMaterialDAL objMoveDAL = new MoveMaterialDAL(SessionHelper.EnterPriseDBName);
                    //TODO: Chirag 08-08-2017, Do not change AddedFrom and Editedfrom is used in trigger to prevent same twice record in ATT
                    moveMTRDTO.AddedFrom = "Web-Move";
                    moveMTRDTO.EditedFrom = "Web-Move";
                    long SessionUserId = SessionHelper.UserID;
                    bool returnStatus = objMoveDAL.MoveInventory(moveMTRDTO, SessionUserId,SessionHelper.RoomDateFormat,SessionHelper.EnterPriceID);

                    return Json(new { Message = "ok", Status = returnStatus, ErrorList = lstErrors }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Message = "Errors", Status = false, ErrorList = lstErrors }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                lstErrors.Add(ex.Message.ToString());
                return Json(new { Message = "Errors", Status = false, ErrorList = lstErrors }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult MoveQuantitySourceToDestinationPost(MoveMaterialDTO moveMTRDTO)
        {
            List<string> lstErrors = new List<string>();
            try
            {
                if (moveMTRDTO.MoveType == (int)MoveType.InvToStag || moveMTRDTO.MoveType == (int)MoveType.StagToStag)
                {
                    if (string.IsNullOrWhiteSpace(moveMTRDTO.DestinationLocation))
                    {
                        moveMTRDTO.DestinationLocation = "[|EmptyStagingBin|]";
                    }

                }
                if (ValidateMove(moveMTRDTO, lstErrors))
                {
                    moveMTRDTO.CreatedBy = SessionHelper.UserID;
                    moveMTRDTO.UpdatedBy = SessionHelper.UserID;
                    moveMTRDTO.RoomID = RoomID;
                    moveMTRDTO.CompanyID = CompanyID;
                    moveMTRDTO.UpdatedOn = DateTimeUtility.DateTimeNow;
                    moveMTRDTO.CreatedOn = DateTimeUtility.DateTimeNow;

                    string[] stringSeparators = new string[] { };
                    if (!string.IsNullOrEmpty(moveMTRDTO.DestinationLocation) && moveMTRDTO.DestinationLocation.Contains("[||]"))
                    {
                        stringSeparators = moveMTRDTO.DestinationLocation.Split(new string[1] { "[||]" }, StringSplitOptions.RemoveEmptyEntries);
                        if (stringSeparators != null && stringSeparators.Count() > 0)
                            moveMTRDTO.DestinationLocation = stringSeparators[1];
                    }

                    if (!string.IsNullOrEmpty(moveMTRDTO.SourceLocation) && moveMTRDTO.SourceLocation.Contains("[||]"))
                    {
                        stringSeparators = new string[] { };
                        stringSeparators = moveMTRDTO.SourceLocation.Split(new string[1] { "[||]" }, StringSplitOptions.RemoveEmptyEntries);
                        if (stringSeparators != null && stringSeparators.Count() > 0)
                            moveMTRDTO.SourceLocation = stringSeparators[1];
                    }

                    MoveMaterialDAL objMoveDAL = new MoveMaterialDAL(SessionHelper.EnterPriseDBName);
                    //TODO: Chirag 08-08-2017, Do not change AddedFrom and Editedfrom is used in trigger to prevent same twice record in ATT
                    moveMTRDTO.AddedFrom = "Web-Move";
                    moveMTRDTO.EditedFrom = "Web-Move";
                    long SessionUserId = SessionHelper.UserID;
                    bool returnStatus = objMoveDAL.MoveInventory(moveMTRDTO, SessionUserId,SessionHelper.RoomDateFormat,SessionHelper.EnterPriceID);

                    return Json(new { Message = "ok", Status = returnStatus, ErrorList = lstErrors }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Message = "Errors", Status = false, ErrorList = lstErrors }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                lstErrors.Add(ex.Message.ToString());
                return Json(new { Message = "Errors", Status = false, ErrorList = lstErrors }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// ValidateMove
        /// </summary>
        /// <param name="objDTO"></param>
        /// <param name="lstErrorsList"></param>
        /// <returns></returns>
        public bool ValidateMove(MoveMaterialDTO objDTO, List<string> lstErrorsList)
        {
            bool isValid = true;
            if (objDTO.SourceBinID <= 0)
            {
                lstErrorsList.Add(ResMoveMaterial.ErrorMsgSourceLocation);
                isValid = false;
            }

            if (objDTO.MoveQuanity <= 0)
            {
                lstErrorsList.Add(ResMoveMaterial.ErrorMsgMoveQty);
                isValid = false;
            }

            if (objDTO.MoveType == (int)MoveType.InvToStag || objDTO.MoveType == (int)MoveType.StagToStag)
            {
                if (string.IsNullOrEmpty(objDTO.DestinationStagingHeader))
                {
                    lstErrorsList.Add(ResMoveMaterial.ErrorMsgDestinationStagingHeader);
                    isValid = false;
                }
            }

            if (string.IsNullOrEmpty(objDTO.DestinationLocation))
            {
                lstErrorsList.Add(ResMoveMaterial.ErrorMsgDestinationLocation);
                isValid = false;
            }

            if (objDTO.MoveType == (int)MoveType.InvToInv && objDTO.SourceBinID > 0 && objDTO.SourceBinID == objDTO.DestBinID)
            {
                lstErrorsList.Add(ResMoveMaterial.ErrorMsgSourceAndDestinationAreSame);
                isValid = false;
            }

            if (objDTO.MoveType == (int)MoveType.StagToStag
                && objDTO.SourceStagingHeaderGuid.GetValueOrDefault(Guid.Empty) == objDTO.DestStagingHeaderGuid.GetValueOrDefault(Guid.Empty)
                && objDTO.SourceBinID == objDTO.DestBinID)
            {
                lstErrorsList.Add(ResMoveMaterial.ErrorMsgSourceAndDestinationAreSame);
                isValid = false;
            }
            double oveTotalSourceQty = 0;
            if (objDTO.MoveQuanity > 0)
            {
                if (objDTO.MoveType == (int)MoveType.InvToInv || objDTO.MoveType == (int)MoveType.InvToStag)
                {
                    ItemLocationDetailsDAL objItmLocDtlsDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                    IEnumerable<ItemLocationDetailsDTO> SourceItemLocations = objItmLocDtlsDAL.GetCustomerFirstThenConsigedByLIFOFIFO(false, objDTO.SourceBinID, RoomID, CompanyID, objDTO.ItemGUID, null);
                    if (SourceItemLocations != null && SourceItemLocations.Count() > 0)
                    {
                        oveTotalSourceQty = SourceItemLocations.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0) + x.CustomerOwnedQuantity.GetValueOrDefault(0));
                    }
                    objItmLocDtlsDAL = null;
                    SourceItemLocations = null;

                }
                else if (objDTO.MoveType == (int)MoveType.StagToInv || objDTO.MoveType == (int)MoveType.StagToStag)
                {
                    MaterialStagingPullDetailDAL objMSPullDtlDAL = new MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName);
                    //IEnumerable<MaterialStagingPullDetailDTO> SourceItemLocations = objMSPullDtlDAL.GetMsPullDetailsByItemGUIDANDBinID(objDTO.ItemGUID, objDTO.SourceBinID, RoomID, CompanyID).Where(x => x.MaterialStagingGUID.GetValueOrDefault(Guid.Empty) == objDTO.SourceStagingHeaderGuid);
                    IEnumerable<MaterialStagingPullDetailDTO> SourceItemLocations = objMSPullDtlDAL.GetMsPullDetailsByItemGUIDANDBinIDANDMSGUID(objDTO.ItemGUID, objDTO.SourceBinID, RoomID, CompanyID, objDTO.SourceStagingHeaderGuid.GetValueOrDefault((Guid.Empty)));
                    if (SourceItemLocations != null && SourceItemLocations.Count() > 0)
                    {
                        oveTotalSourceQty = SourceItemLocations.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0) + x.CustomerOwnedQuantity.GetValueOrDefault(0));
                    }
                    objMSPullDtlDAL = null;
                    SourceItemLocations = null;
                }

                if (oveTotalSourceQty < objDTO.MoveQuanity)
                {
                    lstErrorsList.Add(ResMoveMaterial.ErrorMsgNotSuffucientQtyAtSource);
                    isValid = false;
                }
            }

            return isValid;
        }

        /// <summary>
        /// Get Move Type List
        /// </summary>
        /// <param name="openFrom"></param>
        /// <returns></returns>
        private List<CommonDTO> GetMoveTypeList(MoveDialogOpenFrom openFrom)
        {
            List<CommonDTO> lstCommon = new List<CommonDTO>();
            if (openFrom == MoveDialogOpenFrom.FromMove)
            {
                lstCommon.Add(new CommonDTO() { ID = 1, Text = ResMoveMaterial.MoveTypeItemInvtoInv });
                lstCommon.Add(new CommonDTO() { ID = 2, Text = ResMoveMaterial.MoveTypeItemInvtoStage });
                lstCommon.Add(new CommonDTO() { ID = 3, Text = ResMoveMaterial.MoveTypeItemStageToInv });
                lstCommon.Add(new CommonDTO() { ID = 4, Text = ResMoveMaterial.MoveTypeItemStageToStage });
            }
            if (openFrom == MoveDialogOpenFrom.FromItem)
            {
                lstCommon.Add(new CommonDTO() { ID = 1, Text = ResMoveMaterial.MoveTypeItemInvtoInv });
                lstCommon.Add(new CommonDTO() { ID = 2, Text = ResMoveMaterial.MoveTypeItemInvtoStage });
            }
            if (openFrom == MoveDialogOpenFrom.FromStage)
            {
                lstCommon.Add(new CommonDTO() { ID = 3, Text = ResMoveMaterial.MoveTypeItemStageToInv });
                lstCommon.Add(new CommonDTO() { ID = 4, Text = ResMoveMaterial.MoveTypeItemStageToStage });
            }
            return lstCommon;
        }


        public ActionResult LotSrSelectionForMove(JQueryDataTableParamModel param)
        {
            Guid ItemGUID = Guid.Empty;
            Guid.TryParse(Convert.ToString(Request["ItemGUID"]), out ItemGUID);

            long BinID = 0;
            long.TryParse(Convert.ToString(Request["BinID"]), out BinID);
            string CurrentLoaded = Convert.ToString(Request["CurrentLoaded"]);
            bool IsDeleteRowMode = Convert.ToBoolean(Request["IsDeleteRowMode"]);
            int intMoveType = 0;
            int.TryParse(Convert.ToString(Request["MoveType"]), out intMoveType);

            string[] arrIds = new string[] { };

            if (!string.IsNullOrWhiteSpace(CurrentLoaded))
            {
                arrIds = CurrentLoaded.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }

            ItemMasterDTO oItem = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, ItemGUID);

            int TotalRecordCount = 0;
            ItemLocationDetailsDAL objILD = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            MaterialStagingPullDetailDAL objMSPD = new MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName);
            List<ItemLocationLotSerialDTO> lstLotSrs = new List<ItemLocationLotSerialDTO>();
            List<ItemLocationLotSerialDTO> retlstLotSrs = new List<ItemLocationLotSerialDTO>();
            //Dictionary<string, double> dicSerialLots = new Dictionary<string, double>();
            List<ItemLocationLotSerialDTO> dicSerialLots = new List<ItemLocationLotSerialDTO>();
            string[] arrItem;

            if (arrIds.Count() > 0)
            {
                //string[] arrSerialLots = new string[arrIds.Count()];
                
                for (int i = 0; i < arrIds.Count(); i++)
                {
                    ItemLocationLotSerialDTO dicSerialLot = new ItemLocationLotSerialDTO();

                    if ((oItem.SerialNumberTracking && !oItem.DateCodeTracking)
                        || (oItem.LotNumberTracking && !oItem.DateCodeTracking)
                        || !oItem.DateCodeTracking)
                    {
                        arrItem = new string[] { };
                        arrItem = arrIds[i].Split(new string[] { "&_&" }, StringSplitOptions.RemoveEmptyEntries);

                        //arrItem = new string[3];
                        //arrItem[0] = arrIds[i].Substring(0, arrIds[i].LastIndexOf("_"));
                        //arrItem[1] = arrIds[i].Replace(arrItem[0] + "_", "");
                        if (arrItem.Length > 1)
                        {
                            //arrSerialLots[i] = arrItem[0] + "_" + arrItem[2];
                            dicSerialLot.SerialLotExpirationcombin = arrItem[0];
                            dicSerialLot.QuantityToMove = Convert.ToDouble(arrItem[1]);
                            if (arrItem.Length > 2)
                                dicSerialLot.BinNumber = arrItem[2];
                            else
                                dicSerialLot.BinNumber = "";
                            dicSerialLots.Add(dicSerialLot);
                            //dicSerialLots.Add(arrItem[0], Convert.ToDouble(arrItem[1]));
                        }
                    }
                    else if ((oItem.SerialNumberTracking && oItem.DateCodeTracking)
                        || (oItem.LotNumberTracking && oItem.DateCodeTracking))
                    {
                        arrItem = new string[] { };
                        arrItem = arrIds[i].Split(new string[] { "&_&" }, StringSplitOptions.RemoveEmptyEntries);

                        if (arrItem.Length > 1)
                        {
                            //arrSerialLots[i] = arrItem[0] + "_" + arrItem[1] + "_" + arrItem[3];
                            dicSerialLot.SerialLotExpirationcombin = arrItem[0] + "_" + arrItem[1];
                            dicSerialLot.QuantityToMove = Convert.ToDouble(arrItem[2]);
                            if (arrItem.Length > 3)
                                dicSerialLot.BinNumber = arrItem[3];
                            else
                                dicSerialLot.BinNumber = "";
                            dicSerialLots.Add(dicSerialLot);
                            //dicSerialLots.Add(arrItem[0], Convert.ToDouble(arrItem[2]));
                        }
                    }
                    else if (!oItem.SerialNumberTracking && !oItem.DateCodeTracking && oItem.DateCodeTracking)
                    {
                        arrItem = new string[] { };
                        arrItem = arrIds[i].Split(new string[] { "&_&" }, StringSplitOptions.RemoveEmptyEntries);

                        if (arrItem.Length > 1)
                        {
                            //arrSerialLots[i] = arrItem[0] + "_" + arrItem[2];
                            dicSerialLot.SerialLotExpirationcombin = arrItem[0];
                            dicSerialLot.QuantityToMove = Convert.ToDouble(arrItem[1]);
                            if (arrItem.Length > 2)
                                dicSerialLot.BinNumber = arrItem[2];
                            else
                                dicSerialLot.BinNumber = "";
                            dicSerialLots.Add(dicSerialLot);
                            //dicSerialLots.Add(arrItem[0], Convert.ToDouble(arrItem[1]));
                        }
                    }
                    else
                    {
                        arrItem = new string[] { };
                        arrItem = arrIds[i].Split(new string[] { "&_&" }, StringSplitOptions.RemoveEmptyEntries);

                        if (arrItem.Length > 1)
                        {
                            //arrSerialLots[i] = arrItem[0];
                            dicSerialLot.BinNumber = arrItem[0];
                            dicSerialLot.QuantityToMove = Convert.ToDouble(arrItem[1]);
                            //dicSerialLot.BinNumber = arrItem[2];
                            dicSerialLots.Add(dicSerialLot);

                            //dicSerialLots.Add(arrItem[0], Convert.ToDouble(arrItem[1]));
                        }
                    }
                }

                if (intMoveType == (int)MoveType.InvToInv || intMoveType == (int)MoveType.InvToStag)
                    lstLotSrs = objILD.GetItemLocationsWithLotSerialsForMove(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                else if (intMoveType == (int)MoveType.StagToInv || intMoveType == (int)MoveType.StagToStag)
                {
                    lstLotSrs = objMSPD.GetMaterialStagingPullWithLotSerialsForMove(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    foreach (ItemLocationLotSerialDTO objDto in lstLotSrs)
                    {
                        if (!string.IsNullOrWhiteSpace(objDto.BinNumber))
                            objDto.BinNumber = (objDto.BinNumber.ToLower().Equals("[|EmptyStagingBin|]".ToLower()) ? "" : objDto.BinNumber);
                    }
                }

                //retlstLotSrs = lstLotSrs.Where(t => ((arrSerialLots.Contains(t.LotOrSerailNumber + "_" + t.BinNumber) && (oItem.SerialNumberTracking || oItem.LotNumberTracking))
                //    || (arrSerialLots.Contains(t.Expiration) && oItem.DateCodeTracking)
                //    || (arrSerialLots.Contains(t.BinNumber) && !oItem.SerialNumberTracking && !oItem.LotNumberTracking && !oItem.DateCodeTracking))).ToList();

                retlstLotSrs = lstLotSrs.Where(t =>
                       (
                           (
                               //arrSerialLots.Contains(t.LotOrSerailNumber, StringComparer.OrdinalIgnoreCase)
                               dicSerialLots.Where(x => x.SerialLotExpirationcombin == t.LotOrSerailNumber
                                                    && x.BinNumber == t.BinNumber).Count() > 0
                               && (oItem.SerialNumberTracking || oItem.LotNumberTracking)
                               && !oItem.DateCodeTracking)
                       ||
                           (
                               //arrSerialLots.Contains(t.SerialLotExpirationcombin)
                               dicSerialLots.Where(x => x.SerialLotExpirationcombin == t.SerialLotExpirationcombin
                                                    && x.BinNumber == t.BinNumber).Count() > 0
                               && (oItem.SerialNumberTracking || oItem.LotNumberTracking)
                               && oItem.DateCodeTracking)
                       ||
                           (
                               //arrSerialLots.Contains(t.SerialLotExpirationcombin)
                               dicSerialLots.Where(x => x.SerialLotExpirationcombin == t.SerialLotExpirationcombin
                                                    && x.BinNumber == t.BinNumber).Count() > 0
                               && (!oItem.SerialNumberTracking && !oItem.LotNumberTracking)
                               && oItem.DateCodeTracking)
                       || (
                                //arrSerialLots.Contains(t.BinNumber)
                                dicSerialLots.Where(x => x.BinNumber == t.BinNumber).Count() > 0
                                && !oItem.SerialNumberTracking && !oItem.LotNumberTracking && !oItem.DateCodeTracking
                            )
                      )).ToList();

                if (!IsDeleteRowMode)
                {
                    //retlstLotSrs = retlstLotSrs.Union(lstLotSrs.Where(t =>
                    //        ((!arrSerialLots.Contains(t.LotOrSerailNumber + "_" + t.BinNumber) && (oItem.SerialNumberTracking || oItem.LotNumberTracking))
                    //    || (!arrSerialLots.Contains(t.Expiration) && oItem.DateCodeTracking)
                    //    || (!arrSerialLots.Contains(t.BinNumber) && !oItem.SerialNumberTracking && !oItem.LotNumberTracking && !oItem.DateCodeTracking))).Take(1)).ToList();

                    retlstLotSrs =
                               retlstLotSrs.Union
                               (
                                   lstLotSrs.Where(t =>
                                 (
                                       (
                                           //!arrSerialLots.Contains(t.LotOrSerailNumber, StringComparer.OrdinalIgnoreCase)
                                           dicSerialLots.Where(x => x.SerialLotExpirationcombin == t.LotOrSerailNumber
                                                    && x.BinNumber == t.BinNumber).Count() <= 0
                                           && (oItem.SerialNumberTracking || oItem.LotNumberTracking)
                                           && !oItem.DateCodeTracking
                                       )
                                   ||
                                       (
                                           //!arrSerialLots.Contains(t.SerialLotExpirationcombin)
                                           dicSerialLots.Where(x => x.SerialLotExpirationcombin == t.SerialLotExpirationcombin
                                                    && x.BinNumber == t.BinNumber).Count() <= 0
                                           && (oItem.SerialNumberTracking || oItem.LotNumberTracking)
                                           && oItem.DateCodeTracking
                                       )
                                   ||
                                       (
                                           //!arrSerialLots.Contains(t.SerialLotExpirationcombin)
                                           dicSerialLots.Where(x => x.SerialLotExpirationcombin == t.SerialLotExpirationcombin
                                                    && x.BinNumber == t.BinNumber).Count() <= 0
                                           && (!oItem.SerialNumberTracking && !oItem.LotNumberTracking)
                                           && oItem.DateCodeTracking
                                       )
                                   ||
                                       (
                                           //!arrSerialLots.Contains(t.BinNumber)
                                           dicSerialLots.Where(x => x.BinNumber == t.BinNumber).Count() <= 0
                                           && !oItem.SerialNumberTracking
                                           && !oItem.LotNumberTracking
                                           && !oItem.DateCodeTracking
                                        )
                                )).Take(1)
                             ).ToList();
                }
            }
            else
            {
                if (intMoveType == (int)MoveType.InvToInv || intMoveType == (int)MoveType.InvToStag)
                    retlstLotSrs = objILD.GetItemLocationsWithLotSerialsForMove(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
                else if (intMoveType == (int)MoveType.StagToInv || intMoveType == (int)MoveType.StagToStag)
                    retlstLotSrs = objMSPD.GetMaterialStagingPullWithLotSerialsForMove(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();

                if (BinID > 0)
                    retlstLotSrs = retlstLotSrs.Where(x => x.BinID == BinID).Take(1).ToList();
                else
                    retlstLotSrs = retlstLotSrs.Take(1).ToList();
                foreach (ItemLocationLotSerialDTO objDto in retlstLotSrs)
                {
                    if (!string.IsNullOrWhiteSpace(objDto.BinNumber))
                        objDto.BinNumber = (objDto.BinNumber.ToLower().Equals("[|EmptyStagingBin|]".ToLower()) ? "" : objDto.BinNumber);
                    if(objDto.ExpirationDate != null)
                    {
                        objDto.Expiration = FnCommon.ConvertDateByTimeZone(objDto.ExpirationDate, false, true);
                    }
                }
            }

            foreach (ItemLocationLotSerialDTO item in retlstLotSrs)
            {
                if (dicSerialLots.Where(x => x.SerialLotExpirationcombin == item.LotOrSerailNumber && x.BinNumber == item.BinNumber).Count() > 0
                    && (oItem.SerialNumberTracking || oItem.LotNumberTracking))
                {
                    double value = dicSerialLots.Where(x => x.SerialLotExpirationcombin == item.LotOrSerailNumber && x.BinNumber == item.BinNumber).FirstOrDefault().QuantityToMove;
                    item.QuantityToMove = value;
                }
                else if (dicSerialLots.Where(x => x.SerialLotExpirationcombin == item.Expiration && x.BinNumber == item.BinNumber).Count() > 0
                        && oItem.DateCodeTracking)
                {
                    double value = dicSerialLots.Where(x => x.SerialLotExpirationcombin == item.Expiration && x.BinNumber == item.BinNumber).FirstOrDefault().QuantityToMove;
                    item.QuantityToMove = value;
                }
                else if (dicSerialLots.Where(x => x.BinNumber == item.BinNumber).Count() > 0
                        && !oItem.SerialNumberTracking && !oItem.LotNumberTracking && !oItem.DateCodeTracking)
                {
                    double value = dicSerialLots.Where(x => x.BinNumber == item.BinNumber).FirstOrDefault().QuantityToMove;
                    item.QuantityToMove = value;
                }
                if (item.ExpirationDate != null)
                {
                    item.Expiration = FnCommon.ConvertDateByTimeZone(item.ExpirationDate, false, true);
                }
            }

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = retlstLotSrs
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLotOrSerailNumberListForMove(int maxRows, string name_startsWith, Guid? ItemGuid, int intMoveType)
        {
            ItemLocationDetailsDAL objILD = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            MaterialStagingPullDetailDAL objMSPD = new MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName);
            List<ItemLocationLotSerialDTO> objItemLocationLotSerialDTO = null;

            if (intMoveType == (int)MoveType.InvToInv || intMoveType == (int)MoveType.InvToStag)
                objItemLocationLotSerialDTO = objILD.GetItemLocationsWithLotSerialsForMove(ItemGuid ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID);
            else if (intMoveType == (int)MoveType.StagToInv || intMoveType == (int)MoveType.StagToStag)
                objItemLocationLotSerialDTO = objMSPD.GetMaterialStagingPullWithLotSerialsForMove(ItemGuid ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID);

            objItemLocationLotSerialDTO.ToList().ForEach(x => x.BinNumber = x.BinNumber.ToLower().Equals("[|EmptyStagingBin|]".ToLower()) ? "" : x.BinNumber);

            var lstLotSr =
                objItemLocationLotSerialDTO.Where(x => x.LotOrSerailNumber.Contains(name_startsWith)).Select(x => new { x.LotOrSerailNumber, x.BinNumber, DisplatText = x.LotOrSerailNumber + ((x.BinNumber == "" || x.BinNumber.ToLower().Equals("[|EmptyStagingBin|]".ToLower())) ? "" : " (" + x.BinNumber + ")") }).Distinct();

            if (lstLotSr.Count() == 0)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            return Json(lstLotSr, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ValidateSerialLotNumber(Guid? ItemGuid, string SerialOrLotNumber, string BinNumber, int intMoveType)
        {
            ItemLocationDetailsDAL objILD = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            MaterialStagingPullDetailDAL objMSPD = new MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName);
            ItemLocationLotSerialDTO objItemLocationLotSerialDTO = null;

            if (intMoveType == (int)MoveType.InvToInv || intMoveType == (int)MoveType.InvToStag)
            {
                if (!string.IsNullOrEmpty(BinNumber))
                {
                    objItemLocationLotSerialDTO = objILD.GetItemLocationsWithLotSerialsForMove(ItemGuid ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.BinNumber == BinNumber && x.LotOrSerailNumber == SerialOrLotNumber).FirstOrDefault();
                    if (objItemLocationLotSerialDTO != null)
                        objItemLocationLotSerialDTO.BinNumber = objItemLocationLotSerialDTO.BinNumber.ToLower().Equals("[|EmptyStagingBin|]".ToLower()) ? "" : objItemLocationLotSerialDTO.BinNumber;
                }
                else
                {
                    objItemLocationLotSerialDTO = objILD.GetItemLocationsWithLotSerialsForMove(ItemGuid ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.LotOrSerailNumber == SerialOrLotNumber).FirstOrDefault();
                    if (objItemLocationLotSerialDTO != null)
                        objItemLocationLotSerialDTO.BinNumber = objItemLocationLotSerialDTO.BinNumber.ToLower().Equals("[|EmptyStagingBin|]".ToLower()) ? "" : objItemLocationLotSerialDTO.BinNumber;
                }
                if (objItemLocationLotSerialDTO != null 
                    && objItemLocationLotSerialDTO.ExpirationDate != null)
                {
                    objItemLocationLotSerialDTO.Expiration = FnCommon.ConvertDateByTimeZone(objItemLocationLotSerialDTO.ExpirationDate, false, true);
                }
            }
            else if (intMoveType == (int)MoveType.StagToInv || intMoveType == (int)MoveType.StagToStag)
            {
                if (!string.IsNullOrEmpty(BinNumber))
                {
                    objItemLocationLotSerialDTO = objMSPD.GetMaterialStagingPullWithLotSerialsForMove(ItemGuid ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.BinNumber == BinNumber && x.LotOrSerailNumber == SerialOrLotNumber).FirstOrDefault();
                    if (objItemLocationLotSerialDTO != null)
                        objItemLocationLotSerialDTO.BinNumber = objItemLocationLotSerialDTO.BinNumber.ToLower().Equals("[|EmptyStagingBin|]".ToLower()) ? "" : objItemLocationLotSerialDTO.BinNumber;
                }
                else
                {
                    objItemLocationLotSerialDTO = objMSPD.GetMaterialStagingPullWithLotSerialsForMove(ItemGuid ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.LotOrSerailNumber == SerialOrLotNumber).FirstOrDefault();
                    if (objItemLocationLotSerialDTO != null)
                        objItemLocationLotSerialDTO.BinNumber = objItemLocationLotSerialDTO.BinNumber.ToLower().Equals("[|EmptyStagingBin|]".ToLower()) ? "" : objItemLocationLotSerialDTO.BinNumber;
                }
                if (objItemLocationLotSerialDTO != null
                    && objItemLocationLotSerialDTO.ExpirationDate != null)
                {
                    objItemLocationLotSerialDTO.Expiration = FnCommon.ConvertDateByTimeZone(objItemLocationLotSerialDTO.ExpirationDate, false, true);
                }
            }

            if (objItemLocationLotSerialDTO == null)
            {
                objItemLocationLotSerialDTO = new ItemLocationLotSerialDTO();
                objItemLocationLotSerialDTO.BinID = 0;
                objItemLocationLotSerialDTO.ID = 0;
                objItemLocationLotSerialDTO.ItemGUID = ItemGuid;
                objItemLocationLotSerialDTO.LotOrSerailNumber = string.Empty;
                objItemLocationLotSerialDTO.Expiration = string.Empty;
                objItemLocationLotSerialDTO.BinNumber = string.Empty;
            }
            return Json(objItemLocationLotSerialDTO);
        }

        /// <summary>
        /// MoveQuantitySourceToDestination
        /// </summary>
        /// <param name="moveMTRDTO"></param>
        /// <returns></returns>
        public JsonResult MoveQuantitySourceToDestinationLotSr(List<MoveMaterialDTO> moveMTRDTOList)
        {
            List<string> lstErrors = new List<string>();
            try
            {
                foreach (MoveMaterialDTO moveMTRDTO in moveMTRDTOList)
                {
                    if (moveMTRDTO.MoveType == (int)MoveType.InvToStag || moveMTRDTO.MoveType == (int)MoveType.StagToStag)
                    {
                        if (string.IsNullOrWhiteSpace(moveMTRDTO.DestinationLocation))
                        {
                            moveMTRDTO.DestinationLocation = "[|EmptyStagingBin|]";
                        }
                    }
                }

                if (ValidateMoveLotSr(moveMTRDTOList, lstErrors))
                {
                    foreach (MoveMaterialDTO moveMTRDTO in moveMTRDTOList)
                    {
                        moveMTRDTO.CreatedBy = SessionHelper.UserID;
                        moveMTRDTO.UpdatedBy = SessionHelper.UserID;
                        moveMTRDTO.RoomID = RoomID;
                        moveMTRDTO.CompanyID = CompanyID;
                        moveMTRDTO.UpdatedOn = DateTimeUtility.DateTimeNow;
                        moveMTRDTO.CreatedOn = DateTimeUtility.DateTimeNow;

                        string[] stringSeparators = new string[] { };
                        if (!string.IsNullOrEmpty(moveMTRDTO.DestinationLocation) && moveMTRDTO.DestinationLocation.Contains("[||]"))
                        {
                            stringSeparators = moveMTRDTO.DestinationLocation.Split(new string[1] { "[||]" }, StringSplitOptions.RemoveEmptyEntries);
                            if (stringSeparators != null && stringSeparators.Count() > 0)
                                moveMTRDTO.DestinationLocation = stringSeparators[1];
                        }

                        if (!string.IsNullOrEmpty(moveMTRDTO.SourceLocation) && moveMTRDTO.SourceLocation.Contains("[||]"))
                        {
                            stringSeparators = new string[] { };
                            stringSeparators = moveMTRDTO.SourceLocation.Split(new string[1] { "[||]" }, StringSplitOptions.RemoveEmptyEntries);
                            if (stringSeparators != null && stringSeparators.Count() > 0)
                                moveMTRDTO.SourceLocation = stringSeparators[1];
                        }

                        MoveMaterialDAL objMoveDAL = new MoveMaterialDAL(SessionHelper.EnterPriseDBName);
                        long SessionUserId = SessionHelper.UserID;
                        bool returnStatus = objMoveDAL.MoveInventory(moveMTRDTO, SessionUserId,SessionHelper.RoomDateFormat,SessionHelper.EnterPriceID);
                    }

                    return Json(new { Message = "ok", Status = true, ErrorList = lstErrors }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Message = "Errors", Status = false, ErrorList = lstErrors }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                lstErrors.Add(ex.Message.ToString());
                return Json(new { Message = "Errors", Status = false, ErrorList = lstErrors }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// ValidateMove
        /// </summary>
        /// <param name="objDTO"></param>
        /// <param name="lstErrorsList"></param>
        /// <returns></returns>
        public bool ValidateMoveLotSr(List<MoveMaterialDTO> moveMTRDTOList, List<string> lstErrorsList)
        {
            bool isValid = true;

            foreach (MoveMaterialDTO moveMTRDTO in moveMTRDTOList)
            {
                if (moveMTRDTO.MoveType == (int)MoveType.InvToStag || moveMTRDTO.MoveType == (int)MoveType.StagToStag)
                {
                    if (string.IsNullOrEmpty(moveMTRDTO.DestinationStagingHeader))
                    {
                        lstErrorsList.Add(ResMoveMaterial.ErrorMsgDestinationStagingHeader);
                        isValid = false;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(moveMTRDTO.DestinationLocation))
                {
                    lstErrorsList.Add(ResMoveMaterial.ErrorMsgDestinationLocation);
                    isValid = false;
                    break;
                }

                string strLotSerialNumber = string.Empty;
                if (moveMTRDTO.SerialNumberTracking)
                    strLotSerialNumber = moveMTRDTO.SerialNumber;
                else if (moveMTRDTO.LotNumberTracking)
                    strLotSerialNumber = moveMTRDTO.LotNumber;

                if ((moveMTRDTO.SerialNumberTracking || moveMTRDTO.LotNumberTracking) && string.IsNullOrWhiteSpace(strLotSerialNumber))
                {
                    lstErrorsList.Add(moveMTRDTO.SerialNumberTracking ? ": " + ResKitMaster.SerialNumberCantBeEmpty : " " + ResKitMaster.LotNumberCantBeEmpty);
                    isValid = false;
                }

                if (moveMTRDTO.SourceBinID <= 0)
                {
                    lstErrorsList.Add(strLotSerialNumber + ": " + ResMoveMaterial.ErrorMsgSourceLocation);
                    isValid = false;
                }

                if (moveMTRDTO.MoveQuanity <= 0)
                {
                    lstErrorsList.Add(strLotSerialNumber + ": " + ResMoveMaterial.ErrorMsgMoveQty);
                    isValid = false;
                }

                if (moveMTRDTO.MoveType == (int)MoveType.InvToInv && moveMTRDTO.SourceBinID > 0 && moveMTRDTO.SourceBinID == moveMTRDTO.DestBinID)
                {
                    lstErrorsList.Add(strLotSerialNumber + ": " + ResMoveMaterial.ErrorMsgSourceAndDestinationAreSame);
                    isValid = false;
                }

                if (moveMTRDTO.MoveType == (int)MoveType.StagToStag
                && moveMTRDTO.SourceStagingHeaderGuid.GetValueOrDefault(Guid.Empty) == moveMTRDTO.DestStagingHeaderGuid.GetValueOrDefault(Guid.Empty)
                && moveMTRDTO.SourceBinID == moveMTRDTO.DestBinID)
                {
                    lstErrorsList.Add(strLotSerialNumber + ": " + ResMoveMaterial.ErrorMsgSourceAndDestinationAreSame);
                    isValid = false;
                }

                double oveTotalSourceQty = 0;
                if (moveMTRDTO.MoveQuanity > 0)
                {
                    if (moveMTRDTO.MoveType == (int)MoveType.InvToInv || moveMTRDTO.MoveType == (int)MoveType.InvToStag)
                    {
                        #region "LIFO FIFO"
                        RoomDAL objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
                        RoomDTO objRoomDTO = new RoomDTO();

                        //  objRoomDTO = objRoomDAL.GetRoomByIDPlain(RoomID);
                        CommonDAL objDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                        string columnList = "ID,RoomName,InventoryConsuptionMethod";
                        objRoomDTO = objDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");
                        //Boolean IsFIFO = false;
                        //if (objRoomDTO != null && objRoomDTO.ID > 0)
                        //{
                        //    if (!string.IsNullOrEmpty(objRoomDTO.InventoryConsuptionMethod) && objRoomDTO.InventoryConsuptionMethod.ToLower() == "fifo")
                        //        IsFIFO = true;
                        //    if (!string.IsNullOrEmpty(objRoomDTO.InventoryConsuptionMethod) && objRoomDTO.InventoryConsuptionMethod.ToLower() == "lifo")
                        //        IsFIFO = false;
                        //}
                        //else
                        //{
                        //    IsFIFO = true;
                        //}
                        #endregion

                        ItemLocationDetailsDAL objItmLocDtlsDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                        IEnumerable<ItemLocationDetailsDTO> SourceItemLocations = objItmLocDtlsDAL.GetCustomerFirstThenConsigedByLIFOFIFOForLotSr(false, moveMTRDTO.SourceBinID, RoomID, CompanyID, moveMTRDTO.ItemGUID, null, moveMTRDTO.LotNumber, moveMTRDTO.SerialNumber);
                        if (SourceItemLocations != null && SourceItemLocations.Count() > 0)
                        {
                            oveTotalSourceQty = SourceItemLocations.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0) + x.CustomerOwnedQuantity.GetValueOrDefault(0));
                        }
                        objItmLocDtlsDAL = null;
                        SourceItemLocations = null;

                        if (moveMTRDTO.MoveType == (int)MoveType.InvToStag)
                        {
                            if ((moveMTRDTO.SerialNumberTracking) && !string.IsNullOrWhiteSpace(strLotSerialNumber))
                            {
                                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                                string IsDestinationValid = objCommonDAL.CheckDuplicateMSSerialNumbers(moveMTRDTO.SerialNumber, RoomID, CompanyID, moveMTRDTO.ItemGUID);
                                if (IsDestinationValid == "duplicate")
                                {
                                    lstErrorsList.Add(string.Format(ResMoveMaterial.SerialAvailableOnStaging, strLotSerialNumber)); 
                                    isValid = false;
                                }
                            }
                        }

                    }
                    else if (moveMTRDTO.MoveType == (int)MoveType.StagToInv || moveMTRDTO.MoveType == (int)MoveType.StagToStag)
                    {
                        MaterialStagingPullDetailDAL objMSPullDtlDAL = new MaterialStagingPullDetailDAL(SessionHelper.EnterPriseDBName);
                        IEnumerable<MaterialStagingPullDetailDTO> SourceItemLocations = objMSPullDtlDAL.GetMsPullDetailsByItemGUIDANDBinIDForLotSr(moveMTRDTO.ItemGUID, moveMTRDTO.SourceBinID, RoomID, CompanyID, moveMTRDTO.LotNumber, moveMTRDTO.SerialNumber).Where(x => x.MaterialStagingGUID.GetValueOrDefault(Guid.Empty) == moveMTRDTO.SourceStagingHeaderGuid);
                        if (SourceItemLocations != null && SourceItemLocations.Count() > 0)
                        {
                            oveTotalSourceQty = SourceItemLocations.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0) + x.CustomerOwnedQuantity.GetValueOrDefault(0));
                        }
                        objMSPullDtlDAL = null;
                        SourceItemLocations = null;

                        if (moveMTRDTO.MoveType == (int)MoveType.StagToInv)
                        {
                            if ((moveMTRDTO.SerialNumberTracking) && !string.IsNullOrWhiteSpace(strLotSerialNumber))
                            {
                                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                                string IsDestinationValid = objCommonDAL.CheckDuplicateSerialNumbersForMove(moveMTRDTO.SerialNumber, Guid.Empty, RoomID, CompanyID, moveMTRDTO.ItemGUID);
                                if (IsDestinationValid == "duplicate")
                                {
                                    lstErrorsList.Add(string.Format(ResMoveMaterial.SerialAvailable, strLotSerialNumber));
                                    isValid = false;
                                }
                            }
                        }
                    }

                    if (oveTotalSourceQty < moveMTRDTO.MoveQuanity)
                    {
                        lstErrorsList.Add(strLotSerialNumber + ": " + ResMoveMaterial.ErrorMsgNotSuffucientQtyAtSource);
                        isValid = false;
                    }
                }
            }

            return isValid;
        }

    }
}
