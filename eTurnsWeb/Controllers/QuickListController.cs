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
    public class QuickListController : eTurnsControllerBase
    {
        /// <summary>
        /// Quick List
        /// </summary>
        /// <returns></returns>
        public ActionResult QuickList()
        {
            Session["QuicklistType"] = null;
            ViewBag.QuickListType = GetQuickListType();
            return View();
        }

        /// <summary>
        /// _CreateQuickList
        /// </summary>
        /// <returns></returns>
        public PartialViewResult _CreateQuickList()
        {
            return PartialView();
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult QuickListCreate()
        {
            QuickListMasterDTO objDTO = new QuickListMasterDTO()
            {
                ID = 0,
                Created = DateTimeUtility.DateTimeNow,
                LastUpdated = DateTimeUtility.DateTimeNow,
                CreatedBy = SessionHelper.UserID,
                CreatedByName = SessionHelper.UserName,
                LastUpdatedBy = SessionHelper.UserID,
                Room = SessionHelper.RoomID,
                CompanyID = SessionHelper.CompanyID,
                RoomName = SessionHelper.RoomName,
                UpdatedByName = SessionHelper.UserName,
                Type = 1,
            };
            ViewBag.UDFs = GetUDFDataPageWise("QuickListMaster");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            ViewBag.QuickListType = GetQuickListType();

            Session["QuicklistType"] = objDTO.Type;

            return PartialView("_CreateQuickList", objDTO);
        }

        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>
        public ActionResult QuickListEdit(string QuickListGUID)
        {
            //Session["IsInsert"] = "";
            //QuickListAPIController obj = new QuickListAPIController();
            QuickListDAL obj = new QuickListDAL(SessionHelper.EnterPriseDBName);
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            QuickListMasterDTO objDTO = obj.GetRecord(QuickListGUID, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted);
            if (objDTO != null)
            {
                objDTO.CreatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                objDTO.UpdatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.LastUpdated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
            }

            Session["QuicklistType"] = objDTO.Type;

            ViewBag.QuickListType = GetQuickListType();
            ViewBag.UDFs = GetUDFDataPageWise("QuickListMaster");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;

            return PartialView("_CreateQuickList", objDTO);
        }

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult GetQuickListItems(QuickListJQueryDataTableParamModel param)
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
            sortColumnName = Request["SortingField"];
            //Int64 QuickListID = 0;
            //Int64.TryParse(Request["QuickListID"], out QuickListID);
            bool IsArchived = false;
            bool IsDeleted = false;
            if (!string.IsNullOrEmpty(Convert.ToString(Request["IsArchived"])))
                IsArchived = bool.Parse(Convert.ToString(Request["IsArchived"]));

            if (!string.IsNullOrEmpty(Convert.ToString(Request["IsDeleted"])))
                IsDeleted = bool.Parse(Convert.ToString(Request["IsDeleted"]));

            if (!string.IsNullOrEmpty(sortColumnName))
                sortColumnName = sortColumnName.Trim();

            Int64 QuickListHistoryID = 0;
            if (!string.IsNullOrEmpty(Request["QuickListHistoryID"]))
                Int64.TryParse(Request["QuickListHistoryID"], out QuickListHistoryID);

            // set the default column sorting here, if first time then required to set 
            if (QuickListHistoryID > 0)
            {
                if (sortColumnName == "0" || sortColumnName == "undefined")
                    sortColumnName = "HistoryID";
            }
            //else if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID";
            else if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "ID";
            }
            else
                sortColumnName = "ID";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;
            int TotalRecordCount = 0;

            //QuickListAPIController quickListApiController = new QuickListAPIController();
            QuickListDAL quickListApiController = new QuickListDAL(SessionHelper.EnterPriseDBName);
            List<QuickListDetailDTO> DataFromDB = new List<QuickListDetailDTO>();

            if (QuickListHistoryID <= 0)
                DataFromDB = quickListApiController.GetQuickListItemsPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, Request["QuickListGUID"].ToString(), IsDeleted, IsArchived, SessionHelper.UserSupplierIds);
            else if (QuickListHistoryID > 0)
                DataFromDB = quickListApiController.GetQuickListItemsPagedRecordsOfHistory(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, Request["QuickListHistoryGUID"].ToString());

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public JsonResult DeleteQuickListMasterRecords(string ids)
        {
            try
            {
                //QuickListAPIController obj = new QuickListAPIController();
                QuickListDAL obj = new QuickListDAL(SessionHelper.EnterPriseDBName);
                obj.DeleteRecords(ids, SessionHelper.UserID, SessionHelper.CompanyID, SessionHelper.RoomID);
                //return "ok";
                return Json(new { Message = ResMessage.DeletedSuccessfully, Status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //return ex.Message;
                return Json(new { Message = ex.Message, Status = "ok" }, JsonRequestBehavior.AllowGet);

            }
        }

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult GetQuickList(JQueryDataTableParamModel param)
        {
            QuickListMasterDTO entity = new QuickListMasterDTO();
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

            //if (sortDirection == "asc")
            //    sortColumnName = sortColumnName + " asc";
            //else
            //    sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;
            int TotalRecordCount = 0;

            //QuickListAPIController controller = new QuickListAPIController();
            QuickListDAL controller = new QuickListDAL(SessionHelper.EnterPriseDBName);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            IEnumerable<QuickListMasterDTO> DataFromDB = controller.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, "1,2,3", Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone);

            var result = from u in DataFromDB
                         select new
                         {
                             ID = u.ID,
                             Name = u.Name,
                             Comment = u.Comment,
                             CompanyID = u.CompanyID,
                             IsDeleted = u.IsDeleted,
                             IsArchived = u.IsArchived,
                             NoOfItems = u.NoOfItems,
                             Type = u.Type,
                             Created = u.Created,
                             LastUpdated = u.LastUpdated,
                             CreatedByName = u.CreatedByName,
                             UpdatedByName = u.UpdatedByName,
                             RoomName = u.RoomName,
                             CreatedBy = u.CreatedBy,
                             LastUpdatedBy = u.LastUpdatedBy,
                             Room = u.Room,
                             GUID = u.GUID,
                             UDF1 = u.UDF1,
                             UDF2 = u.UDF2,
                             UDF3 = u.UDF3,
                             UDF4 = u.UDF4,
                             UDF5 = u.UDF5,
                             AppendedBarcodeString = u.AppendedBarcodeString,
                             TotalRecords = u.TotalRecords,
                             ReceivedOn = u.ReceivedOn,
                             ReceivedOnWeb = u.ReceivedOnWeb,
                             AddedFrom = u.AddedFrom,
                             EditedFrom = u.EditedFrom,
                             ReceivedOnDate = FnCommon.ConvertDateByTimeZone(u.ReceivedOn, true, false),// CommonUtility.ConvertDateByTimeZone(u.ReceivedOn, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             ReceivedOnWebDate = FnCommon.ConvertDateByTimeZone(u.ReceivedOnWeb, true, false),//CommonUtility.ConvertDateByTimeZone(u.ReceivedOnWeb, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             CreatedDate = FnCommon.ConvertDateByTimeZone(u.Created, true, false),//CommonUtility.ConvertDateByTimeZone(u.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             UpdatedDate = FnCommon.ConvertDateByTimeZone(u.LastUpdated, true, false),//CommonUtility.ConvertDateByTimeZone(u.LastUpdated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                         };

            return Json(new { sEcho = param.sEcho, iTotalRecords = TotalRecordCount, iTotalDisplayRecords = TotalRecordCount, aaData = result, }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Update Records
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <param name="rowId"></param>
        /// <param name="columnPosition"></param>
        /// <param name="columnId"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string UpdateQuickMasterData(Int64 id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            return value;
        }

        /// <summary>
        /// JSON Record Save - Enter key Save/Update
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Technician"></param>
        /// <returns></returns>
        /// 

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult SaveQL(QuickListMasterDTO objDTO)
        {
            string message = "";
            string status = "";
            bool va = ModelState.IsValid;
            if (!ModelState.IsValid)
            {
                status = "fail";
                IEnumerable<ModelError> errors = ModelState.Keys.SelectMany(key => this.ModelState[key].Errors);
                message = string.Join(",", errors.Select(t => t.ErrorMessage));
                return Json(new { Message = ResMessage.InvalidModel + "\r\n" + message, Status = status }, JsonRequestBehavior.AllowGet);
            }


            //QuickListAPIController obj = new QuickListAPIController();
            QuickListDAL obj = new QuickListDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

            if (string.IsNullOrEmpty(objDTO.Name))
            {
                message = string.Format(ResMessage.Required, ResQuickList.Name);
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.UpdatedByName = SessionHelper.UserName;

            if (objDTO.ID == 0)
            {
                string strOK = objCDAL.DuplicateCheck(objDTO.Name, "add", objDTO.ID, "QuickListMaster", "Name", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResQuickList.Name , objDTO.Name);
                    status = "duplicate";
                }
                else
                {
                    objDTO.GUID = Guid.NewGuid();
                    long ReturnVal = obj.Insert(objDTO);
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
                string strOK = objCDAL.DuplicateCheck(objDTO.Name, "edit", objDTO.ID, "QuickListMaster", "Name", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResQuickList.Name, objDTO.Name);
                    status = "duplicate";
                }
                else
                {
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.EditedFrom = "Web";
                    bool ReturnVal = obj.Edit(objDTO);
                    if (ReturnVal)
                    {
                        message = ResMessage.SaveMessage;  //ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); //string.Format(ResMessage.SaveErrorMsg, hrmResult.StatusCode); // "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }
            Session["IsInsert"] = "True";
            return Json(new { Message = message, Status = status, UpdatedDTO = objDTO }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GetUDFDataPageWise
        /// </summary>
        /// <param name="PageName"></param>
        /// <returns></returns>
        private object GetUDFDataPageWise(string PageName)
        {
            //UDFApiController objUDFApiController = new UDFApiController();
            UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetUDFsByUDFTableNamePlain(PageName, SessionHelper.RoomID, SessionHelper.CompanyID);

            var result = from c in DataFromDB
                         select new UDFDTO
                         {
                             ID = c.ID,
                             CompanyID = c.CompanyID,
                             Room = c.Room,
                             UDFTableName = c.UDFTableName,
                             UDFColumnName = c.UDFColumnName,
                             UDFDefaultValue = c.UDFDefaultValue,
                             UDFOptionsCSV = c.UDFOptionsCSV,
                             UDFControlType = c.UDFControlType,
                             UDFIsRequired = c.UDFIsRequired,
                             UDFIsSearchable = c.UDFIsRequired = c.UDFIsRequired,
                             Created = c.Created,
                             Updated = c.Updated,
                             //UpdatedByName = c.UpdatedByName,
                             //CreatedByName = c.CreatedByName,
                             IsDeleted = c.IsDeleted,
                         };
            return result;
        }

        /// <summary>
        /// GetQuickListType
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetQuickListType()
        {
            List<SelectListItem> returnList = new List<SelectListItem>();
            returnList.Add(new SelectListItem() { Text = eTurns.DTO.QuickListType.General.ToString(), Value = Convert.ToString((int)eTurns.DTO.QuickListType.General) });
            returnList.Add(new SelectListItem() { Text = eTurns.DTO.QuickListType.Asset.ToString(), Value = Convert.ToString((int)eTurns.DTO.QuickListType.Asset) });
            returnList.Add(new SelectListItem() { Text = eTurns.DTO.QuickListType.Count.ToString(), Value = Convert.ToString((int)eTurns.DTO.QuickListType.Count) });
            return returnList;
        }

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public string DeleteQuickListItem(string ids)
        {
            try
            {
                //QuickListAPIController obj = new QuickListAPIController();
                QuickListDAL obj = new QuickListDAL(SessionHelper.EnterPriseDBName);
                obj.DeleteQuickListItemsRecords(ids, SessionHelper.UserID, SessionHelper.CompanyID);
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Update Records
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <param name="rowId"></param>
        /// <param name="columnPosition"></param>
        /// <param name="columnId"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string UpdateQuickListItemData(string para)
        {
            try
            {

                JavaScriptSerializer s = new JavaScriptSerializer();
                QuickListDetailDTO QLDetails = s.Deserialize<QuickListDetailDTO>(para);
                //QuickListAPIController objApi = new QuickListAPIController();
                QuickListDAL objApi = new QuickListDAL(SessionHelper.EnterPriseDBName);
                QLDetails.Room = SessionHelper.RoomID;
                QLDetails.LastUpdatedBy = SessionHelper.UserID;
                QLDetails.CompanyID = SessionHelper.CompanyID;

                QLDetails.ReceivedOn = DateTimeUtility.DateTimeNow;
                QLDetails.EditedFrom = "Web";

                objApi.QuickListItemsEdit(QLDetails);

                return "1";
            }
            catch (Exception)
            {

                return "0";
            }
        }

        /// <summary>
        /// Update Records
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <param name="rowId"></param>
        /// <param name="columnPosition"></param>
        /// <param name="columnId"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public JsonResult UpdateQLItemQtyData(List<QuickListDetailDTO> objQLItemQty)
        {
            try
            {
                if (objQLItemQty != null && objQLItemQty.Count > 0)
                {
                    QuickListDAL objApi = new QuickListDAL(SessionHelper.EnterPriseDBName);
                    BinMasterDAL binDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);

                    //------------------------------------------------------------------------------
                    //
                    UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
                    IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetRequiredUDFsByUDFTableNamePlain("QuickListItems", SessionHelper.RoomID, SessionHelper.CompanyID);
                    string udfRequier = string.Empty;
                    foreach (var item in objQLItemQty)
                    {
                        foreach (var i in DataFromDB)
                        {
                                if (i.UDFColumnName == "UDF1"  && string.IsNullOrWhiteSpace(item.UDF1))
                                {
                                    string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                                    string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                                    if (!string.IsNullOrEmpty(val))
                                        i.UDFDisplayColumnName = val;
                                    else
                                        i.UDFDisplayColumnName = i.UDFColumnName;
                                    udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                                }
                                else if (i.UDFColumnName == "UDF2"  && string.IsNullOrWhiteSpace(item.UDF2))
                                {
                                    string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                                    string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                                    if (!string.IsNullOrEmpty(val))
                                        i.UDFDisplayColumnName = val;
                                    else
                                        i.UDFDisplayColumnName = i.UDFColumnName;
                                    udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                                }
                                else if (i.UDFColumnName == "UDF3"  && string.IsNullOrWhiteSpace(item.UDF3))
                                {
                                    string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                                    string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                                    if (!string.IsNullOrEmpty(val))
                                        i.UDFDisplayColumnName = val;
                                    else
                                        i.UDFDisplayColumnName = i.UDFColumnName;
                                    udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                                }
                                else if (i.UDFColumnName == "UDF4"  && string.IsNullOrWhiteSpace(item.UDF4))
                                {
                                    string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                                    string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                                    if (!string.IsNullOrEmpty(val))
                                        i.UDFDisplayColumnName = val;
                                    else
                                        i.UDFDisplayColumnName = i.UDFColumnName;
                                    udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                                }
                                else if (i.UDFColumnName == "UDF5"  && string.IsNullOrWhiteSpace(item.UDF5))
                                {
                                    string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                                    string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                                    if (!string.IsNullOrEmpty(val))
                                        i.UDFDisplayColumnName = val;
                                    else
                                        i.UDFDisplayColumnName = i.UDFColumnName;
                                    udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                                }

                                if (!string.IsNullOrEmpty(udfRequier))
                                    break;
                            
                        }
                    }

                    if (!string.IsNullOrEmpty(udfRequier))
                    {
                        return Json(new { Massage = udfRequier, success = "fail" }, JsonRequestBehavior.AllowGet);
                    }

                    //------------------------------------------------------------------------------
                    //
                    foreach (var item in objQLItemQty)
                    {
                        if (!string.IsNullOrWhiteSpace(item.BinName))
                        {
                            BinMasterDAL objItemLocationDetailsDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                            BinMasterDTO objBin = objItemLocationDetailsDAL.GetItemBinPlain(item.ItemGUID ?? Guid.Empty, item.BinName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, false);
                            item.BinID = objBin.ID;
                        }
                        else
                        {
                            item.BinID = null;
                        }

                        item.Room = SessionHelper.RoomID;
                        item.LastUpdatedBy = SessionHelper.UserID;
                        item.CompanyID = SessionHelper.CompanyID;
                        item.ReceivedOn = DateTimeUtility.DateTimeNow;
                        item.EditedFrom = "Web";
                        if (!objApi.isItemBinExistinQL(item.QuickListGUID, item.ItemGUID ?? Guid.Empty, item.BinID, item.GUID))
                        {
                            objApi.QuickListItemsEdit(item);
                        }
                    }
                }
                return Json(new { Massage = "ok", success = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new { Massage = "fail", success = "fail" }, JsonRequestBehavior.AllowGet);

            }
        }



        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult GetItemsModelMethod(QuickListJQueryDataTableParamModel param)
        {
            ItemMasterDAL obj = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            int _QuickListType = 1;
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
            int.TryParse(Request["QuickListType"], out _QuickListType);
            bool IsArchived = false; //bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = false; //bool.Parse(Request["IsDeleted"].ToString());

            //make changes to resolve an issue of Sort (WI-431)
            if (sortColumnName == "0" || sortColumnName.Contains("undefined"))
                sortColumnName = "ItemNumber Asc";

            string searchQuery = string.Empty;
            Int64 QLID = 0;
            Int64.TryParse(Request["ParentID"], out QLID);
            int TotalRecordCount = 0;
            string ItemsIDs = "";
            // .Where(x=>x.ItemType != 4); , as Labour Type item not required in this module
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetPagedRecordsForModel(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, ItemsIDs, string.Empty, SessionHelper.UserSupplierIds, RoomDateFormat, CurrentTimeZone, _QuickListType).ToList();

            //if(_QuickListType == 3)
            //{
            //    if (DataFromDB != null && DataFromDB.Count > 0)
            //    {
            //        DataFromDB = DataFromDB.Where(x => x.SerialNumberTracking == false && x.LotNumberTracking == false && x.DateCodeTracking == false).ToList();
            //        if (DataFromDB != null && DataFromDB.Count > 0)
            //        {
            //            TotalRecordCount = DataFromDB.Count;
            //        }
            //        else
            //        {
            //            TotalRecordCount = 0;
            //        }
            //    }
            //    else
            //    {
            //        TotalRecordCount = 0;
            //    }

            //}

            DataFromDB.ForEach(t =>
            {
                t.ItemUDF1 = t.UDF1;
                t.ItemUDF2 = t.UDF2;
                t.ItemUDF3 = t.UDF3;
                t.ItemUDF4 = t.UDF4;
                t.ItemUDF5 = t.UDF5;
                t.ItemUDF6 = t.UDF6;
                t.ItemUDF7 = t.UDF7;
                t.ItemUDF8 = t.UDF8;
                t.ItemUDF9 = t.UDF9;
                t.ItemUDF10 = t.UDF10;

            });

            /*
            var result = from u in DataFromDB
                         select new ItemMasterDTO
                         {
                             ID = u.ID,
                             ItemNumber = u.ItemNumber,
                             ManufacturerID = u.ManufacturerID,
                             ManufacturerNumber = u.ManufacturerNumber,
                             ManufacturerName = u.ManufacturerName,
                             SupplierID = u.SupplierID,
                             SupplierPartNo = u.SupplierPartNo,
                             SupplierName = u.SupplierName,
                             UPC = u.UPC,
                             UNSPSC = u.UNSPSC,
                             Description = u.Description,
                             LongDescription = u.LongDescription,
                             CategoryID = u.CategoryID,
                             GLAccountID = u.GLAccountID,
                             UOMID = u.UOMID,
                             PricePerTerm = u.PricePerTerm,
                             DefaultReorderQuantity = u.DefaultReorderQuantity,
                             DefaultPullQuantity = u.DefaultPullQuantity,
                             Cost = u.Cost,
                             Markup = u.Markup,
                             SellPrice = u.SellPrice,
                             ExtendedCost = u.ExtendedCost,
                             LeadTimeInDays = u.LeadTimeInDays,
                             Trend = u.Trend,
                             Taxable = u.Taxable,
                             Consignment = u.Consignment,
                             StagedQuantity = u.StagedQuantity,
                             InTransitquantity = u.InTransitquantity,
                             OnOrderQuantity = u.OnOrderQuantity,
                             OnTransferQuantity = u.OnTransferQuantity,
                             SuggestedOrderQuantity = u.SuggestedOrderQuantity,
                             SuggestedTransferQuantity = u.SuggestedTransferQuantity,
                             RequisitionedQuantity = u.RequisitionedQuantity,
                             AverageUsage = u.AverageUsage,
                             Turns = u.Turns,
                             OnHandQuantity = u.OnHandQuantity,
                             CriticalQuantity = u.CriticalQuantity,
                             MinimumQuantity = u.MinimumQuantity,
                             MaximumQuantity = u.MaximumQuantity,
                             WeightPerPiece = u.WeightPerPiece,
                             ItemUniqueNumber = u.ItemUniqueNumber,
                             //TransferOrPurchase = u.TransferOrPurchase,
                             IsPurchase = u.IsPurchase,
                             IsTransfer = u.IsTransfer,
                             DefaultLocation = u.DefaultLocation,
                             DefaultLocationName = u.DefaultLocationName,
                             InventoryClassification = u.InventoryClassification,
                             SerialNumberTracking = u.SerialNumberTracking,
                             LotNumberTracking = u.LotNumberTracking,
                             DateCodeTracking = u.DateCodeTracking,
                             ItemType = u.ItemType,
                             ImagePath = u.ImagePath,
                             UDF1 = u.UDF1,
                             UDF2 = u.UDF2,
                             UDF3 = u.UDF3,
                             UDF4 = u.UDF4,
                             UDF5 = u.UDF5,
                             UDF6 = u.UDF6,
                             UDF7 = u.UDF7,
                             UDF8 = u.UDF8,
                             UDF9 = u.UDF9,
                             UDF10 = u.UDF10,
                             ItemUDF1 = u.UDF1,
                             ItemUDF2 = u.UDF2,
                             ItemUDF3 = u.UDF3,
                             ItemUDF4 = u.UDF4,
                             ItemUDF5 = u.UDF5,
                             ItemUDF6 = u.UDF6,
                             ItemUDF7 = u.UDF7,
                             ItemUDF8 = u.UDF8,
                             ItemUDF9 = u.UDF9,
                             ItemUDF10 = u.UDF10,
                             Created = u.Created,
                             Updated = u.Updated,
                             CreatedBy = u.CreatedBy,
                             LastUpdatedBy = u.LastUpdatedBy,
                             IsDeleted = u.IsDeleted,
                             IsArchived = u.IsArchived,
                             CompanyID = u.CompanyID,
                             Room = u.Room,
                             UpdatedByName = u.UpdatedByName,
                             CreatedByName = u.CreatedByName,
                             RoomName = u.RoomName,
                             IsLotSerialExpiryCost = u.IsLotSerialExpiryCost,
                             GUID = u.GUID,
                             CategoryName = u.CategoryName,
                             Unit = u.Unit,
                             GLAccount = u.GLAccount,
                             CreatedDate = FnCommon.ConvertDateByTimeZone(u.Created, true),// CommonUtility.ConvertDateByTimeZone(u.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             UpdatedDate = FnCommon.ConvertDateByTimeZone(u.Updated, true),//CommonUtility.ConvertDateByTimeZone(u.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             OnOrderInTransitQuantity = u.OnOrderInTransitQuantity,
                             ImageType=u.ImageType,
                             ItemImageExternalURL=u.ItemImageExternalURL,
                             CostUOMID = u.CostUOMID,
                             CostUOMName = u.CostUOMName
                         };

            */
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB//result
            },
                        JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// LoadItemMasterModel
        /// </summary>
        /// <param name="ParentId"></param>
        /// <param name="ParentGuid"></param>
        /// <returns></returns>
        public ActionResult LoadItemMasterModel(string ParentId, string ParentGuid, int QuicklistType = 0)
        {
            //ItemModelPerameter obj = new ItemModelPerameter()
            //{
            //    AjaxURLAddItemToSession = "~/QuickList/AddItemToSession/",
            //    PerentID = ParentId,
            //    ModelHeader = eTurns.DTO.ResQuickList.ModelHeader,
            //    AjaxURLAddMultipleItemToSession = "~/QuickList/AddItemToSessionMultiple/",
            //    AjaxURLToFillItemGrid = "~/QuickList/GetItemsModelMethod/"
            //};


            Session["QuicklistType"] = QuicklistType;

            ItemModelPerameter obj = new ItemModelPerameter()
            {
                AjaxURLAddItemToSession = "~/QuickList/AddItemToDetailTable/",
                PerentID = ParentId,
                PerentGUID = ParentGuid,
                ModelHeader = eTurns.DTO.ResQuickList.ModelHeader,
                AjaxURLAddMultipleItemToSession = "~/QuickList/AddItemToDetailTable/",
                AjaxURLToFillItemGrid = "~/QuickList/GetItemsModelMethod/",
                CallingFromPageName = "QL",
                SelectedQuickListType = (QuickListType)QuicklistType
            };

            return PartialView("ItemMasterModel", obj);
        }

        /// <summary>
        /// LoadQuickListItems
        /// </summary>
        /// <param name="QuickListGUID"></param>
        /// <returns></returns>
        public ActionResult LoadQuickListItems(string QuickListGUID, Int32 QuicklistType)
        {
            Session["QuicklistType"] = QuicklistType;
            ViewBag.SelectedQuicklistType = QuicklistType;

            QuickListMasterDTO obj = new QuickListDAL(SessionHelper.EnterPriseDBName).GetRecord(QuickListGUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
            if (obj == null)
            {
                obj = new QuickListDAL(SessionHelper.EnterPriseDBName).GetRecord(QuickListGUID, SessionHelper.RoomID, SessionHelper.CompanyID, true, true);
                if (obj == null)
                {
                    obj = new QuickListDAL(SessionHelper.EnterPriseDBName).GetRecord(QuickListGUID, SessionHelper.RoomID, SessionHelper.CompanyID, true, false);

                    if (obj == null)
                    {
                        obj = new QuickListDAL(SessionHelper.EnterPriseDBName).GetRecord(QuickListGUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, true);
                    }
                }
            }

            return PartialView("QuickListItems", obj);
        }

        /// <summary>
        /// LoadQLLineItemsHistory
        /// </summary>
        /// <param name="historyID"></param>
        /// <returns></returns>
        public ActionResult LoadQLLineItemsHistory(string historyID)
        {
            //QuickListMasterDTO obj = new QuickListAPIController().GetHistoryRecord(Int64.Parse(Convert.ToString(historyID)));
            QuickListMasterDTO obj = new QuickListDAL(SessionHelper.EnterPriseDBName).GetHistoryRecord(Int64.Parse(Convert.ToString(historyID)));
            return PartialView("QuickListItemsHistory", obj);
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult QuickListHistoryView(Int64 ID)
        {
            //QuickListAPIController obj = new QuickListAPIController();
            QuickListDAL obj = new QuickListDAL(SessionHelper.EnterPriseDBName);
            QuickListMasterDTO objDTO = obj.GetHistoryRecord(ID);
            if (objDTO != null)
            {
                objDTO.ReceivedOnDate = CommonUtility.ConvertDateByTimeZone(objDTO.ReceivedOn, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                objDTO.ReceivedOnDateWeb = CommonUtility.ConvertDateByTimeZone(objDTO.ReceivedOnWeb, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
            }

            ViewBag.QuickListType = GetQuickListType();
            ViewBag.UDFs = GetUDFDataPageWise("QuickListMaster");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;
            objDTO.RoomName = SessionHelper.RoomName;
            return PartialView("_CreateQuickListHistory", objDTO);
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
            int TotalItems = 0;
            int SuccessItems = 0;
            JavaScriptSerializer s = new JavaScriptSerializer();
            QuickListDetailDTO[] QLDetails = s.Deserialize<QuickListDetailDTO[]>(para);

            //------------------------------------------------------------------------------
            //
            UDFDAL objUDFApiController = new UDFDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<UDFDTO> DataFromDB = objUDFApiController.GetRequiredUDFsByUDFTableNamePlain("QuickListItems", SessionHelper.RoomID, SessionHelper.CompanyID);
            string udfRequier = string.Empty;
            foreach (QuickListDetailDTO item in QLDetails)
            {
                foreach (var i in DataFromDB)
                {
                    
                        if (i.UDFColumnName == "UDF1" && string.IsNullOrWhiteSpace(item.UDF1))
                        {
                            string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                            string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                            if (!string.IsNullOrEmpty(val))
                                i.UDFDisplayColumnName = val;
                            else
                                i.UDFDisplayColumnName = i.UDFColumnName;
                            udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                        }
                        else if (i.UDFColumnName == "UDF2" && string.IsNullOrWhiteSpace(item.UDF2))
                        {
                            string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                            string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                            if (!string.IsNullOrEmpty(val))
                                i.UDFDisplayColumnName = val;
                            else
                                i.UDFDisplayColumnName = i.UDFColumnName;
                            udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                        }
                        else if (i.UDFColumnName == "UDF3"  && string.IsNullOrWhiteSpace(item.UDF3))
                        {
                            string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                            string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                            if (!string.IsNullOrEmpty(val))
                                i.UDFDisplayColumnName = val;
                            else
                                i.UDFDisplayColumnName = i.UDFColumnName;
                            udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                        }
                        else if (i.UDFColumnName == "UDF4" && string.IsNullOrWhiteSpace(item.UDF4))
                        {
                            string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                            string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                            if (!string.IsNullOrEmpty(val))
                                i.UDFDisplayColumnName = val;
                            else
                                i.UDFDisplayColumnName = i.UDFColumnName;
                            udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                        }
                        else if (i.UDFColumnName == "UDF5" && string.IsNullOrWhiteSpace(item.UDF5))
                        {
                            string UDFTableResourceFileName = eTurnsWeb.Models.UDFDictionaryTables.GetUDFResourceFromKey(i.UDFTableName);
                            string val = ResourceUtils.GetResource(UDFTableResourceFileName, i.UDFColumnName, true);
                            if (!string.IsNullOrEmpty(val))
                                i.UDFDisplayColumnName = val;
                            else
                                i.UDFDisplayColumnName = i.UDFColumnName;
                            udfRequier = string.Format(ResMessage.MsgRequired, i.UDFDisplayColumnName);
                        }

                        if (!string.IsNullOrEmpty(udfRequier))
                            break;
                    
                }
            }

            if (!string.IsNullOrEmpty(udfRequier))
            {
                return Json(new { Message = udfRequier, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }

            //------------------------------------------------------------------------------
            //
            //QuickListAPIController objApi = new QuickListAPIController();
            QuickListDAL objApi = new QuickListDAL(SessionHelper.EnterPriseDBName);
            if (QLDetails != null && QLDetails.Count() > 0)
            {
                TotalItems = QLDetails.Count();
                foreach (QuickListDetailDTO item in QLDetails)
                {
                    item.Room = SessionHelper.RoomID;
                    item.CreatedBy = SessionHelper.UserID;
                    item.LastUpdatedBy = SessionHelper.UserID;
                    if (!string.IsNullOrWhiteSpace(item.BinName))
                    {
                        BinMasterDAL objItemLocationDetailsDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                        BinMasterDTO objBin = objItemLocationDetailsDAL.GetItemBinPlain(item.ItemGUID ?? Guid.Empty, item.BinName, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, false);
                        item.BinID = objBin.ID;
                    }
                    else
                    {
                        item.BinID = null;
                    }
                    item.CompanyID = SessionHelper.CompanyID;
                    if (!objApi.isItemBinExistinQL(item.QuickListGUID, item.ItemGUID ?? Guid.Empty, item.BinID))
                    {
                        objApi.QuickListItemInsert(item);
                        SuccessItems++;
                    }

                    item.CreatedDate = CommonUtility.ConvertDateByTimeZone(item.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                    item.UpdatedDate = CommonUtility.ConvertDateByTimeZone(item.LastUpdated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                }

                if (TotalItems == SuccessItems)
                {
                    message = ResCommon.AllItemsAddedSuccessfully;
                    status = "success";
                }
                else
                {
                    message = string.Format(ResCommon.ItemsAddedSuccessfully, SuccessItems.ToString());
                    status = "success";
                }
            }
            else
            {
                message = ResCommon.NoItemFound;
                status = "fail";
            }

            //message = "Item added";
            //status = "success";
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult BlankSession()
        {
            Session["IsInsert"] = "";
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);

        }

        #region NarrowSearch

        public JsonResult GetNarrowSearchData(bool IsDeleted, bool IsArchived)
        {
            CommonDAL objCommonCtrl = new CommonDAL(SessionHelper.EnterPriseDBName);
            var tmpsupplierIds = new List<long>();
            NarrowSearchData objNarrowSearchData = objCommonCtrl.GetNarrowSearchDataFromCache("QuickListMaster", SessionHelper.CompanyID, SessionHelper.RoomID, IsArchived, IsDeleted, "", tmpsupplierIds);
            return Json(new { Success = true, Message = ResCommon.MsgDataSuccessfullyGet, Data = objNarrowSearchData }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get QuickList NarrwSearchHTML
        /// </summary>
        /// <returns></returns>
        public ActionResult GetQLNarrwSearchHTML()
        {
            return PartialView("_QuickListNarrowSearch");

        }

        #endregion
    }

    public class QuickListJQueryDataTableParamModel : JQueryDataTableParamModel
    {
        public Int64 QuickListID { get; set; }
        public Int64 SupplierID { get; set; }
        //public List<QuickListDetailDTO> lstQuickList { get; set; }
    }

    public class ItemWithQuentity
    {
        public string ID { get; set; }
        public string GUID { get; set; }
        public string Qty { get; set; }
    }

    public class QuickListItemWithQuantity
    {
        IEnumerable<ItemWithQuentity> ItemsWithQty { get; set; }
        public Int64 QuickListID { get; set; }
    }
}
