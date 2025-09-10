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
using System.Web.Script.Serialization;

namespace eTurnsWeb.Controllers
{
    [AuthorizeHelper]
    public class KitController : eTurnsControllerBase
    {
        UDFController objUDFDAL = new UDFController();
        bool isInsert = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Kits, eTurnsWeb.Helper.SessionHelper.PermissionType.Insert);
        bool isUpdate = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Kits, eTurnsWeb.Helper.SessionHelper.PermissionType.Update);

        Int64 RoomID = SessionHelper.RoomID;
        Int64 CompanyID = SessionHelper.CompanyID;
        List<long> UserSupplierIds = SessionHelper.UserSupplierIds;
        Int64 UserID = SessionHelper.UserID;


        /// <summary>
        /// GetReorderTypeOrKitCategory
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        private List<ReorderTypeAndKitCategory> GetReorderTypeOrKitCategory(int Type)
        {
            List<ReorderTypeAndKitCategory> lst = new List<ReorderTypeAndKitCategory>();
            if (Type == 0)
            {
                lst.Add(new ReorderTypeAndKitCategory() { ReOrderType = "Re-Order", typeValue = true });
                lst.Add(new ReorderTypeAndKitCategory() { ReOrderType = "Transfer", typeValue = false });
            }
            else
            {
                lst.Add(new ReorderTypeAndKitCategory() { KitCategory = "WIP", CategoryValue = 0 });
                lst.Add(new ReorderTypeAndKitCategory() { KitCategory = "Direct", CategoryValue = 1 });
            }
            return lst;

        }

        /// <summary>
        /// KitList
        /// </summary>
        /// <returns></returns>
        public ActionResult KitList()
        {
            Session["ToolBinReplanish"] = null;
            return View();
        }

        /// <summary>
        /// BlankSession
        /// </summary>
        public JsonResult BlankSession()
        {
            Session["IsInsert"] = "";
            return Json(new { Message = "ok", Status = "ok" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// KitMasterListAjax
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public JsonResult KitMasterListAjax(JQueryDataTableParamModel param)
        {
            Session["ToolBinReplanish"] = null;
            int PageIndex = int.Parse(param.sEcho);
            int PageSize = param.iDisplayLength;
            int TotalRecordCount = 0;
            string sortColumnName = Request["SortingField"].ToString();
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "ID desc";
            }
            else
                sortColumnName = "ID desc";

            KitMasterDAL controller = new KitMasterDAL(SessionHelper.EnterPriseDBName);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            IEnumerable<KitMasterDTO> DataFromDB = controller.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, RoomID, CompanyID, IsArchived, IsDeleted, UserID, UserSupplierIds, SessionHelper.RoomDateFormat, CurrentTimeZone);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// _KitCreate
        /// </summary>
        /// <returns></returns>
        public PartialViewResult _KitCreate()
        {
            return PartialView();
        }

        /// <summary>
        /// CreateKit
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateKit()
        {
            string NewNumber = string.Empty;// new AutoSequenceDAL(SessionHelper.EnterPriseDBName).GetLastGeneratedROOMID("NextKitNo", SessionHelper.RoomID, SessionHelper.CompanyID).ToString();

            KitMasterDTO objDTO = new KitMasterDTO()
            {
                KitCategory = 0,
                ReOrderType = true,
                //KitPartNumber = "#K" + NewNumber,
                KitPartNumber = NewNumber,
                Created = DateTimeUtility.DateTimeNow,
                Updated = DateTimeUtility.DateTimeNow,
                CreatedBy = SessionHelper.UserID,
                CreatedByName = SessionHelper.UserName,
                LastUpdatedBy = SessionHelper.UserID,
                Room = SessionHelper.RoomID,
                CompanyID = SessionHelper.CompanyID,
                RoomName = SessionHelper.RoomName,
                UpdatedByName = SessionHelper.UserName,
                IsKitBuildAction = false
            };
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("KitMaster");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            Session["ToolBinReplanish"] = null;
            ViewBag.ReOrderTypeList = GetReorderTypeOrKitCategory(0);
            ViewBag.KitCategoryList = GetReorderTypeOrKitCategory(1);
            return PartialView("_KitCreate", objDTO);
        }

        /// <summary>
        ///  GET: /Kit/ for Edit
        /// </summary>
        /// <returns></returns>
        public ActionResult KitEdit(Guid KitGUID)
        {
            KitMasterDAL obj = new KitMasterDAL(SessionHelper.EnterPriseDBName);
            KitMasterDTO objDTO = obj.GetKitByGuidFull(KitGUID);
            if (objDTO == null)
            {
                objDTO = new KitMasterDTO();
            }
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("KitMaster");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }

            //ViewBag.ReOrderTypeList = GetReorderTypeOrKitCategory(0);
            ViewBag.KitCategoryList = GetReorderTypeOrKitCategory(1);
            KitDetailDAL kitDtlDAL = new KitDetailDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<KitDetailDTO> objKits = kitDtlDAL.GetAllRecordsByKitGUID(KitGUID, RoomID, CompanyID, false, false, false);
            objDTO.NoOfItemsInKit = objKits.Where(x => x.QuantityReadyForAssembly.GetValueOrDefault(0) > 0).Count();
            objDTO.WIPKitCost = objKits.Where(x => x.AvailableItemsInWIP.GetValueOrDefault(0) > 0).Sum(x => x.AvailableItemsInWIP.GetValueOrDefault(0) * x.Cost.GetValueOrDefault(0));

            LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(SessionHelper.EnterPriseDBName);
            List<LocationMasterDTO> lstLocation = objLocationCntrl.GetLocationByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            lstLocation = lstLocation.Where(t => (!string.IsNullOrWhiteSpace(t.Location))).ToList();
            lstLocation.Insert(0, new LocationMasterDTO() { ID = 0, Location = ResCommon.MsgSelectLocation });
            ViewBag.LocationList = lstLocation;

            ToolLocationDetailsDAL objToolLocationDetailDAL = new ToolLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            List<ToolLocationDetailsDTO> lstToolLocationDetailsDTO = objToolLocationDetailDAL.GetToolLocationsByToolGUID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, objDTO.GUID).ToList();

            List<LocationMasterDTO> lstAllLocation = new List<LocationMasterDTO>(lstLocation.ToList());
            List<Guid> objGUIDList = lstToolLocationDetailsDTO.Select(u => u.LocationGUID).ToList();
            lstAllLocation = lstAllLocation.Where(l => objGUIDList.Contains(l.GUID)).ToList();

            Session["ToolBinReplanish"] = lstAllLocation;
            List<BinMasterDTO> lstBins = new List<BinMasterDTO>();
            BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            lstBins = objBinMasterDAL.GetItemLocationsForKitBreak(KitGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
            objDTO.KitBins = lstBins;
            return PartialView("_KitCreate", objDTO);
        }

        /// <summary>
        /// DeleteKitMasterRecords
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public string DeleteKitMasterRecords(string GUIDs)
        {
            try
            {
                KitMasterDAL obj = new KitMasterDAL(SessionHelper.EnterPriseDBName);
                obj.DeleteRecords(GUIDs, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID);
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// JSON Record Save - Enter key Save/Update
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Technician"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveKitHeader(KitMasterDTO objDTO)
        {

            //string message = "";
            //string status = "fail";
            ////KitMasterController obj = new KitMasterController();
            //KitMasterDAL obj = new KitMasterDAL(SessionHelper.EnterPriseDBName);
            //CommonDAL objCDal = new CommonDAL(SessionHelper.EnterPriseDBName);

            //if (string.IsNullOrEmpty(objDTO.KitPartNumber))
            //{
            //    message = string.Format(ResMessage.Required, ResKitMaster.KitPartNumber);
            //    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            //}
            ////else if (objDTO.MaximumKitQuentity != null)
            ////{
            ////    if (objDTO.MaximumKitQuentity == null)
            ////    {
            ////        message = "Please enter minimum kit quantity.";
            ////        return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            ////    }
            ////    if (objDTO.MinimumKitQuentity.GetValueOrDefault(0) >= objDTO.MaximumKitQuentity.GetValueOrDefault(0))
            ////    {
            ////        message = "Minimum kit quantity is less than maximum kit quantity.";
            ////        return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            ////    }
            ////}

            //objDTO.LastUpdatedBy = SessionHelper.UserID;
            //objDTO.UpdatedByName = SessionHelper.UserName;
            //objDTO.Room = SessionHelper.RoomID;
            //objDTO.CompanyID = SessionHelper.CompanyID;

            //if (objDTO.ID == 0)
            //{
            //    string strOK = objCDal.DuplicateCheck(objDTO.KitPartNumber, "add", objDTO.ID, "KitMaster", "KitPartNumber", SessionHelper.RoomID, SessionHelper.CompanyID);
            //    if (strOK == "duplicate")
            //    {
            //        message = string.Format(ResMessage.DuplicateMessage, ResKitMaster.KitPartNumber, objDTO.KitPartNumber);
            //        status = "duplicate";
            //    }
            //    else
            //    {
            //        objDTO.GUID = Guid.NewGuid();
            //        if (objDTO.MaximumKitQuentity.GetValueOrDefault(0) > 0)
            //            objDTO.KitDemand = objDTO.MaximumKitQuentity;
            //        else if (objDTO.MinimumKitQuentity.GetValueOrDefault(0) > 0)
            //            objDTO.KitDemand = objDTO.MinimumKitQuentity;
            //        else
            //            objDTO.KitDemand = 1;

            //        long ReturnVal = obj.Insert(objDTO);
            //        if (ReturnVal > 0)
            //        {
            //            message = ResMessage.SaveMessage;
            //            status = "ok";
            //        }
            //        else
            //        {
            //            message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);
            //        }
            //    }
            //}
            //else
            //{
            //    string strOK = objCDal.DuplicateCheck(objDTO.KitPartNumber, "edit", objDTO.ID, "KitMaster", "KitPartNumber", SessionHelper.RoomID, SessionHelper.CompanyID);
            //    if (strOK == "duplicate")
            //    {
            //        message = string.Format(ResMessage.DuplicateMessage, ResOrder.OrderNumber, objDTO.KitPartNumber);
            //        status = "duplicate";
            //    }
            //    else
            //    {
            //        if (objDTO.NoOfItemsInKit <= 0)
            //        {
            //            message = "Kit must have minimum one line item.";
            //            status = "fail";
            //        }
            //        else
            //        {
            //            objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
            //            objDTO.EditedFrom = "Web";
            //            bool ReturnVal = obj.Edit(objDTO);
            //            if (ReturnVal)
            //            {
            //                message = ResMessage.SaveMessage;  //ResMessage.SaveMessage; //"Record Saved Sucessfully...";
            //                status = "ok";
            //            }
            //            else
            //            {
            //                message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); //string.Format(ResMessage.SaveErrorMsg, hrmResult.StatusCode); // "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
            //            }
            //        }
            //    }
            //}
            //Session["IsInsert"] = "True";
            return Json(new { Message = "", Status = false, UpdatedDTO = objDTO }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// LoadOrderLineItems
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public ActionResult LoadKitLineItemsByKitMasterDTO(Guid KitGUID)
        {
            KitMasterDAL objKitMstCtrl = new KitMasterDAL(SessionHelper.EnterPriseDBName);
            KitMasterDTO KitDTO = objKitMstCtrl.GetKitByGuidFull(KitGUID);
            if (KitDTO == null)
            {
                KitDTO = new KitMasterDTO();
            }
            KitDTO.KitItemList = new KitDetailDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByKitGUID(KitGUID, SessionHelper.RoomID, SessionHelper.CompanyID, KitDTO.IsArchived.GetValueOrDefault(false), KitDTO.IsDeleted.GetValueOrDefault(false), true).ToList();
            return PartialView("_KitLineItems", KitDTO);
        }

        /// <summary>
        /// LoadItemMasterModel
        /// </summary>
        /// <param name="ParentId"></param>
        /// <returns></returns>
        public ActionResult LoadItemMasterModel(KitMasterDTO KitDTO)
        {
            ItemModelPerameter obj = new ItemModelPerameter()
            {
                AjaxURLAddMultipleItemToSession = "~/Kit/AddItemToDetailTable/",
                AjaxURLToFillItemGrid = "~/Kit/GetItemsModelMethod/",
                CallingFromPageName = "KIT",
                AjaxURLAddItemToSession = "~/Kit/AddItemToDetailTable/",
                ModelHeader = eTurns.DTO.ResKitMaster.PageHeader,
                PerentID = KitDTO.ID.ToString(),
                PerentGUID = KitDTO.GUID.ToString(),
            };

            return PartialView("ItemMasterModel", obj);
        }

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult GetItemsModelMethod(JQueryDataTableParamModel param)
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
            //if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID";

            //make changes to resolve an issue of Sort (WI-431)
            if (sortColumnName == "0" || sortColumnName.Contains("undefined"))
                sortColumnName = "ItemNumber Asc";

            string searchQuery = string.Empty;
            int TotalRecordCount = 0;
            string ItemsGUIDs = string.Empty;
            Guid KitGUID = Guid.Empty; ;
            Guid.TryParse(Request["ParentGUID"], out KitGUID);
            IEnumerable<KitDetailDTO> lstItems = new KitDetailDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByKitGUID(KitGUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false);

            if (lstItems != null && lstItems.Count() > 0)
                ItemsGUIDs = string.Join(",", lstItems.Select(x => x.ItemGUID.ToString()).ToArray());

            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            var tmpsupplierIds = new List<long>();
            // .Where(x=>x.ItemType != 4); , as Labour Type item not required in this module
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetPagedRecordsForModel(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, ItemsGUIDs, "labor", tmpsupplierIds, RoomDateFormat, CurrentTimeZone);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// AddDetailItem
        /// </summary>
        /// <param name="para"></param>
        /// <param name="ItemID"></param>
        /// <param name="ItemGUID"></param>
        /// <param name="pQuentity"></param>
        /// <param name="QuickListID"></param>
        /// <param name="QuickListGuid"></param>
        /// <returns></returns>
        ///public JsonResult AddDetailItem(string para, Int64 ItemID, string ItemGUID, double pQuentity, Int64 QuickListID, string QuickListGuid)
        public JsonResult AddItemToDetailTable(string para)
        {
            string message = "";
            string status = "";
            try
            {
                long SessionUserId = SessionHelper.UserID;
                JavaScriptSerializer s = new JavaScriptSerializer();
                KitDetailDTO[] QLDetails = s.Deserialize<KitDetailDTO[]>(para);
                //KitDetailController objApi = new KitDetailController();
                KitDetailDAL objApi = new KitDetailDAL(SessionHelper.EnterPriseDBName);
                var enterpriseId = SessionHelper.EnterPriceID;

                foreach (KitDetailDTO item in QLDetails)
                {
                    item.Room = SessionHelper.RoomID;
                    item.RoomName = SessionHelper.RoomName;
                    item.CreatedBy = SessionHelper.UserID;
                    item.CreatedByName = SessionHelper.UserName;
                    item.UpdatedByName = SessionHelper.UserName;
                    item.LastUpdatedBy = SessionHelper.UserID;
                    item.CompanyID = SessionHelper.CompanyID;
                    if (item.ID > 0)
                    {
                        item.ReceivedOn = DateTimeUtility.DateTimeNow;
                        item.EditedFrom = "Web";
                        objApi.Edit(item, SessionUserId, enterpriseId);
                    }
                    else
                    {
                        item.QuantityReadyForAssembly = 0;
                        item.AvailableItemsInWIP = 0;
                        //List<KitDetailDTO> tempDTO = objApi.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false).Where(x => x.KitGUID == item.KitGUID && x.ItemGUID == item.ItemGUID).ToList();
                        List<KitDetailDTO> tempDTO = objApi.GetKitDetail(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false, Convert.ToString(item.KitGUID), Convert.ToString(item.ItemGUID)).ToList();
                        if (tempDTO == null || tempDTO.Count == 0)
                            objApi.Insert(item, SessionUserId, SessionHelper.EnterPriceID);
                    }
                }

                message = ResCommon.RecordsSavedSuccessfully;
                status = "ok";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                message = "Error";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                //throw;
            }
        }

        /// <summary>
        /// DeleteOrderLineItem
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public string KitLineItemsDelete(string Ids)
        {
            try
            {
                //KitDetailController kitDetailCtrl = new KitDetailController();
                KitDetailDAL kitDetailCtrl = new KitDetailDAL(SessionHelper.EnterPriseDBName);
                kitDetailCtrl.DeleteRecords(Ids, SessionHelper.UserID, SessionHelper.CompanyID);

                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public JsonResult UpdateKitCost(KitMasterDTO KitDTO)
        {
            try
            {
                string message = ResMessage.SaveMessage;
                string status = "ok";
                KitMasterDAL objKitMstCtrl = new KitMasterDAL(SessionHelper.EnterPriseDBName);
                KitDTO = objKitMstCtrl.GetRecord(KitDTO.GUID.ToString());

                if (KitDTO.IsDeleted.GetValueOrDefault(false) || KitDTO.IsArchived.GetValueOrDefault(false) || (!isUpdate && Convert.ToString(Session["IsInsert"]) != "True"))
                {
                    return Json(new { Message = message, Status = status, KitDTO = KitDTO }, JsonRequestBehavior.AllowGet);
                }

                KitDetailDAL kitDtlDAL = new KitDetailDAL(SessionHelper.EnterPriseDBName);
                var objKits = kitDtlDAL.GetKitDetailByKitGUIDNormal(KitDTO.GUID, RoomID, CompanyID);
                KitDTO.NoOfItemsInKit = objKits.Where(x => x.QuantityReadyForAssembly.GetValueOrDefault(0) > 0).Count();
                KitDTO.WIPKitCost = objKits.Where(x => x.AvailableItemsInWIP.GetValueOrDefault(0) > 0).Sum(x => x.AvailableItemsInWIP.GetValueOrDefault(0) * x.Cost.GetValueOrDefault(0));
                return Json(new { Message = message, Status = status, KitDTO = KitDTO }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new { Message = "fail", Status = "fail", KitDTO = new KitMasterDTO() }, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// MoveInMoveOutQty
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="DefaultLocation"></param>
        /// <param name="isMoveIn"></param>
        /// <returns></returns>
        public ActionResult ShowMoveInQtyModel(string KitDetailGUID)
        {
            KitDetailDTO objKitDetailDTO = new KitDetailDAL(SessionHelper.EnterPriseDBName).GetRecord(KitDetailGUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, true);
            CommonDAL objCommonCTRL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<CommonDTO> binDTO = objCommonCTRL.GetLocationListWithQuntity(objKitDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
            KitMasterDAL objKitDAL = new KitMasterDAL(SessionHelper.EnterPriseDBName);
            KitMasterDTO objKitDTO = objKitDAL.GetRecord(objKitDetailDTO.KitGUID.GetValueOrDefault(Guid.Empty).ToString());

            ViewBag.MoveInLocation = binDTO;
            double qtyToMeetDemand = 0;
            qtyToMeetDemand = (objKitDetailDTO.QuantityPerKit.GetValueOrDefault(0) * objKitDTO.KitDemand.GetValueOrDefault(0)) - objKitDetailDTO.AvailableItemsInWIP.GetValueOrDefault(0);
            if (qtyToMeetDemand <= 0)
                qtyToMeetDemand = 0;

            ViewBag.QtyToMeetDemand = qtyToMeetDemand.ToString("N" + SessionHelper.NumberDecimalDigits);

            if (objKitDetailDTO.ItemDetail.ItemNumber.Length > 50)
                objKitDetailDTO.ItemDetail.ItemNumber = objKitDetailDTO.ItemDetail.ItemNumber.Substring(0, 47) + "...";
            return PartialView("_MoveInQtyModel", objKitDetailDTO);

        }

        /// <summary>
        /// MoveInMoveOutQty
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="DefaultLocation"></param>
        /// <param name="isMoveIn"></param>
        /// <returns></returns>
        public ActionResult ShowMoveOutQtyModel(string KitDetailGUID)
        {
            KitDetailDTO kitDetailDTO = new KitDetailDAL(SessionHelper.EnterPriseDBName).GetRecord(KitDetailGUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false);

            Guid ItemGUID = kitDetailDTO.ItemGUID.GetValueOrDefault(Guid.Empty);
            ViewBag.ForCreditPull = "ForKitCredit";
            ViewBag.KitDetailID = kitDetailDTO.ID;
            ViewBag.KitDetailGUID = kitDetailDTO.GUID;
            ViewBag.ItemGUID = ItemGUID;
            ViewBag.IsPullCredit = "True";
            BinMasterDAL objCommon = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<BinMasterDTO> lstBinDTO = objCommon.GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Guid.Empty, 0, null, false);//.Where(x => x.IsStagingLocation == false);
            ViewBag.BinLocations = lstBinDTO;
            ItemMasterDAL objItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            var Objitem = objItemDAL.GetItemWithMasterTableJoins(null, ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
            ViewBag.ItemGUID_ItemType = ItemGUID.ToString() + "#" + Objitem.ItemType;
            List<ItemLocationDetailsDTO> lstData = new List<ItemLocationDetailsDTO>();
            ViewBag.AvailableInWIP = kitDetailDTO.AvailableItemsInWIP.GetValueOrDefault(0).ToString("N" + SessionHelper.NumberDecimalDigits);
            double Count = 0;
            Count = kitDetailDTO.AvailableItemsInWIP.GetValueOrDefault(0);

            if (Objitem != null)
            {
                if (Objitem.SerialNumberTracking == false && Objitem.LotNumberTracking == false && Objitem.DateCodeTracking == false)
                {
                    Count = 1;
                }
            }
            //Add empty Rows to the list
            for (int i = 0; i < Count; i++)
            {
                ItemLocationDetailsDTO objEmplty = new ItemLocationDetailsDTO();
                //objEmplty.ID = i;
                objEmplty.ItemNumber = Objitem.ItemNumber;
                objEmplty.Cost = Objitem.Cost;
                objEmplty.Created = DateTimeUtility.DateTimeNow;
                objEmplty.ItemGUID = Objitem.GUID;
                objEmplty.SerialNumberTracking = Objitem.SerialNumberTracking;

                if (Objitem.SerialNumberTracking)
                {
                    if (Objitem.Consignment)
                    {
                        objEmplty.ConsignedQuantity = 1;
                    }
                    else
                    {
                        objEmplty.CustomerOwnedQuantity = 1;
                    }
                }

                objEmplty.LotNumberTracking = Objitem.LotNumberTracking;

                objEmplty.IsCreditPull = true;
                //objEmplty.KitDetailID = kitDetailDTO.ID;
                objEmplty.KitDetailGUID = Guid.Parse(KitDetailGUID);
                if (!string.IsNullOrEmpty(Objitem.DefaultLocationName))
                {
                    objEmplty.BinNumber = Objitem.DefaultLocationName;
                }
                else if (lstBinDTO != null && lstBinDTO.Count() > 0)
                {
                    objEmplty.BinNumber = lstBinDTO.OrderBy("BinNumber asc").FirstOrDefault().BinNumber;
                }

                lstData.Add(objEmplty);
            }

            //Set default SerialNumberTracking OR LotNumberTracking - for all records... assuming it is can't be edited after one location add
            lstData = lstData.Select(c => { if (Objitem.SerialNumberTracking) { c.SerialNumberTracking = true; } if (Objitem.LotNumberTracking) { c.LotNumberTracking = true; } return c; }).ToList();

            ViewBag.IsSerialNumberTracking = Objitem.SerialNumberTracking;
            ViewBag.IsLotNumberTracking = Objitem.LotNumberTracking;
            ViewBag.IsDateCodeTracking = Objitem.DateCodeTracking;
            ViewBag.ItemNumber = Objitem.ItemNumber;
            ViewBag.Consignment = Objitem.Consignment;

            return PartialView("../Inventory/_LocationDetails", lstData);
        }

        /// <summary>
        /// CheckLocationWiseQuantityForMoveIn
        /// </summary>
        /// <param name="LocationID"></param>
        /// <param name="ItemID"></param>
        /// <param name="MoviInQty"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CheckMoveInQuantity(Int64 BinID, string ItemGUID, double Qty)
        {
            string message = "";
            string status = "ok";
            try
            {
                //CommonController objCommonCtrl = new CommonController();
                CommonDAL objCommonCtrl = new CommonDAL(SessionHelper.EnterPriseDBName);
                ResponseMessage ResponseMsg = objCommonCtrl.CheckQuantityByLocation(BinID, Guid.Parse(ItemGUID), Qty, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID, SessionHelper.UserID);

                if (ResponseMsg.IsSuccess)
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { Message = ResponseMsg.Message, Status = "fail" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, Status = "fail", KitDTO = new KitMasterDTO() }, JsonRequestBehavior.AllowGet);
            }


        }

        /// <summary>
        /// AddItemQuantityInItemWIP
        /// </summary>
        /// <param name="objMoveQty"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult MoveQuantityInItemWIP(MoveInOutQtyDetail objMoveQty)
        {
            try
            {
                string status = "fail";
                //KitMoveInOutDetailController kitMoveInOutCtrl = new KitMoveInOutDetailController();
                KitMoveInOutDetailDAL kitMoveInOutCtrl = new KitMoveInOutDetailDAL(SessionHelper.EnterPriseDBName);
                long SessionUserId = SessionHelper.UserID;
                ResponseMessage RespMsg = kitMoveInOutCtrl.DecreaseQuantity(objMoveQty, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionUserId, SessionHelper.EnterPriceID, ResourceHelper.CurrentCult.Name);
                if (RespMsg.IsSuccess)
                    status = "ok";

                return Json(new { Message = RespMsg.Message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// BuildKit
        /// </summary>
        /// <param name="objKit"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult BuildKit(KitMasterDTO objKit)
        {
            try
            {
                long SessionUserId = SessionHelper.UserID;
                var enterpriseId = SessionHelper.EnterPriceID;

                if (objKit.QuantityToBuildBreak.GetValueOrDefault(0) > 0)
                {
                    //KitDetailController objKitDetailctrl = new KitDetailController();
                    KitDetailDAL objKitDetailctrl = new KitDetailDAL(SessionHelper.EnterPriseDBName);

                    List<KitDetailDTO> lstKitDetailDTO = objKitDetailctrl.GetAllRecordsByKitGUID(objKit.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, true).ToList();
                    foreach (var item in lstKitDetailDTO)
                    {
                        if (item.ItemDetail.ItemType != 4)
                        {
                            item.AvailableItemsInWIP = item.AvailableItemsInWIP.GetValueOrDefault(0) - (item.QuantityPerKit * objKit.QuantityToBuildBreak);
                            item.LastUpdatedBy = SessionHelper.UserID;
                            item.ReceivedOn = DateTimeUtility.DateTimeNow;
                            item.EditedFrom = "Web";
                            objKitDetailctrl.Edit(item, SessionUserId, enterpriseId);
                        }
                    }
                    objKit.AvailableKitQuantity = objKit.AvailableKitQuantity.GetValueOrDefault(0) + objKit.QuantityToBuildBreak.GetValueOrDefault(0);
                    objKit.AvailableWIPKit = objKit.AvailableWIPKit.GetValueOrDefault(0) - objKit.QuantityToBuildBreak.GetValueOrDefault(0);
                    if (objKit.KitDemand.GetValueOrDefault(0) > 0)
                    {
                        objKit.KitDemand = objKit.KitDemand.GetValueOrDefault(0) - objKit.QuantityToBuildBreak.GetValueOrDefault(0);
                    }
                    objKit.LastUpdatedBy = SessionHelper.UserID;
                    //KitMasterController objKitMasterCtrl = new KitMasterController();
                    KitMasterDAL objKitMasterCtrl = new KitMasterDAL(SessionHelper.EnterPriseDBName);
                    objKitMasterCtrl.Edit(objKit);

                    return Json(new { Message = ResCommon.MsgUpdatedSuccessfully, Status = "ok" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { Message = ResKitMaster.EnterQtyToBuildKit, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { Message = ex.Message, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// BreakKit
        /// </summary>
        /// <param name="objKit"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult BreakKit(KitMasterDTO objKit)
        {
            try
            {
                long SessionUserId = SessionHelper.UserID;
                var enterpriseId = SessionHelper.EnterPriceID;

                if (objKit.QuantityToBuildBreak.GetValueOrDefault(0) > 0)
                {
                    //KitDetailController objKitDetailctrl = new KitDetailController();
                    KitDetailDAL objKitDetailctrl = new KitDetailDAL(SessionHelper.EnterPriseDBName);
                    List<KitDetailDTO> lstKitDetailDTO = objKitDetailctrl.GetAllRecordsByKitGUID(objKit.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, true).ToList();
                    foreach (var item in lstKitDetailDTO)
                    {
                        if (item.ItemDetail.ItemType != 4)
                        {
                            item.AvailableItemsInWIP = item.AvailableItemsInWIP.GetValueOrDefault(0) + (item.QuantityPerKit * objKit.QuantityToBuildBreak);
                            item.LastUpdatedBy = SessionHelper.UserID;
                            item.ReceivedOn = DateTimeUtility.DateTimeNow;
                            item.EditedFrom = "Web";
                            objKitDetailctrl.Edit(item, SessionUserId, enterpriseId);
                        }
                    }
                    objKit.AvailableKitQuantity = objKit.AvailableKitQuantity.GetValueOrDefault(0) - objKit.QuantityToBuildBreak.GetValueOrDefault(0);
                    objKit.AvailableWIPKit = objKit.AvailableWIPKit.GetValueOrDefault(0) + objKit.QuantityToBuildBreak.GetValueOrDefault(0);
                    if (objKit.KitDemand.GetValueOrDefault(0) > 0)
                    {
                        objKit.KitDemand = objKit.KitDemand.GetValueOrDefault(0) + objKit.QuantityToBuildBreak.GetValueOrDefault(0);
                    }
                    objKit.LastUpdatedBy = SessionHelper.UserID;
                    //KitMasterController objKitMasterCtrl = new KitMasterController();
                    KitMasterDAL objKitMasterCtrl = new KitMasterDAL(SessionHelper.EnterPriseDBName);
                    objKitMasterCtrl.Edit(objKit);

                    return Json(new { Message = ResCommon.MsgUpdatedSuccessfully, Status = "ok" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { Message = ResKitMaster.EnterQtyToBreakKit, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new { Message = ex.Message, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }


        #region *********************** Commented Code ***************************************



        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        //public ActionResult KitEdit(Int64 ID)
        //{
        //    ClearKitSession();
        //    KitMasterController obj = new KitMasterController();
        //    KitMasterDTO objDTO = obj.GetRecord(ID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
        //    objDTO.IsKitBuildAction = false;
        //    if (objDTO.KitItemList != null && objDTO.KitItemList.Count > 0)
        //    {
        //        ClearKitSession();
        //        SessionHelper.Add(KitSessionKey, objDTO.KitItemList);
        //    }

        //    ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("KitMaster");

        //    foreach (var i in ViewBag.UDFs)
        //    {
        //        string _UDFColumnName = (string)i.UDFColumnName;
        //        ViewData[_UDFColumnName] = i.UDFDefaultValue;
        //    }

        //    ViewBag.ReOrderTypeList = GetReorderTypeOrKitCategory(0);
        //    ViewBag.KitCategoryList = GetReorderTypeOrKitCategory(1);

        //    return PartialView("_KitCreate", objDTO);
        //}

        /// <summary>
        /// JSON Record Save - Enter key Save/Update
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Technician"></param>
        /// <returns></returns>
        ///[HttpPost]
        //public JsonResult SaveKit(KitMasterDTO objDTO)
        //{

        //    string message = "";
        //    string status = "";
        //    KitMasterController obj = new KitMasterController();

        //    if (string.IsNullOrEmpty(objDTO.KitPartNumber))
        //    {
        //        message = string.Format(ResMessage.Required, ResKitMaster.KitPartNumber);
        //        status = "fail";
        //        return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        //    }

        //    objDTO.LastUpdatedBy = SessionHelper.UserID;
        //    objDTO.Room = SessionHelper.RoomID;


        //    List<KitDetailDTO> objCached = ((List<KitDetailDTO>)SessionHelper.Get(KitSessionKey));
        //    if (objCached != null && objCached.Count > 0)
        //    {

        //        if (objDTO.IsKitBuildAction)
        //        {
        //            //double minQty = objCached.Min(x => x.QuantityReadyForAssembly.GetValueOrDefault(0));

        //            objCached.ForEach(yy => { yy.AvailableItemsInWIP -= yy.QuantityPerKit * objDTO.AvailableWIPKit; yy.QuantityReadyForAssembly = yy.AvailableItemsInWIP / yy.QuantityPerKit; });

        //            objDTO.AvailableKitQuentity += objDTO.AvailableWIPKit;
        //            objDTO.KitDemand = objDTO.AvailableWIPKit >= 5 ? 0 : objDTO.KitDemand - objDTO.AvailableWIPKit;
        //            objDTO.AvailableWIPKit = 0;
        //        }
        //        else if (objDTO.IsKitBreakAction)
        //        {
        //            objCached.ForEach(yy => { yy.AvailableItemsInWIP += yy.QuantityPerKit * objDTO.AvailableWIPKit; yy.QuantityReadyForAssembly = yy.AvailableItemsInWIP / yy.QuantityPerKit; });

        //            objDTO.AvailableWIPKit += objDTO.AvailableKitQuentity;
        //            objDTO.KitDemand = objDTO.KitDemand + objDTO.AvailableWIPKit >= 5 ? 0 : 5 - objDTO.KitDemand + objDTO.AvailableWIPKit;
        //            objDTO.AvailableKitQuentity = 0;
        //        }

        //        objDTO.KitItemList = objCached;
        //        SessionHelper.RomoveSessionByKey(KitSessionKey);
        //        SessionHelper.Add(KitSessionKey, objCached);


        //    }
        //    else
        //    {
        //        message = ResOrder.BlankItemSavedMessage;// "Please add items for save order";
        //        status = "fail";
        //        return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        //    }

        //    if (objDTO.ID == 0)
        //    {
        //        string strOK = obj.DuplicateCheck(objDTO.KitPartNumber, "add", objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID);
        //        if (strOK == "duplicate")
        //        {
        //            message = string.Format(ResMessage.DuplicateMessage, ResKitMaster.KitPartNumber, objDTO.KitPartNumber);
        //            status = "duplicate";
        //        }
        //        else
        //        {

        //            objDTO.GUID = Guid.NewGuid();
        //            HttpResponseMessage hrmResult = obj.PostRecord(objDTO);
        //            if (hrmResult.StatusCode == System.Net.HttpStatusCode.OK)
        //            {
        //                message = ResMessage.SaveMessage;
        //                status = "ok";
        //            }
        //            else
        //            {
        //                message = string.Format(ResMessage.SaveErrorMsg, hrmResult.StatusCode);
        //                status = "fail";
        //            }

        //            ClearKitSession();
        //        }
        //    }
        //    else
        //    {
        //        string strOK = obj.DuplicateCheck(objDTO.KitPartNumber, "edit", objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID);
        //        if (strOK == "duplicate")
        //        {
        //            message = string.Format(ResMessage.DuplicateMessage, ResOrder.OrderNumber, objDTO.KitPartNumber);
        //            status = "duplicate";
        //        }
        //        else
        //        {
        //            HttpResponseMessage hrmResult = obj.PutRecord(objDTO.ID, objDTO);


        //            string deletedIDs = (string)SessionHelper.Get(DeletedKitSessionKey);
        //            if (!string.IsNullOrEmpty(deletedIDs))
        //                new KitDetailController().DeleteRecords(deletedIDs, SessionHelper.UserID, SessionHelper.CompanyID);

        //            if (hrmResult.StatusCode == System.Net.HttpStatusCode.OK)
        //            {
        //                message = ResMessage.SaveMessage;  //ResMessage.SaveMessage; //"Record Saved Sucessfully...";
        //                status = "ok";
        //                JavaScriptSerializer s = new JavaScriptSerializer();
        //                if (objDTO.IsKitBuildAction)
        //                {
        //                    objDTO.IsKitBreakAction = false;
        //                    objDTO.IsKitBuildAction = false;
        //                    status = "build";

        //                }
        //                else if (objDTO.IsKitBreakAction)
        //                {
        //                    objDTO.IsKitBreakAction = false;
        //                    objDTO.IsKitBuildAction = false;
        //                    status = "break";

        //                }
        //            }
        //            else
        //            {
        //                message = string.Format(ResMessage.SaveErrorMsg, hrmResult.StatusCode); //string.Format(ResMessage.SaveErrorMsg, hrmResult.StatusCode); // "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
        //                status = "fail";
        //            }
        //        }
        //    }
        //    if (objCached != null && objCached.Count > 0)
        //    {
        //        foreach (var item in objCached)
        //        {
        //            List<PullItemsWithQuantity> lstPullItemsforKit = (List<PullItemsWithQuantity>)SessionHelper.Get(PullItemsForKitSessionKey + item.ItemID);
        //            if (lstPullItemsforKit != null && lstPullItemsforKit.Count > 0)
        //            {
        //                lstPullItemsforKit = new List<PullItemsWithQuantity>();
        //                SubmitPulls(lstPullItemsforKit.ToArray());
        //            }
        //        }
        //    }

        //    ClearKitSession();

        //    return Json(new { Message = message, Status = status, UpdatedDTO = objDTO }, JsonRequestBehavior.AllowGet);

        //}

        /// <summary>
        /// LoadItemMasterModel
        /// </summary>
        /// <param name="ParentId"></param>
        /// <returns></returns>
        //public ActionResult LoadItemMasterModel(string ParentId)
        //{
        //    ItemModelPerameter obj = new ItemModelPerameter()
        //    {
        //        AjaxURLAddItemToSession = "~/Kit/AddItemToSession/",
        //        PerentID = ParentId,
        //        ModelHeader = "Add Items for Kitting",
        //        AjaxURLAddMultipleItemToSession = "~/Kit/AddItemToSessionMultiple/",
        //        AjaxURLToFillItemGrid = "~/Kit/GetItemsModelMethod/"
        //    };

        //    return PartialView("ItemMasterModel", obj);
        //}

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        //public ActionResult GetItemsModelMethod(JQueryDataTableParamModel param)
        //{
        //    int PageIndex = int.Parse(param.sEcho);
        //    int PageSize = param.iDisplayLength;
        //    var sortDirection = Request["sSortDir_0"];
        //    var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
        //    var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
        //    var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
        //    var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
        //    string sortColumnName = string.Empty;
        //    string sDirection = string.Empty;
        //    int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
        //    sortColumnName = Request["SortingField"].ToString();

        //    bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
        //    bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());

        //    // set the default column sorting here, if first time then required to set 
        //    if (sortColumnName == "0" || sortColumnName == "undefined")
        //        sortColumnName = "ID";

        //    if (sortDirection == "asc")
        //        sortColumnName = sortColumnName + " asc";
        //    else
        //        sortColumnName = sortColumnName + " desc";

        //    string searchQuery = string.Empty;

        //    int TotalRecordCount = 0;

        //    string ItemsIDs = string.Empty;
        //    object objLineItems = SessionHelper.Get(KitSessionKey);
        //    List<KitDetailDTO> lstDto = null;

        //    if (objLineItems != null)
        //    {
        //        lstDto = (List<KitDetailDTO>)objLineItems;

        //        foreach (var item in lstDto)
        //        {
        //            if (!string.IsNullOrEmpty(ItemsIDs))
        //                ItemsIDs += ",";

        //            ItemsIDs += item.ItemID.ToString();
        //        }
        //    }

        //    ItemMasterController obj = new ItemMasterController();
        //    var DataFromDB = obj.GetPagedRecordsForModel(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, ItemsIDs);


        //    return Json(new
        //    {
        //        sEcho = param.sEcho,
        //        iTotalRecords = TotalRecordCount,
        //        iTotalDisplayRecords = TotalRecordCount,
        //        aaData = DataFromDB
        //    }, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// LoadOrderLineItems
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        //public ActionResult LoadKitLineItems(string KitMasterID)
        //{
        //    // Int64 sID = 0;
        //    //Int64.TryParse(supplierID, out sID);
        //    KitMasterDTO obj = null;
        //    if (Int64.Parse(KitMasterID) <= 0)
        //        obj = new KitMasterDTO() { ID = Int64.Parse(KitMasterID) };
        //    else
        //        obj = new KitMasterController().GetRecord(Int64.Parse(KitMasterID), SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

        //    return PartialView("KitLineItems", obj);
        //}

        /// <summary>
        /// DeleteOrderLineItem
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        //public string DeleteKitLineItem(string Ids)
        //{
        //    try
        //    {

        //        List<KitDetailDTO> objCached = ((List<KitDetailDTO>)SessionHelper.Get(KitSessionKey));
        //        string newIds = string.Empty;
        //        if (objCached != null && objCached.Count > 0)
        //        {
        //            string[] arrIds = Ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //            foreach (var item in arrIds)
        //            {
        //                if (!string.IsNullOrEmpty(item))
        //                {
        //                    int index = objCached.FindIndex(x => x.ItemID == int.Parse(item));
        //                    if (index >= 0)
        //                    {
        //                        if (objCached[index].ID > 0)
        //                        {
        //                            if (!string.IsNullOrEmpty(newIds))
        //                                newIds += ",";

        //                            newIds += objCached[index].ID.ToString();
        //                        }
        //                        objCached.RemoveAt(index);

        //                    }
        //                }
        //            }

        //            SessionHelper.RomoveSessionByKey(KitSessionKey);
        //            SessionHelper.Add(KitSessionKey, objCached);


        //            string deletedIDs = (string)SessionHelper.Get(DeletedKitSessionKey);

        //            if (string.IsNullOrEmpty(deletedIDs))
        //                deletedIDs = newIds;
        //            else
        //                deletedIDs += "," + newIds;


        //            string[] arrDeletedIDs = deletedIDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //            if (arrDeletedIDs.Length > 0)
        //            {
        //                List<string> lstDeletedID = new List<string>();
        //                foreach (var item in arrDeletedIDs)
        //                {
        //                    if (!string.IsNullOrEmpty(item))
        //                        lstDeletedID.Add(item.Trim());
        //                }
        //                SessionHelper.Add(DeletedKitSessionKey, string.Join(",", lstDeletedID.ToArray()));
        //            }
        //        }


        //        return "ok";
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //}

        /// <summary>
        /// _MoveInOutQty
        /// </summary>
        /// <returns></returns>
        //public PartialViewResult _MoveInOutQty()
        //{
        //    return PartialView();
        //}

        /// <summary>
        /// MoveInMoveOutQty
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="DefaultLocation"></param>
        /// <param name="isMoveIn"></param>
        /// <returns></returns>
        //public ActionResult MoveInMoveOutQty(Int64 itemID, Int64? DefaultLocation, bool isMoveIn, Int64 KitDetailID)
        //{
        //    BinMasterController objBinMasterApi = new BinMasterController();

        //    IEnumerable<BinMasterDTO> binDTO = objBinMasterApi.GetAllRecord(itemID, SessionHelper.RoomID, SessionHelper.CompanyID);
        //    SelectList selectionLst = null;
        //    if (DefaultLocation.GetValueOrDefault(0) > 0)
        //        selectionLst = new SelectList(binDTO, "ID", "BinNumber", DefaultLocation);
        //    else
        //        selectionLst = new SelectList(binDTO, "ID", "BinNumber");

        //    ViewBag.DropDownData = selectionLst;
        //    if (isMoveIn)
        //        ViewBag.ButtonText = "Move In";
        //    else
        //        ViewBag.ButtonText = "Move Out";

        //    ItemMasterController itemctrl = new ItemMasterController();
        //    ItemMasterDTO itemDTO = itemctrl.GetRecord(itemID, SessionHelper.RoomID, SessionHelper.CompanyID);
        //    KitDetailDTO kitDetailDTO = new KitDetailDTO() { ID = KitDetailID, ItemID = itemID, ItemGUID = itemDTO.GUID, ItemDetail = itemDTO };

        //    return PartialView("_MoveInOutQty", kitDetailDTO);

        //}

        /// <summary>
        /// LoadOrderLineItems
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        //public ActionResult LoadKitLineItems(Int64 KitID)
        //{
        //    KitMasterDTO objDTO = new KitMasterController().GetRecord(KitID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
        //    return PartialView("_KitLineItems", objDTO);
        //}

        /// <summary>
        /// Get Available Quentity 
        /// </summary>
        /// <param name="objDetailList"></param>
        /// <returns></returns>
        //private double GetAvailableQuentityInWIP(List<KitDetailDTO> objDetailList)
        //{
        //    double TotalQty = 0;
        //    double TempQty = 0;
        //    foreach (var item in objDetailList)
        //    {
        //        TempQty = item.AvailableItemsInWIP.GetValueOrDefault(0) / item.QuantityPerKit.GetValueOrDefault(0);
        //        if (TempQty < TotalQty)
        //            TotalQty = TempQty;

        //    }


        //    return TotalQty;
        //}

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        //public ActionResult GetKitLineItems(JQueryDataTableParamModel param)
        //{
        //    int PageIndex = int.Parse(param.sEcho);
        //    int PageSize = param.iDisplayLength;
        //    var sortDirection = Request["sSortDir_0"];
        //    var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
        //    var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
        //    var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
        //    var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
        //    string sortColumnName = string.Empty;
        //    string sDirection = string.Empty;
        //    int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
        //    sortColumnName = Request["SortingField"].ToString();

        //    // set the default column sorting here, if first time then required to set 
        //    if (sortColumnName == "0" || sortColumnName == "undefined")
        //        sortColumnName = "ID";

        //    if (sortDirection == "asc")
        //        sortColumnName += " asc";
        //    else
        //        sortColumnName += " desc";


        //    string searchQuery = string.Empty;
        //    Int64 KitMasterID = 0;
        //    Int64.TryParse(Request["ParentID"], out KitMasterID);

        //    int TotalRecordCount = 0;

        //    List<KitDetailDTO> objCached = ((List<KitDetailDTO>)SessionHelper.Get(KitSessionKey));
        //    KitDetailController KitDetailController = new KitDetailController();
        //    IEnumerable<KitDetailDTO> DataFromDB = GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, objCached);

        //    return Json(new
        //    {
        //        sEcho = param.sEcho,
        //        iTotalRecords = TotalRecordCount,
        //        iTotalDisplayRecords = TotalRecordCount,
        //        aaData = DataFromDB
        //    }, JsonRequestBehavior.AllowGet);


        //}

        /// <summary>
        /// RevertItemQtyOnCancel
        /// </summary>
        //public void RevertItemQtyOnCancel()
        //{
        //    List<KitDetailDTO> objCached = ((List<KitDetailDTO>)SessionHelper.Get(KitSessionKey));
        //    if (objCached != null && objCached.Count > 0)
        //    {
        //        foreach (var item in objCached)
        //        {
        //            List<PullItemsWithQuantity> lstPullItemsforKit = (List<PullItemsWithQuantity>)SessionHelper.Get(PullItemsForKitSessionKey + item.ItemID);
        //            if (lstPullItemsforKit != null && lstPullItemsforKit.Count > 0)
        //            {
        //                lstPullItemsforKit.ForEach(x => x.ItemGUID = x.PullOrCredit == "kitcredit" ? "kitpull" : "kitcredit");
        //                SubmitPulls(lstPullItemsforKit.ToArray());
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// AddItemToSession
        /// </summary>
        /// <param name="pItemID"></param>
        /// <param name="pItemGUID"></param>
        /// <param name="pQuentity"></param>
        /// <returns></returns>
        //public JsonResult AddItemToSession(string pItemID, string pItemGUID, string pQuentity)
        //{
        //    string message = "";
        //    string status = "";

        //    object obj = SessionHelper.Get(KitSessionKey);
        //    List<KitDetailDTO> lstDto = null;
        //    if (obj != null)
        //        lstDto = (List<KitDetailDTO>)obj;
        //    else
        //        lstDto = new List<KitDetailDTO>();
        //    int Index = lstDto.FindIndex(x => x.ItemDetail.ID == Int64.Parse(pItemID));

        //    if (Index < 0)
        //    {
        //        ItemMasterController itemcontrol = new ItemMasterController();
        //        ItemMasterDTO itm = itemcontrol.GetRecord(Int64.Parse(pItemID), SessionHelper.RoomID, SessionHelper.CompanyID);

        //        KitDetailDTO LineItemDTO = new KitDetailDTO()
        //        {
        //            ItemID = Int64.Parse(pItemID),
        //            ItemGUID = Guid.Parse(pItemGUID),
        //            QuantityPerKit = double.Parse(pQuentity),
        //            AvailableItemsInWIP = 0,
        //            Room = SessionHelper.RoomID,
        //            CreatedBy = SessionHelper.UserID,
        //            Created = DateTime.Now,
        //            LastUpdatedBy = SessionHelper.UserID,
        //            LastUpdated = DateTime.Now,
        //            CompanyID = SessionHelper.CompanyID

        //        };

        //        LineItemDTO.ItemDetail = itm;
        //        lstDto.Add(LineItemDTO);
        //    }
        //    else
        //    {
        //        lstDto[Index].QuantityPerKit = double.Parse(pQuentity);
        //        lstDto[Index].LastUpdatedBy = SessionHelper.UserID;
        //        lstDto[Index].LastUpdated = DateTime.Now;
        //        lstDto[Index].CompanyID = SessionHelper.CompanyID;
        //        lstDto[Index].Room = SessionHelper.RoomID;


        //    }

        //    SessionHelper.RomoveSessionByKey(KitSessionKey);
        //    SessionHelper.Add(KitSessionKey, lstDto);
        //    message = "Item added";
        //    status = "success";
        //    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// AddItemToSessionMultiple
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ///[HttpPost]
        //public JsonResult AddItemToSessionMultiple(string para)
        //{
        //    string message = "";
        //    string status = "";
        //    JavaScriptSerializer s = new JavaScriptSerializer();
        //    try
        //    {
        //        ItemWithQuentity[] ItemWithQty = s.Deserialize<ItemWithQuentity[]>(para);
        //        if (ItemWithQty != null && ItemWithQty.Length > 0)
        //        {
        //            object obj = SessionHelper.Get(KitSessionKey);
        //            List<KitDetailDTO> lstDto = null;
        //            if (obj != null)
        //                lstDto = (List<KitDetailDTO>)obj;
        //            else
        //                lstDto = new List<KitDetailDTO>();

        //            foreach (ItemWithQuentity item in ItemWithQty)
        //            {
        //                int Index = lstDto.FindIndex(x => x.ItemID == Int64.Parse(item.ID));

        //                if (Index < 0)
        //                {
        //                    ItemMasterController itemcontrol = new ItemMasterController();
        //                    ItemMasterDTO itm = itemcontrol.GetRecord(Int64.Parse(item.ID), SessionHelper.RoomID, SessionHelper.CompanyID);

        //                    KitDetailDTO OItemDTO = new KitDetailDTO()
        //                    {


        //                        ID = 0,
        //                        ItemID = Int64.Parse(item.ID),
        //                        KitID = 0,
        //                        KitGUID = Guid.Empty,
        //                        ItemGUID = Guid.Parse(item.GUID),
        //                        QuantityPerKit = double.Parse(item.Qty),
        //                        AvailableItemsInWIP = 0,
        //                        GUID = Guid.Empty,
        //                        RoomName = SessionHelper.RoomName,
        //                        UpdatedByName = SessionHelper.UserName,
        //                        CreatedByName = SessionHelper.UserName,
        //                        Room = SessionHelper.RoomID,
        //                        CreatedBy = SessionHelper.UserID,
        //                        Created = DateTime.Now,
        //                        LastUpdatedBy = SessionHelper.UserID,
        //                        LastUpdated = DateTime.Now,
        //                        CompanyID = SessionHelper.CompanyID,

        //                    };

        //                    OItemDTO.ItemDetail = itm;
        //                    lstDto.Add(OItemDTO);

        //                }
        //                else
        //                {
        //                    lstDto[Index].QuantityPerKit = double.Parse(item.Qty);
        //                    lstDto[Index].LastUpdatedBy = SessionHelper.UserID;
        //                    lstDto[Index].LastUpdated = DateTime.Now;
        //                    lstDto[Index].CompanyID = SessionHelper.CompanyID;
        //                    lstDto[Index].Room = SessionHelper.RoomID;

        //                }
        //            }
        //            SessionHelper.RomoveSessionByKey(KitSessionKey);
        //            SessionHelper.Add(KitSessionKey, lstDto);
        //        }
        //        message = "Item added";
        //        status = "success";
        //        return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        message = "Item not added";
        //        status = "fail";
        //        return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);

        //    }

        //}

        /// <summary>
        /// GetItemWisePullUnSavedData
        /// </summary>
        /// <param name="ItemID"></param>
        /// <returns></returns>
        //public JsonResult GetItemWisePullUnSavedData(Int64 ItemID)
        //{
        //    List<PullItemsWithQuantity> lstPullItemsforKit = null;// (List<PullItemsWithQuantity>)SessionHelper.Get(PullItemsForKitSessionKey + ItemID);
        //    if (lstPullItemsforKit == null)
        //        lstPullItemsforKit = new List<PullItemsWithQuantity>();

        //    return Json(new { Result = lstPullItemsforKit }, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// AddItemQuantityInItemWIP
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        //public JsonResult AddItemQuantityInItemWIP(string para)
        //{
        //    try
        //    {


        //        JavaScriptSerializer s = new JavaScriptSerializer();
        //        MoveInOutQtyDetail LocationWiseItemsQty = s.Deserialize<MoveInOutQtyDetail>(para);
        //        string result = "";
        //        List<PullItemsWithQuantity> lstOldItem = (List<PullItemsWithQuantity>)SessionHelper.Get(PullItemsForKitSessionKey + LocationWiseItemsQty.ItemID);
        //        if (lstOldItem == null)
        //            lstOldItem = new List<PullItemsWithQuantity>();

        //        List<PullItemsWithQuantity> lstPullItemsforKit = new List<PullItemsWithQuantity>();

        //        foreach (var item in LocationWiseItemsQty.BinWiseQty)
        //        {
        //            PullItemsWithQuantity newPulledItem = new PullItemsWithQuantity()
        //            {
        //                BinID = item.LocationID.ToString(),
        //                ID = "",
        //                ItemGUID = LocationWiseItemsQty.ItemGUID,
        //                ItemID = LocationWiseItemsQty.ItemID.ToString(),
        //                ProjectID = "",
        //                PullCreditQuantity = item.Quantity.ToString(),
        //                PullOrCredit = LocationWiseItemsQty.ButtonText == "Move In" ? "kitpull" : "kitcredit",
        //                TempPullQTY = item.Quantity.ToString(),
        //                BinLocation = item.LocationName,
        //                //KitDetailID = item.KitDetailID
        //            };

        //            lstPullItemsforKit.Add(newPulledItem);
        //            lstOldItem.Add(newPulledItem);

        //        }

        //        SessionHelper.Add(PullItemsForKitSessionKey + LocationWiseItemsQty.ItemID, lstOldItem);

        //        result = SubmitPulls(lstPullItemsforKit.ToArray());
        //        if (result == "ok")
        //        {
        //            object obj = SessionHelper.Get(KitSessionKey);
        //            List<KitDetailDTO> lstDto = (List<KitDetailDTO>)obj;
        //            int Index = lstDto.FindIndex(x => x.ItemDetail.ID == LocationWiseItemsQty.ItemID);
        //            //ItemMasterDTO itmDTO = new ItemMasterController().GetRecord(LocationWiseItemsQty.ItemID, SessionHelper.RoomID, SessionHelper.CompanyID);
        //            //lstDto[Index].ItemDetail = itmDTO;
        //            if (LocationWiseItemsQty.ButtonText == "Move In")
        //            {
        //                lstDto[Index].AvailableItemsInWIP += LocationWiseItemsQty.TotalQty;
        //                lstDto[Index].ItemDetail.OnHandQuantity -= LocationWiseItemsQty.TotalQty;
        //            }
        //            else
        //            {
        //                lstDto[Index].AvailableItemsInWIP += LocationWiseItemsQty.TotalQty;
        //                lstDto[Index].ItemDetail.OnHandQuantity -= LocationWiseItemsQty.TotalQty;
        //            }
        //            lstDto[Index].QuantityReadyForAssembly = Math.Floor(lstDto[Index].AvailableItemsInWIP.GetValueOrDefault(0) / lstDto[Index].QuantityPerKit.GetValueOrDefault(1));


        //            lstDto[Index].LastUpdatedBy = SessionHelper.UserID;
        //            lstDto[Index].LastUpdated = DateTime.Now;
        //            lstDto[Index].CompanyID = SessionHelper.CompanyID;
        //            lstDto[Index].Room = SessionHelper.RoomID;
        //            SessionHelper.RomoveSessionByKey(KitSessionKey);
        //            SessionHelper.Add(KitSessionKey, lstDto);


        //            return Json(new { Message = "Success", Status = "success" }, JsonRequestBehavior.AllowGet);
        //        }
        //        return Json(new { Message = "fail", Status = "fail" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception)
        //    {

        //        return Json(new { Message = "fail", Status = "fail" }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        /// <summary>
        /// SubmitPulls
        /// </summary>
        /// <param name="PullItemsQty"></param>
        /// <returns></returns>
        ///[HttpPost]
        //public string SubmitPulls(PullItemsWithQuantity[] PullItemsQty)
        //{
        //    string message = "";
        //    string status = "";

        //    try
        //    {
        //        StringBuilder sbItemLocationMSG = new StringBuilder();


        //        if (PullItemsQty != null && PullItemsQty.Length > 0)
        //        {
        //            KitMoveInOutDetailController obj = new KitMoveInOutDetailController();


        //            foreach (PullItemsWithQuantity item in PullItemsQty)
        //            {
        //                KitMoveInOutDetailDTO objDTO = new KitMoveInOutDetailDTO();
        //                if (!string.IsNullOrEmpty(item.ID))
        //                    objDTO.ID = Int64.Parse(item.ID);

        //                objDTO.ItemID = Int64.Parse(item.ItemID);



        //                if (!string.IsNullOrEmpty(item.ItemGUID))
        //                    objDTO.ItemGUID = Guid.Parse(item.ItemGUID);

        //                objDTO.Updated = System.DateTime.Now;
        //                objDTO.LastUpdatedBy = SessionHelper.UserID;
        //                objDTO.CompanyID = SessionHelper.CompanyID;
        //                objDTO.Room = SessionHelper.RoomID;
        //                objDTO.CreatedBy = SessionHelper.UserID;

        //                Int64 kitDetailID = 0;
        //                Int64.TryParse(item.KitDetailID, out kitDetailID);
        //                objDTO.KitDetailID = kitDetailID;

        //                try
        //                {
        //                    // <param name="IsCreditPullNothing"></param> 1 = credit, 2 = pull, 3 = nothing

        //                    int IsCreditPullNothing = 0; // false means its PULL
        //                    if (item.PullOrCredit == "credit")
        //                        IsCreditPullNothing = 1;
        //                    else if (item.PullOrCredit == "pull")
        //                        IsCreditPullNothing = 2;
        //                    else if (item.PullOrCredit == "kitpull")
        //                        IsCreditPullNothing = 2;
        //                    else if (item.PullOrCredit == "kitcredit")
        //                        IsCreditPullNothing = 1;
        //                    else
        //                        IsCreditPullNothing = 3;

        //                    string ItemLocationMSG = "";

        //                    #region "Check Project Spend Condition"
        //                    bool IsProjecSpendAllowed = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowOverrideProjectSpendLimits, eTurnsWeb.Helper.SessionHelper.PermissionType.Approval);
        //                    #endregion

        //                    //obj.UpdatePullData(objDTO, IsCreditPullNothing, SessionHelper.RoomID, SessionHelper.CompanyID, out ItemLocationMSG, IsProjecSpendAllowed);

        //                    if (ItemLocationMSG != "")
        //                        sbItemLocationMSG.Append(ItemLocationMSG + "[##]");

        //                    message = ResMessage.SaveMessage;
        //                    status = "ok";
        //                }
        //                catch
        //                {
        //                    message = ResMessage.SaveErrorMsg;
        //                    status = "fail";
        //                }
        //            }
        //        }
        //        message = "Pulled successfully";
        //        status = "ok";
        //        //return Json(new { Message = message, Status = status, LocationMSG = sbItemLocationMSG.ToString() }, JsonRequestBehavior.AllowGet);
        //        return status;
        //    }
        //    catch (Exception ex)
        //    {
        //        message = "Error occured";
        //        status = "fail";
        //        //return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        //        return status;
        //    }
        //}


        /// <summary>
        /// GetPagedRecords
        /// </summary>
        /// <param name="StartRowIndex"></param>
        /// <param name="MaxRows"></param>
        /// <param name="TotalCount"></param>
        /// <param name="SearchTerm"></param>
        /// <param name="sortColumnName"></param>
        /// <param name="RoomID"></param>
        /// <param name="CompanyID"></param>
        /// <param name="IsArchived"></param>
        /// <param name="IsDeleted"></param>
        /// <param name="lstCached"></param>
        /// <returns></returns>
        //public IEnumerable<KitDetailDTO> GetPagedRecords(Int32 StartRowIndex, Int32 MaxRows, out Int32 TotalCount, string SearchTerm, string sortColumnName, Int64 RoomID, Int64 CompanyID, bool IsArchived, bool IsDeleted, List<KitDetailDTO> lstCached)
        //{
        //    //Get Cached-Media
        //    if (lstCached == null || lstCached.Count <= 0)
        //    {
        //        TotalCount = 0;
        //        return new List<KitDetailDTO>();
        //    }

        //    IEnumerable<KitDetailDTO> ObjCache = lstCached;
        //    IEnumerable<KitDetailDTO> ObjGlobalCache = ObjCache;
        //    ObjCache = ObjCache.Where(t => (t.IsArchived.GetValueOrDefault(false) == false && t.IsDeleted.GetValueOrDefault(false) == false));

        //    if (IsArchived && IsDeleted)
        //    {
        //        ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsArchived == IsArchived || t.IsDeleted == IsDeleted));
        //        ObjCache = ObjCache.Concat(ObjGlobalCache);
        //    }
        //    else if (IsArchived)
        //    {
        //        ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsArchived == IsArchived));
        //        ObjCache = ObjCache.Concat(ObjGlobalCache);
        //    }
        //    else if (IsDeleted)
        //    {
        //        ObjGlobalCache = ObjGlobalCache.Where(t => (t.IsDeleted == IsDeleted));
        //        ObjCache = ObjCache.Concat(ObjGlobalCache);
        //    }

        //    if (String.IsNullOrEmpty(SearchTerm))
        //    {

        //        TotalCount = ObjCache.Count();
        //        return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

        //    }
        //    else if (SearchTerm.Contains("[###]"))
        //    {
        //        //Get Cached-Media
        //        //IEnumerable<KitDetailDTO> ObjCache = GetCachedData(RoomID, CompanyID);
        //        string[] stringSeparators = new string[] { "[###]" };
        //        string[] Fields = SearchTerm.Split(stringSeparators, StringSplitOptions.None);

        //        ObjCache = ObjCache.Where(t =>
        //               ((Fields[1].Split('@')[0] == "") || (Fields[1].Split('@')[0].Split(',').ToList().Contains(t.CreatedByName)))
        //            && ((Fields[1].Split('@')[1] == "") || (Fields[1].Split('@')[1].Split(',').ToList().Contains(t.UpdatedByName)))
        //            && ((Fields[1].Split('@')[2] == "") || (t.Created >= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[0]).Date && t.Created <= Convert.ToDateTime(Fields[1].Split('@')[2].Split(',')[1]).Date))
        //            && ((Fields[1].Split('@')[3] == "") || (t.LastUpdated >= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[0]).Date && t.LastUpdated <= Convert.ToDateTime(Fields[1].Split('@')[3].Split(',')[1]).Date))
        //            );

        //        TotalCount = ObjCache.Count();
        //        return ObjCache.OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);

        //    }
        //    else
        //    {

        //        TotalCount = ObjCache.Where
        //            (
        //                t => t.ID.ToString().Contains(SearchTerm)

        //            ).Count();
        //        return ObjCache.Where(t => t.ID.ToString().Contains(SearchTerm)).OrderBy(sortColumnName).Skip(StartRowIndex).Take(MaxRows);


        //    }
        //}

        /// <summary>
        /// ClearKitSession
        /// </summary>
        //public void ClearKitSession()
        //{
        //    List<KitDetailDTO> objCached = ((List<KitDetailDTO>)SessionHelper.Get(KitSessionKey));
        //    if (objCached != null && objCached.Count > 0)
        //    {
        //        foreach (var item in objCached)
        //        {
        //            List<PullItemsWithQuantity> lstPullItemsforKit = (List<PullItemsWithQuantity>)SessionHelper.Get(PullItemsForKitSessionKey + item.ItemID);
        //            SessionHelper.RomoveSessionByKey(PullItemsForKitSessionKey + item.ItemID);
        //        }
        //    }

        //    SessionHelper.RomoveSessionByKey(KitSessionKey);
        //    SessionHelper.RomoveSessionByKey(DeletedKitSessionKey);
        //}

        /// <summary>
        /// UpdateKitItemQty
        /// </summary>
        /// <param name="ItemID"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        //public string UpdateKitItemQty(Int64 ItemID, string value)
        //{
        //    object obj = SessionHelper.Get(KitSessionKey);
        //    List<KitDetailDTO> lstDto = (List<KitDetailDTO>)obj; ;
        //    int Index = lstDto.FindIndex(x => x.ItemDetail.ID == ItemID);
        //    double qty = 0;
        //    double.TryParse(value, out qty);
        //    if (qty > 0)
        //    {
        //        lstDto[Index].QuantityPerKit = double.Parse(value);
        //        lstDto[Index].QuantityReadyForAssembly = lstDto[Index].AvailableItemsInWIP.GetValueOrDefault(0) / lstDto[Index].QuantityPerKit;
        //        lstDto[Index].LastUpdatedBy = SessionHelper.UserID;
        //        lstDto[Index].LastUpdated = DateTime.Now;
        //        lstDto[Index].CompanyID = SessionHelper.CompanyID;
        //        lstDto[Index].Room = SessionHelper.RoomID;
        //        SessionHelper.RomoveSessionByKey(KitSessionKey);
        //        SessionHelper.Add(KitSessionKey, lstDto);
        //    }
        //    return lstDto[Index].QuantityPerKit.GetValueOrDefault(0).ToString();
        //}

        /// <summary>
        /// _KitHisory
        /// </summary>
        /// <returns></returns>
        //public PartialViewResult _KitHisory()
        //{
        //    return PartialView();
        //}

        /// <summary>
        /// LoadOrderLineItems
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        //public ActionResult LoadKitLineItemHistory(string KitMasterHistoryID)
        //{
        //    // Int64 sID = 0;
        //    //Int64.TryParse(supplierID, out sID);
        //    KitMasterDTO obj = null;
        //    obj = new KitMasterController().GetHistoryRecord(Int64.Parse(KitMasterHistoryID));
        //    return PartialView("_KitLineItemHistory", obj);
        //}

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        //public ActionResult GetKitLineItemHistory(JQueryDataTableParamModel param)
        //{
        //    int PageIndex = int.Parse(param.sEcho);
        //    int PageSize = param.iDisplayLength;
        //    var sortDirection = Request["sSortDir_0"];
        //    var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
        //    var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
        //    var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
        //    var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
        //    string sortColumnName = string.Empty;
        //    string sDirection = string.Empty;
        //    int StartWith = (param.iDisplayLength - param.iDisplayStart) + 1;
        //    sortColumnName = Request["SortingField"].ToString();

        //    // set the default column sorting here, if first time then required to set 
        //    if (sortColumnName == "0" || sortColumnName == "undefined")
        //        sortColumnName = "ID";

        //    if (sortDirection == "asc")
        //        sortColumnName += " asc";
        //    else
        //        sortColumnName += " desc";


        //    string searchQuery = string.Empty;
        //    Int64 KitMasterHistoryID = 0;
        //    Int64.TryParse(Request["ParentID"], out KitMasterHistoryID);

        //    int TotalRecordCount = 0;

        //    List<KitDetailDTO> objCached = ((List<KitDetailDTO>)SessionHelper.Get(KitSessionKey));
        //    KitDetailController KitDetailController = new KitDetailController();
        //    IEnumerable<KitDetailDTO> DataFromDB = GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, objCached);

        //    return Json(new
        //    {
        //        sEcho = param.sEcho,
        //        iTotalRecords = TotalRecordCount,
        //        iTotalDisplayRecords = TotalRecordCount,
        //        aaData = DataFromDB
        //    }, JsonRequestBehavior.AllowGet);


        //}
        #endregion

        /// <summary>
        /// KitMasterListHistory
        /// </summary>
        /// <returns></returns>
        public ActionResult KitMasterListHistory()
        {
            return PartialView("_KitListHistory");
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult KitHistoryView(Int64 ID)
        {
            //KitMasterController obj = new KitMasterController();
            KitMasterDAL obj = new KitMasterDAL(SessionHelper.EnterPriseDBName);
            KitMasterDTO objDTO = obj.GetKitHistoryByHistoryIdFull(ID);
            objDTO.IsHistory = true;

            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("KitMaster");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }

            ViewBag.ReOrderTypeList = GetReorderTypeOrKitCategory(0);
            ViewBag.KitCategoryList = GetReorderTypeOrKitCategory(1);


            return PartialView("_KitHistory", objDTO);
        }

        /// <summary>
        /// LoadKitLineItemHistory
        /// </summary>
        /// <param name="KitDTO"></param>
        /// <returns></returns>
        public ActionResult LoadKitLineItemHistory(Int64 KitHistoryID)
        {
            KitMasterDTO KitDTO = new KitMasterDAL(SessionHelper.EnterPriseDBName).GetKitHistoryByHistoryIdFull(KitHistoryID);
            KitDTO.KitItemList = new KitDetailDAL(SessionHelper.EnterPriseDBName).GetAllHistoryRecord(KitDTO.GUID.ToString(), SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            return PartialView("_KitLineItemHistory", KitDTO);
        }

        public JsonResult ValidateLotExpirationCombination(Guid KitGuid, string LotNumber, string ExpirationDate)
        {
            var pullTransactionDAL = new PullTransactionDAL(SessionHelper.EnterPriseDBName);
            DateTime ExpirationDateUTC = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(ExpirationDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone);
            var isLotWithDifferentExpiredDateNotExist = pullTransactionDAL.ValidateLotDateCodeForCredit(KitGuid, LotNumber, ExpirationDateUTC, SessionHelper.CompanyID, SessionHelper.RoomID);
            var status = "ok";
            var msg = string.Empty;
            if (!isLotWithDifferentExpiredDateNotExist)
            {
                status = "duplicate";
                msg = string.Format(ResTransfer.MsgLotPlusExpirationDateNotMatched, LotNumber);
            }

            return Json(new { Message = msg, Status = status }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult BuildNewKit(string objDTO)
        {
            JavaScriptSerializer s = new JavaScriptSerializer();
            BuildBreakKitDetail QLDetails = s.Deserialize<BuildBreakKitDetail>(objDTO);
            List<ItemLocationDetailsDTO> lstLocationWiseDTO = new List<ItemLocationDetailsDTO>();
            ItemLocationDetailsDTO itemLocDTO = new ItemLocationDetailsDTO();
            itemLocDTO.BinID = Int64.Parse(QLDetails.LocationID);
            BinMasterDTO objBinMasterDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(itemLocDTO.BinID ?? 0, SessionHelper.RoomID, SessionHelper.CompanyID);
            if (objBinMasterDTO != null)
            {
                itemLocDTO.BinNumber = objBinMasterDTO.BinNumber;
            }
            itemLocDTO.CompanyID = SessionHelper.CompanyID;
            itemLocDTO.Cost = Convert.ToDouble(QLDetails.Cost);
            itemLocDTO.Received = DateTimeUtility.DateTimeNow.ToString("MM/dd/yyyy");
            itemLocDTO.ReceivedDate = DateTimeUtility.DateTimeNow;
            itemLocDTO.Room = SessionHelper.RoomID;
            if (QLDetails.Consignment)
                itemLocDTO.ConsignedQuantity = double.Parse(QLDetails.Quantity);
            else
                itemLocDTO.CustomerOwnedQuantity = double.Parse(QLDetails.Quantity);
            if (!string.IsNullOrEmpty(QLDetails.LotNumber))
            {
                itemLocDTO.LotNumber = QLDetails.LotNumber;
                itemLocDTO.LotNumberTracking = true;
            }
            if (!string.IsNullOrEmpty(QLDetails.ExpirationDate))
            {
                itemLocDTO.DateCodeTracking = true;
                itemLocDTO.Expiration = QLDetails.ExpirationDate;
                itemLocDTO.ExpirationDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(QLDetails.ExpirationDate, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone);//Convert.ToDateTime(QLDetails.ExpirationDate);

                if (!string.IsNullOrEmpty(QLDetails.LotNumber))
                {
                    var pullTransactionDAL = new PullTransactionDAL(SessionHelper.EnterPriseDBName);
                    var isLotWithDifferentExpiredDateNotExist = pullTransactionDAL.ValidateLotDateCodeForCredit(Guid.Parse(QLDetails.KitGuid), QLDetails.LotNumber, itemLocDTO.ExpirationDate.Value, SessionHelper.CompanyID, SessionHelper.RoomID);
                    if (!isLotWithDifferentExpiredDateNotExist)
                    {
                        return Json(new { Status = "fail", Message = string.Format(ResTransfer.MsgLotPlusExpirationDateNotMatched, QLDetails.LotNumber), ReturnDTO = objDTO }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            itemLocDTO.CreatedBy = SessionHelper.UserID;
            itemLocDTO.LastUpdatedBy = SessionHelper.UserID;
            itemLocDTO.ItemGUID = Guid.Parse(QLDetails.KitGuid);
            itemLocDTO.ItemType = 3;
            itemLocDTO.InsertedFrom = "KitBuild";
            itemLocDTO.EditedFrom = "Web-KitBuild";
            itemLocDTO.AddedFrom = "Web-KitBuild";
            //itemLocDTO.Action = "KitBuild";
            lstLocationWiseDTO.Add(itemLocDTO);
            long SessionUserId = SessionHelper.UserID;
            var enterpriseId = SessionHelper.EnterPriceID;

            if (lstLocationWiseDTO != null && lstLocationWiseDTO.Count > 0)
            {
                JsonResult objJsonResult = new InventoryController().ItemLocationDetailsSave(lstLocationWiseDTO);
                if (objJsonResult.Data.ToString().Contains("Status = OK"))
                {
                    ItemMasterDAL objItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                    ItemMasterDTO objItemDTO = objItemDAL.GetItemWithoutJoins(null, Guid.Parse(QLDetails.KitGuid));
                    objItemDTO.LastUpdatedBy = SessionHelper.UserID;
                    objItemDTO.Updated = DateTimeUtility.DateTimeNow;
                    objItemDTO.WhatWhereAction = "Kit-Build";
                    objItemDAL.Edit(objItemDTO, SessionUserId, SessionHelper.EnterPriceID);
                    KitDetailDAL objKitDetailctrl = new KitDetailDAL(SessionHelper.EnterPriseDBName);
                    List<KitDetailDTO> lstKitDetailDTO = objKitDetailctrl.GetAllRecordsByKitGUID(Guid.Parse(QLDetails.KitGuid), SessionHelper.RoomID, SessionHelper.CompanyID, false, false, true).ToList();
                    foreach (var item in lstKitDetailDTO)
                    {
                        //if (item.ItemType != 4)
                        //{
                        item.AvailableItemsInWIP = item.AvailableItemsInWIP.GetValueOrDefault(0) - (item.QuantityPerKit * double.Parse(QLDetails.Quantity));
                        item.LastUpdatedBy = SessionHelper.UserID;
                        item.ReceivedOn = DateTimeUtility.DateTimeNow;
                        item.EditedFrom = "Web";
                        objKitDetailctrl.Edit(item, SessionUserId, enterpriseId);
                        //}
                    }

                    KitMoveInOutDetailDTO objKitMoveInOutDTO = new KitMoveInOutDetailDTO()
                    {
                        BinID = objBinMasterDTO.ID,

                        MoveInOut = "BuildKit",
                        ItemGUID = objItemDTO.GUID,

                        Created = DateTime.Now,
                        Updated = DateTime.Now,
                        CreatedBy = UserID,
                        LastUpdatedBy = UserID,
                        CompanyID = CompanyID,
                        Room = RoomID,

                        IsArchived = false,
                        IsDeleted = false,
                        ItemLocationDetailGUID = lstLocationWiseDTO[0].GUID,
                        ConsignedQuantity = itemLocDTO.ConsignedQuantity,
                        CustomerOwnedQuantity = itemLocDTO.CustomerOwnedQuantity,
                        TotalQuantity = itemLocDTO.ConsignedQuantity.GetValueOrDefault(0) + itemLocDTO.CustomerOwnedQuantity.GetValueOrDefault(0),
                    };

                    KitMoveInOutDetailDAL KitMoveInOut = new KitMoveInOutDetailDAL(SessionHelper.EnterPriseDBName);
                    KitMoveInOut.Insert(objKitMoveInOutDTO);

                }
                return objJsonResult;
            }
            return Json(new { Status = "fail", Message = "fail", ReturnDTO = objDTO }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// BreakKit
        /// </summary>
        /// <param name="objKit"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult BreakNewKit(MoveInOutQtyDetail objMoveQty)
        {
            try
            {
                string status = "fail";
                long SessionUserId = SessionHelper.UserID;
                //KitMoveInOutDetailController kitMoveInOutCtrl = new KitMoveInOutDetailController();
                KitMoveInOutDetailDAL kitMoveInOutCtrl = new KitMoveInOutDetailDAL(SessionHelper.EnterPriseDBName);
                ResponseMessage RespMsg = kitMoveInOutCtrl.BreakKit(objMoveQty, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionUserId, SessionHelper.EnterPriceID, ResourceHelper.CurrentCult.Name);
                if (RespMsg.IsSuccess)
                    status = "ok";

                return Json(new { Message = RespMsg.Message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public string GetWIPKitCountForRedCircle()
        {
            try
            {
                KitMasterDAL controller = new KitMasterDAL(SessionHelper.EnterPriseDBName);
                // IEnumerable<KitMasterDTO> DataFromDB = controller.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, SessionHelper.UserSupplierID);

                int kitCount = controller.GetWIPKitCountForRedCircle(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserSupplierIds);
                return kitCount.ToString();
            }
            catch (Exception)
            {

                return "0";
            }
        }

        /// <summary>
        /// QtyToMove
        /// </summary>
        /// <param name="KitDetailGuid"></param>
        /// <param name="Qty"></param>
        /// <param name="Bin"></param>
        /// <param name="TransType"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult QtyToMove(List<KitMoveInOutDetailDTO> MoveInDTO)
        {
            KitDetailDAL kitDtlDAL = null;
            string msg = string.Empty;

            try
            {
                kitDtlDAL = new KitDetailDAL(SessionHelper.EnterPriseDBName);
                foreach (var item in MoveInDTO)
                {
                    item.CreatedBy = UserID;
                    item.LastUpdatedBy = UserID;
                    item.Room = RoomID;
                    item.CompanyID = CompanyID;
                    long SessionUserId = SessionHelper.UserID;
                    if (item.MoveInOut == "IN")
                    {
                        msg += kitDtlDAL.QtyToMoveIn(item, SessionUserId, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, ResourceHelper.CurrentCult.Name);
                    }
                    else if (item.MoveInOut == "OUT")
                    {
                        msg += kitDtlDAL.QtyToMoveOut(item, SessionUserId, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, ResourceHelper.CurrentCult.Name);
                    }
                }

                /*
                 if (MoveInDTO.TotalQuantity <= 0)
                     msg = "Quantity Must be greater then zero.";
                 else if (string.IsNullOrEmpty(MoveInDTO.BinNumber))
                     msg = "Please select bin.";

                 if (!string.IsNullOrEmpty(msg))
                     return Json(new { Status = false, Message = msg }, JsonRequestBehavior.AllowGet);

                 kitDtlDAL = new KitDetailDAL(SessionHelper.EnterPriseDBName);

                 msg += kitDtlDAL.QtyToMoveIn(MoveInDTO);

                 if (!string.IsNullOrEmpty(msg) && msg.Length > 0)
                 {
                     return Json(new { Status = false, Message = msg }, JsonRequestBehavior.AllowGet);
                 }
                
                */
                if (!string.IsNullOrEmpty(msg))
                    return Json(new { Status = false, Message = msg }, JsonRequestBehavior.AllowGet);

                return Json(new { Status = true, Message = "Success" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                kitDtlDAL = null;
            }
        }


        public JsonResult QtyToMoveBulk(double qty, Guid KitGuid, string MoveType, List<Guid> KitDetailGuids, bool isSingleMoveOut = false)
        {
            KitDetailDAL kitDetailDAL = null;
            ItemMasterDAL itemDAL = null;
            List<KitDetailDTO> kitDetails = null;
            ItemMasterDTO item = null;
            List<KitMoveInOutDetailDTO> kitMoveInOutDetails = null;
            KitMoveInOutDetailDTO kitMoveInOutDetail = null;
            List<ItemLocationDetailsDTO> itmLocs = null;
            ItemLocationDetailsDAL itmLocDtlDAL = null;
            string strErrorMsg = "";
            try
            {
                if (qty <= 0)
                {
                    return Json(new { Status = false, Message = string.Format(ResCommon.MsgGreaterThanZero, ResMoveMaterial.MoveQuantity) }, JsonRequestBehavior.AllowGet);
                }

                if (KitDetailGuids == null || KitDetailGuids.Count <= 0)
                {
                    return Json(new { Status = false, Message = ResKitMaster.SelectItemsForBulkMove }, JsonRequestBehavior.AllowGet);
                }

                itemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                itmLocDtlDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                kitDetailDAL = new KitDetailDAL(SessionHelper.EnterPriseDBName);
                kitMoveInOutDetails = new List<KitMoveInOutDetailDTO>();
                kitDetails = kitDetailDAL.GetAllRecordsByKitGUID(KitGuid, RoomID, CompanyID, false, false, false).ToList();

                foreach (var kitItem in kitDetails)
                {
                    if (KitDetailGuids.Contains(kitItem.GUID))
                    {
                        item = itemDAL.GetItemByGuidNormalForKit(kitItem.ItemGUID.GetValueOrDefault(Guid.Empty));
                        double totalMoveQty = isSingleMoveOut ? qty : (qty * kitItem.QuantityPerKit.GetValueOrDefault(0));

                        if (MoveType == "IN")
                        {
                            if (totalMoveQty > item.OnHandQuantity.GetValueOrDefault(0))
                            {
                                //strErrorMsg = "Item#:" + item.ItemNumber + "'s Bin:" + item.DefaultLocationName + " has not enough quantity.";
                                strErrorMsg = string.Format(ResKitMaster.ItemHasNotEnoughQty, item.ItemNumber);
                                break;
                            }

                            //itmLocs = itmLocDtlDAL.GetCachedDataeVMI(item.GUID, RoomID, CompanyID).Where(x => x.BinID == item.DefaultLocation.GetValueOrDefault(0)).ToList();
                            itmLocs = itmLocDtlDAL.GetItemLocationeForKitMove(item.GUID, RoomID, CompanyID).ToList();
                            double sumLocQty = itmLocs.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0) + x.CustomerOwnedQuantity.GetValueOrDefault(0));
                            if (totalMoveQty > sumLocQty)
                            {
                                //strErrorMsg = "Item#:" + item.ItemNumber + "'s  Bin:" + item.DefaultLocationName + " has not enough quantity.";
                                strErrorMsg = string.Format(ResKitMaster.ItemHasNotEnoughQty, item.ItemNumber);
                                break;
                            }
                        }
                        else if (MoveType == "OUT")
                        {
                            if (item.SerialNumberTracking)
                            {
                                strErrorMsg = string.Format(ResKitMaster.ItemIsSerialTracking, item.ItemNumber);
                                break;
                            }
                            if (item.LotNumberTracking)
                            {
                                strErrorMsg = string.Format(ResKitMaster.ItemIsLotTracking, item.ItemNumber);
                                break;
                            }
                            if (item.DateCodeTracking)
                            {
                                strErrorMsg = string.Format(ResKitMaster.ItemIsDateCodeTracking, item.ItemNumber);
                                break;
                            }

                            if (totalMoveQty > kitItem.AvailableItemsInWIP.GetValueOrDefault(0))
                            {
                                strErrorMsg = string.Format(ResKitMaster.ItemHasNotEnoughQtyToMoveOut, item.ItemNumber);
                                break;
                            }
                        }

                        if (string.IsNullOrEmpty(strErrorMsg))
                        {
                            double kitQty = isSingleMoveOut ? qty : (qty * kitItem.QuantityPerKit.GetValueOrDefault(0));
                            if (MoveType == "OUT")
                            {
                                if (kitItem.AvailableItemsInWIP.GetValueOrDefault(0) == 0)
                                {
                                    strErrorMsg += string.Format(ResMessage.KitItemAvailableInWIP, item.ItemNumber); //"Item#:" + item.ItemNumber + "'Available ItemsInWIP";
                                    continue;
                                }
                                else if (kitQty > kitItem.AvailableItemsInWIP.GetValueOrDefault(0))
                                {
                                    kitQty = kitItem.AvailableItemsInWIP.GetValueOrDefault(0);
                                }
                            }
                            if (MoveType == "IN")
                            {
                                double sumOfQtyAtDefaultLocation = itmLocs.Where(x => x.BinID == item.DefaultLocation.GetValueOrDefault(0)).ToList().Sum(x => x.ConsignedQuantity.GetValueOrDefault(0) + x.CustomerOwnedQuantity.GetValueOrDefault(0));
                                long? binID = 0;
                                string binNumber = string.Empty;

                                if (totalMoveQty <= sumOfQtyAtDefaultLocation)
                                {
                                    binID = item.DefaultLocation.GetValueOrDefault(0);
                                    binNumber = item.DefaultLocationName;
                                }
                                else
                                {
                                    var binIDs = itmLocs.ToList().OrderBy(e => e.BinNumber).Select(e => e.BinID).Distinct();

                                    foreach (var Id in binIDs)
                                    {
                                        var itemLocations = itmLocs.Where(e => e.BinID == Id).ToList();
                                        var totalQty = itemLocations.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0) + x.CustomerOwnedQuantity.GetValueOrDefault(0));
                                        if (totalQty >= totalMoveQty)
                                        {
                                            binID = Id.Value;
                                            binNumber = itemLocations.FirstOrDefault().BinNumber;
                                            break;
                                        }
                                    }
                                }
                                if (binID.HasValue && binID.Value > 0 && !string.IsNullOrEmpty(binNumber))
                                {
                                    kitMoveInOutDetail = new KitMoveInOutDetailDTO()
                                    {
                                        AddedFrom = "Kit Move",
                                        BinID = binID,
                                        BinNumber = binNumber,
                                        CompanyID = CompanyID,
                                        ConsignedQuantity = 0,
                                        Created = DateTimeUtility.DateTimeNow,
                                        CreatedBy = UserID,
                                        CreatedByName = "",
                                        CustomerOwnedQuantity = 0,
                                        EditedFrom = "Web",
                                        TotalQuantity = kitQty,
                                        MoveInOut = MoveType,
                                        ItemGUID = kitItem.ItemGUID,
                                        GUID = Guid.Empty,
                                        ID = 0,
                                        IsArchived = false,
                                        IsDeleted = false,
                                        KitDetailGUID = kitItem.GUID,
                                        LastUpdatedBy = UserID,
                                        ReceivedOn = DateTimeUtility.DateTimeNow,
                                        ReceivedOnDate = string.Empty,
                                        ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                                        ReceivedOnWebDate = string.Empty,
                                        Room = RoomID,
                                        RoomName = string.Empty,
                                        Updated = DateTimeUtility.DateTimeNow,
                                        UpdatedByName = string.Empty,

                                    };
                                    kitMoveInOutDetails.Add(kitMoveInOutDetail);
                                }
                                else
                                {
                                    var locations = itmLocs.Where(x => x.ConsignedQuantity.GetValueOrDefault(0) + x.CustomerOwnedQuantity.GetValueOrDefault(0) > 0).OrderBy(x => x.BinNumber).ToList();
                                    double pendingQtyToMoveIn = kitQty;

                                    foreach (var loc in locations)
                                    {
                                        if (pendingQtyToMoveIn <= 0)
                                            break;

                                        double tmpKitQty = 0;
                                        if ((loc.ConsignedQuantity.GetValueOrDefault(0) + loc.CustomerOwnedQuantity.GetValueOrDefault(0)) > pendingQtyToMoveIn)
                                        {
                                            tmpKitQty = pendingQtyToMoveIn;
                                        }
                                        else
                                        {
                                            tmpKitQty = (loc.ConsignedQuantity.GetValueOrDefault(0) + loc.CustomerOwnedQuantity.GetValueOrDefault(0));
                                        }
                                        pendingQtyToMoveIn -= tmpKitQty;

                                        kitMoveInOutDetail = new KitMoveInOutDetailDTO()
                                        {
                                            AddedFrom = "Kit Move",
                                            BinID = loc.BinID,
                                            BinNumber = loc.BinNumber,
                                            CompanyID = CompanyID,
                                            ConsignedQuantity = 0,
                                            Created = DateTimeUtility.DateTimeNow,
                                            CreatedBy = UserID,
                                            CreatedByName = "",
                                            CustomerOwnedQuantity = 0,
                                            EditedFrom = "Web",
                                            TotalQuantity = tmpKitQty,
                                            MoveInOut = MoveType,
                                            ItemGUID = kitItem.ItemGUID,
                                            GUID = Guid.Empty,
                                            ID = 0,
                                            IsArchived = false,
                                            IsDeleted = false,
                                            KitDetailGUID = kitItem.GUID,
                                            LastUpdatedBy = UserID,
                                            ReceivedOn = DateTimeUtility.DateTimeNow,
                                            ReceivedOnDate = string.Empty,
                                            ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                                            ReceivedOnWebDate = string.Empty,
                                            Room = RoomID,
                                            RoomName = string.Empty,
                                            Updated = DateTimeUtility.DateTimeNow,
                                            UpdatedByName = string.Empty,

                                        };
                                        kitMoveInOutDetails.Add(kitMoveInOutDetail);
                                    }
                                }
                            }
                            else
                            {
                                var kitMoveInOutDetailDAL = new KitMoveInOutDetailDAL(SessionHelper.EnterPriseDBName);
                                var kitMoveInOutDetailsForMoveOut = kitMoveInOutDetailDAL.GetItemLocationsForMoveOut(kitItem.GUID, kitItem.ItemGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);

                                if (kitMoveInOutDetailsForMoveOut != null && kitMoveInOutDetailsForMoveOut.Any())
                                {
                                    double pendingQtyToMoveOUT = kitQty;
                                    foreach (var inOutDetail in kitMoveInOutDetailsForMoveOut)
                                    {
                                        if (pendingQtyToMoveOUT <= 0)
                                            break;

                                        var tmpMoveOutQty = inOutDetail.TotalQuantity >= pendingQtyToMoveOUT ? pendingQtyToMoveOUT : inOutDetail.TotalQuantity;

                                        kitMoveInOutDetail = new KitMoveInOutDetailDTO()
                                        {
                                            AddedFrom = "Kit Move",
                                            BinID = inOutDetail.BinID,
                                            BinNumber = inOutDetail.BinNumber,
                                            CompanyID = CompanyID,
                                            ConsignedQuantity = 0,
                                            Created = DateTimeUtility.DateTimeNow,
                                            CreatedBy = UserID,
                                            CreatedByName = "",
                                            CustomerOwnedQuantity = 0,
                                            EditedFrom = "Web",
                                            TotalQuantity = tmpMoveOutQty,
                                            MoveInOut = MoveType,
                                            ItemGUID = kitItem.ItemGUID,
                                            GUID = Guid.Empty,
                                            ID = 0,
                                            IsArchived = false,
                                            IsDeleted = false,
                                            KitDetailGUID = kitItem.GUID,
                                            LastUpdatedBy = UserID,
                                            ReceivedOn = DateTimeUtility.DateTimeNow,
                                            ReceivedOnDate = string.Empty,
                                            ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                                            ReceivedOnWebDate = string.Empty,
                                            Room = RoomID,
                                            RoomName = string.Empty,
                                            Updated = DateTimeUtility.DateTimeNow,
                                            UpdatedByName = string.Empty,
                                        };

                                        kitMoveInOutDetails.Add(kitMoveInOutDetail);
                                        pendingQtyToMoveOUT -= tmpMoveOutQty;
                                    }
                                }
                                else
                                {
                                    kitMoveInOutDetail = new KitMoveInOutDetailDTO()
                                    {
                                        AddedFrom = "Kit Move",
                                        BinID = item.DefaultLocation.GetValueOrDefault(0),
                                        BinNumber = item.DefaultLocationName,
                                        CompanyID = CompanyID,
                                        ConsignedQuantity = 0,
                                        Created = DateTimeUtility.DateTimeNow,
                                        CreatedBy = UserID,
                                        CreatedByName = "",
                                        CustomerOwnedQuantity = 0,
                                        EditedFrom = "Web",
                                        TotalQuantity = kitQty,
                                        MoveInOut = MoveType,
                                        ItemGUID = kitItem.ItemGUID,
                                        GUID = Guid.Empty,
                                        ID = 0,
                                        IsArchived = false,
                                        IsDeleted = false,
                                        KitDetailGUID = kitItem.GUID,
                                        LastUpdatedBy = UserID,
                                        ReceivedOn = DateTimeUtility.DateTimeNow,
                                        ReceivedOnDate = string.Empty,
                                        ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                                        ReceivedOnWebDate = string.Empty,
                                        Room = RoomID,
                                        RoomName = string.Empty,
                                        Updated = DateTimeUtility.DateTimeNow,
                                        UpdatedByName = string.Empty,

                                    };
                                    kitMoveInOutDetails.Add(kitMoveInOutDetail);
                                }
                            }
                        }
                    }
                }

                if (string.IsNullOrEmpty(strErrorMsg) && kitMoveInOutDetails.Count > 0)
                {
                    return QtyToMove(kitMoveInOutDetails);
                }

                return Json(new { Status = false, Message = strErrorMsg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                //kitDAL = null;
                kitDetailDAL = null;
                itemDAL = null;
                //kitDTO = null;
                kitDetails = null;
                item = null;
            }
        }

        public JsonResult GetLocationDetailsForMoveIn(double qty, Guid KitGuid, string MoveType, Guid KitDetailGuid,long BinID, string BinName)
        {
            KitDetailDAL kitDetailDAL = null;
            ItemMasterDAL itemDAL = null;
            List<KitDetailDTO> kitDetails = null;
            ItemMasterDTO item = null;
            List<KitMoveInOutDetailDTO> kitMoveInOutDetails = null;
            KitMoveInOutDetailDTO kitMoveInOutDetail = null;
            List<ItemLocationDetailsDTO> itmLocs = null;
            ItemLocationDetailsDAL itmLocDtlDAL = null;
            string strErrorMsg = "";

            try
            {
                if (qty <= 0)
                {
                    return Json(new { Status = false, Message = string.Format(ResCommon.MsgGreaterThanZero, ResMoveMaterial.MoveQuantity) }, JsonRequestBehavior.AllowGet);
                }

                if (KitDetailGuid == null || KitDetailGuid == Guid.Empty)
                {
                    return Json(new { Status = false, Message = ResKitMaster.SelectItemsForBulkMove }, JsonRequestBehavior.AllowGet);
                }

                itemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                itmLocDtlDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                kitDetailDAL = new KitDetailDAL(SessionHelper.EnterPriseDBName);
                kitMoveInOutDetails = new List<KitMoveInOutDetailDTO>();
                kitDetails = kitDetailDAL.GetAllRecordsByKitGUID(KitGuid, RoomID, CompanyID, false, false, false).ToList();

                foreach (var kitItem in kitDetails)
                {
                    if (KitDetailGuid == kitItem.GUID)
                    {
                        item = itemDAL.GetItemWithMasterTableJoins(null, kitItem.ItemGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
                        double totalMoveQty = qty;

                        if (MoveType == "IN")
                        {
                            if (totalMoveQty > item.OnHandQuantity.GetValueOrDefault(0))
                            {
                                strErrorMsg = string.Format(ResKitMaster.ItemHasNotEnoughQty, item.ItemNumber);
                                break;
                            }

                            itmLocs = itmLocDtlDAL.GetCachedDataeVMI(item.GUID, RoomID, CompanyID).ToList();
                            itmLocs = itmLocs.FindAll(x => x.BinID == BinID).ToList();
                            double sumLocQty = itmLocs.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0) + x.CustomerOwnedQuantity.GetValueOrDefault(0));
                            if (totalMoveQty > sumLocQty)
                            {
                                //strErrorMsg = "Item#:" + item.ItemNumber + "'s  Bin:" + item.DefaultLocationName + " has not enough quantity.";
                                strErrorMsg = string.Format(ResKitMoveInOutDetail.ItemBinDoesNotHaveSufficientQtyToMoveIn, item.ItemNumber, BinName);
                                break;
                            }
                        }

                        if (string.IsNullOrEmpty(strErrorMsg))
                        {
                            double kitQty = qty;

                            if (MoveType == "IN")
                            {
                                double sumOfQtyAtDefaultLocation = itmLocs.Where(x => x.BinID == item.DefaultLocation.GetValueOrDefault(0)).ToList().Sum(x => x.ConsignedQuantity.GetValueOrDefault(0) + x.CustomerOwnedQuantity.GetValueOrDefault(0));
                                long? binID = 0;
                                string binNumber = string.Empty;

                                if (totalMoveQty <= sumOfQtyAtDefaultLocation)
                                {
                                    binID = item.DefaultLocation.GetValueOrDefault(0);
                                    binNumber = item.DefaultLocationName;
                                }
                                else
                                {
                                    var binIDs = itmLocs.ToList().OrderBy(e => e.BinNumber).Select(e => e.BinID).Distinct();

                                    foreach (var Id in binIDs)
                                    {
                                        var itemLocations = itmLocs.Where(e => e.BinID == Id).ToList();
                                        var totalQty = itemLocations.Sum(x => x.ConsignedQuantity.GetValueOrDefault(0) + x.CustomerOwnedQuantity.GetValueOrDefault(0));
                                        if (totalQty >= totalMoveQty)
                                        {
                                            binID = Id.Value;
                                            binNumber = itemLocations.FirstOrDefault().BinNumber;
                                            break;
                                        }
                                    }
                                }
                                if (binID.HasValue && binID.Value > 0 && !string.IsNullOrEmpty(binNumber))
                                {
                                    kitMoveInOutDetail = new KitMoveInOutDetailDTO()
                                    {
                                        BinID = binID,
                                        BinNumber = binNumber,
                                        TotalQuantity = kitQty,
                                    };
                                    kitMoveInOutDetails.Add(kitMoveInOutDetail);
                                }
                                else
                                {
                                    Dictionary<long, string> bins = new Dictionary<long, string>();
                                    Dictionary<long, double> qtyAtBin = new Dictionary<long, double>();
                                    var locations = itmLocs.Where(x => x.ConsignedQuantity.GetValueOrDefault(0) + x.CustomerOwnedQuantity.GetValueOrDefault(0) > 0).OrderBy(x => x.BinNumber).ToList();
                                    double pendingQtyToMoveIn = kitQty;

                                    foreach (var loc in locations)
                                    {
                                        if (pendingQtyToMoveIn <= 0)
                                            break;

                                        double tmpKitQty = 0;
                                        if ((loc.ConsignedQuantity.GetValueOrDefault(0) + loc.CustomerOwnedQuantity.GetValueOrDefault(0)) > pendingQtyToMoveIn)
                                        {
                                            tmpKitQty = pendingQtyToMoveIn;
                                        }
                                        else
                                        {
                                            tmpKitQty = (loc.ConsignedQuantity.GetValueOrDefault(0) + loc.CustomerOwnedQuantity.GetValueOrDefault(0));
                                        }
                                        pendingQtyToMoveIn -= tmpKitQty;

                                        if (!bins.ContainsKey(loc.BinID.GetValueOrDefault(0)))
                                        {
                                            bins.Add(loc.BinID.GetValueOrDefault(0), loc.BinNumber);
                                            qtyAtBin.Add(loc.BinID.GetValueOrDefault(0), tmpKitQty);
                                        }
                                        else
                                        {
                                            qtyAtBin[loc.BinID.GetValueOrDefault(0)] = (qtyAtBin[loc.BinID.GetValueOrDefault(0)] + tmpKitQty);
                                        }
                                    }
                                    foreach (KeyValuePair<long, string> entry in bins)
                                    {
                                        kitMoveInOutDetail = new KitMoveInOutDetailDTO()
                                        {
                                            BinID = entry.Key,
                                            BinNumber = entry.Value,
                                            TotalQuantity = qtyAtBin[entry.Key]
                                        };
                                        kitMoveInOutDetails.Add(kitMoveInOutDetail);
                                    }
                                }
                            }
                            else
                            {
                                var kitMoveInOutDetailDAL = new KitMoveInOutDetailDAL(SessionHelper.EnterPriseDBName);
                                //if (!kitMoveInOutDetailDAL.IsMoveInAvailableForKitItem(KitDetailGuid,kitItem.ItemGUID.GetValueOrDefault(Guid.Empty).ToString(), CompanyID, RoomID))
                                //{
                                //   double? TotalQuantity = kitMoveInOutDetailDAL.GetTotalQuantityForMoveOut(kitItem.KitGUID.GetValueOrDefault(Guid.Empty),KitDetailGuid, kitItem.ItemGUID.GetValueOrDefault(Guid.Empty), CompanyID, RoomID);
                                //   kitMoveInOutDetail = new KitMoveInOutDetailDTO()
                                //    {
                                //        BinID = kitItem.DefaultBinID,
                                //        BinNumber = kitItem.DefualtBinName,
                                //        TotalQuantity = TotalQuantity.GetValueOrDefault(0)
                                //   };
                                //    kitMoveInOutDetails.Add(kitMoveInOutDetail);
                                //}
                                //else 
                                //{
                                //var kitMoveInOutDetailsForMoveOut = kitMoveInOutDetailDAL.GetItemLocationsForMoveOut(KitDetailGuid, kitItem.ItemGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
                                var kitMoveInOutDetailsForMoveOut = kitMoveInOutDetailDAL.GetKitMoveInOutDetailForMoveOutPopup(KitDetailGuid, kitItem.ItemGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
                                double tmpQtyAvailableWIP = kitItem.AvailableItemsInWIP.GetValueOrDefault(0);
                                kitMoveInOutDetailsForMoveOut = kitMoveInOutDetailsForMoveOut.FindAll(x => x.BinID == BinID).ToList();
                                foreach (var inOutDetail in kitMoveInOutDetailsForMoveOut)
                                {
                                    inOutDetail.TotalQuantity = kitQty;
                                    inOutDetail.BinID = BinID;
                                    inOutDetail.BinNumber = BinName;
                                    var tmpKitDetail = kitMoveInOutDetails.Where(e => e.BinID == inOutDetail.BinID.GetValueOrDefault(0)).FirstOrDefault();
                                    if (kitItem.AvailableItemsInWIP.GetValueOrDefault(0) <= inOutDetail.TotalQuantity)
                                    {
                                        if (tmpKitDetail != null && tmpKitDetail.BinID.GetValueOrDefault(0) > 0)
                                        {
                                            tmpKitDetail.TotalQuantity += kitItem.AvailableItemsInWIP.GetValueOrDefault(0);//inOutDetail.TotalQuantity;
                                        }
                                        else
                                        {
                                            kitMoveInOutDetail = new KitMoveInOutDetailDTO()
                                            {
                                                BinID = inOutDetail.BinID.GetValueOrDefault(0),
                                                BinNumber = inOutDetail.BinNumber,
                                                TotalQuantity = kitItem.AvailableItemsInWIP.GetValueOrDefault(0)//inOutDetail.TotalQuantity
                                            };
                                            kitMoveInOutDetails.Add(kitMoveInOutDetail);
                                        }
                                        break;
                                    }
                                    else
                                    {
                                        if (tmpQtyAvailableWIP <= 0)
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            if (tmpQtyAvailableWIP <= inOutDetail.TotalQuantity)
                                            {
                                                if (tmpKitDetail != null && tmpKitDetail.BinID.GetValueOrDefault(0) > 0)
                                                {
                                                    tmpKitDetail.TotalQuantity += tmpQtyAvailableWIP;//inOutDetail.TotalQuantity;
                                                }
                                                else
                                                {
                                                    kitMoveInOutDetail = new KitMoveInOutDetailDTO()
                                                    {
                                                        BinID = inOutDetail.BinID.GetValueOrDefault(0),
                                                        BinNumber = inOutDetail.BinNumber,
                                                        TotalQuantity = tmpQtyAvailableWIP//inOutDetail.TotalQuantity
                                                    };
                                                    kitMoveInOutDetails.Add(kitMoveInOutDetail);
                                                }
                                                tmpQtyAvailableWIP = 0;
                                            }
                                            else
                                            {
                                                tmpQtyAvailableWIP = tmpQtyAvailableWIP - inOutDetail.TotalQuantity;

                                                if (tmpKitDetail != null && tmpKitDetail.BinID.GetValueOrDefault(0) > 0)
                                                {
                                                    tmpKitDetail.TotalQuantity += inOutDetail.TotalQuantity;
                                                }
                                                else
                                                {
                                                    kitMoveInOutDetail = new KitMoveInOutDetailDTO()
                                                    {
                                                        BinID = inOutDetail.BinID.GetValueOrDefault(0),
                                                        BinNumber = inOutDetail.BinNumber,
                                                        TotalQuantity = inOutDetail.TotalQuantity
                                                    };
                                                    kitMoveInOutDetails.Add(kitMoveInOutDetail);
                                                }
                                            }

                                        }
                                    }
                                }

                                //}

                            }
                        }
                    }
                }

                if (string.IsNullOrEmpty(strErrorMsg) && kitMoveInOutDetails.Count > 0)
                {
                    return Json(new { Status = true, Data = kitMoveInOutDetails }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { Status = false, Data = "", ErrorMessage = strErrorMsg }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { Status = false, Data = "" }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                //kitDAL = null;
                kitDetailDAL = null;
                itemDAL = null;
                //kitDTO = null;
                kitDetails = null;
                item = null;
                //kitDetail = null;
            }
        }

        /// <summary>
        /// This method is used to validate bulk move out
        /// </summary>
        /// <param name="qty"></param>
        /// <param name="KitGuid"></param>
        /// <param name="MoveType"></param>
        /// <param name="KitDetailGuids"></param>
        /// <returns></returns>
        public JsonResult ValidateMoveOutBulk(double qty, Guid KitGuid, string MoveType, List<Guid> KitDetailGuids, bool isSingleMoveOut = false)
        {
            KitMasterDAL kitDAL = null;
            KitDetailDAL kitDetailDAL = null;
            ItemMasterDAL itemDAL = null;
            KitMasterDTO kitDTO = null;
            List<KitDetailDTO> kitDetails = null;
            ItemMasterDTO item = null;
            List<KitMoveInOutDetailDTO> kitMoveInOutDetails = null;
            KitMoveInOutDetailDTO kitMoveInOutDetail = null;
            ItemLocationDetailsDAL itmLocDtlDAL = null;
            string strErrorMsg = "";
            try
            {
                if (qty <= 0)
                {
                    return Json(new { Status = false, Message = string.Format(ResCommon.MsgGreaterThanZero, ResMoveMaterial.MoveQuantity) }, JsonRequestBehavior.AllowGet);
                }

                if (KitDetailGuids == null || KitDetailGuids.Count <= 0)
                {
                    return Json(new { Status = false, Message = ResKitMaster.SelectItemsForBulkMove }, JsonRequestBehavior.AllowGet);
                }

                itemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                kitDAL = new KitMasterDAL(SessionHelper.EnterPriseDBName);
                itmLocDtlDAL = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                kitDTO = kitDAL.GetRecord(KitGuid.ToString());
                kitDetailDAL = new KitDetailDAL(SessionHelper.EnterPriseDBName);
                kitMoveInOutDetails = new List<KitMoveInOutDetailDTO>();
                kitDetails = kitDetailDAL.GetAllRecordsByKitGUID(KitGuid, RoomID, CompanyID, false, false, false).ToList();

                foreach (var kitItem in kitDetails)
                {
                    if (KitDetailGuids.Contains(kitItem.GUID))
                    {
                        item = itemDAL.GetItemWithMasterTableJoins(null, kitItem.ItemGUID.GetValueOrDefault(Guid.Empty), RoomID, CompanyID);
                        double totalMoveQty = isSingleMoveOut ? qty : (qty * kitItem.QuantityPerKit.GetValueOrDefault(0));

                        if (MoveType == "OUT")
                        {
                            if (totalMoveQty > kitItem.AvailableItemsInWIP.GetValueOrDefault(0))
                            {
                                strErrorMsg = string.Format(ResKitMaster.ItemHasNotEnoughQtyToMoveOut, item.ItemNumber);
                                break;
                            }
                        }

                        if (string.IsNullOrEmpty(strErrorMsg))
                        {
                            double kitQty = isSingleMoveOut ? qty : (qty * kitItem.QuantityPerKit.GetValueOrDefault(0));

                            if (MoveType == "OUT")
                            {
                                if (kitItem.AvailableItemsInWIP.GetValueOrDefault(0) == 0)
                                {
                                    strErrorMsg += string.Format(ResMessage.KitItemAvailableInWIP, item.ItemNumber); //"Item#:" + item.ItemNumber + "'Available ItemsInWIP";
                                    continue;
                                }
                                else if (kitQty > kitItem.AvailableItemsInWIP.GetValueOrDefault(0))
                                {
                                    kitQty = kitItem.AvailableItemsInWIP.GetValueOrDefault(0);
                                }
                            }

                            kitMoveInOutDetail = new KitMoveInOutDetailDTO()
                            {
                                AddedFrom = "Kit Move",
                                BinID = item.DefaultLocation.GetValueOrDefault(0),
                                BinNumber = item.DefaultLocationName,
                                CompanyID = CompanyID,
                                ConsignedQuantity = 0,
                                Created = DateTimeUtility.DateTimeNow,
                                CreatedBy = UserID,
                                CreatedByName = "",
                                CustomerOwnedQuantity = 0,
                                EditedFrom = "Web",
                                TotalQuantity = kitQty,
                                MoveInOut = MoveType,
                                ItemGUID = kitItem.ItemGUID,
                                GUID = Guid.Empty,
                                ID = 0,
                                IsArchived = false,
                                IsDeleted = false,
                                KitDetailGUID = kitItem.GUID,
                                LastUpdatedBy = UserID,
                                ReceivedOn = DateTimeUtility.DateTimeNow,
                                ReceivedOnDate = string.Empty,
                                ReceivedOnWeb = DateTimeUtility.DateTimeNow,
                                ReceivedOnWebDate = string.Empty,
                                Room = RoomID,
                                RoomName = string.Empty,
                                Updated = DateTimeUtility.DateTimeNow,
                                UpdatedByName = string.Empty,
                            };
                            kitMoveInOutDetails.Add(kitMoveInOutDetail);
                        }
                    }
                }

                if (string.IsNullOrEmpty(strErrorMsg) && kitMoveInOutDetails.Count > 0)
                {
                    return Json(new { Status = true, Message = "Success" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { Status = false, Message = strErrorMsg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                kitDAL = null;
                kitDetailDAL = null;
                itemDAL = null;
                kitDTO = null;
                kitDetails = null;
                item = null;
            }
        }

        /// <summary>
        /// Open Popup To Return Item
        /// </summary>
        /// <param name="returnInfo"></param>
        /// <returns></returns>
        public ActionResult OpenPopupToMoveInItem(List<KitItemToMoveDTO> returnInfo)
        {
            //RoomDTO roomDTO = objRoomDAL.GetRoomByIDPlain(RoomID);
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            ItemMasterDAL itemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            string columnList = "ID,RoomName,InventoryConsuptionMethod";
            RoomDTO roomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + RoomID.ToString() + "", "");

            foreach (var item in returnInfo)
            {
                item.InventoryConsuptionMethod = roomDTO.InventoryConsuptionMethod;
                if (item.ValidateOnHandQty && item.MoveType == "IN")
                {
                    var itemdetail = itemMasterDAL.GetItemWithMasterTableJoins(null, item.ItemGUID, RoomID, CompanyID);
                    double totalMoveQty = item.QtyToMoveIn;
                    if (totalMoveQty > itemdetail.OnHandQuantity.GetValueOrDefault(0))
                    {
                        item.hasValidOnHandQty = false;
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return PartialView("_KitItemToMove", returnInfo);
        }

        [HttpPost]
        public JsonResult MoveInSerialsAndLots(List<KitItemToMoveDTO> objItemPullInfo)
        {
            List<KitItemToMoveDTO> oReturn = new List<KitItemToMoveDTO>();

            KitMoveInOutDetailDAL objPullMasterDAL = new KitMoveInOutDetailDAL(SessionHelper.EnterPriseDBName);
            List<ItemPullInfo> oReturnError = new List<ItemPullInfo>();
            BinMasterDAL binDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            long SessionUserId = SessionHelper.UserID;
            var enterpriseId = SessionHelper.EnterPriceID;

            foreach (KitItemToMoveDTO item in objItemPullInfo)
            {

                if (item.lstItemPullDetails != null && item.lstItemPullDetails.Count > 0)
                {
                    item.lstItemPullDetails = item.lstItemPullDetails.Where(x => x.PullQuantity > 0).ToList();
                    if (item.lstItemPullDetails != null && item.lstItemPullDetails.Count > 0)
                    {
                        KitItemToMoveDTO oItemPullInfo = item;
                        if (item.BinID <= 0 && !string.IsNullOrEmpty(item.BinNumber))
                        {
                            item.BinID = binDAL.GetOrInsertBinIDByName(item.ItemGUID, item.BinNumber, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, false) ?? 0;
                        }
                        oItemPullInfo.CompanyId = SessionHelper.CompanyID;
                        oItemPullInfo.RoomId = SessionHelper.RoomID;
                        oItemPullInfo.CreatedBy = SessionHelper.UserID;
                        oItemPullInfo.LastUpdatedBy = SessionHelper.UserID;
                        oItemPullInfo.CanOverrideProjectLimits = true;
                        oItemPullInfo.ErrorList = new List<PullErrorInfo>();
                        oItemPullInfo = ValidateLotAndSerial(oItemPullInfo);

                        if (oItemPullInfo.ErrorList.Count == 0)
                        {
                            oItemPullInfo.EnterpriseId = enterpriseId;
                            oItemPullInfo = objPullMasterDAL.MoveInKitItemQuantity(oItemPullInfo, SessionUserId, enterpriseId);
                        }

                        oReturn.Add(oItemPullInfo);
                    }
                }
            }

            return Json(oReturn);


        }


        [HttpPost]
        public JsonResult MoveOutSerialsAndLots(List<KitItemToMoveDTO> objItemPullInfo)
        {
            List<KitItemToMoveDTO> oReturn = new List<KitItemToMoveDTO>();

            KitMoveInOutDetailDAL objPullMasterDAL = new KitMoveInOutDetailDAL(SessionHelper.EnterPriseDBName);
            List<ItemPullInfo> oReturnError = new List<ItemPullInfo>();
            BinMasterDAL binDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            long SessionUserId = SessionHelper.UserID;
            var enterpriseId = SessionHelper.EnterPriceID;

            foreach (KitItemToMoveDTO item in objItemPullInfo)
            {
                if (item.lstItemPullDetails != null && item.lstItemPullDetails.Count > 0)
                {
                    item.lstItemPullDetails = item.lstItemPullDetails.Where(x => x.PullQuantity > 0).ToList();
                    if (item.lstItemPullDetails != null && item.lstItemPullDetails.Count > 0)
                    {
                        int counter = 0;
                        item.lstItemPullDetails.ForEach(x => x.SrNo = counter++);

                        List<ItemLocationLotSerialDTO> lstLoc = new List<ItemLocationLotSerialDTO>();
                        lstLoc = item.lstItemPullDetails;
                        KitItemToMoveDTO oItemPullInfo = item;
                        //if (item.BinID <= 0 && !string.IsNullOrEmpty(item.BinNumber))
                        //{
                        //    item.BinID = binDAL.GetOrInsertBinIDByName(item.ItemGUID, item.BinNumber, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, false) ?? 0;
                        //}
                        oItemPullInfo.CompanyId = SessionHelper.CompanyID;
                        oItemPullInfo.RoomId = SessionHelper.RoomID;
                        oItemPullInfo.CreatedBy = SessionHelper.UserID;
                        oItemPullInfo.LastUpdatedBy = SessionHelper.UserID;
                        oItemPullInfo.CanOverrideProjectLimits = true;
                        oItemPullInfo.ErrorList = new List<PullErrorInfo>();
                        oItemPullInfo = ValidateLotAndSerialForMoveOut(oItemPullInfo);
                        foreach (var itemLoc in lstLoc)
                        {
                            foreach (var itemPull in oItemPullInfo.lstItemPullDetails.Where(y => y.SrNo == itemLoc.SrNo))
                            {
                                itemPull.Expiration = itemLoc.Expiration;
                            }
                        }
                        if (oItemPullInfo.ErrorList.Count == 0)
                        {
                            oItemPullInfo.EnterpriseId = SessionHelper.EnterPriceID;
                            oItemPullInfo = objPullMasterDAL.MoveOutKitItemQuantity(oItemPullInfo, SessionUserId, enterpriseId);
                        }

                        oReturn.Add(oItemPullInfo);
                    }
                }
            }

            return Json(oReturn);


        }

        private KitItemToMoveDTO ValidateLotAndSerial(KitItemToMoveDTO objItemPullInfo)
        {
            ItemMasterDTO objItem = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, objItemPullInfo.ItemGUID);

            double TotalPulled = 0, Diff = 0, ConsignedTaken = 0, CustownedTaken = 0, TotalCustOwned = 0, TotalConsigned = 0;
            double PullCost = 0;
            //ItemMaster objItem = context.ItemMasters.FirstOrDefault(t => t.GUID == objItemPullInfo.ItemGUID);

            double AvailQty = 0;
            ItemLocationQTYDTO oLocQty = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName).GetRecordByBinItem(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
            if (oLocQty != null)
            {
                AvailQty = (oLocQty.CustomerOwnedQuantity ?? 0) + (oLocQty.ConsignedQuantity ?? 0);
            }

            PullTransactionDAL objPullMasterDAL = new PullTransactionDAL(SessionHelper.EnterPriseDBName);
            //double AvailQty = context.ItemLocationDetails.Where(t => t.ItemGUID == objItemPullInfo.ItemGUID && t.BinID == objItemPullInfo.BinID && (t.IsDeleted ?? false) == false && (t.IsArchived ?? false) == false).Sum(t => (t.CustomerOwnedQuantity ?? 0) + (t.ConsignedQuantity ?? 0));
            List<ItemLocationLotSerialDTO> lstAvailableQty = new List<ItemLocationLotSerialDTO>();
            if (AvailQty >= objItemPullInfo.QtyToMoveIn)
            {
                if (!objItem.LotNumberTracking && !objItem.SerialNumberTracking)
                {
                    lstAvailableQty = objPullMasterDAL.GetItemLocationsLotSerials(objItemPullInfo.ItemGUID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId, objItemPullInfo.BinID, objItemPullInfo.QtyToMoveIn, true);
                    lstAvailableQty.ForEach(il =>
                    {
                        ConsignedTaken = il.ConsignedQuantity ?? 0;
                        CustownedTaken = il.CustomerOwnedQuantity ?? 0;
                        TotalPulled += (ConsignedTaken + CustownedTaken);
                        PullCost += (TotalPulled * (il.Cost ?? 0));
                        Diff = (objItemPullInfo.QtyToMoveIn - TotalPulled);
                        if (Diff < 0)
                        {
                            TotalPulled -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0));
                            PullCost -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) * (il.Cost ?? 0);
                            if (il.IsConsignedLotSerial)
                            {
                                ConsignedTaken = (objItemPullInfo.QtyToMoveIn - TotalPulled);
                            }
                            else
                            {
                                CustownedTaken = (objItemPullInfo.QtyToMoveIn - TotalPulled);
                            }
                            TotalPulled += (ConsignedTaken + CustownedTaken);
                            PullCost += (ConsignedTaken + CustownedTaken) * (il.Cost.GetValueOrDefault(0));

                        }
                        TotalCustOwned += CustownedTaken;
                        TotalConsigned += ConsignedTaken;
                        il.CustomerOwnedTobePulled = CustownedTaken;
                        il.ConsignedTobePulled = ConsignedTaken;
                        il.TotalTobePulled = CustownedTaken + ConsignedTaken;
                        il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((il.Cost ?? 0));
                    });

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
                                                 //where il.LotNumber == t.LotNumber
                                                 group il by new { il.LotNumber, il.ExpirationDate } into grpms
                                                 select new
                                                 {
                                                     CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                     ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                     LotNumber = grpms.Key.LotNumber,
                                                     ExpirationDate = grpms.Key.ExpirationDate
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
                                if (lstLotQty.ExpirationDate != null)
                                {
                                    t.ExpirationDate = lstLotQty.ExpirationDate;
                                }
                                //if (lstLotQty.LotNumber != null)
                                //{
                                //    t.LotNumber = lstLotQty.LotNumber;
                                //}
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
                                PullCost += (TotalPulled * (il.Cost ?? 0));
                                Diff = (objItemPullInfo.QtyToMoveIn - TotalPulled);
                                if (Diff < 0)
                                {
                                    TotalPulled -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0));
                                    PullCost -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) * (il.Cost ?? 0);
                                    if (il.IsConsignedLotSerial)
                                    {
                                        ConsignedTaken = (objItemPullInfo.QtyToMoveIn - TotalPulled);
                                    }
                                    else
                                    {
                                        CustownedTaken = (objItemPullInfo.QtyToMoveIn - TotalPulled);
                                    }
                                    TotalPulled += (ConsignedTaken + CustownedTaken);
                                    PullCost += ((ConsignedTaken + CustownedTaken) * (il.Cost ?? 0));

                                }
                                TotalCustOwned += CustownedTaken;
                                TotalConsigned += ConsignedTaken;
                                il.CustomerOwnedTobePulled = CustownedTaken;
                                il.ConsignedTobePulled = ConsignedTaken;
                                il.TotalTobePulled = CustownedTaken + ConsignedTaken;
                                il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((il.Cost ?? 0));

                            });

                            objItemPullInfo.TotalConsignedTobePulled = TotalConsigned;
                            objItemPullInfo.TotalCustomerOwnedTobePulled = TotalCustOwned;
                            objItemPullInfo.lstItemPullDetails = lstAvailableQty;

                        }
                    }
                    if (objItem.SerialNumberTracking)
                    {
                        lstAvailableQty = objItemPullInfo.lstItemPullDetails;
                        lstAvailableQty.ForEach(t =>
                        {
                            List<ItemLocationDetailsDTO> objItemLocationDetail = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetItemsLocationsSerLotQty(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, string.Empty, t.SerialNumber, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);
                            if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                            {
                                var lstLotQty = (from il in objItemLocationDetail
                                                 //where il.SerialNumber == t.SerialNumber
                                                 group il by new { il.SerialNumber, il.ExpirationDate } into grpms
                                                 select new
                                                 {
                                                     CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                     ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                     LotNumber = grpms.Key.SerialNumber,
                                                     ExpirationDate = grpms.Key.ExpirationDate
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
                                if (lstLotQty.ExpirationDate != null)
                                {
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
                                PullCost += (TotalPulled * (il.Cost ?? 0));
                                Diff = (objItemPullInfo.QtyToMoveIn - TotalPulled);
                                if (Diff < 0)
                                {
                                    TotalPulled -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0));
                                    PullCost -= (((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) * (il.Cost ?? 0));
                                    if (il.IsConsignedLotSerial)
                                    {
                                        ConsignedTaken = (objItemPullInfo.QtyToMoveIn - TotalPulled);
                                    }
                                    else
                                    {
                                        CustownedTaken = (objItemPullInfo.QtyToMoveIn - TotalPulled);
                                    }
                                    TotalPulled += (ConsignedTaken + CustownedTaken);
                                    PullCost += ((ConsignedTaken + CustownedTaken) * (il.Cost ?? 0));

                                }
                                TotalCustOwned += CustownedTaken;
                                TotalConsigned += ConsignedTaken;
                                il.CustomerOwnedTobePulled = CustownedTaken;
                                il.ConsignedTobePulled = ConsignedTaken;
                                il.TotalTobePulled = CustownedTaken + ConsignedTaken;
                                il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((il.Cost ?? 0));

                            });

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

        public ActionResult MoveOutLotSrSelection(JQueryDataTableParamModel param)
        {
            Guid ItemGUID = Guid.Empty;
            Guid ToolGUID = Guid.Empty;
            Guid KitDetailGUID = Guid.Empty;
            Guid KitGUID = Guid.Empty;


            long BinID = 0;
            double PullQuantity = 0;
            double EnteredPullQuantity = 0;
            Guid.TryParse(Convert.ToString(Request["ItemGUID"]), out ItemGUID);
            Guid.TryParse(Convert.ToString(Request["ToolGUID"]), out ToolGUID);
            Guid.TryParse(Convert.ToString(Request["KitDetailGUID"]), out KitDetailGUID);
            Guid.TryParse(Convert.ToString(Request["KitGUID"]), out KitGUID);

            long.TryParse(Convert.ToString(Request["BinID"]), out BinID);
            double.TryParse(Convert.ToString(Request["PullQuantity"]), out PullQuantity);
            string InventoryConsuptionMethod = Convert.ToString(Request["InventoryConsuptionMethod"]);
            string CurrentLoaded = Convert.ToString(Request["CurrentLoaded"]);
            string ViewRight = Convert.ToString(Request["ViewRight"]);
            bool IsDeleteRowMode = Convert.ToBoolean(Request["IsDeleteRowMode"]);
            string binName = Convert.ToString(Request["BinName"]);
            string CurrentDeletedLoaded = Convert.ToString(Request["CurrentDeletedLoaded"]);

            bool IsStagginLocation = false;
            bool isNewBin = false;
            long binIdForSearch = BinID;
            bool isFromBuildBreak = false;
            string[] arrIds = new string[] { };
            EnteredPullQuantity = PullQuantity;

            if (!string.IsNullOrWhiteSpace(CurrentLoaded))
            {
                arrIds = CurrentLoaded.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }

            ItemMasterDTO oItem = null;
            BinMasterDTO objLocDTO = null;

            if (ItemGUID != Guid.Empty)
            {
                oItem = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, ItemGUID);

                if (!string.IsNullOrEmpty(binName))
                {
                    BinMasterDAL objItemLocationDetailsDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                    objLocDTO = objItemLocationDetailsDAL.GetItemBinPlain(ItemGUID, binName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, false);

                    if (objLocDTO != null && objLocDTO.ID > 0)
                    {
                        IsStagginLocation = objLocDTO.IsStagingLocation;
                        isNewBin = (!(objLocDTO.ID == BinID));
                        binIdForSearch = isNewBin ? 0 : binIdForSearch;
                        BinID = objLocDTO.ID;
                    }
                }
                else
                {
                    objLocDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByIDPlain(BinID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (objLocDTO != null && objLocDTO.ID > 0 && objLocDTO.IsStagingLocation)
                        IsStagginLocation = true;
                }

            }


            int TotalRecordCount = 0;
            KitMoveInOutDetailDAL kitMoveInOutDetail = new KitMoveInOutDetailDAL(SessionHelper.EnterPriseDBName);
            List<ItemLocationLotSerialDTO> lstLotSrs = new List<ItemLocationLotSerialDTO>();
            List<ItemLocationLotSerialDTO> retlstLotSrs = new List<ItemLocationLotSerialDTO>();
            List<ItemLocationLotSerialDTO> ItemSerialLocationList = new List<ItemLocationLotSerialDTO>();
            Dictionary<string, double> dicSerialLots = new Dictionary<string, double>();
            List<ItemLocationLotSerialDTO> lstsetPulls = new List<ItemLocationLotSerialDTO>();
            string[] arrItem;

            if (oItem != null && oItem.ItemType == 4)
            {
                ItemLocationLotSerialDTO oLotSr = new ItemLocationLotSerialDTO();
                oLotSr.BinID = BinID;
                oLotSr.ID = BinID;
                oLotSr.ItemGUID = ItemGUID;
                oLotSr.LotOrSerailNumber = string.Empty;
                oLotSr.Expiration = string.Empty;
                oLotSr.BinNumber = string.Empty;
                oLotSr.PullQuantity = oItem.DefaultPullQuantity.GetValueOrDefault(0) > PullQuantity ? oItem.DefaultPullQuantity.GetValueOrDefault(0) : PullQuantity;
                oLotSr.LotSerialQuantity = PullQuantity;//oItem.DefaultPullQuantity.GetValueOrDefault(0);

                retlstLotSrs.Add(oLotSr);
            }
            else
            {
                if (arrIds.Count() > 0)
                {
                    string[] arrSerialLots = new string[arrIds.Count()];
                    string[] separatingStrings = { "@@" };
                    for (int i = 0; i < arrIds.Count(); i++)
                    {
                        if ((oItem.SerialNumberTracking && !oItem.DateCodeTracking)
                            || (oItem.LotNumberTracking && !oItem.DateCodeTracking)
                            || !oItem.DateCodeTracking)
                        {
                            //string[] arrItem = arrIds[i].Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                            arrItem = new string[2];
                            arrItem[0] = arrIds[i].Substring(0, arrIds[i].LastIndexOf("@@"));
                            arrItem[1] = arrIds[i].Replace(arrItem[0] + "@@", "");
                            if (arrItem.Length > 1)
                            {
                                arrSerialLots[i] = arrItem[0];
                                dicSerialLots.Add(arrItem[0], Convert.ToDouble(arrItem[1]));

                                ItemLocationLotSerialDTO objpull = new ItemLocationLotSerialDTO();
                                if (oItem.SerialNumberTracking)
                                    objpull.SerialNumber = arrItem[0];
                                if (oItem.LotNumberTracking)
                                    objpull.LotNumber = arrItem[0];
                                objpull.PullQuantity = Convert.ToDouble(arrItem[1]);

                                lstsetPulls.Add(objpull);

                            }
                        }
                        else if ((oItem.SerialNumberTracking && oItem.DateCodeTracking)
                            || (oItem.LotNumberTracking && oItem.DateCodeTracking))
                        {
                            arrItem = arrIds[i].Split(separatingStrings,StringSplitOptions.RemoveEmptyEntries);
                            if (arrItem.Length > 1)
                            {
                                arrSerialLots[i] = arrItem[0] + "_" + arrItem[1];
                                dicSerialLots.Add(arrItem[0], Convert.ToDouble(arrItem[2]));

                                ItemLocationLotSerialDTO objpull = new ItemLocationLotSerialDTO();
                                if (oItem.SerialNumberTracking)
                                    objpull.SerialNumber = arrItem[0];
                                if (oItem.LotNumberTracking)
                                    objpull.LotNumber = arrItem[0];
                                if (oItem.DateCodeTracking)
                                {
                                    DateTime ExpirationDateUTC = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(arrItem[1], SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone);
                                    objpull.ExpirationDate = ExpirationDateUTC;
                                    objpull.Expiration = Convert.ToString(arrItem[1]);
                                }
                                objpull.PullQuantity = Convert.ToDouble(arrItem[2]);

                                lstsetPulls.Add(objpull);

                            }
                        }
                        else if (!oItem.SerialNumberTracking && !oItem.DateCodeTracking && oItem.DateCodeTracking)
                        {
                            arrItem = arrIds[i].Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
                            if (arrItem.Length > 1)
                            {
                                arrSerialLots[i] = arrItem[0];
                                dicSerialLots.Add(arrItem[0], Convert.ToDouble(arrItem[1]));

                                ItemLocationLotSerialDTO objpull = new ItemLocationLotSerialDTO();
                                if (oItem.SerialNumberTracking)
                                    objpull.SerialNumber = arrItem[0];
                                if (oItem.LotNumberTracking)
                                    objpull.LotNumber = arrItem[0];
                                if (oItem.DateCodeTracking)
                                {
                                    DateTime ExpirationDateUTC = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(arrItem[0], SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone);
                                    objpull.ExpirationDate = ExpirationDateUTC;
                                    objpull.Expiration = Convert.ToString(arrItem[0]);
                                }
                                objpull.PullQuantity = Convert.ToDouble(arrItem[1]);

                                lstsetPulls.Add(objpull);
                            }
                        }
                        else
                        {
                            arrItem = arrIds[i].Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries);
                            if (arrItem.Length > 1)
                            {
                                arrSerialLots[i] = arrItem[0];
                                dicSerialLots.Add(arrItem[0], Convert.ToDouble(arrItem[1]));

                                ItemLocationLotSerialDTO objpull = new ItemLocationLotSerialDTO();
                                if (oItem.SerialNumberTracking)
                                    objpull.SerialNumber = arrItem[0];
                                if (oItem.LotNumberTracking)
                                    objpull.LotNumber = arrItem[0];
                                if (oItem.DateCodeTracking)
                                {
                                    DateTime ExpirationDateUTC = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(arrItem[0], SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone);
                                    objpull.ExpirationDate = ExpirationDateUTC;
                                    objpull.Expiration = Convert.ToString(arrItem[0]);
                                }
                                objpull.PullQuantity = Convert.ToDouble(arrItem[1]);

                                lstsetPulls.Add(objpull);
                            }
                        }
                    }

                    //lstLotSrs = kitMoveInOutDetail.GetItemLocationsWithLotSerialsForMoveOut(KitDetailGUID, ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, binIdForSearch, PullQuantity, "0", string.Empty, (IsStagginLocation ? "1" : "0"));

                    //if (!lstLotSrs.Any() && KitGUID != Guid.Empty)
                    //{
                    //    lstLotSrs = kitMoveInOutDetail.GetItemLocationsWithLotSerialsForMoveOut(KitGUID, ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    //    isFromBuildBreak = true;
                    //}

                    retlstLotSrs = lstLotSrs.Where(t =>
                        (
                            (
                                arrSerialLots.Contains(t.LotOrSerailNumber, StringComparer.OrdinalIgnoreCase)
                                && (oItem.SerialNumberTracking || oItem.LotNumberTracking)
                                && !oItem.DateCodeTracking)
                        ||
                            (
                                arrSerialLots.Contains(t.SerialLotExpirationcombin)
                                && (oItem.SerialNumberTracking || oItem.LotNumberTracking)
                                && oItem.DateCodeTracking)
                        ||
                            (
                                arrSerialLots.Contains(t.SerialLotExpirationcombin)
                                && (!oItem.SerialNumberTracking && !oItem.LotNumberTracking)
                                && oItem.DateCodeTracking)
                        || (arrSerialLots.Contains(t.BinNumber) && !oItem.SerialNumberTracking && !oItem.LotNumberTracking && !oItem.DateCodeTracking))).ToList();

                    if (!IsDeleteRowMode)
                    {
                        if (ViewRight == "NoRight" && (oItem.SerialNumberTracking || oItem.LotNumberTracking))
                        {
                            ItemLocationLotSerialDTO oLotSr = new ItemLocationLotSerialDTO();
                            oLotSr.BinID = BinID;
                            oLotSr.ID = BinID;
                            oLotSr.ItemGUID = ItemGUID;
                            oLotSr.LotOrSerailNumber = string.Empty;
                            oLotSr.Expiration = string.Empty;
                            oLotSr.BinNumber = string.Empty;

                            if (objLocDTO != null && objLocDTO.ID > 0)
                            {
                                oLotSr.BinNumber = objLocDTO.BinNumber;
                            }
                            if (oItem.SerialNumberTracking)
                            {
                                oLotSr.PullQuantity = 1;
                            }
                            oLotSr.LotNumberTracking = oItem.LotNumberTracking;
                            oLotSr.SerialNumberTracking = oItem.SerialNumberTracking;
                            oLotSr.DateCodeTracking = oItem.DateCodeTracking;
                            retlstLotSrs.Add(oLotSr);
                        }
                        else
                        {
                            retlstLotSrs =
                                retlstLotSrs.Union
                                (
                                    lstLotSrs.Where(t =>
                                  (
                                        (
                                            !arrSerialLots.Contains(t.LotOrSerailNumber, StringComparer.OrdinalIgnoreCase)
                                            && (oItem.SerialNumberTracking || oItem.LotNumberTracking)
                                            && !oItem.DateCodeTracking
                                        )
                                    ||
                                        (
                                            !arrSerialLots.Contains(t.SerialLotExpirationcombin)
                                            && (oItem.SerialNumberTracking || oItem.LotNumberTracking)
                                            && oItem.DateCodeTracking
                                        )
                                    ||
                                        (
                                            !arrSerialLots.Contains(t.SerialLotExpirationcombin)
                                            && (!oItem.SerialNumberTracking && !oItem.LotNumberTracking)
                                            && oItem.DateCodeTracking
                                        )
                                    ||
                                        (
                                            !arrSerialLots.Contains(t.BinNumber)
                                            && !oItem.SerialNumberTracking
                                            && !oItem.LotNumberTracking
                                            && !oItem.DateCodeTracking
                                         )
                                 )).Take(1)
                              ).ToList();

                        }
                    }
                }
                else
                {
                    if (ViewRight == "NoRight" && (oItem.SerialNumberTracking || oItem.LotNumberTracking))
                    {
                        ItemLocationLotSerialDTO oLotSr = new ItemLocationLotSerialDTO();
                        oLotSr.BinID = BinID;
                        oLotSr.ID = BinID;
                        oLotSr.ItemGUID = ItemGUID;
                        oLotSr.LotOrSerailNumber = string.Empty;
                        oLotSr.Expiration = string.Empty;
                        oLotSr.BinNumber = string.Empty;

                        if (objLocDTO != null && objLocDTO.ID > 0)
                        {
                            oLotSr.BinNumber = objLocDTO.BinNumber;
                        }
                        if (oItem.SerialNumberTracking)
                        {
                            oLotSr.PullQuantity = 1;

                        }
                        oLotSr.LotNumberTracking = oItem.LotNumberTracking;
                        oLotSr.SerialNumberTracking = oItem.SerialNumberTracking;
                        oLotSr.DateCodeTracking = oItem.DateCodeTracking;

                        retlstLotSrs.Add(oLotSr);
                    }
                    //else
                    //{
                    //    retlstLotSrs = kitMoveInOutDetail.GetItemLocationsWithLotSerialsForMoveOut(KitDetailGUID, ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, binIdForSearch, PullQuantity, "1", string.Empty, (IsStagginLocation ? "1" : "0"));
                    //    if (!retlstLotSrs.Any() && KitGUID != Guid.Empty || (oItem.SerialNumberTracking && !retlstLotSrs.Any()) || (oItem.SerialNumberTracking && retlstLotSrs.Count < PullQuantity))
                    //    {
                    //        retlstLotSrs = kitMoveInOutDetail.GetItemLocationsWithLotSerialsForMoveOutKitBreak(KitDetailGUID, ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, binIdForSearch, PullQuantity, "1", string.Empty, (IsStagginLocation ? "1" : "0"));
                    //        if (!retlstLotSrs.Any())
                    //        {
                    //            retlstLotSrs = kitMoveInOutDetail.GetItemLocationsWithLotSerialsForMoveOut(KitGUID, ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    //            isFromBuildBreak = true;
                    //        }
                    //    }
                    //}
                }
                if (oItem.SerialNumberTracking != true && oItem.DateCodeTracking)
                {
                    retlstLotSrs = retlstLotSrs.GroupBy(t => new { t.BinNumber, t.ExpirationDate }).Select(grp => grp.First()).ToList();
                }

                foreach (var item in retlstLotSrs)
                {
                    if (dicSerialLots.ContainsKey(item.LotOrSerailNumber) && (oItem.SerialNumberTracking || oItem.LotNumberTracking))
                    {
                        double value = dicSerialLots[item.LotOrSerailNumber];
                        item.PullQuantity = value;
                        PullQuantity -= item.PullQuantity;
                    }
                    else if (dicSerialLots.ContainsKey(item.Expiration ?? string.Empty) && oItem.DateCodeTracking)
                    {
                        double value = dicSerialLots[item.Expiration];
                        item.PullQuantity = value;
                        PullQuantity -= item.PullQuantity;
                    }
                    else if (dicSerialLots.ContainsKey(item.BinNumber) && !oItem.SerialNumberTracking && !oItem.LotNumberTracking && !oItem.DateCodeTracking)
                    {
                        double value = dicSerialLots[item.BinNumber];
                        item.PullQuantity = value;
                        PullQuantity -= item.PullQuantity;
                    }
                    else if (item.PullQuantity <= PullQuantity)
                    {
                        PullQuantity -= item.PullQuantity;
                    }
                    else if (PullQuantity >= 0)
                    {
                        if (item.SerialNumberTracking)
                        {
                            break;
                        }
                        item.PullQuantity = PullQuantity;
                        PullQuantity = 0;
                    }
                    else
                    {
                        item.PullQuantity = 0;
                    }
                    if (item.ExpirationDate != null && item.ExpirationDate.HasValue && item.ExpirationDate.Value != DateTime.MinValue)
                    {
                        item.Expiration = FnCommon.ConvertDateByTimeZone(item.ExpirationDate.Value, false, true);
                    }
                    if (item.ReceivedDate != null && item.ReceivedDate.HasValue && item.ReceivedDate.Value != DateTime.MinValue)
                    {
                        item.Received = FnCommon.ConvertDateByTimeZone(item.ReceivedDate.Value, true, true);
                    }
                    if (item.PullQuantity > 0)
                        item.IsSelected = true;

                    if (isNewBin || isFromBuildBreak)
                    {
                        item.BinID = BinID;
                        item.BinNumber = binName;
                    }
                    if(item.BinID != BinID)
                    {
                        item.BinID = BinID;
                        item.BinNumber = binName;
                    }
                    ItemSerialLocationList.Add(item);
                }
            }

            if (oItem.SerialNumberTracking)
            {
                retlstLotSrs = ItemSerialLocationList;
            }

            if (!(ViewRight == "NoRight" && (oItem.SerialNumberTracking || oItem.LotNumberTracking)))
                retlstLotSrs = retlstLotSrs.Where(x => x.PullQuantity > 0).ToList();

            if (PullQuantity > 0)
            {
                if (lstsetPulls != null && lstsetPulls.Count > 0)
                {
                    if (oItem.SerialNumberTracking && oItem.DateCodeTracking)
                    {
                        lstsetPulls = lstsetPulls.Distinct().Where(x => !retlstLotSrs.Any(e => e.SerialNumber == x.SerialNumber && Convert.ToDateTime(e.ExpirationDate).Date == Convert.ToDateTime(x.ExpirationDate).Date)).ToList();
                    }
                    else if (oItem.LotNumberTracking && oItem.DateCodeTracking)
                    {
                        lstsetPulls = lstsetPulls.Distinct().Where(x => !retlstLotSrs.Any(e => e.LotNumber == x.LotNumber && Convert.ToDateTime(e.ExpirationDate).Date == Convert.ToDateTime(x.ExpirationDate).Date)).ToList();
                    }
                    else if (oItem.SerialNumberTracking)
                    {
                        lstsetPulls = lstsetPulls.Distinct().Where(x => !retlstLotSrs.Any(e => e.SerialNumber == x.SerialNumber)).ToList();
                    }
                    else if (oItem.LotNumberTracking)
                    {
                        lstsetPulls = lstsetPulls.Distinct().Where(x => !retlstLotSrs.Any(e => e.LotNumber == x.LotNumber)).ToList();
                    }
                    else if (oItem.DateCodeTracking)
                    {
                        lstsetPulls = lstsetPulls.Distinct().Where(x => !retlstLotSrs.Any(e => Convert.ToDateTime(e.ExpirationDate).Date == Convert.ToDateTime(x.ExpirationDate).Date)).ToList();
                    }
                }
                for (int i = 0; i < lstsetPulls.Count(); i++)
                {
                    PullQuantity -= lstsetPulls[i].PullQuantity;

                    ItemLocationLotSerialDTO oLotSr = new ItemLocationLotSerialDTO();
                    oLotSr.BinID = BinID;
                    oLotSr.ID = BinID;
                    oLotSr.ItemGUID = ItemGUID;
                    oLotSr.LotOrSerailNumber = (oItem.SerialNumberTracking ? lstsetPulls[i].SerialNumber : lstsetPulls[i].LotNumber);
                    oLotSr.Expiration = (oItem.DateCodeTracking ? lstsetPulls[i].Expiration : string.Empty);
                    oLotSr.Received = FnCommon.ConvertDateByTimeZone(DateTimeUtility.DateTimeNow, true, true);
                    oLotSr.BinNumber = string.Empty;
                    if (objLocDTO != null && objLocDTO.ID > 0)
                    {
                        oLotSr.BinNumber = objLocDTO.BinNumber;
                    }
                    if (oItem.SerialNumberTracking)
                    {
                        oLotSr.PullQuantity = 1;
                    }
                    else
                    {
                        oLotSr.PullQuantity = lstsetPulls[i].PullQuantity;
                    }
                    oLotSr.LotNumberTracking = oItem.LotNumberTracking;
                    oLotSr.SerialNumberTracking = oItem.SerialNumberTracking;
                    oLotSr.DateCodeTracking = oItem.DateCodeTracking;
                    retlstLotSrs.Add(oLotSr);
                }
            }

            //if (CurrentDeletedLoaded != "")
            //{
            //    string[] arrDeletedIds = new string[] { };
            //    if (!string.IsNullOrWhiteSpace(CurrentDeletedLoaded))
            //    {
            //        arrDeletedIds = CurrentDeletedLoaded.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            //        if (arrDeletedIds.Count() > 0)
            //        {
            //            string[] arrSerialLots = new string[arrDeletedIds.Count()];
            //            for (int i = 0; i < arrDeletedIds.Count(); i++)
            //            {
            //                PullQuantity += 1;
            //                if ((oItem.SerialNumberTracking && !oItem.DateCodeTracking)
            //                    || (oItem.LotNumberTracking && !oItem.DateCodeTracking)
            //                    || !oItem.DateCodeTracking)
            //                {
            //                    arrItem = new string[2];
            //                    arrItem[0] = arrDeletedIds[i].Substring(0, arrDeletedIds[i].LastIndexOf("_"));
            //                    arrItem[1] = arrDeletedIds[i].Replace(arrItem[0] + "_", "");
            //                    if (arrItem.Length > 1)
            //                    {
            //                        if (oItem.SerialNumberTracking)
            //                        {
            //                            retlstLotSrs.RemoveAll(x => x.SerialNumber == arrItem[0]);
            //                        }
            //                        if (oItem.LotNumberTracking)
            //                        {
            //                            retlstLotSrs.RemoveAll(x => x.LotNumber == arrItem[0]);
            //                        }
            //                    }
            //                }
            //                else if ((oItem.SerialNumberTracking && oItem.DateCodeTracking)
            //                    || (oItem.LotNumberTracking && oItem.DateCodeTracking))
            //                {
            //                    arrItem = arrDeletedIds[i].Split('_');
            //                    if (arrItem.Length > 1)
            //                    {
            //                        if (oItem.SerialNumberTracking && oItem.DateCodeTracking)
            //                        {
            //                            if (!string.IsNullOrWhiteSpace(arrItem[1]))
            //                            {
            //                                DateTime DtExpirationDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(arrItem[1], SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone);
            //                                retlstLotSrs.RemoveAll(x => x.SerialNumber == arrItem[0] && Convert.ToDateTime(x.ExpirationDate).Date == DtExpirationDate.Date);
            //                            }
            //                            else
            //                                retlstLotSrs.RemoveAll(x => x.SerialNumber == arrItem[0]);
            //                        }
            //                        if (oItem.LotNumberTracking && oItem.DateCodeTracking)
            //                        {
            //                            if (!string.IsNullOrWhiteSpace(arrItem[1]))
            //                            {
            //                                DateTime DtExpirationDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(arrItem[1], SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone);
            //                                retlstLotSrs.RemoveAll(x => x.LotNumber == arrItem[0] && Convert.ToDateTime(x.ExpirationDate).Date == DtExpirationDate.Date);
            //                            }
            //                            else
            //                            {
            //                                retlstLotSrs.RemoveAll(x => x.LotNumber == arrItem[0]);
            //                            }
            //                        }
            //                    }
            //                }
            //                else if (!oItem.SerialNumberTracking && !oItem.DateCodeTracking && oItem.DateCodeTracking)
            //                {
            //                    arrItem = arrDeletedIds[i].Split('_');
            //                    if (arrItem.Length > 1)
            //                    {
            //                        if (oItem.DateCodeTracking)
            //                        {
            //                            if (!string.IsNullOrWhiteSpace(arrItem[0]))
            //                            {
            //                                DateTime DtExpirationDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(arrItem[0], SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone);
            //                                retlstLotSrs.RemoveAll(x => Convert.ToDateTime(x.ExpirationDate).Date == DtExpirationDate.Date);
            //                            }
            //                        }
            //                    }
            //                }
            //                else
            //                {
            //                    arrItem = arrDeletedIds[i].Split('_');
            //                    if (arrItem.Length > 1)
            //                    {
            //                        if (oItem.SerialNumberTracking)
            //                        {
            //                            retlstLotSrs.RemoveAll(x => x.SerialNumber == arrItem[0]);
            //                        }
            //                        if (oItem.LotNumberTracking)
            //                        {
            //                            retlstLotSrs.RemoveAll(x => x.LotNumber == arrItem[0]);
            //                        }
            //                        if (oItem.DateCodeTracking)
            //                        {
            //                            if (!string.IsNullOrWhiteSpace(arrItem[0]))
            //                            {
            //                                DateTime DtExpirationDate = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(arrItem[0], SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone);
            //                                retlstLotSrs.RemoveAll(x => Convert.ToDateTime(x.ExpirationDate).Date == DtExpirationDate.Date);
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            PullQuantity = EnteredPullQuantity - retlstLotSrs.Sum(x => x.PullQuantity);

            if (PullQuantity > 0)
            {
                if (oItem.SerialNumberTracking)
                {
                    for (int i = 0; i < PullQuantity; i++)
                    {
                        ItemLocationLotSerialDTO oLotSr = new ItemLocationLotSerialDTO();
                        oLotSr.BinID = BinID;
                        oLotSr.ID = BinID;
                        oLotSr.ItemGUID = ItemGUID;
                        oLotSr.LotOrSerailNumber = string.Empty;
                        oLotSr.Expiration = string.Empty;
                        oLotSr.Received = FnCommon.ConvertDateByTimeZone(DateTimeUtility.DateTimeNow, true, true);
                        oLotSr.BinNumber = string.Empty;

                        if (objLocDTO != null && objLocDTO.ID > 0)
                        {
                            oLotSr.BinNumber = objLocDTO.BinNumber;
                        }
                        if (oItem.SerialNumberTracking)
                        {
                            oLotSr.PullQuantity = 1;
                        }
                        oLotSr.LotNumberTracking = oItem.LotNumberTracking;
                        oLotSr.SerialNumberTracking = oItem.SerialNumberTracking;
                        oLotSr.DateCodeTracking = oItem.DateCodeTracking;

                        retlstLotSrs.Add(oLotSr);
                    }
                }
                else
                {
                    ItemLocationLotSerialDTO oLotSr = new ItemLocationLotSerialDTO();
                    oLotSr.BinID = BinID;
                    oLotSr.ID = BinID;
                    oLotSr.ItemGUID = ItemGUID;
                    oLotSr.LotOrSerailNumber = string.Empty;
                    oLotSr.Expiration = string.Empty;
                    oLotSr.Received = FnCommon.ConvertDateByTimeZone(DateTimeUtility.DateTimeNow, true, true);
                    oLotSr.BinNumber = string.Empty;

                    if (objLocDTO != null && objLocDTO.ID > 0)
                    {
                        oLotSr.BinNumber = objLocDTO.BinNumber;
                    }
                    oLotSr.PullQuantity = PullQuantity;
                    oLotSr.LotNumberTracking = oItem.LotNumberTracking;
                    oLotSr.SerialNumberTracking = oItem.SerialNumberTracking;
                    oLotSr.DateCodeTracking = oItem.DateCodeTracking;

                    retlstLotSrs.Add(oLotSr);
                }
            }

            retlstLotSrs.ForEach(x => x.KitDetailGUID = Guid.NewGuid());

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = retlstLotSrs
            }, JsonRequestBehavior.AllowGet);
        }

        private KitItemToMoveDTO ValidateLotAndSerialForMoveOut(KitItemToMoveDTO objItemPullInfo)
        {
            ItemMasterDTO objItem = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, objItemPullInfo.ItemGUID);
            KitMoveInOutDetailDAL kitMoveInOutDetail = new KitMoveInOutDetailDAL(SessionHelper.EnterPriseDBName);

            double TotalPulled = 0, Diff = 0, ConsignedTaken = 0, CustownedTaken = 0, TotalCustOwned = 0, TotalConsigned = 0;
            double PullCost = 0;
            //double AvailQty = 0;
            //ItemLocationQTYDTO oLocQty = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName).GetRecordByBinItem(objItemPullInfo.ItemGUID, objItemPullInfo.BinID, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);

            //if (oLocQty != null)
            //{
            //    AvailQty = (oLocQty.CustomerOwnedQuantity ?? 0) + (oLocQty.ConsignedQuantity ?? 0);
            //}

            PullTransactionDAL objPullMasterDAL = new PullTransactionDAL(SessionHelper.EnterPriseDBName);
            List<ItemLocationLotSerialDTO> lstAvailableQty = new List<ItemLocationLotSerialDTO>();
            if (!objItem.LotNumberTracking && !objItem.SerialNumberTracking)
            {
                bool isLotMovedInQtyNotAvailable = false;
                lstAvailableQty = objItemPullInfo.lstItemPullDetails;
                bool isExpiratoindateEmpty = false;
                lstAvailableQty.ForEach(t =>
                {
                   if (t.DateCodeTracking && string.IsNullOrEmpty(t.Expiration))
                   {
                      isExpiratoindateEmpty = true;
                      t.ValidationMessage = ResPullMaster.EnterValidExpirationDate;
                    }
                    else
                    {
                        var objItemLocationDetail = kitMoveInOutDetail.GetItemLocationsWithLotSerialsForMoveOut(objItemPullInfo.KitDetailGUID, objItemPullInfo.ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, 0, objItemPullInfo.QtyToMoveIn, "0", string.Empty, "0");
                        if (!objItemLocationDetail.Any())
                        {
                            objItemLocationDetail = kitMoveInOutDetail.GetItemLocationsWithLotSerialsForMoveOutKitBreak(objItemPullInfo.KitDetailGUID, objItemPullInfo.ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, 0, objItemPullInfo.QtyToMoveIn, "0", string.Empty, "0");
                            if (!objItemLocationDetail.Any())
                            {
                                objItemLocationDetail = kitMoveInOutDetail.GetItemLocationsWithLotSerialsForMoveOut(objItemPullInfo.KitGuid, objItemPullInfo.ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                            }
                        }
                        if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                        {
                            var lstLotQty = (from il in objItemLocationDetail
                                                 //where il.LotNumber == t.LotNumber
                                             group il by new { il.LotNumber, il.ExpirationDate } into grpms
                                             select new
                                             {
                                                 CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                 ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                 LotNumber = grpms.Key.LotNumber,
                                                 ExpirationDate = grpms.Key.ExpirationDate
                                             }).FirstOrDefault();

                            if (lstLotQty != null)
                            {
                                //if (t.PullQuantity > (lstLotQty.CustomerOwnedQuantity) + ((lstLotQty.ConsignedQuantity)))
                                //{
                                //    t.ValidationMessage = ResPullMaster.msgInvalidQuantityLot;
                                //}
                                //else
                                //{
                                if ((lstLotQty.CustomerOwnedQuantity) > 0)
                                    t.CustomerOwnedQuantity = t.PullQuantity;
                                else
                                    t.ConsignedQuantity = t.PullQuantity;
                                //}

                                if (lstLotQty.ExpirationDate != null)
                                {
                                    t.ExpirationDate = lstLotQty.ExpirationDate;
                                }
                            }
                            else
                            {
                                //t.ValidationMessage = "Lot number not available in Move In.";
                                //isLotNotMovedIn = true;

                                t.ValidationMessage = ResKitMaster.EnoughMoveInQtyNotAvailable;
                                isLotMovedInQtyNotAvailable = true;

                                //if (!objItem.Consignment)
                                //    t.CustomerOwnedQuantity = t.PullQuantity;
                                //else
                                //    t.ConsignedQuantity = t.PullQuantity;
                            }
                        }
                        else
                        {
                            t.ValidationMessage = ResPullMaster.msgInvalidLot;
                        }

                    }
                 
                });

                    //lstAvailableQty = kitMoveInOutDetail.GetItemLocationsWithLotSerialsForMoveOut(objItemPullInfo.KitDetailGUID, objItemPullInfo.ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, objItemPullInfo.BinID, objItemPullInfo.QtyToMoveIn, "0", string.Empty, "0");
                    
                
                //objItemPullInfo.lstItemPullDetails.ForEach(t =>
                //{
                //    if (t.DateCodeTracking && string.IsNullOrEmpty(t.Expiration))
                //    {
                //        isExpiratoindateEmpty = true;
                //        t.ValidationMessage = ResPullMaster.EnterValidExpirationDate;
                //    }
                //});
                if (isExpiratoindateEmpty)
                {
                    objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "6", ErrorMessage = objItem.ItemNumber + ": " + ResPullMaster.EnterValidExpirationDate });
                }
                else
                {
                    lstAvailableQty.ForEach(il =>
                    {
                        ConsignedTaken = il.ConsignedQuantity ?? 0;
                        CustownedTaken = il.CustomerOwnedQuantity ?? 0;
                        TotalPulled += (ConsignedTaken + CustownedTaken);
                        PullCost += (TotalPulled * (il.Cost ?? 0));
                        Diff = (objItemPullInfo.QtyToMoveIn - TotalPulled);
                        if (Diff < 0)
                        {
                            TotalPulled -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0));
                            PullCost -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) * (il.Cost ?? 0);
                            if (il.IsConsignedLotSerial)
                            {
                                ConsignedTaken = (objItemPullInfo.QtyToMoveIn - TotalPulled);
                            }
                            else
                            {
                                CustownedTaken = (objItemPullInfo.QtyToMoveIn - TotalPulled);
                            }
                            TotalPulled += (ConsignedTaken + CustownedTaken);
                            PullCost += (ConsignedTaken + CustownedTaken) * (il.Cost.GetValueOrDefault(0));

                        }
                        TotalCustOwned += CustownedTaken;
                        TotalConsigned += ConsignedTaken;
                        il.CustomerOwnedTobePulled = CustownedTaken;
                        il.ConsignedTobePulled = ConsignedTaken;
                        il.TotalTobePulled = CustownedTaken + ConsignedTaken;
                        il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((il.Cost ?? 0));
                        il.BinID = objItemPullInfo.BinID;
                        //il.Expiration = objItemPullInfo.lstItemPullDetails.Where(x => x.ItemGUID == il.ItemGUID).Select(x => x.Expiration).FirstOrDefault();

                    });


                    objItemPullInfo.TotalConsignedTobePulled = TotalConsigned;
                    objItemPullInfo.TotalCustomerOwnedTobePulled = TotalCustOwned;
                    objItemPullInfo.lstItemPullDetails = lstAvailableQty;
                }


            }
            else
            {
                if (objItem.LotNumberTracking)
                {
                    //bool isLotNotMovedIn = false;
                    bool isLotMovedInQtyNotAvailable = false;
                    bool isLotNumberEmpty = false;
                    bool isExpiratoindateEmpty = false;
                    lstAvailableQty = objItemPullInfo.lstItemPullDetails;
                    lstAvailableQty.ForEach(t =>
                    {
                        if (string.IsNullOrEmpty(t.LotNumber))
                        {
                            isLotNumberEmpty = true;
                            t.ValidationMessage = ResKitMaster.LotNumberCantBeEmpty;
                        }
                        else if (t.DateCodeTracking && string.IsNullOrEmpty(t.Expiration))
                        {
                            isExpiratoindateEmpty = true;
                            t.ValidationMessage = ResPullMaster.EnterValidExpirationDate;
                        }
                        else
                        {
                            //var objItemLocationDetail = kitMoveInOutDetail.GetItemLocationsWithLotSerialsForMoveOut(objItemPullInfo.KitDetailGUID, objItemPullInfo.ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, objItemPullInfo.BinID, objItemPullInfo.QtyToMoveIn, "0", t.LotNumber, "0");
                            var objItemLocationDetail = kitMoveInOutDetail.GetItemLocationsWithLotSerialsForMoveOut(objItemPullInfo.KitDetailGUID, objItemPullInfo.ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, 0, objItemPullInfo.QtyToMoveIn, "0", string.Empty, "0");
                            if (!objItemLocationDetail.Any() && objItemPullInfo.KitGuid != Guid.Empty)
                            {
                                objItemLocationDetail = kitMoveInOutDetail.GetItemLocationsWithLotSerialsForMoveOutKitBreak(objItemPullInfo.KitDetailGUID, objItemPullInfo.ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, 0, objItemPullInfo.QtyToMoveIn, "0", string.Empty, "0");
                                if (!objItemLocationDetail.Any())
                                {
                                    objItemLocationDetail = kitMoveInOutDetail.GetItemLocationsWithLotSerialsForMoveOut(objItemPullInfo.KitGuid, objItemPullInfo.ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                                }
                            }
                            if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                            {
                                var lstLotQty = (from il in objItemLocationDetail
                                                 //where il.LotNumber == t.LotNumber
                                                 group il by new { il.LotNumber, il.ExpirationDate } into grpms
                                                 select new
                                                 {
                                                     CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                     ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                     LotNumber = grpms.Key.LotNumber,
                                                     ExpirationDate = grpms.Key.ExpirationDate
                                                 }).FirstOrDefault();

                                if (lstLotQty != null)
                                {
                                    //if (t.PullQuantity > (lstLotQty.CustomerOwnedQuantity) + ((lstLotQty.ConsignedQuantity)))
                                    //{
                                    //    t.ValidationMessage = ResPullMaster.msgInvalidQuantityLot;
                                    //}
                                    //else
                                    //{
                                    if ((lstLotQty.CustomerOwnedQuantity) > 0)
                                        t.CustomerOwnedQuantity = t.PullQuantity;
                                    else
                                        t.ConsignedQuantity = t.PullQuantity;
                                    //}

                                    if (lstLotQty.ExpirationDate != null)
                                    {
                                        t.ExpirationDate = lstLotQty.ExpirationDate;
                                    }
                                }
                                else
                                {
                                    //t.ValidationMessage = "Lot number not available in Move In.";
                                    //isLotNotMovedIn = true;

                                    t.ValidationMessage = ResKitMaster.EnoughMoveInQtyNotAvailable;
                                    isLotMovedInQtyNotAvailable = true;

                                    //if (!objItem.Consignment)
                                    //    t.CustomerOwnedQuantity = t.PullQuantity;
                                    //else
                                    //    t.ConsignedQuantity = t.PullQuantity;
                                }
                            }
                            else
                            {
                                t.ValidationMessage = ResPullMaster.msgInvalidLot;
                            }
                        }

                    });



                    if (lstAvailableQty.Any(t => (t.ValidationMessage ?? string.Empty) != string.Empty))
                    {
                        if (isLotMovedInQtyNotAvailable)
                        {
                            objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "6", ErrorMessage = objItem.ItemNumber + ": " + ResKitMaster.EnoughMoveInQtyNotAvailable });
                        }
                        else if (isLotNumberEmpty)
                        {
                            objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "6", ErrorMessage = objItem.ItemNumber + ": " + ResKitMaster.LotNumberCantBeEmpty });
                        }
                        else if (isExpiratoindateEmpty)
                        {
                            objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "6", ErrorMessage = objItem.ItemNumber + ": " + ResPullMaster.EnterValidExpirationDate });
                        }
                        else
                        {
                            objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "6", ErrorMessage = objItem.ItemNumber + ": " + ResPullMaster.msgInvalidQuantityLot });
                        }

                    }
                    else
                    {
                        lstAvailableQty.ForEach(il =>
                        {
                            ConsignedTaken = il.ConsignedQuantity ?? 0;
                            CustownedTaken = il.CustomerOwnedQuantity ?? 0;

                            TotalPulled += (ConsignedTaken + CustownedTaken);
                            PullCost += (TotalPulled * (il.Cost ?? 0));
                            Diff = (objItemPullInfo.QtyToMoveIn - TotalPulled);
                            if (Diff < 0)
                            {
                                TotalPulled -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0));
                                PullCost -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) * (il.Cost ?? 0);
                                if (il.IsConsignedLotSerial)
                                {
                                    ConsignedTaken = (objItemPullInfo.QtyToMoveIn - TotalPulled);
                                }
                                else
                                {
                                    CustownedTaken = (objItemPullInfo.QtyToMoveIn - TotalPulled);
                                }
                                TotalPulled += (ConsignedTaken + CustownedTaken);
                                PullCost += ((ConsignedTaken + CustownedTaken) * (il.Cost ?? 0));

                            }
                            TotalCustOwned += CustownedTaken;
                            TotalConsigned += ConsignedTaken;
                            il.CustomerOwnedTobePulled = CustownedTaken;
                            il.ConsignedTobePulled = ConsignedTaken;
                            il.TotalTobePulled = CustownedTaken + ConsignedTaken;
                            il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((il.Cost ?? 0));

                        });

                        objItemPullInfo.TotalConsignedTobePulled = TotalConsigned;
                        objItemPullInfo.TotalCustomerOwnedTobePulled = TotalCustOwned;
                        objItemPullInfo.lstItemPullDetails = lstAvailableQty;

                    }
                }
                if (objItem.SerialNumberTracking)
                {
                    lstAvailableQty = objItemPullInfo.lstItemPullDetails;
                    //bool isSerialNotMovedIn = false;
                    bool isSerialMovedInQtyNotAvailable = false;
                    bool isSerialNumberEmpty = false;
                    string serialExistMsg = string.Empty;
                    bool isExpiratoindateEmpty = false;
                    lstAvailableQty.ForEach(t =>
                    {
                        if (string.IsNullOrEmpty(t.SerialNumber))
                        {
                            isSerialNumberEmpty = true;
                            t.ValidationMessage = ResKitMaster.SerialNumberCantBeEmpty;
                        }
                        else if (t.DateCodeTracking && string.IsNullOrEmpty(t.Expiration))
                        {
                            isExpiratoindateEmpty = true;
                            t.ValidationMessage = ResPullMaster.EnterValidExpirationDate;
                        }
                        else
                        {
                            List<ItemLocationDetailsDTO> objItemLocationDetail = new ItemLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetItemsLocationsSerLotQtyForMoveOut(objItemPullInfo.ItemGUID, string.Empty, t.SerialNumber, objItemPullInfo.RoomId, objItemPullInfo.CompanyId);

                            if (objItemLocationDetail != null && objItemLocationDetail.Count > 0)
                            {
                                t.ValidationMessage = string.Format(ResKitMaster.SerialAlreadyExist, t.SerialNumber);
                                serialExistMsg += ("<br> " + string.Format(ResKitMaster.SerialAlreadyExist, t.SerialNumber));
                            }
                            else
                            {
                                //var objItemLocationDetailSerial = kitMoveInOutDetail.GetItemLocationsWithLotSerialsForMoveOut(objItemPullInfo.KitDetailGUID, objItemPullInfo.ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, objItemPullInfo.BinID, objItemPullInfo.QtyToMoveIn, "0", t.SerialNumber, "0");
                                var objItemLocationDetailSerial = kitMoveInOutDetail.GetItemLocationsWithLotSerialsForMoveOut(objItemPullInfo.KitDetailGUID, objItemPullInfo.ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, 0, objItemPullInfo.QtyToMoveIn, "0", string.Empty, "0");

                                var lstLotQty = (dynamic)null;
                                if (!objItemLocationDetailSerial.Any() && objItemPullInfo.KitGuid != Guid.Empty)
                                {
                                    objItemLocationDetailSerial = kitMoveInOutDetail.GetItemLocationsWithLotSerialsForMoveOut(objItemPullInfo.KitGuid, objItemPullInfo.ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                                    lstLotQty = (from il in objItemLocationDetailSerial
                                                 group il by new { il.SerialNumber, il.ExpirationDate } into grpms
                                                 select new
                                                 {
                                                     CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                     ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                     LotNumber = grpms.Key.SerialNumber,
                                                     ExpirationDate = grpms.Key.ExpirationDate
                                                 }).FirstOrDefault();
                                }
                                else
                                {
                                    lstLotQty = (from il in objItemLocationDetailSerial
                                                 where il.SerialNumber == t.SerialNumber
                                                 group il by new { il.SerialNumber, il.ExpirationDate } into grpms
                                                 select new
                                                 {
                                                     CustomerOwnedQuantity = grpms.Sum(x => x.CustomerOwnedQuantity.GetValueOrDefault()),
                                                     ConsignedQuantity = grpms.Sum(x => x.ConsignedQuantity.GetValueOrDefault()),
                                                     LotNumber = grpms.Key.SerialNumber,
                                                     ExpirationDate = grpms.Key.ExpirationDate
                                                 }).FirstOrDefault();
                                }

                                if (lstLotQty != null)
                                {
                                    //if (t.PullQuantity > (lstLotQty.CustomerOwnedQuantity) + ((lstLotQty.ConsignedQuantity)))
                                    //{
                                    //    t.ValidationMessage = ResPullMaster.msgInvalidQuantityLot;
                                    //}
                                    //else
                                    //{
                                    if ((lstLotQty.CustomerOwnedQuantity) > 0)
                                        t.CustomerOwnedQuantity = t.PullQuantity;
                                    else
                                        t.ConsignedQuantity = t.PullQuantity;
                                    //}

                                    if (lstLotQty.ExpirationDate != null)
                                    {
                                        t.ExpirationDate = lstLotQty.ExpirationDate;
                                    }
                                }
                                else
                                {
                                    //t.ValidationMessage = "Serial not available in Move In.";
                                    //isSerialNotMovedIn = true;
                                    t.ValidationMessage = ResKitMaster.EnoughMoveInQtyNotAvailable;
                                    isSerialMovedInQtyNotAvailable = true;

                                    //if (!objItem.Consignment)
                                    //    t.CustomerOwnedQuantity = t.PullQuantity;
                                    //else
                                    //    t.ConsignedQuantity = t.PullQuantity;
                                }

                            }
                        }

                    });

                    if (lstAvailableQty.Any(t => (t.ValidationMessage ?? string.Empty) != string.Empty))
                    {
                        if (isSerialMovedInQtyNotAvailable)
                        {
                            objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "6", ErrorMessage = objItem.ItemNumber + ": " + ResKitMaster.EnoughMoveInQtyNotAvailable });
                        }
                        else if (isSerialNumberEmpty)
                        {
                            objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "6", ErrorMessage = objItem.ItemNumber + ": " + ResKitMaster.SerialNumberCantBeEmpty });
                        }
                        else if (isExpiratoindateEmpty)
                        {
                            objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "6", ErrorMessage = objItem.ItemNumber + ": " + ResPullMaster.EnterValidExpirationDate });
                        }
                        else
                        {
                            //objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "6", ErrorMessage = objItem.ItemNumber + ": " + "Serial number already exist." });
                            objItemPullInfo.ErrorList.Add(new PullErrorInfo() { ErrorCode = "6", ErrorMessage = objItem.ItemNumber + ": " + serialExistMsg });
                        }
                    }
                    else
                    {
                        lstAvailableQty.ForEach(il =>
                        {

                            ConsignedTaken = il.ConsignedQuantity ?? 0;
                            CustownedTaken = il.CustomerOwnedQuantity ?? 0;

                            TotalPulled += (ConsignedTaken + CustownedTaken);
                            PullCost += (TotalPulled * (il.Cost ?? 0));
                            Diff = (objItemPullInfo.QtyToMoveIn - TotalPulled);
                            if (Diff < 0)
                            {
                                TotalPulled -= ((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0));
                                PullCost -= (((il.CustomerOwnedQuantity ?? 0) + (il.ConsignedQuantity ?? 0)) * (il.Cost ?? 0));
                                if (il.IsConsignedLotSerial)
                                {
                                    ConsignedTaken = (objItemPullInfo.QtyToMoveIn - TotalPulled);
                                }
                                else
                                {
                                    CustownedTaken = (objItemPullInfo.QtyToMoveIn - TotalPulled);
                                }
                                TotalPulled += (ConsignedTaken + CustownedTaken);
                                PullCost += ((ConsignedTaken + CustownedTaken) * (il.Cost ?? 0));

                            }
                            TotalCustOwned += CustownedTaken;
                            TotalConsigned += ConsignedTaken;
                            il.CustomerOwnedTobePulled = CustownedTaken;
                            il.ConsignedTobePulled = ConsignedTaken;
                            il.TotalTobePulled = CustownedTaken + ConsignedTaken;
                            il.TotalPullCost = il.TotalTobePulled * Convert.ToDouble((il.Cost ?? 0));

                        });

                        objItemPullInfo.TotalConsignedTobePulled = TotalConsigned;
                        objItemPullInfo.TotalCustomerOwnedTobePulled = TotalCustOwned;
                        objItemPullInfo.lstItemPullDetails = lstAvailableQty;

                    }

                }
            }
            return objItemPullInfo;
        }

        /****************************************** Tool Kit methods start *******************************/
        public ActionResult KitToolList()
        {
            Session["ToolBinReplanish"] = null;
            Session["ToolORDType"] = null;
            return View();
        }
        public JsonResult KitToolMasterListAjax(JQueryDataTableParamModel param)
        {
            Session["ToolKitDetail"] = null;
            ToolMasterDAL obj = new eTurns.DAL.ToolMasterDAL(SessionHelper.EnterPriseDBName);

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
            //if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID";



            if (sortColumnName.Trim() == "ToolUDF1")
                sortColumnName = "UDF1";
            else if (sortColumnName.Trim() == "ToolUDF2")
                sortColumnName = "UDF2";
            else if (sortColumnName.Trim() == "ToolUDF3")
                sortColumnName = "UDF3";
            else if (sortColumnName.Trim() == "ToolUDF4")
                sortColumnName = "UDF4";
            else if (sortColumnName.Trim() == "ToolUDF5")
                sortColumnName = "UDF5";

            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "ID";

                if (sortDirection == "asc")
                    sortColumnName = sortColumnName + " asc";
                else
                    sortColumnName = sortColumnName + " desc";
            }
            else
            {
                sortColumnName = "ID desc";

            }
            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            string ToolType = Request["ToolType"].ToString();
            Session["ToolType"] = ToolType;
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetPagedTools(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, null, null, RoomDateFormat, CurrentTimeZone, Type: ToolType);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreateToolKit()
        {
            string NewNumber = new AutoSequenceDAL(SessionHelper.EnterPriseDBName).GetNextAutoNumberByModule("NextToolNo", SessionHelper.RoomID, SessionHelper.CompanyID);
            Session["ToolKitDetail"] = null;
            Session["ToolBinReplanish"] = null;
            ToolMasterDTO objDTO = new ToolMasterDTO();
            objDTO.ID = 0;
            //objDTO.ToolName = "#T" + NewNumber;
            objDTO.ToolName = NewNumber;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
            objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.RoomName = SessionHelper.RoomName;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.GUID = Guid.NewGuid();
            objDTO.IsOnlyFromItemUI = true;
            objDTO.Type = 2;
            objDTO.KitToolQuantity = 0;
            objDTO.KitToolName = objDTO.ToolName;
            objDTO.KitToolSerial = objDTO.Serial;
            //ToolCategoryMasterDAL objToolCategory = new ToolCategoryMasterDAL(SessionHelper.EnterPriseDBName);
            //List<ToolCategoryMasterDTO> lstToolCategory = objToolCategory.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
            //lstToolCategory.Insert(0, new ToolCategoryMasterDTO() { ID = 0, ToolCategory = "-- Select Category --" });
            //ViewBag.ToolCategoryList = lstToolCategory;

            LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(SessionHelper.EnterPriseDBName);
            List<LocationMasterDTO> lstLocation = objLocationCntrl.GetLocationByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            lstLocation = lstLocation.Where(t => (!string.IsNullOrWhiteSpace(t.Location))).ToList();
            lstLocation.Insert(0, new LocationMasterDTO() { ID = 0, Location = ResCommon.MsgSelectLocation });
            ViewBag.LocationList = lstLocation;


            //TechnicialMasterDAL objTechnicianCntrl = new eTurns.DAL.TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
            //List<TechnicianMasterDTO> lstTechnician = objTechnicianCntrl.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
            //lstTechnician.Insert(0, new TechnicianMasterDTO() { GUID = Guid.Empty, Technician = Convert.ToString(eTurns.DTO.Resources.ResCommon.SelectTechnicianText), TechnicianCode = string.Empty });
            //ViewBag.TechnicianList = lstTechnician;

            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("ToolMaster");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            ViewBag.GropOfItemsBag = GetGroupOfItems();

            objDTO.ImageType = "ExternalImage";
            return PartialView("_KitToolCreate", objDTO);
        }
        public ActionResult ToolEdit(Int64 ID)
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



            ToolMasterDAL obj = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            ToolMasterDTO objDTO = new ToolMasterDTO();
            if (!IsChangeLog)
                objDTO = obj.GetToolByIDPlain(ID);
            else
                objDTO = obj.GetHistoryRecord(ID, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted);
            objDTO.KitToolQuantity = objDTO.Quantity;
            objDTO.KitToolName = objDTO.ToolName;
            objDTO.KitToolSerial = objDTO.Serial;
            ToolDetailDAL objToolDetailDAL = new ToolDetailDAL(SessionHelper.EnterPriseDBName);
            Session["ToolKitDetail"] = objToolDetailDAL.GetAllRecordsByKitGUID(objDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false, true).ToList();

            objDTO.IsOnlyFromItemUI = true;

            //ToolCategoryMasterDAL objToolCategory = new ToolCategoryMasterDAL(SessionHelper.EnterPriseDBName);
            //List<ToolCategoryMasterDTO> lstToolCategory = objToolCategory.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            //lstToolCategory.Insert(0, new ToolCategoryMasterDTO() { ID = 0, ToolCategory = "-- Select Category --" });
            //ViewBag.ToolCategoryList = lstToolCategory;

            //LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(SessionHelper.EnterPriseDBName);
            //List<LocationMasterDTO> lstLocation = objLocationCntrl.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
            //lstLocation.Insert(0, new LocationMasterDTO() { ID = 0, Location = "-- Select Location --" });
            //ViewBag.LocationList = lstLocation;
            LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(SessionHelper.EnterPriseDBName);
            List<LocationMasterDTO> lstLocation = objLocationCntrl.GetLocationByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            lstLocation = lstLocation.Where(t => (!string.IsNullOrWhiteSpace(t.Location))).ToList();
            lstLocation.Insert(0, new LocationMasterDTO() { ID = 0, Location = ResCommon.MsgSelectLocation });
            ViewBag.LocationList = lstLocation;



            ToolLocationDetailsDAL objToolLocationDetailDAL = new ToolLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            List<ToolLocationDetailsDTO> lstToolLocationDetailsDTO = objToolLocationDetailDAL.GetToolLocationsByToolGUID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, objDTO.GUID).ToList();

            List<LocationMasterDTO> lstAllLocation = new List<LocationMasterDTO>(lstLocation.ToList());
            List<Guid> objGUIDList = lstToolLocationDetailsDTO.Select(u => u.LocationGUID).ToList();
            lstAllLocation = lstAllLocation.Where(l => objGUIDList.Contains(l.GUID)).ToList();
            if ((objDTO.LocationID ?? 0) > 0)
            {
                LocationMasterDTO objLocationMasterDTO = lstAllLocation.Where(t => t.ID == objDTO.LocationID).FirstOrDefault();
                if (objLocationMasterDTO != null)
                {
                    objLocationMasterDTO.IsDefault = true;
                }
            }
            Session["ToolBinReplanish"] = lstAllLocation;
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("ToolMaster");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;

            ViewBag.GropOfItemsBag = GetGroupOfItems();

            return PartialView("_KitToolCreate", objDTO);
        }
        public ActionResult LoadToolKitComponentofItem(Guid ToolGUID, int? AddCount)
        {
            ViewBag.ToolGUID = ToolGUID;
            List<ToolDetailDTO> lstToolDetailDTO = null;
            if (Session["ToolKitDetail"] != null)
            {
                lstToolDetailDTO = (List<ToolDetailDTO>)Session["ToolKitDetail"];
                //Delete blank rows
                lstToolDetailDTO.Remove(lstToolDetailDTO.Where(t => t.GUID == Guid.Empty && t.ToolGUID.GetValueOrDefault(Guid.Empty) == Guid.Empty).FirstOrDefault());
            }
            else
            {
                lstToolDetailDTO = new List<ToolDetailDTO>();
            }

            if (AddCount != null && AddCount > 0)
            {
                for (int i = 0; i < AddCount; i++)
                {
                    lstToolDetailDTO.Add(new ToolDetailDTO() { ID = 0, SessionSr = lstToolDetailDTO.Count + 1, ToolGUID = ToolGUID, Room = SessionHelper.RoomID, CompanyID = SessionHelper.CompanyID, Updated = DateTimeUtility.DateTimeNow, LastUpdatedBy = SessionHelper.UserID, Created = DateTimeUtility.DateTimeNow, CreatedBy = SessionHelper.UserID });
                }
            }
            return PartialView("_ToolKitComponent", lstToolDetailDTO.OrderBy(t => t.Serial).ToList());
        }
        public ActionResult LoadToolKitModel(string Parentid, string ParentGuid, string ToolType)
        {
            Session["ToolType"] = ToolType;
            ItemModelPerameter obj = new ItemModelPerameter()
            {
                AjaxURLAddItemToSession = "~/Kit/AddToolToDetailTableKit/",
                PerentID = Parentid,
                PerentGUID = ParentGuid,
                ModelHeader = ResKitMaster.AddToolKitComponentToToolKit,
                AjaxURLAddMultipleItemToSession = "~/Kit/AddToolToDetailTableKit/",
                AjaxURLToFillItemGrid = "~/Kit/GetToolsModelMethodKit/",
                CallingFromPageName = "KITTool"
            };

            return PartialView("ToolMasterModel", obj);
        }
        public JsonResult AddToolToDetailTableKit(string para)
        {
            string message = "";
            string status = "";
            JavaScriptSerializer s = new JavaScriptSerializer();
            ToolDetailDTO[] ToolDetails = s.Deserialize<ToolDetailDTO[]>(para);

            //ToolDetailDAL objDAL = new eTurns.DAL.ToolDetailDAL(SessionHelper.EnterPriseDBName);
            int sessionsr = 0;

            ToolMasterDAL objToolDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);

            foreach (ToolDetailDTO item in ToolDetails)
            {
                item.Room = SessionHelper.RoomID;
                item.RoomName = SessionHelper.RoomName;
                item.CreatedBy = SessionHelper.UserID;
                item.CreatedByName = SessionHelper.UserName;
                item.UpdatedByName = SessionHelper.UserName;
                item.LastUpdatedBy = SessionHelper.UserID;
                item.CompanyID = SessionHelper.CompanyID;
                item.Updated = DateTimeUtility.DateTimeNow;

                if (!(item.GUID != null && item.GUID != Guid.Empty))
                {
                    item.Created = DateTimeUtility.DateTimeNow;

                    ToolMasterDTO ObjToolDTO = objToolDAL.GetToolByGUIDPlain(item.ToolItemGUID.GetValueOrDefault(Guid.Empty));
                    if (ObjToolDTO != null && ObjToolDTO.Type != 2)
                    {
                        item.ToolItemGUID = ObjToolDTO.GUID;
                        List<ToolDetailDTO> lstToolDetail = null;
                        if (Session["ToolKitDetail"] != null)
                        {
                            lstToolDetail = (List<ToolDetailDTO>)Session["ToolKitDetail"];
                            item.SessionSr = lstToolDetail.Count + 1;
                        }
                        else
                        {

                            item.SessionSr = sessionsr + 1;
                            lstToolDetail = new List<ToolDetailDTO>();
                        }
                        lstToolDetail.Add(item);
                        Session["ToolKitDetail"] = lstToolDetail;

                    }
                }
            }

            message = ResKitMaster.ToolQtyUpdatedSuccessfully;
            status = "ok";
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetToolsModelMethodKit(JQueryDataTableParamModel param)
        {
            //ItemMasterController obj = new ItemMasterController();
            ToolMasterDAL obj = new eTurns.DAL.ToolMasterDAL(SessionHelper.EnterPriseDBName);

            //LoadTestEntities entity = new LoadTestEntities();
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

            if (sortColumnName == "0" || sortColumnName.Contains("undefined"))
                sortColumnName = "Serial Asc";

            string searchQuery = string.Empty;

            //Guid QLID = Guid.Empty;
            //Guid.TryParse(Convert.ToString(Request["ParentGUID"]), out QLID);

            int TotalRecordCount = 0;

            List<ToolDetailDTO> objQLItems = null;
            if (Session["ToolKitDetail"] != null)
            {
                objQLItems = (List<ToolDetailDTO>)Session["ToolKitDetail"];
            }

            string ToolIDs = "";
            if (objQLItems != null && objQLItems.Count > 0)
            {
                foreach (var item in objQLItems)
                {
                    if (!string.IsNullOrEmpty(ToolIDs))
                        ToolIDs += ",";

                    ToolIDs += item.ToolItemGUID.ToString();
                }
            }
            string ToolType = Request["ToolType"].ToString();
            Session["ToolType"] = ToolType;
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetPagedTools(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, "", "", RoomDateFormat, CurrentTimeZone, ToolIDs, Type: ToolType);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveToolKitQty(Guid ToolItemGuid, double? QuantityPerKit)
        {
            List<ToolDetailDTO> lstToolDetailDTO = (List<ToolDetailDTO>)Session["ToolKitDetail"];
            if (lstToolDetailDTO != null && lstToolDetailDTO.Count > 0)
            {
                ToolDetailDTO objToolDetailDTO = lstToolDetailDTO.Where(t => t.ToolItemGUID == ToolItemGuid).FirstOrDefault();
                if (objToolDetailDTO != null)
                {
                    objToolDetailDTO.QuantityPerKit = Convert.ToInt64(QuantityPerKit);
                }
                lstToolDetailDTO = lstToolDetailDTO.Where(t => t.ToolItemGUID != ToolItemGuid).ToList();
                lstToolDetailDTO.Add(objToolDetailDTO);
                Session["ToolKitDetail"] = lstToolDetailDTO;
            }
            return Json(true);

        }

        [ValidateAntiForgeryToken]
        public JsonResult ToolKitSave(ToolMasterDTO objDTO)
        {
            Int64 TempToolID = 0;
            string message = "";
            string status = "";
            ToolMasterDAL obj = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            LocationMasterDAL objLocationMasterDAL = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
            ToolLocationDetailsDAL objToolLocationDetailsDAL = new ToolLocationDetailsDAL(SessionHelper.EnterPriseDBName);
            objDTO.Quantity = objDTO.KitToolQuantity ?? 0;
            objDTO.ToolName = objDTO.KitToolName;
            objDTO.Serial = objDTO.KitToolSerial;
            ToolLocationDetailsDTO objToolLocationDetailsDTO = null;
            if ((List<ToolDetailDTO>)Session["ToolKitDetail"] == null || ((List<ToolDetailDTO>)Session["ToolKitDetail"]).Count == 0)
            {
                message = ResKitMaster.AddKitComponentRequired;
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(objDTO.ToolName))
            {
                message = string.Format(ResMessage.Required, ResKitToolMaster.ToolName);// "Tool is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(objDTO.Serial))
            {
                message = string.Format(ResMessage.Required, ResKitToolMaster.Serial);// "Tool is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }

            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.CompanyID = SessionHelper.CompanyID;

            CommonDAL objCommon = new CommonDAL(SessionHelper.EnterPriseDBName);
            bool AllowToolOrdering = SessionHelper.AllowToolOrdering;
            if (objDTO.ID == 0)
            {
                objDTO.CreatedBy = SessionHelper.UserID;

                //string strOK = objCommon.DuplicateCheck(objDTO.Serial ?? string.Empty, "add", objDTO.ID, "ToolMaster", "Serial", SessionHelper.RoomID, SessionHelper.CompanyID);
                // string strOK = obj.ToolSerialDuplicateCheck((objDTO.Serial ?? string.Empty).Trim(), objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID);
                string strOK = "ok";// obj.ToolSerialDuplicateCheck((objDTO.Serial ?? string.Empty).Trim(), objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (!string.IsNullOrWhiteSpace(objDTO.ToolTypeTracking) && objDTO.ToolTypeTracking.Contains("2"))
                {
                    objDTO.SerialNumberTracking = true;
                }
                if (AllowToolOrdering)
                {
                    //if (objDTO.SerialNumberTracking)
                    //{

                    //    strOK = obj.ToolNameDuplicateCheck((objDTO.ToolName ?? string.Empty).Trim(), objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    //}
                    //else
                    {
                        strOK = obj.ToolNameSerialDuplicateCheck((objDTO.ToolName ?? string.Empty).Trim() + "$" + (objDTO.Serial ?? string.Empty).Trim(), objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    }
                }
                else
                {
                    strOK = obj.ToolSerialDuplicateCheck((objDTO.Serial ?? string.Empty).Trim(), objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID);
                }
                if (strOK == "duplicate")
                {
                    if (AllowToolOrdering)
                    {
                        message = string.Format(ResMessage.DuplicateMessage, ResToolMaster.ToolName, (objDTO.ToolName ?? string.Empty).Trim());  //"Tool \"" + objDTO.ToolName + "\" already exist! Try with Another!";
                        status = "duplicate";
                    }
                    else
                    {

                        message = string.Format(ResMessage.DuplicateMessage, ResToolMaster.Serial, (objDTO.Serial ?? string.Empty).Trim());  //"Tool \"" + objDTO.ToolName + "\" already exist! Try with Another!";
                        status = "duplicate";
                    }
                }
                else
                {
                    //objDTO.GUID = Guid.NewGuid();
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.CreatedByName = SessionHelper.UserName;
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    TempToolID = obj.Insert(objDTO, AllowToolOrdering);
                    if (TempToolID > 0)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                        List<ToolDetailDTO> lstKitDetailDTO = null;
                        if (Session["ToolKitDetail"] != null)
                        {
                            lstKitDetailDTO = ((List<ToolDetailDTO>)Session["ToolKitDetail"]).Where(t => t.ToolItemGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty).ToList();
                            foreach (var itembr in lstKitDetailDTO)
                            {
                                itembr.ToolGUID = objDTO.GUID;
                                //  itembr.ItemGUID =  objDTO.GUID;
                                itembr.WhatWhereAction = "KitController---> ToolKitSave";
                                ToolDetailDAL objToolDetailDAL = new ToolDetailDAL(SessionHelper.EnterPriseDBName);
                                if (itembr.ID > 0)
                                {
                                    itembr.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    itembr.EditedFrom = "Web";

                                    objToolDetailDAL.Edit(itembr);
                                }
                                else
                                {

                                    objToolDetailDAL.Insert(itembr);
                                }
                            }
                            LocationMasterDTO objLocationMasterDTO = null;
                            ToolMasterDTO objToolMasterDTO = obj.GetToolBySerialPlain(objDTO.Serial, SessionHelper.RoomID, SessionHelper.CompanyID);
                            //if (objDTO.LocationID.GetValueOrDefault(0) <= 0)
                            //{
                            //    objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(objToolMasterDTO.GUID, string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "KitController>>ToolKitSave");
                            //}
                            //if (objDTO.LocationID.GetValueOrDefault(0) > 0)
                            //{
                            //    LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(SessionHelper.EnterPriseDBName);
                            //    List<LocationMasterDTO> lstLocation = objLocationCntrl.GetLocationForRoomWise(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
                            //    objLocationMasterDTO = lstLocation.Where(i => i.ID == objDTO.LocationID).FirstOrDefault();
                            //    objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(objDTO.GUID, objLocationMasterDTO.Location, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "AssetController>>ToolSave");

                            //}
                            if (AllowToolOrdering)
                            {
                                if (Session["ToolBinReplanish"] != null)
                                {
                                    List<LocationMasterDTO> lstAllLocation = (List<LocationMasterDTO>)Session["ToolBinReplanish"];

                                    if (lstAllLocation != null && lstAllLocation.Count() > 0 && lstAllLocation.Any())
                                    {
                                        for (int i = 0; i < lstAllLocation.Count(); i++)
                                        {
                                            if ((lstAllLocation[i].IsDefault ?? false) == true)
                                            {
                                                LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(SessionHelper.EnterPriseDBName);
                                                objLocationMasterDTO = objLocationCntrl.GetLocationByNamePlain(lstAllLocation[i].Location, SessionHelper.RoomID, SessionHelper.CompanyID);
                                                objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(objDTO.GUID, objLocationMasterDTO.Location, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "AssetController>>ToolSave");
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(SessionHelper.EnterPriseDBName);
                                        objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(objDTO.GUID, string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "ToolController>>ToolSave");
                                        objLocationMasterDTO = objLocationCntrl.GetLocationByNamePlain(string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID);
                                    }
                                }
                                else
                                {
                                    LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(SessionHelper.EnterPriseDBName);
                                    objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(objDTO.GUID, string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "ToolController>>ToolSave");
                                    objLocationMasterDTO = objLocationCntrl.GetLocationByNamePlain(string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID);
                                }
                                if (objLocationMasterDTO == null)
                                {
                                    LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(SessionHelper.EnterPriseDBName);
                                    objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(objDTO.GUID, string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "ToolController>>ToolSave");
                                    objLocationMasterDTO = objLocationCntrl.GetLocationByNamePlain(string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID);
                                }
                                if (objDTO.Quantity > 0 && (!objDTO.SerialNumberTracking))
                                {
                                    ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO = new ToolAssetQuantityDetailDTO();
                                    objToolAssetQuantityDetailDTO.ToolGUID = objToolMasterDTO.GUID;

                                    objToolAssetQuantityDetailDTO.AssetGUID = null;


                                    objToolAssetQuantityDetailDTO.ToolBinID = objToolLocationDetailsDTO != null ? objToolLocationDetailsDTO.ID : ((objLocationMasterDTO != null) ? objLocationMasterDTO.ID : 0);
                                    objToolAssetQuantityDetailDTO.Quantity = objDTO.Quantity;
                                    objToolAssetQuantityDetailDTO.RoomID = SessionHelper.RoomID;
                                    objToolAssetQuantityDetailDTO.CompanyID = SessionHelper.CompanyID;
                                    objToolAssetQuantityDetailDTO.Created = objDTO.Created;
                                    objToolAssetQuantityDetailDTO.Updated = objDTO.Updated ?? DateTimeUtility.DateTimeNow;
                                    objToolAssetQuantityDetailDTO.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                                    objToolAssetQuantityDetailDTO.ReceivedOn = objDTO.ReceivedOn;
                                    objToolAssetQuantityDetailDTO.AddedFrom = "Web";
                                    objToolAssetQuantityDetailDTO.EditedFrom = "Web";
                                    objToolAssetQuantityDetailDTO.WhatWhereAction = "KitController>>ToolKitSave";
                                    objToolAssetQuantityDetailDTO.ReceivedDate = null;
                                    objToolAssetQuantityDetailDTO.InitialQuantityWeb = objDTO.Quantity;
                                    objToolAssetQuantityDetailDTO.InitialQuantityPDA = 0;
                                    objToolAssetQuantityDetailDTO.ExpirationDate = null;
                                    objToolAssetQuantityDetailDTO.EditedOnAction = "Tool Kit Created From Web Page. insert Entry of Tool Kit.";
                                    objToolAssetQuantityDetailDTO.CreatedBy = SessionHelper.UserID;
                                    objToolAssetQuantityDetailDTO.UpdatedBy = SessionHelper.UserID;
                                    objToolAssetQuantityDetailDTO.IsDeleted = false;
                                    objToolAssetQuantityDetailDTO.IsArchived = false;

                                    ToolAssetQuantityDetailDAL objToolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
                                    objToolAssetQuantityDetailDAL.Insert(objToolAssetQuantityDetailDTO, false, "AdjCredit", ReferalAction: "Initial Tool Create");
                                }
                                if (Session["ToolBinReplanish"] != null)
                                {
                                    List<LocationMasterDTO> lstAllLocation = (List<LocationMasterDTO>)Session["ToolBinReplanish"];

                                    if (lstAllLocation != null && lstAllLocation.Count() > 0 && lstAllLocation.Any())
                                    {
                                        for (int i = 0; i < lstAllLocation.Count(); i++)
                                        {
                                            objLocationMasterDAL.GetToolLocation(objToolMasterDTO.GUID, lstAllLocation[i].Location, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "KitController>>ToolKitSave", lstAllLocation[i].IsDefault);
                                        }
                                    }
                                }
                            }
                            Session["ToolKitDetail"] = null;
                        }
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                        status = "fail";
                    }

                }
            }
            else
            {
                objDTO.LastUpdatedBy = SessionHelper.UserID;
                string strOK = "";
                if (!string.IsNullOrWhiteSpace(objDTO.ToolTypeTracking) && objDTO.ToolTypeTracking.Contains("2"))
                {
                    objDTO.SerialNumberTracking = true;
                }
                if (AllowToolOrdering)
                {
                    //if (objDTO.SerialNumberTracking)
                    //{
                    //    strOK = obj.ToolNameDuplicateCheck((objDTO.ToolName ?? string.Empty).Trim(), objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    //}
                    //else
                    {
                        strOK = obj.ToolNameSerialDuplicateCheck((objDTO.ToolName ?? string.Empty).Trim() + "$" + (objDTO.Serial ?? string.Empty).Trim(), objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    }
                }
                else
                {
                    strOK = objCommon.DuplicateCheck(objDTO.Serial ?? string.Empty, "edit", objDTO.ID, "ToolMaster", "Serial", SessionHelper.RoomID, SessionHelper.CompanyID);
                }
                //string strOK = objCommon.DuplicateCheck(objDTO.Serial ?? string.Empty, "edit", objDTO.ID, "ToolMaster", "Serial", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    if (AllowToolOrdering)
                    {
                        message = string.Format(ResMessage.DuplicateMessage, ResToolMaster.ToolName, (objDTO.ToolName ?? string.Empty).Trim());  //"Tool \"" + objDTO.ToolName + "\" already exist! Try with Another!";
                        status = "duplicate";
                    }
                    else
                    {

                        message = string.Format(ResMessage.DuplicateMessage, ResToolMaster.Serial, (objDTO.Serial ?? string.Empty).Trim());  //"Tool \"" + objDTO.ToolName + "\" already exist! Try with Another!";
                        status = "duplicate";
                    }
                }
                else
                {
                    if (objDTO.IsOnlyFromItemUI)
                    {
                        objDTO.EditedFrom = "Web";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    }
                    ToolMasterDTO tempobjDTO = new ToolMasterDTO();

                    tempobjDTO = obj.GetToolByIDPlain(objDTO.ID);
                    if (tempobjDTO != null)
                    {
                        objDTO.AddedFrom = tempobjDTO.AddedFrom;
                        objDTO.ReceivedOnWeb = tempobjDTO.ReceivedOnWeb;
                    }

                    if (obj.Edit(objDTO, AllowToolOrdering))
                    {
                        TempToolID = objDTO.ID;
                        message = ResMessage.SaveMessage;
                        status = "ok";
                        List<ToolDetailDTO> lstToolDetailDTO = null;
                        //if (objDTO.LocationID.GetValueOrDefault(0) <= 0)
                        //{
                        //    objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(objDTO.GUID, string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "KitController>>ToolKitSave");
                        //}
                        if (Session["ToolKitDetail"] != null)
                        {
                            lstToolDetailDTO = ((List<ToolDetailDTO>)Session["ToolKitDetail"]).Where(t => t.ToolGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty).ToList();
                            if (lstToolDetailDTO.Count > 0)
                            {
                                string KitIDs = "";
                                ToolDetailDAL objKitDetailDAL = new ToolDetailDAL(SessionHelper.EnterPriseDBName);
                                foreach (var itembr in lstToolDetailDTO)
                                {
                                    //itembr.GUID = objDTO.GUID;
                                    itembr.WhatWhereAction = "KitController---> ToolKitSave";
                                    if (itembr.ID > 0)
                                    {
                                        itembr.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        itembr.EditedFrom = "Web";

                                        objKitDetailDAL.Edit(itembr);
                                    }
                                    else
                                    {

                                        objKitDetailDAL.Insert(itembr);
                                    }
                                    KitIDs += itembr.ID + ",";
                                }
                                //Delete except session record....

                                objKitDetailDAL.DeleteRecordsExcept(KitIDs, objDTO.GUID, SessionHelper.UserID, SessionHelper.CompanyID);
                            }
                            Session["ToolKitDetail"] = null;
                            ToolMasterDTO objToolMasterDTO = obj.GetToolBySerialPlain(objDTO.Serial, SessionHelper.RoomID, SessionHelper.CompanyID);
                            LocationMasterDTO objLocationMasterDTO = null;
                            if (AllowToolOrdering)
                            {
                                //if (objDTO.LocationID.GetValueOrDefault(0) > 0)
                                //{
                                //    LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(SessionHelper.EnterPriseDBName);
                                //    List<LocationMasterDTO> lstLocation = objLocationCntrl.GetLocationForRoomWise(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
                                //    objLocationMasterDTO = lstLocation.Where(i => i.ID == objDTO.LocationID).FirstOrDefault();
                                //    objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(objDTO.GUID, objLocationMasterDTO.Location, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "AssetController>>ToolSave");

                                //}
                                if (Session["ToolBinReplanish"] != null)
                                {
                                    List<LocationMasterDTO> lstAllLocation = (List<LocationMasterDTO>)Session["ToolBinReplanish"];

                                    if (lstAllLocation != null && lstAllLocation.Count() > 0 && lstAllLocation.Any())
                                    {
                                        for (int i = 0; i < lstAllLocation.Count(); i++)
                                        {
                                            if ((lstAllLocation[i].IsDefault ?? false) == true)
                                            {
                                                LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(SessionHelper.EnterPriseDBName);
                                                objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(objDTO.GUID, lstAllLocation[i].Location, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "AssetController>>ToolSave");
                                                objLocationMasterDTO = objLocationCntrl.GetLocationByNamePlain(lstAllLocation[i].Location, SessionHelper.RoomID, SessionHelper.CompanyID);
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(SessionHelper.EnterPriseDBName);
                                        objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(objDTO.GUID, string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "ToolController>>ToolSave");
                                        objLocationMasterDTO = objLocationCntrl.GetLocationByNamePlain(string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID);
                                    }
                                }
                                else
                                {
                                    LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(SessionHelper.EnterPriseDBName);
                                    objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(objDTO.GUID, string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "ToolController>>ToolSave");
                                    objLocationMasterDTO = objLocationCntrl.GetLocationByNamePlain(string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID);

                                }
                                if (objLocationMasterDTO == null)
                                {
                                    LocationMasterDAL objLocationCntrl = new eTurns.DAL.LocationMasterDAL(SessionHelper.EnterPriseDBName);
                                    objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolLocation(objDTO.GUID, string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "ToolController>>ToolSave");
                                    objLocationMasterDTO = objLocationCntrl.GetLocationByNamePlain(string.Empty, SessionHelper.RoomID, SessionHelper.CompanyID);

                                    objLocationMasterDAL.UpdateToolWithDefault(objDTO.GUID, objLocationMasterDTO.ID, objLocationMasterDTO.GUID);

                                }
                                if (!objDTO.SerialNumberTracking)
                                {
                                    ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO = new ToolAssetQuantityDetailDTO();
                                    objToolAssetQuantityDetailDTO.ToolGUID = objDTO.GUID;

                                    objToolAssetQuantityDetailDTO.AssetGUID = null;


                                    objToolAssetQuantityDetailDTO.ToolBinID = objToolLocationDetailsDTO != null ? objToolLocationDetailsDTO.ID : ((objLocationMasterDTO != null) ? objLocationMasterDTO.ID : 0);
                                    objToolAssetQuantityDetailDTO.Quantity = objDTO.Quantity;
                                    objToolAssetQuantityDetailDTO.RoomID = SessionHelper.RoomID;
                                    objToolAssetQuantityDetailDTO.CompanyID = SessionHelper.CompanyID;
                                    objToolAssetQuantityDetailDTO.Created = objDTO.Created;
                                    objToolAssetQuantityDetailDTO.Updated = objDTO.Updated ?? DateTimeUtility.DateTimeNow;
                                    objToolAssetQuantityDetailDTO.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                                    objToolAssetQuantityDetailDTO.ReceivedOn = objDTO.ReceivedOn;
                                    objToolAssetQuantityDetailDTO.AddedFrom = "Web";
                                    objToolAssetQuantityDetailDTO.EditedFrom = "Web";
                                    objToolAssetQuantityDetailDTO.WhatWhereAction = "KitController>>ToolKitSave";
                                    objToolAssetQuantityDetailDTO.ReceivedDate = null;
                                    objToolAssetQuantityDetailDTO.InitialQuantityWeb = objDTO.Quantity;
                                    objToolAssetQuantityDetailDTO.InitialQuantityPDA = 0;
                                    objToolAssetQuantityDetailDTO.ExpirationDate = null;
                                    objToolAssetQuantityDetailDTO.EditedOnAction = "Tool Kit Update From Web Page. Update Entry of Tool Kit.";
                                    objToolAssetQuantityDetailDTO.CreatedBy = SessionHelper.UserID;
                                    objToolAssetQuantityDetailDTO.UpdatedBy = SessionHelper.UserID;
                                    objToolAssetQuantityDetailDTO.IsDeleted = false;
                                    objToolAssetQuantityDetailDTO.IsArchived = false;

                                    ToolAssetQuantityDetailDAL objToolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
                                    objToolAssetQuantityDetailDAL.UpdateOrInsert(objToolAssetQuantityDetailDTO, null);
                                }
                                if (Session["ToolBinReplanish"] != null)
                                {
                                    List<LocationMasterDTO> lstAllLocation = (List<LocationMasterDTO>)Session["ToolBinReplanish"];

                                    if (lstAllLocation != null && lstAllLocation.Count() > 0 && lstAllLocation.Any())
                                    {
                                        for (int i = 0; i < lstAllLocation.Count(); i++)
                                        {
                                            objLocationMasterDTO = objLocationMasterDAL.GetToolLocation(objDTO.GUID, lstAllLocation[i].Location, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "AssetController>>ToolSave", lstAllLocation[i].IsDefault);
                                            lstAllLocation[i].GUID = objLocationMasterDTO.GUID;
                                        }
                                    }

                                    ToolLocationDetailsDAL objToolLocationDetailDAL = new ToolLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                                    List<ToolLocationDetailsDTO> lstToolLocationDetailsDTO = objToolLocationDetailDAL.GetToolLocationsByToolGUID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, objDTO.GUID).ToList();

                                    //List<LocationMasterDTO> lstToolLocation = new List<LocationMasterDTO>(lstLocation.ToList());
                                    List<Guid> objGUIDList = lstAllLocation.Select(u => u.GUID).ToList();

                                    // List<ToolLocationDetailsDTO> objExcludeGUIDList = lstToolLocationDetailsDTO.Select(u => (!objGUIDList.Contains(u.ToolLocationGuid)).
                                    lstToolLocationDetailsDTO = lstToolLocationDetailsDTO.Where(l => (!objGUIDList.Contains(l.LocationGUID))).ToList();
                                    if (lstToolLocationDetailsDTO != null && lstToolLocationDetailsDTO.Count() > 0 && lstToolLocationDetailsDTO.Any())
                                    {
                                        foreach (ToolLocationDetailsDTO t in lstToolLocationDetailsDTO)
                                        {
                                            objToolLocationDetailDAL.DeleteByToolLocationGuid(t.LocationGUID, SessionHelper.UserID, "KitController>>ToolKitSave", "Web", objDTO.GUID);
                                        }
                                    }
                                    if (lstAllLocation == null || lstAllLocation.Count() == 0 || (!lstAllLocation.Any()))
                                    {

                                        objLocationMasterDAL.UpdateToolWithDefault(objDTO.GUID, 0, Guid.Empty);

                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);
                        status = "fail";
                    }
                }
            }

            return Json(new { Message = message, Status = status, ToolID = TempToolID }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeletetoSeesionToolKitComponentSingle(Int64 ID, Int32 SessionSr, Guid GUID, Guid ITEMGUID, Guid KitGUID, double QuantityPerKit)
        {

            List<ToolDetailDTO> lstBinReplanish = null;
            if (Session["ToolKitDetail"] != null)
            {
                lstBinReplanish = (List<ToolDetailDTO>)Session["ToolKitDetail"];
            }
            else
            {
                lstBinReplanish = new List<ToolDetailDTO>();
            }

            ///Delete the Records......
            if (ID > 0)
            {
                try
                {

                    //check the kit deletable logic if it is allow or nots
                    ToolDetailDAL objKitDetailDAL = new ToolDetailDAL(SessionHelper.EnterPriseDBName);
                    if (objKitDetailDAL.IsKitItemDeletable(GUID.ToString(), SessionHelper.RoomID, SessionHelper.CompanyID))
                    {
                        lstBinReplanish.Remove(lstBinReplanish.Where(t => t.ID == ID).FirstOrDefault());

                        Session["ToolKitDetail"] = lstBinReplanish;
                    }
                    else
                    {
                        return Json(new { status = "reference" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch
                {
                    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "deleted" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    if (GUID == Guid.Empty && ITEMGUID != Guid.Empty)
                    {
                        lstBinReplanish.Remove(lstBinReplanish.Where(t => t.ToolItemGUID == ITEMGUID && t.SessionSr == SessionSr).FirstOrDefault());
                    }
                    else
                    {
                        lstBinReplanish.Remove(lstBinReplanish.Where(t => t.GUID == GUID && t.SessionSr == SessionSr).FirstOrDefault());
                    }

                    Session["ToolKitDetail"] = lstBinReplanish;
                }
                catch
                {
                    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "deleted" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult ToolQtyToMoveIn(List<ToolMoveInOutDetailDTO> MoveInDTO)
        {
            ToolMoveInOutDetailDAL ToolMoveInOutDtlDAL = null;
            ToolMasterDAL objToolMasterDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            string msg = string.Empty;
            bool AllowToolOrdering = SessionHelper.AllowToolOrdering;
            try
            {
                ToolMoveInOutDtlDAL = new ToolMoveInOutDetailDAL(SessionHelper.EnterPriseDBName);
                foreach (var item in MoveInDTO)
                {
                    item.CreatedBy = UserID;
                    item.LastUpdatedBy = UserID;
                    item.Room = RoomID;
                    item.CompanyID = CompanyID;

                    if (item.MoveInOut == "IN")
                    {
                        msg += ToolMoveInOutDtlDAL.QtyToMoveIn(item, RoomID, CompanyID, UserID, SessionHelper.EnterPriceID, ResourceHelper.CurrentCult.Name, AllowToolOrdering);




                    }
                    else if (item.MoveInOut == "OUT")
                    {


                        msg += ToolMoveInOutDtlDAL.QtyToMoveOut(item, RoomID, CompanyID, UserID, SessionHelper.EnterPriceID, ResourceHelper.CurrentCult.Name, AllowToolOrdering);
                        if (string.IsNullOrWhiteSpace(msg))
                        {
                            //ToolMasterDTO objToolMasterDTO = objToolMasterDAL.GetToolListByID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, item.ToolItemGUID, null);

                            //ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO = new ToolAssetQuantityDetailDTO();
                            //objToolAssetQuantityDetailDTO.ToolGUID = objToolMasterDTO.GUID;

                            //objToolAssetQuantityDetailDTO.AssetGUID = null;


                            //ToolLocationDetailsDAL objToolLocationDetailsDAL = new ToolLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                            //ToolLocationDetailsDTO objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolDefaultLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, objToolMasterDTO.GUID, SessionHelper.UserID, "Web", "KitController>>ToolQtytomovein");


                            //objToolAssetQuantityDetailDTO.ToolBinID = objToolLocationDetailsDTO != null ? objToolLocationDetailsDTO.ID : objToolMasterDTO.ToolLocationDetailsID;
                            //objToolAssetQuantityDetailDTO.Quantity = item.Quantity;
                            //objToolAssetQuantityDetailDTO.RoomID = SessionHelper.RoomID;
                            //objToolAssetQuantityDetailDTO.CompanyID = SessionHelper.CompanyID;
                            //objToolAssetQuantityDetailDTO.Created = DateTimeUtility.DateTimeNow;
                            //objToolAssetQuantityDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                            //objToolAssetQuantityDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                            //objToolAssetQuantityDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            //objToolAssetQuantityDetailDTO.AddedFrom = "Web";
                            //objToolAssetQuantityDetailDTO.EditedFrom = "Web";
                            //objToolAssetQuantityDetailDTO.WhatWhereAction = "KitController>>ToolQtyMoveIn";
                            //objToolAssetQuantityDetailDTO.ReceivedDate = null;
                            //objToolAssetQuantityDetailDTO.InitialQuantityWeb = objToolMasterDTO.Quantity;
                            //objToolAssetQuantityDetailDTO.InitialQuantityPDA = 0;
                            //objToolAssetQuantityDetailDTO.ExpirationDate = null;
                            //objToolAssetQuantityDetailDTO.EditedOnAction = "Tool was Checkin from Web While Kit moveout.";
                            //objToolAssetQuantityDetailDTO.CreatedBy = SessionHelper.UserID;
                            //objToolAssetQuantityDetailDTO.UpdatedBy = SessionHelper.UserID;
                            //objToolAssetQuantityDetailDTO.IsDeleted = false;
                            //objToolAssetQuantityDetailDTO.IsArchived = false;

                            //ToolAssetQuantityDetailDAL objToolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
                            //objToolAssetQuantityDetailDAL.Insert(objToolAssetQuantityDetailDTO, false, ReferalAction: "Move Out");
                        }
                    }
                }

                if (!string.IsNullOrEmpty(msg))
                    return Json(new { Status = false, Message = msg }, JsonRequestBehavior.AllowGet);

                return Json(new { Status = true, Message = "Success" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                ToolMoveInOutDtlDAL = null;
            }
        }
        public JsonResult ToolQtyToMoveBulk(double qty, Guid ToolKitGuid, string MoveType, List<Guid> KitDetailGuids)
        {
            string msg = string.Empty;
            ToolMoveInOutDetailDAL objToolMoveInOutDetailDAL = new ToolMoveInOutDetailDAL(SessionHelper.EnterPriseDBName);
            List<ToolMoveInOutDetailDTO> MoveInDTO = new List<ToolMoveInOutDetailDTO>();
            ToolMoveInOutDetailDAL ToolMoveInOutDtlDAL = new ToolMoveInOutDetailDAL(SessionHelper.EnterPriseDBName);
            bool AllowToolOrdering = SessionHelper.AllowToolOrdering;
            ToolMasterDAL objToolMasterDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            foreach (Guid guid in KitDetailGuids)
            {

                ToolMoveInOutDetailDTO item = new ToolMoveInOutDetailDTO();
                ToolDetailDAL objToolDetailDAL = new ToolDetailDAL(SessionHelper.EnterPriseDBName);
                ToolDetailDTO objToolDetailDTO = objToolDetailDAL.GetRecord(guid.ToString(), SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false);

                double totalMoveQty = qty * objToolDetailDTO.QuantityPerKit.GetValueOrDefault(0);

                if (MoveType == "IN")
                {

                    item.GUID = guid;
                    item.CreatedBy = UserID;
                    item.LastUpdatedBy = UserID;
                    item.ToolDetailGUID = ToolKitGuid;
                    item.ToolItemGUID = objToolDetailDTO.ToolItemGUID;
                    item.Quantity = totalMoveQty;
                    item.Room = RoomID;
                    item.ReasonFromMove = "From Kit Page";
                    item.CompanyID = CompanyID;
                    msg += ToolMoveInOutDtlDAL.QtyToMoveIn(item, RoomID, CompanyID, UserID, SessionHelper.EnterPriceID, ResourceHelper.CurrentCult.Name, AllowToolOrdering);

                    //ToolMasterDTO objToolMasterDTO = objToolMasterDAL.GetToolListByID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, item.ToolItemGUID, null);
                    //ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO = new ToolAssetQuantityDetailDTO();
                    //objToolAssetQuantityDetailDTO.ToolGUID = objToolMasterDTO.GUID;

                    //objToolAssetQuantityDetailDTO.AssetGUID = null;


                    //objToolAssetQuantityDetailDTO.ToolBinID = objToolMasterDTO.ToolLocationDetailsID;
                    //objToolAssetQuantityDetailDTO.Quantity = 0;
                    //objToolAssetQuantityDetailDTO.RoomID = SessionHelper.RoomID;
                    //objToolAssetQuantityDetailDTO.CompanyID = SessionHelper.CompanyID;
                    //objToolAssetQuantityDetailDTO.Created = DateTimeUtility.DateTimeNow;
                    //objToolAssetQuantityDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                    //objToolAssetQuantityDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    //objToolAssetQuantityDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    //objToolAssetQuantityDetailDTO.AddedFrom = "Web";
                    //objToolAssetQuantityDetailDTO.EditedFrom = "Web";
                    //objToolAssetQuantityDetailDTO.WhatWhereAction = "KitController>>ToolQtyToMoveInBulk";
                    //objToolAssetQuantityDetailDTO.ReceivedDate = null;
                    //objToolAssetQuantityDetailDTO.InitialQuantityWeb = 0;
                    //objToolAssetQuantityDetailDTO.InitialQuantityPDA = 0;
                    //objToolAssetQuantityDetailDTO.ExpirationDate = null;
                    //objToolAssetQuantityDetailDTO.EditedOnAction = "Tool was CheckOut from Web While Kit movein Bulk.";
                    //objToolAssetQuantityDetailDTO.CreatedBy = SessionHelper.UserID;
                    //objToolAssetQuantityDetailDTO.UpdatedBy = SessionHelper.UserID;
                    //objToolAssetQuantityDetailDTO.IsDeleted = false;
                    //objToolAssetQuantityDetailDTO.IsArchived = false;

                    //ToolAssetQuantityDetailDAL objToolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
                    //double Quantity = 0;
                    //Quantity = item.Quantity;
                    //objToolAssetQuantityDetailDAL.UpdateOrInsert(objToolAssetQuantityDetailDTO, Quantity, ReferalAction: "Move In");

                }
                else if (MoveType == "OUT")
                {
                    ToolMasterDTO objToolMasterDTO = null;
                    item = objToolMoveInOutDetailDAL.GetToolMoveInOutDetailByToolDetailGUID(guid, false, false);
                    if (item != null)
                    {

                        item.GUID = guid;
                        item.CreatedBy = UserID;
                        item.LastUpdatedBy = UserID;
                        item.ToolDetailGUID = ToolKitGuid;
                        item.ToolItemGUID = objToolDetailDTO.ToolItemGUID;
                        item.LastUpdatedBy = UserID;
                        item.Room = RoomID;
                        item.ReasonFromMove = "From Kit Page";
                        item.CompanyID = CompanyID;
                        item.Quantity = totalMoveQty;

                        msg += ToolMoveInOutDtlDAL.QtyToMoveOut(item, RoomID, CompanyID, UserID, SessionHelper.EnterPriceID, ResourceHelper.CurrentCult.Name, AllowToolOrdering);


                        //  objToolMasterDTO = objToolMasterDAL.GetToolListByID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, item.ToolItemGUID, null);
                    }
                    else
                    {

                        List<ToolMoveInOutDetailDTO> lstToolMoveInOutDetailDTO = new ToolMoveInOutDetailDAL(SessionHelper.EnterPriseDBName).GetAllToolMoveInOutDetailByToolDetailGUID(item.GUID, false, false);
                        double? MoveInTotal = lstToolMoveInOutDetailDTO.Where(t => t.MoveInOut.ToLower() == "in").Sum(t => t.Quantity);
                        double? MoveOutTotal = lstToolMoveInOutDetailDTO.Where(t => t.MoveInOut.ToLower() == "out").Sum(t => t.Quantity);
                        objToolMasterDTO = objToolMasterDAL.GetToolByGUIDPlain(objToolDetailDTO.ToolItemGUID ?? Guid.Empty);
                        double checkinQty = qty;
                        if (MoveInTotal >= MoveOutTotal)
                        {
                            foreach (ToolMoveInOutDetailDTO t in lstToolMoveInOutDetailDTO)
                            {
                                if (checkinQty > 0)
                                {
                                    double Qty = (t.Quantity >= checkinQty ? checkinQty : t.Quantity);
                                    objToolMoveInOutDetailDAL = new ToolMoveInOutDetailDAL(SessionHelper.EnterPriseDBName);
                                    ToolMoveInOutDetailDTO objToolMoveInDTO = new ToolMoveInOutDetailDTO();
                                    objToolMoveInDTO.GUID = Guid.NewGuid();
                                    objToolMoveInDTO.ToolDetailGUID = item.GUID;
                                    objToolMoveInDTO.ToolItemGUID = item.ToolItemGUID;
                                    objToolMoveInDTO.MoveInOut = "OUT";
                                    objToolMoveInDTO.Quantity = Qty;
                                    objToolMoveInDTO.Created = DateTimeUtility.DateTimeNow;
                                    objToolMoveInDTO.Updated = DateTimeUtility.DateTimeNow;
                                    objToolMoveInDTO.CreatedBy = item.CreatedBy;
                                    objToolMoveInDTO.LastUpdatedBy = item.LastUpdatedBy;
                                    objToolMoveInDTO.CompanyID = CompanyID;
                                    objToolMoveInDTO.Room = RoomID;
                                    objToolMoveInDTO.IsDeleted = false;
                                    objToolMoveInDTO.IsArchived = false;
                                    objToolMoveInDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                    objToolMoveInDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                    objToolMoveInDTO.AddedFrom = "web";
                                    objToolMoveInDTO.EditedFrom = "web";
                                    objToolMoveInDTO.ReasonFromMove = "From Kit Page";
                                    //objToolMoveInDTO.ToolDetailGUID = InOutDTO.ToolDetailGUID;
                                    objToolMoveInDTO.WhatWhereAction = "ToolMoveInOutDetailDAL-->QtyToMoveOut";
                                    new ToolMoveInOutDetailDAL(SessionHelper.EnterPriseDBName).Insert(objToolMoveInDTO);
                                    if (AllowToolOrdering)
                                    {
                                        ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO = new ToolAssetQuantityDetailDTO();
                                        objToolAssetQuantityDetailDTO.ToolGUID = objToolMasterDTO.GUID;

                                        objToolAssetQuantityDetailDTO.AssetGUID = null;


                                        ToolLocationDetailsDAL objToolLocationDetailsDAL = new ToolLocationDetailsDAL(SessionHelper.EnterPriseDBName);
                                        ToolLocationDetailsDTO objToolLocationDetailsDTO = objToolLocationDetailsDAL.GetToolDefaultLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, objToolMasterDTO.GUID, SessionHelper.UserID, "Web", "KitController>>ToolQtytomovein");


                                        objToolAssetQuantityDetailDTO.ToolBinID = objToolLocationDetailsDTO != null ? objToolLocationDetailsDTO.ID : objToolMasterDTO.ToolLocationDetailsID;
                                        objToolAssetQuantityDetailDTO.Quantity = item.Quantity;
                                        objToolAssetQuantityDetailDTO.RoomID = SessionHelper.RoomID;
                                        objToolAssetQuantityDetailDTO.CompanyID = SessionHelper.CompanyID;
                                        objToolAssetQuantityDetailDTO.Created = DateTimeUtility.DateTimeNow;
                                        objToolAssetQuantityDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                                        objToolAssetQuantityDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                                        objToolAssetQuantityDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                                        objToolAssetQuantityDetailDTO.AddedFrom = "Web";
                                        objToolAssetQuantityDetailDTO.EditedFrom = "Web";
                                        objToolAssetQuantityDetailDTO.WhatWhereAction = "KitController>>ToolQtyMoveIn";
                                        objToolAssetQuantityDetailDTO.ReceivedDate = null;
                                        objToolAssetQuantityDetailDTO.InitialQuantityWeb = objToolMasterDTO.Quantity;
                                        objToolAssetQuantityDetailDTO.InitialQuantityPDA = 0;
                                        objToolAssetQuantityDetailDTO.ExpirationDate = null;
                                        objToolAssetQuantityDetailDTO.EditedOnAction = "Tool was Checkin from Web While Kit moveout.";
                                        objToolAssetQuantityDetailDTO.CreatedBy = SessionHelper.UserID;
                                        objToolAssetQuantityDetailDTO.UpdatedBy = SessionHelper.UserID;
                                        objToolAssetQuantityDetailDTO.IsDeleted = false;
                                        objToolAssetQuantityDetailDTO.IsArchived = false;

                                        ToolAssetQuantityDetailDAL objToolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);

                                        if (t.ReasonFromMove == "Order Kit Received")
                                        {
                                            objToolMasterDTO.Quantity = (objToolMasterDTO.Quantity) + Qty;
                                            objToolAssetQuantityDetailDAL.Insert(objToolAssetQuantityDetailDTO, true, ReferalAction: "Move Out", SerialNumber: item.SerialNumber);
                                        }
                                        else
                                        {
                                            objToolAssetQuantityDetailDAL.Insert(objToolAssetQuantityDetailDTO, false, ReferalAction: "Move Out", SerialNumber: item.SerialNumber);
                                        }
                                    }
                                    checkinQty = checkinQty - Qty;
                                }
                            }
                        }
                        objToolDetailDTO.AvailableItemsInWIP = objToolDetailDTO.AvailableItemsInWIP.GetValueOrDefault(0) - ((objToolDetailDTO.QuantityPerKit ?? 0) * qty);
                        objToolDetailDTO.LastUpdatedBy = SessionHelper.UserID;
                        objToolDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objToolDetailDTO.EditedFrom = "Web";
                        objToolDetailDTO.WhatWhereAction = "KitController-->WithoutMovein- out Entry";
                        objToolDetailDAL.Edit(objToolDetailDTO);


                        objToolMasterDTO.EditedFrom = "Web";
                        objToolMasterDTO.Updated = DateTimeUtility.DateTimeNow;
                        objToolMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objToolMasterDTO.Quantity = (objToolMasterDTO.Quantity) + ((objToolDetailDTO.QuantityPerKit ?? 0) * qty);
                        objToolMasterDAL.Edit(objToolMasterDTO);
                    }
                    //if (objToolMasterDTO != null)
                    //{
                    //    ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO = new ToolAssetQuantityDetailDTO();

                    //    objToolAssetQuantityDetailDTO.ToolGUID = objToolMasterDTO.GUID;

                    //    objToolAssetQuantityDetailDTO.AssetGUID = null;


                    //    objToolAssetQuantityDetailDTO.ToolBinID = objToolMasterDTO.ToolLocationDetailsID;
                    //    objToolAssetQuantityDetailDTO.Quantity = ((objToolDetailDTO.QuantityPerKit ?? 0) * qty);
                    //    objToolAssetQuantityDetailDTO.RoomID = SessionHelper.RoomID;
                    //    objToolAssetQuantityDetailDTO.CompanyID = SessionHelper.CompanyID;
                    //    objToolAssetQuantityDetailDTO.Created = DateTimeUtility.DateTimeNow;
                    //    objToolAssetQuantityDetailDTO.Updated = DateTimeUtility.DateTimeNow;
                    //    objToolAssetQuantityDetailDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    //    objToolAssetQuantityDetailDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    //    objToolAssetQuantityDetailDTO.AddedFrom = "Web";
                    //    objToolAssetQuantityDetailDTO.EditedFrom = "Web";
                    //    objToolAssetQuantityDetailDTO.WhatWhereAction = "KitController>>ToolQtyToMoveBulk";
                    //    objToolAssetQuantityDetailDTO.ReceivedDate = null;
                    //    objToolAssetQuantityDetailDTO.InitialQuantityWeb = objToolMasterDTO.Quantity;
                    //    objToolAssetQuantityDetailDTO.InitialQuantityPDA = 0;
                    //    objToolAssetQuantityDetailDTO.ExpirationDate = null;
                    //    objToolAssetQuantityDetailDTO.EditedOnAction = "Tool was Checkin from Web While Kit moveout bulk.";
                    //    objToolAssetQuantityDetailDTO.CreatedBy = SessionHelper.UserID;
                    //    objToolAssetQuantityDetailDTO.UpdatedBy = SessionHelper.UserID;
                    //    objToolAssetQuantityDetailDTO.IsDeleted = false;
                    //    objToolAssetQuantityDetailDTO.IsArchived = false;

                    //    ToolAssetQuantityDetailDAL objToolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
                    //    objToolAssetQuantityDetailDAL.Insert(objToolAssetQuantityDetailDTO, false, ReferalAction: "Move Out");
                    //}
                }
            }
            if (!string.IsNullOrEmpty(msg))
                return Json(new { Status = false, Message = msg }, JsonRequestBehavior.AllowGet);
            return Json(new { Status = true, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }
        private List<CommonDTO> GetGroupOfItems()
        {
            List<CommonDTO> ItemType = new List<CommonDTO>();

            ItemType.Add(new CommonDTO() { ID = 1, Text = "Yes" });
            ItemType.Add(new CommonDTO() { ID = 0, Text = "No" });

            return ItemType;
        }
        public ActionResult ToolKitBuildBreak(Guid KitToolGUID)
        {
            ToolMasterDAL obj = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            ToolMasterDTO objDTO = obj.GetToolByGUIDFull(KitToolGUID);
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("ToolMaster");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }

            ViewBag.ReOrderTypeList = GetReorderTypeOrKitCategory(0);
            ViewBag.KitCategoryList = GetReorderTypeOrKitCategory(1);
            //objDTO.KitCost = objDTO.KitCost.GetValueOrDefault(0) * objDTO.AvailableKitQuantity.GetValueOrDefault(0);
            ToolDetailDAL kitDtlDAL = new ToolDetailDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<ToolDetailDTO> objKits = kitDtlDAL.GetAllRecordsByKitGUID(KitToolGUID, RoomID, CompanyID, false, false, false);
            objDTO.NoOfItemsInKit = objKits.Where(x => x.QuantityReadyForAssembly.GetValueOrDefault(0) > 0).Count();
            objDTO.WIPKitCost = objKits.Where(x => x.AvailableItemsInWIP.GetValueOrDefault(0) > 0).Sum(x => x.AvailableItemsInWIP.GetValueOrDefault(0) * x.Cost.GetValueOrDefault(0));

            return PartialView("_KitToolBuildBreak", objDTO);
        }
        public ActionResult LoadToolKitLineItemsByToolKitMasterDTO(Guid ToolKitGUID)
        {

            ToolMasterDAL objToolMasterDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            ToolMasterDTO ToolMasterDTO = objToolMasterDAL.GetToolByGUIDFull(ToolKitGUID);
            ToolMasterDTO.ToolKitItemList = new ToolDetailDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByKitGUIDNew(ToolKitGUID, SessionHelper.RoomID, SessionHelper.CompanyID, ToolMasterDTO.IsArchived.GetValueOrDefault(false), ToolMasterDTO.IsDeleted.GetValueOrDefault(false), true).ToList();
            return PartialView("_ToolKitLineItemsForBuildBreak", ToolMasterDTO);
            //return PartialView("_ToolKitLineItemsForBuildBreak", ToolMasterDTO);
        }
        public JsonResult ToolUpdateKitCost(ToolMasterDTO KitDTO)
        {
            try
            {
                string message = ResMessage.SaveMessage;
                string status = "ok";
                ToolMasterDAL objToolMstCtrl = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                //KitMasterController objKitMstCtrl = new KitMasterController();
                KitDTO = objToolMstCtrl.GetToolByGUIDFull(KitDTO.GUID);
                if (KitDTO.IsDeleted.GetValueOrDefault(false) || KitDTO.IsArchived.GetValueOrDefault(false) || (!isUpdate && Convert.ToString(Session["IsInsert"]) != "True"))
                {
                    return Json(new { Message = message, Status = status, KitDTO = KitDTO }, JsonRequestBehavior.AllowGet);
                }

                //KitMasterDTO objKitMasterDTO = objKitMstCtrl.UpdateKitCost(KitDTO, SessionHelper.RoomID, SessionHelper.CompanyID);
                ToolDetailDAL kitDtlDAL = new ToolDetailDAL(SessionHelper.EnterPriseDBName);
                IEnumerable<ToolDetailDTO> objKits = kitDtlDAL.GetAllRecordsByKitGUID(KitDTO.GUID, RoomID, CompanyID, false, false, false);
                KitDTO.NoOfItemsInKit = objKits.Where(x => x.QuantityReadyForAssembly.GetValueOrDefault(0) > 0).Count();
                KitDTO.WIPKitCost = objKits.Where(x => x.AvailableItemsInWIP.GetValueOrDefault(0) > 0).Sum(x => x.AvailableItemsInWIP.GetValueOrDefault(0) * x.Cost.GetValueOrDefault(0));


                return Json(new { Message = message, Status = status, KitDTO = KitDTO }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new { Message = "fail", Status = "fail", KitDTO = new KitMasterDTO() }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public JsonResult BuildNewToolKit(string objDTO)
        {
            try
            {
                bool AllowToolOrdering = SessionHelper.AllowToolOrdering;
                JavaScriptSerializer s = new JavaScriptSerializer();
                BuildBreakKitDetail QLDetails = s.Deserialize<BuildBreakKitDetail>(objDTO);

                ToolMasterDAL objToolDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                ToolMasterDTO objToolDTO = objToolDAL.GetToolByGUIDPlain(Guid.Parse(QLDetails.KitGuid));
                objToolDTO.LastUpdatedBy = SessionHelper.UserID;
                objToolDTO.Updated = DateTimeUtility.DateTimeNow;
                objToolDTO.Quantity = objToolDTO.Quantity + Convert.ToDouble(QLDetails.Quantity);
                objToolDAL.Edit(objToolDTO);
                ToolDetailDAL objKitDetailctrl = new ToolDetailDAL(SessionHelper.EnterPriseDBName);
                List<ToolDetailDTO> lstKitDetailDTO = objKitDetailctrl.GetAllRecordsByKitGUID(Guid.Parse(QLDetails.KitGuid), SessionHelper.RoomID, SessionHelper.CompanyID, false, false, true).ToList();
                foreach (var item in lstKitDetailDTO)
                {
                    //if (item.ItemType != 4)
                    //{
                    item.AvailableItemsInWIP = item.AvailableItemsInWIP.GetValueOrDefault(0) - (item.QuantityPerKit * double.Parse(QLDetails.Quantity));
                    item.LastUpdatedBy = SessionHelper.UserID;
                    item.ReceivedOn = DateTimeUtility.DateTimeNow;
                    item.EditedFrom = "Web";
                    objKitDetailctrl.Edit(item);
                    //}
                }

                ToolMoveInOutDetailDTO objToolMoveInOutDetailDTO = new ToolMoveInOutDetailDTO()
                {
                    MoveInOut = "BuildKit",
                    ToolItemGUID = objToolDTO.GUID,

                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    CreatedBy = UserID,
                    LastUpdatedBy = UserID,
                    CompanyID = CompanyID,
                    Room = RoomID,

                    IsArchived = false,
                    IsDeleted = false,

                    Quantity = Convert.ToDouble(QLDetails.Quantity),
                    WhatWhereAction = "ToolMoveInOutDetailDAL-->BuildNewToolKit",
                };

                ToolMoveInOutDetailDAL KitMoveInOut = new ToolMoveInOutDetailDAL(SessionHelper.EnterPriseDBName);
                KitMoveInOut.Insert(objToolMoveInOutDetailDTO);

                // ToolMasterDTO objToolMasterDTO = objToolDAL.GetToolNameBySerial(objToolDTO.Serial, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (AllowToolOrdering)
                {
                    ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO = new ToolAssetQuantityDetailDTO();
                    objToolAssetQuantityDetailDTO.ToolGUID = objToolDTO.GUID;

                    objToolAssetQuantityDetailDTO.AssetGUID = null;


                    objToolAssetQuantityDetailDTO.ToolBinID = objToolDTO.ToolLocationDetailsID;
                    objToolAssetQuantityDetailDTO.Quantity = Convert.ToDouble(QLDetails.Quantity);
                    objToolAssetQuantityDetailDTO.RoomID = SessionHelper.RoomID;
                    objToolAssetQuantityDetailDTO.CompanyID = SessionHelper.CompanyID;
                    objToolAssetQuantityDetailDTO.Created = objToolDTO.Created;
                    objToolAssetQuantityDetailDTO.Updated = objToolDTO.Updated ?? DateTimeUtility.DateTimeNow;
                    objToolAssetQuantityDetailDTO.ReceivedOnWeb = objToolDTO.ReceivedOnWeb;
                    objToolAssetQuantityDetailDTO.ReceivedOn = objToolDTO.ReceivedOn;
                    objToolAssetQuantityDetailDTO.AddedFrom = "Web";
                    objToolAssetQuantityDetailDTO.EditedFrom = "Web";
                    objToolAssetQuantityDetailDTO.WhatWhereAction = "KitController>>BuildNewToolKit";
                    objToolAssetQuantityDetailDTO.ReceivedDate = null;
                    objToolAssetQuantityDetailDTO.InitialQuantityWeb = Convert.ToDouble(QLDetails.Quantity);
                    objToolAssetQuantityDetailDTO.InitialQuantityPDA = 0;
                    objToolAssetQuantityDetailDTO.ExpirationDate = null;
                    objToolAssetQuantityDetailDTO.EditedOnAction = "Tool Kit Update From Web Page. Tool Kit Quantity Build.";
                    objToolAssetQuantityDetailDTO.CreatedBy = SessionHelper.UserID;
                    objToolAssetQuantityDetailDTO.UpdatedBy = SessionHelper.UserID;
                    objToolAssetQuantityDetailDTO.IsDeleted = false;
                    objToolAssetQuantityDetailDTO.IsArchived = false;

                    ToolAssetQuantityDetailDAL objToolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
                    objToolAssetQuantityDetailDAL.Insert(objToolAssetQuantityDetailDTO, false, "AdjCredit", ReferalAction: "Kit Build");
                }
                return Json(new { Message = ResCommon.RecordsSavedSuccessfully, Status = "OK" }, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                return Json(new { Status = "fail", Message = "fail", ReturnDTO = objDTO }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult BreakNewToolKit(MoveInOutQtyDetail objMoveQty)
        {
            try
            {
                string status = "fail";
                //KitMoveInOutDetailController kitMoveInOutCtrl = new KitMoveInOutDetailController();
                ToolMoveInOutDetailDAL kitMoveInOutCtrl = new ToolMoveInOutDetailDAL(SessionHelper.EnterPriseDBName);
                bool AllowToolOrdering = SessionHelper.AllowToolOrdering;
                ToolMasterDAL objToolMasterDAL = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
                ResponseMessage RespMsg = kitMoveInOutCtrl.BreakToolKit(objMoveQty, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, SessionHelper.EnterPriceID, ResourceHelper.CurrentCult.Name);
                if (RespMsg.IsSuccess)
                {
                    status = "ok";
                    if (AllowToolOrdering)
                    {
                        ToolMasterDTO objDTO = objToolMasterDAL.GetToolByGUIDFull(Guid.Parse(objMoveQty.ItemGUID));

                        ToolAssetQuantityDetailDTO objToolAssetQuantityDetailDTO = new ToolAssetQuantityDetailDTO();
                        objToolAssetQuantityDetailDTO.ToolGUID = objDTO.GUID;

                        objToolAssetQuantityDetailDTO.AssetGUID = null;


                        objToolAssetQuantityDetailDTO.ToolBinID = objDTO.ToolLocationDetailsID;
                        objToolAssetQuantityDetailDTO.Quantity = objMoveQty.TotalQty;
                        objToolAssetQuantityDetailDTO.RoomID = SessionHelper.RoomID;
                        objToolAssetQuantityDetailDTO.CompanyID = SessionHelper.CompanyID;
                        objToolAssetQuantityDetailDTO.Created = objDTO.Created;
                        objToolAssetQuantityDetailDTO.Updated = objDTO.Updated ?? DateTimeUtility.DateTimeNow;
                        objToolAssetQuantityDetailDTO.ReceivedOnWeb = objDTO.ReceivedOnWeb;
                        objToolAssetQuantityDetailDTO.ReceivedOn = objDTO.ReceivedOn;
                        objToolAssetQuantityDetailDTO.AddedFrom = "Web";
                        objToolAssetQuantityDetailDTO.EditedFrom = "Web";
                        objToolAssetQuantityDetailDTO.WhatWhereAction = "KitController>>BreakNewToolKit";
                        objToolAssetQuantityDetailDTO.ReceivedDate = null;
                        objToolAssetQuantityDetailDTO.InitialQuantityWeb = objMoveQty.TotalQty;
                        objToolAssetQuantityDetailDTO.InitialQuantityPDA = 0;
                        objToolAssetQuantityDetailDTO.ExpirationDate = null;
                        objToolAssetQuantityDetailDTO.EditedOnAction = "Tool Kit Update From Web Page. Tool Kit Quantity Break.";
                        objToolAssetQuantityDetailDTO.CreatedBy = SessionHelper.UserID;
                        objToolAssetQuantityDetailDTO.UpdatedBy = SessionHelper.UserID;
                        objToolAssetQuantityDetailDTO.IsDeleted = false;
                        objToolAssetQuantityDetailDTO.IsArchived = false;

                        ToolAssetQuantityDetailDAL objToolAssetQuantityDetailDAL = new ToolAssetQuantityDetailDAL(SessionHelper.EnterPriseDBName);
                        objToolAssetQuantityDetailDAL.UpdateOrInsert(objToolAssetQuantityDetailDTO, objMoveQty.TotalQty);
                    }
                }
                return Json(new { Message = RespMsg.Message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult LoadLocationsofTool(Guid ToolGUID, int? AddCount)
        {
            ViewBag.ToolGUID = ToolGUID;
            LocationMasterDAL objLocationMasterDAL = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
            //ViewBag.DefaultLocationBag = objBinMasterDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(t => t.IsStagingLocation != true);

            List<LocationMasterDTO> lstBinReplanish = new List<LocationMasterDTO>();
            if (Session["ToolBinReplanish"] != null)
            {
                lstBinReplanish = (List<LocationMasterDTO>)Session["ToolBinReplanish"];
            }
            else
            {
                RoomDAL objRoomDal = new RoomDAL(SessionHelper.EnterPriseDBName);
                LocationMasterDTO objLocationmasterDTO = new LocationMasterDTO();

            }

            if (AddCount != null && AddCount > 0)
            {
                for (int i = 0; i < AddCount; i++)
                {
                    lstBinReplanish.Add(new LocationMasterDTO() { ID = 0, SessionSr = lstBinReplanish.Count + 1, ToolGUID = ToolGUID, Room = SessionHelper.RoomID, CompanyID = SessionHelper.CompanyID, LastUpdated = DateTimeUtility.DateTimeNow, LastUpdatedBy = SessionHelper.UserID, Created = DateTimeUtility.DateTimeNow, CreatedBy = SessionHelper.UserID, GUID = Guid.NewGuid() });
                }
            }

            return PartialView("_BinReplanishLocations", lstBinReplanish.OrderBy(t => t.Location).ToList());


        }
        public JsonResult SavetoSeesionBinReplanishSingle(Int64 ID, Int32 SessionSr, Guid GUID, Guid ToolGUID, Int64 BinID, string BinLocation, bool? IsDefault)
        {

            List<LocationMasterDTO> lstBinReplanish = null;
            if (Session["ToolBinReplanish"] != null)
            {
                lstBinReplanish = (List<LocationMasterDTO>)Session["ToolBinReplanish"];
            }
            else
            {
                lstBinReplanish = new List<LocationMasterDTO>();
            }




            if (ID > 0 && SessionSr == 0)
            {
                LocationMasterDTO objDTO = lstBinReplanish.Where(t => t.ID == ID).FirstOrDefault();
                if (objDTO != null)
                {
                    lstBinReplanish.Remove(lstBinReplanish.Where(t => t.ID == ID).FirstOrDefault());
                    objDTO.ID = ID;
                    //  objDTO.BinID = BinID;
                    objDTO.Location = BinLocation;

                    objDTO.ToolGUID = ToolGUID;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objDTO.GUID = GUID;
                    objDTO.IsDefault = IsDefault;
                    objDTO.Room = SessionHelper.RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.RoomName = SessionHelper.RoomName;
                    objDTO.UpdatedByName = SessionHelper.UserName;
                    objDTO.CreatedByName = SessionHelper.UserName;

                    //   if (eVMISensorID != 0)

                    lstBinReplanish.Add(objDTO);
                }
            }
            else
            {
                if (SessionSr > 0)
                {
                    LocationMasterDTO objDTO = lstBinReplanish.Where(t => t.SessionSr == SessionSr).FirstOrDefault();
                    if (objDTO != null)
                    {
                        lstBinReplanish.Remove(lstBinReplanish.Where(t => t.SessionSr == SessionSr).FirstOrDefault());
                        objDTO.ID = ID;
                        // objDTO.BinID = BinID;
                        objDTO.Location = BinLocation;
                        objDTO.IsDefault = IsDefault;
                        objDTO.ToolGUID = ToolGUID;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        objDTO.GUID = Guid.NewGuid();

                        objDTO.Room = SessionHelper.RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.RoomName = SessionHelper.RoomName;
                        objDTO.UpdatedByName = SessionHelper.UserName;
                        objDTO.CreatedByName = SessionHelper.UserName;

                        // if (eVMISensorID != 0)

                        lstBinReplanish.Add(objDTO);
                    }
                    else
                    {
                        objDTO = new LocationMasterDTO();
                        objDTO.ID = 0;
                        // objDTO.BinID = BinID;
                        objDTO.Location = BinLocation;
                        objDTO.IsDefault = IsDefault;
                        objDTO.ToolGUID = ToolGUID;
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.Created = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        objDTO.GUID = GUID;

                        objDTO.Room = SessionHelper.RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.RoomName = SessionHelper.RoomName;
                        objDTO.UpdatedByName = SessionHelper.UserName;
                        objDTO.CreatedByName = SessionHelper.UserName;
                        objDTO.SessionSr = lstBinReplanish.Count + 1;

                        lstBinReplanish.Add(objDTO);
                    }
                }
                else
                {
                    LocationMasterDTO objDTO = new LocationMasterDTO();
                    objDTO.ID = 0;
                    //   objDTO.ID = BinID;
                    objDTO.Location = BinLocation;
                    objDTO.IsDefault = IsDefault;
                    objDTO.ToolGUID = ToolGUID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objDTO.GUID = GUID;
                    objDTO.Room = SessionHelper.RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.RoomName = SessionHelper.RoomName;
                    objDTO.UpdatedByName = SessionHelper.UserName;
                    objDTO.CreatedByName = SessionHelper.UserName;
                    objDTO.SessionSr = lstBinReplanish.Count + 1;

                    lstBinReplanish.Add(objDTO);
                }
            }
            Session["ToolBinReplanish"] = lstBinReplanish;

            return Json(new { status = "ok" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SavetoSeesionBinReplanishSingleNew(Int64 ID, Int32 SessionSr, Guid GUID, Guid ToolGUID, Int64 BinID, string BinLocation, bool? IsDefault)
        {
            Guid newGUID = new Guid();
            List<LocationMasterDTO> lstBinReplanish = null;
            if (Session["ToolBinReplanish"] != null)
            {
                lstBinReplanish = (List<LocationMasterDTO>)Session["ToolBinReplanish"];
            }
            else
            {
                lstBinReplanish = new List<LocationMasterDTO>();
            }



            if (ID > 0 && SessionSr == 0)
            {
                LocationMasterDTO objDTO = lstBinReplanish.Where(t => t.ID == ID).FirstOrDefault();
                if (objDTO != null)
                {
                    lstBinReplanish.Remove(lstBinReplanish.Where(t => t.ID == ID).FirstOrDefault());
                    objDTO.ID = ID;
                    //  objDTO.BinID = BinID;
                    objDTO.Location = BinLocation;
                    objDTO.IsDefault = IsDefault;
                    objDTO.ToolGUID = ToolGUID;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objDTO.GUID = GUID;
                    newGUID = objDTO.GUID;
                    objDTO.Room = SessionHelper.RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.RoomName = SessionHelper.RoomName;
                    objDTO.UpdatedByName = SessionHelper.UserName;
                    objDTO.CreatedByName = SessionHelper.UserName;

                    lstBinReplanish.Add(objDTO);
                }
            }
            else
            {
                if (SessionSr > 0)
                {
                    LocationMasterDTO objDTO = lstBinReplanish.Where(t => t.SessionSr == SessionSr).FirstOrDefault();
                    if (objDTO != null)
                    {
                        lstBinReplanish.Remove(lstBinReplanish.Where(t => t.SessionSr == SessionSr).FirstOrDefault());
                        objDTO.ID = ID;
                        // objDTO.BinID = BinID;
                        objDTO.Location = BinLocation;
                        objDTO.IsDefault = IsDefault;
                        objDTO.ToolGUID = ToolGUID;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        objDTO.GUID = GUID;
                        newGUID = objDTO.GUID;
                        objDTO.Room = SessionHelper.RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.RoomName = SessionHelper.RoomName;
                        objDTO.UpdatedByName = SessionHelper.UserName;
                        objDTO.CreatedByName = SessionHelper.UserName;

                        lstBinReplanish.Add(objDTO);
                    }
                    else
                    {
                        objDTO = new LocationMasterDTO();
                        objDTO.ID = 0;
                        // objDTO.BinID = BinID;
                        objDTO.Location = BinLocation;
                        objDTO.IsDefault = IsDefault;
                        objDTO.ToolGUID = ToolGUID;
                        objDTO.CreatedBy = SessionHelper.UserID;
                        objDTO.Created = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        objDTO.GUID = GUID;
                        newGUID = objDTO.GUID;
                        objDTO.Room = SessionHelper.RoomID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.RoomName = SessionHelper.RoomName;
                        objDTO.UpdatedByName = SessionHelper.UserName;
                        objDTO.CreatedByName = SessionHelper.UserName;
                        objDTO.SessionSr = lstBinReplanish.Count + 1;

                        lstBinReplanish.Add(objDTO);
                    }
                }
                else
                {
                    LocationMasterDTO objDTO = new LocationMasterDTO();
                    objDTO.ID = 0;
                    //   objDTO.ID = BinID;
                    objDTO.Location = BinLocation;
                    objDTO.IsDefault = IsDefault;
                    objDTO.ToolGUID = ToolGUID;
                    objDTO.CreatedBy = SessionHelper.UserID;
                    objDTO.Created = DateTimeUtility.DateTimeNow;
                    objDTO.LastUpdatedBy = SessionHelper.UserID;
                    objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                    objDTO.GUID = GUID;
                    newGUID = objDTO.GUID;
                    objDTO.Room = SessionHelper.RoomID;
                    objDTO.CompanyID = SessionHelper.CompanyID;
                    objDTO.RoomName = SessionHelper.RoomName;
                    objDTO.UpdatedByName = SessionHelper.UserName;
                    objDTO.CreatedByName = SessionHelper.UserName;
                    objDTO.SessionSr = lstBinReplanish.Count + 1;

                    lstBinReplanish.Add(objDTO);
                }
            }
            Session["ToolBinReplanish"] = lstBinReplanish;

            return Json(new { status = "ok", newGUID = newGUID }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeletetoSeesionBinReplanishSingle(Int64 ID, Guid GUID, Guid ToolGUID, Int64 BinID)
        {
            List<LocationMasterDTO> lstBinReplanish = null;
            if (Session["ToolBinReplanish"] != null)
            {
                lstBinReplanish = (List<LocationMasterDTO>)Session["ToolBinReplanish"];
            }
            else
            {
                lstBinReplanish = new List<LocationMasterDTO>();
            }
            ///Delete the Records......
            if (ID > 0)
            {
                try
                {
                    lstBinReplanish.Remove(lstBinReplanish.Where(t => t.ID == ID).FirstOrDefault());
                    Session["ToolBinReplanish"] = lstBinReplanish;
                }
                catch
                {
                    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "deleted" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    lstBinReplanish.Remove(lstBinReplanish.Where(t => t.GUID == GUID).FirstOrDefault());
                    Session["ToolBinReplanish"] = lstBinReplanish;
                }
                catch
                {
                    return Json(new { status = "error" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { status = "deleted" }, JsonRequestBehavior.AllowGet);
            }
        }
        /****************************************** Tool Kit methods end *******************************/

        public JsonResult GetLotOrSerailNumberListForMoveOut(int maxRows, string name_startsWith, Guid? ItemGuid, long BinID, Guid KitDetailGUID, Guid? KitGUID, string prmSerialLotNos = null)
        {
            bool IsStagginLocation = false;

            BinMasterDTO objLocDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(BinID, SessionHelper.RoomID, SessionHelper.CompanyID);
            if (objLocDTO != null && objLocDTO.ID > 0 && objLocDTO.IsStagingLocation)
            {
                IsStagginLocation = true;
            }

            KitMoveInOutDetailDAL kitMoveInOutDetail = new KitMoveInOutDetailDAL(SessionHelper.EnterPriseDBName);
            List<ItemLocationLotSerialDTO> objItemLocationLotSerialDTO = kitMoveInOutDetail.GetItemLocationsWithLotSerialsForMoveOut(KitDetailGUID, ItemGuid.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, BinID, 0, "0", string.Empty, (IsStagginLocation ? "1" : "0"));

            if (!objItemLocationLotSerialDTO.Any() && KitGUID != Guid.Empty)
            {
                objItemLocationLotSerialDTO = kitMoveInOutDetail.GetItemLocationsWithLotSerialsForMoveOut(KitGUID.GetValueOrDefault(Guid.Empty), ItemGuid.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
            }

            string[] arrSerialLotNos = prmSerialLotNos.Split(new string[] { "|#|" }, StringSplitOptions.RemoveEmptyEntries);
            if (!string.IsNullOrWhiteSpace(name_startsWith))
            {
                name_startsWith = name_startsWith.Trim();
            }
            var lstLotSr =
                objItemLocationLotSerialDTO.Where(x => x.LotOrSerailNumber.Contains(name_startsWith) && !arrSerialLotNos.Contains(x.LotOrSerailNumber)).Select(x => new { x.LotOrSerailNumber }).Distinct();

            if (lstLotSr.Count() == 0)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            return Json(lstLotSr, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ValidateSerialLotNumberForKitMoveOut(Guid? ItemGuid, string SerialOrLotNumber, long BinID, Guid KitDetailGUID, Guid? KitGUID)
        {
            bool IsStagginLocation = false;

            if (!string.IsNullOrWhiteSpace(SerialOrLotNumber))
            {
                SerialOrLotNumber = SerialOrLotNumber.Trim();
            }
            BinMasterDTO objLocDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(BinID, SessionHelper.RoomID, SessionHelper.CompanyID);

            if (objLocDTO != null && objLocDTO.ID > 0 && objLocDTO.IsStagingLocation)
            {
                IsStagginLocation = true;
            }

            PullTransactionDAL objPullDetails = new PullTransactionDAL(SessionHelper.EnterPriseDBName);
            KitMoveInOutDetailDAL kitMoveInOutDetail = new KitMoveInOutDetailDAL(SessionHelper.EnterPriseDBName);
            ItemLocationLotSerialDTO objItemLocationLotSerialDTO = kitMoveInOutDetail.GetItemLocationsWithLotSerialsForMoveOut(KitDetailGUID, ItemGuid.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, BinID, 0, "0", SerialOrLotNumber, (IsStagginLocation ? "1" : "0")).FirstOrDefault();

            if (objItemLocationLotSerialDTO == null)
            {
                objItemLocationLotSerialDTO = kitMoveInOutDetail.GetItemLocationsWithLotSerialsForMoveOut(KitGUID.GetValueOrDefault(Guid.Empty), ItemGuid.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID).FirstOrDefault();
            }

            if (objItemLocationLotSerialDTO == null)
            {
                objItemLocationLotSerialDTO = new ItemLocationLotSerialDTO();
                objItemLocationLotSerialDTO.BinID = BinID;
                objItemLocationLotSerialDTO.ID = BinID;
                objItemLocationLotSerialDTO.ItemGUID = ItemGuid;
                objItemLocationLotSerialDTO.LotOrSerailNumber = string.Empty;
                objItemLocationLotSerialDTO.Expiration = string.Empty;
                objItemLocationLotSerialDTO.BinNumber = string.Empty;
            }
            return Json(objItemLocationLotSerialDTO);
        }
    }

    public class BuildBreakKitDetail
    {
        public string LotNumber { get; set; }
        public string ExpirationDate { get; set; }
        public string Quantity { get; set; }
        public string KitGuid { get; set; }
        public string LocationID { get; set; }
        public string ReceiveDate { get; set; }
        public string Cost { get; set; }
        public bool Consignment { get; set; }
    }

    public class ReorderTypeAndKitCategory
    {
        public string ReOrderType { get; set; }
        public bool typeValue { get; set; }
        public string KitCategory { get; set; }
        public int CategoryValue { get; set; }
    }
}
