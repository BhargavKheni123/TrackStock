using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsMaster.DAL;
using eTurnsWeb.BAL;
using eTurnsWeb.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using eTurns.DTO.Helper;
using System.Net.Http.Headers;
using System.Net.Http;
using NPOI.SS.Formula.Functions;

namespace eTurnsWeb.Controllers
{
    [AuthorizeHelper]
    public partial class MasterController : eTurnsControllerBase
    {
        UDFController objUDFDAL = new UDFController();
        string CtrlName = Convert.ToString(ConfigurationManager.AppSettings["CtrlName"]);
        string ActName = Convert.ToString(ConfigurationManager.AppSettings["ActName"]);
        //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));

        #region "Bin Master"

        /// <summary>
        ///GET ALL: /Master/ 
        /// </summary>
        /// <returns></returns>
        public ActionResult BinList()
        {
            Session["AllBins"] = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllBins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, string.Empty, null);
            return View();
        }

        public PartialViewResult _CreateBin()
        {
            return PartialView();
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult BinCreate(Guid? ItemGUID, string callFor = "", bool IsStage = false)
        {
            BinMasterDTO objDTO = new BinMasterDTO()
            {
                //BinNumber = "#B" + NewNumber,
                //BinNumber = NewNumber,
                Created = DateTimeUtility.DateTimeNow,
                LastUpdated = DateTimeUtility.DateTimeNow,
                CreatedBy = SessionHelper.UserID,
                CreatedByName = SessionHelper.UserName,
                LastUpdatedBy = SessionHelper.UserID,
                Room = SessionHelper.RoomID,
                CompanyID = SessionHelper.CompanyID,
                RoomName = SessionHelper.RoomName,
                UpdatedByName = SessionHelper.UserName,
                IsOnlyFromItemUI = true,
            };
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("BinMaster");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }

            if (ItemGUID.GetValueOrDefault(Guid.Empty) != Guid.Empty && callFor == "INC")
            {
                ViewBag.ItemGUID = ItemGUID;
                objDTO.callFor = callFor;
                objDTO.IsStagingLocation = IsStage;
            }
            else
            {
                ViewBag.ItemGUID = null;
                objDTO.callFor = "";
            }

            return PartialView("_CreateBin", objDTO);
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
        public JsonResult BinSave(BinMasterDTO objDTO)
        {
            objDTO.BinNumber = objDTO.BinNumber.Trim();

            bool va = ModelState.IsValid;
            if (!ModelState.IsValid)
                return Json(new { Message = ResMessage.InvalidModel, Status = "Fa" }, JsonRequestBehavior.AllowGet);

            string message = "";
            string status = "";
            Int64 NewBinID = 0;
            BinMasterDAL obj = new BinMasterDAL(SessionHelper.EnterPriseDBName);

            if (string.IsNullOrEmpty(objDTO.BinNumber))
            {
                message = string.Format(ResMessage.Required, ResBin.BinNumber);
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.Room = SessionHelper.RoomID;

            if (objDTO.ID == 0)
            {
                string strOK = "";
                if (objDTO.callFor == "INC")
                {
                    BinMasterDTO objBinMasterDTO = obj.getInventoryBinByItemAndBinNumber(objDTO.BinNumber, objDTO.ItemGUID ?? Guid.Empty, SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (objBinMasterDTO != null && objBinMasterDTO.ID > 0)
                    {
                        strOK = "duplicate";
                    }
                }
                else
                {
                    strOK = obj.BinDuplicateCheck(objDTO.ID, objDTO.BinNumber, SessionHelper.RoomID, SessionHelper.CompanyID, objDTO.IsStagingLocation);
                }

                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResBin.BinNumber, objDTO.BinNumber);  //"BinNumber \"" + objDTO.BinNumber + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    if (objDTO.callFor == "INC")
                    {
                        objDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemBinPlain(objDTO.ItemGUID ?? Guid.Empty, objDTO.BinNumber, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, objDTO.IsStagingLocation, objDTO.EditedFrom, false, null);
                    }
                    else
                    {
                        objDTO = obj.InsertBin(objDTO);
                    }
                    if (objDTO.ID > 0)
                    {
                        message = ResMessage.SaveMessage; //ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                        NewBinID = objDTO.ID;
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }
            else
            {
                string strOK = obj.BinDuplicateCheck(objDTO.ID, objDTO.BinNumber, SessionHelper.RoomID, SessionHelper.CompanyID, objDTO.IsStagingLocation);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResBin.BinNumber, objDTO.BinNumber);  //"BinNumber \"" + objDTO.BinNumber + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    objDTO.IsOnlyFromItemUI = true;
                    objDTO.EditedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    bool ReturnVal = obj.Edit(objDTO);

                    if (ReturnVal)
                    {
                        message = ResMessage.SaveMessage;  //ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); //string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }

            return Json(new { Message = message, Status = status, NewBinIDPopup = NewBinID }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>
        public ActionResult BinEdit(Int64 ID)
        {
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            if (IsDeleted || IsArchived)
            {
                ViewBag.ViewOnly = true;
            }

            //BinMasterController obj = new BinMasterController();
            BinMasterDAL obj = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            BinMasterDTO objDTO = obj.GetBinByID(ID, SessionHelper.RoomID, SessionHelper.CompanyID);
            //BinMasterDTO objDTO = obj.GetItemLocation( SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted,Guid.Empty, ID,null,null).FirstOrDefault();
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("BinMaster");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;
            objDTO.IsOnlyFromItemUI = true;
            return PartialView("_CreateBin", objDTO);
        }

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public JsonResult DeleteBinMasterRecords(string ids)
        {
            try
            {
                //eTurns.DAL.CommonDAL _repository = new eTurns.DAL.CommonDAL(SessionHelper.EnterPriseDBName);
                //return _repository.DeleteRecords(ImportMastersDTO.TableName.BinMaster.ToString(), ids, SessionHelper.RoomID, SessionHelper.CompanyID);

                string response = string.Empty;
                string BinNumbers = "";

                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                BinMasterDAL binMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);

                if (!objCommonDAL.CheckIfAnyBinIsDefaultbySP(ids, out BinNumbers))
                {
                    var binWithQuantityMsg = ResBin.BinHaveQtySoCantDelete;
                    var deletedSuccessMessage = ResCommon.MsgDeletedSuccessfully;
                    response = binMasterDAL.DeleteBinMasterRecords(ids, ImportMastersDTO.TableName.BinMaster.ToString(), false, SessionHelper.UserID, binWithQuantityMsg, deletedSuccessMessage);
                    eTurns.DAL.CacheHelper<IEnumerable<BinMasterDTO>>.InvalidateCache();
                    return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Message = string.Format(ResBin.CannotDeleteDefaultBin, BinNumbers), Status = "DefaultBinDelete" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception)
            {
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
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
        public string UpdateBinMasterData(Int64 id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            //BinMasterController obj = new BinMasterController();
            BinMasterDAL obj = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            obj.UpdateData(id, value, rowId, columnPosition, columnId, columnName);
            return value;
        }

        ///// <summary>
        ///// Duplicate Bin Master Check
        ///// </summary>
        ///// <param name="Name"></param>
        ///// <param name="ActionMode"></param>
        ///// <param name="ID"></param>
        ///// <returns></returns>
        //public string DuplicateBinMasterCheck(string Name, string ActionMode, int ID)
        //{
        //    BinMasterController obj = new BinMasterController();
        //    return obj.DuplicateCheck(Name, ActionMode, ID);
        //}

        #region Data Provider

        public JsonResult GetAllLocationOfRoom(string BinNumber, bool? IsStagingLocation)
        {
            BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            List<BinMasterDTO> lstBinMaster = objBinMasterDAL.GetAllBins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, BinNumber, IsStagingLocation ?? false);
            lstBinMaster = lstBinMaster.Where(b => b.BinNumber != null).ToList();
            lstBinMaster = lstBinMaster.OrderBy(b => b.BinNumber.Trim()).ToList();
            return Json(lstBinMaster, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetAllLocationOfRoomByRoomAndCompany(string BinNumber, int RoomID, int CompanyID, bool? IsStagingLocation)
        {
            BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);

            List<BinMasterDTO> lstBinMaster = objBinMasterDAL.GetAllBins(RoomID, CompanyID, false, false, BinNumber, IsStagingLocation ?? false);
            lstBinMaster = lstBinMaster.Where(b => b.BinNumber != null).ToList();
            lstBinMaster = lstBinMaster.OrderBy(b => b.BinNumber.Trim()).ToList();
            return Json(lstBinMaster, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetAllLocationOfRoomForTool(string BinNumber)
        {
            LocationMasterDAL objBinMasterDAL = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
            List<LocationMasterDTO> lstBinMaster = objBinMasterDAL.GetLocationListSearch(SessionHelper.RoomID, SessionHelper.CompanyID, BinNumber).ToList();
            lstBinMaster = lstBinMaster.Where(b => b.Location != null).ToList();
            lstBinMaster = lstBinMaster.OrderBy(b => b.Location.Trim()).ToList();
            return Json(lstBinMaster, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult GetBinList(JQueryDataTableParamModel param)
        {
            BinMasterDTO entity = new BinMasterDTO();
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
            //    sortColumnName = "BinNumber Asc";

            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "BinNumber Asc";
            }
            else
                sortColumnName = "BinNumber Asc";

            //if (sortDirection == "asc")
            //    sortColumnName = sortColumnName + " asc";
            //else
            //    sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;

            //BinMasterController controller = new BinMasterController();
            BinMasterDAL controller = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            //IEnumerable<BinMasterDTO> DataFromDB = controller.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted);
            List<BinMasterDTO> DataFromDB = controller.GetPagedBinRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, RoomDateFormat, CurrentTimeZone);

            //var result = from c in DataFromDB
            //             select new BinMasterDTO
            //             {
            //                 ID = c.ID,
            //                 BinNumber = c.BinNumber,
            //                 RoomName = c.RoomName,
            //                 Created = c.Created,
            //                 LastUpdated = c.LastUpdated,
            //                 UpdatedByName = c.UpdatedByName,
            //                 CreatedByName = c.CreatedByName,
            //                 IsDeleted = c.IsDeleted,
            //                 IsArchived = c.IsArchived,
            //                 IsStagingLocation = c.IsStagingLocation,
            //                 ItemNumber = c.ItemNumber,
            //                 MaximumQuantity = c.MaximumQuantity,
            //                 MinimumQuantity = c.MinimumQuantity,
            //                 CriticalQuantity = c.CriticalQuantity,
            //                 UDF1 = c.UDF1,
            //                 UDF2 = c.UDF2,
            //                 UDF3 = c.UDF3,
            //                 UDF4 = c.UDF4,
            //                 UDF5 = c.UDF5,
            //                 eVMISensorID = c.eVMISensorID,
            //                 eVMISensorPort = c.eVMISensorPort
            //             };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetDefaultLocation(int maxRows, string name_startsWith)
        //{
        //    BinMasterDAL obj = new BinMasterDAL(SessionHelper.EnterPriseDBName);
        //    List<BinMasterDTO> lstUnit; // = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(t => t.IsStagingLocation != true && t.BinNumber.ToLower().Contains(name_startsWith.ToLower().Trim())).ToList();
        //    if (name_startsWith.Trim().Count() > 0)
        //    {
        //        lstUnit = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(t => t.IsStagingLocation != true && t.BinNumber.ToLower().Contains(name_startsWith.ToLower().Trim())).ToList();
        //        //lstUnit = obj.GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false,Guid.Empty,0,null,false).Where(t => t.BinNumber.ToLower().Contains(name_startsWith.ToLower().Trim())).ToList();
        //    }
        //    else
        //    {
        //        lstUnit = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(t => t.IsStagingLocation != true).ToList();
        //        //lstUnit = obj.GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Guid.Empty, 0, null, false).ToList(); //.Where(t => t.IsStagingLocation != true).ToList();
        //    }

        //    if (lstUnit.Count == 0)
        //    {
        //        return Json("", JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(lstUnit, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetDefaultLocationByItemWise(int maxRows, string name_startsWith, string ItemGUID)
        //{
        //    BinMasterDAL obj = new BinMasterDAL(SessionHelper.EnterPriseDBName);
        //    List<BinMasterDTO> lstUnit; // = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(t => t.IsStagingLocation != true && t.BinNumber.ToLower().Contains(name_startsWith.ToLower().Trim())).ToList();
        //    if (name_startsWith.Trim().Count() > 0)
        //    {
        //        lstUnit = obj.GetAllRecordsByItemLocationLevelQuanity(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGUID).Where(t => t.IsStagingLocation != true && t.BinNumber.ToLower().Contains(name_startsWith.ToLower().Trim())).ToList();
        //        //lstUnit = obj.GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Guid.Parse(ItemGUID),0,null,false).Where(t => t.BinNumber.ToLower().Contains(name_startsWith.ToLower().Trim())).ToList();//t.IsStagingLocation != true && 
        //    }
        //    else
        //    {
        //        lstUnit = obj.GetAllRecordsByItemLocationLevelQuanity(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGUID).Where(t => t.IsStagingLocation != true).ToList();
        //        //lstUnit = obj.GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Guid.Parse(ItemGUID), 0, null, false).ToList();//.Where(t => t.IsStagingLocation != true)
        //    }

        //    if (lstUnit.Count == 0)
        //    {
        //        return Json("", JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        lstUnit = lstUnit.OrderBy(x => x.BinNumber).ToList();
        //    }

        //    return Json(lstUnit, JsonRequestBehavior.AllowGet);
        //}



        #endregion

        #endregion

        #region "Category Master"
        //   [AuthorizeHelper]
        public ActionResult CategoryList()
        {
            return View();
        }

        // [AuthorizeHelper]
        public PartialViewResult _CreateCategory()
        {
            return PartialView();
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>      
        public ActionResult CategoryCreate(bool isforbom)
        {
            CategoryMasterDTO objDTO = new CategoryMasterDTO();
            objDTO.ID = 0;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.CreatedByName = SessionHelper.UserName;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.RoomName = SessionHelper.RoomName;
            objDTO.GUID = Guid.NewGuid();
            objDTO.isForBOM = isforbom;
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("CategoryMaster");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }

            return PartialView("_CategoryCreate", objDTO);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult CategorySave(CategoryMasterDTO objDTO)
        {
            string message = "";
            string status = "";
            //CategoryController obj = new CategoryController();
            CategoryMasterDAL obj = new CategoryMasterDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            objDTO.Room = SessionHelper.RoomID;
            objDTO.RoomName = SessionHelper.RoomName;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;
            if (string.IsNullOrEmpty(objDTO.Category))
            {
                message = string.Format(ResMessage.Required, ResCategoryMaster.Category);// "Category name is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(objDTO.CategoryColor))
            {
                message = string.Format(ResMessage.Required, ResCategoryMaster.CategoryColor);// "CategoryColor name is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.Room = SessionHelper.RoomID;
            Int64 _NewIDForPopUp = 0;
            if (objDTO.ID == 0)
            {
                objDTO.CreatedBy = SessionHelper.UserID;
                long RoomId = SessionHelper.RoomID;
                if (objDTO.isForBOM)
                {
                    RoomId = 0;
                }
                string strOK = objCDAL.DuplicateCheck(objDTO.Category, "add", objDTO.ID, "CategoryMaster", "Category", RoomId, SessionHelper.CompanyID);
                //if (strOK == "duplicate")
                //{
                //    CategoryMasterDTO objCategoryMasterDTO = new CategoryMasterDTO();
                //    objCategoryMasterDTO = obj.GetAllRecords(RoomId, SessionHelper.CompanyID, false, false, false).Where(t => t.Category == objDTO.Category && t.isForBOM == true).ToList().FirstOrDefault();

                //    if (objCategoryMasterDTO != null && objDTO.isForBOM == false)
                //    {
                //        objCategoryMasterDTO.isForBOM = false;
                //        objCategoryMasterDTO.CategoryColor = objDTO.CategoryColor;
                //        objCategoryMasterDTO.UDF1 = objDTO.UDF1;
                //        objCategoryMasterDTO.UDF2 = objDTO.UDF2;
                //        objCategoryMasterDTO.UDF3 = objDTO.UDF3;
                //        objCategoryMasterDTO.UDF4 = objDTO.UDF4;
                //        objCategoryMasterDTO.UDF5 = objDTO.UDF5;
                //        objCategoryMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                //        objCategoryMasterDTO.EditedFrom = "Web";

                //        if (obj.Edit(objCategoryMasterDTO))
                //        {
                //            message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                //            status = "ok";
                //            _NewIDForPopUp = objCategoryMasterDTO.ID;
                //            return Json(new { Message = message, Status = status, NewIDForPopUp = _NewIDForPopUp }, JsonRequestBehavior.AllowGet);
                //        }
                //    }

                //}
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResCategoryMaster.Category, objDTO.Category);  // "Category \"" + objDTO.Category + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    string strOK1 = objCDAL.DuplicateCheck(objDTO.CategoryColor, "add", objDTO.ID, "CategoryMaster", "CategoryColor", SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (strOK1 == "duplicate")
                    {
                        message = string.Format(ResMessage.DuplicateMessage, ResCategoryMaster.CategoryColor, objDTO.CategoryColor);  // "Category \"" + objDTO.Category + "\" already exist! Try with Another!";
                        status = "duplicate";
                    }
                    else
                    {
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";
                        long ReturnVal = obj.Insert(objDTO);
                        if (ReturnVal > 0)
                        {
                            message = ResMessage.SaveMessage; //ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                            status = "ok";
                            _NewIDForPopUp = ReturnVal;
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); //string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                            status = "fail";
                        }
                    }
                }
            }
            else
            {
                objDTO.LastUpdatedBy = SessionHelper.UserID;
                long RoomId = SessionHelper.RoomID;
                if (objDTO.isForBOM)
                {
                    RoomId = 0;
                }
                string strOK = objCDAL.DuplicateCheck(objDTO.Category, "edit", objDTO.ID, "CategoryMaster", "Category", RoomId, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResCategoryMaster.Category, objDTO.Category);  //"Category \"" + objDTO.Category + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    string strOK1 = objCDAL.DuplicateCheck(objDTO.CategoryColor, "edit", objDTO.ID, "CategoryMaster", "CategoryColor", SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (strOK1 == "duplicate")
                    {
                        message = string.Format(ResMessage.DuplicateMessage, ResCategoryMaster.CategoryColor, objDTO.CategoryColor);  // "Category \"" + objDTO.Category + "\" already exist! Try with Another!";
                        status = "duplicate";
                    }
                    else
                    {
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.EditedFrom = "Web";
                        bool ReturnVal = obj.Edit(objDTO);
                        if (objDTO.isForBOM == true)
                        {
                            BOMItemMasterDAL objBOMItemMasterDAL = new BOMItemMasterDAL(SessionHelper.EnterPriseDBName);
                            objBOMItemMasterDAL.UpdateBOMMasterReference(objDTO.ID, "CategoryMaster", SessionHelper.UserID);
                        }

                        if (ReturnVal)
                        {
                            message = ResMessage.SaveMessage; //ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                            status = "ok";
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                            status = "fail";
                        }
                    }
                }
            }

            return Json(new { Message = message, Status = status, NewIDForPopUp = _NewIDForPopUp }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>      
        public ActionResult CategoryEdit(Int64 ID)
        {
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            if (IsDeleted || IsArchived)
            {
                ViewBag.ViewOnly = true;
            }

            //CategoryController obj = new CategoryController();
            CategoryMasterDAL obj = new CategoryMasterDAL(SessionHelper.EnterPriseDBName);
            CategoryMasterDTO objDTO = obj.GetCategoryByCatID(ID);
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("CategoryMaster");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;
            if (objDTO != null)
            {
                objDTO.CreatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                objDTO.UpdatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
            }
            return PartialView("_CategoryCreate", objDTO);
        }

        #region Ajax Data Provider

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult CategoryListAjax(JQueryDataTableParamModel param)
        {
            //CategoryController obj = new CategoryController();
            CategoryMasterDAL obj = new CategoryMasterDAL(SessionHelper.EnterPriseDBName);
            bool IsForBom = false;
            bool.TryParse(Convert.ToString(Request["IsForBom"]), out IsForBom);
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

            // set the default column sorting here, if first time then required to set 
            //if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID";



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
                sortColumnName = "ID desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;


            IEnumerable<CategoryMasterDTO> DataFromDB;

            //DataFromDB = obj.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, IsForBom);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            DataFromDB = obj.GetPagedCategoryMaster(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, SessionHelper.RoomDateFormat, IsForBom, CurrentTimeZone);

            TimeZone localZone = TimeZone.CurrentTimeZone;

            var microsoftDateFormatSettings = new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat, DateTimeZoneHandling = DateTimeZoneHandling.Local };
            var result = from c in DataFromDB
                         select new CategoryMasterDTO
                         {
                             ID = c.ID,
                             Category = c.Category,
                             RoomName = c.RoomName,
                             Created = c.Created,
                             Updated = c.Updated,
                             //Created = DateTime.SpecifyKind(c.Created ?? DateTime.MinValue, DateTimeKind.Utc),
                             //Created = DateTime.SpecifyKind(c.Created ?? DateTime.MinValue, DateTimeKind.Utc),
                             //Updated = DateTime.SpecifyKind(c.Updated ?? DateTime.MinValue, DateTimeKind.Utc),
                             UpdatedByName = c.UpdatedByName,
                             CreatedByName = c.CreatedByName,
                             IsDeleted = c.IsDeleted,
                             IsArchived = c.IsArchived,
                             UDF1 = c.UDF1,
                             UDF2 = c.UDF2,
                             UDF3 = c.UDF3,
                             UDF4 = c.UDF4,
                             UDF5 = c.UDF5,
                             ReceivedOn = c.ReceivedOn,
                             ReceivedOnWeb = c.ReceivedOnWeb,
                             AddedFrom = c.AddedFrom,
                             EditedFrom = c.EditedFrom,
                             //CreatedDate = (c.Updated ?? DateTime.MinValue).ToString(),
                             CreatedDate = FnCommon.ConvertDateByTimeZone(c.Created, true),//CommonUtility.ConvertDateByTimeZone(c.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             ServerOffset = localZone.GetUtcOffset(c.Created ?? DateTime.MinValue).TotalMinutes,
                             UpdatedDate = FnCommon.ConvertDateByTimeZone(c.Updated, true),//CommonUtility.ConvertDateByTimeZone(c.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             ReceivedOnDate = FnCommon.ConvertDateByTimeZone(c.ReceivedOn, true),//CommonUtility.ConvertDateByTimeZone(c.ReceivedOn, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                             //ReceivedOnDateWeb = CommonUtility.ConvertDateByTimeZone(c.ReceivedOnWeb, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);


        }

        //public string UpdateCategoryData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        //{
        //    //CategoryController obj = new CategoryController();
        //    CategoryMasterDAL obj = new CategoryMasterDAL(SessionHelper.EnterPriseDBName);
        //    obj.UpdateData(id, value, rowId, columnPosition, columnId, columnName);
        //    return value;
        //}

        //public string DuplicateCategoryCheck(string CategoryName, string ActionMode, int ID)
        //{
        //    CategoryController obj = new CategoryController();
        //    return obj.DuplicateCheck(CategoryName, ActionMode, ID);
        //}

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public JsonResult DeleteCategoryRecords(string ids)
        {
            try
            {
                //eTurns.DAL.CommonDAL _repository = new eTurns.DAL.CommonDAL(SessionHelper.EnterPriseDBName);
                //return _repository.DeleteRecords(ImportMastersDTO.TableName.CategoryMaster.ToString(), ids, SessionHelper.RoomID, SessionHelper.CompanyID);
                string response = string.Empty;
                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                response = objCommonDAL.DeleteModulewise(ids, ImportMastersDTO.TableName.CategoryMaster.ToString(), false, SessionHelper.UserID, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);

                eTurns.DAL.CacheHelper<IEnumerable<CategoryMasterDTO>>.InvalidateCache();
                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetCategory(int maxRows, string name_startsWith)
        {
            CategoryMasterDAL obj = new CategoryMasterDAL(SessionHelper.EnterPriseDBName);
            //List<CategoryMasterDTO> lstUnit = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false).Where(t => t.Category.ToLower().Contains(name_startsWith.ToLower().Trim())).Take(maxRows).ToList();
            List<CategoryMasterDTO> lstUnit = obj.GetCategoryListSearch(SessionHelper.RoomID, SessionHelper.CompanyID, (name_startsWith ?? string.Empty).ToLower().Trim()).Take(maxRows).ToList();
            if (lstUnit.Count == 0)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            lstUnit = lstUnit.OrderBy(c => c.Category).ToList();
            return Json(lstUnit, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #endregion

        #region "FreightType Master"

        /// <summary>
        ///GET ALL: /Master/ 
        /// </summary>
        /// <returns></returns>
        //public ActionResult FreightTypeList()
        //{
        //    return View();
        //}

        //public PartialViewResult _CreateFreightType()
        //{
        //    return PartialView();
        //}

        ///// <summary>
        /////  GET: /Master/ for Create
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult FreightTypeCreate()
        //{
        //    FreightTypeMasterDTO objDTO = new FreightTypeMasterDTO()
        //    {
        //        Created = DateTimeUtility.DateTimeNow,
        //        LastUpdated = DateTimeUtility.DateTimeNow,
        //        CreatedBy = SessionHelper.UserID,
        //        CreatedByName = SessionHelper.UserName,
        //        LastUpdatedBy = SessionHelper.UserID,
        //        Room = SessionHelper.RoomID,
        //        CompanyID = SessionHelper.CompanyID,
        //        RoomName = SessionHelper.RoomName,
        //        UpdatedByName = SessionHelper.UserName,
        //    };
        //    ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("FreightTypeMaster");
        //    foreach (var i in ViewBag.UDFs)
        //    {
        //        string _UDFColumnName = (string)i.UDFColumnName;
        //        ViewData[_UDFColumnName] = i.UDFDefaultValue;
        //    }
        //    return PartialView("_CreateFreightType", objDTO);
        //}

        /// <summary>
        /// JSON Record Save - Enter key Save/Update
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Technician"></param>
        /// <returns></returns>
        //public JsonResult FreightTypeSave(FreightTypeMasterDTO objDTO)
        //{
        //    string message = "";
        //    string status = "";
        //    //FreightTypeMasterController obj = new FreightTypeMasterController();
        //    FreightTypeMasterDAL obj = new FreightTypeMasterDAL(SessionHelper.EnterPriseDBName);
        //    CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

        //    if (string.IsNullOrEmpty(objDTO.FreightType))
        //    {
        //        message = string.Format(ResMessage.Required, ResFreightType.FreightType);// "Freight Type is required.";
        //        status = "fail";
        //        return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        //    }

        //    string FreightType = objDTO.FreightType.Replace("'", "''");
        //    objDTO.LastUpdatedBy = SessionHelper.UserID;
        //    objDTO.UpdatedByName = SessionHelper.UserName;
        //    objDTO.Room = SessionHelper.RoomID;
        //    if (objDTO.ID == 0)
        //    {
        //        objDTO.CreatedBy = SessionHelper.UserID;
        //        string strOK = objCDAL.DuplicateCheck(FreightType, "add", objDTO.ID, "FreightTypeMaster", "FreightType", SessionHelper.RoomID, SessionHelper.CompanyID);
        //        if (strOK != "ok")
        //        {
        //            message = string.Format(ResMessage.DuplicateMessage, ResFreightType.FreightType, objDTO.FreightType);  //"FreightType \"" + objDTO.FreightType + "\" already exist! Try with Another!";
        //            status = "ok";
        //        }
        //        else
        //        {
        //            objDTO.GUID = Guid.NewGuid();
        //            long ReturnVal = obj.Insert(objDTO);
        //            if (ReturnVal > 0)
        //            {
        //                message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
        //                status = "ok";
        //            }
        //            else
        //            {
        //                message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
        //                status = "fail";
        //            }
        //        }
        //    }
        //    else
        //    {
        //        string strOK = objCDAL.DuplicateCheck(FreightType, "edit", objDTO.ID, "FreightTypeMaster", "FreightType", SessionHelper.RoomID, SessionHelper.CompanyID);
        //        if (strOK != "ok")
        //        {
        //            message = string.Format(ResMessage.DuplicateMessage, ResFreightType.FreightType, objDTO.FreightType);  //"FreightType \"" + objDTO.FreightType + "\" already exist! Try with Another!";
        //            status = "fail";
        //        }
        //        else
        //        {
        //            bool ReturnVal = obj.Edit(objDTO);

        //            if (ReturnVal)
        //            {
        //                message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
        //                status = "ok";
        //            }
        //            else
        //            {
        //                message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
        //                status = "fail";
        //            }
        //        }
        //    }

        //    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        //}


        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>
        //public ActionResult FreightTypeEdit(int ID)
        //{
        //    bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
        //    bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
        //    if (IsDeleted || IsArchived)
        //    {
        //        ViewBag.ViewOnly = true;
        //    }

        //    //FreightTypeMasterController obj = new FreightTypeMasterController();
        //    FreightTypeMasterDAL obj = new FreightTypeMasterDAL(SessionHelper.EnterPriseDBName);
        //    FreightTypeMasterDTO objDTO = obj.GetFreightTypeByID(ID);
        //    ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("FreightTypeMaster");
        //    ViewData["UDF1"] = objDTO.UDF1;
        //    ViewData["UDF2"] = objDTO.UDF2;
        //    ViewData["UDF3"] = objDTO.UDF3;
        //    ViewData["UDF4"] = objDTO.UDF4;
        //    ViewData["UDF5"] = objDTO.UDF5;
        //    if (objDTO != null)
        //    {
        //        objDTO.CreatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
        //        objDTO.UpdatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.LastUpdated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
        //    }
        //    return PartialView("_CreateFreightType", objDTO);
        //}

        /// <summary>
        /// HTTP Post Request for Add FreightType Master Reocrd
        /// </summary>
        /// <param name="frm"></param>
        /// <returns></returns>
        //[HttpPost]
        //public ActionResult FreightTypeEdit(FreightTypeMasterDTO objDTO)
        //{
        //    //FreightTypeMasterController bb = new FreightTypeMasterController();
        //    FreightTypeMasterDAL bb = new FreightTypeMasterDAL(SessionHelper.EnterPriseDBName);
        //    bb.Edit(objDTO);
        //    return RedirectToAction("FreightTypeList", "Master");
        //}

        /// <summary>
        /// HTTP Post Request for Add BinMaster Reocrd
        /// </summary>
        /// <param name="frm"></param>
        /// <returns></returns>
        //[HttpPost]
        //public JsonResult FrieghtTypeDelete(string IDs)
        //{
        //    try
        //    {
        //        //eTurns.DAL.CommonDAL _repository = new eTurns.DAL.CommonDAL(SessionHelper.EnterPriseDBName);
        //        //return _repository.DeleteRecords(ImportMastersDTO.TableName.FreightTypeMaster.ToString(), IDs, SessionHelper.RoomID, SessionHelper.CompanyID);

        //        string response = string.Empty;
        //        CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
        //        response = objCommonDAL.DeleteModulewise(IDs, ImportMastersDTO.TableName.FreightTypeMaster.ToString(), false, SessionHelper.UserID);
        //        eTurns.DAL.CacheHelper<IEnumerable<FreightTypeMasterDTO>>.InvalidateCache();
        //        return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);

        //    }
        //    catch (Exception)
        //    {
        //        return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
        //    }

        //}

        //public string DuplicateFreightTypeCheck(string Name, string ActionMode, int ID)
        //{
        //    FreightTypeMasterController obj = new FreightTypeMasterController();
        //    return obj.DuplicateCheck(Name, ActionMode, ID);
        //}

        #region Data Provider

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        //public ActionResult DataProviderForFrieghtTypeGrid(JQueryDataTableParamModel param)
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
        //    if (!string.IsNullOrWhiteSpace(param.sSearch))
        //    {
        //        searchQuery = " And (  FreightType like '%" + param.sSearch + "%'" + @"
        //            OR RoomName like '%" + param.sSearch + "%'" + @" 
        //            OR CreatedByName like '%" + param.sSearch + "%'" + @" 
        //            OR UpdatedByName like '%" + param.sSearch + "%' )";

        //    }
        //    else
        //        param.sSearch = "";

        //    int TotalRecordCount = 0;

        //    //FreightTypeMasterController controller = new FreightTypeMasterController();
        //    FreightTypeMasterDAL controller = new FreightTypeMasterDAL(SessionHelper.EnterPriseDBName);
        //    IEnumerable<FreightTypeMasterDTO> dto = controller.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted);

        //    if (dto != null)
        //    {
        //        dto.ToList().ForEach(t =>
        //        {
        //            t.CreatedDate = CommonUtility.ConvertDateByTimeZone(t.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);

        //            t.UpdatedDate = CommonUtility.ConvertDateByTimeZone(t.LastUpdated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
        //        });
        //    }
        //    return Json(new
        //    {
        //        sEcho = param.sEcho,
        //        iTotalRecords = TotalRecordCount,
        //        iTotalDisplayRecords = TotalRecordCount,
        //        aaData = dto
        //    },
        //         JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// UpdateBinMasterData
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <param name="rowId"></param>
        /// <param name="columnPosition"></param>
        /// <param name="columnId"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        //public string UpdateFrieghtTypeData(Int64 id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        //{
        //    //FreightTypeMasterController obj = new FreightTypeMasterController();
        //    FreightTypeMasterDAL obj = new FreightTypeMasterDAL(SessionHelper.EnterPriseDBName);
        //    int idx = obj.GetFreightTypeByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID).FindIndex(x => x.FreightType == value && x.ID != id);
        //    if (idx >= 0)
        //    {
        //        return "Freight Type already exist!";
        //    }
        //    else
        //    {
        //        obj.UpdateData(id, value, 1, columnName);
        //    }
        //    return value;
        //}

        #endregion

        #endregion

        #region "Manufacturer Master"

        /// <summary>
        /// JSON Record Save - Enter key Save/Update
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Manufacturer"></param>
        /// <returns></returns>
        /// 

        [ValidateAntiForgeryToken]
        public JsonResult ManufacturerSave(ManufacturerMasterDTO objDTO)
        {
            string message = "";
            string status = "";
            //ManufacturerMasterController obj = new ManufacturerMasterController();
            ManufacturerMasterDAL obj = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            objDTO.Room = SessionHelper.RoomID;
            objDTO.RoomName = SessionHelper.RoomName;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            Int64 _NewIDForPopUp = 0;
            if (string.IsNullOrEmpty(objDTO.Manufacturer))
            {
                message = string.Format(ResMessage.Required, ResManufacturer.ManufacturerName);// "Manufacturer name is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.Room = SessionHelper.RoomID;
            if (objDTO.ID == 0)
            {
                objDTO.CreatedBy = SessionHelper.UserID;
                long RoomId = SessionHelper.RoomID;
                if (objDTO.isForBOM)
                {
                    RoomId = 0;
                }

                string strOK = objCDAL.DuplicateCheck(objDTO.Manufacturer, "add", objDTO.ID, "ManufacturerMaster", "Manufacturer", RoomId, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    ManufacturerMasterDTO objManufacturerMasterDTO = new ManufacturerMasterDTO();
                    objManufacturerMasterDTO = obj.GetManufacturerByNameNormal(objDTO.Manufacturer, RoomId, SessionHelper.CompanyID, true);
                    if (objManufacturerMasterDTO != null && objDTO.isForBOM == false)
                    {
                        objManufacturerMasterDTO.isForBOM = false;
                        objManufacturerMasterDTO.IsOnlyFromItemUI = objDTO.IsOnlyFromItemUI;
                        objManufacturerMasterDTO.EditedFrom = "Web";
                        objManufacturerMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        ManufacturerMasterDTO tempdto = new ManufacturerMasterDTO();
                        tempdto = obj.GetManufacturerByIDNormal(objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID, objDTO.isForBOM);
                        if (tempdto != null)
                        {
                            objManufacturerMasterDTO.AddedFrom = tempdto.AddedFrom;
                            objManufacturerMasterDTO.ReceivedOnWeb = tempdto.ReceivedOnWeb;
                        }
                        if (obj.Edit(objManufacturerMasterDTO))
                        {
                            message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                            status = "ok";
                            _NewIDForPopUp = objManufacturerMasterDTO.ID;
                            return Json(new { Message = message, Status = status, NewIDForPopUp = _NewIDForPopUp }, JsonRequestBehavior.AllowGet);
                        }
                    }

                }
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResManufacturer.ManufacturerName, objDTO.Manufacturer);  //"Manufacturer \"" + objDTO.Manufacturer + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                    long ReturnVal = obj.Insert(objDTO);
                    if (ReturnVal > 0)
                    {
                        //if it comes from item screen then add it to session
                        if (objDTO.ItemGUID != null)
                        {
                            List<ItemManufacturerDetailsDTO> lstItemManufacture = null;
                            if (Session["ItemManufacture"] != null)
                            {
                                lstItemManufacture = (List<ItemManufacturerDetailsDTO>)Session["ItemManufacture"];
                            }
                            else
                            {
                                lstItemManufacture = new List<ItemManufacturerDetailsDTO>();
                            }

                            lstItemManufacture.Add(new ItemManufacturerDetailsDTO() { ID = 0, ManufacturerID = objDTO.ID, SessionSr = lstItemManufacture.Count + 1, ItemGUID = objDTO.ItemGUID, Room = SessionHelper.RoomID, CompanyID = SessionHelper.CompanyID, Updated = DateTimeUtility.DateTimeNow, LastUpdatedBy = SessionHelper.UserID, Created = DateTimeUtility.DateTimeNow, CreatedBy = SessionHelper.UserID });

                            Session["ItemManufacture"] = lstItemManufacture;

                            /// 
                        }



                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                        _NewIDForPopUp = ReturnVal;
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }
            else
            {
                long RoomId = SessionHelper.RoomID;
                if (objDTO.isForBOM)
                {
                    RoomId = 0;
                }
                objDTO.LastUpdatedBy = SessionHelper.UserID;
                string strOK = objCDAL.DuplicateCheck(objDTO.Manufacturer, "edit", objDTO.ID, "ManufacturerMaster", "Manufacturer", RoomId, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResManufacturer.ManufacturerName, objDTO.Manufacturer);  //"Manufacturer \"" + objDTO.Manufacturer + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    if (objDTO.IsOnlyFromItemUI)
                    {
                        objDTO.EditedFrom = "Web";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    }
                    ManufacturerMasterDTO tempdto = new ManufacturerMasterDTO();
                    tempdto = obj.GetManufacturerByIDNormal(objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID, objDTO.isForBOM);
                    if (tempdto != null)
                    {
                        objDTO.AddedFrom = tempdto.AddedFrom;
                        objDTO.ReceivedOnWeb = tempdto.ReceivedOnWeb;
                    }
                    bool ReturnVal = obj.Edit(objDTO);
                    if (objDTO.isForBOM == true)
                    {
                        BOMItemMasterDAL objBOMItemMasterDAL = new BOMItemMasterDAL(SessionHelper.EnterPriseDBName);
                        objBOMItemMasterDAL.UpdateBOMMasterReference(objDTO.ID, "ManufacturerMaster", SessionHelper.UserID);
                    }
                    if (ReturnVal)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }

            return Json(new { Message = message, Status = status, NewIDForPopUp = _NewIDForPopUp }, JsonRequestBehavior.AllowGet);
        }

        ///// <summary>
        ///// Check Duplicate Manufacturer
        ///// </summary>
        ///// <param name="ManufacturerName"></param>
        ///// <param name="Action"></param>
        ///// <param name="ID"></param>
        ///// <returns></returns>
        //public string DuplicateManufacturerCheck(string ManufacturerName, string Action, Int64 ID)
        //{
        //    ManufacturerMasterController obj = new ManufacturerMasterController();
        //    return obj.DuplicateCheck(ManufacturerName, Action, ID);
        //}

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public JsonResult DeleteManufacturerRecords(string ids)
        {
            try
            {
                //eTurns.DAL.CommonDAL _repository = new eTurns.DAL.CommonDAL(SessionHelper.EnterPriseDBName);
                //return _repository.DeleteRecords(ImportMastersDTO.TableName.ManufacturerMaster.ToString(), ids, SessionHelper.RoomID, SessionHelper.CompanyID);

                string response = string.Empty;
                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                response = objCommonDAL.DeleteModulewise(ids, ImportMastersDTO.TableName.ManufacturerMaster.ToString(), false, SessionHelper.UserID, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);
                eTurns.DAL.CacheHelper<IEnumerable<ManufacturerMasterDTO>>.InvalidateCache();
                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult WrittenOffCategoryListAjax(JQueryDataTableParamModel param)
        {
            //ManufacturerMasterController obj = new ManufacturerMasterController();
            WrittenOffCategoryDAL obj = new WrittenOffCategoryDAL(SessionHelper.EnterPriseDBName);
            bool IsForBom = false;
            bool.TryParse(Convert.ToString(Request["IsForBom"]), out IsForBom);
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

            // set the default column sorting here, if first time then required to set 
            //if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID";
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
                sortColumnName = "ID desc";


            string searchQuery = string.Empty;
            //            if (param.sSearch != null && param.sSearch != "")
            //            {
            //                searchQuery = "WHERE Manufacturer like '%" + param.sSearch + "%'" + @"
            //                    OR RoomName like '%" + param.sSearch + "%'" + @" 
            //                    OR CreatedBy like '%" + param.sSearch + "%'";
            //            }

            int TotalRecordCount = 0;
            IEnumerable<WrittenOfCategoryDTO> DataFromDB;
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            //DataFromDB = obj.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, IsForBom, RoomDateFormat, CurrentTimeZone);

            DataFromDB = obj.GetPagedWrittenOffCategoryMaster(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, IsForBom, RoomDateFormat, CurrentTimeZone);
            var result = from c in DataFromDB
                         select new WrittenOfCategoryDTO
                         {
                             ID = c.ID,
                             WrittenOffCategory = c.WrittenOffCategory,
                             Created = c.Created,
                             CreatedBy = c.CreatedBy,
                             Updated = c.Updated,
                             LastUpdatedBy = c.LastUpdatedBy,
                             UpdatedByName = c.UpdatedByName,
                             Room = c.Room,
                             CreatedByName = c.CreatedByName,
                             IsArchived = c.IsArchived,
                             IsDeleted = c.IsDeleted,
                             //UDF1 = c.UDF1,
                             //UDF2 = c.UDF2,
                             //UDF3 = c.UDF3,
                             //UDF4 = c.UDF4,
                             //UDF5 = c.UDF5,
                             AddedFrom = c.AddedFrom,
                             EditedFrom = c.EditedFrom,
                             ReceivedOn = c.ReceivedOn,
                             ReceivedOnWeb = c.ReceivedOnWeb
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);


        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult WrittenOfCategoryCreate(Guid? ITEMGUID)
        {
            WrittenOfCategoryDTO objDTO = new WrittenOfCategoryDTO();
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.CreatedByName = SessionHelper.UserName;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.isForBOM = false;
            objDTO.IsOnlyFromItemUI = true;
            if (ITEMGUID != null)
            {
                objDTO.GUID = ITEMGUID;
            }

            //ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("ManufacturerMaster");
            //foreach (var i in ViewBag.UDFs)
            //{
            //    string _UDFColumnName = (string)i.UDFColumnName;
            //    ViewData[_UDFColumnName] = i.UDFDefaultValue;
            //}

            return PartialView("_CreateWrittenOfCategory", objDTO);
        }

        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>
        public ActionResult WrittenOfCategoryEdit(Int32 ID)
        {
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            if (IsDeleted || IsArchived)
            {
                ViewBag.ViewOnly = true;
            }

            //ManufacturerMasterController cntrl = new ManufacturerMasterController();
            WrittenOffCategoryDAL objDAL = new WrittenOffCategoryDAL(SessionHelper.EnterPriseDBName);
            WrittenOfCategoryDTO objDTO = objDAL.GetWrittenOffCategoryByIDNormal(ID, SessionHelper.RoomID, SessionHelper.CompanyID, null);
            //ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("ManufacturerMaster");
            //ViewData["UDF1"] = objDTO.UDF1;
            //ViewData["UDF2"] = objDTO.UDF2;
            //ViewData["UDF3"] = objDTO.UDF3;
            //ViewData["UDF4"] = objDTO.UDF4;
            //ViewData["UDF5"] = objDTO.UDF5;
            objDTO.IsOnlyFromItemUI = true;
            return PartialView("_CreateWrittenOfCategory", objDTO);
        }
        #endregion

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public JsonResult DeleteWrittenOffCategoryList(string ids)
        {
            try
            {
                //eTurns.DAL.CommonDAL _repository = new eTurns.DAL.CommonDAL(SessionHelper.EnterPriseDBName);
                //return _repository.DeleteRecords(ImportMastersDTO.TableName.ManufacturerMaster.ToString(), ids, SessionHelper.RoomID, SessionHelper.CompanyID);

                string response = string.Empty;
                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                response = objCommonDAL.DeleteModulewise(ids, ImportMastersDTO.TableName.ToolWrittenOffCategory.ToString(), false, SessionHelper.UserID, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);
                eTurns.DAL.CacheHelper<IEnumerable<ManufacturerMasterDTO>>.InvalidateCache();
                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        [ValidateAntiForgeryToken]
        public JsonResult WrittenOfCategorySave(WrittenOfCategoryDTO objDTO)
        {
            string message = "";
            string status = "";
            WrittenOffCategoryDAL obj = new WrittenOffCategoryDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            objDTO.Room = SessionHelper.RoomID;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            Int64 _NewIDForPopUp = 0;
            if (string.IsNullOrEmpty(objDTO.WrittenOffCategory))
            {
                message = string.Format(ResMessage.Required, ResWrittenOfCategory.WrittenOffCategory);// "WrittenOffCategory name is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.Room = SessionHelper.RoomID;
            if (objDTO.ID == 0)
            {
                objDTO.CreatedBy = SessionHelper.UserID;
                long RoomId = SessionHelper.RoomID;
                if (objDTO.isForBOM)
                {
                    RoomId = 0;
                }

                string strOK = objCDAL.DuplicateCheck(objDTO.WrittenOffCategory, "add", objDTO.ID, "ToolWrittenOffCategory", "WrittenOffCategory", RoomId, SessionHelper.CompanyID);
                //if (strOK == "duplicate")
                //{
                //    WrittenOfCategoryDTO objManufacturerMasterDTO = new WrittenOfCategoryDTO();
                //    objManufacturerMasterDTO = obj.GetWrittenOffCategoryByNameNormal(objDTO.WrittenOffCategory, RoomId, SessionHelper.CompanyID, true);
                //    if (objManufacturerMasterDTO != null && objDTO.isForBOM == false)
                //    {
                //        objManufacturerMasterDTO.isForBOM = false;
                //        objManufacturerMasterDTO.IsOnlyFromItemUI = objDTO.IsOnlyFromItemUI;
                //        objManufacturerMasterDTO.EditedFrom = "Web";
                //        objManufacturerMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                //        ManufacturerMasterDTO tempdto = new ManufacturerMasterDTO();
                //        //tempdto = obj.GetManufacturerByIDNormal(objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID, objDTO.isForBOM);
                //        //if (tempdto != null)
                //        //{
                //        //    objManufacturerMasterDTO.AddedFrom = tempdto.AddedFrom;
                //        //    objManufacturerMasterDTO.ReceivedOnWeb = tempdto.ReceivedOnWeb;
                //        //}
                //        //if (obj.Edit(objManufacturerMasterDTO))
                //        //{
                //        //    message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                //        //    status = "ok";
                //        //    _NewIDForPopUp = objManufacturerMasterDTO.ID;
                //        //    return Json(new { Message = message, Status = status, NewIDForPopUp = _NewIDForPopUp }, JsonRequestBehavior.AllowGet);
                //        //}
                //    }

                //}
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResManufacturer.ManufacturerName, objDTO.WrittenOffCategory);  //"Manufacturer \"" + objDTO.Manufacturer + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                    long ReturnVal = obj.Insert(objDTO);
                    if (ReturnVal > 0)
                    {
                        ////if it comes from item screen then add it to session
                        //if (objDTO.GUID != null)
                        //{
                        //    List<ItemManufacturerDetailsDTO> lstItemManufacture = null;
                        //    if (Session["ItemManufacture"] != null)
                        //    {
                        //        lstItemManufacture = (List<ItemManufacturerDetailsDTO>)Session["ItemManufacture"];
                        //    }
                        //    else
                        //    {
                        //        lstItemManufacture = new List<ItemManufacturerDetailsDTO>();
                        //    }

                        //    lstItemManufacture.Add(new ItemManufacturerDetailsDTO() { ID = 0, ManufacturerID = objDTO.ID, SessionSr = lstItemManufacture.Count + 1, ItemGUID = objDTO.GUID, Room = SessionHelper.RoomID, CompanyID = SessionHelper.CompanyID, Updated = DateTimeUtility.DateTimeNow, LastUpdatedBy = SessionHelper.UserID, Created = DateTimeUtility.DateTimeNow, CreatedBy = SessionHelper.UserID });

                        //    Session["ItemManufacture"] = lstItemManufacture;

                        //    /// 
                        //}



                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                        _NewIDForPopUp = ReturnVal;
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }
            else
            {
                long RoomId = SessionHelper.RoomID;
                if (objDTO.isForBOM)
                {
                    RoomId = 0;
                }
                objDTO.LastUpdatedBy = SessionHelper.UserID;
                objDTO.Updated = DateTimeUtility.DateTimeNow;
                string strOK = objCDAL.DuplicateCheck(objDTO.WrittenOffCategory, "edit", objDTO.ID, "ToolWrittenOffCategory", "WrittenOffCategory", RoomId, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResManufacturer.ManufacturerName, objDTO.WrittenOffCategory);  //"Manufacturer \"" + objDTO.Manufacturer + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    if (objDTO.IsOnlyFromItemUI)
                    {
                        objDTO.EditedFrom = "Web";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    }
                    WrittenOfCategoryDTO tempDTO = new WrittenOfCategoryDTO();
                    tempDTO = obj.GetWrittenOffCategoryByIDNormal(objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID, null);
                    if (tempDTO != null)
                    {
                        objDTO.AddedFrom = tempDTO.AddedFrom;
                        objDTO.ReceivedOnWeb = tempDTO.ReceivedOnWeb;
                        objDTO.ReceivedOn = tempDTO.ReceivedOn;
                    }
                    bool ReturnVal = obj.Edit(objDTO);
                    //if (objDTO.isForBOM == true)
                    //{
                    //    BOMItemMasterDAL objBOMItemMasterDAL = new BOMItemMasterDAL(SessionHelper.EnterPriseDBName);
                    //    objBOMItemMasterDAL.UpdateBOMMasterReference(objDTO.ID, "ManufacturerMaster", SessionHelper.UserID);
                    //}
                    if (ReturnVal)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }

            return Json(new { Message = message, Status = status, NewIDForPopUp = _NewIDForPopUp }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult ManufacturerListAjax(JQueryDataTableParamModel param)
        {
            //ManufacturerMasterController obj = new ManufacturerMasterController();
            ManufacturerMasterDAL obj = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName);
            bool IsForBom = false;
            bool.TryParse(Convert.ToString(Request["IsForBom"]), out IsForBom);
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

            // set the default column sorting here, if first time then required to set 
            //if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID";
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
                sortColumnName = "ID desc";


            string searchQuery = string.Empty;
            //            if (param.sSearch != null && param.sSearch != "")
            //            {
            //                searchQuery = "WHERE Manufacturer like '%" + param.sSearch + "%'" + @"
            //                    OR RoomName like '%" + param.sSearch + "%'" + @" 
            //                    OR CreatedBy like '%" + param.sSearch + "%'";
            //            }

            int TotalRecordCount = 0;
            IEnumerable<ManufacturerMasterDTO> DataFromDB;
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            //DataFromDB = obj.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, IsForBom, RoomDateFormat, CurrentTimeZone);

            DataFromDB = obj.GetPagedManufacturerMaster(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, IsForBom, RoomDateFormat, CurrentTimeZone);
            var result = from c in DataFromDB
                         select new ManufacturerMasterDTO
                         {
                             ID = c.ID,
                             Manufacturer = c.Manufacturer,
                             RoomName = c.RoomName,
                             Created = c.Created,
                             CreatedBy = c.CreatedBy,
                             Updated = c.Updated,
                             LastUpdatedBy = c.LastUpdatedBy,
                             UpdatedByName = c.UpdatedByName,
                             Room = c.Room,
                             CreatedByName = c.CreatedByName,
                             IsArchived = c.IsArchived,
                             IsDeleted = c.IsDeleted,
                             UDF1 = c.UDF1,
                             UDF2 = c.UDF2,
                             UDF3 = c.UDF3,
                             UDF4 = c.UDF4,
                             UDF5 = c.UDF5,
                             AddedFrom = c.AddedFrom,
                             EditedFrom = c.EditedFrom,
                             ReceivedOn = c.ReceivedOn,
                             ReceivedOnWeb = c.ReceivedOnWeb
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);


        }
        /// <summary>
        ///GET ALL: /Master/ 
        /// </summary>
        /// <returns></returns>
        public ActionResult ManufacturerList()
        {
            return View();
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult ManufacturerCreate(bool isforbom, Guid? ITEMGUID)
        {
            ManufacturerMasterDTO objDTO = new ManufacturerMasterDTO();
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.CreatedByName = SessionHelper.UserName;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.RoomName = SessionHelper.RoomName;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.isForBOM = isforbom;
            objDTO.IsOnlyFromItemUI = true;
            if (ITEMGUID != null)
            {
                objDTO.ItemGUID = ITEMGUID;
            }

            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("ManufacturerMaster");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }

            return PartialView("_CreateManufacturer", objDTO);
        }

        public PartialViewResult _CreateWrittenOfCategory()
        {
            return PartialView();
        }


        public PartialViewResult _CreateManufacturer()
        {
            return PartialView();
        }

        public string UpdateManufacturerData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            //ManufacturerMasterController obj = new ManufacturerMasterController();
            ManufacturerMasterDAL obj = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName);
            obj.UpdateData(id, value, rowId, columnPosition, columnId, columnName);
            return value;
        }

        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>
        public ActionResult ManufacturerEdit(Int32 ID)
        {
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            if (IsDeleted || IsArchived)
            {
                ViewBag.ViewOnly = true;
            }

            //ManufacturerMasterController cntrl = new ManufacturerMasterController();
            ManufacturerMasterDAL cntrl = new ManufacturerMasterDAL(SessionHelper.EnterPriseDBName);
            ManufacturerMasterDTO objDTO = cntrl.GetManufacturerByIDNormal(ID, SessionHelper.RoomID, SessionHelper.CompanyID, null);
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("ManufacturerMaster");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;
            objDTO.IsOnlyFromItemUI = true;
            return PartialView("_CreateManufacturer", objDTO);
        }


        #region "GXPR Consigned Job Master"

        /// <summary>
        ///GET ALL: /Master/ 
        /// </summary>
        /// <returns></returns>
        public ActionResult GXPRConsignedJobList()
        {
            return View();
        }

        /// <summary>
        /// _CreateGXPRConsignedJob
        /// </summary>
        /// <returns></returns>
        public PartialViewResult _CreateGXPRConsignedJob()
        {
            return PartialView();
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult GXPRConsignedJobCreate()
        {
            GXPRConsigmentJobMasterDTO objDTO = new GXPRConsigmentJobMasterDTO()
            {
                Created = DateTimeUtility.DateTimeNow,
                Updated = DateTimeUtility.DateTimeNow,
                CreatedBy = SessionHelper.UserID,
                CreatedByName = SessionHelper.UserName,
                LastUpdatedBy = SessionHelper.UserID,
                Room = SessionHelper.RoomID,
                CompanyID = SessionHelper.CompanyID,
                RoomName = SessionHelper.RoomName,
                UpdatedByName = SessionHelper.UserName,
            };
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("GXPRConsigmentJobMaster");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            return PartialView("_CreateGXPRConsignedJob", objDTO);
        }

        /// <summary>
        /// GXPRConsignedJobSave
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GXPRConsignedJobSave(GXPRConsigmentJobMasterDTO objDTO)
        {
            string message = "";
            string status = "";
            //GXPRConsignedJobController obj = new GXPRConsignedJobController();
            GXPRConsignedJobMasterDAL obj = new GXPRConsignedJobMasterDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

            if (string.IsNullOrEmpty(objDTO.GXPRConsigmentJob))
            {
                message = string.Format(ResMessage.Required, ResGXPRConsignedJob.GXPRConsignedJob);// "GXPRConsigmentJob name is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.Room = SessionHelper.RoomID;
            if (objDTO.ID == 0)
            {
                objDTO.CreatedBy = SessionHelper.UserID;

                string strOK = objCDAL.DuplicateCheck(objDTO.GXPRConsigmentJob, "add", objDTO.ID, "GXPRConsigmentJobMaster", "GXPRConsigmentJob", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResGXPRConsignedJob.GXPRConsignedJob, objDTO.GXPRConsigmentJob);  //"GXPRConsigmentJob \"" + objDTO.GXPRConsigmentJob + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    objDTO.GUID = Guid.NewGuid();
                    long ReturnVal = obj.Insert(objDTO);
                    if (ReturnVal > 0)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }
            else
            {

                string strOK = objCDAL.DuplicateCheck(objDTO.GXPRConsigmentJob, "edit", objDTO.ID, "GXPRConsigmentJobMaster", "GXPRConsigmentJob", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResGXPRConsignedJob.GXPRConsignedJob, objDTO.GXPRConsigmentJob);  //"GXPRConsigmentJob \"" + objDTO.GXPRConsigmentJob + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    bool ReturnVal = obj.Edit(objDTO);
                    if (ReturnVal)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }


            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        ///// <summary>
        ///// Check Duplicate GXPRConsignedJob
        ///// </summary>
        ///// <param name="GXPRConsignedJobName"></param>
        ///// <param name="Action"></param>
        ///// <param name="ID"></param>
        ///// <returns></returns>
        //public string DuplicateGXPRConsignedJobCheck(string GXPRConsignedJobName, string Action, Int64 ID)
        //{
        //    GXPRConsignedJobController obj = new GXPRConsignedJobController();
        //    return obj.DuplicateCheck(GXPRConsignedJobName, Action, ID);
        //}

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public string DeleteGXPRConsignedJobRecords(string ids)
        {
            try
            {
                eTurns.DAL.CommonDAL _repository = new eTurns.DAL.CommonDAL(SessionHelper.EnterPriseDBName);
                return _repository.DeleteRecords(ImportMastersDTO.TableName.GXPRConsigmentJobMaster.ToString(), ids, SessionHelper.RoomID, SessionHelper.CompanyID);

                //GXPRConsignedJobController obj = new GXPRConsignedJobController();
                //obj.DeleteRecords(ids, SessionHelper.UserID, SessionHelper.CompanyID);
                //return "ok";

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// // below function used to Save the User wise filter data for jquery data table
        /// </summary>
        /// <param name="UserID"></param> will have userid for currently logged in user
        /// <param name="Data"></param> will have json string of filter criteria 
        /// <returns></returns>
        //public ActionResult SaveGXPRConsignedJobGridState(int UserID, string Data, string GXPRConsignedJobName)
        //{
        //    UsersUISettingsController obj = new UsersUISettingsController();
        //    UsersUISettingsDTO objDTO = new UsersUISettingsDTO();
        //    objDTO.UserID = UserID;
        //    objDTO.JSONDATA = Data;
        //    objDTO.ListName = GXPRConsignedJobName;
        //    obj.PutRecord(objDTO);

        //    return Json(new { objDTO.JSONDATA }, JsonRequestBehavior.AllowGet);

        //    //return Json(new {sEcho = 111},JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// // below function used to load the User wise saved filter data for jquery data table
        /// </summary>
        /// <param name="UserID"></param> will have userid for currently logged in user
        /// <returns></returns>
        //public ActionResult LoadGXPRConsignedJobGridState(int UserID, string GXPRConsignedJobName)
        //{
        //    //string jsonData = @"{""iCreate"":1350639486123,""iStart"":0,""iEnd"":0,""iLength"":10,""aaSorting"":[[0,""asc"",0,""Name""]],""oSearch"":{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true},""aoSearchCols"":[{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true},{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true},{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true},{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}],""abVisCols"":[false,true,true,true,true],""ColReorder"":[0,1,3,2,4]}";
        //    UsersUISettingsController obj = new UsersUISettingsController();
        //    UsersUISettingsDTO objDTO = new UsersUISettingsDTO();
        //    objDTO = obj.GetRecord(UserID, GXPRConsignedJobName);
        //    string jsonData = objDTO.JSONDATA;
        //    return Json(new { jsonData }, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult GetGXPRConsignedJobList(JQueryDataTableParamModel param)
        {
            //GXPRConsignedJobController obj = new GXPRConsignedJobController();
            GXPRConsignedJobMasterDAL obj = new GXPRConsignedJobMasterDAL(SessionHelper.EnterPriseDBName);

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

            // set the default column sorting here, if first time then required to set 
            if (sortColumnName == "0" || sortColumnName == "undefined")
                sortColumnName = "ID";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;
            if (param.sSearch != null && param.sSearch != "")
            {
                searchQuery = "WHERE GXPRConsigmentJob like '%" + param.sSearch + "%'" + @"
                    OR RoomName like '%" + param.sSearch + "%'" + @" 
                    OR CreatedBy like '%" + param.sSearch + "%'";
            }

            int TotalRecordCount = 0;
            var DataFromDB = obj.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted);

            var result = from c in DataFromDB
                         select new GXPRConsigmentJobMasterDTO
                         {
                             ID = c.ID,
                             GXPRConsigmentJob = c.GXPRConsigmentJob,
                             RoomName = c.RoomName,
                             Created = c.Created,
                             CreatedBy = c.CreatedBy,
                             Updated = c.Updated,
                             LastUpdatedBy = c.LastUpdatedBy,
                             UpdatedByName = c.UpdatedByName,
                             Room = c.Room,
                             CreatedByName = c.CreatedByName,
                             IsArchived = c.IsArchived,
                             IsDeleted = c.IsDeleted,
                             UDF1 = c.UDF1,
                             UDF2 = c.UDF2,
                             UDF3 = c.UDF3,
                             UDF4 = c.UDF4,
                             UDF5 = c.UDF5
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);


        }

        /// <summary>
        /// UpdateGXPRConsignedJobData
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <param name="rowId"></param>
        /// <param name="columnPosition"></param>
        /// <param name="columnId"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string UpdateGXPRConsignedJobData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            //GXPRConsignedJobController obj = new GXPRConsignedJobController();
            GXPRConsignedJobMasterDAL obj = new GXPRConsignedJobMasterDAL(SessionHelper.EnterPriseDBName);
            obj.UpdateData(id, value, rowId, columnPosition, columnId, columnName);
            return value;
        }

        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>
        public ActionResult GXPRConsignedJobEdit(Int64 ID)
        {
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            if (IsDeleted || IsArchived)
            {
                ViewBag.ViewOnly = true;
            }

            //GXPRConsignedJobController obj = new GXPRConsignedJobController();
            GXPRConsignedJobMasterDAL obj = new GXPRConsignedJobMasterDAL(SessionHelper.EnterPriseDBName);
            GXPRConsigmentJobMasterDTO objDTO = obj.GetRecord(ID, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted);
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("GXPRConsigmentJobMaster");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;
            return PartialView("_CreateGXPRConsignedJob", objDTO);
        }

        #endregion

        #region "TechnicianMaster"


        public ActionResult TechnicianList()
        {
            return View();
        }

        public PartialViewResult _CreateTechnician()
        {
            return PartialView();
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult TechnicianCreate()
        {
            TechnicianMasterDTO objDTO = new TechnicianMasterDTO();
            objDTO.ID = 0;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.CreatedByName = SessionHelper.UserName;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.RoomName = SessionHelper.RoomName;
            objDTO.GUID = Guid.NewGuid();

            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("TechnicianMaster");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }

            return PartialView("_CreateTechnician", objDTO);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult TechnicianSave(TechnicianMasterDTO objDTO)
        {
            string message = "";
            string status = "";
            //TechnicianController obj = new TechnicianController();
            TechnicialMasterDAL obj = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            long _NewIDForPopUp = 0;
            //if (string.IsNullOrEmpty(objDTO.Technician))
            //{
            //    message = string.Format(ResMessage.Required, ResTechnician.Technician);//"Technician name is required.";
            //    status = "fail";
            //    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            //}
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.Room = SessionHelper.RoomID;

            if (objDTO.ID == 0)
            {
                objDTO.CreatedBy = SessionHelper.UserID;

                string strOK = objCDAL.DuplicateCheck(objDTO.TechnicianCode, "add", objDTO.ID, "TechnicianMaster", "TechnicianCode", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResTechnician.TechnicianCode, objDTO.TechnicianCode);  // "TechnicianCode \"" + objDTO.TechnicianCode + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    objDTO.GUID = Guid.NewGuid();
                    long ReturnVal = obj.Insert(objDTO);

                    if (ReturnVal > 0)
                    {
                        message = ResMessage.SaveMessage;// ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                        _NewIDForPopUp = objDTO.ID;
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);// string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }

            }
            else
            {
                objDTO.LastUpdatedBy = SessionHelper.UserID;
                string strOK = objCDAL.DuplicateCheck(objDTO.TechnicianCode, "edit", objDTO.ID, "TechnicianMaster", "TechnicianCode", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResTechnician.TechnicianCode, objDTO.TechnicianCode);  //"TechnicianCode \"" + objDTO.TechnicianCode + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    bool ReturnVal = obj.Edit(objDTO);
                    if (ReturnVal)
                    {
                        message = ResMessage.SaveMessage; //ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);//string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }

            //if (status == "duplicate")
            //    throw new Exception("Duplicate Found", new Exception("Duplicate Found"));                
            //else if (status == "fail")
            //    throw new Exception("Error! Record Not Saved");
            //else
            //    return Content(message);
            return Json(new { Message = message, Status = status, NewIDForPopUp = _NewIDForPopUp }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>
        public ActionResult TechnicianEdit(Int64 ID)
        {
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            if (IsDeleted || IsArchived)
            {
                ViewBag.ViewOnly = true;
            }

            //TechnicianController obj = new TechnicianController();
            TechnicialMasterDAL obj = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
            TechnicianMasterDTO objDTO = obj.GetTechnicianByIDNormal(ID, SessionHelper.RoomID, SessionHelper.CompanyID);
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("TechnicianMaster");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;
            return PartialView("_CreateTechnician", objDTO);
            //return View("TechnicianEdit",objDTO);
        }

        public ActionResult TechnicianHistoryEdit(Int64 ID)
        {
            TechnicialMasterDAL obj = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
            //TechnicianController obj = new TechnicianController();
            TechnicianMasterDTO objDTO = obj.GetTechnicianHistoryByHistoryID(ID);
            return PartialView("_CreateTechnician", objDTO);
        }

        #region Ajax Data Provider

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult TechnicianListAjax(JQueryDataTableParamModel param)
        {
            //TechnicianController obj = new TechnicianController();
            TechnicialMasterDAL obj = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);

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


            // set the default column sorting here, if first time then required to set 
            //if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID";
            if (!string.IsNullOrEmpty(sortColumnName))
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
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetPagedTechnicianMaster(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, RoomDateFormat, CurrentTimeZone);

            var result = from c in DataFromDB
                         select new TechnicianMasterDTO
                         {
                             ID = c.ID,
                             Technician = c.Technician,
                             RoomName = c.RoomName,
                             Created = c.Created,
                             Updated = c.Updated,
                             IsDeleted = c.IsDeleted,
                             IsArchived = c.IsArchived,
                             UpdatedByName = c.UpdatedByName,
                             CreatedByName = c.CreatedByName,
                             UDF1 = c.UDF1,
                             UDF2 = c.UDF2,
                             UDF3 = c.UDF3,
                             UDF4 = c.UDF4,
                             UDF5 = c.UDF5,
                             TechnicianCode = c.TechnicianCode
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);


        }

        //public string UpdateData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        //{
        //    //TechnicianController obj = new TechnicianController();
        //    TechnicialMasterDAL obj = new TechnicialMasterDAL(SessionHelper.EnterPriseDBName);
        //    obj.UpdateData(id, value, rowId, columnPosition, columnId, columnName);
        //    return value;
        //}

        //public string DuplicateCheck(string TechnicianName, string ActionMode, int ID)
        //{
        //    TechnicianController obj = new TechnicianController();
        //    return obj.DuplicateCheck(TechnicianName, ActionMode, ID);
        //}

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public JsonResult DeleteRecords(string ids)
        {
            try
            {
                //eTurns.DAL.CommonDAL _repository = new eTurns.DAL.CommonDAL(SessionHelper.EnterPriseDBName);
                //return _repository.DeleteRecords(ImportMastersDTO.TableName.TechnicianMaster.ToString(), ids, SessionHelper.RoomID, SessionHelper.CompanyID);

                string response = string.Empty;
                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                response = objCommonDAL.DeleteModulewise(ids, ImportMastersDTO.TableName.TechnicianMaster.ToString(), false, SessionHelper.UserID, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);
                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion




        #endregion

        #region "JobTypeMaster"

        public ActionResult JobTypeList()
        {
            return View();
        }

        public PartialViewResult _CreateJobType()
        {
            return PartialView();
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult JobTypeCreate()
        {
            JobTypeMasterDTO objDTO = new JobTypeMasterDTO();
            objDTO.ID = 0;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.RoomName = SessionHelper.RoomName;
            objDTO.GUID = Guid.NewGuid();
            objDTO.CreatedByName = SessionHelper.UserName;
            objDTO.LastUpdatedByName = SessionHelper.UserName;
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("JobTypeMaster");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            return PartialView("_CreateJobType", objDTO);
            //return View();
        }

        /// <summary>
        /// JSON Record Save - Enter key Save/Update
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Technician"></param>
        /// <returns></returns>
        public JsonResult JobTypeSave(JobTypeMasterDTO objDTO)
        {
            string message = "";
            string status = "";
            //JobTypeMasterController obj = new JobTypeMasterController();
            JobTypeMasterDAL obj = new JobTypeMasterDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

            if (string.IsNullOrEmpty(objDTO.JobType))
            {
                message = string.Format(ResMessage.Required, ResJobType.JobType);// "JobType is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }

            string JobType = objDTO.JobType.Replace("'", "''");
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.LastUpdatedByName = SessionHelper.UserName;
            objDTO.Room = SessionHelper.RoomID;
            if (objDTO.ID == 0)
            {
                objDTO.CreatedBy = SessionHelper.UserID;
                string strOK = objCDAL.DuplicateCheck(JobType, "add", objDTO.ID, "JobTypeMaster", "JobType", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResJobType.JobType, objDTO.JobType);  //"JobType \"" + objDTO.JobType + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    objDTO.GUID = Guid.NewGuid();
                    long ReturnVal = obj.Insert(objDTO);
                    if (ReturnVal > 0)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }
            else
            {
                objDTO.LastUpdatedBy = SessionHelper.UserID;
                string strOK = objCDAL.DuplicateCheck(JobType, "edit", objDTO.ID, "JobTypeMaster", "JobType", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResJobType.JobType, objDTO.JobType);  //"JobType \"" + objDTO.JobType + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    bool ReturnVal = obj.Edit(objDTO);

                    if (ReturnVal)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }

            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>
        public ActionResult JobTypeEdit(Int64 ID)
        {
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            if (IsDeleted || IsArchived)
            {
                ViewBag.ViewOnly = true;
            }

            //JobTypeMasterController obj = new JobTypeMasterController();
            JobTypeMasterDAL obj = new JobTypeMasterDAL(SessionHelper.EnterPriseDBName);
            JobTypeMasterDTO objDTO = obj.GetJobTypeByIDNormal(ID, SessionHelper.RoomID, SessionHelper.CompanyID);
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("JobTypeMaster");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;
            return PartialView("_CreateJobType", objDTO);
        }

        public string DuplicateJobTypeCheck(string Name, string ActionMode, int ID)
        {
            //JobTypeMasterController obj = new JobTypeMasterController();
            JobTypeMasterDAL obj = new JobTypeMasterDAL(SessionHelper.EnterPriseDBName);
            return obj.DuplicateCheck(Name, ActionMode, ID);
        }

        #region Ajax Data Provider

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult JobTypeListAjax(JQueryDataTableParamModel param)
        {
            //JobTypeMasterController obj = new JobTypeMasterController();
            JobTypeMasterDAL obj = new JobTypeMasterDAL(SessionHelper.EnterPriseDBName);

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

            // set the default column sorting here, if first time then required to set 
            if (sortColumnName == "0" || sortColumnName == "undefined")
                sortColumnName = "ID";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;
            if (param.sSearch != null && param.sSearch != "")
            {
                searchQuery = "WHERE JobType like '%" + param.sSearch + "%'" + @"
                    OR RoomName like '%" + param.sSearch + "%'" + @" 
                    OR CreatedBy like '%" + param.sSearch + "%'";
            }

            int TotalRecordCount = 0;
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetPagedJobTypeMaster(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, RoomDateFormat, CurrentTimeZone);

            var result = from c in DataFromDB
                         select new JobTypeMasterDTO
                         {
                             ID = c.ID,
                             JobType = c.JobType,
                             RoomName = c.RoomName,
                             Created = c.Created,
                             CreatedBy = c.CreatedBy,
                             LastUpdated = c.LastUpdated,
                             LastUpdatedBy = c.LastUpdatedBy,
                             LastUpdatedByName = c.LastUpdatedByName,
                             Room = c.Room,
                             CreatedByName = c.CreatedByName,
                             IsArchived = c.IsArchived,
                             IsDeleted = c.IsDeleted,
                             UDF1 = c.UDF1,
                             UDF2 = c.UDF2,
                             UDF3 = c.UDF3,
                             UDF4 = c.UDF4,
                             UDF5 = c.UDF5
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);


        }

        public string UpdateJobTypeData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            //JobTypeMasterController obj = new JobTypeMasterController();
            JobTypeMasterDAL obj = new JobTypeMasterDAL(SessionHelper.EnterPriseDBName);
            //obj.PutUpdateData(id, value, rowId, columnPosition, columnId, columnName);
            //int idx = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList().FindIndex(x => x.JobType == value && x.ID != id);
            int idx = obj.GetJobTypeByJobTypeByIDNormal(SessionHelper.RoomID, SessionHelper.CompanyID, id, value);
            if (idx >= 0)
            {
                return ResJobType.JobTypeExist;
            }
            else
            {
                obj.UpdateJobTypeMaster(id, value, (int)SessionHelper.UserID, columnName);
            }
            return value;

        }

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public string DeleteJobTypeRecords(string ids)
        {
            try
            {
                eTurns.DAL.CommonDAL _repository = new eTurns.DAL.CommonDAL(SessionHelper.EnterPriseDBName);
                return _repository.DeleteRecords(ImportMastersDTO.TableName.JobTypeMaster.ToString(), ids, SessionHelper.RoomID, SessionHelper.CompanyID);

                //JobTypeMasterController obj = new JobTypeMasterController();
                //obj.DeleteRecords(ids, SessionHelper.UserID, SessionHelper.CompanyID);
                //return "ok";

                //return "not ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #endregion

        #region "GLAccountMaster"

        public ActionResult GLAccountList()
        {
            return View();
        }

        public PartialViewResult _CreateGLAccount()
        {
            return PartialView();
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult GLAccountCreate(bool isforbom)
        {
            GLAccountMasterDTO objDTO = new GLAccountMasterDTO();
            objDTO.ID = 0;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.RoomName = SessionHelper.RoomName;
            objDTO.GUID = Guid.NewGuid();
            objDTO.CreatedByName = SessionHelper.UserName;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.isForBOM = isforbom;
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("GLAccountMaster");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            return PartialView("_CreateGLAccount", objDTO);
            //return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult GLAccountSave(GLAccountMasterDTO objDTO)
        {
            string message = "";
            string status = "";
            //GLAccountMasterController obj = new GLAccountMasterController();
            GLAccountMasterDAL obj = new GLAccountMasterDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

            if (string.IsNullOrEmpty(objDTO.GLAccount))
            {
                message = string.Format(ResMessage.Required, ResGLAccount.GLAccount);// "Account number name is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            objDTO.Room = SessionHelper.RoomID;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;
            if (objDTO.ID == 0)
            {
                objDTO.CreatedBy = SessionHelper.UserID;
                long RoomId = SessionHelper.RoomID;
                if (objDTO.isForBOM)
                {
                    RoomId = 0;
                }
                string strOK = objCDAL.DuplicateCheck(objDTO.GLAccount, "add", objDTO.ID, "GLAccountMaster", "GLAccount", RoomId, SessionHelper.CompanyID);
                //if (strOK == "duplicate")
                //{
                //    GLAccountMasterDTO objGLAccountMasterDTO = new GLAccountMasterDTO();
                //    objGLAccountMasterDTO = obj.GetAllRecords(RoomId, SessionHelper.CompanyID, false, false, false).Where(t => t.GLAccount == objDTO.GLAccount && t.isForBOM == true).ToList().FirstOrDefault();
                //    if (objGLAccountMasterDTO != null && objDTO.isForBOM == false)
                //    {
                //        objGLAccountMasterDTO.isForBOM = false;
                //        objGLAccountMasterDTO.Description = objDTO.Description;
                //        if (obj.Edit(objGLAccountMasterDTO))
                //        {
                //            message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                //            status = "ok";

                //            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                //        }
                //    }

                //}

                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResGLAccount.GLAccount, objDTO.GLAccount);  //"Account \"" + objDTO.GLAccount + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    objDTO.GUID = Guid.NewGuid();
                    long ReturnVal = obj.Insert(objDTO);

                    if (ReturnVal > 0)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }
            else
            {
                long RoomId = SessionHelper.RoomID;
                if (objDTO.isForBOM)
                {
                    RoomId = 0;
                }
                string strOK = objCDAL.DuplicateCheck(objDTO.GLAccount, "edit", objDTO.ID, "GLAccountMaster", "GLAccount", RoomId, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResGLAccount.GLAccount, objDTO.GLAccount);  //"Account \"" + objDTO.GLAccount + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    bool ReturnVal = obj.Edit(objDTO);
                    if (objDTO.isForBOM == true)
                    {
                        BOMItemMasterDAL objBOMItemMasterDAL = new BOMItemMasterDAL(SessionHelper.EnterPriseDBName);
                        objBOMItemMasterDAL.UpdateBOMMasterReference(objDTO.ID, "GLAccountMaster", SessionHelper.UserID);
                    }
                    if (ReturnVal)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }

            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>
        public ActionResult GLAccountEdit(Int64 ID)
        {
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            if (IsDeleted || IsArchived)
            {
                ViewBag.ViewOnly = true;
            }

            //GLAccountMasterController obj = new GLAccountMasterController();
            GLAccountMasterDAL obj = new GLAccountMasterDAL(SessionHelper.EnterPriseDBName);
            GLAccountMasterDTO objDTO = obj.GetGLAccountByID(ID);
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("GLAccountMaster");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;
            return PartialView("_CreateGLAccount", objDTO);
        }

        //public string DuplicateGLAccountCheck(string GlAccountName, string ActionMode, int ID)
        //{
        //    GLAccountMasterController obj = new GLAccountMasterController();
        //    return obj.DuplicateCheck(GlAccountName, ActionMode, ID);
        //}

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult GetGLAccountList(JQueryDataTableParamModel param)
        {
            //GLAccountMasterController obj = new GLAccountMasterController();
            GLAccountMasterDAL obj = new GLAccountMasterDAL(SessionHelper.EnterPriseDBName);
            bool IsForBom = false;
            bool.TryParse(Convert.ToString(Request["IsForBom"]), out IsForBom);
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

            // set the default column sorting here, if first time then required to set 
            //if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID";

            if (!string.IsNullOrEmpty(sortColumnName))
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
            IEnumerable<GLAccountMasterDTO> DataFromDB;
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            DataFromDB = obj.GetPagedGLAccountMaster(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, IsForBom, RoomDateFormat, CurrentTimeZone);



            var result = from c in DataFromDB
                         select new GLAccountMasterDTO
                         {
                             ID = c.ID,
                             GLAccount = c.GLAccount,
                             Description = c.Description,
                             RoomName = c.RoomName,
                             Created = c.Created,
                             Updated = c.Updated,
                             UpdatedByName = c.UpdatedByName,
                             CreatedByName = c.CreatedByName,
                             IsArchived = c.IsArchived,
                             IsDeleted = c.IsDeleted,
                             UDF1 = c.UDF1,
                             UDF2 = c.UDF2,
                             UDF3 = c.UDF3,
                             UDF4 = c.UDF4,
                             UDF5 = c.UDF5
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);


        }

        public string UpdateGLAccountData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            //GLAccountMasterController obj = new GLAccountMasterController();
            GLAccountMasterDAL obj = new GLAccountMasterDAL(SessionHelper.EnterPriseDBName);
            obj.UpdateData(id, value, rowId, columnPosition, columnId, columnName);
            return value;
        }

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public JsonResult DeleteGLAccountRecords(string ids)
        {
            try
            {
                //eTurns.DAL.CommonDAL _repository = new eTurns.DAL.CommonDAL(SessionHelper.EnterPriseDBName);
                //return _repository.DeleteRecords(ImportMastersDTO.TableName.GLAccountMaster.ToString(), ids, SessionHelper.RoomID, SessionHelper.CompanyID);
                string response = string.Empty;
                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                response = objCommonDAL.DeleteModulewise(ids, ImportMastersDTO.TableName.GLAccountMaster.ToString(), false, SessionHelper.UserID, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);
                //eTurns.DAL.CacheHelper<IEnumerable<GLAccountMasterDTO>>.InvalidateCache();
                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// // below function used to load the User wise saved filter data for jquery data table
        /// </summary>
        /// <param name="UserID"></param> will have userid for currently logged in user
        /// <returns></returns>
        //public ActionResult LoadGLAccountGridState(int UserID, string ListName)
        //{
        //    UsersUISettingsController obj = new UsersUISettingsController();
        //    UsersUISettingsDTO objDTO = new UsersUISettingsDTO();
        //    objDTO = obj.GetRecord(UserID, ListName);
        //    string jsonData = objDTO.JSONDATA;
        //    return Json(new { jsonData }, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// // below function used to Save the User wise filter data for jquery data table
        /// </summary>
        /// <param name="UserID"></param> will have userid for currently logged in user
        /// <param name="Data"></param> will have json string of filter criteria 
        /// <returns></returns>
        //public ActionResult SaveGLAccountGridState(int UserID, string Data, string ListName)
        //{
        //    UsersUISettingsController obj = new UsersUISettingsController();
        //    UsersUISettingsDTO objDTO = new UsersUISettingsDTO();
        //    objDTO.UserID = UserID;
        //    objDTO.JSONDATA = Data;
        //    objDTO.ListName = ListName;
        //    obj.PutRecord(objDTO);

        //    return Json(new { objDTO.JSONDATA }, JsonRequestBehavior.AllowGet);
        //}

        #endregion

        #region "UnitMaster"

        public ActionResult UnitList()
        {
            return View();
        }

        public PartialViewResult _CreateUnit()
        {
            return PartialView();
        }
        public ViewResult _LicencingTermsandConditions()
        {
            return View();
        }

        public ViewResult TermsAndCondition()
        {
            return View();
        }

        public ViewResult ChangePassword()
        {
            ChangePasswordDTO objChangePasswordDTO = new ChangePasswordDTO();
            objChangePasswordDTO.ID = SessionHelper.UserID;
            return View(objChangePasswordDTO);
        }
        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult UnitCreate(bool isforbom)
        {
            UnitMasterDTO objDTO = new UnitMasterDTO();


            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.RoomName = SessionHelper.RoomName;
            objDTO.CreatedByName = SessionHelper.UserName;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.GUID = Guid.NewGuid();
            objDTO.isForBOM = isforbom;
            objDTO.IsOnlyFromItemUI = true;
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("UnitMaster");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            return PartialView("_CreateUnit", objDTO);
        }

        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>
        public ActionResult UnitEdit(Int64 ID)
        {
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            if (IsDeleted || IsArchived)
            {
                ViewBag.ViewOnly = true;
            }

            //UnitMasterController obj = new UnitMasterController();
            UnitMasterDAL obj = new UnitMasterDAL(SessionHelper.EnterPriseDBName);
            UnitMasterDTO objDTO = obj.GetUnitByIDNormal(ID);
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("UnitMaster");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;
            objDTO.IsOnlyFromItemUI = true;
            return PartialView("_CreateUnit", objDTO);
        }

        /// <summary>
        /// SaveUnitData
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        /// 

        [ValidateAntiForgeryToken]
        public JsonResult SaveUnitData(UnitMasterDTO objDTO)
        {
            string message = "";
            string status = "";
            //UnitMasterController obj = new UnitMasterController();
            UnitMasterDAL obj = new UnitMasterDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.Room = SessionHelper.RoomID;
            Int64 _NewIDForPopUp = 0;
            if (!ModelState.IsValid)
            {
                message = ResMessage.InvalidModel;// "Invalid Data!";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }


            if (objDTO.ID == 0)
            {
                objDTO.CreatedBy = SessionHelper.UserID;
                long RoomId = SessionHelper.RoomID;
                if (objDTO.isForBOM)
                {
                    RoomId = 0;
                }
                string strOK = objCDAL.DuplicateCheck(objDTO.Unit.Trim(), "add", objDTO.ID, "UnitMaster", "Unit", RoomId, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    UnitMasterDTO objUnitMasterDTO = new UnitMasterDTO();
                    objUnitMasterDTO = obj.GetUnitByNamePlain(RoomId, SessionHelper.CompanyID, false, (objDTO.Unit ?? string.Empty).Trim());
                    if (objUnitMasterDTO != null && objDTO.isForBOM == false)
                    {
                        objUnitMasterDTO.isForBOM = false;
                        objUnitMasterDTO.Description = objDTO.Description;
                        objUnitMasterDTO.UDF1 = objDTO.UDF1;
                        if (objDTO.IsOnlyFromItemUI)
                        {
                            objUnitMasterDTO.EditedFrom = "Web";
                            objUnitMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        }
                        if (obj.Edit(objUnitMasterDTO))
                        {
                            message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                            status = "ok";
                            _NewIDForPopUp = objUnitMasterDTO.ID;
                            return Json(new { Message = message, Status = status, NewIDForPopUp = _NewIDForPopUp }, JsonRequestBehavior.AllowGet);
                        }
                    }

                }
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResUnitMaster.Unit, objDTO.Unit);  // "Unit \"" + objDTO.Unit + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.Unit = objDTO.Unit.Trim().ToUpper();
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    long ReturnVal = obj.Insert(objDTO);
                    if (ReturnVal > 0)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                        _NewIDForPopUp = ReturnVal;
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }
            else
            {
                long RoomId = SessionHelper.RoomID;
                if (objDTO.isForBOM)
                {
                    RoomId = 0;
                }
                string strOK = objCDAL.DuplicateCheck(objDTO.Unit, "edit", objDTO.ID, "UnitMaster", "Unit", RoomId, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResUnitMaster.Unit, objDTO.Unit);  // "Unit \"" + objDTO.Unit + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    UnitMasterDTO objTemp = new UnitMasterDTO();
                    objTemp = obj.GetUnitByIDPlain(objDTO.ID);
                    if (objTemp != null && objTemp.ID > 0)
                    {
                        objDTO.AddedFrom = objTemp.AddedFrom;
                        objDTO.ReceivedOnWeb = objTemp.ReceivedOnWeb;
                    }
                    if (objDTO.IsOnlyFromItemUI)
                    {
                        objDTO.EditedFrom = "Web";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    }
                    bool ReturnVal = obj.Edit(objDTO);
                    if (objDTO.isForBOM == true)
                    {
                        BOMItemMasterDAL objBOMItemMasterDAL = new BOMItemMasterDAL(SessionHelper.EnterPriseDBName);
                        objBOMItemMasterDAL.UpdateBOMMasterReference(objDTO.ID, "UnitMaster", SessionHelper.UserID);
                    }
                    if (ReturnVal)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }
            return Json(new { Message = message, Status = status, NewIDForPopUp = _NewIDForPopUp }, JsonRequestBehavior.AllowGet);
            //return Content(message);
        }

        //public string UpdateUnitMasterData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        //{
        //    //UnitMasterController obj = new UnitMasterController();
        //    UnitMasterDAL obj = new UnitMasterDAL(SessionHelper.EnterPriseDBName);
        //    obj.UpdateData(id, value, rowId, columnPosition, columnId, columnName);
        //    return value;
        //}

        //public string DuplicateUnitMasterCheck(string UnitName, string ActionMode, int ID)
        //{
        //    UnitMasterController obj = new UnitMasterController();
        //    return obj.DuplicateCheck(UnitName, ActionMode, ID);
        //}

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public JsonResult DeleteUnitMasterRecords(string ids)
        {
            try
            {
                //eTurns.DAL.CommonDAL _repository = new eTurns.DAL.CommonDAL(SessionHelper.EnterPriseDBName);
                //return _repository.DeleteRecords(ImportMastersDTO.TableName.UnitMaster.ToString(), ids, SessionHelper.RoomID, SessionHelper.CompanyID);

                string response = string.Empty;
                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                response = objCommonDAL.DeleteModulewise(ids, ImportMastersDTO.TableName.UnitMaster.ToString(), false, SessionHelper.UserID, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);
                eTurns.DAL.CacheHelper<IEnumerable<UnitMasterDTO>>.InvalidateCache();
                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        #region Ajax Data Provider

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult GetUnitList(JQueryDataTableParamModel param)
        {
            //UnitMasterController obj = new UnitMasterController();
            UnitMasterDAL obj = new UnitMasterDAL(SessionHelper.EnterPriseDBName);
            bool IsForBom = false;
            bool.TryParse(Convert.ToString(Request["IsForBom"]), out IsForBom);
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

            // set the default column sorting here, if first time then required to set 
            //if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID";
            if (!string.IsNullOrEmpty(sortColumnName))
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
            IEnumerable<UnitMasterDTO> DataFromDB;
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            DataFromDB = obj.GetPagedUnitMaster(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, IsForBom, RoomDateFormat, CurrentTimeZone);





            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);


        }



        public JsonResult GetUnits(int maxRows, string name_startsWith)
        {
            UnitMasterDAL obj = new UnitMasterDAL(SessionHelper.EnterPriseDBName);
            List<UnitMasterDTO> lstUnit = obj.GetUnitByNameSearch(name_startsWith, SessionHelper.RoomID, SessionHelper.CompanyID, false).Take(maxRows).ToList();
            if (lstUnit == null || lstUnit.Count == 0)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            return Json(lstUnit, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// // below function used to load the User wise saved filter data for jquery data table
        /// </summary>
        /// <param name="UserID"></param> will have userid for currently logged in user
        /// <returns></returns>
        //public ActionResult LoadUnitGridState(int UserID, string ListName)
        //{
        //    //string jsonData = @"{""iCreate"":1350639486123,""iStart"":0,""iEnd"":0,""iLength"":10,""aaSorting"":[[0,""asc"",0,""Name""]],""oSearch"":{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true},""aoSearchCols"":[{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true},{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true},{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true},{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}],""abVisCols"":[false,true,true,true,true],""ColReorder"":[0,1,3,2,4]}";
        //    UsersUISettingsController obj = new UsersUISettingsController();
        //    UsersUISettingsDTO objDTO = new UsersUISettingsDTO();
        //    objDTO = obj.GetRecord(UserID, ListName);
        //    string jsonData = objDTO.JSONDATA;
        //    return Json(new { jsonData }, JsonRequestBehavior.AllowGet);
        //}
        /// <summary>
        /// // below function used to Save the User wise filter data for jquery data table
        /// </summary>
        /// <param name="UserID"></param> will have userid for currently logged in user
        /// <param name="Data"></param> will have json string of filter criteria 
        /// <returns></returns>
        //public ActionResult SaveUnitGridState(int UserID, string Data, string ListName)
        //{
        //    UsersUISettingsController obj = new UsersUISettingsController();
        //    UsersUISettingsDTO objDTO = new UsersUISettingsDTO();
        //    objDTO.UserID = UserID;
        //    objDTO.JSONDATA = Data;
        //    objDTO.ListName = ListName;
        //    obj.PutRecord(objDTO);

        //    return Json(new { objDTO.JSONDATA }, JsonRequestBehavior.AllowGet);

        //    //return Json(new {sEcho = 111},JsonRequestBehavior.AllowGet);
        //}



        #endregion

        #endregion

        #region "Tool Category Master"
        /// <summary>
        ///GET ALL: /Master/ ToolCategory
        /// </summary>
        /// <returns></returns>
        public ActionResult ToolCategoryList()
        {
            return View();
        }

        public PartialViewResult _ToolCategoryCreate()
        {
            return PartialView();
        }

        /// <summary>
        ///  GET: /Master/ for Create ToolCategory
        /// </summary>
        /// <returns></returns>
        public ActionResult ToolCategoryCreate()
        {
            ToolCategoryMasterDTO objDTO = new ToolCategoryMasterDTO();
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.CreatedByName = SessionHelper.UserName;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.RoomName = SessionHelper.RoomName;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.IsOnlyFromItemUI = true;
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("ToolCategoryMaster");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            return PartialView("_ToolCategoryCreate", objDTO);
        }

        /// <summary>
        /// JSON Record Save - Enter key Save/Update
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="ToolCategory"></param>
        /// <returns></returns>
        /// 

        [ValidateAntiForgeryToken]
        public JsonResult ToolCategorySave(ToolCategoryMasterDTO objDTO)
        {
            string message = "";
            string status = "";
            //ToolCategoryMasterController obj = new ToolCategoryMasterController();
            ToolCategoryMasterDAL obj = new ToolCategoryMasterDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            objDTO.Room = SessionHelper.RoomID;
            objDTO.RoomName = SessionHelper.RoomName;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;
            if (string.IsNullOrEmpty(objDTO.ToolCategory))
            {
                message = string.Format(ResMessage.Required, ResToolCategory.ToolCategory);// "Tool Category name is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.Room = SessionHelper.RoomID;
            Int64 _NewIDForPopUp = 0;
            if (objDTO.ID == 0)
            {
                objDTO.CreatedBy = SessionHelper.UserID;

                string strOK = objCDAL.DuplicateCheck(objDTO.ToolCategory, "add", objDTO.ID, "ToolCategoryMaster", "ToolCategory", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResToolCategory.ToolCategory, objDTO.ToolCategory);  // "Tool Category \"" + objDTO.ToolCategory + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;

                    long ReturnVal = obj.Insert(objDTO);
                    if (ReturnVal > 0)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                        _NewIDForPopUp = ReturnVal;
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }
            else
            {
                objDTO.LastUpdatedBy = SessionHelper.UserID;
                string strOK = objCDAL.DuplicateCheck(objDTO.ToolCategory, "edit", objDTO.ID, "ToolCategoryMaster", "ToolCategory", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResToolCategory.ToolCategory, objDTO.ToolCategory);  //"Tool Category \"" + objDTO.ToolCategory + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    objDTO.EditedFrom = "Web";
                    objDTO.IsOnlyFromItemUI = true;
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    ToolCategoryMasterDTO objtmp = new ToolCategoryMasterDTO();
                    objtmp = obj.GetToolCategoryByIDPlain(objDTO.ID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (objtmp != null)
                    {
                        objDTO.AddedFrom = objtmp.AddedFrom;
                        objDTO.ReceivedOnWeb = objtmp.ReceivedOnWeb;
                    }
                    bool ReturnVal = obj.Edit(objDTO);
                    if (ReturnVal)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }
            return Json(new { Message = message, Status = status, NewIDForPopUp = _NewIDForPopUp }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>
        public ActionResult ToolCategoryEdit(Int64 ID)
        {
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            if (IsDeleted || IsArchived)
            {
                ViewBag.ViewOnly = true;
            }

            //ToolCategoryMasterController cntrl = new ToolCategoryMasterController();
            ToolCategoryMasterDAL cntrl = new ToolCategoryMasterDAL(SessionHelper.EnterPriseDBName);
            ToolCategoryMasterDTO objDTO = cntrl.GetToolCategoryByIDNormal(ID, SessionHelper.RoomID, SessionHelper.CompanyID);
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("ToolCategoryMaster");
            objDTO.IsOnlyFromItemUI = true;
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;
            return PartialView("_ToolCategoryCreate", objDTO);
        }

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult ToolCategoryListAjax(JQueryDataTableParamModel param)
        {
            //ToolCategoryMasterController obj = new ToolCategoryMasterController();
            ToolCategoryMasterDAL obj = new ToolCategoryMasterDAL(SessionHelper.EnterPriseDBName);
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

            // set the default column sorting here, if first time then required to set 
            //if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID";
            if (!string.IsNullOrEmpty(sortColumnName))
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
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetPagedToolCategoryMaster(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, RoomDateFormat, CurrentTimeZone);


            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);


        }

        ///// <summary>
        ///// Check Duplicate ToolCategory
        ///// </summary>
        ///// <param name="ToolCategoryName"></param>
        ///// <param name="Action"></param>
        ///// <param name="ID"></param>
        ///// <returns></returns>
        //public string DuplicateToolCategoryCheck(string ToolCategoryName, string Action, Int64 ID)
        //{
        //    ToolCategoryMasterController obj = new ToolCategoryMasterController();
        //    return obj.DuplicateCheck(ToolCategoryName, Action, ID);
        //}

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public JsonResult DeleteToolCategoryRecords(string ids)
        {
            try
            {
                //ToolCategoryMasterController obj = new ToolCategoryMasterController();
                //ToolCategoryMasterDAL obj = new ToolCategoryMasterDAL(SessionHelper.EnterPriseDBName);
                //obj.DeleteRecords(ids, SessionHelper.UserID, SessionHelper.CompanyID);
                //return "ok";
                string response = string.Empty;
                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                response = objCommonDAL.DeleteModulewise(ids, ImportMastersDTO.TableName.ToolCategoryMaster.ToString(), false, SessionHelper.UserID, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);
                eTurns.DAL.CacheHelper<IEnumerable<ToolCategoryMasterDTO>>.InvalidateCache();
                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }


        //public string UpdateToolCategoryData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        //{
        //    //ToolCategoryMasterController obj = new ToolCategoryMasterController();
        //    ToolCategoryMasterDAL obj = new ToolCategoryMasterDAL(SessionHelper.EnterPriseDBName);
        //    obj.UpdateData(id, value, rowId, columnPosition, columnId, columnName);
        //    return value;
        //}




        #endregion

        #region "Asset Category Master"
        /// <summary>
        ///GET ALL: /Master/ AssetCategory
        /// </summary>
        /// <returns></returns>
        public ActionResult AssetCategoryList()
        {
            return View();
        }

        public PartialViewResult _AssetCategoryCreate()
        {
            return PartialView();
        }

        /// <summary>
        ///  GET: /Master/ for Create AssetCategory
        /// </summary>
        /// <returns></returns>
        public ActionResult AssetCategoryCreate()
        {
            AssetCategoryMasterDTO objDTO = new AssetCategoryMasterDTO();
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.CreatedByName = SessionHelper.UserName;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.RoomName = SessionHelper.RoomName;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.IsOnlyFromItemUI = true;
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("AssetCategoryMaster");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            return PartialView("_AssetCategoryCreate", objDTO);
        }

        /// <summary>
        /// JSON Record Save - Enter key Save/Update
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="AssetCategory"></param>
        /// <returns></returns>
        /// 

        [ValidateAntiForgeryToken]
        public JsonResult AssetCategorySave(AssetCategoryMasterDTO objDTO)
        {
            string message = "";
            string status = "";
            //AssetCategoryMasterController obj = new AssetCategoryMasterController();
            AssetCategoryMasterDAL obj = new AssetCategoryMasterDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            objDTO.Room = SessionHelper.RoomID;
            objDTO.RoomName = SessionHelper.RoomName;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;

            if (string.IsNullOrEmpty(objDTO.AssetCategory))
            {
                message = string.Format(ResMessage.Required, ResAssetCategory.AssetCategory);// "Asset Category name is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.Room = SessionHelper.RoomID;
            Int64 _NewIDForPopUp = 0;
            if (objDTO.ID == 0)
            {
                objDTO.CreatedBy = SessionHelper.UserID;

                string strOK = objCDAL.DuplicateCheck(objDTO.AssetCategory, "add", objDTO.ID, "AssetCategoryMaster", "AssetCategory", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResAssetCategory.AssetCategory, objDTO.AssetCategory);  // "Asset Category \"" + objDTO.AssetCategory + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    long ReturnVal = obj.Insert(objDTO);
                    if (ReturnVal > 0)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                        _NewIDForPopUp = ReturnVal;
                        eTurns.DAL.CacheHelper<IEnumerable<AssetCategoryMasterDTO>>.InvalidateCache();
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }
            else
            {
                objDTO.LastUpdatedBy = SessionHelper.UserID;
                string strOK = objCDAL.DuplicateCheck(objDTO.AssetCategory, "edit", objDTO.ID, "AssetCategoryMaster", "AssetCategory", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResAssetCategory.AssetCategory, objDTO.AssetCategory);  //"Asset Category \"" + objDTO.AssetCategory + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    if (objDTO.IsOnlyFromItemUI)
                    {
                        objDTO.EditedFrom = "Web";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    }
                    AssetCategoryMasterDTO objtemp = new AssetCategoryMasterDTO();
                    objtemp = obj.GetAssetCategoryByIdOrGUID(objDTO.ID, null, false, false);
                    if (objtemp != null)
                    {
                        objDTO.AddedFrom = objtemp.AddedFrom;
                        objDTO.ReceivedOnWeb = objtemp.ReceivedOnWeb;
                    }
                    bool ReturnVal = obj.Edit(objDTO);
                    if (ReturnVal)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }
            return Json(new { Message = message, Status = status, NewIDForPopUp = _NewIDForPopUp }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>
        public ActionResult AssetCategoryEdit(Int64 ID)
        {
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            if (IsDeleted || IsArchived)
            {
                ViewBag.ViewOnly = true;
            }

            //AssetCategoryMasterController cntrl = new AssetCategoryMasterController();
            AssetCategoryMasterDAL cntrl = new AssetCategoryMasterDAL(SessionHelper.EnterPriseDBName);
            AssetCategoryMasterDTO objDTO = cntrl.GetAssetCategoryByIdOrGUID(ID, null, IsDeleted, IsArchived);
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("AssetCategoryMaster");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;
            if (objDTO != null)
            {
                objDTO.CreatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                objDTO.UpdatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                objDTO.IsOnlyFromItemUI = true;
            }
            return PartialView("_AssetCategoryCreate", objDTO);
        }

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult AssetCategoryListAjax(JQueryDataTableParamModel param)
        {
            //AssetCategoryMasterController obj = new AssetCategoryMasterController();
            AssetCategoryMasterDAL obj = new AssetCategoryMasterDAL(SessionHelper.EnterPriseDBName);
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

            // set the default column sorting here, if first time then required to set 
            //if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID";


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
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = SessionHelper.CurrentTimeZone;
            //var DataFromDB = obj.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted);
            var DataFromDB = obj.GetPagedAssetCategory(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, RoomDateFormat, CurrentTimeZone);

            var result = from c in DataFromDB
                         select new AssetCategoryMasterDTO
                         {
                             ID = c.ID,
                             AssetCategory = c.AssetCategory,
                             RoomName = c.RoomName,
                             Created = c.Created,
                             CreatedBy = c.CreatedBy,
                             Updated = c.Updated,
                             LastUpdatedBy = c.LastUpdatedBy,
                             UpdatedByName = c.UpdatedByName,
                             Room = c.Room,
                             CreatedByName = c.CreatedByName,
                             IsArchived = c.IsArchived,
                             IsDeleted = c.IsDeleted,
                             UDF1 = c.UDF1,
                             UDF2 = c.UDF2,
                             UDF3 = c.UDF3,
                             UDF4 = c.UDF4,
                             UDF5 = c.UDF5,
                             CreatedDate = FnCommon.ConvertDateByTimeZone(c.Created, true),
                             UpdatedDate = FnCommon.ConvertDateByTimeZone(c.Updated, true),
                             AddedFrom = (c.AddedFrom == null ? "Web" : c.AddedFrom),
                             EditedFrom = (c.EditedFrom == null ? "Web" : c.EditedFrom),
                             ReceivedOn = c.ReceivedOn,
                             ReceivedOnWeb = c.ReceivedOnWeb

                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);


        }

        ///// <summary>
        ///// Check Duplicate AssetCategory
        ///// </summary>
        ///// <param name="AssetCategoryName"></param>
        ///// <param name="Action"></param>
        ///// <param name="ID"></param>
        ///// <returns></returns>
        //public string DuplicateAssetCategoryCheck(string AssetCategoryName, string Action, Int64 ID)
        //{
        //    AssetCategoryMasterController obj = new AssetCategoryMasterController();
        //    return obj.DuplicateCheck(AssetCategoryName, Action, ID);
        //}

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public JsonResult DeleteAssetCategoryRecords(string ids)
        {
            try
            {
                //AssetCategoryMasterController obj = new AssetCategoryMasterController();
                //AssetCategoryMasterDAL obj = new AssetCategoryMasterDAL(SessionHelper.EnterPriseDBName);
                //obj.DeleteRecords(ids, SessionHelper.UserID, SessionHelper.CompanyID);
                //return "ok";
                string response = string.Empty;
                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                response = objCommonDAL.DeleteModulewise(ids, ImportMastersDTO.TableName.AssetCategoryMaster.ToString(), false, SessionHelper.UserID, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);
                eTurns.DAL.CacheHelper<IEnumerable<AssetCategoryMasterDTO>>.InvalidateCache();
                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }


        public string UpdateAssetCategoryData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            //AssetCategoryMasterController obj = new AssetCategoryMasterController();
            AssetCategoryMasterDAL obj = new AssetCategoryMasterDAL(SessionHelper.EnterPriseDBName);
            obj.UpdateData(id, value, rowId, columnPosition, columnId, columnName);
            return value;
        }




        #endregion

        #region "ShipVia Master"

        /// <summary>
        /// JSON Record Save - Enter key Save/Update
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="ShipVia"></param>
        /// <returns></returns>
        /// 

        [ValidateAntiForgeryToken]
        public JsonResult ShipViaSave(ShipViaDTO objDTO)
        {
            string message = "";

            //ShipViaController obj = new ShipViaController();
            ShipViaDAL obj = new ShipViaDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.Room = SessionHelper.RoomID;
            Int64 _NewIDForPopUp = 0;
            if (objDTO.ID == 0)
            {
                objDTO.CreatedBy = SessionHelper.UserID;
                string strOK = objCDAL.DuplicateCheck(objDTO.ShipVia, "add", objDTO.ID, "ShipViaMaster", "ShipVia", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK != "ok")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResShipVia.ShipVia, objDTO.ShipVia);  //"ShipVia \"" + objDTO.ShipVia + "\" already exist! Try with Another!";
                }
                else
                {
                    objDTO.GUID = Guid.NewGuid();
                    long ReturnVal = obj.Insert(objDTO);
                    if (ReturnVal > 0)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        _NewIDForPopUp = ReturnVal;
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                    }
                }
            }
            else
            {
                objDTO.LastUpdatedBy = SessionHelper.UserID;
                string strOK = objCDAL.DuplicateCheck(objDTO.ShipVia, "edit", objDTO.ID, "ShipViaMaster", "ShipVia", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK != "ok")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResShipVia.ShipVia, objDTO.ShipVia);  //"ShipVia \"" + objDTO.ShipVia + "\" already exist! Try with Another!";
                }
                else
                {
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.EditedFrom = "Web";
                    bool ReturnVal = obj.Edit(objDTO);
                    if (ReturnVal)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                    }
                }
            }


            return Json(new { Message = message, NewIDForPopUp = _NewIDForPopUp }, JsonRequestBehavior.AllowGet);
        }

        ///// <summary>
        ///// Check Duplicate ShipVia
        ///// </summary>
        ///// <param name="ShipViaName"></param>
        ///// <param name="Action"></param>
        ///// <param name="ID"></param>
        ///// <returns></returns>
        //public string DuplicateShipViaCheck(string ShipViaName, string Action, Int64 ID)
        //{
        //    ShipViaController obj = new ShipViaController();
        //    return obj.DuplicateCheck(ShipViaName, Action, ID);
        //}

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public JsonResult DeleteShipViaRecords(string ids)
        {
            try
            {
                //eTurns.DAL.CommonDAL _repository = new eTurns.DAL.CommonDAL(SessionHelper.EnterPriseDBName);
                //return _repository.DeleteRecords(ImportMastersDTO.TableName.ShipViaMaster.ToString(), ids, SessionHelper.RoomID, SessionHelper.CompanyID);

                string response = string.Empty;
                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                response = objCommonDAL.DeleteModulewise(ids, ImportMastersDTO.TableName.ShipViaMaster.ToString(), false, SessionHelper.UserID, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);
                eTurns.DAL.CacheHelper<IEnumerable<ShipViaDTO>>.InvalidateCache();
                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult ShipViaListAjax(JQueryDataTableParamModel param)
        {
            //ShipViaController obj = new ShipViaController();
            ShipViaDAL obj = new ShipViaDAL(SessionHelper.EnterPriseDBName);

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

            // set the default column sorting here, if first time then required to set 
            if (sortColumnName == "0" || sortColumnName == "undefined")
                sortColumnName = "ID";
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
                sortColumnName = "ID desc";



            string searchQuery = string.Empty;
            if (param.sSearch != null && param.sSearch != "")
            {
                searchQuery = "WHERE ShipVia like '%" + param.sSearch + "%'" + @"
                    OR RoomName like '%" + param.sSearch + "%'" + @" 
                    OR CreatedBy like '%" + param.sSearch + "%'";
            }

            int TotalRecordCount = 0;
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetPagedShipViaMaster(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, RoomDateFormat, CurrentTimeZone);


            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);


        }

        /// <summary>
        ///GET ALL: /Master/ 
        /// </summary>
        /// <returns></returns>
        public ActionResult ShipViaList()
        {
            return View();
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult ShipViaCreate()
        {
            ShipViaDTO objDTO = new ShipViaDTO()
            {
                Created = DateTimeUtility.DateTimeNow,
                Updated = DateTimeUtility.DateTimeNow,
                CreatedBy = SessionHelper.UserID,
                CreatedByName = SessionHelper.UserName,
                LastUpdatedBy = SessionHelper.UserID,
                Room = SessionHelper.RoomID,
                CompanyID = SessionHelper.CompanyID,
                RoomName = SessionHelper.RoomName,
                UpdatedByName = SessionHelper.UserName,
                GUID = Guid.NewGuid()
            };
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("ShipViaMaster");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            return PartialView("_CreateShipVia", objDTO);
        }

        public PartialViewResult _ShipViaCreate()
        {
            return PartialView();
        }

        //public string UpdateShipViaData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        //{
        //    //ShipViaController obj = new ShipViaController();
        //    ShipViaDAL obj = new ShipViaDAL(SessionHelper.EnterPriseDBName);
        //    obj.UpdateData(id, value, rowId, columnPosition, columnId, columnName);
        //    return value;
        //}

        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>
        public ActionResult ShipViaEdit(Int64 ID)
        {
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            if (IsDeleted || IsArchived)
            {
                ViewBag.ViewOnly = true;
            }

            //ShipViaController cntrl = new ShipViaController();
            ShipViaDAL cntrl = new ShipViaDAL(SessionHelper.EnterPriseDBName);
            ShipViaDTO objDTO = cntrl.GetShipViaByIDNormal(ID, SessionHelper.RoomID, SessionHelper.CompanyID);
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("ShipViaMaster");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;
            return PartialView("_CreateShipVia", objDTO);
        }


        #endregion

        #region "Measurement Term Master"

        /// <summary>
        ///GET ALL: /Master/ 
        /// </summary>
        /// <returns></returns>
        public ActionResult MeasurementTermList()
        {
            return View();
        }

        /// <summary>
        /// _MeasurementTermCreate
        /// </summary>
        /// <returns></returns>
        public PartialViewResult _MeasurementTermCreate()
        {
            return PartialView();
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult MeasurementTermCreate()
        {
            MeasurementTermMasterDTO objDTO = new MeasurementTermMasterDTO()
            {
                Created = DateTimeUtility.DateTimeNow,
                Updated = DateTimeUtility.DateTimeNow,
                CreatedBy = SessionHelper.UserID,
                CreatedByName = SessionHelper.UserName,
                LastUpdatedBy = SessionHelper.UserID,
                Room = SessionHelper.RoomID,
                CompanyID = SessionHelper.CompanyID,
                RoomName = SessionHelper.RoomName,
                UpdatedByName = SessionHelper.UserName,
            };
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("MeasurementTerm");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            return PartialView("_CreateMeasurementTerm", objDTO);
        }

        [HttpPost]
        public JsonResult MeasurementTermSave(MeasurementTermMasterDTO objDTO)
        {
            string message = "";
            string status = "";
            //MeasurementTermController obj = new MeasurementTermController();
            MeasurementTermDAL obj = new MeasurementTermDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

            if (string.IsNullOrEmpty(objDTO.MeasurementTerm))
            {
                message = string.Format(ResMessage.Required, ResMeasurementTerm.MeasurementTerm); // "MeasurementTerm name is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.Room = SessionHelper.RoomID;
            if (objDTO.ID == 0)
            {
                objDTO.CreatedBy = SessionHelper.UserID;

                string strOK = objCDAL.DuplicateCheck(objDTO.MeasurementTerm, "add", objDTO.ID, "MeasurementTermMaster", "MeasurementTerm", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResMeasurementTerm.MeasurementTerm, objDTO.MeasurementTerm);  //"MeasurementTerm \"" + objDTO.MeasurementTerm + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    objDTO.GUID = Guid.NewGuid();
                    long ReturnVal = obj.Insert(objDTO);
                    if (ReturnVal > 0)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }
            else
            {

                string strOK = objCDAL.DuplicateCheck(objDTO.MeasurementTerm, "edit", objDTO.ID, "MeasurementTermMaster", "MeasurementTerm", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResMeasurementTerm.MeasurementTerm, objDTO.MeasurementTerm);  //"MeasurementTerm \"" + objDTO.MeasurementTerm + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    bool ReturnVal = obj.Edit(objDTO);
                    if (ReturnVal)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }

            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        ///// <summary>
        ///// Check Duplicate MeasurementTerm
        ///// </summary>
        ///// <param name="MeasurementTermName"></param>
        ///// <param name="Action"></param>
        ///// <param name="ID"></param>
        ///// <returns></returns>
        //public string DuplicateMeasurementTermCheck(string MeasurementTermName, string Action, Int64 ID)
        //{
        //    MeasurementTermController obj = new MeasurementTermController();
        //    return obj.DuplicateCheck(MeasurementTermName, Action, ID);
        //}

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public JsonResult DeleteMeasurementTermRecords(string ids)
        {
            try
            {
                //eTurns.DAL.CommonDAL _repository = new eTurns.DAL.CommonDAL(SessionHelper.EnterPriseDBName);
                //return _repository.DeleteRecords(ImportMastersDTO.TableName.MeasurementTermMaster.ToString(), ids, SessionHelper.RoomID, SessionHelper.CompanyID);

                string response = string.Empty;
                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                response = objCommonDAL.DeleteModulewise(ids, ImportMastersDTO.TableName.MeasurementTermMaster.ToString(), false, SessionHelper.UserID, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);
                eTurns.DAL.CacheHelper<IEnumerable<MeasurementTermMasterDTO>>.InvalidateCache();
                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// // below function used to Save the User wise filter data for jquery data table
        /// </summary>
        /// <param name="UserID"></param> will have userid for currently logged in user
        /// <param name="Data"></param> will have json string of filter criteria 
        /// <returns></returns>
        //public ActionResult SaveMeasurementTermGridState(int UserID, string Data, string ListName)
        //{
        //    UsersUISettingsController obj = new UsersUISettingsController();
        //    UsersUISettingsDTO objDTO = new UsersUISettingsDTO();
        //    objDTO.UserID = UserID;
        //    objDTO.JSONDATA = Data;
        //    objDTO.ListName = ListName;
        //    obj.PutRecord(objDTO);

        //    return Json(new { objDTO.JSONDATA }, JsonRequestBehavior.AllowGet);

        //    //return Json(new {sEcho = 111},JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// // below function used to load the User wise saved filter data for jquery data table
        /// </summary>
        /// <param name="UserID"></param> will have userid for currently logged in user
        /// <returns></returns>
        //public ActionResult LoadMeasurementTermGridState(int UserID, string ListName)
        //{
        //    //string jsonData = @"{""iCreate"":1350639486123,""iStart"":0,""iEnd"":0,""iLength"":10,""aaSorting"":[[0,""asc"",0,""Name""]],""oSearch"":{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true},""aoSearchCols"":[{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true},{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true},{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true},{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}],""abVisCols"":[false,true,true,true,true],""ColReorder"":[0,1,3,2,4]}";
        //    UsersUISettingsController obj = new UsersUISettingsController();
        //    UsersUISettingsDTO objDTO = new UsersUISettingsDTO();
        //    objDTO = obj.GetRecord(UserID, ListName);
        //    string jsonData = objDTO.JSONDATA;
        //    return Json(new { jsonData }, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult GetMeasurementTermList(JQueryDataTableParamModel param)
        {
            //MeasurementTermController obj = new MeasurementTermController();
            MeasurementTermDAL obj = new MeasurementTermDAL(SessionHelper.EnterPriseDBName);

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
            if (sortColumnName == "0" || sortColumnName == "undefined")
                sortColumnName = "ID";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, RoomDateFormat, CurrentTimeZone);

            var result = from c in DataFromDB
                         select new MeasurementTermMasterDTO
                         {
                             ID = c.ID,
                             MeasurementTerm = c.MeasurementTerm,
                             RoomName = c.RoomName,
                             Created = c.Created,
                             Updated = c.Updated,
                             UpdatedByName = c.UpdatedByName,
                             CreatedByName = c.CreatedByName,
                             IsArchived = c.IsArchived,
                             IsDeleted = c.IsDeleted,
                             UDF1 = c.UDF1,
                             UDF2 = c.UDF2,
                             UDF3 = c.UDF3,
                             UDF4 = c.UDF4,
                             UDF5 = c.UDF5
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);




        }

        /// <summary>
        /// UpdateMeasurementTermData
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <param name="rowId"></param>
        /// <param name="columnPosition"></param>
        /// <param name="columnId"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string UpdateMeasurementTermData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            //MeasurementTermController obj = new MeasurementTermController();
            MeasurementTermDAL obj = new MeasurementTermDAL(SessionHelper.EnterPriseDBName);
            obj.UpdateData(id, value, rowId, columnPosition, columnId, columnName);
            return value;
        }

        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>
        public ActionResult MeasurementTermEdit(Int64 ID)
        {
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            if (IsDeleted || IsArchived)
            {
                ViewBag.ViewOnly = true;
            }

            //MeasurementTermController cntrl = new MeasurementTermController();
            MeasurementTermDAL cntrl = new MeasurementTermDAL(SessionHelper.EnterPriseDBName);
            MeasurementTermMasterDTO objDTO = cntrl.GetRecord(ID, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted);
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("MeasurementTerm");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;
            return PartialView("_CreateMeasurementTerm", objDTO);
        }

        #endregion

        #region "Customer Master"

        /// <summary>
        /// JSON Record Save - Enter key Save/Update
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="CustomerMaster"></param>
        /// <returns></returns>
        /// 

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult CustomerMasterSave(CustomerMasterDTO objDTO)
        {
            string message = "";
            string status = "";
            //CustomerMasterController obj = new CustomerMasterController();
            CustomerMasterDAL obj = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.Room = SessionHelper.RoomID;
            //Int64 _NewIDForPopUp = 0;
            Guid _NewIDForPopUp = Guid.Empty;
            if (objDTO.ID == 0)
            {
                objDTO.CreatedBy = SessionHelper.UserID;
                objDTO.Created = DateTimeUtility.DateTimeNow;
                string strOK = objCDAL.DuplicateCheck(objDTO.Customer, "add", objDTO.ID, "CustomerMaster", "Customer", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResCustomer.Customer, objDTO.Customer);  //"Customer Name \"" + objDTO.Customer + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    objDTO.GUID = Guid.NewGuid();
                    if (objDTO != null)
                    {
                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    }
                    long ReturnVal = obj.Insert(objDTO);

                    #region Insert country if not exist
                    if (objDTO != null && !string.IsNullOrWhiteSpace(objDTO.Country))
                    {
                        CountryMasterDAL objCountryMasterDAL = new CountryMasterDAL(SessionHelper.EnterPriseDBName);
                        CountryMasterDTO objCountryMasterDTO = new CountryMasterDTO();
                        objCountryMasterDTO.CountryName = objDTO.Country;
                        objCountryMasterDTO.CreatedBy = SessionHelper.UserID;
                        objCountryMasterDAL.InsertCountry(objCountryMasterDTO);
                    }
                    #endregion

                    if (ReturnVal > 0)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                        _NewIDForPopUp = objDTO.GUID;
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }
            else
            {
                string strOK = objCDAL.DuplicateCheck(objDTO.Customer, "edit", objDTO.ID, "CustomerMaster", "Customer", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResCustomer.Customer, objDTO.Customer);  //"Customer Name \"" + objDTO.Customer + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    CustomerMasterDTO tempdto = new CustomerMasterDTO();
                    tempdto = obj.GetCustomerByID(objDTO.ID);
                    if (tempdto != null)
                    {
                        objDTO.AddedFrom = tempdto.AddedFrom;
                        objDTO.ReceivedOnWeb = tempdto.ReceivedOnWeb;
                    }
                    if (objDTO.IsOnlyFromItemUI)
                    {
                        objDTO.EditedFrom = "Web";
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    }
                    bool ReturnVal = obj.Edit(objDTO);

                    #region Insert country if not exist
                    if (objDTO != null && !string.IsNullOrWhiteSpace(objDTO.Country))
                    {
                        CountryMasterDAL objCountryMasterDAL = new CountryMasterDAL(SessionHelper.EnterPriseDBName);
                        CountryMasterDTO objCountryMasterDTO = new CountryMasterDTO();
                        objCountryMasterDTO.CountryName = objDTO.Country;
                        objCountryMasterDTO.CreatedBy = SessionHelper.UserID;
                        objCountryMasterDAL.InsertCountry(objCountryMasterDTO);
                    }
                    #endregion
                    if (ReturnVal)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }
            return Json(new { Message = message, Status = status, NewIDForPopUp = _NewIDForPopUp }, JsonRequestBehavior.AllowGet);
        }

        ///// <summary>
        ///// Check Duplicate CustomerMaster
        ///// </summary>
        ///// <param name="CustomerMasterName"></param>
        ///// <param name="Action"></param>
        ///// <param name="ID"></param>
        ///// <returns></returns>
        //public string DuplicateCustomerMasterCheck(string CustomerName, string Action, Int64 ID)
        //{
        //    CustomerMasterController obj = new CustomerMasterController();
        //    return obj.DuplicateCheck(CustomerName, Action, ID);
        //}

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public JsonResult DeleteCustomerMasterRecords(string ids)
        {
            try
            {
                //eTurns.DAL.CommonDAL _repository = new eTurns.DAL.CommonDAL(SessionHelper.EnterPriseDBName);
                //return _repository.DeleteRecords(ImportMastersDTO.TableName.CustomerMaster.ToString(), ids, SessionHelper.RoomID, SessionHelper.CompanyID);

                string response = string.Empty;
                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                response = objCommonDAL.DeleteModulewise(ids, ImportMastersDTO.TableName.CustomerMaster.ToString(), false, SessionHelper.UserID, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);
                eTurns.DAL.CacheHelper<IEnumerable<CustomerMasterDTO>>.InvalidateCache();
                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult CustomerMasterListAjax(JQueryDataTableParamModel param)
        {
            //CustomerMasterController obj = new CustomerMasterController();
            CustomerMasterDAL obj = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);

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

            if (!string.IsNullOrEmpty(sortColumnName))
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
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            //var DataFromDB = obj.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted);
            var DataFromDB = obj.GetPagedCustomerMaster(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, SessionHelper.RoomDateFormat, CurrentTimeZone);
            //var result = from c in DataFromDB
            //             select new CustomerMasterDTO
            //             {
            //                 ID = c.ID,
            //                 Customer = c.Customer,
            //                 Account = c.Account,
            //                 Address = c.Address,
            //                 City = c.City,
            //                 State = c.State,
            //                 Country = c.Country,
            //                 ZipCode = c.ZipCode,
            //                 Contact = c.Contact,
            //                 Email = c.Email,
            //                 Phone = c.Phone,
            //                 RoomName = c.RoomName,
            //                 Created = c.Created,
            //                 CreatedBy = c.CreatedBy,
            //                 Updated = c.Updated,
            //                 LastUpdatedBy = c.LastUpdatedBy,
            //                 UpdatedByName = c.UpdatedByName,
            //                 Room = c.Room,
            //                 CreatedByName = c.CreatedByName,
            //                 IsDeleted = c.IsDeleted,
            //                 IsArchived = c.IsArchived,
            //                 UDF1 = c.UDF1,
            //                 UDF2 = c.UDF2,
            //                 UDF3 = c.UDF3,
            //                 UDF4 = c.UDF4,
            //                 UDF5 = c.UDF5,
            //                 Remarks = c.Remarks,
            //                 CreatedDate = FnCommon.ConvertDateByTimeZone(c.Created, false, true),//CommonUtility.ConvertDateByTimeZone(c.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
            //                 UpdatedDate = FnCommon.ConvertDateByTimeZone(c.Updated, false, true),//CommonUtility.ConvertDateByTimeZone(c.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
            //                 ReceivedOn = c.ReceivedOn,
            //                 ReceivedOnWeb = c.ReceivedOnWeb,
            //                 AddedFrom = c.AddedFrom,
            //                 EditedFrom = c.EditedFrom,
            //                 ReceivedOnDate = FnCommon.ConvertDateByTimeZone(c.ReceivedOn, false, true),//CommonUtility.ConvertDateByTimeZone(c.ReceivedOn, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
            //                 ReceivedOnDateWeb = FnCommon.ConvertDateByTimeZone(c.ReceivedOnWeb, false, true),//CommonUtility.ConvertDateByTimeZone(c.ReceivedOnWeb, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true),
            //             };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);


        }

        /// <summary>
        ///GET ALL: /Master/ 
        /// </summary>
        /// <returns></returns>
        public ActionResult CustomerMasterList()
        {
            return View();
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult CustomerMasterCreate()
        {
            CustomerMasterDTO objDTO = new CustomerMasterDTO()
            {
                Created = DateTimeUtility.DateTimeNow,
                Updated = DateTimeUtility.DateTimeNow,
                CreatedBy = SessionHelper.UserID,
                CreatedByName = SessionHelper.UserName,
                LastUpdatedBy = SessionHelper.UserID,
                Room = SessionHelper.RoomID,
                CompanyID = SessionHelper.CompanyID,
                RoomName = SessionHelper.RoomName,
                UpdatedByName = SessionHelper.UserName,
                GUID = Guid.NewGuid(),
                IsOnlyFromItemUI = true
            };
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("CustomerMaster");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            return PartialView("_CreateCustomerMaster", objDTO);
        }

        public PartialViewResult _CustomerMasterCreate()
        {
            return PartialView();
        }

        //public string UpdateCustomerMasterData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        //{
        //    //CustomerMasterController obj = new CustomerMasterController();
        //    CustomerMasterDAL obj = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
        //    obj.UpdateData(id, value, rowId, columnPosition, columnId, columnName);
        //    return value;
        //}

        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>
        public ActionResult CustomerMasterEdit(Int64 ID)
        {
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            if (IsDeleted || IsArchived)
            {
                ViewBag.ViewOnly = true;
            }

            //CustomerMasterController cntrl = new CustomerMasterController();
            CustomerMasterDAL cntrl = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
            var data = cntrl.GetCustomerByID(ID);
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("CustomerMaster");
            ViewData["UDF1"] = data.UDF1;
            ViewData["UDF2"] = data.UDF2;
            ViewData["UDF3"] = data.UDF3;
            ViewData["UDF4"] = data.UDF4;
            ViewData["UDF5"] = data.UDF5;
            data.IsOnlyFromItemUI = true;
            if (data != null)
            {
                data.CreatedDate = CommonUtility.ConvertDateByTimeZone(data.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                data.UpdatedDate = CommonUtility.ConvertDateByTimeZone(data.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
            }

            return PartialView("_CreateCustomerMaster", data);
        }


        #endregion

        #region "Tool Master"

        public ActionResult ToolList()
        {
            return View();
        }

        public PartialViewResult _CreateTool()
        {
            return PartialView();
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult ToolCreate()
        {
            ToolMasterDTO objDTO = new ToolMasterDTO();
            objDTO.ID = 0;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.RoomName = SessionHelper.RoomName;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.GUID = Guid.NewGuid();

            //Dropdown list
            //ToolCategoryMasterController objToolCategory = new ToolCategoryMasterController();
            //ViewBag.ToolCategoryList = new SelectList(objToolCategory.GetAllRecords().ToList(), "ID", "ToolCategory");            

            //ToolCategoryMasterController objToolCategory = new ToolCategoryMasterController();
            ToolCategoryMasterDAL objToolCategory = new ToolCategoryMasterDAL(SessionHelper.EnterPriseDBName);
            List<ToolCategoryMasterDTO> lstToolCategory = objToolCategory.GetToolCategoryByRoomIDPlain(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            lstToolCategory.Insert(0, new ToolCategoryMasterDTO() { ID = 0, ToolCategory = ResCategoryMaster.SelectCategory });
            ViewBag.ToolCategoryList = lstToolCategory;

            //LocationMasterController objLocationCntrl = new LocationMasterController();
            LocationMasterDAL objLocationCntrl = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
            List<LocationMasterDTO> lstLocation = objLocationCntrl.GetLocationByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            lstLocation.Insert(0, new LocationMasterDTO() { ID = 0, Location = ResCommon.MsgSelectLocation });
            ViewBag.LocationList = lstLocation;
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("ToolMaster");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            return PartialView("_CreateTool", objDTO);
        }


        [HttpPost]
        public JsonResult ToolSave(ToolMasterDTO objDTO)
        {
            string message = "";
            string status = "";
            //ToolMasterController obj = new ToolMasterController();
            ToolMasterDAL obj = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

            if (string.IsNullOrEmpty(objDTO.ToolName))
            {
                message = string.Format(ResMessage.Required, ResToolMaster.ToolName);// "Tool is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.Room = SessionHelper.RoomID;
            if (objDTO.ID == 0)
            {
                objDTO.CreatedBy = SessionHelper.UserID;
                string strOK = objCDAL.DuplicateCheck(objDTO.ToolName, "add", objDTO.ID, "ToolMaster", "ToolName", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResToolMaster.ToolName, objDTO.ToolName);  //"Tool \"" + objDTO.ToolName + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.AddedFrom = "Web";
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.EditedFrom = "Web";
                    long ReturnVal = obj.Insert(objDTO);
                    if (ReturnVal > 0)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }
            else
            {
                objDTO.LastUpdatedBy = SessionHelper.UserID;
                string strOK = objCDAL.DuplicateCheck(objDTO.ToolName, "edit", objDTO.ID, "ToolMaster", "ToolName", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResToolMaster.ToolName, objDTO.ToolName);  //"Tool \"" + objDTO.ToolName + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {

                    if (objDTO.IsOnlyFromItemUI)
                    {
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.EditedFrom = "Web";
                    }
                    ToolMasterDTO objtempDto = new ToolMasterDTO();
                    objtempDto = obj.GetToolByIDPlain(objDTO.ID);
                    if (objtempDto != null)
                    {
                        objDTO.AddedFrom = objtempDto.AddedFrom;
                        objDTO.ReceivedOnWeb = objtempDto.ReceivedOnWeb;
                    }
                    bool ReturnVal = obj.Edit(objDTO);
                    if (ReturnVal)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }

            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>
        public ActionResult ToolEdit(Int64 ID)
        {
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            if (IsDeleted || IsArchived)
            {
                ViewBag.ViewOnly = true;
            }

            //ToolMasterController obj = new ToolMasterController();
            ToolMasterDAL obj = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
            ToolMasterDTO objDTO = obj.GetToolByIDNormal(ID);

            //Dropdown list
            //ToolCategoryMasterController objToolCategory = new ToolCategoryMasterController();
            ToolCategoryMasterDAL objToolCategory = new ToolCategoryMasterDAL(SessionHelper.EnterPriseDBName);
            List<ToolCategoryMasterDTO> lstToolCategory = objToolCategory.GetToolCategoryByRoomIDPlain(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            lstToolCategory.Insert(0, new ToolCategoryMasterDTO() { ID = 0, ToolCategory = ResCategoryMaster.SelectCategory });
            ViewBag.ToolCategoryList = lstToolCategory;

            //LocationMasterController objLocationCntrl = new LocationMasterController();
            LocationMasterDAL objLocationCntrl = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
            List<LocationMasterDTO> lstLocation = objLocationCntrl.GetLocationByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
            lstLocation.Insert(0, new LocationMasterDTO() { ID = 0, Location = ResCommon.MsgSelectLocation });
            ViewBag.LocationList = lstLocation;
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("ToolMaster");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;

            return PartialView("_CreateTool", objDTO);
        }

        #region Ajax Data Provider

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        //public ActionResult ToolListAjax(JQueryDataTableParamModel param)
        //{
        //    //ToolMasterController obj = new ToolMasterController();
        //    ToolMasterDAL obj = new ToolMasterDAL(SessionHelper.EnterPriseDBName);

        //    //LoadTestEntities entity = new LoadTestEntities();
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
        //    var DataFromDB = obj.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, Convert.ToString(SessionHelper.RoomDateFormat));

        //    var result = from c in DataFromDB
        //                 select new ToolMasterDTO
        //                 {
        //                     ID = c.ID,
        //                     ToolName = c.ToolName,
        //                     Serial = c.Serial,
        //                     Description = c.Description,
        //                     Cost = c.Cost,
        //                     IsCheckedOut = c.IsCheckedOut,
        //                     ToolCategoryID = c.ToolCategoryID,
        //                     ToolCategory = c.ToolCategory,
        //                     RoomName = c.RoomName,
        //                     Created = c.Created,
        //                     Updated = c.Updated,
        //                     UpdatedByName = c.UpdatedByName,
        //                     CreatedByName = c.CreatedByName,
        //                     Location = c.Location,
        //                     IsDeleted = c.IsDeleted,
        //                     IsArchived = c.IsArchived,
        //                     UDF1 = c.UDF1,
        //                     UDF2 = c.UDF2,
        //                     UDF3 = c.UDF3,
        //                     UDF4 = c.UDF4,
        //                     UDF5 = c.UDF5
        //                 };
        //    return Json(new
        //    {
        //        sEcho = param.sEcho,
        //        iTotalRecords = TotalRecordCount,
        //        iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
        //        aaData = result
        //    },
        //                JsonRequestBehavior.AllowGet);


        //}

        //public string UpdateToolData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        //{
        //    //ToolMasterController obj = new ToolMasterController();
        //    ToolMasterDAL obj = new ToolMasterDAL(SessionHelper.EnterPriseDBName);
        //    obj.UpdateData(id, value, rowId, columnPosition, columnId, columnName);
        //    return value;
        //}

        //public string DuplicateToolCheck(string ToolName, string ActionMode, int ID)
        //{
        //    ToolMasterController obj = new ToolMasterController();
        //    return obj.DuplicateCheck(ToolName, ActionMode, ID);
        //}

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        //public string DeleteToolRecords(string ids)
        //{
        //    try
        //    {

        //        ToolMasterController obj = new ToolMasterController();
        //        obj.DeleteRecords(ids, SessionHelper.UserID, SessionHelper.CompanyID);
        //        return "ok";

        //        //return "not ok";
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //}

        #endregion

        #endregion

        #region "GlobalUISettings"

        public Int32 GlobalUISettingsForGrid(Int64 ID, string SearchType)
        {
            try
            {
                //CompanyConfigDAL obj = new CompanyConfigDAL(SessionHelper.EnterPriseDBName);
                //CompanyConfigDTO objDTO = obj.GetRecord(SessionHelper.CompanyID);
                RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
                if (SessionHelper.eTurnsRegionInfoProp != null)
                {
                    return (Int32)SessionHelper.eTurnsRegionInfoProp.GridRefreshTimeInSecond.GetValueOrDefault(0);
                }

                //if (objDTO != null && objDTO.GridRefreshTimeInSecond.GetValueOrDefault(0) > 1000)
                //{
                //    return (Int32)objDTO.GridRefreshTimeInSecond.GetValueOrDefault(0);
                //}
                else
                {
                    return 30000; // this should be read from config
                }
            }
            catch (Exception)
            {
                //return 0;
                return 30000; // return default miliseconds, this should be read from config
            }
        }
        #endregion

        #region "Location Master"
        /// <summary>
        ///GET ALL: /Master/ 
        /// </summary>
        /// <returns></returns>
        public ActionResult LocationList()
        {
            return View();
        }

        public PartialViewResult _CreateLocation()
        {
            return PartialView();
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult LocationCreate()
        {
            LocationMasterDTO objDTO = new LocationMasterDTO()
            {
                Created = DateTimeUtility.DateTimeNow,
                LastUpdated = DateTimeUtility.DateTimeNow,
                CreatedBy = SessionHelper.UserID,
                CreatedByName = SessionHelper.UserName,
                LastUpdatedBy = SessionHelper.UserID,
                Room = SessionHelper.RoomID,
                CompanyID = SessionHelper.CompanyID,
                RoomName = SessionHelper.RoomName,
                UpdatedByName = SessionHelper.UserName,
                IsOnlyFromItemUI = true,
            };
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("LocationMaster");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            return PartialView("_CreateLocation", objDTO);

        }

        public ActionResult ToolLocationCreate()
        {
            LocationMasterDTO objDTO = new LocationMasterDTO()
            {
                Created = DateTimeUtility.DateTimeNow,
                LastUpdated = DateTimeUtility.DateTimeNow,
                CreatedBy = SessionHelper.UserID,
                CreatedByName = SessionHelper.UserName,
                LastUpdatedBy = SessionHelper.UserID,
                Room = SessionHelper.RoomID,
                CompanyID = SessionHelper.CompanyID,
                RoomName = SessionHelper.RoomName,
                UpdatedByName = SessionHelper.UserName,
                IsOnlyFromItemUI = true,
            };
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("LocationMaster");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            return PartialView("_CreateLocation", objDTO);

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
        public JsonResult LocationSave(LocationMasterDTO objDTO)
        {
            string message = "";
            string status = "";
            //LocationMasterController obj = new LocationMasterController();
            LocationMasterDAL obj = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);

            if (string.IsNullOrEmpty(objDTO.Location))
            {
                message = string.Format(ResMessage.Required, ResLocation.Location);// "Location name is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            objDTO.Room = SessionHelper.RoomID;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;
            Int64 _NewIDForPopUp = 0;
            if (objDTO.ID == 0)
            {
                objDTO.CreatedBy = SessionHelper.UserID;

                string strOK = objCDAL.DuplicateCheck(objDTO.Location, "add", objDTO.ID, "LocationMaster", "Location", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResLocation.Location, objDTO.Location);  // "Location \"" + objDTO.Location + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.EditedFrom = "Web";
                    objDTO.AddedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    long ReturnVal = obj.Insert(objDTO);
                    if (ReturnVal > 0)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                        _NewIDForPopUp = ReturnVal;
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }
            else
            {
                string strOK = objCDAL.DuplicateCheck(objDTO.Location, "edit", objDTO.ID, "LocationMaster", "Location", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResLocation.Location, objDTO.Location);  // "Location \"" + objDTO.Location + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    objDTO.EditedFrom = "Web";
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    bool ReturnVal = obj.Edit(objDTO);
                    if (ReturnVal)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
            }

            return Json(new { Message = message, Status = status, NewIDForPopUp = _NewIDForPopUp }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>
        public ActionResult LocationEdit(Int64 ID)
        {
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            if (IsDeleted || IsArchived)
            {
                ViewBag.ViewOnly = true;
            }

            //LocationMasterController obj = new LocationMasterController();
            LocationMasterDAL obj = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
            LocationMasterDTO objDTO = obj.GetLocationByIDNormal(ID, SessionHelper.RoomID, SessionHelper.CompanyID);
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("LocationMaster");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;
            return PartialView("_CreateLocation", objDTO);
        }

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public JsonResult DeleteLocationMasterRecords(string ids)
        {
            try
            {
                //LocationMasterController obj = new LocationMasterController();
                //LocationMasterDAL obj = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
                //obj.DeleteRecords(ids, SessionHelper.UserID, SessionHelper.CompanyID);
                //return "ok";
                string response = string.Empty;
                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                response = objCommonDAL.DeleteModulewise(ids, ImportMastersDTO.TableName.LocationMaster.ToString(), false, SessionHelper.UserID, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);
                eTurns.DAL.CacheHelper<IEnumerable<LocationMasterDTO>>.InvalidateCache();
                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult UnDeleteLocationMasterRecords(string ids)
        {
            try
            {
                //LocationMasterController obj = new LocationMasterController();
                //LocationMasterDAL obj = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
                //obj.DeleteRecords(ids, SessionHelper.UserID, SessionHelper.CompanyID);
                //return "ok";
                LocationMasterDAL obj = new eTurns.DAL.LocationMasterDAL(SessionHelper.EnterPriseDBName);
                obj.UnDeleteRecords(ids, SessionHelper.UserID);
                eTurns.DAL.CacheHelper<IEnumerable<LocationMasterDTO>>.InvalidateCache();
                return Json(new { Message = ResCommon.RecordsUndeletedSuccessfully, Status = "ok" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
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
        //public string UpdateLocationMasterData(Int64 id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        //{
        //    //LocationMasterController obj = new LocationMasterController();
        //    LocationMasterDAL obj = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
        //    obj.UpdateData(id, value, rowId, columnPosition, columnId, columnName);
        //    return value;
        //}

        #region Data Provider

        /// <summary>
        /// Below method used to Locationd the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult GetLocationList(JQueryDataTableParamModel param)
        {
            LocationMasterDTO entity = new LocationMasterDTO();
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
                sortColumnName = "ID desc";


            string searchQuery = string.Empty;

            int TotalRecordCount = 0;

            //LocationMasterController controller = new LocationMasterController();
            LocationMasterDAL controller = new LocationMasterDAL(SessionHelper.EnterPriseDBName);
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            List<LocationMasterDTO> DataFromDB = controller.GetPagedLocationMaster(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, RoomDateFormat, CurrentTimeZone);
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);
        }



        #endregion

        #endregion

        #region "Common Master"

        //public string GetDropDownList(Guid ItemGUID, string PageName, Nullable<Int64> SelectedID)
        public string GetDropDownList(Guid ItemGUID, string PageName)
        {
            ViewBag.SelectedID = ItemGUID == null ? "" : Convert.ToString(ItemGUID);

            if (PageName == "ItemLocationList")
            {
                //BinMasterController objBinMasterApi = new BinMasterController();
                BinMasterDAL objBinMasterApi = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                ViewBag.DropDownData = objBinMasterApi.GetItemBinQuantityDict(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

                return RenderRazorViewToString("_DropDownList", new CommonDTO() { ControlID = "BinID", PageName = PageName });
            }
            else if (PageName == "ItemLocationListPULL")
            {
                ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);

                ViewBag.DefaultSelectedID = objItemMasterDAL.GetRecordForDropDown(ItemGUID.ToString(), SessionHelper.RoomID, SessionHelper.CompanyID);
                //BinMasterController objBinMasterApi = new BinMasterController();
                BinMasterDAL objBinMasterApi = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                ViewBag.DropDownData = objBinMasterApi.GetAllRecordsPULL(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

                return RenderRazorViewToString("_DropDownList", new CommonDTO() { ControlID = "BinID", PageName = PageName });
            }
            else if (PageName == "ItemLocationListPULLStaging")
            {
                ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);

                ViewBag.DefaultSelectedID = objItemMasterDAL.GetRecordForDropDown(ItemGUID.ToString(), SessionHelper.RoomID, SessionHelper.CompanyID);
                //BinMasterController objBinMasterApi = new BinMasterController();
                BinMasterDAL objBinMasterApi = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                ViewBag.DropDownData = objBinMasterApi.GetAllRecordsPULLStagin(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

                return RenderRazorViewToString("_DropDownList", new CommonDTO() { ControlID = "BinID", PageName = PageName });
            }
            else if (PageName == "ItemLocationListPULLBin")
            {
                ItemMasterDAL objItemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);

                ViewBag.DefaultSelectedID = objItemMasterDAL.GetRecordForDropDown(ItemGUID.ToString(), SessionHelper.RoomID, SessionHelper.CompanyID);
                //BinMasterController objBinMasterApi = new BinMasterController();
                BinMasterDAL objBinMasterApi = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                //ViewBag.DropDownData = objBinMasterApi.GetAllRecordsPULLBin(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

                //List<BinMasterDTO> lstBins = objBinMasterApi.GetAllRecordsPULLBin(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                //List<BinMasterDTO> lstStageBins = objBinMasterApi.GetAllRecordsPULLStagin(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                //lstBins.AddRange(lstStageBins);

                List<BinMasterDTO> lstBins = objBinMasterApi.GetBinsForNewPull(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                ViewBag.DropDownData = lstBins.OrderBy(x => x.BinNumber); ;
                return RenderRazorViewToString("_DropDownList", new CommonDTO() { ControlID = "BinID", PageName = PageName });

            }
            else if (PageName == "KitItemLocationList")
            {
                //BinMasterController objBinMasterApi = new BinMasterController();
                BinMasterDAL objBinMasterApi = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                ViewBag.DropDownData = objBinMasterApi.GetItemBinQuantityDict(ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

                return RenderRazorViewToString("_DropDownList", new CommonDTO() { ControlID = "BinID", PageName = PageName });
            }
            else if (PageName == "OrderItemBinList")
            {
                //BinMasterController objBinMasterApi = new BinMasterController();
                BinMasterDAL objBinMasterApi = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                //ViewBag.DropDownData = objBinMasterApi.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                ViewBag.DropDownData = objBinMasterApi.GetBinMasterByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                return RenderRazorViewToString("_DropDownList", new CommonDTO() { ControlID = "BinID", PageName = PageName });
            }
            if (PageName == "ProjectList")
            {
                //ProjectMasterController objData = new ProjectMasterController();
                ProjectMasterDAL objData = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
                List<ProjectMasterDTO> lstProject = new List<ProjectMasterDTO>();
                var projectTrackAllUsageAgainstThis = objData.GetDefaultProjectSpendRecord(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);

                if (projectTrackAllUsageAgainstThis != null && projectTrackAllUsageAgainstThis.GUID != Guid.Empty)
                {
                    lstProject.Add(projectTrackAllUsageAgainstThis);
                    ViewBag.DropDownData = lstProject;
                }
                else
                {
                    lstProject = objData.GetAllProjectMasterByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();

                    if (lstProject != null && lstProject.Any())
                    {
                        ViewBag.IsClosedFalse = 1;
                    }
                    ViewBag.DropDownData = lstProject;
                }

                return RenderRazorViewToString("_DropDownList", new CommonDTO() { ControlID = "ProjectID", PageName = PageName });
            }
            if (PageName == "ReceiveList")
            {
                //Int64 ItemId = 0;
                //if (SelectedID != null)
                //    ItemId = Int64.Parse(SelectedID.ToString());
                //ReceiveMasterController objReceiveMaster = new ReceiveMasterController();
                ReceiveOrderDetailsDAL objReceiveMaster = new ReceiveOrderDetailsDAL(SessionHelper.EnterPriseDBName);
                //ViewBag.DropDownData = objReceiveMaster.GetLineItemsOrderRecords(SessionHelper.RoomID, SessionHelper.CompanyID, ItemId, 0);
                ViewBag.DropDownData = objReceiveMaster.GetLineItemsOrderRecords(SessionHelper.RoomID, SessionHelper.CompanyID, ItemGUID.ToString(), 0);

                return RenderRazorViewToString("_DropDownList", new CommonDTO() { ControlID = "ID", PageName = PageName });
            }
            if (PageName == "ReceiveBinList")
            {
                //BinMasterController objBinMasterApi = new BinMasterController();
                BinMasterDAL objBinMasterApi = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                //ViewBag.DropDownData = objBinMasterApi.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                ViewBag.DropDownData = objBinMasterApi.GetBinMasterByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                return RenderRazorViewToString("_DropDownList", new CommonDTO() { ControlID = "ID", PageName = PageName });
            }

            return "ERROR";
        }

        //public MvcHtmlString RenderDropDownList(string ID, string ListType, Int64 SelectedID)
        //{
        //    if (ListType == "DDLBinListAll")
        //    {
        //        BinMasterController objBinMasterApi = new BinMasterController();
        //        StringBuilder sbTemp = new StringBuilder();
        //        var objBinlist = objBinMasterApi.GetAllRecord(SessionHelper.RoomID, SessionHelper.CompanyID);
        //        sbTemp.Append("<select id=\"ddl" + ID + "\" class=\"selectBox\">");
        //        sbTemp.Append("<option></option>");
        //        foreach (var item in objBinlist)
        //        {
        //            if (SelectedID > 0 && item.ID == SelectedID)
        //                sbTemp.Append("<option selected=\"True\" value=\"" + item.ID + "\">" + item.BinNumber + "</option>");
        //            else
        //                sbTemp.Append("<option value=\"" + item.ID + "\">" + item.BinNumber + "</option>");
        //        }
        //        sbTemp.Append("</select>");

        //        return MvcHtmlString.Create(sbTemp.ToString());
        //    }
        //    return null;
        //}

        public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        public JsonResult GetDDData(string TableName, string TextFieldName)
        {
            //CommonController obj = new CommonController();
            CommonDAL obj = new CommonDAL(SessionHelper.EnterPriseDBName);
            var data = obj.GetDDData(TableName, TextFieldName, SessionHelper.CompanyID, SessionHelper.RoomID);
            return Json(new { DDData = data }, JsonRequestBehavior.AllowGet);
        }
        //
        public JsonResult GetOrderStatus(string TableName, string TextFieldName)
        {
            //CommonController obj = new CommonController();
            CommonDAL obj = new CommonDAL(SessionHelper.EnterPriseDBName);
            //var data = obj.GetDDData(TableName, TextFieldName, SessionHelper.CompanyID, SessionHelper.RoomID);
            List<CommonDTO> returnList = new List<CommonDTO>();

            foreach (var item in Enum.GetValues(typeof(OrderStatus)))
            {
                string itemText = item.ToString();
                int itemValue = (int)(Enum.Parse(typeof(OrderStatus), itemText));
                //if (itemValue != (int)OrderStatus.Rejected)
                returnList.Add(new CommonDTO() { Text = ResOrder.GetOrderStatusText(item.ToString()), ID = itemValue, Count = 0 });
            }

            var data = returnList;

            return Json(new { DDData = data }, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 7200, VaryByParam = "None")]
        public JsonResult GetOrderRequiredDate(string TableName, string TextFieldName)
        {
            //CommonController obj = new CommonController();
            //CommonDAL obj = new CommonDAL(SessionHelper.EnterPriseDBName);
            //var data = obj.GetDDData(TableName, TextFieldName, SessionHelper.CompanyID, SessionHelper.RoomID);
            List<CommonDTO> returnList = new List<CommonDTO>();

            returnList.Add(new CommonDTO() { Text = ResOrder.MoreThanThreeWeeks, ID = 1, Count = 0 });
            returnList.Add(new CommonDTO() { Text = ResOrder.TwoToThreeWeeks, ID = 2, Count = 0 });
            returnList.Add(new CommonDTO() { Text = ResOrder.NextWeek, ID = 3, Count = 0 });
            returnList.Add(new CommonDTO() { Text = ResOrder.ThisWeek, ID = 4, Count = 0 });

            var data = returnList;

            return Json(new { DDData = data }, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 7200, VaryByParam = "None")]
        public JsonResult GetRequisitionRequiredDate(string TableName, string TextFieldName)
        {
            //CommonController obj = new CommonController();
            // CommonDAL obj = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<CommonDTO> returnList = new List<CommonDTO>();

            returnList.Add(new CommonDTO() { Text = ResRequisitionMaster.MoreThanEightWeeks, ID = 1, Count = 0 });
            returnList.Add(new CommonDTO() { Text = ResRequisitionMaster.FourToEightWeeks, ID = 2, Count = 0 });
            returnList.Add(new CommonDTO() { Text = ResRequisitionMaster.LessThanFourWeeks, ID = 3, Count = 0 });
            returnList.Add(new CommonDTO() { Text = ResRequisitionMaster.LessThanTwoWeeks, ID = 4, Count = 0 });
            returnList.Add(new CommonDTO() { Text = ResRequisitionMaster.LessThanOneWeek, ID = 5, Count = 0 });
            returnList.Add(new CommonDTO() { Text = ResOrder.ThisWeek, ID = 6, Count = 0 });
            returnList.Add(new CommonDTO() { Text = ResRequisitionMaster.Today, ID = 7, Count = 0 });
            returnList.Add(new CommonDTO() { Text = ResRequisitionMaster.PastDue, ID = 8, Count = 0 });

            var data = returnList;

            return Json(new { DDData = data }, JsonRequestBehavior.AllowGet);
        }
        [OutputCache(Duration = 0, VaryByParam = "None")]
        public JsonResult GetNarrowDDData_User(string TableName, List<string> TextFieldNames, bool IsArchived, bool IsDeleted, string RequisitionCurrentTab, bool IsIncludeClosedOrder = true, string ItemModelCallFrom = "", string EnterpriseIds = "", Int64 ParentID = 0, string CompanyIds = "", int? MoveTypeValue = null, bool IsSLCount = false, int LoadDataCount = 0)
        {
            Dictionary<string, Dictionary<string, int>> responseDict = new Dictionary<string, Dictionary<string, int>>();

            List<UserNS> outlstAllPermissions = new List<UserNS>();
            eTurnsMaster.DAL.UserMasterDAL objUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL();

            if (Session["AllUserPermissions"] != null && IsDeleted == false)
            {
                outlstAllPermissions = (List<UserNS>)Session["AllUserPermissions"];
            }
            else
            {
                if (IsDeleted)
                    outlstAllPermissions = objUserMasterDAL.GetPagedUsersNS(SessionHelper.UserID, IsDeleted);
                else
                {
                    Session["AllUserPermissions"] = objUserMasterDAL.GetPagedUsersNS(SessionHelper.UserID, IsDeleted);
                    outlstAllPermissions = (List<UserNS>)Session["AllUserPermissions"];
                }
            }
            using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(SessionHelper.EnterPriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
            {
                foreach (var TextFieldName in TextFieldNames)
                {
                    Dictionary<string, int> ColUDFData = new Dictionary<string, int>();
                    if (TextFieldName == "UserType")
                    {
                        var localColUDFData = (from tmp in outlstAllPermissions.Where(t => t.NSName == "usertype")
                                               select new
                                               {
                                                   count = tmp.UserCount,
                                                   rid = tmp.RoomID,
                                                   rname = tmp.RoomName
                                               }
                             );
                        ColUDFData = localColUDFData.OrderBy(t => t.rname).AsParallel().ToDictionary(e => e.rname + "[###]" + e.rid.ToString(), e => (int)e.count);
                    }
                    if (TextFieldName == "RoomName")
                    {
                        string[] oEnterpriseIDs = EnterpriseIds.Split(',');
                        string[] oCompanyIDs = CompanyIds.Split(',');
                        string newCompanyIds = "";
                        foreach (string item in oCompanyIDs)
                        {
                            if (item.Contains("_"))
                            {
                                newCompanyIds += item.Substring(item.IndexOf("_") + 1) + ",";
                            }
                            else
                            {
                                newCompanyIds += item + ",";
                            }
                        }

                        oCompanyIDs = newCompanyIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                        var localColUDFData = (from tmp in outlstAllPermissions.Where(t => t.NSName == "room"
                                                   && (oEnterpriseIDs.Contains(t.EnterpriseID.ToString()) || string.IsNullOrWhiteSpace(EnterpriseIds))
                                                   && (oCompanyIDs.Contains(t.CompanyID.ToString()) || string.IsNullOrWhiteSpace(CompanyIds)))
                                               select new
                                               {
                                                   count = tmp.UserCount,
                                                   rid = tmp.EnterpriseID + "_" + tmp.CompanyID + "_" + tmp.RoomID,
                                                   rname = tmp.RoomName + "(" + tmp.EnterpriseName + ">>" + tmp.CompanyName + ")"
                                               }
                          );
                        ColUDFData = localColUDFData.OrderBy(t => t.rname).AsParallel().ToDictionary(e => e.rname + "[###]" + e.rid.ToString(), e => (int)e.count);
                    }
                    if (TextFieldName == "CompanyName")
                    {
                        string[] oEnterpriseIDs = EnterpriseIds.Split(',');

                        var localColUDFData = (from tmp in outlstAllPermissions.Where(t => t.NSName == "company"
                            && (oEnterpriseIDs.Contains(t.EnterpriseID.ToString()) || string.IsNullOrWhiteSpace(EnterpriseIds)))
                                               select new
                                               {
                                                   count = tmp.UserCount,
                                                   rid = tmp.EnterpriseID + "_" + tmp.CompanyID,
                                                   rname = tmp.CompanyName + "(" + tmp.EnterpriseName + ")"
                                               }
                         );

                        ColUDFData = localColUDFData.OrderBy(t => t.rname).AsParallel().ToDictionary(e => e.rname + "[###]" + e.rid.ToString(), e => (int)e.count);
                    }
                    if (TextFieldName == "EnterpriseName")
                    {
                        var localColUDFData = (from tmp in outlstAllPermissions.Where(t => t.NSName == "enterprise")
                                               select new
                                               {
                                                   count = tmp.UserCount,
                                                   rid = tmp.EnterpriseID,
                                                   rname = tmp.EnterpriseName
                                               }
                         );

                        ColUDFData = localColUDFData.OrderBy(t => t.rname).AsParallel().ToDictionary(e => e.rname + "[###]" + e.rid.ToString(), e => (int)e.count);
                    }
                    if (TextFieldName == "RoleName")
                    {
                        var localColUDFData = (from tmp in outlstAllPermissions.Where(t => t.NSName == "role")
                                               select new
                                               {
                                                   count = tmp.UserCount,
                                                   rid = tmp.RoleId,
                                                   rname = tmp.RoleName + "(" + tmp.EnterpriseName + ")"
                                               }
                        );
                        ColUDFData = localColUDFData.OrderBy(t => t.rname).AsParallel().ToDictionary(e => e.rname + "[###]" + e.rid.ToString(), e => (int)e.count);
                    }
                    if (TextFieldName == "CreatedBy")
                    {
                        var localColUDFData = (from tmp in outlstAllPermissions.Where(t => t.NSName == "createdby")
                                               select new
                                               {
                                                   count = tmp.UserCount,
                                                   rid = tmp.RoomID,
                                                   rname = tmp.RoomName
                                               }
                          );
                        ColUDFData = localColUDFData.OrderBy(t => t.rname).AsParallel().ToDictionary(e => e.rname + "[###]" + e.rid.ToString(), e => (int)e.count);

                    }
                    if (TextFieldName.ToLower() == "lastupdatedby")
                    {
                        var localColUDFData = (from tmp in outlstAllPermissions.Where(t => t.NSName == "updatedby")
                                               select new
                                               {
                                                   count = tmp.UserCount,
                                                   rid = tmp.RoomID,
                                                   rname = tmp.RoomName
                                               }
                          );
                        ColUDFData = localColUDFData.OrderBy(t => t.rname).AsParallel().ToDictionary(e => e.rname + "[###]" + e.rid.ToString(), e => (int)e.count);

                    }
                    // Store in responseDict
                    responseDict[TextFieldName] = ColUDFData;
                }
            }
            return Json(new { DDData = responseDict }, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 0, VaryByParam = "None")]
        public JsonResult GetNarrowDDData_RoleMaster(List<string> TextFieldNames, string EnterpriseIds = "", string CompanyIds = "")
        {
            Dictionary<string, Dictionary<string, int>> responseDict = new Dictionary<string, Dictionary<string, int>>();
            List<RoleNS> outlstAllPermissions = new List<RoleNS>();
            eTurnsMaster.DAL.RoleMasterDAL objRoleMasterDAL = new eTurnsMaster.DAL.RoleMasterDAL();

            if (Session["AllRolePermissions"] != null)
            {
                outlstAllPermissions = (List<RoleNS>)Session["AllRolePermissions"];
            }
            else
            {
                Session["AllRolePermissions"] = objRoleMasterDAL.GetPagedRoleNS(SessionHelper.UserID);
                outlstAllPermissions = (List<RoleNS>)Session["AllRolePermissions"];
            }
            using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(SessionHelper.EnterPriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
            {
                foreach (var TextFieldName in TextFieldNames)
                {
                    Dictionary<string, int> ColUDFData = new Dictionary<string, int>();
                    if (TextFieldName == "UserType")
                    {
                        var localColUDFData = (from tmp in outlstAllPermissions.Where(t => t.NSName == "usertype")
                                               select new
                                               {
                                                   count = tmp.RoleCount,
                                                   rid = tmp.RoomID,
                                                   rname = tmp.RoomName
                                               }
                            ).Distinct();
                        ColUDFData = localColUDFData.OrderBy(t => t.rname).AsParallel().ToDictionary(e => e.rname + "[###]" + e.rid.ToString(), e => (int)e.count);

                    }
                    else if (TextFieldName == "CreatedBy")
                    {
                        var localColUDFData = (from tmp in outlstAllPermissions.Where(t => t.NSName == "createdby")
                                               select new
                                               {
                                                   count = tmp.RoleCount,
                                                   rid = tmp.RoomID,
                                                   rname = tmp.RoomName
                                               }
                         ).Distinct();
                        ColUDFData = localColUDFData.OrderBy(t => t.rname).AsParallel().ToDictionary(e => e.rname + "[###]" + e.rid.ToString(), e => (int)e.count);
                    }
                    else if (TextFieldName == "LastUpdatedBy")
                    {
                        var localColUDFData = (from tmp in outlstAllPermissions.Where(t => t.NSName == "updatedby")
                                               select new
                                               {
                                                   count = tmp.RoleCount,
                                                   rid = tmp.RoomID,
                                                   rname = tmp.RoomName
                                               }
                           ).Distinct();
                        ColUDFData = localColUDFData.OrderBy(t => t.rname).AsParallel().ToDictionary(e => e.rname + "[###]" + e.rid.ToString(), e => (int)e.count);
                    }
                    else if (TextFieldName == "EnterpriseName")
                    {
                        var localColUDFData = (from tmp in outlstAllPermissions.Where(t => t.NSName == "enterprise")
                                               select new
                                               {
                                                   count = tmp.RoleCount,
                                                   rid = tmp.EnterpriseID,
                                                   rname = tmp.EnterpriseName
                                               }
                        ).Distinct();

                        ColUDFData = localColUDFData.OrderBy(t => t.rname).AsParallel().ToDictionary(e => e.rname + "[###]" + e.rid.ToString(), e => (int)e.count);
                    }
                    else if (TextFieldName == "RoomName")
                    {
                        string[] oEnterpriseIDs = EnterpriseIds.Split(',');
                        string[] oCompanyIDs = CompanyIds.Split(',');
                        string newCompanyIds = "";
                        foreach (string item in oCompanyIDs)
                        {
                            if (item.Contains("_"))
                            {
                                newCompanyIds += item.Substring(item.IndexOf("_") + 1) + ",";
                            }
                            else
                            {
                                newCompanyIds += item + ",";
                            }
                        }

                        oCompanyIDs = newCompanyIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                        var localColUDFData = (from tmp in outlstAllPermissions.Where(t => t.NSName == "room"
                                                         && (oEnterpriseIDs.Contains(t.EnterpriseID.ToString()) || string.IsNullOrWhiteSpace(EnterpriseIds))
                                                         && (oCompanyIDs.Contains(t.CompanyID.ToString()) || string.IsNullOrWhiteSpace(CompanyIds)))
                                               select new
                                               {
                                                   count = tmp.RoleCount,
                                                   rid = tmp.EnterpriseID + "_" + tmp.CompanyID + "_" + tmp.RoomID,
                                                   rname = tmp.RoomName + "(" + tmp.EnterpriseName + ">>" + tmp.CompanyName + ")"
                                               }
                       ).Distinct();

                        ColUDFData = localColUDFData.OrderBy(t => t.rname).AsParallel().ToDictionary(e => e.rname + "[###]" + e.rid.ToString(), e => (int)e.count);
                    }
                    else if (TextFieldName == "CompanyName")
                    {
                        string[] oEnterpriseIDs = EnterpriseIds.Split(',');

                        var localColUDFData = (from tmp in outlstAllPermissions.Where(t => t.NSName == "company"
                                                && (oEnterpriseIDs.Contains(t.EnterpriseID.ToString())
                                                || string.IsNullOrWhiteSpace(EnterpriseIds)))
                                               select new
                                               {
                                                   count = tmp.RoleCount,
                                                   rid = tmp.EnterpriseID + "_" + tmp.CompanyID,
                                                   rname = tmp.CompanyName + "(" + tmp.EnterpriseName + ")"
                                               }
                       ).Distinct();

                        ColUDFData = localColUDFData.OrderBy(t => t.rname).AsParallel().ToDictionary(e => e.rname + "[###]" + e.rid.ToString(), e => (int)e.count);
                    }

                    // Store in responseDict
                    responseDict[TextFieldName] = ColUDFData;
                }
            }
            return Json(new { DDData = responseDict }, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration = 0, VaryByParam = "None")]
        public JsonResult GetNarrowDDData_Room(string TableName, List<string> TextFieldNames, bool IsArchived, bool IsDeleted, string RequisitionCurrentTab, bool IsIncludeClosedOrder = true, string ItemModelCallFrom = "", string EnterpriseIds = "", Int64 ParentID = 0, string CompanyIds = "", int? MoveTypeValue = null, bool IsSLCount = false, int LoadDataCount = 0)
        {
            CommonDAL obj = new CommonDAL(SessionHelper.EnterPriseDBName);

            Dictionary<string, Dictionary<string, int>> responseDict = new Dictionary<string, Dictionary<string, int>>();

            List<EnterpriseDTO> EnterPriseList = SessionHelper.EnterPriseList;
            List<RoomDTO> RoomList = SessionHelper.RoomList;

            using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(SessionHelper.EnterPriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
            {
                foreach (var textField in TextFieldNames)
                {
                    var data = obj.GetNarrowDDData(TableName, textField, SessionHelper.CompanyID, SessionHelper.RoomID, IsArchived,
                                IsDeleted, RequisitionCurrentTab, Convert.ToString(SessionHelper.RoomDateFormat), LoadDataCount,
                                SessionHelper.UserSupplierIds, (SessionHelper.CurrentTimeZone), RoomList, null, -1, EnterPriseList, null,
                                null, false, null, 0, null, null, null, null, null, null, SessionHelper.EnterPriceID, null, "", "1", null,
                                IsIncludeClosedOrder, ItemModelCallFrom: ItemModelCallFrom, EnterpriseIds: EnterpriseIds, false, ModuleGuid: "",
                                ParentID: ParentID, IsSLCount: IsSLCount, UserID: SessionHelper.UserID, requestFor: ItemModelCallFrom
                    );

                    if (textField == "BillingRoomType" && data.Count > 0)
                    {
                        using (BillingRoomTypeMasterBAL billingRoom = new BillingRoomTypeMasterBAL())
                        {
                            var billingTypes = billingRoom.GetBillingRoomTypeMaster(SessionHelper.EnterPriceID);
                            Dictionary<string, int> retDataBilling = new Dictionary<string, int>();
                            if (billingTypes.Count > 0)
                            {
                                foreach (var kv in data)
                                {
                                    string key = kv.Key;
                                    var billingId = int.Parse(key.Split(new string[] { "[###]" }, StringSplitOptions.None)[1]);
                                    string newKey = key;
                                    var objBilling = billingTypes.Where(b => b.ID == billingId).FirstOrDefault();
                                    if (objBilling != null)
                                    {
                                        newKey = objBilling.ResourceValue + "[###]" + billingId;
                                        retDataBilling.Add(newKey, kv.Value);
                                    }
                                }
                                data = retDataBilling;
                            }

                        }
                    }

                    responseDict[textField] = data;
                }
            }
            return Json(new { DDData = responseDict }, JsonRequestBehavior.AllowGet);
        }

        //    
        [OutputCache(Duration = 0, VaryByParam = "None")]
        public JsonResult GetNarrowDDData(string TableName, string TextFieldName, bool IsArchived, bool IsDeleted, string RequisitionCurrentTab, bool IsIncludeClosedOrder = true, string ItemModelCallFrom = "", string EnterpriseIds = "", Int64 ParentID = 0, string CompanyIds = "", int? MoveTypeValue = null, bool IsSLCount = false, int LoadDataCount = 0)
        {
            Dictionary<string, int> retData = new Dictionary<string, int>();
            if (LoadDataCount == 0)
            {
                //LoadDataCount = Settinfile.Element("LoadNarrowDataCount") != null ? Convert.ToInt32(Settinfile.Element("LoadNarrowDataCount").Value) : 0;
                LoadDataCount = SiteSettingHelper.LoadNarrowDataCount != string.Empty ? Convert.ToInt32(SiteSettingHelper.LoadNarrowDataCount) : 0;
            }


            if (TableName.ToLower() == "enterprisemaster")
            {
                CommonMasterDAL objCommonMasterDAL = new CommonMasterDAL();
                retData = objCommonMasterDAL.GetNarrowDDData(TableName, TextFieldName, IsArchived, IsDeleted, SessionHelper.UserType, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);
            }

            else
            {
                TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
                List<RoomDTO> RoomList = SessionHelper.RoomList;
                List<CompanyMasterDTO> CompanyList = SessionHelper.CompanyList;
                List<EnterpriseDTO> EnterPriseList = SessionHelper.EnterPriseList;
                Int64 RoleID = SessionHelper.RoleID;

                List<BinMasterDTO> AllBins = null;
                if (Session["AllBins"] != null)
                {
                    AllBins = (List<BinMasterDTO>)Session["AllBins"];
                }
                else
                {
                    BinMasterDAL objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                    AllBins = objBinDAL.GetAllBins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, string.Empty, null);
                    Session["AllBins"] = AllBins;
                }
                List<CompanyMasterDTO> lstCompanies = null;
                if (Session["AllCompanies"] != null)
                {
                    lstCompanies = (List<CompanyMasterDTO>)Session["AllCompanies"];
                }
                else
                {
                    CompanyMasterDAL objCompanyDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
                    lstCompanies = objCompanyDAL.GetAllCompaniesFromETurnsMaster(false, false, SessionHelper.CompanyList, SessionHelper.RoleID).ToList();
                    Session["AllCompanies"] = lstCompanies;
                }
                bool? Session_ConsignedAllowed = null;
                if (Session["ConsignedAllowed"] != null)
                {
                    Session_ConsignedAllowed = Convert.ToBoolean(Session["ConsignedAllowed"]);
                }
                List<BOMItemDTO> lstBomIterms = new List<BOMItemDTO>();
                if (Session["lstBomIterms"] != null)
                {
                    lstBomIterms = (List<BOMItemDTO>)Session["lstBomIterms"];
                }
                else
                {
                    int TotalRecordCountRM = 0;
                    BOMItemMasterDAL objBOMItemMasterDAL = new BOMItemMasterDAL(SessionHelper.EnterPriseDBName);
                    lstBomIterms = objBOMItemMasterDAL.GetPagedRecords(0, int.MaxValue, out TotalRecordCountRM, string.Empty, string.Empty, SessionHelper.CompanyID, false, false, SessionHelper.RoomID, "popup", SessionHelper.RoomDateFormat, CurrentTimeZone);
                    Session["lstBomIterms"] = lstBomIterms;
                }
                int? Session_QuicklistType = null;
                if (Session["QuicklistType"] != null)
                {
                    Session_QuicklistType = Convert.ToInt32(Session["QuicklistType"]);
                }
                ReportMasterDAL objRPTDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
                List<ReportMasterDTO> lstReportMaster = new List<ReportMasterDTO>();
                if (Session["ReportMasterList"] != null)
                {
                    lstReportMaster = (List<ReportMasterDTO>)Session["ReportMasterList"];
                }
                else
                {
                    Int64 TotalRecordCountRM = 0;
                    lstReportMaster = objRPTDAL.GetPagedReports(0, Int32.MaxValue, out TotalRecordCountRM, string.Empty, string.Empty, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, SessionHelper.RoomDateFormat, CurrentTimeZone);
                    Session["ReportMasterList"] = lstReportMaster;
                }
                ToolWrittenOffDAL toolWrittenOffDAL = new ToolWrittenOffDAL(SessionHelper.EnterPriseDBName);
                List<ToolWrittenOffDTO> lstWrittenOffTool = new List<ToolWrittenOffDTO>();
                if (Session["RoomAllWrittenOffTool"] != null)
                {
                    lstWrittenOffTool = (List<ToolWrittenOffDTO>)Session["RoomAllWrittenOffTool"];
                }
                if (!lstWrittenOffTool.Any())
                {
                    int totalRecords = 0;
                    string roomDateFormat = "yyyy-MM-dd HH:mm:ss";
                    lstWrittenOffTool = toolWrittenOffDAL.GetPagedRecords(0, int.MaxValue, out totalRecords, "", "", SessionHelper.RoomID, SessionHelper.CompanyID, false, false, roomDateFormat, CurrentTimeZone).ToList();
                    Session["RoomAllWrittenOffTool"] = lstWrittenOffTool;
                }
                List<ToolAssetOrderDetailsDTO> ToolAssetOrderDetail = null;
                if (Session["ToolAssetOrderDetail"] != null)
                {
                    ToolAssetOrderDetail = (List<ToolAssetOrderDetailsDTO>)Session["ToolAssetOrderDetail"];
                }
                List<ToolDetailDTO> ToolKitDetail = null;
                if (Session["ToolKitDetail"] != null)
                {
                    ToolKitDetail = (List<ToolDetailDTO>)Session["ToolKitDetail"];
                }
                string ToolORDType = string.Empty;
                if (Session["ToolORDType"] != null)
                {
                    ToolORDType = Convert.ToString(Session["ToolORDType"]);
                }
                string ToolType = string.Empty;
                if (Session["ToolType"] != null)
                {
                    ToolType = Convert.ToString(Session["ToolType"]);
                }
                Int64 Session_EnterPriceID = SessionHelper.EnterPriceID;
                CommonDAL obj = new CommonDAL(SessionHelper.EnterPriseDBName);
                if (!string.IsNullOrEmpty(TableName) && (TableName.ToLower() == "kitmaster" || TableName.ToLower() == "requisitionmaster"))
                {
                    string MainFilter = "";
                    if (Convert.ToString(Session["MainFilter"]).Trim().ToLower() == "true")
                    {
                        MainFilter = "true";
                    }

                    retData = obj.GetNarrowDDData(TableName, TextFieldName, SessionHelper.CompanyID, SessionHelper.RoomID, IsArchived, IsDeleted, RequisitionCurrentTab, Convert.ToString(SessionHelper.RoomDateFormat), LoadDataCount, SessionHelper.UserSupplierIds, CurrentTimeZone, RoomList, CompanyList, RoleID, EnterPriseList, AllBins, lstCompanies, Session_ConsignedAllowed, lstBomIterms, Session_QuicklistType, lstReportMaster, lstWrittenOffTool, ToolAssetOrderDetail, ToolKitDetail, ToolORDType, ToolType, Session_EnterPriceID, null, MainFilter, "1", null, IsIncludeClosedOrder, string.Empty, string.Empty, UserID: SessionHelper.UserID);
                }
                else
                {
                    if (SessionHelper.RoomList == null)
                    {
                        SessionHelper.RoomList = new List<RoomDTO>();
                    }
                    long[] RoomAccess = SessionHelper.RoomList.Where(t => t.IsRoomActive == true && t.ID != SessionHelper.RoomID).Select(t => t.ID).ToArray();
                    List<ToolAssetOrderDetailsDTO> lstToolAssetDetails = new List<ToolAssetOrderDetailsDTO>();
                    if (ParentID > 0 && TableName == "ToolAssetORD")
                    {
                        List<ToolAssetOrderDetailsDTO> lstDetailDTO = new List<ToolAssetOrderDetailsDTO>();

                        lstToolAssetDetails = (List<ToolAssetOrderDetailsDTO>)SessionHelper.Get(GetSessionKey(ParentID));
                        if (lstToolAssetDetails != null && lstToolAssetDetails.Count > 0)
                        {
                            lstDetailDTO = lstToolAssetDetails;
                        }

                    }
                    List<OrderDetailsDTO> lstDetails = new List<OrderDetailsDTO>();
                    if (ParentID > 0 && TableName == "ItemMaster")
                    {
                        List<OrderDetailsDTO> lstDetailDTO = new List<OrderDetailsDTO>();

                        lstDetails = (List<OrderDetailsDTO>)SessionHelper.Get(GetSessionKey(ParentID));
                        if (lstDetails != null && lstDetails.Count > 0)
                        {
                            lstDetailDTO = lstDetails;
                        }

                    }
                    Int64 _ParentID = ParentID;
                    bool IsAllowConsignedCredit = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowConsignedCreditPull, eTurnsWeb.Helper.SessionHelper.PermissionType.AllowPull);

                    if (!string.IsNullOrEmpty(ItemModelCallFrom) && ItemModelCallFrom.Trim().ToLower() == "movemtr")
                    {
                        MoveType moveType;
                        if (_ParentID > 0)
                        {
                            Enum.TryParse(_ParentID.ToString(), out moveType);
                            retData = obj.GetNarrowDDData(TableName, TextFieldName, SessionHelper.CompanyID, SessionHelper.RoomID, IsArchived, IsDeleted, RequisitionCurrentTab, Convert.ToString(SessionHelper.RoomDateFormat), LoadDataCount, SessionHelper.UserSupplierIds, CurrentTimeZone, RoomList, CompanyList, RoleID, EnterPriseList, AllBins, lstCompanies, Session_ConsignedAllowed, lstBomIterms, Session_QuicklistType, lstReportMaster, lstWrittenOffTool, ToolAssetOrderDetail, ToolKitDetail, ToolORDType, ToolType, Session_EnterPriceID, null, "", "1", RoomAccess, IsIncludeClosedOrder, ItemModelCallFrom: ItemModelCallFrom, EnterpriseIds: EnterpriseIds, IsAllowConsignedCredit: IsAllowConsignedCredit, ModuleGuid: "", ParentID: _ParentID, moveType: moveType, UserID: SessionHelper.UserID);
                        }
                        else
                        {
                            retData = obj.GetNarrowDDData(TableName, TextFieldName, SessionHelper.CompanyID, SessionHelper.RoomID, IsArchived, IsDeleted, RequisitionCurrentTab, Convert.ToString(SessionHelper.RoomDateFormat), LoadDataCount, SessionHelper.UserSupplierIds, CurrentTimeZone, RoomList, CompanyList, RoleID, EnterPriseList, AllBins, lstCompanies, Session_ConsignedAllowed, lstBomIterms, Session_QuicklistType, lstReportMaster, lstWrittenOffTool, ToolAssetOrderDetail, ToolKitDetail, ToolORDType, ToolType, Session_EnterPriceID, null, "", "1", RoomAccess, IsIncludeClosedOrder, ItemModelCallFrom: ItemModelCallFrom, EnterpriseIds: EnterpriseIds, IsAllowConsignedCredit: IsAllowConsignedCredit, ModuleGuid: "", ParentID: _ParentID, UserID: SessionHelper.UserID);
                        }


                    }
                    else
                    {
                        string requestFor = ItemModelCallFrom;
                        if (ItemModelCallFrom.ToLower().Equals("credit")
                            || ItemModelCallFrom.ToLower().Equals("creditms"))
                        {
                            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                            string columnList = "ID,RoomName,IsIgnoreCreditRule";
                            RoomDTO objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");

                            bool applayIsIgnoreCreditRule = false;
                            if (objRoomDTO != null)
                            {
                                applayIsIgnoreCreditRule = objRoomDTO.IsIgnoreCreditRule;
                                if (applayIsIgnoreCreditRule)
                                {
                                    ItemModelCallFrom = "newpull";
                                }
                            }
                        }

                        retData = obj.GetNarrowDDData(TableName, TextFieldName, SessionHelper.CompanyID, SessionHelper.RoomID, IsArchived, IsDeleted, RequisitionCurrentTab, Convert.ToString(SessionHelper.RoomDateFormat), LoadDataCount, SessionHelper.UserSupplierIds, CurrentTimeZone, RoomList, CompanyList, RoleID, EnterPriseList, AllBins, lstCompanies, Session_ConsignedAllowed, lstBomIterms, Session_QuicklistType, lstReportMaster, lstWrittenOffTool, ToolAssetOrderDetail, ToolKitDetail, ToolORDType, ToolType, Session_EnterPriceID, null, "", "1", RoomAccess, IsIncludeClosedOrder, ItemModelCallFrom: ItemModelCallFrom, EnterpriseIds: EnterpriseIds, IsAllowConsignedCredit: IsAllowConsignedCredit, ModuleGuid: "", ParentID: _ParentID, IsSLCount: IsSLCount, UserID: SessionHelper.UserID, requestFor: requestFor);

                        if (TableName == "Room" && TextFieldName == "BillingRoomType" && retData.Count > 0)
                        {
                            using (BillingRoomTypeMasterBAL billingRoom = new BillingRoomTypeMasterBAL())
                            {
                                var billingTypes = billingRoom.GetBillingRoomTypeMaster(SessionHelper.EnterPriceID);
                                Dictionary<string, int> retDataBilling = new Dictionary<string, int>();
                                if (billingTypes.Count > 0)
                                {
                                    foreach (var kv in retData)
                                    {
                                        string key = kv.Key;
                                        var billingId = int.Parse(key.Split(new string[] { "[###]" }, StringSplitOptions.None)[1]);
                                        string newKey = key;
                                        var objBilling = billingTypes.Where(b => b.ID == billingId).FirstOrDefault();
                                        if (objBilling != null)
                                        {
                                            newKey = newKey + "[###]" + objBilling.ResourceValue;
                                            retDataBilling.Add(newKey, kv.Value);
                                        }
                                    }
                                    retData = retDataBilling;
                                }

                            }
                        }
                    }
                }
            }
            return Json(new { DDData = retData }, JsonRequestBehavior.AllowGet);
        }
        [OutputCache(Duration = 0, VaryByParam = "None")]
        public JsonResult GetNarrowDDDataForBarcode(string TableName, string TextFieldName, bool IsArchived, bool IsDeleted, string ModuleGuid, int LoadDataCount = 0)
        {
            if (LoadDataCount == 0)
            {
                //LoadDataCount = Settinfile.Element("LoadNarrowDataCount") != null ? Convert.ToInt32(Settinfile.Element("LoadNarrowDataCount").Value) : 0;
                LoadDataCount = SiteSettingHelper.LoadNarrowDataCount != string.Empty ? Convert.ToInt32(SiteSettingHelper.LoadNarrowDataCount) : 0;
            }
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            List<RoomDTO> RoomList = SessionHelper.RoomList;
            List<CompanyMasterDTO> CompanyList = SessionHelper.CompanyList;
            List<EnterpriseDTO> EnterPriseList = SessionHelper.EnterPriseList;
            List<BinMasterDTO> AllBins = null;
            if (Session["AllBins"] != null)
            {
                AllBins = (List<BinMasterDTO>)Session["AllBins"];
            }
            else
            {
                BinMasterDAL objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                AllBins = objBinDAL.GetAllBins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, string.Empty, null);
                Session["AllBins"] = AllBins;
            }
            List<CompanyMasterDTO> lstCompanies = null;
            if (Session["AllCompanies"] != null)
            {
                lstCompanies = (List<CompanyMasterDTO>)Session["AllCompanies"];
            }
            else
            {
                CompanyMasterDAL objCompanyDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
                lstCompanies = objCompanyDAL.GetAllCompaniesFromETurnsMaster(false, false, SessionHelper.CompanyList, SessionHelper.RoleID).ToList();
                Session["AllCompanies"] = lstCompanies;
            }

            bool? Session_ConsignedAllowed = null;
            if (Session["ConsignedAllowed"] != null)
            {
                Session_ConsignedAllowed = Convert.ToBoolean(Session["ConsignedAllowed"]);
            }
            List<BOMItemDTO> lstBomIterms = new List<BOMItemDTO>();
            if (Session["lstBomIterms"] != null)
            {
                lstBomIterms = (List<BOMItemDTO>)Session["lstBomIterms"];
            }
            else
            {
                int TotalRecordCountRM = 0;
                BOMItemMasterDAL objBOMItemMasterDAL = new BOMItemMasterDAL(SessionHelper.EnterPriseDBName);
                lstBomIterms = objBOMItemMasterDAL.GetPagedRecords(0, int.MaxValue, out TotalRecordCountRM, string.Empty, string.Empty, SessionHelper.CompanyID, false, false, SessionHelper.RoomID, "popup", SessionHelper.RoomDateFormat, CurrentTimeZone);
                Session["lstBomIterms"] = lstBomIterms;
            }
            int? Session_QuicklistType = null;
            if (Session["QuicklistType"] != null)
            {
                Session_QuicklistType = Convert.ToInt32(Session["QuicklistType"]);
            }
            ReportMasterDAL objRPTDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            List<ReportMasterDTO> lstReportMaster = new List<ReportMasterDTO>();
            if (Session["ReportMasterList"] != null)
            {
                lstReportMaster = (List<ReportMasterDTO>)Session["ReportMasterList"];
            }
            else
            {
                Int64 TotalRecordCountRM = 0;
                lstReportMaster = objRPTDAL.GetPagedReports(0, Int32.MaxValue, out TotalRecordCountRM, string.Empty, string.Empty, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, SessionHelper.RoomDateFormat, CurrentTimeZone);
                Session["ReportMasterList"] = lstReportMaster;
            }
            ToolWrittenOffDAL toolWrittenOffDAL = new ToolWrittenOffDAL(SessionHelper.EnterPriseDBName);
            List<ToolWrittenOffDTO> lstWrittenOffTool = new List<ToolWrittenOffDTO>();
            if (Session["RoomAllWrittenOffTool"] != null)
            {
                lstWrittenOffTool = (List<ToolWrittenOffDTO>)Session["RoomAllWrittenOffTool"];
            }

            if (!lstWrittenOffTool.Any())
            {
                int totalRecords = 0;
                string roomDateFormat = "yyyy-MM-dd HH:mm:ss";
                lstWrittenOffTool = toolWrittenOffDAL.GetPagedRecords(0, int.MaxValue, out totalRecords, "", "", SessionHelper.RoomID, SessionHelper.CompanyID, false, false, roomDateFormat, CurrentTimeZone).ToList();
                Session["RoomAllWrittenOffTool"] = lstWrittenOffTool;
            }
            List<ToolAssetOrderDetailsDTO> ToolAssetOrderDetail = null;
            if (Session["ToolAssetOrderDetail"] != null)
            {
                ToolAssetOrderDetail = (List<ToolAssetOrderDetailsDTO>)Session["ToolAssetOrderDetail"];
            }
            List<ToolDetailDTO> ToolKitDetail = null;
            if (Session["ToolKitDetail"] != null)
            {
                ToolKitDetail = (List<ToolDetailDTO>)Session["ToolKitDetail"];
            }
            string ToolORDType = string.Empty;
            if (Session["ToolORDType"] != null)
            {
                ToolORDType = Convert.ToString(Session["ToolORDType"]);
            }
            string ToolType = string.Empty;
            if (Session["ToolType"] != null)
            {
                ToolType = Convert.ToString(Session["ToolType"]);
            }
            Int64 Session_EnterPriceID = SessionHelper.EnterPriceID;

            Int64 RoleID = SessionHelper.RoleID;
            Dictionary<string, int> retData = new Dictionary<string, int>();
            CommonDAL obj = new CommonDAL(SessionHelper.EnterPriseDBName);
            retData = obj.GetNarrowDDData(TableName, TextFieldName, SessionHelper.CompanyID, SessionHelper.RoomID, IsArchived, IsDeleted, string.Empty, Convert.ToString(SessionHelper.RoomDateFormat), LoadDataCount, SessionHelper.UserSupplierIds, CurrentTimeZone, RoomList, CompanyList, RoleID, EnterPriseList, AllBins, lstCompanies, Session_ConsignedAllowed, lstBomIterms, Session_QuicklistType, lstReportMaster, lstWrittenOffTool, ToolAssetOrderDetail, ToolKitDetail, ToolORDType, ToolType, Session_EnterPriceID, null, "", "1", null, true, ModuleGuid: ModuleGuid, UserID: SessionHelper.UserID);
            return Json(new { DDData = retData }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetModuleWiseItems(string TableName, string TextFieldName, bool IsArchived, bool IsDeleted, Guid ModuleGuid, int LoadDataCount = 0)
        {
            if (LoadDataCount == 0)
            {
                //LoadDataCount = Settinfile.Element("LoadNarrowDataCount") != null ? Convert.ToInt32(Settinfile.Element("LoadNarrowDataCount").Value) : 0;
                LoadDataCount = SiteSettingHelper.LoadNarrowDataCount != string.Empty ? Convert.ToInt32(SiteSettingHelper.LoadNarrowDataCount) : 0;
            }
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            List<RoomDTO> RoomList = SessionHelper.RoomList;
            List<CompanyMasterDTO> CompanyList = SessionHelper.CompanyList;
            List<EnterpriseDTO> EnterPriseList = SessionHelper.EnterPriseList;
            List<BinMasterDTO> AllBins = null;
            if (Session["AllBins"] != null)
            {
                AllBins = (List<BinMasterDTO>)Session["AllBins"];
            }
            else
            {
                BinMasterDAL objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                AllBins = objBinDAL.GetAllBins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, string.Empty, null);
                Session["AllBins"] = AllBins;
            }
            bool? Session_ConsignedAllowed = null;
            if (Session["ConsignedAllowed"] != null)
            {
                Session_ConsignedAllowed = Convert.ToBoolean(Session["ConsignedAllowed"]);
            }
            List<BOMItemDTO> lstBomIterms = new List<BOMItemDTO>();
            if (Session["lstBomIterms"] != null)
            {
                lstBomIterms = (List<BOMItemDTO>)Session["lstBomIterms"];
            }
            else
            {
                int TotalRecordCountRM = 0;
                BOMItemMasterDAL objBOMItemMasterDAL = new BOMItemMasterDAL(SessionHelper.EnterPriseDBName);
                lstBomIterms = objBOMItemMasterDAL.GetPagedRecords(0, int.MaxValue, out TotalRecordCountRM, string.Empty, string.Empty, SessionHelper.CompanyID, false, false, SessionHelper.RoomID, "popup", SessionHelper.RoomDateFormat, CurrentTimeZone);
                Session["lstBomIterms"] = lstBomIterms;
            }
            List<CompanyMasterDTO> lstCompanies = null;
            if (Session["AllCompanies"] != null)
            {
                lstCompanies = (List<CompanyMasterDTO>)Session["AllCompanies"];
            }
            else
            {
                CompanyMasterDAL objCompanyDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
                lstCompanies = objCompanyDAL.GetAllCompaniesFromETurnsMaster(false, false, SessionHelper.CompanyList, SessionHelper.RoleID).ToList();
                Session["AllCompanies"] = lstCompanies;
            }
            int? Session_QuicklistType = null;
            if (Session["QuicklistType"] != null)
            {
                Session_QuicklistType = Convert.ToInt32(Session["QuicklistType"]);
            }
            ReportMasterDAL objRPTDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            List<ReportMasterDTO> lstReportMaster = new List<ReportMasterDTO>();
            if (Session["ReportMasterList"] != null)
            {
                lstReportMaster = (List<ReportMasterDTO>)Session["ReportMasterList"];
            }
            else
            {
                Int64 TotalRecordCountRM = 0;
                lstReportMaster = objRPTDAL.GetPagedReports(0, Int32.MaxValue, out TotalRecordCountRM, string.Empty, string.Empty, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, SessionHelper.RoomDateFormat, CurrentTimeZone);
                Session["ReportMasterList"] = lstReportMaster;
            }
            ToolWrittenOffDAL toolWrittenOffDAL = new ToolWrittenOffDAL(SessionHelper.EnterPriseDBName);
            List<ToolWrittenOffDTO> lstWrittenOffTool = new List<ToolWrittenOffDTO>();
            if (Session["RoomAllWrittenOffTool"] != null)
            {
                lstWrittenOffTool = (List<ToolWrittenOffDTO>)Session["RoomAllWrittenOffTool"];
            }

            if (!lstWrittenOffTool.Any())
            {
                int totalRecords = 0;
                string roomDateFormat = "yyyy-MM-dd HH:mm:ss";
                lstWrittenOffTool = toolWrittenOffDAL.GetPagedRecords(0, int.MaxValue, out totalRecords, "", "", SessionHelper.RoomID, SessionHelper.CompanyID, false, false, roomDateFormat, CurrentTimeZone).ToList();
                Session["RoomAllWrittenOffTool"] = lstWrittenOffTool;
            }
            List<ToolAssetOrderDetailsDTO> ToolAssetOrderDetail = null;
            if (Session["ToolAssetOrderDetail"] != null)
            {
                ToolAssetOrderDetail = (List<ToolAssetOrderDetailsDTO>)Session["ToolAssetOrderDetail"];
            }
            List<ToolDetailDTO> ToolKitDetail = null;
            if (Session["ToolKitDetail"] != null)
            {
                ToolKitDetail = (List<ToolDetailDTO>)Session["ToolKitDetail"];
            }

            string ToolORDType = string.Empty;
            if (Session["ToolORDType"] != null)
            {
                ToolORDType = Convert.ToString(Session["ToolORDType"]);
            }
            string ToolType = string.Empty;
            if (Session["ToolType"] != null)
            {
                ToolType = Convert.ToString(Session["ToolType"]);
            }
            Int64 Session_EnterPriceID = SessionHelper.EnterPriceID;
            Int64 RoleID = SessionHelper.RoleID;
            Dictionary<string, int> retData = new Dictionary<string, int>();
            CommonDAL obj = new CommonDAL(SessionHelper.EnterPriseDBName);
            if (ModuleGuid != null && (!string.IsNullOrEmpty(Convert.ToString(ModuleGuid))) && ModuleGuid != Guid.Empty)
            {
                retData = obj.GetNarrowDDData(TableName, TextFieldName, SessionHelper.CompanyID, SessionHelper.RoomID, IsArchived, IsDeleted, string.Empty, Convert.ToString(SessionHelper.RoomDateFormat), LoadDataCount, SessionHelper.UserSupplierIds, CurrentTimeZone, RoomList, CompanyList, RoleID, EnterPriseList, AllBins, lstCompanies, Session_ConsignedAllowed, lstBomIterms, Session_QuicklistType, lstReportMaster, lstWrittenOffTool, ToolAssetOrderDetail, ToolKitDetail, ToolORDType, ToolType, Session_EnterPriceID, null, "", "1", null, false, ModuleGuid: ModuleGuid.ToString(), UserID: SessionHelper.UserID);
            }
            else
            {
                retData = obj.GetNarrowDDData(TableName, TextFieldName, SessionHelper.CompanyID, SessionHelper.RoomID, IsArchived, IsDeleted, string.Empty, Convert.ToString(SessionHelper.RoomDateFormat), LoadDataCount, SessionHelper.UserSupplierIds, CurrentTimeZone, RoomList, CompanyList, RoleID, EnterPriseList, AllBins, lstCompanies, Session_ConsignedAllowed, lstBomIterms, Session_QuicklistType, lstReportMaster, lstWrittenOffTool, ToolAssetOrderDetail, ToolKitDetail, ToolORDType, ToolType, Session_EnterPriceID, null, "", "1", null, false, UserID: SessionHelper.UserID);
            }
            return Json(new { DDData = retData }, JsonRequestBehavior.AllowGet);
        }
        private string GetSessionKey(Int64 OrderID = 0)
        {
            string strKey = "OrderLineItem_" + SessionHelper.EnterPriceID + "_" + SessionHelper.CompanyID + "_" + SessionHelper.RoomID;
            return strKey;
        }
        [OutputCache(Duration = 0, VaryByParam = "None")]
        public JsonResult GetNarrowDDDataIMItemType(string TableName, string TextFieldName, bool IsArchived, bool IsDeleted, string RequisitionCurrentTab, string QLType, Int64 ParentID = 0, string ItemModelCallFrom = "", int LoadDataCount = 0, Int64 moveType = 0)
        {
            if (LoadDataCount == 0)
            {
                //LoadDataCount = Settinfile.Element("LoadNarrowDataCount") != null ? Convert.ToInt32(Settinfile.Element("LoadNarrowDataCount").Value) : 0;
                LoadDataCount = SiteSettingHelper.LoadNarrowDataCount != string.Empty ? Convert.ToInt32(SiteSettingHelper.LoadNarrowDataCount) : 0;
            }
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            List<RoomDTO> RoomList = SessionHelper.RoomList;
            List<CompanyMasterDTO> CompanyList = SessionHelper.CompanyList;
            List<EnterpriseDTO> EnterPriseList = SessionHelper.EnterPriseList;
            Int64 RoleID = SessionHelper.RoleID;
            List<BinMasterDTO> AllBins = null;
            if (Session["AllBins"] != null)
            {
                AllBins = (List<BinMasterDTO>)Session["AllBins"];
            }
            else
            {
                BinMasterDAL objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                AllBins = objBinDAL.GetAllBins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, string.Empty, null);
                Session["AllBins"] = AllBins;
            }
            List<CompanyMasterDTO> lstCompanies = null;
            if (Session["AllCompanies"] != null)
            {
                lstCompanies = (List<CompanyMasterDTO>)Session["AllCompanies"];
            }
            else
            {
                CompanyMasterDAL objCompanyDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
                lstCompanies = objCompanyDAL.GetAllCompaniesFromETurnsMaster(false, false, SessionHelper.CompanyList, SessionHelper.RoleID).ToList();
                Session["AllCompanies"] = lstCompanies;
            }
            Dictionary<string, int> retData = new Dictionary<string, int>();
            CommonDAL obj = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<ToolAssetOrderDetailsDTO> lstToolAssetDetails = new List<ToolAssetOrderDetailsDTO>();
            if (ParentID > 0 && TableName == "ToolAssetORD")
            {
                List<ToolAssetOrderDetailsDTO> lstDetailDTO = new List<ToolAssetOrderDetailsDTO>();

                lstToolAssetDetails = (List<ToolAssetOrderDetailsDTO>)SessionHelper.Get(GetSessionKey(ParentID));
                if (lstToolAssetDetails != null && lstToolAssetDetails.Count > 0)
                {
                    lstDetailDTO = lstToolAssetDetails;
                }

            }
            List<OrderDetailsDTO> lstDetails = new List<OrderDetailsDTO>();
            if (ParentID > 0 && TableName == "ItemMaster")
            {
                List<OrderDetailsDTO> lstDetailDTO = new List<OrderDetailsDTO>();

                lstDetails = (List<OrderDetailsDTO>)SessionHelper.Get(GetSessionKey(ParentID));
                if (lstDetails != null && lstDetails.Count > 0)
                {
                    lstDetailDTO = lstDetails;
                }

            }

            bool? Session_ConsignedAllowed = null;
            if (Session["ConsignedAllowed"] != null)
            {
                Session_ConsignedAllowed = Convert.ToBoolean(Session["ConsignedAllowed"]);
            }
            List<BOMItemDTO> lstBomIterms = new List<BOMItemDTO>();
            if (Session["lstBomIterms"] != null)
            {
                lstBomIterms = (List<BOMItemDTO>)Session["lstBomIterms"];
            }
            else
            {
                int TotalRecordCountRM = 0;
                BOMItemMasterDAL objBOMItemMasterDAL = new BOMItemMasterDAL(SessionHelper.EnterPriseDBName);
                lstBomIterms = objBOMItemMasterDAL.GetPagedRecords(0, int.MaxValue, out TotalRecordCountRM, string.Empty, string.Empty, SessionHelper.CompanyID, false, false, SessionHelper.RoomID, "popup", SessionHelper.RoomDateFormat, CurrentTimeZone);
                Session["lstBomIterms"] = lstBomIterms;
            }
            int? Session_QuicklistType = null;
            if (Session["QuicklistType"] != null)
            {
                Session_QuicklistType = Convert.ToInt32(Session["QuicklistType"]);
            }
            ReportMasterDAL objRPTDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            List<ReportMasterDTO> lstReportMaster = new List<ReportMasterDTO>();
            if (Session["ReportMasterList"] != null)
            {
                lstReportMaster = (List<ReportMasterDTO>)Session["ReportMasterList"];
            }
            else
            {
                Int64 TotalRecordCountRM = 0;
                lstReportMaster = objRPTDAL.GetPagedReports(0, Int32.MaxValue, out TotalRecordCountRM, string.Empty, string.Empty, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, SessionHelper.RoomDateFormat, CurrentTimeZone);
                Session["ReportMasterList"] = lstReportMaster;
            }
            ToolWrittenOffDAL toolWrittenOffDAL = new ToolWrittenOffDAL(SessionHelper.EnterPriseDBName);
            List<ToolWrittenOffDTO> lstWrittenOffTool = new List<ToolWrittenOffDTO>();
            if (Session["RoomAllWrittenOffTool"] != null)
            {
                lstWrittenOffTool = (List<ToolWrittenOffDTO>)Session["RoomAllWrittenOffTool"];
            }

            if (!lstWrittenOffTool.Any())
            {
                int totalRecords = 0;
                string roomDateFormat = "yyyy-MM-dd HH:mm:ss";
                lstWrittenOffTool = toolWrittenOffDAL.GetPagedRecords(0, int.MaxValue, out totalRecords, "", "", SessionHelper.RoomID, SessionHelper.CompanyID, false, false, roomDateFormat, CurrentTimeZone).ToList();
                Session["RoomAllWrittenOffTool"] = lstWrittenOffTool;
            }
            List<ToolAssetOrderDetailsDTO> ToolAssetOrderDetail = null;
            if (Session["ToolAssetOrderDetail"] != null)
            {
                ToolAssetOrderDetail = (List<ToolAssetOrderDetailsDTO>)Session["ToolAssetOrderDetail"];
            }
            List<ToolDetailDTO> ToolKitDetail = null;
            if (Session["ToolKitDetail"] != null)
            {
                ToolKitDetail = (List<ToolDetailDTO>)Session["ToolKitDetail"];
            }
            string ToolORDType = string.Empty;
            if (Session["ToolORDType"] != null)
            {
                ToolORDType = Convert.ToString(Session["ToolORDType"]);
            }
            string ToolType = string.Empty;
            if (Session["ToolType"] != null)
            {
                ToolType = Convert.ToString(Session["ToolType"]);
            }
            Int64 Session_EnterPriceID = SessionHelper.EnterPriceID;
            MoveType moveTypeEnumValue;
            Enum.TryParse(moveType.ToString(), out moveTypeEnumValue);

            string requestFor = ItemModelCallFrom;

            retData = obj.GetNarrowDDData(TableName, TextFieldName, SessionHelper.CompanyID, SessionHelper.RoomID, IsArchived, IsDeleted, RequisitionCurrentTab, Convert.ToString(SessionHelper.RoomDateFormat), LoadDataCount, SessionHelper.UserSupplierIds, CurrentTimeZone, RoomList, CompanyList, RoleID, EnterPriseList, AllBins, lstCompanies, Session_ConsignedAllowed, lstBomIterms, Session_QuicklistType, lstReportMaster, lstWrittenOffTool, ToolAssetOrderDetail, ToolKitDetail, ToolORDType, ToolType, Session_EnterPriceID, null, "", QLType, null, true, ItemModelCallFrom: ItemModelCallFrom, ParentID: ParentID, moveType: moveTypeEnumValue, UserID: SessionHelper.UserID, requestFor: requestFor);

            return Json(new { DDData = retData }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUDFDDData_All(List<UDFRequest> udfRequests, bool IsDeleted)
        {
            eTurnsMaster.DAL.UserMasterDAL objUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL();
            var obj = new CommonDAL(SessionHelper.EnterPriseDBName);
            var resultList = new List<object>();
            int LoadDataCount = 0;
            if (LoadDataCount == 0)
            {
                //LoadDataCount = Settinfile.Element("LoadNarrowDataCount") != null ? Convert.ToInt32(Settinfile.Element("LoadNarrowDataCount").Value) : 0;
                LoadDataCount = SiteSettingHelper.LoadNarrowDataCount != string.Empty ? Convert.ToInt32(SiteSettingHelper.LoadNarrowDataCount) : 0;
            }

            List<UserNS> outlstAllPermissions = Session["AllUserPermissions"] as List<UserNS>;
            if (Session["AllUserPermissions"] != null && IsDeleted == false)
            {
                outlstAllPermissions = (List<UserNS>)Session["AllUserPermissions"];
            }
            else
            {
                if (IsDeleted)
                    outlstAllPermissions = objUserMasterDAL.GetPagedUsersNS(SessionHelper.UserID, IsDeleted);
                else
                {
                    Session["AllUserPermissions"] = objUserMasterDAL.GetPagedUsersNS(SessionHelper.UserID, IsDeleted);
                    outlstAllPermissions = (List<UserNS>)Session["AllUserPermissions"];
                }
            }
            if (udfRequests != null)
            {
                if (udfRequests[0].TableName == "UserMaster")
                {
                    foreach (var req in udfRequests)
                    {
                        Dictionary<string, int> ColUDFData = new Dictionary<string, int>();

                        if (req.TableName.ToLower() == "usermaster")
                        {
                            var localColUDFData = outlstAllPermissions
                                .Where(t => t.NSName == req.UDFUniqueID && !string.IsNullOrWhiteSpace(t.UDF))
                                .Select(tmp => new { count = tmp.UserCount, rid = tmp.UDF, rname = tmp.UDF });

                            ColUDFData = localColUDFData.OrderBy(t => t.rname).AsParallel().ToDictionary(e => e.rname, e => (int)e.count);
                        }

                        resultList.Add(new
                        {
                            DDData = ColUDFData,
                            UDFColName = req.UDFName,
                            UDFUniqueID = req.UDFUniqueID
                        });
                    }
                }
                else if (udfRequests[0].TableName == "Room")
                {
                    List<RoomDTO> RoomList = SessionHelper.RoomList;

                    List<RoomDTO> AllRooms = null;
                    if (Session["AllRooms"] != null)
                    {
                        AllRooms = (List<RoomDTO>)Session["AllRooms"];
                    }
                    else
                    {
                        RoomDAL ObjRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
                        AllRooms = ObjRoomDAL.GetAllRoomsFromETurnsMaster(SessionHelper.CompanyID, false, false, SessionHelper.RoomList, string.Empty, SessionHelper.RoleID, SessionHelper.EnterPriceID);
                        Session["AllRooms"] = AllRooms;
                    }

                    foreach (var req in udfRequests)
                    {
                        var data = obj.GetUDFDDData(req.TableName, req.UDFUniqueID, SessionHelper.CompanyID, SessionHelper.RoomID, false, IsDeleted, "", LoadDataCount, SessionHelper.UserSupplierIds, null, null, RoomList, null, -1, null, null, AllRooms, false, null, 0, null, null, null, SessionHelper.EnterPriceID);

                        resultList.Add(new
                        {
                            DDData = data,
                            UDFColName = req.UDFName,
                            UDFUniqueID = req.UDFUniqueID
                        });
                    }
                }
            }
            return Json(resultList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUDFDDData(string TableName, string UDFUniqueID, string UDFName, bool IsArchived, bool IsDeleted, string RequisitionCurrentTab, string ItemModelCallFrom = "", bool NotIncludeDeletedUDF = false, bool IsSLCount = false, string ToolCurrentTab = "", int LoadDataCount = 0, Int64 ParentID = 0, Int64 moveType = 0)
        {
            if (LoadDataCount == 0)
            {
                //LoadDataCount = Settinfile.Element("LoadNarrowDataCount") != null ? Convert.ToInt32(Settinfile.Element("LoadNarrowDataCount").Value) : 0;
                LoadDataCount = SiteSettingHelper.LoadNarrowDataCount != string.Empty ? Convert.ToInt32(SiteSettingHelper.LoadNarrowDataCount) : 0;
            }
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            //CommonController obj = new CommonController();
            CommonDAL obj = new CommonDAL(SessionHelper.EnterPriseDBName);
            if (TableName.ToLower() == "usermaster")
            {
                List<UserNS> outlstAllPermissions = new List<UserNS>();
                Dictionary<string, int> ColUDFData = new Dictionary<string, int>();
                eTurnsMaster.DAL.UserMasterDAL objUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL();


                if (Session["AllUserPermissions"] != null && IsDeleted == false)
                {
                    outlstAllPermissions = (List<UserNS>)Session["AllUserPermissions"];
                }
                else
                {
                    if (IsDeleted)
                        outlstAllPermissions = objUserMasterDAL.GetPagedUsersNS(SessionHelper.UserID, IsDeleted);
                    else
                    {
                        Session["AllUserPermissions"] = objUserMasterDAL.GetPagedUsersNS(SessionHelper.UserID, IsDeleted);
                        outlstAllPermissions = (List<UserNS>)Session["AllUserPermissions"];
                    }

                }
                using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(SessionHelper.EnterPriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
                {

                    if (UDFUniqueID == "UDF1")
                    {
                        var localColUDFData = (from tmp in outlstAllPermissions.Where(t => t.NSName == "UDF1" && (!string.IsNullOrWhiteSpace(t.UDF)))
                                               select new
                                               {
                                                   count = tmp.UserCount,
                                                   rid = tmp.UDF,
                                                   rname = tmp.UDF
                                               }
                          );
                        ColUDFData = localColUDFData.OrderBy(t => t.rname).AsParallel().ToDictionary(e => e.rname, e => (int)e.count);
                        return Json(new { DDData = ColUDFData, UDFColName = UDFName }, JsonRequestBehavior.AllowGet);
                    }
                    if (UDFUniqueID == "UDF2")
                    {
                        var localColUDFData = (from tmp in outlstAllPermissions.Where(t => t.NSName == "UDF2" && (!string.IsNullOrWhiteSpace(t.UDF)))
                                               select new
                                               {
                                                   count = tmp.UserCount,
                                                   rid = tmp.UDF,
                                                   rname = tmp.UDF
                                               }
                          );
                        ColUDFData = localColUDFData.OrderBy(t => t.rname).AsParallel().ToDictionary(e => e.rname, e => (int)e.count);
                        return Json(new { DDData = ColUDFData, UDFColName = UDFName }, JsonRequestBehavior.AllowGet);
                    }
                    if (UDFUniqueID == "UDF3")
                    {
                        var localColUDFData = (from tmp in outlstAllPermissions.Where(t => t.NSName == "UDF3" && (!string.IsNullOrWhiteSpace(t.UDF)))
                                               select new
                                               {
                                                   count = tmp.UserCount,
                                                   rid = tmp.UDF,
                                                   rname = tmp.UDF
                                               }
                          );
                        ColUDFData = localColUDFData.OrderBy(t => t.rname).AsParallel().ToDictionary(e => e.rname, e => (int)e.count);
                        return Json(new { DDData = ColUDFData, UDFColName = UDFName }, JsonRequestBehavior.AllowGet);
                    }
                    if (UDFUniqueID == "UDF4")
                    {
                        var localColUDFData = (from tmp in outlstAllPermissions.Where(t => t.NSName == "UDF4" && (!string.IsNullOrWhiteSpace(t.UDF)))
                                               select new
                                               {
                                                   count = tmp.UserCount,
                                                   rid = tmp.UDF,
                                                   rname = tmp.UDF
                                               }
                          );
                        ColUDFData = localColUDFData.OrderBy(t => t.rname).AsParallel().ToDictionary(e => e.rname, e => (int)e.count);
                        return Json(new { DDData = ColUDFData, UDFColName = UDFName }, JsonRequestBehavior.AllowGet);
                    }
                    if (UDFUniqueID == "UDF5")
                    {
                        var localColUDFData = (from tmp in outlstAllPermissions.Where(t => t.NSName == "UDF5" && (!string.IsNullOrWhiteSpace(t.UDF)))
                                               select new
                                               {
                                                   count = tmp.UserCount,
                                                   rid = tmp.UDF,
                                                   rname = tmp.UDF
                                               }
                          );
                        ColUDFData = localColUDFData.OrderBy(t => t.rname).AsParallel().ToDictionary(e => e.rname, e => (int)e.count);
                        return Json(new { DDData = ColUDFData, UDFColName = UDFName }, JsonRequestBehavior.AllowGet);
                    }
                    if (UDFUniqueID == "UDF6")
                    {
                        var localColUDFData = (from tmp in outlstAllPermissions.Where(t => t.NSName == "UDF6" && (!string.IsNullOrWhiteSpace(t.UDF)))
                                               select new
                                               {
                                                   count = tmp.UserCount,
                                                   rid = tmp.UDF,
                                                   rname = tmp.UDF
                                               }
                          );
                        ColUDFData = localColUDFData.OrderBy(t => t.rname).AsParallel().ToDictionary(e => e.rname, e => (int)e.count);
                        return Json(new { DDData = ColUDFData, UDFColName = UDFName }, JsonRequestBehavior.AllowGet);
                    }
                    if (UDFUniqueID == "UDF7")
                    {
                        var localColUDFData = (from tmp in outlstAllPermissions.Where(t => t.NSName == "UDF7" && (!string.IsNullOrWhiteSpace(t.UDF)))
                                               select new
                                               {
                                                   count = tmp.UserCount,
                                                   rid = tmp.UDF,
                                                   rname = tmp.UDF
                                               }
                          );
                        ColUDFData = localColUDFData.OrderBy(t => t.rname).AsParallel().ToDictionary(e => e.rname, e => (int)e.count);
                        return Json(new { DDData = ColUDFData, UDFColName = UDFName }, JsonRequestBehavior.AllowGet);
                    }
                    if (UDFUniqueID == "UDF8")
                    {
                        var localColUDFData = (from tmp in outlstAllPermissions.Where(t => t.NSName == "UDF8" && (!string.IsNullOrWhiteSpace(t.UDF)))
                                               select new
                                               {
                                                   count = tmp.UserCount,
                                                   rid = tmp.UDF,
                                                   rname = tmp.UDF
                                               }
                          );
                        ColUDFData = localColUDFData.OrderBy(t => t.rname).AsParallel().ToDictionary(e => e.rname, e => (int)e.count);
                        return Json(new { DDData = ColUDFData, UDFColName = UDFName }, JsonRequestBehavior.AllowGet);
                    }
                    if (UDFUniqueID == "UDF9")
                    {
                        var localColUDFData = (from tmp in outlstAllPermissions.Where(t => t.NSName == "UDF9" && (!string.IsNullOrWhiteSpace(t.UDF)))
                                               select new
                                               {
                                                   count = tmp.UserCount,
                                                   rid = tmp.UDF,
                                                   rname = tmp.UDF
                                               }
                          );
                        ColUDFData = localColUDFData.OrderBy(t => t.rname).AsParallel().ToDictionary(e => e.rname, e => (int)e.count);
                        return Json(new { DDData = ColUDFData, UDFColName = UDFName }, JsonRequestBehavior.AllowGet);
                    }
                    if (UDFUniqueID == "UDF10")
                    {
                        var localColUDFData = (from tmp in outlstAllPermissions.Where(t => t.NSName == "UDF10" && (!string.IsNullOrWhiteSpace(t.UDF)))
                                               select new
                                               {
                                                   count = tmp.UserCount,
                                                   rid = tmp.UDF,
                                                   rname = tmp.UDF
                                               }
                          );
                        ColUDFData = localColUDFData.OrderBy(t => t.rname).AsParallel().ToDictionary(e => e.rname, e => (int)e.count);
                        return Json(new { DDData = ColUDFData, UDFColName = UDFName }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { DDData = ColUDFData, UDFColName = UDFName }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                MoveType moveTypeEnumValue;
                Enum.TryParse(moveType.ToString(), out moveTypeEnumValue);
                TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
                List<RoomDTO> RoomList = SessionHelper.RoomList;
                List<CompanyMasterDTO> CompanyList = SessionHelper.CompanyList;
                Int64 RoleID = SessionHelper.RoleID;
                List<BinMasterDTO> AllBins = null;
                if (Session["AllBins"] != null)
                {
                    AllBins = (List<BinMasterDTO>)Session["AllBins"];
                }
                else
                {
                    BinMasterDAL objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                    AllBins = objBinDAL.GetAllBins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, string.Empty, null);
                    Session["AllBins"] = AllBins;
                }
                List<CompanyMasterDTO> lstCompanies = null;
                if (Session["AllCompanies"] != null)
                {
                    lstCompanies = (List<CompanyMasterDTO>)Session["AllCompanies"];
                }
                else
                {
                    CompanyMasterDAL objCompanyDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
                    lstCompanies = objCompanyDAL.GetAllCompaniesFromETurnsMaster(false, false, SessionHelper.CompanyList, SessionHelper.RoleID).ToList();
                    Session["AllCompanies"] = lstCompanies;
                }
                Int64 Session_EnterPriceID = SessionHelper.EnterPriceID;

                List<RoomDTO> AllRooms = null;
                if (Session["AllRooms"] != null)
                {
                    AllRooms = (List<RoomDTO>)Session["AllRooms"];
                }
                else
                {
                    RoomDAL ObjRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
                    AllRooms = ObjRoomDAL.GetAllRoomsFromETurnsMaster(SessionHelper.CompanyID, false, false, SessionHelper.RoomList, string.Empty, SessionHelper.RoleID, Session_EnterPriceID);
                    Session["AllRooms"] = AllRooms;
                }

                bool Session_ConsignedAllowed = true;
                if (Session["ConsignedAllowed"] != null)
                {
                    Session_ConsignedAllowed = Convert.ToBoolean(Session["ConsignedAllowed"]);
                }

                IEnumerable<ItemMasterDTO> lstAllCountItems;
                if (Session["ItemMasterListForCount"] != null)
                {
                    lstAllCountItems = (IEnumerable<ItemMasterDTO>)Session["ItemMasterListForCount"];
                }
                else
                {
                    lstAllCountItems = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();
                    Session["ItemMasterListForCount"] = lstAllCountItems;
                }
                int? Session_QuicklistType = null;
                if (Session["QuicklistType"] != null && Convert.ToInt32(Session["QuicklistType"]) == 3)
                {
                    Session_QuicklistType = Convert.ToInt32(Session["QuicklistType"]);
                }
                IEnumerable<MinMaxDataTableInfo> SessionMinMaxTableForNarrow = null;
                if (Session["SessionMinMaxTableForNarrow"] != null)
                {
                    SessionMinMaxTableForNarrow = (IEnumerable<MinMaxDataTableInfo>)Session["SessionMinMaxTableForNarrow"];
                }
                List<ToolDetailDTO> ToolKitDetail = null;
                if (Session["ToolKitDetail"] != null)
                {
                    ToolKitDetail = (List<ToolDetailDTO>)Session["ToolKitDetail"];
                }
                string ToolType = string.Empty;
                if (Session["ToolType"] != null)
                {
                    ToolType = Convert.ToString(Session["ToolType"]);
                }
                string RequestFor = ItemModelCallFrom;
                if (ItemModelCallFrom.ToLower().Equals("credit")
                    || ItemModelCallFrom.ToLower().Equals("creditms"))
                {
                    CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                    string columnList = "ID,RoomName,IsIgnoreCreditRule";
                    RoomDTO objRoomDTO = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");

                    bool applayIsIgnoreCreditRule = false;
                    if (objRoomDTO != null)
                    {
                        applayIsIgnoreCreditRule = objRoomDTO.IsIgnoreCreditRule;
                        if (applayIsIgnoreCreditRule)
                        {
                            ItemModelCallFrom = "newpull";
                        }
                    }
                }

                var data = obj.GetUDFDDData(TableName, UDFUniqueID, SessionHelper.CompanyID, SessionHelper.RoomID, IsArchived, IsDeleted, RequisitionCurrentTab, LoadDataCount, SessionHelper.UserSupplierIds, RoomDateFormat, CurrentTimeZone, RoomList, CompanyList, RoleID, AllBins, lstCompanies, AllRooms, Session_ConsignedAllowed, lstAllCountItems, Session_QuicklistType, SessionMinMaxTableForNarrow, ToolKitDetail, ToolType, Session_EnterPriceID, ItemModelCallFrom: ItemModelCallFrom, ParentID: ParentID, moveType: moveTypeEnumValue, NotIncludeDeletedUDF: NotIncludeDeletedUDF, ToolCurrentTab: ToolCurrentTab, UserID: SessionHelper.UserID, RequestFor: RequestFor);
                return Json(new { DDData = data, UDFColName = UDFName }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// // below function used to load the User wise saved filter data for jquery data table
        /// </summary>
        /// <param name="UserID"></param> will have userid for currently logged in user
        /// <returns></returns>
        public ActionResult LoadGridState(string ListName)
        {
            ////string jsonData = @"{""iCreate"":1350639486123,""iStart"":0,""iEnd"":0,""iLength"":10,""aaSorting"":[[0,""asc"",0,""Name""]],""oSearch"":{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true},""aoSearchCols"":[{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true},{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true},{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true},{""bCaseInsensitive"":true,""sSearch"":"""",""bRegex"":false,""bSmart"":true}],""abVisCols"":[false,true,true,true,true],""ColReorder"":[0,1,3,2,4]}";
            ////UsersUISettingsController obj = new UsersUISettingsController();
            //eTurnsMaster.DAL.UsersUISettingsDAL obj = new eTurnsMaster.DAL.UsersUISettingsDAL();
            //UsersUISettingsDTO objDTO = new UsersUISettingsDTO();
            //objDTO = obj.GetRecord(SessionHelper.UserID, ListName, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);
            //string jsonData = "";
            //if (objDTO != null && !string.IsNullOrEmpty(objDTO.JSONDATA))
            //{
            //    jsonData = objDTO.JSONDATA;
            //}
            //else
            //{
            //    var isLoadEnterpriseGridOrdering = eTurnsWeb.Helper.CommonUtility.GetIsLoadEnterpriseGridOrdering();

            //    if (isLoadEnterpriseGridOrdering)
            //    {
            //        SiteListMasterDAL siteListMasterDAL = new SiteListMasterDAL(SessionHelper.EnterPriseDBName);
            //        string siteListMasterJson = siteListMasterDAL.GetSiteListMasterDataByListName(ListName);

            //        if (!string.IsNullOrEmpty(siteListMasterJson))
            //        {
            //            jsonData = siteListMasterJson;
            //            eTurns.DAL.UDFDAL obj1 = new eTurns.DAL.UDFDAL(SessionHelper.EnterPriseDBName);
            //            var tmpList = ListName;
            //            if (tmpList.ToLower() == "toollistnew")
            //            {
            //                tmpList = "ToolList";
            //            }

            //            if (tmpList == "PullMaster" || tmpList == "ItemsListForNewReceive" || tmpList == "ToolHistoryList"
            //                || tmpList == "AssetToolSchedulerList" || tmpList == "KitToolMasterList" || tmpList == "EnterpriseList"
            //                || tmpList == "BomCategoryMasterList" || tmpList == "BomGLAccountMasterList"
            //                || tmpList == "BomInventoryClassificationMasterList"
            //                || tmpList == "BomManufacturerMasterList" || tmpList == "BomSupplierMasterList"
            //                || tmpList == "UnitMasterList" || tmpList == "BomUnitMasterList" || tmpList == "MinMaxTuningTable")
            //            {
            //                IEnumerable<UDFDTO> DataFromDB = null;
            //                int tmpTotalRecordCount = 0;
            //                var udfTables = GetUDFTableNamesByListName(tmpList);
            //                int totalUDFCount = 0;

            //                foreach (var tableName in udfTables)
            //                {
            //                    DataFromDB = obj1.GetPagedRecords(0, 10, out tmpTotalRecordCount, string.Empty, "ID asc", SessionHelper.CompanyID, tableName, SessionHelper.RoomID);

            //                    if (DataFromDB != null && DataFromDB.Count() > 0)
            //                    {
            //                        totalUDFCount += DataFromDB.Where(c => c.UDFControlType != null && c.IsDeleted == false).Count();
            //                    }
            //                }

            //                if (totalUDFCount > 0)
            //                {
            //                    jsonData = UpdateJson(siteListMasterJson, ListName, totalUDFCount);
            //                }
            //            }
            //            else
            //            {
            //                eTurnsMaster.DAL.CommonMasterDAL objUDF = new eTurnsMaster.DAL.CommonMasterDAL();
            //                Dictionary<int, string> GridListName = objUDF.GetUDfTableNameByListName(tmpList);
            //                int totalUDFCount = 0;

            //                if (GridListName != null && GridListName.Count() > 0)
            //                {
            //                    foreach (KeyValuePair<int, string> ReportResourceFileName in GridListName)
            //                    {
            //                        string[] Values = ReportResourceFileName.Value.Split('$');
            //                        if (Values != null && Values.Count() > 0)
            //                        {
            //                            int udfCount = 0;
            //                            IEnumerable<UDFDTO> UDFDataFromDB = null;
            //                            int TotalRecordCount = 0;
            //                            UDFDataFromDB = obj1.GetPagedRecords(0, 10, out TotalRecordCount, string.Empty, "ID asc", SessionHelper.CompanyID, Values[0], SessionHelper.RoomID);
            //                            if (UDFDataFromDB != null && UDFDataFromDB.Count() > 0)
            //                            {
            //                                udfCount = UDFDataFromDB.Where(c => c.UDFControlType != null && c.IsDeleted == false).Count();
            //                            }

            //                            if (tmpList == "CartItemList" || tmpList == "ReceiveMasterList")
            //                            {
            //                                Values[1] = "Yes";
            //                                Values[2] = "ItemMaster";
            //                            }

            //                            int ExtraUDFinGrid = 0;

            //                            if (Values[1] == "Yes")
            //                            {
            //                                IEnumerable<UDFDTO> DataFromDB = null;
            //                                int TotalRecordCountExtra = 0;
            //                                DataFromDB = obj1.GetPagedRecords(0, 10, out TotalRecordCountExtra, string.Empty, "ID asc", SessionHelper.CompanyID, Values[2], SessionHelper.RoomID);
            //                                if (DataFromDB != null && DataFromDB.Count() > 0)
            //                                {
            //                                    ExtraUDFinGrid = DataFromDB.Where(c => c.UDFControlType != null && c.IsDeleted == false).Count();
            //                                }
            //                            }
            //                            totalUDFCount += udfCount + ExtraUDFinGrid + Convert.ToInt32(Values[3]);

            //                            if (totalUDFCount > 0)
            //                            {
            //                                jsonData = UpdateJson(siteListMasterJson, ListName, totalUDFCount);
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            string jsonData = "";

            using (MasterBAL masterBAL = new MasterBAL())
            {
                jsonData = masterBAL.getGridState(ListName);
            }

            return Json(new { jsonData }, JsonRequestBehavior.AllowGet);
        }



        ///// <summary>
        ///// This method is used to update grid json by adding grid's UDF
        ///// </summary>
        ///// <param name="Json"></param>
        ///// <param name="ListName"></param>
        ///// <param name="UDFCount"></param>
        ///// <returns></returns>
        //private string UpdateJson(string Json, string ListName, int UDFCount)
        //{
        //    string updatedJSON = string.Empty;

        //    if (!string.IsNullOrEmpty(Json))
        //    {
        //        if (!string.IsNullOrEmpty(Json))
        //        {
        //            JObject gridStateJS = new Newtonsoft.Json.Linq.JObject();
        //            // jsonData = objDTO.JSONDATA;
        //            /*////////CODE FOR UPDATE JSON STRING/////////*/
        //            // JObject gridStaeJS = new JObject();
        //            gridStateJS = JObject.Parse(Json);
        //            /*////////CODE FOR UPDATE JSON STRING/////////*/

        //            JToken orderCols = gridStateJS["ColReorder"];
        //            JArray arrOCols = (JArray)orderCols;
        //            JArray arrONewCols = new JArray();

        //            if (arrOCols != null)
        //            {
        //                int orderClength = arrOCols.Count;

        //                if (orderClength > 4)
        //                {
        //                    JToken abVisCols = gridStateJS["abVisCols"];
        //                    JArray visCols = (JArray)abVisCols;
        //                    JToken aoSearchCols = gridStateJS["aoSearchCols"];
        //                    JArray arrSCols = (JArray)aoSearchCols;

        //                    if (arrSCols != null)
        //                    {
        //                        JObject UpdateAccProfile = new JObject(
        //                                new JProperty("bCaseInsensitive", true),
        //                                new JProperty("sSearch", ""),
        //                                new JProperty("bRegex", false),
        //                                new JProperty("bSmart", true));
        //                        for (int count = 0; count < UDFCount; count++)
        //                        {
        //                            arrSCols.Add((object)UpdateAccProfile);
        //                        }
        //                    }

        //                    if (visCols != null)
        //                    {
        //                        for (int count = 0; count < UDFCount; count++)
        //                        {
        //                            visCols.Add(true);
        //                        }
        //                    }

        //                    JToken widthCols = gridStateJS["ColWidth"];
        //                    JArray arrWCols = (JArray)widthCols;

        //                    if (arrWCols != null)
        //                    {
        //                        for (int count = 0; count < UDFCount; count++)
        //                        {
        //                            arrWCols.Insert(arrWCols.Count, "100px");
        //                        }
        //                    }

        //                    int maxOrder = arrOCols.Select(c => (int)c).ToList().Max();
        //                    long currentUDFVAl = maxOrder + 1;

        //                    for (int count = 0; count < UDFCount; count++)
        //                    {
        //                        arrOCols.Insert(arrOCols.Count, currentUDFVAl + count);
        //                    }

        //                    gridStateJS["ColReorder"] = arrOCols;
        //                    updatedJSON = gridStateJS.ToString();

        //                    /*/////////////CODE FOR SAVE DATA IN GRID STATE//////////////*/
        //                    //objDTO = new UsersUISettingsDTO();
        //                    //objDTO.UserID = SessionHelper.UserID;

        //                    //objDTO.EnterpriseID = SessionHelper.EnterPriceID;
        //                    //objDTO.CompanyID = SessionHelper.CompanyID;
        //                    //objDTO.RoomID = SessionHelper.RoomID;

        //                    //objDTO.JSONDATA = updatedJSON;
        //                    //objDTO.ListName = ListName;
        //                    //obj.SaveUserListViewSettings(objDTO);
        //                    /*/////////////CODE FOR SAVE DATA IN GRID STATE//////////////*/
        //                }
        //            }
        //        }
        //    }
        //    return updatedJSON;
        //}

        private string UpdateJsonForRoom(string Json, string ListName, int CurrentRoomUDFCount, int UpdateRoomUDFCount, long EnterpriseId, long CompanyId, long RoomId)
        {
            string updatedJSON = Json;
            eTurnsMaster.DAL.UsersUISettingsDAL obj = new eTurnsMaster.DAL.UsersUISettingsDAL();
            UsersUISettingsDTO objDTO = new UsersUISettingsDTO();

            if (!string.IsNullOrEmpty(Json))
            {
                JObject gridStateJS = new Newtonsoft.Json.Linq.JObject();
                // jsonData = objDTO.JSONDATA;
                /*////////CODE FOR UPDATE JSON STRING/////////*/
                // JObject gridStaeJS = new JObject();
                gridStateJS = JObject.Parse(Json);
                /*////////CODE FOR UPDATE JSON STRING/////////*/

                JToken orderCols = gridStateJS["ColReorder"];
                JArray arrOCols = (JArray)orderCols;
                JArray arrONewCols = new JArray();

                if (arrOCols != null)
                {
                    int orderClength = arrOCols.Count;

                    if (orderClength > 4)
                    {
                        JToken abVisCols = gridStateJS["abVisCols"];
                        JArray visCols = (JArray)abVisCols;
                        JToken aoSearchCols = gridStateJS["aoSearchCols"];
                        JArray arrSCols = (JArray)aoSearchCols;
                        JToken widthCols = gridStateJS["ColWidth"];
                        JArray arrWCols = (JArray)widthCols;
                        Dictionary<int, int> currentRoomUdfColumnOrder = new Dictionary<int, int>();

                        if (CurrentRoomUDFCount > 0)
                        {
                            int columnOrderMax = arrOCols.Select(c => (int)c).ToList().Max();
                            long UDFStartPosition = columnOrderMax - (CurrentRoomUDFCount);

                            for (int i = 0; i < orderClength; i++)
                            {
                                if (Convert.ToInt32(((JValue)(arrOCols[i])).Value) > UDFStartPosition)
                                {
                                    currentRoomUdfColumnOrder.Add(i, Convert.ToInt32(((JValue)(arrOCols[i])).Value));
                                }
                            }

                            int deleteCount = 0;
                            foreach (KeyValuePair<int, int> entry in currentRoomUdfColumnOrder)
                            {
                                if (arrOCols != null)
                                {
                                    arrOCols.RemoveAt((entry.Key - deleteCount));
                                }

                                //visCols.RemoveAt((entry.Value - deleteCount));
                                if (arrSCols != null)
                                {
                                    arrSCols.RemoveAt((entry.Key - deleteCount));
                                }
                                if (arrWCols != null)
                                {
                                    arrWCols.RemoveAt((entry.Key - deleteCount));
                                }
                                deleteCount++;
                            }
                            int tmpdeleteCount = 0;
                            foreach (KeyValuePair<int, int> entry in currentRoomUdfColumnOrder.OrderBy(e => e.Value))
                            {
                                if (visCols != null)
                                {
                                    visCols.RemoveAt((entry.Value - tmpdeleteCount));
                                }
                                tmpdeleteCount++;
                            }
                        }

                        if (UpdateRoomUDFCount > 0)
                        {
                            int udfColumnsToAdd = UpdateRoomUDFCount;

                            if (CurrentRoomUDFCount > 0 && currentRoomUdfColumnOrder.Any())
                            {
                                JObject UpdateAccProfile = new JObject(
                                        new JProperty("bCaseInsensitive", true),
                                        new JProperty("sSearch", ""),
                                        new JProperty("bRegex", false),
                                        new JProperty("bSmart", true));
                                //Note: when source room udf columns > destination room's udf column then add column with maxorder++ and in sequence of entry.Key

                                if (CurrentRoomUDFCount > 0 && CurrentRoomUDFCount > UpdateRoomUDFCount)
                                {
                                    int columnOrderMaxForUpdatingRoom = arrOCols.Select(c => (int)c).ToList().Max();
                                    columnOrderMaxForUpdatingRoom += 1;
                                    int counter = 0;

                                    //int tmpudfColumnsToAdd = udfColumnsToAdd;

                                    foreach (KeyValuePair<int, int> entry in currentRoomUdfColumnOrder)
                                    {
                                        if (udfColumnsToAdd > 0)
                                        {
                                            if (arrSCols != null)
                                            {
                                                arrSCols.Add((object)UpdateAccProfile);
                                            }
                                            if (visCols != null)
                                            {
                                                visCols.Insert(columnOrderMaxForUpdatingRoom + counter, true);
                                            }
                                            if (arrWCols != null)
                                            {
                                                arrWCols.Insert(entry.Key, "100px");
                                            }
                                            if (arrOCols != null)
                                            {
                                                arrOCols.Insert(entry.Key, columnOrderMaxForUpdatingRoom + counter);
                                            }
                                            udfColumnsToAdd--;
                                            counter++;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }

                                    //int visColsCounter = 0;
                                    //foreach (KeyValuePair<int, int> entry in currentRoomUdfColumnOrder)
                                    //{
                                    //    if (tmpudfColumnsToAdd > 0)
                                    //    {
                                    //        visCols.Insert(columnOrderMaxForUpdatingRoom + visColsCounter, true);
                                    //        tmpudfColumnsToAdd--;
                                    //        visColsCounter++;
                                    //    }
                                    //    else
                                    //    {
                                    //        break;
                                    //    }
                                    //}
                                }
                                else
                                {
                                    int tmpudfColumnsToAdd = udfColumnsToAdd;
                                    foreach (KeyValuePair<int, int> entry in currentRoomUdfColumnOrder)
                                    {
                                        if (udfColumnsToAdd > 0)
                                        {
                                            arrSCols.Add((object)UpdateAccProfile);
                                            //visCols.Insert(entry.Value, true);
                                            arrWCols.Insert(entry.Key, "100px");
                                            arrOCols.Insert(entry.Key, entry.Value);
                                            udfColumnsToAdd--;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }

                                    foreach (KeyValuePair<int, int> entry in currentRoomUdfColumnOrder.OrderBy(e => e.Value))
                                    {
                                        if (tmpudfColumnsToAdd > 0)
                                        {
                                            visCols.Insert(entry.Value, true);
                                            tmpudfColumnsToAdd--;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }

                                    if (udfColumnsToAdd > 0)
                                    {
                                        int columnOrderMaxForUpdatingRoom = arrOCols.Select(c => (int)c).ToList().Max();
                                        columnOrderMaxForUpdatingRoom += 1;

                                        for (int count = 0; count < udfColumnsToAdd; count++)
                                        {
                                            arrSCols.Add((object)UpdateAccProfile);
                                            visCols.Insert(columnOrderMaxForUpdatingRoom + count, true);
                                            arrWCols.Insert(arrWCols.Count, "100px");
                                            arrOCols.Insert(arrOCols.Count, columnOrderMaxForUpdatingRoom + count);
                                        }
                                    }
                                }
                            }
                        }

                        gridStateJS["aoSearchCols"] = arrSCols;
                        gridStateJS["abVisCols"] = visCols;
                        gridStateJS["ColWidth"] = arrWCols;
                        gridStateJS["ColReorder"] = arrOCols;
                        updatedJSON = gridStateJS.ToString();

                        /*/////////////CODE FOR SAVE DATA IN GRID STATE//////////////*/
                        objDTO = new UsersUISettingsDTO();
                        objDTO.UserID = SessionHelper.UserID;
                        objDTO.EnterpriseID = EnterpriseId;
                        objDTO.CompanyID = CompanyId;
                        objDTO.RoomID = RoomId;
                        objDTO.JSONDATA = updatedJSON;
                        objDTO.ListName = ListName;
                        obj.SaveUserListViewSettings(objDTO, SiteSettingHelper.UsersUISettingType, true);
                        /*/////////////CODE FOR SAVE DATA IN GRID STATE//////////////*/
                    }
                }
            }

            return updatedJSON;
        }
        /// <summary>
        /// // below function used to Save the User wise filter data for jquery data table
        /// </summary>
        /// <param name="UserID"></param> will have userid for currently logged in user
        /// <param name="Data"></param> will have json string of filter criteria 
        /// <returns></returns>
        public ActionResult SaveGridState(string Data, string ListName)
        {
            //UsersUISettingsController obj = new UsersUISettingsController();
            eTurnsMaster.DAL.UsersUISettingsDAL obj = new eTurnsMaster.DAL.UsersUISettingsDAL();
            UsersUISettingsDTO objDTO = new UsersUISettingsDTO();
            objDTO.UserID = SessionHelper.UserID;

            objDTO.EnterpriseID = SessionHelper.EnterPriceID;
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.RoomID = SessionHelper.RoomID;
            objDTO.JSONDATA = Data;
            objDTO.ListName = ListName;
            obj.SaveUserListViewSettings(objDTO, SiteSettingHelper.UsersUISettingType, true);
            return Json(new { objDTO.JSONDATA }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult GetHistoryData(JQueryDataTableParamModel param)
        {
            //LoadTestEntities entity = new LoadTestEntities();
            int PageIndex = 0;
            if (param.sEcho == null)
                PageIndex = 0;
            else
                PageIndex = int.Parse(param.sEcho);

            int PageSize = 100; //param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            string PageName = Request["PageType"].ToString();
            Int64 ID = Int64.Parse(Request["PageID"].ToString());

            Int64 EnterpriseID = 0;
            if (Request["EnterpriseID"] != null)
            {
                EnterpriseID = Int64.Parse(Request["EnterpriseID"].ToString());
            }
            Int64 CompanyID = 0;
            if (Request["CompanyID"] != null)
            {
                CompanyID = Int64.Parse(Request["CompanyID"].ToString());
            }

            string sDirection = string.Empty;
            int StartWith = (PageSize - param.iDisplayStart) + 1;
            //sortColumnName = Request["PageType"].ToString();

            int TotalRecordCount = 0;
            IEnumerable<object> DataFromDB = null;
            if (PageName.ToLower() == "enterpriselist")
            {
                CommonMasterDAL objCommon = new CommonMasterDAL();
                DataFromDB = objCommon.GetPagedEnterpriseMasterHistory(param.iDisplayStart, PageSize, out TotalRecordCount, ID);
            }
            else
            {
                CommonDAL objCommon;
                if (EnterpriseID != SessionHelper.EnterPriceID && EnterpriseID > 0)
                {
                    string EnterpriseDBName = new EnterpriseDAL(SessionHelper.EnterPriseDBName).GetEnterpriseDBName(EnterpriseID);
                    objCommon = new CommonDAL(EnterpriseDBName);
                }
                else
                {
                    objCommon = new CommonDAL(SessionHelper.EnterPriseDBName);
                }

                DataFromDB = objCommon.GetPagedRecords(param.iDisplayStart, PageSize, out TotalRecordCount, param.sSearch, PageName, ID, CompanyID, EnterpriseID);
            }

            if (DataFromDB != null)
            {
                DataFromDB.ToList().ForEach(t =>
                {

                    object GetvalC = t.GetType().GetProperty("Created").GetValue(t, null);
                    object GetvalU = null;
                    if (t.GetType().GetProperty("Updated") != null)
                        GetvalU = t.GetType().GetProperty("Updated").GetValue(t, null);
                    else if (t.GetType().GetProperty("LastUpdated") != null)
                        GetvalU = t.GetType().GetProperty("LastUpdated").GetValue(t, null);

                    if (GetvalC != null)
                    {
                        string sc = CommonUtility.ConvertDateByTimeZone((DateTime)GetvalC, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                        t.GetType().GetProperty("CreatedDate").SetValue(t, sc, null);
                    }
                    if (GetvalU != null)
                    {
                        string su = CommonUtility.ConvertDateByTimeZone((DateTime)GetvalU, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                        t.GetType().GetProperty("UpdatedDate").SetValue(t, su, null);
                    }
                });
            }


            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);


        }

        public ActionResult GetHistoryDataGUID(JQueryDataTableParamModel param)
        {
            int PageIndex = 0;
            if (param.sEcho == null)
                PageIndex = 0;
            else
                PageIndex = int.Parse(param.sEcho);

            int PageSize = 100; //param.iDisplayLength;
            var sortDirection = Request["sSortDir_0"];
            var isNameSortable = Convert.ToBoolean(Request["bSortable_1"]);
            var isAddressSortable = Convert.ToBoolean(Request["bSortable_2"]);
            var isTownSortable = Convert.ToBoolean(Request["bSortable_3"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortColumnName = string.Empty;
            string PageName = Request["PageType"].ToString();
            //Int64 ID = int.Parse(Request["PageID"].ToString());
            string sDirection = string.Empty;
            int StartWith = (PageSize - param.iDisplayStart) + 1;
            //sortColumnName = Request["PageType"].ToString();

            int TotalRecordCount = 0;
            CommonDAL objCommon = new CommonDAL(SessionHelper.EnterPriseDBName);

            var DataFromDB = objCommon.GetPagedRecordsGUID(param.iDisplayStart, PageSize, out TotalRecordCount, param.sSearch, PageName, Request["PageID"].ToString());
            DataFromDB.ToList().ForEach(t =>
            {
                //t.CreatedDate = CommonUtility.ConvertDateByTimeZone(t.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                //t.UpdatedDate = CommonUtility.ConvertDateByTimeZone(t.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                object SetvalueC = t.GetType().GetProperty("CreatedDate").GetValue(t, null);
                object SetvalueU = t.GetType().GetProperty("UpdatedDate").GetValue(t, null);

                object GetvalueC = t.GetType().GetProperty("Created").GetValue(t, null);

                if (GetvalueC != null)
                {
                    string sc = CommonUtility.ConvertDateByTimeZone((DateTime)GetvalueC, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                    t.GetType().GetProperty("CreatedDate").SetValue(t, sc, null);
                }


                object GetvalueU = null;
                if (t.GetType().GetProperty("Updated") != null)
                    GetvalueU = t.GetType().GetProperty("Updated").GetValue(t, null);
                else if (t.GetType().GetProperty("LastUpdated") != null)
                    GetvalueU = t.GetType().GetProperty("LastUpdated").GetValue(t, null);

                if (GetvalueU != null)
                {
                    string su = CommonUtility.ConvertDateByTimeZone((DateTime)GetvalueU, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                    t.GetType().GetProperty("UpdatedDate").SetValue(t, su, null);
                }


                object GetvalueCountDate = null;
                if (t.GetType().GetProperty("CountDate") != null)
                    GetvalueCountDate = t.GetType().GetProperty("CountDate").GetValue(t, null);

                if (GetvalueCountDate != null)
                {
                    t.GetType().GetProperty("InventoryCountDate").SetValue(t, FnCommon.ConvertDateByTimeZone((DateTime)GetvalueCountDate, false, true), null);
                }
            });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);
        }

        public ActionResult HistoryList()
        {
            return PartialView("_HistoryList");
        }

        public ActionResult TechnicianHistory()
        {
            return PartialView("_TechnicianHistory");
        }

        public ActionResult BinHistory()
        {
            return PartialView("_BinHistory");
        }

        public ActionResult CategoryHistory()
        {
            return PartialView("_CategoryHistory");
        }

        public ActionResult CostUOMHistory()
        {
            return PartialView("_CostUOMHistory");
        }

        public ActionResult InventoryClassificationHistory()
        {
            return PartialView("_InventoryClassificationHistory");
        }

        public ActionResult FreightTypeHistory()
        {
            return PartialView("_FreightTypeHistory");
        }

        public ActionResult GLAccountHistory()
        {
            return PartialView("_GLAccountHistory");
        }

        public ActionResult GXPRConsignedJobHistory()
        {
            return PartialView("_GXPRConsignedJobHistory");
        }

        public ActionResult JobTypeHistory()
        {
            return PartialView("_JobTypeHistory");
        }

        public ActionResult LocationHistory()
        {
            return PartialView("_LocationHistory");
        }

        public ActionResult ManufacturerHistory()
        {
            return PartialView("_ManufacturerHistory");
        }
        public ActionResult WrittenOffCategoryHistory()
        {
            return PartialView("_WrittenOffCategoryHistory");
        }

        public ActionResult MeasurementTermHistory()
        {
            return PartialView("_MeasurementTermHistory");
        }

        public ActionResult ShipViaHistory()
        {
            return PartialView("_ShipViaHistory");
        }

        public ActionResult SupplierHistory()
        {
            return PartialView("_SupplierHistory");
        }

        public ActionResult ToolCategoryHistory()
        {
            return PartialView("_ToolCategoryHistory");
        }

        public ActionResult AssetCategoryHistory()
        {
            return PartialView("_AssetCategoryHistory");
        }
        public ActionResult ToolHistory()
        {
            return PartialView("~/Views/Assets/_ToolHistory.cshtml");
        }

        public ActionResult AssetHistory()
        {
            return PartialView("~/Views/Assets/_AssetHistory.cshtml");
        }

        public ActionResult SchedulerHistory()
        {
            return PartialView("~/Views/Assets/_SchedulerHistory.cshtml");
        }

        public ActionResult UnitHistory()
        {
            return PartialView("_UnitHistory");
        }

        public ActionResult CustomerHistory()
        {
            return PartialView("_CustomerHistory");
        }

        public ActionResult FTPHistory()
        {
            return PartialView("_FTPHistory");
        }

        public ActionResult RoomHistory()
        {
            return PartialView("_RoomHistory");
        }

        public ActionResult CompanyHistory()
        {
            return PartialView("_CompanyHistory");
        }

        public ActionResult EnterpriseHistory()
        {
            return PartialView("_EnterpriseHistory");
        }

        public ActionResult PullHistory()
        {
            return PartialView("~/Views/Pull/_PullHistory.cshtml");
        }

        public ActionResult QuickListHistory()
        {
            return PartialView("~/Views/QuickList/_QuickListHistory.cshtml");
        }

        public ActionResult ItemHistory(Guid ItemGUID)
        {
            ViewBag.ItemGuid = ItemGUID;
            return PartialView("~/Views/Inventory/_ItemHistory.cshtml");
        }

        public ActionResult MaterialStagingHistory()
        {
            return PartialView("~/Views/Inventory/_MaterialStagingHistory.cshtml");
        }

        public ActionResult RequisitionMHistory()
        {
            return PartialView("~/Views/Consume/_RequisitionMHistory.cshtml");
        }

        public ActionResult WOMHistory()
        {
            return PartialView("~/Views/WorkOrder/_WOMHistory.cshtml");
        }

        public ActionResult VenderMasterHistory()
        {
            return PartialView("_VenderHistory");
        }
        #endregion

        #region "EmailUserConfiguration"
        public ActionResult EmailUserConfiguration()
        {
            List<SelectListItem> lstLanguage = GetLanguage();
            ViewBag.DDLanguage = lstLanguage;
            //List<SelectListItem> lstEmailTemp = GetEmailTemplateList(lstLanguage[0].Value);
            EmailTemplateDAL objEmailTemplate = new EmailTemplateDAL(SessionHelper.EnterPriseDBName);
            List<EmailTemplateDTO> lstEmailTemp = objEmailTemplate.GetAllEmailTemplateUC();
            ViewBag.DDLEmailTemplate = lstEmailTemp;
            return View();
        }
        public JsonResult SaveEmailTemplateUser(EmailUserConfigurationDTO objDTO)
        {
            string message = "";
            EmailUserConfigurationDAL objData = new EmailUserConfigurationDAL(SessionHelper.EnterPriseDBName);
            objDTO.RoomId = SessionHelper.RoomID;
            objDTO.CompanyId = SessionHelper.CompanyID;
            objDTO.CreatedOn = DateTimeUtility.DateTimeNow;
            var result = objData.Insert(objDTO);
            if (result != 0)
            {
                message = ResMessage.SaveMessage; //ResMessage.SaveMessage; //"Record Saved Sucessfully...";
            }
            else
            {
                message = string.Format(ResMessage.DuplicateMessage, ResEmailUserConfiguration.Email, objDTO.Email);
                //message = ResMessage.DuplicateMessage;
            }
            return Json(new { Message = message }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveUserInTemplate(string EmailTemplateName, string para = "")
        {
            string message = "";
            string status = "";
            JavaScriptSerializer s = new JavaScriptSerializer();
            try
            {
                List<EmailUserMasterDetailDTO> LstEmailUserMasterDetail = new List<EmailUserMasterDetailDTO>();
                Int32[] IDArray = s.Deserialize<Int32[]>(para);
                foreach (Int32 item in IDArray)
                {
                    EmailUserMasterDetailDTO obj = new EmailUserMasterDetailDTO();
                    obj.Name = EmailTemplateName;
                    obj.UserID = item;

                    LstEmailUserMasterDetail.Add(obj);
                }
                EmailUserConfigurationDAL objEmail = new EmailUserConfigurationDAL(SessionHelper.EnterPriseDBName);
                objEmail.InsertUserInTemplate(LstEmailUserMasterDetail, SessionHelper.RoomID, SessionHelper.CompanyID);
                message = ResMessage.SaveMessage;
                status = "ok";
            }
            catch (Exception)
            {
                message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);// "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                status = "fail";
            }
            finally
            {
                // resHelper = null;
            }
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }
        public string UpdateUserTemplate(Int64 id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            EmailUserConfigurationDAL obj = new EmailUserConfigurationDAL(SessionHelper.EnterPriseDBName);
            obj.UpdateEmailUserMaster(id, value, columnName);
            return value;
        }
        [HttpPost]
        public JsonResult DeleteUserFromTemplate(Int64 UserID)
        {
            string message = "";
            string status = "";
            JavaScriptSerializer s = new JavaScriptSerializer();
            try
            {
                //CommonDAL obj = new CommonDAL(SessionHelper.EnterPriseDBName);
                //message = obj.DeleteRecords("EmailUserMaster", UserID.ToString(), SessionHelper.RoomID,SessionHelper.CompanyID);

                EmailUserConfigurationDAL obj = new EmailUserConfigurationDAL(SessionHelper.EnterPriseDBName);
                bool isDeleted = obj.Delete(UserID);
                return Json(new { Message = "ok", Status = "ok" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);// "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                // resHelper = null;
            }

        }
        [HttpPost]
        public JsonResult DeleteEmailUserConfig(int id)
        {
            if (ModelState.IsValid)
            {
                EmailUserConfigurationDAL obj = new EmailUserConfigurationDAL(SessionHelper.EnterPriseDBName);
                if (obj.DeleteEmailUserConfig(id))
                    return Json("Success");
            }

            return Json("Fail");
        }
        #endregion

        #region "EmailConfiguration"
        public ActionResult EmailConfiguration()
        {
            List<SelectListItem> lstLanguage = GetLanguage();
            ViewBag.DDLanguage = lstLanguage;
            //List<SelectListItem> lstEmailTemp = GetEmailTemplateList(lstLanguage[0].Value);
            EmailTemplateDAL objEmailTemplate = new EmailTemplateDAL(SessionHelper.EnterPriseDBName);
            List<EmailTemplateDTO> lstEmailTemp = objEmailTemplate.GetAllEmailTemplate();
            ViewBag.DDLEmailTemplate = lstEmailTemp;
            return View();
        }
        public ActionResult GetEmailUserDetail(JQueryDataTableParamModel param)
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
            string EmailTemplateName = Request["EmailTemplateName"].ToString();
            bool IsSelectedOnly = bool.Parse(Request["chkselectedonly"].ToString());

            // set the default column sorting here, if first time then required to set 
            if (sortColumnName == "0" || sortColumnName == "undefined")
                sortColumnName = "ID";

            if (sortDirection == "asc")
                sortColumnName = sortColumnName + " asc";
            else
                sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;



            EmailUserConfigurationDAL objData = new EmailUserConfigurationDAL(SessionHelper.EnterPriseDBName);
            //var result = objData.GetAllRecords(EmailTemplateName, IsSelectedOnly);//.Where(x => x.EmailTemplateName == EmailTemplateName);
            var result = objData.GetAllEmailUserMaster(EmailTemplateName, IsSelectedOnly, SessionHelper.CompanyID, SessionHelper.RoomID);//.Where(x => x.EmailTemplateName == EmailTemplateName);
            //return Json(new { Result = result }, JsonRequestBehavior.AllowGet);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = result.Count(),
                iTotalDisplayRecords = result.Count(), //filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// GetResourceFiles
        /// </summary>
        /// <param name="ModuleID"></param>
        /// <returns></returns>
        public List<SelectListItem> GetEmailTemplateList(string CurCulture)
        {
            List<SelectListItem> lstItem = null;// new List<SelectListItem>();
            try
            {
                // CultureInfo str = eTurns.DTO.Resources.ResourceHelper.CurrentCult;

                DirectoryInfo drInfo = null;
                drInfo = new DirectoryInfo(Server.MapPath("../Content/EmailTemplates"));
                if (drInfo.Exists)
                {
                    lstItem = new List<SelectListItem>();
                    foreach (FileInfo objFileInfo in drInfo.GetFiles("*" + CurCulture + "*"))
                    {
                        SelectListItem obj = new SelectListItem();
                        obj.Text = objFileInfo.Name.Split('-')[0].ToString(); //objFileInfo.Name;
                        obj.Value = objFileInfo.Name.Split('-')[0].ToString();
                        lstItem.Add(obj);
                    }
                }
                if (lstItem != null && lstItem.Count > 0)
                    return lstItem;
                else
                    return new List<SelectListItem>();
            }
            catch (Exception ex)
            {
                return lstItem;
                throw ex;
            }
            finally
            {
                lstItem = null;
            }

        }
        ////public List<SelectListItem> GetEmailTemplateList(string CurCulture)
        ////{
        ////    List<SelectListItem> lstItem = null;// new List<SelectListItem>();
        ////    try
        ////    {
        ////        // CultureInfo str = eTurns.DTO.Resources.ResourceHelper.CurrentCult;
        ////        List<EmailTemplateDTO> lstEmailTemplateDTO = new List<EmailTemplateDTO>();
        ////        EmailTemplate objEmailTemplate=new EmailTemplate();
        ////        lstEmailTemplateDTO=objEmailTemplate.GetAllEmailTemplate();

        ////        ////DirectoryInfo drInfo = null;
        ////        ////drInfo = new DirectoryInfo(Server.MapPath("../Content/EmailTemplates"));
        ////        ////if (drInfo.Exists)
        ////        ////{
        ////        ////    lstItem = new List<SelectListItem>();
        ////        ////    foreach (FileInfo objFileInfo in drInfo.GetFiles("*" + CurCulture + "*"))
        ////        ////    {
        ////        ////        SelectListItem obj = new SelectListItem();
        ////        ////        obj.Text = objFileInfo.Name.Split('-')[0].ToString(); //objFileInfo.Name;
        ////        ////        obj.Value = objFileInfo.Name.Split('-')[0].ToString();
        ////        ////        lstItem.Add(obj);
        ////        ////    }
        ////        ////}

        ////            lstItem = new List<SelectListItem>();
        ////            foreach (EmailTemplateDTO item in lstEmailTemplateDTO)
        ////            {
        ////                SelectListItem obj = new SelectListItem();
        ////                obj.Text = item.TemplateName ; //objFileInfo.Name;
        ////                obj.Value =Convert.ToString( item.ID );
        ////                lstItem.Add(obj);
        ////            }

        ////        if (lstItem != null && lstItem.Count > 0)
        ////            return lstItem;
        ////        else
        ////            return new List<SelectListItem>();
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        return lstItem;
        ////        throw ex;
        ////    }
        ////    finally
        ////    {
        ////        lstItem = null;
        ////    }

        ////}
        /// <summary>
        /// GetResourceFiles
        /// </summary>
        /// <param name="ModuleID"></param>
        /// <returns></returns>
        public JsonResult GetEmailTemplate(string CurCulture)
        {
            List<SelectListItem> lstItem = null;// new List<SelectListItem>();

            try
            {
                lstItem = GetEmailTemplateList(CurCulture);

                return Json(lstItem, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(lstItem, JsonRequestBehavior.AllowGet);
                throw ex;
            }
            finally
            {
                lstItem = null;
            }



        }
        public JsonResult GetEmailTemplateText(string EmailTemplateName, string CurCulture)
        {
            StringBuilder MessageBody = new StringBuilder();
            string EmailSubject = string.Empty;
            //if (EmailTemplateName.Contains("OrderApproval") && CurCulture == "en-US")
            //{
            //    MessageBody.Append(System.IO.File.ReadAllText(Server.MapPath("../") + "Content\\EmailTemplates/" + EmailTemplateName + "-" + CurCulture + ".xslt"));
            //}
            //else
            long TemplateId = GetTemplateId(EmailTemplateName);
            EmailTemplateDAL objEmailTemplate = new EmailTemplateDAL(SessionHelper.EnterPriseDBName);
            EmailTemplateDetail objEmailTempateDetail = new EmailTemplateDetail();

            objEmailTempateDetail = objEmailTemplate.GetTemplateDetail(TemplateId, GetLanguageId(CurCulture), SessionHelper.CompanyID, SessionHelper.RoomID);
            if (objEmailTempateDetail != null && objEmailTempateDetail.EmailTemplateId > 0)
            {
                MessageBody.Append(Convert.ToString(objEmailTempateDetail.MailBodyText));
                EmailSubject = objEmailTempateDetail.MailSubject;
            }
            else
            {
                objEmailTempateDetail = objEmailTemplate.GetTemplateDetail(TemplateId, GetLanguageId(CurCulture), 0, 0);
                if (objEmailTempateDetail != null && objEmailTempateDetail.EmailTemplateId > 0)
                {
                    MessageBody.Append(Convert.ToString(objEmailTempateDetail.MailBodyText));
                    EmailSubject = objEmailTempateDetail.MailSubject;
                }
                //if (System.IO.File.Exists(Server.MapPath("../") + "Content\\EmailTemplates\\" + EmailTemplateName + "-" + CurCulture + ".html"))
                //{
                //    MessageBody.Append(System.IO.File.ReadAllText(Server.MapPath("../") + "Content\\EmailTemplates/" + EmailTemplateName + "-" + CurCulture + ".html"));
                //}

            }

            return Json(new { MessageBody = MessageBody.ToString(), EmailSubject = EmailSubject }, JsonRequestBehavior.AllowGet);

        }
        public long GetLanguageId(string Culturename)
        {
            long languageid = 0;
            using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(SessionHelper.EnterPriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
            {
                languageid = Convert.ToInt64(context.ResourceLaguages.Where(u => u.Culture == Culturename).Select(p => p.ID).SingleOrDefault());
            }
            return languageid;
        }
        public long GetTemplateId(string EmailTemplateName)
        {
            long TemplateId = 0;
            using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(SessionHelper.EnterPriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
            {
                TemplateId = Convert.ToInt64(context.EmailTemplates.Where(u => u.TemplateName == EmailTemplateName).Select(p => p.ID).SingleOrDefault());
            }
            return TemplateId;
        }
        public JsonResult SaveEmailTemplate(string EmailTemplateName, string EmailText, string CurCulture, string EmailSubject)
        {
            string message = "";
            EmailTemplateDetailDTO objEmailTemplateDeailDTO = new EmailTemplateDetailDTO();
            EmailTemplateDAL objEmailTempateDetail = new EmailTemplateDAL(SessionHelper.EnterPriseDBName);

            objEmailTemplateDeailDTO.EmailTempateId = Convert.ToInt64(EmailTemplateName);
            objEmailTemplateDeailDTO.MailBodyText = HttpUtility.UrlDecode(EmailText);
            objEmailTemplateDeailDTO.ResourceLaguageId = GetLanguageId(CurCulture);
            objEmailTemplateDeailDTO.MailSubject = EmailSubject;
            objEmailTemplateDeailDTO.RoomId = SessionHelper.RoomID;
            objEmailTemplateDeailDTO.CompanyID = SessionHelper.CompanyID;
            objEmailTemplateDeailDTO.CreatedBy = SessionHelper.UserID;
            objEmailTemplateDeailDTO.LastUpdatedBy = SessionHelper.UserID;
            objEmailTemplateDeailDTO.Created = DateTimeUtility.DateTimeNow;
            objEmailTemplateDeailDTO.Updated = DateTimeUtility.DateTimeNow;

            if (objEmailTempateDetail.SaveEmailTemplate(objEmailTemplateDeailDTO))
                message = ResMessage.SaveMessage; //ResMessage.SaveMessage; //"Record Saved Sucessfully...";
            else
                message = ResMessage.SaveErrorMsg;
            return Json(new { Message = message }, JsonRequestBehavior.AllowGet);

            //if (EmailTemplateName.Contains("OrderApproval") && CurCulture == "en-US")
            //{
            //    using (FileStream fs = new FileStream(Server.MapPath("../") + "Content\\EmailTemplates\\" + EmailTemplateName + "-" + CurCulture + ".xslt", FileMode.Create))
            //    {
            //        using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
            //        {
            //            w.WriteLine(HttpUtility.UrlDecode(EmailText));
            //        }
            //    }
            //}
            //else
            //{
            //----------------------------------------------------
            ////using (FileStream fs = new FileStream(Server.MapPath("../") + "Content\\EmailTemplates\\" + EmailTemplateName + "-" + CurCulture + ".html", FileMode.Create))
            ////{
            ////    using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
            ////    {
            ////        w.WriteLine(HttpUtility.UrlDecode(EmailText));
            ////    }
            ////}
            //----------------------------------------------------
            //}

        }

        /// <summary>
        /// GetLanguage
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetLanguage()
        {
            //ResourceController resApiController = null;
            ResourceDAL resApiController = null;
            IEnumerable<ResourceLanguageDTO> resLangDTO = null;
            List<SelectListItem> lstItem = null;
            try
            {
                resApiController = new ResourceDAL(SessionHelper.EnterPriseDBName);
                resLangDTO = resApiController.GetCachedResourceLanguageData(SessionHelper.EnterPriceID);
                lstItem = new List<SelectListItem>();
                foreach (var item in resLangDTO)
                {
                    SelectListItem obj = new SelectListItem();
                    obj.Text = item.Language;
                    obj.Value = item.Culture;
                    lstItem.Add(obj);
                }
                return lstItem;
            }
            catch (Exception ex)
            {
                return lstItem;
                throw ex;
            }
            finally
            {
                resApiController = null;
                resLangDTO = null;
                lstItem = null;

            }

        }

        #endregion

        #region "CostUOM Master"
        //   [AuthorizeHelper]
        public ActionResult CostUOMList()
        {
            return View();
        }

        // [AuthorizeHelper]
        public PartialViewResult _CreateCostUOM()
        {
            return PartialView();
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>      
        public ActionResult CostUOMCreate(bool isforbom)
        {
            CostUOMMasterDTO objDTO = new CostUOMMasterDTO();
            objDTO.ID = 0;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.CreatedByName = SessionHelper.UserName;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.RoomName = SessionHelper.RoomName;
            objDTO.GUID = Guid.NewGuid();
            objDTO.isForBOM = isforbom;
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("CostUOMMaster");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }

            return PartialView("_CostUOMCreate", objDTO);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult CostUOMSave(CostUOMMasterDTO objDTO)
        {
            string message = "";
            string status = "";
            //CategoryController obj = new CategoryController();
            CostUOMMasterDAL obj = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            objDTO.Room = SessionHelper.RoomID;
            objDTO.RoomName = SessionHelper.RoomName;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;
            if (string.IsNullOrEmpty(objDTO.CostUOM))
            {
                message = string.Format(ResMessage.Required, ResCostUOMMaster.CostUOM);// "CostUOM is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(objDTO.CostUOMValue.ToString()))
            {
                message = string.Format(ResMessage.Required, ResCostUOMMaster.CostUOMValue);// "CategoryColor name is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            if (objDTO.CostUOMValue.GetValueOrDefault(0) <= 0)
            {
                message = ResCostUOMMaster.MsgCostUOMMinimumValue;
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.Room = SessionHelper.RoomID;

            Int64 _NewIDForPopUp = 0;
            if (objDTO.ID == 0)
            {
                objDTO.CreatedBy = SessionHelper.UserID;
                long RoomId = SessionHelper.RoomID;
                if (objDTO.isForBOM)
                {
                    RoomId = 0;
                }
                //string strOK = objCDAL.DuplicateCheck(objDTO.CostUOM.Trim(), "add", objDTO.ID, "CostUOMMaster", "CostUOM", RoomId, SessionHelper.CompanyID);
                string strOK = objCDAL.DuplicateCheck(objDTO.CostUOM.Trim(), "add", objDTO.ID, "CostUOMMaster", "CostUOM", RoomId, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResCostUOMMaster.CostUOM, objDTO.CostUOM);  // "Category \"" + objDTO.Category + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    //string strOK1 = string.Empty;
                    //if (objDTO.isForBOM == false)
                    //{
                    //    strOK1 = objCDAL.DuplicateCheck(objDTO.CostUOMValue.ToString(), "add", objDTO.ID, "CostUOMMaster", "CostUOMValue", SessionHelper.RoomID, SessionHelper.CompanyID);
                    //}
                    //if (strOK1 == "duplicate")
                    //{
                    //    message = string.Format(ResMessage.DuplicateMessage, ResCostUOMMaster.CostUOMValue, objDTO.CostUOMValue);  // "Category \"" + objDTO.Category + "\" already exist! Try with Another!";
                    //    status = "duplicate";
                    //}
                    //else
                    //{
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.CostUOM = objDTO.CostUOM.Trim().ToUpper();
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    //if (objDTO.isForBOM == null)
                    //{
                    //    objDTO.isForBOM = false;
                    //}
                    long ReturnVal = obj.Insert(objDTO);
                    if (ReturnVal > 0)
                    {
                        message = ResMessage.SaveMessage; //ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                        _NewIDForPopUp = ReturnVal;
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); //string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                    //}
                }
            }
            else
            {
                objDTO.LastUpdatedBy = SessionHelper.UserID;

                long RoomId = SessionHelper.RoomID;
                if (objDTO.isForBOM)
                {
                    objDTO.Room = 0;
                    RoomId = 0;
                }
                string strUOMMessage = string.Empty;
                if (objDTO.isForBOM == false)
                {
                    CostUOMMasterDTO ExistCUM = obj.GetCostUOMByID(objDTO.ID);
                    if (ExistCUM != null && ExistCUM.CostUOMValue.GetValueOrDefault(0) != objDTO.CostUOMValue.GetValueOrDefault(0))
                    {
                        Int64 Count = 0;
                        Count = obj.CheckCostUOMwithOrder(objDTO.ID, RoomId, SessionHelper.CompanyID);
                        if (Count > 0)
                        {
                            strUOMMessage = string.Format(ResCostUOMMaster.CostUOMValueExistInOrder);
                        }
                    }
                }

                string strOK = objCDAL.DuplicateCheck(objDTO.CostUOM, "edit", objDTO.ID, "CostUOMMaster", "CostUOM", RoomId, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResCostUOMMaster.CostUOM, objDTO.CostUOM);  //"Category \"" + objDTO.Category + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    //string strOK1 = objCDAL.DuplicateCheck(objDTO.CostUOMValue.ToString(), "edit", objDTO.ID, "CostUOMMaster", "CostUOMValue", SessionHelper.RoomID, SessionHelper.CompanyID);
                    //if (strOK1 == "duplicate")
                    //{
                    //    message = string.Format(ResMessage.DuplicateMessage, ResCostUOMMaster.CostUOMValue, objDTO.CostUOMValue);  // "Category \"" + objDTO.Category + "\" already exist! Try with Another!";
                    //    status = "duplicate";
                    //}
                    //else
                    //{

                    if (strUOMMessage == string.Empty)
                    {
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.EditedFrom = "Web";
                        //if (objDTO.isForBOM == null)
                        //{
                        //    objDTO.isForBOM = false;
                        //}
                        bool ReturnVal = obj.Edit(objDTO);
                        if (ReturnVal)
                        {
                            message = ResMessage.SaveMessage; //ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                            status = "ok";
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                            status = "fail";
                        }
                    }
                    else
                    {
                        message = strUOMMessage;
                        status = "UOMExist";
                    }
                    // }
                }
            }

            return Json(new { Message = message, Status = status, NewIDForPopUp = _NewIDForPopUp }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>      
        public ActionResult CostUOMEdit(Int64 ID)
        {
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            bool isForBom = bool.Parse(Request["isForBom"].ToString());

            if (IsDeleted || IsArchived)
            {
                ViewBag.ViewOnly = true;
            }

            long RoomId = SessionHelper.RoomID;

            CostUOMMaster objCostUOMMaster = new CostUOMMaster();
            if (ID > 0)
            {
                using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(SessionHelper.EnterPriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
                {
                    objCostUOMMaster = context.CostUOMMasters.Where(x => x.ID == ID).FirstOrDefault();
                    if (objCostUOMMaster != null && objCostUOMMaster.IsForBOM)
                    {
                        RoomId = 0;
                    }
                }
            }

            CostUOMMasterDAL obj = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName);
            CostUOMMasterDTO objDTO = obj.GetCostUOMByID(ID);
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("CostUOMMaster");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;
            objDTO.isForBOM = isForBom;
            return PartialView("_CostUOMCreate", objDTO);
        }

        #region Ajax Data Provider

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult CostUOMListAjax(JQueryDataTableParamModel param)
        {
            CostUOMMasterDAL obj = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName);
            bool IsForBom = false;
            bool.TryParse(Convert.ToString(Request["IsForBom"]), out IsForBom);
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
                    sortColumnName = "ID";

                if (sortDirection == "asc")
                    sortColumnName = sortColumnName + " asc";
                else
                    sortColumnName = sortColumnName + " desc";
            }
            else
                sortColumnName = "ID desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetPagedCostUOMRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, SessionHelper.RoomDateFormat, IsForBom, CurrentTimeZone);

            if (DataFromDB != null)
            {
                DataFromDB.ToList().ForEach(t =>
                {
                    t.CreatedDate = FnCommon.ConvertDateByTimeZone(t.Created, true);// CommonUtility.ConvertDateByTimeZone(t.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                    t.UpdatedDate = FnCommon.ConvertDateByTimeZone(t.Updated, true); //CommonUtility.ConvertDateByTimeZone(t.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                });
            }

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);


        }

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public JsonResult DeleteCostUOMRecords(string ids)
        {
            try
            {
                string response = string.Empty;
                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                response = objCommonDAL.DeleteModulewise(ids, ImportMastersDTO.TableName.CostUOMMaster.ToString(), false, SessionHelper.UserID, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);
                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetCostUOM(int maxRows, string name_startsWith)
        {
            CostUOMMasterDAL obj = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName);
            List<CostUOMMasterDTO> lstUnit = obj.GetCostUOMsByNameSearch((name_startsWith ?? string.Empty).ToLower().Trim(), SessionHelper.RoomID, SessionHelper.CompanyID);

            if (lstUnit.Count == 0)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            return Json(lstUnit, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #endregion

        #region "OrderUOM Master"
        public ActionResult OrderUOMList()
        {
            return View();
        }

        public PartialViewResult _CreateOrderUOM()
        {
            return PartialView();
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>      
        public ActionResult OrderUOMCreate(bool isforbom)
        {
            OrderUOMMasterDTO objDTO = new OrderUOMMasterDTO();
            objDTO.ID = 0;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.CreatedByName = SessionHelper.UserName;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.RoomName = SessionHelper.RoomName;
            objDTO.GUID = Guid.NewGuid();
            objDTO.isForBOM = isforbom;
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("OrderUOMMaster");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }

            return PartialView("_OrderUOMCreate", objDTO);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult OrderUOMSave(OrderUOMMasterDTO objDTO)
        {
            string message = "";
            string status = "";
            OrderUOMMasterDAL obj = new OrderUOMMasterDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            objDTO.Room = SessionHelper.RoomID;
            objDTO.RoomName = SessionHelper.RoomName;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;
            if (string.IsNullOrEmpty(objDTO.OrderUOM))
            {
                message = string.Format(ResMessage.Required, ResOrderUOMMaster.OrderUOM);
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(objDTO.OrderUOMValue.ToString()))
            {
                message = string.Format(ResMessage.Required, ResOrderUOMMaster.OrderUOMValue);
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            if (objDTO.OrderUOMValue.GetValueOrDefault(0) <= 0)
            {
                message = string.Format(ResCommon.MsgGreaterThanZero, ResOrderUOMMaster.OrderUOMValue);
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.Room = SessionHelper.RoomID;

            Int64 _NewIDForPopUp = 0;
            if (objDTO.ID == 0)
            {
                objDTO.CreatedBy = SessionHelper.UserID;
                long RoomId = SessionHelper.RoomID;
                if (objDTO.isForBOM)
                {
                    RoomId = 0;
                }

                string strOK = objCDAL.DuplicateCheck(objDTO.OrderUOM.Trim(), "add", objDTO.ID, "OrderUOMMaster", "OrderUOM", RoomId, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResOrderUOMMaster.OrderUOM, objDTO.OrderUOM);
                    status = "duplicate";
                }
                else
                {
                    //string strOK1 = string.Empty;
                    //if (objDTO.isForBOM == false)
                    //{
                    //    strOK1 = objCDAL.DuplicateCheck(objDTO.CostUOMValue.ToString(), "add", objDTO.ID, "CostUOMMaster", "CostUOMValue", SessionHelper.RoomID, SessionHelper.CompanyID);
                    //}
                    //if (strOK1 == "duplicate")
                    //{
                    //    message = string.Format(ResMessage.DuplicateMessage, ResCostUOMMaster.CostUOMValue, objDTO.CostUOMValue);  // "Category \"" + objDTO.Category + "\" already exist! Try with Another!";
                    //    status = "duplicate";
                    //}
                    //else
                    //{
                    objDTO.GUID = Guid.NewGuid();
                    objDTO.OrderUOM = objDTO.OrderUOM.Trim().ToUpper();
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                    objDTO.AddedFrom = "Web";
                    objDTO.EditedFrom = "Web";
                    //if (objDTO.isForBOM == null)
                    //{
                    //    objDTO.isForBOM = false;
                    //}
                    long ReturnVal = obj.Insert(objDTO);
                    if (ReturnVal > 0)
                    {
                        message = ResMessage.SaveMessage;
                        status = "ok";
                        _NewIDForPopUp = ReturnVal;
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);
                        status = "fail";
                    }
                    //}
                }
            }
            else
            {
                objDTO.LastUpdatedBy = SessionHelper.UserID;

                long RoomId = SessionHelper.RoomID;
                if (objDTO.isForBOM)
                {
                    objDTO.Room = 0;
                    RoomId = 0;
                }

                string strOK = objCDAL.DuplicateCheck(objDTO.OrderUOM, "edit", objDTO.ID, "OrderUOMMaster", "OrderUOM", RoomId, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResOrderUOMMaster.OrderUOM, objDTO.OrderUOM);  //"Category \"" + objDTO.Category + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    //string strOK1 = objCDAL.DuplicateCheck(objDTO.CostUOMValue.ToString(), "edit", objDTO.ID, "CostUOMMaster", "CostUOMValue", SessionHelper.RoomID, SessionHelper.CompanyID);
                    //if (strOK1 == "duplicate")
                    //{
                    //    message = string.Format(ResMessage.DuplicateMessage, ResCostUOMMaster.CostUOMValue, objDTO.CostUOMValue);  // "Category \"" + objDTO.Category + "\" already exist! Try with Another!";
                    //    status = "duplicate";
                    //}
                    //else
                    //{
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.EditedFrom = "Web";
                    //if (objDTO.isForBOM == null)
                    //{
                    //    objDTO.isForBOM = false;
                    //}
                    bool ReturnVal = obj.Edit(objDTO);
                    if (ReturnVal)
                    {
                        message = ResMessage.SaveMessage; //ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                        status = "fail";
                    }
                    // }
                }
            }

            return Json(new { Message = message, Status = status, NewIDForPopUp = _NewIDForPopUp }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>      
        public ActionResult OrderUOMEdit(Int64 ID)
        {
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());

            if (IsDeleted || IsArchived)
            {
                ViewBag.ViewOnly = true;
            }

            long RoomId = SessionHelper.RoomID;

            OrderUOMMaster objOrderUOMMaster = new OrderUOMMaster();
            if (ID > 0)
            {
                using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(SessionHelper.EnterPriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
                {
                    objOrderUOMMaster = context.OrderUOMMasters.Where(x => x.ID == ID).FirstOrDefault();
                    if (objOrderUOMMaster != null && objOrderUOMMaster.IsForBOM)
                    {
                        RoomId = 0;
                    }
                }
            }

            OrderUOMMasterDAL obj = new OrderUOMMasterDAL(SessionHelper.EnterPriseDBName);
            OrderUOMMasterDTO objDTO = obj.GetRecord(ID, RoomId, SessionHelper.CompanyID, IsArchived, IsDeleted, objOrderUOMMaster.IsForBOM);
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("OrderUOMMaster");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;
            return PartialView("_OrderUOMCreate", objDTO);
        }

        #region Ajax Data Provider

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult OrderUOMListAjax(JQueryDataTableParamModel param)
        {
            OrderUOMMasterDAL obj = new OrderUOMMasterDAL(SessionHelper.EnterPriseDBName);
            bool IsForBom = false;
            bool.TryParse(Convert.ToString(Request["IsForBom"]), out IsForBom);
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
                    sortColumnName = "ID";

                if (sortDirection == "asc")
                    sortColumnName = sortColumnName + " asc";
                else
                    sortColumnName = sortColumnName + " desc";
            }
            else
                sortColumnName = "ID desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetPagedRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, Convert.ToString(SessionHelper.RoomDateFormat), IsForBom, CurrentTimeZone);

            if (DataFromDB != null)
            {
                DataFromDB.ToList().ForEach(t =>
                {
                    t.CreatedDate = FnCommon.ConvertDateByTimeZone(t.Created, true);
                    t.UpdatedDate = FnCommon.ConvertDateByTimeZone(t.Updated, true);
                });
            }

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);


        }

        public string UpdateOrderUOMData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            //CostUOMMasterDAL obj = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName);
            //obj.UpdateData(id, value, rowId, columnPosition, columnId, columnName);
            return value;
        }


        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public JsonResult DeleteOrderUOMRecords(string ids)
        {
            try
            {
                string response = string.Empty;
                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                response = objCommonDAL.DeleteModulewise(ids, ImportMastersDTO.TableName.OrderUOMMaster.ToString(), false, SessionHelper.UserID, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);
                eTurns.DAL.CacheHelper<IEnumerable<OrderUOMMasterDTO>>.InvalidateCache();
                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        //public JsonResult GetOrderUOM(int maxRows, string name_startsWith)
        //{
        //    CostUOMMasterDAL obj = new CostUOMMasterDAL(SessionHelper.EnterPriseDBName);
        //    List<CostUOMMasterDTO> lstUnit = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(t => t.CostUOM.ToLower().Contains(name_startsWith.ToLower().Trim())).Take(maxRows).ToList();
        //    if (lstUnit.Count == 0)
        //    {
        //        return Json("", JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(lstUnit, JsonRequestBehavior.AllowGet);
        //}

        #endregion


        #endregion

        public ActionResult WrittenOffCategoryList()
        {
            return View();
        }

        #region "InventoryClassification Master"

        // [AuthorizeHelper]
        public ActionResult InventoryClassificationList()
        {
            return View();
        }

        // [AuthorizeHelper]
        public PartialViewResult _CreateInventoryClassification()
        {
            return PartialView();
        }

        /// <summary>
        /// GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>      
        public ActionResult InventoryClassificationCreate(bool isforbom)
        {
            InventoryClassificationMasterDTO objDTO = new InventoryClassificationMasterDTO();

            objDTO.ID = 0;
            objDTO.Created = DateTimeUtility.DateTimeNow;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.CreatedByName = SessionHelper.UserName;
            objDTO.Updated = DateTimeUtility.DateTimeNow;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.RoomName = SessionHelper.RoomName;
            objDTO.GUID = Guid.NewGuid();
            objDTO.isForBOM = isforbom;
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("InventoryClassificationMaster");

            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            return PartialView("_InventoryClassificationCreate", objDTO);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult InventoryClassificationSave(InventoryClassificationMasterDTO objDTO)
        {
            string message = "";
            string status = "";
            //CategoryController obj = new CategoryController();
            InventoryClassificationMasterDAL obj = new InventoryClassificationMasterDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            objDTO.Room = SessionHelper.RoomID;
            objDTO.RoomName = SessionHelper.RoomName;
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;

            if (string.IsNullOrEmpty(objDTO.InventoryClassification))
            {
                message = string.Format(ResMessage.Required, ResInventoryClassificationMaster.InventoryClassification);// "InventoryClassification is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            //if (string.IsNullOrEmpty(objDTO.BaseOfInventory.ToString()))
            //{
            //    message = string.Format(ResMessage.Required, ResInventoryClassificationMaster.BaseOfInventory);// "BaseOfInventory name is required.";
            //    status = "fail";
            //    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            //}

            //if (string.IsNullOrEmpty(objDTO.RangeStart.ToString()))
            //{
            //    message = string.Format(ResMessage.Required, ResInventoryClassificationMaster.RangeStart);// "RangeStart name is required.";
            //    status = "fail";
            //    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            //}

            //if (string.IsNullOrEmpty(objDTO.RangeEnd.ToString()))
            //{
            //    message = string.Format(ResMessage.Required, ResInventoryClassificationMaster.RangeEnd);// "RangeEnd name is required.";
            //    status = "fail";
            //    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            //}

            //if (((double)objDTO.RangeEnd) <= ((double)objDTO.RangeStart))
            //{
            //    message = "Endrange value mast be grater then Startrange."; // "RangeEnd name is required.";
            //    status = "fail";
            //    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            //}
            List<InventoryClassificationMasterDTO> objDTOlst = new List<InventoryClassificationMasterDTO>();
            // Check Range Validation 
            //if (objDTO.ID > 0)
            //{
            //    objDTOlst = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false).Where(t => t.ID != objDTO.ID).ToList();
            //}
            //else
            //{
            //    objDTOlst = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false).ToList();
            //}

            // find fromrange is between range 
            // List<InventoryClassificationMasterDTO> fromlist = obj.GetRangeRecord((double)objDTO.RangeStart, SessionHelper.RoomID).Where(t => t.ID != objDTO.ID).ToList();
            //if (fromlist.Count > 0)
            //{
            //    message = "Start range is already defined."; // "RangeEnd name is required.";
            //    status = "fail";
            //    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            //}
            // find torange is between range 
            //List<InventoryClassificationMasterDTO> Tolist = obj.GetRangeRecord((double)objDTO.RangeEnd, SessionHelper.RoomID).Where(t => t.ID != objDTO.ID).ToList();  //objDTOlst.Where(a => a.RangeStart <= (double)objDTO.RangeEnd && a.RangeEnd >= (double)objDTO.RangeEnd).ToList();
            //if (Tolist.Count > 0)
            //{
            //    message = "End range is already defined."; // "RangeEnd name is required.";
            //    status = "fail";
            //    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            //}

            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.Room = SessionHelper.RoomID;
            Int64 _NewIDForPopUp = 0;
            if (objDTO.ID == 0)
            {
                objDTO.CreatedBy = SessionHelper.UserID;
                long RoomId = SessionHelper.RoomID;
                if (objDTO.isForBOM)
                {
                    RoomId = 0;
                }
                string strOK = objCDAL.DuplicateCheck(objDTO.InventoryClassification, "add", objDTO.ID, "InventoryClassificationMaster", "InventoryClassification", RoomId, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    InventoryClassificationMasterDTO objInventoryClassificationMasterDTO = new InventoryClassificationMasterDTO();
                    //objInventoryClassificationMasterDTO = obj.GetAllRecords(RoomId, SessionHelper.CompanyID, false, false, false).Where(t => t.InventoryClassification == objDTO.InventoryClassification && t.isForBOM == true).ToList().FirstOrDefault();
                    objInventoryClassificationMasterDTO = obj.GetInventoryClassificationByNamePlain(RoomId, SessionHelper.CompanyID, objDTO.isForBOM, objDTO.InventoryClassification);
                    if (objInventoryClassificationMasterDTO != null && objDTO.isForBOM == false)
                    {
                        objInventoryClassificationMasterDTO.isForBOM = false;
                        //  objInventoryClassificationMasterDTO.BaseOfInventory = objDTO.BaseOfInventory;
                        objInventoryClassificationMasterDTO.RangeStart = objDTO.RangeStart;
                        objInventoryClassificationMasterDTO.RangeEnd = objDTO.RangeEnd;
                        objInventoryClassificationMasterDTO.UDF1 = objDTO.UDF1;

                        objInventoryClassificationMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objInventoryClassificationMasterDTO.EditedFrom = "Web";

                        if (obj.Edit(objInventoryClassificationMasterDTO))
                        {
                            message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                            status = "ok";
                            _NewIDForPopUp = objInventoryClassificationMasterDTO.ID;
                            return Json(new { Message = message, Status = status, NewIDForPopUp = _NewIDForPopUp }, JsonRequestBehavior.AllowGet);
                        }
                    }

                }

                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResInventoryClassificationMaster.InventoryClassification, objDTO.InventoryClassification);  // "Category \"" + objDTO.Category + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    string strOK1 = objCDAL.DuplicateCheck(objDTO.InventoryClassification.ToString(), "add", objDTO.ID, "InventoryClassificationMaster", "InventoryClassification", RoomId, SessionHelper.CompanyID);
                    if (strOK1 == "duplicate")
                    {
                        message = string.Format(ResMessage.DuplicateMessage, ResInventoryClassificationMaster.InventoryClassification, objDTO.InventoryClassification);  // "Category \"" + objDTO.Category + "\" already exist! Try with Another!";
                        status = "duplicate";
                    }
                    else
                    {
                        objDTO.GUID = Guid.NewGuid();
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.ReceivedOnWeb = DateTimeUtility.DateTimeNow;
                        objDTO.AddedFrom = "Web";
                        objDTO.EditedFrom = "Web";
                        long ReturnVal = obj.Insert(objDTO);
                        if (ReturnVal > 0)
                        {
                            message = ResMessage.SaveMessage; //ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                            status = "ok";
                            _NewIDForPopUp = ReturnVal;
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); //string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                            status = "fail";
                        }
                    }
                }
            }
            else
            {
                objDTO.LastUpdatedBy = SessionHelper.UserID;
                long RoomId = SessionHelper.RoomID;
                if (objDTO.isForBOM)
                {
                    RoomId = 0;
                }
                string strOK = objCDAL.DuplicateCheck(objDTO.InventoryClassification, "edit", objDTO.ID, "InventoryClassificationMaster", "InventoryClassification", RoomId, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResInventoryClassificationMaster.InventoryClassification, objDTO.InventoryClassification);  //"Category \"" + objDTO.Category + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    string strOK1 = objCDAL.DuplicateCheck(objDTO.InventoryClassification.ToString(), "edit", objDTO.ID, "InventoryClassificationMaster", "InventoryClassification", RoomId, SessionHelper.CompanyID);
                    if (strOK1 == "duplicate")
                    {
                        message = string.Format(ResMessage.DuplicateMessage, ResInventoryClassificationMaster.InventoryClassification, objDTO.InventoryClassification);  // "Category \"" + objDTO.Category + "\" already exist! Try with Another!";
                        status = "duplicate";
                    }
                    else
                    {
                        objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objDTO.EditedFrom = "Web";
                        bool ReturnVal = obj.Edit(objDTO);
                        if (objDTO.isForBOM == true)
                        {
                            BOMItemMasterDAL objBOMItemMasterDAL = new BOMItemMasterDAL(SessionHelper.EnterPriseDBName);
                            objBOMItemMasterDAL.UpdateBOMMasterReference(objDTO.ID, "InventoryClassificationMaster", SessionHelper.UserID);
                        }
                        if (ReturnVal)
                        {
                            message = ResMessage.SaveMessage; //ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                            status = "ok";
                        }
                        else
                        {
                            message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                            status = "fail";
                        }
                    }
                }
            }
            return Json(new { Message = message, Status = status, NewIDForPopUp = _NewIDForPopUp }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Save Inventory Classification Range
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public JsonResult SaveInventoryClassificationRangeData(List<InventoryClassificationMasterDTO> lstDTO)
        {
            Dictionary<string, object> json = null;
            JsonResult objValidRusult = null;
            JavaScriptSerializer objJSSerial = null;
            InventoryClassificationMasterDAL obj = null;
            InventoryClassificationMasterDTO objInventoryClassificationMasterDTO = null;
            try
            {
                objValidRusult = ValidateInventoryClassificationRangeData(lstDTO);
                objJSSerial = new JavaScriptSerializer();
                json = objJSSerial.Deserialize<Dictionary<string, object>>(objJSSerial.Serialize(objValidRusult.Data));

                if (Convert.ToString(json["Message"]) != "ok")
                {
                    return objValidRusult;
                }

                obj = new InventoryClassificationMasterDAL(SessionHelper.EnterPriseDBName);
                foreach (var item in lstDTO)
                {
                    objInventoryClassificationMasterDTO = obj.GetInventoryClassificationByIDPlain(item.ID);
                    if (objInventoryClassificationMasterDTO != null)
                    {
                        objInventoryClassificationMasterDTO.isForBOM = item.isForBOM;
                        objInventoryClassificationMasterDTO.RangeStart = item.RangeStart;
                        objInventoryClassificationMasterDTO.RangeEnd = item.RangeEnd;
                        objInventoryClassificationMasterDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                        objInventoryClassificationMasterDTO.EditedFrom = "Web";
                        obj.Edit(objInventoryClassificationMasterDTO);
                    }
                }
                if (lstDTO != null && lstDTO[0].isForBOM == false)
                {
                    DashboardDAL objDashboardDAL = new DashboardDAL(SessionHelper.EnterPriseDBName);
                    objDashboardDAL.ReclassifyAllItems(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                }
                return Json(new { Success = true, Message = "ok" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                json = null;
                objValidRusult = null;
                objJSSerial = null;
            }
        }

        /// <summary>
        /// Validate Inventory Classification Range Data
        /// </summary>
        /// <param name="objDTO"></param>
        /// <returns></returns>
        public JsonResult ValidateInventoryClassificationRangeData(List<InventoryClassificationMasterDTO> listDTO)
        {
            bool status = true;
            //Dictionary<Int64, string> lstErrorMsg = null;
            List<string> lstErrorMsg = null;
            Dictionary<string, string> dictErrorMsg = null;
            IEnumerable<InventoryClassificationMasterDTO> lstInvClassCompareList = null;
            InventoryClassificationMasterDTO objCurrDTO = null;
            InventoryClassificationMasterDTO objNextDTO = null;
            string errorMsg = "ok";
            try
            {
                lstErrorMsg = new List<string>();
                dictErrorMsg = new Dictionary<string, string>();
                //MinusRange
                lstInvClassCompareList = listDTO.Where(x => x.RangeStart.GetValueOrDefault(0) < 0 || x.RangeEnd.GetValueOrDefault(0) < 0);

                if (lstInvClassCompareList != null && lstInvClassCompareList.Count() > 0)
                {
                    status = false;
                    foreach (var item in lstInvClassCompareList)
                    {
                        if (!dictErrorMsg.ContainsKey(item.InventoryClassification))
                        {
                            dictErrorMsg.Add(item.InventoryClassification, " " + ResInventoryClassificationMaster.NotProperRange);
                        }

                    }
                }

                //StartRange
                lstInvClassCompareList = null;
                lstInvClassCompareList = listDTO.Where(x => x.RangeStart.GetValueOrDefault(0) == 0 && x.RangeEnd.GetValueOrDefault(0) > 0);

                if (lstInvClassCompareList != null && lstInvClassCompareList.Count() > 1)
                {
                    status = false;
                    foreach (var item in lstInvClassCompareList)
                    {
                        if (!dictErrorMsg.ContainsKey(item.InventoryClassification))
                        {
                            dictErrorMsg.Add(item.InventoryClassification, " " + ResInventoryClassificationMaster.NotProperRange);
                        }
                    }
                }

                //EndRange
                lstInvClassCompareList = null;
                lstInvClassCompareList = listDTO.Where(x => x.RangeStart.GetValueOrDefault(0) > 0 && x.RangeEnd.GetValueOrDefault(0) == 0);

                if (lstInvClassCompareList != null && lstInvClassCompareList.Count() > 1)
                {
                    status = false;
                    foreach (var item in lstInvClassCompareList)
                    {
                        if (!dictErrorMsg.ContainsKey(item.InventoryClassification))
                        {
                            dictErrorMsg.Add(item.InventoryClassification, " " + ResInventoryClassificationMaster.NotProperRange);
                        }
                    }
                }

                //SameRange
                lstInvClassCompareList = null;
                lstInvClassCompareList = listDTO.Where(x => (x.RangeStart.GetValueOrDefault(0) > 0 && x.RangeEnd.GetValueOrDefault(0) > 0) && (x.RangeStart.GetValueOrDefault(0) == x.RangeEnd.GetValueOrDefault(0)));

                if (lstInvClassCompareList != null && lstInvClassCompareList.Count() > 0)
                {
                    status = false;
                    foreach (var item in lstInvClassCompareList)
                    {
                        if (!dictErrorMsg.ContainsKey(item.InventoryClassification))
                        {
                            dictErrorMsg.Add(item.InventoryClassification, " " + ResInventoryClassificationMaster.NotProperRange);
                        }
                    }
                }

                //InvalidRange
                lstInvClassCompareList = null;
                lstInvClassCompareList = listDTO.Where(x => (x.RangeStart.GetValueOrDefault(0) > 0 && x.RangeEnd.GetValueOrDefault(0) > 0) && (x.RangeStart.GetValueOrDefault(0) > x.RangeEnd.GetValueOrDefault(0)));
                if (lstInvClassCompareList != null && lstInvClassCompareList.Count() > 0)
                {
                    status = false;
                    foreach (var item in lstInvClassCompareList)
                    {
                        if (!dictErrorMsg.ContainsKey(item.InventoryClassification))
                        {
                            dictErrorMsg.Add(item.InventoryClassification, " " + ResInventoryClassificationMaster.NotProperRange);
                        }
                    }
                }

                //InvalidRange12
                lstInvClassCompareList = null;
                lstInvClassCompareList = listDTO.Where(x => (x.RangeStart.GetValueOrDefault(0) >= 0 && x.RangeEnd.GetValueOrDefault(0) > 0) || (x.RangeStart.GetValueOrDefault(0) > 0 && x.RangeEnd.GetValueOrDefault(0) >= 0)).OrderBy(x => x.RangeStart.GetValueOrDefault(0));
                if (lstInvClassCompareList != null && lstInvClassCompareList.Count() > 0)
                {
                    for (int i = 0; i < lstInvClassCompareList.Count() - 1; i++)
                    {
                        objCurrDTO = lstInvClassCompareList.ElementAt(i);
                        objNextDTO = lstInvClassCompareList.ElementAt(i + 1);

                        if (objNextDTO.RangeStart.GetValueOrDefault(0) <= objCurrDTO.RangeEnd.GetValueOrDefault(0) || objCurrDTO.RangeStart.GetValueOrDefault(0) >= objNextDTO.RangeStart.GetValueOrDefault(0))
                        {
                            status = false;
                            //lstErrorMsg.Add(objCurrDTO.ID, objCurrDTO.InventoryClassification + ": has Invalid Range");
                            //lstErrorMsg.Add(objNextDTO.InventoryClassification + ": has Invalid Range.");
                            if (!dictErrorMsg.ContainsKey(objNextDTO.InventoryClassification))
                            {
                                dictErrorMsg.Add(objNextDTO.InventoryClassification, " " + ResInventoryClassificationMaster.NotProperRange);
                            }
                        }
                    }
                }

                if (dictErrorMsg.Count > 0)
                {
                    foreach (var item in dictErrorMsg)
                    {
                        //string mergeValue = string.Empty;
                        //string[] values = item.Value.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        //for (int i = 0; i < values.Length; i++)
                        //{
                        //    if (i > 0 && i < values.Length - 1)
                        //        mergeValue += ",";
                        //    else if (i > 0 && i == values.Length - 1)
                        //        mergeValue += " and ";

                        //    mergeValue += values[i];
                        //}

                        lstErrorMsg.Add(item.Key + ": " + item.Value);
                    }
                    errorMsg = "fail";
                }

                return Json(new { Success = status, Message = errorMsg, ErrorList = lstErrorMsg }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                lstErrorMsg = null;
                lstInvClassCompareList = null;
                objCurrDTO = null;
                objNextDTO = null;
            }

        }

        /// <summary>
        /// InventoryClassificationRange
        /// </summary>
        //public class InventoryClassificationRange
        //{
        //    public Int64 ID { get; set; }
        //    public Nullable<System.Double> RangeStart { get; set; }
        //    public Nullable<System.Double> RangeEnd { get; set; }
        //}

        /// <summary>
        /// SaveInventoryClassificationRange
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ///[HttpPost]
        //public JsonResult SaveInventoryClassificationRange(string para = "")
        //{
        //    string message = "";
        //    string status = "";
        //    try
        //    {
        //        JavaScriptSerializer s = new JavaScriptSerializer();
        //        InventoryClassificationRange[] LstInventoryLocation = s.Deserialize<InventoryClassificationRange[]>(para);
        //        InventoryClassificationMasterDAL obj = new InventoryClassificationMasterDAL(SessionHelper.EnterPriseDBName);
        //        if (LstInventoryLocation != null && LstInventoryLocation.Length > 0)
        //        {
        //            InventoryClassificationMasterDTO objInventoryClassificationMasterDTO = new InventoryClassificationMasterDTO();
        //            foreach (InventoryClassificationRange item in LstInventoryLocation)
        //            {
        //                objInventoryClassificationMasterDTO = obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, false).Where(t => t.ID == item.ID).ToList().FirstOrDefault();
        //                if (objInventoryClassificationMasterDTO != null)
        //                {
        //                    objInventoryClassificationMasterDTO.isForBOM = false;
        //                    //  objInventoryClassificationMasterDTO.BaseOfInventory = objDTO.BaseOfInventory;
        //                    objInventoryClassificationMasterDTO.RangeStart = item.RangeStart;
        //                    objInventoryClassificationMasterDTO.RangeEnd = item.RangeEnd;
        //                    obj.Edit(objInventoryClassificationMasterDTO);
        //                }
        //            }
        //        }
        //        message = string.Format(ResMessage.SaveMessage, HttpStatusCode.OK);
        //        status = ResMessage.SaveMessage;

        //    }
        //    catch (Exception ex)
        //    {
        //        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed);// "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
        //        status = "fail";
        //    }
        //    finally
        //    {
        //        // resHelper = null;
        //    }
        //    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);


        //}

        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>      
        public ActionResult InventoryClassificationEdit(Int64 ID)
        {
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());

            if (IsDeleted || IsArchived)
            {
                ViewBag.ViewOnly = true;
            }

            //CategoryController obj = new CategoryController();
            InventoryClassificationMasterDAL obj = new InventoryClassificationMasterDAL(SessionHelper.EnterPriseDBName);
            InventoryClassificationMasterDTO objDTO = obj.GetInventoryClassificationByIDNormal(ID);
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("InventoryClassificationMaster");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;
            return PartialView("_InventoryClassificationCreate", objDTO);
        }

        #region Ajax Data Provider

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult InventoryClassificationListAjax(JQueryDataTableParamModel param)
        {
            //CategoryController obj = new CategoryController();
            InventoryClassificationMasterDAL obj = new InventoryClassificationMasterDAL(SessionHelper.EnterPriseDBName);
            bool IsForBom = false;
            bool.TryParse(Convert.ToString(Request["IsForBom"]), out IsForBom);
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
            sortColumnName = Convert.ToString(Request["SortingField"]);

            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());

            //if (string.IsNullOrEmpty(sortColumnName) || sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID Desc";
            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined"))
                    sortColumnName = "ID desc";
            }
            else
                sortColumnName = "ID desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;
            IEnumerable<InventoryClassificationMasterDTO> DataFromDB;
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            DataFromDB = obj.GetPagedInventoryClassificationMaster(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, IsForBom, RoomDateFormat, CurrentTimeZone);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);


        }

        /// <summary>
        /// UpdateInventoryClassificationData
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <param name="rowId"></param>
        /// <param name="columnPosition"></param>
        /// <param name="columnId"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        //public string UpdateInventoryClassificationData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        //{
        //    //CategoryController obj = new CategoryController();
        //    InventoryClassificationMasterDAL obj = new InventoryClassificationMasterDAL(SessionHelper.EnterPriseDBName);
        //    obj.UpdateData(id, value, rowId, columnPosition, columnId, columnName);
        //    return value;
        //}

        /// <summary>
        /// DuplicateCategoryCheck
        /// </summary>
        /// <param name="CategoryName"></param>
        /// <param name="ActionMode"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        //public string DuplicateCategoryCheck(string CategoryName, string ActionMode, int ID)
        //{
        //    CategoryController obj = new CategoryController();
        //    return obj.DuplicateCheck(CategoryName, ActionMode, ID);
        //}

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public JsonResult DeleteInventoryClassificationRecords(string ids)
        {
            try
            {
                //eTurns.DAL.CommonDAL _repository = new eTurns.DAL.CommonDAL(SessionHelper.EnterPriseDBName);
                //return _repository.DeleteRecords(ImportMastersDTO.TableName.InventoryClassificationMaster.ToString(), ids, SessionHelper.RoomID, SessionHelper.CompanyID);
                string response = string.Empty;
                CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
                response = objCommonDAL.DeleteModulewise(ids, ImportMastersDTO.TableName.InventoryClassificationMaster.ToString(), false, SessionHelper.UserID, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);
                eTurns.DAL.CacheHelper<IEnumerable<InventoryClassificationMasterDTO>>.InvalidateCache();
                return Json(new { Message = response, Status = "ok" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                return Json(new { Message = "", Status = "Fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetInventoryClassification(int maxRows, string name_startsWith)
        {
            InventoryClassificationMasterDAL obj = new InventoryClassificationMasterDAL(SessionHelper.EnterPriseDBName);
            List<InventoryClassificationMasterDTO> lstUnit = obj.GetInventoryClassificationByNameSearch(name_startsWith, SessionHelper.RoomID, SessionHelper.CompanyID, false).OrderBy(x => x.InventoryClassification).Take(maxRows).ToList();
            if (lstUnit.Count == 0)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            return Json(lstUnit, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion

        public ActionResult UnderConstruction()
        {
            return View();
        }

        #region "Vender Master"

        /// <summary>
        /// JSON Record Save - Enter key Save/Update
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="ShipVia"></param>
        /// <returns></returns>
        /// 

        [ValidateAntiForgeryToken]
        public JsonResult VenderSave(VenderMasterDTO objDTO)
        {
            string message = "";

            VenderMasterDAL obj = new VenderMasterDAL(SessionHelper.EnterPriseDBName);

            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.Room = SessionHelper.RoomID;
            if (objDTO.ID == 0)
            {
                objDTO.CreatedBy = SessionHelper.UserID;
                string strOK = new CommonDAL(SessionHelper.EnterPriseDBName).DuplicateCheck(objDTO.Vender, "add", objDTO.ID, "VenderMaster", "Vender", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK != "ok")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResShipVia.ShipVia, objDTO.Vender);  //"ShipVia \"" + objDTO.ShipVia + "\" already exist! Try with Another!";
                }
                else
                {
                    objDTO.GUID = Guid.NewGuid();
                    Int64 ID = obj.Insert(objDTO);
                    if (ID > 0)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                    }
                }
            }
            else
            {
                objDTO.LastUpdatedBy = SessionHelper.UserID;
                string strOK = new CommonDAL(SessionHelper.EnterPriseDBName).DuplicateCheck(objDTO.Vender, "edit", objDTO.ID, "VenderMaster", "Vender", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK != "ok")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResShipVia.ShipVia, objDTO.Vender);  //"ShipVia \"" + objDTO.ShipVia + "\" already exist! Try with Another!";
                }
                else
                {
                    bool isSave = obj.Edit(objDTO);

                    if (isSave)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                    }
                }
            }


            return Json(new { Message = message }, JsonRequestBehavior.AllowGet);
        }

        ///// <summary>
        ///// Check Duplicate ShipVia
        ///// </summary>
        ///// <param name="ShipViaName"></param>
        ///// <param name="Action"></param>
        ///// <param name="ID"></param>
        ///// <returns></returns>
        //public string DuplicateShipViaCheck(string ShipViaName, string Action, Int64 ID)
        //{
        //    ShipViaController obj = new ShipViaController();
        //    return obj.DuplicateCheck(ShipViaName, Action, ID);
        //}

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public string DeleteVenderRecords(string ids)
        {
            try
            {

                VenderMasterDAL obj = new VenderMasterDAL(SessionHelper.EnterPriseDBName);
                obj.DeleteVenderMasterRecords(ids, SessionHelper.UserID);
                return "ok";

                //return "not ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult VenderMasterListAjax(JQueryDataTableParamModel param)
        {


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

            // set the default column sorting here, if first time then required to set 
            //if (sortColumnName == "0" || sortColumnName == "undefined")
            //    sortColumnName = "ID";
            if (!string.IsNullOrEmpty(sortColumnName))
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
            if (param.sSearch != null && param.sSearch != "")
            {
                searchQuery = "WHERE ShipVia like '%" + param.sSearch + "%'" + @"
                            OR RoomName like '%" + param.sSearch + "%'" + @" 
                            OR CreatedBy like '%" + param.sSearch + "%'";
            }
            VenderMasterDAL obj = new VenderMasterDAL(SessionHelper.EnterPriseDBName);
            int TotalRecordCount = 0;
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetPagedVenderMaster(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, RoomDateFormat, CurrentTimeZone);

            //var result = from c in DataFromDB
            //             select new VenderMasterDTO
            //             {
            //                 ID = c.ID,
            //                 Vender = c.Vender,
            //                 RoomName = c.RoomName,
            //                 Created = c.Created,
            //                 CreatedBy = c.CreatedBy,
            //                 Updated = c.Updated,
            //                 LastUpdatedBy = c.LastUpdatedBy,
            //                 UpdatedByName = c.UpdatedByName,
            //                 Room = c.Room,
            //                 CreatedByName = c.CreatedByName,
            //                 IsDeleted = c.IsDeleted,
            //                 IsArchived = c.IsArchived,
            //                 UDF1 = c.UDF1,
            //                 UDF2 = c.UDF2,
            //                 UDF3 = c.UDF3,
            //                 UDF4 = c.UDF4,
            //                 UDF5 = c.UDF5
            //             };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);


        }

        /// <summary>
        ///GET ALL: /Master/ 
        /// </summary>
        /// <returns></returns>
        public ActionResult VenderMasterList()
        {
            return View();
        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult VenderMasterCreate()
        {
            VenderMasterDTO objDTO = new VenderMasterDTO()
            {
                Created = DateTimeUtility.DateTimeNow,
                Updated = DateTimeUtility.DateTimeNow,
                CreatedBy = SessionHelper.UserID,
                CreatedByName = SessionHelper.UserName,
                LastUpdatedBy = SessionHelper.UserID,
                Room = SessionHelper.RoomID,
                CompanyID = SessionHelper.CompanyID,
                RoomName = SessionHelper.RoomName,
                UpdatedByName = SessionHelper.UserName,
                GUID = Guid.NewGuid()
            };
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("VenderMaster");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            return PartialView("_CreateVenderMaster", objDTO);
        }

        public PartialViewResult _CreateVenderMaster()
        {
            return PartialView();
        }

        public string UpdateVenderMasterData(int id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            //VenderMasterController obj = new VenderMasterController();
            //obj.PutUpdateData(id, value, rowId, columnPosition, columnId, columnName);
            return value;
        }

        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>
        public ActionResult VenderMasterEdit(Int64 ID)
        {
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            if (IsDeleted || IsArchived)
            {
                ViewBag.ViewOnly = true;
            }

            VenderMasterDAL cntrl = new VenderMasterDAL(SessionHelper.EnterPriseDBName);
            VenderMasterDTO objDTO = cntrl.GetVenderByIDNormal(ID, SessionHelper.RoomID, SessionHelper.CompanyID);
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("VenderMaster");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;
            return PartialView("_CreateVenderMaster", objDTO);
        }


        #endregion

        /// <summary>
        /// Method to get status count
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="StatusFieldName"></param>
        /// <returns></returns>
        public ActionResult GetTabStatusWithCount(string TableName, string StatusFieldName)
        {
            ItemManufacturerDetailsDAL objData = new ItemManufacturerDetailsDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCommon = new CommonDAL(SessionHelper.EnterPriseDBName);
            var result = objCommon.GetTabStatusCount(TableName, StatusFieldName, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserSupplierIds, "false");
            return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTabStatusModuleWiseWithCount()
        {
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            List<RedCountDTO> lstRedCount = objCommonDAL.GetRedCountByModuleType("Consume", SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserSupplierIds, SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowOrderToConsignedItem));
            Int64 RequisitionMenuLinkCount = 0;
            List<RedCountDTO> lstReqRedCount = lstRedCount.Where(x => x.ModuleName == "Requisition").ToList();
            RequisitionMenuLinkCount = lstReqRedCount.Where(x => x.Status != "Closed").Sum(x => x.RecCircleCount);

            bool isRequisitions = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.Requisitions, eTurnsWeb.Helper.SessionHelper.PermissionType.View);

            if (!isRequisitions)
            {
                RequisitionMenuLinkCount = 0;
            }

            return Json(new { Result = RequisitionMenuLinkCount.ToString() }, JsonRequestBehavior.AllowGet);
        }

        #region "Auto WorkOrder Requisition"
        public JsonResult CreateReqWOFrmMaintainance(string MaintenanceGUID, double? OdometerEntry)
        {
            Guid RequisitionGUID = Guid.Empty;
            Guid WorkOrderGUID = Guid.Empty;
            ToolsMaintenanceDAL objToolMaiDAL = new ToolsMaintenanceDAL(SessionHelper.EnterPriseDBName);
            ToolsMaintenanceDTO objToolMaiDTO = objToolMaiDAL.GetToolsMaintenanceByGuidPlain(Guid.Parse(MaintenanceGUID));
            long SessionUserId = SessionHelper.UserID;
            if (objToolMaiDTO != null)
            {

                //first update the actual odometer entry and date in the maintenance table.
                objToolMaiDTO.LastMaintenanceDate = DateTimeUtility.DateTimeNow;
                objToolMaiDTO.LastMeasurementValue = OdometerEntry.ToString();

                objToolMaiDAL.Edit(objToolMaiDTO);
                //end-edit maintenance


                ToolMaintenanceDetailsDAL objToolMaiDtlDAL = new ToolMaintenanceDetailsDAL(SessionHelper.EnterPriseDBName);
                //List<ToolMaintenanceDetailsDTO> objToolMaiDtlDTO = objToolMaiDtlDAL.GetCachedData(SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.MaintenanceGUID == objToolMaiDTO.GUID).ToList();
                List<ToolMaintenanceDetailsDTO> objToolMaiDtlDTO = objToolMaiDtlDAL.GetMaintenanceLineItemsBymntsGUID(objToolMaiDTO.GUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                if (objToolMaiDtlDTO != null && objToolMaiDtlDTO.Count > 0)
                {
                    #region "Add WorkOrder"
                    WorkOrderDAL objWODAL = new WorkOrderDAL(SessionHelper.EnterPriseDBName);
                    AutoOrderNumberGenerate objAutoNumber = null;
                    //string nextWONo = new AutoSequenceDAL(SessionHelper.EnterPriseDBName).GetLastGeneratedROOMID("NextWorkOrderNo", SessionHelper.RoomID, SessionHelper.CompanyID).ToString();
                    //string nextWONo = new AutoSequenceDAL(SessionHelper.EnterPriseDBName).GetNextAutoNumberByModule("NextWorkOrderNo", SessionHelper.RoomID, SessionHelper.CompanyID);
                    objAutoNumber = new AutoSequenceDAL(SessionHelper.EnterPriseDBName).GetNextWorkOrderNumber(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.EnterPriceID);

                    string nextWONo = objAutoNumber.OrderNumber;
                    if (nextWONo != null && (!string.IsNullOrEmpty(nextWONo)))
                    {
                        nextWONo = nextWONo.Length > 22 ? nextWONo.Substring(0, 22) : nextWONo;
                    }

                    string _ReleaseNumber = objWODAL.GenerateAndGetReleaseNumber(nextWONo, 0, SessionHelper.RoomID, SessionHelper.CompanyID);
                    WorkOrderDTO objWODTO = new WorkOrderDTO()
                    {
                        Created = DateTimeUtility.DateTimeNow,
                        Updated = DateTimeUtility.DateTimeNow,
                        CreatedBy = SessionHelper.UserID,
                        CreatedByName = SessionHelper.UserName,
                        LastUpdatedBy = SessionHelper.UserID,
                        Room = SessionHelper.RoomID,
                        CompanyID = SessionHelper.CompanyID,
                        RoomName = SessionHelper.RoomName,
                        UpdatedByName = SessionHelper.UserName,
                        IsArchived = false,
                        IsDeleted = false,
                        //WOName = "#W" + nextWONo,
                        WOName = nextWONo,
                        WOStatus = "Open",
                        WOType = "Maint",
                        AssetGUID = objToolMaiDTO.AssetGUID,
                        ToolGUID = objToolMaiDTO.ToolGUID,
                        Odometer_OperationHours = OdometerEntry,
                        WhatWhereAction = "Work Order Maintainance",
                        ReleaseNumber = _ReleaseNumber
                    };
                    WorkOrderGUID = objWODAL.Insert(objWODTO);
                    #endregion
                    #region "Add Requisition"
                    RequisitionMasterDAL objReqDAL = new RequisitionMasterDAL(SessionHelper.EnterPriseDBName);
                    //string nextReqNo = new AutoSequenceDAL(SessionHelper.EnterPriseDBName).GetLastGeneratedROOMID("NextRequisitionNo", SessionHelper.RoomID, SessionHelper.CompanyID).ToString();
                    string nextReqNo = new AutoSequenceDAL(SessionHelper.EnterPriseDBName).GetNextAutoNumberByModule("NextRequisitionNo", SessionHelper.RoomID, SessionHelper.CompanyID);
                    RequisitionMasterDTO objReqDTO = new RequisitionMasterDTO()
                    {
                        Created = DateTimeUtility.DateTimeNow,
                        Updated = DateTimeUtility.DateTimeNow,
                        CreatedBy = SessionHelper.UserID,
                        CreatedByName = SessionHelper.UserName,
                        LastUpdatedBy = SessionHelper.UserID,
                        UpdatedByName = SessionHelper.UserName,
                        Room = SessionHelper.RoomID,
                        RoomName = SessionHelper.RoomName,
                        CompanyID = SessionHelper.CompanyID,
                        IsArchived = false,
                        IsDeleted = false,
                        RequisitionStatus = "Unsubmitted",
                        //RequisitionNumber = "#R" + nextReqNo,
                        RequisitionNumber = nextReqNo,
                        RequisitionType = objToolMaiDTO.AssetGUID != null ? "Asset Service" : "Tool Service",
                        WorkorderGUID = WorkOrderGUID,
                        WorkorderName = nextWONo,
                        WhatWhereAction = "Requisition Maintainance",
                    };
                    RequisitionGUID = objReqDAL.Insert(objReqDTO).GUID;
                    #endregion
                    #region "Add LineItems to Req And WO"
                    RequisitionDetailsDAL objReqDtlDAL = new RequisitionDetailsDAL(SessionHelper.EnterPriseDBName);
                    WorkOrderLineItemsDAL objWODtlDAL = new WorkOrderLineItemsDAL(SessionHelper.EnterPriseDBName);
                    for (int i = 0; i <= objToolMaiDtlDTO.Count - 1; i++)
                    {
                        ItemMasterDTO ObjItemDTO = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, objToolMaiDtlDTO[i].ItemGUID);
                        #region "Req Items"
                        //if (ObjItemDTO.ItemType != 4)
                        //{
                        RequisitionDetailsDTO itemReq = new RequisitionDetailsDTO();
                        itemReq.Room = SessionHelper.RoomID;
                        itemReq.RoomName = SessionHelper.RoomName;
                        itemReq.CreatedBy = SessionHelper.UserID;
                        itemReq.CreatedByName = SessionHelper.UserName;
                        itemReq.UpdatedByName = SessionHelper.UserName;
                        itemReq.LastUpdatedBy = SessionHelper.UserID;
                        itemReq.CompanyID = SessionHelper.CompanyID;
                        itemReq.ItemNumber = ObjItemDTO.ItemNumber;
                        itemReq.ItemGUID = objToolMaiDtlDTO[i].ItemGUID;
                        itemReq.ItemCost = objToolMaiDtlDTO[i].ItemCost;
                        itemReq.ItemSellPrice = ObjItemDTO.SellPrice ?? 0;
                        itemReq.QuantityRequisitioned = objToolMaiDtlDTO[i].Quantity;
                        itemReq.RequisitionGUID = RequisitionGUID;
                        objReqDtlDAL.Insert(itemReq, SessionUserId);
                        //}
                        #endregion
                        //#region "Wo Items"
                        //WorkOrderLineItemsDTO itemWO = new WorkOrderLineItemsDTO();
                        //itemWO.Room = SessionHelper.RoomID;
                        //itemWO.RoomName = SessionHelper.RoomName;
                        //itemWO.CreatedBy = SessionHelper.UserID;
                        //itemWO.CreatedByName = SessionHelper.UserName;
                        //itemWO.UpdatedByName = SessionHelper.UserName;
                        //itemWO.LastUpdatedBy = SessionHelper.UserID;
                        //itemWO.CompanyID = SessionHelper.CompanyID;
                        //itemWO.ItemNumber = ObjItemDTO.ItemNumber;
                        //itemWO.ItemGUID = objToolMaiDtlDTO[i].ItemGUID;
                        //itemWO.ItemCost = objToolMaiDtlDTO[i].ItemCost;
                        //itemWO.Quantity = objToolMaiDtlDTO[i].Quantity;
                        //itemWO.WorkOrderGUID = WorkOrderGUID;
                        //objWODtlDAL.Insert(itemWO);
                        //#endregion
                    }
                    if (RequisitionGUID != Guid.Empty)
                    {
                        objReqDtlDAL.UpdateRequisitionTotalCost(RequisitionGUID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    }

                    #endregion
                    #region "Update Maintenance"
                    objToolMaiDTO.WorkorderGUID = WorkOrderGUID;
                    objToolMaiDTO.RequisitionGUID = RequisitionGUID;
                    objToolMaiDTO.Status = "started";
                    objToolMaiDTO.MaintenanceDate = DateTimeUtility.DateTimeNow;
                    objToolMaiDAL.Edit(objToolMaiDTO);
                    #endregion
                }
            }
            return Json(new { requisitionID = RequisitionGUID, workOrderID = WorkOrderGUID }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        //public string GetNextIncrementedNumber(string ModuleName, string AppendedChar)
        //{
        //    string NewNumber = string.Empty;
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(AppendedChar) && !string.IsNullOrEmpty(ModuleName))
        //            NewNumber = AppendedChar + new AutoSequenceDAL(SessionHelper.EnterPriseDBName).GetLastGeneratedROOMID(ModuleName, SessionHelper.RoomID, SessionHelper.CompanyID).ToString();

        //        return NewNumber;
        //    }
        //    catch (Exception)
        //    {
        //        return NewNumber;
        //    }
        //}

        #region "Adding QuickList to OtherModules"


        public JsonResult AddQuickListToDetailTableWithoutCount(string para, string ModuleName, string QuickListGUID, string QLQTY)
        {
            bool AllowConsignedItemToOrder = SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowOrderToConsignedItem);

            string message = "";
            string status = "";
            bool IsLaborAllowed = false;

            if (ModuleName.Contains("AS") || ModuleName.Contains("WO") || ModuleName.Contains("KIT") || ModuleName.Contains("QL"))
                IsLaborAllowed = true;

            List<RequisitionDetailsDTO> lstReqItemDetails = null;

            if (ModuleName.ToLower() == "rq")
            {
                JavaScriptSerializer s = new JavaScriptSerializer();
                lstReqItemDetails = s.Deserialize<List<RequisitionDetailsDTO>>(para);
            }

            QuickListDAL objQLDtlDAL = new QuickListDAL(SessionHelper.EnterPriseDBName);
            List<QuickListDetailDTO> objQLDtlDTO = new List<QuickListDetailDTO>();

            if (ModuleName.ToLower().Equals("retord"))
                objQLDtlDTO = objQLDtlDAL.GetQuickListItemsRecords(SessionHelper.RoomID, SessionHelper.CompanyID, QuickListGUID, SessionHelper.UserSupplierIds).Where(x => x.OnHandQuantity > 0).ToList();
            else
                objQLDtlDTO = objQLDtlDAL.GetQuickListItemsRecords(SessionHelper.RoomID, SessionHelper.CompanyID, QuickListGUID, SessionHelper.UserSupplierIds).ToList();

            TransferMasterDTO transferDTO = null;
            bool isValidTransfer = false;
            int transferRequestType = (int)RequestType.In;

            if (!string.IsNullOrEmpty(ModuleName) && ModuleName.ToLower().Equals("trf") && objQLDtlDTO != null && objQLDtlDTO.Any())
            {
                JavaScriptSerializer s = new JavaScriptSerializer();
                List<TransferDetailDTO> lstTransferItemDetails = s.Deserialize<List<TransferDetailDTO>>(para);
                TransferMasterDAL transferMasterDAL = new TransferMasterDAL(SessionHelper.EnterPriseDBName);
                transferDTO = transferMasterDAL.GetTransferByGuidPlain(lstTransferItemDetails.FirstOrDefault().TransferGUID);

                if (transferDTO != null)
                {
                    transferRequestType = transferDTO.RequestType;
                }

                isValidTransfer = transferDTO != null ? true : false;
            }

            List<ItemMasterDTO> objItemDTOs = new List<ItemMasterDTO>();
            ItemMasterDAL objItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);

            if (objQLDtlDTO.Count > 0)
            {
                foreach (QuickListDetailDTO qlItem in objQLDtlDTO)
                {
                    ItemMasterDTO tempItemDTO = new ItemMasterDTO();
                    tempItemDTO = objItemDAL.GetItemWithMasterTableJoins(null, qlItem.ItemGUID, SessionHelper.RoomID, SessionHelper.CompanyID);

                    if ((SessionHelper.UserSupplierIds != null && SessionHelper.UserSupplierIds.Any())
                        && (!SessionHelper.UserSupplierIds.Contains(tempItemDTO.SupplierID.GetValueOrDefault(0))))
                    {
                        tempItemDTO = null;
                    }

                    if (tempItemDTO != null && tempItemDTO.ID > 0)
                    {
                        if (isValidTransfer) //this will be true when (!string.IsNullOrEmpty(ModuleName) && ModuleName.ToLower().Equals("trf") && transferDTO != null)
                        {
                            if (transferRequestType == (int)RequestType.Out && qlItem.OnHandQuantity.GetValueOrDefault(0) < 1)
                            {
                                continue;
                            }

                            var replenishRoomItem = objItemDAL.GetItemByItemNumberPlain(tempItemDTO.ItemNumber, transferDTO.ReplenishingRoomID, transferDTO.CompanyID);

                            //Below given code block is to restrict adding item to list which are not matched in replenish room
                            if (!(replenishRoomItem != null && replenishRoomItem.IsActive && replenishRoomItem.IsOrderable && !replenishRoomItem.IsDeleted.Value
                                    && !replenishRoomItem.IsArchived.Value && replenishRoomItem.SerialNumberTracking.Equals(tempItemDTO.SerialNumberTracking)
                                    && replenishRoomItem.LotNumberTracking.Equals(tempItemDTO.LotNumberTracking)
                                    && replenishRoomItem.DateCodeTracking.Equals(tempItemDTO.DateCodeTracking)
                                    && replenishRoomItem.ItemType.Equals(tempItemDTO.ItemType)
                                    && replenishRoomItem.ItemType != 4))
                            {
                                continue;
                            }
                        }
                        double QuantityQL = 0.0;
                        tempItemDTO.BinID = qlItem.BinID;

                        if (!string.IsNullOrEmpty(QLQTY))
                            QuantityQL = qlItem.Quantity.GetValueOrDefault(0) * double.Parse(QLQTY);
                        else
                            QuantityQL = qlItem.Quantity.GetValueOrDefault(0);

                        if (IsLaborAllowed)
                        {
                            tempItemDTO.QuickListItemQTY = QuantityQL;
                            objItemDTOs.Add(tempItemDTO);
                        }
                        else if (tempItemDTO.ItemType != 4)
                        {
                            tempItemDTO.QuickListItemQTY = QuantityQL;
                            if (ModuleName.ToLower() == "ord" || ModuleName.ToLower() == "retord")
                            {
                                tempItemDTO.QuickListGUID = QuickListGUID;
                            }
                            objItemDTOs.Add(tempItemDTO);
                        }

                        if (ModuleName.ToLower() == "rq" && lstReqItemDetails != null && lstReqItemDetails.Count > 0)
                        {
                            tempItemDTO.UDF1 = lstReqItemDetails[0].PullUDF1;
                            tempItemDTO.UDF2 = lstReqItemDetails[0].PullUDF2;
                            tempItemDTO.UDF3 = lstReqItemDetails[0].PullUDF3;
                            tempItemDTO.UDF4 = lstReqItemDetails[0].PullUDF4;
                            tempItemDTO.UDF5 = lstReqItemDetails[0].PullUDF5;
                        }
                    }

                }

                if (ModuleName == "ORD")
                {
                    if (!AllowConsignedItemToOrder)
                    {
                        objItemDTOs = objItemDTOs.Where(x => x.Consignment == false).ToList();
                    }
                }

                if (!string.IsNullOrEmpty(ModuleName) && !string.IsNullOrWhiteSpace(ModuleName) && (ModuleName.ToLower() == "quote" || ModuleName.ToLower().Equals("trf")))
                {
                    objItemDTOs = objItemDTOs.Where(x => x.IsActive == true && x.IsOrderable == true).ToList();
                }
            }

            return Json(new { Message = message, Status = status, Items = objItemDTOs }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddQuickListToDetailTable(string para, string ModuleName, string QuickListGUID)
        {
            bool AllowConsignedItemToOrder = SessionHelper.GetAdminPermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowOrderToConsignedItem);


            string message = "";
            string status = "";
            bool IsLaborAllowed = false;

            if (ModuleName.Contains("AS") || ModuleName.Contains("WO") || ModuleName.Contains("KIT") || ModuleName.Contains("QL"))
                IsLaborAllowed = true;

            QuickListDAL objQLDtlDAL = new QuickListDAL(SessionHelper.EnterPriseDBName);
            List<QuickListDetailDTO> objQLDtlDTO = objQLDtlDAL.GetQuickListItemsRecords(SessionHelper.RoomID, SessionHelper.CompanyID, QuickListGUID, SessionHelper.UserSupplierIds).ToList();
            List<ItemMasterDTO> objItemDTOs = new List<ItemMasterDTO>();
            ItemMasterDAL objItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            if (objQLDtlDTO.Count > 0)
            {
                foreach (QuickListDetailDTO qlItem in objQLDtlDTO)
                {
                    ItemMasterDTO tempItemDTO = new ItemMasterDTO();
                    tempItemDTO = objItemDAL.GetItemWithoutJoins(null, qlItem.ItemGUID);
                    if (IsLaborAllowed)
                    {
                        tempItemDTO.QuickListItemQTY = qlItem.Quantity.GetValueOrDefault(0);
                        objItemDTOs.Add(tempItemDTO);
                    }
                    else if (tempItemDTO.ItemType != 4)
                    {
                        tempItemDTO.QuickListItemQTY = qlItem.Quantity.GetValueOrDefault(0);
                        objItemDTOs.Add(tempItemDTO);
                    }
                }

                if (ModuleName == "ORD")
                {
                    if (!AllowConsignedItemToOrder)
                    {
                        objItemDTOs = objItemDTOs.Where(x => x.Consignment == false).ToList();
                    }
                }
            }
            //message = "Item's Quantity/Amount updated successfully.";
            //status = "ok";
            return Json(new { Message = message, Status = status, Items = objItemDTOs }, JsonRequestBehavior.AllowGet);
            //return Json(new { Message = message, Status = status, Items = objQLDtlDTO }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        public JsonResult GetLocationsinItemMaster(bool? ShowStagLocs, bool? ShowLabourItems, string ItemModelCallFrom = "", Int64 ParentID = 0)
        {
            using (var context = new eTurnsEntities(DbConnectionHelper.GeteTurnsEntityFWConnectionString(SessionHelper.EnterPriseDBName, DbConnectionType.EFReadWrite.ToString("F"))))
            {
                List<ItemMasterDTO> lstAllItems = null;
                if (ItemModelCallFrom.ToLower() == "icnt" && ParentID != 0)
                {
                    //if (Session["ItemMasterListForCount"] != null)
                    //{
                    //    lstAllItems = (List<ItemMasterDTO>)Session["ItemMasterListForCount"];
                    //}
                    //else
                    //{
                    InventoryCountDAL objInventoryCountDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
                    InventoryCountDTO objInventoryCountDTO = objInventoryCountDAL.GetInventoryCountById(ParentID, SessionHelper.RoomID, SessionHelper.CompanyID);
                    if (objInventoryCountDTO != null)
                    {
                        //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));
                        //int LoadDataCount = Settinfile.Element("LoadNarrowDataCount") != null ? Convert.ToInt32(Settinfile.Element("LoadNarrowDataCount").Value) : 0;
                        int LoadDataCount = SiteSettingHelper.LoadNarrowDataCount != string.Empty ? Convert.ToInt32(SiteSettingHelper.LoadNarrowDataCount) : 0;
                        bool IsAllowConsignedCredit = eTurnsWeb.Helper.SessionHelper.GetModulePermission(eTurnsWeb.Helper.SessionHelper.ModuleList.AllowConsignedCreditPull, eTurnsWeb.Helper.SessionHelper.PermissionType.AllowPull);

                        ItemMasterDAL objItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
                        var tmpsupplierIds = new List<long>();
                        Dictionary<string, int> retData = new Dictionary<string, int>();
                        retData = objItemDAL.GetItemCountPopupNarrowSearch(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, "ItemLocation", tmpsupplierIds, IsAllowConsignedCredit, LoadDataCount, (ShowStagLocs ?? false), objInventoryCountDTO.GUID).ToDictionary(e => e.NSColumnText + "[###]" + e.NSColumnValue.ToString(), e => e.NSCount);
                        return Json(new { DDData = retData }, JsonRequestBehavior.AllowGet);
                        // lstAllItems = objInventoryCountDAL.GetPagedItemLocationsForCount(0, int.MaxValue, out TotalRecordCount, "", "ItemNumber Asc", SessionHelper.RoomID, SessionHelper.CompanyID, false, false, 0, ShowStagLocs.GetValueOrDefault(false), "", tmpsupplierIds, objInventoryCountDTO.GUID.ToString()).ToList();
                    }
                    else
                    {
                        lstAllItems = new List<ItemMasterDTO>();
                    }
                    //Session["ItemMasterListForCount"] = lstAllItems;
                    //}

                    var lstsupps = (from tmp in lstAllItems.Where(t => t.ParentBinId.GetValueOrDefault(0) != 0 && t.IsArchived == false && t.IsDeleted == false)
                                    orderby tmp.ParentBinName
                                    group tmp by new { tmp.ParentBinId, tmp.ParentBinName } into grp
                                    select new
                                    {
                                        count = grp.Count(),
                                        sid = grp.Key.ParentBinId,
                                        supname = grp.Key.ParentBinName.Replace("[|EmptyStagingBin|]", "")
                                    });

                    return Json(lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (ShowStagLocs ?? false)
                    {
                        //var lstsupps = (from msd in context.MaterialStagingPullDetails
                        //                join im in context.ItemMasters on msd.ItemGUID equals im.GUID
                        //                join bm in context.BinMasters on msd.StagingBinId equals bm.ID
                        //                where (msd.IsArchived ?? false) == false && (msd.IsDeleted ?? false) == false && msd.Room == SessionHelper.RoomID && msd.CompanyID == SessionHelper.CompanyID
                        //                orderby msd.StagingBinId
                        //                group msd by new { msd.StagingBinId, bm.BinNumber } into grpms
                        //                select new
                        //                {
                        //                    count = context.MaterialStagingPullDetails.Where(t => t.StagingBinId == grpms.Key.StagingBinId && (t.IsArchived ?? false) == false && (t.IsDeleted ?? false) == false && t.Room == SessionHelper.RoomID && t.CompanyID == SessionHelper.CompanyID).Select(t => t.ItemGUID).Distinct().Count(),
                        //                    sid = grpms.Key.StagingBinId,
                        //                    supname = grpms.Key.BinNumber
                        //                });
                        var lstsupps = (from BM in context.BinMasters
                                        join IM in context.ItemMasters on BM.ItemGUID equals IM.GUID
                                        join BM1 in context.BinMasters on BM.ParentBinId equals BM1.ID
                                        where BM.Room == SessionHelper.RoomID && (BM.IsArchived ?? false) == false && (IM.IsDeleted ?? false) == false
                                        && BM.IsDeleted == false && BM.IsStagingLocation == true
                                        && BM1.ParentBinId == null && BM.BinNumber != null
                                        && (ShowLabourItems == true || IM.ItemType != 4)
                                        group BM by new { BM.BinNumber, BM1.ID } into grpms
                                        select new
                                        {
                                            count = grpms.Count(),
                                            sid = grpms.Key.ID,
                                            supname = grpms.Key.BinNumber
                                        });


                        return Json(lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        //var lstsupps = (from msd in context.ItemLocationDetails
                        //                join im in context.ItemMasters on msd.ItemGUID equals im.GUID
                        //                join bm in context.BinMasters on msd.BinID equals bm.ID
                        //                where (msd.IsArchived ?? false) == false && (msd.IsDeleted ?? false) == false && msd.Room == SessionHelper.RoomID && msd.CompanyID == SessionHelper.CompanyID
                        //                orderby msd.BinID
                        //                group msd by new { msd.BinID, bm.BinNumber } into grpms
                        //                select new
                        //                {
                        //                    count = context.ItemLocationDetails.Where(t => t.BinID == grpms.Key.BinID && t.IsArchived == false && t.IsDeleted == false && t.Room == SessionHelper.RoomID && t.CompanyID == SessionHelper.CompanyID).Select(t => t.ItemGUID).Distinct().Count(),
                        //                    sid = grpms.Key.BinID,
                        //                    supname = grpms.Key.BinNumber
                        //                });
                        var lstsupps = (from BM in context.BinMasters
                                        join IM in context.ItemMasters on BM.ItemGUID equals IM.GUID
                                        join BM1 in context.BinMasters on BM.ParentBinId equals BM1.ID
                                        where BM.Room == SessionHelper.RoomID && (BM.IsArchived ?? false) == false && (IM.IsDeleted ?? false) == false
                                        && BM.IsDeleted == false && BM.IsStagingLocation == false
                                        && BM1.ParentBinId == null && BM.BinNumber != null
                                        && (ShowLabourItems == true || IM.ItemType != 4)
                                        group BM by new { BM.BinNumber, BM1.ID } into grpms
                                        select new
                                        {
                                            count = grpms.Count(),
                                            sid = grpms.Key.ID,
                                            supname = grpms.Key.BinNumber
                                        });



                        return Json(lstsupps.OrderBy(t => t.supname).AsParallel().ToDictionary(e => e.supname + "[###]" + e.sid.ToString(), e => (int)e.count), JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }

        public void AddReportParamsToSesstion(string Ids, string Title, string StartDate, string EndDate, string DisplayFields, string CompanyIds, string RoomIds, string BarcodeColumn)
        {
            string[] arrcomp = CompanyIds.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] arrRooms = RoomIds.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (arrcomp == null || arrcomp.Length <= 0)
                CompanyIds = SessionHelper.CompanyID.ToString();

            if (arrRooms == null || arrRooms.Length <= 0)
                RoomIds = SessionHelper.RoomID.ToString();

            Session.Add("Ids", Ids);
            Session.Add("Title", Title);
            Session.Add("StartDate", StartDate);
            Session.Add("EndDate", EndDate);
            Session.Add("DisplayFields", DisplayFields);
            Session.Add("CompanyIds", CompanyIds);
            Session.Add("RoomIds", RoomIds);
            Session.Add("BarcodeColumn", BarcodeColumn);
        }

        [OutputCache(Duration = 0, VaryByParam = "None")]
        public JsonResult GetMultiSelectDD(string TableName, string CompanyIds)
        {
            Dictionary<string, Int64> retData = new Dictionary<string, Int64>();
            if (TableName == "CompanyMaster")
            {
                foreach (CompanyMasterDTO item in SessionHelper.CompanyList)
                {
                    retData.Add(item.Name, item.ID);
                }
            }
            else if (TableName == "Room")
            {
                if (!string.IsNullOrEmpty(CompanyIds))
                {
                    RoomDAL objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
                    foreach (var item in CompanyIds.Split(','))
                    {
                        if (!string.IsNullOrEmpty(item.Trim()))
                        {
                            //List<RoomDTO> RoomData = SessionHelper.RoomList;

                            List<RoomDTO> RoomData = GetRoomDataForUserBasedOnCompany(SessionHelper.EnterPriceID, Int64.Parse(item), SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.UserType, SessionHelper.RoleID, "company", SessionHelper.EnterPriceName, "", SessionHelper.RoomName);

                            //RoomData = RoomData.Where(x => x.CompanyID == Int64.Parse(item)).ToList();

                            foreach (RoomDTO item1 in RoomData)
                            {
                                retData.Add(item1.RoomName, item1.ID);
                            }
                        }
                    }
                }
                else
                    retData = new Dictionary<string, long>();
            }
            return Json(new { DDData = retData }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Company Config
        /// </summary>
        /// <returns></returns>
        public ActionResult CompanyConfig()
        {
            CompanyConfigDAL objCompanyMasterDAL = new CompanyConfigDAL(SessionHelper.EnterPriseDBName);
            ViewBag.DateTimeFormatBag = GetDateTimeFormat();
            return View(objCompanyMasterDAL.GetCompanyConfigByCompanyID(SessionHelper.CompanyID));
        }

        /// <summary>
        /// Company Config
        /// </summary>
        /// <returns></returns>
        public ActionResult EnterPriseConfig()
        {
            EnterPriseConfigDAL objCompanyMasterDAL = new EnterPriseConfigDAL(SessionHelper.EnterPriseDBName);
            ViewBag.DateTimeFormatBag = GetDateTimeFormat();
            return View(objCompanyMasterDAL.GetRecord(SessionHelper.EnterPriceID));
        }

        //public ActionResult AjaxFileDownload()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public FileContentResult DownLoadContentFile()
        //{

        //    var filresult = File(new System.Text.UTF8Encoding().GetBytes(csv), "application/csv", "downloaddocuments.csv");
        //    Response.Clear();
        //    Response.Buffer = true;
        //    Response.ContentType = "application/vnd.ms-excel";
        //    Response.AddHeader("content-disposition", "attachment; filename=Statement_" + "Downloadfile" + ".csv");
        //    Response.Write(csv);
        //    Response.Flush();
        //    return filresult;
        //}
        [NonAction]

        private List<CommonDTO> GetDateTimeFormat()
        {
            List<CommonDTO> ItemType = new List<CommonDTO>();
            ItemType.Add(new CommonDTO() { Text = "m/d/yy" });
            //ItemType.Add(new CommonDTO() { Text = "d/m/yy" });
            ItemType.Add(new CommonDTO() { Text = "yy/m/d" });

            return ItemType;
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult CompanyConfigSave(CompanyConfigDTO objDTO)
        {
            string message = "";
            string status = "";
            //UnitMasterController obj = new UnitMasterController();
            CompanyConfigDAL obj = new CompanyConfigDAL(SessionHelper.EnterPriseDBName);
            objDTO.CompanyID = SessionHelper.CompanyID;

            if (!ModelState.IsValid)
            {
                message = ResMessage.InvalidModel;// "Invalid Data!";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }

            //eTurnsWeb.Helper.SessionHelper.QuantityFormat = "";
            //eTurnsWeb.Helper.SessionHelper.PriceFormat = "";
            //eTurnsWeb.Helper.SessionHelper.TurnUsageFormat = "";

            if (objDTO.ID > 0)
            {

                if (objDTO.DateFormat == "m/d/yy")
                    objDTO.DateFormatCSharp = "MM/dd/yyyy";
                if (objDTO.DateFormat == "yy/m/d")
                    objDTO.DateFormatCSharp = "yyyy/MM/dd";

                if (obj.Edit(objDTO))
                {

                    // eTurnsWeb.Helper.SessionHelper.CompanyConfig = objDTO;

                    //if (SessionHelper.CompanyConfig.QuantityDecimalPoints != null)
                    //{
                    //    string strQuantityFormat = "{0:0}";
                    //    if ((int)eTurnsWeb.Helper.SessionHelper.CompanyConfig.QuantityDecimalPoints > 0)
                    //    {
                    //        int iQCount = Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.CompanyConfig.QuantityDecimalPoints);
                    //        strQuantityFormat = "{0:";
                    //        for (int iq = 0; iq <= iQCount; iq++)
                    //        {
                    //            if (iq == 0)
                    //            {
                    //                strQuantityFormat += "0.";
                    //            }
                    //            else
                    //            {
                    //                strQuantityFormat += "0";
                    //            }

                    //        }
                    //        strQuantityFormat += "}";
                    //    }
                    //    SessionHelper.QuantityFormat = strQuantityFormat;
                    //}

                    //if (eTurnsWeb.Helper.SessionHelper.CompanyConfig.CostDecimalPoints != null)
                    //{
                    //    string strPriceFormat = "{0:0}";
                    //    if ((int)eTurnsWeb.Helper.SessionHelper.CompanyConfig.CostDecimalPoints > 0)
                    //    {
                    //        int iQCount = Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.CompanyConfig.CostDecimalPoints);
                    //        strPriceFormat = "{0:";
                    //        for (int iq = 0; iq <= iQCount; iq++)
                    //        {
                    //            if (iq == 0)
                    //            {
                    //                strPriceFormat += "0.";
                    //            }
                    //            else
                    //            {
                    //                strPriceFormat += "0";
                    //            }

                    //        }
                    //        strPriceFormat += "}";
                    //        eTurnsWeb.Helper.SessionHelper.CompanyConfig.CostDecimalPoints = iQCount;
                    //    }
                    //    SessionHelper.PriceFormat = strPriceFormat;

                    //}
                    //if (SessionHelper.CompanyConfig.TurnsAvgDecimalPoints != null)
                    //{
                    //    string strTurnsAvgFormat = "{0:0}";
                    //    if ((int)eTurnsWeb.Helper.SessionHelper.CompanyConfig.TurnsAvgDecimalPoints > 0)
                    //    {
                    //        int iQCount = Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.CompanyConfig.TurnsAvgDecimalPoints);
                    //        strTurnsAvgFormat = "{0:";
                    //        for (int iq = 0; iq <= iQCount; iq++)
                    //        {
                    //            if (iq == 0)
                    //            {
                    //                strTurnsAvgFormat += "0.";
                    //            }
                    //            else
                    //            {
                    //                strTurnsAvgFormat += "0";
                    //            }

                    //        }
                    //        strTurnsAvgFormat += "}";
                    //        eTurnsWeb.Helper.SessionHelper.CompanyConfig.TurnsAvgDecimalPoints = iQCount;
                    //    }
                    //    SessionHelper.TurnUsageFormat = strTurnsAvgFormat;
                    //}

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

        [ValidateAntiForgeryToken]
        public JsonResult EnterPriseConfigSave(EnterPriseConfigDTO objDTO)
        {
            string message = "";
            string status = "";
            //UnitMasterController obj = new UnitMasterController();
            EnterPriseConfigDAL obj = new EnterPriseConfigDAL(SessionHelper.EnterPriseDBName);
            objDTO.EnterPriseID = SessionHelper.EnterPriceID;

            if (!ModelState.IsValid)
            {
                message = ResMessage.InvalidModel;// "Invalid Data!";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            if (!string.IsNullOrEmpty(Convert.ToString(objDTO.PasswordExpiryDays)))
            {
                if (Convert.ToInt32(objDTO.PasswordExpiryDays) <= Convert.ToInt32(objDTO.PasswordExpiryWarningDays ?? 0))
                {
                    message = ResMessage.InvalidPasswordExpiryMsg;// "Invalid Data!";
                    status = "fail";
                    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
                }
            }


            eTurnsWeb.Helper.SessionHelper.QuantityFormat = "";
            eTurnsWeb.Helper.SessionHelper.PriceFormat = "";
            eTurnsWeb.Helper.SessionHelper.WeightFormat = "";
            eTurnsWeb.Helper.SessionHelper.TurnUsageFormat = "";

            if (objDTO.ID > 0)
            {
                if (objDTO.DateFormat == "m/d/yy")
                    objDTO.DateFormatCSharp = "MM/dd/yyyy";
                if (objDTO.DateFormat == "yy/m/d")
                    objDTO.DateFormatCSharp = "yyyy/MM/dd";

                if (obj.Edit(objDTO))
                {

                    //eTurnsWeb.Helper.SessionHelper.CompanyConfig = objDTO;
                    string strQuantityFormat = "{0:0}";
                    if (!string.IsNullOrWhiteSpace(SessionHelper.NumberDecimalDigits))
                    {

                        if (Convert.ToInt32(Convert.ToString(eTurnsWeb.Helper.SessionHelper.NumberDecimalDigits)) > 0)
                        {
                            int iQCount = Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.NumberDecimalDigits);
                            strQuantityFormat = "{0:";
                            for (int iq = 0; iq <= iQCount; iq++)
                            {
                                if (iq == 0)
                                {
                                    strQuantityFormat += "0.";
                                }
                                else
                                {
                                    strQuantityFormat += "0";
                                }

                            }
                            strQuantityFormat += "}";
                        }
                        SessionHelper.QuantityFormat = strQuantityFormat;
                    }
                    else
                        SessionHelper.QuantityFormat = strQuantityFormat;

                    string strPriceFormat = "{0:0}";
                    if (!string.IsNullOrWhiteSpace(SessionHelper.CurrencyDecimalDigits))
                    {

                        //if ((int)eTurnsWeb.Helper.SessionHelper.CompanyConfig.CurrencySymbol > 0)
                        if (Convert.ToInt32(Convert.ToString(eTurnsWeb.Helper.SessionHelper.CurrencyDecimalDigits)) > 0)
                        {
                            int iQCount = Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.CurrencyDecimalDigits);
                            strPriceFormat = "{0:";
                            for (int iq = 0; iq <= iQCount; iq++)
                            {
                                if (iq == 0)
                                {
                                    strPriceFormat += "0.";
                                }
                                else
                                {
                                    strPriceFormat += "0";
                                }

                            }
                            strPriceFormat += "}";
                        }
                        SessionHelper.PriceFormat = strPriceFormat;
                    }
                    else
                        SessionHelper.PriceFormat = strPriceFormat;

                    string strTurnsAvgFormat = "{0:0}";
                    if (!string.IsNullOrWhiteSpace(SessionHelper.NumberAvgDecimalPoints))
                    {
                        if (Convert.ToInt32(Convert.ToString(eTurnsWeb.Helper.SessionHelper.NumberAvgDecimalPoints)) > 0)
                        {
                            int iQCount = Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.NumberAvgDecimalPoints);
                            strTurnsAvgFormat = "{0:";
                            for (int iq = 0; iq <= iQCount; iq++)
                            {
                                if (iq == 0)
                                {
                                    strTurnsAvgFormat += "0.";
                                }
                                else
                                {
                                    strTurnsAvgFormat += "0";
                                }

                            }
                            strTurnsAvgFormat += "}";
                        }
                        SessionHelper.TurnUsageFormat = strTurnsAvgFormat;
                    }
                    else
                        SessionHelper.TurnUsageFormat = strTurnsAvgFormat;

                    string strWghtFormat = "{0:0}";

                    if (!string.IsNullOrWhiteSpace(SessionHelper.WeightDecimalPoints))
                    {
                        if (Convert.ToInt32(Convert.ToString(eTurnsWeb.Helper.SessionHelper.WeightDecimalPoints)) > 0)
                        {
                            int iQCount = Convert.ToInt32(eTurnsWeb.Helper.SessionHelper.WeightDecimalPoints);
                            strWghtFormat = "{0:";
                            for (int iq = 0; iq <= iQCount; iq++)
                            {
                                if (iq == 0)
                                {
                                    strWghtFormat += "0.";
                                }
                                else
                                {
                                    strWghtFormat += "0";
                                }

                            }
                            strWghtFormat += "}";
                        }
                        SessionHelper.WeightFormat = strWghtFormat;
                    }
                    else
                        SessionHelper.WeightFormat = strWghtFormat;

                    message = ResMessage.SaveMessage;
                    status = "ok";
                }
            }
            else if (objDTO.ID == 0)
            {
                EnterPriseConfigDAL objCompanyMasterDAL = new EnterPriseConfigDAL(SessionHelper.EnterPriseDBName);
                if (objDTO.DateFormat == "m/d/yy")
                    objDTO.DateFormatCSharp = "MM/dd/yyyy";
                if (objDTO.DateFormat == "yy/m/d")
                    objDTO.DateFormatCSharp = "yyyy/MM/dd";

                Int64 Id = obj.Insert(objDTO);
                objDTO.ID = Id;
                message = ResMessage.SaveMessage;
                status = "ok";
            }
            else
            {
                message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
                status = "fail";
            }
            return Json(new { Message = message, Status = status, ID = objDTO.ID }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult eVMISetup()
        {
            eVMISetupDAL objeVMIDAL = new eVMISetupDAL(SessionHelper.EnterPriseDBName);
            ComPortRoomMappingDAL objComPortRoomMappingDAL = new ComPortRoomMappingDAL(SessionHelper.EnterPriseDBName);
            eVMISetupDTO objDto = new eVMISetupDTO();
            objDto = objeVMIDAL.GetRecord(SessionHelper.RoomID, SessionHelper.CompanyID);

            RoomDTO objRoomInfo = new RoomDTO();
            string columnList = "ID,RoomName,DefaultCountType";
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            NotificationDAL objNotificDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
            objRoomInfo = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");

            if (objDto == null)
            {
                objDto = new eVMISetupDTO();
                objDto.PollType = 0;
                objDto.CountType = Convert.ToChar(InventoryCountType.Adjustment).ToString();
                if (objRoomInfo != null)
                    objDto.CountType = objRoomInfo.DefaultCountType;
            }
            if (string.IsNullOrWhiteSpace(objDto.CountType))
            {
                objDto.CountType = Convert.ToChar(InventoryCountType.Adjustment).ToString();
                if (objRoomInfo != null)
                    objDto.CountType = objRoomInfo.DefaultCountType;
            }
            if (objDto != null)
            {
                if (objDto.NextPollDate != null)
                    objDto.NextRunDateTime = eTurnsWeb.Helper.CommonUtility.ConvertDateByTimeZone(objDto.NextPollDate, eTurnsWeb.Helper.SessionHelper.CurrentTimeZone, eTurnsWeb.Helper.SessionHelper.DateTimeFormat, eTurnsWeb.Helper.SessionHelper.RoomCulture, true);

                SchedulerDTO objScheduleDTO = objNotificDAL.GetScheduleByRoomScheduleFor(SessionHelper.RoomID, SessionHelper.CompanyID, (int)eVMIScheduleFor.eVMISchedule);
                if (objScheduleDTO != null)
                    objDto.IsActiveSchedule = objScheduleDTO.IsScheduleActive;

            }
            ViewBag.CountTypeBag = GetCountTypeOptions();


            bool IsOldeVMIRoom = false;
            string CurrentRoomFullId = SessionHelper.EnterPriceID + "_" + SessionHelper.CompanyID + "_" + SessionHelper.RoomID;
            if ((SiteSettingHelper.eVMIRooms ?? string.Empty).ToLower().Contains(CurrentRoomFullId.ToLower()))
            {
                IsOldeVMIRoom = true;
            }

            //string streVMIRooms = SiteSettingHelper.eVMIRooms;
            //if (!string.IsNullOrWhiteSpace(streVMIRooms))
            //{
            //    string[] arrEntCmpRoom = streVMIRooms.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            //    if (arrEntCmpRoom != null && arrEntCmpRoom.Length > 0)
            //    {
            //        foreach (string strEntCmpRoom in arrEntCmpRoom)
            //        {
            //            string[] EntCmpRoom = strEntCmpRoom.Split(new char[1] { '_' }, StringSplitOptions.RemoveEmptyEntries);
            //            if (EntCmpRoom != null && EntCmpRoom.Length > 0 && SessionHelper.RoomID == Convert.ToInt64(EntCmpRoom[2]))
            //            {
            //                IsOldeVMIRoom = true;
            //                break;
            //            }
            //        }
            //    }
            //}

            ComPortMasterDAL objComPortDAL = new ComPortMasterDAL(SessionHelper.EnterPriseDBName);
            List<ComPortMasterDTO> lstComPort = objComPortDAL.GetComPortMasterByCompanyRoom(SessionHelper.CompanyID, SessionHelper.RoomID);
            if (lstComPort != null && lstComPort.Count > 0)
            {
                ComPortMasterDTO objSelectCom = lstComPort.Where(x => x.IsSelected == true).FirstOrDefault();
                if (objSelectCom != null)
                    objDto.ComPortMasterID = objSelectCom.ID;
            }
            ViewBag.ComPortBag = lstComPort;
            ViewBag.RoomCOMMap = objComPortRoomMappingDAL.GetComPortMappingByCompanyRoomID(SessionHelper.CompanyID, SessionHelper.RoomID);
            if (IsOldeVMIRoom)
            {
                return RedirectToAction("eVMISetting");
            }
            else
            {
                return View("eVMISetup", objDto);
            }

        }

        public ActionResult eVMIRequest()
        {
            //BinMasterDAL objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            //List<BinMasterDTO> lstBinMaster = objBinDAL.GetItemLocatoinForeVMI(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).ToList();
            ViewBag.ItemBinBag = new List<BinMasterDTO>();
            List<ComPortRoomMappingDTO> lstmappings = new List<ComPortRoomMappingDTO>();
            if ((SessionHelper.isEVMI ?? false))
            {
                lstmappings = new ComPortRoomMappingDAL(SessionHelper.EnterPriseDBName).GetComPortMappingByCompanyRoomID(SessionHelper.CompanyID, SessionHelper.RoomID);
            }
            List<SelectListItem> lstComMappings = new List<SelectListItem>();
            foreach (var item in lstmappings)
            {
                lstComMappings.Add(new SelectListItem() { Text = item.ComPortName, Value = item.ID.ToString() });
            }
            ViewBag.RoomCOMMappings = lstComMappings;
            ViewBag.RequestTypeBag = GeteVMIRequestOptions();

            return View();

        }

        public ActionResult eVMISetting()
        {
            eVMISetupDAL objeVMIDAL = new eVMISetupDAL(SessionHelper.EnterPriseDBName);
            eVMISetupDTO objDto = new eVMISetupDTO();
            objDto = objeVMIDAL.GetRecord(SessionHelper.RoomID, SessionHelper.CompanyID);

            RoomDTO objRoomInfo = new RoomDTO();
            string columnList = "ID,RoomName,DefaultCountType";
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            objRoomInfo = objCommonDAL.GetSingleRecord<RoomDTO>(columnList, "Room", "ID = " + SessionHelper.RoomID.ToString() + "", "");

            if (objDto == null)
            {
                objDto = new eVMISetupDTO();
                objDto.PollType = 1;
                objDto.CountType = Convert.ToChar(InventoryCountType.Adjustment).ToString();
                if (objRoomInfo != null)
                    objDto.CountType = objRoomInfo.DefaultCountType;
            }
            if (string.IsNullOrWhiteSpace(objDto.CountType))
            {
                objDto.CountType = Convert.ToChar(InventoryCountType.Adjustment).ToString();
                if (objRoomInfo != null)
                    objDto.CountType = objRoomInfo.DefaultCountType;
            }
            if (objDto != null && objDto.NextPollDate != null)
            {
                objDto.NextRunDateTime = eTurnsWeb.Helper.CommonUtility.ConvertDateByTimeZone(objDto.NextPollDate, eTurnsWeb.Helper.SessionHelper.CurrentTimeZone, eTurnsWeb.Helper.SessionHelper.DateTimeFormat, eTurnsWeb.Helper.SessionHelper.RoomCulture, true);
            }
            ViewBag.CountTypeBag = GetCountTypeOptions();

            bool IsOldeVMIRoom = false;
            string CurrentRoomFullId = SessionHelper.EnterPriceID + "_" + SessionHelper.CompanyID + "_" + SessionHelper.RoomID;
            if ((SiteSettingHelper.eVMIRooms ?? string.Empty).ToLower().Contains(CurrentRoomFullId.ToLower()))
            {
                IsOldeVMIRoom = true;
            }
            //string streVMIRooms = SiteSettingHelper.eVMIRooms;
            //bool IsOldeVMIRoom = false;
            //if (!string.IsNullOrWhiteSpace(streVMIRooms))
            //{
            //    string[] arrEntCmpRoom = streVMIRooms.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            //    if (arrEntCmpRoom != null && arrEntCmpRoom.Length > 0)
            //    {
            //        foreach (string strEntCmpRoom in arrEntCmpRoom)
            //        {
            //            string[] EntCmpRoom = strEntCmpRoom.Split(new char[1] { '_' }, StringSplitOptions.RemoveEmptyEntries);
            //            if (EntCmpRoom != null && EntCmpRoom.Length > 0 && SessionHelper.RoomID == Convert.ToInt64(EntCmpRoom[2]))
            //            {
            //                IsOldeVMIRoom = true;
            //                break;
            //            }
            //        }
            //    }
            //}
            if (IsOldeVMIRoom)
            {
                return View("eVMISetting", objDto);

            }
            else
            {
                return RedirectToAction("eVMISetup");
            }

        }

        private List<CommonDTO> GetCountTypeOptions()
        {
            List<CommonDTO> CountType = new List<CommonDTO>();
            CountType.Add(new CommonDTO() { Value = "A", Text = ResInventoryCount.Adjustment });
            CountType.Add(new CommonDTO() { Value = "M", Text = ResInventoryCount.Manual });
            return CountType;
        }

        private List<CommonDTO> GeteVMIRequestOptions()
        {
            List<CommonDTO> PollOpeartion = new List<CommonDTO>();
            PollOpeartion.Add(new CommonDTO() { Value = "1", Text = ResLayout.Poll });
            PollOpeartion.Add(new CommonDTO() { Value = "2", Text = ResLayout.Tare });
            PollOpeartion.Add(new CommonDTO() { Value = "15", Text = ResLayout.TareAll });
            PollOpeartion.Add(new CommonDTO() { Value = "3", Text = ResLayout.Calibrate });
            PollOpeartion.Add(new CommonDTO() { Value = "5", Text = ResLayout.GetWeightPerPiece });
            PollOpeartion.Add(new CommonDTO() { Value = "6", Text = ResLayout.ResetScale });
            PollOpeartion.Add(new CommonDTO() { Value = "7", Text = ResLayout.GetShelfID });
            PollOpeartion.Add(new CommonDTO() { Value = "8", Text = ResLayout.SetShelfID });
            PollOpeartion.Add(new CommonDTO() { Value = "9", Text = ResLayout.GetVersion });
            PollOpeartion.Add(new CommonDTO() { Value = "10", Text = ResLayout.GetSerialNo });
            PollOpeartion.Add(new CommonDTO() { Value = "11", Text = ResLayout.GetModel });
            PollOpeartion.Add(new CommonDTO() { Value = "12", Text = ResLayout.SetModel });
            PollOpeartion.Add(new CommonDTO() { Value = "13", Text = ResLayout.GetCalibrateWeight });
            PollOpeartion.Add(new CommonDTO() { Value = "14", Text = ResLayout.SetCalibrateWeight });

            return PollOpeartion;//.OrderBy(t => t.Text).ToList();
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult eVMISetupSave(eVMISetupDTO objDTO, SchedulerDTO objSchedulerDTO)
        {
            eVMISetupDAL objeVMISetupDAL = new eVMISetupDAL(SessionHelper.EnterPriseDBName);
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.Room = SessionHelper.RoomID;
            objSchedulerDTO.RoomId = SessionHelper.RoomID;
            objSchedulerDTO.SupplierId = 0;
            objSchedulerDTO.CreatedBy = SessionHelper.UserID;
            objSchedulerDTO.CompanyId = SessionHelper.CompanyID;

            if (objSchedulerDTO.IsScheduleChanged == 1)
            {
                objSchedulerDTO = objeVMISetupDAL.SaveeVMISchedule(objSchedulerDTO);
                objDTO.NextPollDate = null;
            }
            objDTO.RoomScheduleID = objSchedulerDTO.ScheduleID;
            objDTO.ScheduleTime = Convert.ToDateTime(objSchedulerDTO.ScheduleRunTime).TimeOfDay;

            objeVMISetupDAL.SaveeVMISetup(objDTO);
            if (objSchedulerDTO.IsScheduleChanged == 1)
            {
                objeVMISetupDAL.SetNexteVMIRunTime(objDTO.ID);
            }
            return Json(new { Message = ResMessage.SaveMessage, Status = "ok" }, JsonRequestBehavior.AllowGet);
        }

        //[ValidateAntiForgeryToken]
        //[HttpPost]
        //public JsonResult eVMISetupSave(eVMISetupDTO objDTO, SchedulerDTO objSchedulerDTO)
        //{
        //    string message = "";
        //    string status = "";
        //    eVMISetupDAL obj = new eVMISetupDAL(SessionHelper.EnterPriseDBName);
        //    objDTO.CompanyID = SessionHelper.CompanyID;
        //    objDTO.Room = SessionHelper.RoomID;
        //    objDTO.RoomName = SessionHelper.RoomName;
        //    bool IsNextDaySchedule = false;

        //    //if (!ModelState.IsValid)
        //    //{
        //    //    message = ResMessage.InvalidModel;// "Invalid Data!";
        //    //    status = "fail";
        //    //    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        //    //}

        //    string CurrentRoomTimeZone = "UTC";
        //    RegionSettingDAL objRegionSettingDAL = new RegionSettingDAL(SessionHelper.EnterPriseDBName);
        //    eTurnsRegionInfo objRegionalSettings = objRegionSettingDAL.GetRegionSettingsById(SessionHelper.RoomID, SessionHelper.CompanyID, 0);
        //    if (objRegionalSettings != null)
        //    {
        //        CurrentRoomTimeZone = objRegionalSettings.TimeZoneName ?? "UTC";
        //    }

        //    DateTime CurrentTimeofTimeZone = DateTimeUtility.ConvertDateFromUTC(CurrentRoomTimeZone, DateTime.UtcNow);
        //    if (objDTO.PollType == 1)
        //    {
        //        //objDTO.NextPollDate = CurrentTimeofTimeZone.ToUniversalTime().AddMinutes(objDTO.PollInterval.GetValueOrDefault(0));
        //    }
        //    else if (objDTO.PollType == 2)
        //    {
        //        List<TimeSpan> lstPollTime = new List<TimeSpan>();
        //        if (objDTO.PollTime1 != null)
        //            lstPollTime.Add((TimeSpan)objDTO.PollTime1);
        //        if (objDTO.PollTime2 != null)
        //            lstPollTime.Add((TimeSpan)objDTO.PollTime2);
        //        if (objDTO.PollTime3 != null)
        //            lstPollTime.Add((TimeSpan)objDTO.PollTime3);
        //        if (objDTO.PollTime4 != null)
        //            lstPollTime.Add((TimeSpan)objDTO.PollTime4);
        //        if (objDTO.PollTime5 != null)
        //            lstPollTime.Add((TimeSpan)objDTO.PollTime5);
        //        if (objDTO.PollTime6 != null)
        //            lstPollTime.Add((TimeSpan)objDTO.PollTime6);

        //        TimeSpan? SchedulePollTime = null;

        //        if (lstPollTime != null && lstPollTime.Count > 0)
        //        {
        //            lstPollTime = lstPollTime.OrderBy(x => x).ToList();
        //            TimeSpan timeOfDay = CurrentTimeofTimeZone.TimeOfDay;
        //            foreach (TimeSpan item in lstPollTime)
        //            {
        //                if (item > timeOfDay)
        //                {
        //                    SchedulePollTime = item;
        //                    break;
        //                }
        //            }
        //            if (SchedulePollTime == null)
        //            {
        //                SchedulePollTime = lstPollTime.FirstOrDefault();
        //                IsNextDaySchedule = true;
        //            }
        //        }
        //        else
        //        {
        //            objDTO.PollTime1 = Convert.ToDateTime("01:00").TimeOfDay;
        //            SchedulePollTime = Convert.ToDateTime("01:00").TimeOfDay;

        //            return Json(new { Message = ReseVMISetup.ValidateSchedulePoll, Status = "fail" }, JsonRequestBehavior.AllowGet);
        //        }
        //        ////objSchedulerDTO.ScheduleTime = SchedulePollTime;
        //        objSchedulerDTO.ScheduleRunTime = SchedulePollTime.ToString();
        //    }

        //    eVMISetupDTO objeVMIExist = obj.GetRecord(SessionHelper.RoomID, SessionHelper.CompanyID);
        //    if (objeVMIExist != null && objeVMIExist.ID > 0)
        //    {
        //        objDTO.ID = objeVMIExist.ID;
        //        objDTO.NextPollDate = objeVMIExist.NextPollDate;
        //    }


        //    if (objDTO.ID > 0)
        //    {
        //        objDTO.LastUpdatedBy = SessionHelper.UserID;
        //        objDTO.UpdatedByName = SessionHelper.UserName;

        //        if (obj.Edit(objDTO))
        //        {
        //            message = ResMessage.SaveMessage;
        //            status = "ok";
        //        }
        //        else
        //        {
        //            message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
        //            status = "fail";
        //        }
        //    }
        //    else
        //    {
        //        objDTO.CreatedBy = SessionHelper.UserID;
        //        objDTO.LastUpdatedBy = SessionHelper.UserID;
        //        objDTO.CreatedByName = SessionHelper.UserName;
        //        objDTO.UpdatedByName = SessionHelper.UserName;

        //        if (obj.Insert(objDTO))
        //        {
        //            message = ResMessage.SaveMessage;
        //            status = "ok";
        //        }
        //        else
        //        {
        //            message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + HttpStatusCode.ExpectationFailed + ") Error! Record Not Saved";
        //            status = "fail";
        //        }
        //    }

        //    #region Schedular

        //    SupplierMasterDAL objSupplierMasterDAL = new SupplierMasterDAL(SessionHelper.EnterPriseDBName);
        //    NotificationDAL objNotificDAL = new NotificationDAL(SessionHelper.EnterPriseDBName);
        //    if (objSchedulerDTO != null)
        //    {
        //        if (objSchedulerDTO.RoomId == 0)
        //            objSchedulerDTO.RoomId = SessionHelper.RoomID;
        //        if (objSchedulerDTO.CompanyId == 0)
        //            objSchedulerDTO.CompanyId = SessionHelper.CompanyID;

        //        SchedulerDTO objExistScheduleDTO = objNotificDAL.GetScheduleByRoomScheduleFor(SessionHelper.RoomID, SessionHelper.CompanyID, (int)eVMIScheduleFor.eVMISchedule);
        //        if (objExistScheduleDTO != null && objSchedulerDTO.ScheduleID <= 0)
        //        {
        //            objSchedulerDTO.ScheduleID = objExistScheduleDTO.ScheduleID;
        //        }
        //        DateTime datetimetoConsider = objRegionSettingDAL.GetCurrentDatetimebyTimeZone(objSchedulerDTO.RoomId, objSchedulerDTO.CompanyId, objSchedulerDTO.CreatedBy);
        //        objSchedulerDTO.LoadSheduleFor = (int)eVMIScheduleFor.eVMISchedule;
        //        if (string.IsNullOrWhiteSpace(objSchedulerDTO.ScheduleRunTime))
        //            objSchedulerDTO.ScheduleRunTime = "01:00";
        //        objSchedulerDTO.ScheduleTime = Convert.ToDateTime(objSchedulerDTO.ScheduleRunTime).TimeOfDay;

        //        string strtmp = datetimetoConsider.ToShortDateString() + " " + objSchedulerDTO.ScheduleRunTime;
        //        objSchedulerDTO.ScheduleRunDateTime = Convert.ToDateTime(strtmp);
        //        objSchedulerDTO.ScheduleRunDateTime = objRegionSettingDAL.ConvertTimeToUTCTime(objSchedulerDTO.ScheduleRunDateTime, objSchedulerDTO.RoomId, objSchedulerDTO.CompanyId, objSchedulerDTO.CreatedBy) ?? objSchedulerDTO.ScheduleRunDateTime;
        //        if (IsNextDaySchedule)
        //            objSchedulerDTO.ScheduleRunDateTime = objSchedulerDTO.ScheduleRunDateTime.AddDays(1);

        //        if (objDTO.PollType == 1)
        //        {
        //            objSchedulerDTO.NextRunDate = null;
        //            objSchedulerDTO.IsScheduleActive = objDTO.IsActiveSchedule;
        //            objSchedulerDTO = objSupplierMasterDAL.SaveSchedule(objSchedulerDTO);
        //            obj.UpdateNextRoomSchedulePollDate(objSchedulerDTO.ScheduleID);
        //        }
        //        else if (objDTO.PollType == 2)
        //        {
        //            objSchedulerDTO.IsScheduleActive = objDTO.IsActiveSchedule;
        //            objSchedulerDTO.NextRunDate = objSchedulerDTO.ScheduleRunDateTime;
        //            objSchedulerDTO.IsScheduleActive = objDTO.IsActiveSchedule;
        //            objDTO.NextPollDate = objSchedulerDTO.ScheduleRunDateTime;
        //            objSchedulerDTO = objSupplierMasterDAL.SaveSchedule(objSchedulerDTO);
        //        }
        //        eVMISetupDTO objeVMI = obj.GetRecord(SessionHelper.RoomID, SessionHelper.CompanyID);
        //        if (objeVMI != null)
        //        {
        //            objeVMI.ScheduleTime = objSchedulerDTO.ScheduleTime;
        //            objeVMI.RoomScheduleID = objSchedulerDTO.ScheduleID;
        //            objeVMI.NextPollDate = objDTO.NextPollDate;
        //            obj.Edit(objeVMI);
        //        }

        //    }
        //    #endregion


        //    //#region Comport
        //    //if (!string.IsNullOrWhiteSpace(objDTO.SelectedComPort) && SessionHelper.isEVMI == true)
        //    //{
        //    //    if (!objDTO.SelectedComPort.Contains(","))
        //    //    {
        //    //        ComPortRoomMappingDAL objComPortMapDAL = new ComPortRoomMappingDAL(SessionHelper.EnterPriseDBName);
        //    //        bool IsComportValidate = false;
        //    //        IsComportValidate = objComPortMapDAL.ValidateComPortMapping(objDTO.SelectedComPort, SessionHelper.RoomID, SessionHelper.CompanyID);
        //    //        if (IsComportValidate)
        //    //        {
        //    //            ComPortRoomMappingDTO objComPortMapDTO = new ComPortRoomMappingDTO();
        //    //            objComPortMapDTO.SelectedComPortMasterIDs = objDTO.SelectedComPort ?? string.Empty;
        //    //            objComPortMapDTO.RoomID = SessionHelper.RoomID;
        //    //            objComPortMapDTO.CompanyID = SessionHelper.CompanyID;
        //    //            objComPortMapDTO.CreatedBy = SessionHelper.UserID;
        //    //            objComPortMapDAL.InsertComPortmapping(objComPortMapDTO);

        //    //            message = ResMessage.SaveMessage;
        //    //            status = "ok";
        //    //        }
        //    //        else
        //    //        {
        //    //            message = string.Format(ReseVMISetup.ComportValidate);
        //    //            status = "fail";
        //    //        }
        //    //    }
        //    //    else
        //    //    {
        //    //        message = string.Format(ReseVMISetup.ComportSingleValidate);
        //    //        status = "fail";
        //    //    }

        //    //}
        //    //else if (string.IsNullOrWhiteSpace(objDTO.SelectedComPort) && SessionHelper.isEVMI == true)
        //    //{
        //    //    message = string.Format(ReseVMISetup.ComportRequire);
        //    //    status = "fail";
        //    //}
        //    //#endregion


        //    //#region Inventory Count
        //    //InventoryCountDAL objICDAL = new InventoryCountDAL(SessionHelper.EnterPriseDBName);
        //    //string CountName = string.Empty;
        //    //if (objDTO.CountType == "A")
        //    //    CountName = "EVMI_Adjustment";
        //    //else if (objDTO.CountType == "M")
        //    //    CountName = "EVMI_Manual";

        //    //objICDAL.SeteVMICountHeader(SessionHelper.CompanyID, SessionHelper.RoomID, CountName, objDTO.CountType, SessionHelper.UserID, "Web");
        //    //#endregion

        //    return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        //}


        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult eVMISettingSave(eVMISetupDTO objDTO, SchedulerDTO objSchedulerDTO)
        {
            string message = "";
            string status = "";
            eVMISetupDAL obj = new eVMISetupDAL(SessionHelper.EnterPriseDBName);
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.RoomName = SessionHelper.RoomName;

            if (objDTO.ID > 0)
            {
                objDTO.LastUpdatedBy = SessionHelper.UserID;
                objDTO.UpdatedByName = SessionHelper.UserName;

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
                objDTO.CreatedBy = SessionHelper.UserID;
                objDTO.LastUpdatedBy = SessionHelper.UserID;
                objDTO.CreatedByName = SessionHelper.UserName;
                objDTO.UpdatedByName = SessionHelper.UserName;

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


        //public ActionResult Reports()
        //{
        //    eTurnsWeb.localhost2012.ReportingService2010 rs = new eTurnsWeb.localhost2012.ReportingService2010();
        //    ReportServices objReportServices;
        //    string Username = ConfigurationManager.AppSettings["NetworkUser"];
        //    string Password = ConfigurationManager.AppSettings["NetworkPassword"];
        //    string Domain = ConfigurationManager.AppSettings["NetworkDomain"];
        //    string DatabaseUsername = ConfigurationManager.AppSettings["DbUserName"];
        //    string DatabasePassword = ConfigurationManager.AppSettings["DbPassword"];
        //    string ReportServerUrl = ConfigurationManager.AppSettings["ReportServerURL"];
        //    string ReportsUrl = ConfigurationManager.AppSettings["ReportsUrl"];
        //    objReportServices = new ReportServices(rs, Domain, Username, Password, DatabaseUsername, DatabasePassword);
        //    Dictionary<string, string> dt = objReportServices.GetTransactionReportList();
        //    ViewBag.ReportListBag = dt;
        //    return View("Reports");
        //}

        //public JsonResult CopyReportsFromSourceToDest(string SourceReportName, string FoldarName, string DestinationReportName, string ReportPath)
        //{
        //    try
        //    {
        //        eTurnsWeb.localhost2012.ReportingService2010 rs = new eTurnsWeb.localhost2012.ReportingService2010();
        //        ReportServices objReportServices;
        //        string Username = ConfigurationManager.AppSettings["NetworkUser"];
        //        string Password = ConfigurationManager.AppSettings["NetworkPassword"];
        //        string Domain = ConfigurationManager.AppSettings["NetworkDomain"];
        //        string DatabaseUsername = ConfigurationManager.AppSettings["DbUserName"];
        //        string DatabasePassword = ConfigurationManager.AppSettings["DbPassword"];
        //        string ReportServerUrl = ConfigurationManager.AppSettings["ReportServerURL"];
        //        string ReportsUrl = ConfigurationManager.AppSettings["ReportsUrl"];
        //        objReportServices = new ReportServices(rs, Domain, Username, Password, DatabaseUsername, DatabasePassword);
        //        if (FoldarName != "")
        //        {
        //            ReportPath = @"/" + "Master" + FoldarName; // in place of Master need to use company wise folder name in future
        //        }
        //        else
        //            ReportPath = @"/" + FoldarName;

        //        // use company wise folder name in future along with Foldar Name in below code as well
        //        if (FoldarName.Contains("Transaction"))
        //            FoldarName = "Transaction";
        //        else
        //            FoldarName = "Supporting Information";

        //        objReportServices.CopyReportInSameFolder(FoldarName, SourceReportName, DestinationReportName, ReportPath);
        //    }
        //    catch
        //    {
        //        return Json(new { Message = "Fail" }, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(new { Message = "Ok" }, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult LoginSessionAlive()
        {
            return Json(new { Message = "Active" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ClickFromMenuNotification(bool IsTrue)
        {
            Session["MainFilter"] = IsTrue;
            return Json(new { Message = "Success", Status = "OK" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetMainFilterSessionValue()
        {

            return Json(new { Message = "Success", Status = "OK", value = Convert.ToString(Session["MainFilter"]).ToLower() }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SetPollAllTrue()
        {
            var isSensorBinsRFIDeTags = SessionHelper.GetAdminPermission(SessionHelper.ModuleList.SensorBinsRFIDeTags);

            if (isSensorBinsRFIDeTags)
            {
                try
                {

                    bool IsValidatePollBetweenTime = true;
                    eVMISetupDAL objeVMISetupDAL = new eVMISetupDAL(SessionHelper.EnterPriseDBName);
                    eVMISetupDTO objeVMISetupDTO = objeVMISetupDAL.GetRecord(SessionHelper.RoomID, SessionHelper.CompanyID);
                    bool _IsOldeVMIRoom = Helper.CommonUtility.IsOldeVMIRoom();

                    if (_IsOldeVMIRoom == false)
                    {
                        ItemLocationPollRequestDAL objILPollRequestDAL = new ItemLocationPollRequestDAL(SessionHelper.EnterPriseDBName);
                        ItemLocationPollRequestDTO objILPRequestDTO = objILPollRequestDAL.GetItemLocationPollRequestForBetween(SessionHelper.CompanyID, SessionHelper.RoomID);

                        //if (objeVMISetupDTO != null && objILPRequestDTO != null && (objeVMISetupDTO.PollAllBetweenTime ?? 0) > 0)
                        //{
                        //    TimeSpan TimeDiff = (DateTime.UtcNow - objILPRequestDTO.Created);
                        //    if (TimeDiff.TotalMinutes < objeVMISetupDTO.PollAllBetweenTime)
                        //        IsValidatePollBetweenTime = false;
                        //}
                        long RemainingPolls = objILPollRequestDAL.IsPollAllQueueCleared(SessionHelper.CompanyID, SessionHelper.RoomID);
                        if (RemainingPolls > 0)
                        {
                            IsValidatePollBetweenTime = false;
                        }
                        else
                        {
                            IsValidatePollBetweenTime = true;
                        }

                        if (IsValidatePollBetweenTime == false)
                        {
                            string strValidatePollBetween = ReseVMISetup.msgRemainingItems + " " + RemainingPolls;
                            return Json(new { Message = strValidatePollBetween, Status = "fail" }, JsonRequestBehavior.AllowGet);
                        }

                        // New eVMI USR
                        if (SessionHelper.isEVMI == true)
                        {
                            ItemLocationPollRequestDTO objILPollRequestDTO = new ItemLocationPollRequestDTO();
                            objILPollRequestDTO.RoomID = SessionHelper.RoomID;
                            objILPollRequestDTO.CompanyID = SessionHelper.CompanyID;
                            objILPollRequestDTO.RequestType = (int)PollRequestType.PollAll;
                            objILPollRequestDTO.IsPollStarted = false;
                            objILPollRequestDTO.CreatedBy = SessionHelper.UserID;
                            InventoryCountDTO objInventoryCountDTO = new InventoryCountDAL(SessionHelper.EnterPriseDBName).InsertPollCount(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "web");
                            if (objInventoryCountDTO != null && objInventoryCountDTO.ID > 0)
                            {
                                objILPollRequestDTO.CountGUID = objInventoryCountDTO.GUID;
                            }
                            objILPollRequestDAL.InsertItemLocationPollAllRequest(objILPollRequestDTO);
                        }

                    }
                    else if (_IsOldeVMIRoom)
                    {
                        PollAllFlagMasterDAL objDAL = new PollAllFlagMasterDAL(SessionHelper.EnterPriseDBName);
                        IEnumerable<PollAllFlagMasterDTO> lstDTO = objDAL.GetAllRecords(SessionHelper.CompanyID, SessionHelper.RoomID);

                        if (lstDTO != null && lstDTO.Count() > 0)
                        {
                            PollAllFlagMasterDTO objDTO = lstDTO.FirstOrDefault();
                            objDTO.LastUpdatedBy = SessionHelper.UserID;
                            objDTO.IsPollALL = true;
                            objDAL.Edit(objDTO);
                        }
                        else
                        {
                            PollAllFlagMasterDTO objDTO = new PollAllFlagMasterDTO()
                            {
                                CompanyID = SessionHelper.CompanyID,
                                IsPollALL = true,
                                LastUpdatedBy = SessionHelper.UserID,
                                CreatedBy = SessionHelper.UserID,
                                RoomID = SessionHelper.RoomID,
                            };

                            objDAL.Insert(objDTO);

                        }

                    }


                    return Json(new { Message = "Success", Status = "ok", value = Convert.ToString(Session["MainFilter"]).ToLower() }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { Message = ex.Message, Status = "fail" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Status = "fail", Message = ResCommon.MsgNoPermission }, JsonRequestBehavior.AllowGet);
            }
        }

        [NonAction]
        public List<RoomDTO> GetRoomDataForUserBasedOnCompany(long EnterpriseId, long CompanyId, long RoomId, long UserId, int UserType, long RoleId, string EventFiredOn, string enterpriseName, string CompanyName, string RoomName)
        {
            eTurnsMaster.DAL.EnterpriseMasterDAL objEnterpriseDAL = new eTurnsMaster.DAL.EnterpriseMasterDAL();
            CompanyMasterDAL objCompanyMasterDAL = new CompanyMasterDAL(SessionHelper.EnterPriseDBName);
            RoomDAL objRoomDAL = new RoomDAL(SessionHelper.EnterPriseDBName);
            eTurnsMaster.DAL.UserMasterDAL objUserMasterDAL = new eTurnsMaster.DAL.UserMasterDAL();
            eTurns.DAL.UserMasterDAL objinterUserDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
            EnterpriseDTO objEnterpriseDTO = new EnterpriseDTO();
            List<RoomDTO> lstRooms = null;

            if (RoleId == -1) // eturns super admin
            {
                CompanyMasterDTO objCompanyMaster = objCompanyMasterDAL.GetCompanyByID(CompanyId);
                lstRooms = objRoomDAL.GetRoomByCompanyIDPlain(SessionHelper.CompanyID).OrderByDescending(x => x.IsRoomActive).ThenBy(r => r.RoomName).ToList();

            }
            else if (RoleId == -2) // enterprice  super admin
            {
                CompanyMasterDTO objCompanyMaster = objCompanyMasterDAL.GetCompanyByID(CompanyId);
                lstRooms = objRoomDAL.GetRoomByCompanyIDPlain(SessionHelper.CompanyID).OrderByDescending(x => x.IsRoomActive).ThenBy(r => r.RoomName).ToList();
            }
            else
            {
                CompanyMasterDTO objCompanyMaster = objCompanyMasterDAL.GetCompanyByID(CompanyId);
                lstRooms = SessionHelper.RoomList; // super admin // enterprice admin only
            }
            return lstRooms;
        }

        #region Get All Records for Autocomplete

        /// <summary>
        /// GetAllCustomerforAutoComplete
        /// </summary>
        /// <param name="NameStartWith"></param>
        /// <returns></returns>
        public JsonResult GetAllCustomers(string NameStartWith)
        {
            CustomerMasterDAL objDAL = new CustomerMasterDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<CustomerMasterDTO> dtoList = objDAL.GetCustomersByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(t => t.Customer);
            if (dtoList != null && dtoList.Count() > 0)
            {
                if (!string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                {
                    dtoList = dtoList.Where(x => x.Customer.ToLower().StartsWith(NameStartWith.ToLower())).OrderBy(t => t.Customer);
                    return Json(dtoList, JsonRequestBehavior.AllowGet);
                }
                else if (NameStartWith.Contains(" "))
                {
                    return Json(dtoList, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(dtoList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GetAllCustomerforAutoComplete
        /// </summary>
        /// <param name="NameStartWith"></param>
        /// <returns></returns>
        public JsonResult GetAllShipVia(string NameStartWith)
        {
            ShipViaDAL objDAL = new ShipViaDAL(SessionHelper.EnterPriseDBName);
            List<ShipViaDTO> dtoList = new List<ShipViaDTO>();
            dtoList = objDAL.GetShipViaListSearch(SessionHelper.RoomID, SessionHelper.CompanyID, NameStartWith).OrderBy(t => t.ShipVia).ToList();
            //if (dtoList != null && dtoList.Count() > 0)
            //{
            //    if (!string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
            //    {
            //        dtoList = dtoList.Where(x => x.ShipVia.ToLower().StartsWith(NameStartWith.ToLower())).OrderBy(t => t.ShipVia);
            //        return Json(dtoList, JsonRequestBehavior.AllowGet);
            //    }
            //    else if (NameStartWith.Contains(" "))
            //    {
            //        return Json(dtoList, JsonRequestBehavior.AllowGet);
            //    }
            //}
            return Json(dtoList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GetAllCustomerforAutoComplete
        /// </summary>
        /// <param name="NameStartWith"></param>
        /// <returns></returns>
        public JsonResult GetAllVendors(string NameStartWith)
        {
            VenderMasterDAL objDAL = new VenderMasterDAL(SessionHelper.EnterPriseDBName);
            IEnumerable<VenderMasterDTO> dtoList = objDAL.GetVenderListSearch(SessionHelper.RoomID, SessionHelper.CompanyID, NameStartWith).OrderBy(t => t.Vender);
            //if (dtoList != null && dtoList.Count() > 0)
            //{
            //    if (!string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
            //    {
            //        dtoList = dtoList.Where(x => x.Vender.ToLower().StartsWith(NameStartWith.ToLower())).OrderBy(t => t.Vender);
            //        return Json(dtoList, JsonRequestBehavior.AllowGet);
            //    }
            //    else if (NameStartWith.Contains(" "))
            //    {
            //        return Json(dtoList, JsonRequestBehavior.AllowGet);
            //    }
            //}
            return Json(dtoList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GetAllCustomerforAutoComplete
        /// </summary>
        /// <param name="NameStartWith"></param>
        /// <returns></returns>
        public JsonResult GetBins(string StagingName, string NameStartWith)
        {
            List<string> dtoList = new List<string>();
            if (!string.IsNullOrEmpty(StagingName) && !string.IsNullOrWhiteSpace(StagingName))
            {
                MaterialStagingDAL objMSDAL = new MaterialStagingDAL(SessionHelper.EnterPriseDBName);
                //MaterialStagingDTO MsDTO = objMSDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(x => x.StagingName == StagingName).FirstOrDefault();
                MaterialStagingDTO MsDTO = objMSDAL.GetMaterialStaging(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, StagingName, null).FirstOrDefault();
                if (MsDTO != null)
                {
                    if (MsDTO.BinID.GetValueOrDefault(0) <= 0)
                    {
                        MaterialStagingDetailDAL objMSDetailDAL = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName);
                        //IEnumerable<MaterialStagingDetailDTO> MsDetailDTOList = objMSDetailDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.MaterialStagingGUID == MsDTO.GUID && !x.IsArchived && !x.IsDeleted).OrderBy(x => x.StagingBinName);
                        IEnumerable<MaterialStagingDetailDTO> MsDetailDTOList = objMSDetailDAL.GetMaterialStagingDetailbyItemGUIDStagingBINID(Convert.ToString(MsDTO.GUID), string.Empty, null, SessionHelper.RoomID, SessionHelper.CompanyID, false, false).OrderBy(x => x.StagingBinName);
                        dtoList = MsDetailDTOList.Select(x => x.StagingBinName).Distinct().ToList<string>();
                        if (!string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                        {
                            dtoList = dtoList.Where(x => x.ToLower().StartsWith(NameStartWith.ToLower())).ToList();
                        }
                    }
                    else
                    {
                        BinMasterDTO objBinDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(MsDTO.BinID.GetValueOrDefault(0), SessionHelper.RoomID, SessionHelper.CompanyID);
                        //BinMasterDTO objBinDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocation( SessionHelper.RoomID, SessionHelper.CompanyID, false, false,Guid.Empty, MsDTO.BinID.GetValueOrDefault(0),null,null).FirstOrDefault();
                        dtoList.Add(objBinDTO.BinNumber);
                    }
                }
            }
            else
            {
                //IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(x => !x.IsStagingLocation).OrderBy(x => x.BinNumber);
                IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Guid.Empty, 0, null, false).OrderBy(x => x.BinNumber);//.Where(x => !x.IsStagingLocation)
                if (objBinDTOList != null && objBinDTOList.Count() > 0)
                {
                    dtoList = objBinDTOList.Select(x => x.BinNumber).ToList();
                    if (!string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                    {
                        dtoList = dtoList.Where(x => x.ToLower().StartsWith(NameStartWith.ToLower())).ToList();
                    }
                }
            }
            return Json(dtoList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// GetAllCustomerforAutoComplete
        /// </summary>
        /// <param name="NameStartWith"></param>
        /// <returns></returns>
        public JsonResult GetBinsOfItem(string StagingName, string NameStartWith, string ItemGUID, bool QtyRequired = false, bool OnlyHaveQty = false, bool? IsLoadMoreLocations = null)
        {
            List<string> dtoList = new List<string>();
            List<DTOForAutoComplete> locations = new List<DTOForAutoComplete>();

            if (!string.IsNullOrEmpty(StagingName) && !string.IsNullOrWhiteSpace(StagingName))
            {
                if (StagingName == "GetAllStagLocationsOfItems")
                {
                    //IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByItemLocationLevelQuanity(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGUID).Where(x => x.IsStagingLocation).OrderBy(x => x.BinNumber);
                    IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByItemLocationLevelQuanity(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGUID, true, string.Empty, null, null).OrderBy(x => x.BinNumber);
                    //IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Guid.Parse(ItemGUID),0,null,true).OrderBy(x => x.BinNumber);//.Where(x => x.IsStagingLocation)
                    if (objBinDTOList != null && objBinDTOList.Count() > 0)
                    {
                        dtoList = objBinDTOList.Select(x => x.BinNumber).ToList();
                        if (!string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                        {
                            dtoList = dtoList.Where(x => x.ToLower().StartsWith(NameStartWith.ToLower())).ToList();
                        }

                        foreach (string item in dtoList)
                        {
                            DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                            objAutoDTO.Key = item;
                            objAutoDTO.Value = item;
                            locations.Add(objAutoDTO);
                        }

                    }
                }
                else
                {
                    MaterialStagingDAL objMSDAL = new MaterialStagingDAL(SessionHelper.EnterPriseDBName);
                    //MaterialStagingDTO MsDTO = objMSDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(x => x.StagingName == StagingName).FirstOrDefault();
                    MaterialStagingDTO MsDTO = objMSDAL.GetMaterialStaging(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, StagingName, null).FirstOrDefault();
                    if (MsDTO != null)
                    {
                        if (MsDTO.BinID.GetValueOrDefault(0) <= 0)
                        {
                            MaterialStagingDetailDAL objMSDetailDAL = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName);
                            //IEnumerable<MaterialStagingDetailDTO> MsDetailDTOList = objMSDetailDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.MaterialStagingGUID == MsDTO.GUID && !x.IsArchived && !x.IsDeleted).OrderBy(x => x.StagingBinName);
                            IEnumerable<MaterialStagingDetailDTO> MsDetailDTOList = objMSDetailDAL.GetMaterialStagingDetailbyItemGUIDStagingBINID(Convert.ToString(MsDTO.GUID), string.Empty, null, SessionHelper.RoomID, SessionHelper.CompanyID, false, false).OrderBy(x => x.StagingBinName);
                            dtoList = MsDetailDTOList.Select(x => x.StagingBinName).Distinct().ToList<string>();
                            if (!string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                            {
                                dtoList = dtoList.Where(x => x.ToLower().StartsWith(NameStartWith.ToLower())).ToList();
                            }
                        }
                        else
                        {
                            BinMasterDTO objBinDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetBinByID(MsDTO.BinID.GetValueOrDefault(0), SessionHelper.RoomID, SessionHelper.CompanyID);
                            //BinMasterDTO objBinDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocation( SessionHelper.RoomID, SessionHelper.CompanyID, false, false,Guid.Empty, MsDTO.BinID.GetValueOrDefault(0),null,null).FirstOrDefault();
                            dtoList.Add(objBinDTO.BinNumber);
                        }

                        foreach (string item in dtoList)
                        {
                            DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                            objAutoDTO.Key = item;
                            objAutoDTO.Value = item;
                            locations.Add(objAutoDTO);
                        }
                    }
                }
            }
            else
            {
                if (QtyRequired == false)
                {
                    //IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByItemLocationLevelQuanity(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGUID).Where(x => !x.IsStagingLocation).OrderBy(x => x.BinNumber);
                    IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByItemLocationLevelQuanity(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGUID, false, string.Empty, null, null).OrderBy(x => x.BinNumber);
                    //IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Guid.Parse(ItemGUID),0,null,false).OrderBy(x => x.BinNumber);//.Where(x => !x.IsStagingLocation)
                    if (objBinDTOList != null && objBinDTOList.Count() > 0)
                    {
                        dtoList = objBinDTOList.Where(x => !string.IsNullOrWhiteSpace(x.BinNumber)).Select(x => x.BinNumber).ToList();
                        if (!string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                        {
                            dtoList = dtoList.Where(x => x.ToLower().StartsWith(NameStartWith.ToLower())).ToList();
                        }
                        foreach (string item in dtoList)
                        {
                            DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                            objAutoDTO.Key = item;
                            objAutoDTO.Value = item;
                            locations.Add(objAutoDTO);
                        }
                    }
                }
                else if (QtyRequired == true)
                {
                    ItemLocationQTYDAL objAPI = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName);
                    List<ItemLocationQTYDTO> objList = new List<ItemLocationQTYDTO>();
                    objList = objAPI.GetBinsByItem(new Guid(ItemGUID), SessionHelper.RoomID, SessionHelper.CompanyID, "BinNumber");
                    Guid ItemGUID1 = Guid.Empty;
                    ItemMasterDTO objItem = null;
                    if (Guid.TryParse(ItemGUID, out ItemGUID1))
                    {
                        objItem = new ItemMasterDAL(SessionHelper.EnterPriseDBName).GetItemWithoutJoins(null, ItemGUID1);
                    }


                    //string Qty = "";
                    if (objList != null && objList.Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                        {
                            objList = objList.Where(x => x.BinNumber.ToLower().StartsWith(NameStartWith.ToLower())).ToList();
                        }

                        //for (int t = 0; t < objList.Count; t++)
                        //{
                        //    Qty = "";
                        //    Qty = objList[t].BinNumber + "(" + objList[t].Quantity.ToString() + ")";
                        //    dtoList.Add(Qty);   //objBinDTOList.Select(x => x.BinNumber).ToList();
                        //}

                        foreach (var item in objList)
                        {
                            DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                            if (objItem.Consignment)
                            {
                                objAutoDTO.Key = item.BinNumber + "(" + item.Quantity.ToString() + ")";
                            }
                            else
                            {
                                objAutoDTO.Key = item.BinNumber + "(" + item.CustomerOwnedQuantity.ToString() + ")";
                            }
                            objAutoDTO.Value = item.BinNumber;
                            objAutoDTO.ID = item.BinID;
                            if (objItem.DefaultLocation.GetValueOrDefault(0) == item.ID)
                            {
                                objAutoDTO.OtherInfo1 = "DefaultLocation";
                            }
                            if (OnlyHaveQty)
                            {
                                if (item.Quantity > 0)
                                    locations.Add(objAutoDTO);
                            }
                            else
                                locations.Add(objAutoDTO);
                        }
                    }
                }
            }

            if (IsLoadMoreLocations.HasValue && !QtyRequired)
            {
                if (IsLoadMoreLocations.Value)
                {
                    locations = AddAllBinsInList(StagingName, NameStartWith, locations);
                }
                else
                {
                    DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                    objAutoDTO.Key = ResBin.MoreLocations;
                    objAutoDTO.Value = ResBin.MoreLocations;
                    locations.Add(objAutoDTO);
                }
            }

            return Json(locations, JsonRequestBehavior.AllowGet);
        }

        public List<DTOForAutoComplete> AddAllBinsInList(string StagingName, string NameStartWith, List<DTOForAutoComplete> locations)
        {
            List<string> dtoList = new List<string>();
            if (!string.IsNullOrEmpty(StagingName) && !string.IsNullOrWhiteSpace(StagingName))
            {
                if (StagingName == "GetAllStagLocationsOfItems")
                {
                    IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllBins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, NameStartWith.ToLower(), true).OrderBy(x => x.BinNumber);
                    if (objBinDTOList != null && objBinDTOList.Count() > 0)
                    {
                        dtoList = objBinDTOList.Select(x => x.BinNumber).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                        if (!string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                        {
                            dtoList = dtoList.Where(x => x.ToLower().StartsWith(NameStartWith.ToLower())).ToList();
                        }
                    }
                }
                else
                {
                    //MaterialStagingDAL objMSDAL = new MaterialStagingDAL(SessionHelper.EnterPriseDBName);
                    //MaterialStagingDTO MsDTO = objMSDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(x => x.StagingName == StagingName).FirstOrDefault();
                    //if (MsDTO != null)
                    //{
                    //    if (MsDTO.BinID.GetValueOrDefault(0) <= 0)
                    //    {
                    //        MaterialStagingDetailDAL objMSDetailDAL = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName);
                    //        IEnumerable<MaterialStagingDetailDTO> MsDetailDTOList = objMSDetailDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => x.MaterialStagingGUID == MsDTO.GUID && !x.IsArchived && !x.IsDeleted).OrderBy(x => x.StagingBinName);
                    //        dtoList = MsDetailDTOList.Select(x => x.StagingBinName).Distinct().ToList<string>();
                    //        if (!string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                    //        {
                    //            dtoList = dtoList.Where(x => x.ToLower().StartsWith(NameStartWith.ToLower())).ToList();
                    //        }
                    //    }
                    //    else
                    //    {
                    //        BinMasterDTO objBinDTO = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetRecord(MsDTO.BinID.GetValueOrDefault(0), SessionHelper.RoomID, SessionHelper.CompanyID, false, false);
                    //        dtoList.Add(objBinDTO.BinNumber);
                    //    }
                    //}
                }
            }
            else
            {
                IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllBins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, NameStartWith.ToLower(), false).OrderBy(x => x.BinNumber);
                if (objBinDTOList != null && objBinDTOList.Count() > 0)
                {
                    dtoList = objBinDTOList.Select(x => x.BinNumber).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                    if (!string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                    {
                        dtoList = dtoList.Where(x => x.ToLower().StartsWith(NameStartWith.ToLower())).ToList();
                    }
                }
            }

            foreach (string item in dtoList)
            {
                if (!locations.Any(x => x.Key == item))
                {
                    if (!item.Equals("[|EmptyStagingBin|]"))
                    {
                        DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                        objAutoDTO.Key = item;
                        objAutoDTO.Value = item;
                        locations.Add(objAutoDTO);
                    }
                }
            }

            return locations;
        }

        /// <summary>
        /// GetAllCustomerforAutoComplete
        /// </summary>
        /// <param name="NameStartWith"></param>
        /// <returns></returns>
        public JsonResult GetBinsOfItemByOrderId(string StagingName, string NameStartWith, string ItemGUID, bool QtyRequired = false, bool OnlyHaveQty = false, Int64 OrderId = 0, bool? IsLoadMoreLocations = false, bool? isForRequisition = false, long? stagingBinId = null)
        {
            List<string> dtoList = new List<string>();
            ItemMasterDAL ItemDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            List<DTOForAutoComplete> locations = new List<DTOForAutoComplete>();
            if (QtyRequired == true && !IsLoadMoreLocations.GetValueOrDefault(false))
            {

                IEnumerable<BinMasterDTO> objBinDTOList;// = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByItemLocationLevelQuanity(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGUID).Where(x => !x.IsStagingLocation).OrderBy(x => x.BinNumber);
                if (isForRequisition.HasValue && isForRequisition.Value && stagingBinId != null && stagingBinId.Value > 0)
                {
                    Guid itemGuid = Guid.Empty;
                    Guid.TryParse(ItemGUID, out itemGuid);
                    objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetStagingBinMasterByRoomID(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(x => x.BinNumber);
                }
                else
                {
                    //objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByItemLocationLevelQuanity(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGUID).Where(x => !x.IsStagingLocation).OrderBy(x => x.BinNumber);
                    objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByItemLocationLevelQuanity(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGUID, false, string.Empty, null, null).OrderBy(x => x.BinNumber);
                }
                //IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Guid.Parse(ItemGUID),0,null,false).OrderBy(x => x.BinNumber);//.Where(x => !x.IsStagingLocation)
                if (objBinDTOList != null && objBinDTOList.Count() > 0)
                {
                    dtoList = objBinDTOList.Select(x => x.BinNumber).ToList();
                    if (!string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                    {
                        objBinDTOList = objBinDTOList.Where(x => x.BinNumber.ToLower().StartsWith(NameStartWith.ToLower())).ToList();
                    }
                    foreach (var item in objBinDTOList)
                    {
                        if (isForRequisition.HasValue && isForRequisition.Value && stagingBinId != null && stagingBinId.Value > 0)
                        {
                            DTOForAutoComplete objAutoDTO = new DTOForAutoComplete()
                            {
                                Key = item.BinNumber,
                                Value = item.BinNumber,
                                ID = item.ID,
                                GUID = item.GUID,
                            };
                            locations.Add(objAutoDTO);
                            locations.Add(new DTOForAutoComplete() { GUID = item.GUID });
                        }
                        else
                        {
                            ItemLocationQTYDAL objLocationQtyDAL = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName);
                            ItemLocationQTYDTO objLocatQtyDTO = objLocationQtyDAL.GetRecordByBinItem(Guid.Parse(ItemGUID), item.ID, SessionHelper.RoomID, SessionHelper.CompanyID);
                            if (objLocatQtyDTO != null && objLocatQtyDTO.Quantity > 0)
                            {
                                ItemMasterDTO IMDTO = ItemDAL.GetItemByGuidNormal(item.ItemGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                                DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                                objAutoDTO.Key = item.BinNumber + " (" + objLocatQtyDTO.Quantity.ToString("N" + SessionHelper.NumberDecimalDigits) + ")";

                                objAutoDTO.Value = (isForRequisition.HasValue && isForRequisition.Value)
                                        ? objAutoDTO.Value = item.BinNumber
                                        : item.BinNumber + " (" + objLocatQtyDTO.Quantity.ToString("N" + SessionHelper.NumberDecimalDigits) + ")";
                                objAutoDTO.ID = item.ID;
                                objAutoDTO.OtherInfo1 = item.DefaultPullQuantity.GetValueOrDefault(0) > 0 ? Convert.ToString(item.DefaultPullQuantity.GetValueOrDefault(0)) : "";
                                objAutoDTO.OtherInfo2 = item.DefaultReorderQuantity.GetValueOrDefault(0) > 0 ? Convert.ToString(item.DefaultReorderQuantity.GetValueOrDefault(0)) : "";

                                if (!string.IsNullOrEmpty(objAutoDTO.OtherInfo2) && Convert.ToInt64(objAutoDTO.OtherInfo2) > 0 && item.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true && IMDTO != null && IMDTO.IsAllowOrderCostuom)
                                {
                                    objAutoDTO.OtherInfo2 = Convert.ToString(item.DefaultReorderQuantity.GetValueOrDefault(0) / IMDTO.OrderUOMValue);
                                    item.DefaultReorderQuantity = item.DefaultReorderQuantity.GetValueOrDefault(0) / IMDTO.OrderUOMValue;
                                }

                                locations.Add(objAutoDTO);
                            }
                            else
                            {

                                DTOForAutoComplete objAutoDTO = new DTOForAutoComplete()
                                {
                                    Key = item.BinNumber + " (0)",
                                    Value = (isForRequisition.HasValue && isForRequisition.Value) ? item.BinNumber : item.BinNumber + " (0)",
                                    ID = item.ID,
                                    GUID = item.GUID,
                                };
                                locations.Add(objAutoDTO);
                            }
                            locations.Add(new DTOForAutoComplete() { GUID = item.GUID });
                        }
                    }
                    /* //Comment for WI-4196
                    //List<OrderDetailsDTO> lstDetails = (List<OrderDetailsDTO>)SessionHelper.Get("OrderLineItem_" + SessionHelper.EnterPriceID + "_" + SessionHelper.CompanyID + "_" + SessionHelper.RoomID);
                    //if (lstDetails != null && lstDetails.Count > 0)
                    //{
                    //    foreach (OrderDetailsDTO o in lstDetails)
                    //    {
                    //        if (o.ItemGUID.GetValueOrDefault(Guid.Empty).ToString() == ItemGUID)
                    //            locations.RemoveAll(a => a.ID == o.Bin);
                    //    }
                    //}
                    */
                }
            }
            else if (QtyRequired == false)
            {
                //IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByItemLocationLevelQuanity(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGUID).Where(x => !x.IsStagingLocation).OrderBy(x => x.BinNumber);
                IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByItemLocationLevelQuanity(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGUID, false, string.Empty, null, null).OrderBy(x => x.BinNumber);
                //IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Guid.Parse(ItemGUID),0,null,false).OrderBy(x => x.BinNumber);//.Where(x => !x.IsStagingLocation)
                if (objBinDTOList != null && objBinDTOList.Count() > 0)
                {
                    dtoList = objBinDTOList.Select(x => x.BinNumber).ToList();
                    if (!string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                    {
                        objBinDTOList = objBinDTOList.Where(x => x.BinNumber.ToLower().StartsWith(NameStartWith.ToLower())).ToList();
                    }
                    foreach (var item in objBinDTOList)
                    {
                        ItemMasterDTO IMDTO = ItemDAL.GetItemByGuidNormal(item.ItemGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID);
                        DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                        objAutoDTO.Key = item.BinNumber;
                        objAutoDTO.Value = item.BinNumber;
                        objAutoDTO.ID = item.ID;
                        objAutoDTO.ItemGuid = item.ItemGUID;
                        objAutoDTO.OtherInfo1 = item.DefaultPullQuantity.GetValueOrDefault(0) > 0 ? Convert.ToString(item.DefaultPullQuantity.GetValueOrDefault(0)) : "";
                        objAutoDTO.OtherInfo2 = item.DefaultReorderQuantity.GetValueOrDefault(0) > 0 ? Convert.ToString(item.DefaultReorderQuantity.GetValueOrDefault(0)) : "";
                        if (!string.IsNullOrEmpty(objAutoDTO.OtherInfo2) && Convert.ToInt64(objAutoDTO.OtherInfo2) > 0 && item.IsEnforceDefaultReorderQuantity.GetValueOrDefault(false) == true && IMDTO != null && IMDTO.IsAllowOrderCostuom)
                        {
                            objAutoDTO.OtherInfo2 = Convert.ToString(item.DefaultReorderQuantity.GetValueOrDefault(0) / IMDTO.OrderUOMValue);
                            item.DefaultReorderQuantity = item.DefaultReorderQuantity.GetValueOrDefault(0) / IMDTO.OrderUOMValue;
                        }
                        locations.Add(objAutoDTO);
                    }

                    /* //Comment for WI-4196
                  //List<OrderDetailsDTO> lstDetails = (List<OrderDetailsDTO>)SessionHelper.Get("OrderLineItem_" + SessionHelper.EnterPriceID + "_" + SessionHelper.CompanyID + "_" + SessionHelper.RoomID);
                  //if (lstDetails != null && lstDetails.Count > 0)
                  //{
                  //    foreach (OrderDetailsDTO o in lstDetails)
                  //    {
                  //        if (o.ItemGUID.GetValueOrDefault(Guid.Empty).ToString() == ItemGUID)
                  //            locations.RemoveAll(a => a.ID == o.Bin);

                  //        if (locations.Where(x => x.Key == o.BinName && x.ItemGuid.GetValueOrDefault(Guid.Empty) == o.ItemGUID).Count() > 0)
                  //            locations.RemoveAll(x => x.Key == o.BinName && x.ItemGuid.GetValueOrDefault(Guid.Empty) == o.ItemGUID);
                  //    }
                  //}
                  */
                }
            }
            if (IsLoadMoreLocations.HasValue && !QtyRequired)
            {
                if (IsLoadMoreLocations.Value)
                {
                    locations = new List<DTOForAutoComplete>();
                    //List<DTOForAutoComplete> Alllocations = AddAllBinsInList(StagingName, NameStartWith, locations);
                    IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllBins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, NameStartWith.ToLower(), false).OrderBy(x => x.BinNumber);

                    if (objBinDTOList != null && objBinDTOList.Count() > 0)
                    {
                        foreach (var item in objBinDTOList)
                        {
                            DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                            objAutoDTO.Key = item.BinNumber;
                            objAutoDTO.Value = item.BinNumber;
                            objAutoDTO.ID = item.ID;
                            objAutoDTO.ItemGuid = item.ItemGUID;
                            locations.Add(objAutoDTO);
                        }
                    }

                    /*  //Comment for WI-4196
                    //List<OrderDetailsDTO> lstDetails = (List<OrderDetailsDTO>)SessionHelper.Get("OrderLineItem_" + SessionHelper.EnterPriceID + "_" + SessionHelper.CompanyID + "_" + SessionHelper.RoomID);
                    //if (lstDetails != null && lstDetails.Count > 0)
                    //{
                    //    foreach (OrderDetailsDTO o in lstDetails)
                    //    {
                    //        if (o.ItemGUID.GetValueOrDefault(Guid.Empty).ToString() == ItemGUID)
                    //            locations.RemoveAll(a => a.ID == o.Bin);

                    //        if (locations.Where(x => x.Key == o.BinName && x.ItemGuid.GetValueOrDefault(Guid.Empty) == o.ItemGUID).Count() > 0)
                    //            locations.RemoveAll(x => x.Key == o.BinName && x.ItemGuid.GetValueOrDefault(Guid.Empty) == o.ItemGUID);
                    //    }
                    //}
                    */
                }
                else
                {
                    DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                    objAutoDTO.Key = ResBin.MoreLocations;
                    objAutoDTO.Value = ResBin.MoreLocations;
                    locations.Add(objAutoDTO);
                }
            }

            return Json(locations, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLocationsOfToolByOrderId(string NameStartWith, string ToolGUID, bool QtyRequired = false, bool OnlyHaveQty = false, Int64 OrderId = 0, bool? IsLoadMoreLocations = false, bool? isForRequisition = false)
        {
            List<string> dtoList = new List<string>();
            List<DTOForAutoComplete> locations = new List<DTOForAutoComplete>();
            if (QtyRequired == true && !IsLoadMoreLocations.GetValueOrDefault(false))
            {
                IEnumerable<LocationMasterDTO> objBinDTOList = new LocationMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByToolLocationLevelQuanity(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ToolGUID).OrderBy(x => x.Location);
                //IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Guid.Parse(ItemGUID),0,null,false).OrderBy(x => x.BinNumber);//.Where(x => !x.IsStagingLocation)
                if (objBinDTOList != null && objBinDTOList.Count() > 0)
                {
                    dtoList = objBinDTOList.Select(x => x.Location).ToList();
                    if (!string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                    {
                        objBinDTOList = objBinDTOList.Where(x => x.Location.ToLower().StartsWith(NameStartWith.ToLower())).ToList();
                    }
                    foreach (var item in objBinDTOList)
                    {
                        ItemLocationQTYDAL objLocationQtyDAL = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName);
                        ItemLocationQTYDTO objLocatQtyDTO = objLocationQtyDAL.GetRecordByBinItem(Guid.Parse(ToolGUID), item.ID, SessionHelper.RoomID, SessionHelper.CompanyID);
                        if (objLocatQtyDTO != null && objLocatQtyDTO.Quantity > 0)
                        {
                            DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                            objAutoDTO.Key = item.Location + " (" + objLocatQtyDTO.Quantity.ToString("N" + SessionHelper.NumberDecimalDigits) + ")";
                            objAutoDTO.Value = item.Location + " (" + objLocatQtyDTO.Quantity.ToString("N" + SessionHelper.NumberDecimalDigits) + ")";
                            objAutoDTO.ID = item.ID;

                            locations.Add(objAutoDTO);
                        }
                        else
                        {
                            DTOForAutoComplete objAutoDTO = new DTOForAutoComplete()
                            {
                                Key = item.Location + " (0)",
                                Value = item.Location + " (0)",
                                ID = item.ID,
                                GUID = item.GUID,
                            };
                            locations.Add(objAutoDTO);
                        }
                        locations.Add(new DTOForAutoComplete() { GUID = item.GUID });
                    }

                }
            }
            else if (QtyRequired == false)
            {
                IEnumerable<LocationMasterDTO> objBinDTOList = new LocationMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByToolLocationLevelQuanity(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ToolGUID).OrderBy(x => x.Location);
                //IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Guid.Parse(ItemGUID),0,null,false).OrderBy(x => x.BinNumber);//.Where(x => !x.IsStagingLocation)
                if (objBinDTOList != null && objBinDTOList.Count() > 0)
                {
                    dtoList = objBinDTOList.Select(x => x.Location).ToList();
                    if (!string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                    {
                        objBinDTOList = objBinDTOList.Where(x => x.Location.ToLower().StartsWith(NameStartWith.ToLower())).ToList();
                    }
                    foreach (var item in objBinDTOList)
                    {
                        DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                        objAutoDTO.Key = item.Location;
                        objAutoDTO.Value = item.Location;
                        objAutoDTO.ID = item.ID;
                        objAutoDTO.ItemGuid = item.ToolGUID;
                        locations.Add(objAutoDTO);
                    }


                }
            }
            if (IsLoadMoreLocations.HasValue && !QtyRequired)
            {
                if (IsLoadMoreLocations.Value)
                {
                    locations = new List<DTOForAutoComplete>();
                    //List<DTOForAutoComplete> Alllocations = AddAllBinsInList(StagingName, NameStartWith, locations);
                    IEnumerable<LocationMasterDTO> objBinDTOList = new LocationMasterDAL(SessionHelper.EnterPriseDBName).GetLocationListSearch(SessionHelper.RoomID, SessionHelper.CompanyID, NameStartWith.ToLower()).OrderBy(x => x.Location);
                    if (objBinDTOList != null && objBinDTOList.Count() > 0)
                    {
                        foreach (var item in objBinDTOList)
                        {
                            DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                            objAutoDTO.Key = item.Location;
                            objAutoDTO.Value = item.Location;
                            objAutoDTO.ID = item.ID;
                            objAutoDTO.ItemGuid = item.ToolGUID;
                            locations.Add(objAutoDTO);
                        }
                    }


                }
                else
                {
                    DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                    objAutoDTO.Key = ResBin.MoreLocations;
                    objAutoDTO.Value = ResBin.MoreLocations;
                    locations.Add(objAutoDTO);
                }
            }

            if (isForRequisition.HasValue && isForRequisition.Value)
            {
                if (IsLoadMoreLocations.GetValueOrDefault(false))
                {
                    locations = new List<DTOForAutoComplete>();
                    IEnumerable<LocationMasterDTO> objBinDTOList = new LocationMasterDAL(SessionHelper.EnterPriseDBName).GetLocationListSearch(SessionHelper.RoomID, SessionHelper.CompanyID, NameStartWith.ToLower()).OrderBy(x => x.Location);
                    if (objBinDTOList != null && objBinDTOList.Count() > 0)
                    {
                        foreach (var item in objBinDTOList)
                        {
                            DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                            objAutoDTO.Key = item.Location;
                            objAutoDTO.Value = item.Location;
                            objAutoDTO.ID = item.ID;
                            objAutoDTO.ItemGuid = item.ToolGUID;
                            locations.Add(objAutoDTO);
                        }
                    }
                }
                else
                {
                    DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                    objAutoDTO.Key = ResBin.MoreLocations;
                    objAutoDTO.Value = ResBin.MoreLocations;
                    locations.Add(objAutoDTO);
                }
            }

            return Json(locations, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// GetAllCustomerforAutoComplete
        /// </summary>
        /// <param name="NameStartWith"></param>
        /// <returns></returns>
        public JsonResult GetItemLocationDetailForAutoComplete(Guid ItemGUID, string ItemSourceBin)
        {
            BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            List<CountLineItemDetailDTOForAutoComplete> locations = objBinMasterDAL.GetItemLocationDetailForAutoComplete(ItemGUID, ItemSourceBin, SessionHelper.CompanyID, SessionHelper.RoomID);
            var objReturn = (from I in locations
                             select new
                             {
                                 LotSerialNumber = (!string.IsNullOrWhiteSpace(I.LotSerialNumber)) ? I.LotSerialNumber.Trim() : string.Empty,
                                 ExpirationDate = I.ExpirationDate,
                                 AvailableQuantity = I.AvailableQuantity,
                                 LotSerialNumberWithoutDate = I.LotSerialNumberWithoutDate,
                             }).ToList();

            return Json(objReturn, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveTermcondition(string TermconditionText, bool IsLicenceAccepted)
        {
            string status = "";
            eTurns.DAL.UserMasterDAL objeTurns = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
            eTurnsMaster.DAL.UserMasterDAL objeTurnsMaster = new eTurnsMaster.DAL.UserMasterDAL();
            objeTurnsMaster.UpdateTermsCondition(SessionHelper.UserID, IsLicenceAccepted);

            objeTurns.UpdateTermsCondition(SessionHelper.UserID, IsLicenceAccepted);
            if (IsLicenceAccepted)
            {
                eTurnsMaster.DAL.UserLicenceDAL objUserLicenceAccept = new eTurnsMaster.DAL.UserLicenceDAL();
                objUserLicenceAccept.InsertLicenceAccept(SessionHelper.UserID);

                if (SessionHelper.IsFromSelectedDomain)
                {
                    objeTurnsMaster.UpdateUserSettingForBorderState(SessionHelper.UserID);
                }
            }
            eTurnsMaster.DAL.UserSettingDAL objUserSettingDAL = new eTurnsMaster.DAL.UserSettingDAL();
            eTurns.DTO.UserSettingDTO objUserSettingDTO = objUserSettingDAL.GetByUserId(SessionHelper.UserID);
            string URL = "../" + CtrlName + "/" + ActName;
            if (!SessionHelper.IsLicenceAccepted && !SessionHelper.IsFromSelectedDomain)
            {
                URL = "../Master/ChangePassword";
            }
            else
            {

                if (objUserSettingDTO != null && (!string.IsNullOrEmpty(objUserSettingDTO.RedirectURL)) && objUserSettingDTO.RedirectURL != null)
                {
                    URL = objUserSettingDTO.RedirectURL;
                }
            }
            SessionHelper.IsLicenceAccepted = IsLicenceAccepted;
            SessionHelper.NewEulaAccept = IsLicenceAccepted;
            SessionHelper.AnotherLicenceAccepted = IsLicenceAccepted;
            status = "Ok";


            return Json(new { Message = status, RedirectURL = URL });
        }
        public JsonResult CreateTermsAndConditionPDF(string TermsAndConditionText)
        {
            string path = string.Empty;
            string filename = "TermsAndCondition.pdf";
            path = Server.MapPath("~/Downloads/") + filename;
            HTMLToPdf(TermsAndConditionText, path);
            return Json(new { Message = filename });
        }
        public void HTMLToPdf(string HtmlStream, string FileName)
        {
            object TargetFile = FileName;
            string ModifiedFileName = string.Empty;
            string FinalFileName = string.Empty;

            /* To add a Password to PDF -http://aspnettutorialonline.blogspot.com/ */
            TestPDF.HtmlToPdfBuilder builder = new TestPDF.HtmlToPdfBuilder(iTextSharp.text.PageSize.A4);
            TestPDF.HtmlPdfPage first = builder.AddPage();
            first.AppendHtml(HtmlStream);
            byte[] file = builder.RenderPdf();
            System.IO.File.WriteAllBytes(TargetFile.ToString(), file);

            iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(TargetFile.ToString());
            ModifiedFileName = TargetFile.ToString();
            ModifiedFileName = ModifiedFileName.Insert(ModifiedFileName.Length - 4, "1");


            iTextSharp.text.pdf.PdfEncryptor.Encrypt(reader, new FileStream(ModifiedFileName, FileMode.Append), iTextSharp.text.pdf.PdfWriter.STRENGTH128BITS, null, "", iTextSharp.text.pdf.PdfWriter.AllowPrinting);

            reader.Close();
            if (System.IO.File.Exists(TargetFile.ToString()))
                System.IO.File.Delete(TargetFile.ToString());
            FinalFileName = ModifiedFileName.Remove(ModifiedFileName.Length - 5, 1);
            System.IO.File.Copy(ModifiedFileName, FinalFileName);
            if (System.IO.File.Exists(ModifiedFileName))
                System.IO.File.Delete(ModifiedFileName);
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult SaveChangePassword(ChangePasswordDTO objChangePasswordDTO)
        {
            string currentCulture = "en-US";
            if (System.Web.HttpContext.Current != null)
            {
                if (eTurns.DTO.Resources.ResourceHelper.CurrentCult != null)
                {
                    if (!string.IsNullOrWhiteSpace(Convert.ToString(eTurns.DTO.Resources.ResourceHelper.CurrentCult)))
                    {
                        currentCulture = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                    }

                }
            }
            else
            {
                eTurns.DTO.eTurnsRegionInfo objeTurnsRegionInfo = new RegionSettingDAL(SessionHelper.EnterPriseDBName).GetRegionSettingsById(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                currentCulture = objeTurnsRegionInfo.CultureName;
            }
            if (objChangePasswordDTO == null || !string.IsNullOrWhiteSpace(objChangePasswordDTO.FirstPassword))
            {
                string passwordMsg = string.Empty;
                bool isStrong = CommonUtilityHelper.ValidateStrongPassword(objChangePasswordDTO.FirstPassword, out passwordMsg, currentCulture, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);
                if (!isStrong || !string.IsNullOrEmpty(passwordMsg))
                {
                    return Json(new { Message = "strongpassword", StrongPassword = passwordMsg });
                }
            }

            string status = "";
            EnterPriseConfigDTO objDTO = new EnterPriseConfigDTO();
            EnterPriseConfigDAL obj = new EnterPriseConfigDAL(SessionHelper.EnterPriseDBName);
            eTurnsMaster.DAL.UserMasterDAL objeTurnsMaster = new eTurnsMaster.DAL.UserMasterDAL();
            int PrevLastNoOfAllowPwd = 2;
            if (SessionHelper.EnterPriceID > 0)
            {
                objDTO = obj.GetRecord(Convert.ToInt32(SessionHelper.EnterPriceID));
                PrevLastNoOfAllowPwd = objDTO.PreviousLastAllowedPWD ?? 0;
            }

            //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));
            string ApplyChangePassword = SiteSettingHelper.ApplyChangePassword; // Settinfile.Element("ApplyChangePassword").Value;

            string oldPassword = string.Empty;
            oldPassword = objeTurnsMaster.GetOldPassword(SessionHelper.UserID);

            if (!string.IsNullOrEmpty(ApplyChangePassword) && ApplyChangePassword.ToLower() == "yes")
            {
                oldPassword = CommonUtility.getSHA15Hash(objChangePasswordDTO.CurrentPassword);
            }

            var result = true;
            using (var context = new eTurns_MasterEntities(MasterDbConnectionHelper.GeteTurnsEntityFWConnectionString(MasterDbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.EFReadWrite.ToString("F"))))
            {
                IList<string> userPasswordChangeHistory = context.UserPasswordChangeHistories.Where(u => u.UserId == SessionHelper.UserID).OrderByDescending(m => m.Id).Select(m => m.OldPassword).Distinct().Take(PrevLastNoOfAllowPwd).ToList();
                if (userPasswordChangeHistory.Contains(CommonUtility.getSHA15Hash(objChangePasswordDTO.FirstPassword)) || CommonUtility.getSHA15Hash(objChangePasswordDTO.FirstPassword) == oldPassword)
                {
                    result = false;
                }
            }
            if (result == false)
            {
                status = ResMessage.RepeatPassword;//"repeatpwd";
                return Json(new { Message = status });
            }

            objChangePasswordDTO.FirstPassword = CommonUtility.getSHA15Hash(objChangePasswordDTO.FirstPassword);
            objChangePasswordDTO.CurrentPassword = oldPassword; //CommonUtility.getSHA15Hash(objChangePasswordDTO.CurrentPassword);

            if (!string.IsNullOrWhiteSpace(objChangePasswordDTO.FirstPassword))
            {

            }
            if (ValidateOldPassword(objChangePasswordDTO.CurrentPassword))
            {
                eTurns.DAL.UserMasterDAL objeTurns = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);

                UserPasswordChangeHistoryDTO objUserPasswordChangeHistoryDTO = new UserPasswordChangeHistoryDTO();

                objUserPasswordChangeHistoryDTO.UserId = SessionHelper.UserID;
                objUserPasswordChangeHistoryDTO.oldPassword = objChangePasswordDTO.CurrentPassword;
                objUserPasswordChangeHistoryDTO.NewPassword = objChangePasswordDTO.FirstPassword;
                objUserPasswordChangeHistoryDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                if (objeTurnsMaster.SaveChangePassword(objUserPasswordChangeHistoryDTO))
                {
                    if (!string.IsNullOrWhiteSpace(objUserPasswordChangeHistoryDTO.NewPassword))
                    {
                        objUserPasswordChangeHistoryDTO.NewPassword = objUserPasswordChangeHistoryDTO.NewPassword;
                    }
                    objeTurnsMaster.UpdatePassword(objUserPasswordChangeHistoryDTO);
                    if (SessionHelper.UserType > 1)
                    {
                        objeTurns.UpdatePassword(objUserPasswordChangeHistoryDTO);
                    }


                    SessionHelper.HasPasswordChanged = true;
                    status = "ok";
                }
                else
                {
                    status = "fail";
                }
            }
            else
            {
                status = "wrngpsw";

            }
            return Json(new { Message = status });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public JsonResult SaveUserChangePassword(UserMasterDTO objUserMasterDTO)
        {
            ChangePasswordDTO objChangePasswordDTO = new ChangePasswordDTO();
            objChangePasswordDTO = objUserMasterDTO.objChangePassword;
            string currentCulture = "en-US";
            if (System.Web.HttpContext.Current != null)
            {
                if (eTurns.DTO.Resources.ResourceHelper.CurrentCult != null)
                {
                    if (!string.IsNullOrWhiteSpace(Convert.ToString(eTurns.DTO.Resources.ResourceHelper.CurrentCult)))
                    {
                        currentCulture = eTurns.DTO.Resources.ResourceHelper.CurrentCult.ToString();
                    }

                }
            }
            else
            {
                eTurns.DTO.eTurnsRegionInfo objeTurnsRegionInfo = new RegionSettingDAL(SessionHelper.EnterPriseDBName).GetRegionSettingsById(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID);
                currentCulture = objeTurnsRegionInfo.CultureName;
            }
            if (objChangePasswordDTO == null || !string.IsNullOrWhiteSpace(objChangePasswordDTO.FirstPassword))
            {
                string passwordMsg = string.Empty;
                bool isStrong = CommonUtilityHelper.ValidateStrongPassword(objChangePasswordDTO.FirstPassword, out passwordMsg, currentCulture, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);
                if (!isStrong || !string.IsNullOrEmpty(passwordMsg))
                {
                    return Json(new { Message = "strongpassword", StrongPassword = passwordMsg });
                }
            }


            string status = "";
            EnterPriseConfigDTO objDTO = new EnterPriseConfigDTO();
            EnterPriseConfigDAL obj = new EnterPriseConfigDAL(SessionHelper.EnterPriseDBName);
            eTurnsMaster.DAL.UserMasterDAL objeTurnsMaster = new eTurnsMaster.DAL.UserMasterDAL();
            int PrevLastNoOfAllowPwd = 2;
            if (SessionHelper.EnterPriceID > 0)
            {
                objDTO = obj.GetRecord(Convert.ToInt32(SessionHelper.EnterPriceID));
                PrevLastNoOfAllowPwd = objDTO.PreviousLastAllowedPWD ?? 0;
            }

            //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));
            string ApplyChangePassword = SiteSettingHelper.ApplyChangePassword; // Settinfile.Element("ApplyChangePassword").Value;

            string oldPassword = string.Empty;
            oldPassword = objeTurnsMaster.GetOldPassword(SessionHelper.UserID);

            if (!string.IsNullOrEmpty(ApplyChangePassword) && ApplyChangePassword.ToLower() == "yes")
            {
                oldPassword = CommonUtility.getSHA15Hash(objChangePasswordDTO.CurrentPassword);
            }


            var result = true;
            using (eTurns_MasterEntities context = new eTurns_MasterEntities(MasterDbConnectionHelper.GeteTurnsEntityFWConnectionString(MasterDbConnectionHelper.GetETurnsMasterDBName(), DbConnectionType.EFReadWrite.ToString("F"))))
            {
                IList<string> userPasswordChangeHistory = context.UserPasswordChangeHistories.Where(u => u.UserId == SessionHelper.UserID).OrderByDescending(m => m.Id).Select(m => m.OldPassword).Distinct().Take(PrevLastNoOfAllowPwd).ToList();
                if (userPasswordChangeHistory.Contains(CommonUtility.getSHA15Hash(objChangePasswordDTO.FirstPassword)) || CommonUtility.getSHA15Hash(objChangePasswordDTO.FirstPassword) == oldPassword)
                {
                    result = false;
                }
            }
            if (result == false)
            {
                status = ResMessage.RepeatPassword;//"repeatpwd";
                return Json(new { Message = status });
            }

            objChangePasswordDTO.FirstPassword = CommonUtility.getSHA15Hash(objChangePasswordDTO.FirstPassword);
            objChangePasswordDTO.CurrentPassword = oldPassword;//CommonUtility.getSHA15Hash(objChangePasswordDTO.CurrentPassword);

            if (!string.IsNullOrWhiteSpace(objChangePasswordDTO.FirstPassword))
            {

            }
            if (ValidateOldPassword(objChangePasswordDTO.CurrentPassword))
            {
                eTurns.DAL.UserMasterDAL objeTurns = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
                UserPasswordChangeHistoryDTO objUserPasswordChangeHistoryDTO = new UserPasswordChangeHistoryDTO();

                objUserPasswordChangeHistoryDTO.UserId = SessionHelper.UserID;
                objUserPasswordChangeHistoryDTO.oldPassword = objChangePasswordDTO.CurrentPassword;
                objUserPasswordChangeHistoryDTO.NewPassword = objChangePasswordDTO.FirstPassword;
                objUserPasswordChangeHistoryDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                if (objeTurnsMaster.SaveChangePassword(objUserPasswordChangeHistoryDTO))
                {
                    if (!string.IsNullOrWhiteSpace(objUserPasswordChangeHistoryDTO.NewPassword))
                    {
                        objUserPasswordChangeHistoryDTO.NewPassword = objUserPasswordChangeHistoryDTO.NewPassword;
                    }
                    objeTurnsMaster.UpdatePassword(objUserPasswordChangeHistoryDTO);
                    if (SessionHelper.UserType > 1)
                    {
                        objeTurns.UpdatePassword(objUserPasswordChangeHistoryDTO);
                    }


                    SessionHelper.HasPasswordChanged = true;
                    status = "ok";
                }
                else
                {
                    status = "fail";
                }
            }
            else
            {
                status = "wrngpsw";

            }
            return Json(new { Message = status });
        }
        public bool ValidateOldPassword(string oldpassword)
        {
            eTurnsMaster.DAL.UserMasterDAL objeTurnsMaster = new eTurnsMaster.DAL.UserMasterDAL();
            return objeTurnsMaster.ValidateOldPassword(oldpassword, SessionHelper.UserID);

        }

        public JsonResult CheckPasswordChange()
        {
            //   bool NeedResetPassword = SessionHelper.GetAdminPermission(SessionHelper.ModuleList.PasswordResetRule);
            if (true)
            {
                string status = "";
                eTurnsMaster.DAL.UserMasterDAL objeTurnsMaster = new eTurnsMaster.DAL.UserMasterDAL();
                EnterPriseConfigDAL objDAL = new EnterPriseConfigDAL(SessionHelper.EnterPriseDBName);
                EnterPriseConfigDTO objDTO = objDAL.GetRecord(SessionHelper.EnterPriceID);
                int ExpiryDays = 0;
                if (objDTO != null)
                {
                    ExpiryDays = objDTO.PasswordExpiryDays ?? 0;
                }

                if (objeTurnsMaster.CheckPasswordChange(SessionHelper.UserID, ExpiryDays))
                {
                    status = "ok";
                }

                return Json(new { Message = status });
            }
            //else
            //{
            //    return Json(new { Message = string.Empty });
            //}
        }

        /// <summary>
        /// GetAllCustomerforAutoComplete
        /// </summary>
        /// <param name="NameStartWith"></param>
        /// <returns></returns>
        public JsonResult GetBinForAutocomplete(GetBinInputPara InputPara)
        {
            List<DTOForAutoComplete> kevValList = new List<DTOForAutoComplete>();
            List<DTOForAutoComplete> returnKeyValList = new List<DTOForAutoComplete>();
            DTOForAutoComplete obj = null;

            BinMasterDAL objBinDAL = null;
            BinMasterDTO objBinDTO = null;
            List<BinMasterDTO> objBinList = null;

            MaterialStagingDAL objMSDAL = null;
            MaterialStagingDTO MsDTO = null;

            MaterialStagingDetailDAL objMSDetailDAL = null;
            MaterialStagingDetailDTO objMSDetailDTO = null;
            List<MaterialStagingDetailDTO> MsDetailDTOList = null;

            ItemLocationQTYDAL objLocDtlQtyDAL = null;
            List<ItemLocationQTYDTO> objItemLocQtyList = null;


            Int64 RoomID = SessionHelper.RoomID;
            Int64 CompID = SessionHelper.CompanyID;
            Guid itemGuid = Guid.Empty;
            string NameStartWith = InputPara.NameStartWith;

            try
            {
                Guid.TryParse(InputPara.ItemGuid, out itemGuid);

                if (InputPara.GetAllBins)
                {
                    if (InputPara.IncludeQty && itemGuid != Guid.Empty)
                    {
                        if (InputPara.IsStaging)
                        {
                            objMSDetailDAL = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName);
                            //MsDetailDTOList = objMSDetailDAL.GetAllRecords(RoomID, CompID).Where(x => x.ItemGUID == itemGuid).ToList();
                            MsDetailDTOList = objMSDetailDAL.GetMaterialStagingDetailbyItemGUIDStagingBINID(string.Empty, Convert.ToString(itemGuid), null, RoomID, CompID, null, null).ToList(); ;
                        }
                        else
                        {
                            objLocDtlQtyDAL = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName);
                            //objItemLocQtyList = objLocDtlQtyDAL.GetAllRecords(RoomID, CompID).Where(x => x.ItemGUID == itemGuid).ToList();
                            objItemLocQtyList = objLocDtlQtyDAL.GetItemLocationQTY(RoomID, CompID, null, Convert.ToString(itemGuid)).ToList();
                        }
                    }
                    objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                    //objBinList = objBinDAL.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(x => x.IsStagingLocation == InputPara.IsStaging).ToList();
                    if (itemGuid != Guid.Empty && InputPara.IncludeQty && InputPara.GetAllBins)
                        objBinList = objBinDAL.GetInventoryAndStagingBinsByItem(SessionHelper.RoomID, SessionHelper.CompanyID, itemGuid);
                    else
                        objBinList = objBinDAL.GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Guid.Empty, 0, null, InputPara.IsStaging).ToList();//.Where(x => x.IsStagingLocation == InputPara.IsStaging)
                    foreach (var item in objBinList)
                    {
                        obj = new DTOForAutoComplete();
                        obj.ID = item.ID;
                        // obj.GUID = item.GUID;
                        obj.Key = item.BinNumber;
                        obj.Value = item.BinNumber;
                        obj.Quantity = 0;

                        if (InputPara.IncludeQty && itemGuid != Guid.Empty)
                        {
                            if (InputPara.IsStaging)
                                obj.Quantity = MsDetailDTOList.Where(x => x.ItemGUID == itemGuid && x.StagingBinID == item.ID).Sum(x => x.Quantity);
                            else
                                obj.Quantity = objItemLocQtyList.Where(x => x.ItemGUID == itemGuid && x.BinID == item.ID).Sum(x => x.Quantity);
                        }

                        kevValList.Add(obj);
                    }

                }
                else if (!string.IsNullOrEmpty(InputPara.StagingHeaderName))
                {
                    objMSDAL = new MaterialStagingDAL(SessionHelper.EnterPriseDBName);
                    //MsDTO = objMSDAL.GetAllRecords(RoomID, CompID, false, false).Where(x => x.StagingName == InputPara.StagingHeaderName).FirstOrDefault();
                    MsDTO = objMSDAL.GetMaterialStaging(RoomID, CompID, false, false, InputPara.StagingHeaderName, null).FirstOrDefault();
                    if (MsDTO != null)
                    {
                        objMSDetailDAL = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName);
                        //MsDetailDTOList = objMSDetailDAL.GetAllRecords(RoomID, CompID).Where(x => x.MaterialStagingGUID == MsDTO.GUID && !x.IsArchived && !x.IsDeleted).OrderBy(x => x.StagingBinName).ToList();
                        MsDetailDTOList = objMSDetailDAL.GetMaterialStagingDetailbyItemGUIDStagingBINID(Convert.ToString(MsDTO.GUID), string.Empty, null, RoomID, CompID, false, false).OrderBy(x => x.StagingBinName).ToList();
                        if ((MsDetailDTOList == null || MsDetailDTOList.Count() <= 0) && MsDTO.BinID > 0)
                        {
                            objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                            objBinDTO = objBinDAL.GetBinByID(MsDTO.BinID.GetValueOrDefault(0), SessionHelper.RoomID, SessionHelper.CompanyID);
                            //objBinDTO = objBinDAL.GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false,Guid.Empty, MsDTO.BinID.GetValueOrDefault(0), null,null).FirstOrDefault();

                            objMSDetailDTO = new MaterialStagingDetailDTO();
                            objMSDetailDTO.StagingBinName = objBinDTO.BinNumber;
                            objMSDetailDTO.Quantity = 0;

                            MsDetailDTOList = new List<MaterialStagingDetailDTO>();
                            MsDetailDTOList.Add(objMSDetailDTO);
                        }

                        foreach (var item in MsDetailDTOList)
                        {
                            obj = new DTOForAutoComplete();
                            obj.ID = item.ID;
                            //obj.GUID = item.GUID;
                            obj.Key = item.StagingBinName;
                            obj.Value = item.StagingBinName;
                            obj.Quantity = 0;

                            if (InputPara.IncludeQty && itemGuid != Guid.Empty)
                            {
                                obj.Quantity = MsDetailDTOList.Where(x => x.ItemGUID == itemGuid && x.StagingBinID == item.ID).Sum(x => x.Quantity);
                            }

                            kevValList.Add(obj);
                        }
                    }
                }
                else if (itemGuid != Guid.Empty && !InputPara.IsStaging)
                {
                    objLocDtlQtyDAL = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName);
                    //objItemLocQtyList = objLocDtlQtyDAL.GetAllRecords(RoomID, CompID).Where(x => x.ItemGUID == itemGuid).ToList();
                    objItemLocQtyList = objLocDtlQtyDAL.GetItemLocationQTY(RoomID, CompID, null, Convert.ToString(itemGuid)).ToList();
                    foreach (var item in objItemLocQtyList)
                    {
                        obj = new DTOForAutoComplete();
                        obj.ID = item.BinID;
                        obj.Key = item.BinNumber;
                        obj.Value = item.BinNumber;
                        obj.Quantity = item.Quantity;
                        kevValList.Add(obj);
                    }
                }

                if (kevValList != null)
                {

                    if (InputPara.IncludeQty)
                    {
                        kevValList.ForEach(x => x.Value += " (" + x.Quantity.ToString("N" + SessionHelper.NumberDecimalDigits) + ")");
                    }

                    if (InputPara.ExcludeZeroQty && InputPara.IncludeQty)
                    {
                        kevValList = kevValList.Where(x => x.Quantity > 0).ToList();
                    }

                    foreach (var item in kevValList)
                    {
                        if (returnKeyValList.FindIndex(x => x.Value == item.Value) < 0)
                            returnKeyValList.Add(item);
                    }

                    if (!string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                    {
                        //InputPara.NameStartWith = InputPara.NameStartWith.Replace("#B", "");
                        returnKeyValList = returnKeyValList.Where(x => x.Key.ToLower().StartsWith(NameStartWith.ToLower())).ToList();
                    }
                }

            }
            catch (Exception)
            {
                //throw ex;
                //Log Error Here
            }
            finally
            {
                obj = null;

                objBinDAL = null;
                objBinDTO = null;
                objBinList = null;

                objMSDAL = null;
                MsDTO = null;

                objMSDetailDAL = null;
                objMSDetailDTO = null;
                MsDetailDTOList = null;

                objLocDtlQtyDAL = null;
                objItemLocQtyList = null;


                RoomID = SessionHelper.RoomID;
                CompID = SessionHelper.CompanyID;
                itemGuid = Guid.Empty;
            }

            return Json(returnKeyValList, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetBinForItemOrStaging(Guid ItemGuid, string StagingHeaderName, bool IncludeQty, string NameStartWith, bool? IsLoadMoreLocations = null)
        {
            List<DTOForAutoComplete> kevValList = new List<DTOForAutoComplete>();
            List<DTOForAutoComplete> returnKeyValList = new List<DTOForAutoComplete>();
            DTOForAutoComplete obj = null;

            BinMasterDAL objBinDAL = null;
            BinMasterDTO objBinDTO = null;
            List<BinMasterDTO> objBinList = null;

            MaterialStagingDAL objMSDAL = null;
            MaterialStagingDTO MsDTO = null;

            MaterialStagingDetailDAL objMSDetailDAL = null;
            MaterialStagingDetailDTO objMSDetailDTO = null;
            List<MaterialStagingDetailDTO> MsDetailDTOList = null;

            //ItemLocationQTYDAL objLocDtlQtyDAL = null;
            List<ItemLocationQTYDTO> objItemLocQtyList = null;

            Int64 RoomID = SessionHelper.RoomID;
            Int64 CompID = SessionHelper.CompanyID;

            try
            {
                if (!string.IsNullOrEmpty(StagingHeaderName))
                {
                    objMSDAL = new MaterialStagingDAL(SessionHelper.EnterPriseDBName);
                    //MsDTO = objMSDAL.GetAllRecords(RoomID, CompID, false, false).Where(x => x.StagingName == StagingHeaderName).FirstOrDefault();
                    MsDTO = objMSDAL.GetMaterialStaging(RoomID, CompID, false, false, StagingHeaderName, null).FirstOrDefault();
                    if (MsDTO != null)
                    {
                        objMSDetailDAL = new MaterialStagingDetailDAL(SessionHelper.EnterPriseDBName);
                        //MsDetailDTOList = objMSDetailDAL.GetAllRecords(RoomID, CompID).Where(x => x.MaterialStagingGUID == MsDTO.GUID && !x.IsArchived && !x.IsDeleted).OrderBy(x => x.StagingBinName).ToList();
                        MsDetailDTOList = objMSDetailDAL.GetMaterialStagingDetailbyItemGUIDStagingBINID(Convert.ToString(MsDTO.GUID), string.Empty, null, RoomID, CompID, false, false).OrderBy(x => x.StagingBinName).ToList();
                        if ((MsDetailDTOList == null || MsDetailDTOList.Count() <= 0) && MsDTO.BinID > 0)
                        {
                            objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                            objBinDTO = objBinDAL.GetBinByID(MsDTO.BinID.GetValueOrDefault(0), SessionHelper.RoomID, SessionHelper.CompanyID);
                            //objBinDTO = objBinDAL.GetItemLocation( SessionHelper.RoomID, SessionHelper.CompanyID, false, false,Guid.Empty, MsDTO.BinID.GetValueOrDefault(0),null,null).FirstOrDefault();

                            objMSDetailDTO = new MaterialStagingDetailDTO();
                            objMSDetailDTO.StagingBinName = objBinDTO.BinNumber;
                            objMSDetailDTO.Quantity = 0;

                            MsDetailDTOList = new List<MaterialStagingDetailDTO>();
                            MsDetailDTOList.Add(objMSDetailDTO);
                        }

                        foreach (var item in MsDetailDTOList)
                        {
                            obj = new DTOForAutoComplete();
                            obj.ID = item.ID;
                            obj.GUID = item.GUID;
                            obj.Key = item.StagingBinName;
                            obj.Value = item.StagingBinName;
                            obj.Quantity = 0;

                            if (IncludeQty && ItemGuid != Guid.Empty)
                            {
                                obj.Quantity = MsDetailDTOList.Where(x => x.ItemGUID == ItemGuid && x.StagingBinID == item.StagingBinID).Sum(x => x.Quantity);
                            }

                            kevValList.Add(obj);
                        }
                    }

                    if (IsLoadMoreLocations.HasValue)
                    {
                        if (IsLoadMoreLocations.Value)
                        {
                            //IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllBins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, NameStartWith.ToLower(), true).Where(x => !string.IsNullOrWhiteSpace(x.BinNumber) && x.BinNumber != "[|EmptyStagingBin|]").OrderBy(x => x.BinNumber);
                            IEnumerable<BinMasterDTO> objBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllBins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, NameStartWith.ToLower(), true, string.Empty, false, false).OrderBy(x => x.BinNumber);
                            if (objBinDTOList != null && objBinDTOList.Count() > 0)
                            {
                                foreach (var item in objBinDTOList)
                                {
                                    if (!kevValList.Any(x => x.Key == item.BinNumber))
                                    {
                                        DTOForAutoComplete objAutoDTO = new DTOForAutoComplete()
                                        {
                                            GUID = Guid.Empty,
                                            ID = 0,
                                            Key = item.BinNumber,
                                            Value = item.BinNumber,
                                            OtherInfo1 = string.Empty,
                                            Quantity = 0
                                        };
                                        kevValList.Add(objAutoDTO);
                                    }
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
                            kevValList.Add(objAutoDTO);
                        }
                    }
                }
                else if (ItemGuid != Guid.Empty)
                {
                    objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);

                    if (IsLoadMoreLocations.HasValue)
                    {
                        //objBinList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByItemLocationLevelQuanity(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGuid.ToString()).Where(x => !x.IsStagingLocation && !string.IsNullOrWhiteSpace(x.BinNumber)).OrderBy(x => x.BinNumber).ToList();
                        objBinList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecordsByItemLocationLevelQuanity(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ItemGuid.ToString(), false, string.Empty, false, null).OrderBy(x => x.BinNumber).ToList();
                        //   objBinList = objBinDAL.GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Guid.Parse(ItemGuid.ToString()),0,null,false).Where(x => !string.IsNullOrWhiteSpace(x.BinNumber)).OrderBy(x => x.BinNumber).ToList();//!x.IsStagingLocation && 
                    }
                    else
                        objBinList = objBinDAL.GetAllBins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, string.Empty, false);// .GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(x => x.IsStagingLocation == false && x.ItemGUID == ItemGuid).ToList();

                    objItemLocQtyList = new ItemLocationQTYDAL(SessionHelper.EnterPriseDBName).GetRecordByItem(ItemGuid, SessionHelper.RoomID, SessionHelper.CompanyID).ToList();

                    foreach (var item in objBinList)
                    {
                        obj = new DTOForAutoComplete();
                        obj.ID = item.ID;
                        obj.GUID = item.GUID;
                        obj.Key = item.BinNumber;
                        obj.Value = item.BinNumber;
                        obj.Quantity = 0;

                        obj.OtherInfo1 = item.DefaultPullQuantity.GetValueOrDefault(0) > 0 ? Convert.ToString(item.DefaultPullQuantity.GetValueOrDefault(0)) : "";
                        obj.OtherInfo2 = item.DefaultReorderQuantity.GetValueOrDefault(0) > 0 ? Convert.ToString(item.DefaultReorderQuantity.GetValueOrDefault(0)) : "";

                        if (IncludeQty && ItemGuid != Guid.Empty)
                        {
                            obj.Quantity = objItemLocQtyList.Where(x => x.ItemGUID == ItemGuid && x.BinID == item.ID).Sum(x => x.Quantity);
                        }

                        kevValList.Add(obj);
                    }

                    if (IsLoadMoreLocations.HasValue)
                    {
                        if (IsLoadMoreLocations.Value)
                        {
                            //IEnumerable<BinMasterDTO> oAllBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllBins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, NameStartWith.ToLower(), false).Where(x => !string.IsNullOrWhiteSpace(x.BinNumber) && x.BinNumber != "[|EmptyStagingBin|]").OrderBy(x => x.BinNumber);
                            IEnumerable<BinMasterDTO> oAllBinDTOList = new BinMasterDAL(SessionHelper.EnterPriseDBName).GetAllBins(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, NameStartWith.ToLower(), false, string.Empty, false, false).OrderBy(x => x.BinNumber);
                            if (oAllBinDTOList != null && oAllBinDTOList.Count() > 0)
                            {
                                if (!string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                                {
                                    oAllBinDTOList = oAllBinDTOList.Where(x => x.BinNumber.ToLower().StartsWith(NameStartWith.ToLower())).ToList();
                                }

                                foreach (var item in oAllBinDTOList)
                                {
                                    if (!kevValList.Any(x => x.Key == item.BinNumber))
                                    {
                                        DTOForAutoComplete objNew = new DTOForAutoComplete()
                                        {
                                            Key = item.BinNumber,
                                            Value = item.BinNumber,
                                            ID = item.ID,
                                            GUID = item.GUID,
                                            Quantity = 0,
                                            OtherInfo1 = item.DefaultPullQuantity.GetValueOrDefault(0) > 0 ? Convert.ToString(item.DefaultPullQuantity.GetValueOrDefault(0)) : "",
                                            OtherInfo2 = item.DefaultReorderQuantity.GetValueOrDefault(0) > 0 ? Convert.ToString(item.DefaultReorderQuantity.GetValueOrDefault(0)) : ""

                                        };
                                        kevValList.Add(objNew);
                                    }
                                }

                            }
                            if (kevValList != null && kevValList.Count > 0)
                            {
                                kevValList = kevValList.OrderBy(r => r.Key).ToList();
                            }
                        }
                        else
                        {
                            if (kevValList != null && kevValList.Count > 0)
                            {
                                kevValList = kevValList.OrderBy(r => r.Key).ToList();
                            }
                            DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                            objAutoDTO.Key = ResBin.MoreLocations;
                            objAutoDTO.Value = ResBin.MoreLocations;
                            objAutoDTO.ID = 0;
                            objAutoDTO.GUID = Guid.Empty;
                            kevValList.Add(objAutoDTO);
                        }
                    }
                    else
                    {
                        if (kevValList != null && kevValList.Count > 0)
                        {
                            kevValList = kevValList.OrderBy(r => r.Key).ToList();
                        }
                    }
                }

                if (kevValList != null)
                {

                    if (IncludeQty)
                    {
                        kevValList = kevValList.Where(x => x.Quantity > 0).ToList();
                        kevValList.ForEach(x => x.Value += " (" + x.Quantity.ToString("N" + SessionHelper.NumberDecimalDigits) + ")");
                    }

                    foreach (var item in kevValList)
                    {
                        if (returnKeyValList.FindIndex(x => x.Value == item.Value) < 0)
                            returnKeyValList.Add(item);
                    }

                    if (!string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                    {
                        returnKeyValList = returnKeyValList.Where(x => x.Key.ToLower().StartsWith(NameStartWith.ToLower())).ToList();
                    }
                }

            }
            catch (Exception)
            {
                //throw ex;
                //Log Error Here
            }
            finally
            {
                obj = null;

                objBinDAL = null;
                objBinDTO = null;
                objBinList = null;

                objMSDAL = null;
                MsDTO = null;

                objMSDetailDAL = null;
                objMSDetailDTO = null;
                MsDetailDTOList = null;

                //objLocDtlQtyDAL = null;
                objItemLocQtyList = null;

            }

            return Json(returnKeyValList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetToolLocationAutoComplete(Guid ToolGuid, bool IncludeQty, string NameStartWith, bool? IsLoadMoreLocations = null)
        {
            List<DTOForAutoComplete> kevValList = new List<DTOForAutoComplete>();
            List<DTOForAutoComplete> returnKeyValList = new List<DTOForAutoComplete>();
            DTOForAutoComplete obj = null;

            LocationMasterDAL objBinDAL = null;
            List<LocationMasterDTO> objBinList = null;


            //ItemLocationQTYDAL objLocDtlQtyDAL = null;
            List<ToolLocationDetailsDTO> objItemLocQtyList = null;

            Int64 RoomID = SessionHelper.RoomID;
            Int64 CompID = SessionHelper.CompanyID;

            try
            {

                if (ToolGuid != Guid.Empty)
                {
                    objBinDAL = new LocationMasterDAL(SessionHelper.EnterPriseDBName);

                    if (IsLoadMoreLocations.HasValue)
                    {
                        objBinList = new LocationMasterDAL(SessionHelper.EnterPriseDBName).GetLocationByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID).OrderBy(x => x.Location).ToList();
                        //   objBinList = objBinDAL.GetItemLocation(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, Guid.Parse(ItemGuid.ToString()),0,null,false).Where(x => !string.IsNullOrWhiteSpace(x.BinNumber)).OrderBy(x => x.BinNumber).ToList();//!x.IsStagingLocation && 
                    }
                    else
                        objBinList = objBinDAL.GetLocationByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID).ToList();

                    objItemLocQtyList = new ToolLocationDetailsDAL(SessionHelper.EnterPriseDBName).GetToolLocationsByToolGUID(SessionHelper.RoomID, SessionHelper.CompanyID, false, false, ToolGuid).ToList();

                    foreach (var item in objBinList)
                    {
                        obj = new DTOForAutoComplete();
                        obj.ID = item.ID;
                        obj.GUID = item.GUID;
                        obj.Key = item.Location;
                        obj.Value = item.Location;
                        obj.Quantity = 0;

                        if (IncludeQty && ToolGuid != Guid.Empty)
                        {
                            obj.Quantity = objItemLocQtyList.Where(x => x.ToolGuid == ToolGuid && x.LocationGUID == item.GUID).Sum(x => x.ID);
                        }

                        kevValList.Add(obj);
                    }

                    if (IsLoadMoreLocations.HasValue)
                    {
                        if (IsLoadMoreLocations.Value)
                        {
                            List<LocationMasterDTO> oAllBinDTOList = new LocationMasterDAL(SessionHelper.EnterPriseDBName).GetLocationByRoomPlain(SessionHelper.RoomID, SessionHelper.CompanyID).Where(x => !string.IsNullOrWhiteSpace(x.Location) && x.Location.ToLower().Trim() != NameStartWith.ToLower().Trim()).OrderBy(x => x.Location).ToList();
                            if (oAllBinDTOList != null && oAllBinDTOList.Count() > 0)
                            {
                                if (!string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                                {
                                    oAllBinDTOList = oAllBinDTOList.Where(x => x.Location.ToLower().StartsWith(NameStartWith.ToLower())).ToList();
                                }

                                foreach (var item in oAllBinDTOList)
                                {
                                    if (!kevValList.Any(x => x.Key == item.Location))
                                    {
                                        DTOForAutoComplete objNew = new DTOForAutoComplete()
                                        {
                                            Key = item.Location,
                                            Value = item.Location,
                                            ID = item.ID,
                                            GUID = item.GUID,
                                            Quantity = 0
                                        };
                                        kevValList.Add(objNew);
                                    }
                                }

                            }
                            if (kevValList != null && kevValList.Count > 0)
                            {
                                kevValList = kevValList.OrderBy(r => r.Key).ToList();
                            }
                        }
                        else
                        {
                            if (kevValList != null && kevValList.Count > 0)
                            {
                                kevValList = kevValList.OrderBy(r => r.Key).ToList();
                            }
                            DTOForAutoComplete objAutoDTO = new DTOForAutoComplete();
                            objAutoDTO.Key = ResBin.MoreLocations;
                            objAutoDTO.Value = ResBin.MoreLocations;
                            objAutoDTO.ID = 0;
                            objAutoDTO.GUID = Guid.Empty;
                            kevValList.Add(objAutoDTO);
                        }
                    }
                }

                if (kevValList != null)
                {

                    if (IncludeQty)
                    {
                        kevValList = kevValList.Where(x => x.Quantity > 0).ToList();
                        kevValList.ForEach(x => x.Value += " (" + x.Quantity.ToString("N" + SessionHelper.NumberDecimalDigits) + ")");
                    }

                    foreach (var item in kevValList)
                    {
                        if (returnKeyValList.FindIndex(x => x.Value == item.Value) < 0)
                            returnKeyValList.Add(item);
                    }

                    if (!string.IsNullOrEmpty(NameStartWith) && !string.IsNullOrWhiteSpace(NameStartWith))
                    {
                        returnKeyValList = returnKeyValList.Where(x => x.Key.ToLower().StartsWith(NameStartWith.ToLower())).ToList();
                    }
                }

            }
            catch (Exception)
            {
                //throw ex;
                //Log Error Here
            }
            finally
            {
                obj = null;

                objBinDAL = null;
                objBinList = null;


                //objLocDtlQtyDAL = null;
                objItemLocQtyList = null;

            }

            return Json(returnKeyValList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        [HttpPost]
        public JsonResult SetReportParaDictionary(List<KeyValDTO> paras, string Id)
        {
            string strPath = string.Empty;
            string strReplacepath = string.Empty;

            if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Request != null)
            {
                strPath = System.Web.HttpContext.Current.Request.Url.ToString();
                strReplacepath = System.Web.HttpContext.Current.Request.Url.PathAndQuery;
                strPath = strPath.Replace(strReplacepath, "/");
            }
            else if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["DomainName"]))
            {
                strPath = System.Configuration.ConfigurationManager.AppSettings["DomainName"];

            }
            string DBName = SessionHelper.EnterPriseDBName;
            ReportMasterDAL objRptDAL = new ReportMasterDAL(SessionHelper.EnterPriseDBName);
            ReportBuilderDTO objRptDTO = new ReportBuilderDTO();

            if (!string.IsNullOrWhiteSpace(Id))
            {
                objRptDTO = objRptDAL.GetReportDetail(Convert.ToInt64(Id));
                if (objRptDTO != null && (objRptDTO.ReportName == "EnterpriseUser" || ((!string.IsNullOrEmpty(objRptDTO.ReportName) && objRptDTO.ReportName.ToLower() == "users")
                    || (!string.IsNullOrEmpty(objRptDTO.ModuleName) && objRptDTO.ModuleName.ToLower() == "userslist"))))
                {
                    DBName = DbConnectionHelper.GetETurnsMasterDBName();
                }
            }
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            string BasePath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;
            string DBServerName = ConfigurationManager.AppSettings["DBserverName"];
            string DBUserName = ConfigurationManager.AppSettings["DbUserName"];
            string DBPassword = ConfigurationManager.AppSettings["DbPassword"];

            string connectionString = @"Data Source={0};Initial Catalog={1};User ID={2};Password={3}";
            connectionString = string.Format(connectionString, DBServerName, DBName, DBUserName, DBPassword);
            string _strRunWithReportConnection = "No";
            if (SiteSettingHelper.RunWithReportConnection != string.Empty)
            {
                _strRunWithReportConnection = Convert.ToString(SiteSettingHelper.RunWithReportConnection);
            }

            if (objRptDTO != null && !string.IsNullOrEmpty(_strRunWithReportConnection) && _strRunWithReportConnection.ToLower() == "yes" && objRptDTO.ReportAppIntent == "ReadOnly")
            {
                connectionString = DbConnectionHelper.GeteTurnsSQLConnectionString(DBName, DbConnectionType.GeneralReadOnly.ToString("F"));
            }
            else
            {
                connectionString = DbConnectionHelper.GeteTurnsSQLConnectionString(DBName, DbConnectionType.GeneralReadWrite.ToString("F"));
            }
            dictionary.Add("ConnectionString", connectionString);
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            if (ConfigurationManager.AppSettings["IsServer"] == "True")
            {
                string baseURL = System.Web.HttpContext.Current.Request.Url.ToString().Replace(System.Web.HttpContext.Current.Request.Url.PathAndQuery, "");
                baseURL = SessionHelper.CurrentDomainURL;
                if (objCommonDAL.HasSpecialDomain(SessionHelper.CurrentDomainURL, SessionHelper.EnterPriceID))
                {
                    dictionary.Add("eTurnsLogoURL", baseURL + ConvertImageToPNG(eTurnsAppConfig.BaseFileSharedPath, "Uploads\\EnterpriseLogos\\" + SessionHelper.EnterPriceID + "\\" + SessionHelper.EnterpriseLogoUrl));
                    dictionary.Add("EnterpriseLogoURL", baseURL + "/Content/OpenAccess/NologoReport.png");
                }
                else
                {
                    dictionary.Add("eTurnsLogoURL", baseURL + "/Content/OpenAccess/logoInReport.png");
                    dictionary.Add("EnterpriseLogoURL", baseURL + ConvertImageToPNG(eTurnsAppConfig.BaseFileSharedPath, "Uploads\\EnterpriseLogos\\" + SessionHelper.EnterPriceID + "\\" + SessionHelper.EnterpriseLogoUrl));
                }
                dictionary.Add("CompanyLogoURL", baseURL + ConvertImageToPNG(eTurnsAppConfig.BaseFileSharedPath, "Uploads\\CompanyLogos\\" + SessionHelper.CompanyID + "\\" + SessionHelper.CompanyLogoUrl));
                dictionary.Add("BarcodeURL", baseURL + "/Barcode/GetBarcodeByKey?barcodekey=");
                dictionary.Add("WOSignatureURL", baseURL + "/Uploads/WorkOrderSignature/" + SessionHelper.CompanyID + "/");
                dictionary.Add("WOAttachmentPath", baseURL + "/Uploads/WorkOrderFile/" + SessionHelper.EnterPriceID + "/" + SessionHelper.CompanyID + "/" + SessionHelper.RoomID + "/");
            }
            else
            {
                if (objCommonDAL.HasSpecialDomain(SessionHelper.CurrentDomainURL, SessionHelper.EnterPriceID))
                {
                    dictionary.Add("eTurnsLogoURL", "https://localhost:" + System.Web.HttpContext.Current.Request.Url.Port + ConvertImageToPNG(BasePath, "Uploads\\EnterpriseLogos\\" + SessionHelper.EnterPriceID + "\\" + SessionHelper.EnterpriseLogoUrl));
                    dictionary.Add("EnterpriseLogoURL", "https://localhost:" + "/Content/OpenAccess/NologoReport.png");
                }
                else
                {
                    dictionary.Add("eTurnsLogoURL", "https://localhost:" + System.Web.HttpContext.Current.Request.Url.Port + "/Content/OpenAccess/logoInReport.png");
                    dictionary.Add("EnterpriseLogoURL", "https://localhost:" + System.Web.HttpContext.Current.Request.Url.Port + ConvertImageToPNG(BasePath, "Uploads\\EnterpriseLogos\\" + SessionHelper.EnterPriceID + "\\" + SessionHelper.EnterpriseLogoUrl));
                }
                dictionary.Add("CompanyLogoURL", "https://localhost:" + System.Web.HttpContext.Current.Request.Url.Port + ConvertImageToPNG(BasePath, "Uploads\\CompanyLogos\\" + SessionHelper.CompanyID + "\\" + SessionHelper.CompanyLogoUrl));
                dictionary.Add("BarcodeURL", "https://localhost:" + System.Web.HttpContext.Current.Request.Url.Port + "/Barcode/GetBarcodeByKey?barcodekey=");
                dictionary.Add("WOSignatureURL", "https://localhost:" + System.Web.HttpContext.Current.Request.Url.Port + "/Uploads/WorkOrderSignature/" + SessionHelper.CompanyID + "/");
                dictionary.Add("WOAttachmentPath", "https://localhost:" + System.Web.HttpContext.Current.Request.Url.Port + "/Uploads/WorkOrderFile/" + SessionHelper.EnterPriceID + "/" + SessionHelper.CompanyID + "/" + SessionHelper.RoomID + "/");
            }


            //dictionary.Add("EnterpriseLogoURL", Request.Url.OriginalString.Replace(Request.RawUrl, "") + "/Uploads/EnterpriseLogos/" + SessionHelper.EnterPriceID + "/" + SessionHelper.EnterpriseLogoUrl);
            //dictionary.Add("BarcodeURL", Request.Url.OriginalString.Replace(Request.RawUrl, "") + "/Barcode/GetBarcodeByKey?barcodekey=");

            dictionary.Add("UserID", SessionHelper.UserID.ToString());
            dictionary.Add("EnterpriseID", SessionHelper.EnterPriceID.ToString());

            if (paras != null && paras.Count > 0)
            {
                foreach (var item in paras)
                {
                    if (item.key.ToLower() == "startdate")
                    {
                        KeyValDTO keyval = paras.Where(p => p.key.ToLower() == "starttime").FirstOrDefault();

                        if (keyval != null)
                        {
                            string[] Hours_Minutes = keyval.value.Split(':');
                            int TotalSeconds = 0;
                            if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                            {
                                TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                                TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                            }
                            dictionary.Add("OrigStartDate", DateTime.ParseExact(item.value, (SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(TotalSeconds).ToString("yyyy-MM-dd HH:mm:ss"));

                            if (objRptDTO.MasterReportResFile == "RES_RPT_PullSummaryByQuarter")
                            {
                                item.value = DateTime.ParseExact(item.value, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult).ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else
                            {
                                item.value = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(item.value, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                            }
                        }
                        else
                        {
                            dictionary.Add("OrigStartDate", DateTime.ParseExact(item.value, (SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).ToString("yyyy-MM-dd HH:mm:ss"));

                            if (objRptDTO.MasterReportResFile == "RES_RPT_PullSummaryByQuarter")
                            {
                                item.value = DateTime.ParseExact(item.value, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult).ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else
                            {
                                item.value = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(item.value, (SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString("yyyy-MM-dd HH:mm:ss");
                            }

                        }
                    }
                    if (item.key.ToLower() == "enddate")
                    {

                        KeyValDTO keyval = paras.Where(p => p.key.ToLower() == "endtime").FirstOrDefault();
                        if (keyval != null)
                        {
                            string[] Hours_Minutes = keyval.value.Split(':');
                            int TotalSeconds = 0;
                            if (Hours_Minutes != null && Hours_Minutes.Length > 0)
                            {
                                TotalSeconds = Convert.ToInt32(Hours_Minutes[0]) * 60 * 60;
                                TotalSeconds += Convert.ToInt32(Hours_Minutes[1]) * 60;
                            }
                            TotalSeconds += 59;
                            dictionary.Add("OrigEndDate", DateTime.ParseExact(item.value, (SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(TotalSeconds).ToString("yyyy-MM-dd HH:mm:ss"));

                            if (objRptDTO.MasterReportResFile == "RES_RPT_PullSummaryByQuarter")
                            {
                                item.value = DateTime.ParseExact(item.value, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399).ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else
                            {
                                item.value = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(item.value, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(TotalSeconds), SessionHelper.CurrentTimeZone).ToString();
                            }

                        }
                        else
                        {
                            dictionary.Add("OrigEndDate", DateTime.ParseExact(item.value, (SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399).ToString("yyyy-MM-dd HH:mm:ss"));

                            if (objRptDTO.MasterReportResFile == "RES_RPT_PullSummaryByQuarter")
                            {
                                item.value = DateTime.ParseExact(item.value, SessionHelper.RoomDateFormat, ResourceHelper.CurrentCult).AddSeconds(86399).ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else
                            {
                                item.value = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(item.value, (SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString("yyyy-MM-dd HH:mm:ss");
                            }
                        }
                    }

                    if (item.key.ToLower() != "starttime" && item.key.ToLower() != "endtime")
                    {
                        dictionary.Add(item.key, item.value);
                    }
                }
            }

            SessionHelper.Add("ReportPara", dictionary);

            return Json(new { ReportURL = "/Reports/NewReportViewer.aspx?ID=" + Id }, JsonRequestBehavior.AllowGet);

        }

        //[HttpPost]
        //public JsonResult SetReportParaDictionary(string paras)
        //{
        //    return null;
        //}


        public JsonResult SetPDFReportParaDictionary(List<KeyValDTO> paras, string ReportID, NotificationDTO objNotificationDTO)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            string BasePath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;
            string DBServerName = ConfigurationManager.AppSettings["DBserverName"];
            string DBUserName = ConfigurationManager.AppSettings["DbUserName"];
            string DBPassword = ConfigurationManager.AppSettings["DbPassword"];
            string DBName = SessionHelper.EnterPriseDBName;
            string connectionString = @"Data Source={0};Initial Catalog={1};User ID={2};Password={3}";
            connectionString = string.Format(connectionString, DBServerName, DBName, DBUserName, DBPassword);
            connectionString = DbConnectionHelper.GeteTurnsSQLConnectionString(SessionHelper.EnterPriseDBName, DbConnectionType.GeneralReadOnly.ToString("F"));
            dictionary.Add("ConnectionString", connectionString);
            CommonDAL objCommonDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            if (System.Web.HttpContext.Current.Request != null && System.Web.HttpContext.Current.Request.Url != null)
            {
                if (ConfigurationManager.AppSettings["IsServer"] == "True")
                {
                    string baseURL = System.Web.HttpContext.Current.Request.Url.ToString().Replace(System.Web.HttpContext.Current.Request.Url.PathAndQuery, "");
                    baseURL = SessionHelper.CurrentDomainURL;
                    if (objCommonDAL.HasSpecialDomain(SessionHelper.CurrentDomainURL, SessionHelper.EnterPriceID))
                    {
                        dictionary.Add("eTurnsLogoURL", baseURL + ConvertImageToPNG(eTurnsAppConfig.BaseFileSharedPath, "Uploads\\EnterpriseLogos\\" + SessionHelper.EnterPriceID + "\\" + SessionHelper.EnterpriseLogoUrl));
                        dictionary.Add("EnterpriseLogoURL", baseURL + "/Content/OpenAccess/NologoReport.png");
                    }
                    else
                    {
                        dictionary.Add("eTurnsLogoURL", baseURL + "/Content/OpenAccess/logoInReport.png");
                        dictionary.Add("EnterpriseLogoURL", baseURL + ConvertImageToPNG(eTurnsAppConfig.BaseFileSharedPath, "Uploads\\EnterpriseLogos\\" + SessionHelper.EnterPriceID + "\\" + SessionHelper.EnterpriseLogoUrl));
                    }
                    dictionary.Add("CompanyLogoURL", baseURL + ConvertImageToPNG(eTurnsAppConfig.BaseFileSharedPath, "Uploads\\CompanyLogos\\" + SessionHelper.CompanyID + "\\" + SessionHelper.CompanyLogoUrl));
                    dictionary.Add("BarcodeURL", baseURL + "/Barcode/GetBarcodeByKey?barcodekey=");
                    dictionary.Add("WOSignatureURL", baseURL + "/Uploads/WorkOrderSignature/" + SessionHelper.CompanyID + "/");
                    dictionary.Add("WOAttachmentPath", baseURL + "/Uploads/WorkOrderFile/" + SessionHelper.EnterPriceID + "/" + SessionHelper.CompanyID + "/" + SessionHelper.RoomID + "/");
                }
                else
                {
                    if (objCommonDAL.HasSpecialDomain(SessionHelper.CurrentDomainURL, SessionHelper.EnterPriceID))
                    {
                        dictionary.Add("eTurnsLogoURL", "https://localhost:" + System.Web.HttpContext.Current.Request.Url.Port + ConvertImageToPNG(BasePath, "Uploads\\EnterpriseLogos\\" + SessionHelper.EnterPriceID + "\\" + SessionHelper.EnterpriseLogoUrl));
                        dictionary.Add("EnterpriseLogoURL", "https://localhost:" + "/Content/OpenAccess/NologoReport.png");
                    }
                    else
                    {
                        dictionary.Add("eTurnsLogoURL", "https://localhost:" + System.Web.HttpContext.Current.Request.Url.Port + "/Content/OpenAccess/logoInReport.png");
                        dictionary.Add("EnterpriseLogoURL", "https://localhost:" + System.Web.HttpContext.Current.Request.Url.Port + ConvertImageToPNG(BasePath, "Uploads\\EnterpriseLogos\\" + SessionHelper.EnterPriceID + "\\" + SessionHelper.EnterpriseLogoUrl));
                    }
                    dictionary.Add("CompanyLogoURL", "http://localhost:" + System.Web.HttpContext.Current.Request.Url.Port + ConvertImageToPNG(BasePath, "Uploads\\CompanyLogos\\" + SessionHelper.CompanyID + "\\" + SessionHelper.CompanyLogoUrl));
                    dictionary.Add("BarcodeURL", "http://localhost:" + System.Web.HttpContext.Current.Request.Url.Port + "/Barcode/GetBarcodeByKey?barcodekey=");
                    dictionary.Add("WOSignatureURL", "http://localhost:" + System.Web.HttpContext.Current.Request.Url.Port + "/Uploads/WorkOrderSignature/" + SessionHelper.CompanyID + "/");
                    dictionary.Add("WOAttachmentPath", "http://localhost:" + System.Web.HttpContext.Current.Request.Url.Port + "/Uploads/WorkOrderFile/" + SessionHelper.EnterPriceID + "/" + SessionHelper.CompanyID + "/" + SessionHelper.RoomID + "/");
                }


                dictionary.Add("UserID", SessionHelper.UserID.ToString());
                if (objNotificationDTO != null)
                {
                    if (!string.IsNullOrEmpty(objNotificationDTO.SupplierIds))
                        dictionary.Add("SupplierIDs", objNotificationDTO.SupplierIds);
                    else
                        dictionary.Add("SupplierIDs", string.Empty);
                }
            }
            if (paras != null && paras.Count > 0)
            {
                foreach (var item in paras)
                {
                    if (item.key.ToLower() == "startdate")
                    {
                        dictionary.Add("OrigStartDate", DateTime.ParseExact(item.value, (SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).ToString("yyyy-MM-dd HH:mm:ss"));
                        item.value = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(item.value, (SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult), SessionHelper.CurrentTimeZone).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    if (item.key.ToLower() == "enddate")
                    {
                        dictionary.Add("OrigEndDate", DateTime.ParseExact(item.value, (SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399).ToString("yyyy-MM-dd HH:mm:ss"));
                        item.value = TimeZoneInfo.ConvertTimeToUtc(DateTime.ParseExact(item.value, (SessionHelper.RoomDateFormat), ResourceHelper.CurrentCult).AddSeconds(86399), SessionHelper.CurrentTimeZone).ToString("yyyy-MM-dd HH:mm:ss");
                    }

                    dictionary.Add(item.key, item.value);
                }

            }

            SessionHelper.Add("ReportPara", dictionary);

            return Json(new { message = "ok" }, JsonRequestBehavior.AllowGet);

        }

        public string ConvertImageToPNG(string BasePath, string InnerPath)
        {
            string returnImagePath = string.Empty;
            System.Drawing.Image bmpImageToConvert = null;
            System.Drawing.Image bmpNewImage = null;
            Graphics gfxNewImage = null;
            string[] arrPath = null;
            try
            {
                string path = BasePath + InnerPath;
                if (!string.IsNullOrEmpty(InnerPath))
                {
                    arrPath = InnerPath.Split(new string[1] { "\\" }, StringSplitOptions.RemoveEmptyEntries);

                    if (arrPath != null && arrPath.Length > 0)
                    {
                        if (arrPath[arrPath.Length - 1].ToLower().Contains(".png"))
                        {
                            returnImagePath = InnerPath;
                        }
                        else
                        {
                            string strNewFileName = arrPath[arrPath.Length - 1];
                            if (strNewFileName.LastIndexOf(".") > 0)
                            {
                                strNewFileName = strNewFileName.Substring(0, strNewFileName.LastIndexOf("."));

                                for (int i = 0; i < arrPath.Length - 1; i++)
                                {
                                    if (i > 0)
                                        returnImagePath += "\\";

                                    returnImagePath += arrPath[i];
                                }

                                returnImagePath += "\\" + strNewFileName + ".png";
                            }
                        }
                    }
                }

                if (!System.IO.File.Exists(BasePath + returnImagePath) && System.IO.File.Exists(path))
                {
                    if (path.LastIndexOf(".svg") == (path.Length - 4))
                    {
                        var svgDocument = Svg.SvgDocument.Open(path);
                        using (var smallBitmap = svgDocument.Draw())
                        {
                            var width = smallBitmap.Width;
                            var height = smallBitmap.Height;
                            //if (width != 135)// I resize my bitmap
                            //{
                            //    width = 135;
                            //    height = 135 / smallBitmap.Width * height;
                            //}
                            using (var bitmap = svgDocument.Draw(width, height))//I render again
                            {
                                bitmap.Save(BasePath + returnImagePath, System.Drawing.Imaging.ImageFormat.Png);
                            }
                        }
                    }
                    else
                    {
                        bmpImageToConvert = System.Drawing.Image.FromFile(path);
                        bmpNewImage = new Bitmap(135, 75);
                        gfxNewImage = Graphics.FromImage(bmpNewImage);
                        gfxNewImage.DrawImage(bmpImageToConvert, new System.Drawing.Rectangle(0, 0, bmpNewImage.Width, bmpNewImage.Height), 0, 0, bmpImageToConvert.Width, bmpImageToConvert.Height, GraphicsUnit.Pixel);
                        gfxNewImage.Dispose();
                        bmpImageToConvert.Dispose();

                        bmpNewImage.Save(BasePath + returnImagePath, System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
                else if (!System.IO.File.Exists(BasePath + returnImagePath))
                {
                    string NoImagePath = System.Web.HttpContext.Current.Server.MapPath("\\Uploads\\EnterpriseLogos\\");
                    NoImagePath += "NoEntImage.png";

                    if (!System.IO.File.Exists(NoImagePath))
                    {
                        bmpNewImage = new Bitmap(135, 75);
                        System.Drawing.Font f = new System.Drawing.Font("Verdana", 12);
                        gfxNewImage = Graphics.FromImage(bmpNewImage);
                        gfxNewImage.DrawString(" ", f, Brushes.Black, new RectangleF(0, 0, 135, 75));
                        gfxNewImage.Dispose();
                        bmpNewImage.Save(NoImagePath, System.Drawing.Imaging.ImageFormat.Png);
                    }

                    returnImagePath = NoImagePath;
                }
                return "/" + returnImagePath.Replace(BasePath, "").Replace("\\", "/");
            }
            finally
            {
                returnImagePath = string.Empty;
                bmpImageToConvert = null;
                bmpNewImage = null;
                gfxNewImage = null;
                arrPath = null;

            }
        }

        public byte[] EncodeBase64(string data)
        {
            string s = data.Trim().Replace(" ", "+");
            if (s.Length % 4 > 0)
                s = s.PadRight(s.Length + 4 - s.Length % 4, '=');
            return Convert.FromBase64String(s);
        }

        //public ActionResult RedirectToPage()
        //{
        //    UserSettingDTO objUserSettingDTO = new UserSettingDTO();
        //    eTurns.DAL.UserSettingDAL objUserSettingDAL = new eTurns.DAL.UserSettingDAL(SessionHelper.EnterPriseDBName);
        //    objUserSettingDTO = objUserSettingDAL.GetByUserId(SessionHelper.UserID);
        //    return PartialView("_RedirectToPage", objUserSettingDTO);
        //}
        public ActionResult ListPageSetting()
        {
            var isEnterpriseGridColumnSetup = SessionHelper.GetAdminPermission(SessionHelper.ModuleList.EnterpriseGridColumnSetup);

            if (SessionHelper.RoleID < 0 && SessionHelper.UserType < 3 && isEnterpriseGridColumnSetup)
            {
                IEnumerable<SiteListMasterDTO> objSiteListMasterDTO;
                //        SiteListMasterDAL objSiteListMasterDAL = new SiteListMasterDAL(eTurnsMaster.DAL.MasterDbConnectionHelper.GeteTurnsDBName());
                //        objSiteListMasterDTO = objSiteListMasterDAL.GetAllItems();
                //        ViewBag.ListPageName = (IEnumerable<SiteListMasterDTO>)objSiteListMasterDTO.OrderBy(o => o.ListName);
                //        ViewBag.FirstList = objSiteListMasterDTO.OrderBy(o => o.ListName).FirstOrDefault().ID;
                //        return View();
                //    }
                //    else
                //    {
                //        return RedirectToAction(ActName, CtrlName);
                //    }
                //}
                //public ActionResult ListPageSettingEnterPrise()
                //{
                //    if (SessionHelper.RoleID == -1)
                //    {
                //        IEnumerable<SiteListMasterDTO> objSiteListMasterDTO;
                SiteListMasterDAL objSiteListMasterDAL = new SiteListMasterDAL(SessionHelper.EnterPriseDBName);
                objSiteListMasterDTO = objSiteListMasterDAL.GetAllItems(SiteSettingHelper.UsersUISettingType);
                ViewBag.ListPageName = (IEnumerable<SiteListMasterDTO>)objSiteListMasterDTO.OrderBy(o => o.ListName);
                ViewBag.FirstList = objSiteListMasterDTO.OrderBy(o => o.ListName).FirstOrDefault().ID;
                return View();
            }
            else
            {
                return RedirectToAction(ActName, CtrlName);
            }
        }

        public ActionResult eTurnsListPageSetting()
        {
            if (SessionHelper.RoleID < 0 && SessionHelper.UserType == 1)
            {
                IEnumerable<SiteListMasterDTO> objSiteListMasterDTO;
                SiteListMasterDAL objSiteListMasterDAL = new SiteListMasterDAL(MasterDbConnectionHelper.GeteTurnsDBName());
                objSiteListMasterDTO = objSiteListMasterDAL.GetAllItems(SiteSettingHelper.UsersUISettingType);
                ViewBag.ListPageName = (IEnumerable<SiteListMasterDTO>)objSiteListMasterDTO.OrderBy(o => o.ListName);
                ViewBag.FirstList = objSiteListMasterDTO.OrderBy(o => o.ListName).FirstOrDefault().ID;
                return View();
            }
            else
            {
                return RedirectToAction(ActName, CtrlName);
            }
        }

        public ActionResult eTurnsListPageSettingUpdate()
        {
            if (SessionHelper.RoleID < 0 && SessionHelper.UserType == 1)
            {
                IEnumerable<SiteListMasterDTO> objSiteListMasterDTO;
                SiteListMasterDAL objSiteListMasterDAL = new SiteListMasterDAL(MasterDbConnectionHelper.GeteTurnsDBName());
                objSiteListMasterDTO = objSiteListMasterDAL.GetAllItems(SiteSettingHelper.UsersUISettingType);
                ViewBag.ListPageName = (IEnumerable<SiteListMasterDTO>)objSiteListMasterDTO.OrderBy(o => o.ListName);
                return View();
            }
            else
            {
                return RedirectToAction(ActName, CtrlName);
            }
        }

        public JsonResult GetSiteListColumns(long ListId)
        {
            IEnumerable<SiteListColumnDetailDTO> objSiteListColumnDetailDTO;
            SiteListColumnDetailDAL objSiteListColumnDetailDAL = new SiteListColumnDetailDAL(MasterDbConnectionHelper.GeteTurnsDBName());
            objSiteListColumnDetailDTO = objSiteListColumnDetailDAL.GetAllItemsByListId(ListId, SiteSettingHelper.UsersUISettingType);
            return Json(new { objSiteListColumnDetailDTO = objSiteListColumnDetailDTO }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BindColumns(Int64 currentPageListId, bool isForeTurns)
        {
            bool IsAccess = (isForeTurns) ? (SessionHelper.RoleID < 0 && SessionHelper.UserType == 1) : (SessionHelper.RoleID < 0 && SessionHelper.UserType < 3);

            //if (SessionHelper.RoleID == -1)
            if (IsAccess)
            {
                string ColumnName = string.Empty;
                string SortType = string.Empty;
                int Pagesize = 10;
                IEnumerable<SiteListColumnDetailDTO> objSiteListColumnDetailDTO;
                SiteListColumnDetailDAL objSiteListColumnDetailDAL = new SiteListColumnDetailDAL(isForeTurns ? MasterDbConnectionHelper.GeteTurnsDBName() : SessionHelper.EnterPriseDBName);
                objSiteListColumnDetailDTO = objSiteListColumnDetailDAL.GetAllItemsByListId(currentPageListId, SiteSettingHelper.UsersUISettingType);

                SiteListMasterDTO objSiteListMasterDTO = new SiteListMasterDTO();
                SiteListMasterDAL objSiteListMasterDAL = new SiteListMasterDAL(isForeTurns ? MasterDbConnectionHelper.GeteTurnsDBName() : SessionHelper.EnterPriseDBName);
                objSiteListMasterDTO = objSiteListMasterDAL.GetAllItemsById(currentPageListId, SiteSettingHelper.UsersUISettingType);

                ViewBag.ColumnList = (IEnumerable<SiteListColumnDetailDTO>)objSiteListColumnDetailDTO;
                string LastOrderSequnce = string.Empty;
                string VisibilityOrder = string.Empty;
                string Columnsizes = string.Empty;
                string ListBreadCrumb = string.Empty;
                if (objSiteListMasterDTO != null && !string.IsNullOrEmpty(objSiteListMasterDTO.ListDetails) && !string.IsNullOrWhiteSpace(objSiteListMasterDTO.ListDetails))
                {
                    ListBreadCrumb = objSiteListMasterDTO.ListDetails;
                }
                if (objSiteListMasterDTO != null && objSiteListMasterDTO.JSONDATA != null && (!string.IsNullOrEmpty(Convert.ToString(objSiteListMasterDTO.JSONDATA))))
                {

                    JQueryTableJSONDTO myDeserializedObjList = (JQueryTableJSONDTO)Newtonsoft.Json.JsonConvert.DeserializeObject(objSiteListMasterDTO.JSONDATA, typeof(JQueryTableJSONDTO));
                    for (int i = 0; i < objSiteListColumnDetailDTO.Count(); i++)
                    {
                        //objSiteListColumnDetailDTO.ToList()[i].ActualColumnName = objSiteListColumnDetailDTO.ToList()[i].ColumnName;
                        objSiteListColumnDetailDTO.ToList()[i].ColumnName = getResourceName(Convert.ToString(objSiteListColumnDetailDTO.ToList()[i].ResourceFileName), Convert.ToString(objSiteListColumnDetailDTO.ToList()[i].ColumnName));
                        int tmpLastOrder = myDeserializedObjList.ColReorder[i];
                        objSiteListColumnDetailDTO.ToList()[i].Visibility = myDeserializedObjList.abVisCols[tmpLastOrder];
                        objSiteListColumnDetailDTO.ToList()[i].LastOrder = tmpLastOrder;
                        if (!string.IsNullOrEmpty(LastOrderSequnce))
                        {
                            LastOrderSequnce = LastOrderSequnce + "," + Convert.ToString(objSiteListColumnDetailDTO.ToList()[i].LastOrder);
                        }
                        else
                        {
                            LastOrderSequnce = Convert.ToString(objSiteListColumnDetailDTO.ToList()[i].LastOrder);
                        }
                        if (!string.IsNullOrEmpty(VisibilityOrder))
                        {
                            VisibilityOrder = VisibilityOrder + "," + Convert.ToString(objSiteListColumnDetailDTO.ToList()[i].Visibility);
                        }
                        else
                        {
                            VisibilityOrder = Convert.ToString(objSiteListColumnDetailDTO.ToList()[i].Visibility);
                        }
                        if (!string.IsNullOrEmpty(Columnsizes))
                        {
                            Columnsizes = Columnsizes + "," + Convert.ToString(myDeserializedObjList.ColWidth[i]).Replace("px", "");
                        }
                        else
                        {
                            Columnsizes = Convert.ToString(myDeserializedObjList.ColWidth[i]).Replace("px", "");
                        }
                    }
                    IEnumerable<SiteListColumnDetailDTO> columndetailData = objSiteListColumnDetailDTO;
                    if (myDeserializedObjList.aaSorting.Count > 0)
                    {
                        ColumnName = Convert.ToString(myDeserializedObjList.aaSorting[0][3]);
                        SortType = Convert.ToString(myDeserializedObjList.aaSorting[0][1]);
                        Pagesize = Convert.ToInt32(myDeserializedObjList.iLength);
                        var stringList = myDeserializedObjList.aaSorting.OfType<object>();
                    }
                }
                else
                {
                    for (int i = 0; i < objSiteListColumnDetailDTO.Count(); i++)
                    {
                        //objSiteListColumnDetailDTO.ToList()[i].ActualColumnName = objSiteListColumnDetailDTO.ToList()[i].ColumnName;
                        objSiteListColumnDetailDTO.ToList()[i].ColumnName = getResourceName(Convert.ToString(objSiteListColumnDetailDTO.ToList()[i].ResourceFileName), Convert.ToString(objSiteListColumnDetailDTO.ToList()[i].ColumnName));

                    }
                }
                if (!string.IsNullOrEmpty(LastOrderSequnce))
                {
                    string[] OrderSequnce = LastOrderSequnce.Split(',');
                    string[] Visibilitysequnce = VisibilityOrder.Split(',');
                    string[] Columnsize = Columnsizes.Split(',');
                    List<SiteListColumnDetailDTO> list = objSiteListColumnDetailDTO.ToList();
                    for (int i = 0; i < OrderSequnce.Length; i++)
                    {
                        int currentOrder = Convert.ToInt32(OrderSequnce[i]);
                        var index = list.FindIndex(x => x.OrderNumber == currentOrder);

                        if (index >= 0 && index <= (list.Count - 1))
                        {
                            var item = list[index];
                            list[index] = list[i];
                            list[i] = item;
                            list[i].Visibility = Convert.ToBoolean(Visibilitysequnce[i]);//Convert.ToBoolean(Visibilitysequnce[list[i].OrderNumber.GetValueOrDefault(0)]);
                            list[i].ColumnSize = Convert.ToDouble(Columnsize[i]);
                        }
                    }
                    objSiteListColumnDetailDTO = list;
                }
                else
                {
                    foreach (SiteListColumnDetailDTO sitelist in objSiteListColumnDetailDTO)
                    {
                        sitelist.Visibility = true;
                        sitelist.ColumnSize = 100.00;
                    }
                }

                return Json(new { objSiteListColumnDetailDTO = objSiteListColumnDetailDTO, ColumnName = ColumnName, SortType = SortType, Pagesize = Pagesize, ListBreadCrumb = ListBreadCrumb }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }

        public ActionResult OverrideRoomsGridSetUpWithCurrentRoom()
        {
            eTurns.DAL.UsersUISettingsDAL userUISettingsDAL = new eTurns.DAL.UsersUISettingsDAL(DbConnectionHelper.GeteTurnsLoggingDBName());
            userUISettingsDAL.InsertOverrideRoomsGridSetUpRequest(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, SessionHelper.RoleID, SessionHelper.UserType, SiteSettingHelper.UsersUISettingType);

            //eTurnsMaster.DAL.UsersUISettingsDAL usersUISettingsDAL = new eTurnsMaster.DAL.UsersUISettingsDAL();
            //eTurnsMaster.DAL.UserMasterDAL obj = new eTurnsMaster.DAL.UserMasterDAL();
            //var userUISettingsForCurrentRoom = usersUISettingsDAL.GetUserUISettingForCurrentRoom(SessionHelper.UserID, SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID);
            //eTurns.DAL.UserMasterDAL objUserMasterDAL = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
            //List<UserAccessDTO> userAccessList = new List<UserAccessDTO>();

            //if (SessionHelper.UserType == 1 && SessionHelper.RoleID < 0)
            //{
            //    userAccessList = obj.GetUserRoomAccessForSuperAdmin(); //Note: select * from mstroom where isdeleted = 0
            //}
            //else if (SessionHelper.UserType == 2 && SessionHelper.RoleID < 0)
            //{
            //    userAccessList = obj.GetUserRoomAccessForEnterpriseAdmin(SessionHelper.EnterPriceID); //Note: select * from mstroom where isdeleted = 0 and enterpriseid = SessionHelper.enterpriseId
            //}
            //else
            //{
            //    userAccessList = (SessionHelper.UserType != 1)
            //    ? objUserMasterDAL.GetUserAccessWithNames(SessionHelper.UserID)
            //    : obj.GetUserRoomAccessByUserIdPlain(SessionHelper.UserID);
            //}

            //if (userAccessList.Any())
            //{
            //    var currentRoomUserAccess = userAccessList.Where(e => e.EnterpriseId == SessionHelper.EnterPriceID && e.CompanyId == SessionHelper.CompanyID && e.RoomId == SessionHelper.RoomID).FirstOrDefault();
            //    if (currentRoomUserAccess != null && currentRoomUserAccess.ID > 0)
            //    {
            //        int currentRoomValue = userAccessList.IndexOf(currentRoomUserAccess);
            //        userAccessList.RemoveAt(currentRoomValue);
            //    }
            //}
            //EnterpriseMasterDAL enterPriseMasterDAL = new EnterpriseMasterDAL();

            //List<long> enterpriseIds = new List<long>();
            //enterpriseIds = userAccessList.Select(e => e.EnterpriseId).ToList();

            //Dictionary<long, string> enterpriseList = enterPriseMasterDAL.GetEnterpriseListWithDBName(enterpriseIds);

            //eTurns.DAL.UDFDAL currentRoomUDFDAL = new eTurns.DAL.UDFDAL(SessionHelper.EnterPriseDBName);
            //var CurrentRoomUDFs = currentRoomUDFDAL.GetAllUDFTableNameByRoom(SessionHelper.CompanyID, SessionHelper.RoomID);

            //foreach (var userAccess in userAccessList)
            //{
            //    string updatingRoomDBName = enterpriseList.ContainsKey(userAccess.EnterpriseId) ? enterpriseList[userAccess.EnterpriseId] : string.Empty;

            //    if (string.IsNullOrEmpty(updatingRoomDBName))
            //        continue;

            //    eTurns.DAL.UDFDAL updatingRoomUDFDAL = new eTurns.DAL.UDFDAL(updatingRoomDBName);
            //    var updatingRoomUDFs = updatingRoomUDFDAL.GetAllUDFTableNameByRoom(userAccess.CompanyId, userAccess.RoomId);

            //    foreach (var setting in userUISettingsForCurrentRoom)
            //    {
            //        string jsonData = string.Empty;

            //        if (!string.IsNullOrEmpty(setting.JSONDATA))
            //        {
            //            jsonData = setting.JSONDATA;
            //            var tmpList = setting.ListName;

            //            if (tmpList.ToLower() == "toollistnew")
            //            {
            //                tmpList = "ToolList";
            //            }

            //            if (tmpList == "PullMaster" || tmpList == "ItemsListForNewReceive" || tmpList == "ToolHistoryList"
            //                || tmpList == "AssetToolSchedulerList" || tmpList == "KitToolMasterList" || tmpList == "EnterpriseList"
            //                || tmpList == "BomCategoryMasterList" || tmpList == "BomGLAccountMasterList"
            //                || tmpList == "BomInventoryClassificationMasterList"
            //                || tmpList == "BomManufacturerMasterList" || tmpList == "BomSupplierMasterList"
            //                || tmpList == "UnitMasterList" || tmpList == "BomUnitMasterList" || tmpList == "MinMaxTuningTable")
            //            {
            //                //IEnumerable<UDFDTO> DataFromDB = null;
            //                //IEnumerable<UDFDTO> updatingRoomDataFromDB = null;
            //                //int tmpTotalRecordCount = 0;
            //                var udfTables = MasterBAL.GetUDFTableNamesByListName(tmpList);
            //                int currentRoomUDFCount = 0;
            //                int updatingRoomUDFCount = 0;

            //                foreach (var tableName in udfTables)
            //                {
            //                    var tmpUDFTableName = !string.IsNullOrEmpty(tableName) ? tableName.ToLower() : string.Empty;

            //                    if (tmpUDFTableName.ToLower() == "companymaster" || tmpUDFTableName.ToLower() == "room" || tmpUDFTableName.ToLower() == "usermaster"
            //                         || tmpUDFTableName.ToLower() == "bomitemmaster" || tmpUDFTableName.ToLower() == "itemcountlist")
            //                    {

            //                        currentRoomUDFCount += currentRoomUDFDAL.GetNonDeletedUDFCountByUDFTableName(tableName, SessionHelper.RoomID, SessionHelper.CompanyID);
            //                        //DataFromDB = currentRoomUDFDAL.GetPagedRecords(0, 10, out tmpTotalRecordCount, string.Empty, "ID asc", SessionHelper.CompanyID, tableName, SessionHelper.RoomID);

            //                        //if (DataFromDB != null && DataFromDB.Count() > 0)
            //                        //{
            //                        //    currentRoomUDFCount += DataFromDB.Where(c => c.UDFControlType != null && c.IsDeleted == false).Count();
            //                        //}

            //                        updatingRoomUDFCount += updatingRoomUDFDAL.GetNonDeletedUDFCountByUDFTableName(tableName, userAccess.RoomId, userAccess.CompanyId);
            //                        //updatingRoomDataFromDB = updatingRoomUDFDAL.GetPagedRecords(0, 10, out tmpTotalRecordCount, string.Empty, "ID asc", userAccess.CompanyId, tableName, userAccess.RoomId);

            //                        //if (updatingRoomDataFromDB != null && updatingRoomDataFromDB.Count() > 0)
            //                        //{
            //                        //    updatingRoomUDFCount += updatingRoomDataFromDB.Where(c => c.UDFControlType != null && c.IsDeleted == false).Count();
            //                        //}
            //                    }
            //                    else
            //                    {
            //                        currentRoomUDFCount += CurrentRoomUDFs.Where(e => e.UDFTableName.ToLower() == tmpUDFTableName).Count();
            //                        updatingRoomUDFCount += updatingRoomUDFs.Where(e => e.UDFTableName.ToLower() == tmpUDFTableName).Count();
            //                    }
            //                }

            //                if ((currentRoomUDFCount != updatingRoomUDFCount) && (currentRoomUDFCount > 0 || updatingRoomUDFCount > 0))
            //                {
            //                    jsonData = UpdateJsonForRoom(jsonData, setting.ListName, currentRoomUDFCount, updatingRoomUDFCount, userAccess.EnterpriseId, userAccess.CompanyId, userAccess.RoomId);
            //                }
            //                else
            //                {
            //                    var usersUISettings = new UsersUISettingsDTO();
            //                    usersUISettings.UserID = SessionHelper.UserID;
            //                    usersUISettings.EnterpriseID = userAccess.EnterpriseId;
            //                    usersUISettings.CompanyID = userAccess.CompanyId;
            //                    usersUISettings.RoomID = userAccess.RoomId;
            //                    usersUISettings.JSONDATA = jsonData;
            //                    usersUISettings.ListName = setting.ListName;
            //                    usersUISettingsDAL.SaveUserListViewSettings(usersUISettings, SiteSettingHelper.UsersUISettingType, true);
            //                }
            //            }
            //            else
            //            {
            //                eTurnsMaster.DAL.CommonMasterDAL objUDF = new eTurnsMaster.DAL.CommonMasterDAL();
            //                Dictionary<int, string> GridListName = objUDF.GetUDfTableNameByListName(tmpList);
            //                int currentRoomTotalUDFCount = 0;
            //                int updatingRoomTotalUDFCount = 0;

            //                if (GridListName != null && GridListName.Count() > 0)
            //                {
            //                    foreach (KeyValuePair<int, string> ReportResourceFileName in GridListName)
            //                    {
            //                        string[] Values = ReportResourceFileName.Value.Split('$');
            //                        if (Values != null && Values.Count() > 0)
            //                        {
            //                            int currentRoomUDFCount = 0;
            //                            int updatingRoomUDFCount = 0;
            //                            //IEnumerable<UDFDTO> UDFDataFromDB = null;
            //                            //IEnumerable<UDFDTO> UpdateRoomUDFDataFromDB = null;
            //                            //int TotalRecordCount = 0;
            //                            var tmpUDFTableName = !string.IsNullOrEmpty(Values[0]) ? Values[0].ToLower() : string.Empty;

            //                            if (tmpUDFTableName.ToLower() == "companymaster" || tmpUDFTableName.ToLower() == "room" || tmpUDFTableName.ToLower() == "usermaster"
            //                                 || tmpUDFTableName.ToLower() == "bomitemmaster" || tmpUDFTableName.ToLower() == "itemcountlist")
            //                            {
            //                                currentRoomUDFCount = currentRoomUDFDAL.GetNonDeletedUDFCountByUDFTableName(Values[0], SessionHelper.RoomID, SessionHelper.CompanyID);
            //                                //UDFDataFromDB = currentRoomUDFDAL.GetPagedRecords(0, 10, out TotalRecordCount, string.Empty, "ID asc", SessionHelper.CompanyID, Values[0], SessionHelper.RoomID);

            //                                //if (UDFDataFromDB != null && UDFDataFromDB.Count() > 0)
            //                                //{
            //                                //    currentRoomUDFCount = UDFDataFromDB.Where(c => c.UDFControlType != null && c.IsDeleted == false).Count();
            //                                //}

            //                                updatingRoomUDFCount = updatingRoomUDFDAL.GetNonDeletedUDFCountByUDFTableName(Values[0], userAccess.RoomId, userAccess.CompanyId);
            //                                //UpdateRoomUDFDataFromDB = updatingRoomUDFDAL.GetPagedRecords(0, 10, out TotalRecordCount, string.Empty, "ID asc", userAccess.CompanyId, Values[0], userAccess.RoomId);

            //                                //if (UpdateRoomUDFDataFromDB != null && UpdateRoomUDFDataFromDB.Count() > 0)
            //                                //{
            //                                //    updatingRoomUDFCount = UpdateRoomUDFDataFromDB.Where(c => c.UDFControlType != null && c.IsDeleted == false).Count();
            //                                //}
            //                            }
            //                            else
            //                            {
            //                                currentRoomUDFCount = CurrentRoomUDFs.Where(e => e.UDFTableName.ToLower() == tmpUDFTableName).Count();
            //                                updatingRoomUDFCount = updatingRoomUDFs.Where(e => e.UDFTableName.ToLower() == tmpUDFTableName).Count();
            //                            }

            //                            if (tmpList == "CartItemList" || tmpList == "ReceiveMasterList")
            //                            {
            //                                Values[1] = "Yes";
            //                                Values[2] = "ItemMaster";
            //                            }

            //                            int ExtraUDFinCurrentRoomGrid = 0;
            //                            int ExtraUDFinUpdatingRoomGrid = 0;

            //                            if (Values[1] == "Yes")
            //                            {
            //                                //IEnumerable<UDFDTO> DataFromDB = null;
            //                                //IEnumerable<UDFDTO> updatingRoomDataFromDB = null;
            //                                //int TotalRecordCountExtra = 0;
            //                                var tmpExtraUDFTableName = !string.IsNullOrEmpty(Values[2]) ? Values[2].ToLower() : string.Empty;
            //                                //int TotalRecordCountExtraForUpdatingRoom = 0;

            //                                if (tmpExtraUDFTableName.ToLower() == "companymaster" || tmpExtraUDFTableName.ToLower() == "room" || tmpExtraUDFTableName.ToLower() == "usermaster"
            //                                     || tmpExtraUDFTableName.ToLower() == "bomitemmaster" || tmpExtraUDFTableName.ToLower() == "itemcountlist")
            //                                {
            //                                    ExtraUDFinCurrentRoomGrid = currentRoomUDFDAL.GetNonDeletedUDFCountByUDFTableName(Values[2], SessionHelper.RoomID, SessionHelper.CompanyID);
            //                                    //DataFromDB = currentRoomUDFDAL.GetPagedRecords(0, 10, out TotalRecordCountExtra, string.Empty, "ID asc", SessionHelper.CompanyID, Values[2], SessionHelper.RoomID);
            //                                    //if (DataFromDB != null && DataFromDB.Count() > 0)
            //                                    //{
            //                                    //    ExtraUDFinCurrentRoomGrid = DataFromDB.Where(c => c.UDFControlType != null && c.IsDeleted == false).Count();
            //                                    //}

            //                                    ExtraUDFinUpdatingRoomGrid = updatingRoomUDFDAL.GetNonDeletedUDFCountByUDFTableName(Values[2], userAccess.RoomId, userAccess.CompanyId);
            //                                    //updatingRoomDataFromDB = updatingRoomUDFDAL.GetPagedRecords(0, 10, out TotalRecordCountExtraForUpdatingRoom, string.Empty, "ID asc", userAccess.CompanyId, Values[2], userAccess.RoomId);

            //                                    //if (updatingRoomDataFromDB != null && updatingRoomDataFromDB.Count() > 0)
            //                                    //{
            //                                    //    ExtraUDFinUpdatingRoomGrid = updatingRoomDataFromDB.Where(c => c.UDFControlType != null && c.IsDeleted == false).Count();
            //                                    //}
            //                                }
            //                                else
            //                                {
            //                                    ExtraUDFinCurrentRoomGrid = CurrentRoomUDFs.Where(e => e.UDFTableName.ToLower() == tmpExtraUDFTableName).Count();
            //                                    ExtraUDFinUpdatingRoomGrid = updatingRoomUDFs.Where(e => e.UDFTableName.ToLower() == tmpExtraUDFTableName).Count();
            //                                }
            //                            }
            //                            currentRoomTotalUDFCount += currentRoomUDFCount + ExtraUDFinCurrentRoomGrid + Convert.ToInt32(Values[3]);
            //                            updatingRoomTotalUDFCount += updatingRoomUDFCount + ExtraUDFinUpdatingRoomGrid + Convert.ToInt32(Values[3]);

            //                            if ((currentRoomTotalUDFCount != updatingRoomTotalUDFCount) && (currentRoomTotalUDFCount > 0 || updatingRoomTotalUDFCount > 0))
            //                            {
            //                                jsonData = UpdateJsonForRoom(jsonData, setting.ListName, currentRoomTotalUDFCount, updatingRoomTotalUDFCount, userAccess.EnterpriseId, userAccess.CompanyId, userAccess.RoomId);
            //                            }
            //                            else
            //                            {
            //                                var usersUISettings = new UsersUISettingsDTO();
            //                                usersUISettings.UserID = SessionHelper.UserID;
            //                                usersUISettings.EnterpriseID = userAccess.EnterpriseId;
            //                                usersUISettings.CompanyID = userAccess.CompanyId;
            //                                usersUISettings.RoomID = userAccess.RoomId;
            //                                usersUISettings.JSONDATA = jsonData;
            //                                usersUISettings.ListName = setting.ListName;
            //                                usersUISettingsDAL.SaveUserListViewSettings(usersUISettings, SiteSettingHelper.UsersUISettingType, true);
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        //public string getResourceName(string ResourceName, string resourceKey)
        //{
        //    if (!string.IsNullOrEmpty(ResourceName))
        //    {
        //        Assembly SampleAssembly;
        //        SampleAssembly = Assembly.LoadFrom(Server.MapPath("~\\bin\\eTurns.DTO.dll"));
        //        int index = SampleAssembly.GetTypes().ToList().FindIndex(s => s.Name == ResourceName);
        //        return Convert.ToString(SampleAssembly.GetTypes()[index].GetProperty(resourceKey).GetValue(SampleAssembly, null));
        //    }
        //    else
        //    {
        //        return resourceKey;
        //    }

        //}
        public JsonResult CheckbinExists(string binid, Guid itemguid)
        {
            string result = string.Empty;
            try
            {
                var count = 0;
                List<OrderDetailsDTO> lstDetails = (List<OrderDetailsDTO>)SessionHelper.Get("OrderLineItem_" + SessionHelper.EnterPriceID + "_" + SessionHelper.CompanyID + "_" + SessionHelper.RoomID);
                if (lstDetails != null && lstDetails.Count > 0)
                {
                    count = lstDetails.Where(a => a.Bin == Convert.ToDecimal(binid) && a.ItemGUID == itemguid).Count();
                }
                if (count > 0)
                {
                    result = string.Empty;
                }
                else
                {
                    result = "true";
                }
            }
            catch
            {
                result = string.Empty;
            }

            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }
        public bool SaveJsonData(string sortelist, string visible, int pageSize, int currentPageListId, string SortColumn, string Sorttype, string colsizelist, int SortColumnIndex, bool isForeTurns)
        {
            if (SessionHelper.RoleID == -1 || SessionHelper.RoleID == -2)
            {

                IEnumerable<SiteListColumnDetailDTO> objSiteListColumnDetailDTO;
                SiteListColumnDetailDAL objSiteListColumnDetailDAL = new SiteListColumnDetailDAL(isForeTurns ? MasterDbConnectionHelper.GeteTurnsDBName() : SessionHelper.EnterPriseDBName);
                objSiteListColumnDetailDTO = objSiteListColumnDetailDAL.GetAllItemsByListId(currentPageListId, SiteSettingHelper.UsersUISettingType);

                SiteListMasterDTO objSiteListMasterDTO = new SiteListMasterDTO();
                SiteListMasterDAL objSiteListMasterDAL = new SiteListMasterDAL(isForeTurns ? MasterDbConnectionHelper.GeteTurnsDBName() : SessionHelper.EnterPriseDBName);
                objSiteListMasterDTO = objSiteListMasterDAL.GetAllItemsById(currentPageListId, SiteSettingHelper.UsersUISettingType);
                if (objSiteListMasterDTO != null && objSiteListMasterDTO.JSONDATA != null && (!string.IsNullOrEmpty(Convert.ToString(objSiteListMasterDTO.JSONDATA))))
                {
                    JQueryTableJSONDTO myDeserializedObjList = (JQueryTableJSONDTO)Newtonsoft.Json.JsonConvert.DeserializeObject(objSiteListMasterDTO.JSONDATA, typeof(JQueryTableJSONDTO));
                    string[] VisibleData = visible.Split(',');
                    string[] SortData = sortelist.Split(',');
                    string[] ColumnSize = colsizelist.Split(',');

                    for (int i = 0; i < objSiteListColumnDetailDTO.Count(); i++)
                    {
                        myDeserializedObjList.abVisCols[i] = Convert.ToBoolean(VisibleData[i]); //Convert.ToBoolean(VisibleData[i]);
                        myDeserializedObjList.ColReorder[i] = Convert.ToInt32(SortData[i]);
                        myDeserializedObjList.ColWidth[i] = Convert.ToString(ColumnSize[i]) + "px";

                    }
                    var sortTypeValue = (Sorttype.ToLower() == "asc" ? 1 : (Sorttype.ToLower() == "desc" ? 0 : 2));
                    if (myDeserializedObjList.aaSorting.Count > 0)
                    {
                        myDeserializedObjList.aaSorting[0][0] = SortColumnIndex;
                        myDeserializedObjList.aaSorting[0][1] = Sorttype;
                        myDeserializedObjList.aaSorting[0][2] = sortTypeValue;
                        myDeserializedObjList.aaSorting[0][3] = SortColumn;
                    }
                    else
                    {
                        List<object> aaSorting = new List<object>();
                        aaSorting.Add(SortColumnIndex);
                        aaSorting.Add(Sorttype);
                        aaSorting.Add((Sorttype.ToLower() == "asc" ? 1 : (Sorttype.ToLower() == "desc" ? 0 : 2)));
                        aaSorting.Add(SortColumn);
                        myDeserializedObjList.aaSorting = new List<List<object>>();
                        myDeserializedObjList.aaSorting.Add(aaSorting);
                    }

                    myDeserializedObjList.iLength = Convert.ToInt32(pageSize);
                    string JSONData = Newtonsoft.Json.JsonConvert.SerializeObject(myDeserializedObjList);

                    objSiteListMasterDAL.UpdateJSONData(currentPageListId, JSONData, SiteSettingHelper.UsersUISettingType);
                    eTurns.DAL.CacheHelper<SiteListMasterDTO>.InvalidateCache();
                    HttpRuntime.Cache.Remove("SiteList_" + currentPageListId);
                    return true;
                }
                else
                {
                    JQueryTableJSONDTO myDeserializedObjList = new JQueryTableJSONDTO();
                    Random randomLongValue = new Random();
                    myDeserializedObjList.iCreate = (long)randomLongValue.Next();
                    myDeserializedObjList.iStart = 0;
                    myDeserializedObjList.iEnd = 0;
                    string[] VisibleData = visible.Split(',');
                    string[] SortData = sortelist.Split(',');
                    string[] ColumnSize = colsizelist.Split(',');
                    List<bool> listOfVisility = new List<bool>();
                    List<int> listOfOrder = new List<int>();
                    List<AoSearchCol> aoSearchObj = new List<AoSearchCol>();
                    List<string> ListOfWidth = new List<string>();
                    for (int i = 0; i < objSiteListColumnDetailDTO.Count(); i++)
                    {
                        bool visCol = Convert.ToBoolean(VisibleData[i]);
                        listOfVisility.Add(visCol);
                        int ColReorder = Convert.ToInt32(SortData[i]);
                        listOfOrder.Add(ColReorder);
                        AoSearchCol aoSearchCol = new AoSearchCol();
                        aoSearchCol.bCaseInsensitive = true;
                        aoSearchCol.sSearch = string.Empty;
                        aoSearchCol.bRegex = false;
                        aoSearchCol.bSmart = true;

                        aoSearchObj.Add(aoSearchCol);
                        string columnWidth = ColumnSize[i] + "px";
                        ListOfWidth.Add(columnWidth);
                    }
                    myDeserializedObjList.abVisCols = listOfVisility;
                    myDeserializedObjList.ColReorder = listOfOrder;
                    myDeserializedObjList.aoSearchCols = aoSearchObj;
                    myDeserializedObjList.ColWidth = ListOfWidth;

                    object sorttypeValue = 0;
                    object SortType = Sorttype;
                    object SortColumnValue = 2;
                    object Sortcolumn = SortColumn;
                    List<object> aaSorting = new List<object>();
                    aaSorting.Add(SortColumnIndex);
                    aaSorting.Add(SortType);
                    aaSorting.Add((Sorttype.ToLower() == "asc" ? 1 : (Sorttype.ToLower() == "desc" ? 0 : 2)));
                    aaSorting.Add(Sortcolumn);
                    myDeserializedObjList.aaSorting = new List<List<object>>();
                    myDeserializedObjList.aaSorting.Add(aaSorting);

                    myDeserializedObjList.iLength = Convert.ToInt32(pageSize);

                    OSearch aoSearchColoSearch = new OSearch();
                    aoSearchColoSearch.bCaseInsensitive = true;
                    aoSearchColoSearch.sSearch = string.Empty;
                    aoSearchColoSearch.bRegex = false;
                    aoSearchColoSearch.bSmart = true;

                    myDeserializedObjList.oSearch = aoSearchColoSearch;
                    string JSONData = Newtonsoft.Json.JsonConvert.SerializeObject(myDeserializedObjList);

                    objSiteListMasterDAL.UpdateJSONData(currentPageListId, JSONData, SiteSettingHelper.UsersUISettingType);

                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public JsonResult UpdateJsonOnNewColumnAddition(long ListId, string UpdatedColumnOrder)
        {

            if (ListId > 0 && !string.IsNullOrEmpty(UpdatedColumnOrder))
            {
                EnterpriseMasterDAL enterPriseMasterDAL = new EnterpriseMasterDAL();
                var enterprises = enterPriseMasterDAL.GetAllEnterprisesPlain();

                if (enterprises.Any())
                {
                    enterprises.Add(new EnterpriseDTO { EnterpriseDBName = MasterDbConnectionHelper.GeteTurnsDBName() });
                }
                List<int> columnOrders = UpdatedColumnOrder.Split(',').Select(int.Parse).ToList();

                if (columnOrders.Any())
                {
                    columnOrders.Sort();

                    foreach (var enterprise in enterprises)
                    {
                        if (string.IsNullOrEmpty(enterprise.EnterpriseDBName))
                            continue;

                        SiteListMasterDAL siteListMasterDAL = new SiteListMasterDAL(enterprise.EnterpriseDBName);
                        SiteListMasterDTO siteListMaster = siteListMasterDAL.GetAllItemsById(ListId, SiteSettingHelper.UsersUISettingType);

                        if (siteListMaster != null && !string.IsNullOrEmpty(siteListMaster.JSONDATA))
                        {
                            var minOrderOfNewlyAddedColumn = columnOrders.Min();
                            var noOfColumnsToAdd = columnOrders.Count();
                            var Json = siteListMaster.JSONDATA;
                            JObject gridStateJS = new JObject();
                            // jsonData = objDTO.JSONDATA;
                            /*////////CODE FOR UPDATE JSON STRING/////////*/
                            // JObject gridStaeJS = new JObject();
                            gridStateJS = JObject.Parse(Json);
                            /*////////CODE FOR UPDATE JSON STRING/////////*/

                            JToken orderCols = gridStateJS["ColReorder"];
                            JArray arrOCols = (JArray)orderCols;
                            JArray arrONewCols = new JArray();

                            if (arrOCols != null)
                            {
                                int orderClength = arrOCols.Count;

                                if (orderClength > 4)
                                {
                                    JToken abVisCols = gridStateJS["abVisCols"];
                                    JArray visCols = (JArray)abVisCols;
                                    JToken aoSearchCols = gridStateJS["aoSearchCols"];
                                    JArray arrSCols = (JArray)aoSearchCols;

                                    if (arrSCols != null)
                                    {
                                        JObject UpdateAccProfile = new JObject(
                                                new JProperty("bCaseInsensitive", true),
                                                new JProperty("sSearch", ""),
                                                new JProperty("bRegex", false),
                                                new JProperty("bSmart", true));
                                        for (int count = 0; count < noOfColumnsToAdd; count++)
                                        {
                                            arrSCols.Add((object)UpdateAccProfile);
                                        }
                                    }

                                    if (visCols != null)
                                    {
                                        for (int count = 0; count < noOfColumnsToAdd; count++)
                                        {
                                            visCols.Insert(columnOrders[count], true);
                                        }
                                    }

                                    JToken widthCols = gridStateJS["ColWidth"];
                                    JArray arrWCols = (JArray)widthCols;

                                    if (arrWCols != null)
                                    {
                                        for (int count = 0; count < noOfColumnsToAdd; count++)
                                        {
                                            arrWCols.Insert(columnOrders[count], "100px");
                                        }
                                    }

                                    for (int i = 0; i < orderClength; i++)
                                    {
                                        if (Convert.ToInt32(((JValue)(arrOCols[i])).Value) >= minOrderOfNewlyAddedColumn)
                                        {
                                            ((JValue)(arrOCols[i])).Value = Convert.ToInt32(((JValue)(arrOCols[i])).Value) + noOfColumnsToAdd;
                                        }
                                    }
                                    for (int count = 0; count < noOfColumnsToAdd; count++)
                                    {
                                        arrOCols.Insert(columnOrders[count], columnOrders[count]);
                                    }

                                    gridStateJS["ColReorder"] = arrOCols;
                                    gridStateJS["abVisCols"] = visCols;
                                    gridStateJS["aoSearchCols"] = arrSCols;
                                    gridStateJS["ColWidth"] = arrWCols;
                                    string updatedJSON = gridStateJS.ToString();

                                    /*/////////////CODE FOR SAVE DATA IN Site List Master//////////////*/
                                    siteListMasterDAL.UpdateJSONData(ListId, updatedJSON, SiteSettingHelper.UsersUISettingType);
                                    /*/////////////CODE FOR SAVE DATA IN Site List Master//////////////*/
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ItemMasterBinList()
        {
            return View();
        }
        public ActionResult ItemMasterBinListAjax(QuickListJQueryDataTableParamModel param)
        {
            ItemMasterBinDAL obj = new ItemMasterBinDAL(SessionHelper.EnterPriseDBName);
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

            if (string.IsNullOrEmpty(sortColumnName) || sortColumnName == "undefine" || sortColumnName == "0" || sortColumnName.Trim().Contains("null") || string.IsNullOrEmpty(sortColumnName.Replace("asc", "").Replace("null", "").Replace("desc", "").Trim()))
                sortColumnName = "ID asc";

            string searchQuery = string.Empty;
            int TotalRecordCount = 0;
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetPagedItemMasterBin(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, WebUtility.HtmlDecode(param.sSearch), sortColumnName, SessionHelper.RoomID, IsArchived, IsDeleted, SessionHelper.CompanyID, Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone);

            JsonResult jsresult = Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB
            }, JsonRequestBehavior.AllowGet);
            jsresult.MaxJsonLength = int.MaxValue;

            return jsresult;
        }
        public ActionResult ItemBinEdit(string BinGUID)
        {
            bool IsArchived = false;
            bool IsDeleted = false;
            bool IsHistory = false;

            if (Request["IsHistory"] != null && Request["IsHistory"].ToString() != "")
                IsHistory = bool.Parse(Request["IsHistory"].ToString());

            if (!string.IsNullOrEmpty(Request["IsArchived"]) && !string.IsNullOrEmpty(Request["IsDeleted"]))
            {
                IsArchived = bool.Parse(Request["IsArchived"].ToString());
                IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            }

            if (IsDeleted || IsArchived || IsHistory)
            {
                ViewBag.ViewOnly = true;
            }

            ItemMasterBinDAL obj = new ItemMasterBinDAL(SessionHelper.EnterPriseDBName);
            ItemMasterBinDTO objDTO = obj.GetBinItemUsingGuid(Guid.Parse(BinGUID), SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted).ToList().FirstOrDefault();

            if (objDTO != null)
            {
                objDTO.CreatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Created, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                objDTO.UpdatedDate = CommonUtility.ConvertDateByTimeZone(objDTO.Updated, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
            }
            string ViewName = "_CreateItemBin";
            return PartialView(ViewName, objDTO);
        }
        public JsonResult ItemBinSave(ItemMasterBinDTO objDTO)
        {
            string Message = string.Empty;
            try
            {
                if (string.IsNullOrWhiteSpace(objDTO.NewLocationName))
                {
                    return Json(new { Message = string.Format(ResMessage.MsgRequired, ResItemBinMaster.NewLocationName), Status = "success" }, JsonRequestBehavior.AllowGet);
                }
                Guid ItemGuid = Guid.Empty;
                if (objDTO.NewLocationName != objDTO.OldLocationName)
                {
                    ItemMasterBinDAL obj = new ItemMasterBinDAL(SessionHelper.EnterPriseDBName);
                    if (Guid.TryParse(objDTO.ItemGuid.ToString(), out ItemGuid))
                    {
                        Message = obj.SaveItemMasterBin(ItemGuid, objDTO.OldLocation ?? 0, objDTO.NewLocationName, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, objDTO.IsDefault ?? false);
                    }
                    else
                    {
                        return Json(new { Message = ResItemBinMaster.IssueWithItem, Status = "fail" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { Message = ResItemBinMaster.BinSavedSuccessfully, Status = "success" }, JsonRequestBehavior.AllowGet);
                }
                if (Message == "Success")
                {
                    return Json(new { Message = ResItemBinMaster.BinSavedSuccessfully, Status = "success" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Message = Message, Status = "fail" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message.ToString(), Status = "fail" }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult GetTotalItemMasterNumber()
        {
            int ItemCount = 0;
            ItemMasterDAL objDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            ItemCount = objDAL.GetItemCountFromRoom(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserSupplierIds);
            return Json(new { result = ItemCount.ToString() }, JsonRequestBehavior.AllowGet);
        }




        public JsonResult GetItemBinChangeHistory(string ItemGUID)
        {
            bool isRecordAvail = false;
            string errorMessage = string.Empty;
            BinMasterDAL objBinMasterDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
            List<ItemBinChangeHistory> lstBinHistory = new List<ItemBinChangeHistory>();
            if (!string.IsNullOrWhiteSpace(ItemGUID))
            {
                string ItemGUIDList = ItemGUID.Trim(',');
                DateTime createdDate = DateTimeUtility.DateTimeNow;
                if (Session["SaveBinCreatedDate"] != null)
                {
                    createdDate = Convert.ToDateTime(Session["SaveBinCreatedDate"]);

                }

                lstBinHistory = objBinMasterDAL.GetItemBinChangeHistoryByUserID(createdDate, SessionHelper.UserID, SessionHelper.RoomID, SessionHelper.CompanyID, ItemGUIDList);
                if (lstBinHistory != null && lstBinHistory.Count > 0)
                {
                    isRecordAvail = true;
                }
                else
                {
                    isRecordAvail = false;
                }
            }
            else
            {
                errorMessage = ResItemBinMaster.InvalidItemGuid;
            }

            Session["SaveBinCreatedDate"] = null;
            return Json(new
            {
                errorMessage = errorMessage,
                isRecordAvail = isRecordAvail,
                historyData = lstBinHistory,
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UserRoleChangeLog(long UserID)
        {
            ViewBag.UserID = UserID;
            return PartialView("_UserRoleChangeLog");
        }


        public ActionResult UserRoleChangeLogListAjax(JQueryDataTableParamModel param)
        {
            eTurns.DAL.UserMasterDAL obj = new eTurns.DAL.UserMasterDAL(SessionHelper.EnterPriseDBName);
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

            long UserID = 0;
            long.TryParse(Request["UserID"], out UserID);
            // set the default column sorting here, if first time then required to set 


            string searchQuery = string.Empty;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
                searchQuery = param.sSearch;

            int TotalRecordCount = 0;

            List<UserRoleModuleDetailsDTO> DataFromDB = obj.GetUserRoleChangeLogRecords(param.iDisplayStart, PageSize, out TotalRecordCount, UserID, searchQuery);

            if (DataFromDB != null)
            {
                DataFromDB.ToList().ForEach(t =>
                {
                    t.HistoryDateDisplay = CommonUtility.ConvertDateByTimeZone(t.HistoryDate, SessionHelper.CurrentTimeZone, SessionHelper.DateTimeFormat, SessionHelper.RoomCulture, true);
                });

            }


            JsonResult jsresult = Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount,
                aaData = DataFromDB
            }, JsonRequestBehavior.AllowGet);
            jsresult.MaxJsonLength = int.MaxValue;
            return jsresult;


        }
        public void InsertAlleTurnsActionMethods()
        {

            //XElement Settinfile = XElement.Load(System.Web.Hosting.HostingEnvironment.MapPath("/SiteSettings.xml"));
            string ControllerName = SiteSettingHelper.ControllerName; // Settinfile.Element("ControllerName").Value;
            string ModuleName = SiteSettingHelper.ModuleName; // Settinfile.Element("ModuleName").Value;
            string ModuleMasterName = SiteSettingHelper.ModuleMasterName; // Settinfile.Element("ModuleMasterName").Value;

            string sSQL = string.Empty;
            Assembly asm = Assembly.GetAssembly(typeof(eTurnsWeb.MvcApplication));
            var controlleractionlist = asm.GetTypes()
           .Where(type => typeof(System.Web.Mvc.Controller).IsAssignableFrom(type))
           .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
           .Where(m => !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any())
           .Select(x => new
           {
               Controller = x.DeclaringType.Name,
               Action = x.Name,
               ReturnType = x.ReturnType.Name,
               Attributes = String.Join(",", x.GetCustomAttributes(true).Select(a => a.GetType().Name.Replace("Attribute", "")))
           })
           .OrderBy(x => x.Controller).ThenBy(x => x.Action).ToList();

            sSQL += "\r\n  Declare @ModeuleID bigint";
            sSQL += "\r\n select @ModeuleID = ID from [" + DbConnectionHelper.GetETurnsMasterDBName() + "].[dbo].[ModuleMaster] where trim(lower(ModuleName))= trim(lower('" + ModuleMasterName + "'))";
            //sSQL += "\r\n GO";
            sSQL += "\r\n IF (isnull(@ModeuleID,0) != 0)";
            sSQL += "\r\n Begin ";
            foreach (var item in controlleractionlist.Where(i => Convert.ToString(i.Controller.ToLower().Trim()) == ControllerName.ToLower().Trim()).ToList())
            {

                string Controller = item.Controller.Replace("Controller", string.Empty);
                string Action = item.Action;
                string Attributes = item.Attributes;
                string ReturnType = item.ReturnType;
                string IsInsert = "0";
                string IsDelete = "0";
                if (Action.ToLower().Trim().Contains("save"))
                {
                    IsInsert = "1";
                }
                if (Action.ToLower().Trim().Contains("delete"))
                {
                    IsDelete = "1";
                }

                sSQL += "\r\n   IF NOT EXISTS (SELECT ID FROM [" + DbConnectionHelper.GetETurnsMasterDBName() + "].[dbo].[AlleTurnsActionMethods] WHERE ActionMethod='" + item.Action + "' and Controller='" + Controller + "')";
                sSQL += "\r\n   Begin ";
                sSQL += " \r\n      INSERT INTO [" + DbConnectionHelper.GetETurnsMasterDBName() + "].[dbo].[AlleTurnsActionMethods] (ActionMethod,Controller,Module,IsView,IsChecked,IsInsert,IsUpdate,IsDelete,ShowDeleted,ShowArchived,ShowUDF,ShowChangeLog,PermissionModuleID,Attributes) ";
                sSQL += "\r\n       VALUES ('" + Action + "','" + Controller + "', '" + ModuleName + "', 1, 0," + Convert.ToString(IsInsert) + "," + Convert.ToString(IsInsert) + "," + Convert.ToString(IsDelete) + ",0,null,null,null, @ModeuleID,'" + Attributes + "' )";
                sSQL += "\r\n    End ";


            }
            sSQL += "\r\n End ";
            sSQL += "\r\n GO ";
            Console.Write(sSQL);
        }

        public JsonResult GetCountOfAutoClassificationItemsInRoom()
        {
            ItemMasterDAL itemMasterDAL = new ItemMasterDAL(SessionHelper.EnterPriseDBName);
            var countOfAutoClassificationItemsInRoom = itemMasterDAL.GetCountOfAutoClassificationItemsInRoom(SessionHelper.CompanyID, SessionHelper.RoomID);
            return Json((countOfAutoClassificationItemsInRoom > 0), JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult SendEmailList()
        {
            if (eTurnsWeb.Helper.SessionHelper.RoleID == -1 && eTurnsWeb.Helper.SessionHelper.UserType == 1)
            {

                long TotalRecordCount = 0;
                eMailToSendDAL objeMailToSendDAL = new eMailToSendDAL();
                string streMailToSendDB = ConfigurationManager.AppSettings["eTurnsEMailDBName"] ?? "eTurnsEmails";
                string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
                List<eMailToSendListDTO> DataFromDB = objeMailToSendDAL.GetSendEmailPagedRecords(streMailToSendDB, 0, int.MaxValue, out TotalRecordCount, string.Empty, string.Empty, SessionHelper.UserID, "currentFlag", RoomDateFormat);
                Session["SendEmailList"] = DataFromDB;
                return View();
            }
            else
            {
                string CtrlName = Convert.ToString(ConfigurationManager.AppSettings["CtrlName"]);
                string ActName = Convert.ToString(ConfigurationManager.AppSettings["ActName"]);
                return RedirectToAction(ActName, CtrlName, new { FromLogin = "yes" });
            }
        }

        public ActionResult SendEmailListAJAX(JQueryDataTableParamModel param)
        {
            string streMailToSendDB = ConfigurationManager.AppSettings["eTurnsEMailDBName"] ?? "eTurnsEmails";
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

            if (!string.IsNullOrWhiteSpace(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined") || sortColumnName.Trim().ToLower() == "id")
                    sortColumnName = "emailHistoryID";

                if (sortDirection == "asc")
                    sortColumnName = sortColumnName + " asc";
                else
                    sortColumnName = sortColumnName + " desc";
            }
            else
                sortColumnName = "emailHistoryID desc";

            string strActionFilter = Request["ActionFilter"].ToString();



            string searchQuery = string.Empty;

            long TotalRecordCount = 0;
            eMailToSendDAL objeMailToSendDAL = new eMailToSendDAL();
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            List<eMailToSendListDTO> DataFromDB = objeMailToSendDAL.GetSendEmailPagedRecords(streMailToSendDB, param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.UserID, strActionFilter, RoomDateFormat);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
                                    JsonRequestBehavior.AllowGet);
        }

        public JsonResult DownloadEmailAttachment(long attHistoryID, string actionFilter)
        {
            if (attHistoryID > 0)
            {
                eMailToSendDAL objeMailToSendDAL = new eMailToSendDAL();
                string streMailToSendDB = ConfigurationManager.AppSettings["eTurnsEMailDBName"] ?? "eTurnsEmails";
                eMailToSendListDTO objeMailToSendDTO = objeMailToSendDAL.GeteMailToSendHistoryData(streMailToSendDB, attHistoryID, actionFilter);
                if (objeMailToSendDTO != null && objeMailToSendDTO.FileData != null && objeMailToSendDTO.FileData.Length > 0)
                {
                    try
                    {
                        byte[] bytes = objeMailToSendDTO.FileData;
                        string RDLCBaseFilePath = CommonUtility.RDLCBaseFilePath;
                        string reportFileName = RDLCBaseFilePath + "/Temp/" + objeMailToSendDTO.AttachedFileName;
                        reportFileName = reportFileName.Replace(eTurnsAppConfig.BaseFileSharedPath, eTurnsAppConfig.BaseFilePath);
                        using (FileStream fs = new FileStream(reportFileName, FileMode.Create))
                        {
                            fs.Write(bytes, 0, bytes.Length);
                        }
                        string FileURL = "/RDLC_Reports/Temp/" + objeMailToSendDTO.AttachedFileName;
                        return Json(new { Status = true, Message = "ok", AttachedFileURL = FileURL }, JsonRequestBehavior.AllowGet);
                    }
                    catch
                    {
                        return Json(new { Status = false, Message = ResCommon.DataNotAvailable, AttachedFileURL = "" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { Status = false, Message = ResCommon.DataNotAvailable, AttachedFileURL = "" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Status = false, Message = ResCommon.DataNotAvailable, AttachedFileURL = "" }, JsonRequestBehavior.AllowGet);

            }
        }
        public JsonResult ReSendSelectedEmails(string EmailIDs, string ActionFilter)
        {
            if (!string.IsNullOrWhiteSpace(EmailIDs))
            {
                string streMailToSendDB = ConfigurationManager.AppSettings["eTurnsEMailDBName"] ?? "eTurnsEmails";
                eMailMasterDAL objeMailToSendDAL = new eMailMasterDAL(streMailToSendDB);

                try
                {
                    bool retMessage = objeMailToSendDAL.ReSednEmailsByIDs(EmailIDs, ActionFilter);
                    return Json(new { Status = true, Message = "ok" }, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    return Json(new { Status = false, Message = ResCommon.DataNotAvailable }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Status = false, Message = ResCommon.DataNotAvailable, AttachedFileURL = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetShelfID(long ComPortRoomMappingID)
        {
            string Msg = string.Empty;
            int shelfId = 0;
            if (SessionHelper.isEVMI == true)
            {
                bool IsImmediate = SessionHelper.SensorBinRoomSettings.IsGetShelfIDRequestImmediate();
                eVMIShelfRequestDAL objDAL = new eVMIShelfRequestDAL(SessionHelper.EnterPriseDBName);
                eVMIShelfRequestDTO objDTO = new eVMIShelfRequestDTO();
                objDTO.ShelfID = 0;
                objDTO.RequestType = IsImmediate ? (int)eVMIShelfRequestType.GetShelfIDImmediate : (int)eVMIShelfRequestType.GetShelfID;
                objDTO.RoomID = SessionHelper.RoomID;
                objDTO.CompanyID = SessionHelper.CompanyID;
                objDTO.CreatedBy = SessionHelper.UserID;
                objDTO.ComPortRoomMappingID = ComPortRoomMappingID;
                objDAL.InsertShelfRequest(objDTO);

                if (IsImmediate)
                {
                    eTurns.eVMIBAL.ShelfRequest request = new eTurns.eVMIBAL.ShelfRequest();
                    shelfId = request.ShelfRequestProcessForRoom(objDTO.ID, SessionHelper.EnterPriseDBName
                        , SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);
                }
            }

            return Json(new { Status = "ok", shelfId = shelfId }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult SetShelfID(string ShelfID, long ComPortRoomMappingID)
        {
            string Msg = string.Empty;
            int shelfId = 0;
            if (SessionHelper.isEVMI == true)
            {
                bool IsImmediate = SessionHelper.SensorBinRoomSettings.IsSetShelfIDRequestImmediate();
                eVMIShelfRequestDAL objDAL = new eVMIShelfRequestDAL(SessionHelper.EnterPriseDBName);
                eVMIShelfRequestDTO objDTO = new eVMIShelfRequestDTO();
                objDTO.ShelfID = Convert.ToInt32(ShelfID);
                objDTO.RequestType = IsImmediate ? (int)eVMIShelfRequestType.SetShelfIDImmediate : (int)eVMIShelfRequestType.SetShelfID;
                objDTO.RoomID = SessionHelper.RoomID;
                objDTO.CompanyID = SessionHelper.CompanyID;
                objDTO.CreatedBy = SessionHelper.UserID;
                objDTO.ComPortRoomMappingID = ComPortRoomMappingID;
                objDAL.InsertShelfRequest(objDTO);

                if (IsImmediate)
                {
                    eTurns.eVMIBAL.ShelfRequest request = new eTurns.eVMIBAL.ShelfRequest();
                    shelfId = request.ShelfRequestProcessForRoom(objDTO.ID, SessionHelper.EnterPriseDBName
                        , SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);
                }

            }

            return Json(new { Status = "ok", shelfId = shelfId }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult SetTareAllTrue()
        {
            var isSensorBinsRFIDeTags = SessionHelper.GetAdminPermission(SessionHelper.ModuleList.SensorBinsRFIDeTags);

            if (isSensorBinsRFIDeTags)
            {
                try
                {
                    bool _IsOldeVMIRoom = Helper.CommonUtility.IsOldeVMIRoom();

                    if (_IsOldeVMIRoom == false)
                    {
                        ItemLocationTareRequestDAL objILTareRequestDAL = new ItemLocationTareRequestDAL(SessionHelper.EnterPriseDBName);

                        if (SessionHelper.isEVMI == true)
                        {
                            ItemLocationTareRequestDTO objILPollRequestDTO = new ItemLocationTareRequestDTO();
                            objILPollRequestDTO.RoomID = SessionHelper.RoomID;
                            objILPollRequestDTO.CompanyID = SessionHelper.CompanyID;
                            objILPollRequestDTO.RequestType = (int)TareRequestType.TareAll;
                            objILPollRequestDTO.IsTareStarted = false;
                            objILPollRequestDTO.CreatedBy = SessionHelper.UserID;

                            objILTareRequestDAL.InsertItemLocationTareAllRequest(objILPollRequestDTO);
                        }

                    }

                    return Json(new { Message = ResCommon.Success, Status = "ok", value = Convert.ToString(Session["MainFilter"]).ToLower() }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { Message = ex.Message, Status = "fail" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Status = "fail", Message = ResCommon.MsgNoPermission }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult SetCalibrateAllTrue()
        {
            try
            {
                bool _IsOldeVMIRoom = Helper.CommonUtility.IsOldeVMIRoom();

                if (_IsOldeVMIRoom == false)
                {
                    ItemLocationCalibrateRequestDAL objILCalibrateRequestDAL = new ItemLocationCalibrateRequestDAL(SessionHelper.EnterPriseDBName);

                    if (SessionHelper.isEVMI == true)
                    {
                        ItemLocationCalibrateRequestDTO objILCalibrateRequestDTO = new ItemLocationCalibrateRequestDTO();
                        objILCalibrateRequestDTO.RoomID = SessionHelper.RoomID;
                        objILCalibrateRequestDTO.CompanyID = SessionHelper.CompanyID;
                        objILCalibrateRequestDTO.RequestType = (int)CalibrateRequestType.CalibrateAll;
                        objILCalibrateRequestDTO.IsStep1Started = false;
                        objILCalibrateRequestDTO.CreatedBy = SessionHelper.UserID;

                        objILCalibrateRequestDAL.InsertItemLocationCalibrateAllRequest(objILCalibrateRequestDTO);
                    }

                }

                return Json(new { Message = ResCommon.Success, Status = "ok", value = Convert.ToString(Session["MainFilter"]).ToLower() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, Status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ItemLocationeVMI(string ItemLocationName, Int64 ItemLocationID, Int32 ReqID, Int64? TotalQTY, Int64? CalibrateWeight, int? SetShelfID, long ComPortRoomMappingID)
        {
            string Msg = string.Empty;
            double weightPerPiece = 0;
            if (SessionHelper.isEVMI == true)
            {
                BinMasterDAL objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                BinMasterDTO objBinDTO = objBinDAL.GetBinByID(ItemLocationID, SessionHelper.RoomID, SessionHelper.CompanyID);

                if (ReqID == 1)  // Poll
                {
                    #region Poll
                    bool IsRequestImmediate = SessionHelper.SensorBinRoomSettings.IsPollRequestImmediate();
                    ItemLocationPollRequestDAL objILPollDAL = new ItemLocationPollRequestDAL(SessionHelper.EnterPriseDBName);
                    ItemLocationPollRequestDTO objILPollDTO = new ItemLocationPollRequestDTO();

                    objILPollDTO.ItemGUID = (Guid)objBinDTO.ItemGUID;
                    objILPollDTO.BinID = objBinDTO.ID;
                    objILPollDTO.RoomID = SessionHelper.RoomID;
                    objILPollDTO.CompanyID = SessionHelper.CompanyID;
                    objILPollDTO.RequestType = IsRequestImmediate ? (int)PollRequestType.PollImmediate : (int)PollRequestType.Poll;
                    objILPollDTO.IsPollStarted = false;
                    objILPollDTO.CreatedBy = SessionHelper.UserID;
                    InventoryCountDTO objInventoryCountDTO = new InventoryCountDAL(SessionHelper.EnterPriseDBName).InsertPollCount(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "web-evmireq");
                    if (objInventoryCountDTO != null && objInventoryCountDTO.ID > 0)
                    {
                        objILPollDTO.CountGUID = objInventoryCountDTO.GUID;
                    }
                    objILPollDAL.InsertItemLocationPollRequest(objILPollDTO);
                    Msg = "ok";

                    if (IsRequestImmediate)
                    {
                        eTurns.eVMIBAL.PollRequest pollRequest = new eTurns.eVMIBAL.PollRequest();
                        pollRequest.ProcessGetWightForItem(objILPollDTO.ID, SessionHelper.EnterPriseDBName, SessionHelper.EnterPriceID
                            , SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);
                    }
                    #endregion
                }
                else if (ReqID == 2) // Tare
                {
                    #region Tare
                    ItemLocationTareRequestDAL objILTareRequestDAL = new ItemLocationTareRequestDAL(SessionHelper.EnterPriseDBName);
                    ItemLocationTareRequestDTO objILTareRequestDTO = new ItemLocationTareRequestDTO();
                    bool IsTareRequestImmediate = SessionHelper.SensorBinRoomSettings.IsTareRequestImmediate();
                    objILTareRequestDTO.ItemGUID = (Guid)objBinDTO.ItemGUID;
                    objILTareRequestDTO.BinID = objBinDTO.ID;
                    objILTareRequestDTO.RoomID = SessionHelper.RoomID;
                    objILTareRequestDTO.CompanyID = SessionHelper.CompanyID;
                    objILTareRequestDTO.RequestType = IsTareRequestImmediate ? (int)TareRequestType.TareImmediate : (int)TareRequestType.Tare;
                    objILTareRequestDTO.IsTareStarted = false;
                    objILTareRequestDTO.CreatedBy = SessionHelper.UserID;

                    objILTareRequestDAL.InsertItemLocationTareRequest(objILTareRequestDTO);

                    if (IsTareRequestImmediate)
                    {
                        // immediate process request
                        eTurns.eVMIBAL.TareRequest tareRequest = new eTurns.eVMIBAL.TareRequest();
                        tareRequest.TareProcessForItem(objILTareRequestDTO.ID, SessionHelper.EnterPriseDBName, SessionHelper.EnterPriceID
                            , SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);
                    }

                    Msg = "ok";
                    #endregion
                }
                else if (ReqID == 3 || ReqID == 4) // Calibrate
                {
                    #region Calibrate
                    ItemLocationCalibrateRequestDAL objILCalibrateDAL = new ItemLocationCalibrateRequestDAL(SessionHelper.EnterPriseDBName);
                    ItemLocationCalibrateRequestDTO objILCalibrateDTO = new ItemLocationCalibrateRequestDTO();

                    objILCalibrateDTO.ItemGUID = (Guid)objBinDTO.ItemGUID;
                    objILCalibrateDTO.BinID = objBinDTO.ID;
                    objILCalibrateDTO.RoomID = SessionHelper.RoomID;
                    objILCalibrateDTO.CompanyID = SessionHelper.CompanyID;
                    objILCalibrateDTO.RequestType = (int)CalibrateRequestType.Calibrate;
                    objILCalibrateDTO.IsStep1Started = false;
                    objILCalibrateDTO.CreatedBy = SessionHelper.UserID;
                    if (ReqID == 4)
                    {
                        objILCalibrateDTO.CalibrationWeight = CalibrateWeight;
                    }

                    objILCalibrateDAL.InsertItemLocationCalibrateRequest(objILCalibrateDTO);
                    Msg = "ok";
                    #endregion
                }
                else if (ReqID == 5) // WeightPerPiece
                {
                    bool IsRequestImmediate = SessionHelper.SensorBinRoomSettings.IsItemWeightPerPieceRequestImmediate();
                    ItemWeightRequestDAL objWeightRequestDAL = new ItemWeightRequestDAL(SessionHelper.EnterPriseDBName);
                    ItemWeightPerPieceRequestDTO objItemWeightReqDTO = new ItemWeightPerPieceRequestDTO();
                    objItemWeightReqDTO.ItemGUID = (Guid)objBinDTO.ItemGUID;
                    objItemWeightReqDTO.RoomID = SessionHelper.RoomID;
                    objItemWeightReqDTO.CompanyID = SessionHelper.CompanyID;
                    objItemWeightReqDTO.IsWeightStarted = false;
                    objItemWeightReqDTO.TotalQty = TotalQTY;
                    objItemWeightReqDTO.CreatedBy = SessionHelper.UserID;
                    objItemWeightReqDTO.RequestType = IsRequestImmediate ? (int)eVMIWeightPerPieceRequestType.WeightPerPieceImmediate : (int)eVMIWeightPerPieceRequestType.WeightPerPiece;
                    var res = objWeightRequestDAL.InsertItemWeightPerPieceRequest(objItemWeightReqDTO);
                    Int64 Flag = res.ReturnFlag;

                    if (Flag == 1)
                    {
                        Msg = "ok";
                    }
                    else if (Flag == 2)
                    {
                        Msg = ResItemMaster.DefaultBinSensor;
                    }

                    if (IsRequestImmediate)
                    {
                        eTurns.eVMIBAL.ItemWeightPerPieceRequest weightPerPieceRequest = new eTurns.eVMIBAL.ItemWeightPerPieceRequest();
                        weightPerPiece = weightPerPieceRequest.GetItemWeightPerPieceProcessForItem(objItemWeightReqDTO.ID, SessionHelper.EnterPriseDBName
                            , SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);
                    }

                }
                else if (ReqID == 6) // Reset Request
                {
                    bool IsResetRequestImmediate = SessionHelper.SensorBinRoomSettings.IsResetRequestImmediate();
                    EVMIResetRequestDAL resetRequestDAL = new EVMIResetRequestDAL(SessionHelper.EnterPriseDBName);
                    eVMIResetRequestDTO resetDTO = new eVMIResetRequestDTO();
                    resetDTO.ItemGUID = (Guid)objBinDTO.ItemGUID;
                    resetDTO.RoomID = SessionHelper.RoomID;
                    resetDTO.CompanyID = SessionHelper.CompanyID;
                    resetDTO.BinID = objBinDTO.ID;
                    resetDTO.CreatedBy = SessionHelper.UserID;
                    resetDTO.RequestType = IsResetRequestImmediate ? (int)eVMIResetRequestType.ResetImmediate : (int)eVMIResetRequestType.Reset;
                    resetRequestDAL.InsertEVMIResetRequest(resetDTO);
                    if (IsResetRequestImmediate)
                    {
                        // immediate process request
                        eTurns.eVMIBAL.ResetRequest resetRequest = new eTurns.eVMIBAL.ResetRequest();
                        resetRequest.ResetProcessForItem(resetDTO.ID, SessionHelper.EnterPriseDBName, SessionHelper.EnterPriceID
                            , SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);
                    }
                    Msg = "ok";

                }


            }

            return Json(new { Status = Msg, WeightPerPiece = weightPerPiece }, JsonRequestBehavior.AllowGet);

        }



        [HttpPost]
        public JsonResult eVMIRequestSave(eVMIRequestDTO objeVMIRequestDTO)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(eTurnsAppConfig.eVMIWebAPIURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                objeVMIRequestDTO.EnterPriceID = SessionHelper.EnterPriceID;
                objeVMIRequestDTO.CompanyID = SessionHelper.CompanyID;
                objeVMIRequestDTO.RoomID = SessionHelper.RoomID;
                objeVMIRequestDTO.UserID = SessionHelper.UserID;
                objeVMIRequestDTO.isEVMI = SessionHelper.isEVMI;
                objeVMIRequestDTO.EnterPriseDBName = SessionHelper.EnterPriseDBName;
                objeVMIRequestDTO.SensorBinRoomSettings = SessionHelper.SensorBinRoomSettings;

                HttpResponseMessage response = client.PostAsJsonAsync("api/SmartShelves/eVMIRequestSave", objeVMIRequestDTO).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    eVmiReqRespDTO resp = response.Content.ReadAsAsync<eVmiReqRespDTO>().Result;
                    return Json(resp);
                }
                else
                {
                    return null;
                }
            }

        }

        [HttpPost]
        public JsonResult eVMIRequestSave1(eVMIRequestDTO objeVMIRequestDTO)
        {
            string Msg = string.Empty;
            double weightPerPiece = 0;
            int ShelfID = 0;
            long? calibrateRequestID = null;
            double calWeight = 0;
            string sVersion = "";
            string SerialNo = "";
            string Model = "";
            if (objeVMIRequestDTO.ComPortRoomMappingID > 0 && objeVMIRequestDTO.ReqID > 0 && (SessionHelper.isEVMI ?? false))
            {
                ComPortRoomMappingDAL objComPortRoomMappingDAL = new ComPortRoomMappingDAL(SessionHelper.EnterPriseDBName);
                ComPortRoomMappingDTO objComPortRoomMappingDTO = objComPortRoomMappingDAL.GetComPortMappingByID(objeVMIRequestDTO.ComPortRoomMappingID);
                if (objComPortRoomMappingDTO != null && objComPortRoomMappingDTO.ID > 0)
                {
                    BinMasterDAL objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                    BinMasterDTO objBinDTO = null;
                    if ((objeVMIRequestDTO.ItemLocationID ?? 0) > 0 && (new int[] { 1, 2, 3, 4, 5 }.Contains(objeVMIRequestDTO.ReqID)))
                    {
                        objBinDTO = objBinDAL.GetBinByID((objeVMIRequestDTO.ItemLocationID ?? 0), SessionHelper.RoomID, SessionHelper.CompanyID);
                    }
                    switch (objeVMIRequestDTO.ReqID)
                    {
                        case 1:
                            #region Poll
                            bool IsPollImm = SessionHelper.SensorBinRoomSettings.IsPollRequestImmediate();
                            ItemLocationPollRequestDAL objILPollDAL = new ItemLocationPollRequestDAL(SessionHelper.EnterPriseDBName);
                            ItemLocationPollRequestDTO objILPollDTO = new ItemLocationPollRequestDTO();

                            objILPollDTO.ItemGUID = (Guid)objBinDTO.ItemGUID;
                            objILPollDTO.BinID = objBinDTO.ID;
                            objILPollDTO.RoomID = SessionHelper.RoomID;
                            objILPollDTO.CompanyID = SessionHelper.CompanyID;
                            objILPollDTO.RequestType = IsPollImm ? (int)PollRequestType.PollImmediate : (int)PollRequestType.Poll;
                            objILPollDTO.IsPollStarted = false;
                            objILPollDTO.CreatedBy = SessionHelper.UserID;
                            InventoryCountDTO objInventoryCountDTO = new InventoryCountDAL(SessionHelper.EnterPriseDBName).InsertPollCount(SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserID, "web-evmireq");
                            if (objInventoryCountDTO != null && objInventoryCountDTO.ID > 0)
                            {
                                objILPollDTO.CountGUID = objInventoryCountDTO.GUID;
                            }
                            objILPollDAL.InsertItemLocationPollRequest(objILPollDTO);
                            Msg = "ok";

                            if (IsPollImm)
                            {
                                eTurns.eVMIBAL.PollRequest pollRequest = new eTurns.eVMIBAL.PollRequest();
                                pollRequest.ProcessGetWightForItem(objILPollDTO.ID, SessionHelper.EnterPriseDBName, SessionHelper.EnterPriceID
                                    , SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);
                            }
                            #endregion
                            break;
                        case 2:
                            #region Tare
                            ItemLocationTareRequestDAL objILTareRequestDAL = new ItemLocationTareRequestDAL(SessionHelper.EnterPriseDBName);
                            ItemLocationTareRequestDTO objILTareRequestDTO = new ItemLocationTareRequestDTO();
                            bool IsTareRequestImmediate = SessionHelper.SensorBinRoomSettings.IsTareRequestImmediate();
                            objILTareRequestDTO.ItemGUID = (Guid)objBinDTO.ItemGUID;
                            objILTareRequestDTO.BinID = objBinDTO.ID;
                            objILTareRequestDTO.RoomID = SessionHelper.RoomID;
                            objILTareRequestDTO.CompanyID = SessionHelper.CompanyID;
                            objILTareRequestDTO.RequestType = IsTareRequestImmediate ? (int)TareRequestType.TareImmediate : (int)TareRequestType.Tare;
                            objILTareRequestDTO.IsTareStarted = false;
                            objILTareRequestDTO.CreatedBy = SessionHelper.UserID;

                            objILTareRequestDAL.InsertItemLocationTareRequest(objILTareRequestDTO);

                            if (IsTareRequestImmediate)
                            {
                                // immediate process request
                                eTurns.eVMIBAL.TareRequest tareRequest = new eTurns.eVMIBAL.TareRequest();
                                tareRequest.TareProcessForItem(objILTareRequestDTO.ID, SessionHelper.EnterPriseDBName, SessionHelper.EnterPriceID
                                    , SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);
                            }

                            Msg = "ok";
                            #endregion
                            break;
                        case 3:
                            #region Calibrate - Run Command for Calibrate Step1 and Step2
                            {
                                bool IsCalImmediate = true; //SessionHelper.SensorBinRoomSettings.IsCalibrateRequestImmediate();

                                Msg = "ok";
                                if (IsCalImmediate)
                                {
                                    // immediate process request
                                    eTurns.eVMIBAL.CalibrateRequest objCalibrateRequest = new eTurns.eVMIBAL.CalibrateRequest();

                                    //Msg = objCalibrateRequest.ProcesStep1AndStep2(objILCalibrateDTO.ID,
                                    //    SessionHelper.EnterPriseDBName, SessionHelper.EnterPriceID,
                                    //    SessionHelper.CompanyID, SessionHelper.RoomID,
                                    //    SessionHelper.UserID);

                                    if (objeVMIRequestDTO.cmd == "cal1")
                                    {
                                        ItemLocationCalibrateRequestDAL objILCalibrateDAL = new ItemLocationCalibrateRequestDAL(SessionHelper.EnterPriseDBName);
                                        ItemLocationCalibrateRequestDTO objILCalibrateDTO = new ItemLocationCalibrateRequestDTO();

                                        objILCalibrateDTO.ItemGUID = (Guid)objBinDTO.ItemGUID;
                                        objILCalibrateDTO.BinID = objBinDTO.ID;
                                        objILCalibrateDTO.RoomID = SessionHelper.RoomID;
                                        objILCalibrateDTO.CompanyID = SessionHelper.CompanyID;
                                        objILCalibrateDTO.RequestType = (int)CalibrateRequestType.Calibrate;
                                        objILCalibrateDTO.IsStep1Started = false;
                                        objILCalibrateDTO.CreatedBy = SessionHelper.UserID;
                                        //if (ReqID == 4)
                                        //{
                                        //    objILCalibrateDTO.CalibrationWeight = CalibrateWeight;
                                        //}

                                        objILCalibrateDAL.InsertItemLocationCalibrateRequest(objILCalibrateDTO);

                                        Msg = objCalibrateRequest.ProcesStep1(objILCalibrateDTO.ID,
                                            SessionHelper.EnterPriseDBName, SessionHelper.EnterPriceID,
                                            SessionHelper.CompanyID, SessionHelper.RoomID,
                                            SessionHelper.UserID);

                                        calibrateRequestID = objILCalibrateDTO.ID;

                                        Msg = string.IsNullOrWhiteSpace(Msg) ? "ok" : Msg;
                                        if (Msg != "ok")
                                        {
                                            calibrateRequestID = 0;
                                        }

                                    }
                                    else if (objeVMIRequestDTO.cmd == "cal2")
                                    {
                                        Msg = objCalibrateRequest.ProcesStep2(objeVMIRequestDTO.pCalibrateRequestID,
                                            SessionHelper.EnterPriseDBName, SessionHelper.EnterPriceID,
                                            SessionHelper.CompanyID, SessionHelper.RoomID,
                                            SessionHelper.UserID);

                                        calibrateRequestID = objeVMIRequestDTO.pCalibrateRequestID;

                                        Msg = string.IsNullOrWhiteSpace(Msg) ? "ok" : Msg;

                                        if (Msg != "ok")
                                        {
                                            calibrateRequestID = 0;
                                        }

                                    }
                                    else if (objeVMIRequestDTO.cmd == "cal3")
                                    {
                                        Msg = objCalibrateRequest.ProcesStep3(objeVMIRequestDTO.pCalibrateRequestID, objeVMIRequestDTO.CalibrateWeight ?? 0,
                                              SessionHelper.EnterPriseDBName, SessionHelper.EnterPriceID,
                                             SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);

                                        calibrateRequestID = 0;

                                        Msg = string.IsNullOrWhiteSpace(Msg) ? "ok" : Msg;
                                    }

                                }


                            }
                            #endregion
                            break;
                        //case 4:
                        //    #region Calibrate Custom - Run Command for Calibrate Step 3
                        //    {

                        //        bool IsCalImmediate = true; // SessionHelper.SensorBinRoomSettings.IsCalibrateRequestImmediate();
                        //        Msg = "ok";
                        //        if (IsCalImmediate)
                        //        {

                        //            eTurns.eVMIBAL.CalibrateRequest objCalibrateRequest = new eTurns.eVMIBAL.CalibrateRequest();
                        //            Msg = objCalibrateRequest.ProcesStep3(pCalibrateRequestID, CalibrateWeight ?? 0,
                        //             SessionHelper.EnterPriseDBName, SessionHelper.EnterPriceID,
                        //             SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);
                        //        }

                        //        Msg = string.IsNullOrWhiteSpace(Msg) ? "ok" : Msg;

                        //    }
                        //    #endregion
                        //    break;
                        case 5:
                            #region WeightPerPiece
                            bool IsWeightImmediate = SessionHelper.SensorBinRoomSettings.IsItemWeightPerPieceRequestImmediate();
                            ItemWeightRequestDAL objWeightRequestDAL = new ItemWeightRequestDAL(SessionHelper.EnterPriseDBName);
                            ItemWeightPerPieceRequestDTO objItemWeightReqDTO = new ItemWeightPerPieceRequestDTO();
                            objItemWeightReqDTO.ItemGUID = (Guid)objBinDTO.ItemGUID;
                            objItemWeightReqDTO.RoomID = SessionHelper.RoomID;
                            objItemWeightReqDTO.CompanyID = SessionHelper.CompanyID;
                            objItemWeightReqDTO.IsWeightStarted = false;
                            objItemWeightReqDTO.TotalQty = objeVMIRequestDTO.TotalQTY;
                            objItemWeightReqDTO.CreatedBy = SessionHelper.UserID;
                            objItemWeightReqDTO.RequestType = IsWeightImmediate ? (int)eVMIWeightPerPieceRequestType.WeightPerPieceImmediate : (int)eVMIWeightPerPieceRequestType.WeightPerPiece;
                            var res = objWeightRequestDAL.InsertItemWeightPerPieceRequest(objItemWeightReqDTO);
                            Int64 Flag = res.ReturnFlag;

                            if (Flag == 1)
                            {
                                Msg = "ok";
                            }
                            else if (Flag == 2)
                            {
                                Msg = ResItemMaster.DefaultBinSensor;
                            }

                            if (IsWeightImmediate)
                            {
                                eTurns.eVMIBAL.ItemWeightPerPieceRequest weightPerPieceRequest = new eTurns.eVMIBAL.ItemWeightPerPieceRequest();
                                weightPerPiece = weightPerPieceRequest.GetItemWeightPerPieceProcessForItem(objItemWeightReqDTO.ID, SessionHelper.EnterPriseDBName
                                    , SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);
                            }
                            #endregion
                            break;
                        case 6:
                            #region Reset
                            bool IsResetRequestImmediate = SessionHelper.SensorBinRoomSettings.IsResetRequestImmediate();
                            EVMIResetRequestDAL resetRequestDAL = new EVMIResetRequestDAL(SessionHelper.EnterPriseDBName);
                            eVMIResetRequestDTO resetDTO = new eVMIResetRequestDTO();
                            resetDTO.ItemGUID = Guid.Empty;
                            resetDTO.RoomID = SessionHelper.RoomID;
                            resetDTO.CompanyID = SessionHelper.CompanyID;
                            resetDTO.BinID = 0;
                            resetDTO.ScaleID = objeVMIRequestDTO.ScaleID;
                            resetDTO.ComPortRoomMappingID = objeVMIRequestDTO.ComPortRoomMappingID;
                            resetDTO.CreatedBy = SessionHelper.UserID;
                            resetDTO.RequestType = IsResetRequestImmediate ? (int)eVMIResetRequestType.ResetImmediate : (int)eVMIResetRequestType.Reset;
                            resetRequestDAL.InsertEVMIResetRequest(resetDTO);
                            if (IsResetRequestImmediate)
                            {
                                // immediate process request
                                eTurns.eVMIBAL.ResetRequest resetRequest = new eTurns.eVMIBAL.ResetRequest();
                                resetRequest.ResetProcessForItem(resetDTO.ID, SessionHelper.EnterPriseDBName, SessionHelper.EnterPriceID
                                    , SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);
                            }
                            Msg = "ok";
                            #endregion
                            break;
                        case 7:
                        case 8:
                            bool IsImmediateGS = SessionHelper.SensorBinRoomSettings.IsGetShelfIDRequestImmediate();
                            bool IsImmediateSS = SessionHelper.SensorBinRoomSettings.IsSetShelfIDRequestImmediate();
                            eVMIShelfRequestDAL objDAL = new eVMIShelfRequestDAL(SessionHelper.EnterPriseDBName);
                            eVMIShelfRequestDTO objDTO = new eVMIShelfRequestDTO();
                            if (objeVMIRequestDTO.ReqID == 7)
                            {
                                objDTO.ShelfID = 0;
                                objDTO.RequestType = IsImmediateGS ? (int)eVMIShelfRequestType.GetShelfIDImmediate : (int)eVMIShelfRequestType.GetShelfID;
                            }
                            else
                            {
                                objDTO.ShelfID = (objeVMIRequestDTO.SetShelfID ?? 0);
                                objDTO.RequestType = IsImmediateSS ? (int)eVMIShelfRequestType.SetShelfIDImmediate : (int)eVMIShelfRequestType.SetShelfID;
                            }
                            objDTO.RoomID = SessionHelper.RoomID;
                            objDTO.CompanyID = SessionHelper.CompanyID;
                            objDTO.CreatedBy = SessionHelper.UserID;
                            objDTO.ComPortRoomMappingID = objeVMIRequestDTO.ComPortRoomMappingID;
                            objDAL.InsertShelfRequest(objDTO);
                            if (IsImmediateGS && objeVMIRequestDTO.ReqID == 7)
                            {
                                eTurns.eVMIBAL.ShelfRequest request = new eTurns.eVMIBAL.ShelfRequest();
                                ShelfID = request.ShelfRequestProcessForRoom(objDTO.ID, SessionHelper.EnterPriseDBName
                                      , SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);
                            }
                            if (IsImmediateSS && objeVMIRequestDTO.ReqID == 8)
                            {
                                eTurns.eVMIBAL.ShelfRequest request = new eTurns.eVMIBAL.ShelfRequest();
                                ShelfID = request.ShelfRequestProcessForRoom(objDTO.ID, SessionHelper.EnterPriseDBName
                                    , SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID);
                            }
                            Msg = "ok";
                            break;
                        case 9: // Version Request
                            {
                                bool IsRequestImmediate = SessionHelper.SensorBinRoomSettings.IsGetVersionRequestImmediate();

                                int requestType = IsRequestImmediate ? (int)eVMICOMCommonRequestType.GetFirmWareVersionImmediate : (int)eVMICOMCommonRequestType.GetFirmWareVersion;
                                var resp = insertEVMICOMCommonRequest(requestType, objeVMIRequestDTO.ScaleID, objeVMIRequestDTO.ComPortRoomMappingID, "");

                                sVersion = resp.Version;

                                Msg = "ok";
                                if (!string.IsNullOrWhiteSpace(resp.ErrorDescription))
                                {
                                    Msg = resp.ErrorDescription;
                                }
                            }
                            break;
                        case 10:// Serial No Request
                            {
                                bool IsRequestImmediate = SessionHelper.SensorBinRoomSettings.IsGetSerialNoRequestImmediate();

                                int requestType = IsRequestImmediate ? (int)eVMICOMCommonRequestType.GetSerialNoImmediate : (int)eVMICOMCommonRequestType.GetSerialNo;
                                var resp = insertEVMICOMCommonRequest(requestType, objeVMIRequestDTO.ScaleID, objeVMIRequestDTO.ComPortRoomMappingID, "");


                                SerialNo = resp.SerialNumber;

                                Msg = "ok";
                                if (!string.IsNullOrWhiteSpace(resp.ErrorDescription))
                                {
                                    Msg = resp.ErrorDescription;
                                }
                            }
                            break;
                        case 11: // get model
                            {
                                bool IsRequestImmediate = SessionHelper.SensorBinRoomSettings.IsGetModelRequestImmediate();

                                int requestType = IsRequestImmediate ? (int)eVMICOMCommonRequestType.GetModelNoImmediate : (int)eVMICOMCommonRequestType.GetModelNo;
                                var resp = insertEVMICOMCommonRequest(requestType, objeVMIRequestDTO.ScaleID, objeVMIRequestDTO.ComPortRoomMappingID, "");

                                Model = resp.ModelNumber;

                                Msg = "ok";
                                if (!string.IsNullOrWhiteSpace(resp.ErrorDescription))
                                {
                                    Msg = resp.ErrorDescription;
                                }
                            }
                            break;
                        case 12: // set model
                            {
                                bool IsRequestImmediate = SessionHelper.SensorBinRoomSettings.IsSetModelRequestImmediate();

                                int requestType = IsRequestImmediate ? (int)eVMICOMCommonRequestType.SetModelNoImmediate : (int)eVMICOMCommonRequestType.SetModelNo;
                                var resp = insertEVMICOMCommonRequest(requestType, objeVMIRequestDTO.ScaleID, objeVMIRequestDTO.ComPortRoomMappingID, objeVMIRequestDTO.ModelNumber);


                                Msg = "ok";
                                if (!string.IsNullOrWhiteSpace(resp.ErrorDescription))
                                {
                                    Msg = resp.ErrorDescription;
                                }
                            }
                            break;
                        case 13: // get calibrate weight
                            {
                                bool IsRequestImmediate = SessionHelper.SensorBinRoomSettings.IsGetCalibrationWeightRequestImmediate();

                                int requestType = IsRequestImmediate ? (int)eVMICalibrationWeightRequestType.GetCalibrationWeightImmediate : (int)eVMICalibrationWeightRequestType.GetCalibrationWeight;

                                var resp = insertEVMICalibrationWeightRequest(requestType, null, objeVMIRequestDTO.ScaleID, objeVMIRequestDTO.ComPortRoomMappingID);
                                calWeight = resp.CalibrationWeight ?? 0;
                                Msg = "ok";
                                if (!string.IsNullOrWhiteSpace(resp.ErrorDescription))
                                {
                                    Msg = resp.ErrorDescription;
                                }


                            }
                            break;
                        case 14: // set calibrate weight
                            {
                                bool IsRequestImmediate = SessionHelper.SensorBinRoomSettings.IsSetCalibrationWeightRequestImmediate();

                                int requestType = IsRequestImmediate ? (int)eVMICalibrationWeightRequestType.SetCalibrationWeightImmediate : (int)eVMICalibrationWeightRequestType.SetCalibrationWeight;

                                var resp = insertEVMICalibrationWeightRequest(requestType, objeVMIRequestDTO.CalibrateWeight, objeVMIRequestDTO.ScaleID, objeVMIRequestDTO.ComPortRoomMappingID);
                                calWeight = resp.CalibrationWeight ?? 0;
                                Msg = "ok";

                                if (!string.IsNullOrWhiteSpace(resp.ErrorDescription))
                                {
                                    Msg = resp.ErrorDescription;
                                }
                            }
                            break;

                        default:

                            Msg = "fail";
                            break;
                    }
                }
            }


            return Json(new
            {
                Status = Msg,
                WeightPerPiece = weightPerPiece,
                ShelfID = ShelfID,
                calibrateRequestID = calibrateRequestID,
                calWeight = calWeight,
                Version = sVersion,
                SerialNo = SerialNo,
                Model = Model
            }, JsonRequestBehavior.AllowGet);

        }


        private eVMICalibrationWeightRequestDTO insertEVMICalibrationWeightRequest(int requestType, double? CalibrationWeight, int ScaleID
            , long ComPortRoomMappingID
            )
        {
            eVMICalibrationWeightRequestDAL requestDAL = new eVMICalibrationWeightRequestDAL(SessionHelper.EnterPriseDBName);
            var objDTO = new eVMICalibrationWeightRequestDTO();

            objDTO.ScaleID = ScaleID;
            objDTO.RoomID = SessionHelper.RoomID;
            objDTO.CompanyID = SessionHelper.CompanyID;

            objDTO.CalibrationWeight = CalibrationWeight;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.RequestType = requestType;
            objDTO.ComPortRoomMappingID = ComPortRoomMappingID;

            requestDAL.InsertEVMICalibrationWeightRequest(objDTO);
            long id = objDTO.ID;

            // if immediate then process request
            var requestTypeEnum = eVMICalibrationWeightRequestType.GetCalibrationWeight;
            bool parsedEnum = Enum.TryParse<eVMICalibrationWeightRequestType>(requestType.ToString(), out requestTypeEnum);

            eVMICalibrationWeightRequestDTO resp = new eVMICalibrationWeightRequestDTO();
            if (parsedEnum)
            {
                var processRequest = new eTurns.eVMIBAL.CalibrationWeightRequest();

                switch (requestTypeEnum)
                {
                    case eVMICalibrationWeightRequestType.GetCalibrationWeightImmediate:
                        resp = processRequest.ProcessCaliWeightRequestForScale(objDTO.ID, SessionHelper.EnterPriseDBName, SessionHelper.EnterPriceID
                            , SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, eVMICalibrationWeightRequestType.GetCalibrationWeightImmediate);
                        break;
                    case eVMICalibrationWeightRequestType.SetCalibrationWeightImmediate:
                        resp = processRequest.ProcessCaliWeightRequestForScale(objDTO.ID, SessionHelper.EnterPriseDBName, SessionHelper.EnterPriceID
                            , SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, eVMICalibrationWeightRequestType.SetCalibrationWeightImmediate);
                        break;
                }
            }

            return resp;
        }

        private eVMICommonReqDTO insertEVMICOMCommonRequest(int requestType, int ScaleID
            , long ComPortRoomMappingID, string ModelNumber)
        {
            eVMICOMCommonRequestDAL requestDAL = new eVMICOMCommonRequestDAL(SessionHelper.EnterPriseDBName);
            var objDTO = new eVMICOMCommonRequestDTO();
            objDTO.ItemGUID = Guid.Empty;
            objDTO.BinID = 0;
            objDTO.RoomID = SessionHelper.RoomID;
            objDTO.CompanyID = SessionHelper.CompanyID;
            objDTO.CreatedBy = SessionHelper.UserID;
            objDTO.RequestType = requestType;
            objDTO.ScaleID = ScaleID;
            objDTO.ModelNumber = ModelNumber;
            objDTO.ComPortRoomMappingID = ComPortRoomMappingID;

            requestDAL.InsertEVMICOMCommonRequest(objDTO);
            long id = objDTO.ID;

            // if immediate then process request
            var requestTypeEnum = eVMICOMCommonRequestType.GetFirmWareVersion;
            bool parsedEnum = Enum.TryParse<eVMICOMCommonRequestType>(requestType.ToString(), out requestTypeEnum);

            var commonRequest = new eTurns.eVMIBAL.EVMICommonRequest();
            eVMICommonReqDTO req = new eVMICommonReqDTO();

            if (parsedEnum)
            {
                switch (requestTypeEnum)
                {
                    case eVMICOMCommonRequestType.GetFirmWareVersionImmediate:
                    case eVMICOMCommonRequestType.GetSerialNoImmediate:
                    case eVMICOMCommonRequestType.GetModelNoImmediate:
                    case eVMICOMCommonRequestType.SetModelNoImmediate:
                        req = commonRequest.ProcessCommonRequestForScale(objDTO.ID, SessionHelper.EnterPriseDBName,
                            SessionHelper.EnterPriceID,
                            SessionHelper.CompanyID, SessionHelper.RoomID, SessionHelper.UserID, requestTypeEnum);
                        break;
                }
            }

            return req;
        }

        [HttpGet]
        public JsonResult GeteVMISensorID(string ItemLocationName, Int64 ItemLocationID)
        {
            string Msg = string.Empty;
            string streVMISensorID = string.Empty;
            if (SessionHelper.isEVMI == true)
            {
                BinMasterDAL objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                BinMasterDTO objBinDTO = objBinDAL.GetBinByID(ItemLocationID, SessionHelper.RoomID, SessionHelper.CompanyID);

                if (objBinDTO.eVMISensorID != null)
                    streVMISensorID = Convert.ToString(objBinDTO.eVMISensorID);

                Msg = "ok";

            }

            return Json(new
            {
                Status = Msg,
                eVMISensorID = streVMISensorID,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetItemBinsbyComPort(long ComPortRoomMappingID)
        {
            //ComPortRoomMappingDAL objComPortRoomMappingDAL = new ComPortRoomMappingDAL(SessionHelper.EnterPriseDBName);
            //ComPortRoomMappingDTO objComPortRoomMappingDTO = objComPortRoomMappingDAL.GetComPortMappingByID(ComPortRoomMappingID);
            string Msg = string.Empty;
            List<BinMasterDTO> lstBinMaster = new List<BinMasterDTO>();
            List<int> lstScales = new List<int>();
            if (SessionHelper.isEVMI == true)
            {
                BinMasterDAL objBinDAL = new BinMasterDAL(SessionHelper.EnterPriseDBName);
                lstBinMaster = objBinDAL.GetItemLocatoinForeVMIByCOMPort(SessionHelper.RoomID, SessionHelper.CompanyID, ComPortRoomMappingID).ToList();
                if (lstBinMaster != null && lstBinMaster.Count > 0)
                {
                    lstScales = lstBinMaster.Select(t => Convert.ToInt32((t.eVMISensorID ?? 0))).Distinct().Where(t => (t > 0 && t < 1000)).ToList();
                }
                Msg = "ok";
            }

            return Json(new
            {
                Status = Msg,
                ItemBins = lstBinMaster,
                Scales = lstScales,
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult PermissionTemplateHistory(string ID)
        {
            ViewBag.TemplateID = ID;
            return PartialView("_PermissionTemplateHistory");
        }

    }


    class PointStructure
    {
        public String X { get; set; }
        public String Y { get; set; }
    }

    /// <summary>
    /// Class that encapsulates most common parameters sent by DataTables plugin
    /// </summary>
    public class JQueryDataTableParamModel
    {
        public string draw { get; set; }

        /// <summary>
        /// Request sequence number sent by DataTable, same value must be returned in response
        /// </summary>       
        public string sEcho { get; set; }

        /// <summary>
        /// Text used for filtering
        /// </summary>
        public string sSearch { get; set; }

        /// <summary>
        /// Number of records that should be shown in table
        /// </summary>
        public int iDisplayLength { get; set; }

        /// <summary>
        /// First record that should be shown(used for paging)
        /// </summary>
        public int iDisplayStart { get; set; }

        /// <summary>
        /// Number of columns in table
        /// </summary>
        public int iColumns { get; set; }

        /// <summary>
        /// Number of columns that are used in sorting
        /// </summary>
        public int iSortingCols { get; set; }

        /// <summary>
        /// Comma separated list of column names
        /// </summary>
        public string sColumns { get; set; }

        public bool IsNarroSearchClear { get; set; }

    }

    public class ListValues
    {
        public int intValue { get; set; }
        public float fltValue { get; set; }
        public string Description { get; set; }
    }
}