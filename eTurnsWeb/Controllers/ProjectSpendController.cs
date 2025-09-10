using eTurns.DAL;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurnsWeb.Helper;
using eTurnsWeb.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace eTurnsWeb.Controllers
{
    [AuthorizeHelper]
    public class ProjectSpendController : eTurnsControllerBase
    {
        #region "Project Master"

        public ActionResult ProjectList()
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            SelectListItem items = new SelectListItem();
            items.Value = "100";
            items.Text = ResProjectMaster.MoreThan100Percentage; 
            lst.Add(items);
            items = new SelectListItem();
            items.Value = "90";
            items.Text = ResProjectMaster.MoreThan90Percentage;
            lst.Add(items);
            items = new SelectListItem();
            items.Value = "75";
            items.Text = ResProjectMaster.MoreThan75Percentage;
            lst.Add(items);
            items = new SelectListItem();
            items.Value = "50";
            items.Text = ResProjectMaster.MoreThan50Percentage;
            lst.Add(items);
            IEnumerable<SelectListItem> TotalSpendLimitInPer = lst;


            lst = new List<SelectListItem>();
            items = new SelectListItem();
            items.Value = "10000-0";
            items.Text = ResProjectMaster.MoreThan10000;
            lst.Add(items);
            items = new SelectListItem();
            items.Value = "7500-9999";
            items.Text = ResProjectMaster.Between7500To9999;
            lst.Add(items);
            items = new SelectListItem();
            items.Value = "5000-7499";
            items.Text = ResProjectMaster.Between5000To7499;
            lst.Add(items);
            items = new SelectListItem();
            items.Value = "2500-4999";
            items.Text = ResProjectMaster.Between2500To4999;
            lst.Add(items);
            items = new SelectListItem();
            items.Value = "0-2500";
            items.Text = ResProjectMaster.LessThan2500;
            lst.Add(items);
            IEnumerable<SelectListItem> TotalSpendLimitInDlr = lst;


            lst = new List<SelectListItem>();
            items = new SelectListItem();
            items.Value = "10000,-1";
            items.Text = ResProjectMaster.MoreThan10000;
            lst.Add(items);
            items = new SelectListItem();
            items.Value = "7500,9999";
            items.Text = ResProjectMaster.Between7500To9999;
            lst.Add(items);
            items = new SelectListItem();
            items.Value = "5000,7499";
            items.Text = ResProjectMaster.Between5000To7499;
            lst.Add(items);
            items = new SelectListItem();
            items.Value = "2500,4999";
            items.Text = ResProjectMaster.Between2500To4999;
            lst.Add(items);
            items = new SelectListItem();
            items.Value = "0,2500";
            items.Text = ResProjectMaster.BetweenZeroTo2500;
            lst.Add(items);
            items = new SelectListItem();
            items.Value = "-1,0";
            items.Text = ResProjectMaster.LessThanZero;
            lst.Add(items);
            IEnumerable<SelectListItem> TotalSpendLimitRemaingInDlr = lst;

            lst = new List<SelectListItem>();
            items = new SelectListItem();
            items.Value = "10000,-1";
            items.Text = ResProjectMaster.MoreThan10000;
            lst.Add(items);
            items = new SelectListItem();
            items.Value = "7500,9999";
            items.Text = ResProjectMaster.Between7500To9999;
            lst.Add(items);
            items = new SelectListItem();
            items.Value = "5000,7499";
            items.Text = ResProjectMaster.Between5000To7499;
            lst.Add(items);
            items = new SelectListItem();
            items.Value = "2500,4999";
            items.Text = ResProjectMaster.Between2500To4999;
            lst.Add(items);
            items = new SelectListItem();
            items.Value = "0,2499";
            items.Text = ResProjectMaster.LessThan2500;
            lst.Add(items);
            IEnumerable<SelectListItem> TotalItemSpendLimitInDlr = lst;

            ViewBag.TotalSpendLimitInPer = TotalSpendLimitInPer;
            ViewBag.TotalSpendLimitInDlr = TotalSpendLimitInDlr;
            ViewBag.TotalSpendLimitRemaingInDlr = TotalSpendLimitRemaingInDlr;
            ViewBag.TotalItemSpendLimitInDlr = TotalItemSpendLimitInDlr;


            return View();

        }

        /// <summary>
        ///  GET: /Master/ for Create
        /// </summary>
        /// <returns></returns>
        public ActionResult ProjectCreate()
        {
            //AutoSequenceNumbersDTO objAutoSequenceDTO = new AutoSequenceDAL(SessionHelper.EnterPriseDBName).GetLastGeneratedID(SessionHelper.RoomID, SessionHelper.CompanyID, "#P");
            //string NewNumber = objAutoSequenceDTO.Prefix + "-" + SessionHelper.CompanyID + "-" + SessionHelper.RoomID + "-" + DateTime.Now.ToString("MMddyy") + "-" + objAutoSequenceDTO.LastGenereted;
            //string NewNumber = new AutoSequenceDAL(SessionHelper.EnterPriseDBName).GetLastGeneratedROOMID("NextProjectSpendNo", SessionHelper.RoomID, SessionHelper.CompanyID).ToString();
            string NewNumber = new AutoSequenceDAL(SessionHelper.EnterPriseDBName).GetNextAutoNumberByModule("NextProjectSpendNo", SessionHelper.RoomID, SessionHelper.CompanyID);

            ProjectMasterDTO objDTO = new ProjectMasterDTO();
            //objDTO.ProjectSpendName = "#P" + NewNumber;
            objDTO.ProjectSpendName = NewNumber;
            objDTO.ID = 0;
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
            UDFController objUDFDAL = new UDFController();
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("ProjectMaster");
            foreach (var i in ViewBag.UDFs)
            {
                string _UDFColumnName = (string)i.UDFColumnName;
                ViewData[_UDFColumnName] = i.UDFDefaultValue;
            }
            IEnumerable<SelectListItem> list = new SelectList(new[]
                                          {
                                              new {ID="0",Name="No"},
                                              new{ID="1",Name="Yes"},
                                          },
                        "ID", "Name", 0);
            ViewData["list"] = list;

            ///Get any project spend have true trackingallagainsthis?
            //ProjectMasterDTO ProjClose = new ProjectMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(t => t.TrackAllUsageAgainstThis == true).FirstOrDefault();
            //if (ProjClose != null)
            //{
            //    ViewBag.AllowCloseProjectSpent = (ProjClose.TrackAllUsageAgainstThis == true ? "0" : "1");
            //}
            //else
            //{
            //    ViewBag.AllowCloseProjectSpent = "1";
            //}
            return PartialView("_CreateProject", objDTO);
            //return View();
        }

        /// <summary>
        /// JSON Record Save - Enter key Save/Update
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Technician"></param>
        /// <returns></returns>
        /// 

        [ValidateAntiForgeryToken]
        public JsonResult ProjectSave(ProjectMasterDTO objDTO)
        {
            string message = "";
            string status = "";

            //ProjectMasterController obj = new ProjectMasterController();
            ProjectMasterDAL obj = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
            CommonDAL objCDAL = new CommonDAL(SessionHelper.EnterPriseDBName);
            objDTO.LastUpdatedBy = SessionHelper.UserID;
            objDTO.UpdatedByName = SessionHelper.UserName;
            objDTO.Room = SessionHelper.RoomID;
            objDTO.CompanyID = SessionHelper.CompanyID;

            if (string.IsNullOrEmpty(objDTO.ProjectSpendName))
            {
                message = string.Format(ResMessage.Required, ResProjectMaster.ProjectSpendName);// "ProjectName name is required.";
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }

            if (objDTO.ID == 0)
            {
                objDTO.CreatedBy = SessionHelper.UserID;
                objDTO.CreatedByName = SessionHelper.UserName;

                //10/3/2013
                //Should be able to add a New Project Spend even if "Track all usage against this Project" is checked in an existing Project.
                //Should NOT close an open Project by creating a New Project and checking "Track all usage against thie Project".  User may want to track usage against a Project for a month and then switch to another and then switch back.
                //if (obj.GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(t => t.TrackAllUsageAgainstThis == true).Count() > 0)
                //{
                //    message = ResProjectMaster.OneTrackAllUsageAgainstThisAlreadyThere;
                //    status = "fail";
                //}
                //else
                //{
                string strOK = objCDAL.DuplicateCheck(objDTO.ProjectSpendName, "add", objDTO.ID, "ProjectMaster", "ProjectSpendName", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResProjectMaster.ProjectSpendName, objDTO.ProjectSpendName);  //"ProjectName \"" + objDTO.ProjectName + "\" already exist! Try with Another!";
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
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                        status = "fail";
                    }
                }
                //}
            }
            else
            {
                string strOK = objCDAL.DuplicateCheck(objDTO.ProjectSpendName, "edit", objDTO.ID, "ProjectMaster", "ProjectSpendName", SessionHelper.RoomID, SessionHelper.CompanyID);
                if (strOK == "duplicate")
                {
                    message = string.Format(ResMessage.DuplicateMessage, ResProjectMaster.ProjectSpendName, objDTO.ProjectSpendName);  //"ProjectName \"" + objDTO.ProjectName + "\" already exist! Try with Another!";
                    status = "duplicate";
                }
                else
                {
                    objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                    objDTO.EditedFrom = "Web";

                    bool ReturnVal = obj.Edit(objDTO);
                    if (ReturnVal)
                    {
                        message = ResMessage.SaveMessage; //"Record Saved Sucessfully...";
                        status = "ok";
                    }
                    else
                    {
                        message = string.Format(ResMessage.SaveErrorMsg, HttpStatusCode.ExpectationFailed); // "(" + hrmResult.StatusCode + ") Error! Record Not Saved";
                        status = "fail";

                    }
                }
            }

            Session["IsInsert"] = "True";
            return Json(new { Message = message, Status = status, ID = objDTO.ID }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// for Edit
        /// </summary>
        /// <returns></returns>
        public ActionResult ProjectEdit(Int64 ID)
        {
            bool IsArchived = bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = bool.Parse(Request["IsDeleted"].ToString());
            if (IsDeleted || IsArchived)
            {
                ViewBag.ViewOnly = true;
            }

            //ProjectMasterController obj = new ProjectMasterController();
            ProjectMasterDAL obj = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
            ProjectMasterDTO objDTO = obj.GetProjectMasterByIDNormal(ID);

            UDFController objUDFDAL = new UDFController();
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("ProjectMaster");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;

            /////Get any project spend have true trackingallagainsthis?
            //ProjectMasterDTO ProjClose = new ProjectMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(t => t.TrackAllUsageAgainstThis == true).SingleOrDefault();
            //if (ProjClose != null)
            //{
            //    ViewBag.AllowCloseProjectSpent = (ProjClose.TrackAllUsageAgainstThis == true ? "0" : "1");
            //}
            //else
            //{
            //    ViewBag.AllowCloseProjectSpent = "1";
            //}


            return PartialView("_CreateProject", objDTO);
        }

        /// <summary>
        /// Update Project 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <param name="rowId"></param>
        /// <param name="columnPosition"></param>
        /// <param name="columnId"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string UpdateProjectData(long id, string value, int? rowId, int? columnPosition, int? columnId, string columnName)
        {
            ProjectMasterDAL obj = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
            obj.UpdateProjectSpend(id, value, columnName, SessionHelper.UserID);
            return value;
        }

        ///// <summary>
        ///// Duplicate Project Check
        ///// </summary>
        ///// <param name="ProjectName"></param>
        ///// <param name="ActionMode"></param>
        ///// <param name="ID"></param>
        ///// <returns></returns>
        //public string DuplicateProjectCheck(string ProjectName, string ActionMode, int ID)
        //{
        //    ProjectMasterController obj = new ProjectMasterController();
        //    return obj.DuplicateCheck(ProjectName, ActionMode, ID);
        //}

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public JsonResult DeleteProjectRecords(string ids)
        {
            try
            {
                //ProjectMasterController obj = new ProjectMasterController();
                ProjectMasterDAL obj = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
                obj.DeleteProjectSpend(ids, SessionHelper.UserID);
                //return "ok";
                return Json(new { Message = ResMessage.DeletedSuccessfully, Status = "ok" }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                //return ex.Message;
                return Json(new { Message = ex.Message, Status = "ok" }, JsonRequestBehavior.AllowGet);

            }
        }

        #region Ajax Data Provider

        /// <summary>
        /// Below method used to bind the data to jquery data table
        /// </summary>
        /// <param name="param"></param> param has different type of paramterers which used for sorting filtering etc ...
        /// <returns></returns>
        public ActionResult GetProjectList(JQueryDataTableParamModel param)
        {
            //ProjectMasterController obj = new ProjectMasterController();
            ProjectMasterDAL obj = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);

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
            //if (string.IsNullOrEmpty(sortColumnName) || sortColumnName == "0" || sortColumnName == "undefined" || sortColumnName.Trim() == "asc")
            //    sortColumnName = "ID";

            if (!string.IsNullOrEmpty(sortColumnName))
            {
                if (sortColumnName.Contains("null") || sortColumnName == "0" || sortColumnName.Contains("undefined") || sortColumnName.Trim() == "asc" || sortColumnName.Trim() == "desc")
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
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetPagedProjectMasterRecords(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, Convert.ToString(SessionHelper.RoomDateFormat), CurrentTimeZone);

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
        /// // below function used to load the User wise saved filter data for jquery data table
        /// </summary>
        /// <param name="UserID"></param> will have userid for currently logged in user
        /// <returns></returns>
        //public ActionResult LoadProjectGridState(int UserID, string ListName)
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
        //public ActionResult SaveProjectGridState(int UserID, string Data, string ListName)
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

        /// <summary>
        /// Create/Edit Page's Items - Refector
        /// </summary>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        public ActionResult LoadProjectItems(Guid ProjectGUID)
        {
            ViewBag.ProjectGUID = ProjectGUID;
            ProjectSpendItemsDAL obj = new ProjectSpendItemsDAL(SessionHelper.EnterPriseDBName);
            //List<ProjectSpendItemsDTO> objDTO = obj.GetAllRecords(ProjectGUID, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserSupplierIds).Where(t => t.IsArchived == false && t.IsDeleted == false).ToList();
            List<ProjectSpendItemsDTO> objDTO = obj.GetProjectSpendItem(ProjectGUID, SessionHelper.RoomID, SessionHelper.CompanyID, SessionHelper.UserSupplierIds, string.Empty).ToList();
            return PartialView("_CreateProjectItems", objDTO);
        }

        #endregion

        public ActionResult ProjectHistory()
        {
            return PartialView("_ProjectHistory");
        }

        public ActionResult LoadItemMasterModel(string ParentId, string ParentGuid)
        {
            ItemModelPerameter obj = new ItemModelPerameter()
            {
                AjaxURLAddItemToSession = "~/ProjectSpend/AddItemToDetailTable/",
                PerentID = ParentId,
                PerentGUID = ParentGuid,
                ModelHeader = eTurns.DTO.ResProjectMaster.ModelHeader,
                AjaxURLAddMultipleItemToSession = "~/ProjectSpend/AddItemToDetailTable/",
                AjaxURLToFillItemGrid = "~/ProjectSpend/GetItemsModelMethod/",
                CallingFromPageName = "PS"
            };

            return PartialView("ItemMasterModel", obj);
        }

        public ActionResult GetItemsModelMethod(JQueryDataTableParamModel param)
        {
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
            //make changes to resolve an issue of Sort (WI-431)
            if (sortColumnName == "0" || sortColumnName.Contains("undefined"))
                sortColumnName = "ItemNumber Asc";

            string searchQuery = string.Empty;
            Guid QLID = Guid.Empty;
            Guid.TryParse(Request["ParentGuid"], out QLID);
            int TotalRecordCount = 0;
            var tmpsupplierIds = new List<long>();
            //List<ProjectSpendItemsDTO> objQLItems = new ProjectSpendItemsDAL(SessionHelper.EnterPriseDBName).GetAllRecords(QLID, SessionHelper.RoomID, SessionHelper.CompanyID, tmpsupplierIds).ToList();
            List<ProjectSpendItemsDTO> objQLItems = new ProjectSpendItemsDAL(SessionHelper.EnterPriseDBName).GetProjectSpendItem(QLID, SessionHelper.RoomID, SessionHelper.CompanyID, tmpsupplierIds, string.Empty).ToList();
            string ItemsIDs = "";

            if (objQLItems != null && objQLItems.Count > 0)
            {
                foreach (var item in objQLItems)
                {
                    if (!string.IsNullOrEmpty(ItemsIDs))
                        ItemsIDs += ",";

                    ItemsIDs += item.ItemGUID.ToString();
                }
            }

            // .Where(x=>x.ItemType != 4); , as Labour Type item not required in this module
            string RoomDateFormat = Convert.ToString(SessionHelper.RoomDateFormat);
            TimeZoneInfo CurrentTimeZone = (SessionHelper.CurrentTimeZone);
            var DataFromDB = obj.GetPagedRecordsForModel(param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, SessionHelper.CompanyID, IsArchived, IsDeleted, ItemsIDs, "labor", "1", SessionHelper.UserSupplierIds, RoomDateFormat, CurrentTimeZone, "", true).ToList();

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
            JavaScriptSerializer s = new JavaScriptSerializer();
            ProjectSpendItemsDTO[] QLDetails = s.Deserialize<ProjectSpendItemsDTO[]>(para);
            ProjectSpendItemsDAL objApi = new ProjectSpendItemsDAL(SessionHelper.EnterPriseDBName);

            foreach (ProjectSpendItemsDTO item in QLDetails)
            {
                item.Room = SessionHelper.RoomID;
                item.RoomName = SessionHelper.RoomName;
                item.CreatedBy = SessionHelper.UserID;
                item.CreatedByName = SessionHelper.UserName;
                item.UpdatedByName = SessionHelper.UserName;
                item.LastUpdatedBy = SessionHelper.UserID;
                item.CompanyID = SessionHelper.CompanyID;

                if (item.Description != null)
                {
                    if (item.Description == "null")
                    {
                        item.Description = string.Empty;
                    }
                }

                if (item.GUID != null && Guid.Parse(item.GUID.ToString()) != Guid.Empty)
                {
                    item.ReceivedOn = DateTimeUtility.DateTimeNow;
                    item.EditedFrom = "Web";
                    objApi.Edit(item);
                }
                else
                {
                    //var tmpsupplierIds = new List<long>();
                    var PSItemsRecordCount = objApi.GetProjectSpendItemsCount(item.ProjectGUID.GetValueOrDefault(Guid.Empty), SessionHelper.RoomID, SessionHelper.CompanyID, item.ItemGUID.GetValueOrDefault(Guid.Empty));
                    if (PSItemsRecordCount < 1)
                        objApi.Insert(item);
                }
            }

            message = ResAssetMaster.MsgItemQtyUpdated;
            status = "ok";
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        public string ProjectItems(Guid ProjectGUID,bool IsDelted)
        {
            ViewBag.ProjectGUID = ProjectGUID;
            ViewBag.IsDelted = IsDelted;
            ProjectSpendItemsDAL objProjAPI = new ProjectSpendItemsDAL(SessionHelper.EnterPriseDBName);
            var tmpsupplierIds = new List<long>();
            //var ObjProject = objProjAPI.GetAllRecords(ProjectGUID, SessionHelper.RoomID, SessionHelper.CompanyID, tmpsupplierIds).ToList();
            var ObjProject = objProjAPI.GetProjectSpendItem(ProjectGUID, SessionHelper.RoomID, SessionHelper.CompanyID, tmpsupplierIds, string.Empty).ToList();
            return RenderRazorViewToString("_ProjectItems", ObjProject);
        }

        public ActionResult GetProjectSpendItemsAjax(JQueryDataTableParamModel param)
        {
            Guid ProjectSpendGUID = Guid.Parse(Request["ItemID"].ToString());
            ProjectSpendItemsDAL obj = new ProjectSpendItemsDAL(SessionHelper.EnterPriseDBName);
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

            bool IsArchived = false;//bool.Parse(Request["IsArchived"].ToString());
            bool IsDeleted = false;//bool.Parse(Request["IsDeleted"].ToString());


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

            //if (sortDirection == "asc")
            //    sortColumnName = sortColumnName + " asc";
            //else
            //    sortColumnName = sortColumnName + " desc";

            string searchQuery = string.Empty;

            int TotalRecordCount = 0;

            var DataFromDB = obj.GetPagedProjectSpendItems(ProjectSpendGUID, param.iDisplayStart, param.iDisplayLength, out TotalRecordCount, param.sSearch, sortColumnName, SessionHelper.RoomID, 
                                                SessionHelper.CompanyID, IsArchived, IsDeleted, Convert.ToString(Session["RoomDateFormat"]), Convert.ToString(Session["CurrentTimeZone"]));
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalRecordCount,
                iTotalDisplayRecords = TotalRecordCount, //filteredCompanies.Count(),
                aaData = DataFromDB
            },
                        JsonRequestBehavior.AllowGet);
        }

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

        /// <summary>
        /// Method called but plugin when a row is deleted
        /// </summary>
        /// <param name="id">Id of the row</param>
        /// <returns>"ok" if delete is successfully performed - any other value will be considered as an error mesage on the client-side</returns>
        public string ProjectItemsDelete(string ids)
        {
            try
            {
                //ProjectSpendItemsController obj = new ProjectSpendItemsController();
                ProjectSpendItemsDAL obj = new ProjectSpendItemsDAL(SessionHelper.EnterPriseDBName);
                obj.DeleteProjectSpendItemsByGuids(ids, SessionHelper.UserID);
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpPost]
        public JsonResult ProjectItemsSave(string para)
        {
            string message = "";
            string status = "";
            JavaScriptSerializer s = new JavaScriptSerializer();
            try
            {
                StringBuilder sbItemLocationMSG = new StringBuilder();
                //var data = { "ID": tempID, "QuantityLimit": tempQuantityLimit, "DollarLimitAmount": tempDollarLimitAmount, "ItemID": tempItemID, "ItemGUID": itemGUID, "ProjectID": tempProjectID, "ProjectGUID": tempProjectGUID, UDF1 : vUDF1, UDF2 : vUDF2, UDF3 : vUDF3, UDF4 : vUDF4, UDF5 : vUDF5};
                ProjectSpendItemsDTO[] projectspenditems = s.Deserialize<ProjectSpendItemsDTO[]>(para);

                if (projectspenditems != null && projectspenditems.Length > 0)
                {
                    ProjectSpendItemsDAL objPSI = new ProjectSpendItemsDAL(SessionHelper.EnterPriseDBName);
                    foreach (ProjectSpendItemsDTO item in projectspenditems)
                    {

                        ProjectSpendItemsDTO objDTO = objPSI.GetProjectSpendItemByGuidPlain(item.GUID);
                        objDTO.ID = item.ID;
                        objDTO.ItemGUID = item.ItemGUID;
                        objDTO.QuantityLimit = item.QuantityLimit;
                        objDTO.DollarLimitAmount = item.DollarLimitAmount;
                        objDTO.ProjectGUID = item.ProjectGUID;
                        objDTO.UDF1 = item.UDF1;
                        objDTO.UDF2 = item.UDF2;
                        objDTO.UDF3 = item.UDF3;
                        objDTO.UDF4 = item.UDF4;
                        objDTO.UDF5 = item.UDF5;
                        objDTO.LastUpdated = DateTimeUtility.DateTimeNow;
                        objDTO.LastUpdatedBy = SessionHelper.UserID;
                        objDTO.CompanyID = SessionHelper.CompanyID;
                        objDTO.Room = SessionHelper.RoomID;
                        objDTO.CreatedBy = SessionHelper.UserID;

                        try
                        {
                            objDTO.ReceivedOn = DateTimeUtility.DateTimeNow;
                            objDTO.EditedFrom = "Web";
                            objPSI.Edit(objDTO);
                            message = ResMessage.SaveMessage;
                            status = "ok";
                        }
                        catch
                        {
                            message = ResMessage.SaveErrorMsg;
                            status = "fail";
                        }
                    }
                }
                message = ResMessage.SaveMessage;
                status = "ok";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                message = ResCommon.ErrorOccurred;
                status = "fail";
                return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult BlankSession()
        {
            Session["IsInsert"] = "";
            string message = "Session blank successfully";
            string status = "ok";
            return Json(new { Message = message, Status = status }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProjectSpend()
        {
            ProjectMasterDAL objDAL = null;
            IEnumerable<ProjectMasterDTO> lstDTO = null;
            Int64 RoomID = SessionHelper.RoomID;
            Int64 CompanyID = SessionHelper.CompanyID;
            List<DTOForAutoComplete> returnKeyValList = new List<DTOForAutoComplete>();
            try
            {
                objDAL = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
                var projectTrackAllUsageAgainstThis = objDAL.GetDefaultProjectSpendRecord(RoomID, CompanyID, false, false);

                if (projectTrackAllUsageAgainstThis != null && projectTrackAllUsageAgainstThis.GUID != Guid.Empty)
                {
                    var project = new List<ProjectMasterDTO>();
                    project.Add(projectTrackAllUsageAgainstThis);
                    lstDTO = project;
                }
                else
                {
                    lstDTO = objDAL.GetAllProjectMasterByRoomPlain(RoomID, CompanyID, false, false);
                }

                if (lstDTO != null && lstDTO.Count() > 0)
                {
                    foreach (var item in lstDTO)
                    {
                        DTOForAutoComplete obj = new DTOForAutoComplete()
                        {
                            Key = item.ProjectSpendName,
                            Value = item.ProjectSpendName,
                            ID = item.ID,
                            GUID = item.GUID,
                            OtherInfo1 = Convert.ToString(item.TrackAllUsageAgainstThis)
                        };
                        returnKeyValList.Add(obj);
                    }
                }

                returnKeyValList = returnKeyValList.OrderBy(x => x.Value).ToList();
                return Json(returnKeyValList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(returnKeyValList, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                objDAL = null;
                lstDTO = null;
            }
        }
        #region Project Spend History
        //private object GetUDFDataPageWise(string PageName)
        //{
        //    //UDFApiController objUDFApiController = new UDFApiController();
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
        public ActionResult ProjectSpendHistoryView(Int64 ID)
        {

            ProjectMasterDAL obj = new ProjectMasterDAL(SessionHelper.EnterPriseDBName);
            ProjectMasterDTO objDTO = obj.GetProjectHistoryByIdNormal(ID);

            UDFController objUDFDAL = new UDFController();
            ViewBag.UDFs = objUDFDAL.GetUDFDataPageWise("ProjectMaster");
            ViewData["UDF1"] = objDTO.UDF1;
            ViewData["UDF2"] = objDTO.UDF2;
            ViewData["UDF3"] = objDTO.UDF3;
            ViewData["UDF4"] = objDTO.UDF4;
            ViewData["UDF5"] = objDTO.UDF5;

            ///Get any project spend have true trackingallagainsthis?
            //ProjectMasterDTO ProjClose = new ProjectMasterDAL(SessionHelper.EnterPriseDBName).GetAllRecords(SessionHelper.RoomID, SessionHelper.CompanyID, false, false).Where(t => t.TrackAllUsageAgainstThis == true).SingleOrDefault();
            //if (ProjClose != null)
            //{
            //    ViewBag.AllowCloseProjectSpent = (ProjClose.TrackAllUsageAgainstThis == true ? "0" : "1");
            //}
            //else
            //{
            //    ViewBag.AllowCloseProjectSpent = "1";
            //}

            return PartialView("_CreateProject_History", objDTO);
        }

        /// <summary>
        /// LoadOrderLineItemsHistory
        /// </summary>
        /// <param name="historyID"></param>
        /// <returns></returns>
        public ActionResult LoadProjectSpendLineItemsHistory(Int64 historyID)
        {
            ProjectMasterDTO objDTO = null;
            objDTO = new ProjectMasterDAL(SessionHelper.EnterPriseDBName).GetProjectHistoryByIdNormal(historyID);
            if (objDTO != null)
                objDTO.ProjectSpendItems = new ProjectSpendItemsDAL(SessionHelper.EnterPriseDBName).GetHistoryRecordByProjectID(objDTO.GUID).ToList();

            return PartialView("_CreateProjectItems_History", objDTO);
        }

        #endregion
    }
}
